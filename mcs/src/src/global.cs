// license:BSD-3-Clause
// copyright-holders:Edward Fast

//#define ASSERT_SLOW

using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using char32_t = System.UInt32;
using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;
using ioport_value = System.UInt32;  //typedef u32 ioport_value;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using PointerU8 = mame.Pointer<System.Byte>;
using s8 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using size_t = System.UInt64;
using std_time_t = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;
using unsigned = System.UInt32;
using uX = mame.FlexPrim;


namespace mame
{
    public interface bool_const { bool value { get; } }
    public class bool_const_true : bool_const { public bool value { get { return true; } } }
    public class bool_const_false : bool_const { public bool value { get { return false; } } }

    public interface int_const { int value { get; } }
    public class int_const_n512 : int_const { public int value { get { return -512; } } }
    public class int_const_n256 : int_const { public int value { get { return -256; } } }
    public class int_const_n128 : int_const { public int value { get { return -128; } } }
    public class int_const_n64 : int_const { public int value { get { return -64; } } }
    public class int_const_n32 : int_const { public int value { get { return -32; } } }
    public class int_const_n16 : int_const { public int value { get { return -16; } } }
    public class int_const_n3 : int_const { public int value { get { return -3; } } }
    public class int_const_n2 : int_const { public int value { get { return -2; } } }
    public class int_const_n1 : int_const { public int value { get { return -1; } } }
    public class int_const_0 : int_const { public int value { get { return 0; } } }
    public class int_const_1 : int_const { public int value { get { return 1; } } }
    public class int_const_2 : int_const { public int value { get { return 2; } } }
    public class int_const_3 : int_const { public int value { get { return 3; } } }
    public class int_const_4 : int_const { public int value { get { return 4; } } }
    public class int_const_5 : int_const { public int value { get { return 5; } } }
    public class int_const_6 : int_const { public int value { get { return 6; } } }
    public class int_const_7 : int_const { public int value { get { return 7; } } }
    public class int_const_8 : int_const { public int value { get { return 8; } } }
    public class int_const_9 : int_const { public int value { get { return 9; } } }
    public class int_const_10 : int_const { public int value { get { return 10; } } }
    public class int_const_11 : int_const { public int value { get { return 11; } } }
    public class int_const_12 : int_const { public int value { get { return 12; } } }
    public class int_const_13 : int_const { public int value { get { return 13; } } }
    public class int_const_14 : int_const { public int value { get { return 14; } } }
    public class int_const_15 : int_const { public int value { get { return 15; } } }
    public class int_const_16 : int_const { public int value { get { return 16; } } }
    public class int_const_17 : int_const { public int value { get { return 17; } } }
    public class int_const_18 : int_const { public int value { get { return 18; } } }
    public class int_const_19 : int_const { public int value { get { return 19; } } }
    public class int_const_20 : int_const { public int value { get { return 20; } } }
    public class int_const_21 : int_const { public int value { get { return 21; } } }
    public class int_const_22 : int_const { public int value { get { return 22; } } }
    public class int_const_23 : int_const { public int value { get { return 23; } } }
    public class int_const_24 : int_const { public int value { get { return 24; } } }
    public class int_const_25 : int_const { public int value { get { return 25; } } }
    public class int_const_26 : int_const { public int value { get { return 26; } } }
    public class int_const_27 : int_const { public int value { get { return 27; } } }
    public class int_const_28 : int_const { public int value { get { return 28; } } }
    public class int_const_29 : int_const { public int value { get { return 29; } } }
    public class int_const_30 : int_const { public int value { get { return 30; } } }
    public class int_const_31 : int_const { public int value { get { return 31; } } }
    public class int_const_32 : int_const { public int value { get { return 32; } } }

    public interface u32_const { UInt32 value { get; } }
    public class u32_const_0 : u32_const { public UInt32 value { get { return 0; } } }
    public class u32_const_1 : u32_const { public UInt32 value { get { return 1; } } }
    public class u32_const_2 : u32_const { public UInt32 value { get { return 2; } } }
    public class u32_const_3 : u32_const { public UInt32 value { get { return 3; } } }
    public class u32_const_4 : u32_const { public UInt32 value { get { return 4; } } }
    public class u32_const_5 : u32_const { public UInt32 value { get { return 5; } } }
    public class u32_const_6 : u32_const { public UInt32 value { get { return 6; } } }
    public class u32_const_7 : u32_const { public UInt32 value { get { return 7; } } }
    public class u32_const_8 : u32_const { public UInt32 value { get { return 8; } } }
    public class u32_const_10 : u32_const { public UInt32 value { get { return 10; } } }
    public class u32_const_12 : u32_const { public UInt32 value { get { return 12; } } }
    public class u32_const_16 : u32_const { public UInt32 value { get { return 16; } } }
    public class u32_const_20 : u32_const { public UInt32 value { get { return 20; } } }

    public interface u64_const { UInt64 value { get; } }
    public class u64_const_0 : u64_const { public UInt64 value { get { return 0; } } }
    public class u64_const_2 : u64_const { public UInt64 value { get { return 2; } } }
    public class u64_const_3 : u64_const { public UInt64 value { get { return 3; } } }
    public class u64_const_4 : u64_const { public UInt64 value { get { return 4; } } }
    public class u64_const_5 : u64_const { public UInt64 value { get { return 5; } } }
    public class u64_const_8 : u64_const { public UInt64 value { get { return 8; } } }
    public class u64_const_16 : u64_const { public UInt64 value { get { return 16; } } }

    public interface endianness_t_const { endianness_t value { get; } }
    public class endianness_t_const_ENDIANNESS_LITTLE : endianness_t_const { public endianness_t value { get { return endianness_t.ENDIANNESS_LITTLE; } } }
    public class endianness_t_const_ENDIANNESS_BIG : endianness_t_const { public endianness_t value { get { return endianness_t.ENDIANNESS_BIG; } } }


    // global functions
    public static class g
    {
        // _74259
        public static ls259_device LS259(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<ls259_device>(mconfig, tag, ls259_device.LS259, clock); }
        public static ls259_device LS259<bool_Required>(machine_config mconfig, device_finder<ls259_device, bool_Required> finder, u32 clock = 0) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, ls259_device.LS259, clock); }


        // adc0808
        public static adc0809_device ADC0809(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<adc0809_device>(mconfig, tag, adc0809_device.ADC0809, clock); }


        // atarimo
        public static atari_motion_objects_device ATARI_MOTION_OBJECTS<bool_Required>(machine_config mconfig, device_finder<atari_motion_objects_device, bool_Required> finder, uint32_t clock, device_finder<screen_device, bool_Required> screen_tag, atari_motion_objects_config config)
            where bool_Required : bool_const, new()
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, atari_motion_objects_device.ATARI_MOTION_OBJECTS, 0);
            device.atari_motion_objects_device_after_ctor(screen_tag, config);
            return device;
        }


        // attotime
        public static attoseconds_t ATTOSECONDS_IN_USEC(u32 x) { return attotime.ATTOSECONDS_IN_USEC(x); }


        // ay8910
        public static ay8910_device AY8910(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<ay8910_device>(mconfig, tag, ay8910_device.AY8910, clock); }
        public static ay8910_device AY8910(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<ay8910_device>(mconfig, tag, ay8910_device.AY8910, clock); }
        public static ay8910_device AY8910<bool_Required>(machine_config mconfig, device_finder<ay8910_device, bool_Required> finder, u32 clock = 0) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, ay8910_device.AY8910, clock); }
        public static ay8910_device AY8910<bool_Required>(machine_config mconfig, device_finder<ay8910_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, ay8910_device.AY8910, clock); }


        // bankdev
        public static address_map_bank_device ADDRESS_MAP_BANK(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<address_map_bank_device>(mconfig, tag, address_map_bank_device.ADDRESS_MAP_BANK, 0); }


        // corefile
        public static string core_filename_extract_base(string name, bool strip_extension = false) { return util.core_filename_extract_base(name, strip_extension); }
        public static bool core_filename_ends_with(string filename, string extension) { return util.core_filename_ends_with(filename, extension); }


        // corestr
        public static int core_stricmp(string s1, string s2) { return corestr_global.core_stricmp(s1, s2); }
        public static int core_strnicmp(string s1, string s2, size_t n) { return corestr_global.core_strnicmp(s1, s2, n); }
        public static int core_strwildcmp(string sp1, string sp2) { return corestr_global.core_strwildcmp(sp1, sp2); }
        public static bool core_iswildstr(string sp) { return corestr_global.core_iswildstr(sp); }
        public static string strtrimspace(string str) { return corestr_global.strtrimspace(str); }
        public static int strreplace(ref string str, string search, string replace) { return corestr_global.strreplace(ref str, search, replace); }


        // coretmpl
        public static u8 make_bitmask8(u32 n) { return util.make_bitmask8(n); }
        public static u8 make_bitmask8(s32 n) { return util.make_bitmask8(n); }
        public static u16 make_bitmask16(s32 n) { return util.make_bitmask16(n); }
        public static u16 make_bitmask16(u32 n) { return util.make_bitmask16(n); }
        public static u32 make_bitmask32(s32 n) { return util.make_bitmask32(n); }
        public static u32 make_bitmask32(u32 n) { return util.make_bitmask32(n); }
        public static u64 make_bitmask64(s32 n) { return util.make_bitmask64(n); }
        public static u64 make_bitmask64(u32 n) { return util.make_bitmask64(n); }
        public static uX make_bitmask_uX(int width, u32 n) { return util.make_bitmask_uX(width, n); }
        public static uX make_bitmask_uX(int width, s32 n) { return util.make_bitmask_uX(width, n); }
        public static int BIT(int x, int n) { return util.BIT(x, n); }
        public static UInt32 BIT(UInt32 x, int n) { return util.BIT(x, n); }
        public static UInt32 BIT(UInt32 x, UInt32 n, UInt32 w) { return util.BIT(x, n, w); }
        public static UInt32 BIT(UInt32 x, int n, UInt32 w) { return util.BIT(x, n, w); }
        public static UInt32 BIT(UInt32 x, int n, int w) { return util.BIT(x, n, w); }
        public static int bitswap(int val, int B1, int B0) { return util.bitswap(val, B1, B0); }
        public static int bitswap(int val, int B3, int B2, int B1, int B0) { return util.bitswap(val, B3, B2, B1, B0); }
        public static int bitswap(int val, int B5, int B4, int B3, int B2, int B1, int B0) { return util.bitswap(val, B5, B4, B3, B2, B1, B0); }
        public static int bitswap(int val, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0) { return util.bitswap(val, B7, B6, B5, B4, B3, B2, B1, B0); }
        public static int bitswap(int val, int B10, int B9, int B8, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0) { return util.bitswap(val, B10, B9, B8, B7, B6, B5, B4, B3, B2, B1, B0); }
        public static int bitswap(int val, int B11, int B10, int B9, int B8, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0) { return util.bitswap(val, B11, B10, B9, B8, B7, B6, B5, B4, B3, B2, B1, B0); }
        public static int bitswap(int val, int B15, int B14, int B13, int B12, int B11, int B10, int B9, int B8, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0) { return util.bitswap(val, B15, B14, B13, B12, B11, B10, B9, B8, B7, B6, B5, B4, B3, B2, B1, B0); }
        public static sbyte iabs(sbyte v) { return util.iabs(v); }
        public static short iabs(short v) { return util.iabs(v); }
        public static int iabs(int v) { return util.iabs(v); }
        public static Int64 iabs(Int64 v) { return util.iabs(v); }
        public static void reduce_fraction(ref UInt32 num, ref UInt32 den) { util.reduce_fraction(ref num, ref den); }


        // crsshair
        public const int CROSSHAIR_VISIBILITY_OFF = crsshair_global.CROSSHAIR_VISIBILITY_OFF;
        public const int CROSSHAIR_VISIBILITY_DEFAULT = crsshair_global.CROSSHAIR_VISIBILITY_DEFAULT;
        public const int CROSSHAIR_VISIBILITY_AUTOTIME_DEFAULT = crsshair_global.CROSSHAIR_VISIBILITY_AUTOTIME_DEFAULT;
        public const int CROSSHAIR_RAW_SIZE = crsshair_global.CROSSHAIR_RAW_SIZE;
        public const int CROSSHAIR_RAW_ROWBYTES = crsshair_global.CROSSHAIR_RAW_ROWBYTES;


        // dac
        public static dac_8bit_r2r_device DAC_8BIT_R2R<bool_Required>(machine_config mconfig, device_finder<dac_8bit_r2r_device, bool_Required> finder, u32 clock = 0) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, dac_8bit_r2r_device.DAC_8BIT_R2R, clock); }


        // device
        public const string DEVICE_SELF = device_global.DEVICE_SELF;
        public const string DEVICE_SELF_OWNER = device_global.DEVICE_SELF_OWNER;
        public static u32 DERIVED_CLOCK(u32 num, u32 den) { return device_global.DERIVED_CLOCK(num, den); }
        public static device_type DEFINE_DEVICE_TYPE(device_type.create_func func, string shortname, string fullname) { return device_global.DEFINE_DEVICE_TYPE(func, shortname, fullname); }


        // diexec
        public const int CLEAR_LINE = diexec_global.CLEAR_LINE;
        public const int ASSERT_LINE = diexec_global.ASSERT_LINE;
        public const int HOLD_LINE = diexec_global.HOLD_LINE;
        public const int MAX_INPUT_LINES = diexec_global.MAX_INPUT_LINES;
        public const int INPUT_LINE_IRQ0 = diexec_global.INPUT_LINE_IRQ0;
        public const int INPUT_LINE_IRQ1 = diexec_global.INPUT_LINE_IRQ1;
        public const int INPUT_LINE_NMI = diexec_global.INPUT_LINE_NMI;
        public const int INPUT_LINE_RESET = diexec_global.INPUT_LINE_RESET;
        public const int INPUT_LINE_HALT = diexec_global.INPUT_LINE_HALT;
        public const u32 SUSPEND_REASON_HALT = device_execute_interface.SUSPEND_REASON_HALT;
        public const u32 SUSPEND_REASON_RESET = device_execute_interface.SUSPEND_REASON_RESET;
        public const u32 SUSPEND_REASON_DISABLE = device_execute_interface.SUSPEND_REASON_DISABLE;


        // digfx
        public const int MAX_GFX_ELEMENTS = digfx_global.MAX_GFX_ELEMENTS;
        public static readonly u32 [] EXTENDED_XOFFS = digfx_global.EXTENDED_XOFFS;
        public static readonly u32 [] EXTENDED_YOFFS = digfx_global.EXTENDED_YOFFS;
        public const UInt32 GFX_RAW = digfx_global.GFX_RAW;
        public static UInt32 RGN_FRAC(UInt32 num, UInt32 den) { return digfx_global.RGN_FRAC(num, den); }
        public static bool IS_FRAC(UInt32 offset) { return digfx_global.IS_FRAC(offset); }
        public static UInt32 FRAC_NUM(UInt32 offset) { return digfx_global.FRAC_NUM(offset); }
        public static UInt32 FRAC_DEN(UInt32 offset) { return digfx_global.FRAC_DEN(offset); }
        public static UInt32 FRAC_OFFSET(UInt32 offset) { return digfx_global.FRAC_OFFSET(offset); }
        public static UInt32 [] ArrayCombineUInt32(params object [] objects) { return digfx_global.ArrayCombineUInt32(objects); }
        public static UInt32 [] STEP2(int START, int STEP) { return digfx_global.STEP2(START, STEP); }
        public static UInt32 [] STEP4(int START, int STEP) { return digfx_global.STEP4(START, STEP); }
        public static UInt32 [] STEP8(int START, int STEP) { return digfx_global.STEP8(START, STEP); }
        public static UInt32 [] STEP16(int START, int STEP) { return digfx_global.STEP16(START, STEP); }
        public static UInt32 [] STEP32(int START, int STEP) { return digfx_global.STEP32(START, STEP); }
        public static UInt32 GFXENTRY_GETXSCALE(UInt32 x) { return digfx_global.GFXENTRY_GETXSCALE(x); }
        public static UInt32 GFXENTRY_GETYSCALE(UInt32 x) { return digfx_global.GFXENTRY_GETYSCALE(x); }
        public static bool GFXENTRY_ISROM(UInt32 x) { return digfx_global.GFXENTRY_ISROM(x); }
        public static bool GFXENTRY_ISRAM(UInt32 x) { return digfx_global.GFXENTRY_ISRAM(x); }
        public static bool GFXENTRY_ISDEVICE(UInt32 x) { return digfx_global.GFXENTRY_ISDEVICE(x); }
        public static bool GFXENTRY_ISREVERSE(UInt32 x) { return digfx_global.GFXENTRY_ISREVERSE(x); }
        public static gfx_decode_entry GFXDECODE_ENTRY(string region, u32 offset, gfx_layout layout, u16 start, u16 colors) { return digfx_global.GFXDECODE_ENTRY(region, offset, layout, start, colors); }
        public static gfx_decode_entry GFXDECODE_SCALE(string region, u32 offset, gfx_layout layout, u16 start, u16 colors, u32 x, u32 y) { return digfx_global.GFXDECODE_SCALE(region, offset, layout, start, colors, x, y); }


        // disc_flt
        public static void calculate_filter2_coefficients(discrete_base_node node, double fc, double d, double type, ref discrete_filter_coeff coeff) { disc_flt_global.calculate_filter2_coefficients(node, fc, d, type, ref coeff); }


        // discrete
        public const int NODE_00 = discrete_global.NODE_00;
        public const int NODE_01 = discrete_global.NODE_01;
        public const int NODE_02 = discrete_global.NODE_02;
        public const int NODE_03 = discrete_global.NODE_03;
        public const int NODE_04 = discrete_global.NODE_04;
        public const int NODE_05 = discrete_global.NODE_05;
        public const int NODE_06 = discrete_global.NODE_06;
        public const int NODE_07 = discrete_global.NODE_07;
        public const int NODE_08 = discrete_global.NODE_08;
        public const int NODE_09 = discrete_global.NODE_09;
        public const int NODE_10 = discrete_global.NODE_10;
        public const int NODE_11 = discrete_global.NODE_11;
        public const int NODE_12 = discrete_global.NODE_12;
        public const int NODE_13 = discrete_global.NODE_13;
        public const int NODE_14 = discrete_global.NODE_14;
        public const int NODE_15 = discrete_global.NODE_15;
        public const int NODE_16 = discrete_global.NODE_16;
        public const int NODE_17 = discrete_global.NODE_17;
        public const int NODE_20 = discrete_global.NODE_20;
        public const int NODE_21 = discrete_global.NODE_21;
        public const int NODE_22 = discrete_global.NODE_22;
        public const int NODE_23 = discrete_global.NODE_23;
        public const int NODE_24 = discrete_global.NODE_24;
        public const int NODE_25 = discrete_global.NODE_25;
        public const int NODE_26 = discrete_global.NODE_26;
        public const int NODE_27 = discrete_global.NODE_27;
        public const int NODE_28 = discrete_global.NODE_28;
        public const int NODE_29 = discrete_global.NODE_29;
        public const int NODE_30 = discrete_global.NODE_30;
        public const int NODE_33 = discrete_global.NODE_33;
        public const int NODE_34 = discrete_global.NODE_34;
        public const int NODE_35 = discrete_global.NODE_35;
        public const int NODE_38 = discrete_global.NODE_38;
        public const int NODE_39 = discrete_global.NODE_39;
        public const int NODE_40 = discrete_global.NODE_40;
        public const int NODE_45 = discrete_global.NODE_45;
        public const int NODE_50 = discrete_global.NODE_50;
        public const int NODE_51 = discrete_global.NODE_51;
        public const int NODE_52 = discrete_global.NODE_52;
        public const int NODE_54 = discrete_global.NODE_54;
        public const int NODE_55 = discrete_global.NODE_55;
        public const int NODE_60 = discrete_global.NODE_60;
        public const int NODE_61 = discrete_global.NODE_61;
        public const int NODE_70 = discrete_global.NODE_70;
        public const int NODE_71 = discrete_global.NODE_71;
        public const int NODE_73 = discrete_global.NODE_73;
        public const int NODE_90 = discrete_global.NODE_90;
        public const int NODE_100 = discrete_global.NODE_100;
        public const int NODE_105 = discrete_global.NODE_105;
        public const int NODE_110 = discrete_global.NODE_110;
        public const int NODE_111 = discrete_global.NODE_111;
        public const int NODE_115 = discrete_global.NODE_115;
        public const int NODE_116 = discrete_global.NODE_116;
        public const int NODE_117 = discrete_global.NODE_117;
        public const int NODE_120 = discrete_global.NODE_120;
        public const int NODE_132 = discrete_global.NODE_132;
        public const int NODE_133 = discrete_global.NODE_133;
        public const int NODE_133_00 = discrete_global.NODE_133_00;
        public const int NODE_133_02 = discrete_global.NODE_133_02;
        public const int NODE_133_03 = discrete_global.NODE_133_03;
        public const int NODE_150 = discrete_global.NODE_150;
        public const int NODE_151 = discrete_global.NODE_151;
        public const int NODE_152 = discrete_global.NODE_152;
        public const int NODE_155 = discrete_global.NODE_155;
        public const int NODE_157 = discrete_global.NODE_157;
        public const int NODE_170 = discrete_global.NODE_170;
        public const int NODE_171 = discrete_global.NODE_171;
        public const int NODE_172 = discrete_global.NODE_172;
        public const int NODE_173 = discrete_global.NODE_173;
        public const int NODE_177 = discrete_global.NODE_177;
        public const int NODE_178 = discrete_global.NODE_178;
        public const int NODE_181 = discrete_global.NODE_181;
        public const int NODE_182 = discrete_global.NODE_182;
        public const int NODE_208 = discrete_global.NODE_208;
        public const int NODE_209 = discrete_global.NODE_209;
        public const int NODE_240 = discrete_global.NODE_240;
        public const int NODE_241 = discrete_global.NODE_241;
        public const int NODE_242 = discrete_global.NODE_242;
        public const int NODE_243 = discrete_global.NODE_243;
        public const int NODE_250 = discrete_global.NODE_250;
        public const int NODE_279 = discrete_global.NODE_279;
        public const int NODE_280 = discrete_global.NODE_280;
        public const int NODE_288 = discrete_global.NODE_288;
        public const int NODE_289 = discrete_global.NODE_289;
        public const int NODE_294 = discrete_global.NODE_294;
        public const int NODE_295 = discrete_global.NODE_295;
        public const int NODE_296 = discrete_global.NODE_296;
        public const int DISCRETE_MAX_NODES = discrete_global.DISCRETE_MAX_NODES;
        public const int DISCRETE_MAX_INPUTS = discrete_global.DISCRETE_MAX_INPUTS;
        public const int DISCRETE_MAX_OUTPUTS = discrete_global.DISCRETE_MAX_OUTPUTS;
        public const int DISCRETE_MAX_TASK_GROUPS = discrete_global.DISCRETE_MAX_TASK_GROUPS;
        public static double RC_CHARGE_EXP_DT(double rc, double dt) { return discrete_global.RC_CHARGE_EXP_DT(rc, dt); }
        public const double DEFAULT_TTL_V_LOGIC_1 = discrete_global.DEFAULT_TTL_V_LOGIC_1;
        public const double DISC_LINADJ = discrete_global.DISC_LINADJ;
        public const int DISC_CLK_MASK = discrete_global.DISC_CLK_MASK;
        public const int DISC_CLK_ON_F_EDGE = discrete_global.DISC_CLK_ON_F_EDGE;
        public const int DISC_CLK_ON_R_EDGE =  discrete_global.DISC_CLK_ON_R_EDGE;
        public const int DISC_CLK_BY_COUNT = discrete_global.DISC_CLK_BY_COUNT;
        public const int DISC_CLK_IS_FREQ = discrete_global.DISC_CLK_IS_FREQ;
        public const int DISC_COUNT_UP = discrete_global.DISC_COUNT_UP;
        public const int DISC_COUNTER_IS_7492 = discrete_global.DISC_COUNTER_IS_7492;
        public const int DISC_OUT_MASK = discrete_global.DISC_OUT_MASK;
        public const int DISC_OUT_IS_ENERGY = discrete_global.DISC_OUT_IS_ENERGY;
        public const int DISC_OUT_HAS_XTIME = discrete_global.DISC_OUT_HAS_XTIME;
        public const int DISC_LFSR_OR = discrete_global.DISC_LFSR_OR;
        public const int DISC_LFSR_AND = discrete_global.DISC_LFSR_AND;
        public const int DISC_LFSR_XNOR = discrete_global.DISC_LFSR_XNOR;
        public const int DISC_LFSR_NOR = discrete_global.DISC_LFSR_NOR;
        public const int DISC_LFSR_NAND = discrete_global.DISC_LFSR_NAND;
        public const int DISC_LFSR_IN1 = discrete_global.DISC_LFSR_IN1;
        public const int DISC_LFSR_NOT_IN1 = discrete_global.DISC_LFSR_NOT_IN1;
        public const int DISC_LFSR_XOR_INV_IN0 = discrete_global.DISC_LFSR_XOR_INV_IN0;
        public const int DISC_LFSR_IN0 = discrete_global.DISC_LFSR_IN0;
        public const int DISC_LFSR_REPLACE = discrete_global.DISC_LFSR_REPLACE;
        public const int DISC_LFSR_XOR = discrete_global.DISC_LFSR_XOR;
        public const int DISC_LFSR_NOT_IN0 = discrete_global.DISC_LFSR_NOT_IN0;
        public const int DISC_LFSR_XOR_INV_IN1 = discrete_global.DISC_LFSR_XOR_INV_IN1;
        public const int DISC_LFSR_FLAG_OUT_INVERT = discrete_global.DISC_LFSR_FLAG_OUT_INVERT;
        public const int DISC_LFSR_FLAG_RESET_TYPE_H = discrete_global.DISC_LFSR_FLAG_RESET_TYPE_H;
        public const int DISC_LFSR_FLAG_OUTPUT_F0 = discrete_global.DISC_LFSR_FLAG_OUTPUT_F0;
        public const int DISC_LFSR_FLAG_OUTPUT_SR_SN1 = discrete_global.DISC_LFSR_FLAG_OUTPUT_SR_SN1;
        public const int DISC_LADDER_MAXRES = discrete_global.DISC_LADDER_MAXRES;
        public const int DISC_FILTER_LOWPASS = discrete_global.DISC_FILTER_LOWPASS;
        public const int DISC_FILTER_HIGHPASS = discrete_global.DISC_FILTER_HIGHPASS;
        public const int DISC_FILTER_BANDPASS = discrete_global.DISC_FILTER_BANDPASS;
        public const int DISC_MIXER_IS_RESISTOR = discrete_global.DISC_MIXER_IS_RESISTOR;
        public const int DISC_MIXER_IS_OP_AMP = discrete_global.DISC_MIXER_IS_OP_AMP;
        public const int DISC_MIXER_IS_OP_AMP_WITH_RI = discrete_global.DISC_MIXER_IS_OP_AMP_WITH_RI;
        public static readonly int NODE_SPECIAL = discrete_global.NODE_SPECIAL;
        public const int NODE_START = discrete_global.NODE_START;
        public static readonly int NODE_END = discrete_global.NODE_END;
        public static int NODE_(int x) { return discrete_global.NODE_(x); }
        public static int NODE_CHILD_NODE_NUM(int x) { return discrete_global.NODE_CHILD_NODE_NUM(x); }
        public static int NODE_DEFAULT_NODE(int x) { return discrete_global.NODE_DEFAULT_NODE(x); }
        public static int NODE_INDEX(int x) { return discrete_global.NODE_INDEX(x); }
        public static int NODE_RELATIVE(int x, int y) { return discrete_global.NODE_RELATIVE(x, y); }
        public static bool IS_VALUE_A_NODE(int val) { return discrete_global.IS_VALUE_A_NODE(val); }
        public const int DISC_OP_AMP_IS_NORTON = discrete_global.DISC_OP_AMP_IS_NORTON;
        public const double OP_AMP_NORTON_VBE = discrete_global.OP_AMP_NORTON_VBE;
        public const double OP_AMP_VP_RAIL_OFFSET = discrete_global.OP_AMP_VP_RAIL_OFFSET;
        public const int DISC_OP_AMP_FILTER_IS_LOW_PASS_1 = discrete_global.DISC_OP_AMP_FILTER_IS_LOW_PASS_1;
        public const int DISC_OP_AMP_FILTER_IS_HIGH_PASS_1 = discrete_global.DISC_OP_AMP_FILTER_IS_HIGH_PASS_1;
        public const int DISC_OP_AMP_FILTER_IS_BAND_PASS_1 = discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1;
        public const int DISC_OP_AMP_FILTER_IS_BAND_PASS_1M = discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_1M;
        public const int DISC_OP_AMP_FILTER_IS_HIGH_PASS_0 = discrete_global.DISC_OP_AMP_FILTER_IS_HIGH_PASS_0;
        public const int DISC_OP_AMP_FILTER_IS_BAND_PASS_0 = discrete_global.DISC_OP_AMP_FILTER_IS_BAND_PASS_0;
        public const int DISC_OP_AMP_FILTER_IS_LOW_PASS_1_A = discrete_global.DISC_OP_AMP_FILTER_IS_LOW_PASS_1_A;
        public const int DISC_OP_AMP_FILTER_TYPE_MASK = discrete_global.DISC_OP_AMP_FILTER_TYPE_MASK;
        public const int DISC_SALLEN_KEY_LOW_PASS = discrete_global.DISC_SALLEN_KEY_LOW_PASS;
        public const int DISC_555_OUT_DC = discrete_global.DISC_555_OUT_DC;
        public const int DISC_555_OUT_AC = discrete_global.DISC_555_OUT_AC;
        public const int DISC_555_OUT_SQW = discrete_global.DISC_555_OUT_SQW;
        public const int DISC_555_OUT_CAP = discrete_global.DISC_555_OUT_CAP;
        public const int DISC_555_OUT_COUNT_F = discrete_global.DISC_555_OUT_COUNT_F;
        public const int DISC_555_OUT_COUNT_R = discrete_global.DISC_555_OUT_COUNT_R;
        public const int DISC_555_OUT_ENERGY = discrete_global.DISC_555_OUT_ENERGY;
        public const int DISC_555_OUT_LOGIC_X = discrete_global.DISC_555_OUT_LOGIC_X;
        public const int DISC_555_OUT_COUNT_F_X = discrete_global.DISC_555_OUT_COUNT_F_X;
        public const int DISC_555_OUT_COUNT_R_X = discrete_global.DISC_555_OUT_COUNT_R_X;
        public const int DISC_555_OUT_MASK = discrete_global.DISC_555_OUT_MASK;
        public const int DISC_555_ASTABLE_HAS_FAST_CHARGE_DIODE = discrete_global.DISC_555_ASTABLE_HAS_FAST_CHARGE_DIODE;
        public const int DISCRETE_555_CC_TO_CAP = discrete_global.DISCRETE_555_CC_TO_CAP;
        public const int DISC_RC_INTEGRATE_TYPE1 = discrete_global.DISC_RC_INTEGRATE_TYPE1;
        public const int DISC_RC_INTEGRATE_TYPE2 = discrete_global.DISC_RC_INTEGRATE_TYPE2;
        public const int DISC_RC_INTEGRATE_TYPE3 = discrete_global.DISC_RC_INTEGRATE_TYPE3;
        public const int DEFAULT_555_CHARGE = discrete_global.DEFAULT_555_CHARGE;
        public const int DEFAULT_555_HIGH = discrete_global.DEFAULT_555_HIGH;
        public const int DEFAULT_555_VALUES_1 = discrete_global.DEFAULT_555_VALUES_1;
        public const int DEFAULT_555_VALUES_2 = discrete_global.DEFAULT_555_VALUES_2;
        public const int DEFAULT_555_CC_SOURCE = discrete_global.DEFAULT_555_CC_SOURCE;
        public static discrete_block DISCRETE_SOUND_END { get { return discrete_global.DISCRETE_SOUND_END; } }
        public static discrete_block DISCRETE_ADJUSTMENT(int NODE, double MIN, double MAX, double LOGLIN, string TAG)  { return discrete_global.DISCRETE_ADJUSTMENT(NODE, MIN, MAX, LOGLIN, TAG); }
        public static discrete_block DISCRETE_INPUT_DATA(int NODE) { return discrete_global.DISCRETE_INPUT_DATA(NODE); }
        public static discrete_block DISCRETE_INPUTX_DATA(int NODE, double GAIN, double OFFSET, double INIT) { return discrete_global.DISCRETE_INPUTX_DATA(NODE, GAIN, OFFSET, INIT); }
        public static discrete_block DISCRETE_INPUT_LOGIC(int NODE) { return discrete_global.DISCRETE_INPUT_LOGIC(NODE); }
        public static discrete_block DISCRETE_INPUT_NOT(int NODE) { return discrete_global.DISCRETE_INPUT_NOT(NODE); }
        public static discrete_block DISCRETE_INPUTX_STREAM(int NODE, double NUM, double GAIN, double OFFSET) { return discrete_global.DISCRETE_INPUTX_STREAM(NODE, NUM, GAIN, OFFSET); }
        public static discrete_block DISCRETE_INPUT_BUFFER(int NODE, double NUM) { return discrete_global.DISCRETE_INPUT_BUFFER(NODE, NUM); }
        public static discrete_block DISCRETE_COUNTER(int NODE, double ENAB, double RESET, double CLK, double MIN, double MAX, double DIR, double INIT0, double CLKTYPE) { return discrete_global.DISCRETE_COUNTER(NODE, ENAB, RESET, CLK, MIN, MAX, DIR, INIT0, CLKTYPE); }
        public static discrete_block DISCRETE_LFSR_NOISE(int NODE, double ENAB, double RESET, double CLK, double AMPL, double FEED, double BIAS, discrete_lfsr_desc LFSRTB) { return discrete_global.DISCRETE_LFSR_NOISE(NODE, ENAB, RESET, CLK, AMPL, FEED, BIAS, LFSRTB); }
        public static discrete_block DISCRETE_NOTE(int NODE, double ENAB, double CLK, double DATA, double MAX1, double MAX2, double CLKTYPE) { return discrete_global.DISCRETE_NOTE(NODE, ENAB, CLK, DATA, MAX1, MAX2, CLKTYPE); }
        public static discrete_block DISCRETE_SQUAREWFIX(int NODE, double ENAB, double FREQ, double AMPL, double DUTY, double BIAS, double PHASE) { return discrete_global.DISCRETE_SQUAREWFIX(NODE, ENAB, FREQ, AMPL, DUTY, BIAS, PHASE); }
        public static discrete_block DISCRETE_INVERTER_OSC(int NODE, double ENAB, double MOD, double RCHARGE, double RP, double C, double R2, discrete_dss_inverter_osc_node.description INFO) { return discrete_global.DISCRETE_INVERTER_OSC(NODE, ENAB, MOD, RCHARGE, RP, C, R2, INFO); }
        public static discrete_block DISCRETE_ADDER2(int NODE, double ENAB, double INP0, double INP1) { return discrete_global.DISCRETE_ADDER2(NODE, ENAB, INP0, INP1); }
        public static discrete_block DISCRETE_CLAMP(int NODE, double INP0, double MIN, double MAX) { return discrete_global.DISCRETE_CLAMP(NODE, INP0, MIN, MAX); }
        public static discrete_block DISCRETE_DIVIDE(int NODE, double ENAB, double INP0, double INP1) { return discrete_global.DISCRETE_DIVIDE(NODE, ENAB, INP0, INP1); }
        public static discrete_block DISCRETE_LOGIC_INVERT(int NODE, double INP0) {return discrete_global.DISCRETE_LOGIC_INVERT(NODE, INP0); }
        public static discrete_block DISCRETE_BITS_DECODE(int NODE, double INP, double BIT_FROM, double BIT_TO, double VOUT) { return discrete_global.DISCRETE_BITS_DECODE(NODE, INP, BIT_FROM, BIT_TO, VOUT); }
        public static discrete_block DISCRETE_LOGIC_DFLIPFLOP(int NODE, double RESET, double SET, double CLK, double INP) { return discrete_global.DISCRETE_LOGIC_DFLIPFLOP(NODE, RESET, SET, CLK, INP); }
        public static discrete_block DISCRETE_MULTIPLY(int NODE, double INP0, double INP1) { return discrete_global.DISCRETE_MULTIPLY(NODE, INP0, INP1); }
        public static discrete_block DISCRETE_MULTADD(int NODE, double INP0, double INP1, double INP2) { return discrete_global.DISCRETE_MULTADD(NODE, INP0, INP1, INP2); }
        public static discrete_block DISCRETE_TRANSFORM2(int NODE, double INP0, double INP1, string FUNCT) { return discrete_global.DISCRETE_TRANSFORM2(NODE, INP0, INP1, FUNCT); }
        public static discrete_block DISCRETE_TRANSFORM3(int NODE, double INP0, double INP1, double INP2, string FUNCT) { return discrete_global.DISCRETE_TRANSFORM3(NODE, INP0, INP1, INP2, FUNCT); }
        public static discrete_block DISCRETE_TRANSFORM4(int NODE, double INP0, double INP1, double INP2, double INP3, string FUNCT) { return discrete_global.DISCRETE_TRANSFORM4(NODE, INP0, INP1, INP2, INP3, FUNCT); }
        public static discrete_block DISCRETE_TRANSFORM5(int NODE, double INP0, double INP1, double INP2, double INP3, double INP4, string FUNCT) {return discrete_global.DISCRETE_TRANSFORM5(NODE, INP0, INP1, INP2, INP3, INP4, FUNCT); }
        public static discrete_block DISCRETE_DAC_R1(int NODE, double DATA, double VDATA, discrete_dac_r1_ladder LADDER) { return discrete_global.DISCRETE_DAC_R1(NODE, DATA, VDATA, LADDER); }
        public static discrete_block DISCRETE_DIODE_MIXER2(int NODE, double IN0, double IN1, double [] TABLE) { return discrete_global.DISCRETE_DIODE_MIXER2(NODE, IN0, IN1, TABLE); }
        public static discrete_block DISCRETE_MIXER2(int NODE, double ENAB, double IN0, double IN1, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER2(NODE, ENAB, IN0, IN1, INFO); }
        public static discrete_block DISCRETE_MIXER3(int NODE, double ENAB, double IN0, double IN1, double IN2, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER3(NODE, ENAB, IN0, IN1, IN2, INFO); }
        public static discrete_block DISCRETE_MIXER4(int NODE, double ENAB, double IN0, double IN1, double IN2, double IN3, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER4(NODE, ENAB, IN0, IN1, IN2, IN3, INFO); }
        public static discrete_block DISCRETE_MIXER5(int NODE, double ENAB, double IN0, double IN1, double IN2, double IN3, double IN4, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER5(NODE, ENAB, IN0, IN1, IN2, IN3, IN4, INFO); }
        public static discrete_block DISCRETE_MIXER6(int NODE, double ENAB, double IN0, double IN1, double IN2, double IN3, double IN4, double IN5, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER6(NODE, ENAB, IN0, IN1, IN2, IN3, IN4, IN5, INFO); }
        public static discrete_block DISCRETE_SALLEN_KEY_FILTER(int NODE, double ENAB, double INP0, double TYPE, discrete_op_amp_filt_info INFO) { return discrete_global.DISCRETE_SALLEN_KEY_FILTER(NODE, ENAB, INP0, TYPE, INFO); }
        public static discrete_block DISCRETE_CRFILTER(int NODE, int INP0, double RVAL, double CVAL) { return discrete_global.DISCRETE_CRFILTER(NODE, INP0, RVAL, CVAL); }
        public static discrete_block DISCRETE_OP_AMP_FILTER(int NODE, double ENAB, double INP0, double INP1, double TYPE, discrete_op_amp_filt_info INFO) { return discrete_global.DISCRETE_OP_AMP_FILTER(NODE, ENAB, INP0, INP1, TYPE, INFO); }
        public static discrete_block DISCRETE_RCDISC(int NODE, double ENAB, double INP0, double RVAL, double CVAL) { return discrete_global.DISCRETE_RCDISC(NODE, ENAB, INP0, RVAL, CVAL); }
        public static discrete_block DISCRETE_RCDISC2(int NODE, double SWITCH, double INP0, double RVAL0, double INP1, double RVAL1, double CVAL) { return discrete_global.DISCRETE_RCDISC2(NODE, SWITCH, INP0, RVAL0, INP1, RVAL1, CVAL); }
        public static discrete_block DISCRETE_RCDISC5(int NODE, double ENAB, double INP0, double RVAL, double CVAL) { return discrete_global.DISCRETE_RCDISC5(NODE, ENAB, INP0, RVAL, CVAL); }
        public static discrete_block DISCRETE_RCDISC_MODULATED(int NODE, double INP0, double INP1, double RVAL0, double RVAL1, double RVAL2, double RVAL3, double CVAL, double VP) { return discrete_global.DISCRETE_RCDISC_MODULATED(NODE, INP0, INP1, RVAL0, RVAL1, RVAL2, RVAL3, CVAL, VP); }
        public static discrete_block DISCRETE_RCFILTER(int NODE, double INP0, double RVAL, double CVAL) { return discrete_global.DISCRETE_RCFILTER(NODE, INP0, RVAL, CVAL); }
        public static discrete_block DISCRETE_RCFILTER_SW(int NODE, double ENAB, double INP0, double SW, double RVAL, double CVAL1, double CVAL2, double CVAL3, double CVAL4) { return discrete_global.DISCRETE_RCFILTER_SW(NODE, ENAB, INP0, SW, RVAL, CVAL1, CVAL2, CVAL3, CVAL4); }
        public static discrete_block DISCRETE_RCINTEGRATE(int NODE, double INP0, double RVAL0, double RVAL1, double RVAL2, double CVAL, double vP, double TYPE) { return discrete_global.DISCRETE_RCINTEGRATE(NODE, INP0, RVAL0, RVAL1, RVAL2, CVAL, vP, TYPE); }
        public static discrete_block DISCRETE_CUSTOM8<CLASS>(int NODE, double IN0, double IN1, double IN2, double IN3, double IN4, double IN5, double IN6, double IN7, object INFO) where CLASS : discrete_base_node, new() { return discrete_global.DISCRETE_CUSTOM8<CLASS>(NODE, IN0, IN1, IN2, IN3, IN4, IN5, IN6, IN7, INFO); }
        public static discrete_block DISCRETE_555_ASTABLE_CV(int NODE, double RESET, double R1, double R2, double C, double CTRLV, discrete_555_desc OPTIONS) { return discrete_global.DISCRETE_555_ASTABLE_CV(NODE, RESET, R1, R2, C, CTRLV, OPTIONS); }
        public static discrete_block DISCRETE_555_CC(int NODE, double RESET, double VIN, double R, double C, double RBIAS, double RGND, double RDIS, discrete_555_cc_desc OPTIONS) { return discrete_global.DISCRETE_555_CC(NODE, RESET, VIN, R, C, RBIAS, RGND, RDIS, OPTIONS); }
        public static discrete_block DISCRETE_TASK_START(double TASK_GROUP) { return discrete_global.DISCRETE_TASK_START(TASK_GROUP); }
        public static discrete_block DISCRETE_TASK_END() { return discrete_global.DISCRETE_TASK_END(); }
        public static discrete_block DISCRETE_OUTPUT(double OPNODE, double GAIN) { return discrete_global.DISCRETE_OUTPUT(OPNODE, GAIN); }
        public const int MAX_SAMPLES_PER_TASK_SLICE = discrete_global.MAX_SAMPLES_PER_TASK_SLICE;
        public const int USE_DISCRETE_TASKS = discrete_global.USE_DISCRETE_TASKS;
        public static discrete_sound_device DISCRETE(machine_config mconfig, string tag, discrete_block [] intf)
        {
            var device = emu.detail.device_type_impl.op<discrete_sound_device>(mconfig, tag, discrete_sound_device.DISCRETE, 0);
            device.set_intf(intf);
            return device;
        }
        public static discrete_sound_device DISCRETE<bool_Required>(machine_config mconfig, device_finder<discrete_sound_device, bool_Required> finder) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, discrete_sound_device.DISCRETE, 0); }
        public static discrete_sound_device DISCRETE<bool_Required>(machine_config mconfig, device_finder<discrete_sound_device, bool_Required> finder, discrete_block [] intf)
            where bool_Required : bool_const, new()
        {
            var device = DISCRETE(mconfig, finder);
            device.set_intf(intf);
            return device;
        }


        // disound
        public const int ALL_OUTPUTS = disound_global.ALL_OUTPUTS;
        public const int AUTO_ALLOC_INPUT = disound_global.AUTO_ALLOC_INPUT;


        // distate
        public const int STATE_GENPC     = distate_global.STATE_GENPC;
        public const int STATE_GENPCBASE = distate_global.STATE_GENPCBASE;
        public const int STATE_GENSP     = distate_global.STATE_GENSP;
        public const int STATE_GENFLAGS  = distate_global.STATE_GENFLAGS;


        // drawgfx
        public static void copyscrollbitmap(bitmap_ind16 dest, bitmap_ind16 src, u32 numrows, s32 [] rowscroll, u32 numcols, s32 [] colscroll, rectangle cliprect) { drawgfx_global.copyscrollbitmap(dest, src, numrows, rowscroll, numcols, colscroll, cliprect); }
        public static void copyscrollbitmap_trans(bitmap_ind16 dest, bitmap_ind16 src, u32 numrows, s32 [] rowscroll, u32 numcols, s32 [] colscroll, rectangle cliprect, u32 trans_pen) { drawgfx_global.copyscrollbitmap_trans(dest, src, numrows, rowscroll, numcols, colscroll, cliprect, trans_pen); }
        public static u32 alpha_blend_r32(u32 d, u32 s, u8 level) { return drawgfx_global.alpha_blend_r32(d, s, level); }
        public static gfxdecode_device GFXDECODE(machine_config mconfig, string tag, string palette_tag, gfx_decode_entry [] gfxinfo)
        {
            var device = emu.detail.device_type_impl.op<gfxdecode_device>(mconfig, tag, gfxdecode_device.GFXDECODE, 0);
            device.gfxdecode_device_after_ctor(palette_tag, gfxinfo);
            return device;
        }
        public static gfxdecode_device GFXDECODE<bool_Required>(machine_config mconfig, device_finder<gfxdecode_device, bool_Required> finder, finder_base palette, gfx_decode_entry [] gfxinfo)
            where bool_Required : bool_const, new()
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, gfxdecode_device.GFXDECODE, 0);
            device.gfxdecode_device_after_ctor(palette, gfxinfo);
            return device;
        }


        // driver
        public static void MCFG_MACHINE_START_OVERRIDE(machine_config config, driver_callback_delegate func) { driver_global.MCFG_MACHINE_START_OVERRIDE(config, func); }
        public static void MCFG_MACHINE_RESET_OVERRIDE(machine_config config, driver_callback_delegate func) { driver_global.MCFG_MACHINE_RESET_OVERRIDE(config, func); }
        public static void MCFG_VIDEO_START_OVERRIDE(machine_config config, driver_callback_delegate func) { driver_global.MCFG_VIDEO_START_OVERRIDE(config, func); }


        // eeprom
        public static eeprom_parallel_2804_device EEPROM_2804(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<eeprom_parallel_2804_device>(mconfig, tag, eeprom_parallel_2804_device.EEPROM_2804, clock); }


        // eminline
        public static int64_t mul_32x32(int32_t a, int32_t b) { return eminline_global.mul_32x32(a, b); }
        public static uint64_t mulu_32x32(uint32_t a, uint32_t b) { return eminline_global.mulu_32x32(a, b); }
        public static int32_t mul_32x32_hi(int32_t a, int32_t b) { return eminline_global.mul_32x32_hi(a, b); }
        public static int32_t div_32x32_shift(int32_t a, int32_t b, uint8_t shift) { return eminline_global.div_32x32_shift(a, b, shift); }
        public static uint32_t divu_64x32(uint64_t a, uint32_t b) { return eminline_global.divu_64x32(a, b); }
        public static uint32_t divu_64x32_rem(uint64_t a, uint32_t b, out uint32_t remainder) { return eminline_global.divu_64x32_rem(a, b, out remainder); }
        public static unsigned population_count_32(uint32_t val) { return eminline_global.population_count_32(val); }
        public static int64_t get_profile_ticks() { return eminline_global.get_profile_ticks(); }


        // emucore
        public static readonly string [] endianness_names = emucore_global.endianness_names;
        public const endianness_t ENDIANNESS_NATIVE = emucore_global.ENDIANNESS_NATIVE;
        public const int ORIENTATION_FLIP_X = emucore_global.ORIENTATION_FLIP_X;
        public const int ORIENTATION_FLIP_Y = emucore_global.ORIENTATION_FLIP_Y;
        public const int ORIENTATION_SWAP_XY = emucore_global.ORIENTATION_SWAP_XY;
        public const int ROT0   = emucore_global.ROT0;
        public const int ROT90  = emucore_global.ROT90;
        public const int ROT180 = emucore_global.ROT180;
        public const int ROT270 = emucore_global.ROT270;
        public static Tuple<object, string> NAME<T>(T x) { return emucore_global.NAME(x); }
        [DebuggerHidden] public static void fatalerror(string format, params object [] args) { emucore_global.fatalerror(format, args); }
        [DebuggerHidden][Conditional("DEBUG")] public static void assert(bool condition, string message = "") { emucore_global.assert(condition, message); }
        [Conditional("ASSERT_SLOW")] public static void assert_slow(bool condition) { emucore_global.assert(condition); }
        [Conditional("DEBUG")] public static void static_assert(bool condition, string message = "") { emucore_global.assert(condition, message); }


        // emumem
        public const int AS_PROGRAM = emumem_global.AS_PROGRAM;
        public const int AS_DATA = emumem_global.AS_DATA;
        public const int AS_IO = emumem_global.AS_IO;
        public const int AS_OPCODES = emumem_global.AS_OPCODES;
        public static void COMBINE_DATA(ref u16 varptr, u16 data, u16 mem_mask) { emumem_global.COMBINE_DATA(ref varptr, data, mem_mask); }
        public static void COMBINE_DATA(ref u32 varptr, u32 data, u32 mem_mask) { emumem_global.COMBINE_DATA(ref varptr, data, mem_mask); }
        public static bool ACCESSING_BITS_0_7(u16 mem_mask) { return emumem_global.ACCESSING_BITS_0_7(mem_mask); }


        // emuopts
        public static void conditionally_peg_priority(core_options.entry entry, bool peg_priority) { emuopts_global.conditionally_peg_priority(entry, peg_priority); }


        // emupal
        public static palette_device PALETTE(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<palette_device>(mconfig, tag, palette_device.PALETTE, 0); }
        public static palette_device PALETTE<bool_Required>(machine_config mconfig, device_finder<palette_device, bool_Required> finder) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, palette_device.PALETTE, 0); }
        public static palette_device PALETTE<bool_Required>(machine_config mconfig, device_finder<palette_device, bool_Required> finder, palette_device.init_delegate init, u32 entries = 0U, u32 indirect = 0U)
            where bool_Required : bool_const, new()
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, palette_device.PALETTE, 0);
            device.palette_device_after_ctor(init, entries, indirect);
            return device;
        }


        // er2055
        public static er2055_device ER2055<bool_Required>(machine_config mconfig, device_finder<er2055_device, bool_Required> finder, u32 clock = 0) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, er2055_device.ER2055, clock); }


        // galaxian
        public static galaxian_sound_device GALAXIAN_SOUND(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<galaxian_sound_device>(mconfig, tag, galaxian_sound_device.GALAXIAN_SOUND, clock); }


        // gamedrv
        public const u64 MACHINE_TYPE_ARCADE = gamedrv_global.MACHINE_TYPE_ARCADE;
        public const u64 MACHINE_NOT_WORKING = gamedrv_global.MACHINE_NOT_WORKING;
        public const u64 MACHINE_SUPPORTS_SAVE = gamedrv_global.MACHINE_SUPPORTS_SAVE;
        public const u64 MACHINE_IS_BIOS_ROOT = gamedrv_global.MACHINE_IS_BIOS_ROOT;
        public const u64 MACHINE_CLICKABLE_ARTWORK = gamedrv_global.MACHINE_CLICKABLE_ARTWORK;
        public const u64 MACHINE_NO_SOUND_HW = gamedrv_global.MACHINE_NO_SOUND_HW;
        public const u64 MACHINE_UNEMULATED_PROTECTION = gamedrv_global.MACHINE_UNEMULATED_PROTECTION;
        public const u64 MACHINE_WRONG_COLORS = gamedrv_global.MACHINE_WRONG_COLORS;
        public const u64 MACHINE_IMPERFECT_COLORS = gamedrv_global.MACHINE_IMPERFECT_COLORS;
        public const u64 MACHINE_IMPERFECT_GRAPHICS = gamedrv_global.MACHINE_IMPERFECT_GRAPHICS;
        public const u64 MACHINE_NO_SOUND = gamedrv_global.MACHINE_NO_SOUND;
        public const u64 MACHINE_IMPERFECT_SOUND = gamedrv_global.MACHINE_IMPERFECT_SOUND;
        public const u64 MACHINE_IMPERFECT_CONTROLS = gamedrv_global.MACHINE_IMPERFECT_CONTROLS;
        public const u64 MACHINE_NODEVICE_MICROPHONE = gamedrv_global.MACHINE_NODEVICE_MICROPHONE;
        public const u64 MACHINE_NODEVICE_PRINTER = gamedrv_global.MACHINE_NODEVICE_PRINTER;
        public const u64 MACHINE_NODEVICE_LAN = gamedrv_global.MACHINE_NODEVICE_LAN;
        public const u64 MACHINE_IMPERFECT_TIMING = gamedrv_global.MACHINE_IMPERFECT_TIMING;
        public static game_driver GAME(device_type.create_func creator, MemoryContainer<tiny_rom_entry> roms, string YEAR, string NAME, string PARENT, machine_creator_wrapper MACHINE, ioport_constructor INPUT, driver_init_wrapper INIT, int MONITOR, string COMPANY, string FULLNAME, u64 FLAGS) { return gamedrv_global.GAME(creator, roms, YEAR, NAME, PARENT, MACHINE, INPUT, INIT, MONITOR, COMPANY, FULLNAME, FLAGS); }


        // gen_latch
        public static generic_latch_8_device GENERIC_LATCH_8<bool_Required>(machine_config mconfig, device_finder<generic_latch_8_device, bool_Required> finder, u32 clock = 0) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, generic_latch_8_device.GENERIC_LATCH_8, clock); }


        // hash
        public static string CRC(string x) { return hash_global.CRC(x); }
        public static string SHA1(string x) { return hash_global.SHA1(x); }
        public const string NO_DUMP = hash_global.NO_DUMP;


        // i8255
        public static i8255_device I8255A<bool_Required>(machine_config mconfig, device_finder<i8255_device, bool_Required> finder, u32 clock = 0) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, i8255_device.I8255A, clock); }


        // i8257
        public static i8257_device I8257<bool_Required>(machine_config mconfig, device_finder<i8257_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, i8257_device.I8257, clock); }


        // input
        public static readonly input_code KEYCODE_F1 = input_global.KEYCODE_F1;
        public static readonly input_code KEYCODE_LSHIFT = input_global.KEYCODE_LSHIFT;
        public static readonly input_code KEYCODE_RSHIFT = input_global.KEYCODE_RSHIFT;
        public static readonly input_code KEYCODE_LALT = input_global.KEYCODE_LALT;
        public static readonly input_code KEYCODE_RALT = input_global.KEYCODE_RALT;
        public static readonly input_code KEYCODE_LCONTROL = input_global.KEYCODE_LCONTROL;
        public static readonly input_code KEYCODE_RCONTROL = input_global.KEYCODE_RCONTROL;


        // input_merger
        public static input_merger_device INPUT_MERGER_ANY_HIGH<bool_Required>(machine_config mconfig, device_finder<input_merger_device, bool_Required> finder, u32 clock = 0) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, input_merger_any_high_device.INPUT_MERGER_ANY_HIGH, clock); }
        public static input_merger_device INPUT_MERGER_ALL_HIGH<bool_Required>(machine_config mconfig, device_finder<input_merger_device, bool_Required> finder, u32 clock = 0) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, input_merger_all_high_device.INPUT_MERGER_ALL_HIGH, clock); }


        // ioport
        public const ioport_value IP_ACTIVE_HIGH = ioport_global.IP_ACTIVE_HIGH;
        public const ioport_value IP_ACTIVE_LOW = ioport_global.IP_ACTIVE_LOW;
        public const int MAX_PLAYERS = ioport_global.MAX_PLAYERS;
        public const char32_t UCHAR_SHIFT_1 = ioport_global.UCHAR_SHIFT_1;
        public const char32_t UCHAR_SHIFT_2 = ioport_global.UCHAR_SHIFT_2;
        public const char32_t UCHAR_SHIFT_BEGIN = ioport_global.UCHAR_SHIFT_BEGIN;
        public const char32_t UCHAR_SHIFT_END = ioport_global.UCHAR_SHIFT_END;
        public const char32_t UCHAR_MAMEKEY_BEGIN = ioport_global.UCHAR_MAMEKEY_BEGIN;
        public const ioport_type IPT_UNUSED = ioport_type.IPT_UNUSED;
        public const ioport_type IPT_UNKNOWN = ioport_type.IPT_UNKNOWN;
        public const ioport_type IPT_START1 = ioport_type.IPT_START1;
        public const ioport_type IPT_START2 = ioport_type.IPT_START2;
        public const ioport_type IPT_COIN1 = ioport_type.IPT_COIN1;
        public const ioport_type IPT_COIN2 = ioport_type.IPT_COIN2;
        public const ioport_type IPT_COIN3 = ioport_type.IPT_COIN3;
        public const ioport_type IPT_SERVICE1 = ioport_type.IPT_SERVICE1;
        public const ioport_type IPT_SERVICE = ioport_type.IPT_SERVICE;
        public const ioport_type IPT_TILT = ioport_type.IPT_TILT;
        public const ioport_type IPT_JOYSTICK_UP = ioport_type.IPT_JOYSTICK_UP;
        public const ioport_type IPT_JOYSTICK_DOWN = ioport_type.IPT_JOYSTICK_DOWN;
        public const ioport_type IPT_JOYSTICK_LEFT = ioport_type.IPT_JOYSTICK_LEFT;
        public const ioport_type IPT_JOYSTICK_RIGHT = ioport_type.IPT_JOYSTICK_RIGHT;
        public const ioport_type IPT_BUTTON1 = ioport_type.IPT_BUTTON1;
        public const ioport_type IPT_BUTTON2 = ioport_type.IPT_BUTTON2;
        public const ioport_type IPT_AD_STICK_X = ioport_type.IPT_AD_STICK_X;
        public const ioport_type IPT_AD_STICK_Y = ioport_type.IPT_AD_STICK_Y;
        public const ioport_type IPT_DIAL = ioport_type.IPT_DIAL;
        public const ioport_type IPT_DIAL_V = ioport_type.IPT_DIAL_V;
        public const ioport_type IPT_TRACKBALL_X = ioport_type.IPT_TRACKBALL_X;
        public const ioport_type IPT_TRACKBALL_Y = ioport_type.IPT_TRACKBALL_Y;
        public const ioport_type IPT_SPECIAL = ioport_type.IPT_SPECIAL;
        public const ioport_type IPT_CUSTOM = ioport_type.IPT_CUSTOM;
        public const INPUT_STRING Off = INPUT_STRING.INPUT_STRING_Off;
        public const INPUT_STRING On = INPUT_STRING.INPUT_STRING_On;
        public const INPUT_STRING No = INPUT_STRING.INPUT_STRING_No;
        public const INPUT_STRING Yes = INPUT_STRING.INPUT_STRING_Yes;
        public const INPUT_STRING Lives = INPUT_STRING.INPUT_STRING_Lives;
        public const INPUT_STRING Bonus_Life = INPUT_STRING.INPUT_STRING_Bonus_Life;
        public const INPUT_STRING Difficulty = INPUT_STRING.INPUT_STRING_Difficulty;
        public const INPUT_STRING Demo_Sounds = INPUT_STRING.INPUT_STRING_Demo_Sounds;
        public const INPUT_STRING Coinage = INPUT_STRING.INPUT_STRING_Coinage;
        public const INPUT_STRING Coin_A = INPUT_STRING.INPUT_STRING_Coin_A;
        public const INPUT_STRING Coin_B = INPUT_STRING.INPUT_STRING_Coin_B;
        public const INPUT_STRING _9C_1C = INPUT_STRING.INPUT_STRING_9C_1C;
        public const INPUT_STRING _8C_1C = INPUT_STRING.INPUT_STRING_8C_1C;
        public const INPUT_STRING _7C_1C = INPUT_STRING.INPUT_STRING_7C_1C;
        public const INPUT_STRING _6C_1C = INPUT_STRING.INPUT_STRING_6C_1C;
        public const INPUT_STRING _5C_1C = INPUT_STRING.INPUT_STRING_5C_1C;
        public const INPUT_STRING _4C_1C = INPUT_STRING.INPUT_STRING_4C_1C;
        public const INPUT_STRING _3C_1C = INPUT_STRING.INPUT_STRING_3C_1C;
        public const INPUT_STRING _2C_1C = INPUT_STRING.INPUT_STRING_2C_1C;
        public const INPUT_STRING _1C_1C = INPUT_STRING.INPUT_STRING_1C_1C;
        public const INPUT_STRING _2C_3C = INPUT_STRING.INPUT_STRING_2C_3C;
        public const INPUT_STRING _1C_2C = INPUT_STRING.INPUT_STRING_1C_2C;
        public const INPUT_STRING _1C_3C = INPUT_STRING.INPUT_STRING_1C_3C;
        public const INPUT_STRING _1C_4C = INPUT_STRING.INPUT_STRING_1C_4C;
        public const INPUT_STRING _1C_5C = INPUT_STRING.INPUT_STRING_1C_5C;
        public const INPUT_STRING _1C_6C = INPUT_STRING.INPUT_STRING_1C_6C;
        public const INPUT_STRING _1C_7C = INPUT_STRING.INPUT_STRING_1C_7C;
        public const INPUT_STRING _1C_8C = INPUT_STRING.INPUT_STRING_1C_8C;
        public const INPUT_STRING Free_Play = INPUT_STRING.INPUT_STRING_Free_Play;
        public const INPUT_STRING Cabinet = INPUT_STRING.INPUT_STRING_Cabinet;
        public const INPUT_STRING Upright = INPUT_STRING.INPUT_STRING_Upright;
        public const INPUT_STRING Cocktail = INPUT_STRING.INPUT_STRING_Cocktail;
        public const INPUT_STRING Flip_Screen = INPUT_STRING.INPUT_STRING_Flip_Screen;
        public const INPUT_STRING Language = INPUT_STRING.INPUT_STRING_Language;
        public const INPUT_STRING English = INPUT_STRING.INPUT_STRING_English;
        public const INPUT_STRING Japanese = INPUT_STRING.INPUT_STRING_Japanese;
        public const INPUT_STRING Chinese = INPUT_STRING.INPUT_STRING_Chinese;
        public const INPUT_STRING French = INPUT_STRING.INPUT_STRING_French;
        public const INPUT_STRING German = INPUT_STRING.INPUT_STRING_German;
        public const INPUT_STRING Italian = INPUT_STRING.INPUT_STRING_Italian;
        public const INPUT_STRING Korean = INPUT_STRING.INPUT_STRING_Korean;
        public const INPUT_STRING Spanish = INPUT_STRING.INPUT_STRING_Spanish;
        public const INPUT_STRING Easiest = INPUT_STRING.INPUT_STRING_Easiest;
        public const INPUT_STRING Easy = INPUT_STRING.INPUT_STRING_Easy;
        public const INPUT_STRING Normal = INPUT_STRING.INPUT_STRING_Normal;
        public const INPUT_STRING Medium = INPUT_STRING.INPUT_STRING_Medium;
        public const INPUT_STRING Medium_Hard = INPUT_STRING.INPUT_STRING_Medium_Hard;
        public const INPUT_STRING Hard = INPUT_STRING.INPUT_STRING_Hard;
        public const INPUT_STRING Hardest = INPUT_STRING.INPUT_STRING_Hardest;
        public const INPUT_STRING Difficult = INPUT_STRING.INPUT_STRING_Difficult;
        public const INPUT_STRING Very_Difficult = INPUT_STRING.INPUT_STRING_Very_Difficult;
        public const INPUT_STRING Allow_Continue = INPUT_STRING.INPUT_STRING_Allow_Continue;
        public const INPUT_STRING Unused = INPUT_STRING.INPUT_STRING_Unused;
        public const INPUT_STRING Unknown = INPUT_STRING.INPUT_STRING_Unknown;
        public const INPUT_STRING Alternate = INPUT_STRING.INPUT_STRING_Alternate;
        public const INPUT_STRING None = INPUT_STRING.INPUT_STRING_None;
        public static char32_t UCHAR_MAMEKEY(char32_t code) { return ioport_global.UCHAR_MAMEKEY(code); }
        public static string DEF_STR(INPUT_STRING str_num) { return ioport_global.DEF_STR(str_num); }


        // irem
        public static m52_soundc_audio_device IREM_M52_SOUNDC_AUDIO(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<m52_soundc_audio_device>(mconfig, tag, m52_soundc_audio_device.IREM_M52_SOUNDC_AUDIO, clock); }


        // language
        public static string __(string param) { return language_global.__(param); }


        // latch8
        public static latch8_device LATCH8<bool_Required>(machine_config mconfig, device_finder<latch8_device, bool_Required> finder, u32 clock = 0) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, latch8_device.LATCH8, clock); }


        // logmacro
        public static void LOGMASKED(int VERBOSE, int mask, device_t device, string format, params object [] args) { logmacro_global.LOGMASKED(VERBOSE, mask, device, format, args); }
        public static void LOGMASKED(int VERBOSE, int mask, device_t device, logmacro_global.LOG_OUTPUT_FUNC log_output_func, string format, params object [] args) { logmacro_global.LOGMASKED(VERBOSE, mask, device, log_output_func, format, args); }
        public static void LOG(int VERBOSE, device_t device, string format, params object [] args) { logmacro_global.LOG(VERBOSE, device, format, args); }
        public static void LOG(int VERBOSE, device_t device, logmacro_global.LOG_OUTPUT_FUNC log_output_func, string format, params object [] args) { logmacro_global.LOG(VERBOSE, device, log_output_func, format, args); }


        // m6502
        public static m6502_device M6502<bool_Required>(machine_config mconfig, device_finder<m6502_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, m6502_device.M6502, clock); }
        public static m6502_device M6502<bool_Required>(machine_config mconfig, device_finder<m6502_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, m6502_device.M6502, clock); }


        // m6801
        public static m6803_cpu_device M6803<bool_Required>(machine_config mconfig, device_finder<m6803_cpu_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, m6803_cpu_device.M6803, clock); }


        // m68705
        public static m68705p_device M68705P5<bool_Required>(machine_config mconfig, device_finder<m68705p_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, m68705p5_device.M68705P5, clock); }


        // machine
        public const int DEBUG_FLAG_ENABLED = machine_global.DEBUG_FLAG_ENABLED;
        public const int DEBUG_FLAG_CALL_HOOK = machine_global.DEBUG_FLAG_CALL_HOOK;
        public const int DEBUG_FLAG_OSD_ENABLED = machine_global.DEBUG_FLAG_OSD_ENABLED;


        // main
        public const int EMU_ERR_NONE = main_global.EMU_ERR_NONE;
        public const int EMU_ERR_FAILED_VALIDITY = main_global.EMU_ERR_FAILED_VALIDITY;
        public const int EMU_ERR_MISSING_FILES = main_global.EMU_ERR_MISSING_FILES;
        public const int EMU_ERR_DEVICE = main_global.EMU_ERR_DEVICE;
        public const int EMU_ERR_NO_SUCH_SYSTEM = main_global.EMU_ERR_NO_SUCH_SYSTEM;
        public const int EMU_ERR_INVALID_CONFIG = main_global.EMU_ERR_INVALID_CONFIG;


        // mcs48
        public static mcs48_cpu_device MB8884<bool_Required>(machine_config mconfig, device_finder<mcs48_cpu_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, mb8884_device.MB8884, clock); }


        // mb88xx
        public static mb88_cpu_device MB8842<bool_Required>(machine_config mconfig, device_finder<mb88_cpu_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, mb8842_cpu_device.MB8842, clock); }
        public static mb88_cpu_device MB8843<bool_Required>(machine_config mconfig, device_finder<mb88_cpu_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, mb8843_cpu_device.MB8843, clock); }
        public static mb88_cpu_device MB8844<bool_Required>(machine_config mconfig, device_finder<mb88_cpu_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, mb8844_cpu_device.MB8844, clock); }


        // msm5205
        public static msm5205_device MSM5205<bool_Required>(machine_config mconfig, device_finder<msm5205_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, msm5205_device.MSM5205, clock); }


        // namco
        public static namco_device NAMCO<bool_Required>(machine_config mconfig, device_finder<namco_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, namco_device.NAMCO, clock); }


        // namco06
        public static namco_06xx_device NAMCO_06XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_06xx_device>(mconfig, tag, namco_06xx_device.NAMCO_06XX, clock); }


        // namco50
        public static namco_50xx_device NAMCO_50XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_50xx_device>(mconfig, tag, namco_50xx_device.NAMCO_50XX, clock); }


        // namco51
        public static namco_51xx_device NAMCO_51XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_51xx_device>(mconfig, tag, namco_51xx_device.NAMCO_51XX, clock); }


        // namco53
        public static namco_53xx_device NAMCO_53XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_53xx_device>(mconfig, tag, namco_53xx_device.NAMCO_53XX, clock); }


        // namco54
        public static int NAMCO_54XX_0_DATA(int base_node) { return namco54_global.NAMCO_54XX_0_DATA(base_node); }
        public static int NAMCO_54XX_1_DATA(int base_node) { return namco54_global.NAMCO_54XX_1_DATA(base_node); }
        public static int NAMCO_54XX_2_DATA(int base_node) { return namco54_global.NAMCO_54XX_2_DATA(base_node); }
        public static namco_54xx_device NAMCO_54XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_54xx_device>(mconfig, tag, namco_54xx_device.NAMCO_54XX, clock); }


        // netlist
        public static netlist_mame_sound_device NETLIST_SOUND(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<netlist_mame_sound_device>(mconfig, tag, netlist_mame_sound_device.NETLIST_SOUND, clock); }
        public static netlist_mame_sound_device NETLIST_SOUND(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<netlist_mame_sound_device>(mconfig, tag, netlist_mame_sound_device.NETLIST_SOUND, clock); }
        public static netlist_mame_logic_input_device NETLIST_LOGIC_INPUT(machine_config mconfig, string tag, string param_name, uint32_t shift)
        {
            var device = emu.detail.device_type_impl.op<netlist_mame_logic_input_device>(mconfig, tag, netlist_mame_logic_input_device.NETLIST_LOGIC_INPUT, 0);
            device.set_params(param_name, shift);
            return device;
        }
        public static netlist_mame_stream_input_device NETLIST_STREAM_INPUT(machine_config mconfig, string tag, int channel, string param_name)
        {
            var device = emu.detail.device_type_impl.op<netlist_mame_stream_input_device>(mconfig, tag, netlist_mame_stream_input_device.NETLIST_STREAM_INPUT, 0);
            device.set_params(channel, param_name);
            return device;
        }
        public static netlist_mame_stream_output_device NETLIST_STREAM_OUTPUT(machine_config mconfig, string tag, int channel, string out_name)
        {
            var device = emu.detail.device_type_impl.op<netlist_mame_stream_output_device>(mconfig, tag, netlist_mame_stream_output_device.NETLIST_STREAM_OUTPUT, 0);
            device.set_params(channel, out_name);
            return device;
        }


        // nl_factory
        public static netlist.factory.constructor_ptr_t NETLIB_DEVICE_IMPL_ALIAS<chip>(string p_alias, string p_name, string p_def_param) { return netlist.factory.nl_factory_global.NETLIB_DEVICE_IMPL_ALIAS<chip>(p_alias, p_name, p_def_param); }
        public static netlist.factory.constructor_ptr_t NETLIB_DEVICE_IMPL<chip>(string p_name, string p_def_param) { return netlist.factory.nl_factory_global.NETLIB_DEVICE_IMPL<chip>(p_name, p_def_param); }
        public static netlist.factory.constructor_ptr_t NETLIB_DEVICE_IMPL_NS<chip>(string ns, string p_name, string p_def_param) { return netlist.factory.nl_factory_global.NETLIB_DEVICE_IMPL_NS<chip>(ns, p_name, p_def_param); }


        // options
        public const int OPTION_PRIORITY_DEFAULT = options_global.OPTION_PRIORITY_DEFAULT;
        public const int OPTION_PRIORITY_NORMAL = options_global.OPTION_PRIORITY_NORMAL;
        public const int OPTION_PRIORITY_HIGH = options_global.OPTION_PRIORITY_HIGH;
        public const int OPTION_PRIORITY_MAXIMUM = options_global.OPTION_PRIORITY_MAXIMUM;
        public const core_options.option_type OPTION_HEADER = options_global.OPTION_HEADER;
        public const core_options.option_type OPTION_COMMAND = options_global.OPTION_COMMAND;
        public const core_options.option_type OPTION_BOOLEAN = options_global.OPTION_BOOLEAN;
        public const core_options.option_type OPTION_INTEGER = options_global.OPTION_INTEGER;
        public const core_options.option_type OPTION_FLOAT = options_global.OPTION_FLOAT;
        public const core_options.option_type OPTION_STRING = options_global.OPTION_STRING;


        // osdcomm
        public static uint32_t swapendian_int32(uint32_t val) { return osdcomm_global.swapendian_int32(val); }
        public static int16_t little_endianize_int16(int16_t x) { return osdcomm_global.little_endianize_int16(x); }
        public static uint16_t little_endianize_int16(uint16_t x) { return osdcomm_global.little_endianize_int16(x); }
        public static int32_t little_endianize_int32(int32_t x) { return osdcomm_global.little_endianize_int32(x); }
        public static uint32_t little_endianize_int32(uint32_t x) { return osdcomm_global.little_endianize_int32(x); }


        // osdcore
        public static void osd_printf_error(string format, params object [] args) { osdcore_interface.osd_printf_error(format, args); }
        public static void osd_printf_warning(string format, params object [] args) { osdcore_interface.osd_printf_warning(format, args); }
        public static void osd_printf_info(string format, params object [] args) { osdcore_interface.osd_printf_info(format, args); }
        public static void osd_printf_verbose(string format, params object [] args) { osdcore_interface.osd_printf_verbose(format, args); }
        public static void osd_printf_debug(string format, params object [] args) { osdcore_interface.osd_printf_debug(format, args); }


        // osdfile
        public const string PATH_SEPARATOR = osdfile_global.PATH_SEPARATOR;
        public const uint32_t OPEN_FLAG_READ = osdfile_global.OPEN_FLAG_READ;
        public const uint32_t OPEN_FLAG_WRITE = osdfile_global.OPEN_FLAG_WRITE;
        public const uint32_t OPEN_FLAG_CREATE = osdfile_global.OPEN_FLAG_CREATE;
        public const uint32_t OPEN_FLAG_CREATE_PATHS = osdfile_global.OPEN_FLAG_CREATE_PATHS;
        public const uint32_t OPEN_FLAG_NO_PRELOAD = osdfile_global.OPEN_FLAG_NO_PRELOAD;


        // palette
        public static uint8_t pal5bit(uint8_t bits) { return palette_global.pal5bit(bits); }
        public static rgb_t rgbexpand<int__RBits, int__GBits, int__BBits>(uint32_t data, uint8_t rshift, uint8_t gshift, uint8_t bshift)
            where int__RBits : int_const, new()
            where int__GBits : int_const, new()
            where int__BBits : int_const, new()
        { return palette_global.rgbexpand<int__RBits, int__GBits, int__BBits>(data, rshift, gshift, bshift); }


        // pokey
        public static pokey_device POKEY(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<pokey_device>(mconfig, tag, pokey_device.POKEY, clock); }
        public static pokey_device POKEY<bool_Required>(machine_config mconfig, device_finder<pokey_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, pokey_device.POKEY, clock); }


        // render
        public const byte RENDER_CREATE_HIDDEN = render_global.RENDER_CREATE_HIDDEN;
        public static u32 PRIMFLAG_TEXORIENT(u32 x) { return render_global.PRIMFLAG_TEXORIENT(x); }
        public static u32 PRIMFLAG_BLENDMODE(u32 x) { return render_global.PRIMFLAG_BLENDMODE(x); }
        public static u32 PRIMFLAG_ANTIALIAS(u32 x) { return render_global.PRIMFLAG_ANTIALIAS(x); }
        public static u32 PRIMFLAG_SCREENTEX(u32 x) { return render_global.PRIMFLAG_SCREENTEX(x); }
        public static u32 PRIMFLAG_TEXWRAP(u32 x) { return render_global.PRIMFLAG_TEXWRAP(x); }
        public const u32 PRIMFLAG_PACKABLE = render_global.PRIMFLAG_PACKABLE;


        // rendertypes
        public const int BLENDMODE_NONE = rendertypes_global.BLENDMODE_NONE;
        public const int BLENDMODE_ALPHA = rendertypes_global.BLENDMODE_ALPHA;
        public const int BLENDMODE_RGB_MULTIPLY = rendertypes_global.BLENDMODE_RGB_MULTIPLY;
        public const int BLENDMODE_ADD = rendertypes_global.BLENDMODE_ADD;


        // rendutil
        public static void render_resample_argb_bitmap_hq(bitmap_argb32 dest, bitmap_argb32 source, render_color color, bool force = false) { rendutil_global.render_resample_argb_bitmap_hq(dest, source, color, force); }
        public static void render_load_msdib(out bitmap_argb32 bitmap, util.core_file file) { rendutil_global.render_load_msdib(out bitmap, file); }
        public static void render_load_jpeg(out bitmap_argb32 bitmap, util.core_file file) { rendutil_global.render_load_jpeg(out bitmap, file); }
        public static bool render_load_png(out bitmap_argb32 bitmap, util.core_file file, bool load_as_alpha_to_existing = false) { return rendutil_global.render_load_png(out bitmap, file, load_as_alpha_to_existing); }
        public static int orientation_add(int orientation1, int orientation2) { return rendutil_global.orientation_add(orientation1, orientation2); }


        // rescap
        public static double RES_K(double res) { return rescap_global.RES_K(res); }
        public static double RES_M(double res) { return rescap_global.RES_M(res); }
        public static double CAP_U(double cap) { return rescap_global.CAP_U(cap); }
        public static double CAP_N(double cap) { return rescap_global.CAP_N(cap); }
        public static double RES_VOLTAGE_DIVIDER(double r1, double r2) { return rescap_global.RES_VOLTAGE_DIVIDER(r1, r2); }
        public static double RES_2_PARALLEL(double r1, double r2) { return rescap_global.RES_2_PARALLEL(r1, r2); }
        public static double RES_3_PARALLEL(double r1, double r2, double r3) { return rescap_global.RES_3_PARALLEL(r1, r2, r3); }


        // resnet
        public const u32 RES_NET_AMP_DARLINGTON = resnet_global.RES_NET_AMP_DARLINGTON;
        public const u32 RES_NET_AMP_EMITTER = resnet_global.RES_NET_AMP_EMITTER;
        public const u32 RES_NET_VCC_5V = resnet_global.RES_NET_VCC_5V;
        public const u32 RES_NET_VBIAS_5V = resnet_global.RES_NET_VBIAS_5V;
        public const u32 RES_NET_VBIAS_TTL = resnet_global.RES_NET_VBIAS_TTL;
        public const u32 RES_NET_VIN_VCC = resnet_global.RES_NET_VIN_VCC;
        public const u32 RES_NET_VIN_TTL_OUT = resnet_global.RES_NET_VIN_TTL_OUT;
        public const u32 RES_NET_MONITOR_SANYO_EZV20 = resnet_global.RES_NET_MONITOR_SANYO_EZV20;
        public const u32 RES_NET_VIN_MB7052 = resnet_global.RES_NET_VIN_MB7052;
        public static int compute_res_net(int inputs, int channel, res_net_info di) { return resnet_global.compute_res_net(inputs, channel, di); }
        public static void compute_res_net_all(out std.vector<rgb_t> rgb, Pointer<u8> prom, res_net_decode_info rdi, res_net_info di) { resnet_global.compute_res_net_all(out rgb, prom, rdi, di); }
        public static double compute_resistor_weights(int minval, int maxval, double scaler, int count_1, int [] resistances_1, out double [] weights_1, int pulldown_1, int pullup_1, int count_2, int [] resistances_2, out double [] weights_2, int pulldown_2, int pullup_2, int count_3, int [] resistances_3, out double [] weights_3, int pulldown_3, int pullup_3 ) { return resnet_global.compute_resistor_weights(minval, maxval, scaler, count_1, resistances_1, out weights_1, pulldown_1, pullup_1, count_2, resistances_2, out weights_2, pulldown_2, pullup_2, count_3, resistances_3, out weights_3, pulldown_3, pullup_3); }
        public static int combine_weights(double [] tab, int w0, int w1, int w2) { return resnet_global.combine_weights(tab, w0, w1, w2); }
        public static int combine_weights(double [] tab, int w0, int w1) { return resnet_global.combine_weights(tab, w0, w1); }


        // romentry
        public const           UInt32 ROMREGION_INVERT = romentry_global.ROMREGION_INVERT;
        public static readonly UInt32 ROMREGION_ERASE00 = romentry_global.ROMREGION_ERASE00;
        public static readonly UInt32 ROMREGION_ERASEFF = romentry_global.ROMREGION_ERASEFF;
        public static tiny_rom_entry ROM_END { get { return romentry_global.ROM_END; } }
        public static tiny_rom_entry ROM_REGION(UInt32 length, string tag, UInt32 flags) { return romentry_global.ROM_REGION(length, tag, flags); }
        public static tiny_rom_entry ROM_LOAD(string name, UInt32 offset, UInt32 length, string hash) { return romentry_global.ROM_LOAD(name, offset, length, hash); }
        public static tiny_rom_entry ROM_CONTINUE(u32 offset, u32 length) { return romentry_global.ROM_CONTINUE(offset, length); }
        public static tiny_rom_entry ROM_FILL(UInt32 offset, UInt32 length, byte value) { return romentry_global.ROM_FILL(offset, length, value); }
        public static tiny_rom_entry ROM_LOAD16_BYTE(string name, u32 offset, u32 length, string hash) { return romentry_global.ROM_LOAD16_BYTE(name, offset, length, hash); }
        public static tiny_rom_entry ROM_RELOAD(UInt32 offset, UInt32 length) { return romentry_global.ROM_RELOAD(offset, length); }


        // romload
        public static bool ROMENTRY_ISFILE(rom_entry_interface r) { return romload_global.ROMENTRY_ISFILE(r); }
        public static bool ROMENTRY_ISREGION(rom_entry_interface r) { return romload_global.ROMENTRY_ISREGION(r); }
        public static bool ROMENTRY_ISEND(rom_entry_interface r) { return romload_global.ROMENTRY_ISEND(r); }
        public static bool ROMENTRY_ISSYSTEM_BIOS(rom_entry_interface r) { return romload_global.ROMENTRY_ISSYSTEM_BIOS(r); }
        public static bool ROMENTRY_ISDEFAULT_BIOS(rom_entry_interface r) { return romload_global.ROMENTRY_ISDEFAULT_BIOS(r); }
        public static bool ROMREGION_ISROMDATA(rom_entry_interface r) { return romload_global.ROMREGION_ISROMDATA(r); }
        public static bool ROMREGION_ISDISKDATA(rom_entry_interface r) { return romload_global.ROMREGION_ISDISKDATA(r); }
        public static string ROM_GETNAME(rom_entry_interface r) { return romload_global.ROM_GETNAME(r); }
        public static bool ROM_ISOPTIONAL(rom_entry_interface r) { return romload_global.ROM_ISOPTIONAL(r); }
        public static UInt32 ROM_GETBIOSFLAGS(rom_entry_interface r) { return romload_global.ROM_GETBIOSFLAGS(r); }
        public static Pointer<rom_entry> rom_first_region(device_t device) { return romload_global.rom_first_region(device); }
        public static Pointer<rom_entry> rom_first_region(Pointer<rom_entry> romp) { return romload_global.rom_first_region(romp); }
        public static Pointer<rom_entry> rom_first_file(Pointer<rom_entry> rompIn) { return romload_global.rom_first_file(rompIn); }
        public static u32 rom_file_size(Pointer<rom_entry> rompIn) { return romload_global.rom_file_size(rompIn); }
        public static std.vector<rom_entry> rom_build_entries(Pointer<tiny_rom_entry> tinyentries) { return romload_global.rom_build_entries(tinyentries); }


        // screen
        public const screen_type_enum SCREEN_TYPE_RASTER = screen_type_enum.SCREEN_TYPE_RASTER;
        public const screen_type_enum SCREEN_TYPE_VECTOR = screen_type_enum.SCREEN_TYPE_VECTOR;
        public const screen_type_enum SCREEN_TYPE_LCD = screen_type_enum.SCREEN_TYPE_LCD;
        public const screen_type_enum SCREEN_TYPE_SVG = screen_type_enum.SCREEN_TYPE_SVG;
        public const screen_type_enum SCREEN_TYPE_INVALID = screen_type_enum.SCREEN_TYPE_INVALID;
        public static screen_device SCREEN(machine_config mconfig, string tag, screen_type_enum type)
        {
            var device = emu.detail.device_type_impl.op<screen_device>(mconfig, tag, screen_device.SCREEN, 0);
            device.screen_device_after_ctor(type);
            return device;
        }
        public static screen_device SCREEN<bool_Required>(machine_config mconfig, device_finder<screen_device, bool_Required> finder, screen_type_enum type)
            where bool_Required : bool_const, new()
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, screen_device.SCREEN, 0);
            device.screen_device_after_ctor(type);
            return device;
        }


        // slapstic
        public static atari_slapstic_device SLAPSTIC<bool_Required>(machine_config mconfig, device_finder<atari_slapstic_device, bool_Required> finder, int chipnum)
            where bool_Required : bool_const, new()
        {
            var device = emu.detail.device_type_impl.op<atari_slapstic_device, bool_Required>(mconfig, finder, atari_slapstic_device.SLAPSTIC, 0);
            device.atari_slapstic_device_after_ctor(chipnum);
            return device;
        }


        // speaker
        public static speaker_device SPEAKER(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<speaker_device>(mconfig, tag, speaker_device.SPEAKER, 0); }


        // starfield
        public static starfield_05xx_device STARFIELD_05XX<bool_Required>(machine_config mconfig, device_finder<starfield_05xx_device, bool_Required> finder, uint32_t clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, starfield_05xx_device.STARFIELD_05XX, clock); }


        // t11
        public static t11_device T11<bool_Required>(machine_config mconfig, device_finder<t11_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, t11_device.T11, clock); }


        // taitosjsec
        public static taito_sj_security_mcu_device TAITO_SJ_SECURITY_MCU<bool_Required>(machine_config mconfig, device_finder<taito_sj_security_mcu_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, taito_sj_security_mcu_device.TAITO_SJ_SECURITY_MCU, clock); }


        // tilemap
        public const u32 TILEMAP_DRAW_CATEGORY_MASK = tilemap_global.TILEMAP_DRAW_CATEGORY_MASK;
        public const u32 TILEMAP_DRAW_OPAQUE = tilemap_global.TILEMAP_DRAW_OPAQUE;
        public const u32 TILEMAP_DRAW_ALL_CATEGORIES = tilemap_global.TILEMAP_DRAW_ALL_CATEGORIES;
        public const u8 TILE_FLIPX = tilemap_global.TILE_FLIPX;
        public const u8 TILE_FORCE_LAYER0 = tilemap_global.TILE_FORCE_LAYER0;
        public const u32 TILEMAP_FLIPX = tilemap_global.TILEMAP_FLIPX;
        public const u32 TILEMAP_FLIPY = tilemap_global.TILEMAP_FLIPY;
        public static u8 TILE_FLIPYX(int YX) { return tilemap_global.TILE_FLIPYX(YX); }
        public static tilemap_device TILEMAP<bool_Required>(machine_config mconfig, device_finder<tilemap_device, bool_Required> finder, string gfxtag, int entrybytes, u16 tilewidth, u16 tileheight, tilemap_standard_mapper mapper, u32 columns, u32 rows)
            where bool_Required : bool_const, new()
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, tilemap_device.TILEMAP, 0);
            device.tilemap_device_after_ctor(gfxtag, entrybytes, tilewidth, tileheight, mapper, columns, rows);
            return device;
        }
        public static tilemap_device TILEMAP<bool_Required>(machine_config mconfig, device_finder<tilemap_device, bool_Required> finder, string gfxtag, int entrybytes, u16 tilewidth, u16 tileheight, tilemap_standard_mapper mapper, u32 columns, u32 rows, pen_t transpen)
            where bool_Required : bool_const, new()
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, tilemap_device.TILEMAP, 0);
            device.tilemap_device_after_ctor(gfxtag, entrybytes, tilewidth, tileheight, mapper, columns, rows, transpen);
            return device;
        }


        // timer
        public static timer_device TIMER(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<timer_device>(mconfig, tag, timer_device.TIMER, clock); }


        // tms5220
        public static tms5220c_device TMS5220C<bool_Required>(machine_config mconfig, device_finder<tms5220c_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, tms5220c_device.TMS5220C, clock); }


        // ui
        public const float UI_MAX_FONT_HEIGHT = ui_global.UI_MAX_FONT_HEIGHT;
        public const float UI_LINE_WIDTH = ui_global.UI_LINE_WIDTH;
        public static readonly rgb_t UI_GREEN_COLOR = ui_global.UI_GREEN_COLOR;
        public static readonly rgb_t UI_YELLOW_COLOR = ui_global.UI_YELLOW_COLOR;
        public static readonly rgb_t UI_RED_COLOR = ui_global.UI_RED_COLOR;
        public const uint32_t UI_HANDLER_CANCEL = ui_global.UI_HANDLER_CANCEL;


        // watchdog
        public static watchdog_timer_device WATCHDOG_TIMER(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<watchdog_timer_device>(mconfig, tag, watchdog_timer_device.WATCHDOG_TIMER, 0); }
        public static watchdog_timer_device WATCHDOG_TIMER<bool_Required>(machine_config mconfig, device_finder<watchdog_timer_device, bool_Required> finder) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, watchdog_timer_device.WATCHDOG_TIMER, 0); }


        // ym2151
        public static ym2151_device YM2151<bool_Required>(machine_config mconfig, device_finder<ym2151_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, ym2151_device.YM2151, clock); }


        // z80
        public static cpu_device Z80<bool_Required>(machine_config mconfig, device_finder<cpu_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, z80_device.Z80, clock); }


        // c++
        public static int sizeof_(object value)
        {
            // alternative to System.Runtime.InteropServices.Marshal.SizeOf(typeof(NativeType)); ?

            switch (value)
            {
                case sbyte  t: return 1;
                case byte   t: return 1;
                case Int16  t: return 2;
                case UInt16 t: return 2;
                case Int32  t: return 4;
                case UInt32 t: return 4;
                case Int64  t: return 8;
                case UInt64 t: return 8;
                case Type   t:
                    if      ((Type)value == typeof(sbyte))  return 1;
                    else if ((Type)value == typeof(byte))   return 1;
                    else if ((Type)value == typeof(Int16))  return 2;
                    else if ((Type)value == typeof(UInt16)) return 2;
                    else if ((Type)value == typeof(Int32))  return 4;
                    else if ((Type)value == typeof(UInt32)) return 4;
                    else if ((Type)value == typeof(Int64))  return 8;
                    else if ((Type)value == typeof(UInt64)) return 8;
                    else throw new emu_unimplemented();
                default: throw new emu_unimplemented();
            }
        }


        // c++ cfloat  - https://www.johndcook.com/blog/2012/01/05/double-epsilon-dbl_epsilon/
        public const double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */


        // c++ cmath
        public const double M_PI = Math.PI;


        public const size_t npos = size_t.MaxValue;
    }


    public static class std
    {
        public static size_t size(string x) { return (size_t)x.Length; }
        public static size_t size<T>(T [] x) { return (size_t)x.Length; }
        public static size_t size<T>(std.vector<T> x) { return (size_t)x.Count; }


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
        public static int clamp(int v, int lo, int hi) { return ExtensionIComparable.Clamp(v, lo, hi); }
        public static float clamp(float v, float lo, float hi) { return ExtensionIComparable.Clamp(v, lo, hi); }
        public static double clamp(double v, double lo, double hi) { return ExtensionIComparable.Clamp(v, lo, hi); }
        public static void fill<T>(MemoryContainer<T> destination, T value) { std.memset(destination, value); }
        public static void fill<T>(IList<T> destination, T value) { std.memset(destination, value); }
        public static void fill<T>(IList<T> destination, Func<T> value) { std.memset(destination, value); }
        public static void fill_n<T>(MemoryContainer<T> destination, size_t count, T value) { std.memset(destination, value, count); }
        public static void fill_n<T>(Pointer<T> destination, size_t count, T value) { std.memset(destination, value, count); }
        public static void fill_n(PointerU16 destination, size_t count, UInt16 value) { std.memset(destination, value, count); }
        public static void fill_n(PointerU32 destination, size_t count, UInt32 value) { std.memset(destination, value, count); }
        public static void fill_n(PointerU64 destination, size_t count, UInt64 value) { std.memset(destination, value, count); }
        public static void fill_n<T>(IList<T> destination, size_t count, T value) { std.memset(destination, value, count); }
        public static T find_if<T>(IEnumerable<T> list, Func<T, bool> pred) { foreach (var item in list) { if (pred(item)) return item; } return default;  }
        public static int lower_bound<C, V>(C collection, V value) where C : IList<V> where V : IComparable<V> { return lower_bound<C, V, V>(collection, value, (i, v) => { return i.CompareTo(v) < 0; }); }
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
        // std::min/max behaves differently than Math.Min/Max with respect to NaN
        // https://docs.microsoft.com/en-us/dotnet/api/system.math.min?view=net-5.0#System_Math_Min_System_Single_System_Single_
        // https://stackoverflow.com/a/39919244
        public static int max(int a, int b) { return Math.Max(a, b); }
        public static UInt32 max(UInt32 a, UInt32 b) { return Math.Max(a, b); }
        public static Int64 max(Int64 a, Int64 b) { return Math.Max(a, b); }
        public static UInt64 max(UInt64 a, UInt64 b) { return Math.Max(a, b); }
        public static float max(float a, float b) { if (float.IsNaN(a) || float.IsNaN(b)) return a; else return Math.Max(a, b); }
        public static double max(double a, double b) { if (double.IsNaN(a) || double.IsNaN(b)) return a; else return Math.Max(a, b); }
        public static attotime max(attotime a, attotime b) { return attotime.Max(a, b); }
        public static netlist_time max(netlist_time a, netlist_time b) { return netlist_time.Max(a, b); }
        public static int min(int a, int b) { return Math.Min(a, b); }
        public static UInt32 min(UInt32 a, UInt32 b) { return Math.Min(a, b); }
        public static Int64 min(Int64 a, Int64 b) { return Math.Min(a, b); }
        public static UInt64 min(UInt64 a, UInt64 b) { return Math.Min(a, b); }
        public static float min(float a, float b) { if (float.IsNaN(a) || float.IsNaN(b)) return a; else return Math.Min(a, b); }
        public static double min(double a, double b) { if (double.IsNaN(a) || double.IsNaN(b)) return a; else return Math.Min(a, b); }
        public static attotime min(attotime a, attotime b) { return attotime.Min(a, b); }
        public static void sort<T>(MemoryContainer<T> list, Comparison<T> pred) { list.Sort(pred); }


        // c++ cmath
        public static int abs(int arg) { return Math.Abs(arg); }
        public static float abs(float arg) { return Math.Abs(arg); }
        public static double abs(double arg) { return Math.Abs(arg); }
        public static float atan(float arg) { return (float)Math.Atan(arg); }
        public static double atan(double arg) { return Math.Atan(arg); }
        public static float cos(float arg) { return (float)Math.Cos(arg); }
        public static double cos(double arg) { return Math.Cos(arg); }
        public static float exp(float x) { return (float)Math.Exp(x); }
        public static double exp(double x) { return Math.Exp(x); }
        public static float fabs(float arg) { return Math.Abs(arg); }
        public static double fabs(double arg) { return Math.Abs(arg); }
        public static float fabsf(float arg) { return Math.Abs(arg); }
        public static float floor(float arg) { return (float)Math.Floor(arg); }
        public static double floor(double arg) { return Math.Floor(arg); }
        public static float floorf(float arg) { return (float)Math.Floor(arg); }
        public static bool isnan(float arg) { return Single.IsNaN(arg); }
        public static bool isnan(double arg) { return Double.IsNaN(arg); }
        public static float log(float arg) { return (float)Math.Log(arg); }
        public static double log(double arg) { return Math.Log(arg); }
        public static float pow(float base_, float exponent) { return (float)Math.Pow(base_, exponent); }
        public static double pow(double base_, double exponent) { return Math.Pow(base_, exponent); }
        public static int lround(double x) { return (int)Math.Round(x, MidpointRounding.AwayFromZero); }
        public static float sin(float arg) { return (float)Math.Sin(arg); }
        public static double sin(double arg) { return Math.Sin(arg); }
        public static float sqrt(float arg) { return (float)Math.Sqrt(arg); }
        public static double sqrt(double arg) { return Math.Sqrt(arg); }
        public static float tan(float arg) { return (float)Math.Tan(arg); }
        public static double tan(double arg) { return Math.Tan(arg); }
        public static float trunc(float arg) { return (float)Math.Truncate(arg); }
        public static double trunc(double arg) { return Math.Truncate(arg); }


        // c++ cstdlib
        public static void abort() { terminate(); }
        public static int atoi(string str) { return Convert.ToInt32(str); }
        public static string getenv(string env_var) { return System.Environment.GetEnvironmentVariable(env_var); }
        public static UInt64 strtoul(string str, string endptr, int base_) { return Convert.ToUInt64(str, 16); }


        // c++ cstring
        public static int memcmp<T>(MemoryContainer<T> ptr1, MemoryContainer<T> ptr2, size_t num) { return ptr1.CompareTo(ptr2, (int)num) ? 0 : 1; }  //  const void * ptr1, const void * ptr2, size_t num
        public static int memcmp<T>(Pointer<T> ptr1, Pointer<T> ptr2, size_t num) { return ptr1.CompareTo(ptr2, (int)num) ? 0 : 1; }  //  const void * ptr1, const void * ptr2, size_t num
        public static void memcpy<T>(MemoryContainer<T> destination, MemoryContainer<T> source, size_t num) { source.CopyTo(0, destination, 0, (int)num); }  //void * destination, const void * source, size_t num );
        public static void memcpy<T>(Pointer<T> destination, Pointer<T> source, size_t num) { source.CopyTo(0, destination, 0, (int)num); }  //void * destination, const void * source, size_t num );
        public static void memset<T>(MemoryContainer<T> destination, T value) { destination.Fill(value); }
        public static void memset<T>(MemoryContainer<T> destination, T value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset<T>(Pointer<T> destination, T value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU16 destination, UInt16 value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU32 destination, UInt32 value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU64 destination, UInt64 value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset<T>(IList<T> destination, T value) { memset(destination, value, (size_t)destination.Count); }
        public static void memset<T>(IList<T> destination, T value, size_t num) { for (int i = 0; i < (int)num; i++) destination[i] = value; }
        public static void memset<T>(IList<T> destination, Func<T> value) { memset(destination, value, (size_t)destination.Count); }
        public static void memset<T>(IList<T> destination, Func<T> value, size_t num) { for (int i = 0; i < (int)num; i++) destination[i] = value(); }
        public static void memset<T>(T [,] destination, T value) { for (int i = 0; i < destination.GetLength(0); i++) for (int j = 0; j < destination.GetLength(1); j++) destination[i, j] = value; }
        public static int strchr(string str, char character) { return str.IndexOf(character); }
        public static int strcmp(string str1, string str2) { return string.Compare(str1, str2); }
        public static size_t strlen(string str) { return (size_t)str.Length; }
        public static int strncmp(string str1, string str2, size_t num) { return string.Compare(str1, 0, str2, 0, (int)num); }
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


        // c++ numeric
        public static UInt32 gcd(UInt32 a, UInt32 b)
        {
            return b != 0 ? gcd(b, a % b) : a;
        }


        // c++ utility
        public static void swap<T>(ref T val1, ref T val2)
        {
            g.assert(typeof(T).GetTypeInfo().IsValueType);

            T temp = val1;
            val1 = val2;
            val2 = temp;
        }

        public static std.pair<T, V> make_pair<T, V>(T t, V v) { return new std.pair<T, V>(t, v); }


        // c++ array
        public class array<T, size_t_N> : IList<T>
            where size_t_N : u64_const, new()
        {
            protected static readonly size_t N = new size_t_N().value;


            MemoryContainer<T> m_data = new MemoryContainer<T>((int)N, true);


            public array() { }
            public array(params T [] args)
            {
                if (args.Length != (int)N)
                    throw new emu_fatalerror("array() parameter count doen't match size. Provided: {0}, Expected: {1}", args.Length, N);

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

                if (ReferenceEquals(lhs, default) || ReferenceEquals(rhs, default))
                    return false;

                if (lhs.size() != rhs.size())
                    return false;

                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                for (size_t i = 0; i < lhs.size(); i++)
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
            public T this[UInt64 index] { get { return m_data[index]; } set { m_data[index] = value; } }


            public size_t size() { return (size_t)Count; }
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
            public size_t size() { return (size_t)m_list.Count; }
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
            public size_t size() { return (size_t)m_dictionary.Count; }
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


        // c++ stdexcept
        public class out_of_range : ArgumentOutOfRangeException
        {
            public out_of_range() : base() { }
            public out_of_range(string paramName) : base(paramName) { }
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
            public size_t size() { return (size_t)m_dictionary.Count; }
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
            public vector(s32 count, T data = default) : base(count) { resize((size_t)count, data); }
            public vector(size_t count, T data = default) : base((int)count) { resize(count, data); }
            public vector(IEnumerable<T> collection) : base(collection) { }


            // std::vector functions
            public T front() { return this[0]; }
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
            public void resize(size_t count, T data = default) { Resize((int)count, data); }
            public void reserve(size_t value) { Capacity = (int)value; }
            public size_t size() { return (size_t)Count; }
        }
    }


    // this wraps a Memory<T> struct so that bulk operations can be done efficiently
    // std.vector and MemoryU8 derive from this.
    public class MemoryContainer<T> : IList<T>
    {
        class MemoryContainerEnumerator : IEnumerator<T>
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
        public virtual IEnumerator<T> GetEnumerator() { return new MemoryContainerEnumerator(this); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }


        // List

        public virtual int Capacity { get { return m_data.Length; } set { } }
        public virtual void CopyTo(int index, T[] array, int arrayIndex, int count) { CopyTo(index, new Span<T>(array, arrayIndex, count), count); }
        public virtual int FindIndex(int startIndex, int count, Predicate<T> match) { return Array.FindIndex(m_data, startIndex, count, match); }
        public virtual int FindIndex(int startIndex, Predicate<T> match) { return Array.FindIndex(m_data, startIndex, match); }
        public virtual int FindIndex(Predicate<T> match) { return Array.FindIndex(m_data, match); }
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


        // UInt64 helper
        public virtual T this[UInt64 index] { get { return m_data[index]; } set { m_data[index] = value; } }


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
            g.assert(typeof(T).GetTypeInfo().IsValueType ? true : (data == null || data.Equals(default)) ? true : false);  // this function doesn't do what you'd expect for ref classes since it doesn't new() for each item.  Manually Add() in this case.
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
        public virtual T this[UInt64 i] { get { return m_memory[m_offset + (int)i]; } set { m_memory[m_offset + (int)i] = value; } }


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


    // this class holds a Pointer reference so that if the pointer changes, this class will track it.
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
        public new UInt16 this[UInt64 i] { get { return this.GetUInt16((int)i); } set { this.SetUInt16((int)i, value); } }

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
        public new UInt32 this[UInt64 i] { get { return this.GetUInt32((int)i); } set { this.SetUInt32((int)i, value); } }

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
        public new UInt64 this[UInt64 i] { get { return this.GetUInt64((int)i); } set { this.SetUInt64((int)i, value); } }

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
            g.assert_slow(offset8 < container.Count);
            var span = container.memory.Slice(offset8, 2).Span;
            BinaryPrimitives.WriteUInt16LittleEndian(span, value);
        }

        public static void SetUInt32Offs8(this MemoryContainer<byte> container, int offset8, UInt32 value)
        {
            g.assert_slow(offset8 < container.Count);
            var span = container.memory.Slice(offset8, 4).Span;
            BinaryPrimitives.WriteUInt32LittleEndian(span, value);
        }

        public static void SetUInt64Offs8(this MemoryContainer<byte> container, int offset8, UInt64 value)
        {
            g.assert_slow(offset8 < container.Count);
            var span = container.memory.Slice(offset8, 8).Span;
            BinaryPrimitives.WriteUInt64LittleEndian(span, value);
        }

        public static UInt16 GetUInt16(this MemoryContainer<byte> container, int offset16 = 0) { return container.GetUInt16Offs8(offset16 << 1); }
        public static UInt32 GetUInt32(this MemoryContainer<byte> container, int offset32 = 0) { return container.GetUInt32Offs8(offset32 << 2); }
        public static UInt64 GetUInt64(this MemoryContainer<byte> container, int offset64 = 0) { return container.GetUInt64Offs8(offset64 << 3); }

        public static UInt16 GetUInt16Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            g.assert_slow(offset8 < container.Count);
            var span = container.memory.Slice(offset8, 2).Span;
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
        }

        public static UInt32 GetUInt32Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            g.assert_slow(offset8 < container.Count);
            var span = container.memory.Slice(offset8, 4).Span;
            return BinaryPrimitives.ReadUInt32LittleEndian(span);
        }

        public static UInt64 GetUInt64Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            g.assert_slow(offset8 < container.Count);
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


    // a flexible primitive type, used when the return is unknown (eg with templates)
    // the size is set at construction.  See uX and devcb_value
    public struct FlexPrim
    {
        Type m_type;

        // use u128 here so that we don't need so much checking
        u64 m_value_u;
        s64 m_value_s;


        public FlexPrim(Type type, u64 value = 0U)
        {
            m_type = type;
            m_value_s = (s64)value;
            m_value_u = value;
        }

        public FlexPrim(Type type, u32 value = 0U) : this(type, (u64)value) { }

        public FlexPrim(Type type, s64 value = 0)
        {
            m_type = type;
            m_value_s = value;
            m_value_u = (u64)value;
        }

        public FlexPrim(Type type, FlexPrim other)
        {
            // convert to specified type
            if (IsUnsigned(other.m_type))
            {
                m_type = type;
                m_value_s = (s64)other.Get_u;
                m_value_u = other.Get_u;
            }
            else
            {
                m_type = type;
                m_value_s = other.Get_s;
                m_value_u = (u64)other.Get_s;
            }
        }

        public FlexPrim(FlexPrim other) : this(other.m_type, other) { }

        public FlexPrim(int width, u64 value = 0) : this(WidthToType(width), value) { }
        public FlexPrim(int width, FlexPrim other) : this(WidthToType(width), other) { }


        public Type type { get { return m_type; } }
        public int width { get { return TypeToWidth(type); } }

        // cast to appropriate type, but return largest type
        public u64 Get_u
        {
            get
            {
                if      (m_type == typeof(u8))  return (u8)m_value_u;
                else if (m_type == typeof(u16)) return (u16)m_value_u;
                else if (m_type == typeof(u32)) return (u32)m_value_u;
                else if (m_type == typeof(u64)) return (u64)m_value_u;
                else if (m_type == typeof(s8))  return (u64)(s8)m_value_s;
                else if (m_type == typeof(s16)) return (u64)(s16)m_value_s;
                else if (m_type == typeof(s32)) return (u64)(s32)m_value_s;
                else if (m_type == typeof(s64)) return (u64)(s64)m_value_s;
                else throw new emu_unimplemented();
            }
        }

        public s64 Get_s
        {
            get
            {
                if      (m_type == typeof(u8))  return (s64)(u8)m_value_u;
                else if (m_type == typeof(u16)) return (s64)(u16)m_value_u;
                else if (m_type == typeof(u32)) return (s64)(u32)m_value_u;
                else if (m_type == typeof(u64)) return (s64)(u64)m_value_u;
                else if (m_type == typeof(s8))  return (s8)m_value_s;
                else if (m_type == typeof(s16)) return (s16)m_value_s;
                else if (m_type == typeof(s32)) return (s32)m_value_s;
                else if (m_type == typeof(s64)) return (s64)m_value_s;
                else throw new emu_unimplemented();
            }
        }


        // cast to appropriate type, and return appropriate type
        public u8 u8 { get { return (u8)Get_u; } }
        public u16 u16 { get { return (u16)Get_u; } }
        public u32 u32 { get { return (u32)Get_u; } }
        public u64 u64 { get { return (u64)Get_u; } }
        public s8 s8 { get { return (s8)Get_s; } }
        public s16 s16 { get { return (s16)Get_s; } }
        public s32 s32 { get { return (s32)Get_s; } }
        public s64 s64 { get { return (s64)Get_s; } }


        public bool IsUnsigned() { return IsUnsigned(m_type); }
        public int TypeToWidth() { return TypeToWidth(m_type); }


        public s32 sizeof_() { return g.sizeof_(m_type); }


        public override bool Equals(Object obj)
        {
            if (obj == null || base.GetType() != obj.GetType()) return false;
            return this == (FlexPrim)obj;
        }
        public override int GetHashCode() { return m_type.GetHashCode() ^ m_value_u.GetHashCode() ^ m_value_s.GetHashCode(); }


        public override string ToString() { return string.Format("type: {0} Get_u: {1} Get_s: {2}", type, Get_u, Get_s); }


        public static Type make_unsigned(Type type)
        {
            if      (type == typeof(u8)) return typeof(u8);
            else if (type == typeof(u32)) return typeof(u32);
            else if (type == typeof(s32)) return typeof(u32);
            else throw new emu_unimplemented();
        }


        public static bool operator ==(FlexPrim left, FlexPrim right) { if (IsUnsigned(left.m_type)) return left.Get_u == right.Get_u; else return left.Get_s == right.Get_s; }
        public static bool operator ==(FlexPrim left, u64 right) { g.assert(IsUnsigned(left)); return left.Get_u == right; }
        public static bool operator ==(FlexPrim left, u32 right) { g.assert(IsUnsigned(left)); return left.Get_u == right; }
        public static bool operator ==(FlexPrim left, s64 right) { g.assert(!IsUnsigned(left)); return left.Get_s == right; }
        public static bool operator !=(FlexPrim left, FlexPrim right) { if (IsUnsigned(left.m_type)) return left.Get_u != right.Get_u; else return left.Get_s != right.Get_s; }
        public static bool operator !=(FlexPrim left, u64 right) { g.assert(IsUnsigned(left)); return left.Get_u != right; }
        public static bool operator !=(FlexPrim left, u32 right) { g.assert(IsUnsigned(left)); return left.Get_u != right; }
        public static bool operator !=(FlexPrim left, s64 right) { g.assert(!IsUnsigned(left)); return left.Get_s != right; }


        public static FlexPrim operator +(FlexPrim left, FlexPrim right) { if (IsUnsigned(left.m_type)) return new FlexPrim(left.m_type, left.Get_u + right.Get_u); else return new FlexPrim(left.m_type, left.Get_s + right.Get_s); }
        public static FlexPrim operator +(FlexPrim left, u64 right) { g.assert(IsUnsigned(left)); return new FlexPrim(left.m_type, left.Get_u + right); }
        public static FlexPrim operator +(FlexPrim left, s64 right) { g.assert(!IsUnsigned(left)); return new FlexPrim(left.m_type, left.Get_s + right); }

        public static FlexPrim operator >>(FlexPrim left, int right) { if (IsUnsigned(left.m_type)) return new FlexPrim(left.m_type, left.Get_u >> right); else return new FlexPrim(left.m_type, left.Get_s >> right); }
        public static FlexPrim operator <<(FlexPrim left, int right) { if (IsUnsigned(left.m_type)) return new FlexPrim(left.m_type, left.Get_u << right); else return new FlexPrim(left.m_type, left.Get_s << right); }
        public static FlexPrim operator &(FlexPrim left, FlexPrim right) { if (IsUnsigned(left.m_type)) return new FlexPrim(left.m_type, left.Get_u & right.Get_u); else return new FlexPrim(left.m_type, left.Get_s & right.Get_s); }
        public static FlexPrim operator &(FlexPrim left, u64 right) { g.assert(IsUnsigned(left)); return new FlexPrim(left.m_type, left.Get_u & right); }
        public static FlexPrim operator &(FlexPrim left, s64 right) { g.assert(!IsUnsigned(left)); return new FlexPrim(left.m_type, left.Get_s & right); }
        public static FlexPrim operator |(FlexPrim left, FlexPrim right) { if (IsUnsigned(left.m_type)) return new FlexPrim(left.m_type, left.Get_u | right.Get_u); else return new FlexPrim(left.m_type, left.Get_s | right.Get_s); }
        public static FlexPrim operator |(FlexPrim left, u64 right) { g.assert(IsUnsigned(left)); return new FlexPrim(left.m_type, left.Get_u | right); }
        public static FlexPrim operator |(FlexPrim left, s64 right) { g.assert(!IsUnsigned(left)); return new FlexPrim(left.m_type, left.Get_s | right); }
        public static FlexPrim operator ^(FlexPrim left, FlexPrim right) { if (IsUnsigned(left.m_type)) return new FlexPrim(left.m_type, left.Get_u ^ right.Get_u); else return new FlexPrim(left.m_type, left.Get_s ^ right.Get_s); }
        public static FlexPrim operator ^(FlexPrim left, u64 right) { g.assert(IsUnsigned(left)); return new FlexPrim(left.m_type, left.Get_u ^ right); }
        public static FlexPrim operator ^(FlexPrim left, s64 right) { g.assert(!IsUnsigned(left)); return new FlexPrim(left.m_type, left.Get_s ^ right); }

        public static FlexPrim operator ~(FlexPrim left) { if (IsUnsigned(left)) return new FlexPrim(left.m_type, ~left.Get_u); else return new FlexPrim(left.m_type, ~left.Get_s); }


        public static bool IsUnsigned(FlexPrim value) { return IsUnsigned(value.m_type); }

        public static bool IsUnsigned(Type type)
        {
            if (type == typeof(u8) ||
                type == typeof(u16) ||
                type == typeof(u32) ||
                type == typeof(u64))
                return true;
            else if (type == typeof(s8) ||
                     type == typeof(s16) ||
                     type == typeof(s32) ||
                     type == typeof(s64))
                return false;
            else
                throw new emu_unimplemented();
        }


        public static FlexPrim MaxValue(Type type) { return new FlexPrim(type, u64.MaxValue); }
        public static FlexPrim MaxValue(int width) { return new FlexPrim(width, u64.MaxValue); }


        public static Type WidthToType(int width)
        {
            switch (width)
            {
                case 0: return typeof(u8);
                case 1: return typeof(u16);
                case 2: return typeof(u32);
                case 3: return typeof(u64);
                default: throw new emu_unimplemented();
            }
        }

        public static int TypeToWidth(Type type)
        {
            if      (type == typeof(u8)) return 0;
            else if (type == typeof(u16)) return 1;
            else if (type == typeof(u32)) return 2;
            else if (type == typeof(u64)) return 3;
            else throw new emu_unimplemented();
        }


        public static s32 sizeof_(int width) { return g.sizeof_(WidthToType(width)); }
    }
}
