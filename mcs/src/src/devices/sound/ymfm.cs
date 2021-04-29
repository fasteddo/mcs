// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using devcb_write_line = mame.devcb_write<int, uint, mame.devcb_operators_s32_u32, mame.devcb_operators_u32_s32, mame.devcb_constant_1<uint, uint, mame.devcb_operators_u32_u32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using s16 = System.Int16;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using ymopm_engine = mame.ymfm_engine_base<mame.ymopm_registers>;


namespace mame
{
    //*********************************************************
    //  MACROS
    //*********************************************************

    // special naming helper to keep our namespace isolated from other
    // same-named objects in the device's namespace (mostly necessary
    // for chips which derive from AY-8910 classes and may have clashing
    // names)
    //#define YMFM_NAME(x) x, "ymfm." #x


    //*********************************************************
    //  REGISTER CLASSES
    //*********************************************************

    // ======================> ymfm_registers_base
    class ymfm_registers_base : global_object
    {
        // internal state
        protected u16 m_chbase;                  // base offset for channel-specific data
        u16 m_opbase;                  // base offset for operator-specific data
        protected std.vector<u8> m_regdata;    // reference to the raw data


        // constructor
        protected ymfm_registers_base(std.vector<u8> regdata, u16 chbase = 0, u16 opbase = 0)
        {
            m_chbase = chbase;
            m_opbase = opbase;
            m_regdata = regdata;
        }


        // system-wide registers that aren't universally supported
        //u8 noise_frequency() const    /*  5 bits */ { return 0; } // not on OPN,OPNA
        //u8 noise_enabled() const      /*  1 bit  */ { return 0; } // not on OPN,OPNA
        //u8 lfo_enabled() const        /*  1 bit  */ { return 0; } // not on OPM,OPN
        //u8 lfo_rate() const           /*3-8 bits */ { return 0; } // not on OPN
        //u8 lfo_waveform() const       /*  2 bits */ { return 0; } // not on OPN,OPNA
        //u8 lfo_pm_depth() const       /*  7 bits */ { return 0; } // not on OPN,OPNA
        //u8 lfo_am_depth() const       /*  7 bits */ { return 0; } // not on OPN,OPNA
        //u8 multi_freq() const         /*  1 bit  */ { return 0; } // not on OPM
        //u16 multi_block_freq0() const /* 14 bits */ { return 0; } // not on OPM
        //u16 multi_block_freq1() const /* 14 bits */ { return 0; } // not on OPM
        //u16 multi_block_freq2() const /* 14 bits */ { return 0; } // not on OPM

        // per-channel registers that aren't universally supported
        //u8 pan_right() const          /*  1 bit  */ { return 1; } // not on OPN
        //u8 pan_left() const           /*  1 bit  */ { return 1; } // not on OPN
        //u8 lfo_pm_sensitivity() const /*  3 bits */ { return 0; } // not on OPN
        //u8 lfo_am_sensitivity() const /*  2 bits */ { return 0; } // not on OPN

        // per-operator registers that aren't universally supported
        //u8 lfo_am_enabled() const     /*  1 bit  */ { return 0; } // not on OPN
        //u8 detune2() const            /*  2 bits */ { return 0; } // not on OPN,OPN2
        //u8 ssg_eg_enabled() const     /*  1 bit  */ { return 0; } // not on OPM
        //u8 ssg_eg_mode() const        /*  1 bit  */ { return 0; } // not on OPM


        // return a bitfield extracted from a byte
        protected u8 sysbyte(u16 offset, u8 start, u8 count)
        {
            return (u8)g.BIT(m_regdata[offset], start, count);
        }

        //u8 chbyte(u16 offset, u8 start, u8 count) const { return sysbyte(offset + m_chbase, start, count); }
        //u8 opbyte(u16 offset, u8 start, u8 count) const { return sysbyte(offset + m_opbase, start, count); }

        // return a bitfield extracted from a pair of bytes, MSBs listed first
        protected u16 sysword(u16 offset1, u8 start1, u8 count1, u16 offset2, u8 start2, u8 count2)
        {
            return (u16)((sysbyte(offset1, start1, count1) << count2) | sysbyte(offset2, start2, count2));
        }

        //u16 chword(u16 offset1, u8 start1, u8 count1, u16 offset2, u8 start2, u8 count2) const { return sysword(offset1 + m_chbase, start1, count1, offset2 + m_chbase, start2, count2); }
    }


    // ======================> ymopm_registers
    class ymopm_registers : ymfm_registers_base
    {
        // constants
        public u8 DEFAULT_PRESCALE = 2;
        public u8 CHANNELS = 8;
        //static constexpr u8 CSM_TRIGGER_MASK = 0xff;
        public u16 REGISTERS = 0x100;
        public u16 REG_MODE = 0x14;
        public u16 REG_KEYON = 0x08;


        // constructor
        public ymopm_registers(std.vector<u8> regdata, u16 chbase = 0, u16 opbase = 0) :
            base(regdata, chbase, opbase)
        {
        }


        // return channel/operator number
        //u8 chnum() const { return BIT(m_chbase, 0, 3); }
        //u8 opnum() const { return BIT(m_opbase, 4) | (BIT(m_opbase, 3) << 1); }

        // reset state to default values
        //void reset()
        //{
        //    // enable output on both channels by default
        //    m_regdata[0x20] = m_regdata[0x21] = m_regdata[0x22] = m_regdata[0x23] = 0xc0;
        //    m_regdata[0x24] = m_regdata[0x25] = m_regdata[0x26] = m_regdata[0x27] = 0xc0;
        //}


        // write access
        public void write(u16 index, u8 data)
        {
            // LFO AM/PM depth are written to the same register (0x19);
            // redirect the PM depth to an unused neighbor (0x1a)
            if (index == 0x19)
                m_regdata[index + g.BIT(data, 7)] = data;
            else if (index != 0x1a)
                m_regdata[index] = data;
        }


        // create a new version of ourself with a different channel/operator base

        public ymopm_registers channel_registers(u8 chnum) { return new ymopm_registers(m_regdata, channel_offset(chnum)); }

        public ymopm_registers operator_registers(u8 opnum) { return new ymopm_registers(m_regdata, m_chbase, (u16)(m_chbase + operator_offset(opnum))); }


        // system-wide registers
        //u8 test() const               /*  8 bits */ { return sysbyte(0x01, 0, 8); }
        public u8 keyon_states()        /*  4 bits */ { return sysbyte(0x08, 3, 4); }
        public u8 keyon_channel()       /*  3 bits */ { return sysbyte(0x08, 0, 3); }
        //u8 noise_frequency() const    /*  5 bits */ { return sysbyte(0x0f, 0, 5); }
        //u8 noise_enabled() const      /*  1 bit  */ { return sysbyte(0x0f, 7, 1); }
        public u16 timer_a_value()      /* 10 bits */ { return sysword(0x10, 0, 8, 0x11, 0, 2); }
        public u8 timer_b_value()       /*  8 bits */ { return sysbyte(0x12, 0, 8); }
        //u8 csm() const                /*  1 bit  */ { return sysbyte(0x14, 7, 1); }
        public u8 reset_timer_b()       /*  1 bit  */ { return sysbyte(0x14, 5, 1); }
        public u8 reset_timer_a()       /*  1 bit  */ { return sysbyte(0x14, 4, 1); }
        //u8 enable_timer_b() const     /*  1 bit  */ { return sysbyte(0x14, 3, 1); }
        //u8 enable_timer_a() const     /*  1 bit  */ { return sysbyte(0x14, 2, 1); }
        public u8 load_timer_b()        /*  1 bit  */ { return sysbyte(0x14, 1, 1); }
        public u8 load_timer_a()        /*  1 bit  */ { return sysbyte(0x14, 0, 1); }
        //u8 lfo_rate() const           /*  8 bits */ { return sysbyte(0x18, 0, 8); }
        //u8 lfo_am_depth() const       /*  7 bits */ { return sysbyte(0x19, 0, 7); }
        //u8 lfo_pm_depth() const       /*  7 bits */ { return sysbyte(0x1a, 0, 7); }
        //u8 lfo_waveform() const       /*  2 bits */ { return sysbyte(0x1b, 0, 2); }

        // per-channel registers
        //u8 pan_right() const          /*  1 bit  */ { return chbyte(0x20, 7, 1); }
        //u8 pan_left() const           /*  1 bit  */ { return chbyte(0x20, 6, 1); }
        //u8 feedback() const           /*  3 bits */ { return chbyte(0x20, 3, 3); }
        //u8 algorithm() const          /*  3 bits */ { return chbyte(0x20, 0, 3); }
        //u16 block_freq() const        /* 13 bits */ { return chword(0x28, 0, 7, 0x30, 2, 6); }
        //u8 lfo_pm_sensitivity() const /*  3 bits */ { return chbyte(0x38, 4, 3); }
        //u8 lfo_am_sensitivity() const /*  2 bits */ { return chbyte(0x38, 0, 2); }

        // per-operator registers
        //u8 detune() const             /*  3 bits */ { return opbyte(0x40, 4, 3); }
        //u8 multiple() const           /*  4 bits */ { return opbyte(0x40, 0, 4); }
        //u8 total_level() const        /*  7 bits */ { return opbyte(0x60, 0, 7); }
        //u8 ksr() const                /*  2 bits */ { return opbyte(0x80, 6, 2); }
        //u8 attack_rate() const        /*  5 bits */ { return opbyte(0x80, 0, 5); }
        //u8 lfo_am_enabled() const     /*  1 bit  */ { return opbyte(0xa0, 7, 1); }
        //u8 decay_rate() const         /*  5 bits */ { return opbyte(0xa0, 0, 5); }
        //u8 detune2() const            /*  2 bits */ { return opbyte(0xc0, 6, 2); }
        //u8 sustain_rate() const       /*  5 bits */ { return opbyte(0xc0, 0, 5); }
        //u8 sustain_level() const      /*  4 bits */ { return opbyte(0xe0, 4, 4); }
        //u8 release_rate() const       /*  4 bits */ { return opbyte(0xe0, 0, 4); }

        // LFO is always enabled
        //u8 lfo_enabled() const { return 1; }

        // special helper for generically getting the attack/decay/statain/release rates
        //u8 adsr_rate(u8 state) const


        // convert a channel number into a register offset; channel goes into the low 3 bits
        static u8 channel_offset(u8 chnum) { return (u8)g.BIT(chnum, 0, 3); }

        // convert an operator number into a register offset; operator goes into bits 3-4
        static u8 operator_offset(u8 opnum) { return (u8)((g.BIT(opnum, 0) << 4) | (g.BIT(opnum, 1) << 3)); }
    }


    // ======================> ymopn_registers
    //class ymopn_registers : public ymfm_registers_base


    // ======================> ymopna_registers
    //class ymopna_registers : public ymopn_registers


    //*********************************************************
    //  CORE ENGINE CLASSES
    //*********************************************************

    // ======================> ymfm_operator
    //template<class RegisterType>
    class ymfm_operator<RegisterType>
    {
        enum envelope_state : u8
        {
            ENV_ATTACK = 0,
            ENV_DECAY = 1,
            ENV_SUSTAIN = 2,
            ENV_RELEASE = 3
        }


        //static constexpr u16 ENV_QUIET = 0x200;


        // internal state
        u32 m_phase;                     // current phase value (10.10 format)
        u16 m_env_attenuation;           // computed envelope attenuation (4.6 format)
        envelope_state m_env_state;      // current envelope state
        u8 m_ssg_inverted;               // non-zero if the output should be inverted (bit 0)
        u8 m_key_state;                  // current key state: on or off (bit 0)
        u8 m_keyon;                      // live key on state (bit 0)
        u8 m_csm_triggered;              // true if a CSM key on has been triggered (bit 0)
        RegisterType m_regs;             // operator-specific registers


        // constructor
        public ymfm_operator(RegisterType regs)
        {
            m_phase = 0;
            m_env_attenuation = 0x3ff;
            m_env_state = envelope_state.ENV_RELEASE;
            m_ssg_inverted = 0;  //false;
            m_key_state = 0;
            m_keyon = 0;
            m_csm_triggered = 0;
            m_regs = regs;
        }


        // register for save states
        //void save(device_t &device, u8 index);

        // reset the operator state
        //void reset();

        // master clocking function
        //void clock(u32 env_counter, s8 lfo_raw_pm, u16 block_freq);

        // compute operator volume
        //s16 compute_volume(u16 modulation, u16 am_offset) const;

        // compute volume for the OPM noise channel
        //s16 compute_noise_volume(u8 noise_state, u16 am_offset) const;

        // key state control
        public void keyonoff(u8 on) { m_keyon = on; }
        //void keyon_csm() { m_csm_triggered = 1; }

        // are we active?
        //bool active() const { return (m_env_state != ENV_RELEASE || m_env_attenuation < ENV_QUIET); }


        // convert the generic block_freq into a 5-bit keycode
        //u8 block_freq_to_keycode(u16 block_freq);

        // return the effective 6-bit ADSR rate after adjustments
        //u8 effective_rate(u8 rawrate, u8 keycode);

        // start the attack phase
        //void start_attack(u8 keycode);

        // start the release phase
        //void start_release();

        // clock phases
        //void clock_keystate(u8 keystate, u8 keycode);
        //void clock_ssg_eg_state(u8 keycode);
        //void clock_envelope(u16 env_counter, u8 keycode);
        //void clock_phase(s8 lfo_raw_pm, u16 block_freq);

        // return effective attenuation of the envelope
        //u16 envelope_attenuation(u8 am_offset) const;
    }


    //template<>
    //u8 ymfm_operator<ymopm_registers>::block_freq_to_keycode(u16 block_freq);

    //template<>
    //void ymfm_operator<ymopm_registers>::clock_phase(s8 lfo_raw_pm, u16 block_freq);


    // ======================> ymfm_channel
    //template<class RegisterType>
    class ymfm_channel<RegisterType> : global_object
        where RegisterType : ymopm_registers
    {
        // internal state
        s16 [] m_feedback = new s16[2];                    // feedback memory for operator 1
        s16 m_feedback_in;            // next input value for op 1 feedback (set in output)  //mutable s16 m_feedback_in;            // next input value for op 1 feedback (set in output)
        ymfm_operator<RegisterType> m_op1;    // operator 1
        ymfm_operator<RegisterType> m_op2;    // operator 2
        ymfm_operator<RegisterType> m_op3;    // operator 3
        ymfm_operator<RegisterType> m_op4;    // operator 4
        RegisterType m_regs;                  // channel-specific registers


        // constructor
        public ymfm_channel(RegisterType regs)
        {
            m_feedback = new s16[2] { 0, 0 };
            m_feedback_in = 0;
            m_op1 = new ymfm_operator<RegisterType>((RegisterType)regs.operator_registers(0));
            m_op2 = new ymfm_operator<RegisterType>((RegisterType)regs.operator_registers(1));
            m_op3 = new ymfm_operator<RegisterType>((RegisterType)regs.operator_registers(2));
            m_op4 = new ymfm_operator<RegisterType>((RegisterType)regs.operator_registers(3));
            m_regs = regs;
        }


        // register for save states
        //void save(device_t &device, u8 index);

        // reset the channel state
        //void reset();

        // signal key on/off to our operators
        //-------------------------------------------------
        //  keyonoff - signal key on/off to our operators
        //-------------------------------------------------
        //template<class RegisterType>
        public void keyonoff(u8 states)
        {
            m_op1.keyonoff((u8)g.BIT(states, 0));
            m_op2.keyonoff((u8)g.BIT(states, 1));
            m_op3.keyonoff((u8)g.BIT(states, 2));
            m_op4.keyonoff((u8)g.BIT(states, 3));
        }

        // signal CSM key on to our operators
        //void keyon_csm();

        // master clocking function
        //void clock(u32 env_counter, s8 lfo_raw_pm, bool is_multi_freq);

        // compute the channel output and add to the left/right output sums
        //void output(u8 lfo_raw_am, u8 noise_state, s32 &lsum, s32 &rsum, u8 rshift, s16 clipmax) const;

        // is this channel active?
        //bool active() const { return m_op1.active() || m_op2.active() || m_op3.active() || m_op4.active(); }


        // convert a 6/8-bit raw AM value into an amplitude offset based on sensitivity
        //u16 lfo_am_offset(u8 am_value) const;
    }


    //template<>
    //u16 ymfm_channel<ymopm_registers>::lfo_am_offset(u8 lfo_raw_am) const;


    // ======================> ymfm_engine_base

    //template<class RegisterType>
    class ymfm_engine_base<RegisterType> : global_object
        where RegisterType : ymopm_registers
    {
        //enum : u8
        //{
        const u8 STATUS_TIMERA = 0x01;
        const u8 STATUS_TIMERB = 0x02;
        const u8 STATUS_BUSY = 0x80;
        //};


        // internal state
        device_t m_device;              // reference to the owning device
        u32 m_env_counter;               // envelope counter; low 2 bits are sub-counter
        u32 m_lfo_counter;               // LFO counter
        u32 m_noise_lfsr;                // noise LFSR state
        u8 m_noise_counter;              // noise counter
        u8 m_noise_state;                // latched noise state
        u8 m_noise_lfo;                  // latched LFO noise value
        u8 m_lfo_am;                     // current LFO AM value
        u8 m_status;                     // current status register
        u8 m_clock_prescale;             // prescale factor (2/3/6)
        u8 m_irq_mask;                   // mask of which bits signal IRQs
        u8 m_irq_state;                  // current IRQ state
        u32 m_active_channels;           // mask of active channels (computed by prepare)
        u32 m_modified_channels;         // mask of channels that have been modified
        u32 m_prepare_count;             // counter to do periodic prepare sweeps
        attotime m_busy_end;             // end of the busy time
        emu_timer [] m_timer = new emu_timer[2];           // our two timers
        devcb_write_line m_irq_handler;  // IRQ callback
        ymfm_channel<RegisterType> [] m_channel; // channel pointers  //std::unique_ptr<ymfm_channel<RegisterType>> m_channel[RegisterType::CHANNELS]; // channel pointers
        std.vector<u8> m_regdata;       // raw register data
        RegisterType m_regs;             // register accessor


        // constructor
        public ymfm_engine_base(device_t device, Func<std.vector<u8>, RegisterType> creator)
        {
            var temp_regs = creator(new std.vector<u8>());

            m_device = device;
            m_env_counter = 0;
            m_lfo_counter = 0;
            m_noise_lfsr = 0;
            m_noise_counter = 0;
            m_noise_state = 0;
            m_noise_lfo = 0;
            m_lfo_am = 0;
            m_status = 0;
            m_clock_prescale = temp_regs.DEFAULT_PRESCALE;  //m_clock_prescale(RegisterType::DEFAULT_PRESCALE),
            m_irq_mask = STATUS_TIMERA | STATUS_TIMERB;
            m_irq_state = 0;
            m_active_channels = 0xffffffff;
            m_modified_channels = 0xffffffff;
            m_prepare_count = 0;
            m_busy_end = attotime.zero;
            m_timer = new emu_timer[2] { null, null };
            m_irq_handler = new devcb_write_line(device);
            m_regdata = new std.vector<u8>(temp_regs.REGISTERS);  //m_regdata(RegisterType::REGISTERS),
            m_regs = creator(m_regdata);  //m_regs(m_regdata)


            // create the channels
            m_channel = new ymfm_channel<RegisterType>[m_regs.CHANNELS];
            for (int chnum = 0; chnum < m_regs.CHANNELS; chnum++)  //for (int chnum = 0; chnum < RegisterType::CHANNELS; chnum++)
                m_channel[chnum] = new ymfm_channel<RegisterType>((RegisterType)m_regs.channel_registers((u8)chnum));  //m_channel[chnum] = std::make_unique<ymfm_channel<RegisterType>>(m_regs.channel_registers(chnum));
        }


        // register for save states
        //void save(device_t &device);

        // reset the overall state
        //void reset();

        // master clocking function
        //u32 clock(u8 chanmask);

        // compute sum of channel outputs
        //void output(s32 &lsum, s32 &rsum, u8 rshift, s16 clipmax, u8 chanmask) const;


        // write to the OPN registers
        //-------------------------------------------------
        //  write - handle writes to the OPN registers
        //-------------------------------------------------
        //template<class RegisterType>
        public void write(u16 regnum, u8 data)
        {
            // special case: writes to the mode register can impact IRQs;
            // schedule these writes to ensure ordering with timers
            if (regnum == m_regs.REG_MODE)  //if (regnum == RegisterType::REG_MODE)
            {
                m_device.machine().scheduler().synchronize(synced_mode_w, data);  //m_device.machine().scheduler().synchronize(timer_expired_delegate(FUNC(ymfm_engine_base<RegisterType>::synced_mode_w), this), data);
                return;
            }

            // most writes are passive, consumed only when needed
            m_regs.write(regnum, data);

            // for now just mark all channels as modified
            m_modified_channels = 0xffffffff;

            // handle writes to the keyon registers
            if (regnum == m_regs.REG_KEYON)  //if (regnum == RegisterType::REG_KEYON)
            {
                u8 chnum = m_regs.keyon_channel();
                if (chnum < m_regs.CHANNELS)  //if (chnum < RegisterType::CHANNELS)
                    m_channel[chnum].keyonoff(m_regs.keyon_states());
            }
        }


        // return the current status
        //-------------------------------------------------
        //  status - return the current state of the
        //  status flags
        //-------------------------------------------------
        //template<class RegisterType>
        public u8 status()
        {
            u8 result = (u8)(m_status & ~STATUS_BUSY);

            // synthesize the busy flag if we're still busy
            if (m_device.machine().time() < m_busy_end)
                result |= STATUS_BUSY;
            return result;
        }


        // set/reset bits in the status register, updating the IRQ status
        void set_reset_status(u8 set, u8 reset) { m_status = (u8)((m_status | set) & ~reset); schedule_check_interrupts(); }

        // set the IRQ mask
        //void set_irq_mask(u8 mask) { m_irq_mask = mask; schedule_check_interrupts(); }

        // helper to compute the busy duration
        public attotime compute_busy_duration(u32 cycles = 32)
        {
            return attotime.from_hz(m_device.clock()) * (cycles * m_clock_prescale);
        }

        // set the time when the busy flag in the status register should be cleared
        public void set_busy_end(attotime end) { m_busy_end = end; }

        // return the current clock prescale
        //u8 clock_prescale() const { return m_clock_prescale; }

        // set prescale factor (2/3/6)
        //void set_clock_prescale(u8 prescale) { m_clock_prescale = prescale; }

        // configuration helpers
        //auto irq_handler() { return m_irq_handler.bind(); }

        // reset the LFO state
        public void reset_lfo() { m_lfo_counter = 0; }


        // clock the LFO, updating m_lfo_am and return the signed PM value
        //s8 clock_lfo();

        // clock the noise generator
        //void clock_noise();


        // update the state of the given timer
        //-------------------------------------------------
        //  update_timer - update the state of the given
        //  timer
        //-------------------------------------------------
        //template<class RegisterType>
        void update_timer(u8 tnum, u8 enable)
        {
            // if the timer is live, but not currently enabled, set the timer
            if (enable != 0 && !m_timer[tnum].enabled())
            {
                // each timer clock is n channels * 4 operators * prescale factor (2/3/6)
                u32 clockscale = m_regs.CHANNELS * 4U * m_clock_prescale;

                // period comes from the registers, and is different for each
                u32 period = (tnum == 0) ? (1024U - m_regs.timer_a_value()) : 16U * (256U - m_regs.timer_b_value());

                // reset it
                m_timer[tnum].adjust(attotime.from_hz(m_device.clock()) * (period * clockscale), tnum);
            }

            // if the timer is not live, ensure it is not enabled
            else if (enable == 0)
                m_timer[tnum].enable(false);
        }


        // timer callback
        //TIMER_CALLBACK_MEMBER(timer_handler);


        // schedule an interrupt check
        //-------------------------------------------------
        //  schedule_check_interrupts - schedule an
        //  interrupt check via timer
        //-------------------------------------------------
        //template<class RegisterType>
        void schedule_check_interrupts()
        {
            // if we're currently executing a CPU, schedule the interrupt check;
            // otherwise, do it directly
            var scheduler = m_device.machine().scheduler();
            if (scheduler.currently_executing() != null)
                scheduler.synchronize(check_interrupts, 0);  //scheduler.synchronize(timer_expired_delegate(FUNC(ymfm_engine_base<RegisterType>::check_interrupts), this), 0);
            else
                check_interrupts(null, 0);
        }


        // check interrupts
        //TIMER_CALLBACK_MEMBER(check_interrupts);
        //-------------------------------------------------
        //  check_interrupts - check the interrupt sources
        //  for interrupts
        //-------------------------------------------------
        //template<class RegisterType>
        void check_interrupts(object ptr, int param)
        {
            // update the state
            u8 old_state = m_irq_state;
            m_irq_state = ((m_status & m_irq_mask) != 0) ? (u8)1 : (u8)0;

            // if changed, signal the new state
            if (old_state != m_irq_state && !m_irq_handler.isnull())
                m_irq_handler.op(m_irq_state != 0 ? g.ASSERT_LINE : g.CLEAR_LINE);
        }


        // handle a mode register write
        //TIMER_CALLBACK_MEMBER(synced_mode_w);
        //-------------------------------------------------
        //  synced_mode_w - handle a mode register write
        //  via timer callback
        //-------------------------------------------------
        //template<class RegisterType>
        void synced_mode_w(object ptr, int param)
        {
            // actually write the mode register now
            m_regs.write(m_regs.REG_MODE, (u8)param);

            // reset timer status
            if (m_regs.reset_timer_b() != 0)
                set_reset_status(0, STATUS_TIMERB);
            if (m_regs.reset_timer_a() != 0)
                set_reset_status(0, STATUS_TIMERA);

            // load timers
            update_timer(1, m_regs.load_timer_b());
            update_timer(0, m_regs.load_timer_a());
        }
    }


    //template<>
    //s8 ymfm_engine_base<ymopm_registers>::clock_lfo();

    //template<>
    //void ymfm_engine_base<ymopm_registers>::clock_noise();


    // ======================> template instantiations
    //extern template class ymfm_engine_base<ymopm_registers>;
    //extern template class ymfm_engine_base<ymopn_registers>;
    //extern template class ymfm_engine_base<ymopna_registers>;

    //using ymopm_engine = ymfm_engine_base<ymopm_registers>;
    //using ymopn_engine = ymfm_engine_base<ymopn_registers>;
    //using ymopna_engine = ymfm_engine_base<ymopna_registers>;
}
