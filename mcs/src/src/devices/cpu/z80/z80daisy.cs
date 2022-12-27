// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint8_t = System.Byte;

using static mame.cpp_global;
using static mame.emucore_global;
using static mame.osdcore_global;
using static mame.z80daisy_global;


namespace mame
{
    static class z80daisy_global
    {
        //**************************************************************************
        //  CONSTANTS
        //**************************************************************************

        // these constants are returned from the irq_state function
        public const uint8_t Z80_DAISY_INT = 0x01;       // interrupt request mask
        public const uint8_t Z80_DAISY_IEO = 0x02;       // interrupt disable mask (IEO)
    }


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
        uint8_t m_last_opcode;


        // construction/destruction
        //-------------------------------------------------
        //  device_z80daisy_interface - constructor
        //-------------------------------------------------
        protected device_z80daisy_interface(machine_config mconfig, device_t device)
            : base(device, "z80daisy")
        {
            m_daisy_next = null;
            m_last_opcode = 0;
        }


        public override void interface_post_start()
        {
            device().save_item(NAME(new { m_last_opcode }));
        }


        public override void interface_post_reset()
        {
            m_last_opcode = 0;
        }


        // required operation overrides
        public abstract int z80daisy_irq_state();
        public abstract int z80daisy_irq_ack();
        public abstract void z80daisy_irq_reti();


        // instruction decoding
        //void z80daisy_decode(uint8_t opcode);
    }


    // ======================> z80_daisy_chain_interface
    public class z80_daisy_chain_interface : device_interface
    {
        const bool VERBOSE = false;


        z80_daisy_config [] m_daisy_config;
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
        public void set_daisy_config(z80_daisy_config [] config) { m_daisy_config = config; }


        // getters
        public bool daisy_chain_present() { return m_chain != null; }


        string daisy_show_chain()
        {
            string result = "";

            // loop over all devices
            for (device_z80daisy_interface intf = m_chain; intf != null; intf = intf.m_daisy_next)
            {
                if (intf != m_chain)
                    result += " -> ";
                result += intf.device().tag();
            }

            return result;
        }


        // interface-level overrides
        public override void interface_post_start()
        {
            if (m_daisy_config != null)
                daisy_init(m_daisy_config);
        }


        public override void interface_post_reset()
        {
            // loop over all chained devices and call their reset function
            for (device_z80daisy_interface intf = m_chain; intf != null; intf = intf.m_daisy_next)
                intf.device().reset();
        }


        // initialization
        void daisy_init(z80_daisy_config [] daisy)
        {
            assert(daisy != null);

            // create a linked list of devices
            device_z80daisy_interface tailptr = m_chain;  //device_z80daisy_interface **tailptr = &m_chain;
            int daisyIdx = 0;
            for ( ; daisy[daisyIdx].devname != null; daisyIdx++)  //for ( ; daisy->devname != nullptr; daisy++)
            {
                // find the device
                device_t target = device().subdevice(daisy[daisyIdx].devname);  //device_t *target = device().subdevice(daisy->devname);
                if (target == null)
                {
                    target = device().siblingdevice(daisy[daisyIdx].devname);  //target = device().siblingdevice(daisy->devname);
                    if (target == null)
                        fatalerror("Unable to locate device '{0}'\n", daisy[daisyIdx].devname);  //fatalerror("Unable to locate device '%s'\n", daisy->devname);
                }

                // make sure it has an interface
                device_z80daisy_interface intf;
                if (!target.interface_(out intf))
                    fatalerror("Device '{0}' does not implement the z80daisy interface!\n", daisy[daisyIdx].devname);  //fatalerror("Device '%s' does not implement the z80daisy interface!\n", daisy->devname);

                // splice it out of the list if it was previously added
                device_z80daisy_interface oldtailptr = tailptr;  //device_z80daisy_interface **oldtailptr = tailptr;
                while (oldtailptr != null)  //while (*oldtailptr != nullptr)
                {
                    if (oldtailptr == intf)  //if (*oldtailptr == intf)
                        oldtailptr = oldtailptr.m_daisy_next;  //*oldtailptr = (*oldtailptr)->m_daisy_next;
                    else
                        oldtailptr = oldtailptr.m_daisy_next;  //oldtailptr = &(*oldtailptr)->m_daisy_next;
                }

                // add the interface to the list
                intf.m_daisy_next = tailptr;  //intf->m_daisy_next = *tailptr;
                tailptr = intf;  //*tailptr = intf;
                tailptr = tailptr.m_daisy_next;  //tailptr = &(*tailptr)->m_daisy_next;
            }

            osd_printf_verbose("Daisy chain = {0}\n", daisy_show_chain());
        }


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
