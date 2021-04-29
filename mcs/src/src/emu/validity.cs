// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using game_driver_map = mame.std.unordered_map<string, mame.game_driver>;
using validity_checker_int_map = mame.std.unordered_map<string, object>;  //using int_map = std::unordered_map<std::string, uintptr_t>;
using validity_checker_string_set = mame.std.unordered_set<string>;  //using string_set = std::unordered_set<std::string>;


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
                string str = ioport_string_from_index((UInt32)strnum);
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
                if (strcmp(driver.type.source(), m_drivlist.driver().type.source()) == 0)
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
            validate_core();
            validate_inlines();
            validate_rgb();

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
                    output += string.Format(format, args);
                    m_error_text = m_error_text.append_(output);
                    break;

                case osd_output_channel.OSD_OUTPUT_CHANNEL_WARNING:
                    // count the error
                    m_warnings++;

                    // output the source(driver) device 'tag'
                    build_output_prefix(ref output);

                    // generate the string and output to the original target
                    output += string.Format(format, args);
                    m_warning_text = m_warning_text.append_(output);
                    break;

                case osd_output_channel.OSD_OUTPUT_CHANNEL_VERBOSE:
                    // if we're not verbose, skip it
                    if (!m_print_verbose) break;

                    // output the source(driver) device 'tag'
                    build_output_prefix(ref output);

                    // generate the string and output to the original target
                    output += string.Format(format, args);
                    m_verbose_text = m_verbose_text.append_(output);
                    break;

                default:
                    chain_output(channel, format, args);
                    break;
            }
        }


        // internal helpers

        //-------------------------------------------------
        //  ioport_string_from_index - return an indexed
        //  string from the I/O port system
        //-------------------------------------------------
        string ioport_string_from_index(UInt32 index)
        {
            return ioport_configurer.string_from_token(index.ToString());
        }


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
                output_via_delegate(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, "Validating driver {0} ({1})...\n", driver.name, g.core_filename_extract_base(driver.type.source()));

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
                    output_via_delegate(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, "Driver {0} (file {1}): {2} errors, {3} warnings\n", driver.name, g.core_filename_extract_base(driver.type.source()), m_errors - start_errors, m_warnings - start_warnings);

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
        //  validate_core - validate core internal systems
        //-------------------------------------------------
        void validate_core()
        {
            //throw new emu_unimplemented();
#if false
            // basic system checks
            if (~0 != -1) osd_printf_error("Machine must be two's complement\n");
#endif

            byte a = 0xff;
            byte b = (byte)(a + 1);
            if (b > a) osd_printf_error("UINT8 must be 8 bits\n");

            // check size of core integer types
            if (sizeof(sbyte)  != 1) osd_printf_error("INT8 must be 8 bits\n");
            if (sizeof(byte)   != 1) osd_printf_error("UINT8 must be 8 bits\n");
            if (sizeof(Int16)  != 2) osd_printf_error("INT16 must be 16 bits\n");
            if (sizeof(UInt16) != 2) osd_printf_error("UINT16 must be 16 bits\n");
            if (sizeof(int)    != 4) osd_printf_error("INT32 must be 32 bits\n");
            if (sizeof(UInt32) != 4) osd_printf_error("UINT32 must be 32 bits\n");
            if (sizeof(Int64)  != 8) osd_printf_error("INT64 must be 64 bits\n");
            if (sizeof(UInt64) != 8) osd_printf_error("UINT64 must be 64 bits\n");

            //throw new emu_unimplemented();
#if false
            // check signed right shift
            INT8  a8 = -3;
            INT16 a16 = -3;
            INT32 a32 = -3;
            INT64 a64 = -3;
            if (a8  >> 1 != -2) osd_printf_error("INT8 right shift must be arithmetic\n");
            if (a16 >> 1 != -2) osd_printf_error("INT16 right shift must be arithmetic\n");
            if (a32 >> 1 != -2) osd_printf_error("INT32 right shift must be arithmetic\n");
            if (a64 >> 1 != -2) osd_printf_error("INT64 right shift must be arithmetic\n");
#endif

            //throw new emu_unimplemented();
#if false
            // check pointer size
//#ifdef PTR64
            if (sizeof(void *) != 8) osdcore_global.m_osdcore.osd_printf_error("PTR64 flag enabled, but was compiled for 32-bit target\n");
//#else
            if (sizeof(void *) != 4) osdcore_global.m_osdcore.osd_printf_error("PTR64 flag not enabled, but was compiled for 64-bit target\n");
//#endif
#endif

            //throw new emu_unimplemented();
#if false
            // TODO: check if this is actually working
            // check endianness definition
            UINT16 lsbtest = 0;
            *(UINT8 *)&lsbtest = 0xff;
//#ifdef LSB_FIRST
            if (lsbtest == 0xff00) osdcore_global.m_osdcore.osd_printf_error("LSB_FIRST specified, but running on a big-endian machine\n");
//#else
            if (lsbtest == 0x00ff) osdcore_global.m_osdcore.osd_printf_error("LSB_FIRST not specified, but running on a little-endian machine\n");
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


        void validate_rgb()
        {
            //throw new emu_unimplemented();
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
            throw new emu_unimplemented();
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
                str += string.Format("{0} device '{1}': ", m_current_device.name(), m_current_device.tag().Substring(1));

            // if we have a current port, indicate that as well
            if (m_current_ioport != null)
                str += string.Format("ioport '{0}': ", m_current_ioport);
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
                text = text.Remove(text.Length);

            text = text.Replace("\n", "\n   ");
            output_via_delegate(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, "{0}:\n   {1}\n", header, text);
        }


        // random number generation
        //s32 random_i32();
        //u32 random_u32();
        //s64 random_i64();
        //u64 random_u64();
    }
}
