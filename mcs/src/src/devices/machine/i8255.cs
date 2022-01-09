// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using offs_t = System.UInt32;  //using offs_t = u32;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.emucore_global;
using static mame.util;


namespace mame
{
    // ======================> i8255_device
    public class i8255_device : device_t
    {
        //DEFINE_DEVICE_TYPE(I8255, i8255_device, "i8255", "Intel 8255 PPI")
        static device_t device_creator_i8255_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new i8255_device(mconfig, tag, owner, clock); }
        public static readonly device_type I8255 = DEFINE_DEVICE_TYPE(device_creator_i8255_device, "i8255", "Intel 8255 PPI");

        public static readonly device_type I8255A = I8255;  //decltype(I8255) I8255A = I8255;


        const int VERBOSE = 0;  //#define VERBOSE 1
        protected void LOG(string format, params object [] args) { LOG(VERBOSE, format, args); }


        //enum
        //{
        const int PORT_A  = 0;
        const int PORT_B  = 1;
        const int PORT_C  = 2;
        const int CONTROL = 3;
        //}


        //enum
        //{
        const int GROUP_A = 0;
        const int GROUP_B = 1;
        //}


        //enum
        //{
        const int MODE_0 = 0;
        const int MODE_1 = 1;
        const int MODE_2 = 2;
        //}


        //enum
        //{
        const int MODE_OUTPUT = 0;
        const int MODE_INPUT  = 1;
        //}


        const uint8_t CONTROL_PORT_C_LOWER_INPUT  = 0x01;
        const uint8_t CONTROL_PORT_B_INPUT        = 0x02;
        const uint8_t CONTROL_GROUP_B_MODE_1      = 0x04;
        const uint8_t CONTROL_PORT_C_UPPER_INPUT  = 0x08;
        const uint8_t CONTROL_PORT_A_INPUT        = 0x10;
        const uint8_t CONTROL_GROUP_A_MODE_MASK   = 0x60;
        const uint8_t CONTROL_MODE_SET            = 0x80;


        bool m_force_portb_in;
        bool m_force_portc_out;
        bool m_dont_clear_output_latches;


        devcb_read8 m_in_pa_cb;
        devcb_read8 m_in_pb_cb;
        devcb_read8 m_in_pc_cb;

        devcb_write8 m_out_pa_cb;
        devcb_write8 m_out_pb_cb;
        devcb_write8 m_out_pc_cb;

        devcb_read8 m_tri_pa_cb;
        devcb_read8 m_tri_pb_cb;
        devcb_read8 m_tri_pc_cb;

        uint8_t m_control;            // mode control word
        uint8_t [] m_output = new uint8_t[3];          // output latch
        uint8_t [] m_input = new uint8_t[3];           // input latch

        int [] m_ibf = new int[2];               // input buffer full flag
        int [] m_obf = new int[2];               // output buffer full flag, negative logic
        int [] m_inte = new int[2];              // interrupt enable
        int m_inte1;                // interrupt enable
        int m_inte2;                // interrupt enable
        int [] m_intr = new int[2];              // interrupt


        // construction/destruction
        //-------------------------------------------------
        //  i8255_device - constructor
        //-------------------------------------------------
        i8255_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : this(mconfig, I8255, tag, owner, clock, false)
        {
        }

        i8255_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, bool is_ams40489)
            : base(mconfig, type, tag, owner, clock)
        {
            m_force_portb_in = is_ams40489;
            m_force_portc_out = is_ams40489;
            m_dont_clear_output_latches = is_ams40489;
            m_in_pa_cb = new devcb_read8(this);
            m_in_pb_cb = new devcb_read8(this);
            m_in_pc_cb = new devcb_read8(this);
            m_out_pa_cb = new devcb_write8(this);
            m_out_pb_cb = new devcb_write8(this);
            m_out_pc_cb = new devcb_write8(this);
            m_tri_pa_cb = new devcb_read8(this);
            m_tri_pb_cb = new devcb_read8(this);
            m_tri_pc_cb = new devcb_read8(this);
            m_control = 0;
            m_intr = new int[2] { 0, 0 };
        }


        public devcb_read8.binder in_pa_callback() { return m_in_pa_cb.bind(); }
        public devcb_read8.binder in_pb_callback() { return m_in_pb_cb.bind(); }
        public devcb_read8.binder in_pc_callback() { return m_in_pc_cb.bind(); }
        public devcb_write8.binder out_pa_callback() { return m_out_pa_cb.bind(); }
        public devcb_write8.binder out_pb_callback() { return m_out_pb_cb.bind(); }
        public devcb_write8.binder out_pc_callback() { return m_out_pc_cb.bind(); }

        // output state when pins are in tri-state, default 0xff
        //auto tri_pa_callback() { return m_tri_pa_cb.bind(); }
        //auto tri_pb_callback() { return m_tri_pb_cb.bind(); }
        //auto tri_pc_callback() { return m_tri_pc_cb.bind(); }


        //-------------------------------------------------
        //  read -
        //-------------------------------------------------
        public uint8_t read(offs_t offset)
        {
            uint8_t data = 0;

            switch (offset & 0x03)
            {
            case PORT_A:
                switch (group_mode(GROUP_A))
                {
                case MODE_0: data = read_mode0(PORT_A); break;
                case MODE_1: data = read_mode1(PORT_A); break;
                case MODE_2: data = read_mode2(); break;
                }
                LOG("I8255 Port A Read: {0}\n", data);  // %02x
                break;

            case PORT_B:
                switch (group_mode(GROUP_B))
                {
                case MODE_0: data = read_mode0(PORT_B); break;
                case MODE_1: data = read_mode1(PORT_B); break;
                }
                LOG("I8255 Port B Read: {0}\n", data);
                break;

            case PORT_C:
                data = read_pc();
                LOG("I8255 Port C Read: {0}\n", data);
                break;

            case CONTROL:
                data = m_control;
                LOG("I8255 Mode Control Word Read: {0}\n", data);
                break;
            }

            return data;
        }


        //-------------------------------------------------
        //  write -
        //-------------------------------------------------
        public void write(offs_t offset, uint8_t data)
        {
            switch (offset & 0x03)
            {
            case PORT_A:
                LOG("I8255 Port A Write: {0}\n", data);  // %02x

                switch (group_mode(GROUP_A))
                {
                case MODE_0: write_mode0(PORT_A, data); break;
                case MODE_1: write_mode1(PORT_A, data); break;
                case MODE_2: write_mode2(data); break;
                }
                break;

            case PORT_B:
                LOG("I8255 Port B Write: {0}\n", data);

                switch (group_mode(GROUP_B))
                {
                case MODE_0: write_mode0(PORT_B, data); break;
                case MODE_1: write_mode1(PORT_B, data); break;
                }
                break;

            case PORT_C:
                LOG("I8255 Port C Write: {0}\n", data);

                m_output[PORT_C] = data;
                output_pc();
                break;

            case CONTROL:
                if ((data & CONTROL_MODE_SET) != 0)
                {
                    LOG("I8255 Mode Control Word: {0}\n", data);

                    set_mode(data);
                }
                else
                {
                    int bit = (data >> 1) & 0x07;
                    int state = BIT(data, 0);

                    LOG("I8255 {0} Port C Bit {1}\n", state != 0 ? "Set" : "Reset", bit);  // %s %u

                    set_pc_bit(bit, state);
                }
                break;
            }
        }


        //uint8_t pa_r();
        //uint8_t acka_r();

        //uint8_t pb_r();
        //uint8_t ackb_r();

        //DECLARE_WRITE_LINE_MEMBER( pc2_w );
        //DECLARE_WRITE_LINE_MEMBER( pc4_w );
        //DECLARE_WRITE_LINE_MEMBER( pc6_w );


        // device-level overrides

        protected override void device_resolve_objects()
        {
            // resolve callbacks
            m_in_pa_cb.resolve_safe_u8(0);
            m_in_pb_cb.resolve_safe_u8(0);
            m_in_pc_cb.resolve_safe_u8(0);
            m_out_pa_cb.resolve_safe();
            m_out_pb_cb.resolve_safe();
            m_out_pc_cb.resolve_safe();
            m_tri_pa_cb.resolve_safe_u8(0xff);
            m_tri_pb_cb.resolve_safe_u8(0xff);
            m_tri_pc_cb.resolve_safe_u8(0xff);
        }


        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // register for state saving
            save_item(NAME(new { m_control }));
            save_item(NAME(new { m_output }));
            save_item(NAME(new { m_input }));
            save_item(NAME(new { m_ibf }));
            save_item(NAME(new { m_obf }));
            save_item(NAME(new { m_inte }));
            save_item(NAME(new { m_inte1 }));
            save_item(NAME(new { m_inte2 }));
            save_item(NAME(new { m_intr }));
        }


        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            set_mode(0x9b);
        }


        //-------------------------------------------------
        //  check_interrupt -
        //-------------------------------------------------
        void check_interrupt(int port)
        {
            switch (group_mode(port))
            {
            case MODE_1:
                switch (port_mode(port))
                {
                case MODE_INPUT:
                    set_intr(port, (m_inte[port] != 0 && m_ibf[port] != 0) ? 1 : 0);
                    break;

                case MODE_OUTPUT:
                    set_intr(port, (m_inte[port] != 0 && m_obf[port] != 0) ? 1 : 0);
                    break;
                }
                break;

            case MODE_2:
                set_intr(port, ((m_inte1 != 0 && m_obf[port] != 0) || (m_inte2 != 0 && m_ibf[port] != 0)) ? 1 : 0);
                break;
            }
        }


        //-------------------------------------------------
        //  set_ibf -
        //-------------------------------------------------
        void set_ibf(int port, int state)
        {
            LOG("I8255 Port {0} IBF: {1}\n", 'A' + port, state);  // %c IBF: %u\n

            m_ibf[port] = state;

            check_interrupt(port);
        }


        //-------------------------------------------------
        //  set_obf -
        //-------------------------------------------------
        void set_obf(int port, int state)
        {
            LOG("I8255 Port {0} OBF: {1}\n", 'A' + port, state);  // %c OBF: %u\n

            m_obf[port] = state;

            check_interrupt(port);
        }


        //-------------------------------------------------
        //  set_inte -
        //-------------------------------------------------
        void set_inte(int port, int state)
        {
            LOG("I8255 Port {0} INTE: {1}\n", 'A' + port, state);

            m_inte[port] = state;

            check_interrupt(port);
        }


        //-------------------------------------------------
        //  set_inte1 -
        //-------------------------------------------------
        void set_inte1(int state)
        {
            LOG("I8255 Port A INTE1: {0}\n", state);

            m_inte1 = state;

            check_interrupt(PORT_A);
        }


        //-------------------------------------------------
        //  set_inte2 -
        //-------------------------------------------------
        void set_inte2(int state)
        {
            LOG("I8255 Port A INTE2: {0}\n", state);

            m_inte2 = state;

            check_interrupt(PORT_A);
        }


        //-------------------------------------------------
        //  set_intr -
        //-------------------------------------------------

        void set_intr(int port, int state)
        {
            LOG("I8255 Port {0} INTR: {1}\n", 'A' + port, state);

            m_intr[port] = state;

            output_pc();
        }


        //-------------------------------------------------
        //  group_mode -
        //-------------------------------------------------
        int group_mode(int group)
        {
            int mode = 0;

            switch (group)
            {
            case GROUP_A:
                switch ((m_control & CONTROL_GROUP_A_MODE_MASK) >> 5)
                {
                case 0: mode = MODE_0; break;
                case 1: mode = MODE_1; break;
                case 2: case 3: mode = MODE_2; break;
                }
                break;

            case GROUP_B:
                mode = (m_control & CONTROL_GROUP_B_MODE_1) != 0 ? MODE_1 : MODE_0;
                break;
            }

            return mode;
        }


        //-------------------------------------------------
        //  port_mode -
        //-------------------------------------------------
        int port_mode(int port)
        {
            int mode = 0;

            switch (port)
            {
            case PORT_A: mode = (m_control & CONTROL_PORT_A_INPUT) != 0 ? MODE_INPUT : MODE_OUTPUT; break;
            case PORT_B: mode = (m_control & CONTROL_PORT_B_INPUT) != 0 ? MODE_INPUT : MODE_OUTPUT; break;
            }

            return mode;
        }


        //-------------------------------------------------
        //  port_c_lower_mode -
        //-------------------------------------------------
        int port_c_lower_mode()
        {
            return (m_control & CONTROL_PORT_C_LOWER_INPUT) != 0 ? MODE_INPUT : MODE_OUTPUT;
        }


        //-------------------------------------------------
        //  port_c_upper_mode -
        //-------------------------------------------------
        int port_c_upper_mode()
        {
            return (m_control & CONTROL_PORT_C_UPPER_INPUT) != 0 ? MODE_INPUT : MODE_OUTPUT;
        }



        //-------------------------------------------------
        //  read_mode0 -
        //-------------------------------------------------
        uint8_t read_mode0(int port)
        {
            uint8_t data;

            if (port_mode(port) == MODE_OUTPUT)
            {
                // read data from output latch
                data = m_output[port];
            }
            else
            {
                // read data from port
                data = (port == PORT_A) ? m_in_pa_cb.op_u8(0) : ((port == PORT_B) ? m_in_pb_cb.op_u8(0) : m_in_pc_cb.op_u8(0));
            }

            return data;
        }


        //-------------------------------------------------
        //  read_mode1 -
        //-------------------------------------------------
        uint8_t read_mode1(int port)
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  read_mode2 -
        //-------------------------------------------------
        uint8_t read_mode2()
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  read_pc -
        //-------------------------------------------------
        uint8_t read_pc()
        {
            uint8_t data = 0;
            uint8_t mask = 0;
            uint8_t b_mask = 0x0f;

            // PC upper
            switch (group_mode(GROUP_A))
            {
            case MODE_0:
                if (port_c_upper_mode() == MODE_OUTPUT)
                {
                    // read data from output latch
                    data |= (uint8_t)(m_output[PORT_C] & 0xf0);
                }
                else
                {
                    // read data from port
                    mask |= 0xf0;
                }
                break;

            case MODE_1:
                data |= m_intr[PORT_A] != 0 ? (uint8_t)0x08 : (uint8_t)0x00;

                if (port_mode(PORT_A) == MODE_OUTPUT)
                {
                    data |= m_obf[PORT_A] != 0 ? (uint8_t)0x80 : (uint8_t)0x00;
                    data |= m_inte[PORT_A] != 0 ? (uint8_t)0x40 : (uint8_t)0x00;
                    mask |= 0x30;
                }
                else
                {
                    data |= m_ibf[PORT_A] != 0 ? (uint8_t)0x20 : (uint8_t)0x00;
                    data |= m_inte[PORT_A] != 0 ? (uint8_t)0x10 : (uint8_t)0x00;
                    mask |= 0xc0;
                }
                break;

            case MODE_2:
                b_mask = 0x07;
                data |= m_intr[PORT_A] != 0 ? (uint8_t)0x08 : (uint8_t)0x00;
                data |= m_inte2 != 0 ? (uint8_t)0x10 : (uint8_t)0x00;
                data |= m_ibf[PORT_A] != 0 ? (uint8_t)0x20 : (uint8_t)0x00;
                data |= m_inte1 != 0 ? (uint8_t)0x40 : (uint8_t)0x00;
                data |= m_obf[PORT_A] != 0 ? (uint8_t)0x80 : (uint8_t)0x00;
                break;
            }

            // PC lower
            switch (group_mode(GROUP_B))
            {
            case MODE_0:
                if (port_c_lower_mode() == MODE_OUTPUT)
                {
                    // read data from output latch
                    data |= (uint8_t)(m_output[PORT_C] & b_mask);
                }
                else
                {
                    // read data from port
                    mask |= b_mask;
                }
                break;

            case MODE_1:
                data |= m_inte[PORT_B] != 0 ? (uint8_t)0x04 : (uint8_t)0x00;
                data |= m_intr[PORT_B] != 0 ? (uint8_t)0x01 : (uint8_t)0x00;

                if (port_mode(PORT_B) == MODE_OUTPUT)
                {
                    data |= m_obf[PORT_B] != 0 ? (uint8_t)0x02 : (uint8_t)0x00;
                }
                else
                {
                    data |= m_ibf[PORT_B] != 0 ? (uint8_t)0x02 : (uint8_t)0x00;
                }
                break;
            }

            if (mask != 0)
            {
                // read data from port
                data |= (uint8_t)(m_in_pc_cb.op_u8(0) & mask);
            }

            return data;
        }


        //-------------------------------------------------
        //  write_mode0 -
        //-------------------------------------------------
        void write_mode0(int port, uint8_t data)
        {
            if (port_mode(port) == MODE_OUTPUT)
            {
                // latch output data
                m_output[port] = data;

                // write data to port
                if (port == PORT_A)
                    m_out_pa_cb.op_u8((offs_t)0, m_output[port]);
                else if (port == PORT_B)
                    m_out_pb_cb.op_u8((offs_t)0, m_output[port]);
                else
                    m_out_pc_cb.op_u8((offs_t)0, m_output[port]);
            }
        }


        //-------------------------------------------------
        //  write_mode1 -
        //-------------------------------------------------
        void write_mode1(int port, uint8_t data)
        {
            if (port_mode(port) == MODE_OUTPUT)
            {
                // latch output data
                m_output[port] = data;

                // write data to port
                if (port == PORT_A)
                    m_out_pa_cb.op_u8((offs_t)0, m_output[port]);
                else if (port == PORT_B)
                    m_out_pb_cb.op_u8((offs_t)0, m_output[port]);
                else
                    m_out_pc_cb.op_u8((offs_t)0, m_output[port]);

                // set output buffer full flag
                set_obf(port, 0);

                // clear interrupt
                set_intr(port, 0);
            }
        }


        //-------------------------------------------------
        //  write_mode2 -
        //-------------------------------------------------
        void write_mode2(uint8_t data)
        {
            // latch output data
            m_output[PORT_A] = data;

            // write data to port
            m_out_pa_cb.op_u8((offs_t)0, data);

            // set output buffer full flag
            set_obf(PORT_A, 0);

            // clear interrupt
            set_intr(PORT_A, 0);
        }


        //-------------------------------------------------
        //  output_pc -
        //-------------------------------------------------
        void output_pc()
        {
            uint8_t data = 0;
            uint8_t mask = 0;
            uint8_t b_mask = 0x0f;

            // PC upper
            switch (group_mode(GROUP_A))
            {
            case MODE_0:
                if (port_c_upper_mode() == MODE_OUTPUT)
                {
                    mask |= 0xf0;
                }
                else
                {
                    // TTL inputs floating
                    data |= (uint8_t)(m_tri_pc_cb.op_u8(0) & (u32)0xf0);
                }
                break;

            case MODE_1:
                data |= m_intr[PORT_A] != 0 ? (uint8_t)0x08 : (uint8_t)0x00;

                if (port_mode(PORT_A) == MODE_OUTPUT)
                {
                    data |= m_obf[PORT_A] != 0 ? (uint8_t)0x80 : (uint8_t)0x00;
                    mask |= 0x30;
                }
                else
                {
                    data |= m_ibf[PORT_A] != 0 ? (uint8_t)0x20 : (uint8_t)0x00;
                    mask |= 0xc0;
                }
                break;

            case MODE_2:
                b_mask = 0x07;
                data |= m_intr[PORT_A] != 0 ? (uint8_t)0x08 : (uint8_t)0x00;
                data |= m_ibf[PORT_A] != 0 ? (uint8_t)0x20 : (uint8_t)0x00;
                data |= m_obf[PORT_A] != 0 ? (uint8_t)0x80 : (uint8_t)0x00;
                break;
            }

            // PC lower
            switch (group_mode(GROUP_B))
            {
            case MODE_0:
                if (port_c_lower_mode() == MODE_OUTPUT)
                {
                    mask |= b_mask;
                }
                else
                {
                    // TTL inputs floating
                    data |= (uint8_t)(m_tri_pc_cb.op_u8(0) & (u32)b_mask);
                }
                break;

            case MODE_1:
                data |= m_intr[PORT_B] != 0 ? (uint8_t)0x01 : (uint8_t)0x00;

                if (port_mode(PORT_B) == MODE_OUTPUT)
                {
                    data |= m_obf[PORT_B] != 0 ? (uint8_t)0x02 : (uint8_t)0x00;
                }
                else
                {
                    data |= m_ibf[PORT_B] != 0 ? (uint8_t)0x02 : (uint8_t)0x00;
                }
                break;
            }

            data |= (uint8_t)(m_output[PORT_C] & mask);

            m_out_pc_cb.op_u8(0, data);
        }


        void set_mode(uint8_t data)
        {
            m_control = data;

            if (m_force_portb_in)
                m_control = (uint8_t)(m_control | CONTROL_PORT_B_INPUT);

            if (m_force_portc_out)
            {
                m_control = (uint8_t)(m_control & ~CONTROL_PORT_C_UPPER_INPUT);
                m_control = (uint8_t)(m_control & ~CONTROL_PORT_C_LOWER_INPUT);
            }

            // group A
            if (!m_dont_clear_output_latches)
                m_output[PORT_A] = 0;
            m_input[PORT_A] = 0;
            m_ibf[PORT_A] = 0;
            m_obf[PORT_A] = 1;
            m_inte[PORT_A] = 0;
            m_inte1 = 0;
            m_inte2 = 0;

            if (port_mode(PORT_A) == MODE_OUTPUT)
            {
                m_out_pa_cb.op_u8((offs_t)0, m_output[PORT_A]);
                m_ibf[PORT_A] = 1; // correct? needed by SAM Coupe Blue Alpha sampler
            }
            else
            {
                // TTL inputs floating
                m_out_pa_cb.op_u8((offs_t)0, m_tri_pa_cb.op_u8(0));
            }

            LOG("I8255 Group A Mode: {0}\n", group_mode(GROUP_A));  // %u
            LOG("I8255 Port A Mode: {0}\n", (port_mode(PORT_A) == MODE_OUTPUT) ? "output" : "input");
            LOG("I8255 Port C Upper Mode: {0}\n", (port_c_upper_mode() == MODE_OUTPUT) ? "output" : "input");
            LOG("I8255 Group B Mode: {0}\n", group_mode(GROUP_B));  // %u
            LOG("I8255 Port B Mode: {0}\n", (port_mode(PORT_B) == MODE_OUTPUT) ? "output" : "input");
            LOG("I8255 Port C Lower Mode: {0}\n", (port_c_lower_mode() == MODE_OUTPUT) ? "output" : "input");

            // group B
            if (!m_dont_clear_output_latches)
                m_output[PORT_B] = 0;
            m_input[PORT_B] = 0;
            m_ibf[PORT_B] = 0;
            m_obf[PORT_B] = 1;
            m_inte[PORT_B] = 0;

            if (port_mode(PORT_B) == MODE_OUTPUT)
            {
                m_out_pb_cb.op_u8((offs_t)0, m_output[PORT_B]);
            }
            else
            {
                // TTL inputs floating
                m_out_pb_cb.op_u8((offs_t)0, m_tri_pb_cb.op_u8(0));
            }

            if (!m_dont_clear_output_latches)
                m_output[PORT_C] = 0;
            m_input[PORT_C] = 0;

            output_pc();
        }


        //-------------------------------------------------
        //  set_pc_bit -
        //-------------------------------------------------
        void set_pc_bit(int bit, int state)
        {
            // set output latch bit
            m_output[PORT_C] &= (uint8_t)(~(1 << bit));
            m_output[PORT_C] |= (uint8_t)(state << bit);

            switch (group_mode(GROUP_A))
            {
            case MODE_1:
                if (port_mode(PORT_A) == MODE_OUTPUT)
                {
                    switch (bit)
                    {
                    case 3: set_intr(PORT_A, state); break;
                    case 6: set_inte(PORT_A, state); break;
                    case 7: set_obf(PORT_A, state); break;
                    default: break;
                    }
                }
                else
                {
                    switch (bit)
                    {
                    case 3: set_intr(PORT_A, state); break;
                    case 4: set_inte(PORT_A, state); break;
                    case 5: set_ibf(PORT_A, state); break;
                    default: break;
                    }
                }
                break;

            case MODE_2:
                switch (bit)
                {
                case 3: set_intr(PORT_A, state); break;
                case 4: set_inte2(state); break;
                case 5: set_ibf(PORT_A, state); break;
                case 6: set_inte1(state); break;
                case 7: set_obf(PORT_A, state); break;
                default: break;
                }
                break;
            }

            if (group_mode(GROUP_B) == MODE_1)
            {
                switch (bit)
                {
                case 0: set_intr(PORT_B, state); break;
                case 1:
                    if (port_mode(PORT_B) == MODE_OUTPUT)
                        set_obf(PORT_B, state);
                    else
                        set_ibf(PORT_B, state);
                    break;
                case 2: set_inte(PORT_B, state); break;
                default: break;
                }
            }

            output_pc();
        }
    }
}
