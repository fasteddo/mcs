// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;


namespace mame.netlist
{
    // -----------------------------------------------------------------------------
    // analog_t
    // -----------------------------------------------------------------------------
    public class analog_t : detail.core_terminal_t
    {
        protected analog_t(core_device_t dev, string aname, state_e state, nl_delegate delegate_)
            : base(dev, aname, state, delegate_)
        {
        }


        public new analog_net_t net()
        {
            return (analog_net_t)base.net();  //return static_cast<const analog_net_t &>(core_terminal_t::net());
        }


        public solver.matrix_solver_t solver()
        {
            return this.has_net() ? net().solver() : null;
        }
    }


    /// \brief Base class for terminals.
    ///
    /// Each \ref nld_two_terminal object consists of two terminals. Terminals
    /// are at the core of analog netlists and are connected to  \ref net_t
    /// objects.
    ///
    public class terminal_t : analog_t
    {
        Pointer<nl_fptype> m_Idr;  //nl_fptype *m_Idr; ///< drive current
        Pointer<nl_fptype> m_go;  //nl_fptype *m_go;  ///< conductance for Voltage from other term
        Pointer<nl_fptype> m_gt;  //nl_fptype *m_gt;  ///< conductance for total conductance


        /// \brief constructor
        ///
        /// \param dev object owning the terminal
        /// \param aname name of this terminal
        /// \param other_terminal pointer to the sibling terminal
        public terminal_t(core_device_t dev, string aname, nl_delegate delegate_)  //terminal_t(core_device_t &dev, const pstring &aname, terminal_t *otherterm, const std::array<terminal_t *, 2> &splitterterms, nldelegate delegate)
            : base(dev, aname, state_e.STATE_BIDIR, delegate_)
        {
            // NOTE - make sure to call terminal_t_after_ctor()

            m_Idr = null;
            m_go = null;
            m_gt = null;


            // this is handled below so that recursive links can be handled properly.  see nld_twoterm()
            //state().setup().register_term(*this, otherterm, splitterterms);
        }


        public void terminal_t_after_ctor(terminal_t other_terminal, std.array<terminal_t, u64_const_2> splitter_terms = null)
        {
            if (splitter_terms == null)
                splitter_terms = new std.array<terminal_t, u64_const_2>(null, null);

            state().setup().register_term(this, other_terminal, splitter_terms);
        }


        /// \brief Returns voltage of connected net
        ///
        /// \return voltage of net this terminal is connected to
        //nl_fptype operator()() const noexcept { return net().Q_Analog(); }
        public nl_fptype op() { return net().Q_Analog(); }


        /// \brief sets conductivity value of this terminal
        ///
        /// \param G Conductivity
        public void set_conductivity(nl_fptype G)
        {
            set_go_gt_I(-G, G, nlconst.zero());
        }


        public void set_go_gt(nl_fptype GO, nl_fptype GT)
        {
            set_go_gt_I(GO, GT, nlconst.zero());
        }


        public void set_go_gt_I(nl_fptype GO, nl_fptype GT, nl_fptype I)
        {
            // Check for rail nets ...
            if (m_go != null)
            {
                m_Idr[0] = I;  //*m_Idr = I;
                m_go[0] = GO;  //*m_go = GO;
                m_gt[0] = GT;  //*m_gt = GT;
            }
        }


        public void set_ptrs(Pointer<nl_fptype> gt, Pointer<nl_fptype> go, Pointer<nl_fptype> Idr)
        {
            // NOLINTNEXTLINE(readability-implicit-bool-conversion)
            if (!(gt != null && go != null && Idr != null) && (gt != null || go != null || Idr != null))
            {
                throw new nl_exception("Either all pointers must be set or none for terminal {0}", name());
            }

            m_gt = gt;
            m_go = go;
            m_Idr = Idr;
        }
    }


    // -----------------------------------------------------------------------------
    // analog_input_t
    // -----------------------------------------------------------------------------
    /// \brief terminal providing analog input voltage.
    ///
    /// This terminal class provides a voltage measurement. The conductance
    /// against ground is infinite.
    class analog_input_t : analog_t
    {
        /// \brief Constructor
        public analog_input_t(core_device_t dev,  ///< owning device
            string aname,       ///< name of terminal
            nl_delegate delegate_) ///< delegate
            : base(dev, aname, state_e.STATE_INP_ACTIVE, delegate_)
        {
            state().setup().register_term(this);
        }


        /// \brief returns voltage at terminal.
        ///  \returns voltage at terminal.
        //nl_fptype operator()() const noexcept { return Q_Analog(); }
        public nl_fptype op() { return Q_Analog(); }


        /// \brief returns voltage at terminal.
        ///  \returns voltage at terminal.
        public nl_fptype Q_Analog() { return net().Q_Analog(); }
    }


    // -----------------------------------------------------------------------------
    // analog_output_t
    // -----------------------------------------------------------------------------
    class analog_output_t : analog_t
    {
        analog_net_t m_my_net;


        public analog_output_t(core_device_t dev, string aname)
            : base(dev, aname, state_e.STATE_OUT, null)  //: analog_t(dev, aname, STATE_OUT, nldelegate())
        {
            m_my_net = new analog_net_t(dev.state(), name() + ".net", this);


            state().register_net(m_my_net);  //state().register_net(device_arena::owned_ptr<analog_net_t>(&m_my_net, false));
            this.set_net(m_my_net);

            //net().m_cur_Analog = NL_FCONST(0.0);
            state().setup().register_term(this);
        }


        public void push(nl_fptype val)
        {
            if (val != m_my_net.Q_Analog())
            {
                m_my_net.set_Q_Analog(val);
                m_my_net.toggle_and_push_to_queue(netlist_time.quantum());
            }
        }


        public void initial(nl_fptype val)
        {
            net().set_Q_Analog(val);
        }
    }
} // namespace netlist
