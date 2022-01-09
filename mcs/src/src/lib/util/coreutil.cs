// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.crc32_global;


namespace mame
{
    public static class coreutil_global
    {
        public static uint32_t core_crc32(uint32_t crc, Pointer<uint8_t> buf, uint32_t len)  //uint32_t core_crc32(uint32_t crc, const uint8_t *buf, uint32_t len);
        {
            return crc32(crc, buf, len);
        }
    }
}
