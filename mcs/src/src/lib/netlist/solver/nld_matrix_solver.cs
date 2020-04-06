// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;
using nl_fptype = System.Double;


namespace mame.netlist
{
    namespace solver
    {
        public enum matrix_sort_type_e  //P_ENUM(matrix_sort_type_e,
        {
            NOSORT,
            ASCENDING,
            DESCENDING,
            PREFER_IDENTITY_TOP_LEFT,
            PREFER_BAND_MATRIX
        }


        public enum matrix_type_e  //P_ENUM(matrix_type_e,
        {
            SOR_MAT,
            MAT_CR,
            MAT,
            SM,
            W,
            SOR,
            GMRES
        }


        public enum matrix_fp_type_e  //P_ENUM(matrix_fp_type_e,
        {
            FLOAT,
            DOUBLE,
            LONGDOUBLE,
            FLOAT128
        }


        struct solver_parameters_t
        {
            param_fp_t m_freq;
            param_fp_t m_gs_sor;
            public param_enum_t_matrix_type_e m_method;
            public param_enum_t_matrix_fp_type_e m_fp_type;
            param_fp_t m_reltol;
            param_fp_t m_vntol;
            param_fp_t m_accuracy;
            public param_num_t_size_t m_nr_loops;
            param_num_t_size_t m_gs_loops;
            public param_fp_t m_gmin;
            param_logic_t m_pivot;
            public param_fp_t m_nr_recalc_delay;
            public param_int_t m_parallel;
            public param_logic_t m_dynamic_ts;
            public param_fp_t m_dynamic_lte;
            param_fp_t m_dynamic_min_ts;
            public param_enum_t_matrix_sort_type_e m_sort_type;

            param_logic_t m_use_gabs;
            param_logic_t m_use_linear_prediction;

            public nl_fptype m_min_timestep;
            public nl_fptype m_max_timestep;


            public solver_parameters_t(device_t parent)
            {
                m_freq = new param_fp_t(parent, "FREQ", nlconst.magic(48000.0));

                // iteration parameters
                m_gs_sor = new param_fp_t(parent, "SOR_FACTOR", nlconst.magic(1.059));
                m_method = new param_enum_t_matrix_type_e(parent, "METHOD", matrix_type_e.MAT_CR);
                m_fp_type = new param_enum_t_matrix_fp_type_e(parent, "FPTYPE", matrix_fp_type_e.DOUBLE);
                m_reltol = new param_fp_t(parent, "RELTOL", nlconst.magic(1e-3));            ///< SPICE RELTOL parameter
                m_vntol = new param_fp_t(parent, "VNTOL", nlconst.magic(1e-7));            ///< SPICE VNTOL parameter
                m_accuracy = new param_fp_t(parent, "ACCURACY", nlconst.magic(1e-7));          ///< Iterative solver accuracy
                m_nr_loops = new param_num_t_size_t(parent, "NR_LOOPS", 250);           ///< Maximum number of Newton-Raphson loops
                m_gs_loops = new param_num_t_size_t(parent, "GS_LOOPS", 9);             ///< Maximum number of Gauss-Seidel loops

                // general parameters
                m_gmin = new param_fp_t(parent, "GMIN", nlconst.magic(1e-9));
                m_pivot = new param_logic_t(parent, "PIVOT", false);               ///< use pivoting on supported solvers
                m_nr_recalc_delay = new param_fp_t(parent, "NR_RECALC_DELAY", netlist_time.quantum().as_fp()); ///< Delay to next solve attempt if nr loops exceeded
                m_parallel = new param_int_t(parent, "PARALLEL", 0);

                // automatic time step
                m_dynamic_ts = new param_logic_t(parent, "DYNAMIC_TS", false);     ///< Use dynamic time stepping
                m_dynamic_lte = new param_fp_t(parent, "DYNAMIC_LTE", nlconst.magic(1e-5));    ///< dynamic time stepping slope
                m_dynamic_min_ts = new param_fp_t(parent, "DYNAMIC_MIN_TIMESTEP", nlconst.magic(1e-6)); ///< smallest time step allowed

                // matrix sorting
                m_sort_type = new param_enum_t_matrix_sort_type_e(parent, "SORT_TYPE", matrix_sort_type_e.PREFER_IDENTITY_TOP_LEFT);

                // special
                m_use_gabs = new param_logic_t(parent, "USE_GABS", true);
                m_use_linear_prediction = new param_logic_t(parent, "USE_LINEAR_PREDICTION", false); // // savings are eaten up by effort

                {
                    m_min_timestep = m_dynamic_min_ts.op();
                    m_max_timestep = netlist_time.from_fp(plib.pmath_global.reciprocal(m_freq.op())).as_fp();  //m_max_timestep = netlist_time::from_fp(plib::reciprocal(m_freq())).as_fp<decltype(m_max_timestep)>();


                    if (m_dynamic_ts != null)
                    {
                        m_max_timestep *= 1;//NL_FCONST(1000.0);
                    }
                    else
                    {
                        m_min_timestep = m_max_timestep;
                    }
                }
            }
        }


        class terms_for_net_t
        {
            public plib.aligned_vector<UInt32> m_nz;   //!< all non zero for multiplication
            public plib.aligned_vector<UInt32> m_nzrd; //!< non zero right of the diagonal for elimination, may include RHS element
            public plib.aligned_vector<UInt32> m_nzbd; //!< non zero below of the diagonal for elimination

            public plib.aligned_vector<int> m_connected_net_idx;

            analog_net_t m_net;
            plib.aligned_vector<terminal_t> m_terms;
            UInt32 m_railstart;  //std::size_t m_railstart;


            public terms_for_net_t(analog_net_t net = null)
            {
                m_net = net;
                m_railstart = 0;
            }


            public void add_terminal(terminal_t term, int net_other, bool sorted)
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

            public UInt32 railstart() { return m_railstart; }

            public std.vector<terminal_t> terms() { return m_terms; }  //inline terminal_t **terms() { return m_terms.data(); }


            //template <typename FT, typename = std::enable_if<std::is_floating_point<FT>::value, void>>
            public double getV() { return m_net.Q_Analog(); }  //FT getV() const noexcept { return static_cast<FT>(m_net->Q_Analog()); }

            //template <typename FT, typename = std::enable_if<std::is_floating_point<FT>::value, void>>
            //void setV(FT v) noexcept { m_net->set_Q_Analog(static_cast<nl_fptype>(v)); }


            public bool isNet(analog_net_t net) { return net == m_net; }


            public void set_railstart(UInt32 val) { m_railstart = val; }
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
            //template <typename T> using aligned_alloc = plib::aligned_allocator<T, PALIGN_VECTOROPT>;

            plib.pmatrix2d_nl_fptype m_gonn = new plib.pmatrix2d_nl_fptype();  //plib::pmatrix2d<nl_fptype, aligned_alloc<nl_fptype>>        m_gonn;
            plib.pmatrix2d_nl_fptype m_gtn = new plib.pmatrix2d_nl_fptype();  //plib::pmatrix2d<nl_fptype, aligned_alloc<nl_fptype>>        m_gtn;
            plib.pmatrix2d_nl_fptype m_Idrn = new plib.pmatrix2d_nl_fptype();  //plib::pmatrix2d<nl_fptype, aligned_alloc<nl_fptype>>        m_Idrn;
            plib.pmatrix2d_listpointer_nl_fptype m_connected_net_Vn = new plib.pmatrix2d_listpointer_nl_fptype();  //plib::pmatrix2d<nl_fptype *, aligned_alloc<nl_fptype *>>    m_connected_net_Vn;

            protected plib.aligned_vector<terms_for_net_t> m_terms;
            plib.aligned_vector<terms_for_net_t> m_rails_temp;

            std.vector<proxied_analog_output_t> m_inps = new std.vector<proxied_analog_output_t>();  //std::vector<unique_pool_ptr<proxied_analog_output_t>> m_inps;

            protected solver_parameters_t m_params;

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


            // ----------------------------------------------------------------------------------------
            // matrix_solver
            // ----------------------------------------------------------------------------------------
            protected matrix_solver_t(netlist_state_t anetlist, string name, analog_net_t.list_t nets, solver_parameters_t params_)
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


                connect_post_start(m_fb_sync, m_Q_sync);
                setup_base(nets);

                // now setup the matrix
                setup_matrix();
            }


            public netlist_time solve(netlist_time now)
            {
                netlist_time delta = now - m_last_step.op;

                // We are already up to date. Avoid oscillations.
                // FIXME: Make this a parameter!
                if (delta < netlist_time.quantum())
                    return netlist_time.zero();

                // update all terminals for new time step
                m_last_step.op = now;
                step(delta);

                ++m_stat_vsolver_calls.op;
                if (has_dynamic_devices())
                {
                    UInt32 this_resched = 0;
                    UInt32 newton_loops = 0;
                    do
                    {
                        update_dynamic();
                        // Gauss-Seidel will revert to Gaussian elemination if steps exceeded.
                        this_resched = this.vsolve_non_dynamic(true);
                        newton_loops++;
                    } while (this_resched > 1 && newton_loops < m_params.m_nr_loops.op());

                    m_stat_newton_raphson.op += (int)newton_loops;
                    // reschedule ....
                    if (this_resched > 1 && !m_Q_sync.net().is_queued())
                    {
                        log().warning.op(nl_errstr_global.MW_NEWTON_LOOPS_EXCEEDED_ON_NET_1(this.name()));
                        m_Q_sync.net().toggle_and_push_to_queue(netlist_time.from_fp(m_params.m_nr_recalc_delay.op()));
                    }
                }
                else
                {
                    this.vsolve_non_dynamic(false);
                }

                netlist_time next_time_step = compute_next_timestep(delta.as_fp());
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


            // update_forced is called from within param_update
            //
            // this should only occur outside of execution and thus
            // using time should be safe.
            public void update_forced()
            {
                netlist_time new_timestep = solve(exec().time());
                update_inputs();

                if (m_params.m_dynamic_ts.op() && has_timestep_devices())
                {
                    m_Q_sync.net().toggle_and_push_to_queue(netlist_time.from_fp(m_params.m_min_timestep));
                }
            }


            //void update_after(const netlist_time &after)
            //{
            //    m_Q_sync.net().toggle_and_push_to_queue(after);
            //}


            // netdevice functions
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


            int get_net_idx(analog_net_t net)
            {
                for (UInt32 k = 0; k < m_terms.size(); k++)
                {
                    if (m_terms[k].isNet(net))
                        return (int)k;
                }
                return -1;
            }


            //std::pair<int, int> get_left_right_of_diag(std::size_t irow, std::size_t idiag);
            //nl_fptype get_weight_around_diag(std::size_t row, std::size_t diag);


            public virtual void log_stats()
            {
                //throw new emu_unimplemented();
#if false
#endif
            }


            protected virtual std.pair<string, string> create_solver_code()
            {
                return new std.pair<string, string>("", new plib.pfmt("/* solver doesn't support static compile */\n\n").op());
            }


            // return number of floating point operations for solve
            //std::size_t ops() { return m_ops; }


            void sort_terms(matrix_sort_type_e sort)
            {
                throw new emu_unimplemented();
            }


            void update_dynamic()
            {
                // update all non-linear devices
                foreach (var dyn in m_dynamic_devices)
                    dyn.update_terminals();
            }


            protected abstract UInt32 vsolve_non_dynamic(bool newton_raphson);

            protected abstract netlist_time compute_next_timestep(nl_fptype cur_ts);


            void add_term(UInt32 net_idx, terminal_t term)
            {
                if (term.connected_terminal().net().isRailNet())
                {
                    m_rails_temp[net_idx].add_terminal(term, -1, false);
                }
                else
                {
                    int ot = get_net_idx(term.connected_terminal().net());
                    if (ot >= 0)
                    {
                        m_terms[net_idx].add_terminal(term, ot, true);
                    }
                    // Should this be allowed ?
                    else // if (ot<0)
                    {
                        m_rails_temp[net_idx].add_terminal(term, ot, true);
                        log().fatal.op(nl_errstr_global.MF_FOUND_TERM_WITH_MISSING_OTHERNET(term.name()));
                        throw new nl_exception(nl_errstr_global.MF_FOUND_TERM_WITH_MISSING_OTHERNET(term.name()));
                    }
                }
            }


            //std::size_t max_railstart() const noexcept


            void set_pointers()
            {
                UInt32 iN = (UInt32)this.m_terms.size();

                UInt32 max_count = 0;
                UInt32 max_rail = 0;
                for (UInt32 k = 0; k < iN; k++)
                {
                    max_count = std.max(max_count, m_terms[k].count());
                    max_rail = std.max(max_rail, m_terms[k].railstart());
                }

                m_gtn.resize(iN, max_count);
                m_gonn.resize(iN, max_count);
                m_Idrn.resize(iN, max_count);
                m_connected_net_Vn.resize(iN, max_count);

                for (UInt32 k = 0; k < iN; k++)
                {
                    var count = m_terms[k].count();

                    for (int i = 0; i < count; i++)
                    {
                        m_terms[k].terms()[i].set_ptrs(new ListPointer<nl_fptype>(m_gtn.op(k), i), new ListPointer<nl_fptype>(m_gonn.op(k), i), new ListPointer<nl_fptype>(m_Idrn.op(k), i));  //m_terms[k].terms()[i]->set_ptrs(&m_gtn[k][i], &m_gonn[k][i], &m_Idrn[k][i]);
                        m_connected_net_Vn.op(k)[i] = new ListPointer<nl_fptype>(m_terms[k].terms()[i].connected_terminal().net().Q_Analog_state_ptr());  //m_connected_net_Vn[k][i] = m_terms[k].terms()[i]->connected_terminal()->net().Q_Analog_state_ptr();
                    }
                }
            }


            //template <typename T, typename M>
            protected void log_fill(std.vector<std.vector<UInt32>> fill, plib.pGEmatrix_cr_t_pmatrix_cr_t_double_uint16 mat)  //void log_fill(const T &fill, M &mat)
            {
                throw new emu_unimplemented();
#if false
                const std::size_t iN = fill.size();

                // FIXME: Not yet working, mat_cr.h needs some more work
#if false
                auto mat_GE = dynamic_cast<plib::pGEmatrix_cr_t<typename M::base> *>(&mat);
#else
                plib::unused_var(mat);
#endif
                std::vector<unsigned> levL(iN, 0);
                std::vector<unsigned> levU(iN, 0);

                // parallel scheme for L x = y
                for (std::size_t k = 0; k < iN; k++)
                {
                    unsigned lm=0;
                    for (std::size_t j = 0; j<k; j++)
                        if (fill[k][j] < M::FILL_INFINITY)
                            lm = std::max(lm, levL[j]);
                    levL[k] = 1+lm;
                }

                // parallel scheme for U x = y
                for (std::size_t k = iN; k-- > 0; )
                {
                    unsigned lm=0;
                    for (std::size_t j = iN; --j > k; )
                        if (fill[k][j] < M::FILL_INFINITY)
                            lm = std::max(lm, levU[j]);
                    levU[k] = 1+lm;
                }
                for (std::size_t k = 0; k < iN; k++)
                {
                    unsigned fm = 0;
                    pstring ml = "";
                    for (std::size_t j = 0; j < iN; j++)
                    {
                        ml += fill[k][j] == 0 ? 'X' : fill[k][j] < M::FILL_INFINITY ? '+' : '.';
                        if (fill[k][j] < M::FILL_INFINITY)
                            if (fill[k][j] > fm)
                                fm = fill[k][j];
                    }
#if false
                    this->log().verbose("{1:4} {2} {3:4} {4:4} {5:4} {6:4}", k, ml,
                        levL[k], levU[k], mat_GE ? mat_GE->get_parallel_level(k) : 0, fm);
#else
                    this->log().verbose("{1:4} {2} {3:4} {4:4} {5:4} {6:4}", k, ml,
                        levL[k], levU[k], 0, fm);
#endif
                }
#endif
            }


            // base setup - called from constructor
            protected void setup_base(analog_net_t.list_t nets)
            {
                log().debug.op("New solver setup\n");

                m_terms.clear();

                foreach (var net in nets)
                {
                    m_terms.emplace_back(new terms_for_net_t(net));
                    m_rails_temp.emplace_back(new terms_for_net_t());
                }

                for (UInt32 k = 0; k < nets.size(); k++)
                {
                    analog_net_t net = nets[k];

                    log().debug.op("adding net with {0} populated connections\n", net.core_terms().size());

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
                                        nl_config_global.nl_assert(p.net().is_analog());
                                        var net_proxy_output_u = new proxied_analog_output_t(this, nname);  //auto net_proxy_output_u = state().make_object<proxied_analog_output_t>(*this, nname, static_cast<analog_net_t *>(&p->net()));
                                        net_proxy_output = net_proxy_output_u;
                                        m_inps.emplace_back(net_proxy_output_u);
                                    }

                                    net_proxy_output.net().add_terminal(p);

                                    // FIXME: repeated calling - kind of brute force
                                    net_proxy_output.net().rebuild_list();

                                    log().debug.op("Added input {0}", net_proxy_output.name());
                                }
                                break;

                            case detail.terminal_type.OUTPUT:
                                log().fatal.op(nl_errstr_global.MF_UNHANDLED_ELEMENT_1_FOUND(p.name()));
                                throw new nl_exception(nl_errstr_global.MF_UNHANDLED_ELEMENT_1_FOUND(p.name()));
                        }
                    }
                }
            }


            // calculate matrix
            void setup_matrix()
            {
                UInt32 iN = (UInt32)m_terms.size();

                for (UInt32 k = 0; k < iN; k++)
                {
                    m_terms[k].set_railstart(m_terms[k].count());
                    for (UInt32 i = 0; i < m_rails_temp[k].count(); i++)
                        this.m_terms[k].add_terminal(m_rails_temp[k].terms()[i], m_rails_temp[k].m_connected_net_idx[i], false);
                }

                // free all - no longer needed
                m_rails_temp.clear();

                sort_terms(m_params.m_sort_type.op());

                this.set_pointers();

                // create a list of non zero elements.
                for (UInt32 k = 0; k < iN; k++)
                {
                    terms_for_net_t t = m_terms[k];
                    // pretty brutal
                    var other = t.m_connected_net_idx;

                    t.m_nz.clear();

                    for (UInt32 i = 0; i < t.railstart(); i++)
                    {
                        if (!t.m_nz.Contains((UInt32)other[i]))  //if (!plib::container::contains(t->m_nz, static_cast<unsigned>(other[i])))
                            t.m_nz.push_back((UInt32)other[i]);
                    }

                    t.m_nz.push_back(k);     // add diagonal

                    // and sort
                    t.m_nz.Sort();  //std::sort(t.m_nz.begin(), t.m_nz.end());
                }

                // create a list of non zero elements right of the diagonal
                // These list anticipate the population of array elements by
                // Gaussian elimination.
                for (UInt32 k = 0; k < iN; k++)
                {
                    terms_for_net_t t = m_terms[k];
                    // pretty brutal
                    var other = t.m_connected_net_idx;

                    if (k == 0)
                    {
                        t.m_nzrd.clear();
                    }
                    else
                    {
                        t.m_nzrd = m_terms[k - 1].m_nzrd;
                        for (var jIdx = 0; jIdx < t.m_nzrd.Count;  )  //for (var j = t.nzrd.begin(); j != t.nzrd.end(); )
                        {
                            var j = t.m_nzrd[jIdx];

                            if (j < k + 1)
                                t.m_nzrd.erase(jIdx);
                            else
                                ++jIdx;
                        }
                    }

                    for (UInt32 i = 0; i < t.railstart(); i++)
                    {
                        if (!t.m_nzrd.Contains((UInt32)other[i]) && other[i] >= (int)(k + 1))  //if (!plib::container::contains(t->m_nzrd, static_cast<unsigned>(other[i])) && other[i] >= static_cast<int>(k + 1))
                            t.m_nzrd.push_back((UInt32)other[i]);
                    }

                    // and sort
                    t.m_nzrd.Sort();  //std::sort(t.m_nzrd.begin(), t.m_nzrd.end());
                }

                // create a list of non zero elements below diagonal k
                // This should reduce cache misses ...

                //std::vector<std::vector<bool>> touched(iN, std::vector<bool>(iN));
                std.vector<std.vector<bool>> touched = new std.vector<std.vector<bool>>();
                for (int i = 0; i < iN; i++)
                    touched.Add(new std.vector<bool>());

                for (UInt32 k = 0; k < iN; k++)
                {
                    for (UInt32 j = 0; j < iN; j++)
                        touched[k][j] = false;

                    for (UInt32 j = 0; j < m_terms[k].m_nz.size(); j++)
                        touched[k][m_terms[k].m_nz[j]] = true;
                }

                m_ops = 0;
                for (UInt32 k = 0; k < iN; k++)
                {
                    m_ops++; // 1/A(k,k)
                    for (UInt32 row = k + 1; row < iN; row++)
                    {
                        if (touched[row][k])
                        {
                            m_ops++;
                            if (!m_terms[k].m_nzbd.Contains(row))  //if (!plib::container::contains(m_terms[k]->m_nzbd, row))
                                m_terms[k].m_nzbd.push_back(row);

                            for (UInt32 col = k + 1; col < iN; col++)
                            {
                                if (touched[k][col])
                                {
                                    touched[row][col] = true;
                                    m_ops += 2;
                                }
                            }
                        }
                    }
                }

                log().verbose.op("Number of mults/adds for {0}: {1}", name(), m_ops);

#if false
                if ((false))
                    for (std::size_t k = 0; k < iN; k++)
                    {
                        pstring line = plib::pfmt("{1:3}")(k);
                        for (const auto & nzrd : m_terms[k].m_nzrd)
                            line += plib::pfmt(" {1:3}")(nzrd);
                        log().verbose("{1}", line);
                    }
#endif

                //
                // save states
                //
                for (UInt32 k = 0; k < iN; k++)
                {
                    string num = new plib.pfmt("{0}").op(k);

                    state().save(this, m_gonn.op(k),"GO" + num, this.name(), m_terms[k].count());
                    state().save(this, m_gtn.op(k),"GT" + num, this.name(), m_terms[k].count());
                    state().save(this, m_Idrn.op(k),"IDR" + num , this.name(), m_terms[k].count());
                }
            }


            void step(netlist_time delta)
            {
                var dd = delta.as_fp();
                foreach (var d in m_step_devices)
                    d.timestep(dd);
            }
        }


        //template <typename FT, int SIZE>
        abstract class matrix_solver_ext_t : matrix_solver_t
        {
            //friend class matrix_solver_t;

            //using float_type = FT;


            // template parameters
            int SIZE;


            //const std::size_t m_dim;

            //static constexpr const std::size_t SIZEABS = plib::parray<FT, SIZE>::SIZEABS();
            //static constexpr const std::size_t m_pitch_ABS = (((SIZEABS + 0) + 7) / 8) * 8;

            //PALIGNAS_VECTOROPT()
            //plib::parray<FT, SIZE> m_new_V;
            //PALIGNAS_VECTOROPT()
            //plib::parray<FT, SIZE> m_RHS;

            //PALIGNAS_VECTOROPT()
            //plib::parray2D<float_type *, SIZE, 0> m_mat_ptr;

            // FIXME: below should be private
            // state - variable time_stepping
            //PALIGNAS_VECTOROPT()
            nl_fptype [] m_last_V;  //plib::parray<nl_fptype, SIZE> m_last_V;
            //PALIGNAS_VECTOROPT()
            nl_fptype [] m_DD_n_m_1;  //plib::parray<nl_fptype, SIZE> m_DD_n_m_1;
            //PALIGNAS_VECTOROPT()
            nl_fptype [] m_h_n_m_1;  //plib::parray<nl_fptype, SIZE> m_h_n_m_1;


            protected matrix_solver_ext_t(int SIZE, netlist_state_t anetlist, string name,
                analog_net_t.list_t nets,
                solver_parameters_t params_, UInt32 size)
                : base(anetlist, name, nets, params_)
            {
                this.SIZE = SIZE;


                throw new emu_unimplemented();
#if false
                m_dim(size)
                m_new_V(size)
                m_RHS(size)
                m_mat_ptr(size, this->max_railstart() + 1)
                m_last_V(size, nlconst::zero())
                m_DD_n_m_1(size, nlconst::zero())
                m_h_n_m_1(size, nlconst::zero())


                //
                // save states
                //
                state().save(*this, m_last_V.as_base(), this->name(), "m_last_V");
                state().save(*this, m_DD_n_m_1.as_base(), this->name(), "m_DD_n_m_1");
                state().save(*this, m_h_n_m_1.as_base(), this->name(), "m_h_n_m_1");
#endif
            }


            protected UInt32 size()
            {
                throw new emu_unimplemented();
#if false
                return (SIZE > 0) ? (UInt32)SIZE : m_dim;  //return (SIZE > 0) ? static_cast<std::size_t>(SIZE) : m_dim;
#endif
            }


            //void store()

            //bool check_err()


            protected override netlist_time compute_next_timestep(nl_fptype cur_ts)
            {
                nl_fptype new_solver_timestep = m_params.m_max_timestep;

                if (m_params.m_dynamic_ts.op())
                {
                    for (UInt32 k = 0; k < size(); k++)
                    {
                        var t = m_terms[k];
                        //const nl_fptype DD_n = (n->Q_Analog() - t->m_last_V);
                        // avoid floating point exceptions

                        nl_fptype DD_n = std.max(-fp_constants.TIMESTEP_MAXDIFF(),
                            std.min(+fp_constants.TIMESTEP_MAXDIFF(), (t.getV() - m_last_V[k])));
                        nl_fptype hn = cur_ts;

                        //printf("%g %g %g %g\n", DD_n, hn, t.m_DD_n_m_1, t.m_h_n_m_1);
                        nl_fptype DD2 = (DD_n / hn - m_DD_n_m_1[k] / m_h_n_m_1[k]) / (hn + m_h_n_m_1[k]);
                        nl_fptype new_net_timestep = 0;

                        m_h_n_m_1[k] = hn;
                        m_DD_n_m_1[k] = DD_n;
                        if (plib.pmath_global.abs(DD2) > fp_constants.TIMESTEP_MINDIV()) // avoid div-by-zero
                            new_net_timestep = plib.pmath_global.sqrt(m_params.m_dynamic_lte.op() / plib.pmath_global.abs(nlconst.magic(0.5) * DD2));
                        else
                            new_net_timestep = m_params.m_max_timestep;

                        if (new_net_timestep < new_solver_timestep)
                            new_solver_timestep = new_net_timestep;

                        m_last_V[k] = t.getV();
                    }
                    if (new_solver_timestep < m_params.m_min_timestep)
                    {
                        new_solver_timestep = m_params.m_min_timestep;
                    }
                }
                //if (new_solver_timestep > 10.0 * hn)
                //    new_solver_timestep = 10.0 * hn;
                //
                // FIXME: Factor 2 below is important. Without, we get timing issues. This must be a bug elsewhere.

                return std.max(netlist_time.from_fp(new_solver_timestep), netlist_time.quantum() * 2);
            }


            //template <typename M>
            //void build_mat_ptr(M &mat)

            //template <typename M>
            //void clear_square_mat(M &m)

            //void fill_matrix_and_rhs()
        }
    }
}
