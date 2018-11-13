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
    // standard state indexes
    enum STATE
    {
        STATE_GENPC = -1,               // generic program counter (live)
        STATE_GENPCBASE = -2,           // generic program counter (base of current instruction)
        STATE_GENSP = -3,               // generic stack pointer
        STATE_GENFLAGS = -4             // generic flags
    }


    // ======================> device_state_entry
    // class describing a single item of exposed device state
    public class device_state_entry
    {
        // device state flags
        const byte DSF_NOSHOW          = 0x01; // don't display this entry in the registers view
        const byte DSF_IMPORT          = 0x02; // call the import function after writing new data
        const byte DSF_IMPORT_SEXT     = 0x04; // sign-extend the data when writing new data
        const byte DSF_EXPORT          = 0x08; // call the export function prior to fetching the data
        const byte DSF_CUSTOM_STRING   = 0x10; // set if the format has a custom string
        const byte DSF_DIVIDER         = 0x20; // set if this is a divider entry
        const byte DSF_READONLY        = 0x40; // set if this entry does not permit writes
        protected const byte DSF_FLOATING_POINT  = 0x80; // set if this entry represents a floating-point value

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
        u64 m_sizemask;             // mask derived from the data size


        // construction/destruction
        //-------------------------------------------------
        //  device_state_entry - constructor
        //-------------------------------------------------
        public device_state_entry(int index, string symbol, byte size, UInt64 sizemask, byte flags, device_state_interface dev)
        {
            m_device_state = dev;
            m_index = index;
            m_datamask = sizemask;
            m_datasize = size;
            m_flags = flags;
            m_symbol = symbol;
            m_default_format = true;
            m_sizemask = sizemask;


            emucore_global.assert(size == 1 || size == 2 || size == 4 || size == 8 || (flags & DSF_FLOATING_POINT) != 0);

            format_from_mask();

            // override well-known symbols
            if (index == (int)STATE.STATE_GENPCBASE)
                m_symbol = "CURPC";
            else if (index == (int)STATE.STATE_GENSP)
                m_symbol = "CURSP";
            else if (index == (int)STATE.STATE_GENFLAGS)
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
            m_sizemask = 0;
        }


        ~device_state_entry()
        {
        }


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
        //public device_state_entry readonly() { m_flags |= DSF_READONLY; return this; }


        // query information
        public int index() { return m_index; }
        //void *dataptr() const { return entry_baseptr(); }
        //UInt64 datamask() { return m_datamask; }
        //const char *symbol() const { return m_symbol; }
        //bool visible() const { return ((m_flags & DSF_NOSHOW) == 0); }
        //bool writeable() const { return ((m_flags & DSF_READONLY) == 0); }
        public bool divider() { return (m_flags & DSF_DIVIDER) != 0; }
        bool is_float() { return (m_flags & DSF_FLOATING_POINT) != 0; }
        //device_state_interface *parent_state() const {return m_device_state;}
        //const std::string &format_string() const { return m_format; }


        // helpers

        //bool needs_custom_string() const { return ((m_flags & DSF_CUSTOM_STRING) != 0); }

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
            int width = 0;
            for (UInt64 tempmask = m_datamask; tempmask != 0; tempmask >>= 4)
                width++;

            m_format = string.Format("%%0{0}", width);  // %%0%dX
        }


        // return the current value -- only for our friends who handle export
        public bool needs_export() { return (m_flags & DSF_EXPORT) != 0; }
        public UInt64 value() { return entry_value() & m_datamask; }
        //double dvalue() const { return entry_dvalue(); }


        string format(string str, bool maxout = false)
        {
            //throw new emu_unimplemented();
            return "";
        }


        // set the current value -- only for our friends who handle import
        //bool needs_import() const { return ((m_flags & DSF_IMPORT) != 0); }

        //-------------------------------------------------
        //  set_value - set the value from a u64
        //-------------------------------------------------
        void set_value(u64 value)
        {
            emucore_global.assert((m_flags & DSF_READONLY) == 0);

            // apply the mask
            value &= m_datamask;

            // sign-extend if necessary
            if ((m_flags & DSF_IMPORT_SEXT) != 0 && value > (m_datamask >> 1))
                value |= ~m_datamask;

            // store the value
            entry_set_value(value);
        }

        //void set_dvalue(double value) const;
        //void set_value(const char *string) const;


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
    }


    // ======================> device_state_register

#if false
    // class template representing a state register of a specific width
    //template<class ItemType>
    class device_state_register<ItemType> : device_state_entry
    {
        ItemType m_data;                 // reference to where the data lives


        // construction/destruction
        device_state_register(int index, string symbol, ItemType data, device_state_interface dev)
            : base(index, symbol, sizeof(ItemType), std::numeric_limits<ItemType>::max(), 0, dev)
        {
            m_data = data;


#if false
            static_assert(std::is_integral<ItemType>().value, "Registration of non-integer types is not currently supported");
#endif
        }


        // device_state_entry overrides
        protected override object entry_baseptr() { return &m_data; }
        protected override UInt64 entry_value() { return m_data; }
        protected override void entry_set_value(UInt64 value) { m_data = value; }
    }
#endif

    // class template representing a state register of a specific width
    //template<class ItemType>
    class device_state_register_int : device_state_entry
    {
        object m_data;                 // reference to where the data lives


        // construction/destruction
        public device_state_register_int(int index, string symbol, object data, device_state_interface dev)
            : base(index, symbol, sizeof(int), int.MaxValue, 0, dev)
        {
            m_data = data;


#if false
            static_assert(std::is_integral<ItemType>().value, "Registration of non-integer types is not currently supported");
#endif
        }


        // device_state_entry overrides
        protected override object entry_baseptr() { return m_data; }
        protected override u64 entry_value() { return (u64)m_data; }
        protected override void entry_set_value(u64 value) { m_data = (int)value; }
    }


    // class template representing a state register of a specific width
    //template<class ItemType>
    class device_state_register_uint : device_state_entry
    {
        object m_data;                 // reference to where the data lives


        // construction/destruction
        public device_state_register_uint(int index, string symbol, object data, device_state_interface dev)
            : base(index, symbol, sizeof(int), int.MaxValue, 0, dev)
        {
            m_data = data;


#if false
            static_assert(std::is_integral<ItemType>().value, "Registration of non-integer types is not currently supported");
#endif
        }


        // device_state_entry overrides
        protected override object entry_baseptr() { return m_data; }
        protected override u64 entry_value() { return Convert.ToUInt64(m_data); }
        protected override void entry_set_value(u64 value) { m_data = (int)value; }
    }


    // class template representing a state register of a specific width
    //template<class ItemType>
    class device_state_register_byte : device_state_entry
    {
        object m_data;                 // reference to where the data lives


        // construction/destruction
        public device_state_register_byte(int index, string symbol, object data, device_state_interface dev)
            : base(index, symbol, sizeof(int), int.MaxValue, 0, dev)
        {
            m_data = data;


#if false
            static_assert(std::is_integral<ItemType>().value, "Registration of non-integer types is not currently supported");
#endif
        }


        // device_state_entry overrides
        protected override object entry_baseptr() { return m_data; }
        protected override u64 entry_value() { return (u64)m_data; }
        protected override void entry_set_value(u64 value) { m_data = (byte)value; }
    }


    // class template representing a state register of a specific width
    //template<class ItemType>
    class device_state_register_ushort : device_state_entry
    {
        object m_data;                 // reference to where the data lives


        // construction/destruction
        public device_state_register_ushort(int index, string symbol, object data, device_state_interface dev)
            : base(index, symbol, sizeof(int), int.MaxValue, 0, dev)
        {
            m_data = data;


#if false
            static_assert(std::is_integral<ItemType>().value, "Registration of non-integer types is not currently supported");
#endif
        }


        // device_state_entry overrides
        protected override object entry_baseptr() { return m_data; }
        protected override u64 entry_value() { return (u64)(u16)m_data; }
        protected override void entry_set_value(u64 value) { m_data = (u16)value; }
    }


    // class template representing a floating-point state register
    //template<>
    class device_state_register_double : device_state_entry
    {
        object m_data;  //double &                m_data;                 // reference to where the data lives


        // construction/destruction
        public device_state_register_double(int index, string symbol, object data, device_state_interface dev)  // double * data
            : base(index, symbol, sizeof(double), UInt64.MaxValue, DSF_FLOATING_POINT, dev)
        {
            m_data = data;
        }


        // device_state_entry overrides
        protected override object entry_baseptr() { return m_data; }
        protected override u64 entry_value() { return (u64)m_data; }
        protected override void entry_set_value(u64 value) { m_data = (double)value; }
        protected override double entry_dvalue() { return (double)m_data; }
        protected override void entry_set_dvalue(double value) { m_data = value; }
    }


#if false
    // ======================> device_pseudo_state_register

    // class template representing a state register of a specific width
    template<class ItemType>
    class device_pseudo_state_register : public device_state_entry
    {
    public:
        typedef typename std::function<ItemType ()> getter_func;
        typedef typename std::function<void (ItemType)> setter_func;

        // construction/destruction
        device_pseudo_state_register(int index, const char *symbol, getter_func &&getter, setter_func &&setter, device_state_interface *dev)
            : device_state_entry(index, symbol, sizeof(ItemType), std::numeric_limits<ItemType>::max(), 0, dev),
                m_getter(std::move(getter)),
                m_setter(std::move(setter))
        {
        }

    protected:
        // device_state_entry overrides
        virtual u64 entry_value() const override { return m_getter(); }
        virtual void entry_set_value(u64 value) const override { m_setter(value); }

    private:
        getter_func             m_getter;               // function to retrieve the data
        setter_func             m_setter;               // function to store the data
    };
#endif


    // ======================> device_state_interface
    // class representing interface-specific live state
    public class device_state_interface : device_interface
    {
        // constants
        const int FAST_STATE_MIN = -4;                           // range for fast state
        const int FAST_STATE_MAX = 256;                          // lookups


        // state
        std_vector<device_state_entry> m_state_list = new std_vector<device_state_entry>();  //std::vector<std::unique_ptr<device_state_entry>>       m_state_list;           // head of state list
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

        //-------------------------------------------------
        //  state_int - return the value of the given piece
        //  of indexed state as a UINT64
        //-------------------------------------------------
        u64 state_int(int index)
        {
            // nullptr or out-of-range entry returns 0
            device_state_entry entry = state_find_entry(index);
            if (entry == null)
                return 0;

            // call the exporter before we do anything
            if (entry.needs_export())
                state_export(entry);

            // pick up the value
            return entry.value();
        }

        //astring &state_string(int index, astring &dest);
        //int state_string_max_length(int index);
        public offs_t pc() { return (offs_t)state_int((int)STATE.STATE_GENPC); }
        //offs_t pcbase() { return state_int(STATE_GENPCBASE); }
        //offs_t sp() { return state_int(STATE_GENSP); }
        //UINT64 flags() { return state_int(STATE_GENFLAGS); }


        // state setters
        //void set_state_int(int index, UINT64 value);
        //void set_state_string(int index, const char *string);
        //void set_pc(offs_t pc) { set_state_int(STATE_GENPC, pc); }


        // deliberately ambiguous functions; if you have the state interface
        // just use it directly
        //device_state_interface &state() { return *this; }


        // add a new state item
        //template<class ItemType> device_state_entry &state_add(int index, const char *symbol, ItemType &data)
        public device_state_entry state_add(int index, string symbol, object data)
        {
            //assert(symbol != nullptr);

            // TODO - we need to pass in a intref, doubleref, ushortref, etc.  need to change all variables.  this is because C# numeric types are not references and these classes want references to these variables.
            if (data is int)         return state_add(new device_state_register_int(index, symbol, data, this));
            if (data is uint)        return state_add(new device_state_register_uint(index, symbol, data, this));
            else if (data is byte)   return state_add(new device_state_register_byte(index, symbol, data, this));
            else if (data is ushort) return state_add(new device_state_register_ushort(index, symbol, data, this));
            else if (data is double) return state_add(new device_state_register_double(index, symbol, data, this));
            else throw new emu_unimplemented();
        }

#if false
        // add a new state pseudo-register item (template argument must be explicit)
        //template<class ItemType> device_state_entry &state_add(int index, const char *symbol,
        //                typename device_pseudo_state_register<ItemType>::getter_func &&getter,
        //                typename device_pseudo_state_register<ItemType>::setter_func &&setter)
        public device_state_entry state_add<ItemType>(int index, string symbol, 
            typename device_pseudo_state_register<ItemType>::getter_func &&getter,
            typename device_pseudo_state_register<ItemType>::setter_func &&setter)
        {
            //assert(symbol != nullptr);
            return state_add(new device_pseudo_state_register<ItemType>(index, symbol, std::move(getter), std::move(setter), this));
        }

        //template<class ItemType> device_state_entry &state_add(int index, const char *symbol,
        //                typename device_pseudo_state_register<ItemType>::getter_func &&getter)
        public device_state_entry state_add<ItemType>(int index, string symbol,
            typename device_pseudo_state_register<ItemType>::getter_func &&getter)
        {
            //assert(symbol != nullptr);
            return state_add(new device_pseudo_state_register<ItemType>(index, symbol, std::move(getter), [](ItemType){}, this)).readonly();
        }
#endif

        //-------------------------------------------------
        //  state_add - add a new piece of indexed state
        //-------------------------------------------------
        public device_state_entry state_add(device_state_entry entry)
        {
            // append to the end of the list
            m_state_list.push_back(entry);
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
        public virtual void state_export(device_state_entry entry) { }

        //-------------------------------------------------
        //  state_string_import - called after new state is
        //  written to perform any post-processing
        //-------------------------------------------------
        public virtual void state_string_import(device_state_entry entry, string str) { }

        //-------------------------------------------------
        //  state_string_export - called after new state is
        //  written to perform any post-processing
        //-------------------------------------------------
        public virtual void state_string_export(device_state_entry entry, out string str) { str = ""; }


        // internal operation overrides

        //-------------------------------------------------
        //  interface_post_start - verify that state was
        //  properly set up
        //-------------------------------------------------
        public override void interface_post_start()
        {
            // make sure we got something during startup
            if (m_state_list.size() == 0)
                throw new emu_fatalerror("No state registered for device '{0}' that supports it!", device().tag());
        }


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
    }
}
