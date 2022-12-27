// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using optional_memory_region = mame.memory_region_finder<mame.bool_const_false>;  //using optional_memory_region = memory_region_finder<false>;
using uint8_t = System.Byte;


namespace mame
{
    partial class btime_state : driver_device
    {
        /* memory pointers */
        optional_shared_ptr<uint8_t> m_rambase;
        required_shared_ptr<uint8_t> m_videoram;
        required_shared_ptr<uint8_t> m_colorram;
        optional_shared_ptr<uint8_t> m_bnj_backgroundram;
        optional_shared_ptr<uint8_t> m_zoar_scrollram;
        optional_shared_ptr<uint8_t> m_lnc_charbank;
        optional_shared_ptr<uint8_t> m_deco_charram;
        optional_shared_ptr<uint8_t> m_spriteram;     // used by disco
//        uint8_t *  m_decrypted;
        optional_shared_ptr<uint8_t> m_audio_rambase;

        /* video-related */
        //std::unique_ptr<bitmap_ind16> m_background_bitmap;
        uint8_t m_btime_palette;
        uint8_t m_bnj_scroll1;
        uint8_t m_bnj_scroll2;
        uint8_t [] m_btime_tilemap = new uint8_t [4];

        /* audio-related */
        uint8_t m_audio_nmi_enable_type;

        /* protection-related (for mmonkey) */
        //int      m_protection_command;
        //int      m_protection_status;
        //int      m_protection_value;
        //int      m_protection_ret;

        /* devices */
        required_device<deco_cpu7_device> m_maincpu;  //required_device<cpu_device> m_maincpu;
        optional_device<m6502_device> m_audiocpu;  //optional_device<cpu_device> m_audiocpu;
        optional_device<input_merger_device> m_audionmi;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;
        optional_device<generic_latch_8_device> m_soundlatch;
        optional_memory_region m_prom_region;


        public btime_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_rambase = new optional_shared_ptr<uint8_t>(this, "rambase");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_colorram = new required_shared_ptr<uint8_t>(this, "colorram");
            m_bnj_backgroundram = new optional_shared_ptr<uint8_t>(this, "bnj_bgram");
            m_zoar_scrollram = new optional_shared_ptr<uint8_t>(this, "zoar_scrollram");
            m_lnc_charbank = new optional_shared_ptr<uint8_t>(this, "lnc_charbank");
            m_deco_charram = new optional_shared_ptr<uint8_t>(this, "deco_charram");
            m_spriteram = new optional_shared_ptr<uint8_t>(this, "spriteram");
            m_audio_rambase = new optional_shared_ptr<uint8_t>(this, "audio_rambase");
            m_maincpu = new required_device<deco_cpu7_device>(this, "maincpu");
            m_audiocpu = new optional_device<m6502_device>(this, "audiocpu");
            m_audionmi = new optional_device<input_merger_device>(this, "audionmi");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_soundlatch = new optional_device<generic_latch_8_device>(this, "soundlatch");
            m_prom_region = new optional_memory_region(this, "proms");
        }
    }
}
