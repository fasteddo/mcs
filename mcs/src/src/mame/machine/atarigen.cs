// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_timer_id = System.UInt32;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;


namespace mame
{
    /***************************************************************************
        CONSTANTS
    ***************************************************************************/
    //#define ATARI_CLOCK_14MHz   XTAL(14'318'181)
    //#define ATARI_CLOCK_20MHz   XTAL(20'000'000)
    //#define ATARI_CLOCK_32MHz   XTAL(32'000'000)
    //#define ATARI_CLOCK_50MHz   XTAL(50'000'000)


    static class atarigen_global
    {
        //**************************************************************************
        //  TYPE DEFINITIONS
        //**************************************************************************

        public static void PORT_ATARI_COMM_SOUND_TO_MAIN_READY(ioport_configurer configurer, driver_device device, string _tag) { ioport_global.PORT_READ_LINE_DEVICE_MEMBER(configurer, _tag, () => { return ((atari_sound_comm_device)device.subdevice(_tag)).sound_to_main_ready(); }); }  //#define PORT_ATARI_COMM_SOUND_TO_MAIN_READY(_tag) PORT_READ_LINE_DEVICE_MEMBER(_tag, atari_sound_comm_device, sound_to_main_ready)
        public static void PORT_ATARI_COMM_MAIN_TO_SOUND_READY(ioport_configurer configurer, driver_device device, string _tag) { ioport_global.PORT_READ_LINE_DEVICE_MEMBER(configurer, _tag, () => { return ((atari_sound_comm_device)device.subdevice(_tag)).main_to_sound_ready(); }); }  //#define PORT_ATARI_COMM_MAIN_TO_SOUND_READY(_tag) PORT_READ_LINE_DEVICE_MEMBER(_tag, atari_sound_comm_device, main_to_sound_ready)
    }


    // ======================> atari_sound_comm_device
    public class atari_sound_comm_device : device_t
    {
        // device type definition
        //DEFINE_DEVICE_TYPE(ATARI_SOUND_COMM, atari_sound_comm_device, "atarscom", "Atari Sound Communications")
        static device_t device_creator_atari_sound_comm_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new atari_sound_comm_device(mconfig, tag, owner, clock); }
        public static readonly device_type ATARI_SOUND_COMM = DEFINE_DEVICE_TYPE(device_creator_atari_sound_comm_device, "atarscom", "Atari Sound Communications");


        // timer IDs
        //enum
        //{
        const device_timer_id TID_SOUND_RESET = 0;
        const device_timer_id TID_SOUND_WRITE = 1;
        const device_timer_id TID_6502_WRITE  = 2;
        //};


        /***************************************************************************
            CONSTANTS
        ***************************************************************************/
        static readonly attotime SOUND_TIMER_RATE  = attotime.from_usec(5);
        static readonly attotime SOUND_TIMER_BOOST = attotime.from_usec(1000);


        // configuration state
        devcb_write_line m_main_int_cb;

        // internal state
        required_device<m6502_device> m_sound_cpu;
        bool m_main_to_sound_ready;
        bool m_sound_to_main_ready;
        u8 m_main_to_sound_data;
        u8 m_sound_to_main_data;
        u8 m_timed_int;
        u8 m_ym2151_int;


        // construction/destruction
        //template <typename T>
        atari_sound_comm_device(machine_config mconfig, string tag, device_t owner, object cputag)
            : this(mconfig, tag, owner, (u32)0)
        {
            if (cputag is string)
                m_sound_cpu.set_tag((string)cputag);
            else if (cputag is device_finder<m6502_device>)
                m_sound_cpu.set_tag((device_finder<m6502_device>)cputag);
            else
                throw new emu_unimplemented();
        }


        atari_sound_comm_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, ATARI_SOUND_COMM, tag, owner, clock)
        {
            m_main_int_cb = new devcb_write_line(this);
            m_sound_cpu = new required_device<m6502_device>(this, finder_base.DUMMY_TAG);
            m_main_to_sound_ready = false;
            m_sound_to_main_ready = false;
            m_main_to_sound_data = 0;
            m_sound_to_main_data = 0;
            m_timed_int = 0;
            m_ym2151_int = 0;
        }


        public void atari_sound_comm_device_after_ctor(object cputag)
        {
            if (cputag is string)
                m_sound_cpu.set_tag((string)cputag);
            else if (cputag is device_finder<m6502_device>)
                m_sound_cpu.set_tag((device_finder<m6502_device>)cputag);
            else
                throw new emu_unimplemented();
        }


        // configuration helpers
        public devcb_write.binder int_callback() { return m_main_int_cb.bind(); }  //auto int_callback() { return m_main_int_cb.bind(); }


        // getters
        public int main_to_sound_ready() { return m_main_to_sound_ready ? ASSERT_LINE : CLEAR_LINE; }  //DECLARE_READ_LINE_MEMBER(main_to_sound_ready) { return m_main_to_sound_ready ? ASSERT_LINE : CLEAR_LINE; }
        public int sound_to_main_ready() { return m_sound_to_main_ready ? ASSERT_LINE : CLEAR_LINE; }  //DECLARE_READ_LINE_MEMBER(sound_to_main_ready) { return m_sound_to_main_ready ? ASSERT_LINE : CLEAR_LINE; }


        // main cpu accessors (forward internally to the atari_sound_comm_device)

        //-------------------------------------------------
        //  main_command_w: Handles communication from the main CPU
        //  to the sound CPU. Two versions are provided, one with the
        //  data byte in the low 8 bits, and one with the data byte in
        //  the upper 8 bits.
        //-------------------------------------------------
        public void main_command_w(u8 data)
        {
            synchronize(TID_SOUND_WRITE, data);
        }


        //-------------------------------------------------
        //  main_response_r: Handles reading data communicated from the
        //  sound CPU to the main CPU. Two versions are provided, one
        //  with the data byte in the low 8 bits, and one with the data
        //  byte in the upper 8 bits.
        //-------------------------------------------------
        public u8 main_response_r()
        {
            if (!machine().side_effects_disabled())
            {
                m_sound_to_main_ready = false;
                m_main_int_cb.op(CLEAR_LINE);
            }

            return m_sound_to_main_data;
        }


        //void sound_reset_w(u16 data = 0);

        // sound cpu accessors
        //void sound_cpu_reset() { synchronize(TID_SOUND_RESET, 1); }


        //-------------------------------------------------
        //  sound_response_w: Handles communication from the
        //  sound CPU to the main CPU.
        //-------------------------------------------------
        public void sound_response_w(u8 data)
        {
            synchronize(TID_6502_WRITE, data);
        }


        //-------------------------------------------------
        //  sound_command_r: Handles reading data
        //  communicated from the main CPU to the sound
        //  CPU.
        //-------------------------------------------------
        public u8 sound_command_r()
        {
            if (!machine().side_effects_disabled())
            {
                m_main_to_sound_ready = false;
                m_sound_cpu.target.set_input_line(device_execute_interface.INPUT_LINE_NMI, CLEAR_LINE);
            }

            return m_main_to_sound_data;
        }


        public void sound_irq_ack_w(u8 data = 0)
        {
            m_timed_int = 0;
            update_sound_irq();
        }


        //u8 sound_irq_ack_r();


        //-------------------------------------------------
        //  sound_irq_gen: Generates an IRQ signal to the
        //  6502 sound processor.
        //-------------------------------------------------
        //INTERRUPT_GEN_MEMBER(sound_irq_gen)
        public void sound_irq_gen(device_t device)
        {
            sound_irq();
        }


        void sound_irq()
        {
            m_timed_int = 1;
            update_sound_irq();
        }


        // additional helpers
        //DECLARE_WRITE_LINE_MEMBER(ym2151_irq_gen);


        // sound I/O helpers

        //-------------------------------------------------
        //  update_sound_irq: Called whenever the IRQ state
        //  changes. An interrupt is generated if either
        //  sound_irq_gen() was called, or if the YM2151
        //  generated an interrupt via the
        //  ym2151_irq_gen() callback.
        //-------------------------------------------------
        void update_sound_irq()
        {
            if (m_timed_int != 0 || m_ym2151_int != 0)
                m_sound_cpu.target.set_input_line(m6502_device.M6502_IRQ_LINE, ASSERT_LINE);
            else
                m_sound_cpu.target.set_input_line(m6502_device.M6502_IRQ_LINE, CLEAR_LINE);
        }


        void delayed_sound_reset(int param)
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  delayed_sound_write: Synchronizes a data write
        //  from the main CPU to the sound CPU.
        //-------------------------------------------------
        void delayed_sound_write(int data)
        {
            // warn if we missed something
            if (m_main_to_sound_ready)
                logerror("Missed command from 680x0\n");

            // set up the states and signal an NMI to the sound CPU
            m_main_to_sound_data = (u8)data;
            m_main_to_sound_ready = true;
            m_sound_cpu.target.set_input_line(device_execute_interface.INPUT_LINE_NMI, ASSERT_LINE);

            // allocate a high frequency timer until a response is generated
            // the main CPU is *very* sensistive to the timing of the response
            machine().scheduler().boost_interleave(SOUND_TIMER_RATE, SOUND_TIMER_BOOST);
        }


        //-------------------------------------------------
        //  delayed_6502_write: Synchronizes a data write
        //  from the sound CPU to the main CPU.
        //-------------------------------------------------
        void delayed_6502_write(int data)
        {
            // warn if we missed something
            if (m_sound_to_main_ready)
                logerror("Missed result from 6502\n");

            // set up the states and signal the sound interrupt to the main CPU
            m_sound_to_main_data = (u8)data;
            m_sound_to_main_ready = true;
            m_main_int_cb.op(ASSERT_LINE);
        }


        // device-level overrides
        //-------------------------------------------------
        //  device_start: Start up the device
        //-------------------------------------------------
        protected override void device_start()
        {
            // resolve callbacks
            m_main_int_cb.resolve_safe();

            // register for save states
            save_item(NAME(new { m_main_to_sound_ready }));
            save_item(NAME(new { m_sound_to_main_ready }));
            save_item(NAME(new { m_main_to_sound_data }));
            save_item(NAME(new { m_sound_to_main_data }));
            save_item(NAME(new { m_timed_int }));
            save_item(NAME(new { m_ym2151_int }));
        }


        //-------------------------------------------------
        //  device_reset: Handle a device reset by
        //  clearing the interrupt lines and states
        //-------------------------------------------------
        protected override void device_reset()
        {
            // reset the internal interrupts states
            m_timed_int = m_ym2151_int = 0;

            // reset the sound I/O states
            m_main_to_sound_data = m_sound_to_main_data = 0;
            m_main_to_sound_ready = m_sound_to_main_ready = false;
        }


        //-------------------------------------------------
        //  device_timer: Handle device-specific timer
        //  calbacks
        //-------------------------------------------------
        protected override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr)
        {
            switch (id)
            {
                case TID_SOUND_RESET:
                    delayed_sound_reset(param);
                    break;

                case TID_SOUND_WRITE:
                    delayed_sound_write(param);
                    break;

                case TID_6502_WRITE:
                    delayed_6502_write(param);
                    break;
            }
        }
    }


    /***************************************************************************
        TYPES & STRUCTURES
    ***************************************************************************/
    class atarigen_screen_timer
    {
        public screen_device screen;
        public emu_timer scanline_interrupt_timer;
        public emu_timer scanline_timer;
    }


    abstract class atarigen_state : driver_device
    {
        // timer IDs
        //enum
        //{
        const int TID_SCANLINE_INTERRUPT = 0;
        const int TID_SCANLINE_TIMER = 1;
        const int TID_UNHALT_CPU = 2;
        const int TID_ATARIGEN_LAST = 3;
        //}


        protected u8 m_scanline_int_state;
        protected u8 m_video_int_state;

        protected optional_shared_ptr_uint16_t m_xscroll;
        protected optional_shared_ptr_uint16_t m_yscroll;

        /* internal state */
        u8 m_slapstic_num;
        Pointer<u8> m_slapsticU8;  //u16 *            m_slapstic;
        u8 m_slapstic_bank;
        std.vector<u8> m_slapstic_bank0;
        UInt32 m_slapstic_last_pc;
        UInt32 m_slapstic_last_address;
        UInt32 m_slapstic_base;
        UInt32 m_slapstic_mirror;

        u32 m_scanlines_per_callback;

        atarigen_screen_timer [] m_screen_timer = new atarigen_screen_timer [2];
        required_device<cpu_device> m_maincpu;

        protected optional_device<gfxdecode_device> m_gfxdecode;
        protected optional_device<screen_device> m_screen;
        optional_device<atari_slapstic_device> m_slapstic_device;


        // construction/destruction
        protected atarigen_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_scanline_int_state = 0;
            m_video_int_state = 0;
            m_xscroll = new optional_shared_ptr_uint16_t(this, "xscroll");
            m_yscroll = new optional_shared_ptr_uint16_t(this, "yscroll");
            m_slapstic_num = 0;
            m_slapsticU8 = null;
            m_slapstic_bank = 0;
            m_slapstic_last_pc = 0;
            m_slapstic_last_address = 0;
            m_slapstic_base = 0;
            m_slapstic_mirror = 0;
            m_scanlines_per_callback = 0;
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_gfxdecode = new optional_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new optional_device<screen_device>(this, "screen");
            m_slapstic_device = new optional_device<atari_slapstic_device>(this, ":slapstic");
        }


        // users must call through to these
        protected override void machine_start()
        {
            // allocate timers for all screens
            int i = 0;
            foreach (screen_device screen in new screen_device_iterator(this))
            {
                assert(i <= m_screen_timer.Length);  //assert(i <= ARRAY_LENGTH(m_screen_timer));
                m_screen_timer[i] = new atarigen_screen_timer();
                m_screen_timer[i].screen = screen;
                m_screen_timer[i].scanline_interrupt_timer = timer_alloc(TID_SCANLINE_INTERRUPT, screen);
                m_screen_timer[i].scanline_timer = timer_alloc(TID_SCANLINE_TIMER, screen);
                i++;
            }

            save_item(NAME(new { m_scanline_int_state }));
            save_item(NAME(new { m_video_int_state }));

            save_item(NAME(new { m_slapstic_num }));
            save_item(NAME(new { m_slapstic_bank }));
            save_item(NAME(new { m_slapstic_last_pc }));
            save_item(NAME(new { m_slapstic_last_address }));

            save_item(NAME(new { m_scanlines_per_callback }));
        }


        protected override void machine_reset()
        {
            // reset the interrupt states
            m_video_int_state = m_scanline_int_state = 0;

            // reset the slapstic
            if (m_slapstic_num != 0)
            {
                if (!m_slapstic_device.found())
                    fatalerror("Slapstic device is missing?\n");

                m_slapstic_device.target.slapstic_reset();
                slapstic_update_bank(m_slapstic_device.target.slapstic_bank());
            }
        }


        protected override void device_post_load()
        {
            if (m_slapstic_num != 0)
            {
                if (!m_slapstic_device.found())
                    fatalerror("Slapstic device is missing?\n");

                slapstic_update_bank(m_slapstic_device.target.slapstic_bank());
            }
        }


        protected override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr)
        {
            switch (id)
            {
                case TID_SCANLINE_INTERRUPT:
                {
                    scanline_int_write_line(1);
                    screen_device screen = (screen_device)ptr;
                    timer.adjust(screen.frame_period());
                    break;
                }

                case TID_SCANLINE_TIMER:
                    scanline_timer(timer, (screen_device)ptr, param);
                    break;

                // unhalt the CPU that was passed as a pointer
                case TID_UNHALT_CPU:
                    ((device_t)ptr).execute().set_input_line(INPUT_LINE_HALT, CLEAR_LINE);
                    break;
            }
        }


        // callbacks provided by the derived class
        protected abstract void update_interrupts();


        protected virtual void scanline_update(screen_device screen, int scanline)
        {
        }


        // interrupt handling
        //void scanline_int_set(screen_device &screen, int scanline);


        //-------------------------------------------------
        //  scanline_int_write_line: Standard write line
        //  callback for the scanline interrupt
        //-------------------------------------------------
        public void scanline_int_write_line(int state)  //WRITE_LINE_MEMBER(atarigen_state::scanline_int_write_line)
        {
            m_scanline_int_state = (u8)state;
            update_interrupts();
        }


        //-------------------------------------------------
        //  scanline_int_ack_w: Resets the state of the
        //  scanline interrupt.
        //-------------------------------------------------
        protected void scanline_int_ack_w(u16 data = 0)
        {
            m_scanline_int_state = 0;
            update_interrupts();
        }


        //-------------------------------------------------
        //  video_int_write_line: Standard write line
        //  callback for the video interrupt.
        //-------------------------------------------------
        public void video_int_write_line(int state)  //WRITE_LINE_MEMBER(atarigen_state::video_int_write_line)
        {
            if (state != 0)
            {
                m_video_int_state = 1;
                update_interrupts();
            }
        }


        //-------------------------------------------------
        //  video_int_ack_w: Resets the state of the video
        //  interrupt.
        //-------------------------------------------------
        protected void video_int_ack_w(u16 data = 0)
        {
            m_video_int_state = 0;
            update_interrupts();
        }


        // slapstic helpers
        //void slapstic_configure(cpu_device &device, offs_t base, offs_t mirror, u8 *mem);


        /***************************************************************************
            SLAPSTIC HANDLING
        ***************************************************************************/
        void slapstic_update_bank(int bank)
        {
            // if the bank has changed, copy the memory; Pit Fighter needs this
            if (bank != m_slapstic_bank)
            {
                // bank 0 comes from the copy we made earlier
                if (bank == 0)
                    memcpy(m_slapsticU8, new Pointer<u8>(m_slapstic_bank0, 0), 0x2000);  //memcpy(m_slapstic, &m_slapstic_bank0[0], 0x2000);
                else
                    memcpy(m_slapsticU8, new Pointer<u8>(m_slapsticU8, bank * 0x1000 * 2), 0x2000);  //memcpy(m_slapstic, &m_slapstic[bank * 0x1000], 0x2000);

                // remember the current bank
                m_slapstic_bank = (u8)bank;
            }
        }


        //DECLARE_WRITE16_MEMBER(slapstic_w);
        void slapstic_w(address_space space, offs_t offset, u16 data, u16 mem_mask)
        {
            throw new emu_unimplemented();
        }


        //DECLARE_READ16_MEMBER(slapstic_r);
        u16 slapstic_r(address_space space, offs_t offset, u16 mem_mask)
        {
            throw new emu_unimplemented();
        }


        // scanline timing
        /***************************************************************************
            SCANLINE TIMING
        ***************************************************************************/
        //-------------------------------------------------
        //  scanline_timer_reset: Sets up the scanline timer.
        //-------------------------------------------------
        protected void scanline_timer_reset(screen_device screen, int frequency)
        {
            // set the scanline callback
            m_scanlines_per_callback = (u32)frequency;

            // set a timer to go off at scanline 0
            if (frequency != 0)
                get_screen_timer(screen).scanline_timer.adjust(screen.time_until_pos(0));
        }


        //-------------------------------------------------
        //  scanline_timer: Called once every n scanlines
        //  to generate the periodic callback to the main
        //  system.
        //-------------------------------------------------
        void scanline_timer(emu_timer timer, screen_device screen, int scanline)
        {
            // callback
            scanline_update(screen, scanline);

            // generate another
            scanline += (int)m_scanlines_per_callback;
            if (scanline >= screen.height())
                scanline = 0;
            timer.adjust(screen.time_until_pos(scanline), scanline);
        }


        // video helpers
        //int get_hblank(screen_device &screen) const { return (screen.hpos() > (screen.width() * 9 / 10)); }
        //void halt_until_hblank_0(device_t &device, screen_device &screen);

        // misc helpers
        //void blend_gfx(int gfx0, int gfx1, int mask0, int mask1);


        static atarigen_screen_timer get_screen_timer(screen_device screen)
        {
            atarigen_state state = screen.machine().driver_data<atarigen_state>();
            int i;

            // find the index of the timer that matches the screen
            for (i = 0; i < state.m_screen_timer.Length; i++)
            {
                if (state.m_screen_timer[i].screen == screen)
                    return state.m_screen_timer[i];
            }

            fatalerror("Unexpected: no atarivc_eof_update_timer for screen '{0}'\n", screen.tag());
            return null;
        }
    }


    /***************************************************************************
        GENERAL ATARI NOTES
    **************************************************************************##

        Atari 68000 list:

        Driver        Pr? Up? VC? PF? P2? MO? AL? BM? PH?
        ----------    --- --- --- --- --- --- --- --- ---
        arcadecl.cpp       *               *       *
        atarig1.cpp        *       *      rle  *
        atarig42.cpp       *       *      rle  *
        atarigt.cpp                *      rle  *
        atarigx2.cpp               *      rle  *
        atarisy1.cpp   *   *       *       *   *              270->260
        atarisy2.cpp   *   *       *       *   *              150->120
        badlands.cpp       *       *       *                  250->260
        batman.cpp     *   *   *   *   *   *   *       *      200->160 ?
        blstroid.cpp       *       *       *                  240->230
        cyberbal.cpp       *       *       *   *              125->105 ?
        eprom.cpp          *       *       *   *              170->170
        gauntlet.cpp   *   *       *       *   *       *      220->250
        klax.cpp       *   *       *       *                  480->440 ?
        offtwall.cpp       *   *   *       *                  260->260
        rampart.cpp        *               *       *          280->280
        relief.cpp     *   *   *   *   *   *                  240->240
        shuuz.cpp          *   *   *       *                  410->290 fix!
        skullxbo.cpp       *       *       *   *              150->145
        thunderj.cpp       *   *   *   *   *   *       *      180->180
        toobin.cpp         *       *       *   *              140->115 fix!
        vindictr.cpp   *   *       *       *   *       *      200->210
        xybots.cpp     *   *       *       *   *              235->238
        ----------  --- --- --- --- --- --- --- --- ---

        Pr? - do we have verifiable proof on priorities?
        Up? - have we updated to use new MO's & tilemaps?
        VC? - does it use the video controller?
        PF? - does it have a playfield?
        P2? - does it have a dual playfield?
        MO? - does it have MO's?
        AL? - does it have an alpha layer?
        BM? - does it have a bitmap layer?
        PH? - does it use the palette hack?

    ***************************************************************************/
}
