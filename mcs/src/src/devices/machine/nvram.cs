// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using optional_memory_region = mame.memory_region_finder<mame.bool_const_false>;  //using optional_memory_region = memory_region_finder<false>;
using PointerU8 = mame.Pointer<System.Byte>;
using size_t = System.UInt64;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.nvram_global;


namespace mame
{
    public class nvram_device : device_t
                                //device_nvram_interface
    {
        //DEFINE_DEVICE_TYPE(NVRAM, nvram_device, "nvram", "NVRAM")
        public static readonly emu.detail.device_type_impl NVRAM = DEFINE_DEVICE_TYPE("nvram", "NVRAM", (type, mconfig, tag, owner, clock) => { return new nvram_device(mconfig, tag, owner, clock); });


        protected class device_nvram_interface_nvram : device_nvram_interface
        {
            public device_nvram_interface_nvram(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void nvram_default() { ((nvram_device)device()).device_nvram_interface_nvram_default(); }
            protected override void nvram_read(emu_file file) { ((nvram_device)device()).device_nvram_interface_nvram_read(file); }
            protected override void nvram_write(emu_file file) { ((nvram_device)device()).device_nvram_interface_nvram_write(file); }
            protected override bool nvram_can_write() { return ((nvram_device)device()).device_nvram_interface_nvram_can_write(); }
        }


        // custom initialization for default state
        delegate void init_delegate(nvram_device device, PointerU8 obj, size_t param);  //typedef device_delegate<void (nvram_device &, void *, size_t)> init_delegate;


        // values
        public enum default_value
        {
            DEFAULT_ALL_0,
            DEFAULT_ALL_1,
            DEFAULT_RANDOM,
            DEFAULT_CUSTOM,
            DEFAULT_NONE
        }


        device_nvram_interface_nvram m_dinvram;


        // configuration state
        optional_memory_region m_region;
        default_value m_default_value;
        init_delegate m_custom_handler;

        // runtime state
        PointerU8 m_base;  //void *                      m_base;
        size_t m_length;


        // construction/destruction
        nvram_device(machine_config mconfig, string tag, device_t owner, default_value value)
            : this(mconfig, tag, owner, 0U)
        {
            set_default_value(value);
        }


        nvram_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, NVRAM, tag, owner, clock)
        {
            //device_nvram_interface(mconfig, *this),
            m_class_interfaces.Add(new device_nvram_interface_nvram(mconfig, this));
            m_dinvram = GetClassInterface<device_nvram_interface_nvram>();


            m_region = new optional_memory_region(this, DEVICE_SELF);
            m_default_value = default_value.DEFAULT_ALL_1;
            m_custom_handler = null;
            m_base = null;
            m_length = 0;
        }


        // inline configuration helpers
        public void set_default_value(default_value value) { m_default_value = value; }

        //template <typename... T> void set_custom_handler(T &&... args)
        //{
        //    m_default_value = DEFAULT_CUSTOM;
        //    m_custom_handler.set(std::forward<T>(args)...);
        //}

        // controls
        //void set_base(void *base, size_t length) { m_base = base; m_length = length; }


        // device-level overrides
        protected override void device_start()
        {
            // bind our handler
            //m_custom_handler.resolve();
        }


        // device_nvram_interface overrides
        protected virtual void device_nvram_interface_nvram_default()
        {
            // make sure we have a valid base pointer
            determine_final_base();

            // region always wins
            if (m_region.found())
            {
                std.memcpy(m_base, new PointerU8(m_region.op0.base_()), m_length);
                return;
            }

            // default values for other cases
            switch (m_default_value)
            {
                // all-0's
                case default_value.DEFAULT_ALL_0:
                    std.memset(m_base, (uint8_t)0, m_length);
                    break;

                // all 1's
                default:
                case default_value.DEFAULT_ALL_1:
                    std.memset(m_base, (uint8_t)0xff, m_length);
                    break;

                // random values
                case default_value.DEFAULT_RANDOM:
                {
                    PointerU8 nvram = new PointerU8(m_base);  //uint8_t *nvram = reinterpret_cast<uint8_t *>(m_base);
                    for (int index = 0; index < (int)m_length; index++)
                        nvram[index] = (uint8_t)machine().rand();
                    break;
                }

                // custom handler
                case default_value.DEFAULT_CUSTOM:
                    m_custom_handler(this, m_base, m_length);
                    break;

                // none - do nothing
                case default_value.DEFAULT_NONE:
                    break;
            }
        }


        protected virtual void device_nvram_interface_nvram_read(emu_file file) { throw new emu_unimplemented(); }
        protected virtual void device_nvram_interface_nvram_write(emu_file file) { throw new emu_unimplemented(); }
        protected virtual bool device_nvram_interface_nvram_can_write() { throw new emu_unimplemented(); }  //{ return m_base && m_length; }


        // internal helpers
        void determine_final_base()
        {
            // find our shared pointer with the target RAM
            if (m_base == null)
            {
                memory_share share = owner().memshare(tag());
                if (share == null)
                    throw new emu_fatalerror("NVRAM device '{0}' has no corresponding share() region", tag());
                m_base = share.ptr();
                m_length = share.bytes();
            }

            // if we are region-backed for the default, find it now and make sure it's the right size
            if (m_region.found() && m_region.op0.bytes() != m_length)
                throw new emu_fatalerror("{0}", util.string_format("NVRAM device '{0}' has a default region, but it should be 0x{1} bytes", tag(), m_length));
        }
    }


    static class nvram_global
    {
        public static nvram_device NVRAM(machine_config mconfig, string tag, nvram_device.default_value value)
        {
            var device = emu.detail.device_type_impl.op<nvram_device>(mconfig, tag, nvram_device.NVRAM, 0);
            device.set_default_value(value);
            return device;
        }
    }
}
