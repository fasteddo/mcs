// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using nvram_interface_enumerator = mame.device_interface_enumerator<mame.device_nvram_interface>;  //typedef device_interface_enumerator<device_nvram_interface> nvram_interface_enumerator;


namespace mame
{
    // ======================> device_nvram_interface
    // class representing interface-specific live nvram
    public abstract class device_nvram_interface : device_interface
    {
        bool m_backup_enabled;


        // construction/destruction
        //-------------------------------------------------
        //  device_nvram_interface - constructor
        //-------------------------------------------------
        public device_nvram_interface(machine_config mconfig, device_t device, bool backup_enabled = true)
            : base(device, "nvram")
        {
            m_backup_enabled = backup_enabled;
        }

        //~device_nvram_interface() { }


        // configuration
        //void nvram_enable_backup(bool enabled) { assert(!device().started()); m_backup_enabled = enabled; }


        // public accessors... for now
        public void nvram_reset() { nvram_default(); }
        public bool nvram_load(util.read_stream file) { return nvram_read(file); }
        public bool nvram_save(util.write_stream file) { return nvram_write(file); }
        public bool nvram_backup_enabled() { return m_backup_enabled; }
        public bool nvram_can_save() { return m_backup_enabled && nvram_can_write(); }


        // derived class overrides
        protected abstract void nvram_default();
        protected abstract bool nvram_read(util.read_stream file);
        protected abstract bool nvram_write(util.write_stream file);
        protected virtual bool nvram_can_write() { return true; }
    }


    // iterator
    //typedef device_interface_enumerator<device_nvram_interface> nvram_interface_enumerator;
}
