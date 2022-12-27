// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using s32 = System.Int32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.diexec_global;
using static mame.emucore_global;
using static mame.z80dma_global;
using static mame.z80dma_internal;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> z80dma_device
    public partial class z80dma_device : device_t
                                         //device_z80daisy_interface
    {
        //DEFINE_DEVICE_TYPE(Z80DMA, z80dma_device, "z80dma", "Z80 DMA Controller")
        public static readonly emu.detail.device_type_impl Z80DMA = DEFINE_DEVICE_TYPE("z80dma", "Z80 DMA Controller", (type, mconfig, tag, owner, clock) => { return new z80dma_device(mconfig, tag, owner, clock); });


        public class device_z80daisy_interface_z80dma_device : device_z80daisy_interface
        {
            public device_z80daisy_interface_z80dma_device(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override int z80daisy_irq_state() { return ((z80dma_device)device()).device_z80daisy_interface_z80daisy_irq_state(); }
            public override int z80daisy_irq_ack() { return ((z80dma_device)device()).device_z80daisy_interface_z80daisy_irq_ack(); }
            public override void z80daisy_irq_reti() { ((z80dma_device)device()).device_z80daisy_interface_z80daisy_irq_reti(); }
        }


        // internal state
        devcb_write_line m_out_busreq_cb;
        devcb_write_line m_out_int_cb;
        devcb_write_line m_out_ieo_cb;
        devcb_write_line m_out_bao_cb;
        devcb_read8 m_in_mreq_cb;
        devcb_write8 m_out_mreq_cb;
        devcb_read8 m_in_iorq_cb;
        devcb_write8 m_out_iorq_cb;

        emu_timer m_timer;

        uint16_t [] m_regs = new uint16_t [(6<<3)+1+1];
        uint8_t m_num_follow;
        uint8_t m_cur_follow;
        uint8_t [] m_regs_follow = new uint8_t [5];
        uint8_t m_read_num_follow;
        uint8_t m_read_cur_follow;
        //uint8_t   m_read_regs_follow[7];
        uint8_t m_status;
        uint8_t m_dma_enabled;

        uint16_t m_addressA;
        uint16_t m_addressB;
        uint16_t m_count;

        int m_rdy;
        int m_force_ready;
        uint8_t m_reset_pointer;

        bool m_is_read;
        uint8_t m_cur_cycle;
        uint8_t m_latch;

        // interrupts
        bool m_iei;
        int m_ip;                   // interrupt pending
        int m_ius;                  // interrupt under service
        uint8_t m_vector;             // interrupt vector


        // construction/destruction
        z80dma_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, Z80DMA, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_z80daisy_interface_z80dma_device(mconfig, this));  //device_z80daisy_interface(mconfig, *this)
            //m_z80daisy = GetClassInterface<device_z80daisy_interface_z80dma_device>();


            m_out_busreq_cb = new devcb_write_line(this);
            m_out_int_cb = new devcb_write_line(this);
            m_out_ieo_cb = new devcb_write_line(this);
            m_out_bao_cb = new devcb_write_line(this);
            m_in_mreq_cb = new devcb_read8(this);
            m_out_mreq_cb = new devcb_write8(this);
            m_in_iorq_cb = new devcb_read8(this);
            m_out_iorq_cb = new devcb_write8(this);
        }


        public devcb_write_line.binder out_busreq_callback() { return m_out_busreq_cb.bind(); }  //auto out_busreq_callback() { return m_out_busreq_cb.bind(); }
        //auto out_int_callback() { return m_out_int_cb.bind(); }
        //auto out_ieo_callback() { return m_out_ieo_cb.bind(); }
        //auto out_bao_callback() { return m_out_bao_cb.bind(); }
        public devcb_read8.binder in_mreq_callback() { return m_in_mreq_cb.bind(); }  //auto in_mreq_callback() { return m_in_mreq_cb.bind(); }
        public devcb_write8.binder out_mreq_callback() { return m_out_mreq_cb.bind(); }  //auto out_mreq_callback() { return m_out_mreq_cb.bind(); }
        //auto in_iorq_callback() { return m_in_iorq_cb.bind(); }
        //auto out_iorq_callback() { return m_out_iorq_cb.bind(); }

        public uint8_t read() { throw new emu_unimplemented(); }
        public void write(uint8_t data) { throw new emu_unimplemented(); }

        //DECLARE_WRITE_LINE_MEMBER(iei_w) { m_iei = state; interrupt_check(); }
        //DECLARE_WRITE_LINE_MEMBER(rdy_w);
        //DECLARE_WRITE_LINE_MEMBER(wait_w);
        //DECLARE_WRITE_LINE_MEMBER(bai_w);


        // device-level overrides
        protected override void device_start()
        {
            // resolve callbacks
            m_out_busreq_cb.resolve_safe();
            m_out_int_cb.resolve_safe();
            m_out_ieo_cb.resolve_safe();
            m_out_bao_cb.resolve_safe();
            m_in_mreq_cb.resolve_safe_u8(0);
            m_out_mreq_cb.resolve_safe();
            m_in_iorq_cb.resolve_safe_u8(0);
            m_out_iorq_cb.resolve_safe();

            // allocate timer
            m_timer = machine().scheduler().timer_alloc(timerproc);

            // register for state saving
            save_item(NAME(new { m_regs }));
            save_item(NAME(new { m_regs_follow }));
            save_item(NAME(new { m_num_follow }));
            save_item(NAME(new { m_cur_follow }));
            save_item(NAME(new { m_status }));
            save_item(NAME(new { m_dma_enabled }));
            save_item(NAME(new { m_vector }));
            save_item(NAME(new { m_iei }));
            save_item(NAME(new { m_ip }));
            save_item(NAME(new { m_ius }));
            save_item(NAME(new { m_addressA }));
            save_item(NAME(new { m_addressB }));
            save_item(NAME(new { m_count }));
            save_item(NAME(new { m_rdy }));
            save_item(NAME(new { m_force_ready }));
            save_item(NAME(new { m_is_read }));
            save_item(NAME(new { m_cur_cycle }));
            save_item(NAME(new { m_latch }));
        }


        protected override void device_reset()
        {
            m_status = 0;
            m_rdy = 0;
            m_force_ready = 0;
            m_num_follow = 0;
            m_dma_enabled = 0;
            m_read_num_follow = m_read_cur_follow = 0;
            m_reset_pointer = 0;
            m_is_read = false;
            std.memset(m_regs, (uint16_t)0);
            std.memset(m_regs_follow, (uint8_t)0);

            // disable interrupts
            WR3 &= unchecked((uint16_t)~0x20);
            m_ip = 0;
            m_ius = 0;
            m_vector = 0;

            update_status();
        }


        // device_z80daisy_interface overrides
        protected virtual int device_z80daisy_interface_z80daisy_irq_state() { throw new emu_unimplemented(); }
        protected virtual int device_z80daisy_interface_z80daisy_irq_ack() { throw new emu_unimplemented(); }
        protected virtual void device_z80daisy_interface_z80daisy_irq_reti() { throw new emu_unimplemented(); }


        // internal helpers
        int is_ready()
        {
            return ((m_force_ready != 0) || (m_rdy == READY_ACTIVE_HIGH)) ? 1 : 0;
        }


        //void interrupt_check();
        //void trigger_interrupt(int level);
        //void do_read();
        //int do_write();
        //void do_transfer_write();
        //void do_search();


        //TIMER_CALLBACK_MEMBER(timerproc);
        void timerproc(object ptr, s32 param)  //void *ptr, s32 param)
        {
            throw new emu_unimplemented();
        }


        void update_status()
        {
            uint16_t pending_transfer;
            attotime next;

            // no transfer is active right now; is there a transfer pending right now?
            pending_transfer = (uint16_t)(is_ready() & m_dma_enabled);

            if (pending_transfer != 0)
            {
                m_is_read = true;
                m_cur_cycle = (PORTA_IS_SOURCE != 0 ? (uint8_t)PORTA_CYCLE_LEN : (uint8_t)PORTB_CYCLE_LEN);
                next = attotime.from_hz(clock());
                m_timer.adjust(
                    attotime.zero,
                    0,
                    // 1 byte transferred in 4 clock cycles
                    next);
            }
            else
            {
                if (m_is_read)
                {
                    // no transfers active right now
                    m_timer.reset();
                }
            }

            // set the busreq line
            m_out_busreq_cb.op_s32(pending_transfer != 0 ? ASSERT_LINE : CLEAR_LINE);
        }


        //TIMER_CALLBACK_MEMBER(rdy_write_callback);
    }


    static class z80dma_internal
    {
        //enum
        //{
        //    INT_RDY = 0,
        //    INT_MATCH,
        //    INT_END_OF_BLOCK,
        //    INT_MATCH_END_OF_BLOCK
        //};

        //constexpr int COMMAND_RESET                         = 0xc3;
        //constexpr int COMMAND_RESET_PORT_A_TIMING           = 0xc7;
        //constexpr int COMMAND_RESET_PORT_B_TIMING           = 0xcb;
        //constexpr int COMMAND_LOAD                          = 0xcf;
        //constexpr int COMMAND_CONTINUE                      = 0xd3;
        //constexpr int COMMAND_DISABLE_INTERRUPTS            = 0xaf;
        //constexpr int COMMAND_ENABLE_INTERRUPTS             = 0xab;
        //constexpr int COMMAND_RESET_AND_DISABLE_INTERRUPTS  = 0xa3;
        //constexpr int COMMAND_ENABLE_AFTER_RETI             = 0xb7;
        //constexpr int COMMAND_READ_STATUS_BYTE              = 0xbf;
        //constexpr int COMMAND_REINITIALIZE_STATUS_BYTE      = 0x8b;
        //constexpr int COMMAND_INITIATE_READ_SEQUENCE        = 0xa7;
        //constexpr int COMMAND_FORCE_READY                   = 0xb3;
        //constexpr int COMMAND_ENABLE_DMA                    = 0x87;
        //constexpr int COMMAND_DISABLE_DMA                   = 0x83;
        //constexpr int COMMAND_READ_MASK_FOLLOWS             = 0xbb;

        //constexpr int TM_TRANSFER           = 0x01;
        //constexpr int TM_SEARCH             = 0x02;
        //constexpr int TM_SEARCH_TRANSFER    = 0x03;
    }


    public partial class z80dma_device : device_t
                                         //device_z80daisy_interface
    {
        //**************************************************************************
        //  MACROS
        //**************************************************************************

        int REGNUM(int _m, int _s) { return (_m << 3) + _s; }
        //#define GET_REGNUM(_r)          (&(_r) - &(WR0))
        ref uint16_t REG(int _m, int _s) { return ref m_regs[REGNUM(_m,_s)]; }
        uint16_t WR0 { get { return REG(0, 0); } }
        //#define WR1                     REG(1, 0)
        //#define WR2                     REG(2, 0)
        uint16_t WR3 { get { return REG(3, 0); } set { REG(3, 0) = value; } }
        //#define WR4                     REG(4, 0)
        uint16_t WR5 { get { return REG(5, 0); } }
        //#define WR6                     REG(6, 0)

        //#define PORTA_ADDRESS_L         REG(0,1)
        //#define PORTA_ADDRESS_H         REG(0,2)

        //#define BLOCKLEN_L              REG(0,3)
        //#define BLOCKLEN_H              REG(0,4)

        uint16_t PORTA_TIMING { get { return REG(1,1); } }
        uint16_t PORTB_TIMING { get { return REG(2,1); } }

        //#define MASK_BYTE               REG(3,1)
        //#define MATCH_BYTE              REG(3,2)

        //#define PORTB_ADDRESS_L         REG(4,1)
        //#define PORTB_ADDRESS_H         REG(4,2)
        //#define INTERRUPT_CTRL          REG(4,3)
        //#define INTERRUPT_VECTOR        REG(4,4)
        //#define PULSE_CTRL              REG(4,5)

        //#define READ_MASK               REG(6,1)

        //#define PORTA_ADDRESS           ((PORTA_ADDRESS_H<<8) | PORTA_ADDRESS_L)
        //#define PORTB_ADDRESS           ((PORTB_ADDRESS_H<<8) | PORTB_ADDRESS_L)
        //#define BLOCKLEN                ((BLOCKLEN_H<<8) | BLOCKLEN_L)

        //#define PORTA_INC               (WR1 & 0x10)
        //#define PORTB_INC               (WR2 & 0x10)
        //#define PORTA_FIXED             (((WR1 >> 4) & 0x02) == 0x02)
        //#define PORTB_FIXED             (((WR2 >> 4) & 0x02) == 0x02)
        //#define PORTA_MEMORY            (((WR1 >> 3) & 0x01) == 0x00)
        //#define PORTB_MEMORY            (((WR2 >> 3) & 0x01) == 0x00)

        uint16_t PORTA_CYCLE_LEN { get { return (uint16_t)(4 - (PORTA_TIMING & 0x03)); } }
        uint16_t PORTB_CYCLE_LEN { get { return (uint16_t)(4 - (PORTB_TIMING & 0x03)); } }

        uint16_t PORTA_IS_SOURCE { get { return (uint16_t)((WR0 >> 2) & 0x01); } }
        //#define PORTB_IS_SOURCE         (!PORTA_IS_SOURCE)
        //#define TRANSFER_MODE           (WR0 & 0x03)

        //#define MATCH_F_SET             (m_status &= ~0x10)
        //#define MATCH_F_CLEAR           (m_status |= 0x10)
        //#define EOB_F_SET               (m_status &= ~0x20)
        //#define EOB_F_CLEAR             (m_status |= 0x20)

        uint16_t READY_ACTIVE_HIGH { get { return (uint16_t)((WR5 >> 3) & 0x01); } }
        //#define AUTO_RESTART            ((WR5>>5) & 0x01)

        //#define INTERRUPT_ENABLE        (WR3 & 0x20)
        //#define INT_ON_MATCH            (INTERRUPT_CTRL & 0x01)
        //#define INT_ON_END_OF_BLOCK     (INTERRUPT_CTRL & 0x02)
        //#define INT_ON_READY            (INTERRUPT_CTRL & 0x40)
        //#define STATUS_AFFECTS_VECTOR   (INTERRUPT_CTRL & 0x20)
    }


    static class z80dma_global
    {
        public static z80dma_device Z80DMA<bool_Required>(machine_config mconfig, device_finder<z80dma_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, z80dma_device.Z80DMA, clock); }
    }
}
