// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
using PointerU8 = mame.Pointer<System.Byte>;
using size_t = System.UInt64;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.language_global;
using static mame.language_internal;
using static mame.osdcomm_global;
using static mame.osdcore_global;


namespace mame
{
    public static partial class language_global
    {
        //**************************************************************************
        //  LOCALIZATION SUPPORT
        //**************************************************************************

        public static string __(string message) { return util.lang_translate(message); }  //#define _(...) lang_translate(__VA_ARGS__)
        public static string __(string context, string message) { return util.lang_translate(context, message); }  //#define _(...) lang_translate(__VA_ARGS__)

        public static string N_(string msg) { return msg; }  //#define N_(msg) (msg)
        public static string N_p(string ctx, string msg) { return msg; }  //#define N_p(ctx, msg) (msg)
    }


    partial class util
    {
        public static void unload_translation()
        {
            f_translation_data = null;  //f_translation_data.reset();
            f_translation_map.clear();
        }

        public static void load_translation(random_read file)
        {
            MemoryU8 translation_data_buffer;  //std::unique_ptr<std::uint32_t []> translation_data;
            PointerU32 translation_data;  //std::unique_ptr<std::uint32_t []> translation_data;
            std.unordered_map<string, std.pair<string, uint32_t>> translation_map = new std.unordered_map<string, std.pair<string, uint32_t>>();  //std::unordered_map<std::string_view, std::pair<char const *, std::uint32_t> > translation_map;

            uint64_t size = 0;
            if (file.length(out size) || (20 > size))
            {
                osd_printf_error("Error reading translation file: {0}-byte file is too small to contain translation data\n", size);
                return;
            }

            translation_data_buffer = new MemoryU8((int)size + 3, true);  //translation_data.reset(new (std::nothrow) std::uint32_t [(size + 3) / 4]);
            translation_data = new PointerU32(f_translation_data);
            if (translation_data == null)
            {
                osd_printf_error("Failed to allocate {0} bytes to load translation data file\n", size);
                return;
            }

            size_t read;
            file.read(translation_data, size, out read);
            if (read != size)
            {
                osd_printf_error("Error reading translation file: requested {0} bytes but got {1} bytes\n", size, read);
                translation_data = null;  //translation_data.reset();
                return;
            }

            if ((translation_data[0] != MO_MAGIC) && (translation_data[0] != MO_MAGIC_REVERSED))
            {
                osd_printf_error("Error reading translation file: unrecognized magic number {0}\n", translation_data[0]);  //0x%08X
                translation_data = null;  //translation_data.reset();
                return;
            }

            var fetch_word = new Func<size_t, uint32_t>(
                    (size_t offset) => //[reversed = translation_data[0] == MO_MAGIC_REVERSED, words = translation_data.get()] (size_t offset)
                    {
                        var reversed = translation_data[0] == MO_MAGIC_REVERSED;
                        var words = translation_data;

                        return reversed ? swapendian_int32(words[offset]) : words[offset];
                    });

            // FIXME: check major/minor version number

            if ((fetch_word(3) % 4) != 0 || (fetch_word(4) % 4) != 0)
            {
                osd_printf_error("Error reading translation file: table offsets {0} and {1} are not word-aligned\n", fetch_word(3), fetch_word(4));
                translation_data = null;  //translation_data.reset();
                return;
            }

            uint32_t number_of_strings = fetch_word(2);
            uint32_t original_table_offset = fetch_word(3) >> 2;
            uint32_t translation_table_offset = fetch_word(4) >> 2;
            if ((4 * (original_table_offset + ((uint64_t)number_of_strings * 2))) > size)
            {
                osd_printf_error("Error reading translation file: {0}-entry original string table at offset {1} extends past end of {2}-byte file\n", number_of_strings, fetch_word(3), size);
                translation_data = null;  //translation_data.reset();
                return;
            }

            if ((4 * (translation_table_offset + ((uint64_t)number_of_strings * 2))) > size)
            {
                osd_printf_error("Error reading translation file: {0}-entry translated string table at offset {1} extends past end of {2}-byte file\n", number_of_strings, fetch_word(4), size);
                translation_data = null;  //translation_data.reset();
                return;
            }

            osd_printf_verbose("Reading translation file: {0} strings, original table at word offset {1}, translated table at word offset {2}\n", number_of_strings, original_table_offset, translation_table_offset);

            PointerU8 data = new PointerU8(translation_data);  //char const *const data = reinterpret_cast<char const *>(translation_data.get());
            for (uint32_t i = 1; number_of_strings > i; ++i)
            {
                uint32_t original_length = fetch_word(original_table_offset + (2 * i));
                uint32_t original_offset = fetch_word(original_table_offset + (2 * i) + 1);
                if ((original_length + original_offset) >= size)
                {
                    osd_printf_error("Error reading translation file: {0}-byte original string {1} at offset {2} extends past end of {3}-byte file\n", original_length, i, original_offset, size);
                    continue;
                }

                if (data[original_length + original_offset] != 0)
                {
                    osd_printf_error("Error reading translation file: {0}-byte original string {1} at offset {2} is not correctly NUL-terminated\n", original_length, i, original_offset);
                    continue;
                }

                uint32_t translation_length = fetch_word(translation_table_offset + (2 * i));
                uint32_t translation_offset = fetch_word(translation_table_offset + (2 * i) + 1);
                if ((translation_length + translation_offset) >= size)
                {
                    osd_printf_error("Error reading translation file: {0}-byte translated string {1} at offset {2} extends past end of {3}-byte file\n", translation_length, i, translation_offset, size);
                    continue;
                }

                if (data[translation_length + translation_offset] != 0)
                {
                    osd_printf_error("Error reading translation file: {0}-byte translated string {1} at offset {2} is not correctly NUL-terminated\n", translation_length, i, translation_offset);
                    continue;
                }

                string original = data.ToString((int)original_offset, int.MaxValue);  //std::string_view const original(&data[original_offset], original_length);
                string translation = data.ToString((int)translation_offset, int.MaxValue);  //char const *const translation(&data[translation_offset]);
                var ins = translation_map.emplace(original, std.make_pair(translation, translation_length));  //auto const ins = translation_map.emplace(original, std::make_pair(translation, translation_length));
                if (!ins)
                {
                    osd_printf_warning(
                            "Loading translation file: translation {0} '{1}'='{2}' conflicts with previous translation '{3}'='{4}'\n",
                            i,
                            original,
                            translation,
                            null,  //ins.first->first,
                            null);  //ins.first->second.first);
                }
            }

            osd_printf_verbose("Loaded {0} translated string from file\n", translation_map.size());
            f_translation_data = translation_data;
            f_translation_map = translation_map;
        }


        //char const *lang_translate(char const *message);
        //std::string_view lang_translate(std::string_view message);
        public static string lang_translate(string message)
        {
            var found = f_translation_map.find(message);
            if (default != found)
                return found.first;
            return message;
        }


        //char const *lang_translate(char const *context, char const *message);
        //std::string_view lang_translate(char const *context, std::string_view message);
        //std::string_view lang_translate(std::string_view context, std::string_view message);
        public static string lang_translate(string context, string message)
        {
            if (!f_translation_map.empty())
            {
                var ctxlen = std.strlen(context);
                var msglen = std.strlen(message);
                string key = "";
                key.reserve(ctxlen + 1 + msglen);
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
        public const uint32_t MO_MAGIC = 0x950412de;
        public const uint32_t MO_MAGIC_REVERSED = 0xde120495;


        public static PointerU32 f_translation_data;  //std::unique_ptr<std::uint32_t []> f_translation_data;
        public static std.unordered_map<string, std.pair<string, uint32_t>> f_translation_map = new std.unordered_map<string, std.pair<string, uint32_t>>();  //std::unordered_map<std::string_view, std::pair<char const *, std::uint32_t> > f_translation_map;
    }
}
