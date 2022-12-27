// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.z80dma_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> z80dma_device
    class z80dma_device : device_t
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

        //emu_timer *m_timer;

        //uint16_t  m_regs[(6<<3)+1+1];
        //uint8_t   m_num_follow;
        //uint8_t   m_cur_follow;
        //uint8_t   m_regs_follow[5];
        //uint8_t   m_read_num_follow;
        //uint8_t   m_read_cur_follow;
        //uint8_t   m_read_regs_follow[7];
        //uint8_t   m_status;
        //uint8_t   m_dma_enabled;

        //uint16_t m_addressA;
        //uint16_t m_addressB;
        //uint16_t m_count;

        //int m_rdy;
        //int m_force_ready;
        //uint8_t m_reset_pointer;

        //bool m_is_read;
        //uint8_t m_cur_cycle;
        //uint8_t m_latch;

        // interrupts
        //bool m_iei;
        //int m_ip;                   // interrupt pending
        //int m_ius;                  // interrupt under service
        //uint8_t m_vector;             // interrupt vector


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

        //uint8_t read();
        //void write(uint8_t data);

        //DECLARE_WRITE_LINE_MEMBER(iei_w) { m_iei = state; interrupt_check(); }
        //DECLARE_WRITE_LINE_MEMBER(rdy_w);
        //DECLARE_WRITE_LINE_MEMBER(wait_w);
        //DECLARE_WRITE_LINE_MEMBER(bai_w);


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_reset() { throw new emu_unimplemented(); }


        // device_z80daisy_interface overrides
        protected virtual int device_z80daisy_interface_z80daisy_irq_state() { throw new emu_unimplemented(); }
        protected virtual int device_z80daisy_interface_z80daisy_irq_ack() { throw new emu_unimplemented(); }
        protected virtual void device_z80daisy_interface_z80daisy_irq_reti() { throw new emu_unimplemented(); }


        // internal helpers
        //int is_ready();
        //void interrupt_check();
        //void trigger_interrupt(int level);
        //void do_read();
        //int do_write();
        //void do_transfer_write();
        //void do_search();

        //TIMER_CALLBACK_MEMBER(timerproc);

        //void update_status();

        //TIMER_CALLBACK_MEMBER(rdy_write_callback);
    }


    static class z80dma_global
    {
        public static z80dma_device Z80DMA<bool_Required>(machine_config mconfig, device_finder<z80dma_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, z80dma_device.Z80DMA, clock); }
    }
}
