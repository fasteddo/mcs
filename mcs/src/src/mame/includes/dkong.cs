// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using int8_t = System.SByte;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;


namespace mame
{
    partial class dkong_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK            = new XTAL(61440000);
        static readonly XTAL CLOCK_1H                = MASTER_CLOCK / 5 / 4;
        static readonly XTAL CLOCK_16H               = CLOCK_1H / 16;
        static readonly XTAL CLOCK_1VF               = CLOCK_16H / 12 / 2;
        // moved to \audio\dkong.cs so that it's init before used
        //static readonly XTAL CLOCK_2VF               = CLOCK_1VF / 2;

        static readonly XTAL PIXEL_CLOCK             = MASTER_CLOCK / 10;
        const u16 HTOTAL                  = 384;
        const u16 HBSTART                 = 256;
        const u16 HBEND                   = 0;
        const u16 VTOTAL                  = 264;
        const u16 VBSTART                 = 240;
        const u16 VBEND                   = 16;

        static readonly XTAL I8035_CLOCK             = new XTAL(6000000);


        /****************************************************************************
        * CONSTANTS
        ****************************************************************************/

        //#define HARDWARE_TYPE_TAG       "HARDWARE_TYPE"

        //enum
        //{
        const uint8_t HARDWARE_TKG04 = 0;
        const uint8_t HARDWARE_TRS01 = 1;
        const uint8_t HARDWARE_TRS02 = 2;
        const uint8_t HARDWARE_TKG02 = 3;
        //};

        //enum
        //{
        const int8_t DKONG_RADARSCP_CONVERSION = 0;
        const int8_t DKONG_BOARD = 1;
        //};


        //enum
        //{
        //    DK2650_HERBIEDK = 0,
        //    DK2650_HUNCHBKD,
        //    DK2650_EIGHTACT,
        //    DK2650_SHOOTGAL,
        //    DK2650_SPCLFORC
        //};


        const u32 DK2B_PALETTE_LENGTH     = 256+256+8+1; /*  (256) */
        //#define DK4B_PALETTE_LENGTH     (256+256+8+1) /*  (256) */
        //#define DK3_PALETTE_LENGTH      (256+256+8+1) /*  (256) */
        //#define RS_PALETTE_LENGTH       (256+256+8+1)


        /* devices */
        required_device<cpu_device> m_maincpu;
        optional_device<mcs48_cpu_device> m_soundcpu;
        optional_device<eeprom_serial_93cxx_device> m_eeprom;
        optional_device<intref> m_dev_n2a03a; /* dkong3 */  //optional_device<n2a03_device> m_dev_n2a03a; /* dkong3 */
        optional_device<intref> m_dev_n2a03b; /* dkong3 */  //optional_device<n2a03_device> m_dev_n2a03b; /* dkong3 */
        optional_device<latch8_device> m_dev_vp2;   /* dkong2, virtual port 2 */
        optional_device<latch8_device> m_dev_6h;    /* dkong2 */
        optional_device<latch8_device> m_ls175_3d;  /* dkong2b_audio */
        optional_device<discrete_device> m_discrete;
        optional_device<intref> m_m58817;    /* radarscp1 */  //optional_device<m58817_device> m_m58817;    /* radarscp1 */
        optional_device<watchdog_timer_device> m_watchdog;

        /* memory pointers */
        required_shared_ptr_uint8_t m_video_ram;
        required_shared_ptr_uint8_t m_sprite_ram;

        /* machine states */
        uint8_t               m_hardware_type;
        uint8_t               m_nmi_mask;

        //std::unique_ptr<uint8_t[]> m_decrypted;

        /* sound state */
        optional_region_ptr_uint8_t  m_snd_rom;

        /* video state */
        tilemap_t m_bg_tilemap;

        bitmap_ind16  m_bg_bits;
        ListBytesPointer m_color_codes;  //const uint8_t *     m_color_codes;
        emu_timer m_scanline_timer;
        int8_t              m_vidhw;          /* Selected video hardware RS Conversion / TKG04 */

        /* radar scope */

        ListBase<uint8_t> m_gfx4;  //uint8_t *           m_gfx4;
        ListBase<uint8_t> m_gfx3;  //uint8_t *           m_gfx3;
        int               m_gfx3_len;

        uint8_t             m_sig30Hz;
        uint8_t             m_lfsr_5I;
        uint8_t             m_grid_sig;
        uint8_t             m_rflip_sig;
        uint8_t             m_star_ff;
        uint8_t             m_blue_level;
        double            m_cd4049_a;
        double            m_cd4049_b;

        /* Specific states */
        int8_t              m_decrypt_counter;

        /* 2650 protection */
        uint8_t             m_protect_type;
        uint8_t             m_hunchloopback;
        uint8_t             m_prot_cnt;
        uint8_t             m_main_fo;

        /* Save state relevant */
        uint8_t             m_gfx_bank;
        uint8_t             m_palette_bank;
        uint8_t             m_grid_on;
        uint16_t            m_grid_col;
        uint8_t             m_sprite_bank;
        uint8_t             m_dma_latch;
        uint8_t             m_flip;

        /* radarscp_step */
        double m_cv1;
        double m_cv2;
        double m_vg1;
        double m_vg2;
        double m_vg3;
        double m_cv3;
        double m_cv4;
        double m_vc17;
        int m_pixelcnt;

        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;
        optional_device<intref> m_z80dma;  //optional_device<z80dma_device> m_z80dma;
        optional_device<i8257_device> m_dma8257;

        /* radarscp_scanline */
        int m_counter;


        public dkong_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_soundcpu = new optional_device<mcs48_cpu_device>(this, "soundcpu");
            m_eeprom = new optional_device<eeprom_serial_93cxx_device>(this, "eeprom");
            m_dev_n2a03a = new optional_device<intref>(this, "n2a03a");
            m_dev_n2a03b = new optional_device<intref>(this, "n2a03b");
            m_dev_vp2 = new optional_device<latch8_device>(this, "virtual_p2");
            m_dev_6h = new optional_device<latch8_device>(this, "ls259.6h");
            m_ls175_3d = new optional_device<latch8_device>(this, "ls175.3d");
            m_discrete = new optional_device<discrete_device>(this, "discrete");
            m_m58817 = new optional_device<intref>(this, "tms");
            m_watchdog = new optional_device<watchdog_timer_device>(this, "watchdog");
            m_video_ram = new required_shared_ptr_uint8_t(this,"video_ram");
            m_sprite_ram = new required_shared_ptr_uint8_t(this,"sprite_ram");
            m_snd_rom = new optional_region_ptr_uint8_t(this, "soundcpu");
            m_vidhw = DKONG_BOARD;
            m_sig30Hz = 0;
            m_blue_level = 0;
            m_cv1 = 0;
            m_cv2 = 0;
            m_vg1 = 0;
            m_vg2 = 0;
            m_vg3 = 0;
            m_cv3 = 0;
            m_cv4 = 0;
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_z80dma = new optional_device<intref>(this, "z80dma");
            m_dma8257 = new optional_device<i8257_device>(this, "dma8257");
        }


        //void dkong_base(machine_config &config);
        //void dk_braze(machine_config &config);
        //void dkj_braze(machine_config &config);
        //void ddk_braze(machine_config &config);
        //void dk3_braze(machine_config &config);
        //void strtheat(machine_config &config);
        //void spclforc(machine_config &config);
        //void s2650(machine_config &config);
        //void dkongjr(machine_config &config);
        //void radarscp1(machine_config &config);
        //void drktnjr(machine_config &config);
        //void dkong2b(machine_config &config);
        //void drakton(machine_config &config);
        //void radarscp(machine_config &config);
        //void pestplce(machine_config &config);
        //void herbiedk(machine_config &config);
        //void dkong3(machine_config &config);
        //void dkong3b(machine_config &config);
        //void radarscp_audio(machine_config &config);
        //void dkong2b_audio(machine_config &config);
        //void dkongjr_audio(machine_config &config);
        //void dkong3_audio(machine_config &config);
        //void radarscp1_audio(machine_config &config);

        //void init_strtheat();
        //void init_herodk();
        //void init_dkingjr();
        //void init_drakton();
        //void init_dkonghs();
        //void init_dkongx();
        //void init_dkong3hs();

        //DECLARE_WRITE_LINE_MEMBER(dk_braze_a15);


        /* reverse address lookup map - hunchbkd */
        //int16_t             m_rev_map[0x200];
        //DECLARE_READ8_MEMBER(hb_dma_read_byte);
        //DECLARE_WRITE8_MEMBER(hb_dma_write_byte);
        //DECLARE_WRITE8_MEMBER(dkong3_coin_counter_w);
        //DECLARE_READ8_MEMBER(dkong_in2_r);
        //DECLARE_READ8_MEMBER(s2650_mirror_r);
        //DECLARE_WRITE8_MEMBER(s2650_mirror_w);
        //DECLARE_READ8_MEMBER(epos_decrypt_rom);
        //DECLARE_WRITE8_MEMBER(s2650_data_w);
        //DECLARE_WRITE_LINE_MEMBER(s2650_fo_w);
        //DECLARE_READ8_MEMBER(s2650_port0_r);
        //DECLARE_READ8_MEMBER(s2650_port1_r);
        //DECLARE_WRITE8_MEMBER(dkong3_2a03_reset_w);
        //DECLARE_READ8_MEMBER(strtheat_inputport_0_r);
        //DECLARE_READ8_MEMBER(strtheat_inputport_1_r);
        //DECLARE_WRITE8_MEMBER(nmi_mask_w);
        //DECLARE_WRITE8_MEMBER(dk_braze_a15_w);
        //DECLARE_WRITE8_MEMBER(dkong_videoram_w);
        //DECLARE_WRITE8_MEMBER(dkongjr_gfxbank_w);
        //DECLARE_WRITE8_MEMBER(dkong3_gfxbank_w);
        //DECLARE_WRITE8_MEMBER(dkong_palettebank_w);
        //DECLARE_WRITE8_MEMBER(radarscp_grid_enable_w);
        //DECLARE_WRITE8_MEMBER(radarscp_grid_color_w);
        //DECLARE_WRITE8_MEMBER(dkong_flipscreen_w);
        //DECLARE_WRITE8_MEMBER(dkong_spritebank_w);
        //DECLARE_WRITE8_MEMBER(dkong_voice_w);
        //DECLARE_WRITE8_MEMBER(dkong_audio_irq_w);
        //DECLARE_READ8_MEMBER(p8257_ctl_r);
        //DECLARE_WRITE8_MEMBER(p8257_ctl_w);
        //DECLARE_WRITE8_MEMBER(p8257_drq_w);
        //DECLARE_WRITE8_MEMBER(dkong_z80dma_rdy_w);
        //DECLARE_READ8_MEMBER(braze_eeprom_r);
        //DECLARE_WRITE8_MEMBER(braze_eeprom_w);
        //TILE_GET_INFO_MEMBER(dkong_bg_tile_info);
        //TILE_GET_INFO_MEMBER(radarscp1_bg_tile_info);
        //DECLARE_MACHINE_START(dkong2b);
        //DECLARE_MACHINE_RESET(dkong);
        //DECLARE_MACHINE_RESET(ddk);
        //DECLARE_VIDEO_START(dkong);
        //DECLARE_VIDEO_START(dkong_base);
        //DECLARE_PALETTE_INIT(dkong2b);
        //DECLARE_MACHINE_START(dkong3);
        //DECLARE_PALETTE_INIT(dkong3);
        //DECLARE_MACHINE_START(radarscp);
        //DECLARE_PALETTE_INIT(radarscp);
        //DECLARE_MACHINE_START(radarscp1);
        //DECLARE_PALETTE_INIT(radarscp1);
        //DECLARE_MACHINE_START(s2650);
        //DECLARE_MACHINE_RESET(strtheat);
        //DECLARE_MACHINE_RESET(drakton);
        //DECLARE_WRITE8_MEMBER(m58817_command_w);
        //DECLARE_READ8_MEMBER(dkong_voice_status_r);
        //DECLARE_READ8_MEMBER(dkong_tune_r);
        //DECLARE_WRITE8_MEMBER(dkong_p1_w);
        //DECLARE_READ8_MEMBER(sound_t0_r);
        //DECLARE_READ8_MEMBER(sound_t1_r);
        //uint32_t screen_update_dkong(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
        //uint32_t screen_update_pestplce(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
        //uint32_t screen_update_spclforc(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
        //DECLARE_WRITE_LINE_MEMBER(s2650_interrupt);
        //DECLARE_WRITE_LINE_MEMBER(vblank_irq);
        //TIMER_CALLBACK_MEMBER(scanline_callback);
        //DECLARE_WRITE_LINE_MEMBER(busreq_w);

        //void braze_decrypt_rom(uint8_t *dest);
        //void dk_braze_decrypt();
        //void drakton_decrypt_rom(uint8_t mod, int offs, int *bs);
        //DECLARE_READ8_MEMBER(memory_read_byte);
        //DECLARE_WRITE8_MEMBER(memory_write_byte);
        //double CD4049(double x);

        //void dkong3_io_map(address_map &map);
        //void dkong3_map(address_map &map);
        //void dkong3_sound1_map(address_map &map);
        //void dkong3_sound2_map(address_map &map);
        //void dkong_map(address_map &map);
        //void dkong_sound_io_map(address_map &map);
        //void dkong_sound_map(address_map &map);
        //void dkongjr_map(address_map &map);
        //void dkongjr_sound_io_map(address_map &map);
        //void epos_readport(address_map &map);
        //void radarscp1_sound_io_map(address_map &map);
        //void s2650_data_map(address_map &map);
        //void s2650_io_map(address_map &map);
        //void s2650_map(address_map &map);

        // video/dkong.c
        //void radarscp_step(int line_cnt);
        //void radarscp_scanline(int scanline);
        //void check_palette(void);
        //void draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect, uint32_t mask_bank, uint32_t shift_bits);
        //void radarscp_draw_background(bitmap_ind16 &bitmap, const rectangle &cliprect);


        // wrappers because I don't know how to find the correct device during construct_ startup

        //READ8_MEMBER( i8257_device::read )
        public u8 i8257_device_read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            //m_dma8257
            throw new emu_unimplemented();
        }


        //WRITE8_MEMBER( i8257_device::write )
        public void i8257_device_write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            i8257_device device = m_dma8257.target;
            device.write(space, offset, data, mem_mask);
        }


        //READ8_MEMBER( latch8_device::read )
        public u8 latch8_device_read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            latch8_device device = (latch8_device)subdevice("virtual_p2");
            return device.read(space, offset, mem_mask);
        }

        //WRITE8_MEMBER( latch8_device::write )
        public void latch8_device_write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            latch8_device device = (latch8_device)subdevice("ls175.3d");
            device.write(space, offset, data, mem_mask);
        }

        //WRITE8_MEMBER( latch8_device::bit0_w )
        public void latch8_device_bit0_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            latch8_device device = m_dev_6h.target;
            device.bit0_w(space, offset, data, mem_mask);
        }

        //READ_LINE_MEMBER(latch8_device::bit3_r)
        public int latch8_device_bit3_r()
        {
            latch8_device device = m_dev_vp2.target;
            return device.bit3_r();
        }

        //READ_LINE_MEMBER(latch8_device::bit4_q_r)
        public int latch8_device_bit4_q_r()
        {
            latch8_device device = (latch8_device)subdevice("virtual_p2");
            return device.bit4_q_r();
        }

        //READ_LINE_MEMBER(latch8_device::bit5_q_r)
        public int latch8_device_bit5_q_r()
        {
            latch8_device device = (latch8_device)subdevice("ls259.6h");
            return device.bit5_q_r();
        }


        //WRITE_LINE_MEMBER(write_line)  // discrete_device
        public void discrete_device_write_line_DS_SOUND0_INP(int state)
        {
            discrete_device device = (discrete_device)subdevice("discrete");
            device.write_line(DS_SOUND0_INP, state);
        }

        //WRITE_LINE_MEMBER(write_line)  // discrete_device
        public void discrete_device_write_line_DS_SOUND1_INP(int state)
        {
            discrete_device device = (discrete_device)subdevice("discrete");
            device.write_line(DS_SOUND1_INP, state);
        }

        //WRITE_LINE_MEMBER(write_line)  // discrete_device
        public void discrete_device_write_line_DS_SOUND2_INP(int state)
        {
            discrete_device device = (discrete_device)subdevice("discrete");
            device.write_line(DS_SOUND2_INP, state);
        }

        //WRITE_LINE_MEMBER(write_line)  // discrete_device
        public void discrete_device_write_line_DS_SOUND6_INP(int state)
        {
            discrete_device device = (discrete_device)subdevice("discrete");
            device.write_line(DS_SOUND6_INP, state);
        }

        //WRITE_LINE_MEMBER(write_line)  // discrete_device
        public void discrete_device_write_line_DS_SOUND7_INP(int state)
        {
            discrete_device device = (discrete_device)subdevice("discrete");
            device.write_line(DS_SOUND7_INP, state);
        }

        //WRITE_LINE_MEMBER(write_line)  // discrete_device
        public void discrete_device_write_line_DS_DISCHARGE_INV(int state)
        {
            throw new emu_unimplemented();
        }
    }
}
