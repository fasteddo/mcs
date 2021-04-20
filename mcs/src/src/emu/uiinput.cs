// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using char32_t = System.UInt32;
using ioport_value = System.UInt32;
using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using s32 = System.Int32;
using u8 = System.Byte;


namespace mame
{
    public class ui_event
    {
        public enum type
        {
            NONE,
            MOUSE_MOVE,
            MOUSE_LEAVE,
            MOUSE_DOWN,
            MOUSE_UP,
            MOUSE_RDOWN,
            MOUSE_RUP,
            MOUSE_DOUBLE_CLICK,
            MOUSE_WHEEL,
            IME_CHAR
        }

        public type event_type;
        public render_target target;
        public int mouse_x;
        public int mouse_y;
        //input_item_id       key;
        public char32_t ch;
        public Int16 zdelta;
        public int num_lines;

        public ui_event(type type) { event_type = type; }
    }


    // ======================> ui_input_manager
    public class ui_input_manager : global_object
    {
        //enum
        //{
        const u8 SEQ_PRESSED_FALSE = 0;      /* not pressed */
        const u8 SEQ_PRESSED_TRUE  = 1;      /* pressed */
        const u8 SEQ_PRESSED_RESET = 2;      /* reset -- converted to FALSE once detected as not pressed */
        //}


        const int EVENT_QUEUE_SIZE = 128;


        // internal state
        running_machine m_machine;                  // reference to our machine

        // pressed states; retrieved with ui_input_pressed()
        bool m_presses_enabled;
        osd_ticks_t [] m_next_repeat = new osd_ticks_t[(int)ioport_type.IPT_COUNT];
        u8 [] m_seqpressed = new u8[(int)ioport_type.IPT_COUNT];

        // mouse position/info
        render_target m_current_mouse_target;
        s32 m_current_mouse_x;
        s32 m_current_mouse_y;
        bool m_current_mouse_down;
        ioport_field m_current_mouse_field;

        // popped states; ring buffer of ui_events
        ui_event [] m_events = new ui_event[EVENT_QUEUE_SIZE];
        int m_events_start;
        int m_events_end;


        // construction/destruction
        //-------------------------------------------------
        //  ui_input_manager - constructor
        //-------------------------------------------------
        public ui_input_manager(running_machine machine)
        {
            m_machine = machine;
            m_presses_enabled = true;
            m_current_mouse_target = null;
            m_current_mouse_x = -1;
            m_current_mouse_y = -1;
            m_current_mouse_down = false;
            m_current_mouse_field = null;
            m_events_start = 0;
            m_events_end = 0;


            std.fill(m_next_repeat, (osd_ticks_t)0);
            std.fill(m_seqpressed, (u8)0);


            // add a frame callback to poll inputs
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_FRAME, frame_update);
        }


        /*-------------------------------------------------
            frame_update - looks through pressed
            input as per events pushed our way and posts
            corresponding IPT_UI_* events
        -------------------------------------------------*/
        void frame_update(running_machine machine)
        {
            // update the state of all the UI keys
            for (ioport_type code = (ioport_type)(ioport_type.IPT_UI_FIRST + 1); code < ioport_type.IPT_UI_LAST; ++code)
            {
                if (m_presses_enabled)
                {
                    bool pressed = machine.ioport().type_pressed(code);
                    if (!pressed || m_seqpressed[(int)code] != SEQ_PRESSED_RESET)
                        m_seqpressed[(int)code] = pressed ? (u8)1 : (u8)0;
                }
                else
                {
                    // UI key presses are disabled
                    m_seqpressed[(int)code] = 0;  //false;
                }
            }

            // perform mouse hit testing
            ioport_field mouse_field = m_current_mouse_down ? find_mouse_field() : null;
            if (m_current_mouse_field != mouse_field)
            {
                // clear the old field if there was one
                if (m_current_mouse_field != null)
                    m_current_mouse_field.set_value(0);

                // set the new field if it exists and isn't already being pressed
                if (mouse_field != null && !mouse_field.digital_value())
                    mouse_field.set_value(1);

                // update internal state
                m_current_mouse_field = mouse_field;
            }
        }


        // pushes a single event onto the queue
        /*-------------------------------------------------
            push_event - pushes a single event
            onto the queue
        -------------------------------------------------*/
        bool push_event(ui_event evt)
        {
            // some pre-processing (this is an icky place to do this stuff!)
            switch (evt.event_type)
            {
                case ui_event.type.MOUSE_MOVE:
                    m_current_mouse_target = evt.target;
                    m_current_mouse_x = evt.mouse_x;
                    m_current_mouse_y = evt.mouse_y;
                    break;

                case ui_event.type.MOUSE_LEAVE:
                    if (m_current_mouse_target == evt.target)
                    {
                        m_current_mouse_target = null;
                        m_current_mouse_x = -1;
                        m_current_mouse_y = -1;
                    }
                    break;

                case ui_event.type.MOUSE_DOWN:
                    m_current_mouse_down = true;
                    break;

                case ui_event.type.MOUSE_UP:
                    m_current_mouse_down = false;
                    break;

                default:
                    /* do nothing */
                    break;
            }

            // is the queue filled up?
            if ((m_events_end + 1) % m_events.Length == m_events_start)
                return false;

            m_events[m_events_end++] = evt;
            m_events_end %= m_events.Length;
            return true;
        }


        // pops an event off of the queue
        /*-------------------------------------------------
            pop_event - pops an event off of the queue
        -------------------------------------------------*/
        public bool pop_event(out ui_event evt)
        {
            if (m_events_start != m_events_end)
            {
                evt = m_events[m_events_start++];
                m_events_start %= m_events.Length;
                return true;
            }
            else
            {
                evt = new ui_event(ui_event.type.NONE);  //memset(evt, 0, sizeof(*evt));
                return false;
            }
        }


        // check the next event type without removing it
        public ui_event.type peek_event_type() { return (m_events_start != m_events_end) ? m_events[m_events_start].event_type : ui_event.type.NONE; }


        // clears all outstanding events
        /*-------------------------------------------------
            reset - clears all outstanding events
            and resets the sequence states
        -------------------------------------------------*/
        public void reset()
        {
            int code;

            m_events_start = 0;
            m_events_end = 0;
            for (code = (int)ioport_type.IPT_UI_FIRST + 1; code < (int)ioport_type.IPT_UI_LAST; code++)
            {
                m_seqpressed[code] = SEQ_PRESSED_RESET;
                m_next_repeat[code] = 0;
            }
        }


        // retrieves the current location of the mouse
        /*-------------------------------------------------
            find_mouse - retrieves the current
            location of the mouse
        -------------------------------------------------*/
        public render_target find_mouse(out int x, out int y, out bool button)
        {
            //if (x != nullptr)
                x = m_current_mouse_x;
            //if (y != nullptr)
                y = m_current_mouse_y;
            //if (button != nullptr)
                button = m_current_mouse_down;
            return m_current_mouse_target;
        }


        /*-------------------------------------------------
            find_mouse_field - retrieves the input field
            the mouse is currently pointing at
        -------------------------------------------------*/
        ioport_field find_mouse_field()
        {
            // map the point and determine what was hit
            if (m_current_mouse_target != null)
            {
                ioport_port port = null;
                ioport_value mask;
                float x, y;
                if (m_current_mouse_target.map_point_input(m_current_mouse_x, m_current_mouse_y, out port, out mask, out x, out y))
                {
                    if (port != null)
                        return port.field(mask);
                }
            }

            return null;
        }


        // return TRUE if a key down for the given user interface sequence is detected
        /*-------------------------------------------------
            pressed - return TRUE if a key down
            for the given user interface sequence is
            detected
        -------------------------------------------------*/
        public bool pressed(int code)
        {
            return pressed_repeat(code, 0);
        }


        // enable/disable UI key presses
        //bool presses_enabled() const { return m_presses_enabled; }
        //void set_presses_enabled(bool enabled) { m_presses_enabled = enabled; }


        // return true if a key down for the given user interface sequence is detected, or if autorepeat at the given speed is triggered
        /*-------------------------------------------------
            pressed_repeat - return TRUE if a key
            down for the given user interface sequence is
            detected, or if autorepeat at the given speed
            is triggered
        -------------------------------------------------*/
        public bool pressed_repeat(int code, int speed)
        {
            bool pressed;

            profiler_global.g_profiler.start(profile_type.PROFILER_INPUT);

            /* get the status of this key (assumed to be only in the defaults) */
            assert(code >= (int)ioport_type.IPT_UI_CONFIGURE && code <= (int)ioport_type.IPT_OSD_16);
            pressed = m_seqpressed[code] == SEQ_PRESSED_TRUE;

            /* if down, handle it specially */
            if (pressed)
            {
                osd_ticks_t tps = osdcore_global.m_osdcore.osd_ticks_per_second();

                /* if this is the first press, set a 3x delay and leave pressed = 1 */
                if (m_next_repeat[code] == 0)
                {
                    m_next_repeat[code] = osdcore_global.m_osdcore.osd_ticks() + 3 * (osd_ticks_t)speed * tps / 60;
                }
                /* if this is an autorepeat case, set a 1x delay and leave pressed = 1 */
                else if (speed > 0 && (osdcore_global.m_osdcore.osd_ticks() + tps - m_next_repeat[code]) >= tps)
                {
                    // In the autorepeatcase, we need to double check the key is still pressed
                    // as there can be a delay between the key polling and our processing of the event
                    m_seqpressed[code] = machine().ioport().type_pressed((ioport_type)code) ? (byte)1 : (byte)0;
                    pressed = (m_seqpressed[code] == SEQ_PRESSED_TRUE);
                    if (pressed)
                        m_next_repeat[code] += 1 * (osd_ticks_t)speed * tps / 60;
                }
                /* otherwise, reset pressed = 0 */
                else
                {
                    pressed = false;
                }
            }

            /* if we're not pressed, reset the memory field */
            else
                m_next_repeat[code] = 0;

            profiler_global.g_profiler.stop();

            return pressed;
        }


        // getters
        running_machine machine() { return m_machine; }


        // queueing events

        /*-------------------------------------------------
            push_mouse_move_event - pushes a mouse
            move event to the specified render_target
        -------------------------------------------------*/
        public void push_mouse_move_event(render_target target, int x, int y)
        {
            ui_event evt = new ui_event(ui_event.type.NONE);
            evt.event_type = ui_event.type.MOUSE_MOVE;
            evt.target = target;
            evt.mouse_x = x;
            evt.mouse_y = y;
            push_event(evt);
        }

        /*-------------------------------------------------
            push_mouse_leave_event - pushes a
            mouse leave event to the specified render_target
        -------------------------------------------------*/
        public void push_mouse_leave_event(render_target target)
        {
            ui_event evt = new ui_event(ui_event.type.NONE);
            evt.event_type = ui_event.type.MOUSE_LEAVE;
            evt.target = target;
            push_event(evt);
        }

        /*-------------------------------------------------
            push_mouse_down_event - pushes a mouse
            down event to the specified render_target
        -------------------------------------------------*/
        public void push_mouse_down_event(render_target target, int x, int y)
        {
            ui_event evt = new ui_event(ui_event.type.NONE);
            evt.event_type = ui_event.type.MOUSE_DOWN;
            evt.target = target;
            evt.mouse_x = x;
            evt.mouse_y = y;
            push_event(evt);
        }

        /*-------------------------------------------------
            push_mouse_down_event - pushes a mouse
            down event to the specified render_target
        -------------------------------------------------*/
        public void push_mouse_up_event(render_target target, int x, int y)
        {
            ui_event evt = new ui_event(ui_event.type.NONE);
            evt.event_type = ui_event.type.MOUSE_UP;
            evt.target = target;
            evt.mouse_x = x;
            evt.mouse_y = y;
            push_event(evt);
        }

        
        //void push_mouse_rdown_event(render_target* target, INT32 x, INT32 y);

        //void push_mouse_rup_event(render_target* target, INT32 x, INT32 y);


        /*-------------------------------------------------
            push_mouse_double_click_event - pushes
            a mouse double-click event to the specified
            render_target
        -------------------------------------------------*/
        public void push_mouse_double_click_event(render_target target, int x, int y)
        {
            ui_event evt = new ui_event(ui_event.type.NONE);
            evt.event_type = ui_event.type.MOUSE_DOUBLE_CLICK;
            evt.target = target;
            evt.mouse_x = x;
            evt.mouse_y = y;
            push_event(evt);
        }

        /*-------------------------------------------------
            push_char_event - pushes a char event
            to the specified render_target
        -------------------------------------------------*/
        public void push_char_event(render_target target, char32_t ch)
        {
            ui_event evt = new ui_event(ui_event.type.NONE);
            evt.event_type = ui_event.type.IME_CHAR;
            evt.target = target;
            evt.ch = ch;
            push_event(evt);
        }

        //void push_mouse_wheel_event(render_target *target, INT32 x, INT32 y, short delta, int ucNumLines);

        //void mark_all_as_pressed();
    }
}
