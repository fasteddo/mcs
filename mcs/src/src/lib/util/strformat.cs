// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    public static partial class util
    {
        public static void stream_format(ref string str, string format, params object [] args) { str += string.Format(format, args); }
        public static string string_format(string format, params object [] args) { return string.Format(format, args); }
    }
}
