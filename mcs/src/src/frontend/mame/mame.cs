// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static partial class emulator_info
    {
        public static string get_bare_build_version() { return version_global.bare_build_version; }
        public static string get_build_version() { return version_global.build_version; }

        public static void display_ui_chooser(running_machine machine)
        {
            mame_ui_manager mui = mame_machine_manager.instance().ui();
            render_container container = machine.render().ui_container();
            if (machine.options().ui() == emu_options.ui_option.UI_SIMPLE)
                ui.simple_menu_select_game.force_game_select(mui, container);
            else
                ui.menu_select_game.force_game_select(mui, container);
        }

        public static int start_frontend(emu_options options, osd_interface osd, std_vector<string> args)
        {
            cli_frontend frontend = new cli_frontend(options, osd);
            return frontend.execute(args);
        }

        public static void draw_user_interface(running_machine machine) { mame_machine_manager.instance().ui().update_and_render(machine.render().ui_container()); }

        public static void periodic_check() { mame_machine_manager.instance().lua().on_periodic(); }

        public static bool frame_hook() { return mame_machine_manager.instance().lua().frame_hook(); }

        public static void layout_file_cb(util.xml.data_node layout)
        {
            throw new emu_unimplemented();
        }

        public static bool standalone() { return false; }
    }


    public class mame_machine_manager : machine_manager
    {
        //DISABLE_COPYING(mame_machine_manager);


        plugin_options m_plugins;              // pointer to plugin options
        lua_engine m_lua;

        game_driver m_new_driver_pending;   // pointer to the next pending driver
        bool m_firstrun;

        static mame_machine_manager m_manager;

        emu_timer m_autoboot_timer;      // autoboot timer
        mame_ui_manager m_ui;                  // internal data from ui.cpp
        cheat_manager m_cheat;            // internal data from cheat.cpp
        inifile_manager m_inifile;      // internal data from inifile.c for INIs
        favorite_manager m_favorite;     // internal data from inifile.c for favorites


        // construction/destruction
        //-------------------------------------------------
        //  mame_machine_manager - constructor
        //-------------------------------------------------
        mame_machine_manager(emu_options options, osd_interface osd)
            : base(options, osd)
        {
            m_plugins = new plugin_options();
            m_lua = new lua_engine();
            m_new_driver_pending = null;
            m_firstrun = true;
            m_autoboot_timer = null;
        }


        public static mame_machine_manager instance(emu_options options, osd_interface osd)
        {
            if (m_manager == null)
                m_manager = new mame_machine_manager(options, osd);
            return m_manager;
        }

        public static mame_machine_manager instance() { return m_manager; }


        ~mame_machine_manager()
        {
            m_lua = null;  //global_free(m_lua);
            m_manager = null;
        }


        //plugin_options &plugins() const { return *m_plugins; }
        public lua_engine lua() { return m_lua; }


        /***************************************************************************
            CORE IMPLEMENTATION
        ***************************************************************************/
        public override void update_machine()
        {
            m_lua.set_machine(machine());
            m_lua.attach_notifiers();
        }


        void reset(running_machine machine)
        {
            // setup autoboot if needed
            m_autoboot_timer.adjust(new attotime(options().autoboot_delay(), 0), 0);
        }


        //TIMER_CALLBACK_MEMBER(mame_machine_manager::autoboot_callback)
        void autoboot_callback(object ptr, int param)
        {
            if (options().autoboot_script().Length != 0)
            {
                mame_machine_manager.instance().lua().load_script(options().autoboot_script());
            }
            else if (options().autoboot_command().Length != 0)
            {
                string cmd = options().autoboot_command();
                cmd = cmd.Replace("'", "\\'");
                string val = "emu.keypost('" + cmd + "')";
                mame_machine_manager.instance().lua().load_string(val);
            }
        }


        public override ui_manager create_ui(running_machine machine)
        {
            m_ui = new mame_ui_manager(machine);
            m_ui.init();

            machine.add_notifier(machine_notification.MACHINE_NOTIFY_RESET, reset);

            m_ui.set_startup_text("Initializing...", true);

            return m_ui;
        }


        public override void ui_initialize(running_machine machine)
        {
            m_ui.initialize(machine);

            // display the startup screens
            m_ui.display_startup_screens(m_firstrun);
        }


        public override void create_custom(running_machine machine)
        {
            // start the inifile manager
            m_inifile = new inifile_manager(machine, m_ui.options());

            // allocate autoboot timer
            m_autoboot_timer = machine.scheduler().timer_alloc(autoboot_callback);

            // start favorite manager
            m_favorite = new favorite_manager(machine, m_ui.options());
        }


        public override void load_cheatfiles(running_machine machine)
        {
            // set up the cheat engine
            m_cheat = new cheat_manager(machine);
        }


        /* execute as configured by the OPTION_SYSTEMNAME option on the specified options */
        /*-------------------------------------------------
            execute - run the core emulation
        -------------------------------------------------*/
        public int execute()
        {
            bool started_empty = false;

            bool firstgame = true;

            // loop across multiple hard resets
            bool exit_pending = false;
            int error = (int)EMU_ERR.EMU_ERR_NONE;

            while (error == (int)EMU_ERR.EMU_ERR_NONE && !exit_pending)
            {
                m_new_driver_pending = null;

                // if no driver, use the internal empty driver
                game_driver system = mame_options.system(m_options);
                if (system == null)
                {
                    system = ___empty.driver____empty;
                    if (firstgame)
                        started_empty = true;
                }

                firstgame = false;

                // parse any INI files as the first thing
                if (m_options.read_config())
                {
                    // but first, revert out any potential game-specific INI settings from previous runs via the internal UI
                    m_options.revert((int)OPTION_PRIORITY.OPTION_PRIORITY_INI);

                    string errors;
                    mame_options.parse_standard_inis(m_options, out errors);
                }

                // otherwise, perform validity checks before anything else
                bool is_empty = system == ___empty.driver____empty;
                if (!is_empty)
                {
                    validity_checker valid = new validity_checker(m_options);
                    valid.set_verbose(false);
                    valid.check_shared_source(system);
                }

                // create the machine configuration
                machine_config config = new machine_config(system, m_options);

                // create the machine structure and driver
                running_machine machine = new running_machine(config, this);

                set_machine(machine);

                // run the machine
                error = machine.run(is_empty);
                m_firstrun = false;

                // check the state of the machine
                if (m_new_driver_pending != null)
                {
                    // set up new system name and adjust device options accordingly
                    m_options.set_system_name(m_new_driver_pending.name);
                    m_firstrun = true;
                }
                else
                {
                    if (machine.exit_pending())
                        m_options.set_system_name("");
                }

                if (machine.exit_pending() && (!started_empty || is_empty))
                    exit_pending = true;

                // machine will go away when we exit scope
                set_machine(null);
            }

            // return an error
            return error;
        }


        public void start_luaengine()
        {
            if (options().plugins())
            {
                path_iterator iter = new path_iterator(options().plugins_path());
                string pluginpath;
                while (iter.next(out pluginpath))
                {
                    m_plugins.parse_json(pluginpath);
                }

                string [] include = options().plugin() == null ? new string[0] : options().plugin().Split(',');  // split(options().plugin(),',');
                string [] exclude = options().no_plugin() == null ? new string[0] : options().no_plugin().Split(',');

                {
                    // parse the file
                    // attempt to open the output file
                    emu_file file = new emu_file(options().ini_path(), osdcore_global.OPEN_FLAG_READ);
                    if (file.open("plugin.ini") == osd_file.error.NONE)
                    {
                        try
                        {
                            m_plugins.parse_ini_file(file.core_file_get(), (int)OPTION_PRIORITY.OPTION_PRIORITY_MAME_INI, OPTION_PRIORITY.OPTION_PRIORITY_MAME_INI < OPTION_PRIORITY.OPTION_PRIORITY_DRIVER_INI, false);
                        }
                        catch (options_exception )
                        {
                            global.osd_printf_error("**Error loading plugin.ini**\n");
                        }
                    }
                }

                foreach (var curentry in m_plugins.entries())
                {
                    if (curentry.type() != core_options.option_type.HEADER)
                    {
                        if (Array.Exists(include, s => s == curentry.name()))  // std::find(include.begin(), include.end(), curentry.name()) != include.end())
                        {
                            m_plugins.set_value(curentry.name(), "1", emu_options.OPTION_PRIORITY_CMDLINE);
                        }

                        if (Array.Exists(exclude, s => s == curentry.name()))  // std::find(exclude.begin(), exclude.end(), curentry.name()) != exclude.end())
                        {
                            m_plugins.set_value(curentry.name(), "0", emu_options.OPTION_PRIORITY_CMDLINE);
                        }
                    }
                }
            }

            if (options().console())
            {
                m_plugins.set_value("console", "1", emu_options.OPTION_PRIORITY_CMDLINE);
                if (m_plugins.exists(emu_options.OPTION_CONSOLE))
                {
                    m_plugins.set_value(emu_options.OPTION_CONSOLE, "1", emu_options.OPTION_PRIORITY_CMDLINE);
                }
                else
                {
                    global.fatalerror("Console plugin not found.\n");
                }
            }

            m_lua.initialize();

            {
                emu_file file = new emu_file(options().plugins_path(), osdcore_global.OPEN_FLAG_READ);
                osd_file.error filerr = file.open("boot.lua");
                if (filerr == osd_file.error.NONE)
                {
                    string exppath;
                    osdcore_global.m_osdcore.osd_subst_env(out exppath, file.fullpath());
                    m_lua.load_script(file.fullpath());
                }
            }
        }


        //-------------------------------------------------
        //  mame_schedule_new_driver - schedule a new game to
        //  be loaded
        //-------------------------------------------------
        public void schedule_new_driver(game_driver driver)
        {
            m_new_driver_pending = driver;
        }


        public mame_ui_manager ui() { global.assert(m_ui != null); return m_ui; }
        public cheat_manager cheat() { global.assert(m_cheat != null); return m_cheat; }
        public inifile_manager inifile() { global.assert(m_inifile != null); return m_inifile; }
        public favorite_manager favorite() { global.assert(m_favorite != null); return m_favorite; }
    }
}
