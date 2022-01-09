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
    public static class language_global
    {
        //**************************************************************************
        //  LOCALIZATION SUPPORT
        //**************************************************************************

        public static string __(string message) { return lang_translate(message); }  //#define _(...) lang_translate(__VA_ARGS__)
        public static string __(string context, string message) { return lang_translate(context, message); }  //#define _(...) lang_translate(__VA_ARGS__)

        public static string N_(string msg) { return msg; }  //#define N_(msg) (msg)
        public static string N_p(string ctx, string msg) { return msg; }  //#define N_p(ctx, msg) (msg)


        public static void load_translation(emu_options m_options)
        {
            f_translation_data = null;  //f_translation_data.reset();
            f_translation_map.clear();

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

            u64 size = file.size();
            if (20 > size)
            {
                file.close();
                osd_printf_error("Error reading translation file {0}: {1}-byte file is too small to contain translation data\n", name, size);
                return;
            }

            f_translation_data_buffer = new MemoryU8((int)size + 3, true);  //f_translation_data.reset(new (std::nothrow) u32 [(size + 3) / 4]);
            f_translation_data = new PointerU32(f_translation_data);
            if (f_translation_data == null)
            {
                file.close();
                osd_printf_error("Failed to allocate {0} bytes to load translation data file {1}\n", size, name);
                return;
            }

            var read = file.read(new PointerU8(f_translation_data), (u32)size);
            file.close();
            if (read != size)
            {
                osd_printf_error("Error reading translation file {0}: requested {1} bytes but got {2} bytes\n", name, size, read);
                f_translation_data = null;  //f_translation_data.reset();
                return;
            }

            if ((f_translation_data[0] != MO_MAGIC) && (f_translation_data[0] != MO_MAGIC_REVERSED))
            {
                osd_printf_error("Error reading translation file {0}: unrecognized magic number 0x{1}\n", name, f_translation_data[0]);
                f_translation_data = null;  //f_translation_data.reset();
                return;
            }


            var fetch_word = new Func<size_t, uint32_t>((offset) =>  //auto fetch_word = [reversed = f_translation_data[0] == MO_MAGIC_REVERSED, words = f_translation_data.get()] (size_t offset)
            {
                bool reversed = f_translation_data[0] == MO_MAGIC_REVERSED;
                var words = f_translation_data;
                return reversed ? swapendian_int32(words[offset]) : words[offset];  //return reversed ? swapendian_int32(words[offset]) : words[offset];
            });

            // FIXME: check major/minor version number

            if ((fetch_word(3) % 4) != 0 || (fetch_word(4) % 4) != 0)
            {
                osd_printf_error("Error reading translation file {0}: table offsets {1} and {2} are not word-aligned\n", name, fetch_word(3), fetch_word(4));
                f_translation_data = null;  //f_translation_data.reset();
                return;
            }

            u32 number_of_strings = fetch_word(2);
            u32 original_table_offset = fetch_word(3) >> 2;
            u32 translation_table_offset = fetch_word(4) >> 2;
            if ((4 * (original_table_offset + ((u64)number_of_strings * 2))) > size)
            {
                osd_printf_error("Error reading translation file {0}: {1}-entry original string table at offset {2} extends past end of {3}-byte file\n", name, number_of_strings, fetch_word(3), size);
                f_translation_data = null;  //f_translation_data.reset();
                return;
            }
            if ((4 * (translation_table_offset + ((u64)number_of_strings * 2))) > size)
            {
                osd_printf_error("Error reading translation file {0}: {1}-entry translated string table at offset {2} extends past end of {3}-byte file\n", name, number_of_strings, fetch_word(4), size);
                f_translation_data = null;  //f_translation_data.reset();
                return;
            }
            osd_printf_verbose("Reading translation file {0}: {1} strings, original table at word offset {2}, translated table at word offset {3}\n", name, number_of_strings, original_table_offset, translation_table_offset);

            PointerU8 data = new PointerU8(f_translation_data);  //char const *const data = reinterpret_cast<char const *>(f_translation_data.get());
            for (u32 i = 1; number_of_strings > i; ++i)
            {
                u32 original_length = fetch_word(original_table_offset + (2 * i));
                u32 original_offset = fetch_word(original_table_offset + (2 * i) + 1);
                if ((original_length + original_offset) >= size)
                {
                    osd_printf_error("Error reading translation file {0}: {1}-byte original string {2} at offset {3} extends past end of {4}-byte file\n", name, original_length, i, original_offset, size);
                    continue;
                }
                if (data[(int)(original_length + original_offset)] != 0)
                {
                    osd_printf_error("Error reading translation file {0}: {1}-byte original string {2} at offset {3} is not correctly NUL-terminated\n", name, original_length, i, original_offset);
                    continue;
                }

                u32 translation_length = fetch_word(translation_table_offset + (2 * i));
                u32 translation_offset = fetch_word(translation_table_offset + (2 * i) + 1);
                if ((translation_length + translation_offset) >= size)
                {
                    osd_printf_error("Error reading translation file {0}: {1}-byte translated string {2} at offset {3} extends past end of {4}-byte file\n", name, translation_length, i, translation_offset, size);
                    continue;
                }
                if (data[(int)(translation_length + translation_offset)] != 0)
                {
                    osd_printf_error("Error reading translation file {0}: {1}-byte translated string {2} at offset {3} is not correctly NUL-terminated\n", name, translation_length, i, translation_offset);
                    continue;
                }

                string original = data.ToString((int)original_offset, int.MaxValue);  //std::string_view const original(&data[original_offset], original_length);
                string translation = data.ToString((int)translation_offset, int.MaxValue);  //char const *const translation(&data[translation_offset]);
                var ins = f_translation_map.emplace(translation, std.make_pair(translation, translation_length));  //auto const ins = f_translation_map.emplace(original, std::make_pair(translation, translation_length));
                if (!ins)
                {
                    osd_printf_warning(
                            "Loading translation file {0}: translation {1} '{2}'='{3}' conflicts with previous translation '{4}'='{5}'\n",  //"Loading translation file %s: translation %u '%s'='%s' conflicts with previous translation '%s'='%s'\n",
                            name,
                            i,
                            original,
                            translation,
                            null,  //ins.first->first,
                            null);  //ins.first->second.first);
                }
            }

            osd_printf_verbose("Loaded {0} translations from file {1}\n", f_translation_map.size(), name);
        }


        static string lang_translate(string message)
        {
            var found = f_translation_map.find(message);
            if (default != found)
                return found.first.Substring(0, (int)found.second);  //return std::string_view(found->second.first, found->second.second);

            return message;
        }


        static string lang_translate(string context, string message)
        {
            if (!f_translation_map.empty())
            {
                var ctxlen = std.strlen(context);
                var msglen = std.strlen(message);
                string key = "";
                //key.reserve(ctxlen + 1 + msglen);
                key = key.append_(context, ctxlen);
                key = key.append_(1, '\u0004');
                key = key.append_(message, msglen);
                var found = f_translation_map.find(key);
                if (default != found)
                    return found.first;
            }

            return message;
        }
    }


    static class language_internal
    {
        public const u32 MO_MAGIC = 0x950412de;
        public const u32 MO_MAGIC_REVERSED = 0xde120495;


        static std.unordered_map<string, string> g_translation = new std.unordered_map<string, string>();


        public static MemoryU8 f_translation_data_buffer;  //std::unique_ptr<u32 []> f_translation_data;
        public static PointerU32 f_translation_data;  //std::unique_ptr<u32 []> f_translation_data;
        public static std.unordered_map<string, std.pair<string, u32>> f_translation_map = new std.unordered_map<string, std.pair<string, u32>>();  //std::unordered_map<std::string_view, std::pair<char const *, u32> > f_translation_map;
    }
}
