// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;

using static mame.device_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> z80pio_device
    public class z80pio_device : device_t
                                 //device_z80daisy_interface
    {
        //DEFINE_DEVICE_TYPE(Z80PIO, z80pio_device, "z80pio", "Z80 PIO")
        public static readonly emu.detail.device_type_impl Z80PIO = DEFINE_DEVICE_TYPE("z80pio", "Z80 PIO", (type, mconfig, tag, owner, clock) => { return new z80pio_device(mconfig, tag, owner, clock); });


        //enum
        //{
        //    PORT_A = 0,
        //    PORT_B,
        //    PORT_COUNT
        //};


        //enum
        //{
        //    MODE_OUTPUT = 0,
        //    MODE_INPUT,
        //    MODE_BIDIRECTIONAL,
        //    MODE_BIT_CONTROL
        //};

        //enum
        //{
        //    ANY = 0,
        //    IOR,
        //    MASK
        //};

        //enum
        //{
        //    ICW_ENABLE_INT    = 0x80,
        //    ICW_AND_OR        = 0x40,
        //    ICW_AND           = 0x40,
        //    ICW_OR            = 0x00,
        //    ICW_HIGH_LOW      = 0x20,
        //    ICW_HIGH          = 0x20,
        //    ICW_LOW           = 0x00,
        //    ICW_MASK_FOLLOWS  = 0x10
        //};


        // a single PIO port
        class pio_port
        {
            //friend class z80pio_device;


            //z80pio_device *             m_device;
            //int                         m_index;

            //int m_mode;                 // mode register
            //int m_next_control_word;    // next control word
            //uint8_t m_input;              // input latch
            //uint8_t m_output;             // output latch
            //uint8_t m_ior;                // input/output register
            //bool m_rdy;                 // ready
            //bool m_stb;                 // strobe

            // interrupts
            //bool m_ie;                  // interrupt enabled
            //bool m_ip;                  // interrupt pending
            //bool m_ius;                 // interrupt under service
            //uint8_t m_icw;                // interrupt control word
            //uint8_t m_vector;             // interrupt vector
            //uint8_t m_mask;               // interrupt mask
            //bool m_match;               // logic equation match


            pio_port()
            {
                throw new emu_unimplemented();
#if false
                m_device(nullptr),
                m_index(0),
                m_mode(0),
                m_next_control_word(0),
                m_input(0),
                m_output(0),
                m_ior(0),
                m_rdy(false),
                m_stb(false),
                m_ie(false),
                m_ip(false),
                m_ius(false),
                m_icw(0),
                m_vector(0),
                m_mask(0),
                m_match(false)
#endif
            }


            //void start(z80pio_device *device, int index);
            //void reset();

            //void trigger_interrupt();

            //int rdy() const { return m_rdy; }
            //void set_rdy(bool state);
            //void set_mode(int mode);
            //void strobe(bool state);

            //uint8_t read();
            //void write(uint8_t data);

            //void control_write(uint8_t data);

            //uint8_t data_read();
            //void data_write(uint8_t data);


            //void check_interrupts() { m_device->check_interrupts(); }
        }


        // internal state
        //pio_port             m_port[2];
        //devcb_write_line    m_out_int_cb;

        //devcb_read8         m_in_pa_cb;
        //devcb_write8        m_out_pa_cb;
        //devcb_write_line    m_out_ardy_cb;

        //devcb_read8         m_in_pb_cb;
        //devcb_write8        m_out_pb_cb;
        //devcb_write_line    m_out_brdy_cb;


        // construction/destruction
        z80pio_device(machine_config mconfig, string tag, device_t owner, uint32_t clock) :
            base(mconfig, Z80PIO, tag, owner, clock)
        {
            throw new emu_unimplemented();
#if false
            device_z80daisy_interface_z80pio  //device_z80daisy_interface(mconfig, *this),


            m_out_int_cb(*this),
            m_in_pa_cb(*this),
            m_out_pa_cb(*this),
            m_out_ardy_cb(*this),
            m_in_pb_cb(*this),
            m_out_pb_cb(*this),
            m_out_brdy_cb(*this)
#endif
            }


        //auto out_int_callback() { return m_out_int_cb.bind(); }
        //auto in_pa_callback() { return m_in_pa_cb.bind(); }
        //auto out_pa_callback() { return m_out_pa_cb.bind(); }
        //auto out_ardy_callback() { return m_out_ardy_cb.bind(); }
        //auto in_pb_callback() { return m_in_pb_cb.bind(); }
        //auto out_pb_callback() { return m_out_pb_cb.bind(); }
        //auto out_brdy_callback() { return m_out_brdy_cb.bind(); }


        // I/O line access
        //int rdy(int which) { return m_port[which].rdy(); }
        //void strobe(int which, bool state) { m_port[which].strobe(state); }
        //DECLARE_READ_LINE_MEMBER( rdy_a ) { return rdy(PORT_A); }
        //DECLARE_READ_LINE_MEMBER( rdy_b ) { return rdy(PORT_B); }
        //DECLARE_WRITE_LINE_MEMBER( strobe_a ) { strobe(PORT_A, state); }
        //DECLARE_WRITE_LINE_MEMBER( strobe_b ) { strobe(PORT_B, state); }

        // control register I/O
        //uint8_t control_read();
        //void control_write(int offset, uint8_t data) { m_port[offset & 1].control_write(data); }
        //void control_a_write(uint8_t data) { control_write(PORT_A, data); }
        //void control_b_write(uint8_t data) { control_write(PORT_B, data); }

        // data register I/O
        //uint8_t data_read(int offset) { return m_port[offset & 1].data_read(); }
        //void data_write(int offset, uint8_t data) { m_port[offset & 1].data_write(data); }
        //uint8_t data_a_read() { return data_read(PORT_A); }
        //uint8_t data_b_read() { return data_read(PORT_B); }
        //void data_a_write(uint8_t data) { data_write(PORT_A, data); }
        //void data_b_write(uint8_t data) { data_write(PORT_B, data); }

        // port I/O
        //uint8_t port_read(int offset) { return m_port[offset & 1].read(); }
        //void port_write(int offset, uint8_t data) { m_port[offset & 1].write(data); }
        //void port_write(int offset, int bit, int state) { port_write(offset, (m_port[offset & 1].m_input & ~(1 << bit)) | (state << bit));  }
        //uint8_t port_a_read() { return port_read(PORT_A); }
        //uint8_t port_b_read() { return port_read(PORT_B); }
        //void port_a_write(uint8_t data) { port_write(PORT_A, data); }
        //void port_b_write(uint8_t data) { port_write(PORT_B, data); }
        //DECLARE_WRITE_LINE_MEMBER( pa0_w ) { port_write(PORT_A, 0, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa1_w ) { port_write(PORT_A, 1, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa2_w ) { port_write(PORT_A, 2, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa3_w ) { port_write(PORT_A, 3, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa4_w ) { port_write(PORT_A, 4, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa5_w ) { port_write(PORT_A, 5, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa6_w ) { port_write(PORT_A, 6, state); }
        //DECLARE_WRITE_LINE_MEMBER( pa7_w ) { port_write(PORT_A, 7, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb0_w ) { port_write(PORT_B, 0, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb1_w ) { port_write(PORT_B, 1, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb2_w ) { port_write(PORT_B, 2, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb3_w ) { port_write(PORT_B, 3, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb4_w ) { port_write(PORT_B, 4, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb5_w ) { port_write(PORT_B, 5, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb6_w ) { port_write(PORT_B, 6, state); }
        //DECLARE_WRITE_LINE_MEMBER( pb7_w ) { port_write(PORT_B, 7, state); }

        // standard read/write, with C/D in bit 1, B/A in bit 0
        //u8 read(offs_t offset);
        //void write(offs_t offset, u8 data);

        // alternate read/write, with C/D in bit 0, B/A in bit 1
        //u8 read_alt(offs_t offset);
        //void write_alt(offs_t offset, u8 data);


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_reset() { throw new emu_unimplemented(); }


        // device_z80daisy_interface overrides
        protected virtual int device_z80daisy_interface_z80daisy_irq_state() { throw new emu_unimplemented(); }
        protected virtual int device_z80daisy_interface_z80daisy_irq_ack() { throw new emu_unimplemented(); }
        protected virtual void device_z80daisy_interface_z80daisy_irq_reti() { throw new emu_unimplemented(); }


        // internal helpers
        //void check_interrupts();
    }
}
