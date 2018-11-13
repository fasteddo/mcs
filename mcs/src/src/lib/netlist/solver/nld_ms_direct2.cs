// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    namespace devices
    {
        class matrix_solver_direct2_t: matrix_solver_direct_t//<2,2>
        {
            public matrix_solver_direct2_t(netlist_t anetlist, string name, solver_parameters_t params_)
                : base(2, 2, anetlist, name, params_, 2)
            { }


            // ----------------------------------------------------------------------------------------
            // matrix_solver - Direct2
            // ----------------------------------------------------------------------------------------
            protected override UInt32 vsolve_non_dynamic(bool newton_raphson)
            {
                throw new emu_unimplemented();
#if false
                build_LE_A<matrix_solver_direct2_t>();
                build_LE_RHS<matrix_solver_direct2_t>();

                const nl_double a = A(0,0);
                const nl_double b = A(0,1);
                const nl_double c = A(1,0);
                const nl_double d = A(1,1);

                nl_double new_V[2];
                new_V[1] = (a * RHS(1) - c * RHS(0)) / (a * d - b * c);
                new_V[0] = (RHS(0) - b * new_V[1]) / a;

                const nl_double err = (newton_raphson ? delta(new_V) : 0.0);
                store(new_V);
                return (err > this->m_params.m_accuracy) ? 2 : 1;
#endif
            }
        }
    } //namespace devices
} // namespace netlist
