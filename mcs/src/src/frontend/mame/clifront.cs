// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using image_interface_enumerator = mame.device_interface_enumerator<mame.device_image_interface>;  //typedef device_interface_enumerator<device_image_interface> image_interface_enumerator;
using int64_t = System.Int64;
using samples_device_enumerator = mame.device_type_enumerator<mame.samples_device>;  //typedef device_type_enumerator<samples_device> samples_device_enumerator;
using slot_interface_enumerator = mame.device_interface_enumerator<mame.device_slot_interface>;  //typedef device_interface_enumerator<device_slot_interface> slot_interface_enumerator;
using uint32_t = System.UInt32;


namespace mame
{
    // cli_frontend handles command-line processing and emulator execution
    public class cli_frontend : global_object
    {
        //**************************************************************************
        //  CONSTANTS
        //**************************************************************************

        // core commands
        const string CLICOMMAND_HELP                = "help";
        const string CLICOMMAND_VALIDATE            = "validate";

        // configuration commands
        const string CLICOMMAND_CREATECONFIG        = "createconfig";
        const string CLICOMMAND_SHOWCONFIG          = "showconfig";
        const string CLICOMMAND_SHOWUSAGE           = "showusage";

        // frontend commands
        const string CLICOMMAND_LISTXML             = "listxml";
        const string CLICOMMAND_LISTFULL            = "listfull";
        const string CLICOMMAND_LISTSOURCE          = "listsource";
        const string CLICOMMAND_LISTCLONES          = "listclones";
        const string CLICOMMAND_LISTBROTHERS        = "listbrothers";
        const string CLICOMMAND_LISTCRC             = "listcrc";
        const string CLICOMMAND_LISTROMS            = "listroms";
        const string CLICOMMAND_LISTSAMPLES         = "listsamples";
        const string CLICOMMAND_VERIFYROMS          = "verifyroms";
        const string CLICOMMAND_VERIFYSAMPLES       = "verifysamples";
        const string CLICOMMAND_ROMIDENT            = "romident";
        const string CLICOMMAND_LISTDEVICES         = "listdevices";
        const string CLICOMMAND_LISTSLOTS           = "listslots";
        const string CLICOMMAND_LISTMEDIA           = "listmedia";
        const string CLICOMMAND_LISTSOFTWARE        = "listsoftware";
        const string CLICOMMAND_VERIFYSOFTWARE      = "verifysoftware";
        const string CLICOMMAND_GETSOFTLIST         = "getsoftlist";
        const string CLICOMMAND_VERIFYSOFTLIST      = "verifysoftlist";
        const string CLICOMMAND_VERSION             = "version";

        // command options
        const string CLIOPTION_DTD                  = "dtd";


        //**************************************************************************
        //  COMMAND-LINE OPTIONS
        //**************************************************************************

        static readonly options_entry [] cli_option_entries = new options_entry []
        {
            /* core commands */
            new options_entry(null,                                 null,   OPTION_HEADER,     "CORE COMMANDS"),
            new options_entry(CLICOMMAND_HELP           + ";h;?",   "0",    OPTION_COMMAND,    "show help message"),
            new options_entry(CLICOMMAND_VALIDATE       + ";valid", "0",    OPTION_COMMAND,    "perform validation on system drivers and devices"),

            /* configuration commands */
            new options_entry(null,                                 null,   OPTION_HEADER,     "CONFIGURATION COMMANDS"),
            new options_entry(CLICOMMAND_CREATECONFIG   + ";cc",    "0",    OPTION_COMMAND,    "create the default configuration file"),
            new options_entry(CLICOMMAND_SHOWCONFIG     + ";sc",    "0",    OPTION_COMMAND,    "display running parameters"),
            new options_entry(CLICOMMAND_SHOWUSAGE      + ";su",    "0",    OPTION_COMMAND,    "show this help"),

            /* frontend commands */
            new options_entry(null,                                 null,   OPTION_HEADER,     "FRONTEND COMMANDS"),
            new options_entry(CLICOMMAND_LISTXML        + ";lx",    "0",    OPTION_COMMAND,    "all available info on driver in XML format"),
            new options_entry(CLICOMMAND_LISTFULL       + ";ll",    "0",    OPTION_COMMAND,    "short name, full name"),
            new options_entry(CLICOMMAND_LISTSOURCE     + ";ls",    "0",    OPTION_COMMAND,    "driver sourcefile"),
            new options_entry(CLICOMMAND_LISTCLONES     + ";lc",    "0",    OPTION_COMMAND,    "show clones"),
            new options_entry(CLICOMMAND_LISTBROTHERS   + ";lb",    "0",    OPTION_COMMAND,    "show \"brothers\", or other drivers from same sourcefile"),
            new options_entry(CLICOMMAND_LISTCRC,                   "0",    OPTION_COMMAND,    "CRC-32s"),
            new options_entry(CLICOMMAND_LISTROMS       + ";lr",    "0",    OPTION_COMMAND,    "list required ROMs for a driver"),
            new options_entry(CLICOMMAND_LISTSAMPLES,               "0",    OPTION_COMMAND,    "list optional samples for a driver"),
            new options_entry(CLICOMMAND_VERIFYROMS,                "0",    OPTION_COMMAND,    "report romsets that have problems"),
            new options_entry(CLICOMMAND_VERIFYSAMPLES,             "0",    OPTION_COMMAND,    "report samplesets that have problems"),
            new options_entry(CLICOMMAND_ROMIDENT,                  "0",    OPTION_COMMAND,    "compare files with known MAME ROMs"),
            new options_entry(CLICOMMAND_LISTDEVICES    + ";ld",    "0",    OPTION_COMMAND,    "list available devices"),
            new options_entry(CLICOMMAND_LISTSLOTS      + ";lslot", "0",    OPTION_COMMAND,    "list available slots and slot devices"),
            new options_entry(CLICOMMAND_LISTMEDIA      + ";lm",    "0",    OPTION_COMMAND,    "list available media for the system"),
            new options_entry(CLICOMMAND_LISTSOFTWARE   + ";lsoft", "0",    OPTION_COMMAND,    "list known software for the system"),
            new options_entry(CLICOMMAND_VERIFYSOFTWARE + ";vsoft", "0",    OPTION_COMMAND,    "verify known software for the system"),
            new options_entry(CLICOMMAND_GETSOFTLIST    + ";glist", "0",    OPTION_COMMAND,    "retrieve software list by name"),
            new options_entry(CLICOMMAND_VERIFYSOFTLIST + ";vlist", "0",    OPTION_COMMAND,    "verify software list by name"),
            new options_entry(CLICOMMAND_VERSION,                   "0",    OPTION_COMMAND,    "get MAME version"),

            new options_entry(null,                                 null,   OPTION_HEADER,     "FRONTEND COMMAND OPTIONS"),
            new options_entry(CLIOPTION_DTD,                        "1",    OPTION_BOOLEAN,    "include DTD in XML output"),
            new options_entry(null),
        };


        struct info_command_struct
        {
            string option;
            int min_args;
            int max_args;
            Action<std.vector<string>> function;  //void (cli_frontend::*function)(const std::vector<std::string> &args);
            string usage;
        }


        //static const char s_softlist_xml_dtd[];


        // internal state
        emu_options m_options;
        osd_interface m_osd;
        int m_result;


        // construction/destruction
        //-------------------------------------------------
        //  cli_frontend - constructor
        //-------------------------------------------------
        public cli_frontend(emu_options options, osd_interface osd)
        {
            m_options = options;
            m_osd = osd;
            m_result = EMU_ERR_NONE;


            m_options.add_entries(cli_option_entries);
        }


        // execute based on the incoming argc/argv
        //-------------------------------------------------
        //  execute - execute a game via the standard
        //  command line interface
        //-------------------------------------------------
        public int execute(std.vector<string> args)
        {
            // wrap the core execution in a try/catch to field all fatal errors
            m_result = EMU_ERR_NONE;
            mame_machine_manager manager = mame_machine_manager.instance(m_options, m_osd);

            //try
            {
                start_execution(manager, args);
            }

            //throw new emu_unimplemented();
#if false
            // handle exceptions of various types
            catch (emu_fatalerror &fatal)
            {
            }
#endif

            util.archive_file.cache_clear();
            manager = null;  // global_free(manager);

            return m_result;
        }


        // direct access to the command operations

        //-------------------------------------------------
        //  listxml - output the XML data for one or more
        //  games
        //-------------------------------------------------
        void listxml(std.vector<string> args)
        {
            // create the XML and print it to stdout
            info_xml_creator creator = new info_xml_creator(m_options, m_options.bool_value(CLIOPTION_DTD));
            creator.output(null, args);  //stdout);
        }


        //-------------------------------------------------
        //  listfull - output the name and description of
        //  one or more games
        //-------------------------------------------------

        delegate void listfull_list_system_name(device_type type, bool first);

        void listfull(std.vector<string> args)
        {
            listfull_list_system_name list_system_name = (type, first) =>
            {
                // print the header
                if (first)
                    osd_printf_info("Name:             Description:\n");

                osd_printf_info("{0} \"{1}\"\n", type.shortname(), type.fullname());
            };

            apply_action(
                    args,
                    (drivlist, first) => { list_system_name(drivlist.driver().type, first); },
                    (type, first) => { list_system_name(type, first); });
        }


        //-------------------------------------------------
        //  listsource - output the name and source
        //  filename of one or more games
        //-------------------------------------------------

        delegate void listsource_list_system_source(device_type type);

        void listsource(std.vector<string> args)
        {
            listsource_list_system_source list_system_source = (type) => { osd_printf_info("{0} {1}\n", type.shortname(), g.core_filename_extract_base(type.source())); };

            apply_action(
                    args,
                    (drivlist, first) => { list_system_source(drivlist.driver().type); },
                    (type, first) => { list_system_source(type); });

        }


        //-------------------------------------------------
        //  listclones - output the name and parent of all
        //  clones matching the given pattern
        //-------------------------------------------------
        void listclones(std.vector<string> args)
        {
            string gamename = args.empty() ? null : args[0];

            // start with a filtered list of drivers
            driver_enumerator drivlist = new driver_enumerator(m_options, gamename);
            int original_count = (int)drivlist.count();

            // iterate through the remaining ones to see if their parent matches
            while (drivlist.next_excluded())
            {
                // if we have a non-bios clone and it matches, keep it
                int clone_of = drivlist.clone();
                if (clone_of >= 0 && ((UInt64)driver_list.driver(clone_of).flags & MACHINE_IS_BIOS_ROOT) == 0)
                {
                    if (driver_list.matches(gamename, driver_list.driver(clone_of).name))
                        drivlist.include();
                }
            }

            // return an error if none found
            if (drivlist.count() == 0)
            {
                // see if we match but just weren't a clone
                if (original_count == 0)
                    throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", gamename);
                else
                    osd_printf_info("Found {0} match(es) for '{1}' but none were clones\n", drivlist.count(), gamename);

                return;
            }

            // print the header
            osd_printf_info("Name:            Clone of:\n");

            // iterate through drivers and output the info
            drivlist.reset();
            while (drivlist.next())
            {
                int clone_of = drivlist.clone();
                if (clone_of >= 0 && ((UInt64)driver_list.driver(clone_of).flags & MACHINE_IS_BIOS_ROOT) == 0)
                    osd_printf_info("{0} {1}\n", drivlist.driver().name, driver_list.driver(clone_of).name);  // %-16s %-8s\n
            }
        }


        //-------------------------------------------------
        //  listbrothers - for each matching game, output
        //  the list of other games that share the same
        //  source file
        //-------------------------------------------------
        void listbrothers(std.vector<string> args)
        {
            string gamename = args.empty() ? null : args[0];

            // start with a filtered list of drivers; return an error if none found
            driver_enumerator initial_drivlist = new driver_enumerator(m_options, gamename);
            if (initial_drivlist.count() == 0)
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", gamename);

            // for the final list, start with an empty driver list
            driver_enumerator drivlist = new driver_enumerator(m_options);
            drivlist.exclude_all();

            // scan through the initially-selected drivers
            while (initial_drivlist.next())
            {
                // if we are already marked in the final list, we don't need to do anything
                if (drivlist.included(initial_drivlist.current()))
                    continue;

                // otherwise, walk excluded items in the final list and mark any that match
                drivlist.reset();
                while (drivlist.next_excluded())
                {
                    if (strcmp(drivlist.driver().type.source(), initial_drivlist.driver().type.source()) == 0)
                        drivlist.include();
                }
            }

            // print the header
            osd_printf_info("{0} {1} {2}\n", "Source file:", "Name:", "Parent:");  // %-20s %-16s %s\n

            // output the entries found
            drivlist.reset();
            while (drivlist.next())
            {
                int clone_of = drivlist.clone();
                if (clone_of != -1)
                    osd_printf_info("{0} {1} {2}\n", g.core_filename_extract_base(drivlist.driver().type.source()), drivlist.driver().name, (clone_of == -1 ? "" : driver_list.driver(clone_of).name));  // %-20s %-16s %-16s\n
                else
                    osd_printf_info("{0} {1}\n", g.core_filename_extract_base(drivlist.driver().type.source()), drivlist.driver().name);  // %-20s %s
            }
        }


        //-------------------------------------------------
        //  listcrc - output the CRC and name of all ROMs
        //  referenced by the emulator
        //-------------------------------------------------
        void listcrc(std.vector<string> args)
        {
            apply_device_action(
                    args,
                    (root, type, first) =>
                    {
                        foreach (device_t device in new device_enumerator(root))
                        {
                            var rom = device.rom_region();
                            for (int romOffset = 0; rom[romOffset] != null && !ROMENTRY_ISEND(rom[romOffset]); ++romOffset)  //for (tiny_rom_entry rom = device.rom_region(); rom != null && !ROMENTRY_ISEND(rom); ++rom)
                            {
                                if (ROMENTRY_ISFILE(rom[romOffset]))
                                {
                                    // if we have a CRC, display it
                                    uint32_t crc;
                                    if (new util.hash_collection(rom[romOffset].hashdata_).crc(out crc))
                                        osd_printf_info("{0} {1}\t{2}\t{3}\n", crc, rom[romOffset].name_, device.shortname(), device.name());  //"%08x %-32s\t%-16s\t%s\n"
                                }
                            }
                        }
                    });
        }


        //-------------------------------------------------
        //  listroms - output the list of ROMs referenced
        //  by matching systems/devices
        //-------------------------------------------------
        void listroms(std.vector<string> args)
        {
            apply_device_action(
                    args,
                    (root, type, first) =>
                    {
                        // space between items
                        if (!first)
                            osd_printf_info("\n");

                        // iterate through ROMs
                        bool hasroms = false;
                        foreach (device_t device in new device_enumerator(root))
                        {
                            for (var region = romload_global.rom_first_region(device); region != null; region = romload_global.rom_next_region(region))
                            {
                                for (var rom = romload_global.rom_first_file(region); rom != null; rom = romload_global.rom_next_file(rom))
                                {
                                    // print a header
                                    if (!hasroms)
                                        osd_printf_info(
                                            "ROMs required for {0} \"{1}\".\n" +
                                            "{2} {3} {4}\n",
                                            type, root.shortname(), "Name", "Size", "Checksum");
                                    hasroms = true;

                                    // accumulate the total length of all chunks
                                    int64_t length = -1;
                                    if (ROMREGION_ISROMDATA(region[0]))
                                        length = romload_global.rom_file_size(rom);

                                    // start with the name
                                    osd_printf_info("{0} ", rom[0].name());

                                    // output the length next
                                    if (length >= 0)
                                        osd_printf_info("{0}", length);
                                    else
                                        osd_printf_info("{0}", "");

                                    // output the hash data
                                    util.hash_collection hashes = new util.hash_collection(rom[0].hashdata());
                                    if (!hashes.flag(util.hash_collection.FLAG_NO_DUMP))
                                    {
                                        if (hashes.flag(util.hash_collection.FLAG_BAD_DUMP))
                                            osd_printf_info(" BAD");
                                        osd_printf_info(" {0}", hashes.macro_string());
                                    }
                                    else
                                    {
                                        osd_printf_info(" NO GOOD DUMP KNOWN");
                                    }

                                    // end with a CR
                                    osd_printf_info("\n");
                                }
                            }
                        }

                        if (!hasroms)
                            osd_printf_info("No ROMs required for {0} \"{1}\".\n", type, root.shortname());
                    });
        }


        //-------------------------------------------------
        //  listsamples - output the list of samples
        //  referenced by a given game or set of games
        //-------------------------------------------------
        void listsamples(std.vector<string> args)
        {
            string gamename = args.empty() ? null : args[0];

            // determine which drivers to output; return an error if none found
            driver_enumerator drivlist = new driver_enumerator(m_options, gamename);
            if (drivlist.count() == 0)
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", gamename);

            // iterate over drivers, looking for SAMPLES devices
            bool first = true;
            while (drivlist.next())
            {
                // see if we have samples
                samples_device_enumerator iter = new samples_device_enumerator(drivlist.config().root_device());
                if (iter.count() == 0)
                    continue;

                // print a header
                if (!first)
                    osd_printf_info("\n");
                first = false;
                osd_printf_info("Samples required for driver \"{0}\".\n", drivlist.driver().name);

                // iterate over samples devices and print the samples from each one
                foreach (samples_device device in iter)
                {
                    samples_iterator sampiter = new samples_iterator(device);
                    for (string samplename = sampiter.first(); samplename != null; samplename = sampiter.next())
                        osd_printf_info("{0}\n", samplename);
                }
            }
        }


        //-------------------------------------------------
        //  listdevices - output the list of devices
        //  referenced by a given game or set of games
        //-------------------------------------------------
        void listdevices(std.vector<string> args)
        {
            string gamename = args.empty() ? null : args[0];

            // determine which drivers to output; return an error if none found
            driver_enumerator drivlist = new driver_enumerator(m_options, gamename);
            if (drivlist.count() == 0)
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", gamename);

            // iterate over drivers, looking for SAMPLES devices
            bool first = true;
            while (drivlist.next())
            {
                // print a header
                if (!first)
                    osd_printf_info("\n");
                first = false;
                osd_printf_info("Driver {0} ({1}):\n", drivlist.driver().name, drivlist.driver().type.fullname());

                // build a list of devices
                std.vector<device_t> device_list = new std.vector<device_t>();
                foreach (device_t device in new device_enumerator(drivlist.config().root_device()))
                    device_list.push_back(device);

                // sort them by tag
                //std::sort(device_list.begin(), device_list.end(), /*[]*/(device_t *dev1, device_t *dev2) =>{
                //    // end of string < ':' < '0'
                //    const char *tag1 = dev1->tag();
                //    const char *tag2 = dev2->tag();
                //    while (*tag1 == *tag2 && *tag1 != '\0' && *tag2 != '\0')
                //    {
                //        tag1++;
                //        tag2++;
                //    }
                //    return (*tag1 == ':' ? ' ' : *tag1) < (*tag2 == ':' ? ' ' : *tag2);
                //});
                device_list.Sort((dev1, dev2) => { return strcmp(dev1.tag(), dev2.tag()); });

                // dump the results
                for (int index = 0; index < device_list.size(); index++)
                {
                    device_t device = device_list[index];

                    // extract the tag, stripping the leading colon
                    string tag = device.tag();
                    int tagIdx = 0;  //const char *tag = device->tag();
                    if (tag[tagIdx] == ':')
                        tagIdx++;

                    // determine the depth
                    int depth = 1;
                    if (tag[tagIdx] == 0)
                    {
                        tag = "<root>";
                        depth = 0;
                    }
                    else
                    {
                        for (int cIdx = tagIdx; cIdx < tag.Length; cIdx++)  //for (char *c = tag; *c != 0; c++)
                        {
                            if (tag[cIdx] == ':')
                            {
                                tag = tag.Substring(cIdx + 1);
                                depth++;
                            }
                        }
                    }
                    osd_printf_info("   {0}{1} {2} {3}", depth * 2, "", 30 - depth * 2, tag, device.name());  //   %*s%-*s %s

                    // add more information
                    UInt32 clock = device.clock();
                    if (clock >= 1000000000)
                        osd_printf_info(" @ {0}.{1} GHz\n", clock / 1000000000, (clock / 10000000) % 100);  //  @ %d.%02d GHz\n
                    else if (clock >= 1000000)
                        osd_printf_info(" @ {0}.{2} MHz\n", clock / 1000000, (clock / 10000) % 100);  //  @ %d.%02d MHz\n
                    else if (clock >= 1000)
                        osd_printf_info(" @ {0}.{3} kHz\n", clock / 1000, (clock / 10) % 100);  //  @ %d.%02d kHz\n
                    else if (clock > 0)
                        osd_printf_info(" @ {0} Hz\n", clock);
                    else
                        osd_printf_info("\n");
                }
            }
        }


        //-------------------------------------------------
        //  listslots - output the list of slot devices
        //  referenced by a given game or set of games
        //-------------------------------------------------
        void listslots(std.vector<string> args)
        {
            string gamename = args.empty() ? null : args[0];

            // determine which drivers to output; return an error if none found
            driver_enumerator drivlist = new driver_enumerator(m_options, gamename);
            if (drivlist.count() == 0)
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", gamename);

            // print header
            osd_printf_info("{0} {1} {2} {3}\n", "SYSTEM", "SLOT NAME", "SLOT OPTIONS", "SLOT DEVICE NAME");  // %-16s %-16s %-16s %s\n
            osd_printf_info("{0} {1} {2} {3}\n", new string('-', 16), new string('-', 16), new string('-', 16), new string('-', 28));

            // iterate over drivers
            while (drivlist.next())
            {
                // iterate
                bool first = true;
                foreach (device_slot_interface slot in new slot_interface_enumerator(drivlist.config().root_device()))
                {
                    if (slot.fixed_())
                        continue;

                    // build a list of user-selectable options
                    std.vector<device_slot_interface.slot_option> option_list = new std.vector<device_slot_interface.slot_option>();
                    foreach (var option in slot.option_list())
                    {
                        if (option.Value.selectable())
                            option_list.push_back(option.Value);
                    }

                    // sort them by name
                    //std::sort(option_list.begin(), option_list.end(), [](device_slot_interface::slot_option const *opt1, device_slot_interface::slot_option const *opt2) {
                    //    return strcmp(opt1->name(), opt2->name()) < 0;
                    //});
                    option_list.Sort((opt1, opt2) => { return strcmp(opt1.name(), opt2.name()); });

                    // output the line, up to the list of extensions
                    osd_printf_info("{0}{1}   ", first ? drivlist.driver().name : "", slot.device().tag().Remove(0, 1));  //+1);  // %-16s%-16s   

                    bool first_option = true;

                    // get the options and print them
                    foreach (device_slot_interface.slot_option opt in option_list)
                    {
                        if (first_option)
                            osd_printf_info("{0} {1}\n", opt.name(), opt.devtype().fullname());  // %-16s %s\n
                        else
                            osd_printf_info("{0}   {1} {2}\n", "", opt.name(), opt.devtype().fullname());  // %-34s   %-16s %s\n

                        first_option = false;
                    }
                    if (first_option)
                        osd_printf_info("{0} {1}\n", "[none]","No options available");  // "%-16s %s\n"

                    // end the line
                    osd_printf_info("\n");

                    first = false;
                }

                // if we didn't get any at all, just print a none line
                if (first)
                    osd_printf_info("{0}(none)\n", drivlist.driver().name);  // %-16s(none)\n
            }
        }


        //-------------------------------------------------
        //  listmedia - output the list of image devices
        //  referenced by a given game or set of games
        //-------------------------------------------------
        void listmedia(std.vector<string> args)
        {
            string gamename = args.empty() ? null : args[0];

            // determine which drivers to output; return an error if none found
            driver_enumerator drivlist = new driver_enumerator(m_options, gamename);
            if (drivlist.count() == 0)
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", gamename);

            // print header
            osd_printf_info("{0} {1} {2} {3}\n", "SYSTEM", "MEDIA NAME", "(brief)", "IMAGE FILE EXTENSIONS SUPPORTED");  // %-16s %-16s %-10s %s\n
            osd_printf_info("{0} {1}-{2} {3}\n", new string('-', 16), new string('-', 16), new string('-', 10), new string('-', 31));

            // iterate over drivers
            while (drivlist.next())
            {
                // iterate
                bool first = true;
                foreach (device_image_interface imagedev in new image_interface_enumerator(drivlist.config().root_device()))
                {
                    if (!imagedev.user_loadable())
                        continue;

                    // extract the shortname with parentheses
                    string paren_shortname = string.Format("({0})", imagedev.brief_instance_name());

                    // output the line, up to the list of extensions
                    osd_printf_info("{0}{1}{2}   ", drivlist.driver().name, imagedev.instance_name(), paren_shortname);  // %-16s%-16s%-10s   

                    // get the extensions and print them
                    string extensions = imagedev.file_extensions();
                    for (int start = 0, end = extensions.IndexOf(','); ; start = end + 1, end = extensions.IndexOf(',', start))
                    {
                        string curext = extensions.Substring(start, end == -1 ? extensions.Length - start : end - start);  // new astring(extensions, start, (end == -1) ? extensions.len() - start : end - start);
                        osd_printf_info(".{0}", curext);  // .%-5s
                        if (end == -1)
                            break;
                    }

                    // end the line
                    osd_printf_info("\n");
                    first = false;
                }

                // if we didn't get any at all, just print a none line
                if (first)
                    osd_printf_info("{0}(none)\n", drivlist.driver().name);  // %-16s(none)\n
            }
        }


        bool verifyroms_included(std.vector<string> args, ref std.vector<bool> matched, ref UInt32 matchcount, string name)
        {
            //auto const included = [&args, &matched, &matchcount] (char const *name) -> bool
            {
                if (args.empty())
                {
                    ++matchcount;
                    return true;
                }

                bool result = false;
                var it = matched;
                var itIdx = 0;
                foreach (string pat in args)
                {
                    if (g.core_strwildcmp(pat, name) == 0)
                    {
                        ++matchcount;
                        result = true;
                        it[itIdx] = true;
                    }
                    ++itIdx;
                }
                return result;
            }
        }


        //-------------------------------------------------
        //  verifyroms - verify the ROM sets of one or
        //  more games
        //-------------------------------------------------
        void verifyroms(std.vector<string> args)
        {
            bool iswild = ((1U != args.size()) || g.core_iswildstr(args[0]));
            std.vector<bool> matched = new std.vector<bool>(args.size());
            matched.resize(args.size(), false);
            UInt32 matchcount = 0;

            //auto const included = [&args, &matched, &matchcount] (char const *name) -> bool

            UInt32 correct = 0;
            UInt32 incorrect = 0;
            UInt32 notfound = 0;

            // iterate over drivers
            driver_enumerator drivlist = new driver_enumerator(m_options);
            media_auditor auditor = new media_auditor(drivlist);
            string summary_string;
            while (drivlist.next())
            {
                if (verifyroms_included(args, ref matched, ref matchcount, drivlist.driver().name))
                {
                    // audit the ROMs in this set
                    media_auditor.summary summary = auditor.audit_media(media_auditor.AUDIT_VALIDATE_FAST);

                    var clone_of = drivlist.clone();
                    print_summary(
                            auditor, summary, true,
                            "rom", drivlist.driver().name, (clone_of >= 0) ? driver_list.driver(clone_of).name : null,
                            ref correct, ref incorrect, ref notfound,
                            out summary_string);

                    // if it wasn't a wildcard, there can only be one
                    if (!iswild)
                        break;
                }
            }

            if (iswild || matchcount == 0)
            {
                machine_config config = new machine_config(___empty.driver____empty, m_options);
                using (machine_config.token tok = config.begin_configuration(config.root_device()))
                {
                    foreach (device_type type in device_global.registered_device_types)
                    {
                        if (verifyroms_included(args, ref matched, ref matchcount, type.shortname()))
                        {
                            // audit the ROMs in this set
                            device_t dev = config.device_add("_tmp", type, 0);
                            media_auditor.summary summary = auditor.audit_device(dev, media_auditor.AUDIT_VALIDATE_FAST);

                            print_summary(
                                    auditor, summary, false,
                                    "rom", dev.shortname(), null,
                                    ref correct, ref incorrect, ref notfound,
                                    out summary_string);
                            config.device_remove("_tmp");

                            // if it wasn't a wildcard, there can only be one
                            if (!iswild)
                                break;
                        }
                    }
                }
            }

            // clear out any cached files
            util.archive_file.cache_clear();

            // return an error if none found
            var it = matched.GetEnumerator();
            foreach (string pat in args)
            {
                if (!it.Current)
                    throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", pat);

                it.MoveNext();
            }

            if ((1U == args.size()) && (matchcount > 0) && (correct == 0) && (incorrect == 0))
            {
                // if we didn't get anything at all, display a generic end message
                if (notfound > 0)
                    throw new emu_fatalerror(EMU_ERR_MISSING_FILES, "romset \"{0}\" not found!\n", args[0]);
                else
                    throw new emu_fatalerror(EMU_ERR_MISSING_FILES, "romset \"{0}\" has no roms!\n", args[0]);
            }
            else
            {
                // otherwise, print a summary
                if (incorrect > 0)
                    throw new emu_fatalerror(EMU_ERR_MISSING_FILES, "{0} romsets found, {1} were OK.\n", correct + incorrect, correct);
                else
                    osd_printf_info("{0} romsets found, {1} were OK.\n", correct, correct);
            }
        }


        //-------------------------------------------------
        //  info_verifysamples - verify the sample sets of
        //  one or more games
        //-------------------------------------------------
        void verifysamples(std.vector<string> args)
        {
            string gamename = args.empty() ? "*" : args[0];

            // determine which drivers to output; return an error if none found
            driver_enumerator drivlist = new driver_enumerator(m_options, gamename);

            UInt32 correct = 0;
            UInt32 incorrect = 0;
            UInt32 notfound = 0;
            UInt32 matched = 0;

            // iterate over drivers
            media_auditor auditor = new media_auditor(drivlist);
            string summary_string;
            while (drivlist.next())
            {
                matched++;

                // audit the samples in this set
                media_auditor.summary summary = auditor.audit_samples();

                var clone_of = drivlist.clone();
                print_summary(
                        auditor, summary, false,
                        "sample", drivlist.driver().name, (clone_of >= 0) ? driver_list.driver(clone_of).name : null,
                        ref correct, ref incorrect, ref notfound,
                        out summary_string);
            }

            // clear out any cached files
            util.archive_file.cache_clear();

            // return an error if none found
            if (matched == 0)
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", gamename);

            // if we didn't get anything at all, display a generic end message
            if (matched > 0 && correct == 0 && incorrect == 0)
            {
                if (notfound > 0)
                    throw new emu_fatalerror(EMU_ERR_MISSING_FILES, "sampleset \"{0}\" not found!\n", gamename);
                else
                    throw new emu_fatalerror(EMU_ERR_MISSING_FILES, "sampleset \"{0}\" not required!\n", gamename);
            }

            // otherwise, print a summary
            else
            {
                if (incorrect > 0)
                    throw new emu_fatalerror(EMU_ERR_MISSING_FILES, "{0} samplesets found, {1} were OK.\n", correct + incorrect, correct);
                osd_printf_info("{0} samplesets found, {1} were OK.\n", correct, correct);
            }
        }


        /*-------------------------------------------------
            info_listsoftware - output the list of
            software supported by a given game or set of
            games
            TODO: Add all information read from the source files
            Possible improvement: use a sorted list for
                identifying duplicate lists.
        -------------------------------------------------*/
        void listsoftware(List<string> args)
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            verifysoftware - verify ROMs from the software
            list of the specified driver(s)
        -------------------------------------------------*/
        void verifysoftware(std.vector<string> args)
        {
            string gamename = args.empty() ? "*" : args[0];

            std.unordered_set<string> list_map = new std.unordered_set<string>();

            int correct = 0;
            int incorrect = 0;
            int notfound = 0;
            int matched = 0;
            int nrlists = 0;

            // determine which drivers to process; return an error if none found
            driver_enumerator drivlist = new driver_enumerator(m_options, gamename);
            if (drivlist.count() == 0)
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", gamename);

            throw new emu_unimplemented();
#if false
#endif

            // clear out any cached files
            util.archive_file.cache_clear();

            // return an error if none found
            if (matched == 0)
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", gamename);

            // if we didn't get anything at all, display a generic end message
            if (matched > 0 && correct == 0 && incorrect == 0)
            {
                throw new emu_fatalerror(EMU_ERR_MISSING_FILES, "romset \"{0}\" has no software entries defined!\n", gamename);
            }
            // otherwise, print a summary
            else
            {
                if (incorrect > 0)
                    throw new emu_fatalerror(EMU_ERR_MISSING_FILES, "{0} romsets found in {1} software lists, {2} were OK.\n", correct + incorrect, nrlists, correct);
                osd_printf_info("{0} romsets found in {1} software lists, {2} romsets were OK.\n", correct, nrlists, correct);
            }
        }


        /*-------------------------------------------------
            getsoftlist - retrieve software list by name
        -------------------------------------------------*/
        void getsoftlist(List<string> args)
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            verifysoftlist - verify software list by name
        -------------------------------------------------*/
        void verifysoftlist(std.vector<string> args)
        {
            string gamename = args.empty() ? "*" : args[0];

            std.unordered_set<string> list_map;
            int correct = 0;
            int incorrect = 0;
            int notfound = 0;
            int matched = 0;

            driver_enumerator drivlist = new driver_enumerator(m_options);
            media_auditor auditor = new media_auditor(drivlist);
            string summary_string;

            throw new emu_unimplemented();
#if false
#endif

            throw new emu_unimplemented();
#if false
            // clear out any cached files
            util::archive_file::cache_clear();
#endif

            // return an error if none found
            if (matched == 0)
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching software lists found for '{0}'", gamename);

            // if we didn't get anything at all, display a generic end message
            if (matched > 0 && correct == 0 && incorrect == 0)
            {
                throw new emu_fatalerror(EMU_ERR_MISSING_FILES, "no romsets found for software list \"{1}\"!\n", gamename);
            }
            // otherwise, print a summary
            else
            {
                if (incorrect > 0)
                    throw new emu_fatalerror(EMU_ERR_MISSING_FILES, "{0} romsets found in {1} software lists, {2} were OK.\n", correct + incorrect, matched, correct);
                osd_printf_info("{0} romsets found in {1} software lists, {2} romsets were OK.\n", correct, matched, correct);
            }
        }


        //-------------------------------------------------
        //  version - emit MAME version to stdout
        //-------------------------------------------------
        void version(std.vector<string> args)
        {
            osd_printf_info("{0}", emulator_info.get_build_version());
        }


        //-------------------------------------------------
        //  romident - identify ROMs by looking for
        //  matches in our internal database
        //-------------------------------------------------
        void romident(List<string> args)
        {
            throw new emu_unimplemented();
        }


        // internal helpers

        //-------------------------------------------------
        //  apply_action - apply action to matching
        //  systems/devices
        //-------------------------------------------------

        //template <typename T, typename U>
        void apply_action(std.vector<string> args, Action<driver_enumerator, bool> drvact, Action<device_type, bool> devact)  //void cli_frontend::apply_action(const std::vector<std::string> &args, T &&drvact, U &&devact)
        {
            bool iswild = (1U != args.size()) || g.core_iswildstr(args[0]);
            std.vector<bool> matched = new std.vector<bool>(args.size(), false);
            Func<string, bool> included = (name) =>
            {
                if (args.empty())
                    return true;

                bool result = false;
                int itIdx = 0;  //auto it = matched.begin();
                foreach (string pat in args)
                {
                    if (g.core_strwildcmp(pat, name) == 0)
                    {
                        result = true;
                        matched[itIdx] = true;  //*it = true;
                    }
                    ++itIdx;  //++it;
                }
                return result;
            };

            // determine which drivers to output
            driver_enumerator drivlist = new driver_enumerator(m_options);

            // iterate through matches
            bool first = true;
            while (drivlist.next())
            {
                if (included(drivlist.driver().name))
                {
                    drvact(drivlist, first);
                    first = false;

                    // if it wasn't a wildcard, there can only be one
                    if (!iswild)
                        break;
                }
            }

            if (iswild || first)
            {
                foreach (device_type type in device_global.registered_device_types)
                {
                    if (included(type.shortname()))
                    {
                        devact(type, first);
                        first = false;

                        // if it wasn't a wildcard, there can only be one
                        if (!iswild)
                            break;
                    }
                }
            }

            // return an error if none found
            {
                int itIdx = 0;  //auto it = matched.begin();
                foreach (string pat in args)
                {
                    if (!matched[itIdx])  //if (!*it)
                        throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", pat);

                    ++itIdx;  //++it;
                }
            }
        }


        //-------------------------------------------------
        //  apply_device_action - apply action to matching
        //  systems/devices
        //-------------------------------------------------

        //template <typename T>
        void apply_device_action(std.vector<string> args, Action<device_t, string, bool> action)  //void cli_frontend::apply_device_action(const std::vector<std::string> &args, T &&action)
        {
            machine_config config = new machine_config(___empty.driver____empty, m_options);
            machine_config.token tok = config.begin_configuration(config.root_device());
            apply_action(
                    args,
                    (drivlist, first) =>
                    {
                        action(drivlist.config().root_device(), "driver", first);
                    },
                    (type, first) =>
                    {
                        device_t dev = config.device_add("_tmp", type, 0);
                        action(dev, "device", first);
                        config.device_remove("_tmp");
                    });
        }


        //-------------------------------------------------
        //  execute_commands - execute various frontend
        //  commands
        //-------------------------------------------------
        void execute_commands(string exename)
        {
            // help?
            if (m_options.command() == cli_options.CLICOMMAND_HELP)
            {
                display_help(exename);
                return;
            }

            // showusage?
            if (m_options.command() == cli_options.CLICOMMAND_SHOWUSAGE)
            {
                osd_printf_info("Usage:  {0} [machine] [media] [software] [options]", exename);
                osd_printf_info("\n\nOptions:\n%s", m_options.output_help());
                return;
            }

            // validate?
            if (m_options.command() == cli_options.CLICOMMAND_VALIDATE)
            {
                if (m_options.command_arguments().size() > 1)
                {
                    osd_printf_error("Auxiliary verb -validate takes at most 1 argument\n");
                    return;
                }

                validity_checker valid = new validity_checker(m_options, false);
                string sysname = m_options.command_arguments().empty() ? null : m_options.command_arguments()[0];
                bool result = valid.check_all_matching(sysname);
                if (!result)
                    throw new emu_fatalerror(EMU_ERR_FAILED_VALIDITY, "Validity check failed ({0} errors, {1} warnings in total)\n", valid.errors(), valid.warnings());
                valid.Dispose();

                return;
            }

            // other commands need the INIs parsed
            string option_errors;
            mame_options.parse_standard_inis(m_options, out option_errors);
            if (option_errors.Length > 0)
                osd_printf_error("{0}\n", option_errors);

            // createconfig?
            if (m_options.command() == cli_options.CLICOMMAND_CREATECONFIG)
            {
                // attempt to open the output file
                emu_file file = new emu_file(OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
                if (file.open(emulator_info.get_configname() + ".ini") != osd_file.error.NONE)
                    throw new emu_fatalerror("Unable to create file {0}.ini\n", emulator_info.get_configname());

                // generate the updated INI
                file.puts(m_options.output_ini());
                file.close();

                ui_options ui_opts;
                emu_file file_ui = new emu_file(OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
                if (file_ui.open("ui.ini") != osd_file.error.NONE)
                    throw new emu_fatalerror("Unable to create file ui.ini\n");

                file_ui.close();

                throw new emu_unimplemented();
#if false
#endif
            }

            // showconfig?
            if (m_options.command() == cli_options.CLICOMMAND_SHOWCONFIG)
            {
                // print the INI text
                osd_printf_info("{0}\n", m_options.output_ini());
                return;
            }

            // all other commands call out to one of the info_commands helpers; first

            throw new emu_unimplemented();
#if false
#endif
        }


        //-------------------------------------------------
        //  display_help - display help to standard
        //  output
        //-------------------------------------------------
        void display_help(string exename)
        {
            osd_printf_info(
                    "{2} v{1}\n" +  //"%3$s v%2$s\n"
                    "{4}\n" +  //"%5$s\n"
                    "\n" +
                    "This software reproduces, more or less faithfully, the behaviour of a wide range\n" +
                    "of machines. But hardware is useless without software, so images of the ROMs and\n" +
                    "other media which run on that hardware are also required.\n" +
                    "\n" +
                    "Usage:  {0} [machine] [media] [software] [options]\n" +  //"Usage:  %1$s [machine] [media] [software] [options]\n"
                    "\n" +
                    "        {0} -showusage    for a list of options\n" +  //"        %1$s -showusage    for a list of options\n"
                    "        {0} -showconfig   to show current configuration in {3}.ini format\n" +  //"        %1$s -showconfig   to show current configuration in %4$s.ini format\n"
                    "        {0} -listmedia    for a full list of supported media\n" +  //"        %1$s -listmedia    for a full list of supported media\n"
                    "        {0} -createconfig to create a {3}.ini file\n" +  //"        %1$s -createconfig to create a %4$s.ini file\n"
                    "\n" +
                    "For usage instructions, please visit https://docs.mamedev.org/\n",
                    exename,
                    version_global.build_version,
                    emulator_info.get_appname(),
                    emulator_info.get_configname(),
                    emulator_info.get_copyright_info());
        }


        //void output_single_softlist(std::ostream &out, software_list_device &swlist);


        void start_execution(mame_machine_manager manager, std.vector<string> args)
        {
            string option_errors = "";

            // because softlist evaluation relies on hashpath being populated, we are going to go through
            // a special step to force it to be evaluated
            mame_options.populate_hashpath_from_args_and_inis(m_options, args);

            // parse the command line, adding any system-specific options
            try
            {
                m_options.parse_command_line(args, emu_options.OPTION_PRIORITY_CMDLINE);
            }
            catch (options_warning_exception ex)
            {
                osd_printf_error("{0}", ex.message());
            }
            catch (options_exception ex)
            {
                // if we failed, check for no command and a system name first; in that case error on the name
                if (m_options.command().empty() && mame_options.system(m_options) == null && !m_options.attempted_system_name().empty())
                    throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "Unknown system '{0}'", m_options.attempted_system_name());

                // otherwise, error on the options
                throw new emu_fatalerror(global_object.EMU_ERR_INVALID_CONFIG, "{0}", ex.message());
            }

            m_osd.set_verbose(m_options.verbose());

            // determine the base name of the EXE
            string exename = g.core_filename_extract_base(args[0], true);

            // if we have a command, execute that
            if (!m_options.command().empty())
            {
                execute_commands(exename);
                return;
            }

            // read INI's, if appropriate
            if (m_options.read_config())
            {
                mame_options.parse_standard_inis(m_options, out option_errors);
                m_osd.set_verbose(m_options.verbose());
            }

            // otherwise, check for a valid system
            language_global.load_translation(m_options);

            manager.start_http_server();

            manager.start_luaengine();

            if (option_errors.Length > 0)  //if (option_errors.tellp() > 0)
                osd_printf_error("Error in command line:\n{0}\n", g.strtrimspace(option_errors));

            // if we can't find it, give an appropriate error
            game_driver system = mame_options.system(m_options);
            if (system == null && !string.IsNullOrEmpty(m_options.system_name()))
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "Unknown system '{0}'", m_options.system_name());

            // otherwise just run the game
            m_result = manager.execute();
        }


        //static const info_command_struct *find_command(const std::string &s);


        void print_summary(
                media_auditor auditor, media_auditor.summary summary, bool record_none_needed,
                string type, string name, string parent,
                ref UInt32 correct, ref UInt32 incorrect, ref UInt32 notfound,
                out string buffer)
        {
            buffer = "";

            if (summary == media_auditor.summary.NOTFOUND)
            {
                // if not found, count that and leave it at that
                ++notfound;
            }
            else if (record_none_needed || (summary != media_auditor.summary.NONE_NEEDED))
            {
                // output the summary of the audit
                buffer = "";
                //buffer.seekp(0);
                auditor.summarize(name, ref buffer);
                //buffer.put('\0');
                osd_printf_info("{0}", buffer);

                // output the name of the driver and its parent
                osd_printf_info("{0}set {1} ", type, name);
                if (parent != null)
                    osd_printf_info("[{0}] ", parent);

                // switch off of the result
                switch (summary)
                {
                case media_auditor.summary.INCORRECT:
                    osd_printf_info("is bad\n");
                    ++incorrect;
                    return;

                case media_auditor.summary.CORRECT:
                    osd_printf_info("is good\n");
                    ++correct;
                    return;

                case media_auditor.summary.BEST_AVAILABLE:
                case media_auditor.summary.NONE_NEEDED:
                    osd_printf_info("is best available\n");
                    ++correct;
                    return;

                case media_auditor.summary.NOTFOUND:
                    osd_printf_info("not found\n");
                    return;
                }

                assert(false);
                osd_printf_error("has unknown status ({0})\n", summary);
            }
        }
    }
}
