// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;


namespace mame.ui
{
    class menu_confirm_quit : autopause_menu  //class menu_confirm_quit : public autopause_menu<>
    {
        public menu_confirm_quit(mame_ui_manager mui, render_container container)
            : base(mui, container)
        {
            set_one_shot(true);
            set_process_flags(PROCESS_CUSTOM_ONLY | PROCESS_NOINPUT);
        }

        //virtual ~menu_confirm_quit();

        protected override void custom_render(object selectedref, float top, float bottom, float x, float y, float x2, float y2)
        {
            throw new emu_unimplemented();
        }

        protected override void populate(ref float customtop, ref float custombottom)
        {
            throw new emu_unimplemented();
        }

        protected override void handle(event_ ev)
        {
            throw new emu_unimplemented();
        }
    }
}
