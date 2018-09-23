// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u8 = System.Byte;


namespace mame
{
    interface dac_byte_interface
    {
        void write(u8 data);

        //virtual DECLARE_WRITE8_MEMBER(write) = 0;
        void write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff);

        //virtual DECLARE_WRITE8_MEMBER(data_w) = 0;
        void data_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff);
    }
}
