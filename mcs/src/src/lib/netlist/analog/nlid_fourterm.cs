// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using base_device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = base_device_param_t;  //using base_device_param_t = const base_device_data_t &;  //using base_device_data_t = core_device_data_t;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;


namespace mame.netlist.analog
{
    // ----------------------------------------------------------------------------------------
    // nld_VCCS
    // ----------------------------------------------------------------------------------------

    //
    //   Voltage controlled current source
    //
    //   IP ---+           +------> OP
    //         |           |
    //         RI          I
    //         RI => G =>  I    IOut = (V(IP)-V(IN)) * G
    //         RI          I
    //         |           |
    //   IN ---+           +------< ON
    //
    //   G=1 ==> 1V ==> 1A
    //
    //   RI = 1 / NETLIST_GMIN
    //
    public class nld_VCCS : base_device_t
    {
        public param_fp_t m_G;
        public param_fp_t m_RI;

        terminal_t m_OP;
        terminal_t m_ON;

        terminal_t m_IP;
        terminal_t m_IN;

        protected terminal_t m_OP1;
        protected terminal_t m_ON1;

        nl_fptype m_gfac;


        public nld_VCCS(base_device_t_constructor_param_t data) : this(data, nlconst.magic(1e9)) { }
        public nld_VCCS(base_device_t_constructor_param_t data, nl_fptype ri)
            : base(data)
        {
            m_G = new param_fp_t(this, "G", nlconst.one());
            m_RI = new param_fp_t(this, "RI", ri);

            m_OP = new terminal_t(this, "OP", terminal_handler);  //m_OP(*this, "OP", &m_IP, {&m_ON, &m_IN}, NETLIB_DELEGATE(terminal_handler));
            m_ON = new terminal_t(this, "ON", terminal_handler);  //m_ON(*this, "ON", &m_IP, {&m_OP, &m_IN}, NETLIB_DELEGATE(terminal_handler));
            m_IP = new terminal_t(this, "IP", terminal_handler);  //m_IP(*this, "IP", &m_IN, {&m_OP, &m_ON}, NETLIB_DELEGATE(terminal_handler));
            m_IN = new terminal_t(this, "IN", terminal_handler);  //m_IN(*this, "IN", &m_IP, {&m_OP, &m_ON}, NETLIB_DELEGATE(terminal_handler));
            m_OP1 = new terminal_t(this, "_OP1", terminal_handler);//, m_OP1(*this, "_OP1", &m_IN, NETLIB_DELEGATE(terminal_handler))
            m_ON1 = new terminal_t(this, "_ON1", terminal_handler);//, m_ON1(*this, "_ON1", &m_IN, NETLIB_DELEGATE(terminal_handler))
            m_OP.terminal_t_after_ctor(m_IP, new std.array<terminal_t, u64_const_2>(m_ON, m_IN));
            m_ON.terminal_t_after_ctor(m_IP, new std.array<terminal_t, u64_const_2>(m_OP, m_IN));
            m_IP.terminal_t_after_ctor(m_IN, new std.array<terminal_t, u64_const_2>(m_OP, m_ON));
            m_IN.terminal_t_after_ctor(m_IP, new std.array<terminal_t, u64_const_2>(m_OP, m_ON));
            m_OP1.terminal_t_after_ctor(m_IN);
            m_ON1.terminal_t_after_ctor(m_IN);

            m_gfac = nlconst.one();


            connect(m_OP, m_OP1);
            connect(m_ON, m_ON1);
        }


        //NETLIB_RESETI();
        //NETLIB_RESET(VCCS)
        public override void reset()
        {
            nl_fptype m_mult = m_G.op() * m_gfac; // 1.0 ==> 1V ==> 1A
            nl_fptype GI = plib.pg.reciprocal(m_RI.op());

            m_IP.set_conductivity(GI);
            m_IN.set_conductivity(GI);

            m_OP.set_go_gt(-m_mult, nlconst.zero());
            m_OP1.set_go_gt(m_mult, nlconst.zero());

            m_ON.set_go_gt(m_mult, nlconst.zero());
            m_ON1.set_go_gt(-m_mult, nlconst.zero());
        }


        //NETLIB_HANDLERI(terminal_handler);
        //NETLIB_HANDLER(VCCS, terminal_handler)
        protected void terminal_handler()
        {
            solver.matrix_solver_t solv = null;
            // only called if connected to a rail net ==> notify the solver to recalculate
            if ((solv = m_IP.solver()) != null)
                solv.solve_now();
            else if ((solv = m_IN.solver()) != null)
                solv.solve_now();
            else if ((solv = m_OP.solver()) != null)
                solv.solve_now();
            else if ((solv = m_ON.solver()) != null)
                solv.solve_now();
        }


        //NETLIB_UPDATE_PARAMI()
        public override void update_param()
        {
            this.reset();  //NETLIB_NAME(VCCS)::reset(); }
        }


        protected void set_gfac(nl_fptype g)
        {
            m_gfac = g;
        }


        nl_fptype get_gfac()
        {
            return m_gfac;
        }
    }


    //class nld_LVCCS : public nld_VCCS

    //class nld_CCCS : public nld_VCCS


    public class nld_VCVS : nld_VCCS
    {
        public param_fp_t m_RO;

        terminal_t m_OP2;
        terminal_t m_ON2;


        public nld_VCVS(base_device_t_constructor_param_t data)
            : base(data)
        { 
            m_RO = new param_fp_t(this, "RO", nlconst.one());

            m_OP2 = new terminal_t(this, "_OP2", terminal_handler);//, &m_ON2, NETLIB_DELEGATE(terminal_handler))
            m_ON2 = new terminal_t(this, "_ON2", terminal_handler);//, &m_OP2, NETLIB_DELEGATE(terminal_handler))
            m_OP2.terminal_t_after_ctor(m_ON2);
            m_ON2.terminal_t_after_ctor(m_OP2);


            connect(m_OP2, m_OP1);
            connect(m_ON2, m_ON1);
        }


        //NETLIB_RESETI();
        //NETLIB_RESET(VCVS)
        public override void reset()
        {
            var gfac = plib.pg.reciprocal(m_RO.op());
            set_gfac(gfac);

            base.reset();

            m_OP2.set_conductivity(gfac);
            m_ON2.set_conductivity(gfac);
        }


        ////NETLIB_UPDATE_PARAMI();


        //NETLIB_HANDLERI(terminal_handler)
        new void terminal_handler()
        {
            base.terminal_handler();  //NETLIB_NAME(VCCS) :: terminal_handler();
        }
    }


    //class nld_CCVS : public nld_VCCS
}
