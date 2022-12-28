// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using image_interface_enumerator = mame.device_interface_enumerator<mame.device_image_interface>;  //typedef device_interface_enumerator<device_image_interface> image_interface_enumerator;
using int64_t = System.Int64;
using size_t = System.UInt64;
using slot_interface_enumerator = mame.device_interface_enumerator<mame.device_slot_interface>;  //typedef device_interface_enumerator<device_slot_interface> slot_interface_enumerator;
using u64 = System.UInt64;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.corefile_global;
using static mame.corestr_global;
using static mame.cpp_global;
using static mame.device_global;
using static mame.emuopts_global;
using static mame.gamedrv_global;
using static mame.language_global;
using static mame.main_global;
using static mame.osdcore_global;
using static mame.osdfile_global;
using static mame.romload_global;
using static mame.version_global;


namespace mame
{
    // cli_frontend handles command-line processing and emulator execution
    public class cli_frontend
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
            new options_entry(null,                                 null,   core_options.option_type.HEADER,     "CORE COMMANDS"),
            new options_entry(CLICOMMAND_HELP           + ";h;?",   "0",    core_options.option_type.COMMAND,    "show help message"),
            new options_entry(CLICOMMAND_VALIDATE       + ";valid", "0",    core_options.option_type.COMMAND,    "perform validation on system drivers and devices"),

            /* configuration commands */
            new options_entry(null,                                 null,   core_options.option_type.HEADER,     "CONFIGURATION COMMANDS"),
            new options_entry(CLICOMMAND_CREATECONFIG   + ";cc",    "0",    core_options.option_type.COMMAND,    "create the default configuration file"),
            new options_entry(CLICOMMAND_SHOWCONFIG     + ";sc",    "0",    core_options.option_type.COMMAND,    "display running parameters"),
            new options_entry(CLICOMMAND_SHOWUSAGE      + ";su",    "0",    core_options.option_type.COMMAND,    "show this help"),

            /* frontend commands */
            new options_entry(null,                                 null,   core_options.option_type.HEADER,     "FRONTEND COMMANDS"),
            new options_entry(CLICOMMAND_LISTXML        + ";lx",    "0",    core_options.option_type.COMMAND,    "all available info on driver in XML format"),
            new options_entry(CLICOMMAND_LISTFULL       + ";ll",    "0",    core_options.option_type.COMMAND,    "short name, full name"),
            new options_entry(CLICOMMAND_LISTSOURCE     + ";ls",    "0",    core_options.option_type.COMMAND,    "driver sourcefile"),
            new options_entry(CLICOMMAND_LISTCLONES     + ";lc",    "0",    core_options.option_type.COMMAND,    "show clones"),
            new options_entry(CLICOMMAND_LISTBROTHERS   + ";lb",    "0",    core_options.option_type.COMMAND,    "show \"brothers\", or other drivers from same sourcefile"),
            new options_entry(CLICOMMAND_LISTCRC,                   "0",    core_options.option_type.COMMAND,    "CRC-32s"),
            new options_entry(CLICOMMAND_LISTROMS       + ";lr",    "0",    core_options.option_type.COMMAND,    "list required ROMs for a driver"),
            new options_entry(CLICOMMAND_LISTSAMPLES,               "0",    core_options.option_type.COMMAND,    "list optional samples for a driver"),
            new options_entry(CLICOMMAND_VERIFYROMS,                "0",    core_options.option_type.COMMAND,    "report romsets that have problems"),
            new options_entry(CLICOMMAND_VERIFYSAMPLES,             "0",    core_options.option_type.COMMAND,    "report samplesets that have problems"),
            new options_entry(CLICOMMAND_ROMIDENT,                  "0",    core_options.option_type.COMMAND,    "compare files with known MAME ROMs"),
            new options_entry(CLICOMMAND_LISTDEVICES    + ";ld",    "0",    core_options.option_type.COMMAND,    "list available devices"),
            new options_entry(CLICOMMAND_LISTSLOTS      + ";lslot", "0",    core_options.option_type.COMMAND,    "list available slots and slot devices"),
            new options_entry(CLICOMMAND_LISTMEDIA      + ";lm",    "0",    core_options.option_type.COMMAND,    "list available media for the system"),
            new options_entry(CLICOMMAND_LISTSOFTWARE   + ";lsoft", "0",    core_options.option_type.COMMAND,    "list known software for the system"),
            new options_entry(CLICOMMAND_VERIFYSOFTWARE + ";vsoft", "0",    core_options.option_type.COMMAND,    "verify known software for the system"),
            new options_entry(CLICOMMAND_GETSOFTLIST    + ";glist", "0",    core_options.option_type.COMMAND,    "retrieve software list by name"),
            new options_entry(CLICOMMAND_VERIFYSOFTLIST + ";vlist", "0",    core_options.option_type.COMMAND,    "verify software list by name"),
            new options_entry(CLICOMMAND_VERSION,                   "0",    core_options.option_type.COMMAND,    "get MAME version"),

            new options_entry(null,                                 null,   core_options.option_type.HEADER,     "FRONTEND COMMAND OPTIONS"),
            new options_entry(CLIOPTION_DTD,                        "1",    core_options.option_type.BOOLEAN,    "include DTD in XML output"),
            new options_entry(null),
        };


        class info_command_struct
        {
            public string option;
            public int min_args;
            public int max_args;
            public Action<std.vector<string>> function;  //void (cli_frontend::*function)(const std::vector<std::string> &args);
            public string usage;

            public info_command_struct(string option, int min_args, int max_args, Action<std.vector<string>> function, string usage)
            { this.option = option; this.min_args = min_args; this.max_args = max_args; this.function = function; this.usage = usage; }
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

            try
            {
                start_execution(manager, args);
            }
            // handle exceptions of various types
            catch (emu_fatalerror fatal)
            {
                osd_printf_error("%s\n", strtrimspace(fatal.what()));
                m_result = (fatal.exitcode() != 0) ? fatal.exitcode() : EMU_ERR_FATALERROR;

                // if a game was specified, wasn't a wildcard, and our error indicates this was the
                // reason for failure, offer some suggestions
                if (m_result == EMU_ERR_NO_SUCH_SYSTEM
                    && !m_options.attempted_system_name().empty()
                    && !core_iswildstr(m_options.attempted_system_name())
                    && mame_options.system(m_options) == null)
                {
                    // get the top 16 approximate matches
                    driver_enumerator drivlist = new driver_enumerator(m_options);
                    int [] matches = new int [16];
                    drivlist.find_approximate_matches(m_options.attempted_system_name(), matches.Length, out matches);

                    // work out how wide the titles need to be
                    int titlelen = 0;
                    foreach (int match in matches)
                    {
                        if (0 <= match)
                            titlelen = std.max(titlelen, (int)std.strlen(driver_list.driver((size_t)match).type.fullname()));
                    }

                    // print them out
                    osd_printf_error("\n\"{0}\" approximately matches the following\n" +
                            "supported machines (best match first):\n\n", m_options.attempted_system_name());
                    foreach (int match in matches)
                    {
                        if (0 <= match)
                        {
                            game_driver drv = driver_list.driver((size_t)match);
                            osd_printf_error("%{0,-18}-{1}({2}, {3})\n", drv.name, titlelen + 2, drv.type.fullname(), drv.manufacturer, drv.year);
                        }
                    }
                }
            }
            catch (tag_add_exception aex)
            {
                osd_printf_error("Tag '{0}' already exists in tagged map\n", aex.tag());
                m_result = EMU_ERR_FATALERROR;
            }
            catch (emu_exception)
            {
                osd_printf_error("Caught unhandled emulator exception\n");
                m_result = EMU_ERR_FATALERROR;
            }
            catch (Exception ex)
            {
                osd_printf_error("Caught unhandled {0} exception: {1}\n", ex.GetType(), ex.ToString());
                m_result = EMU_ERR_FATALERROR;
            }
            //catch (...)
            //{
            //    osd_printf_error("Caught unhandled exception\n");
            //    m_result = EMU_ERR_FATALERROR;
            //}

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
            string output = "";
            creator.output(ref output, args);  //stdout);
            osd_printf_info(output);
        }


        //-------------------------------------------------
        //  listfull - output the name and description of
        //  one or more games
        //-------------------------------------------------
        void listfull(std.vector<string> args)
        {
            Action<device_type, bool> list_system_name = (type, first) =>
            {
                // print the header
                if (first)
                    osd_printf_info("Name:             Description:\n");

                osd_printf_info("{0,-17} \"{1}\"\n", type.shortname(), type.fullname());
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
        void listsource(std.vector<string> args)
        {
            Action<device_type> list_system_source = (type) =>
            {
                string src = type.source();
                var prefix = src.find("src/mame/");
                if (npos == prefix)
                    prefix = src.find("src\\mame\\");
                if (npos != prefix)
                {
                    src = src.remove_prefix_(prefix + 9);
                }
                else
                {
                    var prefix2 = src.find("src/");
                    if (npos == prefix2)
                        prefix2 = src.find("src\\");
                    if (npos != prefix2)
                    {
                        src = src.remove_prefix_(prefix2 + 4);
                    }
                }

                osd_printf_info("{0,-16} {1}\n", type.shortname(), src);
            };

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
                if (clone_of >= 0 && ((u64)driver_list.driver((size_t)clone_of).flags & MACHINE_IS_BIOS_ROOT) == 0)
                {
                    if (driver_list.matches(gamename, driver_list.driver((size_t)clone_of).name))
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
                if (clone_of >= 0 && ((u64)driver_list.driver((size_t)clone_of).flags & MACHINE_IS_BIOS_ROOT) == 0)
                    osd_printf_info("{0,-16} {1}\n", drivlist.driver().name, driver_list.driver((size_t)clone_of).name);
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
                if (drivlist.included((size_t)initial_drivlist.current()))
                    continue;

                // otherwise, walk excluded items in the final list and mark any that match
                drivlist.reset();
                while (drivlist.next_excluded())
                {
                    if (std.strcmp(drivlist.driver().type.source(), initial_drivlist.driver().type.source()) == 0)
                        drivlist.include();
                }
            }

            // print the header
            osd_printf_info("{0,-20} {1,-16} {2}\n", "Source file:", "Name:", "Parent:");

            // output the entries found
            drivlist.reset();
            while (drivlist.next())
            {
                string src = drivlist.driver().type.source();
                var prefix = src.find("src/mame/");
                if (npos == prefix)
                    prefix = src.find("src\\mame\\");
                if (npos != prefix)
                    src = src.remove_prefix_(prefix + 9);
                int clone_of = drivlist.clone();
                if (clone_of != -1)
                    osd_printf_info("{0,-20} {1,-16} {2}\n", src, drivlist.driver().name, (clone_of == -1 ? "" : driver_list.driver((size_t)clone_of).name));
                else
                    osd_printf_info("{0,-20} {1}\n", src, drivlist.driver().name);
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
                            //OLD var rom = device.rom_region();
                            for (Pointer<tiny_rom_entry> rom = device.rom_region(); rom != null && !ROMENTRY_ISEND(rom); ++rom)  //for (tiny_rom_entry rom = device.rom_region(); rom != null && !ROMENTRY_ISEND(rom); ++rom)
                            {
                                if (ROMENTRY_ISFILE(rom))
                                {
                                    // if we have a CRC, display it
                                    uint32_t crc;
                                    if (new util.hash_collection(rom.op.hashdata_).crc(out crc))
                                        osd_printf_info("{0} {1}\t{2}\t{3}\n", crc, rom.op.name_, device.shortname(), device.name());  //"%08x %-32s\t%-16s\t%s\n"
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
                    std.list<Tuple<string, int64_t, string>> entries = new std.list<Tuple<string, int64_t, string>>();
                    std.set<string> devnames = new std.set<string>();
                    foreach (device_t device in new device_enumerator(root))
                    {
                        bool hasroms = false;
                        for (var region = rom_first_region(device); region != null; region = rom_next_region(region))
                        {
                            for (var rom = rom_first_file(region); rom != null; rom = rom_next_file(rom))
                            {
                                if (!hasroms)
                                {
                                    hasroms = true;
                                    if (device != root)
                                        devnames.insert(device.shortname());
                                }

                                // accumulate the total length of all chunks
                                int64_t length = -1;
                                if (ROMREGION_ISROMDATA(region))
                                    length = rom_file_size(rom);

                                entries.emplace_back(new Tuple<string, int64_t, string>(rom.op.name(), length, rom.op.hashdata()));
                            }
                        }
                    }

                    // print results
                    if (entries.empty())
                    {
                        osd_printf_info("No ROMs required for {0} \"{1}\".\n", type, root.shortname());
                    }
                    else
                    {
                        // print a header
                        osd_printf_info("ROMs required for {0} \"{1}\"", type, root.shortname());
                        if (!devnames.empty())
                        {
                            osd_printf_info(" (including device{0}", devnames.size() > 1 ? "s" : "");
                            bool first2 = true;
                            foreach (string devname in devnames)
                            {
                                if (first2)
                                    first2 = false;
                                else
                                    osd_printf_info(",");

                                osd_printf_info(" \"{0}\"", devname);
                            }
                            osd_printf_info(")");
                        }
                        osd_printf_info(".\n{0,-32} {1,10} {2}\n", "Name", "Size", "Checksum");

                        foreach (var entry in entries)
                        {
                            // start with the name
                            osd_printf_info("{0,-32} ", entry.Item1);

                            // output the length next
                            int64_t length = entry.Item2;
                            if (length >= 0)
                                osd_printf_info("{0,10}", (unsigned)(uint64_t)length);
                            else
                                osd_printf_info("{0,10}", "");

                            // output the hash data
                            util.hash_collection hashes = new util.hash_collection(entry.Item3);
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
                object [] iter = samples_device_enumerator_helper.get_samples_devices(drivlist.config().root_device());  //samples_device_enumerator iter(drivlist.config()->root_device());
                if (iter.Length == 0)  //if (iter.count() == 0)
                    continue;

                // print a header
                if (!first)
                    osd_printf_info("\n");
                first = false;
                osd_printf_info("Samples required for driver \"{0}\".\n", drivlist.driver().name);

                // iterate over samples devices and print the samples from each one
                foreach (object device in iter)  //foreach (samples_device device in iter)
                {
                    object sampiter = samples_device_enumerator_helper.get_samples_iterator(device);  //samples_iterator sampiter = new samples_iterator(device);
                    string [] iter_samplenames = samples_device_enumerator_helper.get_samplenames(sampiter);
                    foreach (var samplename in iter_samplenames)  //for (string samplename = sampiter.first(); samplename != null; samplename = sampiter.next())
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
                device_list.Sort((dev1, dev2) => { return std.strcmp(dev1.tag(), dev2.tag()); });

                // dump the results
                foreach (var device in device_list)
                {
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
                                tag = tag[(cIdx + 1)..];  //tag = c + 1;
                                depth++;
                            }
                        }
                    }
                    osd_printf_info("   {0}{1} {2} {3}", depth * 2, "", 30 - depth * 2, tag, device.name());  //   %*s%-*s %s

                    // add more information
                    uint32_t clock = device.clock();
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
            osd_printf_info("{0,-16} {1,-16} {2,-16} {3}\n", "SYSTEM", "SLOT NAME", "SLOT OPTIONS", "SLOT DEVICE NAME");
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
                    option_list.Sort((opt1, opt2) => { return std.strcmp(opt1.name(), opt2.name()); });

                    // output the line, up to the list of extensions
                    osd_printf_info("{0,-16} {1,-16} ", first ? drivlist.driver().name : "", slot.device().tag().Remove(0, 1));  //+1);

                    bool first_option = true;

                    // get the options and print them
                    foreach (device_slot_interface.slot_option opt in option_list)
                    {
                        if (first_option)
                            osd_printf_info("{0,-16} {1}\n", opt.name(), opt.devtype().fullname());
                        else
                            osd_printf_info("{0,-34}{1,-16} {2}\n", "", opt.name(), opt.devtype().fullname());

                        first_option = false;
                    }
                    if (first_option)
                        osd_printf_info("{0,-16} {1}\n", "[none]","No options available");

                    // end the line
                    osd_printf_info("\n");

                    first = false;
                }

                // if we didn't get any at all, just print a none line
                if (first)
                    osd_printf_info("{0,-16} (none)\n", drivlist.driver().name);
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
            osd_printf_info("{0,-16} {1,-16} {2,-10} {3}\n", "SYSTEM", "MEDIA NAME", "(brief)", "IMAGE FILE EXTENSIONS SUPPORTED");
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
                    osd_printf_info("{0,-16} {1,-16} {2,-10} ", drivlist.driver().name, imagedev.instance_name(), paren_shortname);

                    // get the extensions and print them
                    string extensions = imagedev.file_extensions();
                    for (int start = 0, end = extensions.IndexOf(','); ; start = end + 1, end = extensions.IndexOf(',', start))
                    {
                        string curext = extensions.Substring(start, end == -1 ? extensions.Length - start : end - start);  // new astring(extensions, start, (end == -1) ? extensions.len() - start : end - start);
                        osd_printf_info(".{0,-5}", curext);
                        if (end == -1)
                            break;
                    }

                    // end the line
                    osd_printf_info("\n");
                    first = false;
                }

                // if we didn't get any at all, just print a none line
                if (first)
                    osd_printf_info("{0,-16} (none)\n", drivlist.driver().name);
            }
        }


        //-------------------------------------------------
        //  verifyroms - verify the ROM sets of one or
        //  more games
        //-------------------------------------------------
        void verifyroms(std.vector<string> args)
        {
            bool iswild = ((1U != args.size()) || core_iswildstr(args[0]));
            std.vector<bool> matched = new std.vector<bool>(args.size());
            matched.resize(args.size(), false);
            unsigned matchcount = 0;

            Func<string, bool> included = (name) =>  //auto const included = [&args, &matched, &matchcount] (char const *name) -> bool
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
                    if (core_strwildcmp(pat, name) == 0)
                    {
                        ++matchcount;
                        result = true;
                        it[itIdx] = true;
                    }
                    ++itIdx;
                }
                return result;
            };

            unsigned correct = 0;
            unsigned incorrect = 0;
            unsigned notfound = 0;

            // iterate over drivers
            driver_enumerator drivlist = new driver_enumerator(m_options);
            media_auditor auditor = new media_auditor(drivlist);
            string summary_string;
            while (drivlist.next())
            {
                if (included(drivlist.driver().name))
                {
                    // audit the ROMs in this set
                    media_auditor.summary summary = auditor.audit_media(media_auditor.AUDIT_VALIDATE_FAST);

                    var clone_of = drivlist.clone();
                    print_summary(
                            auditor, summary, true,
                            "rom", drivlist.driver().name, (clone_of >= 0) ? driver_list.driver((size_t)clone_of).name : null,
                            ref correct, ref incorrect, ref notfound,
                            out summary_string);

                    // if it wasn't a wildcard, there can only be one
                    if (!iswild)
                        break;
                }
            }

            if (iswild || matchcount == 0)
            {
                machine_config config = new machine_config(___empty.driver____empty, m_options);  //machine_config config(GAME_NAME(___empty), m_options);
                using machine_config.token tok = config.begin_configuration(config.root_device());

                foreach (device_type type in registered_device_types)
                {
                    if (included(type.shortname()))
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

            // clear out any cached files
            util.archive_file.cache_clear();

            {
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

            unsigned correct = 0;
            unsigned incorrect = 0;
            unsigned notfound = 0;
            unsigned matched = 0;

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
                        "sample", drivlist.driver().name, (clone_of >= 0) ? driver_list.driver((size_t)clone_of).name : null,
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
        void listsoftware(std.vector<string> args)
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

            unsigned correct = 0;
            unsigned incorrect = 0;
            unsigned notfound = 0;
            unsigned matched = 0;
            unsigned nrlists = 0;

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
        void getsoftlist(std.vector<string> args)
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
            unsigned correct = 0;
            unsigned incorrect = 0;
            unsigned notfound = 0;
            unsigned matched = 0;

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
        void romident(std.vector<string> args)
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
            bool iswild = (1U != args.size()) || core_iswildstr(args[0]);
            std.vector<bool> matched = new std.vector<bool>(args.size(), false);
            Func<string, bool> included = (name) =>
            {
                if (args.empty())
                    return true;

                bool result = false;
                int itIdx = 0;  //auto it = matched.begin();
                foreach (string pat in args)
                {
                    if (core_strwildcmp(pat, name) == 0)
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
                foreach (device_type type in registered_device_types)
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
                osd_printf_info("\n\nOptions:\n{0}", m_options.output_help());
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

                using validity_checker valid = new validity_checker(m_options, false);

                string sysname = m_options.command_arguments().empty() ? null : m_options.command_arguments()[0];
                bool result = valid.check_all_matching(sysname);
                if (!result)
                    throw new emu_fatalerror(EMU_ERR_FAILED_VALIDITY, "Validity check failed ({0} errors, {1} warnings in total)\n", valid.errors(), valid.warnings());

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
                if (file.open(emulator_info.get_configname() + ".ini"))
                    throw new emu_fatalerror("Unable to create file {0}.ini\n", emulator_info.get_configname());

                // generate the updated INI
                file.puts(m_options.output_ini());
                file.close();

                ui_options ui_opts;
                emu_file file_ui = new emu_file(OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
                if (file_ui.open("ui.ini"))
                    throw new emu_fatalerror("Unable to create file ui.ini\n");

                file_ui.close();

                plugin_options plugin_opts = new plugin_options();
                path_iterator iter = new path_iterator(m_options.plugins_path());
                string pluginpath;
                while (iter.next(out pluginpath))
                    plugin_opts.scan_directory(m_osdcore.osd_subst_env(pluginpath), true);
                emu_file file_plugin = new emu_file(OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
                if (file_plugin.open("plugin.ini"))
                    throw new emu_fatalerror("Unable to create file plugin.ini\n");

                // generate the updated INI
                file_plugin.puts(plugin_opts.output_ini());

                return;
            }

            // showconfig?
            if (m_options.command() == cli_options.CLICOMMAND_SHOWCONFIG)
            {
                // print the INI text
                osd_printf_info("{0}\n", m_options.output_ini());
                return;
            }

            // all other commands call out to one of the info_commands helpers; first
            // find the command
            var info_command = find_command(m_options.command());
            if (info_command != null)
            {
                // validate argument count
                string error_message = null;
                if ((int)m_options.command_arguments().size() < info_command.min_args)
                    error_message = "Auxiliary verb -{0} requires at least {1} argument(s)\n";
                if ((info_command.max_args >= 0) && ((int)m_options.command_arguments().size() > info_command.max_args))
                    error_message = "Auxiliary verb -{0} takes at most {1} argument(s)\n";
                if (!string.IsNullOrEmpty(error_message))
                {
                    osd_printf_info(error_message, info_command.option, info_command.max_args);
                    osd_printf_info("\n");
                    osd_printf_info("Usage:  {0} -{1} {2}\n", exename, info_command.option, info_command.usage);
                    return;
                }

                // invoke the auxiliary command!
                info_command.function(m_options.command_arguments());
                return;
            }

            if (!m_osd.execute_command(m_options.command()))
                // if we get here, we don't know what has been requested
                throw new emu_fatalerror(EMU_ERR_INVALID_CONFIG, "Unknown command '{0}' specified", m_options.command());
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
                    build_version,
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
                m_options.parse_command_line(args, OPTION_PRIORITY_CMDLINE);
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
                throw new emu_fatalerror(EMU_ERR_INVALID_CONFIG, "{0}", ex.message());
            }

            m_osd.set_verbose(m_options.verbose());

            // determine the base name of the EXE
            string exename = core_filename_extract_base(args[0], true);

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
            load_translation(m_options);

            manager.start_http_server();

            manager.start_luaengine();

            if (option_errors.Length > 0)  //if (option_errors.tellp() > 0)
                osd_printf_error("Error in command line:\n{0}\n", strtrimspace(option_errors));

            // if we can't find it, give an appropriate error
            game_driver system = mame_options.system(m_options);
            if (system == null && !string.IsNullOrEmpty(m_options.system_name()))
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "Unknown system '{0}'", m_options.system_name());

            // otherwise just run the game
            m_result = manager.execute();
        }


        //-------------------------------------------------
        //  find_command
        //-------------------------------------------------
        info_command_struct find_command(string s)
        {
            info_command_struct [] s_info_commands = 
            {
                new info_command_struct( CLICOMMAND_LISTXML,           0, -1, listxml,          "[pattern] ..." ),
                new info_command_struct( CLICOMMAND_LISTFULL,          0, -1, listfull,         "[pattern] ..." ),
                new info_command_struct( CLICOMMAND_LISTSOURCE,        0, -1, listsource,       "[system name]" ),
                new info_command_struct( CLICOMMAND_LISTCLONES,        0,  1, listclones,       "[system name]" ),
                new info_command_struct( CLICOMMAND_LISTBROTHERS,      0,  1, listbrothers,     "[system name]" ),
                new info_command_struct( CLICOMMAND_LISTCRC,           0, -1, listcrc,          "[system name]" ),
                new info_command_struct( CLICOMMAND_LISTDEVICES,       0,  1, listdevices,      "[system name]" ),
                new info_command_struct( CLICOMMAND_LISTSLOTS,         0,  1, listslots,        "[system name]" ),
                new info_command_struct( CLICOMMAND_LISTROMS,          0, -1, listroms,         "[pattern] ..." ),
                new info_command_struct( CLICOMMAND_LISTSAMPLES,       0,  1, listsamples,      "[system name]" ),
                new info_command_struct( CLICOMMAND_VERIFYROMS,        0, -1, verifyroms,       "[pattern] ..." ),
                new info_command_struct( CLICOMMAND_VERIFYSAMPLES,     0,  1, verifysamples,    "[system name|*]" ),
                new info_command_struct( CLICOMMAND_LISTMEDIA,         0,  1, listmedia,        "[system name]" ),
                new info_command_struct( CLICOMMAND_LISTSOFTWARE,      0,  1, listsoftware,     "[system name]" ),
                new info_command_struct( CLICOMMAND_VERIFYSOFTWARE,    0,  1, verifysoftware,   "[system name|*]" ),
                new info_command_struct( CLICOMMAND_ROMIDENT,          1,  1, romident,         "(file or directory path)" ),
                new info_command_struct( CLICOMMAND_GETSOFTLIST,       0,  1, getsoftlist,      "[system name|*]" ),
                new info_command_struct( CLICOMMAND_VERIFYSOFTLIST,    0,  1, verifysoftlist,   "[system name|*]" ),
                new info_command_struct( CLICOMMAND_VERSION,           0,  0, version,          "" ),
            };

            foreach (var info_command in s_info_commands)
            {
                if (s == info_command.option)
                    return info_command;
            }

            return null;
        }


        void print_summary(
                media_auditor auditor, media_auditor.summary summary, bool record_none_needed,
                string type, string name, string parent,
                ref unsigned correct, ref unsigned incorrect, ref unsigned notfound,
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
