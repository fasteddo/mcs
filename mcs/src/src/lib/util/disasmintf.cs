// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    public static partial class util
    {
        // class implementing a disassembler
        public abstract class disasm_interface
        {
            // Disassembler constants for the return value
            //static constexpr u32 SUPPORTED       = 0x80000000;   // are disassembly flags supported?
            //static constexpr u32 STEP_OUT        = 0x40000000;   // this instruction should be the end of a step out sequence
            //static constexpr u32 STEP_OVER       = 0x20000000;   // this instruction should be stepped over by setting a breakpoint afterwards
            //static constexpr u32 STEP_COND       = 0x10000000;   // this instruction may conditionally result in a program transfer or short skip
            //static constexpr u32 OVERINSTMASK    = 0x0c000000;   // number of extra instructions to skip when stepping over
            //static constexpr u32 OVERINSTSHIFT   = 26;           // bits to shift after masking to get the value
            //static constexpr u32 LENGTHMASK      = 0x0000ffff;   // the low 16-bits contain the actual length

            //static inline u32 step_over_extra(u32 x) {
            //    return x << OVERINSTSHIFT;
            //}


            class data_buffer
            {
                //virtual ~data_buffer() = default;
                //virtual u8  r8 (offs_t pc) const = 0;
                //virtual u16 r16(offs_t pc) const = 0;
                //virtual u32 r32(offs_t pc) const = 0;
                //virtual u64 r64(offs_t pc) const = 0;
            }

            //enum
            //{
            //    NONLINEAR_PC        = 0x00000001,
            //    PAGED               = 0x00000002,
            //    PAGED2LEVEL         = 0x00000006,
            //    INTERNAL_DECRYPTION = 0x00000008,
            //    SPLIT_DECRYPTION    = 0x00000018
            //}

            //virtual u32 interface_flags() const;
            //virtual u32 page_address_bits() const;
            //virtual u32 page2_address_bits() const;
            //virtual offs_t pc_linear_to_real(offs_t pc) const;
            //virtual offs_t pc_real_to_linear(offs_t pc) const;
            //virtual u8  decrypt8 (u8  value, offs_t pc, bool opcode) const;
            //virtual u16 decrypt16(u16 value, offs_t pc, bool opcode) const;
            //virtual u32 decrypt32(u32 value, offs_t pc, bool opcode) const;
            //virtual u64 decrypt64(u64 value, offs_t pc, bool opcode) const;

            //virtual u32 opcode_alignment() const = 0;
            //virtual offs_t disassemble(std::ostream &stream, offs_t pc, const data_buffer &opcodes, const data_buffer &params) = 0;
        }
    }
}
