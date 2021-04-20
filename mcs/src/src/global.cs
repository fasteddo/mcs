// license:BSD-3-Clause
// copyright-holders:Edward Fast

//#define ASSERT_SLOW

using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using attoseconds_t = System.Int64;
using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;
using ioport_value = System.UInt32;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using nlsetup_func = System.Action<mame.netlist.nlparse_t>;  //using nlsetup_func = void (*)(nlparse_t &);
using offs_t = System.UInt32;
using pen_t = System.UInt32;
using s32 = System.Int32;
using size_t = System.UInt32;
using size_t_constant = mame.uint32_constant;
using std_time_t = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame
{
    public interface bool_constant { bool value { get; } }
    public class bool_constant_true : bool_constant { public bool value { get { return true; } } }
    public class bool_constant_false : bool_constant { public bool value { get { return false; } } }

    public interface int_constant { int value { get; } }
    public class int_constant_1 : int_constant { public int value { get { return 1; } } }
    public class int_constant_2 : int_constant { public int value { get { return 2; } } }

    public interface uint32_constant { UInt32 value { get; } }
    public class uint32_constant_2 : uint32_constant { public UInt32 value { get { return 2; } } }
    public class uint32_constant_4 : uint32_constant { public UInt32 value { get { return 4; } } }
    public class uint32_constant_5 : uint32_constant { public UInt32 value { get { return 5; } } }
    public class uint32_constant_8 : uint32_constant { public UInt32 value { get { return 8; } } }
    public class uint32_constant_16 : uint32_constant { public UInt32 value { get { return 16; } } }
    public class uint32_constant_20 : uint32_constant { public UInt32 value { get { return 20; } } }


    public class global_object
    {
        // these are in a seperate class so they don't clutter the debug window
        protected class helpers
        {
            machine_config m_helper_config = null;
            device_t m_helper_owner = null;
            public device_t m_helper_device = null;
            address_map_entry m_helper_curentry = null;
            address_map m_helper_map = null;
            ioport_configurer m_helper_configurer = null;
            ioport_list m_helper_portlist = null;
            bool m_helper_originated_from_port_include = false;
            Stack<netlist.nlparse_t> m_helper_setups = new Stack<netlist.nlparse_t>();

            public machine_config helper_config { get { return m_helper_config; } set { m_helper_config = value; } }
            public device_t helper_owner { get { return m_helper_owner; } set { m_helper_owner = value; } }
            public device_t helper_device { get { return m_helper_device; } set { m_helper_device = value; } }
            public address_map_entry helper_curentry { set { m_helper_curentry = value; } }
            public address_map helper_address_map { set { m_helper_map = value; } }
            public ioport_configurer helper_configurer { get { return m_helper_configurer; } set { m_helper_configurer = value; } }
            public ioport_list helper_portlist { get { return m_helper_portlist; } set { m_helper_portlist = value; } }
            public bool helper_originated_from_port_include { get { return m_helper_originated_from_port_include; } set { m_helper_originated_from_port_include = value; } }
            public netlist.nlparse_t helper_setup { get { return m_helper_setups.Peek(); } }
            public void helper_setup_push(netlist.nlparse_t setup) { m_helper_setups.Push(setup); }
            public void helper_setup_pop() { m_helper_setups.Pop(); }
        }

        protected helpers m_globals = new helpers();


        // _74259
        protected static ls259_device LS259(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<ls259_device>(mconfig, tag, ls259_device.LS259, clock); }
        protected static ls259_device LS259(machine_config mconfig, device_finder<ls259_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, ls259_device.LS259, clock); }


        // adc0808
        protected static adc0809_device ADC0809(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<adc0809_device>(mconfig, tag, adc0809_device.ADC0809, clock); }


        // atarimo
        protected static atari_motion_objects_device ATARI_MOTION_OBJECTS(machine_config mconfig, device_finder<atari_motion_objects_device> finder, uint32_t clock, device_finder<screen_device> screen_tag, atari_motion_objects_config config)
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, atari_motion_objects_device.ATARI_MOTION_OBJECTS, 0);
            device.atari_motion_objects_device_after_ctor(screen_tag, config);
            return device;
        }


        // attotime
        protected static attoseconds_t ATTOSECONDS_IN_USEC(u32 x) { return attotime.ATTOSECONDS_IN_USEC(x); }


        // ay8910
        protected const int AY8910_INTERNAL_RESISTANCE = ay8910_global.AY8910_INTERNAL_RESISTANCE;
        protected static ay8910_device AY8910(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<ay8910_device>(mconfig, tag, ay8910_device.AY8910, clock); }
        protected static ay8910_device AY8910(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<ay8910_device>(mconfig, tag, ay8910_device.AY8910, clock); }
        protected static ay8910_device AY8910(machine_config mconfig, device_finder<ay8910_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, ay8910_device.AY8910, clock); }
        protected static ay8910_device AY8910(machine_config mconfig, device_finder<ay8910_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, ay8910_device.AY8910, clock); }


        // bankdev
        protected static address_map_bank_device ADDRESS_MAP_BANK(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<address_map_bank_device>(mconfig, tag, address_map_bank_device.ADDRESS_MAP_BANK, 0); }


        // corefile
        protected static string core_filename_extract_base(string name, bool strip_extension = false) { return util.corefile_global.core_filename_extract_base(name, strip_extension); }
        protected static bool core_filename_ends_with(string filename, string extension) { return util.corefile_global.core_filename_ends_with(filename, extension); }
        protected static bool is_directory_separator(char c) { return util.corefile_global.is_directory_separator(c); }


        // corestr
        public static int core_stricmp(string s1, string s2) { return corestr_global.core_stricmp(s1, s2); }
        public static int core_strnicmp(string s1, string s2, size_t n) { return corestr_global.core_strnicmp(s1, s2, n); }
        protected static int core_strwildcmp(string sp1, string sp2) { return corestr_global.core_strwildcmp(sp1, sp2); }
        protected static bool core_iswildstr(string sp) { return corestr_global.core_iswildstr(sp); }
        protected static string strtrimspace(string str) { return corestr_global.strtrimspace(str); }
        public static int strreplace(ref string str, string search, string replace) { return corestr_global.strreplace(ref str, search, replace); }


        // coretmpl
        protected static u8 make_bitmask8(u32 n) { return coretmpl_global.make_bitmask8(n); }
        protected static u8 make_bitmask8(s32 n) { return coretmpl_global.make_bitmask8(n); }
        protected static u16 make_bitmask16(s32 n) { return coretmpl_global.make_bitmask16(n); }
        protected static u16 make_bitmask16(u32 n) { return coretmpl_global.make_bitmask16(n); }
        public static u32 make_bitmask32(s32 n) { return coretmpl_global.make_bitmask32(n); }
        protected static u32 make_bitmask32(u32 n) { return coretmpl_global.make_bitmask32(n); }
        protected static u64 make_bitmask64(s32 n) { return coretmpl_global.make_bitmask64(n); }
        protected static u64 make_bitmask64(u32 n) { return coretmpl_global.make_bitmask64(n); }
        protected static int BIT(int x, int n) { return coretmpl_global.BIT(x, n); }
        protected static UInt32 BIT(UInt32 x, int n)  { return coretmpl_global.BIT(x, n); }
        protected static int bitswap(int val, int B1, int B0) { return coretmpl_global.bitswap(val, B1, B0); }
        protected static int bitswap(int val, int B3, int B2, int B1, int B0) { return coretmpl_global.bitswap(val, B3, B2, B1, B0); }
        protected static int bitswap(int val, int B5, int B4, int B3, int B2, int B1, int B0) { return coretmpl_global.bitswap(val, B5, B4, B3, B2, B1, B0); }
        protected static int bitswap(int val, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0) { return coretmpl_global.bitswap(val, B7, B6, B5, B4, B3, B2, B1, B0); }
        protected static int bitswap(int val, int B15, int B14, int B13, int B12, int B11, int B10, int B9, int B8, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0) { return coretmpl_global.bitswap(val, B15, B14, B13, B12, B11, B10, B9, B8, B7, B6, B5, B4, B3, B2, B1, B0); }
        public static sbyte iabs(sbyte v) { return coretmpl_global.iabs(v); }
        public static short iabs(short v) { return coretmpl_global.iabs(v); }
        public static int iabs(int v) { return coretmpl_global.iabs(v); }
        public static Int64 iabs(Int64 v) { return coretmpl_global.iabs(v); }
        protected static void reduce_fraction(ref UInt32 num, ref UInt32 den) { coretmpl_global.reduce_fraction(ref num, ref den); }


        // crsshair
        protected const int CROSSHAIR_VISIBILITY_OFF = crsshair_global.CROSSHAIR_VISIBILITY_OFF;
        protected const int CROSSHAIR_VISIBILITY_DEFAULT = crsshair_global.CROSSHAIR_VISIBILITY_DEFAULT;
        protected const int CROSSHAIR_VISIBILITY_AUTOTIME_DEFAULT = crsshair_global.CROSSHAIR_VISIBILITY_AUTOTIME_DEFAULT;
        protected const int CROSSHAIR_RAW_SIZE = crsshair_global.CROSSHAIR_RAW_SIZE;
        protected const int CROSSHAIR_RAW_ROWBYTES = crsshair_global.CROSSHAIR_RAW_ROWBYTES;


        // dac
        protected static dac_8bit_r2r_device DAC_8BIT_R2R(machine_config mconfig, device_finder<dac_8bit_r2r_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, dac_8bit_r2r_device.DAC_8BIT_R2R, clock); }


        // device
        protected const string DEVICE_SELF = device_global.DEVICE_SELF;
        protected const string DEVICE_SELF_OWNER = device_global.DEVICE_SELF_OWNER;
        protected static u32 DERIVED_CLOCK(u32 num, u32 den) { return device_global.DERIVED_CLOCK(num, den); }
        protected static device_type DEFINE_DEVICE_TYPE(device_type.create_func func, string shortname, string fullname) { return device_global.DEFINE_DEVICE_TYPE(func, shortname, fullname); }


        // diexec
        protected const int CLEAR_LINE = (int)line_state.CLEAR_LINE;
        protected const int ASSERT_LINE = (int)line_state.ASSERT_LINE;
        protected const int HOLD_LINE = (int)line_state.HOLD_LINE;
        protected const int INPUT_LINE_HALT = device_execute_interface.INPUT_LINE_HALT;


        // digfx
        protected static readonly u32 [] EXTENDED_XOFFS = digfx_global.EXTENDED_XOFFS;
        protected static readonly u32 [] EXTENDED_YOFFS = digfx_global.EXTENDED_YOFFS;
        public static UInt32 RGN_FRAC(UInt32 num, UInt32 den) { return digfx_global.RGN_FRAC(num, den); }
        protected static bool IS_FRAC(UInt32 offset) { return digfx_global.IS_FRAC(offset); }
        protected static UInt32 FRAC_NUM(UInt32 offset) { return digfx_global.FRAC_NUM(offset); }
        protected static UInt32 FRAC_DEN(UInt32 offset) { return digfx_global.FRAC_DEN(offset); }
        protected static UInt32 FRAC_OFFSET(UInt32 offset) { return digfx_global.FRAC_OFFSET(offset); }
        protected static UInt32 [] ArrayCombineUInt32(params object [] objects) { return digfx_global.ArrayCombineUInt32(objects); }
        protected static UInt32 [] STEP2(int START, int STEP) { return digfx_global.STEP2(START, STEP); }
        protected static UInt32 [] STEP4(int START, int STEP) { return digfx_global.STEP4(START, STEP); }
        public static UInt32 [] STEP8(int START, int STEP) { return digfx_global.STEP8(START, STEP); }
        protected static UInt32 [] STEP16(int START, int STEP) { return digfx_global.STEP16(START, STEP); }
        protected static UInt32 [] STEP32(int START, int STEP) { return digfx_global.STEP32(START, STEP); }
        protected static UInt32 GFXENTRY_GETXSCALE(UInt32 x) { return digfx_global.GFXENTRY_GETXSCALE(x); }
        protected static UInt32 GFXENTRY_GETYSCALE(UInt32 x) { return digfx_global.GFXENTRY_GETYSCALE(x); }
        protected static bool GFXENTRY_ISROM(UInt32 x) { return digfx_global.GFXENTRY_ISROM(x); }
        protected static bool GFXENTRY_ISRAM(UInt32 x) { return digfx_global.GFXENTRY_ISRAM(x); }
        protected static bool GFXENTRY_ISDEVICE(UInt32 x) { return digfx_global.GFXENTRY_ISDEVICE(x); }
        protected static bool GFXENTRY_ISREVERSE(UInt32 x) { return digfx_global.GFXENTRY_ISREVERSE(x); }
        protected static gfx_decode_entry GFXDECODE_ENTRY(string region, u32 offset, gfx_layout layout, u16 start, u16 colors) { return digfx_global.GFXDECODE_ENTRY(region, offset, layout, start, colors); }
        protected static gfx_decode_entry GFXDECODE_SCALE(string region, u32 offset, gfx_layout layout, u16 start, u16 colors, u32 x, u32 y) { return digfx_global.GFXDECODE_SCALE(region, offset, layout, start, colors, x, y); }


        // disc_flt
        protected static void calculate_filter2_coefficients(discrete_base_node node, double fc, double d, double type, ref discrete_filter_coeff coeff) { disc_flt_global.calculate_filter2_coefficients(node, fc, d, type, ref coeff); }


        // discrete
        protected static discrete_sound_device DISCRETE(machine_config mconfig, string tag, discrete_block [] intf)
        {
            var device = emu.detail.device_type_impl.op<discrete_sound_device>(mconfig, tag, discrete_sound_device.DISCRETE, 0);
            device.set_intf(intf);
            return device;
        }
        protected static discrete_sound_device DISCRETE(machine_config mconfig, device_finder<discrete_sound_device> finder) { return emu.detail.device_type_impl.op(mconfig, finder, discrete_sound_device.DISCRETE, 0); }
        protected static discrete_sound_device DISCRETE(machine_config mconfig, device_finder<discrete_sound_device> finder, discrete_block [] intf)
        {
            var device = DISCRETE(mconfig, finder);
            device.set_intf(intf);
            return device;
        }
        protected const int NODE_00 = discrete_global.NODE_00;
        protected const int NODE_01 = discrete_global.NODE_01;
        protected const int NODE_02 = discrete_global.NODE_02;
        protected const int NODE_03 = discrete_global.NODE_03;
        protected const int NODE_04 = discrete_global.NODE_04;
        protected const int NODE_05 = discrete_global.NODE_05;
        protected const int NODE_06 = discrete_global.NODE_06;
        protected const int NODE_07 = discrete_global.NODE_07;
        protected const int NODE_08 = discrete_global.NODE_08;
        protected const int NODE_09 = discrete_global.NODE_09;
        protected const int NODE_10 = discrete_global.NODE_10;
        protected const int NODE_11 = discrete_global.NODE_11;
        protected const int NODE_12 = discrete_global.NODE_12;
        protected const int NODE_13 = discrete_global.NODE_13;
        protected const int NODE_14 = discrete_global.NODE_14;
        protected const int NODE_15 = discrete_global.NODE_15;
        protected const int NODE_16 = discrete_global.NODE_16;
        protected const int NODE_17 = discrete_global.NODE_17;
        protected const int NODE_20 = discrete_global.NODE_20;
        protected const int NODE_21 = discrete_global.NODE_21;
        protected const int NODE_22 = discrete_global.NODE_22;
        protected const int NODE_23 = discrete_global.NODE_23;
        protected const int NODE_24 = discrete_global.NODE_24;
        protected const int NODE_25 = discrete_global.NODE_25;
        protected const int NODE_26 = discrete_global.NODE_26;
        protected const int NODE_27 = discrete_global.NODE_27;
        protected const int NODE_28 = discrete_global.NODE_28;
        protected const int NODE_29 = discrete_global.NODE_29;
        protected const int NODE_30 = discrete_global.NODE_30;
        protected const int NODE_33 = discrete_global.NODE_33;
        protected const int NODE_34 = discrete_global.NODE_34;
        protected const int NODE_35 = discrete_global.NODE_35;
        protected const int NODE_38 = discrete_global.NODE_38;
        protected const int NODE_39 = discrete_global.NODE_39;
        protected const int NODE_40 = discrete_global.NODE_40;
        protected const int NODE_45 = discrete_global.NODE_45;
        protected const int NODE_50 = discrete_global.NODE_50;
        protected const int NODE_51 = discrete_global.NODE_51;
        protected const int NODE_52 = discrete_global.NODE_52;
        protected const int NODE_54 = discrete_global.NODE_54;
        protected const int NODE_55 = discrete_global.NODE_55;
        protected const int NODE_60 = discrete_global.NODE_60;
        protected const int NODE_61 = discrete_global.NODE_61;
        protected const int NODE_70 = discrete_global.NODE_70;
        protected const int NODE_71 = discrete_global.NODE_71;
        protected const int NODE_73 = discrete_global.NODE_73;
        protected const int NODE_90 = discrete_global.NODE_90;
        protected const int NODE_100 = discrete_global.NODE_100;
        protected const int NODE_105 = discrete_global.NODE_105;
        protected const int NODE_110 = discrete_global.NODE_110;
        protected const int NODE_111 = discrete_global.NODE_111;
        protected const int NODE_115 = discrete_global.NODE_115;
        protected const int NODE_116 = discrete_global.NODE_116;
        protected const int NODE_117 = discrete_global.NODE_117;
        protected const int NODE_120 = discrete_global.NODE_120;
        protected const int NODE_132 = discrete_global.NODE_132;
        protected const int NODE_133 = discrete_global.NODE_133;
        protected const int NODE_133_00 = discrete_global.NODE_133_00;
        protected const int NODE_133_02 = discrete_global.NODE_133_02;
        protected const int NODE_133_03 = discrete_global.NODE_133_03;
        protected const int NODE_150 = discrete_global.NODE_150;
        protected const int NODE_151 = discrete_global.NODE_151;
        protected const int NODE_152 = discrete_global.NODE_152;
        protected const int NODE_155 = discrete_global.NODE_155;
        protected const int NODE_157 = discrete_global.NODE_157;
        protected const int NODE_170 = discrete_global.NODE_170;
        protected const int NODE_171 = discrete_global.NODE_171;
        protected const int NODE_172 = discrete_global.NODE_172;
        protected const int NODE_173 = discrete_global.NODE_173;
        protected const int NODE_177 = discrete_global.NODE_177;
        protected const int NODE_178 = discrete_global.NODE_178;
        protected const int NODE_181 = discrete_global.NODE_181;
        protected const int NODE_182 = discrete_global.NODE_182;
        protected const int NODE_208 = discrete_global.NODE_208;
        protected const int NODE_209 = discrete_global.NODE_209;
        protected const int NODE_240 = discrete_global.NODE_240;
        protected const int NODE_241 = discrete_global.NODE_241;
        protected const int NODE_242 = discrete_global.NODE_242;
        protected const int NODE_243 = discrete_global.NODE_243;
        protected const int NODE_250 = discrete_global.NODE_250;
        protected const int NODE_279 = discrete_global.NODE_279;
        protected const int NODE_280 = discrete_global.NODE_280;
        protected const int NODE_288 = discrete_global.NODE_288;
        protected const int NODE_289 = discrete_global.NODE_289;
        protected const int NODE_294 = discrete_global.NODE_294;
        protected const int NODE_295 = discrete_global.NODE_295;
        protected const int NODE_296 = discrete_global.NODE_296;
        protected const int DISCRETE_MAX_NODES = discrete_global.DISCRETE_MAX_NODES;
        protected const int DISCRETE_MAX_INPUTS = discrete_global.DISCRETE_MAX_INPUTS;
        protected const int DISCRETE_MAX_OUTPUTS = discrete_global.DISCRETE_MAX_OUTPUTS;
        protected const int DISCRETE_MAX_TASK_GROUPS = discrete_global.DISCRETE_MAX_TASK_GROUPS;
        protected static double RC_CHARGE_EXP_DT(double rc, double dt) { return discrete_global.RC_CHARGE_EXP_DT(rc, dt); }
        protected const double DEFAULT_TTL_V_LOGIC_1 = discrete_global.DEFAULT_TTL_V_LOGIC_1;
        protected const double DISC_LINADJ = discrete_global.DISC_LINADJ;
        protected const int DISC_CLK_MASK = discrete_global.DISC_CLK_MASK;
        protected const int DISC_CLK_ON_F_EDGE = discrete_global.DISC_CLK_ON_F_EDGE;
        protected const int DISC_CLK_ON_R_EDGE =  discrete_global.DISC_CLK_ON_R_EDGE;
        protected const int DISC_CLK_BY_COUNT = discrete_global.DISC_CLK_BY_COUNT;
        protected const int DISC_CLK_IS_FREQ = discrete_global.DISC_CLK_IS_FREQ;
        protected const int DISC_COUNT_UP = discrete_global.DISC_COUNT_UP;
        protected const int DISC_COUNTER_IS_7492 = discrete_global.DISC_COUNTER_IS_7492;
        protected const int DISC_OUT_MASK = discrete_global.DISC_OUT_MASK;
        protected const int DISC_OUT_IS_ENERGY = discrete_global.DISC_OUT_IS_ENERGY;
        protected const int DISC_OUT_HAS_XTIME = discrete_global.DISC_OUT_HAS_XTIME;
        protected const int DISC_LFSR_OR = discrete_global.DISC_LFSR_OR;
        protected const int DISC_LFSR_AND = discrete_global.DISC_LFSR_AND;
        protected const int DISC_LFSR_XNOR = discrete_global.DISC_LFSR_XNOR;
        protected const int DISC_LFSR_NOR = discrete_global.DISC_LFSR_NOR;
        protected const int DISC_LFSR_NAND = discrete_global.DISC_LFSR_NAND;
        protected const int DISC_LFSR_IN1 = discrete_global.DISC_LFSR_IN1;
        protected const int DISC_LFSR_NOT_IN1 = discrete_global.DISC_LFSR_NOT_IN1;
        protected const int DISC_LFSR_XOR_INV_IN0 = discrete_global.DISC_LFSR_XOR_INV_IN0;
        protected const int DISC_LFSR_IN0 = discrete_global.DISC_LFSR_IN0;
        protected const int DISC_LFSR_REPLACE = discrete_global.DISC_LFSR_REPLACE;
        protected const int DISC_LFSR_XOR = discrete_global.DISC_LFSR_XOR;
        protected const int DISC_LFSR_NOT_IN0 = discrete_global.DISC_LFSR_NOT_IN0;
        protected const int DISC_LFSR_XOR_INV_IN1 = discrete_global.DISC_LFSR_XOR_INV_IN1;
        protected const int DISC_LFSR_FLAG_OUT_INVERT = discrete_global.DISC_LFSR_FLAG_OUT_INVERT;
        protected const int DISC_LFSR_FLAG_RESET_TYPE_H = discrete_global.DISC_LFSR_FLAG_RESET_TYPE_H;
        protected const int DISC_LFSR_FLAG_OUTPUT_F0 = discrete_global.DISC_LFSR_FLAG_OUTPUT_F0;
        protected const int DISC_LFSR_FLAG_OUTPUT_SR_SN1 = discrete_global.DISC_LFSR_FLAG_OUTPUT_SR_SN1;
        protected const int DISC_LADDER_MAXRES = discrete_global.DISC_LADDER_MAXRES;
        public const int DISC_FILTER_LOWPASS = discrete_global.DISC_FILTER_LOWPASS;
        public const int DISC_FILTER_HIGHPASS = discrete_global.DISC_FILTER_HIGHPASS;
        public const int DISC_FILTER_BANDPASS = discrete_global.DISC_FILTER_BANDPASS;
        protected const int DISC_MIXER_IS_RESISTOR = discrete_global.DISC_MIXER_IS_RESISTOR;
        protected const int DISC_MIXER_IS_OP_AMP = discrete_global.DISC_MIXER_IS_OP_AMP;
        protected const int DISC_MIXER_IS_OP_AMP_WITH_RI = discrete_global.DISC_MIXER_IS_OP_AMP_WITH_RI;
        protected static readonly int NODE_SPECIAL = discrete_global.NODE_SPECIAL;
        protected const int NODE_START = discrete_global.NODE_START;
        protected static readonly int NODE_END = discrete_global.NODE_END;
        protected static int NODE_(int x) { return discrete_global.NODE_(x); }
        protected static int NODE_CHILD_NODE_NUM(int x) { return discrete_global.NODE_CHILD_NODE_NUM(x); }
        protected static int NODE_DEFAULT_NODE(int x) { return discrete_global.NODE_DEFAULT_NODE(x); }
        protected static int NODE_INDEX(int x) { return discrete_global.NODE_INDEX(x); }
        public static int NODE_RELATIVE(int x, int y) { return discrete_global.NODE_RELATIVE(x, y); }
        protected static bool IS_VALUE_A_NODE(int val) { return discrete_global.IS_VALUE_A_NODE(val); }
        protected const int DISC_OP_AMP_IS_NORTON = discrete_global.DISC_OP_AMP_IS_NORTON;
        protected const double OP_AMP_NORTON_VBE = discrete_global.OP_AMP_NORTON_VBE;
        protected const double OP_AMP_VP_RAIL_OFFSET = discrete_global.OP_AMP_VP_RAIL_OFFSET;
        protected const int DISC_OP_AMP_FILTER_IS_LOW_PASS_1 = discrete_global.DISC_OP_AMP_FILTER_IS_LOW_PASS_1;
        protected const int DISC_OP_AMP_FILTER_IS_HIGH_PASS_1 = discrete_global.DISC_OP_AMP_FILTER_IS_HIGH_PASS_1;
        protected const int DISC_OP_AMP_FILTER_IS_BAND_PASS_1 = discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1;
        protected const int DISC_OP_AMP_FILTER_IS_BAND_PASS_1M = discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M;
        protected const int DISC_OP_AMP_FILTER_IS_HIGH_PASS_0 = discrete_global.DISC_OP_AMP_FILTER_IS_HIGH_PASS_0;
        protected const int DISC_OP_AMP_FILTER_IS_BAND_PASS_0 = discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_0;
        protected const int DISC_OP_AMP_FILTER_IS_LOW_PASS_1_A = discrete_global.DISC_OP_AMP_FILTER_IS_LOW_PASS_1_A;
        protected const int DISC_OP_AMP_FILTER_TYPE_MASK = discrete_global.DISC_OP_AMP_FILTER_TYPE_MASK;
        protected const int DISC_SALLEN_KEY_LOW_PASS = discrete_global.DISC_SALLEN_KEY_LOW_PASS;
        protected const int DISC_555_OUT_DC = discrete_global.DISC_555_OUT_DC;
        protected const int DISC_555_OUT_AC = discrete_global.DISC_555_OUT_AC;
        protected const int DISC_555_OUT_SQW = discrete_global.DISC_555_OUT_SQW;
        protected const int DISC_555_OUT_CAP = discrete_global.DISC_555_OUT_CAP;
        protected const int DISC_555_OUT_COUNT_F = discrete_global.DISC_555_OUT_COUNT_F;
        protected const int DISC_555_OUT_COUNT_R = discrete_global.DISC_555_OUT_COUNT_R;
        protected const int DISC_555_OUT_ENERGY = discrete_global.DISC_555_OUT_ENERGY;
        protected const int DISC_555_OUT_LOGIC_X = discrete_global.DISC_555_OUT_LOGIC_X;
        protected const int DISC_555_OUT_COUNT_F_X = discrete_global.DISC_555_OUT_COUNT_F_X;
        protected const int DISC_555_OUT_COUNT_R_X = discrete_global.DISC_555_OUT_COUNT_R_X;
        protected const int DISC_555_OUT_MASK = discrete_global.DISC_555_OUT_MASK;
        protected const int DISC_555_ASTABLE_HAS_FAST_CHARGE_DIODE = discrete_global.DISC_555_ASTABLE_HAS_FAST_CHARGE_DIODE;
        protected const int DISCRETE_555_CC_TO_CAP = discrete_global.DISCRETE_555_CC_TO_CAP;
        protected const int DISC_RC_INTEGRATE_TYPE1 = discrete_global.DISC_RC_INTEGRATE_TYPE1;
        protected const int DISC_RC_INTEGRATE_TYPE2 = discrete_global.DISC_RC_INTEGRATE_TYPE2;
        protected const int DISC_RC_INTEGRATE_TYPE3 = discrete_global.DISC_RC_INTEGRATE_TYPE3;
        protected const int DEFAULT_555_CHARGE = discrete_global.DEFAULT_555_CHARGE;
        protected const int DEFAULT_555_HIGH = discrete_global.DEFAULT_555_HIGH;
        protected const int DEFAULT_555_VALUES_1 = discrete_global.DEFAULT_555_VALUES_1;
        protected const int DEFAULT_555_VALUES_2 = discrete_global.DEFAULT_555_VALUES_2;
        protected const int DEFAULT_555_CC_SOURCE = discrete_global.DEFAULT_555_CC_SOURCE;
        protected static discrete_block DISCRETE_SOUND_END { get { return discrete_global.DISCRETE_SOUND_END; } }
        protected static discrete_block DISCRETE_ADJUSTMENT(int NODE, double MIN, double MAX, double LOGLIN, string TAG)  { return discrete_global.DISCRETE_ADJUSTMENT(NODE, MIN, MAX, LOGLIN, TAG); }
        protected static discrete_block DISCRETE_INPUT_DATA(int NODE) { return discrete_global.DISCRETE_INPUT_DATA(NODE); }
        protected static discrete_block DISCRETE_INPUTX_DATA(int NODE, double GAIN, double OFFSET, double INIT) { return discrete_global.DISCRETE_INPUTX_DATA(NODE, GAIN, OFFSET, INIT); }
        protected static discrete_block DISCRETE_INPUT_LOGIC(int NODE) { return discrete_global.DISCRETE_INPUT_LOGIC(NODE); }
        protected static discrete_block DISCRETE_INPUT_NOT(int NODE) { return discrete_global.DISCRETE_INPUT_NOT(NODE); }
        protected static discrete_block DISCRETE_INPUTX_STREAM(int NODE, double NUM, double GAIN, double OFFSET) { return discrete_global.DISCRETE_INPUTX_STREAM(NODE, NUM, GAIN, OFFSET); }
        protected static discrete_block DISCRETE_INPUT_BUFFER(int NODE, double NUM) { return discrete_global.DISCRETE_INPUT_BUFFER(NODE, NUM); }
        protected static discrete_block DISCRETE_COUNTER(int NODE, double ENAB, double RESET, double CLK, double MIN, double MAX, double DIR, double INIT0, double CLKTYPE) { return discrete_global.DISCRETE_COUNTER(NODE, ENAB, RESET, CLK, MIN, MAX, DIR, INIT0, CLKTYPE); }
        protected static discrete_block DISCRETE_LFSR_NOISE(int NODE, double ENAB, double RESET, double CLK, double AMPL, double FEED, double BIAS, discrete_lfsr_desc LFSRTB) { return discrete_global.DISCRETE_LFSR_NOISE(NODE, ENAB, RESET, CLK, AMPL, FEED, BIAS, LFSRTB); }
        protected static discrete_block DISCRETE_NOTE(int NODE, double ENAB, double CLK, double DATA, double MAX1, double MAX2, double CLKTYPE) { return discrete_global.DISCRETE_NOTE(NODE, ENAB, CLK, DATA, MAX1, MAX2, CLKTYPE); }
        protected static discrete_block DISCRETE_SQUAREWFIX(int NODE, double ENAB, double FREQ, double AMPL, double DUTY, double BIAS, double PHASE) { return discrete_global.DISCRETE_SQUAREWFIX(NODE, ENAB, FREQ, AMPL, DUTY, BIAS, PHASE); }
        protected static discrete_block DISCRETE_INVERTER_OSC(int NODE, double ENAB, double MOD, double RCHARGE, double RP, double C, double R2, discrete_dss_inverter_osc_node.description INFO) { return discrete_global.DISCRETE_INVERTER_OSC(NODE, ENAB, MOD, RCHARGE, RP, C, R2, INFO); }
        protected static discrete_block DISCRETE_ADDER2(int NODE, double ENAB, double INP0, double INP1) { return discrete_global.DISCRETE_ADDER2(NODE, ENAB, INP0, INP1); }
        protected static discrete_block DISCRETE_CLAMP(int NODE, double INP0, double MIN, double MAX) { return discrete_global.DISCRETE_CLAMP(NODE, INP0, MIN, MAX); }
        protected static discrete_block DISCRETE_DIVIDE(int NODE, double ENAB, double INP0, double INP1) { return discrete_global.DISCRETE_DIVIDE(NODE, ENAB, INP0, INP1); }
        protected static discrete_block DISCRETE_LOGIC_INVERT(int NODE, double INP0) {return discrete_global.DISCRETE_LOGIC_INVERT(NODE, INP0); }
        protected static discrete_block DISCRETE_BITS_DECODE(int NODE, double INP, double BIT_FROM, double BIT_TO, double VOUT) { return discrete_global.DISCRETE_BITS_DECODE(NODE, INP, BIT_FROM, BIT_TO, VOUT); }
        protected static discrete_block DISCRETE_LOGIC_DFLIPFLOP(int NODE, double RESET, double SET, double CLK, double INP) { return discrete_global.DISCRETE_LOGIC_DFLIPFLOP(NODE, RESET, SET, CLK, INP); }
        protected static discrete_block DISCRETE_MULTIPLY(int NODE, double INP0, double INP1) { return discrete_global.DISCRETE_MULTIPLY(NODE, INP0, INP1); }
        protected static discrete_block DISCRETE_MULTADD(int NODE, double INP0, double INP1, double INP2) { return discrete_global.DISCRETE_MULTADD(NODE, INP0, INP1, INP2); }
        protected static discrete_block DISCRETE_TRANSFORM2(int NODE, double INP0, double INP1, string FUNCT) { return discrete_global.DISCRETE_TRANSFORM2(NODE, INP0, INP1, FUNCT); }
        protected static discrete_block DISCRETE_TRANSFORM3(int NODE, double INP0, double INP1, double INP2, string FUNCT) { return discrete_global.DISCRETE_TRANSFORM3(NODE, INP0, INP1, INP2, FUNCT); }
        protected static discrete_block DISCRETE_TRANSFORM4(int NODE, double INP0, double INP1, double INP2, double INP3, string FUNCT) { return discrete_global.DISCRETE_TRANSFORM4(NODE, INP0, INP1, INP2, INP3, FUNCT); }
        protected static discrete_block DISCRETE_TRANSFORM5(int NODE, double INP0, double INP1, double INP2, double INP3, double INP4, string FUNCT) {return discrete_global.DISCRETE_TRANSFORM5(NODE, INP0, INP1, INP2, INP3, INP4, FUNCT); }
        protected static discrete_block DISCRETE_DAC_R1(int NODE, double DATA, double VDATA, discrete_dac_r1_ladder LADDER) { return discrete_global.DISCRETE_DAC_R1(NODE, DATA, VDATA, LADDER); }
        protected static discrete_block DISCRETE_DIODE_MIXER2(int NODE, double IN0, double IN1, double [] TABLE) { return discrete_global.DISCRETE_DIODE_MIXER2(NODE, IN0, IN1, TABLE); }
        protected static discrete_block DISCRETE_MIXER2(int NODE, double ENAB, double IN0, double IN1, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER2(NODE, ENAB, IN0, IN1, INFO); }
        protected static discrete_block DISCRETE_MIXER3(int NODE, double ENAB, double IN0, double IN1, double IN2, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER3(NODE, ENAB, IN0, IN1, IN2, INFO); }
        protected static discrete_block DISCRETE_MIXER4(int NODE, double ENAB, double IN0, double IN1, double IN2, double IN3, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER4(NODE, ENAB, IN0, IN1, IN2, IN3, INFO); }
        protected static discrete_block DISCRETE_MIXER5(int NODE, double ENAB, double IN0, double IN1, double IN2, double IN3, double IN4, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER5(NODE, ENAB, IN0, IN1, IN2, IN3, IN4, INFO); }
        protected static discrete_block DISCRETE_MIXER6(int NODE, double ENAB, double IN0, double IN1, double IN2, double IN3, double IN4, double IN5, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER6(NODE, ENAB, IN0, IN1, IN2, IN3, IN4, IN5, INFO); }
        protected static discrete_block DISCRETE_SALLEN_KEY_FILTER(int NODE, double ENAB, double INP0, double TYPE, discrete_op_amp_filt_info INFO) { return discrete_global.DISCRETE_SALLEN_KEY_FILTER(NODE, ENAB, INP0, TYPE, INFO); }
        protected static discrete_block DISCRETE_CRFILTER(int NODE, int INP0, double RVAL, double CVAL) { return discrete_global.DISCRETE_CRFILTER(NODE, INP0, RVAL, CVAL); }
        protected static discrete_block DISCRETE_OP_AMP_FILTER(int NODE, double ENAB, double INP0, double INP1, double TYPE, discrete_op_amp_filt_info INFO) { return discrete_global.DISCRETE_OP_AMP_FILTER(NODE, ENAB, INP0, INP1, TYPE, INFO); }
        protected static discrete_block DISCRETE_RCDISC(int NODE, double ENAB, double INP0, double RVAL, double CVAL) { return discrete_global.DISCRETE_RCDISC(NODE, ENAB, INP0, RVAL, CVAL); }
        protected static discrete_block DISCRETE_RCDISC2(int NODE, double SWITCH, double INP0, double RVAL0, double INP1, double RVAL1, double CVAL) { return discrete_global.DISCRETE_RCDISC2(NODE, SWITCH, INP0, RVAL0, INP1, RVAL1, CVAL); }
        protected static discrete_block DISCRETE_RCDISC5(int NODE, double ENAB, double INP0, double RVAL, double CVAL) { return discrete_global.DISCRETE_RCDISC5(NODE, ENAB, INP0, RVAL, CVAL); }
        protected static discrete_block DISCRETE_RCDISC_MODULATED(int NODE, double INP0, double INP1, double RVAL0, double RVAL1, double RVAL2, double RVAL3, double CVAL, double VP) { return discrete_global.DISCRETE_RCDISC_MODULATED(NODE, INP0, INP1, RVAL0, RVAL1, RVAL2, RVAL3, CVAL, VP); }
        protected static discrete_block DISCRETE_RCFILTER(int NODE, double INP0, double RVAL, double CVAL) { return discrete_global.DISCRETE_RCFILTER(NODE, INP0, RVAL, CVAL); }
        protected static discrete_block DISCRETE_RCFILTER_SW(int NODE, double ENAB, double INP0, double SW, double RVAL, double CVAL1, double CVAL2, double CVAL3, double CVAL4) { return discrete_global.DISCRETE_RCFILTER_SW(NODE, ENAB, INP0, SW, RVAL, CVAL1, CVAL2, CVAL3, CVAL4); }
        protected static discrete_block DISCRETE_RCINTEGRATE(int NODE, double INP0, double RVAL0, double RVAL1, double RVAL2, double CVAL, double vP, double TYPE) { return discrete_global.DISCRETE_RCINTEGRATE(NODE, INP0, RVAL0, RVAL1, RVAL2, CVAL, vP, TYPE); }
        protected static discrete_block DISCRETE_CUSTOM8<CLASS>(int NODE, double IN0, double IN1, double IN2, double IN3, double IN4, double IN5, double IN6, double IN7, object INFO) where CLASS : discrete_base_node, new() { return discrete_global.DISCRETE_CUSTOM8<CLASS>(NODE, IN0, IN1, IN2, IN3, IN4, IN5, IN6, IN7, INFO); }
        protected static discrete_block DISCRETE_555_ASTABLE_CV(int NODE, double RESET, double R1, double R2, double C, double CTRLV, discrete_555_desc OPTIONS) { return discrete_global.DISCRETE_555_ASTABLE_CV(NODE, RESET, R1, R2, C, CTRLV, OPTIONS); }
        protected static discrete_block DISCRETE_555_CC(int NODE, double RESET, double VIN, double R, double C, double RBIAS, double RGND, double RDIS, discrete_555_cc_desc OPTIONS) { return discrete_global.DISCRETE_555_CC(NODE, RESET, VIN, R, C, RBIAS, RGND, RDIS, OPTIONS); }
        protected static discrete_block DISCRETE_TASK_START(double TASK_GROUP) { return discrete_global.DISCRETE_TASK_START(TASK_GROUP); }
        protected static discrete_block DISCRETE_TASK_END() { return discrete_global.DISCRETE_TASK_END(); }
        protected static discrete_block DISCRETE_OUTPUT(double OPNODE, double GAIN) { return discrete_global.DISCRETE_OUTPUT(OPNODE, GAIN); }
        protected const int MAX_SAMPLES_PER_TASK_SLICE = discrete_global.MAX_SAMPLES_PER_TASK_SLICE;
        protected const int USE_DISCRETE_TASKS = discrete_global.USE_DISCRETE_TASKS;


        // disound
        protected const int ALL_OUTPUTS = disound_global.ALL_OUTPUTS;
        protected const int AUTO_ALLOC_INPUT = disound_global.AUTO_ALLOC_INPUT;


        // distate
        protected const int STATE_GENPC     = device_state_interface.STATE_GENPC;
        protected const int STATE_GENPCBASE = device_state_interface.STATE_GENPCBASE;
        protected const int STATE_GENSP     = device_state_interface.STATE_GENSP;
        protected const int STATE_GENFLAGS  = device_state_interface.STATE_GENFLAGS;


        // drawgfx
        protected static gfxdecode_device GFXDECODE(machine_config mconfig, string tag, string palette_tag, gfx_decode_entry [] gfxinfo)
        {
            var device = emu.detail.device_type_impl.op<gfxdecode_device>(mconfig, tag, gfxdecode_device.GFXDECODE, 0);
            device.gfxdecode_device_after_ctor(palette_tag, gfxinfo);
            return device;
        }
        protected static gfxdecode_device GFXDECODE(machine_config mconfig, device_finder<gfxdecode_device> finder, string palette_tag, gfx_decode_entry [] gfxinfo)
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, gfxdecode_device.GFXDECODE, 0);
            device.gfxdecode_device_after_ctor(palette_tag, gfxinfo);
            return device;
        }
        protected static gfxdecode_device GFXDECODE(machine_config mconfig, device_finder<gfxdecode_device> finder, finder_base palette, gfx_decode_entry [] gfxinfo)
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, gfxdecode_device.GFXDECODE, 0);
            device.gfxdecode_device_after_ctor(palette, gfxinfo);
            return device;
        }
        protected static void copyscrollbitmap(bitmap_ind16 dest, bitmap_ind16 src, u32 numrows, s32 [] rowscroll, u32 numcols, s32 [] colscroll, rectangle cliprect) { drawgfx_global.copyscrollbitmap(dest, src, numrows, rowscroll, numcols, colscroll, cliprect); }
        protected static void copyscrollbitmap_trans(bitmap_ind16 dest, bitmap_ind16 src, u32 numrows, s32 [] rowscroll, u32 numcols, s32 [] colscroll, rectangle cliprect, u32 trans_pen) { drawgfx_global.copyscrollbitmap_trans(dest, src, numrows, rowscroll, numcols, colscroll, cliprect, trans_pen); }


        // driver
        protected static void MCFG_MACHINE_START_OVERRIDE(machine_config config, driver_callback_delegate func) { driver_global.MCFG_MACHINE_START_OVERRIDE(config, func); }
        protected static void MCFG_MACHINE_RESET_OVERRIDE(machine_config config, driver_callback_delegate func) { driver_global.MCFG_MACHINE_RESET_OVERRIDE(config, func); }
        protected static void MCFG_VIDEO_START_OVERRIDE(machine_config config, driver_callback_delegate func) { driver_global.MCFG_VIDEO_START_OVERRIDE(config, func); }


        // eeprom
        protected static eeprom_parallel_2804_device EEPROM_2804(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<eeprom_parallel_2804_device>(mconfig, tag, eeprom_parallel_2804_device.EEPROM_2804, clock); }


        // eminline
        public static int64_t mul_32x32(int32_t a, int32_t b) { return eminline_global.mul_32x32(a, b); }
        public static uint64_t mulu_32x32(uint32_t a, uint32_t b) { return eminline_global.mulu_32x32(a, b); }
        protected static uint32_t divu_64x32(uint64_t a, uint32_t b) { return eminline_global.divu_64x32(a, b); }
        public static uint32_t divu_64x32_rem(uint64_t a, uint32_t b, out uint32_t remainder) { return eminline_global.divu_64x32_rem(a, b, out remainder); }
        protected static int64_t get_profile_ticks() { return eminline_global.get_profile_ticks(); }


        // emualloc
        public static MemoryContainer<T> pool_alloc_array<T>(UInt32 num) where T : new() { return emualloc_global.pool_alloc_array<T>(num); }
        public static MemoryContainer<T> pool_alloc_array_clear<T>(UInt32 num) where T : new() { return emualloc_global.pool_alloc_array_clear<T>(num); }


        // emucore
        protected static readonly string [] endianness_names = emucore_global.endianness_names;
        protected const endianness_t ENDIANNESS_NATIVE = emucore_global.ENDIANNESS_NATIVE;
        public const UInt32 ORIENTATION_FLIP_X = emucore_global.ORIENTATION_FLIP_X;
        public const UInt32 ORIENTATION_FLIP_Y = emucore_global.ORIENTATION_FLIP_Y;
        public const UInt32 ORIENTATION_SWAP_XY = emucore_global.ORIENTATION_SWAP_XY;
        protected const UInt32 ROT0   = emucore_global.ROT0;
        protected const UInt32 ROT90  = emucore_global.ROT90;
        protected const UInt32 ROT180 = emucore_global.ROT180;
        protected const UInt32 ROT270 = emucore_global.ROT270;
        protected static Tuple<object, string> NAME<T>(T x) { return emucore_global.NAME(x); }
        public static void fatalerror(string format, params object [] args) { emucore_global.fatalerror(format, args); }
        [Conditional("DEBUG")] public static void assert(bool condition) { emucore_global.assert(condition); }
        [Conditional("DEBUG")] public static void assert(bool condition, string message) { emucore_global.assert(condition, message); }
        [Conditional("ASSERT_SLOW")] public static void assert_slow(bool condition) { emucore_global.assert(condition); }
        [Conditional("DEBUG")] public static void static_assert(bool condition, string message) { assert(condition, message); }


        // emumem
        public const int AS_PROGRAM = emumem_global.AS_PROGRAM;
        protected const int AS_DATA = emumem_global.AS_DATA;
        public const int AS_IO = emumem_global.AS_IO;
        protected const int AS_OPCODES = emumem_global.AS_OPCODES;
        public static void COMBINE_DATA(ref u16 varptr, u16 data, u16 mem_mask) { emumem_global.COMBINE_DATA(ref varptr, data, mem_mask); }
        protected static bool ACCESSING_BITS_0_7(u16 mem_mask) { return emumem_global.ACCESSING_BITS_0_7(mem_mask); }
        protected static uX memory_read_generic(int Width, int AddrShift, endianness_t Endian, int TargetWidth, bool Aligned, Func<offs_t, uX, uX> rop, offs_t address, uX mask) { return emumem_global.memory_read_generic(Width, AddrShift, Endian, TargetWidth, Aligned, rop, address, mask); }
        protected static void memory_write_generic(int Width, int AddrShift, endianness_t Endian, int TargetWidth, bool Aligned, Action<offs_t, uX, uX> wop, offs_t address, uX data, uX mask) { emumem_global.memory_write_generic(Width, AddrShift, Endian, TargetWidth, Aligned, wop, address, data, mask); }
        protected static string core_i64_hex_format(u64 value, u8 mindigits) { return emumem_global.core_i64_hex_format(value, mindigits); }
        protected static int handler_entry_dispatch_lowbits(int highbits, int width, int ashift) { return emumem_global.handler_entry_dispatch_lowbits(highbits, width, ashift); }


        // emuopts
        protected static void conditionally_peg_priority(core_options.entry entry, bool peg_priority) { emuopts_global.conditionally_peg_priority(entry, peg_priority); }


        // emupal
        protected static palette_device PALETTE(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<palette_device>(mconfig, tag, palette_device.PALETTE, 0); }
        protected static palette_device PALETTE(machine_config mconfig, device_finder<palette_device> finder) { return emu.detail.device_type_impl.op(mconfig, finder, palette_device.PALETTE, 0); }
        protected static palette_device PALETTE(machine_config mconfig, string tag, palette_device.init_delegate init, u32 entries = 0U, u32 indirect = 0U)
        {
            var device = emu.detail.device_type_impl.op<palette_device>(mconfig, tag, palette_device.PALETTE, 0);
            device.palette_device_after_ctor(init, entries, indirect);
            return device;
        }
        protected static palette_device PALETTE(machine_config mconfig, device_finder<palette_device> finder, palette_device.init_delegate init, u32 entries = 0U, u32 indirect = 0U)
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, palette_device.PALETTE, 0);
            device.palette_device_after_ctor(init, entries, indirect);
            return device;
        }


        // er2055
        protected static er2055_device ER2055(machine_config mconfig, device_finder<er2055_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, er2055_device.ER2055, clock); }


        // galaxian
        protected static galaxian_sound_device GALAXIAN_SOUND(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<galaxian_sound_device>(mconfig, tag, galaxian_sound_device.GALAXIAN_SOUND, clock); }


        // gamedrv
        protected const UInt64 MACHINE_TYPE_ARCADE = gamedrv_global.MACHINE_TYPE_ARCADE;
        protected const UInt64 MACHINE_NOT_WORKING = gamedrv_global.MACHINE_NOT_WORKING;
        protected const UInt64 MACHINE_SUPPORTS_SAVE = gamedrv_global.MACHINE_SUPPORTS_SAVE;
        public const UInt64 MACHINE_IS_BIOS_ROOT = gamedrv_global.MACHINE_IS_BIOS_ROOT;
        protected const UInt64 MACHINE_CLICKABLE_ARTWORK = gamedrv_global.MACHINE_CLICKABLE_ARTWORK;
        protected const UInt64 MACHINE_NO_SOUND_HW = gamedrv_global.MACHINE_NO_SOUND_HW;
        protected const UInt64 MACHINE_UNEMULATED_PROTECTION = gamedrv_global.MACHINE_UNEMULATED_PROTECTION;
        protected const UInt64 MACHINE_WRONG_COLORS = gamedrv_global.MACHINE_WRONG_COLORS;
        protected const UInt64 MACHINE_IMPERFECT_COLORS = gamedrv_global.MACHINE_IMPERFECT_COLORS;
        protected const UInt64 MACHINE_IMPERFECT_GRAPHICS = gamedrv_global.MACHINE_IMPERFECT_GRAPHICS;
        protected const UInt64 MACHINE_NO_SOUND = gamedrv_global.MACHINE_NO_SOUND;
        protected const UInt64 MACHINE_IMPERFECT_SOUND = gamedrv_global.MACHINE_IMPERFECT_SOUND;
        protected const UInt64 MACHINE_IMPERFECT_CONTROLS = gamedrv_global.MACHINE_IMPERFECT_CONTROLS;
        protected const UInt64 MACHINE_NODEVICE_MICROPHONE = gamedrv_global.MACHINE_NODEVICE_MICROPHONE;
        protected const UInt64 MACHINE_NODEVICE_PRINTER = gamedrv_global.MACHINE_NODEVICE_PRINTER;
        protected const UInt64 MACHINE_NODEVICE_LAN = gamedrv_global.MACHINE_NODEVICE_LAN;
        protected const UInt64 MACHINE_IMPERFECT_TIMING = gamedrv_global.MACHINE_IMPERFECT_TIMING;
        protected static game_driver GAME(device_type.create_func creator, MemoryContainer<tiny_rom_entry> roms, string YEAR, string NAME, string PARENT, machine_creator_wrapper MACHINE, ioport_constructor INPUT, driver_init_wrapper INIT, UInt32 MONITOR, string COMPANY, string FULLNAME, UInt64 FLAGS) { return gamedrv_global.GAME(creator, roms, YEAR, NAME, PARENT, MACHINE, INPUT, INIT, MONITOR, COMPANY, FULLNAME, FLAGS); }


        // gen_latch
        protected static generic_latch_8_device GENERIC_LATCH_8(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<generic_latch_8_device>(mconfig, tag, generic_latch_8_device.GENERIC_LATCH_8, clock); }
        protected static generic_latch_8_device GENERIC_LATCH_8(machine_config mconfig, device_finder<generic_latch_8_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, generic_latch_8_device.GENERIC_LATCH_8, clock); }


        // hash
        protected static string CRC(string x) { return util.hash_global.CRC(x); }
        protected static string SHA1(string x) { return util.hash_global.SHA1(x); }
        protected const string NO_DUMP = util.hash_global.NO_DUMP;


        // i8255
        protected static i8255_device I8255A(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<i8255_device>(mconfig, tag, i8255_device.I8255A, clock); }
        protected static i8255_device I8255A(machine_config mconfig, device_finder<i8255_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, i8255_device.I8255A, clock); }


        // i8257
        protected static i8257_device I8257(machine_config mconfig, device_finder<i8257_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, i8257_device.I8257, clock); }


        // input
        protected static readonly input_code KEYCODE_F1 = input_global.KEYCODE_F1;


        // input_merger
        protected static input_merger_device INPUT_MERGER_ANY_HIGH(machine_config mconfig, device_finder<input_merger_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, input_merger_any_high_device.INPUT_MERGER_ANY_HIGH, clock); }
        protected static input_merger_device INPUT_MERGER_ALL_HIGH(machine_config mconfig, device_finder<input_merger_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, input_merger_all_high_device.INPUT_MERGER_ALL_HIGH, clock); }


        // ioport
        protected const ioport_value IP_ACTIVE_HIGH = ioport_global.IP_ACTIVE_HIGH;
        protected const ioport_value IP_ACTIVE_LOW = ioport_global.IP_ACTIVE_LOW;
        protected const int MAX_PLAYERS = ioport_global.MAX_PLAYERS;
        protected const ioport_type IPT_UNUSED = ioport_type.IPT_UNUSED;
        protected const ioport_type IPT_UNKNOWN = ioport_type.IPT_UNKNOWN;
        protected const ioport_type IPT_START1 = ioport_type.IPT_START1;
        protected const ioport_type IPT_START2 = ioport_type.IPT_START2;
        protected const ioport_type IPT_COIN1 = ioport_type.IPT_COIN1;
        protected const ioport_type IPT_COIN2 = ioport_type.IPT_COIN2;
        protected const ioport_type IPT_COIN3 = ioport_type.IPT_COIN3;
        protected const ioport_type IPT_SERVICE1 = ioport_type.IPT_SERVICE1;
        protected const ioport_type IPT_SERVICE = ioport_type.IPT_SERVICE;
        protected const ioport_type IPT_TILT = ioport_type.IPT_TILT;
        protected const ioport_type IPT_JOYSTICK_UP = ioport_type.IPT_JOYSTICK_UP;
        protected const ioport_type IPT_JOYSTICK_DOWN = ioport_type.IPT_JOYSTICK_DOWN;
        protected const ioport_type IPT_JOYSTICK_LEFT = ioport_type.IPT_JOYSTICK_LEFT;
        protected const ioport_type IPT_JOYSTICK_RIGHT = ioport_type.IPT_JOYSTICK_RIGHT;
        protected const ioport_type IPT_BUTTON1 = ioport_type.IPT_BUTTON1;
        protected const ioport_type IPT_BUTTON2 = ioport_type.IPT_BUTTON2;
        protected const ioport_type IPT_AD_STICK_X = ioport_type.IPT_AD_STICK_X;
        protected const ioport_type IPT_AD_STICK_Y = ioport_type.IPT_AD_STICK_Y;
        protected const ioport_type IPT_DIAL = ioport_type.IPT_DIAL;
        protected const ioport_type IPT_DIAL_V = ioport_type.IPT_DIAL_V;
        protected const ioport_type IPT_TRACKBALL_X = ioport_type.IPT_TRACKBALL_X;
        protected const ioport_type IPT_TRACKBALL_Y = ioport_type.IPT_TRACKBALL_Y;
        protected const ioport_type IPT_SPECIAL = ioport_type.IPT_SPECIAL;
        protected const ioport_type IPT_CUSTOM = ioport_type.IPT_CUSTOM;
        protected const INPUT_STRING Off = INPUT_STRING.INPUT_STRING_Off;
        protected const INPUT_STRING On = INPUT_STRING.INPUT_STRING_On;
        protected const INPUT_STRING No = INPUT_STRING.INPUT_STRING_No;
        protected const INPUT_STRING Yes = INPUT_STRING.INPUT_STRING_Yes;
        protected const INPUT_STRING Lives = INPUT_STRING.INPUT_STRING_Lives;
        protected const INPUT_STRING Bonus_Life = INPUT_STRING.INPUT_STRING_Bonus_Life;
        protected const INPUT_STRING Difficulty = INPUT_STRING.INPUT_STRING_Difficulty;
        protected const INPUT_STRING Demo_Sounds = INPUT_STRING.INPUT_STRING_Demo_Sounds;
        public const INPUT_STRING Coinage = INPUT_STRING.INPUT_STRING_Coinage;
        public const INPUT_STRING Coin_A = INPUT_STRING.INPUT_STRING_Coin_A;
        public const INPUT_STRING Coin_B = INPUT_STRING.INPUT_STRING_Coin_B;
        protected const INPUT_STRING _9C_1C = INPUT_STRING.INPUT_STRING_9C_1C;
        protected const INPUT_STRING _8C_1C = INPUT_STRING.INPUT_STRING_8C_1C;
        public const INPUT_STRING _7C_1C = INPUT_STRING.INPUT_STRING_7C_1C;
        public const INPUT_STRING _6C_1C = INPUT_STRING.INPUT_STRING_6C_1C;
        public const INPUT_STRING _5C_1C = INPUT_STRING.INPUT_STRING_5C_1C;
        public const INPUT_STRING _4C_1C = INPUT_STRING.INPUT_STRING_4C_1C;
        public const INPUT_STRING _3C_1C = INPUT_STRING.INPUT_STRING_3C_1C;
        public const INPUT_STRING _2C_1C = INPUT_STRING.INPUT_STRING_2C_1C;
        public const INPUT_STRING _1C_1C = INPUT_STRING.INPUT_STRING_1C_1C;
        protected const INPUT_STRING _2C_3C = INPUT_STRING.INPUT_STRING_2C_3C;
        public const INPUT_STRING _1C_2C = INPUT_STRING.INPUT_STRING_1C_2C;
        public const INPUT_STRING _1C_3C = INPUT_STRING.INPUT_STRING_1C_3C;
        public const INPUT_STRING _1C_4C = INPUT_STRING.INPUT_STRING_1C_4C;
        public const INPUT_STRING _1C_5C = INPUT_STRING.INPUT_STRING_1C_5C;
        public const INPUT_STRING _1C_6C = INPUT_STRING.INPUT_STRING_1C_6C;
        public const INPUT_STRING _1C_7C = INPUT_STRING.INPUT_STRING_1C_7C;
        public const INPUT_STRING _1C_8C = INPUT_STRING.INPUT_STRING_1C_8C;
        public const INPUT_STRING Free_Play = INPUT_STRING.INPUT_STRING_Free_Play;
        protected const INPUT_STRING Cabinet = INPUT_STRING.INPUT_STRING_Cabinet;
        protected const INPUT_STRING Upright = INPUT_STRING.INPUT_STRING_Upright;
        protected const INPUT_STRING Cocktail = INPUT_STRING.INPUT_STRING_Cocktail;
        protected const INPUT_STRING Flip_Screen = INPUT_STRING.INPUT_STRING_Flip_Screen;
        protected const INPUT_STRING Language = INPUT_STRING.INPUT_STRING_Language;
        protected const INPUT_STRING English = INPUT_STRING.INPUT_STRING_English;
        protected const INPUT_STRING Japanese = INPUT_STRING.INPUT_STRING_Japanese;
        protected const INPUT_STRING Chinese = INPUT_STRING.INPUT_STRING_Chinese;
        protected const INPUT_STRING French = INPUT_STRING.INPUT_STRING_French;
        protected const INPUT_STRING German = INPUT_STRING.INPUT_STRING_German;
        protected const INPUT_STRING Italian = INPUT_STRING.INPUT_STRING_Italian;
        protected const INPUT_STRING Korean = INPUT_STRING.INPUT_STRING_Korean;
        protected const INPUT_STRING Spanish = INPUT_STRING.INPUT_STRING_Spanish;
        protected const INPUT_STRING Easiest = INPUT_STRING.INPUT_STRING_Easiest;
        protected const INPUT_STRING Easy = INPUT_STRING.INPUT_STRING_Easy;
        protected const INPUT_STRING Normal = INPUT_STRING.INPUT_STRING_Normal;
        protected const INPUT_STRING Medium = INPUT_STRING.INPUT_STRING_Medium;
        protected const INPUT_STRING Medium_Hard = INPUT_STRING.INPUT_STRING_Medium_Hard;
        protected const INPUT_STRING Hard = INPUT_STRING.INPUT_STRING_Hard;
        protected const INPUT_STRING Hardest = INPUT_STRING.INPUT_STRING_Hardest;
        protected const INPUT_STRING Difficult = INPUT_STRING.INPUT_STRING_Difficult;
        protected const INPUT_STRING Very_Difficult = INPUT_STRING.INPUT_STRING_Very_Difficult;
        protected const INPUT_STRING Allow_Continue = INPUT_STRING.INPUT_STRING_Allow_Continue;
        protected const INPUT_STRING Unused = INPUT_STRING.INPUT_STRING_Unused;
        protected const INPUT_STRING Unknown = INPUT_STRING.INPUT_STRING_Unknown;
        protected const INPUT_STRING Alternate = INPUT_STRING.INPUT_STRING_Alternate;
        protected const INPUT_STRING None = INPUT_STRING.INPUT_STRING_None;
        protected void INPUT_PORTS_START(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            // if we're inside PORT_INCLUDE, we already have a configurer, and don't need to create a new one
            if (!m_globals.helper_originated_from_port_include)
            {
                ioport_configurer configurer = new ioport_configurer(owner, portlist, ref errorbuf);
                m_globals.helper_configurer = configurer;
                m_globals.helper_owner = owner;
                m_globals.helper_portlist = portlist;
            }
        }
        protected void INPUT_PORTS_END()
        {
            // if we're inside PORT_INCLUDE, don't null out our helper variables, we need them still
            if (!m_globals.helper_originated_from_port_include)
            {
                m_globals.helper_configurer = null;
                m_globals.helper_owner = null;
                m_globals.helper_portlist = null;
            }
        }
        protected void PORT_INCLUDE(ioport_constructor name, ref string errorbuf)
        {
            m_globals.helper_originated_from_port_include = true;
            ioport_global.PORT_INCLUDE(name, m_globals.helper_owner, m_globals.helper_portlist, ref errorbuf);
            m_globals.helper_originated_from_port_include = false;
        }
        protected void PORT_START(string tag) { ioport_global.PORT_START(m_globals.helper_configurer, tag); }
        protected void PORT_MODIFY(string tag) { ioport_global.PORT_MODIFY(m_globals.helper_configurer, tag); }
        protected void PORT_BIT(ioport_value mask, ioport_value default_, ioport_type type) { ioport_global.PORT_BIT(m_globals.helper_configurer, mask, default_, type); }
        protected void PORT_CODE(input_code code) { ioport_global.PORT_CODE(m_globals.helper_configurer, code); }
        protected void PORT_2WAY() { ioport_global.PORT_2WAY(m_globals.helper_configurer); }
        protected void PORT_4WAY() { ioport_global.PORT_4WAY(m_globals.helper_configurer); }
        protected void PORT_8WAY() { ioport_global.PORT_8WAY(m_globals.helper_configurer); }
        protected void PORT_NAME(string _name) { ioport_global.PORT_NAME(m_globals.helper_configurer, _name); }
        protected void PORT_PLAYER(int player) { ioport_global.PORT_PLAYER(m_globals.helper_configurer, player); }
        protected void PORT_COCKTAIL() { ioport_global.PORT_COCKTAIL(m_globals.helper_configurer); }
        protected void PORT_IMPULSE(u8 duration) { ioport_global.PORT_IMPULSE(m_globals.helper_configurer, duration); }
        protected void PORT_REVERSE() { ioport_global.PORT_REVERSE(m_globals.helper_configurer); }
        protected void PORT_MINMAX(ioport_value _min, ioport_value _max) { ioport_global.PORT_MINMAX(m_globals.helper_configurer, _min, _max); }
        protected void PORT_SENSITIVITY(int sensitivity) { ioport_global.PORT_SENSITIVITY(m_globals.helper_configurer, sensitivity); }
        protected void PORT_KEYDELTA(int delta) { ioport_global.PORT_KEYDELTA(m_globals.helper_configurer, delta); }
        protected void PORT_FULL_TURN_COUNT(u16 _count) { ioport_global.PORT_FULL_TURN_COUNT(m_globals.helper_configurer, _count); }
        protected void PORT_CUSTOM_MEMBER(string device, ioport_field_read_delegate callback) { ioport_global.PORT_CUSTOM_MEMBER(m_globals.helper_configurer, device, callback); }
        protected void PORT_READ_LINE_DEVICE_MEMBER(string device, Func<int> _member) { ioport_global.PORT_READ_LINE_DEVICE_MEMBER(m_globals.helper_configurer, device, _member); }
        public void PORT_DIPNAME(ioport_value mask, ioport_value default_, string name) { ioport_global.PORT_DIPNAME(m_globals.helper_configurer, mask, default_, name); }
        public void PORT_DIPSETTING(ioport_value default_, string name) { ioport_global.PORT_DIPSETTING(m_globals.helper_configurer, default_, name); }
        public void PORT_DIPLOCATION(string location) { ioport_global.PORT_DIPLOCATION(m_globals.helper_configurer, location); }
        public void PORT_CONDITION(string tag, ioport_value mask, ioport_condition.condition_t condition, ioport_value value) { ioport_global.PORT_CONDITION(m_globals.helper_configurer, tag, mask, condition, value); }
        protected void PORT_ADJUSTER(ioport_value default_, string name) { ioport_global.PORT_ADJUSTER(m_globals.helper_configurer, default_, name); }
        protected void PORT_CONFNAME(ioport_value mask, ioport_value default_, string name) { ioport_global.PORT_CONFNAME(m_globals.helper_configurer, mask, default_, name); }
        protected void PORT_CONFSETTING(ioport_value default_, string name) { ioport_global.PORT_CONFSETTING(m_globals.helper_configurer, default_, name); }
        protected void PORT_DIPUNUSED_DIPLOC(ioport_value mask, ioport_value default_, string diploc) { ioport_global.PORT_DIPUNUSED_DIPLOC(m_globals.helper_configurer, mask, default_, diploc); }
        protected void PORT_DIPUNUSED(ioport_value mask, ioport_value default_) { ioport_global.PORT_DIPUNUSED(m_globals.helper_configurer, mask, default_); }
        protected void PORT_DIPUNKNOWN_DIPLOC(ioport_value mask, ioport_value default_, string diploc) { ioport_global.PORT_DIPUNKNOWN_DIPLOC(m_globals.helper_configurer, mask, default_, diploc); }
        protected void PORT_SERVICE_DIPLOC(ioport_value mask, ioport_value default_, string diploc) { ioport_global.PORT_SERVICE_DIPLOC(m_globals.helper_configurer, mask, default_, diploc); }
        protected void PORT_SERVICE(ioport_value mask, ioport_value default_) { ioport_global.PORT_SERVICE(m_globals.helper_configurer, mask, default_); }
        protected void PORT_VBLANK(string screen) { ioport_global.PORT_VBLANK(m_globals.helper_configurer, screen, (screen_device)m_globals.helper_owner.subdevice(screen)); }
        public static string DEF_STR(INPUT_STRING str_num) { return ioport_global.DEF_STR(str_num); }


        // irem
        protected static m52_soundc_audio_device IREM_M52_SOUNDC_AUDIO(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<m52_soundc_audio_device>(mconfig, tag, m52_soundc_audio_device.IREM_M52_SOUNDC_AUDIO, clock); }


        // latch8
        protected static latch8_device LATCH8(machine_config mconfig, device_finder<latch8_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, latch8_device.LATCH8, clock); }


        // logmacro
        protected const int LOG_WARN = 1 << 0;

        protected static void LOGMASKED(int mask, device_t device, string format, params object [] args) { logmacro_global.LOGMASKED(mask, device, format, args); }
        protected static void LOG(device_t device, string format, params object [] args) { logmacro_global.LOG(device, format, args); }


        // main
        protected const int EMU_ERR_NONE = main_global.EMU_ERR_NONE;
        protected const int EMU_ERR_FAILED_VALIDITY = main_global.EMU_ERR_FAILED_VALIDITY;
        protected const int EMU_ERR_MISSING_FILES = main_global.EMU_ERR_MISSING_FILES;
        protected const int EMU_ERR_DEVICE = main_global.EMU_ERR_DEVICE;
        protected const int EMU_ERR_NO_SUCH_SYSTEM = main_global.EMU_ERR_NO_SUCH_SYSTEM;
        protected const int EMU_ERR_INVALID_CONFIG = main_global.EMU_ERR_INVALID_CONFIG;


        // m6502
        protected static m6502_device M6502(machine_config mconfig, device_finder<m6502_device> finder, u32 clock) { return emu.detail.device_type_impl.op(mconfig, finder, m6502_device.M6502, clock); }
        protected static m6502_device M6502(machine_config mconfig, device_finder<m6502_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, m6502_device.M6502, clock); }


        // m6801
        protected static m6803_cpu_device M6803(machine_config mconfig, device_finder<m6803_cpu_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, m6803_cpu_device.M6803, clock); }


        // m68705
        protected static m68705p_device M68705P5(machine_config mconfig, device_finder<m68705p_device> finder, u32 clock) { return emu.detail.device_type_impl.op(mconfig, finder, m68705p5_device.M68705P5, clock); }


        // machine
        protected const int DEBUG_FLAG_ENABLED = machine_global.DEBUG_FLAG_ENABLED;
        protected static MemoryContainer<T> auto_alloc_array<T>(running_machine m, UInt32 c) where T : new() { return machine_global.auto_alloc_array<T>(m, c); }
        protected static MemoryContainer<T> auto_alloc_array_clear<T>(running_machine m, UInt32 c) where T : new() { return machine_global.auto_alloc_array_clear<T>(m, c); }


        // mcs48
        protected static mcs48_cpu_device MB8884(machine_config mconfig, device_finder<mcs48_cpu_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, mb8884_device.MB8884, clock); }


        // mb88xx
        protected static mb88_cpu_device MB8842(machine_config mconfig, device_finder<mb88_cpu_device> finder, u32 clock) { return emu.detail.device_type_impl.op(mconfig, finder, mb8842_cpu_device.MB8842, clock); }
        protected static mb88_cpu_device MB8843(machine_config mconfig, device_finder<mb88_cpu_device> finder, u32 clock) { return emu.detail.device_type_impl.op(mconfig, finder, mb8843_cpu_device.MB8843, clock); }
        protected static mb88_cpu_device MB8844(machine_config mconfig, device_finder<mb88_cpu_device> finder, u32 clock) { return emu.detail.device_type_impl.op(mconfig, finder, mb8844_cpu_device.MB8844, clock); }


        // msm5205
        protected static msm5205_device MSM5205(machine_config mconfig, device_finder<msm5205_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, msm5205_device.MSM5205, clock); }


        // namco
        protected static namco_device NAMCO(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<namco_device>(mconfig, tag, namco_device.NAMCO, clock); }
        protected static namco_device NAMCO(machine_config mconfig, device_finder<namco_device> finder, u32 clock) { return emu.detail.device_type_impl.op(mconfig, finder, namco_device.NAMCO, clock); }
        protected static namco_device NAMCO(machine_config mconfig, device_finder<namco_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, namco_device.NAMCO, clock); }


        // namco06
        protected static namco_06xx_device NAMCO_06XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_06xx_device>(mconfig, tag, namco_06xx_device.NAMCO_06XX, clock); }


        // namco50
        protected static namco_50xx_device NAMCO_50XX(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<namco_50xx_device>(mconfig, tag, namco_50xx_device.NAMCO_50XX, clock); }
        protected static namco_50xx_device NAMCO_50XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_50xx_device>(mconfig, tag, namco_50xx_device.NAMCO_50XX, clock); }


        // namco51
        protected static namco_51xx_device NAMCO_51XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_51xx_device>(mconfig, tag, namco_51xx_device.NAMCO_51XX, clock); }


        // namco53
        protected static namco_53xx_device NAMCO_53XX(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<namco_53xx_device>(mconfig, tag, namco_53xx_device.NAMCO_53XX, clock); }
        protected static namco_53xx_device NAMCO_53XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_53xx_device>(mconfig, tag, namco_53xx_device.NAMCO_53XX, clock); }


        // namco54
        protected static namco_54xx_device NAMCO_54XX(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<namco_54xx_device>(mconfig, tag, namco_54xx_device.NAMCO_54XX, clock); }
        protected static namco_54xx_device NAMCO_54XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_54xx_device>(mconfig, tag, namco_54xx_device.NAMCO_54XX, clock); }
        protected static int NAMCO_54XX_0_DATA(int base_node) { return namco54_global.NAMCO_54XX_0_DATA(base_node); }
        protected static int NAMCO_54XX_1_DATA(int base_node) { return namco54_global.NAMCO_54XX_1_DATA(base_node); }
        protected static int NAMCO_54XX_2_DATA(int base_node) { return namco54_global.NAMCO_54XX_2_DATA(base_node); }


        // net_lib
        protected void SOLVER(string name, int freq) { netlist.devices.net_lib_global.SOLVER(m_globals.helper_setup, name, freq); }


        // netlist
        protected static netlist_mame_sound_device NETLIST_SOUND(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<netlist_mame_sound_device>(mconfig, tag, netlist_mame_sound_device.NETLIST_SOUND, clock); }
        protected static netlist_mame_sound_device NETLIST_SOUND(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<netlist_mame_sound_device>(mconfig, tag, netlist_mame_sound_device.NETLIST_SOUND, clock); }
        protected static netlist_mame_logic_input_device NETLIST_LOGIC_INPUT(machine_config mconfig, string tag, string param_name, uint32_t shift)
        {
            var device = emu.detail.device_type_impl.op<netlist_mame_logic_input_device>(mconfig, tag, netlist_mame_logic_input_device.NETLIST_LOGIC_INPUT, 0);
            device.set_params(param_name, shift);
            return device;
        }
        protected static netlist_mame_stream_input_device NETLIST_STREAM_INPUT(machine_config mconfig, string tag, int channel, string param_name)
        {
            var device = emu.detail.device_type_impl.op<netlist_mame_stream_input_device>(mconfig, tag, netlist_mame_stream_input_device.NETLIST_STREAM_INPUT, 0);
            device.set_params(channel, param_name);
            return device;
        }
        protected static netlist_mame_stream_output_device NETLIST_STREAM_OUTPUT(machine_config mconfig, string tag, int channel, string out_name)
        {
            var device = emu.detail.device_type_impl.op<netlist_mame_stream_output_device>(mconfig, tag, netlist_mame_stream_output_device.NETLIST_STREAM_OUTPUT, 0);
            device.set_params(channel, out_name);
            return device;
        }


        // nl_factory
        protected static netlist.factory.constructor_ptr_t NETLIB_DEVICE_IMPL_ALIAS<chip>(string p_alias, string p_name, string p_def_param) { return netlist.factory.nl_factory_global.NETLIB_DEVICE_IMPL_ALIAS<chip>(p_alias, p_name, p_def_param); }
        protected static netlist.factory.constructor_ptr_t NETLIB_DEVICE_IMPL<chip>(string p_name, string p_def_param) { return netlist.factory.nl_factory_global.NETLIB_DEVICE_IMPL<chip>(p_name, p_def_param); }
        protected static netlist.factory.constructor_ptr_t NETLIB_DEVICE_IMPL_NS<chip>(string ns, string p_name, string p_def_param) { return netlist.factory.nl_factory_global.NETLIB_DEVICE_IMPL_NS<chip>(ns, p_name, p_def_param); }


        // nl_setup
        protected void ALIAS(string alias, string name) { netlist.nl_setup_global.ALIAS(m_globals.helper_setup, alias, name); }
        protected void INCLUDE(string name) { netlist.nl_setup_global.INCLUDE(m_globals.helper_setup, name); }
        protected void LOCAL_SOURCE(string name, nlsetup_func netlist_name) { netlist.nl_setup_global.LOCAL_SOURCE(m_globals.helper_setup, name, netlist_name); }
        protected void NET_C(params string [] term1) { netlist.nl_setup_global.NET_C(m_globals.helper_setup, term1); }
        protected void PARAM(string name, int val) { netlist.nl_setup_global.PARAM(m_globals.helper_setup, name, val); }
        protected void PARAM(string name, double val) { netlist.nl_setup_global.PARAM(m_globals.helper_setup, name, val); }
        protected void PARAM(string name, string val) { netlist.nl_setup_global.PARAM(m_globals.helper_setup, name, val); }
        protected void SUBMODEL(string model, string name) { netlist.nl_setup_global.SUBMODEL(m_globals.helper_setup, model, name); }
        protected void NETLIST_START(netlist.nlparse_t setup) { m_globals.helper_setup_push(setup);  netlist.nl_setup_global.NETLIST_START(); }
        protected void NETLIST_END() { m_globals.helper_setup_pop();  netlist.nl_setup_global.NETLIST_END(); }


        // options
        protected const int OPTION_PRIORITY_DEFAULT = options_global.OPTION_PRIORITY_DEFAULT;
        protected const int OPTION_PRIORITY_NORMAL = options_global.OPTION_PRIORITY_NORMAL;
        protected const int OPTION_PRIORITY_HIGH = options_global.OPTION_PRIORITY_HIGH;
        public const int OPTION_PRIORITY_MAXIMUM = options_global.OPTION_PRIORITY_MAXIMUM;
        protected const core_options.option_type OPTION_HEADER = options_global.OPTION_HEADER;
        protected const core_options.option_type OPTION_COMMAND = options_global.OPTION_COMMAND;
        protected const core_options.option_type OPTION_BOOLEAN = options_global.OPTION_BOOLEAN;
        protected const core_options.option_type OPTION_INTEGER = options_global.OPTION_INTEGER;
        protected const core_options.option_type OPTION_FLOAT = options_global.OPTION_FLOAT;
        protected const core_options.option_type OPTION_STRING = options_global.OPTION_STRING;


        // osdcomm
        public static uint32_t swapendian_int32(uint32_t val) { return osdcomm_global.swapendian_int32(val); }
        public static int16_t little_endianize_int16(int16_t x) { return osdcomm_global.little_endianize_int16(x); }
        public static uint16_t little_endianize_int16(uint16_t x) { return osdcomm_global.little_endianize_int16(x); }
        public static int32_t little_endianize_int32(int32_t x) { return osdcomm_global.little_endianize_int32(x); }
        public static uint32_t little_endianize_int32(uint32_t x) { return osdcomm_global.little_endianize_int32(x); }


        // osdcore
        public const string PATH_SEPARATOR = osdcore_global.PATH_SEPARATOR;
        public const UInt32 OPEN_FLAG_READ = osdcore_global.OPEN_FLAG_READ;
        protected const UInt32 OPEN_FLAG_WRITE = osdcore_global.OPEN_FLAG_WRITE;
        protected const UInt32 OPEN_FLAG_CREATE = osdcore_global.OPEN_FLAG_CREATE;
        protected const UInt32 OPEN_FLAG_CREATE_PATHS = osdcore_global.OPEN_FLAG_CREATE_PATHS;
        protected const UInt32 OPEN_FLAG_NO_PRELOAD = osdcore_global.OPEN_FLAG_NO_PRELOAD;
        public static void osd_printf_error(string format, params object [] args) { osdcore_interface.osd_printf_error(format, args); }
        public static void osd_printf_warning(string format, params object [] args) { osdcore_interface.osd_printf_warning(format, args); }
        public static void osd_printf_info(string format, params object [] args) { osdcore_interface.osd_printf_info(format, args); }
        public static void osd_printf_verbose(string format, params object [] args) { osdcore_interface.osd_printf_verbose(format, args); }
        public static void osd_printf_debug(string format, params object [] args) { osdcore_interface.osd_printf_debug(format, args); }


        // palette
        protected static uint8_t pal5bit(uint8_t bits) { return palette_global.pal5bit(bits); }
        protected static rgb_t rgbexpand(int _RBits, int _GBits, int _BBits, UInt32 data, byte rshift, byte gshift, byte bshift) { return palette_global.rgbexpand(_RBits, _GBits, _BBits, data, rshift, gshift, bshift); }


        // pokey
        protected static pokey_device POKEY(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<pokey_device>(mconfig, tag, pokey_device.POKEY, clock); }
        protected static pokey_device POKEY(machine_config mconfig, device_finder<pokey_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, pokey_device.POKEY, clock); }


        // render
        public static u32 PRIMFLAG_BLENDMODE(u32 x) { return render_global.PRIMFLAG_BLENDMODE(x); }
        protected static u32 PRIMFLAG_TEXWRAP(u32 x) { return render_global.PRIMFLAG_TEXWRAP(x); }
        protected const u32 PRIMFLAG_PACKABLE = render_global.PRIMFLAG_PACKABLE;


        // rendutil
        protected static void render_load_jpeg(out bitmap_argb32 bitmap, util.core_file file) { rendutil_global.render_load_jpeg(out bitmap, file); }
        protected static bool render_load_png(out bitmap_argb32 bitmap, util.core_file file, bool load_as_alpha_to_existing = false) { return rendutil_global.render_load_png(out bitmap, file, load_as_alpha_to_existing); }


        // rescap
        protected static double RES_K(double res) { return rescap_global.RES_K(res); }
        protected static double RES_M(double res) { return rescap_global.RES_M(res); }
        protected static double CAP_U(double cap) { return rescap_global.CAP_U(cap); }
        protected static double CAP_N(double cap) { return rescap_global.CAP_N(cap); }
        protected static double RES_VOLTAGE_DIVIDER(double r1, double r2) { return rescap_global.RES_VOLTAGE_DIVIDER(r1, r2); }
        protected static double RES_2_PARALLEL(double r1, double r2) { return rescap_global.RES_2_PARALLEL(r1, r2); }
        protected static double RES_3_PARALLEL(double r1, double r2, double r3) { return rescap_global.RES_3_PARALLEL(r1, r2, r3); }


        // resnet
        protected const u32 RES_NET_AMP_DARLINGTON = resnet_global.RES_NET_AMP_DARLINGTON;
        protected const u32 RES_NET_AMP_EMITTER = resnet_global.RES_NET_AMP_EMITTER;
        protected const u32 RES_NET_VCC_5V = resnet_global.RES_NET_VCC_5V;
        protected const u32 RES_NET_VBIAS_5V = resnet_global.RES_NET_VBIAS_5V;
        protected const u32 RES_NET_VBIAS_TTL = resnet_global.RES_NET_VBIAS_TTL;
        protected const u32 RES_NET_VIN_VCC = resnet_global.RES_NET_VIN_VCC;
        protected const u32 RES_NET_VIN_TTL_OUT = resnet_global.RES_NET_VIN_TTL_OUT;
        protected const u32 RES_NET_MONITOR_SANYO_EZV20 = resnet_global.RES_NET_MONITOR_SANYO_EZV20;
        protected const u32 RES_NET_VIN_MB7052 = resnet_global.RES_NET_VIN_MB7052;
        protected static int compute_res_net(int inputs, int channel, res_net_info di) { return resnet_global.compute_res_net(inputs, channel, di); }
        protected static void compute_res_net_all(out std.vector<rgb_t> rgb, Pointer<u8> prom, res_net_decode_info rdi, res_net_info di) { resnet_global.compute_res_net_all(out rgb, prom, rdi, di); }
        protected static double compute_resistor_weights(int minval, int maxval, double scaler, int count_1, int [] resistances_1, out double [] weights_1, int pulldown_1, int pullup_1, int count_2, int [] resistances_2, out double [] weights_2, int pulldown_2, int pullup_2, int count_3, int [] resistances_3, out double [] weights_3, int pulldown_3, int pullup_3 ) { return resnet_global.compute_resistor_weights(minval, maxval, scaler, count_1, resistances_1, out weights_1, pulldown_1, pullup_1, count_2, resistances_2, out weights_2, pulldown_2, pullup_2, count_3, resistances_3, out weights_3, pulldown_3, pullup_3); }
        protected static int combine_weights(double [] tab, int w0, int w1, int w2) { return resnet_global.combine_weights(tab, w0, w1, w2); }
        protected static int combine_weights(double [] tab, int w0, int w1) { return resnet_global.combine_weights(tab, w0, w1); }


        // romentry
        protected const           UInt32 ROMREGION_INVERT = romentry_global.ROMREGION_INVERT;
        protected static readonly UInt32 ROMREGION_ERASE00 = romentry_global.ROMREGION_ERASE00;
        protected static readonly UInt32 ROMREGION_ERASEFF = romentry_global.ROMREGION_ERASEFF;
        protected static tiny_rom_entry ROM_END { get { return romentry_global.ROM_END; } }
        protected static tiny_rom_entry ROM_REGION(UInt32 length, string tag, UInt32 flags) { return romentry_global.ROM_REGION(length, tag, flags); }
        protected static tiny_rom_entry ROM_LOAD(string name, UInt32 offset, UInt32 length, string hash) { return romentry_global.ROM_LOAD(name, offset, length, hash); }
        protected static tiny_rom_entry ROM_CONTINUE(u32 offset, u32 length) { return romentry_global.ROM_CONTINUE(offset, length); }
        protected static tiny_rom_entry ROM_FILL(UInt32 offset, UInt32 length, byte value) { return romentry_global.ROM_FILL(offset, length, value); }
        protected static tiny_rom_entry ROM_LOAD16_BYTE(string name, u32 offset, u32 length, string hash) { return romentry_global.ROM_LOAD16_BYTE(name, offset, length, hash); }
        protected static tiny_rom_entry ROM_RELOAD(UInt32 offset, UInt32 length) { return romentry_global.ROM_RELOAD(offset, length); }


        // romload
        protected static bool ROMENTRY_ISFILE(rom_entry_interface r) { return romload_global.ROMENTRY_ISFILE(r); }
        protected static bool ROMENTRY_ISEND(rom_entry_interface r) { return romload_global.ROMENTRY_ISEND(r); }
        protected static bool ROMENTRY_ISSYSTEM_BIOS(rom_entry_interface r) { return romload_global.ROMENTRY_ISSYSTEM_BIOS(r); }
        protected static bool ROMENTRY_ISDEFAULT_BIOS(rom_entry_interface r) { return romload_global.ROMENTRY_ISDEFAULT_BIOS(r); }
        protected static bool ROMREGION_ISROMDATA(rom_entry_interface r) { return romload_global.ROMREGION_ISROMDATA(r); }
        protected static bool ROMREGION_ISDISKDATA(rom_entry_interface r) { return romload_global.ROMREGION_ISDISKDATA(r); }
        protected static string ROM_GETNAME(rom_entry_interface r) { return romload_global.ROM_GETNAME(r); }
        protected static string ROM_GETHASHDATA(rom_entry_interface r) { return romload_global.ROM_GETHASHDATA(r); }
        protected static UInt32 ROM_GETBIOSFLAGS(rom_entry_interface r) { return romload_global.ROM_GETBIOSFLAGS(r); }
        protected static std.vector<rom_entry> rom_build_entries(Pointer<tiny_rom_entry> tinyentries) { return romload_global.rom_build_entries(tinyentries); }


        // screen
        protected static screen_device SCREEN(machine_config mconfig, string tag, screen_type_enum type)
        {
            var device = emu.detail.device_type_impl.op<screen_device>(mconfig, tag, screen_device.SCREEN, 0);
            device.screen_device_after_ctor(type);
            return device;
        }
        protected static screen_device SCREEN(machine_config mconfig, device_finder<screen_device> finder, screen_type_enum type)
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, screen_device.SCREEN, 0);
            device.screen_device_after_ctor(type);
            return device;
        }
        protected const screen_type_enum SCREEN_TYPE_RASTER = screen_type_enum.SCREEN_TYPE_RASTER;
        protected const screen_type_enum SCREEN_TYPE_VECTOR = screen_type_enum.SCREEN_TYPE_VECTOR;
        protected const screen_type_enum SCREEN_TYPE_LCD = screen_type_enum.SCREEN_TYPE_LCD;
        protected const screen_type_enum SCREEN_TYPE_SVG = screen_type_enum.SCREEN_TYPE_SVG;
        protected const screen_type_enum SCREEN_TYPE_INVALID = screen_type_enum.SCREEN_TYPE_INVALID;


        // slapstic
        protected static atari_slapstic_device SLAPSTIC(machine_config mconfig, device_finder<atari_slapstic_device> finder, int chipnum, bool m68k_mode)
        {
            var device = emu.detail.device_type_impl.op<atari_slapstic_device>(mconfig, finder, atari_slapstic_device.SLAPSTIC, 0);
            device.atari_slapstic_device_after_ctor(chipnum, m68k_mode);
            return device;
        }


        // speaker
        protected static speaker_device SPEAKER(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<speaker_device>(mconfig, tag, speaker_device.SPEAKER, 0); }


        // starfield
        protected static starfield_05xx_device STARFIELD_05XX(machine_config mconfig, device_finder<starfield_05xx_device> finder, uint32_t clock) { return emu.detail.device_type_impl.op(mconfig, finder, starfield_05xx_device.STARFIELD_05XX, clock); }


        // strformat
        public static string string_format(string format, params object [] args) { return util_.string_format(format, args); }


        // t11
        protected static t11_device T11(machine_config mconfig, device_finder<t11_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, t11_device.T11, clock); }


        // taitosjsec
        protected static taito_sj_security_mcu_device TAITO_SJ_SECURITY_MCU(machine_config mconfig, device_finder<taito_sj_security_mcu_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, taito_sj_security_mcu_device.TAITO_SJ_SECURITY_MCU, clock); }


        // tilemap
        protected const u32 TILEMAP_DRAW_OPAQUE = tilemap_global.TILEMAP_DRAW_OPAQUE;
        protected const u8 TILE_FLIPX = tilemap_global.TILE_FLIPX;
        protected const u8 TILE_FORCE_LAYER0 = tilemap_global.TILE_FORCE_LAYER0;
        protected const u32 TILEMAP_FLIPX = tilemap_global.TILEMAP_FLIPX;
        protected const u32 TILEMAP_FLIPY = tilemap_global.TILEMAP_FLIPY;
        protected static u8 TILE_FLIPYX(int YX) { return tilemap_global.TILE_FLIPYX(YX); }
        protected static tilemap_device TILEMAP(machine_config mconfig, device_finder<tilemap_device> finder, string gfxtag, int entrybytes, u16 tilewidth, u16 tileheight, tilemap_standard_mapper mapper, u32 columns, u32 rows)
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, tilemap_device.TILEMAP, 0);
            device.tilemap_device_after_ctor(gfxtag, entrybytes, tilewidth, tileheight, mapper, columns, rows);
            return device;
        }
        protected static tilemap_device TILEMAP(machine_config mconfig, device_finder<tilemap_device> finder, string gfxtag, int entrybytes, u16 tilewidth, u16 tileheight, tilemap_standard_mapper mapper, u32 columns, u32 rows, pen_t transpen)
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, tilemap_device.TILEMAP, 0);
            device.tilemap_device_after_ctor(gfxtag, entrybytes, tilewidth, tileheight, mapper, columns, rows, transpen);
            return device;
        }


        // timer
        protected static timer_device TIMER(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<timer_device>(mconfig, tag, timer_device.TIMER, clock); }


        // tms5220
        protected static tms5220c_device TMS5220C(machine_config mconfig, device_finder<tms5220c_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, tms5220c_device.TMS5220C, clock); }


        // ui
        protected const int SLIDER_ID_MIXERVOL = (int)slider_id.SLIDER_ID_MIXERVOL;
        protected const int SLIDER_ID_ADJUSTER = (int)slider_id.SLIDER_ID_ADJUSTER;
        protected const int SLIDER_ID_ADJUSTER_LAST = (int)slider_id.SLIDER_ID_ADJUSTER_LAST;
        protected const float UI_MAX_FONT_HEIGHT = ui_global.UI_MAX_FONT_HEIGHT;
        public const float UI_LINE_WIDTH = ui_global.UI_LINE_WIDTH;
        protected static readonly rgb_t UI_GREEN_COLOR = ui_global.UI_GREEN_COLOR;
        protected static readonly rgb_t UI_YELLOW_COLOR = ui_global.UI_YELLOW_COLOR;
        protected static readonly rgb_t UI_RED_COLOR = ui_global.UI_RED_COLOR;
        public const uint32_t UI_HANDLER_CANCEL = ui_global.UI_HANDLER_CANCEL;


        // watchdog
        protected static watchdog_timer_device WATCHDOG_TIMER(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<watchdog_timer_device>(mconfig, tag, watchdog_timer_device.WATCHDOG_TIMER, 0); }
        protected static watchdog_timer_device WATCHDOG_TIMER(machine_config mconfig, device_finder<watchdog_timer_device> finder) { return emu.detail.device_type_impl.op(mconfig, finder, watchdog_timer_device.WATCHDOG_TIMER, 0); }


        // ym2151
        protected static ym2151_device YM2151(machine_config mconfig, device_finder<ym2151_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, ym2151_device.YM2151, clock); }


        // z80
        protected static cpu_device Z80(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<cpu_device>(mconfig, tag, z80_device.Z80, clock); }
        protected static cpu_device Z80(machine_config mconfig, device_finder<cpu_device> finder, u32 clock) { return emu.detail.device_type_impl.op(mconfig, finder, z80_device.Z80, clock); }
        protected static cpu_device Z80(machine_config mconfig, device_finder<cpu_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, z80_device.Z80, clock); }


        // c++
        public static int sizeof_(object value)
        {
            // alternative to System.Runtime.InteropServices.Marshal.SizeOf(typeof(NativeType)); ?

            if (value is sbyte)       return 1;
            else if (value is byte)   return 1;
            else if (value is Int16)  return 2;
            else if (value is UInt16) return 2;
            else if (value is Int32)  return 4;
            else if (value is UInt32) return 4;
            else if (value is Int64)  return 8;
            else if (value is UInt64) return 8;
            else if (value is Type)
            {
                if ((Type)value == typeof(sbyte))       return 1;
                else if ((Type)value == typeof(byte))   return 1;
                else if ((Type)value == typeof(Int16))  return 2;
                else if ((Type)value == typeof(UInt16)) return 2;
                else if ((Type)value == typeof(Int32))  return 4;
                else if ((Type)value == typeof(UInt32)) return 4;
                else if ((Type)value == typeof(Int64))  return 8;
                else if ((Type)value == typeof(UInt64)) return 8;
                else throw new emu_unimplemented();
            }
            else throw new emu_unimplemented();
        }


        // c++ float.h  - https://www.johndcook.com/blog/2012/01/05/double-epsilon-dbl_epsilon/
        protected const double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */


        // c++ math.h
        protected const double M_PI = Math.PI;
        protected static float fabs(float arg) { return std.fabs(arg); }
        protected static double fabs(double arg) { return std.fabs(arg); }
        protected static float floor(float x) { return std.floor(x); }
        protected static double floor(double x) { return std.floor(x); }
        protected static float log(float arg) { return std.log(arg); }
        protected static double log(double arg) { return std.log(arg); }
        protected static int lround(double x) { return std.lround(x); }
        protected static float pow(float base_, float exponent) { return std.pow(base_, exponent); }
        protected static double pow(double base_, double exponent) { return std.pow(base_, exponent); }
        protected static float sin(float arg) { return std.sin(arg); }
        protected static double sin(double arg) { return std.sin(arg); }


        // c++ stdio.h
        protected static int memcmp<T>(MemoryContainer<T> ptr1, MemoryContainer<T> ptr2, UInt32 num) { return ptr1.CompareTo(ptr2, (int)num) ? 0 : 1; }  //  const void * ptr1, const void * ptr2, size_t num
        protected static int memcmp<T>(Pointer<T> ptr1, Pointer<T> ptr2, UInt32 num) { return ptr1.CompareTo(ptr2, (int)num) ? 0 : 1; }  //  const void * ptr1, const void * ptr2, size_t num
        protected static void memcpy<T>(MemoryContainer<T> destination, MemoryContainer<T> source, UInt32 num) { std.memcpy(destination, source, num); }  //  void * destination, const void * source, size_t num );
        protected static void memcpy<T>(Pointer<T> destination, Pointer<T> source, UInt32 num) { std.memcpy(destination, source, num); }  //  void * destination, const void * source, size_t num );
        protected static void memset<T>(MemoryContainer<T> destination, T value) { std.memset(destination, value); }
        protected static void memset<T>(MemoryContainer<T> destination, T value, UInt32 num) { std.memset(destination, value, num); }
        protected static void memset<T>(Pointer<T> destination, T value, UInt32 num) { std.memset(destination, value, num); }
        protected static void memset<T>(IList<T> destination, T value) { std.memset(destination, value, (UInt32)destination.Count); }
        protected static void memset<T>(IList<T> destination, T value, UInt32 num) { std.memset(destination, value, num); }
        protected static void memset<T>(T [,] destination, T value) { std.memset(destination, value); }


        // c++ string.h
        protected static MemoryContainer<T> memmove<T>(MemoryContainer<T> destination, MemoryContainer<T> source, UInt32 num) { memcpy(destination, source, num); return destination; }
        protected static Pointer<T> memmove<T>(Pointer<T> destination, Pointer<T> source, UInt32 num) { memcpy(destination, source, num); return destination; }
        protected static int strlen(string str) { return std.strlen(str); }
        protected static int strcmp(string str1, string str2) { return std.strcmp(str1, str2); }
        protected static int strncmp(string str1, string str2, int num) { return std.strncmp(str1, str2, num); }
    }


    public static class std
    {
        // c++ chrono
        public static class chrono
        {
            public static class system_clock
            {
                public static DateTimeOffset now() { return DateTimeOffset.Now; }
                public static DateTimeOffset from_time_t(std_time_t t) { return DateTimeOffset.FromUnixTimeSeconds(t); }
                public static std_time_t to_time_t(DateTimeOffset t) { return t.ToUnixTimeSeconds(); }
            }

            public static TimeSpan hours(int hours) { return new TimeSpan(hours, 0, 0); }
        }


        // c++ algorithm
        public static void fill<T>(MemoryContainer<T> destination, T value) { std.memset(destination, value); }
        public static void fill<T>(IList<T> destination, T value) { std.memset(destination, value); }
        public static void fill_n<T>(MemoryContainer<T> destination, int count, T value) { std.memset(destination, value, (UInt32)count); }
        public static void fill_n<T>(Pointer<T> destination, int count, T value) { std.memset(destination, value, (UInt32)count); }
        public static void fill_n(PointerU16 destination, int count, UInt16 value) { std.memset(destination, value, (UInt32)count); }
        public static void fill_n(PointerU32 destination, int count, UInt32 value) { std.memset(destination, value, (UInt32)count); }
        public static void fill_n(PointerU64 destination, int count, UInt64 value) { std.memset(destination, value, (UInt32)count); }
        public static void fill_n<T>(IList<T> destination, int count, T value) { std.memset(destination, value, (UInt32)count); }
        public static T find_if<T>(IEnumerable<T> list, Func<T, bool> pred) { foreach (var item in list) { if (pred(item)) return item; } return default;  }
        public static int lower_bound<C, I, V>(C collection, V value, Func<I, V, bool> func)
            where C : IList<I>
        {
            int first = 0;
            int count = collection.Count;

            if (count == 0)
                return -1;

            while (0 < count)
            {
                int count2 = count / 2;
                int mid = first + count2;
                if (func(collection[mid], value))
                {
                    first = mid + 1;
                    count -= count2 + 1;
                }
                else
                {
                    count = count2;
                }
            }

            return first;
        }
        public static int max(int a, int b) { return Math.Max(a, b); }
        public static UInt32 max(UInt32 a, UInt32 b) { return Math.Max(a, b); }
        public static Int64 max(Int64 a, Int64 b) { return Math.Max(a, b); }
        public static float max(float a, float b) { return Math.Max(a, b); }
        public static double max(double a, double b) { return Math.Max(a, b); }
        public static attotime max(attotime a, attotime b) { return attotime.Max(a, b); }
        public static netlist_time max(netlist_time a, netlist_time b) { return netlist_time.Max(a, b); }
        public static int min(int a, int b) { return Math.Min(a, b); }
        public static UInt32 min(UInt32 a, UInt32 b) { return Math.Min(a, b); }
        public static Int64 min(Int64 a, Int64 b) { return Math.Min(a, b); }
        public static float min(float a, float b) { return Math.Min(a, b); }
        public static double min(double a, double b) { return Math.Min(a, b); }
        public static attotime min(attotime a, attotime b) { return attotime.Min(a, b); }
        public static void sort<T>(MemoryContainer<T> list, Comparison<T> pred) { list.Sort(pred); }


        // c++ cmath
        public static int abs(int arg) { return Math.Abs(arg); }
        public static float abs(float arg) { return Math.Abs(arg); }
        public static double abs(double arg) { return Math.Abs(arg); }
        public static float cos(float arg) { return (float)Math.Cos(arg); }
        public static double cos(double arg) { return Math.Cos(arg); }
        public static double exp(double x) { return Math.Exp(x); }
        public static float fabs(float arg) { return Math.Abs(arg); }
        public static double fabs(double arg) { return Math.Abs(arg); }
        public static float fabsf(float arg) { return Math.Abs(arg); }
        public static float floor(float arg) { return (float)Math.Floor(arg); }
        public static double floor(double arg) { return Math.Floor(arg); }
        public static float floorf(float arg) { return (float)Math.Floor(arg); }
        public static float log(float arg) { return (float)Math.Log(arg); }
        public static double log(double arg) { return Math.Log(arg); }
        public static float pow(float base_, float exponent) { return (float)Math.Pow(base_, exponent); }
        public static double pow(double base_, double exponent) { return Math.Pow(base_, exponent); }
        public static int lround(double x) { return (int)Math.Round(x, MidpointRounding.AwayFromZero); }
        public static float sin(float arg) { return (float)Math.Sin(arg); }
        public static double sin(double arg) { return Math.Sin(arg); }
        public static float sqrt(float arg) { return (float)Math.Sqrt(arg); }
        public static double sqrt(double arg) { return Math.Sqrt(arg); }
        public static float trunc(float arg) { return (float)Math.Truncate(arg); }
        public static double trunc(double arg) { return Math.Truncate(arg); }


        // c++ cstdlib
        public static string getenv(string env_var) { return System.Environment.GetEnvironmentVariable(env_var); }
        public static UInt64 strtoul(string str, string endptr, int base_) { return Convert.ToUInt64(str, 16); }


        // c++ cstring
        public static void memcpy<T>(MemoryContainer<T> destination, MemoryContainer<T> source, UInt32 num) { source.CopyTo(0, destination, 0, (int)num); }  //void * destination, const void * source, size_t num );
        public static void memcpy<T>(Pointer<T> destination, Pointer<T> source, UInt32 num) { source.CopyTo(0, destination, 0, (int)num); }  //void * destination, const void * source, size_t num );
        public static void memset<T>(MemoryContainer<T> destination, T value) { destination.Fill(value); }
        public static void memset<T>(MemoryContainer<T> destination, T value, UInt32 num) { destination.Fill(value, (int)num); }
        public static void memset<T>(Pointer<T> destination, T value, UInt32 num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU16 destination, UInt16 value, UInt32 num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU32 destination, UInt32 value, UInt32 num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU64 destination, UInt64 value, UInt32 num) { destination.Fill(value, (int)num); }
        public static void memset<T>(IList<T> destination, T value) { memset(destination, value, (UInt32)destination.Count); }
        public static void memset<T>(IList<T> destination, T value, UInt32 num) { for (int i = 0; i < num; i++) destination[i] = value; }
        public static void memset<T>(T [,] destination, T value) { for (int i = 0; i < destination.GetLength(0); i++) for (int j = 0; j < destination.GetLength(1); j++) destination[i, j] = value; }
        public static int strchr(string str, char character) { return str.IndexOf(character); }
        public static int strcmp(string str1, string str2) { return string.Compare(str1, str2); }
        public static int strlen(string str) { return str.Length; }
        public static int strncmp(string str1, string str2, int num) { return string.Compare(str1, 0, str2, 0, num); }
        public static int strstr(string str1, string str2) { return str1.IndexOf(str2); }
        public static string to_string(int val) { return val.ToString(); }
        public static string to_string(UInt32 val) { return val.ToString(); }
        public static string to_string(Int64 val) { return val.ToString(); }
        public static string to_string(UInt64 val) { return val.ToString(); }
        public static string to_string(float val) { return val.ToString(); }
        public static string to_string(double val) { return val.ToString(); }


        // c++ exception
        public static void terminate() { throw new emu_fatalerror("std.terminate() called"); }


        // c++ iostream
        public static void cerr(string s) { osdcore_interface.osd_printf_debug(s); }


        // c++ utility
        public static void swap<T>(ref T val1, ref T val2)
        {
            global_object.assert(typeof(T).GetTypeInfo().IsValueType);

            T temp = val1;
            val1 = val2;
            val2 = temp;
        }

        public static std.pair<T, V> make_pair<T, V>(T t, V v) { return new std.pair<T, V>(t, v); }


        // c++ array
        public class array<T, size_t_N> : IList<T>
            where size_t_N : size_t_constant, new()
        {
            protected static size_t_constant N = new size_t_N();


            MemoryContainer<T> m_data = new MemoryContainer<T>((int)N.value, true);


            public array() { }
            public array(params T [] args)
            {
                if (args.Length != N.value)
                    throw new emu_fatalerror("array() parameter count doen't match size. Provided: {0}, Expected: {1}", args.Length, N.value);

                for (int i = 0; i < args.Length; i++)
                    m_data[i] = args[i];
            }


            // IList
            public int IndexOf(T value) { return m_data.IndexOf(value); }
            void IList<T>.Insert(int index, T value) { throw new emu_unimplemented(); }
            void IList<T>.RemoveAt(int index) { throw new emu_unimplemented(); }
            void ICollection<T>.Add(T value) { throw new emu_unimplemented(); }
            bool ICollection<T>.Contains(T value) { throw new emu_unimplemented(); }
            void ICollection<T>.Clear() { throw new emu_unimplemented(); }
            void ICollection<T>.CopyTo(T [] array, int index) { throw new emu_unimplemented(); }
            bool ICollection<T>.Remove(T value) { throw new emu_unimplemented(); }
            public int Count { get { return m_data.Count; } }
            bool ICollection<T>.IsReadOnly { get { throw new emu_unimplemented(); } }
            IEnumerator IEnumerable.GetEnumerator() { return m_data.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return ((IEnumerable<T>)m_data).GetEnumerator(); }


            public static bool operator ==(array<T, size_t_N> lhs, array<T, size_t_N> rhs)
            {
                // TODO available in .NET 3.5 and higher
                //return Enumerable.SequenceEquals(lhs, rhs);

                if (ReferenceEquals(lhs, rhs))
                    return true;

                if (lhs == null || rhs == null)
                    return false;

                if (lhs.size() != rhs.size())
                    return false;

                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                for (int i = 0; i < lhs.size(); i++)
                {
                    if (!comparer.Equals(lhs[i], rhs[i])) return false;
                }

                return true;
            }

            public static bool operator !=(array<T, size_t_N> lhs, array<T, size_t_N> rhs)
            {
                return !(lhs == rhs);
            }


            public override bool Equals(object obj)
            {
                return this == (array<T, size_t_N>)obj;
            }


            public override int GetHashCode()
            {
                return m_data.GetHashCode();
            }

            public T this[int index] { get { return m_data[index]; } set { m_data[index] = value; } }
            public T this[UInt32 index] { get { return m_data[index]; } set { m_data[index] = value; } }


            public int size() { return Count; }
            public void fill(T value) { std.fill(this, value); }
            public Pointer<T> data() { return new Pointer<T>(m_data); }
        }


        // c++ forward_list
        public class forward_list<T> : IEnumerable<T>
        {
            LinkedList<T> m_list = new LinkedList<T>();


            public forward_list() { }
            public forward_list(IEnumerable<T> collection) { m_list = new LinkedList<T>(collection); }


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_list.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return m_list.GetEnumerator(); }


            public void emplace_front(T item) { m_list.AddFirst(item); }
        }


        // c++ istream
        public class istream
        {
        }


        // c++ list
        public class list<T> : IEnumerable<T>
        {
            LinkedList<T> m_list = new LinkedList<T>();


            public list() { }
            public list(IEnumerable<T> collection) { m_list = new LinkedList<T>(collection); }


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_list.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return m_list.GetEnumerator(); }


            // std::list functions
            public void clear() { m_list.Clear(); }
            public LinkedListNode<T> emplace_back(T item) { return m_list.AddLast(item); }
            public bool empty() { return m_list.Count == 0; }
            public void push_back(T item) { m_list.AddLast(item); }
            public void push_front(T item) { m_list.AddFirst(item); }
            public int size() { return m_list.Count; }
        }


        // c++ map
        public class map<K, V> : IEnumerable<KeyValuePair<K, V>>
        {
            Dictionary<K, V> m_dictionary = new Dictionary<K, V>();


            public map() { }
            //public Dictionary(int capacity);
            //public Dictionary(IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary);
            //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
            //protected Dictionary(SerializationInfo info, StreamingContext context);


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_dictionary.GetEnumerator(); }
            IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() { return m_dictionary.GetEnumerator(); }


            public V this[K key] { get { return m_dictionary[key]; } set { m_dictionary[key] = value; } }


            public void Add(K key, V value) { m_dictionary.Add(key, value); }


            // std::map functions
            public void clear() { m_dictionary.Clear(); }
            public bool emplace(K key, V value) { if (m_dictionary.ContainsKey(key)) { return false; } else { m_dictionary.Add(key, value); return true; } }
            public void erase(K key) { m_dictionary.Remove(key); }
            public V find(K key) { V value; if (m_dictionary.TryGetValue(key, out value)) return value; else return default; }
            public int size() { return m_dictionary.Count; }
        }


        // c++ multimap
        public class multimap<K, V>
        {
            Dictionary<K, List<V>> m_dictionary = new Dictionary<K, List<V>>();


            public multimap() { }
            //public Dictionary(int capacity);
            //public Dictionary(IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary);
            //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
            //protected Dictionary(SerializationInfo info, StreamingContext context);
        }


        // c++ pair
        public class pair<T, V>
        {
            KeyValuePair<T, V> m_keyValue;


            public pair(T key, V value) { m_keyValue = new KeyValuePair<T, V>(key, value); }

            public override string ToString() { return m_keyValue.ToString(); }


            public T first { get { return m_keyValue.Key; } }
            public V second { get { return m_keyValue.Value; } }
        }


        // c++ set
        public class set<T> : IEnumerable<T>
        {
            HashSet<T> m_set = new HashSet<T>();


            public set() { }
            //public HashSet(IEqualityComparer<T> comparer);
            //public HashSet(IEnumerable<T> collection);
            //public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer);
            //protected HashSet(SerializationInfo info, StreamingContext context);


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_set.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return m_set.GetEnumerator(); }


            public bool ContainsIf(Func<T, bool> predicate)
            {
                foreach (var element in m_set)
                {
                    if (predicate(element))
                        return true;
                }
                return false;
            }


            // std::set functions
            public void clear() { m_set.Clear(); }
            public bool emplace(T item) { return m_set.Add(item); }
            public bool erase(T item) { return m_set.Remove(item); }
            public bool find(T item) { return m_set.Contains(item); }
            public bool insert(T item) { return m_set.Add(item); }
        }


        // c++ stack
        public class stack<T>
        {
            Stack<T> m_stack = new Stack<T>();


            // std::stack functions
            public bool empty() { return m_stack.Count == 0; }
            public T top() { return m_stack.Peek(); }
            public void push(T value) { m_stack.Push(value); }
            public void pop() { m_stack.Pop(); }
        }


        // c++ unordered_map
        public class unordered_map<K, V> : IEnumerable<KeyValuePair<K, V>>
        {
            Dictionary<K, V> m_dictionary = new Dictionary<K, V>();


            public unordered_map() { }
            //public Dictionary(int capacity);
            //public Dictionary(IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary);
            //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
            //protected Dictionary(SerializationInfo info, StreamingContext context);


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_dictionary.GetEnumerator(); }
            IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() { return m_dictionary.GetEnumerator(); }


            // this behavior matches std::unordered_map [] operator, except it doesn't new() the object, but uses default instead.
            // this can cause some unexpected issues.  see models_t.value_str() for example
            public V this[K key]
            {
                get
                {
                    V value;
                    if (m_dictionary.TryGetValue(key, out value))
                        return value;

                    value = default;
                    m_dictionary.Add(key, value);
                    return value;
                }
                set
                {
                    m_dictionary[key] = value;
                }
            }


            // std::unordered_map functions
            public V at(K key) { V value; if (m_dictionary.TryGetValue(key, out value)) return value; else return default; }
            public void clear() { m_dictionary.Clear(); }
            public bool emplace(K key, V value) { if (m_dictionary.ContainsKey(key)) { return false; } else { m_dictionary.Add(key, value); return true; } }
            public bool empty() { return m_dictionary.Count == 0; }
            public bool erase(K key) { return m_dictionary.Remove(key); }
            public V find(K key) { return at(key); }
            public bool insert(K key, V value) { if (m_dictionary.ContainsKey(key)) { return false; } else { m_dictionary.Add(key, value); return true; } }
            public bool insert(std.pair<K, V> keyvalue) { return insert(keyvalue.first, keyvalue.second); }
            public int size() { return m_dictionary.Count; }
        }


        // c++ unordered_set
        public class unordered_set<T>
        {
            HashSet<T> m_set = new HashSet<T>();


            public unordered_set() { }
            //public HashSet(IEqualityComparer<T> comparer);
            //public HashSet(IEnumerable<T> collection);
            //public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer);
            //protected HashSet(SerializationInfo info, StreamingContext context);


            // std::unordered_set functions
            public void clear() { m_set.Clear(); }
            public bool emplace(T item) { return m_set.Add(item); }
            public bool erase(T item) { return m_set.Remove(item); }
            public bool find(T item) { return m_set.Contains(item); }
            public bool insert(T item) { return m_set.Add(item); }
        }


        // c++ vector
        public class vector<T> : MemoryContainer<T>
        {
            public vector() : base() { }
            // this is different behavior as List<T> so that it matches how std::vector works
            public vector(int count, T data = default) : base(count) { resize(count, data); }
            public vector(u32 count, T data = default) : this((int)count, data) { }
            public vector(IEnumerable<T> collection) : base(collection) { }


            // std::vector functions
            public T back() { return empty() ? default : this[Count - 1]; }
            public void clear() { Clear(); }
            public Pointer<T> data() { return new Pointer<T>(this); }
            public void emplace(int index, T item) { Insert(index, item); }
            public void emplace_back(T item) { Add(item); }
            public bool empty() { return Count == 0; }
            public void erase(int index) { RemoveAt(index); }
            public void insert(int index, T item) { Insert(index, item); }
            public void pop_back() { if (Count > 0) { RemoveAt(Count - 1); } }
            public void push_back(T item) { Add(item); }
            public void push_front(T item) { Insert(0, item); }
            public void resize(int count, T data = default) { Resize(count, data); }
            public void reserve(int value) { Capacity = value; }
            public int size() { return Count; }
        }
    }


    // this wraps a Memory<T> struct so that bulk operations can be done efficiently
    // std.vector and MemoryU8 derive from this.
    public class MemoryContainer<T> : IList<T>
    {
        class MemoryContainerEnumerator<T> : IEnumerator<T>
        {
            MemoryContainer<T> m_list;
            int m_index;
            int m_endIndex;
 
            public MemoryContainerEnumerator(MemoryContainer<T> list)
            {
                m_list = list;
                m_index = -1;
                m_endIndex = list.Count;
            }
 
            public void Dispose() { }
            public bool MoveNext() { if (m_index < m_endIndex) { m_index++; return m_index < m_endIndex; } return false; }
            object IEnumerator.Current { get { return null; } }
            public T Current
            {
                get
                {
                    if (m_index < 0) throw new InvalidOperationException(string.Format("ListBaseEnumerator() - {0}", m_index));
                    if (m_index >= m_endIndex) throw new InvalidOperationException(string.Format("ListBaseEnumerator() - {0}", m_index));
                    return m_list[m_index];
                }
            }
            public void Reset() { m_index = -1; }
        }


        T [] m_data;
        Memory<T> m_memory;
        int m_actualLength = 0;


        public MemoryContainer() : this(0) { }
        public MemoryContainer(int capacity, bool allocate = false) { m_data = new T [capacity]; m_memory = new Memory<T>(m_data); if (allocate) Resize(capacity); }
        public MemoryContainer(IEnumerable<T> collection) : this() { foreach (var item in collection) Add(item); }


        // IList

        public virtual T this[int index] { get { return m_data[index]; } set { m_data[index] = value; } }

        public virtual int IndexOf(T item) { return Array.IndexOf(m_data, item, 0, Count); }
        public virtual void Insert(int index, T item)
        {
            var newData = m_data;
            var newMemory = m_memory;
            if (Count + 1 > Capacity)
            {
                int newSize = Math.Min(Count + 1024, (Count + 1) * 2);  // cap the growth
                newData = new T [newSize];
                newMemory = new Memory<T>(newData);
                if (Capacity > 0)
                    m_memory.Slice(0, Math.Min(index + 1, Count)).CopyTo(newMemory.Slice(0, Math.Min(index + 1, Count)));  // copy everything before index
            }
            if (Capacity > 0 && index < Count)
                m_memory.Slice(index, Count - index).CopyTo(newMemory.Slice(index + 1));  // copy everything after index
            newData[index] = item;
            m_data = newData;
            m_memory = newMemory;
            m_actualLength++;
        }
        public virtual void RemoveAt(int index) { RemoveRange(index, 1); }
        public virtual void Add(T item)
        {
            if (Count + 1 > Capacity)
            {
                int newSize = Math.Min(Count + 1024, (Count + 1) * 2);  // cap the growth
                var newData = new T [newSize];
                var newMemory = new Memory<T>(newData);
                m_memory.CopyTo(newMemory);
                m_data = newData;
                m_memory = newMemory;
            }

            m_data[m_actualLength] = item;
            m_actualLength++;
        }
        public virtual void Clear()
        {
            m_data = new T [0];
            m_memory = new Memory<T>(m_data);
            m_actualLength = 0;
        }
        public virtual bool Contains(T item) { return IndexOf(item) != -1; }
        public virtual void CopyTo(T[] array, int arrayIndex) { m_data.CopyTo(array, arrayIndex); }
        public virtual bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index == -1)
                return false;

            RemoveAt(index);
            return true;
        }
        public virtual int Count { get { return m_actualLength; } }
        bool ICollection<T>.IsReadOnly { get { return ((ICollection<T>)m_data).IsReadOnly; } }
        public virtual IEnumerator<T> GetEnumerator() { return new MemoryContainerEnumerator<T>(this); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }


        // List

        public virtual int Capacity { get { return m_data.Length; } set { } }
        public virtual void CopyTo(int index, T[] array, int arrayIndex, int count) { CopyTo(index, new Span<T>(array, arrayIndex, count), count); }
        public virtual int RemoveAll(Predicate<T> match)
        {
            int count = 0;
            int index = Array.FindIndex(m_data, 0, Count, match);
            while (index != -1)
            {
                RemoveAt(index);
                count++;
                index = Array.FindIndex(m_data, 0, Count, match);
            }
            return count;
        }
        public virtual void RemoveRange(int index, int count)
        {
            if (index + count < Count)
                m_memory.Slice(index + count).CopyTo(m_memory.Slice(index));

            m_actualLength -= count;
        }
        public virtual void Sort(Comparison<T> comparison) { Array.Sort(m_data, 0, Count, Comparer<T>.Create(comparison)); }
        public virtual void Sort() { Array.Sort(m_data, 0, Count); }
        public virtual void Sort(IComparer<T> comparer) { Array.Sort(m_data, 0, Count, comparer); }
        public virtual T[] ToArray() { T [] newData = new T [Count]; Array.Copy(m_data, newData, Count); return newData; }


        public Memory<T> memory { get { return m_memory; } }

        // UInt32 helper
        public virtual T this[u32 index] { get { return this[(int)index]; } set { this[(int)index] = value; } }


        public bool MemoryEquals(MemoryContainer<T> other) { return m_data == other.m_data; }


        public virtual void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                Add(item);
        }


        public virtual bool CompareTo(MemoryContainer<T> right, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (!this[i].Equals(right[i]))
                    return false;
            }
            return true;
        }


        public virtual void CopyTo(int srcStart, MemoryContainer<T> dest, int destStart, int count)
        {
            CopyTo(srcStart, dest.m_memory.Slice(destStart, count).Span, count);
        }


        public virtual void CopyTo(int srcStart, Span<T> dest, int count)
        {
            m_memory.Slice(srcStart, count).Span.CopyTo(dest);
        }


        public virtual void Resize(int count) { ResizeInternal(count, default); }
        public virtual void Resize(int count, T data)
        {
            global_object.assert(typeof(T).GetTypeInfo().IsValueType ? true : (data == null || data.Equals(default)) ? true : false);  // this function doesn't do what you'd expect for ref classes since it doesn't new() for each item.  Manually Add() in this case.
            ResizeInternal(count, data);
        }


        protected virtual void ResizeInternal(int count, T data)
        {
            int current = Count;
            if (count < current)
            {
                RemoveRange(count, current - count);
            }
            else if (count > current)
            {
                if (count > Capacity)
                    Capacity = count;

                for (int i = 0; i < count - current; i++)
                    Add(data);
            }
        }


        public virtual void Fill(T value) { Fill(value, 0, Count); }
        public virtual void Fill(T value, int count) { Fill(value, 0, count); }
        public virtual void Fill(T value, int start, int count)
        {
            var valueType = typeof(T).GetTypeInfo().IsValueType ? true : ((value == null || value.Equals(default)) ? true : false);
            if (valueType)
            {
                m_memory.Slice(start, count).Span.Fill(value);
            }
            else
            {
                for (int i = start; i < start + count; i++)
                    this[i] = value;
            }
        }
        public virtual void Fill(Func<T> creator)
        {
            for (int i = 0; i < Count; i++)
                this[i] = creator();
        }
    }


    // this attempts to act as a C++ pointer.  It has a MemoryContainer and an offset into that container.
    public class Pointer<T>
    {
        protected MemoryContainer<T> m_memory;
        protected int m_offset;


        public Pointer() { }
        public Pointer(MemoryContainer<T> memory, int offset = 0) { m_memory = memory; m_offset = offset; }
        public Pointer(Pointer<T> pointer, int offset = 0) : this(pointer.m_memory, pointer.m_offset + offset) { }


        public virtual MemoryContainer<T> Buffer { get { return m_memory; } }
        public virtual int Offset { get { return m_offset; } }
        public virtual int Count { get { return m_memory.Count; } }


        public virtual T this[int i] { get { return m_memory[m_offset + i]; } set { m_memory[m_offset + i] = value; } }
        public virtual T this[UInt32 i] { get { return m_memory[m_offset + (int)i]; } set { m_memory[m_offset + (int)i] = value; } }


        public static Pointer<T> operator +(Pointer<T> left, int right) { return new Pointer<T>(left, right); }
        public static Pointer<T> operator +(Pointer<T> left, UInt32 right) { return new Pointer<T>(left, (int)right); }
        public static Pointer<T> operator +(Pointer<T> left, Pointer<T> right) { if (!left.Buffer.MemoryEquals(right.Buffer)) return null; return new Pointer<T>(left.Buffer, left.Offset + right.Offset); }
        public static Pointer<T> operator ++(Pointer<T> left) { left.m_offset++; return left; }
        public static Pointer<T> operator -(Pointer<T> left, int right) { return new Pointer<T>(left, -right); }
        public static Pointer<T> operator -(Pointer<T> left, UInt32 right) { return new Pointer<T>(left, -(int)right); }
        public static Pointer<T> operator -(Pointer<T> left, Pointer<T> right) { if (!left.Buffer.MemoryEquals(right.Buffer)) return null; return new Pointer<T>(left.Buffer, left.Offset - right.Offset); }
        public static Pointer<T> operator --(Pointer<T> left) { left.m_offset--; return left; }


        public virtual T op { get { return this[0]; } set { this[0] = value; } }


        public virtual bool CompareTo(Pointer<T> right, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (!this[i].Equals(right[i]))
                    return false;
            }
            return true;
        }

        public virtual void CopyTo(int srcStart, Pointer<T> dest, int destStart, int count) { m_memory.CopyTo(m_offset + srcStart, dest.Buffer, dest.m_offset + destStart, count); }
        public virtual void CopyTo(int srcStart, Span<T> span, int count) { m_memory.CopyTo(srcStart, span, count); }

        public virtual void Fill(T value, int count) { Fill(value, 0, count); }
        public virtual void Fill(T value, int start, int count) { m_memory.Fill(value, m_offset + start, count); }
    }


    public class PointerRef<T>
    {
        public Pointer<T> m_pointer;

        public PointerRef() { }
        public PointerRef(Pointer<T> pointer) { m_pointer = pointer; }
    }


    public static class PointerU8Extension
    {
        //public static PointerU8 operator +(PointerU8 left, int right) { return new PointerU8(left, right); }
        //public static PointerU8 operator +(PointerU8 left, UInt32 right) { return new PointerU8(left, (int)right); }
        //public static PointerU8 operator ++(PointerU8 left) { left.m_offset++; return left; }
        //public static PointerU8 operator -(PointerU8 left, int right) { return new PointerU8(left, -right); }
        //public static PointerU8 operator -(PointerU8 left, UInt32 right) { return new PointerU8(left, -(int)right); }
        //public static PointerU8 operator --(PointerU8 left) { left.m_offset--; return left; }

        // offset parameter is based on unit size for each function, not based on byte size.  eg, SetUInt16 parameter offset16 is in uint16 units
        public static void SetUInt16(this Pointer<byte> pointer, int offset16, UInt16 value) { pointer.Buffer.SetUInt16Offs8(pointer.Offset + (offset16 << 1), value); }
        public static void SetUInt32(this Pointer<byte> pointer, int offset32, UInt32 value) { pointer.Buffer.SetUInt32Offs8(pointer.Offset + (offset32 << 2), value); }
        public static void SetUInt64(this Pointer<byte> pointer, int offset64, UInt64 value) { pointer.Buffer.SetUInt64Offs8(pointer.Offset + (offset64 << 3), value); }

        // offset parameter is based on unit size for each function, not based on byte size.  eg, GetUInt16 parameter offset16 is in uint16 units
        public static UInt16 GetUInt16(this Pointer<byte> pointer, int offset16) { return pointer.Buffer.GetUInt16Offs8(pointer.Offset + (offset16 << 1)); }
        public static UInt32 GetUInt32(this Pointer<byte> pointer, int offset32) { return pointer.Buffer.GetUInt32Offs8(pointer.Offset + (offset32 << 2)); }
        public static UInt64 GetUInt64(this Pointer<byte> pointer, int offset64) { return pointer.Buffer.GetUInt64Offs8(pointer.Offset + (offset64 << 3)); }

        public static bool CompareTo(this Pointer<byte> pointer, string compareTo) { return pointer.CompareTo(0, compareTo); }
        public static bool CompareTo(this Pointer<byte> pointer, int startOffset, string compareTo) { return pointer.CompareTo(startOffset, compareTo.ToCharArray()); }
        public static bool CompareTo(this Pointer<byte> pointer, int startOffset, char [] compareTo) { return pointer.Buffer.CompareTo(startOffset, compareTo); }

        public static string ToString(this Pointer<byte> pointer, int length) { return pointer.ToString(length); }
        public static string ToString(this Pointer<byte> pointer, int startOffset, int length) { return pointer.Buffer.ToString(startOffset, length); }
    }


    // use these derived pointer classes when you need to do operations on a Pointer with a different base type
    // eg, note the [] overloads use the type it designates.
    // note the get/set functions for operating on the data as if it's a different type.
    // see PointerU16, PointerU32, PointerU64
    public class PointerU16 : Pointer<byte>
    {
        public PointerU16() : base() { }
        public PointerU16(MemoryContainer<byte> list, int offset16 = 0) : base(list, offset16 << 1) { }
        public PointerU16(Pointer<byte> listPtr, int offset16 = 0) : base(listPtr, offset16 << 1) { }

        public new UInt16 this[int i] { get { return this.GetUInt16(i); } set { this.SetUInt16(i, value); } }
        public new UInt16 this[UInt32 i] { get { return this.GetUInt16((int)i); } set { this.SetUInt16((int)i, value); } }

        public static PointerU16 operator +(PointerU16 left, int right) { return new PointerU16(left, right); }
        public static PointerU16 operator +(PointerU16 left, UInt32 right) { return new PointerU16(left, (int)right); }
        public static PointerU16 operator ++(PointerU16 left) { left.m_offset += 2; return left; }
        public static PointerU16 operator -(PointerU16 left, int right) { return new PointerU16(left, -right); }
        public static PointerU16 operator -(PointerU16 left, UInt32 right) { return new PointerU16(left, -(int)right); }
        public static PointerU16 operator --(PointerU16 left) { left.m_offset -= 2; return left; }

        public void Fill(UInt16 value, int count) { Fill(value, 0, count); }
        public void Fill(UInt16 value, int start, int count) { for (int i = start; i < start + count; i++) this[i] = value; }
    }

    public class PointerU32 : Pointer<byte>
    {
        public PointerU32() : base() { }
        public PointerU32(MemoryContainer<byte> list, int offset32 = 0) : base(list, offset32 << 2) { }
        public PointerU32(Pointer<byte> listPtr, int offset32 = 0) : base(listPtr, offset32 << 2) { }

        public new UInt32 this[int i] { get { return this.GetUInt32(i); } set { this.SetUInt32(i, value); } }
        public new UInt32 this[UInt32 i] { get { return this.GetUInt32((int)i); } set { this.SetUInt32((int)i, value); } }

        public static PointerU32 operator +(PointerU32 left, int right) { return new PointerU32(left, right); }
        public static PointerU32 operator +(PointerU32 left, UInt32 right) { return new PointerU32(left, (int)right); }
        public static PointerU32 operator ++(PointerU32 left) { left.m_offset += 4; return left; }
        public static PointerU32 operator -(PointerU32 left, int right) { return new PointerU32(left, -right); }
        public static PointerU32 operator -(PointerU32 left, UInt32 right) { return new PointerU32(left, -(int)right); }
        public static PointerU32 operator --(PointerU32 left) { left.m_offset -= 4; return left; }

        public void Fill(UInt32 value, int count) { Fill(value, 0, count); }
        public void Fill(UInt32 value, int start, int count) { for (int i = start; i < start + count; i++) this[i] = value; }
    }

    public class PointerU64 : Pointer<byte>
    {
        public PointerU64() : base() { }
        public PointerU64(MemoryContainer<byte> list, int offset64 = 0) : base(list, offset64 << 3) { }
        public PointerU64(Pointer<byte> listPtr, int offset64 = 0) : base(listPtr, offset64 << 3) { }

        public new UInt64 this[int i] { get { return this.GetUInt64(i); } set { this.SetUInt64(i, value); } }
        public new UInt64 this[UInt32 i] { get { return this.GetUInt64((int)i); } set { this.SetUInt64((int)i, value); } }

        public static PointerU64 operator +(PointerU64 left, int right) { return new PointerU64(left, right); }
        public static PointerU64 operator +(PointerU64 left, UInt32 right) { return new PointerU64(left, (int)right); }
        public static PointerU64 operator ++(PointerU64 left) { left.m_offset += 8; return left; }
        public static PointerU64 operator -(PointerU64 left, int right) { return new PointerU64(left, -right); }
        public static PointerU64 operator -(PointerU64 left, UInt32 right) { return new PointerU64(left, -(int)right); }
        public static PointerU64 operator --(PointerU64 left) { left.m_offset -= 8; return left; }

        public void Fill(UInt64 value, int count) { Fill(value, 0, count); }
        public void Fill(UInt64 value, int start, int count) { for (int i = start; i < start + count; i++) this[i] = value; }
    }


    // the usage of this should be when you have a flat piece of memory and intend to work on it with different types
    // eg, a byte array that you cast to UInt32 and do operations.
    // See PointerU8, PointerU16, etc
    public static class MemoryU8Extension
    {
        public static bool CompareTo(this MemoryContainer<byte> container, int startOffset, string compareTo) { return container.CompareTo(startOffset, compareTo.ToCharArray()); }
        public static bool CompareTo(this MemoryContainer<byte> container, int startOffset, char [] compareTo)
        {
            for (int i = 0; i < compareTo.Length; i++)
            {
                if (container[i] != compareTo[i])
                    return false;
            }
            return true;
        }

        public static string ToString(this MemoryContainer<byte> container, int startOffset, int length)
        {
            string s = "";
            for (int i = startOffset; i < startOffset + length; i++)
            {
                s += container[i];
            }

            return s;
        }


        public static int IndexOf(this MemoryContainer<byte> container, int startOffset, int endOffset, byte compare)
        {
            for (int i = startOffset; i < startOffset + endOffset; i++)
            {
                if (container[i] == compare)
                    return i;
            }

            return endOffset;
        }


        public static void SetUInt16(this MemoryContainer<byte> container, int offset16, UInt16 value) { container.SetUInt16Offs8(offset16 << 1, value); }
        public static void SetUInt32(this MemoryContainer<byte> container, int offset32, UInt32 value) { container.SetUInt32Offs8(offset32 << 2, value); }
        public static void SetUInt64(this MemoryContainer<byte> container, int offset64, UInt64 value) { container.SetUInt64Offs8(offset64 << 3, value); }

        public static void SetUInt16Offs8(this MemoryContainer<byte> container, int offset8, UInt16 value)
        {
            global_object.assert_slow(offset8 < container.Count);
            var span = container.memory.Slice(offset8, 2).Span;
            BinaryPrimitives.WriteUInt16LittleEndian(span, value);
        }

        public static void SetUInt32Offs8(this MemoryContainer<byte> container, int offset8, UInt32 value)
        {
            global_object.assert_slow(offset8 < container.Count);
            var span = container.memory.Slice(offset8, 4).Span;
            BinaryPrimitives.WriteUInt32LittleEndian(span, value);
        }

        public static void SetUInt64Offs8(this MemoryContainer<byte> container, int offset8, UInt64 value)
        {
            global_object.assert_slow(offset8 < container.Count);
            var span = container.memory.Slice(offset8, 8).Span;
            BinaryPrimitives.WriteUInt64LittleEndian(span, value);
        }

        public static UInt16 GetUInt16(this MemoryContainer<byte> container, int offset16 = 0) { return container.GetUInt16Offs8(offset16 << 1); }
        public static UInt32 GetUInt32(this MemoryContainer<byte> container, int offset32 = 0) { return container.GetUInt32Offs8(offset32 << 2); }
        public static UInt64 GetUInt64(this MemoryContainer<byte> container, int offset64 = 0) { return container.GetUInt64Offs8(offset64 << 3); }

        public static UInt16 GetUInt16Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            global_object.assert_slow(offset8 < container.Count);
            var span = container.memory.Slice(offset8, 2).Span;
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
        }

        public static UInt32 GetUInt32Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            global_object.assert_slow(offset8 < container.Count);
            var span = container.memory.Slice(offset8, 4).Span;
            return BinaryPrimitives.ReadUInt32LittleEndian(span);
        }

        public static UInt64 GetUInt64Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            global_object.assert_slow(offset8 < container.Count);
            var span = container.memory.Slice(offset8, 8).Span;
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
        }
    }


    // manual boxing class of an int
    public class intref
    {
        int m_value;

        public intref() { }
        public intref(int i) { m_value = i; }

        public int i { get { return m_value; } set { m_value = value; } }

        // these might cause trouble, by replacing the ref, which is what we want to avoid
        //public static implicit operator int(intref x) { return x.get(); }
        //public static implicit operator intref(int x) { return new intref(x); }
    }


    // manual boxing class of a double
    public class doubleref
    {
        double m_value;

        public doubleref() { }
        public doubleref(double value) { m_value = value; }

        public double d { get { return m_value; } set { m_value = value; } }

        // these might cause trouble, by replacing the ref, which is what we want to avoid
        //public static implicit operator int(intref x) { return x.get(); }
        //public static implicit operator intref(int x) { return new intref(x); }
    }
}
