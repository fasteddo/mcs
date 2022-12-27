// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    public interface bool_const { bool value { get; } }
    public class bool_const_true : bool_const { public bool value { get { return true; } } }
    public class bool_const_false : bool_const { public bool value { get { return false; } } }

    public interface u8_const { Byte value { get; } }
    public class u8_const_0 : u8_const { public Byte value { get { return 0; } } }
    public class u8_const_1 : u8_const { public Byte value { get { return 1; } } }

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
    public class u32_const_9 : u32_const { public UInt32 value { get { return 9; } } }
    public class u32_const_10 : u32_const { public UInt32 value { get { return 10; } } }
    public class u32_const_11 : u32_const { public UInt32 value { get { return 11; } } }
    public class u32_const_12 : u32_const { public UInt32 value { get { return 12; } } }
    public class u32_const_16 : u32_const { public UInt32 value { get { return 16; } } }
    public class u32_const_20 : u32_const { public UInt32 value { get { return 20; } } }
    public class u32_const_32 : u32_const { public UInt32 value { get { return 32; } } }

    public interface u64_const { UInt64 value { get; } }
    public class u64_const_0 : u64_const { public UInt64 value { get { return 0; } } }
    public class u64_const_2 : u64_const { public UInt64 value { get { return 2; } } }
    public class u64_const_3 : u64_const { public UInt64 value { get { return 3; } } }
    public class u64_const_4 : u64_const { public UInt64 value { get { return 4; } } }
    public class u64_const_5 : u64_const { public UInt64 value { get { return 5; } } }
    public class u64_const_6 : u64_const { public UInt64 value { get { return 6; } } }
    public class u64_const_7 : u64_const { public UInt64 value { get { return 7; } } }
    public class u64_const_8 : u64_const { public UInt64 value { get { return 8; } } }
    public class u64_const_16 : u64_const { public UInt64 value { get { return 16; } } }
    public class u64_const_17 : u64_const { public UInt64 value { get { return 17; } } }
    public class u64_const_29 : u64_const { public UInt64 value { get { return 29; } } }
    public class u64_const_31 : u64_const { public UInt64 value { get { return 31; } } }
    public class u64_const_37 : u64_const { public UInt64 value { get { return 37; } } }
    public class u64_const_43 : u64_const { public UInt64 value { get { return 43; } } }
    public class u64_const_64 : u64_const { public UInt64 value { get { return 64; } } }
    public class u64_const_156 : u64_const { public UInt64 value { get { return 156; } } }
    public class u64_const_256 : u64_const { public UInt64 value { get { return 256; } } }
    public class u64_const_312 : u64_const { public UInt64 value { get { return 312; } } }
    public class u64_const_6364136223846793005 : u64_const { public UInt64 value { get { return 6364136223846793005; } } }
    public class u64_const_0x5555555555555555 : u64_const { public UInt64 value { get { return 0x5555555555555555; } } }
    public class u64_const_0x71d67fffeda60000 : u64_const { public UInt64 value { get { return 0x71d67fffeda60000; } } }
    public class u64_const_0xb5026f5aa96619e9 : u64_const { public UInt64 value { get { return 0xb5026f5aa96619e9; } } }
    public class u64_const_0xfff7eee000000000 : u64_const { public UInt64 value { get { return 0xfff7eee000000000; } } }
}
