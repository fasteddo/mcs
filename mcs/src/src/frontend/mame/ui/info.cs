// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using size_t = System.UInt32;
using u32 = System.UInt32;


namespace mame.ui
{
    public class machine_static_info : global_object
    {
        protected const machine_flags.type MACHINE_ERRORS    = machine_flags.type.NOT_WORKING | machine_flags.type.MECHANICAL;
        protected const machine_flags.type MACHINE_WARNINGS  = machine_flags.type.NO_COCKTAIL | machine_flags.type.REQUIRES_ARTWORK;
        protected const machine_flags.type MACHINE_BTANB     = machine_flags.type.NO_SOUND_HW | machine_flags.type.IS_INCOMPLETE;


        protected static readonly KeyValuePair<emu.detail.device_feature.type, string> [] FEATURE_NAMES =
        {
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.PROTECTION,    "protection"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.TIMING,        "timing"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.GRAPHICS,      "graphics"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.PALETTE,       "color palette"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.SOUND,         "sound"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.CAPTURE,       "capture hardware"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.CAMERA,        "camera"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.MICROPHONE,    "microphone"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.CONTROLS,      "controls"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.KEYBOARD,      "keyboard"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.MOUSE,         "mouse"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.MEDIA,         "media"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.DISK,          "disk"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.PRINTER,       "printer"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.TAPE,          "magnetic tape"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.PUNCH,         "punch tape"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.DRUM,          "magnetic drum"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.ROM,           "solid state storage"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.COMMS,         "communications"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.LAN,           "LAN"),
            new KeyValuePair<emu.detail.device_feature.type, string>(emu.detail.device_feature.type.WAN,           "WAN")
        };


        ui_options m_options;

        // overall feature status
        machine_flags.type m_flags;
        emu.detail.device_feature.type m_unemulated_features;
        emu.detail.device_feature.type m_imperfect_features;

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
            foreach (device_t device in new device_iterator(config.root_device()))
            {
                // the "no sound hardware" warning doesn't make sense when you plug in a sound card
                if (device.GetClassInterface<device_sound_interface>() != null)  // dynamic_cast<device_sound_interface *>(&device))
                    m_flags &= ~machine_flags.type.NO_SOUND_HW;

                // build overall emulation status
                m_unemulated_features |= device.type().unemulated_features();
                m_imperfect_features |= device.type().imperfect_features();

                // look for BIOS options
                List<tiny_rom_entry> rom = device.rom_region();
                for (int romOffset = 0; !m_has_bioses && rom != null && rom[romOffset] != null && !romload_global.ROMENTRY_ISEND(rom[romOffset]); ++romOffset)
                {
                    if (romload_global.ROMENTRY_ISSYSTEM_BIOS(rom[romOffset]))
                        m_has_bioses = true;
                }

                // if we don't have ports passed in, build here
                if (ports == null)
                    local_ports.append(device, out sink);
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
        public machine_flags.type machine_flags_get() { return m_flags; }
        public emu.detail.device_feature.type unemulated_features() { return m_unemulated_features; }
        public emu.detail.device_feature.type imperfect_features() { return m_imperfect_features; }


        // has... getters
        public bool has_bioses() { return m_has_bioses; }

        // has input types getters
        public bool has_dips() { return m_has_dips; }
        public bool has_configs() { return m_has_configs; }
        public bool has_keyboard() { return m_has_keyboard; }
        public bool has_test_switch() { return m_has_test_switch; }
        public bool has_analog() { return m_has_analog; }


        // message colour
        //-------------------------------------------------
        //  status_color - returns suitable colour for
        //  driver status box
        //-------------------------------------------------
        public rgb_t status_color()
        {
            if ((machine_flags_get() & MACHINE_ERRORS) != 0 || ((unemulated_features() | imperfect_features()) & emu.detail.device_feature.type.PROTECTION) != 0)
                return UI_RED_COLOR;
            else if ((machine_flags_get() & MACHINE_WARNINGS) != 0 || unemulated_features() != 0 || imperfect_features() != 0)
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
            if ((machine_flags_get() & MACHINE_ERRORS) != 0 || ((unemulated_features() | imperfect_features()) & emu.detail.device_feature.type.PROTECTION) != 0)
                return UI_RED_COLOR;
            else if ((machine_flags_get() & MACHINE_WARNINGS) != 0 || unemulated_features() != 0 || imperfect_features() != 0)
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

            // add a warning if any ROMs were loaded with warnings
            if (m_machine.rom_load().warnings() > 0)
                buf += "One or more ROMs/CHDs for this machine are incorrect. The machine may not run correctly.\n";

            if (!m_machine.rom_load().software_load_warnings_message().empty())
                buf += m_machine.rom_load().software_load_warnings_message();

            // if we have at least one warning flag, print the general header
            if ((m_machine.rom_load().knownbad() > 0) || (machine_flags_get() & (MACHINE_ERRORS | MACHINE_WARNINGS | MACHINE_BTANB)) != 0 || unemulated_features() != 0 || imperfect_features() != 0)
            {
                if (!buf.str().empty())
                    buf += "\n";
                buf += "There are known problems with this machine\n\n";
            }

            // add a warning if any ROMs are flagged BAD_DUMP/NO_DUMP
            if (m_machine.rom_load().knownbad() > 0)
                buf += "One or more ROMs/CHDs for this machine have not been correctly dumped.\n";

            // add line for unemulated features
            if (unemulated_features() != 0)
            {
                buf += "Completely unemulated features: ";
                bool first = true;
                foreach (var feature in FEATURE_NAMES)
                {
                    if ((unemulated_features() & feature.Key) != 0)
                    {
                        buf += string.Format(first ? "{0}" : ", {0}", feature.Value);
                        first = false;
                    }
                }
                buf += "\n";
            }

            // add line for imperfect features
            if (imperfect_features() != 0)
            {
                buf += "Imperfectly emulated features: ";
                bool first = true;
                foreach (var feature in FEATURE_NAMES)
                {
                    if ((imperfect_features() & feature.Key) != 0)
                    {
                        buf += string.Format(first ? "{0}" : ", {0}", feature.Value);
                        first = false;
                    }
                }
                buf += "\n";
            }

            // add one line per machine warning flag
            if ((machine_flags_get() & machine_flags.type.NO_COCKTAIL) != 0)
                buf += "Screen flipping in cocktail mode is not supported.\n";
            if ((machine_flags_get() & machine_flags.type.REQUIRES_ARTWORK) != 0) // check if external artwork is present before displaying this warning?
                buf += "This machine requires external artwork files.\n";
            if ((machine_flags_get() & machine_flags.type.IS_INCOMPLETE ) != 0)
                buf += "This machine was never completed. It may exhibit strange behavior or missing elements that are not bugs in the emulation.\n";
            if ((machine_flags_get() & machine_flags.type.NO_SOUND_HW ) != 0)
                buf += "This machine has no sound hardware, MAME will produce no sounds, this is expected behaviour.\n";

            // these are more severe warnings
            if ((machine_flags_get() & machine_flags.type.NOT_WORKING) != 0)
                buf += "\nTHIS MACHINE DOESN'T WORK. The emulation for this machine is not yet complete. There is nothing you can do to fix this problem except wait for the developers to improve the emulation.\n";
            if ((machine_flags_get() & machine_flags.type.MECHANICAL) != 0)
                buf += "\nElements of this machine cannot be emulated as they require physical interaction or consist of mechanical devices. It is not possible to fully experience this machine.\n";

            if ((machine_flags_get() & MACHINE_ERRORS) != 0 || ((m_machine.system().type.unemulated_features() | m_machine.system().type.imperfect_features()) & emu.detail.device_feature.type.PROTECTION) != 0)
            {
                // find the parent of this driver
                driver_enumerator drivlist = new driver_enumerator(m_machine.options());
                int maindrv = driver_list.find(m_machine.system());
                int clone_of = driver_list.non_bios_clone(maindrv);
                if (clone_of != -1)
                    maindrv = clone_of;

                // scan the driver list for any working clones and add them
                bool foundworking = false;
                while (drivlist.next())
                {
                    if (drivlist.current() == maindrv || drivlist.clone() == maindrv)
                    {
                        game_driver driver = drivlist.driver();
                        if ((driver.flags & MACHINE_ERRORS) == 0 && ((driver.type.unemulated_features() | driver.type.imperfect_features()) & emu.detail.device_feature.type.PROTECTION) == 0)
                        {
                            // this one works, add a header and display the name of the clone
                            if (!foundworking)
                                buf += string.Format("\n\nThere are working clones of this machine: {0}", driver.name);
                            else
                                buf += string.Format(", {0}", driver.name);
                            foundworking = true;
                        }
                    }
                }
                if (foundworking)
                    buf += "\n";
            }

            // add the 'press OK' string
            if (!buf.str().empty())
                buf += "\n\nPress any key to continue";

            return buf.str();
        }


        //-------------------------------------------------
        //  game_info_string - return the game info text
        //-------------------------------------------------
        public string game_info_string()
        {
            string buf = "";  //std::ostringstream buf;

            // print description, manufacturer, and CPU:
            buf += string.Format("{0}\n{1} {2}\nDriver: {3}\n\nCPU:\n",  // %1$s\n%2$s %3$s\nDriver: %4$s\n\nCPU:\n
                    m_machine.system().type.fullname(),
                    m_machine.system().year,
                    m_machine.system().manufacturer,
                    core_filename_extract_base(m_machine.system().type.source()));

            // loop over all CPUs
            execute_interface_iterator execiter = new execute_interface_iterator(m_machine.root_device());
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
                    if (exec.device().type() == scan.device().type() && strcmp(name, scan.device().name()) == 0 && exec.device().clock() == scan.device().clock())
                        if (exectags.insert(scan.device().tag()))  //.second)
                            count++;
                }

                string hz = std.to_string(clock);
                int d = (clock >= 1000000000) ? 9 : (clock >= 1000000) ? 6 : (clock >= 1000) ? 3 : 0;
                if (d > 0)
                {
                    int dpos = hz.length() - d;
                    hz = hz.insert_(dpos, ".");
                    int last = hz.find_last_not_of('0');
                    hz = hz.substr(0, last + (last != dpos ? 1 : 0));
                }

                // if more than one, prepend a #x in front of the CPU name and display clock
                buf += string.Format(
                        (count > 1)
                            ? ((clock != 0) ? "{0}X{1} {2} {3}\n" : "{1}x{2}\n")  //? ((clock != 0) ? "%1$d" UTF8_MULTIPLY "%2$s %3$s" UTF8_NBSP "%4$s\n" : "%1$d" UTF8_MULTIPLY "%2$s\n")
                            : ((clock != 0) ? "{1} {2} {3}\n" : "{1}\n"),  //: ((clock != 0) ? "%2$s %3$s" UTF8_NBSP "%4$s\n" : "%2$s\n"),
                        count, name, hz,
                        (d == 9) ? "GHz" : (d == 6) ? "MHz" : (d == 3) ? "kHz" : "Hz");
            }

            // loop over all sound chips
            sound_interface_iterator snditer = new sound_interface_iterator(m_machine.root_device());
            std.unordered_set<string> soundtags = new std.unordered_set<string>();
            bool found_sound = false;
            foreach (device_sound_interface sound in snditer)
            {
                if (!sound.issound() || !soundtags.insert(sound.device().tag()))  //.second)
                    continue;

                // append the Sound: string
                if (!found_sound)
                    buf += "\nSound:\n";

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
                    int dpos = hz.length() - d;
                    hz = hz.insert_(dpos, ".");
                    int last = hz.find_last_not_of('0');
                    hz = hz.substr(0, last + (last != dpos ? 1 : 0));
                }

                // if more than one, prepend a #x in front of the soundchip name and display clock
                buf += string.Format(
                        (count > 1)
                            ? ((clock != 0) ? "{0}X{1} {2} {3}\n" : "{0}X{1}\n")  //? ((clock != 0) ? "%1$d" UTF8_MULTIPLY "%2$s %3$s" UTF8_NBSP "%4$s\n" : "%1$d" UTF8_MULTIPLY "%2$s\n")
                            : ((clock != 0) ? "{1} {2} {3}\n" : "{1}\n"),  //: ((clock != 0) ? "%2$s %3$s" UTF8_NBSP "%4$s\n" : "%2$s\n"),
                        count, sound.device().name(), hz,
                        (d == 9) ? "GHz" : (d == 6) ? "MHz" : (d == 3) ? "kHz" : "Hz");
            }

            // display screen information
            buf += "\nVideo:\n";
            screen_device_iterator scriter = new screen_device_iterator(m_machine.root_device());
            int scrcount = scriter.count();
            if (scrcount == 0)
            {
                buf += "None\n";
            }
            else
            {
                foreach (screen_device screen in scriter)
                {
                    string detail;
                    if (screen.screen_type() == screen_type_enum.SCREEN_TYPE_VECTOR)
                    {
                        detail = "Vector";
                    }
                    else
                    {
                        string hz = std.to_string((float)screen.frame_period().as_hz());
                        int last = hz.find_last_not_of('0');
                        int dpos = hz.find_last_of('.');
                        hz = hz.substr(0, last + (last != dpos ? 1 : 0));

                        rectangle visarea = screen.visible_area();
                        detail = string_format("{0} X {1} ({2}) {3} Hz",  //detail = string_format("%d " UTF8_MULTIPLY " %d (%s) %s" UTF8_NBSP "Hz",
                                visarea.width(), visarea.height(),
                                (screen.orientation() & ORIENTATION_SWAP_XY) != 0 ? "V" : "H",
                                hz);
                    }

                    buf += string.Format(
                            (scrcount > 1) ? "{0}: {1}\n" : "{1}\n",  //(scrcount > 1) ? _("%1$s: %2$s\n") : _("%2$s\n"),
                            get_screen_desc(screen), detail);
                }
            }

            return buf.str();
        }


        //-------------------------------------------------
        //  get_screen_desc - returns the description for
        //  a given screen
        //-------------------------------------------------
        protected string get_screen_desc(screen_device screen)
        {
            if (new screen_device_iterator(m_machine.root_device()).count() > 1)
                return string.Format("Screen '{0}'", screen.tag());
            else
                return "Screen";
        }
    }


#if false
    class menu_game_info : menu
    {
        //menu_game_info(mame_ui_manager &mui, render_container &container);
        //virtual ~menu_game_info() override;


        //virtual void populate(float &customtop, float &custombottom) override;
        //virtual void handle() override;
    }


    class menu_image_info : menu
    {
        //menu_image_info(mame_ui_manager &mui, render_container &container);
        //virtual ~menu_image_info() override;


        //virtual void populate(float &customtop, float &custombottom) override;
        //virtual void handle() override;
        //void image_info(device_image_interface *image);
    }
#endif
}
