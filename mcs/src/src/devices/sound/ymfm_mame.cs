// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using int32_t = System.Int32;
using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using u64 = System.UInt64;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using mame.ymfm;

namespace mame
{
    // set this to 1 to use ymfm's built-in SSG implementation
    // set it to 0 to use MAME's ay8910 as the SSG implementation
    //#define USE_BUILTIN_SSG (1)

    // set this to control the output sample rate for SSG-based chips
    //#define SSG_FIDELITY (ymfm::OPN_FIDELITY_MED)


    //*********************************************************
    //  MAME INTERFACES
    //*********************************************************

    // ======================> ym_generic_device
    // generic base class for a standalone FM device; this class contains the shared
    // configuration helpers, timers, and ymfm interface implementation; it also
    // specifies pure virtual functions for read/write access, which means it
    // can be used as a generic proxy for systems that have multiple FM types that are
    // swappable
    public abstract class ym_generic_device : device_t
                                              //device_sound_interface,
                                              //ymfm::ymfm_interface
    {
        public class ymfm_interface_ym_generic_device : ymfm.ymfm_interface
        {
            ym_generic_device m_device;

            public ymfm_interface_ym_generic_device(ym_generic_device device) { m_device = device; }

            protected override void ymfm_sync_mode_write(uint8_t data) { m_device.ymfm_interface_ymfm_sync_mode_write(data); }
            protected override void ymfm_sync_check_interrupts() { m_device.ymfm_interface_ymfm_sync_check_interrupts(); }
            protected override void ymfm_set_timer(uint32_t tnum, int32_t duration_in_clocks) { m_device.ymfm_interface_ymfm_set_timer(tnum, duration_in_clocks); }
            protected override void ymfm_update_irq(bool asserted) { m_device.ymfm_interface_ymfm_update_irq(asserted); }
            protected override void ymfm_set_busy_end(uint32_t clocks) { m_device.ymfm_interface_ymfm_set_busy_end(clocks); }
            protected override bool ymfm_is_busy() { return m_device.ymfm_interface_ymfm_is_busy(); }
            protected override uint8_t ymfm_external_read(ymfm.access_class type, uint32_t address) { return m_device.ymfm_interface_ymfm_external_read(type, address); }
            protected override void ymfm_external_write(ymfm.access_class type, uint32_t address, uint8_t data) { m_device.ymfm_interface_ymfm_external_write(type, address, data); }
        }


        protected ymfm_interface_ym_generic_device m_ymfm_interface;
        ymfm.ymfm_engine_callbacks m_engine { get { return m_ymfm_interface.m_engine; } }


        // internal state
        attotime m_busy_end;             // busy end time
        emu_timer [] m_timer = new emu_timer[2];           // two timers
        devcb_write_line m_update_irq;   // IRQ update callback
        devcb_read8 [] m_io_read = new devcb_read8[2];        // up to 2 input port handlers
        devcb_write8 [] m_io_write = new devcb_write8[2];      // up to 2 output port handlers


        // constructor
        protected ym_generic_device(machine_config mconfig, string tag, device_t owner, uint32_t clock, device_type type)
            : base(mconfig, type, tag, owner, clock)
        {
            //device_sound_interface(mconfig, *this),

            m_ymfm_interface = new ymfm_interface_ym_generic_device(this);


            m_timer = new emu_timer[] { null, null };
            m_update_irq = new devcb_write_line(this);
            m_io_read = new devcb_read8[] { new devcb_read8(this), new devcb_read8(this) };
            m_io_write = new devcb_write8[] { new devcb_write8(this), new devcb_write8(this) };
        }


        // configuration helpers
        //auto irq_handler() { return m_update_irq.bind(); }
        //auto io_read_handler(int index = 0) { return m_io_read[index & 1].bind(); }
        //auto io_write_handler(int index = 0) { return m_io_write[index & 1].bind(); }

        // read access interface, implemented by the derived chip-specific class
        public abstract u8 read(offs_t offset);
        protected abstract u8 status_r();

        // write access interface, implemented by the derived chip-specific class
        public abstract void write(offs_t offset, u8 data);
        protected abstract void address_w(u8 data);
        protected abstract void data_w(u8 data);


        // the chip implementation calls this when a write happens to the mode
        // register, which could affect timers and interrupts; our responsibility
        // is to ensure the system is up to date before calling the engine's
        // engine_mode_write() method
        public void ymfm_interface_ymfm_sync_mode_write(uint8_t data)
        {
            machine().scheduler().synchronize(fm_mode_write, data);
        }


        // the chip implementation calls this when the chip's status has changed,
        // which may affect the interrupt state; our responsibility is to ensure
        // the system is up to date before calling the engine's
        // engine_check_interrupts() method
        public void ymfm_interface_ymfm_sync_check_interrupts()
        {
            // if we're currently executing a CPU, schedule the interrupt check;
            // otherwise, do it directly
            var scheduler = machine().scheduler();
            if (scheduler.currently_executing() != null)
                scheduler.synchronize(fm_check_interrupts);
            else
                m_engine.engine_check_interrupts();
        }


        // the chip implementation calls this when one of the two internal timers
        // has changed state; our responsibility is to arrange to call the engine's
        // engine_timer_expired() method after the provided number of clocks; if
        // duration_in_clocks is negative, we should cancel any outstanding timers
        public void ymfm_interface_ymfm_set_timer(uint32_t tnum, int32_t duration_in_clocks)
        {
            if (duration_in_clocks >= 0)
                m_timer[tnum].adjust(attotime.from_ticks((u64)duration_in_clocks, clock()), (int)tnum);
            else
                m_timer[tnum].enable(false);
        }


        // the chip implementation calls this when the state of the IRQ signal has
        // changed due to a status change; our responsibility is to respons as
        // needed to the change in IRQ state, signaling any consumers
        public void ymfm_interface_ymfm_update_irq(bool asserted)
        {
            if (!m_update_irq.isnull())
                m_update_irq.op_s32(asserted ? g.ASSERT_LINE : g.CLEAR_LINE);
        }


        // the chip implementation calls this to indicate that the chip should be
        // considered in a busy state until the given number of clocks has passed;
        // our responsibility is to compute and remember the ending time based on
        // the chip's clock for later checking
        public void ymfm_interface_ymfm_set_busy_end(uint32_t clocks)
        {
            m_busy_end = machine().time() + attotime.from_ticks(clocks, clock());
        }


        // the chip implementation calls this to see if the chip is still currently
        // is a busy state, as specified by a previous call to ymfm_set_busy_end();
        // our responsibility is to compare the current time against the previously
        // noted busy end time and return true if we haven't yet passed it
        public bool ymfm_interface_ymfm_is_busy()
        {
            return (machine().time() < m_busy_end);
        }


        // the chip implementation calls this whenever data is read from outside
        // of the chip; our responsibility is to provide the data requested
        public uint8_t ymfm_interface_ymfm_external_read(ymfm.access_class type, uint32_t address)
        {
            return (type != ymfm.access_class.ACCESS_IO || m_io_read[address & 1].isnull()) ? (u8)0 : m_io_read[address & 1].op_u8();
        }


        // the chip implementation calls this whenever data is written outside
        // of the chip; our responsibility is to pass the written data on to any consumers
        public void ymfm_interface_ymfm_external_write(ymfm.access_class type, uint32_t address, uint8_t data)
        {
            if (type == ymfm.access_class.ACCESS_IO && !m_io_write[address & 1].isnull())
                m_io_write[address & 1].op_u8(data);
        }


        // handle device start
        protected override void device_start()
        {
            // allocate our timers
            for (int tnum = 0; tnum < 2; tnum++)
                m_timer[tnum] = machine().scheduler().timer_alloc(fm_timer_handler);

            // resolve the handlers
            m_update_irq.resolve();
            m_io_read[0].resolve();
            m_io_read[1].resolve();
            m_io_write[0].resolve();
            m_io_write[1].resolve();
        }


        // timer callbacks
        void fm_mode_write(object ptr, int param) { m_engine.engine_mode_write((uint8_t)param); }  //void fm_mode_write(void *ptr, int param) { m_engine->engine_mode_write(param); }
        void fm_check_interrupts(object ptr, int param) { m_engine.engine_check_interrupts(); }  //void fm_check_interrupts(void *ptr, int param) { m_engine->engine_check_interrupts(); }
        void fm_timer_handler(object ptr, int param) { m_engine.engine_timer_expired((uint32_t)param); }  //void fm_timer_handler(void *ptr, int param) { m_engine->engine_timer_expired(param); }
    }


    public interface ChipClass_operators<RegisterType, RegisterType_OPS>
        where RegisterType : opm_registers, new()
        where RegisterType_OPS : fm_engine_base_operators, new()
    {
        uint32_t OUTPUTS { get; }

        void reset();
        void save_restore(ymfm.ymfm_saved_state state);
        uint32_t sample_rate(uint32_t input_clock);
        uint8_t read_status();
        uint8_t read(uint32_t offset);
        void write_address(uint8_t data);
        void write_data(uint8_t data);
        void write(uint32_t offset, uint8_t data);
        void generate(fm_engine_base<RegisterType, RegisterType_OPS>.output_data [] output, uint32_t numsamples = 1);
    }


    // ======================> ymfm_device_base
    // this template provides most of the basics used by device objects in MAME
    // that wrap ymfm chips; it provides basic read/write functions; however, this
    // class is not intended to be used directly -- rather, devices should inherit
    // from either ymfm_device_base or ymfm_ssg_device_base, depending on whether
    // they include an SSG or not

    //template<typename ChipClass, bool FMOnly = false>
    public class ymfm_device_base<ChipClass, ChipClass_Registers, ChipClass_Registers_OPS> : ymfm_device_base<ChipClass, ChipClass_Registers, ChipClass_Registers_OPS, bool_const_false>
        where ChipClass : ChipClass_operators<ChipClass_Registers, ChipClass_Registers_OPS>
        where ChipClass_Registers : opm_registers, new()
        where ChipClass_Registers_OPS : fm_engine_base_operators, new()
    {
        protected ymfm_device_base(machine_config mconfig, string tag, device_t owner, uint32_t clock, device_type type, Func<ymfm_interface, ChipClass> chip_class_creator) : base(mconfig, tag, owner, clock, type, chip_class_creator) { }
    }

    //template<typename ChipClass, bool FMOnly = false>
    public class ymfm_device_base<ChipClass, ChipClass_Registers, ChipClass_Registers_OPS, bool_FMOnly> : ym_generic_device
        where ChipClass : ChipClass_operators<ChipClass_Registers, ChipClass_Registers_OPS>
        where ChipClass_Registers : opm_registers, new()
        where ChipClass_Registers_OPS : fm_engine_base_operators, new()
        where bool_FMOnly : bool_const, new()
    {
        public class device_sound_interface_ymfm_device_base : device_sound_interface
        {
            public device_sound_interface_ymfm_device_base(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((ymfm_device_base<ChipClass, ChipClass_Registers, ChipClass_Registers_OPS, bool_FMOnly>)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }
        }


        static readonly bool FMOnly = new bool_FMOnly().value;


        // for SSG chips, we only create a subset of outputs here:
        // YM2203 is 4 outputs: 1 mono FM + 3 SSG
        // YM2608/2610 is 3 outputs: 2 stereo FM + 1 SSG
        int OUTPUTS;  //static constexpr int OUTPUTS = FMOnly ? ((ChipClass::OUTPUTS == 4) ? 1 : 2) : ChipClass::OUTPUTS;


        device_sound_interface_ymfm_device_base m_disound;


        // internal state
        sound_stream m_stream;           // sound stream
        ChipClass m_chip;                 // core chip implementation
        std.vector<uint8_t> m_save_blob; // state saving buffer


        // constructor
        protected ymfm_device_base(machine_config mconfig, string tag, device_t owner, uint32_t clock, device_type type, Func<ymfm_interface, ChipClass> chip_class_creator)
            : base(mconfig, tag, owner, clock, type)
        {
            m_class_interfaces.Add(new device_sound_interface_ymfm_device_base(mconfig, this));  //device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_ymfm_device_base>();


            m_stream = null;
            m_chip = chip_class_creator(m_ymfm_interface);  //eg, m_chip = new ym2151(this);

            OUTPUTS = FMOnly ? ((m_chip.OUTPUTS == 4) ? 1 : 2) : (int)m_chip.OUTPUTS;  //static constexpr int OUTPUTS = FMOnly ? ((ChipClass::OUTPUTS == 4) ? 1 : 2) : ChipClass::OUTPUTS;
        }


        public device_sound_interface_ymfm_device_base disound { get { return m_disound; } }


        // read access: update the streams before performing the read
        public override u8 read(offs_t offset) { return update_streams().read(offset); }
        protected override u8 status_r() { return update_streams().read_status(); }

        // write access: update the strams before performing the write
        public override void write(offs_t offset, u8 data) { update_streams().write(offset, data); }
        protected override void address_w(u8 data) { update_streams().write_address(data); }
        protected override void data_w(u8 data) { update_streams().write_data(data); }


        // handle device start
        protected override void device_start()
        {
            // let our parent do its startup
            base.device_start();

            // allocate our stream
            m_stream = m_disound.stream_alloc(0, OUTPUTS, m_chip.sample_rate(clock()));

            // compute the size of the save buffer by doing an initial save
            ymfm.ymfm_saved_state state = new ymfm.ymfm_saved_state(m_save_blob, true);
            m_chip.save_restore(state);

            // now register the blob for save, on the assumption the size won't change
            save_item(g.NAME(new { m_save_blob }));
        }


        // device reset
        protected override void device_reset()
        {
            m_chip.reset();
        }


        // handle clock changed
        protected override void device_clock_changed()
        {
            if (m_stream != null)
                m_stream.set_sample_rate(m_chip.sample_rate(clock()));
        }


        // handle pre-saving by filling the blob
        protected override void device_pre_save()
        {
            // remember the original blob size
            var orig_size = m_save_blob.size();

            // save the state
            ymfm.ymfm_saved_state state = new ymfm.ymfm_saved_state(m_save_blob, true);
            m_chip.save_restore(state);

            // ensure that the size didn't change since we first allocated
            if (m_save_blob.size() != orig_size)
                throw new emu_fatalerror("State size changed for ymfm chip");
        }


        // handle post-loading by restoring from the blob
        protected override void device_post_load()
        {
            // populate the state from the blob
            ymfm.ymfm_saved_state state = new ymfm.ymfm_saved_state(m_save_blob, false);
            m_chip.save_restore(state);
        }


        // sound overrides
        public void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)
        {
            update_internal(outputs);
        }


        // update streams
        protected virtual ChipClass update_streams()
        {
            m_stream.update();
            return m_chip;
        }


        // internal update helper
        void update_internal(std.vector<write_stream_view> outputs, int output_shift = 0)
        {
            // local buffer to hold samples
            int MAX_SAMPLES = 256;
            fm_engine_base<ChipClass_Registers, ChipClass_Registers_OPS>.output_data [] output = new fm_engine_base<ChipClass_Registers, ChipClass_Registers_OPS>.output_data[MAX_SAMPLES];  //typename ChipClass::output_data output[MAX_SAMPLES];

            // parameters
            int outcount = (int)std.min(outputs.size(), std.size(output[0].data));
            int numsamples = (int)outputs[0].samples();

            // generate the FM/ADPCM stream
            for (int sampindex = 0; sampindex < numsamples; sampindex += MAX_SAMPLES)
            {
                int cursamples = std.min(numsamples - sampindex, MAX_SAMPLES);
                m_chip.generate(output, (uint32_t)cursamples);
                for (int outnum = 0; outnum < outcount; outnum++)
                {
                    int eff_outnum = (outnum + output_shift) % OUTPUTS;
                    for (int index = 0; index < cursamples; index++)
                        outputs[eff_outnum].put_int(sampindex + index, output[index].data[outnum], 32768);
                }
            }
        }
    }


    // ======================> ymfm_ssg_internal_device_base
    // this template adds SSG support to the base template, using ymfm's internal
    // SSG implementation
    //template<typename ChipClass>
    //class ymfm_ssg_internal_device_base : public ymfm_device_base<ChipClass>


    // ======================> ymfm_ssg_external_device_base
    // this template adds SSG support to the base template, using MAME's YM2149
    // implementation in ay8910.cpp; this is the "classic" way to do it in MAME
    // and is more flexible in terms of output handling
    //template<typename ChipClass>
    //class ymfm_ssg_external_device_base : public ymfm_device_base<ChipClass, true>, public ymfm::ssg_override


    // now pick the right one
    //#if USE_BUILTIN_SSG
    //template<typename ChipClass>
    //using ymfm_ssg_device_base = ymfm_ssg_internal_device_base<ChipClass>;
    //#else
    //template<typename ChipClass>
    //using ymfm_ssg_device_base = ymfm_ssg_external_device_base<ChipClass>;
    //#endif
}
