// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.language_global;


namespace mame.ui
{
    class menu_file_manager : menu
    {
        //astring current_directory;
        //astring current_file;
        device_image_interface selected_device;

        string m_warnings;


        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        public menu_file_manager(mame_ui_manager mui, render_container container, string warnings)
            : base(mui, container)
        {
            selected_device = null;
            m_warnings = warnings != null ? warnings : "";

            // The warning string is used when accessing from the force_file_manager call, i.e.
            // when the file manager is loaded top front in the case of mandatory image devices
            set_heading(__("File Manager"));
        }

        //~menu_file_manager() { }


        // force file manager menu
        public static void force_file_manager(mame_ui_manager mui, render_container container, string warnings)
        {
            // drop any existing menus and start the file manager
            menu.stack_reset(mui);
            menu.stack_push_special_main(new menu_file_manager(mui, container, warnings));
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
        protected override void handle(event_ ev)
        {
            throw new emu_unimplemented();
        }


        //virtual void custom_render(void *selectedref, float top, float bottom, float x, float y, float x2, float y2);


        //void fill_image_line(device_image_interface *img, astring &instance, astring &filename);
    }
}
