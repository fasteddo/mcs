// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> device_video_interface
    class device_video_interface : device_interface
    {
        const string s_unconfigured_screen_tag = "!!UNCONFIGURED!!";


        // configuration state
        bool m_screen_required;          // is a screen required?
        device_t m_screen_base;              // base device for resolving target screen
        string m_screen_tag;               // configured tag for the target screen

        // internal state
        screen_device m_screen;                   // pointer to the screen device


        // construction/destruction
        //-------------------------------------------------
        //  device_video_interface - constructor
        //-------------------------------------------------
        public device_video_interface(machine_config mconfig, device_t device, bool screen_required = true)
            : base(device, "video")
        {
            m_screen_required = screen_required;
            m_screen_base = device;
            m_screen_tag = s_unconfigured_screen_tag;
            m_screen = null;
        }

        //virtual ~device_video_interface();


        // configuration

        public void set_screen(string tag)
        {
            m_screen_base = device().mconfig().current_device();
            m_screen_tag = tag;
        }


        //void set_screen(device_t &base, const char *tag)
        //{
        //    m_screen_base = &base;
        //    m_screen_tag = tag;
        //}


        //template <class ObjectClass, bool Required>
        public void set_screen(device_finder<screen_device> finder)  //void set_screen(device_finder<ObjectClass, Required> &finder)
        {
            m_screen_base = finder.finder_target().first;
            m_screen_tag = finder.finder_target().second;
        }


        // getters
        public screen_device screen() { return m_screen; }
        //bool has_screen() const { return m_screen != nullptr; }


        // optional operation overrides
        public override void interface_config_complete()
        {
            // find the screen if explicitly configured
            if (m_screen_tag != null && strcmp(m_screen_tag, s_unconfigured_screen_tag) != 0)
                m_screen = m_screen_base.subdevice<screen_device>(m_screen_tag);

            // device_config_complete may now do whatever it needs to with the screen
        }

        protected override void interface_validity_check(validity_checker valid)
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  interface_pre_start - make sure all our input
        //  devices are started
        //-------------------------------------------------
        public override void interface_pre_start()
        {
            // only look up screens if we haven't explicitly requested no screen
            if (!string.IsNullOrEmpty(m_screen_tag))
            {
                if (strcmp(m_screen_tag, s_unconfigured_screen_tag) != 0)
                {
                    // find the screen device if explicitly configured
                    m_screen = m_screen_base.subdevice<screen_device>(m_screen_tag);
                    if (m_screen == null)
                        throw new emu_fatalerror("Screen '{0}' not found, explicitly set for device '{1}'", m_screen_tag, device().tag());
                }
                else
                {
                    // otherwise, look for a single match
                    screen_device_iterator iter = new screen_device_iterator(device().machine().root_device());
                    m_screen = iter.first();
                    if (iter.count() > 1)
                        throw new emu_fatalerror("No screen specified for device '{0}', but multiple screens found", device().tag());
                }
            }

            // fatal error if no screen is found
            if (m_screen == null && m_screen_required)
                throw new emu_fatalerror("Device '{0}' requires a screen", device().tag());

            // if we have a screen and it's not started, wait for it
            if (m_screen != null && !m_screen.started())
            {
                // avoid circular dependency if we are also a palette device
                device_palette_interface palintf;
                if (!device().interface_(out palintf))
                    throw new device_missing_dependencies();

                // no other palette may be specified
                if (m_screen.has_palette() && palintf != m_screen.palette())
                    throw new emu_fatalerror("Device '{0}' cannot control screen '{1}' with palette '{2}'", device().tag(), m_screen_tag, m_screen.palette().device().tag());
            }
        }
    }

    // iterator
    //typedef device_interface_iterator<device_video_interface> video_interface_iterator;
}
