// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using s32 = System.Int32;
using u8 = System.Byte;


namespace mame
{
    // ======================> joystick_map
    // a 9x9 joystick map
    class joystick_map
    {
        // joystick mapping codes
        const byte JOYSTICK_MAP_NEUTRAL = 0x00;
        const byte JOYSTICK_MAP_LEFT    = 0x01;
        const byte JOYSTICK_MAP_RIGHT   = 0x02;
        const byte JOYSTICK_MAP_UP      = 0x04;
        const byte JOYSTICK_MAP_DOWN    = 0x08;
        const byte JOYSTICK_MAP_STICKY  = 0x0f;


        // internal state
        u8 [,] m_map = new byte[9, 9];            // 9x9 grid
        u8 m_lastmap;              // last value returned (for sticky tracking)
        string m_origstring;           // originally parsed string


        // construction/destruction
        //-------------------------------------------------
        //  joystick_map - constructor
        //-------------------------------------------------
        public joystick_map()
        {
            m_lastmap = JOYSTICK_MAP_NEUTRAL;


            // parse the standard 8-way map as default
            parse(input_class_joystick.map_8way);
        }

        joystick_map(joystick_map src) { copy(src); }


        // operators
        //joystick_map &operator=(const joystick_map &src) { if (this != &src) copy(src); return *this; }


        // parse from a string
        //-------------------------------------------------
        //  parse - parse a string into a joystick map
        //-------------------------------------------------
        public bool parse(string mapstring)
        {
            // save a copy of the original string
            m_origstring = mapstring;

            int mapstringIdx = 0;

            // iterate over rows
            for (int rownum = 0; rownum < 9; rownum++)
            {
                // if we're done, copy from another row
                if (mapstringIdx == mapstring.Length || mapstring[mapstringIdx] == '.')  //(*mapstring == 0 || *mapstring == '.')
                {
                    bool symmetric = (rownum >= 5 && mapstringIdx == mapstring.Length);  // *mapstring == 0);
                    var srcrow = m_map;
                    int srcrowIdx = symmetric ? (8 - rownum) : (rownum - 1);  //const u8 *srcrow = &m_map[symmetric ? (8 - rownum) : (rownum - 1)][0];

                    // if this is row 0, we don't have a source row -- invalid
                    if (rownum == 0)
                        return false;

                    // copy from the srcrow, applying up/down symmetry if in the bottom half
                    for (int colnum = 0; colnum < 9; colnum++)
                    {
                        byte val = srcrow[srcrowIdx, colnum];
                        if (symmetric)
                            val = (byte)((val & (JOYSTICK_MAP_LEFT | JOYSTICK_MAP_RIGHT)) | ((val & JOYSTICK_MAP_UP) << 1) | ((val & JOYSTICK_MAP_DOWN) >> 1));
                        m_map[rownum, colnum] = val;
                    }
                }

                // otherwise, parse this column
                else
                {
                    for (int colnum = 0; colnum < 9; colnum++)
                    {
                        // if we're at the end of row, copy previous to the middle, then apply left/right symmetry
                        if (colnum > 0 && (mapstringIdx == mapstring.Length || mapstring[mapstringIdx] == '.'))  //(*mapstring == 0 || *mapstring == '.'))
                        {
                            bool symmetric = (colnum >= 5);
                            byte val = m_map[rownum, symmetric ? (8 - colnum) : (colnum - 1)];
                            if (symmetric)
                                val = (byte)((val & (JOYSTICK_MAP_UP | JOYSTICK_MAP_DOWN)) | ((val & JOYSTICK_MAP_LEFT) << 1) | ((val & JOYSTICK_MAP_RIGHT) >> 1));
                            m_map[rownum, colnum] = val;
                        }

                        // otherwise, convert the character to its value
                        else
                        {
                            byte [] charmap = new byte[]
                            {
                                JOYSTICK_MAP_UP | JOYSTICK_MAP_LEFT,
                                JOYSTICK_MAP_UP,
                                JOYSTICK_MAP_UP | JOYSTICK_MAP_RIGHT,
                                JOYSTICK_MAP_LEFT,
                                JOYSTICK_MAP_NEUTRAL,
                                JOYSTICK_MAP_RIGHT,
                                JOYSTICK_MAP_DOWN | JOYSTICK_MAP_LEFT,
                                JOYSTICK_MAP_DOWN,
                                JOYSTICK_MAP_DOWN | JOYSTICK_MAP_RIGHT,
                                JOYSTICK_MAP_STICKY
                            };

                            string validchars = "789456123s";
                            int ptrIdx = validchars.IndexOf(mapstring[mapstringIdx++]);  // const char *ptr = strchr(validchars, *mapstring++);

                            // invalid characters exit immediately
                            if (ptrIdx == -1)
                                return false;

                            m_map[rownum, colnum] = charmap[ptrIdx];  // - validchars];
                        }
                    }
                }

                // if we ended with a period, advance to the next row
                if (mapstring[mapstringIdx] == '.')  //(*mapstring == '.')
                    mapstringIdx++;
            }

            return true;
        }


        // create a friendly string
        //-------------------------------------------------
        //  to_string - output the map as a string for
        //  friendly display
        //-------------------------------------------------
        public string to_string()
        {
            string str = m_origstring;
            str += "\n";
            for (int i = 0; i < m_map.GetLength(0); i++)
            {
                str += "  ";
                for (int j = 0; j < m_map.GetLength(1); j++)
                {
                    switch (m_map[i, j])
                    {
                        case JOYSTICK_MAP_UP | JOYSTICK_MAP_LEFT:   str += "7";  break;
                        case JOYSTICK_MAP_UP:                       str += "8";  break;
                        case JOYSTICK_MAP_UP | JOYSTICK_MAP_RIGHT:  str += "9";  break;
                        case JOYSTICK_MAP_LEFT:                     str += "4";  break;
                        case JOYSTICK_MAP_NEUTRAL:                  str += "5";  break;
                        case JOYSTICK_MAP_RIGHT:                    str += "6";  break;
                        case JOYSTICK_MAP_DOWN | JOYSTICK_MAP_LEFT: str += "1";  break;
                        case JOYSTICK_MAP_DOWN:                     str += "2";  break;
                        case JOYSTICK_MAP_DOWN | JOYSTICK_MAP_RIGHT:str += "3";  break;
                        case JOYSTICK_MAP_STICKY:                   str += "s";  break;
                        default:                                    str += "?";  break;
                    }
                }

                str += "\n";
            }

            return str;
        }


        // update the state of a live map
        //u8 update(s32 xaxisval, s32 yaxisval);


        // internal helpers
        void copy(joystick_map src)
        {
            //memcpy(m_map, src.m_map, sizeof(m_map));
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    m_map[i, j] = src.m_map[i, j];

            m_lastmap = JOYSTICK_MAP_NEUTRAL;
            m_origstring = src.m_origstring;
        }
    }


    // ======================> input_device_item
    // a single item on an input device
    public abstract class input_device_item
    {
        // internal state
        input_device m_device;               // reference to our owning device
        string m_name;                 // string name of item
        object m_internal;  //void *                  m_internal;             // internal callback pointer
        input_item_id m_itemid;               // originally specified item id
        input_item_class m_itemclass;            // class of the item
        item_get_state_func m_getstate;             // get state callback
        string m_token;                // tokenized name for non-standard items

        // live state
        protected s32 m_current;              // current raw value
        s32 m_memory;               // "memory" value, to remember where we started during polling


        // construction/destruction
        //-------------------------------------------------
        //  input_device_item - constructor
        //-------------------------------------------------
        protected input_device_item(input_device device, string name, object internalobj, input_item_id itemid, item_get_state_func getstate, input_item_class itemclass)
        {
            m_device = device;
            m_name = name;
            m_internal = internalobj;
            m_itemid = itemid;
            m_itemclass = itemclass;
            m_getstate = getstate;
            m_current = 0;
            m_memory = 0;


            // use a standard token name for know item IDs
            string standard_token = manager().standard_token(itemid);
            if (standard_token != null)
            {
                m_token = standard_token;
            }
            // otherwise, create a tokenized name
            else
            {
                m_token = name;
                m_token = m_token.ToUpper();
                m_token = m_token.Replace(" ", "");  //strdelchr(m_token, ' ');
                m_token = m_token.Replace("_", "");  //strdelchr(m_token, '_');
            }
        }


        // getters
        public input_device device() { return m_device; }
        input_manager manager() { return m_device.manager(); }
        //running_machine &machine() const;
        public string name() { return m_name.c_str(); }
        object internalobj() { return m_internal; }
        protected input_item_id itemid() { return m_itemid; }
        public input_item_class itemclass() { return m_itemclass; }
        public input_code code() { return new input_code(m_device.devclass(), m_device.devindex(), m_itemclass, input_item_modifier.ITEM_MODIFIER_NONE, m_itemid); }

        //const char *token() const { return m_token.c_str(); }
        //s32 current() const { return m_current; }
        public s32 memory() { return m_memory; }


        // helpers

        public s32 update_value() { return m_current = m_getstate(m_device.internalobj(), m_internal); }
        public void set_memory(s32 value) { m_memory = value; }


        // readers
        public abstract s32 read_as_switch(input_item_modifier modifier);
        public abstract s32 read_as_relative(input_item_modifier modifier);
        public abstract s32 read_as_absolute(input_item_modifier modifier);
    }


    // ======================> input_device
    // a logical device of a given class that can provide input
    public abstract class input_device : global_object
    {
        //friend class input_class;


        // internal state
        input_manager m_manager;              // reference to input manager
        string m_name;                 // string name of device
        string m_id;                   // id of device
        int m_devindex;             // device index of this device
        input_device_item [] m_item = new input_device_item[(int)input_item_id.ITEM_ID_ABSOLUTE_MAXIMUM + 1]; // array of pointers to items
        input_item_id m_maxitem;              // maximum item index
        object m_internal;  //void *                  m_internal;             // internal callback pointer

        bool m_steadykey_enabled;    // steadykey enabled for keyboards
        bool m_lightgun_reload_button; // lightgun reload hack


        // construction/destruction
        //-------------------------------------------------
        //  input_device - constructor
        //-------------------------------------------------
        protected input_device(input_manager manager, string name, string id, object internalobj)
        {
            m_manager = manager;
            m_name = name;
            m_id = id;
            m_devindex = -1;
            m_maxitem = 0;
            m_internal = internalobj;
            m_steadykey_enabled = manager.machine().options().steadykey();
            m_lightgun_reload_button = manager.machine().options().offscreen_reload();
        }


        // getters
        public input_manager manager() { return m_manager; }
        running_machine machine() { return m_manager.machine(); }
        public input_device_class devclass() { return device_class(); }
        public string name() { return m_name.c_str(); }
        public string id() { return m_id.c_str(); }
        public int devindex() { return m_devindex; }
        public input_device_item item(input_item_id index) { return m_item[(int)index]; }
        public input_item_id maxitem() { return m_maxitem; }
        public object internalobj() { return m_internal; }
        public bool steadykey_enabled() { return m_steadykey_enabled; }
        public bool lightgun_reload_button() { return m_lightgun_reload_button; }


        // setters
        public void set_devindex(int devindex) { m_devindex = devindex; }


        // item management
        //-------------------------------------------------
        //  add_item - add a new item to an input device
        //-------------------------------------------------
        public input_item_id add_item(string name, input_item_id itemid, item_get_state_func getstate, object internal_obj = null)
        {
            assert_always(machine().phase() == machine_phase.INIT, "Can only call input_device::add_item at init time!");
            assert(name != null);
            assert(itemid > input_item_id.ITEM_ID_INVALID && itemid < input_item_id.ITEM_ID_MAXIMUM);
            assert(getstate != null);

            // if we have a generic ID, pick a new internal one
            input_item_id originalid = itemid;
            if (itemid >= input_item_id.ITEM_ID_OTHER_SWITCH && itemid <= input_item_id.ITEM_ID_OTHER_AXIS_RELATIVE)
            {
                for (itemid = (input_item_id)(input_item_id.ITEM_ID_MAXIMUM + 1); itemid <= input_item_id.ITEM_ID_ABSOLUTE_MAXIMUM; ++itemid)
                {
                    if (m_item[(int)itemid] == null)
                        break;
                }
            }

            assert(itemid <= input_item_id.ITEM_ID_ABSOLUTE_MAXIMUM);

            // make sure we don't have any overlap
            assert(m_item[(int)itemid] == null);

            // determine the class and create the appropriate item class
            switch (m_manager.device_class(devclass()).standard_item_class(originalid))
            {
                case input_item_class.ITEM_CLASS_SWITCH:
                    m_item[(int)itemid] = new input_device_switch_item(this, name, internal_obj, itemid, getstate);
                    break;

                case input_item_class.ITEM_CLASS_RELATIVE:
                    m_item[(int)itemid] = new input_device_relative_item(this, name, internal_obj, itemid, getstate);
                    break;

                case input_item_class.ITEM_CLASS_ABSOLUTE:
                    m_item[(int)itemid] = new input_device_absolute_item(this, name, internal_obj, itemid, getstate);
                    break;

                default:
                    m_item[(int)itemid] = null;
                    assert(false);
                    break;
            }

            // assign the new slot and update the maximum
            m_maxitem = (input_item_id)Math.Max((int)m_maxitem, (int)itemid);
            return itemid;
        }


        // helpers
        //s32 adjust_absolute(s32 value) const { return adjust_absolute_value(value); }
        //bool match_device_id(const char *deviceid);


        // specific overrides
        protected abstract input_device_class device_class();
        protected virtual int adjust_absolute_value(int value) { return value; }
    }


    // ======================> input_device_keyboard
    class input_device_keyboard : input_device
    {
        // construction/destruction
        //-------------------------------------------------
        //  input_device_keyboard - constructor
        //-------------------------------------------------
        public input_device_keyboard(input_manager manager, string _name, string _id, object _internal)
            : base(manager, _name, _id, _internal)
        {
        }


        // helpers
        //-------------------------------------------------
        //  apply_steadykey - apply steadykey option
        //-------------------------------------------------
        public void apply_steadykey()
        {
            // ignore if not enabled
            if (!steadykey_enabled())
                return;

            // update the state of all the keys and see if any changed state
            bool anything_changed = false;
            for (input_item_id itemid = input_item_id.ITEM_ID_FIRST_VALID; itemid <= maxitem(); ++itemid)
            {
                input_device_item itm = item(itemid);
                if (itm != null && itm.itemclass() == input_item_class.ITEM_CLASS_SWITCH)
                {
                    if (((input_device_switch_item)itm).steadykey_changed())
                        anything_changed = true;
                }
            }

            // if the keyboard state is stable, flush the current state
            if (!anything_changed)
            {
                for (input_item_id itemid = input_item_id.ITEM_ID_FIRST_VALID; itemid <= maxitem(); ++itemid)
                {
                    input_device_item itm = item(itemid);
                    if (itm != null && itm.itemclass() == input_item_class.ITEM_CLASS_SWITCH)
                        ((input_device_switch_item)itm).steadykey_update_to_current();
                }
            }
        }


        // specific overrides
        protected override input_device_class device_class() { return input_device_class.DEVICE_CLASS_KEYBOARD; }
    }


    // ======================> input_device_mouse
    class input_device_mouse : input_device
    {
        // construction/destruction
        //-------------------------------------------------
        //  input_device_mouse - constructor
        //-------------------------------------------------
        public input_device_mouse(input_manager manager, string _name, string _id, object _internal)
            : base(manager, _name, _id, _internal)
        {
        }


        // specific overrides
        protected override input_device_class device_class() { return input_device_class.DEVICE_CLASS_MOUSE; }
    }


    // ======================> input_device_lightgun
    class input_device_lightgun : input_device
    {
        // construction/destruction
        //-------------------------------------------------
        //  input_device_lightgun - constructor
        //-------------------------------------------------
        public input_device_lightgun(input_manager manager, string _name, string _id, object _internal)
            : base(manager, _name, _id, _internal)
        {
        }


        // specific overrides
        protected override input_device_class device_class() { return input_device_class.DEVICE_CLASS_LIGHTGUN; }
    }


    // ======================> input_device_joystick
    // a logical device of a given class that can provide input
    class input_device_joystick : input_device
    {
        // joystick information
        joystick_map m_joymap;               // joystick map for this device
        //s32                     m_joystick_deadzone;    // deadzone for joystick
        //s32                     m_joystick_saturation;  // saturation position for joystick


        // construction/destruction
        //-------------------------------------------------
        //  input_device_joystick - constructor
        //-------------------------------------------------
        public input_device_joystick(input_manager manager, string _name, string _id, object _internal)
            : base(manager, _name, _id, _internal)
        {
            throw new emu_unimplemented();
        }


        // getters
        //joystick_map &joymap() { return m_joymap; }


        // item management
        public void set_joystick_map(joystick_map map) { m_joymap = map; }


        // specific overrides
        protected override input_device_class device_class() { return input_device_class.DEVICE_CLASS_JOYSTICK; }
        protected override int adjust_absolute_value(int value) { throw new emu_unimplemented(); }
    }


    // ======================> input_class
    // a class of device that provides input
    public abstract class input_class : global_object
    {
        // internal state
        input_manager m_manager;              // reference to our manager
        input_device [] m_device = new input_device[input_global.DEVICE_INDEX_MAXIMUM]; // array of devices in this class
        input_device_class m_devclass;             // our device class
        string m_name;                 // name of class (used for option settings)
        int m_maxindex;             // maximum populated index
        bool m_enabled;              // is this class enabled?
        bool m_multi;                // are multiple instances of this class allowed?


        // construction/destruction
        //-------------------------------------------------
        //  input_class - constructor
        //-------------------------------------------------
        protected input_class(input_manager manager, input_device_class devclass, string name, bool enabled = false, bool multi = false)
        {
            m_manager = manager;
            m_devclass = devclass;
            m_name = name;
            m_maxindex = 0;
            m_enabled = enabled;
            m_multi = multi;


            assert(m_name != null);
        }


        // getters
        protected input_manager manager() { return m_manager; }
        protected running_machine machine() { return m_manager.machine(); }
        public input_device device(int index) { return (index <= m_maxindex) ? m_device[index] : null; }
        //input_device_class devclass() const { return m_devclass; }
        public string name() { return m_name; }
        public int maxindex() { return m_maxindex; }
        public bool enabled() { return m_enabled; }
        public bool multi() { return m_multi; }


        // setters
        public void enable(bool state = true) { m_enabled = state; }
        //void set_multi(bool multi = true) { m_multi = multi; }


        // device management
        //-------------------------------------------------
        //  add_device - add a new input device
        //-------------------------------------------------
        public input_device add_device(string name, string id, object internal_object = null)
        {
            assert_always(machine().phase() == machine_phase.INIT, "Can only call input_class::add_device at init time!");
            assert(name != null);
            assert(id != null);

            // allocate a new device and add it to the index
            return add_device(make_device(name, id, internal_object));
        }


        public input_device add_device(input_device new_device)
        {
            assert(new_device.devclass() == m_devclass);

            // find the next empty index
            for (int devindex = 0; devindex < input_global.DEVICE_INDEX_MAXIMUM; devindex++)
            {
                if (m_device[devindex] == null)
                {
                    // update the device and maximum index found
                    new_device.set_devindex(devindex);
                    m_maxindex = Math.Max(m_maxindex, devindex);

                    if (new_device.id()[0] == 0)
                        osd_printf_verbose("Input: Adding {0} #{1}: {2}\n", m_name, devindex, new_device.name());
                    else
                        osd_printf_verbose("Input: Adding {0} #{1}: {2} (device id: {3})\n", m_name, devindex, new_device.name(), new_device.id());

                    m_device[devindex] = new_device;
                    return m_device[devindex];
                }
            }

            throw new emu_fatalerror("Input: Too many {0} devices\n", m_name);
        }


        // misc helpers

        //-------------------------------------------------
        //  standard_item_class - return the class of a
        //  standard item
        //-------------------------------------------------
        public input_item_class standard_item_class(input_item_id itemid)
        {
            // most everything standard is a switch, apart from the axes
            if (itemid == input_item_id.ITEM_ID_OTHER_SWITCH || itemid < input_item_id.ITEM_ID_XAXIS || (itemid > input_item_id.ITEM_ID_SLIDER2 && itemid < input_item_id.ITEM_ID_ADD_ABSOLUTE1))
                return input_item_class.ITEM_CLASS_SWITCH;

            // standard mouse axes are relative
            else if (m_devclass == input_device_class.DEVICE_CLASS_MOUSE || itemid == input_item_id.ITEM_ID_OTHER_AXIS_RELATIVE || (itemid >= input_item_id.ITEM_ID_ADD_RELATIVE1 && itemid <= input_item_id.ITEM_ID_ADD_RELATIVE16))
                return input_item_class.ITEM_CLASS_RELATIVE;

            // all other standard axes are absolute
            else
                return input_item_class.ITEM_CLASS_ABSOLUTE;
        }


        //void remap_device_index(int oldindex, int newindex);


        // specific overrides
        protected abstract input_device make_device(string name, string id, object internal_obj);


        // indexing helpers
        //int newindex(input_device &device);
    }


    // ======================> input_class_keyboard
    // class of device that provides keyboard input
    class input_class_keyboard : input_class
    {
        // construction/destruction
        //-------------------------------------------------
        //  input_class_keyboard - constructor
        //-------------------------------------------------
        public input_class_keyboard(input_manager manager)
            : base(manager, input_device_class.DEVICE_CLASS_KEYBOARD, "keyboard", true, manager.machine().options().multi_keyboard())
        {
            // request a per-frame callback for the keyboard class
            machine().add_notifier(machine_notification.MACHINE_NOTIFY_FRAME, frame_callback);
        }


        // specific overrides
        protected override input_device make_device(string name, string id, object internalobj)
        {
            return new input_device_keyboard(manager(), name, id, internalobj);
        }


        // internal helpers
        //-------------------------------------------------
        //  frame_callback - per-frame callback for various
        //  bookkeeping
        //-------------------------------------------------
        void frame_callback(running_machine machine)
        {
            // iterate over all devices in our class
            for (int devnum = 0; devnum <= maxindex(); devnum++)
            {
                if (device(devnum) != null)
                    ((input_device_keyboard)device(devnum)).apply_steadykey();
            }
        }
    }


    // ======================> input_class_mouse
    // class of device that provides mouse input
    class input_class_mouse : input_class
    {
        // construction/destruction
        //-------------------------------------------------
        //  input_class_mouse - constructor
        //-------------------------------------------------
        public input_class_mouse(input_manager manager)
            : base(manager, input_device_class.DEVICE_CLASS_MOUSE, "mouse", manager.machine().options().mouse(), manager.machine().options().multi_mouse())
        {
        }


        // specific overrides
        protected override input_device make_device(string name, string id, object internalobj)
        {
            return new input_device_mouse(manager(), name, id, internalobj);
        }
    }


    // ======================> input_class_lightgun
    // class of device that provides lightgun input
    class input_class_lightgun : input_class
    {
        // construction/destruction
        //-------------------------------------------------
        //  input_class_lightgun - constructor
        //-------------------------------------------------
        public input_class_lightgun(input_manager manager)
            : base(manager, input_device_class.DEVICE_CLASS_LIGHTGUN, "lightgun", manager.machine().options().lightgun(), true)
        {
        }


        // specific overrides
        protected override input_device make_device(string name, string id, object internalobj)
        {
            return new input_device_lightgun(manager(), name, id, internalobj);
        }
    }


    // ======================> input_class_joystick
    // class of device that provides joystick input
    class input_class_joystick : input_class
    {
        // standard joystick maps
        public const string map_8way = "7778...4445";
        public const string map_4way_diagonal = "4444s8888..444458888.444555888.ss5.222555666.222256666.2222s6666.2222s6666";


        // construction/destruction
        //-------------------------------------------------
        //  input_class_joystick - constructor
        //-------------------------------------------------
        public input_class_joystick(input_manager manager)
            : base(manager, input_device_class.DEVICE_CLASS_JOYSTICK, "joystick", manager.machine().options().joystick(), true)
        {
        }


        // misc
        //-------------------------------------------------
        //  set_global_joystick_map - set the map for all
        //  joysticks
        //-------------------------------------------------
        public bool set_global_joystick_map(string mapstring)
        {
            // parse the map
            joystick_map map = new joystick_map();
            if (!map.parse(mapstring))
                return false;

            osd_printf_verbose("Input: Changing default joystick map = {0}\n", map.to_string().c_str());

            // iterate over joysticks and set the map
            for (int joynum = 0; joynum <= maxindex(); joynum++)
            {
                if (device(joynum) != null)
                    ((input_device_joystick)device(joynum)).set_joystick_map(map);
            }

            return true;
        }


        // specific overrides
        protected override input_device make_device(string name, string id, object internalobj)
        {
            return new input_device_joystick(manager(), name, id, internalobj);
        }
    }


    // ======================> input_device_switch_item
    // derived input item representing a switch input
    class input_device_switch_item : input_device_item
    {
        // internal state
        int m_steadykey;            // the live steadykey state
        int m_oldkey;               // old live state


        // construction/destruction
        //-------------------------------------------------
        //  input_device_switch_item - constructor
        //-------------------------------------------------
        public input_device_switch_item(input_device device, string name, object internalobj, input_item_id itemid, item_get_state_func getstate)
            : base(device, name, internalobj, itemid, getstate, input_item_class.ITEM_CLASS_SWITCH)
        {
            m_steadykey = 0;
            m_oldkey = 0;
        }


        // readers
        //-------------------------------------------------
        //  read_as_switch - return the raw switch value,
        //  modified as necessary
        //-------------------------------------------------
        public override int read_as_switch(input_item_modifier modifier)
        {
            // if we're doing a lightgun reload hack, button 1 and 2 operate differently
            input_device_class devclass = device().devclass();
            if (devclass == input_device_class.DEVICE_CLASS_LIGHTGUN && device().lightgun_reload_button())
            {
                // button 1 is pressed if either button 1 or 2 are active
                if (itemid() == input_item_id.ITEM_ID_BUTTON1)
                {
                    input_device_item button2_item = device().item(input_item_id.ITEM_ID_BUTTON2);
                    if (button2_item != null)
                        return button2_item.update_value() | update_value();
                }

                // button 2 is never officially pressed
                if (itemid() == input_item_id.ITEM_ID_BUTTON2)
                    return 0;
            }

            // steadykey for keyboards
            if (devclass == input_device_class.DEVICE_CLASS_KEYBOARD && device().steadykey_enabled())
                return m_steadykey;

            // everything else is just the current value as-is
            return update_value();
        }


        //-------------------------------------------------
        //  read_as_relative - return the switch input as
        //  a relative axis value
        //-------------------------------------------------
        public override int read_as_relative(input_item_modifier modifier)
        {
            // no translation to relative
            return 0;
        }


        //-------------------------------------------------
        //  read_as_absolute - return the switch input as
        //  an absolute axis value
        //-------------------------------------------------
        public override int read_as_absolute(input_item_modifier modifier)
        {
            // no translation to absolute
            return 0;
        }


        // steadykey helper

        //-------------------------------------------------
        //  steadykey_changed - update for steadykey
        //  behavior, returning true if the current state
        //  has changed since the last call
        //-------------------------------------------------
        public bool steadykey_changed()
        {
            int old = m_oldkey;
            m_oldkey = update_value();
            if (((m_current ^ old) & 1) == 0)
                return false;

            // if the keypress was missed, turn it on for one frame
            if (((m_current | m_steadykey) & 1) == 0)
                m_steadykey = 1;

            return true;
        }

        public void steadykey_update_to_current() { m_steadykey = m_current; }
    }


    // ======================> input_device_switch_item
    // derived input item representing a relative axis input
    class input_device_relative_item : input_device_item
    {
        // construction/destruction
        //-------------------------------------------------
        //  input_device_relative_item - constructor
        //-------------------------------------------------
        public input_device_relative_item(input_device device, string name, object internalobj, input_item_id itemid, item_get_state_func getstate)
            : base(device, name, internalobj, itemid, getstate, input_item_class.ITEM_CLASS_RELATIVE)
        {
        }


        // readers
        public override int read_as_switch(input_item_modifier modifier) { throw new emu_unimplemented(); }
        public override int read_as_relative(input_item_modifier modifier) { throw new emu_unimplemented(); }
        public override int read_as_absolute(input_item_modifier modifier) { throw new emu_unimplemented(); }
    }


    // ======================> input_device_switch_item
    // derived input item representing an absolute axis input
    class input_device_absolute_item : input_device_item
    {
        // construction/destruction
        //-------------------------------------------------
        //  input_device_absolute_item - constructor
        //-------------------------------------------------
        public input_device_absolute_item(input_device device, string name, object internalobj, input_item_id itemid, item_get_state_func getstate)
            : base(device, name, internalobj, itemid, getstate, input_item_class.ITEM_CLASS_ABSOLUTE)
        {
        }


        // readers
        public override s32 read_as_switch(input_item_modifier modifier) { throw new emu_unimplemented(); }
        public override s32 read_as_relative(input_item_modifier modifier) { throw new emu_unimplemented(); }
        public override s32 read_as_absolute(input_item_modifier modifier) { throw new emu_unimplemented(); }
    }
}
