// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    // ======================> flac_decoder
    class flac_decoder
    {
        // output state
        //FLAC__StreamDecoder *   m_decoder;              // actual encoder
        //util::core_file *       m_file;                 // output file
        uint32_t m_sample_rate;          // decoded sample rate
        uint8_t m_channels;             // decoded number of channels
        uint8_t m_bits_per_sample;      // decoded bits per sample
        //uint32_t                m_compressed_offset;    // current offset in compressed data
        //const FLAC__byte *      m_compressed_start;     // start of compressed data
        //uint32_t                m_compressed_length;    // length of compressed data
        //const FLAC__byte *      m_compressed2_start;    // start of compressed data
        //uint32_t                m_compressed2_length;   // length of compressed data
        //int16_t *               m_uncompressed_start[8];// pointer to start of uncompressed data (up to 8 streams)
        //uint32_t                m_uncompressed_offset;  // current position in uncompressed data
        //uint32_t                m_uncompressed_length;  // length of uncompressed data
        //bool                    m_uncompressed_swap;    // swap uncompressed sample data
        //uint8_t                 m_custom_header[0x2a];  // custom header


        // construction/destruction
        //-------------------------------------------------
        //  flac_decoder - constructor
        //-------------------------------------------------
        public flac_decoder()
        {
            throw new emu_unimplemented();
        }

        public flac_decoder(object buffer, uint32_t length, object buffer2 = null, uint32_t length2 = 0)  //const void *buffer, UINT32 length, const void *buffer2 = null, UINT32 length2 = 0)
        {
            throw new emu_unimplemented();
        }

        public flac_decoder(util_.core_file file)
        {
            throw new emu_unimplemented();
        }

        //-------------------------------------------------
        //  flac_decoder - destructor
        //-------------------------------------------------
        ~flac_decoder()
        {
            throw new emu_unimplemented();
        }


        // getters (valid after reset)
        public uint32_t sample_rate() { return m_sample_rate; }
        public uint8_t channels() { return m_channels; }
        public uint8_t bits_per_sample() { return m_bits_per_sample; }
        public uint32_t total_samples()
        {
            throw new emu_unimplemented();
        }
        //FLAC__StreamDecoderState state() const { return FLAC__stream_decoder_get_state(m_decoder); }
        //const char *state_string() const { return FLAC__stream_decoder_get_resolved_state_string(m_decoder); }


        // reset
        //bool reset();
        //bool reset(const void *buffer, UINT32 length, const void *buffer2 = NULL, UINT32 length2 = 0);
        //bool reset(UINT32 sample_rate, UINT8 num_channels, UINT32 block_size, const void *buffer, UINT32 length);
        //bool reset(core_file &file);


        // decode to a buffer; num_samples must be a multiple of the block size

        //-------------------------------------------------
        //  decode_interleaved - decode to an interleaved
        //  sound stream
        //-------------------------------------------------
        public bool decode_interleaved(object samples, uint32_t num_samples, bool swap_endian = false)  //INT16 *samples, UINT32 num_samples, bool swap_endian = false)
        {
            throw new emu_unimplemented();
        }

        //bool decode(INT16 **samples, UINT32 num_samples, bool swap_endian = false);


        // finish up
        //-------------------------------------------------
        //  finish - complete encoding and flush the
        //  stream
        //-------------------------------------------------
        public uint32_t finish()
        {
            throw new emu_unimplemented();
        }


        // internal helpers
        //static FLAC__StreamDecoderReadStatus read_callback_static(const FLAC__StreamDecoder *decoder, FLAC__byte buffer[], size_t *bytes, void *client_data);
        //FLAC__StreamDecoderReadStatus read_callback(FLAC__byte buffer[], size_t *bytes);
        //static void metadata_callback_static(const FLAC__StreamDecoder *decoder, const FLAC__StreamMetadata *metadata, void *client_data);
        //static FLAC__StreamDecoderTellStatus tell_callback_static(const FLAC__StreamDecoder *decoder, FLAC__uint64 *absolute_byte_offset, void *client_data);
        //static FLAC__StreamDecoderWriteStatus write_callback_static(const FLAC__StreamDecoder *decoder, const ::FLAC__Frame *frame, const FLAC__int32 * const buffer[], void *client_data);
        //FLAC__StreamDecoderWriteStatus write_callback(const ::FLAC__Frame *frame, const FLAC__int32 * const buffer[]);
        //static void error_callback_static(const FLAC__StreamDecoder *decoder, FLAC__StreamDecoderErrorStatus status, void *client_data);
    }
}
