// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using analog_net_t_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;
using matrix_solver_t_net_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;  //using net_list_t =  plib::aligned_vector<analog_net_t *>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using size_t = System.UInt64;


namespace mame.netlist
{
    namespace solver
    {
        //template <typename FT, int SIZE>
        abstract class matrix_solver_direct_t<FT, FT_OPS, int_SIZE> : matrix_solver_ext_t<FT, FT_OPS, int_SIZE>
            where FT_OPS : plib.constants_operators<FT>, new()
            where int_SIZE : int_const, new()
        {
            //using float_type = FT;


            size_t m_pitch;


            static readonly size_t SIZEABS = (size_t)Math.Abs(SIZE);
            static readonly size_t m_pitch_ABS = (((SIZEABS + 0) + 7) / 8) * 8;


            // PALIGNAS_VECTOROPT() parrays define alignment already
            protected MemoryContainer<MemoryContainer<FT>> m_A;  //plib::parray2D<FT, SIZE, m_pitch_ABS> m_A;


            protected matrix_solver_direct_t(devices.nld_solver main_solver, string name, matrix_solver_t_net_list_t nets, solver.solver_parameters_t params_, size_t size)
                : base(main_solver, name, nets, params_, size)
            {
                m_pitch = m_pitch_ABS != 0 ? m_pitch_ABS : (((size + 0) + 7) / 8) * 8;
                //, m_A(size, m_pitch)
                m_A = new MemoryContainer<MemoryContainer<FT>>((int)size);
                for (int i = 0; i < (int)size; i++)
                    m_A[i] = new MemoryContainer<FT>((int)m_pitch);


                this.build_mat_ptr(m_A);
            }


            public override void reset() { base.reset(); }


            protected override void vsolve_non_dynamic()
            {
                throw new emu_unimplemented();
            }


            protected void solve_non_dynamic()
            {
                this.LE_solve();

                throw new emu_unimplemented();
#if false
                this.LE_back_subst(new Pointer<nl_fptype>(this.m_new_V));
#endif
            }


            void LE_solve()
            {
                size_t kN = this.size();
                if (!this.m_params.m_pivot.op())
                {
                    for (size_t i = 0; i < kN; i++)
                    {
                        // FIXME: Singular matrix?
                        var Ai = m_A[i];
                        FT f = plib.pg.reciprocal<FT, FT_OPS>(Ai[i]);
                        var nzrd = this.m_terms[i].m_nzrd;
                        var nzbd = this.m_terms[i].m_nzbd;

                        foreach (var j in nzbd)
                        {
                            var Aj = m_A[j];
                            FT f1 = ops.multiply(ops.neg(f), Aj[i]);  //const FT f1 = -f * Aj[i];
                            foreach (var k in nzrd)
                                Aj[k] = ops.add(Aj[k], ops.multiply(Ai[k], f1));  //Aj[k] += Ai[k] * f1;

                            this.m_RHS[j] = ops.add(this.m_RHS[j], ops.multiply(this.m_RHS[i], f1));  //this->m_RHS[j] += this->m_RHS[i] * f1;
                        }
                    }
                }
                else
                {
                    for (size_t i = 0; i < kN; i++)
                    {
                        // Find the row with the largest first value
                        size_t maxrow = i;
                        for (size_t j = i + 1; j < kN; j++)
                        {
                            if (ops.greater_than(plib.pg.abs<FT, FT_OPS>(m_A[j][i]), plib.pg.abs<FT, FT_OPS>(m_A[maxrow][i])))  //if (plib::abs(m_A[j][i]) > plib::abs(m_A[maxrow][i]))
                            //if (m_A[j][i] * m_A[j][i] > m_A[maxrow][i] * m_A[maxrow][i])
                                maxrow = j;
                        }

                        if (maxrow != i)
                        {
#if false
                            // Swap the maxrow and ith row
                            for (int k = 0; k < kN; k++)
                            {
                                //std::swap(m_A[i][k], m_A[maxrow][k]);
                                double temp = m_A[i][k];
                                m_A[i][k] = m_A[maxrow][k];
                                m_A[maxrow][k] = temp;
                            }
#else
                            {
                                //std::swap(m_A[i], m_A[maxrow]);
                                var temp = m_A[i];
                                m_A[i] = m_A[maxrow];
                                m_A[maxrow] = temp;
                            }
#endif

                            {
                                //std::swap(this->m_RHS[i], this->m_RHS[maxrow]);
                                var temp = this.m_RHS[i];
                                this.m_RHS[i] = this.m_RHS[maxrow];
                                this.m_RHS[maxrow] = temp;
                            }
                        }

                        // FIXME: Singular matrix?
                        var Ai = m_A[i];
                        FT f = plib.pg.reciprocal<FT, FT_OPS>(Ai[i]);  //const FT f = plib::reciprocal(Ai[i]);

                        // Eliminate column i from row j

                        for (size_t j = i + 1; j < kN; j++)
                        {
                            var Aj = m_A[j];
                            FT f1 = ops.multiply(ops.neg(m_A[j][i]), f);  //const FT f1 = - m_A[j][i] * f;
                            if (ops.not_equals(f1, plib.constants<FT, FT_OPS>.zero()))
                            {
                                Pointer<FT> pi = new Pointer<FT>(Ai, (int)i + 1);  //const FT * pi = &(Ai[i+1]);
                                Pointer<FT> pj = new Pointer<FT>(Aj, (int)i + 1);  //FT * pj = &(Aj[i+1]);
                                plib.pg.vec_add_mult_scalar_p<FT, FT_OPS>((int)(kN - i - 1), pj, pi, f1);  //plib::vec_add_mult_scalar_p(kN-i-1,pj,pi,f1);
                                //for (unsigned k = i+1; k < kN; k++)
                                //  pj[k] = pj[k] + pi[k] * f1;
                                //for (unsigned k = i+1; k < kN; k++)
                                    //A(j,k) += A(i,k) * f1;
                                this.m_RHS[j] = ops.add(this.m_RHS[j], ops.multiply(this.m_RHS[i], f1));  //this->m_RHS[j] += this->m_RHS[i] * f1;
                            }
                        }
                    }
                }
            }


            //template <typename T>
            void LE_back_subst(Pointer<FT> x)  //void LE_back_subst(T * RESTRICT x);
            {
                size_t kN = this.size();

                // back substitution
                if (this.m_params.m_pivot.op())
                {
                    for (size_t j = kN; j-- > 0; )
                    {
                        FT tmp = ops.cast(0);
                        var Aj = m_A[j];

                        for (size_t k = j + 1; k < kN; k++)
                            tmp = ops.add(tmp, ops.multiply(Aj[k], x[k]));  //tmp += Aj[k] * x[k];

                        x[j] = ops.divide(ops.subtract(this.m_RHS[j], tmp), Aj[j]);  //x[j] = (this->m_RHS[j] - tmp) / Aj[j];
                    }
                }
                else
                {
                    for (size_t j = kN; j-- > 0; )
                    {
                        FT tmp = ops.cast(0);
                        var nzrd = this.m_terms[j].m_nzrd;
                        var Aj = m_A[j];
                        var e = nzrd.size();

                        for (size_t k = 0; k < e; k++)
                            tmp = ops.add(tmp, ops.multiply(Aj[nzrd[k]], x[nzrd[k]]));  //tmp += Aj[nzrd[k]] * x[nzrd[k]];

                        x[j] = ops.divide(ops.subtract(this.m_RHS[j], tmp), Aj[j]);  //x[j] = (this->m_RHS[j] - tmp) / Aj[j];
                    }
                }
            }
        }
    }
}
