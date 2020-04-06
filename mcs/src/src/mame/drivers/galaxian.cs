// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    partial class galaxian_state : driver_device
    {
        /*************************************
         *
         *  Interrupts
         *
         *************************************/

        //WRITE_LINE_MEMBER(galaxian_state::vblank_interrupt_w)
        void vblank_interrupt_w(int state)
        {
            /* interrupt line is clocked at VBLANK */
            /* a flip-flop at 6F is held in the preset state based on the NMI ON signal */
            if (state != 0 && m_irq_enabled != 0)
                m_maincpu.target.set_input_line(m_irq_line, ASSERT_LINE);
        }


        //WRITE_LINE_MEMBER(galaxian_state::tenspot_interrupt_w)


        //WRITE8_MEMBER(galaxian_state::irq_enable_w)
        void irq_enable_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* the latched D0 bit here goes to the CLEAR line on the interrupt flip-flop */
            m_irq_enabled = (byte)(data & 1);

            /* if CLEAR is held low, we must make sure the interrupt signal is clear */
            if (m_irq_enabled == 0)
                m_maincpu.target.set_input_line(m_irq_line, CLEAR_LINE);
        }


        /*************************************
         *
         *  DRIVER latch control
         *
         *************************************/

        //WRITE8_MEMBER(galaxian_state::start_lamp_w)
        void start_lamp_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* offset 0 = 1P START LAMP */
            /* offset 1 = 2P START LAMP */
            m_lamps[offset] = BIT(data, 0);
        }


        //WRITE8_MEMBER(galaxian_state::coin_lock_w)
        void coin_lock_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* many variants and bootlegs don't have this */
            machine().bookkeeping().coin_lockout_global_w(~data & 1);
        }


        //WRITE8_MEMBER(galaxian_state::coin_count_0_w)
        void coin_count_0_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            machine().bookkeeping().coin_counter_w(0, data & 1);
        }


        //WRITE8_MEMBER(galaxian_state::coin_count_1_w)
        void coin_count_1_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            machine().bookkeeping().coin_counter_w(1, data & 1);
        }


        /*************************************
         *
         *  General Konami sound I/O
         *
         *************************************/

#if false
        READ8_MEMBER(galaxian_state::konami_ay8910_r)
        WRITE8_MEMBER(galaxian_state::konami_ay8910_w)
#endif


        //WRITE8_MEMBER(galaxian_state::konami_sound_control_w)
        void konami_sound_control_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            byte old = m_konami_sound_control;
            m_konami_sound_control = data;

            /* the inverse of bit 3 clocks the flip flop to signal an INT */
            /* it is automatically cleared on the acknowledge */
            if ((old & 0x08) != 0 && (data & 0x08) == 0)
                m_audiocpu.target.set_input_line(0, HOLD_LINE);

            /* bit 4 is sound disable */
            machine().sound().system_mute((data & 0x10) != 0);
        }


        //READ8_MEMBER(galaxian_state::konami_sound_timer_r)
        u8 konami_sound_timer_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            /*
                The timer is clocked at KONAMI_SOUND_CLOCK and cascades through a
                series of counters. It first encounters a chained pair of 4-bit
                counters in an LS393, which produce an effective divide-by-256. Next
                it enters the divide-by-2 counter in an LS93, followed by the
                divide-by-8 counter. Finally, it clocks a divide-by-5 counter in an
                LS90, followed by the divide-by-2 counter. This produces an effective
                period of 16*16*2*8*5*2 = 40960 clocks.

                The clock for the sound CPU comes from output C of the first
                divide-by-16 counter, or KONAMI_SOUND_CLOCK/8. To recover the
                current counter index, we use the sound cpu clock times 8 mod
                16*16*2*8*5*2.
            */

            UInt32 cycles = (UInt32)((m_audiocpu.target.total_cycles() * 8) % (UInt64)(16*16*2*8*5*2));
            byte hibit = 0;

            /* separate the high bit from the others */
            if (cycles >= 16*16*2*8*5)
            {
                hibit = 1;
                cycles -= 16*16*2*8*5;
            }

            /* the top bits of the counter index map to various bits here */
            return (byte)((hibit << 7) |           /* B7 is the output of the final divide-by-2 counter */
                    (BIT(cycles,14) << 6) | /* B6 is the high bit of the divide-by-5 counter */
                    (BIT(cycles,13) << 5) | /* B5 is the 2nd highest bit of the divide-by-5 counter */
                    (BIT(cycles,11) << 4) | /* B4 is the high bit of the divide-by-8 counter */
                    0x0e);                   /* assume remaining bits are high, except B0 which is grounded */
        }


        static readonly string [] konami_sound_filter_w_ayname = new string[2] { "8910.0", "8910.1" };

        //WRITE8_MEMBER(galaxian_state::konami_sound_filter_w)
        void konami_sound_filter_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            if (m_netlist != null)
            {
                /* the offset is used as data, 6 channels * 2 bits each */
                /* AV0 .. AV5  ==> AY8910 #2 - 3C */
                /* AV6 .. AV11 ==> AY8910 #1 - 3D */
                for (int which = 0; which < 2; which++)
                {
                    if (m_ay8910.op(which) != null)
                    {
                        for (int flt = 0; flt < 6; flt++)
                        {
                            int fltnum = (flt + 6 * which);
                            uint8_t bit = (uint8_t)((offset >> (flt + 6 * (1 - which))) & 1);

                            /* low bit goes to 0.22uF capacitor = 220000pF  */
                            /* high bit goes to 0.047uF capacitor = 47000pF */
                            m_filter_ctl.op(fltnum).target.write(bit);
                        }
                    }
                }
            }
        }


        //WRITE8_MEMBER(galaxian_state::konami_portc_0_w)
        void konami_portc_0_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            logerror("{0}:ppi0_portc_w = {1}\n", machine().describe_context(), data);  // %02X
        }


        //WRITE8_MEMBER(galaxian_state::konami_portc_1_w)
        void konami_portc_1_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            logerror("{0}:ppi1_portc_w = {1}\n", machine().describe_context(), data);  // %02X
        }


        /*************************************
         *
         *  Frogger I/O
         *
         *************************************/

        //READ8_MEMBER(galaxian_state::frogger_ppi8255_r)
        u8 frogger_ppi8255_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            /* the decoding here is very simplistic, and you can address both simultaneously */
            byte result = 0xff;
            if ((offset & 0x1000) != 0) result &= m_ppi8255.op(1).target.read((offset >> 1) & 3);
            if ((offset & 0x2000) != 0) result &= m_ppi8255.op(0).target.read((offset >> 1) & 3);
            return result;
        }


        //WRITE8_MEMBER(galaxian_state::frogger_ppi8255_w)
        void frogger_ppi8255_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* the decoding here is very simplistic, and you can address both simultaneously */
            if ((offset & 0x1000) != 0) m_ppi8255.op(1).target.write((offset >> 1) & 3, data);
            if ((offset & 0x2000) != 0) m_ppi8255.op(0).target.write((offset >> 1) & 3, data);
        }


        //READ8_MEMBER(galaxian_state::frogger_ay8910_r)
        u8 frogger_ay8910_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            /* the decoding here is very simplistic */
            byte result = 0xff;
            if ((offset & 0x40) != 0) result &= m_ay8910.op(0).target.data_r();
            return result;
        }


        //WRITE8_MEMBER(galaxian_state::frogger_ay8910_w)
        void frogger_ay8910_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* the decoding here is very simplistic */
            /* AV6,7 ==> AY8910 #1 */
            if ((offset & 0x40) != 0)
                m_ay8910.op(0).target.data_w(data);
            else if ((offset & 0x80) != 0)
                m_ay8910.op(0).target.address_w(data);
        }


        //READ8_MEMBER(galaxian_state::frogger_sound_timer_r)
        u8 frogger_sound_timer_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            /* same as regular Konami sound but with bits 3,5 swapped */
            byte konami_value = konami_sound_timer_r(space, 0);
            return (byte)bitswap(konami_value, 7,6,3,4,5,2,1,0);
        }


        /*************************************
         *
         *  Decryption helpers
         *
         *************************************/

#if false
        void galaxian_state::decode_mooncrst(int length, uint8_t *dest)
        {
            uint8_t *rom = memregion("maincpu")->base();
            int offs;

            for (offs = 0; offs < length; offs++)
            {
                uint8_t data = rom[offs];
                uint8_t res = data;
                if (BIT(data,1)) res ^= 0x40;
                if (BIT(data,5)) res ^= 0x04;
                if ((offs & 1) == 0) res = BITSWAP8(res,7,2,5,4,3,6,1,0);
                dest[offs] = res;
            }
        }


        void galaxian_state::decode_checkman()
        {
            /*
                                     Encryption Table
                                     ----------------
                +---+---+---+------+------+------+------+------+------+------+------+
                |A2 |A1 |A0 |D7    |D6    |D5    |D4    |D3    |D2    |D1    |D0    |
                +---+---+---+------+------+------+------+------+------+------+------+
                | 0 | 0 | 0 |D7    |D6    |D5    |D4    |D3    |D2    |D1    |D0^^D6|
                | 0 | 0 | 1 |D7    |D6    |D5    |D4    |D3    |D2    |D1^^D5|D0    |
                | 0 | 1 | 0 |D7    |D6    |D5    |D4    |D3    |D2^^D4|D1^^D6|D0    |
                | 0 | 1 | 1 |D7    |D6    |D5    |D4^^D2|D3    |D2    |D1    |D0^^D5|
                | 1 | 0 | 0 |D7    |D6^^D4|D5^^D1|D4    |D3    |D2    |D1    |D0    |
                | 1 | 0 | 1 |D7    |D6^^D0|D5^^D2|D4    |D3    |D2    |D1    |D0    |
                | 1 | 1 | 0 |D7    |D6    |D5    |D4    |D3    |D2^^D0|D1    |D0    |
                | 1 | 1 | 1 |D7    |D6    |D5    |D4^^D1|D3    |D2    |D1    |D0    |
                +---+---+---+------+------+------+------+------+------+------+------+

                For example if A2=1, A1=1 and A0=0 then D2 to the CPU would be an XOR of
                D2 and D0 from the ROM's. Note that D7 and D3 are not encrypted.

                Encryption PAL 16L8 on cardridge
                         +--- ---+
                    OE --|   U   |-- VCC
                 ROMD0 --|       |-- D0
                 ROMD1 --|       |-- D1
                 ROMD2 --|VER 5.2|-- D2
                    A0 --|       |-- NOT USED
                    A1 --|       |-- A2
                 ROMD4 --|       |-- D4
                 ROMD5 --|       |-- D5
                 ROMD6 --|       |-- D6
                   GND --|       |-- M1 (NOT USED)
                         +-------+
                Pin layout is such that links can replace the PAL if encryption is not used.
            */
            static const uint8_t xortable[8][4] =
            {
                { 6,0,6,0 },
                { 5,1,5,1 },
                { 4,2,6,1 },
                { 2,4,5,0 },
                { 4,6,1,5 },
                { 0,6,2,5 },
                { 0,2,0,2 },
                { 1,4,1,4 }
            };
            uint8_t *rombase = memregion("maincpu")->base();
            uint32_t romlength = memregion("maincpu")->bytes();
            uint32_t offs;

            for (offs = 0; offs < romlength; offs++)
            {
                uint8_t data = rombase[offs];
                uint32_t line = offs & 0x07;

                data ^= (BIT(data,xortable[line][0]) << xortable[line][1]) | (BIT(data,xortable[line][2]) << xortable[line][3]);
                rombase[offs] = data;
            }
        }


        void galaxian_state::decode_dingoe()
        {
            uint8_t *rombase = memregion("maincpu")->base();
            uint32_t romlength = memregion("maincpu")->bytes();
            uint32_t offs;

            for (offs = 0; offs < romlength; offs++)
            {
                uint8_t data = rombase[offs];

                /* XOR bit 4 with bit 2, and bit 0 with bit 5, and invert bit 1 */
                data ^= BIT(data, 2) << 4;
                data ^= BIT(data, 5) << 0;
                data ^= 0x02;

                /* Swap bit0 with bit4 */
                if (offs & 0x02)
                    data = BITSWAP8(data, 7,6,5,0,3,2,1,4);
                rombase[offs] = data;
            }
        }
#endif

        void decode_frogger_sound()
        {
            ListBytesPointer rombase = new ListBytesPointer(memregion("audiocpu").base_());  //uint8_t *rombase = memregion("audiocpu")->base_();
            uint32_t offs;

            /* the first ROM of the sound CPU has data lines D0 and D1 swapped */
            for (offs = 0; offs < 0x800; offs++)
                rombase[offs] = (byte)bitswap(rombase[offs], 7,6,5,4,3,2,0,1);
        }

#if false
        // froggermc has a bigger first ROM of the sound CPU, thus a different decode
        void galaxian_state::decode_froggermc_sound()
        {
            uint8_t *rombase = memregion("audiocpu")->base();
            uint32_t offs;

            /* the first ROM of the sound CPU has data lines D0 and D1 swapped */
            for (offs = 0; offs < 0x1000; offs++)
                rombase[offs] = BITSWAP8(rombase[offs], 7,6,5,4,3,2,0,1);
        }
#endif

        void decode_frogger_gfx()
        {
            ListBytesPointer rombase = new ListBytesPointer(memregion("gfx1").base_());  //uint8_t *rombase = memregion("gfx1")->base_();
            uint32_t offs;

            /* the 2nd gfx ROM has data lines D0 and D1 swapped */
            for (offs = 0x0800; offs < 0x1000; offs++)
                rombase[offs] = (byte)bitswap(rombase[offs], 7,6,5,4,3,2,0,1);
        }


        /*************************************
         *
         *  Memory maps
         *
         *************************************/

        /*
        0000-3fff


        4000-7fff
          4000-47ff -> RAM read/write (10 bits = 0x400)
          4800-4fff -> n/c
          5000-57ff -> /VRAM RD or /VRAM WR (10 bits = 0x400)
          5800-5fff -> /OBJRAM RD or /OBJRAM WR (8 bits = 0x100)
          6000-67ff -> /SW0 or /DRIVER
          6800-6fff -> /SW1 or /SOUND
          7000-77ff -> /DIPSW or LATCH
          7800-7fff -> /WDR or /PITCH

        /DRIVER: (write 6000-67ff)
          D0 = data bit
          A0-A2 = decoder
          6000 -> 1P START
          6001 -> 2P START
          6002 -> COIN LOCKOUT
          6003 -> COIN COUNTER
          6004 -> 1M resistor (controls 555 timer @ 9R)
          6005 -> 470k resistor (controls 555 timer @ 9R)
          6006 -> 220k resistor (controls 555 timer @ 9R)
          6007 -> 100k resistor (controls 555 timer @ 9R)

        /SOUND: (write 6800-6fff)
          D0 = data bit
          A0-A2 = decoder
          6800 -> FS1 (enables 555 timer at 8R)
          6801 -> FS2 (enables 555 timer at 8S)
          6802 -> FS3 (enables 555 timer at 8T)
          6803 -> HIT
          6804 -> n/c
          6805 -> FIRE
          6806 -> VOL1
          6807 -> VOL2

        LATCH: (write 7000-77ff)
          D0 = data bit
          A0-A2 = decoder
          7000 -> n/c
          7001 -> NMI ON
          7002 -> n/c
          7003 -> n/c
          7004 -> STARS ON
          7005 -> n/c
          7006 -> HFLIP
          7007 -> VFLIP

        /PITCH: (write 7800-7fff)
          loads latch at 9J
        */

        /* map derived from schematics */

        void galaxian_map_discrete(address_map map, device_t device)
        {
            map.op(0x6004, 0x6007).mirror(0x07f8).w("cust", (space, offset, data, mem_mask) => { ((galaxian_sound_device)subdevice("cust")).lfo_freq_w(space, offset, data, mem_mask); });  //FUNC(galaxian_sound_device::lfo_freq_w));
            map.op(0x6800, 0x6807).mirror(0x07f8).w("cust", (space, offset, data, mem_mask) => { ((galaxian_sound_device)subdevice("cust")).sound_w(space, offset, data, mem_mask); });  //FUNC(galaxian_sound_device::sound_w));
            map.op(0x7800, 0x7800).mirror(0x07ff).w("cust", (space, offset, data, mem_mask) => { ((galaxian_sound_device)subdevice("cust")).pitch_w(space, offset, data, mem_mask); });  //FUNC(galaxian_sound_device::pitch_w));
        }


        void galaxian_map_base(address_map map, device_t device)
        {
            map.unmap_value_high();
            map.op(0x0000, 0x3fff).rom();
            map.op(0x4000, 0x43ff).mirror(0x0400).ram();
            map.op(0x5000, 0x53ff).mirror(0x0400).ram().w(galaxian_videoram_w).share("videoram");
            map.op(0x5800, 0x58ff).mirror(0x0700).ram().w(galaxian_objram_w).share("spriteram");
            map.op(0x6000, 0x6000).mirror(0x07ff).portr("IN0");
            map.op(0x6000, 0x6001).mirror(0x07f8).w(start_lamp_w);
            map.op(0x6002, 0x6002).mirror(0x07f8).w(coin_lock_w);
            map.op(0x6003, 0x6003).mirror(0x07f8).w(coin_count_0_w);
            //map(0x6004, 0x6007).mirror(0x07f8).w("cust", FUNC(galaxian_sound_device::lfo_freq_w));
            map.op(0x6800, 0x6800).mirror(0x07ff).portr("IN1");
            //map(0x6800, 0x6807).mirror(0x07f8).w("cust", FUNC(galaxian_sound_device::sound_w));
            map.op(0x7000, 0x7000).mirror(0x07ff).portr("IN2");
            map.op(0x7001, 0x7001).mirror(0x07f8).w(irq_enable_w);
            map.op(0x7004, 0x7004).mirror(0x07f8).w(galaxian_stars_enable_w);
            map.op(0x7006, 0x7006).mirror(0x07f8).w(galaxian_flip_screen_x_w);
            map.op(0x7007, 0x7007).mirror(0x07f8).w(galaxian_flip_screen_y_w);
            //map(0x7800, 0x7800).mirror(0x07ff).w("cust", FUNC(galaxian_sound_device::pitch_w));
            map.op(0x7800, 0x7800).mirror(0x07ff).r("watchdog", (space, offset, mem_mask) => { return ((watchdog_timer_device)subdevice("watchdog")).reset_r(space); });  //FUNC(watchdog_timer_device::reset_r));
        }


        void galaxian_map(address_map map, device_t device)
        {
            galaxian_map_base(map, device);
            galaxian_map_discrete(map, device);
        }


        /* map derived from schematics */
        void frogger_map(address_map map, device_t device)
        {
            map.unmap_value_high();
            map.op(0x0000, 0x3fff).rom();
            map.op(0x8000, 0x87ff).ram();
            map.op(0x8800, 0x8800).mirror(0x07ff).r("watchdog", (space, offset, mem_mask) => { return ((watchdog_timer_device)subdevice("watchdog")).reset_r(space); });  //FUNC(watchdog_timer_device::reset_r));
            map.op(0xa800, 0xabff).mirror(0x0400).ram().w(galaxian_videoram_w).share("videoram");
            map.op(0xb000, 0xb0ff).mirror(0x0700).ram().w(galaxian_objram_w).share("spriteram");
            map.op(0xb808, 0xb808).mirror(0x07e3).w(irq_enable_w);
            map.op(0xb80c, 0xb80c).mirror(0x07e3).w(galaxian_flip_screen_y_w);
            map.op(0xb810, 0xb810).mirror(0x07e3).w(galaxian_flip_screen_x_w);
            map.op(0xb818, 0xb818).mirror(0x07e3).w(coin_count_0_w); /* IOPC7 */
            map.op(0xb81c, 0xb81c).mirror(0x07e3).w(coin_count_1_w); /* POUT1 */
            map.op(0xc000, 0xffff).rw(frogger_ppi8255_r, frogger_ppi8255_w);
        }


        /*************************************
         *
         *  Sound CPU memory maps
         *
         *************************************/

        // Konami Frogger with 1 x AY-8910A
        void frogger_sound_map(address_map map, device_t device)
        {
            map.global_mask(0x7fff);
            map.op(0x0000, 0x1fff).rom();
            map.op(0x4000, 0x43ff).mirror(0x1c00).ram();
            map.op(0x6000, 0x6fff).mirror(0x1000).w(konami_sound_filter_w);
        }


        void frogger_sound_portmap(address_map map, device_t device)
        {
            map.global_mask(0xff);
            map.op(0x00, 0xff).rw(frogger_ay8910_r, frogger_ay8910_w);
        }
    }


    partial class galaxian : global_object
    {
        /*************************************
         *
         *  Input Ports
         *  Galaxian-derived games
         *
         *************************************/

        //static INPUT_PORTS_START( galaxian )
        void construct_ioport_galaxian(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_COIN1 );
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_COIN2 );
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_2WAY();
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_2WAY();
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 );
            PORT_DIPNAME( 0x20, 0x00, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x20, DEF_STR( Cocktail ) );
            PORT_SERVICE( 0x40, IP_ACTIVE_HIGH );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_SERVICE1 );

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_START1 );
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_START2 );
            PORT_BIT( 0x04, IP_ACTIVE_HIGH, IPT_JOYSTICK_LEFT ); PORT_2WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_JOYSTICK_RIGHT ); PORT_2WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_UNUSED );
            PORT_DIPNAME( 0xc0, 0x00, DEF_STR( Coinage ) );
            PORT_DIPSETTING(    0x40, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x80, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0xc0, DEF_STR( Free_Play ) );

            PORT_START("IN2");
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Bonus_Life ) );
            PORT_DIPSETTING(    0x00, "7000" );
            PORT_DIPSETTING(    0x01, "10000" );
            PORT_DIPSETTING(    0x02, "12000" );
            PORT_DIPSETTING(    0x03, "20000" );
            PORT_DIPNAME( 0x04, 0x04, DEF_STR( Lives ) );
            PORT_DIPSETTING(    0x00, "2" );
            PORT_DIPSETTING(    0x04, "3" );
            PORT_DIPUNUSED( 0x08, 0x00 );
            PORT_BIT( 0xf0, IP_ACTIVE_HIGH, IPT_UNUSED );

            INPUT_PORTS_END();
        }


        /*************************************
         *
         *  Input Ports
         *  Konami games
         *
         *************************************/

        //static INPUT_PORTS_START( frogger )
        void construct_ioport_frogger(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_UNKNOWN ); /* 1P shoot2 - unused */
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_UNKNOWN ); /* 1P shoot1 - unused */
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_COIN1 );

            PORT_START("IN1");
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Lives ) );
            PORT_DIPSETTING(    0x00, "3" );
            PORT_DIPSETTING(    0x01, "5" );
            PORT_DIPSETTING(    0x02, "7" );
            PORT_DIPSETTING(    0x03, "256 (Cheat)");
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_UNKNOWN ); /* 2P shoot2 - unused */
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_UNKNOWN ); /* 2P shoot1 - unused */
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_START1 );

            PORT_START("IN2");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_DIPNAME( 0x06, 0x00, DEF_STR( Coinage ) );
            PORT_DIPSETTING(    0x02, "A 2/1 B 2/1 C 2/1" );
            PORT_DIPSETTING(    0x04, "A 2/1 B 1/3 C 2/1" );
            PORT_DIPSETTING(    0x00, "A 1/1 B 1/1 C 1/1" );
            PORT_DIPSETTING(    0x06, "A 1/1 B 1/6 C 1/1" );
            PORT_DIPNAME( 0x08, 0x00, DEF_STR( Cabinet ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x08, DEF_STR( Cocktail ) );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY();
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("IN3");   /* need for some PPI accesses */
            PORT_BIT( 0xff, 0x00, IPT_UNUSED );

            INPUT_PORTS_END();
        }
    }


    partial class galaxian_state : driver_device
    {
        /*************************************
         *
         *  Graphics layouts
         *
         *************************************/
        static readonly gfx_layout galaxian_charlayout = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,2),
            2,
            ArrayCombineUInt32( RGN_FRAC(0,2), RGN_FRAC(1,2) ),
            ArrayCombineUInt32( STEP8(0,1) ),
            ArrayCombineUInt32( STEP8(0,8) ),
            8*8
        );

        static readonly gfx_layout galaxian_spritelayout = new gfx_layout
        (
            16,16,
            RGN_FRAC(1,2),
            2,
            ArrayCombineUInt32( RGN_FRAC(0,2), RGN_FRAC(1,2) ),
            ArrayCombineUInt32( STEP8(0,1), STEP8(8*8,1) ),
            ArrayCombineUInt32( STEP8(0,8), STEP8(16*8,8) ),
            16*16
        );

#if false
        static const gfx_layout galaxian_charlayout_0x200 =
        {
            8,8,
            0x200,
            2,
            { RGN_FRAC(0,2), RGN_FRAC(1,2) },
            { STEP8(0,1) },
            { STEP8(0,8) },
            8*8
        };

        static const gfx_layout galaxian_spritelayout_0x80 =
        {
            16,16,
            0x80,
            2,
            { RGN_FRAC(0,2), RGN_FRAC(1,2) },
            { STEP8(0,1), STEP8(8*8,1) },
            { STEP8(0,8), STEP8(16*8,8) },
            16*16
        };
#endif

        /*************************************
         *
         *  Graphics decoding
         *
         *************************************/

        //static GFXDECODE_START(gfx_galaxian)
        static readonly gfx_decode_entry [] gfx_galaxian = new gfx_decode_entry[]
        {
            GFXDECODE_SCALE("gfx1", 0x0000, galaxian_charlayout,   0, 8, galaxian_state.GALAXIAN_XSCALE,1),
            GFXDECODE_SCALE("gfx1", 0x0000, galaxian_spritelayout, 0, 8, galaxian_state.GALAXIAN_XSCALE,1),
            //GFXDECODE_END
        };


        /*************************************
         *
         *  Core machine driver pieces
         *
         *************************************/

        void galaxian_base(machine_config config)
        {
            // basic machine hardware
            Z80(config, m_maincpu, GALAXIAN_PIXEL_CLOCK/3/2);
            m_maincpu.target.memory().set_addrmap(AS_PROGRAM, galaxian_map);

            WATCHDOG_TIMER(config, "watchdog").set_vblank_count("screen", 8);

            // video hardware
            GFXDECODE(config, m_gfxdecode, m_palette, gfx_galaxian);
            PALETTE(config, m_palette, galaxian_palette, 32);

            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.target.set_raw(GALAXIAN_PIXEL_CLOCK, GALAXIAN_HTOTAL, GALAXIAN_HBEND, GALAXIAN_HBSTART, GALAXIAN_VTOTAL, GALAXIAN_VBEND, GALAXIAN_VBSTART);
            m_screen.target.set_screen_update(screen_update_galaxian);
            m_screen.target.screen_vblank().set(vblank_interrupt_w).reg();

            // sound hardware
            SPEAKER(config, "speaker").front_center();
        }


        //MACHINE_CONFIG_START(galaxian_state::konami_base)
        void konami_base(machine_config config)
        {
            galaxian_base(config);

            I8255A(config, m_ppi8255.op(0));
            m_ppi8255.op(0).target.in_pa_callback().set_ioport("IN0").reg();
            m_ppi8255.op(0).target.in_pb_callback().set_ioport("IN1").reg();
            m_ppi8255.op(0).target.in_pc_callback().set_ioport("IN2").reg();
            m_ppi8255.op(0).target.out_pc_callback().set(konami_portc_0_w).reg();

            I8255A(config, m_ppi8255.op(1));
            m_ppi8255.op(1).target.out_pa_callback().set(m_soundlatch, (space, offset, data, mem_mask) => { ((generic_latch_8_device)subdevice("soundlatch")).write(data); }).reg();  //FUNC(generic_latch_8_device::write));
            m_ppi8255.op(1).target.out_pb_callback().set(konami_sound_control_w).reg();
            m_ppi8255.op(1).target.in_pc_callback().set_ioport("IN3").reg();
            m_ppi8255.op(1).target.out_pc_callback().set(konami_portc_1_w).reg();
        }


        void konami_sound_1x_ay8910(machine_config config)
        {
            /* 2nd CPU to drive sound */
            Z80(config, m_audiocpu, KONAMI_SOUND_CLOCK/8);
            m_audiocpu.target.memory().set_addrmap(AS_PROGRAM, frogger_sound_map);
            m_audiocpu.target.memory().set_addrmap(AS_IO, frogger_sound_portmap);

            GENERIC_LATCH_8(config, m_soundlatch);

            /* sound hardware */
            AY8910(config, m_ay8910.op(0), KONAMI_SOUND_CLOCK/8);
            m_ay8910.op(0).target.set_flags(ay8910_global.AY8910_RESISTOR_OUTPUT);
            m_ay8910.op(0).target.set_resistors_load((int)1000.0, (int)1000.0, (int)1000.0);
            m_ay8910.op(0).target.port_a_read_callback().set(m_soundlatch, (space, offset, mem_mask) => { return ((generic_latch_8_device)subdevice("soundlatch")).read(); }).reg();  //FUNC(generic_latch_8_device::read));
            m_ay8910.op(0).target.port_b_read_callback().set(frogger_sound_timer_r).reg();
            m_ay8910.op(0).target.disound.add_route(0, "konami", 1.0, 0);
            m_ay8910.op(0).target.disound.add_route(1, "konami", 1.0, 1);
            m_ay8910.op(0).target.disound.add_route(2, "konami", 1.0, 2);

            NETLIST_SOUND(config, "konami", 48000)
                .set_source(netlist_konami1x)
                .disound.add_route(ALL_OUTPUTS, "speaker", 1.0);

            // Filter
            NETLIST_LOGIC_INPUT(config, "konami:ctl0", "CTL0.IN", 0);
            NETLIST_LOGIC_INPUT(config, "konami:ctl1", "CTL1.IN", 0);
            NETLIST_LOGIC_INPUT(config, "konami:ctl2", "CTL2.IN", 0);
            NETLIST_LOGIC_INPUT(config, "konami:ctl3", "CTL3.IN", 0);
            NETLIST_LOGIC_INPUT(config, "konami:ctl4", "CTL4.IN", 0);
            NETLIST_LOGIC_INPUT(config, "konami:ctl5", "CTL5.IN", 0);

            // CHA1 - 3D
            NETLIST_STREAM_INPUT(config, "konami:cin0", 0, "R_AY3D_A.R");
            NETLIST_STREAM_INPUT(config, "konami:cin1", 1, "R_AY3D_B.R");
            NETLIST_STREAM_INPUT(config, "konami:cin2", 2, "R_AY3D_C.R");

            NETLIST_STREAM_OUTPUT(config, "konami:cout0", 0, "OUT").set_mult_offset(30000.0 / 0.05, 0.0);
        }


        /*************************************
         *
         *  Machine drivers
         *
         *************************************/

        public void galaxian(machine_config config)
        {
            galaxian_base(config);

            GALAXIAN_SOUND(config, "cust", 0);
        }


        public void frogger(machine_config config)
        {
            konami_base(config);
            konami_sound_1x_ay8910(config);

            // alternate memory map
            m_maincpu.target.memory().set_addrmap(AS_PROGRAM, frogger_map);
        }


        /*************************************
         *
         *  Driver configuration
         *
         *************************************/

        void common_init(galaxian_draw_bullet_func draw_bullet, galaxian_draw_background_func draw_background,
            galaxian_extend_tile_info_func extend_tile_info, galaxian_extend_sprite_info_func extend_sprite_info)
        {
            m_x_scale = GALAXIAN_XSCALE;
            m_h0_start = GALAXIAN_H0START;
            m_irq_enabled = 0;
            m_irq_line = device_execute_interface.INPUT_LINE_NMI;
            m_numspritegens = 1;
            m_bullets_base = 0x60;
            m_sprites_base = 0x40;
            m_frogger_adjust = 0;  //false;
            m_sfx_tilemap = 0;  //false;
            m_draw_bullet_ptr = (draw_bullet != null) ? draw_bullet : galaxian_draw_bullet;
            m_draw_background_ptr = (draw_background != null) ? draw_background : galaxian_draw_background;
            m_extend_tile_info_ptr = extend_tile_info;
            m_extend_sprite_info_ptr = extend_sprite_info;
        }



        /*************************************
         *
         *  Galaxian-derived games
         *
         *************************************/

        public void init_galaxian()
        {
            common_init(galaxian_draw_bullet, galaxian_draw_background, null, null);
        }


        /*************************************
         *
         *  Konami games
         *
         *************************************/

        public void init_frogger()
        {
            /* video extensions */
            common_init(null, frogger_draw_background, frogger_extend_tile_info, frogger_extend_sprite_info);
            m_frogger_adjust = 1; //true;

            /* decrypt */
            decode_frogger_sound();
            decode_frogger_gfx();
        }
    }


    partial class galaxian : global_object
    {
        /*************************************
         *
         *  ROM definitions
         *  Galaxian-derived games
         *
         *************************************/

        //ROM_START( galaxian )
        static readonly List<tiny_rom_entry> rom_galaxian = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x4000, "maincpu", 0 ),
            ROM_LOAD( "galmidw.u",    0x0000, 0x0800, CRC("745e2d61") + SHA1("e65f74e35b1bfaccd407e168ea55678ae9b68edf") ),
            ROM_LOAD( "galmidw.v",    0x0800, 0x0800, CRC("9c999a40") + SHA1("02fdcd95d8511e64c0d2b007b874112d53e41045") ),
            ROM_LOAD( "galmidw.w",    0x1000, 0x0800, CRC("b5894925") + SHA1("0046b9ed697a34d088de1aead8bd7cbe526a2396") ),
            ROM_LOAD( "galmidw.y",    0x1800, 0x0800, CRC("6b3ca10b") + SHA1("18d8714e5ef52f63ba8888ecc5a25b17b3bf17d1") ),
            ROM_LOAD( "7l",           0x2000, 0x0800, CRC("1b933207") + SHA1("8b44b0f74420871454e27894d0f004859f9e59a9") ),

            ROM_REGION( 0x1000, "gfx1", 0 ),
            ROM_LOAD( "1h.bin",       0x0000, 0x0800, CRC("39fb43a4") + SHA1("4755609bd974976f04855d51e08ec0d62ab4bc07") ),
            ROM_LOAD( "1k.bin",       0x0800, 0x0800, CRC("7e3f56a2") + SHA1("a9795d8b7388f404f3b0e2c6ce15d713a4c5bafa") ),

            ROM_REGION( 0x0020, "proms", 0 ),
            ROM_LOAD( "6l.bpr",       0x0000, 0x0020, CRC("c3ac9467") + SHA1("f382ad5a34d282056c78a5ec00c30ec43772bae2") ),

            ROM_END,
        };


        /*************************************
         *
         *  ROM definitions
         *  Konami games
         *
         *************************************/

        //ROM_START( frogger )
        static readonly List<tiny_rom_entry> rom_frogger = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "frogger.26",   0x0000, 0x1000, CRC("597696d6") + SHA1("e7e021776cad00f095a1ebbef407b7c0a8f5d835") ),
            ROM_LOAD( "frogger.27",   0x1000, 0x1000, CRC("b6e6fcc3") + SHA1("5e8692f2b0c7f4b3642b3ee6670e1c3b20029cdc") ),
            ROM_LOAD( "frsm3.7",      0x2000, 0x1000, CRC("aca22ae0") + SHA1("5a99060ea2506a3ac7d61ca5876ce5cb3e493565") ),

            ROM_REGION( 0x10000, "audiocpu", 0 ),
            ROM_LOAD( "frogger.608",  0x0000, 0x0800, CRC("e8ab0256") + SHA1("f090afcfacf5f13cdfa0dfda8e3feb868c6ce8bc") ),
            ROM_LOAD( "frogger.609",  0x0800, 0x0800, CRC("7380a48f") + SHA1("75582a94b696062cbdb66a4c5cf0bc0bb94f81ee") ),
            ROM_LOAD( "frogger.610",  0x1000, 0x0800, CRC("31d7eb27") + SHA1("2e1d34ae4da385fd7cac94707d25eeddf4604e1a") ),

            ROM_REGION( 0x1000, "gfx1", 0 ),
            ROM_LOAD( "frogger.607",  0x0000, 0x0800, CRC("05f7d883") + SHA1("78831fd287da18928651a8adb7e578d291493eff") ),
            ROM_LOAD( "frogger.606",  0x0800, 0x0800, CRC("f524ee30") + SHA1("dd768967add61467baa08d5929001f157d6cd911") ),

            ROM_REGION( 0x0020, "proms", 0 ),
            ROM_LOAD( "pr-91.6l",     0x0000, 0x0020, CRC("413703bf") + SHA1("66648b2b28d3dcbda5bdb2605d1977428939dd3c") ),

            ROM_END,
        };


        static void galaxian_state_galaxian(machine_config config, device_t device) { galaxian_state galaxian_state = (galaxian_state)device; galaxian_state.galaxian(config); }
        static void galaxian_state_frogger(machine_config config, device_t device) { galaxian_state galaxian_state = (galaxian_state)device; galaxian_state.frogger(config); }
        static void galaxian_state_init_galaxian(device_t owner) { galaxian_state galaxian_state = (galaxian_state)owner; galaxian_state.init_galaxian(); }
        static void galaxian_state_init_frogger(device_t owner) { galaxian_state galaxian_state = (galaxian_state)owner; galaxian_state.init_frogger(); }


        static galaxian m_galaxian = new galaxian();


        static device_t device_creator_galaxian(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new galaxian_state(mconfig, type, tag); }
        static device_t device_creator_frogger(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new galaxian_state(mconfig, type, tag); }


        /*************************************
         *
         *  Game drivers
         *  Galaxian-derived games
         *
         *************************************/

        /* basic galaxian hardware */
        //                                                         creator,                 rom           YEAR,   NAME,       PARENT,  MACHINE,                           INPUT,                                 INIT,                                   MONITOR,COMPANY, FULLNAME,FLAGS
        public static readonly game_driver driver_galaxian = GAME( device_creator_galaxian, rom_galaxian, "1979", "galaxian", null,    galaxian.galaxian_state_galaxian,  m_galaxian.construct_ioport_galaxian,  galaxian.galaxian_state_init_galaxian,  ROT90,  "Namco", "Galaxian (Namco set 1)", MACHINE_SUPPORTS_SAVE );


        /*************************************
         *
         *  Game drivers
         *  Konami games
         *
         *************************************/

        /* Frogger based hardware: 2nd Z80, AY-8910A, 2 8255 PPI for I/O, custom background */
        //                                                        creator,                rom          YEAR,   NAME,      PARENT,  MACHINE,                         INPUT,                               INIT,                                 MONITOR,COMPANY, FULLNAME,FLAGS
        public static readonly game_driver driver_frogger = GAME( device_creator_frogger, rom_frogger, "1981", "frogger", null,    galaxian.galaxian_state_frogger, m_galaxian.construct_ioport_frogger, galaxian.galaxian_state_init_frogger, ROT90,  "Konami", "Frogger", MACHINE_SUPPORTS_SAVE );
    }
}
