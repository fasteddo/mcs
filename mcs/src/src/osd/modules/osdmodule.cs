// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    // a module_type is simply a pointer to its alloc function
    //typedef osd_module *(*module_type)();
    public delegate osd_module module_type();


    // ======================> osd_module
    public abstract class osd_module
    {
        string m_name;
        string m_type;


        public osd_module(string type, string name)
        {
            m_name = name;
            m_type = type;
        }


        public string name() { return m_name; }
        public string type() { return m_type; }


        public virtual bool probe() { return true; }


        public abstract int init(osd_options options);
        public virtual void exit() { }


        protected static module_type MODULE_DEFINITION(module_type creator) { return creator; }  //#define MODULE_DEFINITION(_id, _class)     extern const module_type _id ;      const module_type _id = &module_creator< _class >;
    }


    class osd_module_manager
    {
        //static const int MAX_MODULES = 64;


        List<osd_module> m_modules = new List<osd_module>();  //osd_module *m_modules[MAX_MODULES];
        List<osd_module> m_selected = new List<osd_module>();  //osd_module *m_selected[MAX_MODULES];


        public osd_module_manager()
        {
            //for (int i=0; i<MAX_MODULES; i++)
            //{
            //    m_modules[i]  = NULL;
            //    m_selected[i] = NULL;
            //}
        }

        //~osd_module_manager()
        //{
        //    for (int i = 0; m_modules[i] != NULL; i++)
        //    {
        //        global_free(m_modules[i]);
        //    }
        //}


        public void register_module(module_type mod_type)
        {
            osd_module module = mod_type();
            if (module.probe())
            {
                g.osd_printf_verbose("===> registered module {0} {1}\n", module.name(), module.type());

                //int i;
                //for (i = 0; m_modules[i] != NULL; i++)
                //    ;
                //m_modules[i] = module;
                m_modules.Add(module);
            }
            else
            {
                g.osd_printf_verbose("===> not supported {0} {1}\n", module.name(), module.type());
                //module->~osd_module();
                //global_free(module);
            }
        }

        public bool type_has_name(string type, string name)
        {
            return get_module_index(type, name) >= 0;
        }


        object get_module_generic(string type, string name)
        {
            int i = get_module_index(type, name);
            if (i >= 0)
                return m_modules[i];
            else
                return null;
        }


        //template<class C>
        C select_module<C>(string type, string name = "")
        {
            return (C)select_module(type, name);
        }


        public object select_module(string type, string name = "")
        {
            object m = get_module_generic(type, name);

            // FIXME: check if already exists!
            //int i;
            //for (i = 0; m_selected[i] != NULL; i++)
            //    ;
            //m_selected[i] = m;
            if (m != null)
            {
                if (!m_selected.Contains((osd_module)m))
                    m_selected.Add((osd_module)m);
            }

            return m;
        }


        public void get_module_names(string type, int max_unused, out int num, out List<string> names)
        {
            //num = 0;
            names = new List<string>();
            for (int i = 0; i < m_modules.Count; i++)
            {
                if (m_modules[i].type() == type) // && (num < max))
                {
                    //names[*num] = m_modules[i]->name();
                    //*num = *num + 1;
                    names.Add(m_modules[i].name());
                }
            }
            num = names.Count;
        }


        public void init(osd_options options)
        {
            for (int i = 0; i < m_selected.Count; i++)
            {
                //throw new emu_unimplemented();  - don't need null check when all modules exist
                if (m_selected[i] == null)
                    continue;

                m_selected[i].init(options);
            }
        }

        public void exit()
        {
            // Find count
            //int cnt;
            //for (cnt = 0; m_selected[cnt] != NULL; cnt++)
            //    ;
            for (int i = m_selected.Count - 1; i >= 0; i--)
            {
                //throw new emu_unimplemented();  - don't need null check when all modules exist
                if (m_selected[i] == null)
                    continue;

                m_selected[i].exit();
                m_selected[i] = null;
            }
        }


        int get_module_index(string type, string name)
        {
            for (int i = 0; i < m_modules.Count; i++)
            {
                if (m_modules[i].type() == type && (string.IsNullOrEmpty(name) || name == m_modules[i].name()))
                    return i;
            }

            return -1;
        }
    }


    //#define MODULE_DEFINITION(_id, _class)     extern const module_type _id ;      const module_type _id = &module_creator< _class >;
    //#define MODULE_NOT_SUPPORTED(_mod, _type, _name)     class _mod : public osd_module {     public:         _mod () : osd_module(_type, _name) { }         virtual int init(const osd_options &options) override { return -1; }         virtual bool probe() override { return false; }     };
}
