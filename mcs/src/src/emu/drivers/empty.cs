// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

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


        public void ___empty(machine_config config)
        {
            // video hardware
            screen_device screen = SCREEN(config, "screen", SCREEN_TYPE_RASTER);
            screen.set_screen_update(screen_update);
            screen.set_size(640, 480);
            screen.set_visarea(0, 639, 0, 479);
            screen.set_refresh_hz(30);
        }


        public override std.vector<string> searchpath() { return new std.vector<string>(); }


        protected override void machine_start()
        {
            emulator_info.display_ui_chooser(machine());
        }


        u32 screen_update(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            bitmap.fill(rgb_t.black(), cliprect);
            return 0;
        }
    }


    class ___empty : global_object
    {
        //ROM_START( ___empty )
        static readonly List<tiny_rom_entry> rom____empty = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x10, "user1", ROMREGION_ERASEFF ),
            ROM_END,
        };


        static void empty_state____empty(machine_config config, device_t device) { ((empty_state)device).___empty(config); }


        static ___empty m____empty = new ___empty();


        static device_t device_creator____empty(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new empty_state(mconfig, (device_type)type, tag); }


        //                                                        creator,                 rom           YEAR,   NAME,       PARENT,  MACHINE,              INPUT, INIT,                     MONITOR,COMPANY, FULLNAME,FLAGS
        public static readonly game_driver driver____empty = GAME(device_creator____empty, rom____empty, "2007", "___empty", "0",     empty_state____empty, null,  driver_device.empty_init, ROT0, "MAME", "No Driver Loaded", MACHINE_NO_SOUND_HW );
    }
}
