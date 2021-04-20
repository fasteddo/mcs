// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nl_errstr_global
    {
        public const string sHINT_NO_DEACTIVATE = ".HINT_NO_DEACTIVATE";
        public const string sPowerGND = "GND";
        public const string sPowerVCC = "VCC";


        static string PERRMSGV(int narg, string format, params object [] args) { return plib.pfmtlog_global.PERRMSGV(narg, format, args); }


        // nl_base.cpp

        public static string MF_DUPLICATE_NAME_DEVICE_LIST(params object [] args)   { return PERRMSGV(1, "Error adding {0} to device list. Duplicate name.", args); }
        public static string MF_UNKNOWN_TYPE_FOR_OBJECT(params object [] args)      { return PERRMSGV(1, "Unknown type for object {0},", args); }
        public static string MF_NET_1_DUPLICATE_TERMINAL_2(params object [] args)   { return PERRMSGV(2, "net {0}: duplicate terminal {1}", args); }
        public static string MF_REMOVE_TERMINAL_1_FROM_NET_2(params object [] args) { return PERRMSGV(2, "Can not remove terminal {0} from net {1}.", args); }
        public static string MF_UNKNOWN_PARAM_TYPE(params object [] args)           { return PERRMSGV(1, "Can not determine param_type for {0}", args); }
        public static string MF_ERROR_CONNECTING_1_TO_2(params object [] args)      { return PERRMSGV(2, "Error connecting {0} to {1}", args); }
        public static string MF_NO_SOLVER(params object [] args)                    { return PERRMSGV(0, "No solver found for this netlist although analog elements are present", args); }
        public static string MF_HND_VAL_NOT_SUPPORTED(params object [] args)        { return PERRMSGV(1, "HINT_NO_DEACTIVATE value not supported: <{0}>", args); }
        //PERRMSGV(MW_ROM_NOT_FOUND,                      1, "Rom {1} not found")

        // nl_factory.cpp

        public static string MF_FACTORY_ALREADY_CONTAINS_1(params object [] args)   { return PERRMSGV(1, "factory already contains {0}", args); }
        public static string MF_CLASS_1_NOT_FOUND(params object [] args)            { return PERRMSGV(1, "Class <{0}> not found!", args); }

        // nl_base.cpp

        public static string MF_MODEL_1_CAN_NOT_BE_CHANGED_AT_RUNTIME(params object [] args) { return PERRMSGV(1, "Model {0} can not be changed at runtime", args); }
        public static string MF_MORE_THAN_ONE_1_DEVICE_FOUND(params object [] args) { return PERRMSGV(1, "More than one {0} device found", args); }

        // nl_parser.cpp

        public static string MF_UNEXPECTED_NETLIST_END(params object [] args)       { return PERRMSGV(0, "Unexpected NETLIST_END", args); }
        //PERRMSGV(MF_UNEXPECTED_END_OF_FILE,             0, "Unexpected end of file, missing NETLIST_END")
        public static string MF_UNEXPECTED_NETLIST_START(params object [] args)     { return PERRMSGV(0, "Unexpected NETLIST_START", args); }
        //PERRMSGV(MF_EXPECTED_IDENTIFIER_GOT_1,          1, "Expected an identifier, but got {1}")
        //PERRMSGV(MF_EXPECTED_COMMA_OR_RP_1,             1, "Expected comma or right parenthesis but found <{1}>")
        //PERRMSGV(MF_DIPPINS_EQUAL_NUMBER_1,             1, "DIPPINS requires equal number of pins to DIPPINS, first pin is {}")
        //PERRMSGV(MF_PARAM_NOT_FP_1,                     1, "Parameter value <{1}> not floating point")
        //PERRMSGV(MF_TT_LINE_WITHOUT_HEAD,               0, "TT_LINE found without TT_HEAD")

        // nl_setup.cpp

        public static string MF_UNABLE_TO_PARSE_MODEL_1(params object [] args)      { return PERRMSGV(1, "Unable to parse model: {0}", args); }
        public static string MF_MODEL_ALREADY_EXISTS_1(params object [] args)       { return PERRMSGV(1, "Model already exists: {0}", args); }
        public static string MF_DEVICE_ALREADY_EXISTS_1(params object [] args)      { return PERRMSGV(1, "Device already exists: {0}", args); }
        public static string MF_ADDING_ALI1_TO_ALIAS_LIST(params object [] args)    { return PERRMSGV(1, "Error adding alias {0} to alias list", args); }
        public static string MF_DIP_PINS_MUST_BE_AN_EQUAL_NUMBER_OF_PINS_1(params object [] args) { return PERRMSGV(1, "You must pass an equal number of pins to DIPPINS {0}", args); }
        public static string MF_PARAM_COUNT_MISMATCH_2(params object [] args)       { return PERRMSGV(2, "Parameter count mismatch for {0} - only found {1}", args); }
        public static string MF_PARAM_COUNT_EXCEEDED_2(params object [] args)       { return PERRMSGV(2, "Parameter count exceed for {0} - found {1}", args); }
        public static string MF_UNKNOWN_OBJECT_TYPE_1(params object [] args)        { return PERRMSGV(1, "Unknown object type {0}", args); }
        public static string MF_INVALID_NUMBER_CONVERSION_1_2(params object [] args) { return PERRMSGV(2, "Invalid number conversion {0} : {1}", args); }
        public static string MF_INVALID_ENUM_CONVERSION_1_2(params object [] args)  { return PERRMSGV(2, "Invalid element found {0} : {1}", args); }
        public static string MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(params object [] args) { return PERRMSGV(1, "Error adding parameter {0} to parameter list", args); }
        public static string MF_ADDING_1_2_TO_TERMINAL_LIST(params object [] args)  { return PERRMSGV(2, "Error adding {0} {1} to terminal list", args); }
        public static string MF_NET_C_NEEDS_AT_LEAST_2_TERMINAL(params object [] args) { return PERRMSGV(0, "You must pass at least 2 terminals to NET_C", args); }
        public static string MF_FOUND_NO_OCCURRENCE_OF_1(params object [] args)     { return PERRMSGV(1, "Found no occurrence of {0}", args); }
        public static string MF_TERMINAL_1_2_NOT_FOUND(params object [] args)       { return PERRMSGV(2, "Alias {0} was resolved to be terminal {1}. Terminal {1} was not found.", args); }
        public static string MF_OBJECT_1_2_WRONG_TYPE(params object [] args)        { return PERRMSGV(2, "object {0}({1}) found but wrong type", args); }
        public static string MF_PARAMETER_1_2_NOT_FOUND(params object [] args)      { return PERRMSGV(2, "parameter {0}({1}) not found!", args); }
        public static string MF_CONNECTING_1_TO_2(params object [] args)            { return PERRMSGV(2, "Error connecting {0} to {1}", args); }
        public static string MF_DUPLICATE_PROXY_1(params object [] args)            { return PERRMSGV(1, "Terminal {0} already has proxy", args); }
        public static string MF_MERGE_RAIL_NETS_1_AND_2(params object [] args)      { return PERRMSGV(2, "Trying to merge two rail nets: {0} and {1}", args); }
        public static string MF_OBJECT_INPUT_TYPE_1(params object [] args)          { return PERRMSGV(1, "Unable to determine input type of {0}", args); }
        public static string MF_OBJECT_OUTPUT_TYPE_1(params object [] args)         { return PERRMSGV(1, "Unable to determine output type of {0}", args); }
        public static string MF_INPUT_1_ALREADY_CONNECTED(params object [] args)    { return PERRMSGV(1, "Input {0} already connected", args); }
        public static string MF_LINK_TRIES_EXCEEDED(params object [] args)          { return PERRMSGV(1, "Error connecting, {0} tries exceeded", args); }
        public static string MF_MODEL_NOT_FOUND(params object [] args)              { return PERRMSGV(1, "Model {0} not found", args); }
        public static string MF_MODEL_ERROR_1(params object [] args)                { return PERRMSGV(1, "Model error {0}", args); }
        public static string MF_MODEL_ERROR_ON_PAIR_1(params object [] args)        { return PERRMSGV(1, "Model error on pair {0}", args); }
        public static string MF_MODEL_PARAMETERS_NOT_UPPERCASE_1_2(params object [] args) { return PERRMSGV(2, "Model parameters should be uppercase:{0} {1}", args); }
        public static string MF_ENTITY_1_NOT_FOUND_IN_MODEL_2(params object [] args) { return PERRMSGV(2, "Entity {0} not found in model {1}", args); }
        public static string MF_UNKNOWN_NUMBER_FACTOR_IN_1(params object [] args)   { return PERRMSGV(1, "Unknown number factor in: {0}", args); }
        public static string MF_MODEL_NUMBER_CONVERSION_ERROR(params object [] args) { return PERRMSGV(4, "Can't convert {0}={1} to {2} for model {3}", args); }
        public static string MF_NOT_FOUND_IN_SOURCE_COLLECTION(params object [] args) { return PERRMSGV(1, "unable to find {0} in sources collection", args); }

        public static string MW_OVERWRITING_PARAM_1_OLD_2_NEW_3(params object [] args) { return PERRMSGV(3, "Overwriting {0} old <{1}> new <{2}>", args); }
        public static string MW_CONNECTING_1_TO_ITSELF(params object [] args)       { return PERRMSGV(1, "Connecting {0} to itself. This may be right, though", args); }
        public static string ME_NC_PIN_1_WITH_CONNECTIONS(params object [] args)    { return PERRMSGV(1, "Found NC (not connected) terminal {0} with connections", args); }
        public static string MI_ANALOG_OUTPUT_1_WITHOUT_CONNECTIONS(params object [] args) { return PERRMSGV(1, "Found analog output {0} without connections", args); }
        public static string MI_LOGIC_OUTPUT_1_WITHOUT_CONNECTIONS(params object [] args) { return PERRMSGV(1, "Found logic output {0} without connections", args); }
        public static string MW_LOGIC_INPUT_1_WITHOUT_CONNECTIONS(params object [] args) { return PERRMSGV(1, "Found logic input {0} without connections", args); }
        public static string MW_TERMINAL_1_WITHOUT_CONNECTIONS(params object [] args) { return PERRMSGV(1, "Found terminal {0} without connections", args); }

        public static string ME_TERMINAL_1_WITHOUT_NET(params object [] args) { return PERRMSGV(1, "Found terminal {0} without a net", args); }
        public static string MF_TERMINALS_WITHOUT_NET(params object [] args) { return PERRMSGV(0, "Found terminals without a net", args); }

        public static string MI_REMOVE_DEVICE_1_CONNECTED_ONLY_TO_RAILS_2_3(params object [] args) { return PERRMSGV(3, "Found device {0} connected only to railterminals {1}/{2}. Will be removed", args); }

        public static string MW_DATA_1_NOT_FOUND(params object [] args) { return PERRMSGV(1, "unable to find data {0} in sources collection", args); }

        public static string MW_DEVICE_NOT_FOUND_FOR_HINT(params object [] args) { return PERRMSGV(1, "Device not found for hint {0}", args); }
        public static string MW_UNKNOWN_PARAMETER(params object [] args) { return PERRMSGV(1, "Unknown parameter {0}", args); }

        // nlid_proxy.cpp

        public static string MI_NO_POWER_TERMINALS_ON_DEVICE_2(params object [] args) { return PERRMSGV(2, "D/A Proxy {0}: Found no valid combination of power terminals on device {1}"); }
        public static string MI_MULTIPLE_POWER_TERMINALS_ON_DEVICE(params object [] args) { return PERRMSGV(5, "D/A Proxy: Found multiple power terminals on device {0}: {1} {2} {3} {4}"); }

        // nld_matrix_solver.cpp

        public static string MF_UNHANDLED_ELEMENT_1_FOUND(params object [] args) { return PERRMSGV(1, "setup_base:unhandled element <{0}> found", args); }
        public static string MF_FOUND_TERM_WITH_MISSING_OTHERNET(params object [] args) { return PERRMSGV(1, "found term with missing othernet {0}", args); }

        public static string MW_NEWTON_LOOPS_EXCEEDED_ON_NET_1(params object [] args) { return PERRMSGV(1, "NEWTON_LOOPS exceeded on net {0}... reschedule", args); }

        // nld_solver.cpp

        public static string MI_NO_SPECIFIC_SOLVER(params object [] args) { return PERRMSGV(1, "No specific solver found for netlist of size {0}", args); }
        public static string MW_SOLVER_METHOD_NOT_SUPPORTED(params object [] args) { return PERRMSGV(2, "Solver method {0} not supported. Falling back to {1}", args); }

        // nld_mm5837.cpp

        public static string MW_FREQUENCY_OUTSIDE_OF_SPECS_1(params object [] args) { return PERRMSGV(1, "MM5837: Frequency outside of specs: {0}", args); }

        // nld_opamps.cpp

        public static string MF_OPAMP_UNKNOWN_TYPE(params object [] args) { return PERRMSGV(1, "Unknown opamp type: {0}", args); }
        public static string MW_OPAMP_FAIL_CONVERGENCE(params object [] args) { return PERRMSGV(1, "Opamp <{0}> parameters fail convergence criteria", args); }

        // nld_mosfet.cpp

        //PERRMSGV(MW_MOSFET_THRESHOLD_VOLTAGE,           1, "Mosfet: Threshold voltage not specified for {1}")

        // nl_tool.cpp

        //PERRMSGV(MF_FILE_OPEN_ERROR,                    1, "Error opening file: {1}")
    }
}
