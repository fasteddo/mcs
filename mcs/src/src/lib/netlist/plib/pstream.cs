// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    /// \brief a named istream pointer container
    ///
    /// This moveable object allows to pass istream unique pointers with
    /// information about the origin (filename). This is useful in error
    /// reporting where the source of the stream has to be logged.
    ///
    public class istream_uptr
    {
        std.istream m_strm;  //std::unique_ptr<std::istream> m_strm;
        string m_filename;


        public istream_uptr() { }

        public istream_uptr(std.istream strm, string filename)  //istream_uptr(std::unique_ptr<std::istream> &&strm, const pstring &filename)
        {
            m_strm = strm;
            m_filename = filename;
        }

        //istream_uptr(const istream_uptr &) = delete;
        //istream_uptr &operator=(const istream_uptr &) = delete;
        //istream_uptr(istream_uptr &&rhs)
        //{
        //    m_strm = std::move(rhs.m_strm);
        //    m_filename = std::move(rhs.m_filename);
        //}
        //istream_uptr &operator=(istream_uptr &&) /*noexcept*/ = delete;

        //~istream_uptr() = default;

        //std::istream * operator ->() noexcept { return m_strm.get(); }
        //std::istream & operator *() noexcept { return *m_strm; }
        //pstring filename() { return m_filename; }

        //bool empty() { return m_strm == nullptr; }

        // FIXME: workaround input context should accept stream_ptr

        //std::unique_ptr<std::istream> release_stream() { return std::move(m_strm); }
    }
}
