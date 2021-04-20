// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using devices_collection_type = mame.std.vector<mame.std.pair<string, mame.netlist.core_device_t>>;  //using devices_collection_type = std::vector<std::pair<pstring, poolptr<core_device_t>>>;
using log_type = mame.plib.plog_base<mame.netlist.callbacks_t>;  //using log_type =  plib::plog_base<callbacks_t, NL_DEBUG>;
using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_state_family_collection_type = mame.std.unordered_map<string, mame.netlist.logic_family_desc_t>;  //using family_collection_type = std::unordered_map<pstring, host_arena::unique_ptr<logic_family_desc_t>>;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<NL_PREFER_INT128 && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using nets_collection_type = mame.std.vector<mame.netlist.detail.net_t>;  //using nets_collection_type = std::vector<poolptr<detail::net_t>>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using nldelegate = System.Action;  //using nldelegate = plib::pmfp<void>;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;
using queue_t = mame.netlist.detail.queue_base<mame.netlist.detail.net_t>;  //using queue_t = queue_base<net_t, false>;
using queue_t_entry_t = mame.plib.pqentry_t<mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>, mame.netlist.detail.net_t>;  //using entry_t = plib::pqentry_t<netlist_time_ext, net_t *>;
using props = mame.netlist.detail.property_store_t<mame.netlist.detail.object_t, string>;
using size_t = System.UInt32;
using size_t_constant = mame.uint32_constant;
using state_var_s32 = mame.netlist.state_var<System.Int32>;
using unsigned = System.UInt32;


namespace mame.netlist
{
    //============================================================
    //  MACROS / New Syntax
    //============================================================

    /// \brief Construct a netlist device name
    ///
    //#define NETLIB_NAME(chip) nld_ ## chip

    /// \brief Start a netlist device class.
    ///
    /// Used to start defining a netlist device class.
    /// The simplest device without inputs or outputs would look like this:
    ///
    ///      NETLIB_OBJECT(some_object)
    ///      {
    ///      public:
    ///          NETLIB_CONSTRUCTOR(some_object) { }
    ///      };
    ///
    ///  Also refer to #NETLIB_CONSTRUCTOR.
    //#define NETLIB_OBJECT(name)                                                    \
    //class NETLIB_NAME(name) : public delegator_t<device_t>

    /// \brief Start a derived netlist device class.
    ///
    /// Used to define a derived device class based on plcass.
    /// The simplest device without inputs or outputs would look like this:
    ///
    ///      NETLIB_OBJECT_DERIVED(some_object, parent_object)
    ///      {
    ///      public:
    ///          NETLIB_CONSTRUCTOR(some_object) { }
    ///      };
    ///
    ///  Also refer to #NETLIB_CONSTRUCTOR.
    //#define NETLIB_OBJECT_DERIVED(name, pclass)                                   \
    //class NETLIB_NAME(name) : public delegator_t<NETLIB_NAME(pclass)>

    // Only used for analog objects like diodes and resistors

    //#define NETLIB_BASE_OBJECT(name)                                               \
    //class NETLIB_NAME(name) : public delegator_t<base_device_t>

    //#define NETLIB_CONSTRUCTOR_PASS(cname, ...)                                    \
    //    using this_type = NETLIB_NAME(cname);                                      \
    //    public: template <class CLASS> NETLIB_NAME(cname)(CLASS &owner, const pstring &name) \
    //    : base_type(owner, name, __VA_ARGS__)

    /// \brief Used to define the constructor of a netlist device.
    ///
    ///  Use this to define the constructor of a netlist device. Please refer to
    ///  #NETLIB_OBJECT for an example.
    //#define NETLIB_CONSTRUCTOR(cname)                                              \
    //    using this_type = NETLIB_NAME(cname);                                      \
    //    private: detail::family_setter_t m_famsetter;                              \
    //    public: template <class CLASS> NETLIB_NAME(cname)(CLASS &owner, const pstring &name) \
    //        : base_type(owner, name)

    /// \brief Used to define the constructor of a netlist device and define a default model.
    ///
    ///
    ///      NETLIB_CONSTRUCTOR_MODEL(some_object, "TTL")
    ///      {
    ///      public:
    ///          NETLIB_CONSTRUCTOR(some_object) { }
    ///      };
    ///
    //#define NETLIB_CONSTRUCTOR_MODEL(cname, cmodel)                                              \
    //    using this_type = NETLIB_NAME(cname);                                      \
    //    public: template <class CLASS> NETLIB_NAME(cname)(CLASS &owner, const pstring &name) \
    //        : base_type(owner, name, cmodel)

    /// \brief Define an extended constructor and add further parameters to it.
    /// The macro allows to add further parameters to a device constructor. This is
    /// normally used for sub-devices and system devices only.
    //#define NETLIB_CONSTRUCTOR_EX(cname, ...)                                      \
    //    using this_type = NETLIB_NAME(cname);                                      \
    //    public: template <class CLASS> NETLIB_NAME(cname)(CLASS &owner, const pstring &name, __VA_ARGS__) \
    //        : base_type(owner, name)

    /// \brief Used to define the destructor of a netlist device.
    /// The use of a destructor for netlist device should normally not be necessary.
    //#define NETLIB_DESTRUCTOR(name) public: virtual ~NETLIB_NAME(name)() noexcept override

    /// \brief Add this to a device definition to mark the device as dynamic.
    ///
    ///  If NETLIB_IS_DYNAMIC(true) is added to the device definition the device
    ///  is treated as an analog dynamic device, i.e. \ref NETLIB_UPDATE_TERMINALSI
    ///  is called on a each step of the Newton-Raphson step
    ///  of solving the linear equations.
    ///
    ///  You may also use e.g. NETLIB_IS_DYNAMIC(m_func() != "") to only make the
    ///  device a dynamic device if parameter m_func is set.
    ///
    ///  \param expr boolean expression
    ///
    //#define NETLIB_IS_DYNAMIC(expr)                                                \
    //    public: virtual bool is_dynamic() const override { return expr; }

    /// \brief Add this to a device definition to mark the device as a time-stepping device.
    ///
    ///  You have to implement NETLIB_TIMESTEP in this case as well. Currently, only
    ///  the capacitor and inductor devices uses this.
    ///
    ///  You may also use e.g. NETLIB_IS_TIMESTEP(m_func() != "") to only make the
    ///  device a dynamic device if parameter m_func is set. This is used by the
    ///  Voltage Source element.
    ///
    ///  Example:
    ///
    ///  \code
    ///  NETLIB_TIMESTEP_IS_TIMESTEP()
    ///  NETLIB_TIMESTEPI()
    ///  {
    ///      // Gpar should support convergence
    ///      const nl_fptype G = m_C.Value() / step +  m_GParallel;
    ///      const nl_fptype I = -G/// deltaV();
    ///      set(G, 0.0, I);
    ///  }
    ///  \endcode
    //#define NETLIB_IS_TIMESTEP(expr)                                               \
    //    public: virtual bool is_timestep() const override { return expr; }

    /// \brief Used to implement the time stepping code.
    ///
    /// Please see \ref NETLIB_IS_TIMESTEP for an example.
    //#define NETLIB_TIMESTEPI()                                                     \
    //    public: virtual void timestep(timestep_type ts_type, nl_fptype step)  noexcept override

    /// \brief Used to implement the body of the time stepping code.
    ///
    /// Used when the implementation is outside the class definition
    ///
    /// Please see \ref NETLIB_IS_TIMESTEP for an example.
    ///
    /// \param cname Name of object as given to \ref NETLIB_OBJECT
    ///
    //#define NETLIB_TIMESTEP(cname)                                                 \
    //    void NETLIB_NAME(cname) :: timestep(timestep_type ts_type, nl_fptype step) noexcept

    //#define NETLIB_DELEGATE(name) nldelegate(&this_type :: name, this)

    //#define NETLIB_UPDATE_TERMINALSI() virtual void update_terminals() override
    //#define NETLIB_HANDLERI(name) void name() noexcept
    //#define NETLIB_UPDATE_PARAMI() virtual void update_param() override
    //#define NETLIB_RESETI() virtual void reset() override

    //#define NETLIB_SUB(chip) nld_ ## chip
    //#define NETLIB_SUB_UPTR(ns, chip) device_arena::unique_ptr< ns :: nld_ ## chip >

    //#define NETLIB_HANDLER(chip, name) void NETLIB_NAME(chip) :: name() NL_NOEXCEPT

#if false
    //#define NETLIB_UPDATEI() virtual void update() noexcept override
    //#define NETLIB_UPDATE(chip) NETLIB_HANDLER(chip, update)
#endif

    //#define NETLIB_RESET(chip) void NETLIB_NAME(chip) :: reset(void)

    //#define NETLIB_UPDATE_PARAM(chip) void NETLIB_NAME(chip) :: update_param()

    //#define NETLIB_UPDATE_TERMINALS(chip) void NETLIB_NAME(chip) :: update_terminals()


    namespace detail
    {
        public interface netlist_name_interface
        {
            string name();  // from object_t, device_t
        }


        public interface netlist_interface
        {
            netlist_state_t state();
            netlist_t exec();
        }


        public interface netlist_interface_plus_name : netlist_interface, netlist_name_interface { }
    }


    // -----------------------------------------------------------------------------
    // analog_t
    // -----------------------------------------------------------------------------
    public class analog_t : detail.core_terminal_t
    {
        public analog_t(core_device_t dev, string aname, state_e state, nldelegate delegate_)
            : base(dev, aname, state, delegate_)
        {
        }


        public new analog_net_t net()
        {
            return (analog_net_t)base.net();  //return static_cast<const analog_net_t &>(core_terminal_t::net());
        }


        public solver.matrix_solver_t solver()
        {
            if (this.has_net())
                return net().solver();
            return null;
        }
    }


    /// \brief Base clase for terminals.
    ///
    /// Each \ref nld_twoterm object consists of two terminals. Terminals
    /// are at the core of analog netlists and provide. \ref net_t objects
    /// connect terminals.
    ///
    public class terminal_t : analog_t
    {
        Pointer<nl_fptype> m_Idr;  //nl_fptype *m_Idr1; ///< drive current
        Pointer<nl_fptype> m_go;   //nl_fptype *m_go1;  ///< conductance for Voltage from other term
        Pointer<nl_fptype> m_gt;   //nl_fptype *m_gt1;  ///< conductance for total conductance


        /// \brief constructor
        ///
        /// @param dev core_devict_t object owning the terminal
        /// @param aname name of this terminal
        /// @param otherterm pointer to the sibling terminal
        public terminal_t(core_device_t dev, string aname, nldelegate delegate_)
            : base(dev, aname, state_e.STATE_BIDIR, delegate_)
        {
            // NOTE - make sure to call terminal_t_after_ctor()

            m_Idr = null;
            m_go = null;
            m_gt = null;


            // this is handled below so that recursive links can be handled properly.  see nld_twoterm()
            //state().setup().register_term(this, otherterm);
        }

        public void terminal_t_after_ctor(terminal_t otherterm)
        {
            state().setup().register_term(this, otherterm);
        }


        //~terminal_t() { }


        /// \brief Returns voltage of connected net
        ///
        /// @return voltage of net this terminal is connected to
        //nl_fptype operator ()() const  NL_NOEXCEPT;


        /// @brief sets conductivity value of this terminal
        ///
        /// @param G Conductivity
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
                m_Idr[0] = I;  //*m_Idr1 = I;
                m_go[0] = GO;  //*m_go1 = GO;
                m_gt[0] = GT;  //*m_gt1 = GT;
            }
        }


        public void set_ptrs(Pointer<nl_fptype> gt, Pointer<nl_fptype> go, Pointer<nl_fptype> Idr)  //void set_ptrs(nl_fptype *gt, nl_fptype *go, nl_fptype *Idr) noexcept(false);
        {
            if (!(gt != null && go != null && Idr != null) && (gt != null || go != null || Idr != null))  //if (!(gt && go && Idr) && (gt || go || Idr))
            {
                state().log().fatal.op("Inconsistent nullptrs for terminal {0}", name());
                throw new nl_exception("Inconsistent nullptrs for terminal {0}", name());
            }

            m_gt = new Pointer<nl_fptype>(gt);  //m_gt = gt;
            m_go = new Pointer<nl_fptype>(go);  //m_go = go;
            m_Idr = new Pointer<nl_fptype>(Idr);  //m_Idr = Idr;
        }
    }


    // -----------------------------------------------------------------------------
    // logic_t
    // -----------------------------------------------------------------------------
    public class logic_t : detail.core_terminal_t,
                           logic_family_t
    {
        public logic_t(device_t dev, string aname, state_e terminal_state, nldelegate delegate_)
            : base(dev, aname, terminal_state, delegate_)
        {
        }


        // logic_family_t
        logic_family_desc_t m_logic_family;
        public logic_family_desc_t logic_family() { return m_logic_family; }
        public void set_logic_family(logic_family_desc_t fam) { m_logic_family = fam; }


        public new logic_net_t net() { return (logic_net_t)base.net(); }
    }


    // -----------------------------------------------------------------------------
    // logic_input_t
    // -----------------------------------------------------------------------------
    public class logic_input_t : logic_t
    {
        public logic_input_t(device_t dev, string aname, nldelegate delegate_)
            : base(dev, aname, state_e.STATE_INP_ACTIVE, delegate_)
        {
            if (delegate_ != null)  //if (!delegate.is_set())
                throw new nl_exception("delegate not set for {0}", this.name());

            state().setup().register_term(this);
        }


        //netlist_sig_t operator()() const NL_NOEXCEPT
        public netlist_sig_t op()
        {
            //nl_assert(terminal_state() != STATE_INP_PASSIVE);
#if NL_USE_COPY_INSTEAD_OF_REFERENCE
            return m_Q;
#else
            return net().Q();
#endif
        }


        //void inactivate() NL_NOEXCEPT;
        //void activate() NL_NOEXCEPT;
        //void activate_hl() NL_NOEXCEPT;
        //void activate_lh() NL_NOEXCEPT;
    }


    // -----------------------------------------------------------------------------
    // analog_input_t
    // -----------------------------------------------------------------------------
    /// \brief terminal providing analog input voltage.
    ///
    /// This terminal class provides a voltage measurement. The conductance against
    /// ground is infinite.
    class analog_input_t : analog_t
    {
        /// \brief Constructor
        public analog_input_t(core_device_t dev, ///< owning device
                              string aname,      ///< name of terminal
                              nldelegate delegate_) ///< delegate
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


    public class analog_net_t : detail.net_t
    {
        //using list_t =  plib::aligned_vector<analog_net_t *>;


        public state_var<Pointer<nl_fptype>> m_cur_Analog;  //state_var<nl_fptype>     m_cur_Analog;
        solver.matrix_solver_t m_solver;


        // ----------------------------------------------------------------------------------------
        // analog_net_t
        // ----------------------------------------------------------------------------------------
        public analog_net_t(netlist_state_t nl, string aname, detail.core_terminal_t railterminal = null)
            : base(nl, aname, railterminal)
        {
            m_cur_Analog = new state_var<Pointer<nl_fptype>>(this, "m_cur_Analog", new Pointer<nl_fptype>(new std.vector<nl_fptype>(1)));  //, m_cur_Analog(*this, "m_cur_Analog", nlconst::zero())
            m_solver = null;
        }


        public override void reset()
        {
            base.reset();
            m_cur_Analog.op[0] = nlconst.zero();
        }


        public nl_fptype Q_Analog() { return m_cur_Analog.op[0]; }
        public void set_Q_Analog(nl_fptype v) { m_cur_Analog.op[0] = v; }
        // used by solver code ...
        public Pointer<nl_fptype> Q_Analog_state_ptr() { return m_cur_Analog.op; }  //nl_fptype *Q_Analog_state_ptr() noexcept { return &m_cur_Analog(); }

        //FIXME: needed by current solver code
        public solver.matrix_solver_t solver() { return m_solver; }
        public void set_solver(solver.matrix_solver_t solver) { m_solver = solver; }

        //friend constexpr bool operator==(const analog_net_t &lhs, const analog_net_t &rhs) noexcept { return &lhs == &rhs; }
    }


    // -----------------------------------------------------------------------------
    // logic_output_t
    // -----------------------------------------------------------------------------
    public class logic_output_t : logic_t
    {
        logic_net_t m_my_net;


        /// \brief logic output constructor
        ///
        /// The third parameter does nothing. It is provided only for
        /// compatibility with tristate_output_t in templatized device models
        ///
        /// \param dev Device owning this output
        /// \param aname The name of this output
        /// \param dummy Dummy parameter to allow construction like tristate output
        ///
        public logic_output_t(device_t dev, string aname, bool dummy = false)
            : base(dev, aname, state_e.STATE_OUT, null)  //: logic_t(dev, aname, STATE_OUT, nldelegate())
        {
            m_my_net = new logic_net_t(dev.state(), name() + ".net", this);

            //plib::unused_var(dummy);
            set_net(m_my_net);
            state().register_net(m_my_net);  //state().register_net(device_arena::owned_ptr<logic_net_t>(&m_my_net, false));
            state().setup().register_term(this);
        }


        public void initial(netlist_sig_t val)
        {
            if (has_net())
                net().initial(val);
        }


        public void push(netlist_sig_t newQ, netlist_time delay)
        {
            m_my_net.set_Q_and_push(newQ, delay); // take the shortcut
        }


        //void set_Q_time(const netlist_sig_t newQ, const netlist_time_ext &at) NL_NOEXCEPT


        /// \brief Dummy implementation for templatized generic devices
        ///
        /// This function shall never be called. It is defined here so that
        /// templatized generic device models do not have to do tons of
        /// template magic.
        ///
        /// This function terminates if actually called.
        ///
        //[[noreturn]] static void set_tristate(netlist_sig_t v,
        //    netlist_time ts_off_on, netlist_time ts_on_off)
    }


    // -----------------------------------------------------------------------------
    // tristate_output_t
    // -----------------------------------------------------------------------------
    /// \brief Tristate output
    ///
    /// In a lot of applications tristate enable inputs are just connected to
    /// VCC/GND to permanently enable the outputs. In this case a pure
    /// implementation using analog outputs would not perform well.
    ///
    /// For this object during creation it can be decided if a logic output or
    /// a tristate output is used. Generally the owning device uses parameter
    /// FORCE_TRISTATE_LOGIC to determine this.
    ///
    /// This is the preferred way to implement tristate outputs.
    ///
    class tristate_output_t : logic_output_t
    {
        //using logic_output_t::initial;
        //using logic_output_t::set_Q_time;


        //state_var<netlist_sig_t> m_last_logic;
        //state_var<netlist_sig_t> m_tristate;
        //bool m_force_logic;


        tristate_output_t(device_t dev, string aname, bool force_logic)
            : base(dev, aname)
        {
            throw new emu_unimplemented();
#if false
            m_last_logic(dev, name() + "." + "m_last_logic", 1) // force change
            m_tristate(dev, name() + "." + "m_tristate", force_logic ? 0 : 2) // force change
            m_force_logic(force_logic)
#endif
        }


        //void push(netlist_sig_t newQ, netlist_time delay) noexcept

        //void set_tristate(netlist_sig_t v, netlist_time ts_off_on, netlist_time ts_on_off) noexcept


        public bool is_force_logic()
        {
            throw new emu_unimplemented();
#if false
            return m_force_logic;
#endif
        }
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


    // -----------------------------------------------------------------------------
    // base_device_t
    // -----------------------------------------------------------------------------
    public class base_device_t : core_device_t
    {
        //protected base_device_t(netlist_state_t owner, string name)
        //    : base(owner, name)
        //{ }

        //protected base_device_t(base_device_t owner, string name)
        //    : base(owner, name)
        //{ }

        protected base_device_t(object owner, string name)
            : base(owner, name)
        { }


        //PCOPYASSIGNMOVE(base_device_t, delete)

        //~base_device_t() noexcept override = default;


        //template<class O, class C, typename... Args>
        protected void create_and_register_subdevice(base_device_t owner, string name, out analog.nld_C out_) { out_ = new analog.nld_C(owner, name); } //void create_and_register_subdevice(O& owner, const pstring &name, device_arena::unique_ptr<C> &dev, Args&&... args);
        protected void create_and_register_subdevice(base_device_t owner, string name, out analog.nld_VCVS out_) { out_ = new analog.nld_VCVS(owner, name); } //void create_and_register_subdevice(O& owner, const pstring &name, device_arena::unique_ptr<C> &dev, Args&&... args);
        protected void create_and_register_subdevice(base_device_t owner, string name, out analog.nld_D out_, string model = "D") { out_ = new analog.nld_D(owner, name, model); } //void create_and_register_subdevice(O& owner, const pstring &name, device_arena::unique_ptr<C> &dev, Args&&... args);


        protected void register_subalias(string name, detail.core_terminal_t term)
        {
            string alias = this.name() + "." + name;

            // everything already fully qualified
            state().parser().register_alias_nofqn(alias, term.name());
        }


        protected void register_subalias(string name, string aliased)
        {
            string alias = this.name() + "." + name;
            string aliased_fqn = this.name() + "." + aliased;

            // everything already fully qualified
            state().parser().register_alias_nofqn(alias, aliased_fqn);
        }


        protected void connect(string t1, string t2)
        {
            state().parser().register_link_fqn(name() + "." + t1, name() + "." + t2);
        }


        protected void connect(detail.core_terminal_t t1, detail.core_terminal_t t2)
        {
            state().parser().register_link_fqn(t1.name(), t2.name());
        }


        //NETLIB_UPDATE_TERMINALSI() { }
    }


    // -----------------------------------------------------------------------------
    // device_t
    // -----------------------------------------------------------------------------
    public class device_t : base_device_t,
                            logic_family_t
    {
        param_model_t m_model;


        //protected device_t(netlist_state_t owner, string name);
        //protected device_t(netlist_state_t owner, string name, string model);
        // only needed by proxies
        //protected device_t(netlist_state_t owner, string name, logic_family_desc_t desc);

        //protected device_t(device_t owner, string name);
        // pass in a default model - this may be overwritten by PARAM(DEVICE.MODEL, "XYZ(...)")
        //protected device_t(device_t owner, string name, string model);

        protected device_t(object owner, string name)
            : base(owner, name)
        {
            if (owner is netlist_state_t) device_t_after_ctor((netlist_state_t)owner, name);
            else if (owner is device_t) device_t_after_ctor((device_t)owner, name);
            else throw new emu_unimplemented();
        }

        protected device_t(object owner, string name, string model)
            : base(owner, name)
        {
            if (owner is netlist_state_t) device_t_after_ctor((netlist_state_t)owner, name, model);
            else if (owner is device_t) device_t_after_ctor((device_t)owner, name, model);
            else throw new emu_unimplemented();
        }

        protected device_t(object owner, string name, logic_family_desc_t desc)
            : base(owner, name)
        {
            if (owner is netlist_state_t) device_t_after_ctor((netlist_state_t)owner, name, desc);
            else if (owner is device_t) new emu_unimplemented();
            else throw new emu_unimplemented();
        }


        public void device_t_after_ctor(netlist_state_t owner, string name)
            //: base(owner, name)
        {
            m_model = new param_model_t(this, "MODEL", config.DEFAULT_LOGIC_FAMILY());


            set_logic_family(state().setup().family_from_model(m_model.op()));
            if (logic_family() == null)
                throw new nl_exception(nl_errstr_global.MF_NULLPTR_FAMILY(this.name(), m_model.op()));
        }


        public void device_t_after_ctor(netlist_state_t owner, string name, string model)
            //: base(owner, name)
        {
            m_model = new param_model_t(this, "MODEL", model);


            set_logic_family(state().setup().family_from_model(m_model.op()));
            if (logic_family() == null)
                throw new nl_exception(nl_errstr_global.MF_NULLPTR_FAMILY(this.name(), m_model.op()));
        }


        // only needed by proxies
        public void device_t_after_ctor(netlist_state_t owner, string name, logic_family_desc_t desc)
            //: base(owner, name)
        {
            m_model = new param_model_t(this, "MODEL", "");


            set_logic_family(desc);
            if (logic_family() == null)
                throw new nl_exception(nl_errstr_global.MF_NULLPTR_FAMILY(this.name(), "<pointer provided by constructor>"));
        }


        public void device_t_after_ctor(device_t owner, string name)
            //: base(owner, name)
        {
            m_model = new param_model_t(this, "MODEL", "");


            set_logic_family(owner.logic_family());
            if (logic_family() == null)
                throw new nl_exception(nl_errstr_global.MF_NULLPTR_FAMILY(this.name(), "<owner logic family>"));
        }


        // pass in a default model - this may be overwritten by PARAM(DEVICE.MODEL, "XYZ(...)")
        public void device_t_after_ctor(device_t owner, string name, string model)
            //: base(owner, name)
        {
            m_model = new param_model_t(this, "MODEL", model);


            set_logic_family(state().setup().family_from_model(m_model.op()));
            if (logic_family() == null)
                throw new nl_exception(nl_errstr_global.MF_NULLPTR_FAMILY(this.name(), m_model.op()));
        }


        //PCOPYASSIGNMOVE(device_t, delete)

        //~device_t() noexcept override = default;


        // logic_family_t
        logic_family_desc_t m_logic_family;
        public logic_family_desc_t logic_family() { return m_logic_family; }
        public void set_logic_family(logic_family_desc_t fam) { m_logic_family = fam; }


        //NETLIB_UPDATE_TERMINALSI() { }
    }


    namespace detail
    {
        // Use timed_queue_heap to use stdc++ heap functions instead of linear processing.
        // This slows down processing by about 25% on a Kaby Lake.
        // template <class T, bool TS>
        // using timed_queue = plib::timed_queue_heap<T, TS>;

        //template <class T, bool TS>
        public class timed_queue<T> : plib.timed_queue_linear<T>  //using timed_queue = plib::timed_queue_linear<T, TS>;
        {
            protected timed_queue(bool TS, size_t list_size) : base(TS, list_size) { }
        }


        // -----------------------------------------------------------------------------
        // queue_t
        // -----------------------------------------------------------------------------

        //template <typename O, bool TS>
        public class queue_base<O> : timed_queue<plib.pqentry_t<netlist_time_ext, O>>,  //public timed_queue<plib::pqentry_t<netlist_time_ext, O *>, false>,
                                     plib.state_manager_t.callback_t
        {
            //using entry_t = plib::pqentry_t<netlist_time_ext, O *>;
            //using base_queue = timed_queue<entry_t, false>;
            //using id_delegate = plib::pmfp<std::size_t, const O *>;
            //using obj_delegate = plib::pmfp<O *, std::size_t>;


            size_t m_qsize;
            std.vector<Int64> m_times;  //std::vector<netlist_time_ext::internal_type> m_times;
            std.vector<size_t> m_net_ids;
            Func<O, size_t> m_get_id;  //id_delegate m_get_id;
            Func<size_t, O> m_obj_by_id;  //obj_delegate m_obj_by_id;


            public queue_base(bool TS, size_t size, Func<O, size_t> get_id, Func<size_t, O> get_obj)
                : base(TS, size)
            {
                m_qsize = 0;
                m_times = new std.vector<Int64>(size);
                m_net_ids = new std.vector<size_t>(size);
                m_get_id = get_id;
                m_obj_by_id = get_obj;
            }

            //~queue_base() noexcept override = default;

            //queue_base(const queue_base &) = delete;
            //queue_base(queue_base &&) = delete;
            //queue_base &operator=(const queue_base &) = delete;
            //queue_base &operator=(queue_base &&) = delete;


            public void register_state(plib.state_manager_t manager, string module)
            {
                throw new emu_unimplemented();
#if false
                manager.save_item(this, m_qsize, module + "." + "qsize");
                manager.save_item(this, &m_times[0], module + "." + "times", m_times.size());
                manager.save_item(this, &m_net_ids[0], module + "." + "names", m_net_ids.size());
#endif
            }


            public void on_pre_save(plib.state_manager_t manager)
            {
                throw new emu_unimplemented();
#if false
                plib::unused_var(manager);
                m_qsize = this->size();
                for (std::size_t i = 0; i < m_qsize; i++ )
                {
                    m_times[i] =  this->listptr()[i].exec_time().as_raw();
                    m_net_ids[i] = m_get_id(this->listptr()[i].object());
                }
#endif
            }
            public void on_post_load(plib.state_manager_t manager)
            {
                throw new emu_unimplemented();
#if false
                plib::unused_var(manager);
                this->clear();
                for (std::size_t i = 0; i < m_qsize; i++ )
                {
                    O *n = m_obj_by_id(m_net_ids[i]);
                    this->template push<false>(entry_t(netlist_time_ext::from_raw(m_times[i]),n));
                }
#endif
            }
        }


        //using queue_t = queue_base<net_t, false>;
    }


    // -----------------------------------------------------------------------------
    // netlist_state__t
    // -----------------------------------------------------------------------------
    public class netlist_state_t
    {
        //using nets_collection_type = std::vector<device_arena::owned_ptr<detail::net_t>>;
        //using family_collection_type = std::unordered_map<pstring, host_arena::unique_ptr<logic_family_desc_t>>;

        // need to preserve order of device creation ...
        //using devices_collection_type = std::vector<std::pair<pstring, device_arena::owned_ptr<core_device_t>>>;


        //struct stats_info
        //{
        //    const detail::queue_t               &m_queue;// performance
        //    const plib::pperftime_t<true>       &m_stat_mainloop;
        //    const plib::pperfcount_t<true>      &m_perf_out_processed;
        //};


        device_arena m_pool; // must be deleted last!

        netlist_t m_netlist;  //device_arena::unique_ptr<netlist_t>        m_netlist;
        plib.dynlib_base m_lib;  //host_arena::unique_ptr<plib::dynlib_base>  m_lib;
        plib.state_manager_t m_state = new plib.state_manager_t();
        callbacks_t m_callbacks;  //host_arena::unique_ptr<callbacks_t>        m_callbacks;
        log_type m_log;

        // FIXME: should only be available during device construcion
        setup_t m_setup;  //host_arena::unique_ptr<setup_t>            m_setup;

        nets_collection_type m_nets = new nets_collection_type();
        // sole use is to manage lifetime of net objects
        devices_collection_type m_devices = new devices_collection_type();
        // sole use is to manage lifetime of family objects
        netlist_state_family_collection_type m_family_cache;  //family_collection_type m_family_cache;
        bool m_extended_validation;

        // dummy version
        int m_dummy_version;


        public netlist_state_t(string name, callbacks_t callbacks)  //netlist_state_t(const pstring &name, host_arena::unique_ptr<callbacks_t> &&callbacks);
        {
            m_callbacks = callbacks; // Order is important here
            m_log = new log_type(nl_config_global.NL_DEBUG, m_callbacks);
            m_extended_validation = false;
            m_dummy_version = 1;


            m_lib = m_callbacks.static_solver_lib();

            m_setup = new setup_t(this);  //m_setup = plib::make_unique<setup_t, host_arena>(*this);
            // create the run interface
            m_netlist = new netlist_t(this, name);  //m_netlist = plib::make_unique<netlist_t>(m_pool, *this, name);

            // Make sure save states are invalidated when a new version is deployed

            m_state.save_item(this, m_dummy_version, "V" + version());

            // Initialize factory
            netlist.devices.net_lib_global.initialize_factory(m_setup.parser().factory_());

            // Add default include file
            //using a = plib::psource_str_t;
            string content =
            "#define RES_R(res) (res)            \n" +
            "#define RES_K(res) ((res) * 1e3)    \n" +
            "#define RES_M(res) ((res) * 1e6)    \n" +
            "#define CAP_U(cap) ((cap) * 1e-6)   \n" +
            "#define CAP_N(cap) ((cap) * 1e-9)   \n" +
            "#define CAP_P(cap) ((cap) * 1e-12)  \n" +
            "#define IND_U(ind) ((ind) * 1e-6)   \n" +
            "#define IND_N(ind) ((ind) * 1e-9)   \n" +
            "#define IND_P(ind) ((ind) * 1e-12)  \n";

            //throw new emu_unimplemented();
#if false
            m_setup->parser().add_include<a>("netlist/devices/net_lib.h", content);
#endif

            nlm_base_global.netlist_base(m_setup.parser());  //NETLIST_NAME(base)(m_setup->parser());
        }


        //PCOPYASSIGNMOVE(netlist_state_t, delete)
        /// \brief Destructor
        ///
        /// The destructor is virtual to allow implementation specific devices
        /// to connect to the outside world. For examples see MAME netlist.cpp.
        ///
        //virtual ~netlist_state_t() noexcept = default;


        //template<class C>
        static bool check_class<C>(core_device_t p) where C : core_device_t
        {
            return p is C;  //return dynamic_cast<C *>(p) != nullptr;
        }


        delegate bool get_single_device_func(core_device_t device);

        core_device_t get_single_device(string classname, get_single_device_func cc)  //core_device_t *get_single_device(const pstring &classname, bool (*cc)(core_device_t *)) const;
        {
            core_device_t ret = null;
            foreach (var d in m_devices)
            {
                if (cc(d.second))
                {
                    if (ret != null)
                    {
                        m_log.fatal.op(nl_errstr_global.MF_MORE_THAN_ONE_1_DEVICE_FOUND(classname));
                        throw new nl_exception(nl_errstr_global.MF_MORE_THAN_ONE_1_DEVICE_FOUND(classname));
                    }

                    ret = d.second;
                }
            }

            return ret;
        }

        /// \brief Get single device filtered by class and name
        ///
        /// \tparam C Device class for which devices will be returned
        /// \param  name Name of the device
        ///
        /// \return pointers to device
        //template<class C>
        public C get_single_device<C>(string name) where C : core_device_t
        {
            return (C)get_single_device(name, check_class<C>);  //return dynamic_cast<C *>(get_single_device(name, check_class<C>));
        }


        /// \brief Get vector of devices
        ///
        /// \tparam C Device class for which devices will be returned
        ///
        /// \return vector with pointers to devices
        //template<class C>
        public std.vector<C> get_device_list<C>() where C : core_device_t
        {
            std.vector<C> tmp = new std.vector<C>();
            foreach (var d in m_devices)
            {
                var dev = d.second is C ? (C)d.second : null;  //auto dev = dynamic_cast<C *>(d.second.get());
                if (dev != null)
                    tmp.push_back(dev);
            }
            return tmp;
        }


        // logging

        public log_type log() { return m_log; }

        public plib.dynlib_base lib() { return m_lib; }

        public netlist_t exec() { return m_netlist; }


        // state handling

        public plib.state_manager_t run_state_manager() { return m_state; }


        //template<typename O, typename C>
        public void save(object owner, object state, string module, string stname)  //void save(O &owner, C &state, const pstring &module, const pstring &stname)
        {
            this.run_state_manager().save_item(owner, state, module + "." + stname);  //this->run_state_manager().save_item(static_cast<void *>(&owner), state, module + pstring(".") + stname);
        }
        //template<typename O, typename C>
        public void save(object owner, object state, string module, string stname, UInt32 count)  //void save(O &owner, C *state, const pstring &module, const pstring &stname, const std::size_t count)
        {
            this.run_state_manager().save_state_ptr(owner, module + "." + stname, null, count, state);  //this->run_state_manager().save_state_ptr(static_cast<void *>(&owner), module + pstring(".") + stname, plib::state_manager_t::dtype<C>(), count, state);
        }


        // FIXME: only used by queue_t save state
        public size_t find_net_id(detail.net_t net)
        {
            for (size_t i = 0; i < m_nets.size(); i++)
            {
                if (m_nets[i] == net)
                    return i;
            }

            return size_t.MaxValue;  //return std::numeric_limits<std::size_t>::max();
        }


        public detail.net_t net_by_id(size_t id)
        {
            return m_nets[id];
        }


        //template <typename T>
        public void register_net(detail.net_t net) { m_nets.push_back(net); }  //void register_net(device_arena::owned_ptr<T> &&net) { m_nets.push_back(std::move(net)); }


        /// \brief Get device pointer by name
        ///
        ///
        /// \param name Name of the device
        ///
        /// \return core_device_t pointer if device exists, else nullptr
        public core_device_t find_device(string name)
        {
            foreach (var d in m_devices)
            {
                if (d.first == name)
                    return d.second;
            }

            return null;
        }


        /// \brief Register device using owned_ptr
        ///
        /// Used to register owned devices. These are devices declared as objects
        /// in another devices.
        ///
        /// \param name Name of the device
        /// \param dev Device to be registered
        //template <typename T>
        public void register_device(string name, core_device_t dev)  //void register_device(const pstring &name, device_arena::owned_ptr<T> &&dev) noexcept(false)
        {
            foreach (var d in m_devices)
            {
                if (d.first == name)
                {
                    //dev.release();
                    log().fatal.op(nl_errstr_global.MF_DUPLICATE_NAME_DEVICE_LIST(name));
                    throw new nl_exception(nl_errstr_global.MF_DUPLICATE_NAME_DEVICE_LIST(name));
                }
            }

            //m_devices.push_back(dev);
            m_devices.Add(new std.pair<string, core_device_t>(name, dev));  //m_devices.insert(m_devices.end(), { name, std::move(dev) });
        }


#if false
        /// \brief Register device using unique_ptr
        ///
        /// Used to register devices.
        ///
        /// \param name Name of the device
        /// \param dev Device to be registered
        template <typename T>
        void register_device(const pstring &name, device_arena::unique_ptr<T> &&dev)
        {
            register_device(name, device_arena::owned_ptr<T>(dev.release(), true, dev.get_deleter()));
        }
#endif


        /// \brief Remove device
        ///
        /// Care needs to be applied if this is called to remove devices with
        /// sub-devices which may have registered state.
        ///
        /// \param dev Device to be removed
        public void remove_device(core_device_t dev)
        {
            for (int i = 0; i < m_devices.Count; i++)  //for (auto it = m_devices.begin(); it != m_devices.end(); it++)
            {
                var it = m_devices[i];
                if (it.second == dev)
                {
                    m_state.remove_save_items(dev);
                    m_devices.erase(i);
                    return;
                }
            }
        }


        public setup_t setup() { return m_setup; }


        public nlparse_t parser() { return m_setup.parser(); }


        // FIXME: make a postload member and include code there
        public void rebuild_lists() // must be called after post_load !
        {
            foreach (var net in m_nets)
                net.rebuild_list();
        }


        //static void compile_defines(std::vector<std::pair<pstring, pstring>> &defs);


        static string version()
        {
            return new plib.pfmt("{0}.{1}").op(nl_config_global.NL_VERSION_MAJOR, nl_config_global.NL_VERSION_MINOR);
        }


        //static pstring version_patchlevel();


        public nets_collection_type nets() { return m_nets; }
        public devices_collection_type devices() { return m_devices; }


        public netlist_state_family_collection_type family_cache() { return m_family_cache; }


        //template<typename T, typename... Args>
        //device_arena::unique_ptr<T> make_pool_object(Args&&... args)
        //{
        //    return plib::make_unique<T>(m_pool, std::forward<Args>(args)...);
        //}

        // memory pool - still needed in some places
        public device_arena pool() { return m_pool; }


        /// \brief set extended validation mode.
        ///
        /// The extended validation mode is not intended for running.
        /// The intention is to identify power pins which are not properly
        /// connected. The downside is that this mode creates a netlist which
        /// is different (and not able to run).
        ///
        /// Extended validation is supported by nltool validate option.
        ///
        /// \param val Boolean value enabling/disabling extended validation mode
        public void set_extended_validation(bool val) { m_extended_validation = val; }

        /// \brief State of extended validation mode.
        ///
        /// \returns boolean value indicating if extended validation mode is
        /// turned on.
        public bool is_extended_validation() { return m_extended_validation; }


        /// \brief print statistics gathered during run
        ///
        //void print_stats(stats_info &si) const;


        /// \brief call reset on all netlist components
        ///
        public void reset()
        {
            // Reset all nets once !
            log().verbose.op("Call reset on all nets:");
            foreach (var n in nets())
                n.reset();

            // Reset all devices once !
            log().verbose.op("Call reset on all devices:");
            foreach (var dev in m_devices)
                dev.second.reset();

            // Make sure everything depending on parameters is set
            // Currently analog input and logic input also
            // push their outputs to queue.

            std.vector<core_device_t> devices_called = new std.vector<core_device_t>();
            log().verbose.op("Call update_param on all devices:");
            foreach (var dev in m_devices)
            {
                dev.second.update_param();
                if (!plib.container.contains(devices_called, dev.second))
                    devices_called.push_back(dev.second);
            }

            // Step all devices once !
            //
            // INFO: The order here affects power up of e.g. breakout. However, such
            // variations are explicitly stated in the breakout manual.

            var netlist_params = get_single_device<devices.nld_netlistparams>("parameter");

            switch (netlist_params.m_startup_strategy.op())
            {
                case 0:
                {
                    std.vector<nldelegate> t = new std.vector<nldelegate>();
                    log().verbose.op("Using default startup strategy");
                    foreach (var n in m_nets)
                    {
                        foreach (var term in n.core_terms())
                        {
                            n.update_inputs(); // only used if USE_COPY_INSTEAD_OF_REFERENCE == 1
                            if (!plib.container.contains(t, term.delegate_()))
                            {
                                t.push_back(term.delegate_());
                                term.run_delegate();
                            }

                            throw new emu_unimplemented();
#if false
                            // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
                            auto *dev = reinterpret_cast<core_device_t *>(term->delegate().object());
                            if (!plib::container::contains(devices_called, dev))
                                devices_called.push_back(dev);
#endif
                        }
                    }

                    log().verbose.op("Devices not yet updated:");
                    foreach (var dev in m_devices)
                    {
                        if (!plib.container.contains(devices_called, dev.second))
                        {
                            // FIXME: doesn't seem to be needed, use cases include
                            // analog output devices. Check and remove
                            log().error.op("\t Device {0} not yet updated", dev.second.name());
                            //dev.second->update();
                        }
                    }
                }
                break;
            }

#if true
            // the above may screw up m_active and the list
            rebuild_lists();
#endif
        }


        /// \brief prior to running free no longer needed resources
        ///
        public void free_setup_resources()
        {
            m_setup = null;
        }
    }


    namespace devices
    {
        // -----------------------------------------------------------------------------
        // mainclock
        // -----------------------------------------------------------------------------

        //NETLIB_OBJECT(mainclock)
        class nld_mainclock : device_t
        {
            public logic_output_t m_Q;
            public netlist_time m_inc;
            param_fp_t m_freq;


            //NETLIB_CONSTRUCTOR(mainclock);
            nld_mainclock(object owner, string name)
                : base(owner, name)
            {
                m_Q = new logic_output_t(this, "Q");
                m_freq = new param_fp_t(this, "FREQ", nlconst.magic(7159000.0 * 5));


                m_inc = netlist_time.from_fp(plib.pglobal.reciprocal<nl_fptype, nl_fptype_ops>(m_freq.op() * nlconst.two()));
            }


            //NETLIB_RESETI();
            public override void reset()
            {
                m_Q.net().set_next_scheduled_time(exec().time());
            }


            //NETLIB_UPDATE_PARAMI();
            public override void update_param()
            {
                m_inc = netlist_time.from_fp(plib.pglobal.reciprocal<nl_fptype, nl_fptype_ops>(m_freq.op() * nlconst.two()));
            }
        }
    } // namespace devices


    // ----------------------------------------------------------------------------------------
    // netlist_t
    // ----------------------------------------------------------------------------------------
    public class netlist_t
    {
        netlist_state_t m_state;  //netlist_state_t &                   m_state;
        devices.nld_solver m_solver;  //devices::NETLIB_NAME(solver) *      m_solver;

        // mostly rw
        netlist_time_ext m_time;
        devices.nld_mainclock m_mainclock;  //devices::NETLIB_NAME(mainclock) *   m_mainclock;

        queue_t m_queue;
        bool m_use_stats;

        // performance
        //plib::pperftime_t<true>             m_stat_mainloop;
        //plib::pperfcount_t<true>            m_perf_out_processed;


        public netlist_t(netlist_state_t state, string aname)
        {
            m_state = state;
            m_solver = null;
            m_time = netlist_time_ext.zero();
            m_mainclock = null;
            m_queue = new queue_t(false, config.MAX_QUEUE_SIZE,
                (net) => { return state.find_net_id(net); },  //detail::queue_t::id_delegate(&netlist_state_t :: find_net_id, &state),
                (id) => { return state.net_by_id(id); });  //detail::queue_t::obj_delegate(&netlist_state_t :: net_by_id, &state))
            m_use_stats = false;


            state.save(this, (plib.state_manager_t.callback_t)m_queue, aname, "m_queue");
            state.save(this, m_time, aname, "m_time");
        }

        //PCOPYASSIGNMOVE(netlist_t, delete)

        //virtual ~netlist_t() noexcept = default;


        // run functions

        public netlist_time_ext time() { return m_time; }


        public void process_queue(netlist_time_ext delta)
        {
            if (!m_use_stats)
            {
                process_queue_stats(false, delta, m_mainclock);
            }
            else
            {
                //throw new emu_unimplemented();
#if false
                auto sm_guard(m_stat_mainloop.guard());
#endif
                process_queue_stats(true, delta, m_mainclock);
            }
        }


        //template <bool KEEP_STATS, typename MCT>
        void process_queue_stats(bool KEEP_STATS, netlist_time_ext delta, devices.nld_mainclock mainclock)  //inline void netlist_t::process_queue_stats(const netlist_time delta, MCT *mainclock) NL_NOEXCEPT
        {
            netlist_time_ext stop = m_time + delta;

            qpush(new queue_t_entry_t(stop, null));

            if (m_mainclock == null)
            {
                m_time = m_queue.top().exec_time();
                detail.net_t obj = m_queue.top().object_();
                m_queue.pop();

                while (obj != null)
                {
                    obj.update_devs(KEEP_STATS);

                    //throw new emu_unimplemented();
#if false
                    if (KEEP_STATS)
                        m_perf_out_processed.inc();
#endif

                    queue_t_entry_t top = m_queue.top();
                    m_time = top.exec_time();
                    obj = top.object_();
                    m_queue.pop();
                }
            }
            else
            {
                logic_net_t mc_net = m_mainclock.m_Q.net();
                netlist_time inc = m_mainclock.m_inc;
                netlist_time_ext mc_time = mc_net.next_scheduled_time();

                do
                {
                    queue_t_entry_t top = m_queue.top();
                    while (top.exec_time() > mc_time)
                    {
                        m_time = mc_time;
                        mc_net.toggle_new_Q();
                        mc_net.update_devs(KEEP_STATS);
                        top = m_queue.top();
                        mc_time += inc;
                    }

                    m_time = top.exec_time();
                    var obj = top.object_();
                    m_queue.pop();

                    if (obj != null)
                    {
                        throw new emu_unimplemented();
#if false
#endif
                    }
                    else
                    {
                        break;
                    }

                    throw new emu_unimplemented();
#if false
                    if (KEEP_STATS)
                        m_perf_out_processed.inc();
#endif

                } while (true);

                mc_net.set_next_scheduled_time(mc_time);
            }
        }


        //void abort_current_queue_slice() NL_NOEXCEPT { m_queue.retime(detail::queue_t::entry_t(m_time, nullptr)); }

        public queue_t queue() { return m_queue; }


        //template<typename... Args>
        public void qpush(plib.pqentry_t<netlist_time_ext, detail.net_t> e)  //void qpush(Args&&...args) noexcept
        {
            if (!nl_config_global.NL_USE_QUEUE_STATS || !m_use_stats)
                m_queue.emplace(false, e);  //m_queue.emplace<false>(std::forward<Args>(args)...); // NOLINT(performance-move-const-arg)
            else
                m_queue.emplace(true, e);  //m_queue.emplace<true>(std::forward<Args>(args)...); // NOLINT(performance-move-const-arg)
        }


        //template <class R>
        public void qremove(detail.net_t elem)  //void qremove(const R &elem) noexcept
        {
            if (!nl_config_global.NL_USE_QUEUE_STATS || !m_use_stats)
                m_queue.remove(false, elem);
            else
                m_queue.remove(true, elem);
        }


        // Control functions

        public void stop()
        {
            log().debug.op("Printing statistics ...\n");
            print_stats();
            log().debug.op("Stopping solver device ...\n");
            if (m_solver != null)
                m_solver.stop();
        }


        public void reset()
        {
            log().debug.op("Searching for mainclock\n");
            m_mainclock = m_state.get_single_device<devices.nld_mainclock>("mainclock");

            log().debug.op("Searching for solver\n");
            m_solver = m_state.get_single_device<devices.nld_solver>("solver");

            // Don't reset time
            //m_time = netlist_time_ext.zero();
            m_queue.clear();
            if (m_mainclock != null)
                m_mainclock.m_Q.net().set_next_scheduled_time(m_time);
            //if (m_solver != nullptr)
            //  m_solver->reset();

            m_state.reset();
        }


        // only used by nltool to create static c-code
        //devices::NETLIB_NAME(solver) *solver() const NL_NOEXCEPT { return m_solver; }

        // force late type resolution
        //template <typename X = devices::NETLIB_NAME(solver)>
        public nl_fptype gmin(object solv = null)  //nl_fptype gmin(X *solv = nullptr) const noexcept
        {
            return m_solver.gmin();  //return static_cast<X *>(m_solver)->gmin();
        }


        public netlist_state_t nlstate() { return m_state; }

        public log_type log() { return m_state.log(); }


        void print_stats()
        {
            //throw new emu_unimplemented();
#if false
#endif
        }


        public bool use_stats() { return m_use_stats; }


        public bool stats_enabled() { return m_use_stats; }
        //void enable_stats(bool val) { m_use_stats = val; }


        //template <bool KEEP_STATS>
        //void process_queue_stats(netlist_time delta) noexcept;
    }


    // -----------------------------------------------------------------------------
    // Support classes for devices
    // -----------------------------------------------------------------------------
    //template<class C, std::size_t N>
    class object_array_base_t<C, size_t_N> : plib.static_vector<C, size_t_N>  //class object_array_base_t : public plib::static_vector<C, N>
        where size_t_N : size_t_constant, new()
    {
        //template<class D, typename... Args>
        //object_array_base_t(D &dev, const std::initializer_list<const char *> &names, Args&&... args)
        protected object_array_base_t(object dev, std.array<string, size_t_N> names)  //object_array_base_t(D &dev, std::array<const char *, N> &&names, Args&&... args)
        {
            throw new emu_unimplemented();
#if false
            for (std::size_t i = 0; i<N; i++)
                this->emplace_back(dev, pstring(names[i]), std::forward<Args>(args)...);
#endif
        }

        //template<class D>
        protected object_array_base_t(object dev, string fmt)  //object_array_base_t(D &dev, const pstring &fmt)
        {
            throw new emu_unimplemented();
#if false
            for (size_t i = 0; i < N; i++)
                this->emplace_back(dev, formatted(fmt, i));
#endif
        }

        //template<class D, typename... Args>
        protected object_array_base_t(object dev, size_t offset, string fmt)  //object_array_base_t(D &dev, std::size_t offset, const pstring &fmt, Args&&... args)
        {
            throw new emu_unimplemented();
#if false
            for (size_t i = 0; i < N; i++)
                this->emplace_back(dev, formatted(fmt, i+offset), std::forward<Args>(args)...);
#endif
        }

        //template<class D>
        protected object_array_base_t(object dev, size_t offset, string fmt, nldelegate delegate_)  //object_array_base_t(D &dev, std::size_t offset, const pstring &fmt, nldelegate delegate)
        {
            throw new emu_unimplemented();
#if false
            for (size_t i = 0; i < N; i++)
                this->emplace_back(dev, formatted(fmt, i+offset), delegate);
#endif
        }

        //template<class D>
        protected object_array_base_t(object dev, size_t offset, size_t qmask, string fmt)  //object_array_base_t(D &dev, std::size_t offset, std::size_t qmask, const pstring &fmt)
        {
            throw new emu_unimplemented();
#if false
            for (size_t i = 0; i < N; i++)
            {
                string name = formatted(fmt, i + offset);
                if (((qmask >> (int)i) & 1) != 0)
                    name += "Q";

                this.emplace(i, dev, name);
            }
#endif
        }

        protected object_array_base_t() : base() { }


        static string formatted(string fmt, size_t n)
        {
            if (N.value != 1)
                return new plib.pfmt(fmt).op(n);

            return new plib.pfmt(fmt).op("");
        }
    }


    //template<class C, std::size_t N>
    class object_array_t<C, size_t_N> : object_array_base_t<C, size_t_N>  //class object_array_t : public object_array_base_t<C, N>
        where size_t_N : size_t_constant, new()
    {
        //using base_type = object_array_base_t<C, N>;
        //using base_type::base_type;


        //template<class D, typename... Args>
        public object_array_t(object dev, std.array<string, size_t_N> names) : base(dev, names) { }

        //template<class D>
        public object_array_t(object dev, string fmt) : base(dev, fmt) { }

        //template<class D, typename... Args>
        public object_array_t(object dev, size_t offset, string fmt) : base(dev, offset, fmt) { }

        //template<class D>
        public object_array_t(object dev, size_t offset, string fmt, nldelegate delegate_) : base(dev, offset, fmt, delegate_) { }

        //template<class D>
        public object_array_t(object dev, size_t offset, size_t qmask, string fmt) : base(dev, offset, qmask, fmt) { }

        public object_array_t() : base() { }
    }


    //template<std::size_t N>
    //class object_array_t<logic_input_t,N> : public object_array_base_t<logic_input_t, N>

    //template<std::size_t N>
    //class object_array_t<logic_output_t,N> : public object_array_base_t<logic_output_t, N>

    //template<std::size_t N>
    //class object_array_t<tristate_output_t,N> : public object_array_base_t<tristate_output_t, N>


    // -----------------------------------------------------------------------------
    // power pins - not a device, but a helper
    // -----------------------------------------------------------------------------
    /// \brief Power pins class.
    ///
    /// Power Pins are passive inputs. Delegate noop will silently ignore any
    /// updates.
    class nld_power_pins
    {
        //using this_type = nld_power_pins;


        analog_input_t m_VCC;
        analog_input_t m_GND;


        public nld_power_pins(device_t owner)
        {
            m_VCC = new analog_input_t(owner, owner.logic_family().vcc_pin(), noop);
            m_GND = new analog_input_t(owner, owner.logic_family().gnd_pin(), noop);
        }


        public analog_input_t VCC()
        {
            return m_VCC;
        }

        public analog_input_t GND()
        {
            return m_GND;
        }


        void noop() { }
    }
}
