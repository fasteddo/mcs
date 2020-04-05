// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    // ----------------------------------------------------------------------------------------
    // pdynlib: dynamic loading of libraries  ...
    // ----------------------------------------------------------------------------------------
    public class dynlib  // : public nocopyassignmove
    {
        //bool m_isLoaded;
        //void *m_lib;


        public dynlib(string libname)
        {
            //throw new emu_unimplemented();
#if false
#endif
        }

        public dynlib(string path, string libname)
        {
            //throw new emu_unimplemented();
#if false
#endif
        }

        ~dynlib()
        {
            //throw new emu_unimplemented();
#if false
#endif
        }

        //COPYASSIGNMOVE(dynlib, delete)


        public bool isLoaded()
        {
            return false;
            //throw new emu_unimplemented();
#if false
#endif
        }


        //template <typename T>
        //T getsym(const pstring name)
        //{
        //    return reinterpret_cast<T>(getsym_p(name));
        //}

        //void *getsym_p(const pstring name);
    }


    //template <typename R, typename... Args>
    class dynproc
    {
        //using calltype = R(*) (Args... args);

        //calltype m_sym;


        dynproc()
        {
            throw new emu_unimplemented();
#if false
            m_sym(nullptr)
#endif
        }

        dynproc(dynlib dl, string name)
        {
            throw new emu_unimplemented();
#if false
            m_sym = dl.getsym<calltype>(name);
#endif
        }


        //void load(dynlib &dl, const pstring &name)
        //{
        //    m_sym = dl.getsym<calltype>(name);
        //}

        //R operator ()(Args&&... args) const
        //{
        //    return m_sym(std::forward<Args>(args)...);
        //    //return m_sym(args...);
        //}

        //bool resolved() { return m_sym != nullptr; }
    }
}
