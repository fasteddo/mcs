// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame._74181_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.exidy_global;
using static mame.gamedrv_global;
using static mame.hash_global;
using static mame.nvram_global;
using static mame.romentry_global;
using static mame.s14001a_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.z80_global;


namespace mame
{
    partial class berzerk_state : driver_device
    {
        required_device<z80_device> m_maincpu;
        required_device<s14001a_device> m_s14001a;
        required_device<ttl74181_device> m_ls181_10c;
        required_device<ttl74181_device> m_ls181_12c;
        required_device<exidy_sound_device> m_custom;
        required_device<screen_device> m_screen;

        required_shared_ptr<uint8_t> m_videoram;
        required_shared_ptr<uint8_t> m_colorram;

        output_finder<u32_const_1> m_led;

        //uint8_t m_magicram_control = 0;
        //uint8_t m_last_shift_data = 0;
        //uint8_t m_intercept = 0;
        //emu_timer *m_irq_timer = nullptr;
        //emu_timer *m_nmi_timer = nullptr;
        //uint8_t m_irq_enabled = 0;
        //uint8_t m_nmi_enabled = 0;
        //int m_p1_counter_74ls161 = 0;
        //int m_p1_direction = 0;
        //int m_p2_counter_74ls161 = 0;
        //int m_p2_direction = 0;


        public berzerk_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_maincpu = new required_device<z80_device>(this, "maincpu");
            m_s14001a = new required_device<s14001a_device>(this, "speech");
            m_ls181_10c = new required_device<ttl74181_device>(this, "ls181_10c");
            m_ls181_12c = new required_device<ttl74181_device>(this, "ls181_12c");
            m_custom = new required_device<exidy_sound_device>(this, "exidy");
            m_screen = new required_device<screen_device>(this, "screen");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_colorram = new required_shared_ptr<uint8_t>(this, "colorram");
            m_led = new output_finder<u32_const_1>(this, "led{0}", 0);
        }


        static readonly XTAL MASTER_CLOCK   = new XTAL(10_000_000);
        static readonly XTAL MAIN_CPU_CLOCK = MASTER_CLOCK / 4;
        static readonly XTAL PIXEL_CLOCK    = MASTER_CLOCK / 2;
        static readonly XTAL S14001_CLOCK   = MASTER_CLOCK / 4;
        const u16 HTOTAL                    = 0x140;
        const u16 HBEND                     = 0x000;
        const u16 HBSTART                   = 0x100;
        const u16 VTOTAL                    = 0x106;
        const u16 VBEND                     = 0x020;
        const u16 VBSTART                   = 0x100;
        //#define VCOUNTER_START_NO_VBLANK    (0x020)
        //#define VCOUNTER_START_VBLANK       (0x0da)
        //#define IRQS_PER_FRAME              (2)
        //#define NMIS_PER_FRAME              (8)

        //static const uint8_t irq_trigger_counts[IRQS_PER_FRAME] = { 0x80, 0xda };
        //static const uint8_t irq_trigger_v256s [IRQS_PER_FRAME] = { 0x00, 0x01 };

        //static const uint8_t nmi_trigger_counts[NMIS_PER_FRAME] = { 0x30, 0x50, 0x70, 0x90, 0xb0, 0xd0, 0xf0, 0xf0 };
        //static const uint8_t nmi_trigger_v256s [NMIS_PER_FRAME] = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };


        /*************************************
         *
         *  LED handling
         *
         *************************************/
#if false
        uint8_t berzerk_state::led_on_r()
        {
            m_led = 1;

            return 0;
        }


        void berzerk_state::led_on_w(uint8_t data)
        {
            m_led = 1;
        }


        uint8_t berzerk_state::led_off_r()
        {
            m_led = 0;

            return 0;
        }


        void berzerk_state::led_off_w(uint8_t data)
        {
            m_led = 0;
        }



        /*************************************
         *
         *  Convert to/from our line counting
         *  to the hardware's vsync chain
         *
         *************************************/

        void berzerk_state::vpos_to_vsync_chain_counter(int vpos, uint8_t *counter, uint8_t *v256)
        {
            /* convert from a vertical position to the actual values on the vertical sync counters */
            *v256 = ((vpos < VBEND) || (vpos >= VBSTART));

            if (*v256)
            {
                int temp = vpos - VBSTART + VCOUNTER_START_VBLANK;

                if (temp < 0)
                    *counter = temp + VTOTAL;
                else
                    *counter = temp;
            }
            else
                *counter = vpos;
        }


        int berzerk_state::vsync_chain_counter_to_vpos(uint8_t counter, uint8_t v256)
        {
            /* convert from the vertical sync counters to an actual vertical position */
            int vpos;

            if (v256)
            {
                vpos = counter - VCOUNTER_START_VBLANK + VBSTART;

                if (vpos >= VTOTAL)
                    vpos = vpos - VTOTAL;
            }
            else
                vpos = counter;

            return vpos;
        }



        /*************************************
         *
         *  IRQ generation
         *
         *  There are two IRQ's per frame
         *
         *************************************/

        void berzerk_state::irq_enable_w(uint8_t data)
        {
            m_irq_enabled = data & 0x01;
        }


        TIMER_CALLBACK_MEMBER(berzerk_state::irq_callback)
        {
            int irq_number = param;
            uint8_t next_counter;
            uint8_t next_v256;
            int next_vpos;
            int next_irq_number;

            /* set the IRQ line if enabled */
            if (m_irq_enabled)
                m_maincpu->set_input_line_and_vector(0, HOLD_LINE, 0xfc); // Z80

            /* set up for next interrupt */
            next_irq_number = (irq_number + 1) % IRQS_PER_FRAME;
            next_counter = irq_trigger_counts[next_irq_number];
            next_v256 = irq_trigger_v256s[next_irq_number];

            next_vpos = vsync_chain_counter_to_vpos(next_counter, next_v256);
            m_irq_timer->adjust(m_screen->time_until_pos(next_vpos), next_irq_number);
        }


        void berzerk_state::create_irq_timer()
        {
            m_irq_timer = machine().scheduler().timer_alloc(timer_expired_delegate(FUNC(berzerk_state::irq_callback),this));
        }


        void berzerk_state::start_irq_timer()
        {
            int vpos = vsync_chain_counter_to_vpos(irq_trigger_counts[0], irq_trigger_v256s[0]);
            m_irq_timer->adjust(m_screen->time_until_pos(vpos));
        }



        /*************************************
         *
         *  NMI generation
         *
         *  An NMI is asserted roughly every
         *  32 scanlines when V16 clocks HI.
         *  The NMI is cleared 2 pixels later.
         *  Since this happens so quickly, I am
         *  not emulating it, just pulse
         *  the line instead.
         *
         *************************************/

        void berzerk_state::nmi_enable_w(uint8_t data)
        {
            m_nmi_enabled = 1;
        }


        void berzerk_state::nmi_disable_w(uint8_t data)
        {
            m_nmi_enabled = 0;
        }


        uint8_t berzerk_state::nmi_enable_r()
        {
            m_nmi_enabled = 1;

            return 0;
        }


        uint8_t berzerk_state::nmi_disable_r()
        {
            m_nmi_enabled = 0;

            return 0;
        }


        TIMER_CALLBACK_MEMBER(berzerk_state::nmi_callback)
        {
            int nmi_number = param;
            uint8_t next_counter;
            uint8_t next_v256;
            int next_vpos;
            int next_nmi_number;

            /* pulse the NMI line if enabled */
            if (m_nmi_enabled)
                m_maincpu->pulse_input_line(INPUT_LINE_NMI, attotime::zero);

            /* set up for next interrupt */
            next_nmi_number = (nmi_number + 1) % NMIS_PER_FRAME;
            next_counter = nmi_trigger_counts[next_nmi_number];
            next_v256 = nmi_trigger_v256s[next_nmi_number];

            next_vpos = vsync_chain_counter_to_vpos(next_counter, next_v256);
            m_nmi_timer->adjust(m_screen->time_until_pos(next_vpos), next_nmi_number);
        }


        void berzerk_state::create_nmi_timer()
        {
            m_nmi_timer = machine().scheduler().timer_alloc(timer_expired_delegate(FUNC(berzerk_state::nmi_callback),this));
        }


        void berzerk_state::start_nmi_timer()
        {
            int vpos = vsync_chain_counter_to_vpos(nmi_trigger_counts[0], nmi_trigger_v256s[0]);
            m_nmi_timer->adjust(m_screen->time_until_pos(vpos));
        }
#endif


        /*************************************
         *
         *  Machine setup
         *
         *************************************/
        protected override void machine_start()
        {
            throw new emu_unimplemented();
#if false
            create_irq_timer();
            create_nmi_timer();

            m_led.resolve();

            /* register for state saving */
            save_item(NAME(m_magicram_control));
            save_item(NAME(m_last_shift_data));
            save_item(NAME(m_intercept));
            save_item(NAME(m_irq_enabled));
            save_item(NAME(m_nmi_enabled));
#endif
        }


        /*************************************
         *
         *  Machine reset
         *
         *************************************/
        protected override void machine_reset()
        {
            throw new emu_unimplemented();
#if false
            m_irq_enabled = 0;
            m_nmi_enabled = 0;
            m_led = 0;
            m_magicram_control = 0;

            start_irq_timer();
            start_nmi_timer();
#endif
        }


        /*************************************
         *
         *  Video system
         *
         *************************************/
        protected override void video_start()
        {
            throw new emu_unimplemented();
#if false
            m_ls181_10c->mode_w(1);
            m_ls181_12c->mode_w(1);
#endif
        }


#if false
        void berzerk_state::magicram_w(offs_t offset, uint8_t data)
        {
            uint8_t alu_output;

            uint8_t current_video_data = m_videoram[offset];

            /* shift data towards LSB.  MSB bits are filled by data from last_shift_data.
               The shifter consists of 5 74153 devices @ 7A, 8A, 9A, 10A and 11A,
               followed by 4 more 153's at 11B, 10B, 9B and 8B, which optionally
               reverse the order of the resulting bits */
            uint8_t shift_flop_output = (((uint16_t)m_last_shift_data << 8) | data) >> (m_magicram_control & 0x07);

            if (m_magicram_control & 0x08)
                shift_flop_output = bitswap<8>(shift_flop_output, 0, 1, 2, 3, 4, 5, 6, 7);

            /* collision detection - AND gate output goes to the K pin of the flip-flop,
               while J is LO, therefore, it only resets, never sets */
            if (shift_flop_output & current_video_data)
                m_intercept = 0;

            /* perform ALU step */
            m_ls181_12c->input_a_w(shift_flop_output >> 0);
            m_ls181_10c->input_a_w(shift_flop_output >> 4);
            m_ls181_12c->input_b_w(current_video_data >> 0);
            m_ls181_10c->input_b_w(current_video_data >> 4);
            m_ls181_12c->select_w(m_magicram_control >> 4);
            m_ls181_10c->select_w(m_magicram_control >> 4);

            alu_output = m_ls181_10c->function_r() << 4 | m_ls181_12c->function_r();

            m_videoram[offset] = alu_output ^ 0xff;

            /* save data for next time */
            m_last_shift_data = data & 0x7f;
        }


        void berzerk_state::magicram_control_w(uint8_t data)
        {
            /* save the control byte, clear the shift data latch,
               and set the intercept flip-flop */
            m_magicram_control = data;
            m_last_shift_data = 0;
            m_intercept = 1;
        }


        uint8_t berzerk_state::intercept_v256_r()
        {
            uint8_t counter;
            uint8_t v256;

            vpos_to_vsync_chain_counter(m_screen->vpos(), &counter, &v256);

            return (m_intercept^1) << 7 | v256;
        }


        void berzerk_state::get_pens(rgb_t *pens)
        {
            static const int resistances_wg[] = { 750, 0 };
            static const int resistances_el[] = { static_cast<int>(1.0 / ((1.0 / 750.0) + (1.0 / 360.0))), 0 };

            double color_weights[2];

            if (ioport("MONITOR_TYPE")->read() == 0)
                compute_resistor_weights(0, 0xff, -1.0,
                                            2, resistances_wg, color_weights, 0, 270,
                                            2, resistances_wg, color_weights, 0, 270,
                                            2, resistances_wg, color_weights, 0, 270);
            else
                compute_resistor_weights(0, 0xff, -1.0,
                                            2, resistances_el, color_weights, 0, 270,
                                            2, resistances_el, color_weights, 0, 270,
                                            2, resistances_el, color_weights, 0, 270);

            for (int color = 0; color < 0x10; color++)
            {
                uint8_t r_bit = (color >> 0) & 0x01;
                uint8_t g_bit = (color >> 1) & 0x01;
                uint8_t b_bit = (color >> 2) & 0x01;
                uint8_t i_bit = (color >> 3) & 0x01;

                uint8_t r = combine_weights(color_weights, r_bit & i_bit, r_bit);
                uint8_t g = combine_weights(color_weights, g_bit & i_bit, g_bit);
                uint8_t b = combine_weights(color_weights, b_bit & i_bit, b_bit);

                pens[color] = rgb_t(r, g, b);
            }
        }
#endif


        uint32_t screen_update(screen_device screen, bitmap_rgb32 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
#if false
            rgb_t pens[0x10];
            get_pens(pens);

            for (int offs = 0; offs < m_videoram.bytes(); offs++)
            {
                uint8_t data = m_videoram[offs];
                uint8_t color = m_colorram[((offs >> 2) & 0x07e0) | (offs & 0x001f)];

                uint8_t y = offs >> 5;
                uint8_t x = offs << 3;

                int i;

                for (i = 0; i < 4; i++)
                {
                    rgb_t pen = (data & 0x80) ? pens[color >> 4] : rgb_t::black();
                    bitmap.pix(y, x) = pen;

                    x++;
                    data <<= 1;
                }

                for (; i < 8; i++)
                {
                    rgb_t pen = (data & 0x80) ? pens[color & 0x0f] : rgb_t::black();
                    bitmap.pix(y, x) = pen;

                    x++;
                    data <<= 1;
                }
            }

            return 0;
#endif
        }


#if false
        /*************************************
         *
         *  Audio system
         *
         *************************************/

        void berzerk_state::audio_w(offs_t offset, uint8_t data)
        {
            switch (offset)
            {
            /* offset 4 writes to the S14001A */
            case 4:
                switch (data >> 6)
                {
                /* write data to the S14001 */
                case 0:
                    m_s14001a->data_w(data & 0x3f);

                    /* clock the chip -- via a 555 timer */
                    m_s14001a->start_w(1);
                    m_s14001a->start_w(0);

                    break;

                case 1:
                {
                    /* volume */
                    m_s14001a->force_update();
                    m_s14001a->set_output_gain(0, ((data >> 3 & 0xf) + 1) / 16.0);

                    /* clock control - the first LS161 divides the clock by 9 to 16, the 2nd by 8,
                       giving a final clock from 19.5kHz to 34.7kHz */
                    int clock_divisor = 16 - (data & 0x07);
                    m_s14001a->set_clock(S14001_CLOCK / clock_divisor / 8);
                    break;
                }

                default: break; /* 2 and 3 are not connected */
                }

                break;

            /* offset 6 writes to the sfxcontrol latch */
            case 6:
                m_custom->sfxctrl_w(data >> 6, data);
                break;

            /* everything else writes to the 6840 */
            default:
                m_custom->sh6840_w(offset, data);
                break;
            }
        }


        uint8_t berzerk_state::audio_r(offs_t offset)
        {
            switch (offset)
            {
            /* offset 4 reads from the S14001A */
            case 4:
                return (m_s14001a->busy_r()) ? 0xc0 : 0x40;
            /* offset 6 is open bus */
            case 6:
                logerror("attempted read from berzerk audio reg 6 (sfxctrl)!\n");
                return 0;
            /* everything else reads from the 6840 */
            default:
                return m_custom->sh6840_r(offset);
            }
        }
#endif


        protected override void sound_reset()
        {
            throw new emu_unimplemented();
#if false
            /* clears the flip-flop controlling the volume and freq on the speech chip */
            audio_w(4, 0x40);
#endif
        }


        /*************************************
         *
         *  Memory handlers
         *
         *************************************/

        void berzerk_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map(0x0000, 0x07ff).rom();
            map(0x0800, 0x0bff).mirror(0x0400).ram().share("nvram");
            map(0x1000, 0x3fff).rom();
            map(0x4000, 0x5fff).ram().share("videoram");
            map(0x6000, 0x7fff).ram().w(FUNC(berzerk_state::magicram_w)).share("videoram");
            map(0x8000, 0x87ff).mirror(0x3800).ram().share("colorram");
            map(0xc000, 0xffff).noprw();
#endif
        }


        //void berzerk_state::frenzy_map(address_map &map)


        /*************************************
         *
         *  Port handlers
         *
         *************************************/

        void berzerk_io_map(address_map map, device_t device)
        {
            throw new emu_unimplemented();
#if false
            map.global_mask(0xff);
            map(0x00, 0x3f).noprw();
            map(0x40, 0x47).rw(FUNC(berzerk_state::audio_r), FUNC(berzerk_state::audio_w));
            map(0x48, 0x48).portr("P1").nopw();
            map(0x49, 0x49).portr("SYSTEM").nopw();
            map(0x4a, 0x4a).portr("P2").nopw();
            map(0x4b, 0x4b).nopr().w(FUNC(berzerk_state::magicram_control_w));
            map(0x4c, 0x4c).rw(FUNC(berzerk_state::nmi_enable_r), FUNC(berzerk_state::nmi_enable_w));
            map(0x4d, 0x4d).rw(FUNC(berzerk_state::nmi_disable_r), FUNC(berzerk_state::nmi_disable_w));
            map(0x4e, 0x4e).r(FUNC(berzerk_state::intercept_v256_r)).nopw(); // note reading from here should clear pending frame interrupts, see zfb-1.tiff 74ls74 at 3D pin 13 /CLR
            map(0x4f, 0x4f).nopr().w(FUNC(berzerk_state::irq_enable_w));
            map(0x50, 0x57).noprw(); /* second sound board, initialized but not used */
            map(0x58, 0x5f).noprw();
            map(0x60, 0x60).mirror(0x18).portr("F3").nopw();
            map(0x61, 0x61).mirror(0x18).portr("F2").nopw();
            map(0x62, 0x62).mirror(0x18).portr("F6").nopw();
            map(0x63, 0x63).mirror(0x18).portr("F5").nopw();
            map(0x64, 0x64).mirror(0x18).portr("F4").nopw();
            map(0x65, 0x65).mirror(0x18).portr("SW2").nopw();
            map(0x66, 0x66).mirror(0x18).rw(FUNC(berzerk_state::led_off_r), FUNC(berzerk_state::led_off_w));
            map(0x67, 0x67).mirror(0x18).rw(FUNC(berzerk_state::led_on_r), FUNC(berzerk_state::led_on_w));
            map(0x80, 0xff).noprw();
#endif
        }
    }


    public partial class berzerk : construct_ioport_helper
    {
        /*************************************
         *
         *  Port definitions
         *
         *************************************/

        //#define BERZERK_COINAGE(CHUTE, DIPBANK) \
        //    PORT_DIPNAME( 0x0f, 0x00, "Coin "#CHUTE )  PORT_DIPLOCATION(#DIPBANK":1,2,3,4") \
        //    PORT_DIPSETTING(    0x09, DEF_STR( 2C_1C ) ) \
        //    PORT_DIPSETTING(    0x0d, DEF_STR( 4C_3C ) ) \
        //    PORT_DIPSETTING(    0x00, DEF_STR( 1C_1C ) ) \
        //    PORT_DIPSETTING(    0x0e, DEF_STR( 4C_5C ) ) \
        //    PORT_DIPSETTING(    0x0a, DEF_STR( 2C_3C ) ) \
        //    PORT_DIPSETTING(    0x0f, DEF_STR( 4C_7C ) ) \
        //    PORT_DIPSETTING(    0x01, DEF_STR( 1C_2C ) ) \
        //    PORT_DIPSETTING(    0x0b, DEF_STR( 2C_5C ) ) \
        //    PORT_DIPSETTING(    0x02, DEF_STR( 1C_3C ) ) \
        //    PORT_DIPSETTING(    0x0c, DEF_STR( 2C_7C ) ) \
        //    PORT_DIPSETTING(    0x03, DEF_STR( 1C_4C ) ) \
        //    PORT_DIPSETTING(    0x04, DEF_STR( 1C_5C ) ) \
        //    PORT_DIPSETTING(    0x05, DEF_STR( 1C_6C ) ) \
        //    PORT_DIPSETTING(    0x06, DEF_STR( 1C_7C ) ) \
        //    PORT_DIPSETTING(    0x07, "1 Coin/10 Credits" ) \
        //    PORT_DIPSETTING(    0x08, "1 Coin/14 Credits" )


        //static INPUT_PORTS_START( joystick ) // used on all games except moonwarp
        void construct_ioport_joystick(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("P1")
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ) PORT_8WAY
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ) PORT_8WAY
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ) PORT_8WAY
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ) PORT_8WAY
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 )
            PORT_BIT( 0xe0, IP_ACTIVE_LOW, IPT_UNUSED )

            PORT_START("P2")
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ) PORT_8WAY PORT_COCKTAIL
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON1 ) PORT_COCKTAIL
            PORT_BIT( 0x60, IP_ACTIVE_LOW, IPT_UNUSED )
            PORT_DIPNAME( 0x80, 0x80, DEF_STR( Cabinet ) )
            PORT_DIPSETTING(    0x80, DEF_STR( Upright ) )
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) )

            INPUT_PORTS_END
#endif
        }


        //static INPUT_PORTS_START( common ) // used on all games
        void construct_ioport_common(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("SYSTEM")
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_START1 )
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_START2 )
            PORT_BIT( 0x1c, IP_ACTIVE_LOW, IPT_UNUSED )
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN3 )
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_COIN2 )
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_COIN1 )

            /* fake port for monitor type */
            PORT_START("MONITOR_TYPE")
            PORT_CONFNAME( 0x01, 0x00, "Monitor Type" )
            PORT_CONFSETTING(    0x00, "Wells-Gardner" )
            PORT_CONFSETTING(    0x01, "Electrohome" )
            PORT_BIT( 0xfe, IP_ACTIVE_HIGH, IPT_UNUSED )

            PORT_START("SW2")
            /* port for the 'bookkeeping reset' and 'bookkeeping' buttons;
             * The 'bookkeeping reset' button is an actual button on the zpu-1000 and
             * zpu-1001 pcbs, labeled 'S2' or 'SW2'. It is wired to bit 0.
             * * pressing it while high scores are displayed will give a free game
             *   without adding any coin info to the bookkeeping info in nvram.
             * The 'bookkeeping' button is wired to the control panel, usually hidden
             * underneath or only accessible through the coin door. Wired to bit 7.
             * * It displays various bookkeeping statistics when pressed sequentially.
             *   Pressing P1 fire (according to the manual) when stats are displayed
             *   will clear the stat shown on screen.
             */
            PORT_BIT( 0x01, IP_ACTIVE_HIGH, IPT_SERVICE1 ) PORT_NAME("Free Game (not logged in bookkeeping)")
            PORT_BIT( 0x7e, IP_ACTIVE_LOW,  IPT_UNUSED )
            PORT_BIT( 0x80, IP_ACTIVE_HIGH, IPT_SERVICE2 ) PORT_NAME("Bookkeeping") PORT_CODE(KEYCODE_F1)

            INPUT_PORTS_END
#endif
        }


        //static INPUT_PORTS_START( berzerk )
        void construct_ioport_berzerk(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            throw new emu_unimplemented();
#if false
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_INCLUDE( joystick )
            PORT_INCLUDE( common )

            PORT_START("F2")
            PORT_DIPNAME( 0x03, 0x00, "Color Test" ) PORT_CODE(KEYCODE_F5) PORT_TOGGLE PORT_DIPLOCATION("F2:1,2")
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x03, DEF_STR( On ) )
            PORT_BIT( 0x3c, IP_ACTIVE_LOW,  IPT_UNUSED )
            PORT_DIPNAME( 0xc0, 0xc0, DEF_STR( Bonus_Life ) ) PORT_DIPLOCATION("F2:7,8")
            PORT_DIPSETTING(    0xc0, "5000 and 10000" )
            PORT_DIPSETTING(    0x40, "5000" )
            PORT_DIPSETTING(    0x80, "10000" )
            PORT_DIPSETTING(    0x00, DEF_STR( None ) )

            PORT_START("F3")
            PORT_DIPNAME( 0x01, 0x00, "Input Test Mode" ) PORT_CODE(KEYCODE_F2) PORT_TOGGLE PORT_DIPLOCATION("F3:1")
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x01, DEF_STR( On ) )
            PORT_DIPNAME( 0x02, 0x00, "Crosshair Pattern" ) PORT_CODE(KEYCODE_F4) PORT_TOGGLE PORT_DIPLOCATION("F3:2")
            PORT_DIPSETTING(    0x00, DEF_STR( Off ) )
            PORT_DIPSETTING(    0x02, DEF_STR( On ) )
            PORT_BIT( 0x3c, IP_ACTIVE_LOW,  IPT_UNUSED )
            PORT_DIPNAME( 0xc0, 0x00, DEF_STR( Language ) ) PORT_DIPLOCATION("F3:7,8")
            PORT_DIPSETTING(    0x00, DEF_STR( English ) )
            PORT_DIPSETTING(    0x40, DEF_STR( German ) )
            PORT_DIPSETTING(    0x80, DEF_STR( French ) )
            PORT_DIPSETTING(    0xc0, DEF_STR( Spanish ) )

            PORT_START("F4")
            BERZERK_COINAGE(1, F4)
            PORT_BIT( 0xf0, IP_ACTIVE_LOW,  IPT_UNUSED )

            PORT_START("F5")
            BERZERK_COINAGE(2, F5)
            PORT_BIT( 0xf0, IP_ACTIVE_LOW,  IPT_UNUSED )

            PORT_START("F6")
            BERZERK_COINAGE(3, F6)
            PORT_BIT( 0xf0, IP_ACTIVE_LOW,  IPT_UNUSED )

            INPUT_PORTS_END
#endif
        }


        //static INPUT_PORTS_START( berzerkf )

        //static INPUT_PORTS_START( berzerkg )

        //static INPUT_PORTS_START( berzerks )

        //static INPUT_PORTS_START( frenzy )

        //uint8_t berzerk_state::moonwarp_p1_r()

        //uint8_t berzerk_state::moonwarp_p2_r()

        //static INPUT_PORTS_START( moonwarp )
    }


    partial class berzerk_state : driver_device
    {
        /*************************************
         *
         *  Machine drivers
         *
         *************************************/

        public void berzerk(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, MAIN_CPU_CLOCK);
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, berzerk_map);
            m_maincpu.op0.memory().set_addrmap(AS_IO, berzerk_io_map);

            NVRAM(config, "nvram", nvram_device.default_value.DEFAULT_ALL_0);

            TTL74181(config, m_ls181_10c);
            TTL74181(config, m_ls181_12c);

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(PIXEL_CLOCK, HTOTAL, HBEND, HBSTART, VTOTAL, VBEND, VBSTART);
            m_screen.op0.set_screen_update(screen_update);

            /* audio hardware */
            SPEAKER(config, "mono").front_center();

            S14001A(config, m_s14001a, S14001_CLOCK / 16 / 8).disound.add_route(ALL_OUTPUTS, "mono", 1.00); /* placeholder - the clock is software controllable */
            EXIDY(config, m_custom, 0).disound.add_route(ALL_OUTPUTS, "mono", 0.33);
        }


        //void berzerk_state::frenzy(machine_config &config)
    }


    partial class berzerk : construct_ioport_helper
    {
        /*************************************
         *
         *  ROM definitions
         *
         *************************************/

        //ROM_START( berzerk ) /* All ROMs except 5C were white labels and revision RC31, 5C had a yellow label and is revision RC31A */
        static readonly tiny_rom_entry [] rom_berzerk =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),
            ROM_LOAD( "berzerk_rc31_1c.rom0.1c",  0x0000, 0x0800, CRC("ca566dbc") + SHA1("fae2647f12f1cd82826db61b53b116a5e0c9f995") ),
            ROM_LOAD( "berzerk_rc31_1d.rom1.1d",  0x1000, 0x0800, CRC("7ba69fde") + SHA1("69af170c4a39a3494dcd180737e5c87b455f9203") ),
            ROM_LOAD( "berzerk_rc31_3d.rom2.3d",  0x1800, 0x0800, CRC("a1d5248b") + SHA1("a0b7842f6a5f86c16d80d78e7012c78b3ea11d1d") ),
            ROM_LOAD( "berzerk_rc31_5d.rom3.5d",  0x2000, 0x0800, CRC("fcaefa95") + SHA1("07f849aa39f1e3db938187ffde4a46a588156ddc") ),
            ROM_LOAD( "berzerk_rc31_6d.rom4.6d",  0x2800, 0x0800, CRC("1e35b9a0") + SHA1("5a5e549ec0e4803ab2d1eac6b3e7171aedf28244") ),
            ROM_LOAD( "berzerk_rc31a_5c.rom5.5c", 0x3000, 0x0800, CRC("e0fab8f5") + SHA1("31acef9583546671debe768e3d5c695ba1b9f7e0") ),
            ROM_FILL(                             0x3800, 0x0800, 0xff ), /* ROM socket ROM6 at 3C is unpopulated */

            ROM_REGION( 0x01000, "speech", 0 ), /* voice data */
            ROM_LOAD( "berzerk_r_vo_1c.1c", 0x0000, 0x0800, CRC("2cfe825d") + SHA1("f12fed8712f20fa8213f606c4049a8144bfea42e") ),  /* VSU-1000 board */
            ROM_LOAD( "berzerk_r_vo_2c.2c", 0x0800, 0x0800, CRC("d2b6324e") + SHA1("20a6611ad6ec19409ac138bdae7bdfaeab6c47cf") ),  /* ditto */

            ROM_END,
        };


        //ROM_START( berzerka )

        //ROM_START( berzerkb )

        //ROM_START( berzerkf )

        //ROM_START( berzerkg )

        //ROM_START( berzerks )

        //ROM_START( frenzy )

        //ROM_START( moonwarp )
    }


    //void berzerk_state::init_moonwarp()


    partial class berzerk : construct_ioport_helper
    {
        /*************************************
         *
         *  Game drivers
         *
         *************************************/

        static void berzerk_state_berzerk(machine_config config, device_t device) { berzerk_state berzerk_state = (berzerk_state)device; berzerk_state.berzerk(config); }

        static berzerk m_berzerk = new berzerk();

        static device_t device_creator_berzerk(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new berzerk_state(mconfig, type, tag); }

        public static readonly game_driver driver_berzerk = GAME(device_creator_berzerk, rom_berzerk, "1980", "berzerk",  "0", berzerk_state_berzerk, m_berzerk.construct_ioport_berzerk, driver_device.empty_init,    ROT0, "Stern Electronics", "Berzerk (revision RC31A)", MACHINE_SUPPORTS_SAVE );
    }
}
