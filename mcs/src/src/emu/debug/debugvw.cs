// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    // debug_view_manager manages all the views
    class debug_view_manager
    {
        // internal state
        //running_machine &   m_machine;              // reference to our machine
        //debug_view *        m_viewlist;             // list of views


        // construction/destruction
        //-------------------------------------------------
        //  debug_view_manager - constructor
        //-------------------------------------------------
        public debug_view_manager(running_machine machine)
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  ~debug_view_manager - destructor
        //-------------------------------------------------
        ~debug_view_manager()
        {
            throw new emu_unimplemented();
        }

        // getters
        //running_machine &machine() const { return m_machine; }


        // view allocation
        //debug_view *alloc_view(debug_view_type type, debug_view_osd_update_func osdupdate, void *osdprivate);
        //void free_view(debug_view &view);


        // update helpers
        //void update_all(debug_view_type type = DVT_NONE);
        //void update_all_except(debug_view_type type = DVT_NONE);
        //void flush_osd_updates();


        // private helpers
        //debug_view *append(debug_view *view);
    }
}
