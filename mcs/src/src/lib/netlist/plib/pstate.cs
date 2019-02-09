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


        //template<typename T> struct datatype_f
        //{
        //    static inline const datatype_t f()
        //    {
        //        return datatype_t(sizeof(T),
        //                plib::is_integral<T>::value || std::is_enum<T>::value,
        //                std::is_floating_point<T>::value);
        //    }
        //};


        public interface callback_t
        {
            //using list_t = std::vector<callback_t *>;

            //virtual ~callback_t();

            //virtual void register_state(state_manager_t &manager, const pstring &module) = 0;
            void on_pre_save();
            void on_post_load();
        }


        class entry_t
        {
            //using list_t = std::vector<std::unique_ptr<entry_t>>;

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

            //~entry_t() { }
        }


        //entry_t::list_t m_save;
        std.vector<entry_t> m_custom;  //entry_t::list_t m_custom;  //entry_t::list_t m_custom;


        public state_manager_t()
        {
        }

        //~state_manager_t()
        //{
        //    m_save.clear();
        //    m_custom.clear();
        //}


        //template<typename C>
        public void save_item(object owner, object state, string stname)  //void save_item(const void *owner, C &state, const pstring &stname)
        {
            //throw new emu_unimplemented();
#if false
            save_state_ptr(owner, stname, datatype_f<C>::f(), v.size(), v.data());
#endif
        }

        //template<typename C, std::size_t N> void save_item(const void *owner, C (&state)[N], const pstring &stname)
        //{
        //    save_state_ptr(owner, stname, datatype_f<C>::f(), N, &(state[0]));
        //}

        //template<typename C> void save_item(const void *owner, C *state, const pstring &stname, const std::size_t count)
        //{
        //    save_state_ptr(owner, stname, datatype_f<C>::f(), count, state);
        //}

        //template<typename C>
        //void save_item(const void *owner, std::vector<C> &v, const pstring &stname)
        //{
        //    save_state(v.data(), owner, stname, v.size());
        //}


        public void pre_save()
        {
            foreach (var s in m_custom)
                s.m_callback.on_pre_save();
        }


        public void post_load()
        {
            foreach (var s in m_custom)
                s.m_callback.on_post_load();
        }


        //void remove_save_items(const void *owner);

        //const entry_t::list_t &save_list() const { return m_save; }

        public void save_state_ptr(object owner, string stname, object dt, UInt32 count, object ptr)  //void save_state_ptr(const void *owner, const pstring &stname, const datatype_t &dt, const std::size_t count, void *ptr);
        {
            //throw new emu_unimplemented();
#if false
#endif
        }
    }


    //template<> void state_manager_t::save_item(const void *owner, callback_t &state, const pstring &stname);
}
