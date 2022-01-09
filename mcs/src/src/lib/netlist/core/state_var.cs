// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using state_var_u8 = mame.netlist.state_var<System.Byte>;  //using state_var_u8 = state_var<std::uint8_t>;
using state_var_s32 = mame.netlist.state_var<System.Int32>;  //using state_var_s32 = state_var<std::int32_t>;


namespace mame.netlist
{
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
        public state_var(detail.netlist_interface_plus_name owner,             //!< owner must have a netlist() method.
                         string name,         //!< identifier/name for this state variable
                         T value)             //!< Initial value after construction
        {
            m_value = value;


            owner.state().save(owner, m_value, owner.name(), name);
        }


        //template <typename O>
        //! Constructor.
        public state_var(detail.netlist_interface_plus_name owner,             //!< owner must have a netlist() method.
                         string name)         //!< identifier/name for this state variable
        {
            owner.state().save(owner, m_value, owner.name(), name);
        }


        //PMOVEASSIGN(state_var, delete)

        //! Destructor.
        //~state_var() noexcept = default;

        //! Copy Constructor removed.
        //constexpr state_var(const state_var &rhs) = delete;
        //! Assignment operator to assign value of a state var.
        //constexpr state_var &operator=(const state_var &rhs) noexcept
        //{
        //    if (this != &rhs)
        //        m_value = rhs.m_value;
        //    return *this;
        //} // OSX doesn't like noexcept
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


    /// \brief A persistent array template.
    ///  Use this state_var template to define an array whose contents are saved.
    ///  Please refer to \ref state_var.
    ///
    ///  \tparam C container class to use.
    //template <typename C>
    //struct state_container : public C


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
} // namespace netlist


namespace plib
{
    //template<typename X>
    //struct ptype_traits<netlist::state_var<X>> : ptype_traits<X>
} // namespace plib
