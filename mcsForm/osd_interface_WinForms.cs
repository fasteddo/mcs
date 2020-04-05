// license:BSD-3-Clause
// copyright-holders:Edward Fast

using mame;
using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;


namespace mameForm
{
    public class osd_options_WinForms : osd_options  // windows_options
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
        const string WINOPTION_HLSLPATH                  = "hlslpath";
        const string WINOPTION_HLSL_ENABLE               = "hlsl_enable";
        const string WINOPTION_HLSL_OVERSAMPLING         = "hlsl_oversampling";
        const string WINOPTION_HLSL_WRITE                = "hlsl_write";
        const string WINOPTION_HLSL_SNAP_WIDTH           = "hlsl_snap_width";
        const string WINOPTION_HLSL_SNAP_HEIGHT          = "hlsl_snap_height";
        const string WINOPTION_SHADOW_MASK_TILE_MODE     = "shadow_mask_tile_mode";
        const string WINOPTION_SHADOW_MASK_ALPHA         = "shadow_mask_alpha";
        const string WINOPTION_SHADOW_MASK_TEXTURE       = "shadow_mask_texture";
        const string WINOPTION_SHADOW_MASK_COUNT_X       = "shadow_mask_x_count";
        const string WINOPTION_SHADOW_MASK_COUNT_Y       = "shadow_mask_y_count";
        const string WINOPTION_SHADOW_MASK_USIZE         = "shadow_mask_usize";
        const string WINOPTION_SHADOW_MASK_VSIZE         = "shadow_mask_vsize";
        const string WINOPTION_SHADOW_MASK_UOFFSET       = "shadow_mask_uoffset";
        const string WINOPTION_SHADOW_MASK_VOFFSET       = "shadow_mask_voffset";
        const string WINOPTION_REFLECTION                = "reflection";
        const string WINOPTION_DISTORTION                = "distortion";
        const string WINOPTION_CUBIC_DISTORTION          = "cubic_distortion";
        const string WINOPTION_DISTORT_CORNER            = "distort_corner";
        const string WINOPTION_ROUND_CORNER              = "round_corner";
        const string WINOPTION_SMOOTH_BORDER             = "smooth_border";
        const string WINOPTION_VIGNETTING                = "vignetting";
        const string WINOPTION_SCANLINE_AMOUNT           = "scanline_alpha";
        const string WINOPTION_SCANLINE_SCALE            = "scanline_size";
        const string WINOPTION_SCANLINE_HEIGHT           = "scanline_height";
        const string WINOPTION_SCANLINE_VARIATION        = "scanline_variation";
        const string WINOPTION_SCANLINE_BRIGHT_SCALE     = "scanline_bright_scale";
        const string WINOPTION_SCANLINE_BRIGHT_OFFSET    = "scanline_bright_offset";
        const string WINOPTION_SCANLINE_JITTER           = "scanline_jitter";
        const string WINOPTION_HUM_BAR_ALPHA             = "hum_bar_alpha";
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
        const string WINOPTION_CHROMA_MODE               = "chroma_mode";
        const string WINOPTION_CHROMA_CONVERSION_GAIN    = "chroma_conversion_gain";
        const string WINOPTION_CHROMA_A                  = "chroma_a";
        const string WINOPTION_CHROMA_B                  = "chroma_b";
        const string WINOPTION_CHROMA_C                  = "chroma_c";
        const string WINOPTION_CHROMA_Y_GAIN             = "chroma_y_gain";
        const string WINOPTION_YIQ_ENABLE                = "yiq_enable";
        const string WINOPTION_YIQ_JITTER                = "yiq_jitter";
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
        const string WINOPTION_VECTOR_BEAM_SMOOTH        = "vector_beam_smooth";
        const string WINOPTION_VECTOR_LENGTH_SCALE       = "vector_length_scale";
        const string WINOPTION_VECTOR_LENGTH_RATIO       = "vector_length_ratio";
        const string WINOPTION_BLOOM_BLEND_MODE          = "bloom_blend_mode";
        const string WINOPTION_BLOOM_SCALE               = "bloom_scale";
        const string WINOPTION_BLOOM_OVERDRIVE           = "bloom_overdrive";
        const string WINOPTION_BLOOM_LEVEL0_WEIGHT       = "bloom_lvl0_weight";
        const string WINOPTION_BLOOM_LEVEL1_WEIGHT       = "bloom_lvl1_weight";
        const string WINOPTION_BLOOM_LEVEL2_WEIGHT       = "bloom_lvl2_weight";
        const string WINOPTION_BLOOM_LEVEL3_WEIGHT       = "bloom_lvl3_weight";
        const string WINOPTION_BLOOM_LEVEL4_WEIGHT       = "bloom_lvl4_weight";
        const string WINOPTION_BLOOM_LEVEL5_WEIGHT       = "bloom_lvl5_weight";
        const string WINOPTION_BLOOM_LEVEL6_WEIGHT       = "bloom_lvl6_weight";
        const string WINOPTION_BLOOM_LEVEL7_WEIGHT       = "bloom_lvl7_weight";
        const string WINOPTION_BLOOM_LEVEL8_WEIGHT       = "bloom_lvl8_weight";
        const string WINOPTION_LUT_TEXTURE               = "lut_texture";
        const string WINOPTION_LUT_ENABLE                = "lut_enable";
        const string WINOPTION_UI_LUT_TEXTURE            = "ui_lut_texture";
        const string WINOPTION_UI_LUT_ENABLE             = "ui_lut_enable";

        // full screen options
        const string WINOPTION_TRIPLEBUFFER          = "triplebuffer";
        const string WINOPTION_FULLSCREENBRIGHTNESS  = "full_screen_brightness";
        const string WINOPTION_FULLSCREENCONTRAST    = "full_screen_contrast";
        const string WINOPTION_FULLSCREENGAMMA       = "full_screen_gamma";

        // input options
        const string WINOPTION_GLOBAL_INPUTS         = "global_inputs";
        const string WINOPTION_DUAL_LIGHTGUN         = "dual_lightgun";


        static readonly options_entry [] s_option_entries = new options_entry []
        {
            // debugging options
            new options_entry(null,                                           null,       OPTION_HEADER,     "WINDOWS DEBUGGING OPTIONS"),
            new options_entry(WINOPTION_DEBUGGER_FONT + ";dfont",             "Lucida Console", OPTION_STRING,"specifies the font to use for debugging; defaults to Lucida Console"),
            new options_entry(WINOPTION_DEBUGGER_FONT_SIZE + ";dfontsize",    "9",        OPTION_FLOAT,      "specifies the font size to use for debugging; defaults to 9 pt"),

            // performance options
            new options_entry(null,                                           null,       OPTION_HEADER,     "WINDOWS PERFORMANCE OPTIONS"),
            new options_entry(WINOPTION_PRIORITY + "(-15-1)",                 "0",        OPTION_INTEGER,    "thread priority for the main game thread; range from -15 to 1"),
            new options_entry(WINOPTION_PROFILE,                              "0",        OPTION_INTEGER,    "enable profiling, specifying the stack depth to track"),

            // video options
            new options_entry(null,                                           null,       OPTION_HEADER,     "WINDOWS VIDEO OPTIONS"),
            new options_entry(WINOPTION_PRESCALE,                             "1",        OPTION_INTEGER,    "scale screen rendering by this amount in software"),
            new options_entry(WINOPTION_MENU,                                 "0",        OPTION_BOOLEAN,    "enable menu bar if available by UI implementation"),

            // DirectDraw-specific options
            new options_entry(null,                                           null,       OPTION_HEADER,     "DIRECTDRAW-SPECIFIC OPTIONS"),
            new options_entry(WINOPTION_HWSTRETCH + ";hws",                   "1",        OPTION_BOOLEAN,    "enable hardware stretching"),

            // Direct3D-specific options
            new options_entry(null,                                           null,       OPTION_HEADER,     "DIRECT3D-SPECIFIC OPTIONS"),
            new options_entry(WINOPTION_FILTER + ";d3dfilter;flt",            "1",        OPTION_BOOLEAN,    "enable bilinear filtering on screen output"),

            // post-processing options
            new options_entry(null,                                                     null,        OPTION_HEADER,     "DIRECT3D POST-PROCESSING OPTIONS"),
            new options_entry(WINOPTION_HLSLPATH,                                       "hlsl",      OPTION_STRING,     "path to hlsl files"),
            new options_entry(WINOPTION_HLSL_ENABLE + ";hlsl",                          "0",         OPTION_BOOLEAN,    "enable HLSL post-processing (PS3.0 required)"),
            new options_entry(WINOPTION_HLSL_OVERSAMPLING,                              "0",         OPTION_BOOLEAN,    "enable HLSL oversampling"),
            new options_entry(WINOPTION_HLSL_WRITE,                                     OSDOPTVAL_AUTO,OPTION_STRING,   "enable HLSL AVI writing (huge disk bandwidth suggested)"),
            new options_entry(WINOPTION_HLSL_SNAP_WIDTH,                                "2048",      OPTION_STRING,     "HLSL upscaled-snapshot width"),
            new options_entry(WINOPTION_HLSL_SNAP_HEIGHT,                               "1536",      OPTION_STRING,     "HLSL upscaled-snapshot height"),
            new options_entry(WINOPTION_SHADOW_MASK_TILE_MODE,                          "0",         OPTION_INTEGER,    "shadow mask tile mode (0 for screen based, 1 for source based)"),
            new options_entry(WINOPTION_SHADOW_MASK_ALPHA + ";fs_shadwa(0.0-1.0)",      "0.0",       OPTION_FLOAT,      "shadow mask alpha-blend value (1.0 is fully blended, 0.0 is no mask)"),
            new options_entry(WINOPTION_SHADOW_MASK_TEXTURE + ";fs_shadwt(0.0-1.0)",    "aperture.png", OPTION_STRING,  "shadow mask texture name"),
            new options_entry(WINOPTION_SHADOW_MASK_COUNT_X + ";fs_shadww",             "320",       OPTION_INTEGER,    "shadow mask width, in phosphor dots"),
            new options_entry(WINOPTION_SHADOW_MASK_COUNT_Y + ";fs_shadwh",             "240",       OPTION_INTEGER,    "shadow mask height, in phosphor dots"),
            new options_entry(WINOPTION_SHADOW_MASK_USIZE + ";fs_shadwu(0.0-1.0)",      "0.09375",   OPTION_FLOAT,      "shadow mask texture size in U direction"),
            new options_entry(WINOPTION_SHADOW_MASK_VSIZE + ";fs_shadwv(0.0-1.0)",      "0.109375",  OPTION_FLOAT,      "shadow mask texture size in V direction"),
            new options_entry(WINOPTION_SHADOW_MASK_UOFFSET + ";fs_shadwou(-1.0-1.0)",  "0.0",       OPTION_FLOAT,      "shadow mask texture offset, in U direction"),
            new options_entry(WINOPTION_SHADOW_MASK_VOFFSET + ";fs_shadwov(-1.0-1.0)",  "0.0",       OPTION_FLOAT,      "shadow mask texture offset, in V direction"),
            new options_entry(WINOPTION_DISTORTION + ";fs_dist(-1.0-1.0)",              "0.0",       OPTION_FLOAT,      "screen distortion amount"),
            new options_entry(WINOPTION_CUBIC_DISTORTION + ";fs_cubedist(-1.0-1.0)",    "0.0",       OPTION_FLOAT,      "screen cubic distortion amount"),
            new options_entry(WINOPTION_DISTORT_CORNER + ";fs_distc(0.0-1.0)",          "0.0",       OPTION_FLOAT,      "screen distort corner amount"),
            new options_entry(WINOPTION_ROUND_CORNER + ";fs_rndc(0.0-1.0)",             "0.0",       OPTION_FLOAT,      "screen round corner amount"),
            new options_entry(WINOPTION_SMOOTH_BORDER + ";fs_smob(0.0-1.0)",            "0.0",       OPTION_FLOAT,      "screen smooth border amount"),
            new options_entry(WINOPTION_REFLECTION + ";fs_ref(0.0-1.0)",                "0.0",       OPTION_FLOAT,      "screen reflection amount"),
            new options_entry(WINOPTION_VIGNETTING + ";fs_vig(0.0-1.0)",                "0.0",       OPTION_FLOAT,      "image vignetting amount"),
            /* Beam-related values below this line*/
            new options_entry(WINOPTION_SCANLINE_AMOUNT + ";fs_scanam(0.0-4.0)",        "0.0",       OPTION_FLOAT,      "overall alpha scaling value for scanlines"),
            new options_entry(WINOPTION_SCANLINE_SCALE + ";fs_scansc(0.0-4.0)",         "1.0",       OPTION_FLOAT,      "overall height scaling value for scanlines"),
            new options_entry(WINOPTION_SCANLINE_HEIGHT + ";fs_scanh(0.0-4.0)",         "1.0",       OPTION_FLOAT,      "individual height scaling value for scanlines"),
            new options_entry(WINOPTION_SCANLINE_VARIATION + ";fs_scanv(0.0-4.0)",      "1.0",       OPTION_FLOAT,      "individual height varying value for scanlines"),
            new options_entry(WINOPTION_SCANLINE_BRIGHT_SCALE + ";fs_scanbs(0.0-2.0)",  "1.0",       OPTION_FLOAT,      "overall brightness scaling value for scanlines (multiplicative)"),
            new options_entry(WINOPTION_SCANLINE_BRIGHT_OFFSET + ";fs_scanbo(0.0-1.0)", "0.0",       OPTION_FLOAT,      "overall brightness offset value for scanlines (additive)"),
            new options_entry(WINOPTION_SCANLINE_JITTER + ";fs_scanjt(0.0-4.0)",        "0.0",       OPTION_FLOAT,      "overall interlace jitter scaling value for scanlines"),
            new options_entry(WINOPTION_HUM_BAR_ALPHA + ";fs_humba(0.0-1.0)",           "0.0",       OPTION_FLOAT,      "overall alpha scaling value for hum bar"),
            new options_entry(WINOPTION_DEFOCUS + ";fs_focus",                          "0.0,0.0",   OPTION_STRING,     "overall defocus value in screen-relative coords"),
            new options_entry(WINOPTION_CONVERGE_X + ";fs_convx",                       "0.0,0.0,0.0",OPTION_STRING,    "convergence in screen-relative X direction"),
            new options_entry(WINOPTION_CONVERGE_Y + ";fs_convy",                       "0.0,0.0,0.0",OPTION_STRING,    "convergence in screen-relative Y direction"),
            new options_entry(WINOPTION_RADIAL_CONVERGE_X + ";fs_rconvx",               "0.0,0.0,0.0",OPTION_STRING,    "radial convergence in screen-relative X direction"),
            new options_entry(WINOPTION_RADIAL_CONVERGE_Y + ";fs_rconvy",               "0.0,0.0,0.0",OPTION_STRING,    "radial convergence in screen-relative Y direction"),
            /* RGB colorspace convolution below this line */
            new options_entry(WINOPTION_RED_RATIO + ";fs_redratio",                     "1.0,0.0,0.0",OPTION_STRING,    "red output signal generated by input signal"),
            new options_entry(WINOPTION_GRN_RATIO + ";fs_grnratio",                     "0.0,1.0,0.0",OPTION_STRING,    "green output signal generated by input signal"),
            new options_entry(WINOPTION_BLU_RATIO + ";fs_bluratio",                     "0.0,0.0,1.0",OPTION_STRING,    "blue output signal generated by input signal"),
            new options_entry(WINOPTION_SATURATION + ";fs_sat(0.0-4.0)",                "1.4",        OPTION_FLOAT,     "saturation scaling value"),
            new options_entry(WINOPTION_OFFSET + ";fs_offset",                          "0.0,0.0,0.0",OPTION_STRING,    "signal offset value (additive)"),
            new options_entry(WINOPTION_SCALE + ";fs_scale",                            "0.95,0.95,0.95",OPTION_STRING, "signal scaling value (multiplicative)"),
            new options_entry(WINOPTION_POWER + ";fs_power",                            "0.8,0.8,0.8",OPTION_STRING,    "signal power value (exponential)"),
            new options_entry(WINOPTION_FLOOR + ";fs_floor",                            "0.05,0.05,0.05",OPTION_STRING, "signal floor level"),
            new options_entry(WINOPTION_PHOSPHOR + ";fs_phosphor",                      "0.4,0.4,0.4",OPTION_STRING,    "phosphorescence decay rate (0.0 is instant, 1.0 is forever)"),
            new options_entry(WINOPTION_CHROMA_MODE,                                    "3",                 OPTION_INTEGER,    "number of phosphors to use: 1 - monochrome, 2 - dichrome, 3 - trichrome (color)"),
            new options_entry(WINOPTION_CHROMA_CONVERSION_GAIN,                         "0.299,0.587,0.114", OPTION_STRING,     "gain to be applied when summing RGB signal for monochrome and dichrome modes"),
            new options_entry(WINOPTION_CHROMA_A,                                       "0.64,0.33",         OPTION_STRING,     "chromaticity coordinate for first phosphor"),
            new options_entry(WINOPTION_CHROMA_B,                                       "0.30,0.60",         OPTION_STRING,     "chromaticity coordinate for second phosphor"),
            new options_entry(WINOPTION_CHROMA_C,                                       "0.15,0.06",         OPTION_STRING,     "chromaticity coordinate for third phosphor"),
            new options_entry(WINOPTION_CHROMA_Y_GAIN,                                  "0.2126,0.7152,0.0722", OPTION_STRING,  "gain to be applied for each phosphor"),
            /* NTSC simulation below this line */
            new options_entry(null,                                                     null,        OPTION_HEADER,     "NTSC POST-PROCESSING OPTIONS"),
            new options_entry(WINOPTION_YIQ_ENABLE + ";yiq",                            "0",         OPTION_BOOLEAN,    "enable YIQ-space HLSL post-processing"),
            new options_entry(WINOPTION_YIQ_JITTER + ";yiqj",                           "0.0",       OPTION_FLOAT,      "jitter for the NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_CCVALUE + ";yiqcc",                         "3.59754545",OPTION_FLOAT,      "color Carrier frequency for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_AVALUE + ";yiqa",                           "0.5",       OPTION_FLOAT,      "A value for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_BVALUE + ";yiqb",                           "0.5",       OPTION_FLOAT,      "B value for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_OVALUE + ";yiqo",                           "1.570796325",OPTION_FLOAT,     "outgoing Color Carrier phase offset for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_PVALUE + ";yiqp",                           "1.0",       OPTION_FLOAT,      "incoming Pixel Clock scaling value for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_NVALUE + ";yiqn",                           "1.0",       OPTION_FLOAT,      "Y filter notch width for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_YVALUE + ";yiqy",                           "6.0",       OPTION_FLOAT,      "Y filter cutoff frequency for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_IVALUE + ";yiqi",                           "1.2",       OPTION_FLOAT,      "I filter cutoff frequency for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_QVALUE + ";yiqq",                           "0.6",       OPTION_FLOAT,      "Q filter cutoff frequency for NTSC signal processing"),
            new options_entry(WINOPTION_YIQ_SCAN_TIME + ";yiqsc",                       "52.6",      OPTION_FLOAT,      "horizontal scanline duration for NTSC signal processing (in usec)"),
            new options_entry(WINOPTION_YIQ_PHASE_COUNT + ";yiqp",                      "2",         OPTION_INTEGER,    "phase count value for NTSC signal processing"),
            /* Vector simulation below this line */
            new options_entry(null,                                                     null,        OPTION_HEADER,     "VECTOR POST-PROCESSING OPTIONS"),
            new options_entry(WINOPTION_VECTOR_BEAM_SMOOTH + ";vecsmooth",              "0.0",       OPTION_FLOAT,      "vector beam smoothness"),
            new options_entry(WINOPTION_VECTOR_LENGTH_SCALE + ";veclength",             "0.8",       OPTION_FLOAT,      "maximum vector attenuation"),
            new options_entry(WINOPTION_VECTOR_LENGTH_RATIO + ";vecsize",               "500.0",     OPTION_FLOAT,      "minimum vector length affected by attenuation (vector length to screen size ratio)"),
            /* Bloom below this line */
            new options_entry(null,                                                     null,        OPTION_HEADER,     "BLOOM POST-PROCESSING OPTIONS"),
            new options_entry(WINOPTION_BLOOM_BLEND_MODE,                               "0",                 OPTION_INTEGER,    "bloom blend mode (0 for brighten, 1 for darken)"),
            new options_entry(WINOPTION_BLOOM_SCALE,                                    "0.0",               OPTION_FLOAT,      "intensity factor for bloom"),
            new options_entry(WINOPTION_BLOOM_OVERDRIVE,                                "1.0,1.0,1.0",       OPTION_STRING,     "overdrive factor for bloom"),
            new options_entry(WINOPTION_BLOOM_LEVEL0_WEIGHT,                            "1.0",               OPTION_FLOAT,      "bloom level 0 weight (full-size target)"),
            new options_entry(WINOPTION_BLOOM_LEVEL1_WEIGHT,                            "0.64",              OPTION_FLOAT,      "bloom level 1 weight (1/4 smaller that level 0 target)"),
            new options_entry(WINOPTION_BLOOM_LEVEL2_WEIGHT,                            "0.32",              OPTION_FLOAT,      "bloom level 2 weight (1/4 smaller that level 1 target)"),
            new options_entry(WINOPTION_BLOOM_LEVEL3_WEIGHT,                            "0.16",              OPTION_FLOAT,      "bloom level 3 weight (1/4 smaller that level 2 target)"),
            new options_entry(WINOPTION_BLOOM_LEVEL4_WEIGHT,                            "0.08",              OPTION_FLOAT,      "bloom level 4 weight (1/4 smaller that level 3 target)"),
            new options_entry(WINOPTION_BLOOM_LEVEL5_WEIGHT,                            "0.06",              OPTION_FLOAT,      "bloom level 5 weight (1/4 smaller that level 4 target)"),
            new options_entry(WINOPTION_BLOOM_LEVEL6_WEIGHT,                            "0.04",              OPTION_FLOAT,      "bloom level 6 weight (1/4 smaller that level 5 target)"),
            new options_entry(WINOPTION_BLOOM_LEVEL7_WEIGHT,                            "0.02",              OPTION_FLOAT,      "bloom level 7 weight (1/4 smaller that level 6 target)"),
            new options_entry(WINOPTION_BLOOM_LEVEL8_WEIGHT,                            "0.01",              OPTION_FLOAT,      "bloom level 8 weight (1/4 smaller that level 7 target)"),
            new options_entry(WINOPTION_LUT_TEXTURE,                                    "",                  OPTION_STRING,     "3D LUT texture filename for screen, PNG format"),
            new options_entry(WINOPTION_LUT_ENABLE,                                     "0",                 OPTION_BOOLEAN,    "Enables 3D LUT to be applied to screen after post-processing"),
            new options_entry(WINOPTION_UI_LUT_TEXTURE,                                 "",                  OPTION_STRING,     "3D LUT texture filename of UI, PNG format"),
            new options_entry(WINOPTION_UI_LUT_ENABLE,                                  "0",                 OPTION_BOOLEAN,    "enable 3D LUT to be applied to UI and artwork after post-processing"),

            // full screen options
            new options_entry(null,                                             null,       OPTION_HEADER,     "FULL SCREEN OPTIONS"),
            new options_entry(WINOPTION_TRIPLEBUFFER + ";tb",                   "0",        OPTION_BOOLEAN,    "enable triple buffering"),
            new options_entry(WINOPTION_FULLSCREENBRIGHTNESS + ";fsb(0.1-2.0)", "1.0",      OPTION_FLOAT,      "brightness value in full screen mode"),
            new options_entry(WINOPTION_FULLSCREENCONTRAST + ";fsc(0.1-2.0)",   "1.0",      OPTION_FLOAT,      "contrast value in full screen mode"),
            new options_entry(WINOPTION_FULLSCREENGAMMA + ";fsg(0.1-3.0)",      "1.0",      OPTION_FLOAT,      "gamma value in full screen mode"),

            // input options
            new options_entry(null,                                             null,       OPTION_HEADER,     "INPUT DEVICE OPTIONS"),
            new options_entry(WINOPTION_GLOBAL_INPUTS,                          "0",        OPTION_BOOLEAN,    "enable global inputs"),
            new options_entry(WINOPTION_DUAL_LIGHTGUN + ";dual",                "0",        OPTION_BOOLEAN,    "enable dual lightgun input"),

            new options_entry(null),
        };


        // construction/destruction
        //============================================================
        //  windows_options
        //============================================================
        public osd_options_WinForms() : base()
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
        //int screen_chroma_mode() const { return int_value(WINOPTION_CHROMA_MODE); }
        //const char *screen_chroma_a() const { return value(WINOPTION_CHROMA_A); }
        //const char *screen_chroma_b() const { return value(WINOPTION_CHROMA_B); }
        //const char *screen_chroma_c() const { return value(WINOPTION_CHROMA_C); }
        //const char *screen_chroma_conversion_gain() const { return value(WINOPTION_CHROMA_CONVERSION_GAIN); }
        //const char *screen_chroma_y_gain() const { return value(WINOPTION_CHROMA_Y_GAIN); }
        //const char *screen_lut_texture() const { return value(WINOPTION_LUT_TEXTURE); }
        //bool screen_lut_enable() const { return bool_value(WINOPTION_LUT_ENABLE); }
        //const char *ui_lut_texture() const { return value(WINOPTION_UI_LUT_TEXTURE); }
        //bool ui_lut_enable() const { return bool_value(WINOPTION_UI_LUT_ENABLE); }


        // full screen options
        //bool triple_buffer() const { return bool_value(WINOPTION_TRIPLEBUFFER); }
        //float full_screen_brightness() const { return float_value(WINOPTION_FULLSCREENBRIGHTNESS); }
        //float full_screen_contrast() const { return float_value(WINOPTION_FULLSCREENCONTRAST); }
        //float full_screen_gamma() const { return float_value(WINOPTION_FULLSCREENGAMMA); }


        // input options
        //bool dual_lightgun() const { return bool_value(WINOPTION_DUAL_LIGHTGUN); }
    }


    public class osd_interface_WinForms : osd_common_t
    {
        static int updateCount = 0;
        static int audioUpdateCount = 0;


        render_target m_target;
        RawBuffer screenbuffer;  //g_state.screenbuffer = new uint32_t[400 * 400 * 2];
        public RawBufferPointer screenbufferptr;
        public Queue<Int16> m_audiobuffer = new Queue<Int16>();

        public object osdlock = new object();
        public object osdlock_audio = new object();

        public intref [] keyboard_state;
        public intref [] mouse_axis_state;
        public intref [] mouse_button_state;


        public osd_interface_WinForms(osd_options_WinForms options) : base(options)
        {
        }


        public override void init(running_machine machine)
        {
            // call our parent
            base.init(machine);

            set_verbose(true);

            string stemp;
            osd_options_WinForms options = (osd_options_WinForms)machine.options();

            // determine if we are benchmarking, and adjust options appropriately
            int bench = options.bench();
            if (bench > 0)
            {
                options.set_value(emu_options.OPTION_THROTTLE, 0, OPTION_PRIORITY_MAXIMUM);
                options.set_value(osd_options.OSDOPTION_SOUND, "none", OPTION_PRIORITY_MAXIMUM);
                options.set_value(osd_options.OSDOPTION_VIDEO, "none", OPTION_PRIORITY_MAXIMUM);
                options.set_value(emu_options.OPTION_SECONDS_TO_RUN, bench, OPTION_PRIORITY_MAXIMUM);
            }

            // determine if we are profiling, and adjust options appropriately
            int profile = options.profile();
            if (profile > 0)
            {
                options.set_value(emu_options.OPTION_THROTTLE, 0, OPTION_PRIORITY_MAXIMUM);
                options.set_value(osd_options.OSDOPTION_NUMPROCESSORS, 1, OPTION_PRIORITY_MAXIMUM);
            }

#if false
            // thread priority
            if ((machine.debug_flags_get() & running_machine.DEBUG_FLAG_OSD_ENABLED) == 0)
                SetThreadPriority(GetCurrentThread(), options.priority());
#endif

            // get number of processors
            stemp = options.numprocessors();

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


            // hook up the debugger log
            if (options.oslog())
                machine.add_logerror_callback(osdcore_interface.osd_printf_debug);


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

            validity_checker valid = new validity_checker(machine.options());
            valid.set_validate_all(true);
            string sysname = machine.options().system_name();
            bool result = valid.check_all_matching(string.IsNullOrEmpty(sysname) ? "*" : sysname);
            if (!result)
                throw new emu_fatalerror(main_global.EMU_ERR_FAILED_VALIDITY, "Validity check failed ({0} errors, {1} warnings in total)\n", valid.errors(), valid.warnings());
            valid.Dispose();

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
            //m_target.set_bounds(400, 400, 1.0f);

            set_bounds(10, 10);


            {
                keyboard_state = new intref[(int)input_item_id.ITEM_ID_ABSOLUTE_MAXIMUM];
                for (int i = 0; i < (int)input_item_id.ITEM_ID_ABSOLUTE_MAXIMUM; i++)
                    keyboard_state[i] = new intref();

                input_device keyboard_device;
                keyboard_device = machine.input().device_class(input_device_class.DEVICE_CLASS_KEYBOARD).add_device("Keyboard", "Keyboard0");
                if (keyboard_device == null)
                    throw new emu_fatalerror("osd_interface.init() - FAILED - add_device() failed\n");

                foreach (var entry in mameForm.Form1.keymap)
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

            if (updateCount++ % 100 == 0)
                osd_printf_verbose("osd_interface.update() - {0}\n", updateCount);


            if (!skip_redraw)
            {
                render_primitive_list list = m_target.get_primitives();
                list.acquire_lock();

                UInt32 width = (UInt32)get_width();  //400;  //640; //rect_width(&bounds);
                UInt32 height = (UInt32)get_height();  //400; //480; //rect_height(&bounds);
                UInt32 pitch = width;//(UInt32)((width + 3) & ~3);


                lock (osdlock)
                {
                    //software_renderer<typeof(UInt32), 0,0,0, 16,8,0>.draw_primitives(list, g_state.screenbuffer, width, height, pitch);
                    software_renderer<UInt32>.draw_primitives(new software_renderer<UInt32>.TemplateParams(32, 0,0,0, 16,8,0), list, screenbufferptr, width, height, pitch);
                }

                list.release_lock();
            }
        }


        public override void update_audio_stream(ListPointer<Int16> buffer, int samples_this_frame)  //const int16_t *buffer, int samples_this_frame) = 0;
        {
            base.update_audio_stream(buffer, samples_this_frame);

            if (audioUpdateCount++ % 10 == 0)
                osd_printf_info("osd_interface.update_audio_stream() - {0} - samples: {1}\n", audioUpdateCount, samples_this_frame);

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


        public override void customize_input_type_list(simple_list<input_type_entry> typelist)
        {
            base.customize_input_type_list(typelist);
        }


        public override std.vector<mame.ui.menu_item> get_slider_list()
        {
            return base.get_slider_list();
        }


        public int get_width() { return m_target.width(); }
        public int get_height() { return m_target.height(); }


        public void set_bounds(int width, int height, float pixel_aspect = 0)
        {
            m_target.set_bounds(width, height, pixel_aspect);

            const int bpp = 32;
            const int bytes_per_pixel = bpp / 8;
            screenbuffer = new RawBuffer(width * height * bytes_per_pixel);  //g_state.screenbuffer = new uint32_t[400 * 400 * 2];
            screenbufferptr = new RawBufferPointer(screenbuffer);
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

                if (item.i == 1) Console.WriteLine("Pressed");

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
    }
}
