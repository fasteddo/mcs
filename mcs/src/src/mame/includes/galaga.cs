// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using ioport_value = System.UInt32;
using offs_t = System.UInt32;
using u8 = System.Byte;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    partial class galaga_state : driver_device
    {
        struct star
        {
            public uint16_t x;
            public uint16_t y;
            public uint8_t col;
            public uint8_t set;

            public star(uint16_t x, uint16_t y, uint8_t col, uint8_t set) { this.x = x; this.y = y; this.col = col; this.set = set; }
        }


        // \video
        //static struct star m_star_seed_tab[];


        /* memory pointers */
        protected optional_shared_ptr_uint8_t m_videoram;
        optional_shared_ptr_uint8_t m_galaga_ram1;
        optional_shared_ptr_uint8_t m_galaga_ram2;
        optional_shared_ptr_uint8_t m_galaga_ram3;
        optional_device<ls259_device> m_videolatch; // not present on Xevious
        required_device<cpu_device> m_maincpu;
        required_device<cpu_device> m_subcpu;
        required_device<cpu_device> m_subcpu2;
        required_device<namco_device> m_namco_sound;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;
        output_manager.output_finder/*<2>*/ m_leds;
        emu_timer m_cpu3_interrupt_timer;

        /* machine state */
        uint32_t m_stars_scrollx;
        uint32_t m_stars_scrolly;

        uint32_t m_galaga_gfxbank; // used by catsbee

        /* devices */

        /* bank support */

        /* shared */
        protected tilemap_t m_fg_tilemap;
        protected tilemap_t m_bg_tilemap;

        uint8_t m_main_irq_mask;
        uint8_t m_sub_irq_mask;
        uint8_t m_sub2_nmi_mask;


        public galaga_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_videoram = new optional_shared_ptr_uint8_t(this, "videoram");
            m_galaga_ram1 = new optional_shared_ptr_uint8_t(this, "galaga_ram1");
            m_galaga_ram2 = new optional_shared_ptr_uint8_t(this, "galaga_ram2");
            m_galaga_ram3 = new optional_shared_ptr_uint8_t(this, "galaga_ram3");
            m_videolatch = new optional_device<ls259_device>(this, "videolatch");
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_subcpu = new required_device<cpu_device>(this, "sub");
            m_subcpu2 = new required_device<cpu_device>(this, "sub2");
            m_namco_sound = new required_device<namco_device>(this, "namco");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_leds = new output_manager.output_finder(2, this, "led{0}", 0U);
        }


        public optional_device<ls259_device> videolatch { get { return m_videolatch; } }
        public required_device<cpu_device> maincpu { get { return m_maincpu; } }
        public required_device<cpu_device> subcpu { get { return m_subcpu; } }
        public required_device<cpu_device> subcpu2 { get { return m_subcpu2; } }
        public required_device<namco_device> namco_sound { get { return m_namco_sound; } }
        public required_device<gfxdecode_device> gfxdecode { get { return m_gfxdecode; } }
        public required_device<screen_device> screen { get { return m_screen; } }
        public required_device<palette_device> palette { get { return m_palette; } }


        //DECLARE_READ8_MEMBER(bosco_dsw_r);
        //DECLARE_WRITE_LINE_MEMBER(flip_screen_w);
        //DECLARE_WRITE_LINE_MEMBER(irq1_clear_w);
        //DECLARE_WRITE_LINE_MEMBER(irq2_clear_w);
        //DECLARE_WRITE_LINE_MEMBER(nmion_w);
        //DECLARE_WRITE8_MEMBER(galaga_videoram_w);
        //DECLARE_WRITE_LINE_MEMBER(gatsbee_bank_w);
        //DECLARE_WRITE8_MEMBER(out_0);
        //DECLARE_WRITE8_MEMBER(out_1);
        //DECLARE_READ8_MEMBER(namco_52xx_rom_r);
        //DECLARE_READ8_MEMBER(namco_52xx_si_r);
        //void init_galaga();
        //void init_gatsbee();
        //TILEMAP_MAPPER_MEMBER(tilemap_scan);
        //TILE_GET_INFO_MEMBER(get_tile_info);
        //DECLARE_VIDEO_START(galaga);
        //DECLARE_PALETTE_INIT(galaga);
        //uint32_t screen_update_galaga(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
        //DECLARE_WRITE_LINE_MEMBER(screen_vblank_galaga);
        //DECLARE_WRITE_LINE_MEMBER(vblank_irq);
        //TIMER_CALLBACK_MEMBER(cpu3_interrupt_callback);
        //void draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect );
        //void draw_stars(bitmap_ind16 &bitmap, const rectangle &cliprect );
        //void galaga(machine_config &config);
        //void gatsbee(machine_config &config);
        //void galagab(machine_config &config);
        //void dzigzag_mem4(address_map &map);
        //void galaga_map(address_map &map);
        //void galaga_mem4(address_map &map);
        //void gatsbee_main_map(address_map &map);

        //virtual void machine_start() override;
        //virtual void machine_reset() override;


        // wrappers because I don't know how to find the correct device during construct_ startup

        //WRITE8_MEMBER( namco_51xx_device::write )
        public void namco_51xx_device_write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            namco_51xx_device namco_51xx = (namco_51xx_device)subdevice("51xx");
            namco_51xx.write(space, offset, data, mem_mask);
        }

        //READ8_MEMBER( namco_51xx_device::read )
        public u8 namco_51xx_device_read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            namco_51xx_device namco_51xx = (namco_51xx_device)subdevice("51xx");
            return namco_51xx.read(space, offset, mem_mask);
        }

        //WRITE8_MEMBER( namco_54xx_device::write )
        public void namco_54xx_device_write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            namco_54xx_device namco_54xx = (namco_54xx_device)subdevice("54xx");
            namco_54xx.write(space, offset, data, mem_mask);
        }

        //READ8_MEMBER( namco_06xx_device::data_r )
        public u8 namco_06xx_device_data_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            namco_06xx_device namco_06xx = (namco_06xx_device)subdevice("06xx");
            return namco_06xx.data_r(space, offset, mem_mask);
        }

        //WRITE8_MEMBER( namco_06xx_device::data_w )
        public void namco_06xx_device_data_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            namco_06xx_device namco_06xx = (namco_06xx_device)subdevice("06xx");
            namco_06xx.data_w(space, offset, data, mem_mask);
        }

        //READ8_MEMBER( namco_06xx_device::ctrl_r )
        public u8 namco_06xx_device_ctrl_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            namco_06xx_device namco_06xx = (namco_06xx_device)subdevice("06xx");
            return namco_06xx.ctrl_r(space, offset, mem_mask);
        }

        //WRITE8_MEMBER( namco_06xx_device::ctrl_w )
        public void namco_06xx_device_ctrl_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            namco_06xx_device namco_06xx = (namco_06xx_device)subdevice("06xx");
            namco_06xx.ctrl_w(space, offset, data, mem_mask);
        }

        //WRITE8_MEMBER( namco_device::pacman_sound_w )
        public void namco_device_pacman_sound_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_namco_sound.target.pacman_sound_w(space, offset, data, mem_mask);
        }

        //WRITE8_MEMBER( namco_50xx_device::write )
        public void namco_50xx_device_write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            namco_50xx_device namco_50xx = (namco_50xx_device)subdevice("50xx");
            namco_50xx.write(space, offset, data, mem_mask);
        }

        //WRITE_LINE_MEMBER(namco_50xx_device::read_request)
        public void namco_50xx_device_read_request(int state)
        {
            namco_50xx_device namco_50xx = (namco_50xx_device)subdevice("50xx");
            namco_50xx.read_request(state);
        }

        //READ8_MEMBER( namco_50xx_device::read )
        public u8 namco_50xx_device_read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            namco_50xx_device namco_50xx = (namco_50xx_device)subdevice("50xx");
            return namco_50xx.read(space, offset, mem_mask);
        }

        //WRITE_LINE_MEMBER(namco_53xx_device::read_request)
        public void namco_53xx_device_read_request(int state)
        {
            namco_53xx_device namco_53xx = (namco_53xx_device)subdevice("53xx");
            namco_53xx.read_request(state);
        }

        //READ8_MEMBER( namco_53xx_device::read )
        public u8 namco_53xx_device_read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            namco_53xx_device namco_53xx = (namco_53xx_device)subdevice("53xx");
            return namco_53xx.read(space, offset, mem_mask);
        }

        //WRITE8_MEMBER( watchdog_timer_device::reset_w )
        public void watchdog_timer_device_reset_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            watchdog_timer_device watchdog = (watchdog_timer_device)machine().config().device_find(this, "watchdog");
            watchdog.reset_w(space, offset, data, mem_mask);
        }

        //WRITE8_MEMBER(addressable_latch_device::write_d0)
        public void ls259_device_write_d0_misclatch(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            addressable_latch_device device = (addressable_latch_device)subdevice("misclatch");
            device.write_d0(space, offset, data, mem_mask);
        }

        //WRITE8_MEMBER(addressable_latch_device::write_d0)
        public void ls259_device_write_d0_videolatch(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            addressable_latch_device device = (addressable_latch_device)subdevice("videolatch");
            device.write_d0(space, offset, data, mem_mask);
        }

        //READ_LINE_MEMBER(addressable_latch_device::q5_r)
        public int ls259_device_q5_r()
        {
            addressable_latch_device device = (addressable_latch_device)subdevice("misclatch");
            return device.q5_r();
        }

        //READ_LINE_MEMBER(addressable_latch_device::q6_r)
        public int ls259_device_q6_r()
        {
            addressable_latch_device device = (addressable_latch_device)subdevice("misclatch");
            return device.q6_r();
        }

        //READ_LINE_MEMBER(addressable_latch_device::q7_r)
        public int ls259_device_q7_r()
        {
            addressable_latch_device device = (addressable_latch_device)subdevice("misclatch");
            return device.q7_r();
        }
    }
}
