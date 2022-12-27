// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
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

        public new UInt16 op { get { return this[0]; } set { this[0] = value; } }

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

        public new UInt32 op { get { return this[0]; } set { this[0] = value; } }

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

        public new UInt64 op { get { return this[0]; } set { this[0] = value; } }

        public void Fill(UInt64 value, int count) { Fill(value, 0, count); }
        public void Fill(UInt64 value, int start, int count) { for (int i = start; i < start + count; i++) this[i] = value; }
    }
}
