// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using optional_memory_bank = mame.memory_bank_finder<mame.bool_const_false>;  //using optional_memory_bank = memory_bank_finder<false>;
using u8 = System.Byte;

using static mame.emucore_global;


namespace mame
{
    partial class asteroid_state : driver_device
    {
        /* devices */
        required_device<m6502_device> m_maincpu;  //required_device<cpu_device> m_maincpu;
        required_device<dvg_device> m_dvg;
        optional_device<er2055_device> m_earom;
        required_device<discrete_sound_device> m_discrete;  //required_device<discrete_device> m_discrete;
        required_ioport m_dsw1;
        required_device<ttl153_device> m_dsw_sel;
        optional_ioport m_cocktail;

        /* memory banks */
        optional_memory_bank m_ram1;
        optional_memory_bank m_ram2;
        memory_share_creator<u8> m_sram1;
        memory_share_creator<u8> m_sram2;


        public asteroid_state(machine_config mconfig, device_type type, string tag) :
            base(mconfig, type, tag)
        {
            m_maincpu = new required_device<m6502_device>(this, "maincpu");
            m_dvg = new required_device<dvg_device>(this, "dvg");
            m_earom = new optional_device<er2055_device>(this, "earom");
            m_discrete = new required_device<discrete_sound_device>(this, "discrete");
            m_dsw1 = new required_ioport(this, "DSW1");
            m_dsw_sel = new required_device<ttl153_device>(this, "dsw_sel");
            m_cocktail = new optional_ioport(this, "COCKTAIL");
            m_ram1 = new optional_memory_bank(this, "ram1");
            m_ram2 = new optional_memory_bank(this, "ram2");
            m_sram1 = new memory_share_creator<u8>(this, "ram1", 0x100, ENDIANNESS_LITTLE);
            m_sram2 = new memory_share_creator<u8>(this, "ram2", 0x100, ENDIANNESS_LITTLE);
        }
    }
}
