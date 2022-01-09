// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.util;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************
    public class ttl153_device : device_t
    {
        //DEFINE_DEVICE_TYPE(TTL153, ttl153_device, "ttl153", "SN54/74153")
        static device_t device_creator_ttl153_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new ttl153_device(mconfig, tag, owner, clock); }
        public static readonly device_type TTL153 = DEFINE_DEVICE_TYPE(device_creator_ttl153_device, "ttl153", "SN54/74153");


        // callbacks
        devcb_write_line m_za_cb;
        devcb_write_line m_zb_cb;

        // state
        bool [] m_s = new bool[2];
        bool [] m_ia = new bool[4];
        bool [] m_ib = new bool[4];
        bool [] m_z = new bool[2];


        // construction/destruction
        ttl153_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, TTL153, tag, owner, clock)
        {
            m_za_cb = new devcb_write_line(this);
            m_zb_cb = new devcb_write_line(this);
            m_s = new bool [] { false, false };
            m_ia = new bool [] { false, false, false, false };
            m_ib = new bool [] { false, false, false, false };
            m_z = new bool[] { false, false };
        }


        // configuration
        //auto za_cb() { return m_za_cb.bind(); }
        //auto zb_cb() { return m_zb_cb.bind(); }

        // select
        //DECLARE_WRITE_LINE_MEMBER(s0_w);
        //DECLARE_WRITE_LINE_MEMBER(s1_w);


        public void s_w(uint8_t data)
        {
            m_s[0] = BIT(data, 0) != 0;
            m_s[1] = BIT(data, 1) != 0;
            update_a();
            update_b();
        }


        // input a

        //WRITE_LINE_MEMBER( ttl153_device::i0a_w )
        public void i0a_w(int state)
        {
            m_ia[0] = state != 0;
            update_a();
        }


        //WRITE_LINE_MEMBER( ttl153_device::i1a_w )
        public void i1a_w(int state)
        {
            m_ia[1] = state != 0;
            update_a();
        }


        //WRITE_LINE_MEMBER( ttl153_device::i2a_w )
        public void i2a_w(int state)
        {
            m_ia[2] = state != 0;
            update_a();
        }


        //WRITE_LINE_MEMBER( ttl153_device::i3a_w )
        public void i3a_w(int state)
        {
            m_ia[3] = state != 0;
            update_a();
        }


        //void ia_w(uint8_t data);

        // input b

        //WRITE_LINE_MEMBER( ttl153_device::i0b_w )
        public void i0b_w(int state)
        {
            m_ib[0] = state != 0;
            update_b();
        }


        //WRITE_LINE_MEMBER( ttl153_device::i1b_w )
        public void i1b_w(int state)
        {
            m_ib[1] = state != 0;
            update_b();
        }


        //WRITE_LINE_MEMBER( ttl153_device::i2b_w )
        public void i2b_w(int state)
        {
            m_ib[2] = state != 0;
            update_b();
        }


        //WRITE_LINE_MEMBER( ttl153_device::i3b_w )
        public void i3b_w(int state)
        {
            m_ib[3] = state != 0;
            update_b();
        }


        //void ib_w(uint8_t data);


        // output

        //READ_LINE_MEMBER( ttl153_device::za_r )
        public int za_r()
        {
            return m_z[0] ? 1 : 0;
        }


        //READ_LINE_MEMBER( ttl153_device::zb_r )
        public int zb_r()
        {
            return m_z[1] ? 1 : 0;
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // resolve callbacks
            m_za_cb.resolve_safe();
            m_zb_cb.resolve_safe();


            //throw new emu_unimplemented();
#if false
            // register for save states
            save_pointer(NAME(m_s), 2);
            save_pointer(NAME(m_ia), 4);
            save_pointer(NAME(m_ib), 4);
            save_pointer(NAME(m_z), 2);
#endif
        }


        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            std.fill(m_s, false);
            std.fill(m_ia, false);
            std.fill(m_ib, false);
            std.fill(m_z, false);
        }


        //-------------------------------------------------
        //  update_a - update output a state
        //-------------------------------------------------
        void update_a()
        {
            // calculate state
            bool za = false;
            za |= m_ia[0] && !m_s[1] && !m_s[0];
            za |= m_ia[1] && !m_s[1] &&  m_s[0];
            za |= m_ia[2] &&  m_s[1] && !m_s[0];
            za |= m_ia[3] &&  m_s[1] &&  m_s[0];

            // output
            if (za != m_z[0])
                m_za_cb.op_s32(za ? 1 : 0);

            m_z[0] = za;
        }


        //-------------------------------------------------
        //  update_b - update output b state
        //-------------------------------------------------
        void update_b()
        {
            // calculate state
            bool zb = false;
            zb |= m_ib[0] && !m_s[1] && !m_s[0];
            zb |= m_ib[1] && !m_s[1] &&  m_s[0];
            zb |= m_ib[2] &&  m_s[1] && !m_s[0];
            zb |= m_ib[3] &&  m_s[1] &&  m_s[0];

            // output
            if (zb != m_z[1])
                m_zb_cb.op_s32(zb ? 1 : 0);

            m_z[1] = zb;
        }
    }
}
