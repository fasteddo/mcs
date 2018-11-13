// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    namespace devices
    {
        class matrix_solver_direct1_t : matrix_solver_direct_t//<1,1>
        {
            public matrix_solver_direct1_t(netlist_t anetlist, string name, solver_parameters_t params_)
                : base(1, 1, anetlist, name, params_, 1)
                {}


            // ----------------------------------------------------------------------------------------
            // matrix_solver - Direct1
            // ----------------------------------------------------------------------------------------
            protected override UInt32 vsolve_non_dynamic(bool newton_raphson)
            {
                throw new emu_unimplemented();
#if false
                build_LE_A<matrix_solver_direct1_t>();
                build_LE_RHS<matrix_solver_direct1_t>();
                //NL_VERBOSE_OUT(("{1} {2}\n", new_val, m_RHS[0] / m_A[0][0]);

                nl_double new_V[1] = { RHS(0) / A(0,0) };

                const nl_double err = (newton_raphson ? delta(new_V) : 0.0);
                store(new_V);
                return (err > this->m_params.m_accuracy) ? 2 : 1;
#endif
            }
        }
    } //namespace devices
} // namespace netlist
