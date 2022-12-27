// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.diexec_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.m6502_global;
using static mame.palette_global;
using static mame.pokey_global;
using static mame.rescap_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.util;
using static mame.watchdog_global;


namespace mame
{
    partial class missile_state : driver_device
    {
        required_device<m6502_device> m_maincpu;
        required_shared_ptr<uint8_t> m_videoram;
        required_device<watchdog_timer_device> m_watchdog;
        optional_device<pokey_device> m_pokey;
        required_ioport m_in0;
        required_ioport m_in1;
        required_ioport m_r10;
        required_ioport m_r8;
        required_ioport m_track0_x;
        required_ioport m_track0_y;
        required_ioport m_track1_x;
        required_ioport m_track1_y;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;
        output_finder<u32_const_2> m_leds;

        required_region_ptr<uint8_t> m_mainrom;
        required_region_ptr<uint8_t> m_writeprom;
        emu_timer m_irq_timer;
        emu_timer m_cpu_timer;
        uint8_t m_irq_state;
        uint8_t m_ctrld;
        uint8_t m_flipscreen;
        uint64_t m_madsel_lastcycles;


        public missile_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<m6502_device>(this,"maincpu");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_watchdog = new required_device<watchdog_timer_device>(this, "watchdog");
            m_pokey = new optional_device<pokey_device>(this, "pokey");
            m_in0 = new required_ioport(this, "IN0");
            m_in1 = new required_ioport(this, "IN1");
            m_r10 = new required_ioport(this, "R10");
            m_r8 = new required_ioport(this, "R8");
            m_track0_x = new required_ioport(this, "TRACK0_X");
            m_track0_y = new required_ioport(this, "TRACK0_Y");
            m_track1_x = new required_ioport(this, "TRACK1_X");
            m_track1_y = new required_ioport(this, "TRACK1_Y");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_leds = new output_finder<u32_const_2>(this, "led{0}", 0U);
            m_mainrom = new required_region_ptr<uint8_t>(this, "maincpu");
            m_writeprom = new required_region_ptr<uint8_t>(this, "proms");
        }
    }


    partial class missile_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK    = new XTAL(10_000_000);

        static readonly XTAL PIXEL_CLOCK     = MASTER_CLOCK / 2;
        const u16 HTOTAL          = 320;
        const u16 HBSTART         = 256;
        const u16 HBEND           = 0;
        const u16 VTOTAL          = 256;
        const u16 VBSTART         = 256;
        const u16 VBEND           = 25;    /* 24 causes a garbage line at the top of the screen */


        /*************************************
         *
         *  VBLANK and IRQ generation
         *
         *************************************/

        int scanline_to_v(int scanline)
        {
            /* since the vertical sync counter counts backwards when flipped,
                this function returns the current effective V value, given
                that vpos() only counts forward */
            return m_flipscreen != 0 ? (256 - scanline) : scanline;
        }


        int v_to_scanline(int v)
        {
            /* same as a above, but the opposite transformation */
            return m_flipscreen != 0 ? (256 - v) : v;
        }


        void schedule_next_irq(int curv)
        {
            /* IRQ = /32V, clocked by /16V ^ flip */
            /* When not flipped, clocks on 0, 64, 128, 192 */
            /* When flipped, clocks on 16, 80, 144, 208 */
            if (m_flipscreen != 0)
                curv = ((curv - 32) & 0xff) | 0x10;
            else
                curv = ((curv + 32) & 0xff) & ~0x10;

            /* next one at the start of this scanline */
            m_irq_timer.adjust(m_screen.op0.time_until_pos(v_to_scanline(curv)), curv);
        }


        //TIMER_CALLBACK_MEMBER(missile_state::clock_irq)
        void clock_irq(object ptr, s32 param)  //void *ptr, s32 param)
        {
            int curv = param;

            /* assert the IRQ if not already asserted */
            m_irq_state = (uint8_t)((~curv >> 5) & 1);
            m_maincpu.op0.set_input_line(0, m_irq_state != 0 ? ASSERT_LINE : CLEAR_LINE);

            /* force an update while we're here */
            m_screen.op0.update_partial(v_to_scanline(curv));

            /* find the next edge */
            schedule_next_irq(curv);
        }


        //READ_LINE_MEMBER(missile_state::vblank_r)
        public int vblank_r()
        {
            int v = scanline_to_v(m_screen.op0.vpos());
            return v < 24 ? 1 : 0;
        }


        /*************************************
         *
         *  Machine setup
         *
         *************************************/
        //TIMER_CALLBACK_MEMBER(missile_state::adjust_cpu_speed)
        void adjust_cpu_speed(object ptr, s32 param)  //void *ptr, s32 param)
        {
            int curv = param;

            /* starting at scanline 224, the CPU runs at half speed */
            if (curv == 224)
                m_maincpu.op0.set_unscaled_clock(MASTER_CLOCK / 16);
            else
                m_maincpu.op0.set_unscaled_clock(MASTER_CLOCK / 8);

            /* scanline for the next run */
            curv ^= 224;
            m_cpu_timer.adjust(m_screen.op0.time_until_pos(v_to_scanline(curv)), curv);
        }


        protected override void machine_start()
        {
            m_leds.resolve();

            /* initialize globals */
            m_flipscreen = 0;
            m_ctrld = 0;

            /* create a timer to speed/slow the CPU */
            m_cpu_timer = machine().scheduler().timer_alloc(adjust_cpu_speed);
            m_cpu_timer.adjust(m_screen.op0.time_until_pos(v_to_scanline(0), 0));

            /* create a timer for IRQs and set up the first callback */
            m_irq_timer = machine().scheduler().timer_alloc(clock_irq);
            m_irq_state = 0;
            schedule_next_irq(-32);

            /* setup for save states */
            save_item(NAME(new { m_irq_state }));
            save_item(NAME(new { m_ctrld }));
            save_item(NAME(new { m_flipscreen }));
            save_item(NAME(new { m_madsel_lastcycles }));
        }


        protected override void machine_reset()
        {
            m_maincpu.op0.set_input_line(0, CLEAR_LINE);
            m_irq_state = 0;
            m_madsel_lastcycles = 0;
        }


        /*************************************
         *
         *  VRAM access
         *
         *************************************/
        bool get_madsel()
        {
            /* the MADSEL signal disables standard address decoding and routes
                writes to video RAM; it goes high 5 cycles after an opcode
                fetch where the low 5 bits are 0x01 and the IRQ signal is clear.
            */
            bool madsel = false;

            if (m_madsel_lastcycles != 0)
            {
                madsel = (m_maincpu.op0.total_cycles() - m_madsel_lastcycles) == 5;

                /* reset the count until next time */
                if (madsel)
                    m_madsel_lastcycles = 0;
            }

            return madsel;
        }


        offs_t get_bit3_addr(offs_t pixaddr)
        {
            /* the 3rd bit of video RAM is scattered about various areas
                we take a 16-bit pixel address here and convert it into
                a video RAM address based on logic in the schematics */
            return  (( pixaddr & 0x0800) >> 1) |
                    ((~pixaddr & 0x0800) >> 2) |
                    (( pixaddr & 0x07f8) >> 2) |
                    (( pixaddr & 0x1000) >> 12);
        }


        void write_vram(offs_t address, uint8_t data)
        {
            uint8_t [] data_lookup = new uint8_t [4] { 0x00, 0x0f, 0xf0, 0xff };
            offs_t vramaddr;
            uint8_t vramdata;
            uint8_t vrammask;

            /* basic 2 bit VRAM writes go to addr >> 2 */
            /* data comes from bits 6 and 7 */
            /* this should only be called if MADSEL == 1 */
            vramaddr = address >> 2;
            vramdata = data_lookup[data >> 6];
            vrammask = m_writeprom.op[(address & 7) | 0x10];
            m_videoram[vramaddr].op = (uint8_t)((m_videoram.op[vramaddr] & vrammask) | (vramdata & ~vrammask));

            /* 3-bit VRAM writes use an extra clock to write the 3rd bit elsewhere */
            /* on the schematics, this is the MUSHROOM == 1 case */
            if ((address & 0xe000) == 0xe000)
            {
                vramaddr = get_bit3_addr(address);
                vramdata = (uint8_t)(-((data >> 5) & 1));
                vrammask = m_writeprom.op[(address & 7) | 0x18];
                m_videoram[vramaddr].op = (uint8_t)((m_videoram.op[vramaddr] & vrammask) | (vramdata & ~vrammask));

                /* account for the extra clock cycle */
                m_maincpu.op0.GetClassInterface<device_execute_interface>().adjust_icount(-1);
            }
        }


        uint8_t read_vram(offs_t address)
        {
            offs_t vramaddr;
            uint8_t vramdata;
            uint8_t vrammask;
            uint8_t result = 0xff;

            /* basic 2 bit VRAM reads go to addr >> 2 */
            /* data goes to bits 6 and 7 */
            /* this should only be called if MADSEL == 1 */
            vramaddr = address >> 2;
            vrammask = (uint8_t)(0x11 << (int)(address & 3));
            vramdata = (uint8_t)(m_videoram.op[vramaddr] & vrammask);
            if ((vramdata & 0xf0) == 0)
                result &= unchecked((uint8_t)~0x80);
            if ((vramdata & 0x0f) == 0)
                result &= unchecked((uint8_t)~0x40);

            /* 3-bit VRAM reads use an extra clock to read the 3rd bit elsewhere */
            /* on the schematics, this is the MUSHROOM == 1 case */
            if ((address & 0xe000) == 0xe000)
            {
                vramaddr = get_bit3_addr(address);
                vrammask = (uint8_t)(1U << (int)(address & 7));
                vramdata = (uint8_t)(m_videoram.op[vramaddr] & vrammask);
                if (vramdata == 0)
                    result &= unchecked((uint8_t)~0x20);

                /* account for the extra clock cycle */
                m_maincpu.op0.GetClassInterface<device_execute_interface>().adjust_icount(-1);
            }

            return result;
        }


        /*************************************
         *
         *  Video update
         *
         *************************************/
        uint32_t screen_update_missile(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            // draw the bitmap to the screen, looping over Y
            for (int y = cliprect.top(); y <= cliprect.bottom(); y++)
            {
                PointerU16 dst = bitmap.pix(y);  //uint16_t *const dst = &bitmap.pix(y);

                int effy = m_flipscreen != 0 ? ((256+24 - y) & 0xff) : y;
                Pointer<uint8_t> src = m_videoram.op + (effy * 64);  //uint8_t const *const src = &m_videoram[effy * 64];
                Pointer<uint8_t> src3 = null;  //uint8_t const *src3 = nullptr;

                // compute the base of the 3rd pixel row
                if (effy >= 224)
                    src3 = m_videoram.op + get_bit3_addr((offs_t)effy << 8);  //src3 = &m_videoram[get_bit3_addr(effy << 8)];

                // loop over X
                for (int x = cliprect.left(); x <= cliprect.right(); x++)
                {
                    uint8_t pix = (uint8_t)(src[x / 4] >> (x & 3));
                    pix = (uint8_t)(((pix >> 2) & 4) | ((pix << 1) & 2));

                    // if we're in the lower region, get the 3rd bit
                    if (src3 != null)
                        pix |= (uint8_t)((src3[(x / 8) * 2] >> (x & 7)) & 1);

                    dst[x] = pix;
                }
            }

            return 0;
        }


        /*************************************
         *
         *  Global read/write handlers
         *
         *************************************/
        void missile_w(offs_t offset, uint8_t data)
        {
            /* if this is a MADSEL cycle, write to video RAM */
            if (get_madsel())
            {
                write_vram(offset, data);
                return;
            }

            /* otherwise, strip A15 and handle manually */
            offset &= 0x7fff;

            /* RAM */
            if (offset < 0x4000)
                m_videoram[offset].op = data;

            /* POKEY */
            else if (offset < 0x4800)
            {
                if (m_pokey.found())
                    m_pokey.op0.write(offset, data);
            }

            /* OUT0 */
            else if (offset < 0x4900)
            {
                m_flipscreen = (uint8_t)(~data & 0x40);
                machine().bookkeeping().coin_counter_w(0, data & 0x20);
                machine().bookkeeping().coin_counter_w(1, data & 0x10);
                machine().bookkeeping().coin_counter_w(2, data & 0x08);
                m_leds[1] = BIT(~data, 2);
                m_leds[0] = BIT(~data, 1);
                m_ctrld = (uint8_t)(data & 1);
            }

            /* color RAM */
            else if (offset >= 0x4b00 && offset < 0x4c00)
                m_palette.op0.set_pen_color(offset & 7, pal1bit((uint8_t)(~data >> 3)), pal1bit((uint8_t)(~data >> 2)), pal1bit((uint8_t)(~data >> 1)));

            /* watchdog */
            else if (offset >= 0x4c00 && offset < 0x4d00)
                m_watchdog.op0.watchdog_reset();

            /* interrupt ack */
            else if (offset >= 0x4d00 && offset < 0x4e00)
            {
                if (m_irq_state != 0)
                {
                    m_maincpu.op0.set_input_line(0, CLEAR_LINE);
                    m_irq_state = 0;
                }
            }

            /* anything else */
            else
                logerror("{0}:Unknown write to {1} = {2}\n", m_maincpu.op0.GetClassInterface<device_state_interface>().pc(), offset, data);
        }


        uint8_t missile_r(offs_t offset)
        {
            uint8_t result = 0xff;

            /* if this is a MADSEL cycle, read from video RAM */
            if (get_madsel())
                return read_vram(offset);

            /* otherwise, strip A15 and handle manually */
            offset &= 0x7fff;

            /* RAM */
            if (offset < 0x4000)
                result = m_videoram.op[offset];

            /* ROM */
            else if (offset >= 0x5000)
                result = m_mainrom.op[offset];

            /* POKEY */
            else if (offset < 0x4800)
            {
                if (m_pokey.found())
                    result = m_pokey.op0.read(offset & 0x0f);
            }

            /* IN0 */
            else if (offset < 0x4900)
            {
                if (m_ctrld != 0)    /* trackball */
                {
                    if (m_flipscreen == 0)
                        result = (uint8_t)(((m_track0_y.op0.read() << 4) & 0xf0) | (m_track0_x.op0.read() & 0x0f));
                    else
                        result = (uint8_t)(((m_track1_y.op0.read() << 4) & 0xf0) | (m_track1_x.op0.read() & 0x0f));
                }
                else    /* buttons */
                    result = (uint8_t)m_in0.op0.read();
            }

            /* IN1 */
            else if (offset < 0x4a00)
                result = (uint8_t)m_in1.op0.read();

            /* IN2 */
            else if (offset < 0x4b00)
                result = (uint8_t)m_r10.op0.read();

            /* anything else */
            else
                logerror("{0}:Unknown read from {1}\n", m_maincpu.op0.GetClassInterface<device_state_interface>().pc(), offset);


            /* update the MADSEL state */
            if (m_irq_state == 0 && ((result & 0x1f) == 0x01) && m_maincpu.op0.get_sync())
                m_madsel_lastcycles = m_maincpu.op0.total_cycles();

            return result;
        }


        //void missile_state::bootleg_w(offs_t offset, uint8_t data)

        //uint8_t missile_state::bootleg_r(offs_t offset)


        /*************************************
         *
         *  Main CPU memory handlers
         *
         *************************************/
        /* complete memory map derived from schematics (implemented above) */
        void main_map(address_map map, device_t owner)
        {
            map.op(0x0000, 0xffff).rw(missile_r, missile_w).share("videoram");
        }


        //void missile_state::bootleg_main_map(address_map &map)
    }


    partial class missile : construct_ioport_helper
    {
        /*************************************
         *
         *  Port definitions
         *
         *************************************/
        //static INPUT_PORTS_START( missile )
        void construct_ioport_missile(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            missile_state missile_state = (missile_state)owner;

            PORT_START("IN0");   /* IN0 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON3 ); PORT_COCKTAIL();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON2 ); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_COIN3 );

            PORT_START("IN1");   /* IN1 */
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON3 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON2 );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x18, IP_ACTIVE_HIGH, IPT_CUSTOM );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_TILT );
            PORT_SERVICE( 0x40, IP_ACTIVE_LOW );
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_READ_LINE_MEMBER(missile_state.vblank_r);

            PORT_START("R10");   /* IN2 */
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Coinage ) ); PORT_DIPLOCATION("R10:1,2");
            PORT_DIPSETTING(    0x01, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( Free_Play ) );
            PORT_DIPNAME( 0x0c, 0x00, "Right Coin" ); PORT_DIPLOCATION("R10:3,4");
            PORT_DIPSETTING(    0x00, "*1" );
            PORT_DIPSETTING(    0x04, "*4" );
            PORT_DIPSETTING(    0x08, "*5" );
            PORT_DIPSETTING(    0x0c, "*6" );
            PORT_DIPNAME( 0x10, 0x00, "Center Coin" ); PORT_DIPLOCATION("R10:5");
            PORT_DIPSETTING(    0x00, "*1" );
            PORT_DIPSETTING(    0x10, "*2" );
            PORT_DIPNAME( 0x60, 0x00, DEF_STR( Language ) ); PORT_DIPLOCATION("R10:6,7");
            PORT_DIPSETTING(    0x00, DEF_STR( English ) );
            PORT_DIPSETTING(    0x20, DEF_STR( French ) );
            PORT_DIPSETTING(    0x40, DEF_STR( German ) );
            PORT_DIPSETTING(    0x60, DEF_STR( Spanish ) );
            PORT_DIPNAME( 0x80, 0x80, DEF_STR( Unknown ) ); PORT_DIPLOCATION("R10:8");
            PORT_DIPSETTING(    0x80, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );

            PORT_START("R8");    /* IN3 */
            PORT_DIPNAME( 0x03, 0x03, "Cities" ); PORT_DIPLOCATION("R8:!1,!2");
            PORT_DIPSETTING(    0x02, "4" );
            PORT_DIPSETTING(    0x01, "5" );
            PORT_DIPSETTING(    0x03, "6" );
            PORT_DIPSETTING(    0x00, "7" );
            PORT_DIPNAME( 0x04, 0x04, "Bonus Credit for 4 Coins" ); PORT_DIPLOCATION("R8:!3");
            PORT_DIPSETTING(    0x04, DEF_STR( No ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Yes ) );
            PORT_DIPNAME( 0x08, 0x08, "Trackball Size" ); PORT_DIPLOCATION("R8:!4");
            PORT_DIPSETTING(    0x00, "Mini" ); // Faster Cursor Speed
            PORT_DIPSETTING(    0x08, "Large" ); // Slower Cursor Speed
            PORT_DIPNAME( 0x70, 0x70, "Bonus City" ); PORT_DIPLOCATION("R8:!5,!6,!7");
            PORT_DIPSETTING(    0x10, "8000" );
            PORT_DIPSETTING(    0x70, "10000" );
            PORT_DIPSETTING(    0x60, "12000" );
            PORT_DIPSETTING(    0x50, "14000" );
            PORT_DIPSETTING(    0x40, "15000" );
            PORT_DIPSETTING(    0x30, "18000" );
            PORT_DIPSETTING(    0x20, "20000" );
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );
            PORT_DIPNAME( 0x80, 0x00, DEF_STR( Cabinet ) ); PORT_DIPLOCATION("R8:!8");
            PORT_DIPSETTING(    0x00, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x80, DEF_STR( Cocktail ) );

            PORT_START("TRACK0_X");  /* FAKE */
            PORT_BIT( 0x0f, 0x00, IPT_TRACKBALL_X ); PORT_SENSITIVITY(20); PORT_KEYDELTA(10);

            PORT_START("TRACK0_Y");  /* FAKE */
            PORT_BIT( 0x0f, 0x00, IPT_TRACKBALL_Y ); PORT_SENSITIVITY(20); PORT_KEYDELTA(10); PORT_REVERSE();

            PORT_START("TRACK1_X");  /* FAKE */
            PORT_BIT( 0x0f, 0x00, IPT_TRACKBALL_X ); PORT_SENSITIVITY(20); PORT_KEYDELTA(10); PORT_REVERSE(); PORT_COCKTAIL();

            PORT_START("TRACK1_Y");  /* FAKE */
            PORT_BIT( 0x0f, 0x00, IPT_TRACKBALL_Y ); PORT_SENSITIVITY(20); PORT_KEYDELTA(10); PORT_REVERSE(); PORT_COCKTAIL();

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( missileb )

        //static INPUT_PORTS_START( suprmatk )
    }


    partial class missile_state : driver_device
    {
        /*************************************
         *
         *  Machine driver
         *
         *************************************/
        public void missile(machine_config config)
        {
            /* basic machine hardware */
            M6502(config, m_maincpu, MASTER_CLOCK / 8);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, main_map);

            WATCHDOG_TIMER(config, m_watchdog).set_vblank_count(m_screen, 8);

            /* video hardware */
            PALETTE(config, m_palette).set_entries(8);

            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(PIXEL_CLOCK, HTOTAL, HBEND, HBSTART, VTOTAL, VBEND, VBSTART);
            m_screen.op0.set_screen_update(screen_update_missile);
            m_screen.op0.set_palette(m_palette);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            POKEY(config, m_pokey, MASTER_CLOCK/8);
            m_pokey.op0.allpot_r().set_ioport("R8").reg();
            m_pokey.op0.set_output_rc(RES_K(10), CAP_U(0.1), 5.0);
            m_pokey.op0.disound.add_route(ALL_OUTPUTS, "mono", 1.0);
        }


        //void missile_state::missilea(machine_config &config)

        //void missile_state::missileb(machine_config &config)
    }


    partial class missile : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definitions
         *
         *************************************/
        //ROM_START( missile )
        static readonly tiny_rom_entry [] rom_missile =
        {
            ROM_REGION( 0x8000, "maincpu", 0 ),
            ROM_LOAD( "035820-02.h1",  0x5000, 0x0800, CRC("7a62ce6a") + SHA1("9a39978138dc28fdefe193bfae1b226391e471db") ),
            ROM_LOAD( "035821-02.jk1", 0x5800, 0x0800, CRC("df3bd57f") + SHA1("0916925d3c94d766d33f0e4badf6b0add835d748") ),
            ROM_LOAD( "035822-03e.kl1",0x6000, 0x0800, CRC("1a2f599a") + SHA1("2deb1219223032a9c83114e4e8b2fc11a570754c") ),
            ROM_LOAD( "035823-02.ln1", 0x6800, 0x0800, CRC("82e552bb") + SHA1("d0f22894f779c74ceef644c9f03d840d9545efea") ),
            ROM_LOAD( "035824-02.np1", 0x7000, 0x0800, CRC("606e42e0") + SHA1("9718f84a73c66b4e8ef7805a7ab638a7380624e1") ),
            ROM_LOAD( "035825-02.r1",  0x7800, 0x0800, CRC("f752eaeb") + SHA1("0339a6ce6744d2091cc7e07675e509b202b0f380") ),

            ROM_REGION( 0x0020, "proms", 0 ),
            ROM_LOAD( "035826-01.l6",  0x0000, 0x0020, CRC("86a22140") + SHA1("2beebf7855e29849ada1823eae031fc98220bc43") ),

            ROM_END,
        };


        //ROM_START( missile2 )

        //ROM_START( missile1 )

        //ROM_START( suprmatk )

        //ROM_START( suprmatkd )


        /*

        Missile Command Multigame, produced by Braze Technologies
        from 2005(1st version) to 2007(version 1d). This kit combines
        Missile Command and Super Missile Attack on a daughterboard
        plugged into the main pcb cpu slot.

        - M6502 CPU (from main pcb)
        - 27C512 64KB EPROM
        - 93C46P E2PROM for saving highscore/settings
        - two 74LS chips (labels sandpapered off)

        */

        //ROM_START( missilem )


        /*

        Missile Combat bootlegs by 'Videotron'

        1x 6502A (main)
        1x AY-3-8912 (sound)
        1x oscillator 10000

        PCB is marked: "VIDEOTRON BOLOGNA 002"

        */

        //ROM_START( mcombat )

        //ROM_START( mcombata )

        //ROM_START( mcombats ) /* bootleg (Sidam) @ $ */

        /*
        CPUs
        QTY     Type    clock   position    function
        1x  6502        2B  8-bit Microprocessor - main
        1x  LM380       12B     Audio Amplifier - sound
        1x  oscillator  10.000  6C

        ROMs
        QTY     Type    position    status
        2x  F2708   10C, 10E    dumped
        6x  MCM2716C    1-6     dumped
        1x  DM74S288N   6L  dumped

        RAMs
        QTY     Type    position
        8x  TMS4116     4F,4H,4J,4K,4L,4M,4N,4P
        1x  74S189N     7L

        Others

        1x 22x2 edge connector
        1x trimmer (volume)(12E)
        2x 8x2 switches DIP(8R,10R)
        */

        //ROM_START( missilea )
    }


        /*************************************
         *
         *  Driver initialization
         *
         *************************************/

        //void missile_state::init_suprmatk()

        //void missile_state::init_missilem()


    partial class missile : construct_ioport_helper
    {
        /*************************************
         *
         *  Game drivers
         *
         *************************************/

        static void missile_state_missile(machine_config config, device_t device) { missile_state missile_state = (missile_state)device; missile_state.missile(config); }

        static missile m_missile = new missile();

        static device_t device_creator_missile(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new missile_state(mconfig, type, tag); }

        public static readonly game_driver driver_missile = GAME(device_creator_missile, rom_missile, "1980", "missile", "0", missile_state_missile, m_missile.construct_ioport_missile, driver_device.empty_init, ROT0, "Atari", "Missile Command (rev 3)", MACHINE_SUPPORTS_SAVE);
    }
}
