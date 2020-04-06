// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using devices_collection_type = mame.std.vector<System.Collections.Generic.KeyValuePair<string, mame.netlist.core_device_t>>;  //using devices_collection_type = std::vector<std::pair<pstring, poolptr<core_device_t>>>;
using entry_t = mame.plib.pqentry_t<mame.netlist.detail.net_t, mame.plib.ptime_i64>;
using log_type = mame.plib.plog_base<mame.netlist.callbacks_t>;//, NL_DEBUG>;
using model_map_t = mame.std.unordered_map<string, string>;
using netlist_sig_t = System.UInt32;
using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;
using netlist_time_ext = mame.plib.ptime_i64;  //netlist_time
using nets_collection_type = mame.std.vector<mame.netlist.detail.net_t>;  //using nets_collection_type = std::vector<poolptr<detail::net_t>>;
using nl_fptype = System.Double;
using nlmempool = mame.plib.mempool;
using props = mame.netlist.detail.property_store_t<mame.netlist.detail.object_t, string>;
using state_var_s32 = mame.netlist.state_var<System.Int32>;
using timed_queue = mame.plib.timed_queue_linear;


namespace mame.netlist
{
    //============================================================
    //  MACROS / New Syntax
    //============================================================

    /// Construct a netlist device name
    //#define NETLIB_NAME(chip) nld_ ## chip

    //#define NETLIB_OBJECT_DERIVED(name, pclass)                                   \
    //class NETLIB_NAME(name) : public NETLIB_NAME(pclass)

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
    //class NETLIB_NAME(name) : public device_t

    //#define NETLIB_CONSTRUCTOR_DERIVED(cname, pclass)                              \
    //    private: detail::family_setter_t m_famsetter;                              \
    //    public: template <class CLASS> NETLIB_NAME(cname)(CLASS &owner, const pstring &name) \
    //    : NETLIB_NAME(pclass)(owner, name)

    //#define NETLIB_CONSTRUCTOR_DERIVED_EX(cname, pclass, ...)                      \
    //    private: detail::family_setter_t m_famsetter;                              \
    //    public: template <class CLASS> NETLIB_NAME(cname)(CLASS &owner, const pstring &name, __VA_ARGS__) \
    //    : NETLIB_NAME(pclass)(owner, name)

    /// \brief Used to define the constructor of a netlist device.
    ///  Use this to define the constructor of a netlist device. Please refer to
    ///  #NETLIB_OBJECT for an example.
    //#define NETLIB_CONSTRUCTOR(cname)                                              \
    //    private: detail::family_setter_t m_famsetter;                              \
    //    public: template <class CLASS> NETLIB_NAME(cname)(CLASS &owner, const pstring &name) \
    //        : device_t(owner, name)

    /// \brief Used to define the destructor of a netlist device.
    /// The use of a destructor for netlist device should normally not be necessary.
    //#define NETLIB_DESTRUCTOR(name) public: virtual ~NETLIB_NAME(name)()

    /// \brief Define an extended constructor and add further parameters to it.
    /// The macro allows to add further parameters to a device constructor. This is
    /// normally used for sub-devices and system devices only.
    //#define NETLIB_CONSTRUCTOR_EX(cname, ...)                                      \
    //    private: detail::family_setter_t m_famsetter;                              \
    //    public: template <class CLASS> NETLIB_NAME(cname)(CLASS &owner, const pstring &name, __VA_ARGS__) \
    //        : device_t(owner, name)

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
    //    public: virtual void timestep(const nl_fptype step) override

    /// \brief Used to implement the body of the time stepping code.
    ///
    /// Used when the implementation is outside the class definition
    ///
    /// Please see \ref NETLIB_IS_TIMESTEP for an example.
    ///
    /// \param cname Name of object as given to \ref NETLIB_OBJECT
    ///
    //#define NETLIB_TIMESTEP(cname)                                                 \
    //    void NETLIB_NAME(cname) :: timestep(nl_fptype step) noexcept

    //#define NETLIB_FAMILY(family) , m_famsetter(*this, family)

    //#define NETLIB_DELEGATE(chip, name) nldelegate(&NETLIB_NAME(chip) :: name, this)

    //#define NETLIB_UPDATE_TERMINALSI() virtual void update_terminals() override
    //#define NETLIB_HANDLERI(name) virtual void name() NL_NOEXCEPT
    //#define NETLIB_UPDATEI() virtual void update() NL_NOEXCEPT override
    //#define NETLIB_UPDATE_PARAMI() virtual void update_param() override
    //#define NETLIB_RESETI() virtual void reset() override

    //#define NETLIB_SUB(chip) nld_ ## chip
    //#define NETLIB_SUB_UPTR(ns, chip) unique_pool_ptr< ns :: nld_ ## chip >

    //#define NETLIB_HANDLER(chip, name) void NETLIB_NAME(chip) :: name() NL_NOEXCEPT
    //#define NETLIB_UPDATE(chip) NETLIB_HANDLER(chip, update)

    //#define NETLIB_RESET(chip) void NETLIB_NAME(chip) :: reset(void)

    //#define NETLIB_UPDATE_PARAM(chip) void NETLIB_NAME(chip) :: update_param()

    //#define NETLIB_UPDATE_TERMINALS(chip) void NETLIB_NAME(chip) :: update_terminals()


    static class nl_base_global
    {
        static logic_family_ttl_t family_TTL_obj;
        public static logic_family_desc_t family_TTL() { return family_TTL_obj; }  //*!< logic family for TTL devices.

        static logic_family_cd4xxx_t family_CD4XXX_obj;
        public static logic_family_desc_t family_CD4XXX() { return family_CD4XXX_obj; }  //*!< logic family for CD4XXX CMOS devices.
    }


    /// \brief Generic netlist exception.
    ///  The exception is used in all events which are considered fatal.
    class nl_exception : Exception  //plib.pexception
    {
        /// \brief Constructor.
        ///  Allows a descriptive text to be assed to the exception
        public nl_exception(string text)  //!< text to be passed
            : base(text) { }
        /// \brief Constructor.
        ///  Allows to use \ref plib::pfmt logic to be used in exception
        public nl_exception(string format, params object [] args)  //!< format to be used
            : base(string.Format(format, args)) { }
    }


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


    /// \brief Logic families descriptors are used to create proxy devices.
    ///  The logic family describes the analog capabilities of logic devices,
    ///  inputs and outputs.
    public abstract class logic_family_desc_t
    {
        public nl_fptype m_fixed_V;           //!< For variable voltage families, specify 0. For TTL this would be 5.
        public nl_fptype m_low_thresh_PCNT;   //!< low input threshhold offset. If the input voltage is below this value times supply voltage, a "0" input is signalled
        public nl_fptype m_high_thresh_PCNT;  //!< high input threshhold offset. If the input voltage is above the value times supply voltage, a "0" input is signalled
        public nl_fptype m_low_VO;            //!< low output voltage offset. This voltage is output if the ouput is "0"
        public nl_fptype m_high_VO;           //!< high output voltage offset. The supply voltage minus this offset is output if the ouput is "1"
        public nl_fptype m_R_low;             //!< low output resistance. Value of series resistor used for low output
        public nl_fptype m_R_high;            //!< high output resistance. Value of series resistor used for high output


        public logic_family_desc_t() { }
        //~logic_family_desc_t() { }

        //COPYASSIGNMOVE(logic_family_desc_t, delete)


        public abstract devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_state_t anetlist, string name, logic_output_t proxied);  //virtual unique_pool_ptr<devices::nld_base_d_to_a_proxy> create_d_a_proxy(netlist_state_t &anetlist, const pstring &name, logic_output_t *proxied) const = 0;
        public abstract devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_state_t anetlist, string name, logic_input_t proxied);  //virtual unique_pool_ptr<devices::nld_base_a_to_d_proxy> create_a_d_proxy(netlist_state_t &anetlist, const pstring &name, logic_input_t *proxied) const = 0;


        // FIXME: remove fixed_V()
        //nl_fptype fixed_V() const noexcept{return m_fixed_V; }
        //nl_fptype low_thresh_V(nl_fptype VN, nl_fptype VP) const noexcept{ return VN + (VP - VN) * m_low_thresh_PCNT; }
        //nl_fptype high_thresh_V(nl_fptype VN, nl_fptype VP) const noexcept{ return VN + (VP - VN) * m_high_thresh_PCNT; }
        //nl_fptype low_offset_V() const noexcept{ return m_low_VO; }
        //nl_fptype high_offset_V() const noexcept{ return m_high_VO; }
        //nl_fptype R_low() const noexcept{ return m_R_low; }
        //nl_fptype R_high() const noexcept{ return m_R_high; }

        //bool is_above_high_thresh_V(nl_fptype V, nl_fptype VN, nl_fptype VP) const noexcept
        //{ return (V - VN) > high_thresh_V(VN, VP); }

        //bool is_below_low_thresh_V(nl_fptype V, nl_fptype VN, nl_fptype VP) const noexcept
        //{ return (V - VN) < low_thresh_V(VN, VP); }
    }


    /// \brief Base class for devices, terminals, outputs and inputs which support
    ///  logic families.
    ///  This class is a storage container to store the logic family for a
    ///  netlist object. You will not directly use it. Please refer to
    ///  \ref NETLIB_FAMILY to learn how to define a logic family for a device.
    ///
    /// All terminals inherit the family description from the device
    /// The default is the ttl family, but any device can override the family.
    /// For individual terminals, the family can be overwritten as well.
    ///
    interface logic_family_t
    {
        //const logic_family_desc_t *m_logic_family;

        //logic_family_t() : m_logic_family(nullptr) {}
        //~logic_family_t() { } // prohibit polymorphic destruction
        //COPYASSIGNMOVE(logic_family_t, delete)

        //const logic_family_desc_t *logic_family() const { return m_logic_family; }
        //void set_logic_family(const logic_family_desc_t *fam) { m_logic_family = fam; }
        logic_family_desc_t logic_family();
        void set_logic_family(logic_family_desc_t fam);
    }


    //const logic_family_desc_t *family_TTL();        ///< logic family for TTL devices.
    //const logic_family_desc_t *family_CD4XXX();     ///< logic family for CD4XXX CMOS devices.


    /// \brief A persistent variable template.
    ///  Use the state_var template to define a variable whose value is saved.
    ///  Within a device definition use
    ///
    ///      NETLIB_OBJECT(abc)
    ///      {
    ///          NETLIB_CONSTRUCTOR(abc)
    ///          , m_var(*this, "myvar", 0)
    ///          ...
    ///          state_var<unsigned> m_var;
    ///      }
    //template <typename T>
    public class state_var<T>
    {
        T m_value;


        //template <typename O>
        //! Constructor.
        public state_var(
            detail.netlist_interface_plus_name owner,             //!< owner must have a netlist() method.
            string name,     //!< identifier/name for this state variable
            T value)         //!< Initial value after construction
        {
            m_value = value;


            owner.state().save(owner, m_value, owner.name(), name);
        }


        //! Destructor.
        //~state_var() noexcept = default;
        //! Copy Constructor.
        //constexpr state_var(const state_var &rhs) = default;
        //! Move Constructor.
        //constexpr state_var(state_var &&rhs) noexcept = default;
        //! Assignment operator to assign value of a state var.
        //C14CONSTEXPR state_var &operator=(const state_var &rhs) = default; // OSX doesn't like noexcept
        //! Assignment move operator to assign value of a state var.
        //C14CONSTEXPR state_var &operator=(state_var &&rhs) noexcept = default;
        //! Assignment operator to assign value of type T.
        //C14CONSTEXPR state_var &operator=(const T &rhs) noexcept { m_value = rhs; return *this; }
        //! Assignment move operator to assign value of type T.
        //C14CONSTEXPR state_var &operator=(T &&rhs) noexcept { std::swap(m_value, rhs); return *this; }
        //! Return non-const value of state variable.
        //C14CONSTEXPR operator T & () noexcept { return m_value; }
        //! Return const value of state variable.
        //constexpr operator const T & () const noexcept { return m_value; }
        //! Return non-const value of state variable.
        //C14CONSTEXPR T & operator ()() noexcept { return m_value; }
        //! Return const value of state variable.
        //constexpr const T & operator ()() const noexcept { return m_value; }
        //! Return pointer to state variable.
        //C14CONSTEXPR T * ptr() noexcept { return &m_value; }
        //! Return const pointer to state variable.
        //constexpr const T * ptr() const noexcept{ return &m_value; }

        public T op { get { return m_value; } set { m_value = value; } }
    }


    // -----------------------------------------------------------------------------
    // State variables - predefined and c++11 non-optional
    // -----------------------------------------------------------------------------

    /// \brief predefined state variable type for uint8_t
    //using state_var_u8 = state_var<std::uint8_t>;
    /// \brief predefined state variable type for int8_t
    //using state_var_s8 = state_var<std::int8_t>;

    /// \brief predefined state variable type for uint32_t
    //using state_var_u32 = state_var<std::uint32_t>;
    /// \brief predefined state variable type for int32_t
    //using state_var_s32 = state_var<std::int32_t>;
    /// \brief predefined state variable type for sig_t
    //using state_var_sig = state_var<netlist_sig_t>;


    namespace detail
    {
        //template <typename C, typename T>
        struct property_store_t<C, T>
        {
            public static void add(C obj, T aname)
            {
                store().insert(obj, aname);
            }


            public static T get(C obj)
            {
                try
                {
                    var ret = store().find(obj);
                    nl_config_global.nl_assert(!ret.Equals(default(T)));  //nl_assert(ret != store().end());
                    return ret;
                }
                catch (Exception)
                {
                    nl_config_global.nl_assert_always(true, "exception in property_store_t.get()");
                    return default;  //return *static_cast<T *>(nullptr);
                }
            }


            public static void remove(C obj)
            {
                store().erase(obj);  //store().find(obj));
            }


            static std.unordered_map<C, T> lstore;

            static std.unordered_map<C, T> store()
            {
                return lstore;
            }
        }


        // -----------------------------------------------------------------------------
        // object_t
        // -----------------------------------------------------------------------------

        /// \brief The base class for netlist devices, terminals and parameters.
        ///
        ///  This class serves as the base class for all device, terminal and
        ///  objects. It provides new and delete operators to support e.g. pooled
        ///  memory allocation to enhance locality. Please refer to \ref NL_USE_MEMPOOL as
        ///  well.
        public class object_t : global_object
        {
            //using props = property_store_t<object_t, pstring>;


            /// \brief Constructor.
            /// Every class derived from the object_t class must have a name.
            ///
            /// \param aname string containing name of the object
            public object_t(string aname /*!< string containing name of the object */)
            {
                props.add(this, aname);
            }

            //COPYASSIGNMOVE(object_t, delete)

            // only childs should be destructible
            ~object_t()
            {
                props.remove(this);
            }


            /// \brief return name of the object
            ///
            /// \returns name of the object.
            public string name() { return props.get(this); }
        }


        struct netlist_ref //: netlist_interface
        {
            netlist_t m_netlist;


            public netlist_ref(netlist_t nl) { m_netlist = nl; }

            //COPYASSIGNMOVE(netlist_ref, delete)

            public netlist_state_t state() { return m_netlist.nlstate(); }

            public netlist_t exec() { return m_netlist; }
        }


        // -----------------------------------------------------------------------------
        // device_object_t
        // -----------------------------------------------------------------------------

        /// \brief Base class for all objects being owned by a device.
        ///
        /// Serves as the base class of all objects being owned by a device.
        ///
        public class device_object_t : detail.object_t, netlist_interface_plus_name
        {
            core_device_t m_device;


            /// \brief Constructor.
            ///
            /// \param dev  device owning the object.
            /// \param name string holding the name of the device
            public device_object_t(core_device_t dev, string aname)
                : base(aname)
            {
                m_device = dev;
            }

            /// \brief returns reference to owning device.
            /// \returns reference to owning device.
            public core_device_t device() { return m_device; }

            /// \brief The netlist owning the owner of this object.
            /// \returns reference to netlist object.
            public netlist_state_t state() { return m_device.state(); }

            public netlist_t exec() { return m_device.exec(); }
        }


        // -----------------------------------------------------------------------------
        // core_terminal_t
        // -----------------------------------------------------------------------------
        /// \brief Base class for all terminals.
        ///
        /// All terminals are derived from this class.
        ///
        public class core_terminal_t : device_object_t
                                       //public plib::linkedlist_t<core_terminal_t>::element_t
        {
            /// \brief Number of signal bits
            ///
            /// Going forward setting this to 8 will allow 8-bit signal
            /// busses to be used in netlist, e.g. for more complex memory
            /// arrangements.
            const int INP_BITS = 1;

            const int INP_MASK = (1 << INP_BITS) - 1;
            public const int INP_HL_SHIFT = 0;
            public const int INP_LH_SHIFT = INP_BITS;


            public enum state_e
            {
                STATE_INP_PASSIVE = 0,
                STATE_INP_HL      = (INP_MASK << INP_HL_SHIFT),
                STATE_INP_LH      = (INP_MASK << INP_LH_SHIFT),
                STATE_INP_ACTIVE  = STATE_INP_HL | STATE_INP_LH,
                STATE_OUT         = (1 << (2 * INP_BITS)),
                STATE_BIDIR       = (1 << (2 * INP_BITS + 1))
            }


            nldelegate m_delegate;
            net_t m_net;
            state_var<state_e> m_state;


            public core_terminal_t(core_device_t dev, string aname, state_e state, nldelegate delegate_ = null)  //nldelegate())
                : base(dev, dev.name() + "." + aname)
            {
#if NL_USE_COPY_INSTEAD_OF_REFERENCE
                , m_Q(*this, "m_Q", 0)
#endif
                m_delegate = delegate_;
                m_net = null;
                m_state = new state_var<state_e>(this, "m_state", state);
            }

            //~core_terminal_t() { }

            //COPYASSIGNMOVE(core_terminal_t, delete)


            /// \brief The object type.
            /// \returns type of the object
            public terminal_type type()
            {
                if (this is terminal_t)  //if (dynamic_cast<const terminal_t *>(this) != nullptr)                
                    return terminal_type.TERMINAL;
                if (this is logic_input_t  //else if (dynamic_cast<const logic_input_t *>(this) != nullptr
                    || this is analog_input_t)  //|| dynamic_cast<const analog_input_t *>(this) != nullptr)
                    return terminal_type.INPUT;
                if (this is logic_output_t  //else if (dynamic_cast<const logic_output_t *>(this) != nullptr
                    || this is analog_output_t)  //|| dynamic_cast<const analog_output_t *>(this) != nullptr)
                    return terminal_type.OUTPUT;

                state().log().fatal.op(nl_errstr_global.MF_UNKNOWN_TYPE_FOR_OBJECT(name()));
                throw new nl_exception(nl_errstr_global.MF_UNKNOWN_TYPE_FOR_OBJECT(name()));
                //return terminal_type.TERMINAL; // please compiler
            }


            /// \brief Checks if object is of specified type.
            /// \param atype type to check object against.
            /// \returns true if object is of specified type else false.
            public bool is_type(terminal_type atype) { return type() == atype; }


            public void set_net(net_t anet) { m_net = anet; }
            public void clear_net() { m_net = null; }
            public bool has_net() { return m_net != null; }
            public net_t net() { return m_net; }


            public bool is_logic() { return this is logic_t; }  //return dynamic_cast<const logic_t *>(this) != nullptr;
            public bool is_logic_input() { return this is logic_input_t; }  //return dynamic_cast<const logic_input_t *>(this) != nullptr;
            public bool is_logic_output() { return this is logic_output_t; }  //return dynamic_cast<const logic_output_t *>(this) != nullptr;
            public bool is_analog() { return this is analog_t; }  //return dynamic_cast<const analog_t *>(this) != nullptr;
            bool is_analog_input() { return this is analog_input_t; }  //return dynamic_cast<const analog_input_t *>(this) != nullptr;
            public bool is_analog_output() { return this is analog_output_t; }  //return dynamic_cast<const analog_output_t *>(this) != nullptr;

            //bool is_state(const state_e &astate) const NL_NOEXCEPT { return (m_state == astate); }
            public state_e terminal_state() { return m_state.op; }
            void set_state(state_e astate) { m_state.op = astate; }

            public void reset() { set_state(is_type(terminal_type.OUTPUT) ? state_e.STATE_OUT : state_e.STATE_INP_ACTIVE); }

            public void set_copied_input(netlist_sig_t val) { }


            public void set_delegate(nldelegate delegate_) { m_delegate = delegate_; }
            public nldelegate delegate_() { return m_delegate; }
            public void run_delegate() { m_delegate(); }
        }
    }


    /*! Delegate type for device notification.
        *
        */
    //using nldelegate = plib::pmfp<void>;
    public delegate void nldelegate();


    // -----------------------------------------------------------------------------
    // analog_t
    // -----------------------------------------------------------------------------
    public class analog_t : detail.core_terminal_t
    {
        public analog_t(core_device_t dev, string aname, state_e state, nldelegate delegate_ = null)
            : base(dev, aname, state, delegate_)
        {
        }


        //const analog_net_t & net() const NL_NOEXCEPT;
        //analog_net_t & net() NL_NOEXCEPT;
        public analog_net_t net()
        {
            return (analog_net_t)base.net();  //return static_cast<const analog_net_t &>(core_terminal_t::net());
        }
    }


    // -----------------------------------------------------------------------------
    // terminal_t
    // -----------------------------------------------------------------------------
    public class terminal_t : analog_t
    {
        ListPointer<nl_fptype> m_Idr1;  //nl_fptype *m_Idr1; // drive current
        ListPointer<nl_fptype> m_go1;   //nl_fptype *m_go1;  // conductance for Voltage from other term
        ListPointer<nl_fptype> m_gt1;   //nl_fptype *m_gt1;  // conductance for total conductance


        // ----------------------------------------------------------------------------------------
        // terminal_t
        // ----------------------------------------------------------------------------------------
        public terminal_t(core_device_t dev, string aname, terminal_t otherterm)
            : base(dev, aname, state_e.STATE_BIDIR)
        {
            m_Idr1 = null;
            m_go1 = null;
            m_gt1 = null;


            state().setup().register_term(this, otherterm);
        }

        //~terminal_t() { }


        //nl_fptype operator ()() const  NL_NOEXCEPT;

        //void set_conductivity(const nl_fptype &G) NL_NOEXCEPT
        //void set_go_gt(const nl_fptype &GO, const nl_fptype &GT) NL_NOEXCEPT

        public void set_go_gt_I(nl_fptype GO, nl_fptype GT, nl_fptype I)
        {
            // Check for rail nets ...
            if (m_go1 != null)
            {
                m_Idr1[0] = I;  //*m_Idr1 = I;
                m_go1[0] = GO;  //*m_go1 = GO;
                m_gt1[0] = GT;  //*m_gt1 = GT;
            }
        }


        public void solve_now()
        {
            // Nets may belong to railnets which do not have a solver attached
            if (this.has_net())
            {
                if (net().solver() != null)
                    net().solver().update_forced();
            }
        }


        //void schedule_solve_after(const netlist_time &after);


        public void set_ptrs(ListPointer<nl_fptype> gt, ListPointer<nl_fptype> go, ListPointer<nl_fptype> Idr)  //void set_ptrs(nl_fptype *gt, nl_fptype *go, nl_fptype *Idr) noexcept(false);
        {
            if (!(gt != null && go != null && Idr != null) && (gt != null || go != null || Idr != null))  //if (!(gt && go && Idr) && (gt || go || Idr))
            {
                state().log().fatal.op("Inconsistent nullptrs for terminal {0}", name());
                throw new nl_exception("Inconsistent nullptrs for terminal {0}", name());
            }

            m_gt1 = new ListPointer<nl_fptype>(gt);  //m_gt1 = gt;
            m_go1 = new ListPointer<nl_fptype>(go);  //m_go1 = go;
            m_Idr1 = new ListPointer<nl_fptype>(Idr);  //m_Idr1 = Idr;
        }
    }


    // -----------------------------------------------------------------------------
    // logic_t
    // -----------------------------------------------------------------------------
    public class logic_t : detail.core_terminal_t,
                           logic_family_t
    {
        public logic_t(core_device_t dev, string aname, state_e state, nldelegate delegate_ = null)  // = nldelegate());
            : base(dev, aname, state, delegate_)
        {
        }


        // logic_family_t
        logic_family_desc_t m_logic_family;
        public logic_family_desc_t logic_family() { return m_logic_family; }
        public void set_logic_family(logic_family_desc_t fam) { m_logic_family = fam; }


        //logic_net_t & net() NL_NOEXCEPT;
        //const logic_net_t &  net() const NL_NOEXCEPT;
        public new logic_net_t net() { return (logic_net_t)base.net(); }
    }


    // -----------------------------------------------------------------------------
    // logic_input_t
    // -----------------------------------------------------------------------------
    public class logic_input_t : logic_t
    {
        public logic_input_t(core_device_t dev, string aname, nldelegate delegate_ = null)  // = nldelegate());
            : base(dev, aname, state_e.STATE_INP_ACTIVE, delegate_)
        {
            set_logic_family(dev.logic_family());
            state().setup().register_term(this);
        }


        //netlist_sig_t operator()() const NL_NOEXCEPT

        //void inactivate() NL_NOEXCEPT;
        //void activate() NL_NOEXCEPT;
        //void activate_hl() NL_NOEXCEPT;
        //void activate_lh() NL_NOEXCEPT;

        //netlist_sig_t Q() const NL_NOEXCEPT;
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
                              nldelegate delegate_ = null) ///< delegate
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
        nl_fptype Q_Analog() { return net().Q_Analog(); }
    }


    namespace detail
    {
        // -----------------------------------------------------------------------------
        // net_t
        // -----------------------------------------------------------------------------
        public class net_t : detail.object_t,
                             detail.netlist_interface_plus_name  //public detail::netlist_ref
        {
            enum queue_status
            {
                DELAYED_DUE_TO_INACTIVE = 0,
                QUEUED,
                DELIVERED
            }


            netlist_ref m_netlist_ref;


            state_var<netlist_sig_t> m_new_Q;
            state_var<netlist_sig_t> m_cur_Q;
            state_var<queue_status> m_in_queue;
            state_var<netlist_time_ext> m_next_scheduled_time;

            core_terminal_t m_railterminal;
            std.vector<core_terminal_t> m_list_active = new std.vector<core_terminal_t>();  //plib::linkedlist_t<core_terminal_t> m_list_active;
            std.vector<core_terminal_t> m_core_terms = new std.vector<core_terminal_t>(); // save post-start m_list ...


            public net_t(netlist_state_t nl, string aname, core_terminal_t railterminal = null)
                : base(aname)
                //, netlist_ref(nl)
            {
                m_netlist_ref = new netlist_ref(nl.exec());

                m_new_Q = new state_var<netlist_sig_t>(this, "m_new_Q", 0);
                m_cur_Q = new state_var<netlist_sig_t>(this, "m_cur_Q", 0);
                m_in_queue = new state_var<queue_status>(this, "m_in_queue", queue_status.DELIVERED);
                m_next_scheduled_time = new state_var<netlist_time_ext>(this, "m_time", netlist_time_ext.zero());
                m_railterminal = railterminal;
            }

            //COPYASSIGNMOVE(net_t, delete)

            //virtual ~net_t() noexcept = default;


            protected state_var<netlist_sig_t> new_Q { get { return m_new_Q; } set { m_new_Q = value; } }
            protected state_var<netlist_sig_t> cur_Q { get { return m_cur_Q; } set { m_cur_Q = value; } }


            // netlist_interface
            public netlist_state_t state() { return m_netlist_ref.state(); }
            public netlist_t exec() { return m_netlist_ref.exec(); }


            public void reset()
            {
                m_next_scheduled_time.op = netlist_time_ext.zero();
                m_in_queue.op = queue_status.DELIVERED;

                m_new_Q.op = 0;
                m_cur_Q.op = 0;

                var p = this is analog_net_t ? (analog_net_t)this : null;

                if (p != null)
                    p.cur_Analog.op[0] = nlconst.zero();

                // rebuild m_list and reset terminals to active or analog out state

                m_list_active.clear();
                foreach (core_terminal_t ct in m_core_terms)
                {
                    ct.reset();
                    if (ct.terminal_state() != logic_t.state_e.STATE_INP_PASSIVE)
                        m_list_active.push_back(ct);

                    ct.set_copied_input(m_cur_Q.op);
                }
            }


            public void toggle_new_Q() { m_new_Q.op = (m_cur_Q.op ^ 1); }


            public void toggle_and_push_to_queue(netlist_time delay)
            {
                toggle_new_Q();
                push_to_queue(delay);
            }


            protected void push_to_queue(netlist_time delay)
            {
                if ((num_cons() != 0))
                {
                    m_next_scheduled_time.op = exec().time() + delay;

                    if (!!is_queued())
                        exec().qremove(this);

                    if (!m_list_active.empty())
                    {
                        m_in_queue.op = queue_status.QUEUED;
                        exec().qpush(new entry_t(m_next_scheduled_time.op, this));
                    }
                    else
                    {
                        m_in_queue.op = queue_status.DELAYED_DUE_TO_INACTIVE;
                        update_inputs();
                    }
                }
            }


            public bool is_queued() { return m_in_queue.op == queue_status.QUEUED; }


            //template <bool KEEP_STATS>
            public void update_devs(bool KEEP_STATS)
            {
                nl_config_global.nl_assert(this.isRailNet());

                m_in_queue.op = queue_status.DELIVERED; // mark as taken ...

                if ((m_new_Q.op ^ m_cur_Q.op) != 0)
                    process(KEEP_STATS, (m_new_Q.op << core_terminal_t.INP_LH_SHIFT)
                        | (m_cur_Q.op << core_terminal_t.INP_HL_SHIFT), m_new_Q.op);
            }


            public netlist_time_ext next_scheduled_time() { return m_next_scheduled_time.op; }
            public void set_next_scheduled_time(netlist_time_ext ntime) { m_next_scheduled_time.op = ntime; }

            public bool isRailNet() { return m_railterminal != null; }
            public core_terminal_t railterminal() { return m_railterminal; }

            public int num_cons() { return m_core_terms.size(); }

            //void add_to_active_list(core_terminal_t &term) NL_NOEXCEPT;
            //void remove_from_active_list(core_terminal_t &term) NL_NOEXCEPT;

            // setup stuff

            public void add_terminal(core_terminal_t terminal)
            {
                foreach (var t in m_core_terms)
                {
                    if (t == terminal)
                    {
                        state().log().fatal.op(nl_errstr_global.MF_NET_1_DUPLICATE_TERMINAL_2(name(), t.name()));
                        throw new nl_exception(nl_errstr_global.MF_NET_1_DUPLICATE_TERMINAL_2(name(), t.name()));
                    }
                }

                terminal.set_net(this);

                m_core_terms.push_back(terminal);
            }


            public void remove_terminal(core_terminal_t terminal)
            {
                if (m_core_terms.Contains(terminal))  //(plib::container::contains(m_core_terms, &terminal))
                {
                    terminal.set_net(null);
                    m_core_terms.Remove(terminal);  //plib::container::remove(m_core_terms, &terminal);
                }
                else
                {
                    state().log().fatal.op(nl_errstr_global.MF_REMOVE_TERMINAL_1_FROM_NET_2(terminal.name(), this.name()));
                    throw new nl_exception(nl_errstr_global.MF_REMOVE_TERMINAL_1_FROM_NET_2(terminal.name(), this.name()));
                }
            }


            //bool is_logic() const NL_NOEXCEPT;
            //bool detail::net_t::is_analog() const NL_NOEXCEPT { return dynamic_cast<const analog_net_t *>(this) != nullptr; }
            public bool is_analog() { return this is analog_net_t; }


            public void rebuild_list()     // rebuild m_list after a load
            {
                // rebuild m_list

                m_list_active.clear();
                foreach (var term in m_core_terms)
                {
                    if (term.terminal_state() != logic_t.state_e.STATE_INP_PASSIVE)
                    {
                        m_list_active.push_back(term);
                        term.set_copied_input(m_cur_Q.op);
                    }
                }
            }


            public void move_connections(net_t dest_net)
            {
                foreach (var ct in m_core_terms)
                    dest_net.add_terminal(ct);

                m_core_terms.clear();
            }


            public std.vector<core_terminal_t> core_terms() { return m_core_terms; }


            void update_inputs()
            {
                // nothing needs to be done
            }


            // only used for logic nets
            //netlist_sig_t Q() const noexcept { return m_cur_Q; }


            // only used for logic nets
            public void initial(netlist_sig_t val)
            {
                m_cur_Q.op = m_new_Q.op = val;
                update_inputs();
            }


            // only used for logic nets
            public void set_Q_and_push(netlist_sig_t newQ, netlist_time delay)
            {
                if (newQ != m_new_Q.op)
                {
                    m_new_Q.op = newQ;
                    push_to_queue(delay);
                }
            }


            // only used for logic nets
            //void set_Q_time(const netlist_sig_t newQ, const netlist_time_ext at) NL_NOEXCEPT

            // internal state support
            // FIXME: get rid of this and implement export/import in MAME

            // only used for logic nets
            //netlist_sig_t *Q_state_ptr() { return m_cur_Q.ptr(); }


            //template <bool KEEP_STATS, typename T>
            void process(bool KEEP_STATS, UInt32 mask, netlist_sig_t sig)  //void process(const T mask, netlist_sig_t sig)
            {
                m_cur_Q.op = sig;

                if (KEEP_STATS)
                {
                    foreach (var p in m_list_active)
                    {
                        p.set_copied_input(sig);

                        throw new emu_unimplemented();
#if false
                        auto *stats = p.device().m_stats.get();
                        stats->m_stat_call_count.inc();
                        if ((p.terminal_state() & mask))
                        {
                            auto g(stats->m_stat_total_time.guard());
                            p.run_delegate();
                        }
#endif
                    }
                }
                else
                {
                    foreach (var p in m_list_active)
                    {
                        p.set_copied_input(sig);
                        if (((UInt32)p.terminal_state() & mask) != 0)
                        {
                            p.run_delegate();
                        }
                    }
                }
            }
        }
    }


    public class logic_net_t : detail.net_t
    {
        public logic_net_t(netlist_state_t nl, string aname, detail.core_terminal_t railterminal = null)
            : base(nl, aname, railterminal)
        {
        }
    }


    public class analog_net_t : detail.net_t
    {
        //using list_t =  std::vector<analog_net_t *>;
        public class list_t : std.vector<analog_net_t>
        {

        }

        //friend class detail::net_t;


        state_var<ListPointer<nl_fptype>> m_cur_Analog;  //state_var<nl_fptype>     m_cur_Analog;
        solver.matrix_solver_t m_solver;


        // ----------------------------------------------------------------------------------------
        // analog_net_t
        // ----------------------------------------------------------------------------------------
        public analog_net_t(netlist_state_t nl, string aname, detail.core_terminal_t railterminal = null)
            : base(nl, aname, railterminal)
        {
            m_cur_Analog = new state_var<ListPointer<nl_fptype>>(this, "m_cur_Analog", new ListPointer<nl_fptype>(new std.vector<nl_fptype>(1)));  //, m_cur_Analog(*this, "m_cur_Analog", nlconst::zero())
            m_solver = null;
        }


        public state_var<ListPointer<nl_fptype>> cur_Analog { get { return m_cur_Analog; } set { m_cur_Analog = value; } }


        public nl_fptype Q_Analog() { return m_cur_Analog.op[0]; }
        public void set_Q_Analog(nl_fptype v) { m_cur_Analog.op[0] = v; }
        public ListPointer<nl_fptype> Q_Analog_state_ptr() { return m_cur_Analog.op; }  //nl_fptype *Q_Analog_state_ptr() noexcept { return m_cur_Analog.ptr(); }

        //FIXME: needed by current solver code
        public solver.matrix_solver_t solver() { return m_solver; }
        public void set_solver(solver.matrix_solver_t solver) { m_solver = solver; }
    }


    // -----------------------------------------------------------------------------
    // logic_output_t
    // -----------------------------------------------------------------------------
    public class logic_output_t : logic_t
    {
        logic_net_t m_my_net;


        public logic_output_t(core_device_t dev, string aname)
            : base(dev, aname, state_e.STATE_OUT)
        {
            m_my_net = new logic_net_t(dev.state(), name() + ".net", this);


            set_net(m_my_net);
            state().register_net(m_my_net);  //state().register_net(poolptr<logic_net_t>(&m_my_net, false));
            set_logic_family(dev.logic_family());
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
    }


    // -----------------------------------------------------------------------------
    // core_device_t
    // -----------------------------------------------------------------------------
    public class core_device_t : detail.object_t,
                                 logic_family_t,
                                 detail.netlist_interface_plus_name  //public detail::netlist_ref
    {
        detail.netlist_ref m_netlist_ref;


        /* stats */
        class stats_t
        {
            // NL_KEEP_STATISTICS
            //plib::pperftime_t<true>  m_stat_total_time;
            //plib::pperfcount_t<true> m_stat_call_count;
            //plib::pperfcount_t<true> m_stat_inc_active;
        }
        stats_t m_stats;  //unique_pool_ptr<stats_t> m_stats;

        bool m_hint_deactivate;
        state_var_s32 m_active_outputs;


        // the only common type between netlist_state_t and core_device_t is 'object'.
        // so we accept 'object' as the parameter, and disambiguate below
        protected core_device_t(object owner, string name)
            : base(owner is netlist_state_t ? name : (owner is core_device_t ? ((core_device_t)owner).name() + "." + name : ""))
        {
            assert(owner is netlist_state_t || owner is core_device_t);

            if (owner is netlist_state_t)
                core_device_t_ctor((netlist_state_t)owner, name);
            else if (owner is core_device_t)
                core_device_t_ctor((core_device_t)owner, name);
            else
                throw new emu_fatalerror("unknown type: {0}", owner.GetType());
        }


        protected void core_device_t_ctor(netlist_state_t owner, string name)
            //: base(name)
            //, netlist_ref(owner)
        {
            m_netlist_ref = new detail.netlist_ref(owner.exec());


            m_hint_deactivate = false;
            m_active_outputs = new state_var_s32(this, "m_active_outputs", 1);


            if (logic_family() == null)
                set_logic_family(nl_base_global.family_TTL());
            if (exec().stats_enabled())
                m_stats = new stats_t();  //m_stats = owner.make_object<stats_t>();
        }


        protected void core_device_t_ctor(core_device_t owner, string name)
            //: base(owner.name() + "." + name)
            //, netlist_ref(owner.netlist())
        {
            m_netlist_ref = new detail.netlist_ref(owner.state().exec());


            m_hint_deactivate = false;
            m_active_outputs = new state_var_s32(this, "m_active_outputs", 1);


            set_logic_family(owner.logic_family());
            if (logic_family() == null)
                set_logic_family(nl_base_global.family_TTL());
            owner.state().register_device(this.name(), this);
            if (exec().stats_enabled())
                m_stats = new stats_t();  //m_stats = owner.state().make_object<stats_t>();
        }

        //COPYASSIGNMOVE(core_device_t, delete)

        //virtual ~core_device_t() noexcept = default;


        // netlist_interface
        public netlist_state_t state() { return m_netlist_ref.state(); }
        public netlist_t exec() { return m_netlist_ref.exec(); }


        // logic_family_t
        protected logic_family_desc_t m_logic_family;
        public logic_family_desc_t logic_family() { return m_logic_family; }
        public void set_logic_family(logic_family_desc_t fam) { m_logic_family = fam; }


        //void do_inc_active() NL_NOEXCEPT
        //void do_dec_active() NL_NOEXCEPT


        public void set_hint_deactivate(bool v) { m_hint_deactivate = v; }
        public bool get_hint_deactivate() { return m_hint_deactivate; }
        /* Has to be set in device reset */
        //void set_active_outputs(int n) { m_active_outputs = n; }


        public void set_default_delegate(detail.core_terminal_t term)
        {
            if (term.delegate_() != null)  //if (!term.delegate().is_set())
                term.set_delegate(update);  //term.set_delegate(nldelegate(&core_device_t::update, this));
        }


        protected virtual void update() { }
        public virtual void reset() { }
        protected virtual void inc_active() {  }
        protected virtual void dec_active() {  }


        protected log_type log() { return state().log(); }


        public virtual void timestep(nl_fptype st) { }
        public virtual void update_terminals() { }

        public virtual void update_param() {}
        public virtual bool is_dynamic() { return false; }
        public virtual bool is_timestep() { return false; }
    }


    // -----------------------------------------------------------------------------
    // device_t
    // -----------------------------------------------------------------------------
    public class device_t : core_device_t
    {
        //protected device_t(netlist_base_t owner, string name) : base(owner, name) { }
        //protected device_t(core_device_t owner, string name) : base(owner, name) { }
        protected device_t(object owner, string name) : base(owner, name) { assert(owner is netlist_state_t || owner is core_device_t); }

        //COPYASSIGNMOVE(device_t, delete)

        //~device_t()
        //{
        //    //log().debug("~net_device_t\n");
        //}


        public setup_t setup() { return state().setup(); }

        //template<class C, typename... Args>
        //void create_and_register_subdevice(const pstring &name, unique_pool_ptr<C> &dev, const Args&... args)


        protected void register_subalias(string name, detail.core_terminal_t term)
        {
            string alias = this.name() + "." + name;

            // everything already fully qualified
            state().setup().register_alias_nofqn(alias, term.name());
        }

        //void register_subalias(const pstring &name, const pstring &aliased);


        //protected void connect(string t1, string t2);
        protected void connect(detail.core_terminal_t t1, detail.core_terminal_t t2)
        {
            state().setup().register_link_fqn(t1.name(), t2.name());
        }


        // FIXME: this is only used by solver code since matrix solvers are started in
        //        post_start.
        protected void connect_post_start(detail.core_terminal_t t1, detail.core_terminal_t t2)
        {
            if (!setup().connect(t1, t2))
            {
                log().fatal.op(nl_errstr_global.MF_ERROR_CONNECTING_1_TO_2(t1.name(), t2.name()));
                throw new nl_exception(nl_errstr_global.MF_ERROR_CONNECTING_1_TO_2(t1.name(), t2.name()));
            }
        }


        //NETLIB_UPDATEI() { }
        protected override void update() { }

        //NETLIB_UPDATE_TERMINALSI() { }
        public override void update_terminals() { }
    }


    class analog_output_t : analog_t
    {
        analog_net_t m_my_net;


        // ----------------------------------------------------------------------------------------
        // analog_output_t
        // ----------------------------------------------------------------------------------------
        public analog_output_t(core_device_t dev, string aname)
            : base(dev, aname, state_e.STATE_OUT)
        {
            m_my_net = new analog_net_t(dev.state(), name() + ".net", this);


            state().register_net(m_my_net);  //state().register_net(poolptr<analog_net_t>(&m_my_net, false));
            set_net(m_my_net);

            //net().m_cur_Analog = NL_FCONST(0.0);
            state().setup().register_term(this);
        }


        public void push(nl_fptype val) { set_Q(val); }
        public void initial(nl_fptype val) { net().set_Q_Analog(val); }

        void set_Q(nl_fptype newQ)
        {
            if (newQ != m_my_net.Q_Analog())
            {
                m_my_net.set_Q_Analog(newQ);
                m_my_net.toggle_and_push_to_queue(netlist_time.quantum());
            }
        }
    }


    // -----------------------------------------------------------------------------
    // param_t
    // -----------------------------------------------------------------------------
    public class param_t : detail.device_object_t
    {
        enum param_type_t
        {
            STRING,
            DOUBLE,
            INTEGER,
            LOGIC,
            POINTER // Special-case which is always initialized at MAME startup time
        }


        public param_t(device_t device, string name)
            : base(device, device.name() + "." + name)
        {
            device.state().setup().register_param_t(this.name(), this);
        }

        //COPYASSIGNMOVE(param_t, delete)

        //virtual ~param_t() noexcept = default; // not intended to be destroyed


        //param_type_t param_type() const;


        void update_param()
        {
            device().update_param();
        }


        protected string get_initial(device_t dev, out bool found)
        {
            string res = dev.state().setup().get_initial_param_val(this.name(), "");
            found = res != "";
            return res;
        }


        //template<typename C>
        //void set(C &p, const C v)
        protected void set<C>(ref C p, C v) where C : IComparable
        {
            if (p.CompareTo(v) != 0)  //if (p != v)
            {
                p = v;
                update_param();
            }
        }
    }


    // -----------------------------------------------------------------------------
    // numeric parameter template
    // -----------------------------------------------------------------------------

    //template <typename T>
    public class param_num_t_bool : param_t  //param_num_t<T>::param_num_t(device_t &device, const pstring &name, const T val)
    {
        bool m_param;

        public param_num_t_bool(device_t device, string name, bool val)
            : base(device, name)
        {
            //m_param = device.setup().get_initial_param_val(this->name(),val);
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                throw new emu_unimplemented();
#if false
                plib::pfunction<nl_fptype> func;
                func.compile_infix(p, {});
                auto valx = func.evaluate();
                if (std::is_integral<T>::value)
                    if (plib::abs(valx - plib::trunc(valx)) > nlconst::magic(1e-6))
                        throw nl_exception(MF_INVALID_NUMBER_CONVERSION_1_2(device.name() + "." + name, p));
                m_param = static_cast<T>(valx);
#endif
            }
            else
            {
                m_param = val;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }

        public bool op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }
        public void setTo(bool param) { set(ref m_param, param); }
    }

    public class param_num_t_int : param_t  //param_num_t<T>::param_num_t(device_t &device, const pstring &name, const T val)
    {
        int m_param;

        public param_num_t_int(device_t device, string name, int val)
            : base(device, name)
        {
            //m_param = device.setup().get_initial_param_val(this->name(),val);
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                throw new emu_unimplemented();
#if false
                plib::pfunction<nl_fptype> func;
                func.compile_infix(p, {});
                auto valx = func.evaluate();
                if (std::is_integral<T>::value)
                    if (plib::abs(valx - plib::trunc(valx)) > nlconst::magic(1e-6))
                        throw nl_exception(MF_INVALID_NUMBER_CONVERSION_1_2(device.name() + "." + name, p));
                m_param = static_cast<T>(valx);
#endif
            }
            else
            {
                m_param = val;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }

        public int op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }
        public void setTo(int param) { set(ref m_param, param); }
    }

    public class param_num_t_unsigned : param_t  //param_num_t<T>::param_num_t(device_t &device, const pstring &name, const T val)
    {
        UInt32 m_param;

        public param_num_t_unsigned(device_t device, string name, UInt32 val)
            : base(device, name)
        {
            //m_param = device.setup().get_initial_param_val(this->name(),val);
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                throw new emu_unimplemented();
#if false
                plib::pfunction<nl_fptype> func;
                func.compile_infix(p, {});
                auto valx = func.evaluate();
                if (std::is_integral<T>::value)
                    if (plib::abs(valx - plib::trunc(valx)) > nlconst::magic(1e-6))
                        throw nl_exception(MF_INVALID_NUMBER_CONVERSION_1_2(device.name() + "." + name, p));
                m_param = static_cast<T>(valx);
#endif
            }
            else
            {
                m_param = val;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }

        public UInt32 op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }
        public void setTo(UInt32 param) { set(ref m_param, param); }
    }

    public class param_num_t_size_t : param_t  //param_num_t<T>::param_num_t(device_t &device, const pstring &name, const T val)
    {
        UInt32 m_param;

        public param_num_t_size_t(device_t device, string name, UInt32 val)
            : base(device, name)
        {
            //m_param = device.setup().get_initial_param_val(this->name(),val);
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                throw new emu_unimplemented();
#if false
                plib::pfunction<nl_fptype> func;
                func.compile_infix(p, {});
                auto valx = func.evaluate();
                if (std::is_integral<T>::value)
                    if (plib::abs(valx - plib::trunc(valx)) > nlconst::magic(1e-6))
                        throw nl_exception(MF_INVALID_NUMBER_CONVERSION_1_2(device.name() + "." + name, p));
                m_param = static_cast<T>(valx);
#endif
            }
            else
            {
                m_param = val;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }

        public UInt32 op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }
        public void setTo(UInt32 param) { set(ref m_param, param); }
    }

    public class param_num_t_nl_fptype : param_t  //param_num_t<T>::param_num_t(device_t &device, const pstring &name, const T val)
    {
        nl_fptype m_param;

        public param_num_t_nl_fptype(device_t device, string name, nl_fptype val)
            : base(device, name)
        {
            //m_param = device.setup().get_initial_param_val(this->name(),val);
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                throw new emu_unimplemented();
#if false
                plib::pfunction<nl_fptype> func;
                func.compile_infix(p, {});
                auto valx = func.evaluate();
                if (std::is_integral<T>::value)
                    if (plib::abs(valx - plib::trunc(valx)) > nlconst::magic(1e-6))
                        throw nl_exception(MF_INVALID_NUMBER_CONVERSION_1_2(device.name() + "." + name, p));
                m_param = static_cast<T>(valx);
#endif
            }
            else
            {
                m_param = val;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }

        public nl_fptype op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }
        public void setTo(nl_fptype param) { set(ref m_param, param); }
    }


#if false
    template <typename T>
    class param_enum_t final: public param_t
    {
    public:
        param_enum_t(device_t &device, const pstring &name, const T val);

        T operator()() const NL_NOEXCEPT { return T(m_param); }
        operator T() const NL_NOEXCEPT { return T(m_param); }
        void setTo(const T &param) noexcept { set(m_param, static_cast<int>(param)); }
    private:
        int m_param;
    };
#endif


    //template <typename T>
    public class param_enum_t_matrix_type_e : param_t
    {
        solver.matrix_type_e m_param;  //int m_param;


        public param_enum_t_matrix_type_e(device_t device, string name, solver.matrix_type_e val)
            : base(device, name)
        {
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                solver.matrix_type_e temp = val;
                bool ok = plib.penum_base.set_from_string(p, out temp);  //bool ok = temp.set_from_string(p);
                if (!ok)
                {
                    device.state().log().fatal.op(nl_errstr_global.MF_INVALID_ENUM_CONVERSION_1_2(name, p));
                    throw new nl_exception(nl_errstr_global.MF_INVALID_ENUM_CONVERSION_1_2(name, p));
                }
                m_param = temp;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }


        public solver.matrix_type_e op() { return m_param; }  //T operator()() const NL_NOEXCEPT { return T(m_param); }
        //operator T() const NL_NOEXCEPT { return T(m_param); }
        //void setTo(const T &param) noexcept { set(m_param, static_cast<int>(param)); }
    }


    //template <typename T>
    public class param_enum_t_matrix_sort_type_e : param_t
    {
        solver.matrix_sort_type_e m_param;  //int m_param;


        public param_enum_t_matrix_sort_type_e(device_t device, string name, solver.matrix_sort_type_e val)
            : base(device, name)
        {
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                solver.matrix_sort_type_e temp = val;
                bool ok = plib.penum_base.set_from_string(p, out temp);  //bool ok = temp.set_from_string(p);
                if (!ok)
                {
                    device.state().log().fatal.op(nl_errstr_global.MF_INVALID_ENUM_CONVERSION_1_2(name, p));
                    throw new nl_exception(nl_errstr_global.MF_INVALID_ENUM_CONVERSION_1_2(name, p));
                }
                m_param = temp;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }


        public solver.matrix_sort_type_e op() { return m_param; }  //T operator()() const NL_NOEXCEPT { return T(m_param); }
        //operator T() const NL_NOEXCEPT { return T(m_param); }
        //void setTo(const T &param) noexcept { set(m_param, static_cast<int>(param)); }
    }


    //template <typename T>
    public class param_enum_t_matrix_fp_type_e : param_t
    {
        solver.matrix_fp_type_e m_param;  //int m_param;


        public param_enum_t_matrix_fp_type_e(device_t device, string name, solver.matrix_fp_type_e val)
            : base(device, name)
        {
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                solver.matrix_fp_type_e temp = val;
                bool ok = plib.penum_base.set_from_string(p, out temp);  //bool ok = temp.set_from_string(p);
                if (!ok)
                {
                    device.state().log().fatal.op(nl_errstr_global.MF_INVALID_ENUM_CONVERSION_1_2(name, p));
                    throw new nl_exception(nl_errstr_global.MF_INVALID_ENUM_CONVERSION_1_2(name, p));
                }
                m_param = temp;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }


        public solver.matrix_fp_type_e op() { return m_param; }  //T operator()() const NL_NOEXCEPT { return T(m_param); }
        //operator T() const NL_NOEXCEPT { return T(m_param); }
        //void setTo(const T &param) noexcept { set(m_param, static_cast<int>(param)); }
    }


    // FIXME: these should go as well
    //using param_logic_t = param_num_t<bool>;
    //using param_int_t = param_num_t<int>;
    //using param_fp_t = param_num_t<nl_fptype>;
    public class param_logic_t : param_num_t_bool { public param_logic_t(device_t device, string name, bool val) : base(device, name, val) { } }
    public class param_int_t : param_num_t_int { public param_int_t(device_t device, string name, int val) : base(device, name, val) { } }
    public class param_fp_t : param_num_t_nl_fptype { public param_fp_t(device_t device, string name, double val) : base(device, name, val) { } }


    // -----------------------------------------------------------------------------
    // pointer parameter
    // -----------------------------------------------------------------------------
    //class param_ptr_t final: public param_t


    // -----------------------------------------------------------------------------
    // string parameter
    // -----------------------------------------------------------------------------
    class param_str_t : param_t
    {
        string m_param;


        public param_str_t(device_t device, string name, string val)
            : base(device, name)
        {
            m_param = device.state().setup().get_initial_param_val(this.name(), val);
        }


        //const pstring &operator()() const NL_NOEXCEPT { return Value(); }
        public string op() { return str(); }


        //void setTo(const pstring &param) NL_NOEXCEPT


        protected virtual void changed() { }


        protected string str() { return m_param; }
    }


    // -----------------------------------------------------------------------------
    // model parameter
    // -----------------------------------------------------------------------------
    class param_model_t : param_str_t
    {
        //template <typename T>
        class value_base_t_nl_fptype
        {
            nl_fptype m_value;  //const T m_value;

            public value_base_t_nl_fptype(param_model_t param, string name)
            {
                m_value = param.value(name);
            }

            //T operator()() const noexcept { return m_value; }
            //operator T() const noexcept { return m_value; }
        }

        //using value_t = value_base_t<nl_fptype>;
        class value_t : value_base_t_nl_fptype { public value_t(param_model_t param, string name) : base(param, name) { } }


        public param_model_t(device_t device, string name, string val) : base(device, name, val) { }


        //pstring value_str(const pstring &entity);


        nl_fptype value(string entity)
        {
            return state().setup().models().value(str(), entity);
        }


        //pstring type();


        // hide this
        //void setTo(const pstring &param) = delete;


        protected override void changed() { }
    }


    namespace detail
    {
        // -----------------------------------------------------------------------------
        // queue_t
        // -----------------------------------------------------------------------------

        // We don't need a thread-safe queue currently. Parallel processing of
        // solvers will update inputs after parallel processing.
        public class queue_t : timed_queue,
                               //public timed_queue<pqentry_t<net_t *, netlist_time>, false, NL_KEEP_STATISTICS>,
                               //public timed_queue<plib::pqentry_t<net_t *, netlist_time_ext>, false>,
                               //public detail::netlist_ref,
                               plib.state_manager_t.callback_t
        {
            //using entry_t = plib::pqentry_t<net_t *, netlist_time_ext>;
            //using base_queue = timed_queue<entry_t, false>;


            netlist_ref m_netlist_ref;


            UInt32 m_qsize;  //std::size_t m_qsize;
            std.vector<UInt64> m_times;  //std::vector<netlist_time_ext::internal_type> m_times;
            std.vector<UInt32> m_net_ids;  //std::vector<std::size_t> m_net_ids;


            public queue_t(netlist_t nl)
                : base(false, true, 512)  //: timed_queue<plib::pqentry_t<net_t *, netlist_time>, false>(512)
                //, netlist_ref(nl)
            {
                m_netlist_ref = new netlist_ref(nl);


                m_qsize = 0;
                m_times = new std.vector<UInt64>(512);
                m_net_ids = new std.vector<UInt32>(512);
            }


            public void register_state(plib.state_manager_t manager, string module) { throw new emu_unimplemented(); }
            public void on_pre_save(plib.state_manager_t manager) { throw new emu_unimplemented(); }
            public void on_post_load(plib.state_manager_t manager) { throw new emu_unimplemented(); }
        }
    }


    // -----------------------------------------------------------------------------
    // netlist_state__t
    // -----------------------------------------------------------------------------
    public class netlist_state_t
    {
        //friend class netlist_t; // allow access to private members

        //using nets_collection_type = std::vector<owned_pool_ptr<detail::net_t>>;

        // need to preserve order of device creation ...
        //using devices_collection_type = std::vector<std::pair<pstring, owned_pool_ptr<core_device_t>>>;


        // sole use is to manage lifetime of family objects
        std.unordered_map<string, logic_family_desc_t> m_family_cache = new std.unordered_map<string, logic_family_desc_t>();  //std::unordered_map<pstring, plib::unique_ptr<logic_family_desc_t>> m_family_cache;

        nlmempool m_pool; // must be deleted last!

        string m_name;
        netlist_t m_netlist;
        plib.dynlib m_lib;   //plib::unique_ptr<plib::dynlib>      m_lib; // external lib needs to be loaded as long as netlist exists
        plib.state_manager_t m_state;
        callbacks_t m_callbacks;  //plib::unique_ptr<callbacks_t>       m_callbacks;
        log_type m_log;
        setup_t m_setup;  //plib::unique_ptr<setup_t>           m_setup;

        nets_collection_type m_nets = new nets_collection_type();
        // sole use is to manage lifetime of net objects
        devices_collection_type m_devices = new devices_collection_type();
        bool m_extended_validation;

        // dummy version
        int                                 m_dummy_version;


        public netlist_state_t(string aname,
            callbacks_t callbacks)  //plib::unique_ptr<callbacks_t> &&callbacks,
        {
            m_name = aname;
            m_callbacks = callbacks; // Order is important here
            m_log = new log_type(nl_config_global.NL_DEBUG, m_callbacks);
            m_extended_validation = false;
            m_dummy_version = 1;


            string libpath = "";  //pstring libpath = plib::util::environment("NL_BOOSTLIB", plib::util::buildpath({".", "nlboost.so"}));
            m_lib = new plib.dynlib(libpath);  //m_lib = plib::make_unique<plib::dynlib>(libpath);

            m_setup = new setup_t(this);  //m_setup = plib::make_unique<setup_t>(*this);
            // create the run interface
            m_netlist = new netlist_t(this);  //m_netlist = m_pool.make_unique<netlist_t>(*this);

            // Make sure save states are invalidated when a new version is deployed

            m_state.save_item(this, m_dummy_version, "V" + version());

            // Initialize factory
            netlist.devices.net_lib_global.initialize_factory(m_setup.factory());
            nlm_base_global.netlist_base(m_setup);  //NETLIST_NAME(base)(*m_setup);
        }


        //COPYASSIGNMOVE(netlist_state_t, delete)
        /// \brief Destructor
        ///
        /// The destructor is virtual to allow implementation specific devices
        /// to connect to the outside world. For examples see MAME netlist.cpp.
        ///
        //virtual ~netlist_state_t() noexcept = default;


        public std.unordered_map<string, logic_family_desc_t> family_cache { get { return m_family_cache; } }


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
                if (cc(d.second()))
                {
                    if (ret != null)
                    {
                        m_log.fatal.op(nl_errstr_global.MF_MORE_THAN_ONE_1_DEVICE_FOUND(classname));
                        throw new nl_exception(nl_errstr_global.MF_MORE_THAN_ONE_1_DEVICE_FOUND(classname));
                    }

                    ret = d.second();
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
                var dev = d is C ? (C)d.second() : null;  //auto dev = dynamic_cast<C *>(d.second.get());
                if (dev != null)
                    tmp.push_back(dev);
            }
            return tmp;
        }


        // logging and name

        //const pstring &name() const noexcept { return m_name; }

        public log_type log() { return m_log; }

        public plib.dynlib lib() { return m_lib; }

        public netlist_t exec() { return m_netlist; }


        // state handling

        public plib.state_manager_t run_state_manager() { return m_state; }


        //template<typename O, typename C>
        public void save(detail.netlist_name_interface owner, object state, string module, string stname)  //void save(O &owner, C &state, const pstring &module, const pstring &stname)
        {
            this.run_state_manager().save_item(owner, state, module + "." + stname);  //this->run_state_manager().save_item(static_cast<void *>(&owner), state, module + pstring(".") + stname);
        }
        //template<typename O, typename C>
        public void save(detail.netlist_name_interface owner, object state, string module, string stname, UInt32 count)  //void save(O &owner, C *state, const pstring &module, const pstring &stname, const std::size_t count)
        {
            this.run_state_manager().save_state_ptr(owner, module + "." + stname, null, count, state);  //this->run_state_manager().save_state_ptr(static_cast<void *>(&owner), module + pstring(".") + stname, plib::state_manager_t::dtype<C>(), count, state);
        }


        //detail::net_t *find_net(const pstring &name) const;
        //std::size_t find_net_id(const detail::net_t *net) const;


        //template <typename T>
        public void register_net(detail.net_t net) { m_nets.push_back(net); }  //void register_net(owned_pool_ptr<T> &&net) { m_nets.push_back(std::move(net)); }


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
                if (d.first() == name)
                    return d.second();
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
        public void register_device(string name, core_device_t dev)  //void register_device(const pstring &name, owned_pool_ptr<T> &&dev)
        {
            foreach (var d in m_devices)
            {
                if (d.first() == name)
                {
                    //dev.release();
                    log().fatal.op(nl_errstr_global.MF_DUPLICATE_NAME_DEVICE_LIST(name));
                    throw new nl_exception(nl_errstr_global.MF_DUPLICATE_NAME_DEVICE_LIST(name));
                }
            }

            //m_devices.push_back(dev);
            m_devices.Add(new KeyValuePair<string, core_device_t>(name, dev));  //m_devices.insert(m_devices.end(), { name, std::move(dev) });
        }


#if false
        /// \brief Register device using unique_ptr
        ///
        /// Used to register devices.
        ///
        /// \param name Name of the device
        /// \param dev Device to be registered
        template <typename T>
        void register_device(const pstring &name, unique_pool_ptr<T> &&dev)
        {
            register_device(name, owned_pool_ptr<T>(dev.release(), true, dev.get_deleter()));
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
                if (it.second() == dev)
                {
                    m_state.remove_save_items(dev);
                    m_devices.erase(i);
                    return;
                }
            }
        }


        public setup_t setup() { return m_setup; }


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


        //template<typename T, typename... Args>
        //unique_pool_ptr<T> make_object(Args&&... args)
        //{
        //    return m_pool.make_unique<T>(std::forward<Args>(args)...);
        //}
        // memory pool - still needed in some places
        public nlmempool pool() { return m_pool; }


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


        //void reset();
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


                m_inc = netlist_time.from_fp(plib.pmath_global.reciprocal(m_freq.op() * nlconst.two()));
            }


            //NETLIB_RESETI();
            public override void reset()
            {
                m_Q.net().set_next_scheduled_time(netlist_time_ext.zero());
            }


            //NETLIB_UPDATE_PARAMI();
            public override void update_param()
            {
                m_inc = netlist_time.from_fp(plib.pmath_global.reciprocal(m_freq.op() * nlconst.two()));
            }

            //NETLIB_UPDATEI();
            protected override void update()
            {
                // only called during start up.
                // mainclock will step forced by main loop
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
        //PALIGNAS_CACHELINE()
        netlist_time_ext m_time;
        devices.nld_mainclock m_mainclock;  //devices::NETLIB_NAME(mainclock) *   m_mainclock;

        //PALIGNAS_CACHELINE()
        detail.queue_t m_queue;
        bool m_use_stats;

        // performance
        //plib::pperftime_t<true>             m_stat_mainloop;
        //plib::pperfcount_t<true>            m_perf_out_processed;


        public netlist_t(netlist_state_t state)
        {
            m_state = state;
            m_solver = null;
            m_time = netlist_time_ext.zero();
            m_mainclock = null;
            m_queue = new detail.queue_t(this);
            m_use_stats = false;


            state.run_state_manager().save_item(this, (plib.state_manager_t.callback_t)m_queue, "m_queue");
            state.run_state_manager().save_item(this, m_time, "m_time");
        }

        //COPYASSIGNMOVE(netlist_t, delete)

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

            qpush(new entry_t(stop, null));

            if (m_mainclock == null)
            {
                m_time = m_queue.top().exec_time();
                detail.net_t obj = m_queue.top().object_();
                m_queue.pop();

                while (obj != null)
                {
                    throw new emu_unimplemented();
#if false
#endif
                }
            }
            else
            {
                logic_net_t mc_net = m_mainclock.m_Q.net();
                netlist_time inc = m_mainclock.m_inc;
                netlist_time_ext mc_time = mc_net.next_scheduled_time();

                do
                {
                    entry_t top = m_queue.top();
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

        public detail.queue_t queue() { return m_queue; }


        //template <typename E>
        public void qpush(entry_t e)  //void qpush(E && e) noexcept
        {
            if (!nl_config_global.NL_USE_QUEUE_STATS || !m_use_stats)
                m_queue.push(false, e);  //m_queue.push<false>(std::forward<E>(e)); // NOLINT(performance-move-const-arg)
            else
                m_queue.push(true, e);  //m_queue.push<true>(std::forward<E>(e)); // NOLINT(performance-move-const-arg)
        }


        //template <class R>
        public void qremove<R>(R elem)
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

            m_time = netlist_time_ext.zero();
            m_queue.clear();
            if (m_mainclock != null)
                m_mainclock.m_Q.net().set_next_scheduled_time(netlist_time_ext.zero());
            //if (m_solver != nullptr)
            //  m_solver->reset();

            //m_state.reset();
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


        public bool stats_enabled() { return m_use_stats; }
        //void enable_stats(bool val) { m_use_stats = val; }


        //template <bool KEEP_STATS>
        //void process_queue_stats(netlist_time delta) noexcept;
    }


    // ----------------------------------------------------------------------------------------
    // logic_family_ttl_t
    // ----------------------------------------------------------------------------------------
    class logic_family_ttl_t : logic_family_desc_t
    {
        logic_family_ttl_t() : base()
        {
            m_fixed_V = nlconst.magic(5.0);
            m_low_thresh_PCNT = nlconst.magic(0.8 / 5.0);
            m_high_thresh_PCNT = nlconst.magic(2.0 / 5.0);
            // m_low_V  - these depend on sinked/sourced current. Values should be suitable for typical applications.
            m_low_VO = nlconst.magic(0.1);
            m_high_VO = nlconst.magic(1.0); // 4.0
            m_R_low = nlconst.magic(1.0);
            m_R_high = nlconst.magic(130.0);
        }


        public override devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_state_t anetlist, string name, logic_output_t proxied)  //unique_pool_ptr<devices::nld_base_d_to_a_proxy> create_d_a_proxy(netlist_state_t &anetlist, const pstring &name, logic_output_t *proxied) const override;
        {
            return new devices.nld_d_to_a_proxy(anetlist, name, proxied);  //return anetlist.make_object<devices::nld_d_to_a_proxy>(anetlist, name, proxied);
        }
        public override devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_state_t anetlist, string name, logic_input_t proxied)  //unique_pool_ptr<devices::nld_base_a_to_d_proxy> create_a_d_proxy(netlist_state_t &anetlist, const pstring &name, logic_input_t *proxied) const override;
        {
            return new devices.nld_a_to_d_proxy(anetlist, name, proxied);  //return anetlist.make_object<devices::nld_a_to_d_proxy>(anetlist, name, proxied);
        }
    }


    class logic_family_cd4xxx_t : logic_family_desc_t
    {
        logic_family_cd4xxx_t() : base()
        {
            m_fixed_V = nlconst.magic(0.0);
            m_low_thresh_PCNT = nlconst.magic(1.5 / 5.0);
            m_high_thresh_PCNT = nlconst.magic(3.5 / 5.0);
            // m_low_V  - these depend on sinked/sourced current. Values should be suitable for typical applications.
            m_low_VO = nlconst.magic(0.05);
            m_high_VO = nlconst.magic(0.05); // 4.95
            m_R_low = nlconst.magic(10.0);
            m_R_high = nlconst.magic(10.0);
        }


        public override devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_state_t anetlist, string name, logic_output_t proxied)  //unique_pool_ptr<devices::nld_base_d_to_a_proxy> create_d_a_proxy(netlist_state_t &anetlist, const pstring &name, logic_output_t *proxied) const override;
        {
            return new devices.nld_d_to_a_proxy(anetlist, name, proxied);  //return anetlist.make_object<devices::nld_d_to_a_proxy>(anetlist, name, proxied);
        }
        public override devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_state_t anetlist, string name, logic_input_t proxied)  //unique_pool_ptr<devices::nld_base_a_to_d_proxy> create_a_d_proxy(netlist_state_t &anetlist, const pstring &name, logic_input_t *proxied) const override;
        {
            return new devices.nld_a_to_d_proxy(anetlist, name, proxied);  //return anetlist.make_object<devices::nld_a_to_d_proxy>(anetlist, name, proxied);
        }
    }
}
