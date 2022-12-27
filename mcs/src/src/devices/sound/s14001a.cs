// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.s14001a_global;


namespace mame
{
    public class s14001a_device : device_t
                           //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(S14001A, s14001a_device, "s14001a", "SSi TSI S14001A")
        public static readonly emu.detail.device_type_impl S14001A = DEFINE_DEVICE_TYPE("s14001a", "SSi TSI S14001A", (type, mconfig, tag, owner, clock) => { return new s14001a_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_s14001a : device_sound_interface
        {
            public device_sound_interface_s14001a(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((s14001a_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        //enum class states : u8
        //{
        //    IDLE = 0,
        //    WORDWAIT,
        //    CWARMSB,    // read 8 CWAR MSBs
        //    CWARLSB,    // read 4 CWAR LSBs from rom d7-d4
        //    DARMSB,     // read 8 DAR  MSBs
        //    CTRLBITS,   // read Stop, Voiced, Silence, Length, XRepeat
        //    PLAY,
        //    DELAY
        //};


        device_sound_interface_s14001a m_disound;


        required_region_ptr<uint8_t> m_SpeechRom;
        sound_stream m_stream;

        devcb_write_line m_bsy_handler;
        devcb_read8 m_ext_read_handler;

        // internal state
        //bool m_bPhase1; // 1 bit internal clock

        // registers
        //states m_uStateP1;          // 3 bits
        //states m_uStateP2;

        //uint16_t m_uDAR13To05P1;      // 9 MSBs of delta address register
        //uint16_t m_uDAR13To05P2;      // incrementing uDAR05To13 advances ROM address by 8 bytes

        //uint16_t m_uDAR04To00P1;      // 5 LSBs of delta address register
        //uint16_t m_uDAR04To00P2;      // 3 address ROM, 2 mux 8 bits of data into 2 bit delta
                                    // carry indicates end of quarter pitch period (32 cycles)

        //uint16_t m_uCWARP1;           // 12 bits Control Word Address Register (syllable)
        //uint16_t m_uCWARP2;

        //bool m_bStopP1;
        //bool m_bStopP2;
        //bool m_bVoicedP1;
        //bool m_bVoicedP2;
        //bool m_bSilenceP1;
        //bool m_bSilenceP2;
        //uint8_t m_uLengthP1;          // 7 bits, upper three loaded from ROM length
        //uint8_t m_uLengthP2;          // middle two loaded from ROM repeat and/or uXRepeat
                                    // bit 0 indicates mirror in voiced mode
                                    // bit 1 indicates internal silence in voiced mode
                                    // incremented each pitch period quarter

        //uint8_t m_uXRepeatP1;         // 2 bits, loaded from ROM repeat
        //uint8_t m_uXRepeatP2;
        //uint8_t m_uDeltaOldP1;        // 2 bit old delta
        //uint8_t m_uDeltaOldP2;
        //uint8_t m_uOutputP1;          // 4 bits audio output, calculated during phase 1

        // derived signals
        //bool m_bDAR04To00CarryP2;
        //bool m_bPPQCarryP2;
        //bool m_bRepeatCarryP2;
        //bool m_bLengthCarryP2;
        //uint16_t m_RomAddrP1;         // rom address

        // output pins
        //uint8_t m_uOutputP2;          // output changes on phase2
        //uint16_t m_uRomAddrP2;        // address pins change on phase 2
        //bool m_bBusyP1;             // busy changes on phase 1

        // input pins
        //bool m_bStart;
        //uint8_t m_uWord;              // 6 bit word number to be spoken

        // emulator variables
        // statistics
        //uint32_t m_uNPitchPeriods;
        //uint32_t m_uNVoiced;
        //uint32_t m_uNControlWords;

        // diagnostic output
        //uint32_t m_uPrintLevel;


        s14001a_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, S14001A, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_s14001a(mconfig, this));  // device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_s14001a>();

            m_SpeechRom = new required_region_ptr<uint8_t>(this, DEVICE_SELF);
            m_stream = null;
            m_bsy_handler = new devcb_write_line(this);
            m_ext_read_handler = new devcb_read8(this);
        }


        public device_sound_interface_s14001a disound { get { return m_disound; } }


        // configuration helpers
        //auto bsy() { return m_bsy_handler.bind(); }
        //auto ext_read() { return m_ext_read_handler.bind(); }

        //int busy_r();              // /BUSY (pin 40)
        //int romen_r();             // ROM /EN (pin 9)
        //void start_w(int state);   // START (pin 10)
        //void data_w(uint8_t data); // 6-bit word

        //void set_clock(uint32_t clock);       // set new CLK frequency
        //void set_clock(const XTAL &xtal) { set_clock(xtal.value()); }
        //void force_update();                // update stream, eg. before external ROM bankswitch


        // device-level overrides
        protected override void device_start()
        {
            throw new emu_unimplemented();
        }


        // sound stream update overrides
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            throw new emu_unimplemented();
        }


        //uint8_t readmem(uint16_t offset, bool phase);
        //bool Clock(); // called once to toggle external clock twice

        // emulator helper functions
        //void ClearStatistics();
        //void GetStatistics(uint32_t &uNPitchPeriods, uint32_t &uNVoiced, uint32_t &uNControlWords);
        //void SetPrintLevel(uint32_t uPrintLevel) { m_uPrintLevel = uPrintLevel; }
    }


    public static class s14001a_global
    {
        public static s14001a_device S14001A<bool_Required>(machine_config mconfig, device_finder<s14001a_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, s14001a_device.S14001A, clock); }
    }
}
