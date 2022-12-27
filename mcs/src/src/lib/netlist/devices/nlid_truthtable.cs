// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<config::prefer_int128::value && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using unsigned = System.UInt32;
using size_t = System.UInt64;
using tt_bitset = mame.netlist.devices.pbitset;  //using tt_bitset = pbitset<std::uint_least64_t>;
using uint_least8_t = System.Byte;
using uint_least64_t = System.UInt64;

using static mame.cpp_global;
using static mame.netlist.nl_config_global;


namespace mame.netlist.factory
{
    public abstract class truthtable_base_element_t : factory.element_t
    {
        public std.vector<string> m_desc;
        public string m_family_name;


        public truthtable_base_element_t(string name, properties props)
            : base(name, props)
        {
            m_family_name = config.DEFAULT_LOGIC_FAMILY();
        }
    }


    public static class nlid_truthtable_global
    {
        //#define ENTRYY(n, m, s)    case (n * 100 + m): \
        //    { using xtype = devices::netlist_factory_truthtable_t<n, m>; \
        //        auto cs=s; \
        //        ret = plib::make_unique<xtype, host_arena>(desc.name, std::move(cs)); } \
        //        break

        //#define ENTRY(n, s) ENTRYY(n, 1, s); ENTRYY(n, 2, s); ENTRYY(n, 3, s); \
        //                    ENTRYY(n, 4, s); ENTRYY(n, 5, s); ENTRYY(n, 6, s); \
        //                    ENTRYY(n, 7, s); ENTRYY(n, 8, s); ENTRYY(n, 9, s); \
        //                    ENTRYY(n, 10, s)


        public static truthtable_base_element_t truthtable_create(tt_desc desc, properties props)
        {
            truthtable_base_element_t ret = null;

            switch (desc.ni * 100 + desc.no)
            {
                //ENTRY(1, props);
                case (1 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_1, u32_const_1>(desc.name, props); break;
                case (1 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_1, u32_const_2>(desc.name, props); break;
                case (1 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_1, u32_const_3>(desc.name, props); break;
                case (1 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_1, u32_const_4>(desc.name, props); break;
                case (1 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_1, u32_const_5>(desc.name, props); break;
                case (1 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_1, u32_const_6>(desc.name, props); break;
                case (1 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_1, u32_const_7>(desc.name, props); break;
                case (1 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_1, u32_const_8>(desc.name, props); break;
                case (1 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_1, u32_const_9>(desc.name, props); break;
                case (1 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_1, u32_const_10>(desc.name, props); break;

                //ENTRY(2, props);
                case (2 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_2, u32_const_1>(desc.name, props); break;
                case (2 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_2, u32_const_2>(desc.name, props); break;
                case (2 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_2, u32_const_3>(desc.name, props); break;
                case (2 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_2, u32_const_4>(desc.name, props); break;
                case (2 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_2, u32_const_5>(desc.name, props); break;
                case (2 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_2, u32_const_6>(desc.name, props); break;
                case (2 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_2, u32_const_7>(desc.name, props); break;
                case (2 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_2, u32_const_8>(desc.name, props); break;
                case (2 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_2, u32_const_9>(desc.name, props); break;
                case (2 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_2, u32_const_10>(desc.name, props); break;

                //ENTRY(3, props);
                case (3 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_3, u32_const_1>(desc.name, props); break;
                case (3 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_3, u32_const_2>(desc.name, props); break;
                case (3 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_3, u32_const_3>(desc.name, props); break;
                case (3 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_3, u32_const_4>(desc.name, props); break;
                case (3 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_3, u32_const_5>(desc.name, props); break;
                case (3 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_3, u32_const_6>(desc.name, props); break;
                case (3 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_3, u32_const_7>(desc.name, props); break;
                case (3 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_3, u32_const_8>(desc.name, props); break;
                case (3 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_3, u32_const_9>(desc.name, props); break;
                case (3 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_3, u32_const_10>(desc.name, props); break;

                //ENTRY(4, props);
                case (4 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_4, u32_const_1>(desc.name, props); break;
                case (4 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_4, u32_const_2>(desc.name, props); break;
                case (4 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_4, u32_const_3>(desc.name, props); break;
                case (4 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_4, u32_const_4>(desc.name, props); break;
                case (4 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_4, u32_const_5>(desc.name, props); break;
                case (4 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_4, u32_const_6>(desc.name, props); break;
                case (4 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_4, u32_const_7>(desc.name, props); break;
                case (4 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_4, u32_const_8>(desc.name, props); break;
                case (4 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_4, u32_const_9>(desc.name, props); break;
                case (4 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_4, u32_const_10>(desc.name, props); break;

                //ENTRY(5, props);
                case (5 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_5, u32_const_1>(desc.name, props); break;
                case (5 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_5, u32_const_2>(desc.name, props); break;
                case (5 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_5, u32_const_3>(desc.name, props); break;
                case (5 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_5, u32_const_4>(desc.name, props); break;
                case (5 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_5, u32_const_5>(desc.name, props); break;
                case (5 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_5, u32_const_6>(desc.name, props); break;
                case (5 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_5, u32_const_7>(desc.name, props); break;
                case (5 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_5, u32_const_8>(desc.name, props); break;
                case (5 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_5, u32_const_9>(desc.name, props); break;
                case (5 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_5, u32_const_10>(desc.name, props); break;

                //ENTRY(6, props);
                case (6 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_6, u32_const_1>(desc.name, props); break;
                case (6 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_6, u32_const_2>(desc.name, props); break;
                case (6 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_6, u32_const_3>(desc.name, props); break;
                case (6 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_6, u32_const_4>(desc.name, props); break;
                case (6 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_6, u32_const_5>(desc.name, props); break;
                case (6 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_6, u32_const_6>(desc.name, props); break;
                case (6 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_6, u32_const_7>(desc.name, props); break;
                case (6 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_6, u32_const_8>(desc.name, props); break;
                case (6 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_6, u32_const_9>(desc.name, props); break;
                case (6 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_6, u32_const_10>(desc.name, props); break;

                //ENTRY(7, props);
                case (7 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_7, u32_const_1>(desc.name, props); break;
                case (7 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_7, u32_const_2>(desc.name, props); break;
                case (7 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_7, u32_const_3>(desc.name, props); break;
                case (7 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_7, u32_const_4>(desc.name, props); break;
                case (7 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_7, u32_const_5>(desc.name, props); break;
                case (7 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_7, u32_const_6>(desc.name, props); break;
                case (7 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_7, u32_const_7>(desc.name, props); break;
                case (7 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_7, u32_const_8>(desc.name, props); break;
                case (7 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_7, u32_const_9>(desc.name, props); break;
                case (7 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_7, u32_const_10>(desc.name, props); break;

                //ENTRY(8, props);
                case (8 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_8, u32_const_1>(desc.name, props); break;
                case (8 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_8, u32_const_2>(desc.name, props); break;
                case (8 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_8, u32_const_3>(desc.name, props); break;
                case (8 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_8, u32_const_4>(desc.name, props); break;
                case (8 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_8, u32_const_5>(desc.name, props); break;
                case (8 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_8, u32_const_6>(desc.name, props); break;
                case (8 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_8, u32_const_7>(desc.name, props); break;
                case (8 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_8, u32_const_8>(desc.name, props); break;
                case (8 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_8, u32_const_9>(desc.name, props); break;
                case (8 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_8, u32_const_10>(desc.name, props); break;

                //ENTRY(9, props);
                case (9 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_9, u32_const_1>(desc.name, props); break;
                case (9 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_9, u32_const_2>(desc.name, props); break;
                case (9 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_9, u32_const_3>(desc.name, props); break;
                case (9 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_9, u32_const_4>(desc.name, props); break;
                case (9 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_9, u32_const_5>(desc.name, props); break;
                case (9 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_9, u32_const_6>(desc.name, props); break;
                case (9 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_9, u32_const_7>(desc.name, props); break;
                case (9 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_9, u32_const_8>(desc.name, props); break;
                case (9 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_9, u32_const_9>(desc.name, props); break;
                case (9 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_9, u32_const_10>(desc.name, props); break;

                //ENTRY(10, props);
                case (10 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_10, u32_const_1>(desc.name, props); break;
                case (10 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_10, u32_const_2>(desc.name, props); break;
                case (10 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_10, u32_const_3>(desc.name, props); break;
                case (10 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_10, u32_const_4>(desc.name, props); break;
                case (10 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_10, u32_const_5>(desc.name, props); break;
                case (10 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_10, u32_const_6>(desc.name, props); break;
                case (10 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_10, u32_const_7>(desc.name, props); break;
                case (10 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_10, u32_const_8>(desc.name, props); break;
                case (10 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_10, u32_const_9>(desc.name, props); break;
                case (10 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_10, u32_const_10>(desc.name, props); break;

                //ENTRY(11, props);
                case (11 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_11, u32_const_1>(desc.name, props); break;
                case (11 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_11, u32_const_2>(desc.name, props); break;
                case (11 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_11, u32_const_3>(desc.name, props); break;
                case (11 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_11, u32_const_4>(desc.name, props); break;
                case (11 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_11, u32_const_5>(desc.name, props); break;
                case (11 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_11, u32_const_6>(desc.name, props); break;
                case (11 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_11, u32_const_7>(desc.name, props); break;
                case (11 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_11, u32_const_8>(desc.name, props); break;
                case (11 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_11, u32_const_9>(desc.name, props); break;
                case (11 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_11, u32_const_10>(desc.name, props); break;

                //ENTRY(12, props);
                case (12 * 100 +  1): ret = new devices.netlist_factory_truthtable_t<u32_const_12, u32_const_1>(desc.name, props); break;
                case (12 * 100 +  2): ret = new devices.netlist_factory_truthtable_t<u32_const_12, u32_const_2>(desc.name, props); break;
                case (12 * 100 +  3): ret = new devices.netlist_factory_truthtable_t<u32_const_12, u32_const_3>(desc.name, props); break;
                case (12 * 100 +  4): ret = new devices.netlist_factory_truthtable_t<u32_const_12, u32_const_4>(desc.name, props); break;
                case (12 * 100 +  5): ret = new devices.netlist_factory_truthtable_t<u32_const_12, u32_const_5>(desc.name, props); break;
                case (12 * 100 +  6): ret = new devices.netlist_factory_truthtable_t<u32_const_12, u32_const_6>(desc.name, props); break;
                case (12 * 100 +  7): ret = new devices.netlist_factory_truthtable_t<u32_const_12, u32_const_7>(desc.name, props); break;
                case (12 * 100 +  8): ret = new devices.netlist_factory_truthtable_t<u32_const_12, u32_const_8>(desc.name, props); break;
                case (12 * 100 +  9): ret = new devices.netlist_factory_truthtable_t<u32_const_12, u32_const_9>(desc.name, props); break;
                case (12 * 100 + 10): ret = new devices.netlist_factory_truthtable_t<u32_const_12, u32_const_10>(desc.name, props); break;

                default:
                    string msg = new plib.pfmt("unable to create truthtable<{0},{2}>").op(desc.ni, desc.no);
                    nl_assert_always(false, msg);
                    break;
            }

            ret.m_desc = desc.desc;
            ret.m_family_name = !desc.family.empty() ? desc.family : config.DEFAULT_LOGIC_FAMILY();

            return ret;
        }
    }
}

namespace mame.netlist.devices
{
    //template<std::size_t m_NI, std::size_t m_NO>
    //class NETLIB_NAME(truthtable_t) : public device_t
    class nld_truthtable_t<size_t_m_NI, size_t_m_NO> : device_t
        where size_t_m_NI : u64_const, new()
        where size_t_m_NO : u64_const, new()
    {
        static readonly size_t m_NI = new size_t_m_NI().value;
        static readonly size_t m_NO = new size_t_m_NO().value;


        static readonly Type type_t = plib.pg.fast_type_for_bits((unsigned)(m_NO + m_NI));  //using type_t = typename plib::fast_type_for_bits<m_NO + m_NI>::type;


        static readonly size_t m_num_bits = m_NI;  //static constexpr const std::size_t m_num_bits = m_NI;
        static readonly size_t m_size = 1U << (int)m_num_bits;  //static constexpr const std::size_t m_size = (1 << (m_num_bits));
        static readonly UInt64 m_outmask = (1U << (int)m_NO) - 1U;  //static constexpr const type_t m_outmask = ((1 << m_NO) - 1);

        public class u64_const_m_size : u64_const { public UInt64 value { get { return m_size; } } }
        public class u64_const_m_size_multiply_m_NO : u64_const { public UInt64 value { get { return m_size * m_NO; } } }


        public class truthtable_t
        {
            public std.array<FlexPrim, u64_const_m_size> m_out_state = new std.array<FlexPrim, u64_const_m_size>();  //std::array<type_t, m_size> m_out_state;
            public std.array<uint_least8_t, u64_const_m_size_multiply_m_NO> m_timing_index;  //std::array<uint_least8_t, m_size * m_NO> m_timing_index;
            public std.array<netlist_time, u64_const_16> m_timing_nt = new std.array<netlist_time, u64_const_16>();  //std::array<netlist_time, 16> m_timing_nt;


            public truthtable_t()
            {
                m_timing_index = new std.array<uint_least8_t, u64_const_m_size_multiply_m_NO>();

                m_out_state.fill(new FlexPrim(type_t, 0U));
            }
        }


        plib.static_vector<logic_input_t, size_t_m_NI> m_I = new plib.static_vector<logic_input_t, size_t_m_NI>();  //plib::static_vector<logic_input_t, m_NI> m_I;
        plib.static_vector<logic_output_t, size_t_m_NO> m_Q = new plib.static_vector<logic_output_t, size_t_m_NO>();  //plib::static_vector<logic_output_t, m_NO> m_Q;

#if USE_TT_ALTERNATIVE
        state_var<type_t>   m_state;
#endif
        state_var<UInt64> m_ign;  //state_var<type_t> m_ign;
        truthtable_t m_ttp;
        /* FIXME: the family should provide the names of the power-terminals! */
        nld_power_pins m_power_pins;


        //template <class C>
        public nld_truthtable_t(object owner, string name,  //nld_truthtable_t(C &owner, const pstring &name, const pstring &model,  truthtable_t &ttp, const std::vector<pstring> &desc)
                string model,
                truthtable_t ttp, std.vector<string> desc)
            : base(owner, name, model)
        {
#if USE_TT_ALTERNATIVE
            , m_state(*this, "m_state", 0)
#endif
            m_ign = new state_var<UInt64>(this, "m_ign", 0);
            m_ttp = ttp;
            ///* FIXME: the family should provide the names of the power-terminals! */
            m_power_pins = new nld_power_pins(this);


            m_activate = incdec_active;  //m_activate = activate_delegate(& NETLIB_NAME(truthtable_t) :: incdec_active, this);
            set_hint_deactivate(true);
            init(desc);
        }


        void init(std.vector<string> desc)
        {
            string header = desc[0];

            std.vector<string> io = plib.pg.psplit(header,'|');
            // checks
            nl_assert_always(io.size() == 2, "too many '|'");
            std.vector<string> inout = plib.pg.psplit(io[0], ',');
            nl_assert_always(inout.size() == m_num_bits, "bitcount wrong");
            std.vector<string> outputs = plib.pg.psplit(io[1], ',');
            nl_assert_always(outputs.size() == m_NO, "output count wrong");

    #if !USE_TT_ALTERNATIVE
            for (size_t i = 0; i < m_NI; i++)
            {
                inout[i] = plib.pg.trim(inout[i]);
                m_I.emplace_back(new logic_input_t(this, inout[i], inputs));  //m_I.emplace_back(*this, inout[i], nldelegate(&NETLIB_NAME(truthtable_t)<m_NI, m_NO> :: inputs, this));
            }
    #else
            for (std::size_t i=0; i < m_NI; i++)
            {
                inout[i] = plib::trim(inout[i]);
            }
            if (0 < m_NI) m_I.emplace(0, *this, inout[0]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<0>, this));
            if (1 < m_NI) m_I.emplace(1, *this, inout[1]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<1>, this));
            if (2 < m_NI) m_I.emplace(2, *this, inout[2]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<2>, this));
            if (3 < m_NI) m_I.emplace(3, *this, inout[3]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<3>, this));
            if (4 < m_NI) m_I.emplace(4, *this, inout[4]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<4>, this));
            if (5 < m_NI) m_I.emplace(5, *this, inout[5]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<5>, this));
            if (6 < m_NI) m_I.emplace(6, *this, inout[6]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<6>, this));
            if (7 < m_NI) m_I.emplace(7, *this, inout[7]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<7>, this));
            if (8 < m_NI) m_I.emplace(8, *this, inout[8]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<8>, this));
            if (9 < m_NI) m_I.emplace(9, *this, inout[9]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<9>, this));
            if (10 < m_NI) m_I.emplace(10, *this, inout[10]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<10>, this));
            if (11 < m_NI) m_I.emplace(11, *this, inout[11]); //, nldelegate(&nld_truthtable_t<m_NI, m_NO>::update_N<11>, this));
    #endif
            for (size_t i = 0; i < m_NO; i++)
            {
                outputs[i] = plib.pg.trim(outputs[i]);
                m_Q.emplace_back(new logic_output_t(this, outputs[i]));  //m_Q.emplace_back(*this, outputs[i]);
                // Connect output "Q" to input "_Q" if this exists
                // This enables timed state without having explicit state ....
                string tmp = "_" + outputs[i];
                int idx = inout.IndexOf(tmp);  //const std::size_t idx = plib::container::indexof(inout, tmp);
                if (idx != -1)  //if (idx != plib::container::npos)
                    connect(m_Q.op(i), m_I.op((size_t)idx));
            }

            m_ign.op = 0;
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            int active_outputs = 0;
            m_ign.op = 0;
#if USE_TT_ALTERNATIVE
            m_state = 0;
#endif
            for (size_t i = 0; i < m_NI; ++i)
            {
                m_I.op(i).activate();
#if USE_TT_ALTERNATIVE
                m_state |= (m_I[i]() << i);
#endif
            }

            for (size_t i = 0; i < m_Q.size(); i++)  //for (auto &q : m_Q)
            {
                var q = m_Q.op(i);

                if (q.has_net() && !exec().nlstate().core_terms(q.net()).empty())
                    active_outputs++;
            }

            set_active_outputs(active_outputs);
        }


        //NETLIB_HANDLERI(inputs)
        void inputs()
        {
#if USE_TT_ALTERNATIVE
            m_state = 0;
            for (std::size_t i = 0; i < m_NI; ++i)
            {
                m_state |= (m_I[i]() << i);
            }
#endif
            process(true);
        }


#if USE_TT_ALTERNATIVE
        template <std::size_t N>
        void update_N() noexcept
        {
            m_state &= ~(1<<N);
            m_state |= (m_I[N]() << N);
            process<true>();
        }
#endif


        void incdec_active(bool a)
        {
            if (a)
            {
                process(false);
            }
            else
            {
                for (size_t i = 0; i < m_NI; i++)
                    m_I.op(i).inactivate();

                m_ign.op = (1U << (int)m_NI) - 1U;
            }
        }


        //template<bool doOUT>
        void process(bool doOUT)
        {
            netlist_time_ext mt = netlist_time_ext.zero();
            UInt64 nstate = 0;  //type_t nstate(0);
            UInt64 ign = m_ign.op;  //type_t ign(m_ign);

            if (doOUT)
            {
#if !USE_TT_ALTERNATIVE
                for (size_t Idx = 0; ign != 0; ign >>= 1, ++Idx)  //for (auto I = m_I.begin(); ign != 0; ign >>= 1, ++I)
                {
                    if ((ign & 1U) != 0)
                        m_I.op(Idx).activate();  //I->activate();
                }

                for (size_t i = 0; i < m_NI; i++)
                {
                    nstate |= (m_I.op(i).op() << (int)i);
                }
#else
                nstate = m_state;
                for (std::size_t i = 0; ign != 0; ign >>= 1, ++i)
                {
                    if (ign & 1)
                    {
                        nstate &= ~(1 << i);
                        m_I[i].activate();
                        nstate |= (m_I[i]() << i);
                    }
                }
#endif
            }
            else
            {
                for (size_t i = 0; i < m_NI; i++)
                {
                    m_I.op(i).activate();
                    nstate |= (m_I.op(i).op() << (int)i);
                    mt = netlist_time_ext.Max(this.m_I.op(i).net().next_scheduled_time(), mt);
                }
            }

            UInt64 outstate = m_ttp.m_out_state[nstate].u64;  //const type_t outstate(m_ttp.m_out_state[nstate]);
            UInt64 out_ = (outstate & m_outmask);  //type_t out(outstate & m_outmask);

            m_ign.op = outstate >> (int)m_NO;

            Pointer<uint_least8_t> t = m_ttp.m_timing_index.data() + (int)(nstate * m_NO);  //const auto *t(&m_ttp.m_timing_index[nstate * m_NO]);

            if (doOUT)
            {
                //for (std::size_t i = 0; i < m_NO; ++i)
                //  m_Q[i].push((out >> i) & 1, tim[t[i]]);
                this.push(out_, t);
            }
            else
            {
                var tim = m_ttp.m_timing_nt.data();
                for (size_t i = 0; i < m_NO; ++i)
                    m_Q.op(i).set_Q_time((UInt32)((out_ >> (int)i) & 1U), mt + tim[t[i]]);
            }

            ign = m_ign.op;
            for (size_t Idx = 0; ign != 0; ign >>= 1, ++Idx)  //for (auto I = m_I.begin(); ign != 0; ign >>= 1, ++I)
            {
                if ((ign & 1U) != 0)
                    m_I.op(Idx).inactivate();
            }

#if USE_TT_ALTERNATIVE
            m_state = nstate;
#endif
        }


        //template<typename T>
        void push(UInt64 v, Pointer<uint_least8_t> t)  //void push(const T &v, const std::uint_least8_t * t)
        {
            if (m_NO >= 1) m_Q.op(0).push((netlist_sig_t)((v >> 0) & 1), m_ttp.m_timing_nt[t[0]]);
            if (m_NO >= 2) m_Q.op(1).push((netlist_sig_t)((v >> 1) & 1), m_ttp.m_timing_nt[t[1]]);
            if (m_NO >= 3) m_Q.op(2).push((netlist_sig_t)((v >> 2) & 1), m_ttp.m_timing_nt[t[2]]);
            if (m_NO >= 4) m_Q.op(3).push((netlist_sig_t)((v >> 3) & 1), m_ttp.m_timing_nt[t[3]]);
            if (m_NO >= 5) m_Q.op(4).push((netlist_sig_t)((v >> 4) & 1), m_ttp.m_timing_nt[t[4]]);
            if (m_NO >= 6) m_Q.op(5).push((netlist_sig_t)((v >> 5) & 1), m_ttp.m_timing_nt[t[5]]);
            if (m_NO >= 7) m_Q.op(6).push((netlist_sig_t)((v >> 6) & 1), m_ttp.m_timing_nt[t[6]]);
            if (m_NO >= 8) m_Q.op(7).push((netlist_sig_t)((v >> 7) & 1), m_ttp.m_timing_nt[t[7]]);
            for (size_t i = 8; i < m_NO; i++)
                m_Q.op(i).push((netlist_sig_t)((v >> (int)i) & 1), m_ttp.m_timing_nt[t[i]]);
        }
    }


    // ----------------------------------------------------------------------------------------
    // int compatible bitset ....
    // ----------------------------------------------------------------------------------------
    //template <typename T>
    struct pbitset
    {
        //using type = T;


        uint_least64_t m_bs;  //T m_bs;


        //pbitset() noexcept : m_bs(0) { }
        public pbitset(uint_least64_t v) { m_bs = v; }  //pbitset(T v) noexcept : m_bs(v) { }


        public pbitset set() { m_bs = all_bits().op; return this; }  //pbitset &set() noexcept { *this = all_bits(); return *this; }
        public pbitset set(size_t bit) { m_bs |= (1U << (int)bit); return this; }  //pbitset &set(std::size_t bit) noexcept { m_bs |= (static_cast<T>(1) << bit); return *this; }
        //pbitset &reset() noexcept { *this = no_bits(); return *this; }
        //pbitset &reset(std::size_t bit) noexcept { m_bs &= ~(static_cast<T>(1) << bit); return *this; }

        public pbitset flip() { return new pbitset(~m_bs); }
        public pbitset flip(size_t bit) { return new pbitset(m_bs ^ (1U << (int)bit)); }  //pbitset flip(std::size_t bit) const noexcept { return pbitset(m_bs ^ (static_cast<T>(1) << bit)); }

        public size_t count()
        {
            throw new emu_unimplemented();
            //std::size_t ret(0);
            //for (T v = m_bs; v != 0; v = v >> 1)
            //{
            //    ret += (v & 1);
            //}
            //return ret;
        }

        //constexpr bool test(const std::size_t bit) const { return ((m_bs >> bit) & 1) == 1; }

        //operator T&() noexcept  { return m_bs; }
        //operator const T&() const noexcept { return m_bs; }
        public uint_least64_t op { get { return m_bs; } }

        public uint_least64_t as_uint() { return m_bs; }  //constexpr T as_uint() const noexcept { return m_bs; }

        public bool all() { return this.op == all_bits().op; }  //constexpr bool all() const noexcept { return *this == all_bits(); }

        /// \brief And all bits set with compressed bits from b
        ///
        /// Example:
        ///
        /// \code
        /// b = {b3,b2,b1,b0}
        /// v = {v7, 0, v5, 0, v3, v2, 0, 0}
        /// return {v7 & b3, 0, v5 & b2, 0, v3 & b1, v2 & b0, 0, 0}
        /// \endcode
        ///
        /// \returns pbitset
        ///
        public pbitset expand_and(pbitset b)
        {
            throw new emu_unimplemented();
#if false
            pbitset ret;
            T v( m_bs);

            for (size_t i = 0; v != 0; v = v >> 1, ++i)
            {
                if (v & 1)
                {
                    if (b.test(0))
                        ret.set(i);
                    b = b >> 1;
                }
            }
            return ret;
#endif
        }


        public static pbitset all_bits() { return new pbitset(uint_least64_t.MaxValue); }  //static constexpr pbitset all_bits() noexcept { return pbitset(~static_cast<T>(0)); }
        //static constexpr pbitset no_bits() noexcept{ return pbitset(static_cast<T>(0)); }
    }


    // ----------------------------------------------------------------------------------------
    // Truthtable parsing ....
    // ----------------------------------------------------------------------------------------

    //using tt_bitset = pbitset<std::uint_least64_t>;

    class packed_int
    {
        Pointer<FlexPrim> m_data;  //void *m_data;
        size_t m_size;


        public packed_int(Pointer<FlexPrim> data, size_t bits)  //packed_int(void *data, std::size_t bits) noexcept
        {
            m_data = data;
            m_size = bits;
        }


        public void set(size_t pos, uint_least64_t val)
        {
            //switch (m_size)
            //{
            //    case 8: static_cast<std::uint_least8_t  *>(m_data)[pos] = static_cast<std::uint_least8_t>(val); break;
            //    case 16: static_cast<std::uint_least16_t *>(m_data)[pos] = static_cast<std::uint_least16_t>(val); break;
            //    case 32: static_cast<std::uint_least32_t *>(m_data)[pos] = static_cast<std::uint_least32_t>(val); break;
            //    case 64: static_cast<std::uint_least64_t *>(m_data)[pos] = static_cast<std::uint_least64_t>(val); break;
            //    default: { }
            //}
            m_data[pos] = new FlexPrim(m_data[pos].type, val);
        }

        public uint_least64_t op(size_t pos)  //std::uint_least64_t operator[] (size_t pos) const noexcept  //std::uint_least64_t operator[] (size_t pos) const noexcept
        {
            //switch (m_size)
            //{
            //    case 8: return static_cast<std::uint_least8_t  *>(m_data)[pos];
            //    case 16: return static_cast<std::uint_least16_t *>(m_data)[pos];
            //    case 32: return static_cast<std::uint_least32_t *>(m_data)[pos];
            //    case 64: return static_cast<std::uint_least64_t *>(m_data)[pos];
            //    default:
            //        return 0; //should never happen
            //}
            return m_data[pos].u64;
        }

        public uint_least64_t mask()
        {
            //switch (m_size)
            //{
            //    case 8: return static_cast<std::uint_least8_t>(-1);
            //    case 16: return static_cast<std::uint_least16_t>(-1);
            //    case 32: return static_cast<std::uint_least32_t>(-1);
            //    case 64: return static_cast<std::uint_least64_t>(-1);
            //    default:
            //        return 0; //should never happen
            //}
            return FlexPrim.MaxValue(m_data.op.type).u64;
        }
    }


    class truthtable_parser
    {
        unsigned m_NO;
        unsigned m_NI;
        packed_int m_out_state;
        Pointer<uint_least8_t> m_timing;  //uint_least8_t  *m_timing;
        Pointer<netlist_time> m_timing_nt;  //netlist_time *m_timing_nt;

        size_t m_num_bits;
        size_t m_size;


        public truthtable_parser(unsigned NO, unsigned NI, packed_int outs, Pointer<uint_least8_t> timing, Pointer<netlist_time> timing_nt)  //truthtable_parser(unsigned NO, unsigned NI, packed_int outs, uint_least8_t *timing, netlist_time *timing_nt)
        {
            m_NO = NO;
            m_NI = NI;
            m_out_state = outs;
            m_timing = timing;
            m_timing_nt = timing_nt;
            m_num_bits = m_NI;
            m_size = 1U << (int)m_num_bits;
        }


        public void parse(std.vector<string> truthtable)
        {
            unsigned line = 0;

            string ttline = truthtable[line];
            line++;
            ttline = truthtable[line];
            line++;

            for (unsigned j = 0; j < m_size; j++)
                m_out_state.set(j, tt_bitset.all_bits().op);

            for (int j = 0; j < 16; j++)
                m_timing_nt[j] = netlist_time.zero();

            while (!ttline.empty())
            {
                std.vector<string> io = plib.pg.psplit(ttline,'|');
                // checks
                nl_assert_always(io.size() == 3, "io.count mismatch");
                std.vector<string> inout = plib.pg.psplit(io[0], ',');
                nl_assert_always(inout.size() == m_num_bits, "number of bits not matching");
                std.vector<string> out_ = plib.pg.psplit(io[1], ',');
                nl_assert_always(out_.size() == m_NO, "output count not matching");
                std.vector<string> times = plib.pg.psplit(io[2], ',');
                nl_assert_always(times.size() == m_NO, "timing count not matching");

                tt_bitset val = new tt_bitset(0);
                std.vector<uint_least8_t> tindex = new std.vector<uint_least8_t>();

                //
                // FIXME: evaluation of outputs should be done in parseline to
                //        enable the use of inputs for output values, i.e. "I1" or "~I1"
                //  in addition to "0" and "1".

                for (unsigned j = 0; j < m_NO; j++)
                {
                    string outs = plib.pg.trim(out_[j]);
                    if (outs == "1")
                        val.set(j);
                    else
                        nl_assert_always(outs == "0", "Unknown value (not 0 or 1");

                    // FIXME: error handling
                    netlist_time t = netlist_time.from_nsec(plib.pg.pstonum_ne_int64(false, plib.pg.trim(times[j]), out _));
                    uint_least8_t k = 0;
                    while (m_timing_nt[k] != netlist_time.zero() && m_timing_nt[k] != t)
                        k++;

                    m_timing_nt[k] = t;
                    tindex.push_back(k); //[j] = k;
                }

                parseline(0, inout, new tt_bitset(0), val.op, tindex);
                if (line < truthtable.size())
                    ttline = truthtable[line];
                else
                    ttline = "";

                line++;
            }

            // determine ignore mask by looping over all input combinations
            std.vector<tt_bitset> ign = new std.vector<tt_bitset>(m_size);
            foreach (tt_bitset x in ign)
                x.set();

            for (uint_least64_t i = 0; i < m_size; i++)
            {
                if (ign[i].all()) // not yet visited
                {
                    tt_bitset tign = calculate_ignored_inputs(new tt_bitset(i));

                    ign[i] = tign;

                    // don't need to recalculate similar ones
                    tt_bitset bitsk = new tt_bitset();
                    bitsk.set(tign.count());

                    for (uint_least64_t k = 0; k < bitsk.op; k++)
                    {
                        tt_bitset b = tign.expand_and(new tt_bitset(k));
                        ign[(i & tign.flip().op) | b.op] = tign;
                    }
                }
            }

            for (size_t i = 0; i < m_size; i++)
            {
                if (m_out_state.op(i) == m_out_state.mask())
                    throw new nl_exception(new plib.pfmt("truthtable: found element not set {0}\n").op(i) );
                m_out_state.set(i, m_out_state.op(i) | (ign[i].op << (int)m_NO));
            }
        }


        void parseline(unsigned cur, std.vector<string> list,
                tt_bitset state, uint_least64_t val, std.vector<uint_least8_t> timing_index)
        {
            string elem = plib.pg.trim(list[cur]);
            uint_least64_t start = 0;
            uint_least64_t end = 0;

            if (elem == "0")
            {
                start = 0;
                end = 0;
            }
            else if (elem == "1")
            {
                start = 1;
                end = 1;
            }
            else if (elem == "X")
            {
                start = 0;
                end = 1;
            }
            else
            {
                nl_assert_always(false, "unknown input value (not 0, 1, or X)");
            }

            for (uint_least64_t i = start; i <= end; i++)
            {
                tt_bitset nstate = state;
                if (i == 1)
                    nstate.set(cur);

                if (cur < m_num_bits - 1)
                {
                    parseline(cur + 1, list, nstate, val, timing_index);
                }
                else
                {
                    // cutoff previous inputs and outputs for ignore
                    if (m_out_state.op(nstate.op) != m_out_state.mask() &&  m_out_state.op(nstate.op) != val)
                    {
                        throw new nl_exception(new plib.pfmt("Error in truthtable: State {0} already set, {1} != {2}\n")
                                .op(nstate.as_uint(), m_out_state.op(nstate.op), val));
                    }

                    m_out_state.set(nstate.op, val);
                    for (size_t j = 0; j < m_NO; j++)
                        m_timing[nstate.op * m_NO + j] = timing_index[j];
                }
            }
        }


        tt_bitset calculate_ignored_inputs(tt_bitset state) { throw new emu_unimplemented(); }
    }


    // ----------------------------------------------------------------------------------------
    // Truthtable factory ....
    // ----------------------------------------------------------------------------------------
    //template<unsigned m_NI, unsigned m_NO>
    class netlist_factory_truthtable_t<unsigned_m_NI, unsigned_m_NO> : factory.truthtable_base_element_t
        where unsigned_m_NI : u32_const, new()
        where unsigned_m_NO : u32_const, new()
    {
        //typename nld_truthtable_t<m_NI, m_NO>::truthtable_t m_ttbl;


        static readonly unsigned m_NI = new unsigned_m_NI().value;
        static readonly unsigned m_NO = new unsigned_m_NO().value;

        public class u64_const_m_NI : u64_const { public UInt64 value { get { return m_NI; } } }
        public class u64_const_m_NO : u64_const { public UInt64 value { get { return m_NO; } } }


        nld_truthtable_t<u64_const_m_NI, u64_const_m_NO>.truthtable_t m_ttbl;  //device_arena::unique_ptr<typename nld_truthtable_t<m_NI, m_NO>::truthtable_t> m_ttbl;


        public netlist_factory_truthtable_t(string name, factory.properties props)
            : base(name, props)
        {
        }


        public override core_device_t make_device(device_arena pool, netlist_state_t anetlist, string name)  //device_arena::unique_ptr<core_device_t> make_device(device_arena &pool, netlist_state_t &anetlist, const pstring &name) override
        {
            //using tt_type = nld_truthtable_t<m_NI, m_NO>;

            if (m_ttbl == null)
            {
                m_ttbl = new nld_truthtable_t<u64_const_m_NI, u64_const_m_NO>.truthtable_t();  //m_ttbl = plib::make_unique<typename nld_truthtable_t<m_NI, m_NO>::truthtable_t>(pool);
                truthtable_parser desc_s = new truthtable_parser(m_NO, m_NI,  //truthtable_parser desc_s(m_NO, m_NI,
                        new packed_int(m_ttbl.m_out_state.data(), (size_t)sizeof_(m_ttbl.m_out_state.data()[0].type) * 8),  //packed_int(m_ttbl->m_out_state.data(), sizeof(m_ttbl->m_out_state[0]) * 8),
                        m_ttbl.m_timing_index.data(), m_ttbl.m_timing_nt.data());  //m_ttbl->m_timing_index.data(), m_ttbl->m_timing_nt.data());

                desc_s.parse(m_desc);
            }

            return new nld_truthtable_t<u64_const_m_NI, u64_const_m_NO>(anetlist, name, m_family_name, m_ttbl, m_desc);  //return plib::make_unique<tt_type>(pool, anetlist, name, m_family_name, *m_ttbl, m_desc);
        }
    }
}
