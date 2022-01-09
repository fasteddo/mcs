// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using size_t = System.UInt64;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.netlist.nltypes_global;
using static mame.nl_factory_global;


namespace mame.netlist.devices
{
    //NETLIB_OBJECT(7448)
    class nld_7448 : device_t
    {
        //NETLIB_DEVICE_IMPL(7448, "TTL_7448", "+A,+B,+C,+D,+LTQ,+BIQ,+RBIQ,@VCC,@GND")
        public static readonly netlist.factory.constructor_ptr_t decl_7448 = NETLIB_DEVICE_IMPL<nld_7448>("TTL_7448", "+A,+B,+C,+D,+LTQ,+BIQ,+RBIQ,@VCC,@GND");


        logic_input_t m_A;
        logic_input_t m_B;
        logic_input_t m_C;
        logic_input_t m_D;
        logic_input_t m_LTQ;
        logic_input_t m_BIQ;
        logic_input_t m_RBIQ;

        state_var<unsigned> m_state;

        object_array_t_logic_output_t<u64_const_7> m_Q;  /* a .. g */
        nld_power_pins m_power_pins;


        static uint8_t BITS7(uint32_t b6, uint32_t b5, uint32_t b4, uint32_t b3, uint32_t b2, uint32_t b1, uint32_t b0) { return (uint8_t)((b6 << 6) | (b5 << 5) | (b4 << 4) | (b3 << 3) | (b2 << 2) | (b1 << 1) | (b0 << 0)); }

        static readonly std.array<uint8_t, u64_const_16> tab7448 = new std.array<uint8_t, u64_const_16>
        (
            BITS7(   1, 1, 1, 1, 1, 1, 0 ),  /* 00 - not blanked ! */
            BITS7(   0, 1, 1, 0, 0, 0, 0 ),  /* 01 */
            BITS7(   1, 1, 0, 1, 1, 0, 1 ),  /* 02 */
            BITS7(   1, 1, 1, 1, 0, 0, 1 ),  /* 03 */
            BITS7(   0, 1, 1, 0, 0, 1, 1 ),  /* 04 */
            BITS7(   1, 0, 1, 1, 0, 1, 1 ),  /* 05 */
            BITS7(   0, 0, 1, 1, 1, 1, 1 ),  /* 06 */
            BITS7(   1, 1, 1, 0, 0, 0, 0 ),  /* 07 */
            BITS7(   1, 1, 1, 1, 1, 1, 1 ),  /* 08 */
            BITS7(   1, 1, 1, 0, 0, 1, 1 ),  /* 09 */
            BITS7(   0, 0, 0, 1, 1, 0, 1 ),  /* 10 */
            BITS7(   0, 0, 1, 1, 0, 0, 1 ),  /* 11 */
            BITS7(   0, 1, 0, 0, 0, 1, 1 ),  /* 12 */
            BITS7(   1, 0, 0, 1, 0, 1, 1 ),  /* 13 */
            BITS7(   0, 0, 0, 1, 1, 1, 1 ),  /* 14 */
            BITS7(   0, 0, 0, 0, 0, 0, 0 )   /* 15 */
        );


        //NETLIB_CONSTRUCTOR(7448)
        public nld_7448(object owner, string name)
            : base(owner, name)
        {
            m_A = new logic_input_t(this, "A", inputs);
            m_B = new logic_input_t(this, "B", inputs);
            m_C = new logic_input_t(this, "C", inputs);
            m_D = new logic_input_t(this, "D", inputs);
            m_LTQ = new logic_input_t(this, "LTQ", inputs);
            m_BIQ = new logic_input_t(this, "BIQ", inputs);
            m_RBIQ = new logic_input_t(this, "RBIQ", inputs);
            m_state = new state_var<unsigned>(this, "m_state", 0);
            m_Q = new object_array_t_logic_output_t<u64_const_7>(this, new logic_output_t(this, "a"), new logic_output_t(this, "b"), new logic_output_t(this, "c"), new logic_output_t(this, "d"), new logic_output_t(this, "e"), new logic_output_t(this, "f"), new logic_output_t(this, "g"));
            m_power_pins = new nld_power_pins(this);
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            m_state.op = 0;
        }


        void update_outputs(unsigned v)
        {
            //throw new emu_unimplemented();
#if false
            gsl_Expects(v<16);
#endif

            if (v != m_state.op)
            {
                // max transfer time is 100 NS */

                uint8_t t = tab7448[v];
                for (size_t i = 0; i < 7; i++)
                    m_Q.op(i).push(((uint32_t)t >> (6 - (int)i)) & 1U, NLTIME_FROM_NS(100));

                m_state.op = v;
            }
        }


        //NETLIB_HANDLERI(inputs)
        void inputs()
        {
            if (m_BIQ.op() == 0 || (m_BIQ.op() != 0 && m_LTQ.op() == 0))
            {
                m_A.inactivate();
                m_B.inactivate();
                m_C.inactivate();
                m_D.inactivate();
                m_RBIQ.inactivate();
                if (m_BIQ.op() != 0 && m_LTQ.op() == 0)
                {
                    update_outputs(8);
                }
                else if (m_BIQ.op() == 0)
                {
                    update_outputs(15);
                }
            }
            else
            {
                m_RBIQ.activate();
                m_D.activate();
                m_C.activate();
                m_B.activate();
                m_A.activate();
                unsigned v = (m_A.op() << 0) | (m_B.op() << 1) | (m_C.op() << 2) | (m_D.op() << 3);
                if (m_RBIQ.op() == 0 && (v == 0))
                    v = 15;

                update_outputs(v);
            }
        }
    }

    //#define BITS7(b6,b5,b4,b3,b2,b1,b0) ((b6)<<6) | ((b5)<<5) | ((b4)<<4) | ((b3)<<3) | ((b2)<<2) | ((b1)<<1) | ((b0)<<0)

    //const std::array<uint8_t, 16> NETLIB_NAME(7448)::tab7448 =
    //{
    //        BITS7(   1, 1, 1, 1, 1, 1, 0 ),  /* 00 - not blanked ! */
    //        BITS7(   0, 1, 1, 0, 0, 0, 0 ),  /* 01 */
    //        BITS7(   1, 1, 0, 1, 1, 0, 1 ),  /* 02 */
    //        BITS7(   1, 1, 1, 1, 0, 0, 1 ),  /* 03 */
    //        BITS7(   0, 1, 1, 0, 0, 1, 1 ),  /* 04 */
    //        BITS7(   1, 0, 1, 1, 0, 1, 1 ),  /* 05 */
    //        BITS7(   0, 0, 1, 1, 1, 1, 1 ),  /* 06 */
    //        BITS7(   1, 1, 1, 0, 0, 0, 0 ),  /* 07 */
    //        BITS7(   1, 1, 1, 1, 1, 1, 1 ),  /* 08 */
    //        BITS7(   1, 1, 1, 0, 0, 1, 1 ),  /* 09 */
    //        BITS7(   0, 0, 0, 1, 1, 0, 1 ),  /* 10 */
    //        BITS7(   0, 0, 1, 1, 0, 0, 1 ),  /* 11 */
    //        BITS7(   0, 1, 0, 0, 0, 1, 1 ),  /* 12 */
    //        BITS7(   1, 0, 0, 1, 0, 1, 1 ),  /* 13 */
    //        BITS7(   0, 0, 0, 1, 1, 1, 1 ),  /* 14 */
    //        BITS7(   0, 0, 0, 0, 0, 0, 0 ),  /* 15 */
    //};
}
