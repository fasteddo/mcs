// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    // ======================> device_disasm_interface
    // class representing interface-specific live disasm
    public abstract class device_disasm_interface : device_interface
    {
        //std::unique_ptr<util::disasm_interface> m_disasm;
        //dasm_override_delegate m_dasm_override;
        //bool m_started;


        // construction/destruction
        public device_disasm_interface(machine_config mconfig, device_t device)
            : base(device, "disasm")
        {
        }

        //~device_disasm_interface() { }


        // Override
        //template <typename... T> void set_dasm_override(T &&... args) { m_dasm_override.set(std::forward<T>(args)...); }


        // disassembler request
        //util.disasm_interface get_disassembler()
        //{
        //    if (!m_disasm)
        //    {
        //        if (m_dasm_override.isnull())
        //            m_disasm.reset(create_disassembler());
        //        else
        //            m_disasm = std::make_unique<device_disasm_indirect>(create_disassembler(), m_dasm_override);
        //    }
        //
        //    return m_disasm.get();
        //}


        // disassembler creation
        protected abstract util.disasm_interface create_disassembler();


        // interface-level overrides
        public override void interface_pre_start()
        {
            //throw new emu_unimplemented();
#if false
            m_dasm_override.bind_relative_to(*device().owner());
#endif
        }
    }
}
