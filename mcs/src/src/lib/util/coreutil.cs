// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using uint32_t = System.UInt32;


namespace mame
{
    public static class coreutil_global
    {
        public static uint32_t core_crc32(uint32_t crc, ListBytesPointer buf, uint32_t len)  //uint32_t crc, const uint8_t *buf, uint32_t len);
        {
            return crc32_global.crc32(crc, buf, len);
        }
    }
}
