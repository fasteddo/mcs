// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;
using netlist_time_ext = mame.plib.ptime_i64;  //netlist_time
using size_t = System.UInt32;


namespace mame.plib
{
    // ----------------------------------------------------------------------------------------
    // timed queue
    // ----------------------------------------------------------------------------------------

    // Note: Don't even try the following approach for element:
    //
    //    template <typename Time, typename Element>
    //    struct pqentry_t : public std::pair<Time, Element>
    //
    // This degrades performance significantly.

    //template <typename Time, typename Element>
    public class pqentry_t<Time, Element> where Time : IComparable
    {
        // Time == eg, netlist_time_ext
        // Element == eg, net_t


        Time m_exec_time;
        Element m_object;


        public pqentry_t() { m_exec_time = default; m_object = default; }  //constexpr pqentry_t() noexcept : m_exec_time(), m_object(nullptr) { }
        public pqentry_t(Time t, Element o) { m_exec_time = t;  m_object = o; }  //constexpr pqentry_t(Time t, Element o) noexcept : m_exec_time(t), m_object(o) { }


        //PCOPYASSIGNMOVE(pqentry_t, default)

        //~pqentry_t() = default;


        //constexpr bool operator ==(const pqentry_t &rhs) const noexcept

        //constexpr bool operator ==(const Element &rhs) const noexcept

        //constexpr bool operator <=(const pqentry_t &rhs) const noexcept

        //constexpr bool operator <(const pqentry_t &rhs) const noexcept

        public static bool operator ==(pqentry_t<Time, Element> lhs, pqentry_t<Time, Element> rhs) { return lhs.m_object.Equals(rhs.m_object); }
        public static bool operator !=(pqentry_t<Time, Element> lhs, pqentry_t<Time, Element> rhs) { return !lhs.m_object.Equals(rhs.m_object); }

        public static bool operator ==(pqentry_t<Time, Element> lhs, Element rhs) { return lhs.m_object.Equals(rhs); }
        public static bool operator !=(pqentry_t<Time, Element> lhs, Element rhs) { return !lhs.m_object.Equals(rhs); }

        public static bool operator >(pqentry_t<Time, Element> lhs, pqentry_t<Time, Element> rhs) { return lhs.m_exec_time.CompareTo(rhs.m_exec_time) > 0; }
        public static bool operator <(pqentry_t<Time, Element> lhs, pqentry_t<Time, Element> rhs) { return lhs.m_exec_time.CompareTo(rhs.m_exec_time) < 0; }


        static pqentry_t<Time, Element> never()  //static constexpr pqentry_t never() noexcept { return pqentry_t(Time::never(), nullptr); }
        {
            throw new emu_unimplemented();
#if false
            return new pqentry_t<Time, Element>(Time.never(), default);
#endif
        }

        public Time exec_time() { return m_exec_time; }  //constexpr Time exec_time() const noexcept { return m_exec_time; }
        public Element object_() { return m_object; }  //constexpr Element object() const noexcept { return m_object; }
    }


    // Use TS = true for a threadsafe queue
    //template <class T, bool TS>
    public class timed_queue_linear_pqentry_t_netlist_time_ext_net_t
    {
        //using mutex_type       = pspin_mutex<TS>;
        //using lock_guard_type  = std::lock_guard<mutex_type>;


        object m_lock = new object();  //mutex_type               m_lock;
        //PALIGNAS_CACHELINE()
        int m_endIdx;  //T *                      m_end;
        aligned_vector<pqentry_t<netlist_time_ext, netlist.detail.net_t>> m_list;  //aligned_vector<T> m_list;


        // profiling
        // FIXME: Make those private
        //pperfcount_t<true> m_prof_sortmove; // NOLINT
        //pperfcount_t<true> m_prof_call; // NOLINT
        //pperfcount_t<true> m_prof_remove; // NOLINT
        //pperfcount_t<true> m_prof_retime; // NOLINT


        protected timed_queue_linear_pqentry_t_netlist_time_ext_net_t(size_t list_size)
        {
            m_list = new aligned_vector<pqentry_t<netlist_time_ext, netlist.detail.net_t>>(list_size);


            clear();
        }


        //~timed_queue_linear() = default;

        //PCOPYASSIGNMOVE(timed_queue_linear, delete)


        //std::size_t capacity() const noexcept { return m_list.capacity() - 1; }
        //bool empty() const noexcept { return (m_end == &m_list[1]); }

        //template<bool KEEPSTAT, typename... Args>
        public void emplace(bool KEEPSTAT, pqentry_t<netlist_time_ext, netlist.detail.net_t> e)  //void emplace(Args&&... args) noexcept
        {
            // Lock
            lock (m_lock)  //lock_guard_type lck(m_lock);
            {
                int iIdx = m_endIdx++;  //T * i(m_end++);
                m_list[iIdx] = e;  //*i = T(std::forward<Args>(args)...);

                if (!KEEPSTAT)
                {
                    for (; m_list[iIdx - 1] < m_list[iIdx]; --iIdx)  //for (; *(i-1) < *i; --i)
                    {
                        //std::swap(*(i-1), *(i));
                        var temp = m_list[iIdx - 1];
                        m_list[iIdx - 1] = m_list[iIdx];
                        m_list[iIdx] = temp;
                    }
                }
                else
                {
                    for (; m_list[iIdx - 1] < m_list[iIdx]; --iIdx)  //for (; *(i-1) < *i; --i)
                    {
                        //std::swap(*(i-1), *(i));
                        var temp = m_list[iIdx - 1];
                        m_list[iIdx - 1] = m_list[iIdx];
                        m_list[iIdx] = temp;

                        //throw new emu_unimplemented();
#if false
                        m_prof_sortmove.inc();
#endif
                    }

                        //throw new emu_unimplemented();
#if false
                    m_prof_call.inc();
#endif
                }
            }
        }


        //template<bool KEEPSTAT>
        //void push(T && e) noexcept


        public void pop() { --m_endIdx; }  //void pop() noexcept       { --m_end; }
        public pqentry_t<netlist_time_ext, netlist.detail.net_t> top() { return m_list[m_endIdx - 1]; }  //const T &top() const noexcept { return *(m_end-1); }


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


        //template <bool KEEPSTAT, class R>
        //void retime(R && elem) noexcept


        public void clear()
        {
            lock (m_lock)  //lock_guard_type lck(m_lock);
            {
                m_endIdx = 0;  //m_end = &m_list[0];
                // put an empty element with maximum time into the queue.
                // the insert algo above will run into this element and doesn't
                // need a comparison with queue start.
                //
                throw new emu_unimplemented();
#if false
                m_list[0] = T.never();  //m_list[0] = T::never();
#endif
                m_endIdx++;  //m_end++;
            }
        }


        // save state support & mame disasm

        //const T *listptr() const noexcept { return &m_list[1]; }
        //std::size_t size() const noexcept { return narrow_cast<std::size_t>(m_end - &m_list[1]); }
        //const T & operator[](std::size_t index) const noexcept { return m_list[ 1 + index]; }
    }


    //template <class T, bool TS>
    //class timed_queue_heap
} // namespace plib
