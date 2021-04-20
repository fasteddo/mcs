// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;


namespace mame
{
    /* device get info callback */
    public class namco_06xx_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NAMCO_06XX, namco_06xx_device, "namco06", "Namco 06xx")
        static device_t device_creator_namco_06xx_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new namco_06xx_device(mconfig, tag, owner, clock); }
        public static readonly device_type NAMCO_06XX = DEFINE_DEVICE_TYPE(device_creator_namco_06xx_device, "namco06", "Namco 06xx");


        const bool VERBOSE = false;
        void LOG(string format, params object [] args) { if (VERBOSE) logerror(format, args); }


        // internal state
        emu_timer m_nmi_timer;
        uint8_t m_control;
        bool m_next_timer_state;
        bool m_nmi_stretch;
        bool m_rw_stretch;
        bool m_rw_change;

        required_device<cpu_device> m_nmicpu;

        devcb_write_line.array<devcb_write_line> m_chipsel;  //devcb_write_line::array<4> m_chipsel;
        devcb_write_line.array<devcb_write_line> m_rw;  //devcb_write_line::array<4> m_rw;
        devcb_read8.array<devcb_read8> m_read;
        devcb_write8.array<devcb_write8> m_write;


        namco_06xx_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO_06XX, tag, owner, clock)
        {
            m_control = 0;
            m_next_timer_state = false;
            m_nmi_stretch = false;
            m_rw_stretch = false;
            m_rw_change = false;
            m_nmicpu = new required_device<cpu_device>(this, finder_base.DUMMY_TAG);
            m_chipsel = new devcb_write.array<devcb_write_line>(4, this, () => { return new devcb_write_line(this); });
            m_rw = new devcb_write.array<devcb_write_line>(4, this, () => { return new devcb_write_line(this); });
            m_read = new devcb_read8.array<devcb_read8>(4, this, () => { return new devcb_read8(this); });
            m_write = new devcb_write8.array<devcb_write8>(4, this, () => { return new devcb_write8(this); });
        }


        public void set_maincpu(string tag) { m_nmicpu.set_tag(tag); }  //template <typename T> void set_maincpu(T &&tag) { m_nmicpu.set_tag(std::forward<T>(tag)); }
        public void set_maincpu(finder_base tag) { m_nmicpu.set_tag(tag); }  //template <typename T> void set_maincpu(T &&tag) { m_nmicpu.set_tag(std::forward<T>(tag)); }

        public devcb_write.binder chip_select_callback(int N) { return m_chipsel[N].bind(); }  //template <unsigned N> auto chip_select_callback() { return m_chipsel[N].bind(); }
        public devcb_write.binder rw_callback(int N) { return m_rw[N].bind(); }  //template <unsigned N> auto rw_callback() { return m_rw[N].bind(); }
        public devcb_read.binder read_callback(int N) { return m_read[N].bind(); }  //template <unsigned N> auto read_callback() { return m_read[N].bind(); }
        public devcb_write.binder write_callback(int N) { return m_write[N].bind(); }  //template <unsigned N> auto write_callback() { return m_write[N].bind(); }


        public uint8_t data_r(offs_t offset)
        {
            uint8_t result = 0xff;

            if (BIT(m_control, 4) == 0)
            {
                logerror("{0}: 06XX '{1}' read in write mode {2}\n", machine().describe_context(), tag(), m_control);
                return 0;
            }

            if (BIT(m_control, 0) != 0) result &= m_read[0].op(0);
            if (BIT(m_control, 1) != 0) result &= m_read[1].op(0);
            if (BIT(m_control, 2) != 0) result &= m_read[2].op(0);
            if (BIT(m_control, 3) != 0) result &= m_read[3].op(0);

            return result;
        }


        public void data_w(offs_t offset, uint8_t data)
        {
            if (BIT(m_control, 4) != 0)
            {
                logerror("{0}: 06XX '{1}' write in read mode {2}\n", machine().describe_context(), tag(), m_control);
                return;
            }

            if (BIT(m_control, 0) != 0) m_write[0].op(0, data);
            if (BIT(m_control, 1) != 0) m_write[1].op(0, data);
            if (BIT(m_control, 2) != 0) m_write[2].op(0, data);
            if (BIT(m_control, 3) != 0) m_write[3].op(0, data);
        }


        public uint8_t ctrl_r()
        {
            return m_control;
        }


        public void ctrl_w(uint8_t data)
        {
            m_control = data;

            // The upper 3 control bits are the clock divider.
            if ((m_control & 0xE0) == 0)
            {
                m_nmi_timer.adjust(attotime.never);
                set_nmi(CLEAR_LINE);
                m_chipsel[0].op(0, CLEAR_LINE);
                m_chipsel[1].op(0, CLEAR_LINE);
                m_chipsel[2].op(0, CLEAR_LINE);
                m_chipsel[3].op(0, CLEAR_LINE);
                // Setting this to true makes the next RW change not stretch.
                m_next_timer_state = true;
            }
            else
            {
                m_rw_stretch = !m_next_timer_state;
                m_rw_change = true;
                m_next_timer_state = true;
                m_nmi_stretch = BIT(m_control, 4) != 0;
                // NMI is cleared immediately if its to be stretched.
                if (m_nmi_stretch) set_nmi(CLEAR_LINE);

                uint8_t num_shifts = (uint8_t)((m_control & 0xe0) >> 5);
                uint8_t divisor = (uint8_t)(1U << num_shifts);
                // The next change should happen on the next clock falling edge.
                // Xevious' race causes this to bootloopsif it isn't 0.
                m_nmi_timer.adjust(attotime.zero, 0, attotime.from_hz(clock() / divisor) / 2);
            }
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            m_chipsel.resolve_all_safe();
            m_rw.resolve_all_safe();
            m_read.resolve_all_safe(0xff);
            m_write.resolve_all_safe();

            /* allocate a timer */
            m_nmi_timer = machine().scheduler().timer_alloc(nmi_generate); //timer_expired_delegate(FUNC(namco_06xx_device::nmi_generate),this));

            save_item(NAME(new { m_control }));
            save_item(NAME(new { m_next_timer_state }));
            save_item(NAME(new { m_nmi_stretch }));
            save_item(NAME(new { m_rw_stretch }));
            save_item(NAME(new { m_rw_change }));
        }

        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            m_control = 0;
        }


        void set_nmi(int state)
        {
            if (!m_nmicpu.target.suspended(device_execute_interface.SUSPEND_REASON_HALT | device_execute_interface.SUSPEND_REASON_RESET | device_execute_interface.SUSPEND_REASON_DISABLE))
            {
                m_nmicpu.target.set_input_line(device_execute_interface.INPUT_LINE_NMI, state);
            }
        }


        void nmi_generate(object o, int param)
        {
            // This timer runs at twice the clock, since we do work on both the
            // rising and falling edge.
            //
            // During reads, the first NMI pulse is supressed to give the chip a
            // cycle to write.
            //
            // If the control register is written while CS is asserted, RW won't be
            // changed until the next rising edge.

            if (m_rw_change && m_next_timer_state)
            {
                if (!m_rw_stretch)
                {
                    m_rw[0].op(0, BIT(m_control, 4));
                    m_rw[1].op(0, BIT(m_control, 4));
                    m_rw[2].op(0, BIT(m_control, 4));
                    m_rw[3].op(0, BIT(m_control, 4));
                    m_rw_change = false;
                }
            }

            if (m_next_timer_state && !m_nmi_stretch )
            {
                set_nmi(ASSERT_LINE);
            }
            else
            {
                set_nmi(CLEAR_LINE);
            }

            m_chipsel[0].op(0, (BIT(m_control, 0) != 0 && m_next_timer_state) ? 1 : 0);
            m_chipsel[1].op(0, (BIT(m_control, 1) != 0 && m_next_timer_state) ? 1 : 0);
            m_chipsel[2].op(0, (BIT(m_control, 2) != 0 && m_next_timer_state) ? 1 : 0);
            m_chipsel[3].op(0, (BIT(m_control, 3) != 0 && m_next_timer_state) ? 1 : 0);

            m_next_timer_state = !m_next_timer_state;
            m_nmi_stretch = false;
            m_rw_stretch = false;
        }
    }
}
