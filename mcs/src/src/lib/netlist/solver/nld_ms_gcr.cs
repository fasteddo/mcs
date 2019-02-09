// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.std.vector<mame.netlist.analog_net_t>;
using mat_index_type = System.UInt16;  //plib::mat_cr_t<FT, SIZE>::index_type  //typedef C index_type;
using netlist_base_t = mame.netlist.netlist_state_t;
using uint16_t = System.UInt16;


namespace mame.netlist
{
    namespace devices
    {
        //template <typename FT, int SIZE>
        class matrix_solver_GCR_t : matrix_solver_t
        {
            //typedef typename mat_cr_t<storage_N>::type mattype;
            //typedef typename plib::mat_cr_t<FT, SIZE>::index_type mat_index_type;


            // FIXME: dirty hack to make this compile
            const int storage_N = 100;


            //using extsolver = void (*)(double * RESTRICT m_A, double * RESTRICT RHS, double * RESTRICT V);
            delegate void extsolver(double m_A, double RHS, double V);


            // template parameters
            int SIZE;


            UInt32 m_dim;
            double [] RHS;  //plib::parray<FT, SIZE> RHS;
            double [] new_V;  //plib::parray<FT, SIZE> new_V;

            std.vector<uint16_t> [] m_term_cr = new std.vector<uint16_t> [storage_N];  //std::vector<FT *> m_term_cr[storage_N];

            plib.mat_cr_t mat;  //plib::mat_cr_t<FT, SIZE> mat;

            extsolver m_proc;
            //plib::dynproc<void, double * RESTRICT, double * RESTRICT, double * RESTRICT> m_proc;


            public matrix_solver_GCR_t(int SIZE, netlist_base_t anetlist, string name, solver_parameters_t params_, UInt32 size)
                : base(anetlist, name, matrix_solver_t.eSortType.ASCENDING, params_)
            {
                this.SIZE = SIZE;


                for (int i = 0; i < m_term_cr.Length; i++)
                    m_term_cr[i] = new std.vector<uint16_t>();


                m_dim = size;
                RHS = new double [size];
                new_V = new double [size];
                mat = new plib.mat_cr_t(storage_N, size);
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

                std.vector<std.vector<UInt32>> fill = new std.vector<std.vector<uint>>(iN);

                UInt32 raw_elements = 0;

                for (UInt32 k = 0; k < iN; k++)
                {
                    fill[k] = new std.vector<UInt32>();
                    fill[k].resize((int)iN, plib.mat_cr_t.FILL_INFINITY);  //decltype(mat)::FILL_INFINITY);
                    foreach (var j in this.terms[k].nz)
                    {
                        fill[k][j] = 0;
                        raw_elements++;
                    }
                }

                var gr = mat.gaussian_extend_fill_mat(fill);

                for (UInt32 k = 0; k < iN; k++)
                {
                    UInt32 fm = 0;
                    string ml = "";
                    for (UInt32 j = 0; j < iN; j++)
                    {
                        ml += fill[k][j] < plib.mat_cr_t.FILL_INFINITY ? "X" : "_";
                        if (fill[k][j] < plib.mat_cr_t.FILL_INFINITY)
                        {
                            if (fill[k][j] > fm)
                                fm = fill[k][j];
                        }
                    }

                    this.log().verbose.op("{0} {1} {2}", k, ml, fm);
                }


                mat.build_from_fill_mat(fill);

                for (mat_index_type k = 0; k < iN; k++)
                {
                    m_term_cr[k].clear();
                    /* build pointers into the compressed row format matrix for each terminal */
                    for (UInt32 j = 0; j < this.terms[k].railstart; j++)
                    {
                        int other = this.terms[k].connected_net_idx()[j];
                        for (var i = mat.row_idx[k]; i < mat.row_idx[k + 1]; i++)
                        {
                            if (other == (int)mat.col_idx[i])
                            {
                                m_term_cr[k].push_back(mat.A[i]);
                                break;
                            }
                        }
                    }

                    nl_base_global.nl_assert(m_term_cr[k].size() == this.terms[k].railstart);
                    m_term_cr[k].push_back(mat.A[mat.diag[k]]);
                }

                this.log().verbose.op("maximum fill: {0}", gr.first());
                this.log().verbose.op("Post elimination occupancy ratio: {1} Ops: {0}", gr.second(), (double)mat.nz_num / (double)(iN * iN));
                this.log().verbose.op(" Pre elimination occupancy ratio: {0}", (double)raw_elements / (double)(iN * iN));

                // FIXME: Move me

                if (state().lib().isLoaded())
                {
                    throw new emu_unimplemented();
#if false
                    string symname = static_compile_name();
                    m_proc.load(this.state().lib(), symname);
                    if (m_proc.resolved())
                        this.log().warning("External static solver {0} found ...", symname);
                    else
                        this.log().warning("External static solver {0} not found ...", symname);
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


            //void csc_private(plib::putf8_fmt_writer &strm);

            //string static_compile_name()
        }
    } //namespace devices
} // namespace netlist
