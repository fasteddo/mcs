// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.ui
{
    class menu_network_devices : menu
    {
        public menu_network_devices(mame_ui_manager mui, render_container container) : base(mui, container) { }
        //~menu_network_devices() { }


        /*-------------------------------------------------
            menu_network_devices_populate - populates the main
            network device menu
        -------------------------------------------------*/
        protected override void populate(ref float customtop, ref float custombottom)
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            ui_menu_network_devices - menu that
        -------------------------------------------------*/
        protected override void handle()
        {
            throw new emu_unimplemented();
        }
    }


    class menu_bookkeeping : menu
    {
        attotime prevtime;


        /*-------------------------------------------------
            menu_bookkeeping - handle the bookkeeping
            information menu
        -------------------------------------------------*/
        public menu_bookkeeping(mame_ui_manager mui, render_container container) : base(mui, container) { }
        //~menu_bookkeeping() { }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            throw new emu_unimplemented();
        }


        protected override void handle()
        {
            throw new emu_unimplemented();
        }
    }


    class menu_crosshair : menu
    {
#if false
        enum
        {
            CROSSHAIR_ITEM_VIS = 0,
            CROSSHAIR_ITEM_PIC,
            CROSSHAIR_ITEM_AUTO_TIME
        }
#endif


        class crosshair_item_data
        {
            //UINT8               type;
            //UINT8               player;
            //UINT8               min, max;
            //UINT8               cur;
            //UINT8               defvalue;
            //char                last_name[CROSSHAIR_PIC_NAME_LENGTH + 1];
            //char                next_name[CROSSHAIR_PIC_NAME_LENGTH + 1];
        }


        /*-------------------------------------------------
            menu_crosshair - handle the crosshair settings
            menu
        -------------------------------------------------*/
        public menu_crosshair(mame_ui_manager mui, render_container container) : base(mui, container) { }
        //~menu_crosshair() { }


        /*-------------------------------------------------
            menu_crosshair_populate - populate the
            crosshair settings menu
        -------------------------------------------------*/
        protected override void populate(ref float customtop, ref float custombottom)
        {
            throw new emu_unimplemented();
        }


        protected override void handle()
        {
            throw new emu_unimplemented();
        }
    }


    class menu_quit_game : menu
    {
        /*-------------------------------------------------
            menu_quit_game - handle the "menu" for
            quitting the game
        -------------------------------------------------*/
        public menu_quit_game(mame_ui_manager mui, render_container container) : base(mui, container) { }
        //~menu_quit_game() { }


        protected override void populate(ref float customtop, ref float custombottom)
        {
        }


        protected override void handle()
        {
            /* request a reset */
            machine().schedule_exit();

            /* reset the menu stack */
            stack_reset();
        }
    }


    class menu_bios_selection : menu
    {
        /*-------------------------------------------------
            ui_menu_bios_selection - populates the main
            bios selection menu
        -------------------------------------------------*/
        public menu_bios_selection(mame_ui_manager mui, render_container container) : base(mui, container) { }
        //~menu_bios_selection() { }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            ui_menu_bios_selection - menu that
        -------------------------------------------------*/
        protected override void handle()
        {
            throw new emu_unimplemented();
        }
    }
}
