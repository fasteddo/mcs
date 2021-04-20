// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;
using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;
using nl_fptype = System.Double;
using size_t = System.UInt32;
using unsigned = System.UInt32;


namespace mame.netlist
{
    namespace solver
    {

        //template <typename FT, int SIZE>
        abstract class matrix_solver_ext_t_nl_fptype : matrix_solver_t
        {
            protected int SIZE;

            //using float_type = FT;


            //static constexpr const std::size_t SIZEABS = plib::parray<FT, SIZE>::SIZEABS();
            //static constexpr const std::size_t m_pitch_ABS = (((SIZEABS + 0) + 7) / 8) * 8;


            //PALIGNAS_VECTOROPT() parrays define alignment already
            protected nl_fptype [] m_new_V;  //plib::parray<float_type, SIZE> m_new_V;
            //PALIGNAS_VECTOROPT() parrays define alignment already
            protected nl_fptype [] m_RHS;  //plib::parray<float_type, SIZE> m_RHS;

            //PALIGNAS_VECTOROPT() parrays define alignment already
            protected plib.pmatrix2d_listpointer_nl_fptype m_mat_ptr;  //plib::pmatrix2d<float_type *> m_mat_ptr;


            // state - variable time_stepping
            //PALIGNAS_VECTOROPT() parrays define alignment already
            nl_fptype [] m_last_V;  //plib::parray<fptype, SIZE> m_last_V;
            // PALIGNAS_VECTOROPT() parrays define alignment already
            nl_fptype [] m_DD_n_m_1;  //plib::parray<fptype, SIZE> m_DD_n_m_1;
            // PALIGNAS_VECTOROPT() parrays define alignment already
            nl_fptype [] m_h_n_m_1;  //plib::parray<fptype, SIZE> m_h_n_m_1;

            size_t m_dim;  //const std::size_t m_dim;


            public matrix_solver_ext_t_nl_fptype(int SIZE, netlist_state_t anetlist, string name, analog_net_t_list_t nets, solver_parameters_t params_, size_t size)
                : base(anetlist, name, nets, params_)
            {
                this.SIZE = SIZE;


                m_new_V = new nl_fptype [size];
                m_RHS = new nl_fptype [size];
                m_mat_ptr = new plib.pmatrix2d_listpointer_nl_fptype(size, this.max_railstart() + 1);
                m_last_V = new nl_fptype [size]; std.fill(m_last_V, nlconst.zero());
                m_DD_n_m_1 = new nl_fptype [size]; std.fill(m_DD_n_m_1, nlconst.zero());
                m_h_n_m_1 = new nl_fptype [size]; std.fill(m_h_n_m_1, nlconst.magic(1e-6)); // we need a non zero value here
                m_dim = size;


                //
                // save states
                //
                state().save(this, m_last_V, this.name(), "m_last_V");
                state().save(this, m_DD_n_m_1, this.name(), "m_DD_n_m_1");
                state().save(this, m_h_n_m_1, this.name(), "m_h_n_m_1");
            }


            size_t max_railstart()
            {
                size_t max_rail = 0;
                for (size_t k = 0; k < m_terms.size(); k++)
                    max_rail = std.max(max_rail, m_terms[k].railstart());

                return max_rail;
            }


            //template <typename T, typename M>
            protected void log_fill(object fill, object mat)  //void log_fill(const T &fill, M &mat)
            {
                throw new emu_unimplemented();
#if false
                size_t iN = fill.size();

                // FIXME: Not yet working, mat_cr.h needs some more work
#if false
                auto mat_GE = dynamic_cast<plib::pGEmatrix_cr_t<typename M::base> *>(&mat);
#else
                //plib.unused_var(mat);
#endif
                std.vector<unsigned> levL = new std.vector<unsigned>(iN, 0);
                std.vector<unsigned> levU = new std.vector<unsigned>(iN, 0);

                // parallel scheme for L x = y
                for (size_t k = 0; k < iN; k++)
                {
                    unsigned lm = 0;
                    for (size_t j = 0; j < k; j++)
                    {
                        if (fill[k][j] < M.FILL_INFINITY)
                            lm = std.max(lm, levL[j]);
                    }

                    levL[k] = 1 + lm;
                }

                // parallel scheme for U x = y
                for (size_t k = iN; k-- > 0; )
                {
                    unsigned lm = 0;
                    for (size_t j = iN; --j > k; )
                    {
                        if (fill[k][j] < M.FILL_INFINITY)
                            lm = std.max(lm, levU[j]);
                    }

                    levU[k] = 1 + lm;
                }

                for (size_t k = 0; k < iN; k++)
                {
                    unsigned fm = 0;
                    string ml = "";
                    for (size_t j = 0; j < iN; j++)
                    {
                        ml += fill[k][j] == 0 ? 'X' : fill[k][j] < M.FILL_INFINITY ? '+' : '.';
                        if (fill[k][j] < M.FILL_INFINITY)
                        {
                            if (fill[k][j] > fm)
                                fm = fill[k][j];
                        }
                    }
#if false
                    this->log().verbose("{1:4} {2} {3:4} {4:4} {5:4} {6:4}", k, ml,
                        levL[k], levU[k], mat_GE ? mat_GE->get_parallel_level(k) : 0, fm);
#else
                    this.log().verbose.op("{0:4} {1} {2:4} {3:4} {4:4} {5:4}", k, ml,
                        levL[k], levU[k], 0, fm);
#endif
                }
#endif
            }


            protected size_t size()
            {
                return (SIZE > 0) ? (size_t)SIZE : m_dim;
            }


#if true
            protected override void store()
            {
                size_t iN = size();
                for (size_t i = 0; i < iN; i++)
                    this.m_terms[i].setV((nl_fptype)m_new_V[i]);
            }
#else
            // global tanh damping (4.197)
            // partially cures the symptoms but not the cause
            void store() override
            {
                const std::size_t iN = size();
                for (std::size_t i = 0; i < iN; i++)
                {
                    auto oldV = this->m_terms[i].template getV<fptype>();
                    this->m_terms[i].setV(oldV + 0.02 * plib::tanh((m_new_V[i]-oldV)*50.0));
                }
            }
#endif


            protected override bool check_err()
            {
                // NOTE: Ideally we should also include currents (RHS) here. This would
                // need a reevaluation of the right hand side after voltages have been updated
                // and thus belong into a different calculation. This applies to all solvers.

                size_t iN = size();
                var reltol = (nl_fptype)m_params.m_reltol.op();
                var vntol = (nl_fptype)m_params.m_vntol.op();
                for (size_t i = 0; i < iN; i++)
                {
                    var vold = (nl_fptype)this.m_terms[i].getV();
                    var vnew = m_new_V[i];
                    var tol = vntol + reltol * std.max(plib.pglobal.abs(vnew), plib.pglobal.abs(vold));
                    if (plib.pglobal.abs(vnew - vold) > tol)
                        return true;
                }

                return false;
            }


            protected override netlist_time compute_next_timestep(nl_fptype cur_ts, nl_fptype max_ts)
            {
                nl_fptype new_solver_timestep = max_ts;

                for (size_t k = 0; k < size(); k++)
                {
                    var t = m_terms[k];
                    var v = (nl_fptype)t.getV();
                    // avoid floating point exceptions
                    nl_fptype DD_n = std.max(-fp_constants_double.TIMESTEP_MAXDIFF(),
                        std.min(+fp_constants_double.TIMESTEP_MAXDIFF(), v - m_last_V[k]));

                    m_last_V[k] = v;
                    nl_fptype hn = cur_ts;

                    nl_fptype DD2 = (DD_n / hn - m_DD_n_m_1[k] / m_h_n_m_1[k]) / (hn + m_h_n_m_1[k]);
                    nl_fptype new_net_timestep = 0;

                    m_h_n_m_1[k] = hn;
                    m_DD_n_m_1[k] = DD_n;
                    if (plib.pglobal.abs(DD2) > fp_constants_double.TIMESTEP_MINDIV()) // avoid div-by-zero
                        new_net_timestep = plib.pglobal.sqrt(m_params.m_dynamic_lte.op() / plib.pglobal.abs(nlconst.half() * DD2));
                    else
                        new_net_timestep = m_params.m_max_timestep;

                    new_solver_timestep = std.min(new_net_timestep, new_solver_timestep);
                }

                new_solver_timestep = std.max(new_solver_timestep, m_params.m_min_timestep);

                // FIXME: Factor 2 below is important. Without, we get timing issues. This must be a bug elsewhere.
                return std.max(netlist_time.from_fp(new_solver_timestep), netlist_time.quantum() * 2);
            }


            //template <typename M>
            protected void build_mat_ptr(object mat)
            {
                size_t iN = size();

                for (size_t k = 0; k < iN; k++)
                {
                    size_t cnt = 0;

                    // build pointers into the compressed row format matrix for each terminal
                    for (size_t j = 0; j< this.m_terms[k].railstart(); j++)
                    {
                        int other = this.m_terms[k].m_connected_net_idx[j];
                        if (other >= 0)
                        {
                            throw new emu_unimplemented();
#if false
                            m_mat_ptr[k][j] = &(mat[k][(size_t)other]);
                            cnt++;
#endif
                        }
                    }

                    throw new emu_unimplemented();
#if false
                    nl_config_global.nl_assert_always(cnt == this.m_terms[k].railstart(), "Count and railstart mismatch");
                    m_mat_ptr[k][this.m_terms[k].railstart()] = &(mat[k][k]);
#endif
                }
            }


            //template <typename M>
            protected void clear_square_mat(object m)
            {
                size_t n = size();
                for (size_t k = 0; k < n; k++)
                {
                    throw new emu_unimplemented();
#if false
                    var p = &(m[k][0]);
                    //using mat_elem_type = typename std::decay<decltype(*p)>::type;
                    for (size_t i = 0; i < n; i++)
                        p[i] = plib.constants<mat_elem_type>.zero();
#endif
                }
            }


            protected void fill_matrix_and_rhs()
            {
                size_t N = size();

                for (size_t k = 0; k < N; k++)
                {
                    throw new emu_unimplemented();
#if false
                    var net = m_terms[k];
                    var tcr_r = &(m_mat_ptr[k][0]);  //auto **tcr_r = &(m_mat_ptr[k][0]);

                    //using source_type = typename decltype(m_gtn)::value_type;
                    size_t term_count = net.count();
                    size_t railstart = net.railstart();
                    var go = m_gonn[k];
                    var gt = m_gtn[k];
                    var Idr = m_Idrn[k];
                    var cnV = m_connected_net_Vn[k];

                    // FIXME: gonn, gtn and Idr - which float types should they have?

                    var gtot_t = std.accumulate(gt, gt + term_count, plib.constants<source_type>.zero());

                    // update diagonal element ...
                    *tcr_r[railstart] = (nl_fptype)gtot_t; //mat.A[mat.diag[k]] += gtot_t;

                    for (size_t i = 0; i < railstart; i++)
                        *tcr_r[i] += static_cast<FT>(go[i]);

                    var RHS_t = std.accumulate(Idr, Idr + term_count, plib.constants<source_type>.zero());

                    for (size_t i = railstart; i < term_count; i++)
                        RHS_t +=  (- go[i]) * *cnV[i];

                    m_RHS[k] = (nl_fptype)RHS_t;
#endif
                }
            }
        }
    } // namespace solver
} // namespace netlist
