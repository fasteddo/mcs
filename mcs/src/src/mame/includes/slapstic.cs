// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    /*************************************************************************
        Atari Slapstic decoding helper
    **************************************************************************
        For more information on the slapstic, see slapstic.html, or go to
        http://www.aarongiles.com/slapstic.html
    *************************************************************************/

    /*************************************
     *
     *  Structure of slapstic params
     *
     *************************************/
    struct mask_value
    {
        public int mask;
        public int value;

        public mask_value(int mask, int value) { this.mask = mask; this.value = value; }
    }


    class slapstic_data
    {
        public int bankstart;
        public int [] bank = new int[4];

        public mask_value alt1;
        public mask_value alt2;
        public mask_value alt3;
        public mask_value alt4;
        public int altshift;

        public mask_value bit1;
        public mask_value bit2c0;
        public mask_value bit2s0;
        public mask_value bit2c1;
        public mask_value bit2s1;
        public mask_value bit3;

        public mask_value add1;
        public mask_value add2;
        public mask_value addplus1;
        public mask_value addplus2;
        public mask_value add3;

        public slapstic_data() { }
        public slapstic_data
        (
            int bankstart,
            int [] bank,
            mask_value alt1,
            mask_value alt2,
            mask_value alt3,
            mask_value alt4,
            int altshift,
            mask_value bit1,
            mask_value bit2c0,
            mask_value bit2s0,
            mask_value bit2c1,
            mask_value bit2s1,
            mask_value bit3,
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
            this.bit2c0 = bit2c0;
            this.bit2s0 = bit2s0;
            this.bit2c1 = bit2c1;
            this.bit2s1 = bit2s1;
            this.bit3 = bit3;
            this.add1 = add1;
            this.add2 = add2;
            this.addplus1 = addplus1;
            this.addplus2 = addplus2;
            this.add3 = add3;
        }
    }


    public class atari_slapstic_device : device_t
    {
        //DEFINE_DEVICE_TYPE(SLAPSTIC, atari_slapstic_device, "slapstic", "Atari Slapstic")
        static device_t device_creator_atari_slapstic_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new atari_slapstic_device(mconfig, tag, owner, clock); }
        public static readonly device_type SLAPSTIC = DEFINE_DEVICE_TYPE(device_creator_atari_slapstic_device, "slapstic", "Atari Slapstic");


        const bool LOG_SLAPSTIC = false;  //#define LOG_SLAPSTIC    (0)


        /*************************************
         *
         *  Shorthand
         *
         *************************************/

        const int UNKNOWN = int.MaxValue;  //#define UNKNOWN 0xffff
        //#define NO_BITWISE          \
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
        static readonly mask_value NO_ADDITIVE = new mask_value(UNKNOWN, UNKNOWN);

        static bool MATCHES_MASK_VALUE(offs_t val, mask_value maskval) { return (val & maskval.mask) == maskval.value; }  //#define MATCHES_MASK_VALUE(val, maskval)    (((val) & (maskval).mask) == (maskval).value)


        /*************************************
         *
         *  Constants
         *
         *************************************/
        //enum slapstic_state
        //{
        const uint8_t DISABLED   = 0;
        const uint8_t ENABLED    = 1;
        const uint8_t ALTERNATE1 = 2;
        const uint8_t ALTERNATE2 = 3;
        const uint8_t ALTERNATE3 = 4;
        const uint8_t BITWISE1   = 5;
        const uint8_t BITWISE2   = 6;
        const uint8_t BITWISE3   = 7;
        const uint8_t ADDITIVE1  = 8;
        const uint8_t ADDITIVE2  = 9;
        const uint8_t ADDITIVE3  = 10;
        //}


        /*************************************
         *
         *  Slapstic definitions
         *
         *************************************/

        static readonly slapstic_data slapstic101 = null;
        static readonly slapstic_data slapstic103 = null;
        static readonly slapstic_data slapstic104 = null;

        /* slapstic 137412-105: Indiana Jones/Paperboy (confirmed) */
        static readonly slapstic_data slapstic105 = new slapstic_data
        (
            /* basic banking */
            3,                              /* starting bank */
            new int [] { 0x0010,0x0014,0x0018,0x001c },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x003d ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x0092 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3ffc,0x00a4 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3ff3,0x0010 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            new mask_value( 0x3ff0,0x35b0 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3ff3,0x35b0 ),              /* clear bit 0 value */
            new mask_value( 0x3ff3,0x35b1 ),              /*   set bit 0 value */
            new mask_value( 0x3ff3,0x35b2 ),              /* clear bit 1 value */
            new mask_value( 0x3ff3,0x35b3 ),              /*   set bit 1 value */
            new mask_value( 0x3ff8,0x35c0 ),              /* final mask/value in sequence */

            /* additive banking */
            NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE
        );

        static readonly slapstic_data slapstic106 = null;

        /* slapstic 137412-107: Peter Packrat/Xybots/2p Gauntlet/720 (confirmed) */
        static readonly slapstic_data slapstic107 = new slapstic_data
        (
            /* basic banking */
            3,                              /* starting bank */
            new int [] { 0x0018,0x001a,0x001c,0x001e },/* bank select values */

            /* alternate banking */
            new mask_value( 0x007f,0x006b ),              /* 1st mask/value in sequence */
            new mask_value( 0x3fff,0x3d52 ),              /* 2nd mask/value in sequence */
            new mask_value( 0x3ffc,0x3d64 ),              /* 3rd mask/value in sequence */
            new mask_value( 0x3ff9,0x0018 ),              /* 4th mask/value in sequence */
            0,                              /* shift to get bank from 3rd */

            /* bitwise banking */
            new mask_value( 0x3ff0,0x00a0 ),              /* 1st mask/value in sequence */
            new mask_value( 0x3ff3,0x00a0 ),              /* clear bit 0 value */
            new mask_value( 0x3ff3,0x00a1 ),              /*   set bit 0 value */
            new mask_value( 0x3ff3,0x00a2 ),              /* clear bit 1 value */
            new mask_value( 0x3ff3,0x00a3 ),              /*   set bit 1 value */
            new mask_value( 0x3ff8,0x00b0 ),              /* final mask/value in sequence */

            /* additive banking */
            NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE, NO_ADDITIVE
        );

        static readonly slapstic_data slapstic108 = null;
        static readonly slapstic_data slapstic109 = null;
        static readonly slapstic_data slapstic110 = null;
        static readonly slapstic_data slapstic111 = null;
        static readonly slapstic_data slapstic112 = null;
        static readonly slapstic_data slapstic113 = null;
        static readonly slapstic_data slapstic114 = null;
        static readonly slapstic_data slapstic115 = null;
        static readonly slapstic_data slapstic116 = null;
        static readonly slapstic_data slapstic117 = null;
        static readonly slapstic_data slapstic118 = null;


        /*************************************
        *
        *  Master slapstic table
        *
        *************************************/
        /* master table */
        static readonly slapstic_data [] slapstic_table = new slapstic_data[]
        {
            slapstic101,   /* NOT confirmed! */
            null,           /* never seen */
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


        int m_chipnum;

        uint8_t state;
        uint8_t current_bank;
        int access_68k;

        uint8_t alt_bank;
        uint8_t bit_bank;
        uint8_t add_bank;
        uint8_t bit_xor;

        slapstic_data slapstic = new slapstic_data();


        // construction/destruction
        atari_slapstic_device(machine_config mconfig, string tag, device_t owner, int chipnum, bool m68k_mode)
            : this(mconfig, tag, owner, (uint32_t)0)
        {
            set_chipnum(chipnum);
            set_access68k(m68k_mode ? 1 : 0);
        }


        atari_slapstic_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, SLAPSTIC, tag, owner, clock)
        {
            state = 0;
            current_bank = 0;
            access_68k = -1;
            alt_bank = 0;
            bit_bank = 0;
            add_bank = 0;
            bit_xor = 0;


            slapstic.bankstart = 0;
            slapstic.bank[0] = slapstic.bank[1] = slapstic.bank[2] = slapstic.bank[3] = 0;
            slapstic.alt1.mask = 0;
            slapstic.alt1.value = 0;
            slapstic.alt2.mask = 0;
            slapstic.alt2.value = 0;
            slapstic.alt3.mask = 0;
            slapstic.alt3.value = 0;
            slapstic.alt4.mask = 0;
            slapstic.alt4.value = 0;
            slapstic.altshift = 0;
            slapstic.bit1.mask = 0;
            slapstic.bit1.value = 0;
            slapstic.bit2c0.mask = 0;
            slapstic.bit2c0.value = 0;
            slapstic.bit2s0.mask = 0;
            slapstic.bit2s0.value = 0;
            slapstic.bit2c1.mask = 0;
            slapstic.bit2c1.value = 0;
            slapstic.bit2s1.mask = 0;
            slapstic.bit2s1.value = 0;
            slapstic.bit3.mask = 0;
            slapstic.bit3.value = 0;
            slapstic.add1.mask = 0;
            slapstic.add1.value = 0;
            slapstic.add2.mask = 0;
            slapstic.add2.value = 0;
            slapstic.addplus1.mask = 0;
            slapstic.addplus1.value = 0;
            slapstic.addplus2.mask = 0;
            slapstic.addplus2.value = 0;
            slapstic.add3.mask = 0;
            slapstic.add3.value = 0;
        }

        public void atari_slapstic_device_after_ctor(int chipnum, bool m68k_mode)
        {
            set_chipnum(chipnum);
            set_access68k(m68k_mode ? 1 : 0);
        }


        /*************************************
         *
         *  Initialization
         *
         *************************************/
        public void slapstic_init()
        {
            /* set up the parameters */
            slapstic = slapstic_table[m_chipnum - 101];

            /* reset the chip */
            slapstic_reset();


            /* save state */
            save_item(NAME(new { state }));
            save_item(NAME(new { current_bank }));
            save_item(NAME(new { alt_bank }));
            save_item(NAME(new { bit_bank }));
            save_item(NAME(new { add_bank }));
            save_item(NAME(new { bit_xor }));
        }


        public void slapstic_reset()
        {
            /* reset the chip */
            state = DISABLED;

            /* the 111 and later chips seem to reset to bank 0 */
            current_bank = (uint8_t)slapstic.bankstart;
        }


        /*************************************
         *
         *  Returns active bank without tweaking
         *
         *************************************/
        public int slapstic_bank()
        {
            return current_bank;
        }


        /*************************************
         *
         *  Call this *after* every access
         *
         *************************************/
        public int slapstic_tweak(address_space space, offs_t offset)
        {
            /* reset is universal */
            if (offset == 0x0000)
            {
                state = ENABLED;
            }

            /* otherwise, use the state machine */
            else
            {
                switch (state)
                {
                    /* DISABLED state: everything is ignored except a reset */
                    case DISABLED:
                        break;

                    /* ENABLED state: the chip has been activated and is ready for a bankswitch */
                    case ENABLED:

                        /* check for request to enter bitwise state */
                        if (MATCHES_MASK_VALUE(offset, slapstic.bit1))
                        {
                            state = BITWISE1;
                        }

                        /* check for request to enter additive state */
                        else if (MATCHES_MASK_VALUE(offset, slapstic.add1))
                        {
                            state = ADDITIVE1;
                        }

                        /* check for request to enter alternate state */
                        else if (MATCHES_MASK_VALUE(offset, slapstic.alt1))
                        {
                            state = ALTERNATE1;
                        }

                        /* special kludge for catching the second alternate address if */
                        /* the first one was missed (since it's usually an opcode fetch) */
                        else if (MATCHES_MASK_VALUE(offset, slapstic.alt2))
                        {
                            state = (uint8_t)alt2_kludge(space, offset);
                        }

                        /* check for standard bankswitches */
                        else if (offset == slapstic.bank[0])
                        {
                            state = DISABLED;
                            current_bank = 0;
                        }
                        else if (offset == slapstic.bank[1])
                        {
                            state = DISABLED;
                            current_bank = 1;
                        }
                        else if (offset == slapstic.bank[2])
                        {
                            state = DISABLED;
                            current_bank = 2;
                        }
                        else if (offset == slapstic.bank[3])
                        {
                            state = DISABLED;
                            current_bank = 3;
                        }
                        break;

                    /* ALTERNATE1 state: look for alternate2 offset, or else fall back to ENABLED */
                    case ALTERNATE1:
                        if (MATCHES_MASK_VALUE(offset, slapstic.alt2))
                        {
                            state = ALTERNATE2;
                        }
                        else
                        {
                            state = ENABLED;
                        }
                        break;

                    /* ALTERNATE2 state: look for altbank offset, or else fall back to ENABLED */
                    case ALTERNATE2:
                        if (MATCHES_MASK_VALUE(offset, slapstic.alt3))
                        {
                            state = ALTERNATE3;
                            alt_bank = (uint8_t)((offset >> slapstic.altshift) & 3);
                        }
                        else
                        {
                            state = ENABLED;
                        }
                        break;

                    /* ALTERNATE3 state: wait for the final value to finish the transaction */
                    case ALTERNATE3:
                        if (MATCHES_MASK_VALUE(offset, slapstic.alt4))
                        {
                            state = DISABLED;
                            current_bank = alt_bank;
                        }
                        break;

                    /* BITWISE1 state: waiting for a bank to enter the BITWISE state */
                    case BITWISE1:
                        if (offset == slapstic.bank[0] || offset == slapstic.bank[1] ||
                            offset == slapstic.bank[2] || offset == slapstic.bank[3])
                        {
                            state = BITWISE2;
                            bit_bank = current_bank;
                            bit_xor = 0;
                        }
                        break;

                    /* BITWISE2 state: watch for twiddling and the escape mechanism */
                    case BITWISE2:

                        /* check for clear bit 0 case */
                        if (MATCHES_MASK_VALUE(offset ^ bit_xor, slapstic.bit2c0))
                        {
                            bit_bank &= unchecked((uint8_t)~1);
                            bit_xor ^= 3;
                        }

                        /* check for set bit 0 case */
                        else if (MATCHES_MASK_VALUE(offset ^ bit_xor, slapstic.bit2s0))
                        {
                            bit_bank |= 1;
                            bit_xor ^= 3;
                        }

                        /* check for clear bit 1 case */
                        else if (MATCHES_MASK_VALUE(offset ^ bit_xor, slapstic.bit2c1))
                        {
                            bit_bank &= unchecked((uint8_t)~2);
                            bit_xor ^= 3;
                        }

                        /* check for set bit 1 case */
                        else if (MATCHES_MASK_VALUE(offset ^ bit_xor, slapstic.bit2s1))
                        {
                            bit_bank |= 2;
                            bit_xor ^= 3;
                        }

                        /* check for escape case */
                        else if (MATCHES_MASK_VALUE(offset, slapstic.bit3))
                        {
                            state = BITWISE3;
                        }
                        break;

                    /* BITWISE3 state: waiting for a bank to seal the deal */
                    case BITWISE3:
                        if (offset == slapstic.bank[0] || offset == slapstic.bank[1] ||
                            offset == slapstic.bank[2] || offset == slapstic.bank[3])
                        {
                            state = DISABLED;
                            current_bank = bit_bank;
                        }
                        break;

                    /* ADDITIVE1 state: look for add2 offset, or else fall back to ENABLED */
                    case ADDITIVE1:
                        if (MATCHES_MASK_VALUE(offset, slapstic.add2))
                        {
                            state = ADDITIVE2;
                            add_bank = current_bank;
                        }
                        else
                        {
                            state = ENABLED;
                        }
                        break;

                    /* ADDITIVE2 state: watch for twiddling and the escape mechanism */
                    case ADDITIVE2:

                        /* check for add 1 case -- can intermix */
                        if (MATCHES_MASK_VALUE(offset, slapstic.addplus1))
                        {
                            add_bank = (uint8_t)((add_bank + 1) & 3);
                        }

                        /* check for add 2 case -- can intermix */
                        if (MATCHES_MASK_VALUE(offset, slapstic.addplus2))
                        {
                            add_bank = (uint8_t)((add_bank + 2) & 3);
                        }

                        /* check for escape case -- can intermix with the above */
                        if (MATCHES_MASK_VALUE(offset, slapstic.add3))
                        {
                            state = ADDITIVE3;
                        }
                        break;

                    /* ADDITIVE3 state: waiting for a bank to seal the deal */
                    case ADDITIVE3:
                        if (offset == slapstic.bank[0] || offset == slapstic.bank[1] ||
                            offset == slapstic.bank[2] || offset == slapstic.bank[3])
                        {
                            state = DISABLED;
                            current_bank = add_bank;
                        }
                        break;
                }
            }

            /* log this access */
            if (LOG_SLAPSTIC)
                slapstic_log(machine(), offset);

            /* return the active bank */
            return current_bank;
        }


        int alt2_kludge(address_space space, offs_t offset) { throw new emu_unimplemented(); }


        void set_access68k(int type) { access_68k = type; }

        void set_chipnum(int chipnum) { m_chipnum = chipnum; }


        void slapstic_log(running_machine machine, offs_t offset) { throw new emu_unimplemented(); }
        //FILE *slapsticlog;


        protected override void device_start()
        {
        }


        protected override void device_reset()
        {
        }


        protected override void device_validity_check(validity_checker valid)
        {
            throw new emu_unimplemented();
        }
    }
}
