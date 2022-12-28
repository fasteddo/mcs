// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using object_t_props = mame.netlist.detail.property_store_t<mame.netlist.detail.object_t, string>;  //using props = property_store_t<object_t, pstring>;
using state_var_s32 = mame.netlist.state_var<System.Int32>;  //using state_var_s32 = state_var<std::int32_t>;
using state_var_sig = mame.netlist.state_var<System.UInt32>;  //using state_var_sig = state_var<netlist_sig_t>;  //using netlist_sig_t = std::uint32_t;
using unsigned = System.UInt32;

using static mame.netlist.nl_errstr_global;


namespace mame.netlist.detail
{
    //template <typename C, typename T>
    static class property_store_t<C, T> where T : class
    {
        //using value_type = T;
        //using key_type = const C *;
        //using store_type = std::unordered_map<key_type, value_type>;


        public static void add(C obj, T value)  //static void add(key_type obj, const value_type &value) noexcept
        {
            try
            {
                store().insert(obj, value);
            }
            catch (Exception)
            {
                plib.pg.terminate("exception in property_store_t.add()");
            }
        }


        public static T get(C obj)  //static const value_type &get(key_type obj) noexcept
        {
            try
            {
                var ret = store().find(obj);  //typename store_type::iterator ret(store().find(obj));
                if (ret == default)
                    plib.pg.terminate("object not found in property_store_t.get()");
                return ret;
            }
            catch (Exception)
            {
                plib.pg.terminate("exception in property_store_t.get()");
                return default;
            }
        }


        //static void remove(key_type obj) noexcept
        //{
        //    try
        //    {
        //        store().erase(store().find(obj));
        //    }
        //    catch (...)
        //    {
        //        plib::terminate("exception in property_store_t.remove()");
        //    }
        //}


        static std.unordered_map<C, T> static_store = new std.unordered_map<C, T>();

        static std.unordered_map<C, T> store()
        {
            return static_store;
        }
    }


    // -----------------------------------------------------------------------------
    // object_t
    // -----------------------------------------------------------------------------
    /// \brief The base class for netlist devices, terminals and parameters.
    ///
    ///  This class serves as the base class for all device, terminal and
    ///  objects.
    public class object_t
    {
        /// \brief Constructor.
        /// Every class derived from the object_t class must have a name.
        ///
        /// \param aname string containing name of the object
        protected object_t(string aname) { object_t_props.add(this, aname); }


        //PCOPYASSIGNMOVE(object_t, delete)

        /// \brief return name of the object
        ///
        /// \returns name of the object.
        public string name() { return object_t_props.get(this); }


        //using props = property_store_t<object_t, pstring>;

        // only childs should be destructible
        //~object_t() noexcept { props::remove(this); }
    }


    /// \brief Base class for all objects being owned by a netlist
    ///
    /// The object provides adds \ref netlist_state_t and \ref netlist_t
    /// accessors.
    ///
    public class netlist_object_t : object_t, netlist_interface_plus_name
    {
        netlist_t m_netlist;


        protected netlist_object_t(netlist_t nl, string name)
            : base(name)
        {
            m_netlist = nl;
        }

        //~netlist_object_t() = default;

        //PCOPYASSIGNMOVE(netlist_object_t, delete)

        public netlist_state_t state() { return m_netlist.nl_state(); }
        //const netlist_state_t & state() const noexcept;

        public netlist_t exec() { return m_netlist; }
        //const netlist_t & exec() const noexcept { return m_netlist; }

        // to ease template design
        //template<typename T, typename... Args>
        //device_arena::unique_ptr<T> make_pool_object(Args&&... args)
        //{
        //    return state().make_pool_object<T>(std::forward<Args>(args)...);
        //}
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
    public class device_object_t : object_t, netlist_interface_plus_name
    {
        core_device_t m_device;


        /// \brief Constructor.
        ///
        /// \param dev  pointer to device owning the object.
        /// \param name string holding the name of the device
        protected device_object_t(core_device_t dev, string aname)
            : base(aname)
        {
            m_device = dev;
        }


        /// \brief returns reference to owning device.
        /// \returns reference to owning device.
        public core_device_t device() { return m_device; }
        //const core_device_t &device() const noexcept { return *m_device; }

        /// \brief The netlist owning the owner of this object.
        /// \returns reference to netlist object.
        public netlist_state_t state() { return m_device.state(); }
        //const netlist_state_t &state() const noexcept;
    }


    // -----------------------------------------------------------------------------
    // core_terminal_t
    // -----------------------------------------------------------------------------
    /// \brief Base class for all terminals.
    ///
    /// All terminals are derived from this class.
    ///
    public class core_terminal_t : device_object_t
                                   //, public plib::linked_list_t<core_terminal_t, 0>::element_t
#if NL_USE_INPLACE_CORE_TERMS
                                   , public plib::linked_list_t<core_terminal_t, 1>::element_t
#endif
    {
        /// \brief Number of signal bits
        ///
        /// Going forward setting this to 8 will allow 8-bit signal
        /// buses to be used in netlist, e.g. for more complex memory
        /// arrangements.
        /// Minimum value is 2 here to support tristate output on proxies.
        const unsigned INP_BITS = 2;

        const unsigned INP_MASK = (1 << (int)INP_BITS) - 1;
        public const unsigned INP_HL_SHIFT = 0;
        public const unsigned INP_LH_SHIFT = INP_BITS;

        public static netlist_sig_t OUT_TRISTATE() { return INP_MASK; }

        //static_assert(INP_BITS * 2 <= sizeof(netlist_sig_t) * 8, "netlist_sig_t size not sufficient");


        public enum state_e : unsigned
        {
            STATE_INP_PASSIVE = 0,
            STATE_INP_HL      = INP_MASK << (int)INP_HL_SHIFT,
            STATE_INP_LH      = INP_MASK << (int)INP_LH_SHIFT,
            STATE_INP_ACTIVE  = STATE_INP_HL | STATE_INP_LH,
            STATE_OUT         = 1 << (int)(2 * INP_BITS),
            STATE_BIDIR       = 1 << (int)(2 * INP_BITS + 1)
        }


        //// std::conditional_t<config::use_copy_instead_of_reference::value,
        //// state_var_sig, void *> m_Q;
        protected state_var_sig m_Q_CIR;

        nl_delegate m_delegate;
        core_device_t m_delegate_device;
        net_t m_net;
        state_var<state_e> m_state;


        protected core_terminal_t(core_device_t dev, string aname, state_e state, nl_delegate delegate_)
            : base(dev, dev.name() + "." + aname)
        {
            m_Q_CIR = new state_var_sig(this, "m_Q", 0);
            m_delegate = delegate_;
            m_net = null;
            m_state = new state_var<state_e>(this, "m_state", state);
        }

        //virtual ~core_terminal_t() noexcept = default;

        //PCOPYASSIGNMOVE(core_terminal_t, delete)


        /// \brief The object type.
        /// \returns type of the object
        public terminal_type type()
        {
            if (this is terminal_t)  //if (dynamic_cast<const terminal_t *>(this) != nullptr)
                return terminal_type.TERMINAL;
            if (this is logic_input_t  //if (dynamic_cast<const logic_input_t *>(this) != nullptr
                || this is analog_input_t)  //|| dynamic_cast<const analog_input_t *>(this) != nullptr)
                return terminal_type.INPUT;
            if (this is logic_output_t  //if (dynamic_cast<const logic_output_t *>(this) != nullptr
                || this is analog_output_t)  //|| dynamic_cast<const analog_output_t *>(this) != nullptr)
                return terminal_type.OUTPUT;

            state().log().fatal.op(MF_UNKNOWN_TYPE_FOR_OBJECT(name()));
            throw new nl_exception(MF_UNKNOWN_TYPE_FOR_OBJECT(name()));
            //return terminal_type::TERMINAL; // please compiler
        }

        /// \brief Checks if object is of specified type.
        /// \param atype type to check object against.
        /// \returns true if object is of specified type else false.
        public bool is_type(terminal_type atype)
        {
            return type() == atype;
        }

        public void set_net(net_t anet) { m_net = anet; }
        public void clear_net() { m_net = null; }
        public bool has_net() { return m_net != null; }

        public net_t net() { return m_net;}

        public bool is_logic() { return this is logic_t; }  //return dynamic_cast<const logic_t *>(this) != nullptr;
        public bool is_logic_input() { return this is logic_input_t; }  //return dynamic_cast<const logic_input_t *>(this) != nullptr;
        public bool is_logic_output() { return this is logic_output_t; }  //return dynamic_cast<const logic_output_t *>(this) != nullptr;
        public bool is_tristate_output() { return this is tristate_output_t; }  //return dynamic_cast<const tristate_output_t *>(this) != nullptr;
        public bool is_analog() { return this is analog_t; }  //return dynamic_cast<const analog_t *>(this) != nullptr;
        //bool is_analog_input() const noexcept;
        public bool is_analog_output() { return this is analog_output_t; }  //return dynamic_cast<const analog_output_t *>(this) != nullptr;

        protected bool is_state(state_e astate)
        {
            return m_state.op == astate;
        }

        public state_e terminal_state() { return m_state.op; }
        public void set_state(state_e state) { m_state.op = state; }

        public void reset() { set_state(is_type(terminal_type.OUTPUT) ? state_e.STATE_OUT : state_e.STATE_INP_ACTIVE); }

        public void set_copied_input(netlist_sig_t val)
        {
            if (config.use_copy_instead_of_reference)
            {
                m_Q_CIR.op = val;
            }
        }

        public void set_delegate(nl_delegate delegate_, core_device_t device)
        {
            m_delegate = delegate_;
            m_delegate_device = device;
        }

        public nl_delegate delegate_() { return m_delegate; }
        public core_device_t delegate_device() { return m_delegate_device; }
        public void run_delegate() { m_delegate(); }
    }
}
