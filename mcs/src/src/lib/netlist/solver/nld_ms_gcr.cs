// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.std.vector<mame.netlist.analog_net_t>;
using mat_index_type = System.UInt16;  //using mat_index_type = typename plib::matrix_compressed_rows_t<FT, SIZE>::index_type;
using netlist_base_t = mame.netlist.netlist_state_t;
using uint16_t = System.UInt16;


namespace mame.netlist
{
    namespace devices
    {
        //template <typename FT, int SIZE>
        class matrix_solver_GCR_t : matrix_solver_t
        {
            //using mat_type = plib::pGEmatrix_cr_t<plib::pmatrix_cr_t<FT, SIZE>>;
            //using mat_index_type = typename plib::pmatrix_cr_t<FT, SIZE>::index_type;


            // FIXME: dirty hack to make this compile
            const int storage_N = 100;


            // template parameters
            int SIZE;


            UInt32 m_dim;
            double [] RHS;  //plib::parray<FT, SIZE> RHS;
            double [] new_V;  //plib::parray<FT, SIZE> new_V;

            plib.pGEmatrix_cr_t_pmatrix_cr_t_double_uint16 mat;  //mat_type mat;

            delegate void procCallback(double m_A, double RHS, double V);
            procCallback m_proc;  //plib::dynproc<void, double * , double * , double * > m_proc;


            public matrix_solver_GCR_t(int SIZE, netlist_state_t anetlist, string name, solver_parameters_t params_, UInt32 size)
                : base(anetlist, name, params_)
            {
                this.SIZE = SIZE;


                m_dim = size;
                RHS = new double [size];
                new_V = new double [size];
                mat = new plib.pGEmatrix_cr_t_pmatrix_cr_t_double_uint16(SIZE, (uint16_t)size);  //mat(static_cast<typename mat_type::index_type>(size))
                m_proc = null;
            }

            //~matrix_solver_GCR_t() { }


            UInt32 N() { return m_dim; }


            // ----------------------------------------------------------------------------------------
            // matrix_solver - GCR
            // ----------------------------------------------------------------------------------------

            //template <typename FT, int SIZE>
            protected override void vsetup(analog_net_t_list_t nets)
            {
                setup_base(nets);

                UInt32 iN = this.N();

                /* build the final matrix */

                std.vector<std.vector<UInt32>> fill = new std.vector<std.vector<UInt32>>(iN);

                UInt32 raw_elements = 0;

                for (UInt32 k = 0; k < iN; k++)
                {
                    fill[k] = new std.vector<UInt32>();
                    fill[k].resize((int)iN, (UInt32)plib.pGEmatrix_cr_t_pmatrix_cr_t_double_uint16.constants_e.FILL_INFINITY);  //decltype(mat)::FILL_INFINITY);
                    foreach (var j in this.terms[k].nz)
                    {
                        fill[k][j] = 0;
                        raw_elements++;
                    }
                }

                var gr = mat.gaussian_extend_fill_mat(fill);

                log_fill(fill, mat);

                mat.build_from_fill_mat(fill);

                for (mat_index_type k = 0; k < iN; k++)
                {
                    UInt32 cnt = 0;
                    /* build pointers into the compressed row format matrix for each terminal */
                    for (UInt32 j = 0; j < this.terms[k].railstart; j++)
                    {
                        int other = this.terms[k].connected_net_idx[j];
                        for (var i = mat.row_idx[k]; i < mat.row_idx[k + 1]; i++)
                        {
                            if (other == (int)mat.col_idx[i])
                            {
                                m_mat_ptr.op(k)[j] = new ListPointer<double>(mat.A, i);  //m_mat_ptr[k][j] = &mat.A[i];
                                cnt++;
                                break;
                            }
                        }
                    }

                    nl_base_global.nl_assert(cnt == this.terms[k].railstart);
                    m_mat_ptr.op(k)[this.m_terms[k].railstart] = new ListPointer<double>(mat.A, mat.diag[k]);  //m_mat_ptr[k][this->m_terms[k]->m_railstart] = &mat.A[mat.diag[k]];
                }

                this.log().verbose.op("maximum fill: {0}", gr.first());
                this.log().verbose.op("Post elimination occupancy ratio: {1} Ops: {0}", gr.second(), (double)mat.nz_num / (double)(iN * iN));
                this.log().verbose.op(" Pre elimination occupancy ratio: {0}", (double)raw_elements / (double)(iN * iN));

                // FIXME: Move me

                if (state().lib().isLoaded())
                {
                    throw new emu_unimplemented();
#if false
                    pstring symname = static_compile_name();
                    m_proc.load(this->state().lib(), symname);
                    if (m_proc.resolved())
                    {
                        this->log().info("External static solver {1} found ...", symname);
                    }
                    else
                    {
                        this->log().warning("External static solver {1} not found ...", symname);
                    }
#endif
                }
            }


            protected override UInt32 vsolve_non_dynamic(bool newton_raphson)
            {
                throw new emu_unimplemented();
            }


            protected override KeyValuePair<string, string> create_solver_code()
            {
                throw new emu_unimplemented();
            }


            //void generate_code(plib::putf8_fmt_writer &strm);

            //string static_compile_name()
        }
    } //namespace devices
} // namespace netlist
