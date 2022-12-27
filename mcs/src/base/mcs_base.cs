// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using char32_t = System.UInt32;
using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;
using ioport_value = System.UInt32;  //typedef u32 ioport_value;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
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

using static mame.cpp_global;


namespace mame
{
    public class mcs_notimplemented : NotImplementedException
    {
        [DebuggerHidden]
        public mcs_notimplemented() : base("mcs not implemented") { }
    }


    public class mcs_fatal : Exception
    {
        [DebuggerHidden]
        public mcs_fatal() : base("mcs fatal exception.") { }
        [DebuggerHidden]
        public mcs_fatal(string format, params object [] args) : base(string.Format("mcs fatal exception: {0}", string.Format(format, args))) { }
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
