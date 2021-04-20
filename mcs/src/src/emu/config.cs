// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    //typedef delegate<void (config_type, xml_data_node *)> config_saveload_delegate;
    public delegate void config_saveload_delegate(config_type param1, util.xml.data_node param2);


    public enum config_type
    {
        INIT = 0,       // opportunity to initialize things first
        CONTROLLER,     // loading from controller file
        DEFAULT,        // loading from default.cfg
        GAME,           // loading from game.cfg
        FINAL           // opportunity to finish initialization
    }


    // ======================> configuration_manager
    public class configuration_manager : global_object
    {
        class config_element
        {
            public string name;              /* node name */
            public config_saveload_delegate load;              /* load callback */
            public config_saveload_delegate save;              /* save callback */
        }


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
        public void config_register(string nodename, config_saveload_delegate load, config_saveload_delegate save)
        {
            config_element element = new config_element();
            element.name = nodename;
            element.load = load;
            element.save = save;

            m_typelist.Add(element);
        }


        /*************************************
         *
         *  Settings save/load frontend
         *
         *************************************/
        public int load_settings()
        {
            string controller = machine().options().ctrlr();
            int loaded = 0;

            /* loop over all registrants and call their init function */
            foreach (var type in m_typelist)
                type.load(config_type.INIT, null);

            /* now load the controller file */
            if (!string.IsNullOrEmpty(controller))
            {
                /* open the config file */
                emu_file file = new emu_file(machine().options().ctrlr_path(), OPEN_FLAG_READ);
                osd_printf_verbose("Attempting to parse: {0}.cfg\n", controller);
                osd_file.error filerr = file.open(controller + ".cfg");

                if (filerr != osd_file.error.NONE)
                    throw new emu_fatalerror("Could not load controller file {0}.cfg", controller);

                /* load the XML */
                if (load_xml(file, config_type.CONTROLLER) == 0)
                    throw new emu_fatalerror("Could not load controller file {0}.cfg", controller);

                file.close();
            }

            {
                /* next load the defaults file */
                emu_file file = new emu_file(machine().options().cfg_directory(), OPEN_FLAG_READ);
                osd_file.error filerr = file.open("default.cfg");
                osd_printf_verbose("Attempting to parse: default.cfg\n");
                if (filerr == osd_file.error.NONE)
                    load_xml(file, config_type.DEFAULT);

                /* finally, load the game-specific file */
                filerr = file.open(machine().basename() + ".cfg");
                osd_printf_verbose("Attempting to parse: {0}.cfg\n", machine().basename());
                if (filerr == osd_file.error.NONE)
                    loaded = load_xml(file, config_type.GAME);

                file.close();
            }

            /* loop over all registrants and call their final function */
            foreach (var type in m_typelist)
                type.load(config_type.FINAL, null);

            /* if we didn't find a saved config, return 0 so the main core knows that it */
            /* is the first time the game is run and it should display the disclaimer. */
            return loaded;
        }


        public void save_settings()
        {
            /* loop over all registrants and call their init function */
            foreach (var type in m_typelist)
                type.save(config_type.INIT, null);

            /* save the defaults file */
            emu_file file = new emu_file(machine().options().cfg_directory(), OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
            osd_file.error filerr = file.open("default.cfg");
            if (filerr == osd_file.error.NONE)
                save_xml(file, config_type.DEFAULT);

            /* finally, save the game-specific file */
            filerr = file.open(machine().basename() + ".cfg");
            if (filerr == osd_file.error.NONE)
                save_xml(file, config_type.GAME);

            file.close();

            /* loop over all registrants and call their final function */
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
        int load_xml(emu_file file, config_type which_type)
        {
            throw new emu_unimplemented();
        }


        /*************************************
         *
         *  XML file save
         *
         *************************************/
        int save_xml(emu_file file, config_type which_type)
        {
            throw new emu_unimplemented();
        }
    }
}
