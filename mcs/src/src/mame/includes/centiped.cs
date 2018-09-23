// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;
using uint8_t = System.Byte;


namespace mame
{
    partial class centiped_state : driver_device
    {
        optional_shared_ptr_byte m_rambase;
        required_shared_ptr_byte m_videoram;
        required_shared_ptr_byte m_spriteram;
        optional_shared_ptr_byte m_paletteram;
        optional_shared_ptr_byte m_bullsdrt_tiles_bankram;

        required_device<cpu_device> m_maincpu;
        required_device<ls259_device> m_outlatch;
        optional_device<er2055_device> m_earom;
        optional_device<eeprom_serial_93cxx_device> m_eeprom;
        required_device<gfxdecode_device> m_gfxdecode;
        required_device<screen_device> m_screen;
        required_device<palette_device> m_palette;
        optional_device<ay8910_device> m_aysnd;

        uint8_t [] m_oldpos = new uint8_t[4];
        uint8_t [] m_sign = new uint8_t[4];
        uint8_t m_dsw_select;
        //uint8_t m_control_select;
        uint8_t m_flipscreen;
        uint8_t m_prg_bank;
        uint8_t m_gfx_bank;
        uint8_t m_bullsdrt_sprites_bank;
        uint8_t [] m_penmask = new uint8_t[64];
        tilemap_t m_bg_tilemap;


        public centiped_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_rambase = new optional_shared_ptr_byte(this, "rambase");
            m_videoram = new required_shared_ptr_byte(this, "videoram");
            m_spriteram = new required_shared_ptr_byte(this, "spriteram");
            m_paletteram = new optional_shared_ptr_byte(this, "paletteram");
            m_bullsdrt_tiles_bankram = new optional_shared_ptr_byte(this, "bullsdrt_bank");
            m_maincpu = new required_device<cpu_device>(this, "maincpu");
            m_outlatch = new required_device<ls259_device>(this, "outlatch");
            m_earom = new optional_device<er2055_device>(this, "earom");
            m_eeprom = new optional_device<eeprom_serial_93cxx_device>(this, "eeprom");
            m_gfxdecode = new required_device<gfxdecode_device>(this, "gfxdecode");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new required_device<palette_device>(this, "palette");
            m_aysnd = new optional_device<ay8910_device>(this, "aysnd");
        }


        public required_device<ls259_device> outlatch { get { return m_outlatch; } }


        //void centiped_base(machine_config &config);
        //void milliped(machine_config &config);
        //void bullsdrt(machine_config &config);
        //void centipdb(machine_config &config);
        //void magworm(machine_config &config);
        //void caterplr(machine_config &config);
        //void centiped(machine_config &config);
        //void centipedj(machine_config &config);
        //void mazeinv(machine_config &config);
        //void warlords(machine_config &config);
        //void multiped(machine_config &config);

        //void init_multiped();
        //void init_bullsdrt();

        // drivers/centiped.cpp
        //DECLARE_WRITE8_MEMBER(irq_ack_w);
        //DECLARE_READ8_MEMBER(centiped_IN0_r);
        //DECLARE_READ8_MEMBER(centiped_IN2_r);
        //DECLARE_READ8_MEMBER(milliped_IN1_r);
        //DECLARE_READ8_MEMBER(milliped_IN2_r);
        //DECLARE_WRITE_LINE_MEMBER(input_select_w);
        //DECLARE_WRITE_LINE_MEMBER(control_select_w);
        //DECLARE_READ8_MEMBER(mazeinv_input_r);
        //DECLARE_WRITE8_MEMBER(mazeinv_input_select_w);
        //DECLARE_READ8_MEMBER(bullsdrt_data_port_r);
        //DECLARE_WRITE_LINE_MEMBER(coin_counter_left_w);
        //DECLARE_WRITE_LINE_MEMBER(coin_counter_center_w);
        //DECLARE_WRITE_LINE_MEMBER(coin_counter_right_w);
        //DECLARE_WRITE_LINE_MEMBER(bullsdrt_coin_count_w);
        //DECLARE_READ8_MEMBER(earom_read);
        //DECLARE_WRITE8_MEMBER(earom_write);
        //DECLARE_WRITE8_MEMBER(earom_control_w);
        //DECLARE_READ8_MEMBER(caterplr_unknown_r);
        //DECLARE_WRITE8_MEMBER(caterplr_AY8910_w);
        //DECLARE_READ8_MEMBER(caterplr_AY8910_r);
        //DECLARE_READ8_MEMBER(multiped_eeprom_r);
        //DECLARE_WRITE8_MEMBER(multiped_eeprom_w);
        //DECLARE_WRITE8_MEMBER(multiped_prgbank_w);

        // video/centiped.c
        //DECLARE_WRITE8_MEMBER(centiped_videoram_w);
        //DECLARE_WRITE_LINE_MEMBER(flip_screen_w);
        //DECLARE_WRITE8_MEMBER(multiped_gfxbank_w);
        //DECLARE_WRITE8_MEMBER(bullsdrt_tilesbank_w);
        //DECLARE_WRITE8_MEMBER(bullsdrt_sprites_bank_w);
        //DECLARE_WRITE8_MEMBER(centiped_paletteram_w);
        //DECLARE_WRITE8_MEMBER(milliped_paletteram_w);
        //DECLARE_WRITE8_MEMBER(mazeinv_paletteram_w);
        //TILE_GET_INFO_MEMBER(centiped_get_tile_info);
        //TILE_GET_INFO_MEMBER(warlords_get_tile_info);
        //TILE_GET_INFO_MEMBER(milliped_get_tile_info);
        //TILE_GET_INFO_MEMBER(bullsdrt_get_tile_info);
        //DECLARE_MACHINE_START(centiped);
        //DECLARE_MACHINE_RESET(centiped);
        //DECLARE_VIDEO_START(centiped);
        //DECLARE_VIDEO_START(bullsdrt);
        //DECLARE_MACHINE_RESET(magworm);
        //DECLARE_VIDEO_START(milliped);
        //DECLARE_VIDEO_START(warlords);
        //DECLARE_PALETTE_INIT(warlords);
        //uint32_t screen_update_centiped(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
        //uint32_t screen_update_bullsdrt(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
        //uint32_t screen_update_milliped(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
        //uint32_t screen_update_warlords(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect);
        //TIMER_DEVICE_CALLBACK_MEMBER(generate_interrupt);
        //void init_penmask();
        //void init_common();
        //void milliped_set_color(offs_t offset, uint8_t data);
        //inline int read_trackball(int idx, int switch_port);
        //void bullsdrt_data_map(address_map &map);
        //void bullsdrt_map(address_map &map);
        //void bullsdrt_port_map(address_map &map);
        //void caterplr_map(address_map &map);
        //void centipdb_map(address_map &map);
        //void centiped_base_map(address_map &map);
        //void centiped_map(address_map &map);
        //void centipedj_map(address_map &map);
        //void magworm_map(address_map &map);
        //void mazeinv_map(address_map &map);
        //void milliped_map(address_map &map);
        //void multiped_map(address_map &map);
        //void warlords_map(address_map &map);


        // wrappers because I don't know how to find the correct device during construct_ startup

        //READ8_MEMBER( pokey_device::read )
        public u8 pokey_device_read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            pokey_device pokey = (pokey_device)subdevice("pokey");
            return pokey.read(space, offset, mem_mask);
        }

        //WRITE8_MEMBER( pokey_device::write )
        public void pokey_device_write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            pokey_device pokey = (pokey_device)subdevice("pokey");
            pokey.write(space, offset, data, mem_mask);
        }

        //WRITE8_MEMBER( watchdog_timer_device::reset_w )
        public void watchdog_timer_device_reset_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            watchdog_timer_device watchdog = (watchdog_timer_device)machine().config().device_find(this, "watchdog");
            watchdog.reset_w(space, offset, data, mem_mask);
        }

        //WRITE8_MEMBER(addressable_latch_device::write_d7)
        public void ls259_device_write_d7_outlatch(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            addressable_latch_device device = (addressable_latch_device)subdevice("outlatch");
            device.write_d7(space, offset, data, mem_mask);
        }
    }
}
