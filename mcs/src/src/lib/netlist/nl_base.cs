// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Diagnostics;

using analog_net_t_list_t = mame.std_vector<mame.netlist.analog_net_t>;
using model_map_t = mame.std_unordered_map<string, string>;
using netlist_sig_t = System.UInt32;
using netlist_time = mame.netlist.ptime_u64;  //using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;
using nl_double = System.Double;
using state_var_s32 = mame.netlist.state_var<System.Int32>;


namespace mame.netlist
{
    // ----------------------------------------------------------------------------------------
    // Type definitions
    // ----------------------------------------------------------------------------------------

    /*! netlist_sig_t is the type used for logic signals. */
    //using netlist_sig_t = std::uint32_t;


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

    //#define NETLIB_UPDATE_AFTER_PARAM_CHANGE()                                     \
    //    public: virtual bool needs_update_after_param_change() const override { return true; }

    //#define NETLIB_FAMILY(family) , m_famsetter(*this, family)

    //#define NETLIB_DELEGATE(chip, name) nldelegate(&NETLIB_NAME(chip) :: name, this)

    //#define NETLIB_UPDATE_TERMINALSI() public: virtual void update_terminals() override
    //#define NETLIB_HANDLERI(name) private: virtual void name() NL_NOEXCEPT
    //#define NETLIB_UPDATEI() protected: virtual void update() NL_NOEXCEPT override
    //#define NETLIB_UPDATE_PARAMI() public: virtual void update_param() override
    //#define NETLIB_RESETI() protected: virtual void reset() override

    //#define NETLIB_TIMESTEP(chip) void NETLIB_NAME(chip) :: timestep(const nl_double step)

    //#define NETLIB_SUB(chip) nld_ ## chip
    //#define NETLIB_SUBXX(ns, chip) std::unique_ptr< ns :: nld_ ## chip >

    //#define NETLIB_HANDLER(chip, name) void NETLIB_NAME(chip) :: name(void) NL_NOEXCEPT
    //#define NETLIB_UPDATE(chip) NETLIB_HANDLER(chip, update)

    // FIXME: NETLIB_PARENT_UPDATE should disappear
    //#define NETLIB_PARENT_UPDATE(chip) NETLIB_NAME(chip) :: update();

    //#define NETLIB_RESET(chip) void NETLIB_NAME(chip) :: reset(void)

    //#define NETLIB_UPDATE_PARAM(chip) void NETLIB_NAME(chip) :: update_param(void)
    //#define NETLIB_FUNC_VOID(chip, name, params) void NETLIB_NAME(chip) :: name params

    //#define NETLIB_UPDATE_TERMINALS(chip) void NETLIB_NAME(chip) :: update_terminals(void)


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
        [Conditional("DEBUG")] public static void nl_assert(bool condition) { global.assert(condition); }

        //#define nl_assert_always(x, msg)    do { if (!(x)) throw nl_exception(plib::pfmt("Fatal error: {1}\nCaused by assert: {2}:{3}: {4}")(msg)(__FILE__)(__LINE__)(#x)); } while (0)
        public static void nl_assert_always(bool condition, string message) { global.assert_always(condition, message); }


        static logic_family_ttl_t family_TTL_obj;
        public static logic_family_desc_t family_TTL() { return family_TTL_obj; }  //*!< logic family for TTL devices.

        static logic_family_cd4xxx_t family_CD4XXX_obj;
        public static logic_family_desc_t family_CD4XXX() { return family_CD4XXX_obj; }  //*!< logic family for CD4XXX CMOS devices.
    }


    namespace detail
    {
        public interface netlist_interface
        {
            netlist_t netlist();
        }
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
        ~logic_family_desc_t() { }


        public logic_family_desc_t get() { return this; }  // for smart ptr

        public double fixed_V { get { return m_fixed_V; } set { m_fixed_V = value; } }
        public double low_thresh_PCNT { get { return m_low_thresh_PCNT; } set { m_low_thresh_PCNT = value; } }
        public double high_thresh_PCNT { get { return m_high_thresh_PCNT; } set { m_high_thresh_PCNT = value; } }
        public double low_VO { get { return m_low_VO; } set { m_low_VO = value; } }
        public double high_VO { get { return m_high_VO; } set { m_high_VO = value; } }
        public double R_low { get { return m_R_low; } set { m_R_low = value; } }
        public double R_high { get { return m_R_high; } set { m_R_high = value; } }


        public abstract devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_t anetlist, string name, logic_output_t proxied);  //virtual plib::owned_ptr<devices::nld_base_d_to_a_proxy> create_d_a_proxy(netlist_t &anetlist, const pstring &name, logic_output_t *proxied) const = 0;
        public abstract devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_t anetlist, string name, logic_input_t proxied);  //virtual plib::owned_ptr<devices::nld_base_a_to_d_proxy> create_a_d_proxy(netlist_t &anetlist, const pstring &name, logic_input_t *proxied) const = 0;


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

        void blah<G>() { }

        //template <typename O>
        //! Constructor.
        public state_var(detail.netlist_interface owner,             //!< owner must have a netlist() method.
                         string name,     //!< identifier/name for this state variable
                         T value)         //!< Initial value after construction
        {
            m_value = value;


            owner.netlist().save(owner, m_value, name);
        }


        //! Copy Constructor.
        //constexpr state_var(const state_var &rhs) noexcept = default;
        //! Move Constructor.
        //constexpr state_var(state_var &&rhs) noexcept = default;

        //! Assignment operator to assign value of a state var.
        //C14CONSTEXPR state_var &operator=(const state_var &rhs) noexcept = default;
        //! Assignment move operator to assign value of a state var.
        //C14CONSTEXPR state_var &operator=(state_var &&rhs) noexcept = default;
        //! Assignment operator to assign value of type T.
        //C14CONSTEXPR state_var &operator=(const T &rhs) noexcept { m_value = rhs; return *this; }
        public T op { get { return m_value; } set { m_value = value; } }
        //! Assignment move operator to assign value of type T.
        //C14CONSTEXPR state_var &operator=(T &&rhs) noexcept { std::swap(m_value, rhs); return *this; }
        //! Return value of state variable.
        //C14CONSTEXPR operator T & () noexcept { return m_value; }
        //! Return const value of state variable.
        //constexpr operator const T & () const noexcept { return m_value; }
        //C14CONSTEXPR T * ptr() noexcept { return &m_value; }
        //constexpr const T * ptr() const noexcept{ return &m_value; }
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
        public class object_t
        {
            string m_name;  //std::unique_ptr<pstring> m_name;


            /*! Constructor.
             *
             *  Every class derived from the object_t class must have a name.
             */
            public object_t(string aname /*!< string containing name of the object */)
            {
                m_name = string.Copy(aname);  //(plib::make_unique<pstring>(aname))
            }

            ~object_t() { }


            /*! return name of the object
             *
             *  \returns name of the object.
             */
            public string name() { return m_name; }


            //void * operator new (size_t size, void *ptr) { return ptr; }
            //void operator delete (void *ptr, void *) {  }
            //void * operator new (size_t size);
            //void operator delete (void * mem);
        }


        struct netlist_ref : netlist_interface
        {
            netlist_t m_netlist;


            public netlist_ref(netlist_t nl) { m_netlist = nl; }

            //C14CONSTEXPR netlist_t & netlist() NL_NOEXCEPT { return m_netlist; }
            //constexpr const netlist_t & netlist() const NL_NOEXCEPT { return m_netlist; }
            public netlist_t netlist() { return m_netlist; }
        }


        // -----------------------------------------------------------------------------
        // device_object_t
        // -----------------------------------------------------------------------------

        /*! Base class for all objects being owned by a device.
         *
         * Serves as the base class of all objects being owned by a device.
         *
         */
        public class device_object_t : detail.object_t, netlist_interface
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
            //core_device_t &device() NL_NOEXCEPT { return m_device; }
            //const core_device_t &device() const NL_NOEXCEPT { return m_device; }
            public core_device_t device() { return m_device; }

            /*! The netlist owning the owner of this object.
             * \returns reference to netlist object.
             */
            //netlist_t &netlist() NL_NOEXCEPT;
            //const netlist_t &netlist() const NL_NOEXCEPT;
            public netlist_t netlist() { return m_device.netlist(); }
        }


        /*! Delegate type for device notification.
         *
         */
        //typedef plib::pmfp<void> nldelegate;


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


            const int INP_HL_SHIFT = 0;
            const int INP_LH_SHIFT = 1;
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

            ~core_terminal_t() { }


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
                    netlist().log().fatal.op(nl_errstr_global.MF_1_UNKNOWN_TYPE_FOR_OBJECT, name());
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

            //const net_t & net() const NL_NOEXCEPT { return *m_net;}
            //net_t & net() NL_NOEXCEPT { return *m_net;}
            public net_t net() { return m_net; }


            public bool is_logic() { return this is logic_t; }  //return dynamic_cast<const logic_t *>(this) != nullptr;
            public bool is_analog() { return this is analog_t; }  //return dynamic_cast<const analog_t *>(this) != nullptr;

            //bool is_state(const state_e &astate) const NL_NOEXCEPT { return (m_state == astate); }
            public state_e state() { return m_state.op; }
            void set_state(state_e astate) { m_state.op = astate; }

            public void reset()
            {
                if (is_type(terminal_type.OUTPUT))
                    set_state(state_e.STATE_OUT);
                else
                    set_state(state_e.STATE_INP_ACTIVE);
            }
        }
    }


    /*! Delegate type for device notification.
        *
        */
    //typedef plib::pmfp<void> nldelegate;
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

        ~analog_t() { }


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
        terminal_t m_otherterm;
        ListPointer<nl_double> m_Idr1;  //nl_double *m_Idr1; // drive current
        ListPointer<nl_double> m_go1;  //nl_double *m_go1;  // conductance for Voltage from other term
        ListPointer<nl_double> m_gt1;  //nl_double *m_gt1;  // conductance for total conductance


        // ----------------------------------------------------------------------------------------
        // terminal_t
        // ----------------------------------------------------------------------------------------
        public terminal_t(core_device_t dev, string aname)
            : base(dev, aname, state_e.STATE_BIDIR)
        {
            m_otherterm = null;
            m_Idr1 = null;
            m_go1 = null;
            m_gt1 = null;


            netlist().setup().register_term(this);
        }

        ~terminal_t() { }


        public terminal_t otherterm { get { return m_otherterm; } set { m_otherterm = value; } }


        //nl_double operator ()() const  NL_NOEXCEPT;

        //void set(const nl_double &G) NL_NOEXCEPT
        //{
        //    set(G,G, 0.0);
        //}

        //void set(const nl_double &GO, const nl_double &GT) NL_NOEXCEPT
        //{
        //    set(GO, GT, 0.0);
        //}

        //void set(const nl_double &GO, const nl_double &GT, const nl_double &I) NL_NOEXCEPT
        public void set(nl_double GO, nl_double GT, nl_double I)
        {
            set_ptr(m_Idr1, I);
            set_ptr(m_go1, GO);
            set_ptr(m_gt1, GT);
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


        public void set_ptrs(ListPointer<nl_double> gt, ListPointer<nl_double> go, ListPointer<nl_double> Idr)  //nl_double *gt, nl_double *go, nl_double *Idr)
        {
            m_gt1 = new ListPointer<nl_double>(gt);
            m_go1 = new ListPointer<nl_double>(go);
            m_Idr1 = new ListPointer<nl_double>(Idr);
        }


        //void set_ptr(nl_double *ptr, const nl_double &val) NL_NOEXCEPT
        void set_ptr(ListPointer<nl_double> ptr, nl_double val)
        {
            if (ptr != null && ptr[0] != val)
            {
                ptr[0] = val;  //*ptr = val;
            }
        }
    }


    // -----------------------------------------------------------------------------
    // logic_t
    // -----------------------------------------------------------------------------
    public class logic_t : detail.core_terminal_t,
                           logic_family_t  //, public logic_family_t
    {
        devices.nld_base_proxy m_proxy;


        public logic_t(core_device_t dev, string aname, state_e state, nldelegate delegate_ = null)  // = nldelegate());
            : base(dev, aname, state, delegate_)
            //, logic_family_t()
        {
            m_proxy = null;
        }

        ~logic_t() { }


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
            netlist().setup().register_term(this);
        }

        ~logic_input_t() { }


        //netlist_sig_t operator()() const NL_NOEXCEPT
        //{
        //    return Q();
        //}

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
            netlist().setup().register_term(this);
        }

        /*! Destructor */
        ~analog_input_t() { }


        /*! returns voltage at terminal.
         *  \returns voltage at terminal.
         */
        //nl_double operator()() const NL_NOEXCEPT { return Q_Analog(); }

        /*! returns voltage at terminal.
         *  \returns voltage at terminal.
         */
        //nl_double Q_Analog() const NL_NOEXCEPT;
    }


    namespace detail
    {
        // -----------------------------------------------------------------------------
        // net_t
        // -----------------------------------------------------------------------------
        public class net_t : detail.object_t,
                             detail.netlist_interface  //public detail::netlist_ref
        {
            enum queue_status
            {
                QS_DELAYED_DUE_TO_INACTIVE = 0,
                QS_QUEUED,
                QS_DELIVERED
            }


            netlist_ref m_netlist_ref;


            std_vector<core_terminal_t> m_core_terms = new std_vector<core_terminal_t>(); // save post-start m_list ...

            state_var<netlist_sig_t> m_new_Q;
            state_var<netlist_sig_t> m_cur_Q;
            state_var<queue_status> m_in_queue;    /* 0: not in queue, 1: in queue, 2: last was taken */
            state_var_s32 m_active;

            state_var<netlist_time> m_time;

            std_vector<core_terminal_t> m_list_active = new std_vector<core_terminal_t>();  //plib::linkedlist_t<core_terminal_t> m_list_active;
            core_terminal_t m_railterminal;


            public net_t(netlist_t nl, string aname, core_terminal_t mr = null)
                : base(aname)
                //, netlist_ref(nl)
            {
                m_netlist_ref = new netlist_ref(nl);

                m_new_Q = new state_var<netlist_sig_t>(this, "m_new_Q", 0);
                m_cur_Q = new state_var<netlist_sig_t>(this, "m_cur_Q", 0);
                m_in_queue = new state_var<queue_status>(this, "m_in_queue", queue_status.QS_DELIVERED);
                m_active = new state_var_s32(this, "m_active", 0);
                m_time = new state_var<netlist_time>(this, "m_time", netlist_time.zero());
                m_railterminal = mr;
            }


            ~net_t()
            {
                throw new emu_unimplemented();
#if false
                netlist().state().remove_save_items(this);
#endif
            }


            public std_vector<core_terminal_t> core_terms { get { return m_core_terms; } }
            protected state_var<netlist_sig_t> new_Q { get { return m_new_Q; } set { m_new_Q = value; } }
            protected state_var<netlist_sig_t> cur_Q { get { return m_cur_Q; } set { m_cur_Q = value; } }
            public netlist_t netlist() { return m_netlist_ref.netlist(); }


            public void reset()
            {
                m_time.op = netlist_time.zero();
                m_active.op = 0;
                m_in_queue.op = queue_status.QS_DELIVERED;

                m_new_Q.op = 0;
                m_cur_Q.op = 0;

                analog_net_t p = this is analog_net_t ? (analog_net_t)this : null;

                if (p != null)
                    p.cur_Analog.op.d = 0.0;

                /* rebuild m_list */

                m_list_active.clear();
                foreach (core_terminal_t ct in m_core_terms)
                    m_list_active.push_back(ct);

                foreach (core_terminal_t ct in m_core_terms)
                    ct.reset();

                foreach (core_terminal_t ct in m_core_terms)
                {
                    if (ct.state() != logic_t.state_e.STATE_INP_PASSIVE)
                        m_active.op++;
                }
            }


            //void toggle_new_Q() NL_NOEXCEPT { m_new_Q = (m_cur_Q ^ 1);   }

            //void toggle_and_push_to_queue(const netlist_time &delay) NL_NOEXCEPT
            //{
            //    toggle_new_Q();
            //    push_to_queue(delay);
            //}

            //void push_to_queue(const netlist_time &delay) NL_NOEXCEPT;
            //bool is_queued() const NL_NOEXCEPT { return m_in_queue == QS_QUEUED; }

            //void update_devs() NL_NOEXCEPT;

            //const netlist_time &time() const NL_NOEXCEPT { return m_time; }
            public void set_time(netlist_time ntime) { m_time.op = ntime; }

            public bool isRailNet() { return m_railterminal != null; }
            public core_terminal_t railterminal() { return m_railterminal; }

            public int num_cons() { return m_core_terms.size(); }

            //void inc_active(core_terminal_t &term) NL_NOEXCEPT;
            //void dec_active(core_terminal_t &term) NL_NOEXCEPT;

            /* setup stuff */

            public void add_terminal(core_terminal_t terminal)
            {
                foreach (var t in m_core_terms)
                {
                    if (t == terminal)
                        netlist().log().fatal.op(nl_errstr_global.MF_2_NET_1_DUPLICATE_TERMINAL_2, name(), t.name());
                }

                terminal.set_net(this);

                m_core_terms.push_back(terminal);

                if (terminal.state() != logic_t.state_e.STATE_INP_PASSIVE)
                    m_active.op++;
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
                    netlist().log().fatal.op(nl_errstr_global.MF_2_REMOVE_TERMINAL_1_FROM_NET_2, terminal.name(), this.name());
                }

                if (terminal.state() != logic_t.state_e.STATE_INP_PASSIVE)
                    m_active.op--;
            }


            //bool is_logic() const NL_NOEXCEPT;
            //bool detail::net_t::is_analog() const NL_NOEXCEPT { return dynamic_cast<const analog_net_t *>(this) != nullptr; }
            public bool is_analog() { return this is analog_net_t; }


            public void rebuild_list()     /* rebuild m_list after a load */
            {
                /* rebuild m_list */

                int cnt = 0;
                m_list_active.clear();
                foreach (var term in m_core_terms)
                {
                    if (term.state() != logic_t.state_e.STATE_INP_PASSIVE)
                    {
                        m_list_active.push_back(term);
                        cnt++;
                    }
                }
                m_active.op = cnt;
            }


            public void move_connections(net_t dest_net)
            {
                foreach (var ct in m_core_terms)
                    dest_net.add_terminal(ct);

                m_core_terms.clear();
                m_active.op = 0;
            }


            //void process(unsigned Mask);
        }
    }


    public class logic_net_t : detail.net_t
    {
        public logic_net_t(netlist_t nl, string aname, detail.core_terminal_t mr = null)
            : base(nl, aname, mr)
        {
        }

        ~logic_net_t() { }


        //netlist_sig_t Q() const NL_NOEXCEPT { return m_cur_Q; }
        public void initial(netlist_sig_t val) { cur_Q.op = val;  new_Q.op = val; }

        //void set_Q_and_push(const netlist_sig_t newQ, const netlist_time &delay) NL_NOEXCEPT
        //{
        //    if (newQ != m_new_Q )
        //    {
        //        m_new_Q = newQ;
        //        push_to_queue(delay);
        //    }
        //}

        //void set_Q_and_push_force(const netlist_sig_t newQ, const netlist_time &delay) NL_NOEXCEPT
        //{
        //    if (newQ != m_new_Q || is_queued())
        //    {
        //        m_new_Q = newQ;
        //        push_to_queue(delay);
        //    }
        //}

        //void set_Q_time(const netlist_sig_t newQ, const netlist_time &at) NL_NOEXCEPT
        //{
        //    if (newQ != m_new_Q)
        //    {
        //        m_in_queue = QS_DELAYED_DUE_TO_INACTIVE;
        //        m_time = at;
        //    }
        //    m_cur_Q = m_new_Q = newQ;
        //}

        /* internal state support
         * FIXME: get rid of this and implement export/import in MAME
         */
        //netlist_sig_t *Q_state_ptr() { return m_cur_Q.ptr(); }
    }


    class analog_net_t : detail.net_t
    {
        //using list_t =  std::vector<analog_net_t *>;

        //friend class detail::net_t;


        state_var<doubleref> m_cur_Analog;  //state_var<nl_double>     m_cur_Analog;
        devices.matrix_solver_t m_solver;


        // ----------------------------------------------------------------------------------------
        // analog_net_t
        // ----------------------------------------------------------------------------------------
        public analog_net_t(netlist_t nl, string aname, detail.core_terminal_t mr = null)
            : base(nl, aname, mr)
        {
            m_cur_Analog = new state_var<doubleref>(this, "m_cur_Analog", new doubleref(0.0));
            m_solver = null;
        }

        ~analog_net_t() { }


        public state_var<doubleref> cur_Analog { get { return m_cur_Analog; } set { m_cur_Analog = value; } }


        //nl_double Q_Analog() const NL_NOEXCEPT { return m_cur_Analog; }
        public void set_Q_Analog(nl_double v) { m_cur_Analog.op.d = v; }
        public doubleref Q_Analog_state_ptr() { return m_cur_Analog.op; }  //nl_double *Q_Analog_state_ptr() NL_NOEXCEPT { return m_cur_Analog.ptr(); }

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
            m_my_net = new logic_net_t(dev.netlist(), name() + ".net", this);


            set_net(m_my_net);
            netlist().nets.push_back(m_my_net);  //plib::owned_ptr<logic_net_t>(&m_my_net, false));
            set_logic_family(dev.logic_family());
            netlist().setup().register_term(this);
        }


        ~logic_output_t() { }


        public void initial(netlist_sig_t val)
        {
            if (has_net())
                net().initial(val);
        }


        //void push(const netlist_sig_t newQ, const netlist_time &delay) NL_NOEXCEPT
        //{
        //    m_my_net.set_Q_and_push(newQ, delay); // take the shortcut
        //}

        //void push_force(const netlist_sig_t newQ, const netlist_time &delay) NL_NOEXCEPT
        //{
        //    m_my_net.set_Q_and_push_force(newQ, delay); // take the shortcut
        //}

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
            m_my_net = new analog_net_t(dev.netlist(), name() + ".net", this);


            netlist().nets.push_back(m_my_net);  //plib::owned_ptr<analog_net_t>(&m_my_net, false));
            set_net(m_my_net);

            //net().m_cur_Analog = NL_FCONST(0.0);
            netlist().setup().register_term(this);
        }


        ~analog_output_t() { }


        //void push(const nl_double val) NL_NOEXCEPT { set_Q(val); }

        public void initial(nl_double val)
        {
            net().set_Q_Analog(val);
        }

        //void set_Q(const nl_double newQ) NL_NOEXCEPT;
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
            device.setup().register_param(this.name(), this);
        }

        ~param_t() { } /* not intended to be destroyed */


        //param_type_t param_type() const;


        //void update_param();

        //template<typename C>
        //void set(C &p, const C v)
        //{
        //    if (p != v)
        //    {
        //        p = v;
        //        update_param();
        //    }
        //}
    }


    class param_logic_t : param_t
    {
        bool m_param;


        public param_logic_t(device_t device, string name, bool val)
            : base(device, name)
        {
            m_param = device.setup().get_initial_param_val(this.name(), val ? 1 : 0) != 0;
            netlist().save(this, m_param, "m_param");
        }


        //const bool &operator()() const NL_NOEXCEPT { return m_param; }
        public bool op() { return m_param; }

        //void setTo(const bool &param) { set(m_param, param); }
    }


    class param_int_t : param_t
    {
        int m_param;


        public param_int_t(device_t device, string name, int val)
            : base(device, name)
        {
            m_param = device.setup().get_initial_param_val(this.name(), val);
            netlist().save(this, m_param, "m_param");
        }


        //const int &operator()() const NL_NOEXCEPT { return m_param; }
        public int op() { return m_param; }

        //void setTo(const int &param) { set(m_param, param); }
    }


    class param_double_t : param_t
    {
        double m_param;


        public param_double_t(device_t device, string name, double val)
            : base(device, name)
        {
            m_param = device.setup().get_initial_param_val(this.name(), val);
            netlist().save(this, m_param, "m_param");
        }


        //const double &operator()() const NL_NOEXCEPT { return m_param; }
        public double op() { return m_param; }

        //void setTo(const double &param) { set(m_param, param); }
    }


    class param_str_t : param_t
    {
        string m_param;


        public param_str_t(device_t device, string name, string val)
            : base(device, name)
        {
            m_param = device.setup().get_initial_param_val(this.name(), val);
        }

        ~param_str_t() { }


        //const pstring &operator()() const NL_NOEXCEPT { return Value(); }
        public string op() { return Value(); }

        //void setTo(const pstring &param) NL_NOEXCEPT
        //{
        //    if (m_param != param)
        //    {
        //        m_param = param;
        //        changed();
        //        update_param();
        //    }
        //}

        //virtual void changed();

        protected string Value() { return m_param; }
    }


    class param_model_t : param_str_t
    {
        class value_t
        {
            double m_value;

            value_t(param_model_t param, string name)
            {
                m_value = param.model_value(name);
            }

            //const double &operator()() const NL_NOEXCEPT { return m_value; }
            //operator const double&() const NL_NOEXCEPT { return m_value; }
        }


        //friend class value_t;


        model_map_t m_map;


        public param_model_t(device_t device, string name, string val) : base(device, name, val) { }


        //const pstring model_value_str(const pstring &entity) /*const*/;
        //const pstring model_type() /*const*/;

        //virtual void changed() override;

        nl_double model_value(string entity)
        {
            if (m_map.size() == 0)
                netlist().setup().model_parse(this.Value(), m_map);

            return netlist().setup().model_value(m_map, entity);
        }

        /* hide this */
        //void setTo(const pstring &param) = delete;
    }


    // -----------------------------------------------------------------------------
    // core_device_t
    // -----------------------------------------------------------------------------
    public class core_device_t : detail.object_t,
                                 logic_family_t,  //public logic_family_t,
                                 detail.netlist_interface  //public detail::netlist_ref
    {
        detail.netlist_ref m_netlist_ref;


        /* stats */
        //nperftime_t  m_stat_total_time;
        //nperfcount_t m_stat_call_count;
        //nperfcount_t m_stat_inc_active;

        bool m_hint_deactivate;


        // the only common type between netlist_t and core_devie_t is 'object'.
        // so we accept 'object' as the parameter, and disambiguate below
        protected core_device_t(object owner, string name)
            : base(owner is netlist_t ? name : (owner is core_device_t ? ((core_device_t)owner).name() + "." + name : ""))
        {
            global.assert(owner is netlist_t || owner is core_device_t);

            if (owner is netlist_t)
                core_device_t_ctor((netlist_t)owner, name);
            else if (owner is core_device_t)
                core_device_t_ctor((core_device_t)owner, name);
            else
                throw new emu_fatalerror("unknown type: {0}", owner.GetType());
        }


        protected void core_device_t_ctor(netlist_t owner, string name)
            //: base(name)
            //, logic_family_t()
            //, netlist_ref(owner)
        {
            m_netlist_ref = new detail.netlist_ref(owner);


            m_hint_deactivate = false;


            if (logic_family() == null)
                set_logic_family(nl_base_global.family_TTL());
        }


        protected void core_device_t_ctor(core_device_t owner, string name)
            //: base(owner.name() + "." + name)
            //, logic_family_t()
            //, netlist_ref(owner.netlist())
        {
            m_netlist_ref = new detail.netlist_ref(owner.netlist());


            m_hint_deactivate = false;


            set_logic_family(owner.logic_family());
            if (logic_family() == null)
                set_logic_family(nl_base_global.family_TTL());

            owner.netlist().register_dev(this);  //owner.netlist().register_dev(plib::owned_ptr<core_device_t>(this, false));
        }

        ~core_device_t() { }


        // netlist_interface
        public netlist_t netlist()
        {
            return m_netlist_ref.netlist();
        }


        // logic_family_t
        protected logic_family_desc_t m_logic_family;
        public logic_family_desc_t logic_family() { return m_logic_family; }
        public void set_logic_family(logic_family_desc_t fam) { m_logic_family = fam; }


        public void update_dev() { do_update(); }


        //void do_inc_active() NL_NOEXCEPT
        //{
        //    if (m_hint_deactivate)
        //    {
        //        m_stat_inc_active.inc();
        //        inc_active();
        //    }
        //}

        //void do_dec_active() NL_NOEXCEPT
        //{
        //    if (m_hint_deactivate)
        //        dec_active();
        //}


        public void do_reset() { reset(); }
        public void set_hint_deactivate(bool v) { m_hint_deactivate = v; }
        public bool get_hint_deactivate() { return m_hint_deactivate; }


        public void set_default_delegate(detail.core_terminal_t term)
        {
            if (term.delegate_ != null)  //if (!term.delegate_.is_set())
                term.delegate_ = update;  //    term.delegate_.set(core_device_t.update, this);
        }


        protected virtual void update() { }
        protected virtual void inc_active() {  }
        protected virtual void dec_active() {  }
        protected virtual void reset() { }


        void do_update()
        {
            update();
        }


        protected plib.plog_base<netlist_t> log() { return netlist().log(); }


        protected virtual void timestep(nl_double st) { }
        protected virtual void update_terminals() { }

        public virtual void update_param() {}
        public virtual bool is_dynamic() { return false; }
        public virtual bool is_timestep() { return false; }
        public virtual bool needs_update_after_param_change() { return false; }
    }


    // -----------------------------------------------------------------------------
    // device_t
    // -----------------------------------------------------------------------------
    public class device_t : core_device_t
    {
        //protected device_t(netlist_t owner, string name) : base(owner, name) { }
        //protected device_t(core_device_t owner, string name) : base(owner, name) { }
        protected device_t(object owner, string name) : base(owner, name) { global.assert(owner is netlist_t || owner is core_device_t); }

        ~device_t()
        {
            //log().debug("~net_device_t\n");
        }


        public setup_t setup() { return netlist().setup(); }

        //template<class C, typename... Args>
        //void register_sub(const pstring &name, std::unique_ptr<C> &dev, const Args&... args)
        //{
        //    dev.reset(plib::palloc<C>(*this, name, args...));
        //}


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
                log().fatal.op(nl_errstr_global.MF_2_ERROR_CONNECTING_1_TO_2, t1.name(), t2.name());
        }


        //NETLIB_UPDATEI() { }
        //NETLIB_UPDATE_TERMINALSI() { }
    }


    // -----------------------------------------------------------------------------
    // nld_base_dummy : basis for dummy devices
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
        class queue_t : timed_queue,  //timed_queue<pqentry_t<net_t, netlist_time>, false>,
                        //public detail::netlist_ref,
                        plib.state_manager_t.callback_t
        {
            //typedef pqentry_t<net_t *, netlist_time> entry_t;


            netlist_ref m_netlist_ref;


            UInt32 m_qsize;  //std::size_t m_qsize;
            std_vector<UInt64> m_times;  //std::vector<netlist_time::internal_type> m_times;
            std_vector<UInt32> m_net_ids;  //std::vector<std::size_t> m_net_ids;


            public queue_t(netlist_t nl)
                : base(false, 512)
                //, netlist_ref(nl)
                //, plib::state_manager_t::callback_t()
            {
                m_netlist_ref = new netlist_ref(nl);

                m_qsize = 0;
                m_times = new std_vector<UInt64>(512);
                m_net_ids = new std_vector<UInt32>(512);
            }


            //void register_state(plib::state_manager_t &manager, const pstring &module) override;
            //void on_pre_save() override;
            //void on_post_load() override;
        }
    }


    // -----------------------------------------------------------------------------
    // netlist_t
    // -----------------------------------------------------------------------------
    public abstract class netlist_t  //: private plib::nocopyassignmove
    {
        // FIXME: find something better
        /* sole use is to manage lifetime of net objects */
        std_vector<detail.net_t> m_nets = new std_vector<detail.net_t>();  //std::vector<plib::owned_ptr<detail::net_t>> m_nets;
        /* sole use is to manage lifetime of family objects */
        std_vector<KeyValuePair<string, logic_family_desc_t>> m_family_cache = new std_vector<KeyValuePair<string, logic_family_desc_t>>();  //std::vector<std::pair<pstring, std::unique_ptr<logic_family_desc_t>>> m_family_cache;


        /* mostly rw */
        netlist_time m_time;
        detail.queue_t m_queue;  //detail::queue_t                     m_queue;

        /* mostly ro */

        devices.nld_mainclock m_mainclock;  //devices::NETLIB_NAME(mainclock) *    m_mainclock;
        devices.nld_solver m_solver;  //devices::NETLIB_NAME(solver) *       m_solver;
        devices.nld_netlistparams m_params;  //devices::NETLIB_NAME(netlistparams) *m_params;

        string m_name;
        setup_t m_setup;  //std::unique_ptr<setup_t>            m_setup;
        plib.plog_base<netlist_t> m_log;  //plib::plog_base<netlist_t, NL_DEBUG>           m_log;
        plib.dynlib m_lib;  //std::unique_ptr<plib::dynlib>       m_lib; // external lib needs to be loaded as long as netlist exists

        plib.state_manager_t m_state;  //plib.state_manager_t m_state;

        // performance
        //nperftime_t     m_stat_mainloop;
        //nperfcount_t    m_perf_out_processed;

        std_vector<core_device_t> m_devices = new std_vector<core_device_t>();  //std::vector<plib::owned_ptr<core_device_t>> m_devices;


        public netlist_t(string aname)
        {
            m_time = netlist_time.zero();
            m_queue = new detail.queue_t(this);
            m_mainclock = null;
            m_solver = null;
            m_params = null;
            m_name = aname;
            m_log = new plib.plog_base<netlist_t>(nl_config_global.NL_DEBUG, this);
            m_lib = null;
            m_state = new plib.state_manager_t();


            state().save_item(this, (plib.state_manager_t.callback_t)m_queue, "m_queue");  //state().save_item(this, static_cast<plib::state_manager_t::callback_t &>(m_queue), "m_queue");
            state().save_item(this, m_time, "m_time");
            m_setup = new setup_t(this);


            /* FIXME: doesn't really belong here */
            nlm_base_global.nld_base(m_setup);  //NETLIST_NAME(base)(m_setup);
        }


        ~netlist_t()
        {
            m_nets.clear();
            m_devices.clear();
        }


        public std_vector<detail.net_t> nets { get { return m_nets; } }
        public std_vector<KeyValuePair<string, logic_family_desc_t>> family_cache { get { return m_family_cache; } }


        /* run functions */

        //const netlist_time &time() const NL_NOEXCEPT { return m_time; }
        devices.nld_solver solver() { return m_solver; }

        /* never use this in constructors! */
        public nl_double gmin() { return solver().gmin(); }

        //void process_queue(const netlist_time &delta) NL_NOEXCEPT;
        //void abort_current_queue_slice() NL_NOEXCEPT { m_queue.retime(detail::queue_t::entry_t(m_time, nullptr)); }

        /* Control functions */

        public void start()
        {
            setup().start_devices();

            /* load the library ... */

            /* make sure the solver and parameters are started first! */

            foreach (var e in setup().device_factory)
            {
                if (setup().factory().is_class<devices.nld_solver>(e.second())
                 || setup().factory().is_class<devices.nld_netlistparams>(e.second()))
                {
                    var dev = e.second().Create(this, e.first());  //plib::owned_ptr<device_t>(e.second->Create(*this, e.first));
                    register_dev(dev);  //register_dev(std::move(dev));
                }
            }

            log().debug.op("Searching for mainclock and solver ...\n");

            m_solver = get_single_device<devices.nld_solver>("solver");
            m_params = get_single_device<devices.nld_netlistparams>("parameter");

            /* create devices */

            log().debug.op("Creating devices ...\n");
            foreach (var e in setup().device_factory)
            {
                if (!setup().factory().is_class<devices.nld_solver>(e.second())
                 && !setup().factory().is_class<devices.nld_netlistparams>(e.second()))
                {
                    var dev = e.second().Create(this, e.first());  //plib::owned_ptr<device_t>(e.second->Create(*this, e.first));
                    register_dev(dev);  //register_dev(std::move(dev));
                }
            }

            log().debug.op("Searching for mainclock\n");
            m_mainclock = get_single_device<devices.nld_mainclock>("mainclock");

            bool use_deactivate = m_params.use_deactivate.op() ? true : false;

            foreach (var d in m_devices)
            {
                if (use_deactivate)
                {
                    var p = setup().param_values.find(d.name() + ".HINT_NO_DEACTIVATE");
                    if (p != null)
                    {
                        //FIXME: turn this into a proper function
                        //bool error;
                        var v = Convert.ToDouble(p);  //auto v = p->second.as_double(&error);
                        if (Math.Abs(v - Math.Floor(v)) > 1e-6)  //if (error || std::abs(v - std::floor(v)) > 1e-6)
                            log().fatal.op(nl_errstr_global.MF_1_HND_VAL_NOT_SUPPORTED, p);
                        d.set_hint_deactivate(v == 0.0);
                    }
                }
                else
                {
                    d.set_hint_deactivate(false);
                }
            }

            string libpath = "";  //plib::util::environment("NL_BOOSTLIB", plib::util::buildpath({".", "nlboost.so"}));
            m_lib = new plib.dynlib(libpath);  //plib::make_unique<plib::dynlib>(libpath);

            /* resolve inputs */
            setup().resolve_inputs();

            log().verbose.op("looking for two terms connected to rail nets ...");
            foreach (var t in get_device_list<analog.nld_twoterm>())
            {
                if (t.N.net().isRailNet() && t.P.net().isRailNet())
                {
                    log().warning.op(nl_errstr_global.MW_3_REMOVE_DEVICE_1_CONNECTED_ONLY_TO_RAILS_2_3, t.name(), t.N.net().name(), t.P.net().name());
                    t.N.net().remove_terminal(t.N);
                    t.P.net().remove_terminal(t.P);
                    remove_dev(t);
                }
            }

            log().verbose.op("initialize solver ...\n");

            if (m_solver == null)
            {
                foreach (var p in m_nets)
                {
                    if (p.is_analog())
                        log().fatal.op(nl_errstr_global.MF_0_NO_SOLVER);
                }
            }
            else
            {
                m_solver.post_start();
            }

            foreach (var n in m_nets)
            {
                foreach (var term in n.core_terms)
                {
                    //core_device_t *dev = reinterpret_cast<core_device_t *>(term->m_delegate.object());
                    core_device_t dev = term.device();
                    dev.set_default_delegate(term);
                }
            }
        }


        //void stop();


        public void reset()
        {
            m_time = netlist_time.zero();
            m_queue.clear();
            if (m_mainclock != null)
                m_mainclock.Q.net().set_time(netlist_time.zero());
            //if (m_solver != nullptr)
            //  m_solver->do_reset();

            std_unordered_map<core_device_t, bool> m = new std_unordered_map<core_device_t, bool>();

            foreach (var d in m_devices)
            {
                m[d] = d.get_hint_deactivate();
            }


            // Reset all nets once !
            foreach (var n in m_nets)
                n.reset();

            // Reset all devices once !
            foreach (var dev in m_devices)
                dev.do_reset();

            // Make sure everything depending on parameters is set
            foreach (var dev in m_devices)
                dev.update_param();

            // Step all devices once !
            /*
             * INFO: The order here affects power up of e.g. breakout. However, such
             * variations are explicitly stated in the breakout manual.
             */

            UInt32 startup_strategy = 1; //! \note make this a parameter

            switch (startup_strategy)
            {
                case 0:
                {
                    std_vector<core_device_t> d = new std_vector<core_device_t>();
                    std_vector<nldelegate> t = new std_vector<nldelegate>();

                    log().verbose.op("Using default startup strategy");

                    foreach (var n in m_nets)
                    {
                        foreach (var term in n.core_terms)
                        {
                            if (term.delegate_ != null)  //has_object())
                            {
                                if (!t.Contains(term.delegate_))  //if (!plib::container::contains(t, &term->m_delegate))
                                {
                                    t.push_back(term.delegate_);
                                    term.delegate_();
                                }

                                core_device_t dev = (core_device_t)(term.device());  //core_device_t dev = reinterpret_cast<core_device_t *>(term->m_delegate.object_());
                                if (!d.Contains(dev))  //if (!plib::container::contains(d, dev))
                                    d.push_back(dev);
                            }
                        }
                    }

                    log().verbose.op("Call update on devices which need parameter update:");
                    foreach (var dev in m_devices)
                    {
                        if (dev.needs_update_after_param_change())
                        {
                            if (!d.Contains(dev))  //if (!plib::container::contains(d, dev.get()))
                            {
                                d.push_back(dev);
                                log().verbose.op("\t ...{0}", dev.name());
                                dev.update_dev();
                            }
                        }
                    }

                    log().verbose.op("Devices not yet updated:");

                    foreach (var dev in m_devices)
                    {
                        if (!d.Contains(dev))  //if (!plib::container::contains(d, dev.get()))
                            log().verbose.op("\t ...{0}", dev.name());
                            //x->update_dev();
                    }
                }
                break;

                case 1:     // brute force backward
                {
                    int i = m_devices.size();
                    while (i > 0)
                        m_devices[--i].update_dev();
                }
                break;

                case 2:     // brute force forward
                {
                    foreach (var d in m_devices)
                        d.update_dev();
                }
                break;
            }

#if true
            /* the above may screw up m_active and the list */
            foreach (var n in m_nets)
                n.rebuild_list();
#endif
        }


        //const detail::queue_t &queue() const NL_NOEXCEPT { return m_queue; }
        //detail::queue_t &queue() NL_NOEXCEPT { return m_queue; }

        /* netlist build functions */

        public setup_t setup() { return m_setup; }


        public void register_dev(core_device_t dev)
        {
            foreach (var d in m_devices)
            {
                if (d.name() == dev.name())
                    log().fatal.op(nl_errstr_global.MF_1_DUPLICATE_NAME_DEVICE_LIST, d.name());
            }
            m_devices.push_back(dev);
        }


        void remove_dev(core_device_t dev)
        {
            //m_devices.erase(
            //    std::remove_if(
            //        m_devices.begin(),
            //        m_devices.end(),
            //        [&] (plib::owned_ptr<core_device_t> const& p)
            //        {
            //            return p.get() == dev;
            //        }),
            //        m_devices.end()
            //    );
            m_devices.erase(m_devices.IndexOf(dev));
        }


        //detail::net_t *find_net(const pstring &name) const;
        //std::size_t find_net_id(const detail::net_t *net) const;

        //template<class device_class>
        public std_vector<device_class> get_device_list<device_class>() where device_class : core_device_t
        {
            std_vector<device_class> tmp = new std_vector<device_class>();
            foreach (var d in m_devices)
            {
                device_class dev = d is device_class ? (device_class)d : null;  //device_class *dev = dynamic_cast<device_class *>(d.get());
                if (dev != null)
                    tmp.push_back(dev);
            }
            return tmp;
        }


        delegate bool check_class_func(core_device_t p);

        //template<class C>
        //static bool check_class(core_device_t *p) { return dynamic_cast<C *>(p) != nullptr; }
        static bool check_class<C>(core_device_t p) { return p is C; }

        //template<class C>
        //C *get_single_device(const pstring &classname) const { return dynamic_cast<C *>(get_single_device(classname, check_class<C>)); }
        C get_single_device<C>(string classname) where C : core_device_t { return (C)get_single_device(classname, check_class<C>); }

        /* logging and name */

        //pstring name() const { return m_name; }

        //plib::plog_base<netlist_t, NL_DEBUG> &log() { return m_log; }
        //const plib::plog_base<netlist_t, NL_DEBUG> &log() const { return m_log; }
        public plib.plog_base<netlist_t> log() { return m_log; }


        /* state related */

        plib.state_manager_t state() { return m_state; }


        //template<typename O, typename C>
        public void save<O, C>(O owner, C state, string stname)
        {
            //throw new emu_unimplemented();
#if false
            state().save_item(static_cast<void *>(&owner), state, from_utf8(owner.name()) + pstring(".") + stname);
#endif
        }

        //template<typename O, typename C>
        public void save<O, C>(O owner, C state, string stname, UInt32 count)
        {
            //throw new emu_unimplemented();
#if false
            state().save_state_ptr(static_cast<void *>(&owner), from_utf8(owner.name()) + pstring(".") + stname, plib::state_manager_t::datatype_f<C>::f(), count, state);
#endif
        }


        public plib.dynlib lib() { return m_lib; }


        // FIXME: sort rebuild_lists out
        //void rebuild_lists(); /* must be called after post_load ! */

        /* logging callback */
        public abstract void vlog(plib.plog_level l, string ls);

        //void print_stats() const;

        /* helper for save above */
        //static pstring from_utf8(const char *c) { return pstring(c, pstring::UTF8); }
        //static pstring from_utf8(const pstring &c) { return c; }

        //core_device_t *get_single_device(const pstring &classname, bool (*cc)(core_device_t *)) const;
        core_device_t get_single_device(string classname, check_class_func cc)
        {
            core_device_t ret = null;
            foreach (var d in m_devices)
            {
                if (cc(d))
                {
                    if (ret != null)
                        log().fatal.op(nl_errstr_global.MF_1_MORE_THAN_ONE_1_DEVICE_FOUND, classname);
                    else
                        ret = d;
                }
            }
            return ret;
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


        public override devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_t anetlist, string name, logic_output_t proxied)
        {
            return new devices.nld_d_to_a_proxy(anetlist, name, proxied);  //return plib::owned_ptr<devices::nld_base_d_to_a_proxy>::Create<devices::nld_d_to_a_proxy>(anetlist, name, proxied);
        }
        public override devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_t anetlist, string name, logic_input_t proxied)
        {
            return new devices.nld_a_to_d_proxy(anetlist, name, proxied);  //return plib::owned_ptr<devices::nld_base_a_to_d_proxy>::Create<devices::nld_a_to_d_proxy>(anetlist, name, proxied);
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


        public override devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_t anetlist, string name, logic_output_t proxied)
        {
            return new devices.nld_d_to_a_proxy(anetlist, name, proxied);  //return plib::owned_ptr<devices::nld_base_d_to_a_proxy>::Create<devices::nld_d_to_a_proxy>(anetlist, name, proxied);
        }
        public override devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_t anetlist, string name, logic_input_t proxied)
        {
            return new devices.nld_a_to_d_proxy(anetlist, name, proxied);  //return plib::owned_ptr<devices::nld_base_a_to_d_proxy>::Create<devices::nld_a_to_d_proxy>(anetlist, name, proxied);
        }
    }
}
