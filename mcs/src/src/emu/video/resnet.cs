// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class resnet_global
    {
        public static bool VERBOSE = false;


        /* this should be moved to one of the core files */

        const int MAX_NETS = 3;
        const int MAX_RES_PER_NET = 18;


        /* return a single value for one channel */
        //int compute_res_net(int inputs, int channel, const res_net_info &di);


        /* compute all values */
        //void compute_res_net_all(std::vector<rgb_t> &rgb, const u8 *prom, const res_net_decode_info &rdi, const res_net_info &di);


        /* legacy interface */
        public static double compute_resistor_weights(
            int minval, int maxval, double scaler,
            int count_1, int [] resistances_1, out double [] weights_1, int pulldown_1, int pullup_1,
            int count_2, int [] resistances_2, out double [] weights_2, int pulldown_2, int pullup_2,
            int count_3, int [] resistances_3, out double [] weights_3, int pulldown_3, int pullup_3 )
        {
            weights_1 = new double[count_1];
            weights_2 = new double[count_2];
            weights_3 = new double[count_3];

            int networks_no;

            int [] rescount = new int[MAX_NETS];     /* number of resistors in each of the nets */
            double [,] r = new double[MAX_NETS, MAX_RES_PER_NET];        /* resistances */
            double [,] w = new double[MAX_NETS, MAX_RES_PER_NET];        /* calulated weights */
            double [,] ws = new double[MAX_NETS, MAX_RES_PER_NET];   /* calulated, scaled weights */
            int [] r_pd = new int[MAX_NETS];         /* pulldown resistances */
            int [] r_pu = new int[MAX_NETS];         /* pullup resistances */

            double [] max_out = new double[MAX_NETS];
            double [][] out_ = new double[MAX_NETS][]; //double * out_[MAX_NETS];

            int i;
            int j;
            int n;
            double scale;
            double max;

            /* parse input parameters */

            networks_no = 0;

            for (n = 0; n < MAX_NETS; n++)
            {
                int count;
                int pd;
                int pu;
                int [] resistances;  //const int * resistances;
                double [] weights;  //double * weights;

                switch (n)
                {
                case 0:
                        count       = count_1;
                        resistances = resistances_1;
                        weights     = weights_1;
                        pd          = pulldown_1;
                        pu          = pullup_1;
                        break;
                case 1:
                        count       = count_2;
                        resistances = resistances_2;
                        weights     = weights_2;
                        pd          = pulldown_2;
                        pu          = pullup_2;
                        break;
                case 2:
                default:
                        count       = count_3;
                        resistances = resistances_3;
                        weights     = weights_3;
                        pd          = pulldown_3;
                        pu          = pullup_3;
                        break;
                }

                /* parameters validity check */
                if (count > MAX_RES_PER_NET)
                    throw new emu_fatalerror("compute_resistor_weights(): too many resistors in net #{0}. The maximum allowed is {1}, the number requested was: {2}\n", n, MAX_RES_PER_NET, count);

                if (count > 0)
                {
                    rescount[networks_no] = count;
                    for (i = 0; i < count; i++)
                    {
                        r[networks_no, i] = 1.0 * resistances[i];
                    }
                    out_[networks_no] = weights;
                    r_pd[networks_no] = pd;
                    r_pu[networks_no] = pu;
                    networks_no++;
                }
            }

            if (networks_no < 1)
                throw new emu_fatalerror("compute_resistor_weights(): no input data\n");

            /* calculate outputs for all given networks */
            for (i = 0; i < networks_no; i++ )
            {
                double R0;
                double R1;
                double Vout;
                double dst;

                /* of n resistors */
                for (n = 0; n < rescount[i]; n++)
                {
                    R0 = ( r_pd[i] == 0 ) ? 1.0/1e12 : 1.0/r_pd[i];
                    R1 = ( r_pu[i] == 0 ) ? 1.0/1e12 : 1.0/r_pu[i];

                    for( j = 0; j < rescount[i]; j++ )
                    {
                        if( j==n )  /* only one resistance in the network connected to Vcc */
                        {
                            if (r[i, j] != 0.0)
                                R1 += 1.0 / r[i, j];
                        }
                        else
                            if (r[i, j] != 0.0)
                                R0 += 1.0 / r[i, j];
                    }

                    /* now determine the voltage */
                    R0 = 1.0/R0;
                    R1 = 1.0/R1;
                    Vout = (maxval - minval) * R0 / (R1 + R0) + minval;

                    /* and convert it to a destination value */
                    dst = (Vout < minval) ? minval : (Vout > maxval) ? maxval : Vout;

                    w[i, n] = dst;
                }
            }

            /* calculate maximum outputs for all given networks */
            j = 0;
            max = 0.0;
            for (i = 0; i < networks_no; i++ )
            {
                double sum = 0.0;

                /* of n resistors */
                for (n = 0; n < rescount[i]; n++ )
                    sum += w[i, n]; /* maximum output, ie when each resistance is connected to Vcc */

                max_out[i] = sum;
                if (max < sum)
                {
                    max = sum;
                    j = i;
                }
            }


            if (scaler < 0.0)   /* use autoscale ? */
                /* calculate the output scaler according to the network with the greatest output */
                scale = ((double)maxval) / max_out[j];
            else                /* use scaler provided on entry */
                scale = scaler;

            /* calculate scaled output and fill the output table(s)*/
            for (i = 0; i < networks_no;i++)
            {
                for (n = 0; n < rescount[i]; n++)
                {
                    ws[i, n] = w[i, n]*scale;   /* scale the result */
                    (out_[i])[n] = ws[i, n];     /* fill the output table */
                }
            }

            /* debug code */
            if (resnet_global.VERBOSE)
            {
                global.osd_printf_info("compute_resistor_weights():  scaler = %{0}\n",scale);  // %15.10f
                global.osd_printf_info("min val :{0}  max val:{1}  Total number of networks :{2}\n", minval, maxval, networks_no);  // %i

                for (i = 0; i < networks_no;i++)
                {
                    double sum = 0.0;

                    global.osd_printf_info(" Network no.{0}=>  resistances: {1}", i, rescount[i]);  // %i
                    if (r_pu[i] != 0)
                        global.osd_printf_info(", pullup resistor: {0} Ohms", r_pu[i]);  // %i
                    if (r_pd[i] != 0)
                        global.osd_printf_info(", pulldown resistor: {0} Ohms", r_pd[i]);  // %i
                    global.osd_printf_info("\n  maximum output of this network:{0} (scaled to {1})\n", max_out[i], max_out[i] * scale);  // :%10.5f (scaled to %15.10f)
                    for (n = 0; n < rescount[i]; n++)
                    {
                        global.osd_printf_info("   res {0}:{1} Ohms  weight={2} (scaled = {3})\n", n, r[i, n], w[i, n], ws[i, n]);  //    res %2i:%9.1f Ohms  weight=%10.5f (scaled = %15.10f)\n
                        sum += ws[i, n];
                    }
                    global.osd_printf_info("                              sum of scaled weights = {0}\n", sum);  // %15.10f
                }
            }
            /* debug end */

            return scale;
        }


        public static int combine_8_weights(double [] tab, int w0, int w1, int w2, int w3, int w4, int w5, int w6, int w7)  { return (int)(((tab)[0]*(w0) + (tab)[1]*(w1) + (tab)[2]*(w2) + (tab)[3]*(w3) + (tab)[4]*(w4) + (tab)[5]*(w5) + (tab)[6]*(w6) + (tab)[7]*(w7)) + 0.5); }
        public static int combine_7_weights(double [] tab, int w0, int w1, int w2, int w3, int w4, int w5, int w6)          { return (int)(((tab)[0]*(w0) + (tab)[1]*(w1) + (tab)[2]*(w2) + (tab)[3]*(w3) + (tab)[4]*(w4) + (tab)[5]*(w5) + (tab)[6]*(w6)) + 0.5); }
        public static int combine_6_weights(double [] tab, int w0, int w1, int w2, int w3, int w4, int w5)                  { return (int)(((tab)[0]*(w0) + (tab)[1]*(w1) + (tab)[2]*(w2) + (tab)[3]*(w3) + (tab)[4]*(w4) + (tab)[5]*(w5)) + 0.5); }
        public static int combine_5_weights(double [] tab, int w0, int w1, int w2, int w3, int w4)                          { return (int)(((tab)[0]*(w0) + (tab)[1]*(w1) + (tab)[2]*(w2) + (tab)[3]*(w3) + (tab)[4]*(w4)) + 0.5); }
        public static int combine_4_weights(double [] tab, int w0, int w1, int w2, int w3)                                  { return (int)(((tab)[0]*(w0) + (tab)[1]*(w1) + (tab)[2]*(w2) + (tab)[3]*(w3)) + 0.5); }
        public static int combine_3_weights(double [] tab, int w0, int w1, int w2)                                          { return (int)(((tab)[0]*(w0) + (tab)[1]*(w1) + (tab)[2]*(w2)) + 0.5); }
        public static int combine_2_weights(double [] tab, int w0, int w1)                                                  { return (int)(((tab)[0]*(w0) + (tab)[1]*(w1)) + 0.5); }
        public static int combine_1_weights(double [] tab, int w0)                                                          { return (int)(((tab)[0]*(w0) + 0.5)); }


        /* for the open collector outputs PROMs */
        //double compute_resistor_net_outputs(
        //    int minval, int maxval, double scaler,
        //    int count_1, const int * resistances_1, double * outputs_1, int pulldown_1, int pullup_1,
        //    int count_2, const int * resistances_2, double * outputs_2, int pulldown_2, int pullup_2,
        //    int count_3, const int * resistances_3, double * outputs_3, int pulldown_3, int pullup_3 );
    }
}
