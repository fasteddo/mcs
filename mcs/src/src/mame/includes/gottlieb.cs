// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u8 = System.Byte;
using u16 = System.UInt16;


namespace mame
{
    // ======================> gottlieb_state
    // shared driver state
    partial class gottlieb_state : driver_device
    {
        const u16 GOTTLIEB_VIDEO_HCOUNT   = 318;
        const u16 GOTTLIEB_VIDEO_HBLANK   = 256;
        const u16 GOTTLIEB_VIDEO_VCOUNT   = 256;
        const u16 GOTTLIEB_VIDEO_VBLANK   = 240;


        //enum
        //{
        //    TIMER_LASERDISC_PHILIPS,
        //    TIMER_LASERDISC_BIT_OFF,
        //    TIMER_LASERDISC_BIT,
        //    TIMER_NMI_CLEAR
        //};


        // devices
        required_device<i8088_cpu_device> m_maincpu;
        optional_device<pioneer_pr8210_device> m_laserdisc;
        optional_device<gottlieb_sound_r1_with_votrax_device> m_r1_sound;
        optional_device<gottlieb_sound_r2_device> m_r2_sound;
        optional_device<samples_device> m_knocker_sample;

        required_shared_ptr<u8> m_videoram;
        required_shared_ptr<u8> m_charram;
        required_shared_ptr<u8> m_spriteram;

        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;
        required_shared_ptr<u8> m_paletteram;

        optional_ioport m_track_x;
        optional_ioport m_track_y;
        output_finder<u32_const_3> m_leds;  // only used by reactor
        output_finder<u32_const_1> m_knockers;  // only used by qbert

        //u8 m_knocker_prev = 0U;
        //u8 m_joystick_select = 0U;
        //u8 m_track[2]{};
        //emu_timer *m_laserdisc_bit_timer = null;
        //emu_timer *m_laserdisc_philips_timer = null;
        //u8 m_laserdisc_select = 0U;
        //u8 m_laserdisc_status = 0U;
        //uint16_t m_laserdisc_philips_code = 0U;
        //std::unique_ptr<u8[]> m_laserdisc_audio_buffer;
        //uint16_t m_laserdisc_audio_address = 0U;
        //int16_t m_laserdisc_last_samples[2]{};
        //attotime m_laserdisc_last_time;
        //attotime m_laserdisc_last_clock;
        //u8 m_laserdisc_zero_seen = 0U;
        //u8 m_laserdisc_audio_bits = 0U;
        //u8 m_laserdisc_audio_bit_count = 0U;
        //u8 m_gfxcharlo = 0U;
        //u8 m_gfxcharhi = 0U;
        //u8 m_background_priority = 0U;
        //u8 m_spritebank = 0U;
        //u8 m_transparent0 = 0U;
        //tilemap_t *m_bg_tilemap = null;
        //double m_weights[4]{};


        public gottlieb_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<i8088_cpu_device>(this, "maincpu");
            m_laserdisc = new optional_device<pioneer_pr8210_device>(this, "laserdisc");
            m_r1_sound = new optional_device<gottlieb_sound_r1_with_votrax_device>(this, "r1sound");
            m_r2_sound = new optional_device<gottlieb_sound_r2_device>(this, "r2sound");
            m_knocker_sample = new optional_device<samples_device>(this, "knocker_sam");
            m_videoram = new required_shared_ptr<u8>(this, "videoram");
            m_charram = new required_shared_ptr<u8>(this, "charram");
            m_spriteram = new required_shared_ptr<u8>(this, "spriteram");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_paletteram = new required_shared_ptr<u8>(this, "paletteram");
            m_track_x = new optional_ioport(this, "TRACKX");
            m_track_y = new optional_ioport(this, "TRACKY");
            m_leds = new output_finder<u32_const_3>(this, "led{0}", 0U);
            m_knockers = new output_finder<u32_const_1>(this, "knocker{0}", 0U);
        }


        protected override void machine_start() { throw new emu_unimplemented(); }
        protected override void machine_reset() { throw new emu_unimplemented(); }
        protected override void video_start() { throw new emu_unimplemented(); }
        protected override void device_timer(emu_timer timer, device_timer_id id, int param) { throw new emu_unimplemented(); }
    }
}
