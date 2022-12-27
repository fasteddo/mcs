// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using unsigned = System.UInt32;

using static mame.diexec_global;
using static mame.device_global;
using static mame.emucore_global;
using static mame.namco06_global;
using static mame.util;


namespace mame
{
    /* device get info callback */
    public class namco_06xx_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NAMCO_06XX, namco_06xx_device, "namco06", "Namco 06xx")
        public static readonly emu.detail.device_type_impl NAMCO_06XX = DEFINE_DEVICE_TYPE("namco06", "Namco 06xx", (type, mconfig, tag, owner, clock) => { return new namco_06xx_device(mconfig, tag, owner, clock); });


        const int VERBOSE = 0;
        //#include "logmacro.h"
        void LOG(string format, params object [] args) { logmacro_global.LOG(VERBOSE, this, format, args); }


        // internal state
        emu_timer m_nmi_timer = null;
        uint8_t m_control;
        bool m_next_timer_state;
        bool m_read_stretch;

        required_device<cpu_device> m_nmicpu;

        devcb_write_line.array<u64_const_4> m_chipsel;  //devcb_write_line::array<4> m_chipsel;
        devcb_write_line.array<u64_const_4> m_rw;  //devcb_write_line::array<4> m_rw;
        devcb_read8.array<u64_const_4> m_read;
        devcb_write8.array<u64_const_4> m_write;


        namco_06xx_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO_06XX, tag, owner, clock)
        {
            m_control = 0;
            m_next_timer_state = false;
            m_read_stretch = false;
            m_nmicpu = new required_device<cpu_device>(this, finder_base.DUMMY_TAG);
            m_chipsel = new devcb_write_line.array<u64_const_4>(this, () => { return new devcb_write_line(this); });
            m_rw = new devcb_write_line.array<u64_const_4>(this, () => { return new devcb_write_line(this); });
            m_read = new devcb_read8.array<u64_const_4>(this, () => { return new devcb_read8(this); });
            m_write = new devcb_write8.array<u64_const_4>(this, () => { return new devcb_write8(this); });
        }


        public void set_maincpu(string tag) { m_nmicpu.set_tag(tag); }  //template <typename T> void set_maincpu(T &&tag) { m_nmicpu.set_tag(std::forward<T>(tag)); }
        public void set_maincpu(finder_base tag) { m_nmicpu.set_tag(tag); }  //template <typename T> void set_maincpu(T &&tag) { m_nmicpu.set_tag(std::forward<T>(tag)); }

        public devcb_write_line.binder chip_select_callback<unsigned_N>() where unsigned_N : u32_const, new() { unsigned N = new unsigned_N().value; return m_chipsel[N].bind(); }  //template <unsigned N> auto chip_select_callback() { return m_chipsel[N].bind(); }
        public devcb_write_line.binder rw_callback<unsigned_N>() where unsigned_N : u32_const, new() { unsigned N = new unsigned_N().value; return m_rw[N].bind(); }  //template <unsigned N> auto rw_callback() { return m_rw[N].bind(); }
        public devcb_read8.binder read_callback<unsigned_N>() where unsigned_N : u32_const, new() { unsigned N = new unsigned_N().value; return m_read[N].bind(); }  //template <unsigned N> auto read_callback() { return m_read[N].bind(); }
        public devcb_write8.binder write_callback<unsigned_N>() where unsigned_N : u32_const, new() { unsigned N = new unsigned_N().value; return m_write[N].bind(); }  //template <unsigned N> auto write_callback() { return m_write[N].bind(); }


        public uint8_t data_r(offs_t offset)
        {
            uint8_t result = 0xff;

            if (BIT(m_control, 4) == 0)
            {
                logerror("{0}: 06XX '{1}' read in write mode {2}\n", machine().describe_context(), tag(), m_control);
                return 0;
            }

            if (BIT(m_control, 0) != 0) result &= m_read[0].op_u8(0);
            if (BIT(m_control, 1) != 0) result &= m_read[1].op_u8(0);
            if (BIT(m_control, 2) != 0) result &= m_read[2].op_u8(0);
            if (BIT(m_control, 3) != 0) result &= m_read[3].op_u8(0);

            return result;
        }


        public void data_w(offs_t offset, uint8_t data)
        {
            machine().scheduler().synchronize(write_sync, data);  //machine().scheduler().synchronize(timer_expired_delegate(FUNC(namco_06xx_device::write_sync),this), data);
        }


        public uint8_t ctrl_r()
        {
            return m_control;
        }


        public void ctrl_w(uint8_t data)
        {
            machine().scheduler().synchronize(ctrl_w_sync, data);  //machine().scheduler().synchronize(timer_expired_delegate(FUNC(namco_06xx_device::ctrl_w_sync),this), data);
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            m_chipsel.resolve_all_safe();
            m_rw.resolve_all_safe();
            m_read.resolve_all_safe_u8(0xff);
            m_write.resolve_all_safe();

            /* allocate a timer */
            m_nmi_timer = machine().scheduler().timer_alloc(nmi_generate); //timer_expired_delegate(FUNC(namco_06xx_device::nmi_generate),this));

            save_item(NAME(new { m_control }));
            save_item(NAME(new { m_next_timer_state }));
            save_item(NAME(new { m_read_stretch }));
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
            if (!m_nmicpu.op0.suspended(SUSPEND_REASON_HALT | SUSPEND_REASON_RESET | SUSPEND_REASON_DISABLE))
            {
                m_nmicpu.op0.set_input_line(INPUT_LINE_NMI, state);
            }
        }


        //TIMER_CALLBACK_MEMBER( namco_06xx_device::nmi_generate )
        void nmi_generate(s32 param)
        {
            // This timer runs at twice the clock, since we do work on both the
            // rising and falling edge.
            //
            // During reads, the first NMI pulse is supressed to give the chip a
            // cycle to write.

            if (m_next_timer_state)
            {
                m_rw[0].op_s32(0, BIT(m_control, 4));
                m_rw[1].op_s32(0, BIT(m_control, 4));
                m_rw[2].op_s32(0, BIT(m_control, 4));
                m_rw[3].op_s32(0, BIT(m_control, 4));
            }

            if (m_next_timer_state && !m_read_stretch)
            {
                set_nmi(ASSERT_LINE);
            }
            else
            {
                set_nmi(CLEAR_LINE);
            }

            m_read_stretch = false;

            m_chipsel[0].op_s32(0, (BIT(m_control, 0) != 0 && m_next_timer_state) ? 1 : 0);
            m_chipsel[1].op_s32(0, (BIT(m_control, 1) != 0 && m_next_timer_state) ? 1 : 0);
            m_chipsel[2].op_s32(0, (BIT(m_control, 2) != 0 && m_next_timer_state) ? 1 : 0);
            m_chipsel[3].op_s32(0, (BIT(m_control, 3) != 0 && m_next_timer_state) ? 1 : 0);

            m_next_timer_state = !m_next_timer_state;
        }


        //TIMER_CALLBACK_MEMBER( write_sync );
        void write_sync(s32 param)
        {
            if (BIT(m_control, 4) != 0)
            {
                logerror("{0}: 06XX '{1}' write in read mode {2}\n", machine().describe_context(), tag(), m_control);
                return;
            }

            if (BIT(m_control, 0) != 0) m_write[0].op_u8(0, (u8)param);
            if (BIT(m_control, 1) != 0) m_write[1].op_u8(0, (u8)param);
            if (BIT(m_control, 2) != 0) m_write[2].op_u8(0, (u8)param);
            if (BIT(m_control, 3) != 0) m_write[3].op_u8(0, (u8)param);
        }


        //TIMER_CALLBACK_MEMBER( ctrl_w_sync );
        void ctrl_w_sync(s32 param)
        {
            m_control = (uint8_t)param;

            // The upper 3 control bits are the clock divider.
            if ((m_control & 0xe0) == 0)
            {
                m_nmi_timer.adjust(attotime.never);
                m_next_timer_state = true;
                set_nmi(CLEAR_LINE);
                m_chipsel[0].op_s32(0, CLEAR_LINE);
                m_chipsel[1].op_s32(0, CLEAR_LINE);
                m_chipsel[2].op_s32(0, CLEAR_LINE);
                m_chipsel[3].op_s32(0, CLEAR_LINE);
                // RW is left as-is
            }
            else
            {
                // NMI is cleared immediately if this is a read
                // It will be supressed the next clock cycle.
                if (BIT(m_control, 4) != 0)
                {
                    set_nmi(CLEAR_LINE);
                    m_read_stretch = true;
                }
                else
                {
                    m_read_stretch = false;
                }


                uint8_t num_shifts = (uint8_t)((m_control & 0xe0) >> 5);
                uint8_t divisor = (uint8_t)(1U << num_shifts);
                attotime period = attotime.from_hz(clock() / divisor) / 2;
                // This delay should be the next falling clock.
                // That's complicated to get, as it's derived from the master
                // clock. The CPU uses this same clock, so writes will come at
                // a specific pace.
                // Instead, just approximate a quarter cycle.
                // Xevious is very sensitive to this. It will bootloop if it
                // isn't correct.
                attotime delay = attotime.from_hz(clock()) / 4; // average of one clock
                if (!m_next_timer_state)
                {
                    // NMI is asserted, wait one additional clock to start
                    m_nmi_timer.adjust(delay + attotime.from_hz(clock() / divisor), 0, period);
                }
                else
                {
                    m_nmi_timer.adjust(delay, 0, period);
                }
            }
        }
    }


    static class namco06_global
    {
        public static namco_06xx_device NAMCO_06XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_06xx_device>(mconfig, tag, namco_06xx_device.NAMCO_06XX, clock); }
    }
}
