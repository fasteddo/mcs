// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;


namespace mame
{
    //typedef void (*machine_creator_wrapper)(machine_config &, device_t &);
    public delegate void machine_creator_wrapper(machine_config config, device_t owner, device_t device);

    //typedef void (*driver_init_wrapper)(device_t &);
    public delegate void driver_init_wrapper(running_machine machine, device_t owner);


    public static class gamedrv_global
    {
        // maxima
        //const int MAX_DRIVER_NAME_CHARS = 16;

        // flags for machine drivers
        public const UInt64 MACHINE_TYPE_ARCADE               = (UInt64)machine_flags.type.TYPE_ARCADE;
        public const UInt64 MACHINE_TYPE_CONSOLE              = (UInt64)machine_flags.type.TYPE_CONSOLE;
        public const UInt64 MACHINE_TYPE_COMPUTER             = (UInt64)machine_flags.type.TYPE_COMPUTER;
        public const UInt64 MACHINE_TYPE_OTHER                = (UInt64)machine_flags.type.TYPE_OTHER;
        public const UInt64 MACHINE_NOT_WORKING               = (UInt64)machine_flags.type.NOT_WORKING;
        public const UInt64 MACHINE_SUPPORTS_SAVE             = (UInt64)machine_flags.type.SUPPORTS_SAVE;
        public const UInt64 MACHINE_NO_COCKTAIL               = (UInt64)machine_flags.type.NO_COCKTAIL;
        public const UInt64 MACHINE_IS_BIOS_ROOT              = (UInt64)machine_flags.type.IS_BIOS_ROOT;
        public const UInt64 MACHINE_REQUIRES_ARTWORK          = (UInt64)machine_flags.type.REQUIRES_ARTWORK;
        public const UInt64 MACHINE_CLICKABLE_ARTWORK         = (UInt64)machine_flags.type.CLICKABLE_ARTWORK;
        public const UInt64 MACHINE_UNOFFICIAL                = (UInt64)machine_flags.type.UNOFFICIAL;
        public const UInt64 MACHINE_NO_SOUND_HW               = (UInt64)machine_flags.type.NO_SOUND_HW;
        public const UInt64 MACHINE_MECHANICAL                = (UInt64)machine_flags.type.MECHANICAL;
        public const UInt64 MACHINE_IS_INCOMPLETE             = (UInt64)machine_flags.type.IS_INCOMPLETE;

        // flags taht map to device feature flags
        public const UInt64 MACHINE_UNEMULATED_PROTECTION     = 0x0000000100000000;   // game's protection not fully emulated
        public const UInt64 MACHINE_WRONG_COLORS              = 0x0000000200000000;   // colors are totally wrong
        public const UInt64 MACHINE_IMPERFECT_COLORS          = 0x0000000400000000;   // colors are not 100% accurate, but close
        public const UInt64 MACHINE_IMPERFECT_GRAPHICS        = 0x0000000800000000;   // graphics are wrong/incomplete
        public const UInt64 MACHINE_NO_SOUND                  = 0x0000001000000000;   // sound is missing
        public const UInt64 MACHINE_IMPERFECT_SOUND           = 0x0000002000000000;   // sound is known to be wrong
        public const UInt64 MACHINE_IMPERFECT_CONTROLS        = 0x0000004000000000;   // controls are known to be imperfectly emulated
        public const UInt64 MACHINE_NODEVICE_MICROPHONE       = 0x0000008000000000;   // any game/system that has unemulated audio capture device
        public const UInt64 MACHINE_NODEVICE_PRINTER          = 0x0000010000000000;   // any game/system that has unemulated hardcopy output device
        public const UInt64 MACHINE_NODEVICE_LAN              = 0x0000020000000000;   // any game/system that has unemulated local networking
        public const UInt64 MACHINE_IMPERFECT_TIMING          = 0x0000040000000000;   // timing is known to be imperfectly emulated

        // useful combinations of flags
        public const UInt64 MACHINE_IS_SKELETON               = MACHINE_NO_SOUND | MACHINE_NOT_WORKING; // flag combination for skeleton drivers
        public const UInt64 MACHINE_IS_SKELETON_MECHANICAL    = MACHINE_IS_SKELETON | MACHINE_MECHANICAL | MACHINE_REQUIRES_ARTWORK; // flag combination for skeleton mechanical machines


        // wrappers for declaring and defining game drivers
        //#define GAME_NAME(name)         driver_##name
        //#define GAME_TRAITS_NAME(name)  driver_##name##traits
        //#define GAME_EXTERN(name)       extern game_driver const GAME_NAME(name)

        // static game traits
        //#define GAME_DRIVER_TRAITS(NAME, FULLNAME) \
        //namespace { \
        //    struct GAME_TRAITS_NAME(NAME) { static constexpr char const shortname[] = #NAME, fullname[] = FULLNAME, source[] = __FILE__; }; \
        //    constexpr char const GAME_TRAITS_NAME(NAME)::shortname[], GAME_TRAITS_NAME(NAME)::fullname[], GAME_TRAITS_NAME(NAME)::source[]; \
        //}

        public struct game_traits
        {
            public string shortname;
            public string fullname;
            public string source;

            public game_traits(string shortname, string fullname) { this.shortname = shortname; this.fullname = fullname; this.source = "__FILE__"; }
        }

        public static game_traits GAME_DRIVER_TRAITS(string shortname, string fullname) { return new game_traits(shortname, fullname); }


        //#define GAME_DRIVER_TYPE(NAME, CLASS, FLAGS) \
        //driver_device_creator< \
        //        CLASS, \
        //        (GAME_TRAITS_NAME(NAME)::shortname), \
        //        (GAME_TRAITS_NAME(NAME)::fullname), \
        //        (GAME_TRAITS_NAME(NAME)::source), \
        //        game_driver::unemulated_features(FLAGS), \
        //        game_driver::imperfect_features(FLAGS)>

        // standard GAME() macro
        //#define GAME(YEAR,NAME,PARENT,MACHINE,INPUT,CLASS,INIT,MONITOR,COMPANY,FULLNAME,FLAGS) \
        //GAME_DRIVER_TRAITS(NAME,FULLNAME)                                       \
        //extern game_driver const GAME_NAME(NAME)                                \
        //{                                                                       \
        //    GAME_DRIVER_TYPE(NAME, CLASS, FLAGS),                               \
        //    #PARENT,                                                            \
        //    #YEAR,                                                              \
        //    COMPANY,                                                            \
        //    [] (machine_config &config, device_t &owner) { downcast<CLASS &>(owner).MACHINE(config); }, \
        //    INPUT_PORTS_NAME(INPUT),                                            \
        //    [] (device_t &owner) { downcast<CLASS &>(owner).INIT(); },          \
        //    ROM_NAME(NAME),                                                     \
        //    nullptr,                                                            \
        //    nullptr,                                                            \
        //    machine_flags::type(u32((MONITOR) | (FLAGS) | MACHINE_TYPE_ARCADE)),\
        //    #NAME                                                               \
        //};

        public static game_driver GAME(device_type.create_func creator, List<tiny_rom_entry> roms, string YEAR, string NAME, string PARENT, machine_creator_wrapper MACHINE, ioport_constructor INPUT, driver_init_wrapper INIT, UInt32 MONITOR, string COMPANY, string FULLNAME, UInt64 FLAGS)
        {
            var traits = GAME_DRIVER_TRAITS(NAME, FULLNAME);

            device_type game_device = new device_type(new driver_device_creator(creator, traits.shortname, traits.fullname, traits.source, game_driver.unemulated_features(FLAGS), game_driver.imperfect_features(FLAGS), driver_device.unemulated_features(), driver_device.imperfect_features()).driver_tag());

            return new game_driver(game_device, PARENT, YEAR, COMPANY, MACHINE, INPUT, INIT, roms, MONITOR, FLAGS, NAME, FULLNAME);
        }


        // standard macro with additional layout
        //#define GAMEL(YEAR,NAME,PARENT,MACHINE,INPUT,CLASS,INIT,MONITOR,COMPANY,FULLNAME,FLAGS,LAYOUT) \
        //GAME_DRIVER_TRAITS(NAME,FULLNAME)                                       \
        //extern game_driver const GAME_NAME(NAME)                                \
        //{                                                                       \
        //    GAME_DRIVER_TYPE(NAME, CLASS, FLAGS),                               \
        //    #PARENT,                                                            \
        //    #YEAR,                                                              \
        //    COMPANY,                                                            \
        //    [] (machine_config &config, device_t &owner) { downcast<CLASS &>(owner).MACHINE(config); }, \
        //    INPUT_PORTS_NAME(INPUT),                                            \
        //    [] (device_t &owner) { downcast<CLASS &>(owner).INIT(); },          \
        //    ROM_NAME(NAME),                                                     \
        //    nullptr,                                                            \
        //    &LAYOUT,                                                            \
        //    machine_flags::type(u32((MONITOR) | (FLAGS) | MACHINE_TYPE_ARCADE)),\
        //    #NAME                                                               \
        //};


        // standard console definition macro
        //#define CONS(YEAR,NAME,PARENT,COMPAT,MACHINE,INPUT,CLASS,INIT,COMPANY,FULLNAME,FLAGS) \
        //GAME_DRIVER_TRAITS(NAME,FULLNAME)                                       \
        //extern game_driver const GAME_NAME(NAME)                                \
        //{                                                                       \
        //    GAME_DRIVER_TYPE(NAME, CLASS, FLAGS),                               \
        //    #PARENT,                                                            \
        //    #YEAR,                                                              \
        //    COMPANY,                                                            \
        //    [] (machine_config &config, device_t &owner) { downcast<CLASS &>(owner).MACHINE(config); }, \
        //    INPUT_PORTS_NAME(INPUT),                                            \
        //    [] (device_t &owner) { downcast<CLASS &>(owner).INIT(); },          \
        //    ROM_NAME(NAME),                                                     \
        //    #COMPAT,                                                            \
        //    nullptr,                                                            \
        //    machine_flags::type(u32(ROT0 | (FLAGS) | MACHINE_TYPE_CONSOLE)),    \
        //    #NAME                                                               \
        //};

        // standard computer definition macro
        //#define COMP(YEAR,NAME,PARENT,COMPAT,MACHINE,INPUT,CLASS,INIT,COMPANY,FULLNAME,FLAGS) \
        //GAME_DRIVER_TRAITS(NAME,FULLNAME)                                       \
        //extern game_driver const GAME_NAME(NAME)                                \
        //{                                                                       \
        //    GAME_DRIVER_TYPE(NAME, CLASS, FLAGS),                               \
        //    #PARENT,                                                            \
        //    #YEAR,                                                              \
        //    COMPANY,                                                            \
        //    [] (machine_config &config, device_t &owner) { downcast<CLASS &>(owner).MACHINE(config); }, \
        //    INPUT_PORTS_NAME(INPUT),                                            \
        //    [] (device_t &owner) { downcast<CLASS &>(owner).INIT(); },          \
        //    ROM_NAME(NAME),                                                     \
        //    #COMPAT,                                                            \
        //    nullptr,                                                            \
        //    machine_flags::type(u32(ROT0 | (FLAGS) | MACHINE_TYPE_COMPUTER)),   \
        //    #NAME                                                               \
        //};

        // standard system definition macro
        //#define SYST(YEAR,NAME,PARENT,COMPAT,MACHINE,INPUT,CLASS,INIT,COMPANY,FULLNAME,FLAGS) \
        //GAME_DRIVER_TRAITS(NAME,FULLNAME)                                       \
        //extern game_driver const GAME_NAME(NAME)                                \
        //{                                                                       \
        //    GAME_DRIVER_TYPE(NAME, CLASS, FLAGS),                               \
        //    #PARENT,                                                            \
        //    #YEAR,                                                              \
        //    COMPANY,                                                            \
        //    [] (machine_config &config, device_t &owner) { downcast<CLASS &>(owner).MACHINE(config); }, \
        //    INPUT_PORTS_NAME(INPUT),                                            \
        //    [] (device_t &owner) { downcast<CLASS &>(owner).INIT(); },          \
        //    ROM_NAME(NAME),                                                     \
        //    #COMPAT,                                                            \
        //    nullptr,                                                            \
        //    machine_flags::type(u32(ROT0 | (FLAGS) | MACHINE_TYPE_OTHER)),      \
        //    #NAME                                                               \
        //};
    }


    public struct machine_flags
    {
        public enum type //: u32
        {
            MASK_ORIENTATION    = 0x00000007,
            MASK_TYPE           = 0x00000038,

            FLIP_X              = 0x00000001,
            FLIP_Y              = 0x00000002,
            SWAP_XY             = 0x00000004,
            ROT0                = 0x00000000,
            ROT90               = FLIP_X | SWAP_XY,
            ROT180              = FLIP_X | FLIP_Y,
            ROT270              = FLIP_Y | SWAP_XY,

            TYPE_ARCADE         = 0x00000008,   // coin-operated machine for public use
            TYPE_CONSOLE        = 0x00000010,   // console system
            TYPE_COMPUTER       = 0x00000018,   // any kind of computer including home computers, minis, calculators, ...
            TYPE_OTHER          = 0x00000038,   // any other emulated system (e.g. clock, satellite receiver, ...)

            NOT_WORKING         = 0x00000040,
            SUPPORTS_SAVE       = 0x00000080,   // system supports save states
            NO_COCKTAIL         = 0x00000100,   // screen flip support is missing
            IS_BIOS_ROOT        = 0x00000200,   // this driver entry is a BIOS root
            REQUIRES_ARTWORK    = 0x00000400,   // requires external artwork for key game elements
            CLICKABLE_ARTWORK   = 0x00000800,   // artwork is clickable and requires mouse cursor
            UNOFFICIAL          = 0x00001000,   // unofficial hardware modification
            NO_SOUND_HW         = 0x00002000,   // system has no sound output
            MECHANICAL          = 0x00004000,   // contains mechanical parts (pinball, redemption games, ...)
            IS_INCOMPLETE       = 0x00008000    // official system with blatantly incomplete hardware/software
        }
    }
    //DECLARE_ENUM_BITWISE_OPERATORS(machine_flags::type);


    public class game_driver
    {
        public device_type type;               // static type info for driver class
        public string parent;                     // if this is a clone, the name of the parent
        public string year;                       // year the game was released
        public string manufacturer;               // manufacturer of the game
        public machine_creator_wrapper machine_creator;  // machine driver tokens
        public ioport_constructor ipt;            // pointer to constructor for input ports
        public driver_init_wrapper driver_init;      // DRIVER_INIT callback
        public List<tiny_rom_entry> rom;               // pointer to list of ROMs for the game
        public string compatible_with;
        public internal_layout default_layout;             // default internally defined layout
        public machine_flags.type flags;                      // orientation and other flags; see defines below
        public string name;                       // short name of the game


        public game_driver(device_type type, string parent, string year, string manufacturer, machine_creator_wrapper machine_creator, ioport_constructor ipt, driver_init_wrapper driver_init, List<tiny_rom_entry> rom, UInt32 monitor, UInt64 flags, string name, string fullname)
        {
            this.type = type;
            this.parent = parent;
            this.year = year;
            this.manufacturer = manufacturer;
            this.machine_creator = machine_creator;
            this.ipt = ipt;
            this.driver_init = driver_init;
            this.rom = rom;
            this.compatible_with = null;
            this.default_layout = null;
            this.flags = (machine_flags.type)(monitor | flags | gamedrv_global.MACHINE_TYPE_ARCADE);
            this.name = name;
        }


        public static emu.detail.device_feature.type unemulated_features(UInt64 flags)
        {
            return
                    ((flags & gamedrv_global.MACHINE_WRONG_COLORS) != 0             ? emu.detail.device_feature.type.PALETTE    : emu.detail.device_feature.type.NONE) |
                    ((flags & gamedrv_global.MACHINE_NO_SOUND) != 0                 ? emu.detail.device_feature.type.SOUND      : emu.detail.device_feature.type.NONE) |
                    ((flags & gamedrv_global.MACHINE_NODEVICE_MICROPHONE) != 0      ? emu.detail.device_feature.type.MICROPHONE : emu.detail.device_feature.type.NONE) |
                    ((flags & gamedrv_global.MACHINE_NODEVICE_PRINTER) != 0         ? emu.detail.device_feature.type.PRINTER    : emu.detail.device_feature.type.NONE) |
                    ((flags & gamedrv_global.MACHINE_NODEVICE_LAN) != 0             ? emu.detail.device_feature.type.LAN        : emu.detail.device_feature.type.NONE);
        }

        public static emu.detail.device_feature.type imperfect_features(UInt64 flags)
        {
            return
                    ((flags & gamedrv_global.MACHINE_UNEMULATED_PROTECTION) != 0    ? emu.detail.device_feature.type.PROTECTION : emu.detail.device_feature.type.NONE) |
                    ((flags & gamedrv_global.MACHINE_IMPERFECT_COLORS) != 0         ? emu.detail.device_feature.type.PALETTE    : emu.detail.device_feature.type.NONE) |
                    ((flags & gamedrv_global.MACHINE_IMPERFECT_GRAPHICS) != 0       ? emu.detail.device_feature.type.GRAPHICS   : emu.detail.device_feature.type.NONE) |
                    ((flags & gamedrv_global.MACHINE_IMPERFECT_SOUND) != 0          ? emu.detail.device_feature.type.SOUND      : emu.detail.device_feature.type.NONE) |
                    ((flags & gamedrv_global.MACHINE_IMPERFECT_CONTROLS) != 0       ? emu.detail.device_feature.type.CONTROLS   : emu.detail.device_feature.type.NONE) |
                    ((flags & gamedrv_global.MACHINE_IMPERFECT_TIMING) != 0         ? emu.detail.device_feature.type.TIMING     : emu.detail.device_feature.type.NONE);
        }
    }
}
