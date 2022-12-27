// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.decocpu7_global;
using static mame.device_global;
using static mame.util;


namespace mame
{
    public class deco_cpu7_device : m6502_device
    {
        //DEFINE_DEVICE_TYPE(DECO_CPU7, deco_cpu7_device, "decocpu7", "DECO CPU-7")
        public static readonly emu.detail.device_type_impl DECO_CPU7 = DEFINE_DEVICE_TYPE("decocpu7", "DECO CPU-7", (type, mconfig, tag, owner, clock) => { return new deco_cpu7_device(mconfig, tag, owner, clock); });


        deco_cpu7_device(machine_config mconfig, string tag, device_t owner, uint32_t clock) :
            base(mconfig, DECO_CPU7, tag, owner, clock)
        {
        }


        class mi_decrypt : mi_default
        {
            public bool had_written;


            //virtual ~mi_decrypt() {}


            public override uint8_t read_sync(uint16_t adr)
            {
                uint8_t res = cprogram.read_byte(adr);

                if (had_written)
                {
                    had_written = false;
                    if ((adr & 0x0104) == 0x0104)
                        res = (uint8_t)bitswap(res, 6,5,3,4,2,7,1,0);
                }

                return res;
            }


            public override void write(uint16_t adr, uint8_t val)
            {
                program.write_byte(adr, val);
                had_written = true;
            }
        }


        //class disassembler : public m6502_disassembler {
        //public:
        //    mi_decrypt *mintf;
        //
        //    disassembler(mi_decrypt *m);
        //    virtual ~disassembler() = default;
        //    virtual u32 interface_flags() const override;
        //    virtual u8 decrypt8(u8 value, offs_t pc, bool opcode) const override;
        //};


        protected override void device_start()
        {
            mintf = new mi_decrypt();  //mintf = std::make_unique<mi_decrypt>();
            init();
        }


        protected override void device_reset()
        {
            base.device_reset();
            ((mi_decrypt)mintf).had_written = false;
        }


        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;
    }


    static class decocpu7_global
    {
        public static deco_cpu7_device DECO_CPU7<bool_Required>(machine_config mconfig, device_finder<deco_cpu7_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, deco_cpu7_device.DECO_CPU7, clock); }
    }
}
