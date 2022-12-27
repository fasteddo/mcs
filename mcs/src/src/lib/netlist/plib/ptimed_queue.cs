// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<config::prefer_int128::value && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using size_t = System.UInt64;


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
    public class pqentry_t<Time, Element> : IComparable<pqentry_t<Time, Element>>
        where Time : netlist_time_ext
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

        public static bool operator ==(pqentry_t<Time, Element> lhs, pqentry_t<Time, Element> rhs) { return lhs == rhs.m_object; }
        public static bool operator !=(pqentry_t<Time, Element> lhs, pqentry_t<Time, Element> rhs) { return !(lhs == rhs.m_object); }

        public static bool operator ==(pqentry_t<Time, Element> lhs, Element rhs)
        {
            if (lhs.m_object == null && rhs == null)
                return true;
            else if (lhs.m_object == null && rhs != null)
                return false;
            else if (lhs.m_object != null && rhs == null)
                return false;
            else
                return lhs.m_object.Equals(rhs);
        }
        public static bool operator !=(pqentry_t<Time, Element> lhs, Element rhs) { return !lhs.m_object.Equals(rhs); }

        public static bool operator >(pqentry_t<Time, Element> lhs, pqentry_t<Time, Element> rhs) { return lhs.m_exec_time.CompareTo(rhs.m_exec_time) > 0; }
        public static bool operator <(pqentry_t<Time, Element> lhs, pqentry_t<Time, Element> rhs) { return lhs.m_exec_time.CompareTo(rhs.m_exec_time) < 0; }

        public override bool Equals(Object obj)
        {
            if (obj == null || base.GetType() != obj.GetType()) return false;
            return this == (pqentry_t<Time, Element>)obj;
        }
        public override int GetHashCode() { return m_object.GetHashCode(); }
        public int CompareTo(pqentry_t<Time, Element> other) { return m_exec_time.CompareTo(other.m_exec_time); }


        public static pqentry_t<Time, Element> never()  //static constexpr pqentry_t never() noexcept { return pqentry_t(Time::never(), nullptr); }
        {
            return new pqentry_t<Time, Element>((Time)netlist_time_ext.never(), default);
        }

        public Time exec_time() { return m_exec_time; }  //constexpr Time exec_time() const noexcept { return m_exec_time; }
        public Element object_() { return m_object; }  //constexpr Element object() const noexcept { return m_object; }
    }


    // Use TS = true for a threadsafe queue
    //template <class T, bool TS>
    public class timed_queue_linear<T, bool_TS, U, V>
        where T : pqentry_t<U, V>
        where bool_TS : bool_const, new()
        where U : netlist_time_ext
    {
        //using mutex_type       = pspin_mutex<TS>;
        //using lock_guard_type  = std::lock_guard<mutex_type>;


        object m_lock = new object();  //mutex_type               m_lock;
        //T *                      m_end;
        SortedSet<T> m_list;  //aligned_vector<T> m_list;


        // profiling
        // FIXME: Make those private
        //pperfcount_t<true> m_prof_sortmove; // NOLINT
        //pperfcount_t<true> m_prof_call; // NOLINT
        //pperfcount_t<true> m_prof_remove; // NOLINT


        protected timed_queue_linear(size_t list_size)
        {
            m_list = new SortedSet<T>();  //m_list(list_size)


            clear();
        }

        //~timed_queue_linear() = default;

        //PCOPYASSIGNMOVE(timed_queue_linear, delete)

        //std::size_t capacity() const noexcept { return m_list.capacity() - 1; }


        public bool empty() { return m_list.Count == 1; }  //bool empty() const noexcept { return (m_end == &m_list[1]); }


        //template<bool KEEPSTAT, typename... Args>
        public void emplace<bool_KEEPSTAT>(T e)  //void emplace(Args&&... args) noexcept
        {
            // Lock
            //lock_guard_type lck(m_lock);
            //T * i(m_end++);
            //*i = T(std::forward<Args>(args)...);
            //
            //if (!KEEPSTAT)
            //{
            //    for (; *(i-1) < *i; --i)
            //    {
            //        std::swap(*(i-1), *(i));
            //    }
            //}
            //else
            //{
            //    for (; *(i-1) < *i; --i)
            //    {
            //        std::swap(*(i-1), *(i));
            //        m_prof_sortmove.inc();
            //    }
            //    m_prof_call.inc();
            //}

            lock (m_lock)
            {
                m_list.Add(e);
            }
        }


        //template<bool KEEPSTAT>
        public void push<bool_KEEPSTAT>(T e)  //void push(T && e) noexcept
        {
            // Lock
            //lock_guard_type lck(m_lock);
            //T * i(m_end++);
            //*i = std::move(e);
            //for (; *(i-1) < *i; --i)
            //{
            //    std::swap(*(i-1), *(i));
            //    if (KEEPSTAT)
            //        m_prof_sortmove.inc();
            //}
            //
            //if (KEEPSTAT)
            //    m_prof_call.inc();

            emplace<bool_KEEPSTAT>(e);
        }


        public void pop() { m_list.Remove(top()); }  //void pop() noexcept       { --m_end; }


        public T top() { return m_list.Reverse().ElementAt(1); }  //const T &top() const noexcept { return *(m_end-1); }


        //template <bool KEEPSTAT, class R>
        public void remove<bool_KEEPSTAT>(V elem)  //void remove(const R &elem) noexcept
        {
            // Lock
            //lock_guard_type lck(m_lock);
            //if (KEEPSTAT)
            //    m_prof_remove.inc();
            //for (T * i = m_end - 1; i > &m_list[0]; --i)
            //{
            //    // == operator ignores time!
            //    if (*i == elem)
            //    {
            //        std::copy(i+1, m_end--, i);
            //        return;
            //    }
            //}

            lock (m_lock)
            {
                bool first = true;
                foreach (var i in m_list.Reverse())
                {
                    if (first)
                    {
                        first = false;
                        continue;
                    }

                    if (i == elem)
                    {
                        m_list.Remove(i);
                        break;
                    }
                }
            }
        }


        public void clear()
        {
            lock (m_lock)  //lock_guard_type lck(m_lock);
            {
                //m_end = &m_list[0];
                // put an empty element with maximum time into the queue.
                // the insert algo above will run into this element and doesn't
                // need a comparison with queue start.
                //
                //m_list[0] = T::never();
                //m_end++;
                m_list.Clear();
                m_list.Add((T)pqentry_t<U, V>.never());
            }
        }


        // save state support & mame disasm

        //const T *listptr() const noexcept { return &m_list[1]; }


        public size_t size()  //std::size_t size() const noexcept { return narrow_cast<std::size_t>(m_end - &m_list[1]); }
        {
            throw new emu_unimplemented();
#if false
            { return narrow_cast<std::size_t>(m_end - &m_list[1]); }
#endif
        }


        //const T & operator[](std::size_t index) const noexcept { return m_list[ 1 + index]; }
    }


    //template <class T, bool TS>
    //class timed_queue_heap
} // namespace plib
