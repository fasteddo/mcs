// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;  //using offs_t = u32;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    // ======================> cpu_device
    public abstract class cpu_device : device_t
                                       //public device_execute_interface,
                                       //public device_memory_interface,
                                       //public device_state_interface,
                                       //public device_disasm_interface
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

        //~cpu_device() { }


        // configuration helpers
        //void set_force_no_drc(bool value) { m_force_no_drc = value; }
        //bool allow_drc() const;


        // device_execute_interface helpers
        public attotime cycles_to_attotime(u64 cycles) { return execute().cycles_to_attotime(cycles); }
        public void set_disable() { execute().set_disable(); }
        public void set_input_line(int linenum, int state) { execute().set_input_line(linenum, state); }
        public void set_input_line_vector(int linenum, int vector) { execute().set_input_line_vector(linenum, vector); }
        public void set_input_line_and_vector(int linenum, int state, int vector) { execute().set_input_line_and_vector(linenum, state, vector); }
        public void pulse_input_line(int irqline, attotime duration) { execute().pulse_input_line(irqline, duration); }
        public bool suspended(u32 reason = device_execute_interface.SUSPEND_ANY_REASON) { return execute().suspended(reason); }
        public u64 total_cycles() { return execute().total_cycles(); }
        public void set_icountptr(intref icount) { execute().set_icountptr(icount); }
        public int standard_irq_callback(int irqline) { return execute().standard_irq_callback(irqline); }
        public void debugger_instruction_hook(offs_t curpc) { execute().debugger_instruction_hook(curpc); }
    }
}
