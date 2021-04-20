// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using unsigned = System.UInt32;


namespace mame.netlist
{
    namespace factory
    {
        public abstract class truthtable_base_element_t : factory.element_t
        {
            std.vector<string> m_desc;
            string m_family_name;


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
            //                    ENTRYY(n, 7, s); ENTRYY(n, 8, s)


            public static truthtable_base_element_t truthtable_create(tt_desc desc, properties props)
            {
                truthtable_base_element_t ret;

                // re-write the case statement
                throw new emu_unimplemented();
#if false
                //switch (desc.ni * 100 + desc.no)
                //{
                //    ENTRY(1, props);
                //    ENTRY(2, props);
                //    ENTRY(3, props);
                //    ENTRY(4, props);
                //    ENTRY(5, props);
                //    ENTRY(6, props);
                //    ENTRY(7, props);
                //    ENTRY(8, props);
                //    ENTRY(9, props);
                //    ENTRY(10, props);
                //    ENTRY(11, props);
                //    ENTRY(12, props);
                //    default:
                //        pstring msg = plib::pfmt("unable to create truthtable<{1},{2}>")(desc.ni)(desc.no);
                //        nl_assert_always(false, msg.c_str());
                //}

                ret->m_desc = desc.desc;
                ret->m_family_name = (!desc.family.empty() ? desc.family : pstring(config::DEFAULT_LOGIC_FAMILY()));

                return ret;
#endif
            }
        }
    }


    namespace devices
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


            netlist_factory_truthtable_t(unsigned m_NI, unsigned m_NO, string name, factory.properties props)
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
}
