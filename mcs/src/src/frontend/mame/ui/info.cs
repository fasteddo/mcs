// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_t_feature = mame.emu.detail.device_feature;  //using feature = emu::detail::device_feature;
using device_t_feature_type = mame.emu.detail.device_feature.type;  //using feature_type = emu::detail::device_feature::type;
using execute_interface_enumerator = mame.device_interface_enumerator<mame.device_execute_interface>;  //typedef device_interface_enumerator<device_execute_interface> execute_interface_enumerator;
using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using size_t = System.UInt64;
using sound_interface_enumerator = mame.device_interface_enumerator<mame.device_sound_interface>;  //typedef device_interface_enumerator<device_sound_interface> sound_interface_enumerator;
using u32 = System.UInt32;

using static mame.cpp_global;
using static mame.emucore_global;
using static mame.language_global;
using static mame.romload_global;
using static mame.ui.info_internal;
using static mame.ui_global;
using static mame.util;


namespace mame.ui
{
    public class machine_static_info
    {
        ui_options m_options;

        // overall feature status
        machine_flags.type m_flags;
        device_t_feature_type m_unemulated_features;
        device_t_feature_type m_imperfect_features;

        // has...
        bool m_has_bioses;

        // has input types
        bool m_has_dips;
        bool m_has_configs;
        bool m_has_keyboard;
        bool m_has_test_switch;
        bool m_has_analog;


        //-------------------------------------------------
        //  machine_static_info - constructors
        //-------------------------------------------------
        public machine_static_info(ui_options options, machine_config config)
            : this(options, config, null)
        {
        }


        protected machine_static_info(ui_options options, machine_config config, ioport_list ports)
        {
            m_options = options;
            m_flags = config.gamedrv().flags;
            m_unemulated_features = config.gamedrv().type.unemulated_features();
            m_imperfect_features = config.gamedrv().type.imperfect_features();
            m_has_bioses = false;
            m_has_dips = false;
            m_has_configs = false;
            m_has_keyboard = false;
            m_has_test_switch = false;
            m_has_analog = false;


            ioport_list local_ports = new ioport_list();
            string sink;
            foreach (device_t device in new device_enumerator(config.root_device()))
            {
                // the "no sound hardware" warning doesn't make sense when you plug in a sound card
                if (device is speaker_device)  //if (dynamic_cast<speaker_device *>(&device))
                    m_flags &= ~machine_flags.type.NO_SOUND_HW;

                // build overall emulation status
                m_unemulated_features |= device.type().unemulated_features();
                m_imperfect_features |= device.type().imperfect_features();

                // look for BIOS options
                device_t parent = device.owner();
                device_slot_interface slot = device.GetClassInterface<device_slot_interface>();  //device_slot_interface const *const slot(dynamic_cast<device_slot_interface const *>(parent));
                if (parent == null || (slot != null && (slot.get_card_device() == device)))
                {
                    for (Pointer<tiny_rom_entry> rom = device.rom_region(); !m_has_bioses && rom != null && !ROMENTRY_ISEND(rom); ++rom)  //for (tiny_rom_entry const *rom = device.rom_region(); !m_has_bioses && rom && !ROMENTRY_ISEND(rom); ++rom)
                    {
                        if (ROMENTRY_ISSYSTEM_BIOS(rom))
                            m_has_bioses = true;
                    }
                }
            }

            // suppress "requires external artwork" warning when external artwork was loaded
            if (config.root_device().has_running_machine())
            {
                foreach (render_target target in config.root_device().machine().render().targets())
                {
                    if (!target.hidden() && target.external_artwork())
                    {
                        m_flags &= ~machine_flags.type.REQUIRES_ARTWORK;
                        break;
                    }
                }
            }

            // unemulated trumps imperfect when aggregating (always be pessimistic)
            m_imperfect_features &= ~m_unemulated_features;

            // scan the input port array to see what options we need to enable
            foreach (var port in (ports != null ? ports : local_ports))
            {
                foreach (ioport_field field in port.Value.fields())
                {
                    switch (field.type())
                    {
                    case ioport_type.IPT_DIPSWITCH: m_has_dips = true;          break;
                    case ioport_type.IPT_CONFIG:    m_has_configs = true;       break;
                    case ioport_type.IPT_KEYBOARD:  m_has_keyboard = true;      break;
                    case ioport_type.IPT_SERVICE:   m_has_test_switch = true;   break;
                    default: break;
                    }

                    if (field.is_analog())
                        m_has_analog = true;
                }
            }
        }


        // overall emulation status
        public machine_flags.type machine_flags_() { return m_flags; }
        public device_t_feature_type unemulated_features() { return m_unemulated_features; }
        public device_t_feature_type imperfect_features() { return m_imperfect_features; }


        // has... getters
        public bool has_bioses() { return m_has_bioses; }

        // has input types getters
        public bool has_dips() { return m_has_dips; }
        public bool has_configs() { return m_has_configs; }
        public bool has_keyboard() { return m_has_keyboard; }
        public bool has_test_switch() { return m_has_test_switch; }
        public bool has_analog() { return m_has_analog; }


        // warning severity indications
        //bool has_warnings() const;


        //-------------------------------------------------
        //  has_severe_warnings - returns true if the
        //  system has issues that warrant a red message
        //-------------------------------------------------
        public bool has_severe_warnings()
        {
            return
                    (machine_flags_() & MACHINE_ERRORS) != 0 ||
                    (unemulated_features() & (device_t_feature.type.PROTECTION | device_t_feature.type.GRAPHICS | device_t_feature.type.SOUND)) != 0 ||
                    (imperfect_features() & device_t_feature.type.PROTECTION) != 0;
        }


        //-------------------------------------------------
        //  status_color - returns suitable colour for
        //  driver status box
        //-------------------------------------------------
        public rgb_t status_color()
        {
            if (has_severe_warnings())
                return UI_RED_COLOR;
            else if ((machine_flags_() & MACHINE_WARNINGS & ~machine_flags.type.REQUIRES_ARTWORK) != 0 || unemulated_features() != 0 || imperfect_features() != 0)
                return UI_YELLOW_COLOR;
            else
                return UI_GREEN_COLOR;
        }


        //-------------------------------------------------
        //  warnings_color - returns suitable colour for
        //  warning message based on severity
        //-------------------------------------------------
        public rgb_t warnings_color()
        {
            if (has_severe_warnings())
                return UI_RED_COLOR;
            else if ((machine_flags_() & MACHINE_WARNINGS) != 0 || unemulated_features() != 0 || imperfect_features() != 0)
                return UI_YELLOW_COLOR;
            else
                return m_options.background_color();
        }
    }


    public class machine_info : machine_static_info
    {
        // reference to machine
        running_machine m_machine;

        //-------------------------------------------------
        //  machine_info - constructor
        //-------------------------------------------------
        public machine_info(running_machine machine)
            : base(((mame_ui_manager)machine.ui()).options(), machine.config(), machine.ioport().ports())
        {
            m_machine = machine;
        }


        // text generators

        //-------------------------------------------------
        //  warnings_string - print the warning flags
        //  text to the given buffer
        //-------------------------------------------------
        public string warnings_string()
        {
            string buf = "";

            get_general_warnings(ref buf, m_machine, machine_flags_(), unemulated_features(), imperfect_features());
            get_system_warnings(ref buf, m_machine, machine_flags_(), unemulated_features(), imperfect_features());

            return buf;
        }


        //-------------------------------------------------
        //  game_info_string - return the game info text
        //-------------------------------------------------
        public string game_info_string()
        {
            string buf = "";  //std::ostringstream buf;

            // print description, manufacturer, and CPU:
            string src = m_machine.system().type.source();
            var prefix = src.find("src/mame/");
            if (npos == prefix)
                prefix = src.find("src\\mame\\");
            if (npos != prefix)
                src = src.remove_prefix_(prefix + 9);
            util.stream_format(ref buf, __("{0}\n{1} {2}\nDriver: {3}\n\nCPU:\n"),  //util::stream_format(buf, _("%1$s\n%2$s %3$s\nDriver: %4$s\n\nCPU:\n"),
                    system_list.instance().systems()[driver_list.find(m_machine.system().name)].description,
                    m_machine.system().year,
                    m_machine.system().manufacturer,
                    src);

            // loop over all CPUs
            execute_interface_enumerator execiter = new execute_interface_enumerator(m_machine.root_device());
            std.unordered_set<string> exectags = new std.unordered_set<string>();
            foreach (device_execute_interface exec in execiter)
            {
                if (!exectags.insert(exec.device().tag()))  //.second)
                    continue;

                // get cpu specific clock that takes internal multiplier/dividers into account
                u32 clock = exec.device().clock();

                // count how many identical CPUs we have
                int count = 1;
                string name = exec.device().name();
                foreach (device_execute_interface scan in execiter)
                {
                    if (exec.device().type() == scan.device().type() && std.strcmp(name, scan.device().name()) == 0 && exec.device().clock() == scan.device().clock())
                        if (exectags.insert(scan.device().tag()))  //.second)
                            count++;
                }

                string hz = std.to_string(clock);
                int d = (clock >= 1000000000) ? 9 : (clock >= 1000000) ? 6 : (clock >= 1000) ? 3 : 0;
                if (d > 0)
                {
                    size_t dpos = hz.length() - (size_t)d;
                    hz = hz.insert_(dpos, ".");
                    size_t last = hz.find_last_not_of('0');
                    hz = hz.substr(0, last + (last != dpos ? 1U : 0U));
                }

                // if more than one, prepend a #x in front of the CPU name and display clock
                util.stream_format(ref buf,
                        (count > 1)
                            ? ((clock != 0) ? "{0}X{1} {2} {3}\n" : "{1}x{2}\n")  //? ((clock != 0) ? "%1$d" UTF8_MULTIPLY "%2$s %3$s" UTF8_NBSP "%4$s\n" : "%1$d" UTF8_MULTIPLY "%2$s\n")
                            : ((clock != 0) ? "{1} {2} {3}\n" : "{1}\n"),  //: ((clock != 0) ? "%2$s %3$s" UTF8_NBSP "%4$s\n" : "%2$s\n"),
                        count, name, hz,
                        (d == 9) ? __("GHz") : (d == 6) ? __("MHz") : (d == 3) ? __("kHz") : __("Hz"));
            }

            // loop over all sound chips
            sound_interface_enumerator snditer = new sound_interface_enumerator(m_machine.root_device());
            std.unordered_set<string> soundtags = new std.unordered_set<string>();
            bool found_sound = false;
            foreach (device_sound_interface sound in snditer)
            {
                if (!sound.issound() || !soundtags.insert(sound.device().tag()))  //.second)
                    continue;

                // append the Sound: string
                if (!found_sound)
                    buf += __("\nSound:\n");

                found_sound = true;

                // count how many identical sound chips we have
                int count = 1;
                foreach (device_sound_interface scan in snditer)
                {
                    if (sound.device().type() == scan.device().type() && sound.device().clock() == scan.device().clock())
                        if (soundtags.insert(scan.device().tag()))  //.second)
                            count++;
                }

                u32 clock = sound.device().clock();
                string hz = std.to_string(clock);
                int d = (clock >= 1000000000) ? 9 : (clock >= 1000000) ? 6 : (clock >= 1000) ? 3 : 0;
                if (d > 0)
                {
                    size_t dpos = hz.length() - (size_t)d;
                    hz = hz.insert_(dpos, ".");
                    size_t last = hz.find_last_not_of('0');
                    hz = hz.substr(0, last + (last != dpos ? 1U : 0U));
                }

                // if more than one, prepend a #x in front of the soundchip name and display clock
                util.stream_format(ref buf,
                        (count > 1)
                            ? ((clock != 0) ? "{0}X{1} {2} {3}\n" : "{0}X{1}\n")  //? ((clock != 0) ? "%1$d" UTF8_MULTIPLY "%2$s %3$s" UTF8_NBSP "%4$s\n" : "%1$d" UTF8_MULTIPLY "%2$s\n")
                            : ((clock != 0) ? "{1} {2} {3}\n" : "{1}\n"),  //: ((clock != 0) ? "%2$s %3$s" UTF8_NBSP "%4$s\n" : "%2$s\n"),
                        count, sound.device().name(), hz,
                        (d == 9) ? __("GHz") : (d == 6) ? __("MHz") : (d == 3) ? __("kHz") : __("Hz"));
            }

            // display screen information
            buf += __("\nVideo:\n");
            screen_device_enumerator scriter = new screen_device_enumerator(m_machine.root_device());
            int scrcount = scriter.count();
            if (scrcount == 0)
            {
                buf += __("None\n");
            }
            else
            {
                foreach (screen_device screen in scriter)
                {
                    string detail;
                    if (screen.screen_type() == screen_type_enum.SCREEN_TYPE_VECTOR)
                    {
                        detail = __("Vector");
                    }
                    else
                    {
                        u32 rate = (u32)(screen.frame_period().as_hz() * 1_000_000 + 0.5);
                        bool valid = rate >= 1_000_000;
                        string hz = valid ? std.to_string(rate) : "?";
                        if (valid)
                        {
                            size_t dpos = hz.length() - 6;
                            hz = hz.insert_(dpos, ".");
                            size_t last = hz.find_last_not_of('0');
                            hz = hz.substr(0, last + (last != dpos ? 1U : 0U));
                        }

                        rectangle visarea = screen.visible_area();
                        detail = util.string_format("{0} X {1} ({2}) {3} Hz",  //detail = string_format("%d " UTF8_MULTIPLY " %d (%s) %s" UTF8_NBSP "Hz",
                                visarea.width(), visarea.height(),
                                (screen.orientation() & ORIENTATION_SWAP_XY) != 0 ? "V" : "H",
                                hz);
                    }

                    util.stream_format(ref buf,
                            (scrcount > 1) ? __("{0}: {1}\n") : __("{1}\n"),  //(scrcount > 1) ? _("%1$s: %2$s\n") : _("%2$s\n"),
                            get_screen_desc(screen), detail);
                }
            }

            return buf;
        }


        //-------------------------------------------------
        //  get_screen_desc - returns the description for
        //  a given screen
        //-------------------------------------------------
        protected string get_screen_desc(screen_device screen)
        {
            if (new screen_device_enumerator(m_machine.root_device()).count() > 1)
                return string_format(__("Screen '{0}'"), screen.tag());
            else
                return __("Screen");
        }
    }


    //class menu_game_info : public menu_textbox


    //class menu_warn_info : public menu_textbox


    //class menu_image_info : public menu


    static class info_internal
    {
        public const machine_flags.type MACHINE_ERRORS    = machine_flags.type.NOT_WORKING | machine_flags.type.MECHANICAL;
        public const machine_flags.type MACHINE_WARNINGS  = machine_flags.type.NO_COCKTAIL | machine_flags.type.REQUIRES_ARTWORK;
        public const machine_flags.type MACHINE_BTANB     = machine_flags.type.NO_SOUND_HW | machine_flags.type.IS_INCOMPLETE;


        public static readonly std.pair<device_t_feature_type, string> [] FEATURE_NAMES =
        {
            new std.pair<device_t_feature_type, string>(device_t_feature_type.PROTECTION,    N_p("emulation-feature",    "protection")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.TIMING,        N_p("emulation-feature",    "timing")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.GRAPHICS,      N_p("emulation-feature",    "graphics")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.PALETTE,       N_p("emulation-feature",    "color palette")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.SOUND,         N_p("emulation-feature",    "sound")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.CAPTURE,       N_p("emulation-feature",    "capture hardware")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.CAMERA,        N_p("emulation-feature",    "camera")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.MICROPHONE,    N_p("emulation-feature",    "microphone")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.CONTROLS,      N_p("emulation-feature",    "controls")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.KEYBOARD,      N_p("emulation-feature",    "keyboard")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.MOUSE,         N_p("emulation-feature",    "mouse")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.MEDIA,         N_p("emulation-feature",    "media")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.DISK,          N_p("emulation-feature",    "disk")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.PRINTER,       N_p("emulation-feature",    "printer")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.TAPE,          N_p("emulation-feature",    "magnetic tape")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.PUNCH,         N_p("emulation-feature",    "punch tape")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.DRUM,          N_p("emulation-feature",    "magnetic drum")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.ROM,           N_p("emulation-feature",    "solid state storage")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.COMMS,         N_p("emulation-feature",    "communications")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.LAN,           N_p("emulation-feature",    "LAN")),
            new std.pair<device_t_feature_type, string>(device_t_feature_type.WAN,           N_p("emulation-feature",    "WAN"))
        };


        public static void get_general_warnings(ref string buf, running_machine machine, machine_flags.type flags, device_t_feature_type unemulated, device_t_feature_type imperfect)  //void get_general_warnings(std::ostream &buf, running_machine &machine, machine_flags::type flags, device_t::feature_type unemulated, device_t::feature_type imperfect)
        {
            // add a warning if any ROMs were loaded with warnings
            bool bad_roms = false;
            if (machine.rom_load().warnings() > 0)
            {
                bad_roms = true;
                buf += __("One or more ROMs/disk images for this system are incorrect. The system may not run correctly.\n");
            }
            if (!machine.rom_load().software_load_warnings_message().empty())
            {
                bad_roms = true;
                buf += machine.rom_load().software_load_warnings_message();
            }

            // if we have at least one warning flag, print the general header
            if ((machine.rom_load().knownbad() > 0) || (flags & (MACHINE_ERRORS | MACHINE_WARNINGS | MACHINE_BTANB)) != 0 || unemulated != 0 || imperfect != 0)
            {
                if (bad_roms)
                    buf += '\n';
                buf += __("There are known problems with this system\n\n");
            }

            // add a warning if any ROMs are flagged BAD_DUMP/NO_DUMP
            if (machine.rom_load().knownbad() > 0)
                buf += __("One or more ROMs/disk images for this system have not been correctly dumped.\n");
        }


        static void get_device_warnings(ref string buf, device_t_feature_type unemulated, device_t_feature_type imperfect)
        {
            // add line for unemulated features
            if (unemulated != 0)
            {
                buf += __("Completely unemulated features: ");
                bool first = true;
                foreach (var feature in FEATURE_NAMES)
                {
                    if (unemulated != 0 & feature.first != 0)
                    {
                        util.stream_format(ref buf, first ? __("{0}") : __(", {0}"), __("emulation-feature", feature.second));
                        first = false;
                    }
                }

                buf += '\n';
            }

            // add line for imperfect features
            if (imperfect != 0)
            {
                buf += __("Imperfectly emulated features: ");
                bool first = true;
                foreach (var feature in FEATURE_NAMES)
                {
                    if (imperfect != 0 & feature.first != 0)
                    {
                        util.stream_format(ref buf, first ? __("{0}") : __(", {0}"), __("emulation-feature", feature.second));
                        first = false;
                    }
                }

                buf += '\n';
            }
        }


        public static void get_system_warnings(ref string buf, running_machine machine, machine_flags.type flags, device_t_feature_type unemulated, device_t_feature_type imperfect)
        {
            // start with the unemulated/imperfect features
            get_device_warnings(ref buf, unemulated, imperfect);

            // add one line per machine warning flag
            if ((flags & machine_flags.type.NO_COCKTAIL) != 0)
                buf += __("Screen flipping in cocktail mode is not supported.\n");
            if ((flags & machine_flags.type.REQUIRES_ARTWORK) != 0)
                buf += __("This system requires external artwork files.\n");
            if ((flags & machine_flags.type.IS_INCOMPLETE) != 0)
                buf += __("This system was never completed. It may exhibit strange behavior or missing elements that are not bugs in the emulation.\n");
            if ((flags & machine_flags.type.NO_SOUND_HW) != 0)
                buf += __("This system has no sound hardware, MAME will produce no sounds, this is expected behaviour.\n");

            // these are more severe warnings
            if ((flags & machine_flags.type.NOT_WORKING) != 0)
                buf += __("\nTHIS SYSTEM DOESN'T WORK. The emulation for this system is not yet complete. There is nothing you can do to fix this problem except wait for the developers to improve the emulation.\n");
            if ((flags & machine_flags.type.MECHANICAL) != 0)
                buf += __("\nElements of this system cannot be emulated as they require physical interaction or consist of mechanical devices. It is not possible to fully experience this system.\n");

            if ((flags & MACHINE_ERRORS) != 0 || ((machine.system().type.unemulated_features() | machine.system().type.imperfect_features()) & device_t_feature.type.PROTECTION) != 0)
            {
                // find the parent of this driver
                driver_enumerator drivlist = new driver_enumerator(machine.options());
                int maindrv = driver_list.find(machine.system());
                int clone_of = driver_list.non_bios_clone((size_t)maindrv);
                if (clone_of != -1)
                    maindrv = clone_of;

                // scan the driver list for any working clones and add them
                bool foundworking = false;
                while (drivlist.next())
                {
                    if (drivlist.current() == maindrv || drivlist.clone() == maindrv)
                    {
                        game_driver driver = drivlist.driver();
                        if ((driver.flags & MACHINE_ERRORS) == 0 && ((driver.type.unemulated_features() | driver.type.imperfect_features()) & device_t_feature.type.PROTECTION) == 0)
                        {
                            // this one works, add a header and display the name of the clone
                            if (!foundworking)
                                util.stream_format(ref buf, __("\n\nThere are working clones of this system: {0}"), driver.name);
                            else
                                util.stream_format(ref buf, __(", {0}"), driver.name);

                            foundworking = true;
                        }
                    }
                }

                if (foundworking)
                    buf += '\n';
            }
        }
    }
}
