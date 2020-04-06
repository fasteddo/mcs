// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public class lua_engine : global_object, IDisposable
    {
        struct menu_item
        {
#if false
            std::string text;
            std::string subtext;
            std::string flags;
#endif
        }


        struct hook
        {
#if false
            lua_State *L;
            int cb;

            hook();
            void set(lua_State *L, int idx);
            lua_State *precall();
            void call(lua_engine *engine, lua_State *T, int nparam);
            bool active() const { return L != nullptr; }
#endif
        }


        struct lua_machine
        {
#if false
            int l_popmessage(lua_State *L);
            int l_logerror(lua_State *L);
#endif
        }


        struct lua_addr_space
        {
#if false
            template<typename T> int l_mem_read(lua_State *L);
            template<typename T> int l_mem_write(lua_State *L);
            template<typename T> int l_direct_mem_read(lua_State *L);
            template<typename T> int l_direct_mem_write(lua_State *L);
#endif
        }


        struct lua_screen
        {
#if false
            int l_height(lua_State *L);
            int l_width(lua_State *L);
            int l_orientation(lua_State *L);
            int l_refresh(lua_State *L);
            int l_type(lua_State *L);
            int l_snapshot(lua_State *L);
            int l_draw_box(lua_State *L);
            int l_draw_line(lua_State *L);
            int l_draw_text(lua_State *L);
#endif
        }


        struct lua_video
        {
#if false
            int l_begin_recording(lua_State *L);
            int l_end_recording(lua_State *L);
#endif
        }


        struct lua_cheat_entry
        {
#if false
            int l_get_state(lua_State *L);
#endif
        }


        struct lua_options_entry
        {
#if false
            int l_entry_value(lua_State *L);
#endif
        }


        struct lua_memory_region
        {
#if false
            template<typename T> int l_region_read(lua_State *L);
            template<typename T> int l_region_write(lua_State *L);
#endif
        }


        struct lua_emu_file
        {
#if false
            lua_emu_file(const char *searchpath, UINT32 openflags) :
                path(searchpath),
                file(path.c_str(), openflags) {}

            int l_emu_file_read(lua_State *L);
            osd_file::error open(const char *name) {return file.open(name);}
            osd_file::error open_next() {return file.open_next();}
            int seek(INT64 offset, int whence) {return file.seek(offset, whence);}
            UINT64 size() {return file.size();}
            const char *filename() {return file.filename();}
            const char *fullpath() {return file.fullpath();}

            std::string path;
            emu_file file;
#endif
        }


        struct lua_item
        {
#if false
            lua_item(int index);
            void *l_item_base;
            unsigned int l_item_size;
            unsigned int l_item_count;
            int l_item_read(lua_State *L);
            int l_item_read_block(lua_State *L);
            int l_item_write(lua_State *L);
#endif
        }


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
            m_machine = nullptr;
            luaThis = this;
            m_lua_state = luaL_newstate();  /* create state */
            output_notifier_set = false;

            luaL_checkversion(m_lua_state);
            lua_gc(m_lua_state, LUA_GCSTOP, 0);  /* stop collector during initialization */
            luaL_openlibs(m_lua_state);  /* open libraries */

                // Get package.preload so we can store builtins in it.
            lua_getglobal(m_lua_state, "package");
            lua_getfield(m_lua_state, -1, "preload");
            lua_remove(m_lua_state, -2); // Remove package

            lua_pushcfunction(m_lua_state, luaopen_zlib);
            lua_setfield(m_lua_state, -2, "zlib");

            lua_pushcfunction(m_lua_state, luaopen_lsqlite3);
            lua_setfield(m_lua_state, -2, "lsqlite3");

            lua_pushcfunction(m_lua_state, luaopen_lfs);
            lua_setfield(m_lua_state, -2, "lfs");

            luaopen_ioport(m_lua_state);

            lua_gc(m_lua_state, LUA_GCRESTART, 0);
            msg.ready = 0;
            msg.status = 0;
            msg.done = 0;
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
