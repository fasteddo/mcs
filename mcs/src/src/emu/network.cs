// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    // ======================> network_manager
    class network_manager
    {
        // internal state
        running_machine m_machine;                  // reference to our machine


        // construction/destruction
        //-------------------------------------------------
        //  network_manager - constructor
        //-------------------------------------------------
        public network_manager(running_machine machine)
        {
            m_machine = machine;


            machine.configuration().config_register("network", config_load, config_save);
        }


        // getters
        running_machine machine() { return m_machine; }


        void config_load(config_type cfg_type, config_level cfg_lvl, util.xml.data_node parentnode)
        {
            //throw new emu_unimplemented();
        }


        void config_save(config_type cfg_type, util.xml.data_node parentnode)
        {
            //throw new emu_unimplemented();
        }
    }
}
