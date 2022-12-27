// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;

using static mame.device_global;
using static mame.diexec_global;
using static mame.emucore_global;
using static mame.m68705_global;
using static mame.taitosjsec_global;
using static mame.util;


namespace mame
{
    public class taito_sj_security_mcu_device : device_t
    {
        //DEFINE_DEVICE_TYPE(TAITO_SJ_SECURITY_MCU,  taito_sj_security_mcu_device, "taitosjsecmcu", "Taito SJ Security MCU Interface")
        public static readonly emu.detail.device_type_impl TAITO_SJ_SECURITY_MCU = DEFINE_DEVICE_TYPE("taitosjsecmcu", "Taito SJ Security MCU Interface", (type, mconfig, tag, owner, clock) => { return new taito_sj_security_mcu_device(mconfig, tag, owner, clock); });


        public enum int_mode
        {
            NONE,
            LATCH,
            WRITE
        }


        required_device<m68705p_device> m_mcu;

        int_mode m_int_mode;
        devcb_read8 m_68read_cb;
        devcb_write8 m_68write_cb;
        devcb_write_line m_68intrq_cb;
        devcb_write_line m_busrq_cb;

        // IC6/IC10 latch/count the low byte, IC14 buffers low byte, IC3 latches high byte
        u16 m_addr;

        // latched by IC9
        u8 m_mcu_data;

        // latched by IC13
        u8 m_host_data;

        // buffered by IC16
        u8 m_read_data;

        // IC7 pin 6, indicates CPU has accepted data from MCU
        bool m_zaccept;

        // IC7 pin 9, indicates CPU has sent data to MCU
        bool m_zready;

        // previous MCU port outputs for detecting edges
        u8 m_pa_val;
        u8 m_pb_val;

        // input state
        bool m_busak;
        bool m_reset;


        taito_sj_security_mcu_device(
                machine_config mconfig,
                string tag,
                device_t owner,
                u32 clock)
            : base(mconfig, TAITO_SJ_SECURITY_MCU, tag, owner, clock)
        {
            m_mcu = new required_device<m68705p_device>(this, "mcu");
            m_int_mode = int_mode.NONE;
            m_68read_cb = new devcb_read8(this);
            m_68write_cb = new devcb_write8(this);
            m_68intrq_cb = new devcb_write_line(this);
            m_busrq_cb = new devcb_write_line(this);
            m_addr = 0;
            m_mcu_data = 0;
            m_host_data = 0;
            m_read_data = 0;
            m_zaccept = false;
            m_zready = false;
            m_pa_val = 0;
            m_pb_val = 0;
            m_busak = false;
            m_reset = false;
        }


        protected override void device_start()
        {
            m_68read_cb.resolve_safe_u8(0xff);
            m_68write_cb.resolve_safe();
            m_68intrq_cb.resolve_safe();
            m_busrq_cb.resolve_safe();

            save_item(NAME(new { m_addr }));
            save_item(NAME(new { m_mcu_data }));
            save_item(NAME(new { m_host_data }));
            save_item(NAME(new { m_read_data }));
            save_item(NAME(new { m_zaccept }));
            save_item(NAME(new { m_zready }));
            save_item(NAME(new { m_pa_val }));
            save_item(NAME(new { m_pb_val }));
            save_item(NAME(new { m_busak }));
            save_item(NAME(new { m_reset }));

            m_addr = 0xffff;
            m_mcu_data = 0xff;
            m_host_data = 0xff;
            m_read_data = 0xff;
            m_pb_val = 0xff;
            m_busak = false;
            m_reset = false;
        }


        protected override void device_reset()
        {
            m_zaccept = true;
            m_zready = false;
            if (int_mode.LATCH == m_int_mode)
                m_mcu.op0.set_input_line(M68705_IRQ_LINE, CLEAR_LINE);
        }


        protected override void device_add_mconfig(machine_config config)
        {
            M68705P5(config, m_mcu, DERIVED_CLOCK(1, 1));
            m_mcu.op0.porta_r().set(mcu_pa_r).reg();
            m_mcu.op0.portc_r().set(mcu_pc_r).reg();
            m_mcu.op0.porta_w().set(mcu_pa_w).reg();
            m_mcu.op0.portb_w().set(mcu_pb_w).reg();
        }


        public u8 data_r(address_space space, offs_t offset)
        {
            if (BIT(offset, 0) != 0)
            {
                // ZLSTATUS
                machine().scheduler().boost_interleave(attotime.zero, attotime.from_usec(10));
                return
                    (u8)(((u8)space.unmap() & 0xfc) |
                          (u8)(m_zaccept ? 0x00 : 0x02) |
                          (u8)(m_zready ? 0x00 : 0x01));
            }
            else
            {
                // ZLREAD
                if (!machine().side_effects_disabled())
                    m_zaccept = true;

                return m_mcu_data;
            }
        }


        public void data_w(offs_t offset, u8 data)
        {
            if (BIT(offset, 0) != 0)
            {
                // ZINTRQ
                // if jumpered this way, the Z80 write strobe pulses the MCU interrupt line
                // should be PULSE_LINE because it's edge sensitive, but diexec only allows PULSE_LINE on reset and NMI
                if (int_mode.WRITE == m_int_mode)
                    m_mcu.op0.set_input_line(M68705_IRQ_LINE, HOLD_LINE);
            }
            else
            {
                // ZLWRITE
                device_scheduler sched = machine().scheduler();
                sched.synchronize(do_host_write, data);
                sched.boost_interleave(attotime.zero, attotime.from_usec(10));
            }
        }


        public void set_int_mode(int_mode mode) { m_int_mode = mode; }
        public devcb_read8.binder m68read_cb() { return m_68read_cb.bind(); }
        public devcb_write8.binder m68write_cb() { return m_68write_cb.bind(); }
        public devcb_write_line.binder m68intrq_cb() { return m_68intrq_cb.bind(); }
        public devcb_write_line.binder busrq_cb() { return m_busrq_cb.bind(); }


        //WRITE_LINE_MEMBER(taito_sj_security_mcu_device::busak_w)
        public void busak_w(int state)
        {
            m_busak = (ASSERT_LINE == state);
        }


        u8 mcu_pa_r() { return get_bus_val(); }

        u8 mcu_pc_r()
        {
            // FIXME 68INTAK is on PC3 but we're ignoring it
            return
                (u8)((m_zready ? 0x01U : 0x00U) |
                     (m_zaccept ? 0x02U : 0x00U) |
                     (m_busak ? 0x00U : 0x04U));
        }


        void mcu_pa_w(u8 data)
        {
            m_pa_val = data;
            if (BIT(~m_pb_val, 6) != 0)
                m_addr = (u16)((m_addr & 0xff00U) | (u16)get_bus_val());
        }


        void mcu_pb_w(u8 data)
        {
            bool inc_addr = false;
            u8 diff = (u8)(m_pb_val ^ data);

            // 68INTRQ
            if (BIT(diff, 0) != 0)
                m_68intrq_cb.op_s32(BIT(data, 0) != 0 ? CLEAR_LINE : ASSERT_LINE);

            // 68LRD
            u8 bus_val = get_bus_val();
            if (BIT(diff & data, 1) != 0)
            {
                machine().scheduler().synchronize(do_mcu_read);
                if (int_mode.LATCH == m_int_mode)
                    m_mcu.op0.set_input_line(M68705_IRQ_LINE, CLEAR_LINE);
            }

            // 68LWR
            if (BIT(diff & data, 2) != 0)
                machine().scheduler().synchronize(do_mcu_write, bus_val);

            // BUSRQ
            if (BIT(diff, 3) != 0)
                m_busrq_cb.op_s32(BIT(data, 3) != 0 ? CLEAR_LINE : ASSERT_LINE);

            // 68WRITE
            if (BIT(diff, 4) != 0)
            {
                if (BIT(~data, 4) != 0)
                    m_68write_cb.op_u8(m_addr, bus_val);
                else if (BIT(data, 5) != 0)
                    inc_addr = true;
            }

            // 68READ
            if (BIT(diff, 5) != 0)
            {
                if (BIT(~data, 5) != 0)
                    m_read_data = m_68read_cb.op_u8(m_addr);
                else if (BIT(data, 4) != 0)
                    inc_addr = true;
            }

            // LAL
            if (BIT(~data, 6) != 0)
                m_addr = (u16)((m_addr & 0xff00) | (u16)bus_val);
            else if (inc_addr)
                m_addr = (u16)((m_addr & 0xff00) | ((m_addr + 1) & 0x00ff));

            // UAL
            if (BIT(~data, 7) != 0)
                m_addr = (u16)((m_addr & 0x00ff) | ((u16)bus_val << 8));

            m_pb_val = data;
        }


        u8 get_bus_val() { return (u8)((BIT(~m_pb_val, 1) != 0 ? m_host_data : 0xffU) & m_pa_val & ((m_busak && BIT(~m_pb_val, 5) != 0) ? m_read_data : 0xffU)); }


        //TIMER_CALLBACK_MEMBER(taito_sj_security_mcu_device::do_mcu_read)
        void do_mcu_read(object ptr, s32 param)  //void *ptr, s32 param)
        {
            m_zready = false;
        }


        //TIMER_CALLBACK_MEMBER(taito_sj_security_mcu_device::do_mcu_write)
        void do_mcu_write(object ptr, s32 param)  //void *ptr, s32 param)
        {
            m_mcu_data = (u8)param;
            if (!m_reset)
                m_zaccept = false;
        }


        //TIMER_CALLBACK_MEMBER(taito_sj_security_mcu_device::do_host_write)
        void do_host_write(object ptr, s32 param)  //void *ptr, s32 param)
        {
            m_host_data = (u8)param;
            if (!m_reset)
            {
                m_zready = true;
                if (int_mode.LATCH == m_int_mode)
                    m_mcu.op0.set_input_line(M68705_IRQ_LINE, ASSERT_LINE);
            }
        }
    }


    static class taitosjsec_global
    {
        public static taito_sj_security_mcu_device TAITO_SJ_SECURITY_MCU<bool_Required>(machine_config mconfig, device_finder<taito_sj_security_mcu_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, taito_sj_security_mcu_device.TAITO_SJ_SECURITY_MCU, clock); }
    }
}
