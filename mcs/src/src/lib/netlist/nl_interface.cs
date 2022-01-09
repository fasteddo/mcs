// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;
using param_logic_t = mame.netlist.param_num_t<bool, mame.netlist.param_num_t_operators_bool>;  //using param_logic_t = param_num_t<bool>;
using param_num_t_operators_size_t = mame.netlist.param_num_t_operators_uint64;
using size_t = System.UInt64;


namespace mame.netlist.interface_
{
    /// \brief analog_callback device
    ///
    /// This device is used to call back into the application which
    /// is controlling the execution of the netlist.
    ///
    /// The device will call the provided lambda with a reference to
    /// itself and the current value of the net it is connected to.
    ///
    /// The following code is an example on how to add the device to
    /// the netlist factory.
    ///
    ///     const pstring pin(m_in);
    ///     pstring dname = pstring("OUT_") + pin;
    ///
    ///     const auto lambda = [this](auto &in, netlist::nl_fptype val)
    ///     {
    ///         this->cpu()->update_icount(in.exec().time());
    ///         this->m_delegate(val, this->cpu()->local_time());
    ///         this->cpu()->check_mame_abort_slice();
    ///     };
    ///
    ///     using lb_t = decltype(lambda);
    ///     using cb_t = netlist::interface::NETLIB_NAME(analog_callback)<lb_t>;
    ///
    ///     parser.factory().add<cb_t, netlist::nl_fptype, lb_t>(dname,
    ///         netlist::factory::properties("-", PSOURCELOC()), 1e-6, std::forward<lb_t>(lambda));
    ///

    //template <typename FUNC>
    class nld_analog_callback : device_t  //NETLIB_OBJECT(analog_callback)
    {
        public delegate void FUNC(device_t device, nl_fptype val);


        analog_input_t m_in;
        nl_fptype m_threshold;
        state_var<nl_fptype> m_last;
        FUNC m_func;


        //NETLIB_CONSTRUCTOR_EX(analog_callback, nl_fptype threshold, FUNC &&func)
        public nld_analog_callback(object owner, string name, nl_fptype threshold, FUNC func)
            : base(owner, name)
        {
            m_in = new analog_input_t(this, "IN", in_);
            m_threshold = threshold;
            m_last = new state_var<nl_fptype>(this, "m_last", 0);
            m_func = func;
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            m_last.op = 0.0;
        }


        //NETLIB_HANDLERI(in)
        void in_()
        {
            nl_fptype cur = m_in.op();
            if (plib.pg.abs(cur - m_last.op) > m_threshold)
            {
                m_last.op = cur;
                m_func(this, cur);
            }
        }
    }


    /// \brief logic_callback device
    ///
    /// This device must be connected to a logic net. It has no power terminals
    /// and conversion with proxies will not work.
    ///
    /// Background: This device may be inserted later into the driver and should
    /// not modify the resulting analog representation of the netlist.
    ///
    /// If you get error messages on missing power terminals you have to use the
    /// analog callback device instead.

    //template <typename FUNC>
    public class nld_logic_callback : device_t  //NETLIB_OBJECT(logic_callback)
    {
        public delegate void FUNC(device_t device, netlist_sig_t val);


        logic_input_t m_in;
        FUNC m_func;


        //NETLIB_CONSTRUCTOR_EX(logic_callback, FUNC &&func)
        public nld_logic_callback(object owner, string name, FUNC func)
            : base(owner, name)
        {
            m_in = new logic_input_t(this, "IN", in_);
            m_func = func;
        }


        //NETLIB_HANDLERI(in)
        void in_()
        {
            netlist_sig_t cur = m_in.op();
            m_func(this, cur);
        }
    }


    /// \brief Set parameters to buffers contents at regular intervals
    ///
    /// This device will update a parameter from a buffers passed to the device.
    /// It is the responsibility of the controlling application to ensure that
    /// the buffer is filled at regular intervals.
    ///
    /// \tparam T The buffer type
    /// \tparam N Maximum number of supported buffers
    ///
    //template <typename T>
    //NETLIB_OBJECT(buffered_param_setter)
    class nld_buffered_param_setter<T> : device_t
    {
        delegate void setter_t(nl_fptype param);  //using setter_t = plib::pmfp<void,nl_fptype>;


        netlist_time m_sample_time;

        logic_input_t m_feedback;
        logic_output_t m_Q;

        size_t m_pos;
        size_t m_samples;

        param_str_t m_param_name;
        param_fp_t m_param_mult;
        param_fp_t m_param_offset;
        param_t m_param;  //param_t *   m_param;
        setter_t m_param_setter;
        T m_buffer;  //T *         m_buffer;
        param_num_t<size_t, param_num_t_operators_size_t> m_id;


        //NETLIB_CONSTRUCTOR(buffered_param_setter)
        protected nld_buffered_param_setter(object owner, string name)
            : base(owner, name)
        {
            m_sample_time = netlist_time.zero();
            m_feedback = new logic_input_t(this, "FB", feedback); // clock part
            m_Q = new logic_output_t(this, "Q");
            m_pos = 0;
            m_samples = 0;
            m_param_name = new param_str_t(this, "CHAN", "");
            m_param_mult = new param_fp_t(this, "MULT", 1.0);
            m_param_offset = new param_fp_t(this, "OFFSET", 0.0);
            m_param = null;
            m_id = new param_num_t<size_t, param_num_t_operators_size_t>(this, "ID", 0);


            connect("FB", "Q");
            m_buffer = default;
        }


        //NETLIB_RESETI()
        public override void reset()
        {
        }


        //NETLIB_HANDLERI(feedback)
        void feedback()
        {
            if (m_pos < m_samples)
            {
                throw new emu_unimplemented();
#if false
                // check if called outside of stream_update
                if (m_buffer != null)
                {
                    const nl_fptype v = (*m_buffer)[m_pos];  //const nl_fptype v = (*m_buffer)[m_pos];
                    //m_params[i]->set(v * m_param_mults[i]() + m_param_offsets[i]());
                    m_param_setter(v * m_param_mult() + m_param_offset());
                }
#endif
            }
            else
            {
                // FIXME: The logic has a rounding issue because time resolution divided
                //        by 48,000 is not a natural number. The fractional part
                //        adds up to one samples every 13 seconds for 100 ps resolution.
                //        Fixing this is possible but complicated and expensive.
            }

            m_pos++;

            m_Q.net().toggle_and_push_to_queue(m_sample_time);
        }


        /// \brief resolve parameter names to pointers
        ///
        /// This function must be called after all device were constructed but
        /// before reset is called.
        public void resolve_params(netlist_time sample_time)
        {
            m_pos = 0;
            m_sample_time = sample_time;

            if (m_param_name.op() != "")
            {
                param_t p = state().setup().find_param(m_param_name.op()).param();
                m_param = p;
                if (p is param_fp_t)  //if (dynamic_cast<param_fp_t *>(p) != nullptr)
                    m_param_setter = setter<param_fp_t>;  //m_param_setter = setter_t(&NETLIB_NAME(buffered_param_setter)::setter<param_fp_t>, this);
                else if (p is param_logic_t)  //else if (dynamic_cast<param_logic_t *>(p) != nullptr)
                    m_param_setter = setter<param_logic_t>;  //m_param_setter = setter_t(&NETLIB_NAME(buffered_param_setter)::setter<param_logic_t>, this);
            }
        }


        public void buffer_reset(netlist_time sample_time, size_t num_samples, T inputs)  //void buffer_reset(netlist_time sample_time, std::size_t num_samples, T *inputs)
        {
            m_samples = num_samples;
            m_sample_time = sample_time;
            m_pos = 0;
            m_buffer = inputs;
        }


        public size_t id() { return m_id.op(); }


        //template <typename S>
        void setter<S>(nl_fptype v)
        {
            //static_cast<S *>(m_param)->set(v);
            if (typeof(S) == typeof(param_fp_t))
                ((param_fp_t)m_param).set(v);
            else if (typeof(S) == typeof(param_logic_t))
                ((param_logic_t)m_param).set(v != 0);
            else
                throw new emu_unimplemented();
        }
    }
} // namespace netlist
