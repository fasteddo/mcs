// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.version_global;
using static mame.version_internal;


namespace mame
{
    public static class version_internal
    {
        public const string BARE_BUILD_VERSION = "0.242";
    }

    public static class version_global
    {
        public const string bare_build_version = BARE_BUILD_VERSION;
        public const string build_version = BARE_BUILD_VERSION + " (mcs)";
    }
}
