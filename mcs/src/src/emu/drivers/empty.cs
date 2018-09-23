// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using u32 = System.UInt32;


namespace mame
{
    class empty_state : driver_device
    {
        // constructor
        public empty_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
        }


        protected override void machine_start()
        {
            emulator_info.display_ui_chooser(machine());
        }


        public static u32 screen_update____empty(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            bitmap.fill(rgb_t.black(), cliprect);
            return 0;
        }
    }


    public class ___empty : device_init_helpers
    {
        //MACHINE_CONFIG_START( empty_state::___empty )
        void empty_state____empty(machine_config config, device_t owner, device_t device)
        {
            MACHINE_CONFIG_START(config, owner, device);

            // video hardware
            MCFG_SCREEN_ADD("screen", SCREEN_TYPE_RASTER);
            MCFG_SCREEN_UPDATE_DRIVER(empty_state.screen_update____empty);
            MCFG_SCREEN_SIZE(640, 480);
            MCFG_SCREEN_VISIBLE_AREA(0, 639, 0, 479);
            MCFG_SCREEN_REFRESH_RATE(30);

            MACHINE_CONFIG_END();
        }


        //ROM_START( ___empty )
        static readonly List<tiny_rom_entry> rom____empty = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x10, "user1", ROMREGION_ERASEFF ),
            ROM_END(),
        };


        static ___empty m____empty = new ___empty();


        static device_t device_creator____empty(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new empty_state(mconfig, type, tag); }


        //                                                        creator,                 rom           YEAR,   NAME,       PARENT,  MACHINE,                         INPUT, INIT,                     MONITOR,COMPANY, FULLNAME,FLAGS
        public static readonly game_driver driver____empty = GAME(device_creator____empty, rom____empty, "2007", "___empty", null,    m____empty.empty_state____empty, null,  driver_device.empty_init, ROT0, "MAME", "No Driver Loaded", MACHINE_NO_SOUND_HW );
    }
}
