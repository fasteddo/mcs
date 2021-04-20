// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;
using nl_fptype = System.Double;
using size_t = System.UInt32;


namespace mame.netlist
{
    namespace solver
    {
        //template <typename FT, int SIZE>
        abstract class matrix_solver_direct_t_nl_fptype : matrix_solver_ext_t_nl_fptype
        {
            //using float_type = FT;


            size_t m_pitch;  //const std::size_t m_pitch;


            size_t SIZEABS { get { return (size_t)Math.Abs(SIZE); } }  //static constexpr const std::size_t SIZEABS = plib::parray<FT, SIZE>::SIZEABS();
            size_t m_pitch_ABS { get { return (((SIZEABS + 0) + 7) / 8) * 8; } }


            // PALIGNAS_VECTOROPT() parrays define alignment already
            protected MemoryContainer<MemoryContainer<nl_fptype>> m_A;  //plib::parray2D<FT, SIZE, m_pitch_ABS> m_A;


            protected matrix_solver_direct_t_nl_fptype(int SIZE, netlist_state_t anetlist, string name, analog_net_t_list_t nets, solver_parameters_t params_, UInt32 size)
                : base(SIZE, anetlist, name, nets, params_, size)
            {
                m_pitch = m_pitch_ABS != 0 ? m_pitch_ABS : (((size + 0) + 7) / 8) * 8;
                //, m_A(size, m_pitch)
                m_A = new MemoryContainer<MemoryContainer<nl_fptype>>((int)size);
                for (int i = 0; i < size; i++)
                    m_A[i] = new MemoryContainer<nl_fptype>((int)m_pitch);


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
                        nl_fptype f = plib.pglobal.reciprocal(Ai[i]);
                        var nzrd = this.m_terms[i].m_nzrd;
                        var nzbd = this.m_terms[i].m_nzbd;

                        foreach (var j in nzbd)
                        {
                            var Aj = m_A[j];
                            nl_fptype f1 = -f * Aj[i];
                            foreach (var k in nzrd)
                                Aj[k] += Ai[k] * f1;

                            this.m_RHS[j] += this.m_RHS[i] * f1;
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
                            if (plib.pglobal.abs(m_A[j][i]) > plib.pglobal.abs(m_A[maxrow][i]))
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
                        nl_fptype f = plib.pglobal.reciprocal(Ai[i]);  //const FT f = plib::reciprocal(Ai[i]);

                        // Eliminate column i from row j

                        for (size_t j = i + 1; j < kN; j++)
                        {
                            var Aj = m_A[j];
                            nl_fptype f1 = - m_A[j][i] * f;  //const FT f1 = - m_A[j][i] * f;
                            if (f1 != plib.constants_nl_fptype.zero())
                            {
                                Pointer<nl_fptype> pi = new Pointer<nl_fptype>(Ai, (int)i + 1);  //const FT * pi = &(Ai[i+1]);
                                Pointer<nl_fptype> pj = new Pointer<nl_fptype>(Aj, (int)i + 1);  //FT * pj = &(Aj[i+1]);
                                plib.pglobal.vec_add_mult_scalar_p((int)(kN - i - 1), pj, pi, f1);
                                //for (unsigned k = i+1; k < kN; k++)
                                //  pj[k] = pj[k] + pi[k] * f1;
                                //for (unsigned k = i+1; k < kN; k++)
                                    //A(j,k) += A(i,k) * f1;
                                this.m_RHS[j] += this.m_RHS[i] * f1;
                            }
                        }
                    }
                }
            }


            //template <typename T>
            void LE_back_subst(Pointer<double> x)  //void LE_back_subst(T * RESTRICT x);
            {
                size_t kN = this.size();

                // back substitution
                if (this.m_params.m_pivot.op())
                {
                    for (size_t j = kN; j-- > 0; )
                    {
                        nl_fptype tmp = 0;
                        var Aj = m_A[j];

                        for (size_t k = j + 1; k < kN; k++)
                            tmp += Aj[k] * x[k];

                        x[j] = (this.m_RHS[j] - tmp) / Aj[j];
                    }
                }
                else
                {
                    for (size_t j = kN; j-- > 0; )
                    {
                        nl_fptype tmp = 0;
                        var nzrd = this.m_terms[j].m_nzrd;
                        var Aj = m_A[j];
                        var e = nzrd.size();

                        for (size_t k = 0; k < e; k++)
                            tmp += Aj[nzrd[k]] * x[nzrd[k]];

                        x[j] = (this.m_RHS[j] - tmp) / Aj[j];
                    }
                }
            }
        }
    }
}
