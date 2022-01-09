// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;


namespace mame.plib
{
    // ----------------------------------------------------------------------------------------
    // pdynlib: dynamic loading of libraries  ...
    // ----------------------------------------------------------------------------------------

    public abstract class dynlib_base
    {
        bool m_is_loaded;


        protected dynlib_base() { m_is_loaded = false; }

        //virtual ~dynlib_base() = default;

        //PCOPYASSIGN(dynlib_base, delete)
        //PMOVEASSIGN(dynlib_base, default)

        public bool isLoaded() { return m_is_loaded; }

        //template <typename T>
        public Action<Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<Pointer<nl_fptype>>> getsym(string name)  //T getsym(const pstring &name) const noexcept
        {
            return getsym_p(name);  //return reinterpret_cast<T>(getsym_p(name));
        }

        protected void set_loaded(bool v) { m_is_loaded = v; }

        protected abstract Action<Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<Pointer<nl_fptype>>> getsym_p(string name);  //virtual void *getsym_p(const pstring &name) const noexcept = 0;
    }


    //class dynlib : dynlib_base


    public struct dynlib_static_sym
    {
        public string name;
        public Action<Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<Pointer<nl_fptype>>> addr;
    }


    class dynlib_static : dynlib_base
    {
        dynlib_static_sym [] m_syms;


        public dynlib_static(dynlib_static_sym [] syms)
        {
            m_syms = syms;


            if (syms != null)
                set_loaded(true);
        }


        protected override Action<Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<Pointer<nl_fptype>>> getsym_p(string name)  //void *getsym_p(const pstring &name) const noexcept override
        {
            dynlib_static_sym [] p_ = m_syms;  //const dynlib_static_sym *p = m_syms;
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


    //template <typename R, typename... Args>
    class dynproc
    {
        //using calltype = R(*) (Args... args);


        //calltype m_sym;
        Action<Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<nl_fptype>, Pointer<Pointer<nl_fptype>>> m_sym;


        public dynproc() { m_sym = null; }

        public dynproc(dynlib_base dl, string name)
        {
            m_sym = dl.getsym(name);  //m_sym = dl.getsym<calltype>(name);
        }

        public void load(dynlib_base dl, string name)
        {
            m_sym = dl.getsym(name);  //m_sym = dl.getsym<calltype>(name);
        }

        public void op(Pointer<nl_fptype> arg1, Pointer<nl_fptype> arg2, Pointer<nl_fptype> arg3, Pointer<nl_fptype> arg4, Pointer<Pointer<nl_fptype>> arg5)  //R operator ()(Args&&... args) const
        {
            m_sym(arg1, arg2, arg3, arg4, arg5);  //return m_sym(std::forward<Args>(args)...);
            ////return m_sym(args...);
        }

        public bool resolved() { return m_sym != null; }
    }
}
