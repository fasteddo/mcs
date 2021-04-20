// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Threading;

using int32_t = System.Int32;
using osd_ticks_t = System.UInt64;
using uint32_t = System.UInt32;


namespace mame
{
    public static class osdsync_global
    {
        public const osd_ticks_t OSD_EVENT_WAIT_INFINITE = ~(osd_ticks_t)0;

        const string ENV_PROCESSORS               = "OSDPROCESSORS";
        const string ENV_WORKQUEUEMAXTHREADS      = "OSDWORKQUEUEMAXTHREADS";

        static osd_ticks_t SPIN_LOOP_TIME         = osdcore_global.m_osdcore.osd_ticks_per_second() / 10000;


#if KEEP_STATISTICS
        define add_to_stat(v,x)        do { (v) += (x); } while (0)
        define begin_timing(v)         do { (v) -= get_profile_ticks(); } while (0)
        define end_timing(v)           do { (v) += get_profile_ticks(); } while (0)
#else
        static void add_to_stat(ref osd_ticks_t v, osd_ticks_t x) { }
        static void begin_timing(ref osd_ticks_t v) { }
        static void end_timing(ref osd_ticks_t v) { }
#endif


        public static int osd_num_processors = 0;


        //template<typename _AtomType, typename _MainType>
        static void spin_while(ref int atom, int val, osd_ticks_t timeout)  //static void spin_while(const volatile _AtomType * volatile atom, const _MainType val, const osd_ticks_t timeout, const int invert = 0)
        {
            osd_ticks_t stopspin = osdcore_global.m_osdcore.osd_ticks() + timeout;

            do
            {
                int spin = 10000;
                while (--spin != 0)
                {
                    //if ((*atom != val) ^ invert)
                    if (atom != val)
                        return;
                }
            } while (atom == val && osdcore_global.m_osdcore.osd_ticks() < stopspin);  //(((*atom == val) ^ invert) && osd_ticks() < stopspin);
        }

        static void spin_while(ref osd_work_item atom, osd_work_item val, osd_ticks_t timeout)  //static void spin_while(const volatile _AtomType * volatile atom, const _MainType val, const osd_ticks_t timeout, const int invert = 0)
        {
            osd_ticks_t stopspin = osdcore_global.m_osdcore.osd_ticks() + timeout;

            do
            {
                int spin = 10000;
                while (--spin != 0)
                {
                    //if ((*atom != val) ^ invert)
                    if (atom != val)
                        return;
                }
            } while (atom == val && osdcore_global.m_osdcore.osd_ticks() < stopspin);  //(((*atom == val) ^ invert) && osd_ticks() < stopspin);
        }

        //template<typename _AtomType, typename _MainType>
        static void spin_while_not(ref int atom, int val, osd_ticks_t timeout)  //static void spin_while_not(const volatile _AtomType * volatile atom, const _MainType val, const osd_ticks_t timeout)
        {
            //spin_while<_AtomType, _MainType>(atom, val, timeout, 1);
            osd_ticks_t stopspin = osdcore_global.m_osdcore.osd_ticks() + timeout;

            do
            {
                int spin = 10000;
                while (--spin != 0)
                {
                    //if ((*atom != val) ^ invert)
                    if (atom == val)
                        return;
                }
            } while (atom != val && osdcore_global.m_osdcore.osd_ticks() < stopspin);  //(((*atom == val) ^ invert) && osd_ticks() < stopspin);
        }


        //============================================================
        //  osd_num_processors
        //============================================================
        static int osd_get_num_processors()
        {
            // max out at 4 for now since scaling above that seems to do poorly
            //return std::min(std::thread::hardware_concurrency(), 4U);
            return Math.Min(Environment.ProcessorCount, 4);
        }


        //============================================================
        //  osd_thread_adjust_priority
        //============================================================
        // TODO - find workaround for .net standard 1.3
#if false
        static int thread_adjust_priority(Thread thread, ThreadPriority adjust)  // std::thread *thread, int adjust)
        {
#if false
//#if defined(OSD_WINDOWS) || defined(SDLMAME_WIN32)
            if (adjust)
                SetThreadPriority((HANDLE)thread->native_handle(), THREAD_PRIORITY_ABOVE_NORMAL);
            else
                SetThreadPriority((HANDLE)thread->native_handle(), GetThreadPriority(GetCurrentThread()));
//#endif
//#if defined(SDLMAME_LINUX) || defined(SDLMAME_BSD) || defined(SDLMAME_HAIKU) || defined(SDLMAME_DARWIN)
            struct sched_param  sched;
            int                 policy;

            if (pthread_getschedparam(thread->native_handle(), &policy, &sched) == 0)
            {
                sched.sched_priority += adjust;
                if (pthread_setschedparam(thread->native_handle(), policy, &sched) == 0)
                    return true;
                else
                    return false;
            }
//#endif
#endif

            thread.Priority = adjust;

            return 1;  //true;
        }
#endif


        //============================================================
        //  osd_work_queue_alloc
        //============================================================
        public static osd_work_queue osd_work_queue_alloc(int flags)
        {
            int threadnum;
            int numprocs = effective_num_processors();
            osd_work_queue queue;
            int osdthreadnum = 0;
            int allocthreadnum;
            string osdworkqueuemaxthreads = osdcore_global.m_osdcore.osd_getenv(ENV_WORKQUEUEMAXTHREADS);

            // allocate a new queue
            queue = new osd_work_queue();

            // initialize basic queue members
            queue.tailptr = queue.list;  //(osd_work_item **)&queue->list;
            queue.flags = (UInt32)flags;

            // force single thread
            numprocs = 1;

            // determine how many threads to create...
            // on a single-CPU system, create 1 thread for I/O queues, and 0 threads for everything else
            if (numprocs == 1)
                threadnum = (flags & osdcore_interface.WORK_QUEUE_FLAG_IO) != 0 ? 1 : 0;
            // on an n-CPU system, create n-1 threads for multi queues, and 1 thread for everything else
            else
                threadnum = (flags & osdcore_interface.WORK_QUEUE_FLAG_MULTI) != 0 ? (numprocs - 1) : 1;

            if (osdworkqueuemaxthreads != null && Int32.TryParse(osdworkqueuemaxthreads, out osdthreadnum) && threadnum > osdthreadnum)  // sscanf(osdworkqueuemaxthreads, "%d", &osdthreadnum) == 1
                threadnum = osdthreadnum;

            // clamp to the maximum
            queue.threads = (UInt32)Math.Min(threadnum, osdcore_interface.WORK_MAX_THREADS);

            // allocate memory for thread array (+1 to count the calling thread if WORK_QUEUE_FLAG_MULTI)
            if ((flags & osdcore_interface.WORK_QUEUE_FLAG_MULTI) != 0)
                allocthreadnum = (int)queue.threads + 1;
            else
                allocthreadnum = (int)queue.threads;

#if KEEP_STATISTICS
            printf("osdprocs: %d effecprocs: %d threads: %d allocthreads: %d osdthreads: %d maxthreads: %d queuethreads: %d\n", osd_num_processors, numprocs, threadnum, allocthreadnum, osdthreadnum, WORK_MAX_THREADS, queue->threads);
#endif

            for (threadnum = 0; threadnum < allocthreadnum; threadnum++)
                queue.thread.push_back(new work_thread_info((UInt32)threadnum, queue));

            // iterate over threads
            for (threadnum = 0; threadnum < queue.threads; threadnum++)
            {
                work_thread_info thread = queue.thread[threadnum];

                throw new emu_unimplemented();
#if false
                // create the thread
                thread.handle = new Thread(worker_thread_entry);  // new std::thread(worker_thread_entry, thread);
                thread.handle.IsBackground = true;
                if (thread.handle == null)
                    goto error;

                // set its priority: I/O threads get high priority because they are assumed to be
                // blocked most of the time; other threads just match the creator's priority
                if ((flags & osdcore_interface.WORK_QUEUE_FLAG_IO) != 0)
                    thread_adjust_priority(thread.handle, ThreadPriority.AboveNormal);
                else
                    thread_adjust_priority(thread.handle, thread.handle.Priority);

                thread.handle.Start(thread);
#endif
            }

            // start a timer going for "waittime" on the main thread
            if ((flags & osdcore_interface.WORK_QUEUE_FLAG_MULTI) != 0)
            {
#if KEEP_STATISTICS
                begin_timing(ref queue.thread[(int)queue.threads].waittime);
#endif
            }

            return queue;

error:
            osd_work_queue_free(queue);
            return null;
        }


        //============================================================
        //  osd_work_queue_wait
        //============================================================
        public static bool osd_work_queue_wait(osd_work_queue queue, osd_ticks_t timeout)
        {
            // if no threads, no waiting
            if (queue.threads == 0)
                return true;

            // if no items, we're done
            if (queue.items == 0)
                return true;

            // if this is a multi queue, help out rather than doing nothing
            if ((queue.flags & osdcore_interface.WORK_QUEUE_FLAG_MULTI) != 0)
            {
                work_thread_info thread = queue.thread[(int)queue.threads];

#if KEEP_STATISTICS
                end_timing(thread.waittime);
#endif

                // process what we can as a worker thread
                worker_thread_process(queue, thread);

                // if we're a high frequency queue, spin until done
                if ((queue.flags & osdcore_interface.WORK_QUEUE_FLAG_HIGH_FREQ) != 0 && queue.items != 0)
                {
                    // spin until we're done
#if KEEP_STATISTICS
                    begin_timing(thread.spintime);
#endif
                    spin_while_not(ref queue.items, 0, timeout);  //spin_while_not<std::atomic<int>,int>(&queue->items, 0, timeout);
#if KEEP_STATISTICS
                    end_timing(thread.spintime);
#endif

#if KEEP_STATISTICS
                    begin_timing(thread.waittime);
#endif
                    return queue.items == 0;
                }

#if KEEP_STATISTICS
                begin_timing(thread.waittime);
#endif
            }

            // reset our done event and double-check the items before waiting
            queue.doneevent.reset();
            queue.waiting = 1;  //true;
            if (queue.items != 0)
                queue.doneevent.wait(timeout);
            queue.waiting = 0;  //false;

            // return true if we actually hit 0
            return queue.items == 0;
        }


        //============================================================
        //  osd_work_queue_free
        //============================================================
        public static void osd_work_queue_free(osd_work_queue queue)
        {
            // stop the timer for "waittime" on the main thread
            if ((queue.flags & osdcore_interface.WORK_QUEUE_FLAG_MULTI) != 0)
            {
#if KEEP_STATISTICS
                end_timing(queue.thread[queue.threads]->waittime);
#endif
            }

            // signal all the threads to exit
            queue.exiting = 1;  //true;
            for (int threadnum = 0; threadnum < queue.threads; threadnum++)
            {
                work_thread_info thread = queue.thread[threadnum];
                thread.wakeevent.set();
            }

            // wait for all the threads to go away
            for (int threadnum = 0; threadnum < queue.threads; threadnum++)
            {
                work_thread_info thread = queue.thread[threadnum];

                // block on the thread going away, then close the handle
                if (thread.handle != null)
                {
                    thread.handle.Join();
                    thread.handle = null;  //delete thread->handle;
                }
            }

#if KEEP_STATISTICS
            // output per-thread statistics
            for (work_thread_info *thread : queue->thread)
            {
                osd_ticks_t total = thread->runtime + thread->waittime + thread->spintime;
                printf("Thread %d:  items=%9d run=%5.2f%% (%5.2f%%)  spin=%5.2f%%  wait/other=%5.2f%% total=%9d\n",
                        thread->id, thread->itemsdone,
                        (double)thread->runtime * 100.0 / (double)total,
                        (double)thread->actruntime * 100.0 / (double)total,
                        (double)thread->spintime * 100.0 / (double)total,
                        (double)thread->waittime * 100.0 / (double)total,
                        (uint32_t) total);
            }
#endif

            // free the list
            //foreach (var th in queue.thread)
                //delete th;
            queue.thread.clear();

            // free all items in the free list
            while (queue.free != null)  //.load() != nullptr)
            {
                var item = queue.free;  //auto *item = (osd_work_item *)queue->free;
                queue.free = item.next;
                item.event_ = null;  //delete item.event;
                item = null;  //delete item;
            }

            // free all items in the active list
            while (queue.list != null)  //.load() != nullptr)
            {
                var item = queue.list;  //auto *item = (osd_work_item *)queue->list;
                queue.list = item.next;
                item.event_ = null;  //delete item->event;
                item = null;  //delete item;
            }

#if KEEP_STATISTICS
            printf("Items queued   = %9d\n", queue->itemsqueued.load());
            printf("SetEvent calls = %9d\n", queue->setevents.load());
            printf("Extra items    = %9d\n", queue->extraitems.load());
            printf("Spin loops     = %9d\n", queue->spinloops.load());
#endif

            // free the queue itself
            //delete queue;
        }


        //============================================================
        //  osd_work_item_queue_multiple
        //============================================================
        public static osd_work_item osd_work_item_queue_multiple(osd_work_queue queue, osd_work_callback callback, int32_t numitems, List<Object> parambase, /*int paramstep,*/ uint32_t flags)
        {
            int32_t paramstep = 0;

            osd_work_item itemlist = null;  // osd_work_item *itemlist = nullptr, *lastitem = nullptr;
            osd_work_item lastitem = null;
            osd_work_item item_tailptr = itemlist;  // osd_work_item **item_tailptr = &itemlist;
            int itemnum;

            // loop over items, building up a local list of work
            for (itemnum = 0; itemnum < numitems; itemnum++)
            {
                osd_work_item item;

                // first allocate a new work item; try the free list first
                {
                    lock (queue.lockObj)  //std::lock_guard<std::mutex> lock(queue->lock);
                    {
                        do
                        {
                            item = queue.free;  //(osd_work_item *)queue->free;
                        } while (item != null && Interlocked.CompareExchange(ref item, item.next, item) == item);  //!queue.free.compare_exchange_weak(item, item.next, std::memory_order_release, std::memory_order_relaxed));
                    }
                }

                // if nothing, allocate something new
                if (item == null)
                {
                    // allocate the item
                    item = new osd_work_item(queue);
                    if (item == null)
                        return null;
                }
                else
                {
                    item.done = 0;  //false; // needs to be set this way to prevent data race/usage of uninitialized memory on Linux
                }

                // fill in the basics
                item.next = null;
                item.callback = callback;
                item.param = parambase[paramstep];  // parambase;
                item.result = null;
                item.flags = flags;

                // advance to the next
                lastitem = item;
                // *item_tailptr = item;
                // item_tailptr = &item->next;
                if (item_tailptr == null)
                {
                    itemlist = item;
                    item_tailptr = item;
                }
                else
                {
                    item_tailptr.next = item;
                    item_tailptr = item;
                }

                paramstep++;  //parambase = (uint8_t *)parambase + paramstep;
            }


            // enqueue the whole thing within the critical section
            {
                lock (queue.lockObj)  //std::lock_guard<std::mutex> lock(queue->lock);
                {
                    //*queue->tailptr = itemlist;
                    //queue->tailptr = item_tailptr;
                    if (queue.tailptr == null)
                    {
                        queue.tailptr = itemlist;
                        if (queue.list == null)
                        {
                            queue.list = queue.tailptr;
                        }
                    }
                    else
                    {
                        queue.tailptr.next = itemlist;
                        queue.tailptr = item_tailptr;
                    }
                }
            }

            // increment the number of items in the queue
            queue.items += numitems;
#if KEEP_STATISTICS
            add_to_stat(queue.itemsqueued, numitems);
#endif

            // look for free threads to do the work
            if (queue.livethreads < queue.threads)
            {
                int threadnum;

                // iterate over all the threads
                for (threadnum = 0; threadnum < queue.threads; threadnum++)
                {
                    work_thread_info thread = queue.thread[threadnum];

                    // if this thread is not active, wake him up
                    if (thread.active == 0)
                    {
                        thread.wakeevent.set();
#if KEEP_STATISTICS
                        add_to_stat(queue.setevents, 1);
#endif

                        // for non-shared, the first one we find is good enough
                        if (--numitems == 0)
                            break;
                    }
                }
            }

            // if no threads, run the queue now on this thread
            if (queue.threads == 0)
            {
#if KEEP_STATISTICS
                end_timing(queue.thread[0].waittime);
#endif
                worker_thread_process(queue, queue.thread[0]);
#if KEEP_STATISTICS
                begin_timing(queue.thread[0].waittime);
#endif
            }

            // only return the item if it won't get released automatically
            return (flags & osdcore_interface.WORK_ITEM_FLAG_AUTO_RELEASE) != 0 ? null : lastitem;
        }


        //============================================================
        //  osd_work_item_wait
        //============================================================
        static bool osd_work_item_wait(osd_work_item item, osd_ticks_t timeout)
        {
            // if we're done already, just return
            if (item.done != 0)
                return true;

            // if we don't have an event, create one
            if (item.event_ == null)
            {
                //std::lock_guard<std::mutex> lock(item->queue.lock);
                lock (item.queue.lockObj)
                    item.event_ = new osd_event(1, 0);  // true, false);     // manual reset, not signalled
            }
            else
            {
                item.event_.reset();
            }

            // if we don't have an event, we need to spin (shouldn't ever really happen)
            if (item.event_ == null)
            {
                // TODO: do we need to measure the spin time here as well? and how can we do it?
                spin_while(ref item.done, 0, timeout);  //spin_while<std::atomic<int>,int>(&item->done, 0, timeout);
            }

            // otherwise, block on the event until done
            else if (item.done == 0)
            {
                item.event_.wait(timeout);
            }

            // return true if the refcount actually hit 0
            return item.done != 0;
        }


        //============================================================
        //  osd_work_item_release
        //============================================================

        static void osd_work_item_release(osd_work_item item)
        {
            osd_work_item next;

            // make sure we're done first
            osd_work_item_wait(item, 100 * osdcore_global.m_osdcore.osd_ticks_per_second());

            // add us to the free list on our queue
            //throw new emu_unimplemented();
#if false
            lock (item.queue.lockObj)  //std::lock_guard<std::mutex> lock(item->queue.lock);
            {
                do
                {
                    next = item.queue.free;  //(osd_work_item *) item->queue.free;
                    item.next = next;
                } while (false/* doesn't work Interlocked.CompareExchange(ref item.queue.free, next, item) == item.queue.free*/);  //(!item.queue.free.compare_exchange_weak(next, item, std::memory_order_release, std::memory_order_relaxed));
            }
#endif
        }


        //============================================================
        //  effective_num_processors
        //============================================================
        static int effective_num_processors()
        {
            int physprocs = osd_get_num_processors();

            // osd_num_processors == 0 for 'auto'
            if (osd_num_processors > 0)
            {
                return Math.Min(4 * physprocs, osd_num_processors);
            }
            else
            {
                int numprocs = 0;

                // if the OSDPROCESSORS environment variable is set, use that value if valid
                // note that we permit more than the real number of processors for testing
                string procsoverride = osdcore_global.m_osdcore.osd_getenv(ENV_PROCESSORS);
                if (procsoverride != null && Int32.TryParse(procsoverride, out numprocs) && numprocs > 0)  // sscanf(procsoverride, "%d", &numprocs) == 1
                    return Math.Min(4 * physprocs, numprocs);

                // otherwise, return the info from the system
                return physprocs;
            }
        }


        //============================================================
        //  worker_thread_entry
        //============================================================
        static void worker_thread_entry(object param)
        {
            var thread = (work_thread_info)param;
            osd_work_queue queue = thread.queue;

            // loop until we exit
            for ( ;; )
            {
                // block waiting for work or exit
                // bail on exit, and only wait if there are no pending items in queue
                if (queue.exiting != 0)
                    break;

                if (!queue_has_list_items(queue))
                {
#if KEEP_STATISTICS
                    begin_timing(thread.waittime);
#endif
                    thread.wakeevent.wait( OSD_EVENT_WAIT_INFINITE);
#if KEEP_STATISTICS
                    end_timing(thread.waittime);
#endif
                }

                if (queue.exiting != 0)
                    break;

                // indicate that we are live
                thread.active = 1;  //true;
                ++queue.livethreads;

                // process work items
                for ( ;; )
                {
                    // process as much as we can
                    worker_thread_process(queue, thread);

                    // if we're a high frequency queue, spin for a while before giving up
                    if ((queue.flags & osdcore_interface.WORK_QUEUE_FLAG_HIGH_FREQ) != 0 && queue.list == null)  //.load() == nullptr)
                    {
                        // spin for a while looking for more work
#if KEEP_STATISTICS
                        begin_timing(thread.spintime);
#endif
                        spin_while(ref queue.list, null, SPIN_LOOP_TIME);  //spin_while<std::atomic<osd_work_item *>, osd_work_item *>(&queue.list, (osd_work_item *)nullptr, SPIN_LOOP_TIME);
#if KEEP_STATISTICS
                        end_timing(thread.spintime);
#endif
                    }

                    // if nothing more, release the processor
                    if (!queue_has_list_items(queue))
                        break;

#if KEEP_STATISTICS
                    add_to_stat(queue.spinloops, 1);
#endif
                }

                // decrement the live thread count
                thread.active = 0;  //false;
                --queue.livethreads;
            }

            //return nullptr;
        }


        //============================================================
        //  worker_thread_process
        //============================================================
        static void worker_thread_process(osd_work_queue queue, work_thread_info thread)
        {
            int threadid = (int)thread.id;

#if KEEP_STATISTICS
            begin_timing(thread.runtime);
#endif

            // loop until everything is processed
            while (true)
            {
                osd_work_item item = null;

                bool end_loop = false;

                // use a critical section to synchronize the removal of items
                {
                    //std::lock_guard<std::mutex> lock_(queue.lock_);
                    lock (queue.lockObj)
                    {
                        if (queue.list == null)  //.load() == null)
                        {
                            end_loop = true;
                        }
                        else
                        {
                            // pull the item from the queue
                            item = queue.list;  //(osd_work_item *)queue.list;
                            if (item != null)
                            {
                                queue.list = item.next;
                                if (queue.list == null)  //.load() == null)
                                    queue.tailptr = queue.list;  //(osd_work_item **)&queue.list;
                            }
                        }
                    }
                }

                if (end_loop)
                    break;

                // process non-NULL items
                if (item != null)
                {
                    // call the callback and stash the result
#if KEEP_STATISTICS
                    begin_timing(thread.actruntime);
#endif
                    item.result = item.callback(item.param, threadid);
#if KEEP_STATISTICS
                    end_timing(thread.actruntime);
#endif

                    // decrement the item count after we are done
                    --queue.items;
                    item.done = 1;  //true;
#if KEEP_STATISTICS
                    add_to_stat(thread.itemsdone, 1);
#endif

                    // if it's an auto-release item, release it
                    if ((item.flags & osdcore_interface.WORK_ITEM_FLAG_AUTO_RELEASE) != 0)
                        osd_work_item_release(item);

                    // set the result and signal the event
                    else
                    {
                        //std::lock_guard<std::mutex> lock_(queue.lock_);
                        lock (queue.lockObj)
                        {
                            if (item.event_ != null)
                            {
                                item.event_.set();
#if KEEP_STATISTICS
                                add_to_stat(item.queue.setevents, 1);
#endif
                            }
                        }
                    }

                    // if we removed an item and there's still work to do, bump the stats
#if KEEP_STATISTICS
                    if (queue_has_list_items(queue))
                        add_to_stat(queue.extraitems, 1);
#endif
                }
            }

            // we don't need to set the doneevent for multi queues because they spin
            if (queue.waiting != 0)
            {
                queue.doneevent.set();
#if KEEP_STATISTICS
                add_to_stat(queue.setevents, 1);
#endif
            }

#if KEEP_STATISTICS
            end_timing(thread.runtime);
#endif
        }


        static bool queue_has_list_items(osd_work_queue queue)
        {
            //std::lock_guard<std::mutex> lock(queue->lock);
            lock (queue.lockObj)
            {
                bool has_list_items = queue.list != null;  //.load() != nullptr);
                return has_list_items;
            }
        }
    }


    /* osd_event is an opaque type which represents a setable/resettable event */

    public class osd_event
    {
        Mutex m_mutex = new Mutex();  //std::mutex               m_mutex;
        ManualResetEvent m_cond;  //std::condition_variable  m_cond;
        int m_autoreset;  //std::atomic<int32_t>       m_autoreset;
        int m_signalled;  //std::atomic<int32_t>       m_signalled;


        /*-----------------------------------------------------------------------------
            constructor: allocate a new event

            Parameters:

                manualreset  - boolean. If true, the event will be automatically set
                               to non-signalled after a thread successfully waited for
                               it.
                initialstate - boolean. If true, the event is signalled initially.

            Return value:

                A pointer to the allocated event.
        -----------------------------------------------------------------------------*/
        public osd_event(int manualreset, int initialstate)
        {
            m_signalled = initialstate;
            m_autoreset = manualreset == 0 ? 1 : 0;


            m_cond = new ManualResetEvent(m_signalled != 0);
        }


        /*-----------------------------------------------------------------------------
            wait:   wait for an event to be signalled
                    If the event is in signalled state, the
                    function returns immediately. If not it will wait for the event
                    to become signalled.

            Parameters:

                timeout - timeout in osd_ticks

            Return value:

                true:  The event was signalled
                false: A timeout occurred
        -----------------------------------------------------------------------------*/
        public bool wait(osd_ticks_t timeout)
        {
            if (timeout == osdsync_global.OSD_EVENT_WAIT_INFINITE)
                timeout = osdcore_global.m_osdcore.osd_ticks_per_second() * (osd_ticks_t)10000;

            //std::unique_lock<std::mutex> lock(m_mutex);
            lock (m_mutex)
            {
                if (timeout == 0)
                {
                    if (m_signalled == 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (m_signalled == 0)
                    {
                        UInt64 msec = timeout * 1000 / osdcore_global.m_osdcore.osd_ticks_per_second();

                        do
                        {
                            //if (m_cond.wait_for(lock, std::chrono::milliseconds(msec)) == std::cv_status::timeout)
                            if (m_cond.WaitOne((int)msec))
                            {
                                if (m_signalled == 0)
                                {
                                    return false;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        } while (true);
                    }
                }

                if (m_autoreset != 0)
                    m_signalled = 0;

                return true;
            }
        }


        /*-----------------------------------------------------------------------------
            osd_event_reset: reset an event to non-signalled state

            Parameters:

                None

            Return value:

                None
        -----------------------------------------------------------------------------*/
        public void reset()
        {
            throw new emu_unimplemented();
        }


        /*-----------------------------------------------------------------------------
            osd_event_set: set an event to signalled state

            Parameters:

                None

            Return value:

                None

            Notes:

                All threads waiting for the event will be signalled.
        -----------------------------------------------------------------------------*/
        public void set()
        {
            m_mutex.WaitOne();  //m_mutex.lock();
            if (m_signalled == 0)  //false)
            {
                m_signalled = 1;  //true;
                if (m_autoreset != 0)
                    m_cond.Set();  //m_cond.notify_one();
                else
                    m_cond.Reset();  // ??? //m_cond.notify_all();
            }
            m_mutex.ReleaseMutex();  //m_mutex.unlock();
        }
    }


    public class work_thread_info
    {
        public osd_work_queue queue;          // pointer back to the queue
        public Thread handle;  //std::thread *       handle;         // handle to the thread
        public osd_event wakeevent;      // wake event for the thread
        public int active;  //std::atomic<int32_t>  active;         // are we actively processing work?
        public UInt32 id;

#if KEEP_STATISTICS
        int32_t               itemsdone;
        osd_ticks_t         actruntime;
        osd_ticks_t         runtime;
        osd_ticks_t         spintime;
        osd_ticks_t waittime;
#endif


        public work_thread_info(UInt32 aid, osd_work_queue aqueue)
        {
            queue = aqueue;

            handle = null;
            wakeevent = new osd_event(0, 0);  //(false, false);  // auto-reset, not signalled
            active = 0;
            id = aid;
#if KEEP_STATISTICS
            , itemsdone(0)
            , actruntime(0)
            , runtime(0)
            , spintime(0)
            , waittime(0)
#endif
        }
    }


    public class osd_work_queue
    {
        public object lockObj;  // std::mutex          lock;           // lock for protecting the queue
        public osd_work_item list;  //std::atomic<osd_work_item *> list;  // list of items in the queue
        public osd_work_item tailptr;  //osd_work_item ** volatile tailptr;  // pointer to the tail pointer of work items in the queue
        public osd_work_item free;  //std::atomic<osd_work_item *> free;  // free list of work items
        public int items;  //std::atomic<int32_t>  items;          // items in the queue
        public int livethreads;  //std::atomic<int32_t>  livethreads;    // number of live threads
        public int waiting;  //std::atomic<int32_t>  waiting;        // is someone waiting on the queue to complete?
        public int exiting;  //std::atomic<int32_t>  exiting;        // should the threads exit on their next opportunity?
        public UInt32 threads;        // number of threads in this queue
        public UInt32 flags;          // creation flags
        public std.vector<work_thread_info> thread = new std.vector<work_thread_info>();   // array of thread information
        public osd_event doneevent;      // event signalled when work is complete

#if KEEP_STATISTICS
        std::atomic<int32_t>  itemsqueued;    // total items queued
        std::atomic<int32_t>  setevents;      // number of times we called SetEvent
        std::atomic<int32_t>  extraitems;     // how many extra items we got after the first in the queue loop
        std::atomic<int32_t>  spinloops;      // how many times spinning bought us more items
#endif


        public osd_work_queue()
        {
            lockObj = new object();
            list = null;
            tailptr = null;
            free = null;
            items = 0;
            livethreads = 0;
            waiting = 0;
            exiting = 0;
            threads = 0;
            flags = 0;
            doneevent = new osd_event(1, 1);  //true, true);     // manual reset, signalled
#if KEEP_STATISTICS
            itemsqueued(0)
            setevents(0)
            extraitems(0)
            spinloops(0)
#endif
        }
    }


    public class osd_work_item
    {
        public osd_work_item next;           // pointer to next item
        public osd_work_queue queue;          // pointer back to the owning queue
        public osd_work_callback callback;       // callback function
        public object param;  //void *              param;          // callback parameter
        public object result;  //void *              result;         // callback result
        public osd_event event_;          // event signalled when complete
        public UInt32 flags;          // creation flags
        public int done;  //std::atomic<int32_t>  done;           // is the item done?


        public osd_work_item(osd_work_queue aqueue)
        {
            next = null;
            queue = aqueue;
            callback = null;
            param = null;
            result = null;
            event_ = null;                // manual reset, not signalled
            flags = 0;
            done = 0;  //false;
        }
    }
}
