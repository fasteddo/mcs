// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using attoseconds_t = System.Int64;
using device_timer_id = System.UInt32;
using device_type = mame.emu.detail.device_type_impl_base;
using ioport_value = System.UInt32;
using offs_t = System.UInt32;
using u32 = System.UInt32;


namespace mame
{
    public class device_init_helpers
    {
        machine_config m_helper_config = null;
        device_t m_helper_owner = null;
        device_t m_helper_device = null;
        address_map_entry m_helper_curentry = null;
        address_map m_helper_map = null;
        ioport_configurer m_helper_configurer = null;
        netlist.setup_t m_helper_setup = null;

        protected machine_config helper_config { set { m_helper_config = value; } }
        protected device_t helper_owner { get { return m_helper_owner; } private set { m_helper_owner = value; } }
        protected device_t helper_device { get { return m_helper_device; } private set { m_helper_device = value; } }
        address_map_entry helper_curentry { set { m_helper_curentry = value; } }
        address_map helper_address_map { set { m_helper_map = value; } }
        protected ioport_configurer helper_ioport_configurer { set { m_helper_configurer = value; } }


        // machine config helpers
        protected void MACHINE_CONFIG_START(machine_config config, device_t owner, device_t device)
        {
            //device_t *device = nullptr; \
            //devcb_base *devcb = nullptr; \
            //(void)device; \
            //(void)devcb;

            global.assert(device != null);

            helper_config = config;
            helper_owner = device;  // after we create the device, the owner becomes the device so that children are created under it.
            helper_device = null;

            mconfig_global.MACHINE_CONFIG_START();
        }

        protected void MACHINE_CONFIG_END()
        {
            mconfig_global.MACHINE_CONFIG_END();

            helper_owner = null;
            helper_curentry = null;
            helper_address_map = null;
            helper_device = null;
        }


        // _74259
        protected static ls259_device LS259(machine_config mconfig, string tag, u32 clock = 0) { return (ls259_device)mconfig.device_add(tag, ls259_device.LS259, clock); }
        protected static ls259_device LS259(machine_config mconfig, device_finder<ls259_device> finder, u32 clock = 0)
        {
            var target = finder.finder_target();  //std::pair<device_t &, char const *> const target(finder.finder_target());
            finder.target = LS259(mconfig, target.second(), clock);
            return finder.target;
        }
        // ay8910
        protected const int AY8910_INTERNAL_RESISTANCE = ay8910_global.AY8910_INTERNAL_RESISTANCE;
        protected static ay8910_device AY8910(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<ay8910_device>(mconfig, tag, ay8910_device.AY8910, clock); }
        protected static ay8910_device AY8910(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<ay8910_device>(mconfig, tag, ay8910_device.AY8910, clock); }
        protected static ay8910_device AY8910(machine_config mconfig, device_finder<ay8910_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, ay8910_device.AY8910, clock); }
        protected static ay8910_device AY8910(machine_config mconfig, device_finder<ay8910_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, ay8910_device.AY8910, clock); }
        // coretmpl
        protected static int bitswap(int val, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0) { return coretmpl_global.bitswap(val, B7, B6, B5, B4, B3, B2, B1, B0); }
        protected static int bitswap(int val, int B15, int B14, int B13, int B12, int B11, int B10, int B9, int B8, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0) { return coretmpl_global.bitswap(val, B15, B14, B13, B12, B11, B10, B9, B8, B7, B6, B5, B4, B3, B2, B1, B0); }
        // devcb
        protected static DEVCB_IOPORT IOPORT(string tag) { return new DEVCB_IOPORT(tag); }
        protected static read_line_delegate READLINE(string tag, read_line_delegate func) { return devcb_global.DEVCB_READLINE(tag, func); }
        protected static read_line_delegate READLINE(read_line_delegate func) { return devcb_global.DEVCB_READLINE(func); }
        protected static read8_delegate READ8(string tag, read8_delegate func) { return devcb_global.DEVCB_READ8(tag, func); }
        protected static read8_delegate READ8(read8_delegate func) { return devcb_global.DEVCB_READ8(func); }
        protected static write_line_delegate WRITELINE(string tag, write_line_delegate func) { return devcb_global.DEVCB_WRITELINE(tag, func); }
        protected static write_line_delegate WRITELINE(write_line_delegate func) { return devcb_global.DEVCB_WRITELINE(func); }
        protected static write8_delegate WRITE8(string tag, write8_delegate func) { return devcb_global.DEVCB_WRITE8(tag, func); }
        protected static write8_delegate WRITE8(write8_delegate func) { return devcb_global.DEVCB_WRITE8(func); }
        // device
        protected const string DEVICE_SELF = device_global.DEVICE_SELF;
        protected const string DEVICE_SELF_OWNER = device_global.DEVICE_SELF_OWNER;
        protected static u32 DERIVED_CLOCK(u32 num, u32 den) { return device_global.DERIVED_CLOCK(num, den); }
        protected static device_type DEFINE_DEVICE_TYPE(device_type.create_func func, string shortname, string fullname) { return device_global.DEFINE_DEVICE_TYPE(func, shortname, fullname); }
        // diexec
        protected void MCFG_DEVICE_DISABLE() { diexec_global.MCFG_DEVICE_DISABLE(m_helper_device); }
        protected void MCFG_DEVICE_VBLANK_INT_DRIVER(string tag, device_interrupt_delegate func) { diexec_global.MCFG_DEVICE_VBLANK_INT_DRIVER(m_helper_device, tag, func); }
        protected void MCFG_DEVICE_PERIODIC_INT_DRIVER(device_interrupt_delegate func, int rate) { diexec_global.MCFG_DEVICE_PERIODIC_INT_DRIVER(m_helper_device, func, rate); }
        // digfx
        protected static UInt32 RGN_FRAC(UInt32 num, UInt32 den) { return digfx_global.RGN_FRAC(num, den); }
        protected static UInt32 [] STEP2(int START, int STEP) { return digfx_global.STEP2(START, STEP); }
        protected static UInt32 [] STEP4(int START, int STEP) { return digfx_global.STEP4(START, STEP); }
        protected static UInt32 [] STEP8(int START, int STEP) { return digfx_global.STEP8(START, STEP); }
        // dimemory
        protected void MCFG_DEVICE_PROGRAM_MAP(address_map_constructor map) { dimemory_global.MCFG_DEVICE_PROGRAM_MAP(m_helper_device, map); }
        protected void MCFG_DEVICE_IO_MAP(address_map_constructor map) { dimemory_global.MCFG_DEVICE_IO_MAP(m_helper_device, map); }
        // discrete
        protected const int DISC_MIXER_IS_OP_AMP = discrete_global.DISC_MIXER_IS_OP_AMP;
        protected void MCFG_DISCRETE_INTF(discrete_block [] intf) { discrete_global.MCFG_DISCRETE_INTF(m_helper_device, intf); }
        protected static discrete_block DISCRETE_SOUND_END() { return discrete_global.DISCRETE_SOUND_END(); }
        protected static discrete_block DISCRETE_INPUT_DATA(NODE node) { return discrete_global.DISCRETE_INPUT_DATA(node); }
        protected static discrete_block DISCRETE_INPUTX_DATA(NODE node, double GAIN, double OFFSET, double INIT) { return discrete_global.DISCRETE_INPUTX_DATA(node, GAIN, OFFSET, INIT); }
        protected static discrete_block DISCRETE_INPUT_LOGIC(NODE node) { return discrete_global.DISCRETE_INPUT_LOGIC(node); }
        protected static discrete_block DISCRETE_INPUTX_STREAM(NODE node, int NUM, double GAIN, int OFFSET) { return discrete_global.DISCRETE_INPUTX_STREAM(node, NUM, GAIN, OFFSET); }
        protected static discrete_block DISCRETE_LFSR_NOISE(NODE node, int ENAB, int RESET, int CLK, double AMPL, int FEED, double BIAS, discrete_lfsr_desc LFSRTB) { return discrete_global.DISCRETE_LFSR_NOISE(node, ENAB, RESET, CLK, AMPL, FEED, BIAS, LFSRTB); }
        protected static discrete_block DISCRETE_NOTE(NODE node, int ENAB, int CLK, NODE DATA, int MAX1, int MAX2, int CLKTYPE) { return discrete_global.DISCRETE_NOTE(node, ENAB, CLK, DATA, MAX1, MAX2, CLKTYPE); }
        protected static discrete_block DISCRETE_SQUAREWFIX(NODE node, int ENAB, int FREQ, double AMPL, int DUTY, double BIAS, double PHASE) { return discrete_global.DISCRETE_SQUAREWFIX(node, ENAB, FREQ, AMPL, DUTY, BIAS, PHASE); }
        protected static discrete_block DISCRETE_CLAMP(NODE node, NODE INP0, double MIN, double MAX) { return discrete_global.DISCRETE_CLAMP(node, INP0, MIN, MAX); }
        protected static discrete_block DISCRETE_LOGIC_INVERT(NODE node, NODE INP0) {return discrete_global.DISCRETE_LOGIC_INVERT(node, INP0); }
        protected static discrete_block DISCRETE_BITS_DECODE(NODE node, NODE INP, int BIT_FROM, int BIT_TO, double VOUT) { return discrete_global.DISCRETE_BITS_DECODE(node, INP, BIT_FROM, BIT_TO, VOUT); }
        protected static discrete_block DISCRETE_LOGIC_DFLIPFLOP(NODE node, int RESET, int SET, NODE CLK, NODE INP) { return discrete_global.DISCRETE_LOGIC_DFLIPFLOP(node, RESET, SET, CLK, INP); }
        protected static discrete_block DISCRETE_MULTIPLY(NODE node, double INP0, NODE INP1) { return discrete_global.DISCRETE_MULTIPLY(node, INP0, INP1); }
        protected static discrete_block DISCRETE_MULTADD(NODE node, NODE INP0, double INP1, double INP2) { return discrete_global.DISCRETE_MULTADD(node, INP0, INP1, INP2); }
        protected static discrete_block DISCRETE_TRANSFORM5(NODE node, NODE INP0, double INP1, double INP2, NODE INP3, double INP4, string FUNCT) {return discrete_global.DISCRETE_TRANSFORM5(node, INP0, INP1, INP2, INP3, INP4, FUNCT); }
        protected static discrete_block DISCRETE_DAC_R1(NODE node, NODE DATA, double VDATA, discrete_dac_r1_ladder LADDER) { return discrete_global.DISCRETE_DAC_R1(node, DATA, VDATA, LADDER); }
        protected static discrete_block DISCRETE_MIXER3(NODE node, int ENAB, NODE IN0, NODE IN1, NODE IN2, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER3(node, ENAB, IN0, IN1, IN2, INFO); }
        protected static discrete_block DISCRETE_MIXER5(NODE node, int ENAB, NODE IN0, NODE IN1, NODE IN2, NODE IN3, NODE IN4, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER5(node, ENAB, IN0, IN1, IN2, IN3, IN4, INFO); }
        protected static discrete_block DISCRETE_MIXER6(NODE node, int ENAB, NODE IN0, NODE IN1, NODE IN2, NODE IN3, NODE IN4, NODE IN5, discrete_mixer_desc INFO) { return discrete_global.DISCRETE_MIXER6(node, ENAB, IN0, IN1, IN2, IN3, IN4, IN5, INFO); }
        protected static discrete_block DISCRETE_CRFILTER(NODE node, NODE INP0, double RVAL, double CVAL) { return discrete_global.DISCRETE_CRFILTER(node, INP0, RVAL, CVAL); }
        protected static discrete_block DISCRETE_OP_AMP_FILTER(NODE node, int ENAB, NODE INP0, int INP1, int TYPE, discrete_op_amp_filt_info INFO) { return discrete_global.DISCRETE_OP_AMP_FILTER(node, ENAB, INP0, INP1, TYPE, INFO); }
        protected static discrete_block DISCRETE_RCDISC5(NODE node, NODE ENAB, NODE INP0, double RVAL, double CVAL) { return discrete_global.DISCRETE_RCDISC5(node, ENAB, INP0, RVAL, CVAL); }
        protected static discrete_block DISCRETE_RCFILTER(NODE node, NODE INP0, double RVAL, double CVAL) { return discrete_global.DISCRETE_RCFILTER(node, INP0, RVAL, CVAL); }
        protected static discrete_block DISCRETE_RCFILTER_SW(NODE node, int ENAB, NODE INP0, NODE SW, int RVAL, double CVAL1, double CVAL2, int CVAL3, int CVAL4) { return discrete_global.DISCRETE_RCFILTER_SW(node, ENAB, INP0, SW, RVAL, CVAL1, CVAL2, CVAL3, CVAL4); }
        protected static discrete_block DISCRETE_555_ASTABLE_CV(NODE node, int RESET, double R1, double R2, double C, NODE CTRLV, discrete_555_desc OPTIONS) { return discrete_global.DISCRETE_555_ASTABLE_CV(node, RESET, R1, R2, C, CTRLV, OPTIONS); }
        protected static discrete_block DISCRETE_555_CC(NODE node, int RESET, NODE VIN, double R, double C, int RBIAS, int RGND, int RDIS, discrete_555_cc_desc OPTIONS) { return discrete_global.DISCRETE_555_CC(node, RESET, VIN, R, C, RBIAS, RGND, RDIS, OPTIONS); }
        protected static discrete_block DISCRETE_TASK_START(double TASK_GROUP) { return discrete_global.DISCRETE_TASK_START(TASK_GROUP); }
        protected static discrete_block DISCRETE_TASK_END() { return discrete_global.DISCRETE_TASK_END(); }
        protected static discrete_block DISCRETE_OUTPUT(NODE OPNODE, double GAIN) { return discrete_global.DISCRETE_OUTPUT(OPNODE, GAIN); }
        // disound
        protected const int ALL_OUTPUTS = disound_global.ALL_OUTPUTS;
        protected void MCFG_SOUND_ROUTE(u32 output, string target, double gain) { disound_global.MCFG_SOUND_ROUTE(m_helper_device, output, target, gain); }
        protected void MCFG_SOUND_ROUTE(u32 output, string target, double gain, u32 input) { disound_global.MCFG_SOUND_ROUTE(m_helper_device, output, target, gain, input); }
        // drawgfx
        protected static gfxdecode_device GFXDECODE(machine_config mconfig, string tag, string palette_tag, gfx_decode_entry [] gfxinfo)
        {
            var device = emu.detail.device_type_impl.op<gfxdecode_device>(mconfig, tag, gfxdecode_device.GFXDECODE, 0);
            device.gfxdecode_device_after_ctor(palette_tag, gfxinfo);
            return device;
        }
        protected static gfxdecode_device GFXDECODE(machine_config mconfig, device_finder<gfxdecode_device> finder, string palette_tag, gfx_decode_entry [] gfxinfo)
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, gfxdecode_device.GFXDECODE, 0);
            device.gfxdecode_device_after_ctor(palette_tag, gfxinfo);
            return device;
        }
        // driver
        protected void MCFG_MACHINE_START_OVERRIDE(driver_callback_delegate func) { driver_global.MCFG_MACHINE_START_OVERRIDE(m_helper_config, func); }
        protected void MCFG_MACHINE_RESET_OVERRIDE(driver_callback_delegate func) { driver_global.MCFG_MACHINE_RESET_OVERRIDE(m_helper_config, func); }
        protected void MCFG_VIDEO_START_OVERRIDE(driver_callback_delegate func) { driver_global.MCFG_VIDEO_START_OVERRIDE(m_helper_config, func); }
        // emucore
        protected const UInt32 ROT0   = emucore_global.ROT0;
        protected const UInt32 ROT90  = emucore_global.ROT90;
        protected const UInt32 ROT270 = emucore_global.ROT270;
        // emupal
        protected static palette_device PALETTE(machine_config mconfig, string tag, u32 entries)
        {
            var device = emu.detail.device_type_impl.op<palette_device>(mconfig, tag, palette_device.PALETTE, 0);
            device.palette_device_after_ctor(entries);
            return device;
        }
        protected static palette_device PALETTE(machine_config mconfig, device_finder<palette_device> finder, u32 entries)
        {
            var device = emu.detail.device_type_impl.op(mconfig, finder, palette_device.PALETTE, 0);
            device.palette_device_after_ctor(entries);
            return device;
        }
        protected void MCFG_PALETTE_ADD(string tag, u32 entries) { emupal_global.MCFG_PALETTE_ADD(out m_helper_device, m_helper_config, m_helper_owner, tag, entries); }
        protected void MCFG_PALETTE_ADD(device_finder<palette_device> finder, u32 entries) { emupal_global.MCFG_PALETTE_ADD(out m_helper_device, m_helper_config, m_helper_owner, finder, entries); }
        protected void MCFG_PALETTE_INIT_OWNER(palette_init_delegate method) { emupal_global.MCFG_PALETTE_INIT_OWNER(m_helper_device, method); }
        protected void MCFG_PALETTE_INDIRECT_ENTRIES(UInt32 entries) { emupal_global.MCFG_PALETTE_INDIRECT_ENTRIES(m_helper_device, entries); }
        // gamedrv
        protected const UInt64 MACHINE_SUPPORTS_SAVE = gamedrv_global.MACHINE_SUPPORTS_SAVE;
        protected const UInt64 MACHINE_NO_SOUND_HW = gamedrv_global.MACHINE_NO_SOUND_HW;
        protected const UInt64 MACHINE_IMPERFECT_GRAPHICS = gamedrv_global.MACHINE_IMPERFECT_GRAPHICS;
        protected static game_driver GAME(device_type.create_func creator, List<tiny_rom_entry> roms, string YEAR, string NAME, string PARENT, machine_creator_wrapper MACHINE, ioport_constructor INPUT, driver_init_wrapper INIT, UInt32 MONITOR, string COMPANY, string FULLNAME, UInt64 FLAGS) { return gamedrv_global.GAME(creator, roms, YEAR, NAME, PARENT, MACHINE, INPUT, INIT, MONITOR, COMPANY, FULLNAME, FLAGS); }
        // gen_latch
        protected static generic_latch_8_device GENERIC_LATCH_8(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<generic_latch_8_device>(mconfig, tag, generic_latch_8_device.GENERIC_LATCH_8, clock); }
        protected static generic_latch_8_device GENERIC_LATCH_8(machine_config mconfig, device_finder<generic_latch_8_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, generic_latch_8_device.GENERIC_LATCH_8, clock); }
        // i8255
        protected static i8255_device I8255A(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<i8255_device>(mconfig, tag, i8255_device.I8255A, clock); }
        protected static i8255_device I8255A(machine_config mconfig, device_finder<i8255_device> finder, u32 clock = 0) { return emu.detail.device_type_impl.op(mconfig, finder, i8255_device.I8255A, clock); }
        // mb88xx
        protected void MCFG_MB88XX_READ_K_CB(read8_delegate read8_devcb) { mb88xx_global.MCFG_MB88XX_READ_K_CB(m_helper_device, read8_devcb); }
        protected void MCFG_MB88XX_WRITE_O_CB(write8_delegate write8_devcb) { mb88xx_global.MCFG_MB88XX_WRITE_O_CB(m_helper_device, write8_devcb); }
        protected void MCFG_MB88XX_WRITE_P_CB(write8_delegate write8_devcb) { mb88xx_global.MCFG_MB88XX_WRITE_P_CB(m_helper_device, write8_devcb); }
        protected void MCFG_MB88XX_READ_R0_CB(read8_delegate read8_devcb) { mb88xx_global.MCFG_MB88XX_READ_R0_CB(m_helper_device, read8_devcb); }
        protected void MCFG_MB88XX_READ_R1_CB(read8_delegate read8_devcb) { mb88xx_global.MCFG_MB88XX_READ_R1_CB(m_helper_device, read8_devcb); }
        protected void MCFG_MB88XX_WRITE_R1_CB(write8_delegate write8_devcb) { mb88xx_global.MCFG_MB88XX_WRITE_R1_CB(m_helper_device, write8_devcb); }
        protected void MCFG_MB88XX_READ_R2_CB(read8_delegate read8_devcb) { mb88xx_global.MCFG_MB88XX_READ_R2_CB(m_helper_device, read8_devcb); }
        protected void MCFG_MB88XX_READ_R3_CB(read8_delegate read8_devcb) { mb88xx_global.MCFG_MB88XX_READ_R3_CB(m_helper_device, read8_devcb); }
        // mconfig
        protected void MCFG_QUANTUM_TIME(attotime time) { mconfig_global.MCFG_QUANTUM_TIME(m_helper_config, time); }
        protected void MCFG_DEVICE_ADD(string tag, device_type type, u32 clock = 0) { mconfig_global.MCFG_DEVICE_ADD(out m_helper_device, m_helper_config, m_helper_owner, tag, type, clock); }
        protected void MCFG_DEVICE_ADD(string tag, device_type type, XTAL clock) { mconfig_global.MCFG_DEVICE_ADD(out m_helper_device, m_helper_config, m_helper_owner, tag, type, clock); }
        protected void MCFG_DEVICE_ADD_discrete_sound_device(discrete_block [] intf) { ((discrete_sound_device)helper_device).discrete_sound_device_after_ctor(intf); }
        protected void MCFG_DEVICE_ADD_gfxdecode_device(string tag, gfx_decode_entry [] gfxinfo) { ((gfxdecode_device)helper_device).gfxdecode_device_after_ctor(tag, gfxinfo); }
        protected speaker_device SPEAKER(machine_config mconfig, string tag) { mconfig_global.MCFG_DEVICE_ADD(out m_helper_device, mconfig, m_helper_owner, tag, speaker_device.SPEAKER, 0); return (speaker_device)m_helper_device; }  // alias for device_type_impl<DeviceClass>::operator()
        protected void MCFG_DEVICE_MODIFY(string tag) { mconfig_global.MCFG_DEVICE_MODIFY(out m_helper_device, m_helper_config, m_helper_owner, tag); }
        // namco
        protected static namco_device NAMCO(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<namco_device>(mconfig, tag, namco_device.NAMCO, clock); }
        protected static namco_device NAMCO(machine_config mconfig, device_finder<namco_device> finder, u32 clock) { return emu.detail.device_type_impl.op(mconfig, finder, namco_device.NAMCO, clock); }
        protected static namco_device NAMCO(machine_config mconfig, device_finder<namco_device> finder, XTAL clock) { return emu.detail.device_type_impl.op(mconfig, finder, namco_device.NAMCO, clock); }
        protected static namco_50xx_device NAMCO_50XX(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<namco_50xx_device>(mconfig, tag, namco_50xx_device.NAMCO_50XX, clock); }
        protected static namco_50xx_device NAMCO_50XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_50xx_device>(mconfig, tag, namco_50xx_device.NAMCO_50XX, clock); }
        protected static namco_53xx_device NAMCO_53XX(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<namco_53xx_device>(mconfig, tag, namco_53xx_device.NAMCO_53XX, clock); }
        protected static namco_53xx_device NAMCO_53XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_53xx_device>(mconfig, tag, namco_53xx_device.NAMCO_53XX, clock); }
        protected static namco_54xx_device NAMCO_54XX(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<namco_54xx_device>(mconfig, tag, namco_54xx_device.NAMCO_54XX, clock); }
        protected static namco_54xx_device NAMCO_54XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_54xx_device>(mconfig, tag, namco_54xx_device.NAMCO_54XX, clock); }
        protected void MCFG_NAMCO_AUDIO_VOICES(int voices) { namco_global.MCFG_NAMCO_AUDIO_VOICES(m_helper_device, voices); }
        protected void MCFG_NAMCO_06XX_ADD(string tag, XTAL clock) { namco06_global.MCFG_NAMCO_06XX_ADD(out m_helper_device, m_helper_config, m_helper_owner, tag, clock); }
        protected void MCFG_NAMCO_06XX_MAINCPU(string tag) { namco06_global.MCFG_NAMCO_06XX_MAINCPU(m_helper_device, tag); }
        protected void MCFG_NAMCO_06XX_READ_0_CB(read8_delegate read8_devcb) { namco06_global.MCFG_NAMCO_06XX_READ_0_CB(m_helper_device, read8_devcb); }
        protected void MCFG_NAMCO_06XX_READ_1_CB(read8_delegate read8_devcb) { namco06_global.MCFG_NAMCO_06XX_READ_1_CB(m_helper_device, read8_devcb); }
        protected void MCFG_NAMCO_06XX_READ_2_CB(read8_delegate read8_devcb) { namco06_global.MCFG_NAMCO_06XX_READ_2_CB(m_helper_device, read8_devcb); }
        protected void MCFG_NAMCO_06XX_READ_3_CB(read8_delegate read8_devcb) { namco06_global.MCFG_NAMCO_06XX_READ_3_CB(m_helper_device, read8_devcb); }
        protected void MCFG_NAMCO_06XX_READ_REQUEST_0_CB(write_line_delegate write_line_devcb) { namco06_global.MCFG_NAMCO_06XX_READ_REQUEST_0_CB(m_helper_device, write_line_devcb); }
        protected void MCFG_NAMCO_06XX_READ_REQUEST_1_CB(write_line_delegate write_line_devcb) { namco06_global.MCFG_NAMCO_06XX_READ_REQUEST_1_CB(m_helper_device, write_line_devcb); }
        protected void MCFG_NAMCO_06XX_READ_REQUEST_2_CB(write_line_delegate write_line_devcb) { namco06_global.MCFG_NAMCO_06XX_READ_REQUEST_2_CB(m_helper_device, write_line_devcb); }
        protected void MCFG_NAMCO_06XX_READ_REQUEST_3_CB(write_line_delegate write_line_devcb) { namco06_global.MCFG_NAMCO_06XX_READ_REQUEST_3_CB(m_helper_device, write_line_devcb); }
        protected void MCFG_NAMCO_06XX_WRITE_0_CB(write8_delegate write8_devcb) { namco06_global.MCFG_NAMCO_06XX_WRITE_0_CB(m_helper_device, write8_devcb); }
        protected void MCFG_NAMCO_06XX_WRITE_1_CB(write8_delegate write8_devcb) { namco06_global.MCFG_NAMCO_06XX_WRITE_1_CB(m_helper_device, write8_devcb); }
        protected void MCFG_NAMCO_06XX_WRITE_2_CB(write8_delegate write8_devcb) { namco06_global.MCFG_NAMCO_06XX_WRITE_2_CB(m_helper_device, write8_devcb); }
        protected void MCFG_NAMCO_06XX_WRITE_3_CB(write8_delegate write8_devcb) { namco06_global.MCFG_NAMCO_06XX_WRITE_3_CB(m_helper_device, write8_devcb); }
        protected void MCFG_NAMCO_51XX_ADD(string tag, XTAL clock) { namco51_global.MCFG_NAMCO_51XX_ADD(out m_helper_device, m_helper_config, m_helper_owner, tag, clock); }
        protected void MCFG_NAMCO_51XX_SCREEN(string screen_tag) { namco51_global.MCFG_NAMCO_51XX_SCREEN(m_helper_device, screen_tag); }
        protected void MCFG_NAMCO_51XX_INPUT_0_CB(DEVCB_IOPORT ioport_desc_devcb) { namco51_global.MCFG_NAMCO_51XX_INPUT_0_CB(m_helper_device, ioport_desc_devcb); }
        protected void MCFG_NAMCO_51XX_INPUT_1_CB(DEVCB_IOPORT ioport_desc_devcb) { namco51_global.MCFG_NAMCO_51XX_INPUT_1_CB(m_helper_device, ioport_desc_devcb); }
        protected void MCFG_NAMCO_51XX_INPUT_2_CB(DEVCB_IOPORT ioport_desc_devcb) { namco51_global.MCFG_NAMCO_51XX_INPUT_2_CB(m_helper_device, ioport_desc_devcb); }
        protected void MCFG_NAMCO_51XX_INPUT_3_CB(DEVCB_IOPORT ioport_desc_devcb) { namco51_global.MCFG_NAMCO_51XX_INPUT_3_CB(m_helper_device, ioport_desc_devcb); }
        protected void MCFG_NAMCO_51XX_OUTPUT_0_CB(write8_delegate write8_devcb) { namco51_global.MCFG_NAMCO_51XX_OUTPUT_0_CB(m_helper_device, write8_devcb); }
        protected void MCFG_NAMCO_51XX_OUTPUT_1_CB(write8_delegate write8_devcb) { namco51_global.MCFG_NAMCO_51XX_OUTPUT_1_CB(m_helper_device, write8_devcb); }
        // net_lib
        protected void SOLVER(string name, int freq) { netlist.devices.net_lib_global.SOLVER(m_helper_setup, name, freq); }
        // netlist
        protected void MCFG_NETLIST_SETUP(setup_func setup) { netlist_global.MCFG_NETLIST_SETUP(m_helper_device, setup); }
        protected void MCFG_NETLIST_ANALOG_MULT_OFFSET(double mult, double offset) { netlist_global.MCFG_NETLIST_ANALOG_MULT_OFFSET(m_helper_device, mult, offset); }
        protected void MCFG_NETLIST_STREAM_INPUT(string basetag, int chan, string name) { netlist_global.MCFG_NETLIST_STREAM_INPUT(out m_helper_device, m_helper_config, m_helper_owner, basetag, chan, name); }
        protected void MCFG_NETLIST_STREAM_OUTPUT(string basetag, int chan, string name) { netlist_global.MCFG_NETLIST_STREAM_OUTPUT(out m_helper_device, m_helper_config, m_helper_owner, basetag, chan, name); }
        // nl_setup
        protected void NET_C(params string [] term1) { netlist.nl_setup_global.NET_C(m_helper_setup, term1); }
        protected void PARAM(string name, int val) { netlist.nl_setup_global.PARAM(m_helper_setup, name, val); }
        protected void PARAM(string name, double val) { netlist.nl_setup_global.PARAM(m_helper_setup, name, val); }
        protected void NETLIST_START(netlist.setup_t setup) { m_helper_setup = setup;  netlist.nl_setup_global.NETLIST_START(); }
        protected void NETLIST_END() { m_helper_setup = null;  netlist.nl_setup_global.NETLIST_END(); }
        // nld_system
        protected void ANALOG_INPUT(string name, int v) { netlist.devices.nld_system_global.ANALOG_INPUT(m_helper_setup, name, v); }
        // nld_twoterm
        protected void RES(string name, int p_R) { netlist.nld_twoterm_global.RES(m_helper_setup, name, p_R); }
        protected void POT(string name, int p_R) { netlist.nld_twoterm_global.POT(m_helper_setup, name, p_R); }
        protected void CAP(string name, double p_C) { netlist.nld_twoterm_global.CAP(m_helper_setup, name, p_C); }
        // pokey
        protected void MCFG_POKEY_OUTPUT_OPAMP_LOW_PASS(double _R, double _C, double _V) { pokey_global.MCFG_POKEY_OUTPUT_OPAMP_LOW_PASS(m_helper_device, _R, _C, _V); }
        // rescap
        protected static double RES_K(double res) { return rescap_global.RES_K(res); }
        protected static double CAP_U(double cap) { return rescap_global.CAP_U(cap); }
        // screen
        protected const screen_type_enum SCREEN_TYPE_RASTER = screen_type_enum.SCREEN_TYPE_RASTER;
        protected const screen_type_enum SCREEN_TYPE_VECTOR = screen_type_enum.SCREEN_TYPE_VECTOR;
        protected const screen_type_enum SCREEN_TYPE_LCD = screen_type_enum.SCREEN_TYPE_LCD;
        protected const screen_type_enum SCREEN_TYPE_SVG = screen_type_enum.SCREEN_TYPE_SVG;
        protected const screen_type_enum SCREEN_TYPE_INVALID = screen_type_enum.SCREEN_TYPE_INVALID;
        protected screen_device SCREEN(machine_config mconfig, string tag, screen_type_enum type)
        {
            var screen = emu.detail.device_type_impl.op<screen_device>(mconfig, tag, screen_device.SCREEN, 0);
            screen.screen_device_after_ctor(type);
            return screen;
        }
        protected screen_device SCREEN(machine_config mconfig, device_finder<screen_device> finder, screen_type_enum type)
        {
            var screen = emu.detail.device_type_impl.op(mconfig, finder, screen_device.SCREEN, 0);
            screen.screen_device_after_ctor(type);
            return screen;
        }
        protected void MCFG_SCREEN_ADD(string tag, screen_type_enum type) { screen_global.MCFG_SCREEN_ADD(out m_helper_device, m_helper_config, m_helper_owner, tag, type); }
        protected void MCFG_SCREEN_ADD(device_finder<screen_device> finder, screen_type_enum type) { screen_global.MCFG_SCREEN_ADD(out m_helper_device, m_helper_config, m_helper_owner, finder, type); }
        protected void MCFG_SCREEN_RAW_PARAMS(XTAL pixclock, UInt16 htotal, UInt16 hbend, UInt16 hbstart, UInt16 vtotal, UInt16 vbend, UInt16 vbstart) { screen_global.MCFG_SCREEN_RAW_PARAMS(m_helper_device, pixclock, htotal, hbend, hbstart, vtotal, vbend, vbstart); }
        protected void MCFG_SCREEN_REFRESH_RATE(UInt32 rate) { screen_global.MCFG_SCREEN_REFRESH_RATE(m_helper_device, rate); }
        protected void MCFG_SCREEN_VBLANK_TIME(attoseconds_t time) { screen_global.MCFG_SCREEN_VBLANK_TIME(m_helper_device, time); }
        protected void MCFG_SCREEN_SIZE(UInt16 width, UInt16 height) { screen_global.MCFG_SCREEN_SIZE(m_helper_device, width, height); }
        protected void MCFG_SCREEN_VISIBLE_AREA(Int16 minx, Int16 maxx, Int16 miny, Int16 maxy) { screen_global.MCFG_SCREEN_VISIBLE_AREA(m_helper_device, minx, maxx, miny, maxy); }
        protected void MCFG_SCREEN_UPDATE_DRIVER(screen_update_ind16_delegate method) { screen_global.MCFG_SCREEN_UPDATE_DRIVER(m_helper_device, method); }
        protected void MCFG_SCREEN_UPDATE_DRIVER(screen_update_rgb32_delegate method) { screen_global.MCFG_SCREEN_UPDATE_DRIVER(m_helper_device, method); }
        protected void MCFG_SCREEN_VBLANK_CALLBACK(write_line_delegate method) { screen_global.MCFG_SCREEN_VBLANK_CALLBACK(m_helper_device, method); }
        protected void MCFG_SCREEN_PALETTE(string palette_tag) { screen_global.MCFG_SCREEN_PALETTE(m_helper_device, palette_tag); }
        // timer
        protected void MCFG_TIMER_DRIVER_ADD_SCANLINE(string tag, timer_device.expired_delegate callback, string screen, int first_vpos, int increment) { timer_global.MCFG_TIMER_DRIVER_ADD_SCANLINE(out m_helper_device, m_helper_config, m_helper_owner, tag, callback, screen, first_vpos, increment); }
        // watchdog
        protected static watchdog_timer_device WATCHDOG_TIMER(machine_config mconfig, string tag) { return emu.detail.device_type_impl.op<watchdog_timer_device>(mconfig, tag, watchdog_timer_device.WATCHDOG_TIMER, 0); }
        protected static watchdog_timer_device WATCHDOG_TIMER(machine_config mconfig, device_finder<watchdog_timer_device> finder) { return emu.detail.device_type_impl.op<watchdog_timer_device>(mconfig, finder, watchdog_timer_device.WATCHDOG_TIMER, 0); }
        // z80
        protected static cpu_device Z80(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<cpu_device>(mconfig, tag, z80_device.Z80, clock); }
        protected static cpu_device Z80(machine_config mconfig, device_finder<cpu_device> finder, u32 clock) { return emu.detail.device_type_impl.op<cpu_device>(mconfig, finder, z80_device.Z80, clock); }
        protected static cpu_device Z80(machine_config mconfig, device_finder<cpu_device> finder, XTAL clock) { return emu.detail.device_type_impl.op<cpu_device>(mconfig, finder, z80_device.Z80, clock); }


        // rom helpers
        protected static tiny_rom_entry ROM_END() { return romentry_global.ROM_END(); }
        protected static tiny_rom_entry ROM_REGION(UInt32 length, string tag, UInt32 flags) { return romentry_global.ROM_REGION(length, tag, flags); }
        protected static tiny_rom_entry ROM_LOAD(string name, UInt32 offset, UInt32 length, string hash) { return romentry_global.ROM_LOAD(name, offset, length, hash); }
        protected static tiny_rom_entry ROM_FILL(UInt32 offset, UInt32 length, byte value) { return romentry_global.ROM_FILL(offset, length, value); }

        protected static string CRC(string x) { return util.hash_global.CRC(x); }
        protected static string SHA1(string x) { return util.hash_global.SHA1(x); }


        // romentry helpers
        protected static readonly UInt32 ROMREGION_ERASEFF = romentry_global.ROMREGION_ERASEFF;


        // input helpers
        protected static readonly input_code KEYCODE_F1 = input_global.KEYCODE_F1;


        // ioport helpers
        protected const ioport_value IP_ACTIVE_HIGH = ioport_global.IP_ACTIVE_HIGH;
        protected const ioport_value IP_ACTIVE_LOW = ioport_global.IP_ACTIVE_LOW;

        protected const ioport_type IPT_UNUSED = ioport_type.IPT_UNUSED;
        protected const ioport_type IPT_UNKNOWN = ioport_type.IPT_UNKNOWN;
        protected const ioport_type IPT_START1 = ioport_type.IPT_START1;
        protected const ioport_type IPT_START2 = ioport_type.IPT_START2;
        protected const ioport_type IPT_COIN1 = ioport_type.IPT_COIN1;
        protected const ioport_type IPT_COIN2 = ioport_type.IPT_COIN2;
        protected const ioport_type IPT_SERVICE1 = ioport_type.IPT_SERVICE1;
        protected const ioport_type IPT_TILT = ioport_type.IPT_TILT;
        protected const ioport_type IPT_JOYSTICK_UP = ioport_type.IPT_JOYSTICK_UP;
        protected const ioport_type IPT_JOYSTICK_DOWN = ioport_type.IPT_JOYSTICK_DOWN;
        protected const ioport_type IPT_JOYSTICK_LEFT = ioport_type.IPT_JOYSTICK_LEFT;
        protected const ioport_type IPT_JOYSTICK_RIGHT = ioport_type.IPT_JOYSTICK_RIGHT;
        protected const ioport_type IPT_BUTTON1 = ioport_type.IPT_BUTTON1;
        protected const ioport_type IPT_BUTTON2 = ioport_type.IPT_BUTTON2;
        protected const ioport_type IPT_TRACKBALL_X = ioport_type.IPT_TRACKBALL_X;
        protected const ioport_type IPT_TRACKBALL_Y = ioport_type.IPT_TRACKBALL_Y;
        protected const ioport_type IPT_SPECIAL = ioport_type.IPT_SPECIAL;
        protected const ioport_type IPT_CUSTOM = ioport_type.IPT_CUSTOM;

        protected const INPUT_STRING Off = INPUT_STRING.INPUT_STRING_Off;
        protected const INPUT_STRING On = INPUT_STRING.INPUT_STRING_On;
        protected const INPUT_STRING No = INPUT_STRING.INPUT_STRING_No;
        protected const INPUT_STRING Yes = INPUT_STRING.INPUT_STRING_Yes;
        protected const INPUT_STRING Lives = INPUT_STRING.INPUT_STRING_Lives;
        protected const INPUT_STRING Bonus_Life = INPUT_STRING.INPUT_STRING_Bonus_Life;
        protected const INPUT_STRING Difficulty = INPUT_STRING.INPUT_STRING_Difficulty;
        protected const INPUT_STRING Demo_Sounds = INPUT_STRING.INPUT_STRING_Demo_Sounds;
        protected const INPUT_STRING Coinage = INPUT_STRING.INPUT_STRING_Coinage;
        protected const INPUT_STRING Coin_A = INPUT_STRING.INPUT_STRING_Coin_A;
        protected const INPUT_STRING Coin_B = INPUT_STRING.INPUT_STRING_Coin_B;
        protected const INPUT_STRING _4C_1C = INPUT_STRING.INPUT_STRING_4C_1C;
        protected const INPUT_STRING _3C_1C = INPUT_STRING.INPUT_STRING_3C_1C;
        protected const INPUT_STRING _2C_1C = INPUT_STRING.INPUT_STRING_2C_1C;
        protected const INPUT_STRING _1C_1C = INPUT_STRING.INPUT_STRING_1C_1C;
        protected const INPUT_STRING _2C_3C = INPUT_STRING.INPUT_STRING_2C_3C;
        protected const INPUT_STRING _1C_2C = INPUT_STRING.INPUT_STRING_1C_2C;
        protected const INPUT_STRING _1C_3C = INPUT_STRING.INPUT_STRING_1C_3C;
        protected const INPUT_STRING _1C_4C = INPUT_STRING.INPUT_STRING_1C_4C;
        protected const INPUT_STRING _1C_6C = INPUT_STRING.INPUT_STRING_1C_6C;
        protected const INPUT_STRING _1C_7C = INPUT_STRING.INPUT_STRING_1C_7C;
        protected const INPUT_STRING Free_Play = INPUT_STRING.INPUT_STRING_Free_Play;
        protected const INPUT_STRING Cabinet = INPUT_STRING.INPUT_STRING_Cabinet;
        protected const INPUT_STRING Upright = INPUT_STRING.INPUT_STRING_Upright;
        protected const INPUT_STRING Cocktail = INPUT_STRING.INPUT_STRING_Cocktail;
        protected const INPUT_STRING Flip_Screen = INPUT_STRING.INPUT_STRING_Flip_Screen;
        protected const INPUT_STRING Language = INPUT_STRING.INPUT_STRING_Language;
        protected const INPUT_STRING English = INPUT_STRING.INPUT_STRING_English;
        protected const INPUT_STRING Japanese = INPUT_STRING.INPUT_STRING_Japanese;
        protected const INPUT_STRING Chinese = INPUT_STRING.INPUT_STRING_Chinese;
        protected const INPUT_STRING French = INPUT_STRING.INPUT_STRING_French;
        protected const INPUT_STRING German = INPUT_STRING.INPUT_STRING_German;
        protected const INPUT_STRING Italian = INPUT_STRING.INPUT_STRING_Italian;
        protected const INPUT_STRING Korean = INPUT_STRING.INPUT_STRING_Korean;
        protected const INPUT_STRING Spanish = INPUT_STRING.INPUT_STRING_Spanish;
        protected const INPUT_STRING Easy = INPUT_STRING.INPUT_STRING_Easy;
        protected const INPUT_STRING Normal = INPUT_STRING.INPUT_STRING_Normal;
        protected const INPUT_STRING Medium = INPUT_STRING.INPUT_STRING_Medium;
        protected const INPUT_STRING Hard = INPUT_STRING.INPUT_STRING_Hard;
        protected const INPUT_STRING Hardest = INPUT_STRING.INPUT_STRING_Hardest;
        protected const INPUT_STRING Difficult = INPUT_STRING.INPUT_STRING_Difficult;
        protected const INPUT_STRING Very_Difficult = INPUT_STRING.INPUT_STRING_Very_Difficult;
        protected const INPUT_STRING Allow_Continue = INPUT_STRING.INPUT_STRING_Allow_Continue;
        protected const INPUT_STRING Alternate = INPUT_STRING.INPUT_STRING_Alternate;
        protected const INPUT_STRING None = INPUT_STRING.INPUT_STRING_None;

        protected void INPUT_PORTS_START(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            ioport_configurer configurer = new ioport_configurer(owner, portlist, ref errorbuf);
            helper_ioport_configurer = configurer;
            helper_owner = owner;
        }
        protected void INPUT_PORTS_END() { helper_ioport_configurer = null; helper_owner = null; }
        protected void PORT_START( string tag) { ioport_global.PORT_START(m_helper_configurer, tag); }
        protected void PORT_BIT(ioport_value mask, ioport_value defval, ioport_type type) { ioport_global.PORT_BIT(m_helper_configurer, mask, defval, type); }
        protected void PORT_CODE(input_code code) { ioport_global.PORT_CODE(m_helper_configurer, code); }
        protected void PORT_2WAY() { ioport_global.PORT_2WAY(m_helper_configurer); }
        protected void PORT_4WAY() { ioport_global.PORT_4WAY(m_helper_configurer); }
        protected void PORT_8WAY() { ioport_global.PORT_8WAY(m_helper_configurer); }
        protected void PORT_PLAYER(int player) { ioport_global.PORT_PLAYER(m_helper_configurer, player); }
        protected void PORT_COCKTAIL() { ioport_global.PORT_COCKTAIL(m_helper_configurer); }
        protected void PORT_REVERSE() { ioport_global.PORT_REVERSE(m_helper_configurer); }
        protected void PORT_SENSITIVITY(int sensitivity) { ioport_global.PORT_SENSITIVITY(m_helper_configurer, sensitivity); }
        protected void PORT_KEYDELTA(int delta) { ioport_global.PORT_KEYDELTA(m_helper_configurer, delta); }
        protected void PORT_CUSTOM_MEMBER(string device, ioport_field_read_delegate callback, Object param) { ioport_global.PORT_CUSTOM_MEMBER(m_helper_configurer, device, callback, param); }
        protected void PORT_DIPNAME(ioport_value mask, ioport_value defval, string name) { ioport_global.PORT_DIPNAME(m_helper_configurer, mask, defval, name); }
        protected void PORT_DIPSETTING(ioport_value defval, string name) { ioport_global.PORT_DIPSETTING(m_helper_configurer, defval, name); }
        protected void PORT_DIPLOCATION(string location) { ioport_global.PORT_DIPLOCATION(m_helper_configurer, location); }
        protected void PORT_CONDITION(string tag, ioport_value mask, ioport_condition.condition_t condition, ioport_value value) { ioport_global.PORT_CONDITION(m_helper_configurer, tag, mask, condition, value); }
        protected void PORT_DIPUNUSED_DIPLOC(ioport_value mask, ioport_value defval, string diploc) { ioport_global.PORT_DIPUNUSED_DIPLOC(m_helper_configurer, mask, defval, diploc); }
        protected void PORT_DIPUNUSED(ioport_value mask, ioport_value defval) { ioport_global.PORT_DIPUNUSED(m_helper_configurer, mask, defval); }
        protected void PORT_SERVICE_DIPLOC(ioport_value mask, ioport_value defval, string diploc) { ioport_global.PORT_SERVICE_DIPLOC(m_helper_configurer, mask, defval, diploc); }
        protected void PORT_SERVICE(ioport_value mask, ioport_value defval) { ioport_global.PORT_SERVICE(m_helper_configurer, mask, defval); }
        protected void PORT_VBLANK(string screen) { ioport_global.PORT_VBLANK(m_helper_configurer, screen, (screen_device)m_helper_owner.subdevice(screen)); }
        protected static string DEF_STR(INPUT_STRING str_num) { return ioport_global.DEF_STR(str_num); }


        // gfxdecode helpers
        protected static gfx_decode_entry GFXDECODE_ENTRY(string region, UInt32 offset, gfx_layout layout, UInt16 start, UInt16 colors) { return digfx_global.GFXDECODE_ENTRY(region, offset, layout, start, colors); }
        protected static gfx_decode_entry GFXDECODE_SCALE(string region, UInt32 offset, gfx_layout layout, UInt16 start, UInt16 colors, UInt32 x, UInt32 y) { return digfx_global.GFXDECODE_SCALE(region, offset, layout, start, colors, x, y); }
    }
}
