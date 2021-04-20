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
            _1942.driver_1942,
            atarisy2.driver_720,
            atarisy2.driver_paperboy,
            centiped.driver_centipede,
            dkong.driver_dkong,
            galaga.driver_digdug,
            galaga.driver_galaga,
            galaga.driver_xevious,
            galaxian.driver_frogger,
            galaxian.driver_galaxian,
            m52.driver_mpatrol,
            pacman.driver_mspacman,
            pacman.driver_pacman,
            pacman.driver_pacplus,
            pacman.driver_puckman,
            taitosj.driver_elevator,
            taitosj.driver_jungleh,
            taitosj.driver_junglek,
        };

        public static readonly int s_driver_count = s_drivers_sorted.Count;
    }
}
