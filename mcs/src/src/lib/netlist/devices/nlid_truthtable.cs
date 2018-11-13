// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    namespace devices
    {
        public static class nlid_truthtable_global
        {
            //#define ENTRYY(n, m, s)    case (n * 100 + m): \
            //    { using xtype = netlist_factory_truthtable_t<n, m>; \
            //        ret = plib::palloc<xtype>(desc.name, desc.classname, desc.def_param, s); } break

            //#define ENTRY(n, s) ENTRYY(n, 1, s); ENTRYY(n, 2, s); ENTRYY(n, 3, s); \
            //                    ENTRYY(n, 4, s); ENTRYY(n, 5, s); ENTRYY(n, 6, s); \
            //                    ENTRYY(n, 7, s); ENTRYY(n, 8, s)

            public static void tt_factory_create(setup_t setup, tt_desc desc, string sourcefile)
            {
                netlist_base_factory_truthtable_t ret;

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
                if (desc.family != "")
                    ret.family = setup.family_from_model(desc.family);

                setup.factory().register_device(ret);  //std::unique_ptr<netlist_base_factory_truthtable_t>(ret));
            }
        }


        abstract class netlist_base_factory_truthtable_t : factory.element_t
        {
            std_vector<string> m_desc;
            logic_family_desc_t m_family;


            public netlist_base_factory_truthtable_t(string name, string classname, string def_param, string sourcefile)
                : base(name, classname, def_param, sourcefile)
            {
                m_family = nl_base_global.family_TTL();
            }

            ~netlist_base_factory_truthtable_t() { }


            public std_vector<string> desc { get { return m_desc; } set { m_desc = value; } }
            public logic_family_desc_t family { get { return m_family; } set { m_family = value; } }
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


            public override device_t Create(netlist_t anetlist, string name)
            {
                throw new emu_unimplemented();
#if false
                typedef nld_truthtable_t<m_NI, m_NO> tt_type;
                truthtable_parser desc_s(m_NO, m_NI, &m_ttbl.m_initialized,
                        packed_int(m_ttbl.m_outs, sizeof(m_ttbl.m_outs[0]) * 8),
                        m_ttbl.m_timing, m_ttbl.m_timing_nt);

                desc_s.parse(m_desc);
                return plib::owned_ptr<device_t>::Create<tt_type>(anetlist, name, m_family, m_ttbl, m_desc);
#endif
            }
        }
    } //namespace devices
} // namespace netlist
