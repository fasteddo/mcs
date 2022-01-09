// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using size_t = System.UInt64;


namespace mame
{
    public static class drivlist_global
    {
        // extern const game_driver driver_pacman
 
        public static readonly game_driver [] s_drivers_sorted = new game_driver []
        {
            ___empty.driver____empty,
            _1942.driver_1942,
            asteroid.driver_asteroid,
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
            mw8080bw.driver_invaders,
            pacman.driver_mspacman,
            pacman.driver_pacman,
            pacman.driver_pacplus,
            pacman.driver_puckman,
            pong.driver_pong,
            taitosj.driver_elevator,
            taitosj.driver_jungleh,
            taitosj.driver_junglek,
        };

        public static readonly size_t s_driver_count = (size_t)s_drivers_sorted.Length;
    }
}
