// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_read_line = mame.devcb_read<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_read_line = devcb_read<int, 1U>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using u32 = System.UInt32;

using static mame.device_global;
using static mame.i8279_global;


namespace mame
{
    public class i8279_device : device_t
    {
        //DEFINE_DEVICE_TYPE(I8279, i8279_device, "i8279", "Intel 8279 KDC")
        public static readonly emu.detail.device_type_impl I8279 = DEFINE_DEVICE_TYPE("i8279", "Intel 8279 KDC", (type, mconfig, tag, owner, clock) => { return new i8279_device(mconfig, tag, owner, clock); });


        devcb_write_line m_out_irq_cb;       // IRQ
        devcb_write8 m_out_sl_cb;        // Scanlines SL0-3
        devcb_write8 m_out_disp_cb;      // Display outputs B0-3, A0-3
        devcb_write_line m_out_bd_cb;        // BD
        devcb_read8 m_in_rl_cb;         // kbd readlines RL0-7
        devcb_read_line m_in_shift_cb;      // Shift key
        devcb_read_line m_in_ctrl_cb;       // Ctrl-Strobe line

        //emu_timer *m_timer;

        //u8 m_d_ram[16];     // display ram
        //u8 m_d_ram_ptr;
        //u8 m_s_ram[8];      // might be same as fifo ram
        //u8 m_s_ram_ptr;
        //u8 m_fifo[8];       // queued keystrokes
        //u8 m_cmd[8];        // Device settings
        //u8 m_status;        // Returned via status_r
        //u32 m_scanclock;    // Internal scan clock
        //u8 m_scanner;       // next output on SL lines

        //bool m_autoinc;     // auto-increment flag
        //bool m_read_flag;   // read from where
        //bool m_ctrl_key;    // previous state of strobe input
        //bool m_se_mode;     // special error mode flag
        //u8 m_key_down;      // current key being debounced
        //u8 m_debounce;      // debounce counter


        // construction/destruction
        i8279_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, I8279, tag, owner, clock)
        {
            m_out_irq_cb = new devcb_write_line(this);
            m_out_sl_cb = new devcb_write8(this);
            m_out_disp_cb = new devcb_write8(this);
            m_out_bd_cb = new devcb_write_line(this);
            m_in_rl_cb = new devcb_read8(this);
            m_in_shift_cb = new devcb_read_line(this);
            m_in_ctrl_cb = new devcb_read_line(this);
        }


        //auto out_irq_callback() { return m_out_irq_cb.bind(); }
        public devcb_write8.binder out_sl_callback() { return m_out_sl_cb.bind(); }  //auto out_sl_callback() { return m_out_sl_cb.bind(); }
        public devcb_write8.binder out_disp_callback() { return m_out_disp_cb.bind(); }  //auto out_disp_callback() { return m_out_disp_cb.bind(); }
        //auto out_bd_callback() { return m_out_bd_cb.bind(); }
        public devcb_read8.binder in_rl_callback() { return m_in_rl_cb.bind(); }  //auto in_rl_callback() { return m_in_rl_cb.bind(); }
        //auto in_shift_callback() { return m_in_shift_cb.bind(); }
        //auto in_ctrl_callback() { return m_in_ctrl_cb.bind(); }

        // read & write handlers
        //u8 read(offs_t offset);
        //u8 status_r();
        //u8 data_r();
        //void write(offs_t offset, u8 data);
        //void cmd_w(u8 data);
        //void data_w(u8 data);
        //void timer_mainloop();


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_reset() { throw new emu_unimplemented(); }
        protected override void device_post_load() { }
        protected override void device_clock_changed() { }


        //TIMER_CALLBACK_MEMBER( timerproc_callback );


        //void timer_adjust();
        //void clear_display();
        //void new_fifo(u8 data);
        //void set_irq(bool state);
    }


    public static class i8279_global
    {
        public static i8279_device I8279(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<i8279_device>(mconfig, tag, i8279_device.I8279, clock); }
    }
}
