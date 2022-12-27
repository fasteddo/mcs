// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int8_t = System.SByte;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
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
        const u32 DK3_PALETTE_LENGTH      = 256+256+8+1; /*  (256) */
        //#define RS_PALETTE_LENGTH       (256+256+8+1)


        /* devices */
        required_device<z80_device> m_maincpu;  //required_device<cpu_device> m_maincpu;
        optional_device<mcs48_cpu_device> m_soundcpu;
        optional_device<eeprom_serial_93cxx_device> m_eeprom;
        optional_device<n2a03_device> m_dev_n2a03a; /* dkong3 */
        optional_device<n2a03_device> m_dev_n2a03b; /* dkong3 */
        optional_device<latch8_device> m_dev_vp2;   /* dkong2, virtual port 2 */
        optional_device<latch8_device> m_dev_6h;    /* dkong2 */
        optional_device<latch8_device> m_ls175_3d;  /* dkong2b_audio */
        optional_device<discrete_device> m_discrete;
        optional_device<intref> m_m58817;    /* radarscp1 */  //optional_device<m58817_device> m_m58817;    /* radarscp1 */
        optional_device<watchdog_timer_device> m_watchdog;

        /* memory pointers */
        required_shared_ptr<uint8_t> m_video_ram;
        required_shared_ptr<uint8_t> m_sprite_ram;

        /* machine states */
        uint8_t m_hardware_type = 0;
        uint8_t m_nmi_mask = 0;

        //std::unique_ptr<uint8_t[]> m_decrypted;

        /* sound state */
        optional_region_ptr<uint8_t> m_snd_rom;

        /* video state */
        tilemap_t m_bg_tilemap = null;

        bitmap_ind16 m_bg_bits;
        Pointer<uint8_t> m_color_codes = null;  //const uint8_t *     m_color_codes;
        emu_timer m_scanline_timer = null;
        int8_t m_vidhw;          /* Selected video hardware RS Conversion / TKG04 */

        /* radar scope */

        MemoryU8 m_gfx4 = null;  //uint8_t *           m_gfx4;
        MemoryU8 m_gfx3 = null;  //uint8_t *           m_gfx3;
        int m_gfx3_len = 0;

        uint8_t m_sig30Hz;
        uint8_t m_lfsr_5I = 0;
        uint8_t m_grid_sig = 0;
        uint8_t m_rflip_sig = 0;
        uint8_t m_star_ff = 0;
        uint8_t m_blue_level = 0;
        double m_cd4049_a = 0;
        double m_cd4049_b = 0;

        /* Specific states */
        int8_t m_decrypt_counter = 0;

        /* 2650 protection */
        uint8_t m_protect_type = 0;
        uint8_t m_hunchloopback = 0;
        uint8_t m_prot_cnt = 0;
        uint8_t m_main_fo = 0;

        /* Save state relevant */
        uint8_t m_gfx_bank = 0;
        uint8_t m_palette_bank = 0;
        uint8_t m_grid_on = 0;
        uint16_t m_grid_col = 0;
        uint8_t m_sprite_bank = 0;
        uint8_t m_dma_latch = 0;
        uint8_t m_flip = 0;

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
        optional_device<z80dma_device> m_z80dma;
        optional_device<i8257_device> m_dma8257;
        memory_bank_creator m_bank1;
        memory_bank_creator m_bank2;

        /* radarscp_scanline */
        int m_counter = 0;


        public dkong_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_soundcpu = new optional_device<mcs48_cpu_device>(this, "soundcpu");
            m_eeprom = new optional_device<eeprom_serial_93cxx_device>(this, "eeprom");
            m_dev_n2a03a = new optional_device<n2a03_device>(this, "n2a03a");
            m_dev_n2a03b = new optional_device<n2a03_device>(this, "n2a03b");
            m_dev_vp2 = new optional_device<latch8_device>(this, "virtual_p2");
            m_dev_6h = new optional_device<latch8_device>(this, "ls259.6h");
            m_ls175_3d = new optional_device<latch8_device>(this, "ls175.3d");
            m_discrete = new optional_device<discrete_device>(this, "discrete");
            m_m58817 = new optional_device<intref>(this, "tms");
            m_watchdog = new optional_device<watchdog_timer_device>(this, "watchdog");
            m_video_ram = new required_shared_ptr<uint8_t>(this,"video_ram");
            m_sprite_ram = new required_shared_ptr<uint8_t>(this,"sprite_ram");
            m_snd_rom = new optional_region_ptr<uint8_t>(this, "soundcpu");
            m_vidhw = DKONG_BOARD;
            m_sig30Hz = 0;
            m_star_ff = 0;
            m_blue_level = 0;
            m_cv1 = 0;
            m_cv2 = 0;
            m_vg1 = 0;
            m_vg2 = 0;
            m_vg3 = 0;
            m_cv3 = 0;
            m_cv4 = 0;
            m_vc17 = 0;
            m_pixelcnt = 0;
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_z80dma = new optional_device<z80dma_device>(this, "z80dma");
            m_dma8257 = new optional_device<i8257_device>(this, "dma8257");
            m_bank1 = new memory_bank_creator(this, "bank1");
            m_bank2 = new memory_bank_creator(this, "bank2");
        }
    }
}
