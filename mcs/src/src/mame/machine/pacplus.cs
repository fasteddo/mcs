// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    partial class pacman_state : driver_device
    {
        static readonly uint8_t [,] swap_xor_table = new uint8_t[6, 9]
        {
            { 7,6,5,4,3,2,1,0, 0x00 },
            { 7,6,5,4,3,2,1,0, 0x28 },
            { 6,1,3,2,5,7,0,4, 0x96 },
            { 6,1,5,2,3,7,0,4, 0xbe },
            { 0,3,7,6,4,2,1,5, 0xd5 },
            { 0,3,4,6,7,2,1,5, 0xdd }
        };

        static readonly int [] picktable = new int[32]
        {
            0,2,4,2,4,0,4,2,2,0,2,2,4,0,4,2,
            2,2,4,0,4,2,4,0,0,4,0,4,4,2,4,2
        };

        uint8_t pacplus_decrypt(int addr, uint8_t e)
        {
            uint32_t method = 0;
            //const UINT8 *tbl;

            /* pick method from bits 0 2 5 7 9 of the address */
            method = (uint32_t)picktable[
                (addr & 0x001) |
                ((addr & 0x004) >> 1) |
                ((addr & 0x020) >> 3) |
                ((addr & 0x080) >> 4) |
                ((addr & 0x200) >> 5)];

            /* switch method if bit 11 of the address is set */
            if ((addr & 0x800) == 0x800)
                method ^= 1;

            //tbl = swap_xor_table[method];
            return (uint8_t)(g.bitswap(e,swap_xor_table[method,0],swap_xor_table[method,1],swap_xor_table[method,2],swap_xor_table[method,3],swap_xor_table[method,4],swap_xor_table[method,5],swap_xor_table[method,6],swap_xor_table[method,7]) ^ swap_xor_table[method,8]);
        }

        void pacplus_decode()
        {
            int i;
            Pointer<uint8_t> RAM;  //uint8_t *RAM;

            /* CPU ROMs */

            RAM = new Pointer<uint8_t>(memregion("maincpu").base_());
            for (i = 0; i < 0x4000; i++)
            {
                RAM[i] = pacplus_decrypt(i,RAM[i]);
            }
        }
    }
}
