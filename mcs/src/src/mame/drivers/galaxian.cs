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
        public void vblank_interrupt_w(int state)
        {
            /* interrupt line is clocked at VBLANK */
            /* a flip-flop at 6F is held in the preset state based on the NMI ON signal */
            if (state != 0 && m_irq_enabled != 0)
                m_maincpu.target.execute().set_input_line(m_irq_line, line_state.ASSERT_LINE);
        }


        //WRITE_LINE_MEMBER(galaxian_state::tenspot_interrupt_w)


        //WRITE8_MEMBER(galaxian_state::irq_enable_w)
        public void irq_enable_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* the latched D0 bit here goes to the CLEAR line on the interrupt flip-flop */
            m_irq_enabled = (byte)(data & 1);

            /* if CLEAR is held low, we must make sure the interrupt signal is clear */
            if (m_irq_enabled == 0)
                m_maincpu.target.execute().set_input_line(m_irq_line, line_state.CLEAR_LINE);
        }


        /*************************************
         *
         *  DRIVER latch control
         *
         *************************************/

        //WRITE8_MEMBER(galaxian_state::start_lamp_w)
        public void start_lamp_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* offset 0 = 1P START LAMP */
            /* offset 1 = 2P START LAMP */
            m_lamps[offset] = coretmpl_global.BIT(data, 0);
        }


        //WRITE8_MEMBER(galaxian_state::coin_lock_w)
        public void coin_lock_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* many variants and bootlegs don't have this */
            machine().bookkeeping().coin_lockout_global_w(~data & 1);
        }


        //WRITE8_MEMBER(galaxian_state::coin_count_0_w)
        public void coin_count_0_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            machine().bookkeeping().coin_counter_w(0, data & 1);
        }


        //WRITE8_MEMBER(galaxian_state::coin_count_1_w)
        public void coin_count_1_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
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
        {
            /* the decoding here is very simplistic, and you can address both simultaneously */
            uint8_t result = 0xff;
            if (offset & 0x20) result &= m_ay8910[1]->data_r(space, 0);
            if (offset & 0x80) result &= m_ay8910[0]->data_r(space, 0);
            return result;
        }


        WRITE8_MEMBER(galaxian_state::konami_ay8910_w)
        {
            /* AV 4,5 ==> AY8910 #2 */
            /* the decoding here is very simplistic, and you can address two simultaneously */
            if (offset & 0x10)
                m_ay8910[1]->address_w(space, 0, data);
            else if (offset & 0x20)
                m_ay8910[1]->data_w(space, 0, data);
            /* AV6,7 ==> AY8910 #1 */
            if (offset & 0x40)
                m_ay8910[0]->address_w(space, 0, data);
            else if (offset & 0x80)
                m_ay8910[0]->data_w(space, 0, data);
        }
#endif


        //WRITE8_MEMBER(galaxian_state::konami_sound_control_w)
        public void konami_sound_control_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            byte old = m_konami_sound_control;
            m_konami_sound_control = data;

            /* the inverse of bit 3 clocks the flip flop to signal an INT */
            /* it is automatically cleared on the acknowledge */
            if ((old & 0x08) != 0 && (data & 0x08) == 0)
                m_audiocpu.target.execute().set_input_line(0, line_state.HOLD_LINE);

            /* bit 4 is sound disable */
            machine().sound().system_mute((data & 0x10) != 0);
        }


        //READ8_MEMBER(galaxian_state::konami_sound_timer_r)
        public u8 konami_sound_timer_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
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

            UInt32 cycles = (UInt32)((m_audiocpu.target.execute().total_cycles() * 8) % (UInt64)(16*16*2*8*5*2));
            byte hibit = 0;

            /* separate the high bit from the others */
            if (cycles >= 16*16*2*8*5)
            {
                hibit = 1;
                cycles -= 16*16*2*8*5;
            }

            /* the top bits of the counter index map to various bits here */
            return (byte)((hibit << 7) |           /* B7 is the output of the final divide-by-2 counter */
                    (global.BIT(cycles,14) << 6) | /* B6 is the high bit of the divide-by-5 counter */
                    (global.BIT(cycles,13) << 5) | /* B5 is the 2nd highest bit of the divide-by-5 counter */
                    (global.BIT(cycles,11) << 4) | /* B4 is the high bit of the divide-by-8 counter */
                    0x0e);                   /* assume remaining bits are high, except B0 which is grounded */
        }


        static readonly string [] konami_sound_filter_w_ayname = new string[2] { "8910.0", "8910.1" };

        //WRITE8_MEMBER(galaxian_state::konami_sound_filter_w)
        public void konami_sound_filter_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            if (m_discrete != null)
            {
                /* the offset is used as data, 6 channels * 2 bits each */
                /* AV0 .. AV5 ==> AY8910 #2 */
                /* AV6 .. AV11 ==> AY8910 #1 */
                for (int which = 0; which < 2; which++)
                {
                    if (m_ay8910.op(which) != null)
                    {
                        for (int chan = 0; chan < 3; chan++)
                        {
                            uint8_t bits = (uint8_t)((offset >> (2 * chan + 6 * (1 - which))) & 3);

                            /* low bit goes to 0.22uF capacitor = 220000pF  */
                            /* high bit goes to 0.047uF capacitor = 47000pF */
                            m_discrete.target.write(space, (offs_t)discrete_global.NODE_GET(3 * which + chan + 11), bits);
                        }
                    }
                }
            }
        }


        //WRITE8_MEMBER(galaxian_state::konami_portc_0_w)
        public void konami_portc_0_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            logerror("{0}:ppi0_portc_w = {1}\n", machine().describe_context(), data);  // %02X
        }


        //WRITE8_MEMBER(galaxian_state::konami_portc_1_w)
        public void konami_portc_1_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            logerror("{0}:ppi1_portc_w = {1}\n", machine().describe_context(), data);  // %02X
        }


        /*************************************
         *
         *  Frogger I/O
         *
         *************************************/

        //READ8_MEMBER(galaxian_state::frogger_ppi8255_r)
        public u8 frogger_ppi8255_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            /* the decoding here is very simplistic, and you can address both simultaneously */
            byte result = 0xff;
            if ((offset & 0x1000) != 0) result &= m_ppi8255.op(1).target.read((offset >> 1) & 3);
            if ((offset & 0x2000) != 0) result &= m_ppi8255.op(0).target.read((offset >> 1) & 3);
            return result;
        }


        //WRITE8_MEMBER(galaxian_state::frogger_ppi8255_w)
        public void frogger_ppi8255_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* the decoding here is very simplistic, and you can address both simultaneously */
            if ((offset & 0x1000) != 0) m_ppi8255.op(1).target.write((offset >> 1) & 3, data);
            if ((offset & 0x2000) != 0) m_ppi8255.op(0).target.write((offset >> 1) & 3, data);
        }


        //READ8_MEMBER(galaxian_state::frogger_ay8910_r)
        public u8 frogger_ay8910_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            /* the decoding here is very simplistic */
            byte result = 0xff;
            if ((offset & 0x40) != 0) result &= m_ay8910.op(0).target.data_r(space, 0);
            return result;
        }


        //WRITE8_MEMBER(galaxian_state::frogger_ay8910_w)
        public void frogger_ay8910_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* the decoding here is very simplistic */
            /* AV6,7 ==> AY8910 #1 */
            if ((offset & 0x40) != 0)
                m_ay8910.op(0).target.data_w(space, 0, data);
            else if ((offset & 0x80) != 0)
                m_ay8910.op(0).target.address_w(space, 0, data);
        }


        //READ8_MEMBER(galaxian_state::frogger_sound_timer_r)
        public u8 frogger_sound_timer_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
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
         *  Driver configuration
         *
         *************************************/

        void common_init(galaxian_draw_bullet_func draw_bullet, galaxian_draw_background_func draw_background,
            galaxian_extend_tile_info_func extend_tile_info, galaxian_extend_sprite_info_func extend_sprite_info)
        {
            m_x_scale = GALAXIAN_XSCALE;
            m_h0_start = GALAXIAN_H0START;
            m_irq_enabled = 0;
            m_irq_line = (int)INPUT_LINE.INPUT_LINE_NMI;
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

        public static void galaxian_state_init_galaxian(running_machine machine, device_t owner)
        {
            galaxian_state galaxian_state = (galaxian_state)owner;

            galaxian_state.common_init(galaxian_state.galaxian_draw_bullet, galaxian_state.galaxian_draw_background, null, null);
        }


        /*************************************
         *
         *  Konami games
         *
         *************************************/

        public static void galaxian_state_init_frogger(running_machine machine, device_t owner)
        {
            galaxian_state galaxian_state = (galaxian_state)owner;

            /* video extensions */
            galaxian_state.common_init(null, galaxian_state.frogger_draw_background, galaxian_state.frogger_extend_tile_info, galaxian_state.frogger_extend_sprite_info);
            galaxian_state.m_frogger_adjust = 1; //true;

            /* decrypt */
            galaxian_state.decode_frogger_sound();
            galaxian_state.decode_frogger_gfx();
        }
    }


    public partial class galaxian : device_init_helpers
    {
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

        //void galaxian_state::galaxian_map_discrete(address_map &map)
        void galaxian_state_galaxian_map_discrete(address_map map, device_t device)
        {
            galaxian_state galaxian_state = (galaxian_state)device;

            map.op(0x6004, 0x6007).mirror(0x07f8).w("cust", galaxian_state.galaxian_sound_device_lfo_freq_w);
            map.op(0x6800, 0x6807).mirror(0x07f8).w("cust", galaxian_state.galaxian_sound_device_sound_w);
            map.op(0x7800, 0x7800).mirror(0x07ff).w("cust", galaxian_state.galaxian_sound_device_pitch_w);
        }


        //void galaxian_state::galaxian_map_base(address_map &map)
        void galaxian_state_galaxian_map_base(address_map map, device_t device)
        {
            galaxian_state galaxian_state = (galaxian_state)device;

            map.unmap_value_high();
            map.op(0x0000, 0x3fff).rom();
            map.op(0x4000, 0x43ff).mirror(0x0400).ram();
            map.op(0x5000, 0x53ff).mirror(0x0400).ram().w(galaxian_state.galaxian_videoram_w).share("videoram");
            map.op(0x5800, 0x58ff).mirror(0x0700).ram().w(galaxian_state.galaxian_objram_w).share("spriteram");
            map.op(0x6000, 0x6000).mirror(0x07ff).portr("IN0");
            map.op(0x6000, 0x6001).mirror(0x07f8).w(galaxian_state.start_lamp_w);
            map.op(0x6002, 0x6002).mirror(0x07f8).w(galaxian_state.coin_lock_w);
            map.op(0x6003, 0x6003).mirror(0x07f8).w(galaxian_state.coin_count_0_w);
            //AM_RANGE(0x6004, 0x6007) AM_MIRROR(0x07f8) AM_DEVWRITE("cust", galaxian_sound_device, lfo_freq_w)
            map.op(0x6800, 0x6800).mirror(0x07ff).portr("IN1");
            //AM_RANGE(0x6800, 0x6807) AM_MIRROR(0x07f8) AM_DEVWRITE("cust", galaxian_sound_device, sound_w)
            map.op(0x7000, 0x7000).mirror(0x07ff).portr("IN2");
            map.op(0x7001, 0x7001).mirror(0x07f8).w(galaxian_state.irq_enable_w);
            map.op(0x7004, 0x7004).mirror(0x07f8).w(galaxian_state.galaxian_stars_enable_w);
            map.op(0x7006, 0x7006).mirror(0x07f8).w(galaxian_state.galaxian_flip_screen_x_w);
            map.op(0x7007, 0x7007).mirror(0x07f8).w(galaxian_state.galaxian_flip_screen_y_w);
            //AM_RANGE(0x7800, 0x7800) AM_MIRROR(0x07ff) AM_DEVWRITE("cust", galaxian_sound_device, pitch_w)
            map.op(0x7800, 0x7800).mirror(0x07ff).r("watchdog", galaxian_state.watchdog_timer_device_reset_r);
        }


        //void galaxian_state::galaxian_map(address_map &map)
        void galaxian_state_galaxian_map(address_map map, device_t device)
        {
            galaxian_state galaxian_state = (galaxian_state)device;

            galaxian_state_galaxian_map_base(map, device);
            galaxian_state_galaxian_map_discrete(map, device);
        }


        /* map derived from schematics */
        //void galaxian_state::frogger_map(address_map &map)
        void galaxian_state_frogger_map(address_map map, device_t device)
        {
            galaxian_state galaxian_state = (galaxian_state)device;

            map.unmap_value_high();
            map.op(0x0000, 0x3fff).rom();
            map.op(0x8000, 0x87ff).ram();
            map.op(0x8800, 0x8800).mirror(0x07ff).r("watchdog", galaxian_state.watchdog_timer_device_reset_r);
            map.op(0xa800, 0xabff).mirror(0x0400).ram().w(galaxian_state.galaxian_videoram_w).share("videoram");
            map.op(0xb000, 0xb0ff).mirror(0x0700).ram().w(galaxian_state.galaxian_objram_w).share("spriteram");
            map.op(0xb808, 0xb808).mirror(0x07e3).w(galaxian_state.irq_enable_w);
            map.op(0xb80c, 0xb80c).mirror(0x07e3).w(galaxian_state.galaxian_flip_screen_y_w);
            map.op(0xb810, 0xb810).mirror(0x07e3).w(galaxian_state.galaxian_flip_screen_x_w);
            map.op(0xb818, 0xb818).mirror(0x07e3).w(galaxian_state.coin_count_0_w); /* IOPC7 */
            map.op(0xb81c, 0xb81c).mirror(0x07e3).w(galaxian_state.coin_count_1_w); /* POUT1 */
            map.op(0xc000, 0xffff).rw(galaxian_state.frogger_ppi8255_r, galaxian_state.frogger_ppi8255_w);
        }


        /*************************************
         *
         *  Sound CPU memory maps
         *
         *************************************/

        // Konami Frogger with 1 x AY-8910A
        //void galaxian_state::frogger_sound_map(address_map &map)
        void galaxian_state_frogger_sound_map(address_map map, device_t device)
        {
            galaxian_state galaxian_state = (galaxian_state)device;

            map.global_mask(0x7fff);
            map.op(0x0000, 0x1fff).rom();
            map.op(0x4000, 0x43ff).mirror(0x1c00).ram();
            map.op(0x6000, 0x6fff).mirror(0x1000).w(galaxian_state.konami_sound_filter_w);
        }


        //void galaxian_state::frogger_sound_portmap(address_map &map)
        void galaxian_state_frogger_sound_portmap(address_map map, device_t device)
        {
            galaxian_state galaxian_state = (galaxian_state)device;

            map.global_mask(0xff);
            map.op(0x00, 0xff).rw(galaxian_state.frogger_ay8910_r, galaxian_state.frogger_ay8910_w);
        }


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
            digfx_global.ArrayCombineUInt32( RGN_FRAC(0,2), RGN_FRAC(1,2) ),
            digfx_global.ArrayCombineUInt32( STEP8(0,1) ),
            digfx_global.ArrayCombineUInt32( STEP8(0,8) ),
            8*8
        );

        static readonly gfx_layout galaxian_spritelayout = new gfx_layout
        (
            16,16,
            RGN_FRAC(1,2),
            2,
            digfx_global.ArrayCombineUInt32( RGN_FRAC(0,2), RGN_FRAC(1,2) ),
            digfx_global.ArrayCombineUInt32( STEP8(0,1), STEP8(8*8,1) ),
            digfx_global.ArrayCombineUInt32( STEP8(0,8), STEP8(16*8,8) ),
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
         *  Sound configuration
         *
         *************************************/

        static readonly discrete_mixer_desc konami_sound_mixer_desc = new discrete_mixer_desc
        (
            DISC_MIXER_IS_OP_AMP,
            new double [] {RES_K(5.1), RES_K(5.1), RES_K(5.1), RES_K(5.1), RES_K(5.1), RES_K(5.1)},
            new int [] {0,0,0,0,0,0},  /* no variable resistors   */
            new double [] {0,0,0,0,0,0},  /* no node capacitors      */
            0, RES_K(2.2),
            0,
            0, /* modelled separately */
            0, 1
        );

        //static DISCRETE_SOUND_START( konami_sound_discrete )
        public static readonly discrete_block [] konami_sound_discrete = new discrete_block []
        {
            DISCRETE_INPUTX_STREAM(NODE.NODE_01, 0, 1.0, 0),
            DISCRETE_INPUTX_STREAM(NODE.NODE_02, 1, 1.0, 0),
            DISCRETE_INPUTX_STREAM(NODE.NODE_03, 2, 1.0, 0),

            DISCRETE_INPUTX_STREAM(NODE.NODE_04, 3, 1.0, 0),
            DISCRETE_INPUTX_STREAM(NODE.NODE_05, 4, 1.0, 0),
            DISCRETE_INPUTX_STREAM(NODE.NODE_06, 5, 1.0, 0),

            DISCRETE_INPUT_DATA(NODE.NODE_11),
            DISCRETE_INPUT_DATA(NODE.NODE_12),
            DISCRETE_INPUT_DATA(NODE.NODE_13),

            DISCRETE_INPUT_DATA(NODE.NODE_14),
            DISCRETE_INPUT_DATA(NODE.NODE_15),
            DISCRETE_INPUT_DATA(NODE.NODE_16),

            DISCRETE_RCFILTER_SW(NODE.NODE_21, 1, NODE.NODE_01, NODE.NODE_11, AY8910_INTERNAL_RESISTANCE+1000, CAP_U(0.22), CAP_U(0.047), 0, 0),
            DISCRETE_RCFILTER_SW(NODE.NODE_22, 1, NODE.NODE_02, NODE.NODE_12, AY8910_INTERNAL_RESISTANCE+1000, CAP_U(0.22), CAP_U(0.047), 0, 0),
            DISCRETE_RCFILTER_SW(NODE.NODE_23, 1, NODE.NODE_03, NODE.NODE_13, AY8910_INTERNAL_RESISTANCE+1000, CAP_U(0.22), CAP_U(0.047), 0, 0),

            DISCRETE_RCFILTER_SW(NODE.NODE_24, 1, NODE.NODE_04, NODE.NODE_14, AY8910_INTERNAL_RESISTANCE+1000, CAP_U(0.22), CAP_U(0.047), 0, 0),
            DISCRETE_RCFILTER_SW(NODE.NODE_25, 1, NODE.NODE_05, NODE.NODE_15, AY8910_INTERNAL_RESISTANCE+1000, CAP_U(0.22), CAP_U(0.047), 0, 0),
            DISCRETE_RCFILTER_SW(NODE.NODE_26, 1, NODE.NODE_06, NODE.NODE_16, AY8910_INTERNAL_RESISTANCE+1000, CAP_U(0.22), CAP_U(0.047), 0, 0),

            DISCRETE_MIXER6(NODE.NODE_30, 1, NODE.NODE_21, NODE.NODE_22, NODE.NODE_23, NODE.NODE_24, NODE.NODE_25, NODE.NODE_26, konami_sound_mixer_desc),

            /* FIXME the amplifier M51516L has a decay circuit */
            /* This is handled with sound_global_enable but    */
            /* belongs here.                                   */

            /* Input impedance of a M51516L is typically 30k (datasheet) */
            DISCRETE_CRFILTER(NODE.NODE_40,NODE.NODE_30,RES_K(30),CAP_U(0.15)),

            DISCRETE_OUTPUT(NODE.NODE_40, 10.0 ),

            DISCRETE_SOUND_END(),
        };


        /*************************************
         *
         *  Core machine driver pieces
         *
         *************************************/

        //MACHINE_CONFIG_START(galaxian_state::galaxian_base)
        void galaxian_state_galaxian_base(machine_config config, device_t owner, device_t device)
        {
            MACHINE_CONFIG_START(config, owner, device);

            galaxian_state galaxian_state = (galaxian_state)helper_owner;

            /* basic machine hardware */
            MCFG_DEVICE_ADD("maincpu", z80_device.Z80, galaxian_state.GALAXIAN_PIXEL_CLOCK/3/2);
            MCFG_DEVICE_PROGRAM_MAP(galaxian_state_galaxian_map);

            WATCHDOG_TIMER(config, "watchdog").set_vblank_count("screen", 8);

            /* video hardware */
            MCFG_DEVICE_ADD("gfxdecode", gfxdecode_device.GFXDECODE);//, "palette", gfx_galaxian);
            MCFG_DEVICE_ADD_gfxdecode_device("palette", gfx_galaxian);
            MCFG_PALETTE_ADD("palette", 32);
            MCFG_PALETTE_INIT_OWNER(galaxian_state.palette_init_galaxian);

            MCFG_SCREEN_ADD("screen", SCREEN_TYPE_RASTER);
            MCFG_SCREEN_RAW_PARAMS(galaxian_state.GALAXIAN_PIXEL_CLOCK, galaxian_state.GALAXIAN_HTOTAL, galaxian_state.GALAXIAN_HBEND, galaxian_state.GALAXIAN_HBSTART, galaxian_state.GALAXIAN_VTOTAL, galaxian_state.GALAXIAN_VBEND, galaxian_state.GALAXIAN_VBSTART);
            MCFG_SCREEN_UPDATE_DRIVER(galaxian_state.screen_update_galaxian);
            MCFG_SCREEN_VBLANK_CALLBACK(WRITELINE(galaxian_state.vblank_interrupt_w));

            /* sound hardware */
            SPEAKER(config, "speaker").front_center();

            MACHINE_CONFIG_END();
        }


        //MACHINE_CONFIG_START(galaxian_state::konami_base)
        void galaxian_state_konami_base(machine_config config, device_t owner, device_t device)
        {
            galaxian_state_galaxian_base(config, owner, device);

            MACHINE_CONFIG_START(config, owner, device);

            galaxian_state galaxian_state = (galaxian_state)helper_owner;

            I8255A(config, galaxian_state.ppi8255.op(0));
            galaxian_state.ppi8255.op(0).target.in_pa_callback().set_ioport("IN0").reg();
            galaxian_state.ppi8255.op(0).target.in_pb_callback().set_ioport("IN1").reg();
            galaxian_state.ppi8255.op(0).target.in_pc_callback().set_ioport("IN2").reg();
            galaxian_state.ppi8255.op(0).target.out_pc_callback().set(galaxian_state.konami_portc_0_w).reg();

            I8255A(config, galaxian_state.ppi8255.op(1));
            galaxian_state.ppi8255.op(1).target.out_pa_callback().set("soundlatch", galaxian_state.generic_latch_8_device_write).reg();
            galaxian_state.ppi8255.op(1).target.out_pb_callback().set(galaxian_state.konami_sound_control_w).reg();
            galaxian_state.ppi8255.op(1).target.in_pc_callback().set_ioport("IN3").reg();
            galaxian_state.ppi8255.op(1).target.out_pc_callback().set(galaxian_state.konami_portc_1_w).reg();

            MACHINE_CONFIG_END();
        }


        //MACHINE_CONFIG_START(galaxian_state::konami_sound_1x_ay8910)
        void galaxian_state_konami_sound_1x_ay8910(machine_config config, device_t owner, device_t device)
        {
            MACHINE_CONFIG_START(config, owner, device);

            galaxian_state galaxian_state = (galaxian_state)helper_owner;

            /* 2nd CPU to drive sound */
            MCFG_DEVICE_ADD("audiocpu", z80_device.Z80, galaxian_state.KONAMI_SOUND_CLOCK/8);
            MCFG_DEVICE_PROGRAM_MAP(galaxian_state_frogger_sound_map);
            MCFG_DEVICE_IO_MAP(galaxian_state_frogger_sound_portmap);

            MCFG_GENERIC_LATCH_8_ADD("soundlatch");

            /* sound hardware */
            MCFG_DEVICE_ADD("8910.0", ay8910_device.AY8910, galaxian_state.KONAMI_SOUND_CLOCK/8);
            MCFG_AY8910_OUTPUT_TYPE(ay8910_global.AY8910_DISCRETE_OUTPUT);
            MCFG_AY8910_RES_LOADS((int)RES_K(5.1), (int)RES_K(5.1), (int)RES_K(5.1));
            MCFG_AY8910_PORT_A_READ_CB(READ8("soundlatch", galaxian_state.generic_latch_8_device_read));
            MCFG_AY8910_PORT_B_READ_CB(READ8(galaxian_state.frogger_sound_timer_r));
            MCFG_SOUND_ROUTE(0, "konami", 1.0, 0);
            MCFG_SOUND_ROUTE(1, "konami", 1.0, 1);
            MCFG_SOUND_ROUTE(2, "konami", 1.0, 2);

            MCFG_DEVICE_ADD("konami", discrete_sound_device.DISCRETE);//, konami_sound_discrete);
            MCFG_DEVICE_ADD_discrete_sound_device(konami_sound_discrete);
            MCFG_SOUND_ROUTE(ALL_OUTPUTS, "speaker", 0.75);

            MACHINE_CONFIG_END();
        }


        /*************************************
         *
         *  Machine drivers
         *
         *************************************/

        //MACHINE_CONFIG_START(galaxian_state::galaxian)
        void galaxian_state_galaxian(machine_config config, device_t owner, device_t device)
        {
            galaxian_state_galaxian_base(config, owner, device);
            galaxian_state_galaxian_audio(config, owner, device);

            MACHINE_CONFIG_START(config, owner, device);

            MACHINE_CONFIG_END();
        }


        //MACHINE_CONFIG_START(galaxian_state::frogger)
        void galaxian_state_frogger(machine_config config, device_t owner, device_t device)
        {
            galaxian_state_konami_base(config, owner, device);
            galaxian_state_konami_sound_1x_ay8910(config, owner, device);

            MACHINE_CONFIG_START(config, owner, device);

            galaxian_state galaxian_state = (galaxian_state)helper_owner;

            /* alternate memory map */
            MCFG_DEVICE_MODIFY("maincpu");
            MCFG_DEVICE_PROGRAM_MAP(galaxian_state_frogger_map);

            MACHINE_CONFIG_END();
        }


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

            ROM_END(),
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

            ROM_END(),
        };



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
        //                                                         creator,                 rom           YEAR,   NAME,       PARENT,  MACHINE,                             INPUT,                                 INIT,                                         MONITOR,COMPANY, FULLNAME,FLAGS
        public static readonly game_driver driver_galaxian = GAME( device_creator_galaxian, rom_galaxian, "1979", "galaxian", null,    m_galaxian.galaxian_state_galaxian,  m_galaxian.construct_ioport_galaxian,  galaxian_state.galaxian_state_init_galaxian,  ROT90,  "Namco", "Galaxian (Namco set 1)", MACHINE_SUPPORTS_SAVE );


        /*************************************
         *
         *  Game drivers
         *  Konami games
         *
         *************************************/

        /* Frogger based hardware: 2nd Z80, AY-8910A, 2 8255 PPI for I/O, custom background */
        //                                                        creator,                rom          YEAR,   NAME,      PARENT,  MACHINE,                           INPUT,                               INIT,                                       MONITOR,COMPANY, FULLNAME,FLAGS
        public static readonly game_driver driver_frogger = GAME( device_creator_frogger, rom_frogger, "1981", "frogger", null,    m_galaxian.galaxian_state_frogger, m_galaxian.construct_ioport_frogger, galaxian_state.galaxian_state_init_frogger, ROT90,  "Konami", "Frogger", MACHINE_SUPPORTS_SAVE );
    }
}
