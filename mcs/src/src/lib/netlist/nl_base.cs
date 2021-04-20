// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using devices_collection_type = mame.std.vector<mame.std.pair<string, mame.netlist.core_device_t>>;  //using devices_collection_type = std::vector<std::pair<pstring, poolptr<core_device_t>>>;
using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_state_t_family_collection_type = mame.std.unordered_map<string, mame.netlist.logic_family_desc_t>;  //using family_collection_type = std::unordered_map<pstring, host_arena::unique_ptr<logic_family_desc_t>>;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<NL_PREFER_INT128 && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using netlist_state_t_nets_collection_type = mame.std.vector<mame.netlist.detail.net_t>;  //using nets_collection_type = std::vector<poolptr<detail::net_t>>;
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
