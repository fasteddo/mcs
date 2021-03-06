// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


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
            new options_entry(null,                             null,      OPTION_HEADER,     "CORE COMMANDS"),
            new options_entry(CLICOMMAND_HELP + ";h;?",         "0",       OPTION_COMMAND,    "show help message"),
            new options_entry(CLICOMMAND_VALIDATE + ";valid",   "0",       OPTION_COMMAND,    "perform driver validation on all game drivers"),

            /* configuration commands */
            new options_entry(null,                             null,      OPTION_HEADER,     "CONFIGURATION COMMANDS"),
            new options_entry(CLICOMMAND_CREATECONFIG + ";cc",  "0",       OPTION_COMMAND,    "create the default configuration file"),
            new options_entry(CLICOMMAND_SHOWCONFIG + ";sc",    "0",       OPTION_COMMAND,    "display running parameters"),
            new options_entry(CLICOMMAND_SHOWUSAGE + ";su",     "0",       OPTION_COMMAND,    "show this help"),

            /* frontend commands */
            new options_entry(null,                             null,      OPTION_HEADER,     "FRONTEND COMMANDS"),
            new options_entry(CLICOMMAND_LISTXML + ";lx",       "0",       OPTION_COMMAND,    "all available info on driver in XML format"),
            new options_entry(CLICOMMAND_LISTFULL + ";ll",      "0",       OPTION_COMMAND,    "short name, full name"),
            new options_entry(CLICOMMAND_LISTSOURCE + ";ls",    "0",       OPTION_COMMAND,    "driver sourcefile"),
            new options_entry(CLICOMMAND_LISTCLONES + ";lc",    "0",       OPTION_COMMAND,    "show clones"),
            new options_entry(CLICOMMAND_LISTBROTHERS + ";lb",  "0",       OPTION_COMMAND,    "show \"brothers\", or other drivers from same sourcefile"),
            new options_entry(CLICOMMAND_LISTCRC,               "0",       OPTION_COMMAND,    "CRC-32s"),
            new options_entry(CLICOMMAND_LISTROMS + ";lr",      "0",       OPTION_COMMAND,    "list required roms for a driver"),
            new options_entry(CLICOMMAND_LISTSAMPLES,           "0",       OPTION_COMMAND,    "list optional samples for a driver"),
            new options_entry(CLICOMMAND_VERIFYROMS,            "0",       OPTION_COMMAND,    "report romsets that have problems"),
            new options_entry(CLICOMMAND_VERIFYSAMPLES,         "0",       OPTION_COMMAND,    "report samplesets that have problems"),
            new options_entry(CLICOMMAND_ROMIDENT,              "0",       OPTION_COMMAND,    "compare files with known MAME roms"),
            new options_entry(CLICOMMAND_LISTDEVICES + ";ld",   "0",       OPTION_COMMAND,    "list available devices"),
            new options_entry(CLICOMMAND_LISTSLOTS + ";lslot",  "0",       OPTION_COMMAND,    "list available slots and slot devices"),
            new options_entry(CLICOMMAND_LISTMEDIA + ";lm",     "0",       OPTION_COMMAND,    "list available media for the system"),
            new options_entry(CLICOMMAND_LISTSOFTWARE + ";lsoft", "0",     OPTION_COMMAND,    "list known software for the system"),
            new options_entry(CLICOMMAND_VERIFYSOFTWARE + ";vsoft", "0",   OPTION_COMMAND,    "verify known software for the system"),
            new options_entry(CLICOMMAND_GETSOFTLIST + ";glist", "0",      OPTION_COMMAND,    "retrieve software list by name"),
            new options_entry(CLICOMMAND_VERIFYSOFTLIST + ";vlist", "0",   OPTION_COMMAND,    "verify software list by name"),
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
