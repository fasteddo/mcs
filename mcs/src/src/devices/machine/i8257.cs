// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.diexec_global;
using static mame.emucore_global;
using static mame.i8257_global;
using static mame.util;


namespace mame
{
    public class i8257_device : device_t
                                //device_execute_interface
    {
        //DEFINE_DEVICE_TYPE(I8257, i8257_device, "i8257", "Intel 8257 DMA Controller")
        public static readonly emu.detail.device_type_impl I8257 = DEFINE_DEVICE_TYPE("i8257", "Intel 8257 DMA Controller", (type, mconfig, tag, owner, clock) => { return new i8257_device(mconfig, tag, owner, clock); });


        class device_execute_interface_i8257 : device_execute_interface
        {
            public device_execute_interface_i8257(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void execute_run() { ((i8257_device)device()).device_execute_interface_execute_run(); }
        }


        //#define LOG_GENERAL (1U << 0) //defined in logmacro.h already
        const int LOG_SETUP     = 1 << 1;
        const int LOG_TFR       = 1 << 2;
        const int VERBOSE = 0;  //#define VERBOSE (LOG_GENERAL | LOG_SETUP | LOG_TFR)
        //#define LOG_OUTPUT_STREAM std::cout
        //#include "logmacro.h"
        void LOGMASKED(int mask, string format, params object [] args) { logmacro_global.LOGMASKED(VERBOSE, mask, this, format, args); }
        void LOG(string format, params object [] args) { logmacro_global.LOG(VERBOSE, this, format, args); }

        void LOGSETUP(string format, params object [] args) { LOGMASKED(LOG_SETUP, format, args); }  //#define LOGSETUP(...) LOGMASKED(LOG_SETUP, __VA_ARGS__)
        void LOGTFR(string format, params object [] args) { LOGMASKED(LOG_TFR, format, args); }  //#define LOGTFR(...)   LOGMASKED(LOG_TFR, __VA_ARGS__)


        //enum
        //{
        const offs_t REGISTER_ADDRESS = 0;
        const offs_t REGISTER_WORD_COUNT = 1;
        const offs_t REGISTER_STATUS = 8;
        const offs_t REGISTER_MODE = REGISTER_STATUS;
        //};


        bool MODE_CHAN_ENABLE(int x) { return BIT(m_transfer_mode, x) != 0; }
        bool MODE_ROTATING_PRIORITY { get { return BIT(m_transfer_mode, 4) != 0; } }
        bool MODE_EXTENDED_WRITE { get { return BIT(m_transfer_mode, 5) != 0; } }
        bool MODE_TC_STOP { get { return BIT(m_transfer_mode, 6) != 0; } }
        bool MODE_AUTOLOAD { get { return BIT(m_transfer_mode, 7) != 0; } }
        uint8_t MODE_TRANSFER_MASK { get { return m_channel[m_current_channel].m_mode; } }
        const uint8_t MODE_TRANSFER_VERIFY     = 0;
        const uint8_t MODE_TRANSFER_WRITE      = 1;
        const uint8_t MODE_TRANSFER_READ       = 2;


        //enum
        //{
        const int STATE_SI = 0;
        const int STATE_S0 = 1;
        const int STATE_S1 = 2;
        const int STATE_S2 = 3;
        const int STATE_S3 = 4;
        const int STATE_SW = 5;
        const int STATE_S4 = 6;
        //};


        device_execute_interface_i8257 m_diexec;


        bool m_reverse_rw;
        bool m_tc;
        int m_msb;
        int m_hreq;
        int m_hack;
        int m_ready;
        int m_state;
        int m_current_channel;
        int m_last_channel;
        uint8_t m_transfer_mode;
        uint8_t m_status;
        uint8_t m_request;
        uint8_t m_temp;

        devcb_write_line m_out_hrq_cb;
        devcb_write_line m_out_tc_cb;

        // accessors to main memory
        devcb_read8 m_in_memr_cb;
        devcb_write8 m_out_memw_cb;

        // channel accessors
        devcb_read8.array<u64_const_4> m_in_ior_cb;
        devcb_write8.array<u64_const_4> m_out_iow_cb;
        devcb_write_line.array<u64_const_4> m_out_dack_cb;

        struct channel
        {
            public uint16_t m_address;
            public uint16_t m_count;
            public uint8_t m_mode;
        }
        channel [] m_channel = new channel [4];


        intref m_icount = new intref();  //int m_icount;


        // construction/destruction
        i8257_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, I8257, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_i8257(mconfig, this));  //device_execute_interface(mconfig, *this),


            m_icount.i = 0;
            m_reverse_rw = false;
            m_tc = false;
            m_msb = 0;
            m_hreq = CLEAR_LINE;
            m_hack = 0;
            m_ready = 1;
            m_state = 0;
            m_current_channel = 0;
            m_last_channel = 0;
            m_transfer_mode = 0;
            m_status = 0;
            m_request = 0;
            m_temp = 0;
            m_out_hrq_cb = new devcb_write_line(this);
            m_out_tc_cb = new devcb_write_line(this);
            m_in_memr_cb = new devcb_read8(this);
            m_out_memw_cb = new devcb_write8(this);
            m_in_ior_cb = new devcb_read8.array<u64_const_4>(this, () => { return new devcb_read8(this); });
            m_out_iow_cb = new devcb_write8.array<u64_const_4>(this, () => { return new devcb_write8(this); });
            m_out_dack_cb = new devcb_write_line.array<u64_const_4>(this, () => { return new devcb_write_line(this); });
        }


        public uint8_t read(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        public void write(offs_t offset, uint8_t data)
        {
            LOG("{0} \n", "write");
            if (BIT(offset, 3) == 0)
            {
                int channel = (int)((offset >> 1) & 0x03);

                switch (offset & 0x01)
                {
                case REGISTER_ADDRESS:
                    LOGSETUP(" * Register Address <- {0}\n", data);
                    if (m_msb != 0)
                    {
                        m_channel[channel].m_address = (uint16_t)((data << 8) | (m_channel[channel].m_address & 0xff));
                        if (MODE_AUTOLOAD && (channel == 2))
                            m_channel[3].m_address = (uint16_t)((data << 8) | (m_channel[3].m_address & 0xff));
                    }
                    else
                    {
                        m_channel[channel].m_address = (uint16_t)((m_channel[channel].m_address & 0xff00) | data);
                        if (MODE_AUTOLOAD && (channel == 2))
                            m_channel[3].m_address = (uint16_t)((m_channel[3].m_address & 0xff00) | data);
                    }
                    break;

                case REGISTER_WORD_COUNT:
                    LOGSETUP(" * Register Word Count <- {0}\n", data);
                    if (m_msb != 0)
                    {
                        m_channel[channel].m_count = (uint16_t)(((data & 0x3f) << 8) | (m_channel[channel].m_count & 0xff));
                        m_channel[channel].m_mode = (uint8_t)(data >> 6);

                        if (m_reverse_rw && m_channel[channel].m_mode != 0)
                            m_channel[channel].m_mode = (m_channel[channel].m_mode == 1) ? (uint8_t)2 : (uint8_t)1;

                        if (MODE_AUTOLOAD && (channel == 2))
                        {
                            m_channel[3].m_count = (uint16_t)(((data & 0x3f) << 8) | (m_channel[3].m_count & 0xff));
                            m_channel[3].m_mode = m_channel[2].m_mode;
                        }
                    }
                    else
                    {
                        m_channel[channel].m_count = (uint16_t)((m_channel[channel].m_count & 0xff00) | data);
                        if (MODE_AUTOLOAD && (channel == 2))
                            m_channel[3].m_count = (uint16_t)((m_channel[3].m_count & 0xff00) | data);
                    }
                    break;
                }

                m_msb = m_msb == 0 ? 1 : 0;
            }
            else if (offset == REGISTER_MODE)
            {
                m_transfer_mode = data;

                LOGSETUP("I8257 Command Register: {0}\n", m_transfer_mode);
            }

            if ((m_transfer_mode & m_request & 0x0f) != 0)
            {
                machine().scheduler().eat_all_cycles();
                trigger(1);
            }
        }


        public void hlda_w(int state)
        {
            LOG("I8257 Hold Acknowledge: {0}\n", state);

            m_hack = state;
            trigger(1);
        }


        //void ready_w(int state);


        public void dreq0_w(int state)
        {
            LOG("{0}\n", "dreq0_w");
            dma_request(0, state);
        }

        public void dreq1_w(int state)
        {
            LOG("{0}\n", "dreq1_w");
            dma_request(1, state);
        }

        //void dreq2_w(int state);
        //void dreq3_w(int state);


        public devcb_write_line.binder out_hrq_cb() { return m_out_hrq_cb.bind(); }
        //auto out_tc_cb() { return m_out_tc_cb.bind(); }
        public devcb_read8.binder in_memr_cb() { return m_in_memr_cb.bind(); }
        public devcb_write8.binder out_memw_cb() { return m_out_memw_cb.bind(); }
        public devcb_read8.binder in_ior_cb(int Ch) { return m_in_ior_cb[Ch].bind(); }
        public devcb_write8.binder out_iow_cb(int Ch) { return m_out_iow_cb[Ch].bind(); }
        //template <unsigned Ch> auto out_dack_cb() { return m_out_dack_cb[Ch].bind(); }

        // This should be set for systems that map the DMAC registers into the memory space rather than as I/O ports (e.g. radio86)
        public void set_reverse_rw_mode(bool flag) { m_reverse_rw = flag; }


        // device-level overrides
        protected override void device_start()
        {
            m_diexec = GetClassInterface<device_execute_interface_i8257>();


            LOG("{0}\n", "device_start"); //FUNCNAME);

            // set our instruction counter
            set_icountptr(m_icount);

            // resolve callbacks
            m_out_hrq_cb.resolve_safe();
            m_out_tc_cb.resolve_safe();
            m_in_memr_cb.resolve_safe_u8(0);
            m_out_memw_cb.resolve_safe();
            m_in_ior_cb.resolve_all_safe_u8(0);
            m_out_iow_cb.resolve_all_safe();
            m_out_dack_cb.resolve_all_safe();

            // state saving
            save_item(NAME(new { m_msb }));
            save_item(NAME(new { m_hreq }));
            save_item(NAME(new { m_hack }));
            save_item(NAME(new { m_ready }));
            save_item(NAME(new { m_state }));
            save_item(NAME(new { m_current_channel }));
            save_item(NAME(new { m_last_channel }));
            save_item(NAME(new { m_transfer_mode }));
            save_item(NAME(new { m_status }));
            save_item(NAME(new { m_request }));

            //throw new emu_unimplemented();
#if false
            save_item(STRUCT_MEMBER(m_channel, m_address));
            save_item(STRUCT_MEMBER(m_channel, m_count));
            save_item(STRUCT_MEMBER(m_channel, m_mode));
#endif
        }


        protected override void device_reset()
        {
            LOG("{0}\n", "device_reset");
            m_state = STATE_SI;
            m_transfer_mode = 0;
            m_status = 0;
            m_msb = 0;
            m_current_channel = -1;
            m_last_channel = 3;
            m_hreq = -1;
            m_tc = false;

            for (int i = 0; i < m_channel.Length; i++)
            {
                var elem = m_channel[i];
                elem.m_address = 0;
                elem.m_count = 0;
                elem.m_mode = 0;
            }

            set_hreq(0);
            set_dack();
        }


        // device_execute_interface helpers
        public void set_icountptr(intref icount) { execute().set_icountptr(icount); }
        public void suspend_until_trigger(int trigid, bool eatcycles) { execute().suspend_until_trigger(trigid, eatcycles); }
        public void trigger(int trigid) { execute().trigger(trigid); }


        // device_execute_interface overrides
        void device_execute_interface_execute_run()
        {
            LOG("{0}\n", "execute_run");

            do
            {
                switch (m_state)
                {
                case STATE_SI:
                    set_tc(0);
                    if (next_channel())
                    {
                        m_state = STATE_S0;
                    }
                    else
                    {
                        m_icount.i = 0;
                        suspend_until_trigger(1, true);
                    }
                    break;

                case STATE_S0:
                    set_hreq(1);

                    if (m_hack != 0)
                    {
                        m_state = STATE_S1;
                    }
                    else
                    {
                        m_icount.i = 0;
                        suspend_until_trigger(1, true);
                    }
                    break;

                case STATE_S1:
                    set_tc(0);
                    m_state = STATE_S2;
                    break;

                case STATE_S2:
                    set_dack();
                    m_state = STATE_S3;
                    break;

                case STATE_S3:
                    dma_read();

                    if (MODE_EXTENDED_WRITE)
                    {
                        dma_write();
                    }

                    if (m_ready != 0)
                    {
                        m_state = STATE_S4;
                        if ((m_channel[m_current_channel].m_count == 0) && (MODE_TRANSFER_MASK != MODE_TRANSFER_READ))
                            set_tc(1);
                    }
                    else
                    {
                        m_state = STATE_SW;
                    }
                    break;

                case STATE_SW:
                    if (m_ready != 0)
                    {
                        m_state = STATE_S4;
                        if ((m_channel[m_current_channel].m_count == 0) && (MODE_TRANSFER_MASK != MODE_TRANSFER_READ))
                            set_tc(1);
                    }
                    break;

                case STATE_S4:
                    if (!MODE_EXTENDED_WRITE)
                    {
                        dma_write();
                    }

                    if ((m_channel[m_current_channel].m_count == 0) && (MODE_TRANSFER_MASK == MODE_TRANSFER_READ))
                        set_tc(1);

                    advance();

                    if (m_hack != 0 && next_channel())
                    {
                        m_state = STATE_S1;
                    }
                    else
                    {
                        set_hreq(0);
                        m_current_channel = -1;
                        m_state = STATE_SI;
                        set_dack();
                    }
                    break;
                }

                m_icount.i--;

            } while (m_icount.i > 0);
        }


        void dma_request(int channel, int state)
        {
            LOG("I8257 Channel {0} DMA Request: {1}\n", channel, state);
            LOG("I8257 Channel {0} DMA Request: {2} ({3}abled)\n", channel, state, MODE_CHAN_ENABLE(channel) ? "en" : "dis");

            if (state != 0)
                m_request |= (uint8_t)(1 << channel);
            else
                m_request &= (uint8_t)(~(1 << channel));

            trigger(1);
        }


        bool is_request_active(int channel)
        {
            LOG("{0} Channel {1}: {2} && MODE_CHAN_ENABLE:{3}\n", "is_request_active", channel, m_request, MODE_CHAN_ENABLE(channel));
            return BIT(m_request, channel) != 0 && MODE_CHAN_ENABLE(channel);
        }


        void set_hreq(int state)
        {
            LOG("{0}\n", "set_hreq");
            if (m_hreq != state)
            {
                m_out_hrq_cb.op_s32(state);
                m_hreq = state;
                m_diexec.abort_timeslice();
            }
        }


        void set_dack()
        {
            LOG("{0}\n", "set_dack");
            for (int ch = 0; ch < 4; ch++)
                m_out_dack_cb[ch].op_s32(m_current_channel != ch ? 1 : 0);
        }


        void dma_read()
        {
            LOG("{0}\n", "dma_read");
            offs_t offset = m_channel[m_current_channel].m_address;

            switch (MODE_TRANSFER_MASK)
            {
            case MODE_TRANSFER_VERIFY:
            case MODE_TRANSFER_WRITE:
                LOGTFR(" - MODE TRANSFER VERIFY/WRITE");
                m_temp = m_in_ior_cb[m_current_channel].op_u8(offset);
                break;

            case MODE_TRANSFER_READ:
                LOGTFR(" - MODE TRANSFER READ");
                m_temp = m_in_memr_cb.op_u8(offset);
                break;
            }
            LOGTFR(" channel {0} Offset {1}: {2}\n", m_current_channel, offset, m_temp);
        }


        void dma_write()
        {
            LOG("{0}\n", "dma_write");
            offs_t offset = m_channel[m_current_channel].m_address;

            switch (MODE_TRANSFER_MASK)
            {
            case MODE_TRANSFER_VERIFY:
            {
                LOGTFR(" - MODE TRANSFER VERIFY");
                uint8_t v1 = m_in_memr_cb.op_u8(offset);
                if (false && m_temp != v1)
                    logerror("verify error {0} vs. {1}\n", m_temp, v1);
                break;
            }

            case MODE_TRANSFER_WRITE:
                LOGTFR(" - MODE TRANSFER WRITE");
                m_out_memw_cb.op_u8(offset, m_temp);
                break;

            case MODE_TRANSFER_READ:
                LOGTFR(" - MODE TRANSFER READ");
                m_out_iow_cb[m_current_channel].op_u8(offset, m_temp);
                break;
            }

            LOGTFR(" channel {0} Offset {1}: {2} {3}\n", m_current_channel, offset, m_temp, m_channel[m_current_channel].m_count);
        }


        void advance()
        {
            LOG("{0}\n", "advance");
            bool tc = m_tc;
            bool al = (MODE_AUTOLOAD && (m_current_channel == 2));

            set_tc(0);
            if (tc)
            {
                m_status |= (uint8_t)(1 << m_current_channel);

                if (al)
                {
                    // autoinitialize
                    m_channel[2].m_address = m_channel[3].m_address;
                    m_channel[2].m_count = m_channel[3].m_count;
                    m_channel[2].m_mode = m_channel[3].m_mode;
                }
                else if (MODE_TC_STOP)
                {
                    // disable channel
                    m_transfer_mode &= (uint8_t)(~(1 << m_current_channel));
                }
            }

            if (!(al && tc))
            {
                m_channel[m_current_channel].m_count--;
                m_channel[m_current_channel].m_count &= 0x3fff;
                m_channel[m_current_channel].m_address++;
            }
        }


        void set_tc(int state)
        {
            LOG("{0}: {1}\n", "set_tc", state);
            if (m_tc != (state != 0))
            {
                m_out_tc_cb.op_s32(state);

                m_tc = state != 0;
            }
        }


        bool next_channel()
        {
            LOG("{0}\n", "next_channel");
            int [] priority = new int [] { 0, 1, 2, 3 };

            if (MODE_ROTATING_PRIORITY)
            {
                int last_channel = m_last_channel;

                for (int channel = 3; channel >= 0; channel--)
                {
                    priority[channel] = last_channel;
                    last_channel--;
                    if (last_channel < 0) last_channel = 3;
                }
            }

            foreach (var elem in priority)
            {
                if (is_request_active(elem))
                {
                    m_current_channel = m_last_channel = elem;
                    return true;
                }
            }

            return false;
        }
    }


    public static class i8257_global
    {
        public static i8257_device I8257<bool_Required>(machine_config mconfig, device_finder<i8257_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, i8257_device.I8257, clock); }
    }
}
