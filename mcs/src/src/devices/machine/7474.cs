// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;

using static mame.device_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> ttl7474_device
    public class ttl7474_device : device_t
    {
        //DEFINE_DEVICE_TYPE(TTL7474, ttl7474_device, "7474", "7474 TTL")
        public static readonly emu.detail.device_type_impl TTL7474 = DEFINE_DEVICE_TYPE("7474", "7474 TTL", (type, mconfig, tag, owner, clock) => { return new ttl7474_device(mconfig, tag, owner, clock); });


        // callbacks
        //devcb_write_line m_output_func;
        //devcb_write_line m_comp_output_func;

        // inputs
        //uint8_t m_clear;              // pin 1/13
        //uint8_t m_preset;             // pin 4/10
        //uint8_t m_clk;                // pin 3/11
        //uint8_t m_d;                  // pin 2/12

        // outputs
        //uint8_t m_output;             // pin 5/9
        //uint8_t m_output_comp;        // pin 6/8

        // internal
        //uint8_t m_last_clock;
        //uint8_t m_last_output;
        //uint8_t m_last_output_comp;


        // construction/destruction
        ttl7474_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, TTL7474, tag, owner, clock)
        {
            throw new emu_unimplemented();
#if false
            m_output_func(*this),
            m_comp_output_func(*this)


            init();
#endif
        }


        // static configuration helpers
        //auto output_cb() { return m_output_func.bind(); }
        //auto comp_output_cb() { return m_comp_output_func.bind(); }

        // public interfaces
        //DECLARE_WRITE_LINE_MEMBER( clear_w );
        //DECLARE_WRITE_LINE_MEMBER( preset_w );
        //DECLARE_WRITE_LINE_MEMBER( clock_w );
        //DECLARE_WRITE_LINE_MEMBER( d_w );
        //DECLARE_READ_LINE_MEMBER( output_r );
        //DECLARE_READ_LINE_MEMBER( output_comp_r );    // NOT strictly the same as !output_r()


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_reset() { throw new emu_unimplemented(); }
        protected override void device_post_load() { }
        protected override void device_clock_changed() { }


        //void update();
        //void init();
    }
}
