// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;

using static mame.device_global;
using static mame.diexec_global;
using static mame.emucore_global;
using static mame.z80ctc_global;
using static mame.z80ctc_internal;
using static mame.z80daisy_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> z80ctc_channel_device
    // a single channel within the CTC
    public class z80ctc_channel_device : device_t
    {
        //DEFINE_DEVICE_TYPE(Z80CTC_CHANNEL, z80ctc_channel_device, "z80ctc_channel", "Z80 CTC Channel")
        public static readonly emu.detail.device_type_impl Z80CTC_CHANNEL = DEFINE_DEVICE_TYPE("z80ctc_channel", "Z80 CTC Channel", (type, mconfig, tag, owner, clock) => { return new z80ctc_channel_device(mconfig, tag, owner, clock); });


        //friend class z80ctc_device;


        const int VERBOSE = 0;
        //#include "logmacro.h"
        void LOG(string format, params object [] args) { logmacro_global.LOG(VERBOSE, this, format, args); }


        required_device<z80ctc_device> m_device; // pointer back to our device
        public int m_index;                // our channel index
        u16 m_mode;                 // current mode
        u16 m_tconst;               // time constant
        u16 m_down;                 // down counter (clock mode only)
        bool m_extclk;               // current signal from the external clock
        emu_timer m_timer;                // array of active timers
        public u8 m_int_state;            // interrupt status (for daisy chain)
        emu_timer m_zc_to_timer;          // zc to pulse timer


        // construction/destruction
        z80ctc_channel_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, Z80CTC_CHANNEL, tag, owner, clock)
        {
            m_device = new required_device<z80ctc_device>(this, DEVICE_SELF_OWNER);
            m_index = 0;
            m_mode = 0;
            m_tconst = 0;
            m_down = 0;
            m_extclk = false;
            m_timer = null;
            m_int_state = 0;
            m_zc_to_timer = null;
        }


        // device-level overrides
        protected override void device_start()
        {
            // initialize state
            m_timer = machine().scheduler().timer_alloc(timer_callback);
            m_zc_to_timer = machine().scheduler().timer_alloc(zc_to_callback);

            // register for save states
            save_item(NAME(new { m_mode }));
            save_item(NAME(new { m_tconst }));
            save_item(NAME(new { m_down }));
            save_item(NAME(new { m_extclk }));
            save_item(NAME(new { m_int_state }));
        }


        protected override void device_reset()
        {
            m_mode = RESET_ACTIVE;
            m_tconst = 0x100;
            m_timer.adjust(attotime.never);
            m_int_state = 0;
        }


        u8 read()
        {
            // if we're in counter mode, just return the count
            if (!m_timer.enabled() || (m_mode & WAITING_FOR_TRIG) != 0)
                return (u8)m_down;

            // else compute the down counter value
            else
            {
                attotime period;
                if ((m_mode & MODE) == MODE_COUNTER)
                    period = clocks_to_attotime(1);
                else
                    period = m_device.op0.clocks_to_attotime((m_mode & PRESCALER) == PRESCALER_16 ? 16U : 256U);

                LOG("CTC clock {0}\n", period.as_hz());

                if (!m_timer.remaining().is_never())
                    return (u8)((m_timer.remaining().as_double() / period.as_double()) + 1.0);
                else
                    return 0;
            }
        }


        public void write(u8 data)
        {
            // if we're waiting for a time constant, this is it
            if ((m_mode & CONSTANT) == CONSTANT_LOAD)
            {
                LOG("Time constant = {0}\n", data);

                // set the time constant (0 -> 0x100)
                m_tconst = data != 0 ? data : (u16)0x100;

                // clear the internal mode -- we're no longer waiting
                m_mode &= unchecked((u16)~CONSTANT);

                // also clear the reset, since the constant gets it going again
                m_mode &= unchecked((u16)~RESET);

                // if we're triggering on the time constant, reset the down counter now
                if ((m_mode & MODE) == MODE_COUNTER || (m_mode & TRIGGER) == TRIGGER_AUTO)
                {
                    attotime curperiod = period();
                    m_timer.adjust(curperiod, 0, curperiod);
                }

                // else set the bit indicating that we're waiting for the appropriate trigger
                else
                {
                    m_mode |= WAITING_FOR_TRIG;
                    m_timer.adjust(clocks_to_attotime(1));
                }

                // also set the down counter in case we're clocking externally
                m_down = m_tconst;
            }

            // if we're writing the interrupt vector, handle it specially
#if false   // Tatsuyuki Satoh changes
            // The 'Z80family handbook' wrote,
            // interrupt vector is able to set for even channel (0 or 2)
            else if ((data & CONTROL) == CONTROL_VECTOR && (m_index & 1) == 0)
#else
            else if ((data & CONTROL) == CONTROL_VECTOR && m_index == 0)
#endif
            {
                m_device.op0.m_vector = (u8)(data & 0xf8);
                LOG("Vector = {0}\n", m_device.op0.m_vector);
            }

            // this must be a control word
            else if ((data & CONTROL) == CONTROL_WORD)
            {
                // (mode change without reset?)
                if ((m_mode & MODE) == MODE_TIMER && (data & MODE) == MODE_COUNTER && (data & RESET) == 0)
                {
                    m_timer.adjust(attotime.never);
                }

                // if we're being reset, clear out any pending timers for this channel
                if ((data & RESET) == RESET_ACTIVE)
                {
                    // remember the present count
                    m_down = read();
                    m_timer.adjust(attotime.never);
                    // note that we don't clear the interrupt state here!
                }

                // set the new mode
                m_mode = data;
                LOG("Channel mode = {0}\n", data);

                // clearing this bit resets the interrupt state regardless of M1 activity (or lack thereof)
                if ((data & INTERRUPT) == INTERRUPT_OFF && (m_int_state & Z80_DAISY_INT) != 0)
                {
                    m_int_state &= unchecked((u8)~Z80_DAISY_INT);
                    LOG("Interrupt forced off\n");
                    m_device.op0.interrupt_check();
                }
            }
        }


        attotime period()
        {
            // if reset active, no period
            if ((m_mode & RESET) == RESET_ACTIVE)
                return attotime.never;

            // if counter mode, no real period unless the channel clock is specifically configured
            if ((m_mode & MODE) == MODE_COUNTER)
                return clocks_to_attotime(m_tconst);

            // compute the period
            attotime period = m_device.op0.clocks_to_attotime((m_mode & PRESCALER) == PRESCALER_16 ? 16U : 256U);
            return period * m_tconst;
        }


        public void trigger(bool state)
        {
            // see if the trigger value has changed
            if (state != m_extclk)
            {
                m_extclk = state;

                // see if this is the active edge of the trigger
                if (((m_mode & EDGE) == EDGE_RISING && state) || ((m_mode & EDGE) == EDGE_FALLING && !state))
                {
                    // if we're waiting for a trigger, start the timer
                    if ((m_mode & WAITING_FOR_TRIG) != 0 && (m_mode & MODE) == MODE_TIMER)
                    {
                        attotime curperiod = period();
                        LOG("Period = {0}\n", curperiod.as_string());
                        m_timer.adjust(curperiod, 0, curperiod);
                    }

                    // we're no longer waiting
                    m_mode &= unchecked((u16)~WAITING_FOR_TRIG);

                    // if we're clocking externally, decrement the count
                    if ((m_mode & MODE) == MODE_COUNTER)
                    {
                        // if we hit zero, do the same thing as for a timer interrupt
                        if (--m_down == 0)
                            timer_callback(null, 0);
                    }
                }
            }
        }


        //TIMER_CALLBACK_MEMBER(timer_callback);
        void timer_callback(object ptr, s32 param)  //void *ptr, s32 param)
        {
            if ((m_mode & WAITING_FOR_TRIG) != 0)
            {
                attotime curperiod = period();
                LOG("Period = {0}\n", curperiod.as_string());
                m_timer.adjust(curperiod, 0, curperiod);

                // we're no longer waiting
                m_mode &= unchecked((u16)~WAITING_FOR_TRIG);

                return;
            }

            // down counter has reached zero - see if we should interrupt
            if ((m_mode & INTERRUPT) == INTERRUPT_ON)
            {
                m_int_state |= Z80_DAISY_INT;
                LOG("Timer interrupt\n");
                m_device.op0.interrupt_check();
            }

            // generate the clock pulse
            m_device.op0.m_zc_cb[m_index].op_s32(1);
            m_zc_to_timer.adjust(m_device.op0.clocks_to_attotime(1));

            // reset the down counter
            m_down = m_tconst;
        }


        //TIMER_CALLBACK_MEMBER(zc_to_callback);
        void zc_to_callback(object ptr, s32 param)  //void *ptr, s32 param)
        {
            m_device.op0.m_zc_cb[m_index].op_s32(0);
        }
    }


    // ======================> z80ctc_device
    public class z80ctc_device : device_t
                                 //device_z80daisy_interface
    {
        //DEFINE_DEVICE_TYPE(Z80CTC, z80ctc_device, "z80ctc", "Z80 CTC")
        public static readonly emu.detail.device_type_impl Z80CTC = DEFINE_DEVICE_TYPE("z80ctc", "Z80 CTC", (type, mconfig, tag, owner, clock) => { return new z80ctc_device(mconfig, tag, owner, clock); });


        public class device_z80daisy_interface_z80ctc_device : device_z80daisy_interface
        {
            public device_z80daisy_interface_z80ctc_device(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override int z80daisy_irq_state() { return ((z80ctc_device)device()).device_z80daisy_interface_z80daisy_irq_state(); }
            public override int z80daisy_irq_ack() { return ((z80ctc_device)device()).device_z80daisy_interface_z80daisy_irq_ack(); }
            public override void z80daisy_irq_reti() { ((z80ctc_device)device()).device_z80daisy_interface_z80daisy_irq_reti(); }
        }


        const int VERBOSE = 0;
        //#include "logmacro.h"
        void LOG(string format, params object [] args) { logmacro_global.LOG(VERBOSE, this, format, args); }


        //friend class z80ctc_channel_device;


        device_z80daisy_interface_z80ctc_device m_z80daisy;


        // internal state
        devcb_write_line m_intr_cb;              // interrupt callback
        public devcb_write_line.array<u64_const_4> m_zc_cb;             // zero crossing/timer output callbacks

        public u8 m_vector;               // interrupt vector

        // subdevice for each channel
        required_device_array<z80ctc_channel_device, u32_const_4> m_channel;


        // construction/destruction
        z80ctc_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, Z80CTC, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_z80daisy_interface_z80ctc_device(mconfig, this));  //, device_z80daisy_interface(mconfig, *this)
            m_z80daisy = GetClassInterface<device_z80daisy_interface_z80ctc_device>();


            m_intr_cb = new devcb_write_line(this);
            m_zc_cb = new devcb_write_line.array<u64_const_4>(this, () => { return new devcb_write_line(this); });
            m_vector = 0;
            m_channel = new required_device_array<z80ctc_channel_device, u32_const_4>(this, "ch{0}", 0U, (base_, tag_) => { return new device_finder<z80ctc_channel_device, bool_const_true>(base_, tag_); });
        }


        public devcb_write_line.binder intr_callback() { return m_intr_cb.bind(); }  //auto intr_callback() { return m_intr_cb.bind(); }
        public devcb_write_line.binder zc_callback<int_Channel>() where int_Channel : int_const, new() { int Channel = new int_Channel().value; return m_zc_cb[Channel].bind(); }  //template <int Channel> auto zc_callback() { return m_zc_cb[Channel].bind(); } // m_zc_cb[3] not supported on a standard ctc, only used for the tmpz84c015
        //template <int Channel> void set_clk(u32 clock) { channel_config(Channel).set_clock(clock); }
        //template <int Channel> void set_clk(const XTAL &xtal) { channel_config(Channel).set_clock(xtal); }


        // read/write handlers
        public uint8_t read(offs_t offset) { throw new emu_unimplemented(); }

        public void write(offs_t offset, uint8_t data)
        {
            m_channel[(int)(offset & 3)].op0.write(data);
        }


        public void trg0(int state) { m_channel[0].op0.trigger(state != 0); }  //DECLARE_WRITE_LINE_MEMBER( trg0 );
        public void trg1(int state) { m_channel[1].op0.trigger(state != 0); }  //DECLARE_WRITE_LINE_MEMBER( trg1 );
        public void trg2(int state) { m_channel[2].op0.trigger(state != 0); }  //DECLARE_WRITE_LINE_MEMBER( trg2 );
        public void trg3(int state) { m_channel[3].op0.trigger(state != 0); }  //DECLARE_WRITE_LINE_MEMBER( trg3 );


        //u16 get_channel_constant(int ch) const { return m_channel[ch]->m_tconst; }


        // device-level overrides
        protected override void device_add_mconfig(machine_config config)
        {
            for (int ch = 0; ch < 4; ch++)
            {
                Z80CTC_CHANNEL(config, m_channel[ch]);

                // assign channel index
                m_channel[ch].op0.m_index = ch;
            }
        }


        protected override void device_resolve_objects()
        {
            // resolve callbacks
            m_intr_cb.resolve_safe();
            m_zc_cb.resolve_all_safe();
        }


        protected override void device_start()
        {
            // register for save states
            save_item(NAME(new { m_vector }));
        }


        protected override void device_reset_after_children()
        {
            // check for interrupts
            interrupt_check();
            LOG("CTC Reset\n");
        }


        // z80daisy_interface overrides
        protected virtual int device_z80daisy_interface_z80daisy_irq_state()
        {
            LOG("CTC IRQ state = {0}{1}{2}{3}\n", m_channel[0].op0.m_int_state, m_channel[1].op0.m_int_state, m_channel[2].op0.m_int_state, m_channel[3].op0.m_int_state);

            // loop over all channels
            int state = 0;
            for (int ch = 0; ch < 4; ch++)
            {
                // if we're servicing a request, don't indicate more interrupts
                if ((m_channel[ch].op0.m_int_state & Z80_DAISY_IEO) != 0)
                {
                    state |= Z80_DAISY_IEO;
                    break;
                }
                state |= m_channel[ch].op0.m_int_state;
            }

            return state;
        }


        protected virtual int device_z80daisy_interface_z80daisy_irq_ack() { throw new emu_unimplemented(); }
        protected virtual void device_z80daisy_interface_z80daisy_irq_reti() { throw new emu_unimplemented(); }


        // internal helpers
        public void interrupt_check()
        {
            int state = (m_z80daisy.z80daisy_irq_state() & Z80_DAISY_INT) != 0 ? ASSERT_LINE : CLEAR_LINE;
            m_intr_cb.op_s32(state);
        }


        //z80ctc_channel_device &channel_config(int ch) { return *subdevice<z80ctc_channel_device>(m_channel[ch].finder_tag()); }
    }


    static class z80ctc_internal
    {
        //**************************************************************************
        //  CONSTANTS
        //**************************************************************************

        // these are the bits of the incoming commands to the CTC
        public const u16 INTERRUPT         = 0x80;
        public const u16 INTERRUPT_ON      = 0x80;
        public const u16 INTERRUPT_OFF     = 0x00;

        public const u16 MODE              = 0x40;
        public const u16 MODE_TIMER        = 0x00;
        public const u16 MODE_COUNTER      = 0x40;

        public const u16 PRESCALER         = 0x20;
        //constexpr u16 PRESCALER_256     = 0x20;
        public const u16 PRESCALER_16      = 0x00;

        public const u16 EDGE              = 0x10;
        public const u16 EDGE_FALLING      = 0x00;
        public const u16 EDGE_RISING       = 0x10;

        public const u16 TRIGGER           = 0x08;
        public const u16 TRIGGER_AUTO      = 0x00;
        //constexpr u16 TRIGGER_CLOCK     = 0x08;

        public const u16 CONSTANT          = 0x04;
        public const u16 CONSTANT_LOAD     = 0x04;
        //constexpr u16 CONSTANT_NONE     = 0x00;

        public const u16 RESET             = 0x02;
        //constexpr u16 RESET_CONTINUE    = 0x00;
        public const u16 RESET_ACTIVE      = 0x02;

        public const u16 CONTROL           = 0x01;
        public const u16 CONTROL_VECTOR    = 0x00;
        public const u16 CONTROL_WORD      = 0x01;

        // these extra bits help us keep things accurate
        public const u16 WAITING_FOR_TRIG  = 0x100;
    }


    static class z80ctc_global
    {
        public static z80ctc_channel_device Z80CTC_CHANNEL<bool_Required>(machine_config mconfig, device_finder<z80ctc_channel_device, bool_Required> finder) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, z80ctc_channel_device.Z80CTC_CHANNEL, 0); }
        public static z80ctc_device Z80CTC<bool_Required>(machine_config mconfig, device_finder<z80ctc_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, z80ctc_device.Z80CTC, clock); }
    }
}
