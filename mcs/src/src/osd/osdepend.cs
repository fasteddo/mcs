// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int16_t = System.Int16;


namespace mame
{
    // ======================> osd_font interface
    public interface osd_font
    {
        //typedef std::unique_ptr<osd_font> ptr;

        /** attempt to "open" a handle to the font with the given name */
        bool open(string font_path, string name, out int height);

        /** release resources associated with a given OSD font */
        //virtual void close() = 0;

        /*!
         * allocate and populate a BITMAP_FORMAT_ARGB32 bitmap containing
         * the pixel values rgb_t(0xff,0xff,0xff,0xff) or
         * rgb_t(0x00,0xff,0xff,0xff) for each pixel of a black & white font
         */
        //virtual bool get_bitmap(char32_t chnum, bitmap_argb32 &bitmap, std::int32_t &width, std::int32_t &xoffs, std::int32_t &yoffs) = 0;
    }


    // ======================> osd_interface
    // description of the currently-running machine
    public interface osd_interface
    {
        // general overridables
        void init(running_machine machine);
        void update(bool skip_redraw);
        void input_update();
        void set_verbose(bool print_verbose);

        // debugger overridables
        //virtual void init_debugger() = 0;
        //virtual void wait_for_debugger(device_t &device, bool firststop) = 0;

        // audio overridables
        void update_audio_stream(Pointer<int16_t> buffer, int samples_this_frame);  //const int16_t *buffer, int samples_this_frame) = 0;
        void set_mastervolume(int attenuation);
        bool no_sound();

        // input overridables
        void customize_input_type_list(std.vector<input_type_entry> typelist);

        // video overridables
        void add_audio_to_recording(Pointer<int16_t> buffer, int samples_this_frame);  //const int16_t *buffer, int samples_this_frame) = 0;
        std.vector<ui.menu_item> get_slider_list();

        // font interface
        osd_font font_alloc();
        //virtual bool get_font_families(std::string const &font_path, std::vector<std::pair<std::string, std::string> > &result) = 0;

        // command option overrides
        bool execute_command(string command);

        // midi interface
        //virtual osd_midi_device *create_midi_device() = 0;
    }


    /***************************************************************************
        MIDI I/O INTERFACES
    ***************************************************************************/
    //class osd_midi_device
    //{
    //public:
    //    virtual ~osd_midi_device() { }
    //    virtual bool open_input(const char *devname) = 0;
    //    virtual bool open_output(const char *devname) = 0;
    //    virtual void close() = 0;
    //    virtual bool poll() = 0;
    //    virtual int read(uint8_t *pOut) = 0;
    //    virtual void write(uint8_t data) = 0;
    //};
}
