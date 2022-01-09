// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using size_t = System.UInt64;

using static mame.corefile_global;
using static mame.emucore_global;
using static mame.emuopts_global;
using static mame.mameopts_global;
using static mame.options_global;
using static mame.osdcore_global;
using static mame.osdfile_global;


namespace mame
{
    public static class mameopts_global
    {
        // option priorities
        //enum
        //{
        // command-line options are HIGH priority
        const int OPTION_PRIORITY_SUBCMD      = OPTION_PRIORITY_HIGH;
        public const int OPTION_PRIORITY_CMDLINE     = OPTION_PRIORITY_HIGH + 1;

        // INI-based options are NORMAL priority, in increasing order:
        public const int OPTION_PRIORITY_MAME_INI    = OPTION_PRIORITY_NORMAL + 1;
        public const int OPTION_PRIORITY_DEBUG_INI   = OPTION_PRIORITY_NORMAL + 2;
        public const int OPTION_PRIORITY_ORIENTATION_INI = OPTION_PRIORITY_NORMAL + 3;
        public const int OPTION_PRIORITY_SYSTYPE_INI = OPTION_PRIORITY_NORMAL + 4;
        public const int OPTION_PRIORITY_SCREEN_INI  = OPTION_PRIORITY_NORMAL + 5;
        public const int OPTION_PRIORITY_SOURCE_INI  = OPTION_PRIORITY_NORMAL + 6;
        public const int OPTION_PRIORITY_GPARENT_INI = OPTION_PRIORITY_NORMAL + 7;
        public const int OPTION_PRIORITY_PARENT_INI  = OPTION_PRIORITY_NORMAL + 8;
        public const int OPTION_PRIORITY_DRIVER_INI  = OPTION_PRIORITY_NORMAL + 9;
        public const int OPTION_PRIORITY_INI         = OPTION_PRIORITY_NORMAL + 10;
        //}
    }


    class mame_options
    {
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
            parse_one_ini(options, emulator_info.get_configname(), OPTION_PRIORITY_MAME_INI, ref error_stream);
            parse_one_ini(options, emulator_info.get_configname(), OPTION_PRIORITY_MAME_INI, ref error_stream);

            // debug mode: parse "debug.ini" as well
            if (options.debug())
                parse_one_ini(options, "debug", OPTION_PRIORITY_DEBUG_INI, ref error_stream);

            // if we have a valid system driver, parse system-specific INI files
            game_driver cursystem = driver == null ? system(options) : driver;
            if (cursystem == null)
                return;

            // parse "vertical.ini" or "horizont.ini"
            if (((int)cursystem.flags & ORIENTATION_SWAP_XY) != 0)
                parse_one_ini(options, "vertical", OPTION_PRIORITY_ORIENTATION_INI, ref error_stream);
            else
                parse_one_ini(options, "horizont", OPTION_PRIORITY_ORIENTATION_INI, ref error_stream);

            switch (cursystem.flags & machine_flags.type.MASK_TYPE)
            {
            case machine_flags.type.TYPE_ARCADE:
                parse_one_ini(options, "arcade", OPTION_PRIORITY_SYSTYPE_INI, ref error_stream);
                break;
            case machine_flags.type.TYPE_CONSOLE:
                parse_one_ini(options, "console", OPTION_PRIORITY_SYSTYPE_INI, ref error_stream);
                break;
            case machine_flags.type.TYPE_COMPUTER:
                parse_one_ini(options, "computer", OPTION_PRIORITY_SYSTYPE_INI, ref error_stream);
                break;
            case machine_flags.type.TYPE_OTHER:
                parse_one_ini(options, "othersys", OPTION_PRIORITY_SYSTYPE_INI, ref error_stream);
                break;
            default:
                break;
            }

            machine_config config = new machine_config(cursystem, options);
            foreach (screen_device device in new screen_device_enumerator(config.root_device()))
            {
                // parse "raster.ini" for raster games
                if (device.screen_type() == screen_type_enum.SCREEN_TYPE_RASTER)
                {
                    parse_one_ini(options, "raster", OPTION_PRIORITY_SCREEN_INI, ref error_stream);
                    break;
                }
                // parse "vector.ini" for vector games
                if (device.screen_type() == screen_type_enum.SCREEN_TYPE_VECTOR)
                {
                    parse_one_ini(options, "vector", OPTION_PRIORITY_SCREEN_INI, ref error_stream);
                    break;
                }
                // parse "lcd.ini" for lcd games
                if (device.screen_type() == screen_type_enum.SCREEN_TYPE_LCD)
                {
                    parse_one_ini(options, "lcd", OPTION_PRIORITY_SCREEN_INI, ref error_stream);
                    break;
                }
            }

            // next parse "source/<sourcefile>.ini"
            string sourcename = core_filename_extract_base(cursystem.type.source(), true).Insert(0, "source" + PATH_SEPARATOR);
            parse_one_ini(options, sourcename, OPTION_PRIORITY_SOURCE_INI, ref error_stream);

            // then parse the grandparent, parent, and system-specific INIs
            int parent = driver_list.clone(cursystem);
            int gparent = (parent != -1) ? driver_list.clone((size_t)parent) : -1;
            if (gparent != -1)
                parse_one_ini(options, driver_list.driver((size_t)gparent).name, OPTION_PRIORITY_GPARENT_INI, ref error_stream);
            if (parent != -1)
                parse_one_ini(options, driver_list.driver((size_t)parent).name, OPTION_PRIORITY_PARENT_INI, ref error_stream);

            parse_one_ini(options, cursystem.name, OPTION_PRIORITY_DRIVER_INI, ref error_stream);
        }


        //-------------------------------------------------
        //  system - return a pointer to the specified
        //  system driver, or nullptr if no match
        //-------------------------------------------------
        public static game_driver system(emu_options options)
        {
            int index = driver_list.find(core_filename_extract_base(options.system_name(), true));
            return (index != -1) ? driver_list.driver((size_t)index) : null;
        }


        //-------------------------------------------------
        //  populate_hashpath_from_args_and_inis
        //-------------------------------------------------
        public static void populate_hashpath_from_args_and_inis(emu_options options, std.vector<string> args)
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
            temp_options.set_default_value(OPTION_INIPATH, options.ini_path());

            try
            {
                temp_options.parse_command_line(args, mameopts_global.OPTION_PRIORITY_CMDLINE, true);
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
            var entry = temp_options.get_entry(OPTION_HASHPATH);
            if (entry != null)
            {
                try
                {
                    options.set_value(OPTION_HASHPATH, entry.value(), entry.priority());
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
            emu_file file = new emu_file(options.ini_path(), OPEN_FLAG_READ);
            osd_printf_verbose("Attempting load of {0}.ini\n", basename);
            std.error_condition filerr = file.open(basename + ".ini");
            if (filerr)
                return;

            // parse the file
            osd_printf_verbose("Parsing {0}.ini\n", basename);
            try
            {
                options.parse_ini_file(file.core_file_get(), priority, priority < OPTION_PRIORITY_DRIVER_INI, false);
            }
            catch (options_exception ex)
            {
                if (error_stream != null)
                    error_stream += string.Format("While parsing {0}:\n{1}\n", file.fullpath(), ex.message());
                return;
            }
            finally
            {
                file.close();
            }
        }
    }
}
