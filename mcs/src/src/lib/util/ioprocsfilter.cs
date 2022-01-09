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


        //std::unique_ptr<read_stream> read_stream_fill(std::unique_ptr<read_stream> &&stream, std::uint8_t filler) noexcept;
        //std::unique_ptr<random_read> random_read_fill(std::unique_ptr<random_read> &&stream, std::uint8_t filler) noexcept;
        //std::unique_ptr<read_stream> read_stream_fill(read_stream &stream, std::uint8_t filler) noexcept;
        //std::unique_ptr<random_read> random_read_fill(random_read &stream, std::uint8_t filler) noexcept;

        //std::unique_ptr<random_write> random_write_fill(std::unique_ptr<random_write> &&stream, std::uint8_t filler) noexcept;
        //std::unique_ptr<random_write> random_write_fill(random_write &stream, std::uint8_t filler) noexcept;

        //std::unique_ptr<random_read_write> random_read_write_fill(std::unique_ptr<random_read_write> &&stream, std::uint8_t filler) noexcept;
        //std::unique_ptr<random_read_write> random_read_write_fill(random_read_write &stream, std::uint8_t filler) noexcept;


        // creating decompressing filters

        //std::unique_ptr<read_stream> zlib_read(std::unique_ptr<read_stream> &&stream, std::size_t read_chunk) noexcept;
        //std::unique_ptr<read_stream> zlib_read(std::unique_ptr<random_read> &&stream, std::size_t read_chunk) noexcept;

        public static read_stream zlib_read(read_stream stream, size_t read_chunk)  //std::unique_ptr<read_stream> zlib_read(read_stream &stream, std::size_t read_chunk) noexcept;
        {
            return new zlib_read_filter<read_stream>(stream, read_chunk);  //return read_stream::ptr(new (std::nothrow) zlib_read_filter<read_stream>(stream, read_chunk));
        }

        public static read_stream zlib_read(random_read stream, size_t read_chunk)  //std::unique_ptr<read_stream> zlib_read(random_read &stream, std::size_t read_chunk) noexcept;
        {
            return new zlib_read_filter<random_read>(stream, read_chunk);  //return read_stream::ptr(new (std::nothrow) zlib_read_filter<random_read>(stream, read_chunk));
        }


        // creating compressing filters

        //std::unique_ptr<write_stream> zlib_write(std::unique_ptr<write_stream> &&stream, int level, std::size_t buffer_size) noexcept;

        public static write_stream zlib_write(write_stream stream, int level, size_t buffer_size)  //std::unique_ptr<write_stream> zlib_write(write_stream &stream, int level, std::size_t buffer_size) noexcept;
        {
            return new zlib_write_filter(stream, level, buffer_size);  //return write_stream::ptr(new (std::nothrow) zlib_write_filter(stream, level, buffer_size));
        }
    }
}
