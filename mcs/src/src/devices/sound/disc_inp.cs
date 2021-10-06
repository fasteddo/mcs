// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int32_t = System.Int32;
using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using s32 = System.Int32;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using u32 = System.UInt32;


namespace mame
{
    //class DISCRETE_CLASS_NAME(dss_adjustment): public discrete_base_node, public discrete_step_interface
    partial class discrete_dss_adjustment_node : discrete_base_node,
                                                 discrete_step_interface
    {
        double DSS_ADJUSTMENT__MIN { get { return DISCRETE_INPUT(0); } }
        double DSS_ADJUSTMENT__MAX { get { return DISCRETE_INPUT(1); } }
        double DSS_ADJUSTMENT__LOG { get { return DISCRETE_INPUT(2); } }
        //#define DSS_ADJUSTMENT__PORT    DISCRETE_INPUT(3)
        double DSS_ADJUSTMENT__PMIN { get { return DISCRETE_INPUT(4); } }
        double DSS_ADJUSTMENT__PMAX { get { return DISCRETE_INPUT(5); } }


        // discrete_base_node

        //DISCRETE_RESET(dss_adjustment)
        public override void reset()
        {
            double min;
            double max;

            m_port = m_device.machine().root_device().ioport(m_device.siblingtag((string)this.custom_data()));
            if (m_port == null)
                g.fatalerror("DISCRETE_ADJUSTMENT - NODE_{0} has invalid tag\n", this.index());

            m_lastpval = 0x7fffffff;
            m_pmin     = (int32_t)DSS_ADJUSTMENT__PMIN;
            m_pscale   = 1.0 / (double)(DSS_ADJUSTMENT__PMAX - DSS_ADJUSTMENT__PMIN);

            /* linear scale */
            if (DSS_ADJUSTMENT__LOG == 0)
            {
                m_min   = DSS_ADJUSTMENT__MIN;
                m_scale = DSS_ADJUSTMENT__MAX - DSS_ADJUSTMENT__MIN;
            }

            /* logarithmic scale */
            else
            {
                /* force minimum and maximum to be > 0 */
                min = (DSS_ADJUSTMENT__MIN > 0) ? DSS_ADJUSTMENT__MIN : 1;
                max = (DSS_ADJUSTMENT__MAX > 0) ? DSS_ADJUSTMENT__MAX : 1;
                m_min   = Math.Log10(min);
                m_scale = Math.Log10(max) - Math.Log10(min);
            }

            this.step();
        }


        // discrete_step_interface

        public osd_ticks_t run_time { get; set; }
        public discrete_base_node self { get; set; }


        //DISCRETE_STEP(dss_adjustment)
        public void step()
        {
            int32_t  rawportval = (int32_t)m_port.read();

            /* only recompute if the value changed from last time */
            if (rawportval != m_lastpval)
            {
                double portval   = (double)(rawportval - m_pmin) * m_pscale;
                double scaledval = portval * m_scale + m_min;

                m_lastpval = rawportval;
                if (DSS_ADJUSTMENT__LOG == 0)
                    set_output(0,  scaledval);
                else
                    set_output(0,  Math.Pow(10, scaledval));
            }
        }
    }


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
        // discrete_base_node

        //DISCRETE_RESET(dss_input_logic)
        public override void reset()
        {
            m_gain = DSS_INPUT__GAIN;
            m_offset = DSS_INPUT__OFFSET;

            m_data = (DSS_INPUT__INIT == 0) ? (byte)0 : (byte)1;
            set_output(0,  m_data * m_gain + m_offset);
        }


        // discrete_input_interface

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
    public partial class discrete_dss_input_stream_node : discrete_base_node,
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


        // discrete_base_node

        //DISCRETE_RESET(dss_input_stream)
        public override void reset()
        {
            m_inview = null;
            m_data = 0;
        }


        //DISCRETE_START(dss_input_stream)
        public override void start()
        {
            base.start();

            /* Stream out number is set during start */
            m_stream_in_number = (UInt32)DSS_INPUT_STREAM__STREAM;
            m_gain = DSS_INPUT_STREAM__GAIN;
            m_offset = DSS_INPUT_STREAM__OFFSET;
            m_inview = null;

            m_is_buffered = is_buffered() ? (byte)1 : (byte)0;
            m_buffer_stream = null;
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


        // discrete_step_interface

        //DISCRETE_STEP(dss_input_stream)
        public void step()
        {
            /* the context pointer is set to point to the current input stream data in discrete_stream_update */
            if (m_inview != null)
            {
                set_output(0, m_inview.get((s32)m_inview_sample) * 32768.0 * m_gain + m_offset);
                m_inview_sample++;
            }
            else
            {
                set_output(0, 0);
            }
        }


        void stream_generate(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //void discrete_dss_input_stream_node::stream_generate(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs)
        {
            outputs[0].fill((stream_buffer_sample_t)(m_data * (1.0 / 32768.0)));
        }


        /* This is called by discrete_sound_device */
        public void stream_start()
        {
            if (m_is_buffered != 0)
            {
                /* stream_buffered input only supported for sound devices */
                discrete_sound_device snd_device = (discrete_sound_device)m_device;
                //assert(DSS_INPUT_STREAM__STREAM < snd_device->m_input_stream_list.count());

                m_buffer_stream = m_device.machine().sound().stream_alloc(snd_device, 0, 1, (u32)this.sample_rate(), stream_generate, sound_stream_flags.STREAM_DEFAULT_FLAGS);  //m_buffer_stream = m_device->machine().sound().stream_alloc(*snd_device, 0, 1, this->sample_rate(), stream_update_delegate(&discrete_dss_input_stream_node::stream_generate,this), STREAM_DEFAULT_FLAGS);

                snd_device.get_stream().set_input((int)m_stream_in_number, m_buffer_stream);
            }
        }
    }
}
