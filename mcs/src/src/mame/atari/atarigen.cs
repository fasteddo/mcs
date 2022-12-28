// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;

using static mame.diexec_global;


namespace mame
{
    abstract class atarigen_state : driver_device
    {
        required_device<cpu_device> m_maincpu;

        protected optional_device<gfxdecode_device> m_gfxdecode;
        protected optional_device<screen_device> m_screen;

        //std::unique_ptr<u8[]> m_blended_data;

        emu_timer m_unhalt_cpu_timer;


        // construction/destruction
        protected atarigen_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_gfxdecode = new optional_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new optional_device<screen_device>(this, "screen");
            m_unhalt_cpu_timer = null;
        }


        // users must call through to these
        protected override void machine_start()
        {
            m_unhalt_cpu_timer = timer_alloc(unhalt_cpu);
        }


        protected override void machine_reset()
        {
        }


        // video helpers
        //void halt_until_hblank_0(device_t &device, screen_device &screen);

        // misc helpers
        //void blend_gfx(int gfx0, int gfx1, int mask0, int mask1);

        //TIMER_CALLBACK_MEMBER(unhalt_cpu);
        void unhalt_cpu(s32 param)
        {
            m_maincpu.op0.set_input_line(INPUT_LINE_HALT, CLEAR_LINE);
        }
    }


    /***************************************************************************
        GENERAL ATARI NOTES
    **************************************************************************##

        Atari 68000 list:

        Driver        Pr? Up? VC? PF? P2? MO? AL? BM? PH?
        ----------    --- --- --- --- --- --- --- --- ---
        arcadecl.cpp       *               *       *
        atarig1.cpp        *       *      rle  *
        atarig42.cpp       *       *      rle  *
        atarigt.cpp                *      rle  *
        atarigx2.cpp               *      rle  *
        atarisy1.cpp   *   *       *       *   *              270->260
        atarisy2.cpp   *   *       *       *   *              150->120
        badlands.cpp       *       *       *                  250->260
        batman.cpp     *   *   *   *   *   *   *       *      200->160 ?
        blstroid.cpp       *       *       *                  240->230
        cyberbal.cpp       *       *       *   *              125->105 ?
        eprom.cpp          *       *       *   *              170->170
        gauntlet.cpp   *   *       *       *   *       *      220->250
        klax.cpp       *   *       *       *                  480->440 ?
        offtwall.cpp       *   *   *       *                  260->260
        rampart.cpp        *               *       *          280->280
        relief.cpp     *   *   *   *   *   *                  240->240
        shuuz.cpp          *   *   *       *                  410->290 fix!
        skullxbo.cpp       *       *       *   *              150->145
        thunderj.cpp       *   *   *   *   *   *       *      180->180
        toobin.cpp         *       *       *   *              140->115 fix!
        vindictr.cpp   *   *       *       *   *       *      200->210
        xybots.cpp     *   *       *       *   *              235->238
        ----------  --- --- --- --- --- --- --- --- ---

        Pr? - do we have verifiable proof on priorities?
        Up? - have we updated to use new MO's & tilemaps?
        VC? - does it use the video controller?
        PF? - does it have a playfield?
        P2? - does it have a dual playfield?
        MO? - does it have MO's?
        AL? - does it have an alpha layer?
        BM? - does it have a bitmap layer?
        PH? - does it use the palette hack?

    ***************************************************************************/
}
