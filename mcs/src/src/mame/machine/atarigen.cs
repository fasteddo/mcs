// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_timer_id = System.UInt32;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;


namespace mame
{
    abstract class atarigen_state : driver_device
    {
        // timer IDs
        //enum
        //{
        const int TID_UNHALT_CPU = 0;
        const int TID_ATARIGEN_LAST = 1;
        //}


        /* internal state */
        u8 m_slapstic_num;
        Pointer<u8> m_slapsticU8;  //u16 *            m_slapstic;
        u8 m_slapstic_bank;
        std.vector<u8> m_slapstic_bank0;
        UInt32 m_slapstic_last_pc;
        UInt32 m_slapstic_last_address;
        UInt32 m_slapstic_base;
        UInt32 m_slapstic_mirror;

        required_device<cpu_device> m_maincpu;

        protected optional_device<gfxdecode_device> m_gfxdecode;
        protected optional_device<screen_device> m_screen;
        optional_device<atari_slapstic_device> m_slapstic_device;


        // construction/destruction
        protected atarigen_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_slapstic_num = 0;
            m_slapsticU8 = null;
            m_slapstic_bank = 0;
            m_slapstic_last_pc = 0;
            m_slapstic_last_address = 0;
            m_slapstic_base = 0;
            m_slapstic_mirror = 0;
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_gfxdecode = new optional_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new optional_device<screen_device>(this, "screen");
            m_slapstic_device = new optional_device<atari_slapstic_device>(this, ":slapstic");
        }


        // users must call through to these
        protected override void machine_start()
        {
            save_item(NAME(new { m_slapstic_num }));
            save_item(NAME(new { m_slapstic_bank }));
            save_item(NAME(new { m_slapstic_last_pc }));
            save_item(NAME(new { m_slapstic_last_address }));
        }


        protected override void machine_reset()
        {
            // reset the slapstic
            if (m_slapstic_num != 0)
            {
                if (!m_slapstic_device.found())
                    fatalerror("Slapstic device is missing?\n");

                m_slapstic_device.target.slapstic_reset();
                slapstic_update_bank(m_slapstic_device.target.slapstic_bank());
            }
        }


        protected override void device_post_load()
        {
            if (m_slapstic_num != 0)
            {
                if (!m_slapstic_device.found())
                    fatalerror("Slapstic device is missing?\n");

                slapstic_update_bank(m_slapstic_device.target.slapstic_bank());
            }
        }


        protected override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr)
        {
            switch (id)
            {
                // unhalt the CPU that was passed as a pointer
                case TID_UNHALT_CPU:
                    ((device_t)ptr).execute().set_input_line(INPUT_LINE_HALT, CLEAR_LINE);
                    break;
            }
        }


        // slapstic helpers
        //void slapstic_configure(cpu_device &device, offs_t base, offs_t mirror, u8 *mem);


        /***************************************************************************
            SLAPSTIC HANDLING
        ***************************************************************************/
        void slapstic_update_bank(int bank)
        {
            // if the bank has changed, copy the memory; Pit Fighter needs this
            if (bank != m_slapstic_bank)
            {
                // bank 0 comes from the copy we made earlier
                if (bank == 0)
                    memcpy(m_slapsticU8, new Pointer<u8>(m_slapstic_bank0, 0), 0x2000);  //memcpy(m_slapstic, &m_slapstic_bank0[0], 0x2000);
                else
                    memcpy(m_slapsticU8, new Pointer<u8>(m_slapsticU8, bank * 0x1000 * 2), 0x2000);  //memcpy(m_slapstic, &m_slapstic[bank * 0x1000], 0x2000);

                // remember the current bank
                m_slapstic_bank = (u8)bank;
            }
        }


        //DECLARE_WRITE16_MEMBER(slapstic_w);
        void slapstic_w(address_space space, offs_t offset, u16 data, u16 mem_mask)
        {
            throw new emu_unimplemented();
        }


        //DECLARE_READ16_MEMBER(slapstic_r);
        u16 slapstic_r(address_space space, offs_t offset, u16 mem_mask)
        {
            throw new emu_unimplemented();
        }


        // video helpers
        //void halt_until_hblank_0(device_t &device, screen_device &screen);

        // misc helpers
        //void blend_gfx(int gfx0, int gfx1, int mask0, int mask1);
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
