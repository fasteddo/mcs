// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;

using static mame.device_global;
using static mame.emucore_global;
using static mame.gen_latch_global;


namespace mame
{
    // ======================> generic_latch_base_device
    public class generic_latch_base_device : device_t
    {
        protected const int LOG_WARN = 1 << 0;
        const int VERBOSE = LOG_WARN;  //#define VERBOSE (LOG_WARN)
        //#include "logmacro.h"
        protected void LOGMASKED(int mask, string format, params object [] args) { logmacro_global.LOGMASKED(VERBOSE, mask, this, format, args); }
        protected void LOG(string format, params object [] args) { logmacro_global.LOG(VERBOSE, this, format, args); }


        bool m_separate_acknowledge;
        bool m_latch_written;
        devcb_write_line m_data_pending_cb;


        // construction/destruction
        //-------------------------------------------------
        //  generic_latch_base_device - constructor
        //-------------------------------------------------
        protected generic_latch_base_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_separate_acknowledge = false;
            m_latch_written = false;
            m_data_pending_cb = new devcb_write_line(this);
        }


        // configuration
        public devcb_write_line.binder data_pending_callback() { return m_data_pending_cb.bind(); }  //auto data_pending_callback() { return m_data_pending_cb.bind(); }


        //void set_separate_acknowledge(bool ack) { m_separate_acknowledge = ack; }


        //-------------------------------------------------
        //  pending_r - tell whether the latch is waiting
        //  to be read
        //-------------------------------------------------
        //READ_LINE_MEMBER(generic_latch_base_device::pending_r)
        public int pending_r()
        {
            return m_latch_written ? 1 : 0;
        }


        //u8 acknowledge_r(address_space &space);
        //void acknowledge_w(u8 data = 0);


        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            m_data_pending_cb.resolve_safe();
            save_item(NAME(new { m_latch_written }));

            // synchronization is needed since other devices may not be initialized yet
            machine().scheduler().synchronize(init_callback);
        }


        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            m_latch_written = false;
        }


        protected bool has_separate_acknowledge() { return m_separate_acknowledge; }
        protected bool is_latch_written() { return m_latch_written; }

        //-------------------------------------------------
        //  set_latch_written - helper to signal that latch
        //  has been written or has been read
        //-------------------------------------------------
        protected void set_latch_written(bool latch_written)
        {
            if (m_latch_written != latch_written)
            {
                m_latch_written = latch_written;
                m_data_pending_cb.op_s32(latch_written ? 1 : 0);
            }
        }


        //-------------------------------------------------
        //  init_callback - set initial state
        //-------------------------------------------------
        void init_callback(object ptr, int param)
        {
            m_data_pending_cb.op_s32(m_latch_written ? 1 : 0);
        }
    }


    // ======================> generic_latch_8_device
    public class generic_latch_8_device : generic_latch_base_device
    {
        //DEFINE_DEVICE_TYPE(GENERIC_LATCH_8, generic_latch_8_device, "generic_latch_8", "Generic 8-bit latch")
        public static readonly emu.detail.device_type_impl GENERIC_LATCH_8 = DEFINE_DEVICE_TYPE("generic_latch_8", "Generic 8-bit latch", (type, mconfig, tag, owner, clock) => { return new generic_latch_8_device(mconfig, tag, owner, clock); });


        u8 m_latched_value;


        // construction/destruction
        //-------------------------------------------------
        //  generic_latch_8_device - constructor
        //-------------------------------------------------
        generic_latch_8_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, GENERIC_LATCH_8, tag, owner, clock)
        {
            m_latched_value = 0;
        }


        public u8 read()
        {
            if (!has_separate_acknowledge() && !machine().side_effects_disabled())
                set_latch_written(false);
            return m_latched_value;
        }


        public void write(u8 data)
        {
            machine().scheduler().synchronize(sync_callback, data);
        }


        //void preset_w(u8 data = 0xff);
        //void clear_w(u8 data = 0);
        //DECLARE_WRITE_LINE_MEMBER( preset );
        //DECLARE_WRITE_LINE_MEMBER( clear );


        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // register for state saving
            base.device_start();
            save_item(NAME(new { m_latched_value }));
        }


        //-------------------------------------------------
        //  soundlatch_sync_callback - time-delayed
        //  callback to set a latch value
        //-------------------------------------------------
        void sync_callback(object ptr, int param)
        {
            u8 value = (u8)param;

            // if the latch has been written and the value is changed, log a warning
            if (is_latch_written() && m_latched_value != value)
                LOGMASKED(LOG_WARN, "Warning: latch written before being read. Previous: {0}, new: {1}\n", m_latched_value, value);  // %02x, new: %02x

            // store the new value and mark it not read
            m_latched_value = value;
            set_latch_written(true);
        }
    }


    // ======================> generic_latch_16_device
    class generic_latch_16_device : generic_latch_base_device
    {
        //DEFINE_DEVICE_TYPE(GENERIC_LATCH_16, generic_latch_16_device, "generic_latch_16", "Generic 16-bit latch")
        public static readonly emu.detail.device_type_impl GENERIC_LATCH_16 = DEFINE_DEVICE_TYPE("generic_latch_16", "Generic 16-bit latch", (type, mconfig, tag, owner, clock) => { return new generic_latch_16_device(mconfig, tag, owner, clock); });


        u16 m_latched_value;


        // construction/destruction
        //-------------------------------------------------
        //  generic_latch_16_device - constructor
        //-------------------------------------------------
        generic_latch_16_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, GENERIC_LATCH_16, tag, owner, clock)
        {
            m_latched_value = 0;
        }


        //u16 read();
        //void write(u16 data);

        //void preset_w(u16 data = 0xffff);
        //void clear_w(u16 data = 0);
        //DECLARE_WRITE_LINE_MEMBER( preset );
        //DECLARE_WRITE_LINE_MEMBER( clear );


        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // register for state saving
            base.device_start();
            save_item(NAME(new { m_latched_value }));
        }


        //void sync_callback(void *ptr, s32 param);
    }


    static class gen_latch_global
    {
        public static generic_latch_8_device GENERIC_LATCH_8(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<generic_latch_8_device>(mconfig, tag, generic_latch_8_device.GENERIC_LATCH_8, 0); }
        public static generic_latch_8_device GENERIC_LATCH_8<bool_Required>(machine_config mconfig, device_finder<generic_latch_8_device, bool_Required> finder) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, generic_latch_8_device.GENERIC_LATCH_8, 0); }
    }
}
