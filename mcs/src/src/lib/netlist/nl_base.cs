// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;


namespace mame.netlist
{
    namespace detail
    {
        public interface netlist_name_interface
        {
            string name();  // from object_t, device_t
        }


        public interface netlist_interface
        {
            netlist_state_t state();
        }


        public interface netlist_interface_plus_name : netlist_interface, netlist_name_interface { }
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
}
