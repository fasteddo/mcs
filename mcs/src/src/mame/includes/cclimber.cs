// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint8_t = System.Byte;

using static mame.diexec_global;


namespace mame
{
    partial class cclimber_state : driver_device
    {
        required_device<z80_device> m_maincpu;  //required_device<cpu_device> m_maincpu;
        optional_device<cpu_device> m_audiocpu;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<palette_device> m_palette;
        required_device<ls259_device> m_mainlatch;
        optional_device<generic_latch_8_device> m_soundlatch;

        required_shared_ptr<uint8_t> m_bigsprite_videoram;
        required_shared_ptr<uint8_t> m_videoram;
        optional_shared_ptr<uint8_t> m_column_scroll;
        required_shared_ptr<uint8_t> m_spriteram;
        required_shared_ptr<uint8_t> m_bigsprite_control;
        required_shared_ptr<uint8_t> m_colorram;
        optional_shared_ptr<uint8_t> m_swimmer_background_color;
        optional_shared_ptr<uint8_t> m_toprollr_bg_videoram;
        optional_shared_ptr<uint8_t> m_toprollr_bg_coloram;
        optional_shared_ptr<uint8_t> m_decrypted_opcodes;

        bool m_flip_x = false;
        bool m_flip_y = false;
        //bool m_swimmer_side_background_enabled = false;
        //bool m_swimmer_palettebank = false;

        //uint8_t m_yamato_p0 = 0U;
        //uint8_t m_yamato_p1 = 0U;
        uint8_t m_toprollr_rombank = 0;
        bool m_nmi_mask = false;
        tilemap_t m_pf_tilemap = null;
        tilemap_t m_bs_tilemap = null;
        //tilemap_t *m_toproller_bg_tilemap = nullptr;
        //std::unique_ptr<uint8_t[]> m_opcodes;


        public cclimber_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_audiocpu = new optional_device<cpu_device>(this, "audiocpu");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
            m_mainlatch = new required_device<ls259_device>(this, "mainlatch");
            m_soundlatch = new optional_device<generic_latch_8_device>(this, "soundlatch");
            m_bigsprite_videoram = new required_shared_ptr<uint8_t>(this, "bigspriteram");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_column_scroll = new optional_shared_ptr<uint8_t>(this, "column_scroll");
            m_spriteram = new required_shared_ptr<uint8_t>(this, "spriteram");
            m_bigsprite_control = new required_shared_ptr<uint8_t>(this, "bigspritectrl");
            m_colorram = new required_shared_ptr<uint8_t>(this, "colorram");
            m_swimmer_background_color = new optional_shared_ptr<uint8_t>(this, "bgcolor");
            m_toprollr_bg_videoram = new optional_shared_ptr<uint8_t>(this, "bg_videoram");
            m_toprollr_bg_coloram = new optional_shared_ptr<uint8_t>(this, "bg_coloram");
            m_decrypted_opcodes = new optional_shared_ptr<uint8_t>(this, "decrypted_opcodes");
        }


        protected override void machine_reset() { m_maincpu.op0.pulse_input_line(INPUT_LINE_RESET, attotime.zero); }
    }
}
