// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using uint32_t = System.UInt32;

using static mame.device_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> midway_cheap_squeak_deluxe_device
    public class midway_cheap_squeak_deluxe_device : device_t
                                                     //device_mixer_interface
    {
        //DEFINE_DEVICE_TYPE(MIDWAY_CHEAP_SQUEAK_DELUXE, midway_cheap_squeak_deluxe_device, "midcsd", "Cheap Squeak Deluxe Sound Board")
        public static readonly emu.detail.device_type_impl MIDWAY_CHEAP_SQUEAK_DELUXE = DEFINE_DEVICE_TYPE("midcsd", "Cheap Squeak Deluxe Sound Board", (type, mconfig, tag, owner, clock) => { return new midway_cheap_squeak_deluxe_device(mconfig, tag, owner, clock); });


        // devices
        //required_device<m68000_device> m_cpu;
        //required_device<pia6821_device> m_pia;
        //required_device<dac_word_interface> m_dac;

        // internal state
        //uint8_t m_status;
        //uint16_t m_dacval;
        //emu_timer *m_pia_sync_timer;


        // construction/destruction
        midway_cheap_squeak_deluxe_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 16_000_000) :
            base(mconfig, MIDWAY_CHEAP_SQUEAK_DELUXE, tag, owner, clock)
        {
            throw new emu_unimplemented();
#if false
            device_mixer_interface(mconfig, *this),


            m_cpu(*this, "cpu"),
            m_pia(*this, "pia"),
            m_dac(*this, "dac"),
            m_status(0),
            m_dacval(0)
#endif
        }


        // helpers
        //void suspend_cpu();

        // read/write
        //u8 stat_r();
        //void sr_w(u8 data);
        //DECLARE_WRITE_LINE_MEMBER(sirq_w);
        //DECLARE_WRITE_LINE_MEMBER(reset_w);

        //void csdeluxe_map(address_map &map);


        // device-level overrides
        protected override void device_add_mconfig(machine_config config) { throw new emu_unimplemented(); }
        protected override Pointer<tiny_rom_entry> device_rom_region() { throw new emu_unimplemented(); }
        protected override void device_start() { throw new emu_unimplemented(); }
        //TIMER_CALLBACK_MEMBER(sync_pia);


        // internal communications
        //void porta_w(uint8_t data);
        //void portb_w(uint8_t data);
        //DECLARE_WRITE_LINE_MEMBER(irq_w);
    }
}
