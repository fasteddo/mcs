// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class mame_global_emulator_info
    {
        const string APPNAME                 = "MAME";
        const string APPNAME_LOWER           = "mame";
        const string CONFIGNAME              = "mame";
        const string COPYRIGHT               = "Copyright Nicola Salmoria\nand the MAME team\nhttp://mamedev.org";
        const string COPYRIGHT_INFO          = "Copyright Nicola Salmoria and the MAME team";


        public static string get_appname() { return APPNAME;}
        public static string get_appname_lower() { return APPNAME_LOWER;}
        public static string get_configname() { return CONFIGNAME;}
        public static string get_copyright() { return COPYRIGHT;}
        public static string get_copyright_info() { return COPYRIGHT_INFO;}
    }
}
