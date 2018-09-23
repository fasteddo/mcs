// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    public static class sha1_global
    {
        const int SHA1_DIGEST_SIZE = 20;
        public const int SHA1_DATA_SIZE = 64;

        /* Digest is kept internally as 4 32-bit words. */
        public const int _SHA1_DIGEST_LENGTH = 5;


        const UInt32 h0init = 0x67452301;
        const UInt32 h1init = 0xEFCDAB89;
        const UInt32 h2init = 0x98BADCFE;
        const UInt32 h3init = 0x10325476;
        const UInt32 h4init = 0xC3D2E1F0;


        public static void sha1_init(out sha1_ctx ctx)
        {
            ctx = new sha1_ctx();

            /* Set the h-vars to their initial values */
            ctx.digest[ 0 ] = h0init;
            ctx.digest[ 1 ] = h1init;
            ctx.digest[ 2 ] = h2init;
            ctx.digest[ 3 ] = h3init;
            ctx.digest[ 4 ] = h4init;

            /* Initialize bit count */
            ctx.count_low = ctx.count_high = 0;

            /* Initialize buffer */
            ctx.index = 0;
        }

        public static void sha1_update(ref sha1_ctx ctx, UInt32 length, ListBytesPointer data)
        {
            //throw new emu_unimplemented();
        }

        public static void sha1_final(ref sha1_ctx ctx)
        {
            //throw new emu_unimplemented();
        }

        public static void sha1_digest(sha1_ctx ctx, uint8_t [] digest)
        {
            //throw new emu_unimplemented();
        }
    }


    public class sha1_ctx
    {
        public uint32_t [] digest = new uint32_t[sha1_global._SHA1_DIGEST_LENGTH];   /* Message digest */
        public uint32_t count_low;
        public uint32_t count_high;         /* 64-bit block count */
        uint8_t [] block = new uint8_t[sha1_global.SHA1_DATA_SIZE];          /* SHA1 data buffer */
        public UInt32 index;                     /* index into buffer */
    }
}
