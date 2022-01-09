// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    public static class nlmod_rtest_global
    {
        //NETLIST_START(RTEST)
        public static void netlist_RTEST(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            RES(R1, RES_K(10))
            ALIAS(1, R1.1)
            ALIAS(2, R1.2)
#endif

            h.NETLIST_END();
        }
    }
}
