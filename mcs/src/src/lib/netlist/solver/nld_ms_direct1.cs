// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;


namespace mame.netlist
{
    namespace solver
    {
        //template <typename FT>
        abstract class matrix_solver_direct1_t : matrix_solver_direct_t_nl_fptype  //class matrix_solver_direct1_t: public matrix_solver_direct_t<FT, 1>
        {
            //typedef FT float_type;
            //typedef matrix_solver_direct_t<FT, 1> base_type;


            public matrix_solver_direct1_t(netlist_state_t anetlist, string name, analog_net_t_list_t nets, solver_parameters_t params_)
                : base(1, anetlist, name, nets, params_, 1)
                {}


            // ----------------------------------------------------------------------------------------
            // matrix_solver - Direct1
            // ----------------------------------------------------------------------------------------
            protected override void vsolve_non_dynamic()
            {
                this.clear_square_mat(this.m_A);
                this.fill_matrix_and_rhs();

                this.m_new_V[0] = this.m_RHS[0] / this.m_A[0][0];
            }
        }
    }
}
