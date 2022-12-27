// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_read_line = mame.devcb_read<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_read_line = devcb_read<int, 1U>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using offs_t = System.UInt32;  //using offs_t = u32;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame._6821pia_global;
using static mame.device_global;
using static mame.diexec_global;
using static mame.emucore_global;


namespace mame
{
    /***************************************************************************
        TYPE DEFINITIONS
    ***************************************************************************/

    // ======================> pia6821_device
    public class pia6821_device : device_t
    {
        //DEFINE_DEVICE_TYPE(PIA6821, pia6821_device, "pia6821", "6821 PIA")
        public static readonly emu.detail.device_type_impl PIA6821 = DEFINE_DEVICE_TYPE("pia6821", "6821 PIA", (type, mconfig, tag, owner, clock) => { return new pia6821_device(mconfig, tag, owner, clock); });


        devcb_read8 m_in_a_handler;
        devcb_read8 m_in_b_handler;
        devcb_read_line m_in_ca1_handler;
        devcb_read_line m_in_cb1_handler;
        devcb_read_line m_in_ca2_handler;
        devcb_write8 m_out_a_handler;
        devcb_write8 m_out_b_handler;
        devcb_read8 m_ts_b_handler;
        devcb_write_line m_ca2_handler;
        devcb_write_line m_cb2_handler;
        devcb_write_line m_irqa_handler;
        devcb_write_line m_irqb_handler;

        uint8_t m_in_a;
        uint8_t m_in_ca1;
        uint8_t m_in_ca2;
        uint8_t m_out_a;
        uint8_t m_a_input_overrides_output_mask;
        uint8_t m_out_ca2;
        uint8_t m_ddr_a;
        uint8_t m_ctl_a;
        bool m_irq_a1;
        bool m_irq_a2;
        uint8_t m_irq_a_state;

        uint8_t m_in_b;
        uint8_t m_in_cb1;
        uint8_t m_in_cb2;
        uint8_t m_out_b;
        uint8_t m_out_cb2;
        uint8_t m_last_out_cb2_z;
        uint8_t m_ddr_b;
        uint8_t m_ctl_b;
        bool m_irq_b1;
        bool m_irq_b2;
        uint8_t m_irq_b_state;

        // variables that indicate if access a line externally -
        // used to for logging purposes ONLY
        bool m_in_a_pushed;
        bool m_out_a_needs_pulled;
        bool m_in_ca1_pushed;
        bool m_in_ca2_pushed;
        bool m_out_ca2_needs_pulled;
        bool m_in_b_pushed;
        bool m_out_b_needs_pulled;
        bool m_in_cb1_pushed;
        bool m_in_cb2_pushed;
        bool m_out_cb2_needs_pulled;
        bool m_logged_port_a_not_connected;
        bool m_logged_port_b_not_connected;
        bool m_logged_ca1_not_connected;
        bool m_logged_ca2_not_connected;
        bool m_logged_cb1_not_connected;
        bool m_logged_cb2_not_connected;


        // construction/destruction
        pia6821_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, PIA6821, tag, owner, clock)
        {
            m_in_a_handler = new devcb_read8(this);
            m_in_b_handler = new devcb_read8(this);
            m_in_ca1_handler = new devcb_read_line(this);
            m_in_cb1_handler = new devcb_read_line(this);
            m_in_ca2_handler = new devcb_read_line(this);
            m_out_a_handler = new devcb_write8(this);
            m_out_b_handler = new devcb_write8(this);
            m_ts_b_handler = new devcb_read8(this);
            m_ca2_handler = new devcb_write_line(this);
            m_cb2_handler = new devcb_write_line(this);
            m_irqa_handler = new devcb_write_line(this);
            m_irqb_handler = new devcb_write_line(this);
            m_in_a = 0;
            m_in_ca1 = 0;
            m_in_ca2 = 0;
            m_out_a = 0;
            m_a_input_overrides_output_mask = 0;
            m_out_ca2 = 0;
            m_ddr_a = 0;
            m_ctl_a = 0;
            m_irq_a1 = false;
            m_irq_a2 = false;
            m_irq_a_state = 0;
            m_in_b = 0;
            m_in_cb1 = 0;
            m_in_cb2 = 0;
            m_out_b = 0;
            m_out_cb2 = 0;
            m_last_out_cb2_z = 0;
            m_ddr_b = 0;
            m_ctl_b = 0;
            m_irq_b1 = false;
            m_irq_b2 = false;
            m_irq_b_state = 0;
            m_in_a_pushed = false;
            m_out_a_needs_pulled = false;
            m_in_ca1_pushed = false;
            m_in_ca2_pushed = false;
            m_out_ca2_needs_pulled = false;
            m_in_b_pushed = false;
            m_out_b_needs_pulled = false;
            m_in_cb1_pushed = false;
            m_in_cb2_pushed = false;
            m_out_cb2_needs_pulled = false;
            m_logged_port_a_not_connected = false;
            m_logged_port_b_not_connected = false;
            m_logged_ca1_not_connected = false;
            m_logged_ca2_not_connected = false;
            m_logged_cb1_not_connected = false;
            m_logged_cb2_not_connected = false;
        }


        // TODO: REMOVE THESE
        public devcb_read8.binder readpa_handler() { return m_in_a_handler.bind(); }  //auto readpa_handler() { return m_in_a_handler.bind(); }
        public devcb_read8.binder readpb_handler() { return m_in_b_handler.bind(); }  //auto readpb_handler() { return m_in_b_handler.bind(); }
        //auto readca1_handler() { return m_in_ca1_handler.bind(); }
        //auto readca2_handler() { return m_in_ca2_handler.bind(); }
        //auto readcb1_handler() { return m_in_cb1_handler.bind(); }

        public devcb_write8.binder writepa_handler() { return m_out_a_handler.bind(); }  //auto writepa_handler() { return m_out_a_handler.bind(); }
        public devcb_write8.binder writepb_handler() { return m_out_b_handler.bind(); }  //auto writepb_handler() { return m_out_b_handler.bind(); }
        //auto tspb_handler() { return m_ts_b_handler.bind(); }

        public devcb_write_line.binder ca2_handler() { return m_ca2_handler.bind(); }  //auto ca2_handler() { return m_ca2_handler.bind(); }
        public devcb_write_line.binder cb2_handler() { return m_cb2_handler.bind(); }  //auto cb2_handler() { return m_cb2_handler.bind(); }
        public devcb_write_line.binder irqa_handler() { return m_irqa_handler.bind(); }  //auto irqa_handler() { return m_irqa_handler.bind(); }
        public devcb_write_line.binder irqb_handler() { return m_irqb_handler.bind(); }  //auto irqb_handler() { return m_irqb_handler.bind(); }

        public uint8_t read(offs_t offset) { throw new emu_unimplemented(); }
        public void write(offs_t offset, uint8_t data) { throw new emu_unimplemented(); }
        public uint8_t read_alt(offs_t offset) { return read(((offset << 1) & 0x02) | ((offset >> 1) & 0x01)); }
        public void write_alt(offs_t offset, uint8_t data) { write(((offset << 1) & 0x02) | ((offset >> 1) & 0x01), data); }

        //uint8_t port_b_z_mask() const { return ~m_ddr_b; } // see notes

        //void porta_w(uint8_t data);
        //void write_porta_line(int line, bool state);
        //void set_a_input(uint8_t data);
        //uint8_t a_output();
        //void set_port_a_input_overrides_output_mask(uint8_t mask) { m_a_input_overrides_output_mask = mask; }

        //DECLARE_WRITE_LINE_MEMBER( pa0_w ) { write_porta_line(0, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa1_w ) { write_porta_line(1, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa2_w ) { write_porta_line(2, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa3_w ) { write_porta_line(3, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa4_w ) { write_porta_line(4, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa5_w ) { write_porta_line(5, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa6_w ) { write_porta_line(6, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa7_w ) { write_porta_line(7, state); }

        //DECLARE_WRITE_LINE_MEMBER( ca1_w );

        //DECLARE_WRITE_LINE_MEMBER( ca2_w );
        //bool ca2_output();
        //bool ca2_output_z();

        //void portb_w(uint8_t data);
        //void write_portb_line(int line, bool state);
        //uint8_t b_output();

        //DECLARE_WRITE_LINE_MEMBER( pb0_w ) { write_portb_line(0, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb1_w ) { write_portb_line(1, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb2_w ) { write_portb_line(2, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb3_w ) { write_portb_line(3, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb4_w ) { write_portb_line(4, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb5_w ) { write_portb_line(5, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb6_w ) { write_portb_line(6, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb7_w ) { write_portb_line(7, state); }

        //DECLARE_WRITE_LINE_MEMBER( cb1_w );

        //DECLARE_WRITE_LINE_MEMBER( cb2_w );
        //bool cb2_output();
        //bool cb2_output_z();

        public int irq_a_state() { return m_irq_a_state; }
        public int irq_b_state() { return m_irq_b_state; }


        // device-level overrides
        protected override void device_resolve_objects()
        {
            // resolve callbacks
            m_in_a_handler.resolve();
            m_in_b_handler.resolve();
            m_in_ca1_handler.resolve();
            m_in_cb1_handler.resolve();
            m_in_ca2_handler.resolve();
            m_out_a_handler.resolve();
            m_out_b_handler.resolve();
            m_ts_b_handler.resolve();
            m_ca2_handler.resolve();
            m_cb2_handler.resolve();
            m_irqa_handler.resolve_safe();
            m_irqb_handler.resolve_safe();
        }


        protected override void device_start()
        {
            m_in_a = 0xff;
            m_in_b = 0;
            m_in_ca1 = 1;  //true;
            m_in_ca2 = 1;  //true;
            m_in_cb1 = 0;
            m_in_cb2 = 0;
            m_in_a_pushed = false;
            m_out_a_needs_pulled = false;
            m_in_ca1_pushed = false;
            m_in_ca2_pushed = false;
            m_out_ca2_needs_pulled = false;
            m_in_b_pushed = false;
            m_out_b_needs_pulled = false;
            m_in_cb1_pushed = false;
            m_in_cb2_pushed = false;
            m_out_cb2_needs_pulled = false;
            m_logged_port_a_not_connected = false;
            m_logged_port_b_not_connected = false;
            m_logged_ca1_not_connected = false;
            m_logged_ca2_not_connected = false;
            m_logged_cb1_not_connected = false;
            m_logged_cb2_not_connected = false;

            save_item(NAME(new { m_in_a }));
            save_item(NAME(new { m_in_ca1 }));
            save_item(NAME(new { m_in_ca2 }));
            save_item(NAME(new { m_out_a }));
            save_item(NAME(new { m_out_ca2 }));
            save_item(NAME(new { m_ddr_a }));
            save_item(NAME(new { m_ctl_a }));
            save_item(NAME(new { m_irq_a1 }));
            save_item(NAME(new { m_irq_a2 }));
            save_item(NAME(new { m_irq_a_state }));
            save_item(NAME(new { m_in_b }));
            save_item(NAME(new { m_in_cb1 }));
            save_item(NAME(new { m_in_cb2 }));
            save_item(NAME(new { m_out_b }));
            save_item(NAME(new { m_out_cb2 }));
            save_item(NAME(new { m_last_out_cb2_z }));
            save_item(NAME(new { m_ddr_b }));
            save_item(NAME(new { m_ctl_b }));
            save_item(NAME(new { m_irq_b1 }));
            save_item(NAME(new { m_irq_b2 }));
            save_item(NAME(new { m_irq_b_state }));
            save_item(NAME(new { m_in_a_pushed }));
            save_item(NAME(new { m_out_a_needs_pulled }));
            save_item(NAME(new { m_in_ca1_pushed }));
            save_item(NAME(new { m_in_ca2_pushed }));
            save_item(NAME(new { m_out_ca2_needs_pulled }));
            save_item(NAME(new { m_in_b_pushed }));
            save_item(NAME(new { m_out_b_needs_pulled }));
            save_item(NAME(new { m_in_cb1_pushed }));
            save_item(NAME(new { m_in_cb2_pushed }));
            save_item(NAME(new { m_out_cb2_needs_pulled }));
        }


        protected override void device_reset()
        {
            //
            // set default read values.
            //
            // ports A,CA1,CA2 default to 1
            // ports B,CB1,CB2 are three-state and undefined (set to 0)
            //
            m_out_a = 0;
            m_out_ca2 = 0;
            m_ddr_a = 0;
            m_ctl_a = 0;
            m_irq_a1 = false;
            m_irq_a2 = false;
            m_irq_a_state = 0;
            m_out_b = 0;
            m_out_cb2 = 0;
            m_last_out_cb2_z = 0;
            m_ddr_b = 0;
            m_ctl_b = 0;
            m_irq_b1 = false;
            m_irq_b2 = false;
            m_irq_b_state = 0;

            // clear the IRQs
            m_irqa_handler.op_s32(CLEAR_LINE);
            m_irqb_handler.op_s32(CLEAR_LINE);

            // reset port A to internal pullups
            if (!m_out_a_handler.isnull())
                m_out_a_handler.op_u8(0xff);
            if (!m_ca2_handler.isnull())
                m_ca2_handler.op_s32(1);

            // reset port B to three-state outputs
            if (!m_out_b_handler.isnull() && !m_ts_b_handler.isnull())
                m_out_b_handler.op_u8((offs_t)0, m_ts_b_handler.op().u8, 0);
        }


        //void update_interrupts();

        //uint8_t get_in_a_value();
        //uint8_t get_in_b_value();

        //uint8_t get_out_a_value();
        //uint8_t get_out_b_value();

        //void set_out_ca2(int data);
        //void set_out_cb2(int data);

        //uint8_t port_a_r();
        //uint8_t ddr_a_r();
        //uint8_t control_a_r();

        //uint8_t port_b_r();
        //uint8_t ddr_b_r();
        //uint8_t control_b_r();

        //void send_to_out_a_func(const char* message);
        //void send_to_out_b_func(const char* message);

        //void port_a_w(uint8_t data);
        //void ddr_a_w(uint8_t data);

        //void port_b_w(uint8_t data);
        //void ddr_b_w(uint8_t data);

        //void control_a_w(uint8_t data);
        //void control_b_w(uint8_t data);

        //static bool irq1_enabled(uint8_t c);
        //static bool c1_low_to_high(uint8_t c);
        //static bool c1_high_to_low(uint8_t c);
        //static bool output_selected(uint8_t c);
        //static bool irq2_enabled(uint8_t c);
        //static bool strobe_e_reset(uint8_t c);
        //static bool strobe_c1_reset(uint8_t c);
        //static bool c2_set(uint8_t c);
        //static bool c2_low_to_high(uint8_t c);
        //static bool c2_high_to_low(uint8_t c);
        //static bool c2_set_mode(uint8_t c);
        //static bool c2_strobe_mode(uint8_t c);
        //static bool c2_output(uint8_t c);
        //static bool c2_input(uint8_t c);
    }


    static class _6821pia_global
    {
        public static pia6821_device PIA6821<bool_Required>(machine_config mconfig, device_finder<pia6821_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, pia6821_device.PIA6821, clock); }
    }
}
