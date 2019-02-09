// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.ui
{
    class menu_file_manager : menu
    {
        //astring current_directory;
        //astring current_file;
        device_image_interface selected_device;

        string m_warnings;
        bool m_curr_selected;


        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        public menu_file_manager(mame_ui_manager mui, render_container container, string warnings)
            : base(mui, container)
        {
            selected_device = null;

            // This warning string is used when accessing from the force_file_manager call, i.e.
            // when the file manager is loaded top front in the case of mandatory image devices
            if (!string.IsNullOrEmpty(warnings))
                m_warnings = warnings;
            else
                m_warnings = "";

            m_curr_selected = false;
        }

        //~menu_file_manager() { }


        // force file manager menu
        public static void force_file_manager(mame_ui_manager mui, render_container container, string warnings)
        {
            // reset the menu stack
            stack_reset(mui.machine());

            // add the quit entry followed by the game select entry
            stack_push_special_main(new menu_quit_game(mui, container));
            stack_push(new menu_file_manager(mui, container, warnings));

            // force the menus on
            mui.show_menu();

            // make sure MAME is paused
            mui.machine().pause();
        }


        //-------------------------------------------------
        //  populate
        //-------------------------------------------------
        protected override void populate(ref float customtop, ref float custombottom)
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  handle
        //-------------------------------------------------
        protected override void handle()
        {
            throw new emu_unimplemented();
        }


        //virtual void custom_render(void *selectedref, float top, float bottom, float x, float y, float x2, float y2);


        //void fill_image_line(device_image_interface *img, astring &instance, astring &filename);
    }
}
