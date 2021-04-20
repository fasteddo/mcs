// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using NAudio;
using NAudio.Wave;

using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;


namespace mcsForm
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);


        Bitmap m_bitmap;
        BitmapHelper m_bitmapHelper;
        Random m_random = new Random();
        Graphics m_graphics;
        int m_bitmapXOffset = 10;
        int m_bitmapYOffset = 10;
        int m_updateCount = 0;


        IWavePlayer m_waveOutDevice;
        MameWaveProvider16 m_mameWavProvider16;


        public Form1()
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            this.MouseMove += Form1_MouseMove;
            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;
            this.MouseDoubleClick += Form1_MouseDoubleClick;
            this.MouseLeave += Form1_MouseLeave;
            this.FormClosing += Form1_FormClosing;


            m_bitmap = new Bitmap(Width - (m_bitmapXOffset * 2), Height - (m_bitmapYOffset * 2), System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            m_bitmapHelper = new BitmapHelper(m_bitmap);


            Timer updateTimer = new Timer();
            updateTimer.Interval = 50;
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Start();


            m_graphics = CreateGraphics();


            m_waveOutDevice = new WaveOut();
            m_mameWavProvider16 = new MameWaveProvider16();
            m_waveOutDevice.Init(m_mameWavProvider16);
            //AudioFileReader audioFileReader = new AudioFileReader("101.mp3");  // to test mp3 playback
            //m_waveOutDevice.Init(audioFileReader);
            m_waveOutDevice.Play();
        }


        void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (m_waveOutDevice != null)
            {
                m_waveOutDevice.Stop();
                m_waveOutDevice.Dispose();
                m_waveOutDevice = null;
            }
        }


        // master keyboard translation table
        //static const int win_key_trans_table[][4] =
        public static readonly Dictionary<Keys, mame.input_item_id> keymap = new Dictionary<Keys, mame.input_item_id>()
        {
            // MAME key             dinput key          virtual key     ascii

            { Keys.Escape, mame.input_item_id.ITEM_ID_ESC },  //          DIK_ESCAPE,         VK_ESCAPE,      27 },

            { Keys.D0, mame.input_item_id.ITEM_ID_0 },
            { Keys.D1, mame.input_item_id.ITEM_ID_1 },
            { Keys.D2, mame.input_item_id.ITEM_ID_2 },
            { Keys.D3, mame.input_item_id.ITEM_ID_3 },
            { Keys.D4, mame.input_item_id.ITEM_ID_4 },
            { Keys.D5, mame.input_item_id.ITEM_ID_5 },
            { Keys.D6, mame.input_item_id.ITEM_ID_6 },
            { Keys.D7, mame.input_item_id.ITEM_ID_7 },
            { Keys.D8, mame.input_item_id.ITEM_ID_8 },
            { Keys.D9, mame.input_item_id.ITEM_ID_9 },

            { Keys.OemMinus,        mame.input_item_id.ITEM_ID_MINUS },  //        DIK_MINUS,          VK_OEM_MINUS,   '-' },
            { Keys.Oemplus,         mame.input_item_id.ITEM_ID_EQUALS },  //       DIK_EQUALS,         VK_OEM_PLUS,    '=' },
            { Keys.Back,            mame.input_item_id.ITEM_ID_BACKSPACE },  //    DIK_BACK,           VK_BACK,        8 },
            { Keys.Tab,             mame.input_item_id.ITEM_ID_TAB },  //          DIK_TAB,            VK_TAB,         9 },

            { Keys.Q, mame.input_item_id.ITEM_ID_Q },
            { Keys.W, mame.input_item_id.ITEM_ID_W },
            { Keys.E, mame.input_item_id.ITEM_ID_E },
            { Keys.R, mame.input_item_id.ITEM_ID_R },
            { Keys.T, mame.input_item_id.ITEM_ID_T },
            { Keys.Y, mame.input_item_id.ITEM_ID_Y },
            { Keys.U, mame.input_item_id.ITEM_ID_U },
            { Keys.I, mame.input_item_id.ITEM_ID_I },
            { Keys.O, mame.input_item_id.ITEM_ID_O },
            { Keys.P, mame.input_item_id.ITEM_ID_P },

            { Keys.OemOpenBrackets, mame.input_item_id.ITEM_ID_OPENBRACE },  //    DIK_LBRACKET,       VK_OEM_4,       '[' },
            { Keys.Oem6,            mame.input_item_id.ITEM_ID_CLOSEBRACE },  //   DIK_RBRACKET,       VK_OEM_6,       ']' },
            { Keys.Enter,           mame.input_item_id.ITEM_ID_ENTER },  //        DIK_RETURN,         VK_RETURN,      13 },
            { Keys.LControlKey,     mame.input_item_id.ITEM_ID_LCONTROL },  //     DIK_LCONTROL,       VK_LCONTROL,    0 },

            { Keys.A, mame.input_item_id.ITEM_ID_A },
            { Keys.S, mame.input_item_id.ITEM_ID_S },
            { Keys.D, mame.input_item_id.ITEM_ID_D },
            { Keys.F, mame.input_item_id.ITEM_ID_F },
            { Keys.G, mame.input_item_id.ITEM_ID_G },
            { Keys.H, mame.input_item_id.ITEM_ID_H },
            { Keys.J, mame.input_item_id.ITEM_ID_J },
            { Keys.K, mame.input_item_id.ITEM_ID_K },
            { Keys.L, mame.input_item_id.ITEM_ID_L },

            { Keys.OemSemicolon,    mame.input_item_id.ITEM_ID_COLON },  //        DIK_SEMICOLON,      VK_OEM_1,       ';' },
            { Keys.OemQuotes,       mame.input_item_id.ITEM_ID_QUOTE },  //        DIK_APOSTROPHE,     VK_OEM_7,       '\'' },
            { Keys.Oemtilde,        mame.input_item_id.ITEM_ID_TILDE },  //        DIK_GRAVE,          VK_OEM_3,       '`' },
            { Keys.LShiftKey,       mame.input_item_id.ITEM_ID_LSHIFT },  //       DIK_LSHIFT,         VK_LSHIFT,      0 },
            { Keys.Oem5,            mame.input_item_id.ITEM_ID_BACKSLASH },  //    DIK_BACKSLASH,      VK_OEM_5,       '\\' },
            //{ Keys., mame.input_item_id.ITEM_ID_BACKSLASH2 },  //   DIK_OEM_102,        VK_OEM_102,     '<' },

            { Keys.Z, mame.input_item_id.ITEM_ID_Z },
            { Keys.X, mame.input_item_id.ITEM_ID_X },
            { Keys.C, mame.input_item_id.ITEM_ID_C },
            { Keys.V, mame.input_item_id.ITEM_ID_V },
            { Keys.B, mame.input_item_id.ITEM_ID_B },
            { Keys.N, mame.input_item_id.ITEM_ID_N },
            { Keys.M, mame.input_item_id.ITEM_ID_M },

            { Keys.Oemcomma,        mame.input_item_id.ITEM_ID_COMMA },  //        DIK_COMMA,          VK_OEM_COMMA,   ',' },
            { Keys.OemPeriod,       mame.input_item_id.ITEM_ID_STOP },  //         DIK_PERIOD,         VK_OEM_PERIOD,  '.' },
            { Keys.OemQuestion,     mame.input_item_id.ITEM_ID_SLASH },  //        DIK_SLASH,          VK_OEM_2,       '/' },
            { Keys.RShiftKey,       mame.input_item_id.ITEM_ID_RSHIFT },  //       DIK_RSHIFT,         VK_RSHIFT,      0 },
            //{ Keys., mame.input_item_id.ITEM_ID_ASTERISK },  //     DIK_MULTIPLY,       VK_MULTIPLY,    '*' },
            { Keys.LMenu,           mame.input_item_id.ITEM_ID_LALT },  //         DIK_LMENU,          VK_LMENU,       0 },
            { Keys.Space,           mame.input_item_id.ITEM_ID_SPACE },  //        DIK_SPACE,          VK_SPACE,       ' ' },
            { Keys.Capital,         mame.input_item_id.ITEM_ID_CAPSLOCK },  //     DIK_CAPITAL,        VK_CAPITAL,     0 },

            { Keys.F1,  mame.input_item_id.ITEM_ID_F1 },
            { Keys.F2,  mame.input_item_id.ITEM_ID_F2 },
            { Keys.F3,  mame.input_item_id.ITEM_ID_F3 },
            { Keys.F4,  mame.input_item_id.ITEM_ID_F4 },
            { Keys.F5,  mame.input_item_id.ITEM_ID_F5 },
            { Keys.F6,  mame.input_item_id.ITEM_ID_F6 },
            { Keys.F7,  mame.input_item_id.ITEM_ID_F7 },
            { Keys.F8,  mame.input_item_id.ITEM_ID_F8 },
            { Keys.F9,  mame.input_item_id.ITEM_ID_F9 },
            { Keys.F10, mame.input_item_id.ITEM_ID_F10 },

            { Keys.NumLock,         mame.input_item_id.ITEM_ID_NUMLOCK },  //      DIK_NUMLOCK,        VK_NUMLOCK,     0 },
            { Keys.Scroll,          mame.input_item_id.ITEM_ID_SCRLOCK },  //      DIK_SCROLL,         VK_SCROLL,      0 },
            //{ ITEM_ID_7_PAD,        DIK_NUMPAD7,        VK_NUMPAD7,     0 },
            //{ ITEM_ID_8_PAD,        DIK_NUMPAD8,        VK_NUMPAD8,     0 },
            //{ ITEM_ID_9_PAD,        DIK_NUMPAD9,        VK_NUMPAD9,     0 },
            //{ ITEM_ID_MINUS_PAD,    DIK_SUBTRACT,       VK_SUBTRACT,    0 },
            //{ ITEM_ID_4_PAD,        DIK_NUMPAD4,        VK_NUMPAD4,     0 },
            //{ ITEM_ID_5_PAD,        DIK_NUMPAD5,        VK_NUMPAD5,     0 },
            //{ ITEM_ID_6_PAD,        DIK_NUMPAD6,        VK_NUMPAD6,     0 },
            //{ ITEM_ID_PLUS_PAD,     DIK_ADD,            VK_ADD,         0 },
            //{ ITEM_ID_1_PAD,        DIK_NUMPAD1,        VK_NUMPAD1,     0 },
            //{ ITEM_ID_2_PAD,        DIK_NUMPAD2,        VK_NUMPAD2,     0 },
            //{ ITEM_ID_3_PAD,        DIK_NUMPAD3,        VK_NUMPAD3,     0 },
            //{ ITEM_ID_0_PAD,        DIK_NUMPAD0,        VK_NUMPAD0,     0 },
            //{ ITEM_ID_DEL_PAD,      DIK_DECIMAL,        VK_DECIMAL,     0 },

            { Keys.F11, mame.input_item_id.ITEM_ID_F11 },
            { Keys.F12, mame.input_item_id.ITEM_ID_F12 },
            { Keys.F13, mame.input_item_id.ITEM_ID_F13 },
            { Keys.F14, mame.input_item_id.ITEM_ID_F14 },
            { Keys.F15, mame.input_item_id.ITEM_ID_F15 },

            //{ ITEM_ID_ENTER_PAD,    DIK_NUMPADENTER,    VK_RETURN,      0 },
            { Keys.RControlKey, mame.input_item_id.ITEM_ID_RCONTROL },  //     DIK_RCONTROL,       VK_RCONTROL,    0 },
            //{ ITEM_ID_SLASH_PAD,    DIK_DIVIDE,         VK_DIVIDE,      0 },
            //{ ITEM_ID_PRTSCR,       DIK_SYSRQ,          0,              0 },
            { Keys.RMenu,       mame.input_item_id.ITEM_ID_RALT },  //         DIK_RMENU,          VK_RMENU,       0 },
            //{ ITEM_ID_HOME,         DIK_HOME,           VK_HOME,        0 },
            { Keys.Up,          mame.input_item_id.ITEM_ID_UP },  //           DIK_UP,             VK_UP,          0 },
            //{ ITEM_ID_PGUP,         DIK_PRIOR,          VK_PRIOR,       0 },
            { Keys.Left,        mame.input_item_id.ITEM_ID_LEFT },  //         DIK_LEFT,           VK_LEFT,        0 },
            { Keys.Right,       mame.input_item_id.ITEM_ID_RIGHT },  //        DIK_RIGHT,          VK_RIGHT,       0 },
            //{ ITEM_ID_END,          DIK_END,            VK_END,         0 },
            { Keys.Down,        mame.input_item_id.ITEM_ID_DOWN },  //         DIK_DOWN,           VK_DOWN,        0 },
            //{ ITEM_ID_PGDN,         DIK_NEXT,           VK_NEXT,        0 },
            //{ ITEM_ID_INSERT,       DIK_INSERT,         VK_INSERT,      0 },
            //{ ITEM_ID_DEL,          DIK_DELETE,         VK_DELETE,      0 },
            //{ ITEM_ID_LWIN,         DIK_LWIN,           VK_LWIN,        0 },
            //{ ITEM_ID_RWIN,         DIK_RWIN,           VK_RWIN,        0 },
            //{ ITEM_ID_MENU,         DIK_APPS,           VK_APPS,        0 },
            //{ ITEM_ID_PAUSE,        DIK_PAUSE,          VK_PAUSE,       0 },
            //{ ITEM_ID_CANCEL,       0,                  VK_CANCEL,      0 },

            // New keys introduced in Windows 2000. These have no MAME codes to
            // preserve compatibility with old config files that may refer to them
            // as e.g. FORWARD instead of e.g. KEYCODE_WEBFORWARD. They need table
            // entries anyway because otherwise they aren't recognized when
            // GetAsyncKeyState polling is used (as happens currently when MAME is
            // paused). Some codes are missing because the mapping to vkey codes
            // isn't clear, and MapVirtualKey is no help.

            //{ ITEM_ID_OTHER_SWITCH, DIK_MUTE,           VK_VOLUME_MUTE,         0 },
            //{ ITEM_ID_OTHER_SWITCH, DIK_VOLUMEDOWN,     VK_VOLUME_DOWN,         0 },
            //{ ITEM_ID_OTHER_SWITCH, DIK_VOLUMEUP,       VK_VOLUME_UP,           0 },
            //{ ITEM_ID_OTHER_SWITCH, DIK_WEBHOME,        VK_BROWSER_HOME,        0 },
            //{ ITEM_ID_OTHER_SWITCH, DIK_WEBSEARCH,      VK_BROWSER_SEARCH,      0 },
            //{ ITEM_ID_OTHER_SWITCH, DIK_WEBFAVORITES,   VK_BROWSER_FAVORITES,   0 },
            //{ ITEM_ID_OTHER_SWITCH, DIK_WEBREFRESH,     VK_BROWSER_REFRESH,     0 },
            //{ ITEM_ID_OTHER_SWITCH, DIK_WEBSTOP,        VK_BROWSER_STOP,        0 },
            //{ ITEM_ID_OTHER_SWITCH, DIK_WEBFORWARD,     VK_BROWSER_FORWARD,     0 },
            //{ ITEM_ID_OTHER_SWITCH, DIK_WEBBACK,        VK_BROWSER_BACK,        0 },
            //{ ITEM_ID_OTHER_SWITCH, DIK_MAIL,           VK_LAUNCH_MAIL,         0 },
            //{ ITEM_ID_OTHER_SWITCH, DIK_MEDIASELECT,    VK_LAUNCH_MEDIA_SELECT, 0 },
        };


        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (mame.mame_machine_manager.instance() == null || mame.mame_machine_manager.instance().osd() == null)
                return;


            //for (int i = 0; i < 256; i++)
            //{
            //    if (Convert.ToBoolean(GetAsyncKeyState(i)))
            //        Console.WriteLine("---------- '{0}' '{1}'", i, (Keys)i);
            //}


            Keys keycode = e.KeyCode;

            if (e.KeyCode == Keys.ShiftKey)
            {
                if (Convert.ToBoolean(GetAsyncKeyState((int)Keys.LShiftKey)))
                    keycode = Keys.LShiftKey;

                if (Convert.ToBoolean(GetAsyncKeyState((int)Keys.RShiftKey)))
                    keycode = Keys.RShiftKey;
            }

            if (e.KeyCode == Keys.Menu)
            {
                if (Convert.ToBoolean(GetAsyncKeyState((int)Keys.LMenu)))
                    keycode = Keys.LMenu;

                if (Convert.ToBoolean(GetAsyncKeyState((int)Keys.RMenu)))
                    keycode = Keys.RMenu;
            }

            if (e.KeyCode == Keys.ControlKey)
            {
                if (Convert.ToBoolean(GetAsyncKeyState((int)Keys.LControlKey)))
                    keycode = Keys.LControlKey;

                if (Convert.ToBoolean(GetAsyncKeyState((int)Keys.RControlKey)))
                    keycode = Keys.RControlKey;
            }

            Console.WriteLine("Form1_KeyDown() - '{0}' - '{1}' - '{2}' - '{3}'", e.KeyCode, e.KeyData, e.KeyValue, keycode);

            osd_interface_WinForms osd = (osd_interface_WinForms)mame.mame_machine_manager.instance().osd();

            if (osd.keyboard_state == null)
                return;

            mame.input_item_id input_id;
            if (keymap.TryGetValue(keycode, out input_id))
                osd.keyboard_state[(int)input_id].i = 1;


            // from Windows.Forms.Keys
            //  O = 79,
            //  K = 75,
#if false
            if      (key >= 65 && key <= 90) keyboard_state[ITEM_ID_A + (key - 65)] = 1;
            else if (key >= 48 && key <= 57) keyboard_state[ITEM_ID_0 + (key - 48)] = 1;
            else if (key == 13)              keyboard_state[ITEM_ID_ENTER] = 1;
            else if (key == 27)              keyboard_state[ITEM_ID_ESC] = 1;
            else if (key == 32)              keyboard_state[ITEM_ID_SPACE] = 1;
            else if (key == 37)              keyboard_state[ITEM_ID_LEFT] = 1;
            else if (key == 38)              keyboard_state[ITEM_ID_UP] = 1;
            else if (key == 39)              keyboard_state[ITEM_ID_RIGHT] = 1;
            else if (key == 40)              keyboard_state[ITEM_ID_DOWN] = 1;
#endif
        }


        void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (mame.mame_machine_manager.instance() == null || mame.mame_machine_manager.instance().osd() == null)
                return;

            osd_interface_WinForms osd = (osd_interface_WinForms)mame.mame_machine_manager.instance().osd();

            if (osd.keyboard_state == null)
                return;

            mame.input_item_id input_id;
            if (keymap.TryGetValue(e.KeyCode, out input_id))
                osd.keyboard_state[(int)input_id].i = 0;

            if (e.KeyCode == Keys.ShiftKey)
            {
                if (keymap.TryGetValue(Keys.LShiftKey, out input_id))
                    osd.keyboard_state[(int)input_id].i = 0;
                if (keymap.TryGetValue(Keys.RShiftKey, out input_id))
                    osd.keyboard_state[(int)input_id].i = 0;
            }

            if (e.KeyCode == Keys.Menu)
            {
                if (keymap.TryGetValue(Keys.LMenu, out input_id))
                    osd.keyboard_state[(int)input_id].i = 0;
                if (keymap.TryGetValue(Keys.RMenu, out input_id))
                    osd.keyboard_state[(int)input_id].i = 0;
            }

            if (e.KeyCode == Keys.ControlKey)
            {
                if (keymap.TryGetValue(Keys.LControlKey, out input_id))
                    osd.keyboard_state[(int)input_id].i = 0;
                if (keymap.TryGetValue(Keys.RControlKey, out input_id))
                    osd.keyboard_state[(int)input_id].i = 0;
            }
        }


        void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            int mouseX = e.X;
            int mouseY = e.Y;

            //float dpiX = m_graphics.DpiX;
            //float dpiY = m_graphics.DpiY;
            //float scaleX = dpiX / 96.0f;
            //float scaleY = dpiY / 96.0f;
            float dpiX = 96;
            float scaleX = 1;
            float scaleY = 1;

            mouseX -= m_bitmapXOffset;
            mouseY -= m_bitmapYOffset;

            mouseX = (int)(mouseX / scaleX);
            mouseY = (int)(mouseY / scaleY);


            if (mame.mame_machine_manager.instance() == null || mame.mame_machine_manager.instance().osd() == null)
                return;

            osd_interface_WinForms osd = (osd_interface_WinForms)mame.mame_machine_manager.instance().osd();

            if (osd.mouse_axis_state == null)
                return;

            Console.WriteLine("Form1_MouseMove() - '{0}' - '{1}' - '{2}' - '{3}'", mouseX, mouseY, dpiX, scaleX);

            osd.mouse_axis_state[0].i = mouseX;
            osd.mouse_axis_state[1].i = mouseY;

            osd.ui_input_push_mouse_move_event(mouseX, mouseY);
        }


        void Form1_MouseLeave(object sender, EventArgs e)
        {
            if (mame.mame_machine_manager.instance() == null || mame.mame_machine_manager.instance().osd() == null)
                return;

            osd_interface_WinForms osd = (osd_interface_WinForms)mame.mame_machine_manager.instance().osd();

            if (osd.mouse_axis_state == null)
                return;

            Console.WriteLine("Form1_MouseLeave()");

            osd.ui_input_push_mouse_leave_event();
        }


        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            int mouseX = e.X;
            int mouseY = e.Y;

            int button = -1;
            switch (e.Button)
            {
                case MouseButtons.Left:     button = 0; break;
                case MouseButtons.Right:    button = 1; break;
                case MouseButtons.Middle:   button = 2; break;
                case MouseButtons.XButton1: button = 3; break;
                case MouseButtons.XButton2: button = 4; break;
                case MouseButtons.None: default: break;
            }

            if (mame.mame_machine_manager.instance() == null || mame.mame_machine_manager.instance().osd() == null)
                return;

            osd_interface_WinForms osd = (osd_interface_WinForms)mame.mame_machine_manager.instance().osd();

            if (osd.mouse_button_state == null)
                return;

            Console.WriteLine("Form1_MouseDown() - '{0}' - '{1}'", e.Button, button);

            if (button != -1)
            {
                osd.mouse_button_state[button].i = 1;

                if (button == 0)
                    osd.ui_input_push_mouse_down_event(mouseX, mouseY);
            }
        }


        void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            int mouseX = e.X;
            int mouseY = e.Y;

            int button = -1;
            switch (e.Button)
            {
                case MouseButtons.Left:     button = 0; break;
                case MouseButtons.Right:    button = 1; break;
                case MouseButtons.Middle:   button = 2; break;
                case MouseButtons.XButton1: button = 3; break;
                case MouseButtons.XButton2: button = 4; break;
                case MouseButtons.None: default: break;
            }

            if (mame.mame_machine_manager.instance() == null || mame.mame_machine_manager.instance().osd() == null)
                return;

            osd_interface_WinForms osd = (osd_interface_WinForms)mame.mame_machine_manager.instance().osd();

            if (osd.mouse_button_state == null)
                return;

            Console.WriteLine("Form1_MouseUp() - '{0}' - '{1}'", e.Button, button);

            if (button != -1)
            {
                osd.mouse_button_state[button].i = 0;

                if (button == 0)
                    osd.ui_input_push_mouse_up_event(mouseX, mouseY);
            }
        }


        void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int mouseX = e.X;
            int mouseY = e.Y;

            int button = -1;
            switch (e.Button)
            {
                case MouseButtons.Left:     button = 0; break;
                case MouseButtons.Right:    button = 1; break;
                case MouseButtons.Middle:   button = 2; break;
                case MouseButtons.XButton1: button = 3; break;
                case MouseButtons.XButton2: button = 4; break;
                case MouseButtons.None: default: break;
            }

            if (mame.mame_machine_manager.instance() == null || mame.mame_machine_manager.instance().osd() == null)
                return;

            osd_interface_WinForms osd = (osd_interface_WinForms)mame.mame_machine_manager.instance().osd();

            if (osd.mouse_button_state == null)
                return;

            Console.WriteLine("Form1_MouseDoubleClick() - '{0}' - '{1}'", e.Button, button);

            if (button != -1)
            {
                osd.mouse_button_state[button].i = 0;

                if (button == 0)
                    osd.ui_input_push_mouse_double_click_event(mouseX, mouseY);
            }
        }


        void updateTimer_Tick(object sender, EventArgs e)
        {
            if (m_updateCount++ % 100 == 0)
                Console.WriteLine("updateTimer_Tick() - {0}", m_updateCount);


            if (mame.mame_machine_manager.instance() == null)
                return;  // mame's not ready to update yet


            osd_interface_WinForms osd = (osd_interface_WinForms)mame.mame_machine_manager.instance().osd();
            mame.Pointer<byte> framedata = osd.screenbufferptr;
            ConcurrentDictionary<int, int> frameDataCopyThreadIds = new ConcurrentDictionary<int, int>();

            if (framedata == null)
                return;

#if true
            lock (osd.osdlock)
            {
                //if (Width != m_bitmap.Width || Height != m_bitmap.Height ||
                //    Width != osd.get_width() || Height != osd.get_height())
                //{
                //    m_bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                //    m_bitmapHelper = new BitmapHelper(m_bitmap);
                //
                //    osd.set_bounds(Width, Height);
                //
                //    framedata = osd.screenbufferptr;
                //}

                // right now the source and target bitmaps are different sizes, so detect resizes differently for each

                if (Width - (m_bitmapXOffset * 2) != m_bitmap.Width || Height - (m_bitmapYOffset * 2) != m_bitmap.Height)
                {
                    m_bitmap = new Bitmap(Width - (m_bitmapXOffset * 2), Height - (m_bitmapYOffset * 2), System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    m_bitmapHelper = new BitmapHelper(m_bitmap);

                    m_graphics = CreateGraphics();
                }

                if (osd.get_width() != 600 || osd.get_height() != 600)
                {
                    osd.set_bounds(600, 600);

                    framedata = osd.screenbufferptr;
                }

                m_bitmapHelper.Lock();

#if false
                {
                    // random dots to make sure things are working, and not frozen
                    int stride = osd.get_width();
                    int x = 200;
                    int y = 100;
                    Random r = new System.Random();
                    if (x + 10 < osd.get_width() && y + 60 < osd.get_height())
                    {
                        for (int i = 0; i < 10; i++)
                            framedata.set_uint32(x + (y * stride) + i, (UInt32)(r.Next() % 16000000));

                        for (int i = 0; i < 10; i++)
                            framedata.set_uint32(x + ((y + 10) * stride) + i, mame.rgb_t.white());

#if false
                        for (int i = 0; i < 10; i++)
                            framedata.set_uint32(x + ((y + 20) * stride) + i, mame.rgb_t.red);

                        for (int i = 0; i < 10; i++)
                            framedata.set_uint32(x + ((y + 30) * stride) + i, mame.rgb_t.green);

                        for (int i = 0; i < 10; i++)
                            framedata.set_uint32(x + ((y + 40) * stride) + i, mame.rgb_t.blue);

                        for (int i = 0; i < 10; i++)
                            framedata.set_uint32(x + ((y + 50) * stride) + i, mame.rgb_t.pink);

                        for (int i = 0; i < 10; i++)
                            framedata.set_uint32(x + ((y + 60) * stride) + i, mame.rgb_t.greenyellow);
#endif
                    }
                }
#endif

                {
                    // copy whole line at once
                    int sourceWidth = osd.get_width();
                    int sourceHeight = osd.get_height();
                    int sourceBytesPerPix = 4;  // TODO query this
                    int sourceStride = sourceWidth * sourceBytesPerPix;
                    int destWidth = m_bitmapHelper.Width;
                    int destHeight = m_bitmapHelper.Height;
                    int destBytesPerPix = 4;  // TODO fix funcs in BitmapHelper and query this
                    int destStride = destWidth * destBytesPerPix;
                    
                    // if dest is smaller than source, clamp
                    sourceHeight = Math.Min(sourceHeight, destHeight);
                    int copyLength = Math.Min(sourceStride, destStride);

                    System.Threading.Tasks.Parallel.For(0, sourceHeight, y =>
                    //for (int y = 0; y < sourceHeight; y++)
                    {
                        frameDataCopyThreadIds.TryAdd(System.Threading.Thread.CurrentThread.ManagedThreadId, 0);  // count how many threads are in use when using Parallel.For() version
                        framedata.CopyTo(y * sourceStride, m_bitmapHelper.GetBitmapData(y * destStride, copyLength), copyLength);
                    });
                }

#if false
                // copy pixel by pixel

                BitmapHelper.SetPixelRawFunc setPixelFunc = m_bitmapHelper.IsAlphaBitmap ? m_bitmapHelper.SetPixelRawAlpha : (BitmapHelper.SetPixelRawFunc)m_bitmapHelper.SetPixelRawNoAlpha;
                int destPixelSize = m_bitmapHelper.GetRawPixelSize();
                const int scale = 1;
                for (int y = 0; y < osd.get_height(); y++)
                {
                    int sourceArrayIndex = y * osd.get_height();

                    for (int j = 0; j < scale; j++)
                    {
                        int destArrayIndex = m_bitmapHelper.GetRawYIndex((y * scale) + j);

                        for (int x = 0; x < osd.get_width(); x++)
                        {
                            UInt32 color = framedata.get_uint32(sourceArrayIndex + x);

                            for (int i = 0; i < scale; i++)
                            {
                                setPixelFunc(destArrayIndex, color);
                                destArrayIndex += destPixelSize;
                            }
                        }
                    }
                }
#endif

                m_bitmapHelper.Unlock(true);
            }
#endif


            if (m_graphics != null)
            {
                m_graphics.DrawImageUnscaled(m_bitmap, m_bitmapXOffset, m_bitmapYOffset);
                //m_graphics.DrawImage(m_bitmap, m_bitmapXOffset, m_bitmapYOffset, Width - (m_bitmapXOffset * 3), Height - (m_bitmapYOffset * 3));

                if (mame.mame_machine_manager.instance().machine() == null || mame.mame_machine_manager.instance().machine().phase() != mame.machine_phase.RUNNING)
                    return;

                var video = mame.mame_machine_manager.instance().machine().video();
                osd_ticks_t tps = mame.osdcore_global.m_osdcore.osd_ticks_per_second();
                double final_real_time = tps == 0 ? 0 : (double)video.m_overall_real_seconds + (double)video.m_overall_real_ticks / (double)tps;
                double final_emu_time = video.m_overall_emutime.as_double();
                double average_speed_percentage = final_real_time == 0 ? 0 : 100 * final_emu_time / final_real_time;
                string total_time = (video.m_overall_emutime + new mame.attotime(0, mame.attotime.ATTOSECONDS_PER_SECOND / 2)).as_string(2);
                this.Text = string.Format("Emulator running... Avg Speed: {0:f2}% ({1} seconds) Framedata_Copy_Threads: {2} - speed_text: {3}", average_speed_percentage, total_time, frameDataCopyThreadIds.Count, video.speed_text());
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            //graphics = e.Graphics;
            //e.Graphics.DrawString("Emulator running...", this.Font, System.Drawing.Brushes.Gray, new System.Drawing.PointF(0, 0));
            //e.Graphics.DrawImage(bitmap, 15, 15);
        }
    }
}
