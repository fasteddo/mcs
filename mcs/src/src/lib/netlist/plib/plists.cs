// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Diagnostics;

using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;
using timed_queue = mame.plib.timed_queue_linear;


namespace mame.plib
{
    //template <class Element, class Time>
    public class pqentry_t<Element, Time>
    {
        netlist_time m_exec_time;  //Time m_exec_time;
        Element m_object;


        public pqentry_t() { m_exec_time = new netlist_time();  m_object = default; }  //constexpr pqentry_t() noexcept : m_exec_time(), m_object(nullptr) { }
        public pqentry_t(netlist_time t, Element o) { m_exec_time = t;  m_object = o; }  //constexpr pqentry_t(const Time &t, const Element &o) noexcept : m_exec_time(t), m_object(o) { }
        //~pqentry_t() = default;
        //constexpr pqentry_t(const pqentry_t &e) noexcept = default;
        //constexpr pqentry_t(pqentry_t &&e) = default;


        public static bool operator ==(pqentry_t<Element, Time> lhs, pqentry_t<Element, Time> rhs) { return lhs.m_object.Equals(rhs.m_object); }
        public static bool operator !=(pqentry_t<Element, Time> lhs, pqentry_t<Element, Time> rhs) { return !lhs.m_object.Equals(rhs.m_object); }

        public static bool operator ==(pqentry_t<Element, Time> lhs, Element rhs) { return lhs.m_object.Equals(rhs); }
        public static bool operator !=(pqentry_t<Element, Time> lhs, Element rhs) { return !lhs.m_object.Equals(rhs); }

        //inline bool operator <=(const pqentry_t &rhs) const noexcept
        //{
        //    return (m_exec_time <= rhs.m_exec_time);
        //}

        public static bool operator >(pqentry_t<Element, Time> lhs, pqentry_t<Element, Time> rhs) { return lhs.m_exec_time > rhs.m_exec_time; }
        public static bool operator <(pqentry_t<Element, Time> lhs, pqentry_t<Element, Time> rhs) { return lhs.m_exec_time < rhs.m_exec_time; }


        public static pqentry_t<Element, Time> never() { return new pqentry_t<Element, Time>(netlist_time.never(), default); }


        public netlist_time exec_time() { return m_exec_time; }  //Time exec_time() const noexcept { return m_exec_time; }
        public Element object_()  { return m_object; }
    }


    /* Use TS = true for a threadsafe queue */
    //template <class T, bool TS, bool KEEPSTAT, class QueueOp = typename T::QueueOp>
    public class timed_queue_linear  //: plib::nocopyassignmove
    {
        //using mutex_type = pspin_mutex<TS>;
        //using lock_guard_type = std::lock_guard<mutex_type>;


        // template parameters
        bool TS;


        object m_lock = new object();  //mutex_type      m_lock;
        int m_endIdx;  //T * m_end;
        std.vector<pqentry_t<netlist.detail.net_t, netlist_time>> m_list;  //std::vector<T> m_list;


        // profiling
        //nperfcount_t<KEEPSTAT> m_prof_sortmove;
        //nperfcount_t<KEEPSTAT> m_prof_call;
        //nperfcount_t<KEEPSTAT> m_prof_remove;
        //nperfcount_t<KEEPSTAT> m_prof_retime;


        public timed_queue_linear(bool TS, bool KEEPSTAT, UInt32 list_size)
        {
            this.TS = TS;


            m_list = new std.vector<pqentry_t<netlist.detail.net_t, netlist_time>>(list_size);
            for (int i = 0; i < list_size; i++)
                m_list[i] = new pqentry_t<netlist.detail.net_t, netlist_time>();


            clear();
        }


        //constexpr std::size_t capacity() const noexcept { return m_list.capacity() - 1; }
        //constexpr bool empty() const noexcept { return (m_end == &m_list[1]); }


        //template<bool KEEPSTAT>
        public void push(bool KEEPSTAT, pqentry_t<netlist.detail.net_t, netlist_time> e)  //void push(T && e) noexcept
        {
            /* Lock */
            lock (m_lock)  //lock_guard_type lck(m_lock);
            {
                int iIdx = m_endIdx++;  //T * i(m_end++);
                m_list[iIdx] = e;  //*i = std::move(e);
                for (; m_list[iIdx - 1] < m_list[iIdx]; --iIdx)  //for (; *(i-1) < *i; --i)
                {
                    //std::swap(*(i-1), *(i));
                    var temp = m_list[iIdx - 1];
                    m_list[iIdx - 1] = m_list[iIdx];
                    m_list[iIdx] = temp;

                    //throw new emu_unimplemented();
#if false
                    if (KEEPSTAT)
                        m_prof_sortmove.inc();
#endif
                }

                //throw new emu_unimplemented();
#if false
                if (KEEPSTAT)
                    m_prof_call.inc();
#endif
            }
        }


        public pqentry_t<netlist.detail.net_t, netlist_time> pop() { return m_list[--m_endIdx]; }  //{ return *(--m_end); }
        public pqentry_t<netlist.detail.net_t, netlist_time> top() { return m_list[m_endIdx - 1]; }  //{ return *(m_end-1); }


        //template <bool KEEPSTAT, class R>
        public void remove(bool KEEPSTAT, netlist.detail.net_t elem)  //void remove(const R &elem) noexcept
        {
            // Lock
            lock (m_lock)  //lock_guard_type lck(m_lock);
            {
                //throw new emu_unimplemented();
#if false
                if (KEEPSTAT)
                    m_prof_remove.inc();
#endif

                for (int iIdx = m_endIdx - 1; m_list[iIdx] > m_list[0]; --iIdx)  //for (T * i = m_end - 1; i > &m_list[0]; --i)
                {
                    // == operator ignores time!
                    if (m_list[iIdx] == elem)  //if (*i == elem)
                    {
                        m_list.CopyTo(iIdx + 1, m_list, iIdx, m_endIdx-- - (iIdx + 1));  //std::copy(i+1, m_end--, i);
                        return;
                    }
                }
            }
        }


        //void retime(const T &elem) noexcept
        //{
        //    /* Lock */
        //    tqlock lck(m_lock);
        //    for (T * i = m_end - 1; i > &m_list[0]; --i)
        //    {
        //        if (QueueOp::equal(*i, elem)) // partial equal!
        //        {
        //            *i = elem;
        //            while (QueueOp::less(*(i-1), *i))
        //            {
        //                std::swap(*(i-1), *i);
        //                --i;
        //            }
        //            while (i < m_end && QueueOp::less(*i, *(i+1)))
        //            {
        //                std::swap(*(i+1), *i);
        //                ++i;
        //            }
        //            return;
        //        }
        //    }
        //}

        public void clear()
        {
            lock (m_lock)  //lock_guard_type lck(m_lock);
            {
                m_endIdx = 0;  //m_end = m_list[0];
                // put an empty element with maximum time into the queue.
                // the insert algo above will run into this element and doesn't
                // need a comparison with queue start.
                //
                m_list[0] = pqentry_t<netlist.detail.net_t, netlist_time>.never();  //m_list[0] = T::never();
                m_endIdx++;  //m_end++;
            }
        }

        // save state support & mame disasm

        //const T *listptr() const noexcept { return &m_list[1]; }
        //std::size_t size() const noexcept { return static_cast<std::size_t>(m_end - &m_list[1]); }
        //const T & operator[](const std::size_t index) const noexcept { return m_list[ 1 + index]; }
    }
}
