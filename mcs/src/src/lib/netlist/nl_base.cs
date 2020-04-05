// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Diagnostics;

using analog_net_t_list_t = mame.std.vector<mame.netlist.analog_net_t>;
using devices_collection_type = mame.std.vector<System.Collections.Generic.KeyValuePair<string, mame.netlist.core_device_t>>;  //using devices_collection_type = std::vector<std::pair<pstring, poolptr<core_device_t>>>;
using entry_t = mame.netlist.pqentry_t<mame.netlist.detail.net_t, mame.netlist.ptime_u64>;
using log_type = mame.plib.plog_base<mame.netlist.callbacks_t>;//, NL_DEBUG>;
using model_map_t = mame.std.unordered_map<string, string>;
using netlist_base_t = mame.netlist.netlist_state_t;
using netlist_sig_t = System.UInt32;
using netlist_time = mame.netlist.ptime_u64;  //using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;
using nets_collection_type = mame.std.vector<mame.netlist.detail.net_t>;  //using nets_collection_type = std::vector<poolptr<detail::net_t>>;
using nl_double = System.Double;
using state_var_s32 = mame.netlist.state_var<System.Int32>;
using timed_queue = mame.netlist.timed_queue_linear;


namespace mame.netlist
{
    //============================================================
    //  MACROS / New Syntax
    //============================================================

    /*! Construct a netlist device name */
    //#define NETLIB_NAME(chip) nld_ ## chip

    //#define NETLIB_OBJECT_DERIVED(name, pclass)                                   \
    //class NETLIB_NAME(name) : public NETLIB_NAME(pclass)

    /*! Start a netlist device class.
    *  Used to start defining a netlist device class.
    *  The simplest device without inputs or outputs would look like this:
    *
    *      NETLIB_OBJECT(base_dummy)
    *      {
    *      public:
    *          NETLIB_CONSTRUCTOR(base_dummy) { }
    *      };
    *
    *  Also refer to #NETLIB_CONSTRUCTOR.
    */
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

    /*! Used to define the constructor of a netlist device.
    *  Use this to define the constructor of a netlist device. Please refer to
    *  #NETLIB_OBJECT for an example.
    */
    //#define NETLIB_CONSTRUCTOR(cname)                                              \
    //    private: detail::family_setter_t m_famsetter;                              \
    //    public: template <class CLASS> NETLIB_NAME(cname)(CLASS &owner, const pstring &name) \
    //        : device_t(owner, name)

    /*! Used to define the destructor of a netlist device.
    *  The use of a destructor for netlist device should normally not be necessary.
    */
    //#define NETLIB_DESTRUCTOR(name) public: virtual ~NETLIB_NAME(name)()

    /*! Define an extended constructor and add further parameters to it.
    *  The macro allows to add further parameters to a device constructor. This is
    *  normally used for sub-devices and system devices only.
    */
    //#define NETLIB_CONSTRUCTOR_EX(cname, ...)                                      \
    //    private: detail::family_setter_t m_famsetter;                              \
    //    public: template <class CLASS> NETLIB_NAME(cname)(CLASS &owner, const pstring &name, __VA_ARGS__) \
    //        : device_t(owner, name)

    /*! Add this to a device definition to mark the device as dynamic.
    *  If NETLIB_IS_DYNAMIC(true) is added to the device definition the device
    *  is treated as an analog dynamic device, i.e. #NETLIB_UPDATE_TERMINALSI
    *  is called on a each step of the Newton-Raphson step
    *  of solving the linear equations.
    *
    *  You may also use e.g. NETLIB_IS_DYNAMIC(m_func() != "") to only make the
    *  device a dynamic device if parameter m_func is set.
    */
    //#define NETLIB_IS_DYNAMIC(expr)                                                \
    //    public: virtual bool is_dynamic() const override { return expr; }

    /*! Add this to a device definition to mark the device as a time-stepping device.
    *
    *  You have to implement NETLIB_TIMESTEP in this case as well. Currently, only
    *  the capacitor and inductor devices uses this.
    *
    *  You may also use e.g. NETLIB_IS_TIMESTEP(m_func() != "") to only make the
    *  device a dynamic device if parameter m_func is set. This is used by the
    *  Voltage Source element.
    *
    *  Example:
    *
    *   NETLIB_TIMESTEP_IS_TIMESTEP()
    *   NETLIB_TIMESTEPI()
    *       {
    *           // Gpar should support convergence
    *           const nl_double G = m_C.Value() / step +  m_GParallel;
    *           const nl_double I = -G * deltaV();
    *           set(G, 0.0, I);
    *       }
    *
    */
    //#define NETLIB_IS_TIMESTEP(expr)                                               \
    //    public: virtual bool is_timestep() const override { return expr; }

    /*! Used to implement the time stepping code.
    *
    * Please see NETLIB_IS_TIMESTEP for an example.
    */
    //#define NETLIB_TIMESTEPI()                                                     \
    //    public: virtual void timestep(const nl_double step) override

    //#define NETLIB_FAMILY(family) , m_famsetter(*this, family)

    //#define NETLIB_DELEGATE(chip, name) nldelegate(&NETLIB_NAME(chip) :: name, this)

    //#define NETLIB_UPDATE_TERMINALSI() virtual void update_terminals() override
    //#define NETLIB_HANDLERI(name) virtual void name() NL_NOEXCEPT
    //#define NETLIB_UPDATEI() virtual void update() NL_NOEXCEPT override
    //#define NETLIB_UPDATE_PARAMI() virtual void update_param() override
    //#define NETLIB_RESETI() virtual void reset() override

    //#define NETLIB_TIMESTEP(chip) void NETLIB_NAME(chip) :: timestep(const nl_double step)

    //#define NETLIB_SUB(chip) nld_ ## chip
    //#define NETLIB_SUBXX(ns, chip) poolptr< ns :: nld_ ## chip >

    //#define NETLIB_HANDLER(chip, name) void NETLIB_NAME(chip) :: name() NL_NOEXCEPT
    //#define NETLIB_UPDATE(chip) NETLIB_HANDLER(chip, update)

    //#define NETLIB_RESET(chip) void NETLIB_NAME(chip) :: reset(void)

    //#define NETLIB_UPDATE_PARAM(chip) void NETLIB_NAME(chip) :: update_param()
    //#define NETLIB_FUNC_VOID(chip, name, params) void NETLIB_NAME(chip) :: name params

    //#define NETLIB_UPDATE_TERMINALS(chip) void NETLIB_NAME(chip) :: update_terminals()


    static class nl_base_global
    {
        //============================================================
        //  Asserts
        //============================================================

        //#if defined(MAME_DEBUG)
        //#define nl_assert(x)    do { if (1) if (!(x)) throw nl_exception(plib::pfmt("assert: {1}:{2}: {3}")(__FILE__)(__LINE__)(#x) ); } while (0)
        //#define NL_NOEXCEPT
        //#else
        //#define nl_assert(x)    do { if (0) if (!(x)) { /*throw nl_exception(plib::pfmt("assert: {1}:{2}: {3}")(__FILE__)(__LINE__)(#x) ); */} } while (0)
        //#define NL_NOEXCEPT     noexcept
        //#endif
        [Conditional("DEBUG")] public static void nl_assert(bool condition) { global_object.assert(condition); }

        //#define nl_assert_always(x, msg)    do { if (!(x)) throw nl_exception(plib::pfmt("Fatal error: {1}\nCaused by assert: {2}:{3}: {4}")(msg)(__FILE__)(__LINE__)(#x)); } while (0)
        public static void nl_assert_always(bool condition, string message) { global_object.assert_always(condition, message); }


        static logic_family_ttl_t family_TTL_obj;
        public static logic_family_desc_t family_TTL() { return family_TTL_obj; }  //*!< logic family for TTL devices.

        static logic_family_cd4xxx_t family_CD4XXX_obj;
        public static logic_family_desc_t family_CD4XXX() { return family_CD4XXX_obj; }  //*!< logic family for CD4XXX CMOS devices.
    }


    /*! Generic netlist exception.
     *  The exception is used in all events which are considered fatal.
     */
    class nl_exception : Exception  //plib.pexception
    {
        /*! Constructor.
         *  Allows a descriptive text to be assed to the exception
         */
        public nl_exception(string text)  //!< text to be passed
            : base(text) { }
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


    /*! Logic families descriptors are used to create proxy devices.
     *  The logic family describes the analog capabilities of logic devices,
     *  inputs and outputs.
     */
    public abstract class logic_family_desc_t
    {
        double m_fixed_V;           //!< For variable voltage families, specify 0. For TTL this would be 5. */
        double m_low_thresh_PCNT;   //!< low input threshhold offset. If the input voltage is below this value times supply voltage, a "0" input is signalled
        double m_high_thresh_PCNT;  //!< high input threshhold offset. If the input voltage is above the value times supply voltage, a "0" input is signalled
        double m_low_VO;            //!< low output voltage offset. This voltage is output if the ouput is "0"
        double m_high_VO;           //!< high output voltage offset. The supply voltage minus this offset is output if the ouput is "1"
        double m_R_low;             //!< low output resistance. Value of series resistor used for low output
        double m_R_high;            //!< high output resistance. Value of series resistor used for high output


        public logic_family_desc_t() { }
        //~logic_family_desc_t() { }

        //COPYASSIGNMOVE(logic_family_desc_t, delete)


        public double fixed_V { get { return m_fixed_V; } set { m_fixed_V = value; } }
        public double low_thresh_PCNT { get { return m_low_thresh_PCNT; } set { m_low_thresh_PCNT = value; } }
        public double high_thresh_PCNT { get { return m_high_thresh_PCNT; } set { m_high_thresh_PCNT = value; } }
        public double low_VO { get { return m_low_VO; } set { m_low_VO = value; } }
        public double high_VO { get { return m_high_VO; } set { m_high_VO = value; } }
        public double R_low { get { return m_R_low; } set { m_R_low = value; } }
        public double R_high { get { return m_R_high; } set { m_R_high = value; } }


        public abstract devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_state_t anetlist, string name, logic_output_t proxied);  //virtual poolptr<devices::nld_base_d_to_a_proxy> create_d_a_proxy(netlist_state_t &anetlist, const pstring &name, logic_output_t *proxied) const = 0;
        public abstract devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_state_t anetlist, string name, logic_input_t proxied);  //virtual poolptr<devices::nld_base_a_to_d_proxy> create_a_d_proxy(netlist_state_t &anetlist, const pstring &name, logic_input_t *proxied) const = 0;


        //double fixed_V() const { return m_fixed_V; }
        //double low_thresh_V(const double VN, const double VP) const { return VN + (VP - VN) * m_low_thresh_PCNT; }
        //double high_thresh_V(const double VN, const double VP) const { return VN + (VP - VN) * m_high_thresh_PCNT; }
        //double low_V(const double VN, const double VP) const { return VN + m_low_VO; }
        //double high_V(const double VN, const double VP) const { return VP - m_high_VO; }
        //double R_low() const { return m_R_low; }
        //double R_high() const { return m_R_high; }
    }


    /*! Base class for devices, terminals, outputs and inputs which support
     *  logic families.
     *  This class is a storage container to store the logic family for a
     *  netlist object. You will not directly use it. Please refer to
     *  #NETLIB_FAMILY to learn how to define a logic family for a device.
     *
     * All terminals inherit the family description from the device
     * The default is the ttl family, but any device can override the family.
     * For individual terminals, the family can be overwritten as well.
     *
     */
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

    //const logic_family_desc_t *family_TTL();        //*!< logic family for TTL devices.
    //const logic_family_desc_t *family_CD4XXX();     //*!< logic family for CD4XXX CMOS devices.


    /*! A persistent variable template.
     *  Use the state_var template to define a variable whose value is saved.
     *  Within a device definition use
     *
     *      NETLIB_OBJECT(abc)
     *      {
     *          NETLIB_CONSTRUCTOR(abc)
     *          , m_var(*this, "myvar", 0)
     *          ...
     *          state_var<unsigned> m_var;
     *      }
     */
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


        //! Copy Constructor.
        //state_array(const state_array &rhs) NL_NOEXCEPT = default;
        //! Destructor.
        //~state_array() noexcept = default;
        //! Move Constructor.
        //state_array(state_array &&rhs) NL_NOEXCEPT = default;
        //state_array &operator=(const state_array &rhs) NL_NOEXCEPT = default;
        //state_array &operator=(state_array &&rhs) NL_NOEXCEPT = default;

        //state_array &operator=(const T &rhs) NL_NOEXCEPT { m_value = rhs; return *this; }
        //T & operator[](const std::size_t i) NL_NOEXCEPT { return m_value[i]; }
        //constexpr const T & operator[](const std::size_t i) const NL_NOEXCEPT { return m_value[i]; }

        public T op { get { return m_value; } set { m_value = value; } }
    }


    // -----------------------------------------------------------------------------
    // State variables - predefined and c++11 non-optional
    // -----------------------------------------------------------------------------

    /*! predefined state variable type for uint8_t */
    //using state_var_u8 = state_var<std::uint8_t>;
    /*! predefined state variable type for int8_t */
    //using state_var_s8 = state_var<std::int8_t>;

    /*! predefined state variable type for uint32_t */
    //using state_var_u32 = state_var<std::uint32_t>;
    /*! predefined state variable type for int32_t */
    //using state_var_s32 = state_var<std::int32_t>;
    /*! predefined state variable type for sig_t */
    //using state_var_sig = state_var<netlist_sig_t>;


    namespace detail
    {
        // -----------------------------------------------------------------------------
        // object_t
        // -----------------------------------------------------------------------------

        /*! The base class for netlist devices, terminals and parameters.
         *
         *  This class serves as the base class for all device, terminal and
         *  objects. It provides new and delete operators to support e.g. pooled
         *  memory allocation to enhance locality. Please refer to \ref USE_MEMPOOL as
         *  well.
         */
        public class object_t : global_object
        {
            //string m_name;  //std::unique_ptr<pstring> m_name;

            static std.unordered_map<object_t, string> name_hash_lhash;
            static std.unordered_map<object_t, string> name_hash()
            {
                //static std::unordered_map<const object_t *, pstring> lhash;
                return name_hash_lhash;
            }


            /*! Constructor.
             *
             *  Every class derived from the object_t class must have a name.
             */
            public object_t(string aname /*!< string containing name of the object */)
            {
                name_hash().insert(this, aname);
            }

            //COPYASSIGNMOVE(object_t, delete)

            // only childs should be destructible
            ~object_t()
            {
                name_hash().erase(this);  //name_hash().erase(name_hash().find(this));
            }


            /*! return name of the object
             *
             *  \returns name of the object.
             */
            public string name() { return name_hash().find(this); }
        }


        struct netlist_ref //: netlist_interface
        {
            netlist_t m_netlist;


            public netlist_ref(netlist_state_t nl) { m_netlist = nl.exec(); }

            //COPYASSIGNMOVE(netlist_ref, delete)

            public netlist_state_t state() { return m_netlist.nlstate(); }

            setup_t setup() { return m_netlist.nlstate().setup(); }

            public netlist_t exec() { return m_netlist; }
        }


        // -----------------------------------------------------------------------------
        // device_object_t
        // -----------------------------------------------------------------------------

        /*! Base class for all objects being owned by a device.
         *
         * Serves as the base class of all objects being owned by a device.
         *
         */
        public class device_object_t : detail.object_t, netlist_interface_plus_name
        {
            core_device_t m_device;


            /*! Constructor.
             *
             * \param dev  device owning the object.
             * \param name string holding the name of the device
             */
            public device_object_t(core_device_t dev, string aname)
                : base(aname)
            {
                m_device = dev;
            }

            /*! returns reference to owning device.
             * \returns reference to owning device.
             */
            public core_device_t device() { return m_device; }

            /*! The netlist owning the owner of this object.
             * \returns reference to netlist object.
             */
            public netlist_state_t state() { return m_device.state(); }

            public netlist_t exec() { return m_device.exec(); }
        }


        // -----------------------------------------------------------------------------
        // core_terminal_t
        // -----------------------------------------------------------------------------
        /*! Base class for all terminals.
         *
         * All terminals are derived from this class.
         *
         */
        public class core_terminal_t : device_object_t
                                       //public plib::linkedlist_t<core_terminal_t>::element_t
        {
            //using list_t = std::vector<core_terminal_t *>;


            public const int INP_HL_SHIFT = 0;
            public const int INP_LH_SHIFT = 1;
            const int INP_ACTIVE_SHIFT = 2;


            public enum state_e
            {
                STATE_INP_PASSIVE = 0,
                STATE_INP_HL      = (1 << INP_HL_SHIFT),
                STATE_INP_LH      = (1 << INP_LH_SHIFT),
                STATE_INP_ACTIVE  = (1 << INP_ACTIVE_SHIFT),
                STATE_OUT = 128,
                STATE_BIDIR = 256
            }


            nldelegate m_delegate;

            net_t m_net;
            state_var<state_e> m_state;


            public core_terminal_t(core_device_t dev, string aname, state_e state, nldelegate delegate_ = null)  //nldelegate())
                : base(dev, dev.name() + "." + aname)
                //, plib::linkedlist_t<core_terminal_t>::element_t()
            {
                m_delegate = delegate_;
                m_net = null;
                m_state = new state_var<state_e>(this, "m_state", state);
            }

            //~core_terminal_t() { }

            //COPYASSIGNMOVE(core_terminal_t, delete)

            public nldelegate delegate_ { get { return m_delegate; } set { m_delegate = value; } }


            /*! The object type.
             * \returns type of the object
             */
            public terminal_type type()
            {
                if (this is terminal_t)
                    return terminal_type.TERMINAL;
                else if (this is logic_input_t)
                    return terminal_type.INPUT;
                else if (this is logic_output_t)
                    return terminal_type.OUTPUT;
                else if (this is analog_input_t)
                    return terminal_type.INPUT;
                else if (this is analog_output_t)
                    return terminal_type.OUTPUT;
                else
                {
                    state().log().fatal.op(nl_errstr_global.MF_UNKNOWN_TYPE_FOR_OBJECT(name()));
                    return terminal_type.TERMINAL; // please compiler
                }
            }


            /*! Checks if object is of specified type.
             * \param atype type to check object against.
             * \returns true if object is of specified type else false.
             */
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
    class analog_t : detail.core_terminal_t
    {
        public analog_t(core_device_t dev, string aname, state_e state)
            : base(dev, aname, state)
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
    class terminal_t : analog_t
    {
        terminal_t m_connected_terminal;
        ListPointer<nl_double> m_Idr1;  //nl_double *m_Idr1; // drive current
        ListPointer<nl_double> m_go1;  //nl_double *m_go1;  // conductance for Voltage from other term
        ListPointer<nl_double> m_gt1;  //nl_double *m_gt1;  // conductance for total conductance


        // ----------------------------------------------------------------------------------------
        // terminal_t
        // ----------------------------------------------------------------------------------------
        public terminal_t(core_device_t dev, string aname, terminal_t otherterm)
            : base(dev, aname, state_e.STATE_BIDIR)
        {
            m_Idr1 = null;
            m_go1 = null;
            m_gt1 = null;
            m_connected_terminal = otherterm;


            state().setup().register_term(this);
        }

        //~terminal_t() { }


        //nl_double operator ()() const  NL_NOEXCEPT;

        //void set_conductivity(const nl_double &G) NL_NOEXCEPT
        //void set_go_gt(const nl_double &GO, const nl_double &GT) NL_NOEXCEPT

        public void set_go_gt_I(nl_double GO, nl_double GT, nl_double I)
        {
            if (m_go1 != null)
            {
                if (m_Idr1[0] != I) m_Idr1[0] = I;  //if (*m_Idr1 != I) *m_Idr1 = I;
                if (m_go1[0] != GO) m_go1[0] = GO;  //if (*m_go1 != GO) *m_go1 = GO;
                if (m_gt1[0] != GT) m_gt1[0] = GT;  //if (*m_gt1 != GT) *m_gt1 = GT;
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


        public void set_ptrs(ListPointer<nl_double> gt, ListPointer<nl_double> go, ListPointer<nl_double> Idr)  //void set_ptrs(nl_double *gt, nl_double *go, nl_double *Idr) noexcept;
        {
            if (!(gt != null && go != null && Idr != null) && (gt != null || go != null || Idr != null))  //if (!(gt && go && Idr) && (gt || go || Idr))
            {
                state().log().fatal.op("Inconsistent nullptrs for terminal {0}", name());
            }
            else
            {
                m_gt1 = new ListPointer<nl_double>(gt);  //m_gt1 = gt;
                m_go1 = new ListPointer<nl_double>(go);  //m_go1 = go;
                m_Idr1 = new ListPointer<nl_double>(Idr);  //m_Idr1 = Idr;
            }
        }


        public terminal_t connected_terminal() { return m_connected_terminal; }
    }


    // -----------------------------------------------------------------------------
    // logic_t
    // -----------------------------------------------------------------------------
    public class logic_t : detail.core_terminal_t,
                           logic_family_t
    {
        devices.nld_base_proxy m_proxy;


        public logic_t(core_device_t dev, string aname, state_e state, nldelegate delegate_ = null)  // = nldelegate());
            : base(dev, aname, state, delegate_)
            //, logic_family_t()
        {
            m_proxy = null;
        }


        // logic_family_t
        logic_family_desc_t m_logic_family;
        public logic_family_desc_t logic_family() { return m_logic_family; }
        public void set_logic_family(logic_family_desc_t fam) { m_logic_family = fam; }


        public bool has_proxy() { return m_proxy != null; }
        public devices.nld_base_proxy get_proxy() { return m_proxy; }
        public void set_proxy(devices.nld_base_proxy proxy) { m_proxy = proxy; }

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
    /*! terminal providing analog input voltage.
     *
     * This terminal class provides a voltage measurement. The conductance against
     * ground is infinite.
     */
    class analog_input_t : analog_t
    {
        /*! Constructor */
        public analog_input_t(core_device_t dev, /*!< owning device */
                              string aname)      /*!< name of terminal */
            : base(dev, aname, state_e.STATE_INP_ACTIVE)
        {
            state().setup().register_term(this);
        }


        /*! returns voltage at terminal.
         *  \returns voltage at terminal.
         */
        //nl_double operator()() const NL_NOEXCEPT { return Q_Analog(); }
        public nl_double op() { return Q_Analog(); }

        /*! returns voltage at terminal.
         *  \returns voltage at terminal.
         */
        nl_double Q_Analog() { return net().Q_Analog(); }
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
            state_var<queue_status> m_in_queue;    /* 0: not in queue, 1: in queue, 2: last was taken */
            state_var<netlist_time> m_next_scheduled_time;

            core_terminal_t m_railterminal;
            std.vector<core_terminal_t> m_list_active = new std.vector<core_terminal_t>();  //plib::linkedlist_t<core_terminal_t> m_list_active;
            std.vector<core_terminal_t> m_core_terms = new std.vector<core_terminal_t>(); // save post-start m_list ...


            public net_t(netlist_state_t nl, string aname, core_terminal_t mr = null)
                : base(aname)
                //, netlist_ref(nl)
            {
                m_netlist_ref = new netlist_ref(nl);

                m_new_Q = new state_var<netlist_sig_t>(this, "m_new_Q", 0);
                m_cur_Q = new state_var<netlist_sig_t>(this, "m_cur_Q", 0);
                m_in_queue = new state_var<queue_status>(this, "m_in_queue", queue_status.DELIVERED);
                m_next_scheduled_time = new state_var<netlist_time>(this, "m_time", netlist_time.zero());
                m_railterminal = mr;
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
                m_next_scheduled_time.op = netlist_time.zero();
                m_in_queue.op = queue_status.DELIVERED;

                m_new_Q.op = 0;
                m_cur_Q.op = 0;

                var p = this is analog_net_t ? (analog_net_t)this : null;

                if (p != null)
                    p.cur_Analog.op[0] = 0.0;

                /* rebuild m_list and reset terminals to active or analog out state */

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
                    var lexec = exec();
                    var q = lexec.queue();
                    var nst = lexec.time() + delay;

                    if (is_queued())
                        q.remove(this);

                    m_in_queue.op = !m_list_active.empty() ? queue_status.QUEUED : queue_status.DELAYED_DUE_TO_INACTIVE;    /* queued ? */
                    if (m_in_queue.op == queue_status.QUEUED)
                        q.push(new entry_t(nst, this));
                    else
                        update_inputs();

                    m_next_scheduled_time.op = nst;
                }
            }


            public bool is_queued() { return m_in_queue.op == queue_status.QUEUED; }


            public void update_devs()
            {
                nl_base_global.nl_assert(this.isRailNet());

                var new_Q = m_new_Q;

                var mask = (new_Q.op << core_terminal_t.INP_LH_SHIFT) | (m_cur_Q.op << core_terminal_t.INP_HL_SHIFT);

                m_in_queue.op = queue_status.DELIVERED; /* mark as taken ... */

                switch (mask)
                {
                    case (UInt32)core_terminal_t.state_e.STATE_INP_HL:
                    case (UInt32)core_terminal_t.state_e.STATE_INP_LH:
                        process((UInt32)(mask | (UInt32)core_terminal_t.state_e.STATE_INP_ACTIVE), new_Q.op);
                        break;
                    default:
                        /* do nothing */
                        break;
                }
            }


            public netlist_time next_scheduled_time() { return m_next_scheduled_time.op; }
            public void set_next_scheduled_time(netlist_time ntime) { m_next_scheduled_time.op = ntime; }

            public bool isRailNet() { return m_railterminal != null; }
            public core_terminal_t railterminal() { return m_railterminal; }

            public int num_cons() { return m_core_terms.size(); }

            //void add_to_active_list(core_terminal_t &term) NL_NOEXCEPT;
            //void remove_from_active_list(core_terminal_t &term) NL_NOEXCEPT;

            /* setup stuff */

            public void add_terminal(core_terminal_t terminal)
            {
                foreach (var t in m_core_terms)
                {
                    if (t == terminal)
                        state().log().fatal.op(nl_errstr_global.MF_NET_1_DUPLICATE_TERMINAL_2(name(), t.name()));
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
                }
            }


            //bool is_logic() const NL_NOEXCEPT;
            //bool detail::net_t::is_analog() const NL_NOEXCEPT { return dynamic_cast<const analog_net_t *>(this) != nullptr; }
            public bool is_analog() { return this is analog_net_t; }


            public void rebuild_list()     /* rebuild m_list after a load */
            {
                /* rebuild m_list */

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
                /* nothing needs to be done */
            }


            /* only used for logic nets */
            //netlist_sig_t Q() const noexcept { return m_cur_Q; }


            /* only used for logic nets */
            public void initial(netlist_sig_t val)
            {
                m_cur_Q.op = m_new_Q.op = val;
                update_inputs();
            }


            /* only used for logic nets */
            public void set_Q_and_push(netlist_sig_t newQ, netlist_time delay)
            {
                if (newQ != m_new_Q.op)
                {
                    m_new_Q.op = newQ;
                    push_to_queue(delay);
                }
            }


            /* only used for logic nets */
            //void set_Q_time(const netlist_sig_t newQ, const netlist_time at) NL_NOEXCEPT

            /* internal state support
             * FIXME: get rid of this and implement export/import in MAME
             */
            /* only used for logic nets */
            //netlist_sig_t *Q_state_ptr() { return m_cur_Q.ptr(); }


            //template <typename T>
            void process(UInt32 mask, netlist_sig_t sig)  //void process(const T mask, netlist_sig_t sig)
            {
                m_cur_Q.op = sig;

                foreach (var p in m_list_active)
                {
                    p.set_copied_input(sig);

                    //throw new emu_unimplemented();
#if false
                    p.device().m_stat_call_count.inc();
#endif

                    if (((UInt32)p.terminal_state() & mask) != 0)
                    {
                        //throw new emu_unimplemented();
#if false
                        auto g(p.device().m_stat_total_time.guard());
#endif

                        //p.device().m_stat_total_time.start();
                        if (p.delegate_ != null)
                            p.delegate_();
                        //p.device().m_stat_total_time.stop();
                    }
                }
            }
        }
    }


    public class logic_net_t : detail.net_t
    {
        public logic_net_t(netlist_base_t nl, string aname, detail.core_terminal_t mr = null)
            : base(nl, aname, mr)
        {
        }
    }


    class analog_net_t : detail.net_t
    {
        //using list_t =  std::vector<analog_net_t *>;

        //friend class detail::net_t;


        state_var<ListPointer<nl_double>> m_cur_Analog;  //state_var<nl_double>     m_cur_Analog;
        devices.matrix_solver_t m_solver;


        // ----------------------------------------------------------------------------------------
        // analog_net_t
        // ----------------------------------------------------------------------------------------
        public analog_net_t(netlist_state_t nl, string aname, detail.core_terminal_t mr = null)
            : base(nl, aname, mr)
        {
            m_cur_Analog = new state_var<ListPointer<nl_double>>(this, "m_cur_Analog", new ListPointer<nl_double>(new std.vector<nl_double>(1)));
            m_solver = null;
        }


        public state_var<ListPointer<nl_double>> cur_Analog { get { return m_cur_Analog; } set { m_cur_Analog = value; } }


        public nl_double Q_Analog() { return m_cur_Analog.op[0]; }
        public void set_Q_Analog(nl_double v) { m_cur_Analog.op[0] = v; }
        public ListPointer<nl_double> Q_Analog_state_ptr() { return m_cur_Analog.op; }  //nl_double *Q_Analog_state_ptr() NL_NOEXCEPT { return m_cur_Analog.ptr(); }

        //FIXME: needed by current solver code
        public devices.matrix_solver_t solver() { return m_solver; }
        public void set_solver(devices.matrix_solver_t solver) { m_solver = solver; }
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


        //void set_Q_time(const netlist_sig_t newQ, const netlist_time &at) NL_NOEXCEPT
        //{
        //    m_my_net.set_Q_time(newQ, at); // take the shortcut
        //}
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


        public void push(nl_double val) { set_Q(val); }
        public void initial(nl_double val) { net().set_Q_Analog(val); }

        void set_Q(nl_double newQ)
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
            device.setup().register_param_t(this.name(), this);
        }

        //COPYASSIGNMOVE(param_t, delete)

        //~param_t() { } /* not intended to be destroyed */


        //param_type_t param_type() const;


        void update_param()
        {
            device().update_param();
        }


        protected string get_initial(device_t dev, out bool found)
        {
            string res = dev.setup().get_initial_param_val(this.name(), "");
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
                bool err = false;
                var vald = plib.pstring_global.pstonum_ne_bool(p, out err);
                if (err)
                    device.state().log().fatal.op(nl_errstr_global.MF_INVALID_NUMBER_CONVERSION_1_2(name, p));
                m_param = vald;
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
                bool err = false;
                var vald = plib.pstring_global.pstonum_ne_int(p, out err);
                if (err)
                    device.state().log().fatal.op(nl_errstr_global.MF_INVALID_NUMBER_CONVERSION_1_2(name, p));
                m_param = vald;
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

    class param_num_t_double : param_t  //param_num_t<T>::param_num_t(device_t &device, const pstring &name, const T val)
    {
        double m_param;

        public param_num_t_double(device_t device, string name, double val)
            : base(device, name)
        {
            //m_param = device.setup().get_initial_param_val(this->name(),val);
            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                bool err = false;
                var vald = plib.pstring_global.pstonum_ne_double(p, out err);
                if (err)
                    device.state().log().fatal.op(nl_errstr_global.MF_INVALID_NUMBER_CONVERSION_1_2(name, p));
                m_param = vald;
            }
            else
            {
                m_param = val;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }

        public double op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }
        public void setTo(double param) { set(ref m_param, param); }
    }

    /* FIXME: these should go as well */
    //using param_logic_t = param_num_t<bool>;
    //using param_int_t = param_num_t<int>;
    //using param_double_t = param_num_t<double>;
    public class param_logic_t : param_num_t_bool { public param_logic_t(device_t device, string name, bool val) : base(device, name, val) { } }
    public class param_int_t : param_num_t_int { public param_int_t(device_t device, string name, int val) : base(device, name, val) { } }
    class param_double_t : param_num_t_double { public param_double_t(device_t device, string name, double val) : base(device, name, val) { } }


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
            m_param = device.setup().get_initial_param_val(this.name(), val);
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
        class value_base_t_nl_double
        {
            nl_double m_value;  //const T m_value;

            public value_base_t_nl_double(param_model_t param, string name)
            {
                m_value = param.value(name);
            }

            //T operator()() const noexcept { return m_value; }
            //operator T() const noexcept { return m_value; }
        }

        //using value_t = value_base_t<nl_double>;
        class value_t : value_base_t_nl_double { public value_t(param_model_t param, string name) : base(param, name) { } }


        //template <typename T>
        //friend class value_base_t;


        public param_model_t(device_t device, string name, string val) : base(device, name, val) { }


        //const pstring value_str(const pstring &entity) /*const*/;

        nl_double value(string entity)
        {
            return state().setup().models().value(str(), entity);
        }

        //const pstring type() /*const*/;


        protected override void changed()
        {
            throw new emu_unimplemented();
        }



        /* hide this */
        //void setTo(const pstring &param) = delete;
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
        //nperftime_t<NL_KEEP_STATISTICS>  m_stat_total_time;
        //nperfcount_t<NL_KEEP_STATISTICS> m_stat_call_count;
        //nperfcount_t<NL_KEEP_STATISTICS> m_stat_inc_active;

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
            //, logic_family_t()
            //, netlist_ref(owner)
        {
            m_netlist_ref = new detail.netlist_ref(owner);


            m_hint_deactivate = false;
            m_active_outputs = new state_var_s32(this, "m_active_outputs", 1);


            if (logic_family() == null)
                set_logic_family(nl_base_global.family_TTL());
        }


        protected void core_device_t_ctor(core_device_t owner, string name)
            //: base(owner.name() + "." + name)
            //, logic_family_t()
            //, netlist_ref(owner.netlist())
        {
            m_netlist_ref = new detail.netlist_ref(owner.state());


            m_hint_deactivate = false;
            m_active_outputs = new state_var_s32(this, "m_active_outputs", 1);


            set_logic_family(owner.logic_family());
            if (logic_family() == null)
                set_logic_family(nl_base_global.family_TTL());

            state().add_dev(this.name(), this);  //state().add_dev(this->name(), poolptr<core_device_t>(this, false));
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
            if (term.delegate_ != null)  //if (!term.delegate_.is_set())
                term.delegate_ = update;  //    term.delegate_.set(core_device_t.update, this);
        }


        protected virtual void update() { }
        public virtual void reset() { }
        protected virtual void inc_active() {  }
        protected virtual void dec_active() {  }


        protected log_type log() { return state().log(); }


        public virtual void timestep(nl_double st) { }
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
        //void create_and_register_subdevice(const pstring &name, std::unique_ptr<C> &dev, const Args&... args)


        protected void register_subalias(string name, detail.core_terminal_t term)
        {
            string alias = this.name() + "." + name;

            // everything already fully qualified
            setup().register_alias_nofqn(alias, term.name());
        }

        //void register_subalias(const pstring &name, const pstring &aliased);


        //protected void connect(string t1, string t2);
        protected void connect(detail.core_terminal_t t1, detail.core_terminal_t t2)
        {
            setup().register_link_fqn(t1.name(), t2.name());
        }


        /* FIXME: this is only used by solver code since matrix solvers are started in
         *        post_start.
         */
        protected void connect_post_start(detail.core_terminal_t t1, detail.core_terminal_t t2)
        {
            if (!setup().connect(t1, t2))
                log().fatal.op(nl_errstr_global.MF_ERROR_CONNECTING_1_TO_2(t1.name(), t2.name()));
        }


        //NETLIB_UPDATEI() { }
        //NETLIB_UPDATE_TERMINALSI() { }
    }


    // -----------------------------------------------------------------------------
    // nld_base_dummy : basis for dummy devices
    // FIXME: this is not the right place to define this
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(base_dummy)
    class nld_base_dummy : device_t
    {
        //NETLIB_CONSTRUCTOR(base_dummy) { }
        //detail.family_setter_t m_famsetter;
        //template <class CLASS>
        public nld_base_dummy(object owner, string name)
            : base(owner, name)
        {
        }
    }


    namespace detail
    {
        // -----------------------------------------------------------------------------
        // queue_t
        // -----------------------------------------------------------------------------

        /* We don't need a thread-safe queue currently. Parallel processing of
         * solvers will update inputs after parallel processing.
         */
        public class queue_t : timed_queue,  //public timed_queue<pqentry_t<net_t *, netlist_time>, false, NL_KEEP_STATISTICS>,
                               //public detail::netlist_ref,
                               plib.state_manager_t.callback_t
        {
            //using entry_t = pqentry_t<net_t *, netlist_time>;


            netlist_ref m_netlist_ref;


            UInt32 m_qsize;  //std::size_t m_qsize;
            std.vector<UInt64> m_times;  //std::vector<netlist_time::internal_type> m_times;
            std.vector<UInt32> m_net_ids;  //std::vector<std::size_t> m_net_ids;


            public queue_t(netlist_state_t nl)
                : base(false, 512)
                //, netlist_ref(nl)
                //, plib::state_manager_t::callback_t()
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
        //using nets_collection_type = std::vector<poolptr<detail::net_t>>;

        /* need to preserve order of device creation ... */
        //using devices_collection_type = std::vector<std::pair<pstring, poolptr<core_device_t>>>;


        string m_name;
        netlist_t m_netlist;
        plib.dynlib m_lib;   //plib::unique_ptr<plib::dynlib>      m_lib; // external lib needs to be loaded as long as netlist exists
        plib.state_manager_t m_state;
        callbacks_t m_callbacks;  //plib::unique_ptr<callbacks_t>       m_callbacks;
        log_type m_log;
        setup_t m_setup;  //plib::unique_ptr<setup_t>           m_setup;

        nets_collection_type m_nets = new nets_collection_type();
        /* sole use is to manage lifetime of net objects */
        devices_collection_type m_devices = new devices_collection_type();

        devices.nld_netlistparams m_params;  //devices::NETLIB_NAME(netlistparams) *m_params;

        /* sole use is to manage lifetime of family objects */
        std.vector<KeyValuePair<string, logic_family_desc_t>> m_family_cache = new std.vector<KeyValuePair<string, logic_family_desc_t>>();  //std::vector<std::pair<pstring, plib::unique_ptr<logic_family_desc_t>>> m_family_cache;


        public netlist_state_t(string aname,
            netlist_t anetlist,
            callbacks_t callbacks)  //plib::unique_ptr<callbacks_t> &&callbacks,
        {
            m_name = aname;
            m_netlist = anetlist;
            m_state = new plib.state_manager_t();
            m_callbacks = callbacks; // Order is important here
            m_log = new log_type(nl_config_global.NL_DEBUG, m_callbacks);
            m_setup = new setup_t(this);  // , m_setup(plib::make_unique<setup_t>(*this))


            string libpath = "";  //pstring libpath = plib::util::environment("NL_BOOSTLIB", plib::util::buildpath({".", "nlboost.so"}));
            m_lib = new plib.dynlib(libpath);
        }


        //COPYASSIGNMOVE(netlist_state_t, delete)
        //~netlist_state_t() { }


        public devices.nld_netlistparams params_ { get { return m_params; } set { m_params = value; } }
        public std.vector<KeyValuePair<string, logic_family_desc_t>> family_cache { get { return m_family_cache; } }


        //template<class C>
        static bool check_class<C>(core_device_t p) where C : core_device_t
        {
            return p is C;  //return dynamic_cast<C *>(p) != nullptr;
        }

        //template<class C>
        public C get_single_device<C>(string classname) where C : core_device_t
        {
            return (C)get_single_device(classname, check_class<C>);  //return dynamic_cast<C *>(get_single_device(classname, check_class<C>));
        }


        /* logging and name */

        //pstring name() const { return m_name; }

        public log_type log() { return m_log; }

        public plib.dynlib lib() { return m_lib; }

        public netlist_t exec() { return m_netlist; }


        /* state handling */

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


        delegate bool get_single_device_func(core_device_t device);

        core_device_t get_single_device(string classname, get_single_device_func cc)  //core_device_t *get_single_device(const pstring &classname, bool (*cc)(core_device_t *)) const;
        {
            core_device_t ret = null;
            foreach (var d in m_devices)
            {
                if (cc(d.second()))
                {
                    if (ret != null)
                        m_log.fatal.op(nl_errstr_global.MF_MORE_THAN_ONE_1_DEVICE_FOUND(classname));
                    else
                        ret = d.second();
                }
            }

            return ret;
        }


        //detail::net_t *find_net(const pstring &name) const;
        //std::size_t find_net_id(const detail::net_t *net) const;


        //template <typename T>
        public void register_net(detail.net_t net) { m_nets.push_back(net); }  //void register_net(poolptr<T> &&net) { m_nets.push_back(std::move(net)); }


        //template<class device_class>
        public std.vector<device_class> get_device_list<device_class>() where device_class : core_device_t
        {
            std.vector<device_class> tmp = new std.vector<device_class>();
            foreach (var d in m_devices)
            {
                var dev = d is device_class ? (device_class)d.second() : null;  //device_class *dev = dynamic_cast<device_class *>(d.get());
                if (dev != null)
                    tmp.push_back(dev);
            }
            return tmp;
        }


        //template <typename T>
        public void add_dev(string name, core_device_t dev)  //void add_dev(const pstring &name, pool_owned_ptr<T> &&dev)
        {
            foreach (var d in m_devices)
            {
                if (d.first() == name)
                {
                    //dev.release();
                    log().fatal.op(nl_errstr_global.MF_DUPLICATE_NAME_DEVICE_LIST(name));
                }
            }

            //m_devices.push_back(dev);
            m_devices.Add(new KeyValuePair<string, core_device_t>(name, dev));  //m_devices.insert(m_devices.end(), { name, std::move(dev) });
        }


        /**
         * @brief Remove device
         *
         * Care needs to be applied if this is called to remove devices with
         * sub-devices which may have registered state.
         *
         * @param dev Device to be removed
         */
        public void remove_dev(core_device_t dev)
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

        public nets_collection_type nets() { return m_nets; }
        public devices_collection_type devices() { return m_devices; }


        // FIXME: make a postload member and include code there
        public void rebuild_lists() /* must be called after post_load ! */
        {
            foreach (var net in m_nets)
                net.rebuild_list();
        }


        //void reset();
    }


    public class netlist_t
    {
        netlist_state_t m_state;  //plib::unique_ptr<netlist_state_t>   m_state;
        devices.nld_solver m_solver;  //devices::NETLIB_NAME(solver) *      m_solver;

        /* mostly rw */
        //PALIGNAS_CACHELINE()
        netlist_time m_time;
        devices.nld_mainclock m_mainclock;  //devices::NETLIB_NAME(mainclock) *   m_mainclock;

        //PALIGNAS_CACHELINE()
        detail.queue_t m_queue;

        // performance
        //nperftime_t<NL_KEEP_STATISTICS>     m_stat_mainloop;
        //nperfcount_t<NL_KEEP_STATISTICS>    m_perf_out_processed;


        public netlist_t(string aname, callbacks_t callbacks)  //explicit netlist_t(const pstring &aname, plib::unique_ptr<callbacks_t> callbacks);
        {
            m_state = new netlist_state_t(aname, this, callbacks);
            m_solver = null;
            m_time = netlist_time.zero();
            m_mainclock = null;
            m_queue = new detail.queue_t(m_state);


            devices.net_lib_global.initialize_factory(nlstate().setup().factory());
            nlm_base_global.netlist_base(nlstate().setup());  //NETLIST_NAME(base)(nlstate().setup());
            run_state_manager().save_item(this, (plib.state_manager_t.callback_t)m_queue, "m_queue");
            run_state_manager().save_item(this, m_time, "m_time");
        }

        //COPYASSIGNMOVE(netlist_t, delete)

        //~netlist_t() { }


        /* run functions */

        public netlist_time time() { return m_time; }


        public void process_queue(netlist_time delta)
        {
            //throw new emu_unimplemented();
#if false
            auto sm_guard(m_stat_mainloop.guard());
#endif

            netlist_time stop = m_time + delta;

            m_queue.push(new entry_t(stop, null));

            if (m_mainclock == null)
            {
                entry_t e = m_queue.pop();
                m_time = e.exec_time;
                while (e.object_ != null)
                {
                    e.object_.update_devs();

                    //throw new emu_unimplemented();
#if false
                    m_perf_out_processed.inc();
#endif

                    e = m_queue.pop();
                    m_time = e.exec_time;
                }
            }
            else
            {
                logic_net_t mc_net = m_mainclock.Q.net();
                netlist_time inc = m_mainclock.inc;
                netlist_time mc_time = mc_net.next_scheduled_time();

                do
                {
                    while (m_queue.top().exec_time > mc_time)
                    {
                        m_time = mc_time;
                        mc_net.toggle_new_Q();
                        mc_net.update_devs();
                        mc_time += inc;
                    }

                    entry_t e = m_queue.pop();
                    m_time = e.exec_time;
                    if (e.object_ != null)
                    {
                        e.object_.update_devs();

                        //throw new emu_unimplemented();
#if false
                        m_perf_out_processed.inc();
#endif
                    }
                    else
                    {
                        break;
                    }
                } while (true); //while (e.m_object != nullptr);

                mc_net.set_next_scheduled_time(mc_time);
            }
        }


        //void abort_current_queue_slice() NL_NOEXCEPT { m_queue.retime(detail::queue_t::entry_t(m_time, nullptr)); }

        public detail.queue_t queue() { return m_queue; }


        /* Control functions */

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

            m_time = netlist_time.zero();
            m_queue.clear();
            if (m_mainclock != null)
                m_mainclock.Q.net().set_next_scheduled_time(netlist_time.zero());
            //if (m_solver != nullptr)
            //  m_solver->reset();

            //m_state.reset();
        }


        /* state handling */

        public plib.state_manager_t run_state_manager() { return m_state.run_state_manager(); }

        /* only used by nltool to create static c-code */
        //devices::NETLIB_NAME(solver) *solver() const NL_NOEXCEPT { return m_solver; }

        /* force late type resolution */
        //template <typename X = devices::NETLIB_NAME(solver)>
        public nl_double gmin(object solv = null)  //nl_double gmin(X *solv = nullptr) const NL_NOEXCEPT
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
    }


    class logic_family_ttl_t : logic_family_desc_t
    {
        logic_family_ttl_t() : base()
        {
            fixed_V = 5.0;
            low_thresh_PCNT = 0.8 / 5.0;
            high_thresh_PCNT = 2.0 / 5.0;
            // m_low_V  - these depend on sinked/sourced current. Values should be suitable for typical applications.
            low_VO = 0.1;
            high_VO = 1.0; // 4.0
            R_low = 1.0;
            R_high = 130.0;
        }


        public override devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_state_t anetlist, string name, logic_output_t proxied)  //poolptr<devices::nld_base_d_to_a_proxy> create_d_a_proxy(netlist_state_t &anetlist, const pstring &name, logic_output_t *proxied) const override;
        {
            return new devices.nld_d_to_a_proxy(anetlist, name, proxied);  //return pool().make_poolptr<devices::nld_d_to_a_proxy>(anetlist, name, proxied);
        }
        public override devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_state_t anetlist, string name, logic_input_t proxied)  //poolptr<devices::nld_base_a_to_d_proxy> create_a_d_proxy(netlist_state_t &anetlist, const pstring &name, logic_input_t *proxied) const override;
        {
            return new devices.nld_a_to_d_proxy(anetlist, name, proxied);  //return pool().make_poolptr<devices::nld_a_to_d_proxy>(anetlist, name, proxied);
        }
    }


    class logic_family_cd4xxx_t : logic_family_desc_t
    {
        logic_family_cd4xxx_t() : base()
        {
            fixed_V = 0.0;
            low_thresh_PCNT = 1.5 / 5.0;
            high_thresh_PCNT = 3.5 / 5.0;
            // m_low_V  - these depend on sinked/sourced current. Values should be suitable for typical applications.
            low_VO = 0.05;
            high_VO = 0.05; // 4.95
            R_low = 10.0;
            R_high = 10.0;
        }


        public override devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_state_t anetlist, string name, logic_output_t proxied)  //poolptr<devices::nld_base_d_to_a_proxy> create_d_a_proxy(netlist_state_t &anetlist, const pstring &name, logic_output_t *proxied) const override;
        {
            return new devices.nld_d_to_a_proxy(anetlist, name, proxied);  //return pool().make_poolptr<devices::nld_d_to_a_proxy>(anetlist, name, proxied);
        }
        public override devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_state_t anetlist, string name, logic_input_t proxied)  //poolptr<devices::nld_base_a_to_d_proxy> create_a_d_proxy(netlist_state_t &anetlist, const pstring &name, logic_input_t *proxied) const override;
        {
            return new devices.nld_a_to_d_proxy(anetlist, name, proxied);  //return pool().make_poolptr<devices::nld_a_to_d_proxy>(anetlist, name, proxied);
        }
    }
}
