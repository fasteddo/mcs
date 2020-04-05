// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


// ----------------------------------------------------------------------------------------
// state saving ...
// ----------------------------------------------------------------------------------------

namespace mame.plib
{
    public class state_manager_t
    {
        public struct datatype_t
        {
            int size;
            bool is_integral;
            bool is_float;
            bool is_custom;

            public datatype_t(int bsize, bool bintegral, bool bfloat)
            {
                size = bsize;
                is_integral = bintegral;
                is_float = bfloat;
                is_custom = false;
            }

            public datatype_t(bool bcustom)
            {
                size = 0;
                is_integral = false;
                is_float = false;
                is_custom = bcustom;
            }
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
            //using list_t = std::vector<plib::unique_ptr<entry_t>>;

            string             m_name;
            datatype_t    m_dt;
            object m_owner;  //const void *        m_owner;
            public callback_t m_callback;  //callback_t *        m_callback;
            int m_count;
            object m_ptr;  //void *              m_ptr;

        
            entry_t(string stname, datatype_t dt, object owner, int count, object ptr)
            {
                m_name = stname;
                m_dt = dt;
                m_owner = owner;
                m_callback = null;
                m_count = count;
                m_ptr = ptr;
            }

            entry_t(string stname, object owner, callback_t callback)
            {
                m_name = stname;
                m_dt = new datatype_t(true);
                m_owner = owner;
                m_callback = callback;
                m_count = 0;
                m_ptr = null;
            }
        }


        //entry_t::list_t m_save;
        std.vector<entry_t> m_custom;  //entry_t::list_t m_custom;  //entry_t::list_t m_custom;


        public state_manager_t() { }


        //template<typename C>
        public void save_item(object owner, object state, string stname)  //void save_item(const void *owner, C &state, const pstring &stname)
        {
            //throw new emu_unimplemented();
#if false
            save_state_ptr(owner, stname, datatype_f<C>::f(), v.size(), v.data());
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


        public void remove_save_items(object owner) { throw new emu_unimplemented(); }


        //const entry_t::list_t &save_list() const

        public void save_state_ptr(object owner, string stname, object dt, UInt32 count, object ptr)  //void save_state_ptr(const void *owner, const pstring &stname, const datatype_t &dt, const std::size_t count, void *ptr);
        {
            //throw new emu_unimplemented();
#if false
#endif
        }
    }


    //template<> void state_manager_t::save_item(const void *owner, callback_t &state, const pstring &stname);
}
