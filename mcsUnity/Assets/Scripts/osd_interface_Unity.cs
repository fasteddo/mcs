// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using UnityEngine;
using mame;

using static mame.emuopts_global;
using static mame.osdcore_global;


namespace mcsUnity
{
    public class osd_options_Unity : osd_options  // windows_options
    {
        // debugging options
        const string WINOPTION_DEBUGGER_FONT         = "debugger_font";
        const string WINOPTION_DEBUGGER_FONT_SIZE    = "debugger_font_size";

        // performance options
        const string WINOPTION_PRIORITY              = "priority";
        const string WINOPTION_PROFILE               = "profile";

        // video options
        const string WINOPTION_PRESCALE              = "prescale";
        const string WINOPTION_MENU                  = "menu";

        // DirectDraw-specific options
        const string WINOPTION_HWSTRETCH             = "hwstretch";

        // Direct3D-specific options
        const string WINOPTION_FILTER                = "filter";

        // core post-processing options
        const string WINOPTION_HLSL_ENABLE               = "hlsl_enable";
        const string WINOPTION_HLSLPATH                  = "hlslpath";
        const string WINOPTION_HLSL_PRESCALE_X           = "hlsl_prescale_x";
        const string WINOPTION_HLSL_PRESCALE_Y           = "hlsl_prescale_y";
        const string WINOPTION_HLSL_PRESET               = "hlsl_preset";
        const string WINOPTION_HLSL_WRITE                = "hlsl_write";
        const string WINOPTION_HLSL_SNAP_WIDTH           = "hlsl_snap_width";
        const string WINOPTION_HLSL_SNAP_HEIGHT          = "hlsl_snap_height";
        const string WINOPTION_SHADOW_MASK_ALPHA         = "shadow_mask_alpha";
        const string WINOPTION_SHADOW_MASK_TEXTURE       = "shadow_mask_texture";
        const string WINOPTION_SHADOW_MASK_COUNT_X       = "shadow_mask_x_count";
        const string WINOPTION_SHADOW_MASK_COUNT_Y       = "shadow_mask_y_count";
        const string WINOPTION_SHADOW_MASK_USIZE         = "shadow_mask_usize";
        const string WINOPTION_SHADOW_MASK_VSIZE         = "shadow_mask_vsize";
        const string WINOPTION_PINCUSHION                = "pincushion";
        const string WINOPTION_CURVATURE                 = "curvature";
        const string WINOPTION_SCANLINE_AMOUNT           = "scanline_alpha";
        const string WINOPTION_SCANLINE_SCALE            = "scanline_size";
        const string WINOPTION_SCANLINE_HEIGHT           = "scanline_height";
        const string WINOPTION_SCANLINE_BRIGHT_SCALE     = "scanline_bright_scale";
        const string WINOPTION_SCANLINE_BRIGHT_OFFSET    = "scanline_bright_offset";
        const string WINOPTION_SCANLINE_OFFSET           = "scanline_jitter";
        const string WINOPTION_DEFOCUS                   = "defocus";
        const string WINOPTION_CONVERGE_X                = "converge_x";
        const string WINOPTION_CONVERGE_Y                = "converge_y";
        const string WINOPTION_RADIAL_CONVERGE_X         = "radial_converge_x";
        const string WINOPTION_RADIAL_CONVERGE_Y         = "radial_converge_y";
        const string WINOPTION_RED_RATIO                 = "red_ratio";
        const string WINOPTION_GRN_RATIO                 = "grn_ratio";
        const string WINOPTION_BLU_RATIO                 = "blu_ratio";
        const string WINOPTION_OFFSET                    = "offset";
        const string WINOPTION_SCALE                     = "scale";
        const string WINOPTION_POWER                     = "power";
        const string WINOPTION_FLOOR                     = "floor";
        const string WINOPTION_PHOSPHOR                  = "phosphor_life";
        const string WINOPTION_SATURATION                = "saturation";
        const string WINOPTION_YIQ_ENABLE                = "yiq_enable";
        const string WINOPTION_YIQ_CCVALUE               = "yiq_cc";
        const string WINOPTION_YIQ_AVALUE                = "yiq_a";
        const string WINOPTION_YIQ_BVALUE                = "yiq_b";
        const string WINOPTION_YIQ_OVALUE                = "yiq_o";
        const string WINOPTION_YIQ_PVALUE                = "yiq_p";
        const string WINOPTION_YIQ_NVALUE                = "yiq_n";
        const string WINOPTION_YIQ_YVALUE                = "yiq_y";
        const string WINOPTION_YIQ_IVALUE                = "yiq_i";
        const string WINOPTION_YIQ_QVALUE                = "yiq_q";
        const string WINOPTION_YIQ_SCAN_TIME             = "yiq_scan_time";
        const string WINOPTION_YIQ_PHASE_COUNT           = "yiq_phase_count";
        const string WINOPTION_VECTOR_LENGTH_SCALE       = "vector_length_scale";
        const string WINOPTION_VECTOR_LENGTH_RATIO       = "vector_length_ratio";
        const string WINOPTION_VECTOR_BLOOM_SCALE        = "vector_bloom_scale";
        const string WINOPTION_VECTOR_TIME_PERIOD        = "vector_time_period";
        const string WINOPTION_RASTER_BLOOM_SCALE        = "raster_bloom_scale";
        const string WINOPTION_BLOOM_LEVEL0_WEIGHT       = "bloom_lvl0_weight";
        const string WINOPTION_BLOOM_LEVEL1_WEIGHT       = "bloom_lvl1_weight";
        const string WINOPTION_BLOOM_LEVEL2_WEIGHT       = "bloom_lvl2_weight";
        const string WINOPTION_BLOOM_LEVEL3_WEIGHT       = "bloom_lvl3_weight";
        const string WINOPTION_BLOOM_LEVEL4_WEIGHT       = "bloom_lvl4_weight";
        const string WINOPTION_BLOOM_LEVEL5_WEIGHT       = "bloom_lvl5_weight";
        const string WINOPTION_BLOOM_LEVEL6_WEIGHT       = "bloom_lvl6_weight";
        const string WINOPTION_BLOOM_LEVEL7_WEIGHT       = "bloom_lvl7_weight";
        const string WINOPTION_BLOOM_LEVEL8_WEIGHT       = "bloom_lvl8_weight";
        const string WINOPTION_BLOOM_LEVEL9_WEIGHT       = "bloom_lvl9_weight";
        const string WINOPTION_BLOOM_LEVEL10_WEIGHT      = "bloom_lvl10_weight";

        // full screen options
        const string WINOPTION_TRIPLEBUFFER          = "triplebuffer";
        const string WINOPTION_FULLSCREENBRIGHTNESS  = "full_screen_brightness";
        const string WINOPTION_FULLSCREENCONTRAST    = "full_screen_contrast";
        const string WINOPTION_FULLSCREENGAMMA       = "full_screen_gamma";

        // input options
        const string WINOPTION_DUAL_LIGHTGUN         = "dual_lightgun";


        static readonly options_entry [] s_option_entries = new options_entry []
        {
            // debugging options
            new options_entry(null,                                           null,       options_global.OPTION_HEADER,     "WINDOWS DEBUGGING OPTIONS"),
            new options_entry(WINOPTION_DEBUGGER_FONT + ";dfont",             "Lucida Console", options_global.OPTION_STRING,"specifies the font to use for debugging; defaults to Lucida Console"),
            new options_entry(WINOPTION_DEBUGGER_FONT_SIZE + ";dfontsize",    "9",        options_global.OPTION_FLOAT,      "specifies the font size to use for debugging; defaults to 9 pt"),

            // performance options
            new options_entry(null,                                           null,       options_global.OPTION_HEADER,     "WINDOWS PERFORMANCE OPTIONS"),
            new options_entry(WINOPTION_PRIORITY + "(-15-1)",                 "0",        options_global.OPTION_INTEGER,    "thread priority for the main game thread; range from -15 to 1"),
            new options_entry(WINOPTION_PROFILE,                              "0",        options_global.OPTION_INTEGER,    "enable profiling, specifying the stack depth to track"),

            // video options
            new options_entry(null,                                           null,       options_global.OPTION_HEADER,     "WINDOWS VIDEO OPTIONS"),
            new options_entry(WINOPTION_PRESCALE,                             "1",        options_global.OPTION_INTEGER,    "scale screen rendering by this amount in software"),
            new options_entry(WINOPTION_MENU,                                 "0",        options_global.OPTION_BOOLEAN,    "enable menu bar if available by UI implementation"),

            // DirectDraw-specific options
            new options_entry(null,                                           null,       options_global.OPTION_HEADER,     "DIRECTDRAW-SPECIFIC OPTIONS"),
            new options_entry(WINOPTION_HWSTRETCH + ";hws",                   "1",        options_global.OPTION_BOOLEAN,    "enable hardware stretching"),

            // Direct3D-specific options
            new options_entry(null,                                           null,       options_global.OPTION_HEADER,     "DIRECT3D-SPECIFIC OPTIONS"),
            new options_entry(WINOPTION_FILTER + ";d3dfilter;flt",            "1",        options_global.OPTION_BOOLEAN,    "enable bilinear filtering on screen output"),

            // post-processing options
            new options_entry(null,                                                     null,        options_global.OPTION_HEADER,     "DIRECT3D POST-PROCESSING OPTIONS"),
            new options_entry(WINOPTION_HLSL_ENABLE + ";hlsl",                          "0",         options_global.OPTION_BOOLEAN,    "enable HLSL post-processing (PS3.0 required)"),
            new options_entry(WINOPTION_HLSLPATH,                                       "hlsl",      options_global.OPTION_STRING,     "path to hlsl files"),
            new options_entry(WINOPTION_HLSL_PRESCALE_X,                                "0",         options_global.OPTION_INTEGER,    "HLSL pre-scale override factor for X (0 for auto)"),
            new options_entry(WINOPTION_HLSL_PRESCALE_Y,                                "0",         options_global.OPTION_INTEGER,    "HLSL pre-scale override factor for Y (0 for auto)"),
            new options_entry(WINOPTION_HLSL_PRESET + ";(-1-3)",                        "-1",        options_global.OPTION_INTEGER,    "HLSL preset to use (0-3)"),
            new options_entry(WINOPTION_HLSL_WRITE,                                     null,        options_global.OPTION_STRING,     "enable HLSL AVI writing (huge disk bandwidth suggested)"),
            new options_entry(WINOPTION_HLSL_SNAP_WIDTH,                                "2048",      options_global.OPTION_STRING,     "HLSL upscaled-snapshot width"),
            new options_entry(WINOPTION_HLSL_SNAP_HEIGHT,                               "1536",      options_global.OPTION_STRING,     "HLSL upscaled-snapshot height"),
            new options_entry(WINOPTION_SHADOW_MASK_ALPHA + ";fs_shadwa(0.0-1.0)",      "0.0",       options_global.OPTION_FLOAT,      "shadow mask alpha-blend value (1.0 is fully blended, 0.0 is no mask)"),
            new options_entry(WINOPTION_SHADOW_MASK_TEXTURE + ";fs_shadwt(0.0-1.0)",    "aperture.png", options_global.OPTION_STRING,  "shadow mask texture name"),
            new options_entry(WINOPTION_SHADOW_MASK_COUNT_X + ";fs_shadww",             "320",       options_global.OPTION_INTEGER,    "shadow mask width, in phosphor dots"),
            new options_entry(WINOPTION_SHADOW_MASK_COUNT_Y + ";fs_shadwh",             "240",       options_global.OPTION_INTEGER,    "shadow mask height, in phosphor dots"),
            new options_entry(WINOPTION_SHADOW_MASK_USIZE + ";fs_shadwu(0.0-1.0)",      "0.09375",   options_global.OPTION_FLOAT,      "shadow mask texture size in U direction"),
            new options_entry(WINOPTION_SHADOW_MASK_VSIZE + ";fs_shadwv(0.0-1.0)",      "0.109375",  options_global.OPTION_FLOAT,      "shadow mask texture size in V direction"),
            new options_entry(WINOPTION_CURVATURE + ";fs_curv(0.0-4.0)",                "0.03",      options_global.OPTION_FLOAT,      "screen curvature amount"),
            /* Beam-related values below this line*/
            new options_entry(WINOPTION_PINCUSHION + ";fs_pin(0.0-4.0)",                "0.03",      options_global.OPTION_FLOAT,      "pincushion amount"),
            new options_entry(WINOPTION_SCANLINE_AMOUNT + ";fs_scanam(0.0-4.0)",        "1.0",       options_global.OPTION_FLOAT,      "overall alpha scaling value for scanlines"),
            new options_entry(WINOPTION_SCANLINE_SCALE + ";fs_scansc(0.0-4.0)",         "1.0",       options_global.OPTION_FLOAT,      "overall height scaling value for scanlines"),
            new options_entry(WINOPTION_SCANLINE_HEIGHT + ";fs_scanh(0.0-4.0)",         "1.0",       options_global.OPTION_FLOAT,      "individual height scaling value for scanlines"),
            new options_entry(WINOPTION_SCANLINE_BRIGHT_SCALE + ";fs_scanbs(0.0-2.0)",  "1.0",       options_global.OPTION_FLOAT,      "overall brightness scaling value for scanlines (multiplicative)"),
            new options_entry(WINOPTION_SCANLINE_BRIGHT_OFFSET + ";fs_scanbo(0.0-1.0)", "0.0",       options_global.OPTION_FLOAT,      "overall brightness offset value for scanlines (additive)"),
            new options_entry(WINOPTION_SCANLINE_OFFSET + ";fs_scanjt(0.0-4.0)",        "0.0",       options_global.OPTION_FLOAT,      "overall interlace jitter scaling value for scanlines"),
            new options_entry(WINOPTION_DEFOCUS + ";fs_focus",                          "0.0,0.0",   options_global.OPTION_STRING,     "overall defocus value in screen-relative coords"),
            new options_entry(WINOPTION_CONVERGE_X + ";fs_convx",                       "0.3,0.0,-0.3",options_global.OPTION_STRING,   "convergence in screen-relative X direction"),
            new options_entry(WINOPTION_CONVERGE_Y + ";fs_convy",                       "0.0,0.3,-0.3",options_global.OPTION_STRING,   "convergence in screen-relative Y direction"),
            new options_entry(WINOPTION_RADIAL_CONVERGE_X + ";fs_rconvx",               "0.0,0.0,0.0",options_global.OPTION_STRING,    "radial convergence in screen-relative X direction"),
            new options_entry(WINOPTION_RADIAL_CONVERGE_Y + ";fs_rconvy",               "0.0,0.0,0.0",options_global.OPTION_STRING,    "radial convergence in screen-relative Y direction"),
            /* RGB colorspace convolution below this line */
            new options_entry(WINOPTION_RED_RATIO + ";fs_redratio",                     "1.0,0.0,0.0",options_global.OPTION_STRING,    "red output signal generated by input signal"),
            new options_entry(WINOPTION_GRN_RATIO + ";fs_grnratio",                     "0.0,1.0,0.0",options_global.OPTION_STRING,    "green output signal generated by input signal"),
            new options_entry(WINOPTION_BLU_RATIO + ";fs_bluratio",                     "0.0,0.0,1.0",options_global.OPTION_STRING,    "blue output signal generated by input signal"),
            new options_entry(WINOPTION_SATURATION + ";fs_sat(0.0-4.0)",                "1.4",        options_global.OPTION_FLOAT,     "saturation scaling value"),
            new options_entry(WINOPTION_OFFSET + ";fs_offset",                          "0.0,0.0,0.0",options_global.OPTION_STRING,    "signal offset value (additive)"),
            new options_entry(WINOPTION_SCALE + ";fs_scale",                            "0.95,0.95,0.95",options_global.OPTION_STRING, "signal scaling value (multiplicative)"),
            new options_entry(WINOPTION_POWER + ";fs_power",                            "0.8,0.8,0.8",options_global.OPTION_STRING,    "signal power value (exponential)"),
            new options_entry(WINOPTION_FLOOR + ";fs_floor",                            "0.05,0.05,0.05",options_global.OPTION_STRING, "signal floor level"),
            new options_entry(WINOPTION_PHOSPHOR + ";fs_phosphor",                      "0.4,0.4,0.4",options_global.OPTION_STRING,    "phosphorescence decay rate (0.0 is instant, 1.0 is forever)"),
            /* NTSC simulation below this line */
            new options_entry(null,                                                     null,        options_global.OPTION_HEADER,     "NTSC POST-PROCESSING OPTIONS"),
            new options_entry(WINOPTION_YIQ_ENABLE + ";yiq",                            "0",         options_global.OPTION_BOOLEAN,    "enable YIQ-space HLSL post-processing"),
            new options_entry(WINOPTION_YIQ_CCVALUE + ";yiqcc",                         "3.59754545",options_global.OPTION_FLOAT,      "Color Carrier frequency for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_AVALUE + ";yiqa",                           "0.5",       options_global.OPTION_FLOAT,      "A value for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_BVALUE + ";yiqb",                           "0.5",       options_global.OPTION_FLOAT,      "B value for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_OVALUE + ";yiqo",                           "1.570796325",options_global.OPTION_FLOAT,     "Outgoing Color Carrier phase offset for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_PVALUE + ";yiqp",                           "1.0",       options_global.OPTION_FLOAT,      "Incoming Pixel Clock scaling value for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_NVALUE + ";yiqn",                           "1.0",       options_global.OPTION_FLOAT,      "Y filter notch width for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_YVALUE + ";yiqy",                           "6.0",       options_global.OPTION_FLOAT,      "Y filter cutoff frequency for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_IVALUE + ";yiqi",                           "1.2",       options_global.OPTION_FLOAT,      "I filter cutoff frequency for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_QVALUE + ";yiqq",                           "0.6",       options_global.OPTION_FLOAT,      "Q filter cutoff frequency for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_SCAN_TIME + ";yiqsc",                       "52.6",      options_global.OPTION_FLOAT,      "Horizontal scanline duration for NTSC signal processing (in usec)"),
            new options_entry(WINOPTION_YIQ_PHASE_COUNT + ";yiqp",                      "2",         options_global.OPTION_INTEGER,    "Phase Count value for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_SCAN_TIME + ";yiqsc",                       "52.6",      options_global.OPTION_FLOAT,      "Horizontal scanline duration for NTSC signal processing (in usec)"),
            new options_entry(WINOPTION_YIQ_PHASE_COUNT + ";yiqp",                      "2",         options_global.OPTION_INTEGER,    "Phase Count value for NTSC signal processing"),
            /* Vector simulation below this line */
            new options_entry(null,                                                     null,        options_global.OPTION_HEADER,     "VECTOR POST-PROCESSING OPTIONS"),
            new options_entry(WINOPTION_VECTOR_LENGTH_SCALE + ";veclength",             "0.8",       options_global.OPTION_FLOAT,      "How much length affects vector fade"),
            new options_entry(WINOPTION_VECTOR_LENGTH_RATIO + ";vecsize",               "500.0",     options_global.OPTION_FLOAT,      "Vector fade length (4.0 - vectors fade the most at and above 4 pixels, etc.)"),
            /* Bloom below this line */
            new options_entry(null,                                                     null,        options_global.OPTION_HEADER,     "BLOOM POST-PROCESSING OPTIONS"),
            new options_entry(WINOPTION_VECTOR_BLOOM_SCALE,                             "0.3",       options_global.OPTION_FLOAT,      "Intensity factor for vector bloom"),
            new options_entry(WINOPTION_RASTER_BLOOM_SCALE,                             "0.225",     options_global.OPTION_FLOAT,      "Intensity factor for raster bloom"),
            new options_entry(WINOPTION_BLOOM_LEVEL0_WEIGHT,                            "1.0",       options_global.OPTION_FLOAT,      "Bloom level 0  (full-size target) weight"),
            new options_entry(WINOPTION_BLOOM_LEVEL1_WEIGHT,                            "0.21",      options_global.OPTION_FLOAT,      "Bloom level 1  (half-size target) weight"),
            new options_entry(WINOPTION_BLOOM_LEVEL2_WEIGHT,                            "0.19",      options_global.OPTION_FLOAT,      "Bloom level 2  (quarter-size target) weight"),
            new options_entry(WINOPTION_BLOOM_LEVEL3_WEIGHT,                            "0.17",      options_global.OPTION_FLOAT,      "Bloom level 3  (.) weight"),
            new options_entry(WINOPTION_BLOOM_LEVEL4_WEIGHT,                            "0.15",      options_global.OPTION_FLOAT,      "Bloom level 4  (.) weight"),
            new options_entry(WINOPTION_BLOOM_LEVEL5_WEIGHT,                            "0.14",      options_global.OPTION_FLOAT,      "Bloom level 5  (.) weight"),
            new options_entry(WINOPTION_BLOOM_LEVEL6_WEIGHT,                            "0.13",      options_global.OPTION_FLOAT,      "Bloom level 6  (.) weight"),
            new options_entry(WINOPTION_BLOOM_LEVEL7_WEIGHT,                            "0.12",      options_global.OPTION_FLOAT,      "Bloom level 7  (.) weight"),
            new options_entry(WINOPTION_BLOOM_LEVEL8_WEIGHT,                            "0.11",      options_global.OPTION_FLOAT,      "Bloom level 8  (.) weight"),
            new options_entry(WINOPTION_BLOOM_LEVEL9_WEIGHT,                            "0.10",      options_global.OPTION_FLOAT,      "Bloom level 9  (.) weight"),
            new options_entry(WINOPTION_BLOOM_LEVEL10_WEIGHT,                           "0.09",      options_global.OPTION_FLOAT,      "Bloom level 10 (1x1 target) weight"),

            // full screen options
            new options_entry(null,                                             null,       options_global.OPTION_HEADER,     "FULL SCREEN OPTIONS"),
            new options_entry(WINOPTION_TRIPLEBUFFER + ";tb",                   "0",        options_global.OPTION_BOOLEAN,    "enable triple buffering"),
            new options_entry(WINOPTION_FULLSCREENBRIGHTNESS + ";fsb(0.1-2.0)", "1.0",      options_global.OPTION_FLOAT,      "brightness value in full screen mode"),
            new options_entry(WINOPTION_FULLSCREENCONTRAST + ";fsc(0.1-2.0)",   "1.0",      options_global.OPTION_FLOAT,      "contrast value in full screen mode"),
            new options_entry(WINOPTION_FULLSCREENGAMMA + ";fsg(0.1-3.0)",      "1.0",      options_global.OPTION_FLOAT,      "gamma value in full screen mode"),

            // input options
            new options_entry(null,                                             null,       options_global.OPTION_HEADER,     "INPUT DEVICE OPTIONS"),
            new options_entry(WINOPTION_DUAL_LIGHTGUN + ";dual",                "0",        options_global.OPTION_BOOLEAN,    "enable dual lightgun input"),

            new options_entry(null),
        };


        // construction/destruction
        //============================================================
        //  windows_options
        //============================================================
        public osd_options_Unity() : base()
        {
            add_entries(s_option_entries);
        }


        // debugging options
        //const char *debugger_font() const { return value(WINOPTION_DEBUGGER_FONT); }
        //float debugger_font_size() const { return float_value(WINOPTION_DEBUGGER_FONT_SIZE); }


        // performance options
        //int priority() const { return int_value(WINOPTION_PRIORITY); }
        public int profile() { return int_value(WINOPTION_PROFILE); }


        // video options
        //int prescale() const { return int_value(WINOPTION_PRESCALE); }
        //bool menu() const { return bool_value(WINOPTION_MENU); }


        // DirectDraw-specific options
        //bool hwstretch() const { return bool_value(WINOPTION_HWSTRETCH); }


        // Direct3D-specific options
        //bool filter() const { return bool_value(WINOPTION_FILTER); }


        // core post-processing options
        //const char *screen_post_fx_dir() const { return value(WINOPTION_HLSLPATH); }
        //bool d3d_hlsl_enable() const { return bool_value(WINOPTION_HLSL_ENABLE); }
        //const char *d3d_hlsl_write() const { return value(WINOPTION_HLSL_WRITE); }
        //int d3d_hlsl_prescale_x() const { return int_value(WINOPTION_HLSL_PRESCALE_X); }
        //int d3d_hlsl_prescale_y() const { return int_value(WINOPTION_HLSL_PRESCALE_Y); }
        //int d3d_hlsl_preset() const { return int_value(WINOPTION_HLSL_PRESET); }
        //int d3d_snap_width() const { return int_value(WINOPTION_HLSL_SNAP_WIDTH); }
        //int d3d_snap_height() const { return int_value(WINOPTION_HLSL_SNAP_HEIGHT); }
        //float screen_shadow_mask_alpha() const { return float_value(WINOPTION_SHADOW_MASK_ALPHA); }
        //const char *screen_shadow_mask_texture() const { return value(WINOPTION_SHADOW_MASK_TEXTURE); }
        //int screen_shadow_mask_count_x() const { return int_value(WINOPTION_SHADOW_MASK_COUNT_X); }
        //int screen_shadow_mask_count_y() const { return int_value(WINOPTION_SHADOW_MASK_COUNT_Y); }
        //float screen_shadow_mask_u_size() const { return float_value(WINOPTION_SHADOW_MASK_USIZE); }
        //float screen_shadow_mask_v_size() const { return float_value(WINOPTION_SHADOW_MASK_VSIZE); }
        //float screen_scanline_amount() const { return float_value(WINOPTION_SCANLINE_AMOUNT); }
        //float screen_scanline_scale() const { return float_value(WINOPTION_SCANLINE_SCALE); }
        //float screen_scanline_height() const { return float_value(WINOPTION_SCANLINE_HEIGHT); }
        //float screen_scanline_bright_scale() const { return float_value(WINOPTION_SCANLINE_BRIGHT_SCALE); }
        //float screen_scanline_bright_offset() const { return float_value(WINOPTION_SCANLINE_BRIGHT_OFFSET); }
        //float screen_scanline_offset() const { return float_value(WINOPTION_SCANLINE_OFFSET); }
        //float screen_pincushion() const { return float_value(WINOPTION_PINCUSHION); }
        //float screen_curvature() const { return float_value(WINOPTION_CURVATURE); }
        //const char *screen_defocus() const { return value(WINOPTION_DEFOCUS); }
        //const char *screen_converge_x() const { return value(WINOPTION_CONVERGE_X); }
        //const char *screen_converge_y() const { return value(WINOPTION_CONVERGE_Y); }
        //const char *screen_radial_converge_x() const { return value(WINOPTION_RADIAL_CONVERGE_X); }
        //const char *screen_radial_converge_y() const { return value(WINOPTION_RADIAL_CONVERGE_Y); }
        //const char *screen_red_ratio() const { return value(WINOPTION_RED_RATIO); }
        //const char *screen_grn_ratio() const { return value(WINOPTION_GRN_RATIO); }
        //const char *screen_blu_ratio() const { return value(WINOPTION_BLU_RATIO); }
        //bool screen_yiq_enable() const { return bool_value(WINOPTION_YIQ_ENABLE); }
        //float screen_yiq_cc() const { return float_value(WINOPTION_YIQ_CCVALUE); }
        //float screen_yiq_a() const { return float_value(WINOPTION_YIQ_AVALUE); }
        //float screen_yiq_b() const { return float_value(WINOPTION_YIQ_BVALUE); }
        //float screen_yiq_o() const { return float_value(WINOPTION_YIQ_OVALUE); }
        //float screen_yiq_p() const { return float_value(WINOPTION_YIQ_PVALUE); }
        //float screen_yiq_n() const { return float_value(WINOPTION_YIQ_NVALUE); }
        //float screen_yiq_y() const { return float_value(WINOPTION_YIQ_YVALUE); }
        //float screen_yiq_i() const { return float_value(WINOPTION_YIQ_IVALUE); }
        //float screen_yiq_q() const { return float_value(WINOPTION_YIQ_QVALUE); }
        //float screen_yiq_scan_time() const { return float_value(WINOPTION_YIQ_SCAN_TIME); }
        //int screen_yiq_phase_count() const { return int_value(WINOPTION_YIQ_PHASE_COUNT); }
        //float screen_vector_length_scale() const { return float_value(WINOPTION_VECTOR_LENGTH_SCALE); }
        //float screen_vector_length_ratio() const { return float_value(WINOPTION_VECTOR_LENGTH_RATIO); }
        //float screen_vector_bloom_scale() const { return float_value(WINOPTION_VECTOR_BLOOM_SCALE); }
        //float screen_vector_time_period() const { return float_value(WINOPTION_VECTOR_TIME_PERIOD); }
        //float screen_raster_bloom_scale() const { return float_value(WINOPTION_RASTER_BLOOM_SCALE); }
        //float screen_bloom_lvl0_weight() const { return float_value(WINOPTION_BLOOM_LEVEL0_WEIGHT); }
        //float screen_bloom_lvl1_weight() const { return float_value(WINOPTION_BLOOM_LEVEL1_WEIGHT); }
        //float screen_bloom_lvl2_weight() const { return float_value(WINOPTION_BLOOM_LEVEL2_WEIGHT); }
        //float screen_bloom_lvl3_weight() const { return float_value(WINOPTION_BLOOM_LEVEL3_WEIGHT); }
        //float screen_bloom_lvl4_weight() const { return float_value(WINOPTION_BLOOM_LEVEL4_WEIGHT); }
        //float screen_bloom_lvl5_weight() const { return float_value(WINOPTION_BLOOM_LEVEL5_WEIGHT); }
        //float screen_bloom_lvl6_weight() const { return float_value(WINOPTION_BLOOM_LEVEL6_WEIGHT); }
        //float screen_bloom_lvl7_weight() const { return float_value(WINOPTION_BLOOM_LEVEL7_WEIGHT); }
        //float screen_bloom_lvl8_weight() const { return float_value(WINOPTION_BLOOM_LEVEL8_WEIGHT); }
        //float screen_bloom_lvl9_weight() const { return float_value(WINOPTION_BLOOM_LEVEL9_WEIGHT); }
        //float screen_bloom_lvl10_weight() const { return float_value(WINOPTION_BLOOM_LEVEL10_WEIGHT); }
        //const char *screen_offset() const { return value(WINOPTION_OFFSET); }
        //const char *screen_scale() const { return value(WINOPTION_SCALE); }
        //const char *screen_power() const { return value(WINOPTION_POWER); }
        //const char *screen_floor() const { return value(WINOPTION_FLOOR); }
        //const char *screen_phosphor() const { return value(WINOPTION_PHOSPHOR); }
        //float screen_saturation() const { return float_value(WINOPTION_SATURATION); }


        // full screen options
        //bool triple_buffer() const { return bool_value(WINOPTION_TRIPLEBUFFER); }
        //float full_screen_brightness() const { return float_value(WINOPTION_FULLSCREENBRIGHTNESS); }
        //float full_screen_contrast() const { return float_value(WINOPTION_FULLSCREENCONTRAST); }
        //float full_screen_gamma() const { return float_value(WINOPTION_FULLSCREENGAMMA); }


        // input options
        //bool dual_lightgun() const { return bool_value(WINOPTION_DUAL_LIGHTGUN); }
    }


    public class osd_interface_Unity : osd_common_t
    {
        static int updateCount = 0;
        static int audioUpdateCount = 0;


        // master keyboard translation table
        //static const int win_key_trans_table[][4] =
        public static readonly Dictionary<KeyCode, input_item_id> keymap = new Dictionary<KeyCode, input_item_id>()
        {
            // MAME key             dinput key          virtual key     ascii

            { KeyCode.Escape, input_item_id.ITEM_ID_ESC },  //          DIK_ESCAPE,         VK_ESCAPE,      27 },

            { KeyCode.Alpha0, input_item_id.ITEM_ID_0 },
            { KeyCode.Alpha1, input_item_id.ITEM_ID_1 },
            { KeyCode.Alpha2, input_item_id.ITEM_ID_2 },
            { KeyCode.Alpha3, input_item_id.ITEM_ID_3 },
            { KeyCode.Alpha4, input_item_id.ITEM_ID_4 },
            { KeyCode.Alpha5, input_item_id.ITEM_ID_5 },
            { KeyCode.Alpha6, input_item_id.ITEM_ID_6 },
            { KeyCode.Alpha7, input_item_id.ITEM_ID_7 },
            { KeyCode.Alpha8, input_item_id.ITEM_ID_8 },
            { KeyCode.Alpha9, input_item_id.ITEM_ID_9 },

            { KeyCode.Minus,        input_item_id.ITEM_ID_MINUS },  //        DIK_MINUS,          VK_OEM_MINUS,   '-' },
            { KeyCode.Plus,         input_item_id.ITEM_ID_EQUALS },  //       DIK_EQUALS,         VK_OEM_PLUS,    '=' },
            { KeyCode.Backspace,    input_item_id.ITEM_ID_BACKSPACE },  //    DIK_BACK,           VK_BACK,        8 },
            { KeyCode.Tab,          input_item_id.ITEM_ID_TAB },  //          DIK_TAB,            VK_TAB,         9 },

            { KeyCode.Q, input_item_id.ITEM_ID_Q },
            { KeyCode.W, input_item_id.ITEM_ID_W },
            { KeyCode.E, input_item_id.ITEM_ID_E },
            { KeyCode.R, input_item_id.ITEM_ID_R },
            { KeyCode.T, input_item_id.ITEM_ID_T },
            { KeyCode.Y, input_item_id.ITEM_ID_Y },
            { KeyCode.U, input_item_id.ITEM_ID_U },
            { KeyCode.I, input_item_id.ITEM_ID_I },
            { KeyCode.O, input_item_id.ITEM_ID_O },
            { KeyCode.P, input_item_id.ITEM_ID_P },

            { KeyCode.LeftBracket,  input_item_id.ITEM_ID_OPENBRACE },  //    DIK_LBRACKET,       VK_OEM_4,       '[' },
            { KeyCode.RightBracket, input_item_id.ITEM_ID_CLOSEBRACE },  //   DIK_RBRACKET,       VK_OEM_6,       ']' },
            { KeyCode.Return,       input_item_id.ITEM_ID_ENTER },  //        DIK_RETURN,         VK_RETURN,      13 },
            { KeyCode.LeftControl,  input_item_id.ITEM_ID_LCONTROL },  //     DIK_LCONTROL,       VK_LCONTROL,    0 },

            { KeyCode.A, input_item_id.ITEM_ID_A },
            { KeyCode.S, input_item_id.ITEM_ID_S },
            { KeyCode.D, input_item_id.ITEM_ID_D },
            { KeyCode.F, input_item_id.ITEM_ID_F },
            { KeyCode.G, input_item_id.ITEM_ID_G },
            { KeyCode.H, input_item_id.ITEM_ID_H },
            { KeyCode.J, input_item_id.ITEM_ID_J },
            { KeyCode.K, input_item_id.ITEM_ID_K },
            { KeyCode.L, input_item_id.ITEM_ID_L },

            { KeyCode.Semicolon,   input_item_id.ITEM_ID_COLON },  //        DIK_SEMICOLON,      VK_OEM_1,       ';' },
            { KeyCode.Quote,       input_item_id.ITEM_ID_QUOTE },  //        DIK_APOSTROPHE,     VK_OEM_7,       '\'' },
            { KeyCode.BackQuote,   input_item_id.ITEM_ID_TILDE },  //        DIK_GRAVE,          VK_OEM_3,       '`' },
            { KeyCode.LeftShift,   input_item_id.ITEM_ID_LSHIFT },  //       DIK_LSHIFT,         VK_LSHIFT,      0 },
            { KeyCode.Backslash,   input_item_id.ITEM_ID_BACKSLASH },  //    DIK_BACKSLASH,      VK_OEM_5,       '\\' },
            //{ KeyCode., input_item_id.ITEM_ID_BACKSLASH2 },  //   DIK_OEM_102,        VK_OEM_102,     '<' },

            { KeyCode.Z, input_item_id.ITEM_ID_Z },
            { KeyCode.X, input_item_id.ITEM_ID_X },
            { KeyCode.C, input_item_id.ITEM_ID_C },
            { KeyCode.V, input_item_id.ITEM_ID_V },
            { KeyCode.B, input_item_id.ITEM_ID_B },
            { KeyCode.N, input_item_id.ITEM_ID_N },
            { KeyCode.M, input_item_id.ITEM_ID_M },

            { KeyCode.Comma,        input_item_id.ITEM_ID_COMMA },  //        DIK_COMMA,          VK_OEM_COMMA,   ',' },
            { KeyCode.Period,       input_item_id.ITEM_ID_STOP },  //         DIK_PERIOD,         VK_OEM_PERIOD,  '.' },
            { KeyCode.Question,     input_item_id.ITEM_ID_SLASH },  //        DIK_SLASH,          VK_OEM_2,       '/' },
            { KeyCode.RightShift,       input_item_id.ITEM_ID_RSHIFT },  //       DIK_RSHIFT,         VK_RSHIFT,      0 },
            //{ KeyCode., input_item_id.ITEM_ID_ASTERISK },  //     DIK_MULTIPLY,       VK_MULTIPLY,    '*' },
            { KeyCode.LeftAlt,           input_item_id.ITEM_ID_LALT },  //         DIK_LMENU,          VK_LMENU,       0 },
            { KeyCode.Space,           input_item_id.ITEM_ID_SPACE },  //        DIK_SPACE,          VK_SPACE,       ' ' },
            { KeyCode.CapsLock,         input_item_id.ITEM_ID_CAPSLOCK },  //     DIK_CAPITAL,        VK_CAPITAL,     0 },

            { KeyCode.F1,  input_item_id.ITEM_ID_F1 },
            { KeyCode.F2,  input_item_id.ITEM_ID_F2 },
            { KeyCode.F3,  input_item_id.ITEM_ID_F3 },
            { KeyCode.F4,  input_item_id.ITEM_ID_F4 },
            { KeyCode.F5,  input_item_id.ITEM_ID_F5 },
            { KeyCode.F6,  input_item_id.ITEM_ID_F6 },
            { KeyCode.F7,  input_item_id.ITEM_ID_F7 },
            { KeyCode.F8,  input_item_id.ITEM_ID_F8 },
            { KeyCode.F9,  input_item_id.ITEM_ID_F9 },
            { KeyCode.F10, input_item_id.ITEM_ID_F10 },

            { KeyCode.Numlock,         input_item_id.ITEM_ID_NUMLOCK },  //      DIK_NUMLOCK,        VK_NUMLOCK,     0 },
            { KeyCode.ScrollLock,          input_item_id.ITEM_ID_SCRLOCK },  //      DIK_SCROLL,         VK_SCROLL,      0 },
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

            { KeyCode.F11, input_item_id.ITEM_ID_F11 },
            { KeyCode.F12, input_item_id.ITEM_ID_F12 },
            { KeyCode.F13, input_item_id.ITEM_ID_F13 },
            { KeyCode.F14, input_item_id.ITEM_ID_F14 },
            { KeyCode.F15, input_item_id.ITEM_ID_F15 },

            //{ ITEM_ID_ENTER_PAD,    DIK_NUMPADENTER,    VK_RETURN,      0 },
            { KeyCode.RightControl, input_item_id.ITEM_ID_RCONTROL },  //     DIK_RCONTROL,       VK_RCONTROL,    0 },
            //{ ITEM_ID_SLASH_PAD,    DIK_DIVIDE,         VK_DIVIDE,      0 },
            //{ ITEM_ID_PRTSCR,       DIK_SYSRQ,          0,              0 },
            { KeyCode.RightAlt,       input_item_id.ITEM_ID_RALT },  //         DIK_RMENU,          VK_RMENU,       0 },
            //{ ITEM_ID_HOME,         DIK_HOME,           VK_HOME,        0 },
            { KeyCode.UpArrow,          input_item_id.ITEM_ID_UP },  //           DIK_UP,             VK_UP,          0 },
            //{ ITEM_ID_PGUP,         DIK_PRIOR,          VK_PRIOR,       0 },
            { KeyCode.LeftArrow,        input_item_id.ITEM_ID_LEFT },  //         DIK_LEFT,           VK_LEFT,        0 },
            { KeyCode.RightArrow,       input_item_id.ITEM_ID_RIGHT },  //        DIK_RIGHT,          VK_RIGHT,       0 },
            //{ ITEM_ID_END,          DIK_END,            VK_END,         0 },
            { KeyCode.DownArrow,        input_item_id.ITEM_ID_DOWN },  //         DIK_DOWN,           VK_DOWN,        0 },
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


        render_target m_target;
        MemoryContainer<byte> screenbuffer = new MemoryContainer<byte>(640 * 480 * 2 * 4); // g_state.screenbuffer = new uint32_t[400 * 400 * 2];
        public Pointer<byte> screenbufferptr;
        public Queue<Int16> m_audiobuffer = new Queue<Int16>();

        public object osdlock = new object();
        public object osdlock_audio = new object();
        public bool osdlock_screen_buffer_updated = false;

        public intref [] keyboard_state;
        public intref [] mouse_axis_state;
        public intref [] mouse_button_state;


        public osd_interface_Unity(osd_options_Unity options) : base(options)
        {
        }


        public override void init(running_machine machine)
        {
            // call our parent
            base.init(machine);

            set_verbose(true);

            //string stemp;
            osd_options_Unity options = (osd_options_Unity)machine.options();

            // determine if we are benchmarking, and adjust options appropriately
            int bench = options.bench();
            if (bench > 0)
            {
                options.set_value(OPTION_THROTTLE, 0, options_global.OPTION_PRIORITY_MAXIMUM);
                options.set_value(osd_options.OSDOPTION_SOUND, "none", options_global.OPTION_PRIORITY_MAXIMUM);
                options.set_value(osd_options.OSDOPTION_VIDEO, "none", options_global.OPTION_PRIORITY_MAXIMUM);
                options.set_value(OPTION_SECONDS_TO_RUN, bench, options_global.OPTION_PRIORITY_MAXIMUM);
            }

            // determine if we are profiling, and adjust options appropriately
            int profile = options.profile();
            if (profile > 0)
            {
                options.set_value(OPTION_THROTTLE, 0, options_global.OPTION_PRIORITY_MAXIMUM);
                options.set_value(osd_options.OSDOPTION_NUMPROCESSORS, 1, options_global.OPTION_PRIORITY_MAXIMUM);
            }

#if false
            // thread priority
            if ((machine.debug_flags_get() & running_machine.DEBUG_FLAG_OSD_ENABLED) == 0)
                SetThreadPriority(GetCurrentThread(), options.priority());
#endif

            // get number of processors
            //stemp = options.numprocessors();

#if false
            osd_num_processors = 0;

            if (stemp != "auto")
            {
                osd_num_processors = Convert.ToInt32(stemp);
                if (osd_num_processors < 1)
                {
                    osdcore_global.m_osdcore.osd_printf_warning("Warning: numprocessors < 1 doesn't make much sense. Assuming auto ...\n");
                    osd_num_processors = 0;
                }
            }
#endif

            // initialize the subsystems
            init_subsystems();

#if false
            // notify listeners of screen configuration
            string tempstring;
            for (win_window_info info = win_window_list; info != null; info = info.m_next)
            {
                string tmp = utf8_from_tstring(info.m_monitor.info.szDevice);
                string tempstring = string.Format("Orientation({0})", tmp);
                output_set_value(tempstring, info.m_targetorient);
                //osd_free(tmp);
            }
#endif

#if false
            // hook up the debugger log
            if (options.oslog())
                machine.add_logerror_callback(output_oslog);
#endif

#if false
            // crank up the multimedia timer resolution to its max
            // this gives the system much finer timeslices
            timeresult = timeGetDevCaps(&timecaps, sizeof(timecaps));
            if (timeresult == TIMERR_NOERROR)
                timeBeginPeriod(timecaps.wPeriodMin);
#endif

#if false
            // if a watchdog thread is requested, create one
            int watchdog = options.watchdog();
            if (watchdog != 0)
            {
                watchdog_reset_event = CreateEvent(NULL, FALSE, FALSE, NULL);
                emucore_global.assert_always(watchdog_reset_event != null, "Failed to create watchdog reset event");
                watchdog_exit_event = CreateEvent(NULL, TRUE, FALSE, NULL);
                emucore_global.assert_always(watchdog_exit_event != null, "Failed to create watchdog exit event");
                watchdog_thread = CreateThread(NULL, 0, watchdog_thread_entry, (LPVOID)(FPTR)watchdog, 0, NULL);
                emucore_global.assert_always(watchdog_thread != null, "Failed to create watchdog thread");
            }
#endif

#if false
            // create and start the profiler
            if (profile > 0)
            {
                profiler = new sampling_profiler(1000, profile - 1));
                profiler.start();
            }
#endif

#if false
            // initialize sockets
            win_init_sockets();
#endif

#if false
            // note the existence of a machine
            g_current_machine = &machine; 
#endif


            /////////////////////////////////////////////
            // custom code below

            validity_checker valid = new validity_checker(machine.options(), false);
            string sysname = machine.options().system_name();
            bool result = valid.check_all_matching(string.IsNullOrEmpty(sysname) ? "*" : sysname);
            if (!result)
                throw new emu_fatalerror((int)main_global.EMU_ERR_FAILED_VALIDITY, "Validity check failed ({0} errors, {1} warnings in total)\n", valid.errors(), valid.warnings());

            /**
             *  Save away the machine, we'll need it in osd_customize_input_type_list
             **/
            //g_state.machine = machine;
    
            /**
             * Create the render_target that tells MAME the rendering parameters it
             * will use.
             **/
            m_target = machine.render().target_alloc();


            /**
             * Have this target hold every view since we only support one target
             **/
            m_target.set_view(m_target.configured_view("auto", 0, 1));

            /**
             * Set render target bounds to 10000 x 10000 and allow the callback to
             * scale that to whatever they want.
             **/
            //m_target.set_bounds(640, 480, 1.0f);
            m_target.set_bounds(400, 400, 1.0f);


            screenbufferptr = new Pointer<byte>(screenbuffer);


            {
                keyboard_state = new intref[(int)input_item_id.ITEM_ID_ABSOLUTE_MAXIMUM];
                for (int i = 0; i < (int)input_item_id.ITEM_ID_ABSOLUTE_MAXIMUM; i++)
                    keyboard_state[i] = new intref();

                input_device keyboard_device;
                keyboard_device = machine.input().device_class(input_device_class.DEVICE_CLASS_KEYBOARD).add_device("Keyboard", "Keyboard0");
                if (keyboard_device == null)
                    throw new emu_fatalerror("osd_interface.init() - FAILED - add_device() failed\n");

                foreach (var entry in keymap)
                {
                    string defname = entry.Key.ToString();  //string.Format("Scan{0}", count++);
                    input_item_id itemid = entry.Value;
                    keyboard_device.add_item(defname, itemid, keyboard_get_state, keyboard_state[(int)itemid]);
                }
            }


            {
                mouse_axis_state = new intref[2];
                for (int i = 0; i < 2; i++)
                    mouse_axis_state[i] = new intref();

                input_device mouse_device;
                mouse_device = machine.input().device_class(input_device_class.DEVICE_CLASS_MOUSE).add_device("Mouse", "Mouse0");
                if (mouse_device == null)
                    throw new emu_fatalerror("osd_interface.init() - FAILED - add_device() failed\n");

                string defname;
                defname = string.Format("X {0}", mouse_device.name());
                mouse_device.add_item(defname, input_item_id.ITEM_ID_XAXIS, mouse_axis_get_state, mouse_axis_state[0]);
                defname = string.Format("Y {0}", mouse_device.name());
                mouse_device.add_item(defname, input_item_id.ITEM_ID_YAXIS, mouse_axis_get_state, mouse_axis_state[1]);


                mouse_button_state = new intref[5];
                for (int i = 0; i < 5; i++)
                    mouse_button_state[i] = new intref();

                defname = string.Format("B1");
                mouse_device.add_item(defname, input_item_id.ITEM_ID_BUTTON1, mouse_button_get_state, mouse_button_state[0]);
                defname = string.Format("B2");
                mouse_device.add_item(defname, input_item_id.ITEM_ID_BUTTON2, mouse_button_get_state, mouse_button_state[1]);
                defname = string.Format("B3");
                mouse_device.add_item(defname, input_item_id.ITEM_ID_BUTTON3, mouse_button_get_state, mouse_button_state[2]);
                defname = string.Format("B4");
                mouse_device.add_item(defname, input_item_id.ITEM_ID_BUTTON4, mouse_button_get_state, mouse_button_state[3]);
                defname = string.Format("B5");
                mouse_device.add_item(defname, input_item_id.ITEM_ID_BUTTON5, mouse_button_get_state, mouse_button_state[4]);
            }


            //System.Windows.Forms.Application.Run(new mameForm.Form1());
            //System.Windows.Forms.Form mainForm = new mameForm.Form1();
            //mainForm.FormClosed += (_sender, _args) => { System.Windows.Forms.Application.ExitThread(); };
            //mainForm.Show();

            //Console.WriteLine("After Show()");
        }


        public override void update(bool skip_redraw)
        {
            base.update(skip_redraw);

            //System.Windows.Forms.Application.DoEvents();

            lock (osdlock)
            {
                if (m_exitScheduled)
                {
                    osd_printf_verbose("osd_interface.update() - calling schedule_exit()");
                    machine().schedule_exit();
                }
            }


            if (updateCount++ % 50 == 0)
                osd_printf_verbose("osd_interface.update() - {0}", updateCount);


            if (!skip_redraw)
            {
                render_primitive_list list = m_target.get_primitives();
                list.acquire_lock();

                UInt32 width = 400;  //640; //rect_width(&bounds);
                UInt32 height = 400; //480; //rect_height(&bounds);
                UInt32 pitch = (UInt32)((width + 3) & ~3);


                lock (osdlock)
                {
                    //software_renderer<typeof(UInt32), 0,0,0, 16,8,0>.draw_primitives(list, g_state.screenbuffer, width, height, pitch);
                    software_renderer<UInt32,  int_const_0, int_const_0, int_const_0,  int_const_16, int_const_8, int_const_0>.draw_primitives(list, screenbufferptr, width, height, pitch);

                    osdlock_screen_buffer_updated = true;
                }

                list.release_lock();
            }
        }


        bool m_exitScheduled = false;

        public void ScheduleExit()
        {
            m_exitScheduled = true;
        }


        public override void input_update()
        {
        }


        public override void update_audio_stream(Pointer<Int16> buffer, int samples_this_frame)  //const int16_t *buffer, int samples_this_frame) = 0;
        {
            base.update_audio_stream(buffer, samples_this_frame);

            if (audioUpdateCount++ % 10 == 0)
                osd_printf_verbose("osd_interface.update_audio_stream() - {0} - samples: {1}\n", audioUpdateCount, samples_this_frame);

            lock (osdlock_audio)
            {
                for (int i = 0; i < samples_this_frame; i++)
                    m_audiobuffer.Enqueue(buffer[i]);
            }
        }


        public override void set_mastervolume(int attenuation)
        {
            base.set_mastervolume(attenuation);
        }


        public override bool no_sound()
        {
            return base.no_sound();
        }


        public override void customize_input_type_list(std.vector<input_type_entry> typelist)
        {
            base.customize_input_type_list(typelist);
        }


        public override std.vector<mame.ui.menu_item> get_slider_list()
        {
            return base.get_slider_list();
        }


        public void ui_input_push_mouse_move_event(int x, int y) { machine().ui_input().push_mouse_move_event(m_target, x, y); }
        public void ui_input_push_mouse_leave_event() { machine().ui_input().push_mouse_leave_event(m_target); }
        public void ui_input_push_mouse_down_event(int x, int y) { machine().ui_input().push_mouse_down_event(m_target, x, y); }
        public void ui_input_push_mouse_up_event(int x, int y) { machine().ui_input().push_mouse_up_event(m_target, x, y); }
        public void ui_input_push_mouse_double_click_event(int x, int y) { machine().ui_input().push_mouse_double_click_event(m_target, x, y); }
        public void ui_input_push_char_event(char ch) { machine().ui_input().push_char_event(m_target, ch); }


        //============================================================
        //  keyboard_get_state
        //============================================================
        int keyboard_get_state(object device_internal, object item_internal)
        {
            // this function is called by the input system to get the current key
            // state; it is specified as a callback when adding items to input
            // devices
            //UINT8 *keystate = (UINT8 *)item_internal;
            //return *keystate;

            // might want to put this check into an assert() instead
            if (item_internal is intref)
            {
                intref item = (intref)item_internal;

                if (item.i == 1) osd_printf_info("keyboard_get_state() - Pressed");

                return item.i;
            }

            return 0;
        }


        //============================================================
        //  generic_axis_get_state
        //============================================================
        int mouse_axis_get_state(object device_internal, object item_internal)
        {
            //INT32 *axisdata = (INT32 *) item_internal;
            // return the current state
            //return *axisdata;

            // might want to put this check into an assert() instead
            if (item_internal is intref)
            {
                intref item = (intref)item_internal;

                Console.WriteLine("mouse_axis_get_state() - {0}", item.i);

                return item.i;
            }

            return 0;
        }


        //============================================================
        //  generic_button_get_state
        //============================================================
        int mouse_button_get_state(object device_internal, object item_internal)
        {
            //device_info *devinfo = (device_info *)device_internal;
            //BYTE *itemdata = (BYTE *)item_internal;
            // return the current state
            //poll_if_necessary(devinfo->machine());
            //return *itemdata >> 7;

            // might want to put this check into an assert() instead
            if (item_internal is intref)
            {
                intref item = (intref)item_internal;

                Console.WriteLine("mouse_button_get_state() - {0}", item.i);

                return item.i;
            }

            return 0;
        }


        //-------------------------------------------------
        //  output_callback  - callback for osd_printf_...
        //-------------------------------------------------
        public override void output_callback(osd_output_channel channel, string format, params object [] args)
        {
            switch (channel)
            {
                case osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR:
                case osd_output_channel.OSD_OUTPUT_CHANNEL_WARNING:
                    Debug.LogFormat(format, args);  //vfprintf(stderr, msg, args);
                    //System.Diagnostics.Debug.Write(str);
                    break;
                case osd_output_channel.OSD_OUTPUT_CHANNEL_INFO:
                case osd_output_channel.OSD_OUTPUT_CHANNEL_LOG:
                    Debug.LogFormat(format, args);  //vfprintf(stdout, msg, args);
                    //System.Diagnostics.Debug.Write(str);
                    break;
                case osd_output_channel.OSD_OUTPUT_CHANNEL_VERBOSE:
                    if (verbose())
                    {
                        Debug.LogFormat(format, args);  //vfprintf(stdout, msg, args);
                        //System.Diagnostics.Debug.Write(str);
                    }
                    break;
                case osd_output_channel.OSD_OUTPUT_CHANNEL_DEBUG:
//#if MAME_DEBUG
                    Debug.LogFormat(format, args);  //vfprintf(stdout, msg, args);
                    //System.Diagnostics.Debug.Write(str);
//#endif
                    break;
                default:
                    break;
            }
        }
    }
}
