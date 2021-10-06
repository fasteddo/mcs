// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class debugger_global
    {
        /* OSD can call this to safely flush all traces in the event of a crash */
        //void debugger_flush_all_traces_on_abnormal_exit();
    }


    // ======================> debugger_manager
    public class debugger_manager : IDisposable
    {
        // internal state
        running_machine m_machine;                  // reference to our machine

        //std::unique_ptr<debugger_commands> m_commands;
        //std::unique_ptr<debugger_cpu> m_cpu;
        //std::unique_ptr<debugger_console> m_console;


        // construction/destruction
        //-------------------------------------------------
        //  debugger_manager - constructor
        //-------------------------------------------------
        public debugger_manager(running_machine machine)
        {
            m_machine = machine;


            //throw new emu_unimplemented();
#if false
            /* initialize the submodules */
            m_cpu = std::make_unique<debugger_cpu>(machine);
            m_console = std::make_unique<debugger_console>(machine);
            m_commands = std::make_unique<debugger_commands>(machine, cpu(), console());

            g_machine = &machine;

            /* register an atexit handler if we haven't yet */
            if (!g_atexit_registered)
                atexit(debugger_flush_all_traces_on_abnormal_exit);
            g_atexit_registered = TRUE;

            /* initialize osd debugger features */
            machine.osd().init_debugger();
#endif
        }

        ~debugger_manager()
        {
            g.assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            //throw new emu_unimplemented();
#if false
            g_machine = null;
#endif

            m_isDisposed = true;
        }


        // break into the debugger
        //void debug_break();


        public bool within_instruction_hook()
        {
            throw new emu_unimplemented();
        }


        /* redraw the current video display */
        //void refresh_display();


        // getters
        //running_machine &machine() const { return m_machine; }


        //debugger_commands &commands() const { return *m_commands; }
        //debugger_cpu &cpu() const { return *m_cpu; }
        //debugger_console &console() const { return *m_console; }
    }
}
