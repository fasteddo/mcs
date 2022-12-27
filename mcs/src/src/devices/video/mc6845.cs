// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int8_t = System.SByte;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.mc6845_global;


namespace mame
{
    /* callback definitions */
    //#define MC6845_RECONFIGURE(name)  void name(int width, int height, const rectangle &visarea, attoseconds_t frame_period)

    //#define MC6845_BEGIN_UPDATE(name)  void name(bitmap_rgb32 &bitmap, const rectangle &cliprect)

    //#define MC6845_UPDATE_ROW(name)     void name(bitmap_rgb32 &bitmap, const rectangle &cliprect, uint16_t ma, uint8_t ra, \
    //                                                uint16_t y, uint8_t x_count, int8_t cursor_x, int de, int hbp, int vbp)

    //#define MC6845_END_UPDATE(name)     void name(bitmap_rgb32 &bitmap, const rectangle &cliprect)

    //#define MC6845_ON_UPDATE_ADDR_CHANGED(name) void name(int address, int strobe)


    public class mc6845_device : device_t
                                 //device_video_interface
    {
        //DEFINE_DEVICE_TYPE(MC6845,   mc6845_device,   "mc6845",   "Motorola MC6845 CRTC")
        public static readonly emu.detail.device_type_impl MC6845 = DEFINE_DEVICE_TYPE("mc6845", "Motorola MC6845 CRTC", (type, mconfig, tag, owner, clock) => { return new mc6845_device(mconfig, tag, owner, clock); });


        delegate void reconfigure_delegate(int width, int height, in rectangle visarea, attoseconds_t frame_period);  //typedef device_delegate<void (int width, int height, const rectangle &visarea, attoseconds_t frame_period)> reconfigure_delegate;
        delegate void begin_update_delegate(bitmap_rgb32 bitmap, in rectangle cliprect);  //typedef device_delegate<void (bitmap_rgb32 &bitmap, const rectangle &cliprect)> begin_update_delegate;
        delegate void update_row_delegate(bitmap_rgb32 bitmap, in rectangle cliprect, uint16_t ma, uint8_t ra, uint16_t y, uint8_t x_count, int8_t cursor_x, int de, int hbp, int vbp);  //typedef device_delegate<void (bitmap_rgb32 &bitmap, const rectangle &cliprect, uint16_t ma, uint8_t ra, uint16_t y, uint8_t x_count, int8_t cursor_x, int de, int hbp, int vbp)> update_row_delegate;
        delegate void end_update_delegate(bitmap_rgb32 bitmap, in rectangle cliprect);  //typedef device_delegate<void (bitmap_rgb32 &bitmap, const rectangle &cliprect)> end_update_delegate;
        delegate void on_update_addr_changed_delegate(int address, int strobe);  //typedef device_delegate<void (int address, int strobe)> on_update_addr_changed_delegate;


        device_video_interface m_divideo;


        //bool m_supports_disp_start_addr_r;
        //bool m_supports_vert_sync_width;
        //bool m_supports_status_reg_d5;
        //bool m_supports_status_reg_d6;
        //bool m_supports_status_reg_d7;
        //bool m_supports_transparent;

        /* register file */
        //uint8_t   m_horiz_char_total;     /* 0x00 */
        //uint8_t   m_horiz_disp;           /* 0x01 */
        //uint8_t   m_horiz_sync_pos;       /* 0x02 */
        //uint8_t   m_sync_width;           /* 0x03 */
        //uint8_t   m_vert_char_total;      /* 0x04 */
        //uint8_t   m_vert_total_adj;       /* 0x05 */
        //uint8_t   m_vert_disp;            /* 0x06 */
        //uint8_t   m_vert_sync_pos;        /* 0x07 */
        //uint8_t   m_mode_control;         /* 0x08 */
        //uint8_t   m_max_ras_addr;         /* 0x09 */
        //uint8_t   m_cursor_start_ras;     /* 0x0a */
        //uint8_t   m_cursor_end_ras;       /* 0x0b */
        //uint16_t  m_disp_start_addr;      /* 0x0c/0x0d */
        //uint16_t  m_cursor_addr;          /* 0x0e/0x0f */
        //uint16_t  m_light_pen_addr;       /* 0x10/0x11 */
        //uint16_t  m_update_addr;          /* 0x12/0x13 */

        /* other internal state */
        //uint8_t   m_register_address_latch;
        //bool    m_cursor_state;
        //uint8_t   m_cursor_blink_count;
        //bool    m_update_ready_bit;
        /* output signals */
        //int     m_cur;
        //int     m_hsync;
        //int     m_vsync;
        //int     m_de;

        /* internal counters */
        //uint8_t   m_character_counter;        /* Not used yet */
        //uint8_t   m_hsync_width_counter;  /* Not used yet */
        //uint8_t   m_line_counter;
        //uint8_t   m_raster_counter;
        //uint8_t   m_adjust_counter;
        //uint8_t   m_vsync_width_counter;

        //bool    m_line_enable_ff;       /* Internal flip flop which is set when the line_counter is reset and reset when vert_disp is reached */
        //uint8_t   m_vsync_ff;
        //uint8_t   m_adjust_active;
        //uint16_t  m_line_address;
        //int16_t   m_cursor_x;

        /* timers */
        //static const device_timer_id TIMER_LINE = 0;
        //static const device_timer_id TIMER_DE_OFF = 1;
        //static const device_timer_id TIMER_CUR_ON = 2;
        //static const device_timer_id TIMER_CUR_OFF = 3;
        //static const device_timer_id TIMER_HSYNC_ON = 4;
        //static const device_timer_id TIMER_HSYNC_OFF = 5;
        //static const device_timer_id TIMER_LIGHT_PEN_LATCH = 6;
        //static const device_timer_id TIMER_UPD_ADR = 7;
        //static const device_timer_id TIMER_UPD_TRANS = 8;

        //emu_timer *m_line_timer;
        //emu_timer *m_de_off_timer;
        //emu_timer *m_cur_on_timer;
        //emu_timer *m_cur_off_timer;
        //emu_timer *m_hsync_on_timer;
        //emu_timer *m_hsync_off_timer;
        //emu_timer *m_light_pen_latch_timer;
        //emu_timer *m_upd_adr_timer;
        //emu_timer *m_upd_trans_timer;

        /* computed values - do NOT state save these! */
        /* These computed are used to define the screen parameters for a driver */
        //uint16_t  m_horiz_pix_total;
        //uint16_t  m_vert_pix_total;
        //uint16_t  m_max_visible_x;
        //uint16_t  m_max_visible_y;
        //uint16_t  m_hsync_on_pos;
        //uint16_t  m_hsync_off_pos;
        //uint16_t  m_vsync_on_pos;
        //uint16_t  m_vsync_off_pos;
        //bool    m_has_valid_parameters;
        //bool    m_display_disabled_msg_shown;

        //uint16_t   m_current_disp_addr;   /* the display address currently drawn (used only in mc6845_update) */

        //bool     m_light_pen_latched;
        //attotime m_upd_time;


        /************************
         interface CRTC - driver
         ************************/

        bool m_show_border_area;        /* visible screen area (false) active display (true) active display + blanking */
        int m_noninterlace_adjust;      /* adjust max ras in non-interlace mode */
        int m_interlace_adjust;         /* adjust max ras in interlace mode */

        uint32_t m_clk_scale;

        /* visible screen area adjustment */
        int m_visarea_adjust_min_x;
        int m_visarea_adjust_max_x;
        int m_visarea_adjust_min_y;
        int m_visarea_adjust_max_y;

        int m_hpixels_per_column;       /* number of pixels per video memory address */

        reconfigure_delegate m_reconfigure_cb;

        /* if specified, this gets called before any pixel update,
         optionally return a pointer that will be passed to the
         update and tear down callbacks */
        begin_update_delegate m_begin_update_cb;

        /* this gets called for every row, the driver must output
         x_count * hpixels_per_column pixels.
         cursor_x indicates the character position where the cursor is, or -1
         if there is no cursor on this row */
        update_row_delegate m_update_row_cb;

        /* if specified, this gets called after all row updating is complete */
        end_update_delegate m_end_update_cb;

        /* Called whenever the update address changes
         * For vblank/hblank timing strobe indicates the physical update.
         * vblank/hblank timing not supported yet! */
        on_update_addr_changed_delegate m_on_update_addr_changed_cb;

        /* if specified, this gets called for every change of the display enable pin (pin 18) */
        devcb_write_line m_out_de_cb;

        /* if specified, this gets called for every change of the cursor pin (pin 19) */
        devcb_write_line m_out_cur_cb;

        /* if specified, this gets called for every change of the HSYNC pin (pin 39) */
        devcb_write_line m_out_hsync_cb;

        /* if specified, this gets called for every change of the VSYNC pin (pin 40) */
        devcb_write_line m_out_vsync_cb;


        // construction/destruction
        mc6845_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, MC6845, tag, owner, clock)
        {
        }


        mc6845_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_divideo = new device_video_interface(mconfig, this, false);  //, device_video_interface(mconfig, *this, false)


            m_show_border_area = true;
            m_noninterlace_adjust = 0;
            m_interlace_adjust = 0;
            m_clk_scale = 1;
            m_visarea_adjust_min_x = 0;
            m_visarea_adjust_max_x = 0;
            m_visarea_adjust_min_y = 0;
            m_visarea_adjust_max_y = 0;
            m_hpixels_per_column = 0;
            m_reconfigure_cb = null;
            m_begin_update_cb = null;
            m_update_row_cb = null;
            m_end_update_cb = null;
            m_on_update_addr_changed_cb = null;
            m_out_de_cb = new devcb_write_line(this);
            m_out_cur_cb = new devcb_write_line(this);
            m_out_hsync_cb = new devcb_write_line(this);
            m_out_vsync_cb = new devcb_write_line(this);
        }


        public device_video_interface divideo { get { return m_divideo; } }


        public void set_show_border_area(bool show) { m_show_border_area = show; }
        //void set_visarea_adjust(int min_x, int max_x, int min_y, int max_y)
        //{
        //    m_visarea_adjust_min_x = min_x;
        //    m_visarea_adjust_max_x = max_x;
        //    m_visarea_adjust_min_y = min_y;
        //    m_visarea_adjust_max_y = max_y;
        //}
        public void set_char_width(int pixels) { m_hpixels_per_column = pixels; }

        //template <typename... T> void set_reconfigure_callback(T &&... args) { m_reconfigure_cb.set(std::forward<T>(args)...); }
        //template <typename... T> void set_begin_update_callback(T &&... args) { m_begin_update_cb.set(std::forward<T>(args)...); }
        //template <typename... T> void set_update_row_callback(T &&... args) { m_update_row_cb.set(std::forward<T>(args)...); }
        //template <typename... T> void set_end_update_callback(T &&... args) { m_end_update_cb.set(std::forward<T>(args)...); }
        //template <typename... T> void set_on_update_addr_change_callback(T &&... args) { m_on_update_addr_changed_cb.set(std::forward<T>(args)...); }

        //auto out_de_callback() { return m_out_de_cb.bind(); }
        //auto out_cur_callback() { return m_out_cur_cb.bind(); }
        //auto out_hsync_callback() { return m_out_hsync_cb.bind(); }
        //auto out_vsync_callback() { return m_out_vsync_cb.bind(); }

        /* select one of the registers for reading or writing */
        //void address_w(uint8_t data);

        /* read from the status register */
        //uint8_t status_r();

        /* read from the currently selected register */
        //uint8_t register_r();

        /* write to the currently selected register */
        //void register_w(uint8_t data);

        // read display enable line state
        //DECLARE_READ_LINE_MEMBER( de_r );

        // read cursor line state
        //DECLARE_READ_LINE_MEMBER( cursor_r );

        // read horizontal sync line state
        //DECLARE_READ_LINE_MEMBER( hsync_r );

        // read vertical sync line state
        //DECLARE_READ_LINE_MEMBER( vsync_r );

        /* return the current value on the MA0-MA13 pins */
        //uint16_t get_ma();

        /* return the current value on the RA0-RA4 pins */
        //uint8_t get_ra();

        /* simulates the LO->HI clocking of the light pen pin (pin 3) */
        //void assert_light_pen_input();

        /* set number of pixels per video memory address */
        //void set_hpixels_per_column(int hpixels_per_column);

        /* updates the screen -- this will call begin_update(),
           followed by update_row() repeatedly and after all row
           updating is complete, end_update() */
        //uint32_t screen_update(screen_device &screen, bitmap_rgb32 &bitmap, const rectangle &cliprect);


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_reset() { throw new emu_unimplemented(); }
        protected override void device_post_load() { throw new emu_unimplemented(); }
        protected override void device_clock_changed() { throw new emu_unimplemented(); }
        protected override void device_timer(emu_timer timer, device_timer_id id, int param) { throw new emu_unimplemented(); }

        //attotime cclks_to_attotime(uint64_t clocks) const { return clocks_to_attotime(clocks * m_clk_scale); }
        //uint64_t attotime_to_cclks(const attotime &duration) const { return attotime_to_clocks(duration) / m_clk_scale; }


        //void update_upd_adr_timer();
        //void call_on_update_address(int strobe);
        //void transparent_update();
        //void recompute_parameters(bool postload);
        //void update_counters();
        //void set_de(int state);
        //void set_hsync(int state);
        //void set_vsync(int state);
        //void set_cur(int state);
        //bool match_line();
        protected virtual bool check_cursor_visible(uint16_t ra, uint16_t line_addr) { throw new emu_unimplemented(); }
        //void handle_line_timer();
        protected virtual void update_cursor_state() { throw new emu_unimplemented(); }
        protected virtual uint8_t draw_scanline(int y, bitmap_rgb32 bitmap, rectangle cliprect) { throw new emu_unimplemented(); }
    }


    //class mc6845_1_device : public mc6845_device

    //class r6545_1_device : public mc6845_device

    //class c6545_1_device : public mc6845_device

    //class hd6845s_device : public mc6845_device

    //class sy6545_1_device : public mc6845_device

    //class sy6845e_device : public mc6845_device

    //class hd6345_device : public hd6845s_device

    //class ams40489_device : public mc6845_device

    //class mos8563_device : public mc6845_device,

    //class mos8568_device : public mos8563_device


    public static class mc6845_global
    {
        public static mc6845_device MC6845(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<mc6845_device>(mconfig, tag, mc6845_device.MC6845, clock); }
    }
}
