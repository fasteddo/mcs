// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class nl_errstr_global
    {
        //#define PERRMSG(name, str) \
        //    struct name \
        //    { \
        //        operator pstring() const { return str; } \
        //    };

        //#define PERRMSGV(name, narg, str) \
        //    struct name \
        //    { \
        //        template<typename... Args> name(Args&&... args) \
        //        : m_m(plib::pfmt(str)(std::forward<Args>(args)...)) \
        //        { static_assert(narg == sizeof...(args), "Argument count mismatch"); } \
        //        operator pstring() const { return m_m; } \
        //        pstring m_m; \
        //    };
        static string PERRMSGV(int narg, string format, params object [] args) { global_object.static_assert(narg == args.Length, "Argument count mismatch"); return String.Format(format, args); }


        // nl_base.cpp

        public static string MF_DUPLICATE_NAME_DEVICE_LIST(params object [] args)   { return PERRMSGV(1, "Error adding {0} to device list. Duplicate name.", args); }
        public static string MF_UNKNOWN_TYPE_FOR_OBJECT(params object [] args)      { return PERRMSGV(1, "Unknown type for object {1},", args); }
        public static string MF_NET_1_DUPLICATE_TERMINAL_2(params object [] args)   { return PERRMSGV(2, "net {1}: duplicate terminal {2}", args); }
        public static string MF_REMOVE_TERMINAL_1_FROM_NET_2(params object [] args) { return PERRMSGV(2, "Can not remove terminal {1} from net {2}.", args); }
        public static string MF_UNKNOWN_PARAM_TYPE(params object [] args)           { return PERRMSGV(1, "Can not determine param_type for {1}", args); }
        public static string MF_ERROR_CONNECTING_1_TO_2(params object [] args)      { return PERRMSGV(2, "Error connecting {1} to {2}", args); }
        public static string MF_NO_SOLVER(params object [] args)                    { return PERRMSGV(0, "No solver found for this netlist although analog elements are present", args); }
        public static string MF_HND_VAL_NOT_SUPPORTED(params object [] args)        { return PERRMSGV(1, "HINT_NO_DEACTIVATE value not supported: <{1}>", args); }

        // nl_factory.cpp

        public static string MF_FACTORY_ALREADY_CONTAINS_1(params object [] args)   { return PERRMSGV(1, "factory already contains {1}", args); }
        public static string MF_CLASS_1_NOT_FOUND(params object [] args)            { return PERRMSGV(1, "Class <{1}> not found!", args); }

        // nld_opamps.cpp

        public static string MF_UNKNOWN_OPAMP_TYPE(params object [] args)           { return PERRMSGV(1, "Unknown opamp type: {1}", args); }

        // nld_matrix_solver.cpp

        public static string MF_UNHANDLED_ELEMENT_1_FOUND(params object [] args)    { return PERRMSGV(1, "setup_base:unhandled element <{1}> found", args); }
        public static string MF_FOUND_TERM_WITH_MISSING_OTHERNET(params object [] args) { return PERRMSGV(1, "found term with missing othernet {1}", args); }

        public static string MW_NEWTON_LOOPS_EXCEEDED_ON_NET_1(params object [] args) { return PERRMSGV(1, "NEWTON_LOOPS exceeded on net {1}... reschedule", args); }

        // nld_solver.cpp

        public static string MF_UNKNOWN_SOLVER_TYPE(params object [] args)          { return PERRMSGV(1, "Unknown solver type: {1}", args); }
        public static string MF_NETGROUP_SIZE_EXCEEDED_1(params object [] args)     { return PERRMSGV(1, "Encountered netgroup with > {1} nets", args); }

        public static string MI_NO_SPECIFIC_SOLVER(params object [] args)           { return PERRMSGV(1, "No specific solver found for netlist of size {1}", args); }

        // nl_base.cpp

        public static string MF_MODEL_1_CAN_NOT_BE_CHANGED_AT_RUNTIME(params object [] args) { return PERRMSGV(1, "Model {1} can not be changed at runtime", args); }
        public static string MF_MORE_THAN_ONE_1_DEVICE_FOUND(params object [] args) { return PERRMSGV(1, "more than one {1} device found", args); }

        // nl_parser.cpp

        public static string MF_UNEXPECTED_NETLIST_END(params object [] args)       { return PERRMSGV(0, "Unexpected NETLIST_END", args); }
        public static string MF_UNEXPECTED_NETLIST_START(params object [] args)     { return PERRMSGV(0, "Unexpected NETLIST_START", args); }

        // nl_setup.cpp

        //PERRMSG(MF_1_CLASS_1_NOT_FOUND   ,"Class {1} not found!")
        public static string MF_UNABLE_TO_PARSE_MODEL_1(params object [] args)      { return PERRMSGV(1, "Unable to parse model: {1}", args); }
        public static string MF_MODEL_ALREADY_EXISTS_1(params object [] args)       { return PERRMSGV(1, "Model already exists: {1}", args); }
        public static string MF_DEVICE_ALREADY_EXISTS_1(params object [] args)      { return PERRMSGV(1, "Device already exists: {1}", args); }
        public static string MF_ADDING_ALI1_TO_ALIAS_LIST(params object [] args)    { return PERRMSGV(1, "Error adding alias {1} to alias list", args); }
        public static string MF_DIP_PINS_MUST_BE_AN_EQUAL_NUMBER_OF_PINS_1(params object [] args) { return PERRMSGV( 1,"You must pass an equal number of pins to DIPPINS {1}", args); }
        public static string MF_UNKNOWN_OBJECT_TYPE_1(params object [] args)        { return PERRMSGV(1, "Unknown object type {1}", args); }
        public static string MF_INVALID_NUMBER_CONVERSION_1_2(params object [] args) { return PERRMSGV(2, "Invalid number conversion {1} : {2}", args); }
        public static string MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(params object [] args) { return PERRMSGV(1, "Error adding parameter {1} to parameter list", args); }
        public static string MF_ADDING_1_2_TO_TERMINAL_LIST(params object [] args)  { return PERRMSGV(2, "Error adding {1} {2} to terminal list", args); }
        public static string MF_NET_C_NEEDS_AT_LEAST_2_TERMINAL(params object [] args) { return PERRMSGV(0, "You must pass at least 2 terminals to NET_C", args); }
        public static string MF_FOUND_NO_OCCURRENCE_OF_1(params object [] args)     { return PERRMSGV(1, "Found no occurrence of {1}", args); }
        public static string MF_TERMINAL_1_2_NOT_FOUND(params object [] args)       { return PERRMSGV(2, "Alias {1} was resolved to be terminal {2}. Terminal {2} was not found.", args); }
        public static string MF_OBJECT_1_2_WRONG_TYPE(params object [] args)        { return PERRMSGV(2, "object {1}({2}) found but wrong type", args); }
        public static string MF_PARAMETER_1_2_NOT_FOUND(params object [] args)      { return PERRMSGV(2, "parameter {1}({2}) not found!", args); }
        public static string MF_CONNECTING_1_TO_2(params object [] args)            { return PERRMSGV(2, "Error connecting {1} to {2}", args); }
        public static string MF_MERGE_RAIL_NETS_1_AND_2(params object [] args)      { return PERRMSGV(2, "Trying to merge two rail nets: {1} and {2}", args); }
        public static string MF_OBJECT_INPUT_TYPE_1(params object [] args)          { return PERRMSGV(1, "Unable to determine input type of {1}", args); }
        public static string MF_OBJECT_OUTPUT_TYPE_1(params object [] args)         { return PERRMSGV(1, "Unable to determine output type of {1}", args); }
        public static string MF_INPUT_1_ALREADY_CONNECTED(params object [] args)    { return PERRMSGV(1, "Input {1} already connected", args); }
        public static string MF_LINK_TRIES_EXCEEDED(params object [] args)          { return PERRMSGV(1, "Error connecting, {1} tries exceeded", args); }
        public static string MF_MODEL_NOT_FOUND(params object [] args)              { return PERRMSGV(1, "Model {1} not found", args); }
        public static string MF_MODEL_ERROR_1(params object [] args)                { return PERRMSGV(1, "Model error {1}", args); }
        public static string MF_MODEL_ERROR_ON_PAIR_1(params object [] args)        { return PERRMSGV(1, "Model error on pair {1}", args); }
        public static string MF_MODEL_PARAMETERS_NOT_UPPERCASE_1_2(params object [] args) { return PERRMSGV(2, "Model parameters should be uppercase:{1} {2}", args); }
        public static string MF_ENTITY_1_NOT_FOUND_IN_MODEL_2(params object [] args) { return PERRMSGV(2, "Entity {1} not found in model {2}", args); }
        public static string MF_UNKNOWN_NUMBER_FACTOR_IN_1(params object [] args)   { return PERRMSGV(1, "Unknown number factor in: {1}", args); }
        public static string MF_NOT_FOUND_IN_SOURCE_COLLECTION(params object [] args) { return PERRMSGV(1, "unable to find {1} in sources collection", args); }

        public static string MW_OVERWRITING_PARAM_1_OLD_2_NEW_3(params object [] args) { return PERRMSGV(3, "Overwriting {1} old <{2}> new <{3}>", args); }
        public static string MW_CONNECTING_1_TO_ITSELF(params object [] args)       { return PERRMSGV(1, "Connecting {1} to itself. This may be right, though", args); }
        public static string MI_DUMMY_1_WITHOUT_CONNECTIONS(params object [] args)  { return PERRMSGV(1, "Found dummy terminal {1} without connections", args); }
        public static string MI_ANALOG_OUTPUT_1_WITHOUT_CONNECTIONS(params object [] args) { return PERRMSGV(1, "Found analog output {1} without connections", args); }
        public static string MI_LOGIC_OUTPUT_1_WITHOUT_CONNECTIONS(params object [] args) { return PERRMSGV(1, "Found logic output {1} without connections", args); }
        public static string MW_LOGIC_INPUT_1_WITHOUT_CONNECTIONS(params object [] args) { return PERRMSGV(1, "Found logic input {1} without connections", args); }
        public static string MW_TERMINAL_1_WITHOUT_CONNECTIONS(params object [] args) { return PERRMSGV(1, "Found terminal {1} without connections", args); }

        public static string MI_REMOVE_DEVICE_1_CONNECTED_ONLY_TO_RAILS_2_3(params object [] args) { return PERRMSGV( 3, "Found device {1} connected only to railterminals {2}/{3}. Will be removed", args); }

        public static string MW_DATA_1_NOT_FOUND(params object [] args)             { return PERRMSGV(1, "unable to find data {1} in sources collection", args); }

        // nld_mm5837.cpp

        public static string MW_FREQUENCY_OUTSIDE_OF_SPECS_1(params object [] args) { return PERRMSGV(1, "MM5837: Frequency outside of specs: {1}", args); }

        // nlid_proxy.cpp

        public static string MI_NO_POWER_TERMINALS_ON_DEVICE_1(params object [] args) { return PERRMSGV(1, "D/A Proxy: Found no valid combination of power terminals on device {1}", args); }
    }
}
