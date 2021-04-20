// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using u32 = System.UInt32;
using uint32_t = System.UInt32;


namespace mame
{
    public enum cassette_state
    {
        /* this part of the state is controlled by the UI */
        CASSETTE_STOPPED            = 0,
        CASSETTE_PLAY               = 1,
        CASSETTE_RECORD             = 2,

        /* this part of the state is controlled by drivers */
        CASSETTE_MOTOR_ENABLED      = 0,
        CASSETTE_MOTOR_DISABLED     = 4,
        CASSETTE_SPEAKER_ENABLED    = 0,
        CASSETTE_SPEAKER_MUTED      = 8,

        /* masks */
        CASSETTE_MASK_UISTATE       = 3,
        CASSETTE_MASK_MOTOR         = 4,
        CASSETTE_MASK_SPEAKER       = 8,
        CASSETTE_MASK_DRVSTATE      = 12
    }


    // ======================> cassette_image_device
    public class cassette_image_device : device_t
                                         //device_image_interface
                                         //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(CASSETTE, cassette_image_device, "cassette_image", "Cassette")
        static device_t device_creator_cassette_image_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new cassette_image_device(mconfig, tag, owner, clock); }
        public static readonly device_type CASSETTE = DEFINE_DEVICE_TYPE(device_creator_cassette_image_device, "cassette_image", "Cassette");


        //cassette_image  *m_cassette;
        //cassette_state  m_state;
        //double          m_position;
        //double          m_position_time;
        //INT32           m_value;
        //int             m_channel;
        //double          m_speed; // speed multiplier for tape speeds other than standard 1.875ips (used in adam driver)
        //int             m_direction; // direction select
        //char            m_extension_list[256];
        //const struct CassetteFormat*    const *m_formats;
        //const struct CassetteOptions    *m_create_opts;
        //cassette_state                  m_default_state;
        //const char *                    m_interface;
        //bool            m_stereo;


        // construction/destruction
        //-------------------------------------------------
        //  cassette_image_device - constructor
        //-------------------------------------------------
        public cassette_image_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, CASSETTE, tag, owner, clock)
        {
            throw new emu_unimplemented();
        }


        //void set_formats(const struct CassetteFormat*  const *formats) { m_formats = formats; }
        //void set_create_opts(const struct CassetteOptions  *create_opts) { m_create_opts = create_opts; }
        //void set_default_state(cassette_state default_state) { m_default_state = default_state; }
        //void set_default_state(int default_state) { m_default_state = (cassette_state)default_state; }
        //void set_interface(const char *interface) { m_interface = interface; }


        // image-level overrides
        //virtual image_init_result call_load();
        //virtual image_init_result call_create(int format_type, util::option_resolution *format_options);
        //virtual void call_unload();
        //virtual string call_display();


        //virtual iodevice_t image_type() const { return IO_CASSETTE; }


        //virtual bool is_readable()  const noexcept override { return true; }
        //virtual bool is_writeable() const noexcept override { return true; }
        //virtual bool is_creatable() const noexcept override { return true; }
        //virtual bool must_be_loaded() const noexcept override { return false; }
        //virtual bool is_reset_on_load() const noexcept override { return false; }
        //virtual const char *image_interface() const noexcept override { return m_interface; }
        //virtual const char *file_extensions() const noexcept override { return m_extension_list; }


        // specific implementation
        //cassette_state get_state() { return m_state; }
        //void set_state(cassette_state state) { change_state(state, (cassette_state)(~0)); }
        public void change_state(cassette_state state, cassette_state mask)
        {
            throw new emu_unimplemented();
        }

        //double input();
        //void output(double value);

        //cassette_image *get_image() { return m_cassette; }
        //double get_position();
        //double get_length();
        //void set_speed(double speed);
        //void set_channel(int channel);
        //void go_forward();
        //void go_reverse();
        //void seek(double time, int origin);


        // sound stream update overrides
        //void sound_stream_update_legacy(sound_stream &stream, stream_sample_t const * const *inputs, stream_sample_t * const *outputs, int samples) override;
        //device_sound_interface& set_stereo() { m_stereo = true; return *this; }


        //bool is_motor_on();
        //void update();


        // device-level overrides
        //virtual void device_config_complete();
        protected override void device_start() { throw new emu_unimplemented(); }
        //virtual const bool use_software_list_file_extension_for_filetype() const override { return true; }

        // device_image_interface implementation
        //virtual const software_list_loader &get_software_list_loader() const override { return image_software_list_loader::instance(); }

        //image_init_result internal_load(bool is_create);
    }


    // device iterator
    //typedef device_type_iterator<&device_creator<cassette_image_device>, cassette_image_device> cassette_device_iterator;
    public class cassette_device_iterator : device_type_iterator<cassette_image_device>
    {
        public cassette_device_iterator(device_t root, int maxdepth = 255) : base(cassette_image_device.CASSETTE, root, maxdepth) { }
    }
}
