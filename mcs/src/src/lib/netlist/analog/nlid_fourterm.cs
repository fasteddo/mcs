// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;


namespace mame.netlist
{
    namespace analog
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
        //NETLIB_OBJECT(VCCS)
        class nld_VCCS : device_t
        {
            public param_fp_t m_G;
            public param_fp_t m_RI;

            terminal_t m_OP;
            terminal_t m_ON;

            terminal_t m_IP;
            terminal_t m_IN;

            protected terminal_t m_OP1;
            protected terminal_t m_ON1;

            protected nl_fptype m_gfac;


            //NETLIB_CONSTRUCTOR_EX(VCCS, nl_fptype ari = nlconst::magic(1e9))
            public nld_VCCS(object owner, string name) : this(owner, name, nlconst.magic(1e9)) { }
            public nld_VCCS(object owner, string name, nl_fptype ari)
                : base(owner, name)
            {
                m_G = new param_fp_t(this, "G", nlconst.one());
                m_RI = new param_fp_t(this, "RI", ari);

                m_OP = new terminal_t(this, "OP");//, &m_IP);
                m_ON = new terminal_t(this, "ON");//, &m_IP);
                m_IP = new terminal_t(this, "IP");//, &m_IN);   // <= this should be NULL and terminal be filtered out prior to solving...
                m_IN = new terminal_t(this, "IN");//, &m_IP);   // <= this should be NULL and terminal be filtered out prior to solving...
                m_OP1 = new terminal_t(this, "_OP1");//, &m_IN);
                m_ON1 = new terminal_t(this, "_ON1");//, &m_IN);
                m_OP.terminal_t_after_ctor(m_IP);
                m_ON.terminal_t_after_ctor(m_IP);
                m_IP.terminal_t_after_ctor(m_IN);
                m_IN.terminal_t_after_ctor(m_IP);
                m_OP1.terminal_t_after_ctor(m_IN);
                m_ON1.terminal_t_after_ctor(m_IN);

                m_gfac = nlconst.one();


                connect(m_OP, m_OP1);
                connect(m_ON, m_ON1);
                m_gfac = nlconst.one();
            }


            //NETLIB_RESETI();
            //NETLIB_RESET(VCCS)
            public override void reset()
            {
                nl_fptype m_mult = m_G.op() * m_gfac; // 1.0 ==> 1V ==> 1A
                nl_fptype GI = plib.pglobal.reciprocal(m_RI.op());

                m_IP.set_conductivity(GI);
                m_IN.set_conductivity(GI);

                m_OP.set_go_gt(-m_mult, nlconst.zero());
                m_OP1.set_go_gt(m_mult, nlconst.zero());

                m_ON.set_go_gt(m_mult, nlconst.zero());
                m_ON1.set_go_gt(-m_mult, nlconst.zero());
            }


            //NETLIB_UPDATEI();
            //NETLIB_UPDATE(VCCS)
            public override void update()
            {
                // only called if connected to a rail net ==> notify the solver to recalculate
                if (!m_IP.net().is_rail_net())
                    m_IP.solve_now();
                else if (!m_IN.net().is_rail_net())
                    m_IN.solve_now();
                else if (!m_OP.net().is_rail_net())
                    m_OP.solve_now();
                else if (!m_ON.net().is_rail_net())
                    m_ON.solve_now();
            }


            //NETLIB_UPDATE_PARAMI()
            public override void update_param()
            {
                this.reset();
            }
        }


        //NETLIB_OBJECT_DERIVED(LVCCS, VCCS)

        //NETLIB_OBJECT_DERIVED(CCCS, VCCS)


        //NETLIB_OBJECT_DERIVED(VCVS, VCCS)
        class nld_VCVS : nld_VCCS
        {
            public param_fp_t m_RO;

            terminal_t m_OP2;
            terminal_t m_ON2;


            //NETLIB_CONSTRUCTOR_DERIVED(VCVS, VCCS)
            public nld_VCVS(object owner, string name)
                : base(owner, name)
            { 
                m_RO = new param_fp_t(this, "RO", nlconst.one());

                m_OP2 = new terminal_t(this, "_OP2");//, &m_ON2);
                m_ON2 = new terminal_t(this, "_ON2");//, &m_OP2);
                m_OP2.terminal_t_after_ctor(m_ON2);
                m_ON2.terminal_t_after_ctor(m_OP2);


                connect(m_OP2, m_OP1);
                connect(m_ON2, m_ON1);
            }


            //NETLIB_RESETI();
            //NETLIB_RESET(VCVS)
            public override void reset()
            {
                m_gfac = plib.pglobal.reciprocal(m_RO.op());
                base.reset();

                m_OP2.set_conductivity(m_gfac);
                m_ON2.set_conductivity(m_gfac);
            }


            //NETLIB_UPDATEI();
            //NETLIB_UPDATE_PARAMI();
        }


        //NETLIB_OBJECT_DERIVED(CCVS, VCCS)
    }
}
