// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;  //using offs_t = u32;
using optional_address_space = mame.address_space_finder<mame.bool_const_false>;  //using optional_address_space = address_space_finder<false>;
using optional_memory_bank = mame.memory_bank_finder<mame.bool_const_false>;  //using optional_memory_bank = memory_bank_finder<false>;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    public class atari_slapstic_device : device_t
    {
        //DEFINE_DEVICE_TYPE(SLAPSTIC, atari_slapstic_device, "slapstic", "Atari Slapstic")
        static device_t device_creator_atari_slapstic_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new atari_slapstic_device(mconfig, tag, owner, clock); }
        public static readonly device_type SLAPSTIC = g.DEFINE_DEVICE_TYPE(device_creator_atari_slapstic_device, "slapstic", "Atari Slapstic");


        struct mask_value
        {
            public u16 mask;
            public u16 value;

            public mask_value(u16 mask, u16 value) { this.mask = mask; this.value = value; }
        }


        class slapstic_data
        {
            public u8 bankstart;
            public u16 [] bank = new u16[4];

            public mask_value alt1;
            public mask_value alt2;
            public mask_value alt3;
            public mask_value alt4;
            public int altshift;

            public mask_value bit1;
            public mask_value bit2;
            public mask_value bit3c0;
            public mask_value bit3s0;
            public mask_value bit3c1;
            public mask_value bit3s1;
            public mask_value bit4;

            public mask_value add1;
            public mask_value add2;
            public mask_value addplus1;
            public mask_value addplus2;
            public mask_value add3;


            public slapstic_data() { }
            public slapstic_data
            (
                u8 bankstart,
                u16 [] bank,
                mask_value alt1,
                mask_value alt2,
                mask_value alt3,
                mask_value alt4,
                int altshift,
                mask_value bit1,
                mask_value bit2,
                mask_value bit3c0,
                mask_value bit3s0,
                mask_value bit3c1,
                mask_value bit3s1,
                mask_value bit4,
                mask_value add1,
                mask_value add2,
                mask_value addplus1,
                mask_value addplus2,
                mask_value add3
            )
            {
                this.bankstart = bankstart;
                this.bank = bank;
                this.alt1 = alt1;
                this.alt2 = alt2;
                this.alt3 = alt3;
                this.alt4 = alt4;
                this.altshift = altshift;
                this.bit1 = bit1;
                this.bit2 = bit2;
                this.bit3c0 = bit3c0;
                this.bit3s0 = bit3s0;
                this.bit3c1 = bit3c1;
                this.bit3s1 = bit3s1;
                this.bit4 = bit4;
                this.add1 = add1;
                this.add2 = add2;
                this.addplus1 = addplus1;
                this.addplus2 = addplus2;
                this.add3 = add3;
            }
        }


        /*************************************
         *
         *  Slapstic definitions
         *
         *************************************/

        const u16 UNKNOWN = 0xffff;  //#define UNKNOWN 0xffff
        //#define NO_BITWISE          \
        //    { UNKNOWN,UNKNOWN },    \
        //    { UNKNOWN,UNKNOWN },    \
        //    { UNKNOWN,UNKNOWN },    \
        //    { UNKNOWN,UNKNOWN },    \
        //    { UNKNOWN,UNKNOWN },    \
        //    { UNKNOWN,UNKNOWN },    \
        //    { UNKNOWN,UNKNOWN }
        //#define NO_ADDITIVE         \
        //    { UNKNOWN,UNKNOWN },    \
        //    { UNKNOWN,UNKNOWN },    \
        //    { UNKNOWN,UNKNOWN },    \
        //    { UNKNOWN,UNKNOWN },    \
        //    { UNKNOWN,UNKNOWN }
        static readonly mask_value NO_BITWISE = new mask_value(UNKNOWN,UNKNOWN);
        static readonly mask_value NO_ADDITIVE = new mask_value(UNKNOWN,UNKNOWN);


        /* slapstic 137412-101: Empire Strikes Back/Tetris */
        static readonly slapstic_data slapstic101 = new slapstic_data
        (
            /* basic banking */
            3,                              /* starting bank */
            new u16 [] { 0x0080,0x0090,0x00a0,0x00b0 },/* bank select values */

            /* alternate banking */
            new mask_value( 0x1f00,0x1e00 ),              /* 1st mask/value in sequence */
            new mask_value( 0x1fff,0x1fff ),              /* 2nd mask/value in sequence, *outside* of the range */
            new mask_value( 0x1ffc,0x1b5c ),              /* 3rd mask/value in sequence */
            new mask_value( 0x1fcf,0x0080 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            new mask_value( 0x1ff0,0x1540 ),              /* 1st mask/value */
            new mask_value( 0x1fcf,0x0080 ),              /* 2nd mask/value */
            new mask_value( 0x1ff3,0x1540 ),              /* clear bit 0 value on odd /   set bit 1 on even */
            new mask_value( 0x1ff3,0x1541 ),              /*   set bit 0 value on odd / clear bit 1 on even */
            new mask_value( 0x1ff3,0x1542 ),              /* clear bit 1 value on odd /   set bit 0 on even */
            new mask_value( 0x1ff3,0x1543 ),              /*   set bit 1 value on odd / clear bit 0 on even */
            new mask_value( 0x1ff8,0x1550 ),              /* final mask/value */

            /* additive banking */
            NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE
        );


        /* slapstic 137412-103: Marble Madness */
        static readonly slapstic_data slapstic103 = new slapstic_data
        (
            /* basic banking */
            3,                              /* starting bank */
            new u16 [] { 0x0040,0x0050,0x0060,0x0070 },/* bank select values */

            /* alternate banking */
            // Real values, to be worked on later
            new mask_value( 0x007f,0x002d ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x3d14 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3ffc,0x3d24 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3fcf,0x0040 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            new mask_value( 0x3ff0,0x34c0 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fcf,0x0040 ),              /* 2nd mask/value */
            new mask_value( 0x3ff3,0x34c0 ),              /* clear bit 0 value on odd /   set bit 1 on even */
            new mask_value( 0x3ff3,0x34c1 ),              /*   set bit 0 value on odd / clear bit 1 on even */
            new mask_value( 0x3ff3,0x34c2 ),              /* clear bit 1 value on odd /   set bit 0 on even */
            new mask_value( 0x3ff3,0x34c3 ),              /*   set bit 1 value on odd / clear bit 0 on even */
            new mask_value( 0x3ff8,0x34d0 ),              /* final mask/value in sequence */

            /* additive banking */
            NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE
        );


        /* slapstic 137412-104: Gauntlet */
        static readonly slapstic_data slapstic104 = new slapstic_data
        (
            /* basic banking */
            3,                              /* starting bank */
            new u16 [] { 0x0020,0x0028,0x0030,0x0038 },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x0069 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x3735 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3ffc,0x3764 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3fe7,0x0020 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            new mask_value( 0x3ff0,0x3d90 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fe7,0x0020 ),              /* 2nd mask/value */
            new mask_value( 0x3ff3,0x3d90 ),              /* clear bit 0 value on odd /   set bit 1 on even */
            new mask_value( 0x3ff3,0x3d91 ),              /*   set bit 0 value on odd / clear bit 1 on even */
            new mask_value( 0x3ff3,0x3d92 ),              /* clear bit 1 value on odd /   set bit 0 on even */
            new mask_value( 0x3ff3,0x3d93 ),              /*   set bit 1 value on odd / clear bit 0 on even */
            new mask_value( 0x3ff8,0x3da0 ),              /* final mask/value in sequence */

            /* additive banking */
            NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE
        );


        /* slapstic 137412-105: Indiana Jones/Paperboy */
        static readonly slapstic_data slapstic105 = new slapstic_data
        (
            /* basic banking */
            3,                              /* starting bank */
            new u16 [] { 0x0010,0x0014,0x0018,0x001c },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x003d ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x0092 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3ffc,0x00a4 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3ff3,0x0010 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            new mask_value( 0x3ff0,0x35b0 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3ff3,0x0010 ),              /* 2nd mask/value */
            new mask_value( 0x3ff3,0x35b0 ),              /* clear bit 0 value on odd /   set bit 1 on even */
            new mask_value( 0x3ff3,0x35b1 ),              /*   set bit 0 value on odd / clear bit 1 on even */
            new mask_value( 0x3ff3,0x35b2 ),              /* clear bit 1 value on odd /   set bit 0 on even */
            new mask_value( 0x3ff3,0x35b3 ),              /*   set bit 1 value on odd / clear bit 0 on even */
            new mask_value( 0x3ff8,0x35c0 ),              /* final mask/value in sequence */

            /* additive banking */
            NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE
        );


        /* slapstic 137412-106: Gauntlet II */
        static readonly slapstic_data slapstic106 = new slapstic_data
        (
            /* basic banking */
            3,                              /* starting bank */
            new u16 [] { 0x0008,0x000a,0x000c,0x000e },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x002b ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x0052 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3ffc,0x0064 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3ff9,0x0008 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            new mask_value( 0x3ff0,0x3da0 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3ff9,0x0008 ),              /* 2nd mask/value */
            new mask_value( 0x3ff3,0x3da0 ),              /* clear bit 0 value on odd /   set bit 1 on even */
            new mask_value( 0x3ff3,0x3da1 ),              /*   set bit 0 value on odd / clear bit 1 on even */
            new mask_value( 0x3ff3,0x3da2 ),              /* clear bit 1 value on odd /   set bit 0 on even */
            new mask_value( 0x3ff3,0x3da3 ),              /*   set bit 1 value on odd / clear bit 0 on even */
            new mask_value( 0x3ff8,0x3db0 ),              /* final mask/value in sequence */

            /* additive banking */
            NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE
        );


        /* slapstic 137412-107: Peter Packrat/Xybots/2p Gauntlet/720 */
        static readonly slapstic_data slapstic107 = new slapstic_data
        (
            /* basic banking */
            3,                              /* starting bank */
            new u16 [] { 0x0018,0x001a,0x001c,0x001e },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x006b ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x3d52 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3ffc,0x3d64 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3ff9,0x0018 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            new mask_value( 0x3ff0,0x00a0 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3ff9,0x0018 ),              /* 2nd mask/value */
            new mask_value( 0x3ff3,0x00a0 ),              /* clear bit 0 value on odd /   set bit 1 on even */
            new mask_value( 0x3ff3,0x00a1 ),              /*   set bit 0 value on odd / clear bit 1 on even */
            new mask_value( 0x3ff3,0x00a2 ),              /* clear bit 1 value on odd /   set bit 0 on even */
            new mask_value( 0x3ff3,0x00a3 ),              /*   set bit 1 value on odd / clear bit 0 on even */
            new mask_value( 0x3ff8,0x00b0 ),              /* final mask/value in sequence */

            /* additive banking */
            NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE
        );


        /* slapstic 137412-108: Road Runner/Super Sprint */
        static readonly slapstic_data slapstic108 = new slapstic_data
        (
            /* basic banking */
            3,                              /* starting bank */
            new u16 [] { 0x0028,0x002a,0x002c,0x002e },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x001f ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x3772 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3ffc,0x3764 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3ff9,0x0028 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            new mask_value( 0x3ff0,0x0060 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3ff9,0x0028 ),              /* 2nd mask/value */
            new mask_value( 0x3ff3,0x0060 ),              /* clear bit 0 value on odd /   set bit 1 on even */
            new mask_value( 0x3ff3,0x0061 ),              /*   set bit 0 value on odd / clear bit 1 on even */
            new mask_value( 0x3ff3,0x0062 ),              /* clear bit 1 value on odd /   set bit 0 on even */
            new mask_value( 0x3ff3,0x0063 ),              /*   set bit 1 value on odd / clear bit 0 on even */
            new mask_value( 0x3ff8,0x0070 ),              /* final mask/value in sequence */

            /* additive banking */
            NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE
        );


        /* slapstic 137412-109: Championship Sprint/Road Blasters */
        static readonly slapstic_data slapstic109 = new slapstic_data
        (
            /* basic banking */
            3,                              /* starting bank */
            new u16 [] { 0x0008,0x000a,0x000c,0x000e },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x002b ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x0052 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3ffc,0x0064 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3ff9,0x0008 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            new mask_value( 0x3ff0,0x3da0 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3ff9,0x0008 ),              /* 2nd mask/value */
            new mask_value( 0x3ff3,0x3da0 ),              /* clear bit 0 value on odd /   set bit 1 on even */
            new mask_value( 0x3ff3,0x3da1 ),              /*   set bit 0 value on odd / clear bit 1 on even */
            new mask_value( 0x3ff3,0x3da2 ),              /* clear bit 1 value on odd /   set bit 0 on even */
            new mask_value( 0x3ff3,0x3da3 ),              /*   set bit 1 value on odd / clear bit 0 on even */
            new mask_value( 0x3ff8,0x3db0 ),              /* final mask/value in sequence */

            /* additive banking */
            NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE
        );


        /* slapstic 137412-110: Road Blasters/APB */
        static readonly slapstic_data slapstic110 = new slapstic_data
        (
            /* basic banking */
            3,                              /* starting bank */
            new u16 [] { 0x0040,0x0050,0x0060,0x0070 },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x002d ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x3d14 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3ffc,0x3d24 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3fcf,0x0040 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            new mask_value( 0x3ff0,0x34c0 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fcf,0x0040 ),              /* 2nd mask/value */
            new mask_value( 0x3ff3,0x34c0 ),              /* clear bit 0 value on odd /   set bit 1 on even */
            new mask_value( 0x3ff3,0x34c1 ),              /*   set bit 0 value on odd / clear bit 1 on even */
            new mask_value( 0x3ff3,0x34c2 ),              /* clear bit 1 value on odd /   set bit 0 on even */
            new mask_value( 0x3ff3,0x34c3 ),              /*   set bit 1 value on odd / clear bit 0 on even */
            new mask_value( 0x3ff8,0x34d0 ),              /* final mask/value in sequence */

            /* additive banking */
            NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE
        );



        /*************************************
         *
         *  Slapstic-2 definitions
         *
         *************************************/

        /* slapstic 137412-111: Pit Fighter (Aug 09, 1990 to Aug 22, 1990) */
        static readonly slapstic_data slapstic111 = new slapstic_data
        (
            /* basic banking */
            0,                              /* starting bank */
            new u16 [] { 0x0042,0x0052,0x0062,0x0072 },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x000a ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x28a4 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x0784,0x0080 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3fcf,0x0042 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE,

            /* additive banking */
            new mask_value( 0x3fff,0x00a1 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x00a2 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3c4f,0x284d ),              /* +1 mask/value */
            new mask_value( 0x3a5f,0x285d ),              /* +2 mask/value */
            new mask_value( 0x3ff8,0x2800 )               /* final mask/value in sequence */
        );


        /* slapstic 137412-112: Pit Fighter (Aug 22, 1990 to Oct 01, 1990) */
        static readonly slapstic_data slapstic112 = new slapstic_data
        (
            /* basic banking */
            0,                              /* starting bank */
            new u16 [] { 0x002c,0x003c,0x006c,0x007c },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x0014 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x29a0 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x0073,0x0010 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3faf,0x002c ),              /* 4th mask/value in sequence */
            2,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE,

            /* additive banking */
            new mask_value( 0x3fff,0x2dce ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x2dcf ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3def,0x15e2 ),              /* +1 mask/value */
            new mask_value( 0x3fbf,0x15a2 ),              /* +2 mask/value */
            new mask_value( 0x3ffc,0x1450 )               /* final mask/value in sequence */
        );


        /* slapstic 137412-113: Pit Fighter (Oct 09, 1990 to Oct 12, 1990) */
        static readonly slapstic_data slapstic113 = new slapstic_data
        (
            /* basic banking */
            0,                              /* starting bank */
            new u16 [] { 0x0008,0x0018,0x0028,0x0038 },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x0059 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x11a5 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x0860,0x0800 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3fcf,0x0008 ),              /* 4th mask/value in sequence */
            3,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE,

            /* additive banking */
            new mask_value( 0x3fff,0x049b ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x049c ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3fcf,0x3ec7 ),              /* +1 mask/value */
            new mask_value( 0x3edf,0x3ed7 ),              /* +2 mask/value */
            new mask_value( 0x3fff,0x3fb2 )               /* final mask/value in sequence */
        );


        /* slapstic 137412-114: Pit Fighter (Nov 01, 1990 and later) */
        static readonly slapstic_data slapstic114 = new slapstic_data
        (
            /* basic banking */
            0,                              /* starting bank */
            new u16 [] { 0x0040,0x0048,0x0050,0x0058 },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x0016 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x24de ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3871,0x0000 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3fe7,0x0040 ),              /* 4th mask/value in sequence */
            1,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE,

            /* additive banking */
            new mask_value( 0x3fff,0x0ab7 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x0ab8 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3f63,0x0d40 ),              /* +1 mask/value */
            new mask_value( 0x3fd9,0x0dc8 ),              /* +2 mask/value */
            new mask_value( 0x3fff,0x0ab0 )               /* final mask/value in sequence */
        );


        /* slapstic 137412-115: Race Drivin' DSK board */
        static readonly slapstic_data slapstic115 = new slapstic_data
        (
            /* basic banking */
            0,                              /* starting bank */
            new u16 [] { 0x0020,0x0022,0x0024,0x0026 },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x0054 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x3e01 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3879,0x0029 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3ff9,0x0020 ),              /* 4th mask/value in sequence */
            1,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE,

            /* additive banking */
            new mask_value( 0x3fff,0x2591 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x2592 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3fe6,0x3402 ),              /* +1 mask/value */
            new mask_value( 0x3fb4,0x3410 ),              /* +2 mask/value */
            new mask_value( 0x3fff,0x34a2 )               /* final mask/value in sequence */
        );


        /* slapstic 137412-116: Hydra */
        static readonly slapstic_data slapstic116 = new slapstic_data
        (
            /* basic banking */
            0,                              /* starting bank */
            new u16 [] { 0x0044,0x004c,0x0054,0x005c },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x0069 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x2bab ),              /* 2nd mask/value in sequence */
            new mask_value( 0x387c,0x0808 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3fe7,0x0044 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE,

            /* additive banking */
            new mask_value( 0x3fff,0x3f7c ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x3f7d ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3db2,0x3c12 ),              /* +1 mask/value */
            new mask_value( 0x3fe3,0x3e43 ),              /* +2 mask/value */
            new mask_value( 0x3fff,0x2ba8 )               /* final mask/value in sequence */
        );


        /* slapstic 137412-117: Race Drivin' main board */
        static readonly slapstic_data slapstic117 = new slapstic_data
        (
            /* basic banking */
            0,                              /* starting bank */
            new u16 [] { 0x0008,0x001a,0x002c,0x003e },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x007d ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x3580 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x0079,0x0020 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3fc9,0x0008 ),              /* 4th mask/value in sequence */
            1,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE,

            /* additive banking */
            new mask_value( 0x3fff,0x0676 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x0677 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3e62,0x1a42 ),              /* +1 mask/value */
            new mask_value( 0x3e35,0x1a11 ),              /* +2 mask/value */
            new mask_value( 0x3fff,0x1a42 )               /* final mask/value in sequence */
        );


        /* slapstic 137412-118: Rampart/Vindicators II */
        static readonly slapstic_data slapstic118 = new slapstic_data
        (
            /* basic banking */
            0,                              /* starting bank */
            new u16 [] { 0x0014,0x0034,0x0054,0x0074 },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x0002 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x1950 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x0067,0x0020 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3f9f,0x0014 ),              /* 4th mask/value in sequence */
            3,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE, NO_BITWISE,

            /* additive banking */
            new mask_value( 0x3fff,0x1958 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x1959 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3f73,0x3052 ),              /* +1 mask/value */
            new mask_value( 0x3f67,0x3042 ),              /* +2 mask/value */
            new mask_value( 0x3ff8,0x30e0 )               /* final mask/value in sequence */
        );


        /*************************************
         *
         *  Master slapstic table
         *
         *************************************/
        /* master table */
        static readonly slapstic_data [] slapstic_table = new slapstic_data[]
        {
            slapstic101,
            null,      /* never seen */
            slapstic103,
            slapstic104,
            slapstic105,
            slapstic106,
            slapstic107,
            slapstic108,
            slapstic109,
            slapstic110,
            slapstic111,
            slapstic112,
            slapstic113,
            slapstic114,
            slapstic115,
            slapstic116,
            slapstic117,
            slapstic118
        };


        struct test
        {
            offs_t m_m;
            offs_t m_v;


            //test() : m_m(0), m_v(0) {}
            public test(offs_t m, offs_t v) { m_m = m; m_v = v; }


            //bool operator()(offs_t a) const { return (a & m_m) == m_v; }
            public bool op(offs_t a) { return (a & m_m) == m_v; }
        }


        struct checker
        {
            offs_t m_range_mask;
            offs_t m_range_value;
            offs_t m_shift;
            offs_t m_input_mask;


            public checker(offs_t start, offs_t end, offs_t mirror, int data_width, int address_lines)
            {
                m_range_mask = ~((end - start) | mirror);
                m_range_value = start;
                if ((m_range_value & ~m_range_mask) != 0)
                    g.fatalerror("The slapstic range {0}-{1} mirror {2} is not masking friendly", start, end, mirror);
                m_shift = data_width == 16 ? 1U : 0U;
                m_input_mask = g.make_bitmask32(address_lines) << (int)m_shift;
            }


            public test test_in(mask_value mv)
            {
                return new test(m_range_mask | ((u16)(mv.mask << (int)m_shift)), m_range_value | ((u16)(mv.value << (int)m_shift)));
            }


            public test test_any(mask_value mv)
            {
                return new test((u16)(mv.mask << (int)m_shift), (u16)(mv.value << (int)m_shift));
            }


            public test test_inside()
            {
                return new test(m_range_mask, m_range_value);
            }


            public test test_reset()
            {
                return new test(m_range_mask | m_input_mask, m_range_value);
            }


            public test test_bank(u16 b)
            {
                return new test(m_range_mask | m_input_mask, m_range_value | ((u16)(b << (int)m_shift)));
            }
        }


        abstract class state
        {
            protected atari_slapstic_device m_sl;


            protected state(atari_slapstic_device sl) { m_sl = sl; }
            //virtual ~state() = default;


            public abstract void test(offs_t addr);
            public abstract u8 state_id();
        }


        class idle : state
        {
            test m_reset;


            public idle(atari_slapstic_device sl, checker check, slapstic_data data)
                : base(sl)
            {
                m_reset = check.test_reset();
            }


            public override void test(offs_t addr)
            {
                if (m_reset.op(addr))
                {
                    m_sl.logerror("reset ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
            }


            public override u8 state_id() { return S_IDLE; }
        }


        class active_101_102 : state
        {
            test [] m_bank = new test[4];
            test m_alt;
            test m_bit;


            public active_101_102(atari_slapstic_device sl, checker check, slapstic_data data)
                : base(sl)
            {
                for (int i = 0; i != 4; i++)
                    m_bank[i] = check.test_bank(data.bank[i]);
                m_alt = check.test_in(data.alt1);
                m_bit = check.test_in(data.bit1);
            }


            public override void test(offs_t addr)
            {
                if (m_bank[0].op(addr))
                {
                    m_sl.logerror("direct switch bank 0 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(0);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_bank[1].op(addr))
                {
                    m_sl.logerror("direct switch bank 1 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(1);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_bank[2].op(addr))
                {
                    m_sl.logerror("direct switch bank 2 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(2);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_bank[3].op(addr))
                {
                    m_sl.logerror("direct switch bank 3 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(3);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_alt.op(addr))
                {
                    m_sl.logerror("alt start ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_alt_valid;
                }
                else if (m_bit.op(addr))
                {
                    m_sl.logerror("bitwise start ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_bit_load;
                }
            }


            public override u8 state_id() { return S_ACTIVE; }
        }


        class active_103_110 : state
        {
            test [] m_bank = new test[4];
            test m_alt;
            test m_bit;


            public active_103_110(atari_slapstic_device sl, checker check, slapstic_data data)
                : base(sl)
            {
                for (int i = 0; i != 4; i++)
                    m_bank[i] = check.test_bank(data.bank[i]);
                m_alt = check.test_any(data.alt1);
                m_bit = check.test_in(data.bit1);
            }


            public override void test(offs_t addr)
            {
                if (m_bank[0].op(addr))
                {
                    m_sl.logerror("direct switch bank 0 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(0);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_bank[1].op(addr))
                {
                    m_sl.logerror("direct switch bank 1 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(1);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_bank[2].op(addr))
                {
                    m_sl.logerror("direct switch bank 2 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(2);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_bank[3].op(addr))
                {
                    m_sl.logerror("direct switch bank 3 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(3);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_alt.op(addr))
                {
                    m_sl.logerror("alt start ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_alt_valid;
                }
                else if (m_bit.op(addr))
                {
                    m_sl.logerror("bitwise start ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_bit_load;
                }
            }


            public override u8 state_id() { return S_ACTIVE; }
        }


        class active_111_118 : state
        {
            test [] m_bank = new test[4];
            test m_alt;
            test m_add;


            public active_111_118(atari_slapstic_device sl, checker check, slapstic_data data)
                : base(sl)
            {
                for (int i = 0; i != 4; i++)
                    m_bank[i] = check.test_bank(data.bank[i]);
                m_alt = check.test_any(data.alt1);
                m_add = check.test_in(data.add1);
            }


            public override void test(offs_t addr)
            {
                if (m_bank[0].op(addr))
                {
                    m_sl.logerror("direct switch bank 0 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(0);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_bank[1].op(addr))
                {
                    m_sl.logerror("direct switch bank 1 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(1);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_bank[2].op(addr))
                {
                    m_sl.logerror("direct switch bank 2 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(2);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_bank[3].op(addr))
                {
                    m_sl.logerror("direct switch bank 3 ({0})\n", m_sl.machine().describe_context());
                    m_sl.change_bank(3);
                    m_sl.m_state = m_sl.m_s_idle;
                }
                else if (m_alt.op(addr))
                {
                    m_sl.logerror("alt start ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_alt_valid;
                }
                else if (m_add.op(addr))
                {
                    m_sl.logerror("add start ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_add_load;
                }
            }


            public override u8 state_id() { return S_ACTIVE; }
        }


        class alt_valid_101_102 : state
        {
            test m_reset;
            test m_inside;
            test m_valid;


            public alt_valid_101_102(atari_slapstic_device sl, checker check, slapstic_data data)
                : base(sl)
            {
                m_reset = check.test_reset();
                m_inside = check.test_inside();
                m_valid = check.test_any(data.alt2);
            }


            public override void test(offs_t addr)
            {
                if (m_reset.op(addr))
                {
                    m_sl.logerror("reset ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
                else if (!m_inside.op(addr) && m_valid.op(addr))
                {
                    m_sl.logerror("alt valid ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_alt_select;
                }
                else
                {
                    m_sl.logerror("alt sequence break at valid ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
            }


            public override u8 state_id() { return S_ALT_VALID; }
        }


        class alt_valid_103_110 : state
        {
            test m_reset;
            test m_valid;


            public alt_valid_103_110(atari_slapstic_device sl, checker check, slapstic_data data)
                : base(sl)
            {
                m_reset = check.test_reset();
                m_valid = check.test_in(data.alt2);
            }


            public override void test(offs_t addr)
            {
                if (m_reset.op(addr))
                {
                    m_sl.logerror("reset ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
                else if (m_valid.op(addr))
                {
                    m_sl.logerror("alt valid ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_alt_select;
                }
                else
                {
                    m_sl.logerror("alt sequence break at valid ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
            }


            public override u8 state_id() { return S_ALT_VALID; }
        }


        class alt_valid_111_118 : state
        {
            test m_reset;
            test m_valid;
            test m_add;


            public alt_valid_111_118(atari_slapstic_device sl, checker check, slapstic_data data)
                : base(sl)
            {
                m_reset = check.test_reset();
                m_valid = check.test_in(data.alt2);
                m_add   = check.test_in(data.add1);
            }


            public override void test(offs_t addr)
            {
                if (m_reset.op(addr))
                {
                    m_sl.logerror("reset ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
                else if (m_valid.op(addr))
                {
                    m_sl.logerror("alt valid ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_alt_select;
                }
                else if (m_add.op(addr))
                {
                    m_sl.logerror("alt switch to add ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_add_load;
                }
                else
                {
                    m_sl.logerror("alt sequence break at valid ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
            }


            public override u8 state_id() { return S_ALT_VALID; }
        }


        class alt_select_101_110 : state
        {
            test m_reset;
            test m_select;
            int m_shift;


            public alt_select_101_110(atari_slapstic_device sl, checker check, slapstic_data data, int shift)
                : base(sl)
            {
                m_reset  = check.test_reset();
                m_select = check.test_in(data.alt3);
                m_shift  = shift + data.altshift;
            }


            public override void test(offs_t addr)
            {
                if (m_reset.op(addr))
                {
                    m_sl.logerror("reset ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
                else if (m_select.op(addr))
                {
                    m_sl.logerror("alt select ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_loaded_bank = (u8)((addr >> m_shift) & 3);
                    m_sl.m_state = m_sl.m_s_alt_commit;
                }
                else
                {
                    m_sl.logerror("alt sequence break at select ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
            }


            public override u8 state_id() { return S_ALT_SELECT; }
        }


        class alt_select_111_118 : state
        {
            test m_reset;
            test m_select;
            int m_shift;


            public alt_select_111_118(atari_slapstic_device sl, checker check, slapstic_data data, int shift)
                : base(sl)
            {
                m_reset  = check.test_reset();
                m_select = check.test_any(data.alt3);
                m_shift  = shift + data.altshift;
            }


            public override void test(offs_t addr)
            {
                if (m_reset.op(addr))
                {
                    m_sl.logerror("reset ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
                else if (m_select.op(addr))
                {
                    m_sl.logerror("alt select ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_loaded_bank = (u8)((addr >> m_shift) & 3);
                    m_sl.m_state = m_sl.m_s_alt_commit;
                }
                else
                {
                    m_sl.logerror("alt sequence break at select ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
            }


            public override u8 state_id() { return S_ALT_SELECT; }
        }


        class alt_commit : state
        {
            test m_reset;
            test m_commit;


            public alt_commit(atari_slapstic_device sl, checker check, slapstic_data data)
                : base(sl)
            {
                m_reset  = check.test_reset();
                m_commit = check.test_any(data.alt4);
            }


            public override void test(offs_t addr)
            {
                if (m_reset.op(addr))
                {
                    m_sl.logerror("reset ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
                else if (m_commit.op(addr))
                {
                    m_sl.logerror("alt/add commit ({0})\n", m_sl.machine().describe_context());
                    m_sl.commit_bank();
                    m_sl.m_state = m_sl.m_s_idle;
                }
            }


            public override u8 state_id() { return S_ALT_COMMIT; }
        }


        class bit_load : state
        {
            test m_reset;
            test m_load;


            public bit_load(atari_slapstic_device sl, checker check, slapstic_data data)
                : base(sl)
            {
                m_reset = check.test_reset();
                m_load  = check.test_in(data.bit2);
            }


            public override void test(offs_t addr)
            {
                if (m_reset.op(addr))
                {
                    m_sl.logerror("reset ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
                else if (m_load.op(addr))
                {
                    m_sl.logerror("bitwise load ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_loaded_bank = m_sl.m_current_bank;
                    m_sl.m_state = m_sl.m_s_bit_set_odd;
                }
            }


            public override u8 state_id() { return S_BIT_LOAD; }
        }


        class bit_set : state
        {
            test m_reset;
            test m_set0;
            test m_clear0;
            test m_set1;
            test m_clear1;
            test m_commit;
            bool m_is_odd;


            public bit_set(atari_slapstic_device sl, checker check, slapstic_data data, bool is_odd)
                : base(sl)
            {
                m_is_odd = is_odd;


                m_reset  = check.test_reset();
                m_clear0 = check.test_in(is_odd ? data.bit3c0 : data.bit3s1);
                m_set0   = check.test_in(is_odd ? data.bit3s0 : data.bit3c1);
                m_clear1 = check.test_in(is_odd ? data.bit3c1 : data.bit3s0);
                m_set1   = check.test_in(is_odd ? data.bit3s1 : data.bit3c0);
                m_commit = check.test_in(data.bit4);
            }


            public override void test(offs_t addr)
            {
                if (m_reset.op(addr))
                {
                    m_sl.logerror("reset ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
                else if (m_clear0.op(addr))
                {
                    m_sl.logerror("bitwise clear 0 ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_loaded_bank &= unchecked((u8)~1);
                    m_sl.m_state = m_is_odd ? m_sl.m_s_bit_set_even : m_sl.m_s_bit_set_odd;
                }
                else if (m_set0.op(addr))
                {
                    m_sl.logerror("bitwise set 0 ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_loaded_bank |= 1;
                    m_sl.m_state = m_is_odd ? m_sl.m_s_bit_set_even : m_sl.m_s_bit_set_odd;
                }
                else if (m_clear1.op(addr))
                {
                    m_sl.logerror("bitwise clear 1 ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_loaded_bank &= unchecked((u8)~2);
                    m_sl.m_state = m_is_odd ? m_sl.m_s_bit_set_even : m_sl.m_s_bit_set_odd;
                }
                else if (m_set1.op(addr))
                {
                    m_sl.logerror("bitwise set 1 ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_loaded_bank |= 2;
                    m_sl.m_state = m_is_odd ? m_sl.m_s_bit_set_even : m_sl.m_s_bit_set_odd;
                }
                else if (m_commit.op(addr))
                {
                    m_sl.logerror("bitwise commit {0} ({1})\n", m_sl.m_loaded_bank, m_sl.machine().describe_context());
                    m_sl.commit_bank();
                    m_sl.m_state = m_sl.m_s_idle;
                }
            }


            public override u8 state_id() { return m_is_odd ? S_BIT_SET_ODD : S_BIT_SET_EVEN; }
        }


        class add_load : state
        {
            test m_reset;
            test m_load;


            public add_load(atari_slapstic_device sl, checker check, slapstic_data data)
                : base(sl)
            {
                m_reset = check.test_reset();
                m_load  = check.test_in(data.add2);
            }


            public override void test(offs_t addr)
            {
                if (m_reset.op(addr))
                {
                    m_sl.logerror("reset ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
                else if (m_load.op(addr))
                {
                    m_sl.logerror("add load ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_loaded_bank = m_sl.m_current_bank;
                    m_sl.m_state = m_sl.m_s_add_set;
                }
                else
                {
                    m_sl.logerror("add sequence break at load ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
            }


            public override u8 state_id() { return S_ADD_LOAD; }
        }


        class add_set : state
        {
            test m_reset;
            test m_add1;
            test m_add2;
            test m_end;


            public add_set(atari_slapstic_device sl, checker check, slapstic_data data)
                : base(sl)
            {
                m_reset = check.test_reset();
                m_add1  = check.test_in(data.addplus1);
                m_add2  = check.test_in(data.addplus2);
                m_end   = check.test_in(data.add3);
            }


            public override void test(offs_t addr)
            {
                if (m_reset.op(addr))
                {
                    m_sl.logerror("reset ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_active;
                }
                else if (m_add1.op(addr))
                {
                    m_sl.logerror("add +1 ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_loaded_bank = (u8)((m_sl.m_loaded_bank + 1) & 3);
                }
                else if (m_add2.op(addr))
                {
                    m_sl.logerror("add +2 ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_loaded_bank = (u8)((m_sl.m_loaded_bank + 2) & 3);
                }
                else if (m_end.op(addr))
                {
                    m_sl.logerror("add end ({0})\n", m_sl.machine().describe_context());
                    m_sl.m_state = m_sl.m_s_alt_commit;
                }
            }


            public override u8 state_id() { return S_ADD_SET; }
        }


        int m_chipnum;

        optional_memory_bank m_bank;
        memory_view m_view;
        optional_address_space m_space;
        offs_t m_start;
        offs_t m_end;
        offs_t m_mirror;

        state m_s_idle;
        state m_s_active;

        state m_s_alt_valid;
        state m_s_alt_select;
        state m_s_alt_commit;

        state m_s_bit_load;
        state m_s_bit_set_odd;
        state m_s_bit_set_even;

        state m_s_add_load;
        state m_s_add_set;

        state m_state;

        //enum {
        const u8 S_IDLE         = 0;
        const u8 S_ACTIVE       = 1;
        const u8 S_ALT_VALID    = 2;
        const u8 S_ALT_SELECT   = 3;
        const u8 S_ALT_COMMIT   = 4;
        const u8 S_BIT_LOAD     = 5;
        const u8 S_BIT_SET_ODD  = 6;
        const u8 S_BIT_SET_EVEN = 7;
        const u8 S_ADD_LOAD     = 8;
        const u8 S_ADD_SET      = 9;
        //};
        u8 m_saved_state;

        u8 m_current_bank;
        u8 m_loaded_bank;


        atari_slapstic_device(machine_config mconfig, string tag, device_t owner, int chipnum)
            : this(mconfig, tag, owner, (u32)0)
        {
            m_chipnum = chipnum;
        }


        atari_slapstic_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, SLAPSTIC, tag, owner, clock)
        {
            m_bank = new optional_memory_bank(this, finder_base.DUMMY_TAG);
            m_view = null;
            m_space = new optional_address_space(this, finder_base.DUMMY_TAG, -1);
            m_start = 0;
            m_end = 0;
            m_mirror = 0;
            m_saved_state = S_IDLE;
            m_current_bank = 0;
            m_loaded_bank = 0;
        }


        public void atari_slapstic_device_after_ctor(int chipnum)
        {
            m_chipnum = chipnum;
        }


        //template <typename T> void set_bank(T &&tag) { m_bank.set_tag(std::forward<T>(tag)); }


        public void set_view(memory_view view) { m_view = view; }


        //template <typename T>
        public void set_range(finder_base tag, int index, offs_t start, offs_t end, offs_t mirror)  //template <typename T> void set_range(T &&tag, int index, offs_t start, offs_t end, offs_t mirror) {
        {
            m_space.set_tag(tag, index);  //m_space.set_tag(std::forward<T>(tag), index);
            m_start = start;
            m_end = end;
            m_mirror = mirror;
        }


        //void set_chipnum(int chipnum) { m_chipnum = chipnum; }


        /*************************************
         *
         *  Initialization
         *
         *************************************/
        protected override void device_start()
        {
            /* save state */
            save_item(g.NAME(new { m_current_bank }));
            save_item(g.NAME(new { m_loaded_bank }));
            save_item(g.NAME(new { m_saved_state }));

            /* Address space tap installation */
            if (m_space.op[0].data_width() == 16)
            {
                m_space.op[0].install_readwrite_tap(0, m_space.op[0].addrmask(), 0, "slapstic",
                                               (offs_t offset, ref u16 data, u16 mem_mask) => { if (!machine().side_effects_disabled()) m_state.test(offset); },
                                               (offs_t offset, ref u16 data, u16 mem_mask) => { if (!machine().side_effects_disabled()) m_state.test(offset); });
            }
            else
            {
                m_space.op[0].install_readwrite_tap(0, m_space.op[0].addrmask(), 0, "slapstic",
                                               (offs_t offset, ref u8 data, u8 mem_mask) => { if (!machine().side_effects_disabled()) m_state.test(offset); },
                                               (offs_t offset, ref u8 data, u8 mem_mask) => { if (!machine().side_effects_disabled()) m_state.test(offset); });
            }

            checker check = new checker(m_start, m_end, m_mirror, m_space.op[0].data_width(), m_chipnum == 101 ? 13 : 14);
            var info = slapstic_table[m_chipnum - 101];

            m_s_idle = new idle(this, check, info);

            if (m_chipnum <= 102)
            {
                m_s_active = new active_101_102(this, check, info);
                m_s_alt_valid = new alt_valid_101_102(this, check, info);

            }
            else if (m_chipnum <= 110)
            {
                m_s_active = new active_103_110(this, check, info);
                m_s_alt_valid = new alt_valid_103_110(this, check, info);

            }
            else
            {
                m_s_active = new active_111_118(this, check, info);
                m_s_alt_valid = new alt_valid_111_118(this, check, info);
            }

            if (m_chipnum <= 110)
                m_s_alt_select = new alt_select_101_110(this, check, info, m_space.op[0].data_width() == 16 ? 1 : 0);
            else
                m_s_alt_select = new alt_select_111_118(this, check, info, m_space.op[0].data_width() == 16 ? 1 : 0);

            m_s_alt_commit = new alt_commit(this, check, info);

            if (m_chipnum <= 110)
            {
                m_s_bit_load = new bit_load(this, check, info);
                m_s_bit_set_odd  = new bit_set(this, check, info, true);
                m_s_bit_set_even = new bit_set(this, check, info, false);
            }

            if (m_chipnum >= 111)
            {
                m_s_add_load = new add_load(this, check, info);
                m_s_add_set  = new add_set(this, check, info);
            }

            m_state = m_s_idle;
        }


        protected override void device_reset()
        {
            /* reset the chip */
            m_state = m_s_idle;

            /* the 111 and later chips seem to reset to bank 0 */
            change_bank(slapstic_table[m_chipnum - 101].bankstart);
        }


        protected override void device_validity_check(validity_checker valid)
        {
            // only a small number of chips are known to exist
            if (m_chipnum < 101 || m_chipnum > 118 || slapstic_table[m_chipnum - 101] == null)
                g.osd_printf_error("Unknown slapstic number: {0}\n", m_chipnum);
        }


        protected override void device_pre_save()
        {
            m_saved_state = m_state.state_id();
        }


        protected override void device_post_load()
        {
            switch (m_saved_state)
            {
                case S_IDLE:         m_state = m_s_idle; break;
                case S_ACTIVE:       m_state = m_s_active; break;
                case S_ALT_VALID:    m_state = m_s_alt_valid; break;
                case S_ALT_SELECT:   m_state = m_s_alt_select; break;
                case S_ALT_COMMIT:   m_state = m_s_alt_commit; break;
                case S_BIT_LOAD:     m_state = m_s_bit_load; break;
                case S_BIT_SET_ODD:  m_state = m_s_bit_set_odd; break;
                case S_BIT_SET_EVEN: m_state = m_s_bit_set_even; break;
                case S_ADD_LOAD:     m_state = m_s_add_load; break;
                case S_ADD_SET:      m_state = m_s_add_set; break;
            }
        }


        void change_bank(int bank)
        {
            logerror("current bank {0}\n", bank);
            m_current_bank = (u8)bank;
            if (m_bank.bool_)
                m_bank.op[0].set_entry(m_current_bank);
            if (m_view != null)
                m_view.select(m_current_bank);
        }


        void commit_bank()
        {
            change_bank(m_loaded_bank);
        }
    }
}
