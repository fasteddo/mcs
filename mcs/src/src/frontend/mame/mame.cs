// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using image_interface_enumerator = mame.device_interface_enumerator<mame.device_image_interface>;  //typedef device_interface_enumerator<device_image_interface> image_interface_enumerator;
using s32 = System.Int32;
using size_t = System.UInt64;

using static mame.cpp_global;
using static mame.emucore_global;
using static mame.emuopts_global;
using static mame.main_global;
using static mame.mameopts_global;
using static mame.osdcore_global;
using static mame.osdfile_global;
using static mame.version_global;


namespace mame
{
    public class mame_machine_manager : machine_manager
    {
        //DISABLE_COPYING(mame_machine_manager);


        plugin_options m_plugins;              // pointer to plugin options
        lua_engine m_lua;

        game_driver m_new_driver_pending;   // pointer to the next pending driver
        bool m_firstrun;

        emu_timer m_autoboot_timer;      // autoboot timer
        //std::unique_ptr<sol::load_result>  m_autoboot_script;   // auto-boot script
        mame_ui_manager m_ui;                  // internal data from ui.cpp
        cheat_manager m_cheat;            // internal data from cheat.cpp
        inifile_manager m_inifile;      // internal data from inifile.c for INIs
        favorite_manager m_favorite;     // internal data from inifile.c for favorites


        static mame_machine_manager s_manager;


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
            if (s_manager == null)
                s_manager = new mame_machine_manager(options, osd);
            return s_manager;
        }

        public static mame_machine_manager instance() { return s_manager; }


        //~mame_machine_manager()
        //{
        //    m_lua.reset();
        //    s_manager = nullptr;
        //}


        public static void close_instance()
        {
            if (s_manager != null)
            {
                s_manager.m_lua.Dispose();
                s_manager = null;
            }
        }


        //plugin_options &plugins() const { return *m_plugins; }
        public lua_engine lua() { return m_lua; }


        /***************************************************************************
            CORE IMPLEMENTATION
        ***************************************************************************/

        //-------------------------------------------------
        //  update_machine
        //-------------------------------------------------
        public override void update_machine()
        {
            m_lua.set_machine(machine());
            m_lua.attach_notifiers();
        }


        //-------------------------------------------------
        //  split
        //-------------------------------------------------
        static std.vector<string> split(string text, char sep)
        {
            std.vector<string> tokens = new std.vector<string>();
            size_t start = 0;
            size_t end = 0;
            while ((end = text.find(sep, start)) != npos)
            {
                string temp = text.substr(start, end - start);
                if (temp != "") tokens.push_back(temp);
                start = end + 1;
            }

            {
                string temp = text.substr(start);
                if (temp != "") tokens.push_back(temp);
            }

            return tokens;
        }


        void reset(running_machine machine)
        {
            // setup autoboot if needed
            m_autoboot_timer.adjust(new attotime(options().autoboot_delay(), 0), 0);
        }


        //TIMER_CALLBACK_MEMBER(mame_machine_manager::autoboot_callback)
        void autoboot_callback(s32 param)
        {
            if (!string.IsNullOrEmpty(options().autoboot_script()))
            {
                throw new emu_unimplemented();
#if false
                assert(m_autoboot_script);
                sol::protected_function func = *m_autoboot_script;
                sol::protected_function_result result = lua()->invoke(func);
                if (!result.valid())
                {
                    sol::error err = result;
                    sol::call_status status = result.status();
                    fatalerror("Error running autoboot script %s: %s error\n%s\n",
                            options().autoboot_script(),
                            sol::to_string(status),
                            err.what());
                }
#endif
            }
            else if (!string.IsNullOrEmpty(options().autoboot_command()))
            {
                throw new emu_unimplemented();
#if false
                string cmd = options().autoboot_command();  //std::string cmd(options().autoboot_command());
                strreplace(cmd, "'", "\\'");
                string val = "emu.keypost('".append(cmd).append("')");  //std::string val = std::string("emu.keypost('").append(cmd).append("')");
                auto &l(*lua());
                l.invoke(l.load_string(val));
#endif
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


        public override void before_load_settings(running_machine machine)
        {
            //throw new emu_unimplemented();
#if false
            m_lua.on_machine_before_load_settings();
#endif
        }


        //-------------------------------------------------
        //  missing_mandatory_images - search for devices
        //  which need an image to be loaded
        //-------------------------------------------------
        public std.vector<string> missing_mandatory_images()  //std::vector<std::reference_wrapper<const std::string>> mame_machine_manager::missing_mandatory_images()
        {
            std.vector<string> results = new std.vector<string>();
            assert(machine() != null);

            // make sure that any required image has a mounted file
            foreach (device_image_interface image in new image_interface_enumerator(machine().root_device()))
            {
                if (image.must_be_loaded())
                {
                    if (machine().options().image_option(image.instance_name()).value().empty())
                    {
                        // this is a missing image; give LUA plugins a chance to handle it
                        if (!lua().on_missing_mandatory_image(image.instance_name()))
                            results.push_back(image.instance_name());
                    }
                }
            }

            return results;
        }


        public override void create_custom(running_machine machine)
        {
            // start the inifile manager
            m_inifile = new inifile_manager(m_ui.options());

            // allocate autoboot timer
            m_autoboot_timer = machine.scheduler().timer_alloc(autoboot_callback);

            // start favorite manager
            m_favorite = new favorite_manager(m_ui.options());

            // attempt to load the autoboot script if configured
            //throw new emu_unimplemented();
#if false
            m_autoboot_script.reset();
#endif

            if (!string.IsNullOrEmpty(options().autoboot_script()))
            {
                throw new emu_unimplemented();
#if false
                auto result = lua()->load_script(options().autoboot_script());
                if (!result.valid())
                {
                    sol::error err = result;
                    sol::load_status status = result.status();
                    fatalerror("Error loading autoboot script %s: %s error\n%s\n",
                            options().autoboot_script(),
                            sol::to_string(status),
                            err.what());
                }
                m_autoboot_script.reset(new sol::load_result(std::move(result)));
                sol::protected_function func = *m_autoboot_script;
                sol::set_environment(lua()->make_environment(), func);
#endif
            }
        }


        public override void load_cheatfiles(running_machine machine)
        {
            // set up the cheat engine
            m_cheat = new cheat_manager(machine);
        }


        /* execute as configured by the OPTION_SYSTEMNAME option on the specified options */
        //-------------------------------------------------
        //  execute - run the core emulation
        //-------------------------------------------------
        public int execute()
        {
            bool started_empty = false;

            bool firstgame = true;

            // loop across multiple hard resets
            bool exit_pending = false;
            int error = EMU_ERR_NONE;

            while (error == EMU_ERR_NONE && !exit_pending)
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
                    m_options.revert(OPTION_PRIORITY_INI);

                    string errors;
                    mame_options.parse_standard_inis(m_options, out errors);
                }

                // otherwise, perform validity checks before anything else
                bool is_empty = system == ___empty.driver____empty;
                if (!is_empty)
                {
                    validity_checker valid = new validity_checker(m_options, true);
                    valid.set_verbose(false);
                    valid.check_shared_source(system);
                    valid.Dispose();
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
                machine.Dispose();
                set_machine(null);
            }

            // return an error
            return error;
        }


        //-------------------------------------------------
        //  start_luaengine
        //-------------------------------------------------
        public void start_luaengine()
        {
            if (options().plugins())
            {
                //throw new emu_unimplemented();
#if false
                // scan all plugin directories
                path_iterator iter = new path_iterator(options().plugins_path());
                string pluginpath;
                while (iter.next(out pluginpath))
                {
                    // user may specify environment variables; subsitute them
                    pluginpath = osd_subst_env(pluginpath);

                    // and then scan the directory recursively
                    m_plugins.scan_directory(pluginpath, true);
                }

                {
                    // parse the file
                    // attempt to open the output file
                    emu_file file = new emu_file(options().ini_path(), OPEN_FLAG_READ);
                    if (file.open("plugin.ini") == osd_file.error.NONE)
                    {
                        try
                        {
                            m_plugins.parse_ini_file(file.core_file_get());  //(util::core_file&)file
                        }
                        catch (options_exception )
                        {
                            osd_printf_error("**Error loading plugin.ini**\n");
                        }

                        file.close();
                    }
                }

                // process includes
                foreach (string incl in split(options().plugin(), ','))
                {
                    plugin_options::plugin *p = m_plugins->find(incl);
                    if (!p)
                        fatalerror("Fatal error: Could not load plugin: %s\n", incl);
                    p.m_start = true;
                }

                // process excludes
                foreach (string excl in split(options().no_plugin(), ','))
                {
                    plugin_options::plugin *p = m_plugins->find(excl);
                    if (!p)
                        fatalerror("Fatal error: Unknown plugin: %s\n", excl);
                    p.m_start = false;
                }
#endif
            }

            // we have a special way to open the console plugin
            if (options().console())
            {
                plugin_options.plugin p = m_plugins.find(OPTION_CONSOLE);
                if (p == null)
                    fatalerror("Fatal error: Console plugin not found.\n");

                p.m_start = true;
            }

            m_lua.initialize();

            {
                emu_file file = new emu_file(options().plugins_path(), OPEN_FLAG_READ);
                std.error_condition filerr = file.open("boot.lua");
                if (!filerr)
                {
                    throw new emu_unimplemented();
#if false
                    const std::string exppath = osd_subst_env(file.fullpath());ubst_env(out exppath, file.fullpath());
                    auto &l(*lua());
                    auto load_result = l.load_script(exppath);
                    if (!load_result.valid())
                    {
                        sol::error err = load_result;
                        sol::load_status status = load_result.status();
                        fatalerror("Error plugin bootstrap script %s: %s error\n%s\n",
                                exppath,
                                sol::to_string(status),
                                err.what());
                    }
                    sol::protected_function func = load_result;
                    sol::protected_function_result call_result = l.invoke(func);
                    if (!call_result.valid())
                    {
                        sol::error err = call_result;
                        sol::call_status status = call_result.status();
                        fatalerror("Error running plugin bootstrap script %s: %s error\n%s\n",
                                options().autoboot_script(),
                                sol::to_string(status),
                                err.what());
                    }
#endif
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


        public mame_ui_manager ui() { assert(m_ui != null); return m_ui; }
        public cheat_manager cheat() { assert(m_cheat != null); return m_cheat; }
        public inifile_manager inifile() { assert(m_inifile != null); return m_inifile; }
        public favorite_manager favorite() { assert(m_favorite != null); return m_favorite; }
    }


    public static partial class emulator_info
    {
        public static string get_bare_build_version() { return bare_build_version; }
        public static string get_build_version() { return build_version; }

        public static void display_ui_chooser(running_machine machine)
        {
            mame_ui_manager mui = mame_machine_manager.instance().ui();
            render_container container = machine.render().ui_container();
            if (machine.options().ui() == emu_options.ui_option.UI_SIMPLE)
                ui.simple_menu_select_game.force_game_select(mui, container);
            else
                ui.menu_select_game.force_game_select(mui, container);
        }

        public static int start_frontend(emu_options options, osd_interface osd, std.vector<string> args)
        {
            cli_frontend frontend = new cli_frontend(options, osd);
            return frontend.execute(args);
        }

        public static void draw_user_interface(running_machine machine) { mame_machine_manager.instance().ui().update_and_render(machine.render().ui_container()); }

        public static void periodic_check() { mame_machine_manager.instance().lua().on_periodic(); }

        public static bool frame_hook() { return mame_machine_manager.instance().lua().frame_hook(); }

        public static void sound_hook() { mame_machine_manager.instance().lua().on_sound_update(); }

        public static void layout_script_cb(layout_file file, string script)
        {
            throw new emu_unimplemented();
        }

        public static bool standalone() { return false; }
    }
}
