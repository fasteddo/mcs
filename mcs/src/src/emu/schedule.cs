// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using execute_interface_enumerator = mame.device_interface_enumerator<mame.device_execute_interface>;  //typedef device_interface_enumerator<device_execute_interface> execute_interface_enumerator;
using s32 = System.Int32;
using u32 = System.UInt32;
using u64 = System.UInt64;

using static mame.attotime_global;
using static mame.cpp_global;
using static mame.diexec_global;
using static mame.eminline_global;
using static mame.machine_global;
using static mame.profiler_global;


namespace mame
{
    // timer callbacks look like this
    public delegate void timer_expired_delegate(s32 param);  //typedef named_delegate<void (s32)> timer_expired_delegate;


    // ======================> emu_timer
    public class emu_timer : simple_list_item<emu_timer>
    {
        //friend class device_scheduler;
        //friend class simple_list<emu_timer>;
        //friend class fixed_allocator<emu_timer>;


        // internal state
        running_machine m_machine;      // reference to the owning machine
        public emu_timer m_next;         // next timer in order in the list
        public emu_timer m_prev;         // previous timer in order in the list
        public timer_expired_delegate m_callback;  // callback function
        public int m_param;        // integer parameter
        public bool m_enabled;      // is the timer enabled?
        public bool m_temporary;    // is the timer temporary?
        public attotime m_period;       // the repeat frequency of the timer
        attotime m_start;        // time when the timer was started
        attotime m_expire;       // time when the timer will expire
        public device_t m_device;       // for device timers, a pointer to the device
        public device_timer_id m_id;           // for device timers, the ID of the timer


        // construction/destruction
        //-------------------------------------------------
        //  emu_timer - constructor
        //-------------------------------------------------
        public emu_timer()
        {
            m_period = attotime.zero;
            m_start = attotime.zero;
            m_expire = attotime.never;
        }


        // allocation and re-use

        //-------------------------------------------------
        //  init - completely initialize the state when
        //  re-allocated as a non-device timer
        //-------------------------------------------------
        public emu_timer init(running_machine machine, timer_expired_delegate callback, bool temporary)
        {
            // ensure the entire timer state is clean
            m_machine = machine;
            m_next = null;
            m_prev = null;
            m_callback = callback;
            m_param = 0;
            m_enabled = false;
            m_temporary = temporary;
            m_period = attotime.never;
            m_start = machine.time();
            m_expire = attotime.never;
            m_device = null;
            m_id = 0;

            // if we're not temporary, register ourselves with the save state system
            if (!m_temporary)
                register_save();

            // insert into the list
            machine.scheduler().timer_list_insert(this);

            return this;
        }

        //-------------------------------------------------
        //  init - completely initialize the state when
        //  re-allocated as a device timer
        //-------------------------------------------------
        public emu_timer init(device_t device, device_timer_id id, bool temporary)
        {
            // ensure the entire timer state is clean
            m_machine = device.machine();
            m_next = null;
            m_prev = null;
            m_callback = device_timer_expired;  //m_callback = timer_expired_delegate(FUNC(emu_timer::device_timer_expired), this);
            m_param = 0;
            m_enabled = false;
            m_temporary = temporary;
            m_period = attotime.never;
            m_start = machine().time();
            m_expire = attotime.never;
            m_device = device;
            m_id = id;

            // if we're not temporary, register ourselves with the save state system
            if (!m_temporary)
                register_save();

            // insert into the list
            machine().scheduler().timer_list_insert(this);

            return this;
        }

        //-------------------------------------------------
        //  release - release us from the global list
        //  management when deallocating
        //-------------------------------------------------
        public emu_timer release()
        {
            // unhook us from the global list
            machine().scheduler().timer_list_remove(this);
            return this;
        }


        // getters
        public emu_timer next() { return m_next; }
        public emu_timer m_next_get() { return m_next; }
        public void m_next_set(emu_timer value) { m_next = value; }

        running_machine machine() { assert(m_machine != null); return m_machine; }
        public bool enabled() { return m_enabled; }
        public int param() { return m_param; }


        // setters

        //-------------------------------------------------
        //  enable - enable/disable a timer
        //-------------------------------------------------
        public bool enable(bool enable = true)
        {
            // reschedule only if the state has changed
            bool old = m_enabled;
            if (old != enable)
            {
                // set the enable flag
                m_enabled = enable;

                // remove the timer and insert back into the list
                machine().scheduler().timer_list_remove(this);
                machine().scheduler().timer_list_insert(this);
            }

            return old;
        }

        //void set_param(int param) { m_param = param; }


        // control

        public void reset() { reset(attotime.never); }
        public void reset(attotime duration) { adjust(duration, m_param, m_period); }  //void reset(const attotime &duration = attotime::never) { adjust(duration, m_param, m_period); }

        //-------------------------------------------------
        //  adjust - adjust the time when this timer will
        //  fire and specify a period for subsequent
        //  firings
        //-------------------------------------------------
        public void adjust(attotime start_delay) { adjust(start_delay, 0, attotime.never); }
        public void adjust(attotime start_delay, int param) { adjust(start_delay, param, attotime.never); }
        public void adjust(attotime start_delay, int param, attotime period)  //void adjust(attotime start_delay, s32 param = 0, const attotime &periodicity = attotime::never);
        {
            // if this is the callback timer, mark it modified
            device_scheduler scheduler = machine().scheduler();
            if (scheduler.callback_timer() == this)
                scheduler.callback_timer_modified_set(true);

            // compute the time of the next firing and insert into the list
            m_param = param;
            m_enabled = true;

            // clamp negative times to 0
            if (start_delay.seconds() < 0)
                start_delay = attotime.zero;

            // set the start and expire times
            m_start = scheduler.time();
            m_expire = m_start + start_delay;
            m_period = period;

            // remove and re-insert the timer in its new order
            scheduler.timer_list_remove(this);
            scheduler.timer_list_insert(this);

            // if this was inserted as the head, abort the current timeslice and resync
            if (this == scheduler.first_timer())
                scheduler.abort_timeslice();
        }


        // timing queries

        //attotime elapsed() const;


        public attotime remaining()
        {
            attotime curtime = machine().time();
            if (curtime >= m_expire)
                return attotime.zero;
            return m_expire - curtime;
        }


        attotime start() { return m_start; }
        public attotime expire() { return m_expire; }
        //attotime period() const { return m_period; }


        // internal helpers

        //-------------------------------------------------
        //  register_save - register ourself with the save
        //  state system
        //-------------------------------------------------
        void register_save()
        {
            //throw new emu_unimplemented();
#if false
#endif
        }

        //-------------------------------------------------
        //  schedule_next_period - schedule the next
        //  period
        //-------------------------------------------------
        public void schedule_next_period()
        {
            // advance by one period
            m_start = m_expire;
            m_expire += m_period;

            // remove and re-insert us
            device_scheduler scheduler = machine().scheduler();
            scheduler.timer_list_remove(this);
            scheduler.timer_list_insert(this);
        }

        //-------------------------------------------------
        //  dump - dump internal state to a single output
        //  line in the error log
        //-------------------------------------------------
        public void dump()
        {
            machine().logerror("{0}: en={1} temp={2} exp={3} start={4} per={5} param={6}", this, m_enabled, m_temporary, m_expire.as_string(), m_start.as_string(), m_period.as_string(), m_param);
            if (m_device == null)
                if (m_callback == null)
                    machine().logerror(" cb=NULL\n");
                else
                    machine().logerror(" cb={0}\n", m_callback);
            else
                machine().logerror(" dev={0} id={1}\n", m_device.tag(), m_id);
        }


        //-------------------------------------------------
        //  device_timer_expired - trampoline to avoid a
        //  conditional jump on the hot path
        //-------------------------------------------------
        void device_timer_expired(s32 param)  //static void device_timer_expired(emu_timer &timer, s32 param);
        {
            this.m_device.timer_expired(this, this.m_id, param);  //timer.m_device->timer_expired(timer, timer.m_id, param);
        }
    }


    // ======================> device_scheduler
    public class device_scheduler : IDisposable
    {
        const bool VERBOSE = false;
        void LOG(string format, params object [] args) { if (VERBOSE) machine().logerror(format, args); }


        // internal state
        running_machine m_machine;                  // reference to our machine
        device_execute_interface m_executing_device;         // pointer to currently executing device
        device_execute_interface m_execute_list;             // list of devices to be executed
        attotime m_basetime;                 // global basetime; everything moves forward from here

        // list of active timers
        emu_timer m_timer_list;               // head of the active list
        fixed_allocator<emu_timer> m_timer_allocator = new fixed_allocator<emu_timer>();          // allocator for timers

        // other internal states
        emu_timer m_callback_timer;           // pointer to the current callback timer
        bool m_callback_timer_modified;  // true if the current callback timer was modified
        attotime m_callback_timer_expire_time; // the original expiration time
        bool m_suspend_changes_pending;  // suspend/resume changes are pending

        // scheduling quanta
        class quantum_slot : simple_list_item<quantum_slot>
        {
            quantum_slot m_next;
            attoseconds_t m_actual;                   // actual duration of the quantum
            attoseconds_t m_requested;                // duration of the requested quantum
            attotime m_expire;                   // absolute expiration time of this quantum

            // getters
            public quantum_slot next() { return m_next; }
            public quantum_slot m_next_get() { return m_next; }
            public void m_next_set(quantum_slot value) { m_next = value; }

            public attoseconds_t actual() { return m_actual; }
            public attoseconds_t requested() { return m_requested; }
            public attotime expire() { return m_expire; }

            // setters
            public void actual_set(attoseconds_t value) { m_actual = value; }
            public void requested_set(attoseconds_t value) { m_requested = value; }
            public void expire_set(attotime value) { m_expire = value; }
        }

        simple_list<quantum_slot>   m_quantum_list = new simple_list<quantum_slot>();             // list of active quanta
        fixed_allocator<quantum_slot> m_quantum_allocator = new fixed_allocator<quantum_slot>();      // allocator for quanta
        attoseconds_t m_quantum_minimum;          // duration of minimum quantum


        // construction/destruction

        //-------------------------------------------------
        //  device_scheduler - constructor
        //-------------------------------------------------
        public device_scheduler(running_machine machine)
        {
            m_machine = machine;
            m_basetime = attotime.zero;
            m_callback_timer_expire_time = attotime.zero;
            m_suspend_changes_pending = true;
            m_quantum_minimum = ATTOSECONDS_IN_NSEC(1) / 1000;

            // append a single never-expiring timer so there is always one in the list
            //m_timer_list = m_timer_allocator.alloc().init(machine, null, null, true);
            //m_timer_list.adjust(attotime.never);

            // register global states
            machine.save().save_item(m_basetime, "m_basetime");
            machine.save().register_presave(presave);
            machine.save().register_postload(postload);
        }

        public void device_scheduler_after_ctor(running_machine machine)
        {
            // ED: there's a circular dependency with device_scheduler.  it creates a emu_timer, which calls machine.time().  so we null check here to fix that.


            // append a single never-expiring timer so there is always one in the list
            m_timer_list = m_timer_allocator.alloc().init(machine, null, true);
            m_timer_list.adjust(attotime.never);
        }

        ~device_scheduler()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            // remove all timers
            while (m_timer_list != null)
                m_timer_allocator.reclaim(m_timer_list.release());

            m_isDisposed = true;
        }


        // getters
        running_machine machine() { return m_machine; }

        //-------------------------------------------------
        //  time - return the current time
        //-------------------------------------------------
        public attotime time()
        {
            // if we're currently in a callback, use the timer's expiration time as a base
            if (m_callback_timer != null)
                return m_callback_timer_expire_time;

            // if we're executing as a particular CPU, use its local time as a base
            // otherwise, return the global base time
            return (m_executing_device != null) ? m_executing_device.local_time() : m_basetime;
        }

        public emu_timer first_timer() { return m_timer_list; }
        public device_execute_interface currently_executing() { return m_executing_device; }
        //bool can_save() const;
        public emu_timer callback_timer() { return m_callback_timer; }
        public bool callback_timer_modified() { return m_callback_timer_modified; }


        // setters
        public void callback_timer_modified_set(bool value) { m_callback_timer_modified = value; }


        // execution

        //-------------------------------------------------
        //  timeslice - execute all devices for a single
        //  timeslice
        //-------------------------------------------------
        public void timeslice()
        {
            bool call_debugger = (machine().debug_flags & DEBUG_FLAG_ENABLED) != 0;

            // build the execution list if we don't have one yet
            //if (UNEXPECTED(m_execute_list == null))
            if (m_execute_list == null)
                rebuild_execute_list();

            // if the current quantum has expired, find a new one
            while (m_basetime >= m_quantum_list.first().expire())
                m_quantum_allocator.reclaim(m_quantum_list.detach_head());

            // loop until we hit the next timer
            while (m_basetime < m_timer_list.expire())
            {
                // by default, assume our target is the end of the next quantum
                attotime target = m_basetime + new attotime(0, m_quantum_list.first().actual());

                // however, if the next timer is going to fire before then, override
                if (m_timer_list.expire() < target)
                    target = m_timer_list.expire();

                if (machine().video().frame_update_count() % 1000 == 0)
                {
                //LOG(("------------------\n"));
                LOG("device_scheduler.timeslice() - cpu_timeslice: target = {0}, m_timer_list.expire: {1}\n", target.as_string(), m_timer_list.expire().as_string());
                }

                // do we have pending suspension changes?
                if (m_suspend_changes_pending)
                    apply_suspend_changes();

                // loop over all CPUs
                for (device_execute_interface exec = m_execute_list; exec != null; exec = exec.m_nextexec)
                {
                    // only process if this CPU is executing or truly halted (not yielding)
                    // and if our target is later than the CPU's current time (coarse check)
                    if ((exec.m_suspend == 0 || exec.m_eatcycles > 0) && target.seconds() >= exec.m_localtime.seconds())  //if (EXPECTED((exec->m_suspend == 0 || exec->m_eatcycles) && target.seconds() >= exec->m_localtime.seconds()))
                    {
                        // compute how many attoseconds to execute this CPU
                        attoseconds_t delta = target.attoseconds() - exec.m_localtime.attoseconds();
                        if (delta < 0 && target.seconds() > exec.m_localtime.seconds())
                            delta += ATTOSECONDS_PER_SECOND;

                        assert(delta == (target - exec.m_localtime).as_attoseconds());

                        if (exec.m_attoseconds_per_cycle == 0)
                        {
                            exec.m_localtime = target;
                        }
                        // if we have enough for at least 1 cycle, do the math
                        else if (delta >= exec.m_attoseconds_per_cycle)
                        {
                            // compute how many cycles we want to execute
                            int ran = exec.m_cycles_running = (int)divu_64x32((u64)delta >> exec.m_divshift, (u32)exec.m_divisor);

                            if (machine().video().frame_update_count() % 1000 == 0)
                            {
                            LOG("device_scheduler.timeslice() - cpu '{0}': {1} ({2} cycles)\n", exec.device().tag(), delta, exec.m_cycles_running);
                            }

                            // if we're not suspended, actually execute
                            if (exec.m_suspend == 0)
                            {
                                g_profiler.start(exec.m_profiler);


                                // note that this global variable cycles_stolen can be modified
                                // via the call to cpu_execute
                                exec.m_cycles_stolen = 0;
                                m_executing_device = exec;

                                exec.m_icountptr.i = exec.m_cycles_running;  // *exec->m_icountptr = exec->m_cycles_running;

                                if (!call_debugger)
                                {
                                    exec.run();
                                }
                                else
                                {
                                    exec.debugger_start_cpu_hook(target);
                                    exec.run();
                                    exec.debugger_stop_cpu_hook();
                                }

                                // adjust for any cycles we took back

                                //throw new emu_unimplemented();
#if false
                                assert(ran >= *exec->m_icountptr);
#endif

                                ran -= exec.m_icountptr.i;  //ran -= *exec->m_icountptr;

                                //throw new emu_unimplemented();
#if false
                                assert(ran >= exec->m_cycles_stolen);
#endif

                                ran -= exec.m_cycles_stolen;

                                g_profiler.stop();
                            }

                            // account for these cycles
                            exec.m_totalcycles += (u64)ran;

                            // update the local time for this CPU
                            attotime deltatime;
                            if (ran < exec.m_cycles_per_second)
                            {
                                deltatime = new attotime(0, exec.m_attoseconds_per_cycle * ran);
                            }
                            else
                            {
                                u32 remainder;
                                s32 secs = (s32)divu_64x32_rem((u64)ran, exec.m_cycles_per_second, out remainder);
                                deltatime = new attotime(secs, remainder * exec.m_attoseconds_per_cycle);
                            }

                            assert(deltatime >= attotime.zero);
                            exec.m_localtime += deltatime;

                            if (machine().video().frame_update_count() % 100 == 0)
                            {
                            LOG("device_scheduler.timeslice() - {0} ran, {1} total, time = {2}\n", ran, exec.m_totalcycles, exec.m_localtime.as_string());
                            }

                            // if the new local CPU time is less than our target, move the target up, but not before the base
                            if (exec.m_localtime < target)
                            {
                                target = attotime.Max(exec.m_localtime, m_basetime);

                                if (machine().video().frame_update_count() % 1000 == 0)
                                {
                                LOG("device_scheduler.timeslice() - (new target)\n");
                                }
                            }
                        }
                    }
                }

                m_executing_device = null;

                // update the base time
                m_basetime = target;
            }

            // execute timers
            execute_timers();
        }


        //-------------------------------------------------
        //  abort_timeslice - abort execution for the
        //  current timeslice
        //-------------------------------------------------
        public void abort_timeslice()
        {
            if (m_executing_device != null)
                m_executing_device.abort_timeslice();
        }


        //void trigger(int trigid, const attotime &after = attotime::zero);


        //-------------------------------------------------
        //  boost_interleave - temporarily boosts the
        //  interleave factor
        //-------------------------------------------------
        public void boost_interleave(attotime timeslice_time, attotime boost_duration)
        {
            // ignore timeslices > 1 second
            if (timeslice_time.seconds() > 0)
                return;

            add_scheduling_quantum(timeslice_time, boost_duration);
        }


        public void suspend_resume_changed() { m_suspend_changes_pending = true; }


        // timers, specified by callback/name

        //-------------------------------------------------
        //  timer_alloc - allocate a global non-device
        //  timer and return a pointer
        //-------------------------------------------------
        public emu_timer timer_alloc(timer_expired_delegate callback) { return m_timer_allocator.alloc().init(machine(), callback, false); }

        //-------------------------------------------------
        //  timer_set - allocate an anonymous non-device
        //  timer and set it to go off after the given
        //  amount of time
        //-------------------------------------------------
        public void timer_set(attotime duration, timer_expired_delegate callback, int param = 0) { m_timer_allocator.alloc().init(machine(), callback, true).adjust(duration, param); }

        public void synchronize(timer_expired_delegate callback = null, int param = 0) { timer_set(attotime.zero, callback, param); }


        // timers, specified by device/id; generally devices should use the device_t methods instead

        //-------------------------------------------------
        //  timer_alloc - allocate a global device timer
        //  and return a pointer
        //-------------------------------------------------
        public emu_timer timer_alloc(device_t device, device_timer_id id = 0) { return m_timer_allocator.alloc().init(device, id, false); }

        //-------------------------------------------------
        //  timer_set - allocate an anonymous device timer
        //  and set it to go off after the given amount of
        //  time
        //-------------------------------------------------
        public void timer_set(attotime duration, device_t device, device_timer_id id = 0, int param = 0) { m_timer_allocator.alloc().init(device, id, true).adjust(duration, param); }


        // debugging
        //-------------------------------------------------
        //  dump_timers - dump the current timer state
        //-------------------------------------------------
        void dump_timers()
        {
            machine().logerror("=============================================\n");
            machine().logerror("Timer Dump: Time = {0}\n", time().as_string());
            for (emu_timer timer = first_timer(); timer != null; timer = timer.next())
                timer.dump();
            machine().logerror("=============================================\n");
        }


        // for emergencies only!
        //-------------------------------------------------
        //  eat_all_cycles - eat a ton of cycles on all
        //  CPUs to force a quick exit
        //-------------------------------------------------
        public void eat_all_cycles()
        {
            for (device_execute_interface exec = m_execute_list; exec != null; exec = exec.m_nextexec)
                exec.eat_cycles(1000000000);
        }


        // callbacks
        //void timed_trigger(s32 param);

        //-------------------------------------------------
        //  presave - before creating a save state
        //-------------------------------------------------
        void presave()
        {
            // report the timer state after a log
            LOG("Prior to saving state:\n");
#if VERBOSE
            dump_timers();
#endif
        }

        //-------------------------------------------------
        //  postload - after loading a save state
        //-------------------------------------------------
        void postload()
        {
            // remove all timers and make a private list of permanent ones
            simple_list<emu_timer> private_list = new simple_list<emu_timer>();
            while (m_timer_list != null)
            {
                emu_timer timer = m_timer_list;

                // temporary timers go away entirely (except our special never-expiring one)
                if (timer.m_temporary && !timer.expire().is_never())
                    m_timer_allocator.reclaim(timer.release());

                // permanent ones get added to our private list
                else
                    private_list.append(timer_list_remove(timer));
            }

            {
                // now re-insert them; this effectively re-sorts them by time
                emu_timer timer;
                while ((timer = private_list.detach_head()) != null)
                    timer_list_insert(timer);
            }

            m_suspend_changes_pending = true;
            rebuild_execute_list();

            // report the timer state after a log
            LOG("After resetting/reordering timers:\n");
#if VERBOSE
            dump_timers();
#endif
        }

        // scheduling helpers
        //-------------------------------------------------
        //  compute_perfect_interleave - compute the
        //  "perfect" interleave interval
        //-------------------------------------------------
        public void compute_perfect_interleave()
        {
            // ensure we have a list of executing devices
            if (m_execute_list == null)
                rebuild_execute_list();

            // start with the first one
            device_execute_interface first = m_execute_list;
            if (first != null)
            {
                // start with a huge time factor and find the 2nd smallest cycle time
                attoseconds_t smallest = first.minimum_quantum();
                attoseconds_t perfect = ATTOSECONDS_PER_SECOND - 1;
                for (device_execute_interface exec = first.m_nextexec; exec != null; exec = exec.m_nextexec)
                {
                    // find the 2nd smallest cycle interval
                    attoseconds_t curquantum = exec.minimum_quantum();
                    if (curquantum < smallest)
                    {
                        perfect = smallest;
                        smallest = curquantum;
                    }
                    else if (curquantum < perfect)
                    {
                        perfect = curquantum;
                    }
                }

                // if this is a new minimum quantum, apply it
                if (m_quantum_minimum != perfect)
                {
                    // adjust all the actuals; this doesn't affect the current
                    m_quantum_minimum = perfect;
                    for (quantum_slot quant = m_quantum_list.first(); quant != null; quant = quant.next())
                        quant.actual_set(std.max(quant.requested(), m_quantum_minimum));
                }
            }
        }

        //-------------------------------------------------
        //  rebuild_execute_list - rebuild the list of
        //  executing CPUs, moving suspended CPUs to the
        //  end
        //-------------------------------------------------
        void rebuild_execute_list()
        {
            // if we haven't yet set a scheduling quantum, do it now
            if (m_quantum_list.empty())
            {
                // set the core scheduling quantum, ensuring it's no longer than 60Hz
                attotime min_quantum = machine().config().maximum_quantum(attotime.from_hz(60));

                // if the configuration specifies a device to make perfect, pick that as the minimum
                device_execute_interface exec = machine().config().perfect_quantum_device();
                if (exec != null)
                    min_quantum = attotime.Min(new attotime(0, exec.minimum_quantum()), min_quantum);

                // inform the timer system of our decision
                add_scheduling_quantum(min_quantum, attotime.never);
            }


            // start with an empty list
            //device_execute_interface **active_tailptr = &m_execute_list;
            //*active_tailptr = NULL;

            // also make an empty list of suspended devices
            //device_execute_interface *suspend_list = NULL;
            //device_execute_interface **suspend_tailptr = &suspend_list;

            List<device_execute_interface> active_list = new List<device_execute_interface>();
            List<device_execute_interface> suspend_list = new List<device_execute_interface>();


            // iterate over all devices
            foreach (device_execute_interface exec in new execute_interface_enumerator(machine().root_device()))
            {
                // append to the appropriate list
                exec.m_nextexec = null;
                if (exec.m_suspend == 0)
                {
                    //*active_tailptr = exec;
                    //active_tailptr = &exec.m_nextexec;
                    active_list.Add(exec);
                }
                else
                {
                    //*suspend_tailptr = exec;
                    //suspend_tailptr = &exec.m_nextexec;
                    suspend_list.Add(exec);
                }
            }


            // append the suspend list to the end of the active list
            //*active_tailptr = suspend_list;
            active_list.AddRange(suspend_list);
            if (active_list.Count > 0)
            {
                m_execute_list = active_list[0];

                for (int i = 0; i < active_list.Count; i++)
                {
                    if (i < active_list.Count - 1)
                        active_list[i].m_nextexec = active_list[i + 1];
                    else
                        active_list[i].m_nextexec = null;
                }
            }
        }

        //-------------------------------------------------
        //  apply_suspend_changes - applies suspend/resume
        //  changes to all device_execute_interfaces
        //-------------------------------------------------
        void apply_suspend_changes()
        {
            u32 suspendchanged = 0;

            for (device_execute_interface exec = m_execute_list; exec != null; exec = exec.m_nextexec)
            {
                suspendchanged |= exec.m_suspend ^ exec.m_nextsuspend;
                exec.m_suspend = exec.m_nextsuspend;
                exec.m_nextsuspend &= ~SUSPEND_REASON_TIMESLICE;
                exec.m_eatcycles = exec.m_nexteatcycles;
            }

            // recompute the execute list if any CPUs changed their suspension state
            if (suspendchanged != 0)
                rebuild_execute_list();
            else
                m_suspend_changes_pending = false;
        }

        //-------------------------------------------------
        //  add_scheduling_quantum - add a scheduling
        //  quantum; the smallest active one is the one
        //  that is in use
        //-------------------------------------------------
        void add_scheduling_quantum(attotime quantum, attotime duration)
        {
            assert(quantum.seconds() == 0);

            attotime curtime = time();
            attotime expire = curtime + duration;
            attoseconds_t quantum_attos = quantum.attoseconds();

            // figure out where to insert ourselves, expiring any quanta that are out-of-date
            quantum_slot insert_after = null;
            quantum_slot next;
            for (quantum_slot quant = m_quantum_list.first(); quant != null; quant = next)
            {
                // if this quantum is expired, nuke it
                next = quant.next();
                if (curtime >= quant.expire())
                    m_quantum_allocator.reclaim(m_quantum_list.detach(quant));

                // if this quantum is shorter than us, we need to be inserted afterwards
                else if (quant.requested() <= quantum_attos)
                    insert_after = quant;
            }

            // if we found an exact match, just take the maximum expiry time
            if (insert_after != null && insert_after.requested() == quantum_attos)
                insert_after.expire_set(attotime.Max(insert_after.expire(), expire));

            // otherwise, allocate a new quantum and insert it after the one we picked
            else
            {
                quantum_slot quant = m_quantum_allocator.alloc();
                quant.requested_set(quantum_attos);
                quant.actual_set(std.max(quantum_attos, m_quantum_minimum));
                quant.expire_set(expire);
                m_quantum_list.insert_after(quant, insert_after);
            }
        }


        // timer helpers

        //-------------------------------------------------
        //  timer_list_insert - insert a new timer into
        //  the list at the appropriate location
        //-------------------------------------------------
        public emu_timer timer_list_insert(emu_timer timer)
        {
            // disabled timers sort to the end
            attotime expire = timer.enabled() ? timer.expire() : attotime.never;

            // loop over the timer list
            emu_timer prevtimer = null;
            for (emu_timer curtimer = m_timer_list; curtimer != null; prevtimer = curtimer, curtimer = curtimer.next())
            {
                // if the current list entry expires after us, we should be inserted before it
                if (curtimer.expire() > expire)
                {
                    // link the new guy in before the current list entry
                    timer.m_prev = prevtimer;
                    timer.m_next = curtimer;

                    if (prevtimer != null)
                        prevtimer.m_next = timer;
                    else
                        m_timer_list = timer;

                    curtimer.m_prev = timer;

                    return timer;
                }
            }

            // need to insert after the last one
            if (prevtimer != null)
                prevtimer.m_next = timer;
            else
                m_timer_list = timer;

            timer.m_prev = prevtimer;
            timer.m_next = null;

            return timer;
        }

        //-------------------------------------------------
        //  timer_list_remove - remove a timer from the
        //  linked list
        //-------------------------------------------------
        public emu_timer timer_list_remove(emu_timer timer)
        {
            // remove it from the list
            if (timer.m_prev != null)
                timer.m_prev.m_next = timer.m_next;
            else
                m_timer_list = timer.m_next;

            if (timer.m_next != null)
                timer.m_next.m_prev = timer.m_prev;

            return timer;
        }

        //-------------------------------------------------
        //  execute_timers - execute timers that are due
        //-------------------------------------------------
        void execute_timers()
        {
            if (machine().video().frame_update_count() % 400 == 0)
            {
            LOG("execute_timers: new={0} head->expire={1}\n", m_basetime.as_string(), m_timer_list.expire().as_string());
            }

            // now process any timers that are overdue
            while (m_timer_list.expire() <= m_basetime)
            {
                // if this is a one-shot timer, disable it now
                emu_timer timer = m_timer_list;
                bool was_enabled = timer.enabled();
                if (timer.m_period.is_zero() || timer.m_period.is_never())
                    timer.m_enabled = false;

                // set the global state of which callback we're in
                m_callback_timer_modified = false;
                m_callback_timer = timer;
                m_callback_timer_expire_time = timer.expire();

                // call the callback
                if (was_enabled)
                {
                    g_profiler.start(profile_type.PROFILER_TIMER_CALLBACK);

                    if (timer.m_callback != null)
                    {
                        if (timer.m_device != null)
                            LOG("execute_timers: expired: {0} timer device {1} timer {2}\n", timer.expire().attoseconds(), timer.m_device.tag(), timer.m_id);
                        else
                            LOG("execute_timers: expired: {0} timer callback {1}\n", timer.expire().attoseconds(), timer.m_callback.ToString());

                        timer.m_callback(timer.m_param);
                    }

                    g_profiler.stop();
                }

                // reset or remove the timer, but only if it wasn't modified during the callback
                if (!m_callback_timer_modified)
                {
                    // if the timer is temporary, remove it now
                    if (timer.m_temporary)
                        m_timer_allocator.reclaim(timer.release());

                    // otherwise, reschedule it
                    else
                        timer.schedule_next_period();
                }
            }

            // clear the callback timer global
            m_callback_timer = null;
        }
    }
}
