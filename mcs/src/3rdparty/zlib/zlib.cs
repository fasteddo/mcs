// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;


namespace mame
{
    public static class zlib_global
    {
        /* constants */
        public const int Z_NO_FLUSH      = 0;
        public const int Z_PARTIAL_FLUSH = 1;
        public const int Z_SYNC_FLUSH    = 2;
        public const int Z_FULL_FLUSH    = 3;
        public const int Z_FINISH        = 4;
        public const int Z_BLOCK         = 5;
        public const int Z_TREES         = 6;
        /* Allowed flush values; see deflate() and inflate() below for details */

        public const int Z_OK            =  0;
        public const int Z_STREAM_END    =  1;
        public const int Z_NEED_DICT     =  2;
        public const int Z_ERRNO         = -1;
        public const int Z_STREAM_ERROR  = -2;
        public const int Z_DATA_ERROR    = -3;
        public const int Z_MEM_ERROR     = -4;
        public const int Z_BUF_ERROR     = -5;
        public const int Z_VERSION_ERROR = -6;


        public static int inflate(z_stream strm, int flush)
        {
            throw new emu_unimplemented();
        }
    }


    public struct z_stream
    {
        public ListBytesPointer next_in;  //z_const Bytef *next_in;     /* next input byte */
        public UInt32 next_inOffset;
        public UInt32 avail_in;  /* number of bytes available at next_in */
        //uLong    total_in;  /* total number of input bytes read so far */

        public ListBytesPointer next_out;  //Bytef    *next_out; /* next output byte should be put there */
        public UInt32 avail_out; /* remaining free space at next_out */
        //uLong    total_out; /* total number of bytes output so far */

        //z_const char *msg;  /* last error message, NULL if no error */
        //struct internal_state FAR *state; /* not visible by applications */

        //alloc_func zalloc;  /* used to allocate the internal state */
        //free_func  zfree;   /* used to free the internal state */
        //voidpf     opaque;  /* private data object passed to zalloc and zfree */

        //int     data_type;  /* best guess about the data type: binary or text */
        //uLong   adler;      /* adler32 value of the uncompressed data */
        //uLong   reserved;   /* reserved for future use */
    }
}
