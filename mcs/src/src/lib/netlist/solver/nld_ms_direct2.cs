// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using matrix_solver_t_net_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;  //using net_list_t =  plib::aligned_vector<analog_net_t *>;


namespace mame.netlist
{
    namespace solver
    {
        // ----------------------------------------------------------------------------------------
        // matrix_solver - Direct2
        // ----------------------------------------------------------------------------------------

        //template <typename FT>
        abstract class matrix_solver_direct2_t<FT, FT_OPS> : matrix_solver_direct_t<FT, FT_OPS, int_constant_2>  //class matrix_solver_direct2_t: public matrix_solver_direct_t<FT, 2>
            where FT_OPS : plib.constants_operators<FT>, new()
        {
            //typedef FT float_type;


            matrix_solver_direct2_t(devices.nld_solver main_solver, string name, matrix_solver_t_net_list_t nets, solver.solver_parameters_t params_)
                : base(main_solver, name, nets, params_, 2)
            { }


            // ----------------------------------------------------------------------------------------
            // matrix_solver - Direct2
            // ----------------------------------------------------------------------------------------
            protected override void vsolve_non_dynamic()
            {
                throw new emu_unimplemented();
            }
        }
    }
}
