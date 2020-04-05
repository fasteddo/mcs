// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    public class i8257_device : device_t
                                //device_execute_interface
    {
        //DEFINE_DEVICE_TYPE(I8257, i8257_device, "i8257", "Intel 8257 DMA Controller")
        static device_t device_creator_i8257_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new i8257_device(mconfig, tag, owner, clock); }
        public static readonly device_type I8257 = DEFINE_DEVICE_TYPE(device_creator_i8257_device, "i8257", "Intel 8257 DMA Controller");


        class device_execute_interface_i8257 : device_execute_interface
        {
            public device_execute_interface_i8257(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void execute_run() { ((i8257_device)device()).device_execute_interface_execute_run(); }
        }


        const int LOG_SETUP     = 1 << 1;
        const int LOG_TFR       = 1 << 2;

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

        devcb_write_line   m_out_hrq_cb;
        devcb_write_line   m_out_tc_cb;

        /* accessors to main memory */
        devcb_read8        m_in_memr_cb;
        devcb_write8       m_out_memw_cb;

        /* channel accessors */
        devcb_read8        [] m_in_ior_cb = new devcb_read8 [4];
        devcb_write8       [] m_out_iow_cb = new devcb_write8 [4];
        devcb_write_line   [] m_out_dack_cb = new devcb_write_line [4];

        struct channel
        {
            public uint16_t m_address;
            public uint16_t m_count;
            public uint8_t m_mode;
        }
        channel [] m_channel = new channel [4];


        intref m_icountRef = new intref();  //int m_icount;


        // construction/destruction
        i8257_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, I8257, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_i8257(mconfig, this));  //device_execute_interface(mconfig, *this),


            m_icountRef.i = 0;
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
            for (int i = 0; i < 4; i++)
                m_in_ior_cb[i] = new devcb_read8(this);
            for (int i = 0; i < 4; i++)
                m_out_iow_cb[i] = new devcb_write8(this);
            for (int i = 0; i < 4; i++)
                m_out_dack_cb[i] = new devcb_write_line(this);
        }


        //READ8_MEMBER( i8257_device::read )
        public u8 read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
        }


        //WRITE8_MEMBER( i8257_device::write )
        public void write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
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

            trigger(1);
        }


        //WRITE_LINE_MEMBER( i8257_device::hlda_w )
        public void hlda_w(int state)
        {
            LOG("I8257 Hold Acknowledge: {0}\n", state);

            m_hack = state;
            trigger(1);
        }


        //DECLARE_WRITE_LINE_MEMBER( ready_w );


        //WRITE_LINE_MEMBER( i8257_device::dreq0_w )
        public void dreq0_w(int state)
        {
            LOG("{0}\n", "dreq0_w");
            dma_request(0, state);
        }

        //WRITE_LINE_MEMBER( i8257_device::dreq1_w )
        public void dreq1_w(int state)
        {
            LOG("{0}\n", "dreq1_w");
            dma_request(1, state);
        }

        //DECLARE_WRITE_LINE_MEMBER( dreq2_w );
        //DECLARE_WRITE_LINE_MEMBER( dreq3_w );


        public devcb_write.binder out_hrq_cb() { return m_out_hrq_cb.bind(); }
        //auto out_tc_cb() { return m_out_tc_cb.bind(); }
        public devcb_read.binder in_memr_cb() { return m_in_memr_cb.bind(); }
        public devcb_write.binder out_memw_cb() { return m_out_memw_cb.bind(); }
        public devcb_read.binder in_ior_cb(int Ch) { return m_in_ior_cb[Ch].bind(); }
        public devcb_write.binder out_iow_cb(int Ch) { return m_out_iow_cb[Ch].bind(); }
        //template <unsigned Ch> auto out_dack_cb() { return m_out_dack_cb[Ch].bind(); }

        // This should be set for systems that map the DMAC registers into the memory space rather than as I/O ports (e.g. radio86)
        public void set_reverse_rw_mode(bool flag) { m_reverse_rw = flag; }


        // device-level overrides
        protected override void device_start()
        {
            m_diexec = GetClassInterface<device_execute_interface_i8257>();


            LOG("{0}\n", "device_start"); //FUNCNAME);

            // set our instruction counter
            set_icountptr(m_icountRef);

            // resolve callbacks
            m_out_hrq_cb.resolve_safe();
            m_out_tc_cb.resolve_safe();
            m_in_memr_cb.resolve_safe(0);
            m_out_memw_cb.resolve_safe();
            foreach (var cb in m_in_ior_cb)
                cb.resolve_safe(0);
            foreach (var cb in m_out_iow_cb)
                cb.resolve_safe();
            foreach (var cb in m_out_dack_cb)
                cb.resolve_safe();

            // state saving
            save_item(m_msb, "m_msb");
            save_item(m_hreq, "m_hreq");
            save_item(m_hack, "m_hack");
            save_item(m_ready, "m_ready");
            save_item(m_state, "m_state");
            save_item(m_current_channel, "m_current_channel");
            save_item(m_last_channel, "m_last_channel");
            save_item(m_transfer_mode, "m_transfer_mode");
            save_item(m_status, "m_status");
            save_item(m_request, "m_request");

            save_item(m_channel[0].m_address, "m_channel[0].m_address");
            save_item(m_channel[0].m_count, "m_channel[0].m_count");
            save_item(m_channel[0].m_mode, "m_channel[0].m_mode");
            save_item(m_channel[1].m_address, "m_channel[1].m_address");
            save_item(m_channel[1].m_count, "m_channel[1].m_count");
            save_item(m_channel[1].m_mode, "m_channel[1].m_mode");
            save_item(m_channel[2].m_address, "m_channel[2].m_address");
            save_item(m_channel[2].m_count, "m_channel[2].m_count");
            save_item(m_channel[2].m_mode, "m_channel[2].m_mode");
            save_item(m_channel[3].m_address, "m_channel[3].m_address");
            save_item(m_channel[3].m_count, "m_channel[3].m_count");
            save_item(m_channel[3].m_mode, "m_channel[3].m_mode");
        }


        protected override void device_reset()
        {
            LOG("{0}\n", "device_reset");
            m_state = STATE_SI;
            m_transfer_mode = 0;
            m_status = 0;
            m_request = 0;
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
                        suspend_until_trigger(1, true);
                        m_icountRef.i = 0;
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
                        suspend_until_trigger(1, true);
                        m_icountRef.i = 0;
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

                    m_state = m_ready != 0 ? STATE_S4 : STATE_SW;
                    break;

                case STATE_SW:
                    m_state = m_ready != 0 ? STATE_S4 : STATE_SW;
                    break;

                case STATE_S4:
                    if (!MODE_EXTENDED_WRITE)
                    {
                        dma_write();
                    }

                    advance();

                    if (next_channel())
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

                m_icountRef.i--;

            } while (m_icountRef.i > 0);
        }


        void dma_request(int channel, int state)
        {
            LOG("I8257 Channel {0} DMA Request: {1}\n", channel, state);

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
                m_out_hrq_cb.op(state);
                m_hreq = state;
            }
        }


        void set_dack()
        {
            LOG("{0}\n", "set_dack");
            for (int ch = 0; ch < 4; ch++)
                m_out_dack_cb[ch].op(m_current_channel != ch ? 1 : 0);
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
                m_temp = m_in_ior_cb[m_current_channel].op(offset);
                break;

            case MODE_TRANSFER_READ:
                LOGTFR(" - MODE TRANSFER READ");
                m_temp = m_in_memr_cb.op(offset);
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
                uint8_t v1 = m_in_memr_cb.op(offset);
                if (false && m_temp != v1)
                    logerror("verify error {0} vs. {1}\n", m_temp, v1);
                break;
            }

            case MODE_TRANSFER_WRITE:
                LOGTFR(" - MODE TRANSFER WRITE");
                m_out_memw_cb.op(offset, m_temp);
                break;

            case MODE_TRANSFER_READ:
                LOGTFR(" - MODE TRANSFER READ");
                m_out_iow_cb[m_current_channel].op(offset, m_temp);
                break;
            }

            LOGTFR(" channel {0} Offset {1}: {2} {3}\n", m_current_channel, offset, m_temp, m_channel[m_current_channel].m_count);
        }


        void advance()
        {
            LOG("{0}\n", "advance");
            bool tc = (m_channel[m_current_channel].m_count == 0);
            bool al = (MODE_AUTOLOAD && (m_current_channel == 2));

            if (tc)
            {
                m_status |= (uint8_t)(1 << m_current_channel);
                set_tc(1);

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
                m_out_tc_cb.op(state);

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
}
