// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;

using int16_t = System.Int16;
using int32_t = System.Int32;
using size_t = System.UInt64;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.osdcomm_global;


namespace mame
{
    partial class util
    {
        public class wav_file
        {
            public FileStream file;  // FILE *file;
            public BinaryWriter writer;
            public uint32_t total_offs;
            public uint32_t data_offs;
        }


        //struct wav_deleter


        public static wav_file wav_open(string filename, int sample_rate, int channels)
        {
            uint32_t temp32;
            uint16_t temp16;

            // allocate memory for the wav struct
            var wav = new wav_file();  //auto *const wav = new (std::nothrow) wav_file;
            if (wav == null)
                return null;

            // create the file */
            //wav.file = fopen(filename, "wb");
            //if (!wav.file)
            //{
            //    //global_free(wav);
            //    return null;
            //}
            wav.file = new FileStream(filename, FileMode.Create);
            wav.writer = new BinaryWriter(wav.file);

            // write the 'RIFF' header
            wav.writer.Write("RIFF".ToCharArray());  //fwrite("RIFF", 1, 4, wav.file);

            // write the total size
            temp32 = 0;
            wav.total_offs = (uint32_t)wav.file.Length;  // ftell(wav.file);
            wav.writer.Write(temp32);  //fwrite(&temp32, 1, 4, wav.file);

            // write the 'WAVE' type
            wav.writer.Write("WAVE".ToCharArray());  //fwrite("WAVE", 1, 4, wav.file);

            // write the 'fmt ' tag
            wav.writer.Write("fmt ".ToCharArray());  //fwrite("fmt ", 1, 4, wav.file);

            // write the format length
            temp32 = (uint32_t)little_endianize_int32(16);
            wav.writer.Write(temp32);  //fwrite(&temp32, 1, 4, wav.file);

            // write the format (PCM)
            temp16 = (uint16_t)little_endianize_int16(1);
            wav.writer.Write(temp16);  //fwrite(&temp16, 1, 2, wav.file);

            // write the channels
            temp16 = (uint16_t)little_endianize_int16((int16_t)channels);
            wav.writer.Write(temp16);  //fwrite(&temp16, 1, 2, wav.file);

            // write the sample rate
            temp32 = (uint32_t)little_endianize_int32(sample_rate);
            wav.writer.Write(temp32);  //fwrite(&temp32, 1, 4, wav.file);

            // write the bytes/second
            uint32_t bps = (uint32_t)(sample_rate * 2 * channels);
            temp32 = (uint32_t)little_endianize_int32((int)bps);
            wav.writer.Write(temp32);  //fwrite(&temp32, 1, 4, wav.file);

            // write the block align
            uint16_t align = (uint16_t)(2 * channels);
            temp16 = (uint16_t)little_endianize_int16((int16_t)align);
            wav.writer.Write(temp16);  //fwrite(&temp16, 1, 2, wav.file);

            // write the bits/sample
            temp16 = (uint16_t)little_endianize_int16(16);
            wav.writer.Write(temp16);  //fwrite(&temp16, 1, 2, wav.file);

            // write the 'data' tag
            wav.writer.Write("data".ToCharArray());  //fwrite("data", 1, 4, wav.file);

            // write the data length
            temp32 = 0;
            wav.data_offs = (uint32_t)wav.file.Length;  //ftell(wav.file);
            wav.writer.Write(temp32);  //fwrite(&temp32, 1, 4, wav.file);

            return wav;
        }

        public static void wav_close(wav_file wav)
        {
            uint32_t total;
            uint32_t temp32;

            if (wav == null)
                return;

            total = (uint32_t)wav.file.Length;  //ftell(wav.file);

            /* update the total file size */
            wav.writer.Seek((int)wav.total_offs, SeekOrigin.Begin);  //fseek(wav.file, wav.total_offs, SEEK_SET);
            temp32 = total - (wav.total_offs + 4);
            temp32 = (uint32_t)little_endianize_int32((int)temp32);
            wav.writer.Write(temp32);  //fwrite(&temp32, 1, 4, wav.file);

            /* update the data size */
            wav.writer.Seek((int)wav.data_offs, SeekOrigin.Begin);  //fseek(wav.file, wav.data_offs, SEEK_SET);
            temp32 = total - (wav.data_offs + 4);
            temp32 = (uint32_t)little_endianize_int32((int)temp32);
            wav.writer.Write(temp32);  //fwrite(&temp32, 1, 4, wav.file);

            //fclose(wav.file);
            wav.writer.Dispose();
            wav.file.Dispose();
            wav.writer = null;
            wav.file = null;

            //global_free(wav);
        }

        public static void wav_add_data_16(wav_file wav, Pointer<int16_t> data, int samples)  //void wav_add_data_16(wav_file *wavptr, std::int16_t *data, int samples);
        {
            if (wav == null || samples == 0)
                return;

            throw new emu_unimplemented();
        }

        public static void wav_add_data_32(wav_file wav, Pointer<int32_t> data, int samples, int shift)  //void wav_add_data_32(wav_file *wavptr, std::int32_t *data, int samples, int shift);
        {
            std.vector<int16_t> temp = new std.vector<int16_t>();
            int i;

            if (wav == null || samples == 0)
                return;

            /* resize dynamic array */
            temp.resize((size_t)samples);

            /* clamp */
            for (i = 0; i < samples; i++)
            {
                int val = data[i] >> shift;
                temp[i] = (int16_t)((val < -32768) ? -32768 : (val > 32767) ? 32767 : val);
            }

            throw new emu_unimplemented();
        }

        public static void wav_add_data_16lr(wav_file wav, Pointer<int16_t> left, Pointer<int16_t> right, int samples)  //void wav_add_data_16lr(wav_file *wavptr, std::int16_t *left, std::int16_t *right, int samples);
        {
            std.vector<int16_t> temp = new std.vector<int16_t>();
            int i;

            if (wav == null || samples == 0)
                return;

            /* resize dynamic array */
            temp.resize((size_t)samples * 2);

            /* interleave */
            for (i = 0; i < samples * 2; i++)
                temp[i] = (i & 1) != 0 ? right[i / 2] : left[i / 2];

            throw new emu_unimplemented();
        }

        public static void wav_add_data_32lr(wav_file wav, Pointer<int32_t> left, Pointer<int32_t> right, int samples, int shift)  //void wav_add_data_32lr(wav_file *wavptr, std::int32_t *left, std::int32_t *right, int samples, int shift);
        {
            std.vector<int16_t> temp = new std.vector<int16_t>();
            int i;

            if (wav == null || samples == 0)
                return;

            /* resize dynamic array */
            temp.resize((size_t)samples);

            /* interleave */
            for (i = 0; i < samples * 2; i++)
            {
                int val = (i & 1) != 0 ? right[i / 2] : left[i / 2];
                val >>= shift;
                temp[i] = (int16_t)((val < -32768) ? -32768 : (val > 32767) ? 32767 : val);
            }

            throw new emu_unimplemented();
        }
    }
}
