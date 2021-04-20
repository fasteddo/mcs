// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    /* Error types */
    public enum png_error
    {
        PNGERR_NONE,
        PNGERR_OUT_OF_MEMORY,
        PNGERR_UNKNOWN_FILTER,
        PNGERR_FILE_ERROR,
        PNGERR_BAD_SIGNATURE,
        PNGERR_DECOMPRESS_ERROR,
        PNGERR_FILE_TRUNCATED,
        PNGERR_FILE_CORRUPT,
        PNGERR_UNKNOWN_CHUNK,
        PNGERR_COMPRESS_ERROR,
        PNGERR_UNSUPPORTED_FORMAT
    }


    public static class png_global
    {
        //png_error png_read_bitmap(util::core_file &fp, bitmap_argb32 &bitmap);

        //png_error png_write_bitmap(util::core_file &fp, png_info *info, bitmap_t &bitmap, int palette_length, const rgb_t *palette);

        //png_error mng_capture_start(util::core_file &fp, bitmap_t &bitmap, unsigned rate);
        public static png_error mng_capture_frame(util.core_file fp, png_info info, bitmap_t bitmap, int palette_length, List<rgb_t> palette) { throw new emu_unimplemented(); }
        public static png_error mng_capture_stop(util.core_file fp) { throw new emu_unimplemented(); }
    }


    public class png_info
    {
        //using png_text = std::pair<std::string, std::string>;

        //std::unique_ptr<std::uint8_t []>    image;
        //std::uint32_t                       width, height;
        //std::uint32_t                       xres = 0, yres = 0;
        //rectangle                           screen;
        //double                              xscale = 0, yscale = 0;
        //double                              source_gamma = 0;
        //std::uint32_t                       resolution_unit = 0;
        //std::uint8_t                        bit_depth = 0;
        //std::uint8_t                        color_type = 0;
        //std::uint8_t                        compression_method = 0;
        //std::uint8_t                        filter_method = 0;
        //std::uint8_t                        interlace_method = 0;

        //std::unique_ptr<std::uint8_t []>    palette = 0;
        //std::uint32_t                       num_palette = 0;

        //std::unique_ptr<std::uint8_t []>    trans = 0;
        //std::uint32_t                       num_trans = 0;

        //std::list<png_text>                 textlist;

        //~png_info() { free_data(); }

        //png_error read_file(util::core_file &fp);
        //png_error copy_to_bitmap(bitmap_argb32 &bitmap, bool &hasalpha);
        //png_error expand_buffer_8bit();

        public png_error add_text(string keyword, string text) { throw new emu_unimplemented(); }

        //void free_data();
        //void reset() { free_data(); operator=(png_info()); }

        //static png_error verify_header(util::core_file &fp);

        //png_info &operator=(png_info &&) = default;
    }
}
