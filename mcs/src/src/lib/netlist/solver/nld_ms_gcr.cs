// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using mat_index_type = System.UInt16;  //using mat_index_type = typename plib::matrix_compressed_rows_t<FT, SIZE>::index_type;
using nl_fptype = System.Double;
using size_t = System.UInt32;
using uint16_t = System.UInt16;
using unsigned = System.UInt32;


namespace mame.netlist
{
    namespace solver
    {
        //template <typename FT, int SIZE>
        class matrix_solver_GCR_t : matrix_solver_ext_t  //class matrix_solver_GCR_t: public matrix_solver_ext_t<FT, SIZE>
        {
            //using mat_type = plib::pGEmatrix_cr_t<plib::pmatrix_cr_t<FT, SIZE>>;
            //using mat_index_type = typename plib::pmatrix_cr_t<FT, SIZE>::index_type;


            plib.pGEmatrix_cr_t_pmatrix_cr_t_double_uint16 mat;  //mat_type mat;
            plib.dynproc m_proc;  //plib::dynproc<void, FT *, nl_fptype *, nl_fptype *, nl_fptype *, nl_fptype ** > m_proc;


            public matrix_solver_GCR_t(int SIZE, netlist_state_t anetlist, string name, analog_net_t.list_t nets, solver_parameters_t params_, UInt32 size)
                : base(SIZE, anetlist, name, nets, params_, size)
            {
                mat = new plib.pGEmatrix_cr_t_pmatrix_cr_t_double_uint16(SIZE, (uint16_t)size);  //mat(static_cast<typename mat_type::index_type>(size))
                m_proc = new plib.dynproc();


                int iN = this.size();

                // build the final matrix

                std.vector<std.vector<unsigned>> fill = new std.vector<std.vector<unsigned>>(iN);
                fill.Fill(() => { return new std.vector<size_t>(); });

                size_t raw_elements = 0;

                for (size_t k = 0; k < iN; k++)
                {
                    fill[k].resize(iN, (unsigned)plib.pGEmatrix_cr_t_pmatrix_cr_t_double_uint16.constants_e.FILL_INFINITY);
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
                                this.m_mat_ptr[k][j] = new Pointer<nl_fptype>(mat.A, i);
                                cnt++;
                                break;
                            }
                        }
                    }
                    nl_config_global.nl_assert(cnt == this.m_terms[k].railstart());
                    this.m_mat_ptr[k][this.m_terms[k].railstart()] = new Pointer<nl_fptype>(mat.A, mat.diag[k]);
                }

                anetlist.log().verbose.op("maximum fill: {0}", gr.first);
                anetlist.log().verbose.op("Post elimination occupancy ratio: {1} Ops: {0}", gr.second,
                        (nl_fptype)mat.nz_num / (nl_fptype)(iN * iN));
                anetlist.log().verbose.op(" Pre elimination occupancy ratio: {0}",
                        (nl_fptype)raw_elements / (nl_fptype)(iN * iN));

                // FIXME: Move me
                //

                // During extended validation there is no reason to check for
                // differences in the generated code since during
                // extended validation this will be different (and non-functional)
                if (!anetlist.is_extended_validation() && anetlist.lib().isLoaded())
                {
                    string symname = static_compile_name();
                    m_proc.load(anetlist.lib(), symname);
                    if (m_proc.resolved())
                    {
                        anetlist.log().info.op("External static solver {0} found ...", symname);
                    }
                    else
                    {
                        anetlist.log().warning.op("External static solver {0} not found ...", symname);
                    }
                }
            }


            //template <typename FT, int SIZE>
            protected override void vsolve_non_dynamic()
            {
                if (m_proc.resolved())
                {
                    m_proc.op(new Pointer<nl_fptype>(this.m_new_V, 0),
                        this.m_gonn.data(), this.m_gtn.data(), this.m_Idrn.data(),
                        this.m_connected_net_Vn.data());
                }
                else
                {
                    //  clear matrix
                    mat.set_scalar(plib.constants.zero());

                    // populate matrix
                    this.fill_matrix_and_rhs();

                    // now solve it
                    // parallel is slow -- very slow
                    // mat.gaussian_elimination_parallel(RHS);
                    mat.gaussian_elimination(this.m_RHS);
                    // backward substitution
                    mat.gaussian_back_substitution(new Pointer<nl_fptype>(this.m_new_V), this.m_RHS);
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
                string fptype = fp_constants.name();
                string fpsuffix = fp_constants.suffix();

                for (size_t i = 0; i < mat.nz_num; i++)
                    strm += string.Format("{0} m_A{1}(0.0);\n", fptype, i, i);

                for (size_t k = 0; k < iN; k++)
                {
                    var net = this.m_terms[k];

                    // FIXME: gonn, gtn and Idr - which float types should they have?

                    //auto gtot_t = std::accumulate(gt, gt + term_count, plib::constants<FT>::zero());
                    //*tcr_r[railstart] = static_cast<FT>(gtot_t); //mat.A[mat.diag[k]] += gtot_t;

                    var pd = this.m_mat_ptr[k][net.railstart()] - new Pointer<nl_fptype>(this.mat.A, 0);  //auto pd = this->m_mat_ptr[k][net.railstart()] - &this->mat.A[0];
                    string terms = new plib.pfmt("m_A{0} = gt[{1}]").op(pd.Offset, this.m_gtn.didx(k,0));
                    for (size_t i = 1; i < net.count(); i++)
                        terms += new plib.pfmt(" + gt[{0}]").op(this.m_gtn.didx(k,i));

                    strm += string.Format("\t{0};\n", terms);

                    //for (std::size_t i = 0; i < railstart; i++)
                    //  *tcr_r[i]       += static_cast<FT>(go[i]);

                    for (size_t i = 0; i < net.railstart(); i++)
                    {
                        var p = this.m_mat_ptr[k][i] - new Pointer<nl_fptype>(this.mat.A, 0);
                        strm += string.Format("\tm_A{0} = m_A{0} + go[{1}];\n", p.Offset, this.m_gonn.didx(k,i));
                    }

                    //auto RHS_t(std::accumulate(Idr, Idr + term_count, plib::constants<FT>::zero()));

                    terms = new plib.pfmt("{0} RHS{1} = Idr[{2}]").op(fptype, k, this.m_Idrn.didx(k,0));
                    for (size_t i = 1; i < net.count(); i++)
                        terms += new plib.pfmt(" + Idr[{0}]").op(this.m_Idrn.didx(k,i));
                    //for (std::size_t i = railstart; i < term_count; i++)
                    //  RHS_t +=  (- go[i]) * *cnV[i];

                    for (size_t i = net.railstart(); i < net.count(); i++)
                        terms += new plib.pfmt(" - go[{0}] * *cnV[{0}]").op(this.m_gonn.didx(k,i), this.m_connected_net_Vn.didx(k,i));

                    strm += string.Format("\t{0};\n", terms);
                }

                for (size_t i = 0; i < iN - 1; i++)
                {
                    var nzbd = this.m_terms[i].m_nzbd;

                    if (!nzbd.empty())
                    {
                        size_t pi = mat.diag[i];

                        //const FT f = 1.0 / m_A[pi++];
                        strm += string.Format("const {0} f{1} = 1.0{2} / m_A{3};\n", fptype, i, fpsuffix, pi);
                        pi++;
                        size_t piie = mat.row_idx[i+1];

                        //for (auto & j : nzbd)
                        foreach (size_t j in nzbd)
                        {
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
                }

                //new_V[iN - 1] = RHS[iN - 1] / mat.A[mat.diag[iN - 1]];
                strm += string.Format("\tV[{0}] = RHS{1} / m_A{2};\n", iN - 1, iN - 1, mat.diag[iN - 1]);
                for (size_t j = iN - 1; j-- > 0;)
                {
                    strm += string.Format("\t{0} tmp{1} = 0.0{2};\n", fptype, j, fpsuffix);
                    size_t e = mat.row_idx[j + 1];
                    for (size_t pk = (size_t)mat.diag[j] + 1; pk < e; pk++)
                    {
                        strm += string.Format("\ttmp{0} += m_A{1} * V[{2}];\n", j, pk, mat.col_idx[pk]);
                    }
                    strm += string.Format("\tV[{0}] = (RHS{0} - tmp{0}) / m_A{3};\n", j, j, j, mat.diag[j]);
                }
            }


            //template <typename FT, int SIZE>
            string static_compile_name()
            {
                //std::stringstream t;
                //t.imbue(std::locale::classic());
                //plib::putf8_fmt_writer w(&t);
                string w;
                generate_code(out w);
                var t = w;

                ////std::hash<typename std::remove_const<std::remove_reference<decltype(t.str())>::type>::type> h;
                return new plib.pfmt("nl_gcr_{0:x}_{1}").op(plib.pglobal.hash(t.str().c_str(), (size_t)t.str().size()), mat.nz_num);
            }
        }
    }
}
