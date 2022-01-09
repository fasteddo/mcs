// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;

using static mame.device_global;


namespace mame
{
    // ======================> digitalker_device
    public class digitalker_device : device_t
                                     //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(DIGITALKER, digitalker_device, "digitalker", "MM54104 Digitalker")
        static device_t device_creator_digitalker_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new digitalker_device(mconfig, tag, owner, clock); }
        public static readonly device_type DIGITALKER = DEFINE_DEVICE_TYPE(device_creator_digitalker_device, "digitalker", "MM54104 Digitalker");


        class device_sound_interface_digitalker : device_sound_interface
        {
            public device_sound_interface_digitalker(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { throw new emu_unimplemented(); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        device_sound_interface_digitalker m_disound;


        //required_region_ptr<uint8_t> m_rom;
        //sound_stream *m_stream;

        // Port/lines state
        //uint8_t m_data;
        //uint8_t m_cs;
        //uint8_t m_cms;
        //uint8_t m_wr;
        //uint8_t m_intr;

        // Current decoding state
        //uint16_t m_bpos;
        //uint16_t m_apos;

        //uint8_t m_mode;
        //uint8_t m_cur_segment;
        //uint8_t m_cur_repeat;
        //uint8_t m_segments;
        //uint8_t m_repeats;

        //uint8_t m_prev_pitch;
        //uint8_t m_pitch;
        //uint8_t m_pitch_pos;

        //uint8_t m_stop_after;
        //uint8_t m_cur_dac;
        //uint8_t m_cur_bits;

        // Zero-range size
        //uint32_t m_zero_count; // 0 for done

        // Waveform and current index in it
        //uint8_t m_dac_index; // 128 for done
        //int16_t m_dac[128];


        digitalker_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, DIGITALKER, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_digitalker(mconfig, this));  // device_sound_interface(mconfig, *this);


            throw new emu_unimplemented();
        }


        //void digitalker_0_cs_w(int line);
        //void digitalker_0_cms_w(int line);
        //void digitalker_0_wr_w(int line);
        //int digitalker_0_intr_r();

        //void digitalker_data_w(uint8_t data);

        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            m_disound = GetClassInterface<device_sound_interface_digitalker>();


            throw new emu_unimplemented();
        }


        // sound stream update overrides
        //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;


        //void digitalker_write(uint8_t *adr, uint8_t vol, int8_t dac);
        //uint8_t digitalker_pitch_next(uint8_t val, uint8_t prev, int step);
        //void digitalker_set_intr(uint8_t intr);
        //void digitalker_start_command(uint8_t cmd);
        //void digitalker_step_mode_0();
        //void digitalker_step_mode_1();
        //void digitalker_step_mode_2();
        //void digitalker_step_mode_3();
        //void digitalker_step();
        //void digitalker_cs_w(int line);
        //void digitalker_cms_w(int line);
        //void digitalker_wr_w(int line);
        //int digitalker_intr_r();
        //void digitalker_register_for_save();
    }
}
