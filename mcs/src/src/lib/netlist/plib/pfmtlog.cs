// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    public static class pfmtlog_global
    {
        //#define PERRMSGV(name, narg, str) \
        //    struct name : public plib::perrmsg \
        //    { \
        //        template<typename... Args> explicit name(Args&&... args) \
        //        : plib::perrmsg(str, std::forward<Args>(args)...) \
        //        { static_assert(narg == sizeof...(args), "Argument count mismatch"); } \
        //    };
        public static string PERRMSGV(int narg, string format, params object [] args) { global_object.static_assert(narg == args.Length, "Argument count mismatch"); return String.Format(format, args); }
    }


    public enum plog_level
    {
        DEBUG,
        VERBOSE,
        INFO,
        WARNING,
        ERROR,
        FATAL
    }


    class pfmt
    {
        string m_str;
        UInt32 m_arg;


        public pfmt(string fmt)
        {
            m_str = fmt;
            m_arg = 0;
        }


        //pfmt &operator ()(const void *x) {return format_element("", 'p', x);  }
        //pfmt &operator ()(const pstring &x) {return format_element("", 's', x.c_str() );  }
        public string op(params object [] x) { return string.Format(m_str, x); }

        //template<typename T>
        //pfmt &operator ()(const T &x)
        //{
        //    return format_element(ptype_traits<T>::size_spec(), ptype_traits<T>::fmt_spec(), ptype_traits<T>::cast(x));
        //}

        //template<typename T>
        //pfmt &operator ()(const T *x)
        //{
        //    return format_element(ptype_traits<T *>::size_spec(), ptype_traits<T *>::fmt_spec(), ptype_traits<T *>::cast(x));
        //}

        //pfmt &operator ()()
        //{
        //    return *this;
        //}

        //template<typename X, typename Y, typename... Args>
        //pfmt &operator()(X&& x, Y && y, Args&&... args)
        //{
        //    return ((*this)(std::forward<X>(x)))(std::forward<Y>(y), std::forward<Args>(args)...);
        //}
    }


    //template <class T, bool build_enabled = true>
    public abstract class pfmt_writer_t<T>
    {
        // template parameters
        bool build_enabled;


        bool m_enabled;


        public pfmt_writer_t(bool build_enabled = true) { this.build_enabled = build_enabled;  m_enabled = true; }

        //COPYASSIGNMOVE(pfmt_writer_t, delete)

        //~pfmt_writer_t() noexcept = default;


        protected abstract void vdowrite(string ls);


        /* runtime enable */
        //template<bool enabled, typename... Args>
        //void log(const pstring & fmt, Args&&... args) const
        //{
        //    if (build_enabled && enabled && m_enabled)
        //    {
        //        pfmt pf(fmt);
        //        static_cast<T *>(this)->vdowrite(xlog(pf, std::forward<Args>(args)...));
        //    }
        //}

        //template<typename... Args>
        //void operator ()(const pstring &fmt, Args&&... args) const
        //{
        //    if (build_enabled && m_enabled)
        //    {
        //        pfmt pf(fmt);
        //        static_cast<const T *>(this)->vdowrite(xlog(pf, std::forward<Args>(args)...));
        //    }
        //}
        public void op(string format, params object [] args)
        {
            if (build_enabled && m_enabled)
            {
                string s = string.Format(format, args);
                vdowrite(s);  //static_cast<const T *>(this)->vdowrite(xlog(pf, std::forward<Args>(args)...));
            }
        }

        public void set_enabled(bool v)
        {
            m_enabled = v;
        }

        //bool is_enabled() const { return m_enabled; }

        //pfmt &xlog(pfmt &fmt) const { return fmt; }

        //template<typename X, typename... Args>
        //pfmt &xlog(pfmt &fmt, X&& x, Args&&... args) const
        //{
        //    return xlog(fmt(std::forward<X>(x)), std::forward<Args>(args)...);
        //}
    }


    //template <class T, plog_level::E L, bool build_enabled = true>
    public class plog_channel<T> : pfmt_writer_t<plog_channel<T>> where T : netlist.callbacks_t  //pfmt_writer_t<plog_channel<T, L, build_enabled>, build_enabled>
    {
        //friend class pfmt_writer_t<plog_channel<T, L, build_enabled>, build_enabled>;


        // template parameters
        plog_level L;
        bool build_enabled = true;

        T m_base;


        public plog_channel(T b, plog_level L, bool build_enabled = true) : base(build_enabled) {this.L = L;  this.build_enabled = build_enabled;  m_base = b; }

        //COPYASSIGNMOVE(plog_channel, delete)

        //~plog_channel() noexcept = default;


        protected override void vdowrite(string ls)
        {
            m_base.vlog(L, ls);
        }
    }


    //template<class T, bool debug_enabled>
    public class plog_base<T> where T : netlist.callbacks_t
    {
        // template parameter
        bool debug_enabled;


        public plog_channel<T> debug;  //plog_channel<T, plog_level::DEBUG, debug_enabled> debug;
        public plog_channel<T> info;  //plog_channel<T, plog_level::INFO> info;
        public plog_channel<T> verbose;  //plog_channel<T, plog_level::VERBOSE> verbose;
        public plog_channel<T> warning;  //plog_channel<T, plog_level::WARNING> warning;
        public plog_channel<T> error;  //plog_channel<T, plog_level::ERROR> error;
        public plog_channel<T> fatal;  //plog_channel<T, plog_level::FATAL> fatal;


        public plog_base(bool debug_enabled, T proxy)
        {
            debug = new plog_channel<T>(proxy, plog_level.DEBUG, debug_enabled);
            info = new plog_channel<T>(proxy, plog_level.INFO);
            verbose = new plog_channel<T>(proxy, plog_level.VERBOSE);
            warning = new plog_channel<T>(proxy, plog_level.WARNING);
            error = new plog_channel<T>(proxy, plog_level.ERROR);
            fatal = new plog_channel<T>(proxy, plog_level.FATAL);
        }

        //PCOPYASSIGNMOVE(plog_base, default)
        //virtual ~plog_base() noexcept = default;
    }
}
