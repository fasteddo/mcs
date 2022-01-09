// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using discrete_device_node_list_t = mame.std.vector<mame.discrete_base_node>;  //typedef std::vector<std::unique_ptr<discrete_base_node> > node_list_t;
using discrete_device_node_step_list_t = mame.std.vector<mame.discrete_step_interface>;  //typedef std::vector<discrete_step_interface *> node_step_list_t;
using discrete_device_sound_block_list_t = mame.std.vector<mame.discrete_block>;  //typedef std::vector<const discrete_block *> sound_block_list_t;
using discrete_device_task_list_t = mame.std.vector<mame.discrete_task>;  //typedef std::vector<std::unique_ptr<discrete_task> > task_list_t;
using discrete_sound_device_istream_node_list_t = mame.std.vector<mame.discrete_dss_input_stream_node>;  //typedef std::vector<discrete_dss_input_stream_node *> istream_node_list_t;
using discrete_sound_device_node_output_list_t = mame.std.vector<mame.discrete_sound_output_interface>;  //typedef std::vector<discrete_sound_output_interface *> node_output_list_t;
using int32_t = System.Int32;
using offs_t = System.UInt32;  //using offs_t = u32;
using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using size_t = System.UInt64;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.cpp_global;
using static mame.device_global;
using static mame.discrete_global;
using static mame.discrete_internal;
using static mame.eminline_global;
using static mame.emucore_global;
using static mame.osdcore_global;


namespace mame
{
    public static partial class discrete_global
    {
        /*************************************
         *
         *  macros
         *  see also: emu\machine\rescap.h
         *
         *************************************/

        /* calculate charge exponent using discrete sample time */
        public static double RC_CHARGE_EXP(discrete_base_node node, double rc) { return 1.0 - exp(-node.sample_time() / (rc)); }

        /* calculate charge exponent using given sample time */
        public static double RC_CHARGE_EXP_DT(double rc, double dt) { return 1.0 - exp(-dt / rc); }
        //define RC_CHARGE_NEG_EXP_DT(rc, dt)            (1.0 - exp((dt) / (rc)))

        /* calculate discharge exponent using discrete sample time */
        public static double RC_DISCHARGE_EXP(discrete_base_node node, double rc) { return exp(-node.sample_time() / rc); }
        /* calculate discharge exponent using given sample time */
        //define RC_DISCHARGE_EXP_DT(rc, dt)             (exp(-(dt) / (rc)))
        //define RC_DISCHARGE_NEG_EXP_DT(rc, dt)         (exp((dt) / (rc)))

        //define FREQ_OF_555(_r1, _r2, _c)   (1.49 / ((_r1 + 2 * _r2) * _c))


        /*************************************
         *
         *  Interface & Naming
         *
         *************************************/

        //#define DISCRETE_CLASS_FUNC(_class, _func)      DISCRETE_CLASS_NAME(_class) :: _func

        //#define DISCRETE_STEP(_class)                   void DISCRETE_CLASS_FUNC(_class, step)(void)
        //#define DISCRETE_RESET(_class)                  void DISCRETE_CLASS_FUNC(_class, reset)(void)
        //#define DISCRETE_START(_class)                  void DISCRETE_CLASS_FUNC(_class, start)(void)
        //#define DISCRETE_STOP(_class)                   void DISCRETE_CLASS_FUNC(_class, stop)(void)
        //#define DISCRETE_DECLARE_INFO(_name)            const _name *info = (const  _name *)this->custom_data();


        //#define DISCRETE_INPUT(_num)                  (*(this->m_input[_num]))
        public static double DISCRETE_INPUT(int num, Func<int, double> input) { return input(num); }  //#define DISCRETE_INPUT(_num)                    (input(_num))


        /*************************************
         *
         *  Core constants
         *
         *************************************/

        public const int DISCRETE_MAX_NODES = 299;  //300;
        public const int DISCRETE_MAX_INPUTS = 10;
        public const int DISCRETE_MAX_OUTPUTS = 8;

        public const int DISCRETE_MAX_TASK_GROUPS = 10;


        /*************************************
         *
         *  Node-specific constants
         *
         *************************************/

        public const double DEFAULT_TTL_V_LOGIC_1               = 3.4;

        //#define DISC_LOGADJ                         1.0
        public const double DISC_LINADJ                         = 0.0;

        /* DISCRETE_COMP_ADDER types */
        public const int DISC_COMP_P_CAPACITOR               = 0x00;
        public const int DISC_COMP_P_RESISTOR                = 0x01;

        /* clk types */
        public const int DISC_CLK_MASK                       = 0x03;
        public const int DISC_CLK_ON_F_EDGE                  = 0x00;
        public const int DISC_CLK_ON_R_EDGE                  = 0x01;
        public const int DISC_CLK_BY_COUNT                   = 0x02;
        public const int DISC_CLK_IS_FREQ                    = 0x03;

        //#define DISC_COUNT_DOWN                     0
        public const int DISC_COUNT_UP                       = 1;

        public const int DISC_COUNTER_IS_7492                = 0x08;

        public const int DISC_OUT_MASK                       = 0x30;
        //#define DISC_OUT_DEFAULT                    0x00
        public const int DISC_OUT_IS_ENERGY                  = 0x10;
        public const int DISC_OUT_HAS_XTIME                  = 0x20;

        /* Function possibilities for the LFSR feedback nodes */
        /* 2 inputs, one output                               */
        public const int DISC_LFSR_XOR                       = 0;
        public const int DISC_LFSR_OR                        = 1;
        public const int DISC_LFSR_AND                       = 2;
        public const int DISC_LFSR_XNOR                      = 3;
        public const int DISC_LFSR_NOR                       = 4;
        public const int DISC_LFSR_NAND                      = 5;
        public const int DISC_LFSR_IN0                       = 6;
        public const int DISC_LFSR_IN1                       = 7;
        public const int DISC_LFSR_NOT_IN0                   = 8;
        public const int DISC_LFSR_NOT_IN1                   = 9;
        public const int DISC_LFSR_REPLACE                   = 10;
        public const int DISC_LFSR_XOR_INV_IN0               = 11;
        public const int DISC_LFSR_XOR_INV_IN1               = 12;

        /* LFSR Flag Bits */
        public const int DISC_LFSR_FLAG_OUT_INVERT           = 0x01;
        //#define DISC_LFSR_FLAG_RESET_TYPE_L         0x00
        public const int DISC_LFSR_FLAG_RESET_TYPE_H         = 0x02;
        public const int DISC_LFSR_FLAG_OUTPUT_F0            = 0x04;
        public const int DISC_LFSR_FLAG_OUTPUT_SR_SN1        = 0x08;

        /* Sample & Hold supported clock types */
        public const int DISC_SAMPHOLD_REDGE                 = 0;
        public const int DISC_SAMPHOLD_FEDGE                 = 1;
        public const int DISC_SAMPHOLD_HLATCH                = 2;
        public const int DISC_SAMPHOLD_LLATCH                = 3;

        /* Shift options */
        //#define DISC_LOGIC_SHIFT__RESET_L           0x00
        //#define DISC_LOGIC_SHIFT__RESET_H           0x10
        //#define DISC_LOGIC_SHIFT__LEFT              0x00
        //#define DISC_LOGIC_SHIFT__RIGHT             0x20

        /* Maximum number of resistors in ladder chain */
        public const int DISC_LADDER_MAXRES = 8;

        /* Filter types */
        public const int DISC_FILTER_LOWPASS  = 0;
        public const int DISC_FILTER_HIGHPASS = 1;
        public const int DISC_FILTER_BANDPASS = 2;

        /* Mixer types */
        public const int DISC_MIXER_IS_RESISTOR       = 0;
        public const int DISC_MIXER_IS_OP_AMP         = 1;
        public const int DISC_MIXER_IS_OP_AMP_WITH_RI = 2;   /* Used only internally.  Use DISC_MIXER_IS_OP_AMP */


        /* Triggered Op Amp Functions */
        //enum
        //{
        public const int DISC_OP_AMP_TRIGGER_FUNCTION_NONE = 0;
        public const int DISC_OP_AMP_TRIGGER_FUNCTION_TRG0 = 1;
        public const int DISC_OP_AMP_TRIGGER_FUNCTION_TRG0_INV = 2;
        public const int DISC_OP_AMP_TRIGGER_FUNCTION_TRG1 = 3;
        public const int DISC_OP_AMP_TRIGGER_FUNCTION_TRG1_INV = 4;
        public const int DISC_OP_AMP_TRIGGER_FUNCTION_TRG2 = 5;
        public const int DISC_OP_AMP_TRIGGER_FUNCTION_TRG2_INV = 6;
        public const int DISC_OP_AMP_TRIGGER_FUNCTION_TRG01_AND = 7;
        public const int DISC_OP_AMP_TRIGGER_FUNCTION_TRG01_NAND = 8;
        //}


        /* Common Op Amp Flags and values */
        public const int DISC_OP_AMP_IS_NORTON = 0x100;
        public const double OP_AMP_NORTON_VBE = 0.5;     // This is the norton junction voltage. Used only internally.
        public const double OP_AMP_VP_RAIL_OFFSET = 1.5;     // This is how close an op-amp can get to the vP rail. Used only internally.

        /* Integrate options */
        //#define DISC_INTEGRATE_OP_AMP_1             0x00
        //#define DISC_INTEGRATE_OP_AMP_2             0x10

        /* op amp 1 shot types */
        public const int DISC_OP_AMP_1SHT_1                  = 0x00;

        /* Op Amp Filter Options */
        public const int DISC_OP_AMP_FILTER_IS_LOW_PASS_1   = 0x00;
        public const int DISC_OP_AMP_FILTER_IS_HIGH_PASS_1  = 0x10;
        public const int DISC_OP_AMP_FILTER_IS_BAND_PASS_1  = 0x20;
        public const int DISC_OP_AMP_FILTER_IS_BAND_PASS_1M = 0x30;
        public const int DISC_OP_AMP_FILTER_IS_HIGH_PASS_0  = 0x40;
        public const int DISC_OP_AMP_FILTER_IS_BAND_PASS_0  = 0x50;
        public const int DISC_OP_AMP_FILTER_IS_LOW_PASS_1_A = 0x60;

        public const int DISC_OP_AMP_FILTER_TYPE_MASK = 0xf0 | DISC_OP_AMP_IS_NORTON;  // Used only internally.

        /* Sallen-Key filter Opions */
        public const int DISC_SALLEN_KEY_LOW_PASS            = 0x01;
        //#define DISC_SALLEN_KEY_HIGH_PASS           0x02


        /* Op Amp Oscillator Flags */
        public const int DISC_OP_AMP_OSCILLATOR_TYPE_MASK    = 0xf0 | DISC_OP_AMP_IS_NORTON;  // Used only internally.
        public const int DISC_OP_AMP_OSCILLATOR_1            = 0x00;
        public const int DISC_OP_AMP_OSCILLATOR_2            = 0x10;
        public const int DISC_OP_AMP_OSCILLATOR_VCO_1        = 0x20;
        public const int DISC_OP_AMP_OSCILLATOR_VCO_2        = 0x30;
        public const int DISC_OP_AMP_OSCILLATOR_VCO_3        = 0x40;

        public const int DISC_OP_AMP_OSCILLATOR_OUT_MASK         = 0x07;
        public const int DISC_OP_AMP_OSCILLATOR_OUT_CAP          = 0x00;
        public const int DISC_OP_AMP_OSCILLATOR_OUT_SQW          = 0x01;
        public const int DISC_OP_AMP_OSCILLATOR_OUT_ENERGY       = 0x02;
        public const int DISC_OP_AMP_OSCILLATOR_OUT_LOGIC_X      = 0x03;
        public const int DISC_OP_AMP_OSCILLATOR_OUT_COUNT_F_X    = 0x04;
        public const int DISC_OP_AMP_OSCILLATOR_OUT_COUNT_R_X    = 0x05;

        /* Schmitt Oscillator Options */
        //#define DISC_SCHMITT_OSC_IN_IS_LOGIC        0x00
        //#define DISC_SCHMITT_OSC_IN_IS_VOLTAGE      0x01

        //#define DISC_SCHMITT_OSC_ENAB_IS_AND        0x00
        //#define DISC_SCHMITT_OSC_ENAB_IS_NAND       0x02
        //#define DISC_SCHMITT_OSC_ENAB_IS_OR         0x04
        //#define DISC_SCHMITT_OSC_ENAB_IS_NOR        0x06

        //#define DISC_SCHMITT_OSC_ENAB_MASK          0x06    /* Bits that define output enable type. */
                                                              /* Used only internally in module. */

        /* 555 Common output flags */
        public const int DISC_555_OUT_DC                     = 0x00;
        public const int DISC_555_OUT_AC                     = 0x10;

        //#define DISC_555_TRIGGER_IS_LOGIC           0x00
        //#define DISC_555_TRIGGER_IS_VOLTAGE         0x20
        //#define DISC_555_TRIGGER_IS_COUNT           0x40
        //#define DSD_555_TRIGGER_TYPE_MASK           0x60
        //#define DISC_555_TRIGGER_DISCHARGES_CAP     0x80

        public const int DISC_555_OUT_SQW                    = 0x00;    /* Squarewave */
        public const int DISC_555_OUT_CAP                    = 0x01;    /* Cap charge waveform */
        public const int DISC_555_OUT_COUNT_F                = 0x02;    /* Falling count */
        public const int DISC_555_OUT_COUNT_R                = 0x03;    /* Rising count */
        public const int DISC_555_OUT_ENERGY                 = 0x04;
        public const int DISC_555_OUT_LOGIC_X                = 0x05;
        public const int DISC_555_OUT_COUNT_F_X              = 0x06;
        public const int DISC_555_OUT_COUNT_R_X              = 0x07;

        public const int DISC_555_OUT_MASK                   = 0x07;    /* Bits that define output type. */
                                                                        /* Used only internally in module. */

        public const int DISC_555_ASTABLE_HAS_FAST_CHARGE_DIODE      = 0x80;
        //#define DISCRETE_555_CC_TO_DISCHARGE_PIN            0x00
        public const int DISCRETE_555_CC_TO_CAP                      = 0x80;

        /* 566 output flags */
        //#define DISC_566_OUT_DC                     0x00
        //#define DISC_566_OUT_AC                     0x10

        //#define DISC_566_OUT_SQUARE                 0x00    /* Squarewave */
        //#define DISC_566_OUT_ENERGY                 0x01    /* anti-alaised Squarewave */
        //#define DISC_566_OUT_TRIANGLE               0x02    /* Triangle waveform */
        //#define DISC_566_OUT_LOGIC                  0x03    /* 0/1 logic output */
        //#define DISC_566_OUT_COUNT_F                0x04
        //#define DISC_566_OUT_COUNT_R                0x05
        //#define DISC_566_OUT_COUNT_F_X              0x06
        //#define DISC_566_OUT_COUNT_R_X              0x07
        //#define DISC_566_OUT_MASK                   0x07    /* Bits that define output type. */
                                                              /* Used only internally in module. */

        /* LS624 output flags */
        //#define DISC_LS624_OUT_SQUARE               0x01
        //#define DISC_LS624_OUT_ENERGY               0x02
        //#define DISC_LS624_OUT_LOGIC                0x03
        //#define DISC_LS624_OUT_LOGIC_X              0x04
        //#define DISC_LS624_OUT_COUNT_F              0x05
        //#define DISC_LS624_OUT_COUNT_R              0x06
        //#define DISC_LS624_OUT_COUNT_F_X            0x07
        //#define DISC_LS624_OUT_COUNT_R_X            0x08

        /* Oneshot types */
        //#define DISC_ONESHOT_FEDGE                  0x00
        //#define DISC_ONESHOT_REDGE                  0x01

        //#define DISC_ONESHOT_NORETRIG               0x00
        //#define DISC_ONESHOT_RETRIG                 0x02

        //#define DISC_OUT_ACTIVE_LOW                 0x04
        //#define DISC_OUT_ACTIVE_HIGH                0x00

        //#define DISC_CD4066_THRESHOLD               2.75

        /* Integrate */

        public const int DISC_RC_INTEGRATE_TYPE1             = 0x00;
        public const int DISC_RC_INTEGRATE_TYPE2             = 0x01;
        public const int DISC_RC_INTEGRATE_TYPE3             = 0x02;
    }


    /*************************************
        *
        *  Classes and structs to handle
        *  linked lists.
        *
        *************************************/

    /*************************************
     *
     *  Node-specific struct types
     *
     *************************************/

    public class discrete_lfsr_desc
    {
        public int clock_type;
        public int bitlength;
        public int reset_value;

        public int feedback_bitsel0;
        public int feedback_bitsel1;
        public int feedback_function0;         /* Combines bitsel0 & bitsel1 */

        public int feedback_function1;         /* Combines funct0 & infeed bit */

        public int feedback_function2;         /* Combines funct1 & shifted register */
        public int feedback_function2_mask;    /* Which bits are affected by function 2 */

        public int flags;

        public int output_bit;

        public discrete_lfsr_desc(int clock_type, int bitlength, int reset_value, int feedback_bitsel0, int feedback_bitsel1, int feedback_function0, int feedback_function1, int feedback_function2, int feedback_function2_mask, int flags, int output_bit)
        { this.clock_type = clock_type; this.bitlength = bitlength; this.reset_value = reset_value; this.feedback_bitsel0 = feedback_bitsel0; this.feedback_bitsel1 = feedback_bitsel1; this.feedback_function0 = feedback_function0; this.feedback_function1 = feedback_function1; this.feedback_function2 = feedback_function2; this.feedback_function2_mask = feedback_function2_mask; this.flags = flags; this.output_bit = output_bit; }
    }


    public class discrete_op_amp_osc_info
    {
        public uint32_t type;
        //double r1;
        //double r2;
        //double r3;
        //double r4;
        //double r5;
        //double r6;
        //double r7;
        //double r8;
        public MemoryContainer<double> r = new MemoryContainer<double>(9, true);
        public double c;
        public double vP;     // Op amp B+

        public discrete_op_amp_osc_info(uint32_t type, double r1, double r2, double r3, double r4, double r5, double r6, double r7, double r8, double c, double vP)
        { this.type = type; this.r[1] = r1; this.r[2] = r2; this.r[3] = r3; this.r[4] = r4; this.r[5] = r5; this.r[6] = r6; this.r[7] = r7; this.r[8] = r8; this.c = c; this.vP = vP; }
    }


    public class discrete_comp_adder_table
    {
        public int type;
        public double cDefault;               // Default component.  0 if not used.
        public int length;
        public double [] c = new double [DISC_LADDER_MAXRES];  // Component table

        public discrete_comp_adder_table(int type, double cDefault, int length, double [] c)
        { this.type = type; this.cDefault = cDefault; this.length = length; this.c = c; }
    }


    public class discrete_dac_r1_ladder
    {
        public int ladderLength;       // 2 to DISC_LADDER_MAXRES.  1 would be useless.
        public double [] r = new double[DISC_LADDER_MAXRES];  // Don't use 0 for valid resistors.  That is a short.
        public double vBias;          // Voltage Bias resistor is tied to (0 = not used)
        public double rBias;          // Additional resistor tied to vBias (0 = not used)
        public double rGnd;           // Resistor tied to ground (0 = not used)
        public double cFilter;        // Filtering cap (0 = not used)

        public discrete_dac_r1_ladder(int ladderLength, double [] r, double vBias, double rBias, double rGnd, double cFilter)
        { this.ladderLength = ladderLength; Array.Copy(r, this.r, r.Length); this.vBias = vBias; this.rBias = rBias; this.rGnd = rGnd; this.cFilter = cFilter; }
    }


    //struct discrete_integrate_info


    public class discrete_mixer_desc
    {
        const int DISC_MAX_MIXER_INPUTS = 8;

        public int type;
        public double [] r = new double[DISC_MAX_MIXER_INPUTS];       /* static input resistance values.  These are in series with rNode, if used. */
        public int [] r_node = new int[DISC_MAX_MIXER_INPUTS];  /* variable resistance nodes, if needed.  0 if not used. */
        public double [] c = new double[DISC_MAX_MIXER_INPUTS];
        public double rI;
        public double rF;
        public double cF;
        public double cAmp;
        public double vRef;
        public double gain;               /* Scale value to get output close to +/- 32767 */

        public discrete_mixer_desc(int type, double [] r, int [] r_node, double [] c, double rI, double rF, double cF, double cAmp, double vRef, double gain)
        { this.type = type; Array.Copy(r, this.r, r.Length); Array.Copy(r_node, this.r_node, r_node.Length); Array.Copy(c, this.c, c.Length); this.rI = rI; this.rF = rF; this.cF = cF; this.cAmp = cAmp; this.vRef = vRef; this.gain = gain; }
    }


    public class discrete_op_amp_info
    {
        public uint32_t type;
        public double r1;
        public double r2;
        public double r3;
        public double r4;
        public double c;
        public double vN;     // Op amp B-
        public double vP;     // Op amp B+

        public discrete_op_amp_info(uint32_t type, double r1, double r2, double r3, double r4, double c, double vN, double vP)
        { this.type = type; this.r1 = r1; this.r2 = r2; this.r3 = r3; this.r4 = r4; this.c = c; this.vN = vN; this.vP = vP; }
    }


    public class discrete_op_amp_1sht_info
    {
        uint32_t type;
        public double r1;
        public double r2;
        public double r3;
        public double r4;
        public double r5;
        public double c1;
        public double c2;
        public double vN;     // Op amp B-
        public double vP;     // Op amp B+

        public discrete_op_amp_1sht_info(uint32_t type, double r1, double r2, double r3, double r4, double r5, double c1, double c2, double vN, double vP)
        { this.type = type; this.r1 = r1; this.r2 = r2; this.r3 = r3; this.r4 = r4; this.r5 = r5; this.c1 = c1; this.c2 = c2; this.vN = vN; this.vP = vP; }
    }


    public class discrete_op_amp_tvca_info
    {
        public double r1;
        public double r2;     // r2a + r2b
        public double r3;     // r3a + r3b
        public double r4;
        public double r5;
        public double r6;
        public double r7;
        public double r8;
        public double r9;
        public double r10;
        public double r11;
        public double c1;
        public double c2;
        public double c3;
        public double c4;
        public double v1;
        public double v2;
        public double v3;
        public double vP;
        public int f0;
        public int f1;
        public int f2;
        public int f3;
        public int f4;
        public int f5;

        public discrete_op_amp_tvca_info(double r1, double r2, double r3, double r4, double r5, double r6, double r7, double r8, double r9, double r10, double r11, double c1, double c2, double c3, double c4, 
            double v1, double v2, double v3, double vP, int f0, int f1, int f2, int f3, int f4, int f5)
        { this.r1 = r1; this.r2 = r2; this.r3 = r3; this.r4 = r4; this.r5 = r5; this.r6 = r6; this.r7 = r7; this.r8 = r8; this.r9 = r9; this.r10 = r10; this.r11 = r11; this.c1 = c1; this.c2 = c2; this.c3 = c3; this.c4 = c4;
            this.v1 = v1; this.v2 = v2; this.v3 = v3; this.vP = vP; this.f0 = f0; this.f1 = f1; this.f2 = f2; this.f3 = f3; this.f4 = f4; this.f5 = f5; }
    }


    public class discrete_op_amp_filt_info
    {
        public double r1;
        public double r2;
        public double r3;
        public double r4;
        public double rF;
        public double c1;
        public double c2;
        public double c3;
        public double vRef;
        public double vP;
        public double vN;

        public discrete_op_amp_filt_info(double r1, double r2, double r3, double r4, double rF, double c1, double c2, double c3, double vRef = 0, double vP = 0, double vN = 0)
        { this.r1 = r1; this.r2 = r2; this.r3 = r3; this.r4 = r4; this.rF = rF; this.c1 = c1; this.c2 = c2; this.c3 = c3; this.vRef = vRef; this.vP = vP; this.vN = vN;  }
    }


    public static partial class discrete_global
    {
        public const int DEFAULT_555_CHARGE      = -1;
        public const int DEFAULT_555_HIGH        = -1;
        //#define DEFAULT_555_VALUES      DEFAULT_555_CHARGE, DEFAULT_555_HIGH
        public const int DEFAULT_555_VALUES_1      = DEFAULT_555_CHARGE;
        public const int DEFAULT_555_VALUES_2      = DEFAULT_555_HIGH;
    }


    public class discrete_555_desc
    {
        public int     options;    /* bit mapped options */
        public double  v_pos;      /* B+ voltage of 555 */
        public double  v_charge;   /* voltage to charge circuit  (Defaults to v_pos) */
        public double  v_out_high; /* High output voltage of 555 (Defaults to v_pos - 1.2V) */

        public discrete_555_desc(int options, double v_pos, double v_charge, double v_out_high)
        { this.options = options; this.v_pos = v_pos; this.v_charge = v_charge; this.v_out_high = v_out_high; }
    }


    public static partial class discrete_global
    {
        public const int DEFAULT_555_CC_SOURCE   = DEFAULT_555_CHARGE;
    }


    public class discrete_555_cc_desc
    {
        public int     options;        /* bit mapped options */
        public double  v_pos;          /* B+ voltage of 555 */
        public double  v_cc_source;    /* Voltage of the Constant Current source */
        public double  v_out_high;     /* High output voltage of 555 (Defaults to v_pos - 1.2V) */
        public double  v_cc_junction;  /* The voltage drop of the Constant Current source transistor (0 if Op Amp) */

        public discrete_555_cc_desc(int options, double v_pos, double v_cc_source, double v_out_high, double v_cc_junction)
        { this.options = options; this.v_pos = v_pos; this.v_cc_source = v_cc_source; this.v_out_high = v_out_high; this.v_cc_junction = v_cc_junction; }
    }

#if false
    struct discrete_555_vco1_desc
    {
        int    options;             /* bit mapped options */
        double r1, r2, r3, r4, c;
        double v_pos;               /* B+ voltage of 555 */
        double v_charge;            /* (ignored) */
        double v_out_high;          /* High output voltage of 555 (Defaults to v_pos - 1.2V) */
    }


    struct discrete_adsr
    {
        double attack_time;  /* All times are in seconds */
        double attack_value;
        double decay_time;
        double decay_value;
        double sustain_time;
        double sustain_value;
        double release_time;
        double release_value;
    }
#endif


    public static partial class discrete_global
    {
        /*************************************
         *
         *  The node numbers themselves
         *
         *************************************/

        //define NODE0_DEF(_x) NODE_ ## 0 ## _x = (0x40000000 + (_x) * DISCRETE_MAX_OUTPUTS), \
        //    NODE_ ## 0 ## _x ## _00 = NODE_ ## 0 ## _x, NODE_ ## 0 ## _x ## _01, NODE_ ## 0 ## _x ## _02, NODE_ ## 0 ## _x ## _03, \
        //    NODE_ ## 0 ## _x ## _04, NODE_ ## 0 ## _x ## _05, NODE_ ## 0 ## _x ## _06, NODE_ ## 0 ## _x ## _07
        //define NODE_DEF(_x) NODE_ ## _x = (0x40000000 + (_x) * DISCRETE_MAX_OUTPUTS), \
        //    NODE_ ## _x ## _00 = NODE_ ## _x, NODE_ ## _x ## _01, NODE_ ## _x ## _02, NODE_ ## _x ## _03, \
        //    NODE_ ## _x ## _04, NODE_ ## _x ## _05, NODE_ ## _x ## _06, NODE_ ## _x ## _07

        //enum
        //{
        //NODE0_DEF(_x)
        //NODE_ ## 0 ## _x = (0x40000000 + (_x) * DISCRETE_MAX_OUTPUTS), \
        //NODE_ ## 0 ## _x ## _00 = NODE_ ## 0 ## _x,
        //NODE_ ## 0 ## _x ## _01, 
        //NODE_ ## 0 ## _x ## _02, 
        //NODE_ ## 0 ## _x ## _03, \
        //NODE_ ## 0 ## _x ## _04, 
        //NODE_ ## 0 ## _x ## _05, 
        //NODE_ ## 0 ## _x ## _06, 
        //NODE_ ## 0 ## _x ## _07

        public const int NODE_00 = 0x40000000 + 0 * DISCRETE_MAX_OUTPUTS; const int NODE_00_00 = NODE_00; const int NODE_00_01 = NODE_00 + 1; const int NODE_00_02 = NODE_00 + 2; const int NODE_00_03 = NODE_00 + 3; const int NODE_00_04 = NODE_00 + 4; const int NODE_00_05 = NODE_00 + 5; const int NODE_00_06 = NODE_00 + 6; const int NODE_00_07 = NODE_00 + 7;
        public const int NODE_01 = 0x40000000 + 1 * DISCRETE_MAX_OUTPUTS; const int NODE_01_00 = NODE_01; const int NODE_01_01 = NODE_01 + 1; const int NODE_01_02 = NODE_01 + 2; const int NODE_01_03 = NODE_01 + 3; const int NODE_01_04 = NODE_01 + 4; const int NODE_01_05 = NODE_01 + 5; const int NODE_01_06 = NODE_01 + 6; const int NODE_01_07 = NODE_01 + 7;
        public const int NODE_02 = 0x40000000 + 2 * DISCRETE_MAX_OUTPUTS; const int NODE_02_00 = NODE_02; const int NODE_02_01 = NODE_02 + 1; const int NODE_02_02 = NODE_02 + 2; const int NODE_02_03 = NODE_02 + 3; const int NODE_02_04 = NODE_02 + 4; const int NODE_02_05 = NODE_02 + 5; const int NODE_02_06 = NODE_02 + 6; const int NODE_02_07 = NODE_02 + 7;
        public const int NODE_03 = 0x40000000 + 3 * DISCRETE_MAX_OUTPUTS; const int NODE_03_00 = NODE_03; const int NODE_03_01 = NODE_03 + 1; const int NODE_03_02 = NODE_03 + 2; const int NODE_03_03 = NODE_03 + 3; const int NODE_03_04 = NODE_03 + 4; const int NODE_03_05 = NODE_03 + 5; const int NODE_03_06 = NODE_03 + 6; const int NODE_03_07 = NODE_03 + 7;
        public const int NODE_04 = 0x40000000 + 4 * DISCRETE_MAX_OUTPUTS; const int NODE_04_00 = NODE_04; const int NODE_04_01 = NODE_04 + 1; const int NODE_04_02 = NODE_04 + 2; const int NODE_04_03 = NODE_04 + 3; const int NODE_04_04 = NODE_04 + 4; const int NODE_04_05 = NODE_04 + 5; const int NODE_04_06 = NODE_04 + 6; const int NODE_04_07 = NODE_04 + 7;
        public const int NODE_05 = 0x40000000 + 5 * DISCRETE_MAX_OUTPUTS; const int NODE_05_00 = NODE_05; const int NODE_05_01 = NODE_05 + 1; const int NODE_05_02 = NODE_05 + 2; const int NODE_05_03 = NODE_05 + 3; const int NODE_05_04 = NODE_05 + 4; const int NODE_05_05 = NODE_05 + 5; const int NODE_05_06 = NODE_05 + 6; const int NODE_05_07 = NODE_05 + 7;
        public const int NODE_06 = 0x40000000 + 6 * DISCRETE_MAX_OUTPUTS; const int NODE_06_00 = NODE_06; const int NODE_06_01 = NODE_06 + 1; const int NODE_06_02 = NODE_06 + 2; const int NODE_06_03 = NODE_06 + 3; const int NODE_06_04 = NODE_06 + 4; const int NODE_06_05 = NODE_06 + 5; const int NODE_06_06 = NODE_06 + 6; const int NODE_06_07 = NODE_06 + 7;
        public const int NODE_07 = 0x40000000 + 7 * DISCRETE_MAX_OUTPUTS; const int NODE_07_00 = NODE_07; const int NODE_07_01 = NODE_07 + 1; const int NODE_07_02 = NODE_07 + 2; const int NODE_07_03 = NODE_07 + 3; const int NODE_07_04 = NODE_07 + 4; const int NODE_07_05 = NODE_07 + 5; const int NODE_07_06 = NODE_07 + 6; const int NODE_07_07 = NODE_07 + 7;
        public const int NODE_08 = 0x40000000 + 8 * DISCRETE_MAX_OUTPUTS; const int NODE_08_00 = NODE_08; const int NODE_08_01 = NODE_08 + 1; const int NODE_08_02 = NODE_08 + 2; const int NODE_08_03 = NODE_08 + 3; const int NODE_08_04 = NODE_08 + 4; const int NODE_08_05 = NODE_08 + 5; const int NODE_08_06 = NODE_08 + 6; const int NODE_08_07 = NODE_08 + 7;
        public const int NODE_09 = 0x40000000 + 9 * DISCRETE_MAX_OUTPUTS; const int NODE_09_00 = NODE_09; const int NODE_09_01 = NODE_09 + 1; const int NODE_09_02 = NODE_09 + 2; const int NODE_09_03 = NODE_09 + 3; const int NODE_09_04 = NODE_09 + 4; const int NODE_09_05 = NODE_09 + 5; const int NODE_09_06 = NODE_09 + 6; const int NODE_09_07 = NODE_09 + 7;

        public const int NODE_10 = 0x40000000 + 10 * DISCRETE_MAX_OUTPUTS; const int NODE_10_00 = NODE_10; const int NODE_10_01 = NODE_10 + 1; const int NODE_10_02 = NODE_10 + 2; const int NODE_10_03 = NODE_10 + 3; const int NODE_10_04 = NODE_10 + 4; const int NODE_10_05 = NODE_10 + 5; const int NODE_10_06 = NODE_10 + 6; const int NODE_10_07 = NODE_10 + 7;
        public const int NODE_11 = 0x40000000 + 11 * DISCRETE_MAX_OUTPUTS; const int NODE_11_00 = NODE_11; const int NODE_11_01 = NODE_11 + 1; const int NODE_11_02 = NODE_11 + 2; const int NODE_11_03 = NODE_11 + 3; const int NODE_11_04 = NODE_11 + 4; const int NODE_11_05 = NODE_11 + 5; const int NODE_11_06 = NODE_11 + 6; const int NODE_11_07 = NODE_11 + 7;
        public const int NODE_12 = 0x40000000 + 12 * DISCRETE_MAX_OUTPUTS; const int NODE_12_00 = NODE_12; const int NODE_12_01 = NODE_12 + 1; const int NODE_12_02 = NODE_12 + 2; const int NODE_12_03 = NODE_12 + 3; const int NODE_12_04 = NODE_12 + 4; const int NODE_12_05 = NODE_12 + 5; const int NODE_12_06 = NODE_12 + 6; const int NODE_12_07 = NODE_12 + 7;
        public const int NODE_13 = 0x40000000 + 13 * DISCRETE_MAX_OUTPUTS; const int NODE_13_00 = NODE_13; const int NODE_13_01 = NODE_13 + 1; const int NODE_13_02 = NODE_13 + 2; const int NODE_13_03 = NODE_13 + 3; const int NODE_13_04 = NODE_13 + 4; const int NODE_13_05 = NODE_13 + 5; const int NODE_13_06 = NODE_13 + 6; const int NODE_13_07 = NODE_13 + 7;
        public const int NODE_14 = 0x40000000 + 14 * DISCRETE_MAX_OUTPUTS; const int NODE_14_00 = NODE_14; const int NODE_14_01 = NODE_14 + 1; const int NODE_14_02 = NODE_14 + 2; const int NODE_14_03 = NODE_14 + 3; const int NODE_14_04 = NODE_14 + 4; const int NODE_14_05 = NODE_14 + 5; const int NODE_14_06 = NODE_14 + 6; const int NODE_14_07 = NODE_14 + 7;
        public const int NODE_15 = 0x40000000 + 15 * DISCRETE_MAX_OUTPUTS; const int NODE_15_00 = NODE_15; const int NODE_15_01 = NODE_15 + 1; const int NODE_15_02 = NODE_15 + 2; const int NODE_15_03 = NODE_15 + 3; const int NODE_15_04 = NODE_15 + 4; const int NODE_15_05 = NODE_15 + 5; const int NODE_15_06 = NODE_15 + 6; const int NODE_15_07 = NODE_15 + 7;
        public const int NODE_16 = 0x40000000 + 16 * DISCRETE_MAX_OUTPUTS; const int NODE_16_00 = NODE_16; const int NODE_16_01 = NODE_16 + 1; const int NODE_16_02 = NODE_16 + 2; const int NODE_16_03 = NODE_16 + 3; const int NODE_16_04 = NODE_16 + 4; const int NODE_16_05 = NODE_16 + 5; const int NODE_16_06 = NODE_16 + 6; const int NODE_16_07 = NODE_16 + 7;
        public const int NODE_17 = 0x40000000 + 17 * DISCRETE_MAX_OUTPUTS; const int NODE_17_00 = NODE_17; const int NODE_17_01 = NODE_17 + 1; const int NODE_17_02 = NODE_17 + 2; const int NODE_17_03 = NODE_17 + 3; const int NODE_17_04 = NODE_17 + 4; const int NODE_17_05 = NODE_17 + 5; const int NODE_17_06 = NODE_17 + 6; const int NODE_17_07 = NODE_17 + 7;
        const int NODE_18 = 0x40000000 + 18 * DISCRETE_MAX_OUTPUTS; const int NODE_18_00 = NODE_18; const int NODE_18_01 = NODE_18 + 1; const int NODE_18_02 = NODE_18 + 2; const int NODE_18_03 = NODE_18 + 3; const int NODE_18_04 = NODE_18 + 4; const int NODE_18_05 = NODE_18 + 5; const int NODE_18_06 = NODE_18 + 6; const int NODE_18_07 = NODE_18 + 7;
        const int NODE_19 = 0x40000000 + 19 * DISCRETE_MAX_OUTPUTS; const int NODE_19_00 = NODE_19; const int NODE_19_01 = NODE_19 + 1; const int NODE_19_02 = NODE_19 + 2; const int NODE_19_03 = NODE_19 + 3; const int NODE_19_04 = NODE_19 + 4; const int NODE_19_05 = NODE_19 + 5; const int NODE_19_06 = NODE_19 + 6; const int NODE_19_07 = NODE_19 + 7;

        public const int NODE_20 = 0x40000000 + 20 * DISCRETE_MAX_OUTPUTS; const int NODE_20_00 = NODE_20; const int NODE_20_01 = NODE_20 + 1; const int NODE_20_02 = NODE_20 + 2; const int NODE_20_03 = NODE_20 + 3; const int NODE_20_04 = NODE_20 + 4; const int NODE_20_05 = NODE_20 + 5; const int NODE_20_06 = NODE_20 + 6; const int NODE_20_07 = NODE_20 + 7;
        public const int NODE_21 = 0x40000000 + 21 * DISCRETE_MAX_OUTPUTS; const int NODE_21_00 = NODE_21; const int NODE_21_01 = NODE_21 + 1; const int NODE_21_02 = NODE_21 + 2; const int NODE_21_03 = NODE_21 + 3; const int NODE_21_04 = NODE_21 + 4; const int NODE_21_05 = NODE_21 + 5; const int NODE_21_06 = NODE_21 + 6; const int NODE_21_07 = NODE_21 + 7;
        public const int NODE_22 = 0x40000000 + 22 * DISCRETE_MAX_OUTPUTS; const int NODE_22_00 = NODE_22; const int NODE_22_01 = NODE_22 + 1; const int NODE_22_02 = NODE_22 + 2; const int NODE_22_03 = NODE_22 + 3; const int NODE_22_04 = NODE_22 + 4; const int NODE_22_05 = NODE_22 + 5; const int NODE_22_06 = NODE_22 + 6; const int NODE_22_07 = NODE_22 + 7;
        public const int NODE_23 = 0x40000000 + 23 * DISCRETE_MAX_OUTPUTS; const int NODE_23_00 = NODE_23; const int NODE_23_01 = NODE_23 + 1; const int NODE_23_02 = NODE_23 + 2; const int NODE_23_03 = NODE_23 + 3; const int NODE_23_04 = NODE_23 + 4; const int NODE_23_05 = NODE_23 + 5; const int NODE_23_06 = NODE_23 + 6; const int NODE_23_07 = NODE_23 + 7;
        public const int NODE_24 = 0x40000000 + 24 * DISCRETE_MAX_OUTPUTS; const int NODE_24_00 = NODE_24; const int NODE_24_01 = NODE_24 + 1; const int NODE_24_02 = NODE_24 + 2; const int NODE_24_03 = NODE_24 + 3; const int NODE_24_04 = NODE_24 + 4; const int NODE_24_05 = NODE_24 + 5; const int NODE_24_06 = NODE_24 + 6; const int NODE_24_07 = NODE_24 + 7;
        public const int NODE_25 = 0x40000000 + 25 * DISCRETE_MAX_OUTPUTS; const int NODE_25_00 = NODE_25; const int NODE_25_01 = NODE_25 + 1; const int NODE_25_02 = NODE_25 + 2; const int NODE_25_03 = NODE_25 + 3; const int NODE_25_04 = NODE_25 + 4; const int NODE_25_05 = NODE_25 + 5; const int NODE_25_06 = NODE_25 + 6; const int NODE_25_07 = NODE_25 + 7;
        public const int NODE_26 = 0x40000000 + 26 * DISCRETE_MAX_OUTPUTS; const int NODE_26_00 = NODE_26; const int NODE_26_01 = NODE_26 + 1; const int NODE_26_02 = NODE_26 + 2; const int NODE_26_03 = NODE_26 + 3; const int NODE_26_04 = NODE_26 + 4; const int NODE_26_05 = NODE_26 + 5; const int NODE_26_06 = NODE_26 + 6; const int NODE_26_07 = NODE_26 + 7;
        public const int NODE_27 = 0x40000000 + 27 * DISCRETE_MAX_OUTPUTS; const int NODE_27_00 = NODE_27; const int NODE_27_01 = NODE_27 + 1; const int NODE_27_02 = NODE_27 + 2; const int NODE_27_03 = NODE_27 + 3; const int NODE_27_04 = NODE_27 + 4; const int NODE_27_05 = NODE_27 + 5; const int NODE_27_06 = NODE_27 + 6; const int NODE_27_07 = NODE_27 + 7;
        public const int NODE_28 = 0x40000000 + 28 * DISCRETE_MAX_OUTPUTS; const int NODE_28_00 = NODE_28; const int NODE_28_01 = NODE_28 + 1; const int NODE_28_02 = NODE_28 + 2; const int NODE_28_03 = NODE_28 + 3; const int NODE_28_04 = NODE_28 + 4; const int NODE_28_05 = NODE_28 + 5; const int NODE_28_06 = NODE_28 + 6; const int NODE_28_07 = NODE_28 + 7;
        public const int NODE_29 = 0x40000000 + 29 * DISCRETE_MAX_OUTPUTS; const int NODE_29_00 = NODE_29; const int NODE_29_01 = NODE_29 + 1; const int NODE_29_02 = NODE_29 + 2; const int NODE_29_03 = NODE_29 + 3; const int NODE_29_04 = NODE_29 + 4; const int NODE_29_05 = NODE_29 + 5; const int NODE_29_06 = NODE_29 + 6; const int NODE_29_07 = NODE_29 + 7;

        public const int NODE_30 = 0x40000000 + 30 * DISCRETE_MAX_OUTPUTS; const int NODE_30_00 = NODE_30; const int NODE_30_01 = NODE_30 + 1; const int NODE_30_02 = NODE_30 + 2; const int NODE_30_03 = NODE_30 + 3; const int NODE_30_04 = NODE_30 + 4; const int NODE_30_05 = NODE_30 + 5; const int NODE_30_06 = NODE_30 + 6; const int NODE_30_07 = NODE_30 + 7;
        public const int NODE_31 = 0x40000000 + 31 * DISCRETE_MAX_OUTPUTS; const int NODE_31_00 = NODE_31; const int NODE_31_01 = NODE_31 + 1; const int NODE_31_02 = NODE_31 + 2; const int NODE_31_03 = NODE_31 + 3; const int NODE_31_04 = NODE_31 + 4; const int NODE_31_05 = NODE_31 + 5; const int NODE_31_06 = NODE_31 + 6; const int NODE_31_07 = NODE_31 + 7;
        public const int NODE_32 = 0x40000000 + 32 * DISCRETE_MAX_OUTPUTS; const int NODE_32_00 = NODE_32; const int NODE_32_01 = NODE_32 + 1; const int NODE_32_02 = NODE_32 + 2; const int NODE_32_03 = NODE_32 + 3; const int NODE_32_04 = NODE_32 + 4; const int NODE_32_05 = NODE_32 + 5; const int NODE_32_06 = NODE_32 + 6; const int NODE_32_07 = NODE_32 + 7;
        public const int NODE_33 = 0x40000000 + 33 * DISCRETE_MAX_OUTPUTS; const int NODE_33_00 = NODE_33; const int NODE_33_01 = NODE_33 + 1; const int NODE_33_02 = NODE_33 + 2; const int NODE_33_03 = NODE_33 + 3; const int NODE_33_04 = NODE_33 + 4; const int NODE_33_05 = NODE_33 + 5; const int NODE_33_06 = NODE_33 + 6; const int NODE_33_07 = NODE_33 + 7;
        public const int NODE_34 = 0x40000000 + 34 * DISCRETE_MAX_OUTPUTS; const int NODE_34_00 = NODE_34; const int NODE_34_01 = NODE_34 + 1; const int NODE_34_02 = NODE_34 + 2; const int NODE_34_03 = NODE_34 + 3; const int NODE_34_04 = NODE_34 + 4; const int NODE_34_05 = NODE_34 + 5; const int NODE_34_06 = NODE_34 + 6; const int NODE_34_07 = NODE_34 + 7;
        public const int NODE_35 = 0x40000000 + 35 * DISCRETE_MAX_OUTPUTS; const int NODE_35_00 = NODE_35; const int NODE_35_01 = NODE_35 + 1; const int NODE_35_02 = NODE_35 + 2; const int NODE_35_03 = NODE_35 + 3; const int NODE_35_04 = NODE_35 + 4; const int NODE_35_05 = NODE_35 + 5; const int NODE_35_06 = NODE_35 + 6; const int NODE_35_07 = NODE_35 + 7;
        const int NODE_36 = 0x40000000 + 36 * DISCRETE_MAX_OUTPUTS; const int NODE_36_00 = NODE_36; const int NODE_36_01 = NODE_36 + 1; const int NODE_36_02 = NODE_36 + 2; const int NODE_36_03 = NODE_36 + 3; const int NODE_36_04 = NODE_36 + 4; const int NODE_36_05 = NODE_36 + 5; const int NODE_36_06 = NODE_36 + 6; const int NODE_36_07 = NODE_36 + 7;
        const int NODE_37 = 0x40000000 + 37 * DISCRETE_MAX_OUTPUTS; const int NODE_37_00 = NODE_37; const int NODE_37_01 = NODE_37 + 1; const int NODE_37_02 = NODE_37 + 2; const int NODE_37_03 = NODE_37 + 3; const int NODE_37_04 = NODE_37 + 4; const int NODE_37_05 = NODE_37 + 5; const int NODE_37_06 = NODE_37 + 6; const int NODE_37_07 = NODE_37 + 7;
        public const int NODE_38 = 0x40000000 + 38 * DISCRETE_MAX_OUTPUTS; const int NODE_38_00 = NODE_38; const int NODE_38_01 = NODE_38 + 1; const int NODE_38_02 = NODE_38 + 2; const int NODE_38_03 = NODE_38 + 3; const int NODE_38_04 = NODE_38 + 4; const int NODE_38_05 = NODE_38 + 5; const int NODE_38_06 = NODE_38 + 6; const int NODE_38_07 = NODE_38 + 7;
        public const int NODE_39 = 0x40000000 + 39 * DISCRETE_MAX_OUTPUTS; const int NODE_39_00 = NODE_39; const int NODE_39_01 = NODE_39 + 1; const int NODE_39_02 = NODE_39 + 2; const int NODE_39_03 = NODE_39 + 3; const int NODE_39_04 = NODE_39 + 4; const int NODE_39_05 = NODE_39 + 5; const int NODE_39_06 = NODE_39 + 6; const int NODE_39_07 = NODE_39 + 7;

        public const int NODE_40 = 0x40000000 + 40 * DISCRETE_MAX_OUTPUTS; const int NODE_40_00 = NODE_40; const int NODE_40_01 = NODE_40 + 1; const int NODE_40_02 = NODE_40 + 2; const int NODE_40_03 = NODE_40 + 3; const int NODE_40_04 = NODE_40 + 4; const int NODE_40_05 = NODE_40 + 5; const int NODE_40_06 = NODE_40 + 6; const int NODE_40_07 = NODE_40 + 7;
        public const int NODE_41 = 0x40000000 + 41 * DISCRETE_MAX_OUTPUTS; const int NODE_41_00 = NODE_41; const int NODE_41_01 = NODE_41 + 1; const int NODE_41_02 = NODE_41 + 2; const int NODE_41_03 = NODE_41 + 3; const int NODE_41_04 = NODE_41 + 4; const int NODE_41_05 = NODE_41 + 5; const int NODE_41_06 = NODE_41 + 6; const int NODE_41_07 = NODE_41 + 7;
        public const int NODE_42 = 0x40000000 + 42 * DISCRETE_MAX_OUTPUTS; const int NODE_42_00 = NODE_42; const int NODE_42_01 = NODE_42 + 1; const int NODE_42_02 = NODE_42 + 2; const int NODE_42_03 = NODE_42 + 3; const int NODE_42_04 = NODE_42 + 4; const int NODE_42_05 = NODE_42 + 5; const int NODE_42_06 = NODE_42 + 6; const int NODE_42_07 = NODE_42 + 7;
        public const int NODE_43 = 0x40000000 + 43 * DISCRETE_MAX_OUTPUTS; const int NODE_43_00 = NODE_43; const int NODE_43_01 = NODE_43 + 1; const int NODE_43_02 = NODE_43 + 2; const int NODE_43_03 = NODE_43 + 3; const int NODE_43_04 = NODE_43 + 4; const int NODE_43_05 = NODE_43 + 5; const int NODE_43_06 = NODE_43 + 6; const int NODE_43_07 = NODE_43 + 7;
        const int NODE_44 = 0x40000000 + 44 * DISCRETE_MAX_OUTPUTS; const int NODE_44_00 = NODE_44; const int NODE_44_01 = NODE_44 + 1; const int NODE_44_02 = NODE_44 + 2; const int NODE_44_03 = NODE_44 + 3; const int NODE_44_04 = NODE_44 + 4; const int NODE_44_05 = NODE_44 + 5; const int NODE_44_06 = NODE_44 + 6; const int NODE_44_07 = NODE_44 + 7;
        public const int NODE_45 = 0x40000000 + 45 * DISCRETE_MAX_OUTPUTS; const int NODE_45_00 = NODE_45; const int NODE_45_01 = NODE_45 + 1; const int NODE_45_02 = NODE_45 + 2; const int NODE_45_03 = NODE_45 + 3; const int NODE_45_04 = NODE_45 + 4; const int NODE_45_05 = NODE_45 + 5; const int NODE_45_06 = NODE_45 + 6; const int NODE_45_07 = NODE_45 + 7;
        const int NODE_46 = 0x40000000 + 46 * DISCRETE_MAX_OUTPUTS; const int NODE_46_00 = NODE_46; const int NODE_46_01 = NODE_46 + 1; const int NODE_46_02 = NODE_46 + 2; const int NODE_46_03 = NODE_46 + 3; const int NODE_46_04 = NODE_46 + 4; const int NODE_46_05 = NODE_46 + 5; const int NODE_46_06 = NODE_46 + 6; const int NODE_46_07 = NODE_46 + 7;
        const int NODE_47 = 0x40000000 + 47 * DISCRETE_MAX_OUTPUTS; const int NODE_47_00 = NODE_47; const int NODE_47_01 = NODE_47 + 1; const int NODE_47_02 = NODE_47 + 2; const int NODE_47_03 = NODE_47 + 3; const int NODE_47_04 = NODE_47 + 4; const int NODE_47_05 = NODE_47 + 5; const int NODE_47_06 = NODE_47 + 6; const int NODE_47_07 = NODE_47 + 7;
        const int NODE_48 = 0x40000000 + 48 * DISCRETE_MAX_OUTPUTS; const int NODE_48_00 = NODE_48; const int NODE_48_01 = NODE_48 + 1; const int NODE_48_02 = NODE_48 + 2; const int NODE_48_03 = NODE_48 + 3; const int NODE_48_04 = NODE_48 + 4; const int NODE_48_05 = NODE_48 + 5; const int NODE_48_06 = NODE_48 + 6; const int NODE_48_07 = NODE_48 + 7;
        const int NODE_49 = 0x40000000 + 49 * DISCRETE_MAX_OUTPUTS; const int NODE_49_00 = NODE_49; const int NODE_49_01 = NODE_49 + 1; const int NODE_49_02 = NODE_49 + 2; const int NODE_49_03 = NODE_49 + 3; const int NODE_49_04 = NODE_49 + 4; const int NODE_49_05 = NODE_49 + 5; const int NODE_49_06 = NODE_49 + 6; const int NODE_49_07 = NODE_49 + 7;

        public const int NODE_50 = 0x40000000 + 50 * DISCRETE_MAX_OUTPUTS; const int NODE_50_00 = NODE_50; const int NODE_50_01 = NODE_50 + 1; const int NODE_50_02 = NODE_50 + 2; const int NODE_50_03 = NODE_50 + 3; const int NODE_50_04 = NODE_50 + 4; const int NODE_50_05 = NODE_50 + 5; const int NODE_50_06 = NODE_50 + 6; const int NODE_50_07 = NODE_50 + 7;
        public const int NODE_51 = 0x40000000 + 51 * DISCRETE_MAX_OUTPUTS; const int NODE_51_00 = NODE_51; const int NODE_51_01 = NODE_51 + 1; const int NODE_51_02 = NODE_51 + 2; const int NODE_51_03 = NODE_51 + 3; const int NODE_51_04 = NODE_51 + 4; const int NODE_51_05 = NODE_51 + 5; const int NODE_51_06 = NODE_51 + 6; const int NODE_51_07 = NODE_51 + 7;
        public const int NODE_52 = 0x40000000 + 52 * DISCRETE_MAX_OUTPUTS; const int NODE_52_00 = NODE_52; const int NODE_52_01 = NODE_52 + 1; const int NODE_52_02 = NODE_52 + 2; const int NODE_52_03 = NODE_52 + 3; const int NODE_52_04 = NODE_52 + 4; const int NODE_52_05 = NODE_52 + 5; const int NODE_52_06 = NODE_52 + 6; const int NODE_52_07 = NODE_52 + 7;
        public const int NODE_53 = 0x40000000 + 53 * DISCRETE_MAX_OUTPUTS; const int NODE_53_00 = NODE_53; const int NODE_53_01 = NODE_53 + 1; const int NODE_53_02 = NODE_53 + 2; const int NODE_53_03 = NODE_53 + 3; const int NODE_53_04 = NODE_53 + 4; const int NODE_53_05 = NODE_53 + 5; const int NODE_53_06 = NODE_53 + 6; const int NODE_53_07 = NODE_53 + 7;
        public const int NODE_54 = 0x40000000 + 54 * DISCRETE_MAX_OUTPUTS; const int NODE_54_00 = NODE_54; const int NODE_54_01 = NODE_54 + 1; const int NODE_54_02 = NODE_54 + 2; const int NODE_54_03 = NODE_54 + 3; const int NODE_54_04 = NODE_54 + 4; const int NODE_54_05 = NODE_54 + 5; const int NODE_54_06 = NODE_54 + 6; const int NODE_54_07 = NODE_54 + 7;
        public const int NODE_55 = 0x40000000 + 55 * DISCRETE_MAX_OUTPUTS; const int NODE_55_00 = NODE_55; const int NODE_55_01 = NODE_55 + 1; const int NODE_55_02 = NODE_55 + 2; const int NODE_55_03 = NODE_55 + 3; const int NODE_55_04 = NODE_55 + 4; const int NODE_55_05 = NODE_55 + 5; const int NODE_55_06 = NODE_55 + 6; const int NODE_55_07 = NODE_55 + 7;
        const int NODE_56 = 0x40000000 + 56 * DISCRETE_MAX_OUTPUTS; const int NODE_56_00 = NODE_56; const int NODE_56_01 = NODE_56 + 1; const int NODE_56_02 = NODE_56 + 2; const int NODE_56_03 = NODE_56 + 3; const int NODE_56_04 = NODE_56 + 4; const int NODE_56_05 = NODE_56 + 5; const int NODE_56_06 = NODE_56 + 6; const int NODE_56_07 = NODE_56 + 7;
        const int NODE_57 = 0x40000000 + 57 * DISCRETE_MAX_OUTPUTS; const int NODE_57_00 = NODE_57; const int NODE_57_01 = NODE_57 + 1; const int NODE_57_02 = NODE_57 + 2; const int NODE_57_03 = NODE_57 + 3; const int NODE_57_04 = NODE_57 + 4; const int NODE_57_05 = NODE_57 + 5; const int NODE_57_06 = NODE_57 + 6; const int NODE_57_07 = NODE_57 + 7;
        const int NODE_58 = 0x40000000 + 58 * DISCRETE_MAX_OUTPUTS; const int NODE_58_00 = NODE_58; const int NODE_58_01 = NODE_58 + 1; const int NODE_58_02 = NODE_58 + 2; const int NODE_58_03 = NODE_58 + 3; const int NODE_58_04 = NODE_58 + 4; const int NODE_58_05 = NODE_58 + 5; const int NODE_58_06 = NODE_58 + 6; const int NODE_58_07 = NODE_58 + 7;
        const int NODE_59 = 0x40000000 + 59 * DISCRETE_MAX_OUTPUTS; const int NODE_59_00 = NODE_59; const int NODE_59_01 = NODE_59 + 1; const int NODE_59_02 = NODE_59 + 2; const int NODE_59_03 = NODE_59 + 3; const int NODE_59_04 = NODE_59 + 4; const int NODE_59_05 = NODE_59 + 5; const int NODE_59_06 = NODE_59 + 6; const int NODE_59_07 = NODE_59 + 7;

        public const int NODE_60 = 0x40000000 + 60 * DISCRETE_MAX_OUTPUTS; const int NODE_60_00 = NODE_60; const int NODE_60_01 = NODE_60 + 1; const int NODE_60_02 = NODE_60 + 2; const int NODE_60_03 = NODE_60 + 3; const int NODE_60_04 = NODE_60 + 4; const int NODE_60_05 = NODE_60 + 5; const int NODE_60_06 = NODE_60 + 6; const int NODE_60_07 = NODE_60 + 7;
        public const int NODE_61 = 0x40000000 + 61 * DISCRETE_MAX_OUTPUTS; const int NODE_61_00 = NODE_61; const int NODE_61_01 = NODE_61 + 1; const int NODE_61_02 = NODE_61 + 2; const int NODE_61_03 = NODE_61 + 3; const int NODE_61_04 = NODE_61 + 4; const int NODE_61_05 = NODE_61 + 5; const int NODE_61_06 = NODE_61 + 6; const int NODE_61_07 = NODE_61 + 7;
        public const int NODE_62 = 0x40000000 + 62 * DISCRETE_MAX_OUTPUTS; const int NODE_62_00 = NODE_62; const int NODE_62_01 = NODE_62 + 1; const int NODE_62_02 = NODE_62 + 2; const int NODE_62_03 = NODE_62 + 3; const int NODE_62_04 = NODE_62 + 4; const int NODE_62_05 = NODE_62 + 5; const int NODE_62_06 = NODE_62 + 6; const int NODE_62_07 = NODE_62 + 7;
        public const int NODE_63 = 0x40000000 + 63 * DISCRETE_MAX_OUTPUTS; const int NODE_63_00 = NODE_63; const int NODE_63_01 = NODE_63 + 1; const int NODE_63_02 = NODE_63 + 2; const int NODE_63_03 = NODE_63 + 3; const int NODE_63_04 = NODE_63 + 4; const int NODE_63_05 = NODE_63 + 5; const int NODE_63_06 = NODE_63 + 6; const int NODE_63_07 = NODE_63 + 7;
        public const int NODE_64 = 0x40000000 + 64 * DISCRETE_MAX_OUTPUTS; const int NODE_64_00 = NODE_64; const int NODE_64_01 = NODE_64 + 1; const int NODE_64_02 = NODE_64 + 2; const int NODE_64_03 = NODE_64 + 3; const int NODE_64_04 = NODE_64 + 4; const int NODE_64_05 = NODE_64 + 5; const int NODE_64_06 = NODE_64 + 6; const int NODE_64_07 = NODE_64 + 7;
        const int NODE_65 = 0x40000000 + 65 * DISCRETE_MAX_OUTPUTS; const int NODE_65_00 = NODE_65; const int NODE_65_01 = NODE_65 + 1; const int NODE_65_02 = NODE_65 + 2; const int NODE_65_03 = NODE_65 + 3; const int NODE_65_04 = NODE_65 + 4; const int NODE_65_05 = NODE_65 + 5; const int NODE_65_06 = NODE_65 + 6; const int NODE_65_07 = NODE_65 + 7;
        const int NODE_66 = 0x40000000 + 66 * DISCRETE_MAX_OUTPUTS; const int NODE_66_00 = NODE_66; const int NODE_66_01 = NODE_66 + 1; const int NODE_66_02 = NODE_66 + 2; const int NODE_66_03 = NODE_66 + 3; const int NODE_66_04 = NODE_66 + 4; const int NODE_66_05 = NODE_66 + 5; const int NODE_66_06 = NODE_66 + 6; const int NODE_66_07 = NODE_66 + 7;
        const int NODE_67 = 0x40000000 + 67 * DISCRETE_MAX_OUTPUTS; const int NODE_67_00 = NODE_67; const int NODE_67_01 = NODE_67 + 1; const int NODE_67_02 = NODE_67 + 2; const int NODE_67_03 = NODE_67 + 3; const int NODE_67_04 = NODE_67 + 4; const int NODE_67_05 = NODE_67 + 5; const int NODE_67_06 = NODE_67 + 6; const int NODE_67_07 = NODE_67 + 7;
        const int NODE_68 = 0x40000000 + 68 * DISCRETE_MAX_OUTPUTS; const int NODE_68_00 = NODE_68; const int NODE_68_01 = NODE_68 + 1; const int NODE_68_02 = NODE_68 + 2; const int NODE_68_03 = NODE_68 + 3; const int NODE_68_04 = NODE_68 + 4; const int NODE_68_05 = NODE_68 + 5; const int NODE_68_06 = NODE_68 + 6; const int NODE_68_07 = NODE_68 + 7;
        const int NODE_69 = 0x40000000 + 69 * DISCRETE_MAX_OUTPUTS; const int NODE_69_00 = NODE_69; const int NODE_69_01 = NODE_69 + 1; const int NODE_69_02 = NODE_69 + 2; const int NODE_69_03 = NODE_69 + 3; const int NODE_69_04 = NODE_69 + 4; const int NODE_69_05 = NODE_69 + 5; const int NODE_69_06 = NODE_69 + 6; const int NODE_69_07 = NODE_69 + 7;

        public const int NODE_70 = 0x40000000 + 70 * DISCRETE_MAX_OUTPUTS; const int NODE_70_00 = NODE_70; const int NODE_70_01 = NODE_70 + 1; const int NODE_70_02 = NODE_70 + 2; const int NODE_70_03 = NODE_70 + 3; const int NODE_70_04 = NODE_70 + 4; const int NODE_70_05 = NODE_70 + 5; const int NODE_70_06 = NODE_70 + 6; const int NODE_70_07 = NODE_70 + 7;
        public const int NODE_71 = 0x40000000 + 71 * DISCRETE_MAX_OUTPUTS; const int NODE_71_00 = NODE_71; const int NODE_71_01 = NODE_71 + 1; const int NODE_71_02 = NODE_71 + 2; const int NODE_71_03 = NODE_71 + 3; const int NODE_71_04 = NODE_71 + 4; const int NODE_71_05 = NODE_71 + 5; const int NODE_71_06 = NODE_71 + 6; const int NODE_71_07 = NODE_71 + 7;
        public const int NODE_72 = 0x40000000 + 72 * DISCRETE_MAX_OUTPUTS; const int NODE_72_00 = NODE_72; const int NODE_72_01 = NODE_72 + 1; const int NODE_72_02 = NODE_72 + 2; const int NODE_72_03 = NODE_72 + 3; const int NODE_72_04 = NODE_72 + 4; const int NODE_72_05 = NODE_72 + 5; const int NODE_72_06 = NODE_72 + 6; const int NODE_72_07 = NODE_72 + 7;
        public const int NODE_73 = 0x40000000 + 73 * DISCRETE_MAX_OUTPUTS; const int NODE_73_00 = NODE_73; const int NODE_73_01 = NODE_73 + 1; const int NODE_73_02 = NODE_73 + 2; const int NODE_73_03 = NODE_73 + 3; const int NODE_73_04 = NODE_73 + 4; const int NODE_73_05 = NODE_73 + 5; const int NODE_73_06 = NODE_73 + 6; const int NODE_73_07 = NODE_73 + 7;
        const int NODE_74 = 0x40000000 + 74 * DISCRETE_MAX_OUTPUTS; const int NODE_74_00 = NODE_74; const int NODE_74_01 = NODE_74 + 1; const int NODE_74_02 = NODE_74 + 2; const int NODE_74_03 = NODE_74 + 3; const int NODE_74_04 = NODE_74 + 4; const int NODE_74_05 = NODE_74 + 5; const int NODE_74_06 = NODE_74 + 6; const int NODE_74_07 = NODE_74 + 7;
        const int NODE_75 = 0x40000000 + 75 * DISCRETE_MAX_OUTPUTS; const int NODE_75_00 = NODE_75; const int NODE_75_01 = NODE_75 + 1; const int NODE_75_02 = NODE_75 + 2; const int NODE_75_03 = NODE_75 + 3; const int NODE_75_04 = NODE_75 + 4; const int NODE_75_05 = NODE_75 + 5; const int NODE_75_06 = NODE_75 + 6; const int NODE_75_07 = NODE_75 + 7;
        const int NODE_76 = 0x40000000 + 76 * DISCRETE_MAX_OUTPUTS; const int NODE_76_00 = NODE_76; const int NODE_76_01 = NODE_76 + 1; const int NODE_76_02 = NODE_76 + 2; const int NODE_76_03 = NODE_76 + 3; const int NODE_76_04 = NODE_76 + 4; const int NODE_76_05 = NODE_76 + 5; const int NODE_76_06 = NODE_76 + 6; const int NODE_76_07 = NODE_76 + 7;
        const int NODE_77 = 0x40000000 + 77 * DISCRETE_MAX_OUTPUTS; const int NODE_77_00 = NODE_77; const int NODE_77_01 = NODE_77 + 1; const int NODE_77_02 = NODE_77 + 2; const int NODE_77_03 = NODE_77 + 3; const int NODE_77_04 = NODE_77 + 4; const int NODE_77_05 = NODE_77 + 5; const int NODE_77_06 = NODE_77 + 6; const int NODE_77_07 = NODE_77 + 7;
        const int NODE_78 = 0x40000000 + 78 * DISCRETE_MAX_OUTPUTS; const int NODE_78_00 = NODE_78; const int NODE_78_01 = NODE_78 + 1; const int NODE_78_02 = NODE_78 + 2; const int NODE_78_03 = NODE_78 + 3; const int NODE_78_04 = NODE_78 + 4; const int NODE_78_05 = NODE_78 + 5; const int NODE_78_06 = NODE_78 + 6; const int NODE_78_07 = NODE_78 + 7;
        const int NODE_79 = 0x40000000 + 79 * DISCRETE_MAX_OUTPUTS; const int NODE_79_00 = NODE_79; const int NODE_79_01 = NODE_79 + 1; const int NODE_79_02 = NODE_79 + 2; const int NODE_79_03 = NODE_79 + 3; const int NODE_79_04 = NODE_79 + 4; const int NODE_79_05 = NODE_79 + 5; const int NODE_79_06 = NODE_79 + 6; const int NODE_79_07 = NODE_79 + 7;

        public const int NODE_80 = 0x40000000 + 80 * DISCRETE_MAX_OUTPUTS; const int NODE_80_00 = NODE_80; const int NODE_80_01 = NODE_80 + 1; const int NODE_80_02 = NODE_80 + 2; const int NODE_80_03 = NODE_80 + 3; const int NODE_80_04 = NODE_80 + 4; const int NODE_80_05 = NODE_80 + 5; const int NODE_80_06 = NODE_80 + 6; const int NODE_80_07 = NODE_80 + 7;
        public const int NODE_81 = 0x40000000 + 81 * DISCRETE_MAX_OUTPUTS; const int NODE_81_00 = NODE_81; const int NODE_81_01 = NODE_81 + 1; const int NODE_81_02 = NODE_81 + 2; const int NODE_81_03 = NODE_81 + 3; const int NODE_81_04 = NODE_81 + 4; const int NODE_81_05 = NODE_81 + 5; const int NODE_81_06 = NODE_81 + 6; const int NODE_81_07 = NODE_81 + 7;
        public const int NODE_82 = 0x40000000 + 82 * DISCRETE_MAX_OUTPUTS; const int NODE_82_00 = NODE_82; const int NODE_82_01 = NODE_82 + 1; const int NODE_82_02 = NODE_82 + 2; const int NODE_82_03 = NODE_82 + 3; const int NODE_82_04 = NODE_82 + 4; const int NODE_82_05 = NODE_82 + 5; const int NODE_82_06 = NODE_82 + 6; const int NODE_82_07 = NODE_82 + 7;
        public const int NODE_83 = 0x40000000 + 83 * DISCRETE_MAX_OUTPUTS; const int NODE_83_00 = NODE_83; const int NODE_83_01 = NODE_83 + 1; const int NODE_83_02 = NODE_83 + 2; const int NODE_83_03 = NODE_83 + 3; const int NODE_83_04 = NODE_83 + 4; const int NODE_83_05 = NODE_83 + 5; const int NODE_83_06 = NODE_83 + 6; const int NODE_83_07 = NODE_83 + 7;
        public const int NODE_84 = 0x40000000 + 84 * DISCRETE_MAX_OUTPUTS; const int NODE_84_00 = NODE_84; const int NODE_84_01 = NODE_84 + 1; const int NODE_84_02 = NODE_84 + 2; const int NODE_84_03 = NODE_84 + 3; const int NODE_84_04 = NODE_84 + 4; const int NODE_84_05 = NODE_84 + 5; const int NODE_84_06 = NODE_84 + 6; const int NODE_84_07 = NODE_84 + 7;
        const int NODE_85 = 0x40000000 + 85 * DISCRETE_MAX_OUTPUTS; const int NODE_85_00 = NODE_85; const int NODE_85_01 = NODE_85 + 1; const int NODE_85_02 = NODE_85 + 2; const int NODE_85_03 = NODE_85 + 3; const int NODE_85_04 = NODE_85 + 4; const int NODE_85_05 = NODE_85 + 5; const int NODE_85_06 = NODE_85 + 6; const int NODE_85_07 = NODE_85 + 7;
        const int NODE_86 = 0x40000000 + 86 * DISCRETE_MAX_OUTPUTS; const int NODE_86_00 = NODE_86; const int NODE_86_01 = NODE_86 + 1; const int NODE_86_02 = NODE_86 + 2; const int NODE_86_03 = NODE_86 + 3; const int NODE_86_04 = NODE_86 + 4; const int NODE_86_05 = NODE_86 + 5; const int NODE_86_06 = NODE_86 + 6; const int NODE_86_07 = NODE_86 + 7;
        const int NODE_87 = 0x40000000 + 87 * DISCRETE_MAX_OUTPUTS; const int NODE_87_00 = NODE_87; const int NODE_87_01 = NODE_87 + 1; const int NODE_87_02 = NODE_87 + 2; const int NODE_87_03 = NODE_87 + 3; const int NODE_87_04 = NODE_87 + 4; const int NODE_87_05 = NODE_87 + 5; const int NODE_87_06 = NODE_87 + 6; const int NODE_87_07 = NODE_87 + 7;
        const int NODE_88 = 0x40000000 + 88 * DISCRETE_MAX_OUTPUTS; const int NODE_88_00 = NODE_88; const int NODE_88_01 = NODE_88 + 1; const int NODE_88_02 = NODE_88 + 2; const int NODE_88_03 = NODE_88 + 3; const int NODE_88_04 = NODE_88 + 4; const int NODE_88_05 = NODE_88 + 5; const int NODE_88_06 = NODE_88 + 6; const int NODE_88_07 = NODE_88 + 7;
        const int NODE_89 = 0x40000000 + 89 * DISCRETE_MAX_OUTPUTS; const int NODE_89_00 = NODE_89; const int NODE_89_01 = NODE_89 + 1; const int NODE_89_02 = NODE_89 + 2; const int NODE_89_03 = NODE_89 + 3; const int NODE_89_04 = NODE_89 + 4; const int NODE_89_05 = NODE_89 + 5; const int NODE_89_06 = NODE_89 + 6; const int NODE_89_07 = NODE_89 + 7;

        public const int NODE_90 = 0x40000000 + 90 * DISCRETE_MAX_OUTPUTS; const int NODE_90_00 = NODE_90; const int NODE_90_01 = NODE_90 + 1; const int NODE_90_02 = NODE_90 + 2; const int NODE_90_03 = NODE_90 + 3; const int NODE_90_04 = NODE_90 + 4; const int NODE_90_05 = NODE_90 + 5; const int NODE_90_06 = NODE_90 + 6; const int NODE_90_07 = NODE_90 + 7;
        public const int NODE_91 = 0x40000000 + 91 * DISCRETE_MAX_OUTPUTS; const int NODE_91_00 = NODE_91; const int NODE_91_01 = NODE_91 + 1; const int NODE_91_02 = NODE_91 + 2; const int NODE_91_03 = NODE_91 + 3; const int NODE_91_04 = NODE_91 + 4; const int NODE_91_05 = NODE_91 + 5; const int NODE_91_06 = NODE_91 + 6; const int NODE_91_07 = NODE_91 + 7;
        const int NODE_92 = 0x40000000 + 92 * DISCRETE_MAX_OUTPUTS; const int NODE_92_00 = NODE_92; const int NODE_92_01 = NODE_92 + 1; const int NODE_92_02 = NODE_92 + 2; const int NODE_92_03 = NODE_92 + 3; const int NODE_92_04 = NODE_92 + 4; const int NODE_92_05 = NODE_92 + 5; const int NODE_92_06 = NODE_92 + 6; const int NODE_92_07 = NODE_92 + 7;
        const int NODE_93 = 0x40000000 + 93 * DISCRETE_MAX_OUTPUTS; const int NODE_93_00 = NODE_93; const int NODE_93_01 = NODE_93 + 1; const int NODE_93_02 = NODE_93 + 2; const int NODE_93_03 = NODE_93 + 3; const int NODE_93_04 = NODE_93 + 4; const int NODE_93_05 = NODE_93 + 5; const int NODE_93_06 = NODE_93 + 6; const int NODE_93_07 = NODE_93 + 7;
        const int NODE_94 = 0x40000000 + 94 * DISCRETE_MAX_OUTPUTS; const int NODE_94_00 = NODE_94; const int NODE_94_01 = NODE_94 + 1; const int NODE_94_02 = NODE_94 + 2; const int NODE_94_03 = NODE_94 + 3; const int NODE_94_04 = NODE_94 + 4; const int NODE_94_05 = NODE_94 + 5; const int NODE_94_06 = NODE_94 + 6; const int NODE_94_07 = NODE_94 + 7;
        const int NODE_95 = 0x40000000 + 95 * DISCRETE_MAX_OUTPUTS; const int NODE_95_00 = NODE_95; const int NODE_95_01 = NODE_95 + 1; const int NODE_95_02 = NODE_95 + 2; const int NODE_95_03 = NODE_95 + 3; const int NODE_95_04 = NODE_95 + 4; const int NODE_95_05 = NODE_95 + 5; const int NODE_95_06 = NODE_95 + 6; const int NODE_95_07 = NODE_95 + 7;
        const int NODE_96 = 0x40000000 + 96 * DISCRETE_MAX_OUTPUTS; const int NODE_96_00 = NODE_96; const int NODE_96_01 = NODE_96 + 1; const int NODE_96_02 = NODE_96 + 2; const int NODE_96_03 = NODE_96 + 3; const int NODE_96_04 = NODE_96 + 4; const int NODE_96_05 = NODE_96 + 5; const int NODE_96_06 = NODE_96 + 6; const int NODE_96_07 = NODE_96 + 7;
        const int NODE_97 = 0x40000000 + 97 * DISCRETE_MAX_OUTPUTS; const int NODE_97_00 = NODE_97; const int NODE_97_01 = NODE_97 + 1; const int NODE_97_02 = NODE_97 + 2; const int NODE_97_03 = NODE_97 + 3; const int NODE_97_04 = NODE_97 + 4; const int NODE_97_05 = NODE_97 + 5; const int NODE_97_06 = NODE_97 + 6; const int NODE_97_07 = NODE_97 + 7;
        const int NODE_98 = 0x40000000 + 98 * DISCRETE_MAX_OUTPUTS; const int NODE_98_00 = NODE_98; const int NODE_98_01 = NODE_98 + 1; const int NODE_98_02 = NODE_98 + 2; const int NODE_98_03 = NODE_98 + 3; const int NODE_98_04 = NODE_98 + 4; const int NODE_98_05 = NODE_98 + 5; const int NODE_98_06 = NODE_98 + 6; const int NODE_98_07 = NODE_98 + 7;
        const int NODE_99 = 0x40000000 + 99 * DISCRETE_MAX_OUTPUTS; const int NODE_99_00 = NODE_99; const int NODE_99_01 = NODE_99 + 1; const int NODE_99_02 = NODE_99 + 2; const int NODE_99_03 = NODE_99 + 3; const int NODE_99_04 = NODE_99 + 4; const int NODE_99_05 = NODE_99 + 5; const int NODE_99_06 = NODE_99 + 6; const int NODE_99_07 = NODE_99 + 7;

        public const int NODE_100 = 0x40000000 + 100 * DISCRETE_MAX_OUTPUTS; const int NODE_100_00 = NODE_100; const int NODE_100_01 = NODE_100 + 1; const int NODE_100_02 = NODE_100 + 2; const int NODE_100_03 = NODE_100 + 3; const int NODE_100_04 = NODE_100 + 4; const int NODE_100_05 = NODE_100 + 5; const int NODE_100_06 = NODE_100 + 6; const int NODE_100_07 = NODE_100 + 7;
        const int NODE_101 = 0x40000000 + 101 * DISCRETE_MAX_OUTPUTS; const int NODE_101_00 = NODE_101; const int NODE_101_01 = NODE_101 + 1; const int NODE_101_02 = NODE_101 + 2; const int NODE_101_03 = NODE_101 + 3; const int NODE_101_04 = NODE_101 + 4; const int NODE_101_05 = NODE_101 + 5; const int NODE_101_06 = NODE_101 + 6; const int NODE_101_07 = NODE_101 + 7;
        const int NODE_102 = 0x40000000 + 102 * DISCRETE_MAX_OUTPUTS; const int NODE_102_00 = NODE_102; const int NODE_102_01 = NODE_102 + 1; const int NODE_102_02 = NODE_102 + 2; const int NODE_102_03 = NODE_102 + 3; const int NODE_102_04 = NODE_102 + 4; const int NODE_102_05 = NODE_102 + 5; const int NODE_102_06 = NODE_102 + 6; const int NODE_102_07 = NODE_102 + 7;
        const int NODE_103 = 0x40000000 + 103 * DISCRETE_MAX_OUTPUTS; const int NODE_103_00 = NODE_103; const int NODE_103_01 = NODE_103 + 1; const int NODE_103_02 = NODE_103 + 2; const int NODE_103_03 = NODE_103 + 3; const int NODE_103_04 = NODE_103 + 4; const int NODE_103_05 = NODE_103 + 5; const int NODE_103_06 = NODE_103 + 6; const int NODE_103_07 = NODE_103 + 7;
        const int NODE_104 = 0x40000000 + 104 * DISCRETE_MAX_OUTPUTS; const int NODE_104_00 = NODE_104; const int NODE_104_01 = NODE_104 + 1; const int NODE_104_02 = NODE_104 + 2; const int NODE_104_03 = NODE_104 + 3; const int NODE_104_04 = NODE_104 + 4; const int NODE_104_05 = NODE_104 + 5; const int NODE_104_06 = NODE_104 + 6; const int NODE_104_07 = NODE_104 + 7;
        public const int NODE_105 = 0x40000000 + 105 * DISCRETE_MAX_OUTPUTS; const int NODE_105_00 = NODE_105; const int NODE_105_01 = NODE_105 + 1; const int NODE_105_02 = NODE_105 + 2; const int NODE_105_03 = NODE_105 + 3; const int NODE_105_04 = NODE_105 + 4; const int NODE_105_05 = NODE_105 + 5; const int NODE_105_06 = NODE_105 + 6; const int NODE_105_07 = NODE_105 + 7;
        const int NODE_106 = 0x40000000 + 106 * DISCRETE_MAX_OUTPUTS; const int NODE_106_00 = NODE_106; const int NODE_106_01 = NODE_106 + 1; const int NODE_106_02 = NODE_106 + 2; const int NODE_106_03 = NODE_106 + 3; const int NODE_106_04 = NODE_106 + 4; const int NODE_106_05 = NODE_106 + 5; const int NODE_106_06 = NODE_106 + 6; const int NODE_106_07 = NODE_106 + 7;
        const int NODE_107 = 0x40000000 + 107 * DISCRETE_MAX_OUTPUTS; const int NODE_107_00 = NODE_107; const int NODE_107_01 = NODE_107 + 1; const int NODE_107_02 = NODE_107 + 2; const int NODE_107_03 = NODE_107 + 3; const int NODE_107_04 = NODE_107 + 4; const int NODE_107_05 = NODE_107 + 5; const int NODE_107_06 = NODE_107 + 6; const int NODE_107_07 = NODE_107 + 7;
        const int NODE_108 = 0x40000000 + 108 * DISCRETE_MAX_OUTPUTS; const int NODE_108_00 = NODE_108; const int NODE_108_01 = NODE_108 + 1; const int NODE_108_02 = NODE_108 + 2; const int NODE_108_03 = NODE_108 + 3; const int NODE_108_04 = NODE_108 + 4; const int NODE_108_05 = NODE_108 + 5; const int NODE_108_06 = NODE_108 + 6; const int NODE_108_07 = NODE_108 + 7;
        const int NODE_109 = 0x40000000 + 109 * DISCRETE_MAX_OUTPUTS; const int NODE_109_00 = NODE_109; const int NODE_109_01 = NODE_109 + 1; const int NODE_109_02 = NODE_109 + 2; const int NODE_109_03 = NODE_109 + 3; const int NODE_109_04 = NODE_109 + 4; const int NODE_109_05 = NODE_109 + 5; const int NODE_109_06 = NODE_109 + 6; const int NODE_109_07 = NODE_109 + 7;

        public const int NODE_110 = 0x40000000 + 110 * DISCRETE_MAX_OUTPUTS; const int NODE_110_00 = NODE_110; const int NODE_110_01 = NODE_110 + 1; const int NODE_110_02 = NODE_110 + 2; const int NODE_110_03 = NODE_110 + 3; const int NODE_110_04 = NODE_110 + 4; const int NODE_110_05 = NODE_110 + 5; const int NODE_110_06 = NODE_110 + 6; const int NODE_110_07 = NODE_110 + 7;
        public const int NODE_111 = 0x40000000 + 111 * DISCRETE_MAX_OUTPUTS; const int NODE_111_00 = NODE_111; const int NODE_111_01 = NODE_111 + 1; const int NODE_111_02 = NODE_111 + 2; const int NODE_111_03 = NODE_111 + 3; const int NODE_111_04 = NODE_111 + 4; const int NODE_111_05 = NODE_111 + 5; const int NODE_111_06 = NODE_111 + 6; const int NODE_111_07 = NODE_111 + 7;
        const int NODE_112 = 0x40000000 + 112 * DISCRETE_MAX_OUTPUTS; const int NODE_112_00 = NODE_112; const int NODE_112_01 = NODE_112 + 1; const int NODE_112_02 = NODE_112 + 2; const int NODE_112_03 = NODE_112 + 3; const int NODE_112_04 = NODE_112 + 4; const int NODE_112_05 = NODE_112 + 5; const int NODE_112_06 = NODE_112 + 6; const int NODE_112_07 = NODE_112 + 7;
        const int NODE_113 = 0x40000000 + 113 * DISCRETE_MAX_OUTPUTS; const int NODE_113_00 = NODE_113; const int NODE_113_01 = NODE_113 + 1; const int NODE_113_02 = NODE_113 + 2; const int NODE_113_03 = NODE_113 + 3; const int NODE_113_04 = NODE_113 + 4; const int NODE_113_05 = NODE_113 + 5; const int NODE_113_06 = NODE_113 + 6; const int NODE_113_07 = NODE_113 + 7;
        const int NODE_114 = 0x40000000 + 114 * DISCRETE_MAX_OUTPUTS; const int NODE_114_00 = NODE_114; const int NODE_114_01 = NODE_114 + 1; const int NODE_114_02 = NODE_114 + 2; const int NODE_114_03 = NODE_114 + 3; const int NODE_114_04 = NODE_114 + 4; const int NODE_114_05 = NODE_114 + 5; const int NODE_114_06 = NODE_114 + 6; const int NODE_114_07 = NODE_114 + 7;
        public const int NODE_115 = 0x40000000 + 115 * DISCRETE_MAX_OUTPUTS; const int NODE_115_00 = NODE_115; const int NODE_115_01 = NODE_115 + 1; const int NODE_115_02 = NODE_115 + 2; const int NODE_115_03 = NODE_115 + 3; const int NODE_115_04 = NODE_115 + 4; const int NODE_115_05 = NODE_115 + 5; const int NODE_115_06 = NODE_115 + 6; const int NODE_115_07 = NODE_115 + 7;
        public const int NODE_116 = 0x40000000 + 116 * DISCRETE_MAX_OUTPUTS; const int NODE_116_00 = NODE_116; const int NODE_116_01 = NODE_116 + 1; const int NODE_116_02 = NODE_116 + 2; const int NODE_116_03 = NODE_116 + 3; const int NODE_116_04 = NODE_116 + 4; const int NODE_116_05 = NODE_116 + 5; const int NODE_116_06 = NODE_116 + 6; const int NODE_116_07 = NODE_116 + 7;
        public const int NODE_117 = 0x40000000 + 117 * DISCRETE_MAX_OUTPUTS; const int NODE_117_00 = NODE_117; const int NODE_117_01 = NODE_117 + 1; const int NODE_117_02 = NODE_117 + 2; const int NODE_117_03 = NODE_117 + 3; const int NODE_117_04 = NODE_117 + 4; const int NODE_117_05 = NODE_117 + 5; const int NODE_117_06 = NODE_117 + 6; const int NODE_117_07 = NODE_117 + 7;
        const int NODE_118 = 0x40000000 + 118 * DISCRETE_MAX_OUTPUTS; const int NODE_118_00 = NODE_118; const int NODE_118_01 = NODE_118 + 1; const int NODE_118_02 = NODE_118 + 2; const int NODE_118_03 = NODE_118 + 3; const int NODE_118_04 = NODE_118 + 4; const int NODE_118_05 = NODE_118 + 5; const int NODE_118_06 = NODE_118 + 6; const int NODE_118_07 = NODE_118 + 7;
        const int NODE_119 = 0x40000000 + 119 * DISCRETE_MAX_OUTPUTS; const int NODE_119_00 = NODE_119; const int NODE_119_01 = NODE_119 + 1; const int NODE_119_02 = NODE_119 + 2; const int NODE_119_03 = NODE_119 + 3; const int NODE_119_04 = NODE_119 + 4; const int NODE_119_05 = NODE_119 + 5; const int NODE_119_06 = NODE_119 + 6; const int NODE_119_07 = NODE_119 + 7;

        public const int NODE_120 = 0x40000000 + 120 * DISCRETE_MAX_OUTPUTS; const int NODE_120_00 = NODE_120; const int NODE_120_01 = NODE_120 + 1; const int NODE_120_02 = NODE_120 + 2; const int NODE_120_03 = NODE_120 + 3; const int NODE_120_04 = NODE_120 + 4; const int NODE_120_05 = NODE_120 + 5; const int NODE_120_06 = NODE_120 + 6; const int NODE_120_07 = NODE_120 + 7;
        const int NODE_121 = 0x40000000 + 121 * DISCRETE_MAX_OUTPUTS; const int NODE_121_00 = NODE_121; const int NODE_121_01 = NODE_121 + 1; const int NODE_121_02 = NODE_121 + 2; const int NODE_121_03 = NODE_121 + 3; const int NODE_121_04 = NODE_121 + 4; const int NODE_121_05 = NODE_121 + 5; const int NODE_121_06 = NODE_121 + 6; const int NODE_121_07 = NODE_121 + 7;
        const int NODE_122 = 0x40000000 + 122 * DISCRETE_MAX_OUTPUTS; const int NODE_122_00 = NODE_122; const int NODE_122_01 = NODE_122 + 1; const int NODE_122_02 = NODE_122 + 2; const int NODE_122_03 = NODE_122 + 3; const int NODE_122_04 = NODE_122 + 4; const int NODE_122_05 = NODE_122 + 5; const int NODE_122_06 = NODE_122 + 6; const int NODE_122_07 = NODE_122 + 7;
        const int NODE_123 = 0x40000000 + 123 * DISCRETE_MAX_OUTPUTS; const int NODE_123_00 = NODE_123; const int NODE_123_01 = NODE_123 + 1; const int NODE_123_02 = NODE_123 + 2; const int NODE_123_03 = NODE_123 + 3; const int NODE_123_04 = NODE_123 + 4; const int NODE_123_05 = NODE_123 + 5; const int NODE_123_06 = NODE_123 + 6; const int NODE_123_07 = NODE_123 + 7;
        const int NODE_124 = 0x40000000 + 124 * DISCRETE_MAX_OUTPUTS; const int NODE_124_00 = NODE_124; const int NODE_124_01 = NODE_124 + 1; const int NODE_124_02 = NODE_124 + 2; const int NODE_124_03 = NODE_124 + 3; const int NODE_124_04 = NODE_124 + 4; const int NODE_124_05 = NODE_124 + 5; const int NODE_124_06 = NODE_124 + 6; const int NODE_124_07 = NODE_124 + 7;
        const int NODE_125 = 0x40000000 + 125 * DISCRETE_MAX_OUTPUTS; const int NODE_125_00 = NODE_125; const int NODE_125_01 = NODE_125 + 1; const int NODE_125_02 = NODE_125 + 2; const int NODE_125_03 = NODE_125 + 3; const int NODE_125_04 = NODE_125 + 4; const int NODE_125_05 = NODE_125 + 5; const int NODE_125_06 = NODE_125 + 6; const int NODE_125_07 = NODE_125 + 7;
        const int NODE_126 = 0x40000000 + 126 * DISCRETE_MAX_OUTPUTS; const int NODE_126_00 = NODE_126; const int NODE_126_01 = NODE_126 + 1; const int NODE_126_02 = NODE_126 + 2; const int NODE_126_03 = NODE_126 + 3; const int NODE_126_04 = NODE_126 + 4; const int NODE_126_05 = NODE_126 + 5; const int NODE_126_06 = NODE_126 + 6; const int NODE_126_07 = NODE_126 + 7;
        const int NODE_127 = 0x40000000 + 127 * DISCRETE_MAX_OUTPUTS; const int NODE_127_00 = NODE_127; const int NODE_127_01 = NODE_127 + 1; const int NODE_127_02 = NODE_127 + 2; const int NODE_127_03 = NODE_127 + 3; const int NODE_127_04 = NODE_127 + 4; const int NODE_127_05 = NODE_127 + 5; const int NODE_127_06 = NODE_127 + 6; const int NODE_127_07 = NODE_127 + 7;
        const int NODE_128 = 0x40000000 + 128 * DISCRETE_MAX_OUTPUTS; const int NODE_128_00 = NODE_128; const int NODE_128_01 = NODE_128 + 1; const int NODE_128_02 = NODE_128 + 2; const int NODE_128_03 = NODE_128 + 3; const int NODE_128_04 = NODE_128 + 4; const int NODE_128_05 = NODE_128 + 5; const int NODE_128_06 = NODE_128 + 6; const int NODE_128_07 = NODE_128 + 7;
        const int NODE_129 = 0x40000000 + 129 * DISCRETE_MAX_OUTPUTS; const int NODE_129_00 = NODE_129; const int NODE_129_01 = NODE_129 + 1; const int NODE_129_02 = NODE_129 + 2; const int NODE_129_03 = NODE_129 + 3; const int NODE_129_04 = NODE_129 + 4; const int NODE_129_05 = NODE_129 + 5; const int NODE_129_06 = NODE_129 + 6; const int NODE_129_07 = NODE_129 + 7;

        const int NODE_130 = 0x40000000 + 130 * DISCRETE_MAX_OUTPUTS; const int NODE_130_00 = NODE_130; const int NODE_130_01 = NODE_130 + 1; const int NODE_130_02 = NODE_130 + 2; const int NODE_130_03 = NODE_130 + 3; const int NODE_130_04 = NODE_130 + 4; const int NODE_130_05 = NODE_130 + 5; const int NODE_130_06 = NODE_130 + 6; const int NODE_130_07 = NODE_130 + 7;
        const int NODE_131 = 0x40000000 + 131 * DISCRETE_MAX_OUTPUTS; const int NODE_131_00 = NODE_131; const int NODE_131_01 = NODE_131 + 1; const int NODE_131_02 = NODE_131 + 2; const int NODE_131_03 = NODE_131 + 3; const int NODE_131_04 = NODE_131 + 4; const int NODE_131_05 = NODE_131 + 5; const int NODE_131_06 = NODE_131 + 6; const int NODE_131_07 = NODE_131 + 7;
        public const int NODE_132 = 0x40000000 + 132 * DISCRETE_MAX_OUTPUTS; const int NODE_132_00 = NODE_132; const int NODE_132_01 = NODE_132 + 1; const int NODE_132_02 = NODE_132 + 2; const int NODE_132_03 = NODE_132 + 3; const int NODE_132_04 = NODE_132 + 4; const int NODE_132_05 = NODE_132 + 5; const int NODE_132_06 = NODE_132 + 6; const int NODE_132_07 = NODE_132 + 7;
        public const int NODE_133 = 0x40000000 + 133 * DISCRETE_MAX_OUTPUTS; public const int NODE_133_00 = NODE_133; const int NODE_133_01 = NODE_133 + 1; public const int NODE_133_02 = NODE_133 + 2; public const int NODE_133_03 = NODE_133 + 3; const int NODE_133_04 = NODE_133 + 4; const int NODE_133_05 = NODE_133 + 5; const int NODE_133_06 = NODE_133 + 6; const int NODE_133_07 = NODE_133 + 7;
        const int NODE_134 = 0x40000000 + 134 * DISCRETE_MAX_OUTPUTS; const int NODE_134_00 = NODE_134; const int NODE_134_01 = NODE_134 + 1; const int NODE_134_02 = NODE_134 + 2; const int NODE_134_03 = NODE_134 + 3; const int NODE_134_04 = NODE_134 + 4; const int NODE_134_05 = NODE_134 + 5; const int NODE_134_06 = NODE_134 + 6; const int NODE_134_07 = NODE_134 + 7;
        const int NODE_135 = 0x40000000 + 135 * DISCRETE_MAX_OUTPUTS; const int NODE_135_00 = NODE_135; const int NODE_135_01 = NODE_135 + 1; const int NODE_135_02 = NODE_135 + 2; const int NODE_135_03 = NODE_135 + 3; const int NODE_135_04 = NODE_135 + 4; const int NODE_135_05 = NODE_135 + 5; const int NODE_135_06 = NODE_135 + 6; const int NODE_135_07 = NODE_135 + 7;
        const int NODE_136 = 0x40000000 + 136 * DISCRETE_MAX_OUTPUTS; const int NODE_136_00 = NODE_136; const int NODE_136_01 = NODE_136 + 1; const int NODE_136_02 = NODE_136 + 2; const int NODE_136_03 = NODE_136 + 3; const int NODE_136_04 = NODE_136 + 4; const int NODE_136_05 = NODE_136 + 5; const int NODE_136_06 = NODE_136 + 6; const int NODE_136_07 = NODE_136 + 7;
        const int NODE_137 = 0x40000000 + 137 * DISCRETE_MAX_OUTPUTS; const int NODE_137_00 = NODE_137; const int NODE_137_01 = NODE_137 + 1; const int NODE_137_02 = NODE_137 + 2; const int NODE_137_03 = NODE_137 + 3; const int NODE_137_04 = NODE_137 + 4; const int NODE_137_05 = NODE_137 + 5; const int NODE_137_06 = NODE_137 + 6; const int NODE_137_07 = NODE_137 + 7;
        const int NODE_138 = 0x40000000 + 138 * DISCRETE_MAX_OUTPUTS; const int NODE_138_00 = NODE_138; const int NODE_138_01 = NODE_138 + 1; const int NODE_138_02 = NODE_138 + 2; const int NODE_138_03 = NODE_138 + 3; const int NODE_138_04 = NODE_138 + 4; const int NODE_138_05 = NODE_138 + 5; const int NODE_138_06 = NODE_138 + 6; const int NODE_138_07 = NODE_138 + 7;
        const int NODE_139 = 0x40000000 + 139 * DISCRETE_MAX_OUTPUTS; const int NODE_139_00 = NODE_139; const int NODE_139_01 = NODE_139 + 1; const int NODE_139_02 = NODE_139 + 2; const int NODE_139_03 = NODE_139 + 3; const int NODE_139_04 = NODE_139 + 4; const int NODE_139_05 = NODE_139 + 5; const int NODE_139_06 = NODE_139 + 6; const int NODE_139_07 = NODE_139 + 7;

        const int NODE_140 = 0x40000000 + 140 * DISCRETE_MAX_OUTPUTS; const int NODE_140_00 = NODE_140; const int NODE_140_01 = NODE_140 + 1; const int NODE_140_02 = NODE_140 + 2; const int NODE_140_03 = NODE_140 + 3; const int NODE_140_04 = NODE_140 + 4; const int NODE_140_05 = NODE_140 + 5; const int NODE_140_06 = NODE_140 + 6; const int NODE_140_07 = NODE_140 + 7;
        const int NODE_141 = 0x40000000 + 141 * DISCRETE_MAX_OUTPUTS; const int NODE_141_00 = NODE_141; const int NODE_141_01 = NODE_141 + 1; const int NODE_141_02 = NODE_141 + 2; const int NODE_141_03 = NODE_141 + 3; const int NODE_141_04 = NODE_141 + 4; const int NODE_141_05 = NODE_141 + 5; const int NODE_141_06 = NODE_141 + 6; const int NODE_141_07 = NODE_141 + 7;
        const int NODE_142 = 0x40000000 + 142 * DISCRETE_MAX_OUTPUTS; const int NODE_142_00 = NODE_142; const int NODE_142_01 = NODE_142 + 1; const int NODE_142_02 = NODE_142 + 2; const int NODE_142_03 = NODE_142 + 3; const int NODE_142_04 = NODE_142 + 4; const int NODE_142_05 = NODE_142 + 5; const int NODE_142_06 = NODE_142 + 6; const int NODE_142_07 = NODE_142 + 7;
        const int NODE_143 = 0x40000000 + 143 * DISCRETE_MAX_OUTPUTS; const int NODE_143_00 = NODE_143; const int NODE_143_01 = NODE_143 + 1; const int NODE_143_02 = NODE_143 + 2; const int NODE_143_03 = NODE_143 + 3; const int NODE_143_04 = NODE_143 + 4; const int NODE_143_05 = NODE_143 + 5; const int NODE_143_06 = NODE_143 + 6; const int NODE_143_07 = NODE_143 + 7;
        const int NODE_144 = 0x40000000 + 144 * DISCRETE_MAX_OUTPUTS; const int NODE_144_00 = NODE_144; const int NODE_144_01 = NODE_144 + 1; const int NODE_144_02 = NODE_144 + 2; const int NODE_144_03 = NODE_144 + 3; const int NODE_144_04 = NODE_144 + 4; const int NODE_144_05 = NODE_144 + 5; const int NODE_144_06 = NODE_144 + 6; const int NODE_144_07 = NODE_144 + 7;
        const int NODE_145 = 0x40000000 + 145 * DISCRETE_MAX_OUTPUTS; const int NODE_145_00 = NODE_145; const int NODE_145_01 = NODE_145 + 1; const int NODE_145_02 = NODE_145 + 2; const int NODE_145_03 = NODE_145 + 3; const int NODE_145_04 = NODE_145 + 4; const int NODE_145_05 = NODE_145 + 5; const int NODE_145_06 = NODE_145 + 6; const int NODE_145_07 = NODE_145 + 7;
        const int NODE_146 = 0x40000000 + 146 * DISCRETE_MAX_OUTPUTS; const int NODE_146_00 = NODE_146; const int NODE_146_01 = NODE_146 + 1; const int NODE_146_02 = NODE_146 + 2; const int NODE_146_03 = NODE_146 + 3; const int NODE_146_04 = NODE_146 + 4; const int NODE_146_05 = NODE_146 + 5; const int NODE_146_06 = NODE_146 + 6; const int NODE_146_07 = NODE_146 + 7;
        const int NODE_147 = 0x40000000 + 147 * DISCRETE_MAX_OUTPUTS; const int NODE_147_00 = NODE_147; const int NODE_147_01 = NODE_147 + 1; const int NODE_147_02 = NODE_147 + 2; const int NODE_147_03 = NODE_147 + 3; const int NODE_147_04 = NODE_147 + 4; const int NODE_147_05 = NODE_147 + 5; const int NODE_147_06 = NODE_147 + 6; const int NODE_147_07 = NODE_147 + 7;
        const int NODE_148 = 0x40000000 + 148 * DISCRETE_MAX_OUTPUTS; const int NODE_148_00 = NODE_148; const int NODE_148_01 = NODE_148 + 1; const int NODE_148_02 = NODE_148 + 2; const int NODE_148_03 = NODE_148 + 3; const int NODE_148_04 = NODE_148 + 4; const int NODE_148_05 = NODE_148 + 5; const int NODE_148_06 = NODE_148 + 6; const int NODE_148_07 = NODE_148 + 7;
        const int NODE_149 = 0x40000000 + 149 * DISCRETE_MAX_OUTPUTS; const int NODE_149_00 = NODE_149; const int NODE_149_01 = NODE_149 + 1; const int NODE_149_02 = NODE_149 + 2; const int NODE_149_03 = NODE_149 + 3; const int NODE_149_04 = NODE_149 + 4; const int NODE_149_05 = NODE_149 + 5; const int NODE_149_06 = NODE_149 + 6; const int NODE_149_07 = NODE_149 + 7;

        public const int NODE_150 = 0x40000000 + 150 * DISCRETE_MAX_OUTPUTS; const int NODE_150_00 = NODE_150; const int NODE_150_01 = NODE_150 + 1; const int NODE_150_02 = NODE_150 + 2; const int NODE_150_03 = NODE_150 + 3; const int NODE_150_04 = NODE_150 + 4; const int NODE_150_05 = NODE_150 + 5; const int NODE_150_06 = NODE_150 + 6; const int NODE_150_07 = NODE_150 + 7;
        public const int NODE_151 = 0x40000000 + 151 * DISCRETE_MAX_OUTPUTS; const int NODE_151_00 = NODE_151; const int NODE_151_01 = NODE_151 + 1; const int NODE_151_02 = NODE_151 + 2; const int NODE_151_03 = NODE_151 + 3; const int NODE_151_04 = NODE_151 + 4; const int NODE_151_05 = NODE_151 + 5; const int NODE_151_06 = NODE_151 + 6; const int NODE_151_07 = NODE_151 + 7;
        public const int NODE_152 = 0x40000000 + 152 * DISCRETE_MAX_OUTPUTS; const int NODE_152_00 = NODE_152; const int NODE_152_01 = NODE_152 + 1; const int NODE_152_02 = NODE_152 + 2; const int NODE_152_03 = NODE_152 + 3; const int NODE_152_04 = NODE_152 + 4; const int NODE_152_05 = NODE_152 + 5; const int NODE_152_06 = NODE_152 + 6; const int NODE_152_07 = NODE_152 + 7;
        const int NODE_153 = 0x40000000 + 153 * DISCRETE_MAX_OUTPUTS; const int NODE_153_00 = NODE_153; const int NODE_153_01 = NODE_153 + 1; const int NODE_153_02 = NODE_153 + 2; const int NODE_153_03 = NODE_153 + 3; const int NODE_153_04 = NODE_153 + 4; const int NODE_153_05 = NODE_153 + 5; const int NODE_153_06 = NODE_153 + 6; const int NODE_153_07 = NODE_153 + 7;
        const int NODE_154 = 0x40000000 + 154 * DISCRETE_MAX_OUTPUTS; const int NODE_154_00 = NODE_154; const int NODE_154_01 = NODE_154 + 1; const int NODE_154_02 = NODE_154 + 2; const int NODE_154_03 = NODE_154 + 3; const int NODE_154_04 = NODE_154 + 4; const int NODE_154_05 = NODE_154 + 5; const int NODE_154_06 = NODE_154 + 6; const int NODE_154_07 = NODE_154 + 7;
        public const int NODE_155 = 0x40000000 + 155 * DISCRETE_MAX_OUTPUTS; const int NODE_155_00 = NODE_155; const int NODE_155_01 = NODE_155 + 1; const int NODE_155_02 = NODE_155 + 2; const int NODE_155_03 = NODE_155 + 3; const int NODE_155_04 = NODE_155 + 4; const int NODE_155_05 = NODE_155 + 5; const int NODE_155_06 = NODE_155 + 6; const int NODE_155_07 = NODE_155 + 7;
        const int NODE_156 = 0x40000000 + 156 * DISCRETE_MAX_OUTPUTS; const int NODE_156_00 = NODE_156; const int NODE_156_01 = NODE_156 + 1; const int NODE_156_02 = NODE_156 + 2; const int NODE_156_03 = NODE_156 + 3; const int NODE_156_04 = NODE_156 + 4; const int NODE_156_05 = NODE_156 + 5; const int NODE_156_06 = NODE_156 + 6; const int NODE_156_07 = NODE_156 + 7;
        public const int NODE_157 = 0x40000000 + 157 * DISCRETE_MAX_OUTPUTS; const int NODE_157_00 = NODE_157; const int NODE_157_01 = NODE_157 + 1; const int NODE_157_02 = NODE_157 + 2; const int NODE_157_03 = NODE_157 + 3; const int NODE_157_04 = NODE_157 + 4; const int NODE_157_05 = NODE_157 + 5; const int NODE_157_06 = NODE_157 + 6; const int NODE_157_07 = NODE_157 + 7;
        const int NODE_158 = 0x40000000 + 158 * DISCRETE_MAX_OUTPUTS; const int NODE_158_00 = NODE_158; const int NODE_158_01 = NODE_158 + 1; const int NODE_158_02 = NODE_158 + 2; const int NODE_158_03 = NODE_158 + 3; const int NODE_158_04 = NODE_158 + 4; const int NODE_158_05 = NODE_158 + 5; const int NODE_158_06 = NODE_158 + 6; const int NODE_158_07 = NODE_158 + 7;
        const int NODE_159 = 0x40000000 + 159 * DISCRETE_MAX_OUTPUTS; const int NODE_159_00 = NODE_159; const int NODE_159_01 = NODE_159 + 1; const int NODE_159_02 = NODE_159 + 2; const int NODE_159_03 = NODE_159 + 3; const int NODE_159_04 = NODE_159 + 4; const int NODE_159_05 = NODE_159 + 5; const int NODE_159_06 = NODE_159 + 6; const int NODE_159_07 = NODE_159 + 7;

        const int NODE_160 = 0x40000000 + 160 * DISCRETE_MAX_OUTPUTS; const int NODE_160_00 = NODE_160; const int NODE_160_01 = NODE_160 + 1; const int NODE_160_02 = NODE_160 + 2; const int NODE_160_03 = NODE_160 + 3; const int NODE_160_04 = NODE_160 + 4; const int NODE_160_05 = NODE_160 + 5; const int NODE_160_06 = NODE_160 + 6; const int NODE_160_07 = NODE_160 + 7;
        const int NODE_161 = 0x40000000 + 161 * DISCRETE_MAX_OUTPUTS; const int NODE_161_00 = NODE_161; const int NODE_161_01 = NODE_161 + 1; const int NODE_161_02 = NODE_161 + 2; const int NODE_161_03 = NODE_161 + 3; const int NODE_161_04 = NODE_161 + 4; const int NODE_161_05 = NODE_161 + 5; const int NODE_161_06 = NODE_161 + 6; const int NODE_161_07 = NODE_161 + 7;
        const int NODE_162 = 0x40000000 + 162 * DISCRETE_MAX_OUTPUTS; const int NODE_162_00 = NODE_162; const int NODE_162_01 = NODE_162 + 1; const int NODE_162_02 = NODE_162 + 2; const int NODE_162_03 = NODE_162 + 3; const int NODE_162_04 = NODE_162 + 4; const int NODE_162_05 = NODE_162 + 5; const int NODE_162_06 = NODE_162 + 6; const int NODE_162_07 = NODE_162 + 7;
        const int NODE_163 = 0x40000000 + 163 * DISCRETE_MAX_OUTPUTS; const int NODE_163_00 = NODE_163; const int NODE_163_01 = NODE_163 + 1; const int NODE_163_02 = NODE_163 + 2; const int NODE_163_03 = NODE_163 + 3; const int NODE_163_04 = NODE_163 + 4; const int NODE_163_05 = NODE_163 + 5; const int NODE_163_06 = NODE_163 + 6; const int NODE_163_07 = NODE_163 + 7;
        const int NODE_164 = 0x40000000 + 164 * DISCRETE_MAX_OUTPUTS; const int NODE_164_00 = NODE_164; const int NODE_164_01 = NODE_164 + 1; const int NODE_164_02 = NODE_164 + 2; const int NODE_164_03 = NODE_164 + 3; const int NODE_164_04 = NODE_164 + 4; const int NODE_164_05 = NODE_164 + 5; const int NODE_164_06 = NODE_164 + 6; const int NODE_164_07 = NODE_164 + 7;
        const int NODE_165 = 0x40000000 + 165 * DISCRETE_MAX_OUTPUTS; const int NODE_165_00 = NODE_165; const int NODE_165_01 = NODE_165 + 1; const int NODE_165_02 = NODE_165 + 2; const int NODE_165_03 = NODE_165 + 3; const int NODE_165_04 = NODE_165 + 4; const int NODE_165_05 = NODE_165 + 5; const int NODE_165_06 = NODE_165 + 6; const int NODE_165_07 = NODE_165 + 7;
        const int NODE_166 = 0x40000000 + 166 * DISCRETE_MAX_OUTPUTS; const int NODE_166_00 = NODE_166; const int NODE_166_01 = NODE_166 + 1; const int NODE_166_02 = NODE_166 + 2; const int NODE_166_03 = NODE_166 + 3; const int NODE_166_04 = NODE_166 + 4; const int NODE_166_05 = NODE_166 + 5; const int NODE_166_06 = NODE_166 + 6; const int NODE_166_07 = NODE_166 + 7;
        const int NODE_167 = 0x40000000 + 167 * DISCRETE_MAX_OUTPUTS; const int NODE_167_00 = NODE_167; const int NODE_167_01 = NODE_167 + 1; const int NODE_167_02 = NODE_167 + 2; const int NODE_167_03 = NODE_167 + 3; const int NODE_167_04 = NODE_167 + 4; const int NODE_167_05 = NODE_167 + 5; const int NODE_167_06 = NODE_167 + 6; const int NODE_167_07 = NODE_167 + 7;
        const int NODE_168 = 0x40000000 + 168 * DISCRETE_MAX_OUTPUTS; const int NODE_168_00 = NODE_168; const int NODE_168_01 = NODE_168 + 1; const int NODE_168_02 = NODE_168 + 2; const int NODE_168_03 = NODE_168 + 3; const int NODE_168_04 = NODE_168 + 4; const int NODE_168_05 = NODE_168 + 5; const int NODE_168_06 = NODE_168 + 6; const int NODE_168_07 = NODE_168 + 7;
        const int NODE_169 = 0x40000000 + 169 * DISCRETE_MAX_OUTPUTS; const int NODE_169_00 = NODE_169; const int NODE_169_01 = NODE_169 + 1; const int NODE_169_02 = NODE_169 + 2; const int NODE_169_03 = NODE_169 + 3; const int NODE_169_04 = NODE_169 + 4; const int NODE_169_05 = NODE_169 + 5; const int NODE_169_06 = NODE_169 + 6; const int NODE_169_07 = NODE_169 + 7;

        public const int NODE_170 = 0x40000000 + 170 * DISCRETE_MAX_OUTPUTS; const int NODE_170_00 = NODE_170; const int NODE_170_01 = NODE_170 + 1; const int NODE_170_02 = NODE_170 + 2; const int NODE_170_03 = NODE_170 + 3; const int NODE_170_04 = NODE_170 + 4; const int NODE_170_05 = NODE_170 + 5; const int NODE_170_06 = NODE_170 + 6; const int NODE_170_07 = NODE_170 + 7;
        public const int NODE_171 = 0x40000000 + 171 * DISCRETE_MAX_OUTPUTS; const int NODE_171_00 = NODE_171; const int NODE_171_01 = NODE_171 + 1; const int NODE_171_02 = NODE_171 + 2; const int NODE_171_03 = NODE_171 + 3; const int NODE_171_04 = NODE_171 + 4; const int NODE_171_05 = NODE_171 + 5; const int NODE_171_06 = NODE_171 + 6; const int NODE_171_07 = NODE_171 + 7;
        public const int NODE_172 = 0x40000000 + 172 * DISCRETE_MAX_OUTPUTS; const int NODE_172_00 = NODE_172; const int NODE_172_01 = NODE_172 + 1; const int NODE_172_02 = NODE_172 + 2; const int NODE_172_03 = NODE_172 + 3; const int NODE_172_04 = NODE_172 + 4; const int NODE_172_05 = NODE_172 + 5; const int NODE_172_06 = NODE_172 + 6; const int NODE_172_07 = NODE_172 + 7;
        public const int NODE_173 = 0x40000000 + 173 * DISCRETE_MAX_OUTPUTS; const int NODE_173_00 = NODE_173; const int NODE_173_01 = NODE_173 + 1; const int NODE_173_02 = NODE_173 + 2; const int NODE_173_03 = NODE_173 + 3; const int NODE_173_04 = NODE_173 + 4; const int NODE_173_05 = NODE_173 + 5; const int NODE_173_06 = NODE_173 + 6; const int NODE_173_07 = NODE_173 + 7;
        const int NODE_174 = 0x40000000 + 174 * DISCRETE_MAX_OUTPUTS; const int NODE_174_00 = NODE_174; const int NODE_174_01 = NODE_174 + 1; const int NODE_174_02 = NODE_174 + 2; const int NODE_174_03 = NODE_174 + 3; const int NODE_174_04 = NODE_174 + 4; const int NODE_174_05 = NODE_174 + 5; const int NODE_174_06 = NODE_174 + 6; const int NODE_174_07 = NODE_174 + 7;
        const int NODE_175 = 0x40000000 + 175 * DISCRETE_MAX_OUTPUTS; const int NODE_175_00 = NODE_175; const int NODE_175_01 = NODE_175 + 1; const int NODE_175_02 = NODE_175 + 2; const int NODE_175_03 = NODE_175 + 3; const int NODE_175_04 = NODE_175 + 4; const int NODE_175_05 = NODE_175 + 5; const int NODE_175_06 = NODE_175 + 6; const int NODE_175_07 = NODE_175 + 7;
        const int NODE_176 = 0x40000000 + 176 * DISCRETE_MAX_OUTPUTS; const int NODE_176_00 = NODE_176; const int NODE_176_01 = NODE_176 + 1; const int NODE_176_02 = NODE_176 + 2; const int NODE_176_03 = NODE_176 + 3; const int NODE_176_04 = NODE_176 + 4; const int NODE_176_05 = NODE_176 + 5; const int NODE_176_06 = NODE_176 + 6; const int NODE_176_07 = NODE_176 + 7;
        public const int NODE_177 = 0x40000000 + 177 * DISCRETE_MAX_OUTPUTS; const int NODE_177_00 = NODE_177; const int NODE_177_01 = NODE_177 + 1; const int NODE_177_02 = NODE_177 + 2; const int NODE_177_03 = NODE_177 + 3; const int NODE_177_04 = NODE_177 + 4; const int NODE_177_05 = NODE_177 + 5; const int NODE_177_06 = NODE_177 + 6; const int NODE_177_07 = NODE_177 + 7;
        public const int NODE_178 = 0x40000000 + 178 * DISCRETE_MAX_OUTPUTS; const int NODE_178_00 = NODE_178; const int NODE_178_01 = NODE_178 + 1; const int NODE_178_02 = NODE_178 + 2; const int NODE_178_03 = NODE_178 + 3; const int NODE_178_04 = NODE_178 + 4; const int NODE_178_05 = NODE_178 + 5; const int NODE_178_06 = NODE_178 + 6; const int NODE_178_07 = NODE_178 + 7;
        const int NODE_179 = 0x40000000 + 179 * DISCRETE_MAX_OUTPUTS; const int NODE_179_00 = NODE_179; const int NODE_179_01 = NODE_179 + 1; const int NODE_179_02 = NODE_179 + 2; const int NODE_179_03 = NODE_179 + 3; const int NODE_179_04 = NODE_179 + 4; const int NODE_179_05 = NODE_179 + 5; const int NODE_179_06 = NODE_179 + 6; const int NODE_179_07 = NODE_179 + 7;

        const int NODE_180 = 0x40000000 + 180 * DISCRETE_MAX_OUTPUTS; const int NODE_180_00 = NODE_180; const int NODE_180_01 = NODE_180 + 1; const int NODE_180_02 = NODE_180 + 2; const int NODE_180_03 = NODE_180 + 3; const int NODE_180_04 = NODE_180 + 4; const int NODE_180_05 = NODE_180 + 5; const int NODE_180_06 = NODE_180 + 6; const int NODE_180_07 = NODE_180 + 7;
        public const int NODE_181 = 0x40000000 + 181 * DISCRETE_MAX_OUTPUTS; const int NODE_181_00 = NODE_181; const int NODE_181_01 = NODE_181 + 1; const int NODE_181_02 = NODE_181 + 2; const int NODE_181_03 = NODE_181 + 3; const int NODE_181_04 = NODE_181 + 4; const int NODE_181_05 = NODE_181 + 5; const int NODE_181_06 = NODE_181 + 6; const int NODE_181_07 = NODE_181 + 7;
        public const int NODE_182 = 0x40000000 + 182 * DISCRETE_MAX_OUTPUTS; const int NODE_182_00 = NODE_182; const int NODE_182_01 = NODE_182 + 1; const int NODE_182_02 = NODE_182 + 2; const int NODE_182_03 = NODE_182 + 3; const int NODE_182_04 = NODE_182 + 4; const int NODE_182_05 = NODE_182 + 5; const int NODE_182_06 = NODE_182 + 6; const int NODE_182_07 = NODE_182 + 7;
        const int NODE_183 = 0x40000000 + 183 * DISCRETE_MAX_OUTPUTS; const int NODE_183_00 = NODE_183; const int NODE_183_01 = NODE_183 + 1; const int NODE_183_02 = NODE_183 + 2; const int NODE_183_03 = NODE_183 + 3; const int NODE_183_04 = NODE_183 + 4; const int NODE_183_05 = NODE_183 + 5; const int NODE_183_06 = NODE_183 + 6; const int NODE_183_07 = NODE_183 + 7;
        const int NODE_184 = 0x40000000 + 184 * DISCRETE_MAX_OUTPUTS; const int NODE_184_00 = NODE_184; const int NODE_184_01 = NODE_184 + 1; const int NODE_184_02 = NODE_184 + 2; const int NODE_184_03 = NODE_184 + 3; const int NODE_184_04 = NODE_184 + 4; const int NODE_184_05 = NODE_184 + 5; const int NODE_184_06 = NODE_184 + 6; const int NODE_184_07 = NODE_184 + 7;
        const int NODE_185 = 0x40000000 + 185 * DISCRETE_MAX_OUTPUTS; const int NODE_185_00 = NODE_185; const int NODE_185_01 = NODE_185 + 1; const int NODE_185_02 = NODE_185 + 2; const int NODE_185_03 = NODE_185 + 3; const int NODE_185_04 = NODE_185 + 4; const int NODE_185_05 = NODE_185 + 5; const int NODE_185_06 = NODE_185 + 6; const int NODE_185_07 = NODE_185 + 7;
        const int NODE_186 = 0x40000000 + 186 * DISCRETE_MAX_OUTPUTS; const int NODE_186_00 = NODE_186; const int NODE_186_01 = NODE_186 + 1; const int NODE_186_02 = NODE_186 + 2; const int NODE_186_03 = NODE_186 + 3; const int NODE_186_04 = NODE_186 + 4; const int NODE_186_05 = NODE_186 + 5; const int NODE_186_06 = NODE_186 + 6; const int NODE_186_07 = NODE_186 + 7;
        const int NODE_187 = 0x40000000 + 187 * DISCRETE_MAX_OUTPUTS; const int NODE_187_00 = NODE_187; const int NODE_187_01 = NODE_187 + 1; const int NODE_187_02 = NODE_187 + 2; const int NODE_187_03 = NODE_187 + 3; const int NODE_187_04 = NODE_187 + 4; const int NODE_187_05 = NODE_187 + 5; const int NODE_187_06 = NODE_187 + 6; const int NODE_187_07 = NODE_187 + 7;
        const int NODE_188 = 0x40000000 + 188 * DISCRETE_MAX_OUTPUTS; const int NODE_188_00 = NODE_188; const int NODE_188_01 = NODE_188 + 1; const int NODE_188_02 = NODE_188 + 2; const int NODE_188_03 = NODE_188 + 3; const int NODE_188_04 = NODE_188 + 4; const int NODE_188_05 = NODE_188 + 5; const int NODE_188_06 = NODE_188 + 6; const int NODE_188_07 = NODE_188 + 7;
        const int NODE_189 = 0x40000000 + 189 * DISCRETE_MAX_OUTPUTS; const int NODE_189_00 = NODE_189; const int NODE_189_01 = NODE_189 + 1; const int NODE_189_02 = NODE_189 + 2; const int NODE_189_03 = NODE_189 + 3; const int NODE_189_04 = NODE_189 + 4; const int NODE_189_05 = NODE_189 + 5; const int NODE_189_06 = NODE_189 + 6; const int NODE_189_07 = NODE_189 + 7;

        const int NODE_190 = 0x40000000 + 190 * DISCRETE_MAX_OUTPUTS; const int NODE_190_00 = NODE_190; const int NODE_190_01 = NODE_190 + 1; const int NODE_190_02 = NODE_190 + 2; const int NODE_190_03 = NODE_190 + 3; const int NODE_190_04 = NODE_190 + 4; const int NODE_190_05 = NODE_190 + 5; const int NODE_190_06 = NODE_190 + 6; const int NODE_190_07 = NODE_190 + 7;
        const int NODE_191 = 0x40000000 + 191 * DISCRETE_MAX_OUTPUTS; const int NODE_191_00 = NODE_191; const int NODE_191_01 = NODE_191 + 1; const int NODE_191_02 = NODE_191 + 2; const int NODE_191_03 = NODE_191 + 3; const int NODE_191_04 = NODE_191 + 4; const int NODE_191_05 = NODE_191 + 5; const int NODE_191_06 = NODE_191 + 6; const int NODE_191_07 = NODE_191 + 7;
        const int NODE_192 = 0x40000000 + 192 * DISCRETE_MAX_OUTPUTS; const int NODE_192_00 = NODE_192; const int NODE_192_01 = NODE_192 + 1; const int NODE_192_02 = NODE_192 + 2; const int NODE_192_03 = NODE_192 + 3; const int NODE_192_04 = NODE_192 + 4; const int NODE_192_05 = NODE_192 + 5; const int NODE_192_06 = NODE_192 + 6; const int NODE_192_07 = NODE_192 + 7;
        const int NODE_193 = 0x40000000 + 193 * DISCRETE_MAX_OUTPUTS; const int NODE_193_00 = NODE_193; const int NODE_193_01 = NODE_193 + 1; const int NODE_193_02 = NODE_193 + 2; const int NODE_193_03 = NODE_193 + 3; const int NODE_193_04 = NODE_193 + 4; const int NODE_193_05 = NODE_193 + 5; const int NODE_193_06 = NODE_193 + 6; const int NODE_193_07 = NODE_193 + 7;
        const int NODE_194 = 0x40000000 + 194 * DISCRETE_MAX_OUTPUTS; const int NODE_194_00 = NODE_194; const int NODE_194_01 = NODE_194 + 1; const int NODE_194_02 = NODE_194 + 2; const int NODE_194_03 = NODE_194 + 3; const int NODE_194_04 = NODE_194 + 4; const int NODE_194_05 = NODE_194 + 5; const int NODE_194_06 = NODE_194 + 6; const int NODE_194_07 = NODE_194 + 7;
        const int NODE_195 = 0x40000000 + 195 * DISCRETE_MAX_OUTPUTS; const int NODE_195_00 = NODE_195; const int NODE_195_01 = NODE_195 + 1; const int NODE_195_02 = NODE_195 + 2; const int NODE_195_03 = NODE_195 + 3; const int NODE_195_04 = NODE_195 + 4; const int NODE_195_05 = NODE_195 + 5; const int NODE_195_06 = NODE_195 + 6; const int NODE_195_07 = NODE_195 + 7;
        const int NODE_196 = 0x40000000 + 196 * DISCRETE_MAX_OUTPUTS; const int NODE_196_00 = NODE_196; const int NODE_196_01 = NODE_196 + 1; const int NODE_196_02 = NODE_196 + 2; const int NODE_196_03 = NODE_196 + 3; const int NODE_196_04 = NODE_196 + 4; const int NODE_196_05 = NODE_196 + 5; const int NODE_196_06 = NODE_196 + 6; const int NODE_196_07 = NODE_196 + 7;
        const int NODE_197 = 0x40000000 + 197 * DISCRETE_MAX_OUTPUTS; const int NODE_197_00 = NODE_197; const int NODE_197_01 = NODE_197 + 1; const int NODE_197_02 = NODE_197 + 2; const int NODE_197_03 = NODE_197 + 3; const int NODE_197_04 = NODE_197 + 4; const int NODE_197_05 = NODE_197 + 5; const int NODE_197_06 = NODE_197 + 6; const int NODE_197_07 = NODE_197 + 7;
        const int NODE_198 = 0x40000000 + 198 * DISCRETE_MAX_OUTPUTS; const int NODE_198_00 = NODE_198; const int NODE_198_01 = NODE_198 + 1; const int NODE_198_02 = NODE_198 + 2; const int NODE_198_03 = NODE_198 + 3; const int NODE_198_04 = NODE_198 + 4; const int NODE_198_05 = NODE_198 + 5; const int NODE_198_06 = NODE_198 + 6; const int NODE_198_07 = NODE_198 + 7;
        const int NODE_199 = 0x40000000 + 199 * DISCRETE_MAX_OUTPUTS; const int NODE_199_00 = NODE_199; const int NODE_199_01 = NODE_199 + 1; const int NODE_199_02 = NODE_199 + 2; const int NODE_199_03 = NODE_199 + 3; const int NODE_199_04 = NODE_199 + 4; const int NODE_199_05 = NODE_199 + 5; const int NODE_199_06 = NODE_199 + 6; const int NODE_199_07 = NODE_199 + 7;

        const int NODE_200 = 0x40000000 + 200 * DISCRETE_MAX_OUTPUTS; const int NODE_200_00 = NODE_200; const int NODE_200_01 = NODE_200 + 1; const int NODE_200_02 = NODE_200 + 2; const int NODE_200_03 = NODE_200 + 3; const int NODE_200_04 = NODE_200 + 4; const int NODE_200_05 = NODE_200 + 5; const int NODE_200_06 = NODE_200 + 6; const int NODE_200_07 = NODE_200 + 7;
        const int NODE_201 = 0x40000000 + 201 * DISCRETE_MAX_OUTPUTS; const int NODE_201_00 = NODE_201; const int NODE_201_01 = NODE_201 + 1; const int NODE_201_02 = NODE_201 + 2; const int NODE_201_03 = NODE_201 + 3; const int NODE_201_04 = NODE_201 + 4; const int NODE_201_05 = NODE_201 + 5; const int NODE_201_06 = NODE_201 + 6; const int NODE_201_07 = NODE_201 + 7;
        const int NODE_202 = 0x40000000 + 202 * DISCRETE_MAX_OUTPUTS; const int NODE_202_00 = NODE_202; const int NODE_202_01 = NODE_202 + 1; const int NODE_202_02 = NODE_202 + 2; const int NODE_202_03 = NODE_202 + 3; const int NODE_202_04 = NODE_202 + 4; const int NODE_202_05 = NODE_202 + 5; const int NODE_202_06 = NODE_202 + 6; const int NODE_202_07 = NODE_202 + 7;
        const int NODE_203 = 0x40000000 + 203 * DISCRETE_MAX_OUTPUTS; const int NODE_203_00 = NODE_203; const int NODE_203_01 = NODE_203 + 1; const int NODE_203_02 = NODE_203 + 2; const int NODE_203_03 = NODE_203 + 3; const int NODE_203_04 = NODE_203 + 4; const int NODE_203_05 = NODE_203 + 5; const int NODE_203_06 = NODE_203 + 6; const int NODE_203_07 = NODE_203 + 7;
        const int NODE_204 = 0x40000000 + 204 * DISCRETE_MAX_OUTPUTS; const int NODE_204_00 = NODE_204; const int NODE_204_01 = NODE_204 + 1; const int NODE_204_02 = NODE_204 + 2; const int NODE_204_03 = NODE_204 + 3; const int NODE_204_04 = NODE_204 + 4; const int NODE_204_05 = NODE_204 + 5; const int NODE_204_06 = NODE_204 + 6; const int NODE_204_07 = NODE_204 + 7;
        const int NODE_205 = 0x40000000 + 205 * DISCRETE_MAX_OUTPUTS; const int NODE_205_00 = NODE_205; const int NODE_205_01 = NODE_205 + 1; const int NODE_205_02 = NODE_205 + 2; const int NODE_205_03 = NODE_205 + 3; const int NODE_205_04 = NODE_205 + 4; const int NODE_205_05 = NODE_205 + 5; const int NODE_205_06 = NODE_205 + 6; const int NODE_205_07 = NODE_205 + 7;
        const int NODE_206 = 0x40000000 + 206 * DISCRETE_MAX_OUTPUTS; const int NODE_206_00 = NODE_206; const int NODE_206_01 = NODE_206 + 1; const int NODE_206_02 = NODE_206 + 2; const int NODE_206_03 = NODE_206 + 3; const int NODE_206_04 = NODE_206 + 4; const int NODE_206_05 = NODE_206 + 5; const int NODE_206_06 = NODE_206 + 6; const int NODE_206_07 = NODE_206 + 7;
        const int NODE_207 = 0x40000000 + 207 * DISCRETE_MAX_OUTPUTS; const int NODE_207_00 = NODE_207; const int NODE_207_01 = NODE_207 + 1; const int NODE_207_02 = NODE_207 + 2; const int NODE_207_03 = NODE_207 + 3; const int NODE_207_04 = NODE_207 + 4; const int NODE_207_05 = NODE_207 + 5; const int NODE_207_06 = NODE_207 + 6; const int NODE_207_07 = NODE_207 + 7;
        public const int NODE_208 = 0x40000000 + 208 * DISCRETE_MAX_OUTPUTS; const int NODE_208_00 = NODE_208; const int NODE_208_01 = NODE_208 + 1; const int NODE_208_02 = NODE_208 + 2; const int NODE_208_03 = NODE_208 + 3; const int NODE_208_04 = NODE_208 + 4; const int NODE_208_05 = NODE_208 + 5; const int NODE_208_06 = NODE_208 + 6; const int NODE_208_07 = NODE_208 + 7;
        public const int NODE_209 = 0x40000000 + 209 * DISCRETE_MAX_OUTPUTS; const int NODE_209_00 = NODE_209; const int NODE_209_01 = NODE_209 + 1; const int NODE_209_02 = NODE_209 + 2; const int NODE_209_03 = NODE_209 + 3; const int NODE_209_04 = NODE_209 + 4; const int NODE_209_05 = NODE_209 + 5; const int NODE_209_06 = NODE_209 + 6; const int NODE_209_07 = NODE_209 + 7;

        const int NODE_210 = 0x40000000 + 210 * DISCRETE_MAX_OUTPUTS; const int NODE_210_00 = NODE_210; const int NODE_210_01 = NODE_210 + 1; const int NODE_210_02 = NODE_210 + 2; const int NODE_210_03 = NODE_210 + 3; const int NODE_210_04 = NODE_210 + 4; const int NODE_210_05 = NODE_210 + 5; const int NODE_210_06 = NODE_210 + 6; const int NODE_210_07 = NODE_210 + 7;
        const int NODE_211 = 0x40000000 + 211 * DISCRETE_MAX_OUTPUTS; const int NODE_211_00 = NODE_211; const int NODE_211_01 = NODE_211 + 1; const int NODE_211_02 = NODE_211 + 2; const int NODE_211_03 = NODE_211 + 3; const int NODE_211_04 = NODE_211 + 4; const int NODE_211_05 = NODE_211 + 5; const int NODE_211_06 = NODE_211 + 6; const int NODE_211_07 = NODE_211 + 7;
        const int NODE_212 = 0x40000000 + 212 * DISCRETE_MAX_OUTPUTS; const int NODE_212_00 = NODE_212; const int NODE_212_01 = NODE_212 + 1; const int NODE_212_02 = NODE_212 + 2; const int NODE_212_03 = NODE_212 + 3; const int NODE_212_04 = NODE_212 + 4; const int NODE_212_05 = NODE_212 + 5; const int NODE_212_06 = NODE_212 + 6; const int NODE_212_07 = NODE_212 + 7;
        const int NODE_213 = 0x40000000 + 213 * DISCRETE_MAX_OUTPUTS; const int NODE_213_00 = NODE_213; const int NODE_213_01 = NODE_213 + 1; const int NODE_213_02 = NODE_213 + 2; const int NODE_213_03 = NODE_213 + 3; const int NODE_213_04 = NODE_213 + 4; const int NODE_213_05 = NODE_213 + 5; const int NODE_213_06 = NODE_213 + 6; const int NODE_213_07 = NODE_213 + 7;
        const int NODE_214 = 0x40000000 + 214 * DISCRETE_MAX_OUTPUTS; const int NODE_214_00 = NODE_214; const int NODE_214_01 = NODE_214 + 1; const int NODE_214_02 = NODE_214 + 2; const int NODE_214_03 = NODE_214 + 3; const int NODE_214_04 = NODE_214 + 4; const int NODE_214_05 = NODE_214 + 5; const int NODE_214_06 = NODE_214 + 6; const int NODE_214_07 = NODE_214 + 7;
        const int NODE_215 = 0x40000000 + 215 * DISCRETE_MAX_OUTPUTS; const int NODE_215_00 = NODE_215; const int NODE_215_01 = NODE_215 + 1; const int NODE_215_02 = NODE_215 + 2; const int NODE_215_03 = NODE_215 + 3; const int NODE_215_04 = NODE_215 + 4; const int NODE_215_05 = NODE_215 + 5; const int NODE_215_06 = NODE_215 + 6; const int NODE_215_07 = NODE_215 + 7;
        const int NODE_216 = 0x40000000 + 216 * DISCRETE_MAX_OUTPUTS; const int NODE_216_00 = NODE_216; const int NODE_216_01 = NODE_216 + 1; const int NODE_216_02 = NODE_216 + 2; const int NODE_216_03 = NODE_216 + 3; const int NODE_216_04 = NODE_216 + 4; const int NODE_216_05 = NODE_216 + 5; const int NODE_216_06 = NODE_216 + 6; const int NODE_216_07 = NODE_216 + 7;
        const int NODE_217 = 0x40000000 + 217 * DISCRETE_MAX_OUTPUTS; const int NODE_217_00 = NODE_217; const int NODE_217_01 = NODE_217 + 1; const int NODE_217_02 = NODE_217 + 2; const int NODE_217_03 = NODE_217 + 3; const int NODE_217_04 = NODE_217 + 4; const int NODE_217_05 = NODE_217 + 5; const int NODE_217_06 = NODE_217 + 6; const int NODE_217_07 = NODE_217 + 7;
        const int NODE_218 = 0x40000000 + 218 * DISCRETE_MAX_OUTPUTS; const int NODE_218_00 = NODE_218; const int NODE_218_01 = NODE_218 + 1; const int NODE_218_02 = NODE_218 + 2; const int NODE_218_03 = NODE_218 + 3; const int NODE_218_04 = NODE_218 + 4; const int NODE_218_05 = NODE_218 + 5; const int NODE_218_06 = NODE_218 + 6; const int NODE_218_07 = NODE_218 + 7;
        const int NODE_219 = 0x40000000 + 219 * DISCRETE_MAX_OUTPUTS; const int NODE_219_00 = NODE_219; const int NODE_219_01 = NODE_219 + 1; const int NODE_219_02 = NODE_219 + 2; const int NODE_219_03 = NODE_219 + 3; const int NODE_219_04 = NODE_219 + 4; const int NODE_219_05 = NODE_219 + 5; const int NODE_219_06 = NODE_219 + 6; const int NODE_219_07 = NODE_219 + 7;

        const int NODE_220 = 0x40000000 + 220 * DISCRETE_MAX_OUTPUTS; const int NODE_220_00 = NODE_220; const int NODE_220_01 = NODE_220 + 1; const int NODE_220_02 = NODE_220 + 2; const int NODE_220_03 = NODE_220 + 3; const int NODE_220_04 = NODE_220 + 4; const int NODE_220_05 = NODE_220 + 5; const int NODE_220_06 = NODE_220 + 6; const int NODE_220_07 = NODE_220 + 7;
        const int NODE_221 = 0x40000000 + 221 * DISCRETE_MAX_OUTPUTS; const int NODE_221_00 = NODE_221; const int NODE_221_01 = NODE_221 + 1; const int NODE_221_02 = NODE_221 + 2; const int NODE_221_03 = NODE_221 + 3; const int NODE_221_04 = NODE_221 + 4; const int NODE_221_05 = NODE_221 + 5; const int NODE_221_06 = NODE_221 + 6; const int NODE_221_07 = NODE_221 + 7;
        const int NODE_222 = 0x40000000 + 222 * DISCRETE_MAX_OUTPUTS; const int NODE_222_00 = NODE_222; const int NODE_222_01 = NODE_222 + 1; const int NODE_222_02 = NODE_222 + 2; const int NODE_222_03 = NODE_222 + 3; const int NODE_222_04 = NODE_222 + 4; const int NODE_222_05 = NODE_222 + 5; const int NODE_222_06 = NODE_222 + 6; const int NODE_222_07 = NODE_222 + 7;
        const int NODE_223 = 0x40000000 + 223 * DISCRETE_MAX_OUTPUTS; const int NODE_223_00 = NODE_223; const int NODE_223_01 = NODE_223 + 1; const int NODE_223_02 = NODE_223 + 2; const int NODE_223_03 = NODE_223 + 3; const int NODE_223_04 = NODE_223 + 4; const int NODE_223_05 = NODE_223 + 5; const int NODE_223_06 = NODE_223 + 6; const int NODE_223_07 = NODE_223 + 7;
        const int NODE_224 = 0x40000000 + 224 * DISCRETE_MAX_OUTPUTS; const int NODE_224_00 = NODE_224; const int NODE_224_01 = NODE_224 + 1; const int NODE_224_02 = NODE_224 + 2; const int NODE_224_03 = NODE_224 + 3; const int NODE_224_04 = NODE_224 + 4; const int NODE_224_05 = NODE_224 + 5; const int NODE_224_06 = NODE_224 + 6; const int NODE_224_07 = NODE_224 + 7;
        const int NODE_225 = 0x40000000 + 225 * DISCRETE_MAX_OUTPUTS; const int NODE_225_00 = NODE_225; const int NODE_225_01 = NODE_225 + 1; const int NODE_225_02 = NODE_225 + 2; const int NODE_225_03 = NODE_225 + 3; const int NODE_225_04 = NODE_225 + 4; const int NODE_225_05 = NODE_225 + 5; const int NODE_225_06 = NODE_225 + 6; const int NODE_225_07 = NODE_225 + 7;
        const int NODE_226 = 0x40000000 + 226 * DISCRETE_MAX_OUTPUTS; const int NODE_226_00 = NODE_226; const int NODE_226_01 = NODE_226 + 1; const int NODE_226_02 = NODE_226 + 2; const int NODE_226_03 = NODE_226 + 3; const int NODE_226_04 = NODE_226 + 4; const int NODE_226_05 = NODE_226 + 5; const int NODE_226_06 = NODE_226 + 6; const int NODE_226_07 = NODE_226 + 7;
        const int NODE_227 = 0x40000000 + 227 * DISCRETE_MAX_OUTPUTS; const int NODE_227_00 = NODE_227; const int NODE_227_01 = NODE_227 + 1; const int NODE_227_02 = NODE_227 + 2; const int NODE_227_03 = NODE_227 + 3; const int NODE_227_04 = NODE_227 + 4; const int NODE_227_05 = NODE_227 + 5; const int NODE_227_06 = NODE_227 + 6; const int NODE_227_07 = NODE_227 + 7;
        const int NODE_228 = 0x40000000 + 228 * DISCRETE_MAX_OUTPUTS; const int NODE_228_00 = NODE_228; const int NODE_228_01 = NODE_228 + 1; const int NODE_228_02 = NODE_228 + 2; const int NODE_228_03 = NODE_228 + 3; const int NODE_228_04 = NODE_228 + 4; const int NODE_228_05 = NODE_228 + 5; const int NODE_228_06 = NODE_228 + 6; const int NODE_228_07 = NODE_228 + 7;
        const int NODE_229 = 0x40000000 + 229 * DISCRETE_MAX_OUTPUTS; const int NODE_229_00 = NODE_229; const int NODE_229_01 = NODE_229 + 1; const int NODE_229_02 = NODE_229 + 2; const int NODE_229_03 = NODE_229 + 3; const int NODE_229_04 = NODE_229 + 4; const int NODE_229_05 = NODE_229 + 5; const int NODE_229_06 = NODE_229 + 6; const int NODE_229_07 = NODE_229 + 7;

        const int NODE_230 = 0x40000000 + 230 * DISCRETE_MAX_OUTPUTS; const int NODE_230_00 = NODE_230; const int NODE_230_01 = NODE_230 + 1; const int NODE_230_02 = NODE_230 + 2; const int NODE_230_03 = NODE_230 + 3; const int NODE_230_04 = NODE_230 + 4; const int NODE_230_05 = NODE_230 + 5; const int NODE_230_06 = NODE_230 + 6; const int NODE_230_07 = NODE_230 + 7;
        const int NODE_231 = 0x40000000 + 231 * DISCRETE_MAX_OUTPUTS; const int NODE_231_00 = NODE_231; const int NODE_231_01 = NODE_231 + 1; const int NODE_231_02 = NODE_231 + 2; const int NODE_231_03 = NODE_231 + 3; const int NODE_231_04 = NODE_231 + 4; const int NODE_231_05 = NODE_231 + 5; const int NODE_231_06 = NODE_231 + 6; const int NODE_231_07 = NODE_231 + 7;
        const int NODE_232 = 0x40000000 + 232 * DISCRETE_MAX_OUTPUTS; const int NODE_232_00 = NODE_232; const int NODE_232_01 = NODE_232 + 1; const int NODE_232_02 = NODE_232 + 2; const int NODE_232_03 = NODE_232 + 3; const int NODE_232_04 = NODE_232 + 4; const int NODE_232_05 = NODE_232 + 5; const int NODE_232_06 = NODE_232 + 6; const int NODE_232_07 = NODE_232 + 7;
        const int NODE_233 = 0x40000000 + 233 * DISCRETE_MAX_OUTPUTS; const int NODE_233_00 = NODE_233; const int NODE_233_01 = NODE_233 + 1; const int NODE_233_02 = NODE_233 + 2; const int NODE_233_03 = NODE_233 + 3; const int NODE_233_04 = NODE_233 + 4; const int NODE_233_05 = NODE_233 + 5; const int NODE_233_06 = NODE_233 + 6; const int NODE_233_07 = NODE_233 + 7;
        const int NODE_234 = 0x40000000 + 234 * DISCRETE_MAX_OUTPUTS; const int NODE_234_00 = NODE_234; const int NODE_234_01 = NODE_234 + 1; const int NODE_234_02 = NODE_234 + 2; const int NODE_234_03 = NODE_234 + 3; const int NODE_234_04 = NODE_234 + 4; const int NODE_234_05 = NODE_234 + 5; const int NODE_234_06 = NODE_234 + 6; const int NODE_234_07 = NODE_234 + 7;
        const int NODE_235 = 0x40000000 + 235 * DISCRETE_MAX_OUTPUTS; const int NODE_235_00 = NODE_235; const int NODE_235_01 = NODE_235 + 1; const int NODE_235_02 = NODE_235 + 2; const int NODE_235_03 = NODE_235 + 3; const int NODE_235_04 = NODE_235 + 4; const int NODE_235_05 = NODE_235 + 5; const int NODE_235_06 = NODE_235 + 6; const int NODE_235_07 = NODE_235 + 7;
        const int NODE_236 = 0x40000000 + 236 * DISCRETE_MAX_OUTPUTS; const int NODE_236_00 = NODE_236; const int NODE_236_01 = NODE_236 + 1; const int NODE_236_02 = NODE_236 + 2; const int NODE_236_03 = NODE_236 + 3; const int NODE_236_04 = NODE_236 + 4; const int NODE_236_05 = NODE_236 + 5; const int NODE_236_06 = NODE_236 + 6; const int NODE_236_07 = NODE_236 + 7;
        const int NODE_237 = 0x40000000 + 237 * DISCRETE_MAX_OUTPUTS; const int NODE_237_00 = NODE_237; const int NODE_237_01 = NODE_237 + 1; const int NODE_237_02 = NODE_237 + 2; const int NODE_237_03 = NODE_237 + 3; const int NODE_237_04 = NODE_237 + 4; const int NODE_237_05 = NODE_237 + 5; const int NODE_237_06 = NODE_237 + 6; const int NODE_237_07 = NODE_237 + 7;
        const int NODE_238 = 0x40000000 + 238 * DISCRETE_MAX_OUTPUTS; const int NODE_238_00 = NODE_238; const int NODE_238_01 = NODE_238 + 1; const int NODE_238_02 = NODE_238 + 2; const int NODE_238_03 = NODE_238 + 3; const int NODE_238_04 = NODE_238 + 4; const int NODE_238_05 = NODE_238 + 5; const int NODE_238_06 = NODE_238 + 6; const int NODE_238_07 = NODE_238 + 7;
        const int NODE_239 = 0x40000000 + 239 * DISCRETE_MAX_OUTPUTS; const int NODE_239_00 = NODE_239; const int NODE_239_01 = NODE_239 + 1; const int NODE_239_02 = NODE_239 + 2; const int NODE_239_03 = NODE_239 + 3; const int NODE_239_04 = NODE_239 + 4; const int NODE_239_05 = NODE_239 + 5; const int NODE_239_06 = NODE_239 + 6; const int NODE_239_07 = NODE_239 + 7;

        public const int NODE_240 = 0x40000000 + 240 * DISCRETE_MAX_OUTPUTS; const int NODE_240_00 = NODE_240; const int NODE_240_01 = NODE_240 + 1; const int NODE_240_02 = NODE_240 + 2; const int NODE_240_03 = NODE_240 + 3; const int NODE_240_04 = NODE_240 + 4; const int NODE_240_05 = NODE_240 + 5; const int NODE_240_06 = NODE_240 + 6; const int NODE_240_07 = NODE_240 + 7;
        public const int NODE_241 = 0x40000000 + 241 * DISCRETE_MAX_OUTPUTS; const int NODE_241_00 = NODE_241; const int NODE_241_01 = NODE_241 + 1; const int NODE_241_02 = NODE_241 + 2; const int NODE_241_03 = NODE_241 + 3; const int NODE_241_04 = NODE_241 + 4; const int NODE_241_05 = NODE_241 + 5; const int NODE_241_06 = NODE_241 + 6; const int NODE_241_07 = NODE_241 + 7;
        public const int NODE_242 = 0x40000000 + 242 * DISCRETE_MAX_OUTPUTS; const int NODE_242_00 = NODE_242; const int NODE_242_01 = NODE_242 + 1; const int NODE_242_02 = NODE_242 + 2; const int NODE_242_03 = NODE_242 + 3; const int NODE_242_04 = NODE_242 + 4; const int NODE_242_05 = NODE_242 + 5; const int NODE_242_06 = NODE_242 + 6; const int NODE_242_07 = NODE_242 + 7;
        public const int NODE_243 = 0x40000000 + 243 * DISCRETE_MAX_OUTPUTS; const int NODE_243_00 = NODE_243; const int NODE_243_01 = NODE_243 + 1; const int NODE_243_02 = NODE_243 + 2; const int NODE_243_03 = NODE_243 + 3; const int NODE_243_04 = NODE_243 + 4; const int NODE_243_05 = NODE_243 + 5; const int NODE_243_06 = NODE_243 + 6; const int NODE_243_07 = NODE_243 + 7;
        const int NODE_244 = 0x40000000 + 244 * DISCRETE_MAX_OUTPUTS; const int NODE_244_00 = NODE_244; const int NODE_244_01 = NODE_244 + 1; const int NODE_244_02 = NODE_244 + 2; const int NODE_244_03 = NODE_244 + 3; const int NODE_244_04 = NODE_244 + 4; const int NODE_244_05 = NODE_244 + 5; const int NODE_244_06 = NODE_244 + 6; const int NODE_244_07 = NODE_244 + 7;
        const int NODE_245 = 0x40000000 + 245 * DISCRETE_MAX_OUTPUTS; const int NODE_245_00 = NODE_245; const int NODE_245_01 = NODE_245 + 1; const int NODE_245_02 = NODE_245 + 2; const int NODE_245_03 = NODE_245 + 3; const int NODE_245_04 = NODE_245 + 4; const int NODE_245_05 = NODE_245 + 5; const int NODE_245_06 = NODE_245 + 6; const int NODE_245_07 = NODE_245 + 7;
        const int NODE_246 = 0x40000000 + 246 * DISCRETE_MAX_OUTPUTS; const int NODE_246_00 = NODE_246; const int NODE_246_01 = NODE_246 + 1; const int NODE_246_02 = NODE_246 + 2; const int NODE_246_03 = NODE_246 + 3; const int NODE_246_04 = NODE_246 + 4; const int NODE_246_05 = NODE_246 + 5; const int NODE_246_06 = NODE_246 + 6; const int NODE_246_07 = NODE_246 + 7;
        const int NODE_247 = 0x40000000 + 247 * DISCRETE_MAX_OUTPUTS; const int NODE_247_00 = NODE_247; const int NODE_247_01 = NODE_247 + 1; const int NODE_247_02 = NODE_247 + 2; const int NODE_247_03 = NODE_247 + 3; const int NODE_247_04 = NODE_247 + 4; const int NODE_247_05 = NODE_247 + 5; const int NODE_247_06 = NODE_247 + 6; const int NODE_247_07 = NODE_247 + 7;
        const int NODE_248 = 0x40000000 + 248 * DISCRETE_MAX_OUTPUTS; const int NODE_248_00 = NODE_248; const int NODE_248_01 = NODE_248 + 1; const int NODE_248_02 = NODE_248 + 2; const int NODE_248_03 = NODE_248 + 3; const int NODE_248_04 = NODE_248 + 4; const int NODE_248_05 = NODE_248 + 5; const int NODE_248_06 = NODE_248 + 6; const int NODE_248_07 = NODE_248 + 7;
        const int NODE_249 = 0x40000000 + 249 * DISCRETE_MAX_OUTPUTS; const int NODE_249_00 = NODE_249; const int NODE_249_01 = NODE_249 + 1; const int NODE_249_02 = NODE_249 + 2; const int NODE_249_03 = NODE_249 + 3; const int NODE_249_04 = NODE_249 + 4; const int NODE_249_05 = NODE_249 + 5; const int NODE_249_06 = NODE_249 + 6; const int NODE_249_07 = NODE_249 + 7;

        public const int NODE_250 = 0x40000000 + 250 * DISCRETE_MAX_OUTPUTS; const int NODE_250_00 = NODE_250; const int NODE_250_01 = NODE_250 + 1; const int NODE_250_02 = NODE_250 + 2; const int NODE_250_03 = NODE_250 + 3; const int NODE_250_04 = NODE_250 + 4; const int NODE_250_05 = NODE_250 + 5; const int NODE_250_06 = NODE_250 + 6; const int NODE_250_07 = NODE_250 + 7;
        const int NODE_251 = 0x40000000 + 251 * DISCRETE_MAX_OUTPUTS; const int NODE_251_00 = NODE_251; const int NODE_251_01 = NODE_251 + 1; const int NODE_251_02 = NODE_251 + 2; const int NODE_251_03 = NODE_251 + 3; const int NODE_251_04 = NODE_251 + 4; const int NODE_251_05 = NODE_251 + 5; const int NODE_251_06 = NODE_251 + 6; const int NODE_251_07 = NODE_251 + 7;
        const int NODE_252 = 0x40000000 + 252 * DISCRETE_MAX_OUTPUTS; const int NODE_252_00 = NODE_252; const int NODE_252_01 = NODE_252 + 1; const int NODE_252_02 = NODE_252 + 2; const int NODE_252_03 = NODE_252 + 3; const int NODE_252_04 = NODE_252 + 4; const int NODE_252_05 = NODE_252 + 5; const int NODE_252_06 = NODE_252 + 6; const int NODE_252_07 = NODE_252 + 7;
        const int NODE_253 = 0x40000000 + 253 * DISCRETE_MAX_OUTPUTS; const int NODE_253_00 = NODE_253; const int NODE_253_01 = NODE_253 + 1; const int NODE_253_02 = NODE_253 + 2; const int NODE_253_03 = NODE_253 + 3; const int NODE_253_04 = NODE_253 + 4; const int NODE_253_05 = NODE_253 + 5; const int NODE_253_06 = NODE_253 + 6; const int NODE_253_07 = NODE_253 + 7;
        const int NODE_254 = 0x40000000 + 254 * DISCRETE_MAX_OUTPUTS; const int NODE_254_00 = NODE_254; const int NODE_254_01 = NODE_254 + 1; const int NODE_254_02 = NODE_254 + 2; const int NODE_254_03 = NODE_254 + 3; const int NODE_254_04 = NODE_254 + 4; const int NODE_254_05 = NODE_254 + 5; const int NODE_254_06 = NODE_254 + 6; const int NODE_254_07 = NODE_254 + 7;
        const int NODE_255 = 0x40000000 + 255 * DISCRETE_MAX_OUTPUTS; const int NODE_255_00 = NODE_255; const int NODE_255_01 = NODE_255 + 1; const int NODE_255_02 = NODE_255 + 2; const int NODE_255_03 = NODE_255 + 3; const int NODE_255_04 = NODE_255 + 4; const int NODE_255_05 = NODE_255 + 5; const int NODE_255_06 = NODE_255 + 6; const int NODE_255_07 = NODE_255 + 7;
        const int NODE_256 = 0x40000000 + 256 * DISCRETE_MAX_OUTPUTS; const int NODE_256_00 = NODE_256; const int NODE_256_01 = NODE_256 + 1; const int NODE_256_02 = NODE_256 + 2; const int NODE_256_03 = NODE_256 + 3; const int NODE_256_04 = NODE_256 + 4; const int NODE_256_05 = NODE_256 + 5; const int NODE_256_06 = NODE_256 + 6; const int NODE_256_07 = NODE_256 + 7;
        const int NODE_257 = 0x40000000 + 257 * DISCRETE_MAX_OUTPUTS; const int NODE_257_00 = NODE_257; const int NODE_257_01 = NODE_257 + 1; const int NODE_257_02 = NODE_257 + 2; const int NODE_257_03 = NODE_257 + 3; const int NODE_257_04 = NODE_257 + 4; const int NODE_257_05 = NODE_257 + 5; const int NODE_257_06 = NODE_257 + 6; const int NODE_257_07 = NODE_257 + 7;
        const int NODE_258 = 0x40000000 + 258 * DISCRETE_MAX_OUTPUTS; const int NODE_258_00 = NODE_258; const int NODE_258_01 = NODE_258 + 1; const int NODE_258_02 = NODE_258 + 2; const int NODE_258_03 = NODE_258 + 3; const int NODE_258_04 = NODE_258 + 4; const int NODE_258_05 = NODE_258 + 5; const int NODE_258_06 = NODE_258 + 6; const int NODE_258_07 = NODE_258 + 7;
        const int NODE_259 = 0x40000000 + 259 * DISCRETE_MAX_OUTPUTS; const int NODE_259_00 = NODE_259; const int NODE_259_01 = NODE_259 + 1; const int NODE_259_02 = NODE_259 + 2; const int NODE_259_03 = NODE_259 + 3; const int NODE_259_04 = NODE_259 + 4; const int NODE_259_05 = NODE_259 + 5; const int NODE_259_06 = NODE_259 + 6; const int NODE_259_07 = NODE_259 + 7;

        const int NODE_260 = 0x40000000 + 260 * DISCRETE_MAX_OUTPUTS; const int NODE_260_00 = NODE_260; const int NODE_260_01 = NODE_260 + 1; const int NODE_260_02 = NODE_260 + 2; const int NODE_260_03 = NODE_260 + 3; const int NODE_260_04 = NODE_260 + 4; const int NODE_260_05 = NODE_260 + 5; const int NODE_260_06 = NODE_260 + 6; const int NODE_260_07 = NODE_260 + 7;
        const int NODE_261 = 0x40000000 + 261 * DISCRETE_MAX_OUTPUTS; const int NODE_261_00 = NODE_261; const int NODE_261_01 = NODE_261 + 1; const int NODE_261_02 = NODE_261 + 2; const int NODE_261_03 = NODE_261 + 3; const int NODE_261_04 = NODE_261 + 4; const int NODE_261_05 = NODE_261 + 5; const int NODE_261_06 = NODE_261 + 6; const int NODE_261_07 = NODE_261 + 7;
        const int NODE_262 = 0x40000000 + 262 * DISCRETE_MAX_OUTPUTS; const int NODE_262_00 = NODE_262; const int NODE_262_01 = NODE_262 + 1; const int NODE_262_02 = NODE_262 + 2; const int NODE_262_03 = NODE_262 + 3; const int NODE_262_04 = NODE_262 + 4; const int NODE_262_05 = NODE_262 + 5; const int NODE_262_06 = NODE_262 + 6; const int NODE_262_07 = NODE_262 + 7;
        const int NODE_263 = 0x40000000 + 263 * DISCRETE_MAX_OUTPUTS; const int NODE_263_00 = NODE_263; const int NODE_263_01 = NODE_263 + 1; const int NODE_263_02 = NODE_263 + 2; const int NODE_263_03 = NODE_263 + 3; const int NODE_263_04 = NODE_263 + 4; const int NODE_263_05 = NODE_263 + 5; const int NODE_263_06 = NODE_263 + 6; const int NODE_263_07 = NODE_263 + 7;
        const int NODE_264 = 0x40000000 + 264 * DISCRETE_MAX_OUTPUTS; const int NODE_264_00 = NODE_264; const int NODE_264_01 = NODE_264 + 1; const int NODE_264_02 = NODE_264 + 2; const int NODE_264_03 = NODE_264 + 3; const int NODE_264_04 = NODE_264 + 4; const int NODE_264_05 = NODE_264 + 5; const int NODE_264_06 = NODE_264 + 6; const int NODE_264_07 = NODE_264 + 7;
        const int NODE_265 = 0x40000000 + 265 * DISCRETE_MAX_OUTPUTS; const int NODE_265_00 = NODE_265; const int NODE_265_01 = NODE_265 + 1; const int NODE_265_02 = NODE_265 + 2; const int NODE_265_03 = NODE_265 + 3; const int NODE_265_04 = NODE_265 + 4; const int NODE_265_05 = NODE_265 + 5; const int NODE_265_06 = NODE_265 + 6; const int NODE_265_07 = NODE_265 + 7;
        const int NODE_266 = 0x40000000 + 266 * DISCRETE_MAX_OUTPUTS; const int NODE_266_00 = NODE_266; const int NODE_266_01 = NODE_266 + 1; const int NODE_266_02 = NODE_266 + 2; const int NODE_266_03 = NODE_266 + 3; const int NODE_266_04 = NODE_266 + 4; const int NODE_266_05 = NODE_266 + 5; const int NODE_266_06 = NODE_266 + 6; const int NODE_266_07 = NODE_266 + 7;
        const int NODE_267 = 0x40000000 + 267 * DISCRETE_MAX_OUTPUTS; const int NODE_267_00 = NODE_267; const int NODE_267_01 = NODE_267 + 1; const int NODE_267_02 = NODE_267 + 2; const int NODE_267_03 = NODE_267 + 3; const int NODE_267_04 = NODE_267 + 4; const int NODE_267_05 = NODE_267 + 5; const int NODE_267_06 = NODE_267 + 6; const int NODE_267_07 = NODE_267 + 7;
        const int NODE_268 = 0x40000000 + 268 * DISCRETE_MAX_OUTPUTS; const int NODE_268_00 = NODE_268; const int NODE_268_01 = NODE_268 + 1; const int NODE_268_02 = NODE_268 + 2; const int NODE_268_03 = NODE_268 + 3; const int NODE_268_04 = NODE_268 + 4; const int NODE_268_05 = NODE_268 + 5; const int NODE_268_06 = NODE_268 + 6; const int NODE_268_07 = NODE_268 + 7;
        const int NODE_269 = 0x40000000 + 269 * DISCRETE_MAX_OUTPUTS; const int NODE_269_00 = NODE_269; const int NODE_269_01 = NODE_269 + 1; const int NODE_269_02 = NODE_269 + 2; const int NODE_269_03 = NODE_269 + 3; const int NODE_269_04 = NODE_269 + 4; const int NODE_269_05 = NODE_269 + 5; const int NODE_269_06 = NODE_269 + 6; const int NODE_269_07 = NODE_269 + 7;

        const int NODE_270 = 0x40000000 + 270 * DISCRETE_MAX_OUTPUTS; const int NODE_270_00 = NODE_270; const int NODE_270_01 = NODE_270 + 1; const int NODE_270_02 = NODE_270 + 2; const int NODE_270_03 = NODE_270 + 3; const int NODE_270_04 = NODE_270 + 4; const int NODE_270_05 = NODE_270 + 5; const int NODE_270_06 = NODE_270 + 6; const int NODE_270_07 = NODE_270 + 7;
        const int NODE_271 = 0x40000000 + 271 * DISCRETE_MAX_OUTPUTS; const int NODE_271_00 = NODE_271; const int NODE_271_01 = NODE_271 + 1; const int NODE_271_02 = NODE_271 + 2; const int NODE_271_03 = NODE_271 + 3; const int NODE_271_04 = NODE_271 + 4; const int NODE_271_05 = NODE_271 + 5; const int NODE_271_06 = NODE_271 + 6; const int NODE_271_07 = NODE_271 + 7;
        const int NODE_272 = 0x40000000 + 272 * DISCRETE_MAX_OUTPUTS; const int NODE_272_00 = NODE_272; const int NODE_272_01 = NODE_272 + 1; const int NODE_272_02 = NODE_272 + 2; const int NODE_272_03 = NODE_272 + 3; const int NODE_272_04 = NODE_272 + 4; const int NODE_272_05 = NODE_272 + 5; const int NODE_272_06 = NODE_272 + 6; const int NODE_272_07 = NODE_272 + 7;
        const int NODE_273 = 0x40000000 + 273 * DISCRETE_MAX_OUTPUTS; const int NODE_273_00 = NODE_273; const int NODE_273_01 = NODE_273 + 1; const int NODE_273_02 = NODE_273 + 2; const int NODE_273_03 = NODE_273 + 3; const int NODE_273_04 = NODE_273 + 4; const int NODE_273_05 = NODE_273 + 5; const int NODE_273_06 = NODE_273 + 6; const int NODE_273_07 = NODE_273 + 7;
        const int NODE_274 = 0x40000000 + 274 * DISCRETE_MAX_OUTPUTS; const int NODE_274_00 = NODE_274; const int NODE_274_01 = NODE_274 + 1; const int NODE_274_02 = NODE_274 + 2; const int NODE_274_03 = NODE_274 + 3; const int NODE_274_04 = NODE_274 + 4; const int NODE_274_05 = NODE_274 + 5; const int NODE_274_06 = NODE_274 + 6; const int NODE_274_07 = NODE_274 + 7;
        const int NODE_275 = 0x40000000 + 275 * DISCRETE_MAX_OUTPUTS; const int NODE_275_00 = NODE_275; const int NODE_275_01 = NODE_275 + 1; const int NODE_275_02 = NODE_275 + 2; const int NODE_275_03 = NODE_275 + 3; const int NODE_275_04 = NODE_275 + 4; const int NODE_275_05 = NODE_275 + 5; const int NODE_275_06 = NODE_275 + 6; const int NODE_275_07 = NODE_275 + 7;
        const int NODE_276 = 0x40000000 + 276 * DISCRETE_MAX_OUTPUTS; const int NODE_276_00 = NODE_276; const int NODE_276_01 = NODE_276 + 1; const int NODE_276_02 = NODE_276 + 2; const int NODE_276_03 = NODE_276 + 3; const int NODE_276_04 = NODE_276 + 4; const int NODE_276_05 = NODE_276 + 5; const int NODE_276_06 = NODE_276 + 6; const int NODE_276_07 = NODE_276 + 7;
        const int NODE_277 = 0x40000000 + 277 * DISCRETE_MAX_OUTPUTS; const int NODE_277_00 = NODE_277; const int NODE_277_01 = NODE_277 + 1; const int NODE_277_02 = NODE_277 + 2; const int NODE_277_03 = NODE_277 + 3; const int NODE_277_04 = NODE_277 + 4; const int NODE_277_05 = NODE_277 + 5; const int NODE_277_06 = NODE_277 + 6; const int NODE_277_07 = NODE_277 + 7;
        const int NODE_278 = 0x40000000 + 278 * DISCRETE_MAX_OUTPUTS; const int NODE_278_00 = NODE_278; const int NODE_278_01 = NODE_278 + 1; const int NODE_278_02 = NODE_278 + 2; const int NODE_278_03 = NODE_278 + 3; const int NODE_278_04 = NODE_278 + 4; const int NODE_278_05 = NODE_278 + 5; const int NODE_278_06 = NODE_278 + 6; const int NODE_278_07 = NODE_278 + 7;
        public const int NODE_279 = 0x40000000 + 279 * DISCRETE_MAX_OUTPUTS; const int NODE_279_00 = NODE_279; const int NODE_279_01 = NODE_279 + 1; const int NODE_279_02 = NODE_279 + 2; const int NODE_279_03 = NODE_279 + 3; const int NODE_279_04 = NODE_279 + 4; const int NODE_279_05 = NODE_279 + 5; const int NODE_279_06 = NODE_279 + 6; const int NODE_279_07 = NODE_279 + 7;

        public const int NODE_280 = 0x40000000 + 280 * DISCRETE_MAX_OUTPUTS; const int NODE_280_00 = NODE_280; const int NODE_280_01 = NODE_280 + 1; const int NODE_280_02 = NODE_280 + 2; const int NODE_280_03 = NODE_280 + 3; const int NODE_280_04 = NODE_280 + 4; const int NODE_280_05 = NODE_280 + 5; const int NODE_280_06 = NODE_280 + 6; const int NODE_280_07 = NODE_280 + 7;
        const int NODE_281 = 0x40000000 + 281 * DISCRETE_MAX_OUTPUTS; const int NODE_281_00 = NODE_281; const int NODE_281_01 = NODE_281 + 1; const int NODE_281_02 = NODE_281 + 2; const int NODE_281_03 = NODE_281 + 3; const int NODE_281_04 = NODE_281 + 4; const int NODE_281_05 = NODE_281 + 5; const int NODE_281_06 = NODE_281 + 6; const int NODE_281_07 = NODE_281 + 7;
        const int NODE_282 = 0x40000000 + 282 * DISCRETE_MAX_OUTPUTS; const int NODE_282_00 = NODE_282; const int NODE_282_01 = NODE_282 + 1; const int NODE_282_02 = NODE_282 + 2; const int NODE_282_03 = NODE_282 + 3; const int NODE_282_04 = NODE_282 + 4; const int NODE_282_05 = NODE_282 + 5; const int NODE_282_06 = NODE_282 + 6; const int NODE_282_07 = NODE_282 + 7;
        const int NODE_283 = 0x40000000 + 283 * DISCRETE_MAX_OUTPUTS; const int NODE_283_00 = NODE_283; const int NODE_283_01 = NODE_283 + 1; const int NODE_283_02 = NODE_283 + 2; const int NODE_283_03 = NODE_283 + 3; const int NODE_283_04 = NODE_283 + 4; const int NODE_283_05 = NODE_283 + 5; const int NODE_283_06 = NODE_283 + 6; const int NODE_283_07 = NODE_283 + 7;
        const int NODE_284 = 0x40000000 + 284 * DISCRETE_MAX_OUTPUTS; const int NODE_284_00 = NODE_284; const int NODE_284_01 = NODE_284 + 1; const int NODE_284_02 = NODE_284 + 2; const int NODE_284_03 = NODE_284 + 3; const int NODE_284_04 = NODE_284 + 4; const int NODE_284_05 = NODE_284 + 5; const int NODE_284_06 = NODE_284 + 6; const int NODE_284_07 = NODE_284 + 7;
        const int NODE_285 = 0x40000000 + 285 * DISCRETE_MAX_OUTPUTS; const int NODE_285_00 = NODE_285; const int NODE_285_01 = NODE_285 + 1; const int NODE_285_02 = NODE_285 + 2; const int NODE_285_03 = NODE_285 + 3; const int NODE_285_04 = NODE_285 + 4; const int NODE_285_05 = NODE_285 + 5; const int NODE_285_06 = NODE_285 + 6; const int NODE_285_07 = NODE_285 + 7;
        const int NODE_286 = 0x40000000 + 286 * DISCRETE_MAX_OUTPUTS; const int NODE_286_00 = NODE_286; const int NODE_286_01 = NODE_286 + 1; const int NODE_286_02 = NODE_286 + 2; const int NODE_286_03 = NODE_286 + 3; const int NODE_286_04 = NODE_286 + 4; const int NODE_286_05 = NODE_286 + 5; const int NODE_286_06 = NODE_286 + 6; const int NODE_286_07 = NODE_286 + 7;
        const int NODE_287 = 0x40000000 + 287 * DISCRETE_MAX_OUTPUTS; const int NODE_287_00 = NODE_287; const int NODE_287_01 = NODE_287 + 1; const int NODE_287_02 = NODE_287 + 2; const int NODE_287_03 = NODE_287 + 3; const int NODE_287_04 = NODE_287 + 4; const int NODE_287_05 = NODE_287 + 5; const int NODE_287_06 = NODE_287 + 6; const int NODE_287_07 = NODE_287 + 7;
        public const int NODE_288 = 0x40000000 + 288 * DISCRETE_MAX_OUTPUTS; const int NODE_288_00 = NODE_288; const int NODE_288_01 = NODE_288 + 1; const int NODE_288_02 = NODE_288 + 2; const int NODE_288_03 = NODE_288 + 3; const int NODE_288_04 = NODE_288 + 4; const int NODE_288_05 = NODE_288 + 5; const int NODE_288_06 = NODE_288 + 6; const int NODE_288_07 = NODE_288 + 7;
        public const int NODE_289 = 0x40000000 + 289 * DISCRETE_MAX_OUTPUTS; const int NODE_289_00 = NODE_289; const int NODE_289_01 = NODE_289 + 1; const int NODE_289_02 = NODE_289 + 2; const int NODE_289_03 = NODE_289 + 3; const int NODE_289_04 = NODE_289 + 4; const int NODE_289_05 = NODE_289 + 5; const int NODE_289_06 = NODE_289 + 6; const int NODE_289_07 = NODE_289 + 7;

        const int NODE_290 = 0x40000000 + 290 * DISCRETE_MAX_OUTPUTS; const int NODE_290_00 = NODE_290; const int NODE_290_01 = NODE_290 + 1; const int NODE_290_02 = NODE_290 + 2; const int NODE_290_03 = NODE_290 + 3; const int NODE_290_04 = NODE_290 + 4; const int NODE_290_05 = NODE_290 + 5; const int NODE_290_06 = NODE_290 + 6; const int NODE_290_07 = NODE_290 + 7;
        const int NODE_291 = 0x40000000 + 291 * DISCRETE_MAX_OUTPUTS; const int NODE_291_00 = NODE_291; const int NODE_291_01 = NODE_291 + 1; const int NODE_291_02 = NODE_291 + 2; const int NODE_291_03 = NODE_291 + 3; const int NODE_291_04 = NODE_291 + 4; const int NODE_291_05 = NODE_291 + 5; const int NODE_291_06 = NODE_291 + 6; const int NODE_291_07 = NODE_291 + 7;
        const int NODE_292 = 0x40000000 + 292 * DISCRETE_MAX_OUTPUTS; const int NODE_292_00 = NODE_292; const int NODE_292_01 = NODE_292 + 1; const int NODE_292_02 = NODE_292 + 2; const int NODE_292_03 = NODE_292 + 3; const int NODE_292_04 = NODE_292 + 4; const int NODE_292_05 = NODE_292 + 5; const int NODE_292_06 = NODE_292 + 6; const int NODE_292_07 = NODE_292 + 7;
        const int NODE_293 = 0x40000000 + 293 * DISCRETE_MAX_OUTPUTS; const int NODE_293_00 = NODE_293; const int NODE_293_01 = NODE_293 + 1; const int NODE_293_02 = NODE_293 + 2; const int NODE_293_03 = NODE_293 + 3; const int NODE_293_04 = NODE_293 + 4; const int NODE_293_05 = NODE_293 + 5; const int NODE_293_06 = NODE_293 + 6; const int NODE_293_07 = NODE_293 + 7;
        public const int NODE_294 = 0x40000000 + 294 * DISCRETE_MAX_OUTPUTS; const int NODE_294_00 = NODE_294; const int NODE_294_01 = NODE_294 + 1; const int NODE_294_02 = NODE_294 + 2; const int NODE_294_03 = NODE_294 + 3; const int NODE_294_04 = NODE_294 + 4; const int NODE_294_05 = NODE_294 + 5; const int NODE_294_06 = NODE_294 + 6; const int NODE_294_07 = NODE_294 + 7;
        public const int NODE_295 = 0x40000000 + 295 * DISCRETE_MAX_OUTPUTS; const int NODE_295_00 = NODE_295; const int NODE_295_01 = NODE_295 + 1; const int NODE_295_02 = NODE_295 + 2; const int NODE_295_03 = NODE_295 + 3; const int NODE_295_04 = NODE_295 + 4; const int NODE_295_05 = NODE_295 + 5; const int NODE_295_06 = NODE_295 + 6; const int NODE_295_07 = NODE_295 + 7;
        public const int NODE_296 = 0x40000000 + 296 * DISCRETE_MAX_OUTPUTS; const int NODE_296_00 = NODE_296; const int NODE_296_01 = NODE_296 + 1; const int NODE_296_02 = NODE_296 + 2; const int NODE_296_03 = NODE_296 + 3; const int NODE_296_04 = NODE_296 + 4; const int NODE_296_05 = NODE_296 + 5; const int NODE_296_06 = NODE_296 + 6; const int NODE_296_07 = NODE_296 + 7;
        const int NODE_297 = 0x40000000 + 297 * DISCRETE_MAX_OUTPUTS; const int NODE_297_00 = NODE_297; const int NODE_297_01 = NODE_297 + 1; const int NODE_297_02 = NODE_297 + 2; const int NODE_297_03 = NODE_297 + 3; const int NODE_297_04 = NODE_297 + 4; const int NODE_297_05 = NODE_297 + 5; const int NODE_297_06 = NODE_297 + 6; const int NODE_297_07 = NODE_297 + 7;
        const int NODE_298 = 0x40000000 + 298 * DISCRETE_MAX_OUTPUTS; const int NODE_298_00 = NODE_298; const int NODE_298_01 = NODE_298 + 1; const int NODE_298_02 = NODE_298 + 2; const int NODE_298_03 = NODE_298 + 3; const int NODE_298_04 = NODE_298 + 4; const int NODE_298_05 = NODE_298 + 5; const int NODE_298_06 = NODE_298 + 6; const int NODE_298_07 = NODE_298 + 7;
        const int NODE_299 = 0x40000000 + 299 * DISCRETE_MAX_OUTPUTS; const int NODE_299_00 = NODE_299; const int NODE_299_01 = NODE_299 + 1; const int NODE_299_02 = NODE_299 + 2; const int NODE_299_03 = NODE_299 + 3; const int NODE_299_04 = NODE_299 + 4; const int NODE_299_05 = NODE_299 + 5; const int NODE_299_06 = NODE_299 + 6; const int NODE_299_07 = NODE_299 + 7;


#if false
        NODE0_DEF(0), NODE0_DEF(1), NODE0_DEF(2), NODE0_DEF(3), NODE0_DEF(4), NODE0_DEF(5), NODE0_DEF(6), NODE0_DEF(7), NODE0_DEF(8), NODE0_DEF(9),
        NODE_DEF(10), NODE_DEF(11), NODE_DEF(12), NODE_DEF(13), NODE_DEF(14), NODE_DEF(15), NODE_DEF(16), NODE_DEF(17), NODE_DEF(18), NODE_DEF(19),
        NODE_DEF(20), NODE_DEF(21), NODE_DEF(22), NODE_DEF(23), NODE_DEF(24), NODE_DEF(25), NODE_DEF(26), NODE_DEF(27), NODE_DEF(28), NODE_DEF(29),
        NODE_DEF(30), NODE_DEF(31), NODE_DEF(32), NODE_DEF(33), NODE_DEF(34), NODE_DEF(35), NODE_DEF(36), NODE_DEF(37), NODE_DEF(38), NODE_DEF(39),
        NODE_DEF(40), NODE_DEF(41), NODE_DEF(42), NODE_DEF(43), NODE_DEF(44), NODE_DEF(45), NODE_DEF(46), NODE_DEF(47), NODE_DEF(48), NODE_DEF(49),
        NODE_DEF(50), NODE_DEF(51), NODE_DEF(52), NODE_DEF(53), NODE_DEF(54), NODE_DEF(55), NODE_DEF(56), NODE_DEF(57), NODE_DEF(58), NODE_DEF(59),
        NODE_DEF(60), NODE_DEF(61), NODE_DEF(62), NODE_DEF(63), NODE_DEF(64), NODE_DEF(65), NODE_DEF(66), NODE_DEF(67), NODE_DEF(68), NODE_DEF(69),
        NODE_DEF(70), NODE_DEF(71), NODE_DEF(72), NODE_DEF(73), NODE_DEF(74), NODE_DEF(75), NODE_DEF(76), NODE_DEF(77), NODE_DEF(78), NODE_DEF(79),
        NODE_DEF(80), NODE_DEF(81), NODE_DEF(82), NODE_DEF(83), NODE_DEF(84), NODE_DEF(85), NODE_DEF(86), NODE_DEF(87), NODE_DEF(88), NODE_DEF(89),
        NODE_DEF(90), NODE_DEF(91), NODE_DEF(92), NODE_DEF(93), NODE_DEF(94), NODE_DEF(95), NODE_DEF(96), NODE_DEF(97), NODE_DEF(98), NODE_DEF(99),
        NODE_DEF(100),NODE_DEF(101),NODE_DEF(102),NODE_DEF(103),NODE_DEF(104),NODE_DEF(105),NODE_DEF(106),NODE_DEF(107),NODE_DEF(108),NODE_DEF(109),
        NODE_DEF(110),NODE_DEF(111),NODE_DEF(112),NODE_DEF(113),NODE_DEF(114),NODE_DEF(115),NODE_DEF(116),NODE_DEF(117),NODE_DEF(118),NODE_DEF(119),
        NODE_DEF(120),NODE_DEF(121),NODE_DEF(122),NODE_DEF(123),NODE_DEF(124),NODE_DEF(125),NODE_DEF(126),NODE_DEF(127),NODE_DEF(128),NODE_DEF(129),
        NODE_DEF(130),NODE_DEF(131),NODE_DEF(132),NODE_DEF(133),NODE_DEF(134),NODE_DEF(135),NODE_DEF(136),NODE_DEF(137),NODE_DEF(138),NODE_DEF(139),
        NODE_DEF(140),NODE_DEF(141),NODE_DEF(142),NODE_DEF(143),NODE_DEF(144),NODE_DEF(145),NODE_DEF(146),NODE_DEF(147),NODE_DEF(148),NODE_DEF(149),
        NODE_DEF(150),NODE_DEF(151),NODE_DEF(152),NODE_DEF(153),NODE_DEF(154),NODE_DEF(155),NODE_DEF(156),NODE_DEF(157),NODE_DEF(158),NODE_DEF(159),
        NODE_DEF(160),NODE_DEF(161),NODE_DEF(162),NODE_DEF(163),NODE_DEF(164),NODE_DEF(165),NODE_DEF(166),NODE_DEF(167),NODE_DEF(168),NODE_DEF(169),
        NODE_DEF(170),NODE_DEF(171),NODE_DEF(172),NODE_DEF(173),NODE_DEF(174),NODE_DEF(175),NODE_DEF(176),NODE_DEF(177),NODE_DEF(178),NODE_DEF(179),
        NODE_DEF(180),NODE_DEF(181),NODE_DEF(182),NODE_DEF(183),NODE_DEF(184),NODE_DEF(185),NODE_DEF(186),NODE_DEF(187),NODE_DEF(188),NODE_DEF(189),
        NODE_DEF(190),NODE_DEF(191),NODE_DEF(192),NODE_DEF(193),NODE_DEF(194),NODE_DEF(195),NODE_DEF(196),NODE_DEF(197),NODE_DEF(198),NODE_DEF(199),
        NODE_DEF(200),NODE_DEF(201),NODE_DEF(202),NODE_DEF(203),NODE_DEF(204),NODE_DEF(205),NODE_DEF(206),NODE_DEF(207),NODE_DEF(208),NODE_DEF(209),
        NODE_DEF(210),NODE_DEF(211),NODE_DEF(212),NODE_DEF(213),NODE_DEF(214),NODE_DEF(215),NODE_DEF(216),NODE_DEF(217),NODE_DEF(218),NODE_DEF(219),
        NODE_DEF(220),NODE_DEF(221),NODE_DEF(222),NODE_DEF(223),NODE_DEF(224),NODE_DEF(225),NODE_DEF(226),NODE_DEF(227),NODE_DEF(228),NODE_DEF(229),
        NODE_DEF(230),NODE_DEF(231),NODE_DEF(232),NODE_DEF(233),NODE_DEF(234),NODE_DEF(235),NODE_DEF(236),NODE_DEF(237),NODE_DEF(238),NODE_DEF(239),
        NODE_DEF(240),NODE_DEF(241),NODE_DEF(242),NODE_DEF(243),NODE_DEF(244),NODE_DEF(245),NODE_DEF(246),NODE_DEF(247),NODE_DEF(248),NODE_DEF(249),
        NODE_DEF(250),NODE_DEF(251),NODE_DEF(252),NODE_DEF(253),NODE_DEF(254),NODE_DEF(255),NODE_DEF(256),NODE_DEF(257),NODE_DEF(258),NODE_DEF(259),
        NODE_DEF(260),NODE_DEF(261),NODE_DEF(262),NODE_DEF(263),NODE_DEF(264),NODE_DEF(265),NODE_DEF(266),NODE_DEF(267),NODE_DEF(268),NODE_DEF(269),
        NODE_DEF(270),NODE_DEF(271),NODE_DEF(272),NODE_DEF(273),NODE_DEF(274),NODE_DEF(275),NODE_DEF(276),NODE_DEF(277),NODE_DEF(278),NODE_DEF(279),
        NODE_DEF(280),NODE_DEF(281),NODE_DEF(282),NODE_DEF(283),NODE_DEF(284),NODE_DEF(285),NODE_DEF(286),NODE_DEF(287),NODE_DEF(288),NODE_DEF(289),
        NODE_DEF(290),NODE_DEF(291),NODE_DEF(292),NODE_DEF(293),NODE_DEF(294),NODE_DEF(295),NODE_DEF(296),NODE_DEF(297),NODE_DEF(298),NODE_DEF(299)
#endif
        //}


        /* Some Pre-defined nodes for convenience */

        public static int NODE_(int x) { return NODE_00 + x * DISCRETE_MAX_OUTPUTS; }
        //#define NODE_SUB(_x, _y) ((_x) + (_y))

        //#if DISCRETE_MAX_OUTPUTS == 8
        public static int NODE_CHILD_NODE_NUM(int x) { return x & 7; }
        public static int NODE_DEFAULT_NODE(int x) { return x & ~7; }
        public static int NODE_INDEX(int x) { return (x - NODE_START) >> 3; }
        //#else
        //#error "DISCRETE_MAX_OUTPUTS != 8"
        //#endif

        public static int NODE_RELATIVE(int x, int y) { return NODE_(NODE_INDEX(x) + y); }

        const int NODE_NC = NODE_00;
        public static readonly int NODE_SPECIAL = NODE_(DISCRETE_MAX_NODES);

        public const int NODE_START = NODE_00;
        public static readonly int NODE_END = NODE_SPECIAL;

        public static bool IS_VALUE_A_NODE(int val) { return val > (int)NODE_START && val <= (int)NODE_END; }

        // Optional node such as used in CR_FILTER
        //#define OPT_NODE(val)   (int) val
        static int OPT_NODE(double val) { return (int)val; }
    }


    /*************************************
     *
     *  Enumerated values for Node types
     *  in the simulation
     *
     *      DSS - Discrete Sound Source
     *      DST - Discrete Sound Transform
     *      DSD - Discrete Sound Device
     *      DSO - Discrete Sound Output
     *
     *************************************/

    enum discrete_node_type
    {
        DSS_NULL,           /* Nothing, nill, zippo, only to be used as terminating node */
        DSS_NOP,            /* just do nothing, placeholder for potential DISCRETE_REPLACE in parent block */

        /* standard node */

        DSS_NODE,           /* a standard node */

        /* Custom */
        DST_CUSTOM,         /* whatever you want */

        /* Debugging */
        DSO_CSVLOG,         /* Dump nodes as csv file */
        DSO_WAVLOG,     /* Dump nodes as wav file */

        /* Parallel execution */
        DSO_TASK_START, /* start of parallel task */
        DSO_TASK_END,   /* end of parallel task */

        /* Output Node -- this must be the last entry in this enum! */
        DSO_OUTPUT,         /* The final output node */

        /* Import another blocklist */
        DSO_IMPORT,         /* import from another discrete block */
        DSO_REPLACE,        /* replace next node */
        DSO_DELETE,         /* delete nodes */

        /* Marks end of this enum -- must be last entry ! */
        DSO_LAST
    }


    /*************************************
     *
     *  The discrete sound blocks as
     *  defined in the drivers
     *
     *************************************/
    public class discrete_block
    {
        public int node;                           /* Output node number */
        public Func<discrete_device, discrete_block, discrete_base_node> factory;  //std::unique_ptr<discrete_base_node> (*factory)(discrete_device &pdev, const discrete_block &block);
        public int type;                           /* see defines below */
        public int active_inputs;                  /* Number of active inputs on this node type */
        public int [] input_node = new int[DISCRETE_MAX_INPUTS];/* input/control nodes */
        public MemoryContainer<double> initial = new MemoryContainer<double>(DISCRETE_MAX_INPUTS, true);  //double          initial[DISCRETE_MAX_INPUTS];   /* Initial values */
        public Object custom;  //const void *    custom;                         /* Custom function specific initialisation data */
        string name;                        /* Node Name */
        public string mod_name;                       /* Module / class name */

        public discrete_block(int node, Func<discrete_device, discrete_block, discrete_base_node> factory, int type, int active_inputs, int [] input_node, MemoryContainer<double> initial, Object custom, string name, string mod_name)
        {
            this.node = node;
            this.factory = factory;
            this.type = type;
            this.active_inputs = active_inputs;
            Array.Copy(input_node, this.input_node, input_node.Length);
            initial.CopyTo(0, this.initial, 0, initial.Count);
            this.custom = custom;
            this.name = name;
            this.mod_name = mod_name;
        }
    }


    /*************************************
        *
        *  Node interfaces
        *
        *************************************/

    public interface discrete_step_interface
    {
        //public osd_ticks_t run_time;
        //public discrete_base_node self;

        //~discrete_step_interface() { }

        osd_ticks_t run_time { get; set; }
        discrete_base_node self { get; set; }

        void step();
    }


    public interface discrete_input_interface
    {
        //~discrete_input_interface() { }

        void input_write(int sub_node, uint8_t data );
    }


    public interface discrete_sound_output_interface
    {
        //~discrete_sound_output_interface() { }

        void set_output_ptr(write_stream_view view);
    }


    // ======================> discrete_device
    public class discrete_device : device_t
    {
        //typedef std::vector<std::unique_ptr<discrete_task> > task_list_t;
        //typedef std::vector<std::unique_ptr<discrete_base_node> > node_list_t;
        //typedef std::vector<discrete_step_interface *> node_step_list_t;
        //typedef std::vector<const discrete_block *> sound_block_list_t;


        // configuration state
        discrete_block [] m_intf;  //const discrete_block *m_intf;

        // internal state

        /* --------------------------------- */

        /* emulation info */
        protected int m_sample_rate;
        double m_sample_time;
        double m_neg_sample_time;

        /* list of all nodes */
        protected discrete_device_node_list_t m_node_list = new discrete_device_node_list_t();        /* node_description * */


        /* internal node tracking */
        discrete_base_node [] m_indexed_node;  //std::unique_ptr<discrete_base_node * []>   m_indexed_node;

        /* tasks */
        discrete_device_task_list_t task_list = new discrete_device_task_list_t();      /* discrete_task_context * */

        /* debugging statistics */
        //FILE *                  m_disclogfile;

        /* parallel tasks */
        osd_work_queue m_queue;

        /* profiling */
        int m_profiling;
        uint64_t m_total_samples;
        uint64_t m_total_stream_updates;


        //friend class discrete_base_node;

        // construction/destruction
        public discrete_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_intf = null;
            m_sample_rate = 0;
            m_sample_time = 0;
            m_neg_sample_time = 0;
            m_indexed_node = null;

            //throw new emu_unimplemented();
#if false
            m_disclogfile = null;
#endif

            m_queue = null;
            m_profiling = 0;
            m_total_samples = 0;
            m_total_stream_updates = 0;
        }


        // inline configuration helpers

        //-------------------------------------------------
        //  static_set_intf - configuration helper to set
        //  the interface
        //-------------------------------------------------
        public void set_intf(discrete_block [] intf) { m_intf = intf; }


        //-------------------------------------------------
        //  read - read from the chip's registers and internal RAM
        //-------------------------------------------------
        public uint8_t read(offs_t offset)
        {
            discrete_base_node node = discrete_find_node((int)offset);

            uint8_t data;

            /* Read the node input value if allowed */
            if (node != null)
            {
                /* Bring the system up to now */
                update_to_current_time();

                data = (uint8_t)node.m_output[NODE_CHILD_NODE_NUM((int)offset)].m_pointer[0];
            }
            else
            {
                throw new emu_fatalerror("discrete_sound_r read from non-existent NODE_{0}\n", offset - NODE_00);
            }

            return data;
        }

        //-------------------------------------------------
        //  write - write to the chip's registers and internal RAM
        //-------------------------------------------------
        public void write(offs_t offset, uint8_t data)
        {
            discrete_base_node node = discrete_find_node((int)offset);

            /* Update the node input value if it's a proper input node */
            if (node != null)
            {
                discrete_input_interface intf;
                if (node.interface_get(out intf))
                    intf.input_write(0, data);
                else
                    discrete_log("discrete_sound_w write to non-input NODE_{0}\n", offset-NODE_00);
            }
            else
            {
                discrete_log("discrete_sound_w write to non-existent NODE_{0}\n", offset-NODE_00);
            }
        }


        //template<int DiscreteInput>
        //DECLARE_WRITE_LINE_MEMBER(write_line)
        public void write_line<int_DiscreteInput>(int state)
            where int_DiscreteInput : int_const, new()
        {
            int DiscreteInput = new int_DiscreteInput().value;

            write((offs_t)DiscreteInput, state != 0 ? (uint8_t)1 : (uint8_t)0);
        }


        /* --------------------------------- */

        public virtual void update_to_current_time() { }


        /* process a number of samples */
        //-------------------------------------------------
        //  discrete_device_process - process a number of
        //  samples.
        //
        //  input / output buffers are s32
        //  to not to have to convert the buffers.
        //  a "discrete cpu" device will pass NULL here
        //-------------------------------------------------
        protected void process(int samples)
        {
            if (samples == 0)
                return;

            /* Setup tasks */
            foreach (var task in task_list)
            {
                /* unlock the thread */
                task.unlock();

                task.prepare_for_queue(samples);
            }

            foreach (var task in task_list)
            {
                /* Fire a work item for each task */
                //(void)task;
                m_osdcore.osd_work_item_queue(m_queue, discrete_task.task_callback, task_list, osdcore_interface.WORK_ITEM_FLAG_AUTO_RELEASE);
            }

            m_osdcore.osd_work_queue_wait(m_queue, m_osdcore.osd_ticks_per_second() * 10);

            if (m_profiling != 0)
            {
                m_total_samples += (uint64_t)samples;
                m_total_stream_updates++;
            }
        }

        /* access to the discrete_logging facility */
        //-------------------------------------------------
        //  discrete_log: Debug logging
        //-------------------------------------------------
        public void discrete_log(string format, params object [] args)
        {
            if (DISCRETE_DEBUGLOG != 0)
            {
                throw new emu_unimplemented();
#if false
                va_list arg;
                va_start(arg, text);

                if(m_disclogfile)
                {
                    vfprintf(m_disclogfile, text, arg);
                    fprintf(m_disclogfile, "\n");
                    fflush(m_disclogfile);
                }

                va_end(arg);
#endif

                logerror(format, args);
            }
        }

        /* get pointer to a info struct node ref */
        public Pointer<double> node_output_ptr(int onode)  // const double *discrete_device::node_output_ptr(int onode)
        {
            discrete_base_node node;
            node = discrete_find_node(onode);

            if (node != null)
            {
                return new Pointer<double>(node.m_output[NODE_CHILD_NODE_NUM(onode)].m_pointer);  //&(node->m_output[NODE_CHILD_NODE_NUM(onode)]);
            }
            else
            {
                return null;
            }
        }


        /* FIXME: this is used by csv and wav logs - going forward, identifiers should be explicitly passed */
        //int same_module_index(const discrete_base_node &node);


        /* get node */
        public discrete_base_node discrete_find_node(int node)
        {
            if (node < NODE_START || node > NODE_END)
                return null;

            return m_indexed_node[NODE_INDEX(node)];
        }

        /* are we profiling */
        public int profiling() { return m_profiling; }

        public int sample_rate() { return m_sample_rate; }
        public double sample_time() { return m_sample_time; }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // create the stream
            //m_stream = stream_alloc(0, 2, 22257);

            discrete_block [] intf_start = m_intf;
            string name;

            /* If a clock is specified we will use it, otherwise run at the audio sample rate. */
            if (clock() != 0)
                m_sample_rate = (int)clock();
            else
                m_sample_rate = machine().sample_rate();

            m_sample_time = 1.0 / m_sample_rate;
            m_neg_sample_time = - m_sample_time;

            m_total_samples = 0;
            m_total_stream_updates = 0;

            /* create the logfile */

            //throw new emu_unimplemented();
#if false
            if (DISCRETE_DEBUGLOG)
                m_disclogfile = fopen(util::string_format("discrete%s.log", this->tag()).c_str(), "w");
#endif

            /* enable profiling */
            m_profiling = 0;

            //throw new emu_unimplemented();
#if false
            if (getenv("DISCRETE_PROFILING"))
                m_profiling = atoi(getenv("DISCRETE_PROFILING"));
#endif

            /* Build the final block list */
            discrete_device_sound_block_list_t block_list = new discrete_device_sound_block_list_t();
            discrete_build_list(intf_start, block_list);

            /* first pass through the nodes: sanity check, fill in the indexed_nodes, and make a total count */
            discrete_sanity_check(block_list);

            /* Start with empty lists */
            m_node_list.clear();

            /* allocate memory to hold pointers to nodes by index */
            m_indexed_node = new discrete_base_node [DISCRETE_MAX_NODES];  //m_indexed_node = make_unique_clear<discrete_base_node * []>(DISCRETE_MAX_NODES);

            /* initialize the node data */
            init_nodes(block_list);

            /* now go back and find pointers to all input nodes */
            foreach (var node in m_node_list)
            {
                node.resolve_input_nodes();
            }

            /* allocate a queue */
            m_queue = m_osdcore.osd_work_queue_alloc((int)(osdcore_interface.WORK_QUEUE_FLAG_MULTI | osdcore_interface.WORK_QUEUE_FLAG_HIGH_FREQ));

            /* Process nodes which have a start func */
            foreach (var node in m_node_list)
            {
                node.start();
            }

            /* Now set up tasks */
            foreach (var task in task_list)
            {
                foreach (var dest_task in task_list)
                {
                    if (task.task_group > dest_task.task_group)
                        dest_task.check(task);
                }
            }
        }

        protected override void device_stop()
        {
            if (m_queue != null)
            {
                m_osdcore.osd_work_queue_free(m_queue);
            }

            if (m_profiling != 0)
            {
                display_profiling();
            }

            /* Process nodes which have a stop func */

            foreach (var node in m_node_list)
            {
                node.stop();
            }

            if (DISCRETE_DEBUGLOG != 0)
            {
                throw new emu_unimplemented();
#if false
                /* close the debug log */
                if (m_disclogfile)
                    fclose(m_disclogfile);
                m_disclogfile = NULL;
#endif
            }
        }

        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            update_to_current_time();

            /* loop over all nodes */
            foreach (var node in m_node_list)
            {
                /* Fimxe : node_level */
                //node.m_output[0][0] = 0;

                node.reset();
            }
        }


        //-------------------------------------------------
        //  discrete_build_list: Build import list
        //-------------------------------------------------
        void discrete_build_list(discrete_block [] intf, discrete_device_sound_block_list_t block_list)
        {
            int node_count = 0;

            for (; intf[node_count].type != (int)discrete_node_type.DSS_NULL; )
            {
                /* scan imported */
                if (intf[node_count].type == (int)discrete_node_type.DSO_IMPORT)
                {
                    discrete_log("discrete_build_list() - DISCRETE_IMPORT @ NODE_{0}", NODE_INDEX(intf[node_count].node));
                    discrete_build_list((discrete_block [])intf[node_count].custom, block_list);
                }
                else if (intf[node_count].type == (int)discrete_node_type.DSO_REPLACE)
                {
                    bool found = false;
                    node_count++;
                    if (intf[node_count].type == (int)discrete_node_type.DSS_NULL)
                        throw new emu_fatalerror("discrete_build_list: DISCRETE_REPLACE at end of node_list\n");

                    for (size_t i = 0; i < block_list.size(); i++)
                    {
                        discrete_block block = block_list[i];

                        if (block.type != NODE_SPECIAL)
                        {
                            if (block.node == intf[node_count].node)
                            {
                                block_list[i] = intf[node_count];
                                discrete_log("discrete_build_list() - DISCRETE_REPLACE @ NODE_{0}", NODE_INDEX(intf[node_count].node));
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found)
                        throw new emu_fatalerror("discrete_build_list: DISCRETE_REPLACE did not found node {0}\n", NODE_INDEX(intf[node_count].node));

                }
                else if (intf[node_count].type == (int)discrete_node_type.DSO_DELETE)
                {
                    std.vector<int> deletethem = new std.vector<int>();

                    for (int i = 0; i < (int)block_list.size(); i++)
                    {
                        discrete_block block = block_list[i];

                        if ((block.node >= intf[node_count].input_node[0]) &&
                                (block.node <= intf[node_count].input_node[1]))
                        {
                            discrete_log("discrete_build_list() - DISCRETE_DELETE deleted NODE_{0}", NODE_INDEX(block.node));
                            deletethem.push_back(i);
                        }
                    }

                    foreach (int i in deletethem)
                        block_list.erase(i); // FIXME: how is this supposed to work if there's more than one item to remove?  indices are shifted back on each removal  //block_list.erase(block_list.begin() + i);
                }
                else
                {
                    discrete_log("discrete_build_list() - adding node {0}\n", node_count);
                    block_list.push_back(intf[node_count]);
                }

                node_count++;
            }
        }

        //-------------------------------------------------
        // discrete_sanity_check: Sanity check list
        //-------------------------------------------------
        void discrete_sanity_check(discrete_device_sound_block_list_t block_list)
        {
            int node_count = 0;

            discrete_log("discrete_start() - Doing node list sanity check");
            for (int i = 0; i < (int)block_list.size(); i++)
            {
                discrete_block block = block_list[i];

                /* make sure we don't have too many nodes overall */
                if (node_count > DISCRETE_MAX_NODES)
                    throw new emu_fatalerror("discrete_start() - Upper limit of {0} nodes exceeded, have you terminated the interface block?\n", DISCRETE_MAX_NODES);

                /* make sure the node number is in range */
                if (block.node < NODE_START || block.node > NODE_END)
                    throw new emu_fatalerror("discrete_start() - Invalid node number on node {0} descriptor\n", block.node);

                /* make sure the node type is valid */
                if (block.type > (int)discrete_node_type.DSO_OUTPUT)
                    throw new emu_fatalerror("discrete_start() - Invalid function type on NODE_{0}\n", NODE_INDEX(block.node));

                /* make sure this is a main node */
                if (NODE_CHILD_NODE_NUM(block.node) > 0)
                    throw new emu_fatalerror("discrete_start() - Child node number on NODE_{0}\n", NODE_INDEX(block.node));

                node_count++;
            }

            discrete_log("discrete_start() - Sanity check counted {0} nodes", node_count);
        }


        /*************************************
         *
         *  Master discrete system stop
         *
         *************************************/

        static uint64_t list_run_time(discrete_device_node_list_t list)
        {
            uint64_t total = 0;

            foreach (var node in list)
            {
                discrete_step_interface step;
                if (node.interface_get(out step))
                    total += step.run_time;
            }

            return total;
        }

        static uint64_t step_list_run_time(discrete_device_node_step_list_t list)
        {
            uint64_t total = 0;

            foreach (discrete_step_interface node in list)
            {
                total += node.run_time;
            }

            return total;
        }

        void display_profiling()
        {
            int count;
            uint64_t total;
            uint64_t tresh;

            /* calculate total time */
            total = list_run_time(m_node_list);
            count = (int)m_node_list.size();
            /* print statistics */
            osd_printf_info("Total Samples  : {0}\n", m_total_samples);
            tresh = total / (uint64_t)count;
            osd_printf_info("Threshold (mean): {0}\n", tresh / m_total_samples);
            foreach (var node in m_node_list)
            {
                discrete_step_interface step;
                if (node.interface_get(out step))
                    if (step.run_time > tresh)
                        osd_printf_info("{0}: {1} {2} {3}\n", node.index(), node.module_name(), (double)step.run_time / (double)total * 100.0, ((double)step.run_time) / (double)m_total_samples);
            }

            /* Task information */
            foreach (var task in task_list)
            {
                double tt = step_list_run_time(task.step_list);

                osd_printf_info("Task({0}): {1} {2}\n", task.task_group, tt / (double)total * 100.0, tt / (double)m_total_samples);
            }

            osd_printf_info("Average samples/double->update: {0}\n", (double)m_total_samples / (double)m_total_stream_updates);
        }

        /*************************************
         *
         *  First pass init of nodes
         *
         *************************************/
        void init_nodes(discrete_device_sound_block_list_t block_list)
        {
            discrete_task task = null;
            /* list tail pointers */
            bool has_tasks = false;

            /* check whether we have tasks ... */
            if (USE_DISCRETE_TASKS != 0)
            {
                for (int i = 0; !has_tasks && i < (int)block_list.size(); i++)
                {
                    if (block_list[i].type == (int)discrete_node_type.DSO_TASK_START)
                        has_tasks = true;
                }
            }

            if (!has_tasks)
            {
                /* make sure we have one simple task
                 * No need to create a node since there are no dependencies.
                 */
                task_list.push_back(new discrete_task(this));
                task = task_list.back();
            }

            /* loop over all nodes */
            for (int i = 0; i < (int)block_list.size(); i++)
            {
                discrete_block block = block_list[i];

                // add to node list
                m_node_list.push_back(block.factory(this, block));
                discrete_base_node node = m_node_list.back();

                if (block.node == NODE_SPECIAL)
                {
                    // keep track of special nodes
                    switch (block.type)
                    {
                        /* Output Node */
                        case (int)discrete_node_type.DSO_OUTPUT:
                            /* nothing -> handled later */
                            break;

                        /* CSVlog Node for debugging */
                        case (int)discrete_node_type.DSO_CSVLOG:
                            break;

                        /* Wavelog Node for debugging */
                        case (int)discrete_node_type.DSO_WAVLOG:
                            break;

                        /* Task processing */
                        case (int)discrete_node_type.DSO_TASK_START:
                            if (USE_DISCRETE_TASKS != 0)
                            {
                                if (task != null)
                                    throw new emu_fatalerror("init_nodes() - Nested DISCRETE_START_TASK.\n");

                                task_list.push_back(new discrete_task(this));
                                task = task_list.back();
                                task.task_group = (int)block.initial[0];
                                if (task.task_group < 0 || task.task_group >= DISCRETE_MAX_TASK_GROUPS)
                                    fatalerror("discrete_dso_task: illegal task_group {0}\n", task.task_group);
                                //logerror("task group %d\n", task->task_group);
                            }
                            break;

                        case (int)discrete_node_type.DSO_TASK_END:
                            if (USE_DISCRETE_TASKS != 0)
                            {
                                if (task == null)
                                    throw new emu_fatalerror("init_nodes() - NO DISCRETE_START_TASK.\n");
                            }
                            break;

                        default:
                            throw new emu_fatalerror("init_nodes() - Failed, trying to create unknown special discrete node.\n");
                    }
                }
                else
                {
                    // otherwise, make sure we are not a duplicate, and put ourselves into the indexed list

                    if (m_indexed_node[NODE_INDEX(block.node)] != null)
                        throw new emu_fatalerror("init_nodes() - Duplicate entries for NODE_{0}\n", NODE_INDEX(block.node));

                    m_indexed_node[NODE_INDEX(block.node)] = node;
                }

                // our running order just follows the order specified
                // does the node step?
                discrete_step_interface step;
                if (node.interface_get(out step))
                {
                    /* do we belong to a task? */
                    if (task == null)
                        throw new emu_fatalerror("init_nodes() - found node outside of task: {0}\n", node.module_name());
                    else
                        task.step_list.push_back(step);
                }

                if (USE_DISCRETE_TASKS != 0 && block.type == (int)discrete_node_type.DSO_TASK_END)
                {
                    task = null;
                }

                /* and register save state */
                node.save_state();
            }

            if (!has_tasks)
            {
            }
        }
    }


    // ======================> discrete_sound_device
    public class discrete_sound_device : discrete_device
                                         //public device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(DISCRETE, discrete_sound_device, "discrete", "Discrete Sound")
        static device_t device_creator_discrete_sound_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new discrete_sound_device(mconfig, tag, owner, clock); }
        public static readonly device_type DISCRETE = DEFINE_DEVICE_TYPE(device_creator_discrete_sound_device, "discrete", "Discrete Sound");


        //typedef std::vector<discrete_dss_input_stream_node *> istream_node_list_t;
        //typedef std::vector<discrete_sound_output_interface *> node_output_list_t;


        public class device_sound_interface_discrete : device_sound_interface
        {
            public device_sound_interface_discrete(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((discrete_sound_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        device_sound_interface_discrete m_disound;


        /* the output stream */
        sound_stream m_stream;

        /* the input streams */
        discrete_sound_device_istream_node_list_t m_input_stream_list = new discrete_sound_device_istream_node_list_t();
        /* output node tracking */
        discrete_sound_device_node_output_list_t m_output_list = new discrete_sound_device_node_output_list_t();


        // construction/destruction
        discrete_sound_device(machine_config mconfig, string tag, device_t owner, uint32_t clock, discrete_block [] intf)
            : this(mconfig, tag, owner, clock)
        {
            set_intf(intf);
        }

        discrete_sound_device(machine_config mconfig, string tag, device_t owner, discrete_block [] intf)
            : this(mconfig, tag, owner, (uint32_t)0)
        {
            set_intf(intf);
        }

        discrete_sound_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, DISCRETE, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_discrete(mconfig, this));  //device_sound_interface(mconfig, *this)
            m_disound = GetClassInterface<device_sound_interface_discrete>();

            m_stream = null;
        }


        public void discrete_sound_device_after_ctor(discrete_block [] intf)
        {
            set_intf(intf);
        }


        public device_sound_interface_discrete disound { get { return m_disound; } }


        /* --------------------------------- */

        public override void update_to_current_time() { m_stream.update(); }

        public sound_stream get_stream() { return m_stream; }


        // device-level overrides
        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            m_input_stream_list.clear();
            m_output_list.clear();

            /* call the parent */
            base.device_start();

            /* look for input stream nodes */
            foreach (var node in m_node_list)
            {
                /* if we are an stream input node, track that */
                discrete_dss_input_stream_node input_stream = (node is discrete_dss_input_stream_node) ? (discrete_dss_input_stream_node)node : null;
                if (input_stream != null)
                {
                    m_input_stream_list.push_back(input_stream);
                }

                /* if this is an output interface, add it the output list */
                discrete_sound_output_interface out_intf;
                if (node.interface_get(out out_intf))
                    m_output_list.push_back(out_intf);
            }

            /* if no outputs, give an error */
            if (m_output_list.empty())
                throw new emu_fatalerror("init_nodes() - Couldn't find an output node\n");

            /* initialize the stream(s) */
            m_stream = m_disound.stream_alloc((int)m_input_stream_list.size(), (int)m_output_list.size(), (u32)m_sample_rate);

            /* Finalize stream_input_nodes */
            foreach (discrete_dss_input_stream_node node in m_input_stream_list)
            {
                node.stream_start();
            }
        }

        protected override void device_reset()
        {
            base.device_reset();
        }


        // device_sound_interface overrides
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            int outputnum = 0;

            /* Setup any output streams */
            foreach (discrete_sound_output_interface node in m_output_list)
            {
                node.set_output_ptr(outputs[outputnum]);
                outputnum++;
            }

            /* Setup any input streams */
            foreach (discrete_dss_input_stream_node node in m_input_stream_list)
            {
                node.m_inview = inputs[node.m_stream_in_number];  //(*node)->m_inview = &inputs[(*node)->m_stream_in_number];
                node.m_inview_sample = 0;
            }

            /* just process it */
            process((int)outputs[0].samples());
        }
    }


    /*************************************
     *
     *  Node class
     *
     *************************************/
    public class discrete_base_node
    {
        public double DSS_INPUT__GAIN { get { return DISCRETE_INPUT(0); } }
        public double DSS_INPUT__OFFSET { get { return DISCRETE_INPUT(1); } }
        public double DSS_INPUT__INIT { get { return DISCRETE_INPUT(2); } }


        /* calculate charge exponent using discrete sample time */
        protected double RC_CHARGE_EXP(double rc) { return discrete_global.RC_CHARGE_EXP(this, rc); }

        /* calculate discharge exponent using discrete sample time */
        protected double RC_DISCHARGE_EXP(double rc) { return discrete_global.RC_DISCHARGE_EXP(this, rc); }


        //#define DISCRETE_INPUT(_num)                  (*(this->m_input[_num]))
        //#define DISCRETE_INPUT(_num)                    (input(_num))
        public double DISCRETE_INPUT(int num) { return discrete_global.DISCRETE_INPUT(num, input); }



        public PointerRef<double> [] m_output = new PointerRef<double> [DISCRETE_MAX_OUTPUTS];  //double                          m_output[DISCRETE_MAX_OUTPUTS];     /* The node's last output value */
        public PointerRef<double> [] m_input = new PointerRef<double> [DISCRETE_MAX_INPUTS];  //const double *                  m_input[DISCRETE_MAX_INPUTS];       /* Addresses of Input values */
        protected discrete_device m_device;                           /* Points to the parent */


        discrete_block m_block;  //const discrete_block *  m_block;                            /* Points to the node's setup block. */
        int m_active_inputs;                    /* Number of active inputs on this node type */

        Object m_custom;  //const void *                    m_custom;                           /* Custom function specific initialisation data */
        int m_input_is_node;

        discrete_step_interface m_step_intf;
        discrete_input_interface m_input_intf;
        discrete_sound_output_interface m_output_intf;


        //friend class discrete_device;
        //template <class C> friend class discrete_node_factory;
        //friend class discrete_task;


        public discrete_base_node()
        {
            m_device = null;
            m_block = null;
            m_active_inputs = 0;
            m_custom = null;
            m_input_is_node = 0;
            m_step_intf = null;
            m_input_intf = null;
            m_output_intf = null;


            //m_output[0][0] = 0.0;


            for (int i = 0; i < DISCRETE_MAX_OUTPUTS; i++)
                m_output[i] = new PointerRef<double>(new Pointer<double>(new MemoryContainer<double>(new double [] { 0 })));
            for (int i = 0; i < DISCRETE_MAX_INPUTS; i++)
                m_input[i] = new PointerRef<double>(new Pointer<double>(new MemoryContainer<double>(new double [] { 0 })));
        }

        //~discrete_base_node() { }


        public virtual void reset() { }
        public virtual void start() { }
        public virtual void stop() { }

        public virtual void save_state()
        {
            if (m_block.node != NODE_SPECIAL)
                m_device.save_item(NAME(new { m_output }), m_block.node);
        }

        protected virtual int max_output() { return 1; }


        public bool interface_get(out discrete_step_interface intf) { intf = m_step_intf; return intf != null; }  //inline bool interface(discrete_step_interface *&intf) const { intf = m_step_intf; return (intf != NULL); }
        public bool interface_get(out discrete_input_interface intf) { intf = m_input_intf; return intf != null; }  //inline bool interface(discrete_input_interface *&intf) const { intf = m_input_intf; return (intf != NULL); }
        public bool interface_get(out discrete_sound_output_interface intf) { intf = m_output_intf; return intf != null; }  //inline bool interface(discrete_sound_output_interface *&intf) const { intf = m_output_intf; return (intf != NULL); }


        /* get the input value from node #n */
        double input(int n) { return m_input[n].m_pointer[0]; }

        /* set an output */
        public void set_output(int n, double val) { if (m_output[n].m_pointer != null && m_output[n].m_pointer.Buffer != null) m_output[n].m_pointer[0] = val; }

        /* Return the node index, i.e. X from NODE(X) */
        public int index() { return NODE_INDEX(m_block.node); }

        /* Return the node number, i.e. NODE(X) */
        public int block_node() { return m_block.node;  }

        /* Custom function specific initialisation data */
        protected Object custom_data() { return m_custom; }

        public int input_node(int inputnum) { return m_block.input_node[inputnum]; }

        /* Number of active inputs on this node type */
        public int active_inputs() { return m_active_inputs; }
        /* Bit Flags.  1 in bit location means input_is_node */
        protected int input_is_node() { return m_input_is_node; }

        public double sample_time() { return m_device.sample_time(); }
        public int sample_rate() { return m_device.sample_rate(); }

        public string module_name() { return m_block.mod_name; }
        //inline int          module_type(void) const { return m_block->type; }


        /* finish node setup after allocation is complete */
        public void init(discrete_device pdev, discrete_block xblock)
        {
            m_device = pdev;
            m_block = xblock;

            m_custom = m_block.custom;
            m_active_inputs = m_block.active_inputs;

            m_step_intf = (this is discrete_step_interface) ? (discrete_step_interface)this : null;
            m_input_intf = (this is discrete_input_interface) ? (discrete_input_interface)this : null;
            m_output_intf = (this is discrete_sound_output_interface) ? (discrete_sound_output_interface)this : null;

            if (m_step_intf != null)
            {
                m_step_intf.run_time = 0;
                m_step_intf.self = this;
            }
        }


        public void resolve_input_nodes()
        {
            int inputnum;

            /* loop over all active inputs */
            for (inputnum = 0; inputnum < m_active_inputs; inputnum++)
            {
                int inputnode = m_block.input_node[inputnum];

                /* if this input is node-based, find the node in the indexed list */
                if (IS_VALUE_A_NODE(inputnode))
                {
                    //discrete_base_node *node_ref = m_device->m_indexed_node[NODE_INDEX(inputnode)];
                    discrete_base_node node_ref = m_device.discrete_find_node(inputnode);
                    if (node_ref == null)
                        throw new emu_fatalerror("discrete_start - NODE_{0} referenced a non existent node NODE_{1}\n", index(), NODE_INDEX(inputnode));

                    if ((NODE_CHILD_NODE_NUM(inputnode) >= node_ref.max_output()) /*&& (node_ref->module_type() != DST_CUSTOM)*/)
                        throw new emu_fatalerror("discrete_start - NODE_{0} referenced non existent output {1} on node NODE_{2}\n", index(), NODE_CHILD_NODE_NUM(inputnode), NODE_INDEX(inputnode));

                    m_input[inputnum] = node_ref.m_output[NODE_CHILD_NODE_NUM(inputnode)];  // m_input[inputnum] = &(node_ref->m_output[NODE_CHILD_NODE_NUM(inputnode)]);  /* Link referenced node out to input */
                    m_input_is_node |= 1 << inputnum;           /* Bit flag if input is node */
                }
                else
                {
                    /* warn if trying to use a node for an input that can only be static */
                    if (IS_VALUE_A_NODE((int)m_block.initial[inputnum]))
                    {
                        m_device.discrete_log("Warning - discrete_start - NODE_{0} trying to use a node on static input {1}", index(), inputnum);
                        /* also report it in the error log so it is not missed */
                        m_device.logerror("Warning - discrete_start - NODE_{0} trying to use a node on static input {1}", index(), inputnum);
                    }
                    else
                    {
                        m_input[inputnum].m_pointer = new Pointer<double>(m_block.initial, inputnum);  //m_input[inputnum] = &(m_block->initial[inputnum]);
                    }
                }
            }

            for (inputnum = m_active_inputs; inputnum < DISCRETE_MAX_INPUTS; inputnum++)
            {
                /* FIXME: Check that no nodes follow ! */
                m_input[inputnum].m_pointer = new Pointer<double>(m_block.initial, inputnum);  //m_input[inputnum] = &(m_block->initial[inputnum]);
            }
        }
    }


    //template <class C>
    class discrete_node_factory<C>
        where C : discrete_base_node, new()
    {
        public static discrete_base_node create(discrete_device pdev, discrete_block block)
        {
            discrete_base_node r = new C();  //std::unique_ptr<discrete_base_node> r = make_unique_clear<C>();

            r.init(pdev, block);
            return r;
        }
    }


    public static partial class discrete_global
    {
        static discrete_base_node discrete_create_node<C>(discrete_device pdev, discrete_block block) where C : discrete_base_node, new() { return discrete_node_factory<C>.create(pdev, block); }


        //#define DISCRETE_SOUND_EXTERN(name) extern const discrete_block name[]
        //#define DISCRETE_SOUND_START(name) const discrete_block name[] = {
        //#define DSC_SND_ENTRY(_nod, _class, _dss, _num, _iact, _iinit, _custom, _name) { _nod,  new discrete_node_factory< DISCRETE_CLASS_NAME(_class) >, _dss, _num, _iact, _iinit, _custom, _name, # _class }
        public static discrete_block DSC_SND_ENTRY<class_type>(int node, int dss, int num, int [] iact, MemoryContainer<double> iinit, Object custom, string name) where class_type : discrete_base_node, new()
        { return new discrete_block(node, discrete_create_node<class_type>, dss, num, iact, iinit, custom, name, typeof(class_type).FullName); } // _nod,  &discrete_create_node< DISCRETE_CLASS_NAME(_class) >, _dss, _num, _iact, _iinit, _custom, _name, /* # _class*/ }; }

        public static discrete_block DISCRETE_SOUND_END { get { return DSC_SND_ENTRY<discrete_special_node>( NODE_00, (int)discrete_node_type.DSS_NULL     , 0, DSE( NODE_NC ), DSE( 0.0 ) ,null  ,"DISCRETE_SOUND_END" ); } }

        static int [] DSE(params int [] objects) { return objects; }  //#define DSE( ... ) { __VA_ARGS__ }
        static MemoryContainer<double> DSE(params double [] objects) { return new MemoryContainer<double>(objects); }  //#define DSE( ... ) { __VA_ARGS__ }

        /*      Module Name                                                       out,  enum value,      #in,   {variable inputs},              {static inputs},    data pointer,   "name" */

        /* from disc_inp.inc */
        public static discrete_block DISCRETE_ADJUSTMENT(int NODE, double MIN, double MAX, double LOGLIN, string TAG) { return DSC_SND_ENTRY<discrete_dss_adjustment_node>( NODE, (int)discrete_node_type.DSS_NODE        , 6, DSE( NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC ), DSE( MIN,MAX,LOGLIN,0   ,0   ,100  ), TAG   , "DISCRETE_ADJUSTMENT" ); }
        //#define DISCRETE_ADJUSTMENTX(NODE,MIN,MAX,LOGLIN,TAG,PMIN,PMAX)         DSC_SND_ENTRY( NODE, dss_adjustment  , DSS_NODE        , 6, DSE( NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC ), DSE( MIN,MAX,LOGLIN,0   ,PMIN,PMAX ), TAG   , "DISCRETE_ADJUSTMENTX"  ),
        //#define DISCRETE_CONSTANT(NODE,CONST)                                   DSC_SND_ENTRY( NODE, dss_constant    , DSS_NODE        , 1, DSE( NODE_NC ), DSE( CONST ) ,NULL  ,"DISCRETE_CONSTANT" ),
        public static discrete_block DISCRETE_INPUT_DATA(int NODE) { return DSC_SND_ENTRY<discrete_dss_input_data_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( NODE_NC,NODE_NC,NODE_NC ), DSE( 1.0,0,0 ), null, "DISCRETE_INPUT_DATA" ); }
        public static discrete_block DISCRETE_INPUTX_DATA(int NODE, double GAIN, double OFFSET, double INIT) { return DSC_SND_ENTRY<discrete_dss_input_data_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( NODE_NC,NODE_NC,NODE_NC ), DSE( GAIN,OFFSET,INIT ), null, "DISCRETE_INPUTX_DATA" ); }
        public static discrete_block DISCRETE_INPUT_LOGIC(int NODE) { return DSC_SND_ENTRY<discrete_dss_input_logic_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( NODE_NC,NODE_NC,NODE_NC ), DSE( 1.0,0,0 ), null, "DISCRETE_INPUT_LOGIC" ); }
        public static discrete_block DISCRETE_INPUTX_LOGIC(int NODE, double GAIN, double OFFSET, double INIT) { return DSC_SND_ENTRY<discrete_dss_input_logic_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( NODE_NC,NODE_NC,NODE_NC ), DSE( GAIN,OFFSET,INIT ), null, "DISCRETE_INPUTX_LOGIC" ); }
        public static discrete_block DISCRETE_INPUT_NOT(int NODE) { return DSC_SND_ENTRY<discrete_dss_input_not_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( NODE_NC,NODE_NC,NODE_NC ), DSE( 1.0,0,0 ), null, "DISCRETE_INPUT_NOT" ); }
        //#define DISCRETE_INPUTX_NOT(NODE,GAIN,OFFSET,INIT)                      DSC_SND_ENTRY( NODE, dss_input_not   , DSS_NODE        , 3, DSE( NODE_NC,NODE_NC,NODE_NC ), DSE( GAIN,OFFSET,INIT ), NULL, "DISCRETE_INPUTX_NOT" ),
        public static discrete_block DISCRETE_INPUT_PULSE(int NODE, double INIT) { return DSC_SND_ENTRY<discrete_dss_input_pulse_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( NODE_NC,NODE_NC,NODE_NC ), DSE( 1,0,INIT ), null, "DISCRETE_INPUT_PULSE" ); }

        //#define DISCRETE_INPUT_STREAM(NODE, NUM)                                DSC_SND_ENTRY( NODE, dss_input_stream, DSS_NODE        , 3, DSE( NUM,NODE_NC,NODE_NC ), DSE( NUM,1,0 ), NULL, "DISCRETE_INPUT_STREAM" ),
        public static discrete_block DISCRETE_INPUTX_STREAM(int NODE, double NUM, double GAIN, double OFFSET) { return DSC_SND_ENTRY<discrete_dss_input_stream_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)NUM,NODE_NC,NODE_NC ), DSE( NUM,GAIN,OFFSET ), null, "DISCRETE_INPUTX_STREAM" ); }

        public static discrete_block DISCRETE_INPUT_BUFFER(int NODE, double NUM) { return DSC_SND_ENTRY<discrete_dss_input_buffer_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)NUM,NODE_NC,NODE_NC ), DSE( NUM,1,0 ), null, "DISCRETE_INPUT_BUFFER" ); }

        /* from disc_wav.inc */
        /* generic modules */
        public static discrete_block DISCRETE_COUNTER(int NODE, double ENAB, double RESET, double CLK, double MIN, double MAX, double DIR, double INIT0, double CLKTYPE) { return DSC_SND_ENTRY<discrete_dss_counter_node>( NODE, (int)discrete_node_type.DSS_NODE        , 8, DSE( (int)ENAB,(int)RESET,(int)CLK,NODE_NC,NODE_NC,(int)DIR,(int)INIT0,NODE_NC ), DSE( ENAB,RESET,CLK,MIN,MAX,DIR,INIT0,CLKTYPE ), null, "DISCRETE_COUNTER" ); }
        //#define DISCRETE_COUNTER_7492(NODE,ENAB,RESET,CLK,CLKTYPE)              DSC_SND_ENTRY( NODE, dss_counter     , DSS_NODE        , 8, DSE( ENAB,RESET,CLK,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC ), DSE( ENAB,RESET,CLK,CLKTYPE,0,1,0,DISC_COUNTER_IS_7492 ), NULL, "DISCRETE_COUNTER_7492" ),
        public static discrete_block DISCRETE_LFSR_NOISE(int NODE, double ENAB, double RESET, double CLK, double AMPL, double FEED, double BIAS, discrete_lfsr_desc LFSRTB) { return DSC_SND_ENTRY<discrete_dss_lfsr_noise_node>( NODE, (int)discrete_node_type.DSS_NODE        , 6, DSE( (int)ENAB,(int)RESET,(int)CLK,(int)AMPL,(int)FEED,(int)BIAS ), DSE( ENAB,RESET,CLK,AMPL,FEED,BIAS ), LFSRTB, "DISCRETE_LFSR_NOISE" ); }
        //#define DISCRETE_NOISE(NODE,ENAB,FREQ,AMPL,BIAS)                        DSC_SND_ENTRY( NODE, dss_noise       , DSS_NODE        , 4, DSE( ENAB,FREQ,AMPL,BIAS ), DSE( ENAB,FREQ,AMPL,BIAS ), NULL, "DISCRETE_NOISE" ),
        public static discrete_block DISCRETE_NOTE(int NODE, double ENAB, double CLK, double DATA, double MAX1, double MAX2, double CLKTYPE) { return DSC_SND_ENTRY<discrete_dss_note_node>( NODE, (int)discrete_node_type.DSS_NODE        , 6, DSE( (int)ENAB,(int)CLK,(int)DATA,NODE_NC,NODE_NC,NODE_NC ), DSE( ENAB,CLK,DATA,MAX1,MAX2,CLKTYPE ), null, "DISCRETE_NOTE" ); }
        //#define DISCRETE_SAWTOOTHWAVE(NODE,ENAB,FREQ,AMPL,BIAS,GRAD,PHASE)      DSC_SND_ENTRY( NODE, dss_sawtoothwave, DSS_NODE        , 6, DSE( ENAB,FREQ,AMPL,BIAS,NODE_NC,NODE_NC ), DSE( ENAB,FREQ,AMPL,BIAS,GRAD,PHASE ), NULL, "DISCRETE_SAWTOOTHWAVE" ),
        //#define DISCRETE_SINEWAVE(NODE,ENAB,FREQ,AMPL,BIAS,PHASE)               DSC_SND_ENTRY( NODE, dss_sinewave    , DSS_NODE        , 5, DSE( ENAB,FREQ,AMPL,BIAS,NODE_NC ), DSE( ENAB,FREQ,AMPL,BIAS,PHASE ), NULL, "DISCRETE_SINEWAVE" ),
        public static discrete_block DISCRETE_SQUAREWAVE(int NODE, double ENAB, double FREQ, double AMPL, double DUTY, double BIAS, double PHASE) { return DSC_SND_ENTRY<discrete_dss_squarewave_node>( NODE, (int)discrete_node_type.DSS_NODE        , 6, DSE( (int)ENAB,(int)FREQ,(int)AMPL,(int)DUTY,(int)BIAS,NODE_NC ), DSE( ENAB,FREQ,AMPL,DUTY,BIAS,PHASE ), null, "DISCRETE_SQUAREWAVE" ); }
        public static discrete_block DISCRETE_SQUAREWFIX(int NODE, double ENAB, double FREQ, double AMPL, double DUTY, double BIAS, double PHASE) { return DSC_SND_ENTRY<discrete_dss_squarewfix_node>( NODE, (int)discrete_node_type.DSS_NODE        , 6, DSE( (int)ENAB,(int)FREQ,(int)AMPL,(int)DUTY,(int)BIAS,NODE_NC ), DSE( ENAB,FREQ,AMPL,DUTY,BIAS,PHASE ), null, "DISCRETE_SQUAREWFIX" ); }
        //#define DISCRETE_SQUAREWAVE2(NODE,ENAB,AMPL,T_OFF,T_ON,BIAS,TSHIFT)     DSC_SND_ENTRY( NODE, dss_squarewave2 , DSS_NODE        , 6, DSE( ENAB,AMPL,T_OFF,T_ON,BIAS,NODE_NC ), DSE( ENAB,AMPL,T_OFF,T_ON,BIAS,TSHIFT ), NULL, "DISCRETE_SQUAREWAVE2" ),
        public static discrete_block DISCRETE_TRIANGLEWAVE(int NODE, double ENAB, double FREQ, double AMPL, double BIAS, double PHASE) { return DSC_SND_ENTRY<discrete_dss_trianglewave_node>( NODE, (int)discrete_node_type.DSS_NODE        , 5, DSE( (int)ENAB,(int)FREQ,(int)AMPL,(int)BIAS,NODE_NC ), DSE( ENAB,FREQ,AMPL,BIAS,PHASE ), null, "DISCRETE_TRIANGLEWAVE" ); }
        /* Component specific */
        public static discrete_block DISCRETE_INVERTER_OSC(int NODE, double ENAB, double MOD, double RCHARGE, double RP, double C, double R2, discrete_dss_inverter_osc_node.description INFO) { return DSC_SND_ENTRY<discrete_dss_inverter_osc_node>( NODE, (int)discrete_node_type.DSS_NODE        , 6, DSE( (int)ENAB,(int)MOD,NODE_NC,NODE_NC,NODE_NC,NODE_NC ), DSE( ENAB,MOD,RCHARGE,RP,C,R2 ), INFO, "DISCRETE_INVERTER_OSC" ); }
        public static discrete_block DISCRETE_OP_AMP_OSCILLATOR(int NODE, double ENAB, discrete_op_amp_osc_info INFO) { return DSC_SND_ENTRY<discrete_dss_op_amp_osc_node>( NODE, (int)discrete_node_type.DSS_NODE        , 1, DSE( (int)ENAB ), DSE( ENAB ), INFO, "DISCRETE_OP_AMP_OSCILLATOR" ); }
        public static discrete_block DISCRETE_OP_AMP_VCO1(int NODE, double ENAB, double VMOD1, discrete_op_amp_osc_info INFO) { return DSC_SND_ENTRY<discrete_dss_op_amp_osc_node>( NODE, (int)discrete_node_type.DSS_NODE        , 2, DSE( (int)ENAB,(int)VMOD1 ), DSE( ENAB,VMOD1 ), INFO, "DISCRETE_OP_AMP_VCO1" ); }
        public static discrete_block DISCRETE_OP_AMP_VCO2(int NODE, double ENAB, double VMOD1, double VMOD2, discrete_op_amp_osc_info INFO) { return DSC_SND_ENTRY<discrete_dss_op_amp_osc_node>( NODE,  (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)ENAB,(int)VMOD1,(int)VMOD2 ), DSE( ENAB,VMOD1,VMOD2 ), INFO, "DISCRETE_OP_AMP_VCO2" ); }
        //#define DISCRETE_SCHMITT_OSCILLATOR(NODE,ENAB,INP0,AMPL,TABLE)          DSC_SND_ENTRY( NODE, dss_schmitt_osc , DSS_NODE        , 3, DSE( ENAB,INP0,AMPL ), DSE( ENAB,INP0,AMPL ), TABLE, "DISCRETE_SCHMITT_OSCILLATOR" ),
        /* Not yet implemented */
        //#define DISCRETE_ADSR_ENV(NODE,ENAB,TRIGGER,GAIN,ADSRTB)                DSC_SND_ENTRY( NODE, dss_adsr        , DSS_NODE        , 3, DSE( ENAB,TRIGGER,GAIN ), DSE( ENAB,TRIGGER,GAIN ), ADSRTB, "DISCRETE_ADSR_ENV" ),

        /* from disc_mth.inc */
        /* generic modules */
        public static discrete_block DISCRETE_ADDER2(int NODE, double ENAB, double INP0, double INP1) { return DSC_SND_ENTRY<discrete_dst_adder_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)ENAB,(int)INP0,(int)INP1 ), DSE( ENAB,INP0,INP1 ), null, "DISCRETE_ADDER2" ); }
        //#define DISCRETE_ADDER3(NODE,ENAB,INP0,INP1,INP2)                       DSC_SND_ENTRY( NODE, dst_adder       , DSS_NODE        , 4, DSE( ENAB,INP0,INP1,INP2 ), DSE( ENAB,INP0,INP1,INP2 ), NULL, "DISCRETE_ADDER3" ),
        public static discrete_block DISCRETE_ADDER4(int NODE, double ENAB, double INP0, double INP1, double INP2, double INP3) { return DSC_SND_ENTRY<discrete_dst_adder_node>( NODE, (int)discrete_node_type.DSS_NODE        , 5, DSE( (int)ENAB,(int)INP0,(int)INP1,(int)INP2,(int)INP3 ), DSE( ENAB,INP0,INP1,INP2,INP3 ), null, "DISCRETE_ADDER4" ); }
        public static discrete_block DISCRETE_CLAMP(int NODE, double INP0, double MIN, double MAX) { return DSC_SND_ENTRY<discrete_dst_clamp_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)INP0,(int)MIN,(int)MAX ), DSE( INP0,MIN,MAX ), null, "DISCRETE_CLAMP" ); }
        public static discrete_block DISCRETE_DIVIDE(int NODE, double ENAB, double INP0, double INP1) { return DSC_SND_ENTRY<discrete_dst_divide_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)ENAB,(int)INP0,(int)INP1 ), DSE( ENAB,INP0,INP1 ), null, "DISCRETE_DIVIDE" ); }
        public static discrete_block DISCRETE_GAIN(int NODE, double INP0, double GAIN) { return DSC_SND_ENTRY<discrete_dst_gain_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)INP0,NODE_NC,NODE_NC ), DSE( INP0,GAIN,0 ), null, "DISCRETE_GAIN" ); }
        //#define DISCRETE_INVERT(NODE,INP0)                                      DSC_SND_ENTRY( NODE, dst_gain        , DSS_NODE        , 3, DSE( INP0,NODE_NC,NODE_NC ), DSE( INP0,-1,0 ), NULL, "DISCRETE_INVERT" ),
        public static discrete_block DISCRETE_LOGIC_INVERT(int NODE, double INP0) {return DSC_SND_ENTRY<discrete_dst_logic_inv_node>( NODE, (int)discrete_node_type.DSS_NODE        , 1, DSE( (int)INP0 ), DSE( INP0 ), null, "DISCRETE_LOGIC_INVERT" ); }

        //#define DISCRETE_BIT_DECODE(NODE, INP, BIT_N, VOUT)                     DSC_SND_ENTRY( NODE, dst_bits_decode , DSS_NODE        , 4, DSE( INP,NODE_NC,NODE_NC,NODE_NC ), DSE( INP,BIT_N,BIT_N,VOUT ), NULL, "DISCRETE_BIT_DECODE" ),
        public static discrete_block DISCRETE_BITS_DECODE(int NODE, double INP, double BIT_FROM, double BIT_TO, double VOUT) { return DSC_SND_ENTRY<discrete_dst_bits_decode_node>( NODE, (int)discrete_node_type.DSS_NODE        , 4, DSE( (int)INP,NODE_NC,NODE_NC,NODE_NC ), DSE( INP,BIT_FROM,BIT_TO,VOUT ), null, "DISCRETE_BITS_DECODE" ); }

        //#define DISCRETE_LOGIC_AND(NODE,INP0,INP1)                              DSC_SND_ENTRY( NODE, dst_logic_and   , DSS_NODE        , 4, DSE( INP0,INP1,NODE_NC,NODE_NC ), DSE( INP0,INP1,1.0,1.0 ), NULL, "DISCRETE_LOGIC_AND" ),
        public static discrete_block DISCRETE_LOGIC_AND3(int NODE, double INP0, double INP1, double INP2) { return DSC_SND_ENTRY<discrete_dst_logic_and_node>( NODE, (int)discrete_node_type.DSS_NODE        , 4, DSE( (int)INP0,(int)INP1,(int)INP2,NODE_NC ), DSE( INP0,INP1,INP2,1.0 ), null, "DISCRETE_LOGIC_AND3" ); }
        //#define DISCRETE_LOGIC_AND4(NODE,INP0,INP1,INP2,INP3)                   DSC_SND_ENTRY( NODE, dst_logic_and   , DSS_NODE        , 4, DSE( INP0,INP1,INP2,INP3 ), DSE( INP0,INP1,INP2,INP3 ) ,NULL, "DISCRETE_LOGIC_AND4" ),
        //#define DISCRETE_LOGIC_NAND(NODE,INP0,INP1)                             DSC_SND_ENTRY( NODE, dst_logic_nand  , DSS_NODE        , 4, DSE( INP0,INP1,NODE_NC,NODE_NC ), DSE( INP0,INP1,1.0,1.0 ), NULL, "DISCRETE_LOGIC_NAND" ),
        //#define DISCRETE_LOGIC_NAND3(NODE,INP0,INP1,INP2)                       DSC_SND_ENTRY( NODE, dst_logic_nand  , DSS_NODE        , 4, DSE( INP0,INP1,INP2,NODE_NC ), DSE( INP0,INP1,INP2,1.0 ), NULL, "DISCRETE_LOGIC_NAND3" ),
        //#define DISCRETE_LOGIC_NAND4(NODE,INP0,INP1,INP2,INP3)                  DSC_SND_ENTRY( NODE, dst_logic_nand  , DSS_NODE        , 4, DSE( INP0,INP1,INP2,INP3 ), DSE( INP0,INP1,INP2,INP3 ), NULL, ")DISCRETE_LOGIC_NAND4" ),
        //#define DISCRETE_LOGIC_OR(NODE,INP0,INP1)                               DSC_SND_ENTRY( NODE, dst_logic_or    , DSS_NODE        , 4, DSE( INP0,INP1,NODE_NC,NODE_NC ), DSE( INP0,INP1,0.0,0.0 ), NULL, "DISCRETE_LOGIC_OR" ),
        //#define DISCRETE_LOGIC_OR3(NODE,INP0,INP1,INP2)                         DSC_SND_ENTRY( NODE, dst_logic_or    , DSS_NODE        , 4, DSE( INP0,INP1,INP2,NODE_NC ), DSE( INP0,INP1,INP2,0.0 ), NULL, "DISCRETE_LOGIC_OR3" ),
        //#define DISCRETE_LOGIC_OR4(NODE,INP0,INP1,INP2,INP3)                    DSC_SND_ENTRY( NODE, dst_logic_or    , DSS_NODE        , 4, DSE( INP0,INP1,INP2,INP3 ), DSE( INP0,INP1,INP2,INP3 ), NULL, "DISCRETE_LOGIC_OR4" ),
        //#define DISCRETE_LOGIC_NOR(NODE,INP0,INP1)                              DSC_SND_ENTRY( NODE, dst_logic_nor   , DSS_NODE        , 4, DSE( INP0,INP1,NODE_NC,NODE_NC ), DSE( INP0,INP1,0.0,0.0 ), NULL, "DISCRETE_LOGIC_NOR" ),
        //#define DISCRETE_LOGIC_NOR3(NODE,INP0,INP1,INP2)                        DSC_SND_ENTRY( NODE, dst_logic_nor   , DSS_NODE        , 4, DSE( INP0,INP1,INP2,NODE_NC ), DSE( INP0,INP1,INP2,0.0 ), NULL, "DISCRETE_LOGIC_NOR3" ),
        //#define DISCRETE_LOGIC_NOR4(NODE,INP0,INP1,INP2,INP3)                   DSC_SND_ENTRY( NODE, dst_logic_nor   , DSS_NODE        , 4, DSE( INP0,INP1,INP2,INP3 ), DSE( INP0,INP1,INP2,INP3 ), NULL, "DISCRETE_LOGIC_NOR4" ),
        //#define DISCRETE_LOGIC_XOR(NODE,INP0,INP1)                              DSC_SND_ENTRY( NODE, dst_logic_xor   , DSS_NODE        , 2, DSE( INP0,INP1 ), DSE( INP0,INP1 ), NULL, "DISCRETE_LOGIC_XOR" ),
        //#define DISCRETE_LOGIC_XNOR(NODE,INP0,INP1)                             DSC_SND_ENTRY( NODE, dst_logic_nxor  , DSS_NODE        , 2, DSE( INP0,INP1 ), DSE( INP0,INP1 ), NULL, "DISCRETE_LOGIC_XNOR" ),
        public static discrete_block DISCRETE_LOGIC_DFLIPFLOP(int NODE, double RESET, double SET, double CLK, double INP) { return DSC_SND_ENTRY<discrete_dst_logic_dff_node>( NODE, (int)discrete_node_type.DSS_NODE        , 4, DSE( (int)RESET,(int)SET,(int)CLK,(int)INP ), DSE( RESET,SET,CLK,INP ), null, "DISCRETE_LOGIC_DFLIPFLOP" ); }
        //#define DISCRETE_LOGIC_JKFLIPFLOP(NODE,RESET,SET,CLK,J,K)               DSC_SND_ENTRY( NODE, dst_logic_jkff  , DSS_NODE        , 5, DSE( RESET,SET,CLK,J,K ), DSE( RESET,SET,CLK,J,K ), NULL, "DISCRETE_LOGIC_JKFLIPFLOP" ),
        //#define DISCRETE_LOGIC_SHIFT(NODE,INP0,RESET,CLK,SIZE,OPTIONS)          DSC_SND_ENTRY( NODE, dst_logic_shift , DSS_NODE        , 5, DSE( INP0,RESET,CLK,NODE_NC,NODE_NC ), DSE( INP0,RESET,CLK,SIZE,OPTIONS ), NULL, "DISCRETE_LOGIC_SHIFT" ),
        //#define DISCRETE_LOOKUP_TABLE(NODE,ADDR,SIZE,TABLE)                     DSC_SND_ENTRY( NODE, dst_lookup_table, DSS_NODE        , 2, DSE( ADDR,NODE_NC ), DSE( ADDR,SIZE ), TABLE, "DISCRETE_LOOKUP_TABLE" ),
        //#define DISCRETE_MULTIPLEX2(NODE,ADDR,INP0,INP1)                        DSC_SND_ENTRY( NODE, dst_multiplex   , DSS_NODE        , 3, DSE( ADDR,INP0,INP1 ), DSE( ADDR,INP0,INP1 ), NULL, "DISCRETE_MULTIPLEX2" ),
        //#define DISCRETE_MULTIPLEX4(NODE,ADDR,INP0,INP1,INP2,INP3)              DSC_SND_ENTRY( NODE, dst_multiplex   , DSS_NODE        , 5, DSE( ADDR,INP0,INP1,INP2,INP3 ), DSE( ADDR,INP0,INP1,INP2,INP3 ), NULL, "DISCRETE_MULTIPLEX4" ),
        //#define DISCRETE_MULTIPLEX8(NODE,ADDR,INP0,INP1,INP2,INP3,INP4,INP5,INP6,INP7) DSC_SND_ENTRY( NODE, dst_multiplex, DSS_NODE    , 9, DSE( ADDR,INP0,INP1,INP2,INP3,INP4,INP5,INP6,INP7 ), DSE( ADDR,INP0,INP1,INP2,INP3,INP4,INP5,INP6,INP7 ), NULL, "DISCRETE_MULTIPLEX8" ),
        public static discrete_block DISCRETE_MULTIPLY(int NODE, double INP0, double INP1) { return DSC_SND_ENTRY<discrete_dst_gain_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)INP0,(int)INP1,NODE_NC ), DSE( INP0,INP1,0 ), null, "DISCRETE_MULTIPLY" ); }
        public static discrete_block DISCRETE_MULTADD(int NODE, double INP0, double INP1, double INP2) { return DSC_SND_ENTRY<discrete_dst_gain_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)INP0,(int)INP1,(int)INP2 ), DSE( INP0,INP1,INP2 ), null, "DISCRETE_MULTADD" ); }
        //#define DISCRETE_ONESHOT(NODE,TRIG,AMPL,WIDTH,TYPE)                     DSC_SND_ENTRY( NODE, dst_oneshot     , DSS_NODE        , 5, DSE( 0,TRIG,AMPL,WIDTH,NODE_NC ), DSE( 0,TRIG,AMPL,WIDTH,TYPE ), NULL, "DISCRETE_ONESHOT" ),
        //#define DISCRETE_ONESHOTR(NODE,RESET,TRIG,AMPL,WIDTH,TYPE)              DSC_SND_ENTRY( NODE, dst_oneshot     , DSS_NODE        , 5, DSE( RESET,TRIG,AMPL,WIDTH,NODE_NC ), DSE( RESET,TRIG,AMPL,WIDTH,TYPE ), NULL, "One Shot Resetable" ),
        //#define DISCRETE_ONOFF(NODE,ENAB,INP0)                                  DSC_SND_ENTRY( NODE, dst_gain        , DSS_NODE        , 3, DSE( ENAB,INP0,NODE_NC ), DSE( 0,1,0 ), NULL, "DISCRETE_ONOFF" ),
        public static discrete_block DISCRETE_RAMP(int NODE, double ENAB, double RAMP, double GRAD, double START, double END, double CLAMP) { return DSC_SND_ENTRY<discrete_dst_ramp_node>( NODE, (int)discrete_node_type.DSS_NODE        , 6, DSE( (int)ENAB,(int)RAMP,(int)GRAD,(int)START,(int)END,(int)CLAMP ), DSE( ENAB,RAMP,GRAD,START,END,CLAMP ), null, "DISCRETE_RAMP" ); }
        public static discrete_block DISCRETE_SAMPLHOLD(int NODE, double INP0, double CLOCK, double CLKTYPE) { return DSC_SND_ENTRY<discrete_dst_samphold_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)INP0,(int)CLOCK,NODE_NC ), DSE( INP0,CLOCK,CLKTYPE ), null, "DISCRETE_SAMPLHOLD" ); }
        //#define DISCRETE_SWITCH(NODE,ENAB,SWITCH,INP0,INP1)                     DSC_SND_ENTRY( NODE, dst_switch      , DSS_NODE        , 4, DSE( ENAB,SWITCH,INP0,INP1 ), DSE( ENAB,SWITCH,INP0,INP1 ), NULL, "DISCRETE_SWITCH" ),
        //#define DISCRETE_ASWITCH(NODE,CTRL,INP,THRESHOLD)                       DSC_SND_ENTRY( NODE, dst_aswitch     , DSS_NODE        , 3, DSE( CTRL,INP,THRESHOLD ), DSE( CTRL,INP, THRESHOLD), NULL, "Analog Switch" ),
        public static discrete_block DISCRETE_TRANSFORM2(int NODE, double INP0, double INP1, string FUNCT) { return DSC_SND_ENTRY<discrete_dst_transform_node>( NODE, (int)discrete_node_type.DSS_NODE        , 2, DSE( (int)INP0,(int)INP1 ), DSE( INP0,INP1 ), FUNCT, "DISCRETE_TRANSFORM2" ); }
        public static discrete_block DISCRETE_TRANSFORM3(int NODE, double INP0, double INP1, double INP2, string FUNCT) { return DSC_SND_ENTRY<discrete_dst_transform_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)INP0,(int)INP1,(int)INP2 ), DSE( INP0,INP1,INP2 ), FUNCT, "DISCRETE_TRANSFORM3" ); }
        public static discrete_block DISCRETE_TRANSFORM4(int NODE, double INP0, double INP1, double INP2, double INP3, string FUNCT) { return DSC_SND_ENTRY<discrete_dst_transform_node>( NODE, (int)discrete_node_type.DSS_NODE        , 4, DSE( (int)INP0,(int)INP1,(int)INP2,(int)INP3 ), DSE( INP0,INP1,INP2,INP3 ), FUNCT, "DISCRETE_TRANSFORM4" ); }
        public static discrete_block DISCRETE_TRANSFORM5(int NODE, double INP0, double INP1, double INP2, double INP3, double INP4, string FUNCT) {return DSC_SND_ENTRY<discrete_dst_transform_node>( NODE, (int)discrete_node_type.DSS_NODE        , 5, DSE( (int)INP0,(int)INP1,(int)INP2,(int)INP3,(int)INP4 ), DSE( INP0,INP1,INP2,INP3,INP4 ), FUNCT, "DISCRETE_TRANSFORM5" ); }
        /* Component specific */
        public static discrete_block DISCRETE_COMP_ADDER(int NODE, double DATA, discrete_comp_adder_table TABLE) { return DSC_SND_ENTRY<discrete_dst_comp_adder_node>( NODE, (int)discrete_node_type.DSS_NODE        , 1, DSE( (int)DATA ), DSE( DATA ), TABLE, "DISCRETE_COMP_ADDER" ); }
        public static discrete_block DISCRETE_DAC_R1(int NODE, double DATA, double VDATA, discrete_dac_r1_ladder LADDER) { return DSC_SND_ENTRY<discrete_dst_dac_r1_node>( NODE, (int)discrete_node_type.DSS_NODE        , 2, DSE( (int)DATA,NODE_NC ), DSE( DATA,VDATA ), LADDER, "DISCRETE_DAC_R1" ); }
        public static discrete_block DISCRETE_DIODE_MIXER2(int NODE, double IN0, double IN1, double [] TABLE) { return DSC_SND_ENTRY<discrete_dst_diode_mix_node>( NODE, (int)discrete_node_type.DSS_NODE        , 2, DSE( (int)IN0,(int)IN1 ), DSE( IN0,IN1 ), TABLE, "DISCRETE_DIODE_MIXER2" ); }
        //#define DISCRETE_DIODE_MIXER3(NODE,IN0,IN1,IN2,TABLE)                   DSC_SND_ENTRY( NODE, dst_diode_mix   , DSS_NODE        , 3, DSE( IN0,IN1,IN2 ), DSE( IN0,IN1,IN2 ), TABLE, "DISCRETE_DIODE_MIXER3" ),
        //#define DISCRETE_DIODE_MIXER4(NODE,IN0,IN1,IN2,IN3,TABLE)               DSC_SND_ENTRY( NODE, dst_diode_mix   , DSS_NODE        , 4, DSE( IN0,IN1,IN2,IN3 ), DSE( IN0,IN1,IN2,IN3 ), TABLE, "DISCRETE_DIODE_MIXER4" ),
        //#define DISCRETE_INTEGRATE(NODE,TRG0,TRG1,INFO)                         DSC_SND_ENTRY( NODE, dst_integrate   , DSS_NODE        , 2, DSE( TRG0,TRG1 ), DSE( TRG0,TRG1 ), INFO, "DISCRETE_INTEGRATE" ),
        public static discrete_block DISCRETE_MIXER2(int NODE, double ENAB, double IN0, double IN1, discrete_mixer_desc INFO) { return DSC_SND_ENTRY<discrete_dst_mixer_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)ENAB,(int)IN0,(int)IN1 ), DSE( ENAB,IN0,IN1 ), INFO, "DISCRETE_MIXER2" ); }
        public static discrete_block DISCRETE_MIXER3(int NODE, double ENAB, double IN0, double IN1, double IN2, discrete_mixer_desc INFO) { return DSC_SND_ENTRY<discrete_dst_mixer_node>( NODE, (int)discrete_node_type.DSS_NODE        , 4, DSE( (int)ENAB,(int)IN0,(int)IN1,(int)IN2 ), DSE( ENAB,IN0,IN1,IN2 ), INFO, "DISCRETE_MIXER3" ); }
        public static discrete_block DISCRETE_MIXER4(int NODE, double ENAB, double IN0, double IN1, double IN2, double IN3, discrete_mixer_desc INFO) { return DSC_SND_ENTRY<discrete_dst_mixer_node>( NODE, (int)discrete_node_type.DSS_NODE        , 5, DSE( (int)ENAB,(int)IN0,(int)IN1,(int)IN2,(int)IN3 ), DSE( ENAB,IN0,IN1,IN2,IN3 ), INFO, "DISCRETE_MIXER4" ); }
        public static discrete_block DISCRETE_MIXER5(int NODE, double ENAB, double IN0, double IN1, double IN2, double IN3, double IN4, discrete_mixer_desc INFO) { return DSC_SND_ENTRY<discrete_dst_mixer_node>( NODE, (int)discrete_node_type.DSS_NODE        , 6, DSE( (int)ENAB,(int)IN0,(int)IN1,(int)IN2,(int)IN3,(int)IN4 ), DSE( ENAB,IN0,IN1,IN2,IN3,IN4 ), INFO, "DISCRETE_MIXER5" ); }
        public static discrete_block DISCRETE_MIXER6(int NODE, double ENAB, double IN0, double IN1, double IN2, double IN3, double IN4, double IN5, discrete_mixer_desc INFO) { return DSC_SND_ENTRY<discrete_dst_mixer_node>( NODE, (int)discrete_node_type.DSS_NODE        , 7, DSE( (int)ENAB,(int)IN0,(int)IN1,(int)IN2,(int)IN3,(int)IN4,(int)IN5 ), DSE( ENAB,IN0,IN1,IN2,IN3,IN4,IN5 ), INFO, "DISCRETE_MIXER6" ); }
        //#define DISCRETE_MIXER7(NODE,ENAB,IN0,IN1,IN2,IN3,IN4,IN5,IN6,INFO)     DSC_SND_ENTRY( NODE, dst_mixer       , DSS_NODE        , 8, DSE( ENAB,IN0,IN1,IN2,IN3,IN4,IN5,IN6 ), DSE( ENAB,IN0,IN1,IN2,IN3,IN4,IN5,IN6 ), INFO, "DISCRETE_MIXER7" ),
        //#define DISCRETE_MIXER8(NODE,ENAB,IN0,IN1,IN2,IN3,IN4,IN5,IN6,IN7,INFO) DSC_SND_ENTRY( NODE, dst_mixer       , DSS_NODE        , 9, DSE( ENAB,IN0,IN1,IN2,IN3,IN4,IN5,IN6,IN7 ), DSE( ENAB,IN0,IN1,IN2,IN3,IN4,IN5,IN6,IN7 ), INFO, "DISCRETE_MIXER8" ),
        public static discrete_block DISCRETE_OP_AMP(int NODE, double ENAB, double IN0, double IN1, discrete_op_amp_info INFO) { return DSC_SND_ENTRY<discrete_dst_op_amp_node>( NODE,       (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)ENAB,(int)IN0,(int)IN1 ), DSE( ENAB,IN0,IN1 ), INFO, "DISCRETE_OP_AMP" ); }
        public static discrete_block DISCRETE_OP_AMP_ONESHOT(int NODE, double TRIG, discrete_op_amp_1sht_info INFO) { return DSC_SND_ENTRY<discrete_dst_op_amp_1sht_node>( NODE, (int)discrete_node_type.DSS_NODE        , 1, DSE( (int)TRIG ), DSE( TRIG ), INFO, "DISCRETE_OP_AMP_ONESHOT" ); }
        public static discrete_block DISCRETE_OP_AMP_TRIG_VCA(int NODE, double TRG0, double TRG1, double TRG2, double IN0, double IN1, discrete_op_amp_tvca_info INFO) { return DSC_SND_ENTRY<discrete_dst_tvca_op_amp_node>( NODE,  (int)discrete_node_type.DSS_NODE        , 5, DSE( (int)TRG0,(int)TRG1,(int)TRG2,(int)IN0,(int)IN1 ), DSE( TRG0,TRG1,TRG2,IN0,IN1 ), INFO, "DISCRETE_OP_AMP_TRIG_VCA" ); }
        //#define DISCRETE_VCA(NODE,ENAB,IN0,CTRL,TYPE)                           DSC_SND_ENTRY( NODE, dst_vca         , DSS_NODE        , 4, DSE( ENAB,IN0,CTRL,NODE_NC ), DSE( ENAB,IN0,CTRL,TYPE ), NULL, "DISCRETE_VCA" ),
        //#define DISCRETE_XTIME_BUFFER(NODE,IN0,LOW,HIGH)                        DSC_SND_ENTRY( NODE, dst_xtime_buffer, DSS_NODE        , 4, DSE( IN0,LOW,HIGH,NODE_NC ), DSE( IN0,LOW,HIGH,0 ), NULL, "DISCRETE_XTIME_BUFFER" ),
        //#define DISCRETE_XTIME_INVERTER(NODE,IN0,LOW,HIGH)                      DSC_SND_ENTRY( NODE, dst_xtime_buffer, DSS_NODE        , 4, DSE( IN0,LOW,HIGH,NODE_NC ), DSE( IN0,LOW,HIGH,1 ), NULL, "DISCRETE_XTIME_INVERTER" ),
        //#define DISCRETE_XTIME_AND(NODE,IN0,IN1,LOW,HIGH)                       DSC_SND_ENTRY( NODE, dst_xtime_and   , DSS_NODE        , 5, DSE( IN0,IN1,LOW,HIGH,NODE_NC ), DSE( IN0,IN1,LOW,HIGH,0 ), NULL, "DISCRETE_XTIME_AND" ),
        //#define DISCRETE_XTIME_NAND(NODE,IN0,IN1,LOW,HIGH)                      DSC_SND_ENTRY( NODE, dst_xtime_and   , DSS_NODE        , 5, DSE( IN0,IN1,LOW,HIGH,NODE_NC ), DSE( IN0,IN1,LOW,HIGH,1 ), NULL, "DISCRETE_XTIME_NAND" ),
        //#define DISCRETE_XTIME_OR(NODE,IN0,IN1,LOW,HIGH)                        DSC_SND_ENTRY( NODE, dst_xtime_or    , DSS_NODE        , 5, DSE( IN0,IN1,LOW,HIGH,NODE_NC ), DSE( IN0,IN1,LOW,HIGH,0 ), NULL, "DISCRETE_XTIME_OR" ),
        //#define DISCRETE_XTIME_NOR(NODE,IN0,IN1,LOW,HIGH)                       DSC_SND_ENTRY( NODE, dst_xtime_or    , DSS_NODE        , 5, DSE( IN0,IN1,LOW,HIGH,NODE_NC ), DSE( IN0,IN1,LOW,HIGH,1 ), NULL, "DISCRETE_XTIME_NOR" ),
        //#define DISCRETE_XTIME_XOR(NODE,IN0,IN1,LOW,HIGH)                       DSC_SND_ENTRY( NODE, dst_xtime_xor   , DSS_NODE        , 5, DSE( IN0,IN1,LOW,HIGH,NODE_NC ), DSE( IN0,IN1,LOW,HIGH,0 ), NULL, "DISCRETE_XTIME_XOR" ),
        //#define DISCRETE_XTIME_XNOR(NODE,IN0,IN1,LOW,HIGH)                      DSC_SND_ENTRY( NODE, dst_xtime_xnor  , DSS_NODE        , 5, DSE( IN0,IN1,LOW,HIGH,NODE_NC ), DSE( IN0,IN1,LOW,HIGH,1 ), NULL, "DISCRETE_XTIME_XNOR" ),

        /* from disc_flt.inc */
        /* generic modules */
        public static discrete_block DISCRETE_FILTER1(int NODE, double ENAB, double INP0, double FREQ, double TYPE) { return DSC_SND_ENTRY<discrete_dst_filter1_node>( NODE, (int)discrete_node_type.DSS_NODE        , 4, DSE( (int)ENAB,(int)INP0,NODE_NC,NODE_NC ), DSE( ENAB,INP0,FREQ,TYPE ), null, "DISCRETE_FILTER1" ); }
        public static discrete_block DISCRETE_FILTER2(int NODE, double ENAB, double INP0, double FREQ, double DAMP, double TYPE) { return DSC_SND_ENTRY<discrete_dst_filter2_node>( NODE, (int)discrete_node_type.DSS_NODE        , 5, DSE( (int)ENAB,(int)INP0,NODE_NC,NODE_NC,NODE_NC ), DSE( ENAB,INP0,FREQ,DAMP,TYPE ), null, "DISCRETE_FILTER2" ); }
        /* Component specific */
        public static discrete_block DISCRETE_SALLEN_KEY_FILTER(int NODE, double ENAB, double INP0, double TYPE, discrete_op_amp_filt_info INFO) { return DSC_SND_ENTRY<discrete_dst_sallen_key_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)ENAB,(int)INP0,NODE_NC ), DSE( ENAB,INP0,TYPE ), INFO, "DISCRETE_SALLEN_KEY_FILTER" ); }
        public static discrete_block DISCRETE_CRFILTER(int NODE, double INP0, double RVAL, double CVAL) { return DSC_SND_ENTRY<discrete_dst_crfilter_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)INP0,OPT_NODE(RVAL),OPT_NODE(CVAL) ), DSE( INP0,RVAL,CVAL ), null, "DISCRETE_CRFILTER" ); }
        //#define DISCRETE_CRFILTER_VREF(NODE,INP0,RVAL,CVAL,VREF)                DSC_SND_ENTRY( NODE, dst_crfilter    , DSS_NODE        , 4, DSE( INP0,OPT_NODE(RVAL),OPT_NODE(CVAL),VREF ), DSE( INP0,RVAL,CVAL,VREF ), NULL, "DISCRETE_CRFILTER_VREF" ),
        public static discrete_block DISCRETE_OP_AMP_FILTER(int NODE, double ENAB, double INP0, double INP1, double TYPE, discrete_op_amp_filt_info INFO) { return DSC_SND_ENTRY<discrete_dst_op_amp_filt_node>( NODE, (int)discrete_node_type.DSS_NODE        , 4, DSE( (int)ENAB,(int)INP0,(int)INP1,NODE_NC ), DSE( ENAB,INP0,INP1,TYPE ), INFO, "DISCRETE_OP_AMP_FILTER" ); }
        //#define DISCRETE_RC_CIRCUIT_1(NODE,INP0,INP1,RVAL,CVAL)                 DSC_SND_ENTRY( NODE, dst_rc_circuit_1, DSS_NODE        , 4, DSE( INP0,INP1,NODE_NC,NODE_NC ), DSE( INP0,INP1,RVAL,CVAL ), NULL, "DISCRETE_RC_CIRCUIT_1" ),
        public static discrete_block DISCRETE_RCDISC(int NODE, double ENAB, double INP0, double RVAL, double CVAL) { return DSC_SND_ENTRY<discrete_dst_rcdisc_node>( NODE, (int)discrete_node_type.DSS_NODE        , 4, DSE( (int)ENAB,(int)INP0,NODE_NC,NODE_NC ), DSE( ENAB,INP0,RVAL,CVAL ), null, "DISCRETE_RCDISC" ); }
        public static discrete_block DISCRETE_RCDISC2(int NODE, double SWITCH, double INP0, double RVAL0, double INP1, double RVAL1, double CVAL) { return DSC_SND_ENTRY<discrete_dst_rcdisc2_node>( NODE, (int)discrete_node_type.DSS_NODE        , 6, DSE( (int)SWITCH,(int)INP0,NODE_NC,(int)INP1,NODE_NC,NODE_NC ), DSE( SWITCH,INP0,RVAL0,INP1,RVAL1,CVAL ), null, "DISCRETE_RCDISC2" ); }
        //#define DISCRETE_RCDISC3(NODE,ENAB,INP0,RVAL0,RVAL1,CVAL,DJV)           DSC_SND_ENTRY( NODE, dst_rcdisc3     , DSS_NODE        , 6, DSE( ENAB,INP0,NODE_NC,NODE_NC,NODE_NC,NODE_NC ), DSE( ENAB,INP0,RVAL0,RVAL1,CVAL,DJV ), NULL, "DISCRETE_RCDISC3" ),
        //#define DISCRETE_RCDISC4(NODE,ENAB,INP0,RVAL0,RVAL1,RVAL2,CVAL,VP,TYPE) DSC_SND_ENTRY( NODE, dst_rcdisc4     , DSS_NODE        , 8, DSE( ENAB,INP0,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC ), DSE( ENAB,INP0,RVAL0,RVAL1,RVAL2,CVAL,VP,TYPE ), NULL, "DISCRETE_RCDISC4" ),
        public static discrete_block DISCRETE_RCDISC5(int NODE, double ENAB, double INP0, double RVAL, double CVAL) { return DSC_SND_ENTRY<discrete_dst_rcdisc5_node>( NODE, (int)discrete_node_type.DSS_NODE        , 4, DSE( (int)ENAB,(int)INP0,NODE_NC,NODE_NC ), DSE( ENAB,INP0,RVAL,CVAL ), null, "DISCRETE_RCDISC5" ); }
        public static discrete_block DISCRETE_RCDISC_MODULATED(int NODE, double INP0, double INP1, double RVAL0, double RVAL1, double RVAL2, double RVAL3, double CVAL, double VP) { return DSC_SND_ENTRY<discrete_dst_rcdisc_mod_node>( NODE, (int)discrete_node_type.DSS_NODE        , 8, DSE( (int)INP0,(int)INP1,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC ), DSE( INP0,INP1,RVAL0,RVAL1,RVAL2,RVAL3,CVAL,VP ), null, "DISCRETE_RCDISC_MODULATED" ); }
        public static discrete_block DISCRETE_RCFILTER(int NODE, double INP0, double RVAL, double CVAL) { return DSC_SND_ENTRY<discrete_dst_rcfilter_node>( NODE, (int)discrete_node_type.DSS_NODE        , 3, DSE( (int)INP0,OPT_NODE(RVAL),OPT_NODE(CVAL) ), DSE( INP0,RVAL,CVAL ), null, "DISCRETE_RCFILTER" ); }
        //#define DISCRETE_RCFILTER_VREF(NODE,INP0,RVAL,CVAL,VREF)                DSC_SND_ENTRY( NODE, dst_rcfilter    , DSS_NODE        , 4, DSE( INP0,OPT_NODE(RVAL),OPT_NODE(CVAL),VREF ), DSE( INP0,RVAL,CVAL,VREF ), NULL, "DISCRETE_RCFILTER_VREF" ),
        public static discrete_block DISCRETE_RCFILTER_SW(int NODE, double ENAB, double INP0, double SW, double RVAL, double CVAL1, double CVAL2, double CVAL3, double CVAL4) { return DSC_SND_ENTRY<discrete_dst_rcfilter_sw_node>( NODE, (int)discrete_node_type.DSS_NODE    , 8, DSE( (int)ENAB,(int)INP0,(int)SW,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC ), DSE( ENAB,INP0,SW,RVAL,CVAL1,CVAL2,CVAL3,CVAL4 ), null, "DISCRETE_RCFILTER_SW" ); }
        public static discrete_block DISCRETE_RCINTEGRATE(int NODE, double INP0, double RVAL0, double RVAL1, double RVAL2, double CVAL, double vP, double TYPE) { return DSC_SND_ENTRY<discrete_dst_rcintegrate_node>( NODE, (int)discrete_node_type.DSS_NODE        , 7, DSE( (int)INP0,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC ), DSE( INP0,RVAL0,RVAL1,RVAL2,CVAL,vP,TYPE ), null, "DISCRETE_RCINTEGRATE" ); }
        /* For testing - seem to be buggered.  Use versions not ending in N. */
        //#define DISCRETE_RCDISCN(NODE,ENAB,INP0,RVAL,CVAL)                      DSC_SND_ENTRY( NODE, dst_rcdiscn     , DSS_NODE        , 4, DSE( ENAB,INP0,NODE_NC,NODE_NC ), DSE( ENAB,INP0,RVAL,CVAL ), NULL, "DISCRETE_RCDISCN" ),
        //#define DISCRETE_RCDISC2N(NODE,SWITCH,INP0,RVAL0,INP1,RVAL1,CVAL)       DSC_SND_ENTRY( NODE, dst_rcdisc2n    , DSS_NODE        , 6, DSE( SWITCH,INP0,NODE_NC,INP1,NODE_NC,NODE_NC ), DSE( SWITCH,INP0,RVAL0,INP1,RVAL1,CVAL ), NULL, "DISCRETE_RCDISC2N" ),
        //#define DISCRETE_RCFILTERN(NODE,ENAB,INP0,RVAL,CVAL)                    DSC_SND_ENTRY( NODE, dst_rcfiltern   , DSS_NODE        , 4, DSE( ENAB,INP0,NODE_NC,NODE_NC ), DSE( ENAB,INP0,RVAL,CVAL ), NULL, "DISCRETE_RCFILTERN" ),

        /* from disc_dev.inc */
        /* generic modules */
        //#define DISCRETE_CUSTOM1(NODE,CLASS,IN0,INFO)                                 DSC_SND_ENTRY( NODE, CLASS, DST_CUSTOM      , 1, DSE( IN0 ), DSE( IN0 ), INFO, "DISCRETE_CUSTOM1" ),
        //#define DISCRETE_CUSTOM2(NODE,CLASS,IN0,IN1,INFO)                             DSC_SND_ENTRY( NODE, CLASS, DST_CUSTOM      , 2, DSE( IN0,IN1 ), DSE( IN0,IN1 ), INFO, "DISCRETE_CUSTOM2" ),
        //#define DISCRETE_CUSTOM3(NODE,CLASS,IN0,IN1,IN2,INFO)                         DSC_SND_ENTRY( NODE, CLASS, DST_CUSTOM      , 3, DSE( IN0,IN1,IN2 ), DSE( IN0,IN1,IN2 ), INFO, "DISCRETE_CUSTOM3" ),
        //#define DISCRETE_CUSTOM4(NODE,CLASS,IN0,IN1,IN2,IN3,INFO)                     DSC_SND_ENTRY( NODE, CLASS, DST_CUSTOM      , 4, DSE( IN0,IN1,IN2,IN3 ), DSE( IN0,IN1,IN2,IN3 ), INFO, "DISCRETE_CUSTOM4" ),
        //#define DISCRETE_CUSTOM5(NODE,CLASS,IN0,IN1,IN2,IN3,IN4,INFO)                 DSC_SND_ENTRY( NODE, CLASS, DST_CUSTOM      , 5, DSE( IN0,IN1,IN2,IN3,IN4 ), DSE( IN0,IN1,IN2,IN3,IN4 ), INFO, "DISCRETE_CUSTOM5" ),
        //#define DISCRETE_CUSTOM6(NODE,CLASS,IN0,IN1,IN2,IN3,IN4,IN5,INFO)             DSC_SND_ENTRY( NODE, CLASS, DST_CUSTOM      , 6, DSE( IN0,IN1,IN2,IN3,IN4,IN5 ), DSE( IN0,IN1,IN2,IN3,IN4,IN5 ), INFO, "DISCRETE_CUSTOM6" ),
        //#define DISCRETE_CUSTOM7(NODE,CLASS,IN0,IN1,IN2,IN3,IN4,IN5,IN6,INFO)         DSC_SND_ENTRY( NODE, CLASS, DST_CUSTOM      , 7, DSE( IN0,IN1,IN2,IN3,IN4,IN5,IN6 ), DSE( IN0,IN1,IN2,IN3,IN4,IN5,IN6 ), INFO, "DISCRETE_CUSTOM7" ),
        public static discrete_block DISCRETE_CUSTOM8<CLASS>(int NODE, double IN0, double IN1, double IN2, double IN3, double IN4, double IN5, double IN6, double IN7, object INFO) where CLASS : discrete_base_node, new() { return DSC_SND_ENTRY<CLASS>( NODE, (int)discrete_node_type.DST_CUSTOM      , 8, DSE( (int)IN0,(int)IN1,(int)IN2,(int)IN3,(int)IN4,(int)IN5,(int)IN6,(int)IN7 ), DSE( IN0,IN1,IN2,IN3,IN4,IN5,IN6,IN7 ), INFO, "DISCRETE_CUSTOM8" ); }
        //#define DISCRETE_CUSTOM9(NODE,CLASS,IN0,IN1,IN2,IN3,IN4,IN5,IN6,IN7,IN8,INFO) DSC_SND_ENTRY( NODE, CLASS, DST_CUSTOM      , 9, DSE( IN0,IN1,IN2,IN3,IN4,IN5,IN6,IN7,IN8 ), DSE( IN0,IN1,IN2,IN3,IN4,IN5,IN6,IN7,IN8 ), INFO, "DISCRETE_CUSTOM9" ),

        /* Component specific */
        public static discrete_block DISCRETE_555_ASTABLE(int NODE, double RESET, double R1, double R2, double C, discrete_555_desc OPTIONS) { return DSC_SND_ENTRY<discrete_dsd_555_astbl_node>( NODE,    (int)discrete_node_type.DSS_NODE        , 5, DSE( (int)RESET,(int)R1,(int)R2,(int)C,NODE_NC ), DSE( RESET,R1,R2,C,-1 ), OPTIONS, "DISCRETE_555_ASTABLE" ); }
        public static discrete_block DISCRETE_555_ASTABLE_CV(int NODE, double RESET, double R1, double R2, double C, double CTRLV, discrete_555_desc OPTIONS) { return DSC_SND_ENTRY<discrete_dsd_555_astbl_node>( NODE, (int)discrete_node_type.DSS_NODE        , 5, DSE( (int)RESET,(int)R1,(int)R2,(int)C,(int)CTRLV ), DSE( RESET,R1,R2,C,CTRLV ), OPTIONS, "DISCRETE_555_ASTABLE_CV" ); }
        //#define DISCRETE_555_MSTABLE(NODE,RESET,TRIG,R,C,OPTIONS)               DSC_SND_ENTRY( NODE, dsd_555_mstbl   , DSS_NODE        , 4, DSE( RESET,TRIG,R,C ), DSE( RESET,TRIG,R,C ), OPTIONS, "DISCRETE_555_MSTABLE" ),
        public static discrete_block DISCRETE_555_CC(int NODE, double RESET, double VIN, double R, double C, double RBIAS, double RGND, double RDIS, discrete_555_cc_desc OPTIONS) { return DSC_SND_ENTRY<discrete_dsd_555_cc_node>( NODE, (int)discrete_node_type.DSS_NODE        , 7, DSE( (int)RESET,(int)VIN,(int)R,(int)C,(int)RBIAS,(int)RGND,(int)RDIS ), DSE( RESET,VIN,R,C,RBIAS,RGND,RDIS ), OPTIONS, "DISCRETE_555_CC" ); }
        //#define DISCRETE_555_VCO1(NODE,RESET,VIN,OPTIONS)                       DSC_SND_ENTRY( NODE, dsd_555_vco1    , DSS_NODE        , 3, DSE( RESET,VIN,NODE_NC ), DSE( RESET,VIN,-1 ), OPTIONS, "DISCRETE_555_VCO1" ),
        //#define DISCRETE_555_VCO1_CV(NODE,RESET,VIN,CTRLV,OPTIONS)              DSC_SND_ENTRY( NODE, dsd_555_vco1    , DSS_NODE        , 3, DSE( RESET,VIN,CTRLV ), DSE( RESET,VIN,CTRLV ), OPTIONS, "DISCRETE_555_VCO1_CV" ),
        //#define DISCRETE_566(NODE,VMOD,R,C,VPOS,VNEG,VCHARGE,OPTIONS)           DSC_SND_ENTRY( NODE, dsd_566         , DSS_NODE        , 7, DSE( VMOD,R,C,NODE_NC,NODE_NC,VCHARGE,NODE_NC ), DSE( VMOD,R,C,VPOS,VNEG,VCHARGE,OPTIONS ), NULL, "DISCRETE_566" ),
        //#define DISCRETE_74LS624(NODE,ENAB,VMOD,VRNG,C,R_FREQ_IN,C_FREQ_IN,R_RNG_IN,OUTTYPE) DSC_SND_ENTRY( NODE, dsd_ls624   , DSS_NODE        , 8, DSE( ENAB,VMOD,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC,NODE_NC ), DSE( ENAB,VMOD,VRNG,C,R_FREQ_IN,C_FREQ_IN,R_RNG_IN,OUTTYPE ), NULL, "DISCRETE_74LS624" ),

        /* NOP */
        //#define DISCRETE_NOP(NODE)                                              DSC_SND_ENTRY( NODE, dss_nop         , DSS_NOP         , 0, DSE( 0 ), DSE( 0 ), NULL, "DISCRETE_NOP" ),

        /* logging */
        //#define DISCRETE_CSVLOG1(NODE1)                                         DSC_SND_ENTRY( NODE_SPECIAL, dso_csvlog  , DSO_CSVLOG  , 1, DSE( NODE1 ), DSE( NODE1 ), NULL, "DISCRETE_CSVLOG1" ),
        //#define DISCRETE_CSVLOG2(NODE1,NODE2)                                   DSC_SND_ENTRY( NODE_SPECIAL, dso_csvlog  , DSO_CSVLOG  , 2, DSE( NODE1,NODE2 ), DSE( NODE1,NODE2 ), NULL, "DISCRETE_CSVLOG2" ),
        //#define DISCRETE_CSVLOG3(NODE1,NODE2,NODE3)                             DSC_SND_ENTRY( NODE_SPECIAL, dso_csvlog  , DSO_CSVLOG  , 3, DSE( NODE1,NODE2,NODE3 ), DSE( NODE1,NODE2,NODE3 ), NULL, "DISCRETE_CSVLOG3" ),
        //#define DISCRETE_CSVLOG4(NODE1,NODE2,NODE3,NODE4)                       DSC_SND_ENTRY( NODE_SPECIAL, dso_csvlog  , DSO_CSVLOG  , 4, DSE( NODE1,NODE2,NODE3,NODE4 ), DSE( NODE1,NODE2,NODE3,NODE4 ), NULL, "DISCRETE_CSVLOG4" ),
        //#define DISCRETE_CSVLOG5(NODE1,NODE2,NODE3,NODE4,NODE5)                 DSC_SND_ENTRY( NODE_SPECIAL, dso_csvlog  , DSO_CSVLOG  , 5, DSE( NODE1,NODE2,NODE3,NODE4,NODE5 ), DSE( NODE1,NODE2,NODE3,NODE4,NODE5 ), NULL, "DISCRETE_CSVLOG5" ),
        //#define DISCRETE_WAVLOG1(NODE1,GAIN1)                                   DSC_SND_ENTRY( NODE_SPECIAL, dso_wavlog  , DSO_WAVLOG  , 2, DSE( NODE1,NODE_NC ), DSE( NODE1,GAIN1 ), NULL, "DISCRETE_WAVLOG1" ),
        //#define DISCRETE_WAVLOG2(NODE1,GAIN1,NODE2,GAIN2)                       DSC_SND_ENTRY( NODE_SPECIAL, dso_wavlog  , DSO_WAVLOG  , 4, DSE( NODE1,NODE_NC,NODE2,NODE_NC ), DSE( NODE1,GAIN1,NODE2,GAIN2 ), NULL, "DISCRETE_WAVLOG2" ),

        /* import */
        //#define DISCRETE_IMPORT(INFO)                                           DSC_SND_ENTRY( NODE_SPECIAL, special     , DSO_IMPORT  , 0, DSE( 0 ), DSE( 0 ), &(INFO), "DISCRETE_IMPORT" ),
        //#define DISCRETE_DELETE(NODE_FROM, NODE_TO)                             DSC_SND_ENTRY( NODE_SPECIAL, special     , DSO_DELETE  , 2, DSE( NODE_FROM, NODE_TO ), DSE( NODE_FROM, NODE_TO ), NULL, "DISCRETE_DELETE" ),
        //#define DISCRETE_REPLACE                                                DSC_SND_ENTRY( NODE_SPECIAL, special     , DSO_REPLACE , 0, DSE( 0 ), DSE( 0 ), NULL, "DISCRETE_REPLACE" ),

        /* parallel tasks */

        public static discrete_block DISCRETE_TASK_START(double TASK_GROUP) { return DSC_SND_ENTRY<discrete_special_node>( NODE_SPECIAL, (int)discrete_node_type.DSO_TASK_START, 2, DSE( NODE_NC, NODE_NC ), DSE( TASK_GROUP, 0 ), null, "DISCRETE_TASK_START" ); }
        public static discrete_block DISCRETE_TASK_END() { return DSC_SND_ENTRY<discrete_special_node>( NODE_SPECIAL, (int)discrete_node_type.DSO_TASK_END  , 0, DSE( 0 ), DSE( 0.0 ), null, "DISCRETE_TASK_END" ); }
        //#define DISCRETE_TASK_SYNC()                                          DSC_SND_ENTRY( NODE_SPECIAL, special     , DSO_TASK_SYNC , 0, DSE( 0 ), DSE( 0 ), NULL, "DISCRETE_TASK_SYNC" ),

        /* output */
        public static discrete_block DISCRETE_OUTPUT(double OPNODE, double GAIN) { return DSC_SND_ENTRY<discrete_dso_output_node>( NODE_SPECIAL, (int)discrete_node_type.DSO_OUTPUT   ,2, DSE( (int)OPNODE,NODE_NC ), DSE( 0,GAIN ), null, "DISCRETE_OUTPUT" ); }
    }


    static class discrete_internal
    {
        /*************************************
         *
         *  Performance
         *
         *************************************/

        /*
         * Normally, the discrete core processes 960 samples per update.
         * With the various buffers involved, this on a Core2 is not as
         * performant as processing 240 samples 4 times.
         * The setting most probably depends on CPU and which modules are
         * run and how many tasks are defined.
         *
         * Values < 32 exhibit poor performance (too much overhead) while
         * Values > 500 have a slightly worse performace (too much cache misses?).
         */

        public const int MAX_SAMPLES_PER_TASK_SLICE = 960 / 4;

        /*************************************
         *
         *  Debugging
         *
         *************************************/

        public const int DISCRETE_DEBUGLOG = 0;

        /*************************************
         *
         *  Use tasks ?
         *
         *************************************/

        public const int USE_DISCRETE_TASKS = 1;
    }


    class output_buffer
    {
        public MemoryContainer<double> node_buf;  //std::unique_ptr<double []>  node_buf;
        public PointerRef<double> source;  //const double                *source;
        public Pointer<double> ptr;  //volatile double             *ptr;
        public int node_num;
    }


    class input_buffer
    {
        public Pointer<double> ptr;  //volatile const double       *ptr;               /* pointer into linked_outbuf.nodebuf */
        public output_buffer linked_outbuf;  //output_buffer *             linked_outbuf;      /* what output are we connected to ? */
        public PointerRef<double> buffer;  //double                      buffer;             /* input[] will point here */
    }


    class discrete_task
    {
        //friend class discrete_device;


        public discrete_device_node_step_list_t step_list = new discrete_device_node_step_list_t();

        /* list of source nodes */
        std.vector<input_buffer> source_list = new std.vector<input_buffer>();      /* discrete_source_node */

        public int task_group = 0;


        std.vector<output_buffer> m_buffers = new std.vector<output_buffer>();
        discrete_device m_device;

        int32_t m_threadid;  //std::atomic<int32_t>      m_threadid;
        int m_samples = 0;  //volatile int            m_samples = 0;


        public discrete_task(discrete_device pdev)
        {
            m_device = pdev;
            m_threadid = -1;


            // FIXME: the code expects to be able to take pointers to members of elements of this vector before it's filled
            source_list.reserve(16);
        }

        //~discrete_task() { }


        /*************************************
         *
         *  Task implementation
         *
         *************************************/

        void step_nodes()
        {
            foreach (input_buffer sn in source_list)
            {
                sn.buffer.m_pointer = new Pointer<double>(new MemoryContainer<double>(new double [] { sn.ptr[0] }));  //sn.buffer = *sn.ptr++;
                sn.ptr++;
            }

            if (m_device.profiling() == 0)
            {
                foreach (discrete_step_interface entry in step_list)
                {
                    /* Now step the node */
                    entry.step();
                }
            }
            else
            {
                osd_ticks_t last = (osd_ticks_t)get_profile_ticks();

                foreach (discrete_step_interface node in step_list)
                {
                    node.run_time -= last;
                    node.step();
                    last = (osd_ticks_t)get_profile_ticks();
                    node.run_time += last;
                }
            }

            /* buffer the outputs */
            foreach (output_buffer outbuf in m_buffers)
            {
                outbuf.ptr.Buffer[0] = outbuf.source.m_pointer[0];  //*(outbuf.ptr++) = *outbuf.source;
                outbuf.ptr++;
            }
        }

        bool lock_threadid(int32_t threadid)
        {
            int expected = -1;
            int prev_id = System.Threading.Interlocked.CompareExchange(ref m_threadid, threadid, expected);
            return prev_id == -1 && m_threadid == threadid;
        }

        public void unlock() { m_threadid = -1; }


        public static Object task_callback(Object param, int threadid)
        {
            var list = (discrete_device_task_list_t)param;  //const auto &list = *reinterpret_cast<const discrete_sound_device::task_list_t *>(param);

            do
            {
                foreach (var task in list)
                {
                    /* try to lock */
                    if (task.lock_threadid(threadid))
                    {
                        if (!task.process())
                            return null;

                        task.unlock();
                    }
                }
            } while (true);

            return null;
        }


        bool process()
        {
            int samples = std.min(m_samples, MAX_SAMPLES_PER_TASK_SLICE);

            /* check dependencies */
            foreach (input_buffer sn in source_list)
            {
                // make sure buffer pointers are equal, so we can perform math operation on the offsets
                assert(sn.linked_outbuf.ptr.Buffer == sn.ptr.Buffer);

                int avail = sn.linked_outbuf.ptr.Offset - sn.ptr.Offset;  // avail = sn.linked_outbuf.ptr - sn.ptr;

                if (avail < 0)
                    throw new emu_fatalerror("discrete_task::process: available samples are negative");

                if (avail < samples)
                    samples = avail;
            }

            m_samples -= samples;

            if (m_samples < 0)
                throw new emu_fatalerror("discrete_task::process: m_samples got negative");

            while (samples > 0)
            {
                /* step */
                step_nodes();
                samples--;
            }

            if (m_samples == 0)
            {
                /* return and keep the task locked so it is not picked up by other worker threads */
                return false;
            }

            return true;
        }


        public void check(discrete_task dest_task)
        {
            // FIXME: this function takes addresses of elements of a vector that has items added later
            // 16 is enough for the systems in MAME, but the code should be fixed properly
            m_buffers.reserve(16);

            /* Determine, which nodes in the task are referenced by nodes in dest_task
             * and add them to the list of nodes to be buffered for further processing
             */
            foreach (discrete_step_interface node_entry in step_list)
            {
                discrete_base_node task_node = node_entry.self;

                foreach (discrete_step_interface step_entry in dest_task.step_list)
                {
                    discrete_base_node dest_node = step_entry.self;

                    /* loop over all active inputs */
                    for (int inputnum = 0; inputnum < dest_node.active_inputs(); inputnum++)
                    {
                        int inputnode_num = dest_node.input_node(inputnum);
                        if (IS_VALUE_A_NODE(inputnode_num))
                        {
                            /* Fixme: sub nodes ! */
                            if (NODE_DEFAULT_NODE(task_node.block_node()) == NODE_DEFAULT_NODE(inputnode_num))
                            {
                                int found = -1;
                                output_buffer pbuf = null;

                                for (int i = 0; i < (int)m_buffers.size(); i++)
                                {
                                    //if (m_buffers[i].node->block_node() == inputnode_num)
                                    if (m_buffers[i].node_num == inputnode_num)
                                    {
                                        found = i;
                                        pbuf = m_buffers[i];
                                        break;
                                    }
                                }

                                if (found < 0)
                                {
                                    output_buffer buf = new output_buffer();

                                    buf.node_buf = new MemoryContainer<double>((task_node.sample_rate() + sound_manager.STREAMS_UPDATE_FREQUENCY) / sound_manager.STREAMS_UPDATE_FREQUENCY);  //buf.node_buf = std::make_unique<double []>((task_node->sample_rate() + sound_manager::STREAMS_UPDATE_FREQUENCY) / sound_manager::STREAMS_UPDATE_FREQUENCY);
                                    buf.ptr = new Pointer<double>(buf.node_buf);  //buf.ptr = buf.node_buf;
                                    buf.source = dest_node.m_input[inputnum];  //buf.source = dest_node->m_input[inputnum];
                                    buf.node_num = inputnode_num;
                                    //buf.node = device->discrete_find_node(inputnode);
                                    m_buffers.push_back(buf);
                                    pbuf = m_buffers.back();
                                }

                                m_device.discrete_log("dso_task_start - buffering {0}({1}) in task {2} group {3} referenced by {4} group {5}", NODE_INDEX(inputnode_num), NODE_CHILD_NODE_NUM(inputnode_num), this, task_group, dest_node.index(), dest_task.task_group);

                                /* register into source list */
                                dest_task.source_list.push_back(new input_buffer() { ptr = null, linked_outbuf = pbuf, buffer = new PointerRef<double>(new Pointer<double>(new MemoryContainer<double>(new double [] { 0.0 })))});
                                // FIXME: taking address of element of vector before it's filled
                                dest_node.m_input[inputnum] = dest_task.source_list.back().buffer;
                            }
                        }
                    }
                }
            }
        }

        public void prepare_for_queue(int samples)
        {
            m_samples = samples;

            /* set up task buffers */
            foreach (output_buffer ob in m_buffers)
            {
                ob.ptr = new Pointer<double>(ob.node_buf);
            }

            /* initialize sources */
            foreach (input_buffer sn in source_list)
            {
                sn.ptr = new Pointer<double>(sn.linked_outbuf.node_buf);
            }
        }
    }
}
