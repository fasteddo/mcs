// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.osdcore_global;
using static mame.osdfile_global;


namespace mame
{
    public enum config_type : int
    {
        INIT,           // opportunity to initialize things first
        CONTROLLER,     // loading from controller file
        DEFAULT,        // loading from default.cfg
        SYSTEM,         // loading from system.cfg
        FINAL           // opportunity to finish initialization
    }


    public enum config_level : int
    {
        DEFAULT,
        SOURCE,
        BIOS,
        PARENT,
        SYSTEM
    }


    // ======================> configuration_manager
    public class configuration_manager
    {
        class config_element
        {
            public string name;              // node name
            public load_delegate load;              // load callback
            public save_delegate save;              // save callback
        }


        public delegate void load_delegate(config_type param1, config_level param2, util.xml.data_node param3);  //typedef delegate<void (config_type, config_level, util::xml::data_node const *)> load_delegate;
        public delegate void save_delegate(config_type param1, util.xml.data_node param2);  //typedef delegate<void (config_type, util::xml::data_node *)> save_delegate;


        public const int CONFIG_VERSION = 10;


        // internal state
        running_machine m_machine;                  // reference to our machine
        std.vector<config_element> m_typelist = new std.vector<config_element>();


        // construction/destruction
        //-------------------------------------------------
        //  configuration_manager - constructor
        //-------------------------------------------------
        public configuration_manager(running_machine machine)
        {
            m_machine = machine;
        }


        /*************************************
         *
         *  Register to be involved in config
         *  save/load
         *
         *************************************/
        public void config_register(string nodename, load_delegate load, save_delegate save)
        {
            config_element element = new config_element();
            element.name = nodename;
            element.load = load;
            element.save = save;

            m_typelist.emplace_back(element);
        }


        /*************************************
         *
         *  Settings save/load frontend
         *
         *************************************/
        public bool load_settings()
        {
            // loop over all registrants and call their init function
            foreach (var type in m_typelist)
                type.load(config_type.INIT, config_level.DEFAULT, null);

            // now load the controller file
            string controller = machine().options().ctrlr();
            if (!string.IsNullOrEmpty(controller))
            {
                // open the config file
                emu_file file = new emu_file(machine().options().ctrlr_path(), OPEN_FLAG_READ);
                osd_printf_verbose("Attempting to parse: {0}.cfg\n", controller);
                std.error_condition filerr = file.open(controller + ".cfg");
                if (filerr)
                    throw new emu_fatalerror("Could not open controller file {0}.cfg ({1}:{2} {3})", controller, filerr.category().name(), filerr.value(), filerr.message());

                // load the XML
                if (!load_xml(file, config_type.CONTROLLER))
                    throw new emu_fatalerror("Could not load controller file {0}.cfg", controller);

                file.close();
            }

            bool loaded;

            {
                // next load the defaults file
                emu_file file = new emu_file(machine().options().cfg_directory(), OPEN_FLAG_READ);
                std.error_condition filerr = file.open("default.cfg");
                osd_printf_verbose("Attempting to parse: default.cfg\n");
                if (!filerr)
                    load_xml(file, config_type.DEFAULT);

                // finally, load the game-specific file
                filerr = file.open(machine().basename() + ".cfg");
                osd_printf_verbose("Attempting to parse: {0}.cfg\n", machine().basename());
                loaded = !filerr && load_xml(file, config_type.SYSTEM);

                file.close();
            }

            // loop over all registrants and call their final function
            foreach (var type in m_typelist)
                type.load(config_type.FINAL, config_level.DEFAULT, null);

            // if we didn't find a saved config, return false so the main core knows that it
            // is the first time the game is run and it should display the disclaimer.
            return loaded;
        }


        public void save_settings()
        {
            // loop over all registrants and call their init function
            foreach (var type in m_typelist)
                type.save(config_type.INIT, null);

            // save the defaults file
            emu_file file = new emu_file(machine().options().cfg_directory(), OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
            std.error_condition filerr = file.open("default.cfg");
            if (!filerr)
                save_xml(file, config_type.DEFAULT);

            // finally, save the system-specific file
            filerr = file.open(machine().basename() + ".cfg");
            if (!filerr)
                save_xml(file, config_type.SYSTEM);

            file.close();

            // loop over all registrants and call their final function
            foreach (var type in m_typelist)
                type.save(config_type.FINAL, null);
        }


        // getters
        running_machine machine() { return m_machine; }


        /*************************************
         *
         *  XML file load
         *
         *************************************/
        bool load_xml(emu_file file, config_type which_type)
        {
            throw new emu_unimplemented();
        }


        /*************************************
         *
         *  XML file save
         *
         *************************************/
        bool save_xml(emu_file file, config_type which_type)
        {
            throw new emu_unimplemented();
        }
    }
}
