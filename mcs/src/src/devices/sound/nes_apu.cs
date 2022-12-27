// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using offs_t = System.UInt32;  //using offs_t = u32;
using s8 = System.SByte;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.device_global;
using static mame.emucore_global;
using static mame.nes_apu_global;
using static mame.nes_apu_internal;
using static mame.nes_defs_global;
using static mame.util;


namespace mame
{
    public class nesapu_device : device_t
                                 //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(NES_APU, nesapu_device, "nesapu", "N2A03 APU")
        public static readonly emu.detail.device_type_impl NES_APU = DEFINE_DEVICE_TYPE("nesapu", "N2A03 APU", (type, mconfig, tag, owner, clock) => { return new nesapu_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_nesapu_device : device_sound_interface
        {
            public device_sound_interface_nesapu_device(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((nesapu_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        /* GLOBAL CONSTANTS */
        const unsigned SYNCS_MAX1     = 0x20;
        const unsigned SYNCS_MAX2     = 0x80;
        //static constexpr u32       NTSC_APU_CLOCK = 21477272 / 12;
        const u32 PAL_APU_CLOCK       = 26601712 / 16;


        device_sound_interface_nesapu_device m_disound;


        // internal state
        apu_t m_APU = new apu_t();                   /* Actual APUs */
        int m_is_pal;
        u32 m_samps_per_sync;        /* Number of samples per vsync */
        u32 [] m_vbl_times = new u32 [SYNCS_MAX1];       /* VBL durations in samples */
        u32 [] m_sync_times1 = new u32 [SYNCS_MAX1]; /* Samples per sync table */
        u32 [] m_sync_times2 = new u32 [SYNCS_MAX2]; /* Samples per sync table */
        stream_buffer_sample_t [] m_square_lut = new stream_buffer_sample_t [31];       // Non-linear Square wave output LUT
        stream_buffer_sample_t [,,] m_tnd_lut = new stream_buffer_sample_t [16, 16, 128]; // Non-linear Triangle, Noise, DMC output LUT
        sound_stream m_stream;
        devcb_write_line m_irq_handler;
        devcb_read8 m_mem_read_cb;


        nesapu_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : this(mconfig, NES_APU, tag, owner, clock)
        { }


        nesapu_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_nesapu_device(mconfig, this));  //device_sound_interface(mconfig, *this),
            m_disound = GetClassInterface<device_sound_interface_nesapu_device>();


            m_is_pal = 0;
            m_samps_per_sync = 0;
            m_stream = null;
            m_irq_handler = new devcb_write_line(this);
            m_mem_read_cb = new devcb_read8(this);
        }


        public device_sound_interface_nesapu_device disound { get { return m_disound; } }


        // configuration helpers
        public devcb_write_line.binder irq() { return m_irq_handler.bind(); }  //auto irq() { return m_irq_handler.bind(); }
        public devcb_read8.binder mem_read() { return m_mem_read_cb.bind(); }  //auto mem_read() { return m_mem_read_cb.bind(); }


        protected override void device_reset()
        {
            write(0x15, 0x00);
        }


        protected override void device_clock_changed()
        {
            calculate_rates();
            m_is_pal = m_clock == PAL_APU_CLOCK ? 1 : 0;
        }


        public u8 read(offs_t offset) { throw new emu_unimplemented(); }


        /* WRITE REGISTER VALUE */
        public void write(offs_t offset, u8 value)
        {
            m_stream.update();

            int chan = (int)BIT(offset, 2);

            switch (offset)
            {
            /* squares */
            case apu_t.WRA0:
            case apu_t.WRB0:
                m_APU.squ[chan].regs[0] = value;
                break;

            case apu_t.WRA1:
            case apu_t.WRB1:
                m_APU.squ[chan].regs[1] = value;
                break;

            case apu_t.WRA2:
            case apu_t.WRB2:
                m_APU.squ[chan].regs[2] = value;
                if (m_APU.squ[chan].enabled)
                    m_APU.squ[chan].freq = ((((m_APU.squ[chan].regs[3] & 7) << 8) + value) + 1) << 16;
                break;

            case apu_t.WRA3:
            case apu_t.WRB3:
                m_APU.squ[chan].regs[3] = value;

                if (m_APU.squ[chan].enabled)
                {
                    m_APU.squ[chan].vbl_length = (int)m_vbl_times[value >> 3];
                    m_APU.squ[chan].env_vol = 0;
                    m_APU.squ[chan].freq = ((((value & 7) << 8) + m_APU.squ[chan].regs[2]) + 1) << 16;
                }

                break;

            /* triangle */
            case apu_t.WRC0:
                m_APU.tri.regs[0] = value;

                if (m_APU.tri.enabled)
                {                                          /* ??? */
                    if (!m_APU.tri.counter_started)
                        m_APU.tri.linear_length = (int)m_sync_times2[value & 0x7f];
                }

                break;

            case 0x4009:
                /* unused */
                m_APU.tri.regs[1] = value;
                break;

            case apu_t.WRC2:
                m_APU.tri.regs[2] = value;
                break;

            case apu_t.WRC3:
                m_APU.tri.regs[3] = value;

                /* this is somewhat of a hack.  there is some latency on the Real
                ** Thing between when trireg0 is written to and when the linear
                ** length counter actually begins its countdown.  we want to prevent
                ** the case where the program writes to the freq regs first, then
                ** to reg 0, and the counter accidentally starts running because of
                ** the sound queue's timestamp processing.
                **
                ** set to a few NES sample -- should be sufficient
                **
                **    3 * (1789772.727 / 44100) = ~122 cycles, just around one scanline
                **
                ** should be plenty of time for the 6502 code to do a couple of table
                ** dereferences and load up the other triregs
                */

            /* used to be 3, but now we run the clock faster, so base it on samples/sync */
                m_APU.tri.write_latency = (int)((m_samps_per_sync + 239) / 240);

                if (m_APU.tri.enabled)
                {
                    m_APU.tri.counter_started = false;
                    m_APU.tri.vbl_length = (int)m_vbl_times[value >> 3];
                    m_APU.tri.linear_length = (int)m_sync_times2[m_APU.tri.regs[0] & 0x7f];
                    m_APU.tri.linear_reload = true;
                }

                break;

            /* noise */
            case apu_t.WRD0:
                m_APU.noi.regs[0] = value;
                break;

            case 0x400D:
                /* unused */
                m_APU.noi.regs[1] = value;
                break;

            case apu_t.WRD2:
                m_APU.noi.regs[2] = value;
                break;

            case apu_t.WRD3:
                m_APU.noi.regs[3] = value;

                if (m_APU.noi.enabled)
                {
                    m_APU.noi.vbl_length = (int)m_vbl_times[value >> 3];
                    m_APU.noi.env_vol = 0; /* reset envelope */
                }
                break;

            /* DMC */
            case apu_t.WRE0:
                m_APU.dpcm.regs[0] = value;
                if ((value & 0x80) == 0)
                {
                    m_irq_handler.op_s32(0);
                    m_APU.dpcm.irq_occurred = false;
                }
                break;

            case apu_t.WRE1: /* 7-bit DAC */
                m_APU.dpcm.regs[1] = (u8)(value & 0x7f);
                m_APU.dpcm.vol = m_APU.dpcm.regs[1];
                break;

            case apu_t.WRE2:
                m_APU.dpcm.regs[2] = value;
                //apu_dpcmreset(m_APU.dpcm);
                break;

            case apu_t.WRE3:
                m_APU.dpcm.regs[3] = value;
                break;

            case apu_t.IRQCTRL:
                if ((value & 0x80) != 0)
                    m_APU.step_mode = 5;
                else
                    m_APU.step_mode = 4;
                break;

            case apu_t.SMASK:
                if ((value & 0x01) != 0)
                {
                    m_APU.squ[0].enabled = true;
                }
                else
                {
                    m_APU.squ[0].enabled = false;
                    m_APU.squ[0].vbl_length = 0;
                }

                if ((value & 0x02) != 0)
                {
                    m_APU.squ[1].enabled = true;
                }
                else
                {
                    m_APU.squ[1].enabled = false;
                    m_APU.squ[1].vbl_length = 0;
                }

                if ((value & 0x04) != 0)
                {
                    m_APU.tri.enabled = true;
                }
                else
                {
                    m_APU.tri.enabled = false;
                    m_APU.tri.vbl_length = 0;
                    m_APU.tri.linear_length = 0;
                    m_APU.tri.counter_started = false;
                    m_APU.tri.write_latency = 0;
                }

                if ((value & 0x08) != 0)
                {
                    m_APU.noi.enabled = true;
                }
                else
                {
                    m_APU.noi.enabled = false;
                    m_APU.noi.vbl_length = 0;
                }

                if ((value & 0x10) != 0)
                {
                    /* only reset dpcm values if DMA is finished */
                    if (!m_APU.dpcm.enabled)
                    {
                        m_APU.dpcm.enabled = true;
                        apu_dpcmreset(m_APU.dpcm);
                    }
                }
                else
                {
                    m_APU.dpcm.enabled = false;
                }

                //m_irq_handler(false);
                m_APU.dpcm.irq_occurred = false;

                break;

            default:
#if MAME_DEBUG
                logerror("invalid apu write: $%02X at $%04X\n", value, offset);
#endif
                break;
            }
        }


        // device-level overrides
        protected override void device_start()
        {
            // resolve callbacks
            m_irq_handler.resolve_safe();
            m_mem_read_cb.resolve_safe_u8(0x00);

            calculate_rates();

            // calculate mixer output
            /*
            pulse channel output:

                     95.88
            -----------------------
                  8128
            ----------------- + 100
            pulse 1 + pulse 2

            */
            for (int i = 0; i < 31; i++)
            {
                stream_buffer_sample_t pulse_out = (i == 0) ? 0 : (stream_buffer_sample_t)(95.88 / ((8128.0 / i) + 100.0));
                m_square_lut[i] = pulse_out;
            }

            /*
            triangle, noise, DMC channel output:

                         159.79
            -------------------------------
                        1
            ------------------------- + 100
            triangle   noise    dmc
            -------- + ----- + -----
              8227     12241   22638

            */
            for (int t = 0; t < 16; t++)
            {
                for (int n = 0; n < 16; n++)
                {
                    for (int d = 0; d < 128; d++)
                    {
                        stream_buffer_sample_t tnd_out = (stream_buffer_sample_t)((t / 8227.0) + (n / 12241.0) + (d / 22638.0));
                        tnd_out = (tnd_out == 0.0) ? 0 : (stream_buffer_sample_t)(159.79 / ((1.0 / tnd_out) + 100.0));
                        m_tnd_lut[t, n, d] = tnd_out;
                    }
                }
            }

            /* register for save */
            for (int i = 0; i < 2; i++)
            {
                save_item(NAME(new { m_APU.squ[i].regs }), i);
                save_item(NAME(new { m_APU.squ[i].vbl_length }), i);
                save_item(NAME(new { m_APU.squ[i].freq }), i);
                save_item(NAME(new { m_APU.squ[i].phaseacc }), i);
                save_item(NAME(new { m_APU.squ[i].env_phase }), i);
                save_item(NAME(new { m_APU.squ[i].sweep_phase }), i);
                save_item(NAME(new { m_APU.squ[i].adder }), i);
                save_item(NAME(new { m_APU.squ[i].env_vol }), i);
                save_item(NAME(new { m_APU.squ[i].enabled }), i);
                save_item(NAME(new { m_APU.squ[i].output }), i);
            }

            save_item(NAME(new { m_APU.tri.regs }));
            save_item(NAME(new { m_APU.tri.linear_length }));
            save_item(NAME(new { m_APU.tri.linear_reload }));
            save_item(NAME(new { m_APU.tri.vbl_length }));
            save_item(NAME(new { m_APU.tri.write_latency }));
            save_item(NAME(new { m_APU.tri.phaseacc }));
            save_item(NAME(new { m_APU.tri.adder }));
            save_item(NAME(new { m_APU.tri.counter_started }));
            save_item(NAME(new { m_APU.tri.enabled }));
            save_item(NAME(new { m_APU.tri.output }));

            save_item(NAME(new { m_APU.noi.regs }));
            save_item(NAME(new { m_APU.noi.seed }));
            save_item(NAME(new { m_APU.noi.vbl_length }));
            save_item(NAME(new { m_APU.noi.phaseacc }));
            save_item(NAME(new { m_APU.noi.env_phase }));
            save_item(NAME(new { m_APU.noi.env_vol }));
            save_item(NAME(new { m_APU.noi.enabled }));
            save_item(NAME(new { m_APU.noi.output }));

            save_item(NAME(new { m_APU.dpcm.regs }));
            save_item(NAME(new { m_APU.dpcm.address }));
            save_item(NAME(new { m_APU.dpcm.length }));
            save_item(NAME(new { m_APU.dpcm.bits_left }));
            save_item(NAME(new { m_APU.dpcm.phaseacc }));
            save_item(NAME(new { m_APU.dpcm.cur_byte }));
            save_item(NAME(new { m_APU.dpcm.enabled }));
            save_item(NAME(new { m_APU.dpcm.irq_occurred }));
            save_item(NAME(new { m_APU.dpcm.vol }));
            save_item(NAME(new { m_APU.dpcm.output }));

            save_item(NAME(new { m_APU.step_mode }));
        }


        // sound stream update overrides
        protected virtual void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { throw new emu_unimplemented(); }


        void calculate_rates()
        {
            m_samps_per_sync = 89490 / 12; // Is there a different PAL value?

            // initialize sample times in terms of vsyncs
            for (int i = 0; i < SYNCS_MAX1; i++)
            {
                m_vbl_times[i] = vbl_length[i] * m_samps_per_sync / 2;
                m_sync_times1[i] = (u32)(m_samps_per_sync * (i + 1));
            }

            for (int i = 0; i < SYNCS_MAX2; i++)
                m_sync_times2[i] = (u32)((m_samps_per_sync * i) >> 2);

            int rate = (int)(clock() / 4);

            if (m_stream != null)
                m_stream.set_sample_rate((u32)rate);
            else
                m_stream = disound.stream_alloc(0, 1, (u32)rate);
        }


        //void apu_square(apu_t::square_t *chan);
        //void apu_triangle(apu_t::triangle_t *chan);
        //void apu_noise(apu_t::noise_t *chan);
        //void apu_dpcm(apu_t::dpcm_t *chan);
    }


    static class nes_apu_internal
    {
        /* RESET DPCM PARAMETERS */
        public static void apu_dpcmreset(apu_t.dpcm_t chan)
        {
            chan.address = (u32)0xc000 + (u16)(chan.regs[2] << 6);
            chan.length = (u16)(chan.regs[3] << 4) + 1U;
            chan.bits_left = (int)(chan.length << 3);
            chan.irq_occurred = false;
            chan.enabled = true; /* Fixed * Proper DPCM channel ENABLE/DISABLE flag behaviour*/
        }
    }


    static class nes_apu_global
    {
        public static nesapu_device NES_APU<bool_Required>(machine_config mconfig, device_finder<nesapu_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, nesapu_device.NES_APU, clock); }
    }
}
