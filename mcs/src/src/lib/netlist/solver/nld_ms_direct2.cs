// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_base_t = mame.netlist.netlist_state_t;


namespace mame.netlist
{
    namespace devices
    {
        // ----------------------------------------------------------------------------------------
        // matrix_solver - Direct2
        // ----------------------------------------------------------------------------------------

        //template <typename FT>
        class matrix_solver_direct2_t: matrix_solver_direct_t//<FT, 2>
        {
            //typedef FT float_type;

            public matrix_solver_direct2_t(netlist_state_t anetlist, string name, solver_parameters_t params_)
                : base(2, anetlist, name, params_, 2)
            { }


            // ----------------------------------------------------------------------------------------
            // matrix_solver - Direct2
            // ----------------------------------------------------------------------------------------
            protected override UInt32 vsolve_non_dynamic(bool newton_raphson)
            {
                throw new emu_unimplemented();
            }
        }
    } //namespace devices
} // namespace netlist
