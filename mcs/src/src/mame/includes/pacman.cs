// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;


namespace mame
{
    partial class pacman_state : driver_device
    {
        required_device<cpu_device> m_maincpu;
        optional_device<ls259_device> m_mainlatch;
        optional_device<namco_device> m_namco_sound;
        required_device<watchdog_timer_device> m_watchdog;
        optional_shared_ptr_uint8_t  m_spriteram;
        optional_shared_ptr_uint8_t  m_spriteram2;
        optional_shared_ptr_uint8_t  m_s2650_spriteram;
        required_shared_ptr_uint8_t  m_videoram;
        optional_shared_ptr_uint8_t  m_colorram;
        optional_shared_ptr_uint8_t  m_s2650games_tileram;
        optional_shared_ptr_uint8_t  m_rocktrv2_prot_data;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<palette_device> m_palette;


        public pacman_state(machine_config mconfig, device_type type, string tag) 
            : base(mconfig, type, tag)
        {
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_mainlatch = new optional_device<ls259_device>(this, "mainlatch");
            m_namco_sound = new optional_device<namco_device>(this, "namco");
            m_watchdog = new required_device<watchdog_timer_device>(this, "watchdog");
            m_spriteram = new optional_shared_ptr_uint8_t(this, "spriteram");
            m_spriteram2 = new optional_shared_ptr_uint8_t(this, "spriteram2");
            m_s2650_spriteram = new optional_shared_ptr_uint8_t(this, "s2650_spriteram");
            m_videoram = new required_shared_ptr_uint8_t(this, "videoram");
            m_colorram = new optional_shared_ptr_uint8_t(this, "colorram");
            m_s2650games_tileram = new optional_shared_ptr_uint8_t(this, "s2650_tileram");
            m_rocktrv2_prot_data = new optional_shared_ptr_uint8_t(this, "rocktrv2_prot");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_palette = new required_device<palette_device>(this, "palette");
        }


        //UINT8 m_cannonb_bit_to_read;
        //int m_mystery;
        //UINT8 m_counter;
        //int m_bigbucks_bank;
        //UINT8 m_rocktrv2_question_bank;
        tilemap_t m_bg_tilemap;
        byte m_charbank;
        byte m_spritebank;
        byte m_palettebank;
        byte m_colortablebank;
        byte m_flipscreen;
        byte m_bgpriority;
        int m_xoffsethack;
        byte m_inv_spr;
        //uint8_t m_maketrax_counter;
        //uint8_t m_maketrax_offset;
        //int m_maketrax_disable_protection;

        byte m_irq_mask;


        public required_device<cpu_device> maincpu { get { return m_maincpu; } }
        public optional_device<ls259_device> mainlatch { get { return m_mainlatch; } }
        public optional_device<namco_device> namco_sound { get { return m_namco_sound; } }
        public required_device<watchdog_timer_device> watchdog { get { return m_watchdog; } }
        public required_device<gfxdecode_device> gfxdecode { get { return m_gfxdecode; } }
        public required_device<palette_device> palette { get { return m_palette; } }


        //DECLARE_WRITE8_MEMBER(pacman_interrupt_vector_w);
        //DECLARE_WRITE8_MEMBER(piranha_interrupt_vector_w);
        //DECLARE_WRITE8_MEMBER(nmouse_interrupt_vector_w);
        //DECLARE_WRITE_LINE_MEMBER(coin_counter_w);
        //DECLARE_WRITE_LINE_MEMBER(coin_lockout_global_w);


        //DECLARE_PALETTE_INIT(pacman);
        //video\pacman.cs


        //DECLARE_WRITE_LINE_MEMBER(vblank_irq);
        //INTERRUPT_GEN_MEMBER(periodic_irq);
        //DECLARE_WRITE_LINE_MEMBER(rocktrv2_vblank_irq);
        //DECLARE_WRITE_LINE_MEMBER(vblank_nmi);
        //DECLARE_WRITE_LINE_MEMBER(s2650_interrupt);


        //DECLARE_READ8_MEMBER(pacman_read_nop);
        //DECLARE_READ8_MEMBER(mspacman_disable_decode_r_0x0038);
        //DECLARE_READ8_MEMBER(mspacman_disable_decode_r_0x03b0);
        //DECLARE_READ8_MEMBER(mspacman_disable_decode_r_0x1600);
        //DECLARE_READ8_MEMBER(mspacman_disable_decode_r_0x2120);
        //DECLARE_READ8_MEMBER(mspacman_disable_decode_r_0x3ff0);
        //DECLARE_READ8_MEMBER(mspacman_disable_decode_r_0x8000);
        //DECLARE_READ8_MEMBER(mspacman_disable_decode_r_0x97f0);
        //DECLARE_WRITE8_MEMBER(mspacman_disable_decode_w);
        //DECLARE_READ8_MEMBER(mspacman_enable_decode_r_0x3ff8);
        //DECLARE_WRITE8_MEMBER(mspacman_enable_decode_w);


        //void birdiy(machine_config &config);
        //void rocktrv2(machine_config &config);
        //void mspacman(machine_config &config);
        //void dremshpr(machine_config &config);
        //void mschamp(machine_config &config);
        //void acitya(machine_config &config);
        //void theglobp(machine_config &config);
        //void nmouse(machine_config &config);
        //void vanvan(machine_config &config);
        //void s2650games(machine_config &config);
        //void woodpek(machine_config &config);
        //void crushs(machine_config &config);
        //void eeekk(machine_config &config);
        //void superabc(machine_config &config);
        //void numcrash(machine_config &config);
        //void crush4(machine_config &config);
        //void bigbucks(machine_config &config);
        //void porky(machine_config &config);
        //void pacman(machine_config &config);
        //void _8bpm(machine_config &config);
        //void maketrax(machine_config &config);
        //void korosuke(machine_config &config);
        //void alibaba(machine_config &config);
        //void drivfrcp(machine_config &config);
        //void pengojpm(machine_config &config);
        //void piranha(machine_config &config);

        //void _8bpm_portmap(address_map &map);
        //void alibaba_map(address_map &map);
        //void bigbucks_map(address_map &map);
        //void bigbucks_portmap(address_map &map);
        //void birdiy_map(address_map &map);
        //void crushs_map(address_map &map);
        //void crushs_portmap(address_map &map);
        //void dremshpr_map(address_map &map);
        //void dremshpr_portmap(address_map &map);
        //void drivfrcp_portmap(address_map &map);
        //void epos_map(address_map &map);
        //void epos_portmap(address_map &map);
        //void mschamp_map(address_map &map);
        //void mschamp_portmap(address_map &map);
        //void mspacman_map(address_map &map);
        //void nmouse_portmap(address_map &map);
        //void numcrash_map(address_map &map);
        //void pacman_map(address_map &map);
        //void pengojpm_map(address_map &map);
        //void piranha_portmap(address_map &map);
        //void porky_portmap(address_map &map);
        //void rocktrv2_map(address_map &map);
        //void s2650games_dataport(address_map &map);
        //void s2650games_map(address_map &map);
        //void superabc_map(address_map &map);
        //void vanvan_portmap(address_map &map);
        //void woodpek_map(address_map &map);
        //void writeport(address_map &map);


        // wrappers because I don't know how to find the correct device during construct_ startup

        //WRITE_LINE_MEMBER(namco_audio_device::sound_enable_w)
        public void namco_audio_device_sound_enable_w(int state)
        {
            m_namco_sound.target.sound_enable_w(state);
        }

        //WRITE8_MEMBER( namco_device::pacman_sound_w )
        public void namco_device_pacman_sound_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_namco_sound.target.pacman_sound_w(space, offset, data, mem_mask);
        }


        //WRITE8_MEMBER( watchdog_timer_device::reset_w )
        public void watchdog_timer_device_reset_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            watchdog_timer_device watchdog = (watchdog_timer_device)machine().config().device_find(this, "watchdog");
            watchdog.reset_w(space, offset, data, mem_mask);
        }


        //WRITE8_MEMBER(addressable_latch_device::write_d0)
        public void ls259_device_write_d0_mainlatch(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            addressable_latch_device device = (addressable_latch_device)subdevice("mainlatch");
            device.write_d0(space, offset, data, mem_mask);
        }
    }
}
