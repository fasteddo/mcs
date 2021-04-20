// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using image_interface_enumerator = mame.device_interface_enumerator<mame.device_image_interface>;  //typedef device_interface_enumerator<device_image_interface> image_interface_enumerator;
using slot_interface_enumerator = mame.device_interface_enumerator<mame.device_slot_interface>;  //typedef device_interface_enumerator<device_slot_interface> slot_interface_enumerator;


namespace mame.ui
{
    class menu_main : menu
    {
        /*-------------------------------------------------
            ui_menu_main constructor - populate the main menu
        -------------------------------------------------*/
        public menu_main(mame_ui_manager mui, render_container container) : base(mui, container) { }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            /* add main menu items */
            item_append("Input (general)", 0, menu_options.INPUT_GROUPS);

            item_append("Input (this machine)", 0, menu_options.INPUT_SPECIFIC);

            if (ui().machine_info().has_analog())
                item_append("Analog Controls", 0, menu_options.ANALOG);
            if (ui().machine_info().has_dips())
                item_append("DIP Switches", 0, menu_options.SETTINGS_DIP_SWITCHES);
            if (ui().machine_info().has_configs())
                item_append("Machine Configuration", null, 0, menu_options.SETTINGS_DRIVER_CONFIG);

            item_append("Bookkeeping Info", 0, menu_options.BOOKKEEPING);

            item_append("Machine Information", 0, menu_options.GAME_INFO);

            if (ui().found_machine_warnings())
                item_append("Warning Information", 0, menu_options.WARN_INFO);

            foreach (device_image_interface image in new image_interface_enumerator(machine().root_device()))
            {
                if (image.user_loadable())
                {
                    item_append("Image Information", 0, menu_options.IMAGE_MENU_IMAGE_INFO);

                    item_append("File Manager", 0, menu_options.IMAGE_MENU_FILE_MANAGER);

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
                item_append("BIOS Selection", 0, menu_options.BIOS_SELECTION);

            if (new slot_interface_enumerator(machine().root_device()).first() != null)
                item_append("Slot Devices", 0, menu_options.SLOT_DEVICES);

            throw new emu_unimplemented();
#if false
            if (barcode_reader_device_iterator(machine().root_device()).first() != nullptr)
                item_append("Barcode Reader", 0, ui_menu_main_options.BARCODE_READ);

            if (network_interface_iterator(machine().root_device()).first() != nullptr)
                item_append("Network Devices", 0, ui_menu_main_options.NETWORK_DEVICES);

            if (machine().ioport().natkeyboard().keyboard_count())
                item_append("Keyboard Mode", 0, ui_menu_main_options.KEYBOARD_MODE);
#endif

            item_append("Slider Controls", 0, menu_options.SLIDERS);

            item_append("Video Options", 0, menu_options.VIDEO_TARGETS);

            throw new emu_unimplemented();
#if false
            if (machine().crosshair().get_usage())
                item_append("Crosshair Options", null, 0, ui_menu_main_options.CROSSHAIR);
#endif

            throw new emu_unimplemented();
#if false
            if (machine().options().cheat() && machine().cheat().first() != null)
                item_append("Cheat", null, 0, ui_menu_main_options.CHEAT);
#endif

            if (machine().options().plugins())
                item_append("Plugin Options", 0, menu_options.PLUGINS);

            throw new emu_unimplemented();
#if false
            if (mame_machine_manager::instance()->lua()->call_plugin_check<const char *>("data_list", true))
                item_append("External DAT View", null, 0, ui_menu_main_options.EXTERNAL_DATS);

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

            item_append("Select New Machine", 0, menu_options.SELECT_GAME);
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
}
