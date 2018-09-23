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
    public static class namco06_global
    {
        public static void MCFG_NAMCO_06XX_ADD(out device_t device, machine_config config, device_t owner, string tag, XTAL clock) { mconfig_global.MCFG_DEVICE_ADD(out device, config, owner, tag, namco_06xx_device.NAMCO_06XX, clock); }
        public static void MCFG_NAMCO_06XX_MAINCPU(device_t device, string tag) { ((namco_06xx_device)device).set_maincpu(tag); }
        public static void MCFG_NAMCO_06XX_READ_0_CB(device_t device, read8_delegate read8_devcb) { ((namco_06xx_device)device).set_read_callback(0, read8_devcb); }  //DEVCB_##_devcb);
        public static void MCFG_NAMCO_06XX_READ_1_CB(device_t device, read8_delegate read8_devcb) { ((namco_06xx_device)device).set_read_callback(1, read8_devcb); }  //DEVCB_##_devcb);
        public static void MCFG_NAMCO_06XX_READ_2_CB(device_t device, read8_delegate read8_devcb) { ((namco_06xx_device)device).set_read_callback(2, read8_devcb); }  //DEVCB_##_devcb);
        public static void MCFG_NAMCO_06XX_READ_3_CB(device_t device, read8_delegate read8_devcb) { ((namco_06xx_device)device).set_read_callback(3, read8_devcb); }  //DEVCB_##_devcb);

        public static void MCFG_NAMCO_06XX_READ_REQUEST_0_CB(device_t device, write_line_delegate write_line_devcb) { ((namco_06xx_device)device).set_read_request_callback(0, write_line_devcb); }  //DEVCB_##_devcb);
        public static void MCFG_NAMCO_06XX_READ_REQUEST_1_CB(device_t device, write_line_delegate write_line_devcb) { ((namco_06xx_device)device).set_read_request_callback(1, write_line_devcb); }  //DEVCB_##_devcb);
        public static void MCFG_NAMCO_06XX_READ_REQUEST_2_CB(device_t device, write_line_delegate write_line_devcb) { ((namco_06xx_device)device).set_read_request_callback(2, write_line_devcb); }  //DEVCB_##_devcb);
        public static void MCFG_NAMCO_06XX_READ_REQUEST_3_CB(device_t device, write_line_delegate write_line_devcb) { ((namco_06xx_device)device).set_read_request_callback(3, write_line_devcb); }  //DEVCB_##_devcb);

        public static void MCFG_NAMCO_06XX_WRITE_0_CB(device_t device, write8_delegate write8_devcb) { ((namco_06xx_device)device).set_write_callback(0, write8_devcb); }  //DEVCB_##_devcb);
        public static void MCFG_NAMCO_06XX_WRITE_1_CB(device_t device, write8_delegate write8_devcb) { ((namco_06xx_device)device).set_write_callback(1, write8_devcb); }  //DEVCB_##_devcb);
        public static void MCFG_NAMCO_06XX_WRITE_2_CB(device_t device, write8_delegate write8_devcb) { ((namco_06xx_device)device).set_write_callback(2, write8_devcb); }  //DEVCB_##_devcb);
        public static void MCFG_NAMCO_06XX_WRITE_3_CB(device_t device, write8_delegate write8_devcb) { ((namco_06xx_device)device).set_write_callback(3, write8_devcb); }  //DEVCB_##_devcb);
    }


    /* device get info callback */
    class namco_06xx_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NAMCO_06XX, namco_06xx_device, "namco06", "Namco 06xx")
        static device_t device_creator_namco_06xx_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new namco_06xx_device(mconfig, tag, owner, clock); }
        public static readonly device_type NAMCO_06XX = DEFINE_DEVICE_TYPE(device_creator_namco_06xx_device, "namco06", "Namco 06xx");


        const bool VERBOSE = false;
        void LOG(string format, params object [] args) { if (VERBOSE) logerror(format, args); }


        // internal state
        byte m_control;
        emu_timer m_nmi_timer;

        required_device<cpu_device> m_nmicpu;

        devcb_read8 [] m_read = new devcb_read8[4];
        devcb_write_line [] m_readreq = new devcb_write_line[4];
        devcb_write8 [] m_write = new devcb_write8[4];


        namco_06xx_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO_06XX, tag, owner, clock)
        {
            m_control = 0;
            m_nmicpu = new required_device<cpu_device>(this, finder_base.DUMMY_TAG);

            for (int i = 0; i < 4; i++)
                m_read[i] = new devcb_read8(this);

            for (int i = 0; i < 4; i++)
                m_readreq[i] = new devcb_write_line(this);

            for (int i = 0; i < 4; i++)
                m_write[i] = new devcb_write8(this);
        }


        public void set_maincpu(string tag) { m_nmicpu.set_tag(tag); }  //downcast <namco_06xx_device &>(device).m_nmicpu.set_tag(tag);

        public devcb_base set_read_callback(int N, read8_delegate cb) { return m_read[N].set_callback(this, cb); }  //template <unsigned N, class Object> devcb_base &set_read_callback(Object &&cb) { return m_read[N].set_callback(std::forward<Object>(cb)); }
        public devcb_base set_read_request_callback(int N, write_line_delegate cb) { return m_readreq[N].set_callback(this, cb); }  //template <unsigned N, class Object> devcb_base &set_read_request_callback(Object &&cb) { return m_readreq[N].set_callback(std::forward<Object>(cb)); }
        public devcb_base set_write_callback(int N, write8_delegate cb) { return m_write[N].set_callback(this, cb); }  //template <unsigned N, class Object> devcb_base &set_write_callback(Object &&cb) { return m_write[N].set_callback(std::forward<Object>(cb)); }


        //READ8_MEMBER( namco_06xx_device::data_r )
        public u8 data_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            byte result = 0xff;

            LOG("{0}: 06XX '{1}' read offset {2}\n", machine().describe_context(), tag(), offset);

            if ((m_control & 0x10) == 0)
            {
                logerror("{0}: 06XX '{1}' read in write mode {2}\n", machine().describe_context(), tag(), m_control);
                return 0;
            }

            if (global.BIT(m_control, 0) != 0) result &= m_read[0].op(space, 0);
            if (global.BIT(m_control, 1) != 0) result &= m_read[1].op(space, 0);
            if (global.BIT(m_control, 2) != 0) result &= m_read[2].op(space, 0);
            if (global.BIT(m_control, 3) != 0) result &= m_read[3].op(space, 0);

            return result;
        }

        //WRITE8_MEMBER( namco_06xx_device::data_w )
        public void data_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            LOG("{0}: 06XX '{1}' write offset {2} = {3}\n", machine().describe_context(), tag(), offset, data);

            if ((m_control & 0x10) != 0)
            {
                logerror("{0}: 06XX '{1}' write in read mode {2}\n", machine().describe_context(), tag(), m_control);
                return;
            }

            if (global.BIT(m_control, 0) != 0) m_write[0].op(space, 0, data);
            if (global.BIT(m_control, 1) != 0) m_write[1].op(space, 0, data);
            if (global.BIT(m_control, 2) != 0) m_write[2].op(space, 0, data);
            if (global.BIT(m_control, 3) != 0) m_write[3].op(space, 0, data);
        }

        //READ8_MEMBER( namco_06xx_device::ctrl_r )
        public u8 ctrl_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            LOG("{0}: 06XX '{1}' ctrl_r\n", machine().describe_context(), tag());
            return m_control;
        }

        //WRITE8_MEMBER( namco_06xx_device::ctrl_w )
        public void ctrl_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            LOG("{0}: 06XX '{1}' control {2}\n", machine().describe_context(), tag(), data);

            m_control = data;

            if ((m_control & 0x0f) == 0)
            {
                LOG("disabling nmi generate timer\n");
                m_nmi_timer.adjust(attotime.never);
            }
            else
            {
                LOG("setting nmi generate timer to 200us\n");

                // this timing is critical. Due to a bug, Bosconian will stop responding to
                // inputs if a transfer terminates at the wrong time.
                // On the other hand, the time cannot be too short otherwise the 54XX will
                // not have enough time to process the incoming controls.
                m_nmi_timer.adjust(attotime.from_usec(200), 0, attotime.from_usec(200));

                if ((m_control & 0x10) != 0)
                {
                    if (global.BIT(m_control, 0) != 0) m_readreq[0].op(space, 0);
                    if (global.BIT(m_control, 1) != 0) m_readreq[1].op(space, 0);
                    if (global.BIT(m_control, 2) != 0) m_readreq[2].op(space, 0);
                    if (global.BIT(m_control, 3) != 0) m_readreq[3].op(space, 0);
                }
            }
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            foreach (devcb_read8 cb in m_read)
                cb.resolve_safe(0xff);

            foreach (devcb_write_line cb in m_readreq)
                cb.resolve_safe();

            foreach (devcb_write8 cb in m_write)
                cb.resolve_safe();

            /* allocate a timer */
            m_nmi_timer = machine().scheduler().timer_alloc(nmi_generate); //timer_expired_delegate(FUNC(namco_06xx_device::nmi_generate),this));

            save_item(m_control, "m_control");
        }

        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            m_control = 0;
        }


        void nmi_generate(object o, int param)
        {
            if (!m_nmicpu.target.execute().suspended(device_execute_interface.SUSPEND_REASON_HALT | device_execute_interface.SUSPEND_REASON_RESET | device_execute_interface.SUSPEND_REASON_DISABLE))
            {
                LOG("NMI cpu '{0}'\n", m_nmicpu.tag());
                m_nmicpu.target.execute().pulse_input_line((int)INPUT_LINE.INPUT_LINE_NMI, attotime.zero);
            }
            else
            {
                LOG("NMI not generated because cpu '{0}' is suspended\n", m_nmicpu.tag());
            }
        }
    }
}
