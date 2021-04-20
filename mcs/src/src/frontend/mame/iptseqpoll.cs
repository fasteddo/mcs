// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using s32 = System.Int32;


namespace mame
{
    abstract class input_code_poller
    {
        protected input_manager m_manager;
        protected std.vector<std.pair<input_device_item, s32>> m_axis_memory;


        protected input_code_poller(input_manager manager)
        {
            m_manager = manager;
            m_axis_memory = new std.vector<std.pair<input_device_item, s32>>();
        }

        //virtual ~input_code_poller();


        public virtual void reset()
        {
            // iterate over device classes and devices
            m_axis_memory.clear();
            for (input_device_class classno = input_device_class.DEVICE_CLASS_FIRST_VALID; input_device_class.DEVICE_CLASS_LAST_VALID >= classno; ++classno)
            {
                input_class devclass = m_manager.device_class(classno);
                if (devclass.enabled())
                {
                    for (int devnum = 0; devclass.maxindex() >= devnum; ++devnum)
                    {
                        // fetch the device; ignore if nullptr
                        input_device device = devclass.device(devnum);
                        if (device != null)
                        {
                            // iterate over items within each device
                            for (input_item_id itemid = input_item_id.ITEM_ID_FIRST_VALID; device.maxitem() >= itemid; ++itemid)
                            {
                                // for any non-switch items, set memory to the current value
                                input_device_item item = device.item(itemid);
                                if (item != null && (item.itemclass() != input_item_class.ITEM_CLASS_SWITCH))
                                    m_axis_memory.emplace_back(new std.pair<input_device_item, s32>(item, m_manager.code_value(item.code())));
                            }
                        }
                    }
                }
            }

            m_axis_memory.Sort();  //std::sort(m_axis_memory.begin(), m_axis_memory.end());
        }


        public abstract input_code poll();
    }


    abstract class switch_code_poller_base : input_code_poller
    {
        std.vector<input_code> m_switch_memory;


        protected switch_code_poller_base(input_manager manager)
            : base(manager)
        {
            m_switch_memory = new std.vector<input_code>();
        }


        public override void reset()
        {
            m_switch_memory.clear();
            base.reset();
        }


        protected bool code_pressed_once(input_code code, bool moved)
        {
            throw new emu_unimplemented();
        }
    }


    //class axis_code_poller : public input_code_poller


    class switch_code_poller : switch_code_poller_base
    {
        public switch_code_poller(input_manager manager)
            : base(manager)
        {
        }


        public override input_code poll()
        {
            // iterate over device classes and devices, skipping disabled classes
            for (input_device_class classno = input_device_class.DEVICE_CLASS_FIRST_VALID; input_device_class.DEVICE_CLASS_LAST_VALID >= classno; ++classno)
            {
                input_class devclass = m_manager.device_class(classno);
                if (!devclass.enabled())
                    continue;

                for (int devnum = 0; devclass.maxindex() >= devnum; ++devnum)
                {
                    // fetch the device; ignore if nullptr
                    input_device device = devclass.device(devnum);
                    if (device == null)
                        continue;

                    // iterate over items within each device
                    for (input_item_id itemid = input_item_id.ITEM_ID_FIRST_VALID; device.maxitem() >= itemid; ++itemid)
                    {
                        input_device_item item = device.item(itemid);
                        if (item == null)
                            continue;

                        input_code code = item.code();
                        if (item.itemclass() == input_item_class.ITEM_CLASS_SWITCH)
                        {
                            // item is natively a switch, poll it
                            if (code_pressed_once(code, true))
                                return code;
                            else
                                continue;
                        }

                        throw new emu_unimplemented();
#if false
                        var memory = std.lower_bound(
                                    m_axis_memory,
                                    item,
                                    (std.pair<input_device_item, s32> x, input_device_item y) => { return x.first < y; });
                        if ((m_axis_memory.end() == memory) || (item != memory.first))  //if ((m_axis_memory.end() == memory) || (item != memory->first))
                            continue;

                        // poll axes digitally
                        bool moved = item.check_axis(code.item_modifier(), memory.second);
                        code.set_item_class(input_item_class.ITEM_CLASS_SWITCH);
                        if ((classno == input_device_class.DEVICE_CLASS_JOYSTICK) && (code.item_id() == input_item_id.ITEM_ID_XAXIS))
                        {
                            // joystick X axis - check with left/right modifiers
                            code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_LEFT);
                            if (code_pressed_once(code, moved))
                                return code;
                            code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_RIGHT);
                            if (code_pressed_once(code, moved))
                                return code;
                        }
                        else if ((classno == input_device_class.DEVICE_CLASS_JOYSTICK) && (code.item_id() == input_item_id.ITEM_ID_YAXIS))
                        {
                            // if this is a joystick Y axis, check with up/down modifiers
                            code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_UP);
                            if (code_pressed_once(code, moved))
                                return code;
                            code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_DOWN);
                            if (code_pressed_once(code, moved))
                                return code;
                        }
                        else
                        {
                            // any other axis, check with pos/neg modifiers
                            code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_POS);
                            if (code_pressed_once(code, moved))
                                return code;
                            code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_NEG);
                            if (code_pressed_once(code, moved))
                                return code;
                        }
#endif
                    }
                }
            }

            // if nothing, return an invalid code
            return input_code.INPUT_CODE_INVALID;
        }
    }



    //class keyboard_code_poller : public switch_code_poller_base


    //class input_sequence_poller


    //class axis_sequence_poller : public input_sequence_poller


    //class switch_sequence_poller : public input_sequence_poller
}
