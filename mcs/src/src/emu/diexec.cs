// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using attoseconds_t = System.Int64;
using offs_t = System.UInt32;
using s32 = System.Int32;
using seconds_t = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    // interrupt callback for VBLANK and timed interrupts
    //typedef device_delegate<void (device_t &)> device_interrupt_delegate;
    public delegate void device_interrupt_delegate(device_t device);

    // IRQ callback to be called by executing devices when an IRQ is actually taken
    //typedef device_delegate<int (device_t &, int)> device_irq_acknowledge_delegate;
    public delegate int device_irq_acknowledge_delegate(device_t device, int irqline);


    // I/O line states
    public enum line_state
    {
        CLEAR_LINE = 0,             // clear (a fired or held) line
        ASSERT_LINE,                // assert an interrupt immediately
        HOLD_LINE                   // hold interrupt line until acknowledged
    }


    // I/O line definitions
    public enum INPUT_LINE
    {
        // input lines
        MAX_INPUT_LINES = 32+3,
        INPUT_LINE_IRQ0 = 0,
        INPUT_LINE_IRQ1 = 1,
        INPUT_LINE_IRQ2 = 2,
        INPUT_LINE_IRQ3 = 3,
        INPUT_LINE_IRQ4 = 4,
        INPUT_LINE_IRQ5 = 5,
        INPUT_LINE_IRQ6 = 6,
        INPUT_LINE_IRQ7 = 7,
        INPUT_LINE_IRQ8 = 8,
        INPUT_LINE_IRQ9 = 9,
        INPUT_LINE_NMI = MAX_INPUT_LINES - 3,

        // special input lines that are implemented in the core
        INPUT_LINE_RESET = MAX_INPUT_LINES - 2,
        INPUT_LINE_HALT = MAX_INPUT_LINES - 1
    }


    public static class diexec_global
    {
        //**************************************************************************
        //  MACROS
        //**************************************************************************

        // IRQ callback to be called by device implementations when an IRQ is actually taken
        //#define IRQ_CALLBACK_MEMBER(func)       int func(device_t &device, int irqline)

        // interrupt generator callback called as a VBLANK or periodic interrupt
        //#define INTERRUPT_GEN_MEMBER(func)      void func(device_t &device)


        //**************************************************************************
        //  INTERFACE CONFIGURATION MACROS
        //**************************************************************************

        public static void MCFG_DEVICE_DISABLE(device_t device) { device.execute().set_disable(); }
        public static void MCFG_DEVICE_VBLANK_INT_DRIVER(device_t device, string tag, device_interrupt_delegate func) { device.execute().set_vblank_int(func, tag); }  //device_interrupt_delegate(&_class::_func, #_class "::" #_func, DEVICE_SELF, (_class *)0), _tag);
        //define MCFG_DEVICE_VBLANK_INT_DEVICE(_tag, _devtag, _class, _func)             device_execute_interface::static_set_vblank_int(*device, device_interrupt_delegate(&_class::_func, #_class "::" #_func, _devtag, (_class *)0), _tag);
        //define MCFG_DEVICE_VBLANK_INT_REMOVE()              device_execute_interface::static_set_vblank_int(*device, device_interrupt_delegate(), NULL);
        public static void MCFG_DEVICE_PERIODIC_INT_DRIVER(device_t device, device_interrupt_delegate func, int rate) { device.execute().set_periodic_int(func, attotime.from_hz(rate)); }  //device_interrupt_delegate(&_class::_func, #_class "::" #_func, DEVICE_SELF, (_class *)0), attotime::from_hz(_rate));
        //define MCFG_DEVICE_PERIODIC_INT_DEVICE(_devtag, _class, _func, _rate)             device_execute_interface::static_set_periodic_int(*device, device_interrupt_delegate(&_class::_func, #_class "::" #_func, _devtag, (_class *)0), attotime::from_hz(_rate));
        //define MCFG_DEVICE_PERIODIC_INT_REMOVE()              device_execute_interface::static_set_periodic_int(*device, device_interrupt_delegate(), attotime());
        //define MCFG_DEVICE_IRQ_ACKNOWLEDGE_DRIVER(_class, _func)             device_execute_interface::static_set_irq_acknowledge_callback(*device, device_irq_acknowledge_delegate(&_class::_func, #_class "::" #_func, DEVICE_SELF, (_class *)0));
        //define MCFG_DEVICE_IRQ_ACKNOWLEDGE_DEVICE(_devtag, _class, _func)             device_execute_interface::static_set_irq_acknowledge_callback(*device, device_irq_acknowledge_delegate(&_class::_func, #_class "::" #_func, _devtag, (_class *)0));
        //define MCFG_DEVICE_IRQ_ACKNOWLEDGE_REMOVE()              device_execute_interface::static_set_irq_acknowledge_callback(*device, device_irq_acknowledge_delegate());
    }


    // ======================> device_execute_interface
    public abstract class device_execute_interface : device_interface
    {
        const bool VERBOSE = false;
        const bool TEMPLOG = false;


        // internal information about the state of inputs
        class device_input
        {
            void LOG(string format, params object [] args) { if (VERBOSE) m_execute.device().logerror(format, args); }


            const UInt32 USE_STORED_VECTOR = 0xff000000;


            device_execute_interface m_execute;// pointer to the execute interface
            int m_linenum;          // which input line we are

            int m_stored_vector;    // most recently written vector
            int m_curvector;        // most recently processed vector
            line_state m_curstate;         // most recently processed state
            int [] m_queue = new int[32];        // queue of pending events
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
                m_curstate = line_state.CLEAR_LINE;
                m_qindex = 0;


                Array.Clear(m_queue, 0, m_queue.Length);  // std::fill(std::begin(m_queue), std::end(m_queue), 0);
            }


            public line_state curstate { get { return m_curstate; } }


            //-------------------------------------------------
            //  start - called by interface_pre_start so we
            //  can set ourselves up
            //-------------------------------------------------
            public void start(device_execute_interface execute, int linenum)
            {
                m_execute = execute;
                m_linenum = linenum;

                reset();

                device_t device = m_execute.device();
                device.save_item(m_stored_vector, "m_stored_vector", m_linenum);
                device.save_item(m_curvector, "m_curvector", m_linenum);
                device.save_item(m_curstate, "m_curstate", m_linenum);
            }


            //-------------------------------------------------
            //  reset - reset our input states
            //-------------------------------------------------
            public void reset()
            {
                m_stored_vector = (int)m_execute.default_irq_vector(m_linenum);
                m_curvector = m_stored_vector;
                m_qindex = 0;
            }


            //-------------------------------------------------
            //  set_state_synced - enqueue an event for later
            //  execution via timer
            //-------------------------------------------------
            public void set_state_synced(line_state state, UInt32 vector = USE_STORED_VECTOR)
            {
                LOG("set_state_synced('{0}',{1},{2},{3})\n", m_execute.device().tag(), m_linenum, state, vector);

                if (TEMPLOG) global.osd_printf_info("setline({0},{1},{2},{3})\n", m_execute.device().tag(), m_linenum, state, (vector == USE_STORED_VECTOR) ? 0 : vector);

                global.assert(state == line_state.ASSERT_LINE || state == line_state.HOLD_LINE || state == line_state.CLEAR_LINE);

                // if we're full of events, flush the queue and log a message
                int event_index = m_qindex++;
                if (event_index >= m_queue.Length)
                {
                    m_qindex--;
                    empty_event_queue();
                    event_index = m_qindex++;
                    m_execute.device().logerror("Exceeded pending input line event queue on device '{0}'!\n", m_execute.device().tag());
                }

                // enqueue the event
                if (event_index < m_queue.Length)
                {
                    if (vector == USE_STORED_VECTOR)
                        vector = (UInt32)m_stored_vector;
                    m_queue[event_index] = (int)(((UInt32)state & 0xff) | (vector << 8));

                    // if this is the first one, set the timer
                    if (event_index == 0)
                        m_execute.scheduler().synchronize(empty_event_queue, 0, this);
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
                if (m_curstate == line_state.HOLD_LINE)
                {
                    LOG("->set_irq_line('{0}',{1},{2})\n", m_execute.device().tag(), m_linenum, line_state.CLEAR_LINE);
                    m_execute.execute_set_input(m_linenum, (int)line_state.CLEAR_LINE);
                    m_curstate = line_state.CLEAR_LINE;
                }
                return vector;
            }


            //-------------------------------------------------
            //  empty_event_queue - empty our event queue
            //-------------------------------------------------
            //TIMER_CALLBACK_MEMBER(empty_event_queue);
            void empty_event_queue(object ptr = null, int param = 0)
            {
                if (TEMPLOG) global.osd_printf_info("empty_queue({0},{1},{2})\n", m_execute.device().tag(), m_linenum, m_qindex);

                // loop over all events
                for (int curevent = 0; curevent < m_qindex; curevent++)
                {
                    int input_event = m_queue[curevent];

                    // set the input line state and vector
                    m_curstate = (line_state)(input_event & 0xff);
                    m_curvector = input_event >> 8;

                    if (TEMPLOG) global.osd_printf_info(" ({0},{1})\n", m_curstate, m_curvector);

                    global.assert(m_curstate == line_state.ASSERT_LINE || m_curstate == line_state.HOLD_LINE || m_curstate == line_state.CLEAR_LINE);

                    // special case: RESET
                    if (m_linenum == (int)INPUT_LINE.INPUT_LINE_RESET)
                    {
                        // if we're asserting the line, just halt the device
                        // FIXME: outputs of onboard peripherals also need to be deactivated at this time
                        if (m_curstate == line_state.ASSERT_LINE)
                            m_execute.suspend(SUSPEND_REASON_RESET, true);

                        // if we're clearing the line that was previously asserted, reset the device
                        else if (m_execute.suspended(SUSPEND_REASON_RESET))
                        {
                            m_execute.device().reset();
                            m_execute.resume(SUSPEND_REASON_RESET);
                        }
                    }

                    // special case: HALT
                    else if (m_linenum == (int)INPUT_LINE.INPUT_LINE_HALT)
                    {
                        // if asserting, halt the device
                        if (m_curstate == line_state.ASSERT_LINE)
                            m_execute.suspend(SUSPEND_REASON_HALT, true);

                        // if clearing, unhalt the device
                        else if (m_curstate == line_state.CLEAR_LINE)
                            m_execute.resume(SUSPEND_REASON_HALT);
                    }

                    // all other cases
                    else
                    {
                        // switch off the requested state
                        switch (m_curstate)
                        {
                            case line_state.HOLD_LINE:
                            case line_state.ASSERT_LINE:
                                m_execute.execute_set_input(m_linenum, (int)line_state.ASSERT_LINE);
                                break;

                            case line_state.CLEAR_LINE:
                                m_execute.execute_set_input(m_linenum, (int)line_state.CLEAR_LINE);
                                break;

                            default:
                                m_execute.device().logerror("empty_event_queue device '{0}', line {1}, unknown state {2}\n", m_execute.device().tag(), m_linenum, m_curstate);
                                break;
                        }

                        // generate a trigger to unsuspend any devices waiting on the interrupt
                        if (m_curstate != line_state.CLEAR_LINE)
                            m_execute.signal_interrupt_trigger();
                    }
                }

                // reset counter
                m_qindex = 0;
            }
        }


        // suspension reasons for executing devices
        public const UInt32 SUSPEND_REASON_HALT        = 0x0001;   // HALT line set (or equivalent)
        public const UInt32 SUSPEND_REASON_RESET       = 0x0002;   // RESET line set (or equivalent)
        public const UInt32 SUSPEND_REASON_SPIN        = 0x0004;   // currently spinning
        public const UInt32 SUSPEND_REASON_TRIGGER     = 0x0008;   // waiting for a trigger
        public const UInt32 SUSPEND_REASON_DISABLE     = 0x0010;   // disabled (due to disable flag)
        public const UInt32 SUSPEND_REASON_TIMESLICE   = 0x0020;   // waiting for the next timeslice
        public const UInt32 SUSPEND_REASON_CLOCK       = 0x0040;   // currently not clocked
        public const UInt32 SUSPEND_ANY_REASON         = ~0U;       // all of the above


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
        device_execute_interface m_nextexec;               // pointer to the next device to execute, in order

        // input states and IRQ callbacks
        device_irq_acknowledge_delegate m_driver_irq;       // driver-specific IRQ callback
        device_input [] m_input = new device_input[(int)INPUT_LINE.MAX_INPUT_LINES];   // data about inputs
        emu_timer m_timedint_timer;           // reference to this device's periodic interrupt timer

        // cycle counting and executing
        profile_type m_profiler;                 // profiler tag
        intref m_icountptrRef; //int *                   m_icountptr;                // pointer to the icount
        int m_cycles_running;           // number of cycles we are executing
        int m_cycles_stolen;            // number of cycles we artificially stole

        // suspend states
        u32 m_suspend;                  // suspend reason mask (0 = not suspended)
        u32 m_nextsuspend;              // pending suspend reason mask
        u8 m_eatcycles;                // true if we eat cycles while suspended
        u8 m_nexteatcycles;            // pending value
        s32 m_trigger;                  // pending trigger to release a trigger suspension
        s32 m_inttrigger;               // interrupt trigger index

        // clock and timing information
        u64 m_totalcycles;              // total device cycles executed
        attotime m_localtime = new attotime();                // local time, relative to the timer system's global time
        s32 m_divisor;                  // 32-bit attoseconds_per_cycle divisor
        u8 m_divshift;                 // right shift amount to fit the divisor into 32 bits
        u32 m_cycles_per_second;        // cycles per second, adjusted for multipliers
        attoseconds_t m_attoseconds_per_cycle;    // attoseconds per adjusted clock cycle


        // construction/destruction

        //-------------------------------------------------
        //  device_execute_interface - constructor
        //-------------------------------------------------
        public device_execute_interface(machine_config mconfig, device_t device)
            : base(device, "execute")
        {
            m_scheduler = null;
            m_disabled = false;
            m_vblank_interrupt_screen = null;
            m_timed_interrupt_period = attotime.zero;
            m_nextexec = null;
            m_timedint_timer = null;
            m_profiler = profile_type.PROFILER_IDLE;
            m_icountptrRef = null;
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
            global.assert(device.interfaces().m_execute == null);
            device.interfaces().m_execute = this;
        }


        // getters
        public device_execute_interface nextexec { get { return m_nextexec; } set { m_nextexec = value; } }
        public profile_type profiler { get { return m_profiler; } }
        public intref icountptrRef { get { return m_icountptrRef; } set { m_icountptrRef = value; } }
        public int cycles_running { get { return m_cycles_running; } set { m_cycles_running = value; } }
        public int cycles_stolen { get { return m_cycles_stolen; } set { m_cycles_stolen = value; } }
        public u32 suspend_ { get { return m_suspend; } set { m_suspend = value; } }
        public u32 nextsuspend { get { return m_nextsuspend; } set { m_nextsuspend = value; } }
        public u8 eatcycles { get { return m_eatcycles; } set { m_eatcycles = value; } }
        public u8 nexteatcycles { get { return m_nexteatcycles; } }
        public u64 totalcycles { get { return m_totalcycles; } set { m_totalcycles = value; } }
        public attotime localtime { get { return m_localtime; } set { m_localtime = value; } }
        public s32 divisor { get { return m_divisor; } }
        public u8 divshift { get { return m_divshift; } }
        public u32 cycles_per_second { get { return m_cycles_per_second; } }
        public attoseconds_t attoseconds_per_cycle { get { return m_attoseconds_per_cycle; } }


        // setters
        public void icount_set(int value) { m_icountptrRef.i = value; }


        // configuration access
        bool disabled() { return m_disabled; }
        u64 clocks_to_cycles(u64 clocks) { return execute_clocks_to_cycles(clocks); }
        u64 cycles_to_clocks(u64 cycles) { return execute_cycles_to_clocks(cycles); }
        u32 min_cycles() { return execute_min_cycles(); }
        //u32 max_cycles() const { return execute_max_cycles(); }
        attotime cycles_to_attotime(u64 cycles) { return device().clocks_to_attotime(cycles_to_clocks(cycles)); }
        //UINT64 attotime_to_cycles(const attotime &duration) const { return clocks_to_cycles(device().attotime_to_clocks(duration)); }
        //UINT32 input_lines() const { return execute_input_lines(); }
        u32 default_irq_vector(int linenum) { return execute_default_irq_vector(linenum); }
        bool input_edge_triggered(int linenum) { return execute_input_edge_triggered(linenum); }


        // inline configuration helpers

        public void set_disable() { m_disabled = true; }

        public void set_vblank_int(device_interrupt_delegate function, string tag)  //template <typename Object> void set_vblank_int(Object &&cb, const char *tag)
        {
            m_vblank_interrupt = function;  //m_vblank_interrupt = std::forward<Object>(cb);
            m_vblank_interrupt_screen = tag;
        }

        //void set_vblank_int(device_interrupt_delegate callback, const char *tag)
        //{
        //    m_vblank_interrupt = callback;
        //    m_vblank_interrupt_screen = tag;
        //}
        //template <class FunctionClass> void set_vblank_int(const char *tag, const char *devname, void (FunctionClass::*callback)(device_t &), const char *name)
        //{
        //    set_vblank_int(device_interrupt_delegate(callback, name, devname, static_cast<FunctionClass *>(nullptr)), tag);
        //}
        //template <class FunctionClass> void set_vblank_int(const char *tag, void (FunctionClass::*callback)(device_t &), const char *name)
        //{
        //    set_vblank_int(device_interrupt_delegate(callback, name, nullptr, static_cast<FunctionClass *>(nullptr)), tag);
        //}


        public void set_periodic_int(device_interrupt_delegate function, attotime rate)  //template <typename Object> void set_periodic_int(Object &&cb, const attotime &rate)
        {
            m_timed_interrupt = function;  //m_timed_interrupt = std::forward<Object>(cb);
            m_timed_interrupt_period = rate;
        }

        //void set_periodic_int(device_interrupt_delegate callback, const attotime &rate)
        //{
        //    m_timed_interrupt = callback;
        //    m_timed_interrupt_period = rate;
        //}
        //template <class FunctionClass> void set_periodic_int(const char *devname, void (FunctionClass::*callback)(device_t &), const char *name, const attotime &rate)
        //{
        //    set_periodic_int(device_interrupt_delegate(callback, name, devname, static_cast<FunctionClass *>(nullptr)), rate);
        //}
        //template <class FunctionClass> void set_periodic_int(void (FunctionClass::*callback)(device_t &), const char *name, const attotime &rate)
        //{
        //    set_periodic_int(device_interrupt_delegate(callback, name, nullptr, static_cast<FunctionClass *>(nullptr)), rate);
        //}


        //template <typename Object> void set_irq_acknowledge_callback(Object &&cb) { m_driver_irq = std::forward<Object>(cb); }
        //void set_irq_acknowledge_callback(device_irq_acknowledge_delegate callback) { m_driver_irq = callback; }
        //template <class FunctionClass> void set_irq_acknowledge_callback(const char *devname, int (FunctionClass::*callback)(device_t &, int), const char *name)
        //{
        //    set_irq_acknowledge_callback(device_irq_acknowledge_delegate(callback, name, devname, static_cast<FunctionClass *>(nullptr)));
        //}
        //template <class FunctionClass> void set_irq_acknowledge_callback(int (FunctionClass::*callback)(device_t &, int), const char *name)
        //{
        //    set_irq_acknowledge_callback(device_irq_acknowledge_delegate(callback, name, nullptr, static_cast<FunctionClass *>(nullptr)));
        //}


        // execution management
        device_scheduler scheduler() { global.assert(m_scheduler != null); return m_scheduler; }
        bool executing() { return scheduler().currently_executing() == this; }
        s32 cycles_remaining() { return executing() ? m_icountptrRef.i : 0; }  // *m_icountptr : 0; } // cycles remaining in this timeslice
        public void eat_cycles(int cycles) { if (executing()) m_icountptrRef.i = (cycles > m_icountptrRef.i) ? 0 : (m_icountptrRef.i - cycles); }  // *m_icountptr = (cycles > *m_icountptr) ? 0 : (*m_icountptr - cycles); }
        void adjust_icount(int delta) { if (executing()) m_icountptrRef.i += delta; }  // *m_icountptr += delta;


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
            if (icountptrRef != null)
            {
                int delta = icountptrRef.i;
                m_cycles_stolen += delta;
                m_cycles_running -= delta;
                icountptrRef.i -= delta;
            }
        }


        // input and interrupt management
        public void set_input_line(int linenum, line_state state) { m_input[linenum].set_state_synced(state); }
        public void set_input_line_vector(int linenum, int vector) { m_input[linenum].set_vector(vector); }

        //void set_input_line_and_vector(int linenum, int state, int vector) { m_input[linenum].set_state_synced(state, vector); }
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
                if (irqline != (int)INPUT_LINE.INPUT_LINE_RESET && !input_edge_triggered(irqline))
                    throw new emu_fatalerror("device '{0}': zero-width pulse is not allowed for input line {1}\n", device().tag(), irqline);

                set_input_line(irqline, line_state.ASSERT_LINE);
                set_input_line(irqline, line_state.CLEAR_LINE);
            }
            else
            {
                set_input_line(irqline, line_state.ASSERT_LINE);

                attotime target_time = local_time() + duration;
                m_scheduler.timer_set(target_time - m_scheduler.time(), irq_pulse_clear, irqline);
            }
        }

        //void pulse_input_line_and_vector(int irqline, int vector, const attotime &duration);


        // suspend/resume

        //-------------------------------------------------
        //  suspend - set a suspend reason for this device
        //-------------------------------------------------
        void suspend(u32 reason, bool eatcycles)
        {
            if (TEMPLOG) global.osd_printf_info("suspend {0} ({1})\n", device().tag(), reason);

            // set the suspend reason and eat cycles flag
            m_nextsuspend |= reason;
            m_nexteatcycles = eatcycles ? (byte)1 : (byte)0;
            suspend_resume_changed();
        }

        //-------------------------------------------------
        //  resume - clear a suspend reason for this
        //  device
        //-------------------------------------------------
        void resume(u32 reason)
        {
            if (TEMPLOG) global.osd_printf_info("resume {0} ({1})\n", device().tag(), reason);

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

        //void suspend_until_trigger(int trigid, bool eatcycles);

        //-------------------------------------------------
        //  trigger - respond to a trigger event
        //-------------------------------------------------
        void trigger(int trigid)
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
                /*assert(m_cycles_running >= *m_icountptr);*/
                int cycles = m_cycles_running - icountptrRef.i;
                return m_localtime + cycles_to_attotime((UInt64)cycles);
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
                global.assert(m_cycles_running >= m_icountptrRef.i);
                return m_totalcycles + (u64)m_cycles_running - (u64)m_icountptrRef.i;
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
        public virtual u64 execute_clocks_to_cycles(u64 clocks) { return clocks; }

        //-------------------------------------------------
        //  execute_cycles_to_clocks - convert the number
        //  of cycles to clocks, rounding down if necessary
        //-------------------------------------------------
        public virtual u64 execute_cycles_to_clocks(u64 cycles) { return cycles; }

        //-------------------------------------------------
        //  execute_min_cycles - return the smallest number
        //  of cycles that a single instruction or
        //  operation can take
        //-------------------------------------------------
        public virtual u32 execute_min_cycles() { return 1; }

        //-------------------------------------------------
        //  execute_max_cycles - return the maximum number
        //  of cycles that a single instruction or
        //  operation can take
        //-------------------------------------------------
        public virtual u32 execute_max_cycles() { return 1; }


        // input line information getters

        //-------------------------------------------------
        //  execute_input_lines - return the total number
        //  of input lines for the device
        //-------------------------------------------------
        public virtual u32 execute_input_lines() { return 0; }

        //-------------------------------------------------
        //  execute_default_irq_vector - return the default
        //  IRQ vector when an acknowledge is processed
        //-------------------------------------------------
        public virtual u32 execute_default_irq_vector(int linenum) { return 0; }

        //-------------------------------------------------
        //  execute_input_edge_triggered - return true if
        //  the input line has an asynchronous edge trigger
        //-------------------------------------------------
        public virtual bool execute_input_edge_triggered(int linenum) { return false; }


        // optional operation overrides

        public abstract void execute_run();

        //-------------------------------------------------
        //  execute_burn - called after we consume a bunch
        //  of cycles for artifical reasons (such as
        //  spinning devices for performance optimization)
        //-------------------------------------------------
        public virtual void execute_burn(s32 cycles) { }

        //-------------------------------------------------
        //  execute_set_input - called when a synchronized
        //  input is changed
        //-------------------------------------------------
        public virtual void execute_set_input(int linenum, int state) { }


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
                screen_device_iterator iter = new screen_device_iterator(device().mconfig().root_device());
                if (iter.first() == null)
                    global.osd_printf_error("VBLANK interrupt specified, but the driver is screenless\n");
                else if (m_vblank_interrupt_screen != null && device().siblingdevice(m_vblank_interrupt_screen) == null)
                    global.osd_printf_error("VBLANK interrupt references a non-existant screen tag '{0}'\n", m_vblank_interrupt_screen);
            }

            if (m_timed_interrupt != null && m_timed_interrupt_period == attotime.zero)
                global.osd_printf_error("Timed interrupt handler specified with 0 period\n");
            else if (m_timed_interrupt == null && m_timed_interrupt_period != attotime.zero)
                global.osd_printf_error("No timer interrupt handler specified, but has a non-0 period given\n");
        }

        //-------------------------------------------------
        //  interface_pre_start - work to be done prior to
        //  actually starting a device
        //-------------------------------------------------
        public override void interface_pre_start()
        {
            m_scheduler = device().machine().scheduler();

            // bind delegates
            //m_vblank_interrupt.bind_relative_to(*device().owner());
            //m_timed_interrupt.bind_relative_to(*device().owner());
            //m_driver_irq.bind_relative_to(*device().owner());

            // fill in the initial states
            device_iterator iter = new device_iterator(device().machine().root_device());
            int index = iter.indexof(device());
            m_suspend = SUSPEND_REASON_RESET;
            m_profiler = (profile_type)(index + profile_type.PROFILER_DEVICE_FIRST);
            m_inttrigger = index + TRIGGER_INT;

            // allocate timers if we need them
            if (m_timed_interrupt_period != attotime.zero)
                m_timedint_timer = m_scheduler.timer_alloc(trigger_periodic_interrupt, this);
        }

        //-------------------------------------------------
        //  interface_post_start - work to be done after
        //  actually starting a device
        //-------------------------------------------------
        public override void interface_post_start()
        {
            // make sure somebody set us up the icount
            if (m_icountptrRef == null)
                throw new emu_fatalerror("m_icountptr never initialized!");

            // register for save states
            device().save_item(m_suspend,       "m_suspend");
            device().save_item(m_nextsuspend,   "m_nextsuspend");
            device().save_item(m_eatcycles,     "m_eatcycles");
            device().save_item(m_nexteatcycles, "m_nexteatcycles");
            device().save_item(m_trigger,       "m_trigger");
            device().save_item(m_totalcycles,   "m_totalcycles");
            device().save_item(m_localtime,     "m_localtime");

            // fill in the input states and IRQ callback information
            for (int line = 0; line < m_input.Length; line++)
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
            for (int line = 0; line < m_input.Length; line++)
                m_input[line].reset();

            // reconfingure VBLANK interrupts
            if (!string.IsNullOrEmpty(m_vblank_interrupt_screen))
            {
                // get the screen that will trigger the VBLANK
                screen_device screen = device().siblingdevice<screen_device>(m_vblank_interrupt_screen);

                //assert(screen != NULL);
                screen.register_vblank_callback(on_vblank);
            }

            // reconfigure periodic interrupts
            if (m_timed_interrupt_period != attotime.zero)
            {
                attotime timedint_period = m_timed_interrupt_period;
                //assert(m_timedint_timer != NULL);
                m_timedint_timer.adjust(timedint_period, 0, timedint_period);
            }
        }

        //-------------------------------------------------
        //  interface_clock_changed - recomputes clock
        //  information for this device
        //-------------------------------------------------
        public override void interface_clock_changed()
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
            m_cycles_per_second = (UInt32)clocks_to_cycles(device().clock());
            m_attoseconds_per_cycle = attotime.HZ_TO_ATTOSECONDS(m_cycles_per_second);

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

        int current_input_state(UInt32 i) { return (int)m_input[i].curstate; }
        public void set_icountptr(intref icountptrRef) { global.assert(m_icountptrRef == null); m_icountptrRef = icountptrRef; }

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
            if ((device().machine().debug_flags_get & running_machine.DEBUG_FLAG_ENABLED) != 0)
                device().debug().interrupt_hook(irqline);

            return vector;
        }


        // debugger hooks
        //bool debugger_enabled() const { return bool(device().machine().debug_flags & DEBUG_FLAG_ENABLED); }
        public void debugger_instruction_hook(offs_t curpc) { if ((device().machine().debug_flags_get & running_machine.DEBUG_FLAG_CALL_HOOK) != 0) device().debug().instruction_hook(curpc); }
        //void debugger_exception_hook(int exception) { if (device().machine().debug_flags & DEBUG_FLAG_ENABLED) device().debug()->exception_hook(exception); }


        // internal debugger hooks
        public void debugger_start_cpu_hook(attotime endtime) { if ((device().machine().debug_flags_get & running_machine.DEBUG_FLAG_ENABLED) != 0) device().debug().start_hook(endtime); }
        public void debugger_stop_cpu_hook() { if ((device().machine().debug_flags_get & running_machine.DEBUG_FLAG_ENABLED) != 0) device().debug().stop_hook(); }


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
        void trigger_periodic_interrupt(object ptr, int param)
        {
            // bail if there is no routine
            if (!suspended(SUSPEND_REASON_HALT | SUSPEND_REASON_RESET | SUSPEND_REASON_DISABLE | SUSPEND_REASON_CLOCK))
            {
                if (m_timed_interrupt != null)
                    m_timed_interrupt(device());
            }
        }


        //TIMER_CALLBACK_MEMBER(irq_pulse_clear) { set_input_line(int(param), CLEAR_LINE); }
        void irq_pulse_clear(object ptr, s32 param) { set_input_line((int)param, line_state.CLEAR_LINE); }


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
                return attotime.ATTOSECONDS_PER_SECOND - 1;

            // if we don't have the quantum time, compute it
            attoseconds_t basetick = m_attoseconds_per_cycle;
            if (basetick == 0)
                basetick = attotime.HZ_TO_ATTOSECONDS(clocks_to_cycles(device().clock()));

            // apply the minimum cycle count
            return basetick * min_cycles();
        }

        //attotime minimum_quantum_time() const { return attotime(0, minimum_quantum()); }
    }


    // iterator
    //typedef device_interface_iterator<device_execute_interface> execute_interface_iterator;
    public class execute_interface_iterator : device_interface_iterator<device_execute_interface>
    {
        public execute_interface_iterator(device_t root, int maxdepth = 255) : base(root, maxdepth) { }
    }
}