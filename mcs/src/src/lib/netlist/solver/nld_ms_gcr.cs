// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;
using mat_index_type = System.UInt16;  //using mat_index_type = typename plib::matrix_compressed_rows_t<FT, SIZE>::index_type;
using matrix_solver_t_fptype = System.Double;  //using fptype = nl_fptype;
using matrix_solver_t_net_list_t = mame.plib.aligned_vector<mame.netlist.analog_net_t>;  //using net_list_t =  plib::aligned_vector<analog_net_t *>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using size_t = System.UInt32;
using uint16_t = System.UInt16;
using unsigned = System.UInt32;


namespace mame.netlist
{
    namespace solver
    {
        //template <typename FT, int SIZE>
        abstract class matrix_solver_GCR_t<FT, FT_OPS, int_SIZE> : matrix_solver_ext_t<FT, FT_OPS, int_SIZE>  //class matrix_solver_GCR_t: public matrix_solver_ext_t<FT, SIZE>
            where FT_OPS : plib.constants_operators<FT>, new()
            where int_SIZE : int_constant, new()
        {
            //using mat_type = plib::pGEmatrix_cr<plib::pmatrix_cr_t<FT, SIZE>>;
            //using base_type = matrix_solver_ext_t<FT, SIZE>;
            //using fptype = typename base_type::fptype;
            //using mat_index_type = typename plib::pmatrix_cr<FT, SIZE>::index_type;


            plib.pGEmatrix_cr<FT, int_SIZE> mat;  //mat_type mat;
            plib.dynproc m_proc;  //plib::dynproc<void, FT *, nl_fptype *, nl_fptype *, nl_fptype *, nl_fptype ** > m_proc;


            matrix_solver_GCR_t(devices.nld_solver main_solver, string name, matrix_solver_t_net_list_t nets, solver.solver_parameters_t params_, size_t size)
                : base(main_solver, name, nets, params_, size)
            {
                mat = new plib.pGEmatrix_cr<FT, int_SIZE>((uint16_t)size);  //mat(static_cast<typename mat_type::index_type>(size))
                m_proc = new plib.dynproc();


                size_t iN = this.size();

                // build the final matrix

                std.vector<std.vector<unsigned>> fill = new std.vector<std.vector<unsigned>>(iN);
                fill.Fill(() => { return new std.vector<size_t>(); });

                size_t raw_elements = 0;

                for (size_t k = 0; k < iN; k++)
                {
                    fill[k].resize((int)iN, (unsigned)plib.pGEmatrix_cr<FT, int_SIZE>.constants_e.FILL_INFINITY);
                    foreach (var j in this.m_terms[k].m_nz)
                    {
                        fill[k][j] = 0;
                        raw_elements++;
                    }
                }

                var gr = mat.gaussian_extend_fill_mat(fill);

                this.log_fill(fill, mat);

                mat.build_from_fill_mat(fill);

                for (mat_index_type k = 0; k < iN; k++)
                {
                    size_t cnt = 0;
                    // build pointers into the compressed row format matrix for each terminal
                    for (size_t j = 0; j < this.m_terms[k].railstart(); j++)
                    {
                        int other = this.m_terms[k].m_connected_net_idx[j];
                        for (var i = mat.row_idx[k]; i < mat.row_idx[k+1]; i++)
                        {
                            if (other == (int)mat.col_idx[i])
                            {
                                throw new emu_unimplemented();
#if false
                                this.m_mat_ptr[k][j] = new Pointer<nl_fptype>(mat.A, i);
                                cnt++;
                                break;
#endif
                            }
                        }
                    }

                    throw new emu_unimplemented();
#if false
                    nl_config_global.nl_assert(cnt == this.m_terms[k].railstart());
                    this.m_mat_ptr[k][this.m_terms[k].railstart()] = new Pointer<nl_fptype>(mat.A, mat.diag[k]);
#endif
                }

                this.state().log().verbose.op("maximum fill: {0}", gr.first);
                this.state().log().verbose.op("Post elimination occupancy ratio: {1} Ops: {0}", gr.second,
                        (matrix_solver_t_fptype)mat.nz_num / (matrix_solver_t_fptype)(iN * iN));
                this.state().log().verbose.op(" Pre elimination occupancy ratio: {0}",
                        (matrix_solver_t_fptype)raw_elements / (matrix_solver_t_fptype)(iN * iN));

                // FIXME: Move me
                //

                if (this.state().static_solver_lib().isLoaded())
                {
                    string symname = static_compile_name();
                    m_proc.load(this.state().static_solver_lib(), symname);
                    if (m_proc.resolved())
                    {
                        this.state().log().info.op("External static solver {0} found ...", symname);
                    }
                    else
                    {
                        this.state().log().warning.op("External static solver {0} not found ...", symname);
                    }
                }
            }


            //template <typename FT, int SIZE>
            protected override void vsolve_non_dynamic()
            {
                if (m_proc.resolved())
                {
                    throw new emu_unimplemented();
#if false
                    m_proc.op(new Pointer<nl_fptype>(this.m_new_V, 0),
                        this.m_gonn.data(), this.m_gtn.data(), this.m_Idrn.data(),
                        this.m_connected_net_Vn.data());
#endif
                }
                else
                {
                    //  clear matrix
                    mat.set_scalar(plib.constants<FT, FT_OPS>.zero());

                    // populate matrix
                    this.fill_matrix_and_rhs();

                    // now solve it
                    // parallel is slow -- very slow
                    // mat.gaussian_elimination_parallel(RHS);
                    mat.gaussian_elimination(this.m_RHS);
                    // backward substitution
                    throw new emu_unimplemented();
#if false
                    mat.gaussian_back_substitution(new Pointer<nl_fptype>(this.m_new_V), this.m_RHS);
#endif
                }
            }


            protected override std.pair<string, string> create_solver_code(static_compile_target target)
            {
                throw new emu_unimplemented();
            }


            //template <typename FT, int SIZE>
            void generate_code(out string strm)  //void matrix_solver_GCR_t<FT, SIZE>::generate_code(plib::putf8_fmt_writer &strm)
            {
                strm = "";

                size_t iN = (size_t)this.size();
                string fptype = fp_constants_double.name();  //pstring fptype(fp_constants<FT>::name());
                string fpsuffix = fp_constants_double.suffix();  //pstring fpsuffix(fp_constants<FT>::suffix());

                // avoid unused variable warnings
                strm += string.Format("\tplib::unused_var({0});\n", "cnV");

                for (size_t i = 0; i < mat.nz_num; i++)
                    strm += string.Format("\t{0} m_A{1}(0.0);\n", fptype, i, i);

                for (size_t k = 0; k < iN; k++)
                {
                    var net = this.m_terms[k];

                    // FIXME: gonn, gtn and Idr - which float types should they have?

                    //auto gtot_t = std::accumulate(gt, gt + term_count, plib::constants<FT>::zero());
                    //*tcr_r[railstart] = static_cast<FT>(gtot_t); //mat.A[mat.diag[k]] += gtot_t;

                    throw new emu_unimplemented();
#if false
                    var pd = this.m_mat_ptr[k][net.railstart()] - new Pointer<nl_fptype>(this.mat.A, 0);  //auto pd = this->m_mat_ptr[k][net.railstart()] - &this->mat.A[0];
#if false
                    string terms = new plib.pfmt("m_A{0} = gt[{1}]").op(pd.Offset, this.m_gtn.didx(k,0));
                    for (size_t i = 1; i < net.count(); i++)
                        terms += new plib.pfmt(" + gt[{0}]").op(this.m_gtn.didx(k,i));

                    strm += string.Format("\t{0};\n", terms);
#else
                    for (size_t i = 0; i < net.count(); i++)
                        strm += string.Format("\tm_A{0} += gt[{1}];\n", pd, this.m_gtn.didx(k,i));
#endif

                    //for (std::size_t i = 0; i < railstart; i++)
                    //  *tcr_r[i]       += static_cast<FT>(go[i]);

                    for (size_t i = 0; i < net.railstart(); i++)
                    {
                        var p = this.m_mat_ptr[k][i] - new Pointer<nl_fptype>(this.mat.A, 0);
                        strm += string.Format("\tm_A{0} += go[{1}];\n", p, this.m_gonn.didx(k,i));
                    }

#if false
                    //auto RHS_t(std::accumulate(Idr, Idr + term_count, plib::constants<FT>::zero()));

                    terms = new plib.pfmt("{0} RHS{1} = Idr[{2}]").op(fptype, k, this.m_Idrn.didx(k,0));
                    for (size_t i = 1; i < net.count(); i++)
                        terms += new plib.pfmt(" + Idr[{0}]").op(this.m_Idrn.didx(k,i));
                    //for (std::size_t i = railstart; i < term_count; i++)
                    //  RHS_t +=  (- go[i]) * *cnV[i];

                    for (size_t i = net.railstart(); i < net.count(); i++)
                        terms += new plib.pfmt(" - go[{0}] * *cnV[{1}]").op(this.m_gonn.didx(k,i), this.m_connected_net_Vn.didx(k,i));

                    strm += string.Format("\t{0};\n", terms);
#else
                    //auto RHS_t(std::accumulate(Idr, Idr + term_count, plib::constants<FT>::zero()));
                    strm += string.Format("\t{0} RHS{1} = Idr[{2}];\n", fptype, k, this.m_Idrn.didx(k,0));
                    for (size_t i=1; i < net.count(); i++)
                        strm += string.Format("\tRHS{0} += Idr[{1}];\n", k, this.m_Idrn.didx(k,i));
                    //for (std::size_t i = railstart; i < term_count; i++)
                    //  RHS_t +=  (- go[i]) * *cnV[i];

                    for (size_t i = net.railstart(); i < net.count(); i++)
                        strm += string.Format("\tRHS{0} -= go[{1}] * *cnV[{2}];\n", k, this.m_gonn.didx(k,i), this.m_connected_net_Vn.didx(k,i));
#endif
#endif
                }

                for (size_t i = 0; i < iN - 1; i++)
                {
                    throw new emu_unimplemented();
#if false
                    //const auto &nzbd = this->m_terms[i].m_nzbd;
                    var nzbd = mat.nzbd(i);
                    var nzbd_count = mat.nzbd_count(i);

                    if (nzbd_count > 0)
                    {
                        size_t pi = mat.diag[i];

                        //const FT f = 1.0 / m_A[pi++];
                        strm += string.Format("\tconst {0} f{1} = 1.0{2} / m_A{3};\n", fptype, i, fpsuffix, pi);
                        pi++;
                        size_t piie = mat.row_idx[i+1];

                        //for (auto & j : nzbd)
                        for (size_t jj = 0; jj < nzbd_count; jj++)
                        {
                            size_t j = nzbd[jj];

                            // proceed to column i
                            size_t pj = mat.row_idx[j];

                            while (mat.col_idx[pj] < i)
                                pj++;

                            //const FT f1 = - m_A[pj++] * f;
                            strm += string.Format("\tconst {0} f{1}_{2} = -f{3} * m_A{4};\n", fptype, i, j, i, pj);
                            pj++;

                            // subtract row i from j
                            for (size_t pii = pi; pii < piie; )
                            {
                                while (mat.col_idx[pj] < mat.col_idx[pii])
                                    pj++;
                                //m_A[pj++] += m_A[pii++] * f1;
                                strm += string.Format("\tm_A{0} += m_A{1} * f{2}_{3};\n", pj, pii, i, j);
                                pj++; pii++;
                            }
                            //RHS[j] += f1 * RHS[i];
                            strm += string.Format("\tRHS{0} += f{1}_{2} * RHS{3};\n", j, i, j, i);
                        }
                    }
#endif
                }

                //new_V[iN - 1] = RHS[iN - 1] / mat.A[mat.diag[iN - 1]];
                strm += string.Format("\tV[{0}] = RHS{1} / m_A{2};\n", iN - 1, iN - 1, mat.diag[iN - 1]);
                for (size_t j = iN - 1; j-- > 0;)
                {
#if true
                    strm += string.Format("\t{0} tmp{1} = 0.0{2};\n", fptype, j, fpsuffix);
                    size_t e = mat.row_idx[j + 1];
                    for (size_t pk = (size_t)mat.diag[j] + 1; pk < e; pk++)
                    {
                        strm += string.Format("\ttmp{0} += m_A{1} * V[{2}];\n", j, pk, mat.col_idx[pk]);
                    }
                    strm += string.Format("\tV[{0}] = (RHS{0} - tmp{0}) / m_A{3};\n", j, j, j, mat.diag[j]);
#else
                    pstring tmp;
                    const std::size_t e = mat.row_idx[j+1];
                    for (std::size_t pk = mat.diag[j] + 1; pk < e; pk++)
                    {
                        tmp = tmp + plib::pfmt(" + m_A{2} * V[{3}]")(j, pk, mat.col_idx[pk]);
                    }
                    if (tmp.empty())
                    {
                        strm("\tV[{1}] = RHS{1} / m_A{2};\n", j, mat.diag[j]);
                    }
                    else
                    {
                        //strm("\tconst {1} tmp{2} = {3};\n", fptype, j, tmp.substr(3));
                        //strm("\tV[{1}] = (RHS{1} - tmp{1}) / m_A{2};\n", j, mat.diag[j]);
                        strm("\tV[{1}] = (RHS{1} - ({2})) / m_A{3};\n", j, tmp.substr(3), mat.diag[j]);
                    }
#endif
                }
            }


            //template <typename FT, int SIZE>
            string static_compile_name()
            {
                string str_floattype = fp_constants_double.name();  //pstring str_floattype(fp_constants<FT>::name());
                string str_fptype = fp_constants_double.name();  //pstring str_fptype(fp_constants<fptype>::name());
                //std::stringstream t;
                //t.imbue(std::locale::classic());
                //plib::putf8_fmt_writer w(&t);
                string w;
                generate_code(out w);
                var t = w;

                ////std::hash<typename std::remove_const<std::remove_reference<decltype(t.str())>::type>::type> h;
                return new plib.pfmt("nl_gcr_{0:x}_{1}_{2}_{3}").op(plib.pglobal.hash(t.str().c_str(), (size_t)t.str().size()), mat.nz_num, str_fptype, str_floattype);
            }
        }
    }
}
