// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame
{
    public static class language_global
    {
        static Dictionary<string, string> g_translation = new Dictionary<string, string>();


        const UInt32 MO_MAGIC = 0x950412de;
        const UInt32 MO_MAGIC_REVERSED = 0xde120495;


        static UInt32 endianchange(UInt32 value)
        {
            UInt32 b0 = (value >> 0) & 0xff;
            UInt32 b1 = (value >> 8) & 0xff;
            UInt32 b2 = (value >> 16) & 0xff;
            UInt32 b3 = (value >> 24) & 0xff;

            return (b0 << 24) | (b1 << 16) | (b2 << 8) | b3;
        }


        public static void load_translation(emu_options m_options)
        {
            g_translation.Clear();
            emu_file file = new emu_file(m_options.language_path(), osdcore_global.OPEN_FLAG_READ);
            var name = m_options.language();
            name = name.Replace(" ", "_");
            name = name.Replace("(", "");
            name = name.Replace(")", "");
            if (file.open(name, osdcore_global.PATH_SEPARATOR + "strings.mo") == osd_file.error.NONE)
            {
                uint64_t size = file.size();
                RawBuffer buffer = new RawBuffer(4 * (int)size / 4 + 1); //uint32_t *buffer = global_alloc_array(uint32_t, size / 4 + 1);
                file.read(new ListBytesPointer(buffer), (UInt32)size);
                file.close();

                if (buffer.get_uint32(0) != MO_MAGIC && buffer.get_uint32(0) != MO_MAGIC_REVERSED)
                {
                    buffer = null;  //global_free_array(buffer);
                    return;
                }
                if (buffer.get_uint32(0) == MO_MAGIC_REVERSED)
                {
                    for (var i = 0; i < ((int)size / 4) + 1; ++i)
                    {
                        buffer.set_uint32(i, endianchange(buffer[i]));
                    }
                }

                uint32_t number_of_strings = buffer.get_uint32(2);
                uint32_t original_table_offset = buffer.get_uint32(3) >> 2;
                uint32_t translation_table_offset = buffer.get_uint32(4) >> 2;

                RawBuffer data = buffer;  //const char *data = reinterpret_cast<const char*>(buffer);

                for (var i = 1; i < number_of_strings; ++i)
                {
                    string original = "TODO original";  //(const char *)data + buffer[original_table_offset + 2 * i + 1];
                    string translation = "TODO translation";  //(const char *)data + buffer[translation_table_offset + 2 * i + 1];
                    g_translation.Add(original, translation);
                }

                buffer = null;  //global_free_array(buffer);
            }
        }
    }
}
