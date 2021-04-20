// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;
using nl_fptype = System.Double;
using size_t = System.UInt32;
using stream_sample_t = System.Int32;


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
    /// This devices will set up to N parameters from buffers passed to the device.
    /// It is the responsibility of the controlling application to ensure that
    /// buffers filled at regular intervals.
    ///
    /// \tparam T The buffer type
    /// \tparam N Maximum number of supported buffers
    ///
    //template <typename T, std::size_t N>
    class nld_buffered_param_setter_stream_sample_t : device_t
    {
        int MAX_INPUT_CHANNELS;  //static const int MAX_INPUT_CHANNELS = N;


        netlist_time m_sample_time;

        logic_input_t m_feedback;
        logic_output_t m_Q;

        size_t m_pos;
        size_t m_samples;
        size_t m_num_channels;

        object_array_t<param_str_t> m_param_names;  //object_array_t<param_str_t, MAX_INPUT_CHANNELS> m_param_names;
        object_array_t<param_fp_t>  m_param_mults;  //object_array_t<param_fp_t, MAX_INPUT_CHANNELS>  m_param_mults;
        object_array_t<param_fp_t>  m_param_offsets;  //object_array_t<param_fp_t, MAX_INPUT_CHANNELS>  m_param_offsets;
        std.array<param_fp_t> m_params;  //std::array<param_fp_t *, MAX_INPUT_CHANNELS>             m_params;
        std.array<Pointer<stream_sample_t>> m_buffers;  //std::array<T *, MAX_INPUT_CHANNELS>                               m_buffers;


        protected nld_buffered_param_setter_stream_sample_t(size_t N, netlist_state_t anetlist, string name)
            : base(anetlist, name)
        {
            this.MAX_INPUT_CHANNELS = (int)N;


            m_params = new std.array<param_fp_t>(MAX_INPUT_CHANNELS);
            m_buffers = new std.array<Pointer<stream_sample_t>>(MAX_INPUT_CHANNELS);


            m_sample_time = netlist_time.zero();
            m_feedback = new logic_input_t(this, "FB"); // clock part
            m_Q = new logic_output_t(this, "Q");
            m_pos = 0;
            m_samples = 0;
            m_num_channels = 0;

            throw new emu_unimplemented();
#if false
            m_param_names = new object_array_t<param_str_t>((size_t)MAX_INPUT_CHANNELS, this, 0, "CHAN{}", "");
            m_param_mults = new object_array_t<param_fp_t>((size_t)MAX_INPUT_CHANNELS, this, 0, "MULT{}", 1.0);
            m_param_offsets = new object_array_t<param_fp_t>((size_t)MAX_INPUT_CHANNELS, this, 0, "OFFSET{}", 0.0);
#endif


            connect(m_feedback, m_Q);
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            m_pos = 0;
            for (int i = 0; i < m_buffers.Count; i++)  //for (auto & elem : m_buffers)
                m_buffers[i] = default;  //elem = nullptr;
        }


        //NETLIB_UPDATEI()
        public override void update()
        {
            if (m_pos < m_samples)
            {
                for (size_t i = 0; i < m_num_channels; i++)
                {
                    if (m_buffers[i] == default)
                        break; // stop, called outside of stream_update

                    throw new emu_unimplemented();
#if false
                    nl_fptype v = m_buffers[i][m_pos];
                    m_params[i].set(v * m_param_mults[i]() + m_param_offsets[i]());
#endif
                }
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
            for (size_t i = 0; i < MAX_INPUT_CHANNELS; i++)
            {
                throw new emu_unimplemented();
#if false
                if (m_param_names[i]() != "")
                {
                    if (i != m_num_channels)
                        state().log().fatal.op("sound input numbering has to be sequential!");

                    m_num_channels++;
                    m_params[i] = (param_fp_t)(state().setup().find_param(m_param_names[i]()).param());
                }
#endif
            }
        }


        public void buffer_reset(netlist_time sample_time, size_t num_samples, Pointer<stream_sample_t> [] inputs)  //void buffer_reset(netlist_time sample_time, std::size_t num_samples, T **inputs)
        {
            m_samples = num_samples;
            m_sample_time = sample_time;
            m_pos = 0;
            for (size_t i = 0; i < m_num_channels; i++)
            {
                m_buffers[i] = inputs[i];
            }
        }


        public int num_channels() { return (int)m_num_channels; }
    }
} // namespace netlist
