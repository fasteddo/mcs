// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using size_t = System.UInt64;
using u8 = System.Byte;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.adc0808_global;
using static mame.device_global;
using static mame.emucore_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************
    public class adc0808_device : device_t
    {
        //DEFINE_DEVICE_TYPE(ADC0808, adc0808_device, "adc0808", "ADC0808 A/D Converter")
        public static readonly emu.detail.device_type_impl ADC0808 = DEFINE_DEVICE_TYPE("adc0808", "ADC0808 A/D Converter", (type, mconfig, tag, owner, clock) => { return new adc0808_device(mconfig, tag, owner, clock); });


        const bool VERBOSE = false;


        // callbacks
        devcb_write_line m_eoc_cb;
        devcb_write_line m_eoc_ff_cb;
        devcb_read8.array<u64_const_8> m_in_cb;

        enum state : int
        {
            STATE_IDLE,
            STATE_CONVERSION_START,
            STATE_CONVERSION_READY,
            STATE_CONVERSION_RUNNING
        }
        state m_state;

        emu_timer m_cycle_timer;

        // state
        int m_start;
        int m_address;
        uint8_t m_sar;
        bool m_eoc;


        // construction/destruction
        protected adc0808_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, ADC0808, tag, owner, clock)
        { }


        protected adc0808_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_eoc_cb = new devcb_write_line(this);
            m_eoc_ff_cb = new devcb_write_line(this);
            m_in_cb = new devcb_read8.array<u64_const_8>(this, () => { return new devcb_read8(this); });
            m_state = state.STATE_IDLE;
            m_cycle_timer = null;
            m_start = 0;
            m_address = 0;
            m_sar = 0xff;
            m_eoc = true;
        }


        //auto eoc_callback() { return m_eoc_cb.bind(); }
        //auto eoc_ff_callback() { return m_eoc_ff_cb.bind(); }
        public devcb_read8.binder in_callback<std_size_t_Bit>() where std_size_t_Bit : u32_const, new() { size_t Bit = new std_size_t_Bit().value;  return m_in_cb[Bit].bind(); }  //template <std::size_t Bit> auto in_callback() { return m_in_cb[Bit].bind(); }


        //**************************************************************************
        //  INTERFACE
        //**************************************************************************
        public u8 data_r()
        {
            if (!machine().side_effects_disabled())
            {
                if (VERBOSE)
                    logerror("data_r: {0}\n", m_sar);

                // oe connected to flip-flop clear
                m_eoc_ff_cb.op_s32(0);
            }

            return m_sar;
        }


        void address_w(u8 data)
        {
            m_address = data & 7;
        }


        //WRITE_LINE_MEMBER( adc0808_device::start_w )
        void start_w(int state_)
        {
            if (m_start == state_)
                return;

            if (state_ != 0 && m_start == 0)
            {
                m_state = state.STATE_CONVERSION_START;
                m_cycle_timer.adjust(attotime.from_hz(clock()));
            }
            else if (state_ == 0 && m_start != 0)
            {
                m_cycle_timer.adjust(attotime.from_hz(clock()));
            }

            m_start = state_;
        }


        //DECLARE_READ_LINE_MEMBER(eoc_r);


        // common hookups

        public void address_offset_start_w(offs_t offset, u8 data) // start and ale connected, address to the address bus
        {
            if (VERBOSE)
                logerror("address_offset_start_w {0} {1}\n", offset, data);

            start_w(1);
            address_w((u8)offset);
            start_w(0);
        }


        //void address_data_start_w(u8 data); // start and ale connected, address to the data bus


        // device-level overrides
        protected override void device_start()
        {
            // resolve callbacks
            m_eoc_cb.resolve_safe();
            m_eoc_ff_cb.resolve_safe();
            m_in_cb.resolve_all_safe_u8(0xff);

            // allocate timers
            m_cycle_timer = timer_alloc(update_state);
            m_cycle_timer.adjust(attotime.zero, 0, attotime.from_hz(clock()));

            // register for save states
            save_item(NAME(new { m_state }));
            save_item(NAME(new { m_start }));
            save_item(NAME(new { m_address }));
            save_item(NAME(new { m_sar }));
            save_item(NAME(new { m_eoc }));
        }


        //-------------------------------------------------
        //  update_state
        //-------------------------------------------------
        //TIMER_CALLBACK_MEMBER(update_state);
        void update_state(s32 param)
        {
            switch (m_state)
            {
                case state.STATE_IDLE:
                    m_cycle_timer.adjust(attotime.never);
                    return;

                // ready; beginning to run conversion cycle
                case state.STATE_CONVERSION_READY:
                    m_state = state.STATE_CONVERSION_RUNNING;

                    m_sar = m_in_cb[m_address].op_u8(0);
                    m_eoc = false;
                    m_eoc_cb.op_s32(m_eoc ? 1 : 0);

                    // the conversion takes 8 steps per 8 cycles
                    m_cycle_timer.adjust(attotime.from_ticks(64, clock()));
                    return;

                // start; mark ourselves as ready for conversion 1 cycle later
                case state.STATE_CONVERSION_START:
                    m_state = state.STATE_CONVERSION_READY;
                    m_cycle_timer.adjust(attotime.from_hz(clock()));
                    return;

                // end of conversion cycle
                case state.STATE_CONVERSION_RUNNING:
                {
                    m_state = state.STATE_IDLE;
                    uint8_t start_sar = m_sar;
                    m_sar = m_in_cb[m_address].op_u8(0);

                    m_eoc = true;
                    m_eoc_cb.op_s32(m_eoc ? 1 : 0);
                    m_eoc_ff_cb.op_s32(1);

                    if (VERBOSE)
                        logerror("Conversion finished, result {0}\n", m_sar);

                    if (m_sar != start_sar)
                        logerror("Conversion finished, should fail - starting value {0}, ending value {1}", start_sar, m_sar);

                    // eoc is delayed by one cycle
                    m_cycle_timer.adjust(attotime.never);
                    break;
                }
            }
        }
    }


    public class adc0809_device : adc0808_device
    {
        //DEFINE_DEVICE_TYPE(ADC0809, adc0809_device, "adc0809", "ADC0809 A/D Converter")
        public static readonly emu.detail.device_type_impl ADC0809 = DEFINE_DEVICE_TYPE("adc0809", "ADC0809 A/D Converter", (type, mconfig, tag, owner, clock) => { return new adc0809_device(mconfig, tag, owner, clock); });

        adc0809_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, ADC0809, tag, owner, clock)
        { }
    }


    //class m58990_device : public adc0808_device


    public static class adc0808_global
    {
        public static adc0809_device ADC0809(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<adc0809_device>(mconfig, tag, adc0809_device.ADC0809, clock); }
    }
}
