// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uint32_t = System.UInt32;


namespace mame
{
    public static class language_global
    {
        static std.unordered_map<string, string> g_translation = new std.unordered_map<string, string>();


        const UInt32 MO_MAGIC = 0x950412de;
        const UInt32 MO_MAGIC_REVERSED = 0xde120495;


#if false
        struct cstr_hash
        {
            size_t operator()(char const *s) const noexcept
            {
                // Bernstein string hash
                size_t result(5381);
                while (*s)
                    result = ((result << 5) + result) + u8(*s++);
                return result;
            }
        };

        struct cstr_compare
        {
            size_t operator()(char const *x, char const *y) const noexcept
            {
                return !std::strcmp(x, y);
            }
        };
#endif

        static RawBuffer f_translation_data_buffer;  //std::unique_ptr<u32 []> f_translation_data;
        static UInt32BufferPointer f_translation_data;  //std::unique_ptr<u32 []> f_translation_data;
        static std.unordered_map<string, string> f_translation_map = new std.unordered_map<string, string>();  //std::unordered_map<char const *, char const *, cstr_hash, cstr_compare> f_translation_map;


#if false
        char const *lang_translate(char const *word)
        {
            auto const found = f_translation_map.find(word);
            return (f_translation_map.end() != found) ? found->second : word;
        }
#endif


        public static void load_translation(emu_options m_options)
        {
            f_translation_data = null;  //f_translation_data.reset();
            f_translation_map.clear();

            string name = m_options.language();
            if (name.empty())
                return;

            global_object.strreplace(ref name, " ", "_");
            global_object.strreplace(ref name, "(", "");
            global_object.strreplace(ref name, ")", "");
            emu_file file = new emu_file(m_options.language_path(), global_object.OPEN_FLAG_READ);
            if (file.open(name.c_str(), osdcore_global.PATH_SEPARATOR + "strings.mo") != osd_file.error.NONE)
            {
                global_object.osd_printf_error("Error opening translation file {0}\n", name);
                return;
            }

            u64 size = file.size();
            if (20 > size)
            {
                file.close();
                global_object.osd_printf_error("Error reading translation file {0}: {1}-byte file is too small to contain translation data\n", name, size);
                return;
            }

            f_translation_data_buffer = new RawBuffer((int)size + 3);  //f_translation_data.reset(new uint32_t [(size + 3) / 4]);
            f_translation_data = new UInt32BufferPointer(f_translation_data);
            if (f_translation_data == null)
            {
                file.close();
                global_object.osd_printf_error("Failed to allocate {0} bytes to load translation data file {1}\n", size, name);
                return;
            }

            var read = file.read(new RawBufferPointer(f_translation_data), (u32)size);
            file.close();
            if (read != size)
            {
                global_object.osd_printf_error("Error reading translation file {0}: requested {1} bytes but got {2} bytes\n", name, size, read);
                f_translation_data = null;  //f_translation_data.reset();
                return;
            }

            if ((f_translation_data[0] != MO_MAGIC) && (f_translation_data[0] != MO_MAGIC_REVERSED))
            {
                global_object.osd_printf_error("Error reading translation file {0}: unrecognized magic number 0x{1}\n", name, f_translation_data[0]);
                f_translation_data = null;  //f_translation_data.reset();
                return;
            }

            //auto fetch_word =
            //        [reversed = f_translation_data[0] == MO_MAGIC_REVERSED, words = f_translation_data.get()] (size_t offset)
            //        {
            //            return reversed ? swapendian_int32(words[offset]) : words[offset];
            //        };
            var fetch_word = new Func<UInt32, uint32_t>((offset) =>
            {
                bool reversed = f_translation_data[0] == MO_MAGIC_REVERSED;
                var words = f_translation_data;
                return reversed ? global_object.swapendian_int32(words[offset]) : words[offset];
            });

            // FIXME: check major/minor version number

            if ((fetch_word(3) % 4) != 0 || (fetch_word(4) % 4) != 0)
            {
                global_object.osd_printf_error("Error reading translation file {0}: table offsets {1} and {2} are not word-aligned\n", name, fetch_word(3), fetch_word(4));
                f_translation_data = null;  //f_translation_data.reset();
                return;
            }

            u32 number_of_strings = fetch_word(2);
            u32 original_table_offset = fetch_word(3) >> 2;
            u32 translation_table_offset = fetch_word(4) >> 2;
            if ((4 * (original_table_offset + ((u64)number_of_strings * 2))) > size)
            {
                global_object.osd_printf_error("Error reading translation file {0}: {1}-entry original string table at offset {2} extends past end of {3}-byte file\n", name, number_of_strings, fetch_word(3), size);
                f_translation_data = null;  //f_translation_data.reset();
                return;
            }
            if ((4 * (translation_table_offset + ((u64)number_of_strings * 2))) > size)
            {
                global_object.osd_printf_error("Error reading translation file {0}: {1}-entry translated string table at offset {2} extends past end of {3}-byte file\n", name, number_of_strings, fetch_word(4), size);
                f_translation_data = null;  //f_translation_data.reset();
                return;
            }
            global_object.osd_printf_verbose("Reading translation file {0}: {1} strings, original table at word offset {2}, translated table at word offset {3}\n", name, number_of_strings, original_table_offset, translation_table_offset);

            RawBufferPointer data = new RawBufferPointer(f_translation_data);  //char const *const data = reinterpret_cast<char const *>(f_translation_data.get());
            for (u32 i = 1; number_of_strings > i; ++i)
            {
                u32 original_length = fetch_word(original_table_offset + (2 * i));
                u32 original_offset = fetch_word(original_table_offset + (2 * i) + 1);
                if ((original_length + original_offset) >= size)
                {
                    global_object.osd_printf_error("Error reading translation file {0}: {1}-byte original string {2} at offset {3} extends past end of {4}-byte file\n", name, original_length, i, original_offset, size);
                    continue;
                }
                if (data[(int)(original_length + original_offset)] != 0)
                {
                    global_object.osd_printf_error("Error reading translation file {0}: {1}-byte original string {2} at offset {3} is not correctly NUL-terminated\n", name, original_length, i, original_offset);
                    continue;
                }

                u32 translation_length = fetch_word(translation_table_offset + (2 * i));
                u32 translation_offset = fetch_word(translation_table_offset + (2 * i) + 1);
                if ((translation_length + translation_offset) >= size)
                {
                    global_object.osd_printf_error("Error reading translation file {0}: {1}-byte translated string {2} at offset {3} extends past end of {4}-byte file\n", name, translation_length, i, translation_offset, size);
                    continue;
                }
                if (data[(int)(translation_length + translation_offset)] != 0)
                {
                    global_object.osd_printf_error("Error reading translation file {0}: {1}-byte translated string {2} at offset {3} is not correctly NUL-terminated\n", name, translation_length, i, translation_offset);
                    continue;
                }

                string original = data.get_string((int)original_offset, int.MaxValue);  //string original = &data[original_offset];
                string translation = data.get_string((int)translation_offset, int.MaxValue);  //string translation = &data[translation_offset];
                var ins = f_translation_map.emplace(original, translation);
                if (!ins)
                    global_object.osd_printf_warning("Loading translation file {0}: translation {1} '{2}'='{3}' conflicts with previous translation '{4}'='{5}'\n", name, i, original, translation, null, null);  //ins.first->first, ins.first->second);
            }

            global_object.osd_printf_verbose("Loaded {0} translations from file {1}\n", f_translation_map.size(), name);
        }
    }
}
