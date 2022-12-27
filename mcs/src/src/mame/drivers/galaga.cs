// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;

using static mame._74259_global;
using static mame.diexec_global;
using static mame.digfx_global;
using static mame.discrete_global;
using static mame.disound_global;
using static mame.drawgfx_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.emupal_global;
using static mame.er2055_global;
using static mame.gamedrv_global;
using static mame.generic_global;
using static mame.hash_global;
using static mame.ioport_global;
using static mame.ioport_input_string_helper;
using static mame.ioport_ioport_type_helper;
using static mame.namco_global;
using static mame.namco06_global;
using static mame.namco50_global;
using static mame.namco51_global;
using static mame.namco53_global;
using static mame.namco54_global;
using static mame.romentry_global;
using static mame.screen_global;
using static mame.speaker_global;
using static mame.starfield_05xx_global;
using static mame.util;
using static mame.watchdog_global;
using static mame.z80_global;


namespace mame
{
    partial class galaga_state : driver_device
    {
        protected static readonly XTAL MASTER_CLOCK = new XTAL(18432000);


        const uint16_t STARFIELD_X_OFFSET_GALAGA = 16;
        const uint16_t STARFIELD_X_LIMIT_GALAGA  = 256 + STARFIELD_X_OFFSET_GALAGA;

        //#define STARFIELD_Y_OFFSET_BOSCO        16
        //#define STARFIELD_X_LIMIT_BOSCO     224


        protected uint8_t bosco_dsw_r(offs_t offset)
        {
            int bit0;
            int bit1;

            bit0 = (int)((ioport("DSWB").read() >> (int)offset) & 1);
            bit1 = (int)((ioport("DSWA").read() >> (int)offset) & 1);

            return (uint8_t)(bit0 | (bit1 << 1));
        }

        //WRITE_LINE_MEMBER(galaga_state::flip_screen_w)
        protected void flip_screen_w(int state)
        {
            flip_screen_set((u32)state);
        }

        //WRITE_LINE_MEMBER(galaga_state::irq1_clear_w)
        protected void irq1_clear_w(int state)
        {
            m_main_irq_mask = (uint8_t)state;
            if (m_main_irq_mask == 0)
                m_maincpu.op0.set_input_line(0, CLEAR_LINE);
        }

        //WRITE_LINE_MEMBER(galaga_state::irq2_clear_w)
        protected void irq2_clear_w(int state)
        {
            m_sub_irq_mask = (uint8_t)state;
            if (m_sub_irq_mask == 0)
                m_subcpu.op0.set_input_line(0, CLEAR_LINE);
        }

        //WRITE_LINE_MEMBER(galaga_state::nmion_w)
        protected void nmion_w(int state)
        {
            m_sub2_nmi_mask = state == 0 ? (uint8_t)1 : (uint8_t)0;
        }


        protected void out_(uint8_t data)
        {
            m_leds[1] = BIT(data, 0);
            m_leds[0] = BIT(data, 1);
            machine().bookkeeping().coin_counter_w(1, ~data & 4);
            machine().bookkeeping().coin_counter_w(0, ~data & 8);
        }

        //WRITE_LINE_MEMBER(galaga_state::lockout)
        protected void lockout(int state)
        {
            machine().bookkeeping().coin_lockout_global_w(state);
        }


        //TIMER_CALLBACK_MEMBER(galaga_state::cpu3_interrupt_callback)
        void cpu3_interrupt_callback(s32 param)
        {
            int scanline = param;

            if (m_sub2_nmi_mask != 0)
                m_subcpu2.op0.pulse_input_line(INPUT_LINE_NMI, attotime.zero);

            scanline = scanline + 128;
            if (scanline >= 272)
                scanline = 64;

            /* the vertical synch chain is clocked by H256 -- this is probably not important, but oh well */
            m_cpu3_interrupt_timer.adjust(m_screen.op0.time_until_pos(scanline), scanline);
        }
    }


    partial class digdug_state : galaga_state
    {
        uint8_t earom_read()
        {
            return m_earom.op0.data();
        }


        void earom_write(offs_t offset, uint8_t data)
        {
            m_earom.op0.set_address((uint8_t)(offset & 0x3f));
            m_earom.op0.set_data(data);
        }


        void earom_control_w(uint8_t data)
        {
            // CK = DB0, C1 = /DB1, C2 = DB2, CS1 = DB3, /CS2 = GND
            m_earom.op0.set_control((uint8_t)BIT(data, 3), 1, BIT(data, 1) == 0 ? (uint8_t)1 : (uint8_t)0, (uint8_t)BIT(data, 2));
            m_earom.op0.set_clk(BIT(data, 0));
        }
    }


    partial class galaga_state : driver_device
    {
        protected override void machine_start()
        {
            m_leds.resolve();
            /* create the interrupt timer */
            m_cpu3_interrupt_timer = machine().scheduler().timer_alloc(cpu3_interrupt_callback);
            save_item(NAME(new { m_main_irq_mask }));
            save_item(NAME(new { m_sub_irq_mask }));
            save_item(NAME(new { m_sub2_nmi_mask }));
        }
    }


    partial class digdug_state : galaga_state
    {
        protected override void machine_start()
        {
            base.machine_start();
            earom_control_w(0);
        }
    }


    partial class galaga_state : driver_device
    {
        protected override void machine_reset()
        {
            m_cpu3_interrupt_timer.adjust(m_screen.op0.time_until_pos(64), 64);
        }


        void galaga_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x3fff).rom().nopw();         /* the only area different for each CPU */
            map.op(0x6800, 0x6807).r(bosco_dsw_r);
            map.op(0x6800, 0x681f).w(m_namco_sound, (offset, data) => { m_namco_sound.op0.pacman_sound_w(offset, data); });  //FUNC(namco_device::pacman_sound_w));
            map.op(0x6820, 0x6827).w("misclatch", (offset, data) => { ((ls259_device)subdevice("misclatch")).write_d0(offset, data); });  //FUNC(ls259_device::write_d0));
            map.op(0x6830, 0x6830).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });  //FUNC(watchdog_timer_device::reset_w));
            map.op(0x7000, 0x70ff).rw("06xx", (offset) => { return ((namco_06xx_device)subdevice("06xx")).data_r(offset); }, (offset, data) => { ((namco_06xx_device)subdevice("06xx")).data_w(offset, data); });  //FUNC(namco_06xx_device::data_r), FUNC(namco_06xx_device::data_w));
            map.op(0x7100, 0x7100).rw("06xx", () => { return ((namco_06xx_device)subdevice("06xx")).ctrl_r(); }, (data) => { ((namco_06xx_device)subdevice("06xx")).ctrl_w(data); });  //FUNC(namco_06xx_device::ctrl_r), FUNC(namco_06xx_device::ctrl_w));
            map.op(0x8000, 0x87ff).ram().w(galaga_videoram_w).share("videoram");
            map.op(0x8800, 0x8bff).ram().share("galaga_ram1");
            map.op(0x9000, 0x93ff).ram().share("galaga_ram2");
            map.op(0x9800, 0x9bff).ram().share("galaga_ram3");
            map.op(0xa000, 0xa007).w(m_videolatch, (offset, data) => { m_videolatch.op0.write_d0(offset, data); });  //FUNC(ls259_device::write_d0));
        }
    }


    partial class xevious_state : galaga_state
    {
        void xevious_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x3fff).rom().nopw();         /* the only area different for each CPU */
            map.op(0x6800, 0x6807).r(bosco_dsw_r);
            map.op(0x6800, 0x681f).w(m_namco_sound, (offset, data) => { m_namco_sound.op0.pacman_sound_w(offset, data); });  //FUNC(namco_device::pacman_sound_w));
            map.op(0x6820, 0x6827).w("misclatch", (offset, data) => { ((ls259_device)subdevice("misclatch")).write_d0(offset, data); });  //FUNC(ls259_device::write_d0));
            map.op(0x6830, 0x6830).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });  //FUNC(watchdog_timer_device::reset_w));
            map.op(0x7000, 0x70ff).rw("06xx", (offset) => { return ((namco_06xx_device)subdevice("06xx")).data_r(offset); }, (offset, data) => { ((namco_06xx_device)subdevice("06xx")).data_w(offset, data); });  //FUNC(namco_06xx_device::data_r), FUNC(namco_06xx_device::data_w));
            map.op(0x7100, 0x7100).rw("06xx", () => { return ((namco_06xx_device)subdevice("06xx")).ctrl_r(); }, (data) => { ((namco_06xx_device)subdevice("06xx")).ctrl_w(data); });  //FUNC(namco_06xx_device::ctrl_r), FUNC(namco_06xx_device::ctrl_w));
            map.op(0x7800, 0x7fff).ram().share("share1");                          /* work RAM */
            map.op(0x8000, 0x87ff).ram().share("xevious_sr1"); /* work RAM + sprite registers */
            map.op(0x9000, 0x97ff).ram().share("xevious_sr2"); /* work RAM + sprite registers */
            map.op(0xa000, 0xa7ff).ram().share("xevious_sr3"); /* work RAM + sprite registers */
            map.op(0xb000, 0xb7ff).ram().w(xevious_fg_colorram_w).share("fg_colorram");
            map.op(0xb800, 0xbfff).ram().w(xevious_bg_colorram_w).share("bg_colorram");
            map.op(0xc000, 0xc7ff).ram().w(xevious_fg_videoram_w).share("fg_videoram");
            map.op(0xc800, 0xcfff).ram().w(xevious_bg_videoram_w).share("bg_videoram");
            map.op(0xd000, 0xd07f).w(xevious_vh_latch_w);
            map.op(0xf000, 0xffff).rw(xevious_bb_r, xevious_bs_w);
        }
    }


    partial class digdug_state : galaga_state
    {
        void digdug_state_digdug_map(address_map map, device_t device)
        {
            map.op(0x0000, 0x3fff).rom().nopw();         /* the only area different for each CPU */
            map.op(0x6800, 0x681f).w(m_namco_sound, (offset, data) => { m_namco_sound.op0.pacman_sound_w(offset, data); });  //FUNC(namco_device::pacman_sound_w));
            map.op(0x6820, 0x6827).w("misclatch", (offset, data) => { ((ls259_device)subdevice("misclatch")).write_d0(offset, data); });  //FUNC(ls259_device::write_d0));
            map.op(0x6830, 0x6830).w("watchdog", (data) => { ((watchdog_timer_device)subdevice("watchdog")).reset_w(data); });  //FUNC(watchdog_timer_device::reset_w));
            map.op(0x7000, 0x70ff).rw("06xx", (offset) => { return ((namco_06xx_device)subdevice("06xx")).data_r(offset); }, (offset, data) => { ((namco_06xx_device)subdevice("06xx")).data_w(offset, data); });  //FUNC(namco_06xx_device::data_r), FUNC(namco_06xx_device::data_w));
            map.op(0x7100, 0x7100).rw("06xx", () => { return ((namco_06xx_device)subdevice("06xx")).ctrl_r(); }, (data) => { ((namco_06xx_device)subdevice("06xx")).ctrl_w(data); });  //FUNC(namco_06xx_device::ctrl_r), FUNC(namco_06xx_device::ctrl_w));
            map.op(0x8000, 0x83ff).ram().w(digdug_videoram_w).share("videoram"); /* tilemap RAM (bottom half of RAM 0 */
            map.op(0x8400, 0x87ff).ram().share("share1");                          /* work RAM (top half for RAM 0 */
            map.op(0x8800, 0x8bff).ram().share("digdug_objram");   /* work RAM + sprite registers */
            map.op(0x9000, 0x93ff).ram().share("digdug_posram");   /* work RAM + sprite registers */
            map.op(0x9800, 0x9bff).ram().share("digdug_flpram");   /* work RAM + sprite registers */
            map.op(0xa000, 0xa007).nopr().w(m_videolatch, (offset, data) => { m_videolatch.op0.write_d0(offset, data); });  //FUNC(ls259_device::write_d0));   /* video latches (spurious reads when setting latch bits) */
            map.op(0xb800, 0xb83f).rw(earom_read, earom_write);   /* non volatile memory data */
            map.op(0xb840, 0xb840).w(earom_control_w);                    /* non volatile memory control */
        }
    }


    public partial class galaga : construct_ioport_helper
    {
        //static INPUT_PORTS_START( galaga )
        void construct_ioport_galaga(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_2WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_2WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_2WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_UNUSED );
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_2WAY(); PORT_COCKTAIL();

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_SERVICE( 0x80, IP_ACTIVE_LOW );

            PORT_START("DSWA");
            PORT_DIPNAME( 0x03, 0x03, DEF_STR( Difficulty ) );   PORT_DIPLOCATION("SWB:1,2");
            PORT_DIPSETTING(    0x03, DEF_STR( Easy ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Medium ) );
            PORT_DIPSETTING(    0x01, DEF_STR( Hard ) );
            PORT_DIPSETTING(    0x02, DEF_STR( Hardest ) );
            PORT_DIPUNUSED_DIPLOC( 0x04, IP_ACTIVE_LOW, "SWB:3" ); /* Listed as "Unused" */
            PORT_DIPNAME( 0x08, 0x00, DEF_STR( Demo_Sounds ) );  PORT_DIPLOCATION("SWB:4");
            PORT_DIPSETTING(    0x08, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x10, 0x10, "Freeze" );                PORT_DIPLOCATION("SWB:5");
            PORT_DIPSETTING(    0x10, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x20, 0x20, "Rack Test" );             PORT_DIPLOCATION("SWB:6");
            PORT_DIPSETTING(    0x20, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPUNUSED_DIPLOC( 0x40, IP_ACTIVE_LOW, "SWB:7" ); /* Listed as "Unused" */
            PORT_DIPNAME( 0x80, 0x80, DEF_STR( Cabinet ) );      PORT_DIPLOCATION("SWB:8");
            PORT_DIPSETTING(    0x80, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) );

            PORT_START("DSWB");
            PORT_DIPNAME( 0x07, 0x07, DEF_STR( Coinage ) );      PORT_DIPLOCATION("SWA:1,2,3");
            PORT_DIPSETTING(    0x04, DEF_STR( _4C_1C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x06, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x07, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x05, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Free_Play ) );
            PORT_DIPNAME( 0x38, 0x10, DEF_STR( Bonus_Life ) );   PORT_DIPLOCATION("SWA:4,5,6");
            PORT_DIPSETTING(    0x20, "20K, 60K, Every 60K" );   PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0); /* Began with 2, 3 or 4 fighters */
            PORT_DIPSETTING(    0x18, "20K and 60K Only" );      PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x10, "20K, 70K, Every 70K" );   PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0); // factory default = "20K, 70K, Every70K"
            PORT_DIPSETTING(    0x30, "20K, 80K, Every 80K" );   PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x38, "30K and 80K Only" );      PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x08, "30K, 100K, Every 100K" ); PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x28, "30K, 120K, Every 120K" ); PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );     PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x20, "30K, 100K, Every 100K" ); PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0); /* Began with 5 fighters */
            PORT_DIPSETTING(    0x18, "30K and 150K Only" );     PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x10, "30K, 120K, Every 120K" ); PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x30, "30K, 150K, Every 150K" ); PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x38, "30K Only" );              PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x08, "30K and 100K Only" );     PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x28, "30K and 120K Only" );     PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );     PORT_CONDITION("DSWB", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPNAME( 0xc0, 0x80, DEF_STR( Lives ) );    PORT_DIPLOCATION("SWA:7,8");
            PORT_DIPSETTING(    0x00, "2" );
            PORT_DIPSETTING(    0x80, "3" ); // factory default = "3"
            PORT_DIPSETTING(    0x40, "4" );
            PORT_DIPSETTING(    0xc0, "5" );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( xevious )
        void construct_ioport_xevious(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_8WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_8WAY(); PORT_COCKTAIL();

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_SERVICE( 0x80, IP_ACTIVE_LOW );

            PORT_START("DSWA");
            PORT_DIPNAME( 0x03, 0x03, DEF_STR( Coin_A ) );       PORT_DIPLOCATION("SWA:1,2");
            PORT_DIPSETTING(    0x01, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _1C_2C ) );
            PORT_DIPNAME( 0x1c, 0x1c, DEF_STR( Bonus_Life ) );   PORT_DIPLOCATION("SWA:3,4,5");
            PORT_DIPSETTING(    0x18, "10K, 40K, Every 40K" );   PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.NOTEQUALS, 0x00);
            PORT_DIPSETTING(    0x14, "10K, 50K, Every 50K" );   PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.NOTEQUALS, 0x00);
            PORT_DIPSETTING(    0x10, "20K, 50K, Every 50K" );   PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.NOTEQUALS, 0x00);
            PORT_DIPSETTING(    0x1c, "20K, 60K, Every 60K" );   PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.NOTEQUALS, 0x00); // factory default = "20K, 60K, Every60K"
            PORT_DIPSETTING(    0x0c, "20K, 70K, Every 70K" );   PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.NOTEQUALS, 0x00);
            PORT_DIPSETTING(    0x08, "20K, 80K, Every 80K" );   PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.NOTEQUALS, 0x00);
            PORT_DIPSETTING(    0x04, "20K and 60K Only" );      PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.NOTEQUALS, 0x00);
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );     PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.NOTEQUALS, 0x00);
            PORT_DIPSETTING(    0x18, "10K, 50K, Every 50K" );   PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.EQUALS, 0x00);
            PORT_DIPSETTING(    0x14, "20K, 50K, Every 50K" );   PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.EQUALS, 0x00);
            PORT_DIPSETTING(    0x10, "20K, 60K, Every 60K" );   PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.EQUALS, 0x00);
            PORT_DIPSETTING(    0x1c, "20K, 70K, Every 70K" );   PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.EQUALS, 0x00);
            PORT_DIPSETTING(    0x0c, "20K, 80K, Every 80K" );   PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.EQUALS, 0x00);
            PORT_DIPSETTING(    0x08, "30K, 100K, Every 100K" ); PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.EQUALS, 0x00);
            PORT_DIPSETTING(    0x04, "20K and 80K Only" );      PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.EQUALS, 0x00);
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );     PORT_CONDITION("DSWA", 0x60, ioport_condition.condition_t.EQUALS, 0x00);
            PORT_DIPNAME( 0x60, 0x60, DEF_STR( Lives ) );    PORT_DIPLOCATION("SWA:6,7");
            PORT_DIPSETTING(    0x40, "1" );
            PORT_DIPSETTING(    0x20, "2" );
            PORT_DIPSETTING(    0x60, "3" ); // factory default = "3"
            PORT_DIPSETTING(    0x00, "5" );
            PORT_DIPNAME( 0x80, 0x80, DEF_STR( Cabinet ) );      PORT_DIPLOCATION("SWA:8");
            PORT_DIPSETTING(    0x80, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) );

            PORT_START("DSWB");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON2 );
            PORT_DIPNAME( 0x02, 0x02, "Flags Award Bonus Life" );    PORT_DIPLOCATION("SWB:2");
            PORT_DIPSETTING(    0x00, DEF_STR( No ) );
            PORT_DIPSETTING(    0x02, DEF_STR( Yes ) );
            PORT_DIPNAME( 0x0c, 0x0c, DEF_STR( Coin_B ) );           PORT_DIPLOCATION("SWB:3,4");
            PORT_DIPSETTING(    0x04, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x0c, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x08, DEF_STR( _1C_2C ) );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_BUTTON2 ); PORT_COCKTAIL();
            PORT_DIPNAME( 0x60, 0x60, DEF_STR( Difficulty ) );       PORT_DIPLOCATION("SWB:6,7");
            PORT_DIPSETTING(    0x40, DEF_STR( Easy ) );
            PORT_DIPSETTING(    0x60, DEF_STR( Normal ) );
            PORT_DIPSETTING(    0x20, DEF_STR( Hard ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Hardest ) );
            PORT_DIPNAME( 0x80, 0x80, "Freeze" );                    PORT_DIPLOCATION("SWB:8");
            PORT_DIPSETTING(    0x80, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );

            INPUT_PORTS_END();
        }


        //static INPUT_PORTS_START( digdug )
        void construct_ioport_digdug(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            digdug_state state = (digdug_state)owner;

            INPUT_PORTS_START(owner, portlist, ref errorbuf);

            PORT_START("IN0");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY();
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY();
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY();
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_JOYSTICK_UP ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_JOYSTICK_RIGHT ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_JOYSTICK_DOWN ); PORT_4WAY(); PORT_COCKTAIL();
            PORT_BIT( 0x80, IP_ACTIVE_LOW, IPT_JOYSTICK_LEFT ); PORT_4WAY(); PORT_COCKTAIL();

            PORT_START("IN1");
            PORT_BIT( 0x01, IP_ACTIVE_LOW, IPT_BUTTON1 );
            PORT_BIT( 0x02, IP_ACTIVE_LOW, IPT_BUTTON1 ); PORT_COCKTAIL();
            PORT_BIT( 0x04, IP_ACTIVE_LOW, IPT_START1 );
            PORT_BIT( 0x08, IP_ACTIVE_LOW, IPT_START2 );
            PORT_BIT( 0x10, IP_ACTIVE_LOW, IPT_COIN1 );
            PORT_BIT( 0x20, IP_ACTIVE_LOW, IPT_COIN2 );
            PORT_BIT( 0x40, IP_ACTIVE_LOW, IPT_SERVICE1 );
            PORT_SERVICE( 0x80, IP_ACTIVE_LOW );

            PORT_START("DSWA");
            PORT_DIPNAME( 0x07, 0x01, DEF_STR( Coin_B ) );       PORT_DIPLOCATION("SWA:1,2,3");
            PORT_DIPSETTING(    0x07, DEF_STR( _3C_1C ) );
            PORT_DIPSETTING(    0x03, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x01, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0x05, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x06, DEF_STR( _1C_2C ) );
            PORT_DIPSETTING(    0x02, DEF_STR( _1C_3C ) );
            PORT_DIPSETTING(    0x04, DEF_STR( _1C_6C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_7C ) );
            PORT_DIPNAME( 0x38, 0x18, DEF_STR( Bonus_Life ) );   PORT_DIPLOCATION("SWA:4,5,6");
            PORT_DIPSETTING(    0x20, "10K, 40K, Every 40K" );   PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0); // Atari factory default = "10K, 40K, Every40K"
            PORT_DIPSETTING(    0x10, "10K, 50K, Every 50K" );   PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x30, "20K, 60K, Every 60K" );   PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x08, "20K, 70K, Every 70K" );   PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x28, "10K and 40K Only" );      PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x18, "20K and 60K Only" );      PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0); // Namco factory default = "20K, 60K"
            PORT_DIPSETTING(    0x38, "10K Only" );              PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );     PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.NOTEQUALS, 0xc0);
            PORT_DIPSETTING(    0x20, "20K, 60K, Every 60K" );   PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x10, "30K, 80K, Every 80K" );   PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x30, "20K and 50K Only" );      PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x08, "20K and 60K Only" );      PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x28, "30K and 70K Only" );      PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x18, "20K Only" );              PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x38, "30K Only" );              PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPSETTING(    0x00, DEF_STR( None ) );     PORT_CONDITION("DSWA", 0xc0, ioport_condition.condition_t.EQUALS, 0xc0);
            PORT_DIPNAME( 0xc0, 0x80, DEF_STR( Lives ) );    PORT_DIPLOCATION("SWA:7,8");
            PORT_DIPSETTING(    0x00, "1" );
            PORT_DIPSETTING(    0x40, "2" );
            PORT_DIPSETTING(    0x80, "3" ); // factory default = "3"
            PORT_DIPSETTING(    0xc0, "5" );

            PORT_START("DSWB"); // reverse order against SWA
            PORT_DIPNAME( 0xc0, 0x00, DEF_STR( Coin_A ) );           PORT_DIPLOCATION("SWB:1,2");
            PORT_DIPSETTING(    0x40, DEF_STR( _2C_1C ) );
            PORT_DIPSETTING(    0x00, DEF_STR( _1C_1C ) );
            PORT_DIPSETTING(    0xc0, DEF_STR( _2C_3C ) );
            PORT_DIPSETTING(    0x80, DEF_STR( _1C_2C ) );
            PORT_DIPNAME( 0x20, 0x20, "Freeze" );                    PORT_DIPLOCATION("SWB:3");
            PORT_DIPSETTING(    0x20, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x10, 0x00, DEF_STR( Demo_Sounds ) );      PORT_DIPLOCATION("SWB:4");
            PORT_DIPSETTING(    0x10, DEF_STR( Off ) );
            PORT_DIPSETTING(    0x00, DEF_STR( On ) );
            PORT_DIPNAME( 0x08, 0x00, DEF_STR( Allow_Continue ) );   PORT_DIPLOCATION("SWB:5");
            PORT_DIPSETTING(    0x08, DEF_STR( No ) ); // factory default = "No"
            PORT_DIPSETTING(    0x00, DEF_STR( Yes ) );
            PORT_DIPNAME( 0x04, 0x04, DEF_STR( Cabinet ) );          PORT_DIPLOCATION("SWB:6");
            PORT_DIPSETTING(    0x04, DEF_STR( Upright ) );
            PORT_DIPSETTING(    0x00, DEF_STR( Cocktail ) );
            PORT_DIPNAME( 0x03, 0x00, DEF_STR( Difficulty ) );       PORT_DIPLOCATION("SWB:7,8");
            PORT_DIPSETTING(    0x00, DEF_STR( Easy ) );
            PORT_DIPSETTING(    0x02, DEF_STR( Medium ) );
            PORT_DIPSETTING(    0x01, DEF_STR( Hard ) );
            PORT_DIPSETTING(    0x03, DEF_STR( Hardest ) );

            INPUT_PORTS_END();
        }
    }


    partial class galaga_state : driver_device
    {
        static readonly gfx_layout charlayout_2bpp = new gfx_layout(
            8,8,
            RGN_FRAC(1,1),
            2,
            ArrayCombineUInt32(0, 4),
            ArrayCombineUInt32(STEP4(8*8,1), STEP4(0*8,1)),
            ArrayCombineUInt32(STEP8(0*8,8)),
            16*8
        );


        static readonly gfx_layout spritelayout_galaga = new gfx_layout(
            16,16,
            RGN_FRAC(1,1),
            2,
            ArrayCombineUInt32(0, 4),
            ArrayCombineUInt32(STEP4(0*8,1), STEP4(8*8,1), STEP4(16*8,1), STEP4(24*8,1)),
            ArrayCombineUInt32(STEP8(0*8,8), STEP8(32*8,8)),
            64*8
        );


        static readonly gfx_layout spritelayout_xevious = new gfx_layout(
            16,16,
            RGN_FRAC(1,2),
            3,
            ArrayCombineUInt32(RGN_FRAC(1,2)+4, 0, 4),
            ArrayCombineUInt32(STEP4(0*8,1), STEP4(8*8,1), STEP4(16*8,1), STEP4(24*8,1)),
            ArrayCombineUInt32(STEP8(0*8,8), STEP8(32*8,8)),
            64*8
        );


        static readonly gfx_layout charlayout_digdug = new gfx_layout(
            8,8,
            RGN_FRAC(1,1),
            1,
            ArrayCombineUInt32(0),
            ArrayCombineUInt32(STEP8(7,-1)),
            ArrayCombineUInt32(STEP8(0,8)),
            8*8
        );


        static readonly gfx_layout bgcharlayout = new gfx_layout(
            8,8,
            RGN_FRAC(1,2),
            2,
            ArrayCombineUInt32(0, RGN_FRAC(1,2)),
            ArrayCombineUInt32(STEP8(0,1)),
            ArrayCombineUInt32(STEP8(0,8)),
            8*8
        );


        //static GFXDECODE_START( gfx_galaga )
        static readonly gfx_decode_entry [] gfx_galaga =
        {
            GFXDECODE_ENTRY( "gfx1", 0, charlayout_2bpp,        0, 64 ),
            GFXDECODE_ENTRY( "gfx2", 0, spritelayout_galaga, 64*4, 64 ),
            //GFXDECODE_END
        };


        //static GFXDECODE_START( gfx_xevious )
        protected static readonly gfx_decode_entry [] gfx_xevious =
        {
            GFXDECODE_ENTRY( "gfx1", 0, gfx_8x8x1,          128*4+64*8,  64 ),
            GFXDECODE_ENTRY( "gfx2", 0, bgcharlayout,                0, 128 ),
            GFXDECODE_ENTRY( "gfx3", 0, spritelayout_xevious,    128*4,  64 ),
            //GFXDECODE_END
        };


        //static GFXDECODE_START( gfx_digdug )
        protected static readonly gfx_decode_entry [] gfx_digdug =
        {
            GFXDECODE_ENTRY( "gfx1", 0, charlayout_digdug,         0, 16 ),
            GFXDECODE_ENTRY( "gfx2", 0, spritelayout_galaga,    16*2, 64 ),
            GFXDECODE_ENTRY( "gfx3", 0, charlayout_2bpp, 64*4 + 16*2, 64 ),
            //GFXDECODE_END
        };


        //WRITE_LINE_MEMBER(galaga_state::vblank_irq)
        protected void vblank_irq(int state)
        {
            if (state != 0 && m_main_irq_mask != 0)
                m_maincpu.op0.set_input_line(0, ASSERT_LINE);

            if (state != 0 && m_sub_irq_mask != 0)
                m_subcpu.op0.set_input_line(0, ASSERT_LINE);
        }


        public void galaga(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, MASTER_CLOCK/6);   /* 3.072 MHz */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, galaga_map);

            Z80(config, m_subcpu, MASTER_CLOCK/6);    /* 3.072 MHz */
            m_subcpu.op0.memory().set_addrmap(AS_PROGRAM, galaga_map);

            Z80(config, m_subcpu2, MASTER_CLOCK/6);   /* 3.072 MHz */
            m_subcpu2.op0.memory().set_addrmap(AS_PROGRAM, galaga_map);

            ls259_device misclatch = LS259(config, "misclatch"); // 3C on CPU board
            misclatch.q_out_cb<u32_const_0>().set((write_line_delegate)irq1_clear_w).reg();
            misclatch.q_out_cb<u32_const_1>().set((write_line_delegate)irq2_clear_w).reg();
            misclatch.q_out_cb<u32_const_2>().set((write_line_delegate)nmion_w).reg();
            misclatch.q_out_cb<u32_const_3>().set_inputline("sub", INPUT_LINE_RESET).invert().reg();
            misclatch.q_out_cb<u32_const_3>().append_inputline("sub2", INPUT_LINE_RESET).invert().reg();
            misclatch.q_out_cb<u32_const_3>().append("51xx", (state) => { ((namco_51xx_device)subdevice("51xx")).reset(state); }).reg();
            misclatch.q_out_cb<u32_const_3>().append("54xx", (state) => { ((namco_54xx_device)subdevice("54xx")).reset(state); }).reg();

            namco_51xx_device n51xx = NAMCO_51XX(config, "51xx", MASTER_CLOCK/6/2);      /* 1.536 MHz */
            n51xx.input_callback<u32_const_0>().set_ioport("IN0").mask_u32(0x0f).reg();
            n51xx.input_callback<u32_const_1>().set_ioport("IN0").rshift(4).reg();
            n51xx.input_callback<u32_const_2>().set_ioport("IN1").mask_u32(0x0f).reg();
            n51xx.input_callback<u32_const_3>().set_ioport("IN1").rshift(4).reg();
            n51xx.output_callback().set(out_).reg();
            n51xx.lockout_callback().set((write_line_delegate)lockout).reg();

            namco_54xx_device n54xx = NAMCO_54XX(config, "54xx", MASTER_CLOCK/6/2);      /* 1.536 MHz */
            n54xx.set_discrete("discrete");
            n54xx.set_basenote(NODE_01);

            namco_06xx_device n06xx = NAMCO_06XX(config, "06xx", MASTER_CLOCK/6/64);
            n06xx.set_maincpu(m_maincpu);
            n06xx.chip_select_callback<u32_const_0>().set("51xx", (int state) => { ((namco_51xx_device)subdevice("51xx")).chip_select(state); }).reg();
            n06xx.rw_callback<u32_const_0>().set("51xx", (int state) => { ((namco_51xx_device)subdevice("51xx")).rw(state); }).reg();
            n06xx.read_callback<u32_const_0>().set("51xx", () => { return ((namco_51xx_device)subdevice("51xx")).read(); }).reg();  //FUNC(namco_51xx_device::read));
            n06xx.write_callback<u32_const_0>().set("51xx", (data) => { ((namco_51xx_device)subdevice("51xx")).write(data); }).reg();  //FUNC(namco_51xx_device::write));
            n06xx.write_callback<u32_const_3>().set("54xx", (data) => { ((namco_54xx_device)subdevice("54xx")).write(data); }).reg();  //FUNC(namco_54xx_device::write));
            n06xx.chip_select_callback<u32_const_3>().set("54xx", (int state) => { ((namco_54xx_device)subdevice("54xx")).chip_select(state); }).reg();

            LS259(config, m_videolatch); // 5K on video board
            // Q0-Q5 to 05XX for starfield control
            m_videolatch.op0.q_out_cb<u32_const_7>().set((write_line_delegate)flip_screen_w).reg();

            WATCHDOG_TIMER(config, "watchdog").set_vblank_count(m_screen, 8);

            config.set_maximum_quantum(attotime.from_hz(6000));

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(MASTER_CLOCK/3, 384, 0, 288, 264, 0, 224);
            m_screen.op0.set_screen_update(screen_update_galaga);
            m_screen.op0.set_video_attributes(VIDEO_ALWAYS_UPDATE); // starfield lfsr
            m_screen.op0.screen_vblank().set((write_line_delegate)screen_vblank_galaga).reg();
            m_screen.op0.screen_vblank().append(vblank_irq).reg();
            m_screen.op0.screen_vblank().append("51xx", (state) => { ((namco_51xx_device)subdevice("51xx")).vblank(state); }).reg();
            m_screen.op0.set_palette("palette");

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_galaga);
            PALETTE(config, m_palette, galaga_palette, 64*4 + 64*4 + 4 + 64, 32+64);

            STARFIELD_05XX(config, m_starfield, 0);
            m_starfield.op0.set_starfield_config(STARFIELD_X_OFFSET_GALAGA, 0, STARFIELD_X_LIMIT_GALAGA);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            NAMCO(config, m_namco_sound, MASTER_CLOCK/6/32);
            m_namco_sound.op0.set_voices(3);
            m_namco_sound.op0.disound.add_route(ALL_OUTPUTS, "mono", 0.90 * 10.0 / 16.0);

            /* discrete circuit on the 54XX outputs */
            DISCRETE(config, "discrete", galaga_discrete).disound.add_route(ALL_OUTPUTS, "mono", 0.90);
        }
    }


    partial class xevious_state : galaga_state
    {
        public void xevious(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, MASTER_CLOCK/6);  /* 3.072 MHz */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, xevious_map);

            Z80(config, m_subcpu, MASTER_CLOCK/6);   /* 3.072 MHz */
            m_subcpu.op0.memory().set_addrmap(AS_PROGRAM, xevious_map);

            Z80(config, m_subcpu2, MASTER_CLOCK/6);  /* 3.072 MHz */
            m_subcpu2.op0.memory().set_addrmap(AS_PROGRAM, xevious_map);

            ls259_device misclatch = LS259(config, "misclatch"); // 5K
            misclatch.q_out_cb<u32_const_0>().set((write_line_delegate)irq1_clear_w).reg();
            misclatch.q_out_cb<u32_const_1>().set((write_line_delegate)irq2_clear_w).reg();
            misclatch.q_out_cb<u32_const_2>().set((write_line_delegate)nmion_w).reg();
            misclatch.q_out_cb<u32_const_3>().set_inputline("sub", INPUT_LINE_RESET).invert().reg();
            misclatch.q_out_cb<u32_const_3>().append_inputline("sub2", INPUT_LINE_RESET).invert().reg();
            misclatch.q_out_cb<u32_const_3>().append("50xx", (state) => { ((namco_50xx_device)subdevice("50xx")).reset(state); }).reg();  //misclatch.q_out_cb<3>().append("50xx", FUNC(namco_50xx_device::reset));
            misclatch.q_out_cb<u32_const_3>().append("51xx", (state) => { ((namco_51xx_device)subdevice("51xx")).reset(state); }).reg();  //misclatch.q_out_cb<3>().append("51xx", FUNC(namco_51xx_device::reset));
            misclatch.q_out_cb<u32_const_3>().append("54xx", (state) => { ((namco_54xx_device)subdevice("54xx")).reset(state); }).reg();  //misclatch.q_out_cb<3>().append("54xx", FUNC(namco_54xx_device::reset));

            NAMCO_50XX(config, "50xx", MASTER_CLOCK/6/2);   /* 1.536 MHz */

            namco_51xx_device n51xx = NAMCO_51XX(config, "51xx", MASTER_CLOCK/6/2);      /* 1.536 MHz */
            n51xx.input_callback<u32_const_0>().set_ioport("IN0").mask_u32(0x0f).reg();
            n51xx.input_callback<u32_const_1>().set_ioport("IN0").rshift(4).reg();
            n51xx.input_callback<u32_const_2>().set_ioport("IN1").mask_u32(0x0f).reg();
            n51xx.input_callback<u32_const_3>().set_ioport("IN1").rshift(4).reg();
            n51xx.output_callback().set(out_).reg();
            n51xx.lockout_callback().set((write_line_delegate)lockout).reg();

            namco_54xx_device n54xx = NAMCO_54XX(config, "54xx", MASTER_CLOCK/6/2);      /* 1.536 MHz */
            n54xx.set_discrete("discrete");
            n54xx.set_basenote(NODE_01);

            namco_06xx_device n06xx = NAMCO_06XX(config, "06xx", MASTER_CLOCK/6/64);
            n06xx.set_maincpu(m_maincpu);
            n06xx.chip_select_callback<u32_const_0>().set("51xx", (int state) => { ((namco_51xx_device)subdevice("51xx")).chip_select(state); }).reg();
            n06xx.rw_callback<u32_const_0>().set("51xx", (int state) => { ((namco_51xx_device)subdevice("51xx")).rw(state); }).reg();
            n06xx.read_callback<u32_const_0>().set("51xx", () => { return ((namco_51xx_device)subdevice("51xx")).read(); }).reg();  //FUNC(namco_51xx_device::read));
            n06xx.write_callback<u32_const_0>().set("51xx", (data) => { ((namco_51xx_device)subdevice("51xx")).write(data); }).reg();  //FUNC(namco_51xx_device::write));
            n06xx.chip_select_callback<u32_const_2>().set("50xx", (int state) => { ((namco_50xx_device)subdevice("50xx")).chip_select(state); }).reg();
            n06xx.rw_callback<u32_const_2>().set("50xx", (int state) => { ((namco_50xx_device)subdevice("50xx")).rw(state); }).reg();
            n06xx.read_callback<u32_const_2>().set("50xx", () => { return ((namco_50xx_device)subdevice("50xx")).read(); }).reg();  //FUNC(namco_50xx_device::read));
            n06xx.write_callback<u32_const_2>().set("50xx", (data) => { ((namco_50xx_device)subdevice("50xx")).write(data); }).reg();  //FUNC(namco_50xx_device::write));
            n06xx.write_callback<u32_const_3>().set("54xx", (data) => { ((namco_54xx_device)subdevice("54xx")).write(data); }).reg();  //FUNC(namco_54xx_device::write));
            n06xx.chip_select_callback<u32_const_3>().set("54xx", (int state) => { ((namco_54xx_device)subdevice("54xx")).chip_select(state); }).reg();

            WATCHDOG_TIMER(config, "watchdog").set_vblank_count(m_screen, 8);

            config.set_maximum_quantum(attotime.from_hz(6000));

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(MASTER_CLOCK/3, 384, 0, 288, 264, 0, 224);
            m_screen.op0.set_screen_update(screen_update_xevious);
            m_screen.op0.set_palette(m_palette);
            m_screen.op0.screen_vblank().set((write_line_delegate)vblank_irq).reg();
            m_screen.op0.screen_vblank().append("51xx", (state) => { ((namco_51xx_device)subdevice("51xx")).vblank(state); }).reg();

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_xevious);
            PALETTE(config, m_palette, xevious_palette, 128*4 + 64*8 + 64*2, 128+1);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            NAMCO(config, m_namco_sound, MASTER_CLOCK/6/32);
            m_namco_sound.op0.set_voices(3);
            m_namco_sound.op0.disound.add_route(ALL_OUTPUTS, "mono", 0.90 * 10.0 / 16.0);

            /* discrete circuit on the 54XX outputs */
            DISCRETE(config, "discrete", galaga_discrete).disound.add_route(ALL_OUTPUTS, "mono", 0.90);
        }
    }


    partial class digdug_state : galaga_state
    {
        public void digdug(machine_config config)
        {
            /* basic machine hardware */
            Z80(config, m_maincpu, MASTER_CLOCK/6);   /* 3.072 MHz */
            m_maincpu.op0.memory().set_addrmap(AS_PROGRAM, digdug_state_digdug_map);

            Z80(config, m_subcpu, MASTER_CLOCK/6);    /* 3.072 MHz */
            m_subcpu.op0.memory().set_addrmap(AS_PROGRAM, digdug_state_digdug_map);

            Z80(config, m_subcpu2, MASTER_CLOCK/6);   /* 3.072 MHz */
            m_subcpu2.op0.memory().set_addrmap(AS_PROGRAM, digdug_state_digdug_map);

            ls259_device misclatch = LS259(config, "misclatch"); // 8R
            misclatch.q_out_cb<u32_const_0>().set((write_line_delegate)irq1_clear_w).reg();
            misclatch.q_out_cb<u32_const_1>().set((write_line_delegate)irq2_clear_w).reg();
            misclatch.q_out_cb<u32_const_2>().set((write_line_delegate)nmion_w).reg();
            misclatch.q_out_cb<u32_const_3>().set_inputline("sub", INPUT_LINE_RESET).invert().reg();
            misclatch.q_out_cb<u32_const_3>().append_inputline("sub2", INPUT_LINE_RESET).invert().reg();
            misclatch.q_out_cb<u32_const_3>().append("51xx", (state) => { ((namco_51xx_device)subdevice("51xx")).reset(state); }).reg();  //misclatch.q_out_cb<3>().append("51xx", FUNC(namco_51xx_device::reset));
            misclatch.q_out_cb<u32_const_3>().append("53xx", (state) => { ((namco_53xx_device)subdevice("53xx")).reset(state); }).reg();  //misclatch.q_out_cb<3>().append("53xx", FUNC(namco_53xx_device::reset));
            // Q5-Q7 also used (see below)

            namco_51xx_device n51xx = NAMCO_51XX(config, "51xx", MASTER_CLOCK/6/2);      /* 1.536 MHz */
            n51xx.input_callback<u32_const_0>().set_ioport("IN0").mask_u32(0x0f).reg();
            n51xx.input_callback<u32_const_1>().set_ioport("IN0").rshift(4).reg();
            n51xx.input_callback<u32_const_2>().set_ioport("IN1").mask_u32(0x0f).reg();
            n51xx.input_callback<u32_const_3>().set_ioport("IN1").rshift(4).reg();
            n51xx.output_callback().set(out_).reg();
            n51xx.lockout_callback().set((write_line_delegate)lockout).reg();

            namco_53xx_device n53xx = NAMCO_53XX(config, "53xx", MASTER_CLOCK/6/2);      /* 1.536 MHz */
            n53xx.k_port_callback().set("misclatch", () => { return ((addressable_latch_device)subdevice("misclatch")).q7_r(); }).lshift(3).reg();  // FUNC(ls259_device::q7_r) // MOD 2 = K3
            n53xx.k_port_callback().append("misclatch", () => { return ((addressable_latch_device)subdevice("misclatch")).q6_r(); }).lshift(2).reg();  //FUNC(ls259_device::q6_r) // MOD 1 = K2
            n53xx.k_port_callback().append("misclatch", () => { return ((addressable_latch_device)subdevice("misclatch")).q5_r(); }).lshift(1).reg();  //FUNC(ls259_device::q5_r) // MOD 0 = K1
            // K0 is left unconnected
            n53xx.input_callback<u32_const_0>().set_ioport("DSWA").mask_u32(0x0f).reg();
            n53xx.input_callback<u32_const_1>().set_ioport("DSWA").rshift(4).reg();
            n53xx.input_callback<u32_const_2>().set_ioport("DSWB").mask_u32(0x0f).reg();
            n53xx.input_callback<u32_const_3>().set_ioport("DSWB").rshift(4).reg();

            namco_06xx_device n06xx = NAMCO_06XX(config, "06xx", MASTER_CLOCK/6/64);
            n06xx.set_maincpu(m_maincpu);
            n06xx.chip_select_callback<u32_const_0>().set("51xx", (int state) => { ((namco_51xx_device)subdevice("51xx")).chip_select(state); }).reg();  //n06xx.chip_select_callback<0>().set("51xx", FUNC(namco_51xx_device::chip_select));
            n06xx.rw_callback<u32_const_0>().set("51xx", (int state) => { ((namco_51xx_device)subdevice("51xx")).rw(state); }).reg();  //n06xx.rw_callback<0>().set("51xx", FUNC(namco_51xx_device::rw));
            n06xx.read_callback<u32_const_0>().set("51xx", () => { return ((namco_51xx_device)subdevice("51xx")).read(); }).reg();  //n06xx.read_callback<0>().set("51xx", FUNC(namco_51xx_device::read));
            n06xx.write_callback<u32_const_0>().set("51xx", (data) => { ((namco_51xx_device)subdevice("51xx")).write(data); }).reg();  //n06xx.write_callback<0>().set("51xx", FUNC(namco_51xx_device::write));
            n06xx.chip_select_callback<u32_const_1>().set("53xx", (int state) => { ((namco_53xx_device)subdevice("53xx")).chip_select(state); }).reg();  //n06xx.chip_select_callback<1>().set("53xx", FUNC(namco_53xx_device::chip_select));
            n06xx.read_callback<u32_const_1>().set("53xx", () => { return ((namco_53xx_device)subdevice("53xx")).read(); }).reg();  //n06xx.read_callback<1>().set("53xx", FUNC(namco_53xx_device::read));

            LS259(config, m_videolatch); // 5R
            m_videolatch.op0.parallel_out_cb().set(bg_select_w).mask_u8(0x33).reg();
            m_videolatch.op0.q_out_cb<u32_const_2>().set((write_line_delegate)tx_color_mode_w).reg();
            m_videolatch.op0.q_out_cb<u32_const_3>().set((write_line_delegate)bg_disable_w).reg();
            m_videolatch.op0.q_out_cb<u32_const_7>().set((write_line_delegate)flip_screen_w).reg();

            ER2055(config, m_earom);

            WATCHDOG_TIMER(config, "watchdog");

            config.set_maximum_quantum(attotime.from_hz(6000));

            /* video hardware */
            SCREEN(config, m_screen, SCREEN_TYPE_RASTER);
            m_screen.op0.set_raw(MASTER_CLOCK/3, 384, 0, 288, 264, 0, 224);
            m_screen.op0.set_screen_update(screen_update_digdug);
            m_screen.op0.set_palette(m_palette);
            m_screen.op0.screen_vblank().set((write_line_delegate)vblank_irq).reg();
            m_screen.op0.screen_vblank().append("51xx", (state) => { ((namco_51xx_device)subdevice("51xx")).vblank(state); }).reg();

            GFXDECODE(config, m_gfxdecode, m_palette, gfx_digdug);
            PALETTE(config, m_palette, digdug_palette, 16*2 + 64*4 + 64*4, 32);

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            NAMCO(config, m_namco_sound, MASTER_CLOCK/6/32);
            m_namco_sound.op0.set_voices(3);
            m_namco_sound.op0.disound.add_route(ALL_OUTPUTS, "mono", 0.90 * 10.0 / 16.0);
        }
    }


    partial class galaga : construct_ioport_helper
    {
        /***************************************************************************

          Game driver(s)

        ***************************************************************************/

        //ROM_START( galaga )
        static readonly tiny_rom_entry [] rom_galaga =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ),     /* 64k for code for the first CPU  */
            ROM_LOAD( "gg1_1b.3p",    0x0000, 0x1000, CRC("ab036c9f") + SHA1("ca7f5da42d4e76fd89bb0b35198a23c01462fbfe") ),
            ROM_LOAD( "gg1_2b.3m",    0x1000, 0x1000, CRC("d9232240") + SHA1("ab202aa259c3d332ef13dfb8fc8580ce2a5a253d") ),
            ROM_LOAD( "gg1_3.2m",     0x2000, 0x1000, CRC("753ce503") + SHA1("481f443aea3ed3504ec2f3a6bfcf3cd47e2f8f81") ),
            ROM_LOAD( "gg1_4b.2l",    0x3000, 0x1000, CRC("499fcc76") + SHA1("ddb8b121903646c320939c7d13f4aa4ebb130378") ),

            ROM_REGION( 0x10000, "sub", 0 ),     /* 64k for the second CPU */
            ROM_LOAD( "gg1_5b.3f",    0x0000, 0x1000, CRC("bb5caae3") + SHA1("e957a581463caac27bc37ca2e2a90f27e4f62b6f") ),

            ROM_REGION( 0x10000, "sub2", 0 ),     /* 64k for the third CPU  */
            ROM_LOAD( "gg1_7b.2c",    0x0000, 0x1000, CRC("d016686b") + SHA1("44c1a04fba3c7c826ff484185cb881b4b22e6657") ),

            ROM_REGION( 0x1000, "gfx1", 0 ),
            ROM_LOAD( "gg1_9.4l",     0x0000, 0x1000, CRC("58b2f47c") + SHA1("62f1279a784ab2f8218c4137c7accda00e6a3490") ),

            ROM_REGION( 0x2000, "gfx2", 0 ),
            ROM_LOAD( "gg1_11.4d",    0x0000, 0x1000, CRC("ad447c80") + SHA1("e697c180178cabd1d32483c5d8889a40633f7857") ),
            ROM_LOAD( "gg1_10.4f",    0x1000, 0x1000, CRC("dd6f1afc") + SHA1("c340ed8c25e0979629a9a1730edc762bd72d0cff") ),

            ROM_REGION( 0x0220, "proms", 0 ),
            ROM_LOAD( "prom-5.5n",    0x0000, 0x0020, CRC("54603c6b") + SHA1("1a6dea13b4af155d9cb5b999a75d4f1eb9c71346") ),    /* palette */
            ROM_LOAD( "prom-4.2n",    0x0020, 0x0100, CRC("59b6edab") + SHA1("0281de86c236c88739297ff712e0a4f5c8bf8ab9") ),    /* char lookup table */
            ROM_LOAD( "prom-3.1c",    0x0120, 0x0100, CRC("4a04bb6b") + SHA1("cdd4bc1013f5c11984fdc4fd10e2d2e27120c1e5") ),    /* sprite lookup table */

            ROM_REGION( 0x0200, "namco", 0 ),
            ROM_LOAD( "prom-1.1d",    0x0000, 0x0100, CRC("7a2815b4") + SHA1("085ada18c498fdb18ecedef0ea8fe9217edb7b46") ),
            ROM_LOAD( "prom-2.5c",    0x0100, 0x0100, CRC("77245b66") + SHA1("0c4d0bee858b97632411c440bea6948a74759746") ),    /* timing - not used */

            ROM_END,
        };


        //ROM_START( xevious )
        static readonly tiny_rom_entry [] rom_xevious =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ), /* 64k for the first CPU */
            ROM_LOAD( "xvi_1.3p",     0x0000, 0x1000, CRC("09964dda") + SHA1("4882b25b0938a903f3a367455ba788a30759b5b0") ),
            ROM_LOAD( "xvi_2.3m",     0x1000, 0x1000, CRC("60ecce84") + SHA1("8adc60a5fcbca74092518dbc570ffff0f04c5b17") ),
            ROM_LOAD( "xvi_3.2m",     0x2000, 0x1000, CRC("79754b7d") + SHA1("c6a154858716e1f073b476824b183de20e06d093") ),
            ROM_LOAD( "xvi_4.2l",     0x3000, 0x1000, CRC("c7d4bbf0") + SHA1("4b846de204d08651253d3a141677c8a31626af07") ),

            ROM_REGION( 0x10000, "sub", 0 ), /* 64k for the second CPU */
            ROM_LOAD( "xvi_5.3f",     0x0000, 0x1000, CRC("c85b703f") + SHA1("15f1c005b9d806a384ab1f2240b9c580bfe83893") ),
            ROM_LOAD( "xvi_6.3j",     0x1000, 0x1000, CRC("e18cdaad") + SHA1("6b79efee1a9642edb9f752101737132401248aed") ),

            ROM_REGION( 0x10000, "sub2", 0 ),
            ROM_LOAD( "xvi_7.2c",     0x0000, 0x1000, CRC("dd35cf1c") + SHA1("f8d1f8e019d8198308443c2e7e815d0d04b23d14") ),

            ROM_REGION( 0x1000, "gfx1", 0 ),
            ROM_LOAD( "xvi_12.3b",    0x0000, 0x1000, CRC("088c8b26") + SHA1("9c3b61dfca2f84673a78f7f66e363777a8f47a59") ),    /* foreground characters */

            ROM_REGION( 0x2000, "gfx2", 0 ),
            ROM_LOAD( "xvi_13.3c",    0x0000, 0x1000, CRC("de60ba25") + SHA1("32bc09be5ff8b52ee3a26e0ac3ebc2d4107badb7") ),    /* bg pattern B0 */
            ROM_LOAD( "xvi_14.3d",    0x1000, 0x1000, CRC("535cdbbc") + SHA1("fb9ffe5fc43e0213231267e98d605d43c15f61e8") ),    /* bg pattern B1 */

            ROM_REGION( 0xa000, "gfx3", 0 ),
            ROM_LOAD( "xvi_15.4m",    0x0000, 0x2000, CRC("dc2c0ecb") + SHA1("19ddbd9805f77f38c9a9a1bb30dba6c720b8609f") ),    /* sprite set #1, planes 0/1 */
            ROM_LOAD( "xvi_17.4p",    0x2000, 0x2000, CRC("dfb587ce") + SHA1("acff2bf5cde85a16cdc98a52cdea11f77fadf25a") ),    /* sprite set #2, planes 0/1 */
            ROM_LOAD( "xvi_16.4n",    0x4000, 0x1000, CRC("605ca889") + SHA1("3bf380ef76c03822a042ecc73b5edd4543c268ce") ),    /* sprite set #3, planes 0/1 */
            ROM_LOAD( "xvi_18.4r",    0x5000, 0x2000, CRC("02417d19") + SHA1("b5f830dd2cf25cf154308d2e640f0ecdcda5d8cd") ),    /* sprite set #1, plane 2, set #2, plane 2 */
            /* 0x7000-0x8fff  will be unpacked from 0x5000-0x6fff */
            ROM_FILL(                 0x9000, 0x1000, 0x00 ),    // empty space to decode sprite set #3 as 3 bits per pixel

            ROM_REGION( 0x4000, "gfx4", 0 ), /* background tilemaps */
            ROM_LOAD( "xvi_9.2a",     0x0000, 0x1000, CRC("57ed9879") + SHA1("3106d1aacff06cf78371bd19967141072b32b7d7") ),
            ROM_LOAD( "xvi_10.2b",    0x1000, 0x2000, CRC("ae3ba9e5") + SHA1("49064b25667ffcd81137cd5e800df4b78b182a46") ),
            ROM_LOAD( "xvi_11.2c",    0x3000, 0x1000, CRC("31e244dd") + SHA1("3f7eac12863697a98e1122111801606759e44b2a") ),

            ROM_REGION( 0x0b00, "proms", 0 ),
            ROM_LOAD( "xvi-8.6a",     0x0000, 0x0100, CRC("5cc2727f") + SHA1("0dc1e63a47a4cb0ba75f6f1e0c15e408bb0ee2a1") ), /* palette red component */
            ROM_LOAD( "xvi-9.6d",     0x0100, 0x0100, CRC("5c8796cc") + SHA1("63015e3c0874afc6b1ca032f1ffb8f90562c77c8") ), /* palette green component */
            ROM_LOAD( "xvi-10.6e",    0x0200, 0x0100, CRC("3cb60975") + SHA1("c94d5a5dd4d8a08d6d39c051a4a722581b903f45") ), /* palette blue component */
            ROM_LOAD( "xvi-7.4h",     0x0300, 0x0200, CRC("22d98032") + SHA1("ec6626828c79350417d08b98e9631ad35edd4a41") ), /* bg tiles lookup table low bits */
            ROM_LOAD( "xvi-6.4f",     0x0500, 0x0200, CRC("3a7599f0") + SHA1("a4bdf58c190ca16fc7b976c97f41087a61fdb8b8") ), /* bg tiles lookup table high bits */
            ROM_LOAD( "xvi-4.3l",     0x0700, 0x0200, CRC("fd8b9d91") + SHA1("87ddf0b9d723aabb422d6d416aa9ec6bc246bf34") ), /* sprite lookup table low bits */
            ROM_LOAD( "xvi-5.3m",     0x0900, 0x0200, CRC("bf906d82") + SHA1("776168a73d3b9f0ce05610acc8a623deae0a572b") ), /* sprite lookup table high bits */

            ROM_REGION( 0x0200, "pals_vidbd", 0), /* PAL's located on the video board */
            ROM_LOAD( "xvi-3.1f",     0x0000, 0x0117, CRC("9192d57a") + SHA1("5f36db93b6083767f93aa3a0e4bc2d4fc7e27f9c") ), /* N82S153N */

            ROM_REGION( 0x0200, "namco", 0 ),    /* sound PROMs */
            ROM_LOAD( "xvi-2.7n",     0x0000, 0x0100, CRC("550f06bc") + SHA1("816a0fafa0b084ac11ae1af70a5186539376fc2a") ),
            ROM_LOAD( "xvi-1.5n",     0x0100, 0x0100, CRC("77245b66") + SHA1("0c4d0bee858b97632411c440bea6948a74759746") ),    /* timing - not used */

            ROM_END,
        };


        //ROM_START( digdug )
        static readonly tiny_rom_entry [] rom_digdug =
        {
            ROM_REGION( 0x10000, "maincpu", 0 ), /* 64k for code for the first CPU  */
            ROM_LOAD( "dd1a.1",       0x0000, 0x1000, CRC("a80ec984") + SHA1("86689980410b9429cd7582c7a76342721c87d030") ),
            ROM_LOAD( "dd1a.2",       0x1000, 0x1000, CRC("559f00bd") + SHA1("fde17785df21956d6fd06bcfe675c392dadb1524") ),
            ROM_LOAD( "dd1a.3",       0x2000, 0x1000, CRC("8cbc6fe1") + SHA1("57b8a5777f8bb9773caf0cafe5408c8b9768cb25") ),
            ROM_LOAD( "dd1a.4",       0x3000, 0x1000, CRC("d066f830") + SHA1("b0a615fe4a5c8742c1e4ef234ef34c369d2723b9") ),

            ROM_REGION( 0x10000, "sub", 0 ), /* 64k for the second CPU */
            ROM_LOAD( "dd1a.5",       0x0000, 0x1000, CRC("6687933b") + SHA1("c16144de7633595ddc1450ddce379f48e7b2195a") ),
            ROM_LOAD( "dd1a.6",       0x1000, 0x1000, CRC("843d857f") + SHA1("89b2ead7e478e119d33bfd67376cdf28f83de67a") ),

            ROM_REGION( 0x10000, "sub2", 0 ), /* 64k for the third CPU  */
            ROM_LOAD( "dd1.7",        0x0000, 0x1000, CRC("a41bce72") + SHA1("2b9b74f56aa7939d9d47cf29497ae11f10d78598") ),

            ROM_REGION( 0x1000, "gfx1", 0 ),
            ROM_LOAD( "dd1.9",        0x0000, 0x0800, CRC("f14a6fe1") + SHA1("0aa63300c2cb887196de590aceb98f3cf06fead4") ),

            ROM_REGION( 0x4000, "gfx2", 0 ),
            ROM_LOAD( "dd1.15",       0x0000, 0x1000, CRC("e22957c8") + SHA1("4700c63f4f680cb8ab8c44e6f3e1712aabd5daa4") ),
            ROM_LOAD( "dd1.14",       0x1000, 0x1000, CRC("2829ec99") + SHA1("3e435c1afb2e44487cd7ba28a93ada2e5ccbb86d") ),
            ROM_LOAD( "dd1.13",       0x2000, 0x1000, CRC("458499e9") + SHA1("578bd839f9218c3cf4feee1223a461144e455df8") ),
            ROM_LOAD( "dd1.12",       0x3000, 0x1000, CRC("c58252a0") + SHA1("bd79e39e8a572d2b5c205e6de27ca23e43ec9f51") ),

            ROM_REGION( 0x1000, "gfx3", 0 ),
            ROM_LOAD( "dd1.11",       0x0000, 0x1000, CRC("7b383983") + SHA1("57f1e8f5171d13f9f76bd091d81b4423b59f6b42") ),

            ROM_REGION( 0x1000, "gfx4", 0 ), /* 4k for the playfield graphics */
            ROM_LOAD( "dd1.10b",      0x0000, 0x1000, CRC("2cf399c2") + SHA1("317c48818992f757b1bd0e3997fa99937f81b52c") ),

            ROM_REGION( 0x0220, "proms", 0 ),
            ROM_LOAD( "136007.113",   0x0000, 0x0020, CRC("4cb9da99") + SHA1("91a5852a15d4672c29fdcbae75921794651f960c") ),
            ROM_LOAD( "136007.111",   0x0020, 0x0100, CRC("00c7c419") + SHA1("7ea149e8eb36920c3b84984b5ce623729d492fd3") ),
            ROM_LOAD( "136007.112",   0x0120, 0x0100, CRC("e9b3e08e") + SHA1("a294cc4da846eb702d61678396bfcbc87d30ea95") ),

            ROM_REGION( 0x0200, "namco", 0 ),    /* sound prom */
            ROM_LOAD( "136007.110",   0x0000, 0x0100, CRC("7a2815b4") + SHA1("085ada18c498fdb18ecedef0ea8fe9217edb7b46") ),
            ROM_LOAD( "136007.109",   0x0100, 0x0100, CRC("77245b66") + SHA1("0c4d0bee858b97632411c440bea6948a74759746") ),    /* timing - not used */

            ROM_END,
        };
    }


    partial class galaga_state : driver_device
    {
        public void init_galaga()
        {
            /* swap bytes for flipped character so we can decode them together with normal characters */
            Pointer<uint8_t> rom = new Pointer<uint8_t>(memregion("gfx1").base_());  //uint8_t *rom = memregion("gfx1")->base();
            int len = (int)memregion("gfx1").bytes();

            for (int i = 0; i < len; i++)
            {
                if ((i & 0x0808) == 0x0800)
                {
                    int t = rom[i];
                    rom[i] = rom[i + 8];
                    rom[i+8] = (uint8_t)t;
                }
            }
        }
    }


    partial class xevious_state : galaga_state
    {
        public void init_xevious()
        {
            Pointer<uint8_t> rom = new Pointer<uint8_t>(memregion("gfx3").base_(), 0x5000);  //uint8_t *rom = memregion("gfx3")->base() + 0x5000;
            for (int i = 0; i < 0x2000; i++)
                rom[i + 0x2000] = (uint8_t)(rom[i] >> 4);
        }
    }


    partial class galaga : construct_ioport_helper
    {
        static void galaga_state_galaga(machine_config config, device_t device) { galaga_state galaga_state = (galaga_state)device; galaga_state.galaga(config); }
        static void xevious_state_xevious(machine_config config, device_t device) { xevious_state xevious_state = (xevious_state)device; xevious_state.xevious(config); }
        static void digdug_state_digdug(machine_config config, device_t device) { digdug_state digdug_state = (digdug_state)device; digdug_state.digdug(config); }
        static void galaga_state_init_galaga(device_t owner) { galaga_state galaga_state = (galaga_state)owner; galaga_state.init_galaga(); }
        static void xevious_state_init_xevious(device_t owner) { xevious_state xevious_state = (xevious_state)owner; xevious_state.init_xevious(); }


        static galaga m_galaga = new galaga();


        static device_t device_creator_galaga(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new galaga_state(mconfig, type, tag); }
        static device_t device_creator_xevious(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new xevious_state(mconfig, type, tag); }
        static device_t device_creator_digdug(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new digdug_state(mconfig, type, tag); }


        /* Original Namco hardware, with Namco Customs */

        //                                                        creator,                rom          YEAR,   NAME,       PARENT,  MACHINE,               INPUT,                              INIT,                       MONITOR, COMPANY, FULLNAME,                FLAGS
        public static readonly game_driver driver_galaga  = GAME( device_creator_galaga,  rom_galaga,  "1981", "galaga",   "0",     galaga_state_galaga,   m_galaga.construct_ioport_galaga,   galaga_state_init_galaga,   ROT90,   "Namco", "Galaga (Namco rev. B)", MACHINE_SUPPORTS_SAVE );
        public static readonly game_driver driver_xevious = GAME( device_creator_xevious, rom_xevious, "1982", "xevious",  "0",     xevious_state_xevious, m_galaga.construct_ioport_xevious,  xevious_state_init_xevious, ROT90,   "Namco", "Xevious (Namco)",       MACHINE_SUPPORTS_SAVE );
        public static readonly game_driver driver_digdug  = GAME( device_creator_digdug,  rom_digdug,  "1982", "digdug",   "0",     digdug_state_digdug,   m_galaga.construct_ioport_digdug,   driver_device.empty_init,   ROT90,   "Namco", "Dig Dug (rev 2)",       MACHINE_SUPPORTS_SAVE );
    }
}
