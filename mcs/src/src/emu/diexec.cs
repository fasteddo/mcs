// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using execute_interface_enumerator = mame.device_interface_enumerator<mame.device_execute_interface>;  //typedef device_interface_enumerator<device_execute_interface> execute_interface_enumerator;
using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using seconds_t = System.Int32;  //typedef s32 seconds_t;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt32;

using static mame.attotime_global;
using static mame.cpp_global;
using static mame.diexec_global;
using static mame.emucore_global;
using static mame.machine_global;
using static mame.osdcore_global;


namespace mame
{
    public static class diexec_global
    {
        // suspension reasons for executing devices
        public const u32 SUSPEND_REASON_HALT        = 0x0001;   // HALT line set (or equivalent)
        public const u32 SUSPEND_REASON_RESET       = 0x0002;   // RESET line set (or equivalent)
        public const u32 SUSPEND_REASON_SPIN        = 0x0004;   // currently spinning
        public const u32 SUSPEND_REASON_TRIGGER     = 0x0008;   // waiting for a trigger
        public const u32 SUSPEND_REASON_DISABLE     = 0x0010;   // disabled (due to disable flag)
        public const u32 SUSPEND_REASON_TIMESLICE   = 0x0020;   // waiting for the next timeslice
        public const u32 SUSPEND_REASON_CLOCK       = 0x0040;   // currently not clocked
        public const u32 SUSPEND_ANY_REASON         = ~0U;       // all of the above


        // I/O line states
        //public enum line_state
        //{
        public const int CLEAR_LINE  = 0;             // clear (a fired or held) line
        public const int ASSERT_LINE = 1;                // assert an interrupt immediately
        public const int HOLD_LINE   = 2;                   // hold interrupt line until acknowledged
        //}


        // I/O line definitions
        //enum
        //{
        // input lines
        public const int MAX_INPUT_LINES = 64 + 3;
        public const int INPUT_LINE_IRQ0 = 0;
        public const int INPUT_LINE_IRQ1 = 1;
        const int INPUT_LINE_IRQ2 = 2;
        const int INPUT_LINE_IRQ3 = 3;
        const int INPUT_LINE_IRQ4 = 4;
        const int INPUT_LINE_IRQ5 = 5;
        const int INPUT_LINE_IRQ6 = 6;
        const int INPUT_LINE_IRQ7 = 7;
        const int INPUT_LINE_IRQ8 = 8;
        const int INPUT_LINE_IRQ9 = 9;
        public const int INPUT_LINE_NMI = MAX_INPUT_LINES - 3;

        // special input lines that are implemented in the core
        public const int INPUT_LINE_RESET = MAX_INPUT_LINES - 2;
        public const int INPUT_LINE_HALT = MAX_INPUT_LINES - 1;
        //}


        //**************************************************************************
        //  MACROS
        //**************************************************************************

        // IRQ callback to be called by device implementations when an IRQ is actually taken
        //#define IRQ_CALLBACK_MEMBER(func)       int func(device_t &device, int irqline)

        // interrupt generator callback called as a VBLANK or periodic interrupt
        //#define INTERRUPT_GEN_MEMBER(func)      void func(device_t &device)
    }


    // interrupt callback for VBLANK and timed interrupts
    public delegate void device_interrupt_delegate(device_t device);  //typedef device_delegate<void (device_t &)> device_interrupt_delegate;

    // IRQ callback to be called by executing devices when an IRQ is actually taken
    public delegate int device_irq_acknowledge_delegate(device_t device, int irqline);  //typedef device_delegate<int (device_t &, int)> device_irq_acknowledge_delegate;


    // ======================> device_execute_interface
    public abstract class device_execute_interface : device_interface
    {
        const bool VERBOSE = false;
        const bool TEMPLOG = false;


        // internal information about the state of inputs
        class device_input
        {
            void LOG(string format, params object [] args) { if (VERBOSE) m_execute.device().logerror(format, args); }


            const u32 USE_STORED_VECTOR = 0xff000000;


            device_execute_interface m_execute;// pointer to the execute interface
            int m_linenum;          // which input line we are

            s32 m_stored_vector;    // most recently written vector
            s32 m_curvector;        // most recently processed vector
            public u8 m_curstate;         // most recently processed state
            s32 [] m_queue = new s32 [32];        // queue of pending events
            int m_qindex;           // index within the queue


            //-------------------------------------------------
            //  device_input - constructor
            //-------------------------------------------------
            public device_input()
            {
                m_execute = null;
                m_linenum = 0;
                m_stored_vector = 0;
                m_curvector = 0;
                m_curstate = (u8)CLEAR_LINE;
                m_qindex = 0;


                Array.Clear(m_queue, 0, m_queue.Length);  // std::fill(std::begin(m_queue), std::end(m_queue), 0);
            }


            //-------------------------------------------------
            //  start - called by interface_pre_start so we
            //  can set ourselves up
            //-------------------------------------------------
            public void start(device_execute_interface execute, int linenum)
            {
                m_execute = execute;
                m_linenum = linenum;

                reset();
            }


            //-------------------------------------------------
            //  reset - reset our input states
            //-------------------------------------------------
            public void reset()
            {
                m_stored_vector = (int)m_execute.default_irq_vector(m_linenum);
                m_curvector = m_stored_vector;
            }


            //-------------------------------------------------
            //  set_state_synced - enqueue an event for later
            //  execution via timer
            //-------------------------------------------------
            public void set_state_synced(int state, u32 vector = USE_STORED_VECTOR)
            {
                LOG("set_state_synced('{0}',{1},{2},{3})\n", m_execute.device().tag(), m_linenum, state, vector);

                if (TEMPLOG) osd_printf_info("setline({0},{1},{2},{3})\n", m_execute.device().tag(), m_linenum, state, (vector == USE_STORED_VECTOR) ? 0 : vector);

                assert(state == (int)ASSERT_LINE || state == (int)HOLD_LINE || state == (int)CLEAR_LINE);

                // if we're full of events, flush the queue and log a message
                int event_index = m_qindex++;
                if (event_index >= (int)std.size(m_queue))
                {
                    m_qindex--;
                    empty_event_queue(0);
                    event_index = m_qindex++;
                    m_execute.device().logerror("Exceeded pending input line event queue on device '{0}'!\n", m_execute.device().tag());
                }

                // enqueue the event
                if (event_index < (int)std.size(m_queue))
                {
                    if (vector == USE_STORED_VECTOR)
                        vector = (u32)m_stored_vector;
                    m_queue[event_index] = (int)(((u32)state & 0xff) | (vector << 8));

                    // if this is the first one, set the timer
                    if (event_index == 0)
                        m_execute.scheduler().synchronize(empty_event_queue, 0);
                }
            }

            public void set_vector(int vector) { m_stored_vector = vector; }

            //-------------------------------------------------
            //  default_irq_callback - the default IRQ
            //  callback for this input line
            //-------------------------------------------------
            public int default_irq_callback()
            {
                int vector = m_curvector;

                // if the IRQ state is HOLD_LINE, clear it
                if (m_curstate == (u8)HOLD_LINE)
                {
                    LOG("->set_irq_line('{0}',{1},{2})\n", m_execute.device().tag(), m_linenum, CLEAR_LINE);
                    m_execute.execute_set_input(m_linenum, (int)CLEAR_LINE);
                    m_curstate = (u8)CLEAR_LINE;
                }
                return vector;
            }


            //-------------------------------------------------
            //  empty_event_queue - empty our event queue
            //-------------------------------------------------
            //TIMER_CALLBACK_MEMBER(empty_event_queue);
            void empty_event_queue(s32 param)
            {
                if (TEMPLOG) osd_printf_info("empty_queue({0},{1},{2})\n", m_execute.device().tag(), m_linenum, m_qindex);

                // loop over all events
                for (int curevent = 0; curevent < m_qindex; curevent++)
                {
                    int input_event = m_queue[curevent];

                    // set the input line state and vector
                    m_curstate = (u8)(input_event & 0xff);
                    m_curvector = input_event >> 8;

                    if (TEMPLOG) osd_printf_info(" ({0},{1})\n", m_curstate, m_curvector);

                    assert(m_curstate == (u8)ASSERT_LINE || m_curstate == (u8)HOLD_LINE || m_curstate == (u8)CLEAR_LINE);

                    // special case: RESET
                    if (m_linenum == INPUT_LINE_RESET)
                    {
                        // if we're asserting the line, just halt the device
                        // FIXME: outputs of onboard peripherals also need to be deactivated at this time
                        if (m_curstate == (u8)ASSERT_LINE)
                            m_execute.suspend(SUSPEND_REASON_RESET, true);

                        // if we're clearing the line that was previously asserted, reset the device
                        else if (m_execute.suspended(SUSPEND_REASON_RESET))
                        {
                            m_execute.device().reset();
                            m_execute.resume(SUSPEND_REASON_RESET);
                        }
                    }

                    // special case: HALT
                    else if (m_linenum == INPUT_LINE_HALT)
                    {
                        // if asserting, halt the device
                        if (m_curstate == ASSERT_LINE)
                            m_execute.suspend(SUSPEND_REASON_HALT, true);

                        // if clearing, unhalt the device
                        else if (m_curstate == (u8)CLEAR_LINE)
                            m_execute.resume(SUSPEND_REASON_HALT);
                    }

                    // all other cases
                    else
                    {
                        // switch off the requested state
                        switch (m_curstate)
                        {
                            case HOLD_LINE:
                            case ASSERT_LINE:
                                m_execute.execute_set_input(m_linenum, ASSERT_LINE);
                                break;

                            case CLEAR_LINE:
                                m_execute.execute_set_input(m_linenum, CLEAR_LINE);
                                break;

                            default:
                                m_execute.device().logerror("empty_event_queue device '{0}', line {1}, unknown state {2}\n", m_execute.device().tag(), m_linenum, m_curstate);
                                break;
                        }

                        // generate a trigger to unsuspend any devices waiting on the interrupt
                        if (m_curstate != CLEAR_LINE)
                            m_execute.signal_interrupt_trigger();
                    }
                }

                // reset counter
                m_qindex = 0;
            }
        }


        const int TRIGGER_INT           = -2000;
        const int TRIGGER_SUSPENDTIME   = -4000;


        // scheduler
        device_scheduler m_scheduler;                // pointer to the machine scheduler

        // configuration
        bool m_disabled;                 // disabled from executing?
        device_interrupt_delegate m_vblank_interrupt;       // for interrupts tied to VBLANK
        string m_vblank_interrupt_screen;  // the screen that causes the VBLANK interrupt
        device_interrupt_delegate m_timed_interrupt;        // for interrupts not tied to VBLANK
        attotime m_timed_interrupt_period;   // period for periodic interrupts

        // execution lists
        public device_execute_interface m_nextexec;               // pointer to the next device to execute, in order

        // input states and IRQ callbacks
        device_irq_acknowledge_delegate m_driver_irq;       // driver-specific IRQ callback
        device_input [] m_input = new device_input[MAX_INPUT_LINES];   // data about inputs
        emu_timer m_timedint_timer;           // reference to this device's periodic interrupt timer

        // cycle counting and executing
        public profile_type m_profiler;                 // profiler tag
        public intref m_icountptr;  //int *                   m_icountptr;                // pointer to the icount
        public int m_cycles_running;           // number of cycles we are executing
        public int m_cycles_stolen;            // number of cycles we artificially stole

        // suspend states
        public u32 m_suspend;                  // suspend reason mask (0 = not suspended)
        public u32 m_nextsuspend;              // pending suspend reason mask
        public u8 m_eatcycles;                // true if we eat cycles while suspended
        public u8 m_nexteatcycles;            // pending value
        s32 m_trigger;                  // pending trigger to release a trigger suspension
        s32 m_inttrigger;               // interrupt trigger index

        // clock and timing information
        public u64 m_totalcycles;              // total device cycles executed
        public attotime m_localtime;                // local time, relative to the timer system's global time
        public s32 m_divisor;                  // 32-bit attoseconds_per_cycle divisor
        public u8 m_divshift;                 // right shift amount to fit the divisor into 32 bits
        public u32 m_cycles_per_second;        // cycles per second, adjusted for multipliers
        public attoseconds_t m_attoseconds_per_cycle;    // attoseconds per adjusted clock cycle


        // construction/destruction

        //-------------------------------------------------
        //  device_execute_interface - constructor
        //-------------------------------------------------
        public device_execute_interface(machine_config mconfig, device_t device)
            : base(device, "execute")
        {
            m_scheduler = null;
            m_disabled = false;
            m_vblank_interrupt = null;
            m_vblank_interrupt_screen = null;
            m_timed_interrupt = null;
            m_timed_interrupt_period = attotime.zero;
            m_nextexec = null;
            m_driver_irq = null;
            m_timedint_timer = null;
            m_profiler = profile_type.PROFILER_IDLE;
            m_icountptr = null;
            m_cycles_running = 0;
            m_cycles_stolen = 0;
            m_suspend = 0;
            m_nextsuspend = 0;
            m_eatcycles = 0;
            m_nexteatcycles = 0;
            m_trigger = 0;
            m_inttrigger = 0;
            m_totalcycles = 0;
            m_divisor = 0;
            m_divshift = 0;
            m_cycles_per_second = 0;
            m_attoseconds_per_cycle = 0;


            for (int line = 0; line < m_input.Length; line++)
                m_input[line] = new device_input();


            // configure the fast accessor
            assert(device.interfaces().m_execute == null);
            device.interfaces().m_execute = this;
        }


        // configuration access
        bool disabled() { return m_disabled; }
        u64 clocks_to_cycles(u64 clocks) { return execute_clocks_to_cycles(clocks); }
        u64 cycles_to_clocks(u64 cycles) { return execute_cycles_to_clocks(cycles); }
        u32 min_cycles() { return execute_min_cycles(); }
        //u32 max_cycles() const { return execute_max_cycles(); }
        public attotime cycles_to_attotime(u64 cycles) { return device().clocks_to_attotime(cycles_to_clocks(cycles)); }
        //u64 attotime_to_cycles(const attotime &duration) const { return clocks_to_cycles(device().attotime_to_clocks(duration)); }
        //u32 input_lines() const { return execute_input_lines(); }
        u32 default_irq_vector(int linenum) { return execute_default_irq_vector(linenum); }
        bool input_edge_triggered(int linenum) { return execute_input_edge_triggered(linenum); }


        // inline configuration helpers

        public void set_disable() { m_disabled = true; }


        //template <typename... T> void set_vblank_int(const char *tag, T &&... args)
        //{
        //    m_vblank_interrupt.set(std::forward<T>(args)...);
        //    m_vblank_interrupt_screen = tag;
        //}

        public void set_vblank_int(device_interrupt_delegate function, string tag)
        {
            m_vblank_interrupt = function;  //m_vblank_interrupt = std::forward<Object>(cb);
            m_vblank_interrupt_screen = tag;
        }

        public void set_vblank_int(string tag, device_interrupt_delegate callback)  //template <class FunctionClass> void set_vblank_int(const char *tag, void (FunctionClass::*callback)(device_t &), const char *name)
        {
            set_vblank_int(callback, tag);  //set_vblank_int(device_interrupt_delegate(callback, name, nullptr, static_cast<FunctionClass *>(nullptr)), tag);
        }


        public void remove_vblank_int()
        {
            m_vblank_interrupt = null;  //m_vblank_interrupt = device_interrupt_delegate(*this);
            m_vblank_interrupt_screen = null;
        }


        //template <typename F> void set_periodic_int(F &&cb, const char *name, const attotime &rate)
        //{
        //    m_timed_interrupt.set(std::forward<F>(cb), name);
        //    m_timed_interrupt_period = rate;
        //}
        //template <typename T, typename F> void set_periodic_int(T &&target, F &&cb, const char *name, const attotime &rate)
        //{
        //    m_timed_interrupt.set(std::forward<T>(target), std::forward<F>(cb), name);
        //    m_timed_interrupt_period = rate;
        //}

        public void set_periodic_int(string target, device_interrupt_delegate function, attotime rate) { set_periodic_int(function, rate); }
        public void set_periodic_int(device_interrupt_delegate function, attotime rate)
        {
            m_timed_interrupt = function;  //m_timed_interrupt.set(std::forward<F>(cb), name);
            m_timed_interrupt_period = rate;
        }


        //void remove_periodic_int()
        //{
        //    m_timed_interrupt = device_interrupt_delegate(*this);
        //    m_timed_interrupt_period = attotime();
        //}


        //template <typename... T>
        public void set_irq_acknowledge_callback(device_irq_acknowledge_delegate args)  //void set_irq_acknowledge_callback(T &&... args)
        {
            m_driver_irq = args;  //m_driver_irq.set(std::forward<T>(args)...);
        }


        //void remove_irq_acknowledge_callback()
        //{
        //    m_driver_irq = device_irq_acknowledge_delegate(*this);
        //}


        // execution management
        device_scheduler scheduler() { assert(m_scheduler != null); return m_scheduler; }
        bool executing() { return scheduler().currently_executing() == this; }
        s32 cycles_remaining() { return executing() ? m_icountptr.i : 0; }  // *m_icountptr : 0; } // cycles remaining in this timeslice
        public void eat_cycles(int cycles) { if (executing()) m_icountptr.i = (cycles > m_icountptr.i) ? 0 : (m_icountptr.i - cycles); }  // *m_icountptr = (cycles > *m_icountptr) ? 0 : (*m_icountptr - cycles); }
        public void adjust_icount(int delta) { if (executing()) m_icountptr.i += delta; }  // *m_icountptr += delta;


        //-------------------------------------------------
        //  abort_timeslice - abort execution for the
        //  current timeslice, allowing other devices to
        //  run before we run again
        //-------------------------------------------------
        public void abort_timeslice()
        {
            // ignore if not the executing device
            if (!executing())
                return;

            // swallow the remaining cycles
            if (m_icountptr != null)
            {
                int delta = m_icountptr.i;
                m_cycles_stolen += delta;
                m_cycles_running -= delta;
                m_icountptr.i -= delta;
            }
        }


        // input and interrupt management
        public void set_input_line(int linenum, int state) { m_input[linenum].set_state_synced(state); }
        public void set_input_line_vector(int linenum, int vector) { m_input[linenum].set_vector(vector); }

        public void set_input_line_and_vector(int linenum, int state, int vector) { m_input[linenum].set_state_synced(state, (u32)vector); }

        //int input_state(int linenum) { return m_input[linenum].m_curstate; }


        //-------------------------------------------------
        //  pulse_input_line - "pulse" an input line by
        //  asserting it and then clearing it later
        //-------------------------------------------------
        public void pulse_input_line(int irqline, attotime duration)
        {
            // treat instantaneous pulses as ASSERT+CLEAR
            if (duration == attotime.zero)
            {
                if (irqline != INPUT_LINE_RESET && !input_edge_triggered(irqline))
                    throw new emu_fatalerror("device '{0}': zero-width pulse is not allowed for input line {1}\n", device().tag(), irqline);

                set_input_line(irqline, ASSERT_LINE);
                set_input_line(irqline, CLEAR_LINE);
            }
            else
            {
                set_input_line(irqline, ASSERT_LINE);

                attotime target_time = local_time() + duration;
                m_scheduler.timer_set(target_time - m_scheduler.time(), irq_pulse_clear, irqline);
            }
        }


        // suspend/resume

        //-------------------------------------------------
        //  suspend - set a suspend reason for this device
        //-------------------------------------------------
        public void suspend(u32 reason, bool eatcycles)
        {
            if (TEMPLOG) osd_printf_info("suspend {0} ({1})\n", device().tag(), reason);

            // set the suspend reason and eat cycles flag
            m_nextsuspend |= reason;
            m_nexteatcycles = eatcycles ? (u8)1 : (u8)0;
            suspend_resume_changed();
        }

        //-------------------------------------------------
        //  resume - clear a suspend reason for this
        //  device
        //-------------------------------------------------
        void resume(u32 reason)
        {
            if (TEMPLOG) osd_printf_info("resume {0} ({1})\n", device().tag(), reason);

            // clear the suspend reason and eat cycles flag
            m_nextsuspend &= ~reason;
            suspend_resume_changed();
        }

        public bool suspended(u32 reason = SUSPEND_ANY_REASON) { return (m_nextsuspend & reason) != 0; }
        void yield() { suspend(SUSPEND_REASON_TIMESLICE, false); }
        void spin() { suspend(SUSPEND_REASON_TIMESLICE, true); }
        //void spin_until_trigger(int trigid) { suspend_until_trigger(trigid, true); }
        //void spin_until_time(const attotime &duration);
        //void spin_until_interrupt() { spin_until_trigger(m_inttrigger); }


        // triggers

        //-------------------------------------------------
        //  suspend_until_trigger - suspend execution
        //  until the given trigger fires
        //-------------------------------------------------
        public void suspend_until_trigger(int trigid, bool eatcycles)
        {
            // suspend the device immediately if it's not already
            suspend(SUSPEND_REASON_TRIGGER, eatcycles);

            // set the trigger
            m_trigger = trigid;
        }


        //-------------------------------------------------
        //  trigger - respond to a trigger event
        //-------------------------------------------------
        public void trigger(int trigid)
        {
            // if we're executing, for an immediate abort
            abort_timeslice();

            // see if this is a matching trigger
            if ((m_nextsuspend & SUSPEND_REASON_TRIGGER) != 0 && m_trigger == trigid)
            {
                resume(SUSPEND_REASON_TRIGGER);
                m_trigger = 0;
            }
        }

        void signal_interrupt_trigger() { trigger(m_inttrigger); }


        // time and cycle accounting
        //-------------------------------------------------
        //  local_time - returns the current local time
        //  for a device
        //-------------------------------------------------
        public attotime local_time()
        {
            // if we're active, add in the time from the current slice
            if (executing())
            {
                //throw new emu_unimplemented();
#if false
                assert(m_cycles_running >= *m_icountptr);
#endif
                int cycles = m_cycles_running - m_icountptr.i;
                return m_localtime + cycles_to_attotime((u64)cycles);
            }

            return m_localtime;
        }


        //-------------------------------------------------
        //  total_cycles - return the total number of
        //  cycles executed on this device
        //-------------------------------------------------
        public u64 total_cycles()
        {
            if (executing())
            {
                assert(m_cycles_running >= m_icountptr.i);
                return m_totalcycles + (u64)m_cycles_running - (u64)m_icountptr.i;
            }
            else
            {
                return m_totalcycles;
            }
        }


        // required operation overrides
        public void run() { execute_run(); }


        // clock and cycle information getters

        //-------------------------------------------------
        //  execute_clocks_to_cycles - convert the number
        //  of clocks to cycles, rounding down if necessary
        //-------------------------------------------------
        protected virtual u64 execute_clocks_to_cycles(u64 clocks) { return clocks; }

        //-------------------------------------------------
        //  execute_cycles_to_clocks - convert the number
        //  of cycles to clocks, rounding down if necessary
        //-------------------------------------------------
        protected virtual u64 execute_cycles_to_clocks(u64 cycles) { return cycles; }

        //-------------------------------------------------
        //  execute_min_cycles - return the smallest number
        //  of cycles that a single instruction or
        //  operation can take
        //-------------------------------------------------
        protected virtual u32 execute_min_cycles() { return 1; }

        //-------------------------------------------------
        //  execute_max_cycles - return the maximum number
        //  of cycles that a single instruction or
        //  operation can take
        //-------------------------------------------------
        protected virtual u32 execute_max_cycles() { return 1; }


        // input line information getters

        //-------------------------------------------------
        //  execute_input_lines - return the total number
        //  of input lines for the device
        //-------------------------------------------------
        protected virtual u32 execute_input_lines() { return 0; }

        //-------------------------------------------------
        //  execute_default_irq_vector - return the default
        //  IRQ vector when an acknowledge is processed
        //-------------------------------------------------
        protected virtual u32 execute_default_irq_vector(int linenum) { return 0; }

        //-------------------------------------------------
        //  execute_input_edge_triggered - return true if
        //  the input line has an asynchronous edge trigger
        //-------------------------------------------------
        protected virtual bool execute_input_edge_triggered(int linenum) { return false; }


        // optional operation overrides

        protected abstract void execute_run();

        //-------------------------------------------------
        //  execute_burn - called after we consume a bunch
        //  of cycles for artifical reasons (such as
        //  spinning devices for performance optimization)
        //-------------------------------------------------
        protected virtual void execute_burn(s32 cycles) { }

        //-------------------------------------------------
        //  execute_set_input - called when a synchronized
        //  input is changed
        //-------------------------------------------------
        protected virtual void execute_set_input(int linenum, int state) { }


        // interface-level overrides

        //-------------------------------------------------
        //  interface_validity_check - validation for a
        //  device after the configuration has been
        //  constructed
        //-------------------------------------------------
        protected override void interface_validity_check(validity_checker valid)
        {
            // validate the interrupts
            if (m_vblank_interrupt != null)
            {
                screen_device_enumerator iter = new screen_device_enumerator(device().mconfig().root_device());
                if (iter.first() == null)
                    osd_printf_error("VBLANK interrupt specified, but the driver is screenless\n");
                else if (m_vblank_interrupt_screen != null && device().siblingdevice(m_vblank_interrupt_screen) == null)
                    osd_printf_error("VBLANK interrupt references a nonexistant screen tag '{0}'\n", m_vblank_interrupt_screen);
            }

            if (m_timed_interrupt != null && m_timed_interrupt_period == attotime.zero)
                osd_printf_error("Timed interrupt handler specified with 0 period\n");
            else if (m_timed_interrupt == null && m_timed_interrupt_period != attotime.zero)
                osd_printf_error("No timer interrupt handler specified, but has a non-0 period given\n");
        }

        //-------------------------------------------------
        //  interface_pre_start - work to be done prior to
        //  actually starting a device
        //-------------------------------------------------
        public override void interface_pre_start()
        {
            m_scheduler = device().machine().scheduler();

            // bind delegates
            //m_vblank_interrupt.resolve();
            //m_timed_interrupt.resolve();
            //m_driver_irq.resolve();

            // fill in the initial states
            int index = new device_enumerator(device().machine().root_device()).indexof(device());
            m_suspend = SUSPEND_REASON_RESET;
            m_profiler = (profile_type)(index + profile_type.PROFILER_DEVICE_FIRST);
            m_inttrigger = index + TRIGGER_INT;

            // allocate timers if we need them
            if (m_timed_interrupt_period != attotime.zero)
                m_timedint_timer = m_scheduler.timer_alloc(trigger_periodic_interrupt);
        }

        //-------------------------------------------------
        //  interface_post_start - work to be done after
        //  actually starting a device
        //-------------------------------------------------
        public override void interface_post_start()
        {
            // make sure somebody set us up the icount
            if (m_icountptr == null)
                throw new emu_fatalerror("m_icountptr never initialized!");

            // register for save states
            device().save_item(NAME(new { m_suspend }));
            device().save_item(NAME(new { m_nextsuspend }));
            device().save_item(NAME(new { m_eatcycles }));
            device().save_item(NAME(new { m_nexteatcycles }));
            device().save_item(NAME(new { m_trigger }));
            device().save_item(NAME(new { m_totalcycles }));
            device().save_item(NAME(new { m_localtime }));

            //throw new emu_unimplemented();
#if false
            // it's more efficient and causes less clutter to save these this way
            device().save_item(STRUCT_MEMBER(m_input, m_stored_vector));
            device().save_item(STRUCT_MEMBER(m_input, m_curvector));
            device().save_item(STRUCT_MEMBER(m_input, m_curstate));
#endif

            // fill in the input states and IRQ callback information
            for (int line = 0; line < (int)std.size(m_input); line++)
                m_input[line].start(this, line);
        }

        //-------------------------------------------------
        //  interface_pre_reset - work to be done prior to
        //  actually resetting a device
        //-------------------------------------------------
        public override void interface_pre_reset()
        {
            // reset the total number of cycles
            m_totalcycles = 0;

            // enable all devices (except for disabled and unclocked devices)
            if (disabled())
                suspend(SUSPEND_REASON_DISABLE, true);
            else if (device().clock() != 0)
                resume(SUSPEND_ANY_REASON);
        }

        //-------------------------------------------------
        //  interface_post_reset - work to be done after a
        //  device is reset
        //-------------------------------------------------
        public override void interface_post_reset()
        {
            // reset the interrupt vectors and queues
            foreach (var elem in m_input)
                elem.reset();

            // reconfingure VBLANK interrupts
            if (!string.IsNullOrEmpty(m_vblank_interrupt_screen))
            {
                // get the screen that will trigger the VBLANK
                screen_device screen = device().siblingdevice<screen_device>(m_vblank_interrupt_screen);

                //throw new emu_unimplemented();
#if false
                assert(screen != nullptr);
#endif

                screen.register_vblank_callback(on_vblank);
            }

            // reconfigure periodic interrupts
            if (m_timed_interrupt_period != attotime.zero)
            {
                attotime timedint_period = m_timed_interrupt_period;

                //throw new emu_unimplemented();
#if false
                assert(m_timedint_timer != nullptr);
#endif

                m_timedint_timer.adjust(timedint_period, 0, timedint_period);
            }
        }

        //-------------------------------------------------
        //  interface_clock_changed - recomputes clock
        //  information for this device
        //-------------------------------------------------
        public override void interface_clock_changed(bool sync_on_new_clock_domain)
        {
            // a clock of zero disables the device
            if (device().clock() == 0)
            {
                suspend(SUSPEND_REASON_CLOCK, true);
                return;
            }

            // if we were suspended because we had no clock, enable us now
            if (suspended(SUSPEND_REASON_CLOCK))
                resume(SUSPEND_REASON_CLOCK);

            // recompute cps and spc
            m_cycles_per_second = (u32)clocks_to_cycles(device().clock());
            m_attoseconds_per_cycle = HZ_TO_ATTOSECONDS(m_cycles_per_second);

            // resynchronize the localtime to the clock domain when asked to
            if (sync_on_new_clock_domain)
                m_localtime = attotime.from_ticks(m_localtime.as_ticks(device().clock())+1, device().clock());

            // update the device's divisor
            attoseconds_t attos = m_attoseconds_per_cycle;
            m_divshift = 0;
            while (attos >= (attoseconds_t)(1UL << 31))
            {
                m_divshift++;
                attos >>= 1;
            }
            m_divisor = (seconds_t)attos;

            // re-compute the perfect interleave factor
            m_scheduler.compute_perfect_interleave();
        }


        // for use by devcpu for now...

        int current_input_state(unsigned i) { return (int)m_input[i].m_curstate; }
        public void set_icountptr(intref icount) { assert(m_icountptr == null); m_icountptr = icount; }

        //IRQ_CALLBACK_MEMBER(standard_irq_callback_member);
        //int standard_irq_callback(int irqline);
        //-------------------------------------------------
        //  standard_irq_callback_member - IRQ acknowledge
        //  callback; handles HOLD_LINE case and signals
        //  to the debugger
        //-------------------------------------------------
        public int standard_irq_callback_member(device_t device, int irqline)
        {
            return device.execute().standard_irq_callback(irqline);
        }

        public int standard_irq_callback(int irqline)
        {
            // get the default vector and acknowledge the interrupt if needed
            int vector = m_input[irqline].default_irq_callback();
            if (VERBOSE) device().logerror("standard_irq_callback('{0}', {1}) ${2}\n", device().tag(), irqline, vector);  // $%04x\n

            // if there's a driver callback, run it to get the vector
            if (m_driver_irq != null)
                vector = m_driver_irq(device(), irqline);

            // notify the debugger
            if ((device().machine().debug_flags & DEBUG_FLAG_ENABLED) != 0)
                device().debug().interrupt_hook(irqline);

            return vector;
        }


        // debugger hooks
        public bool debugger_enabled() { return (device().machine().debug_flags & DEBUG_FLAG_ENABLED) != 0; }
        public void debugger_instruction_hook(offs_t curpc) { if ((device().machine().debug_flags & DEBUG_FLAG_CALL_HOOK) != 0) device().debug().instruction_hook(curpc); }
        //void debugger_exception_hook(int exception) { if (device().machine().debug_flags & DEBUG_FLAG_ENABLED) device().debug()->exception_hook(exception); }
        //void debugger_privilege_hook() { if (device().machine().debug_flags & DEBUG_FLAG_ENABLED) device().debug()->privilege_hook(); }


        // internal debugger hooks
        public void debugger_start_cpu_hook(attotime endtime) { if ((device().machine().debug_flags & DEBUG_FLAG_ENABLED) != 0) device().debug().start_hook(endtime); }
        public void debugger_stop_cpu_hook() { if ((device().machine().debug_flags & DEBUG_FLAG_ENABLED) != 0) device().debug().stop_hook(); }


        // callbacks
        //TIMER_CALLBACK_MEMBER(timed_trigger_callback);
        //static void static_timed_trigger_callback(running_machine &machine, void *ptr, int param);


        //-------------------------------------------------
        //  on_vblank - calls any external callbacks
        //  for this screen
        //-------------------------------------------------
        void on_vblank(screen_device screen, bool vblank_state)
        {
            // ignore VBLANK end
            if (!vblank_state)
                return;

            // generate the interrupt callback
            if (!suspended(SUSPEND_REASON_HALT | SUSPEND_REASON_RESET | SUSPEND_REASON_DISABLE | SUSPEND_REASON_CLOCK))
            {
                if (m_vblank_interrupt != null)
                    m_vblank_interrupt(device());
            }
        }


        //TIMER_CALLBACK_MEMBER(trigger_periodic_interrupt);
        void trigger_periodic_interrupt(s32 param)
        {
            // bail if there is no routine
            if (!suspended(SUSPEND_REASON_HALT | SUSPEND_REASON_RESET | SUSPEND_REASON_DISABLE | SUSPEND_REASON_CLOCK))
            {
                if (m_timed_interrupt != null)
                    m_timed_interrupt(device());
            }
        }


        void irq_pulse_clear(s32 param) { set_input_line(param, CLEAR_LINE); }  //TIMER_CALLBACK_MEMBER(irq_pulse_clear) { set_input_line(int(param), CLEAR_LINE); }


        //-------------------------------------------------
        //  suspend_resume_changed
        //-------------------------------------------------
        void suspend_resume_changed()
        {
            // inform the scheduler
            m_scheduler.suspend_resume_changed();

            // if we're active, synchronize
            abort_timeslice();
        }


        //-------------------------------------------------
        //  minimum_quantum - return the minimum quantum
        //  required for this device
        //-------------------------------------------------
        public attoseconds_t minimum_quantum()
        {
            // if we don't have a clock, return a huge factor
            if (device().clock() == 0)
                return ATTOSECONDS_PER_SECOND - 1;

            // if we don't have the quantum time, compute it
            attoseconds_t basetick = m_attoseconds_per_cycle;
            if (basetick == 0)
                basetick = HZ_TO_ATTOSECONDS(clocks_to_cycles(device().clock()));

            // apply the minimum cycle count
            return basetick * min_cycles();
        }

        //attotime minimum_quantum_time() const { return attotime(0, minimum_quantum()); }
    }


    // iterator
    //typedef device_interface_enumerator<device_execute_interface> execute_interface_enumerator;
}
