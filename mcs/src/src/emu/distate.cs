// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    // ======================> device_state_entry
    // class describing a single item of exposed device state
    public class device_state_entry : global_object
    {
        // device state flags
        const u8 DSF_NOSHOW          = 0x01; // don't display this entry in the registers view
        const u8 DSF_IMPORT          = 0x02; // call the import function after writing new data
        const u8 DSF_IMPORT_SEXT     = 0x04; // sign-extend the data when writing new data
        const u8 DSF_EXPORT          = 0x08; // call the export function prior to fetching the data
        const u8 DSF_CUSTOM_STRING   = 0x10; // set if the format has a custom string
        const u8 DSF_DIVIDER         = 0x20; // set if this is a divider entry
        const u8 DSF_READONLY        = 0x40; // set if this entry does not permit writes
        public const u8 DSF_FLOATING_POINT  = 0x80; // set if this entry represents a floating-point value

        // statics
        //static const UINT64 k_decimal_divisor[20];      // divisors for outputting decimal values


        // public state description
        device_state_interface m_device_state;         // link to parent device state
        int m_index;                // index by which this item is referred
        u64 m_datamask;             // mask that applies to the data
        u8 m_datasize;             // size of the data
        u8 m_flags;                // flags for this data
        string m_symbol;               // symbol for display; all lower-case version for expressions
        string m_format;               // supported formats
        bool m_default_format;       // true if we are still using default format


        // construction/destruction
        //-------------------------------------------------
        //  device_state_entry - constructor
        //-------------------------------------------------
        public device_state_entry(int index, string symbol, u8 size, u64 sizemask, u8 flags, device_state_interface dev)
        {
            m_device_state = dev;
            m_index = index;
            m_datamask = sizemask;
            m_datasize = size;
            m_flags = flags;
            m_symbol = symbol;
            m_default_format = true;


            assert(size == 1 || size == 2 || size == 4 || size == 8 || (flags & DSF_FLOATING_POINT) != 0);

            format_from_mask();

            // override well-known symbols
            if (index == device_state_interface.STATE_GENPCBASE)
                m_symbol = "CURPC";
            else if (index == device_state_interface.STATE_GENSP)
                m_symbol = "CURSP";
            else if (index == device_state_interface.STATE_GENFLAGS)
                m_symbol = "CURFLAGS";
        }


        public device_state_entry(int index, device_state_interface dev)
        {
            m_device_state = dev;
            m_index = index;
            m_datamask = 0;
            m_datasize = 0;
            m_flags = DSF_DIVIDER | DSF_READONLY;
            m_symbol = "";
            m_default_format = true;
        }

        //~device_state_entry() { }


        // post-construction modifiers
        public device_state_entry mask(u64 _mask) { m_datamask = _mask; format_from_mask(); return this; }
        //device_state_entry &signed_mask(UINT64 _mask) { m_datamask = _mask; m_flags |= DSF_IMPORT_SEXT; format_from_mask(); return *this; }

        //-------------------------------------------------
        //  formatstr - specify a format string
        //-------------------------------------------------
        public device_state_entry formatstr(string _format)
        {
            m_format = _format;
            m_default_format = false;

            // set the DSF_CUSTOM_STRING flag by formatting with a nullptr string
            m_flags &= (~DSF_CUSTOM_STRING & 0xff);
            format(null);

            return this;
        }

        public device_state_entry callimport() { m_flags |= DSF_IMPORT; return this; }
        public device_state_entry callexport() { m_flags |= DSF_EXPORT; return this; }
        public device_state_entry noshow() { m_flags |= DSF_NOSHOW; return this; }
        public device_state_entry readonly_() { m_flags |= DSF_READONLY; return this; }


        // query information
        public int index() { return m_index; }
        //void *dataptr() const { return entry_baseptr(); }
        //u64 datamask() { return m_datamask; }
        //u8 datasize() const { return m_datasize; }
        //const char *symbol() const { return m_symbol; }
        //bool visible() const { return ((m_flags & DSF_NOSHOW) == 0); }
        //bool writeable() const { return ((m_flags & DSF_READONLY) == 0); }
        public bool divider() { return (m_flags & DSF_DIVIDER) != 0; }
        bool is_float() { return (m_flags & DSF_FLOATING_POINT) != 0; }
        //device_state_interface *parent_state() const {return m_device_state;}
        //const std::string &format_string() const { return m_format; }


        // helpers

        public u64 value() { return entry_value() & m_datamask; }
        //double dvalue() const { return entry_dvalue(); }


        // return the current value as a string
        //std::string to_string() const;
        //int max_length() const;


        //-------------------------------------------------
        //  set_value - set the value from a u64
        //-------------------------------------------------
        void set_value(u64 value)
        {
            assert((m_flags & DSF_READONLY) == 0);

            // apply the mask
            value &= m_datamask;

            // sign-extend if necessary
            if ((m_flags & DSF_IMPORT_SEXT) != 0 && value > (m_datamask >> 1))
                value |= ~m_datamask;

            // store the value
            entry_set_value(value);

            // call the importer to finish up
            if ((m_flags & DSF_IMPORT) != 0)
                m_device_state.state_import(this);
        }


        //void set_dvalue(double value) const;


        // overrides

        //-------------------------------------------------
        //  entry_baseptr - return a pointer to where the
        //  data lives (if applicable)
        //-------------------------------------------------
        protected virtual object entry_baseptr()
        {
            return null;
        }


        //-------------------------------------------------
        //  entry_value - return the current value as a u64
        //-------------------------------------------------
        protected virtual u64 entry_value()
        {
            return 0;
        }


        //-------------------------------------------------
        //  entry_set_value - set the value from a u64
        //-------------------------------------------------
        protected virtual void entry_set_value(u64 value)
        {
        }


        //-------------------------------------------------
        //  entry_dvalue - return the current value as a
        //  double
        //-------------------------------------------------
        protected virtual double entry_dvalue()
        {
            return 0;
        }


        //-------------------------------------------------
        //  entry_set_dvalue - set the value from a double
        //-------------------------------------------------
        protected virtual void entry_set_dvalue(double value)
        {
            set_value((u64)value);
        }


        //-------------------------------------------------
        //  format_from_mask - make a format based on
        //  the data mask
        //-------------------------------------------------
        void format_from_mask()
        {
            // skip if we have a user-provided format
            if (!m_default_format)
                return;

            if (is_float())
            {
                m_format = "%12s";
                return;
            }

            // make up a format based on the mask
            if (m_datamask == 0)
                throw new emu_fatalerror("{0} state entry requires a nonzero mask\n", m_symbol.c_str());

            int width = 0;
            for (u64 tempmask = m_datamask; tempmask != 0; tempmask >>= 4)
                width++;

            m_format = string.Format("%%0{0}", width);  // %%0%dX
        }


        string format(string str, bool maxout = false)
        {
            //throw new emu_unimplemented();
            return "";
        }
    }


    public interface device_state_register_operators<T>
    {
        u8 sizeof_();
        u64 max();
        u8 flags();
    }

    class device_state_register_operators_u8 : device_state_register_operators<u8>
    {
        public u8 sizeof_() { return sizeof(u8); }
        public u64 max() { return u8.MaxValue; }
        public u8 flags() { return 0; }
    }

    class device_state_register_operators_u16 : device_state_register_operators<u16>
    {
        public u8 sizeof_() { return sizeof(u16); }
        public u64 max() { return u16.MaxValue; }
        public u8 flags() { return 0; }
    }

    class device_state_register_operators_u32 : device_state_register_operators<u32>
    {
        public u8 sizeof_() { return sizeof(u32); }
        public u64 max() { return u32.MaxValue; }
        public u8 flags() { return 0; }
    }

    class device_state_register_operators_bool : device_state_register_operators<bool>
    {
        public u8 sizeof_() { return 1; }
        public u64 max() { return 1; }
        public u8 flags() { return 0; }
    }

    class device_state_register_operators_double : device_state_register_operators<double>
    {
        public u8 sizeof_() { return sizeof(double); }
        public u64 max() { return u64.MaxValue; }
        public u8 flags() { return device_state_entry.DSF_FLOATING_POINT; }
    }


    // ======================> device_state_register
    // class template representing a state register of a specific width
    //template<class ItemType>
    class device_state_register<ItemType, ItemType_OPS> : device_state_entry
        where ItemType_OPS : device_state_register_operators<ItemType>, new()
    {
        static device_state_register_operators<ItemType> ops = new ItemType_OPS();


        ItemType m_data;                 // reference to where the data lives  //ItemType &              m_data;                 // reference to where the data lives


        // construction/destruction
        public device_state_register(int index, string symbol, ItemType data, device_state_interface dev)
            : base(index, symbol, ops.sizeof_(), ops.max(), ops.flags(), dev)  //: device_state_entry(index, symbol, sizeof(ItemType), std::numeric_limits<typename std::make_unsigned<ItemType>::type>::max(), 0, dev),
        {
            m_data = data;


            //throw new emu_unimplemented();
#if false
            static_assert(std::is_integral<ItemType>().value, "Registration of non-integer types is not currently supported");
#endif
        }


        // device_state_entry overrides
        protected override object entry_baseptr() { throw new emu_unimplemented(); }  //virtual void *entry_baseptr() const override { return &m_data; }
        protected override u64 entry_value() { throw new emu_unimplemented(); }  //virtual u64 entry_value() const override { return m_data; }
        protected override void entry_set_value(u64 value) { throw new emu_unimplemented(); }  //virtual void entry_set_value(u64 value) const override { m_data = value; }
    }


#if false
    // class template representing a boolean state register
    template<>
    class device_state_register<bool> : public device_state_entry
    {
    public:
        // construction/destruction
        device_state_register(int index, const char *symbol, bool &data, device_state_interface *dev)
            : device_state_entry(index, symbol, sizeof(bool), 1, 0, dev),
                m_data(data)
        {
        }

    protected:
        // device_state_entry overrides
        virtual void *entry_baseptr() const override { return &m_data; }
        virtual u64 entry_value() const override { return m_data; }
        virtual void entry_set_value(u64 value) const override { m_data = bool(value); }

    private:
        bool &                  m_data;                 // reference to where the data lives
    };

    // class template representing a floating-point state register
    template<>
    class device_state_register<double> : public device_state_entry
    {
    public:
        // construction/destruction
        device_state_register(int index, const char *symbol, double &data, device_state_interface *dev)
            : device_state_entry(index, symbol, sizeof(double), ~u64(0), DSF_FLOATING_POINT, dev),
                m_data(data)
        {
        }

    protected:
        // device_state_entry overrides
        virtual void *entry_baseptr() const override { return &m_data; }
        virtual u64 entry_value() const override { return u64(m_data); }
        virtual void entry_set_value(u64 value) const override { m_data = double(value); }
        virtual double entry_dvalue() const override { return m_data; }
        virtual void entry_set_dvalue(double value) const override { m_data = value; }

    private:
        double &                m_data;                 // reference to where the data lives
    };
#endif


    // ======================> device_latched_functional_state_register
    // class template representing a state register of a specific width
    //template<class ItemType>
    public class device_latched_functional_state_register<ItemType, ItemType_OPS> : device_state_entry
        where ItemType_OPS : device_state_register_operators<ItemType>, new()
    {
        static device_state_register_operators<ItemType> ops = new ItemType_OPS();


        //typedef typename std::function<void (ItemType)> setter_func;
        public delegate void setter_func(ItemType cpu);


        ItemType m_data;                 // reference to where the data lives  //ItemType &              m_data;                 // reference to where the data lives
        setter_func m_setter;               // function to store the data


        // construction/destruction
        public device_latched_functional_state_register(int index, string symbol, ItemType data, setter_func setter, device_state_interface dev)  //device_latched_functional_state_register(int index, const char *symbol, ItemType &data, setter_func &&setter, device_state_interface *dev)
            : base(index, symbol, ops.sizeof_(), ops.max(), ops.flags(), dev)  //: device_state_entry(index, symbol, sizeof(ItemType), std::numeric_limits<ItemType>::max(), 0, dev),
        {
            m_data = data;
            m_setter = setter;
        }


        // device_state_entry overrides
        protected override object entry_baseptr() { throw new emu_unimplemented(); }  //virtual void *entry_baseptr() const override { return &m_data; }
        protected override u64 entry_value() { throw new emu_unimplemented(); }  //virtual u64 entry_value() const override { return m_data; }
        protected override void entry_set_value(u64 value) { throw new emu_unimplemented(); }  //virtual void entry_set_value(u64 value) const override { m_setter(value); }
    }


    // ======================> device_functional_state_register

    // class template representing a state register of a specific width
    //template<class ItemType>
    public class device_functional_state_register<ItemType, ItemType_OPS> : device_state_entry
        where ItemType_OPS : device_state_register_operators<ItemType>, new()
    {
        static device_state_register_operators<ItemType> ops = new ItemType_OPS();


        //typedef typename std::function<ItemType ()> getter_func;
        public delegate ItemType getter_func();

        //typedef typename std::function<void (ItemType)> setter_func;
        public delegate void setter_func(ItemType cpu);


        getter_func m_getter;               // function to retrieve the data
        setter_func m_setter;               // function to store the data


        // construction/destruction
        public device_functional_state_register(int index, string symbol, getter_func getter, setter_func setter, device_state_interface dev)  //device_functional_state_register(int index, const char *symbol, getter_func &&getter, setter_func &&setter, device_state_interface *dev)
            : base(index, symbol, ops.sizeof_(), ops.max(), ops.flags(), dev)  //: device_state_entry(index, symbol, sizeof(ItemType), std::numeric_limits<ItemType>::max(), 0, dev),
        {
            m_getter = getter;
            m_setter = setter;
        }


        // device_state_entry overrides
        protected override u64 entry_value() { throw new emu_unimplemented(); }  //{ return ItemType is double ? u64(m_getter()) : m_getter(); }  //virtual u64 entry_value() const override { return u64(m_getter()); }
        protected override void entry_set_value(u64 value) { throw new emu_unimplemented(); }  //{ ItemType is double ? m_setter(double(value)) : m_setter(value); }  //virtual void entry_set_value(u64 value) const override { m_setter(double(value)); }
        protected override double entry_dvalue() { throw new emu_unimplemented(); }  //{ return ItemType is double ? m_getter() : base.entry_dvalue(); }  //virtual double entry_dvalue() const override { return m_getter(); }
        protected override void entry_set_dvalue(double value) { throw new emu_unimplemented(); }  //{ ItemType is double ? m_setter(value) : base.entry_set_dvalue(value); }  //virtual void entry_set_dvalue(double value) const override { m_setter(value); }
    }


#if false
    template<>
    class device_functional_state_register<double> : public device_state_entry
    {
    public:
        typedef typename std::function<double ()> getter_func;
        typedef typename std::function<void (double)> setter_func;

        // construction/destruction
        device_functional_state_register(int index, const char *symbol, getter_func &&getter, setter_func &&setter, device_state_interface *dev)
            : device_state_entry(index, symbol, sizeof(double), ~u64(0), DSF_FLOATING_POINT, dev),
                m_getter(std::move(getter)),
                m_setter(std::move(setter))
        {
        }

    protected:
        // device_state_entry overrides
        virtual u64 entry_value() const override { return u64(m_getter()); }
        virtual void entry_set_value(u64 value) const override { m_setter(double(value)); }
        virtual double entry_dvalue() const override { return m_getter(); }
        virtual void entry_set_dvalue(double value) const override { m_setter(value); }

    private:
        getter_func             m_getter;               // function to retrieve the data
        setter_func             m_setter;               // function to store the data
    };
#endif


    // ======================> device_state_interface
    // class representing interface-specific live state
    public class device_state_interface : device_interface
    {
        // standard state indexes
        //enum
        //{
        public const int STATE_GENPC     = -1;           // generic program counter (live)
        public const int STATE_GENPCBASE = -2;           // generic program counter (base of current instruction)
        public const int STATE_GENSP     = -3;           // generic stack pointer
        public const int STATE_GENFLAGS  = -4;           // generic flags
        //}


        // constants
        const int FAST_STATE_MIN = -4;                           // range for fast state
        const int FAST_STATE_MAX = 256;                          // lookups


        // state
        std.vector<device_state_entry> m_state_list = new std.vector<device_state_entry>();  //std::vector<std::unique_ptr<device_state_entry>>       m_state_list;           // head of state list
        device_state_entry [] m_fast_state = new device_state_entry[FAST_STATE_MAX + 1 - FAST_STATE_MIN];  // fast access to common entries


        // construction/destruction
        //-------------------------------------------------
        //  device_state_interface - constructor
        //-------------------------------------------------
        public device_state_interface(machine_config mconfig, device_t device)
            : base(device, "state")
        {
            //memset(m_fast_state, 0, sizeof(m_fast_state));

            // configure the fast accessor
            device.interfaces().m_state = this;
        }


        // configuration access
        //const simple_list<device_state_entry> &state_entries() const { return m_state_list; }


        // state getters

        u64 state_int(int index) { device_state_entry entry = state_find_entry(index); return (entry == null) ? 0 : entry.value(); }


        //astring &state_string(int index, astring &dest);
        public offs_t pc() { return (offs_t)state_int(STATE_GENPC); }
        //offs_t pcbase() { return state_int(STATE_GENPCBASE); }
        //offs_t sp() { return state_int(STATE_GENSP); }
        //UINT64 flags() { return state_int(STATE_GENFLAGS); }


        // state setters
        //void set_state_int(int index, u64 value) { const device_state_entry *entry = state_find_entry(index); if (entry != nullptr) entry->set_value(value); }
        //void set_pc(offs_t pc) { set_state_int(STATE_GENPC, pc); }


        // find the entry for a given index
        //-------------------------------------------------
        //  state_find_entry - return a pointer to the
        //  state entry for the given index
        //-------------------------------------------------
        device_state_entry state_find_entry(int index)
        {
            // use fast lookup if possible
            if (index >= FAST_STATE_MIN && index <= FAST_STATE_MAX)
                return m_fast_state[index - FAST_STATE_MIN];

            // otherwise, scan the first
            foreach (var entry in m_state_list)
            {
                if (entry.index() == index)
                    return entry;
            }

            // handle failure by returning nullptr
            return null;
        }


        // deliberately ambiguous functions; if you have the state interface
        // just use it directly
        //device_state_interface &state() { return *this; }


        // add a new state register item
        //template<class ItemType>
        public device_state_entry state_add<ItemType, ItemType_OPS>(int index, string symbol, ItemType data)  //template<class ItemType> device_state_entry &state_add(int index, const char *symbol, ItemType &data)
            where ItemType_OPS : device_state_register_operators<ItemType>, new()
        {
            assert(symbol != null);
            return state_add(new device_state_register<ItemType, ItemType_OPS>(index, symbol, data, this));  //return state_add(std::make_unique<device_state_register<ItemType>>(index, symbol, data, this));
        }

        public device_state_entry state_add(int index, string symbol, u8 data) { return state_add<u8, device_state_register_operators_u8>(index, symbol, data); }
        public device_state_entry state_add(int index, string symbol, u16 data) { return state_add<u16, device_state_register_operators_u16>(index, symbol, data); }
        public device_state_entry state_add(int index, string symbol, u32 data) { return state_add<u32, device_state_register_operators_u32>(index, symbol, data); }


        // add a new state register item using functional setter
        //template<class ItemType>
        public device_state_entry state_add<ItemType, ItemType_OPS>(int index, string symbol, ItemType data, device_latched_functional_state_register<ItemType, ItemType_OPS>.setter_func setter)  //template<class ItemType> device_state_entry &state_add(int index, const char *symbol, ItemType &data, typename device_latched_functional_state_register<ItemType>::setter_func &&setter)
            where ItemType_OPS : device_state_register_operators<ItemType>, new()
        {
            assert(symbol != null);
            return state_add(new device_latched_functional_state_register<ItemType, ItemType_OPS>(index, symbol, data, setter, this));  //return state_add(std::make_unique<device_latched_functional_state_register<ItemType>>(index, symbol, data, std::move(setter), this));
        }


        // add a new state register item using functional getter and setter (template argument must be explicit)
        //template<class ItemType>
        public device_state_entry state_add<ItemType, ItemType_OPS>(int index, string symbol, device_functional_state_register<ItemType, ItemType_OPS>.getter_func getter, device_functional_state_register<ItemType, ItemType_OPS>.setter_func setter)  //template<class ItemType> device_state_entry &state_add(int index, const char *symbol, typename device_functional_state_register<ItemType>::getter_func &&getter, typename device_functional_state_register<ItemType>::setter_func &&setter)
            where ItemType_OPS : device_state_register_operators<ItemType>, new()
        {
            assert(symbol != null);
            return state_add(new device_functional_state_register<ItemType, ItemType_OPS>(index, symbol, getter, setter, this));  //return state_add(std::make_unique<device_functional_state_register<ItemType>>(index, symbol, std::move(getter), std::move(setter), this));
        }


        // add a new read-only state register item using functional getter (template argument must be explicit)
        //template<class ItemType>
        public device_state_entry state_add<ItemType, ItemType_OPS>(int index, string symbol, device_functional_state_register<ItemType, ItemType_OPS>.getter_func getter)  //template<class ItemType> device_state_entry &state_add(int index, const char *symbol, typename device_functional_state_register<ItemType>::getter_func &&getter)
            where ItemType_OPS : device_state_register_operators<ItemType>, new()
        {
            assert(symbol != null);
            return state_add(new device_functional_state_register<ItemType, ItemType_OPS>(index, symbol, getter, (i) => { }, this)).readonly_();  //return state_add(std::make_unique<device_functional_state_register<ItemType>>(index, symbol, std::move(getter), [](ItemType){}, this)).readonly();
        }


        public device_state_entry state_add(device_state_entry entry)  //device_state_entry &state_add(std::unique_ptr<device_state_entry> &&entry);
        {
            // append to the end of the list
            m_state_list.push_back(entry);  //m_state_list.push_back(std::move(entry));
            device_state_entry new_entry = m_state_list.back();

            // set the fast entry if applicable
            if (new_entry.index() >= FAST_STATE_MIN && new_entry.index() <= FAST_STATE_MAX && !new_entry.divider())
                m_fast_state[new_entry.index() - FAST_STATE_MIN] = new_entry;

            return new_entry;
        }


        // add a new divider entry
        device_state_entry state_add_divider(int index) { return state_add(new device_state_entry(index, this)); }


        // derived class overrides

        //-------------------------------------------------
        //  state_import - called after new state is
        //  written to perform any post-processing
        //-------------------------------------------------
        public virtual void state_import(device_state_entry entry) { }

        //-------------------------------------------------
        //  state_export - called prior to new state
        //  reading the state
        //-------------------------------------------------
        protected virtual void state_export(device_state_entry entry) { }

        //-------------------------------------------------
        //  state_string_import - called after new state is
        //  written to perform any post-processing
        //-------------------------------------------------
        protected virtual void state_string_import(device_state_entry entry, string str) { }

        //-------------------------------------------------
        //  state_string_export - called after new state is
        //  written to perform any post-processing
        //-------------------------------------------------
        protected virtual void state_string_export(device_state_entry entry, out string str) { str = ""; }


        // internal operation overrides

        //-------------------------------------------------
        //  interface_post_start - verify that state was
        //  properly set up
        //-------------------------------------------------
        public override void interface_post_start()
        {
            // make sure we got something during startup
            if (m_state_list.size() == 0)
                throw new emu_fatalerror("No state registered for device '{0}' that supports it!\n", device().tag());
        }
    }
}
