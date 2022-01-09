// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using image_interface_enumerator = mame.device_interface_enumerator<mame.device_image_interface>;  //typedef device_interface_enumerator<device_image_interface> image_interface_enumerator;
using slot_interface_enumerator = mame.device_interface_enumerator<mame.device_slot_interface>;  //typedef device_interface_enumerator<device_slot_interface> slot_interface_enumerator;
using unsigned = System.UInt32;

using static mame.ui.mainmenu_internal;


namespace mame.ui
{
    class menu_main : menu
    {
        /*-------------------------------------------------
            ui_menu_main constructor - populate the main menu
        -------------------------------------------------*/
        public menu_main(mame_ui_manager mui, render_container container) : base(mui, container)
        {
            set_needs_prev_menu_item(false);
        }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            /* add main menu items */
            item_append("Input (general)", 0, INPUT_GROUPS);

            item_append("Input (this machine)", 0, INPUT_SPECIFIC);

            if (ui().machine_info().has_analog())
                item_append("Analog Controls", 0, ANALOG);
            if (ui().machine_info().has_dips())
                item_append("DIP Switches", 0, SETTINGS_DIP_SWITCHES);
            if (ui().machine_info().has_configs())
                item_append("Machine Configuration", null, 0, SETTINGS_DRIVER_CONFIG);

            item_append("Bookkeeping Info", 0, BOOKKEEPING);

            item_append("Machine Information", 0, GAME_INFO);

            if (ui().found_machine_warnings())
                item_append("Warning Information", 0, WARN_INFO);

            foreach (device_image_interface image in new image_interface_enumerator(machine().root_device()))
            {
                if (image.user_loadable())
                {
                    item_append("Image Information", 0, IMAGE_MENU_IMAGE_INFO);

                    item_append("File Manager", 0, IMAGE_MENU_FILE_MANAGER);

                    break;
                }
            }

            throw new emu_unimplemented();
#if false
            if (cassette_device_iterator(machine().root_device()).first() != nullptr)
                item_append(_("Tape Control"), nullptr, 0, (void *)TAPE_CONTROL);
#endif

            throw new emu_unimplemented();
#if false
            pty_interface_iterator ptyiter = new pty_interface_iterator(machine().root_device());
            if (ptyiter.first() != null)
            {
                item_append("Pseudo terminals", null, 0, ui_menu_main.ui_menu_main_options.PTY_INFO);
            }
#endif

            if (ui().machine_info().has_bioses())
                item_append("BIOS Selection", 0, BIOS_SELECTION);

            if (new slot_interface_enumerator(machine().root_device()).first() != null)
                item_append("Slot Devices", 0, SLOT_DEVICES);

            throw new emu_unimplemented();
#if false
            if (barcode_reader_device_iterator(machine().root_device()).first() != nullptr)
                item_append("Barcode Reader", 0, ui_menu_main_options.BARCODE_READ);

            if (network_interface_iterator(machine().root_device()).first() != nullptr)
                item_append("Network Devices", 0, ui_menu_main_options.NETWORK_DEVICES);

            if (machine().natkeyboard().keyboard_count())
                item_append("Keyboard Mode", 0, ui_menu_main_options.KEYBOARD_MODE);
#endif

            item_append("Slider Controls", 0, SLIDERS);

            item_append("Video Options", 0, VIDEO_TARGETS);

            throw new emu_unimplemented();
#if false
            if (machine().crosshair().get_usage())
                item_append("Crosshair Options", null, 0, ui_menu_main_options.CROSSHAIR);

            if (machine().options().cheat() && machine().cheat().first() != null)
                item_append("Cheat", null, 0, ui_menu_main_options.CHEAT);

            if (machine().phase() >= machine_phase::RESET)
            {
                if (machine().options().plugins() && !mame_machine_manager::instance()->lua()->get_menu().empty())
                    item_append(_("Plugin Options"), 0, (void *)PLUGINS);

                if (mame_machine_manager::instance()->lua()->call_plugin_check<const char *>("data_list", "", true))
                    item_append(_("External DAT View"), 0, (void *)EXTERNAL_DATS);
            }

            item_append(ui_menu_item_type.SEPARATOR);

            if (!mame_machine_manager.instance().favorite().isgame_favorite())
                item_append("Add To Favorites", null, 0, ui_menu_main_options.ADD_FAVORITE);
            else
                item_append("Remove From Favorites", null, 0, ui_menu_main_options.REMOVE_FAVORITE);

            item_append(menu_item_type.SEPARATOR);

            item_append(string_format(_("About %s"), emulator_info::get_appname()), 0, (void *)ABOUT);

            item_append(menu_item_type::SEPARATOR);
#endif

            //  item_append(_("Quit from Machine"), nullptr, 0, (void *)QUIT_GAME);

            throw new emu_unimplemented();
#if false
            if (machine().phase() == machine_phase::INIT)
            {
                item_append(_("Start Machine"), 0, (void *)DISMISS);
            }
            else
            {
                item_append(_("Select New Machine"), 0, (void *)SELECT_GAME);
                item_append(_("Return to Machine"), 0, (void *)DISMISS);
            }
#endif
        }


        /*-------------------------------------------------
            menu_main - handle the main menu
        -------------------------------------------------*/
        protected override void handle()
        {
            /* process the menu */
            menu_event menu_event = process(0);
            if (menu_event != null && menu_event.iptkey == (int)ioport_type.IPT_UI_SELECT)
            {
                throw new emu_unimplemented();
#if false
#endif
            }
        }
    }


    static class mainmenu_internal
    {
        //enum : unsigned {
        public const unsigned INPUT_GROUPS         = 0;
        public const unsigned INPUT_SPECIFIC       = 1;
        public const unsigned SETTINGS_DIP_SWITCHES = 2;
        public const unsigned SETTINGS_DRIVER_CONFIG = 3;
        public const unsigned ANALOG               = 4;
        public const unsigned BOOKKEEPING          = 5;
        public const unsigned GAME_INFO            = 6;
        public const unsigned WARN_INFO            = 7;
        public const unsigned IMAGE_MENU_IMAGE_INFO = 8;
        public const unsigned IMAGE_MENU_FILE_MANAGER = 9;
        public const unsigned TAPE_CONTROL         = 10;
        public const unsigned SLOT_DEVICES         = 11;
        public const unsigned NETWORK_DEVICES      = 12;
        public const unsigned KEYBOARD_MODE        = 13;
        public const unsigned SLIDERS              = 14;
        public const unsigned VIDEO_TARGETS        = 15;
        public const unsigned VIDEO_OPTIONS        = 16;
        public const unsigned CROSSHAIR            = 17;
        public const unsigned CHEAT                = 18;
        public const unsigned PLUGINS              = 19;
        public const unsigned BIOS_SELECTION       = 20;
        public const unsigned BARCODE_READ         = 21;
        public const unsigned PTY_INFO             = 22;
        public const unsigned EXTERNAL_DATS        = 23;
        public const unsigned ADD_FAVORITE         = 24;
        public const unsigned REMOVE_FAVORITE      = 25;
        public const unsigned ABOUT                = 26;
        public const unsigned QUIT_GAME            = 27;
        public const unsigned DISMISS              = 28;
        public const unsigned SELECT_GAME          = 29;
        //};
    }
}
