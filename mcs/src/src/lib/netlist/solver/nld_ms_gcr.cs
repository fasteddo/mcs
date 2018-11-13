// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.std_vector<mame.netlist.analog_net_t>;
using uint16_t = System.UInt16;


namespace mame.netlist
{
    namespace devices
    {
        //template <std::size_t m_N, std::size_t storage_N>
        class matrix_solver_GCR_t : matrix_solver_t
        {
            //typedef typename mat_cr_t<storage_N>::type mattype;
            //typedef typename mat_cr_t<storage_N>::index_type mattype;


            //using extsolver = void (*)(double * RESTRICT m_A, double * RESTRICT RHS, double * RESTRICT V);
            delegate void extsolver(double m_A, double RHS, double V);


            // template parameters
            UInt32 m_N;
            UInt32 storage_N;


            UInt32 m_dim;
            std_vector<UInt32> [] m_term_cr;//[storage_N];  //std::vector<unsigned> m_term_cr[storage_N];
            mat_cr_t mat;  //mat_cr_t<storage_N> mat;

            extsolver m_proc;
            //plib::dynproc<void, double * RESTRICT, double * RESTRICT, double * RESTRICT> m_proc;


            public matrix_solver_GCR_t(UInt32 m_N, UInt32 storage_N, netlist_t anetlist, string name, solver_parameters_t params_, UInt32 size)
                : base(anetlist, name, matrix_solver_t.eSortType.ASCENDING, params_)
            {
                this.m_N = m_N;
                this.storage_N = storage_N;


                m_term_cr = new std_vector<UInt32>[storage_N];
                for (int i = 0; i < m_term_cr.Length; i++)
                    m_term_cr[i] = new std_vector<uint>();


                m_dim = size;
                mat = new mat_cr_t(storage_N, size);
                m_proc = null;
            }

            ~matrix_solver_GCR_t() { }


            UInt32 N() { return (m_N == 0) ? m_dim : m_N; }


            protected override void vsetup(analog_net_t_list_t nets)
            {
                setup_base(nets);

                uint16_t /*mattype*/ nz = 0;
                UInt32 iN = this.N();

                /* build the final matrix */

                bool [,] touched = new bool[storage_N, storage_N];  // = { { false } };  //bool touched[storage_N][storage_N] = { { false } };
                for (UInt32 k = 0; k < iN; k++)
                {
                    foreach (var j in this.terms[k].nz)
                        touched[k, j] = true;
                }

                UInt32 fc = 0;

                UInt32 ops = 0;

                for (UInt32 k = 0; k < iN; k++)
                {
                    ops++; // 1/A(k,k)
                    for (UInt32 row = k + 1; row < iN; row++)
                    {
                        if (touched[row, k])
                        {
                            ops++;
                            fc++;
                            for (UInt32 col = k + 1; col < iN; col++)
                            {
                                if (touched[k, col])
                                {
                                    touched[row, col] = true;
                                    ops += 2;
                                }
                            }
                        }
                    }
                }


                for (uint16_t /*mattype*/ k = 0; k < iN; k++)
                {
                    mat.ia[k] = nz;

                    for (uint16_t /*mattype*/ j = 0; j < iN; j++)
                    {
                        if (touched[k, j])
                        {
                            mat.ja[nz] = j;
                            if (j == k)
                                mat.diag[k] = nz;
                            nz++;
                        }
                    }

                    m_term_cr[k].clear();
                    /* build pointers into the compressed row format matrix for each terminal */
                    for (UInt32 j = 0; j < this.terms[k].railstart; j++)
                    {
                        int other = this.terms[k].connected_net_idx()[j];
                        for (var i = mat.ia[k]; i < nz; i++)
                        {
                            if (other == (int)mat.ja[i])
                            {
                                m_term_cr[k].push_back(i);
                                break;
                            }
                        }
                    }

                    nl_base_global.nl_assert(m_term_cr[k].size() == this.terms[k].railstart);
                }

                mat.ia[iN] = nz;
                mat.nz_num = nz;

                this.log().verbose.op("Ops: {0}  Occupancy ratio: {1}\n", ops, (double)nz / (double)(iN * iN));

                // FIXME: Move me

                if (netlist().lib().isLoaded())
                {
                    throw new emu_unimplemented();
#if false

                    string symname = static_compile_name();
#if false
                    m_proc = this->netlist().lib().template getsym<extsolver>(symname);
                    if (m_proc != nullptr)
                        this->log().verbose("External static solver {1} found ...", symname);
                    else
                        this->log().warning("External static solver {1} not found ...", symname);
#else
                    m_proc.load(this.netlist().lib(), symname);
                    if (m_proc.resolved())
                        this.log().warning.op("External static solver {0} found ...", symname);
                    else
                        this.log().warning.op("External static solver {0} not found ...", symname);
#endif
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
