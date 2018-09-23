// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    // option priorities
    public enum OPTION_PRIORITY
    {
        // command-line options are HIGH priority
        OPTION_PRIORITY_SUBCMD = options_global.OPTION_PRIORITY_HIGH,
        OPTION_PRIORITY_CMDLINE,

        // INI-based options are NORMAL priority, in increasing order:
        OPTION_PRIORITY_MAME_INI = options_global.OPTION_PRIORITY_NORMAL + 1,
        OPTION_PRIORITY_DEBUG_INI,
        OPTION_PRIORITY_ORIENTATION_INI,
        OPTION_PRIORITY_SYSTYPE_INI,
        OPTION_PRIORITY_SCREEN_INI,
        OPTION_PRIORITY_SOURCE_INI,
        OPTION_PRIORITY_GPARENT_INI,
        OPTION_PRIORITY_PARENT_INI,
        OPTION_PRIORITY_DRIVER_INI,
        OPTION_PRIORITY_INI,
    }


    static class mame_options
    {
        static int m_slot_options = 0;
        static int m_device_options = 0;


        // parsing wrappers

        //-------------------------------------------------
        //  parse_standard_inis - parse the standard set
        //  of INI files
        //-------------------------------------------------
        public static void parse_standard_inis(emu_options options, out string error_stream, game_driver driver = null)
        {
            error_stream = "";

            // parse the INI file defined by the platform (e.g., "mame.ini")
            // we do this twice so that the first file can change the INI path
            parse_one_ini(options, emulator_info.get_configname(), (int)OPTION_PRIORITY.OPTION_PRIORITY_MAME_INI, ref error_stream);
            parse_one_ini(options, emulator_info.get_configname(), (int)OPTION_PRIORITY.OPTION_PRIORITY_MAME_INI, ref error_stream);

            // debug mode: parse "debug.ini" as well
            if (options.debug())
                parse_one_ini(options, "debug", (int)OPTION_PRIORITY.OPTION_PRIORITY_DEBUG_INI, ref error_stream);

            // if we have a valid system driver, parse system-specific INI files
            game_driver cursystem = driver == null ? system(options) : driver;
            if (cursystem == null)
                return;

            // parse "vertical.ini" or "horizont.ini"
            if (((UInt32)cursystem.flags & emucore_global.ORIENTATION_SWAP_XY) != 0)
                parse_one_ini(options, "vertical", (int)OPTION_PRIORITY.OPTION_PRIORITY_ORIENTATION_INI, ref error_stream);
            else
                parse_one_ini(options, "horizont", (int)OPTION_PRIORITY.OPTION_PRIORITY_ORIENTATION_INI, ref error_stream);

            switch (cursystem.flags & machine_flags.type.MASK_TYPE)
            {
            case machine_flags.type.TYPE_ARCADE:
                parse_one_ini(options, "arcade", (int)OPTION_PRIORITY.OPTION_PRIORITY_SYSTYPE_INI, ref error_stream);
                break;
            case machine_flags.type.TYPE_CONSOLE:
                parse_one_ini(options, "console", (int)OPTION_PRIORITY.OPTION_PRIORITY_SYSTYPE_INI, ref error_stream);
                break;
            case machine_flags.type.TYPE_COMPUTER:
                parse_one_ini(options, "computer", (int)OPTION_PRIORITY.OPTION_PRIORITY_SYSTYPE_INI, ref error_stream);
                break;
            case machine_flags.type.TYPE_OTHER:
                parse_one_ini(options, "othersys", (int)OPTION_PRIORITY.OPTION_PRIORITY_SYSTYPE_INI, ref error_stream);
                break;
            default:
                break;
            }

            machine_config config = new machine_config(cursystem, options);
            foreach (screen_device device in new screen_device_iterator(config.root_device()))
            {
                // parse "raster.ini" for raster games
                if (device.screen_type() == screen_type_enum.SCREEN_TYPE_RASTER)
                {
                    parse_one_ini(options, "raster", (int)OPTION_PRIORITY.OPTION_PRIORITY_SCREEN_INI, ref error_stream);
                    break;
                }
                // parse "vector.ini" for vector games
                if (device.screen_type() == screen_type_enum.SCREEN_TYPE_VECTOR)
                {
                    parse_one_ini(options, "vector", (int)OPTION_PRIORITY.OPTION_PRIORITY_SCREEN_INI, ref error_stream);
                    break;
                }
                // parse "lcd.ini" for lcd games
                if (device.screen_type() == screen_type_enum.SCREEN_TYPE_LCD)
                {
                    parse_one_ini(options, "lcd", (int)OPTION_PRIORITY.OPTION_PRIORITY_SCREEN_INI, ref error_stream);
                    break;
                }
            }

            // next parse "source/<sourcefile>.ini"
            string sourcename = global.core_filename_extract_base(cursystem.type.source(), true).Insert(0, "source" + osdcore_global.PATH_SEPARATOR);
            parse_one_ini(options, sourcename, (int)OPTION_PRIORITY.OPTION_PRIORITY_SOURCE_INI, ref error_stream);

            // then parse the grandparent, parent, and system-specific INIs
            int parent = driver_list.clone(cursystem);
            int gparent = (parent != -1) ? driver_list.clone((UInt32)parent) : -1;
            if (gparent != -1)
                parse_one_ini(options, driver_list.driver((UInt32)gparent).name, (int)OPTION_PRIORITY.OPTION_PRIORITY_GPARENT_INI, ref error_stream);
            if (parent != -1)
                parse_one_ini(options, driver_list.driver((UInt32)parent).name, (int)OPTION_PRIORITY.OPTION_PRIORITY_PARENT_INI, ref error_stream);

            parse_one_ini(options, cursystem.name, (int)OPTION_PRIORITY.OPTION_PRIORITY_DRIVER_INI, ref error_stream);
        }


        //-------------------------------------------------
        //  system - return a pointer to the specified
        //  system driver, or nullptr if no match
        //-------------------------------------------------
        public static game_driver system(emu_options options)
        {
            int index = driver_list.find(global.core_filename_extract_base(options.system_name(), true).c_str());
            return (index != -1) ? driver_list.driver((UInt32)index) : null;
        }


        //-------------------------------------------------
        //  populate_hashpath_from_args_and_inis
        //-------------------------------------------------
        public static void populate_hashpath_from_args_and_inis(emu_options options, std_vector<string> args)
        {
            // The existence of this function comes from the fact that for softlist options to be properly
            // evaluated, we need to have the hashpath variable set.  The problem is that the hashpath may
            // be set anywhere on the command line, but also in any of the myriad INI files that we parse, some
            // of which may be system specific (e.g. - nes.ini) or otherwise influenced by the system (e.g. - vector.ini)
            //
            // I think that it is terrible that we have to do a completely independent pass on the command line and every
            // argument simply because any one of these things might be setting - hashpath.Unless we invest the effort in
            // building some sort of "late binding" apparatus for options(e.g. - delay evaluation of softlist options until
            // we've scoured all INIs for hashpath) that can completely straddle the command line and the INI worlds, doing
            // this is the best that we can do IMO.

            // parse the command line
            emu_options temp_options = new emu_options(emu_options.option_support.GENERAL_AND_SYSTEM);

            // pick up whatever changes the osd did to the default inipath
            temp_options.set_default_value(emu_options.OPTION_INIPATH, options.ini_path());

            try
            {
                temp_options.parse_command_line(args, emu_options.OPTION_PRIORITY_CMDLINE, true);
            }
            catch (options_exception)
            {
                // Something is very long; we have bigger problems than -hashpath possibly
                // being in never-never land.  Punt and let the main code fail
                return;
            }

            // if we have an auxillary verb, hashpath is irrelevant
            if (!temp_options.command().empty())
                return;

            // read INI files
            if (temp_options.read_config())
            {
                string error_stream;  //std::ostringstream error_stream;
                parse_standard_inis(temp_options, out error_stream);
            }

            // and fish out hashpath
            var entry = temp_options.get_entry(emu_options.OPTION_HASHPATH);
            if (entry != null)
            {
                try
                {
                    options.set_value(emu_options.OPTION_HASHPATH, entry.value(), entry.priority());
                }
                catch (options_exception)
                {
                }
            }
        }


        // INI parsing helper
        //-------------------------------------------------
        //  parse_one_ini - parse a single INI file
        //-------------------------------------------------
        static void parse_one_ini(emu_options options, string basename, int priority, ref string error_stream)
        {
            // don't parse if it has been disabled
            if (!options.read_config())
                return;

            // open the file; if we fail, that's ok
            emu_file file = new emu_file(options.ini_path(), osdcore_global.OPEN_FLAG_READ);
            global.osd_printf_verbose("Attempting load of {0}.ini\n", basename);
            osd_file.error filerr = file.open(basename, ".ini");
            if (filerr != osd_file.error.NONE)
                return;

            // parse the file
            global.osd_printf_verbose("Parsing {0}.ini\n", basename);
            try
            {
                options.parse_ini_file(file.core_file_get(), priority, priority < (int)OPTION_PRIORITY.OPTION_PRIORITY_DRIVER_INI, false);
            }
            catch (options_exception ex)
            {
                if (error_stream != null)
                    error_stream += string.Format("While parsing {0}:\n{1}\n", file.fullpath(), ex.message());
                return;
            }
        }
    }
}
