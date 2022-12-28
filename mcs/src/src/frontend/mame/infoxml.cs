// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_t_feature_type = mame.emu.detail.device_feature.type;  //using feature_type = emu::detail::device_feature::type;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using device_type_set = mame.std.set<mame.emu.detail.device_type_impl_base>;  //typedef std::set<std::add_pointer_t<device_type>, device_type_compare> device_type_set;
using execute_interface_enumerator = mame.device_interface_enumerator<mame.device_execute_interface>;  //typedef device_interface_enumerator<device_execute_interface> execute_interface_enumerator;
using image_interface_enumerator = mame.device_interface_enumerator<mame.device_image_interface>;  //typedef device_interface_enumerator<device_image_interface> image_interface_enumerator;
using int32_t = System.Int32;
using size_t = System.UInt64;
using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using slot_interface_enumerator = mame.device_interface_enumerator<mame.device_slot_interface>;  //typedef device_interface_enumerator<device_slot_interface> slot_interface_enumerator;
using software_list_device_enumerator = mame.device_type_enumerator<mame.software_list_device>;  //typedef device_type_enumerator<software_list_device> software_list_device_enumerator;
using sound_interface_enumerator = mame.device_interface_enumerator<mame.device_sound_interface>;  //typedef device_interface_enumerator<device_sound_interface> sound_interface_enumerator;
using speaker_device_enumerator = mame.device_type_enumerator<mame.speaker_device>;  //using speaker_device_enumerator = device_type_enumerator<speaker_device>;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.attotime_global;
using static mame.corestr_global;
using static mame.cpp_global;
using static mame.device_global;
using static mame.emucore_global;
using static mame.main_global;
using static mame.romload_global;
using static mame.util.xml;


namespace mame
{
    // helper class to putput
    class info_xml_creator
    {
        //class device_type_compare


        class device_filter
        {
            output_filter_delegate m_callback;  //const std::function<bool(const char *shortname, bool &done)> &  m_callback;
            bool m_done;


            public device_filter(output_filter_delegate callback)  //device_filter(const std::function<bool(const char *shortname, bool &done)> &callback)
            {
                m_callback = callback;
                m_done = false;
            }


            // methods
            //-------------------------------------------------
            //  device_filter::filter - apply the filter, if
            //  present
            //-------------------------------------------------
            public bool filter(string shortname)
            {
                return !m_done && (m_callback == null || m_callback(shortname, ref m_done));
            }

            // accessors
            public bool done() { return m_done; }
        }


        class filtered_driver_enumerator
        {
            driver_enumerator m_drivlist;
            device_filter m_devfilter;
            bool m_done;


            public filtered_driver_enumerator(driver_enumerator drivlist, device_filter devfilter)
            {
                m_drivlist = drivlist;
                m_devfilter = devfilter;
                m_done = false;
            }


            // methods
            public std.vector<game_driver> next(int count)  //std::vector<std::reference_wrapper<const game_driver>> next(int count);
            {
                std.vector<game_driver> results = new std.vector<game_driver>();  //std::vector<std::reference_wrapper<const game_driver>> results;
                while (!done() && (int)results.size() < count)
                {
                    if (!m_drivlist.next())
                    {
                        // at this point we are done enumerating through drivlist and it is no
                        // longer safe to call next(), so record that we're done
                        m_done = true;
                    }
                    else if (m_devfilter.filter(m_drivlist.driver().name))
                    {
                        game_driver driver = m_drivlist.driver();
                        results.push_back(driver);
                    }
                }

                return results;
            }

            // accessors
            public bool done() { return m_done || m_devfilter.done(); }
        }


        // internal state
        emu_options m_lookup_options;
        bool m_dtd;


        const string XML_ROOT = "mame";
        const string XML_TOP  = "machine";


        //**************************************************************************
        //  GLOBAL VARIABLES
        //**************************************************************************

        // DTD string describing the data
        static readonly string f_dtd_string =
                "<!DOCTYPE __XML_ROOT__ [\n" +
                "<!ELEMENT __XML_ROOT__ (__XML_TOP__+)>\n" +
                "\t<!ATTLIST __XML_ROOT__ build CDATA #IMPLIED>\n" +
                "\t<!ATTLIST __XML_ROOT__ debug (yes|no) \"no\">\n" +
                "\t<!ATTLIST __XML_ROOT__ mameconfig CDATA #REQUIRED>\n" +
                "\t<!ELEMENT __XML_TOP__ (description, year?, manufacturer?, biosset*, rom*, disk*, device_ref*, sample*, chip*, display*, sound?, input?, dipswitch*, configuration*, port*, adjuster*, driver?, feature*, device*, slot*, softwarelist*, ramoption*)>\n" +
                "\t\t<!ATTLIST __XML_TOP__ name CDATA #REQUIRED>\n" +
                "\t\t<!ATTLIST __XML_TOP__ sourcefile CDATA #IMPLIED>\n" +
                "\t\t<!ATTLIST __XML_TOP__ isbios (yes|no) \"no\">\n" +
                "\t\t<!ATTLIST __XML_TOP__ isdevice (yes|no) \"no\">\n" +
                "\t\t<!ATTLIST __XML_TOP__ ismechanical (yes|no) \"no\">\n" +
                "\t\t<!ATTLIST __XML_TOP__ runnable (yes|no) \"yes\">\n" +
                "\t\t<!ATTLIST __XML_TOP__ cloneof CDATA #IMPLIED>\n" +
                "\t\t<!ATTLIST __XML_TOP__ romof CDATA #IMPLIED>\n" +
                "\t\t<!ATTLIST __XML_TOP__ sampleof CDATA #IMPLIED>\n" +
                "\t\t<!ELEMENT description (#PCDATA)>\n" +
                "\t\t<!ELEMENT year (#PCDATA)>\n" +
                "\t\t<!ELEMENT manufacturer (#PCDATA)>\n" +
                "\t\t<!ELEMENT biosset EMPTY>\n" +
                "\t\t\t<!ATTLIST biosset name CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST biosset description CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST biosset default (yes|no) \"no\">\n" +
                "\t\t<!ELEMENT rom EMPTY>\n" +
                "\t\t\t<!ATTLIST rom name CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST rom bios CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST rom size CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST rom crc CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST rom sha1 CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST rom merge CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST rom region CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST rom offset CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST rom status (baddump|nodump|good) \"good\">\n" +
                "\t\t\t<!ATTLIST rom optional (yes|no) \"no\">\n" +
                "\t\t<!ELEMENT disk EMPTY>\n" +
                "\t\t\t<!ATTLIST disk name CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST disk sha1 CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST disk merge CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST disk region CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST disk index CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST disk writable (yes|no) \"no\">\n" +
                "\t\t\t<!ATTLIST disk status (baddump|nodump|good) \"good\">\n" +
                "\t\t\t<!ATTLIST disk optional (yes|no) \"no\">\n" +
                "\t\t<!ELEMENT device_ref EMPTY>\n" +
                "\t\t\t<!ATTLIST device_ref name CDATA #REQUIRED>\n" +
                "\t\t<!ELEMENT sample EMPTY>\n" +
                "\t\t\t<!ATTLIST sample name CDATA #REQUIRED>\n" +
                "\t\t<!ELEMENT chip EMPTY>\n" +
                "\t\t\t<!ATTLIST chip name CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST chip tag CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST chip type (cpu|audio) #REQUIRED>\n" +
                "\t\t\t<!ATTLIST chip clock CDATA #IMPLIED>\n" +
                "\t\t<!ELEMENT display EMPTY>\n" +
                "\t\t\t<!ATTLIST display tag CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST display type (raster|vector|lcd|svg|unknown) #REQUIRED>\n" +
                "\t\t\t<!ATTLIST display rotate (0|90|180|270) #IMPLIED>\n" +
                "\t\t\t<!ATTLIST display flipx (yes|no) \"no\">\n" +
                "\t\t\t<!ATTLIST display width CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST display height CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST display refresh CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST display pixclock CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST display htotal CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST display hbend CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST display hbstart CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST display vtotal CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST display vbend CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST display vbstart CDATA #IMPLIED>\n" +
                "\t\t<!ELEMENT sound EMPTY>\n" +
                "\t\t\t<!ATTLIST sound channels CDATA #REQUIRED>\n" +
                "\t\t<!ELEMENT condition EMPTY>\n" +
                "\t\t\t<!ATTLIST condition tag CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST condition mask CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST condition relation (eq|ne|gt|le|lt|ge) #REQUIRED>\n" +
                "\t\t\t<!ATTLIST condition value CDATA #REQUIRED>\n" +
                "\t\t<!ELEMENT input (control*)>\n" +
                "\t\t\t<!ATTLIST input service (yes|no) \"no\">\n" +
                "\t\t\t<!ATTLIST input tilt (yes|no) \"no\">\n" +
                "\t\t\t<!ATTLIST input players CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST input coins CDATA #IMPLIED>\n" +
                "\t\t\t<!ELEMENT control EMPTY>\n" +
                "\t\t\t\t<!ATTLIST control type CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST control player CDATA #IMPLIED>\n" +
                "\t\t\t\t<!ATTLIST control buttons CDATA #IMPLIED>\n" +
                "\t\t\t\t<!ATTLIST control reqbuttons CDATA #IMPLIED>\n" +
                "\t\t\t\t<!ATTLIST control minimum CDATA #IMPLIED>\n" +
                "\t\t\t\t<!ATTLIST control maximum CDATA #IMPLIED>\n" +
                "\t\t\t\t<!ATTLIST control sensitivity CDATA #IMPLIED>\n" +
                "\t\t\t\t<!ATTLIST control keydelta CDATA #IMPLIED>\n" +
                "\t\t\t\t<!ATTLIST control reverse (yes|no) \"no\">\n" +
                "\t\t\t\t<!ATTLIST control ways CDATA #IMPLIED>\n" +
                "\t\t\t\t<!ATTLIST control ways2 CDATA #IMPLIED>\n" +
                "\t\t\t\t<!ATTLIST control ways3 CDATA #IMPLIED>\n" +
                "\t\t<!ELEMENT dipswitch (condition?, diplocation*, dipvalue*)>\n" +
                "\t\t\t<!ATTLIST dipswitch name CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST dipswitch tag CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST dipswitch mask CDATA #REQUIRED>\n" +
                "\t\t\t<!ELEMENT diplocation EMPTY>\n" +
                "\t\t\t\t<!ATTLIST diplocation name CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST diplocation number CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST diplocation inverted (yes|no) \"no\">\n" +
                "\t\t\t<!ELEMENT dipvalue (condition?)>\n" +
                "\t\t\t\t<!ATTLIST dipvalue name CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST dipvalue value CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST dipvalue default (yes|no) \"no\">\n" +
                "\t\t<!ELEMENT configuration (condition?, conflocation*, confsetting*)>\n" +
                "\t\t\t<!ATTLIST configuration name CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST configuration tag CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST configuration mask CDATA #REQUIRED>\n" +
                "\t\t\t<!ELEMENT conflocation EMPTY>\n" +
                "\t\t\t\t<!ATTLIST conflocation name CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST conflocation number CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST conflocation inverted (yes|no) \"no\">\n" +
                "\t\t\t<!ELEMENT confsetting (condition?)>\n" +
                "\t\t\t\t<!ATTLIST confsetting name CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST confsetting value CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST confsetting default (yes|no) \"no\">\n" +
                "\t\t<!ELEMENT port (analog*)>\n" +
                "\t\t\t<!ATTLIST port tag CDATA #REQUIRED>\n" +
                "\t\t\t<!ELEMENT analog EMPTY>\n" +
                "\t\t\t\t<!ATTLIST analog mask CDATA #REQUIRED>\n" +
                "\t\t<!ELEMENT adjuster (condition?)>\n" +
                "\t\t\t<!ATTLIST adjuster name CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST adjuster default CDATA #REQUIRED>\n" +
                "\t\t<!ELEMENT driver EMPTY>\n" +
                "\t\t\t<!ATTLIST driver status (good|imperfect|preliminary) #REQUIRED>\n" +
                "\t\t\t<!ATTLIST driver emulation (good|imperfect|preliminary) #REQUIRED>\n" +
                "\t\t\t<!ATTLIST driver cocktail (good|imperfect|preliminary) #IMPLIED>\n" +
                "\t\t\t<!ATTLIST driver savestate (supported|unsupported) #REQUIRED>\n" +
                "\t\t\t<!ATTLIST driver requiresartwork (yes|no) \"no\">\n" +
                "\t\t\t<!ATTLIST driver unofficial (yes|no) \"no\">\n" +
                "\t\t\t<!ATTLIST driver nosoundhardware (yes|no) \"no\">\n" +
                "\t\t\t<!ATTLIST driver incomplete (yes|no) \"no\">\n" +
                "\t\t<!ELEMENT feature EMPTY>\n" +
                "\t\t\t<!ATTLIST feature type (protection|timing|graphics|palette|sound|capture|camera|microphone|controls|keyboard|mouse|media|disk|printer|tape|punch|drum|rom|comms|lan|wan) #REQUIRED>\n" +
                "\t\t\t<!ATTLIST feature status (unemulated|imperfect) #IMPLIED>\n" +
                "\t\t\t<!ATTLIST feature overall (unemulated|imperfect) #IMPLIED>\n" +
                "\t\t<!ELEMENT device (instance?, extension*)>\n" +
                "\t\t\t<!ATTLIST device type CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST device tag CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST device fixed_image CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST device mandatory CDATA #IMPLIED>\n" +
                "\t\t\t<!ATTLIST device interface CDATA #IMPLIED>\n" +
                "\t\t\t<!ELEMENT instance EMPTY>\n" +
                "\t\t\t\t<!ATTLIST instance name CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST instance briefname CDATA #REQUIRED>\n" +
                "\t\t\t<!ELEMENT extension EMPTY>\n" +
                "\t\t\t\t<!ATTLIST extension name CDATA #REQUIRED>\n" +
                "\t\t<!ELEMENT slot (slotoption*)>\n" +
                "\t\t\t<!ATTLIST slot name CDATA #REQUIRED>\n" +
                "\t\t\t<!ELEMENT slotoption EMPTY>\n" +
                "\t\t\t\t<!ATTLIST slotoption name CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST slotoption devname CDATA #REQUIRED>\n" +
                "\t\t\t\t<!ATTLIST slotoption default (yes|no) \"no\">\n" +
                "\t\t<!ELEMENT softwarelist EMPTY>\n" +
                "\t\t\t<!ATTLIST softwarelist tag CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST softwarelist name CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST softwarelist status (original|compatible) #REQUIRED>\n" +
                "\t\t\t<!ATTLIST softwarelist filter CDATA #IMPLIED>\n" +
                "\t\t<!ELEMENT ramoption (#PCDATA)>\n" +
                "\t\t\t<!ATTLIST ramoption name CDATA #REQUIRED>\n" +
                "\t\t\t<!ATTLIST ramoption default CDATA #IMPLIED>\n" +
                "]>";


        // XML feature names
        static readonly std.pair<device_t_feature_type, string> [] f_feature_names =
        {
            new std.pair<device_t_feature_type, string>( device_t_feature_type.PROTECTION,    "protection"    ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.TIMING,        "timing"        ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.GRAPHICS,      "graphics"      ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.PALETTE,       "palette"       ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.SOUND,         "sound"         ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.CAPTURE,       "capture"       ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.CAMERA,        "camera"        ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.MICROPHONE,    "microphone"    ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.CONTROLS,      "controls"      ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.KEYBOARD,      "keyboard"      ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.MOUSE,         "mouse"         ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.MEDIA,         "media"         ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.DISK,          "disk"          ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.PRINTER,       "printer"       ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.TAPE,          "tape"          ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.PUNCH,         "punch"         ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.DRUM,          "drum"          ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.ROM,           "rom"           ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.COMMS,         "comms"         ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.LAN,           "lan"           ),
            new std.pair<device_t_feature_type, string>( device_t_feature_type.WAN,           "wan"           )
        };


        // construction/destruction
        //-------------------------------------------------
        //  info_xml_creator - constructor
        //-------------------------------------------------
        public info_xml_creator(emu_options options, bool dtd = true)
        {
            m_dtd = dtd;
        }


        // output
        //-------------------------------------------------
        //  output - print the XML information for all
        //  known machines matching a pattern
        //-------------------------------------------------
        public void output(ref string out_, std.vector<string> patterns)  //void output(std::ostream &out, const std::vector<std::string> &patterns);
        {
            if (patterns.empty())
            {
                // no patterns specified - show everything
                output(ref out_);
            }
            else
            {
                // patterns specified - we have to filter output
                std.vector<bool> matched = new std.vector<bool>(patterns.size(), false);
                size_t exact_matches = 0;
                output_filter_delegate filter = (string shortname, ref bool done) =>  //const auto filter = [&patterns, &matched, &exact_matches](const char *shortname, bool &done) -> bool
                {
                    bool result = false;
                    int itIdx = 0;  //auto it = matched.begin();
                    foreach (string pat in patterns)
                    {
                        if (core_strwildcmp(pat, shortname) == 0)
                        {
                            // this driver matches the pattern - tell the caller
                            result = true;

                            // did we see this particular pattern before?  if not, track that we have
                            if (!matched[itIdx])  //if (!*it)
                            {
                                matched[itIdx] = true;  //*it = true;
                                if (!core_iswildstr(pat))
                                {
                                    exact_matches++;

                                    // stop looking if we found everything specified
                                    if (exact_matches == patterns.size())
                                        done = true;
                                }
                            }
                        }

                        itIdx++;  //it++;
                    }

                    return result;
                };

                output(ref out_, filter);

                // throw an error if there were unmatched patterns
                var iter = matched.FindIndex(match => { return !match; });  //auto iter = std::find(matched.begin(), matched.end(), false);
                if (iter != -1)  //if (iter != matched.end())
                {
                    int index = iter;  //int index = iter - matched.begin();
                    throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching machines found for '{0}'", patterns[index]);
                }
            }
        }


        //-------------------------------------------------
        //  output - print the XML information for all
        //  known (and filtered) machines
        //-------------------------------------------------
        delegate bool output_filter_delegate(string shortname, ref bool done);

        class prepared_info
        {
            public string m_xml_snippet;
            public device_type_set m_dev_set = new device_type_set();

            //prepared_info() = default;
            //prepared_info(const prepared_info &) = delete;
            //prepared_info(prepared_info &&) = default;
//#if defined(_CPPLIB_VER) && defined(_MSVC_STL_VERSION)
//            // MSVCPRT currently requires default-constructible std::future promise types to be assignable
//            // remove this workaround when that's fixed
//            prepared_info &operator=(const prepared_info &) = default;
//#else
//            prepared_info &operator=(const prepared_info &) = delete;
//#endif
        }

        delegate void output_header_if_necessary_delegate(ref string out_);

        void output(ref string out_, output_filter_delegate filter = null, bool include_devices = true)  //void output(std::ostream &out, const std::function<bool(const char *shortname, bool &done)> &filter = { }, bool include_devices = true);
        {

            // TODO: maybe not the best place for this as it affects the stream passed in
            // if the device part is threaded, the local streams used by the tasks can be
            // imbued and the stream passed in can be left alone
            //out.imbue(std::locale::classic());

            // prepare a driver enumerator and the queue
            driver_enumerator drivlist = new driver_enumerator(m_lookup_options);
            device_filter devfilter = new device_filter(filter);
            filtered_driver_enumerator filtered_drivlist = new filtered_driver_enumerator(drivlist, devfilter);
            bool header_outputted = false;

            // essentially a local method to emit the header if necessary
            output_header_if_necessary_delegate output_header_if_necessary = (ref string out_) =>  //auto output_header_if_necessary = [this, &header_outputted](std::ostream &out)
            {
                if (!header_outputted)
                {
                    output_header(ref out_, m_dtd);
                    header_outputted = true;
                }
            };

            // only keep a device set when we're asked to track it
            device_type_set devset = null;  //std::optional<device_type_set> devset;
            if (include_devices && filter != null)
                devset = new device_type_set();  //devset.emplace();

            // prepare a queue of tasks - this is a FIFO queue because of the
            // need to be deterministic
            std.queue<prepared_info> tasks = new std.queue<prepared_info>();  //std::queue<std::future<prepared_info>> tasks;

            // while we want to be deterministic, asynchronous task scheduling is not; so we want to
            // track the amount of active tasks so that we can keep on spawning tasks even if we're
            // waiting on the task in the front of the queue
            unsigned active_task_count = 0;  //std::atomic<unsigned int> active_task_count = 0;
            unsigned maximum_active_task_count = std.thread.hardware_concurrency() + 10;  //unsigned int maximum_active_task_count = std::thread::hardware_concurrency() + 10;
            unsigned maximum_outstanding_task_count = maximum_active_task_count + 20;

            // loop until we're done enumerating drivers, and until there are no outstanding tasks
            while (!filtered_drivlist.done() || !tasks.empty())
            {
                // loop until there are as many outstanding tasks as possible (we want to separately cap outstanding
                // tasks and active tasks)
                while (!filtered_drivlist.done()
                    && active_task_count < maximum_active_task_count
                    && tasks.size() < maximum_outstanding_task_count)
                {
                    // we want to launch a task; grab a packet of drivers to process
                    std.vector<game_driver> drivers = filtered_drivlist.next(20);  //std::vector<std::reference_wrapper<const game_driver>> drivers = filtered_drivlist.next(20);
                    if (drivers.empty())
                        break;

                    // do the dirty work asychronously
                    Func<prepared_info> task_proc = () =>  //auto task_proc = [&drivlist, drivers{ std::move(drivers) }, include_devices, &active_task_count]
                    {
                        prepared_info result = new prepared_info();
                        string stream = "";  //std::ostringstream stream;
                        //stream.imbue(std::locale::classic());

                        // output each of the drivers
                        foreach (game_driver driver in drivers)
                            output_one(ref stream, drivlist, driver, include_devices ? result.m_dev_set : null);

                        // capture the XML snippet
                        result.m_xml_snippet = stream;  //result.m_xml_snippet = stream.str();

                        // we're done with the task; decrement the counter and return
                        active_task_count--;

                        return result;
                    };

                    // add this task to the queue
                    active_task_count++;
                    tasks.emplace(task_proc());  //tasks.emplace(std::async(std::launch::async, std::move(task_proc)));
                }

                // we've put as many outstanding tasks out as we can; are there any tasks outstanding?
                if (!tasks.empty())
                {
                    // wait for the task at the front of the queue to complete and get the info, in the
                    // spirit of determinism
                    prepared_info pi = tasks.front();
                    tasks.pop();

                    // emit whatever XML we accumulated in the task
                    output_header_if_necessary(ref out_);
                    out_ += pi.m_xml_snippet;  //out << pi.m_xml_snippet;

                    // merge devices into devset, if appropriate
                    if (devset != null)
                    {
                        foreach (var x in pi.m_dev_set)
                            devset.insert(x);
                    }
                }
            }

            // iterate through the device types if not everything matches a driver
            if (devset != null && !devfilter.done())
            {
                foreach (device_type type in registered_device_types)
                {
                    if (devfilter.filter(type.shortname()))
                        devset.insert(type);

                    if (devfilter.done())
                        break;
                }
            }

            // output devices (both devices with roms and slot devices)
            if (include_devices && (devset == null || !devset.empty()))
            {
                output_header_if_necessary(ref out_);
                output_devices(ref out_, m_lookup_options, devset != null ? devset : null);
            }

            if (header_outputted)
                output_footer(ref out_);
        }


        //static char const *feature_name(device_t::feature_type feature);


        //-------------------------------------------------
        //  output_header - print the XML DTD and open
        //  the root element
        //-------------------------------------------------
        void output_header(ref string out_, bool dtd)  //void output_header(std::ostream &out, bool dtd)
        {
            if (dtd)
            {
                // output the DTD
                out_ += "<?xml version=\"1.0\"?>\n";
                string dtd_s = f_dtd_string;
                strreplace(ref dtd_s, "__XML_ROOT__", XML_ROOT);
                strreplace(ref dtd_s, "__XML_TOP__", XML_TOP);

                out_ += dtd_s + "\n\n";
            }

            // top-level tag
            util.stream_format(ref out_,
                    "<{0} build=\"{1}\" debug=\"" +
#if MAME_DEBUG
                    "yes" +
#else
                    "no" +
#endif
                    "\" mameconfig=\"{2}\">\n",
                    XML_ROOT,
                    normalize_string(emulator_info.get_build_version()),
                    configuration_manager.CONFIG_VERSION);
        }


        //-------------------------------------------------
        //  output_header - close the root element
        //-------------------------------------------------
        void output_footer(ref string out_)  //void output_footer(std::ostream &out)
        {
            // close the top level tag
            util.stream_format(ref out_, "</{0}>\n", XML_ROOT);
        }


        //-------------------------------------------------
        //  output_one - print the XML information
        //  for one particular machine driver
        //-------------------------------------------------
        void output_one(ref string out_, driver_enumerator drivlist, game_driver driver, device_type_set devtypes)  //void output_one(std::ostream &out, driver_enumerator &drivlist, const game_driver &driver, device_type_set *devtypes)
        {
            machine_config config = new machine_config(driver, drivlist.options());
            device_enumerator iter = new device_enumerator(config.root_device());

            // allocate input ports and build overall emulation status
            ioport_list portlist = new ioport_list();
            string errors;
            device_t_feature_type overall_unemulated = driver.type.unemulated_features();
            device_t_feature_type overall_imperfect = driver.type.imperfect_features();
            foreach (device_t device in iter)
            {
                portlist.append(device, out errors);
                overall_unemulated |= device.type().unemulated_features();
                overall_imperfect |= device.type().imperfect_features();

                if (devtypes != null && device.owner() != null)
                    devtypes.insert(device.type());
            }

            // renumber player numbers for controller ports
            int player_offset = 0;
            // but treat keyboard count separately from players' number
            int kbd_offset = 0;
            foreach (device_t device in iter)
            {
                int nplayers = 0;
                bool new_kbd = false;
                foreach (var port in portlist)
                {
                    if (port.second().device() == device)
                    {
                        foreach (ioport_field field in port.second().fields())
                        {
                            if (field.type() >= ioport_type.IPT_START && field.type() < ioport_type.IPT_ANALOG_LAST)
                            {
                                if (field.type() == ioport_type.IPT_KEYBOARD)
                                {
                                    if (!new_kbd)
                                        new_kbd = true;
                                    field.set_player((u8)(field.player() + kbd_offset));
                                }
                                else
                                {
                                    nplayers = std.max(nplayers, field.player() + 1);
                                    field.set_player((u8)(field.player() + player_offset));
                                }
                            }
                        }
                    }
                }

                player_offset += nplayers;
                if (new_kbd) kbd_offset++;
            }

            // print the header and the machine name
            util.stream_format(ref out_, "\t<{0} name=\"{1}\"", XML_TOP, normalize_string(driver.name));

            // strip away any path information from the source_file and output it
            string src = driver.type.source();
            var prefix = src.find("src/mame/");
            if (npos == prefix)
                prefix = src.find("src\\mame\\");
            if (npos != prefix)
                src = src.remove_prefix_(prefix + 9);
            util.stream_format(ref out_, " sourcefile=\"{0}\"", normalize_string(src));

            // append bios and runnable flags
            if ((driver.flags & machine_flags.type.IS_BIOS_ROOT) != 0)
                out_ += " isbios=\"yes\"";
            if ((driver.flags & machine_flags.type.MECHANICAL) != 0)
                out_ += " ismechanical=\"yes\"";

            // display clone information
            int clone_of = driver_list.find(driver.parent);
            if (clone_of != -1 && (driver_list.driver((size_t)clone_of).flags & machine_flags.type.IS_BIOS_ROOT) == 0)
                util.stream_format(ref out_, " cloneof=\"{0}\"", normalize_string(driver_list.driver((size_t)clone_of).name));
            if (clone_of != -1)
                util.stream_format(ref out_, " romof=\"{0}\"", normalize_string(driver_list.driver((size_t)clone_of).name));

            // display sample information and close the game tag
            output_sampleof(ref out_, config.root_device());
            out_ += ">\n";

            // output game description
            if (driver.type.fullname() != null)
                util.stream_format(ref out_, "\t\t<description>{0}</description>\n", normalize_string(driver.type.fullname()));

            // print the year only if is a number or another allowed character (? or +)
            if (!string.IsNullOrEmpty(driver.year) && std.strspn(driver.year, "0123456789?+") == std.strlen(driver.year))
                util.stream_format(ref out_, "\t\t<year>{0}</year>\n", normalize_string(driver.year));

            // print the manufacturer information
            if (driver.manufacturer != null)
                util.stream_format(ref out_, "\t\t<manufacturer>{0}</manufacturer>\n", normalize_string(driver.manufacturer));

            // now print various additional information
            output_bios(ref out_, config.root_device());
            output_rom(ref out_, config, drivlist, driver, config.root_device());
            output_device_refs(ref out_, config.root_device());
            output_sample(ref out_, config.root_device());
            output_chips(ref out_, config.root_device(), "");
            output_display(ref out_, config.root_device(), driver.flags, "");
            output_sound(ref out_, config.root_device());
            output_input(ref out_, portlist);
            output_switches(ref out_, portlist, "", (int)ioport_type.IPT_DIPSWITCH, "dipswitch", "diplocation", "dipvalue");
            output_switches(ref out_, portlist, "", (int)ioport_type.IPT_CONFIG, "configuration", "conflocation", "confsetting");
            output_ports(ref out_, portlist);
            output_adjusters(ref out_, portlist);
            output_driver(ref out_, driver, overall_unemulated, overall_imperfect);
            output_features(ref out_, driver.type, overall_unemulated, overall_imperfect);
            output_images(ref out_, config.root_device(), "");
            output_slots(ref out_, config, config.root_device(), "", devtypes);
            output_software_lists(ref out_, config.root_device(), "");
            output_ramoptions(ref out_, config.root_device());

            // close the topmost tag
            util.stream_format(ref out_, "\t</{0}>\n", XML_TOP);
        }


        //-------------------------------------------------
        //  output_one_device - print the XML info for
        //  a single device
        //-------------------------------------------------
        void output_one_device(ref string out_, machine_config config, device_t device, string devtag)  //void output_one_device(std::ostream &out, machine_config &config, device_t &device, const char *devtag)
        {
            bool has_speaker = false;
            bool has_input = false;
            // check if the device adds speakers to the system
            sound_interface_enumerator snditer = new sound_interface_enumerator(device);
            if (snditer.first() != null)
                has_speaker = true;

            // generate input list and build overall emulation status
            ioport_list portlist = new ioport_list();
            string errors;
            device_t_feature_type overall_unemulated = device.type().unemulated_features();
            device_t_feature_type overall_imperfect = device.type().imperfect_features();
            foreach (device_t dev in new device_enumerator(device))
            {
                portlist.append(dev, out errors);
                overall_unemulated |= dev.type().unemulated_features();
                overall_imperfect |= dev.type().imperfect_features();
            }

            // check if the device adds player inputs (other than dsw and configs) to the system
            foreach (var port in portlist)
            {
                foreach (ioport_field field in port.second().fields())
                {
                    if (field.type() >= ioport_type.IPT_START1 && field.type() < ioport_type.IPT_UI_FIRST)
                    {
                        has_input = true;
                        break;
                    }
                }
            }

            // start to output info
            util.stream_format(ref out_, "\t<{0} name=\"{1}\"", XML_TOP, normalize_string(device.shortname()));
            string src = device.source();
            var prefix = src.find("src/");
            if (npos == prefix)
                prefix = src.find("src\\");
            if (npos != prefix)
                src = src.remove_prefix_(prefix + 4);
            util.stream_format(ref out_, " sourcefile=\"{0}\" isdevice=\"yes\" runnable=\"no\"", normalize_string(src));
            var parent = device.type().parent_rom_device_type();
            if (parent != null)
                util.stream_format(ref out_, " romof=\"{0}\"", normalize_string(parent.shortname()));
            output_sampleof(ref out_, device);
            out_ += ">\n";
            util.stream_format(ref out_, "\t\t<description>{0}</description>\n", normalize_string(device.name()));

            output_bios(ref out_, device);
            output_rom(ref out_, config, null, null, device);
            output_device_refs(ref out_, device);

            if (device.type().type() != samples_device_enumerator_helper.get_samples_device_type())  //if (device.type().type() != typeid(samples_device)) // ignore samples_device itself
                output_sample(ref out_, device);

            output_chips(ref out_, device, devtag);
            output_display(ref out_, device, default, devtag);
            if (has_speaker)
                output_sound(ref out_, device);
            if (has_input)
                output_input(ref out_, portlist);
            output_switches(ref out_, portlist, devtag, (int)ioport_type.IPT_DIPSWITCH, "dipswitch", "diplocation", "dipvalue");
            output_switches(ref out_, portlist, devtag, (int)ioport_type.IPT_CONFIG, "configuration", "conflocation", "confsetting");
            output_adjusters(ref out_, portlist);
            output_features(ref out_, device.type(), overall_unemulated, overall_imperfect);
            output_images(ref out_, device, devtag);
            output_slots(ref out_, config, device, devtag, null);
            output_software_lists(ref out_, device, devtag);
            util.stream_format(ref out_, "\t</{0}>\n", XML_TOP);
        }


        //-------------------------------------------------
        //  output_devices - print the XML info for
        //  registered device types
        //-------------------------------------------------
        void output_devices(ref string out_, emu_options lookup_options, device_type_set filter)  //void output_devices(std::ostream &out, emu_options &lookup_options, device_type_set const *filter)
        {
            // get config for empty machine
            machine_config config = new machine_config(___empty.driver____empty, lookup_options);  //machine_config config(GAME_NAME(___empty), lookup_options);

            Func<device_type, string> action = (device_type type) =>
            {
                // add it at the root of the machine config
                device_t dev;
                {
                    using machine_config.token tok = config.begin_configuration(config.root_device());
                    dev = config.device_add("_tmp", type, 0);
                }

                // notify this device and all its subdevices that they are now configured
                foreach (device_t device in new device_enumerator(dev))
                {
                    if (!device.configured())
                        device.config_complete();
                }

                // print details and remove it
                string temp_out = "";
                output_one_device(ref temp_out, config, dev, dev.tag());
                {
                    using machine_config.token tok = config.begin_configuration(config.root_device());
                    config.device_remove("_tmp");
                }

                return temp_out;
            };

            // run through devices
            if (filter != null)
            {
                foreach (device_type type in filter) out_ += action(type);  //for (std::add_pointer_t<device_type> type : *filter) action(*type);
            }
            else
            {
                foreach (device_type type in registered_device_types) out_ += action(type);
            }
        }


        //------------------------------------------------
        //  output_device_refs - when a machine uses a
        //  subdevice, print a reference
        //-------------------------------------------------
        void output_device_refs(ref string out_, device_t root)
        {
            foreach (device_t device in new device_enumerator(root))
            {
                if (device != root)
                    util.stream_format(ref out_, "\t\t<device_ref name=\"{0}\"/>\n", normalize_string(device.shortname()));
            }
        }


        //------------------------------------------------
        //  output_sampleof - print the 'sampleof'
        //  attribute, if appropriate
        //-------------------------------------------------
        void output_sampleof(ref string out_, device_t device)  //void output_sampleof(std::ostream &out, device_t &device)
        {
            // iterate over sample devices
            object [] samples_devices = samples_device_enumerator_helper.get_samples_devices(device);
            foreach (var samples in samples_devices)  //for (samples_device &samples : samples_device_enumerator(device))
            {
                object sampiter = samples_device_enumerator_helper.get_samples_iterator(samples);  //samples_iterator sampiter(samples);
                if (samples_device_enumerator_helper.get_altbasename(sampiter) != null)  //if (sampiter.altbasename() != nullptr)
                {
                    util.stream_format(ref out_, " sampleof=\"{0}\"", normalize_string(samples_device_enumerator_helper.get_altbasename(sampiter)));  //util::stream_format(out, " sampleof=\"%s\"", normalize_string(sampiter.altbasename()));

                    // must stop here, as there can only be one attribute of the same name
                    return;
                }
            }
        }


        //-------------------------------------------------
        //  output_bios - print BIOS sets for a device
        //-------------------------------------------------
        void output_bios(ref string out_, device_t device)
        {
            // first determine the default BIOS name
            string defaultname = null;  //char const *defaultname(nullptr);
            for (Pointer<tiny_rom_entry> rom = device.rom_region(); rom != null && !ROMENTRY_ISEND(rom); ++rom)
            {
                if (ROMENTRY_ISDEFAULT_BIOS(rom))
                    defaultname = rom.op.name();
            }

            // iterate over ROM entries and look for BIOSes
            foreach (romload.system_bios bios in new romload.entries(device.rom_region()).get_system_bioses())
            {
                // output extracted name and descriptions'
                out_ += "\t\t<biosset";
                util.stream_format(ref out_, " name=\"{0}\"", normalize_string(bios.get_name()));
                util.stream_format(ref out_, " description=\"{1}\"", normalize_string(bios.get_description()));
                if (!string.IsNullOrEmpty(defaultname) && std.strcmp(defaultname, bios.get_name()) == 0)
                    out_ += " default=\"yes\"";
                out_ += "/>\n";
            }
        }


        //-------------------------------------------------
        //  output_rom - print the roms section of
        //  the XML output
        //-------------------------------------------------

        enum type { BIOS, NORMAL, DISK }

        void output_rom(ref string out_, machine_config config, driver_list drivlist, game_driver driver, device_t device)
        {
            std.map<u32, string> biosnames = new std.map<u32, string>();
            bool bios_scanned = false;
            Func<Pointer<tiny_rom_entry>, string> get_biosname =
                    (Pointer<tiny_rom_entry> romIn) =>
                    {
                        Pointer<tiny_rom_entry> rom = new Pointer<tiny_rom_entry>(romIn);

                        u32 biosflags = ROM_GETBIOSFLAGS(rom);
                        var found = biosnames.find(biosflags);  //std::map<u32, char const *>::const_iterator const found(biosnames.find(biosflags));
                        if (default != found)
                            return found;

                        string result = null;
                        if (!bios_scanned)
                        {
                            for (++rom; !ROMENTRY_ISEND(rom); ++rom)
                            {
                                if (ROMENTRY_ISSYSTEM_BIOS(rom))
                                {
                                    u32 biosno = ROM_GETBIOSFLAGS(rom);
                                    biosnames.emplace(biosno, rom.op.name());
                                    if (biosflags == biosno)
                                        result = rom.op.name();
                                }
                            }
                            bios_scanned = true;
                        }
                        return result;
                    };
            Func<Pointer<tiny_rom_entry>, u32> rom_file_size = // FIXME: need a common way to do this without the cost of allocating rom_entry
                    (Pointer<tiny_rom_entry> rompIn) =>
                    {
                        Pointer<tiny_rom_entry> romp = new Pointer<tiny_rom_entry>(rompIn);

                        u32 maxlength = 0;

                        // loop until we run out of reloads
                        do
                        {
                            // loop until we run out of continues/ignores
                            u32 curlength = ROM_GETLENGTH(romp);  romp++;
                            while (ROMENTRY_ISCONTINUE(romp) || ROMENTRY_ISIGNORE(romp))
                                { curlength += ROM_GETLENGTH(romp);  romp++; }

                            // track the maximum length
                            maxlength = std.max(maxlength, curlength);
                        }
                        while (ROMENTRY_ISRELOAD(romp));

                        return maxlength;
                    };

            // iterate over 3 different ROM "types": BIOS, ROMs, DISKs
            bool driver_merge = drivlist != null && (driver_device)device != null;
            foreach (type pass in (type [])Enum.GetValues(typeof(type)))
            {
                Pointer<tiny_rom_entry> region = null;
                for (Pointer<tiny_rom_entry> rom = device.rom_region(); rom != null && !ROMENTRY_ISEND(rom); ++rom)
                {
                    if (ROMENTRY_ISREGION(rom))
                        region = new Pointer<tiny_rom_entry>(rom);
                    else if (ROMENTRY_ISSYSTEM_BIOS(rom))
                        biosnames.emplace(ROM_GETBIOSFLAGS(rom), rom.op.name());

                    if (!ROMENTRY_ISFILE(rom))
                        continue;

                    // only list disks on the disk pass
                    bool is_disk = ROMREGION_ISDISKDATA(region);
                    if (type.DISK == pass != is_disk)
                        continue;

                    // BIOS ROMs only apply to BIOSes
                    // FIXME: disk images associated with a system BIOS will never be listed
                    u32 biosno = ROM_GETBIOSFLAGS(rom);
                    if ((type.BIOS == pass) != (biosno != 0))
                        continue;
                    string bios_name = (!is_disk && biosno != 0) ? get_biosname(rom) : null;

                    // if we have a valid ROM and we are a clone, see if we can find the parent ROM
                    util.hash_collection hashes = new util.hash_collection(rom.op.hashdata());
                    string merge_name =
                            hashes.flag(util.hash_collection.FLAG_NO_DUMP) ? null :
                            driver_merge ? get_merge_name(drivlist, driver, hashes) :
                            get_merge_name(config, device, hashes);

                    // opening tag
                    if (is_disk)
                        out_ += "\t\t<disk";
                    else
                        out_ += "\t\t<rom";

                    // add name, merge, bios, and size tags
                    string name = rom.op.name();
                    if (!string.IsNullOrEmpty(name))  //if (name && name[0])
                        util.stream_format(ref out_, " name=\"{0}\"", normalize_string(name));
                    if (!string.IsNullOrEmpty(merge_name))
                        util.stream_format(ref out_, " merge=\"{0}\"", normalize_string(merge_name));
                    if (!string.IsNullOrEmpty(bios_name))
                        util.stream_format(ref out_, " bios=\"{0}\"", normalize_string(bios_name));
                    if (!is_disk)
                        util.stream_format(ref out_, " size=\"{0}\"", rom_file_size(rom));

                    // dump checksum information only if there is a known dump
                    if (!hashes.flag(util.hash_collection.FLAG_NO_DUMP))
                        out_ += " " + hashes.attribute_string(); // iterate over hash function types and print m_output their values
                    else
                        out_ += " status=\"nodump\"";

                    // append a region name
                    util.stream_format(ref out_, " region=\"{0}\"", region.op.name());

                    if (!is_disk)
                    {
                        // for non-disk entries, print offset
                        util.stream_format(ref out_, " offset=\"{0}\"", ROM_GETOFFSET(rom));
                    }
                    else
                    {
                        // for disk entries, add the disk index
                        util.stream_format(ref out_, " index=\"{0}\" writable=\"{1}\"", DISK_GETINDEX(rom), DISK_ISREADONLY(rom) ? "no" : "yes");
                    }

                    // add optional flag
                    if (ROM_ISOPTIONAL(rom))
                        out_ += " optional=\"yes\"";

                    out_ += "/>\n";
                }

                bios_scanned = true;
            }
        }


        //-------------------------------------------------
        //  output_sample - print a list of all
        //  samples referenced by a game_driver
        //-------------------------------------------------
        void output_sample(ref string out_, device_t device)
        {
            // iterate over sample devices
            object [] samples_devices = samples_device_enumerator_helper.get_samples_devices(device);
            foreach (var samples in samples_devices)  //for (samples_device &samples : samples_device_enumerator(device))
            {
                object iter = samples_device_enumerator_helper.get_samples_iterator(samples);  //samples_iterator iter(samples);
                std.unordered_set<string> already_printed = new std.unordered_set<string>();
                string [] iter_samplenames = samples_device_enumerator_helper.get_samplenames(iter);
                foreach (var samplename in iter_samplenames)  //for (const char *samplename = iter.first(); samplename != nullptr; samplename = iter.next())
                {
                    // filter out duplicates
                    if (!already_printed.insert(samplename))
                        continue;

                    // output the sample name
                    util.stream_format(ref out_, "\t\t<sample name=\"{0}\"/>\n", normalize_string(samplename));
                }
            }
        }


        /*-------------------------------------------------
            output_chips - print a list of CPU and
            sound chips used by a game
        -------------------------------------------------*/
        void output_chips(ref string out_, device_t device, string root_tag)
        {
            // iterate over executable devices
            foreach (device_execute_interface exec in new execute_interface_enumerator(device))
            {
                if (std.strcmp(exec.device().tag(), device.tag()) != 0)
                {
                    string newtag = exec.device().tag();
                    string oldtag = ":";
                    newtag = newtag.substr(newtag.find(oldtag = oldtag.append_(root_tag)) + oldtag.length());

                    out_ += "\t\t<chip";
                    out_ += " type=\"cpu\"";
                    util.stream_format(ref out_, " tag=\"{0}\"", normalize_string(newtag));
                    util.stream_format(ref out_, " name=\"{0}\"", normalize_string(exec.device().name()));
                    util.stream_format(ref out_, " clock=\"{0}\"", exec.device().clock());
                    out_ += "/>\n";
                }
            }

            // iterate over sound devices
            foreach (device_sound_interface sound in new sound_interface_enumerator(device))
            {
                if (std.strcmp(sound.device().tag(), device.tag()) != 0 && sound.issound())
                {
                    string newtag = sound.device().tag();
                    string oldtag = ":";
                    newtag = newtag.substr(newtag.find(oldtag = oldtag.append_(root_tag)) + oldtag.length());

                    out_ += "\t\t<chip";
                    out_ += " type=\"audio\"";
                    util.stream_format(ref out_, " tag=\"{0}\"", normalize_string(newtag));
                    util.stream_format(ref out_, " name=\"{0}\"", normalize_string(sound.device().name()));
                    if (sound.device().clock() != 0)
                        util.stream_format(ref out_, " clock=\"{0}\"", sound.device().clock());
                    out_ += "/>\n";
                }
            }
        }


        //-------------------------------------------------
        //  output_display - print a list of all the
        //  displays
        //-------------------------------------------------
        void output_display(ref string out_, device_t device, machine_flags.type flags, string root_tag)
        {
            // iterate over screens
            foreach (screen_device screendev in new screen_device_enumerator(device))
            {
                if (std.strcmp(screendev.tag(), device.tag()) != 0)
                {
                    string newtag = screendev.tag();
                    string oldtag = ":";
                    newtag = newtag.substr(newtag.find(oldtag = oldtag.append_(root_tag)) + oldtag.length());

                    util.stream_format(ref out_, "\t\t<display tag=\"{0}\"", normalize_string(newtag));

                    switch (screendev.screen_type())
                    {
                        case screen_type_enum.SCREEN_TYPE_RASTER:    out_ += " type=\"raster\"";  break;
                        case screen_type_enum.SCREEN_TYPE_VECTOR:    out_ += " type=\"vector\"";  break;
                        case screen_type_enum.SCREEN_TYPE_LCD:       out_ += " type=\"lcd\"";     break;
                        case screen_type_enum.SCREEN_TYPE_SVG:       out_ += " type=\"svg\"";     break;
                        default:                                     out_ += " type=\"unknown\""; break;
                    }

                    // output the orientation as a string
                    switch (screendev.orientation())
                    {
                    case ORIENTATION_FLIP_X:
                        out_ += " rotate=\"0\" flipx=\"yes\"";
                        break;
                    case ORIENTATION_FLIP_Y:
                        out_ += " rotate=\"180\" flipx=\"yes\"";
                        break;
                    case ORIENTATION_FLIP_X|ORIENTATION_FLIP_Y:
                        out_ += " rotate=\"180\"";
                        break;
                    case ORIENTATION_SWAP_XY:
                        out_ += " rotate=\"90\" flipx=\"yes\"";
                        break;
                    case ORIENTATION_SWAP_XY|ORIENTATION_FLIP_X:
                        out_ += " rotate=\"90\"";
                        break;
                    case ORIENTATION_SWAP_XY|ORIENTATION_FLIP_Y:
                        out_ += " rotate=\"270\"";
                        break;
                    case ORIENTATION_SWAP_XY|ORIENTATION_FLIP_X|ORIENTATION_FLIP_Y:
                        out_ += " rotate=\"270\" flipx=\"yes\"";
                        break;
                    default:
                        out_ += " rotate=\"0\"";
                        break;
                    }

                    // output width and height only for games that are not vector
                    if (screendev.screen_type() != screen_type_enum.SCREEN_TYPE_VECTOR)
                    {
                        rectangle visarea = screendev.visible_area();
                        util.stream_format(ref out_, " width=\"{0}\"", visarea.width());
                        util.stream_format(ref out_, " height=\"{0}\"", visarea.height());
                    }

                    // output refresh rate
                    util.stream_format(ref out_, " refresh=\"{0}\"", ATTOSECONDS_TO_HZ(screendev.refresh_attoseconds()));

                    // output raw video parameters only for games that are not vector
                    // and had raw parameters specified
                    if (screendev.screen_type() != screen_type_enum.SCREEN_TYPE_VECTOR && !screendev.oldstyle_vblank_supplied())
                    {
                        int pixclock = (int)(screendev.width() * screendev.height() * ATTOSECONDS_TO_HZ(screendev.refresh_attoseconds()));

                        util.stream_format(ref out_, " pixclock=\"{0}\"", pixclock);
                        util.stream_format(ref out_, " htotal=\"{0}\"", screendev.width());
                        util.stream_format(ref out_, " hbend=\"{0}\"", screendev.visible_area().min_x);
                        util.stream_format(ref out_, " hbstart=\"{0}\"", screendev.visible_area().max_x+1);
                        util.stream_format(ref out_, " vtotal=\"{0}\"", screendev.height());
                        util.stream_format(ref out_, " vbend=\"{0}\"", screendev.visible_area().min_y);
                        util.stream_format(ref out_, " vbstart=\"{0}\"", screendev.visible_area().max_y+1);
                    }

                    out_ += " />\n";
                }
            }
        }


        //-------------------------------------------------
        //  output_sound - print a list of all the
        //  speakers
        //------------------------------------------------
        void output_sound(ref string out_, device_t device)
        {
            speaker_device_enumerator spkiter = new speaker_device_enumerator(device);
            int speakers = spkiter.count();

            // if we have no sound, zero m_output the speaker count
            sound_interface_enumerator snditer = new sound_interface_enumerator(device);
            if (snditer.first() == null)
                speakers = 0;

            util.stream_format(ref out_, "\t\t<sound channels=\"{0}\"/>\n", speakers);
        }


        //-------------------------------------------------
        //  output_ioport_condition - print condition
        //  required to use I/O port field/setting
        //-------------------------------------------------
        void output_ioport_condition(ref string out_, ioport_condition condition, unsigned indent)
        {
            for (unsigned i = 0; indent > i; ++i)
                out_ += '\t';

            string rel = null;
            switch (condition.condition())
            {
            case ioport_condition.condition_t.ALWAYS:          throw new emu_fatalerror("");  //throw false;
            case ioport_condition.condition_t.EQUALS:          rel = "eq"; break;
            case ioport_condition.condition_t.NOTEQUALS:       rel = "ne"; break;
            case ioport_condition.condition_t.GREATERTHAN:     rel = "gt"; break;
            case ioport_condition.condition_t.NOTGREATERTHAN:  rel = "le"; break;
            case ioport_condition.condition_t.LESSTHAN:        rel = "lt"; break;
            case ioport_condition.condition_t.NOTLESSTHAN:     rel = "ge"; break;
            }

            util.stream_format(ref out_, "<condition tag=\"{0}\" mask=\"{1}\" relation=\"{2}\" value=\"{3}\"/>\n", normalize_string(condition.tag()), condition.mask(), rel, condition.value());
        }

        //-------------------------------------------------
        //  output_input - print a summary of a game's
        //  input
        //-------------------------------------------------

        // enumerated list of control types
        // NOTE: the order is chosen so that 'spare' button inputs are assigned to the
        // most-likely-correct input device when info is output (you can think of it as
        // a sort of likelihood order of having buttons)
        //enum
        //{
        const int CTRL_DIGITAL_BUTTONS   =  0;
        const int CTRL_DIGITAL_JOYSTICK  =  1;
        const int CTRL_ANALOG_JOYSTICK   =  2;
        const int CTRL_ANALOG_LIGHTGUN   =  3;
        const int CTRL_ANALOG_DIAL       =  4;
        const int CTRL_ANALOG_POSITIONAL =  5;
        const int CTRL_ANALOG_TRACKBALL  =  6;
        const int CTRL_ANALOG_MOUSE      =  7;
        const int CTRL_ANALOG_PADDLE     =  8;
        const int CTRL_ANALOG_PEDAL      =  9;
        const int CTRL_DIGITAL_KEYPAD    = 10;
        const int CTRL_DIGITAL_KEYBOARD  = 11;
        const int CTRL_DIGITAL_MAHJONG   = 12;
        const int CTRL_DIGITAL_HANAFUDA  = 13;
        const int CTRL_DIGITAL_GAMBLING  = 14;
        const int CTRL_COUNT             = 15;
        //};

        //enum
        //{
        const int CTRL_P1     = 0;
        const int CTRL_P2     = 1;
        const int CTRL_P3     = 2;
        const int CTRL_P4     = 3;
        const int CTRL_P5     = 4;
        const int CTRL_P6     = 5;
        const int CTRL_P7     = 6;
        const int CTRL_P8     = 7;
        const int CTRL_P9     = 8;
        const int CTRL_P10    = 9;
        const int CTRL_PCOUNT = 10;
        //};

        // initialize the list of control types
        class control_info_struct
        {
            public string type;           // general type of input
            public int player;         // player which the input belongs to
            public int nbuttons;       // total number of buttons
            public int reqbuttons;     // total number of non-optional buttons
            public int maxbuttons;     // max index of buttons (using IPT_BUTTONn) [probably to be removed soonish]
            public int ways;           // directions for joystick
            public bool analog;         // is analog input?
            public uint8_t [] helper = new uint8_t [3];      // for dual joysticks [possibly to be removed soonish]
            public int32_t min;            // analog minimum value
            public int32_t max;            // analog maximum value
            public int32_t sensitivity;    // default analog sensitivity
            public int32_t keydelta;       // default analog keydelta
            public bool reverse;        // default analog reverse setting
        }
        //control_info_struct [] control_info = new control_info_struct [CTRL_COUNT * CTRL_PCOUNT];

        void output_input(ref string out_, ioport_list portlist)
        {
            // directions
            const uint8_t DIR_UP = 0x01;
            const uint8_t DIR_DOWN = 0x02;
            const uint8_t DIR_LEFT = 0x04;
            const uint8_t DIR_RIGHT = 0x08;

            //memset(&control_info, 0, sizeof(control_info));
            control_info_struct [] control_info = new control_info_struct [CTRL_COUNT * CTRL_PCOUNT];
            for (int i = 0; i < control_info.Length; i++)
                control_info[i] = new control_info_struct();

            // tracking info as we iterate
            int nplayer = 0;
            int ncoin = 0;
            bool service = false;
            bool tilt = false;

            // iterate over the ports
            foreach (var port in portlist)
            {
                int ctrl_type = CTRL_DIGITAL_BUTTONS;
                bool ctrl_analog = false;
                foreach (ioport_field field in port.second().fields())
                {
                    // track the highest player number
                    if (nplayer < field.player() + 1)
                        nplayer = field.player() + 1;

                    // switch off of the type
                    switch (field.type())
                    {
                    // map joysticks
                    case ioport_type.IPT_JOYSTICK_UP:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[0] |= DIR_UP;
                        break;
                    case ioport_type.IPT_JOYSTICK_DOWN:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[0] |= DIR_DOWN;
                        break;
                    case ioport_type.IPT_JOYSTICK_LEFT:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[0] |= DIR_LEFT;
                        break;
                    case ioport_type.IPT_JOYSTICK_RIGHT:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[0] |= DIR_RIGHT;
                        break;

                    case ioport_type.IPT_JOYSTICKLEFT_UP:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[1] |= DIR_UP;
                        break;
                    case ioport_type.IPT_JOYSTICKLEFT_DOWN:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[1] |= DIR_DOWN;
                        break;
                    case ioport_type.IPT_JOYSTICKLEFT_LEFT:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[1] |= DIR_LEFT;
                        break;
                    case ioport_type.IPT_JOYSTICKLEFT_RIGHT:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[1] |= DIR_RIGHT;
                        break;

                    case ioport_type.IPT_JOYSTICKRIGHT_UP:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[2] |= DIR_UP;
                        break;
                    case ioport_type.IPT_JOYSTICKRIGHT_DOWN:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[2] |= DIR_DOWN;
                        break;
                    case ioport_type.IPT_JOYSTICKRIGHT_LEFT:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[2] |= DIR_LEFT;
                        break;
                    case ioport_type.IPT_JOYSTICKRIGHT_RIGHT:
                        ctrl_type = CTRL_DIGITAL_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "joy";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].ways = field.way();
                        control_info[field.player() * CTRL_COUNT + ctrl_type].helper[2] |= DIR_RIGHT;
                        break;

                    // map analog inputs
                    case ioport_type.IPT_AD_STICK_X:
                    case ioport_type.IPT_AD_STICK_Y:
                    case ioport_type.IPT_AD_STICK_Z:
                        ctrl_analog = true;
                        ctrl_type = CTRL_ANALOG_JOYSTICK;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "stick";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].analog = true;
                        break;

                    case ioport_type.IPT_PADDLE:
                    case ioport_type.IPT_PADDLE_V:
                        ctrl_analog = true;
                        ctrl_type = CTRL_ANALOG_PADDLE;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "paddle";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].analog = true;
                        break;

                    case ioport_type.IPT_PEDAL:
                    case ioport_type.IPT_PEDAL2:
                    case ioport_type.IPT_PEDAL3:
                        ctrl_analog = true;
                        ctrl_type = CTRL_ANALOG_PEDAL;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "pedal";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].analog = true;
                        break;

                    case ioport_type.IPT_LIGHTGUN_X:
                    case ioport_type.IPT_LIGHTGUN_Y:
                        ctrl_analog = true;
                        ctrl_type = CTRL_ANALOG_LIGHTGUN;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "lightgun";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].analog = true;
                        break;

                    case ioport_type.IPT_POSITIONAL:
                    case ioport_type.IPT_POSITIONAL_V:
                        ctrl_analog = true;
                        ctrl_type = CTRL_ANALOG_POSITIONAL;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "positional";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].analog = true;
                        break;

                    case ioport_type.IPT_DIAL:
                    case ioport_type.IPT_DIAL_V:
                        ctrl_analog = true;
                        ctrl_type = CTRL_ANALOG_DIAL;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "dial";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].analog = true;
                        break;

                    case ioport_type.IPT_TRACKBALL_X:
                    case ioport_type.IPT_TRACKBALL_Y:
                        ctrl_analog = true;
                        ctrl_type = CTRL_ANALOG_TRACKBALL;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "trackball";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].analog = true;
                        break;

                    case ioport_type.IPT_MOUSE_X:
                    case ioport_type.IPT_MOUSE_Y:
                        ctrl_analog = true;
                        ctrl_type = CTRL_ANALOG_MOUSE;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "mouse";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].analog = true;
                        break;

                    // map buttons
                    case ioport_type.IPT_BUTTON1:
                    case ioport_type.IPT_BUTTON2:
                    case ioport_type.IPT_BUTTON3:
                    case ioport_type.IPT_BUTTON4:
                    case ioport_type.IPT_BUTTON5:
                    case ioport_type.IPT_BUTTON6:
                    case ioport_type.IPT_BUTTON7:
                    case ioport_type.IPT_BUTTON8:
                    case ioport_type.IPT_BUTTON9:
                    case ioport_type.IPT_BUTTON10:
                    case ioport_type.IPT_BUTTON11:
                    case ioport_type.IPT_BUTTON12:
                    case ioport_type.IPT_BUTTON13:
                    case ioport_type.IPT_BUTTON14:
                    case ioport_type.IPT_BUTTON15:
                    case ioport_type.IPT_BUTTON16:
                        ctrl_analog = false;
                        if (control_info[field.player() * CTRL_COUNT + ctrl_type].type == null)
                        {
                            control_info[field.player() * CTRL_COUNT + ctrl_type].type = "only_buttons";
                            control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                            control_info[field.player() * CTRL_COUNT + ctrl_type].analog = false;
                        }
                        control_info[field.player() * CTRL_COUNT + ctrl_type].maxbuttons = std.max(control_info[field.player() * CTRL_COUNT + ctrl_type].maxbuttons, field.type() - ioport_type.IPT_BUTTON1 + 1);
                        control_info[field.player() * CTRL_COUNT + ctrl_type].nbuttons++;
                        if (!field.optional())
                            control_info[field.player() * CTRL_COUNT + ctrl_type].reqbuttons++;
                        break;

                    // track maximum coin index
                    case ioport_type.IPT_COIN1:
                    case ioport_type.IPT_COIN2:
                    case ioport_type.IPT_COIN3:
                    case ioport_type.IPT_COIN4:
                    case ioport_type.IPT_COIN5:
                    case ioport_type.IPT_COIN6:
                    case ioport_type.IPT_COIN7:
                    case ioport_type.IPT_COIN8:
                    case ioport_type.IPT_COIN9:
                    case ioport_type.IPT_COIN10:
                    case ioport_type.IPT_COIN11:
                    case ioport_type.IPT_COIN12:
                        ncoin = std.max(ncoin, field.type() - ioport_type.IPT_COIN1 + 1);
                        break;

                    // track presence of keypads and keyboards
                    case ioport_type.IPT_KEYPAD:
                        ctrl_type = CTRL_DIGITAL_KEYPAD;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "keypad";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].nbuttons++;
                        if (!field.optional())
                            control_info[field.player() * CTRL_COUNT + ctrl_type].reqbuttons++;
                        break;

                    case ioport_type.IPT_KEYBOARD:
                        ctrl_type = CTRL_DIGITAL_KEYBOARD;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].type = "keyboard";
                        control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                        control_info[field.player() * CTRL_COUNT + ctrl_type].nbuttons++;
                        if (!field.optional())
                            control_info[field.player() * CTRL_COUNT + ctrl_type].reqbuttons++;
                        break;

                    // additional types
                    case ioport_type.IPT_SERVICE:
                        service = true;
                        break;

                    case ioport_type.IPT_TILT:
                        tilt = true;
                        break;

                    default:
                        if (field.type() > ioport_type.IPT_MAHJONG_FIRST && field.type() < ioport_type.IPT_MAHJONG_LAST)
                        {
                            ctrl_type = CTRL_DIGITAL_MAHJONG;
                            control_info[field.player() * CTRL_COUNT + ctrl_type].type = "mahjong";
                            control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                            control_info[field.player() * CTRL_COUNT + ctrl_type].nbuttons++;
                            if (!field.optional())
                                control_info[field.player() * CTRL_COUNT + ctrl_type].reqbuttons++;
                        }
                        else if (field.type() > ioport_type.IPT_HANAFUDA_FIRST && field.type() < ioport_type.IPT_HANAFUDA_LAST)
                        {
                            ctrl_type = CTRL_DIGITAL_HANAFUDA;
                            control_info[field.player() * CTRL_COUNT + ctrl_type].type = "hanafuda";
                            control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                            control_info[field.player() * CTRL_COUNT + ctrl_type].nbuttons++;
                            if (!field.optional())
                                control_info[field.player() * CTRL_COUNT + ctrl_type].reqbuttons++;
                        }
                        else if (field.type() > ioport_type.IPT_GAMBLING_FIRST && field.type() < ioport_type.IPT_GAMBLING_LAST)
                        {
                            ctrl_type = CTRL_DIGITAL_GAMBLING;
                            control_info[field.player() * CTRL_COUNT + ctrl_type].type = "gambling";
                            control_info[field.player() * CTRL_COUNT + ctrl_type].player = field.player() + 1;
                            control_info[field.player() * CTRL_COUNT + ctrl_type].nbuttons++;
                            if (!field.optional())
                                control_info[field.player() * CTRL_COUNT + ctrl_type].reqbuttons++;
                        }
                        break;
                    }

                    if (ctrl_analog)
                    {
                        // get the analog stats
                        if (field.minval() != 0)
                            control_info[field.player() * CTRL_COUNT + ctrl_type].min = (int32_t)field.minval();
                        if (field.maxval() != 0)
                            control_info[field.player() * CTRL_COUNT + ctrl_type].max = (int32_t)field.maxval();
                        if (field.sensitivity() != 0)
                            control_info[field.player() * CTRL_COUNT + ctrl_type].sensitivity = field.sensitivity();
                        if (field.delta() != 0)
                            control_info[field.player() * CTRL_COUNT + ctrl_type].keydelta = field.delta();
                        if (field.analog_reverse() != false)
                            control_info[field.player() * CTRL_COUNT + ctrl_type].reverse = true;
                    }
                }
            }

            // Clean-up those entries, if any, where buttons were defined in a separate port than the actual controller they belong to.
            // This is quite often the case, especially for arcades where controls can be easily mapped to separate input ports on PCB.
            // If such situation would only happen for joystick, it would be possible to work it around by initializing differently
            // ctrl_type above, but it is quite common among analog inputs as well (for instance, this is the tipical situation
            // for lightguns) and therefore we really need this separate loop.
            for (int i = 0; i < CTRL_PCOUNT; i++)
            {
                bool fix_done = false;
                for (int j = 1; j < CTRL_COUNT; j++)
                {
                    if (control_info[i * CTRL_COUNT].type != null && control_info[i * CTRL_COUNT + j].type != null && !fix_done)
                    {
                        control_info[i * CTRL_COUNT + j].nbuttons += control_info[i * CTRL_COUNT].nbuttons;
                        control_info[i * CTRL_COUNT + j].reqbuttons += control_info[i * CTRL_COUNT].reqbuttons;
                        control_info[i * CTRL_COUNT + j].maxbuttons = std.max(control_info[i * CTRL_COUNT + j].maxbuttons, control_info[i * CTRL_COUNT].maxbuttons);

                        control_info[i * CTRL_COUNT] = new control_info_struct();  //memset(&control_info[i * CTRL_COUNT], 0, sizeof(control_info[0]));
                        fix_done = true;
                    }
                }
            }

            // Output the input info
            // First basic info
            out_ += "\t\t<input";
            util.stream_format(ref out_, " players=\"{0}\"", nplayer);
            if (ncoin != 0)
                util.stream_format(ref out_, " coins=\"{0}\"", ncoin);
            if (service)
                util.stream_format(ref out_, " service=\"yes\"");
            if (tilt)
                util.stream_format(ref out_, " tilt=\"yes\"");
            out_ += ">\n";

            // Then controller specific ones
            foreach (var elem in control_info)
            {
                if (elem.type != null)
                {
                    //printf("type %s - player %d - buttons %d\n", elem.type, elem.player, elem.nbuttons);
                    if (elem.analog)
                    {
                        util.stream_format(ref out_, "\t\t\t<control type=\"{0}\"", normalize_string(elem.type));
                        if (nplayer > 1)
                            util.stream_format(ref out_, " player=\"{0}\"", elem.player);
                        if (elem.nbuttons > 0)
                        {
                            util.stream_format(ref out_, " buttons=\"{0}\"", std.strcmp(elem.type, "stick") != 0 ? elem.nbuttons : elem.maxbuttons);
                            if (elem.reqbuttons < elem.nbuttons)
                                util.stream_format(ref out_, " reqbuttons=\"{0}\"", elem.reqbuttons);
                        }
                        if (elem.min != 0 || elem.max != 0)
                            util.stream_format(ref out_, " minimum=\"{0}\" maximum=\"{0}\"", elem.min, elem.max);
                        if (elem.sensitivity != 0)
                            util.stream_format(ref out_, " sensitivity=\"{0}\"", elem.sensitivity);
                        if (elem.keydelta != 0)
                            util.stream_format(ref out_, " keydelta=\"{0}\"", elem.keydelta);
                        if (elem.reverse)
                            out_ += " reverse=\"yes\"";

                        out_ += "/>\n";
                    }
                    else
                    {
                        if (elem.helper[1] == 0 && elem.helper[2] != 0) { elem.helper[1] = elem.helper[2]; elem.helper[2] = 0; }
                        if (elem.helper[0] == 0 && elem.helper[1] != 0) { elem.helper[0] = elem.helper[1]; elem.helper[1] = 0; }
                        if (elem.helper[1] == 0 && elem.helper[2] != 0) { elem.helper[1] = elem.helper[2]; elem.helper[2] = 0; }
                        string joys = (elem.helper[2] != 0) ? "triple" : (elem.helper[1] != 0) ? "double" : "";
                        util.stream_format(ref out_, "\t\t\t<control type=\"{0}{1}\"", joys, normalize_string(elem.type));
                        if (nplayer > 1)
                            util.stream_format(ref out_, " player=\"{0}\"", elem.player);
                        if (elem.nbuttons > 0)
                        {
                            util.stream_format(ref out_, " buttons=\"{0}\"", std.strcmp(elem.type, "joy") != 0 ? elem.nbuttons : elem.maxbuttons);
                            if (elem.reqbuttons < elem.nbuttons)
                                util.stream_format(ref out_, " reqbuttons=\"{0}\"", elem.reqbuttons);
                        }
                        for (int lp = 0; lp < 3 && elem.helper[lp] != 0; lp++)
                        {
                            string plural = (lp==2) ? "3" : (lp==1) ? "2" : "";
                            string ways;
                            string helper;
                            switch (elem.helper[lp] & (DIR_UP | DIR_DOWN | DIR_LEFT | DIR_RIGHT))
                            {
                                case DIR_UP | DIR_DOWN | DIR_LEFT | DIR_RIGHT:
                                    helper = util.string_format("{0}", (elem.ways == 0) ? 8 : elem.ways);  //helper = util::string_format(std::locale::classic(), "%d", (elem.ways == 0) ? 8 : elem.ways);
                                    ways = helper;
                                    break;
                                case DIR_LEFT | DIR_RIGHT:
                                    ways = "2";
                                    break;
                                case DIR_UP | DIR_DOWN:
                                    ways = "vertical2";
                                    break;
                                case DIR_UP:
                                case DIR_DOWN:
                                case DIR_LEFT:
                                case DIR_RIGHT:
                                    ways = "1";
                                    break;
                                case DIR_UP | DIR_DOWN | DIR_LEFT:
                                case DIR_UP | DIR_DOWN | DIR_RIGHT:
                                case DIR_UP | DIR_LEFT | DIR_RIGHT:
                                case DIR_DOWN | DIR_LEFT | DIR_RIGHT:
                                    ways = (elem.ways == 4) ? "3 (half4)" : "5 (half8)";
                                    break;
                                default:
                                    ways = "strange2";
                                    break;
                            }
                            util.stream_format(ref out_, " ways{0}=\"{1}\"", plural, ways);
                        }
                        out_ += "/>\n";
                    }
                }
            }

            out_ += "\t\t</input>\n";
        }


        //-------------------------------------------------
        //  output_switches - print the configurations or
        //  DIP switch settings
        //-------------------------------------------------
        void output_switches(ref string out_, ioport_list portlist, string root_tag, int type, string outertag, string loctag, string innertag)
        {
            // iterate looking for DIP switches
            foreach (var port in portlist)
            {
                foreach (ioport_field field in port.second().fields())
                {
                    if ((int)field.type() == type)
                    {
                        string newtag = port.second().tag();
                        string oldtag = ":";
                        newtag = newtag.substr(newtag.find(oldtag = oldtag.append_(root_tag)) + oldtag.length());

                        // output the switch name information
                        string normalized_field_name = normalize_string(field.specific_name());
                        string normalized_newtag = normalize_string(newtag);
                        util.stream_format(ref out_, "\t\t<{0} name=\"{1}\" tag=\"{2}\" mask=\"{3}\">\n", outertag, normalized_field_name, normalized_newtag, field.mask());
                        if (!field.condition().none())
                            output_ioport_condition(ref out_, field.condition(), 3);

                        // loop over locations
                        foreach (ioport_diplocation diploc in field.diplocations())
                        {
                            util.stream_format(ref out_, "\t\t\t<{0} name=\"{1}\" number=\"{2}\"", loctag, normalize_string(diploc.name()), diploc.number());
                            if (diploc.inverted())
                                out_ += " inverted=\"yes\"";
                            out_ += "/>\n";
                        }

                        // loop over settings
                        foreach (ioport_setting setting in field.settings())
                        {
                            util.stream_format(ref out_, "\t\t\t<{0} name=\"{1}\" value=\"{2}\"", innertag, normalize_string(setting.name()), setting.value());
                            if (setting.value() == field.defvalue())
                                out_ += " default=\"yes\"";
                            if (setting.condition().none())
                            {
                                out_ += "/>\n";
                            }
                            else
                            {
                                out_ += ">\n";
                                output_ioport_condition(ref out_, setting.condition(), 4);
                                util.stream_format(ref out_, "\t\t\t</{0}>\n", innertag);
                            }
                        }

                        // terminate the switch entry
                        util.stream_format(ref out_, "\t\t</{0}>\n", outertag);
                    }
                }
            }
        }


        //-------------------------------------------------
        //  output_ports - print the structure of input
        //  ports in the driver
        //-------------------------------------------------
        void output_ports(ref string out_, ioport_list portlist)
        {
            // cycle through ports
            foreach (var port in portlist)
            {
                util.stream_format(ref out_, "\t\t<port tag=\"{0}\">\n", normalize_string(port.second().tag()));
                foreach (ioport_field field in port.second().fields())
                {
                    if (field.is_analog())
                        util.stream_format(ref out_, "\t\t\t<analog mask=\"{0}\"/>\n", field.mask());
                }
                util.stream_format(ref out_, "\t\t</port>\n");
            }
        }


        //-------------------------------------------------
        //  output_adjusters - print the Analog
        //  Adjusters for a game
        //-------------------------------------------------
        void output_adjusters(ref string out_, ioport_list portlist)
        {
            // iterate looking for Adjusters
            foreach (var port in portlist)
            {
                foreach (ioport_field field in port.second().fields())
                {
                    if (field.type() == ioport_type.IPT_ADJUSTER)
                    {
                        util.stream_format(ref out_, "\t\t<adjuster name=\"{0}\" default=\"{1}\"/>\n", normalize_string(field.specific_name()), field.defvalue());
                    }
                }
            }
        }


        //-------------------------------------------------
        //  output_driver - print driver status
        //-------------------------------------------------
        void output_driver(ref string out_, game_driver driver, device_t_feature_type unemulated, device_t_feature_type imperfect)
        {
            out_ += "\t\t<driver";

            /*
            The status entry is an hint for frontend authors to select working
            and not working games without the need to know all the other status
            entries.  Games marked as status=good are perfectly emulated, games
            marked as status=imperfect are emulated with only some minor issues,
            games marked as status=preliminary don't work or have major
            emulation problems.
            */

            u32 flags = (u32)driver.flags;
            bool machine_preliminary = (flags & (u32)(machine_flags.type.NOT_WORKING | machine_flags.type.MECHANICAL)) != 0;
            bool unemulated_preliminary = (unemulated & (device_t_feature_type.PALETTE | device_t_feature_type.GRAPHICS | device_t_feature_type.SOUND | device_t_feature_type.KEYBOARD)) != 0;
            bool imperfect_preliminary = ((unemulated | imperfect) & device_t_feature_type.PROTECTION) != 0;

            if (machine_preliminary || unemulated_preliminary || imperfect_preliminary)
                out_ += " status=\"preliminary\"";
            else if (imperfect != 0)
                out_ += " status=\"imperfect\"";
            else
                out_ += " status=\"good\"";

            if ((flags & (u32)machine_flags.type.NOT_WORKING) != 0)
                out_ += " emulation=\"preliminary\"";
            else
                out_ += " emulation=\"good\"";

            if ((flags & (u32)machine_flags.type.NO_COCKTAIL) != 0)
                out_ += " cocktail=\"preliminary\"";

            if ((flags & (u32)machine_flags.type.SUPPORTS_SAVE) != 0)
                out_ += " savestate=\"supported\"";
            else
                out_ += " savestate=\"unsupported\"";

            if ((flags & (u32)machine_flags.type.REQUIRES_ARTWORK) != 0)
                out_ += " requiresartwork=\"yes\"";

            if ((flags & (u32)machine_flags.type.UNOFFICIAL) != 0)
                out_ += " unofficial=\"yes\"";

            if ((flags & (u32)machine_flags.type.NO_SOUND_HW) != 0)
                out_ += " nosoundhardware=\"yes\"";

            if ((flags & (u32)machine_flags.type.IS_INCOMPLETE) != 0)
                out_ += " incomplete=\"yes\"";

            out_ += "/>\n";
        }


        //-------------------------------------------------
        //  output_features - print emulation features of
        //
        //-------------------------------------------------
        void output_features(ref string out_, device_type type, device_t_feature_type unemulated, device_t_feature_type imperfect)
        {
            device_t_feature_type flags = type.unemulated_features() | type.imperfect_features() | unemulated | imperfect;
            foreach (var feature in f_feature_names)
            {
                if ((flags & feature.first) != 0)
                {
                    util.stream_format(ref out_, "\t\t<feature type=\"{0}\"", feature.second);
                    if ((type.unemulated_features() & feature.first) != 0)
                    {
                        out_ += " status=\"unemulated\"";
                    }
                    else
                    {
                        if ((type.imperfect_features() & feature.first) != 0)
                            out_ += " status=\"imperfect\"";
                        if ((unemulated & feature.first) != 0)
                            out_ += " overall=\"unemulated\"";
                        else if (((~type.imperfect_features() & imperfect) & feature.first) != 0)
                            out_ += " overall=\"imperfect\"";
                    }
                    out_ += "/>\n";
                }
            }
        }


        //-------------------------------------------------
        //  output_images - prints m_output all info on
        //  image devices
        //-------------------------------------------------
        void output_images(ref string out_, device_t device, string root_tag)
        {
            foreach (device_image_interface imagedev in new image_interface_enumerator(device))
            {
                if (std.strcmp(imagedev.device().tag(), device.tag()) != 0)
                {
                    bool loadable = imagedev.user_loadable();
                    string newtag = imagedev.device().tag();
                    string oldtag = ":";
                    newtag = newtag.substr(newtag.find(oldtag = oldtag.append_(root_tag)) + oldtag.length());

                    // print m_output device type
                    util.stream_format(ref out_, "\t\t<device type=\"{0}\"", normalize_string(imagedev.image_type_name()));

                    // does this device have a tag?
                    if (!string.IsNullOrEmpty(imagedev.device().tag()))
                        util.stream_format(ref out_, " tag=\"{0}\"", normalize_string(newtag));

                    // is this device available as media switch?
                    if (!loadable)
                        out_ += " fixed_image=\"1\"";

                    // is this device mandatory?
                    if (imagedev.must_be_loaded())
                        out_ += " mandatory=\"1\"";

                    if (!string.IsNullOrEmpty(imagedev.image_interface()))
                        util.stream_format(ref out_, " interface=\"{0}\"", normalize_string(imagedev.image_interface()));

                    // close the XML tag
                    out_ += ">\n";

                    if (loadable)
                    {
                        string name = imagedev.instance_name();
                        string shortname = imagedev.brief_instance_name();

                        out_ += "\t\t\t<instance";
                        util.stream_format(ref out_, " name=\"{0}\"", normalize_string(name));
                        util.stream_format(ref out_, " briefname=\"{0}\"", normalize_string(shortname));
                        out_ += "/>\n";

                        string extensions = imagedev.file_extensions();
                        while (!string.IsNullOrEmpty(extensions))
                        {
                            int endIdx = 0;  //char const *end(extensions);
                            while (endIdx < extensions.Length && (',' != extensions[endIdx]))  //while (*end && (',' != *end))
                                ++endIdx;  //++end;

                            util.stream_format(ref out_, "\t\t\t<extension name=\"{0}\"/>\n", normalize_string(extensions[..endIdx]));  //std::string_view(extensions, end - extensions)));
                            extensions = endIdx < extensions.Length ? extensions[(endIdx + 1)..] : null;  //extensions = *end ? (end + 1) : nullptr;
                        }
                    }
                    out_ += "\t\t</device>\n";
                }
            }
        }


        //-------------------------------------------------
        //  output_slots - prints all info about slots
        //-------------------------------------------------
        void output_slots(ref string out_, machine_config config, device_t device, string root_tag, device_type_set devtypes)
        {
            foreach (device_slot_interface slot in new slot_interface_enumerator(device))
            {
                // shall we list fixed slots as non-configurable?
                bool listed = !slot.fixed_() && std.strcmp(slot.device().tag(), device.tag()) != 0;

                if (devtypes != null || listed)
                {
                    machine_config.token tok = config.begin_configuration(slot.device());
                    string newtag = slot.device().tag();
                    string oldtag = ":";
                    newtag = newtag.substr(newtag.find(oldtag = oldtag.append_(root_tag)) + oldtag.length());

                    // print m_output device type
                    if (listed)
                        util.stream_format(ref out_, "\t\t<slot name=\"{0}\">\n", normalize_string(newtag));

                    foreach (var option in slot.option_list())
                    {
                        if (devtypes != null || (listed && option.second().selectable()))
                        {
                            device_t dev = config.device_add("_dummy", option.second().devtype(), option.second().clock());
                            if (!dev.configured())
                                dev.config_complete();

                            if (devtypes != null)
                                foreach (device_t subdevice in new device_enumerator(dev)) devtypes.insert(subdevice.type());

                            if (listed && option.second().selectable())
                            {
                                util.stream_format(ref out_, "\t\t\t<slotoption name=\"{0}\"", normalize_string(option.second().name()));
                                util.stream_format(ref out_, " devname=\"{0}\"", normalize_string(dev.shortname()));
                                if (!string.IsNullOrEmpty(slot.default_option()) && std.strcmp(slot.default_option(), option.second().name()) == 0)
                                    out_ += " default=\"yes\"";
                                out_ += "/>\n";
                            }

                            config.device_remove("_dummy");
                        }
                    }

                    if (listed)
                        out_ += "\t\t</slot>\n";
                }
            }
        }


        //-------------------------------------------------
        //  output_software_lists - print the information
        //  for all known software lists for this system
        //-------------------------------------------------
        void output_software_lists(ref string out_, device_t root, string root_tag)
        {
            foreach (software_list_device swlist in new software_list_device_enumerator(root))
            {
                if ((device_t)swlist == root)
                {
                    assert(swlist.list_name().empty());
                    continue;
                }

                string newtag = swlist.tag();
                string oldtag = ":";
                newtag = newtag.substr(newtag.find(oldtag = oldtag.append_(root_tag)) + oldtag.length());
                util.stream_format(ref out_, "\t\t<softwarelist tag=\"{0}\" name=\"{1}\" status=\"{2}\"", normalize_string(newtag), normalize_string(swlist.list_name()), swlist.is_original() ? "original" : "compatible");

                if (!string.IsNullOrEmpty(swlist.filter()))
                    util.stream_format(ref out_, " filter=\"{0}\"", normalize_string(swlist.filter()));
                out_ += "/>\n";
            }
        }


        //-------------------------------------------------
        //  output_ramoptions - prints m_output all RAM
        //  options for this system
        //-------------------------------------------------
        void output_ramoptions(ref string out_, device_t root)
        {
            //throw new emu_unimplemented();
#if false
            for (const ram_device &ram : ram_device_enumerator(root, 1))
            {
                if (!std::strcmp(ram.tag(), ":" RAM_TAG))
                {
                    uint32_t const defsize(ram.default_size());
                    bool havedefault(false);
                    for (ram_device::extra_option const &option : ram.extra_options())
                    {
                        if (defsize == option.second)
                        {
                            assert(!havedefault);
                            havedefault = true;
                            util::stream_format(out, "\t\t<ramoption name=\"%s\" default=\"yes\">%u</ramoption>\n", normalize_string(option.first), option.second);
                        }
                        else
                        {
                            util::stream_format(out, "\t\t<ramoption name=\"%s\">%u</ramoption>\n", normalize_string(option.first), option.second);
                        }
                    }
                    if (!havedefault)
                        util::stream_format(out, "\t\t<ramoption name=\"%s\" default=\"yes\">%u</ramoption>\n", ram.default_size_string(), defsize);
                    break;
                }
            }
#endif
        }


        //-------------------------------------------------
        //  get_merge_name - get the rom name from a
        //  parent set
        //-------------------------------------------------
        string get_merge_name(driver_list drivlist, game_driver driver, util.hash_collection romhashes)
        {
            string result = null;

            // walk the parent chain
            for (int clone_of = driver_list.find(driver.parent); string.IsNullOrEmpty(result) && (0 <= clone_of); clone_of = driver_list.find(driver_list.driver((size_t)clone_of).parent))
                result = get_merge_name(driver_list.driver((size_t)clone_of).rom, romhashes);

            return result;
        }


        string get_merge_name(machine_config config, device_t device, util.hash_collection romhashes)
        {
            throw new emu_unimplemented();
#if false
            char const *result = nullptr;

            // check for a parent type
            auto const parenttype(device.type().parent_rom_device_type());
            if (parenttype)
            {
                // instantiate the parent device
                machine_config::token const tok(config.begin_configuration(config.root_device()));
                device_t *const parent = config.device_add("_parent", *parenttype, 0);

                // look in the parent's ROMs
                result = get_merge_name(parent->rom_region(), romhashes);

                // remember to remove the device
                config.device_remove("_parent");
            }

            return result;
#endif
        }


        string get_merge_name(Pointer<tiny_rom_entry> roms, util.hash_collection romhashes)
        {
            foreach (romload.region pregion in new romload.entries(roms).get_regions())
            {
                foreach (romload.file prom in pregion.get_files())
                {
                    // stop when we find a match
                    util.hash_collection phashes = new util.hash_collection(prom.get_hashdata());
                    if (!phashes.flag(util.hash_collection.FLAG_NO_DUMP) && (romhashes == phashes))
                        return prom.get_name();
                }
            }

            return null;
        }
    }
}
