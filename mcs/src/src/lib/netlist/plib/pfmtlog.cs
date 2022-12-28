// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.cpp_global;
using static mame.plib.pfmtlog_global;


namespace mame.plib
{
    public static class pfmtlog_global
    {
        //#define PERRMSGV(name, argument_count, str) \
        //    struct name : public plib::perrmsg \
        //    { \
        //        template<typename... Args> explicit name(Args&&... args) \
        //        : plib::perrmsg(str, std::forward<Args>(args)...) \
        //        { static_assert(argument_count == sizeof...(args), "Argument count mismatch"); } \
        //    };
        public static string PERRMSGV(int argument_count, string format, params object [] args) { static_assert(argument_count == args.Length, "Argument count mismatch"); return String.Format(format, args); }
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

    public interface plog_level_constant { plog_level value { get; } }
    public class plog_level_constant_DEBUG : plog_level_constant { public plog_level value { get { return plog_level.DEBUG; } } }
    public class plog_level_constant_VERBOSE : plog_level_constant { public plog_level value { get { return plog_level.VERBOSE; } } }
    public class plog_level_constant_INFO : plog_level_constant { public plog_level value { get { return plog_level.INFO; } } }
    public class plog_level_constant_WARNING : plog_level_constant { public plog_level value { get { return plog_level.WARNING; } } }
    public class plog_level_constant_ERROR : plog_level_constant { public plog_level value { get { return plog_level.ERROR; } } }
    public class plog_level_constant_FATAL : plog_level_constant { public plog_level value { get { return plog_level.FATAL; } } }



    public class pfmt
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
        //    return format_element(format_traits<T>::size_spec(), format_traits<T>::fmt_spec(), format_traits<T>::cast(x));
        //}

        //template<typename T>
        //pfmt &operator ()(const T *x)
        //{
        //    return format_element(format_traits<T *>::size_spec(), format_traits<T *>::fmt_spec(), format_traits<T *>::cast(x));
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
    public abstract class pfmt_writer_t<T, bool_build_enabled>
        where bool_build_enabled : bool_const, new()
    {
        static readonly bool build_enabled = new bool_build_enabled().value;


        bool m_enabled;


        public pfmt_writer_t() { m_enabled = true; }

        //COPYASSIGNMOVE(pfmt_writer_t, delete)

        //~pfmt_writer_t() noexcept = default;


        protected abstract void upstream_write(string ls);


        // runtime enable
        //template<bool enabled, typename... Args>
        //void log(const pstring & fmt, Args&&... args) const
        //{
        //    if (build_enabled && enabled && m_enabled)
        //    {
        //        pfmt pf(fmt);
        //        plib::dynamic_downcast<T &>(*this).upstream_write(log_translate(pf, std::forward<Args>(args)...));
        //    }
        //}

        //template<typename... Args>
        //void operator ()(const pstring &fmt, Args&&... args) const
        //{
        //    if (build_enabled && m_enabled)
        //    {
        //        pfmt pf(fmt);
        //        static_cast<const T &>(*this).upstream_write(log_translate(pf, std::forward<Args>(args)...));
        //    }
        //}
        public void op(string format, params object [] args)
        {
            if (build_enabled && m_enabled)
            {
                string s = string.Format(format, args);
                upstream_write(s);  //static_cast<const T &>(*this).upstream_write(log_translate(pf, std::forward<Args>(args)...));
            }
        }

        public void set_enabled(bool v)
        {
            m_enabled = v;
        }

        //bool is_enabled() const { return m_enabled; }

        //pfmt &log_translate(pfmt &fmt) const { return fmt; }

        //template<typename X, typename... Args>
        //pfmt &log_translate(pfmt &fmt, X&& x, Args&&... args) const
        //{
        //    return log_translate(fmt(std::forward<X>(x)), std::forward<Args>(args)...);
        //}
    }


    public abstract class pfmt_writer_t<T> : pfmt_writer_t<T, bool_const_true>
    {
    }


    public delegate void plog_delegate(plog_level level, string log);  //using plog_delegate = plib::pmfp<void (plog_level, const pstring &)>;


    public class plog_channel<plog_level_L> : plog_channel<plog_level_L, bool_const_true>
        where plog_level_L : plog_level_constant, new()
    {
        public plog_channel(plog_delegate logger) : base(logger) { }
    }

    //template <plog_level::E L, bool build_enabled = true>
    public class plog_channel<plog_level_L, bool_build_enabled> : pfmt_writer_t<plog_channel<plog_level_L, bool_build_enabled>, bool_build_enabled>  //class plog_channel : public pfmt_writer_t<plog_channel<L, build_enabled>, build_enabled>
        where plog_level_L : plog_level_constant, new()
        where bool_build_enabled : bool_const, new()
    {
        static readonly plog_level L = new plog_level_L().value;
        static readonly bool build_enabled = new bool_build_enabled().value;


        //friend class pfmt_writer_t<plog_channel<T, L, build_enabled>, build_enabled>;


        plog_delegate m_logger;


        public plog_channel(plog_delegate logger)
            : base()  //: pfmt_writer_t<plog_channel, build_enabled>()
        {
            m_logger = logger;
        }

        //PCOPYASSIGNMOVE(plog_channel, delete)

        //~plog_channel() noexcept = default;


        protected override void upstream_write(string ls)
        {
            m_logger(L, ls);
        }
    }


    //template<bool debug_enabled>
    public class plog_base<bool_debug_enabled>
        where bool_debug_enabled : bool_const, new()
    {
        static readonly bool debug_enabled = new bool_debug_enabled().value;


        public plog_channel<plog_level_constant_DEBUG, bool_debug_enabled> debug;
        public plog_channel<plog_level_constant_INFO> info;
        public plog_channel<plog_level_constant_VERBOSE> verbose;
        public plog_channel<plog_level_constant_WARNING> warning;
        public plog_channel<plog_level_constant_ERROR> error;
        public plog_channel<plog_level_constant_FATAL> fatal;


        public plog_base(plog_delegate logger)
        {
            debug = new plog_channel<plog_level_constant_DEBUG, bool_debug_enabled>(logger);
            info = new plog_channel<plog_level_constant_INFO>(logger);
            verbose = new plog_channel<plog_level_constant_VERBOSE>(logger);
            warning = new plog_channel<plog_level_constant_WARNING>(logger);
            error = new plog_channel<plog_level_constant_ERROR>(logger);
            fatal = new plog_channel<plog_level_constant_FATAL>(logger);
        }

        //PCOPYASSIGNMOVE(plog_base, default)
        //virtual ~plog_base() noexcept = default;
    }
}
