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
        //friend class fixed_allocator<emu_timer>;
        //friend class simple_list<emu_timer>; // FIXME: fixed_allocator requires this


        // internal state
        device_scheduler m_scheduler;    // reference to the owning machine
        public emu_timer m_next;         // next timer in order in the list
        public emu_timer m_prev;         // previous timer in order in the list
        public timer_expired_delegate m_callback;  // callback function
        public int m_param;        // integer parameter
        public bool m_enabled;      // is the timer enabled?
        public bool m_temporary;    // is the timer temporary?
        public attotime m_period;       // the repeat frequency of the timer
        attotime m_start;        // time when the timer was started
        public attotime m_expire;       // time when the timer will expire


        // construction/destruction
        //-------------------------------------------------
        //  emu_timer - constructor
        //-------------------------------------------------
        public emu_timer()
        {
            //m_period = attotime.zero;
            //m_start = attotime.zero;
            //m_expire = attotime.never;
            m_scheduler = null;
            m_next = null;
            m_prev = null;
            m_param = 0;
            m_enabled = false;
            m_temporary = false;
            m_period = attotime.zero;
            m_start = attotime.zero;
            m_expire = attotime.never;
        }


        // allocation and re-use

        //-------------------------------------------------
        //  init - completely initialize the state when
        //  re-allocated as a non-device timer
        //-------------------------------------------------
        // allocation and re-use
        public emu_timer init(
                running_machine machine,
                timer_expired_delegate callback,
                attotime start_delay,
                int param,
                bool temporary)
        {
            // ensure the entire timer state is clean
            m_scheduler = machine.scheduler();
            m_next = null;
            m_prev = null;
            m_callback = callback;
            m_param = param;
            m_temporary = temporary;
            m_period = attotime.never;

            m_start = m_scheduler.time();
            m_expire = m_start + start_delay;
            m_enabled = !m_expire.is_never();

            // if we're not temporary, register ourselves with the save state system
            if (!m_temporary)
                register_save(machine.save());

            // insert into the list
            m_scheduler.timer_list_insert(this);
            if (this == m_scheduler.first_timer())
                m_scheduler.abort_timeslice();

            return this;
        }


        // getters
        public emu_timer next() { return m_next; }
        public emu_timer m_next_get() { return m_next; }
        public void m_next_set(emu_timer value) { m_next = value; }

        public bool enabled() { return m_enabled; }
        public int param() { return m_param; }


        // setters

        //-------------------------------------------------
        //  enable - enable/disable a timer
        //-------------------------------------------------
        public bool enable(bool enable = true)
        {
            assert(m_scheduler != default);

            // reschedule only if the state has changed
            bool old = m_enabled;
            if (old != enable)
            {
                // set the enable flag
                m_enabled = enable;

                // remove the timer and insert back into the list
                m_scheduler.timer_list_remove(this);
                m_scheduler.timer_list_insert(this);
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
            assert(m_scheduler != default);

            // if this is the callback timer, mark it modified
            if (m_scheduler.m_callback_timer == this)
                m_scheduler.m_callback_timer_modified = true;

            // compute the time of the next firing and insert into the list
            m_param = param;
            m_enabled = true;

            // clamp negative times to 0
            if (start_delay.seconds() < 0)
                start_delay = attotime.zero;

            // set the start and expire times
            m_start = m_scheduler.time();
            m_expire = m_start + start_delay;
            m_period = period;

            // remove and re-insert the timer in its new order
            m_scheduler.timer_list_remove(this);
            m_scheduler.timer_list_insert(this);

            // if this was inserted as the head, abort the current timeslice and resync
            if (this == m_scheduler.first_timer())
                m_scheduler.abort_timeslice();
        }


        // timing queries

        //attotime elapsed() const;


        public attotime remaining()
        {
            assert(m_scheduler != default);

            attotime curtime = m_scheduler.time();
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
        void register_save(save_manager manager)
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
            assert(m_scheduler != default);

            // advance by one period
            m_start = m_expire;
            m_expire += m_period;

            // remove and re-insert us
            m_scheduler.timer_list_remove(this);
            m_scheduler.timer_list_insert(this);
        }

        //-------------------------------------------------
        //  dump - dump internal state to a single output
        //  line in the error log
        //-------------------------------------------------
        public void dump()
        {
            assert(m_scheduler != default);

            m_scheduler.machine().logerror("{0}: en={1} temp={2} exp={3} start={4} per={5} param={6}", this, m_enabled, m_temporary, m_expire.as_string(), m_start.as_string(), m_period.as_string(), m_param);
            if (m_callback == null)
                m_scheduler.machine().logerror(" cb=NULL\n");
            else
                m_scheduler.machine().logerror(" cb={0}\n", m_callback);
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
        emu_timer m_inactive_timers;          // head of the inactive timer list
        fixed_allocator<emu_timer> m_timer_allocator = new fixed_allocator<emu_timer>();          // allocator for timers

        // other internal states
        public emu_timer m_callback_timer;           // pointer to the current callback timer
        public bool m_callback_timer_modified;  // true if the current callback timer was modified
        attotime m_callback_timer_expire_time; // the original expiration time
        bool m_suspend_changes_pending;  // suspend/resume changes are pending


        // scheduling quanta
        class quantum_slot : simple_list_item<quantum_slot>
        {
            quantum_slot m_next;
            public attoseconds_t m_actual;                   // actual duration of the quantum
            public attoseconds_t m_requested;                // duration of the requested quantum
            public attotime m_expire;                   // absolute expiration time of this quantum


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
            m_executing_device = null;
            m_execute_list = null;
            m_basetime = attotime.zero;
            m_timer_list = null;
            m_inactive_timers = null;
            m_callback_timer = null;
            m_callback_timer_modified = false;
            m_callback_timer_expire_time = attotime.zero;
            m_suspend_changes_pending = true;
            m_quantum_minimum = ATTOSECONDS_IN_NSEC(1) / 1000;


            // see device_scheduler_after_ctor

            //// append a single never-expiring timer so there is always one in the list
            //// need to subvert it because it would naturally be inserted in the inactive list
            //m_timer_list = &timer_list_remove(m_timer_allocator.alloc()->init(machine, timer_expired_delegate(), attotime::never, 0, true));
            //
            //assert(m_timer_list);
            //assert(!m_timer_list->m_prev);
            //assert(!m_timer_list->m_next);
            //assert(!m_inactive_timers);
            //
            //// register global states
            //machine.save().save_item(NAME(m_basetime));
            //machine.save().register_presave(save_prepost_delegate(FUNC(device_scheduler::presave), this));
            //machine.save().register_postload(save_prepost_delegate(FUNC(device_scheduler::postload), this));
        }

        public void device_scheduler_after_ctor(running_machine machine)
        {
            // ED: there's a circular dependency with device_scheduler.  it creates a emu_timer, which calls machine.time().  so we null check here to fix that.

            // append a single never-expiring timer so there is always one in the list
            // need to subvert it because it would naturally be inserted in the inactive list
            m_timer_list = timer_list_remove(m_timer_allocator.alloc().init(machine, null, attotime.never, 0, true));

            assert(m_timer_list != null);
            assert(m_timer_list.m_prev == null);
            assert(m_timer_list.m_next == null);
            assert(m_inactive_timers == null);

            // register global states
            machine.save().save_item(m_basetime, "m_basetime");
            machine.save().register_presave(presave);
            machine.save().register_postload(postload);
        }

        ~device_scheduler()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            // remove all timers
            while (m_inactive_timers != null)
                m_timer_allocator.reclaim(timer_list_remove(m_inactive_timers));
            while (m_timer_list != null)
                m_timer_allocator.reclaim(timer_list_remove(m_timer_list));

            m_isDisposed = true;
        }


        // getters
        public running_machine machine() { return m_machine; }

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
        //  add_quantum - add a scheduling quantum;
        //  the smallest active one is the one that is in use
        //-------------------------------------------------
        void add_quantum(attotime quantum, attotime duration)
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
                if (curtime >= quant.m_expire)
                    m_quantum_allocator.reclaim(m_quantum_list.detach(quant));

                // if this quantum is shorter than us, we need to be inserted afterwards
                else if (quant.m_requested <= quantum_attos)
                    insert_after = quant;
            }

            // if we found an exact match, just take the maximum expiry time
            if (insert_after != null && insert_after.m_requested == quantum_attos)
                insert_after.m_expire = attotime.Max(insert_after.m_expire, expire);

            // otherwise, allocate a new quantum and insert it after the one we picked
            else
            {
                quantum_slot quant = m_quantum_allocator.alloc();
                quant.m_requested = quantum_attos;
                quant.m_actual = std.max(quantum_attos, m_quantum_minimum);
                quant.m_expire = expire;
                m_quantum_list.insert_after(quant, insert_after);
            }
        }


        //-------------------------------------------------
        //  perfect_quantum - add a (temporary) minimum
        //  scheduling quantum to boost the interleave
        //-------------------------------------------------
        public void perfect_quantum(attotime duration)
        {
            add_quantum(attotime.zero, duration);
        }


        public void suspend_resume_changed() { m_suspend_changes_pending = true; }


        // timers, specified by callback/name

        //-------------------------------------------------
        //  timer_alloc - allocate a global non-device
        //  timer and return a pointer
        //-------------------------------------------------
        public emu_timer timer_alloc(timer_expired_delegate callback)
        {
            return m_timer_allocator.alloc().init(machine(), callback, attotime.never, 0, false);
        }

        //-------------------------------------------------
        //  timer_set - allocate an anonymous non-device
        //  timer and set it to go off after the given
        //  amount of time
        //-------------------------------------------------
        //[[deprecated("timer_set is deprecated; please avoid anonymous timers. Use TIMER_CALLBACK_MEMBER and an allocated emu_timer instead.")]]
        public void timer_set(attotime duration, timer_expired_delegate callback, int param = 0)
        {
            emu_timer timer = m_timer_allocator.alloc().init(
                    machine(),
                    callback,
                    duration,
                    param,
                    true);
            assert(!timer.m_expire.is_never()); // this is not handled
        }

        //-------------------------------------------------
        //  synchronize - allocate an anonymous non-device
        //  timer and set it to go off as soon as possible
        //-------------------------------------------------
        public void synchronize(timer_expired_delegate callback = null, int param = 0)
        {
            m_timer_allocator.alloc().init(
                    machine(),
                    callback,
                    attotime.zero,
                    param,
                    true);
        }


        // debugging
        //-------------------------------------------------
        //  dump_timers - dump the current timer state
        //-------------------------------------------------
        void dump_timers()
        {
            machine().logerror("=============================================\n");
            machine().logerror("Timer Dump: Time = {0}\n", time().as_string());
            for (emu_timer timer = m_timer_list; timer != null; timer = timer.m_next)
                timer.dump();
            for (emu_timer timer = m_inactive_timers; timer != null; timer = timer.m_next)
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
            emu_timer private_list = null;
            while (m_inactive_timers != null)
            {
                emu_timer timer = m_inactive_timers;
                assert(!timer.m_temporary);

                timer_list_remove(timer).m_next = private_list;
                private_list = timer;
            }

            while (m_timer_list.m_next != null)
            {
                emu_timer timer = m_timer_list;

                if (timer.m_temporary)
                {
                    assert(!timer.expire().is_never());

                    // temporary timers go away entirely (except our special never-expiring one)
                    m_timer_allocator.reclaim(timer_list_remove(timer));
                }
                else
                {
                    // permanent ones get added to our private list
                    timer_list_remove(timer).m_next = private_list;
                    private_list = timer;
                }
            }

            // special dummy timer
            assert(!m_timer_list.m_enabled);
            assert(m_timer_list.m_temporary);
            assert(m_timer_list.m_expire.is_never());

            // now re-insert them; this effectively re-sorts them by time
            while (private_list != null)
            {
                emu_timer timer = private_list;
                private_list = timer.m_next;
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
                add_quantum(min_quantum, attotime.never);
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


        // timer helpers

        //-------------------------------------------------
        //  timer_list_insert - insert a new timer into
        //  the list at the appropriate location
        //-------------------------------------------------
        public emu_timer timer_list_insert(emu_timer timer)
        {
            // disabled timers never expire
            if (!timer.m_expire.is_never() && timer.m_enabled)
            {
                // loop over the timer list
                emu_timer prevtimer = null;
                for (emu_timer curtimer = m_timer_list; curtimer != null; prevtimer = curtimer, curtimer = curtimer.m_next)
                {
                    // if the current list entry expires after us, we should be inserted before it
                    if (curtimer.m_expire > timer.m_expire)
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
            }
            else
            {
                // keep inactive timers in a separate list
                if (m_inactive_timers != null)
                    m_inactive_timers.m_prev = timer;

                timer.m_next = m_inactive_timers;
                timer.m_prev = null;

                m_inactive_timers = timer;
            }

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
            {
                timer.m_prev.m_next = timer.m_next;
            }
            else if (timer == m_timer_list)
            {
                m_timer_list = timer.m_next;
            }
            else
            {
                assert(timer == m_inactive_timers);
                m_inactive_timers = timer.m_next;
            }

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
                        LOG("execute_timers: expired: {0} timer callback {1}\n", timer.expire().attoseconds(), timer.m_callback.ToString());

                        timer.m_callback(timer.m_param);
                    }

                    g_profiler.stop();
                }

                // reset or remove the timer, but only if it wasn't modified during the callback
                if (!m_callback_timer_modified)
                {
                    if (!timer.m_temporary)
                    {
                        // if the timer is not temporary, reschedule it
                        timer.schedule_next_period();
                    }
                    else
                    {
                        // otherwise, remove it now
                        m_timer_allocator.reclaim(timer_list_remove(timer));
                    }
                }
            }

            // clear the callback timer global
            m_callback_timer = null;
        }
    }
}
