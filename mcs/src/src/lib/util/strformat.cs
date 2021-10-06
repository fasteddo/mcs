// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static partial class util
    {
        public static string string_format(string format, params object [] args) { return string.Format(format, args); }
    }
}
