// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using size_t = System.UInt64;


namespace mame
{
    public static class drivlist_global
    {
        // extern const game_driver driver_pacman
 
        public static readonly game_driver [] s_drivers_sorted = 
        {
            ___empty.driver____empty,
            _1942.driver_1942,
            asteroid.driver_asteroid,
            atarisy2.driver_720,
            atarisy2.driver_paperboy,
            btime.driver_btime,
            bzone.driver_bzone,
            cclimber.driver_cclimber,
            centiped.driver_centipede,
            centiped.driver_milliped,
            dkong.driver_dkong,
            dkong.driver_dkong3,
            dkong.driver_dkongjr,
            fastfred.driver_fastfred,
            fastfred.driver_flyboy,
            galaga.driver_digdug,
            galaga.driver_galaga,
            galaga.driver_xevious,
            galaxian.driver_frogger,
            galaxian.driver_galaxian,
            m52.driver_mpatrol,
            mcr.driver_demoderb,
            mcr.driver_dotron,
            mcr.driver_tapper,
            mcr.driver_tron,
            mcr.driver_twotiger,
            missile.driver_missile,
            mw8080bw.driver_gunfight,
            mw8080bw.driver_invaders,
            pacman.driver_mspacman,
            pacman.driver_pacman,
            pacman.driver_pacplus,
            pacman.driver_puckman,
            polepos.driver_polepos,
            pong.driver_pong,
            rallyx.driver_rallyx,
            system1.driver_choplift,
            taitosj.driver_elevator,
            taitosj.driver_jungleh,
            taitosj.driver_junglek,
            williams.driver_defender,
            williams.driver_joust,
            williams.driver_robotron,
            williams.driver_sinistar,
            williams.driver_stargate,
        };

        public static readonly size_t s_driver_count = (size_t)s_drivers_sorted.Length;
    }
}
