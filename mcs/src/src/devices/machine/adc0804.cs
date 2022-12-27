// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using s32 = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;

using static mame.adc0804_global;
using static mame.device_global;
using static mame.emucore_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> adc0804_device
    public class adc0804_device : device_t
    {
        //DEFINE_DEVICE_TYPE(ADC0804, adc0804_device, "adc0804", "ADC0804 A/D Converter")
        public static readonly emu.detail.device_type_impl ADC0804 = DEFINE_DEVICE_TYPE("adc0804", "ADC0804 A/D Converter", (type, mconfig, tag, owner, clock) => { return new adc0804_device(mconfig, tag, owner, clock); });


        //static const int s_conversion_cycles;


        enum read_mode
        {
            RD_STROBED = 0,
            RD_BITBANGED,
            RD_GROUNDED
        }


        // callback objects
        devcb_read8 m_vin_callback;
        devcb_write_line m_intr_callback;

        // timing parameters
        double m_res;
        double m_cap;
        attotime m_fclk_rc;

        // conversion timer
        emu_timer m_timer;

        // inputs
        read_mode m_rd_mode;
        bool m_rd_active;
        bool m_wr_active;

        // internal state
        u8 m_result;
        bool m_intr_active;


        // device type constructors
        adc0804_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : this(mconfig, ADC0804, tag, owner, clock)
        { }


        adc0804_device(machine_config mconfig, string tag, device_t owner, double r, double c)
            : this(mconfig, tag, owner, 0U)
        {
            throw new emu_unimplemented();
#if false
            set_rc(r, c);
#endif
        }


        adc0804_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_vin_callback = new devcb_read8(this);
            m_intr_callback = new devcb_write_line(this);
            m_res = 0.0;
            m_cap = 0.0;
            m_fclk_rc = attotime.zero;
            m_timer = null;
            m_rd_mode = read_mode.RD_STROBED;
            m_rd_active = false;
            m_wr_active = false;
            m_result = 0;
            m_intr_active = false;
        }


        // callback configuration
        public devcb_read8.binder vin_callback() { return m_vin_callback.bind(); }  //auto vin_callback() { return m_vin_callback.bind(); }
        //auto intr_callback() { return m_intr_callback.bind(); }

        // misc. configuration
        //void set_rc(double res, double cap) { assert(!configured()); m_res = res; m_cap = cap; }
        //void set_rd_mode(read_mode mode) { assert(!configured()); m_rd_mode = mode; }

        // data bus interface
        public u8 read() { throw new emu_unimplemented(); }
        //u8 read_and_write();
        public void write(u8 data = 0) { throw new emu_unimplemented(); }

        // control line interface
        //DECLARE_WRITE_LINE_MEMBER(rd_w);
        //DECLARE_WRITE_LINE_MEMBER(wr_w);

        // status line interface
        //DECLARE_READ_LINE_MEMBER(intr_r) { return m_intr_active ? 0 : 1; }


        // device-level overrides
        protected override void device_resolve_objects()
        {
            m_vin_callback.resolve_safe_u8(0);
            m_intr_callback.resolve_safe();

            if (m_rd_mode == read_mode.RD_GROUNDED)
                m_rd_active = true;
        }


        protected override void device_start()
        {
            // calculate RC timing
            if (m_res == 0.0 || m_cap == 0.0)
                m_fclk_rc = attotime.zero;
            else
                m_fclk_rc = attotime.from_double(m_res * m_cap / 1.1);

            // create timer
            m_timer = machine().scheduler().timer_alloc(conversion_done);

            // save state
            if (m_rd_mode == read_mode.RD_BITBANGED)
                save_item(NAME(new { m_rd_active }));
            save_item(NAME(new { m_wr_active }));
            save_item(NAME(new { m_result }));
            save_item(NAME(new { m_intr_active }));
        }


        // internal helpers
        //void set_interrupt(bool state);
        //void conversion_start();


        //TIMER_CALLBACK_MEMBER(conversion_done);
        void conversion_done(object ptr, s32 param)  //void *ptr, s32 param)
        {
            throw new emu_unimplemented();
        }
    }


    // ======================> adc0803_device
    //class adc0803_device : public adc0804_device


    static class adc0804_global
    {
        public static adc0804_device ADC0804<bool_Required>(machine_config mconfig, device_finder<adc0804_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, adc0804_device.ADC0804, clock); }
    }
}
