// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.options_global;


namespace mame
{
    // cli_options wraps the general emu options with CLI-specific additions
    public class cli_options : emu_options
    {
        // core commands
        public const string CLICOMMAND_HELP                = "help";
        public const string CLICOMMAND_VALIDATE            = "validate";

        // configuration commands
        public const string CLICOMMAND_CREATECONFIG        = "createconfig";
        public const string CLICOMMAND_SHOWCONFIG          = "showconfig";
        public const string CLICOMMAND_SHOWUSAGE           = "showusage";

        // frontend commands
        public const string CLICOMMAND_LISTXML             = "listxml";
        public const string CLICOMMAND_LISTFULL            = "listfull";
        public const string CLICOMMAND_LISTSOURCE          = "listsource";
        public const string CLICOMMAND_LISTCLONES          = "listclones";
        public const string CLICOMMAND_LISTBROTHERS        = "listbrothers";
        public const string CLICOMMAND_LISTCRC             = "listcrc";
        public const string CLICOMMAND_LISTROMS            = "listroms";
        public const string CLICOMMAND_LISTSAMPLES         = "listsamples";
        public const string CLICOMMAND_VERIFYROMS          = "verifyroms";
        public const string CLICOMMAND_VERIFYSAMPLES       = "verifysamples";
        public const string CLICOMMAND_ROMIDENT            = "romident";
        public const string CLICOMMAND_LISTDEVICES         = "listdevices";
        public const string CLICOMMAND_LISTSLOTS           = "listslots";
        public const string CLICOMMAND_LISTMEDIA           = "listmedia";     // needed by MESS
        public const string CLICOMMAND_LISTSOFTWARE        = "listsoftware";
        public const string CLICOMMAND_VERIFYSOFTWARE      = "verifysoftware";
        public const string CLICOMMAND_GETSOFTLIST         = "getsoftlist";
        public const string CLICOMMAND_VERIFYSOFTLIST      = "verifysoftlist";


        static readonly options_entry [] s_option_entries = new options_entry []
        {
            /* core commands */
            new options_entry(null,                             null,      core_options.option_type.HEADER,     "CORE COMMANDS"),
            new options_entry(CLICOMMAND_HELP + ";h;?",         "0",       core_options.option_type.COMMAND,    "show help message"),
            new options_entry(CLICOMMAND_VALIDATE + ";valid",   "0",       core_options.option_type.COMMAND,    "perform driver validation on all game drivers"),

            /* configuration commands */
            new options_entry(null,                             null,      core_options.option_type.HEADER,     "CONFIGURATION COMMANDS"),
            new options_entry(CLICOMMAND_CREATECONFIG + ";cc",  "0",       core_options.option_type.COMMAND,    "create the default configuration file"),
            new options_entry(CLICOMMAND_SHOWCONFIG + ";sc",    "0",       core_options.option_type.COMMAND,    "display running parameters"),
            new options_entry(CLICOMMAND_SHOWUSAGE + ";su",     "0",       core_options.option_type.COMMAND,    "show this help"),

            /* frontend commands */
            new options_entry(null,                             null,      core_options.option_type.HEADER,     "FRONTEND COMMANDS"),
            new options_entry(CLICOMMAND_LISTXML + ";lx",       "0",       core_options.option_type.COMMAND,    "all available info on driver in XML format"),
            new options_entry(CLICOMMAND_LISTFULL + ";ll",      "0",       core_options.option_type.COMMAND,    "short name, full name"),
            new options_entry(CLICOMMAND_LISTSOURCE + ";ls",    "0",       core_options.option_type.COMMAND,    "driver sourcefile"),
            new options_entry(CLICOMMAND_LISTCLONES + ";lc",    "0",       core_options.option_type.COMMAND,    "show clones"),
            new options_entry(CLICOMMAND_LISTBROTHERS + ";lb",  "0",       core_options.option_type.COMMAND,    "show \"brothers\", or other drivers from same sourcefile"),
            new options_entry(CLICOMMAND_LISTCRC,               "0",       core_options.option_type.COMMAND,    "CRC-32s"),
            new options_entry(CLICOMMAND_LISTROMS + ";lr",      "0",       core_options.option_type.COMMAND,    "list required roms for a driver"),
            new options_entry(CLICOMMAND_LISTSAMPLES,           "0",       core_options.option_type.COMMAND,    "list optional samples for a driver"),
            new options_entry(CLICOMMAND_VERIFYROMS,            "0",       core_options.option_type.COMMAND,    "report romsets that have problems"),
            new options_entry(CLICOMMAND_VERIFYSAMPLES,         "0",       core_options.option_type.COMMAND,    "report samplesets that have problems"),
            new options_entry(CLICOMMAND_ROMIDENT,              "0",       core_options.option_type.COMMAND,    "compare files with known MAME roms"),
            new options_entry(CLICOMMAND_LISTDEVICES + ";ld",   "0",       core_options.option_type.COMMAND,    "list available devices"),
            new options_entry(CLICOMMAND_LISTSLOTS + ";lslot",  "0",       core_options.option_type.COMMAND,    "list available slots and slot devices"),
            new options_entry(CLICOMMAND_LISTMEDIA + ";lm",     "0",       core_options.option_type.COMMAND,    "list available media for the system"),
            new options_entry(CLICOMMAND_LISTSOFTWARE + ";lsoft", "0",     core_options.option_type.COMMAND,    "list known software for the system"),
            new options_entry(CLICOMMAND_VERIFYSOFTWARE + ";vsoft", "0",   core_options.option_type.COMMAND,    "verify known software for the system"),
            new options_entry(CLICOMMAND_GETSOFTLIST + ";glist", "0",      core_options.option_type.COMMAND,    "retrieve software list by name"),
            new options_entry(CLICOMMAND_VERIFYSOFTLIST + ";vlist", "0",   core_options.option_type.COMMAND,    "verify software list by name"),
            new options_entry(null),
        };


        // construction/destruction
        //-------------------------------------------------
        //  cli_options - constructor
        //-------------------------------------------------
        public cli_options() : base()
        {
            add_entries(s_option_entries);
        }
    }
}
