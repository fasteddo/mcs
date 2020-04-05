// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    namespace devices
    {
        public static class net_lib_global
        {
            //#define SOLVER(name, freq)                                                  \
            //        NET_REGISTER_DEV(SOLVER, name)                                      \
            //        PARAM(name.FREQ, freq)
            public static void SOLVER(nlparse_t setup, string name, int freq)
            {
                nl_setup_global.NET_REGISTER_DEV(setup, "SOLVER", name);
                nl_setup_global.PARAM(setup, name + ".FREQ", freq);
            }


            static factory.list_t initialize_factory_factory = null;

            //#define xstr(s) # s

            //#define NETLIB_DEVICE_DECL(chip) extern factory::constructor_ptr_t decl_ ## chip;

            static void LIB_DECL(string nic, factory.constructor_ptr_t decl) { initialize_factory_factory.register_device(decl(nic)); }  //#define LIB_DECL(nic, decl) factory.register_device( decl ( pstring(xstr(nic))) );
            static void LIB_ENTRY(string nic, factory.constructor_ptr_t decl) { LIB_DECL("nld_" + nic, decl); }  //#define LIB_ENTRY(nic) { NETLIB_DEVICE_DECL(nic); LIB_DECL(NETLIB_NAME(nic), decl_ ## nic) }


            public static void initialize_factory(factory.list_t factory)
            {
                initialize_factory_factory = factory;

                LIB_ENTRY("R", analog.nld_R.decl_R);
                LIB_ENTRY("POT", analog.nld_POT.decl_POT);
                //LIB_ENTRY(POT2)
                LIB_ENTRY("C", analog.nld_C.decl_C);
                //LIB_ENTRY(L)
                //LIB_ENTRY(D)
                //LIB_ENTRY(VS)
                //LIB_ENTRY(CS)
                //LIB_ENTRY(VCVS)
                //LIB_ENTRY(VCCS)
                //LIB_ENTRY(CCCS)
                //LIB_ENTRY(LVCCS)
                //LIB_ENTRY(opamp)
                LIB_ENTRY("dummy_input", nld_dummy_input.decl_dummy_input);
                //LIB_ENTRY(frontier)   // not intended to be used directly
                //LIB_ENTRY(function)   // only for macro devices - NO FEEDBACK loops
                //LIB_ENTRY(QBJT_EB)
                //LIB_ENTRY(QBJT_switch)
                //LIB_ENTRY(logic_input_ttl)
                LIB_ENTRY("logic_input", nld_logic_input.decl_logic_input);
                LIB_ENTRY("analog_input", nld_analog_input.decl_analog_input);
                //LIB_ENTRY(log)
                //LIB_ENTRY(logD)
                //LIB_ENTRY(clock)
                //LIB_ENTRY(extclock)
                //LIB_ENTRY(mainclock)
                LIB_ENTRY("gnd", nld_gnd.decl_gnd);
                LIB_ENTRY("netlistparams", nld_netlistparams.decl_netlistparams);
                LIB_ENTRY("solver", nld_solver.decl_solver);
                //LIB_ENTRY(res_sw)
                //LIB_ENTRY(switch1)
                //LIB_ENTRY(switch2)
                //LIB_ENTRY(nicRSFF)
                //LIB_ENTRY(nicDelay)
                //LIB_ENTRY(2102A)
                //LIB_ENTRY(2102A_dip)
                //LIB_ENTRY(2716)
                //LIB_ENTRY(2716_dip)
                //LIB_ENTRY(7448)
                //LIB_ENTRY(7448_dip)
                //LIB_ENTRY(7450)
                //LIB_ENTRY(7450_dip)
                //LIB_ENTRY(7473)
                //LIB_ENTRY(7473_dip)
                //LIB_ENTRY(7473A)
                //LIB_ENTRY(7473A_dip)
                //LIB_ENTRY(7474)
                //LIB_ENTRY(7474_dip)
                //LIB_ENTRY(7475)
                //LIB_ENTRY(7475_dip)
                //LIB_ENTRY(7477)
                //LIB_ENTRY(7477_dip)
                //LIB_ENTRY(7483)
                //LIB_ENTRY(7483_dip)
                //LIB_ENTRY(7485)
                //LIB_ENTRY(7485_dip)
                //LIB_ENTRY(7490)
                //LIB_ENTRY(7490_dip)
                //LIB_ENTRY(7493)
                //LIB_ENTRY(7493_dip)
                //LIB_ENTRY(7497)
                //LIB_ENTRY(7497_dip)
                //LIB_ENTRY(74107)
                //LIB_ENTRY(74107_dip)
                //LIB_ENTRY(74107A)    // FIXME: implement missing DIP
                //LIB_ENTRY(74123)
                //LIB_ENTRY(74123_dip)
                //LIB_ENTRY(74153)
                //LIB_ENTRY(74153_dip)
                //LIB_ENTRY(74161)
                //LIB_ENTRY(74161_dip)
                //LIB_ENTRY(74164)
                //LIB_ENTRY(74164_dip)
                //LIB_ENTRY(74165)
                //LIB_ENTRY(74165_dip)
                //LIB_ENTRY(74166)
                //LIB_ENTRY(74166_dip)
                //LIB_ENTRY(74174)
                //LIB_ENTRY(74175)
                //LIB_ENTRY(74192)
                //LIB_ENTRY(74193)
                //LIB_ENTRY(74194)
                //LIB_ENTRY(74365)
                ////ENTRY(74279,              TTL_74279,              "") // only dip available
                //LIB_ENTRY(SN74LS629)
                //LIB_ENTRY(82S16)
                //LIB_ENTRY(82S115)
                //LIB_ENTRY(82S123)
                //LIB_ENTRY(82S126)
                //LIB_ENTRY(9310)
                //LIB_ENTRY(9314)
                //LIB_ENTRY(9316)
                //LIB_ENTRY(9322)
                //LIB_ENTRY(9334)
                //LIB_ENTRY(AM2847)
                //// FIXME: duplicate?
                //LIB_ENTRY(CD4020_WI)
                //LIB_ENTRY(CD4020)
                //LIB_ENTRY(CD4066_GATE)
                //LIB_ENTRY(CD4316_GATE)
                //LIB_ENTRY(4538_dip)
                //LIB_ENTRY(schmitt_trigger)
                ///* entries with suffix WI are legacy only */
                ////ENTRY(4066,                 CD_4066,              "+A,B")
                //LIB_ENTRY(NE555)
                //LIB_ENTRY(TMS4800)
                //LIB_ENTRY(TMS4800_dip)
                //LIB_ENTRY(r2r_dac)
                //LIB_ENTRY(tristate)
                //LIB_ENTRY(tristate3)
                //LIB_ENTRY(74174_dip)
                //LIB_ENTRY(74175_dip)
                //LIB_ENTRY(74192_dip)
                //LIB_ENTRY(74193_dip)
                //LIB_ENTRY(74194_dip)
                //LIB_ENTRY(74365_dip)
                //LIB_ENTRY(82S16_dip)
                //LIB_ENTRY(82S115_dip)
                //LIB_ENTRY(82S123_dip)
                //LIB_ENTRY(82S126_dip)
                //LIB_ENTRY(9602_dip)
                //LIB_ENTRY(9310_dip)
                //LIB_ENTRY(9314_dip)
                //LIB_ENTRY(9316_dip)
                //LIB_ENTRY(9322_dip)
                //LIB_ENTRY(9334_dip)
                //LIB_ENTRY(AM2847_dip)
                //LIB_ENTRY(SN74LS629_dip)
                //LIB_ENTRY(NE555_dip)
                //LIB_ENTRY(MM5837_dip)

                initialize_factory_factory = null;
            }
        }
    } //namespace devices
} // namespace netlist
