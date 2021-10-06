// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using unsigned = System.UInt32;


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
                case (1 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(1,  1, desc.name, props); break;
                case (1 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(1,  2, desc.name, props); break;
                case (1 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(1,  3, desc.name, props); break;
                case (1 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(1,  4, desc.name, props); break;
                case (1 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(1,  5, desc.name, props); break;
                case (1 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(1,  6, desc.name, props); break;
                case (1 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(1,  7, desc.name, props); break;
                case (1 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(1,  8, desc.name, props); break;
                case (1 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(1,  9, desc.name, props); break;
                case (1 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(1, 10, desc.name, props); break;

                //ENTRY(2, props);
                case (2 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(2,  1, desc.name, props); break;
                case (2 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(2,  2, desc.name, props); break;
                case (2 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(2,  3, desc.name, props); break;
                case (2 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(2,  4, desc.name, props); break;
                case (2 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(2,  5, desc.name, props); break;
                case (2 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(2,  6, desc.name, props); break;
                case (2 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(2,  7, desc.name, props); break;
                case (2 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(2,  8, desc.name, props); break;
                case (2 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(2,  9, desc.name, props); break;
                case (2 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(2, 10, desc.name, props); break;

                //ENTRY(3, props);
                case (3 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(3,  1, desc.name, props); break;
                case (3 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(3,  2, desc.name, props); break;
                case (3 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(3,  3, desc.name, props); break;
                case (3 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(3,  4, desc.name, props); break;
                case (3 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(3,  5, desc.name, props); break;
                case (3 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(3,  6, desc.name, props); break;
                case (3 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(3,  7, desc.name, props); break;
                case (3 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(3,  8, desc.name, props); break;
                case (3 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(3,  9, desc.name, props); break;
                case (3 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(3, 10, desc.name, props); break;

                //ENTRY(4, props);
                case (4 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(4,  1, desc.name, props); break;
                case (4 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(4,  2, desc.name, props); break;
                case (4 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(4,  3, desc.name, props); break;
                case (4 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(4,  4, desc.name, props); break;
                case (4 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(4,  5, desc.name, props); break;
                case (4 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(4,  6, desc.name, props); break;
                case (4 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(4,  7, desc.name, props); break;
                case (4 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(4,  8, desc.name, props); break;
                case (4 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(4,  9, desc.name, props); break;
                case (4 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(4, 10, desc.name, props); break;

                //ENTRY(5, props);
                case (5 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(5,  1, desc.name, props); break;
                case (5 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(5,  2, desc.name, props); break;
                case (5 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(5,  3, desc.name, props); break;
                case (5 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(5,  4, desc.name, props); break;
                case (5 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(5,  5, desc.name, props); break;
                case (5 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(5,  6, desc.name, props); break;
                case (5 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(5,  7, desc.name, props); break;
                case (5 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(5,  8, desc.name, props); break;
                case (5 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(5,  9, desc.name, props); break;
                case (5 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(5, 10, desc.name, props); break;

                //ENTRY(6, props);
                case (6 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(6,  1, desc.name, props); break;
                case (6 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(6,  2, desc.name, props); break;
                case (6 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(6,  3, desc.name, props); break;
                case (6 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(6,  4, desc.name, props); break;
                case (6 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(6,  5, desc.name, props); break;
                case (6 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(6,  6, desc.name, props); break;
                case (6 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(6,  7, desc.name, props); break;
                case (6 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(6,  8, desc.name, props); break;
                case (6 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(6,  9, desc.name, props); break;
                case (6 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(6, 10, desc.name, props); break;

                //ENTRY(7, props);
                case (7 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(7,  1, desc.name, props); break;
                case (7 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(7,  2, desc.name, props); break;
                case (7 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(7,  3, desc.name, props); break;
                case (7 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(7,  4, desc.name, props); break;
                case (7 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(7,  5, desc.name, props); break;
                case (7 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(7,  6, desc.name, props); break;
                case (7 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(7,  7, desc.name, props); break;
                case (7 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(7,  8, desc.name, props); break;
                case (7 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(7,  9, desc.name, props); break;
                case (7 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(7, 10, desc.name, props); break;

                //ENTRY(8, props);
                case (8 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(8,  1, desc.name, props); break;
                case (8 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(8,  2, desc.name, props); break;
                case (8 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(8,  3, desc.name, props); break;
                case (8 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(8,  4, desc.name, props); break;
                case (8 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(8,  5, desc.name, props); break;
                case (8 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(8,  6, desc.name, props); break;
                case (8 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(8,  7, desc.name, props); break;
                case (8 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(8,  8, desc.name, props); break;
                case (8 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(8,  9, desc.name, props); break;
                case (8 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(8, 10, desc.name, props); break;

                //ENTRY(9, props);
                case (9 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(9,  1, desc.name, props); break;
                case (9 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(9,  2, desc.name, props); break;
                case (9 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(9,  3, desc.name, props); break;
                case (9 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(9,  4, desc.name, props); break;
                case (9 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(9,  5, desc.name, props); break;
                case (9 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(9,  6, desc.name, props); break;
                case (9 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(9,  7, desc.name, props); break;
                case (9 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(9,  8, desc.name, props); break;
                case (9 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(9,  9, desc.name, props); break;
                case (9 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(9, 10, desc.name, props); break;

                //ENTRY(10, props);
                case (10 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(10,  1, desc.name, props); break;
                case (10 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(10,  2, desc.name, props); break;
                case (10 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(10,  3, desc.name, props); break;
                case (10 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(10,  4, desc.name, props); break;
                case (10 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(10,  5, desc.name, props); break;
                case (10 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(10,  6, desc.name, props); break;
                case (10 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(10,  7, desc.name, props); break;
                case (10 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(10,  8, desc.name, props); break;
                case (10 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(10,  9, desc.name, props); break;
                case (10 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(10, 10, desc.name, props); break;

                //ENTRY(11, props);
                case (11 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(11,  1, desc.name, props); break;
                case (11 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(11,  2, desc.name, props); break;
                case (11 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(11,  3, desc.name, props); break;
                case (11 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(11,  4, desc.name, props); break;
                case (11 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(11,  5, desc.name, props); break;
                case (11 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(11,  6, desc.name, props); break;
                case (11 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(11,  7, desc.name, props); break;
                case (11 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(11,  8, desc.name, props); break;
                case (11 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(11,  9, desc.name, props); break;
                case (11 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(11, 10, desc.name, props); break;

                //ENTRY(12, props);
                case (12 * 100 +  1): ret = new devices.netlist_factory_truthtable_t(12,  1, desc.name, props); break;
                case (12 * 100 +  2): ret = new devices.netlist_factory_truthtable_t(12,  2, desc.name, props); break;
                case (12 * 100 +  3): ret = new devices.netlist_factory_truthtable_t(12,  3, desc.name, props); break;
                case (12 * 100 +  4): ret = new devices.netlist_factory_truthtable_t(12,  4, desc.name, props); break;
                case (12 * 100 +  5): ret = new devices.netlist_factory_truthtable_t(12,  5, desc.name, props); break;
                case (12 * 100 +  6): ret = new devices.netlist_factory_truthtable_t(12,  6, desc.name, props); break;
                case (12 * 100 +  7): ret = new devices.netlist_factory_truthtable_t(12,  7, desc.name, props); break;
                case (12 * 100 +  8): ret = new devices.netlist_factory_truthtable_t(12,  8, desc.name, props); break;
                case (12 * 100 +  9): ret = new devices.netlist_factory_truthtable_t(12,  9, desc.name, props); break;
                case (12 * 100 + 10): ret = new devices.netlist_factory_truthtable_t(12, 10, desc.name, props); break;

                default:
                    string msg = new plib.pfmt("unable to create truthtable<{0},{2}>").op(desc.ni, desc.no);
                    nl_config_global.nl_assert_always(false, msg);
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


    // ----------------------------------------------------------------------------------------
    // Truthtable factory ....
    // ----------------------------------------------------------------------------------------
    //template<unsigned m_NI, unsigned m_NO>
    class netlist_factory_truthtable_t : factory.truthtable_base_element_t
    {
        //typename nld_truthtable_t<m_NI, m_NO>::truthtable_t m_ttbl;


        // template parameters
        unsigned m_NI;
        unsigned m_NO;


        //device_arena::unique_ptr<typename nld_truthtable_t<m_NI, m_NO>::truthtable_t> m_ttbl;


        public netlist_factory_truthtable_t(unsigned m_NI, unsigned m_NO, string name, factory.properties props)
            : base(name, props)
        {
            this.m_NI = m_NI;
            this.m_NO = m_NO;
        }


        public override core_device_t make_device(device_arena pool, netlist_state_t anetlist, string name)  //device_arena::unique_ptr<core_device_t> make_device(device_arena &pool, netlist_state_t &anetlist, const pstring &name) override
        {
            throw new emu_unimplemented();
        }
    }
}
