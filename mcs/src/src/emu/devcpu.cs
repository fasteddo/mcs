// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;


namespace mame
{
    public static class devcpu_global
    {
        //**************************************************************************
        //  CPU DEVICE CONFIGURATION MACROS
        //**************************************************************************

        // recompilation parameters
        //#define MCFG_CPU_FORCE_NO_DRC()             dynamic_cast<cpu_device &>(*device).set_force_no_drc(true);
    }


    // ======================> cpu_device
    public class cpu_device : device_t
#if false
        public device_execute_interface,
        public device_memory_interface,
        public device_state_interface,
        public device_disasm_interface
#endif
    {
        // configured state
        bool m_force_no_drc;             // whether or not to force DRC off


        // construction/destruction

        //-------------------------------------------------
        //  cpu_device - constructor
        //-------------------------------------------------
        public cpu_device(machine_config mconfig, device_type type, string tag, device_t owner, UInt32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
#if false
            m_class_interfaces.Add(new device_execute_interface(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface(mconfig, this));
            m_class_interfaces.Add(new device_state_interface(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface(mconfig, this));
#endif


            m_force_no_drc = false;
        }

        ~cpu_device() { }


        // configuration helpers
        //void set_force_no_drc(bool value) { m_force_no_drc = value; }
        //bool allow_drc() const;
    }
}
