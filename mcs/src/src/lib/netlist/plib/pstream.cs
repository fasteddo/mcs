// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    // -----------------------------------------------------------------------------
    // pstream: things common to all streams
    // -----------------------------------------------------------------------------
    public abstract class pstream //: nocopyassignmove
    {
        //using pos_type = std::size_t;


        //static constexpr pos_type SEEK_EOF = static_cast<pos_type>(-1);


        UInt32 m_flags;


        public pstream(UInt32 flags) { m_flags = flags; }

        ~pstream() { }


        //bool seekable() const { return ((m_flags & FLAG_SEEKABLE) != 0); }

        //void seek(const pos_type n)
        //{
        //    return vseek(n);
        //}

        //pos_type tell()
        //{
        //    return vtell();
        //}


        //virtual void vseek(const pos_type n) = 0;
        //virtual pos_type vtell() = 0;

        //static constexpr unsigned FLAG_EOF = 0x01;
        //static constexpr unsigned FLAG_SEEKABLE = 0x04;

        //void set_flag(const unsigned flag)
        //{
        //    m_flags |= flag;
        //}
        //void clear_flag(const unsigned flag)
        //{
        //    m_flags &= ~flag;
        //}
        //unsigned flags() const { return m_flags; }
    }


    // -----------------------------------------------------------------------------
    // pistream: input stream
    // -----------------------------------------------------------------------------
    public class pistream : pstream
    {
        pistream(UInt32 flags) : base(flags) { }

        ~pistream() { }

        //bool eof() const { return ((flags() & FLAG_EOF) != 0); }

        //pos_type read(void *buf, const pos_type n)
        //{
        //    return vread(buf, n);
        //}

        /* read up to n bytes from stream */
        //virtual pos_type vread(void *buf, const pos_type n) = 0;
    }
}