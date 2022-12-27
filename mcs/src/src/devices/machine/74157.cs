// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u8 = System.Byte;
using u32 = System.UInt32;

using static mame._74157_global;
using static mame.device_global;
using static mame.emucore_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> ls157_device
    public class ls157_device : device_t
    {
        //DEFINE_DEVICE_TYPE(LS157, ls157_device, "ls157", "74LS157 Quad 2-to-1 Multiplexer")
        public static readonly emu.detail.device_type_impl LS157 = DEFINE_DEVICE_TYPE("ls157", "74LS157 Quad 2-to-1 Multiplexer", (type, mconfig, tag, owner, clock) => { return new ls157_device(mconfig, tag, owner, clock); });


        // callbacks & configuration
        devcb_read8 m_a_in_cb;
        devcb_read8 m_b_in_cb;
        devcb_write8 m_out_cb;
        u8 m_data_mask;

        // internal state
        u8 m_a;
        u8 m_b;
        bool m_select;
        bool m_strobe;


        // construction/destruction
        ls157_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : this(mconfig, LS157, tag, owner, clock, 0x0f)
        { }


        ls157_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock, u8 mask)
            : base(mconfig, type, tag, owner, clock)
        {
            m_a_in_cb = new devcb_read8(this);
            m_b_in_cb = new devcb_read8(this);
            m_out_cb = new devcb_write8(this);
            m_data_mask = mask;


            m_a = 0;
            m_b = 0;
            m_select = false;
            m_strobe = false;
        }


        public devcb_read8.binder a_in_callback() { return m_a_in_cb.bind(); }  //auto a_in_callback() { return m_a_in_cb.bind(); }
        public devcb_read8.binder b_in_callback() { return m_b_in_cb.bind(); }  //auto b_in_callback() { return m_b_in_cb.bind(); }
        //auto out_callback() { return m_out_cb.bind(); }

        // data writes
        //void a_w(u8 data);
        //void b_w(u8 data);
        //void ab_w(u8 data);
        //void ba_w(u8 data);
        //void interleave_w(u8 data);

        // data line writes
        //DECLARE_WRITE_LINE_MEMBER(a0_w);
        //DECLARE_WRITE_LINE_MEMBER(a1_w);
        //DECLARE_WRITE_LINE_MEMBER(a2_w);
        //DECLARE_WRITE_LINE_MEMBER(a3_w);
        //DECLARE_WRITE_LINE_MEMBER(b0_w);
        //DECLARE_WRITE_LINE_MEMBER(b1_w);
        //DECLARE_WRITE_LINE_MEMBER(b2_w);
        //DECLARE_WRITE_LINE_MEMBER(b3_w);

        // control line writes
        //DECLARE_WRITE_LINE_MEMBER(select_w);
        public void select_w(int state) { throw new emu_unimplemented(); }
        //DECLARE_WRITE_LINE_MEMBER(strobe_w);

        // output read
        public u8 output_r() { throw new emu_unimplemented(); }


        // device-level overrides
        protected override void device_start()
        {
            // resolve callbacks
            m_a_in_cb.resolve();
            m_b_in_cb.resolve();
            m_out_cb.resolve();

            // register items for save state
            save_item(NAME(new { m_a }));
            save_item(NAME(new { m_b }));
            save_item(NAME(new { m_select }));
            save_item(NAME(new { m_strobe }));
        }


        // internal helpers
        //void write_a_bit(int bit, bool state);
        //void write_b_bit(int bit, bool state);
        //void update_output();
    }


    // ======================> ls157_x2_device
    //class ls157_x2_device : public ls157_device

    // ======================> hc157_device
    //class hc157_device : public ls157_device

    // ======================> hct157_device
    //class hct157_device : public ls157_device


    public static class _74157_global
    {
        public static ls157_device LS157(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<ls157_device>(mconfig, tag, ls157_device.LS157, 0); }
        public static ls157_device LS157<bool_Required>(machine_config mconfig, device_finder<ls157_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, ls157_device.LS157, clock); }
    }
}
