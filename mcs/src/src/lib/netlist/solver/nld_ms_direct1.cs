// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    namespace solver
    {
        //template <typename FT>
        class matrix_solver_direct1_t : matrix_solver_direct_t  //class matrix_solver_direct1_t: public matrix_solver_direct_t<FT, 1>
        {
            //typedef FT float_type;
            //typedef matrix_solver_direct_t<FT, 1> base_type;


            public matrix_solver_direct1_t(netlist_state_t anetlist, string name, analog_net_t.list_t nets, solver_parameters_t params_)
                : base(1, anetlist, name, nets, params_, 1)
                {}


            // ----------------------------------------------------------------------------------------
            // matrix_solver - Direct1
            // ----------------------------------------------------------------------------------------
            protected override UInt32 vsolve_non_dynamic(bool newton_raphson)
            {
                this.clear_square_mat(this.m_A);
                this.fill_matrix_and_rhs();

                this.m_new_V[0] = this.m_RHS[0] / this.m_A[0][0];

                bool err = false;
                if (newton_raphson)
                    err = this.check_err();
                this.store();
                return err ? 2u : 1u;
            }
        }
    }
}
