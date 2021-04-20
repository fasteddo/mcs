// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


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
        public void nvram_load(emu_file file) { nvram_read(file); }
        public void nvram_save(emu_file file) { nvram_write(file); }
        public bool nvram_can_save() { return nvram_can_write(); }


        // derived class overrides
        protected abstract void nvram_default();
        protected abstract void nvram_read(emu_file file);
        protected abstract void nvram_write(emu_file file);
        protected virtual bool nvram_can_write() { return true; }
    }


    // iterator
    //typedef device_interface_iterator<device_nvram_interface> nvram_interface_iterator;
    public class nvram_interface_iterator : device_interface_iterator<device_nvram_interface>
    {
        public nvram_interface_iterator(device_t root, int maxdepth = 255) : base(root, maxdepth) { }
    }
}
