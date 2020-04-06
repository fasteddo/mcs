// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using mat_index_type = System.UInt16;  //using mat_index_type = typename plib::matrix_compressed_rows_t<FT, SIZE>::index_type;
using nl_fptype = System.Double;
using uint16_t = System.UInt16;


namespace mame.netlist
{
    namespace solver
    {
        //template <typename FT, int SIZE>
        abstract class matrix_solver_GCR_t : matrix_solver_ext_t  //class matrix_solver_GCR_t: public matrix_solver_ext_t<FT, SIZE>
        {
            //using mat_type = plib::pGEmatrix_cr_t<plib::pmatrix_cr_t<FT, SIZE>>;
            //using mat_index_type = typename plib::pmatrix_cr_t<FT, SIZE>::index_type;


            plib.pGEmatrix_cr_t_pmatrix_cr_t_double_uint16 mat;  //mat_type mat;

            delegate void procCallback(double m_A, double RHS, double V);
            procCallback m_proc;  //plib::dynproc<void, double * , double * , double * > m_proc;


            public matrix_solver_GCR_t(int SIZE, netlist_state_t anetlist, string name, analog_net_t.list_t nets, solver_parameters_t params_, UInt32 size)
                : base(SIZE, anetlist, name, nets, params_, size)
            {
                mat = new plib.pGEmatrix_cr_t_pmatrix_cr_t_double_uint16(SIZE, (uint16_t)size);  //mat(static_cast<typename mat_type::index_type>(size))
                m_proc = null;


                UInt32 iN = this.size();

                // build the final matrix

                std.vector<std.vector<UInt32>> fill = new std.vector<std.vector<UInt32>>(iN);

                UInt32 raw_elements = 0;

                for (UInt32 k = 0; k < iN; k++)
                {
                    fill[k].resize((int)iN, (UInt32)plib.pGEmatrix_cr_t_pmatrix_cr_t_double_uint16.constants_e.FILL_INFINITY);
                    foreach (var j in this.m_terms[k].m_nz)
                    {
                        fill[k][j] = 0;
                        raw_elements++;
                    }

                }

                var gr = mat.gaussian_extend_fill_mat(fill);

                this.log_fill(fill, mat);

                mat.build_from_fill_mat(fill);

                for (mat_index_type k=0; k<iN; k++)
                {
                    UInt32 cnt = 0;
                    // build pointers into the compressed row format matrix for each terminal
                    for (UInt32 j = 0; j < this.m_terms[k].railstart(); j++)
                    {
                        int other = this.m_terms[k].m_connected_net_idx[j];
                        for (var i = mat.row_idx[k]; i < mat.row_idx[k+1]; i++)
                        {
                            if (other == (int)mat.col_idx[i])
                            {
                                throw new emu_unimplemented();
#if false
                                this.m_mat_ptr[k][j] = &mat.A[i];
#endif
                                cnt++;
                                break;
                            }
                        }
                    }
                    nl_config_global.nl_assert(cnt == this.m_terms[k].railstart());

                    throw new emu_unimplemented();
#if false
                    this.m_mat_ptr[k][this.m_terms[k].railstart()] = &mat.A[mat.diag[k]];
#endif
                }

                anetlist.log().verbose.op("maximum fill: {0}", gr.first());
                anetlist.log().verbose.op("Post elimination occupancy ratio: {1} Ops: {0}", gr.second(),
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

                    throw new emu_unimplemented();
#if false
                    m_proc.load(anetlist.lib(), symname);
                    if (m_proc.resolved())
                    {
                        anetlist.log().info.op("External static solver {0} found ...", symname);
                    }
                    else
                    {
                        anetlist.log().warning.op("External static solver {0} not found ...", symname);
                    }
#endif
                }
            }


            protected override UInt32 vsolve_non_dynamic(bool newton_raphson)
            {
                throw new emu_unimplemented();
            }


            protected override std.pair<string, string> create_solver_code()
            {
                throw new emu_unimplemented();
            }


            //void generate_code(plib::putf8_fmt_writer &strm);


            //template <typename FT, int SIZE>
            string static_compile_name()
            {
                throw new emu_unimplemented();
#if false
                std::stringstream t;
                t.imbue(std::locale::classic());
                plib::putf8_fmt_writer w(&t);
                generate_code(w);
                std::hash<typename std::remove_const<std::remove_reference<decltype(t.str())>::type>::type> h;
                return plib::pfmt("nl_gcr_{1:x}_{2}")(h( t.str() ))(mat.nz_num);
#endif
            }
        }
    }
}
