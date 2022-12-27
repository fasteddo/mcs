// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using s16 = System.Int16;
using u8 = System.Byte;
using u32 = System.UInt32;
using unsigned = System.UInt32;

using static mame.nes_defs_global;


namespace mame
{
    /* APU type */
    class apu_t
    {
        /* CHANNEL TYPE DEFINITIONS */

        /* Square Wave */
        public class square_t
        {
            //square_t()
            //{
            //    for (auto & elem : regs)
            //        elem = 0;
            //}

            public u8 [] regs = new u8 [4];
            public int vbl_length = 0;
            public int freq = 0;
            public float phaseacc = 0.0f;
            public float env_phase = 0.0f;
            public float sweep_phase = 0.0f;
            public u8 adder = 0;
            public u8 env_vol = 0;
            public bool enabled = false;
            public u8 output = 0;
        }


        /* Triangle Wave */
        public class triangle_t
        {
            //triangle_t()
            //{
            //    for (auto & elem : regs)
            //        elem = 0;
            //}

            public u8 [] regs = new u8 [4]; /* regs[1] unused */
            public int linear_length = 0;
            public bool linear_reload = false;
            public int vbl_length = 0;
            public int write_latency = 0;
            public float phaseacc = 0.0f;
            public u8 adder = 0;
            public bool counter_started = false;
            public bool enabled = false;
            public u8 output = 0;
        }


        /* Noise Wave */
        public class noise_t
        {
            //noise_t()
            //{
            //    for (auto & elem : regs)
            //        elem = 0;
            //}

            public u8 [] regs = new u8 [4]; /* regs[1] unused */
            public u32 seed = 1;
            public int vbl_length = 0;
            public float phaseacc = 0.0f;
            public float env_phase = 0.0f;
            public u8 env_vol = 0;
            public bool enabled = false;
            public u8 output = 0;
        }


        /* DPCM Wave */
        public class dpcm_t
        {
            //dpcm_t()
            //{
            //    for (auto & elem : regs)
            //        elem = 0;
            //}

            public u8 [] regs = new u8 [4];
            public u32 address = 0;
            public u32 length = 0;
            public int bits_left = 0;
            public float phaseacc = 0.0f;
            public u8 cur_byte = 0;
            public bool enabled = false;
            public bool irq_occurred = false;
            public s16 vol = 0;
            public u8 output = 0;
        }


        /* REGISTER DEFINITIONS */
        public const unsigned WRA0    = 0x00;
        public const unsigned WRA1    = 0x01;
        public const unsigned WRA2    = 0x02;
        public const unsigned WRA3    = 0x03;
        public const unsigned WRB0    = 0x04;
        public const unsigned WRB1    = 0x05;
        public const unsigned WRB2    = 0x06;
        public const unsigned WRB3    = 0x07;
        public const unsigned WRC0    = 0x08;
        public const unsigned WRC2    = 0x0A;
        public const unsigned WRC3    = 0x0B;
        public const unsigned WRD0    = 0x0C;
        public const unsigned WRD2    = 0x0E;
        public const unsigned WRD3    = 0x0F;
        public const unsigned WRE0    = 0x10;
        public const unsigned WRE1    = 0x11;
        public const unsigned WRE2    = 0x12;
        public const unsigned WRE3    = 0x13;
        public const unsigned SMASK   = 0x15;
        public const unsigned IRQCTRL = 0x17;


        /* Sound channels */
        public square_t [] squ = new square_t [2] { new square_t(), new square_t() };
        public triangle_t tri = new triangle_t();
        public noise_t noi = new noise_t();
        public dpcm_t dpcm = new dpcm_t();

        public int step_mode = 0;
    }


    static class nes_defs_global
    {
        /* CONSTANTS */

        /* vblank length table used for squares, triangle, noise */
        public static readonly u8 [] vbl_length = new u8 [32]
        {
            10, 254, 20,  2, 40,  4, 80,  6, 160,  8, 60, 10, 14, 12, 26, 14,
            12,  16, 24, 18, 48, 20, 96, 22, 192, 24, 72, 26, 16, 28, 32, 30
        };

        /* frequency limit of square channels */
        //static const int freq_limit[8] =
        //{
        //    0x3FF, 0x555, 0x666, 0x71C, 0x787, 0x7C1, 0x7E0, 0x7F0,
        //};

        // table of noise period
        // each fundamental is determined as: freq = master / period / 93
        //static const int noise_freq[2][16] =
        //{
        //    { 4, 8, 16, 32, 64, 96, 128, 160, 202, 254, 380, 508, 762, 1016, 2034, 4068 }, // NTSC
        //    { 4, 8, 14, 30, 60, 88, 118, 148, 188, 236, 354, 472, 708,  944, 1890, 3778 }  // PAL
        //};

        // dpcm (cpu) cycle period
        // each frequency is determined as: freq = master / period
        //static const int dpcm_clocks[2][16] =
        //{
        //    { 428, 380, 340, 320, 286, 254, 226, 214, 190, 160, 142, 128, 106, 84, 72, 54 }, // NTSC
        //    { 398, 354, 316, 298, 276, 236, 210, 198, 176, 148, 132, 118, 98, 78, 66, 50 } // PAL
        //};

        /* ratios of pos/neg pulse for square waves */
        /* 2/16 = 12.5%, 4/16 = 25%, 8/16 = 50%, 12/16 = 75% */
        //static const int duty_lut[4] =
        //{
        //    0b01000000, // 01000000 (12.5%)
        //    0b01100000, // 01100000 (25%)
        //    0b01111000, // 01111000 (50%)
        //    0b10011111, // 10011111 (25% negated)
        //};
    }
}
