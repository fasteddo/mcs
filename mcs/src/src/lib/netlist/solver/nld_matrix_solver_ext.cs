// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using analog_net_t_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;
using matrix_solver_t_fptype = System.Double;  //using fptype = nl_fptype;
using matrix_solver_t_net_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;  //using net_list_t =  plib::aligned_vector<analog_net_t *>;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using size_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.netlist.nl_config_global;


namespace mame.netlist
{
    namespace solver
    {
        //template <typename FT, int SIZE>
        abstract class matrix_solver_ext_t<FT, FT_OPS, int_SIZE> : matrix_solver_t
            where FT_OPS : plib.constants_operators<FT>, new()
            where int_SIZE : int_const, new()
        {
            protected static readonly FT_OPS ops = new FT_OPS();
            protected static readonly int SIZE = new int_SIZE().value;


            //using float_type = FT;


            //static constexpr const std::size_t SIZEABS = plib::parray<FT, SIZE>::SIZEABS();
            //static constexpr const std::size_t m_pitch_ABS = (((SIZEABS + 0) + 7) / 8) * 8;


            //PALIGNAS_VECTOROPT() parrays define alignment already
            protected FT [] m_new_V;  //plib::parray<float_type, SIZE> m_new_V;
            //PALIGNAS_VECTOROPT() parrays define alignment already
            protected FT [] m_RHS;  //plib::parray<float_type, SIZE> m_RHS;

            //PALIGNAS_VECTOROPT() parrays define alignment already
            protected plib.pmatrix2d<Pointer<FT>> m_mat_ptr;  //plib::pmatrix2d<float_type *> m_mat_ptr;


            // state - variable time_stepping
            //PALIGNAS_VECTOROPT() parrays define alignment already
            FT [] m_last_V;  //plib::parray<fptype, SIZE> m_last_V;
            // PALIGNAS_VECTOROPT() parrays define alignment already
            FT [] m_DD_n_m_1;  //plib::parray<fptype, SIZE> m_DD_n_m_1;
            // PALIGNAS_VECTOROPT() parrays define alignment already
            FT [] m_h_n_m_1;  //plib::parray<fptype, SIZE> m_h_n_m_1;

            size_t m_dim;  //const std::size_t m_dim;


            protected matrix_solver_ext_t(devices.nld_solver main_solver, string name, matrix_solver_t_net_list_t nets, solver.solver_parameters_t params_, size_t size)
                : base(main_solver, name, nets, params_)
            {
                m_new_V = new FT [size];
                m_RHS = new FT [size];
                m_mat_ptr = new plib.pmatrix2d<Pointer<FT>>(size, this.max_railstart() + 1);
                m_last_V = new FT [size]; std.fill(m_last_V, ops.cast(nlconst.zero()));
                m_DD_n_m_1 = new FT [size]; std.fill(m_DD_n_m_1, ops.cast(nlconst.zero()));
                m_h_n_m_1 = new FT [size]; std.fill(m_h_n_m_1, ops.cast(nlconst.magic(1e-6))); // we need a non zero value here
                m_dim = size;


                //
                // save states
                //
                state().save(this, m_last_V, this.name(), "m_last_V");
                state().save(this, m_DD_n_m_1, this.name(), "m_DD_n_m_1");
                state().save(this, m_h_n_m_1, this.name(), "m_h_n_m_1");
            }


            //template <typename T, typename M>
            protected void log_fill(std.vector<std.vector<unsigned>> fill, plib.pGEmatrix_cr<FT, FT_OPS, int_SIZE> mat)  //void log_fill(const T &fill, M &mat)
            {
                size_t iN = fill.size();

                // FIXME: Not yet working, mat_cr.h needs some more work
#if false
                auto mat_GE = dynamic_cast<plib::pGEmatrix_cr_t<typename M::base> *>(&mat);
#else
                //plib::unused_var(mat);
#endif
                std.vector<unsigned> levL = new std.vector<unsigned>(iN, 0);
                std.vector<unsigned> levU = new std.vector<unsigned>(iN, 0);

                // parallel scheme for L x = y
                for (size_t k = 0; k < iN; k++)
                {
                    unsigned lm = 0;
                    for (size_t j = 0; j < k; j++)
                    {
                        if (fill[k][j] < (unsigned)plib.pGEmatrix_cr<FT, FT_OPS, int_SIZE>.constants_e.FILL_INFINITY)  //if (fill[k][j] < M::FILL_INFINITY)
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
                        if (fill[k][j] < (unsigned)plib.pGEmatrix_cr<FT, FT_OPS, int_SIZE>.constants_e.FILL_INFINITY)  //if (fill[k][j] < M::FILL_INFINITY)
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
                        ml += fill[k][j] == 0 ? 'X' : fill[k][j] < (unsigned)plib.pGEmatrix_cr<FT, FT_OPS, int_SIZE>.constants_e.FILL_INFINITY ? '+' : '.';  //ml += fill[k][j] == 0 ? 'X' : fill[k][j] < M::FILL_INFINITY ? '+' : '.';
                        if (fill[k][j] < (unsigned)plib.pGEmatrix_cr<FT, FT_OPS, int_SIZE>.constants_e.FILL_INFINITY)  //if (fill[k][j] < M::FILL_INFINITY)
                        {
                            if (fill[k][j] > fm)
                                fm = fill[k][j];
                        }
                    }

#if false
                    this->log().verbose("{1:4} {2} {3:4} {4:4} {5:4} {6:4}", k, ml,
                        levL[k], levU[k], mat_GE ? mat_GE->get_parallel_level(k) : 0, fm);
#else
                    this.log().verbose.op("{0:4} {1} {2:4} {3:4} {4:4} {5:4}", k, ml, levL[k], levU[k], 0, fm);
#endif
                }
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
                    this.m_terms[i].setV(ops.cast_double(m_new_V[i]));  //this->m_terms[i].setV(static_cast<fptype>(m_new_V[i]));
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
                var reltol = ops.cast(m_params.m_reltol.op());  //const auto reltol(static_cast<float_type>(m_params.m_reltol));
                var vntol = ops.cast(m_params.m_vntol.op());  //const auto vntol(static_cast<float_type>(m_params.m_vntol));
                for (size_t i = 0; i < iN; i++)
                {
                    var vold = ops.cast(this.m_terms[i].getV());  //const auto vold(static_cast<float_type>(this->m_terms[i].getV()));
                    var vnew = m_new_V[i];
                    var tol = ops.add(vntol, ops.multiply(reltol, ops.max(plib.pg.abs<FT, FT_OPS>(vnew), plib.pg.abs<FT, FT_OPS>(vold))));  //const auto tol(vntol + reltol * std::max(plib::abs(vnew),plib::abs(vold)));
                    if (ops.greater_than(plib.pg.abs<FT, FT_OPS>(ops.subtract(vnew, vold)), tol))  //if (plib::abs(vnew - vold) > tol)
                        return true;
                }

                return false;
            }


            protected override void backup()
            {
                size_t iN = size();
                for (size_t i = 0; i < iN; i++)
                    m_last_V[i] = ops.cast(this.m_terms[i].getV());  //m_last_V[i] = gsl::narrow_cast<fptype>(this->m_terms[i].getV());
            }


            protected override void restore()
            {
                size_t iN = size();
                for (size_t i = 0; i < iN; i++)
                    this.m_terms[i].setV(ops.cast_double(m_last_V[i]));  //this->m_terms[i].setV(static_cast<nl_fptype>(m_last_V[i]));
            }


            protected override netlist_time compute_next_timestep(matrix_solver_t_fptype cur_ts, matrix_solver_t_fptype min_ts, matrix_solver_t_fptype max_ts)
            {
                matrix_solver_t_fptype new_solver_timestep_sq = max_ts * max_ts;

                for (size_t k = 0; k < size(); k++)
                {
                    var t = m_terms[k];
                    var v = (matrix_solver_t_fptype)t.getV();  //const auto v(static_cast<fptype>(t.getV()));
                    // avoid floating point exceptions
                    matrix_solver_t_fptype DD_n = std.max(-fp_constants_double.TIMESTEP_MAXDIFF(), std.min(+fp_constants_double.TIMESTEP_MAXDIFF(), v - ops.cast_double(m_last_V[k])));  //const fptype DD_n = std::max(-fp_constants<fptype>::TIMESTEP_MAXDIFF(), std::min(+fp_constants<fptype>::TIMESTEP_MAXDIFF(),(v - m_last_V[k])));

                    //m_last_V[k] = v;
                    matrix_solver_t_fptype hn = cur_ts;

                    matrix_solver_t_fptype DD2 = (DD_n / hn - ops.cast_double(m_DD_n_m_1[k]) / ops.cast_double(m_h_n_m_1[k])) / (hn + ops.cast_double(m_h_n_m_1[k]));  //fptype DD2 = (DD_n / hn - m_DD_n_m_1[k] / m_h_n_m_1[k]) / (hn + m_h_n_m_1[k]);

                    m_h_n_m_1[k] = ops.cast(hn);
                    m_DD_n_m_1[k] = ops.cast(DD_n);
                    {
                        // save the sqrt for the end
                        matrix_solver_t_fptype new_net_timestep_sq = m_params.m_dynamic_lte.op() / plib.pg.abs(nlconst.half() * DD2);
                        new_solver_timestep_sq = std.min(new_net_timestep_sq, new_solver_timestep_sq);
                    }
                }

                new_solver_timestep_sq = std.max(plib.pg.sqrt(new_solver_timestep_sq), min_ts);

                // FIXME: Factor 2 below is important. Without, we get timing issues. This must be a bug elsewhere.
                return std.max(netlist_time.from_fp(new_solver_timestep_sq), netlist_time.quantum() * 2);
            }


            //template <typename M>
            protected void build_mat_ptr(MemoryContainer<MemoryContainer<FT>> mat)  //void build_mat_ptr(M &mat)
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
                            m_mat_ptr.op(k)[j] = new Pointer<FT>(mat[k], other);  //m_mat_ptr[k][j] = &(mat[k][(size_t)other]);
                            cnt++;
                        }
                    }

                    nl_assert_always(cnt == this.m_terms[k].railstart(), "Count and railstart mismatch");
                    m_mat_ptr.op(k)[this.m_terms[k].railstart()] = new Pointer<FT>(mat[k], (int)k);  //m_mat_ptr[k][this->m_terms[k].railstart()] = &(mat[k][k]);
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
                    var net = m_terms[k];  //auto &net = m_terms[k];
                    var tcr_r = m_mat_ptr.op(k)[0];  //auto **tcr_r = &(m_mat_ptr[k][0]);

                    //using source_type = typename decltype(m_gtn)::value_type;
                    size_t term_count = net.count();
                    size_t railstart = net.railstart();
                    var go = m_gonn.op(k);
                    var gt = m_gtn.op(k);
                    var Idr = m_Idrn.op(k);
                    var cnV = m_connected_net_Vn.op(k);

                    // FIXME: gonn, gtn and Idr - which float types should they have?

                    var gtot_t = plib.constants<matrix_solver_t_fptype, plib.constants_operators_double>.zero();  //auto gtot_t = std::accumulate(gt, gt + term_count, plib::constants<source_type>::zero());
                    for (size_t i = 0; i < term_count; i++)
                        gtot_t += (gt + (int)i)[0];

                    // update diagonal element ...
                    tcr_r[railstart] = ops.cast(gtot_t); //mat.A[mat.diag[k]] += gtot_t;  //*tcr_r[railstart] = static_cast<FT>(gtot_t); //mat.A[mat.diag[k]] += gtot_t;

                    for (size_t i = 0; i < railstart; i++)
                        tcr_r[i]       = ops.add(tcr_r[i], ops.cast(go[i]));  //*tcr_r[i]       += static_cast<FT>(go[i]);

                    var RHS_t = plib.constants<matrix_solver_t_fptype, plib.constants_operators_double>.zero();  //auto RHS_t(std::accumulate(Idr, Idr + term_count, plib::constants<source_type>::zero()));
                    for (size_t i = 0; i < term_count; i++)
                        RHS_t += (Idr + (int)i)[0];

                    for (size_t i = railstart; i < term_count; i++)
                        RHS_t +=  (- go[i]) * cnV[i][0];  //RHS_t +=  (- go[i]) * *cnV[i];

                    m_RHS[k] = ops.cast(RHS_t);  //m_RHS[k] = static_cast<FT>(RHS_t);
                }
            }
        }
    } // namespace solver
} // namespace netlist
