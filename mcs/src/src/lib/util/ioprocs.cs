// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;

using int64_t = System.Int64;
using PointerU8 = mame.Pointer<System.Byte>;
using size_t = System.UInt64;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.cpp_global;


namespace mame
{
    public static partial class util
    {
        /// \defgroup ioprocs Generic I/O interfaces
        /// \{

        /// \brief Interface to an input stream
        ///
        /// Represents a stream producing a sequence of bytes with no further
        /// structure.
        /// \sa write_stream read_write_stream random_read
        public interface read_stream
        {
            //using ptr = std::unique_ptr<read_stream>;

            //virtual ~read_stream() = default;

            /// \brief Read from the current position in the stream
            ///
            /// Reads up to the specified number of bytes from the stream into
            /// the supplied buffer.  May read less than the requested number of
            /// bytes if the end of the stream is reached or an error occurs.
            /// If the stream supports seeking, reading starts at the current
            /// position in the stream, and the current position is incremented
            /// by the number of bytes read.
            /// \param [out] buffer Destination buffer.  Must be large enough to
            ///   hold the requested number of bytes.
            /// \param [in] length Maximum number of bytes to read.
            /// \param [out] actual Number of bytes actually read.  Will always
            ///   be less than or equal to the requested length.
            /// \return An error condition if reading stopped due to an error.
            std.error_condition read(PointerU8 buffer, size_t length, out size_t actual);  //virtual std::error_condition read(void *buffer, std::size_t length, std::size_t &actual) noexcept = 0;

            Stream stream { get; }
        }


        /// \brief Interface to an output stream
        ///
        /// Represents a stream that accepts a sequence of bytes with no further
        /// structure.
        /// \sa read_stream read_write_stream random_write
        public interface write_stream
        {
            //using ptr = std::unique_ptr<write_stream>;

            //virtual ~write_stream() = default;

            /// \brief Finish writing data
            ///
            /// Performs any tasks necessary to finish writing data to the
            /// stream and guarantee that the written data can be read in its
            /// entirety.  Further writes may not be possible.
            /// \return An error condition if the operation fails.
            std.error_condition finalize();

            /// \brief Flush application-side caches
            ///
            /// Flushes any caches to the underlying stream.  Success does not
            /// guarantee that data has reached persistent storage.
            /// \return An error condition if flushing caches fails.
            std.error_condition flush();

            /// \brief Write at the current position in the stream
            ///
            /// Writes up to the specified number of bytes from the supplied
            /// buffer to the stream.  May write less than the requested number
            /// of bytes if an error occurs.  If the stream supports seeking,
            /// writing starts at the current position in the stream, and the
            /// current position is incremented by the number of bytes written.
            /// \param [in] buffer Buffer containing the data to write.  Must
            ///   contain at least the specified number of bytes.
            /// \param [in] length Number of bytes to write.
            /// \param [out] actual Number of bytes actually written.  Will
            ///   always be less than or equal to the requested length.
            /// \return An error condition if writing stopped due to an error.
            std.error_condition write(PointerU8 buffer, size_t length, out size_t actual);
        }


        /// \brief Interface to an I/O stream
        ///
        /// Represents an object that acts as both a source of and sink for byte
        /// sequences.
        /// \sa read_stream write_stream random_read_write
        public interface read_write_stream : read_stream, write_stream  //class read_write_stream : public virtual read_stream, public virtual write_stream
        {
        }


        /// \brief Interface to a byte sequence that supports seeking
        ///
        /// Provides an interface for controlling the position within a byte
        /// sequence that supports seeking.
        /// \sa random_read random_write random_read_write
        public interface random_access
        {
            /// \brief Set the position in the stream
            ///
            /// Sets the position for the next read or write operation.  It may
            /// be possible to set the position beyond the end of the stream.
            /// \param [in] offset The offset in bytes, relative to the position
            ///   specified by the whence parameter.
            /// \param [in] whence One of SEEK_SET, SEEK_CUR or SEEK_END, to
            ///   interpret the offset parameter relative to the beginning of
            ///   the stream, the current position in the stream, or the end of
            ///   the stream, respectively.
            /// \return An error condition of the operation failed.
            std.error_condition seek(int64_t offset, int whence);

            /// \brief Get the current position in the stream
            ///
            /// Gets the position in the stream for the next read or write
            /// operation.  The position may be beyond the end of the stream.
            /// \param [out] result The position in bytes relative to the
            ///   beginning of the stream.  Not valid if the operation fails.
            /// \return An error condition if the operation failed.
            std.error_condition tell(out uint64_t result);

            /// \brief Get the length of the stream
            ///
            /// Gets the current length of the stream.
            /// \param [out] result The length of the stream in bytes.  Not
            ///   valid if the operation fails.
            /// \return An error condtion if the operation failed.
            std.error_condition length(out uint64_t result);
        }


        /// \brief Interface to a random-access byte input sequence
        ///
        /// Provides an interface for reading from arbitrary positions within a
        /// byte sequence.  No further structure is provided.
        /// \sa read_stream random_write random_read_write
        public interface random_read : read_stream, random_access  //class random_read : public virtual read_stream, public virtual random_access
        {
            //using ptr = std::unique_ptr<random_read>;

            /// \brief Read from specified position
            ///
            /// Reads up to the specified number of bytes into the supplied
            /// buffer.  If seeking is supported, reading starts at the
            /// specified position and the current position is unaffected.  May
            /// read less than the requested number of bytes if the end of the
            /// stream is encountered or an error occurs.
            /// \param [in] offset The position to start reading from, specified
            ///   as a number of bytes from the beginning of the stream.
            /// \param [out] buffer Destination buffer.  Must be large enough to
            ///   hold the requested number of bytes.
            /// \param [in] length Maximum number of bytes to read.
            /// \param [out] actual Number of bytes actually read.  Will always
            ///   be less than or equal to the requested length.
            /// \return An error condition if seeking failed or reading stopped
            ///   due to an error.
            std.error_condition read_at(uint64_t offset, PointerU8 buffer, size_t length, out size_t actual);
        }


        /// \brief Interface to a random-access byte output sequence
        ///
        /// Provides an interface for writing to arbitrary positions within a
        /// byte sequence.  No further structure is provided.
        /// \sa write_stream random_read random_read_write
        public interface random_write : write_stream, random_access  //class random_write : public virtual write_stream, public virtual random_access
        {
            //using ptr = std::unique_ptr<random_write>;

            /// \brief Write at specified position
            ///
            /// Writes up to the specified number of bytes from the supplied
            /// buffer.  If seeking is supported, writing starts at the
            /// specified position and the current position is unaffected.   May
            /// write less than the requested number of bytes if an error
            /// occurs.
            /// \param [in] offset The position to start writing at, specified
            ///   as a number of bytes from the beginning of the stream.
            /// \param [in] buffer Buffer containing the data to write.  Must
            ///   contain at least the specified number of bytes.
            /// \param [in] length Number of bytes to write.
            /// \param [out] actual Number of bytes actually written.  Will
            ///   always be less than or equal to the requested length.
            /// \return An error condition if seeking failed or writing stopped
            ///   due to an error.
            std.error_condition write_at(uint64_t offset, PointerU8 buffer, size_t length, out size_t actual);
        }


        /// \brief Interface to a random-access read/write byte sequence
        ///
        /// Provides an interface for reading from and writing to arbitrary
        /// positions within a byte sequence.  No further structure is provided.
        /// \sa random_read random_write read_write_stream
        public interface random_read_write : read_write_stream, random_read, random_write  //class random_read_write : public read_write_stream, public virtual random_read, public virtual random_write
        {
        }

        /// \}


        // helper for holding a block of memory and deallocating it (or not) as necessary
        //template <typename T, bool Owned>
        //class ram_adapter_base : public virtual random_access

        // RAM read implementation
        //template <typename T, bool Owned>
        //class ram_read_adapter : public ram_adapter_base<T, Owned>, public virtual random_read

        // helper for holding a stdio FILE and closing it (or not) as necessary
        //class stdio_adapter_base : public virtual random_access

        // stdio read implementation
        //class stdio_read_adapter : public stdio_adapter_base, public virtual random_read

        // stdio read/write implementation
        //class stdio_read_write_adapter : public stdio_read_adapter, public random_read_write

        // stdio helper that fills space when writing past the end-of-file
        //class stdio_read_write_filler : public random_read_fill_wrapper<stdio_read_write_adapter>


        // helper class for holding an osd_file and closing it (or not) as necessary
        class osd_file_adapter_base : random_access
        {
            protected uint64_t m_pointer = 0U;
            osd_file m_file;  //osd_file *const m_file;
            bool m_close;  //bool const m_close;


            //osd_file_adapter_base(osd_file::ptr &&file) noexcept : m_file(file.release()), m_close(true)
            //{
            //    assert(m_file);
            //}

            protected osd_file_adapter_base(osd_file file)
            {
                m_file = file;
                m_close = false;
            }

            //virtual ~osd_file_adapter_base()


            public std.error_condition seek(int64_t offset, int whence)
            {
                switch (whence)
                {
                case SEEK_SET:
                    if (0 > offset)
                        return std.errc.invalid_argument;
                    m_pointer = (uint64_t)offset;
                    return new std.error_condition();

                case SEEK_CUR:
                    if (0 > offset)
                    {
                        if ((uint64_t)(-offset) > m_pointer)
                            return std.errc.invalid_argument;
                    }
                    else if ((uint64_t)(int64_t.MaxValue - offset) < m_pointer)  //else if ((std::numeric_limits<std::uint64_t>::max() - offset) < m_pointer)
                    {
                        return std.errc.invalid_argument;
                    }
                    m_pointer = (uint64_t)((int64_t)m_pointer + offset);  //m_pointer += offset;
                    return new std.error_condition();

                // TODO: add SEEK_END when osd_file can support it - should it return a different error?
                default:
                    return std.errc.invalid_argument;
                }
            }


            public std.error_condition tell(out uint64_t result)
            {
                result = m_pointer;
                return new std.error_condition();
            }


            public std.error_condition length(out uint64_t result)
            {
                // not supported by osd_file
                result = 0;
                return std.errc.not_supported; // TODO: revisit this error code
            }


            protected osd_file file()
            {
                return m_file;
            }


            public Stream stream { get { return m_file.stream; } }
        }


        // osd_file read implementation
        class osd_file_read_adapter : osd_file_adapter_base, random_read
        {
            //osd_file_read_adapter(osd_file::ptr &&file) noexcept : osd_file_adapter_base(std::move(file))
            //{
            //}

            public osd_file_read_adapter(osd_file file) : base(file)
            {
            }


            public std.error_condition read(PointerU8 buffer, size_t length, out size_t actual)  //virtual std::error_condition read(void *buffer, std::size_t length, std::size_t &actual) noexcept override
            {
                // TODO: should the client have to deal with reading less than expected even if EOF isn't hit?
                if (uint32_t.MaxValue < length)  //if (std::numeric_limits<std::uint32_t>::max() < length)
                {
                    actual = 0U;
                    return std.errc.invalid_argument;
                }

                // actual length not valid on error
                uint32_t count;
                std.error_condition err = file().read(buffer, m_pointer, (uint32_t)length, out count);  //std::error_condition err = file().read(buffer, m_pointer, std::uint32_t(length), count);
                if (!err)
                {
                    m_pointer += count;
                    actual = (size_t)count;
                }
                else
                {
                    actual = 0U;
                }

                return err;
            }


            public std.error_condition read_at(uint64_t offset, PointerU8 buffer, size_t length, out size_t actual)  //virtual std::error_condition read_at(std::uint64_t offset, void *buffer, std::size_t length, std::size_t &actual) noexcept override
            {
                // TODO: should the client have to deal with reading less than expected even if EOF isn't hit?
                if (uint32_t.MaxValue < length)  //if (std::numeric_limits<std::uint32_t>::max() < length)
                {
                    actual = 0U;
                    return std.errc.invalid_argument;
                }

                // actual length not valid on error
                uint32_t count;
                std.error_condition err = file().read(buffer, offset, (uint32_t)length, out count);  //std::error_condition err = file().read(buffer, offset, std::uint32_t(length), count);
                if (!err)
                    actual = (size_t)count;
                else
                    actual = 0U;

                return err;
            }
        }


        // osd_file read/write implementation
        //class osd_file_read_write_adapter : public osd_file_read_adapter, public random_read_write


        //random_read::ptr ram_read(void const *data, std::size_t size) noexcept;
        //random_read::ptr ram_read(void const *data, std::size_t size, std::uint8_t filler) noexcept;
        //random_read::ptr ram_read_copy(void const *data, std::size_t size) noexcept;
        //random_read::ptr ram_read_copy(void const *data, std::size_t size, std::uint8_t filler) noexcept;

        //random_read::ptr stdio_read(FILE *file) noexcept;
        //random_read::ptr stdio_read(FILE *file, std::uint8_t filler) noexcept;
        //random_read::ptr stdio_read_noclose(FILE *file) noexcept;
        //random_read::ptr stdio_read_noclose(FILE *file, std::uint8_t filler) noexcept;

        //random_read_write::ptr stdio_read_write(FILE *file) noexcept;
        //random_read_write::ptr stdio_read_write(FILE *file, std::uint8_t filler) noexcept;
        //random_read_write::ptr stdio_read_write_noclose(FILE *file) noexcept;
        //random_read_write::ptr stdio_read_write_noclose(FILE *file, std::uint8_t filler) noexcept;


        // creating osd_file read adapters

        //random_read::ptr osd_file_read(std::unique_ptr<osd_file> &&file) noexcept;

        public static random_read osd_file_read(osd_file file)  //random_read::ptr osd_file_read(osd_file &file) noexcept;
        {
            return new osd_file_read_adapter(file);  //return random_read::ptr(new (std::nothrow) osd_file_read_adapter(file));
        }


        //random_read_write::ptr osd_file_read_write(std::unique_ptr<osd_file> &&file) noexcept;
        //random_read_write::ptr osd_file_read_write(osd_file &file) noexcept;
    }
}
