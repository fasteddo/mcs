// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;


namespace mame.netlist
{
    namespace solver
    {
        // ----------------------------------------------------------------------------------------
        // matrix_solver - Direct2
        // ----------------------------------------------------------------------------------------

        //template <typename FT>
        class matrix_solver_direct2_t : matrix_solver_direct_t_nl_fptype  //class matrix_solver_direct2_t: public matrix_solver_direct_t<FT, 2>
        {
            //typedef FT float_type;

            public matrix_solver_direct2_t(netlist_state_t anetlist, string name, analog_net_t_list_t nets, solver_parameters_t params_)
                : base(2, anetlist, name, nets, params_, 2)
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
