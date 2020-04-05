// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.std.vector<mame.netlist.analog_net_t>;
using netlist_base_t = mame.netlist.netlist_state_t;
using netlist_time = mame.netlist.ptime_u64;  //using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;
using nl_double = System.Double;


namespace mame.netlist
{
    namespace devices
    {
        /* FIXME: these should become proper devices */

        struct solver_parameters_t
        {
            public bool m_pivot;
            public nl_double m_accuracy;
            public nl_double m_dynamic_lte;
            public nl_double m_min_timestep;
            public nl_double m_max_timestep;
            public nl_double m_gs_sor;
            public bool m_dynamic_ts;
            public UInt32 m_gs_loops;
            public UInt32 m_nr_loops;
            public netlist_time m_nr_recalc_delay;
            public bool m_use_gabs;
            public bool m_use_linear_prediction;
            public bool m_log_stats;
        }


        class terms_for_net_t //: plib::nocopyassignmove
        {
            UInt32 m_railstart;

            std.vector<UInt32> m_nz = new std.vector<UInt32>();   /* all non zero for multiplication */
            std.vector<UInt32> m_nzrd = new std.vector<UInt32>(); /* non zero right of the diagonal for elimination, may include RHS element */
            std.vector<UInt32> m_nzbd = new std.vector<UInt32>(); /* non zero below of the diagonal for elimination */

            /* state */
            nl_double m_last_V;
            nl_double m_DD_n_m_1;
            nl_double m_h_n_m_1;


            std.vector<int> m_connected_net_idx = new std.vector<int>();
            std.vector<terminal_t> m_terms = new std.vector<terminal_t>();


            public terms_for_net_t()
            {
                m_railstart = 0;
                m_last_V = 0.0;
                m_DD_n_m_1 = 0.0;
                m_h_n_m_1 = 1e-12;
            }


            public terms_for_net_t get() { return this; }  // for smart pointers

            public UInt32 railstart { get { return m_railstart; } set { m_railstart = value; } }
            public std.vector<UInt32> nz { get { return m_nz; } }
            public std.vector<UInt32> nzrd { get { return m_nzrd; } set { m_nzrd = value; } }
            public std.vector<UInt32> nzbd { get { return m_nzbd; } }
            public nl_double last_V { get { return m_last_V; } set { m_last_V = value; } }
            public nl_double DD_n_m_1 { get { return m_DD_n_m_1; } set { m_DD_n_m_1 = value; } }
            public nl_double h_n_m_1 { get { return m_h_n_m_1; } set { m_h_n_m_1 = value; } }
            public std.vector<int> connected_net_idx { get { return m_connected_net_idx; } }


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
                            return;
                        }
                    }
                }

                m_terms.push_back(term);
                m_connected_net_idx.push_back(net_other);
            }


            public UInt32 count() { return (UInt32)m_terms.size(); }  //std::size_t count() const { return m_terms.size(); }

            public std.vector<terminal_t> terms() { return m_terms; }  //inline terminal_t **terms() { return m_terms.data(); }
        }


        class proxied_analog_output_t : analog_output_t
        {
            analog_net_t m_proxied_net; // only for proxy nets in analog input logic


            public proxied_analog_output_t(core_device_t dev, string aname)
                : base(dev, aname)
            {
                m_proxied_net = null;
            }


            public analog_net_t proxied_net { get { return m_proxied_net; } set { m_proxied_net = value; } }
        }


        abstract class matrix_solver_t : device_t
        {
            //using list_t = std::vector<matrix_solver_t *>;


            protected enum eSortType
            {
                NOSORT,
                ASCENDING,
                DESCENDING,
                PREFER_IDENTITY_TOP_LEFT,
                PREFER_BAND_MATRIX
            }


            //template <typename T> using aligned_alloc = plib::aligned_allocator<T, PALIGN_VECTOROPT>;

            plib.pmatrix2d_nl_double m_gonn = new plib.pmatrix2d_nl_double();  //plib::pmatrix2d<nl_double, aligned_alloc<nl_double>>        m_gonn;
            plib.pmatrix2d_nl_double m_gtn = new plib.pmatrix2d_nl_double();  //plib::pmatrix2d<nl_double, aligned_alloc<nl_double>>        m_gtn;
            plib.pmatrix2d_nl_double m_Idrn = new plib.pmatrix2d_nl_double();  //plib::pmatrix2d<nl_double, aligned_alloc<nl_double>>        m_Idrn;
            protected plib.pmatrix2d_listpointer_nl_double m_mat_ptr = new plib.pmatrix2d_listpointer_nl_double();  //plib::pmatrix2d<nl_double *, aligned_alloc<nl_double *>>    m_mat_ptr;
            plib.pmatrix2d_listpointer_nl_double m_connected_net_Vn = new plib.pmatrix2d_listpointer_nl_double();  //plib::pmatrix2d<nl_double *, aligned_alloc<nl_double *>>    m_connected_net_Vn;

            //plib::pmatrix2d<nl_double>          m_test;


            protected std.vector<terms_for_net_t> m_terms = new std.vector<terms_for_net_t>();  //std::vector<std::unique_ptr<terms_for_net_t>> m_terms;
            std.vector<analog_net_t> m_nets = new std.vector<analog_net_t>();
            std.vector<proxied_analog_output_t> m_inps = new std.vector<proxied_analog_output_t>();  //std::vector<poolptr<proxied_analog_output_t>> m_inps;

            std.vector<terms_for_net_t> m_rails_temp = new std.vector<terms_for_net_t>();  //std::vector<plib::unique_ptr<terms_for_net_t>> m_rails_temp;

            solver_parameters_t m_params;

            state_var<int> m_stat_calculations;
            state_var<int> m_stat_newton_raphson;
            state_var<int> m_stat_vsolver_calls;
            state_var<int> m_iterative_fail;
            state_var<int> m_iterative_total;


            state_var<netlist_time> m_last_step;
            std.vector<core_device_t> m_step_devices = new std.vector<core_device_t>();
            std.vector<core_device_t> m_dynamic_devices = new std.vector<core_device_t>();

            logic_input_t m_fb_sync;
            logic_output_t m_Q_sync;


            UInt32 m_ops;
            eSortType m_sort;


            // ----------------------------------------------------------------------------------------
            // matrix_solver
            // ----------------------------------------------------------------------------------------
            protected matrix_solver_t(netlist_state_t anetlist, string name, eSortType sort, solver_parameters_t params_)
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


            public std.vector<terms_for_net_t> terms { get { return m_terms; } }


            public void setup(analog_net_t_list_t nets)
            {
                vsetup(nets);
            }


            void solve_base()
            {
                ++m_stat_vsolver_calls.op;
                if (has_dynamic_devices())
                {
                    UInt32 this_resched;
                    UInt32 newton_loops = 0;
                    do
                    {
                        update_dynamic();
                        // Gauss-Seidel will revert to Gaussian elemination if steps exceeded.
                        this_resched = this.vsolve_non_dynamic(true);
                        newton_loops++;
                    } while (this_resched > 1 && newton_loops < m_params.m_nr_loops);

                    m_stat_newton_raphson.op += (int)newton_loops;
                    // reschedule ....
                    if (this_resched > 1 && !m_Q_sync.net().is_queued())
                    {
                        log().warning.op(nl_errstr_global.MW_NEWTON_LOOPS_EXCEEDED_ON_NET_1(this.name()));
                        m_Q_sync.net().toggle_and_push_to_queue(m_params.m_nr_recalc_delay);
                    }
                }
                else
                {
                    this.vsolve_non_dynamic(false);
                }
            }


            /* after every call to solve, update inputs must be called.
             * this can be done as well as a batch to ease parallel processing.
             */
            public netlist_time solve(netlist_time now)
            {
                netlist_time delta = now - m_last_step.op;

                // We are already up to date. Avoid oscillations.
                // FIXME: Make this a parameter!
                if (delta < netlist_time.quantum())
                    return netlist_time.zero();

                /* update all terminals for new time step */
                m_last_step.op = now;
                step(delta);
                solve_base();
                netlist_time next_time_step = compute_next_timestep(delta.as_double());

                return next_time_step;
            }


            public void update_inputs()
            {
                // avoid recursive calls. Inputs are updated outside this call
                foreach (var inp in m_inps)
                    inp.push(inp.proxied_net.Q_Analog());
            }


            public bool has_dynamic_devices() { return m_dynamic_devices.size() > 0; }
            public bool has_timestep_devices() { return m_step_devices.size() > 0; }


            public void update_forced()
            {
                netlist_time new_timestep = solve(exec().time());
                update_inputs();

                if (m_params.m_dynamic_ts && has_timestep_devices())
                {
                    m_Q_sync.net().toggle_and_push_to_queue(netlist_time.from_double(m_params.m_min_timestep));
                }
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
            public override void reset()
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


            //std::pair<int, int> get_left_right_of_diag(std::size_t row, std::size_t diag);
            //double get_weight_around_diag(std::size_t row, std::size_t diag);


            public virtual void log_stats()
            {
                //throw new emu_unimplemented();
#if false
                if (this.m_stat_calculations != 0 && this.m_stat_vsolver_calls && this.m_params.m_log_stats)
                {
                    log().verbose("==============================================");
                    log().verbose("Solver {1}", this->name());
                    log().verbose("       ==> {1} nets", this->m_nets.size()); //, (*(*groups[i].first())->m_core_terms.first())->name());
                    log().verbose("       has {1} elements", this->has_dynamic_devices() ? "dynamic" : "no dynamic");
                    log().verbose("       has {1} elements", this->has_timestep_devices() ? "timestep" : "no timestep");
                    log().verbose("       {1:6.3} average newton raphson loops",
                                static_cast<double>(this->m_stat_newton_raphson) / static_cast<double>(this->m_stat_vsolver_calls));
                    log().verbose("       {1:10} invocations ({2:6.0} Hz)  {3:10} gs fails ({4:6.2} %) {5:6.3} average",
                            this->m_stat_calculations,
                            static_cast<double>(this->m_stat_calculations) / this->netlist().time().as_double(),
                            this->m_iterative_fail,
                            100.0 * static_cast<double>(this->m_iterative_fail)
                                / static_cast<double>(this->m_stat_calculations),
                            static_cast<double>(this->m_iterative_total) / static_cast<double>(this->m_stat_calculations));
                }
#endif
            }


            protected virtual KeyValuePair<string, string> create_solver_code()
            {
                return new KeyValuePair<string, string>("", new plib.pfmt("/* solver doesn't support static compile */\n\n").op());
            }


            /* return number of floating point operations for solve */
            //std::size_t ops() { return m_ops; }


            void sort_terms(eSortType sort)
            {
                throw new emu_unimplemented();
            }


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

                    foreach (var p in net.core_terms())
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
                                log().fatal.op(nl_errstr_global.MF_UNHANDLED_ELEMENT_1_FOUND(p.name()));
                                break;
                        }
                    }

                    log().debug.op("added net with {0} populated connections\n", net.core_terms().size());
                }

                /* now setup the matrix */
                setup_matrix();
            }


            void update_dynamic()
            {
                /* update all non-linear devices  */
                foreach (var dyn in m_dynamic_devices)
                    dyn.update_terminals();
            }


            protected abstract void vsetup(analog_net_t_list_t nets);
            protected abstract UInt32 vsolve_non_dynamic(bool newton_raphson);


            netlist_time compute_next_timestep(double cur_ts)
            {
                nl_double new_solver_timestep = m_params.m_max_timestep;

                if (m_params.m_dynamic_ts)
                {
                    for (int k = 0, iN = m_terms.size(); k < iN; k++)
                    {
                        analog_net_t n = m_nets[k];
                        terms_for_net_t t = m_terms[k].get();

                        //nl_double DD_n = (n.Q_Analog() - t.last_V);
                        // avoid floating point exceptions
                        nl_double DD_n = std.max(-1e100, std.min(1e100, (n.Q_Analog() - t.last_V)));
                        nl_double hn = cur_ts;

                        //printf("%g %g %g %g\n", DD_n, hn, t->m_DD_n_m_1, t->m_h_n_m_1);
                        nl_double DD2 = (DD_n / hn - t.DD_n_m_1 / t.h_n_m_1) / (hn + t.h_n_m_1);
                        nl_double new_net_timestep;

                        t.h_n_m_1 = hn;
                        t.DD_n_m_1 = DD_n;
                        if (std.fabs(DD2) > 1e-60)  //plib::constants<nl_double>::cast(1e-60)) // avoid div-by-zero
                            new_net_timestep = std.sqrt(m_params.m_dynamic_lte / std.fabs(0.5 * DD2));  //std::fabs(plib::constants<nl_double>::cast(0.5)*DD2));
                        else
                            new_net_timestep = m_params.m_max_timestep;

                        if (new_net_timestep < new_solver_timestep)
                            new_solver_timestep = new_net_timestep;

                        t.last_V = n.Q_Analog();
                    }
                    if (new_solver_timestep < m_params.m_min_timestep)
                    {
                        //log().warning("Dynamic timestep below min timestep. Consider decreasing MIN_TIMESTEP: {1} us", new_solver_timestep*1.0e6);
                        new_solver_timestep = m_params.m_min_timestep;
                    }
                }
                //if (new_solver_timestep > 10.0 * hn)
                //    new_solver_timestep = 10.0 * hn;
                /*
                 * FIXME: Factor 2 below is important. Without, we get timing issues. This must be a bug elsewhere.
                 */
                return netlist_time.Max(netlist_time.from_double(new_solver_timestep), netlist_time.quantum() * 2);  //std::max(netlist_time::from_double(new_solver_timestep), netlist_time::quantum() * 2);
            }


            /* virtual */ void add_term(UInt32 k, terminal_t term)
            {
                if (term.connected_terminal().net().isRailNet())
                {
                    m_rails_temp[k].add(term, -1, false);
                }
                else
                {
                    int ot = get_net_idx(term.connected_terminal().net());
                    if (ot >= 0)
                    {
                        m_terms[k].add(term, ot, true);
                    }
                    /* Should this be allowed ? */
                    else // if (ot<0)
                    {
                        m_rails_temp[k].add(term, ot, true);
                        log().fatal.op(nl_errstr_global.MF_FOUND_TERM_WITH_MISSING_OTHERNET(term.name()));
                    }
                }
            }

            //template <typename T>
            //void store(const T & V);
            //template <typename T>
            //auto delta(const T & V) -> typename std::decay<decltype(V[0])>::type;

            //template <typename T>
            //void build_LE_A();
            //template <typename T>
            //void build_LE_RHS();


            void set_pointers()
            {
                UInt32 iN = (UInt32)this.m_nets.size();

                UInt32 max_count = 0;
                UInt32 max_rail = 0;
                for (UInt32 k = 0; k < iN; k++)
                {
                    max_count = std.max(max_count, m_terms[k].count());
                    max_rail = std.max(max_rail, m_terms[k].railstart);
                }

                m_mat_ptr.resize(iN, max_rail+1);

                m_gtn.resize(iN, max_count);
                m_gonn.resize(iN, max_count);
                m_Idrn.resize(iN, max_count);
                m_connected_net_Vn.resize(iN, max_count);

                for (UInt32 k = 0; k < iN; k++)
                {
                    var count = m_terms[k].count();

                    for (int i = 0; i < count; i++)
                    {
                        m_terms[k].terms()[i].set_ptrs(new ListPointer<nl_double>(m_gtn.op(k), i), new ListPointer<nl_double>(m_gonn.op(k), i), new ListPointer<nl_double>(m_Idrn.op(k), i));  //m_terms[k]->terms()[i]->set_ptrs(&m_gtn[k][i], &m_gonn[k][i], &m_Idrn[k][i]);
                        m_connected_net_Vn.op(k)[i] = new ListPointer<nl_double>(m_terms[k].terms()[i].connected_terminal().net().Q_Analog_state_ptr());  //m_connected_net_Vn[k][i] = m_terms[k]->terms()[i]->connected_terminal()->net().Q_Analog_state_ptr();
                    }
                }
            }


            //template <typename AP, typename FT>
            //void fill_matrix(std::size_t N, AP &tcr, FT &RHS)


            /* calculate matrix */
            void setup_matrix()
            {
                UInt32 iN = (UInt32)m_nets.size();

                for (UInt32 k = 0; k < iN; k++)
                {
                    m_terms[k].railstart = m_terms[k].count();
                    for (UInt32 i = 0; i < m_rails_temp[k].count(); i++)
                        this.m_terms[k].add(m_rails_temp[k].terms()[i], m_rails_temp[k].connected_net_idx[i], false);
                }

                // free all - no longer needed
                m_rails_temp.clear();

                sort_terms(m_sort);

                this.set_pointers();

                /* create a list of non zero elements. */
                for (UInt32 k = 0; k < iN; k++)
                {
                    terms_for_net_t t = m_terms[k];
                    /* pretty brutal */
                    var other = t.connected_net_idx;

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
                    var other = t.connected_net_idx;

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

                bool [,] touched = new bool [iN, iN];  //std::vector<std::vector<bool>> touched(iN, std::vector<bool>(iN));

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

                    state().save(this, m_terms[k].last_V, this.name(), "lastV." + num);
                    state().save(this, m_terms[k].DD_n_m_1, this.name(), "m_DD_n_m_1." + num);
                    state().save(this, m_terms[k].h_n_m_1, this.name(), "m_h_n_m_1." + num);

                    state().save(this, m_gonn.op(k),"GO" + num, this.name(), m_terms[k].count());
                    state().save(this, m_gtn.op(k),"GT" + num, this.name(), m_terms[k].count());
                    state().save(this, m_Idrn.op(k),"IDR" + num , this.name(), m_terms[k].count());
                }
            }


            void step(netlist_time delta)
            {
                nl_double dd = delta.as_double();
                for (int k = 0; k < m_step_devices.size(); k++)
                    m_step_devices[k].timestep(dd);
            }
        }
    } //namespace devices
} // namespace netlist
