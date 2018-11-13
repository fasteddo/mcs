// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.std_vector<mame.netlist.analog_net_t>;
using netlist_time = mame.netlist.ptime_u64;  //using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;
using nl_double = System.Double;


namespace mame.netlist
{
    namespace devices
    {
        /* FIXME: these should become proper devices */

        struct solver_parameters_t
        {
            public int m_pivot;
            public nl_double m_accuracy;
            public nl_double m_dynamic_lte;
            public nl_double m_min_timestep;
            public nl_double m_max_timestep;
            public nl_double m_gs_sor;
            public bool m_dynamic_ts;
            public UInt32 m_gs_loops;
            public UInt32 m_nr_loops;
            public netlist_time m_nr_recalc_delay;
            public bool m_log_stats;
        }


        class terms_for_net_t //: plib::nocopyassignmove
        {
            UInt32 m_railstart;

            std_vector<UInt32> m_nz = new std_vector<UInt32>();   /* all non zero for multiplication */
            std_vector<UInt32> m_nzrd = new std_vector<UInt32>(); /* non zero right of the diagonal for elimination, may include RHS element */
            std_vector<UInt32> m_nzbd = new std_vector<UInt32>(); /* non zero below of the diagonal for elimination */

            /* state */
            nl_double m_last_V;
            nl_double m_DD_n_m_1;
            nl_double m_h_n_m_1;


            std_vector<int> m_connected_net_idx = new std_vector<int>();
            std_vector<nl_double> m_go = new std_vector<nl_double>();
            std_vector<nl_double> m_gt = new std_vector<nl_double>();
            std_vector<nl_double> m_Idr = new std_vector<nl_double>();
            std_vector<doubleref> m_connected_net_V = new std_vector<doubleref>();  //std::vector<nl_double *> m_connected_net_V;
            std_vector<terminal_t> m_terms = new std_vector<terminal_t>();


            public terms_for_net_t()
            {
                m_railstart = 0;
                m_last_V = 0.0;
                m_DD_n_m_1 = 0.0;
                m_h_n_m_1 = 1e-9;
            }


            public UInt32 railstart { get { return m_railstart; } set { m_railstart = value; } }
            public std_vector<UInt32> nz { get { return m_nz; } }
            public std_vector<UInt32> nzrd { get { return m_nzrd; } set { m_nzrd = value; } }
            public std_vector<UInt32> nzbd { get { return m_nzbd; } }
            public nl_double last_V { get { return m_last_V; } }
            public nl_double DD_n_m_1 { get { return m_DD_n_m_1; } }
            public nl_double h_n_m_1 { get { return m_h_n_m_1; } }


            public void clear()
            {
                m_terms.clear();
                m_connected_net_idx.clear();
                m_gt.clear();
                m_go.clear();
                m_Idr.clear();
                m_connected_net_V.clear();
            }


            public void add(terminal_t term, int net_other, bool sorted)
            {
                if (sorted)
                {
                    for (int i = 0; i < m_connected_net_idx.size(); i++)
                    {
                        if (m_connected_net_idx[i] > net_other)
                        {
                            m_terms.Insert(i, term);  //plib::container::insert_at(m_terms, i, term);
                            m_connected_net_idx.Insert(i, net_other);  //plib::container::insert_at(m_connected_net_idx, i, net_other);
                            m_gt.Insert(i, 0.0);  //plib::container::insert_at(m_gt, i, 0.0);
                            m_go.Insert(i, 0.0);  //plib::container::insert_at(m_go, i, 0.0);
                            m_Idr.Insert(i, 0.0);  //plib::container::insert_at(m_Idr, i, 0.0);
                            m_connected_net_V.Insert(i, null);  //plib::container::insert_at(m_connected_net_V, i, null);
                            return;
                        }
                    }
                }

                m_terms.push_back(term);
                m_connected_net_idx.push_back(net_other);
                m_gt.push_back(0.0);
                m_go.push_back(0.0);
                m_Idr.push_back(0.0);
                m_connected_net_V.push_back(null);
            }


            public UInt32 count() { return (UInt32)m_terms.size(); }

            public std_vector<terminal_t> terms() { return m_terms; }  //inline terminal_t **terms() { return m_terms.data(); }
            public std_vector<int> connected_net_idx() { return m_connected_net_idx; }  //inline int *connected_net_idx() { return m_connected_net_idx.data(); }
            public std_vector<nl_double> gt() { return m_gt; }  //inline nl_double *gt() { return m_gt.data(); }
            public std_vector<nl_double> go() { return m_go; }  //inline nl_double *go() { return m_go.data(); }
            public std_vector<nl_double> Idr() { return m_Idr; }  //inline nl_double *Idr() { return m_Idr.data(); }
            //inline nl_double * const *connected_net_V() const { return m_connected_net_V.data(); }

            public void set_pointers()
            {
                for (UInt32 i = 0; i < count(); i++)
                {
                    m_terms[i].set_ptrs(new ListPointer<nl_double>(m_gt, (int)i), new ListPointer<nl_double>(m_go, (int)i), new ListPointer<nl_double>(m_Idr, (int)i));  //m_terms[i]->set_ptrs(&m_gt[i], &m_go[i], &m_Idr[i]);
                    m_connected_net_V[i] = m_terms[i].otherterm.net().Q_Analog_state_ptr();
                }
            }
        }


        class proxied_analog_output_t : analog_output_t
        {
            analog_net_t m_proxied_net; // only for proxy nets in analog input logic


            public proxied_analog_output_t(core_device_t dev, string aname)
                : base(dev, aname)
            {
                m_proxied_net = null;
            }
            
            ~proxied_analog_output_t() { }


            public analog_net_t proxied_net { get { return m_proxied_net; } set { m_proxied_net = value; } }
        }


        abstract class matrix_solver_t : device_t
        {
            //using list_t = std::vector<matrix_solver_t *>;


            protected enum eSortType
            {
                NOSORT,
                ASCENDING,
                DESCENDING
            }


            std_vector<terms_for_net_t> m_terms = new std_vector<terms_for_net_t>();  //std::vector<std::unique_ptr<terms_for_net_t>> m_terms;
            std_vector<analog_net_t> m_nets = new std_vector<analog_net_t>();
            std_vector<proxied_analog_output_t> m_inps = new std_vector<proxied_analog_output_t>();  //std::vector<std::unique_ptr<proxied_analog_output_t>> m_inps;

            std_vector<terms_for_net_t> m_rails_temp = new std_vector<terms_for_net_t>();

            solver_parameters_t m_params;

            state_var<int> m_stat_calculations;
            state_var<int> m_stat_newton_raphson;
            state_var<int> m_stat_vsolver_calls;
            state_var<int> m_iterative_fail;
            state_var<int> m_iterative_total;


            state_var<netlist_time> m_last_step;
            std_vector<core_device_t> m_step_devices = new std_vector<core_device_t>();
            std_vector<core_device_t> m_dynamic_devices = new std_vector<core_device_t>();

            logic_input_t m_fb_sync;
            logic_output_t m_Q_sync;


            UInt32 m_ops;
            eSortType m_sort;


            // ----------------------------------------------------------------------------------------
            // matrix_solver
            // ----------------------------------------------------------------------------------------
            protected matrix_solver_t(netlist_t anetlist, string name, eSortType sort, solver_parameters_t params_)
                : base(anetlist, name)
            {
                m_params = params_;
                m_stat_calculations = new state_var<int>(this, "m_stat_calculations", 0);
                m_stat_newton_raphson = new state_var<int>(this, "m_stat_newton_raphson", 0);
                m_stat_vsolver_calls = new state_var<int>(this, "m_stat_vsolver_calls", 0);
                m_iterative_fail = new state_var<int>(this, "m_iterative_fail", 0);
                m_iterative_total = new state_var<int>(this, "m_iterative_total", 0);
                m_last_step = new state_var<netlist_time>(this, "m_last_step", netlist_time.zero());
                m_fb_sync = new logic_input_t(this, "FB_sync");
                m_Q_sync = new logic_output_t(this, "Q_sync");
                m_ops = 0;
                m_sort = sort;


                connect_post_start(m_fb_sync, m_Q_sync);
            }

            ~matrix_solver_t() { }


            public std_vector<terms_for_net_t> terms { get { return m_terms; } }


            public void setup(analog_net_t_list_t nets)
            {
                vsetup(nets);
            }

            //void solve_base();

            /* after every call to solve, update inputs must be called.
             * this can be done as well as a batch to ease parallel processing.
             */
            //const netlist_time solve();
            //void update_inputs();

            public bool has_dynamic_devices() { return m_dynamic_devices.size() > 0; }
            public bool has_timestep_devices() { return m_step_devices.size() > 0; }


            public void update_forced()
            {
                throw new emu_unimplemented();
#if false
                netlist_time new_timestep = solve();
                update_inputs();

                if (m_params.m_dynamic_ts && has_timestep_devices())
                {
                    m_Q_sync.net().toggle_and_push_to_queue(netlist_time.from_double(m_params.m_min_timestep));
                }
#endif
            }


            //void update_after(const netlist_time &after)
            //{
            //    m_Q_sync.net().toggle_and_push_to_queue(after);
            //}


            /* netdevice functions */
            //NETLIB_UPDATEI();
            protected override void update()
            {
                throw new emu_unimplemented();
            }


            //NETLIB_RESETI();
            protected override void reset()
            {
                m_last_step.op = netlist_time.zero();
            }


            int get_net_idx(detail.net_t net)
            {
                for (UInt32 k = 0; k < m_nets.size(); k++)
                {
                    if (m_nets[k] == net)
                        return (int)k;
                }
                return -1;
            }


            protected virtual void log_stats()
            {
                throw new emu_unimplemented();
            }


            protected virtual KeyValuePair<string, string> create_solver_code()
            {
                return new KeyValuePair<string, string>("", new plib.pfmt("/* solver doesn't support static compile */\n\n").op());
            }


            /* return number of floating point operations for solve */
            //std::size_t ops() { return m_ops; }


            protected void setup_base(analog_net_t_list_t nets)
            {
                log().debug.op("New solver setup\n");

                m_nets.clear();
                m_terms.clear();

                foreach (var net in nets)
                {
                    m_nets.push_back(net);
                    m_terms.push_back(new terms_for_net_t());
                    m_rails_temp.push_back(new terms_for_net_t());
                }

                for (UInt32 k = 0; k < nets.size(); k++)
                {
                    analog_net_t net = nets[k];

                    log().debug.op("setting up net\n");

                    net.set_solver(this);

                    foreach (var p in net.core_terms)
                    {
                        log().debug.op("{0} {1} {2}\n", p.name(), net.name(), net.isRailNet());

                        switch (p.type())
                        {
                            case detail.terminal_type.TERMINAL:
                                if (p.device().is_timestep())
                                {
                                    if (!m_step_devices.Contains(p.device()))  //(!plib::container::contains(m_step_devices, &p->device()))
                                        m_step_devices.push_back(p.device());
                                }

                                if (p.device().is_dynamic())
                                {
                                    if (!m_dynamic_devices.Contains(p.device()))  //if (!plib::container::contains(m_dynamic_devices, &p->device()))
                                        m_dynamic_devices.push_back(p.device());
                                }

                                {
                                    terminal_t pterm = (terminal_t)p;
                                    add_term(k, pterm);
                                }

                                log().debug.op("Added terminal {0}\n", p.name());
                                break;

                            case detail.terminal_type.INPUT:
                                {
                                    proxied_analog_output_t net_proxy_output = null;
                                    foreach (var input in m_inps)
                                    {
                                        if (input.proxied_net == p.net())
                                        {
                                            net_proxy_output = input;
                                            break;
                                        }
                                    }

                                    if (net_proxy_output == null)
                                    {
                                        string nname = this.name() + "." + new plib.pfmt("m{0}").op(m_inps.size());
                                        var net_proxy_output_u = new proxied_analog_output_t(this, nname);
                                        net_proxy_output = net_proxy_output_u;
                                        m_inps.push_back(net_proxy_output_u);
                                        nl_base_global.nl_assert(p.net().is_analog());
                                        net_proxy_output.proxied_net = (analog_net_t)p.net();
                                    }

                                    net_proxy_output.net().add_terminal(p);

                                    // FIXME: repeated calling - kind of brute force
                                    net_proxy_output.net().rebuild_list();

                                    log().debug.op("Added input\n");
                                }
                                break;

                            case detail.terminal_type.OUTPUT:
                                log().fatal.op(nl_errstr_global.MF_1_UNHANDLED_ELEMENT_1_FOUND, p.name());
                                break;
                        }
                    }

                    log().debug.op("added net with {0} populated connections\n", net.core_terms.size());
                }

                /* now setup the matrix */
                setup_matrix();
            }


            //void update_dynamic();


            protected abstract void vsetup(analog_net_t_list_t nets);
            protected abstract UInt32 vsolve_non_dynamic(bool newton_raphson);

            //netlist_time compute_next_timestep(const double cur_ts);
            /* virtual */ void add_term(UInt32 k, terminal_t term)
            {
                if (term.otherterm.net().isRailNet())
                {
                    m_rails_temp[k].add(term, -1, false);
                }
                else
                {
                    int ot = get_net_idx(term.otherterm.net());
                    if (ot >= 0)
                    {
                        m_terms[k].add(term, ot, true);
                    }
                    /* Should this be allowed ? */
                    else // if (ot<0)
                    {
                        m_rails_temp[k].add(term, ot, true);
                        log().fatal.op(nl_errstr_global.MF_1_FOUND_TERM_WITH_MISSING_OTHERNET, term.name());
                    }
                }
            }

            //template <typename T>
            //void store(const T * RESTRICT V);
            //template <typename T>
            //T delta(const T * RESTRICT V);

            //template <typename T>
            //void build_LE_A();
            //template <typename T>
            //void build_LE_RHS();


            /* calculate matrix */
            void setup_matrix()
            {
                UInt32 iN = (UInt32)m_nets.size();

                for (UInt32 k = 0; k < iN; k++)
                {
                    m_terms[k].railstart = m_terms[k].count();
                    for (UInt32 i = 0; i < m_rails_temp[k].count(); i++)
                        this.m_terms[k].add(m_rails_temp[k].terms()[i], m_rails_temp[k].connected_net_idx()[i], false);

                    m_terms[k].set_pointers();
                }

                foreach (terms_for_net_t rt in m_rails_temp)
                {
                    rt.clear(); // no longer needed
                    //plib::pfree(rt); // no longer needed
                }

                m_rails_temp.clear();

                /* Sort in descending order by number of connected matrix voltages.
                 * The idea is, that for Gauss-Seidel algo the first voltage computed
                 * depends on the greatest number of previous voltages thus taking into
                 * account the maximum amout of information.
                 *
                 * This actually improves performance on popeye slightly. Average
                 * GS computations reduce from 2.509 to 2.370
                 *
                 * Smallest to largest : 2.613
                 * Unsorted            : 2.509
                 * Largest to smallest : 2.370
                 *
                 * Sorting as a general matrix pre-conditioning is mentioned in
                 * literature but I have found no articles about Gauss Seidel.
                 *
                 * For Gaussian Elimination however increasing order is better suited.
                 * NOTE: Even better would be to sort on elements right of the matrix diagonal.
                 *
                 */

                if (m_sort != eSortType.NOSORT)
                {
                    int sort_order = (m_sort == eSortType.DESCENDING ? 1 : -1);

                    for (UInt32 k = 0; k < iN - 1; k++)
                    {
                        for (UInt32 i = k + 1; i < iN; i++)
                        {
                            if (((int)(m_terms[k].railstart) - (int)(m_terms[i].railstart)) * sort_order < 0)
                            {
                                //std::swap(m_terms[i], m_terms[k]);
                                var termsTemp = m_terms[i];
                                m_terms[i] = m_terms[k];
                                m_terms[k] = termsTemp;
                                //std::swap(m_nets[i], m_nets[k]);
                                var netsTemp = m_nets[i];
                                m_nets[i] = m_nets[k];
                                m_nets[k] = netsTemp;
                            }
                        }
                    }

                    foreach (var term in m_terms)
                    {
                        var other = term.connected_net_idx();
                        for (UInt32 i = 0; i < term.count(); i++)
                        {
                            if (other[i] != -1)
                                other[i] = get_net_idx(term.terms()[i].otherterm.net());
                        }
                    }
                }

                /* create a list of non zero elements. */
                for (UInt32 k = 0; k < iN; k++)
                {
                    terms_for_net_t t = m_terms[k];
                    /* pretty brutal */
                    var other = t.connected_net_idx();

                    t.nz.clear();

                    for (UInt32 i = 0; i < t.railstart; i++)
                    {
                        if (!t.nz.Contains((UInt32)other[i]))  //if (!plib::container::contains(t->m_nz, static_cast<unsigned>(other[i])))
                            t.nz.push_back((UInt32)other[i]);
                    }

                    t.nz.push_back(k);     // add diagonal

                    /* and sort */
                    t.nz.Sort();  //std::sort(t.m_nz.begin(), t.m_nz.end());
                }

                /* create a list of non zero elements right of the diagonal
                 * These list anticipate the population of array elements by
                 * Gaussian elimination.
                 */
                for (UInt32 k = 0; k < iN; k++)
                {
                    terms_for_net_t t = m_terms[k];
                    /* pretty brutal */
                    var other = t.connected_net_idx();

                    if (k == 0)
                    {
                        t.nzrd.clear();
                    }
                    else
                    {
                        t.nzrd = m_terms[k - 1].nzrd;
                        for (var jIdx = 0; jIdx < t.nzrd.Count;  )  //for (var j = t.nzrd.begin(); j != t.nzrd.end(); )
                        {
                            var j = t.nzrd[jIdx];

                            if (j < k + 1)
                                t.nzrd.erase(jIdx);
                            else
                                ++jIdx;
                        }
                    }

                    for (UInt32 i = 0; i < t.railstart; i++)
                    {
                        if (!t.nzrd.Contains((UInt32)other[i]) && other[i] >= (int)(k + 1))  //if (!plib::container::contains(t->m_nzrd, static_cast<unsigned>(other[i])) && other[i] >= static_cast<int>(k + 1))
                            t.nzrd.push_back((UInt32)other[i]);
                    }

                    /* and sort */
                    t.nzrd.Sort();  //std::sort(t.m_nzrd.begin(), t.m_nzrd.end());
                }

                /* create a list of non zero elements below diagonal k
                 * This should reduce cache misses ...
                 */

                bool [,] touched = new bool [iN, iN];  //bool **touched = plib::palloc_array<bool *>(iN);
                //for (UInt32 k = 0; k < iN; k++)
                //    touched[k] = plib::palloc_array<bool>(iN);

                for (UInt32 k = 0; k < iN; k++)
                {
                    for (UInt32 j = 0; j < iN; j++)
                        touched[k, j] = false;

                    for (UInt32 j = 0; j < m_terms[k].nz.size(); j++)
                        touched[k, m_terms[k].nz[j]] = true;
                }

                m_ops = 0;
                for (UInt32 k = 0; k < iN; k++)
                {
                    m_ops++; // 1/A(k,k)
                    for (UInt32 row = k + 1; row < iN; row++)
                    {
                        if (touched[row, k])
                        {
                            m_ops++;
                            if (!m_terms[k].nzbd.Contains(row))  //if (!plib::container::contains(m_terms[k]->m_nzbd, row))
                                m_terms[k].nzbd.push_back(row);

                            for (UInt32 col = k + 1; col < iN; col++)
                            {
                                if (touched[k, col])
                                {
                                    touched[row, col] = true;
                                    m_ops += 2;
                                }
                            }
                        }
                    }
                }

                log().verbose.op("Number of mults/adds for {0}: {1}", name(), m_ops);

#if false
                if ((0))
                    for (unsigned k = 0; k < iN; k++)
                    {
                        pstring line = plib::pfmt("{1:3}")(k);
                        for (unsigned j = 0; j < m_terms[k]->m_nzrd.size(); j++)
                            line += plib::pfmt(" {1:3}")(m_terms[k]->m_nzrd[j]);
                        log().verbose("{1}", line);
                    }
#endif

                /*
                 * save states
                 */
                for (UInt32 k = 0; k < iN; k++)
                {
                    string num = new plib.pfmt("{0}").op(k);

                    netlist().save(this, m_terms[k].last_V, "lastV." + num);
                    netlist().save(this, m_terms[k].DD_n_m_1, "m_DD_n_m_1." + num);
                    netlist().save(this, m_terms[k].h_n_m_1, "m_h_n_m_1." + num);

                    netlist().save(this, m_terms[k].go(),"GO" + num, m_terms[k].count());
                    netlist().save(this, m_terms[k].gt(),"GT" + num, m_terms[k].count());
                    netlist().save(this, m_terms[k].Idr(),"IDR" + num , m_terms[k].count());
                }

                //for (UInt32 k = 0; k < iN; k++)
                //    plib::pfree_array(touched[k]);
                //plib::pfree_array(touched);
                touched = null;
            }


            //void step(const netlist_time &delta);
        }
    } //namespace devices
} // namespace netlist
