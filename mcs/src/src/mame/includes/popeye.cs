// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using offs_t = System.UInt32;  //using offs_t = u32;
using uint8_t = System.Byte;


namespace mame
{
    partial class tnx1_state : driver_device
    {
        required_device<z80_device> m_maincpu;
        protected required_device<ay8910_device> m_aysnd;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<palette_device> m_palette;
        required_shared_ptr<uint8_t> m_dmasource;
        required_shared_ptr<uint8_t> m_videoram;
        required_shared_ptr<uint8_t> m_colorram;
        required_region_ptr<uint8_t> m_color_prom;
        required_region_ptr<uint8_t> m_color_prom_spr;
        required_ioport m_io_mconf;
        required_ioport m_io_dsw1;

        //static const res_net_decode_info mb7051_decode_info;
        //static const res_net_decode_info mb7052_decode_info;
        //static const res_net_info txt_mb7051_net_info;
        //static const res_net_info tnx1_bak_mb7051_net_info;
        //static const res_net_info obj_mb7052_net_info;

        //std::unique_ptr<bitmap_ind16> m_sprite_bitmap;
        //std::vector<uint8_t> m_sprite_ram;
        //std::vector<uint8_t> m_background_ram;
        uint8_t [] m_background_scroll = new uint8_t [3];
        tilemap_t m_fg_tilemap;
        uint8_t m_palette_bank;
        uint8_t m_palette_bank_cache;
        uint8_t m_prot0;
        uint8_t m_prot1;
        uint8_t m_prot_shift;
        uint8_t m_dswbit;
        bool m_nmi_enabled;
        int   m_field;
        //std::array<bitmap_ind16, 2>  m_bitmap;    // bitmaps for fields


        protected tnx1_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_aysnd = new required_device<ay8910_device>(this, "aysnd");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
            m_dmasource = new required_shared_ptr<uint8_t>(this, "dmasource");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_colorram = new required_shared_ptr<uint8_t>(this, "colorram");
            m_color_prom = new required_region_ptr<uint8_t>(this, "proms");
            m_color_prom_spr = new required_region_ptr<uint8_t>(this, "sprpal");
            m_io_mconf = new required_ioport(this, "MCONF");
            m_io_dsw1 = new required_ioport(this, "DSW1");
            m_background_scroll = new uint8_t [] {0,0,0};
            m_fg_tilemap = null;
            m_palette_bank = 0;
            m_palette_bank_cache = 0;
            m_prot0 = 0;
            m_prot1 = 0;
            m_prot_shift = 0;
            m_dswbit = 0;
            m_nmi_enabled = false;
            m_field = 0;
        }


        protected virtual res_net_info bak_mb7051_net_info() { throw new emu_unimplemented(); }//{ return tnx1_bak_mb7051_net_info; }

        protected virtual void refresh_w(offs_t offset, uint8_t data) { throw new emu_unimplemented(); }

        protected virtual void background_w(offs_t offset, uint8_t data) { throw new emu_unimplemented(); }

        protected override void driver_start() { throw new emu_unimplemented(); }
        protected override void video_start() { throw new emu_unimplemented(); }

        protected virtual void tnx1_palette(palette_device palette) { throw new emu_unimplemented(); }

        //virtual DECLARE_WRITE_LINE_MEMBER(screen_vblank);
        protected virtual void screen_vblank(int state) { throw new emu_unimplemented(); }

        protected virtual void decrypt_rom() { throw new emu_unimplemented(); }
        protected virtual void draw_background(bitmap_ind16 bitmap, rectangle cliprect) { throw new emu_unimplemented(); }

        protected virtual void maincpu_program_map(address_map map, device_t device) { throw new emu_unimplemented(); }

        protected virtual bool bootleg_sprites() { return false; }
    }


    class tpp1_state : tnx1_state
    {
        //using tnx1_state::tnx1_state;
        protected tpp1_state(machine_config mconfig, device_type type, string tag) : base(mconfig, type, tag) { }


        protected override void tnx1_palette(palette_device palette) { throw new emu_unimplemented(); }
        protected override void draw_background(bitmap_ind16 bitmap, rectangle cliprect) { throw new emu_unimplemented(); }

        //static const res_net_info tpp1_bak_mb7051_net_info;
        protected override res_net_info bak_mb7051_net_info() { throw new emu_unimplemented(); }//{ return tpp1_bak_mb7051_net_info; }
    }


    //class popeyebl_state : public tpp1_state


    partial class tpp2_state : tpp1_state
    {
        //using tpp1_state::tpp1_state;


        //bool m_watchdog_enabled = false;
        //uint8_t m_watchdog_counter = 0;


        public tpp2_state(machine_config mconfig, device_type type, string tag) : base(mconfig, type, tag) { }


        protected override void driver_start() { throw new emu_unimplemented(); }

        protected override void refresh_w(offs_t offset, uint8_t data) { throw new emu_unimplemented(); }

        //virtual DECLARE_WRITE_LINE_MEMBER(screen_vblank) override;
        protected override void screen_vblank(int state) { throw new emu_unimplemented(); }

        protected override void maincpu_program_map(address_map map, device_t device) { throw new emu_unimplemented(); }
        protected override void decrypt_rom() { throw new emu_unimplemented(); }
        protected override void draw_background(bitmap_ind16 bitmap, rectangle cliprect) { throw new emu_unimplemented(); }
        protected override void background_w(offs_t offset, uint8_t data) { throw new emu_unimplemented(); }
    }


    //class tpp2_noalu_state : public tpp2_state
}
