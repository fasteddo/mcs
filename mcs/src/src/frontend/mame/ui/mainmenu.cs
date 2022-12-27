// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using image_interface_enumerator = mame.device_interface_enumerator<mame.device_image_interface>;  //typedef device_interface_enumerator<device_image_interface> image_interface_enumerator;
using slot_interface_enumerator = mame.device_interface_enumerator<mame.device_slot_interface>;  //typedef device_interface_enumerator<device_slot_interface> slot_interface_enumerator;
using unsigned = System.UInt32;

using static mame.language_global;
using static mame.ui.mainmenu_internal;


namespace mame.ui
{
    class menu_main : menu
    {
        machine_phase m_phase;


        /*-------------------------------------------------
            menu_main constructor/destructor
        -------------------------------------------------*/
        public menu_main(mame_ui_manager mui, render_container container) : base(mui, container)
        {
            m_phase = machine_phase.PREINIT;


            set_needs_prev_menu_item(false);
        }


        /*-------------------------------------------------
            menu_activated - handle coming to foreground
        -------------------------------------------------*/
        protected override void menu_activated()
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            populate - populate main menu items
        -------------------------------------------------*/
        protected override void populate(ref float customtop, ref float custombottom)
        {
            m_phase = machine().phase();

            item_append(__("Input (general)"), 0, INPUT_GROUPS);

            item_append(__("Input (this machine)"), 0, INPUT_SPECIFIC);

            if (ui().machine_info().has_analog())
                item_append(__("Analog Controls"), 0, ANALOG);
            if (ui().machine_info().has_dips())
                item_append(__("DIP Switches"), 0, SETTINGS_DIP_SWITCHES);
            if (ui().machine_info().has_configs())
                item_append(__("Machine Configuration"), 0, SETTINGS_DRIVER_CONFIG);

            item_append(__("Bookkeeping Info"), 0, BOOKKEEPING);

            item_append(__("Machine Information"), 0, GAME_INFO);

            if (ui().found_machine_warnings())
                item_append(__("Warning Information"), 0, WARN_INFO);

            foreach (device_image_interface image in new image_interface_enumerator(machine().root_device()))
            {
                if (image.user_loadable())
                {
                    item_append(__("Image Information"), 0, IMAGE_MENU_IMAGE_INFO);

                    item_append(__("File Manager"), 0, IMAGE_MENU_FILE_MANAGER);

                    break;
                }
            }

            //throw new emu_unimplemented();
#if false
            if (cassette_device_enumerator(machine().root_device()).first() != nullptr)
                item_append(_("Tape Control"), 0, (void *)TAPE_CONTROL);

            if (pty_interface_enumerator(machine().root_device()).first() != nullptr)
                item_append(_("Pseudo Terminals"), 0, (void *)PTY_INFO);
#endif

            if (ui().machine_info().has_bioses())
                item_append(__("BIOS Selection"), 0, BIOS_SELECTION);

            //throw new emu_unimplemented();
#if false
            if (slot_interface_enumerator(machine().root_device()).first() != nullptr)
                item_append(_("Slot Devices"), 0, (void *)SLOT_DEVICES);

            if (barcode_reader_device_enumerator(machine().root_device()).first() != nullptr)
                item_append(_("Barcode Reader"), 0, (void *)BARCODE_READ);

            if (network_interface_enumerator(machine().root_device()).first() != nullptr)
                item_append(_("Network Devices"), 0, (void*)NETWORK_DEVICES);

            if (machine().natkeyboard().keyboard_count())
                item_append(_("Keyboard Mode"), 0, (void *)KEYBOARD_MODE);
#endif

            item_append(__("Slider Controls"), 0, SLIDERS);

            item_append(__("Video Options"), 0, VIDEO_TARGETS);

            //throw new emu_unimplemented();
#if false
            if (machine().crosshair().get_usage())
                item_append(_("Crosshair Options"), 0, (void *)CROSSHAIR);

            if (machine().options().cheat())
                item_append(_("Cheat"), 0, (void *)CHEAT);

            if (machine_phase::RESET <= m_phase)
            {
                if (machine().options().plugins() && !mame_machine_manager::instance()->lua()->get_menu().empty())
                    item_append(_("Plugin Options"), 0, (void *)PLUGINS);

                if (mame_machine_manager::instance()->lua()->call_plugin_check<const char *>("data_list", "", true))
                    item_append(_("External DAT View"), 0, (void *)EXTERNAL_DATS);
            }

            item_append(menu_item_type::SEPARATOR);

            if (!mame_machine_manager::instance()->favorite().is_favorite(machine()))
                item_append(_("Add To Favorites"), 0, (void *)ADD_FAVORITE);
            else
                item_append(_("Remove From Favorites"), 0, (void *)REMOVE_FAVORITE);
#endif

            item_append(menu_item_type.SEPARATOR);

            item_append(util.string_format(__("About {0}"), emulator_info.get_appname()), 0, ABOUT);

            item_append(menu_item_type.SEPARATOR);

//          item_append(_("Quit from Machine"), 0, (void *)QUIT_GAME);

            if (machine_phase.INIT == m_phase)
            {
                item_append(__("Start Machine"), 0, DISMISS);
            }
            else
            {
                item_append(__("Select New Machine"), 0, SELECT_GAME);
                item_append(__("Return to Machine"), 0, DISMISS);
            }
        }


        /*-------------------------------------------------
            menu_main - handle the main menu
        -------------------------------------------------*/
        protected override void handle(event_ ev)
        {
            if (ev != null && (ev.iptkey == (int)ioport_type.IPT_UI_SELECT))
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
