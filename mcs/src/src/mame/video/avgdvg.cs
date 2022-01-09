// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using s64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using unsigned = System.UInt32;

using static mame.device_global;
using static mame.emucore_global;
using static mame.util;


namespace mame
{
    public abstract class avgdvg_device_base : device_t
    {
        const unsigned MAXVECT = 10000;

        /*************************************
         *
         *  Macros and defines
         *
         *************************************/
        const int MASTER_CLOCK = 12096000;
        const int VGSLICE = 10000;
        const int VGVECTOR = 0;
        const int VGCLIP = 1;


        struct vgvector
        {
            public int x;
            public int y;
            public rgb_t color;
            public int intensity;
            public int arg1;
            public int arg2;
            public int status;
        }


        protected required_device<vector_device> m_vector;
        protected required_address_space m_memspace;
        protected offs_t m_membase;

        protected int m_xmin;
        protected int m_ymin;
        protected int m_xcenter;
        protected int m_ycenter;

        int m_nvect;
        vgvector [] m_vectbuf = new vgvector[MAXVECT];

        protected u16 m_pc;
        protected u8 m_sp;
        protected u16 m_dvx;
        protected u16 m_dvy;
        protected u16 [] m_stack = new u16[4];
        protected u16 m_data;

        protected u8 m_state_latch;
        protected u8 m_scale;
        protected u8 m_intensity;

        protected u8 m_op;
        protected u8 m_halt;
        u8 m_sync_halt;

        protected s32 m_xpos;
        protected s32 m_ypos;


        required_region_ptr<u8> m_prom;
        emu_timer m_vg_run_timer;
        emu_timer m_vg_halt_timer;

        bool m_flip_x;
        bool m_flip_y;


        protected avgdvg_device_base(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_vector = new required_device<vector_device>(this, finder_base.DUMMY_TAG);
            m_memspace = new required_address_space(this, finder_base.DUMMY_TAG, -1);
            m_membase = 0;
            m_nvect = 0;
            m_pc = 0;
            m_sp = 0;
            m_dvx = 0;
            m_dvy = 0;
            m_stack = new u16 [] { 0, 0, 0, 0 };
            m_data = 0;
            m_state_latch = 0;
            m_scale = 0;
            m_intensity = 0;
            m_op = 0;
            m_halt = 0;
            m_sync_halt = 0;
            m_xpos = 0;
            m_ypos = 0;
            m_prom = new required_region_ptr<u8>(this, "prom");
            m_vg_run_timer = null;
            m_vg_halt_timer = null;
        }


        //template <typename T>
        public void set_vector(string tag)  //void set_vector(T &&tag)
        {
            m_vector.set_tag(tag);
        }


        //template <typename T>
        public void set_memory(finder_base tag, int no, offs_t base_)  //void set_memory(T &&tag, int no, offs_t base)
        {
            m_memspace.set_tag(tag, no);
            m_membase = base_;
        }


        /*************************************
         *
         *  VG halt/vggo
         *
         ************************************/

        //DECLARE_READ_LINE_MEMBER(done_r);
        public int done_r()
        {
            return (m_sync_halt != 0) ? 1 : 0;
        }


        public void go_w(u8 data = 0)
        {
            vggo();

            if (m_sync_halt != 0 && (m_nvect > 10))
            {
                /*
                 * This is a good time to start a new frame. Major Havoc
                 * sometimes sets VGGO after a very short vector list. That's
                 * why we ignore frames with less than 10 vectors.
                 */
                m_vector.op0.clear_list();
            }

            vg_flush();

            vg_set_halt(0);
            m_vg_run_timer.adjust(attotime.zero);
        }


        /*************************************
         *
         *  Reset
         *
         ************************************/

        public void reset_w(u8 data = 0)
        {
            vgrst();
            vg_set_halt(1);
        }


        //void go_word_w(u16 data = 0);
        //void reset_word_w(u16 data = 0);


        // Tempest and Quantum use this capability
        public void set_flip_x(bool flip) { m_flip_x = flip; }
        public void set_flip_y(bool flip) { m_flip_y = flip; }


        /*************************************
         *
         *  Vector generator init
         *
         ************************************/

        protected override void device_start()
        {
            if (!m_vector.op0.started())
                throw new device_missing_dependencies();

            m_vg_halt_timer = machine().scheduler().timer_alloc(vg_set_halt_callback);
            m_vg_run_timer = machine().scheduler().timer_alloc(run_state_machine);

            m_flip_x = m_flip_y = false;

            save_item(NAME(new { m_pc }));
            save_item(NAME(new { m_sp }));
            save_item(NAME(new { m_dvx }));
            save_item(NAME(new { m_dvy }));
            save_item(NAME(new { m_stack }));
            save_item(NAME(new { m_data }));
            save_item(NAME(new { m_state_latch }));
            save_item(NAME(new { m_scale }));
            save_item(NAME(new { m_intensity }));
            save_item(NAME(new { m_op }));
            save_item(NAME(new { m_halt }));
            save_item(NAME(new { m_sync_halt }));
            save_item(NAME(new { m_xpos }));
            save_item(NAME(new { m_ypos }));

            save_item(NAME(new { m_flip_x }));
            save_item(NAME(new { m_flip_y }));
        }


        protected abstract int handler_0();
        protected abstract int handler_1();
        protected abstract int handler_2();
        protected abstract int handler_3();
        protected abstract int handler_4();
        protected abstract int handler_5();
        protected abstract int handler_6();
        protected abstract int handler_7();
        protected abstract u8 state_addr();
        protected abstract void update_databus();
        protected abstract void vggo();
        protected abstract void vgrst();

        protected u8 OP0() { return (u8)BIT(m_op, 0); }
        protected u8 OP1() { return (u8)BIT(m_op, 1); }
        //u8 OP2() const { return BIT(m_op, 2); }
        protected u8 OP3() { return (u8)BIT(m_op, 3); }

        u8 ST3() { return (u8)BIT(m_state_latch, 3); }


        protected void apply_flipping(ref int x, ref int y)
        {
            if (m_flip_x)
                x += (m_xcenter - x) << 1;
            if (m_flip_y)
                y += (m_ycenter - y) << 1;
        }


        /*************************************
         *
         *  halt functions
         *
         *************************************/

        void vg_set_halt(int dummy)
        {
            m_halt = (u8)dummy;
            m_sync_halt = (u8)dummy;
        }


        /*************************************
         *
         *  Vector buffering
         *
         *************************************/
        void vg_flush()
        {
            int cx0 = 0;
            int cy0 = 0;
            int cx1 = 0x5000000;
            int cy1 = 0x5000000;
            int i = 0;

            while (m_vectbuf[i].status == VGCLIP)
                i++;

            int xs = m_vectbuf[i].x;
            int ys = m_vectbuf[i].y;

            for (i = 0; i < m_nvect; i++)
            {
                if (m_vectbuf[i].status == VGVECTOR)
                {
                    int xe = m_vectbuf[i].x;
                    int ye = m_vectbuf[i].y;
                    int x0 = xs;
                    int y0 = ys;
                    int x1 = xe;
                    int y1 = ye;

                    xs = xe;
                    ys = ye;

                    if ((x0 < cx0 && x1 < cx0) || (x0 > cx1 && x1 > cx1))
                        continue;

                    if (x0 < cx0)
                    {
                        y0 += (int)((s64)(cx0 - x0) * (s64)(y1 - y0) / (x1 - x0));
                        x0 = cx0;
                    }
                    else if (x0 > cx1)
                    {
                        y0 += (int)((s64)(cx1 - x0) * (s64)(y1 - y0) / (x1 - x0));
                        x0 = cx1;
                    }
                    if (x1 < cx0)
                    {
                        y1 += (int)((s64)(cx0 - x1) * (s64)(y1 - y0) / (x1 - x0));
                        x1 = cx0;
                    }
                    else if (x1 > cx1)
                    {
                        y1 += (int)((s64)(cx1 - x1) * (s64)(y1 - y0) / (x1 - x0));
                        x1 = cx1;
                    }

                    if ((y0 < cy0 && y1 < cy0) || (y0 > cy1 && y1 > cy1))
                        continue;

                    if (y0 < cy0)
                    {
                        x0 += (int)((s64)(cy0 - y0) * (s64)(x1 - x0) / (y1 - y0));
                        y0 = cy0;
                    }
                    else if (y0 > cy1)
                    {
                        x0 += (int)((s64)(cy1 - y0) * (s64)(x1 - x0) / (y1 - y0));
                        y0 = cy1;
                    }
                    if (y1 < cy0)
                    {
                        x1 += (int)((s64)(cy0 - y1) * (s64)(x1 - x0) / (y1 - y0));
                        y1 = cy0;
                    }
                    else if (y1 > cy1)
                    {
                        x1 += (int)((s64)(cy1 - y1) * (s64)(x1 - x0) / (y1 - y0));
                        y1 = cy1;
                    }

                    m_vector.op0.add_point(x0, y0, m_vectbuf[i].color, 0);
                    m_vector.op0.add_point(x1, y1, m_vectbuf[i].color, m_vectbuf[i].intensity);
                }

                if (m_vectbuf[i].status == VGCLIP)
                {
                    cx0 = m_vectbuf[i].x;
                    cy0 = m_vectbuf[i].y;
                    cx1 = m_vectbuf[i].arg1;
                    cy1 = m_vectbuf[i].arg2;

                    //using std::swap;

                    if (cx0 > cx1)
                        std.swap(ref cx0, ref cx1);
                    if (cy0 > cy1)
                        std.swap(ref cy0, ref cy1);
                }
            }

            m_nvect = 0;
        }


        protected void vg_add_point_buf(int x, int y, rgb_t color, int intensity)
        {
            if (m_nvect < MAXVECT)
            {
                m_vectbuf[m_nvect].status = VGVECTOR;
                m_vectbuf[m_nvect].x = x;
                m_vectbuf[m_nvect].y = y;
                m_vectbuf[m_nvect].color = color;
                m_vectbuf[m_nvect].intensity = intensity;
                m_nvect++;
            }
        }


        //void vg_add_clip(int xmin, int ymin, int xmax, int ymax);


        //TIMER_CALLBACK_MEMBER(vg_set_halt_callback);
        void vg_set_halt_callback(object ptr, int param)
        {
            vg_set_halt(param);
        }


        //TIMER_CALLBACK_MEMBER(run_state_machine);
        void run_state_machine(object ptr, int param)
        {
            int cycles = 0;

            while (cycles < VGSLICE)
            {
                // Get next state
                m_state_latch = (u8)((m_state_latch & 0x10) | (m_prom.op[state_addr()] & 0xf));

                if (ST3() != 0)
                {
                    // Read vector RAM/ROM
                    update_databus();

                    // Decode state and call the corresponding handler
                    switch (m_state_latch & 7)
                    {
                    case 0 : cycles += handler_0(); break;
                    case 1 : cycles += handler_1(); break;
                    case 2 : cycles += handler_2(); break;
                    case 3 : cycles += handler_3(); break;
                    case 4 : cycles += handler_4(); break;
                    case 5 : cycles += handler_5(); break;
                    case 6 : cycles += handler_6(); break;
                    case 7 : cycles += handler_7(); break;
                    }
                }

                // If halt flag was set, let CPU catch up before we make halt visible
                if (m_halt != 0 && (m_state_latch & 0x10) == 0)
                    m_vg_halt_timer.adjust(attotime.from_hz(MASTER_CLOCK) * (u32)cycles, 1);

                m_state_latch = (u8)((m_halt << 4) | (m_state_latch & 0xf));
                cycles += 8;
            }

            m_vg_run_timer.adjust(attotime.from_hz(MASTER_CLOCK) * (u32)cycles);
        }
    }


    public class dvg_device : avgdvg_device_base
    {
        //DEFINE_DEVICE_TYPE(DVG,          dvg_device,          "dvg",          "Atari DVG")
        static device_t device_creator_dvg_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new dvg_device(mconfig, tag, owner, clock); }
        public static readonly device_type DVG = DEFINE_DEVICE_TYPE(device_creator_dvg_device, "dvg", "Atari DVG");


        dvg_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, DVG, tag, owner, clock)
        { }


        protected override void device_start()
        {
            base.device_start();

            rectangle visarea = m_vector.op0.m_divideo.screen().visible_area();

            m_xmin = visarea.min_x;
            m_ymin = visarea.min_y;

            m_xcenter = 512;
            m_ycenter = 512;
        }


        protected override int handler_0()  // dvg_dmapush
        {
            if (OP0() == 0)
            {
                m_sp = (u8)((m_sp + 1) & 0xf);
                m_stack[m_sp & 3] = m_pc;
            }
            return 0;
        }


        protected override int handler_1()  // dvg_dmald
        {
            if (OP0() != 0)
            {
                m_pc = m_stack[m_sp & 3];
                m_sp = (u8)((m_sp - 1) & 0xf);
            }
            else
            {
                m_pc = m_dvy;
            }

            return 0;
        }


        protected override int handler_2()  //dvg_gostrobe
        {
            int scale;

            if (m_op == 0xf)
            {
                scale = (m_scale +
                            (((m_dvy & 0x800) >> 11)
                            | (((m_dvx & 0x800) ^ 0x800) >> 10)
                            | ((m_dvx & 0x800)  >> 9))) & 0xf;

                m_dvy &= 0xf00;
                m_dvx &= 0xf00;
            }
            else
            {
                scale = (m_scale + m_op) & 0xf;
            }

            int fin = 0xfff - (((2 << scale) & 0x7ff) ^ 0xfff);

            // Count up or down
            int dx = (m_dvx & 0x400) != 0 ? -1 : +1;
            int dy = (m_dvy & 0x400) != 0 ? -1 : +1;

            // Scale factor for rate multipliers
            int mx = (m_dvx << 2) & 0xfff;
            int my = (m_dvy << 2) & 0xfff;

            int cycles = 8 * fin;
            int c = 0;

            while (fin-- != 0)
            {
                /*
                 *  The 7497 Bit Rate Multiplier is a 6 bit counter with
                 *  clever decoding of output bits to perform the following
                 *  operation:
                 *
                 *  fout = m/64 * fin
                 *
                 *  where fin is the input frequency, fout is the output
                 *  frequency and m is a factor at the input pins. Output
                 *  pulses are more or less evenly spaced so we get straight
                 *  lines. The DVG has two cascaded 7497s for each coordinate.
                 */

                int countx = 0;
                int county = 0;

                for (int bit = 0; bit < 12; bit++)
                {
                    if ((c & ((1 << (bit+1)) - 1)) == ((1 << bit) - 1))
                    {
                        if ((mx & (1 << (11 - bit))) != 0)
                            countx = 1;

                        if ((my & (1 << (11 - bit))) != 0)
                            county = 1;
                    }
                }

                c = (c + 1) & 0xfff;

                /*
                 *  Since x- and y-counters always hold the correct count
                 *  wrt. to each other, we can do clipping exactly like the
                 *  hardware does. That is, as soon as any counter's bit 10
                 *  changes to high, we finish the vector. If bit 10 changes
                 *  from high to low, we start a new vector.
                 */

                if (countx != 0)
                {
                    // Is y valid and x entering or leaving the valid range?
                    if ((m_ypos & 0x400) == 0 && ((m_xpos ^ (m_xpos + dx)) & 0x400) != 0)
                    {
                        if (((m_xpos + dx) & 0x400) != 0)  // We are leaving the valid range
                            dvg_draw_to(m_xpos, m_ypos, m_intensity);
                        else                        // We are entering the valid range
                            dvg_draw_to((m_xpos + dx) & 0xfff, m_ypos, 0);
                    }
                    m_xpos = (m_xpos + dx) & 0xfff;
                }

                if (county != 0)
                {
                    if ((m_xpos & 0x400) == 0 && ((m_ypos ^ (m_ypos + dy)) & 0x400) != 0)
                    {
                        if ((m_xpos & 0x400) == 0)
                        {
                            if (((m_ypos + dy) & 0x400) != 0)
                                dvg_draw_to(m_xpos, m_ypos, m_intensity);
                            else
                                dvg_draw_to(m_xpos, (m_ypos + dy) & 0xfff, 0);
                        }
                    }
                    m_ypos = (m_ypos + dy) & 0xfff;
                }
            }

            dvg_draw_to(m_xpos, m_ypos, m_intensity);

            return cycles;
        }


        protected override int handler_3()  // dvg_haltstrobe
        {
            m_halt = OP0();

            if (OP0() == 0)
            {
                m_xpos = m_dvx & 0xfff;
                m_ypos = m_dvy & 0xfff;
                dvg_draw_to(m_xpos, m_ypos, 0);
            }

            return 0;
        }


        protected override int handler_4()  // dvg_latch0
        {
            m_dvy &= 0xf00;
            if (m_op == 0xf)
                handler_7(); //dvg_latch3
            else
                m_dvy = (u16)((m_dvy & 0xf00) | m_data);

            m_pc++;
            return 0;
        }


        protected override int handler_5()  //  dvg_latch1
        {
            m_dvy = (u16)((m_dvy & 0xff) | ((m_data & 0xf) << 8));
            m_op = (u8)(m_data >> 4);

            if (m_op == 0xf)
            {
                m_dvx &= 0xf00;
                m_dvy &= 0xf00;
            }

            return 0;
        }


        protected override int handler_6()  // dvg_latch2
        {
            m_dvx &= 0xf00;
            if (m_op != 0xf)
                m_dvx = (u16)((m_dvx & 0xf00) | m_data);

            if (OP1() != 0 && OP3() != 0)
                m_scale = m_intensity;

            m_pc++;
            return 0;
        }


        protected override int handler_7()  // dvg_latch3
        {
            m_dvx = (u16)((m_dvx & 0xff) | ((m_data & 0xf) << 8));
            m_intensity = (u8)(m_data >> 4);
            return 0;
        }


        protected override u8 state_addr()  // dvg_state_addr
        {
            u8 addr = (u8)(((((m_state_latch >> 4) ^ 1) & 1) << 7) | (m_state_latch & 0xf));

            if (OP3() != 0)
                addr |= (u8)((m_op & 7) << 4);

            return addr;
        }


        /*************************************
         *
         *  DVG handler functions
         *
         *************************************/

        protected override void update_databus()  // dvg_data
        {
            // DVG uses low bit of state for address
            m_data = m_memspace.op0.read_byte(m_membase + ((u32)m_pc << 1) + (m_state_latch & 1U));
        }


        protected override void vggo()  // dvg_vggo
        {
            m_dvy = 0;
            m_op = 0;
        }


        protected override void vgrst()  // dvg_vgrst
        {
            m_state_latch = 0;
            m_dvy = 0;
            m_op = 0;
        }


        void dvg_draw_to(int x, int y, int intensity)
        {
            apply_flipping(ref x, ref y);

            if (((x | y) & 0x400) == 0)
            {
                vg_add_point_buf(
                        (m_xmin + x - 512) << 16,
                        (m_ymin + 512 - y) << 16,
                        vector_device.color111(7),
                        intensity << 4);
            }
        }
    }


    //class avg_device : public avgdvg_device_base


    //class avg_tempest_device : public avg_device


    //class avg_mhavoc_device : public avg_device


    //class avg_starwars_device : public avg_device


    //class avg_quantum_device : public avg_device


    //class avg_bzone_device : public avg_device
}
