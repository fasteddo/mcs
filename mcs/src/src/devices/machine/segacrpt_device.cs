// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.segacrpt_device_global;


namespace mame
{
    // base class
    public abstract class segacrpt_z80_device : z80_device
    {
        string m_decrypted_tag = null;
        Pointer<uint8_t> m_decrypted_ptr;  //uint8_t* m_decrypted_ptr;
        Pointer<uint8_t> m_region_ptr;  //uint8_t* m_region_ptr;
        int m_decode_size;
        int m_numbanks;
        int m_banksize;


        bool m_decryption_done;


        protected segacrpt_z80_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock) :
            base(mconfig, type, tag, owner, clock)
        {
            m_decrypted_ptr = null;
            m_region_ptr = null;
            m_decode_size = 0x8000;
            m_numbanks = 0;
            m_banksize = 0;
            m_decryption_done = false;
        }


        public void set_decrypted_tag(string decrypted_tag) { m_decrypted_tag = decrypted_tag; }

        //void set_size(int size) { m_decode_size = size; }
        //void set_numbanks(int numbanks) { m_numbanks = numbanks; }
        //void set_banksize(int banksize) { m_banksize = banksize; }

        //void set_decrypted_p(uint8_t* ptr);
        //void set_region_p(uint8_t* ptr);


        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_reset() { throw new emu_unimplemented(); }


        protected abstract void decrypt();
    }


    // actual encrypted CPUs
    //class sega_315_5132_device : public segacrpt_z80_device

    //class sega_315_5155_device : public segacrpt_z80_device

    //class sega_315_5110_device : public segacrpt_z80_device

    //class sega_315_5135_device : public segacrpt_z80_device

    //class sega_315_5051_device : public segacrpt_z80_device

    //class sega_315_5098_device : public segacrpt_z80_device

    //class sega_315_5102_device : public segacrpt_z80_device

    //class sega_315_5065_device : public segacrpt_z80_device

    //class sega_315_5064_device : public segacrpt_z80_device

    //class sega_315_5033_device : public segacrpt_z80_device

    //class sega_315_5041_device : public segacrpt_z80_device

    //class sega_315_5048_device : public segacrpt_z80_device

    //class sega_315_5093_device : public segacrpt_z80_device

    //class sega_315_5099_device : public segacrpt_z80_device

    //class sega_315_spat_device : public segacrpt_z80_device

    //class sega_315_5015_device : public segacrpt_z80_device

    //class sega_315_5133_device : public sega_315_5048_device

    //class sega_315_5014_device : public segacrpt_z80_device

    //class sega_315_5013_device : public segacrpt_z80_device

    //class sega_315_5061_device : public segacrpt_z80_device

    //class sega_315_5018_device : public segacrpt_z80_device


    public class sega_315_5010_device : segacrpt_z80_device
    {
        //DEFINE_DEVICE_TYPE(SEGA_315_5010, sega_315_5010_device, "sega_315_5010", "Sega 315-5010")
        public static readonly emu.detail.device_type_impl SEGA_315_5010 = DEFINE_DEVICE_TYPE("sega_315_5010", "Sega 315-5010", (type, mconfig, tag, owner, clock) => { return new sega_315_5010_device(mconfig, tag, owner, clock); });


        sega_315_5010_device(machine_config mconfig, string tag, device_t owner, uint32_t clock) : base(mconfig, SEGA_315_5010, tag, owner, clock) {}


        protected override void decrypt() { throw new emu_unimplemented(); }
    }


    //class sega_315_5128_device : public segacrpt_z80_device

    //class sega_315_5028_device : public segacrpt_z80_device

    //class sega_315_5084_device : public segacrpt_z80_device


    public static partial class segacrpt_device_global
    {
        public static sega_315_5010_device SEGA_315_5010<bool_Required>(emu.detail.machine_config_replace replace, device_finder<z80_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return (sega_315_5010_device)emu.detail.device_type_impl.op(replace, finder, sega_315_5010_device.SEGA_315_5010, clock); }
    }
}
