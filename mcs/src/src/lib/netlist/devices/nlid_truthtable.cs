// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_base_t = mame.netlist.netlist_state_t;


namespace mame.netlist
{
    namespace devices
    {
        public static class nlid_truthtable_global
        {
            //#define ENTRYY(n, m, s)    case (n * 100 + m): \
            //    { using xtype = netlist_factory_truthtable_t<n, m>; \
            //        ret = plib::make_unique<xtype>(desc.name, desc.classname, desc.def_param, s); } break

            //#define ENTRY(n, s) ENTRYY(n, 1, s); ENTRYY(n, 2, s); ENTRYY(n, 3, s); \
            //                    ENTRYY(n, 4, s); ENTRYY(n, 5, s); ENTRYY(n, 6, s); \
            //                    ENTRYY(n, 7, s); ENTRYY(n, 8, s)

            /* the returned element is still missing a pointer to the family ... */
            public static netlist_base_factory_truthtable_t tt_factory_create(tt_desc desc, string sourcefile)  //plib::unique_ptr<netlist_base_factory_truthtable_t> tt_factory_create(tt_desc &desc, const pstring &sourcefile);
            {
                netlist_base_factory_truthtable_t ret;  //plib::unique_ptr<netlist_base_factory_truthtable_t> ret;

                //switch (desc.ni * 100 + desc.no)
                //{
                //    ENTRY(1, sourcefile);
                //    ENTRY(2, sourcefile);
                //    ENTRY(3, sourcefile);
                //    ENTRY(4, sourcefile);
                //    ENTRY(5, sourcefile);
                //    ENTRY(6, sourcefile);
                //    ENTRY(7, sourcefile);
                //    ENTRY(8, sourcefile);
                //    ENTRY(9, sourcefile);
                //    ENTRY(10, sourcefile);
                //    ENTRY(11, sourcefile);
                //    ENTRY(12, sourcefile);
                //    default:
                //        pstring msg = plib::pfmt("unable to create truthtable<{1},{2}>")(desc.ni)(desc.no);
                //        nl_assert_always(false, msg);
                //}

                // re-write of the above case statement
                if (desc.ni >= 1 && desc.ni <= 12 &&
                    desc.no >= 1 && desc.no <= 8)
                {
                    ret = new netlist_factory_truthtable_t(desc.ni, desc.no, desc.name, desc.classname, desc.def_param, sourcefile);
                }
                else
                {
                    string msg = new plib.pfmt("unable to create truthtable<{0},{1}>").op(desc.ni, desc.no);
                    nl_base_global.nl_assert_always(false, msg);
                    ret = new netlist_factory_truthtable_t(0, 0, null, null, null, null);
                }


                ret.desc = desc.desc;
                ret.family_name = desc.family;

                return ret;
            }
        }


        public abstract class netlist_base_factory_truthtable_t : factory.element_t
        {
            std.vector<string> m_desc;
            string m_family_name;
            logic_family_desc_t m_family_desc;


            public netlist_base_factory_truthtable_t(string name, string classname, string def_param, string sourcefile)
                : base(name, classname, def_param, sourcefile)
            {
                m_family_desc = nl_base_global.family_TTL();
            }


            public std.vector<string> desc { get { return m_desc; } set { m_desc = value; } }
            public string family_name { get { return m_family_name; } set { m_family_name = value; } }
            public logic_family_desc_t family_desc { get { return m_family_desc; } set { m_family_desc = value; } }
        }


        // ----------------------------------------------------------------------------------------
        // Truthtable factory ....
        // ----------------------------------------------------------------------------------------
        //template<unsigned m_NI, unsigned m_NO>
        class netlist_factory_truthtable_t : netlist_base_factory_truthtable_t
        {
            //typename nld_truthtable_t<m_NI, m_NO>::truthtable_t m_ttbl;


            // template parameters
            UInt32 m_NI;
            UInt32 m_NO;


            public netlist_factory_truthtable_t(UInt32 m_NI, UInt32 m_NO, string name, string classname, string def_param, string sourcefile)
                : base(name, classname, def_param, sourcefile)
            {
                this.m_NI = m_NI;
                this.m_NO = m_NO;
            }


            public override device_t Create(netlist_state_t anetlist, string name)  //poolptr<device_t> Create(netlist_state_t &anetlist, const pstring &name) override
            {
                throw new emu_unimplemented();
            }
        }
    } //namespace devices
} // namespace netlist
