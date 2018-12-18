// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;


namespace mame
{
    // ======================> generic_latch_base_device
    public class generic_latch_base_device : device_t
    {
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
        //auto data_pending_callback() { return m_data_pending_cb.bind(); }
        //void set_separate_acknowledge(bool ack) { m_separate_acknowledge = ack; }


        //DECLARE_READ_LINE_MEMBER(pending_r);

        //DECLARE_READ8_MEMBER( acknowledge_r );
        //DECLARE_WRITE8_MEMBER( acknowledge_w );


        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            m_data_pending_cb.resolve_safe();
            save_item(m_latch_written, "m_latch_written");

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
                m_data_pending_cb.op(latch_written ? 1 : 0);
            }
        }


        //-------------------------------------------------
        //  init_callback - set initial state
        //-------------------------------------------------
        void init_callback(object ptr, int param)
        {
            m_data_pending_cb.op(m_latch_written ? 1 : 0);
        }
    }


    // ======================> generic_latch_8_device
    public class generic_latch_8_device : generic_latch_base_device
    {
        //DEFINE_DEVICE_TYPE(GENERIC_LATCH_8, generic_latch_8_device, "generic_latch_8", "Generic 8-bit latch")
        static device_t device_creator_generic_latch_8_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new generic_latch_8_device(mconfig, tag, owner, clock); }
        public static readonly device_type GENERIC_LATCH_8 = DEFINE_DEVICE_TYPE(device_creator_generic_latch_8_device, "generic_latch_8", "Generic 8-bit latch");



        byte m_latched_value;


        // construction/destruction
        //-------------------------------------------------
        //  generic_latch_8_device - constructor
        //-------------------------------------------------
        generic_latch_8_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, GENERIC_LATCH_8, tag, owner, clock)
        {
            m_latched_value = 0;
        }


        //READ8_MEMBER( generic_latch_8_device::read )
        public u8 read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            if (!has_separate_acknowledge() && !machine().side_effects_disabled())
                set_latch_written(false);
            return m_latched_value;
        }


        //WRITE8_MEMBER( generic_latch_8_device::write )
        public void write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            machine().scheduler().synchronize(sync_callback, data);
        }

        //DECLARE_WRITE8_MEMBER( preset_w );
        //DECLARE_WRITE8_MEMBER( clear_w );
        //DECLARE_WRITE_LINE_MEMBER( preset );
        //DECLARE_WRITE_LINE_MEMBER( clear );

        //void preset_w(u8 value) { m_latched_value = value; }


        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // register for state saving
            base.device_start();
            save_item(m_latched_value, "m_latched_value");
        }


        //-------------------------------------------------
        //  soundlatch_sync_callback - time-delayed
        //  callback to set a latch value
        //-------------------------------------------------
        void sync_callback(object ptr, int param)
        {
            byte value = (byte)param;

            // if the latch has been written and the value is changed, log a warning
            if (is_latch_written() && m_latched_value != value)
                logerror("Warning: latch written before being read. Previous: {0}, new: {1}\n", m_latched_value, value);  // %02x, new: %02x

            // store the new value and mark it not read
            m_latched_value = value;
            set_latch_written(true);
        }
    }


    // ======================> generic_latch_16_device
    class generic_latch_16_device : generic_latch_base_device
    {
        //DEFINE_DEVICE_TYPE(GENERIC_LATCH_16, generic_latch_16_device, "generic_latch_16", "Generic 16-bit latch")
        static device_t device_creator_generic_latch_16_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new generic_latch_16_device(mconfig, tag, owner, clock); }
        public static readonly device_type GENERIC_LATCH_16 = DEFINE_DEVICE_TYPE(device_creator_generic_latch_16_device, "generic_latch_16", "Generic 16-bit latch");


        UInt16 m_latched_value;


        // construction/destruction
        //-------------------------------------------------
        //  generic_latch_16_device - constructor
        //-------------------------------------------------
        generic_latch_16_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, GENERIC_LATCH_16, tag, owner, clock)
        {
            m_latched_value = 0;
        }


        //DECLARE_READ16_MEMBER( read );
        //DECLARE_WRITE16_MEMBER( write );

        //DECLARE_WRITE16_MEMBER( preset_w );
        //DECLARE_WRITE16_MEMBER( clear_w );
        //DECLARE_WRITE_LINE_MEMBER( preset );
        //DECLARE_WRITE_LINE_MEMBER( clear );

        //void preset_w(u16 value) { m_latched_value = value; }


        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // register for state saving
            base.device_start();
            save_item(m_latched_value, "m_latched_value");
        }


        //void sync_callback(void *ptr, s32 param);
    }
}
