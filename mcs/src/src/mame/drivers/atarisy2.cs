// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.adc0808_global;
using static mame.atarimo_global;
using static mame.diexec_global;
using static mame.digfx_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.eeprom_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.gamedrv_global;
using static mame.gen_latch_global;
using static mame.hash_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.m6502_global;
using static mame.pokey_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.slapstic_global;
using static mame.speaker_global;
using static mame.t11_global;
using static mame.tilemap_global;
using static mame.timer_global;
using static mame.tms5220_global;
using static mame.util;
using static mame.watchdog_global;
using static mame.ymopm_global;


namespace mame
{
    partial class atarisy2_state : driver_device
    {
        static readonly XTAL MASTER_CLOCK = new XTAL(20000000);
        static readonly XTAL SOUND_CLOCK  = new XTAL(14318181);
        static readonly XTAL VIDEO_CLOCK  = new XTAL(32000000);


        /*************************************
         *
         *  Interrupt updating
         *
         *************************************/
        protected void update_interrupts()
        {
            if (m_video_int_state)
                m_maincpu.op0.set_input_line(3, ASSERT_LINE);
            else
                m_maincpu.op0.set_input_line(3, CLEAR_LINE);

            if (m_scanline_int_state)
                m_maincpu.op0.set_input_line(2, ASSERT_LINE);
            else
                m_maincpu.op0.set_input_line(2, CLEAR_LINE);

            if (m_p2portwr_state)
                m_maincpu.op0.set_input_line(1, ASSERT_LINE);
            else
                m_maincpu.op0.set_input_line(1, CLEAR_LINE);

            if (m_p2portrd_state)
                m_maincpu.op0.set_input_line(0, ASSERT_LINE);
            else
                m_maincpu.op0.set_input_line(0, CLEAR_LINE);
        }


        void scanline_int_ack_w(uint8_t data)
        {
            m_scanline_int_state = false;
            update_interrupts();
        }


        void video_int_ack_w(uint8_t data)
        {
            m_video_int_state = false;
            update_interrupts();
        }


        /*************************************
         *
         *  Every 8-scanline update
         *
         *************************************/

        //TIMER_DEVICE_CALLBACK_MEMBER(atarisy2_state::scanline_update)
        protected void scanline_update(timer_device timer, object ptr, s32 param)  //void *ptr, s32 param
        {
            int scanline = param;
            if (scanline <= m_screen.op0.height())
            {
                // generate the 32V interrupt (IRQ 2)
                if ((scanline % 64) == 0)
                {
                    // clock the state through
                    m_scanline_int_state = BIT(m_interrupt_enable, 2) != 0;
                    update_interrupts();
                }
            }
        }


        /*************************************
         *
         *  Initialization
         *
         *************************************/
        protected override void machine_start()
        {
            m_leds.resolve();

            m_scanline_int_state = false;
            m_video_int_state = false;
            m_p2portwr_state = false;
            m_p2portrd_state = false;

            save_item(NAME(new { m_interrupt_enable }));
            save_item(NAME(new { m_scanline_int_state }));
            save_item(NAME(new { m_video_int_state }));
            save_item(NAME(new { m_p2portwr_state }));
            save_item(NAME(new { m_p2portrd_state }));
            save_item(NAME(new { m_sound_reset_state }));

            for (int bank = 0; bank < 2; bank++)
                m_rombank[bank].op0.configure_entries(0, 64, new PointerU8(memregion("maincpu").base_()) + 0x10000, 0x2000);
        }


        protected override void machine_reset()
        {
            m_interrupt_enable = 0;

            sound_reset_w(1);
        }


        /*************************************
         *
         *  Interrupt handlers
         *
         *************************************/
        //WRITE_LINE_MEMBER(atarisy2_state::vblank_int)
        void vblank_int(int state)
        {
            if (state != 0)
            {
                // clock the VBLANK through
                m_video_int_state = BIT(m_interrupt_enable, 3) != 0;
                update_interrupts();
            }
        }


        void int0_ack_w(uint8_t data)
        {
            // reset sound IRQ
            m_p2portrd_state = false;
            update_interrupts();
        }


        void sound_reset_w(uint8_t data)
        {
            // reset sound CPU
            m_audiocpu.op0.set_input_line(INPUT_LINE_RESET, BIT(data, 0) != 0 ? ASSERT_LINE : CLEAR_LINE);

            sndrst_6502_w(0);
            coincount_w(0);
            switch_6502_w(0);
        }


        //TIMER_CALLBACK_MEMBER(atarisy2_state::delayed_int_enable_w)
        void delayed_int_enable_w(s32 param)
        {
            m_interrupt_enable = (uint8_t)param;
        }


        void int_enable_w(uint8_t data)
        {
            machine().scheduler().synchronize(delayed_int_enable_w, data);
        }


        //INTERRUPT_GEN_MEMBER(atarisy2_state::sound_irq_gen)
        void sound_irq_gen(device_t device)
        {
            m_audiocpu.op0.set_input_line(m6502_device.IRQ_LINE, ASSERT_LINE);
        }


        void sound_irq_ack_w(uint8_t data)
        {
            m_audiocpu.op0.set_input_line(m6502_device.IRQ_LINE, CLEAR_LINE);
        }


        //WRITE_LINE_MEMBER(atarisy2_state::boost_interleave_hack)
        public void boost_interleave_hack(int state)
        {
            // apb3 fails the self-test with a 100 µs delay or less
            if (state != 0)
                machine().scheduler().boost_interleave(attotime.zero, attotime.from_usec(200));
        }


        /*************************************
         *
         *  Bank selection.
         *
         *************************************/

        void bankselect_w(offs_t offset, uint16_t data)
        {
            /*static const int bankoffset[64] =
            {
                12, 8, 4, 0,
                13, 9, 5, 1,
                14, 10, 6, 2,
                15, 11, 7, 3,
                28, 24, 20, 16,
                29, 25, 21, 17,
                30, 26, 22, 18,
                31, 27, 23, 19,
                44, 40, 36, 32,
                45, 41, 37, 33,
                46, 42, 38, 34,
                47, 43, 39, 35,
                60, 56, 52, 48,
                61, 57, 53, 49,
                62, 58, 54, 50,
                63, 59, 55, 51
            };*/

            uint8_t banknumber = (uint8_t)((((uint32_t)data >> 10) & 077) ^ 0x03);
            banknumber = (uint8_t)bitswap(banknumber, 5, 4, 1, 0, 3, 2);  //banknumber = bitswap<6>(banknumber, 5, 4, 1, 0, 3, 2);

            m_rombank[(int)offset].op0.set_entry(banknumber);
        }


        protected override void device_post_load()
        {
        }


        /*************************************
         *
         *  I/O read dispatch.
         *
         *************************************/
        uint16_t switch_r()
        {
            return (uint16_t)(ioport("1800").read() | (ioport("1801").read() << 8));
        }


        uint8_t switch_6502_r()
        {
            int result = (int)ioport("1840").read();

            if (m_tms5220.found() && (m_tms5220.op0.readyq_r() == 0))
                result &= ~0x04;

            if ((ioport("1801").read() & 0x80) == 0) result |= 0x10;

            return (uint8_t)result;
        }


        void switch_6502_w(uint8_t data)
        {
            m_leds[0] = BIT(data, 2);
            m_leds[1] = BIT(data, 3);
            if (m_tms5220.found())
            {
                data = (uint8_t)(12 | (((uint32_t)data >> 5) & 1));
                m_tms5220.op0.set_unscaled_clock(MASTER_CLOCK / 4 / (16 - data) / 2);
            }
        }


        uint8_t leta_r(offs_t offset)
        {
            throw new emu_unimplemented();
        }


        void mixer_w(uint8_t data)
        {
            double rbott;
            double rtop;
            double gain;

            // these gains are cheesed up, but give an approximate effect

            /*
             * Before the volume adjustment, all channels pass through
             * a high-pass filter which removes DC components. The
             * filter frequency does also depend on the settings on
             * the resistors.
             *
             * The op-amp after the pokey feeds mixes the op-amp output voltage
             * with a low impedance back to the input. The internal resistance of the
             * pokey now is the ground pole of a three pole resistor mixer: ground,
             * 15V and op-amp output voltage.
             *
             * ==> DISCRETE candidate
             *
             */

            // bits 0-2 control the volume of the YM2151, using 22k, 47k, and 100k resistors
            rtop = 1.0 / (1.0 / 100 + 1.0 / 100);
            rbott = 0;
            if ((data & 0x01) == 0) rbott += 1.0 / 100;
            if ((data & 0x02) == 0) rbott += 1.0 / 47;
            if ((data & 0x04) == 0) rbott += 1.0 / 22;
            gain = (rbott == 0) ? 1.0 : ((1.0 / rbott) / (rtop + (1.0 / rbott)));
            m_ym2151.op0.disound.set_output_gain(ALL_OUTPUTS, (float)gain);

            // bits 3-4 control the volume of the POKEYs, using 47k and 100k resistors
            rtop = 1.0 / (1.0 / 100 + 1.0 / 100);
            rbott = 0;
            if ((data & 0x08) == 0) rbott += 1.0 / 47;
            if ((data & 0x10) == 0) rbott += 1.0 / 22;
            gain = (rbott == 0) ? 1.0 : ((1.0 / rbott) / (rtop + (1.0 / rbott)));
            m_pokey[0].op0.disound.set_output_gain(ALL_OUTPUTS, (float)gain);
            m_pokey[1].op0.disound.set_output_gain(ALL_OUTPUTS, (float)gain);

            // bits 5-7 control the volume of the TMS5220, using 22k, 47k, and 100k resistors
            if (m_tms5220.found())
            {
                rtop = 1.0 / (1.0 / 100 + 1.0 / 100);
                rbott = 0;
                if ((data & 0x20) == 0) rbott += 1.0 / 100;
                if ((data & 0x40) == 0) rbott += 1.0 / 47;
                if ((data & 0x80) == 0) rbott += 1.0 / 22;
                gain = (rbott == 0) ? 1.0 : ((1.0 / rbott) / (rtop + (1.0 / rbott)));
                m_tms5220.op0.disound.set_output_gain(ALL_OUTPUTS, (float)gain);
            }
        }


        void sndrst_6502_w(uint8_t data)
        {
            // if no change, do nothing
            if ((data & 1) == m_sound_reset_state)
                return;

            m_sound_reset_state = (uint8_t)(data & 1);
            m_ym2151.op0.reset_w(m_sound_reset_state);

            // only track the 0 -> 1 transition
            if (m_sound_reset_state == 0)
                return;

            if (m_tms5220.found())
            {
                m_tms5220.op0.reset(); // technically what happens is the tms5220 gets a long stream of 0xFF written to it when sound_reset_state is 0 which halts the chip after a few frames, but this works just as well, even if it isn't exactly true to hardware... The hardware may not have worked either, the resistors to pull input to 0xFF are fighting against the ls263 gate holding the latched value to be sent to the chip.
            }

            mixer_w(0);
        }


        uint16_t sound_r()
        {
            if (!machine().side_effects_disabled())
            {
                // clear the p2portwr state on a p1portrd
                m_p2portwr_state = false;
                update_interrupts();
            }

            // handle it normally otherwise
            return (uint16_t)(m_mainlatch.op0.read() | 0xff00);
        }


        void sound_6502_w(uint8_t data)
        {
            // clock the state through
            m_p2portwr_state = BIT(m_interrupt_enable, 1) != 0;
            update_interrupts();

            // handle it normally otherwise
            m_mainlatch.op0.write(data);
        }


        uint8_t sound_6502_r()
        {
            if (!machine().side_effects_disabled())
            {
                // clock the state through
                m_p2portrd_state = BIT(m_interrupt_enable, 0) != 0;
                update_interrupts();
            }

            // handle it normally otherwise
            return m_soundlatch.op0.read();
        }


        /*************************************
         *
         *  Speech chip
         *
         *************************************/
        void tms5220_w(uint8_t data)
        {
            if (m_tms5220.found())
            {
                m_tms5220.op0.data_w(data);
            }
        }


        void tms5220_strobe_w(offs_t offset, uint8_t data)
        {
            if (m_tms5220.found())
            {
                m_tms5220.op0.wsq_w(1 - ((int)offset & 1));
            }
        }


        /*************************************
         *
         *  Misc. sound
         *
         *************************************/
        void coincount_w(uint8_t data)
        {
            machine().bookkeeping().coin_counter_w(0, (data >> 0) & 1);
            machine().bookkeeping().coin_counter_w(1, (data >> 1) & 1);
        }


        // full memory map derived from schematics
        void main_map(address_map map, device_t owner)
        {
            map.unmap_value_high();
            map.op(0000000, 0007777).ram();
            map.op(0010000, 0010777).mirror(01000).ram().w("palette", (offs_t offset, u16 data, u16 mem_mask) => { ((palette_device)subdevice("palette")).write16(offset, data, mem_mask); }).share("palette");
            map.op(0012000, 0012000).mirror(00176).r("adc", () => { return ((adc0808_device)subdevice("adc")).data_r(); });
            map.op(0012000, 0012003).mirror(00174).w((write16sm_delegate)bankselect_w);
            map.op(0012200, 0012217).mirror(00160).w("adc", (offset, data) => { ((adc0808_device)subdevice("adc")).address_offset_start_w(offset, data); }).umask16(0x00ff);
            map.op(0012600, 0012600).mirror(00036).w(int0_ack_w);
            map.op(0012640, 0012640).mirror(00036).w(sound_reset_w);
            map.op(0012700, 0012700).mirror(00036).w(scanline_int_ack_w);
            map.op(0012740, 0012740).mirror(00036).w(video_int_ack_w);
            map.op(0013000, 0013000).mirror(00176).w(int_enable_w);
            map.op(0013200, 0013200).mirror(00176).w(m_soundlatch, (data) => { ((generic_latch_8_device)subdevice("soundlatch")).write(data); });
            map.op(0013400, 0013401).mirror(00176).w((write16s_delegate)xscroll_w).share("xscroll");
            map.op(0013600, 0013601).mirror(00176).w((write16s_delegate)yscroll_w).share("yscroll");
            map.op(0014000, 0014001).mirror(01776).r(switch_r);
            map.op(0014000, 0014000).mirror(01776).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });
            map.op(0016000, 0016001).mirror(01776).r(sound_r);
            map.op(0020000, 0037777).view(m_vmmu);
            m_vmmu.op(0).op(020000, 033777).ram().w(m_alpha_tilemap, (offs_t offset, u16 data, u16 mem_mask) => { m_alpha_tilemap.op0.write16(offset, data, mem_mask); }).share("alpha");
            m_vmmu.op(0).op(034000, 037777).ram().w((write16s_delegate)spriteram_w).share("mob");
            m_vmmu.op(2).op(020000, 037777).ram().w((write16s_delegate)playfieldt_w).share(m_playfieldt);
            m_vmmu.op(3).op(020000, 037777).ram().w((write16s_delegate)playfieldb_w).share(m_playfieldb);
            map.op(0040000, 0057777).bankr("rombank1");
            map.op(0060000, 0077777).bankr("rombank2");
            map.op(0100000, 0177777).rom();
        }


        /*************************************
         *
         *  Sound CPU memory handlers
         *
         *************************************/
        // full memory map derived from schematics
        void sound_map(address_map map, device_t owner)
        {
            map.op(0x0000, 0x0fff).mirror(0x2000).ram();
            map.op(0x1000, 0x17ff).mirror(0x2000).rw("eeprom", (address_space space, offs_t offset) => { return ((eeprom_parallel_28xx_device)subdevice("eeprom")).read(space, offset); }, (offs_t offset, u8 data) => { ((eeprom_parallel_28xx_device)subdevice("eeprom")).write(offset, data); });  //map(0x1000, 0x17ff).mirror(0x2000).rw("eeprom", FUNC(eeprom_parallel_28xx_device::read), FUNC(eeprom_parallel_28xx_device::write));
            map.op(0x1800, 0x180f).mirror(0x2780).rw(m_pokey[0], (offset) => { return m_pokey[0].op0.read(offset); }, (offs_t offset, u8 data) => { m_pokey[0].op0.write(offset, data); });  //map(0x1800, 0x180f).mirror(0x2780).rw(m_pokey[0], FUNC(pokey_device::read), FUNC(pokey_device::write));
            map.op(0x1810, 0x1813).mirror(0x278c).r(leta_r);
            map.op(0x1830, 0x183f).mirror(0x2780).rw(m_pokey[1], (offset) => { return m_pokey[1].op0.read(offset); }, (offs_t offset, u8 data) => { m_pokey[1].op0.write(offset, data); });  //map(0x1830, 0x183f).mirror(0x2780).rw(m_pokey[1], FUNC(pokey_device::read), FUNC(pokey_device::write));
            map.op(0x1840, 0x1840).mirror(0x278f).r(switch_6502_r);
            map.op(0x1850, 0x1851).mirror(0x278e).rw(m_ym2151, (offset) => { return m_ym2151.op0.read(offset); }, (offs_t offset, u8 data) => { m_ym2151.op0.write(offset, data); });  //map(0x1850, 0x1851).mirror(0x278e).rw(m_ym2151, FUNC(ym2151_device::read), FUNC(ym2151_device::write));
            map.op(0x1860, 0x1860).mirror(0x278f).r(sound_6502_r);
            map.op(0x1870, 0x1870).mirror(0x2781).w(tms5220_w);
            map.op(0x1872, 0x1873).mirror(0x2780).w(tms5220_strobe_w);
            map.op(0x1874, 0x1874).mirror(0x2781).w(sound_6502_w);
            map.op(0x1876, 0x1876).mirror(0x2781).w(coincount_w);
            map.op(0x1878, 0x1878).mirror(0x2781).w(sound_irq_ack_w);
            map.op(0x187a, 0x187a).mirror(0x2781).w(mixer_w);
            map.op(0x187c, 0x187c).mirror(0x2781).w(switch_6502_w);
            map.op(0x187e, 0x187e).mirror(0x2781).w(sndrst_6502_w);
            map.op(0x4000, 0xffff).rom();
        }
    }


    partial class atarisy2 : construct_ioport_helper
    {
        //static INPUT_PORTS_START( paperboy )
        void construct_ioport_paperboy(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            atarisy2_state atarisy2_state = (atarisy2_state)owner;

            PORT_START("1840");  /*(sound) */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_READ_LINE_DEVICE_MEMBER("soundlatch", () => { return ((generic_latch_8_device)atarisy2_state.subdevice("soundlatch")).pending_r(); }); // P1TALK
            PORT_BIT( 0x02, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_READ_LINE_DEVICE_MEMBER("mainlatch", () => { return ((generic_latch_8_device)atarisy2_state.subdevice("mainlatch")).pending_r(); }); // P2TALK
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_CUSTOM );
            PORT_BIT( 0x08, IP_ACTIVE_HIGH, IPT_UNUSED );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_SERVICE );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN3 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_COIN2 );

            PORT_START("1800");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_UNKNOWN );
            PORT_BIT( 0x10, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_READ_LINE_DEVICE_MEMBER("mainlatch", () => { return ((generic_latch_8_device)atarisy2_state.subdevice("mainlatch")).pending_r(); }); // P2TALK
            PORT_BIT( 0x20, IP_ACTIVE_HIGH, IPT_CUSTOM ); PORT_READ_LINE_DEVICE_MEMBER("soundlatch", () => { return ((generic_latch_8_device)atarisy2_state.subdevice("soundlatch")).pending_r(); }); // P1TALK
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_BUTTON2 );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_BUTTON1 );

            PORT_START("1801");
            PORT_BIT( 0x7f, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_SERVICE( 0x80, IP_ACTIVE_LOW );

            PORT_START("ADC0");
            PORT_BIT( 0xff, 0x80, IPT_AD_STICK_X ); PORT_MINMAX(0x10,0xf0); PORT_SENSITIVITY(100); PORT_KEYDELTA(10); PORT_PLAYER(1);

            PORT_START("ADC1");
            PORT_BIT( 0xff, 0x80, IPT_AD_STICK_Y ); PORT_MINMAX(0x10,0xf0); PORT_SENSITIVITY(100); PORT_KEYDELTA(10); PORT_PLAYER(1);

            PORT_START("ADC2");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("ADC3");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("LETA0");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("LETA1");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("LETA2");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("LETA3");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_START("DSW0");
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Coinage ) );      PORT_DIPLOCATION("6/7A:!8,!7");
            PORT_DIPSETTING(    0x03, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPNAME( 0x0c, 0x00, "Right Coin" );            PORT_DIPLOCATION("6/7A:!6,!5");
            PORT_DIPSETTING(    0x00, "*1" );
            PORT_DIPSETTING(    0x04, "*4" );
            PORT_DIPSETTING(    0x08, "*5" );
            PORT_DIPSETTING(    0x0c, "*6" );
            PORT_DIPNAME( 0x10, 0x00, "Left Coin" );             PORT_DIPLOCATION("6/7A:!4");
            PORT_DIPSETTING(    0x00, "*1" );
            PORT_DIPSETTING(    0x10, "*2" );
            PORT_DIPNAME( 0xe0, 0x00, "Bonus Coins" );           PORT_DIPLOCATION("6/7A:!3,!2,!1");
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );
            PORT_DIPSETTING(    0x80, "1 Each 5" );
            PORT_DIPSETTING(    0x40, "1 Each 4" );
            PORT_DIPSETTING(    0xa0, "1 Each 3" );
            PORT_DIPSETTING(    0x60, "2 Each 4" );
            PORT_DIPSETTING(    0x20, "1 Each 2" );
            PORT_DIPSETTING(    0xc0, "1 Each ?" );              // Not Documented
            PORT_DIPSETTING(    0xe0, DEF_STR( Free_Play ) );

            PORT_START("DSW1");
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Difficulty ) );   PORT_DIPLOCATION("5/6A:!8,!7");
            PORT_DIPSETTING(    0x01, DEF_STR( Easy ) );
            PORT_DIPSETTING(    0x02, DEF_STR( Medium ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Medium_Hard ) );
            PORT_DIPSETTING(    0x03, DEF_STR( Hard ) );
            PORT_DIPNAME( 0x0c, 0x00, DEF_STR( Bonus_Life ) );   PORT_DIPLOCATION("5/6A:!6,!5");
            PORT_DIPSETTING(    0x08, "10000" );
            PORT_DIPSETTING(    0x00, "15000" );
            PORT_DIPSETTING(    0x0c, "20000" );
            PORT_DIPSETTING(    0x04, DEF_STR( None ) );
            PORT_DIPNAME( 0x30, 0x00, DEF_STR( Lives ) );        PORT_DIPLOCATION("5/6A:!4,!3");
            PORT_DIPSETTING(    0x20, "3" );
            PORT_DIPSETTING(    0x00, "4" );
            PORT_DIPSETTING(    0x30, "5" );
            PORT_DIPSETTING(    0x10, "Infinite (Cheat)");
            PORT_DIPUNUSED_DIPLOC( 0x40, 0x40, "5/6A:!2" );      // Listed as "Unused"
            PORT_DIPUNUSED_DIPLOC( 0x80, 0x80, "5/6A:!1" );      // Listed as "Unused"

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( 720 )
        void construct_ioport_720(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_INCLUDE( construct_ioport_paperboy, ref errorbuf );

            PORT_MODIFY("ADC0");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED );

            PORT_MODIFY("ADC1");
            PORT_BIT( 0xff, IP_ACTIVE_LOW, IPT_UNUSED );

            /* 720 uses a special controller to control the player rotation.
             * It uses 1 disc with 72 teeth for the rotation and another disc
             * with 2 teeth for the alignment of the joystick to the top position.
             * The following graph shows how the Center and Rotate disc align.
             * The numbers show how the optical count varies from center.
             *
             *   _____2  1________1  2_____
             *        |__|        |__|          Center disc - 2 teeth.  Shown lined up with Rotate disc
             *      __    __    __    __
             *   __|  |__|  |__|  |__|  |__     Rotate disc - 72 teeth (144 positions)
             *     4  3  2  1  1  2  3  4
             */

            /* Center disc */
            /* X1, X2 LETA inputs */
            PORT_MODIFY("LETA0");    // not direct mapped
            PORT_BIT( 0xff, 0x00, IPT_DIAL_V ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_NAME("Center"); PORT_CONDITION("SELECT",0x03,ioport_condition.condition_t.EQUALS,0x00);

            /* Rotate disc */
            /* Y1, Y2 LETA inputs */
            /* The disc has 72 teeth which are read by the hardware at 2x */
            /* Computer hardware reads at 4x, so we set the sensitivity to 50% */
            PORT_MODIFY("LETA1");    // not direct mapped
            PORT_BIT( 0xff, 0x00, IPT_DIAL ); PORT_SENSITIVITY(50); PORT_KEYDELTA(10); PORT_FULL_TURN_COUNT(144); PORT_NAME("Rotate"); PORT_CONDITION("SELECT",0x03,ioport_condition.condition_t.EQUALS,0x00);

            PORT_START("FAKE_JOY_X");    // not direct mapped
            PORT_BIT( 0xff, 0x80, IPT_AD_STICK_X ); PORT_SENSITIVITY(100); PORT_KEYDELTA(10); PORT_CONDITION("SELECT",0x03,ioport_condition.condition_t.EQUALS,0x01);

            PORT_START("FAKE_JOY_Y");    // not direct mapped
            PORT_BIT( 0xff, 0x80, IPT_AD_STICK_Y ); PORT_SENSITIVITY(100); PORT_KEYDELTA(10); PORT_REVERSE(); PORT_CONDITION("SELECT",0x03,ioport_condition.condition_t.EQUALS,0x01);

            /* Let's assume we are using a 1200 count spinner.  We scale to get a 144 count.
             * 144/1200 = 0.12 = 12% */
            PORT_START("FAKE_SPINNER");  // not direct mapped
            PORT_BIT( 0xffff, 0x00, IPT_DIAL ); PORT_SENSITIVITY(12); PORT_KEYDELTA(10); PORT_CONDITION("SELECT",0x03,ioport_condition.condition_t.EQUALS,0x02);

            PORT_START("SELECT");
            PORT_CONFNAME( 0x03, 0x02, "Controller Type" );
            PORT_CONFSETTING(    0x00, "Real" );
            PORT_CONFSETTING(    0x01, "Joystick" );
            PORT_CONFSETTING(    0x02, "Spinner" );

            PORT_MODIFY("DSW1");
            PORT_DIPNAME( 0x03, 0x01, DEF_STR( Bonus_Life ) );       PORT_DIPLOCATION("5/6A:!8,!7");
            PORT_DIPSETTING(    0x01, "3000" );
            PORT_DIPSETTING(    0x00, "5000" );
            PORT_DIPSETTING(    0x02, "8000" );
            PORT_DIPSETTING(    0x03, "12000" );
            PORT_DIPNAME( 0x0c, 0x04, DEF_STR( Difficulty ) );       PORT_DIPLOCATION("5/6A:!6,!5");
            PORT_DIPSETTING(    0x04, DEF_STR( Easy ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Medium ) );
            PORT_DIPSETTING(    0x08, DEF_STR( Hard ) );
            PORT_DIPSETTING(    0x0c, DEF_STR( Hardest ) );
            PORT_DIPNAME( 0x30, 0x10, "Maximum Add. A. Coins" );     PORT_DIPLOCATION("5/6A:!4,!3");
            PORT_DIPSETTING(    0x10, "0" );
            PORT_DIPSETTING(    0x20, "1" );
            PORT_DIPSETTING(    0x00, "2" );
            PORT_DIPSETTING(    0x30, "3" );
            PORT_DIPNAME( 0xc0, 0x40, "Coins Required" );            PORT_DIPLOCATION("5/6A:!2,!1");
            PORT_DIPSETTING(    0x80, "3 To Start, 2 To Continue" );
            PORT_DIPSETTING(    0xc0, "3 To Start, 1 To Continue" );
            PORT_DIPSETTING(    0x00, "2 To Start, 1 To Continue" );
            PORT_DIPSETTING(    0x40, "1 To Start, 1 To Continue" );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( ssprint )
        //static INPUT_PORTS_START( csprint )
        //static INPUT_PORTS_START( apb )
    }


    partial class atarisy2_state : driver_device
    {
        /*************************************
         *
         *  Graphics definitions
         *
         *************************************/
        static readonly gfx_layout anlayout = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,1),
            2,
            ArrayCombineUInt32(0, 4),
            ArrayCombineUInt32(STEP4(0, 1), STEP4(8, 1)),
            ArrayCombineUInt32(STEP8(0, 8 * 2)),
            8*8*2
        );

        static readonly gfx_layout pflayout = new gfx_layout
        (
            8,8,
            RGN_FRAC(1,2),
            4,
            ArrayCombineUInt32(0, 4, RGN_FRAC(1, 2) + 0, RGN_FRAC(1, 2) + 4),
            ArrayCombineUInt32(STEP4(0, 1), STEP4(8, 1)),
            ArrayCombineUInt32(STEP8(0, 8 * 2)),
            8*8*2
        );

        static readonly gfx_layout molayout = new gfx_layout
        (
            16,16,
            RGN_FRAC(1,2),
            4,
            ArrayCombineUInt32(0, 4, RGN_FRAC(1, 2) + 0, RGN_FRAC(1, 2) + 4),
            ArrayCombineUInt32(STEP4(8 * 0, 1), STEP4(8 * 1, 1), STEP4(8 * 2, 1), STEP4(8 * 3, 1)),
            ArrayCombineUInt32(STEP16(0, 8 * 4)),
            16*16*2
        );


        //static GFXDECODE_START( gfx_atarisy2 )
        static readonly gfx_decode_entry [] gfx_atarisy2 =
        {
            GFXDECODE_ENTRY( "gfx1", 0, pflayout, 128, 8 ),
            GFXDECODE_ENTRY( "gfx2", 0, molayout,   0, 4 ),
            GFXDECODE_ENTRY( "gfx3", 0, anlayout,  64, 8 ),
            //GFXDECODE_END
        };


        //void atarisy2_state::atarisy2(machine_config &config)
        /*************************************
         *
         *  Machine driver
         *
         *************************************/
        void atarisy2(machine_config config)
        {
            // basic machine hardware
            T11(config, m_maincpu, MASTER_CLOCK / 2);
            m_maincpu.op0.set_initial_mode(0x36ff); // initial mode word has DAL15,14,11,8 pulled low
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, main_map);

            M6502(config, m_audiocpu, SOUND_CLOCK / 8);
            m_audiocpu.op0.memory().set_addrmap(AS_PROGRAM, sound_map);
            m_audiocpu.op0.execute().set_periodic_int(sound_irq_gen, attotime.from_hz(MASTER_CLOCK/2/16/16/16/10));

            adc0809_device adc = ADC0809(config, "adc", MASTER_CLOCK / 32); // 625 kHz
            adc.in_callback<u32_const_0>().set_ioport("ADC0").reg(); // J102 pin 5 (POT1)
            adc.in_callback<u32_const_1>().set_ioport("ADC1").reg(); // J102 pin 7 (POT2)
            adc.in_callback<u32_const_2>().set_ioport("ADC2").reg(); // J102 pin 9 (POT3)
            adc.in_callback<u32_const_3>().set_ioport("ADC3").reg(); // J102 pin 8 (POT4)
            // IN4 = J102 pin 6 (unused)
            // IN5 = J102 pin 4 (unused)
            // IN6 = J102 pin 2 (unused)
            // IN7 = J102 pin 3 (unused)

            EEPROM_2804(config, "eeprom");

            TIMER(config, "scantimer").configure_scanline(scanline_update, m_screen, 0, 64);

            WATCHDOG_TIMER(config, "watchdog").set_time(attotime.from_hz(MASTER_CLOCK/2/16/16/16/256));

            // video hardware
            GFXDECODE(config, "gfxdecode", "palette", gfx_atarisy2);
            PALETTE(config, "palette").set_format(2, RRRRGGGGBBBBIIII, 256);

            TILEMAP(config, m_playfield_tilemap, "gfxdecode", 2, 8,8, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 128,64).set_info_callback(get_playfield_tile_info);
            TILEMAP(config, m_alpha_tilemap, "gfxdecode", 2, 8,8, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 64,48, 0).set_info_callback(get_alpha_tile_info);

            ATARI_MOTION_OBJECTS(config, m_mob, 0, m_screen, s_mob_config);
            m_mob.op0.set_gfxdecode(m_gfxdecode);

            screen_device screen = SCREEN(config, "screen", SCREEN_TYPE_RASTER);
            screen.set_video_attributes(VIDEO_UPDATE_BEFORE_VBLANK);
            screen.set_raw(VIDEO_CLOCK / 2, 640, 0, 512, 416, 0, 384);
            screen.set_screen_update(screen_update_atarisy2);
            screen.set_palette("palette");
            screen.screen_vblank().set((write_line_delegate)vblank_int).reg();

            // sound hardware
            SPEAKER(config, "lspeaker").front_left();
            SPEAKER(config, "rspeaker").front_right();

            GENERIC_LATCH_8(config, m_soundlatch);
            m_soundlatch.op0.data_pending_callback().set_inputline(m_audiocpu, m6502_device.NMI_LINE).reg();
            m_soundlatch.op0.data_pending_callback().append(boost_interleave_hack).reg();

            GENERIC_LATCH_8(config, m_mainlatch);

            YM2151(config, m_ym2151, SOUND_CLOCK / 4);
            m_ym2151.op0.disound.add_route(0, "lspeaker", 0.60);
            m_ym2151.op0.disound.add_route(1, "rspeaker", 0.60);

            POKEY(config, m_pokey[0], SOUND_CLOCK / 8);
            m_pokey[0].op0.allpot_r().set_ioport("DSW0").reg();
            m_pokey[0].op0.disound.add_route(ALL_OUTPUTS, "lspeaker", 1.35);

            POKEY(config, m_pokey[1], SOUND_CLOCK / 8);
            m_pokey[1].op0.allpot_r().set_ioport("DSW1").reg();
            m_pokey[1].op0.disound.add_route(ALL_OUTPUTS, "rspeaker", 1.35);

            TMS5220C(config, m_tms5220, MASTER_CLOCK / 4 / 4 / 2);
            m_tms5220.op0.disound.add_route(ALL_OUTPUTS, "lspeaker", 0.75);
            m_tms5220.op0.disound.add_route(ALL_OUTPUTS, "rspeaker", 0.75);
        }


        public void paperboy(machine_config config)
        {
            atarisy2(config);
            SLAPSTIC(config, m_slapstic, 105);
            m_slapstic.op0.set_range(m_maincpu, AS_PROGRAM, 0100000, 0100777, 0);
            m_slapstic.op0.set_view(m_vmmu);
        }


        public void _720(machine_config config)
        {
            atarisy2(config);
            /* without the default EEPROM, 720 hangs at startup due to communication
               issues with the sound CPU; temporarily increasing the sound CPU frequency
               to ~2.2MHz "fixes" the problem */

            SLAPSTIC(config, m_slapstic, 107);
            m_slapstic.op0.set_range(m_maincpu, AS_PROGRAM, 0100000, 0100777, 0);
            m_slapstic.op0.set_view(m_vmmu);
        }


        //void atarisy2_state::ssprint(machine_config &config)
        //void atarisy2_state::csprint(machine_config &config)
        //void atarisy2_state::apb(machine_config &config)
    }


    partial class atarisy2 : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definition(s)
         *
         *************************************/

        //ROM_START( paperboy ) // ALL of these roms should be 136034-xxx but the correct labels aren't known per game rev!
        static readonly tiny_rom_entry [] rom_paperboy =
        {
            ROM_REGION( 0x90000, "maincpu", 0 ), // 9*64k for T11 code
            ROM_LOAD16_BYTE( "cpu_l07.rv3", 0x008000, 0x004000, CRC("4024bb9b") + SHA1("9030ce5a6a1a3d769c699a92b32a55013f9766aa") ),
            ROM_LOAD16_BYTE( "cpu_n07.rv3", 0x008001, 0x004000, CRC("0260901a") + SHA1("39d786f5c440ca1fd529ee73e2a4d2406cd1db8f") ),
            ROM_LOAD16_BYTE( "cpu_f06.rv2", 0x010000, 0x004000, CRC("3fea86ac") + SHA1("90722bfd0426efbfb69714151f8644b56075b4c1") ),
            ROM_LOAD16_BYTE( "cpu_n06.rv2", 0x010001, 0x004000, CRC("711b17ba") + SHA1("7c9b19f754f1e3ba4d081edce2a39e81ce87f6bb") ),
            ROM_LOAD16_BYTE( "cpu_j06.rv1", 0x030000, 0x004000, CRC("a754b12d") + SHA1("7b07efe70f9696041355b72f5cded7fcbd8be460") ),
            ROM_LOAD16_BYTE( "cpu_p06.rv1", 0x030001, 0x004000, CRC("89a1ff9c") + SHA1("aa947e0726bb68164b9556d57daf6547b4580ed0") ),
            ROM_LOAD16_BYTE( "cpu_k06.rv1", 0x050000, 0x004000, CRC("290bb034") + SHA1("71dfceb6a8b3b0e3be2cc907c3d4b91fe6973fec") ),
            ROM_LOAD16_BYTE( "cpu_r06.rv1", 0x050001, 0x004000, CRC("826993de") + SHA1("59c6b87bcbca80b0a6192d7bb534a0747f32b907") ),
            ROM_LOAD16_BYTE( "cpu_l06.rv2", 0x070000, 0x004000, CRC("8a754466") + SHA1("2c4c6ca797c7f4349c2893d8c0ba7e2658fdca99") ),
            ROM_LOAD16_BYTE( "cpu_s06.rv2", 0x070001, 0x004000, CRC("224209f9") + SHA1("c41269bfadb8fff1c8ff0f6ea0b8e8b34feb49d6") ),

            ROM_REGION( 0x10000, "audiocpu", 0 ),    // 64k for 6502 code
            ROM_LOAD( "cpu_a02.rv3", 0x004000, 0x004000, CRC("ba251bc4") + SHA1("768e42608263205e412e651082ffa2a083b04644") ),
            ROM_LOAD( "cpu_b02.rv2", 0x008000, 0x004000, CRC("e4e7a8b9") + SHA1("f11a0cf40d5c51ff180f0fa1cf676f95090a1010") ),
            ROM_LOAD( "cpu_c02.rv2", 0x00c000, 0x004000, CRC("d44c2aa2") + SHA1("f1b00e36d87f6d77746cf003198c7f19aa2f4fab") ),

            ROM_REGION( 0x20000, "gfx1", 0 ),
            ROM_LOAD( "vid_a06.rv1", 0x000000, 0x008000, CRC("b32ffddf") + SHA1("5b7619008e34ed7f5eb5e85e5f45c375e078086a") ),
            ROM_LOAD( "vid_b06.rv1", 0x00c000, 0x004000, CRC("301b849d") + SHA1("d608a854027da5eb88c071df1d01f31124db89a8") ),
            ROM_LOAD( "vid_c06.rv1", 0x010000, 0x008000, CRC("7bb59d68") + SHA1("fcaa8bd32448d8f951ae446eb425b608f2cecbef") ),
            ROM_LOAD( "vid_d06.rv1", 0x01c000, 0x004000, CRC("1a1d4ba8") + SHA1("603d61fd17e312d0784d883a50ce6b03aba27d10") ),

            ROM_REGION( 0x40000, "gfx2", ROMREGION_INVERT ),
            ROM_LOAD( "vid_l06.rv1", 0x000000, 0x008000, CRC("067ef202") + SHA1("519f32995a32ed96086f4ed3d49530b6917ad7d3") ),
            ROM_LOAD( "vid_k06.rv1", 0x008000, 0x008000, CRC("76b977c4") + SHA1("09988aceaf398279556980e3a21c0dc1b619fb72") ),
            ROM_LOAD( "vid_j06.rv1", 0x010000, 0x008000, CRC("2a3cc8d0") + SHA1("c0165286486a0844baf99c782d2fffdd6ad003b6") ),
            ROM_LOAD( "vid_h06.rv1", 0x018000, 0x008000, CRC("6763a321") + SHA1("15ed912f0346f6b5c3ad23ff22e7493d31ad18a7") ),
            ROM_LOAD( "vid_s06.rv1", 0x020000, 0x008000, CRC("0a321b7b") + SHA1("681317494a0bd50569bb822783336e68551cfd5e") ),
            ROM_LOAD( "vid_p06.rv1", 0x028000, 0x008000, CRC("5bd089ee") + SHA1("9ac98391a6c70d3cfbe609342294668530d690b4") ),
            ROM_LOAD( "vid_n06.rv1", 0x030000, 0x008000, CRC("c34a517d") + SHA1("f0af3db87f73c1fad00a270269ba380898ef5a4b") ),
            ROM_LOAD( "vid_m06.rv1", 0x038000, 0x008000, CRC("df723956") + SHA1("613d398f30463086c0cc720a760bda652e0f3832") ),

            ROM_REGION( 0x2000, "gfx3", 0 ),
            ROM_LOAD( "vid_t06.rv1", 0x000000, 0x002000, CRC("60d7aebb") + SHA1("ad74221c4270496ebcfedd46ea16dca2cda1b4be") ),

            ROM_REGION( 0x200, "eeprom", 0 ),
            ROM_LOAD( "paperboy-eeprom.bin", 0x0000, 0x0200, CRC("756b90cc") + SHA1("b78762e354f1316087f9de4005734c343356c8ef") ),

            ROM_END,
        };


        //ROM_START( paperboyr2 )
        //ROM_START( paperboyr1 )
        //ROM_START( paperboyp )


        //ROM_START( 720 )
        static readonly tiny_rom_entry [] rom_720 =
        {
            ROM_REGION( 0x90000, "maincpu", 0 ),     // 9 * 64k T11 code
            ROM_LOAD16_BYTE( "136047-3126.7lm", 0x008000, 0x004000, CRC("43abd367") + SHA1("bb58c42f25ef0ee5357782652e9e2b28df0ba82e") ),
            ROM_LOAD16_BYTE( "136047-3127.7mn", 0x008001, 0x004000, CRC("772e1e5b") + SHA1("1ee9b6bd7b2a5e4866b7157db95ee38b53f5c4ce") ),
            ROM_LOAD16_BYTE( "136047-3128.6fh", 0x010000, 0x010000, CRC("bf6f425b") + SHA1("22732465959c2d30383523e0354b8d3759963765") ),
            ROM_LOAD16_BYTE( "136047-4131.6mn", 0x010001, 0x010000, CRC("2ea8a20f") + SHA1("927f464e7da540221e341524581cb7bc65e1a31e") ),
            ROM_LOAD16_BYTE( "136047-1129.6hj", 0x030000, 0x010000, CRC("eabf0b01") + SHA1("aaf5ab31b63c6ba414f0d4c95bbbebcceedd1ae4") ),
            ROM_LOAD16_BYTE( "136047-1132.6p",  0x030001, 0x010000, CRC("a24f333e") + SHA1("e4bfa4c670bfb375118d5774f1dbe848e39e6460") ),
            ROM_LOAD16_BYTE( "136047-1130.6k",  0x050000, 0x010000, CRC("93fba845") + SHA1("4de5867272af63be696855f2a4dff99476b213ad") ),
            ROM_LOAD16_BYTE( "136047-1133.6r",  0x050001, 0x010000, CRC("53c177be") + SHA1("a60c81899944e0dda9886e6697edc4d9309ca8f4") ),

            ROM_REGION( 0x10000, "audiocpu", 0 ),     // 64k for 6502 code
            ROM_LOAD( "136047-2134.2a",  0x004000, 0x004000, CRC("0db4ca28") + SHA1("71c2e0eee0eee418bdd2f806bd6ce5ae1c72bf69") ),
            ROM_LOAD( "136047-1135.2b",  0x008000, 0x004000, CRC("b1f157d0") + SHA1("26355324d49baa02acb777940d7f49d074a75fe5") ),
            ROM_LOAD( "136047-2136.2cd", 0x00c000, 0x004000, CRC("00b06bec") + SHA1("cd771eea329e0f6ab5bff1035f931800cc5da545") ),

            ROM_REGION( 0x40000, "gfx1", 0 ),
            ROM_LOAD( "136047-1121.6a",  0x000000, 0x008000, CRC("7adb5f9a") + SHA1("8b4dba6c7ecd9d1c03c5d87326b5971ad1cb8863") ),
            ROM_LOAD( "136047-1122.6b",  0x008000, 0x008000, CRC("41b60141") + SHA1("a426a0a5f6d4b500571731b3ce5ce8acb5e1db92") ),
            ROM_LOAD( "136047-1123.7a",  0x010000, 0x008000, CRC("501881d5") + SHA1("f38b13774c45eb5b48c87c4410afe4bd311cf3c7") ),
            ROM_LOAD( "136047-1124.7b",  0x018000, 0x008000, CRC("096f2574") + SHA1("6b59ff9a89a93c39c18011a0ac7043457617f336") ),
            ROM_LOAD( "136047-1117.6c",  0x020000, 0x008000, CRC("5a55f149") + SHA1("9dbee28a0bc8ec0d3936d61b7359cb63f4860fff") ),
            ROM_LOAD( "136047-1118.6d",  0x028000, 0x008000, CRC("9bb2429e") + SHA1("80655839e5f53aea19115d83bf395b4f70997edc") ),
            ROM_LOAD( "136047-1119.7d",  0x030000, 0x008000, CRC("8f7b20e5") + SHA1("9f0928a442f63c66350e66b35b1503fe4f9d8e33") ),
            ROM_LOAD( "136047-1120.7c",  0x038000, 0x008000, CRC("46af6d35") + SHA1("c3c2b131245f1231839b3649c117bf5bbace0641") ),

            ROM_REGION( 0x100000, "gfx2", ROMREGION_INVERT ),
            ROM_LOAD( "136047-1109.6t",  0x020000, 0x008000, CRC("0a46b693") + SHA1("77a743816663a8b8fe6bd9aa2dd0a4e570071068") ),
            ROM_CONTINUE(                0x000000, 0x008000 ),
            ROM_LOAD( "136047-1110.6sr", 0x028000, 0x008000, CRC("457d7e38") + SHA1("9ac8e5b49e8f61cb8ce4d739462d17049c966a5d") ),
            ROM_CONTINUE(                0x008000, 0x008000 ),
            ROM_LOAD( "136047-1111.6p",  0x030000, 0x008000, CRC("ffad0a5b") + SHA1("127502a256e31c3fca92323544129ec8fcabacb8") ),
            ROM_CONTINUE(                0x010000, 0x008000 ),
            ROM_LOAD( "136047-1112.6n",  0x038000, 0x008000, CRC("06664580") + SHA1("2173536af27d9af5b506997a5bbcfd5a40e2023a") ),
            ROM_CONTINUE(                0x018000, 0x008000 ),
            ROM_LOAD( "136047-1113.6m",  0x060000, 0x008000, CRC("7445dc0f") + SHA1("cfaa535a4a81a00d0cf47ca3e89625e12abde0f5") ),
            ROM_CONTINUE(                0x040000, 0x008000 ),
            ROM_LOAD( "136047-1114.6l",  0x068000, 0x008000, CRC("23eaceb0") + SHA1("8206da45d09b03c51d5c41fdbe964fec0e399837") ),
            ROM_CONTINUE(                0x048000, 0x008000 ),
            ROM_LOAD( "136047-1115.6kj", 0x070000, 0x008000, CRC("0cc8de53") + SHA1("656fc4011e6ea362f706048a36e99ff31ecbf7cc") ),
            ROM_CONTINUE(                0x050000, 0x008000 ),
            ROM_LOAD( "136047-1116.6jh", 0x078000, 0x008000, CRC("2d8f1369") + SHA1("d35fc5f6733c83d59b0029eb6ee3945e22f0d13b") ),
            ROM_CONTINUE(                0x058000, 0x008000 ),
            ROM_LOAD( "136047-1101.5t",  0x0a0000, 0x008000, CRC("2ac77b80") + SHA1("cae6de4ef8a3cf5fb370c0178f734332369e17da") ),
            ROM_CONTINUE(                0x080000, 0x008000 ),
            ROM_LOAD( "136047-1102.5sr", 0x0a8000, 0x008000, CRC("f19c3b06") + SHA1("12e2194e5cc9604f02bad03dd6f62bba7f459e73") ),
            ROM_CONTINUE(                0x088000, 0x008000 ),
            ROM_LOAD( "136047-1103.5p",  0x0b0000, 0x008000, CRC("78f9ab90") + SHA1("c531e264edaacf61abfbdc8f15b1b47e85a4cdf0") ),
            ROM_CONTINUE(                0x090000, 0x008000 ),
            ROM_LOAD( "136047-1104.5n",  0x0b8000, 0x008000, CRC("77ce4a7f") + SHA1("5c4a6fb01bd744f17cbacc3087c4bdb5e3bfe475") ),
            ROM_CONTINUE(                0x098000, 0x008000 ),
            ROM_LOAD( "136047-1105.5m",  0x0e0000, 0x008000, CRC("bef5a025") + SHA1("5cfe82f1ef2dd95cc5fa317bd59f69c4cd69fdd2") ),
            ROM_CONTINUE(                0x0c0000, 0x008000 ),
            ROM_LOAD( "136047-1106.5l",  0x0e8000, 0x008000, CRC("92a159c8") + SHA1("bc4f06eb666967ac726b7f85719d2fcd74e3b573") ),
            ROM_CONTINUE(                0x0c8000, 0x008000 ),
            ROM_LOAD( "136047-1107.5kj", 0x0f0000, 0x008000, CRC("0a94a3ef") + SHA1("7dec8c768d0673ab3c8211f19b17674531dda308") ),
            ROM_CONTINUE(                0x0d0000, 0x008000 ),
            ROM_LOAD( "136047-1108.5jh", 0x0f8000, 0x008000, CRC("9815eda6") + SHA1("89a80c67f4b3426e7516cd1179d5712779ef5db7") ),
            ROM_CONTINUE(                0x0d8000, 0x008000 ),

            ROM_REGION( 0x4000, "gfx3", 0 ),
            ROM_LOAD( "136047-1125.4t",  0x000000, 0x004000, CRC("6b7e2328") + SHA1("cc9a315ccafe7228951b7c32cf3b31caa89ae7d3") ),

            ROM_REGION( 0x200, "eeprom", 0 ),
            ROM_LOAD( "720-eeprom.bin", 0x0000, 0x0200, CRC("cfe1c24e") + SHA1("5f7623b0a2ff0d99ffa8e6420a5bc03e0c55250d") ),
            ROM_END,
        };


        //ROM_START( 720r3 )
        //ROM_START( 720r2 )
        //ROM_START( 720r1 )
        //ROM_START( 720g )
        //ROM_START( 720gr1 )
        //ROM_START( ssprint )
        //ROM_START( ssprints )
        //ROM_START( ssprintf )
        //ROM_START( ssprintg )
        //ROM_START( ssprint3 )
        //ROM_START( ssprintg1 )
        //ROM_START( ssprint1 )
        //ROM_START( csprints )
        //ROM_START( csprint )
        //ROM_START( csprints1 )
        //ROM_START( csprintf )
        //ROM_START( csprintg )
        //ROM_START( csprint2 )
        //ROM_START( csprintg1 )
        //ROM_START( csprint1 )
        //ROM_START( apb )
        //ROM_START( apb6 )
        //ROM_START( apb5 )
        //ROM_START( apb4 )
        //ROM_START( apb3 )
        //ROM_START( apb2 )
        //ROM_START( apb1 )
        //ROM_START( apbg )
        //ROM_START( apbf )
    }


    partial class atarisy2_state : driver_device
    {
        /*************************************
         *
         *  Driver initialization
         *
         *************************************/

        public void init_paperboy()
        {
            MemoryU8 cpu1 = memregion("maincpu").base_();  //uint8_t *cpu1 = memregion("maincpu")->base();

            // expand the 16k program ROMs into full 64k chunks
            for (int i = 0x10000; i < 0x90000; i += 0x20000)
            {
                std.memcpy(new PointerU8(cpu1, i + 0x08000), new PointerU8(cpu1, i), 0x8000);  //memcpy(&cpu1[i + 0x08000], &cpu1[i], 0x8000);
                std.memcpy(new PointerU8(cpu1, i + 0x10000), new PointerU8(cpu1, i), 0x8000);  //memcpy(&cpu1[i + 0x10000], &cpu1[i], 0x8000);
                std.memcpy(new PointerU8(cpu1, i + 0x18000), new PointerU8(cpu1, i), 0x8000);  //memcpy(&cpu1[i + 0x18000], &cpu1[i], 0x8000);
            }

            m_pedal_count = 0;
            m_tms5220.op0.rsq_w(1); // /RS is tied high on sys2 hw
        }


        public void init_720()
        {
            m_pedal_count = -1;
            m_tms5220.op0.rsq_w(1); // /RS is tied high on sys2 hw
        }


        //void atarisy2_state::init_ssprint()
        //void atarisy2_state::init_csprint()
        //void atarisy2_state::init_apb()
    }


    /*************************************
     *
     *  Game driver(s)
     *
     *************************************/
    partial class atarisy2 : construct_ioport_helper
    {
        static atarisy2 m_atarisy2 = new atarisy2();

        static device_t device_creator_paperboy(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new atarisy2_state(mconfig, type, tag); }
        static void atarisy2_state_paperboy(machine_config config, device_t device) { ((atarisy2_state)device).paperboy(config); }
        static void atarisy2_state_init_paperboy(device_t owner) { ((atarisy2_state)owner).init_paperboy(); }

        //                                                         creator,                 rom           YEAR,   NAME,       PARENT, MACHINE,                 INPUT,                                INIT,                         MONITOR,COMPANY,       FULLNAME,           FLAGS
        public static readonly game_driver driver_paperboy = GAME( device_creator_paperboy, rom_paperboy, "1984", "paperboy", "0",    atarisy2_state_paperboy, m_atarisy2.construct_ioport_paperboy, atarisy2_state_init_paperboy, ROT0,   "Atari Games", "Paperboy (rev 3)", MACHINE_SUPPORTS_SAVE );


        static device_t device_creator_720(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new atarisy2_state(mconfig, type, tag); }
        static void atarisy2_state__720(machine_config config, device_t device) { ((atarisy2_state)device)._720(config); }
        static void atarisy2_state_init_720(device_t owner) { ((atarisy2_state)owner).init_720(); }

        //                                                    creator,            rom      YEAR,   NAME,  PARENT, MACHINE,             INPUT,                           INIT,                    MONITOR,COMPANY,       FULLNAME,              FLAGS
        public static readonly game_driver driver_720 = GAME( device_creator_720, rom_720, "1986", "720", "0",    atarisy2_state__720, m_atarisy2.construct_ioport_720, atarisy2_state_init_720, ROT0,   "Atari Games", "720 Degrees (rev 4)", MACHINE_SUPPORTS_SAVE );


        //GAME( 1986, ssprint,    0,        ssprint,  ssprint,  atarisy2_state, init_ssprint,  ROT0,   "Atari Games", "Super Sprint (rev 4)", MACHINE_SUPPORTS_SAVE )
        //GAME( 1986, csprint,    0,        csprint,  csprint,  atarisy2_state, init_csprint,  ROT0,   "Atari Games", "Championship Sprint (rev 3)", MACHINE_SUPPORTS_SAVE )
        //GAME( 1987, apb,        0,        apb,      apb,      atarisy2_state, init_apb,      ROT270, "Atari Games", "APB - All Points Bulletin (rev 7)", MACHINE_SUPPORTS_SAVE )
    }
}
