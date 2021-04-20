// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using s32 = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;


namespace mame
{
    // callback delegate for presave/postload
    //typedef named_delegate<void ()> save_prepost_delegate;
    public delegate void save_prepost_delegate();


    public class save_manager
    {
        //friend class ram_state;
        //friend class rewinder;


        class state_entry
        {
            // state
            //void *          m_data;                 // pointer to the memory to save/restore
            //std::string     m_name;                 // full name
            //device_t *      m_device;               // associated device, nullptr if none
            //std::string     m_module;               // module name
            //std::string     m_tag;                  // tag name
            //int             m_index;                // index
            //u8              m_typesize;             // size of the raw data type
            //u32             m_typecount;            // number of items in each block
            //u32             m_blockcount;           // number of blocks of items
            //u32             m_stride;               // stride between blocks of items in units of item size


            // construction/destruction
            //-------------------------------------------------
            //  state_entry - constructor
            //-------------------------------------------------
            public state_entry(object data, string name, device_t device, string module, string tag, int index, u8 size, u32 valcount, u32 blockcount, u32 stride)
            {
                throw new emu_unimplemented();
            }


            // helpers
            //void flip_data();
        }


        // internal state
        running_machine m_machine;              // reference to our machine
        //rewinder m_rewind;               // rewinder
        bool m_reg_allowed;          // are registrations allowed?
        s32 m_illegal_regs;         // number of illegal registrations

        List<state_entry> m_entry_list = new List<state_entry>();  //std::vector<std::unique_ptr<state_entry>>    m_entry_list;       // list of registered entries
        //std::vector<std::unique_ptr<ram_state>>      m_ramstate_list;    // list of ram states
        //std::vector<std::unique_ptr<state_callback>> m_presave_list;     // list of pre-save functions
        //std::vector<std::unique_ptr<state_callback>> m_postload_list;    // list of post-load functions


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

        //const char *indexed_item(int index, void *&base, u32 &valsize, u32 &valcount, u32 &blockcount, u32 &stride) const;


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


        // callback dispatching
        //void dispatch_presave();
        //void dispatch_postload();


        // generic memory registration
        //void save_memory(device_t *device, const char *module, const char *tag, u32 index, const char *name, void *val, u32 valsize, u32 valcount = 1, u32 blockcount = 1, u32 stride = 0);


        // templatized wrapper for general objects and arrays
        //template <typename ItemType>
        public void save_item<ItemType>(device_t device, string module, string tag, int index, ItemType value, string valname)  //void save_item(device_t *device, const char *module, const char *tag, int index, ItemType &value, const char *valname)
        {
            //throw new emu_unimplemented();
#if false
            static_assert(!type_checker<ItemType>::is_pointer, "Called save_item on a pointer with no count!");
            static_assert(type_checker<typename array_unwrap<ItemType>::underlying_type>::is_atom, "Called save_item on a non-fundamental type!");
            save_memory(device, module, tag, index, valname, array_unwrap<ItemType>::ptr(value), array_unwrap<ItemType>::SIZE, array_unwrap<ItemType>::SAVE_COUNT);
#endif
        }

        public void save_item<ItemType>(device_t device, string module, string tag, int index, Tuple<ItemType, string> value)
        { save_item(device, module, tag, index, value.Item1, value.Item2); }


        // templatized wrapper for structure members
        //template <typename ItemType, typename StructType, typename ElementType>
        //void save_item(device_t *device, const char *module, const char *tag, int index, ItemType &value, ElementType StructType::*element, const char *valname)
        //{
        //    static_assert(std::is_base_of<StructType, typename array_unwrap<ItemType>::underlying_type>::value, "Called save_item on a non-matching struct member pointer!");
        //    static_assert(!(sizeof(typename array_unwrap<ItemType>::underlying_type) % sizeof(typename array_unwrap<ElementType>::underlying_type)), "Called save_item on an unaligned struct member!");
        //    static_assert(!type_checker<ElementType>::is_pointer, "Called save_item on a struct member pointer!");
        //    static_assert(type_checker<typename array_unwrap<ElementType>::underlying_type>::is_atom, "Called save_item on a non-fundamental type!");
        //    save_memory(device, module, tag, index, valname, array_unwrap<ElementType>::ptr(array_unwrap<ItemType>::ptr(value)->*element), array_unwrap<ElementType>::SIZE, array_unwrap<ElementType>::SAVE_COUNT, array_unwrap<ItemType>::SAVE_COUNT, sizeof(typename array_unwrap<ItemType>::underlying_type) / sizeof(typename array_unwrap<ElementType>::underlying_type));
        //}

        // templatized wrapper for pointers
        //template <typename ItemType>
        //void save_pointer(device_t *device, const char *module, const char *tag, int index, ItemType *value, const char *valname, u32 count)
        //{
        //    static_assert(type_checker<typename array_unwrap<ItemType>::underlying_type>::is_atom, "Called save_pointer on a non-fundamental type!");
        //    save_memory(device, module, tag, index, valname, array_unwrap<ItemType>::ptr(value[0]), array_unwrap<ItemType>::SIZE, array_unwrap<ItemType>::SAVE_COUNT * count);
        //}

        //template <typename ItemType, typename StructType, typename ElementType>
        //void save_pointer(device_t *device, const char *module, const char *tag, int index, ItemType *value, ElementType StructType::*element, const char *valname, u32 count)
        //{
        //    static_assert(std::is_base_of<StructType, typename array_unwrap<ItemType>::underlying_type>::value, "Called save_pointer on a non-matching struct member pointer!");
        //    static_assert(!(sizeof(typename array_unwrap<ItemType>::underlying_type) % sizeof(typename array_unwrap<ElementType>::underlying_type)), "Called save_pointer on an unaligned struct member!");
        //    static_assert(!type_checker<ElementType>::is_pointer, "Called save_pointer on a struct member pointer!");
        //    static_assert(type_checker<typename array_unwrap<ElementType>::underlying_type>::is_atom, "Called save_pointer on a non-fundamental type!");
        //    save_memory(device, module, tag, index, valname, array_unwrap<ElementType>::ptr(array_unwrap<ItemType>::ptr(value[0])->*element), array_unwrap<ElementType>::SIZE, array_unwrap<ElementType>::SAVE_COUNT, array_unwrap<ItemType>::SAVE_COUNT * count, sizeof(typename array_unwrap<ItemType>::underlying_type) / sizeof(typename array_unwrap<ElementType>::underlying_type));
        //}

        // templatized wrapper for std::unique_ptr
        //template <typename ItemType>
        //void save_pointer(device_t *device, const char *module, const char *tag, int index, const std::unique_ptr<ItemType []> &value, const char *valname, u32 count)
        //{
        //    static_assert(type_checker<typename array_unwrap<ItemType>::underlying_type>::is_atom, "Called save_pointer on a non-fundamental type!");
        //    save_memory(device, module, tag, index, valname, array_unwrap<ItemType>::ptr(value[0]), array_unwrap<ItemType>::SIZE, array_unwrap<ItemType>::SAVE_COUNT * count);
        //}

        //template <typename ItemType, typename StructType, typename ElementType>
        //void save_pointer(device_t *device, const char *module, const char *tag, int index, const std::unique_ptr<ItemType []> &value, ElementType StructType::*element, const char *valname, u32 count)
        //{
        //    static_assert(std::is_base_of<StructType, typename array_unwrap<ItemType>::underlying_type>::value, "Called save_pointer on a non-matching struct member pointer!");
        //    static_assert(!(sizeof(typename array_unwrap<ItemType>::underlying_type) % sizeof(typename array_unwrap<ElementType>::underlying_type)), "Called save_pointer on an unaligned struct member!");
        //    static_assert(!type_checker<ElementType>::is_pointer, "Called save_pointer on a struct member pointer!");
        //    static_assert(type_checker<typename array_unwrap<ElementType>::underlying_type>::is_atom, "Called save_pointer on a non-fundamental type!");
        //    save_memory(device, module, tag, index, valname, array_unwrap<ElementType>::ptr(array_unwrap<ItemType>::ptr(value[0])->*element), array_unwrap<ElementType>::SIZE, array_unwrap<ElementType>::SAVE_COUNT, array_unwrap<ItemType>::SAVE_COUNT * count, sizeof(typename array_unwrap<ItemType>::underlying_type) / sizeof(typename array_unwrap<ElementType>::underlying_type));
        //}


        // global memory registration
        //template<typename ItemType>
        public void save_item<ItemType>(ItemType value, string valname, int index = 0)
        { save_item(null, "global", null, index, value, valname); }

        // state saving interfaces
        //template<typename _ItemType>
        public void save_item<ItemType>(Tuple<ItemType, string> value, int index = 0)
        { save_item(null, "global", null, index, value.Item1, value.Item2); }

        //template <typename ItemType, typename StructType, typename ElementType>
        //void save_item(ItemType &value, ElementType StructType::*element, const char *valname, int index = 0)
        //{ save_item(nullptr, "global", nullptr, index, value, element, valname); }

        //template <typename ItemType>
        //void save_pointer(ItemType &&value, const char *valname, u32 count, int index = 0)
        //{ save_pointer(nullptr, "global", nullptr, index, std::forward<ItemType>(value), valname, count); }
        //template <typename ItemType, typename StructType, typename ElementType>
        //void save_pointer(ItemType &&value, ElementType StructType::*element, const char *valname, u32 count, int index = 0)
        //{ save_pointer(nullptr, "global", nullptr, index, std::forward<ItemType>(value), element, valname, count); }


        // file processing
        //static save_error check_file(running_machine &machine, emu_file &file, const char *gamename, void (CLIB_DECL *errormsg)(const char *fmt, ...));
        //save_error write_file(emu_file &file);
        //save_error read_file(emu_file &file);

        //save_error write_stream(std::ostream &str);
        //save_error read_stream(std::istream &str);

        //save_error write_buffer(void *buf, size_t size);
        //save_error read_buffer(const void *buf, size_t size);


        // internal helpers
        //template <typename T, typename U, typename V, typename W>
        //save_error do_write(T check_space, U write_block, V start_header, W start_data);
        //template <typename T, typename U, typename V, typename W>
        //save_error do_read(T check_length, U read_block, V start_header, W start_data);
        //u32 signature() const;
        //void dump_registry() const;
        //static save_error validate_header(const u8 *header, const char *gamename, u32 signature, void (CLIB_DECL *errormsg)(const char *fmt, ...), const char *error_prefix);
    }
}
