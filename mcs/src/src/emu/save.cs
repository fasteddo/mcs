// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    // callback delegate for presave/postload
    public delegate void save_prepost_delegate();


    class state_entry
    {
        // state
        //void *              m_data;                 // pointer to the memory to save/restore
        //astring             m_name;                 // full name
        //device_t *          m_device;               // associated device, NULL if none
        //std::string         m_module;               // module name
        //std::string         m_tag;                  // tag name
        //int                 m_index;                // index
        //UINT8               m_typesize;             // size of the raw data type
        //UINT32              m_typecount;            // number of items
        //UINT32              m_offset;               // offset within the final structure


        // construction/destruction
        //-------------------------------------------------
        //  state_entry - constructor
        //-------------------------------------------------
        public state_entry(object data, string name, device_t device, string module, string tag, int index, byte size, UInt32 count)
        {
            throw new emu_unimplemented();
        }


        // helpers
        //void flip_data();
    }


    public class save_manager
    {
        // internal state
        running_machine m_machine;              // reference to our machine
        //rewinder m_rewind;               // rewinder
        bool m_reg_allowed;          // are registrations allowed?
        int m_illegal_regs;         // number of illegal registrations

        List<state_entry> m_entry_list = new List<state_entry>();          // list of reigstered entries
        //std::vector<std::unique_ptr<ram_state>>      m_ramstate_list;    // list of ram states
        //simple_list<state_callback> m_presave_list;     // list of pre-save functions
        //simple_list<state_callback> m_postload_list;    // list of post-load functions


        // construction/destruction

        //-------------------------------------------------
        //  save_manager - constructor
        //-------------------------------------------------
        public save_manager(running_machine machine)
        {
            m_machine = machine;
            m_reg_allowed = true;
            m_illegal_regs = 0;
        }


        // getters
        //running_machine &machine() const { return m_machine; }
        //rewinder rewind() { return m_rewind.get(); }
        public int registration_count() { return m_entry_list.Count; }
        public bool registration_allowed() { return m_reg_allowed; }


        // registration control
        public void allow_registration(bool allowed = true)
        {
            //throw new emu_unimplemented();
        }

        //const char *indexed_item(int index, void *&base, UINT32 &valsize, UINT32 &valcount) const;


        // function registration
        //-------------------------------------------------
        //  register_presave - register a pre-save
        //  function callback
        //-------------------------------------------------
        public void register_presave(save_prepost_delegate func)
        {
            //throw new emu_unimplemented();
#if false
            // check for invalid timing
            if (!m_reg_allowed)
                global.fatalerror("Attempt to register callback function after state registration is closed!\n");

            // scan for duplicates and push through to the end
            foreach (state_callback cb in m_presave_list)
            {
                if (cb.m_func == func)
                    global.fatalerror("Duplicate save state function ({0}/{1})\n", cb.m_func.name(), func.name());
            }

            // allocate a new entry
            m_presave_list.append(new state_callback(func));
#endif
        }

        //-------------------------------------------------
        //  state_save_register_postload -
        //  register a post-load function callback
        //-------------------------------------------------
        public void register_postload(save_prepost_delegate func)
        {
            //throw new emu_unimplemented();
#if false
            // check for invalid timing
            if (!m_reg_allowed)
                fatalerror("Attempt to register callback function after state registration is closed!\n");

            // scan for duplicates and push through to the end
            for (state_callback *cb = m_postload_list.first(); cb != NULL; cb = cb->next())
                if (cb->m_func == func)
                    fatalerror("Duplicate save state function (%s/%s)\n", cb->m_func.name(), func.name());

            // allocate a new entry
            m_postload_list.append(*global_alloc(state_callback(func)));
#endif
        }


        // templatized wrapper for general objects
        //template<typename _ItemType>
        public void save_item<_ItemType>(device_t device, string module, string tag, int index, _ItemType value, string valname)
        {
            //throw new emu_unimplemented();
#if false
            if (type_checker<_ItemType>::is_pointer) throw emu_fatalerror("Called save_item on a pointer with no count!");
            if (!type_checker<_ItemType>::is_atom) throw emu_fatalerror("Called save_item on a non-fundamental type!");
            save_memory(module, tag, index, valname, &value, sizeof(value));
#endif
        }

        // templatized wrapper for 1-dimensional arrays
        //template<typename _ItemType, std::size_t N>
        void save_item<_ItemType, N>(device_t device, string module, string tag, int index, _ItemType [] value, string valname)
        {
            throw new emu_unimplemented();
        }

#if false
        // templatized wrapper for 2-dimensional arrays
        template<typename _ItemType, std::size_t M, std::size_t N>
        void save_item(const char *module, const char *tag, int index, _ItemType (&value)[M][N], const char *valname)
        {
            if (!type_checker<_ItemType>::is_atom) throw emu_fatalerror("Called save_item on a non-fundamental type!");
            save_memory(module, tag, index, valname, &value[0][0], sizeof(value[0][0]), M * N);
        }
#endif


        // global memory registration
        //template<typename _ItemType>
        public void save_item<_ItemType>(_ItemType value, string valname, int index = 0) { save_item(null, "global", null, index, value, valname); }

        //template<typename _ItemType>
        //void save_pointer(_ItemType *value, const char *valname, UINT32 count, int index = 0) { save_pointer(NULL, "global", NULL, index, value, valname, count); }


        // file processing
        //static save_error check_file(running_machine &machine, emu_file &file, const char *gamename, void (CLIB_DECL *errormsg)(const char *fmt, ...));
        //save_error write_file(emu_file &file);
        //save_error read_file(emu_file &file);


        // internal helpers
        //UINT32 signature() const;
        //void dump_registry() const;
        //static save_error validate_header(const UINT8 *header, const char *gamename, UINT32 signature, void (CLIB_DECL *errormsg)(const char *fmt, ...), const char *error_prefix);
    }
}
