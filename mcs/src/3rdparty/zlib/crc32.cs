// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint8_t = System.Byte;
using uInt = System.UInt32;
using unsigned_long = System.UInt32;

using static mame.crc32_global;


namespace mame
{
    public static class crc32_global
    {
        public static unsigned_long crc32(unsigned_long crc, Pointer<uint8_t> buf, uInt len)
        {
            //throw new emu_unimplemented();
            return 0;
        }
    }
}
