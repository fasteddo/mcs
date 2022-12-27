// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using static mame.cpp_global;


namespace mame
{
    public class lua_engine : IDisposable
    {
        //struct menu_item

        //struct hook

        //struct lua_machine

        //struct lua_addr_space

        //struct lua_screen

        //struct lua_video

        //struct lua_cheat_entry

        //struct lua_options_entry

        //struct lua_memory_region

        //struct lua_emu_file

        //struct lua_item


        //static const char *const tname_ioport;

        // internal state
        //lua_State          *m_lua_state;
        running_machine m_machine;
        //std::unique_ptr<input_sequence_poller> m_seq_poll;

        //std::vector<std::string> m_menu;

        //hook hook_output_cb;
        //bool output_notifier_set;

        //hook hook_frame_cb;

        //static lua_engine*  luaThis;

        //std::map<lua_State *, std::pair<lua_State *, int> > thread_registry;


        // construction/destruction
        //-------------------------------------------------
        //  lua_engine - constructor
        //-------------------------------------------------
        public lua_engine()
        {
            //throw new emu_unimplemented();
#if false
#endif
        }

        ~lua_engine()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            close();
            m_isDisposed = true;
        }


        public void initialize()
        {
            //throw new emu_unimplemented();
        }


        public void load_script(string filename)
        {
            //throw new emu_unimplemented();
        }


        public void load_string(string value)
        {
            //throw new emu_unimplemented();
        }


        public bool frame_hook()
        {
            //throw new emu_unimplemented();
            return false;
        }


        //void menu_populate(std::string &menu, std::vector<menu_item> &menu_list);
        //bool menu_callback(std::string &menu, int index, std::string event);


        public void set_machine(running_machine machine)
        {
            //throw new emu_unimplemented();
#if false
            if (machine == null || (machine != m_machine))
                m_seq_poll.reset();
#endif

            m_machine = machine;
        }

        //std::vector<std::string> &get_menu() { return m_menu; }

        public void attach_notifiers()
        {
            //throw new emu_unimplemented();
        }


        public void on_frame_done()
        {
            //throw new emu_unimplemented();
        }


        public void on_sound_update()
        {
            //throw new emu_unimplemented();
        }


        public void on_periodic()
        {
            //throw new emu_unimplemented();
        }


        public bool on_missing_mandatory_image(string instance_name)
        {
            //throw new emu_unimplemented();
            return false;
        }


        //void on_machine_before_load_settings();



        //template<typename T, typename U>
        public bool call_plugin<T, U>(string name, T in_, out U out_)
        {
            throw new emu_unimplemented();
#if false
            bool ret = false;
            sol::object outobj = call_plugin(name, sol::make_object(sol(), in));
            if(outobj.is<U>())
            {
                out = outobj.as<U>();
                ret = true;
            }
            return ret;
#endif
        }

        //template<typename T, typename U>
        public bool call_plugin<T, U>(string name, T in_, out List<U> out_)
        {
            throw new emu_unimplemented();
#if false
            bool ret = false;
            sol::object outobj = call_plugin(name, sol::make_object(sol(), in));
            if(outobj.is<sol::table>())
            {
                for(auto &entry : outobj.as<sol::table>())
                {
                    if(entry.second.template is<U>())
                    {
                        out.push_back(entry.second.template as<U>());
                        ret = true;
                    }
                }
            }
            return ret;
#endif
        }


#if false
        // this can also check if a returned table contains type T
        template<typename T, typename U>
        bool call_plugin_check(const std::string &name, const U in, bool table = false)
        {
            bool ret = false;
            sol::object outobj = call_plugin(name, sol::make_object(sol(), in));
            if(outobj.is<T>() && !table)
                ret = true;
            else if(outobj.is<sol::table>() && table)
            {
                // check just one entry, checking the whole thing shouldn't be necessary as this only supports homogeneous tables
                if(outobj.as<sol::table>().begin().operator*().second.template is<T>())
                    ret = true;
            }
            return ret;
        }

        template<typename T>
        void call_plugin_set(const std::string &name, const T in)
        {
            call_plugin(name, sol::make_object(sol(), in));
        }
#endif

        //sol::state_view &sol() const { return *m_sol_state; }


        void close()
        {
            //throw new emu_unimplemented();
        }


        //void run(sol::load_result res);
    }
}
