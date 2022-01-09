// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> movie_recording
    public abstract class movie_recording
    {
        // movie format options
        public enum format
        {
            MNG,
            AVI
        }


        //typedef std::unique_ptr<movie_recording> ptr;


        //screen_device * m_screen;               // screen associated with this movie (can be nullptr)
        //attotime        m_frame_period;         // duration of movie frame
        //attotime        m_next_frame_time;      // time of next frame
        //int             m_frame;                // current movie frame number


        // ctor
        movie_recording(screen_device screen) { throw new emu_unimplemented(); }

        // dtor
        //virtual ~movie_recording();


        // accessors
        //screen_device *screen()                 { return m_screen; }
        //attotime frame_period()                 { return m_frame_period; }
        public void set_next_frame_time(attotime time) { throw new emu_unimplemented(); }  //{ m_next_frame_time = time; }
        //attotime next_frame_time() const        { return m_next_frame_time; }

        // methods
        //bool append_video_frame(bitmap_rgb32 &bitmap, attotime curtime);

        // virtuals
        public abstract bool add_sound_to_recording(object sound, int numsamples);  //virtual bool add_sound_to_recording(const s16 *sound, int numsamples) = 0;

        // statics
        //static movie_recording::ptr create(running_machine &machine, screen_device *screen, format fmt, std::unique_ptr<emu_file> &&file, bitmap_rgb32 &snap_bitmap);
        //static const char *format_file_extension(format fmt);


        // virtuals
        protected abstract bool append_single_video_frame(bitmap_rgb32 bitmap, rgb_t palette, int palette_entries);  //virtual bool append_single_video_frame(bitmap_rgb32 &bitmap, const rgb_t *palette, int palette_entries) = 0;


        // accessors
        //int current_frame() const { return m_frame; }
        //void set_frame_period(attotime time) { m_frame_period = time; }
    }
}
