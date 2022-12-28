// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u32 = System.UInt32;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.device_global;
using static mame.hash_global;
using static mame.namcoio_global;
using static mame.romentry_global;


namespace mame
{
    /***************************************************************************
        TYPE DEFINITIONS
    ***************************************************************************/
    abstract class namcoio_device : device_t
    {
        //enum {
        protected const int TYPE_NAMCO56XX = 0;
        const int TYPE_NAMCO58XX = 1;
        const int TYPE_NAMCO59XX = 2;
        //};

        // internal state
        //uint8_t          m_ram[16]{};

        devcb_read8.array<u64_const_4> m_in_cb;
        devcb_write8.array<u64_const_2> m_out_cb;

        //int            m_reset = 0;
        //int32_t        m_lastcoins = 0, m_lastbuttons = 0;
        //int32_t        m_credits = 0;
        //int32_t        m_coins[2]{};
        //int32_t        m_coins_per_cred[2]{};
        //int32_t        m_creds_per_coin[2]{};
        //int32_t        m_in_count = 0;

        //int m_device_type;


        protected namcoio_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, int device_type)
            : base(mconfig, type, tag, owner, clock)
        {
            m_in_cb = new devcb_read8.array<u64_const_4>(this, () => { return new devcb_read8(this); });
            m_out_cb = new devcb_write8.array<u64_const_2>(this, () => { return new devcb_write8(this); });
            //, m_device_type(device_type)
        }


        public devcb_read8.binder in_callback<unsigned_N>() where unsigned_N : u32_const, new() { unsigned N = new unsigned_N().value;  return m_in_cb[N].bind(); }  //template <unsigned N> auto in_callback() { return m_in_cb[N].bind(); }
        public devcb_write8.binder out_callback<unsigned_N>() where unsigned_N : u32_const, new() { unsigned N = new unsigned_N().value;  return m_out_cb[N].bind(); }  //template <unsigned N> auto out_callback() { return m_out_cb[N].bind(); }

        //uint8_t read(offs_t offset);
        //void write(offs_t offset, uint8_t data);

        //WRITE_LINE_MEMBER( set_reset_line );
        public void set_reset_line(int state) { throw new emu_unimplemented(); }

        //READ_LINE_MEMBER( read_reset_line );

        protected abstract void customio_run();


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_reset() { throw new emu_unimplemented(); }


        //void handle_coins( int swap );
    }


    class namco56xx_device : namcoio_device
    {
        //DEFINE_DEVICE_TYPE(NAMCO_56XX, namco56xx_device, "namco56", "Namco 56xx I/O")
        public static readonly emu.detail.device_type_impl NAMCO_56XX = DEFINE_DEVICE_TYPE("namco56", "Namco 56xx I/O", (type, mconfig, tag, owner, clock) => { return new namco56xx_device(mconfig, tag, owner, clock); });

        namco56xx_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, NAMCO_56XX, tag, owner, clock, TYPE_NAMCO56XX)
        {
        }


        //ROM_START(namco_56xx)
        static readonly tiny_rom_entry [] rom_namco_56xx =
        {
            ROM_REGION(0x400, "mcu", 0),
            ROM_LOAD("56xx.bin", 0x0000, 0x0400, NO_DUMP),

            ROM_END,
        };


        protected override Pointer<tiny_rom_entry> device_rom_region()
        {
            return new Pointer<tiny_rom_entry>(new MemoryContainer<tiny_rom_entry>(rom_namco_56xx));  //return ROM_NAME(namco_56xx);
        }


        protected override void customio_run() { throw new emu_unimplemented(); }
    }


    //class namco58xx_device : public namcoio_device

    //class namco59xx_device : public namcoio_device


    static partial class namcoio_global
    {
        public static namco56xx_device NAMCO_56XX<bool_Required>(machine_config mconfig, device_finder<namco56xx_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, namco56xx_device.NAMCO_56XX, clock); }
    }
}
