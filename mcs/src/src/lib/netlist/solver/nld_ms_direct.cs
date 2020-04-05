// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.std.vector<mame.netlist.analog_net_t>;
using netlist_base_t = mame.netlist.netlist_state_t;


namespace mame.netlist
{
    namespace devices
    {
        //template <typename FT, int SIZE>
        class matrix_solver_direct_t: matrix_solver_t
        {
            //friend class matrix_solver_t;


            //typedef FT float_type;


            protected matrix_solver_direct_t(UInt32 SIZE, netlist_state_t anetlist, string name, solver_parameters_t params_, UInt32 size)
                : base(null, null, eSortType.NOSORT, params_)
            {
                throw new emu_unimplemented();
            }

            protected matrix_solver_direct_t(UInt32 SIZE, netlist_state_t anetlist, string name, eSortType sort, solver_parameters_t params_, UInt32 size)
                : base(null, null, eSortType.NOSORT, params_)
            {
                throw new emu_unimplemented();
            }


            protected override void vsetup(analog_net_t_list_t nets)
            {
                throw new emu_unimplemented();
            }


            public override void reset()
            {
                throw new emu_unimplemented();
#if false
                matrix_solver_t::reset();
#endif
            }


            protected override UInt32 vsolve_non_dynamic(bool newton_raphson)
            {
                throw new emu_unimplemented();
            }


            //unsigned solve_non_dynamic(const bool newton_raphson);

            //constexpr std::size_t size() const { return (m_N == 0) ? m_dim : m_N; }

            //void LE_solve();

            //template <typename T>
            //void LE_back_subst(T * RESTRICT x);
        }
    } //namespace devices
} // namespace netlist
