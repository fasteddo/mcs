// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Numerics;

using s8 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    // a flexible primitive type, used when the return is unknown (eg with templates)
    // the size is set at construction.  See uX and devcb_value
    public struct FlexPrim
    {
        Type m_type;

        // TODO - use u128 here once native type is available
        BigInteger m_value;


        public FlexPrim(Type type) : this(type, 0U) { }
        public FlexPrim(Type type, u32 value) : this(type, (u64)value) { }
        public FlexPrim(Type type, u64 value) : this(type, new BigInteger(value)) { }
        public FlexPrim(Type type, s64 value) : this(type, new BigInteger(value)) { }
        public FlexPrim(Type type, FlexPrim other) : this(type, other.Get) { }
        public FlexPrim(FlexPrim other) : this(other.m_type, other) { }
        public FlexPrim(int width) : this(width, 0U) { }
        public FlexPrim(int width, u64 value) : this(WidthToType(width), value) { }
        public FlexPrim(int width, FlexPrim other) : this(WidthToType(width), other) { }

        FlexPrim(Type type, BigInteger value)
        {
            m_type = type;
            m_value = value;
        }


        public Type type { get { return m_type; } }
        public int width { get { return TypeToWidth(type); } }

        // cast to appropriate type, but return largest type
        BigInteger Get
        {
            get
            {
                if      (m_type == typeof(u8))  return (u8)(m_value & u8.MaxValue);
                else if (m_type == typeof(u16)) return (u16)(m_value & u16.MaxValue);
                else if (m_type == typeof(u32)) return (u32)(m_value & u32.MaxValue);
                else if (m_type == typeof(u64)) return (u64)(m_value & u64.MaxValue);
                else if (m_type == typeof(s8))  return (s8)(m_value & s8.MaxValue);
                else if (m_type == typeof(s16)) return (s16)(m_value & s16.MaxValue);
                else if (m_type == typeof(s32)) return (s32)(m_value & s32.MaxValue);
                else if (m_type == typeof(s64)) return (s64)(m_value & s64.MaxValue);
                else throw new mcs_notimplemented();
            }
        }


        // cast to appropriate type, and return appropriate type
        public u8 u8 { get { return (u8)Get; } }
        public u16 u16 { get { return (u16)Get; } }
        public u32 u32 { get { return (u32)Get; } }
        public u64 u64 { get { return (u64)Get; } }
        public s8 s8 { get { return (s8)Get; } }
        public s16 s16 { get { return (s16)Get; } }
        public s32 s32 { get { return (s32)Get; } }
        public s64 s64 { get { return (s64)Get; } }


        public bool IsUnsigned() { return IsUnsigned(m_type); }
        public int TypeToWidth() { return TypeToWidth(m_type); }


        public override bool Equals(Object obj)
        {
            if (obj == null || base.GetType() != obj.GetType()) return false;
            return this == (FlexPrim)obj;
        }
        public override int GetHashCode() { return m_type.GetHashCode() ^ m_value.GetHashCode(); }


        public override string ToString() { return string.Format("type: {0} Get: {1}", type, Get); }


        public static Type make_unsigned(Type type)
        {
            if      (type == typeof(u8)) return typeof(u8);
            else if (type == typeof(u16)) return typeof(u16);
            else if (type == typeof(u32)) return typeof(u32);
            else if (type == typeof(s32)) return typeof(u32);
            else throw new mcs_notimplemented();
        }


        public static bool operator ==(FlexPrim left, FlexPrim right) { return left.Get == right.Get; }
        public static bool operator ==(FlexPrim left, u64 right) { return left.Get == right; }
        public static bool operator ==(FlexPrim left, u32 right) { return left.Get == right; }
        public static bool operator ==(FlexPrim left, s64 right) { return left.Get == right; }
        public static bool operator !=(FlexPrim left, FlexPrim right) { return left.Get != right.Get; }
        public static bool operator !=(FlexPrim left, u64 right) { return left.Get != right; }
        public static bool operator !=(FlexPrim left, u32 right) { return left.Get != right; }
        public static bool operator !=(FlexPrim left, s64 right) { return left.Get != right; }

        public static FlexPrim operator +(FlexPrim left, FlexPrim right) { return new FlexPrim(left.m_type, left.Get + right.Get); }
        public static FlexPrim operator +(FlexPrim left, u64 right) { return new FlexPrim(left.m_type, left.Get + right); }
        public static FlexPrim operator +(FlexPrim left, s64 right) { return new FlexPrim(left.m_type, left.Get + right); }

        public static FlexPrim operator >>(FlexPrim left, int right) { return new FlexPrim(left.m_type, left.Get >> right); }
        public static FlexPrim operator <<(FlexPrim left, int right) { return new FlexPrim(left.m_type, left.Get << right); }
        public static FlexPrim operator &(FlexPrim left, FlexPrim right) { return new FlexPrim(left.m_type, left.Get & right.Get); }
        public static FlexPrim operator &(FlexPrim left, u64 right) { return new FlexPrim(left.m_type, left.Get & right); }
        public static FlexPrim operator &(FlexPrim left, s64 right) { return new FlexPrim(left.m_type, left.Get & right); }
        public static FlexPrim operator |(FlexPrim left, FlexPrim right) { return new FlexPrim(left.m_type, left.Get | right.Get); }
        public static FlexPrim operator |(FlexPrim left, u64 right) { return new FlexPrim(left.m_type, left.Get | right); }
        public static FlexPrim operator |(FlexPrim left, s64 right) { return new FlexPrim(left.m_type, left.Get | right); }
        public static FlexPrim operator ^(FlexPrim left, FlexPrim right) { return new FlexPrim(left.m_type, left.Get ^ right.Get); }
        public static FlexPrim operator ^(FlexPrim left, u64 right) { return new FlexPrim(left.m_type, left.Get ^ right); }
        public static FlexPrim operator ^(FlexPrim left, s64 right) { return new FlexPrim(left.m_type, left.Get ^ right); }

        public static FlexPrim operator ~(FlexPrim left) { return new FlexPrim(left.m_type, ~left.Get); }


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
                throw new mcs_notimplemented();
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
                default: throw new mcs_notimplemented();
            }
        }

        public static int TypeToWidth(Type type)
        {
            if      (type == typeof(u8)) return 0;
            else if (type == typeof(u16)) return 1;
            else if (type == typeof(u32)) return 2;
            else if (type == typeof(u64)) return 3;
            else throw new mcs_notimplemented();
        }
    }
}
