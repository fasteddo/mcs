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
        // construction/destruction
        //-------------------------------------------------
        //  device_nvram_interface - constructor
        //-------------------------------------------------
        public device_nvram_interface(machine_config mconfig, device_t device)
            : base(device, "nvram")
        {
        }

        //~device_nvram_interface() { }


        // public accessors... for now
        public void nvram_reset() { nvram_default(); }
        public bool nvram_load(util.read_stream file) { return nvram_read(file); }
        public bool nvram_save(util.write_stream file) { return nvram_write(file); }
        public bool nvram_can_save() { return nvram_can_write(); }


        // derived class overrides
        protected abstract void nvram_default();
        protected abstract bool nvram_read(util.read_stream file);
        protected abstract bool nvram_write(util.write_stream file);
        protected virtual bool nvram_can_write() { return true; }
    }


    // iterator
    //typedef device_interface_enumerator<device_nvram_interface> nvram_interface_enumerator;
}
