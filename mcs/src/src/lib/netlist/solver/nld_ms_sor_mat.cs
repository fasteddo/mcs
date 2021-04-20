// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;
using nl_fptype = System.Double;


namespace mame.netlist
{
    namespace solver
    {
        //template <typename FT, int SIZE>
        class matrix_solver_SOR_mat_t : matrix_solver_direct_t_nl_fptype  //class matrix_solver_SOR_mat_t: public matrix_solver_direct_t<FT, SIZE>
        {
            //using float_type = FT;


            state_var<double> m_omega;


            public matrix_solver_SOR_mat_t(int SIZE, netlist_state_t anetlist, string name, analog_net_t_list_t nets, solver_parameters_t params_, UInt32 size)
                : base(SIZE, anetlist, name, nets, params_, size)
            {
                m_omega = new state_var<double>(this, "m_omega", (double)params_.m_gs_sor.op());
            }


            protected override void vsolve_non_dynamic()
            {
                // The matrix based code looks a lot nicer but actually is 30% slower than
                // the optimized code which works directly on the data structures.
                // Need something like that for gaussian elimination as well.

                int iN = (int)this.size();

                this.clear_square_mat(this.m_A);
                this.fill_matrix_and_rhs();

                bool resched = false;

                UInt32 resched_cnt = 0;


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


                for (int k = 0; k < iN; k++)
                    this.m_new_V[k] = (nl_fptype)this.m_terms[k].getV();

                do
                {
                    resched = false;
                    double cerr = plib.constants_nl_fptype.zero();

                    for (int k = 0; k < iN; k++)
                    {
                        double Idrive = 0;

                        var p = this.m_terms[k].m_nz;
                        int e = this.m_terms[k].m_nz.size();

                        for (int i = 0; i < e; i++)
                            Idrive = Idrive + this.m_A[k][p[i]] * this.m_new_V[p[i]];

                        double w = m_omega.op / this.m_A[k][k];
                        if (this.m_params.m_use_gabs.op())
                        {
                            double gabs_t = plib.constants_nl_fptype.zero();
                            for (int i = 0; i < e; i++)
                            {
                                if (p[i] != k)
                                    gabs_t = gabs_t + plib.pglobal.abs(this.m_A[k][p[i]]);
                            }

                            gabs_t *= plib.constants_nl_fptype.one(); // derived by try and error
                            if (gabs_t > this.m_A[k][k])
                            {
                                w = plib.constants_nl_fptype.one() / (this.m_A[k][k] + gabs_t);
                            }
                        }

                        double delta = w * (this.m_RHS[k] - Idrive);
                        cerr = std.max(cerr, plib.pglobal.abs(delta));
                        this.m_new_V[k] += delta;
                    }

                    if (cerr > (double)this.m_params.m_accuracy.op())
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
}
