// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;


namespace mame.plib
{
    public abstract class dynamic_library_base
    {
        //template <typename R, typename... Args>
        public class function
        {
            //using calltype = R(*) (Args... args);


            Action<Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<Pointer<nl_fptype>>> m_sym;  //calltype m_sym;


            public function() { m_sym = null; }

            public function(dynamic_library_base dl, string name)
            {
                m_sym = dl.get_symbol(name);  //m_sym = dl.get_symbol<calltype>(name);
            }


            public void load(dynamic_library_base dl, string name) { throw new emu_unimplemented(); }
            //{
            //    m_sym = dl.get_symbol<calltype>(name);
            //}

            //R operator ()(Args&&... args) const
            //{
            //    return m_sym(std::forward<Args>(args)...);
            //    //return m_sym(args...);
            //}

            public bool resolved() { throw new emu_unimplemented(); }  //{ return m_sym != nullptr; }
        }


        bool m_is_loaded;


        protected dynamic_library_base() { m_is_loaded = false; }

        //virtual ~dynamic_library_base() = default;

        //dynamic_library_base(const dynamic_library_base &) = delete;
        //dynamic_library_base &operator=(const dynamic_library_base &) = delete;

        //dynamic_library_base(dynamic_library_base &&) noexcept = default;
        //dynamic_library_base &operator=(dynamic_library_base &&) noexcept = default;


        public bool isLoaded() { return m_is_loaded; }

        //template <typename T>
        public Action<Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<Pointer<nl_fptype>>> get_symbol(string name)  //T get_symbol(const pstring &name) const noexcept
        {
            return get_symbol_pointer(name);  //return reinterpret_cast<T>(get_symbol_pointer(name));
        }

        protected void set_loaded(bool v) { m_is_loaded = v; }

        protected abstract Action<Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<Pointer<nl_fptype>>> get_symbol_pointer(string name);  //virtual void *get_symbol_pointer(const pstring &name) const noexcept = 0;
    }


    class dynamic_library : dynamic_library_base
    {
        //void *m_lib;


        dynamic_library(string libname)
        {
            //m_lib = LoadLibrary(winapi_string(putf8string(libname)).c_str());
            set_loaded(true);
        }

        dynamic_library(string path, string libname)
        {
            //m_lib = LoadLibrary(winapi_string(putf8string(libname)).c_str());
            set_loaded(true);
        }

        //~dynamic_library() override;

        //PCOPYASSIGN(dynamic_library, delete)
        //PMOVEASSIGN(dynamic_library, default)


        protected override Action<Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<Pointer<nl_fptype>>> get_symbol_pointer(string name) { throw new emu_unimplemented(); }  //void *get_symbol_pointer(const pstring &name) const noexcept override;
    }


    public class static_library : dynamic_library_base
    {
        public struct symbol
        {
            public string name;
            public Action<Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<Pointer<nl_fptype>>> addr;  //void       *addr;
        }


        symbol [] m_syms;


        public static_library(symbol [] symbols)
        {
            m_syms = symbols;


            if (symbols != null)
                set_loaded(true);
        }


        protected override Action<Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<Pointer<nl_fptype>>> get_symbol_pointer(string name)  //void *get_symbol_pointer(const pstring &name) const noexcept override
        {
            symbol [] p_ = m_syms;  //const symbol *p = m_syms;
            //while (p->name[0] != 0)
            //{
            //    if (name == pstring(p->name))
            //        return p->addr;
            //    p++;
            //}
            foreach (var p in p_)
            {
                if (name == p.name)
                    return p.addr;
            }

            return null;
        }
    }
}
