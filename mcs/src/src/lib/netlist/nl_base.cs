// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using devices_collection_type = mame.std.vector<mame.std.pair<string, mame.netlist.core_device_t>>;  //using devices_collection_type = std::vector<std::pair<pstring, poolptr<core_device_t>>>;
using log_type = mame.plib.plog_base<mame.netlist.callbacks_t>;//, NL_DEBUG>;
using netlist_sig_t = System.UInt32;
using netlist_state_family_collection_type = mame.std.unordered_map<string, mame.netlist.logic_family_desc_t>;  //using family_collection_type = std::unordered_map<pstring, host_arena::unique_ptr<logic_family_desc_t>>;
using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;
using netlist_time_ext = mame.plib.ptime_i64;  //netlist_time
using nets_collection_type = mame.std.vector<mame.netlist.detail.net_t>;  //using nets_collection_type = std::vector<poolptr<detail::net_t>>;
using nl_fptype = System.Double;
using queue_t_entry_t = mame.plib.pqentry_t<mame.plib.ptime_i64, mame.netlist.detail.net_t>;  //using entry_t = plib::pqentry_t<netlist_time_ext, net_t *>;
using props = mame.netlist.detail.property_store_t<mame.netlist.detail.object_t, string>;
using size_t = System.UInt32;
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

    //#define NETLIB_DELEGATE(name) nldelegate(&this_type :: name, this)

    //#define NETLIB_UPDATE_TERMINALSI() virtual void update_terminals() override
    //#define NETLIB_HANDLERI(name) void name() noexcept
    //#define NETLIB_UPDATEI() virtual void update() NL_NOEXCEPT override
    //#define NETLIB_UPDATE_PARAMI() virtual void update_param() override
    //#define NETLIB_RESETI() virtual void reset() override

    //#define NETLIB_SUB(chip) nld_ ## chip
    //#define NETLIB_SUB_UPTR(ns, chip) device_arena::unique_ptr< ns :: nld_ ## chip >

    //#define NETLIB_HANDLER(chip, name) void NETLIB_NAME(chip) :: name() NL_NOEXCEPT
    //#define NETLIB_UPDATE(chip) NETLIB_HANDLER(chip, update)

    //#define NETLIB_RESET(chip) void NETLIB_NAME(chip) :: reset(void)

    //#define NETLIB_UPDATE_PARAM(chip) void NETLIB_NAME(chip) :: update_param()

    //#define NETLIB_UPDATE_TERMINALS(chip) void NETLIB_NAME(chip) :: update_terminals()


    //template <class CX>
    //class delegator_t : public CX


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

        //PCOPYASSIGNMOVE(logic_family_desc_t, delete)


        public abstract devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_state_t anetlist, string name, logic_output_t proxied);  //virtual device_arena::unique_ptr<devices::nld_base_d_to_a_proxy> create_d_a_proxy(netlist_state_t &anetlist, const pstring &name, const logic_output_t *proxied) const = 0;
        public abstract devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_state_t anetlist, string name, logic_input_t proxied);  //virtual device_arena::unique_ptr<devices::nld_base_a_to_d_proxy> create_a_d_proxy(netlist_state_t &anetlist, const pstring &name, const logic_input_t *proxied) const = 0;


        // FIXME: remove fixed_V()
        //nl_fptype fixed_V() const noexcept{return m_fixed_V; }
        //nl_fptype low_thresh_V(nl_fptype VN, nl_fptype VP) const noexcept{ return VN + (VP - VN) * m_low_thresh_PCNT; }
        //nl_fptype high_thresh_V(nl_fptype VN, nl_fptype VP) const noexcept{ return VN + (VP - VN) * m_high_thresh_PCNT; }
        public nl_fptype low_offset_V() { return m_low_VO; }
        public nl_fptype high_offset_V() { return m_high_VO; }
        public nl_fptype R_low() { return m_R_low; }
        public nl_fptype R_high() { return m_R_high; }

        //bool is_above_high_thresh_V(nl_fptype V, nl_fptype VN, nl_fptype VP) const noexcept
        //{ return V > high_thresh_V(VN, VP); }

        //bool is_below_low_thresh_V(nl_fptype V, nl_fptype VN, nl_fptype VP) const noexcept
        //{ return V < low_thresh_V(VN, VP); }
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
        //logic_family_t(const logic_family_desc_t *d) : m_logic_family(d) {}
        //PCOPYASSIGNMOVE(logic_family_t, delete)

        //const logic_family_desc_t *logic_family() const { return m_logic_family; }
        //void set_logic_family(const logic_family_desc_t *fam) { m_logic_family = fam; }
        logic_family_desc_t logic_family();
        void set_logic_family(logic_family_desc_t fam);
    }


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
        //using value_type = T;


        T m_value;


        //template <typename O>
        //! Constructor.
        public state_var(detail.netlist_interface_plus_name owner,             //!< owner must have a netlist() method.  //O &owner,             //!< owner must have a netlist() method.
            string name,     //!< identifier/name for this state variable
            T value)         //!< Initial value after construction
        {
            m_value = value;


            owner.state().save(owner, m_value, owner.name(), name);
        }


        //template <typename O>
        //! Constructor.
        state_var(detail.netlist_interface_plus_name owner,             //!< owner must have a netlist() method.  //O &owner,             //!< owner must have a netlist() method.
            string name)     //!< identifier/name for this state variable
        {
            owner.state().save(owner, m_value, owner.name(), name);
        }


        //PMOVEASSIGN(state_var, delete)


        //! Destructor.
        //~state_var() noexcept = default;

        //! Copy Constructor removed.
        //constexpr state_var(const state_var &rhs) = default;
        //! Assignment operator to assign value of a state var.
        //constexpr state_var &operator=(const state_var &rhs) noexcept { m_value = rhs.m_value; return *this; } // OSX doesn't like noexcept
        //! Assignment operator to assign value of type T.
        //constexpr state_var &operator=(const T &rhs) noexcept { m_value = rhs; return *this; }
        //! Assignment move operator to assign value of type T.
        //constexpr state_var &operator=(T &&rhs) noexcept { std::swap(m_value, rhs); return *this; }
        //! Return non-const value of state variable.
        //constexpr operator T & () noexcept { return m_value; }
        //! Return const value of state variable.
        //constexpr operator const T & () const noexcept { return m_value; }
        //! Return non-const value of state variable.
        //constexpr T & var() noexcept { return m_value; }
        //! Return const value of state variable.
        //constexpr const T & var() const noexcept { return m_value; }
        //! Return non-const value of state variable.
        //constexpr T & operator ()() noexcept { return m_value; }
        //! Return const value of state variable.
        //constexpr const T & operator ()() const noexcept { return m_value; }
        //! Access state variable by ->.
        //constexpr T * operator->() noexcept { return &m_value; }
        //! Access state variable by const ->.
        //constexpr const T * operator->() const noexcept{ return &m_value; }

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
            //using value_type = T;
            //using key_type = const C *;


            public static void add(C obj, T aname)
            {
                store().insert(obj, aname);
            }


            public static T get(C obj)
            {
                try
                {
                    var ret = store().find(obj);
                    if (ret == null)
                        return default;
                    return ret;
                }
                catch (Exception)
                {
                    plib.pglobal.terminate("exception in property_store_t.get()");
                    return default;  //return *static_cast<T *>(nullptr);
                }
            }


            public static void remove(C obj)
            {
                store().erase(obj);  //store().find(obj));
            }


            static std.unordered_map<C, T> lstore = new std.unordered_map<C, T>();

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
        ///  objects.
        public class object_t : global_object, netlist_name_interface
        {
            //using props = property_store_t<object_t, pstring>;


            /// \brief Constructor.
            /// Every class derived from the object_t class must have a name.
            ///
            /// \param aname string containing name of the object
            public object_t(string aname /*!< string containing name of the object */)
            {
                hash = hashStart++;


                props.add(this, aname);
            }

            //PCOPYASSIGNMOVE(object_t, delete)

            // only childs should be destructible
            ~object_t()
            {
                props.remove(this);
            }


            // for use in unordered_map, see nl_setup.m_connected_terminals
            static int hashStart = 0;
            int hash;
            public override int GetHashCode() { return hash; }
            public override bool Equals(object obj) { return hash.Equals(((object_t)obj).hash); }


            /// \brief return name of the object
            ///
            /// \returns name of the object.
            public string name() { return props.get(this); }
        }


        /// \brief Base class for all objects bejng owned by a netlist
        ///
        /// The object provides adds \ref netlist_state_t and \ref netlist_t
        /// accessors.
        ///
        public class netlist_object_t : object_t, netlist_interface_plus_name
        {
            netlist_t m_netlist;


            public netlist_object_t(netlist_t nl, string name) : base(name) { m_netlist = nl; }

            //~netlist_object_t() = default;

            //PCOPYASSIGNMOVE(netlist_object_t, delete)

            public netlist_state_t state() { return m_netlist.nlstate(); }

            public netlist_t exec() { return m_netlist; }


            // to ease template design
            //template<typename T, typename... Args>
            //device_arena::unique_ptr<T> make_pool_object(Args&&... args);
        }


        // -----------------------------------------------------------------------------
        // device_object_t
        // -----------------------------------------------------------------------------

        /// \brief Base class for all objects being owned by a device.
        ///
        /// Serves as the base class of all objects being owned by a device.
        ///
        /// The class also supports device-less objects. In this case,
        /// nullptr is passed in as the device object.
        ///
        public class device_object_t : detail.object_t, netlist_interface_plus_name
        {
            core_device_t m_device;


            /// \brief Constructor.
            ///
            /// \param dev  pointer to device owning the object.
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
            /// Mimimum value is 2 here to support tristate output on proxies.
            const int INP_BITS = 2;

            const int INP_MASK = (1 << INP_BITS) - 1;
            public const int INP_HL_SHIFT = 0;
            public const int INP_LH_SHIFT = INP_BITS;


            public static netlist_sig_t OUT_TRISTATE() { return INP_MASK; }  //static constexpr netlist_sig_t OUT_TRISTATE() { return INP_MASK; }


            //static_assert(INP_BITS * 2 <= sizeof(netlist_sig_t) * 8, "netlist_sig_t size not sufficient");


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

            //PCOPYASSIGNMOVE(core_terminal_t, delete)


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
            public bool is_tristate_output() { return this is tristate_output_t; }  //return dynamic_cast<const tristate_output_t *>(this) != nullptr;
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
    //using nldelegate_ts = plib::pmfp<void, nl_fptype>;
    public delegate void nldelegate_ts(nl_fptype param);
    //using nldelegate_dyn = plib::pmfp<void>;
    public delegate void nldelegate_dyn();


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
        public terminal_t(core_device_t dev, string aname)//, terminal_t otherterm)
            : base(dev, aname, state_e.STATE_BIDIR)
        {
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


        /// @brief Solve the system this terminal is connected to.
        ///
        /// \note deprecated - will be removed
        public void solve_now()
        {
            var solv = solver();
            // Nets may belong to railnets which do not have a solver attached
            if (solv != null)
                solver().solve_now();
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
        public logic_t(device_t dev, string aname, state_e terminal_state, nldelegate delegate_ = null)  // = nldelegate());
            : base(dev, aname, terminal_state, delegate_)
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
        public logic_input_t(device_t dev, string aname, nldelegate delegate_ = null)  // = nldelegate());
            : base(dev, aname, state_e.STATE_INP_ACTIVE, delegate_ != null ? delegate_ : dev.default_delegate())
        {
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
        public nl_fptype Q_Analog() { return net().Q_Analog(); }
    }


    namespace detail
    {
        // -----------------------------------------------------------------------------
        // net_t
        // -----------------------------------------------------------------------------
        public class net_t : netlist_object_t
        {
            enum queue_status
            {
                DELAYED_DUE_TO_INACTIVE = 0,
                QUEUED,
                DELIVERED
            }


            state_var<netlist_sig_t> m_new_Q;
            state_var<netlist_sig_t> m_cur_Q;
            state_var<queue_status> m_in_queue;
            state_var<netlist_time_ext> m_next_scheduled_time;

            core_terminal_t m_railterminal;
            std.vector<core_terminal_t> m_list_active = new std.vector<core_terminal_t>();  //plib::linkedlist_t<core_terminal_t> m_list_active;
            std.vector<core_terminal_t> m_core_terms = new std.vector<core_terminal_t>(); // save post-start m_list ...


            public net_t(netlist_state_t nl, string aname, core_terminal_t railterminal = null)
                : base(nl.exec(), aname)
            {
                m_new_Q = new state_var<netlist_sig_t>(this, "m_new_Q", (netlist_sig_t)0);
                m_cur_Q = new state_var<netlist_sig_t>(this, "m_cur_Q", (netlist_sig_t)0);
                m_in_queue = new state_var<queue_status>(this, "m_in_queue", queue_status.DELIVERED);
                m_next_scheduled_time = new state_var<netlist_time_ext>(this, "m_time", netlist_time_ext.zero());
                m_railterminal = railterminal;


                props.add(this, "");
            }

            //PCOPYASSIGNMOVE(net_t, delete)

            //virtual ~net_t() noexcept = default;


            public void reset()
            {
                m_next_scheduled_time.op = netlist_time_ext.zero();
                m_in_queue.op = queue_status.DELIVERED;

                m_new_Q.op = 0;
                m_cur_Q.op = 0;

                var p = this is analog_net_t ? (analog_net_t)this : null;

                if (p != null)
                    p.set_Q_Analog(nlconst.zero());

                // rebuild m_list and reset terminals to active or analog out state

                m_list_active.clear();
                foreach (core_terminal_t ct in core_terms())
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
                if (has_connections())
                {
                    if (!!is_queued())
                        exec().qremove(this);

                    var nst = exec().time() + delay;
                    m_next_scheduled_time.op = nst;

                    if (!m_list_active.empty())
                    {
                        m_in_queue.op = queue_status.QUEUED;
                        exec().qpush(new queue_t_entry_t(nst, this));
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
                nl_config_global.nl_assert(this.is_rail_net());

                m_in_queue.op = queue_status.DELIVERED; // mark as taken ...

                if ((m_new_Q.op ^ m_cur_Q.op) != 0)
                    process(KEEP_STATS, (m_new_Q.op << core_terminal_t.INP_LH_SHIFT)
                        | (m_cur_Q.op << core_terminal_t.INP_HL_SHIFT), m_new_Q.op);
            }


            public netlist_time_ext next_scheduled_time() { return m_next_scheduled_time.op; }
            public void set_next_scheduled_time(netlist_time_ext ntime) { m_next_scheduled_time.op = ntime; }

            public bool is_rail_net() { return m_railterminal != null; }
            public core_terminal_t railterminal() { return m_railterminal; }

            public bool has_connections() { return !m_core_terms.empty(); }


            //void add_to_active_list(core_terminal_t &term) NL_NOEXCEPT;
            //void remove_from_active_list(core_terminal_t &term) NL_NOEXCEPT;


            // setup stuff

            //bool is_logic() const NL_NOEXCEPT;
            //bool detail::net_t::is_analog() const NL_NOEXCEPT { return dynamic_cast<const analog_net_t *>(this) != nullptr; }
            public bool is_analog() { return this is analog_net_t; }


            public void rebuild_list()     // rebuild m_list after a load
            {
                // rebuild m_list

                m_list_active.clear();
                foreach (var term in core_terms())
                {
                    if (term.terminal_state() != logic_t.state_e.STATE_INP_PASSIVE)
                    {
                        m_list_active.push_back(term);
                        term.set_copied_input(m_cur_Q.op);
                    }
                }
            }


            public std.vector<core_terminal_t> core_terms() { return m_core_terms; }


            public void update_inputs()
            {
#if NL_USE_COPY_INSTEAD_OF_REFERENCE
                for (auto & term : m_core_terms)
                    term->m_Q = m_cur_Q;
#endif

                // nothing needs to be done if define not set
            }


            // only used for logic nets
            public netlist_sig_t Q() { return m_cur_Q.op; }


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


            //template <bool KEEP_STATS, typename T, typename S>
            void process(bool KEEP_STATS, UInt32 mask, netlist_sig_t sig)  //void process(T mask, const S &sig) noexcept;
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
        //using detail::net_t::Q;
        //using detail::net_t::initial;
        //using detail::net_t::set_Q_and_push;
        //using detail::net_t::set_Q_time;

        public logic_net_t(netlist_state_t nl, string aname, detail.core_terminal_t railterminal = null)
            : base(nl, aname, railterminal)
        {
        }
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
            : base(dev, aname, state_e.STATE_OUT)
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
            : base(dev, aname, state_e.STATE_OUT)
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


    /// @brief Base class for all device parameters
    ///
    /// All device parameters classes derive from this object.
    public abstract class param_t : detail.device_object_t
    {
        enum param_type_t
        {
            STRING,
            DOUBLE,
            INTEGER,
            LOGIC,
            POINTER // Special-case which is always initialized at MAME startup time
        }


        //deviceless, it's the responsibility of the owner to register!
        protected param_t(string name)
            : base(null, name)
        {
        }


        public param_t(core_device_t device, string name)
            : base(device, device.name() + "." + name)
        {
            device.state().setup().register_param_t(this);
        }

        //PCOPYASSIGNMOVE(param_t, delete)

        //virtual ~param_t() noexcept = default; // not intended to be destroyed


        //param_type_t param_type() const;


        public abstract string valstr();


        void update_param()
        {
            device().update_param();
        }


        protected string get_initial(core_device_t dev, out bool found)
        {
            string res = dev.state().setup().get_initial_param_val(this.name(), "");
            found = !res.empty();
            return res;
        }


        //template<typename C>
        //void set(C &p, const C v)
        protected void set_and_update_param<C>(ref C p, C v) where C : IComparable
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

#if false
    template <typename T>
    class param_num_t final: public param_t
    {
    public:
        using value_type = T;

        param_num_t(core_device_t &device, const pstring &name, T val) noexcept(false);

        T operator()() const noexcept { return m_param; }
        operator T() const noexcept { return m_param; }

        void set(const T &param) noexcept { set_and_update_param(m_param, param); }

        pstring valstr() const override
        {
            return plib::pfmt("{}").e(gsl::narrow<nl_fptype>(m_param));
        }

    private:
        T m_param;
    };
#endif

    //template <typename T>
    public class param_num_t_bool : param_t  //param_num_t<T>::param_num_t(device_t &device, const pstring &name, const T val)
    {
        // using value_type = T;

        bool m_param;

        public param_num_t_bool(core_device_t device, string name, bool val)
            : base(device, name)
        {
            //m_param = device.setup().get_initial_param_val(this->name(),val);
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                plib.pfunction_int func = new plib.pfunction_int();
                func.compile_infix(p, new std.vector<string>());
                var valx = func.evaluate();
                if (true)  //bool is integral  //if (plib::is_integral<T>::value)
                    if (plib.pglobal.abs(valx - plib.pglobal.trunc(valx)) > nlconst.magic(1e-6))
                        throw new nl_exception(nl_errstr_global.MF_INVALID_NUMBER_CONVERSION_1_2(device.name() + "." + name, p));
                m_param = valx != 0;  //m_param = static_cast<T>(valx);
            }
            else
            {
                m_param = val;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }

        public bool op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }
        public void set(bool param) { set_and_update_param(ref m_param, param); }

        public override string valstr()
        {
            return new plib.pfmt("{0}").op(m_param);  //return plib::pfmt("{}").e(gsl::narrow<nl_fptype>(m_param));
        }
    }

    public class param_num_t_int : param_t  //param_num_t<T>::param_num_t(device_t &device, const pstring &name, const T val)
    {
        int m_param;

        public param_num_t_int(core_device_t device, string name, int val)
            : base(device, name)
        {
            //m_param = device.setup().get_initial_param_val(this->name(),val);
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                plib.pfunction_int func = new plib.pfunction_int();
                func.compile_infix(p, new std.vector<string>());
                var valx = func.evaluate();
                if (true)  //int is integral  //if (plib::is_integral<T>::value)
                    if (plib.pglobal.abs(valx - plib.pglobal.trunc(valx)) > nlconst.magic(1e-6))
                        throw new nl_exception(nl_errstr_global.MF_INVALID_NUMBER_CONVERSION_1_2(device.name() + "." + name, p));
                m_param = (int)valx;
            }
            else
            {
                m_param = val;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }

        public int op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }
        public void set(int param) { set_and_update_param(ref m_param, param); }

        public override string valstr()
        {
            return new plib.pfmt("{0}").op(m_param);  //return plib::pfmt("{}").e(gsl::narrow<nl_fptype>(m_param));
        }
    }

    public class param_num_t_unsigned : param_t  //param_num_t<T>::param_num_t(device_t &device, const pstring &name, const T val)
    {
        unsigned m_param;

        public param_num_t_unsigned(core_device_t device, string name, UInt32 val)
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
                if (plib::is_integral<T>::value)
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

        public unsigned op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }
        public void set(unsigned param) { set_and_update_param(ref m_param, param); }

        public override string valstr()
        {
            return new plib.pfmt("{0}").op(m_param);  //return plib::pfmt("{}").e(gsl::narrow<nl_fptype>(m_param));
        }
    }

    public class param_num_t_size_t : param_t  //param_num_t<T>::param_num_t(device_t &device, const pstring &name, const T val)
    {
        size_t m_param;

        public param_num_t_size_t(core_device_t device, string name, UInt32 val)
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
                if (plib::is_integral<T>::value)
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

        public size_t op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }
        public void set(size_t param) { set_and_update_param(ref m_param, param); }

        public override string valstr()
        {
            return new plib.pfmt("{0}").op(m_param);  //return plib::pfmt("{}").e(gsl::narrow<nl_fptype>(m_param));
        }
    }

    public class param_num_t_nl_fptype : param_t  //param_num_t<T>::param_num_t(device_t &device, const pstring &name, const T val)
    {
        nl_fptype m_param;

        public param_num_t_nl_fptype(core_device_t device, string name, nl_fptype val)
            : base(device, name)
        {
            //m_param = device.setup().get_initial_param_val(this->name(),val);
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                plib.pfunction_nl_fptype func = new plib.pfunction_nl_fptype();
                func.compile_infix(p, new std.vector<string>());
                var valx = func.evaluate();
                if (false)  //nl_fptype is not integral  //if (plib::is_integral<T>::value)
                    if (plib.pglobal.abs(valx - plib.pglobal.trunc(valx)) > nlconst.magic(1e-6))
                        throw new nl_exception(nl_errstr_global.MF_INVALID_NUMBER_CONVERSION_1_2(device.name() + "." + name, p));
                m_param = (nl_fptype)valx;  //m_param = static_cast<T>(valx);
            }
            else
            {
                m_param = val;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }

        public nl_fptype op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }
        public void set(nl_fptype param) { set_and_update_param(ref m_param, param); }

        public override string valstr()
        {
            return new plib.pfmt("{0}").op(m_param);  //return plib::pfmt("{}").e(gsl::narrow<nl_fptype>(m_param));
        }
    }


#if false
    template <typename T>
    class param_enum_t final: public param_t
    {
    public:
        using value_type = T;

        param_enum_t(core_device_t &device, const pstring &name, T val) noexcept(false);

        T operator()() const noexcept { return T(m_param); }
        operator T() const noexcept { return T(m_param); }
        void set(const T &param) noexcept { set_and_update_param(m_param, static_cast<int>(param)); }

        pstring valstr() const override
        {
            // returns the numerical value
            return plib::pfmt("{}")(m_param);
        }
    private:
        int m_param;
    };
#endif


    //template <typename T>
    public class param_enum_t_matrix_type_e : param_t
    {
        //using value_type = T;

        solver.matrix_type_e m_param;  //int m_param;


        public param_enum_t_matrix_type_e(core_device_t device, string name, solver.matrix_type_e val)
            : base(device, name)
        {
            m_param = val;


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
        //void set(const T &param) noexcept { set_and_update_param(m_param, static_cast<int>(param)); }

        public override string valstr()
        {
            // returns the numerical value
            return new plib.pfmt("{0}").op(m_param);  //return plib::pfmt("{}")(m_param);
        }
    }


    //template <typename T>
    public class param_enum_t_matrix_sort_type_e : param_t
    {
        solver.matrix_sort_type_e m_param;  //int m_param;


        public param_enum_t_matrix_sort_type_e(core_device_t device, string name, solver.matrix_sort_type_e val)
            : base(device, name)
        {
            m_param = val;


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
        //void set(const T &param) noexcept { set_and_update_param(m_param, static_cast<int>(param)); }

        public override string valstr()
        {
            // returns the numerical value
            return new plib.pfmt("{0}").op(m_param);  //return plib::pfmt("{}")(m_param);
        }
    }


    //template <typename T>
    public class param_enum_t_matrix_fp_type_e : param_t
    {
        solver.matrix_fp_type_e m_param;  //int m_param;


        public param_enum_t_matrix_fp_type_e(core_device_t device, string name, solver.matrix_fp_type_e val)
            : base(device, name)
        {
            m_param = val;


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
        //void set(const T &param) noexcept { set_and_update_param(m_param, static_cast<int>(param)); }

        public override string valstr()
        {
            // returns the numerical value
            return new plib.pfmt("{0}").op(m_param);  //return plib::pfmt("{}")(m_param);
        }
    }


    // FIXME: these should go as well
    //using param_logic_t = param_num_t<bool>;
    //using param_int_t = param_num_t<int>;
    //using param_fp_t = param_num_t<nl_fptype>;
    public class param_logic_t : param_num_t_bool { public param_logic_t(core_device_t device, string name, bool val) : base(device, name, val) { } }
    public class param_int_t : param_num_t_int { public param_int_t(core_device_t device, string name, int val) : base(device, name, val) { } }
    public class param_fp_t : param_num_t_nl_fptype { public param_fp_t(core_device_t device, string name, double val) : base(device, name, val) { } }


    // -----------------------------------------------------------------------------
    // pointer parameter
    // -----------------------------------------------------------------------------
    // FIXME: not a core component -> legacy
    //class param_ptr_t final: public param_t


    // -----------------------------------------------------------------------------
    // string parameter
    // -----------------------------------------------------------------------------
    class param_str_t : param_t
    {
        string m_param;


        public param_str_t(core_device_t device, string name, string val)
            : base(device, name)
        {
            m_param = val;  //m_param = plib::make_unique<pstring, host_arena>(val);
            m_param = device.state().setup().get_initial_param_val(this.name(), val);  //*m_param = device.state().setup().get_initial_param_val(this->name(),val);
        }

        public param_str_t(netlist_state_t state, string name, string val)
            : base(name)
        {
            // deviceless parameter, no registration, owner is responsible
            m_param = val;  //m_param = plib::make_unique<pstring, host_arena>(val);
            m_param = state.setup().get_initial_param_val(this.name(), val);  //*m_param = state.setup().get_initial_param_val(this->name(),val);
        }


        //const pstring &operator()() const NL_NOEXCEPT { return Value(); }
        public string op() { return str(); }


        //void set(const pstring &param)


        public override string valstr()
        {
            return m_param;
        }


        protected virtual void changed() { }


        protected string str() { return m_param; }
    }


    // -----------------------------------------------------------------------------
    // model parameter
    // -----------------------------------------------------------------------------
    class param_model_t : param_str_t
    {
        //template <typename T>
        public class value_base_t_nl_fptype
        {
            nl_fptype m_value;  //const T m_value;

            //template <typename P, typename Y=T, typename DUMMY = std::enable_if_t<plib::is_arithmetic<Y>::value>>
            public value_base_t_nl_fptype(param_model_t param, string name)  //value_base_t(P &param, const pstring &name)
            {
                m_value = param.value(name);  //: m_value(gsl::narrow<T>(param.value(name)))
            }

            //template <typename P, typename Y=T, std::enable_if_t<!plib::is_arithmetic<Y>::value, int> = 0>
            //value_base_t(P &param, const pstring &name)
            //: m_value(static_cast<T>(param.value_str(name)))
            //{
            //}

            //T operator()() const noexcept { return m_value; }
            //operator T() const noexcept { return m_value; }
            public nl_fptype op() { return m_value; }
        }


        //template <typename T>
        public class value_base_t_string
        {
            string m_value;  //const T m_value;

            //template <typename P, typename Y=T, typename DUMMY = std::enable_if_t<plib::is_arithmetic<Y>::value>>
            public value_base_t_string(param_model_t param, string name)  //value_base_t(P &param, const pstring &name)
            {
                m_value = param.value_str(name);  //: m_value(gsl::narrow<T>(param.value(name)))
            }

            //template <typename P, typename Y=T, std::enable_if_t<!plib::is_arithmetic<Y>::value, int> = 0>
            //value_base_t(P &param, const pstring &name)
            //: m_value(static_cast<T>(param.value_str(name)))
            //{
            //}

            //T operator()() const noexcept { return m_value; }
            //operator T() const noexcept { return m_value; }
            public string op() { return m_value; }
        }

        //using value_t = value_base_t<nl_fptype>;
        public class value_t : value_base_t_nl_fptype { public value_t(param_model_t param, string name) : base(param, name) { } }
        //using value_str_t = value_base_t<pstring>;
        public class value_str_t : value_base_t_string { public value_str_t(param_model_t param, string name) : base(param, name) { } }


        public param_model_t(core_device_t device, string name, string val) : base(device, name, val) { }


        string value_str(string entity)
        {
            return state().setup().models().get_model(str()).value_str(entity);
        }


        nl_fptype value(string entity)
        {
            return state().setup().models().get_model(str()).value(entity);
        }


        //pstring type();


        // hide this
        //void set(const pstring &param) = delete;


        protected override void changed() { }
    }


    // -----------------------------------------------------------------------------
    // data parameter
    // -----------------------------------------------------------------------------
    //class param_data_t : public param_str_t


    // -----------------------------------------------------------------------------
    // rom parameter
    // -----------------------------------------------------------------------------
    //template <typename ST, std::size_t AW, std::size_t DW>
    //class param_rom_t final: public param_data_t


        //OLD
#if false
    // -----------------------------------------------------------------------------
    // core_device_t
    // -----------------------------------------------------------------------------
    public class core_device_t : detail.netlist_object_t,
                                 logic_family_t
    {
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
            //: netlist_object_t(owner.exec(), name)
            //: netlist_object_t(owner.state().exec(), owner.name() + "." + name)
            : base(owner is netlist_state_t ? ((netlist_state_t)owner).exec() : 
                   (owner is core_device_t ? ((core_device_t)owner).state().exec() : 
                   null),
                   owner is netlist_state_t ? name : 
                   (owner is core_device_t ? ((core_device_t)owner).name() + "." + name :
                   ""))
        {
            assert(owner is netlist_state_t || owner is core_device_t);

            if (owner is netlist_state_t)
                core_device_t_ctor((netlist_state_t)owner, name);
            else if (owner is core_device_t)
                core_device_t_ctor((core_device_t)owner, name);
            else
                throw new emu_fatalerror("unknown type: {0}", owner.GetType());
        }


        void core_device_t_ctor(netlist_state_t owner, string name)
            //: netlist_object_t(owner.exec(), name)
        {
            m_hint_deactivate = false;
            m_active_outputs = new state_var_s32(this, "m_active_outputs", 1);


            if (exec().stats_enabled())
                m_stats = new stats_t();  //m_stats = owner.make_pool_object<stats_t>();
        }


        void core_device_t_ctor(core_device_t owner, string name)
            //: netlist_object_t(owner.state().exec(), owner.name() + "." + name)
        {
            m_hint_deactivate = false;
            m_active_outputs = new state_var_s32(this, "m_active_outputs", 1);


            //printf("owned device: %s\n", this->name().c_str());

            owner.state().register_device(this.name(), this);  //owner.state().register_device(this->name(), device_arena::owned_ptr<core_device_t>(this, false));
            if (exec().stats_enabled())
                m_stats = new stats_t();  //m_stats = owner.state().make_pool_object<stats_t>();
        }

        //PCOPYASSIGNMOVE(core_device_t, delete)

        //virtual ~core_device_t() noexcept = default;


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
            if (term.delegate_() == null)  //if (!term.delegate().is_set())
                term.set_delegate(update);  //term.set_delegate(nldelegate(&core_device_t::update, this));
        }


        public virtual void update() { }
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
#endif


    // -----------------------------------------------------------------------------
    // core_device_t
    // -----------------------------------------------------------------------------
    public class core_device_t : detail.netlist_object_t
    {
        // stats
        struct stats_t
        {
            // NL_KEEP_STATISTICS
            //plib::pperftime_t<true>  m_stat_total_time;
            //plib::pperfcount_t<true> m_stat_call_count;
            //plib::pperfcount_t<true> m_stat_inc_active;
        }


        bool m_hint_deactivate;
        state_var_s32 m_active_outputs;
        stats_t m_stats;  //device_arena::unique_ptr<stats_t> m_stats;


        protected core_device_t(object owner, string name)
            : base(owner is netlist_state_t ? ((netlist_state_t)owner).exec() : ((core_device_t)owner).state().exec(), owner is netlist_state_t ? name : ((core_device_t)owner).name() + "." + name)
        {
            if (owner is netlist_state_t)
                core_device_t_after_ctor((netlist_state_t)owner, name);
            else if (owner is core_device_t)
                core_device_t_after_ctor((core_device_t)owner, name);
            else
                throw new emu_unimplemented();
        }

        void core_device_t_after_ctor(netlist_state_t owner, string name)
            //: base(owner.exec(), name)
        {
            m_hint_deactivate = false;
            m_active_outputs = new state_var_s32(this, "m_active_outputs", 1);


            if (exec().stats_enabled())
                m_stats = new stats_t();  //m_stats = owner.make_pool_object<stats_t>();
        }

        void core_device_t_after_ctor(core_device_t owner, string name)
            //: base(owner.state().exec(), owner.name() + "." + name)
        {
            m_hint_deactivate = false;
            m_active_outputs = new state_var_s32(this, "m_active_outputs", 1);


            //printf("owned device: %s\n", this->name().c_str());
            owner.state().register_device(this.name(), this);  //owner.state().register_device(this->name(), device_arena::owned_ptr<core_device_t>(this, false));
            if (exec().stats_enabled())
                m_stats = new stats_t();  //m_stats = owner.state().make_pool_object<stats_t>();
        }


        //PCOPYASSIGNMOVE(core_device_t, delete)

        //virtual ~core_device_t() noexcept = default;

        //void do_inc_active() noexcept

        //void do_dec_active() noexcept


        public void set_hint_deactivate(bool v) { m_hint_deactivate = v; }


        //bool get_hint_deactivate() const noexcept { return m_hint_deactivate; }
        // Has to be set in device reset
        //void set_active_outputs(int n) noexcept { m_active_outputs = n; }


        public void set_default_delegate(detail.core_terminal_t term)
        {
            if (term.delegate_() == null)
                term.set_delegate(update);
        }


        // stats
        //struct stats_t
        //{
        //    // NL_KEEP_STATISTICS
        //    plib::pperftime_t<true>  m_stat_total_time;
        //    plib::pperfcount_t<true> m_stat_call_count;
        //    plib::pperfcount_t<true> m_stat_inc_active;
        //};

        //stats_t * stats() const noexcept { return m_stats.get(); }


        public virtual void update() { }
        public virtual void reset() { }


        protected virtual void inc_active() { }
        protected virtual void dec_active() { }


        protected log_type log()
        {
            return state().log();
        }


        protected virtual void timestep(nl_fptype st) { }  // plib.unused_var(st); }
        public virtual void update_terminals() { }

        public virtual void update_param() { }
        protected virtual bool is_dynamic() { return false; }
        protected virtual bool is_timestep() { return false; }
    }


    // -----------------------------------------------------------------------------
    // base_device_t
    // -----------------------------------------------------------------------------
    public class base_device_t : core_device_t
    {
        //protected base_device_t(netlist_state_t owner, string name)
        //    : base(owner, name)
        //{
        //}

        //protected base_device_t(base_device_t owner, string name)
        //    : base(owner, name)
        //{
        //}

        protected base_device_t(object owner, string name)
            : base(owner, name)
        {
        }

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


        public override void update() { }  //NETLIB_UPDATEI() { }
        public override void update_terminals() { }  //NETLIB_UPDATE_TERMINALSI() { }
    }


    // -----------------------------------------------------------------------------
    // device_t
    // -----------------------------------------------------------------------------
    public class device_t : base_device_t,
                            logic_family_t
    {
        param_model_t m_model;


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


        ////nldelegate default_delegate() { return nldelegate(&device_t::update, this); }
        public nldelegate default_delegate() { return update; }  //nldelegate default_delegate() { return { &device_t::update, this }; }


        public override void update() { }  //NETLIB_UPDATEI() { }
        public override void update_terminals() { }  //NETLIB_UPDATE_TERMINALSI() { }
    }


    namespace detail
    {
        // Use timed_queue_heap to use stdc++ heap functions instead of linear processing.
        // This slows down processing by about 25% on a Kaby Lake.
        // template <class T, bool TS>
        // using timed_queue = plib::timed_queue_heap<T, TS>;
        //template <class T, bool TS>
        //using timed_queue = plib::timed_queue_linear<T, TS>;


        // -----------------------------------------------------------------------------
        // queue_t
        // -----------------------------------------------------------------------------

        // We don't need a thread-safe queue currently. Parallel processing of
        // solvers will update inputs after parallel processing.
        public class queue_t : ////public timed_queue<pqentry_t<net_t *, netlist_time>, false, NL_KEEP_STATISTICS>,
                               plib.timed_queue_linear_pqentry_t_netlist_time_ext_net_t,  //public timed_queue<plib::pqentry_t<netlist_time_ext, net_t *>, false>,
                               //public netlist_object_t,
                               plib.state_manager_t.callback_t
        {
            //using entry_t = plib::pqentry_t<netlist_time_ext, net_t *>;
            //using base_queue = timed_queue<entry_t, false>;


            netlist_object_t m_netlist_object_t;


            size_t m_qsize;  //std::size_t m_qsize;
            std.vector<UInt64> m_times;  //std::vector<netlist_time_ext::internal_type> m_times;
            std.vector<size_t> m_net_ids;  //std::vector<std::size_t> m_net_ids;


            public queue_t(netlist_t nl, string name)
                : base(config.MAX_QUEUE_SIZE)  //: timed_queue<plib::pqentry_t<netlist_time_ext, net_t *>, false>(config::MAX_QUEUE_SIZE::value)
                //, netlist_object_t(nl, name)
            {
                m_netlist_object_t = new netlist_object_t(nl, name);


                m_qsize = 0;
                m_times = new std.vector<UInt64>(config.MAX_QUEUE_SIZE);
                m_net_ids = new std.vector<size_t>(config.MAX_QUEUE_SIZE);
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
        //std::size_t find_net_id(const detail::net_t *net) const;


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


        public void reset()
        {
            m_setup = null;

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

            log().verbose.op("Call update_param on all devices:");
            foreach (var dev in m_devices)
                dev.second.update_param();

            // Step all devices once !
            //
            // INFO: The order here affects power up of e.g. breakout. However, such
            // variations are explicitly stated in the breakout manual.

            var netlist_params = get_single_device<devices.nld_netlistparams>("parameter");

            switch (netlist_params.m_startup_strategy.op())
            {
                case 0:
                {
                    std.vector<core_device_t> d = new std.vector<core_device_t>();
                    std.vector<nldelegate> t = new std.vector<nldelegate>();
                    log().verbose.op("Using default startup strategy");
                    foreach (var n in m_nets)
                    {
                        foreach (var term in n.core_terms())
                        {
                            if (term.delegate_() != null)  //if (term->delegate().has_object())
                            {
                                if (!plib.container.contains(t, term.delegate_()))
                                {
                                    t.push_back(term.delegate_());
                                    term.run_delegate();
                                }

                                throw new emu_unimplemented();
#if false
                                // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
                                auto *dev = reinterpret_cast<core_device_t *>(term->delegate().object());
                                if (!plib::container::contains(d, dev))
                                    d.push_back(dev);
#endif
                            }
                        }
                    }

                    log().verbose.op("Devices not yet updated:");
                    foreach (var dev in m_devices)
                    {
                        if (!plib.container.contains(d, dev.second))
                        {
                            log().verbose.op("\t ...{0}", dev.second.name());
                            dev.second.update();
                        }
                    }
                }
                break;

                case 1:     // brute force backward
                {
                    log().verbose.op("Using brute force backward startup strategy");

                    foreach (var n in m_nets)  // only used if USE_COPY_INSTEAD_OF_REFERENCE == 1
                        n.update_inputs();

                    int i = m_devices.size();
                    while (i > 0)
                        m_devices[--i].second.update();

                    foreach (var n in m_nets)  // only used if USE_COPY_INSTEAD_OF_REFERENCE == 1
                        n.update_inputs();

                }
                break;

                case 2:     // brute force forward
                {
                    log().verbose.op("Using brute force forward startup strategy");
                    foreach (var d in m_devices)
                        d.second.update();
                }
                break;
            }

#if true
            // the above may screw up m_active and the list
            rebuild_lists();
#endif
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


                m_inc = netlist_time.from_fp(plib.pglobal.reciprocal(m_freq.op() * nlconst.two()));
            }


            //NETLIB_RESETI();
            public override void reset()
            {
                m_Q.net().set_next_scheduled_time(netlist_time_ext.zero());
            }


            //NETLIB_UPDATE_PARAMI();
            public override void update_param()
            {
                m_inc = netlist_time.from_fp(plib.pglobal.reciprocal(m_freq.op() * nlconst.two()));
            }

            //NETLIB_UPDATEI();
            public override void update()
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


        public netlist_t(netlist_state_t state, string aname)
        {
            m_state = state;
            m_solver = null;
            m_time = netlist_time_ext.zero();
            m_mainclock = null;
            m_queue = new detail.queue_t(this, aname + "." + "m_queue");
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

        public detail.queue_t queue() { return m_queue; }


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

            m_time = netlist_time_ext.zero();
            m_queue.clear();
            if (m_mainclock != null)
                m_mainclock.m_Q.net().set_next_scheduled_time(netlist_time_ext.zero());
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


        public bool stats_enabled() { return m_use_stats; }
        //void enable_stats(bool val) { m_use_stats = val; }


        //template <bool KEEP_STATS>
        //void process_queue_stats(netlist_time delta) noexcept;
    }


    // -----------------------------------------------------------------------------
    // Support classes for devices
    // -----------------------------------------------------------------------------
    //template<class C, std::size_t N>
    class object_array_base_t<C> : plib.uninitialised_array_t<C>  //class object_array_base_t : public plib::uninitialised_array_t<C, N>
    {
        size_t N;


        //template<class D, typename... Args>
        protected object_array_base_t(size_t N, object dev, string [] names)  //object_array_base_t(D &dev, const std::initializer_list<const char *> &names, Args&&... args)
        {
            this.N = N;

            throw new emu_unimplemented();
#if false
            //passert_always_msg(names.size() == N, "initializer_list size mismatch");
            size_t i = 0;
            foreach (var n in names)
                this.emplace(i++, dev, n);  //this->emplace(i++, dev, pstring(n), std::forward<Args>(args)...);
#endif
        }

        //template<class D>
        protected object_array_base_t(size_t N, object dev, string fmt)  //object_array_base_t(D &dev, const pstring &fmt)
        {
            this.N = N;

            throw new emu_unimplemented();
#if false
            for (size_t i = 0; i < N; i++)
                this.emplace(i, dev, formatted(fmt, i));  //this->emplace(i, dev, formatted(fmt, i));
#endif
        }

        //template<class D, typename... Args>
        protected object_array_base_t(size_t N, object dev, size_t offset, string fmt)  //object_array_base_t(D &dev, std::size_t offset, const pstring &fmt, Args&&... args)
        {
            this.N = N;

            throw new emu_unimplemented();
#if false
            for (size_t i = 0; i < N; i++)
                this.emplace(i, dev, formatted(fmt, i + offset));  //this->emplace(i, dev, formatted(fmt, i+offset), std::forward<Args>(args)...);
#endif
        }

        //template<class D>
        protected object_array_base_t(size_t N, object dev, size_t offset, string fmt, nldelegate delegate_)  //object_array_base_t(D &dev, std::size_t offset, const pstring &fmt, nldelegate delegate)
        {
            this.N = N;

            throw new emu_unimplemented();
#if false
            for (size_t i = 0; i < N; i++)
                this.emplace(i, dev, formatted(fmt, i + offset), delegate_);  //this->emplace(i, dev, formatted(fmt, i+offset), delegate);
#endif
        }

        //template<class D>
        protected object_array_base_t(size_t N, object dev, size_t offset, size_t qmask, string fmt)  //object_array_base_t(D &dev, std::size_t offset, std::size_t qmask, const pstring &fmt)
        {
            this.N = N;

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


        /*static*/ string formatted(string fmt, size_t n)
        {
            if (N != 1)
                return new plib.pfmt(fmt).op(n);

            return new plib.pfmt(fmt).op("");
        }
    }


    //template<class C, std::size_t N>
    class object_array_t<C> : object_array_base_t<C>  //class object_array_t : public object_array_base_t<C, N>
    {
        //using base_type = object_array_base_t<C, N>;
        //using base_type::base_type;


        //template<class D, typename... Args>
        public object_array_t(size_t N, object dev, string [] names) : base(N, dev, names) { }

        //template<class D>
        public object_array_t(size_t N, object dev, string fmt) : base(N, dev, fmt) { }

        //template<class D, typename... Args>
        public object_array_t(size_t N, object dev, size_t offset, string fmt) : base(N, dev, offset, fmt) { }

        //template<class D>
        public object_array_t(size_t N, object dev, size_t offset, string fmt, nldelegate delegate_) : base(N, dev, offset, fmt, delegate_) { }

        //template<class D>
        public object_array_t(size_t N, object dev, size_t offset, size_t qmask, string fmt) : base(N, dev, offset, qmask, fmt) { }

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


        public nld_power_pins(device_t owner, string sVCC = nl_errstr_global.sPowerVCC, string sGND = nl_errstr_global.sPowerGND)
        {
            m_VCC = new analog_input_t(owner, sVCC, noop);
            m_GND = new analog_input_t(owner, sGND, noop);
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
