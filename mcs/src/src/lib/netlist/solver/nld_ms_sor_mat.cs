// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using matrix_solver_t_net_list_t = mame.std.vector<mame.netlist.analog_net_t>;  //using net_list_t =  std::vector<analog_net_t *>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using size_t = System.UInt64;
using unsigned = System.UInt32;


namespace mame.netlist.solver
{
    //template <typename FT, int SIZE>
    class matrix_solver_SOR_mat_t<FT, FT_OPS, int_SIZE> : matrix_solver_direct_t<FT, FT_OPS, int_SIZE>  //class matrix_solver_SOR_mat_t: public matrix_solver_direct_t<FT, SIZE>
        where FT_OPS : plib.constants_operators<FT>, new()
        where int_SIZE : int_const, new()
    {
        //using float_type = FT;


        state_var<FT> m_omega;


        matrix_solver_SOR_mat_t(devices.nld_solver main_solver, string name, matrix_solver_t_net_list_t nets, solver_parameters_t params_, size_t size)
            : base(main_solver, name, nets, params_, size)
        {
            m_omega = new state_var<FT>(this, "m_omega", ops.cast(params_.m_gs_sor.op()));
        }


        protected override void upstream_solve_non_dynamic()
        {
            // The matrix based code looks a lot nicer but actually is 30% slower than
            // the optimized code which works directly on the data structures.
            // Need something like that for gaussian elimination as well.

            size_t iN = this.size();

            this.clear_square_mat(this.m_A);
            this.fill_matrix_and_rhs();

            bool resched = false;

            unsigned resched_cnt = 0;


#if false
            static int ws_cnt = 0;
            ws_cnt++;
            if (1 && ws_cnt % 200 == 0)
            {
                // update omega
                float_type lambdaN = 0;
                float_type lambda1 = 1e9;
                for (int k = 0; k < iN; k++)
                {
#if false
                    float_type akk = plib::abs(this->m_A[k][k]);
                    if ( akk > lambdaN)
                        lambdaN = akk;
                    if (akk < lambda1)
                        lambda1 = akk;
#else
                    float_type akk = plib::abs(this->m_A[k][k]);
                    float_type s = 0.0;
                    for (int i=0; i<iN; i++)
                        s = s + plib::abs(this->m_A[k][i]);
                    akk = s / akk - 1.0;
                    if ( akk > lambdaN)
                        lambdaN = akk;
                    if (akk < lambda1)
                        lambda1 = akk;
#endif
                }

                //ws = 2.0 / (2.0 - lambdaN - lambda1);
                m_omega = 2.0 / (2.0 - lambda1);
            }
#endif


            for (int k = 0; k < (int)iN; k++)
                this.m_new_V[k] = ops.cast(this.m_terms[k].getV());  //this->m_new_V[k] = static_cast<float_type>(this->m_terms[k].getV());

            do
            {
                resched = false;
                FT cerr = plib.constants<FT, FT_OPS>.zero();

                for (size_t k = 0; k < iN; k++)
                {
                    FT Idrive = ops.cast(0);

                    var p = this.m_terms[k].m_nz;
                    size_t e = (size_t)this.m_terms[k].m_nz.size();

                    for (size_t i = 0; i < e; i++)
                        Idrive = ops.add(Idrive, ops.multiply(this.m_A[k][p[i]], this.m_new_V[p[i]]));  //Idrive = Idrive + this->m_A[k][p[i]] * this->m_new_V[p[i]];

                    FT w = ops.divide(m_omega.op, this.m_A[k][k]);  //FT w = m_omega / this->m_A[k][k];
                    if (this.m_params.m_use_gabs.op())
                    {
                        FT gabs_t = plib.constants<FT, FT_OPS>.zero();
                        for (int i = 0; i < (int)e; i++)
                        {
                            if (p[i] != k)
                                gabs_t = ops.add(gabs_t, plib.pg.abs<FT, FT_OPS>(this.m_A[k][p[i]]));  //gabs_t = gabs_t + plib::abs(this->m_A[k][p[i]]);
                        }

                        gabs_t = ops.multiply(gabs_t, plib.constants<FT, FT_OPS>.one()); // derived by try and error  //gabs_t *= plib::constants<FT>::one(); // derived by try and error
                        if (ops.greater_than(gabs_t, this.m_A[k][k]))  //if (gabs_t > this->m_A[k][k])
                        {
                            w = ops.divide(plib.constants<FT, FT_OPS>.one(), ops.add(this.m_A[k][k], gabs_t));  //w = plib::constants<FT>::one() / (this->m_A[k][k] + gabs_t);
                        }
                    }

                    FT delta = ops.multiply(w, ops.subtract(this.m_RHS[k], Idrive));  //const float_type delta = w * (this->m_RHS[k] - Idrive) ;
                    cerr = ops.max(cerr, plib.pg.abs<FT, FT_OPS>(delta));  //cerr = std::max(cerr, plib::abs(delta));
                    this.m_new_V[k] = ops.add(this.m_new_V[k], delta);  //this->m_new_V[k] += delta;
                }

                if (ops.greater_than(cerr, ops.cast(this.m_params.m_accuracy.op())))  //if (cerr > static_cast<float_type>(this->m_params.m_accuracy))
                {
                    resched = true;
                }

                resched_cnt++;

            } while (resched && (resched_cnt < this.m_params.m_gs_loops.op()));

            this.m_iterative_total.op += resched_cnt;

            if (resched)
            {
                this.m_iterative_fail.op++;
                base.solve_non_dynamic();
            }
        }
    }
}
