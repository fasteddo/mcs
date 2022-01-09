// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;


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
        Stream m_strm;  //std::unique_ptr<std::istream> m_strm;
        string m_filename;


        public istream_uptr() { }

        public istream_uptr(Stream strm, string filename)  //istream_uptr(std::unique_ptr<std::istream> &&strm, const pstring &filename)
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
        public string filename() { return m_filename; }

        public bool empty() { return m_strm == null || m_strm.Length == 0; }

        // FIXME: workaround input context should accept stream_ptr

        public Stream release_stream() { return m_strm; }  //std::unique_ptr<std::istream> release_stream() { return std::move(m_strm); }
    }
}
