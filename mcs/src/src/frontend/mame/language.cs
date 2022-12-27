// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
using PointerU8 = mame.Pointer<System.Byte>;
using size_t = System.UInt64;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uint32_t = System.UInt32;

using static mame.corestr_global;
using static mame.language_global;
using static mame.language_internal;
using static mame.osdcomm_global;
using static mame.osdcore_global;
using static mame.osdfile_global;


namespace mame
{
    public static partial class language_global
    {
        public static void load_translation(emu_options m_options)
        {
            util.unload_translation();

            string name = m_options.language();
            if (name.empty())
                return;

            strreplace(ref name, " ", "_");
            strreplace(ref name, "(", "");
            strreplace(ref name, ")", "");
            emu_file file = new emu_file(m_options.language_path(), OPEN_FLAG_READ);
            if (file.open(name + PATH_SEPARATOR + "strings.mo"))
            {
                osd_printf_error("Error opening translation file {0}\n", name);
                return;
            }

            osd_printf_verbose("Loading translation file {0}\n", file.fullpath());
            util.load_translation(file.core_file_);
        }
    }
}
