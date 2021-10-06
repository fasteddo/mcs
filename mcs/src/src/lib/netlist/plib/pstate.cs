// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using size_t = System.UInt64;


// ----------------------------------------------------------------------------------------
// state saving ...
// ----------------------------------------------------------------------------------------

namespace mame.plib
{
    public class state_manager_t
    {
        public struct datatype_t
        {
            UInt32 m_size;
            bool m_is_integral;
            bool m_is_float;
            bool m_is_custom;

            public datatype_t(UInt32 bsize, bool bintegral, bool bfloat)
            {
                m_size = bsize;
                m_is_integral = bintegral;
                m_is_float = bfloat;
                m_is_custom = false;
            }

            public datatype_t(bool bcustom)
            {
                m_size = 0;
                m_is_integral = false;
                m_is_float = false;
                m_is_custom = bcustom;
            }


            //std::size_t size() const noexcept { return m_size; }
            //bool is_integral() const noexcept { return m_is_integral; }
            //bool is_float()    const noexcept { return m_is_float; }
            //bool is_custom()   const noexcept { return m_is_custom; }
        }


        //template<typename T>
        //static datatype_t dtype()
        //{
        //    return datatype_t(sizeof(T),
        //            plib::is_integral<T>::value || std::is_enum<T>::value,
        //            std::is_floating_point<T>::value);
        //}


        public interface callback_t
        {
            //using list_t = std::vector<callback_t *>;

            //callback_t() = default;
            //~callback_t() = default;
            //COPYASSIGNMOVE(callback_t, default)


            void register_state(state_manager_t manager, string module);
            void on_pre_save(state_manager_t manager);
            void on_post_load(state_manager_t manager);
        }


        class entry_t
        {
            //using list_t = std::vector<entry_t>;


            string m_name;
            datatype_t m_dt;
            object m_owner;  //const void *        m_owner;
            public callback_t m_callback;  //callback_t *        m_callback;
            size_t m_count;
            object m_ptr;  //void *              m_ptr;


            entry_t(string stname, datatype_t dt, object owner, int count, object ptr)  //entry_t(const pstring &stname, const datatype_t &dt, const void *owner, const std::size_t count, void *ptr)
            {
                m_name = stname;
                m_dt = dt;
                m_owner = owner;
                m_callback = null;
                m_count = (UInt32)count;
                m_ptr = ptr;
            }

            entry_t(string stname, object owner, callback_t callback)  //entry_t(const pstring &stname, const void *owner, callback_t *callback)
            {
                m_name = stname;
                m_dt = new datatype_t(true);
                m_owner = owner;
                m_callback = callback;
                m_count = 0;
                m_ptr = null;
            }


            //pstring name() const noexcept { return m_name; }
            //datatype_t dt() const noexcept { return m_dt; }
            public object owner() { return m_owner; }  //const void * owner() const noexcept { return m_owner; }
            //callback_t * callback() const noexcept { return m_callback; }
            //std::size_t count() const noexcept { return m_count; }
            //void * ptr() const noexcept { return m_ptr; }
        }


        //struct saver_t


        std.vector<entry_t> m_save = new std.vector<entry_t>();  //entry_t::list_t m_save;
        std.vector<entry_t> m_custom = new std.vector<entry_t>();  //entry_t::list_t m_custom;


        public state_manager_t() { }


        //template<typename C>
        public void save_item(object owner, object state, string stname)  //void save_item(const void *owner, C &state, const pstring &stname)
        {
            //throw new emu_unimplemented();
#if false
            save_item_dispatch(owner, state, stname);
#endif
        }

        //template<typename C, std::size_t N> void save_item(const void *owner, C (&state)[N], const pstring &stname)
        //template<typename C> void save_item(const void *owner, C *state, const pstring &stname, const std::size_t count)
        //template<typename C> void save_item(const void *owner, std::vector<C> &v, const pstring &stname)


        public void pre_save()
        {
            foreach (var s in m_custom)
                s.m_callback.on_pre_save(this);
        }


        public void post_load()
        {
            foreach (var s in m_custom)
                s.m_callback.on_post_load(this);
        }


        public void remove_save_items(object owner)  //void remove_save_items(const void *owner)
        {
            int i = m_save.Count;  //auto i = m_save.end();
            while (i > 0)  //while (i != m_save.begin())
            {
                i--;
                if (m_save[i].owner() == owner)  //if (i->owner() == owner)
                    { m_save.erase(i); i--; }  //i = m_save.erase(i);
            }

            i = m_custom.Count;  //i = m_custom.end();
            while (i > 0)  //while (i > m_custom.begin())
            {
                i--;
                if (m_custom[i].owner() == owner)  //if (i->owner() == owner)
                    { m_custom.erase(i); i--; }  //i = m_custom.erase(i);
            }
        }


        //const entry_t::list_t &save_list() const

        public void save_state_ptr(object owner, string stname, object dt, size_t count, object ptr)  //void save_state_ptr(const void *owner, const pstring &stname, const datatype_t &dt, const std::size_t count, void *ptr);
        {
            //throw new emu_unimplemented();
#if false
#endif
        }


        //template<typename C>
        //std::enable_if_t<plib::is_integral<C>::value || std::is_enum<C>::value
        //        || plib::is_floating_point<C>::value>
        //save_item_dispatch(const void *owner, C &state, const pstring &stname)


        //template<typename C>
        //std::enable_if_t<!(plib::is_integral<C>::value || std::is_enum<C>::value
        //        || plib::is_floating_point<C>::value)>
        //save_item_dispatch(const void *owner, C &state, const pstring &stname)
    }
}
