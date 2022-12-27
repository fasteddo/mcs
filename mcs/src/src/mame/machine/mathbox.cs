// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int16_t = System.Int16;
using int32_t = System.Int32;
using offs_t = System.UInt32;  //using offs_t = u32;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.emucore_global;
using static mame.mathbox_global;


namespace mame
{
    /* ----- device interface ----- */
    public class mathbox_device : device_t
    {
        //DEFINE_DEVICE_TYPE(MATHBOX, mathbox_device, "mathbox", "Atari MATHBOX")
        public static readonly emu.detail.device_type_impl MATHBOX = DEFINE_DEVICE_TYPE("mathbox", "Atari MATHBOX", (type, mconfig, tag, owner, clock) => { return new mathbox_device(mconfig, tag, owner, clock); });


        int16_t REG0 { get { return m_reg[0x00]; } set { m_reg[0x00] = value; } }
        int16_t REG1 { get { return m_reg[0x01]; } set { m_reg[0x01] = value; } }
        int16_t REG2 { get { return m_reg[0x02]; } set { m_reg[0x02] = value; } }
        int16_t REG3 { get { return m_reg[0x03]; } set { m_reg[0x03] = value; } }
        int16_t REG4 { get { return m_reg[0x04]; } set { m_reg[0x04] = value; } }
        int16_t REG5 { get { return m_reg[0x05]; } set { m_reg[0x05] = value; } }
        int16_t REG6 { get { return m_reg[0x06]; } set { m_reg[0x06] = value; } }
        int16_t REG7 { get { return m_reg[0x07]; } set { m_reg[0x07] = value; } }
        int16_t REG8 { get { return m_reg[0x08]; } set { m_reg[0x08] = value; } }
        int16_t REG9 { get { return m_reg[0x09]; } set { m_reg[0x09] = value; } }
        int16_t REGa { get { return m_reg[0x0a]; } set { m_reg[0x0a] = value; } }
        int16_t REGb { get { return m_reg[0x0b]; } set { m_reg[0x0b] = value; } }
        int16_t REGc { get { return m_reg[0x0c]; } set { m_reg[0x0c] = value; } }
        int16_t REGd { get { return m_reg[0x0d]; } set { m_reg[0x0d] = value; } }
        int16_t REGe { get { return m_reg[0x0e]; } set { m_reg[0x0e] = value; } }
        int16_t REGf { get { return m_reg[0x0f]; } set { m_reg[0x0f] = value; } }


        const bool MB_TEST = false;
        void LOG(string format, params object [] args) { if (MB_TEST) logerror(format, args); }


        // internal state

        /* math box scratch registers */
        int16_t [] m_reg = new int16_t [16];

        /* math box result */
        int16_t m_result;


        mathbox_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, MATHBOX, tag, owner, clock)
        {
        }


        public void go_w(offs_t offset, uint8_t data)
        {
            int32_t mb_temp;  /* temp 32-bit multiply results */
            int16_t mb_q;     /* temp used in division */
            int msb;

            LOG("math box command {0} data {1}  ", offset, data);

            switch (offset)
            {
            case 0x00: m_result = REG0 = (int16_t)((REG0 & 0xff00) | data);        break;
            case 0x01: m_result = REG0 = (int16_t)((REG0 & 0x00ff) | (data << 8)); break;
            case 0x02: m_result = REG1 = (int16_t)((REG1 & 0xff00) | data);        break;
            case 0x03: m_result = REG1 = (int16_t)((REG1 & 0x00ff) | (data << 8)); break;
            case 0x04: m_result = REG2 = (int16_t)((REG2 & 0xff00) | data);        break;
            case 0x05: m_result = REG2 = (int16_t)((REG2 & 0x00ff) | (data << 8)); break;
            case 0x06: m_result = REG3 = (int16_t)((REG3 & 0xff00) | data);        break;
            case 0x07: m_result = REG3 = (int16_t)((REG3 & 0x00ff) | (data << 8)); break;
            case 0x08: m_result = REG4 = (int16_t)((REG4 & 0xff00) | data);        break;
            case 0x09: m_result = REG4 = (int16_t)((REG4 & 0x00ff) | (data << 8)); break;

            case 0x0a: m_result = REG5 = (int16_t)((REG5 & 0xff00) | data);        break;
                /* note: no function loads low part of REG5 without performing a computation */

            case 0x0c: m_result = REG6 = data; break;
                /* note: no function loads high part of REG6 */

            case 0x15: m_result = REG7 = (int16_t)((REG7 & 0xff00) | data);        break;
            case 0x16: m_result = REG7 = (int16_t)((REG7 & 0x00ff) | (data << 8)); break;

            case 0x1a: m_result = REG8 = (int16_t)((REG8 & 0xff00) | data);        break;
            case 0x1b: m_result = REG8 = (int16_t)((REG8 & 0x00ff) | (data << 8)); break;

            case 0x0d: m_result = REGa = (int16_t)((REGa & 0xff00) | data);        break;
            case 0x0e: m_result = REGa = (int16_t)((REGa & 0x00ff) | (data << 8)); break;
            case 0x0f: m_result = REGb = (int16_t)((REGb & 0xff00) | data);        break;
            case 0x10: m_result = REGb = (int16_t)((REGb & 0x00ff) | (data << 8)); break;

            case 0x17: m_result = REG7; break;
            case 0x19: m_result = REG8; break;
            case 0x18: m_result = REG9; break;

            case 0x0b:

                REG5 = (int16_t)((REG5 & 0x00ff) | (data << 8));

                REGf = 0xff;  //(int16_t)0xffff;
                REG4 -= REG2;
                REG5 -= REG3;

            step_048:

                mb_temp = ((int32_t) REG0) * ((int32_t) REG4);
                REGc = (int16_t)(mb_temp >> 16);
                REGe = (int16_t)(mb_temp & 0xffff);

                mb_temp = ((int32_t)(-REG1)) * ((int32_t) REG5);
                REG7 = (int16_t)(mb_temp >> 16);
                mb_q = (int16_t)(mb_temp & 0xffff);

                REG7 += REGc;

                /* rounding */
                REGe = (int16_t)((REGe >> 1) & 0x7fff);
                REGc = (int16_t)((mb_q >> 1) & 0x7fff);
                mb_q = (int16_t)(REGc + REGe);
                if (mb_q < 0)
                    REG7++;

                m_result = REG7;

                if (REGf < 0)
                    break;

                REG7 += REG2;

                /* fall into command 12 */
                goto case 0x12;  //[[fallthrough]];

            case 0x12:

                mb_temp = ((int32_t) REG1) * ((int32_t) REG4);
                REGc = (int16_t)(mb_temp >> 16);
                REG9 = (int16_t)(mb_temp & 0xffff);

                mb_temp = ((int32_t) REG0) * ((int32_t) REG5);
                REG8 = (int16_t)(mb_temp >> 16);
                mb_q = (int16_t)(mb_temp & 0xffff);

                REG8 += REGc;

                /* rounding */
                REG9 = (int16_t)((REG9 >> 1) & 0x7fff);
                REGc = (int16_t)((mb_q >> 1) & 0x7fff);
                REG9 += REGc;
                if (REG9 < 0)
                    REG8++;
                REG9 <<= 1;  /* why? only to get the desired load address? */

                m_result = REG8;

                if (REGf < 0)
                    break;

                REG8 += REG3;

                REG9 = (int16_t)(REG9 & 0xff00);

                /* fall into command 13 */
                goto case 0x13;  //[[fallthrough]];

            case 0x13:
                LOG("\nR7: {0}  R8: {1}  R9: {2}\n", REG7, REG8, REG9);

                REGc = REG9;
                mb_q = REG8;
                goto step_0bf;

            case 0x14:
                REGc = REGa;
                mb_q = REGb;

            step_0bf:
                REGe = (int16_t)(REG7 ^ mb_q);  /* save sign of result */
                REGd = mb_q;
                if (mb_q >= 0)
                    mb_q = REGc;
                else
                {
                    REGd = (int16_t)(- mb_q - 1);
                    mb_q = (int16_t)(- REGc - 1);
                    if ((mb_q < 0) && ((mb_q + 1) < 0))
                        REGd++;
                    mb_q++;
                }

            /* step 0c9: */
                /* REGc = abs (REG7) */
                if (REG7 >= 0)
                    REGc = REG7;
                else
                    REGc = (int16_t)(-REG7);

                REGf = REG6;  /* step counter */

                do
                {
                    REGd -= REGc;
                    msb = ((mb_q & 0x8000) != 0) ? 1 : 0;
                    mb_q <<= 1;
                    if (REGd >= 0)
                        mb_q++;
                    else
                        REGd += REGc;
                    REGd <<= 1;
                    REGd += (int16_t)msb;
                }
                while (--REGf >= 0);

                if (REGe >= 0)
                    m_result = mb_q;
                else
                    m_result = (int16_t)(-mb_q);
                break;

            case 0x11:
                REG5 = (int16_t)((REG5 & 0x00ff) | (data << 8));
                REGf = 0x0000;  /* do everything in one step */
                goto step_048;
                //break; // never reached

            case 0x1c:
                /* window test? */
                REG5 = (int16_t)((REG5 & 0x00ff) | (data << 8));
                do
                {
                    REGe = (int16_t)((REG4 + REG7) >> 1);
                    REGf = (int16_t)((REG5 + REG8) >> 1);
                    if ((REGb < REGe) && (REGf < REGe) && ((REGe + REGf) >= 0))
                    { REG7 = REGe; REG8 = REGf; }
                    else
                    { REG4 = REGe; REG5 = REGf; }
                }
                while (--REG6 >= 0);

                m_result = REG8;
                break;

            case 0x1d:
                REG3 = (int16_t)((REG3 & 0x00ff) | (data << 8));

                REG2 -= REG0;
                if (REG2 < 0)
                    REG2 = (int16_t)(-REG2);

                REG3 -= REG1;
                if (REG3 < 0)
                    REG3 = (int16_t)(-REG3);

                /* fall into command 1e */
                goto case 0x1e;  //[[fallthrough]];

            case 0x1e:
                /* result = max (REG2, REG3) + 3/8 * min (REG2, REG3) */
                if (REG3 >= REG2)
                { REGc = REG2; REGd = REG3; }
                else
                { REGd = REG2; REGc = REG3; }
                REGc >>= 2;
                REGd += REGc;
                REGc >>= 1;
                m_result = REGd = (int16_t)(REGc + REGd);
                break;

            case 0x1f:
                logerror("math box function 0x1f\n");
                /* $$$ do some computation here (selftest? signature analysis? */
                break;
            }

            LOG("  result {0}\n", m_result & 0xffff);
        }


        public uint8_t status_r()
        {
            return 0x00; /* always done! */
        }


        public uint8_t lo_r()
        {
            return (uint8_t)(m_result & 0xff);
        }


        public uint8_t hi_r()
        {
            return (uint8_t)((m_result >> 8) & 0xff);
        }


        // device-level overrides
        protected override void device_start()
        {
            /* register for save states */
            save_item(NAME(new { m_result }));
            save_item(NAME(new { m_reg }));
        }


        protected override void device_reset()
        {
            m_result = 0;
            Array.Clear(m_reg, 0, 16);  //memset(m_reg, 0, sizeof(int16_t)*16);
        }
    }


    static class mathbox_global
    {
        public static mathbox_device MATHBOX<bool_Required>(machine_config mconfig, device_finder<mathbox_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, mathbox_device.MATHBOX, clock); }
    }
}
