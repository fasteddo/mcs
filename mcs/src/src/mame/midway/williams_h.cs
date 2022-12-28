// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using optional_memory_bank = mame.memory_bank_finder<mame.bool_const_false>;  //using optional_memory_bank = memory_bank_finder<false>;
using PointerU8 = mame.Pointer<System.Byte>;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;

using static mame.williams_global;


namespace mame
{
    // base Williams hardware
    partial class williams_state : driver_device
    {
        //enum
        //{
        //    // controlbyte (0xCA00) bit definitions
        //    WMS_BLITTER_CONTROLBYTE_NO_EVEN = 0x80,
        //    WMS_BLITTER_CONTROLBYTE_NO_ODD = 0x40,
        //    WMS_BLITTER_CONTROLBYTE_SHIFT = 0x20,
        //    WMS_BLITTER_CONTROLBYTE_SOLID = 0x10,
        //    WMS_BLITTER_CONTROLBYTE_FOREGROUND_ONLY = 0x08,
        //    WMS_BLITTER_CONTROLBYTE_SLOW = 0x04, // 2us blits instead of 1us
        //    WMS_BLITTER_CONTROLBYTE_DST_STRIDE_256 = 0x02,
        //    WMS_BLITTER_CONTROLBYTE_SRC_STRIDE_256 = 0x01
        //};


        required_shared_ptr<uint8_t> m_nvram;
        required_shared_ptr<uint8_t> m_videoram;
        optional_memory_bank m_mainbank;
        protected uint8_t m_blitter_config;
        protected uint16_t m_blitter_clip_address;
        uint8_t m_blitter_window_enable;
        uint8_t m_cocktail;
        rgb_t [] m_palette_lookup;  //std::unique_ptr<rgb_t[]> m_palette_lookup;
        uint8_t [] m_blitterram = new uint8_t[8];
        uint8_t m_blitter_xor;
        uint8_t m_blitter_remap_index;
        PointerU8 m_blitter_remap;  //const uint8_t *m_blitter_remap;
        MemoryU8 m_blitter_remap_lookup;  //std::unique_ptr<uint8_t[]> m_blitter_remap_lookup;

        protected required_device<mc6809e_device> m_maincpu;  //required_device<cpu_device> m_maincpu;
        protected required_device<m6808_cpu_device> m_soundcpu;  //required_device<cpu_device> m_soundcpu;
        required_device<watchdog_timer_device> m_watchdog;
        protected required_device<screen_device> m_screen;
        optional_device<palette_device> m_palette;
        optional_shared_ptr<uint8_t> m_paletteram;
        protected optional_device_array<pia6821_device, u32_const_4> m_pia;  //optional_device_array<pia6821_device, 4> m_pia;


        public williams_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_nvram = new required_shared_ptr<uint8_t>(this, "nvram");
            m_videoram = new required_shared_ptr<uint8_t>(this, "videoram");
            m_mainbank = new optional_memory_bank(this, "mainbank");
            m_maincpu = new required_device<mc6809e_device>(this, "maincpu");
            m_soundcpu = new required_device<m6808_cpu_device>(this, "soundcpu");
            m_watchdog = new required_device<watchdog_timer_device>(this, "watchdog");
            m_screen = new required_device<screen_device>(this, "screen");
            m_palette = new optional_device<palette_device>(this, "palette");
            m_paletteram = new optional_shared_ptr<uint8_t>(this, "paletteram");
            m_pia = new optional_device_array<pia6821_device, u32_const_4>(this, "pia_{0}", 0U, (base_, tag_) => { return new device_finder<pia6821_device, bool_const_false>(base_, tag_); });
        }
    }


    // Defender
    partial class defender_state : williams_state
    {
        required_device<address_map_bank_device> m_bankc000;


        public defender_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_bankc000 = new required_device<address_map_bank_device>(this, "bankc000");
        }


        protected override void machine_start() { }
    }


    //class mayday_state : public defender_state


    // Sinistar: blitter window clip
    partial class sinistar_state : williams_state
    {
        public sinistar_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        { }
    }


    //class bubbles_state : public williams_state

    //class playball_state : public williams_state


    // Joust, Splat: muxed inputs
    partial class wms_muxed_state : williams_state
    {
        required_device<ls157_device> m_mux0;
        required_device<ls157_device> m_mux1;


        public wms_muxed_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_mux0 = new required_device<ls157_device>(this, "mux_0");
            m_mux1 = new required_device<ls157_device>(this, "mux_1");
        }
    }


    //class spdball_state : public williams_state

    //class blaster_state : public williams_state

    //class williams2_state : public williams_state

    //class williams_d000_rom_state : public williams2_state

    //class williams_d000_ram_state : public williams2_state

    //class inferno_state : public williams_d000_ram_state

    //class mysticm_state : public williams_d000_ram_state

    //class tshoot_state : public williams_d000_rom_state

    //class joust2_state : public williams_d000_rom_state

    /*----------- defined in video/williams.cpp -----------*/


    static class williams_global
    {
        // blitter type
        //enum 
        //{
        public const int WILLIAMS_BLITTER_NONE       = 0;       /* no blitter */
        public const int WILLIAMS_BLITTER_SC1        = 1;       /* Special Chip 1 blitter */
        //WILLIAMS_BLITTER_SC2 = 2       /* Special Chip 2 "bugfixed" blitter */
        //}
    }
}
