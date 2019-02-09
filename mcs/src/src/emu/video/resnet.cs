// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using s16 = System.Int16;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;


namespace mame
{
    /* Structures */

    public class res_net_channel_info
    {
        // per channel options
        public u32     options;
        // Pullup resistor value in Ohms
        public double  rBias;
        // Pulldown resistor value in Ohms
        public double  rGnd;
        // Number of inputs connected to resistors
        public int     num;
        // Resistor values
        // - Least significant bit first
        public double  [] R = new double[8];
        // Minimum output voltage
        // - Applicable if output is routed through a complimentary
        // - darlington circuit
        // - typical value ~ 0.9V
        public double  minout;
        // Cutoff output voltage
        // - Applicable if output is routed through 1:1 transistor amplifier
        // - Typical value ~ 0.7V
        public double  cut;
        // Voltage at the pullup resistor
        // - Typical voltage ~5V
        public double  vBias;


        public res_net_channel_info
            (
                u32     options,
                double  rBias,
                double  rGnd,
                int     num,
                double  [] R
            )
        {
            this.options = options;
            this.rBias = rBias;
            this.rGnd = rGnd;
            this.num = num;
            Array.Copy(R, this.R, R.Length);
        }
    }


    public class res_net_info
    {
        // global options
        public u32     options;
        // The three color channels
        public res_net_channel_info [] rgb = new res_net_channel_info[3];
        // Supply Voltage
        // - Typical value 5V
        public double  vcc;
        // High Level output voltage
        // - TTL : 3.40V
        // - CMOS: 4.95V (@5v vcc)
        public double  vOL;
        // Low Level output voltage
        // - TTL : 0.35V
        // - CMOS: 0.05V (@5v vcc)
        public double  vOH;
        // Open Collector flag
        public u8      OpenCol;


        public res_net_info
            (
                u32     options,
                res_net_channel_info [] rgb
            )
        {
            this.options = options;
            Array.Copy(rgb, this.rgb, rgb.Length);
        }
    }


    public class res_net_decode_info
    {
        const int RES_NET_MAX_COMP    = 3;

        public int numcomp;
        public int start;
        public int end;
        public u16 [] offset = new u16[3 * RES_NET_MAX_COMP];
        public s16 [] shift = new s16[3 * RES_NET_MAX_COMP];
        public u16 [] mask = new u16[3 * RES_NET_MAX_COMP];


        public res_net_decode_info
            (
                int numcomp,
                int start,
                int end,
                u16 [] offset,
                s16 [] shift,
                u16 [] mask
            )
        {
            this.numcomp = numcomp;
            this.start = start;
            this.end = end;
            Array.Copy(offset, this.offset, offset.Length);
            Array.Copy(shift, this.shift, shift.Length);
            Array.Copy(mask, this.mask, mask.Length);
        }
    }


    public static class resnet_global
    {
        public static bool VERBOSE = false;


        /* Amplifier stage per channel but may be specified globally as default */

        public const u32 RES_NET_AMP_USE_GLOBAL      = 0x0000;
        public const u32 RES_NET_AMP_NONE            = 0x0001;      //Out0
        public const u32 RES_NET_AMP_DARLINGTON      = 0x0002;      //Out1
        public const u32 RES_NET_AMP_EMITTER         = 0x0003;      //Out2
        public const u32 RES_NET_AMP_CUSTOM          = 0x0004;      //Out3
        public const u32 RES_NET_AMP_MASK            = 0x0007;

        /* VCC prebuilds - Global */

        public const u32 RES_NET_VCC_5V              = 0x0000;
        public const u32 RES_NET_VCC_CUSTOM          = 0x0008;
        public const u32 RES_NET_VCC_MASK            = 0x0008;

        /* VBias prebuilds - per channel but may be specified globally as default */

        public const u32 RES_NET_VBIAS_USE_GLOBAL    = 0x0000;
        public const u32 RES_NET_VBIAS_5V            = 0x0010;
        public const u32 RES_NET_VBIAS_TTL           = 0x0020;
        public const u32 RES_NET_VBIAS_CUSTOM        = 0x0030;
        public const u32 RES_NET_VBIAS_MASK          = 0x0030;

        /* Input Voltage levels - Global */

        public const u32 RES_NET_VIN_OPEN_COL        = 0x0000;
        public const u32 RES_NET_VIN_VCC             = 0x0100;
        public const u32 RES_NET_VIN_TTL_OUT         = 0x0200;
        public const u32 RES_NET_VIN_CUSTOM          = 0x0300;
        public const u32 RES_NET_VIN_MASK            = 0x0300;

        /* Monitor options */

        // Just invert the signal
        public const u32 RES_NET_MONITOR_INVERT      = 0x1000;
        // SANYO_EZV20 / Nintendo with inverter circuit
        public const u32 RES_NET_MONITOR_SANYO_EZV20 = 0x2000;
        // Electrohome G07 Series
        // 5.6k input impedance
        public const u32 RES_NET_MONITOR_ELECTROHOME_G07 = 0x3000;

        public const u32 RES_NET_MONITOR_MASK        = 0x3000;

        /* General defines */

        const int RES_NET_CHAN_RED            = 0x00;
        const int RES_NET_CHAN_GREEN          = 0x01;
        const int RES_NET_CHAN_BLUE           = 0x02;

        /* Some aliases */

        //#define RES_NET_VIN_MB7051          RES_NET_VIN_TTL_OUT
        public const u32 RES_NET_VIN_MB7052          = RES_NET_VIN_TTL_OUT;
        //#define RES_NET_VIN_MB7053          RES_NET_VIN_TTL_OUT
        //#define RES_NET_VIN_28S42           RES_NET_VIN_TTL_OUT


        /* this should be moved to one of the core files */

        const int MAX_NETS = 3;
        const int MAX_RES_PER_NET = 18;


        /* Datasheets give a maximum of 0.4V to 0.5V
         * However in the circuit simulated here this will only
         * occur if (rBias + rOutn) = 50 Ohm, rBias exists.
         * This is highly unlikely. With the resistor values used
         * in such circuits VOL is likely to be around 50mV.
         */

        const double TTL_VOL         = 0.05;


        /* Likely, datasheets give a typical value of 3.4V to 3.6V
         * for VOH. Modelling the TTL circuit however backs a value
         * of 4V for typical currents involved in resistor networks.
         */

        const double TTL_VOH         = 4.0;


        /* return a single value for one channel */
        public static int compute_res_net(int inputs, int channel, res_net_info di)
        {
            double rTotal = 0.0;
            double v = 0;
            int    i;

            double vBias = di.rgb[channel].vBias;
            double vOH = di.vOH;
            double vOL = di.vOL;
            double minout = di.rgb[channel].minout;
            double cut = di.rgb[channel].cut;
            double vcc = di.vcc;
            double ttlHRes = 0;
            double rGnd = di.rgb[channel].rGnd;
            u8     OpenCol = di.OpenCol;

            /* Global options */

            switch (di.options & RES_NET_AMP_MASK)
            {
                case RES_NET_AMP_USE_GLOBAL:
                    /* just ignore */
                    break;
                case RES_NET_AMP_NONE:
                    minout = 0.0;
                    cut = 0.0;
                    break;
                case RES_NET_AMP_DARLINGTON:
                    minout = 0.9;
                    cut = 0.0;
                    break;
                case RES_NET_AMP_EMITTER:
                    minout = 0.0;
                    cut = 0.7;
                    break;
                case RES_NET_AMP_CUSTOM:
                    /* Fall through */
                    break;
                default:
                    global_object.fatalerror("compute_res_net: Unknown amplifier type\n");
                    break;
            }

            switch (di.options & RES_NET_VCC_MASK)
            {
                case RES_NET_VCC_5V:
                    vcc = 5.0;
                    break;
                case RES_NET_VCC_CUSTOM:
                    /* Fall through */
                    break;
                default:
                    global_object.fatalerror("compute_res_net: Unknown vcc type\n");
                    break;
            }

            switch (di.options & RES_NET_VBIAS_MASK)
            {
                case RES_NET_VBIAS_USE_GLOBAL:
                    /* just ignore */
                    break;
                case RES_NET_VBIAS_5V:
                    vBias = 5.0;
                    break;
                case RES_NET_VBIAS_TTL:
                    vBias = TTL_VOH;
                    break;
                case RES_NET_VBIAS_CUSTOM:
                    /* Fall through */
                    break;
                default:
                    global_object.fatalerror("compute_res_net: Unknown vcc type\n");
                    break;
            }

            switch (di.options & RES_NET_VIN_MASK)
            {
                case RES_NET_VIN_OPEN_COL:
                    OpenCol = 1;
                    vOL = TTL_VOL;
                    break;
                case RES_NET_VIN_VCC:
                    vOL = 0.0;
                    vOH = vcc;
                    OpenCol = 0;
                    break;
                case RES_NET_VIN_TTL_OUT:
                    vOL = TTL_VOL;
                    vOH = TTL_VOH;
                    /* rough estimation from 82s129 (7052) datasheet and from various sources
                     * 1.4k / 30
                     */
                    ttlHRes = 50;
                    OpenCol = 0;
                    break;
                case RES_NET_VIN_CUSTOM:
                    /* Fall through */
                    break;
                default:
                    global_object.fatalerror("compute_res_net: Unknown vin type\n");
                    break;
            }

            /* Per channel options */

            switch (di.rgb[channel].options & RES_NET_AMP_MASK)
            {
                case RES_NET_AMP_USE_GLOBAL:
                    /* use global defaults */
                    break;
                case RES_NET_AMP_NONE:
                    minout = 0.0;
                    cut = 0.0;
                    break;
                case RES_NET_AMP_DARLINGTON:
                    minout = 0.7;
                    cut = 0.0;
                    break;
                case RES_NET_AMP_EMITTER:
                    minout = 0.0;
                    cut = 0.7;
                    break;
                case RES_NET_AMP_CUSTOM:
                    /* Fall through */
                    break;
                default:
                    global_object.fatalerror("compute_res_net: Unknown amplifier type\n");
                    break;
            }

            switch (di.rgb[channel].options & RES_NET_VBIAS_MASK)
            {
                case RES_NET_VBIAS_USE_GLOBAL:
                    /* use global defaults */
                    break;
                case RES_NET_VBIAS_5V:
                    vBias = 5.0;
                    break;
                case RES_NET_VBIAS_TTL:
                    vBias = TTL_VOH;
                    break;
                case RES_NET_VBIAS_CUSTOM:
                    /* Fall through */
                    break;
                default:
                    global_object.fatalerror("compute_res_net: Unknown vcc type\n");
                    break;
            }

            /* Input impedances */

            switch (di.options & RES_NET_MONITOR_MASK)
            {
                case RES_NET_MONITOR_INVERT:
                case RES_NET_MONITOR_SANYO_EZV20:
                    /* Nothing */
                    break;
                case RES_NET_MONITOR_ELECTROHOME_G07:
                    if (rGnd != 0.0)
                        rGnd = rGnd * 5600 / (rGnd + 5600);
                    else
                        rGnd = 5600;
                    break;
            }

            /* compute here - pass a / low inputs */

            for (i=0; i<di.rgb[channel].num; i++)
            {
                int level = ((inputs >> i) & 1);
                if (di.rgb[channel].R[i] != 0.0 && level == 0)
                {
                    // There is no difference in the calculation of the "low" input
                    // (transistor conducting to ground) between TTL output and
                    // open collector output. This is documented explicitly in the
                    // code below (no difference if / else.
                    if (OpenCol != 0)
                    {
                        rTotal += 1.0 / di.rgb[channel].R[i];
                        v += vOL / di.rgb[channel].R[i];
                    }
                    else
                    {
                        rTotal += 1.0 / di.rgb[channel].R[i];
                        v += vOL / di.rgb[channel].R[i];
                    }
                }
            }

            /* Mix in rbias and rgnd */
            if ( di.rgb[channel].rBias != 0.0 )
            {
                rTotal += 1.0 / di.rgb[channel].rBias;
                v += vBias / di.rgb[channel].rBias;
            }
            if (rGnd != 0.0)
                rTotal += 1.0 / rGnd;

            /* if the resulting voltage after application of all low inputs is
             * greater than vOH, treat high inputs as open collector/high impedance
             * There will be now current into/from the TTL gate
             */

            if ( (di.options & RES_NET_VIN_MASK)==RES_NET_VIN_TTL_OUT)
            {
                if (v / rTotal > vOH)
                    OpenCol = 1;
            }

            /* Second pass - high inputs */

            for (i=0; i<di.rgb[channel].num; i++)
            {
                int level = ((inputs >> i) & 1);
                if (di.rgb[channel].R[i] != 0.0 && level != 0)
                {
                    if (OpenCol != 0)
                    {
                        rTotal += 0;
                        v += 0;
                    }
                    else
                    {
                        rTotal += 1.0 / (di.rgb[channel].R[i] + ttlHRes);
                        v += vOH / (di.rgb[channel].R[i] + ttlHRes);
                    }
                }
            }

            rTotal = 1.0 / rTotal;
            v *= rTotal;
            v = Math.Max(minout, v - cut);

            switch (di.options & RES_NET_MONITOR_MASK)
            {
                case RES_NET_MONITOR_INVERT:
                    v = vcc - v;
                    break;
                case RES_NET_MONITOR_SANYO_EZV20:
                    v = vcc - v;
                    v = Math.Max((double)0, v-0.7);
                    v = Math.Min(v, vcc - 2 * 0.7);
                    v = v / (vcc-1.4);
                    v = v * vcc;
                    break;
                case RES_NET_MONITOR_ELECTROHOME_G07:
                    /* Nothing */
                    break;
            }

            return (int) (v * 255 / vcc + 0.4);
        }


        /* compute all values */
        public static void compute_res_net_all(out std.vector<rgb_t> rgb, ListBytesPointer prom, res_net_decode_info rdi, res_net_info di)  //std::vector<rgb_t> &rgb, const u8 *prom, const res_net_decode_info &rdi, const res_net_info &di);
        {
            u8 r;
            u8 g;
            u8 b;
            int i;
            int j;
            int k;

            rgb = new std.vector<rgb_t>();
            rgb.resize(rdi.end - rdi.start + 1);
            for (i = rdi.start; i <= rdi.end; i++)
            {
                u8 [] t = new u8[3] {0, 0, 0};
                int s;
                for (j = 0; j < rdi.numcomp; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        s = rdi.shift[3*j+k];
                        if (s > 0)
                            t[k] = (u8)(t[k] | ((prom[i + rdi.offset[3 * j + k]] >> s) & rdi.mask[3 * j + k]));
                        else
                            t[k] = (u8)(t[k] | ((prom[i + rdi.offset[3 * j + k]] << (0 - s)) & rdi.mask[3 * j + k]));
                    }
                }

                r = (u8)compute_res_net(t[0], RES_NET_CHAN_RED, di);
                g = (u8)compute_res_net(t[1], RES_NET_CHAN_GREEN, di);
                b = (u8)compute_res_net(t[2], RES_NET_CHAN_BLUE, di);
                rgb[i - rdi.start] = new rgb_t(r, g, b);
            }
        }


        /* legacy interface */

        //namespace emu { namespace detail {
        //
        //template <std::size_t I, typename T, std::size_t N, typename U>
        //constexpr auto combine_weights(T const (&tab)[N], U w) { return tab[I] * w; }
        //
        //template <std::size_t I, typename T, std::size_t N, typename U, typename... V>
        //constexpr auto combine_weights(T const (&tab)[N], U w0, V... w) { return (tab[I] * w0) + combine_weights<I + 1>(tab, w...); }
        //
        //} } // namespace emu::detail

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
                global_object.osd_printf_info("compute_resistor_weights():  scaler = %{0}\n",scale);  // %15.10f
                global_object.osd_printf_info("min val :{0}  max val:{1}  Total number of networks :{2}\n", minval, maxval, networks_no);  // %i

                for (i = 0; i < networks_no;i++)
                {
                    double sum = 0.0;

                    global_object.osd_printf_info(" Network no.{0}=>  resistances: {1}", i, rescount[i]);  // %i
                    if (r_pu[i] != 0)
                        global_object.osd_printf_info(", pullup resistor: {0} Ohms", r_pu[i]);  // %i
                    if (r_pd[i] != 0)
                        global_object.osd_printf_info(", pulldown resistor: {0} Ohms", r_pd[i]);  // %i
                    global_object.osd_printf_info("\n  maximum output of this network:{0} (scaled to {1})\n", max_out[i], max_out[i] * scale);  // :%10.5f (scaled to %15.10f)
                    for (n = 0; n < rescount[i]; n++)
                    {
                        global_object.osd_printf_info("   res {0}:{1} Ohms  weight={2} (scaled = {3})\n", n, r[i, n], w[i, n], ws[i, n]);  //    res %2i:%9.1f Ohms  weight=%10.5f (scaled = %15.10f)\n
                        sum += ws[i, n];
                    }
                    global_object.osd_printf_info("                              sum of scaled weights = {0}\n", sum);  // %15.10f
                }
            }
            /* debug end */

            return scale;
        }


        //template <typename T, std::size_t N, typename... U>
        //constexpr int combine_weights(T const (&tab)[N], U... w)
        //{
        //    return int(emu::detail::combine_weights<0U>(tab, w...) + 0.5);
        //}


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
