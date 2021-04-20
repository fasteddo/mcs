// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class emuopts_global
    {
        // takes an existing emu_options and adds system specific options
        //void osd_setup_osd_specific_emu_options(emu_options &opts);


        //std::vector<std::string> get_full_option_names(const device_image_interface &image)


        //-------------------------------------------------
        //  conditionally_peg_priority
        //-------------------------------------------------
        public static void conditionally_peg_priority(core_options.entry entry, bool peg_priority)
        {
            // if the [image|slot] entry was specified outside of the context of the options sytem, we need
            // to peg the priority of any associated core_options::entry at the maximum priority
            if (peg_priority)// && !entry.expired())
                entry.set_priority(global_object.OPTION_PRIORITY_MAXIMUM);
        }
    }


    public class slot_option : global_object
    {
        emu_options m_host;
        bool m_specified;
        string m_specified_value;
        string m_specified_bios;
        string m_default_card_software;
        string m_default_value;
        core_options.entry m_entry;  //core_options::entry.weak_ptr   m_entry;


        //-------------------------------------------------
        //  slot_option ctor
        //-------------------------------------------------
        slot_option(emu_options host, string default_value)
        {
            m_host = host;
            m_specified = false;
            m_default_value = default_value != null ? default_value : "";
        }

        //slot_option(const slot_option &that) = delete;
        //slot_option(slot_option &&that) = default;


        // accessors

        //-------------------------------------------------
        //  slot_option::value
        //-------------------------------------------------
        public string value()
        {
            // There are a number of ways that the value can be determined; there
            // is a specific order of precedence:
            //
            //  1.  Highest priority is whatever may have been specified by the user (whether it
            //      was specified at the command line, an INI file, or in the UI).  We keep track
            //      of whether these values were specified this way
            //
            //      Take note that slots have a notion of being "selectable".  Slots that are not
            //      marked as selectable cannot be specified with this technique
            //
            //  2.  Next highest is what is returned from get_default_card_software()
            //
            //  3.  Last in priority is what was specified as the slot default.  This comes from
            //      device setup
            if (m_specified)
                return m_specified_value;
            else if (!m_default_card_software.empty())
                return m_default_card_software;
            else
                return m_default_value;
        }


        //-------------------------------------------------
        //  slot_option::specified_value
        //-------------------------------------------------
        public string specified_value()
        {
            string result = "";
            if (m_specified)
            {
                result = m_specified_bios.empty()
                    ? m_specified_value
                    : string.Format("{0},bios={1}", m_specified_value, m_specified_bios);
            }
            return result;
        }


        public string bios() { return m_specified_bios; }
        public string default_card_software() { return m_default_card_software; }
        public bool specified() { return m_specified; }
        //core_options::entry::shared_ptr option_entry() const { return m_entry.lock(); }

        // seters

        //-------------------------------------------------
        //  slot_option::specify
        //-------------------------------------------------
        public void specify(string text, bool peg_priority = true)
        {
            // record the old value; we may need to trigger an update
            string old_value = value();

            // we need to do some elementary parsing here
            string bios_arg = ",bios=";

            int pos = text.IndexOf(bios_arg);
            if (pos != -1)
            {
                m_specified = true;
                m_specified_value = text.substr(0, pos);
                m_specified_bios = text.substr(pos + bios_arg.Length);
            }
            else
            {
                m_specified = true;
                m_specified_value = text;
                m_specified_bios = "";
            }

            conditionally_peg_priority(m_entry, peg_priority);

            // we may have changed
            possibly_changed(old_value);
        }

        //void specify(std::string &&text, bool peg_priority = true);

        //void set_bios(std::string &&text);

        //public void set_default_card_software(string s)


        // instantiates an option entry (don't call outside of emuopts.cpp)
        //core_options::entry::shared_ptr setup_option_entry(const char *name);

        //-------------------------------------------------
        //  slot_option::possibly_changed
        //-------------------------------------------------
        void possibly_changed(string old_value)
        {
            if (value() != old_value)
                m_host.update_slot_and_image_options();
        }
    }


    public class image_option : global_object
    {
        emu_options m_host;
        string m_canonical_instance_name;
        string m_value;
        core_options.entry m_entry;


        //-------------------------------------------------
        //  image_option ctor
        //-------------------------------------------------
        image_option(emu_options host, string canonical_instance_name)
        {
            m_host = host;
            m_canonical_instance_name = canonical_instance_name;
        }

        //image_option(const image_option &that) = delete;
        //image_option(image_option &&that) = default;


        // accessors
        //const std::string &canonical_instance_name() const { return m_canonical_instance_name; }
        public string value() { return m_value; }
        //core_options::entry::shared_ptr option_entry() const { return m_entry.lock(); }


        // mutators

        //-------------------------------------------------
        //  image_option::specify
        //-------------------------------------------------
        public void specify(string value, bool peg_priority = true)
        {
            if (value != m_value)
            {
                m_value = value;

                throw new emu_unimplemented();
#if false
                m_host.reevaluate_default_card_software();
#endif
            }
            conditionally_peg_priority(m_entry, peg_priority);
        }

        //void specify(std::string &&value, bool peg_priority = true);


        // instantiates an option entry (don't call outside of emuopts.cpp)
        //core_options::entry::shared_ptr setup_option_entry(std::vector<std::string> &&names);
    }



    public class emu_options : core_options
    {
        //friend class slot_option;
        //friend class image_option;

        public enum ui_option
        {
            UI_CABINET,
            UI_SIMPLE
        }


        public enum option_support
        {
            FULL,                   // full option support
            GENERAL_AND_SYSTEM,     // support for general options and system (no softlist)
            GENERAL_ONLY            // only support for general options
        }


        struct software_options
        {
            //std::unordered_map<std::string, std::string>    slot;
            //std::unordered_map<std::string, std::string>    image;
        }


        public const int OPTION_PRIORITY_CMDLINE = OPTION_PRIORITY_HIGH + 1;

        // core options
        public static string OPTION_SYSTEMNAME   { get { return unadorned(0); } }
        public static string OPTION_SOFTWARENAME { get { return unadorned(1); } }

        // core configuration options
        const string OPTION_READCONFIG          = "readconfig";
        const string OPTION_WRITECONFIG         = "writeconfig";

        // core search path options
        const string OPTION_HOMEPATH            = "homepath";
        const string OPTION_MEDIAPATH           = "rompath";
        public const string OPTION_HASHPATH            = "hashpath";
        const string OPTION_SAMPLEPATH          = "samplepath";
        const string OPTION_ARTPATH             = "artpath";
        const string OPTION_CTRLRPATH           = "ctrlrpath";
        public const string OPTION_INIPATH             = "inipath";
        const string OPTION_FONTPATH            = "fontpath";
        const string OPTION_CHEATPATH           = "cheatpath";
        const string OPTION_CROSSHAIRPATH       = "crosshairpath";
        const string OPTION_PLUGINSPATH         = "pluginspath";
        const string OPTION_LANGUAGEPATH        = "languagepath";
        const string OPTION_SWPATH              = "swpath";

        // core directory options
        const string OPTION_CFG_DIRECTORY       = "cfg_directory";
        const string OPTION_NVRAM_DIRECTORY     = "nvram_directory";
        const string OPTION_INPUT_DIRECTORY     = "input_directory";
        const string OPTION_STATE_DIRECTORY     = "state_directory";
        public const string OPTION_SNAPSHOT_DIRECTORY = "snapshot_directory";
        const string OPTION_DIFF_DIRECTORY      = "diff_directory";
        const string OPTION_COMMENT_DIRECTORY   = "comment_directory";

        // core state/playback options
        const string OPTION_STATE               = "state";
        const string OPTION_AUTOSAVE            = "autosave";
        const string OPTION_REWIND              = "rewind";
        const string OPTION_REWIND_CAPACITY     = "rewind_capacity";
        const string OPTION_PLAYBACK            = "playback";
        const string OPTION_RECORD              = "record";
        const string OPTION_RECORD_TIMECODE     = "record_timecode";
        const string OPTION_EXIT_AFTER_PLAYBACK = "exit_after_playback";
        const string OPTION_MNGWRITE            = "mngwrite";
        const string OPTION_AVIWRITE            = "aviwrite";
        const string OPTION_WAVWRITE            = "wavwrite";
        public const string OPTION_SNAPNAME            = "snapname";
        const string OPTION_SNAPSIZE            = "snapsize";
        const string OPTION_SNAPVIEW            = "snapview";
        const string OPTION_SNAPBILINEAR        = "snapbilinear";
        const string OPTION_STATENAME           = "statename";
        const string OPTION_BURNIN              = "burnin";

        // core performance options
        const string OPTION_AUTOFRAMESKIP       = "autoframeskip";
        const string OPTION_FRAMESKIP           = "frameskip";
        public const string OPTION_SECONDS_TO_RUN = "seconds_to_run";
        public const string OPTION_THROTTLE     = "throttle";
        const string OPTION_SLEEP               = "sleep";
        const string OPTION_SPEED               = "speed";
        const string OPTION_REFRESHSPEED        = "refreshspeed";
        const string OPTION_LOWLATENCY          = "lowlatency";

        // core render options
        const string OPTION_KEEPASPECT          = "keepaspect";
        const string OPTION_UNEVENSTRETCH       = "unevenstretch";
        const string OPTION_UNEVENSTRETCHX      = "unevenstretchx";
        const string OPTION_UNEVENSTRETCHY      = "unevenstretchy";
        const string OPTION_AUTOSTRETCHXY       = "autostretchxy";
        const string OPTION_INTOVERSCAN         = "intoverscan";
        const string OPTION_INTSCALEX           = "intscalex";
        const string OPTION_INTSCALEY           = "intscaley";

        // core rotation options
        const string OPTION_ROTATE              = "rotate";
        const string OPTION_ROR                 = "ror";
        const string OPTION_ROL                 = "rol";
        const string OPTION_AUTOROR             = "autoror";
        const string OPTION_AUTOROL             = "autorol";
        const string OPTION_FLIPX               = "flipx";
        const string OPTION_FLIPY               = "flipy";

        // core artwork options
        const string OPTION_ARTWORK_CROP        = "artwork_crop";
        const string OPTION_FALLBACK_ARTWORK    = "fallback_artwork";
        const string OPTION_OVERRIDE_ARTWORK    = "override_artwork";

        // core screen options
        const string OPTION_BRIGHTNESS          = "brightness";
        const string OPTION_CONTRAST            = "contrast";
        const string OPTION_GAMMA               = "gamma";
        const string OPTION_PAUSE_BRIGHTNESS    = "pause_brightness";
        const string OPTION_EFFECT              = "effect";

        // core vector options
        const string OPTION_BEAM_WIDTH_MIN      = "beam_width_min";
        const string OPTION_BEAM_WIDTH_MAX      = "beam_width_max";
        const string OPTION_BEAM_DOT_SIZE       = "beam_dot_size";
        const string OPTION_BEAM_INTENSITY_WEIGHT = "beam_intensity_weight";
        const string OPTION_FLICKER             = "flicker";

        // core sound options
        const string OPTION_SAMPLERATE          = "samplerate";
        const string OPTION_SAMPLES             = "samples";
        const string OPTION_VOLUME              = "volume";

        // core input options
        const string OPTION_COIN_LOCKOUT        = "coin_lockout";
        const string OPTION_CTRLR               = "ctrlr";
        const string OPTION_MOUSE               = "mouse";
        const string OPTION_JOYSTICK            = "joystick";
        const string OPTION_LIGHTGUN            = "lightgun";
        const string OPTION_MULTIKEYBOARD       = "multikeyboard";
        const string OPTION_MULTIMOUSE          = "multimouse";
        const string OPTION_STEADYKEY           = "steadykey";
        const string OPTION_UI_ACTIVE           = "ui_active";
        const string OPTION_OFFSCREEN_RELOAD    = "offscreen_reload";
        const string OPTION_JOYSTICK_MAP        = "joystick_map";
        const string OPTION_JOYSTICK_DEADZONE   = "joystick_deadzone";
        const string OPTION_JOYSTICK_SATURATION = "joystick_saturation";
        public const string OPTION_NATURAL_KEYBOARD = "natural";
        const string OPTION_JOYSTICK_CONTRADICTORY = "joystick_contradictory";
        const string OPTION_COIN_IMPULSE        = "coin_impulse";

        // input autoenable options
        public const string OPTION_PADDLE_DEVICE       = "paddle_device";
        public const string OPTION_ADSTICK_DEVICE      = "adstick_device";
        public const string OPTION_PEDAL_DEVICE        = "pedal_device";
        public const string OPTION_DIAL_DEVICE         = "dial_device";
        public const string OPTION_TRACKBALL_DEVICE    = "trackball_device";
        public const string OPTION_LIGHTGUN_DEVICE     = "lightgun_device";
        public const string OPTION_POSITIONAL_DEVICE   = "positional_device";
        public const string OPTION_MOUSE_DEVICE        = "mouse_device";

        // core debugging options
        const string OPTION_LOG                 = "log";
        const string OPTION_DEBUG               = "debug";
        const string OPTION_VERBOSE             = "verbose";
        const string OPTION_OSLOG               = "oslog";
        const string OPTION_UPDATEINPAUSE       = "update_in_pause";
        const string OPTION_DEBUGSCRIPT         = "debugscript";
        const string OPTION_DEBUGLOG            = "debuglog";

        // core misc options
        const string OPTION_DRC                 = "drc";
        const string OPTION_DRC_USE_C           = "drc_use_c";
        const string OPTION_DRC_LOG_UML         = "drc_log_uml";
        const string OPTION_DRC_LOG_NATIVE      = "drc_log_native";
        public const string OPTION_BIOS         = "bios";
        const string OPTION_CHEAT               = "cheat";
        const string OPTION_SKIP_GAMEINFO       = "skip_gameinfo";
        const string OPTION_UI_FONT             = "uifont";
        const string OPTION_UI                  = "ui";
        public const string OPTION_RAMSIZE      = "ramsize";
        const string OPTION_NVRAM_SAVE          = "nvram_save";

        // core comm options
        const string OPTION_COMM_LOCAL_HOST     = "comm_localhost";
        const string OPTION_COMM_LOCAL_PORT     = "comm_localport";
        const string OPTION_COMM_REMOTE_HOST    = "comm_remotehost";
        const string OPTION_COMM_REMOTE_PORT    = "comm_remoteport";
        const string OPTION_COMM_FRAME_SYNC     = "comm_framesync";

        const string OPTION_CONFIRM_QUIT        = "confirm_quit";
        const string OPTION_UI_MOUSE            = "ui_mouse";

        const string OPTION_AUTOBOOT_COMMAND    = "autoboot_command";
        const string OPTION_AUTOBOOT_DELAY      = "autoboot_delay";
        const string OPTION_AUTOBOOT_SCRIPT     = "autoboot_script";

        public const string OPTION_CONSOLE      = "console";
        const string OPTION_PLUGINS             = "plugins";
        const string OPTION_PLUGIN              = "plugin";
        const string OPTION_NO_PLUGIN           = "noplugin";

        const string OPTION_LANGUAGE            = "language";

        const string OPTION_HTTP                = "http";
        const string OPTION_HTTP_PORT           = "http_port";
        const string OPTION_HTTP_ROOT           = "http_root";


        // static list of options entries
        static readonly options_entry [] s_option_entries = new options_entry []
        {
            // unadorned options - only a single one supported at the moment
            new options_entry(OPTION_SYSTEMNAME,                                 null,        OPTION_STRING,     null),
            new options_entry(OPTION_SOFTWARENAME,                               null,        OPTION_STRING,     null),

            // config options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE CONFIGURATION OPTIONS"),
            new options_entry(OPTION_READCONFIG + ";rc",                         "1",         OPTION_BOOLEAN,    "enable loading of configuration files"),
            new options_entry(OPTION_WRITECONFIG + ";wc",                        "0",         OPTION_BOOLEAN,    "write configuration to (driver).ini on exit"),

            // search path options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE SEARCH PATH OPTIONS"),
            new options_entry(OPTION_HOMEPATH,                                   ".",         OPTION_STRING,     "path to base folder for plugin data (read/write)"),
            new options_entry(OPTION_MEDIAPATH + ";rp;biospath;bp",              "roms",      OPTION_STRING,     "path to ROM sets and hard disk images"),
            new options_entry(OPTION_HASHPATH + ";hash_directory;hash",          "hash",      OPTION_STRING,     "path to software definition files"),
            new options_entry(OPTION_SAMPLEPATH + ";sp",                         "samples",   OPTION_STRING,     "path to audio sample sets"),
            new options_entry(OPTION_ARTPATH,                                    "artwork",   OPTION_STRING,     "path to artwork files"),
            new options_entry(OPTION_CTRLRPATH,                                  "ctrlr",     OPTION_STRING,     "path to controller definitions"),
            new options_entry(OPTION_INIPATH,                                    ".;ini;ini/presets", OPTION_STRING, "path to ini files"),
            new options_entry(OPTION_FONTPATH,                                   ".",         OPTION_STRING,     "path to font files"),
            new options_entry(OPTION_CHEATPATH,                                  "cheat",     OPTION_STRING,     "path to cheat files"),
            new options_entry(OPTION_CROSSHAIRPATH,                              "crosshair", OPTION_STRING,     "path to crosshair files"),
            new options_entry(OPTION_PLUGINSPATH,                                "plugins",   OPTION_STRING,     "path to plugin files"),
            new options_entry(OPTION_LANGUAGEPATH,                               "language",  OPTION_STRING,     "path to UI translation files"),
            new options_entry(OPTION_SWPATH,                                     "software",  OPTION_STRING,     "path to loose software"),

            // output directory options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE OUTPUT DIRECTORY OPTIONS"),
            new options_entry(OPTION_CFG_DIRECTORY,                              "cfg",       OPTION_STRING,     "directory to save configurations"),
            new options_entry(OPTION_NVRAM_DIRECTORY,                            "nvram",     OPTION_STRING,     "directory to save NVRAM contents"),
            new options_entry(OPTION_INPUT_DIRECTORY,                            "inp",       OPTION_STRING,     "directory to save input device logs"),
            new options_entry(OPTION_STATE_DIRECTORY,                            "sta",       OPTION_STRING,     "directory to save states"),
            new options_entry(OPTION_SNAPSHOT_DIRECTORY,                         "snap",      OPTION_STRING,     "directory to save/load screenshots"),
            new options_entry(OPTION_DIFF_DIRECTORY,                             "diff",      OPTION_STRING,     "directory to save hard drive image difference files"),
            new options_entry(OPTION_COMMENT_DIRECTORY,                          "comments",  OPTION_STRING,     "directory to save debugger comments"),

            // state/playback options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE STATE/PLAYBACK OPTIONS"),
            new options_entry(OPTION_STATE,                                      null,        OPTION_STRING,     "saved state to load"),
            new options_entry(OPTION_AUTOSAVE,                                   "0",         OPTION_BOOLEAN,    "automatically restore state on start and save on exit for supported systems"),
            new options_entry(OPTION_REWIND,                                     "0",         OPTION_BOOLEAN,    "enable rewind savestates"),
            new options_entry(OPTION_REWIND_CAPACITY + "(1-2048)",               "100",       OPTION_INTEGER,    "rewind buffer size in megabytes"),
            new options_entry(OPTION_PLAYBACK + ";pb",                           null,        OPTION_STRING,     "playback an input file"),
            new options_entry(OPTION_RECORD + ";rec",                            null,        OPTION_STRING,     "record an input file"),
            new options_entry(OPTION_RECORD_TIMECODE,                            "0",         OPTION_BOOLEAN,    "record an input timecode file (requires -record option)"),
            new options_entry(OPTION_EXIT_AFTER_PLAYBACK,                        "0",         OPTION_BOOLEAN,    "close the program at the end of playback"),

            new options_entry(OPTION_MNGWRITE,                                   null,        OPTION_STRING,     "optional filename to write a MNG movie of the current session"),
            new options_entry(OPTION_AVIWRITE,                                   null,        OPTION_STRING,     "optional filename to write an AVI movie of the current session"),
            new options_entry(OPTION_WAVWRITE,                                   null,        OPTION_STRING,     "optional filename to write a WAV file of the current session"),
            new options_entry(OPTION_SNAPNAME,                                   "%g/%i",     OPTION_STRING,     "override of the default snapshot/movie naming; %g == gamename, %i == index"),
            new options_entry(OPTION_SNAPSIZE,                                   "auto",      OPTION_STRING,     "specify snapshot/movie resolution (<width>x<height>) or 'auto' to use minimal size "),
            new options_entry(OPTION_SNAPVIEW,                                   "internal",  OPTION_STRING,     "specify snapshot/movie view or 'internal' to use internal pixel-aspect views"),
            new options_entry(OPTION_SNAPBILINEAR,                               "1",         OPTION_BOOLEAN,    "specify if the snapshot/movie should have bilinear filtering applied"),
            new options_entry(OPTION_STATENAME,                                  "%g",        OPTION_STRING,     "override of the default state subfolder naming; %g == gamename"),
            new options_entry(OPTION_BURNIN,                                     "0",         OPTION_BOOLEAN,    "create burn-in snapshots for each screen"),

            // performance options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE PERFORMANCE OPTIONS"),
            new options_entry(OPTION_AUTOFRAMESKIP + ";afs",                     "0",         OPTION_BOOLEAN,    "enable automatic frameskip adjustment to maintain emulation speed"),
            new options_entry(OPTION_FRAMESKIP + ";fs(0-10)",                    "0",         OPTION_INTEGER,    "set frameskip to fixed value, 0-10 (upper limit with autoframeskip)"),
            new options_entry(OPTION_SECONDS_TO_RUN + ";str",                    "0",         OPTION_INTEGER,    "number of emulated seconds to run before automatically exiting"),
            new options_entry(OPTION_THROTTLE,                                   "1",         OPTION_BOOLEAN,    "throttle emulation to keep system running in sync with real time"),
            new options_entry(OPTION_SLEEP,                                      "1",         OPTION_BOOLEAN,    "enable sleeping, which gives time back to other applications when idle"),
            new options_entry(OPTION_SPEED + "(0.01-100)",                       "1.0",       OPTION_FLOAT,      "controls the speed of gameplay, relative to realtime; smaller numbers are slower"),
            new options_entry(OPTION_REFRESHSPEED + ";rs",                       "0",         OPTION_BOOLEAN,    "automatically adjust emulation speed to keep the emulated refresh rate slower than the host screen"),
            new options_entry(OPTION_LOWLATENCY + ";lolat",                      "0",         OPTION_BOOLEAN,    "draws new frame before throttling to reduce input latency"),

            // render options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE RENDER OPTIONS"),
            new options_entry(OPTION_KEEPASPECT + ";ka",                         "1",         OPTION_BOOLEAN,    "maintain aspect ratio when scaling to fill output screen/window"),
            new options_entry(OPTION_UNEVENSTRETCH + ";ues",                     "1",         OPTION_BOOLEAN,    "allow non-integer ratios when scaling to fill output screen/window horizontally or vertically"),
            new options_entry(OPTION_UNEVENSTRETCHX + ";uesx",                   "0",         OPTION_BOOLEAN,    "allow non-integer ratios when scaling to fill output screen/window horizontally"),
            new options_entry(OPTION_UNEVENSTRETCHY + ";uesy",                   "0",         OPTION_BOOLEAN,    "allow non-integer ratios when scaling to fill otuput screen/window vertially"),
            new options_entry(OPTION_AUTOSTRETCHXY + ";asxy",                    "0",         OPTION_BOOLEAN,    "automatically apply -unevenstretchx/y based on source native orientation"),
            new options_entry(OPTION_INTOVERSCAN + ";ios",                       "0",         OPTION_BOOLEAN,    "allow overscan on integer scaled targets"),
            new options_entry(OPTION_INTSCALEX + ";sx",                          "0",         OPTION_INTEGER,    "set horizontal integer scale factor"),
            new options_entry(OPTION_INTSCALEY + ";sy",                          "0",         OPTION_INTEGER,    "set vertical integer scale factor"),

            // rotation options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE ROTATION OPTIONS"),
            new options_entry(OPTION_ROTATE,                                     "1",         OPTION_BOOLEAN,    "rotate the game screen according to the game's orientation when needed"),
            new options_entry(OPTION_ROR,                                        "0",         OPTION_BOOLEAN,    "rotate screen clockwise 90 degrees"),
            new options_entry(OPTION_ROL,                                        "0",         OPTION_BOOLEAN,    "rotate screen counterclockwise 90 degrees"),
            new options_entry(OPTION_AUTOROR,                                    "0",         OPTION_BOOLEAN,    "automatically rotate screen clockwise 90 degrees if vertical"),
            new options_entry(OPTION_AUTOROL,                                    "0",         OPTION_BOOLEAN,    "automatically rotate screen counterclockwise 90 degrees if vertical"),
            new options_entry(OPTION_FLIPX,                                      "0",         OPTION_BOOLEAN,    "flip screen left-right"),
            new options_entry(OPTION_FLIPY,                                      "0",         OPTION_BOOLEAN,    "flip screen upside-down"),

            // artwork options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE ARTWORK OPTIONS"),
            new options_entry(OPTION_ARTWORK_CROP + ";artcrop",                  "0",         OPTION_BOOLEAN,    "crop artwork so emulated screen image fills output screen/window in one axis"),
            new options_entry(OPTION_FALLBACK_ARTWORK,                           null,        OPTION_STRING,     "fallback artwork if no external artwork or internal driver layout defined"),
            new options_entry(OPTION_OVERRIDE_ARTWORK,                           null,        OPTION_STRING,     "override artwork for external artwork and internal driver layout"),

            // screen options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE SCREEN OPTIONS"),
            new options_entry(OPTION_BRIGHTNESS + "(0.1-2.0)",                   "1.0",       OPTION_FLOAT,      "default game screen brightness correction"),
            new options_entry(OPTION_CONTRAST + "(0.1-2.0)",                     "1.0",       OPTION_FLOAT,      "default game screen contrast correction"),
            new options_entry(OPTION_GAMMA + "(0.1-3.0)",                        "1.0",       OPTION_FLOAT,      "default game screen gamma correction"),
            new options_entry(OPTION_PAUSE_BRIGHTNESS + "(0.0-1.0)",             "0.65",      OPTION_FLOAT,      "amount to scale the screen brightness when paused"),
            new options_entry(OPTION_EFFECT,                                     "none",      OPTION_STRING,     "name of a PNG file to use for visual effects, or 'none'"),

            // vector options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE VECTOR OPTIONS"),
            new options_entry(OPTION_BEAM_WIDTH_MIN,                             "1.0",       OPTION_FLOAT,      "set vector beam width minimum"),
            new options_entry(OPTION_BEAM_WIDTH_MAX,                             "1.0",       OPTION_FLOAT,      "set vector beam width maximum"),
            new options_entry(OPTION_BEAM_DOT_SIZE,                              "1.0",       OPTION_FLOAT,      "set vector beam size for dots"),
            new options_entry(OPTION_BEAM_INTENSITY_WEIGHT,                      "0",         OPTION_FLOAT,      "set vector beam intensity weight "),
            new options_entry(OPTION_FLICKER,                                    "0",         OPTION_FLOAT,      "set vector flicker effect"),

            // sound options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE SOUND OPTIONS"),
            new options_entry(OPTION_SAMPLERATE + ";sr(1000-1000000)",           "48000",     OPTION_INTEGER,    "set sound output sample rate"),
            new options_entry(OPTION_SAMPLES,                                    "1",         OPTION_BOOLEAN,    "enable the use of external samples if available"),
            new options_entry(OPTION_VOLUME + ";vol",                            "0",         OPTION_INTEGER,    "sound volume in decibels (-32 min, 0 max)"),

            // input options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE INPUT OPTIONS"),
            new options_entry(OPTION_COIN_LOCKOUT + ";coinlock",                 "1",         OPTION_BOOLEAN,    "ignore coin inputs if coin lockout output is active"),
            new options_entry(OPTION_CTRLR,                                      null,        OPTION_STRING,     "preconfigure for specified controller"),
            new options_entry(OPTION_MOUSE,                                      "0",         OPTION_BOOLEAN,    "enable mouse input"),
            new options_entry(OPTION_JOYSTICK + ";joy",                          "1",         OPTION_BOOLEAN,    "enable joystick input"),
            new options_entry(OPTION_LIGHTGUN + ";gun",                          "0",         OPTION_BOOLEAN,    "enable lightgun input"),
            new options_entry(OPTION_MULTIKEYBOARD + ";multikey",                "0",         OPTION_BOOLEAN,    "enable separate input from each keyboard device (if present)"),
            new options_entry(OPTION_MULTIMOUSE,                                 "0",         OPTION_BOOLEAN,    "enable separate input from each mouse device (if present)"),
            new options_entry(OPTION_STEADYKEY + ";steady",                      "0",         OPTION_BOOLEAN,    "enable steadykey support"),
            new options_entry(OPTION_UI_ACTIVE,                                  "0",         OPTION_BOOLEAN,    "enable user interface on top of emulated keyboard (if present)"),
            new options_entry(OPTION_OFFSCREEN_RELOAD + ";reload",               "0",         OPTION_BOOLEAN,    "convert lightgun button 2 into offscreen reload"),
            new options_entry(OPTION_JOYSTICK_MAP + ";joymap",                   "auto",      OPTION_STRING,     "explicit joystick map, or auto to auto-select"),
            new options_entry(OPTION_JOYSTICK_DEADZONE + ";joy_deadzone;jdz(0.00-1)", "0.3",  OPTION_FLOAT,      "center deadzone range for joystick where change is ignored (0.0 center, 1.0 end)"),
            new options_entry(OPTION_JOYSTICK_SATURATION + ";joy_saturation;jsat(0.00-1)", "0.85", OPTION_FLOAT, "end of axis saturation range for joystick where change is ignored (0.0 center, 1.0 end)"),
            new options_entry(OPTION_NATURAL_KEYBOARD + ";nat",                  "0",         OPTION_BOOLEAN,    "specifies whether to use a natural keyboard or not"),
            new options_entry(OPTION_JOYSTICK_CONTRADICTORY + ";joy_contradictory", "0",      OPTION_BOOLEAN,    "enable contradictory direction digital joystick input at the same time"),
            new options_entry(OPTION_COIN_IMPULSE,                               "0",         OPTION_INTEGER,    "set coin impulse time (n<0 disable impulse, n==0 obey driver, 0<n set time n)"),

            // input autoenable options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE INPUT AUTOMATIC ENABLE OPTIONS"),
            new options_entry(OPTION_PADDLE_DEVICE + ";paddle",                  "keyboard",  OPTION_STRING,     "enable (none|keyboard|mouse|lightgun|joystick) if a paddle control is present"),
            new options_entry(OPTION_ADSTICK_DEVICE + ";adstick",                "keyboard",  OPTION_STRING,     "enable (none|keyboard|mouse|lightgun|joystick) if an analog joystick control is present"),
            new options_entry(OPTION_PEDAL_DEVICE + ";pedal",                    "keyboard",  OPTION_STRING,     "enable (none|keyboard|mouse|lightgun|joystick) if a pedal control is present"),
            new options_entry(OPTION_DIAL_DEVICE + ";dial",                      "keyboard",  OPTION_STRING,     "enable (none|keyboard|mouse|lightgun|joystick) if a dial control is present"),
            new options_entry(OPTION_TRACKBALL_DEVICE + ";trackball",            "keyboard",  OPTION_STRING,     "enable (none|keyboard|mouse|lightgun|joystick) if a trackball control is present"),
            new options_entry(OPTION_LIGHTGUN_DEVICE,                            "keyboard",  OPTION_STRING,     "enable (none|keyboard|mouse|lightgun|joystick) if a lightgun control is present"),
            new options_entry(OPTION_POSITIONAL_DEVICE,                          "keyboard",  OPTION_STRING,     "enable (none|keyboard|mouse|lightgun|joystick) if a positional control is present"),
            new options_entry(OPTION_MOUSE_DEVICE,                               "mouse",     OPTION_STRING,     "enable (none|keyboard|mouse|lightgun|joystick) if a mouse control is present"),

            // debugging options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE DEBUGGING OPTIONS"),
            new options_entry(OPTION_VERBOSE + ";v",                             "0",         OPTION_BOOLEAN,    "display additional diagnostic information"),
            new options_entry(OPTION_LOG,                                        "0",         OPTION_BOOLEAN,    "generate an error.log file"),
            new options_entry(OPTION_OSLOG,                                      "1",         OPTION_BOOLEAN,    "output error.log data to system diagnostic output (debugger or standard error)"),
            new options_entry(OPTION_DEBUG + ";d",                               "0",         OPTION_BOOLEAN,    "enable/disable debugger"),
            new options_entry(OPTION_UPDATEINPAUSE,                              "0",         OPTION_BOOLEAN,    "keep calling video updates while in pause"),
            new options_entry(OPTION_DEBUGSCRIPT,                                null,        OPTION_STRING,     "script for debugger"),
            new options_entry(OPTION_DEBUGLOG,                                   "0",         OPTION_BOOLEAN,    "write debug console output to debug.log"),

            // comm options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE COMM OPTIONS"),
            new options_entry(OPTION_COMM_LOCAL_HOST,                            "0.0.0.0",   OPTION_STRING,     "local address to bind to"),
            new options_entry(OPTION_COMM_LOCAL_PORT,                            "15112",     OPTION_STRING,     "local port to bind to"),
            new options_entry(OPTION_COMM_REMOTE_HOST,                           "127.0.0.1", OPTION_STRING,     "remote address to connect to"),
            new options_entry(OPTION_COMM_REMOTE_PORT,                           "15112",     OPTION_STRING,     "remote port to connect to"),
            new options_entry(OPTION_COMM_FRAME_SYNC,                            "0",         OPTION_BOOLEAN,    "sync frames"),

            // misc options
            new options_entry(null,                                              null,        OPTION_HEADER,     "CORE MISC OPTIONS"),
            new options_entry(OPTION_DRC,                                        "1",         OPTION_BOOLEAN,    "enable DRC CPU core if available"),
            new options_entry(OPTION_DRC_USE_C,                                  "0",         OPTION_BOOLEAN,    "force DRC to use C backend"),
            new options_entry(OPTION_DRC_LOG_UML,                                "0",         OPTION_BOOLEAN,    "write DRC UML disassembly log"),
            new options_entry(OPTION_DRC_LOG_NATIVE,                             "0",         OPTION_BOOLEAN,    "write DRC native disassembly log"),
            new options_entry(OPTION_BIOS,                                       null,        OPTION_STRING,     "select the system BIOS to use"),
            new options_entry(OPTION_CHEAT + ";c",                               "0",         OPTION_BOOLEAN,    "enable cheat subsystem"),
            new options_entry(OPTION_SKIP_GAMEINFO,                              "0",         OPTION_BOOLEAN,    "skip displaying the system information screen at startup"),
            new options_entry(OPTION_UI_FONT,                                    "default",   OPTION_STRING,     "specify a font to use"),
            new options_entry(OPTION_UI,                                         "cabinet",   OPTION_STRING,     "type of UI (simple|cabinet)"),
            new options_entry(OPTION_RAMSIZE + ";ram",                           null,        OPTION_STRING,     "size of RAM (if supported by driver)"),
            new options_entry(OPTION_CONFIRM_QUIT,                               "0",         OPTION_BOOLEAN,    "ask for confirmation before exiting"),
            new options_entry(OPTION_UI_MOUSE,                                   "1",         OPTION_BOOLEAN,    "display UI mouse cursor"),
            new options_entry(OPTION_LANGUAGE + ";lang",                         "English",   OPTION_STRING,     "set UI display language"),
            new options_entry(OPTION_NVRAM_SAVE + ";nvwrite",                    "1",         OPTION_BOOLEAN,    "save NVRAM data on exit"),

            new options_entry(null,                                              null,        OPTION_HEADER,     "SCRIPTING OPTIONS"),
            new options_entry(OPTION_AUTOBOOT_COMMAND + ";ab",                   null,        OPTION_STRING,     "command to execute after machine boot"),
            new options_entry(OPTION_AUTOBOOT_DELAY,                             "0",         OPTION_INTEGER,    "delay before executing autoboot command (seconds)"),
            new options_entry(OPTION_AUTOBOOT_SCRIPT + ";script",                null,        OPTION_STRING,     "Lua script to execute after machine boot"),
            new options_entry(OPTION_CONSOLE,                                    "0",         OPTION_BOOLEAN,    "enable emulator Lua console"),
            new options_entry(OPTION_PLUGINS,                                    "1",         OPTION_BOOLEAN,    "enable Lua plugin support"),
            new options_entry(OPTION_PLUGIN,                                     null,        OPTION_STRING,     "list of plugins to enable"),
            new options_entry(OPTION_NO_PLUGIN,                                  null,        OPTION_STRING,     "list of plugins to disable"),

            new options_entry(null,                                              null,        OPTION_HEADER,     "HTTP SERVER OPTIONS"),
            new options_entry(OPTION_HTTP,                                       "0",         OPTION_BOOLEAN,    "enable HTTP server"),
            new options_entry(OPTION_HTTP_PORT,                                  "8080",      OPTION_INTEGER,    "HTTP server port"),
            new options_entry(OPTION_HTTP_ROOT,                                  "web",       OPTION_STRING,     "HTTP server document root"),

            new options_entry(null),
        };


        // the basics
        option_support m_support;
        game_driver m_system;

        // slots and devices
        std.unordered_map<string, slot_option> m_slot_options = new std.unordered_map<string, slot_option>();
        //std::unordered_map<std::string, ::image_option>     m_image_options_canonical;
        std.unordered_map<string, image_option> m_image_options = new std.unordered_map<string, image_option>();


        // cached options, for scenarios where parsing core_options is too slow
        int m_coin_impulse;
        bool m_joystick_contradictory;
        bool m_sleep;
        bool m_refresh_speed;
        ui_option m_ui;

        // special option; the system name we tried to specify
        string m_attempted_system_name;

        // special option; the software set name that we did specify
        string m_software_name;


        public emu_options(option_support support = option_support.FULL)
            : base()
        {
            m_support = support;
            m_system = null;
            m_coin_impulse = 0;
            m_joystick_contradictory = false;
            m_sleep = true;
            m_refresh_speed = false;
            m_ui = ui_option.UI_CABINET;


            // add entries
            if (support == option_support.FULL || support == option_support.GENERAL_AND_SYSTEM)
                add_entry(new system_name_option_entry(this));
            if (support == option_support.FULL)
                add_entry(new software_name_option_entry(this));
            add_entries(s_option_entries);

            // adding handlers to keep copies of frequently requested options in member variables
            set_value_changed_handler(OPTION_COIN_IMPULSE, (value) => { m_coin_impulse = int_value(OPTION_COIN_IMPULSE); });  //,              [this](const char *value) { m_coin_impulse = int_value(OPTION_COIN_IMPULSE); });
            set_value_changed_handler(OPTION_JOYSTICK_CONTRADICTORY, (value) => { m_joystick_contradictory = bool_value(OPTION_JOYSTICK_CONTRADICTORY); });  //,    [this](const char *value) { m_joystick_contradictory = bool_value(OPTION_JOYSTICK_CONTRADICTORY); });
            set_value_changed_handler(OPTION_SLEEP, (value) => { m_sleep = bool_value(OPTION_SLEEP); });  //,                     [this](const char *value) { m_sleep = bool_value(OPTION_SLEEP); });
            set_value_changed_handler(OPTION_REFRESHSPEED, (value) => { m_refresh_speed = bool_value(OPTION_REFRESHSPEED); });  //,              [this](const char *value) { m_refresh_speed = bool_value(OPTION_REFRESHSPEED); });
            set_value_changed_handler(OPTION_UI, (value) =>  //, [this](const std::string &value)
            {
                if (value == "simple")
                    m_ui = ui_option.UI_SIMPLE;
                else
                    m_ui = ui_option.UI_CABINET;
            });
        }


        // mutation

        //-------------------------------------------------
        //  set_system_name - called to set the system
        //  name; will adjust slot/image options as appropriate
        //-------------------------------------------------
        public void set_system_name(string new_system_name)
        {
            game_driver new_system = null;

            // we are making an attempt - record what we're attempting
            m_attempted_system_name = new_system_name;

            // was a system name specified?
            if (!m_attempted_system_name.empty())
            {
                // if so, first extract the base name (the reason for this is drag-and-drop on Windows; a side
                // effect is a command line like 'mame pacman.foo' will work correctly, but so be it)
                string new_system_base_name = core_filename_extract_base(m_attempted_system_name, true);

                // perform the lookup (and error if it cannot be found)
                int index = driver_list.find(new_system_base_name.c_str());
                if (index < 0)
                    throw new options_error_exception("Unknown system '{0}'", m_attempted_system_name);
                new_system = driver_list.driver(index);
            }

            // did we change anything?
            if (new_system != m_system)
            {
                // if so, specify the new system and update (if we're fully supporting slot/image options)
                m_system = new_system;
                m_software_name = "";
                if (m_support == option_support.FULL)
                    update_slot_and_image_options();
            }
        }

        //void set_system_name(std::string &&new_system_name);


        //-------------------------------------------------
        //  set_software - called to load "unqualified"
        //  software out of a software list (e.g. - "mame nes 'zelda'")
        //-------------------------------------------------
        public void set_software(string new_software)
        {
            throw new emu_unimplemented();
        }


        // core options
        public game_driver system() { return m_system; }
        public string system_name() { return m_system != null ? m_system.name : ""; }
        public string attempted_system_name() { return m_attempted_system_name; }
        //const std::string &software_name() const { return m_software_name; }


        // core configuration options
        public bool read_config() { return bool_value(OPTION_READCONFIG); }
        public bool write_config() { return bool_value(OPTION_WRITECONFIG); }


        // core search path options
        string home_path() { return value(OPTION_HOMEPATH); }
        public string media_path() { return value(OPTION_MEDIAPATH); }
        public string hash_path() { return value(OPTION_HASHPATH); }
        public string sample_path() { return value(OPTION_SAMPLEPATH); }
        public string art_path() { return value(OPTION_ARTPATH); }
        public string ctrlr_path() { return value(OPTION_CTRLRPATH); }
        public string ini_path() { return value(OPTION_INIPATH); }
        public string font_path() { return value(OPTION_FONTPATH); }
        //string cheat_path() { return value(OPTION_CHEATPATH); }
        public string crosshair_path() { return value(OPTION_CROSSHAIRPATH); }
        public string plugins_path() { return value(OPTION_PLUGINSPATH); }
        public string language_path() { return value(OPTION_LANGUAGEPATH); }
        string sw_path() { return value(OPTION_SWPATH); }


        // core directory options
        public string cfg_directory() { return value(OPTION_CFG_DIRECTORY); }
        public string nvram_directory() { return value(OPTION_NVRAM_DIRECTORY); }
        public string input_directory() { return value(OPTION_INPUT_DIRECTORY); }
        //const char *state_directory() const { return value(OPTION_STATE_DIRECTORY); }
        public string snapshot_directory() { return value(OPTION_SNAPSHOT_DIRECTORY); }
        //const char *diff_directory() const { return value(OPTION_DIFF_DIRECTORY); }
        //const char *comment_directory() const { return value(OPTION_COMMENT_DIRECTORY); }


        // core state/playback options
        public string state() { return value(OPTION_STATE); }
        public bool autosave() { return bool_value(OPTION_AUTOSAVE); }
        bool rewind() { return bool_value(OPTION_REWIND); }
        int rewind_capacity() { return int_value(OPTION_REWIND_CAPACITY); }
        public string playback() { return value(OPTION_PLAYBACK); }
        public string record() { return value(OPTION_RECORD); }
        public bool record_timecode() { return bool_value(OPTION_RECORD_TIMECODE); }
        public bool exit_after_playback() { return bool_value(OPTION_EXIT_AFTER_PLAYBACK); }
        public string mng_write() { return value(OPTION_MNGWRITE); }
        public string avi_write() { return value(OPTION_AVIWRITE); }
        public string wav_write() { return value(OPTION_WAVWRITE); }
        //const char *snap_name() const { return value(OPTION_SNAPNAME); }
        public string snap_size() { return value(OPTION_SNAPSIZE); }
        public string snap_view() { return value(OPTION_SNAPVIEW); }
        public bool snap_bilinear() { return bool_value(OPTION_SNAPBILINEAR); }
        //const char *state_name() const { return value(OPTION_STATENAME); }
        public bool burnin() { return bool_value(OPTION_BURNIN); }


        // core performance options
        public bool auto_frameskip() { return bool_value(OPTION_AUTOFRAMESKIP); }
        public int frameskip() { return int_value(OPTION_FRAMESKIP); }
        public int seconds_to_run() { return int_value(OPTION_SECONDS_TO_RUN); }
        public bool throttle() { return bool_value(OPTION_THROTTLE); }
        public bool sleep() { return m_sleep; }
        public float speed() { return float_value(OPTION_SPEED); }
        public bool refresh_speed() { return m_refresh_speed; }
        public bool low_latency() { return bool_value(OPTION_LOWLATENCY); }


        // core render options
        public bool keep_aspect() { return bool_value(OPTION_KEEPASPECT); }
        public bool uneven_stretch() { return bool_value(OPTION_UNEVENSTRETCH); }
        public bool uneven_stretch_x() { return bool_value(OPTION_UNEVENSTRETCHX); }
        public bool uneven_stretch_y() { return bool_value(OPTION_UNEVENSTRETCHY); }
        public bool auto_stretch_xy() { return bool_value(OPTION_AUTOSTRETCHXY); }
        public bool int_overscan() { return bool_value(OPTION_INTOVERSCAN); }
        public int int_scale_x() { return int_value(OPTION_INTSCALEX); }
        public int int_scale_y() { return int_value(OPTION_INTSCALEY); }


        // core rotation options
        public bool rotate() { return bool_value(OPTION_ROTATE); }
        public bool ror() { return bool_value(OPTION_ROR); }
        public bool rol() { return bool_value(OPTION_ROL); }
        public bool auto_ror() { return bool_value(OPTION_AUTOROR); }
        public bool auto_rol() { return bool_value(OPTION_AUTOROL); }
        public bool flipx() { return bool_value(OPTION_FLIPX); }
        public bool flipy() { return bool_value(OPTION_FLIPY); }


        // core artwork options
        public bool artwork_crop() { return bool_value(OPTION_ARTWORK_CROP); }
        public string fallback_artwork() { return value(OPTION_FALLBACK_ARTWORK); }
        public string override_artwork() { return value(OPTION_OVERRIDE_ARTWORK); }


        // core screen options
        public float brightness() { return float_value(OPTION_BRIGHTNESS); }
        public float contrast() { return float_value(OPTION_CONTRAST); }
        public float gamma() { return float_value(OPTION_GAMMA); }
        public float pause_brightness() { return float_value(OPTION_PAUSE_BRIGHTNESS); }
        public string effect() { return value(OPTION_EFFECT); }


        // core vector options
        //float beam_width_min() const { return float_value(OPTION_BEAM_WIDTH_MIN); }
        //float beam_width_max() const { return float_value(OPTION_BEAM_WIDTH_MAX); }
        //float beam_dot_size() const { return float_value(OPTION_BEAM_DOT_SIZE); }
        //float beam_intensity_weight() const { return float_value(OPTION_BEAM_INTENSITY_WEIGHT); }
        //float flicker() const { return float_value(OPTION_FLICKER); }


        // core sound options
        public int sample_rate() { return int_value(OPTION_SAMPLERATE); }
        public bool samples() { return bool_value(OPTION_SAMPLES); }
        public int volume() { return int_value(OPTION_VOLUME); }


        // core input options
        public bool coin_lockout() { return bool_value(OPTION_COIN_LOCKOUT); }
        public string ctrlr() { return value(OPTION_CTRLR); }
        public bool mouse() { return bool_value(OPTION_MOUSE); }
        public bool joystick() { return bool_value(OPTION_JOYSTICK); }
        public bool lightgun() { return bool_value(OPTION_LIGHTGUN); }
        public bool multi_keyboard() { return bool_value(OPTION_MULTIKEYBOARD); }
        public bool multi_mouse() { return bool_value(OPTION_MULTIMOUSE); }
        //const char *paddle_device() const { return value(OPTION_PADDLE_DEVICE); }
        //const char *adstick_device() const { return value(OPTION_ADSTICK_DEVICE); }
        //const char *pedal_device() const { return value(OPTION_PEDAL_DEVICE); }
        //const char *dial_device() const { return value(OPTION_DIAL_DEVICE); }
        //const char *trackball_device() const { return value(OPTION_TRACKBALL_DEVICE); }
        //const char *lightgun_device() const { return value(OPTION_LIGHTGUN_DEVICE); }
        //const char *positional_device() const { return value(OPTION_POSITIONAL_DEVICE); }
        //const char *mouse_device() const { return value(OPTION_MOUSE_DEVICE); }
        public string joystick_map() { return value(OPTION_JOYSTICK_MAP); }
        public float joystick_deadzone() { return float_value(OPTION_JOYSTICK_DEADZONE); }
        public float joystick_saturation() { return float_value(OPTION_JOYSTICK_SATURATION); }
        public bool steadykey() { return bool_value(OPTION_STEADYKEY); }
        public bool ui_active() { return bool_value(OPTION_UI_ACTIVE); }
        public bool offscreen_reload() { return bool_value(OPTION_OFFSCREEN_RELOAD); }
        public bool natural_keyboard() { return bool_value(OPTION_NATURAL_KEYBOARD); }
        public bool joystick_contradictory() { return m_joystick_contradictory; }
        public int coin_impulse() { return m_coin_impulse; }


        // core debugging options
        public bool log() { return bool_value(OPTION_LOG); }
        public bool debug() { return bool_value(OPTION_DEBUG); }
        public bool verbose() { return bool_value(OPTION_VERBOSE); }
        public bool oslog() { return bool_value(OPTION_OSLOG); }
        //const char *debug_script() const { return value(OPTION_DEBUGSCRIPT); }
        public bool update_in_pause() { return bool_value(OPTION_UPDATEINPAUSE); }
        public bool debuglog() { return bool_value(OPTION_DEBUGLOG); }

        // core misc options
        //bool drc() const { return bool_value(OPTION_DRC); }
        //bool drc_use_c() const { return bool_value(OPTION_DRC_USE_C); }
        //bool drc_log_uml() const { return bool_value(OPTION_DRC_LOG_UML); }
        //bool drc_log_native() const { return bool_value(OPTION_DRC_LOG_NATIVE); }
        public string bios() { return value(OPTION_BIOS); }
        public bool cheat() { return bool_value(OPTION_CHEAT); }
        public bool skip_gameinfo() { return bool_value(OPTION_SKIP_GAMEINFO); }
        public string ui_font() { return value(OPTION_UI_FONT); }
        public ui_option ui() { return m_ui; }
        //const char *ram_size() const { return value(OPTION_RAMSIZE); }
        public bool nvram_save() { return bool_value(OPTION_NVRAM_SAVE); }

        // core comm options
        //const char *comm_localhost() const { return value(OPTION_COMM_LOCAL_HOST); }
        //const char *comm_localport() const { return value(OPTION_COMM_LOCAL_PORT); }
        //const char *comm_remotehost() const { return value(OPTION_COMM_REMOTE_HOST); }
        //const char *comm_remoteport() const { return value(OPTION_COMM_REMOTE_PORT); }
        //bool comm_framesync() const { return bool_value(OPTION_COMM_FRAME_SYNC); }

        public bool confirm_quit() { return bool_value(OPTION_CONFIRM_QUIT); }
        public bool ui_mouse() { return bool_value(OPTION_UI_MOUSE); }

        public string autoboot_command() { return value(OPTION_AUTOBOOT_COMMAND); }
        public int autoboot_delay() { return int_value(OPTION_AUTOBOOT_DELAY); }
        public string autoboot_script() { return value(OPTION_AUTOBOOT_SCRIPT); }

        public bool console() { return bool_value(OPTION_CONSOLE); }

        public bool plugins() { return bool_value(OPTION_PLUGINS); }

        public string plugin() { return value(OPTION_PLUGIN); }
        public string no_plugin() { return value(OPTION_NO_PLUGIN); }

        public string language() { return value(OPTION_LANGUAGE); }

        // Web server specific options
        public bool http() { return bool_value(OPTION_HTTP); }
        public Int16 http_port() { return (Int16)int_value(OPTION_HTTP_PORT); }
        public string http_root() { return value(OPTION_HTTP_ROOT); }

        // slots and devices - the values for these are stored outside of the core_options
        // structure

        public slot_option slot_option(string device_name)
        {
            slot_option opt = find_slot_option(device_name);
            assert(opt != null && "Attempt to access non-existent slot option" != null);
            return opt;
        }

        //::slot_option &slot_option(const std::string &device_name);

        slot_option find_slot_option(string device_name)
        {
            var iter = m_slot_options.find(device_name);
            return iter != null ? iter : null;
        }

        //::slot_option *find_slot_option(const std::string &device_name);

        public bool has_slot_option(string device_name) { return find_slot_option(device_name) != null ? true : false; }

        public image_option image_option(string device_name)
        {
            var iter = m_image_options.find(device_name);
            assert(iter != null && "Attempt to access non-existent image option" != null);
            return iter;
        }
        //::image_option &image_option(const std::string &device_name);
        public bool has_image_option(string device_name) { return m_image_options.find(device_name) != null; }


        //-------------------------------------------------
        //  command_argument_processed
        //-------------------------------------------------
        protected override void command_argument_processed()
        {
            // some command line arguments require that the system name be set, so we can get slot options
            if (command_arguments().size() == 1 && !core_iswildstr(command_arguments()[0].c_str()) &&
                (command() == "listdevices" || (command() == "listslots") || (command() == "listmedia") || (command() == "listsoftware")))
            {
                set_system_name(command_arguments()[0]);
            }
        }


        // slot/image/softlist calculus
        //software_options evaluate_initial_softlist_options(const std::string &software_identifier);


        //-------------------------------------------------
        //  update_slot_and_image_options
        //-------------------------------------------------
        public void update_slot_and_image_options()
        {
            bool changed;
            do
            {
                changed = false;

                // first we add and remove slot options depending on what has been configured in the
                // device, bringing m_slot_options up to a state where it matches machine_config
                if (add_and_remove_slot_options())
                    changed = true;

                // second, we perform an analgous operation with m_image_options
                if (add_and_remove_image_options())
                    changed = true;

                // if we changed anything, we should reevaluate existing options
                if (changed)
                    reevaluate_default_card_software();

            } while (changed);
        }


        bool add_and_remove_slot_options()
        {
            //throw new emu_unimplemented();
            return false;
        }


        bool add_and_remove_image_options()
        {
            //throw new emu_unimplemented();
            return false;
        }


        void reevaluate_default_card_software() { throw new emu_unimplemented(); }


        //std::string get_default_card_software(device_slot_interface &slot);
    }


    //**************************************************************************
    //  CUSTOM OPTION ENTRIES AND SUPPORT CLASSES
    //**************************************************************************

    // custom option entry for the system name
    class system_name_option_entry : core_options.entry
    {
        emu_options m_host;


        public system_name_option_entry(emu_options host)
            : base(emu_options.OPTION_SYSTEMNAME)
        {
            m_host = host;
        }

        public override string value()
        {
            // This is returning an empty string instead of nullptr to signify that
            // specifying the value is a meaningful operation.  The option types that
            // return nullptr are option types that cannot have a value (e.g. - commands)
            //
            // See comments in core_options::entry::value() and core_options::simple_entry::value()
            return m_host.system() != null ? m_host.system().name : "";
        }

        protected override void internal_set_value(string newvalue)
        {
            m_host.set_system_name(newvalue);
        }
    }

    // custom option entry for the software name
    class software_name_option_entry : core_options.entry
    {
        emu_options m_host;

        public software_name_option_entry(emu_options host)
            : base(emu_options.OPTION_SOFTWARENAME)
        {
            m_host = host;
        }

        protected override void internal_set_value(string newvalue)
        {
            m_host.set_software(newvalue);
        }
    }

    // custom option entry for slots
    class slot_option_entry : core_options.entry
    {
        slot_option m_host;
        string m_temp;  //mutable std::string m_temp;


        slot_option_entry(string name, slot_option host)
            : base(name)
        {
            m_host = host;
        }

        public override string value()
        {
            string result = null;
            if (m_host.specified())
            {
                // m_temp is a temporary variable used to keep the specified value
                // so the result can be returned as 'const char *'.  Obviously, this
                // value will be trampled upon if value() is called again.  This doesn't
                // happen in practice
                //
                // In reality, I want to really return std::optional<std::string> here
                // FIXME: the std::string assignment can throw exceptions, and returning std::optional<std::string> also isn't safe in noexcept
                m_temp = m_host.specified_value();
                result = m_temp.c_str();
            }
            return result;
        }

        protected override void internal_set_value(string newvalue)
        {
            m_host.specify(newvalue, false);
        }
    }

    // custom option entry for images
    class image_option_entry : core_options.entry
    {
        image_option m_host;

        image_option_entry(std.vector<string> names, image_option host)
            : base(names)
        {
            m_host = host;
        }

        public override string value()
        {
            return m_host.value().c_str();
        }

        protected override void internal_set_value(string newvalue)
        {
            m_host.specify(newvalue, false);
        }
    }

    // existing option tracker class; used by slot/image calculus to identify existing
    // options for later purging
    //template<typename T>
    class existing_option_tracker<T>
    {
        std.vector<string> m_vec = new std.vector<string>();

        existing_option_tracker(std.unordered_map<string, T> map)
        {
            m_vec.reserve(map.size());
            foreach (var entry in map)
                m_vec.push_back(entry.Key);
        }

        //template<typename TStr>
        //void remove(const TStr &str)
        //{
        //    auto iter = std::find_if(
        //        m_vec.begin(),
        //        m_vec.end(),
        //        [&str](const auto &x) { return *x == str; });
        //    if (iter != m_vec.end())
        //        m_vec.erase(iter);
        //}

        //std::vector<const std::string *>::iterator begin() { return m_vec.begin(); }
        //std::vector<const std::string *>::iterator end() { return m_vec.end(); }
    }
}
