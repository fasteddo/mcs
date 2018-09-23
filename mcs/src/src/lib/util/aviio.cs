// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int16_t = System.Int16;
using uint32_t = System.UInt32;


namespace mame
{
    abstract class avi_file
    {
        /***********************************************************************
            CONSTANTS
        ***********************************************************************/
        public enum error
        {
            NONE = 0,
            END,
            INVALID_DATA,
            NO_MEMORY,
            READ_ERROR,
            WRITE_ERROR,
            STACK_TOO_DEEP,
            UNSUPPORTED_FEATURE,
            CANT_OPEN_FILE,
            INCOMPATIBLE_AUDIO_STREAMS,
            INVALID_SAMPLERATE,
            INVALID_STREAM,
            INVALID_FRAME,
            INVALID_BITMAP,
            UNSUPPORTED_VIDEO_FORMAT,
            UNSUPPORTED_AUDIO_FORMAT,
            EXCEEDED_SOUND_BUFFER
        }


        enum datatype
        {
            VIDEO,
            AUDIO_CHAN0,
            AUDIO_CHAN1,
            AUDIO_CHAN2,
            AUDIO_CHAN3,
            AUDIO_CHAN4,
            AUDIO_CHAN5,
            AUDIO_CHAN6,
            AUDIO_CHAN7
        }


        /***********************************************************************
            TYPE DEFINITIONS
        ***********************************************************************/

        struct movie_info
        {
#if false
            std::uint32_t   video_format;               // format of video data
            std::uint32_t   video_timescale;            // timescale for video data
            std::uint32_t   video_sampletime;           // duration of a single video sample (frame)
            std::uint32_t   video_numsamples;           // total number of video samples
            std::uint32_t   video_width;                // width of the video
            std::uint32_t   video_height;               // height of the video
            std::uint32_t   video_depth;                // depth of the video

            std::uint32_t   audio_format;               // format of audio data
            std::uint32_t   audio_timescale;            // timescale for audio data
            std::uint32_t   audio_sampletime;           // duration of a single audio sample
            std::uint32_t   audio_numsamples;           // total number of audio samples
            std::uint32_t   audio_channels;             // number of audio channels
            std::uint32_t   audio_samplebits;           // number of bits per channel
            std::uint32_t   audio_samplerate;           // sample rate of audio
#endif
        }

        //typedef std::unique_ptr<avi_file> ptr;


        /***********************************************************************
            PROTOTYPES
        ***********************************************************************/

        //protected avi_file();
        //static error open(std::string const &filename, ptr &file);
        //static error create(std::string const &filename, movie_info const &info, ptr &file);
        //virtual ~avi_file();

        //virtual void printf_chunks() = 0;
        //static const char *error_string(error err);

        //virtual movie_info const &get_movie_info() const = 0;
        //virtual std::uint32_t first_sample_in_frame(std::uint32_t framenum) const = 0;

        //virtual error read_video_frame(std::uint32_t framenum, bitmap_yuy16 &bitmap) = 0;
        //virtual error read_sound_samples(int channel, std::uint32_t firstsample, std::uint32_t numsamples, std::int16_t *output) = 0;

        public abstract error append_video_frame(bitmap_yuy16 bitmap);
        public abstract error append_video_frame(bitmap_rgb32 bitmap);
        public abstract error append_sound_samples(int channel, ListPointer<int16_t> samples, uint32_t numsamples, uint32_t sampleskip);  //std::int16_t const *samples
    }
}
