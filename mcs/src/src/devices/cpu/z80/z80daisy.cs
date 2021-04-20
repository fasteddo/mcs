// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    // ======================> z80_daisy_config
    public class z80_daisy_config
    {
        public string devname;                    // name of the device
    }


    // ======================> device_z80daisy_interface
    public abstract class device_z80daisy_interface : device_interface
    {
        //friend class z80_daisy_chain_interface;


        public device_z80daisy_interface m_daisy_next;    // next device in the chain


        // construction/destruction
        //-------------------------------------------------
        //  device_z80daisy_interface - constructor
        //-------------------------------------------------
        device_z80daisy_interface(machine_config mconfig, device_t device)
            : base(device, "z80daisy")
        {
            m_daisy_next = null;
        }


        // required operation overrides
        public abstract int z80daisy_irq_state();
        public abstract int z80daisy_irq_ack();
        public abstract void z80daisy_irq_reti();
    }


    // ======================> z80_daisy_chain_interface
    public class z80_daisy_chain_interface : device_interface
    {
        const bool VERBOSE = false;


        // these constants are returned from the irq_state function
        const byte Z80_DAISY_INT = 0x01;       // interrupt request mask
        const byte Z80_DAISY_IEO = 0x02;       // interrupt disable mask (IEO)


        z80_daisy_config m_daisy_config;
        device_z80daisy_interface m_chain;     // head of the daisy chain


        // construction/destruction
        //-------------------------------------------------
        //  z80_daisy_chain_interface - constructor
        //-------------------------------------------------
        public z80_daisy_chain_interface(machine_config mconfig, device_t device)
            : base (device, "z80daisychain")
        {
            m_daisy_config = null;
            m_chain = null;
        }


        // configuration helpers
        //void set_daisy_config(const z80_daisy_config *config) { m_daisy_config = config; }


        // getters
        public bool daisy_chain_present()
        {
            //throw new emu_unimplemented();
            return false;
        }

        //std::string daisy_show_chain() const;


        // interface-level overrides
        public override void interface_post_start()
        {
            //throw new emu_unimplemented();
        }

        public override void interface_post_reset()
        {
            //throw new emu_unimplemented();
        }


        // initialization
        //void daisy_init(const z80_daisy_config *daisy);


        // callbacks

        public int daisy_update_irq_state() { throw new emu_unimplemented(); }


        //-------------------------------------------------
        //  daisy_get_irq_device - return the device
        //  in the chain that requested the interrupt
        //-------------------------------------------------
        public device_z80daisy_interface daisy_get_irq_device()
        {
            // loop over all devices; dev[0] is the highest priority
            for (device_z80daisy_interface intf = m_chain; intf != null; intf = intf.m_daisy_next)
            {
                // if this device is asserting the INT line, that's the one we want
                int state = intf.z80daisy_irq_state();
                if ((state & Z80_DAISY_INT) != 0)
                    return intf;
            }

            if (VERBOSE && daisy_chain_present())
                device().logerror("Interrupt from outside Z80 daisy chain\n");

            return null;
        }


        public void daisy_call_reti_device() { throw new emu_unimplemented(); }
    }
}
