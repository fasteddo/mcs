// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using s32 = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;

using static mame.digfx_global;
using static mame.diexec_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.driver_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.gen_latch_global;
using static mame.hash_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.mcs51_global;
using static mame.i8255_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.sn76496_global;
using static mame.speaker_global;
using static mame.timer_global;
using static mame.util;
using static mame.z80_global;


namespace mame
{
    partial class system1_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK    = new XTAL(20_000_000);
        static readonly XTAL SOUND_CLOCK     = new XTAL(8_000_000);


        /*************************************
         *  Machine initialization
         *************************************/

        /*
            About main CPU clocking:

            A 20MHz crystal clocks an LS161 which counts up from either 10 or 11 to 16 before
            carrying out and forcing a reload. The low bit of the reload value comes from the
            Z80's /M1 signal. When /M1 is low (an opcode is being fetched), the reload count
            is 10, which means the 20MHz clock is divided by 6. When /M1 is high, the reload
            count is 11, which means the clock is divided by 5.

            To account for this, we install custom cycle tables for the Z80. We clock the Z80
            at 20MHz and count 5 cycles for each original Z80 cycle, plus an extra 2 cycles for
            each opcode fetch (since the M1 line is low for 2 cycles per byte).
        */

        static readonly u8 [] cc_op = new u8 [0x100] {
            4*5+1*2,10*5+3*2, 7*5+1*2, 6*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+2*2, 4*5+1*2, 4*5+1*2,11*5+1*2, 7*5+1*2, 6*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+2*2, 4*5+1*2,
            8*5+2*2,10*5+3*2, 7*5+1*2, 6*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+2*2, 4*5+1*2,12*5+2*2,11*5+1*2, 7*5+1*2, 6*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+2*2, 4*5+1*2,
            7*5+2*2,10*5+3*2,16*5+3*2, 6*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+2*2, 4*5+1*2, 7*5+2*2,11*5+1*2,16*5+3*2, 6*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+2*2, 4*5+1*2,
            7*5+2*2,10*5+3*2,13*5+3*2, 6*5+1*2,11*5+1*2,11*5+1*2,10*5+2*2, 4*5+1*2, 7*5+2*2,11*5+1*2,13*5+3*2, 6*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+2*2, 4*5+1*2,
            4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2,
            4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2,
            4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2,
            7*5+1*2, 7*5+1*2, 7*5+1*2, 7*5+1*2, 7*5+1*2, 7*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2,
            4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2,
            4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2,
            4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2,
            4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 4*5+1*2, 7*5+1*2, 4*5+1*2,
            5*5+1*2,10*5+1*2,10*5+3*2,10*5+3*2,10*5+3*2,11*5+1*2, 7*5+2*2,11*5+1*2, 5*5+1*2,10*5+1*2,10*5+3*2, 0*5    ,10*5+3*2,17*5+3*2, 7*5+2*2,11*5+1*2,
            5*5+1*2,10*5+1*2,10*5+3*2,11*5+2*2,10*5+3*2,11*5+1*2, 7*5+2*2,11*5+1*2, 5*5+1*2, 4*5+1*2,10*5+3*2,11*5+2*2,10*5+3*2, 0*5    , 7*5+2*2,11*5+1*2,
            5*5+1*2,10*5+1*2,10*5+3*2,19*5+1*2,10*5+3*2,11*5+1*2, 7*5+2*2,11*5+1*2, 5*5+1*2, 4*5+1*2,10*5+3*2, 4*5+1*2,10*5+3*2, 0*5    , 7*5+2*2,11*5+1*2,
            5*5+1*2,10*5+1*2,10*5+3*2, 4*5+1*2,10*5+3*2,11*5+1*2, 7*5+2*2,11*5+1*2, 5*5+1*2, 6*5+1*2,10*5+3*2, 4*5+1*2,10*5+3*2, 0*5    , 7*5+2*2,11*5+1*2
        };

        static readonly u8 [] cc_cb = new u8 [0x100] {
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,12*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,12*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,12*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,12*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,12*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,12*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,12*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,12*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,15*5+2*2, 8*5+2*2
        };

        static readonly u8 [] cc_ed = new u8 [0x100] {
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,
            12*5+2*2,12*5+2*2,15*5+2*2,20*5+4*2,8*5+2*2,14*5+2*2, 8*5+2*2, 9*5+2*2,12*5+2*2,12*5+2*2,15*5+2*2,20*5+4*2, 8*5+2*2,14*5+2*2, 8*5+2*2, 9*5+2*2,
            12*5+2*2,12*5+2*2,15*5+2*2,20*5+4*2,8*5+2*2,14*5+2*2, 8*5+2*2, 9*5+2*2,12*5+2*2,12*5+2*2,15*5+2*2,20*5+4*2, 8*5+2*2,14*5+2*2, 8*5+2*2, 9*5+2*2,
            12*5+2*2,12*5+2*2,15*5+2*2,20*5+4*2,8*5+2*2,14*5+2*2, 8*5+2*2,18*5+2*2,12*5+2*2,12*5+2*2,15*5+2*2,20*5+4*2, 8*5+2*2,14*5+2*2, 8*5+2*2,18*5+2*2,
            12*5+2*2,12*5+2*2,15*5+2*2,20*5+4*2,8*5+2*2,14*5+2*2, 8*5+2*2, 8*5+2*2,12*5+2*2,12*5+2*2,15*5+2*2,20*5+4*2, 8*5+2*2,14*5+2*2, 8*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,
            16*5+2*2,16*5+2*2,16*5+2*2,16*5+2*2,8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,16*5+2*2,16*5+2*2,16*5+2*2,16*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,
            16*5+2*2,16*5+2*2,16*5+2*2,16*5+2*2,8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,16*5+2*2,16*5+2*2,16*5+2*2,16*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2,
            8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2, 8*5+2*2
        };

        static readonly u8 [] cc_xy = new u8 [0x100] {
            ( 4+4)*5+2*2,(10+4)*5+4*2,( 7+4)*5+2*2,( 6+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 7+4)*5+3*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(11+4)*5+2*2,( 7+4)*5+2*2,( 6+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 7+4)*5+3*2,( 4+4)*5+2*2,
            ( 8+4)*5+3*2,(10+4)*5+4*2,( 7+4)*5+2*2,( 6+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 7+4)*5+3*2,( 4+4)*5+2*2,(12+4)*5+3*2,(11+4)*5+2*2,( 7+4)*5+2*2,( 6+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 7+4)*5+3*2,( 4+4)*5+2*2,
            ( 7+4)*5+3*2,(10+4)*5+4*2,(16+4)*5+4*2,( 6+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 7+4)*5+3*2,( 4+4)*5+2*2,( 7+4)*5+3*2,(11+4)*5+2*2,(16+4)*5+4*2,( 6+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 7+4)*5+3*2,( 4+4)*5+2*2,
            ( 7+4)*5+3*2,(10+4)*5+4*2,(13+4)*5+4*2,( 6+4)*5+2*2,(23  )*5+3*2,(23  )*5+3*2,(19  )*5+4*2,( 4+4)*5+2*2,( 7+4)*5+3*2,(11+4)*5+2*2,(13+4)*5+4*2,( 6+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 7+4)*5+3*2,( 4+4)*5+2*2,
            ( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,
            ( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,
            ( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,
            (19  )*5+3*2,(19  )*5+3*2,(19  )*5+3*2,(19  )*5+3*2,(19  )*5+3*2,(19  )*5+3*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,
            ( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,
            ( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,
            ( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,
            ( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,( 4+4)*5+2*2,(19  )*5+3*2,( 4+4)*5+2*2,
            ( 5+4)*5+2*2,(10+4)*5+2*2,(10+4)*5+4*2,(10+4)*5+4*2,(10+4)*5+4*2,(11+4)*5+2*2,( 7+4)*5+3*2,(11+4)*5+2*2,( 5+4)*5+2*2,(10+4)*5+2*2,(10+4)*5+4*2,( 0  )*5    ,(10+4)*5+4*2,(17+4)*5+4*2,( 7+4)*5+3*2,(11+4)*5+2*2,
            ( 5+4)*5+2*2,(10+4)*5+2*2,(10+4)*5+4*2,(11+4)*5+3*2,(10+4)*5+4*2,(11+4)*5+2*2,( 7+4)*5+3*2,(11+4)*5+2*2,( 5+4)*5+2*2,( 4+4)*5+2*2,(10+4)*5+4*2,(11+4)*5+3*2,(10+4)*5+4*2,( 4  )*5+2*2,( 7+4)*5+3*2,(11+4)*5+2*2,
            ( 5+4)*5+2*2,(10+4)*5+2*2,(10+4)*5+4*2,(19+4)*5+2*2,(10+4)*5+4*2,(11+4)*5+2*2,( 7+4)*5+3*2,(11+4)*5+2*2,( 5+4)*5+2*2,( 4+4)*5+2*2,(10+4)*5+4*2,( 4+4)*5+2*2,(10+4)*5+4*2,( 4  )*5+2*2,( 7+4)*5+3*2,(11+4)*5+2*2,
            ( 5+4)*5+2*2,(10+4)*5+2*2,(10+4)*5+4*2,( 4+4)*5+2*2,(10+4)*5+4*2,(11+4)*5+2*2,( 7+4)*5+3*2,(11+4)*5+2*2,( 5+4)*5+2*2,( 6+4)*5+2*2,(10+4)*5+4*2,( 4+4)*5+2*2,(10+4)*5+4*2,( 4  )*5+2*2,( 7+4)*5+3*2,(11+4)*5+2*2
        };

        static readonly u8 [] cc_xycb = new u8 [0x100] {
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,
            20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,
            20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,
            20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,
            20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,20*5+4*2,
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,
            23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2,23*5+4*2
        };

        /* extra cycles if jr/jp/call taken and 'interrupt latency' on rst 0-7 */
        static readonly u8 [] cc_ex = new u8 [0x100] {
            0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5,
            5*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, /* DJNZ */
            5*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 5*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, /* JR NZ/JR Z */
            5*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 5*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, /* JR NC/JR C */
            0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5,
            0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5,
            0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5,
            0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5,
            0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5,
            0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5,
            0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5, 0*5,
            5*5, 5*5, 5*5, 5*5, 0*5, 0*5, 0*5, 0*5, 5*5, 5*5, 5*5, 5*5, 0*5, 0*5, 0*5, 0*5, /* LDIR/CPIR/INIR/OTIR LDDR/CPDR/INDR/OTDR */
            6*5, 0*5, 0*5, 0*5, 7*5, 0*5, 0*5, 2*5, 6*5, 0*5, 0*5, 0*5, 7*5, 0*5, 0*5, 2*5,
            6*5, 0*5, 0*5, 0*5, 7*5, 0*5, 0*5, 2*5, 6*5, 0*5, 0*5, 0*5, 7*5, 0*5, 0*5, 2*5,
            6*5, 0*5, 0*5, 0*5, 7*5, 0*5, 0*5, 2*5, 6*5, 0*5, 0*5, 0*5, 7*5, 0*5, 0*5, 2*5,
            6*5, 0*5, 0*5, 0*5, 7*5, 0*5, 0*5, 2*5, 6*5, 0*5, 0*5, 0*5, 7*5, 0*5, 0*5, 2*5
        };


        protected override void machine_start()
        {
            u32 numbanks = (m_maincpu_region.op0.bytes() - 0x10000) / 0x4000;

            if (numbanks > 0)
                m_bank1.op0.configure_entries(0, (int)numbanks, new PointerU8(m_maincpu_region.op0.base_()) + 0x10000, 0x4000);
            else
                m_bank1.op0.configure_entry(0, new PointerU8(m_maincpu_region.op0.base_()) + 0x8000);

            m_bank1.op0.set_entry(0);

            if (m_banked_decrypted_opcodes != null)
            {
                m_bank0d.op0.set_base(new PointerU8(m_banked_decrypted_opcodes));
                m_bank1d.op0.configure_entries(0, (int)numbanks, new PointerU8(m_banked_decrypted_opcodes) + 0x10000, 0x4000);
                m_bank1d.op0.set_entry(0);
            }

            m_maincpu.op0.z80_set_cycle_tables(cc_op, cc_cb, cc_ed, cc_xy, cc_xycb, cc_ex);

            m_mute_xor = 0x00;
            m_dakkochn_mux_data = 0x00;

            save_item(NAME(new { m_dakkochn_mux_data }));
            save_item(NAME(new { m_videomode_prev }));
            save_item(NAME(new { m_mcu_control }));
            save_item(NAME(new { m_nob_maincpu_latch }));
            save_item(NAME(new { m_nob_mcu_latch }));
            save_item(NAME(new { m_nob_mcu_status }));
        }


        //MACHINE_START_MEMBER(system1_state,system2)
        void machine_start_system2()
        {
            machine_start();
            m_mute_xor = 0x01;
        }


        protected override void machine_reset()
        {
            m_dakkochn_mux_data = 0;
        }


        /*************************************
         *  ROM banking
         *************************************/

        //void system1_state::bank44_custom_w(u8 data, u8 prevdata)
        //{
        //    /* bank bits are bits 6 and 2 */
        //    m_bank1->set_entry(((data & 0x40) >> 5) | ((data & 0x04) >> 2));
        //}


        void bank0c_custom_w(u8 data, u8 prevdata)
        {
            /* bank bits are bits 3 and 2 */
            m_bank1.op0.set_entry((data & 0x0c) >> 2);
            if (m_bank1d != null && m_bank1d.op0 != null)
                m_bank1d.op0.set_entry((data & 0x0c) >> 2);
        }


        void videomode_w(u8 data)
        {
            /* bit 6 is connected to the 8751 IRQ */
            if (m_mcu != null)
                m_mcu.op0.set_input_line(MCS51_INT1_LINE, (data & 0x40) != 0 ? CLEAR_LINE : ASSERT_LINE);

            /* handle any custom banking or other stuff */
            if (m_videomode_custom != null)
                this.m_videomode_custom(data, m_videomode_prev);

            m_videomode_prev = data;

            /* bit 0 is for the coin counters */
            machine().bookkeeping().coin_counter_w(0, data & 1);

            /* remaining signals are video-related */
            common_videomode_w(data);
        }


        //CUSTOM_INPUT_MEMBER(system1_state::dakkochn_mux_data_r)

        //CUSTOM_INPUT_MEMBER(system1_state::dakkochn_mux_status_r)

        //void system1_state::dakkochn_custom_w(u8 data, u8 prevdata)

        //u8 system1_state::shtngmst_gunx_r()


        /*************************************
         *  Sound I/O
         *************************************/
        void sound_control_w(u8 data)
        {
            /* bit 0 = MUTE (inverted sense on System 2) */
            machine().sound().system_mute(((data ^ m_mute_xor) & 1) != 0);

            /* bit 6 = feedback from sound board that read occurrred */

            /* bit 7 controls the sound CPU's NMI line */
            m_soundcpu.op0.set_input_line(INPUT_LINE_NMI, (data & 0x80) != 0 ? CLEAR_LINE : ASSERT_LINE);

            /* remaining bits are used for video RAM banking */
            videoram_bank_w(data);
        }


        u8 sound_data_r()
        {
            throw new emu_unimplemented();
#if false
            /* if we have an 8255 PPI, get the data from the port and toggle the ack */
            if (m_ppi8255 != nullptr)
            {
                m_ppi8255->pc6_w(0);
                m_ppi8255->pc6_w(1);
                return m_soundlatch->read();
            }

            /* if we have a Z80 PIO, get the data from the port and toggle the strobe */
            else if (m_pio != nullptr)
            {
                u8 data = m_pio->port_read(z80pio_device::PORT_A);
                m_pio->strobe(z80pio_device::PORT_A, false);
                m_pio->strobe(z80pio_device::PORT_A, true);
                return data;
            }

            return 0xff;
#endif
        }


        void soundport_w(u8 data)
        {
            /* boost interleave when communicating with the sound CPU */
            m_soundlatch.op0.write(data);
            machine().scheduler().boost_interleave(attotime.zero, attotime.from_usec(100));
        }


        //TIMER_DEVICE_CALLBACK_MEMBER(system1_state::soundirq_gen)
        void soundirq_gen(timer_device timer, object ptr, s32 param)  //void *ptr, s32 param
        {
            throw new emu_unimplemented();
#if false
            /* sound IRQ is generated on 32V, 96V, ... and auto-acknowledged */
            m_soundcpu->set_input_line(0, HOLD_LINE);
#endif
        }


        /*************************************
         *  MCU I/O
         *************************************/
        void mcu_control_w(u8 data)
        {
            /*
                Bit 7 -> connects to TD62003 pins 5 & 6 @ IC151
                Bit 6 -> via PLS153, when high, asserts the BUSREQ signal, halting the Z80
                Bit 5 -> n/c
                Bit 4 -> (with bit 3) Memory select: 0=Z80 program space, 1=banked ROM, 2=Z80 I/O space, 3=watchdog?
                Bit 3 ->
                Bit 2 -> n/c
                Bit 1 -> n/c
                Bit 0 -> Directly connected to Z80 /INT line
            */

            /* boost interleave to ensure that the MCU can break the Z80 out of a HALT */
            if (BIT(m_mcu_control, 6) == 0 && BIT(data, 6) != 0)
                machine().scheduler().boost_interleave(attotime.zero, attotime.from_usec(10));

            m_mcu_control = data;
            m_maincpu.op0.set_input_line(INPUT_LINE_HALT, (data & 0x40) != 0 ? ASSERT_LINE : CLEAR_LINE);
            m_maincpu.op0.set_input_line(0, (data & 0x01) != 0 ? CLEAR_LINE : ASSERT_LINE);
        }


        void mcu_io_w(offs_t offset, u8 data)
        {
            throw new emu_unimplemented();
#if false
            switch ((m_mcu_control >> 3) & 3)
            {
                case 0:
                    m_maincpu->space(AS_PROGRAM).write_byte(offset, data);
                    break;

                case 2:
                    m_maincpu->space(AS_IO).write_byte(offset, data);
                    break;

                default:
                    logerror("%03X: MCU movx write mode %02X offset %04X = %02X\n",
                                m_mcu->pc(), m_mcu_control, offset, data);
                    break;
            }
#endif
        }


        u8 mcu_io_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
            switch ((m_mcu_control >> 3) & 3)
            {
                case 0:
                    return m_maincpu->space(AS_PROGRAM).read_byte(offset);

                case 1:
                    return m_maincpu_region->base()[offset + 0x10000];

                case 2:
                    return m_maincpu->space(AS_IO).read_byte(offset);

                default:
                    logerror("%03X: MCU movx read mode %02X offset %04X\n",
                                m_mcu->pc(), m_mcu_control, offset);
                    return 0xff;
            }
#endif
        }


        //TIMER_DEVICE_CALLBACK_MEMBER(system1_state::mcu_t0_callback)
        void mcu_t0_callback(timer_device timer, object ptr, s32 param)  //void *ptr, s32 param
        {
            /* The T0 line is clocked by something; if it is not clocked fast
               enough, the MCU will fail; on shtngmst this happens after 3
               VBLANKs without a tick.
               choplift is even more picky about it, affecting scroll speed
            */

            m_mcu.op0.set_input_line(MCS51_T0_LINE, ASSERT_LINE);
            m_mcu.op0.set_input_line(MCS51_T0_LINE, CLEAR_LINE);
        }


        //u8 system1_state::nob_mcu_latch_r()

        //void system1_state::nob_mcu_latch_w(u8 data)

        //void system1_state::nob_mcu_status_w(u8 data)

        //void system1_state::nob_mcu_control_p2_w(u8 data)

        //u8 system1_state::nob_maincpu_latch_r()

        //void system1_state::nob_maincpu_latch_w(u8 data)

        //u8 system1_state::nob_mcu_status_r()

        //u8 system1_state::nobb_inport1c_r()

        //u8 system1_state::nobb_inport22_r()

        //u8 system1_state::nobb_inport23_r()

        //void system1_state::nobb_outport24_w(u8 data)


        /*************************************
         *  Main CPU address maps
         *************************************/
        /* main memory map */
        void system1_map(address_map map, device_t owner)
        {
            map.op(0x0000, 0x7fff).rom();
            map.op(0x8000, 0xbfff).bankr("bank1");
            map.op(0xc000, 0xcfff).ram().share("ram");
            map.op(0xd000, 0xd7ff).ram().share("spriteram");
            map.op(0xd800, 0xdfff).ram().w(paletteram_w).share("paletteram");
            map.op(0xe000, 0xefff).rw(videoram_r, videoram_w);
            map.op(0xf000, 0xf3ff).rw(mixer_collision_r, mixer_collision_w);
            map.op(0xf400, 0xf7ff).w(mixer_collision_reset_w);
            map.op(0xf800, 0xfbff).rw(sprite_collision_r, sprite_collision_w);
            map.op(0xfc00, 0xffff).w(sprite_collision_reset_w);
        }

#if false
        void system1_state::decrypted_opcodes_map(address_map &map)
        {
            map(0x0000, 0x7fff).rom().share("decrypted_opcodes");
            map(0x8000, 0xbfff).bankr("bank1");
            map(0xc000, 0xcfff).ram().share("ram");
            map(0xd000, 0xd7ff).ram().share("spriteram");
            map(0xd800, 0xdfff).ram().w(FUNC(system1_state::paletteram_w)).share("paletteram");
        }

        void system1_state::banked_decrypted_opcodes_map(address_map &map)
        {
            map(0x0000, 0x7fff).bankr("bank0d");
            map(0x8000, 0xbfff).bankr("bank1d");
            map(0xc000, 0xcfff).ram().share("ram");
            map(0xd000, 0xd7ff).ram().share("spriteram");
            map(0xd800, 0xdfff).ram().w(FUNC(system1_state::paletteram_w)).share("paletteram");
        }
#endif

        /* same as normal System 1 except address map is shuffled (RAM/collision are swapped) */
        //void system1_state::nobo_map(address_map &map)


        /* I/O map for systems with an 8255 PPI */
        void system1_ppi_io_map(address_map map, device_t owner)
        {
            map.global_mask(0x1f);
            map.op(0x00, 0x00).mirror(0x03).portr("P1");
            map.op(0x04, 0x04).mirror(0x03).portr("P2");
            map.op(0x08, 0x08).mirror(0x03).portr("SYSTEM");
            map.op(0x0c, 0x0c).mirror(0x02).portr("SWA");    /* DIP2 */
            map.op(0x0d, 0x0d).mirror(0x02).portr("SWB");    /* DIP1 some games read it from here... */
            map.op(0x10, 0x10).mirror(0x03).portr("SWB");    /* DIP1 ... and some others from here but there are games which check BOTH! */
            map.op(0x14, 0x17).rw(m_ppi8255, (offset) => { return m_ppi8255.op0.read(offset); }, (offset, data) => { m_ppi8255.op0.write(offset, data); });
        }


#if false
        /* I/O map for systems with a Z80 PIO chip */
        void system1_state::system1_pio_io_map(address_map &map)
        {
            map.global_mask(0x1f);
            map(0x00, 0x00).mirror(0x03).portr("P1");
            map(0x04, 0x04).mirror(0x03).portr("P2");
            map(0x08, 0x08).mirror(0x03).portr("SYSTEM");
            map(0x0c, 0x0c).mirror(0x02).portr("SWA");    /* DIP2 */
            map(0x0d, 0x0d).mirror(0x02).portr("SWB");    /* DIP1 some games read it from here... */
            map(0x10, 0x10).mirror(0x03).portr("SWB");    /* DIP1 ... and some others from here but there are games which check BOTH! */
            map(0x18, 0x1b).rw("pio", FUNC(z80pio_device::read), FUNC(z80pio_device::write));
        }
#endif


        //void system1_state::blockgal_pio_io_map(address_map &map)


        /*************************************
         *  Sound CPU address maps
         *************************************/
        void sound_map(address_map map, device_t owner)
        {
            map.op(0x0000, 0x7fff).rom();
            map.op(0x8000, 0x87ff).mirror(0x1800).ram();
            map.op(0xa000, 0xa000).mirror(0x1fff).w("sn1", (data) => { ((sn76489a_device)subdevice("sn1")).write(data); });
            map.op(0xc000, 0xc000).mirror(0x1fff).w("sn2", (data) => { ((sn76489a_device)subdevice("sn2")).write(data); });
            map.op(0xe000, 0xe000).mirror(0x1fff).r(sound_data_r);
        }


        /*************************************
         *  MCU address maps
         *************************************/
        void mcu_io_map(address_map map, device_t owner)
        {
            map.op(0x0000, 0xffff).rw(mcu_io_r, mcu_io_w);
        }
    }


    partial class system1 : construct_ioport_helper
    {
        /*************************************
         *  Generic port definitions
         *************************************/
        //static INPUT_PORTS_START( system1_generic )
        void construct_ioport_system1_generic(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("P1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON2 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY();

            PORT_START("P2");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON2 ); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY(); PORT_COCKTAIL();

            PORT_START("SYSTEM");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_SERVICE_NO_TOGGLE( 0x04, IP_ACTIVE_LOW );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNKNOWN );

            PORT_START("SWA");
            PORT_DIPNAME( 0x0f, 0x0f, DEF_STR( Coin_A ) );       PORT_DIPLOCATION("SWA:1,2,3,4");
            PORT_DIPSETTING(    0x07, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x08, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x09, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x05, "2 Coins/1 Credit 5/3 6/4" );
            PORT_DIPSETTING(    0x04, "2 Coins/1 Credit 4/3" );
            PORT_DIPSETTING(    0x0f, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x01, "1 Coin/1 Credit 2/3" );
            PORT_DIPSETTING(    0x02, "1 Coin/1 Credit 4/5" );
            PORT_DIPSETTING(    0x03, "1 Coin/1 Credit 5/6" );
            PORT_DIPSETTING(    0x06, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x0e, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x0d, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0x0c, DEF_STR( _1C_4C ) );
            PORT_DIPSETTING(    0x0b, DEF_STR( _1C_5C ) );
            PORT_DIPSETTING(    0x0a, DEF_STR( _1C_6C ) );
            /*  PORT_DIPSETTING(    0x00, DEF_STR( 1C_1C ) ) Not allowed by mame coinage sorting, but valid */
            PORT_DIPNAME( 0xf0, 0xf0, DEF_STR( Coin_B ) );       PORT_DIPLOCATION("SWA:5,6,7,8");
            PORT_DIPSETTING(    0x70, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x80, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x90, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x50, "2 Coins/1 Credit 5/3 6/4" );
            PORT_DIPSETTING(    0x40, "2 Coins/1 Credit 4/3" );
            PORT_DIPSETTING(    0xf0, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x10, "1 Coin/1 Credit 2/3" );
            PORT_DIPSETTING(    0x20, "1 Coin/1 Credit 4/5" );
            PORT_DIPSETTING(    0x30, "1 Coin/1 Credit 5/6" );
            PORT_DIPSETTING(    0x60, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0xe0, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0xd0, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0xc0, DEF_STR( _1C_4C ) );
            PORT_DIPSETTING(    0xb0, DEF_STR( _1C_5C ) );
            PORT_DIPSETTING(    0xa0, DEF_STR( _1C_6C ) );
            /*  PORT_DIPSETTING(    0x00, DEF_STR( 1C_1C ) ) Not allowed by mame coinage sorting, but valid */

            PORT_START("SWB");
            PORT_DIPNAME( 0x01, 0x00, DEF_STR( Cabinet ) );      PORT_DIPLOCATION("SWB:1");
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x01, DEF_STR( Cocktail ) );
            PORT_DIPUNUSED_DIPLOC( 0x02, 0x02, "SWB:2" );
            PORT_DIPUNUSED_DIPLOC( 0x04, 0x04, "SWB:3" );
            PORT_DIPUNUSED_DIPLOC( 0x08, 0x08, "SWB:4" );
            PORT_DIPUNUSED_DIPLOC( 0x10, 0x10, "SWB:5" );
            PORT_DIPUNUSED_DIPLOC( 0x20, 0x20, "SWB:6" );
            PORT_DIPUNUSED_DIPLOC( 0x40, 0x40, "SWB:7" );
            /* If you don't like the description, feel free to change it */
            PORT_DIPNAME( 0x80, 0x80, "SW 0 Read From" );        PORT_DIPLOCATION("SWB:8");
            PORT_DIPSETTING(    0x80, "Port $0D" );
            PORT_DIPSETTING(    0x00, "Port $10" );

            INPUT_PORTS_END();
        }


        /*************************************
         *  Game-specific port definitions
         *************************************/

        //static INPUT_PORTS_START( starjack )

        //static INPUT_PORTS_START( starjacks )

        //static INPUT_PORTS_START( regulus )

        //static INPUT_PORTS_START( reguluso )

        //static INPUT_PORTS_START( upndown )

        //static INPUT_PORTS_START( mrviking )

        //static INPUT_PORTS_START( mrvikingj )

        //static INPUT_PORTS_START( swat )

        //static INPUT_PORTS_START( flicky )

        //static INPUT_PORTS_START( flickys1 )

        //static INPUT_PORTS_START( flickys2 )

        //static INPUT_PORTS_START( wmatch )

        //static INPUT_PORTS_START( bullfgt )

        //static INPUT_PORTS_START( spatter )

        //static INPUT_PORTS_START( pitfall2 )

        //static INPUT_PORTS_START( pitfall2u )

        //static INPUT_PORTS_START( seganinj )

        //static INPUT_PORTS_START( imsorry )

        //static INPUT_PORTS_START( teddybb )

        //static INPUT_PORTS_START( hvymetal )

        //static INPUT_PORTS_START( myhero )

        //static INPUT_PORTS_START( 4dwarrio )

        //static INPUT_PORTS_START( brain )

        //static INPUT_PORTS_START( gardia )

        //static INPUT_PORTS_START( raflesia )

        //static INPUT_PORTS_START( wboy )

        //static INPUT_PORTS_START( wboy3 )

        //static INPUT_PORTS_START( wbdeluxe )

        //static INPUT_PORTS_START( wboyu )

        //static INPUT_PORTS_START( nob )


        //static INPUT_PORTS_START( choplift )
        void construct_ioport_choplift(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_INCLUDE(construct_ioport_system1_generic, ref errorbuf);

            PORT_MODIFY("SWB");
            PORT_DIPNAME( 0x0f, 0x0f, DEF_STR( Coin_A ) );       PORT_DIPLOCATION("SWA:1,2,3,4");
            PORT_DIPSETTING(    0x07, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x08, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x09, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x05, "2 Coins/1 Credit 5/3 6/4" );
            PORT_DIPSETTING(    0x04, "2 Coins/1 Credit 4/3" );
            PORT_DIPSETTING(    0x0f, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x01, "1 Coin/1 Credit 2/3" );
            PORT_DIPSETTING(    0x02, "1 Coin/1 Credit 4/5" );
            PORT_DIPSETTING(    0x03, "1 Coin/1 Credit 5/6" );
            PORT_DIPSETTING(    0x06, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x0e, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x0d, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0x0c, DEF_STR( _1C_4C ) );
            PORT_DIPSETTING(    0x0b, DEF_STR( _1C_5C ) );
            PORT_DIPSETTING(    0x0a, DEF_STR( _1C_6C ) );
            PORT_DIPNAME( 0xf0, 0xf0, DEF_STR( Coin_B ) );       PORT_DIPLOCATION("SWA:5,6,7,8");
            PORT_DIPSETTING(    0x70, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x80, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x90, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x50, "2 Coins/1 Credit 5/3 6/4" );
            PORT_DIPSETTING(    0x40, "2 Coins/1 Credit 4/3" );
            PORT_DIPSETTING(    0xf0, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x10, "1 Coin/1 Credit 2/3" );
            PORT_DIPSETTING(    0x20, "1 Coin/1 Credit 4/5" );
            PORT_DIPSETTING(    0x30, "1 Coin/1 Credit 5/6" );
            PORT_DIPSETTING(    0x60, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0xe0, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0xd0, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0xc0, DEF_STR( _1C_4C ) );
            PORT_DIPSETTING(    0xb0, DEF_STR( _1C_5C ) );
            PORT_DIPSETTING(    0xa0, DEF_STR( _1C_6C ) );

            PORT_MODIFY("SWA");
            PORT_DIPNAME( 0x01, 0x00, DEF_STR( Cabinet ) );      PORT_DIPLOCATION("SWB:1");
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x01, DEF_STR( Cocktail ) );
            PORT_DIPNAME( 0x02, 0x00, DEF_STR( Demo_Sounds ) );  PORT_DIPLOCATION("SWB:2");
            PORT_DIPSETTING(    0x02, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x0c, 0x0c, DEF_STR( Lives ) );        PORT_DIPLOCATION("SWB:3,4");
            PORT_DIPSETTING(    0x08, "2" );
            PORT_DIPSETTING(    0x0c, "3" );
            PORT_DIPSETTING(    0x04, "4" );
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) );
            PORT_DIPNAME( 0x10, 0x10, DEF_STR( Bonus_Life ) );   PORT_DIPLOCATION("SWB:5");
            PORT_DIPSETTING(    0x10, "20k 70k 120k 170k" );
            PORT_DIPSETTING(    0x00, "50k 100k 150k 200k" );
            PORT_DIPNAME( 0x20, 0x00, DEF_STR( Difficulty ) );   PORT_DIPLOCATION("SWB:6");
            PORT_DIPSETTING(    0x00, DEF_STR( Hard ) );
            PORT_DIPSETTING(    0x20, DEF_STR( Easy ) );
            PORT_DIPUNUSED_DIPLOC( 0x40, 0x40, "SWB:7" );
            PORT_DIPUNUSED_DIPLOC( 0x80, 0x80, "SWB:8" );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( shtngmst )

        //static INPUT_PORTS_START( wboysys2 )

        //static INPUT_PORTS_START( blockgal )

        //static INPUT_PORTS_START( blockgalb )

        //static INPUT_PORTS_START( tokisens )

        //static INPUT_PORTS_START( tokisensa )

        //static INPUT_PORTS_START( wbml )

        //static INPUT_PORTS_START( dakkochn )

        //static INPUT_PORTS_START( ufosensi )
    }


    partial class system1_state : driver_device
    {
        /*************************************
         *  Graphics definitions
         *************************************/
        static readonly gfx_layout charlayout = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,3),
            3,
            new u32 [] { RGN_FRAC(0,3), RGN_FRAC(1,3), RGN_FRAC(2,3) },
            STEP8(0,1),
            STEP8(0,8),
            8*8
        );

        //static GFXDECODE_START( gfx_system1 )
        static readonly gfx_decode_entry [] gfx_system1 =
        {
            GFXDECODE_ENTRY( "tiles", 0, charlayout, 0, 256 )
            //GFXDECODE_END
        };


        /*************************************
         *  Machine driver
         *************************************/
        /* original board with 64kbit ROMs and an 8255 PPI for outputs */
        void sys1ppi(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, MASTER_CLOCK);  /* not really, see notes above */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, system1_map);
            m_maincpu.op0.memory().set_addrmap(AS_IO, system1_ppi_io_map);
            m_maincpu.op0.execute().set_vblank_int("screen", irq0_line_hold);

            Z80(config, m_soundcpu, SOUND_CLOCK / 2);
            m_soundcpu.op0.memory().set_addrmap(AS_PROGRAM, sound_map);

            TIMER(config, "soundirq", 0).configure_scanline(soundirq_gen, "screen", 32, 64);

            config.set_maximum_quantum(attotime.from_hz(6000));

            I8255A(config, m_ppi8255);
            m_ppi8255.op0.out_pa_callback().set(soundport_w).reg();
            m_ppi8255.op0.out_pb_callback().set(videomode_w).reg();
            m_ppi8255.op0.out_pc_callback().set(sound_control_w).reg();

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_video_attributes(VIDEO_ALWAYS_UPDATE);  /* needed for proper hardware collisions */
            m_screen.op0.set_raw(MASTER_CLOCK / 2, 640, 0, 512, 260, 0, 224);
            m_screen.op0.set_screen_update(screen_update_system1);
            m_screen.op0.set_palette(m_palette);

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_system1);
            PALETTE(config, m_palette, system1_palette).set_entries(2048, 256);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            GENERIC_LATCH_8(config, m_soundlatch);

            SN76489A(config, "sn1", SOUND_CLOCK / 4).disound.add_route(ALL_OUTPUTS, "mono", 0.50);

            /* 2nd SN's clock is selectable via jumper */
            SN76489A(config, "sn2", SOUND_CLOCK / 2).disound.add_route(ALL_OUTPUTS, "mono", 0.50);
        }


#if false
        /* reduced visible area for scrolling games */
        void system1_state::sys1ppis(machine_config &config)
        {
            sys1ppi(config);

            /* video hardware */
            m_screen->set_visarea(2*(0*8+8), 2*(32*8-1-8), 0*8, 28*8-1);
        }

        /* revised board with 128kbit ROMs and a Z80 PIO for outputs */
        void system1_state::sys1pio(machine_config &config)
        {
            sys1ppi(config);

            m_maincpu->set_addrmap(AS_IO, &system1_state::system1_pio_io_map);

            config.device_remove("ppi8255");

            Z80PIO(config, m_pio, MASTER_CLOCK);
            m_pio->out_pa_callback().set(FUNC(system1_state::soundport_w));
            m_pio->out_ardy_callback().set_inputline(m_soundcpu, INPUT_LINE_NMI);
            m_pio->out_pb_callback().set(FUNC(system1_state::videomode_w));
        }

        void system1_state::encrypted_sys1ppi_maps(machine_config &config)
        {
            m_maincpu->set_addrmap(AS_PROGRAM, &system1_state::system1_map);
            m_maincpu->set_addrmap(AS_OPCODES, &system1_state::decrypted_opcodes_map);
            m_maincpu->set_addrmap(AS_IO, &system1_state::system1_ppi_io_map);
            m_maincpu->set_vblank_int("screen", FUNC(system1_state::irq0_line_hold));
        }

        void system1_state::encrypted_sys1pio_maps(machine_config &config)
        {
            m_maincpu->set_addrmap(AS_PROGRAM, &system1_state::system1_map);
            m_maincpu->set_addrmap(AS_OPCODES, &system1_state::decrypted_opcodes_map);
            m_maincpu->set_addrmap(AS_IO, &system1_state::system1_pio_io_map);
            m_maincpu->set_vblank_int("screen", FUNC(system1_state::irq0_line_hold));
        }

        void system1_state::encrypted_sys2_mc8123_maps(machine_config &config)
        {
            m_maincpu->set_addrmap(AS_PROGRAM, &system1_state::system1_map);
            m_maincpu->set_addrmap(AS_OPCODES, &system1_state::banked_decrypted_opcodes_map);
            m_maincpu->set_addrmap(AS_IO, &system1_state::system1_ppi_io_map);
            m_maincpu->set_vblank_int("screen", FUNC(system1_state::irq0_line_hold));
        }

        void system1_state::sys1pioxb(machine_config &config)
        {
            sys1pio(config);
            MC8123(config.replace(), m_maincpu, MASTER_CLOCK);
            encrypted_sys1pio_maps(config);
        }

        //void system1_state::blockgal(machine_config &config)

        void system1_state::sys1ppix_315_5178(machine_config &config)
        {
            sys1ppi(config);
            segacrp2_z80_device &z80(SEGA_315_5178(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(m_decrypted_opcodes);
        }

        void system1_state::sys1ppix_315_5179(machine_config &config)
        {
            sys1ppi(config);
            segacrp2_z80_device &z80(SEGA_315_5179(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(m_decrypted_opcodes);
        }

        void system1_state::sys1ppix_315_5051(machine_config &config)
        {
            sys1ppi(config);
            segacrpt_z80_device &z80(SEGA_315_5051(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1ppix_315_5048(machine_config &config)
        {
            sys1ppi(config);
            segacrpt_z80_device &z80(SEGA_315_5048(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1ppix_315_5033(machine_config &config)
        {
            sys1ppi(config);
            segacrpt_z80_device &z80(SEGA_315_5033(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1ppix_315_5065(machine_config &config)
        {
            sys1ppi(config);
            segacrpt_z80_device &z80(SEGA_315_5065(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1ppix_315_5098(machine_config &config)
        {
            sys1ppi(config);
            segacrpt_z80_device &z80(SEGA_315_5098(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1piox_315_5177(machine_config &config)
        {
            sys1pio(config);
            segacrp2_z80_device &z80(SEGA_315_5177(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(m_decrypted_opcodes);
        }

        void system1_state::sys1piox_315_5162(machine_config &config)
        {
            sys1pio(config);
            segacrp2_z80_device &z80(SEGA_315_5162(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(m_decrypted_opcodes);
        }

        void system1_state::sys1piox_317_0006(machine_config &config)
        {
            sys1pio(config);
            segacrp2_z80_device &z80(SEGA_317_0006(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(m_decrypted_opcodes);
        }

        void system1_state::sys1piox_315_5135(machine_config &config)
        {
            sys1pio(config);
            segacrpt_z80_device &z80(SEGA_315_5135(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1piox_315_5132(machine_config &config)
        {
            sys1pio(config);
            segacrpt_z80_device &z80(SEGA_315_5132(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1piox_315_5155(machine_config &config)
        {
            sys1pio(config);
            segacrpt_z80_device &z80(SEGA_315_5155(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1piox_315_5110(machine_config &config)
        {
            sys1pio(config);
            segacrpt_z80_device &z80(SEGA_315_5110(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1piox_315_5051(machine_config &config)
        {
            sys1pio(config);
            segacrpt_z80_device &z80(SEGA_315_5051(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1piox_315_5098(machine_config &config)
        {
            sys1pio(config);
            segacrpt_z80_device &z80(SEGA_315_5098(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1piox_315_5102(machine_config &config)
        {
            sys1pio(config);
            segacrpt_z80_device &z80(SEGA_315_5102(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1piox_315_5133(machine_config &config)
        {
            sys1pio(config);
            segacrpt_z80_device &z80(SEGA_315_5133(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1piox_315_5093(machine_config &config)
        {
            sys1pio(config);
            segacrpt_z80_device &z80(SEGA_315_5093(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1piox_315_5065(machine_config &config)
        {
            sys1pio(config);
            segacrpt_z80_device &z80(SEGA_315_5065(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        /* reduced visible area for scrolling games */
        void system1_state::sys1pios(machine_config &config)
        {
            sys1pio(config);
            m_screen->set_visarea(2*(0*8+8), 2*(32*8-1-8), 0*8, 28*8-1);
        }

        void system1_state::sys1piosx_315_5099(machine_config &config)
        {
            sys1pio(config);
            segacrpt_z80_device &z80(SEGA_315_5099(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1piosx_315_spat(machine_config &config)
        {
            sys1pios(config);
            segacrpt_z80_device &z80(SEGA_315_SPAT(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1pio_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1ppisx_315_5064(machine_config &config)
        {
            sys1ppis(config);
            segacrpt_z80_device &z80(SEGA_315_5064(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }

        void system1_state::sys1ppisx_315_5041(machine_config &config)
        {
            sys1ppis(config);
            segacrpt_z80_device &z80(SEGA_315_5041(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(":decrypted_opcodes");
        }
#endif


        /* this describes the additional 8751 MCU when present */
        void mcu(machine_config config)
        {
            /* basic machine hardware */
            m_maincpu.op0.execute().remove_vblank_int();

            I8751(config, m_mcu, SOUND_CLOCK);
            m_mcu.op0.memory().set_addrmap(AS_IO, mcu_io_map);
            m_mcu.op0.port_out_cb<u32_const_1>().set(mcu_control_w).reg();

            m_screen.op0.screen_vblank().set_inputline("mcu", MCS51_INT0_LINE).reg();
            // This interrupt is driven by pin 15 of a PAL16R4 (315-5138 on Choplifter), based on the vertical count.
            // The actual duty cycle likely differs from VBLANK, which is another output from the same PAL.

            TIMER(config, "mcu_t0", 0).configure_periodic(mcu_t0_callback, attotime.from_usec(2500));
        }


        //void system1_state::nob(machine_config &config)

        //void system1_state::nobm(machine_config &config)


        /* system2 video */
        void sys2(machine_config config)
        {
            sys1ppi(config);

            MCFG_MACHINE_START_OVERRIDE(config, machine_start_system2);

            /* video hardware */
            MCFG_VIDEO_START_OVERRIDE(config, video_start_system2);
            m_screen.op0.set_screen_update(screen_update_system2);
        }


#if false
        void system1_state::sys2x(machine_config &config)
        {
            sys2(config);
            m_maincpu->set_addrmap(AS_OPCODES, &system1_state::decrypted_opcodes_map);
        }

        void system1_state::sys2_315_5177(machine_config &config)
        {
            sys2(config);
            segacrp2_z80_device &z80(SEGA_315_5177(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(m_decrypted_opcodes);
        }

        void system1_state::sys2_315_5176(machine_config &config)
        {
            sys2(config);
            segacrp2_z80_device &z80(SEGA_315_5176(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(m_decrypted_opcodes);
        }

        void system1_state::sys2_317_0006(machine_config &config)
        {
            sys2(config);
            segacrp2_z80_device &z80(SEGA_317_0006(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(m_decrypted_opcodes);
        }

        void system1_state::sys2_317_0007(machine_config &config)
        {
            sys2(config);
            segacrp2_z80_device &z80(SEGA_317_0007(config.replace(), m_maincpu, MASTER_CLOCK));
            encrypted_sys1ppi_maps(config);
            z80.set_decrypted_tag(m_decrypted_opcodes);
        }

        void system1_state::sys2xb(machine_config &config)
        {
            sys2(config);
            MC8123(config.replace(), m_maincpu, MASTER_CLOCK);
            encrypted_sys2_mc8123_maps(config);
        }

        void system1_state::sys2xboot(machine_config &config)
        {
            sys2(config);
            m_maincpu->set_addrmap(AS_OPCODES, &system1_state::banked_decrypted_opcodes_map);
        }

        void system1_state::sys2m(machine_config &config)
        {
            sys2(config);
            mcu(config);
        }
#endif


        /* system2 with rowscroll */
        void sys2row(machine_config config)
        {
            sys2(config);

            /* video hardware */
            m_screen.op0.set_screen_update(screen_update_system2_rowscroll);
        }


#if false
        void system1_state::sys2rowxb(machine_config &config)
        {
            sys2row(config);
            MC8123(config.replace(), m_maincpu, MASTER_CLOCK);
            encrypted_sys2_mc8123_maps(config);
        }

        void system1_state::sys2rowxboot(machine_config &config)
        {
            sys2row(config);
            m_maincpu->set_addrmap(AS_OPCODES, &system1_state::banked_decrypted_opcodes_map);
        }
#endif


        public void sys2rowm(machine_config config)
        {
            sys2row(config);
            mcu(config);
        }
    }


    partial class system1 : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definition(s)
         *
         *************************************/
        //ROM_START( starjack )

        //ROM_START( starjacks )

        //ROM_START( upndown )

        //ROM_START( upndownu )

        //ROM_START( regulus )

        //ROM_START( reguluso )

        //ROM_START( regulusu ) /* Sega game ID# 834-5328-02 REGULUS */

        //ROM_START( mrviking )

        //ROM_START( mrvikingj )

        //ROM_START( swat )

        //ROM_START( flicky )

        //ROM_START( flickya )

        //ROM_START( flickys2 )

        //ROM_START( flickys1 )

        //ROM_START( flickyo )

        //ROM_START( flickyup )

        //ROM_START( wmatch )

        //ROM_START( bullfgt )

        //ROM_START( thetogyu )

        //ROM_START( spatter )

        //ROM_START( spattera )

        //ROM_START( ssanchan )

        //ROM_START( pitfall2 )

        //ROM_START( pitfall2a )

        //ROM_START( pitfall2u )

        //ROM_START( seganinj )

        //ROM_START( seganinju )

        //ROM_START( seganinja )

        //ROM_START( nprinces )

        //ROM_START( nprinceso )

        //ROM_START( nprincesu )

        //ROM_START( nprincesb )

        //ROM_START( ninja )

        //ROM_START( imsorry )

        //ROM_START( imsorryj )

        //ROM_START( teddybb )

        //ROM_START( teddybbo )

        //ROM_START( teddybbobl ) // data in romset is an exact match for teddybbo, including encryption

        //ROM_START( hvymetal )

        //ROM_START( myhero )

        //ROM_START( sscandal )

        //ROM_START( myherobl )

        //ROM_START( myherok )

        //ROM_START( 4dwarrio )

        //ROM_START( shtngmst )


        /*
            Choplifter (8751 315-5151)
            Year: 1985
            System 2

            Main Board 834-5795
        */
        //ROM_START( choplift )
        static readonly tiny_rom_entry [] rom_choplift = 
        {
            ROM_REGION( 0x20000, "maincpu", 0 ),
            ROM_LOAD( "epr-7124.ic90",  0x00000, 0x8000, CRC("678d5c41") + SHA1("7553979f78270c2ddc5b3f3ebf7817ead8e08de7") ),
            ROM_LOAD( "epr-7125.ic91",  0x10000, 0x8000, CRC("f5283498") + SHA1("1ad40f6d7b4cd18212ee56917240c0796f1a4ec2") ),
            ROM_LOAD( "epr-7126.ic92",  0x18000, 0x8000, CRC("dbd192ab") + SHA1("03d280c82599a14fc6a2065d57c6241cdc6f1143") ),

            ROM_REGION( 0x10000, "soundcpu", 0 ),
            ROM_LOAD( "epr-7130.ic126", 0x0000, 0x8000, CRC("346af118") + SHA1("ef579818a45b8ebb276d5832092b26e232d5a737") ),

            ROM_REGION( 0x1000, "mcu", 0 ),
            ROM_LOAD( "315-5151.ic74",  0x00000, 0x1000, CRC("1377a6ef") + SHA1("b85acd7292e5480c98af1a0492b6b5d3f9b1716c") ),

            ROM_REGION( 0x18000, "tiles", 0 ),
            ROM_LOAD( "epr-7127.ic4",   0x00000, 0x8000, CRC("1e708f6d") + SHA1("b975e13bdc44105e7a15c2694e3ec53b60e23e5e") ),
            ROM_LOAD( "epr-7128.ic5",   0x08000, 0x8000, CRC("b922e787") + SHA1("16087671ec7de25f749b5fd66409d48ef7b35820") ),
            ROM_LOAD( "epr-7129.ic6",   0x10000, 0x8000, CRC("bd3b6e6e") + SHA1("c66f21b98cb8fc61a9318041ac1812c13099d974") ),

            ROM_REGION( 0x20000, "sprites", 0 ),
            ROM_LOAD( "epr-7121.ic87",  0x00000, 0x8000, CRC("f2b88f73") + SHA1("2b06da1beabbea82d502fbe12f6ec3ef26056edd") ),
            ROM_LOAD( "epr-7120.ic86",  0x08000, 0x8000, CRC("517d7fd3") + SHA1("3fb5c00224920c3f62fb86e82caf0fee2293e1e2") ),
            ROM_LOAD( "epr-7123.ic89",  0x10000, 0x8000, CRC("8f16a303") + SHA1("5f2465505f001dc052e9de4cf66bc1d53fc8c7da") ),
            ROM_LOAD( "epr-7122.ic88",  0x18000, 0x8000, CRC("7c93f160") + SHA1("6ab156cad7556808496070f8b02a708ce405c492") ),

            ROM_REGION( 0x0300, "color_proms", 0 ),
            ROM_LOAD( "pr7119.ic20",    0x0000, 0x0100, CRC("b2a8260f") + SHA1("36c1debb4b3f2f190a25b18d533319d7380416de") ), /* palette red component */
            ROM_LOAD( "pr7118.ic14",    0x0100, 0x0100, CRC("693e20c7") + SHA1("9ebf4bd2c30ddd9648bc4b41c7739cfdf80100da") ), /* palette green component */
            ROM_LOAD( "pr7117.ic8",     0x0200, 0x0100, CRC("4124307e") + SHA1("cee28d891e6ce732c43a61acb5beeafd2200cf37") ), /* palette blue component */

            ROM_REGION( 0x0100, "lookup_proms", 0 ),
            ROM_LOAD( "pr5317.ic28",    0x0000, 0x0100, CRC("648350b8") + SHA1("c7986aa9127ef5b50b845434cb4e81dff9861cd2") ),

            ROM_REGION( 0x0618, "plds", 0 ),
            ROM_LOAD( "315-5152.bin",   0x00000, 0x0104, CRC("2c9229b4") + SHA1("9755013afcf89f99d7a399c7e223e027761cf89a") ), /* PAL16R4A located at IC10. */
            ROM_LOAD( "315-5138.bin",   0x00000, 0x0104, CRC("dd223015") + SHA1("8d70f91b118e8653dda1efee3eaea287ae63809f") ), /* TI PAL16R4NC located at IC11. */
            ROM_LOAD( "315-5139.bin",   0x00000, 0x0104, NO_DUMP ), /* CK2605 located at IC50. */
            ROM_LOAD( "315-5025.bin",   0x00000, 0x0104, NO_DUMP ), /* Located at IC7. */
            ROM_LOAD( "315-5025.bin",   0x00000, 0x0104, NO_DUMP ), /* Located at IC13. */
            ROM_LOAD( "315-5025.bin",   0x00000, 0x0104, NO_DUMP ), /* Located at IC19. */

            ROM_END,
        };


        //ROM_START( chopliftu )

        //ROM_START( chopliftbl )

        //ROM_START( raflesia )

        //ROM_START( raflesiau )

        //ROM_START( wboy )

        //ROM_START( wboyub ) // seems to be the same as 'wboy' but a different rom layout and external encryption handling

        //ROM_START( wboyo )

        //ROM_START( wboy2 )

        //ROM_START( wboy2u )

        //ROM_START( wboy3 )

        //ROM_START( wboy4 )

        //ROM_START( wboy5 )

        //ROM_START( wboy6 )

        //ROM_START( wboyu )

        //ROM_START( wboysys2 )

        //ROM_START( wboysys2a )

        //ROM_START( wbdeluxe )

        //ROM_START( gardia ) /* Factory conversion, Sega game ID# 834-6119-04 GARDIA (CVT) */

        //ROM_START( gardiab )

        //ROM_START( gardiaj ) /* Sega game ID# 834-6119-02 GARDIA */

        //ROM_START( brain )

        //ROM_START( tokisens ) /* Sega game ID#  834-6409 - Sega MC-8123, 317-0040 */

        //ROM_START( tokisensa )

        //ROM_START( wbml ) /* Sega game ID# 834-6409 MONSTER LAND */

        //ROM_START( wbmljo ) /* Sega game ID# 834-6409 MONSTER LAND */

        //ROM_START( wbmld )

        //ROM_START( wbmljod )

        //ROM_START( wbmljb )

        //ROM_START( wbmlb )

        //ROM_START( wbmlbg )

        //ROM_START( wbmlbge )

        //ROM_START( wbmlvc )

        //ROM_START( wbmlvcd )

        //ROM_START( dakkochn )

        //ROM_START( ufosensi )

        //ROM_START( ufosensib )

        //ROM_START( blockgal )

        //ROM_START( blockgalb )

        //ROM_START( nob )

        //ROM_START( nobb )
    }


    partial class system1_state : driver_device
    {
        /*************************************
         *  Generic driver initialization
         *************************************/
#if false
        void system1_state::init_bank44()
        {
            m_videomode_custom = &system1_state::bank44_custom_w;
        }
#endif

        public void init_bank0c()
        {
            m_videomode_custom = bank0c_custom_w;
        }


        //void system1_state::init_myherok()

        //void system1_state::init_blockgal()

        //void system1_state::init_wbml()

        //void system1_state::init_tokisens()

        //void system1_state::init_dakkochn()

        //u8 system1_state::nob_start_r()

        //void system1_state::init_nob()

        //void system1_state::init_nobb()

        //void system1_state::init_bootleg()

        //void system1_state::init_bootsys2()

        //void system1_state::init_bootsys2d()

        //void system1_state::init_shtngmst()
    }


    partial class system1 : construct_ioport_helper
    {
        /*************************************
         *
         *  Game driver(s)
         *
         *************************************/

        static void system1_state_sys2rowm(machine_config config, device_t device) { system1_state system1_state = (system1_state)device; system1_state.sys2rowm(config); }

        static void system1_state_init_bank0c(device_t owner) { system1_state system1_state = (system1_state)owner; system1_state.init_bank0c(); }

        static system1 m_system1 = new system1();

        static device_t device_creator_choplift(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new system1_state(mconfig, type, tag); }

        /* System 2 */
        public static readonly game_driver driver_choplift = GAME(device_creator_choplift, rom_choplift, "1985", "choplift", "0", system1_state_sys2rowm, m_system1.construct_ioport_choplift, system1_state_init_bank0c, ROT0, "Sega (licensed from Dan Gorlin)", "Choplifter (8751 315-5151)", MACHINE_SUPPORTS_SAVE);
    }
}
