// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.netlist.devices.lib_entries_global;
using static mame.netlist.devices.net_lib_global;
using static mame.netlist.nl_setup_global;


namespace mame.netlist.devices
{
    public static class net_lib_global
    {
        //#ifdef RES_R
        //#warning "Do not include rescap.h in a netlist environment"
        //#endif
        //#ifndef RES_R
        //#define RES_R(res) (res)
        public static double RES_K(double res) { return res * 1e3; }
        public static double RES_M(double res) { return res * 1e6; }
        //#define CAP_U(cap) ((cap) * 1e-6)
        //#define CAP_N(cap) ((cap) * 1e-9)
        public static double CAP_P(double cap) { return cap * 1e-12; }
        //#define IND_U(ind) ((ind) * 1e-6)
        //#define IND_N(ind) ((ind) * 1e-9)
        //#define IND_P(ind) ((ind) * 1e-12)
        //#endif


        //#define SOLVER(name, freq)                                                  \
        //        NET_REGISTER_DEVEXT(SOLVER, name, freq)
        public static void SOLVER(nlparse_t setup, string name, int freq)
        {
            NET_REGISTER_DEV(setup, "SOLVER", name);
            PARAM(setup, name + ".FREQ", freq);
        }


        public static void initialize_factory(factory.list_t factory)
        {
            // The following is from a script which automatically creates
            // the entries.
            // FIXME: the list should be either included or the whole
            // initialize factory code should be created programmatically.

            //#include "../generated/lib_entries.hxx"
            lib_entries_global.initialize_factory(factory);
        }
    }
}
