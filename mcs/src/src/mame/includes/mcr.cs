// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int8_t = System.SByte;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    partial class mcr_state : driver_device
    {
        /* constants */
        static readonly XTAL MAIN_OSC_MCR_I      = new XTAL(19_968_000);


        int8_t m_mcr12_sprite_xoffs_flip;
        uint8_t m_input_mux;
        uint8_t m_last_op4;
        tilemap_t m_bg_tilemap;

        uint8_t m_mcr_cocktail_flip;

        required_device<z80_device> m_maincpu;
        optional_shared_ptr<uint8_t> m_spriteram;
        optional_shared_ptr<uint8_t> m_videoram;
        optional_shared_ptr<uint8_t> m_paletteram;

        required_device<z80ctc_device> m_ctc;
        optional_device<midway_ssio_device> m_ssio;
        optional_device<midway_cheap_squeak_deluxe_device> m_cheap_squeak_deluxe;
        optional_device<midway_sounds_good_device> m_sounds_good;
        optional_device<midway_turbo_cheap_squeak_device> m_turbo_cheap_squeak;
        optional_device<bally_squawk_n_talk_device> m_squawk_n_talk;
        optional_device<samples_device> m_samples;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<palette_device> m_palette;


        uint32_t m_mcr_cpu_board;
        uint32_t m_mcr_sprite_board;

        int8_t m_mcr12_sprite_xoffs;


        public mcr_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_spriteram = new optional_shared_ptr<uint8_t>(this, "spriteram");
            m_videoram = new optional_shared_ptr<uint8_t>(this, "videoram");
            m_paletteram = new optional_shared_ptr<uint8_t>(this, "paletteram");
            m_ctc = new required_device<z80ctc_device>(this, "ctc");
            m_ssio = new optional_device<midway_ssio_device>(this, "ssio");
            m_cheap_squeak_deluxe = new optional_device<midway_cheap_squeak_deluxe_device>(this, "csd");
            m_sounds_good = new optional_device<midway_sounds_good_device>(this, "sg");
            m_turbo_cheap_squeak = new optional_device<midway_turbo_cheap_squeak_device>(this, "tcs");
            m_squawk_n_talk = new optional_device<bally_squawk_n_talk_device>(this, "snt");
            m_samples = new optional_device<samples_device>(this, "samples");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
        }
    }


    //class mcr_dpoker_state : public mcr_state

    //class mcr_nflfoot_state : public mcr_state
}
