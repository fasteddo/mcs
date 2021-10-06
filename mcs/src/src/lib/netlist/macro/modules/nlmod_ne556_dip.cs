// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nlmod_ne556_dip_global
    {
        //NETLIST_START(NE556_DIP)
        public static void netlist_NE556_DIP(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            throw new emu_unimplemented();
#if false
            NE555(A)
            NE555(B)

            NET_C(A.GND, B.GND)
            NET_C(A.VCC, B.VCC)

            DIPPINS(      /*        +--------------+        */
                 A.DISCH, /* 1DISCH |1     ++    14| VCC    */ A.VCC,
                A.THRESH, /* 1THRES |2           13| 2DISCH */ B.DISCH,
                  A.CONT, /*  1CONT |3           12| 2THRES */ B.THRESH,
                 A.RESET, /* 1RESET |4   NE556   11| 2CONT  */ B.CONT,
                   A.OUT, /*   1OUT |5           10| 2RESET */ B.RESET,
                  A.TRIG, /*  1TRIG |6            9| 2OUT   */ B.OUT,
                   A.GND, /*    GND |7            8| 2TRIG  */ B.TRIG
                          /*        +--------------+        */
            )
#endif

            h.NETLIST_END();
        }
    }
}
