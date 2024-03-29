// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using matrix_solver_t_net_list_t = mame.std.vector<mame.netlist.analog_net_t>;  //using net_list_t =  std::vector<analog_net_t *>;


namespace mame.netlist.solver
{
    //template <typename FT>
    class matrix_solver_direct1_t<FT, FT_OPS> : matrix_solver_direct_t<FT, FT_OPS, int_const_1>  //class matrix_solver_direct1_t: public matrix_solver_direct_t<FT, 1>
        where FT_OPS : plib.constants_operators<FT>, new()
    {
        //typedef FT float_type;
        //typedef matrix_solver_direct_t<FT, 1> base_type;


        public matrix_solver_direct1_t(devices.nld_solver main_solver, string name, matrix_solver_t_net_list_t nets, solver.solver_parameters_t params_)
            : base(main_solver, name, nets, params_, 1)
        { }


        // ----------------------------------------------------------------------------------------
        // matrix_solver - Direct1
        // ----------------------------------------------------------------------------------------
        protected override void upstream_solve_non_dynamic()
        {
            this.clear_square_mat(this.m_A);
            this.fill_matrix_and_rhs();

            this.m_new_V[0] = ops.divide(this.m_RHS[0], this.m_A[0][0]);  //this->m_new_V[0] = this->m_RHS[0] / this->m_A[0][0];
        }
    }
}
