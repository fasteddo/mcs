// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using game_driver_map = mame.std.unordered_map<string, mame.game_driver>;
using s8 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using validity_checker_int_map = mame.std.unordered_map<string, object>;  //using int_map = std::unordered_map<std::string, uintptr_t>;
using validity_checker_string_set = mame.std.unordered_set<string>;  //using string_set = std::unordered_set<std::string>;

using static mame.corefile_global;
using static mame.corestr_global;
using static mame.cpp_global;
using static mame.main_global;
using static mame.osdcore_global;


namespace mame
{
    // core validity checker class
    public class validity_checker : osd_output, IDisposable
    {
        // internal map types
        //using game_driver_map = std::unordered_map<std::string, game_driver const *>;
        //using int_map = std::unordered_map<std::string, uintptr_t>;
        //using string_set = std::unordered_set<std::string>;


        // internal driver list
        driver_enumerator m_drivlist;

        // blank options for use during validation
        emu_options m_blank_options;

        // error tracking
        int m_errors;
        int m_warnings;
        bool m_print_verbose;
        string m_error_text;
        string m_warning_text;
        string m_verbose_text;

        // maps for finding duplicates
        game_driver_map m_names_map = new game_driver_map();
        game_driver_map m_descriptions_map = new game_driver_map();
        game_driver_map m_roms_map = new game_driver_map();
        validity_checker_int_map m_defstr_map = new validity_checker_int_map();

        // current state
        game_driver m_current_driver;
        device_t m_current_device;
        string m_current_ioport;
        validity_checker_int_map m_region_map = new validity_checker_int_map();
        validity_checker_string_set m_ioport_set = new validity_checker_string_set();
        validity_checker_string_set m_already_checked = new validity_checker_string_set();
        validity_checker_string_set m_slotcard_set = new validity_checker_string_set();
        bool m_checking_card;
        bool m_quick;


        //-------------------------------------------------
        //  validity_checker - constructor
        //-------------------------------------------------
        public validity_checker(emu_options options, bool quick)
        {
            m_drivlist = new driver_enumerator(options);
            m_errors = 0;
            m_warnings = 0;
            m_print_verbose = options.verbose();
            m_current_driver = null;
            m_current_device = null;
            m_current_ioport = null;
            m_checking_card = false;
            m_quick = quick;


            // pre-populate the defstr map with all the default strings
            for (int strnum = 1; strnum < (int)INPUT_STRING.INPUT_STRING_COUNT; strnum++)
            {
                string str = ioport_string_from_index((u32)strnum);
                if (!string.IsNullOrEmpty(str))
                    m_defstr_map.insert(str, strnum);
            }
        }

        ~validity_checker()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            validate_end();
            m_isDisposed = true;
        }


        // getters
        public int errors() { return m_errors; }
        public int warnings() { return m_warnings; }
        bool quick() { return m_quick; }


        // setter
        public void set_verbose(bool verbose) { m_print_verbose = verbose; }


        // operations

        //void check_driver(const game_driver &driver);

        //-------------------------------------------------
        //  check_shared_source - check all drivers that
        //  share the same source file as the given driver
        //-------------------------------------------------
        public void check_shared_source(game_driver driver)
        {
            // initialize
            validate_begin();

            // then iterate over all drivers and check the ones that share the same source file
            m_drivlist.reset();
            while (m_drivlist.next())
            {
                if (std.strcmp(driver.type.source(), m_drivlist.driver().type.source()) == 0)
                    validate_one(m_drivlist.driver());
            }

            // cleanup
            validate_end();
        }


        //-------------------------------------------------
        //  check_all_matching - check all drivers whose
        //  names match the given string
        //-------------------------------------------------
        public bool check_all_matching(string str = "*")
        {
            // start by checking core stuff
            validate_begin();
            validate_integer_semantics();
            validate_inlines();
            validate_rgb();
            validate_delegates_mfp();
            validate_delegates_latebind();
            validate_delegates_functoid();

            // if we had warnings or errors, output
            if (m_errors > 0 || m_warnings > 0 || !string.IsNullOrEmpty(m_verbose_text))
            {
                output_via_delegate(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, "Core: {0} errors, {1} warnings\n", m_errors, m_warnings);
                if (m_errors > 0)
                    output_indented_errors(m_error_text, "Errors");
                if (m_warnings > 0)
                    output_indented_errors(m_warning_text, "Warnings");
                if (!string.IsNullOrEmpty(m_verbose_text))
                    output_indented_errors(m_verbose_text, "Messages");

                output_via_delegate(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, "\n");
            }

            // then iterate over all drivers and check them
            m_drivlist.reset();
            bool validated_any = false;
            while (m_drivlist.next())
            {
                if (driver_list.matches(str, m_drivlist.driver().name))
                {
                    validate_one(m_drivlist.driver());
                    validated_any = true;
                }
            }

            // validate devices
            if (str == null)
                validate_device_types();

            // cleanup
            validate_end();

            // if we failed to match anything, it
            if (str != null && !validated_any)
                throw new emu_fatalerror(EMU_ERR_NO_SUCH_SYSTEM, "No matching systems found for '{0}'", str);

            return !(m_errors > 0 || m_warnings > 0);
        }


        // helpers for devices

        //void validate_tag(const char *tag);

        public int region_length(string tag) { return (int)m_region_map.find(tag); }


        public bool ioport_missing(string tag) { return !m_checking_card && (m_ioport_set.find(tag)); }


        // generic registry of already-checked stuff
        //bool already_checked(const char *string) { return (m_already_checked.add(string, 1, false) == TMERR_DUPLICATE); }


        // osd_output interface
        //-------------------------------------------------
        //  error_output - error message output override
        //-------------------------------------------------
        public override void output_callback(osd_output_channel channel, string format, params object [] args)  //virtual void output_callback(osd_output_channel channel, const util::format_argument_pack<std::ostream> &args) override;
        {
            string output = "";
            switch (channel)
            {
                case osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR:
                    // count the error
                    m_errors++;

                    // output the source(driver) device 'tag'
                    build_output_prefix(ref output);

                    // generate the string
                    util.stream_format(ref output, format, args);
                    m_error_text = m_error_text.append_(output);
                    break;

                case osd_output_channel.OSD_OUTPUT_CHANNEL_WARNING:
                    // count the error
                    m_warnings++;

                    // output the source(driver) device 'tag'
                    build_output_prefix(ref output);

                    // generate the string and output to the original target
                    util.stream_format(ref output, format, args);
                    m_warning_text = m_warning_text.append_(output);
                    break;

                case osd_output_channel.OSD_OUTPUT_CHANNEL_VERBOSE:
                    // if we're not verbose, skip it
                    if (!m_print_verbose) break;

                    // output the source(driver) device 'tag'
                    build_output_prefix(ref output);

                    // generate the string and output to the original target
                    util.stream_format(ref output, format, args);
                    m_verbose_text = m_verbose_text.append_(output);
                    break;

                default:
                    chain_output(channel, format, args);
                    break;
            }
        }


        // internal helpers

        //int get_defstr_index(const char *string, bool suppress_error = false);


        // core helpers

        //-------------------------------------------------
        //  validate_begin - prepare for validation by
        //  taking over the output callbacks and resetting
        //  our internal state
        //-------------------------------------------------
        void validate_begin()
        {
            // take over error and warning outputs
            osd_output.push(this);

            // reset all our maps
            m_names_map.clear();
            m_descriptions_map.clear();
            m_roms_map.clear();
            m_defstr_map.clear();
            m_region_map.clear();
            m_ioport_set.clear();
            m_slotcard_set.clear();

            // reset internal state
            m_errors = 0;
            m_warnings = 0;
            m_already_checked.clear();
        }


        //-------------------------------------------------
        //  validate_end - restore output callbacks and
        //  clean up
        //-------------------------------------------------
        void validate_end()
        {
            // restore the original output callbacks
            osd_output.pop(this);
        }


        //-------------------------------------------------
        //  validate_drivers - master validity checker
        //-------------------------------------------------
        void validate_one(game_driver driver)
        {
            // help verbose validation detect configuration-related crashes
            if (m_print_verbose)
                output_via_delegate(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, "Validating driver {0} ({1})...\n", driver.name, core_filename_extract_base(driver.type.source()));

            // set the current driver
            m_current_driver = driver;
            m_current_device = null;
            m_current_ioport = null;
            m_region_map.clear();
            m_ioport_set.clear();
            m_checking_card = false;

            // reset error/warning state
            int start_errors = m_errors;
            int start_warnings = m_warnings;
            m_error_text = "";
            m_warning_text = "";
            m_verbose_text = "";

            // wrap in try/catch to catch fatalerrors
            try
            {
                machine_config config = new machine_config(driver, m_blank_options);
                validate_driver(config.root_device());
                validate_roms(config.root_device());
                validate_inputs(config.root_device());
                validate_devices(config);
            }
            catch (emu_fatalerror err)
            {
                osd_printf_error("Fatal error {0}", err.what());
            }


            // if we had warnings or errors, output
            if (m_errors > start_errors || m_warnings > start_warnings || !string.IsNullOrEmpty(m_verbose_text))
            {
                if (!m_print_verbose)
                    output_via_delegate(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, "Driver {0} (file {1}): {2} errors, {3} warnings\n", driver.name, core_filename_extract_base(driver.type.source()), m_errors - start_errors, m_warnings - start_warnings);

                output_via_delegate(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, "{0} errors, {1} warnings\n", m_errors - start_errors, m_warnings - start_warnings);

                if (m_errors > start_errors)
                    output_indented_errors(m_error_text, "Errors");
                if (m_warnings > start_warnings)
                    output_indented_errors(m_warning_text, "Warnings");
                if (!string.IsNullOrEmpty(m_verbose_text))
                    output_indented_errors(m_verbose_text, "Messages");

                output_via_delegate(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, "\n");
            }

            // reset the driver/device
            m_current_driver = null;
            m_current_device = null;
            m_current_ioport = null;
            m_region_map.clear();
            m_ioport_set.clear();
            m_checking_card = false;
        }


        // internal sub-checks

        //-------------------------------------------------
        //  validate_integer_semantics - validate that
        //  integers behave as expected, particularly
        //  with regards to overflow and shifting
        //-------------------------------------------------
        void validate_integer_semantics()
        {
            //throw new emu_unimplemented();
#if false
            // basic system checks
            if (~0 != -1) osd_printf_error("Machine must be two's complement\n");
#endif

            u8 a = 0xff;
            u8 b = (u8)(a + 1);
            if (b > a) osd_printf_error("u8 must be 8 bits\n");

            // check size of core integer types
            if (sizeof(s8)  != 1) osd_printf_error("s8 must be 8 bits\n");
            if (sizeof(u8)  != 1) osd_printf_error("u8 must be 8 bits\n");
            if (sizeof(s16) != 2) osd_printf_error("s16 must be 16 bits\n");
            if (sizeof(u16) != 2) osd_printf_error("u16 must be 16 bits\n");
            if (sizeof(s32) != 4) osd_printf_error("s32 must be 32 bits\n");
            if (sizeof(u32) != 4) osd_printf_error("u32 must be 32 bits\n");
            if (sizeof(s64) != 8) osd_printf_error("s64 must be 64 bits\n");
            if (sizeof(u64) != 8) osd_printf_error("u64 must be 64 bits\n");

            //throw new emu_unimplemented();
#if false
            // check signed right shift
            s8  a8 = -3;
            s16 a16 = -3;
            s32 a32 = -3;
            s64 a64 = -3;
            if (a8  >> 1 != -2) osd_printf_error("s8 right shift must be arithmetic\n");
            if (a16 >> 1 != -2) osd_printf_error("s16 right shift must be arithmetic\n");
            if (a32 >> 1 != -2) osd_printf_error("s32 right shift must be arithmetic\n");
            if (a64 >> 1 != -2) osd_printf_error("s64 right shift must be arithmetic\n");
#endif

            //throw new emu_unimplemented();
#if false
            // check pointer size
//#ifdef PTR64
            if (sizeof(void *) != 8) m_osdcore.osd_printf_error("PTR64 flag enabled, but was compiled for 32-bit target\n");
//#else
            if (sizeof(void *) != 4) m_osdcore.osd_printf_error("PTR64 flag not enabled, but was compiled for 64-bit target\n");
//#endif
#endif

            //throw new emu_unimplemented();
#if false
            // TODO: check if this is actually working
            // check endianness definition
            UINT16 lsbtest = 0;
            *(UINT8 *)&lsbtest = 0xff;
//#ifdef LSB_FIRST
            if (lsbtest == 0xff00) m_osdcore.osd_printf_error("LSB_FIRST specified, but running on a big-endian machine\n");
//#else
            if (lsbtest == 0x00ff) m_osdcore.osd_printf_error("LSB_FIRST not specified, but running on a little-endian machine\n");
//#endif
#endif
        }


        //-------------------------------------------------
        //  validate_inlines - validate inline function
        //  behaviors
        //-------------------------------------------------
        void validate_inlines()
        {
            //throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  validate_rgb - validate optimised RGB utility
        //  class
        //-------------------------------------------------
        void validate_rgb()
        {
            //throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  validate_delegates_mfp - test delegate member
        //  function functionality
        //-------------------------------------------------

        void validate_delegates_mfp()
        {
            //throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  validate_delegates_latebind - test binding a
        //  delegate to an object after the function is
        //  set
        //-------------------------------------------------
        void validate_delegates_latebind()
        {
        }


        //-------------------------------------------------
        //  validate_delegates_functoid - test delegate
        //  functoid functionality
        //-------------------------------------------------
        void validate_delegates_functoid()
        {
        }


        //-------------------------------------------------
        //  validate_driver - validate basic driver
        //  information
        //-------------------------------------------------
        void validate_driver(device_t root)
        {
            //throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  validate_roms - validate ROM definitions
        //-------------------------------------------------
        void validate_roms(device_t root)
        {
            //throw new emu_unimplemented();
        }


        //void validate_analog_input_field(ioport_field &field);
        //void validate_dip_settings(ioport_field &field);
        //void validate_condition(ioport_condition &condition, device_t &device);


        //-------------------------------------------------
        //  validate_inputs - validate input configuration
        //-------------------------------------------------
        void validate_inputs(device_t root)
        {
            //throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  validate_devices - run per-device validity
        //  checks
        //-------------------------------------------------
        void validate_devices(machine_config config)
        {
            //throw new emu_unimplemented();
        }


        void validate_device_types()
        {
            //throw new emu_unimplemented();
        }


        // output helpers

        //-------------------------------------------------
        //  build_output_prefix - create a prefix
        //  indicating the current source file, driver,
        //  and device
        //-------------------------------------------------
        void build_output_prefix(ref string str)  //void build_output_prefix(std::ostream &str) const;
        {
            // if we have a current (non-root) device, indicate that
            if (m_current_device != null && m_current_device.owner() != null)
                util.stream_format(ref str, "{0} device '{1}': ", m_current_device.name(), m_current_device.tag()[1..]);

            // if we have a current port, indicate that as well
            if (m_current_ioport != null)
                util.stream_format(ref str, "ioport '{0}': ", m_current_ioport);
        }


        //-------------------------------------------------
        //  output_via_delegate - helper to output a
        //  message via a varargs string, so the argptr
        //  can be forwarded onto the given delegate
        //-------------------------------------------------
        void output_via_delegate(osd_output_channel channel, string fmt, params object [] args)  //template <typename Format, typename... Params> void output_via_delegate(osd_output_channel channel, Format &&fmt, Params &&...args);
        {
            // call through to the delegate with the proper parameters
            chain_output(channel, fmt, args);
        }


        //-------------------------------------------------
        //  output_indented_errors - helper to output error
        //  and warning messages with header and indents
        //-------------------------------------------------
        void output_indented_errors(string text, string header)
        {
            // remove trailing newline
            if (text[text.Length - 1] == '\n')
                text = text.Remove(text.Length - 1);

            text = text.Replace("\n", "\n   ");
            output_via_delegate(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, "{0}:\n   {1}\n", header, text);
        }


        //-------------------------------------------------
        //  ioport_string_from_index - return an indexed
        //  string from the I/O port system
        //-------------------------------------------------
        string ioport_string_from_index(u32 index)
        {
            return ioport_configurer.string_from_token(index.ToString());
        }
    }
}
