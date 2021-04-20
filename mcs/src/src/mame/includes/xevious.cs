// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using uint8_t = System.Byte;


namespace mame
{
    partial class xevious_state : galaga_state
    {
        required_shared_ptr<uint8_t> m_xevious_sr1;
        required_shared_ptr<uint8_t> m_xevious_sr2;
        required_shared_ptr<uint8_t> m_xevious_sr3;
        required_shared_ptr<uint8_t> m_xevious_fg_colorram;
        required_shared_ptr<uint8_t> m_xevious_bg_colorram;
        required_shared_ptr<uint8_t> m_xevious_fg_videoram;
        required_shared_ptr<uint8_t> m_xevious_bg_videoram;
        optional_device<samples_device> m_samples;

        int [] m_xevious_bs = new int[2];

        //UINT8 m_customio[16];
        //char m_battles_customio_command;
        //char m_battles_customio_prev_command;
        //char m_battles_customio_command_count;
        //char m_battles_customio_data;
        //char m_battles_sound_played;

        optional_device<cpu_device> m_subcpu3;


        public xevious_state(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag)
        {
            m_xevious_sr1 = new required_shared_ptr<uint8_t>(this, "xevious_sr1");
            m_xevious_sr2 = new required_shared_ptr<uint8_t>(this, "xevious_sr2");
            m_xevious_sr3 = new required_shared_ptr<uint8_t>(this, "xevious_sr3");
            m_xevious_fg_colorram = new required_shared_ptr<uint8_t>(this, "fg_colorram");
            m_xevious_bg_colorram = new required_shared_ptr<uint8_t>(this, "bg_colorram");
            m_xevious_fg_videoram = new required_shared_ptr<uint8_t>(this, "fg_videoram");
            m_xevious_bg_videoram = new required_shared_ptr<uint8_t>(this, "bg_videoram");
            m_samples = new optional_device<samples_device>(this, "samples");
            m_subcpu3 = new optional_device<cpu_device>(this, "sub3");
        }
    }
}
