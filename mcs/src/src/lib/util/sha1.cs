// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
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


        static UInt32 READ_UINT32(Pointer<uint8_t> data)
        {
            return ((uint32_t)data[0] << 24) |
                   ((uint32_t)data[1] << 16) |
                   ((uint32_t)data[2] << 8) |
                   ((uint32_t)data[3]);
        }


        static void WRITE_UINT32(Pointer<uint8_t> data, uint32_t val)
        {
            data[0] = (uint8_t)((val >> 24) & 0xFF);
            data[1] = (uint8_t)((val >> 16) & 0xFF);
            data[2] = (uint8_t)((val >> 8) & 0xFF);
            data[3] = (uint8_t)((val >> 0) & 0xFF);
        }


        /* A block, treated as a sequence of 32-bit words. */
        const int SHA1_DATA_LENGTH = 16;


        /* The SHA f()-functions.  The f1 and f3 functions can be optimized to
           save one boolean operation each - thanks to Rich Schroeppel,
           rcs@cs.arizona.edu for discovering this */
        /* #define f1(x,y,z) ( ( x & y ) | ( ~x & z ) )            Rounds  0-19 */
        static uint32_t f1(uint32_t x, uint32_t y, uint32_t z) { return z ^ (x & (y ^ z)); }           /* Rounds  0-19 */
        static uint32_t f2(uint32_t x, uint32_t y, uint32_t z) { return x ^ y ^ z; }                       /* Rounds 20-39 */
        /* #define f3(x,y,z) ( ( x & y ) | ( x & z ) | ( y & z ) ) Rounds 40-59 */
        static uint32_t f3(uint32_t x, uint32_t y, uint32_t z) { return (x & y) | (z & (x | y)); }   /* Rounds 40-59 */
        static uint32_t f4(uint32_t x, uint32_t y, uint32_t z) { return x ^ y ^ z; }                       /* Rounds 60-79 */


        /* The SHA Mysterious Constants */
        const uint32_t K1 = 0x5A827999;                                 /* Rounds  0-19 */
        const uint32_t K2 = 0x6ED9EBA1;                                 /* Rounds 20-39 */
        const uint32_t K3 = 0x8F1BBCDC;                                 /* Rounds 40-59 */
        const uint32_t K4 = 0xCA62C1D6;                                 /* Rounds 60-79 */


        /* SHA initial values */
        const uint32_t h0init = 0x67452301;
        const uint32_t h1init = 0xEFCDAB89;
        const uint32_t h2init = 0x98BADCFE;
        const uint32_t h3init = 0x10325476;
        const uint32_t h4init = 0xC3D2E1F0;


        /* 32-bit rotate left - kludged with shifts */
        //#ifdef _MSC_VER
        //#define ROTL(n,X)  _lrotl(X, n)
        //#else
        //#define ROTL(n,X)  ( ( (X) << (n) ) | ( (X) >> ( 32 - (n) ) ) )
        //#endif
        static uint32_t ROTL(int n, uint32_t X) { return (X << n) | (X >> (32 - n)); }


        /* The initial expanding function.  The hash function is defined over an
           80-word expanded input array W, where the first 16 are copies of the input
           data, and the remaining 64 are defined by

                W[ i ] = W[ i - 16 ] ^ W[ i - 14 ] ^ W[ i - 8 ] ^ W[ i - 3 ]

           This implementation generates these values on the fly in a circular
           buffer - thanks to Colin Plumb, colin@nyx10.cs.du.edu for this
           optimization.

           The updated SHA changes the expanding function by adding a rotate of 1
           bit.  Thanks to Jim Gillogly, jim@rand.org, and an anonymous contributor
           for this information */

        static uint32_t expand(uint32_t [] W, int i) { return W[i & 15] = ROTL(1, W[i & 15] ^ W[(i - 14) & 15] ^ W[(i - 8) & 15] ^ W[(i - 3) & 15]); }  //#define expand(W,i) ( W[ i & 15 ] = ROTL( 1, ( W[ i & 15 ] ^ W[ (i - 14) & 15 ] ^ W[ (i - 8) & 15 ] ^ W[ (i - 3) & 15 ] ) ) )


        /* The prototype SHA sub-round.  The fundamental sub-round is:

                a' = e + ROTL( 5, a ) + f( b, c, d ) + k + data;
                b' = a;
                c' = ROTL( 30, b );
                d' = c;
                e' = d;

           but this is implemented by unrolling the loop 5 times and renaming the
           variables ( e, a, b, c, d ) = ( a', b', c', d', e' ) each iteration.
           This code is then replicated 20 times for each of the 4 functions, using
           the next 20 values from the W[] array each time */
        static void subRound(uint32_t a, ref uint32_t b, uint32_t c, uint32_t d, ref uint32_t e, Func<uint32_t, uint32_t, uint32_t, uint32_t> f, uint32_t k, uint32_t data) { e += ROTL( 5, a ) + f( b, c, d ) + k + data; b = ROTL( 30, b ); }  //#define subRound(a, b, c, d, e, f, k, data) ( e += ROTL( 5, a ) + f( b, c, d ) + k + data, b = ROTL( 30, b ) )


        /* Initialize the SHA values */
        /**
         * @fn  void sha1_init(struct sha1_ctx *ctx)
         *
         * @brief   Sha 1 initialise.
         *
         * @param [in,out]  ctx If non-null, the context.
         */
        public static void sha1_init(out sha1_ctx ctx)
        {
            ctx = new sha1_ctx();

            /* Set the h-vars to their initial values */
            ctx.digest[0] = h0init;
            ctx.digest[1] = h1init;
            ctx.digest[2] = h2init;
            ctx.digest[3] = h3init;
            ctx.digest[4] = h4init;

            /* Initialize bit count */
            ctx.count_low = ctx.count_high = 0;

            /* Initialize buffer */
            ctx.index = 0;
        }


        /* Perform the SHA transformation.  Note that this code, like MD5, seems to
           break some optimizing compilers due to the complexity of the expressions
           and the size of the basic block.  It may be necessary to split it into
           sections, e.g. based on the four subrounds

           Note that this function destroys the data area */
        /**
         * @fn  static void sha1_transform(uint32_t *state, uint32_t *data)
         *
         * @brief   Sha 1 transform.
         *
         * @param [in,out]  state   If non-null, the state.
         * @param [in,out]  data    If non-null, the data.
         */
        static void sha1_transform(uint32_t [] state, uint32_t [] data)  //static void sha1_transform(uint32_t *state, uint32_t *data)
        {
            uint32_t A;
            uint32_t B;
            uint32_t C;
            uint32_t D;
            uint32_t E;     /* Local vars */

            /* Set up first buffer and local data buffer */
            A = state[0];
            B = state[1];
            C = state[2];
            D = state[3];
            E = state[4];

            /* Heavy mangling, in 4 sub-rounds of 20 iterations each. */
            subRound( A, ref B, C, D, ref E, f1, K1, data[ 0] );
            subRound( E, ref A, B, C, ref D, f1, K1, data[ 1] );
            subRound( D, ref E, A, B, ref C, f1, K1, data[ 2] );
            subRound( C, ref D, E, A, ref B, f1, K1, data[ 3] );
            subRound( B, ref C, D, E, ref A, f1, K1, data[ 4] );
            subRound( A, ref B, C, D, ref E, f1, K1, data[ 5] );
            subRound( E, ref A, B, C, ref D, f1, K1, data[ 6] );
            subRound( D, ref E, A, B, ref C, f1, K1, data[ 7] );
            subRound( C, ref D, E, A, ref B, f1, K1, data[ 8] );
            subRound( B, ref C, D, E, ref A, f1, K1, data[ 9] );
            subRound( A, ref B, C, D, ref E, f1, K1, data[10] );
            subRound( E, ref A, B, C, ref D, f1, K1, data[11] );
            subRound( D, ref E, A, B, ref C, f1, K1, data[12] );
            subRound( C, ref D, E, A, ref B, f1, K1, data[13] );
            subRound( B, ref C, D, E, ref A, f1, K1, data[14] );
            subRound( A, ref B, C, D, ref E, f1, K1, data[15] );
            subRound( E, ref A, B, C, ref D, f1, K1, expand( data, 16 ) );
            subRound( D, ref E, A, B, ref C, f1, K1, expand( data, 17 ) );
            subRound( C, ref D, E, A, ref B, f1, K1, expand( data, 18 ) );
            subRound( B, ref C, D, E, ref A, f1, K1, expand( data, 19 ) );

            subRound( A, ref B, C, D, ref E, f2, K2, expand( data, 20 ) );
            subRound( E, ref A, B, C, ref D, f2, K2, expand( data, 21 ) );
            subRound( D, ref E, A, B, ref C, f2, K2, expand( data, 22 ) );
            subRound( C, ref D, E, A, ref B, f2, K2, expand( data, 23 ) );
            subRound( B, ref C, D, E, ref A, f2, K2, expand( data, 24 ) );
            subRound( A, ref B, C, D, ref E, f2, K2, expand( data, 25 ) );
            subRound( E, ref A, B, C, ref D, f2, K2, expand( data, 26 ) );
            subRound( D, ref E, A, B, ref C, f2, K2, expand( data, 27 ) );
            subRound( C, ref D, E, A, ref B, f2, K2, expand( data, 28 ) );
            subRound( B, ref C, D, E, ref A, f2, K2, expand( data, 29 ) );
            subRound( A, ref B, C, D, ref E, f2, K2, expand( data, 30 ) );
            subRound( E, ref A, B, C, ref D, f2, K2, expand( data, 31 ) );
            subRound( D, ref E, A, B, ref C, f2, K2, expand( data, 32 ) );
            subRound( C, ref D, E, A, ref B, f2, K2, expand( data, 33 ) );
            subRound( B, ref C, D, E, ref A, f2, K2, expand( data, 34 ) );
            subRound( A, ref B, C, D, ref E, f2, K2, expand( data, 35 ) );
            subRound( E, ref A, B, C, ref D, f2, K2, expand( data, 36 ) );
            subRound( D, ref E, A, B, ref C, f2, K2, expand( data, 37 ) );
            subRound( C, ref D, E, A, ref B, f2, K2, expand( data, 38 ) );
            subRound( B, ref C, D, E, ref A, f2, K2, expand( data, 39 ) );

            subRound( A, ref B, C, D, ref E, f3, K3, expand( data, 40 ) );
            subRound( E, ref A, B, C, ref D, f3, K3, expand( data, 41 ) );
            subRound( D, ref E, A, B, ref C, f3, K3, expand( data, 42 ) );
            subRound( C, ref D, E, A, ref B, f3, K3, expand( data, 43 ) );
            subRound( B, ref C, D, E, ref A, f3, K3, expand( data, 44 ) );
            subRound( A, ref B, C, D, ref E, f3, K3, expand( data, 45 ) );
            subRound( E, ref A, B, C, ref D, f3, K3, expand( data, 46 ) );
            subRound( D, ref E, A, B, ref C, f3, K3, expand( data, 47 ) );
            subRound( C, ref D, E, A, ref B, f3, K3, expand( data, 48 ) );
            subRound( B, ref C, D, E, ref A, f3, K3, expand( data, 49 ) );
            subRound( A, ref B, C, D, ref E, f3, K3, expand( data, 50 ) );
            subRound( E, ref A, B, C, ref D, f3, K3, expand( data, 51 ) );
            subRound( D, ref E, A, B, ref C, f3, K3, expand( data, 52 ) );
            subRound( C, ref D, E, A, ref B, f3, K3, expand( data, 53 ) );
            subRound( B, ref C, D, E, ref A, f3, K3, expand( data, 54 ) );
            subRound( A, ref B, C, D, ref E, f3, K3, expand( data, 55 ) );
            subRound( E, ref A, B, C, ref D, f3, K3, expand( data, 56 ) );
            subRound( D, ref E, A, B, ref C, f3, K3, expand( data, 57 ) );
            subRound( C, ref D, E, A, ref B, f3, K3, expand( data, 58 ) );
            subRound( B, ref C, D, E, ref A, f3, K3, expand( data, 59 ) );

            subRound( A, ref B, C, D, ref E, f4, K4, expand( data, 60 ) );
            subRound( E, ref A, B, C, ref D, f4, K4, expand( data, 61 ) );
            subRound( D, ref E, A, B, ref C, f4, K4, expand( data, 62 ) );
            subRound( C, ref D, E, A, ref B, f4, K4, expand( data, 63 ) );
            subRound( B, ref C, D, E, ref A, f4, K4, expand( data, 64 ) );
            subRound( A, ref B, C, D, ref E, f4, K4, expand( data, 65 ) );
            subRound( E, ref A, B, C, ref D, f4, K4, expand( data, 66 ) );
            subRound( D, ref E, A, B, ref C, f4, K4, expand( data, 67 ) );
            subRound( C, ref D, E, A, ref B, f4, K4, expand( data, 68 ) );
            subRound( B, ref C, D, E, ref A, f4, K4, expand( data, 69 ) );
            subRound( A, ref B, C, D, ref E, f4, K4, expand( data, 70 ) );
            subRound( E, ref A, B, C, ref D, f4, K4, expand( data, 71 ) );
            subRound( D, ref E, A, B, ref C, f4, K4, expand( data, 72 ) );
            subRound( C, ref D, E, A, ref B, f4, K4, expand( data, 73 ) );
            subRound( B, ref C, D, E, ref A, f4, K4, expand( data, 74 ) );
            subRound( A, ref B, C, D, ref E, f4, K4, expand( data, 75 ) );
            subRound( E, ref A, B, C, ref D, f4, K4, expand( data, 76 ) );
            subRound( D, ref E, A, B, ref C, f4, K4, expand( data, 77 ) );
            subRound( C, ref D, E, A, ref B, f4, K4, expand( data, 78 ) );
            subRound( B, ref C, D, E, ref A, f4, K4, expand( data, 79 ) );

            /* Build message digest */
            state[0] += A;
            state[1] += B;
            state[2] += C;
            state[3] += D;
            state[4] += E;
        }


        /**
         * @fn  static void sha1_block(struct sha1_ctx *ctx, const uint8_t *block)
         *
         * @brief   Sha 1 block.
         *
         * @param [in,out]  ctx If non-null, the context.
         * @param   block       The block.
         */
        static void sha1_block(sha1_ctx ctx, Pointer<uint8_t> block_)  //static void sha1_block(struct sha1_ctx *ctx, const uint8_t *block)
        {
            Pointer<uint8_t> block = new Pointer<uint8_t>(block_);

            uint32_t [] data = new uint32_t [SHA1_DATA_LENGTH];
            int i;

            /* Update block count */
            if (++ctx.count_low == 0)
                ++ctx.count_high;

            /* Endian independent conversion */
            for (i = 0; i < SHA1_DATA_LENGTH; i++, block += 4)
                data[i] = READ_UINT32(block);

            sha1_transform(ctx.digest, data);
        }


        /**
         * @fn  void sha1_update(struct sha1_ctx *ctx, unsigned length, const uint8_t *buffer)
         *
         * @brief   Sha 1 update.
         *
         * @param [in,out]  ctx If non-null, the context.
         * @param   length      The length.
         * @param   buffer      The buffer.
         */
        public static void sha1_update(sha1_ctx ctx, UInt32 length, Pointer<uint8_t> buffer_)  //void sha1_update(struct sha1_ctx *ctx, unsigned length, const uint8_t *buffer)
        {
            Pointer<uint8_t> buffer = new Pointer<uint8_t>(buffer_);

            if (ctx.index != 0)
            { 
                /* Try to fill partial block */
                UInt32 left = SHA1_DATA_SIZE - ctx.index;
                if (length < left)
                {
                    std.memcpy(new Pointer<uint8_t>(ctx.block, (int)ctx.index), buffer, length);
                    ctx.index += length;
                    return; /* Finished */
                }
                else
                {
                    std.memcpy(new Pointer<uint8_t>(ctx.block, (int)ctx.index), buffer, left);
                    sha1_block(ctx, new Pointer<uint8_t>(ctx.block));
                    buffer += left;
                    length -= left;
                }
            }

            while (length >= SHA1_DATA_SIZE)
            {
                sha1_block(ctx, buffer);
                buffer += SHA1_DATA_SIZE;
                length -= SHA1_DATA_SIZE;
            }

            ctx.index = length;
            if (length != 0)
                /* Buffer leftovers */
                std.memcpy(new Pointer<uint8_t>(ctx.block), buffer, length);
        }


        /* Final wrapup - pad to SHA1_DATA_SIZE-byte boundary with the bit pattern
           1 0* (64-bit count of bits processed, MSB-first) */
        /**
         * @fn  void sha1_final(struct sha1_ctx *ctx)
         *
         * @brief   Sha 1 final.
         *
         * @param [in,out]  ctx If non-null, the context.
         */
        public static void sha1_final(sha1_ctx ctx)
        {
            uint32_t [] data = new uint32_t [SHA1_DATA_LENGTH];
            int i;
            int words;

            i = (int)ctx.index;

            /* Set the first char of padding to 0x80.  This is safe since there is
             always at least one byte free */

            global_object.assert(i < SHA1_DATA_SIZE);
            ctx.block[i++] = 0x80;

            /* Fill rest of word */
            for ( ; (i & 3) != 0; i++)
                ctx.block[i] = 0;

            /* i is now a multiple of the word size 4 */
            words = i >> 2;
            for (i = 0; i < words; i++)
                data[i] = READ_UINT32(new Pointer<uint8_t>(ctx.block, 4 * i));

            if (words > (SHA1_DATA_LENGTH-2))
            {
                /* No room for length in this block. Process it and
                 * pad with another one */
                for (i = words ; i < SHA1_DATA_LENGTH; i++)
                    data[i] = 0;

                sha1_transform(ctx.digest, data);

                for (i = 0; i < SHA1_DATA_LENGTH - 2; i++)
                    data[i] = 0;
            }
            else
            {
                for (i = words ; i < SHA1_DATA_LENGTH - 2; i++)
                    data[i] = 0;
            }

            /* There are 512 = 2^9 bits in one block */
            data[SHA1_DATA_LENGTH - 2] = (ctx.count_high << 9) | (ctx.count_low >> 23);
            data[SHA1_DATA_LENGTH - 1] = (ctx.count_low << 9) | (ctx.index << 3);
            sha1_transform(ctx.digest, data);
        }


        /**
         * @fn  void sha1_digest(const struct sha1_ctx *ctx, unsigned length, uint8_t *digest)
         *
         * @brief   Sha 1 digest.
         *
         * @param   ctx             The context.
         * @param   length          The length.
         * @param [in,out]  digest  If non-null, the digest.
         */
        public static void sha1_digest(sha1_ctx ctx, UInt32 length, Pointer<uint8_t> digest_)  //void sha1_digest(const struct sha1_ctx *ctx, unsigned length, uint8_t *digest)
        {
            Pointer<uint8_t> digest = new Pointer<uint8_t>(digest_);

            UInt32 i;
            UInt32 words;
            UInt32 leftover;

            global_object.assert(length <= SHA1_DIGEST_SIZE);

            words = length / 4;
            leftover = length % 4;

            for (i = 0; i < words; i++, digest += 4)
                WRITE_UINT32(digest, ctx.digest[i]);

            if (leftover != 0)
            {
                uint32_t word;
                UInt32 j = leftover;

                global_object.assert(i < _SHA1_DIGEST_LENGTH);

                word = ctx.digest[i];

                switch (leftover)
                {
                    default:
                        /* this is just here to keep the compiler happy; it can never happen */
                        break;
                    case 3:
                        digest[--j] = (uint8_t)((word >> 8) & 0xff);
                        /* Fall through */
                        goto case 2;
                    case 2:
                        digest[--j] = (uint8_t)((word >> 16) & 0xff);
                        /* Fall through */
                        goto case 1;
                    case 1:
                        digest[--j] = (uint8_t)((word >> 24) & 0xff);
                        break;
                }
            }
        }
    }


    public class sha1_ctx
    {
        public uint32_t [] digest = new uint32_t[sha1_global._SHA1_DIGEST_LENGTH];   /* Message digest */
        public uint32_t count_low;
        public uint32_t count_high;         /* 64-bit block count */
        public MemoryU8 block = new MemoryU8();  //uint8_t block[SHA1_DATA_SIZE];          /* SHA1 data buffer */
        public UInt32 index;                     /* index into buffer */

        public sha1_ctx() { block.Resize(sha1_global.SHA1_DATA_SIZE); }
    }
}
