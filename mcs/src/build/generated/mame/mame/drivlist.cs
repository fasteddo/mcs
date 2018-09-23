// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class drivlist_global
    {
        // extern const game_driver driver_pacman
 
        public static readonly List<game_driver> s_drivers_sorted = new List<game_driver>()
        {
            ___empty.driver____empty,
            pacman.driver_puckman,
            pacman.driver_pacman,
            pacman.driver_mspacman,
            pacman.driver_pacplus,
            galaga.driver_galaga,
            galaga.driver_xevious,
            galaga.driver_digdug,
            //_1942.driver_1942,
            centiped.driver_centipede,
            galaxian.driver_galaxian,
            galaxian.driver_frogger,
        };

        public static readonly UInt32 s_driver_count = (UInt32)s_drivers_sorted.Count;
    }
}
