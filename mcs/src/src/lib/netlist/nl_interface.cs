// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;
using size_t = System.UInt32;


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
    //class NETLIB_NAME(analog_callback) : public device_t


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
    //class NETLIB_NAME(logic_callback) : public device_t


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
        //using setter_t = plib::pmfp<void,nl_fptype>;


        netlist_time m_sample_time;

        logic_input_t m_feedback;
        logic_output_t m_Q;

        size_t m_pos;
        size_t m_samples;

        //param_str_t m_param_name;
        //param_fp_t  m_param_mult;
        //param_fp_t  m_param_offset;
        //param_t *   m_param;
        //setter_t    m_param_setter;
        //T *         m_buffer;
        param_num_t<size_t, param_num_t_operators_uint32> m_id;


        //NETLIB_CONSTRUCTOR(buffered_param_setter)
        protected nld_buffered_param_setter(object owner, string name)
            : base(owner, name)
        {
            m_sample_time = netlist_time.zero();
            m_feedback = new logic_input_t(this, "FB", feedback); // clock part
            m_Q = new logic_output_t(this, "Q");
            m_pos = 0;
            m_samples = 0;

            throw new emu_unimplemented();
#if false
            , m_param_name(*this, "CHAN", "")
            , m_param_mult(*this, "MULT", 1.0)
            , m_param_offset(*this, "OFFSET", 0.0)
            , m_param(nullptr)
            , m_id(*this, "ID", 0)


            connect("FB", "Q");
            m_buffer = null;
#endif
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

            throw new emu_unimplemented();
#if false
            if (m_param_name() != "")
            {
                param_t *p = &state().setup().find_param(m_param_name()).param();
                m_param = p;
                if (dynamic_cast<param_fp_t *>(p) != nullptr)
                    m_param_setter = setter_t(&NETLIB_NAME(buffered_param_setter)::setter<param_fp_t>, this);
                else if (dynamic_cast<param_logic_t *>(p) != nullptr)
                    m_param_setter = setter_t(&NETLIB_NAME(buffered_param_setter)::setter<param_logic_t>, this);
            }
#endif
        }


        public void buffer_reset(netlist_time sample_time, size_t num_samples, object inputs)  //void buffer_reset(netlist_time sample_time, std::size_t num_samples, T *inputs)
        {
            m_samples = num_samples;
            m_sample_time = sample_time;
            m_pos = 0;

            throw new emu_unimplemented();
#if false
            m_buffer = inputs;
#endif
        }


        public size_t id() { return m_id.op(); }


        //template <typename S>
        //void setter(nl_fptype v)
        //{
        //    static_cast<S *>(m_param)->set(v);
        //}
    }
} // namespace netlist
