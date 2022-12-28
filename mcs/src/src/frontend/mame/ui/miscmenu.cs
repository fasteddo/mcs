// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.language_global;


namespace mame.ui
{
    class menu_network_devices : menu
    {
        public menu_network_devices(mame_ui_manager mui, render_container container)
            : base(mui, container)
        {
            set_heading(__("Network Devices"));
        }

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
        protected override void handle(event_ ev)
        {
            throw new emu_unimplemented();
        }
    }


    class menu_bookkeeping : menu_textbox
    {
        attotime prevtime;


        /*-------------------------------------------------
            menu_bookkeeping - handle the bookkeeping
            information menu
        -------------------------------------------------*/
        public menu_bookkeeping(mame_ui_manager mui, render_container container) : base(mui, container) { }
        //~menu_bookkeeping() { }


        protected override void menu_activated() { throw new emu_unimplemented(); }
        protected override void populate_text(text_layout layout, float width, int lines) { throw new emu_unimplemented(); }
        protected override void populate(ref float customtop, ref float custombottom) { throw new emu_unimplemented(); }
        protected override void handle(event_ ev) { throw new emu_unimplemented(); }
    }


    class menu_crosshair : menu
    {
        //class crosshair_item_data


        /*-------------------------------------------------
            menu_crosshair - handle the crosshair settings
            menu
        -------------------------------------------------*/
        public menu_crosshair(mame_ui_manager mui, render_container container)
            : base(mui, container)
        {
            set_process_flags(PROCESS_LR_REPEAT);
            set_heading(__("menu-crosshair", "Crosshair Options"));
        }

        //~menu_crosshair() { }


        protected override void populate(ref float customtop, ref float custombottom) { throw new emu_unimplemented(); }
        protected override void handle(event_ ev) { throw new emu_unimplemented(); }
    }


    class menu_bios_selection : menu
    {
        /*-------------------------------------------------
            ui_menu_bios_selection - populates the main
            bios selection menu
        -------------------------------------------------*/
        public menu_bios_selection(mame_ui_manager mui, render_container container)
            : base(mui, container)
        {
            set_heading(__("BIOS Selection"));
        }

        //~menu_bios_selection() { }


        protected override void populate(ref float customtop, ref float custombottom) { throw new emu_unimplemented(); }
        protected override void handle(event_ ev) { throw new emu_unimplemented(); }
    }


    //class menu_export : public menu

    //class menu_machine_configure : public menu

    //class menu_plugins_configure : public menu
}
