// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame._74181_global;
using static mame.device_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> ttl74181_device
    public class ttl74181_device : device_t
    {
        //DEFINE_DEVICE_TYPE(TTL74181, ttl74181_device, "ttl74181", "74181 TTL")
        public static readonly emu.detail.device_type_impl TTL74181 = DEFINE_DEVICE_TYPE("ttl74181", "74181 TTL", (type, mconfig, tag, owner, clock) => { return new ttl74181_device(mconfig, tag, owner, clock); });


        // inputs
        uint8_t m_a;
        uint8_t m_b;
        uint8_t m_s;
        int m_m;
        int m_c;

        // outputs
        uint8_t m_f;
        int m_cn;
        int m_g;
        int m_p;
        int m_equals;


        // construction/destruction
        ttl74181_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0) :
            base(mconfig, TTL74181, tag, owner, clock)
        {
            m_a = 0;
            m_b = 0;
            m_s = 0;
            m_m = 0;
            m_c = 0;
            m_f = 0;
            m_cn = 0;
            m_g = 0;
            m_p = 0;
            m_equals = 0;
        }


        // inputs
        //void input_a_w(uint8_t data);
        //void input_b_w(uint8_t data);
        //void select_w(uint8_t data);
        //DECLARE_WRITE_LINE_MEMBER( mode_w );
        //DECLARE_WRITE_LINE_MEMBER( carry_w );

        // outputs
        //uint8_t function_r() { return m_f; }
        //DECLARE_READ_LINE_MEMBER( carry_r ) { return m_cn; }
        //DECLARE_READ_LINE_MEMBER( generate_r ) { return m_g; }
        //DECLARE_READ_LINE_MEMBER( propagate_r ) { return m_p; }
        //DECLARE_READ_LINE_MEMBER( equals_r ) { return m_equals; }


        // device-level overrides
        protected override void device_start()
        {
            throw new emu_unimplemented();
        }

        protected override void device_post_load()
        {
            throw new emu_unimplemented();
        }


        //void update();
    }


    public static class _74181_global
    {
        public static ttl74181_device TTL74181<bool_Required>(machine_config mconfig, device_finder<ttl74181_device, bool_Required> finder) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, ttl74181_device.TTL74181, 0); }
    }
}
