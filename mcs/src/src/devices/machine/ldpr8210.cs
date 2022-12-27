// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int32_t = System.Int32;
using uint32_t = System.UInt32;

using static mame.device_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> pioneer_pr8210_device
    // base pr8210 class
    public class pioneer_pr8210_device : laserdisc_device
    {
        //DEFINE_DEVICE_TYPE(PIONEER_PR8210,   pioneer_pr8210_device,   "pr8210",   "Pioneer PR-8210")
        public static readonly emu.detail.device_type_impl PIONEER_PR8210 = DEFINE_DEVICE_TYPE("pr8210", "Pioneer PR-8210", (type, mconfig, tag, owner, clock) => { return new pioneer_pr8210_device(mconfig, tag, owner, clock); });


        // timer IDs
        //enum
        //{
        //    TID_VSYNC_OFF = TID_FIRST_PLAYER_TIMER,
        //    TID_VBI_DATA_FETCH,
        //    TID_FIRST_SUBCLASS_TIMER
        //};


        // LED outputs
        //output_finder<>     m_audio1;
        //output_finder<>     m_audio2;
        //output_finder<>     m_clv;
        //output_finder<>     m_cav;
        //output_finder<>     m_srev;
        //output_finder<>     m_sfwd;
        //output_finder<>     m_play;
        //output_finder<>     m_step;
        //output_finder<>     m_pause;
        //output_finder<>     m_standby;

        // internal state
        //uint8_t             m_control;              // control line state
        //uint8_t             m_lastcommand;          // last command seen
        //uint16_t            m_accumulator;          // bit accumulator
        //attotime            m_lastcommandtime;      // time of the last command
        //attotime            m_lastbittime;          // time of last bit received
        //attotime            m_firstbittime;         // time of first bit in command

        // low-level emulation data
        //required_device<i8049_device> m_i8049_cpu;  // 8049 CPU device
        //attotime            m_slowtrg;              // time of the last SLOW TRG
        //pioneer_pia         m_pia;                  // PIA state
        //bool                m_vsync;                // live VSYNC state
        //uint8_t             m_i8049_port1;          // 8049 port 1 state
        //uint8_t             m_i8049_port2;          // 8049 port 2 state


        // construction/destruction
        pioneer_pr8210_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : this(mconfig, PIONEER_PR8210, tag, owner, clock)
        {
        }

        pioneer_pr8210_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            throw new emu_unimplemented();
#if false
            m_audio1(*this, "pr8210_audio1"),
            m_audio2(*this, "pr8210_audio2"),
            m_clv(*this, "pr8210_clv"),
            m_cav(*this, "pr8210_cav"),
            m_srev(*this, "pr8210_srev"),
            m_sfwd(*this, "pr8210_sfwd"),
            m_play(*this, "pr8210_play"),
            m_step(*this, "pr8210_step"),
            m_pause(*this, "pr8210_pause"),
            m_standby(*this, "pr8210_standby"),
            m_control(0),
            m_lastcommand(0),
            m_accumulator(0),
            m_lastcommandtime(attotime::zero),
            m_lastbittime(attotime::zero),
            m_firstbittime(attotime::zero),
            m_i8049_cpu(*this, "pr8210"),
            m_slowtrg(attotime::zero),
            m_vsync(false),
            m_i8049_port1(0),
            m_i8049_port2(0)
#endif
        }



        // input and output
        //void control_w(uint8_t data);


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_reset() { throw new emu_unimplemented(); }
        protected override void device_timer(emu_timer timer, device_timer_id id, int param) { throw new emu_unimplemented(); }
        protected override Pointer<tiny_rom_entry> device_rom_region() { throw new emu_unimplemented(); }
        protected override void device_add_mconfig(machine_config config) { throw new emu_unimplemented(); }


        // subclass overrides
        protected override void player_vsync(vbi_metadata vbi, int fieldnum, attotime curtime) { throw new emu_unimplemented(); }
        protected override int32_t player_update(vbi_metadata vbi, int fieldnum, attotime curtime) { throw new emu_unimplemented(); }
        protected override void player_overlay(bitmap_yuy16 bitmap) { throw new emu_unimplemented(); }


        // internal helpers
        //bool focus_on() const { return !(m_i8049_port1 & 0x08); }
        //bool spdl_on() const { return !(m_i8049_port1 & 0x10); }
        //bool laser_on() const { return !(m_i8049_port2 & 0x01); }

        protected virtual bool override_control() { return false; }

        //void update_video_squelch() { set_video_squelch((m_i8049_port1 & 0x20) != 0); }

        protected virtual void update_audio_squelch() { throw new emu_unimplemented(); }//{ set_audio_squelch((m_i8049_port1 & 0x40) || !(m_pia.portb & 0x01), (m_i8049_port1 & 0x40) || !(m_pia.portb & 0x02)); }


        // internal read/write handlers
        //uint8_t i8049_pia_r(offs_t offset);
        //void i8049_pia_w(offs_t offset, uint8_t data);
        //uint8_t i8049_bus_r();
        //void i8049_port1_w(uint8_t data);
        //void i8049_port2_w(uint8_t data);
        //int i8049_t0_r();
        //int i8049_t1_r();


        // pioneer PIA subclass
        //class pioneer_pia
        //{
        //public:
        //    uint8_t               frame[7];               // (20-26) 7 characters for the chapter/frame
        //    uint8_t               text[17];               // (20-30) 17 characters for the display
        //    uint8_t               control;                // (40) control lines
        //    uint8_t               latchdisplay;           //   flag: set if the display was latched
        //    uint8_t               portb;                  // (60) port B value (LEDs)
        //    uint8_t               display;                // (80) display enable
        //    uint8_t               porta;                  // (A0) port A value (from serial decoder)
        //    uint8_t               vbi1;                   // (C0) VBI decoding state 1
        //    uint8_t               vbi2;                   // (E0) VBI decoding state 2
        //};

        // internal overlay helpers
        //void overlay_draw_group(bitmap_yuy16 &bitmap, const uint8_t *text, int count, float xstart);
        //void overlay_erase(bitmap_yuy16 &bitmap, float xstart, float xend);
        //void overlay_draw_char(bitmap_yuy16 &bitmap, uint8_t ch, float xstart);

        //void pr8210_portmap(address_map &map);
    }


    // ======================> simutrek_special_device
    //class simutrek_special_device : public pioneer_pr8210_device
}
