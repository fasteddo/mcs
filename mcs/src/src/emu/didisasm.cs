// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


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
        //template <typename Obj> void set_dasm_override(Obj &&cb) { m_dasm_override = std::forward<Obj>(cb); }
        //void set_dasm_override(dasm_override_delegate callback) { m_dasm_override = callback; }
        //template <class FunctionClass> void set_dasm_override(offs_t (FunctionClass::*callback)(std::ostream &, offs_t,
        //    const util::disasm_interface::data_buffer &, const util::disasm_interface::data_buffer &), const char *name)
        //{
        //    set_dasm_override(dasm_override_delegate(callback, name, nullptr, static_cast<FunctionClass *>(nullptr)));
        //}


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
