// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int16_t = System.Int16;


namespace mame
{
    public class osd_options : emu_options
    {
        const string OSDOPTION_UIMODEKEY            = "uimodekey";

        public const string OSDCOMMAND_LIST_MIDI_DEVICES = "listmidi";
        public const string OSDCOMMAND_LIST_NETWORK_ADAPTERS = "listnetwork";

        public const string OSDOPTION_DEBUGGER      = "debugger";
        const string OSDOPTION_DEBUGGER_PORT        = "debugger_port";
        const string OSDOPTION_DEBUGGER_FONT        = "debugger_font";
        const string OSDOPTION_DEBUGGER_FONT_SIZE   = "debugger_font_size";
        const string OSDOPTION_WATCHDOG             = "watchdog";

        public const string OSDOPTION_NUMPROCESSORS = "numprocessors";
        const string OSDOPTION_BENCH                = "bench";

        public const string OSDOPTION_VIDEO         = "video";
        const string OSDOPTION_NUMSCREENS           = "numscreens";
        const string OSDOPTION_WINDOW               = "window";
        const string OSDOPTION_MAXIMIZE             = "maximize";
        const string OSDOPTION_WAITVSYNC            = "waitvsync";
        const string OSDOPTION_SYNCREFRESH          = "syncrefresh";

        const string OSDOPTION_SCREEN               = "screen";
        const string OSDOPTION_ASPECT               = "aspect";
        const string OSDOPTION_RESOLUTION           = "resolution";
        const string OSDOPTION_VIEW                 = "view";

        const string OSDOPTION_SWITCHRES            = "switchres";

        const string OSDOPTION_FILTER               = "filter";
        const string OSDOPTION_PRESCALE             = "prescale";

        const string OSDOPTION_SHADER_MAME          = "glsl_shader_mame";
        const string OSDOPTION_SHADER_SCREEN        = "glsl_shader_screen";
        const string OSDOPTION_GLSL_FILTER          = "gl_glsl_filter";
        const string OSDOPTION_GL_GLSL              = "gl_glsl";
        const string OSDOPTION_GL_PBO               = "gl_pbo";
        const string OSDOPTION_GL_VBO               = "gl_vbo";
        const string OSDOPTION_GL_NOTEXTURERECT     = "gl_notexturerect";
        const string OSDOPTION_GL_FORCEPOW2TEXTURE  = "gl_forcepow2texture";

        public const string OSDOPTION_SOUND         = "sound";
        const string OSDOPTION_AUDIO_LATENCY        = "audio_latency";

        const string OSDOPTION_PA_API               = "pa_api";
        const string OSDOPTION_PA_DEVICE            = "pa_device";
        const string OSDOPTION_PA_LATENCY           = "pa_latency";

        const string OSDOPTION_AUDIO_OUTPUT         = "audio_output";
        const string OSDOPTION_AUDIO_EFFECT         = "audio_effect";

        protected const string OSDOPTVAL_AUTO       = "auto";
        public const string OSDOPTVAL_NONE          = "none";

        const string OSDOPTION_BGFX_PATH            = "bgfx_path";
        const string OSDOPTION_BGFX_BACKEND         = "bgfx_backend";
        const string OSDOPTION_BGFX_DEBUG           = "bgfx_debug";
        const string OSDOPTION_BGFX_SCREEN_CHAINS   = "bgfx_screen_chains";
        const string OSDOPTION_BGFX_SHADOW_MASK     = "bgfx_shadow_mask";
        const string OSDOPTION_BGFX_LUT             = "bgfx_lut";
        const string OSDOPTION_BGFX_AVI_NAME        = "bgfx_avi_name";


        static readonly options_entry [] s_option_entries = new options_entry []
        {
            new options_entry(null,                                   null,              g.OPTION_HEADER,    "OSD KEYBOARD MAPPING OPTIONS"),
#if false//#if defined(SDLMAME_MACOSX) || defined(OSD_MAC)
            new options_entry(OSDOPTION_UIMODEKEY,                    "DEL",             g.OPTION_STRING,    "key to enable/disable MAME controls when emulated system has keyboard inputs"),
#else
            new options_entry(OSDOPTION_UIMODEKEY,                    "SCRLOCK",         g.OPTION_STRING,    "key to enable/disable MAME controls when emulated system has keyboard inputs"),
#endif  // SDLMAME_MACOSX

            new options_entry(null,                                   null,              g.OPTION_HEADER,    "OSD FONT OPTIONS"),
            new options_entry(font_module.OSD_FONT_PROVIDER,          OSDOPTVAL_AUTO,    g.OPTION_STRING,    "provider for UI font: "),

            new options_entry(null,                                   null,              g.OPTION_HEADER,    "OSD OUTPUT OPTIONS"),
            new options_entry(output_module.OSD_OUTPUT_PROVIDER,      OSDOPTVAL_AUTO,    g.OPTION_STRING,    "provider for output notifications: "),

            new options_entry(null,                                   null,              g.OPTION_HEADER,    "OSD INPUT OPTIONS"),
            new options_entry(input_module.OSD_KEYBOARDINPUT_PROVIDER, OSDOPTVAL_AUTO,   g.OPTION_STRING,    "provider for keyboard input: "),
            new options_entry(input_module.OSD_MOUSEINPUT_PROVIDER,   OSDOPTVAL_AUTO,    g.OPTION_STRING,    "provider for mouse input: "),
            new options_entry(input_module.OSD_LIGHTGUNINPUT_PROVIDER, OSDOPTVAL_AUTO,   g.OPTION_STRING,    "provider for lightgun input: "),
            new options_entry(input_module.OSD_JOYSTICKINPUT_PROVIDER, OSDOPTVAL_AUTO,   g.OPTION_STRING,    "provider for joystick input: "),

            new options_entry(null,                                   null,              g.OPTION_HEADER,    "OSD CLI OPTIONS"),
            new options_entry(OSDCOMMAND_LIST_MIDI_DEVICES + ";mlist", "0",              g.OPTION_COMMAND,   "list available MIDI I/O devices"),
            new options_entry(OSDCOMMAND_LIST_NETWORK_ADAPTERS + ";nlist", "0",          g.OPTION_COMMAND,   "list available network adapters"),

            new options_entry(null,                                   null,             g.OPTION_HEADER,     "OSD DEBUGGING OPTIONS"),
            new options_entry(OSDOPTION_DEBUGGER,                     OSDOPTVAL_AUTO,   g.OPTION_STRING,     "debugger used : "),
            new options_entry(OSDOPTION_DEBUGGER_PORT,                "23946",          g.OPTION_INTEGER,    "port to use for gdbstub debugger"),
            new options_entry(OSDOPTION_DEBUGGER_FONT + ";dfont",     OSDOPTVAL_AUTO,   g.OPTION_STRING,     "font to use for debugger views"),
            new options_entry(OSDOPTION_DEBUGGER_FONT_SIZE + ";dfontsize", "0",         g.OPTION_FLOAT,      "font size to use for debugger views"),
            new options_entry(OSDOPTION_WATCHDOG + ";wdog",           "0",              g.OPTION_INTEGER,    "force the program to terminate if no updates within specified number of seconds"),

            new options_entry(null,                                   null,             g.OPTION_HEADER,     "OSD PERFORMANCE OPTIONS"),
            new options_entry(OSDOPTION_NUMPROCESSORS + ";np",        OSDOPTVAL_AUTO,   g.OPTION_STRING,     "number of processors; this overrides the number the system reports"),
            new options_entry(OSDOPTION_BENCH,                        "0",              g.OPTION_INTEGER,    "benchmark for the given number of emulated seconds; implies -video none -sound none -nothrottle"),

            new options_entry(null,                                   null,             g.OPTION_HEADER,     "OSD VIDEO OPTIONS"),
            // OS X can be trusted to have working hardware OpenGL, so default to it on for the best user experience
            new options_entry(OSDOPTION_VIDEO,                        OSDOPTVAL_AUTO,   g.OPTION_STRING,     "video output method: "),
            new options_entry(OSDOPTION_NUMSCREENS + "(1-4)",         "1",              g.OPTION_INTEGER,    "number of output screens/windows to create; usually, you want just one"),
            new options_entry(OSDOPTION_WINDOW + ";w",                "0",              g.OPTION_BOOLEAN,    "enable window mode; otherwise, full screen mode is assumed"),
            new options_entry(OSDOPTION_MAXIMIZE + ";max",            "1",              g.OPTION_BOOLEAN,    "default to maximized windows"),
            new options_entry(OSDOPTION_WAITVSYNC + ";vs",            "0",              g.OPTION_BOOLEAN,    "enable waiting for the start of VBLANK before flipping screens (reduces tearing effects)"),
            new options_entry(OSDOPTION_SYNCREFRESH + ";srf",         "0",              g.OPTION_BOOLEAN,    "enable using the start of VBLANK for throttling instead of the game time"),
            new options_entry(monitor_module.OSD_MONITOR_PROVIDER,    OSDOPTVAL_AUTO,   g.OPTION_STRING,     "monitor discovery method: "),

            // per-window options
            new options_entry(null,                                   null,             g.OPTION_HEADER,    "OSD PER-WINDOW VIDEO OPTIONS"),
            new options_entry(OSDOPTION_SCREEN,                       OSDOPTVAL_AUTO,   g.OPTION_STRING,    "explicit name of the first screen; 'auto' here will try to make a best guess"),
            new options_entry(OSDOPTION_ASPECT + ";screen_aspect",    OSDOPTVAL_AUTO,   g.OPTION_STRING,    "aspect ratio for all screens; 'auto' here will try to make a best guess"),
            new options_entry(OSDOPTION_RESOLUTION + ";r",            OSDOPTVAL_AUTO,   g.OPTION_STRING,    "preferred resolution for all screens; format is <width>x<height>[@<refreshrate>] or 'auto'"),
            new options_entry(OSDOPTION_VIEW,                         OSDOPTVAL_AUTO,   g.OPTION_STRING,    "preferred view for all screens"),

            new options_entry(OSDOPTION_SCREEN + "0",                 OSDOPTVAL_AUTO,   g.OPTION_STRING,    "explicit name of the first screen; 'auto' here will try to make a best guess"),
            new options_entry(OSDOPTION_ASPECT + "0",                 OSDOPTVAL_AUTO,   g.OPTION_STRING,    "aspect ratio of the first screen; 'auto' here will try to make a best guess"),
            new options_entry(OSDOPTION_RESOLUTION + "0;r0",          OSDOPTVAL_AUTO,   g.OPTION_STRING,    "preferred resolution of the first screen; format is <width>x<height>[@<refreshrate>] or 'auto'"),
            new options_entry(OSDOPTION_VIEW + "0",                   OSDOPTVAL_AUTO,   g.OPTION_STRING,    "preferred view for the first screen"),

            new options_entry(OSDOPTION_SCREEN + "1",                 OSDOPTVAL_AUTO,   g.OPTION_STRING,    "explicit name of the second screen; 'auto' here will try to make a best guess"),
            new options_entry(OSDOPTION_ASPECT + "1",                 OSDOPTVAL_AUTO,   g.OPTION_STRING,    "aspect ratio of the second screen; 'auto' here will try to make a best guess"),
            new options_entry(OSDOPTION_RESOLUTION + "1;r1",          OSDOPTVAL_AUTO,   g.OPTION_STRING,    "preferred resolution of the second screen; format is <width>x<height>[@<refreshrate>] or 'auto'"),
            new options_entry(OSDOPTION_VIEW + "1",                   OSDOPTVAL_AUTO,   g.OPTION_STRING,    "preferred view for the second screen"),

            new options_entry(OSDOPTION_SCREEN + "2",                 OSDOPTVAL_AUTO,   g.OPTION_STRING,    "explicit name of the third screen; 'auto' here will try to make a best guess"),
            new options_entry(OSDOPTION_ASPECT + "2",                 OSDOPTVAL_AUTO,   g.OPTION_STRING,    "aspect ratio of the third screen; 'auto' here will try to make a best guess"),
            new options_entry(OSDOPTION_RESOLUTION + "2;r2",          OSDOPTVAL_AUTO,   g.OPTION_STRING,    "preferred resolution of the third screen; format is <width>x<height>[@<refreshrate>] or 'auto'"),
            new options_entry(OSDOPTION_VIEW + "2",                   OSDOPTVAL_AUTO,   g.OPTION_STRING,    "preferred view for the third screen"),

            new options_entry(OSDOPTION_SCREEN + "3",                 OSDOPTVAL_AUTO,   g.OPTION_STRING,    "explicit name of the fourth screen; 'auto' here will try to make a best guess"),
            new options_entry(OSDOPTION_ASPECT + "3",                 OSDOPTVAL_AUTO,   g.OPTION_STRING,    "aspect ratio of the fourth screen; 'auto' here will try to make a best guess"),
            new options_entry(OSDOPTION_RESOLUTION + "3;r3",          OSDOPTVAL_AUTO,   g.OPTION_STRING,    "preferred resolution of the fourth screen; format is <width>x<height>[@<refreshrate>] or 'auto'"),
            new options_entry(OSDOPTION_VIEW + "3",                   OSDOPTVAL_AUTO,   g.OPTION_STRING,    "preferred view for the fourth screen"),

            // full screen options
            new options_entry(null,                                   null,             g.OPTION_HEADER,    "OSD FULL SCREEN OPTIONS"),
            new options_entry(OSDOPTION_SWITCHRES,                    "0",              g.OPTION_BOOLEAN,   "enable resolution switching"),

            new options_entry(null,                                   null,             g.OPTION_HEADER,    "OSD ACCELERATED VIDEO OPTIONS"),
            new options_entry(OSDOPTION_FILTER + ";glfilter;flt",     "1",              g.OPTION_BOOLEAN,   "use bilinear filtering when scaling emulated video"),
            new options_entry(OSDOPTION_PRESCALE + "(1-8)",           "1",              g.OPTION_INTEGER,   "scale emulated video by this factor before applying filters/shaders"),

#if USE_OPENGL
            new options_entry(null,                                   null,             OPTION_HEADER,    "OpenGL-SPECIFIC OPTIONS"),
            new options_entry(OSDOPTION_GL_FORCEPOW2TEXTURE,          "0",              OPTION_BOOLEAN,   "force power-of-two texture sizes (default no)"),
            new options_entry(OSDOPTION_GL_NOTEXTURERECT,             "0",              OPTION_BOOLEAN,   "don't use OpenGL GL_ARB_texture_rectangle (default on)"),
            new options_entry(OSDOPTION_GL_VBO,                       "1",              OPTION_BOOLEAN,   "enable OpenGL VBO if available (default on)"),
            new options_entry(OSDOPTION_GL_PBO,                       "1",              OPTION_BOOLEAN,   "enable OpenGL PBO if available (default on)"),
            new options_entry(OSDOPTION_GL_GLSL,                      "0",              OPTION_BOOLEAN,   "enable OpenGL GLSL if available (default off)"),
            { OSDOPTION_GLSL_FILTER,                  "1",              OPTION_STRING,    "enable OpenGL GLSL filtering instead of FF filtering 0-plain, 1-bilinear (default), 2-bicubic" },
            new options_entry(OSDOPTION_SHADER_MAME + "0",            OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader set mame bitmap 0"),
            new options_entry(OSDOPTION_SHADER_MAME + "1",            OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader set mame bitmap 1"),
            new options_entry(OSDOPTION_SHADER_MAME + "2",            OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader set mame bitmap 2"),
            new options_entry(OSDOPTION_SHADER_MAME + "3",            OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader set mame bitmap 3"),
            new options_entry(OSDOPTION_SHADER_MAME + "4",            OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader set mame bitmap 4"),
            new options_entry(OSDOPTION_SHADER_MAME + "5",            OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader set mame bitmap 5"),
            new options_entry(OSDOPTION_SHADER_MAME + "6",            OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader set mame bitmap 6"),
            new options_entry(OSDOPTION_SHADER_MAME + "7",            OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader set mame bitmap 7"),
            new options_entry(OSDOPTION_SHADER_MAME + "8",            OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader set mame bitmap 8"),
            new options_entry(OSDOPTION_SHADER_MAME + "9",            OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader set mame bitmap 9"),
            new options_entry(OSDOPTION_SHADER_SCREEN + "0",          OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader screen bitmap 0"),
            new options_entry(OSDOPTION_SHADER_SCREEN + "1",          OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader screen bitmap 1"),
            new options_entry(OSDOPTION_SHADER_SCREEN + "2",          OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader screen bitmap 2"),
            new options_entry(OSDOPTION_SHADER_SCREEN + "3",          OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader screen bitmap 3"),
            new options_entry(OSDOPTION_SHADER_SCREEN + "4",          OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader screen bitmap 4"),
            new options_entry(OSDOPTION_SHADER_SCREEN + "5",          OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader screen bitmap 5"),
            new options_entry(OSDOPTION_SHADER_SCREEN + "6",          OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader screen bitmap 6"),
            new options_entry(OSDOPTION_SHADER_SCREEN + "7",          OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader screen bitmap 7"),
            new options_entry(OSDOPTION_SHADER_SCREEN + "8",          OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader screen bitmap 8"),
            new options_entry(OSDOPTION_SHADER_SCREEN + "9",          OSDOPTVAL_NONE,   OPTION_STRING,    "custom OpenGL GLSL shader screen bitmap 9"),
#endif

            new options_entry(null,                                   null,             g.OPTION_HEADER,    "OSD SOUND OPTIONS"),
            new options_entry(OSDOPTION_SOUND,                        OSDOPTVAL_AUTO,   g.OPTION_STRING,    "sound output method: "),
            new options_entry(OSDOPTION_AUDIO_LATENCY + "(0-5)",      "2",              g.OPTION_INTEGER,   "set audio latency (increase to reduce glitches, decrease for responsiveness)"),

#if NO_USE_PORTAUDIO
            { nullptr,                                nullptr,          OPTION_HEADER,    "PORTAUDIO OPTIONS" },
            { OSDOPTION_PA_API,                       OSDOPTVAL_NONE,   OPTION_STRING,    "PortAudio API" },
            { OSDOPTION_PA_DEVICE,                    OSDOPTVAL_NONE,   OPTION_STRING,    "PortAudio device" },
            { OSDOPTION_PA_LATENCY "(0-0.25)",        "0",              OPTION_FLOAT,     "suggested latency in seconds, 0 for default" },
#endif

#if SDLMAME_MACOSX
            new options_entry(null,                                   null,             OPTION_HEADER,    "CoreAudio-SPECIFIC OPTIONS"),
            new options_entry(OSDOPTION_AUDIO_OUTPUT,                 OSDOPTVAL_AUTO,   OPTION_STRING,    "audio output device"),
            new options_entry(OSDOPTION_AUDIO_EFFECT + "0",           OSDOPTVAL_NONE,   OPTION_STRING,    "AudioUnit effect 0"),
            new options_entry(OSDOPTION_AUDIO_EFFECT + "1",           OSDOPTVAL_NONE,   OPTION_STRING,    "AudioUnit effect 1"),
            new options_entry(OSDOPTION_AUDIO_EFFECT + "2",           OSDOPTVAL_NONE,   OPTION_STRING,    "AudioUnit effect 2"),
            new options_entry(OSDOPTION_AUDIO_EFFECT + "3",           OSDOPTVAL_NONE,   OPTION_STRING,    "AudioUnit effect 3"),
            new options_entry(OSDOPTION_AUDIO_EFFECT + "4",           OSDOPTVAL_NONE,   OPTION_STRING,    "AudioUnit effect 4"),
            new options_entry(OSDOPTION_AUDIO_EFFECT + "5",           OSDOPTVAL_NONE,   OPTION_STRING,    "AudioUnit effect 5"),
            new options_entry(OSDOPTION_AUDIO_EFFECT + "6",           OSDOPTVAL_NONE,   OPTION_STRING,    "AudioUnit effect 6"),
            new options_entry(OSDOPTION_AUDIO_EFFECT + "7",           OSDOPTVAL_NONE,   OPTION_STRING,    "AudioUnit effect 7"),
            new options_entry(OSDOPTION_AUDIO_EFFECT + "8",           OSDOPTVAL_NONE,   OPTION_STRING,    "AudioUnit effect 8"),
            new options_entry(OSDOPTION_AUDIO_EFFECT + "9",           OSDOPTVAL_NONE,   OPTION_STRING,    "AudioUnit effect 9"),
#endif

            new options_entry(null,                                   null,              g.OPTION_HEADER,  "BGFX POST-PROCESSING OPTIONS"),
            new options_entry(OSDOPTION_BGFX_PATH,                    "bgfx",            g.OPTION_STRING,  "path to BGFX-related files"),
            new options_entry(OSDOPTION_BGFX_BACKEND,                 "auto",            g.OPTION_STRING,  "BGFX backend to use (d3d9, d3d11, d3d12, metal, opengl, gles, vulkan)"),
            new options_entry(OSDOPTION_BGFX_DEBUG,                   "0",               g.OPTION_BOOLEAN, "enable BGFX debugging statistics"),
            new options_entry(OSDOPTION_BGFX_SCREEN_CHAINS,           "default",         g.OPTION_STRING,  "comma-delimited list of screen chain JSON names, colon-delimited per-window"),
            new options_entry(OSDOPTION_BGFX_SHADOW_MASK,             "slot-mask.png",   g.OPTION_STRING,  "shadow mask texture name"),
            new options_entry(OSDOPTION_BGFX_LUT,                     "",                g.OPTION_STRING, "LUT texture name"),
            new options_entry(OSDOPTION_BGFX_AVI_NAME,                OSDOPTVAL_AUTO,    g.OPTION_STRING,  "filename for BGFX output logging"),

            // End of list
            new options_entry(null),
        };


        // construction/destruction
        public osd_options() : base()
        {
            add_entries(s_option_entries);
        }


        // keyboard mapping
        string ui_mode_key() { return value(OSDOPTION_UIMODEKEY); }


        // debugging options
        public string debugger() { return value(OSDOPTION_DEBUGGER); }
        //int debugger_port() const { return int_value(OSDOPTION_DEBUGGER_PORT); }
        //const char *debugger_font() const { return value(OSDOPTION_DEBUGGER_FONT); }
        //float debugger_font_size() const { return float_value(OSDOPTION_DEBUGGER_FONT_SIZE); }
        public int watchdog() { return int_value(OSDOPTION_WATCHDOG); }


        // performance options
        public string numprocessors() { return value(OSDOPTION_NUMPROCESSORS); }
        public int bench() { return int_value(OSDOPTION_BENCH); }


        // video options
        public string video() { return value(OSDOPTION_VIDEO); }
        //int numscreens() const { return int_value(OSDOPTION_NUMSCREENS); }
        //bool window() const { return bool_value(OSDOPTION_WINDOW); }
        //bool maximize() const { return bool_value(OSDOPTION_MAXIMIZE); }
        //bool wait_vsync() const { return bool_value(OSDOPTION_WAITVSYNC); }
        //bool sync_refresh() const { return bool_value(OSDOPTION_SYNCREFRESH); }


        // per-window options
        //const char *screen() const { return value(OSDOPTION_SCREEN); }
        //const char *aspect() const { return value(OSDOPTION_ASPECT); }
        //const char *resolution() const { return value(OSDOPTION_RESOLUTION); }
        //const char *view() const { return value(OSDOPTION_VIEW); }
        //const char *screen(int index) const { return value(string_format("%s%d", OSDOPTION_SCREEN, index).c_str()); }
        //const char *aspect(int index) const { return value(string_format("%s%d", OSDOPTION_ASPECT, index).c_str()); }
        //const char *resolution(int index) const { return value(string_format("%s%d", OSDOPTION_RESOLUTION, index).c_str()); }
        //const char *view(int index) const { return value(string_format("%s%d", OSDOPTION_VIEW, index).c_str()); }


        // full screen options
        //bool switch_res() const { return bool_value(OSDOPTION_SWITCHRES); }


        // accelerated video options
        //bool filter() const { return bool_value(OSDOPTION_FILTER); }
        //int prescale() const { return int_value(OSDOPTION_PRESCALE); }


        // OpenGL specific options
        //bool gl_force_pow2_texture() const { return bool_value(OSDOPTION_GL_FORCEPOW2TEXTURE); }
        //bool gl_no_texture_rect() const { return bool_value(OSDOPTION_GL_NOTEXTURERECT); }
        //bool gl_vbo() const { return bool_value(OSDOPTION_GL_VBO); }
        //bool gl_pbo() const { return bool_value(OSDOPTION_GL_PBO); }
        //bool gl_glsl() const { return bool_value(OSDOPTION_GL_GLSL); }
        //int glsl_filter() const { return int_value(OSDOPTION_GLSL_FILTER); }
        //const char *shader_mame(int index) const { return value(string_format("%s%d", OSDOPTION_SHADER_MAME, index).c_str()); }
        //const char *shader_screen(int index) const { return value(string_format("%s%d", OSDOPTION_SHADER_SCREEN, index).c_str()); }


        // sound options
        public string sound() { return value(OSDOPTION_SOUND); }
        public int audio_latency() { return int_value(OSDOPTION_AUDIO_LATENCY); }


        // CoreAudio specific options
        //const char *audio_output() const { return value(OSDOPTION_AUDIO_OUTPUT); }
        //const char *audio_effect(int index) const { return value(string_format("%s%d", OSDOPTION_AUDIO_EFFECT, index).c_str()); }


        // BGFX specific options
        //const char *bgfx_path() const { return value(OSDOPTION_BGFX_PATH); }
        //const char *bgfx_backend() const { return value(OSDOPTION_BGFX_BACKEND); }
        //bool bgfx_debug() const { return bool_value(OSDOPTION_BGFX_DEBUG); }
        //const char *bgfx_screen_chains() const { return value(OSDOPTION_BGFX_SCREEN_CHAINS); }
        //const char *bgfx_shadow_mask() const { return value(OSDOPTION_BGFX_SHADOW_MASK); }
        //const char *bgfx_lut() const { return value(OSDOPTION_BGFX_LUT); }
        //const char *bgfx_avi_name() const { return value(OSDOPTION_BGFX_AVI_NAME); }

        // PortAudio options
        //const char *pa_api() const { return value(OSDOPTION_PA_API); }
        //const char *pa_device() const { return value(OSDOPTION_PA_DEVICE); }
        //const float pa_latency() const { return float_value(OSDOPTION_PA_LATENCY); }
    }


    // ======================> osd_interface
    // description of the currently-running machine
    public abstract class osd_common_t : osd_output,
                                         osd_interface,
                                         IDisposable
    {
        // internal state
        running_machine m_machine;
        osd_options m_options;

        bool m_print_verbose;

        osd_module_manager m_mod_man = new osd_module_manager();
        font_module m_font_module;


        //static std::list<std::shared_ptr<osd_window>> s_window_list;


        sound_module m_sound;
        debug_module m_debugger;
        midi_module m_midi;
        input_module m_keyboard_input;
        input_module m_mouse_input;
        input_module m_lightgun_input;
        input_module m_joystick_input;
        output_module m_output;
        monitor_module m_monitor_module;
        osd_watchdog m_watchdog;  //std::unique_ptr<osd_watchdog> m_watchdog;
        std.vector<ui.menu_item> m_sliders;


        std.vector<string> m_video_names = new std.vector<string>();
        std.unordered_map<string, string> m_option_descs = new std.unordered_map<string, string>();


        // construction/destruction
        //-------------------------------------------------
        //  osd_interface - constructor
        //-------------------------------------------------
        public osd_common_t(osd_options options)
            : base()
        {
            m_machine = null;
            m_options = options;
            m_print_verbose = false;
            m_font_module = null;
            m_sound = null;
            m_debugger = null;
            m_midi = null;
            m_keyboard_input = null;
            m_mouse_input = null;
            m_lightgun_input = null;
            m_joystick_input = null;
            m_output = null;
            m_monitor_module = null;
            m_watchdog = null;


            osd_output.push(this);
        }

        ~osd_common_t()
        {
            g.assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            //m_video_options,reset();
            osd_output.pop(this);

            m_isDisposed = true;
        }


        //#define REGISTER_MODULE(_O, _X ) { extern const module_type _X; _O . register_module( _X ); }

        // FIXME: simply option handling
        public void register_options()
        {
            //module_type FONT_OSX = null; m_mod_man.register_module(FONT_OSX);  //REGISTER_MODULE(m_mod_man, FONT_OSX);
            //module_type FONT_WINDOWS = null; m_mod_man.register_module(FONT_WINDOWS);  //REGISTER_MODULE(m_mod_man, FONT_WINDOWS);
            //REGISTER_MODULE(m_mod_man, FONT_DWRITE);
            //module_type FONT_SDL = null; m_mod_man.register_module(FONT_SDL);  //REGISTER_MODULE(m_mod_man, FONT_SDL);
            m_mod_man.register_module(font_none.FONT_NONE);  //REGISTER_MODULE(m_mod_man, FONT_NONE);

            //REGISTER_MODULE(m_mod_man, SOUND_XAUDIO2);
            //module_type SOUND_DSOUND = null; m_mod_man.register_module(SOUND_DSOUND);  //REGISTER_MODULE(m_mod_man, SOUND_DSOUND);
            //module_type SOUND_COREAUDIO = null; m_mod_man.register_module(SOUND_COREAUDIO);  //REGISTER_MODULE(m_mod_man, SOUND_COREAUDIO);
            //module_type SOUND_JS = null; m_mod_man.register_module(SOUND_JS);  //REGISTER_MODULE(m_mod_man, SOUND_JS);
            //module_type SOUND_SDL = null; m_mod_man.register_module(SOUND_SDL);  //REGISTER_MODULE(m_mod_man, SOUND_SDL);
#if NO_USE_PORTAUDIO
            REGISTER_MODULE(m_mod_man, SOUND_PORTAUDIO);
#endif
            //module_type SOUND_NONE = null; m_mod_man.register_module(FONT_OSX);  //REGISTER_MODULE(m_mod_man, SOUND_NONE);

            //REGISTER_MODULE(m_mod_man, MONITOR_SDL);
            m_mod_man.register_module(win32_monitor_module.MONITOR_WIN32);  //REGISTER_MODULE(m_mod_man, MONITOR_WIN32);
            //REGISTER_MODULE(m_mod_man, MONITOR_DXGI);
            //REGISTER_MODULE(m_mod_man, MONITOR_MAC);

#if SDLMAME_MACOSX
            module_type DEBUG_OSX = null; m_mod_man.register_module(DEBUG_OSX);  //REGISTER_MODULE(m_mod_man, DEBUG_OSX);
#endif

#if !OSD_MINI
            //module_type DEBUG_WINDOWS = null; m_mod_man.register_module(DEBUG_WINDOWS);  //REGISTER_MODULE(m_mod_man, DEBUG_WINDOWS);
            //module_type DEBUG_QT = null; m_mod_man.register_module(DEBUG_QT);  //REGISTER_MODULE(m_mod_man, DEBUG_QT);
            //REGISTER_MODULE(m_mod_man, DEBUG_IMGUI);
            //REGISTER_MODULE(m_mod_man, DEBUG_GDBSTUB);
            //module_type DEBUG_NONE = null; m_mod_man.register_module(DEBUG_NONE);  //REGISTER_MODULE(m_mod_man, DEBUG_NONE);
#endif

            //module_type NETDEV_TAPTUN = null; m_mod_man.register_module(NETDEV_TAPTUN);  //REGISTER_MODULE(m_mod_man, NETDEV_TAPTUN);
            //module_type NETDEV_PCAP = null; m_mod_man.register_module(NETDEV_PCAP);  //REGISTER_MODULE(m_mod_man, NETDEV_PCAP);
            //module_type NETDEV_NONE = null; m_mod_man.register_module(NETDEV_NONE);  //REGISTER_MODULE(m_mod_man, NETDEV_NONE);

#if !NO_USE_MIDI
            //module_type MIDI_PM = null; m_mod_man.register_module(MIDI_PM);  //REGISTER_MODULE(m_mod_man, MIDI_PM);
#endif
            m_mod_man.register_module(none_module.MIDI_NONE);  //REGISTER_MODULE(m_mod_man, MIDI_NONE);

            //REGISTER_MODULE(m_mod_man, KEYBOARDINPUT_SDL);
            //REGISTER_MODULE(m_mod_man, KEYBOARDINPUT_RAWINPUT);
            //REGISTER_MODULE(m_mod_man, KEYBOARDINPUT_DINPUT);
            //REGISTER_MODULE(m_mod_man, KEYBOARDINPUT_WIN32);
            //REGISTER_MODULE(m_mod_man, KEYBOARDINPUT_UWP);
            m_mod_man.register_module(keyboard_input_none.KEYBOARD_NONE);  //REGISTER_MODULE(m_mod_man, KEYBOARD_NONE);

            //REGISTER_MODULE(m_mod_man, MOUSEINPUT_SDL);
            //REGISTER_MODULE(m_mod_man, MOUSEINPUT_RAWINPUT);
            //REGISTER_MODULE(m_mod_man, MOUSEINPUT_DINPUT);
            //REGISTER_MODULE(m_mod_man, MOUSEINPUT_WIN32);
            m_mod_man.register_module(mouse_input_none.MOUSE_NONE);  //REGISTER_MODULE(m_mod_man, MOUSE_NONE);

            //REGISTER_MODULE(m_mod_man, LIGHTGUN_X11);
            //REGISTER_MODULE(m_mod_man, LIGHTGUNINPUT_RAWINPUT);
            //REGISTER_MODULE(m_mod_man, LIGHTGUNINPUT_WIN32);
            m_mod_man.register_module(lightgun_input_none.LIGHTGUN_NONE);  //REGISTER_MODULE(m_mod_man, LIGHTGUN_NONE);

            //REGISTER_MODULE(m_mod_man, JOYSTICKINPUT_SDL);
            //REGISTER_MODULE(m_mod_man, JOYSTICKINPUT_WINHYBRID);
            //REGISTER_MODULE(m_mod_man, JOYSTICKINPUT_DINPUT);
            //REGISTER_MODULE(m_mod_man, JOYSTICKINPUT_XINPUT);
            //REGISTER_MODULE(m_mod_man, JOYSTICKINPUT_UWP);
            m_mod_man.register_module(joystick_input_none.JOYSTICK_NONE);  //REGISTER_MODULE(m_mod_man, JOYSTICK_NONE);

            m_mod_man.register_module(output_none.OUTPUT_NONE);  //REGISTER_MODULE(m_mod_man, OUTPUT_NONE);
            //REGISTER_MODULE(m_mod_man, OUTPUT_CONSOLE);
            //REGISTER_MODULE(m_mod_man, OUTPUT_NETWORK);
            //REGISTER_MODULE(m_mod_man, OUTPUT_WIN32);


            // after initialization we know which modules are supported

            List<string> names;
            int num;
            std.vector<string> dnames = new std.vector<string>();

            m_mod_man.get_module_names(monitor_module.OSD_MONITOR_PROVIDER, 20, out num, out names);
            for (int i = 0; i < num; i++)
                dnames.push_back(names[i]);
            update_option(monitor_module.OSD_MONITOR_PROVIDER, dnames);

            m_mod_man.get_module_names(font_module.OSD_FONT_PROVIDER, 20, out num, out names);
            for (int i = 0; i < num; i++)
                dnames.push_back(names[i]);
            update_option(font_module.OSD_FONT_PROVIDER, dnames);

            m_mod_man.get_module_names(input_module.OSD_KEYBOARDINPUT_PROVIDER, 20, out num, out names);
            dnames.clear();
            for (int i = 0; i < num; i++)
                dnames.push_back(names[i]);
            update_option(input_module.OSD_KEYBOARDINPUT_PROVIDER, dnames);

            m_mod_man.get_module_names(input_module.OSD_MOUSEINPUT_PROVIDER, 20, out num, out names);
            dnames.clear();
            for (int i = 0; i < num; i++)
                dnames.push_back(names[i]);
            update_option(input_module.OSD_MOUSEINPUT_PROVIDER, dnames);

            m_mod_man.get_module_names(input_module.OSD_LIGHTGUNINPUT_PROVIDER, 20, out num, out names);
            dnames.clear();
            for (int i = 0; i < num; i++)
                dnames.push_back(names[i]);
            update_option(input_module.OSD_LIGHTGUNINPUT_PROVIDER, dnames);

            m_mod_man.get_module_names(input_module.OSD_JOYSTICKINPUT_PROVIDER, 20, out num, out names);
            dnames.clear();
            for (int i = 0; i < num; i++)
                dnames.push_back(names[i]);
            update_option(input_module.OSD_JOYSTICKINPUT_PROVIDER, dnames);

            m_mod_man.get_module_names(sound_module.OSD_SOUND_PROVIDER, 20, out num, out names);
            dnames.clear();
            for (int i = 0; i < num; i++)
                dnames.push_back(names[i]);
            update_option(sound_module.OSD_SOUND_PROVIDER, dnames);

#if false
            // Register midi options and update options
            m_mod_man.get_module_names(midi_module.OSD_MIDI_PROVIDER, 20, out num, out names);
            dnames.Clear();
            for (int i = 0; i < num; i++)
                dnames.push_back(names[i]);
            update_option(midi_module.OSD_MIDI_PROVIDER, dnames);
#endif

            // Register debugger options and update options
            m_mod_man.get_module_names(debug_module.OSD_DEBUG_PROVIDER, 20, out num, out names);
            dnames.clear();
            for (int i = 0; i < num; i++)
                dnames.push_back(names[i]);
            update_option(debug_module.OSD_DEBUG_PROVIDER, dnames);

            m_mod_man.get_module_names(output_module.OSD_OUTPUT_PROVIDER, 20, out num, out names);
            dnames.clear();
            for (int i = 0; i < num; i++)
                dnames.push_back(names[i]);
            update_option(output_module.OSD_OUTPUT_PROVIDER, dnames);

            // Register video options and update options
            video_options_add("none", null);
            video_register();
            update_option(osd_options.OSDOPTION_VIDEO, m_video_names);
        }


        // general overridables

        //-------------------------------------------------
        //  init - initialize the OSD system.
        //-------------------------------------------------
        public virtual void init(running_machine machine)
        {
            // This function is responsible for initializing the OSD-specific
            // video and input functionality, and registering that functionality
            // with the MAME core.
            //
            // In terms of video, this function is expected to create one or more
            // render_targets that will be used by the MAME core to provide graphics
            // data to the system. Although it is possible to do this later, the
            // assumption in the MAME core is that the user interface will be
            // visible starting at init() time, so you will have some work to
            // do to avoid these assumptions.
            //
            // In terms of input, this function is expected to enumerate all input
            // devices available and describe them to the MAME core by adding
            // input devices and their attached items (buttons/axes) via the input
            // system.
            //
            // Beyond these core responsibilities, init() should also initialize
            // any other OSD systems that require information about the current
            // running_machine.
            //
            // This callback is also the last opportunity to adjust the options
            // before they are consumed by the rest of the core.
            //
            // Future work/changes:
            //
            // Audio initialization may eventually move into here as well,
            // instead of relying on independent callbacks from each system.

            m_machine = machine;

            var options = (osd_options)machine.options();

            // extract the verbose printing option
            if (options.verbose())
                set_verbose(true);

            // ensure we get called on the way out
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_EXIT, osd_exit);

            /* now setup watchdog */
            int watchdog_timeout = options.watchdog();

            if (watchdog_timeout != 0)
            {
                m_watchdog = new osd_watchdog();
                m_watchdog.setTimeout(watchdog_timeout);
            }
        }


        //-------------------------------------------------
        //  update - periodic system update
        //-------------------------------------------------
        public virtual void update(bool skip_redraw)
        {
            //
            // This method is called periodically to flush video updates to the
            // screen, and also to allow the OSD a chance to update other systems
            // on a regular basis. In general this will be called at the frame
            // rate of the system being run; however, it may be called at more
            // irregular intervals in some circumstances (e.g., multi-screen games
            // or games with asynchronous updates).
            //

            if (m_watchdog != null)
                m_watchdog.reset();

            update_slider_list();
        }


        public abstract void input_update();


        public virtual void set_verbose(bool print_verbose) { m_print_verbose = print_verbose; }


        // debugger overridables

        //-------------------------------------------------
        //  init_debugger - perform debugger-specific
        //  initialization
        //-------------------------------------------------
        protected virtual void init_debugger()
        {
            //
            // Unlike init() above, this method is only called if the debugger
            // is active. This gives any OSD debugger interface a chance to
            // create all of its structures.
            //
            m_debugger.init_debugger(machine());
        }


        //-------------------------------------------------
        //  wait_for_debugger - wait for a debugger
        //  command to be processed
        //-------------------------------------------------
        protected virtual void wait_for_debugger(device_t device, bool firststop)
        {
            //
            // When implementing an OSD-driver debugger, this method should be
            // overridden to wait for input, process it, and return. It will be
            // called repeatedly until a command is issued that resumes
            // execution.
            //
            m_debugger.wait_for_debugger(device, firststop);
        }


        // audio overridables

        //-------------------------------------------------
        //  update_audio_stream - update the stereo audio
        //  stream
        //-------------------------------------------------
        public virtual void update_audio_stream(Pointer<int16_t> buffer, int samples_this_frame)  //const int16_t *buffer, int samples_this_frame)
        {
            //
            // This method is called whenever the system has new audio data to stream.
            // It provides an array of stereo samples in L-R order which should be
            // output at the configured sample_rate.
            //
            if (m_sound != null)
                m_sound.update_audio_stream(m_machine.video().throttled(), buffer, samples_this_frame);
        }


        //-------------------------------------------------
        //  set_mastervolume - set the system volume
        //-------------------------------------------------
        public virtual void set_mastervolume(int attenuation)
        {
            //
            // Attenuation is the attenuation in dB (a negative number).
            // To convert from dB to a linear volume scale do the following:
            //    volume = MAX_VOLUME;
            //    while (attenuation++ < 0)
            //       volume /= 1.122018454;      //  = (10 ^ (1/20)) = 1dB
            //
            if (m_sound != null)
                m_sound.set_mastervolume(attenuation);
        }

        public virtual bool no_sound()
        {
            return options().sound() == "none";
        }


        // input overridables
        //-------------------------------------------------
        //  customize_input_type_list - provide OSD
        //  additions/modifications to the input list
        //-------------------------------------------------
        public virtual void customize_input_type_list(std.vector<input_type_entry> typelist)
        {
            //
            // inptport.c defines some general purpose defaults for key and joystick bindings.
            // They may be further adjusted by the OS dependent code to better match the
            // available keyboard, e.g. one could map pause to the Pause key instead of P, or
            // snapshot to PrtScr instead of F12. Of course the user can further change the
            // settings to anything he/she likes.
            //
            // This function is called on startup, before reading the configuration from disk.
            // Scan the list, and change the keys/joysticks you want.
            //
        }


        // video overridables

        public virtual void add_audio_to_recording(Pointer<int16_t> buffer, int samples_this_frame)
        {
        }


        //-------------------------------------------------
        //  get_slider_list - allocate and populate a
        //  list of OS-dependent slider values.
        //-------------------------------------------------
        public virtual std.vector<ui.menu_item> get_slider_list()
        {
            return m_sliders;
        }


        // command option overrides
        //-------------------------------------------------
        //  execute_command - execute a command not yet
        //  handled by the core
        //-------------------------------------------------
        public virtual bool execute_command(string command)
        {
            if (command == osd_options.OSDCOMMAND_LIST_NETWORK_ADAPTERS)
            {
                osd_module om = select_module_options<osd_module>(options(), netdev_module.OSD_NETDEV_PROVIDER);

                if (om.probe())
                {
                    om.init(options());
                    osdcore_global.m_osdcore.osd_list_network_adapters();
                    om.exit();
                }

                return true;
            }
            else if (command == osd_options.OSDCOMMAND_LIST_MIDI_DEVICES)
            {
                osd_module om = select_module_options<osd_module>(options(), midi_module.OSD_MIDI_PROVIDER);
                var pm = select_module_options<midi_module>(options(), midi_module.OSD_MIDI_PROVIDER);

                if (om.probe())
                {
                    om.init(options());
                    pm.list_midi_devices();
                    om.exit();
                }

                return true;
            }

            return false;
        }


        public osd_font font_alloc() { return m_font_module.font_alloc(); }
        //virtual bool get_font_families(std::string const &font_path, std::vector<std::pair<std::string, std::string> > &result) override { return m_font_module->get_font_families(font_path, result); }


        //osd_midi_device create_midi_device() { return m_midi->create_midi_device(); }


        // FIXME: everything below seems to be osd specific and not part of
        //        this INTERFACE but part of the osd IMPLEMENTATION

        // getters
        protected running_machine machine() { g.assert(m_machine != null);  return m_machine; }


        protected virtual void debugger_update()
        {
            if (m_debugger != null)
                m_debugger.debugger_update();
        }


        static void output_notifier_callback(string outname, int value, object param)
        {
            ((osd_common_t)param).notify(outname, value);
        }


        protected virtual void init_subsystems()
        {
            // monitors have to be initialized before video init
            m_monitor_module = select_module_options<monitor_module>(options(), monitor_module.OSD_MONITOR_PROVIDER);
            g.assert(m_monitor_module != null);
            m_monitor_module.init(options());

            if (!video_init())
            {
                video_exit();
                g.osd_printf_error("video_init: Initialization failed!\n\n\n");
                //fflush(stderr);
                //fflush(stdout);
                throw new emu_fatalerror("video_init: Initialization failed!\n\n\n");  //Environment.Exit(-1);  // exit(-1);
            }

            m_keyboard_input = select_module_options<input_module>(options(), input_module.OSD_KEYBOARDINPUT_PROVIDER);
            m_mouse_input = select_module_options<input_module>(options(), input_module.OSD_MOUSEINPUT_PROVIDER);
            m_lightgun_input = select_module_options<input_module>(options(), input_module.OSD_LIGHTGUNINPUT_PROVIDER);
            m_joystick_input = select_module_options<input_module>(options(), input_module.OSD_JOYSTICKINPUT_PROVIDER);

            m_font_module = select_module_options<font_module>(options(), font_module.OSD_FONT_PROVIDER);

            m_sound = select_module_options<sound_module>(options(), sound_module.OSD_SOUND_PROVIDER);
            if (m_sound != null)
            {
                m_sound.m_sample_rate = options().sample_rate();
                m_sound.m_audio_latency = options().audio_latency();
            }

            m_debugger = select_module_options<debug_module>(options(), debug_module.OSD_DEBUG_PROVIDER);

            select_module_options<netdev_module>(options(), netdev_module.OSD_NETDEV_PROVIDER);

            m_midi = select_module_options<midi_module>(options(), midi_module.OSD_MIDI_PROVIDER);

            m_output = select_module_options<output_module>(options(), output_module.OSD_OUTPUT_PROVIDER);
            m_output.set_machine(machine());
            machine().output().set_global_notifier(output_notifier_callback, this);

            m_mod_man.init(options());

            input_init();
            // we need pause callbacks
            machine().add_notifier(machine_notification.MACHINE_NOTIFY_PAUSE, input_pause);
            machine().add_notifier(machine_notification.MACHINE_NOTIFY_RESUME, input_resume);
        }


        protected virtual bool video_init() { return true; }

        protected virtual void video_register() { }

        protected virtual bool window_init() { return true; }


        protected virtual void input_resume(running_machine machine)
        {
            m_keyboard_input.resume();
            m_mouse_input.resume();
            m_lightgun_input.resume();
            m_joystick_input.resume();
        }


        protected virtual bool output_init() { return true; }
 

        protected virtual void exit_subsystems()
        {
            video_exit();
        }


        protected virtual void video_exit() { }
        protected virtual void window_exit() { }


        protected virtual void osd_exit(running_machine machine)
        {
            m_mod_man.exit();

            exit_subsystems();
        }


        protected virtual void video_options_add(string name, object type)  // void *type)
        {
            //m_video_options.add(name, type, false);
            m_video_names.push_back(name);
        }


        protected virtual osd_options options() { return m_options; }


        // osd_output interface ...

        //-------------------------------------------------
        //  output_callback  - callback for osd_printf_...
        //-------------------------------------------------
        public override void output_callback(osd_output_channel channel, string format, params object [] args)  //virtual void output_callback(osd_output_channel channel, const util::format_argument_pack<std::ostream> &args)  override;
        {
            switch (channel)
            {
                case osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR:
                case osd_output_channel.OSD_OUTPUT_CHANNEL_WARNING:
                    System.Diagnostics.Debug.Write(string.Format(format, args));  //util::stream_format(std::cerr, args);
                    break;
                case osd_output_channel.OSD_OUTPUT_CHANNEL_INFO:
                case osd_output_channel.OSD_OUTPUT_CHANNEL_LOG:
                    System.Diagnostics.Debug.Write(string.Format(format, args));  //util::stream_format(std::cout, args);
                    break;
                case osd_output_channel.OSD_OUTPUT_CHANNEL_VERBOSE:
                    if (verbose())
                    {
                        System.Diagnostics.Debug.Write(string.Format(format, args));  //util::stream_format(std::cout, args);
                    }
                    break;
                case osd_output_channel.OSD_OUTPUT_CHANNEL_DEBUG:
//#if MAME_DEBUG
                    System.Diagnostics.Debug.Write(string.Format(format, args));  //util::stream_format(std::cout, args);
//#endif
                    break;
                default:
                    break;
            }
        }


        protected bool verbose() { return m_print_verbose; }


        void notify(string outname, int value) { m_output.notify(outname, value); }



        protected virtual bool input_init()
        {
            m_keyboard_input.input_init(machine());
            m_mouse_input.input_init(machine());
            m_lightgun_input.input_init(machine());
            m_joystick_input.input_init(machine());
            return true;
        }


        protected virtual void input_pause(running_machine machine)
        {
            m_keyboard_input.pause();
            m_mouse_input.pause();
            m_lightgun_input.pause();
            m_joystick_input.pause();
        }


        protected virtual void build_slider_list() { }


        protected virtual void update_slider_list() { }


        void update_option(string key, MemoryContainer<string> values)
        {
            string current_value = m_options.description(key);
            string new_option_value = "";
            for (int index = 0; index < values.Count; index++)
            {
                string t = values[index];
                if (new_option_value.Length > 0)
                {
                    if (index != (values.Count - 1))
                        new_option_value += ", ";
                    else
                        new_option_value += " or ";
                }
                new_option_value += t;
            }

            m_option_descs[key] = current_value + new_option_value;
            m_options.set_description(key, m_option_descs[key]);
        }


        // FIXME: should be elsewhere
        object select_module_options(core_options opts, string opt_name)
        {
            string opt_val = opts.exists(opt_name) ? opts.value(opt_name) : "";
            if (opt_val == "auto")
            {
                opt_val = "";
            }
            else if (!m_mod_man.type_has_name(opt_name, opt_val))
            {
                g.osd_printf_warning("Value {0} not supported for option {1} - falling back to auto\n", opt_val, opt_name);
                opt_val = "";
            }

            return m_mod_man.select_module(opt_name, opt_val);
        }

        //template<class C>
        C select_module_options<C>(core_options opts, string opt_name)
        {
            return (C)select_module_options(opts, opt_name);
        }
    }
}
