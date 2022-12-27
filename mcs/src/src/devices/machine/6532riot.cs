// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame._6532riot_global;
using static mame.device_global;
using static mame.diexec_global;


namespace mame
{
    /***************************************************************************
        TYPE DEFINITIONS
    ***************************************************************************/

    // ======================> riot6532_device
    public class riot6532_device : device_t
    {
        //DEFINE_DEVICE_TYPE(RIOT6532, riot6532_device, "riot6532", "6532 RIOT")
        public static readonly emu.detail.device_type_impl RIOT6532 = DEFINE_DEVICE_TYPE("riot6532", "6532 RIOT", (type, mconfig, tag, owner, clock) => { return new riot6532_device(mconfig, tag, owner, clock); });


        //class riot6532_port
        //{
        //public:
        //    uint8_t                   m_in;
        //    uint8_t                   m_out;
        //    uint8_t                   m_ddr;
        //    devcb_read8             *m_in_cb;
        //    devcb_write8            *m_out_cb;
        //};


        //riot6532_port   m_port[2];

        devcb_read8 m_in_pa_cb;
        devcb_write8 m_out_pa_cb;
        devcb_read8 m_in_pb_cb;
        devcb_write8 m_out_pb_cb;
        devcb_write_line m_irq_cb;

        uint8_t m_irqstate;
        uint8_t m_irqenable;
        int m_irq;

        uint8_t m_pa7dir;     /* 0x80 = high-to-low, 0x00 = low-to-high */
        uint8_t m_pa7prev;

        uint8_t m_timershift;
        uint8_t m_timerstate;
        emu_timer m_timer;

        //enum
        //{
        //    TIMER_END_CB
        //};


        // construction/destruction
        riot6532_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, RIOT6532, tag, owner, clock)
        {
            m_in_pa_cb = new devcb_read8(this);
            m_out_pa_cb = new devcb_write8(this);
            m_in_pb_cb = new devcb_read8(this);
            m_out_pb_cb = new devcb_write8(this);
            m_irq_cb = new devcb_write_line(this);
            m_irqstate = 0;
            m_irqenable = 0;
            m_irq = CLEAR_LINE;
            m_pa7dir = 0;
            m_pa7prev = 0;
            m_timershift = 0;
            m_timerstate = 0;
            m_timer = null;


            //memset(m_port, 0x00, sizeof(m_port));
        }


        //auto in_pa_callback() { return m_in_pa_cb.bind(); }
        //auto out_pa_callback() { return m_out_pa_cb.bind(); }
        public devcb_read8.binder in_pb_callback() { return m_in_pb_cb.bind(); }  //auto in_pb_callback() { return m_in_pb_cb.bind(); }
        public devcb_write8.binder out_pb_callback() { return m_out_pb_cb.bind(); }  //auto out_pb_callback() { return m_out_pb_cb.bind(); }
        public devcb_write_line.binder irq_callback() { return m_irq_cb.bind(); }  //auto irq_callback() { return m_irq_cb.bind(); }

        //uint8_t read(offs_t offset);
        //void write(offs_t offset, uint8_t data);

        //uint8_t reg_r(uint8_t offset, bool debugger_access = false);
        //void reg_w(uint8_t offset, uint8_t data);

        //void porta_in_set(uint8_t data, uint8_t mask);
        //void portb_in_set(uint8_t data, uint8_t mask);

        //DECLARE_WRITE_LINE_MEMBER(pa0_w);
        //DECLARE_WRITE_LINE_MEMBER(pa1_w);
        //DECLARE_WRITE_LINE_MEMBER(pa2_w);
        //DECLARE_WRITE_LINE_MEMBER(pa3_w);
        //DECLARE_WRITE_LINE_MEMBER(pa4_w);
        //DECLARE_WRITE_LINE_MEMBER(pa5_w);
        //DECLARE_WRITE_LINE_MEMBER(pa6_w);
        //DECLARE_WRITE_LINE_MEMBER(pa7_w);
        //DECLARE_WRITE_LINE_MEMBER(pb0_w);
        //DECLARE_WRITE_LINE_MEMBER(pb1_w);
        //DECLARE_WRITE_LINE_MEMBER(pb2_w);
        //DECLARE_WRITE_LINE_MEMBER(pb3_w);
        //DECLARE_WRITE_LINE_MEMBER(pb4_w);
        //DECLARE_WRITE_LINE_MEMBER(pb5_w);
        //DECLARE_WRITE_LINE_MEMBER(pb6_w);
        //DECLARE_WRITE_LINE_MEMBER(pb7_w);

        //uint8_t porta_in_get();
        //uint8_t portb_in_get();

        //uint8_t porta_out_get();
        //uint8_t portb_out_get();

        //void timer_end();


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_reset() { throw new emu_unimplemented(); }
        protected override void device_timer(emu_timer timer, device_timer_id id, int param) { throw new emu_unimplemented(); }
        protected override void device_post_load() { throw new emu_unimplemented(); }
        protected override void device_clock_changed() { throw new emu_unimplemented(); }


        //void update_irqstate();
        //uint8_t apply_ddr(const riot6532_port *port);
        //void update_pa7_state();
        //uint8_t get_timer();
    }


    public static class _6532riot_global
    {
        public static riot6532_device RIOT6532<bool_Required>(machine_config mconfig, device_finder<riot6532_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, riot6532_device.RIOT6532, clock); }
    }
}
