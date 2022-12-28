// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using PointerU8 = mame.Pointer<System.Byte>;
using u8 = System.Byte;


namespace mame
{
    public class object_finder_operations_ad7533_device : object_finder_operations<ad7533_device>
    {
        public ad7533_device cast(device_t device) { return (ad7533_device)device; }
        public ad7533_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<ad7533_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_adc0804_device : object_finder_operations<adc0804_device>
    {
        public adc0804_device cast(device_t device) { return (adc0804_device)device; }
        public adc0804_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<adc0804_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_address_map_bank_device : object_finder_operations<address_map_bank_device>
    {
        public address_map_bank_device cast(device_t device) { return (address_map_bank_device)device; }
        public address_map_bank_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<address_map_bank_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_atari_motion_objects_device : object_finder_operations<atari_motion_objects_device>
    {
        public atari_motion_objects_device cast(device_t device) { return (atari_motion_objects_device)device; }
        public atari_motion_objects_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<atari_motion_objects_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_atari_slapstic_device : object_finder_operations<atari_slapstic_device>
    {
        public atari_slapstic_device cast(device_t device) { return (atari_slapstic_device)device; }
        public atari_slapstic_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<atari_slapstic_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_ay8910_device : object_finder_operations<ay8910_device>
    {
        public ay8910_device cast(device_t device) { return (ay8910_device)device; }
        public ay8910_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<ay8910_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_bally_squawk_n_talk_device : object_finder_operations<bally_squawk_n_talk_device>
    {
        public bally_squawk_n_talk_device cast(device_t device) { return (bally_squawk_n_talk_device)device; }
        public bally_squawk_n_talk_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<bally_squawk_n_talk_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_device_palette_interface : object_finder_operations<device_palette_interface>
    {
        public device_palette_interface cast(device_t device) { throw new emu_unimplemented(); }
        public device_palette_interface cast(device_interface device) { return (device_palette_interface)device; }
        public Pointer<device_palette_interface> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_er2055_device : object_finder_operations<er2055_device>
    {
        public er2055_device cast(device_t device) { return (er2055_device)device; }
        public er2055_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<er2055_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_filter_biquad_device : object_finder_operations<filter_biquad_device>
    {
        public filter_biquad_device cast(device_t device) { return (filter_biquad_device)device; }
        public filter_biquad_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<filter_biquad_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_fixedfreq_device : object_finder_operations<fixedfreq_device>
    {
        public fixedfreq_device cast(device_t device) { return (fixedfreq_device)device; }
        public fixedfreq_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<fixedfreq_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_address_space : object_finder_operations<address_space>
    {
        public address_space cast(device_t device) { throw new emu_unimplemented(); }
        public address_space cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<address_space> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_memory_bank : object_finder_operations<memory_bank>
    {
        public memory_bank cast(device_t device) { throw new emu_unimplemented(); }
        public memory_bank cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<memory_bank> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_memory_region : object_finder_operations<memory_region>
    {
        public memory_region cast(device_t device) { throw new emu_unimplemented(); }
        public memory_region cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<memory_region> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_ioport_port : object_finder_operations<ioport_port>
    {
        public ioport_port cast(device_t device) { throw new emu_unimplemented(); }
        public ioport_port cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<ioport_port> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_dac_byte_interface : object_finder_operations<dac_byte_interface>
    {
        public dac_byte_interface cast(device_t device) { throw new emu_unimplemented(); }
        public dac_byte_interface cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<dac_byte_interface> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_cpu_device : object_finder_operations<cpu_device>
    {
        public cpu_device cast(device_t device) { return (cpu_device)device; }
        public cpu_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<cpu_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_dac_8bit_r2r_device : object_finder_operations<dac_8bit_r2r_device>
    {
        public dac_8bit_r2r_device cast(device_t device) { return (dac_8bit_r2r_device)device; }
        public dac_8bit_r2r_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<dac_8bit_r2r_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_dac_16bit_r2r_twos_complement_device : object_finder_operations<dac_16bit_r2r_twos_complement_device>
    {
        public dac_16bit_r2r_twos_complement_device cast(device_t device) { return (dac_16bit_r2r_twos_complement_device)device; }
        public dac_16bit_r2r_twos_complement_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<dac_16bit_r2r_twos_complement_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_deco_cpu7_device : object_finder_operations<deco_cpu7_device>
    {
        public deco_cpu7_device cast(device_t device) { return (deco_cpu7_device)device; }
        public deco_cpu7_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<deco_cpu7_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_digitalker_device : object_finder_operations<digitalker_device>
    {
        public digitalker_device cast(device_t device) { return (digitalker_device)device; }
        public digitalker_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<digitalker_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_discrete_device : object_finder_operations<discrete_device>
    {
        public discrete_device cast(device_t device) { return (discrete_device)device; }
        public discrete_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<discrete_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_discrete_sound_device : object_finder_operations<discrete_sound_device>
    {
        public discrete_sound_device cast(device_t device) { return (discrete_sound_device)device; }
        public discrete_sound_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<discrete_sound_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_dvg_device : object_finder_operations<dvg_device>
    {
        public dvg_device cast(device_t device) { return (dvg_device)device; }
        public dvg_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<dvg_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_eeprom_serial_93cxx_device : object_finder_operations<eeprom_serial_93cxx_device>
    {
        public eeprom_serial_93cxx_device cast(device_t device) { return (eeprom_serial_93cxx_device)device; }
        public eeprom_serial_93cxx_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<eeprom_serial_93cxx_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_generic_latch_8_device : object_finder_operations<generic_latch_8_device>
    {
        public generic_latch_8_device cast(device_t device) { return (generic_latch_8_device)device; }
        public generic_latch_8_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<generic_latch_8_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_gunfight_audio_device : object_finder_operations<gunfight_audio_device>
    {
        public gunfight_audio_device cast(device_t device) { return (gunfight_audio_device)device; }
        public gunfight_audio_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<gunfight_audio_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_gfxdecode_device : object_finder_operations<gfxdecode_device>
    {
        public gfxdecode_device cast(device_t device) { return (gfxdecode_device)device; }
        public gfxdecode_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<gfxdecode_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_i8080_cpu_device : object_finder_operations<i8080_cpu_device>
    {
        public i8080_cpu_device cast(device_t device) { return (i8080_cpu_device)device; }
        public i8080_cpu_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<i8080_cpu_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_i8255_device : object_finder_operations<i8255_device>
    {
        public i8255_device cast(device_t device) { return (i8255_device)device; }
        public i8255_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<i8255_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_i8257_device : object_finder_operations<i8257_device>
    {
        public i8257_device cast(device_t device) { return (i8257_device)device; }
        public i8257_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<i8257_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_i8751_device : object_finder_operations<i8751_device>
    {
        public i8751_device cast(device_t device) { return (i8751_device)device; }
        public i8751_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<i8751_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_input_merger_device : object_finder_operations<input_merger_device>
    {
        public input_merger_device cast(device_t device) { return (input_merger_device)device; }
        public input_merger_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<input_merger_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_latch8_device : object_finder_operations<latch8_device>
    {
        public latch8_device cast(device_t device) { return (latch8_device)device; }
        public latch8_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<latch8_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_ls157_device : object_finder_operations<ls157_device>
    {
        public ls157_device cast(device_t device) { return (ls157_device)device; }
        public ls157_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<ls157_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_ls259_device : object_finder_operations<ls259_device>
    {
        public ls259_device cast(device_t device) { return (ls259_device)device; }
        public ls259_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<ls259_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_m6502_device : object_finder_operations<m6502_device>
    {
        public m6502_device cast(device_t device) { return (m6502_device)device; }
        public m6502_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<m6502_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_m6803_cpu_device : object_finder_operations<m6803_cpu_device>
    {
        public m6803_cpu_device cast(device_t device) { return (m6803_cpu_device)device; }
        public m6803_cpu_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<m6803_cpu_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_m6808_cpu_device : object_finder_operations<m6808_cpu_device>
    {
        public m6808_cpu_device cast(device_t device) { return (m6808_cpu_device)device; }
        public m6808_cpu_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<m6808_cpu_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_m68705p_device : object_finder_operations<m68705p_device>
    {
        public m68705p_device cast(device_t device) { return (m68705p_device)device; }
        public m68705p_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<m68705p_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_mathbox_device : object_finder_operations<mathbox_device>
    {
        public mathbox_device cast(device_t device) { return (mathbox_device)device; }
        public mathbox_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<mathbox_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_mb14241_device : object_finder_operations<mb14241_device>
    {
        public mb14241_device cast(device_t device) { return (mb14241_device)device; }
        public mb14241_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<mb14241_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_mb88_cpu_device : object_finder_operations<mb88_cpu_device>
    {
        public mb88_cpu_device cast(device_t device) { return (mb88_cpu_device)device; }
        public mb88_cpu_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<mb88_cpu_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_mc6809e_device : object_finder_operations<mc6809e_device>
    {
        public mc6809e_device cast(device_t device) { return (mc6809e_device)device; }
        public mc6809e_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<mc6809e_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_mcs48_cpu_device : object_finder_operations<mcs48_cpu_device>
    {
        public mcs48_cpu_device cast(device_t device) { return (mcs48_cpu_device)device; }
        public mcs48_cpu_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<mcs48_cpu_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_midway_cheap_squeak_deluxe_device : object_finder_operations<midway_cheap_squeak_deluxe_device>
    {
        public midway_cheap_squeak_deluxe_device cast(device_t device) { return (midway_cheap_squeak_deluxe_device)device; }
        public midway_cheap_squeak_deluxe_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<midway_cheap_squeak_deluxe_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_midway_sounds_good_device : object_finder_operations<midway_sounds_good_device>
    {
        public midway_sounds_good_device cast(device_t device) { return (midway_sounds_good_device)device; }
        public midway_sounds_good_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<midway_sounds_good_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_midway_ssio_device : object_finder_operations<midway_ssio_device>
    {
        public midway_ssio_device cast(device_t device) { return (midway_ssio_device)device; }
        public midway_ssio_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<midway_ssio_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_midway_turbo_cheap_squeak_device : object_finder_operations<midway_turbo_cheap_squeak_device>
    {
        public midway_turbo_cheap_squeak_device cast(device_t device) { return (midway_turbo_cheap_squeak_device)device; }
        public midway_turbo_cheap_squeak_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<midway_turbo_cheap_squeak_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_msm5205_device : object_finder_operations<msm5205_device>
    {
        public msm5205_device cast(device_t device) { return (msm5205_device)device; }
        public msm5205_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<msm5205_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_namco_device : object_finder_operations<namco_device>
    {
        public namco_device cast(device_t device) { return (namco_device)device; }
        public namco_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<namco_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_nesapu_device : object_finder_operations<nesapu_device>
    {
        public nesapu_device cast(device_t device) { return (nesapu_device)device; }
        public nesapu_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<nesapu_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_netlist_mame_device : object_finder_operations<netlist_mame_device>
    {
        public netlist_mame_device cast(device_t device) { return (netlist_mame_device)device; }
        public netlist_mame_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<netlist_mame_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_netlist_mame_logic_input_device : object_finder_operations<netlist_mame_logic_input_device>
    {
        public netlist_mame_logic_input_device cast(device_t device) { return (netlist_mame_logic_input_device)device; }
        public netlist_mame_logic_input_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<netlist_mame_logic_input_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_netlist_mame_sound_device : object_finder_operations<netlist_mame_sound_device>
    {
        public netlist_mame_sound_device cast(device_t device) { return (netlist_mame_sound_device)device; }
        public netlist_mame_sound_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<netlist_mame_sound_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_palette_device : object_finder_operations<palette_device>
    {
        public palette_device cast(device_t device) { return (palette_device)device; }
        public palette_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<palette_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_pokey_device : object_finder_operations<pokey_device>
    {
        public pokey_device cast(device_t device) { return (pokey_device)device; }
        public pokey_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<pokey_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_pia6821_device : object_finder_operations<pia6821_device>
    {
        public pia6821_device cast(device_t device) { return (pia6821_device)device; }
        public pia6821_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<pia6821_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }


    public class object_finder_operations_rp2a03_device : object_finder_operations<rp2a03_device>
    {
        public rp2a03_device cast(device_t device) { return (rp2a03_device)device; }
        public rp2a03_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<rp2a03_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_samples_device : object_finder_operations<samples_device>
    {
        public samples_device cast(device_t device) { return (samples_device)device; }
        public samples_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<samples_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_screen_device : object_finder_operations<screen_device>
    {
        public screen_device cast(device_t device) { return (screen_device)device; }
        public screen_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<screen_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_sn76477_device : object_finder_operations<sn76477_device>
    {
        public sn76477_device cast(device_t device) { return (sn76477_device)device; }
        public sn76477_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<sn76477_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_starfield_05xx_device : object_finder_operations<starfield_05xx_device>
    {
        public starfield_05xx_device cast(device_t device) { return (starfield_05xx_device)device; }
        public starfield_05xx_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<starfield_05xx_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_t11_device : object_finder_operations<t11_device>
    {
        public t11_device cast(device_t device) { return (t11_device)device; }
        public t11_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<t11_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_ttl153_device : object_finder_operations<ttl153_device>
    {
        public ttl153_device cast(device_t device) { return (ttl153_device)device; }
        public ttl153_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<ttl153_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_ttl7474_device : object_finder_operations<ttl7474_device>
    {
        public ttl7474_device cast(device_t device) { return (ttl7474_device)device; }
        public ttl7474_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<ttl7474_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_vector_device : object_finder_operations<vector_device>
    {
        public vector_device cast(device_t device) { return (vector_device)device; }
        public vector_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<vector_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_taito_sj_security_mcu_device : object_finder_operations<taito_sj_security_mcu_device>
    {
        public taito_sj_security_mcu_device cast(device_t device) { return (taito_sj_security_mcu_device)device; }
        public taito_sj_security_mcu_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<taito_sj_security_mcu_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_tilemap_device : object_finder_operations<tilemap_device>
    {
        public tilemap_device cast(device_t device) { return (tilemap_device)device; }
        public tilemap_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<tilemap_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_timeplt_audio_device : object_finder_operations<timeplt_audio_device>
    {
        public timeplt_audio_device cast(device_t device) { return (timeplt_audio_device)device; }
        public timeplt_audio_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<timeplt_audio_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_tms5220c_device : object_finder_operations<tms5220c_device>
    {
        public tms5220c_device cast(device_t device) { return (tms5220c_device)device; }
        public tms5220c_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<tms5220c_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_watchdog_timer_device : object_finder_operations<watchdog_timer_device>
    {
        public watchdog_timer_device cast(device_t device) { return (watchdog_timer_device)device; }
        public watchdog_timer_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<watchdog_timer_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_ym2151_device : object_finder_operations<ym2151_device>
    {
        public ym2151_device cast(device_t device) { return (ym2151_device)device; }
        public ym2151_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<ym2151_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_z80_device : object_finder_operations<z80_device>
    {
        public z80_device cast(device_t device) { return (z80_device)device; }
        public z80_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<z80_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_z8002_device : object_finder_operations<z8002_device>
    {
        public z8002_device cast(device_t device) { return (z8002_device)device; }
        public z8002_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<z8002_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_z80ctc_channel_device : object_finder_operations<z80ctc_channel_device>
    {
        public z80ctc_channel_device cast(device_t device) { return (z80ctc_channel_device)device; }
        public z80ctc_channel_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<z80ctc_channel_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_z80ctc_device : object_finder_operations<z80ctc_device>
    {
        public z80ctc_device cast(device_t device) { return (z80ctc_device)device; }
        public z80ctc_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<z80ctc_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_z80dma_device : object_finder_operations<z80dma_device>
    {
        public z80dma_device cast(device_t device) { return (z80dma_device)device; }
        public z80dma_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<z80dma_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_z80pio_device : object_finder_operations<z80pio_device>
    {
        public z80pio_device cast(device_t device) { return (z80pio_device)device; }
        public z80pio_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<z80pio_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_intref : object_finder_operations<intref>
    {
        public intref cast(device_t device) { throw new emu_unimplemented(); }
        public intref cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<intref> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_u8 : object_finder_operations<u8>
    {
        public u8 cast(device_t device) { throw new emu_unimplemented(); }
        public u8 cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<u8> cast(PointerU8 memory) { return (Pointer<u8>)memory; }
    }


    public static class object_finder_operations_global
    {
        public static void init()
        {
            object_finder_operations_helper.get_object_finder_operations = (typeof_ObjectClass) =>
            {
                if      (typeof_ObjectClass == typeof(ad7533_device))              return new object_finder_operations_ad7533_device();
                else if (typeof_ObjectClass == typeof(adc0804_device))             return new object_finder_operations_adc0804_device();
                else if (typeof_ObjectClass == typeof(address_map_bank_device))    return new object_finder_operations_address_map_bank_device();
                else if (typeof_ObjectClass == typeof(atari_motion_objects_device)) return new object_finder_operations_atari_motion_objects_device();
                else if (typeof_ObjectClass == typeof(atari_slapstic_device))      return new object_finder_operations_atari_slapstic_device();
                else if (typeof_ObjectClass == typeof(ay8910_device))              return new object_finder_operations_ay8910_device();
                else if (typeof_ObjectClass == typeof(bally_squawk_n_talk_device)) return new object_finder_operations_bally_squawk_n_talk_device();
                else if (typeof_ObjectClass == typeof(cpu_device))                 return new object_finder_operations_cpu_device();
                else if (typeof_ObjectClass == typeof(dac_8bit_r2r_device))        return new object_finder_operations_dac_8bit_r2r_device();
                else if (typeof_ObjectClass == typeof(dac_16bit_r2r_twos_complement_device)) return new object_finder_operations_dac_16bit_r2r_twos_complement_device();
                else if (typeof_ObjectClass == typeof(deco_cpu7_device))           return new object_finder_operations_deco_cpu7_device();
                else if (typeof_ObjectClass == typeof(digitalker_device))          return new object_finder_operations_digitalker_device();
                else if (typeof_ObjectClass == typeof(discrete_device))            return new object_finder_operations_discrete_device();
                else if (typeof_ObjectClass == typeof(discrete_sound_device))      return new object_finder_operations_discrete_sound_device();
                else if (typeof_ObjectClass == typeof(dvg_device))                 return new object_finder_operations_dvg_device();
                else if (typeof_ObjectClass == typeof(eeprom_serial_93cxx_device)) return new object_finder_operations_eeprom_serial_93cxx_device();
                else if (typeof_ObjectClass == typeof(er2055_device))              return new object_finder_operations_er2055_device();
                else if (typeof_ObjectClass == typeof(filter_biquad_device))       return new object_finder_operations_filter_biquad_device();
                else if (typeof_ObjectClass == typeof(fixedfreq_device))           return new object_finder_operations_fixedfreq_device();
                else if (typeof_ObjectClass == typeof(generic_latch_8_device))     return new object_finder_operations_generic_latch_8_device();
                else if (typeof_ObjectClass == typeof(gunfight_audio_device))      return new object_finder_operations_gunfight_audio_device();
                else if (typeof_ObjectClass == typeof(gfxdecode_device))           return new object_finder_operations_gfxdecode_device();
                else if (typeof_ObjectClass == typeof(i8080_cpu_device))           return new object_finder_operations_i8080_cpu_device();
                else if (typeof_ObjectClass == typeof(i8255_device))               return new object_finder_operations_i8255_device();
                else if (typeof_ObjectClass == typeof(i8257_device))               return new object_finder_operations_i8257_device();
                else if (typeof_ObjectClass == typeof(i8751_device))               return new object_finder_operations_i8751_device();
                else if (typeof_ObjectClass == typeof(input_merger_device))        return new object_finder_operations_input_merger_device();
                else if (typeof_ObjectClass == typeof(latch8_device))              return new object_finder_operations_latch8_device();
                else if (typeof_ObjectClass == typeof(ls157_device))               return new object_finder_operations_ls157_device();
                else if (typeof_ObjectClass == typeof(ls259_device))               return new object_finder_operations_ls259_device();
                else if (typeof_ObjectClass == typeof(m6502_device))               return new object_finder_operations_m6502_device();
                else if (typeof_ObjectClass == typeof(m6803_cpu_device))           return new object_finder_operations_m6803_cpu_device();
                else if (typeof_ObjectClass == typeof(m6808_cpu_device))           return new object_finder_operations_m6808_cpu_device();
                else if (typeof_ObjectClass == typeof(m68705p_device))             return new object_finder_operations_m68705p_device();
                else if (typeof_ObjectClass == typeof(mathbox_device))             return new object_finder_operations_mathbox_device();
                else if (typeof_ObjectClass == typeof(mb14241_device))             return new object_finder_operations_mb14241_device();
                else if (typeof_ObjectClass == typeof(mb88_cpu_device))            return new object_finder_operations_mb88_cpu_device();
                else if (typeof_ObjectClass == typeof(mc6809e_device))             return new object_finder_operations_mc6809e_device();
                else if (typeof_ObjectClass == typeof(mcs48_cpu_device))           return new object_finder_operations_mcs48_cpu_device();
                else if (typeof_ObjectClass == typeof(midway_cheap_squeak_deluxe_device)) return new object_finder_operations_midway_cheap_squeak_deluxe_device();
                else if (typeof_ObjectClass == typeof(midway_sounds_good_device))  return new object_finder_operations_midway_sounds_good_device();
                else if (typeof_ObjectClass == typeof(midway_ssio_device))         return new object_finder_operations_midway_ssio_device();
                else if (typeof_ObjectClass == typeof(midway_turbo_cheap_squeak_device)) return new object_finder_operations_midway_turbo_cheap_squeak_device();
                else if (typeof_ObjectClass == typeof(msm5205_device))             return new object_finder_operations_msm5205_device();
                else if (typeof_ObjectClass == typeof(rp2a03_device))              return new object_finder_operations_rp2a03_device();
                else if (typeof_ObjectClass == typeof(namco_device))               return new object_finder_operations_namco_device();
                else if (typeof_ObjectClass == typeof(nesapu_device))              return new object_finder_operations_nesapu_device();
                else if (typeof_ObjectClass == typeof(netlist_mame_device))        return new object_finder_operations_netlist_mame_device();
                else if (typeof_ObjectClass == typeof(netlist_mame_logic_input_device)) return new object_finder_operations_netlist_mame_logic_input_device();
                else if (typeof_ObjectClass == typeof(netlist_mame_sound_device))  return new object_finder_operations_netlist_mame_sound_device();
                else if (typeof_ObjectClass == typeof(palette_device))             return new object_finder_operations_palette_device();
                else if (typeof_ObjectClass == typeof(pokey_device))               return new object_finder_operations_pokey_device();
                else if (typeof_ObjectClass == typeof(pia6821_device))             return new object_finder_operations_pia6821_device();
                else if (typeof_ObjectClass == typeof(samples_device))             return new object_finder_operations_samples_device();
                else if (typeof_ObjectClass == typeof(screen_device))              return new object_finder_operations_screen_device();
                else if (typeof_ObjectClass == typeof(sn76477_device))             return new object_finder_operations_sn76477_device();
                else if (typeof_ObjectClass == typeof(starfield_05xx_device))      return new object_finder_operations_starfield_05xx_device();
                else if (typeof_ObjectClass == typeof(t11_device))                 return new object_finder_operations_t11_device();
                else if (typeof_ObjectClass == typeof(taito_sj_security_mcu_device)) return new object_finder_operations_taito_sj_security_mcu_device();
                else if (typeof_ObjectClass == typeof(tilemap_device))             return new object_finder_operations_tilemap_device();
                else if (typeof_ObjectClass == typeof(timeplt_audio_device))       return new object_finder_operations_timeplt_audio_device();
                else if (typeof_ObjectClass == typeof(tms5220c_device))            return new object_finder_operations_tms5220c_device();
                else if (typeof_ObjectClass == typeof(ttl153_device))              return new object_finder_operations_ttl153_device();
                else if (typeof_ObjectClass == typeof(ttl7474_device))             return new object_finder_operations_ttl7474_device();
                else if (typeof_ObjectClass == typeof(vector_device))              return new object_finder_operations_vector_device();
                else if (typeof_ObjectClass == typeof(watchdog_timer_device))      return new object_finder_operations_watchdog_timer_device();
                else if (typeof_ObjectClass == typeof(ym2151_device))              return new object_finder_operations_ym2151_device();
                else if (typeof_ObjectClass == typeof(z80_device))                 return new object_finder_operations_z80_device();
                else if (typeof_ObjectClass == typeof(z8002_device))               return new object_finder_operations_z8002_device();
                else if (typeof_ObjectClass == typeof(z80ctc_channel_device))      return new object_finder_operations_z80ctc_channel_device();
                else if (typeof_ObjectClass == typeof(z80ctc_device))              return new object_finder_operations_z80ctc_device();
                else if (typeof_ObjectClass == typeof(z80dma_device))              return new object_finder_operations_z80dma_device();
                else if (typeof_ObjectClass == typeof(z80pio_device))              return new object_finder_operations_z80pio_device();

                else if (typeof_ObjectClass == typeof(device_palette_interface))   return new object_finder_operations_device_palette_interface();
                else if (typeof_ObjectClass == typeof(address_space))              return new object_finder_operations_address_space();
                else if (typeof_ObjectClass == typeof(memory_bank))                return new object_finder_operations_memory_bank();
                else if (typeof_ObjectClass == typeof(memory_region))              return new object_finder_operations_memory_region();
                else if (typeof_ObjectClass == typeof(ioport_port))                return new object_finder_operations_ioport_port();
                else if (typeof_ObjectClass == typeof(dac_byte_interface))         return new object_finder_operations_dac_byte_interface();
                else if (typeof_ObjectClass == typeof(intref))                     return new object_finder_operations_intref();
                else if (typeof_ObjectClass == typeof(u8))                         return new object_finder_operations_u8();

                else throw new emu_unimplemented();
            };
        }
    }
}
