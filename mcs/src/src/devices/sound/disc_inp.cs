// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using osd_ticks_t = System.UInt64;
using stream_sample_t = System.Int32;


namespace mame
{
    // moved to discrete_base_node
    //#define DSS_INPUT__GAIN     DISCRETE_INPUT(0)
    //#define DSS_INPUT__OFFSET   DISCRETE_INPUT(1)
    //#define DSS_INPUT__INIT     DISCRETE_INPUT(2)


    //class DISCRETE_CLASS_NAME(dss_input_data): public discrete_base_node, public discrete_input_interface
    partial class discrete_dss_input_data_node : discrete_base_node,
                                                 discrete_input_interface
    {
        /************************************************************************
         *
         * DSS_INPUT_x    - Receives input from discrete_sound_w
         *
         * input[0]    - Gain value
         * input[1]    - Offset value
         * input[2]    - Starting Position
         * input[3]    - Current data value
         *
         ************************************************************************/

        // discrete_base_node
        //DISCRETE_RESET(dss_input_data)
        public override void reset()
        {
            m_gain = DSS_INPUT__GAIN;
            m_offset = DSS_INPUT__OFFSET;

            m_data = (byte)DSS_INPUT__INIT;
            set_output(0, m_data * m_gain + m_offset);
        }


        // discrete_input_interface
        //void DISCRETE_CLASS_FUNC(dss_input_data, input_write)(int sub_node, UINT8 data )
        public void input_write(int sub_node, byte data )
        {
            byte new_data = 0;

            new_data = data;

            if (m_data != new_data)
            {
                /* Bring the system up to now */
                m_device.update_to_current_time();

                m_data = new_data;

                /* Update the node output here so we don't have to do it each step */
                set_output(0,  m_data * m_gain + m_offset);
            }
        }
    }


    //class DISCRETE_CLASS_NAME(dss_input_logic): public discrete_base_node, public discrete_input_interface
    partial class discrete_dss_input_logic_node : discrete_base_node,
                                                  discrete_input_interface
    {
        //DISCRETE_RESET(dss_input_logic)
        public override void reset()
        {
            m_gain = DSS_INPUT__GAIN;
            m_offset = DSS_INPUT__OFFSET;

            m_data = (DSS_INPUT__INIT == 0) ? (byte)0 : (byte)1;
            set_output(0,  m_data * m_gain + m_offset);
        }


        //void DISCRETE_CLASS_FUNC(dss_input_logic, input_write)(int sub_node, uint8_t data )
        public void input_write(int sub_node, byte data)
        {
            byte new_data    = 0;

            new_data =  data != 0 ? (byte)1 : (byte)0;

            if (m_data != new_data)
            {
                /* Bring the system up to now */
                m_device.update_to_current_time();

                m_data = new_data;

                /* Update the node output here so we don't have to do it each step */
                set_output(0,  m_data * m_gain + m_offset);
            }
        }
    }


    //class DISCRETE_CLASS_NAME(dss_input_stream): public discrete_base_node, public discrete_input_interface, public discrete_step_interface
    partial class discrete_dss_input_stream_node : discrete_base_node,
                                                   discrete_input_interface,
                                                   discrete_step_interface
    {
        /************************************************************************
         *
         * DSS_INPUT_STREAM    - Receives input from a routed stream
         *
         * input[0]    - Input stream number
         * input[1]    - Gain value
         * input[2]    - Offset value
         *
         ************************************************************************/
        public double DSS_INPUT_STREAM__STREAM { get { return DISCRETE_INPUT(0); } }
        public double DSS_INPUT_STREAM__GAIN { get { return DISCRETE_INPUT(1); } }
        public double DSS_INPUT_STREAM__OFFSET { get { return DISCRETE_INPUT(2); } }


        void stream_generate(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples)
        {
            ListPointer<stream_sample_t> ptr = new ListPointer<stream_sample_t>(outputs[0]);  //stream_sample_t *ptr = outputs[0];
            int samplenum = samples;

            while (samplenum-- > 0)
            {
                ptr[0] = m_data;
                ptr++;
            }
        }


        //DISCRETE_STEP(dss_input_stream)
        public void step()
        {
            /* the context pointer is set to point to the current input stream data in discrete_stream_update */
            if (m_ptr != null)
            {
                set_output(0, m_ptr[0] * m_gain + m_offset);
                m_ptr++;
            }
            else
            {
                set_output(0,  0);
            }
        }


        //DISCRETE_RESET(dss_input_stream)
        public override void reset()
        {
            m_ptr = null;
            m_data = 0;
        }


        // discrete_input_interface
        //void DISCRETE_CLASS_FUNC(dss_input_stream, input_write)(int sub_node, UINT8 data )
        public void input_write(int sub_node, byte data )
        {
            byte new_data    = 0;

            new_data =  data;

            if (m_data != new_data)
            {
                if (m_is_buffered != 0)
                {
                    /* Bring the system up to now */
                    m_buffer_stream.update();

                    m_data = new_data;
                }
                else
                {
                    /* Bring the system up to now */
                    m_device.update_to_current_time();

                    m_data = new_data;

                    /* Update the node output here so we don't have to do it each step */
                    set_output(0,  new_data * m_gain + m_offset);
                }
            }
        }


        //DISCRETE_START(dss_input_stream)
        public override void start()
        {
            base.start();

            /* Stream out number is set during start */
            m_stream_in_number = (UInt32)DSS_INPUT_STREAM__STREAM;
            m_gain = DSS_INPUT_STREAM__GAIN;
            m_offset = DSS_INPUT_STREAM__OFFSET;
            m_ptr = null;

            m_is_buffered = is_buffered() ? (byte)1 : (byte)0;
            m_buffer_stream = null;
        }


        /* This is called by discrete_sound_device */
        public void stream_start()
        {
            if (m_is_buffered != 0)
            {
                /* stream_buffered input only supported for sound devices */
                discrete_sound_device snd_device = (discrete_sound_device)m_device;
                //assert(DSS_INPUT_STREAM__STREAM < snd_device->m_input_stream_list.count());

                m_buffer_stream = m_device.machine().sound().stream_alloc(snd_device, 0, 1, sample_rate(), stream_generate);  //stream_update_delegate(FUNC(discrete_dss_input_stream_node::stream_generate),this));

                snd_device.get_stream().set_input((int)m_stream_in_number, m_buffer_stream);
            }
        }
    }
}
