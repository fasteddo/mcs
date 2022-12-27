// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using size_t = System.UInt64;


namespace mame
{
    public static class drivlist_global
    {
        // these variables are populated in \build\generated\mame\mame\drivlist.cs
        public static game_driver [] s_drivers_sorted;
        public static size_t s_driver_count;
    }
}
