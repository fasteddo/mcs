// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;

using PointerU8 = mame.Pointer<System.Byte>;
using size_t = System.UInt64;


namespace mame
{
    public static partial class util
    {
        // helper class for holding zlib data
        //class zlib_data


        // helper class for decompressing deflated data
        //class inflate_data : private zlib_data


        // helper class for deflating data
        //class deflate_data : private zlib_data


        // helper for holding an object and deleting it (or not) as necessary
        //template <typename T>
        //class filter_base


        // helper for forwarding to a read stream
        //template <typename T>
        //class read_stream_proxy : public virtual read_stream, public T


        // helper for forwarding to a write stream
        //template <typename T>
        //class write_stream_proxy : public virtual write_stream, public T


        // helper for forwarding to random-access storage
        //template <typename T>
        //class random_access_proxy : public virtual random_access, public T


        // helper for forwarding to random-access read storage
        //template <typename T>
        //class random_read_proxy : public virtual random_read, public read_stream_proxy<T>


        // helper for forwarding to random-access write storage
        //template <typename T>
        //class random_write_proxy : public virtual random_write, public write_stream_proxy<T>


        // helper for forwarding to random-access read/write storage
        //template <typename T>
        //class random_read_write_proxy : public random_read_write, protected random_write_proxy<random_read_proxy<T> >


        // filter for decompressing deflated data
        //template <typename Stream>
        class zlib_read_filter<Stream_> : read_stream//, filter_base<Stream>, inflate_data  //class zlib_read_filter : public read_stream, protected filter_base<Stream>, protected inflate_data
        {
            //zlib_read_filter(std::unique_ptr<Stream> &&stream, std::size_t read_chunk) noexcept :
            //    filter_base<Stream>(std::move(stream)),
            //    inflate_data(read_chunk)
            //{
            //}

            public zlib_read_filter(Stream_ stream, size_t read_chunk)
                //filter_base<Stream>(stream),
                //inflate_data(read_chunk)
            {
                throw new emu_unimplemented();
            }


            public std.error_condition read(PointerU8 buffer, size_t length, out size_t actual)  //virtual std::error_condition read(void *buffer, std::size_t length, std::size_t &actual) noexcept = 0;
            {
                throw new emu_unimplemented();
            }


            public Stream stream { get { throw new emu_unimplemented(); } }
        }


        // filter for deflating data
        class zlib_write_filter : write_stream//, filter_base<write_stream>, deflate_data  //class zlib_write_filter : public write_stream, protected filter_base<write_stream>, protected deflate_data
        {
            //zlib_write_filter(write_stream::ptr &&stream, int level, std::size_t buffer_size) noexcept :
            //    filter_base<write_stream>(std::move(stream)),
            //    deflate_data(level, buffer_size)
            //{
            //}

            public zlib_write_filter(write_stream stream, int level, size_t buffer_size)
                //filter_base<write_stream>(stream),
                //deflate_data(level, buffer_size)
            {
            }


            public std.error_condition finalize()
            {
                throw new emu_unimplemented();
            }

            public std.error_condition flush()
            {
                throw new emu_unimplemented();
            }

            public std.error_condition write(PointerU8 buffer, size_t length, out size_t actual)
            {
                throw new emu_unimplemented();
            }
        }


        /// \brief Create a read stream filter that fills unused buffer space
        ///
        /// Creates a sequential read stream filter that fills unused buffer
        /// space with a specified byte value.  If a read operation does not
        /// produce enough data to fill the supplied buffer, the remaining space
        /// in the buffer is filled with the specified filler byte value.  Takes
        /// ownership of the underlying input stream.
        /// \param [in] stream Underlying stream to read from.
        /// \param [in] filler Byte value to fill unused buffer space.
        /// \return A pointer to a sequential read stream, or nullptr on error.
        /// \sa read_stream
        //std::unique_ptr<read_stream> read_stream_fill(std::unique_ptr<read_stream> &&stream, std::uint8_t filler) noexcept;

        /// \brief Create a random access input filter that fills unused buffer
        ///   space
        ///
        /// Creates a random access input filter that fills unused buffer space
        /// with a specified byte value.  If a read operation does not produce
        /// enough data to fill the supplied buffer, the remaining space in the
        /// buffer is filled with the specified filler byte value.  Takes
        /// ownership of the underlying input sequence.
        /// \param [in] stream Underlying input sequence to read from.
        /// \param [in] filler Byte value to fill unused buffer space.
        /// \return A pointer to a random access input sequence, or nullptr on
        ///   error.
        /// \sa random_read
        //std::unique_ptr<random_read> random_read_fill(std::unique_ptr<random_read> &&stream, std::uint8_t filler) noexcept;

        /// \brief Create a read stream filter that fills unused buffer space
        ///
        /// Creates a sequential read stream filter that fills unused buffer
        /// space with a specified byte value.  If a read operation does not
        /// produce enough data to fill the supplied buffer, the remaining space
        /// in the buffer is filled with the specified filler byte value.  Does
        /// not take ownership of the underlying input stream.
        /// \param [in] stream Underlying stream to read from.
        /// \param [in] filler Byte value to fill unused buffer space.
        /// \return A pointer to a sequential read stream, or nullptr on error.
        /// \sa read_stream
        //std::unique_ptr<read_stream> read_stream_fill(read_stream &stream, std::uint8_t filler) noexcept;

        /// \brief Create a random access input filter that fills unused buffer
        ///   space
        ///
        /// Creates a random access input filter that fills unused buffer space
        /// with a specified byte value.  If a read operation does not produce
        /// enough data to fill the supplied buffer, the remaining space in the
        /// buffer is filled with the specified filler byte value.  Does not
        /// take ownership of the underlying input sequence.
        /// \param [in] stream Underlying input sequence to read from.
        /// \param [in] filler Byte value to fill unused buffer space.
        /// \return A pointer to a random access input sequence, or nullptr on
        ///   error.
        /// \sa random_read
        //std::unique_ptr<random_read> random_read_fill(random_read &stream, std::uint8_t filler) noexcept;

        /// \brief Create a random access output filter that fills unwritten
        ///   space
        ///
        /// Creates a random access output filter that fills unwritten space
        /// with a specified byte value.  If a write operation starts beyond the
        /// end of the output, the space between the end of the output and the
        /// start of the written data is filled with the specified filler byte
        /// value.  Takes ownership of the underlying output sequence.
        /// \param [in] stream Underlying output sequence to write to.
        /// \param [in] filler Byte value to fill unwritten space.
        /// \return A pointer to a random access output sequence, or nullptr on
        ///   error.
        /// \sa random_write
        //std::unique_ptr<random_write> random_write_fill(std::unique_ptr<random_write> &&stream, std::uint8_t filler) noexcept;

        /// \brief Create a random access output filter that fills unwritten
        ///   space
        ///
        /// Creates a random access output filter that fills unwritten space
        /// with a specified byte value.  If a write operation starts beyond the
        /// end of the output, the space between the end of the output and the
        /// start of the written data is filled with the specified filler byte
        /// value.  Does not take ownership of the underlying output sequence.
        /// \param [in] stream Underlying output sequence to write to.
        /// \param [in] filler Byte value to fill unwritten space.
        /// \return A pointer to a random access output sequence, or nullptr on
        ///   error.
        /// \sa random_write
        //std::unique_ptr<random_write> random_write_fill(random_write &stream, std::uint8_t filler) noexcept;

        /// \brief Create a random access I/O filter that fills unused space
        ///
        /// Creates a random access I/O sequence that fills unused read buffer
        /// space and unwritten space with a specified byte value.  If a read
        /// operation does not produce enough data to fill the supplied buffer,
        /// the remaining space in the buffer is filled with the specified
        /// filler byte value.  If a write operation starts beyond the end of
        /// the output, the space between the end of the output and the start of
        /// the written data is filled with the specified filler byte value.
        /// Takes ownership of the underlying I/O sequence.
        /// \param [in] stream Underlying I/O sequence to read from and write
        ///   to.
        /// \param [in] filler Byte value to fill unused read buffer space and
        ///   unwritten space.
        /// \return A pointer to a random access I/O sequence, or nullptr on
        ///   error.
        /// \sa random_read_write
        //std::unique_ptr<random_read_write> random_read_write_fill(std::unique_ptr<random_read_write> &&stream, std::uint8_t filler) noexcept;

        /// \brief Create a random access I/O filter that fills unused space
        ///
        /// Creates a random access I/O sequence that fills unused read buffer
        /// space and unwritten space with a specified byte value.  If a read
        /// operation does not produce enough data to fill the supplied buffer,
        /// the remaining space in the buffer is filled with the specified
        /// filler byte value.  If a write operation starts beyond the end of
        /// the output, the space between the end of the output and the start of
        /// the written data is filled with the specified filler byte value.
        /// Does not take ownership of the underlying I/O sequence.
        /// \param [in] stream Underlying I/O sequence to read from and write
        ///   to.
        /// \param [in] filler Byte value to fill unused read buffer space and
        ///   unwritten space.
        /// \return A pointer to a random access I/O sequence, or nullptr on
        ///   error.
        /// \sa random_read_write
        //std::unique_ptr<random_read_write> random_read_write_fill(random_read_write &stream, std::uint8_t filler) noexcept;


        // creating decompressing filters

        /// \brief Create an input stream filter that decompresses
        ///   zlib-compressed data
        ///
        /// Creates a read stream that decompresses zlib-compressed (deflated)
        /// data read from the underlying input stream.  A read operation will
        /// always stop on reaching an end-of-stream marker in the compressed
        /// data.  A subsequent read operation will expect to find the beginning
        /// of another block of compressed data.  May read past the end of the
        /// compressed data in the underlying input stream.  Takes ownership of
        /// the underlying input stream.
        /// \param [in] stream Underlying input stream to read from.
        /// \param [in] read_chunk Size of buffer for reading compressed data in
        ///   bytes.
        /// \return A pointer to an input stream, or nullptr on error.
        /// \sa read_stream
        //std::unique_ptr<read_stream> zlib_read(std::unique_ptr<read_stream> &&stream, std::size_t read_chunk) noexcept;

        /// \brief Create an input stream filter that decompresses
        ///   zlib-compressed data
        ///
        /// Creates a read stream that decompresses zlib-compressed (deflated)
        /// data read from the underlying input sequence.  A read operation will
        /// always stop on reaching an end-of-stream marker in the compressed
        /// data.  A subsequent read operation will expect to find the beginning
        /// of another block of compressed data.  If a read operation reads past
        /// an end-of-stream marker in the compressed data, it will seek back so
        /// the position for the next read from the underlying input sequence
        /// immediately follows the end-of-stream marker.  Takes ownership of
        /// the underlying input sequence.
        /// \param [in] stream Underlying input sequence to read from.  Must
        ///   support seeking relative to the current position.
        /// \param [in] read_chunk Size of buffer for reading compressed data in
        ///   bytes.
        /// \return A pointer to an input stream, or nullptr on error.
        /// \sa read_stream random_read
        //std::unique_ptr<read_stream> zlib_read(std::unique_ptr<random_read> &&stream, std::size_t read_chunk) noexcept;

        /// \brief Create an input stream filter that decompresses
        ///   zlib-compressed data
        ///
        /// Creates a read stream that decompresses zlib-compressed (deflated)
        /// data read from the underlying input stream.  A read operation will
        /// always stop on reaching an end-of-stream marker in the compressed
        /// data.  A subsequent read operation will expect to find the beginning
        /// of another block of compressed data.  May read past the end of the
        /// compressed data in the underlying input stream.  Does not take
        /// ownership of the underlying input stream.
        /// \param [in] stream Underlying input stream to read from.
        /// \param [in] read_chunk Size of buffer for reading compressed data in
        ///   bytes.
        /// \return A pointer to an input stream, or nullptr on error.
        /// \sa read_stream
        public static read_stream zlib_read(read_stream stream, size_t read_chunk)  //std::unique_ptr<read_stream> zlib_read(read_stream &stream, std::size_t read_chunk) noexcept;
        {
            return new zlib_read_filter<read_stream>(stream, read_chunk);  //return read_stream::ptr(new (std::nothrow) zlib_read_filter<read_stream>(stream, read_chunk));
        }

        /// \brief Create an input stream filter that decompresses
        ///   zlib-compressed data
        ///
        /// Creates a read stream that decompresses zlib-compressed (deflated)
        /// data read from the underlying input sequence.  A read operation will
        /// always stop on reaching an end-of-stream marker in the compressed
        /// data.  A subsequent read operation will expect to find the beginning
        /// of another block of compressed data.  If a read operation reads past
        /// an end-of-stream marker in the compressed data, it will seek back so
        /// the position for the next read from the underlying input sequence
        /// immediately follows the end-of-stream marker.  Does not take
        /// ownership of the underlying input sequence.
        /// \param [in] stream Underlying input sequence to read from.  Must
        ///   support seeking relative to the current position.
        /// \param [in] read_chunk Size of buffer for reading compressed data in
        ///   bytes.
        /// \return A pointer to an input stream, or nullptr on error.
        /// \sa read_stream random_read
        public static read_stream zlib_read(random_read stream, size_t read_chunk)  //std::unique_ptr<read_stream> zlib_read(random_read &stream, std::size_t read_chunk) noexcept;
        {
            return new zlib_read_filter<random_read>(stream, read_chunk);  //return read_stream::ptr(new (std::nothrow) zlib_read_filter<random_read>(stream, read_chunk));
        }


        // creating compressing filters

        /// \brief Create an output stream filter that writes zlib-compressed
        ///   data
        ///
        /// Creates an output stream that compresses data using the zlib deflate
        /// algorithm and writes it to the underlying output stream.  Calling
        /// the \c finalize member function compresses any buffered input,
        /// produces an end-of-stream maker, and writes any buffered compressed
        /// data to the underlying output stream.  A subsequent write operation
        /// will start a new compressed block.  Calling the \c flush member
        /// function writes any buffered compressed data to the underlying
        /// output stream and calls the \c flush member function of the
        /// underlying output stream; it does not ensure all buffered input data
        /// is compressed or force the end of a compressed block.  Takes
        /// ownership of the underlying output stream.
        /// \param [in] stream Underlying output stream for writing compressed
        ///   data.
        /// \param [in] level Compression level.  Use 0 for no compression, 1
        ///   for fastest compression, 9 for maximum compression, or -1 for the
        ///   default compression level as defined by the zlib library.  Larger
        ///   values between 1 and 9 provide higher compression at the expense
        ///   of speed.
        /// \param [in] buffer_size Size of buffer for compressed data in bytes.
        /// \return A pointer to an output stream, or nullptr on error.
        /// \sa write_stream
        //std::unique_ptr<write_stream> zlib_write(std::unique_ptr<write_stream> &&stream, int level, std::size_t buffer_size) noexcept;

        /// \brief Create an output stream filter that writes zlib-compressed
        ///   data
        ///
        /// Creates an output stream that compresses data using the zlib deflate
        /// algorithm and writes it to the underlying output stream.  Calling
        /// the \c finalize member function compresses any buffered input,
        /// produces an end-of-stream maker, and writes any buffered compressed
        /// data to the underlying output stream.  A subsequent write operation
        /// will start a new compressed block.  Calling the \c flush member
        /// function writes any buffered compressed data to the underlying
        /// output stream and calls the \c flush member function of the
        /// underlying output stream; it does not ensure all buffered input data
        /// is compressed or force the end of a compressed block.  Does not take
        /// ownership of the underlying output stream.
        /// \param [in] stream Underlying output stream for writing compressed
        ///   data.
        /// \param [in] level Compression level.  Use 0 for no compression, 1
        ///   for fastest compression, 9 for maximum compression, or -1 for the
        ///   default compression level as defined by the zlib library.  Larger
        ///   values between 1 and 9 provide higher compression at the expense
        ///   of speed.
        /// \param [in] buffer_size Size of buffer for compressed data in bytes.
        /// \return A pointer to an output stream, or nullptr on error.
        /// \sa write_stream
        public static write_stream zlib_write(write_stream stream, int level, size_t buffer_size)  //std::unique_ptr<write_stream> zlib_write(write_stream &stream, int level, std::size_t buffer_size) noexcept;
        {
            return new zlib_write_filter(stream, level, buffer_size);  //return write_stream::ptr(new (std::nothrow) zlib_write_filter(stream, level, buffer_size));
        }
    }
}
