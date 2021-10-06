// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class main_global
    {
        public const int EMU_ERR_NONE             = 0;    // no error
        public const int EMU_ERR_FAILED_VALIDITY  = 1;    // failed validity checks
        public const int EMU_ERR_MISSING_FILES    = 2;    // missing files
        const int EMU_ERR_FATALERROR       = 3;    // some other fatal error
        public const int EMU_ERR_DEVICE           = 4;    // device initialization error
        public const int EMU_ERR_NO_SUCH_SYSTEM   = 5;    // system was specified but doesn't exist
        public const int EMU_ERR_INVALID_CONFIG   = 6;    // some sort of error in configuration
        const int EMU_ERR_IDENT_NONROMS    = 7;    // identified all non-ROM files
        const int EMU_ERR_IDENT_PARTIAL    = 8;    // identified some files but not all
        const int EMU_ERR_IDENT_NONE       = 9;    // identified no files
    }


    public static partial class emulator_info
    {
        public static string get_appname() { return mame_global_emulator_info.get_appname(); }
        public static string get_appname_lower() { return mame_global_emulator_info.get_appname_lower(); }
        public static string get_configname() { return mame_global_emulator_info.get_configname(); }
        public static string get_copyright() { return mame_global_emulator_info.get_copyright(); }
        public static string get_copyright_info() { return mame_global_emulator_info.get_copyright_info(); }
        //public static string get_bare_build_version();
        //public static string get_build_version();
        //public static void display_ui_chooser(running_machine machine);
        //public static int start_frontend(emu_options options, osd_interface osd, List<string> args);
        //public static void draw_user_interface(running_machine machine);
        //public static void periodic_check();
        //public static bool frame_hook();
        public static void sound_hook() { mame_machine_manager.instance().lua().on_sound_update(); }
        //static void layout_script_cb(layout_file &file, const char *script);
        //static bool standalone();
    }


    // ======================> machine_manager
    public class machine_manager
    {
        //DISABLE_COPYING(machine_manager);


        osd_interface m_osd;                  // reference to OSD system
        protected emu_options m_options;              // reference to options
        running_machine m_machine;
        http_manager m_http;


        // construction/destruction
        protected machine_manager(emu_options options, osd_interface osd)
        {
            m_osd = osd;
            m_options = options;
            m_machine = null;
        }


        public osd_interface osd() { return m_osd; }
        protected emu_options options() { return m_options; }
        public running_machine machine() { return m_machine; }

        protected void set_machine(running_machine machine) { m_machine = machine; }

        public virtual ui_manager create_ui(running_machine machine) { return null;  }
        public virtual void create_custom(running_machine machine) { }
        public virtual void load_cheatfiles(running_machine machine) { }
        public virtual void ui_initialize(running_machine machine) { }
        public virtual void before_load_settings(running_machine machine) { }

        public virtual void update_machine() { }

        public http_manager http() { return m_http; }

        public void start_http_server()
        {
            m_http = new http_manager(options().http(), options().http_port(), options().http_root());
        }
    }
}
