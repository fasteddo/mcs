// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using s32 = System.Int32;

using static mame.osd.inputman_global;


namespace mame.osd
{
    public static class inputman_global
    {
        //**************************************************************************
        //  CONSTANTS
        //**************************************************************************

        // relative devices return ~512 units per on-screen pixel
        public const s32 INPUT_RELATIVE_PER_PIXEL = 512;

        // absolute devices return values between -65536 and +65536
        public const s32 INPUT_ABSOLUTE_MIN = -65536;
        public const s32 INPUT_ABSOLUTE_MAX = 65536;

        // invalid memory value for axis polling
        public const s32 INVALID_AXIS_VALUE = 0x7fffffff;
    }


    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // base for application representation of host input device
    public interface input_device
    {
        // callback for getting the value of an individual input on a device
        public delegate s32 item_get_state_func(object device_internal, object item_internal);  //typedef s32 (*item_get_state_func)(void *device_internal, void *item_internal);

        //virtual ~input_device() = default;

        input_item_id add_item(
            string name,
            input_item_id itemid,
            item_get_state_func getstate,
            object internal_ = default);
    }


    // base for application input manager
    interface input_manager
    {
        //virtual ~input_manager() = default;

        input_device add_device(
            input_device_class devclass,
            string name,
            string id,
            object internal_ = default);
    }
}
