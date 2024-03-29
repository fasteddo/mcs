// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.nlm_base_global;


namespace mame
{
    public static class nlm_base_global
    {
        /* ----------------------------------------------------------------------------
         *  Diode Models
         * ---------------------------------------------------------------------------*/
        //static NETLIST_START(diode_models)
        public static void netlist_diode_models(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.NET_MODEL("D _(IS=1e-15 N=1 NBV=3 IBV=0.001 BV=1E9)");

            h.NET_MODEL("1N914 D(Is=2.52n Rs=.568 N=1.752 Cjo=4p M=.4 tt=20n Iave=200m Vpk=75 mfg=OnSemi type=silicon)");
            // FIXME: 1N916 currently only a copy of 1N914!
            h.NET_MODEL("1N916 D(Is=2.52n Rs=.568 N=1.752 Cjo=4p M=.4 tt=20n Iave=200m Vpk=75 mfg=OnSemi type=silicon)");
            h.NET_MODEL("1N4001 D(Is=14.11n N=1.984 Rs=33.89m Ikf=94.81 Xti=3 Eg=1.11 Cjo=25.89p M=.44 Vj=.3245 Fc=.5 Bv=75 Ibv=10u Tt=5.7u Iave=1 Vpk=50 mfg=GI type=silicon)");
            h.NET_MODEL("1N4002 D(IS=65.4p RS=42.2m BV=100 IBV=5.00u CJO=14.8p M=0.333 N=1.36 TT=2.88u)");
            h.NET_MODEL("1N4148 D(Is=2.52n Rs=.568 N=1.752 Cjo=4p M=.4 tt=20n Iave=200m Vpk=75 mfg=OnSemi type=silicon)");
            h.NET_MODEL("1N4154 D(Is=0.1n Rs=4 N=1.67 Cjo=2p M=.333 tt=3n Iave=150m Vpk=35 Bv=60 Ibv=0.1p mfg=Vishay type=silicon)");
            h.NET_MODEL("1N4454 D(Is=168.1e-21 Rs=.1 N=1 Cjo=2p M=.333 tt=5.771n Iave=400m Vpk=35 Bv=75 Ibv=100u mfg=OnSemi type=silicon)");
            h.NET_MODEL("1S1588 D(Is=2.52n Rs=.568 N=1.752 Cjo=4p M=.4 tt=20n Iave=200m Vpk=75)");

            h.NET_MODEL("1N34A D( Bv=75 Cjo=0.5e-12 Eg=0.67 Ibv=18e-3 Is=2e-7 Rs=7 N=1.3 Vj=0.1 M=0.27 type=germanium)");

            h.NET_MODEL("LedRed D(IS=93.2p RS=42M N=3.73 BV=4 IBV=10U CJO=2.97P VJ=.75 M=.333 TT=4.32U Iave=40m Vpk=4 type=LED)");
            h.NET_MODEL("LedGreen D(IS=93.2p RS=42M N=4.61 BV=4 IBV=10U CJO=2.97P VJ=.75 M=.333 TT=4.32U Iave=40m Vpk=4 type=LED)");
            h.NET_MODEL("LedBlue D(IS=93.2p RS=42M N=7.47 BV=5 IBV=10U CJO=2.97P VJ=.75 M=.333 TT=4.32U Iave=40m Vpk=5 type=LED)");
            h.NET_MODEL("LedWhite D(Is=0.27n Rs=5.65 N=6.79 Cjo=42p Iave=30m Vpk=5 type=LED)");

            // Zdiodes ...

            // not really found anywhere, just took the 5239 and changed the
            // breakdown voltage to 5.1 according to the datasheet
            h.NET_MODEL("1N5231  D(BV=5.1 IBV=0.020 NBV=1)");
            h.NET_MODEL("1N5236B D(BV=7.5 IS=27.5p RS=33.8 N=1.10 CJO=58.2p VJ=0.750 M=0.330 TT=50.1n)");
            h.NET_MODEL("1N5240  D(BV=10 IS=14.4p RS=32.0 N=1.10 CJO=24.1p VJ=0.750 M=0.330 TT=50.1n)");
            h.NET_MODEL("1N5240B D(BV=10 IS=14.4p RS=32.0 N=1.10 CJO=24.1p VJ=0.750 M=0.330 TT=50.1n)");

            h.NETLIST_END();
        }


        /* ----------------------------------------------------------------------------
         *  Mosfet Models
         * ---------------------------------------------------------------------------*/
        //static NETLIST_START(mosfet_models)
        public static void netlist_mosfet_models(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            //NET_MODEL("NMOS _(VTO=0.0 N=1.0 IS=1E-14 KP=2E-5 UO=600 PHI=0.6 LD=0.0 L=1.0 TOX=1E-7 W=1.0 NSUB=0.0 GAMMA=0.0 RD=0.0 RS=0.0 LAMBDA=0.0)")
            //NET_MODEL("PMOS _(VTO=0.0 N=1.0 IS=1E-14 KP=2E-5 UO=600 PHI=0.6 LD=0.0 L=1.0 TOX=1E-7 W=1.0 NSUB=0.0 GAMMA=0.0 RD=0.0 RS=0.0 LAMBDA=0.0)")
        
            // NMOS_DEFAULT and PMOS_DEFAULT are created in nl_setup.cpp
            h.NET_MODEL("NMOS NMOS_DEFAULT(VTO=0.0 N=1.0 IS=1E-14 KP=0.0 UO=600 PHI=0.0 LD=0.0 L=100e-6 TOX=1E-7 W=100e-6 NSUB=0.0 GAMMA=0.0 RD=0.0 RS=0.0 LAMBDA=0.0 CGSO=0 CGDO=0 CGBO=0)");
            h.NET_MODEL("PMOS PMOS_DEFAULT(VTO=0.0 N=1.0 IS=1E-14 KP=0.0 UO=600 PHI=0.0 LD=0.0 L=100e-6 TOX=1E-7 W=100e-6 NSUB=0.0 GAMMA=0.0 RD=0.0 RS=0.0 LAMBDA=0.0 CGSO=0 CGDO=0 CGBO=0)");

            h.NETLIST_END();
        }


        /* ----------------------------------------------------------------------------
         *  BJT Models
         * ---------------------------------------------------------------------------*/
        //static NETLIST_START(bjt_models)
        public static void netlist_bjt_models(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.NET_MODEL("NPN _(IS=1e-15 BF=100 NF=1 BR=1 NR=1 CJE=0 CJC=0)");
            h.NET_MODEL("PNP _(IS=1e-15 BF=100 NF=1 BR=1 NR=1 CJE=0 CJC=0)");

            h.NET_MODEL("2SA1015 PNP(Is=295.1E-18 Xti=3 Eg=1.11 Vaf=100 Bf=110 Xtb=1.5 Br=10.45 Rc=15 Cjc=66.2p Mjc=1.054 Vjc=.75 Fc=.5 Cje=5p Mje=.3333 Vje=.75 Tr=10n Tf=1.661n VCEO=45V ICrating=150M MFG=Toshiba)");
            h.NET_MODEL("2SC1815 NPN(Is=2.04f Xti=3 Eg=1.11 Vaf=6 Bf=400 Ikf=20m Xtb=1.5 Br=3.377 Rc=1 Cjc=1p Mjc=.3333 Vjc=.75 Fc=.5 Cje=25p Mje=.3333 Vje=.75 Tr=450n Tf=20n Itf=0 Vtf=0 Xtf=0 VCEO=45V ICrating=150M MFG=Toshiba)");

            h.NET_MODEL("2N3565 NPN(IS=5.911E-15 ISE=5.911E-15 ISC=0 XTI=3 BF=697.1 BR=1.297 IKF=0.01393 IKR=0 XTB=1.5 VAF=62.37 VAR=21.5 VJE=0.65 VJC=0.65 RE=0.15 RC=1.61 RB=10 CJE=4.973E-12 CJC=4.017E-12 XCJC=0.75 FC=0.5 NF=1 NR=1 NE=1.342 NC=2 MJE=0.4146 MJC=0.3174 TF=820.4E-12 TR=4.687E-9 ITF=0.35 VTF=4 XTF=7 EG=1.11 KF=1E-9 AF=1 VCEO=25 ICRATING=500m MFG=NSC)");
            h.NET_MODEL("2N3643 NPN(IS=14.34E-15 ISE=14.34E-15 ISC=0 XTI=3 BF=255.9 BR=6.092 IKF=0.2847 IKR=0 XTB=1.5 VAF=74.03 VAR=28 VJE=0.65 VJC=0.65 RE=0.1 RC=1 RB=10 CJE=22.01E-12 CJC=7.306E-12 XCJC=0.75 FC=0.5 NF=1 NR=1 NE=1.307 NC=2 MJE=0.377 MJC=0.3416 TF=411.1E-12 TR=46.91E-9 ITF=0.6 VTF=1.7 XTF=3 EG=1.11 KF=0 AF=1 VCEO=30 ICRATING=500m MFG=NSC)");
            h.NET_MODEL("2N3645 PNP(IS=650.6E-18 ISE=54.81E-15 ISC=0 XTI=3 BF=231.7 BR=3.563 IKF=1.079 IKR=0 XTB=1.5 VAF=115.7 VAR=35 VJE=0.65 VJC=0.65 RE=0.15 RC=0.715 RB=10 CJE=19.82E-12 CJC=14.76E-12 XCJC=0.75 FC=0.5 NF=1 NR=1 NE=1.829 NC=2 MJE=0.3357 MJC=0.5383 TF=603.7E-12 TR=111.3E-9 ITF=0.65 VTF=5 XTF=1.7 EG=1.11 KF=0 AF=1 VCEO=60 ICRATING=500m MFG=NSC)");
            // 3644 = 3645 Difference between 3644 and 3645 is voltage rating. 3644: VCBO=45, 3645: VCBO=60
            h.NET_MODEL("2N3644 PNP(IS=650.6E-18 ISE=54.81E-15 ISC=0 XTI=3 BF=231.7 BR=3.563 IKF=1.079 IKR=0 XTB=1.5 VAF=115.7 VAR=35 VJE=0.65 VJC=0.65 RE=0.15 RC=0.715 RB=10 CJE=19.82E-12 CJC=14.76E-12 XCJC=0.75 FC=0.5 NF=1 NR=1 NE=1.829 NC=2 MJE=0.3357 MJC=0.5383 TF=603.7E-12 TR=111.3E-9 ITF=0.65 VTF=5 XTF=1.7 EG=1.11 KF=0 AF=1 VCEO=60 ICRATING=500m MFG=NSC)");
            h.NET_MODEL("2N3702 PNP(Is=650.6E-18 Xti=3 Eg=1.11 Vaf=115.7 Bf=133.8 Ne=1.832 Ise=97.16f Ikf=1.081 Xtb=1.5 Br=3.73 Nc=2 Isc=0 Ikr=0 Rc=.715 Cjc=14.76p Mjc=.5383 Vjc=.75 Fc=.5 Cje=19.82p Mje=.3357 Vje=.75 Tr=114.1n Tf=761.3p Itf=.65 Vtf=5 Xtf=1.7 Rb=10 mfg=National)");
            h.NET_MODEL("2N3704 NPN(IS=26.03f VAF=90.7 Bf=736.1K IKF=.1983 XTB=1.5 BR=1.024 CJC=11.01p CJE=24.07p RB=10 RC=.5 RE=.5 TR=233.8n TF=1.03n ITF=0 VTF=0 XTF=0 mfg=Motorola)");
            // SPICE model parameters taken from Fairchild Semiconductor datasheet
            h.NET_MODEL("2N3904 NPN(IS=1E-14 VAF=100 Bf=300 IKF=0.4 XTB=1.5 BR=4 CJC=4E-12 CJE=8E-12 RB=20 RC=0.1 RE=0.1 TR=250E-9 TF=350E-12 ITF=1 VTF=2 XTF=3 Vceo=40 Icrating=200m mfg=Philips)");
            // SPICE model parameters taken from Fairchild Semiconductor datasheet
            h.NET_MODEL("2N3906 PNP(Is=1.41f Xti=3 Eg=1.11 Vaf=18.7 Bf=180.7 Ne=1.5 Ise=0 Ikf=80m Xtb=1.5 Br=4.977 Nc=2 Isc=0 Ikr=0 Rc=2.5 Cjc=9.728p Mjc=.5776 Vjc=.75 Fc=.5 Cje=8.063p Mje=.3677 Vje=.75 Tr=33.42n Tf=179.3p Itf=.4 Vtf=4 Xtf=6 Rb=10)");
            // 2N5190 = BC817-25
            h.NET_MODEL("2N5190 NPN(IS=9.198E-14 NF=1.003 ISE=4.468E-16 NE=1.65 BF=338.8 IKF=0.4913 VAF=107.9 NR=1.002 ISC=5.109E-15 NC=1.071 BR=29.48 IKR=0.193 VAR=25 RB=1 IRB=1000 RBM=1 RE=0.2126 RC=0.143 XTB=0 EG=1.11 XTI=3 CJE=3.825E-11 VJE=0.7004 MJE=0.364 TF=5.229E-10 XTF=219.7 VTF=3.502 ITF=7.257 PTF=0 CJC=1.27E-11 VJC=0.4431 MJC=0.3983 XCJC=0.4555 TR=7E-11 CJS=0 VJS=0.75 MJS=0.333 FC=0.905 Vceo=45 Icrating=500m mfg=Philips)");

            h.NET_MODEL("2N4401 NPN(IS=26.03f XTI=3 EG=1.11 VAF=90.7 BF=4.292K NE=1.244 ISE=26.03f IKF=0.2061 XTB=1.5 BR=1.01 NC=2 ISC=0 IKR=0 RC=0.5 CJC=11.01p MJC=0.3763 VJC=0.75 FC=0.5 CJE=24.07p MJE=0.3641 VJE=0.75 TR=233.7n TF=466.5p ITF=0 VTF=0 XTF=0 RB=10 VCEO=40)");
            h.NET_MODEL("2N4403 PNP(Is=650.6E-18 Xti=3 Eg=1.11 Vaf=115.7 Bf=216.2 Ne=1.829 Ise=58.72f Ikf=1.079 Xtb=1.5 Br=3.578 Nc=2 Isc=0 Ikr=0 Rc=.715 Cjc=14.76p Mjc=.5383 Vjc=.75 Fc=.5 Cje=19.82p Mje=.3357 Vje=.75 Tr=111.6n Tf=603.7p Itf=.65 Vtf=5 Xtf=1.7 Rb=10)");
            h.NET_MODEL("2N4124 NPN(IS=6.734f XTI=3 EG=1.11 VAF=74.03 BF=495 NE=1.28 ISE=6.734f IKF=69.35m XTB=1.5 BR=0.7214 NC=2 ISC=0 IKR=0 RC=1 CJC=3.638p MJC=0.3085 VJC=0.75 FC=0.5 CJE=4.493p MJE=0.2593 VJE=0.75 TR=238.3n TF=301.3p ITF=.4 VTF=4 XTF=2 RB=10 VCEO=25)");
            h.NET_MODEL("2N4126 PNP(IS=1.41f XTI=3 EG=1.11 VAF=18.7 BF=203.7 NE=1.5 ISE=0 IKF=80m XTB=1.5 BR=4.924 NC=2 ISC=0 IKR=0 RC=2.5 CJC=9.728p MJC=0.5776 VJC=0.75 FC=0.5 CJE=8.063p MJE=0.3677 VJE=0.75 TR=33.23n TF=179.3p ITF=.4 VTF=4 XTF=6 RB=10 VCEO=25)");
            // SPICE model parameters taken from http://ltwiki.org/files/LTspiceIV/Vendor%20List/Fairchild/2N/index.html
            h.NET_MODEL("2N5210 NPN(Is=5.911f Xti=3 Eg=1.11 Vaf=62.37 Bf=809.9 Ne=1.358 Ise=5.911f Ikf=14.26m Xtb=1.5 Br=1.287 Nc=2 Isc=0 Ikr=0 Rc=1.61 Cjc=4.017p Mjc=.3174 Vjc=.75 Fc=.5 Cje=4.973p Mje=.4146 Vje=.75 Tr=4.68n Tf=820.9p Itf=.35 Vtf=4 Xtf=7 Rb=10)");
            // SPICE model parameters taken from https://www.onsemi.com/support/design-resources/models?rpn=2N6107
            h.NET_MODEL("2N6107 PNP(IS=7.62308e-14 BF=6692.56 NF=0.85 VAF=10 IKF=0.032192 ISE=2.07832e-13 NE=2.41828 BR=15.6629 NR=1.5 VAR=1.44572 IKR=0.32192 ISC=4.75e-16 NC=3.9375 RB=7.19824 IRB=0.1 RBM=0.1 RE=0.0001 RC=0.355458 XTB=0.1 XTI=2.97595 EG=1.206 CJE=1.84157e-10 VJE=0.99 MJE=0.347177 TF=6.63757e-09 XTF=1.50003 VTF=1.0001 ITF=1 CJC=1.06717e-10 VJC=0.942679 MJC=0.245405 XCJC=0.8 FC=0.533334 CJS=0 VJS=0.75 MJS=0.5 TR=1.32755e-07 PTF=0 KF=0 AF=1)");
            // SPICE model parameters taken from https://www.onsemi.com/support/design-resources/models?rpn=2N6292
            h.NET_MODEL("2N6292 NPN(IS=9.3092e-13 BF=2021.8 NF=0.85 VAF=63.2399 IKF=1 ISE=1.92869e-13 NE=1.97024 BR=40.0703 NR=1.5 VAR=0.89955 IKR=10 ISC=4.92338e-16 NC=3.9992 RB=6.98677 IRB=0.1 RBM=0.1 RE=0.0001 RC=0.326141 XTB=0.1 XTI=2.86739 EG=1.206 CJE=1.84157e-10 VJE=0.99 MJE=0.347174 TF=6.73756e-09 XTF=1.49917 VTF=0.997395 ITF=0.998426 CJC=1.06717e-10 VJC=0.942694 MJC=0.245406 XCJC=0.8 FC=0.533405 CJS=0 VJS=0.75 MJS=0.5 TR=6.0671e-08 PTF=0 KF=0 AF=1)");

            h.NET_MODEL("2SC945 NPN(IS=3.577E-14 BF=2.382E+02 NF=1.01 VAF=1.206E+02 IKF=3.332E-01 ISE=3.038E-16 NE=1.205 BR=1.289E+01 NR=1.015 VAR=1.533E+01 IKR=2.037E-01 ISC=3.972E-14 NC=1.115 RB=3.680E+01 IRB=1.004E-04 RBM=1 RE=8.338E-01 RC=1.557E+00 CJE=1.877E-11 VJE=7.211E-01 MJE=3.486E-01 TF=4.149E-10 XTF=1.000E+02 VTF=9.956 ITF=5.118E-01 PTF=0 CJC=6.876p VJC=3.645E-01 MJC=3.074E-01 TR=5.145E-08 XTB=1.5 EG=1.11 XTI=3 FC=0.5 Vceo=50 Icrating=100m MFG=NEC)");

            h.NET_MODEL("BC237B NPN(IS=1.8E-14 ISE=5.0E-14 ISC=1.72E-13 XTI=3 BF=400 BR=35.5 IKF=0.14 IKR=0.03 XTB=1.5 VAF=80 VAR=12.5 VJE=0.58 VJC=0.54 RE=0.6 RC=0.25 RB=0.56 CJE=13E-12 CJC=4E-12 XCJC=0.75 FC=0.5 NF=0.9955 NR=1.005 NE=1.46 NC=1.27 MJE=0.33 MJC=0.33 TF=0.64E-9 TR=50.72E-9 EG=1.11 KF=0 AF=1 VCEO=45 ICRATING=100M MFG=ZETEX)");
            h.NET_MODEL("BC556B PNP(IS=3.83E-14 NF=1.008 ISE=1.22E-14 NE=1.528 BF=344.4 IKF=0.08039 VAF=21.11 NR=1.005 ISC=2.85E-13 NC=1.28 BR=14.84 IKR=0.047 VAR=32.02 RB=1 IRB=1.00E-06 RBM=1 RE=0.6202 RC=0.5713 XTB=0 EG=1.11 XTI=3 CJE=1.23E-11 VJE=0.6106 MJE=0.378 TF=5.60E-10 XTF=3.414 VTF=5.23 ITF=0.1483 PTF=0 CJC=1.08E-11 VJC=0.1022 MJC=0.3563 XCJC=0.6288 TR=1.00E-32 CJS=0 VJS=0.75 MJS=0.333 FC=0.8027 Vceo=65 Icrating=100m mfg=Philips)");
            h.NET_MODEL("BC548C NPN(IS=1.95E-14 ISE=1.31E-15 ISC=1.0E-13 XTI=3 BF=466 BR=2.42 IKF=0.18 IKR=1 XTB=1.5 VAF=91.7 VAR=24.7 VJE=0.632 VJC=0.339 RE=1 RC=1.73 RB=26.5 RBM=10 IRB=10 CJE=1.33E-11 CJC=5.17E-12 XCJC=1 FC=0.9 NF=0.993 NR=1.2 NE=1.32 NC=2.00 MJE=0.326 MJC=0.319 TF=6.52E-10 TR=0 PTF=0 ITF=1.03 VTF=1.65 XTF=100 EG=1.11 KF=1E-9 AF=1 VCEO=40 ICrating=800M MFG=Siemens)");
            h.NET_MODEL("BC817-25 NPN(IS=9.198E-14 NF=1.003 ISE=4.468E-16 NE=1.65 BF=338.8 IKF=0.4913 VAF=107.9 NR=1.002 ISC=5.109E-15 NC=1.071 BR=29.48 IKR=0.193 VAR=25 RB=1 IRB=1000 RBM=1 RE=0.2126 RC=0.143 XTB=0 EG=1.11 XTI=3 CJE=3.825E-11 VJE=0.7004 MJE=0.364 TF=5.229E-10 XTF=219.7 VTF=3.502 ITF=7.257 PTF=0 CJC=1.27E-11 VJC=0.4431 MJC=0.3983 XCJC=0.4555 TR=7E-11 CJS=0 VJS=0.75 MJS=0.333 FC=0.905 Vceo=45 Icrating=500m mfg=Philips)");

            h.NET_MODEL("9013 NPN(IS=3.40675E-14 BF=166 VAF=67 IKF=1.164 ISE=12.37e-15 NE=2 BR=15.17 VAR=40.84 IKR=0.261352 ISC=1.905E-15 NC=1.066 RB=63.2 IRB=5.62E-6 RBM=22.1 RE=0.02 RC=0.7426 CJE=3.53E-11 VJE=0.808 MJE=0.372 CJC=1.74E-11 VJC=0.614 MJC=0.388 XCJC=0.349 XTB=1.4025 EG=1.0999 XTI=3 VC=0.5 VCEO=20)");

            h.NETLIST_END();
        }


        /* ----------------------------------------------------------------------------
         *  Family models
         * ---------------------------------------------------------------------------*/
        //static NETLIST_START(family_models)
        public static void netlist_family_models(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            // FAMILIES always need a type. UNKNOWN below will break
            h.NET_MODEL("FAMILY _(TYPE=UNKNOWN IVL=0.16 IVH=0.4 OVL=0.1 OVH=1.0 ORL=1.0 ORH=130.0)");
            h.NET_MODEL("OPAMP _()");
            h.NET_MODEL("SCHMITT_TRIGGER _()");

            h.NET_MODEL("74XX FAMILY(TYPE=TTL IVL=0.16 IVH=0.4 OVL=0.1 OVH=1.0 ORL=1.0 ORH=130)");

            // CMOS
            // low input 1.5 , high input trigger 3.5 at 5V supply
            // output offsets, low for CMOS, thus 0.05
            // output currents: see https://www.classe.cornell.edu/~ib38/teaching/p360/lectures/wk09/l26/EE2301Exp3F10.pdf
            // typical CMOS may sink 0.4mA while output stays <= 0.4V
            h.NET_MODEL("CD4XXX FAMILY(TYPE=CMOS IVL=0.3 IVH=0.7 OVL=0.05 OVH=0.05 ORL=500 ORH=500)");

            h.NET_MODEL("74XXOC FAMILY(TYPE=TTL IVL=0.16 IVH=0.4 OVL=0.1 OVH=0.05 ORL=10.0 ORH=1.0e8)");

            h.NETLIST_END();
        }


        /* ----------------------------------------------------------------------------
         *  Always included
         * ---------------------------------------------------------------------------*/
        //NETLIST_START(base_lib)
        public static void netlist_base_lib(netlist.nlparse_t setup)
        {
            netlist.helper h = new netlist.helper();

            h.NETLIST_START(setup);

            h.NET_REGISTER_DEV("GNDA", "GND");
            h.NET_REGISTER_DEV("PARAMETER", "NETLIST");

            h.LOCAL_SOURCE("diode_models", netlist_diode_models);
            h.LOCAL_SOURCE("bjt_models", netlist_bjt_models);
            h.LOCAL_SOURCE("mosfet_models", netlist_mosfet_models);
            h.LOCAL_SOURCE("family_models", netlist_family_models);

            h.EXTERNAL_SOURCE("ttl74xx_lib", nlm_ttl74xx_global.netlist_ttl74xx_lib);
            h.EXTERNAL_SOURCE("cd4xxx_lib", nlm_cd4xxx_global.netlist_cd4xxx_lib);
            h.EXTERNAL_SOURCE("opamp_lib", nlm_opamp_global.netlist_opamp_lib);
            h.EXTERNAL_SOURCE("otheric_lib", nlm_otheric_global.netlist_otheric_lib);
            h.EXTERNAL_SOURCE("roms_lib", nlm_roms_global.netlist_roms_lib);

            h.EXTERNAL_SOURCE("modules_lib", nlm_modules_global.netlist_modules_lib);

            h.INCLUDE("diode_models");
            h.INCLUDE("bjt_models");
            h.INCLUDE("mosfet_models");
            h.INCLUDE("family_models");

            h.INCLUDE("ttl74xx_lib");
            h.INCLUDE("cd4xxx_lib");
            h.INCLUDE("opamp_lib");
            h.INCLUDE("otheric_lib");
            h.INCLUDE("roms_lib");

            h.INCLUDE("modules_lib");

            h.NETLIST_END();
        }
    }
}
