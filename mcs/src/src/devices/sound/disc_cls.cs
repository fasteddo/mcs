// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using osd_ticks_t = System.UInt64;
using stream_sample_t = System.Int32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    //class DISCRETE_CLASS_NAME(special): public discrete_base_node
    class discrete_special_node : discrete_base_node
    {
        //DISCRETE_CLASS_CONSTRUCTOR(special, base)
        public discrete_special_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(special)
        ~discrete_special_node() { }


        protected override int max_output() { return 0; }
    }


#if false
    class DISCRETE_CLASS_NAME(unimplemented): public discrete_base_node
    {
        DISCRETE_CLASS_CONSTRUCTOR(unimplemented, base)
        DISCRETE_CLASS_DESTRUCTOR(unimplemented)
    public:
        int max_output(void) { return 0; }
    };
#endif

    /*************************************
     *
     *  disc_sys.inc
     *
     *************************************/

    //class DISCRETE_CLASS_NAME(dso_output):  public discrete_base_node, public discrete_sound_output_interface, public discrete_step_interface
    class discrete_dso_output_node : discrete_base_node,
                                     discrete_sound_output_interface, 
                                     discrete_step_interface
    {
        ListPointer<stream_sample_t> m_ptr;  //stream_sample_t     *m_ptr;


        //DISCRETE_CLASS_CONSTRUCTOR(dso_output, base)
        public discrete_dso_output_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(dso_output)
        ~discrete_dso_output_node() { }


        // discrete_base_node
        protected override int max_output() { return 0; }


        // discrete_sound_output_interface
        public void set_output_ptr(ListPointer<stream_sample_t> ptr) { m_ptr = new ListPointer<stream_sample_t>(ptr); }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }

        public void step()
        {
            /* Add gain to the output and put into the buffers */
            /* Clipping will be handled by the main sound system */
            double val = DISCRETE_INPUT(0) * DISCRETE_INPUT(1);
            m_ptr[0] = (int)val;  //*m_ptr++ = val;
            m_ptr++;
        }
    }


#if false
    DISCRETE_CLASS(dso_csvlog, 0,
        FILE *m_csv_file;
        INT64 m_sample_num;
        char  m_name[32];
    );

    DISCRETE_CLASS(dso_wavlog, 0,
        wav_file *m_wavfile;
        char      m_name[32];
    );
#endif


    /*************************************
     *
     *  disc_inp.inc
     *
     *************************************/
#if false
    class DISCRETE_CLASS_NAME(dss_adjustment): public discrete_base_node, public discrete_step_interface
    {
        DISCRETE_CLASS_CONSTRUCTOR(dss_adjustment, base)
        DISCRETE_CLASS_DESTRUCTOR(dss_adjustment)
    public:
        void step(void);
        void reset(void);
    private:
        ioport_port *m_port;
        INT32                   m_lastpval;
        INT32                   m_pmin;
        double                  m_pscale;
        double                  m_min;
        double                  m_scale;
    };

    DISCRETE_CLASS_RESET(dss_constant, 1);
#endif


    //class DISCRETE_CLASS_NAME(dss_input_data): public discrete_base_node, public discrete_input_interface
    partial class discrete_dss_input_data_node : discrete_base_node,
                                                 discrete_input_interface
    {
        double m_gain;             /* node gain */
        double m_offset;           /* node offset */
        uint8_t m_data;             /* data written */


        //DISCRETE_CLASS_CONSTRUCTOR(dss_input_data, base)
        public discrete_dss_input_data_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(dss_input_data)
        ~discrete_dss_input_data_node() { }


        // discrete_base_node
        //DISCRETE_RESET(dss_input_data)

        // discrete_input_interface
        //void DISCRETE_CLASS_FUNC(dss_input_data, input_write)(int sub_node, UINT8 data )
    }


    //class DISCRETE_CLASS_NAME(dss_input_logic): public discrete_base_node, public discrete_input_interface
    partial class discrete_dss_input_logic_node : discrete_base_node,
                                                  discrete_input_interface
    {
        double m_gain;             /* node gain */
        double m_offset;           /* node offset */
        uint8_t m_data;             /* data written */


        //DISCRETE_CLASS_CONSTRUCTOR(dss_input_logic, base)
        public discrete_dss_input_logic_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(dss_input_logic)
        ~discrete_dss_input_logic_node() { }


        //DISCRETE_RESET(dss_input_logic)
        //void DISCRETE_CLASS_FUNC(dss_input_logic, input_write)(int sub_node, uint8_t data )
    }


#if false
    class DISCRETE_CLASS_NAME(dss_input_not): public discrete_base_node, public discrete_input_interface
    {
        DISCRETE_CLASS_CONSTRUCTOR(dss_input_not, base)
        DISCRETE_CLASS_DESTRUCTOR(dss_input_not)
    public:
        void reset(void);
        void input_write(int sub_node, UINT8 data );
    private:
        double      m_gain;             /* node gain */
        double      m_offset;           /* node offset */
        UINT8       m_data;             /* data written */
    };
#endif

#if false
    class DISCRETE_CLASS_NAME(dss_input_pulse): public discrete_base_node, public discrete_input_interface, public discrete_step_interface
    {
        DISCRETE_CLASS_CONSTRUCTOR(dss_input_pulse, base)
        DISCRETE_CLASS_DESTRUCTOR(dss_input_pulse)
    public:
        void step(void);
        void reset(void);
        void input_write(int sub_node, UINT8 data );
    private:
        //double      m_gain;             /* node gain */
        //double      m_offset;           /* node offset */
        UINT8       m_data;             /* data written */
    };
#endif


    //class DISCRETE_CLASS_NAME(dss_input_stream): public discrete_base_node, public discrete_input_interface, public discrete_step_interface
    partial class discrete_dss_input_stream_node : discrete_base_node,
                                                   discrete_input_interface,
                                                   discrete_step_interface
    {
        public uint32_t m_stream_in_number;
        public ListPointer<stream_sample_t> m_ptr;  //stream_sample_t     *m_ptr;         /* current in ptr for stream */


        double m_gain;             /* node gain */
        double m_offset;           /* node offset */
        uint8_t m_data;             /* data written */
        uint8_t m_is_buffered;
        /* the buffer stream */
        sound_stream m_buffer_stream;


        //DISCRETE_CLASS_CONSTRUCTOR(dss_input_stream, base)
        public discrete_dss_input_stream_node() : base() { }

        //DISCRETE_CLASS_DESTRUCTOR(dss_input_stream)
        ~discrete_dss_input_stream_node() { }


        // discrete_base_node

        //DISCRETE_RESET(dss_input_stream)
        //DISCRETE_START(dss_input_stream)


        // discrete_input_interface
        //void DISCRETE_CLASS_FUNC(dss_input_stream, input_write)(int sub_node, UINT8 data )


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }

        //DISCRETE_STEP(dss_input_stream)

        protected virtual bool is_buffered() { return false; }


        /* This is called by discrete_sound_device */
        //public void stream_start()

        //void stream_generate(sound_stream stream, dynamic_array_pointer<stream_sample_t> [] inputs, dynamic_array_pointer<stream_sample_t> [] outputs, int samples)
    }


#if false
    class DISCRETE_CLASS_NAME(dss_input_buffer): public DISCRETE_CLASS_NAME(dss_input_stream)
    {
        DISCRETE_CLASS_CONSTRUCTOR(dss_input_buffer, dss_input_stream)
        DISCRETE_CLASS_DESTRUCTOR(dss_input_buffer)
    public:
        bool is_buffered(void) { return true; }
    };
#endif
}
