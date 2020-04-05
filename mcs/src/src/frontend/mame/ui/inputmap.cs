// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.ui
{
    class menu_input_groups : menu
    {
        /*-------------------------------------------------
            menu_input_groups_populate - populate the
            input groups menu
        -------------------------------------------------*/
        public menu_input_groups(mame_ui_manager mui, render_container container) : base(mui, container) { }
        //~menu_input_groups() { }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            int player;

            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            menu_input_groups - handle the input groups
            menu
        -------------------------------------------------*/
        protected override void handle()
        {
            throw new emu_unimplemented();
        }
    }


    abstract class menu_input : menu
    {
#if false
        enum
        {
            INPUT_TYPE_DIGITAL = 0,
            INPUT_TYPE_ANALOG = 1,
            INPUT_TYPE_ANALOG_DEC = INPUT_TYPE_ANALOG + SEQ_TYPE_DECREMENT,
            INPUT_TYPE_ANALOG_INC = INPUT_TYPE_ANALOG + SEQ_TYPE_INCREMENT,
            INPUT_TYPE_TOTAL = INPUT_TYPE_ANALOG + SEQ_TYPE_TOTAL
        }
#endif


        /* internal input menu item data */
        protected class input_item_data
        {
            //input_item_data *   next;               /* pointer to next item in the list */
            //const void *        ref;                /* reference to type description for global inputs or field for game inputs */
            //input_seq_type      seqtype;            /* sequence type */
            //input_seq           seq;                /* copy of the live sequence */
            //const input_seq *   defseq;             /* pointer to the default sequence */
            //const char *        name;               /* pointer to the base name of the item */
            //const char *        owner_name;         /* pointer to the name of the owner of the item */
            //ioport_group group;              /* group type */
            //uint8_t type;               /* type of port */
            //bool                is_optional;        /* true if this input is considered optional */
        }


        //const void *        pollingref;
        //input_seq_type      pollingseq;
        //input_item_data *   pollingitem;


        //input_item_data *   lastitem;
        //bool                record_next;
        //input_seq           starting_seq;


        /*-------------------------------------------------
            menu_input - display a menu for inputs
        -------------------------------------------------*/
        public menu_input(mame_ui_manager mui, render_container container)
            : base(mui, container)
        {
            throw new emu_unimplemented();
        }

        //~menu_input() { }


        protected override void handle()
        {
            throw new emu_unimplemented();
        }


        //void populate_sorted(std::vector<input_item_data *> &&itemarray);
        //void toggle_none_default(input_seq &selected_seq, input_seq &original_seq, const input_seq &selected_defseq);

        //virtual void handle() override;
        protected abstract void update_input(input_item_data seqchangeditem);
    }


    class menu_input_general : menu_input
    {
        int group;


        /*-------------------------------------------------
            menu_input_general - handle the general
            input menu
        -------------------------------------------------*/
        menu_input_general(mame_ui_manager mui, render_container container, int _group)
            : base(mui, container)
        {
            group = _group;
        }

        //~menu_input_general() { }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            throw new emu_unimplemented();
        }


        protected override void update_input(input_item_data seqchangeditem)
        {
            throw new emu_unimplemented();
        }
    }


    class menu_input_specific : menu_input
    {
        /*-------------------------------------------------
            menu_input_specific - handle the game-specific
            input menu
        -------------------------------------------------*/
        public menu_input_specific(mame_ui_manager mui, render_container container) : base(mui, container) { }
        //~menu_input_specific() { }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            throw new emu_unimplemented();
        }


        protected override void update_input(input_item_data seqchangeditem)
        {
            throw new emu_unimplemented();
        }
    }


    class menu_settings : menu
    {
        /* DIP switch descriptor */
        class dip_descriptor
        {
            //dip_descriptor *    next;
            //const char *        name;
            //UINT32              mask;
            //UINT32              state;
        }


        dip_descriptor diplist;
        int dipcount;
        int type;


        /*-------------------------------------------------
            menu_settings_populate - populate one of the
            switches menus
        -------------------------------------------------*/

        public menu_settings(mame_ui_manager mui, render_container container, UInt32 _type)
            : base(mui, container)
        {
            diplist = null;
            dipcount = 0;

            type = (int)_type;
        }

        //~menu_settings() { }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            menu_settings_common - handle one of the
            switches menus
        -------------------------------------------------*/
        protected override void handle()
        {
            throw new emu_unimplemented();
        }
    }


    class menu_settings_dip_switches : menu_settings
    {
        /*-------------------------------------------------
            menu_settings_dip_switches - handle the DIP
            switches menu
        -------------------------------------------------*/
        public menu_settings_dip_switches(mame_ui_manager mui, render_container container) : base(mui, container, (UInt32)ioport_type.IPT_DIPSWITCH) { }
        //~menu_settings_dip_switches() { }


        //virtual void custom_render(void *selectedref, float top, float bottom, float x, float y, float x2, float y2);

        //void custom_render_one(float x1, float y1, float x2, float y2, const dip_descriptor *dip, UINT32 selectedmask);
    }


    class menu_settings_driver_config : menu_settings
    {
        /*-------------------------------------------------
            menu_settings_driver_config - handle the
            driver config menu
        -------------------------------------------------*/
        public menu_settings_driver_config(mame_ui_manager mui, render_container container) : base(mui, container, (UInt32)ioport_type.IPT_CONFIG) { }
        //~menu_settings_driver_config() { }
    }


    class menu_analog : menu
    {
#if false
        enum
        {
            ANALOG_ITEM_KEYSPEED = 0,
            ANALOG_ITEM_CENTERSPEED,
            ANALOG_ITEM_REVERSE,
            ANALOG_ITEM_SENSITIVITY,
            ANALOG_ITEM_COUNT
        }
#endif

        class analog_item_data
        {
            //ioport_field field;
            //int                 type;
            //int                 min, max;
            //int                 cur;
            //int                 defvalue;
        }


        /*-------------------------------------------------
            menu_analog - handle the analog settings menu
        -------------------------------------------------*/

        /*-------------------------------------------------
            menu_analog_populate - populate the analog
            settings menu
        -------------------------------------------------*/

        public menu_analog(mame_ui_manager mui, render_container container) : base(mui, container) { }
        //~menu_analog() { }


        protected override void populate(ref float customtop, ref float custombottom)
        {
            throw new emu_unimplemented();
        }


        protected override void handle()
        {
            throw new emu_unimplemented();
        }
    }
}
