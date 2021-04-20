// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    // ======================> cheat_manager
    // private machine-global data
    public class cheat_manager
    {
        // constants
        //static const int CHEAT_VERSION = 1;


        // internal state
        //running_machine &   m_machine;                          // reference to our machine
        //std::vector<std::unique_ptr<cheat_entry>> m_cheatlist;                   // cheat list
        //UINT64              m_framecount;                       // frame count
        //std::vector<std::string>  m_output;                     // array of output strings
        //std::vector<ui::text_layout::text_justify> m_justify;   // justification for each string
        //UINT8               m_numlines;                         // number of lines available for output
        //INT8                m_lastline;                         // last line used for output
        bool m_disabled;                         // true if the cheat engine is disabled
        //symbol_table        m_symtable;                         // global symbol table
        //std::unique_ptr<debugger_cpu> m_cpu;                    // debugger interface for cpus/memory


        // construction/destruction
        //-------------------------------------------------
        //  cheat_manager - constructor
        //-------------------------------------------------
        public cheat_manager(running_machine machine)
        {
            m_disabled = true;

            //throw new emu_unimplemented();
#if false
#endif
        }


        // getters
        //running_machine &machine() const { return m_machine; }
        public bool enabled() { return !m_disabled; }
        //const simple_list<cheat_entry> &entries() const { return m_cheatlist; }


        // setters
        //-------------------------------------------------
        //  set_enable - globally enable or disable the
        //  cheat engine
        //-------------------------------------------------
        public void set_enable(bool enable)
        {
            throw new emu_unimplemented();
#if false
#endif
        }


        // actions
        //void reload();
        //bool save_all(const char *filename);

        //-------------------------------------------------
        //  render_text - called by the UI system to
        //  render text
        //-------------------------------------------------
        public void render_text(mame_ui_manager mui, render_container container)
        {
            //throw new emu_unimplemented();
#if false
#endif
        }


        // output helpers
        //astring &get_output_string(int row, ui::text_layout::text_justify justify);


        // global helpers
        //static const char *quote_expression(astring &string, const parsed_expression &expression);
        //static uint64_t execute_frombcd(int params, uint64_t const *param);
        //static uint64_t execute_tobcd(int params, uint64_t const *param);


        // internal helpers
        //void frame_update();
        //void load_cheats(const char *filename);
    }
}
