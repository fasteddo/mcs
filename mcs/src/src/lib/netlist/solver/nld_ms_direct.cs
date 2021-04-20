// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    namespace solver
    {
        //template <typename FT, int SIZE>
        abstract class matrix_solver_direct_t : matrix_solver_ext_t
        {
            //friend class matrix_solver_t;


            //typedef FT float_type;


            int m_pitch;  //const std::size_t m_pitch;


            int SIZEABS { get { return Math.Abs(SIZE); } }  //static constexpr const std::size_t SIZEABS = plib::parray<FT, SIZE>::SIZEABS();
            int m_pitch_ABS { get { return (((SIZEABS + 0) + 7) / 8) * 8; } }


            //PALIGNAS_VECTOROPT()
            protected MemoryContainer<MemoryContainer<double>> m_A;  //plib::parray2D<FT, SIZE, m_pitch_ABS> m_A;


            protected matrix_solver_direct_t(int SIZE, netlist_state_t anetlist, string name, analog_net_t.list_t nets, solver_parameters_t params_, UInt32 size)
                : base(SIZE, anetlist, name, nets, params_, size)
            {
                m_pitch = m_pitch_ABS != 0 ? m_pitch_ABS : ((((int)size + 0) + 7) / 8) * 8;
                //, m_A(size, m_pitch)
                m_A = new MemoryContainer<MemoryContainer<double>>((int)size);
                for (int i = 0; i < size; i++)
                    m_A[i] = new MemoryContainer<double>(m_pitch);


                this.build_mat_ptr(m_A);
            }


            public override void reset() { base.reset(); }


            protected override UInt32 vsolve_non_dynamic(bool newton_raphson)
            {
                throw new emu_unimplemented();
            }


            protected UInt32 solve_non_dynamic(bool newton_raphson)
            {
                this.LE_solve();
                this.LE_back_subst(this.m_new_V);

                bool err = false;
                if (newton_raphson)
                    err = this.check_err();
                this.store();
                return err ? 2u : 1u;
            }


            void LE_solve()
            {
                int kN = this.size();
                if (!this.m_params.m_pivot.op())
                {
                    for (int i = 0; i < kN; i++)
                    {
                        // FIXME: Singular matrix?
                        double f = plib.pglobal.reciprocal(m_A[i][i]);
                        var nzrd = this.m_terms[i].m_nzrd;
                        var nzbd = this.m_terms[i].m_nzbd;

                        foreach (var j in nzbd)
                        {
                            double f1 = -f * m_A[j][i];
                            foreach (var k in nzrd)
                                m_A[j][k] += m_A[i][k] * f1;

                            this.m_RHS[j] += this.m_RHS[i] * f1;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < kN; i++)
                    {
                        // Find the row with the largest first value
                        int maxrow = i;
                        for (int j = i + 1; j < kN; j++)
                        {
                            if (plib.pglobal.abs(m_A[j][i]) > plib.pglobal.abs(m_A[maxrow][i]))
                            //if (m_A[j][i] * m_A[j][i] > m_A[maxrow][i] * m_A[maxrow][i])
                                maxrow = j;
                        }

                        if (maxrow != i)
                        {
                            // Swap the maxrow and ith row
                            for (int k = 0; k < kN; k++)
                            {
                                //std::swap(m_A[i][k], m_A[maxrow][k]);
                                double temp = m_A[i][k];
                                m_A[i][k] = m_A[maxrow][k];
                                m_A[maxrow][k] = temp;
                            }

                            {
                                //std::swap(this->m_RHS[i], this->m_RHS[maxrow]);
                                double temp = this.m_RHS[i];
                                this.m_RHS[i] = this.m_RHS[maxrow];
                                this.m_RHS[maxrow] = temp;
                            }
                        }

                        // FIXME: Singular matrix?
                        double f = plib.pglobal.reciprocal(m_A[i][i]);  //const FT f = plib::reciprocal(m_A[i][i]);

                        // Eliminate column i from row j

                        for (int j = i + 1; j < kN; j++)
                        {
                            double f1 = - m_A[j][i] * f;  //const FT f1 = - m_A[j][i] * f;
                            if (f1 != plib.constants.zero())
                            {
                                Pointer<double> pi = new Pointer<double>(m_A[i], i + 1);  //const FT * pi = &(m_A[i][i+1]);
                                Pointer<double> pj = new Pointer<double>(m_A[j], i + 1);  //FT * pj = &(m_A[j][i+1]);
                                plib.vector_ops_global.vec_add_mult_scalar_p(kN - i - 1, pj, pi, f1);
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
            void LE_back_subst(double [] x)  //void LE_back_subst(T * RESTRICT x);
            {
                int kN = this.size();

                // back substitution
                if (this.m_params.m_pivot.op())
                {
                    for (int j = kN; j-- > 0; )
                    {
                        double tmp = 0;  //FT tmp = 0;
                        for (int k = j + 1; k < kN; k++)
                            tmp += m_A[j][k] * x[k];
                        x[j] = (this.m_RHS[j] - tmp) / m_A[j][j];
                    }
                }
                else
                {
                    for (int j = kN; j-- > 0; )
                    {
                        double tmp = 0;  //FT tmp = 0;
                        var nzrd = this.m_terms[j].m_nzrd;  //const auto &nzrd = this->m_terms[j].m_nzrd;
                        var e = nzrd.size(); // - 1; // exclude RHS element
                        for (int k = 0; k < e; k++)
                            tmp += m_A[j][nzrd[k]] * x[nzrd[k]];

                        x[j] = (this.m_RHS[j] - tmp) / m_A[j][j];
                    }
                }
            }
        }
    }
}
