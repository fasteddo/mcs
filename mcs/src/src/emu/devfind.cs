// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
using optional_address_space = mame.address_space_finder<mame.bool_constant_false>;  //using optional_address_space = address_space_finder<false>;
using optional_memory_bank = mame.memory_bank_finder<mame.bool_constant_false>;  //using optional_memory_bank = memory_bank_finder<false>;
using optional_memory_region = mame.memory_region_finder<mame.bool_constant_false>;  //using optional_memory_region = memory_region_finder<false>;
using PointerU8 = mame.Pointer<System.Byte>;
using required_memory_bank = mame.memory_bank_finder<mame.bool_constant_true>;  //using required_memory_bank = memory_bank_finder<true>;
using required_memory_region = mame.memory_region_finder<mame.bool_constant_true>;  //using required_memory_region = memory_region_finder<true>;
using size_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using unsigned = System.UInt32;


namespace mame
{
    /// \brief Helper class to find arrays of devices, etc.
    ///
    /// Useful when a machine/device has a number of similar subdevices, I/O
    /// ports, memory regions, etc.  Template arguments are the element type
    /// and number of elements in the array.  It's assumed that the element
    /// can be constructed with a device_t reference and a C string tag.
    //template <typename T, unsigned Count>
    public class object_array_finder<T, unsigned_Count>
        where T : finder_base
        where unsigned_Count : uint32_constant, new()
    {
        protected static readonly unsigned Count = new unsigned_Count().value;


        /// \brief Element type for Container concept
        //typedef T value_type;

        /// \brief Reference to element type for Container concept
        //typedef T &reference;

        /// \brief Reference to constant element type for Container concept
        //typedef T const &const_reference;

        /// \brief Iterator for Container concept
        //typedef T *iterator;

        /// \brief Constant iterator for Container concept
        //typedef T const *const_iterator;

        /// \brief Iterator difference type for Container concept
        //typedef typename std::iterator_traits<iterator>::difference_type difference_type;

        /// \brief Size type for Container concept
        //typedef std::make_unsigned_t<difference_type> size_type;


        /// \brief Generated tag names
        ///
        /// Finder objects do not copy the search tag supplied at
        /// construction.  Tags that are programmatically generated at
        /// construction are stored here so they persist until resolution
        /// time (and beyond).
        protected string [] m_tag = new string [Count];  //std::string const m_tag[Count];

        /// \brief The object discovery elements
        ///
        /// These are the actual object discovery helpers.  Note that this
        /// member must be initialised after m_tag, as it may depend on
        /// programmatically generated tags.
        protected T [] m_array = new T [Count];  //T m_array[Count];


        /// \brief Construct with programmatically generated tags
        ///
        /// Specify a format string and starting number.  A single unsigned
        /// int format argument is supplied containing the (zero-based)
        /// element index added to the starting number.  For example if
        /// Count = 2, ("p%u_joy", 1) expands to ("p1_joy", "p2_joy").  The
        /// relaxed format rules used by util::string_format apply.
        /// \param [in] base Base device to search from.
        /// \param [in] fmt Search tag format, should expect single unsigned
        ///   int argument.
        /// \param [in] start Number to add to element index when
        ///   calculating values for string format argument.
        /// \param [in] arg Optional additional constructor argument(s) passed
        ///   to all elements.
        /// \sa util::string_format
        //template <typename F, typename... Param, unsigned... V>
        //object_array_finder(device_t &base, F const &fmt, unsigned start, std::integer_sequence<unsigned, V...>, Param const &... arg)
        //    : m_tag{ util::string_format(fmt, start + V)... }
        //    , m_array{ { base, m_tag[V].c_str(), arg... }... }
        //{
        //}

        protected object_array_finder(device_t base_, string fmt, int start, Func<device_t, string, finder_base> creator_func = null)
        {
            //m_tag{ util::string_format(fmt, start + V)... }
            for (int i = 0; i < Count; i++)
                m_tag[i] = string.Format(fmt, start + i);

            //m_array{ { base, m_tag[V].c_str(), arg... }... }
            if (creator_func != null)
            {
                for (int i = 0; i < Count; i++)
                    m_array[i] = (T)creator_func(base_, m_tag[i]);
            }
        }


        //template <typename... Param, unsigned... V>
        //object_array_finder(device_t &base, std::array<char const *, Count> const &tags, std::integer_sequence<unsigned, V...>, Param const &... arg)
        //    : m_array{ { base, tags[V], arg... }... }
        //{
        //}

        /// \brief Construct with programmatically generated tags
        ///
        /// Specify a format string and starting number.  A single unsigned
        /// int format argument is supplied containing the (zero-based)
        /// element index added to the starting number.  For example if
        /// Count = 2, ("p%u_joy", 1) expands to ("p1_joy", "p2_joy").  The
        /// relaxed format rules used by util::string_format apply.
        /// \param [in] base Base device to search from.
        /// \param [in] fmt Search tag format, should expect single unsigned
        ///   int argument.
        /// \param [in] start Number to add to element index when
        ///   calculating values for string format argument.
        /// \param [in] arg Optional additional constructor argument(s) passed
        ///   to all elements.
        /// \sa util::string_format
        //template <typename F, typename... Param>
        //object_array_finder(device_t &base, F const &fmt, unsigned start, Param const &... arg)
        //    : object_array_finder(base, fmt, start, std::make_integer_sequence<unsigned, Count>(), arg...)
        //{
        //}

        /// \brief Construct with free-form list of tags
        ///
        /// Specify arbitrary tags for objects.  Useful when there is no
        /// particular pattern to the object tags.
        /// \param [in] base Base device to search from.
        /// \param [in] tags Tags to search for, e.g. { "player", "dips" }.
        ///   The tags are not copied, it is the caller's responsibility to
        ///   ensure the pointers remain valid until resolution time.
        /// \param [in] arg Optional additional constructor argument(s) passed
        ///   to all elements.
        //template <typename... Param>
        //object_array_finder(device_t &base, std::array<char const *, Count> const &tags, Param const &... arg)
        //    : object_array_finder(base, tags, std::make_integer_sequence<unsigned, Count>(), arg...)
        //{
        //}

        /// \brief Get iterator to first element
        ///
        /// Returns an iterator to the first element in the array.
        /// \return Iterator to first element.
        //const_iterator begin() const { return m_array; }
        //iterator begin() { return m_array; }

        /// \brief Get iterator beyond last element
        ///
        /// Returns an iterator one past the last element in the array.
        /// \return Iterator one past last element.
        //const_iterator end() const { return m_array + Count; }
        //iterator end() { return m_array + Count; }

        /// \brief Get constant iterator to first element
        ///
        /// Returns a constant iterator to the first element in the array.
        /// \return Constant iterator to first element.
        //const_iterator cbegin() const { return m_array; }

        /// \brief Get constant iterator beyond last element
        ///
        /// Returns a constant iterator one past the last element in the
        /// array.
        /// \return Constant iterator one past last element.
        //const_iterator cend() const { return m_array + Count; }

        /// \brief Get array size
        ///
        /// Returns number of elements in the array (compile-time constant).
        /// \return The size of the array.
        //constexpr size_type size() const { return Count; }

        /// \brief Get maximum array size
        ///
        /// Returns maximum number of elements in the array (compile-time
        /// constant, always equal to the size of the array).
        /// \return The size of the array.
        //constexpr size_type max_size() const { return Count; }

        /// \brief Does array have no elements
        ///
        /// Returns whether the array has no elements (compile-time
        /// constant).
        /// \return True if the array has no elements, false otherwise.
        //constexpr bool empty() const { return !Count; }

        /// \brief Get first element
        ///
        /// Returns a reference to the first element in the array.
        /// \return Reference to first element.
        //T const &front() const { return m_array[0]; }
        //T &front() { return m_array[0]; }

        /// \brief Get last element
        ///
        /// Returns a reference to the last element in the array.
        /// \return Reference to last element.
        //T const &back() const { return m_array[Count - 1]; }
        //T &back() { return m_array[Count - 1]; }

        /// \brief Element accessor (const)
        ///
        /// Returns a const reference to the element at the supplied index.
        /// \param [in] index Index of desired element (zero-based).
        /// \return Constant reference to element at specified index.
        //T const &operator[](unsigned index) const { assert(index < Count); return m_array[index]; }

        /// \brief Element accessor (non-const)
        ///
        /// Returns a reference to the element at the supplied index.
        /// \param [in] index Index of desired element (zero-based).
        /// \return Reference to element at specified index.
        //T &operator[](unsigned index) { assert(index < Count); return m_array[index]; }
        public virtual T op(int index) { global_object.assert(index < Count);  return at(index); }

        /// \brief Checked element accessor
        ///
        /// Returns a reference to the element at the supplied index if less
        /// than the size of the array, or throws std::out_of_range
        /// otherwise.
        /// \param [in] index Index of desired element (zero-based).
        /// \return Reference to element at specified index.
        /// \throw std::out_of_range
        //T const &at(unsigned index) const { if (Count > index) return m_array[index]; else throw std::out_of_range("Index out of range"); }
        //T &at(unsigned index) { if (Count > index) return m_array[index]; else throw std::out_of_range("Index out of range"); }
        public virtual T at(int index) { if (Count > index) return m_array[index]; else throw new std.out_of_range("Index out of range"); }
    }


    // ======================> finder_base
    /// \brief Base class for object discovery helpers
    ///
    /// Abstract non-template base class for object auto-discovery helpers.
    /// Provides the interface that the device_t uses to manage discovery at
    /// resolution time.
    public abstract class finder_base : global_object
    {
        /// \brief Dummy tag always treated as not found
        public const string DUMMY_TAG = "finder_dummy_tag";


        // internal state

        /// \brief Pointer to next registered discovery helper
        ///
        /// This is a polymorphic class, so it can't be held in a standard
        /// list container that requires elements of the same type.  Hence
        /// it implements basic intrusive single-linked list behaviour.
        finder_base m_next;

        /// \brief Base device to search from
        protected device_t m_base;

        /// \brief Object tag to search for
        protected string m_tag;

        /// \brief Set when object resolution completes
        protected bool m_resolved;


        // construction/destruction
        //-------------------------------------------------
        //  finder_base - constructor
        //-------------------------------------------------
        protected finder_base(device_t base_, string tag)
        {
            m_next = base_.register_auto_finder(this);
            m_base = base_;
            m_tag = tag;
            m_resolved = false;
        }


        // getters
        public finder_base next() { return m_next; }


        /// \brief Attempt discovery
        ///
        /// Concrete derived classes must implement this member function.
        /// Should return false if the object is required but not found, or
        /// true otherwise (the report_missing member function can assist
        /// in implementing this behaviour).
        /// \param [in] valid Pass a pointer to the validity checker if this
        ///   is a dry run (i.e. no intention to actually start the device),
        ///   or nullptr otherwise.
        /// \return False if the object is required but not found, or true
        ///   otherwise.
        public abstract bool findit(validity_checker valid);

        /// \brief Clear temporary binding from configuration
        ///
        /// Concrete derived classes must implement this member function.
        /// Object finders may allow temporary binding to the anticipated
        /// target during configuration.  This needs to be cleared to ensure
        /// the correct target is found if a device further up the hierarchy
        /// subsequently removes or replaces devices.
        public abstract void end_configuration();

        public string finder_tag() { return m_tag; }

        /// \brief Get search target
        ///
        /// Returns the search base device and tag.
        /// \return a pair consisting of a reference to the device to search
        ///   relative to and the relative tag.
        public std.pair<device_t, string> finder_target() { return std.make_pair(m_base, m_tag); }


        // setter for setting the object

        /// \brief Set search tag
        ///
        /// Allows search tag to be changed after construction.  Note that
        /// this must be done before resolution time to take effect.  Also
        /// note that the tag is not copied.
        /// \param [in] base Updated search base.  The tag must be specified
        ///   relative to this device.
        /// \param [in] tag Updated search tag.  This is not copied, it is
        ///   the caller's responsibility to ensure this pointer remains
        ///   valid until resolution time.
        public void set_tag(device_t base_, string tag) { assert(!m_resolved); m_base = base_; m_tag = tag; }

        /// \brief Set search tag
        ///
        /// Allows search tag to be changed after construction.  Note that
        /// this must be done before resolution time to take effect.  Also
        /// note that the tag is not copied.
        /// \param [in] tag Updated search tag relative to the current
        ///   device being configured.  This is not copied, it is the
        ///   caller's responsibility to ensure this pointer remains valid
        ///   until resolution time.
        public void set_tag(string tag) { m_base = m_base.get().mconfig().current_device(); m_tag = tag; }

        /// \brief Set search tag
        ///
        /// Allows search tag to be changed after construction.  Note that
        /// this must be done before resolution time to take effect.
        /// \param [in] finder Object finder to take the search base and tag
        ///   from.
        public void set_tag(finder_base finder) { assert(!m_resolved); var kvp = finder.finder_target(); m_base = kvp.first; m_tag = kvp.second; }  //std::tie(m_base, m_tag) = finder.finder_target(); }


        // helpers

        //-------------------------------------------------
        //  find_memregion - find memory region
        //-------------------------------------------------
        protected MemoryU8 find_memregion(u8 width, out size_t length, bool required)  //void *finder_base::find_memregion(u8 width, size_t &length, bool required) const
        {
            // look up the region and return nullptr if not found
            memory_region region = m_base.get().memregion(m_tag);
            if (region == null)
            {
                length = 0;
                return null;
            }

            // check the width and warn if not correct
            if (region.bytewidth() != width)
            {
                if (required)
                    osd_printf_warning("Region '{0}' found but is width {1}, not {2} as requested\n", m_tag, region.bitwidth(), width * 8);
                length = 0;
                return null;
            }

            // return results
            length = region.bytes() / width;
            return region.base_();
        }


        protected bool validate_memregion(bool required)
        {
            // make sure we can resolve the full path to the region
            size_t bytes_found = 0;
            string region_fulltag = m_base.get().subtag(m_tag);

            // look for the region
            foreach (device_t dev in new device_enumerator(m_base.get().mconfig().root_device()))
            {
                foreach (tiny_rom_entry region in new romload.entries(dev.rom_region()).get_regions())
                {
                    if (dev.subtag(romload.region.get_tag(region)) == region_fulltag)
                    {
                        bytes_found = region.get_length();
                        break;
                    }
                }

                if (bytes_found != 0)
                    break;
            }

            return report_missing(bytes_found != 0, "memory region", required);
        }


        //-------------------------------------------------
        //  find_memshare - find memory share
        //-------------------------------------------------
        protected PointerU8 find_memshare(u8 width, ref size_t bytes, bool required)  //void *finder_base::find_memshare(u8 width, size_t &bytes, bool required) const
        {
            // look up the share and return nullptr if not found
            memory_share share = m_base.get().memshare(m_tag);
            if (share == null)
                return null;

            // check the width and warn if not correct
            if (width != 0 && share.bitwidth() != width)
            {
                osd_printf_warning("Shared ptr '{0}' found but is width {1}, not {2} as requested\n", m_tag, share.bitwidth(), width);
                return null;
            }

            // return results
            bytes = (size_t)share.bytes();
            return new PointerU8(share.ptr());
        }


        /// \brief Find an address space
        ///
        /// Look up address space and check that its width matches desired
        /// value.  Returns pointer to address space if a matching space
        /// is found, or nullptr otherwise.  Prints a message at warning
        /// level if the address space is required, a device with the
        /// requested tag is found, but it doesn't have a memory interface
        /// or a space with the designated number.
        /// \param [in] spacenum Address space number.
        /// \param [in] width Specific data width, or 0.
        /// \param [in] required Whether warning message should be printed
        ///   if a device with no memory interface or space of that number
        ///   is found.
        /// \return Pointer to address space if a matching address space
        ///   is found, or nullptr otherwise.
        //-------------------------------------------------
        //  find_addrspace - find address space
        //-------------------------------------------------
        protected address_space find_addrspace(int spacenum, u8 width, bool required)
        {
            // look up the device and return nullptr if not found
            device_t device = m_base.get().subdevice(m_tag);
            if (device == null)
                return null;

            // check for memory interface and the specified space number
            device_memory_interface memory;
            if (!device.interface_(out memory))
            {
                if (required)
                    osd_printf_warning("Device '{0}' found but lacks memory interface\n", m_tag);
                return null;
            }

            if (!memory.has_space(spacenum))
            {
                if (required)
                    osd_printf_warning("Device '{0}' found but lacks address space #{1}\n", m_tag, spacenum);
                return null;
            }

            // check data width
            address_space space = memory.space(spacenum);
            if (width != 0 && width != space.data_width())
            {
                osd_printf_warning("Device '{0}' found but address space #{1} has the wrong data width (expected {2}, found {3})\n", m_tag, spacenum, width, space.data_width());
                return null;
            }

            // return result
            return space;
        }


        /// \brief Check that address space exists
        ///
        /// Returns true if the space is required but no matching space is
        /// found, or false otherwise.
        /// \param [in] spacenum Address space number.
        /// \param [in] width Specific data width, or 0.
        /// \param [in] required Whether warning message should be printed
        ///   if a device with no memory interface or space of that number
        ///   is found.
        /// \return True if the space is optional, or if the space is
        ///   space and a matching space is found, or false otherwise.
        //-------------------------------------------------
        //  validate_addrspace - find address space
        //-------------------------------------------------
        protected bool validate_addrspace(int spacenum, u8 width, bool required)
        {
            // look up the device and return false if not found
            device_t device = m_base.get().subdevice(m_tag);
            if (device == null)
                return report_missing(false, "address space", required);

            // check for memory interface and a configuration for the designated space
            device_memory_interface memory = null;
            address_space_config config = null;
            if (device.interface_(out memory))
            {
                config = memory.space_config(spacenum);
                if (required)
                {
                    if (config == null)
                        osd_printf_warning("Device '{0}' found but lacks address space #{1}\n", m_tag, spacenum);
                    else if (width != 0 && width != config.data_width())
                        osd_printf_warning("Device '{0}' found but space #{1} has the wrong data width (expected {2}, found {3})\n", m_tag, spacenum, width, config.data_width());
                }
            }
            else if (required)
            {
                osd_printf_warning("Device '{0}' found but lacks memory interface\n", m_tag);
            }

            // report result
            return report_missing(config != null && (width == 0 || width == config.data_width()), "address space", required);
        }


        //-------------------------------------------------
        //  report_missing - report missing objects and
        //  return true if it's ok
        //-------------------------------------------------
        public virtual bool report_missing(bool found, string objname, bool required)
        {
            if (required && (DUMMY_TAG == m_tag))
            {
                osd_printf_error("Tag not defined for required {0}\n", objname);
                return false;
            }
            else if (found)
            {
                // just pass through in the found case
                return true;
            }
            else
            {
                // otherwise, report
                string region_fulltag = m_base.get().subtag(m_tag);
                if (required)
                    osd_printf_error("Required {0} '{1}' not found\n", objname, region_fulltag);
                else if (DUMMY_TAG != m_tag)
                    osd_printf_verbose("Optional {0} '{1}' not found\n", objname, region_fulltag);

                return !required;
            }
        }
    }


    public interface object_finder_operations<T>
    {
        T cast(device_t device);
        T cast(device_interface device);
        Pointer<T> cast(PointerU8 memory);
    }

    public class object_finder_operations_null<T> : object_finder_operations<T>
    {
        public T cast(device_t device) { throw new emu_unimplemented(); }
        public T cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<T> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_device_palette_interface : object_finder_operations<device_palette_interface>
    {
        public device_palette_interface cast(device_t device) { throw new emu_unimplemented(); }
        public device_palette_interface cast(device_interface device) { return (device_palette_interface)device; }
        public Pointer<device_palette_interface> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_memory_bank : object_finder_operations<memory_bank>
    {
        public memory_bank cast(device_t device) { throw new emu_unimplemented(); }
        public memory_bank cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<memory_bank> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_memory_region : object_finder_operations<memory_region>
    {
        public memory_region cast(device_t device) { throw new emu_unimplemented(); }
        public memory_region cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<memory_region> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_ioport_port : object_finder_operations<ioport_port>
    {
        public ioport_port cast(device_t device) { throw new emu_unimplemented(); }
        public ioport_port cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<ioport_port> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_cpu_device : object_finder_operations<cpu_device>
    {
        public cpu_device cast(device_t device) { return (cpu_device)device; }
        public cpu_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<cpu_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_gfxdecode_device : object_finder_operations<gfxdecode_device>
    {
        public gfxdecode_device cast(device_t device) { return (gfxdecode_device)device; }
        public gfxdecode_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<gfxdecode_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_ls259_device : object_finder_operations<ls259_device>
    {
        public ls259_device cast(device_t device) { return (ls259_device)device; }
        public ls259_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<ls259_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_namco_device : object_finder_operations<namco_device>
    {
        public namco_device cast(device_t device) { return (namco_device)device; }
        public namco_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<namco_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_palette_device : object_finder_operations<palette_device>
    {
        public palette_device cast(device_t device) { return (palette_device)device; }
        public palette_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<palette_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_screen_device : object_finder_operations<screen_device>
    {
        public screen_device cast(device_t device) { return (screen_device)device; }
        public screen_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<screen_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_watchdog_timer_device : object_finder_operations<watchdog_timer_device>
    {
        public watchdog_timer_device cast(device_t device) { return (watchdog_timer_device)device; }
        public watchdog_timer_device cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<watchdog_timer_device> cast(PointerU8 memory) { throw new emu_unimplemented(); }
    }

    public class object_finder_operations_u8 : object_finder_operations<u8>
    {
        public u8 cast(device_t device) { throw new emu_unimplemented(); }
        public u8 cast(device_interface device) { throw new emu_unimplemented(); }
        public Pointer<u8> cast(PointerU8 memory) { return (Pointer<u8>)memory; }
    }


    /// \brief Base class for object discovery helpers
    ///
    /// Abstract template base for auto-discovery of objects of a particular
    /// type.  Provides implicit cast-to-pointer and pointer member access
    /// operators.  Template arguments are the type of object to discover,
    /// and whether failure to find the object is considered an error.
    /// Assumes that non-null pointer is found, and null pointer is not
    /// found.
    //template <class ObjectClass, bool Required>
    public abstract class object_finder_common_base<ObjectClass, bool_Required> : finder_base
        where bool_Required : bool_constant, new()
    {
        static object_finder_operations<ObjectClass> create_object_finder_operations()
        {
            if      (typeof(ObjectClass) == typeof(device_palette_interface)) return (object_finder_operations<ObjectClass>)new object_finder_operations_device_palette_interface();
            else if (typeof(ObjectClass) == typeof(memory_region))            return (object_finder_operations<ObjectClass>)new object_finder_operations_memory_region();
            else if (typeof(ObjectClass) == typeof(cpu_device))               return (object_finder_operations<ObjectClass>)new object_finder_operations_cpu_device();
            else if (typeof(ObjectClass) == typeof(gfxdecode_device))         return (object_finder_operations<ObjectClass>)new object_finder_operations_gfxdecode_device();
            else if (typeof(ObjectClass) == typeof(ls259_device))             return (object_finder_operations<ObjectClass>)new object_finder_operations_ls259_device();
            else if (typeof(ObjectClass) == typeof(namco_device))             return (object_finder_operations<ObjectClass>)new object_finder_operations_namco_device();
            else if (typeof(ObjectClass) == typeof(palette_device))           return (object_finder_operations<ObjectClass>)new object_finder_operations_palette_device();
            else if (typeof(ObjectClass) == typeof(screen_device))            return (object_finder_operations<ObjectClass>)new object_finder_operations_screen_device();
            else if (typeof(ObjectClass) == typeof(watchdog_timer_device))    return (object_finder_operations<ObjectClass>)new object_finder_operations_watchdog_timer_device();
            else if (typeof(ObjectClass) == typeof(u8))                       return (object_finder_operations<ObjectClass>)new object_finder_operations_u8();
            else return new object_finder_operations_null<ObjectClass>();
        }
        protected static readonly object_finder_operations<ObjectClass> ops = create_object_finder_operations();
        protected static readonly bool Required = new bool_Required().value;


        // internal state
        protected Pointer<ObjectClass> m_target = default;  //ObjectClass *m_target = nullptr;


        /// \brief Designated constructor
        ///
        /// Construct base, register with base device to be invoked at
        /// resolution time, and initialise target object pointer to
        /// nullptr.
        /// \param [in] base Base device to search from.
        /// \param [in] tag Object tag to search for.  This is not copied,
        ///   it is the caller's responsibility to ensure this pointer
        ///   remains valid until resolution time.
        protected object_finder_common_base(device_t base_, string tag) : base(base_, tag) { }


        /// \brief Clear temporary binding from configuration
        ///
        /// Object finders may allow temporary binding to the anticipated
        /// target during configuration.  This needs to be cleared to ensure
        /// the correct target is found if a device further up the hierarchy
        /// subsequently removes or replaces devices.
        public override void end_configuration() { assert(!m_resolved); m_target = default; }


        /// \brief Get pointer to target object
        ///
        /// \return Pointer to target object if found, or nullptr otherwise.
        public Pointer<ObjectClass> target() { return new Pointer<ObjectClass>(m_target); }  //ObjectClass *target() const { return m_target; }


        /// \brief Cast-to-pointer operator
        ///
        /// Allows implicit casting to a pointer to the target object.
        /// Returns a null pointer if resolution has not been attempted or
        // object was not found.
        /// \return Pointer to target object if found, or nullptr otherwise.
        //operator ObjectClass *() const { return m_target; }
        public Pointer<ObjectClass> op { get { return new Pointer<ObjectClass>(m_target); } }


        /// \brief Pointer member access operator
        ///
        /// Allows pointer-member-style access to members of the target
        /// object.  Asserts that the target object has been found.
        /// \return Pointer to target object if found, or nullptr otherwise.
        //ObjectClass *operator->() const { assert(m_target); return m_target; }


        protected bool report_missing(string objname) { return base.report_missing(m_target != null, objname, Required); }
    }


    /// \brief Base class for object discovery helpers
    ///
    /// Allows partial specialisations to add members based on object class
    /// and/or whether the object is required.  Template arguments are the
    /// type of object to discover, and whether failure to find the object
    /// is considered an error.
    //template <class ObjectClass, bool Required>
    public abstract class object_finder_base<ObjectClass, bool_Required> : object_finder_common_base<ObjectClass, bool_Required>
        where bool_Required : bool_constant, new()
    {
        //using object_finder_common_base<ObjectClass, Required>::object_finder_common_base;


        protected object_finder_base(device_t base_, string tag) : base(base_, tag) { }


        // EDF - following two functions borrowed from specialized version of this class below

        /// \brief Return whether target has been found
        ///
        /// Works on the assumption that the target object pointer will
        /// be non-null if the target has been found, and null
        /// otherwise.
        /// \return True if object has been found, or false otherwise.
        public bool found() { return this.m_target != null; }


        /// \brief Cast-to-Boolean operator
        ///
        /// Allows truth tests to test whether it's safe to dereference
        /// the object, similar to pointers and various C++ standard
        /// library objects.
        /// \return True if safe to dereference, or false otherwise.
        public bool bool_ { get { return this.m_target != null; } }  //explicit operator bool() const { return this->m_target != nullptr; }
    }


    /// \brief Base class for optional object discovery helpers
    ///
    /// Adds helpers for to test whether the object was found, and by
    /// extension whether it's safe to dereference.  Template argument is
    /// the type of object to discover.
    //template <class ObjectClass>
    //class object_finder_base<ObjectClass, false> : object_finder_common_base<ObjectClass, false>


    /// \brief Device finder template
    ///
    /// Template arguments are the device class to find, and whether the
    /// device is required.  It is a validation error if a required device
    /// is not found.  If a device with matching tag is found but the class
    /// does not match, a message is printed at warning level.  This class
    /// is generally not used directly, instead the optional_device and
    /// required_device helpers are used.
    /// \sa optional_device required_device
    //template<class DeviceClass, bool Required>
    public class device_finder<DeviceClass, bool_Required> : object_finder_base<DeviceClass, bool_Required>
        where bool_Required : bool_constant, new()
    {
        //using object_finder_base<DeviceClass, Required>::set_tag;


        /// \brief Device finder constructor
        /// \param [in] base Base device to search from.
        /// \param [in] tag Device tag to search for.  This is not copied,
        ///   it is the caller's responsibility to ensure this pointer
        ///   remains valid until resolution time.
        public device_finder(device_t base_, string tag) : base(base_, tag) { }


        /// \brief Set search tag
        ///
        /// Allows search tag to be changed after construction.  Note that
        /// this must be done before resolution time to take effect.  Note
        /// that this binds to a particular instance, so the device must not
        /// be removed or replaced, as it will cause a use-after-free when
        /// resolving objects.
        /// \param [in] object Object to refer to.
        //void set_tag(DeviceClass &object) { set_tag(object, DEVICE_SELF); }


        /// \brief Set target during configuration
        ///
        /// During configuration, device_finder instances may be assigned
        /// a reference to the anticipated target device to avoid the need
        /// for tempories during configuration.  Normal resolution will
        /// still happen after machine configuration is completed to ensure
        /// device removal/replacement is handled properly.
        /// \param [in] device Reference to anticipated target device.
        /// \return The same reference supplied by the caller.
        //template <typename T>
        //std::enable_if_t<std::is_convertible<T *, DeviceClass *>::value, T &> operator=(T &device)
        public DeviceClass assign(DeviceClass device)
        {
            assert(!this.m_resolved);

            //assert(is_expected_tag(device));
            if (device is device_t d) assert(is_expected_tag_device_t(d));
            else if (device is device_interface di) assert(is_expected_tag_device_interface(di));
            else throw new emu_unimplemented();

            this.m_target = new Pointer<DeviceClass>(new MemoryContainer<DeviceClass>() { device });
            return device;
        }

        /// \brief Check that device implementation has expected tag
        /// \param [in] device Reference to device.
        /// \return True if supplied device matches the configured target
        ///   tag, or false otherwise.
        //template <typename T>
        bool is_expected_tag_device_t(device_t device)  //std::enable_if_t<emu::detail::is_device_implementation<T>::value, bool> is_expected_tag(T const &device) const
        {
            return this.m_base.get().subtag(this.m_tag) == device.tag();  //return this->m_base.get().subtag(this->m_tag) == device.tag();
        }

        /// \brief Check that device mixin has expected tag
        /// \param [in] device Reference to interface/mixin.
        /// \return True if supplied mixin matches the configured target
        ///   tag, or false otherwise.
        //template <typename T>
        bool is_expected_tag_device_interface(device_interface interface_)  //std::enable_if_t<emu::detail::is_device_interface<T>::value, bool> is_expected_tag(T const &interface) const
        {
            return this.m_base.get().subtag(this.m_tag) == interface_.device().tag();  //return this->m_base.get().subtag(this->m_tag) == interface.device().tag();
        }

        // finder
        /// \brief Find device
        ///
        /// Find device of desired type with requested tag.  If a device
        /// with the requested tag is found but the type is incorrect, a
        /// warning message will be printed.  This method is called by the
        /// base device at resolution time.
        /// \param [in] valid Pass a pointer to the validity checker if this
        ///   is a dry run (i.e. no intention to actually start the device),
        ///   or nullptr otherwise.
        /// \return True if the device is optional or if a matching device
        ///   is found, false otherwise.
        public override bool findit(validity_checker valid)
        {
            if (valid == null)
            {
                assert(!this.m_resolved);
                this.m_resolved = true;
            }

            device_t device = this.m_base.get().subdevice(this.m_tag);  //device_t *const device = this->m_base.get().subdevice(this->m_tag);

            if (device == null)
                this.m_target = new Pointer<DeviceClass>(new MemoryContainer<DeviceClass>() { default });  //this->m_target = dynamic_cast<DeviceClass *>(device);
            else if (typeof(DeviceClass) == typeof(device_palette_interface) && device is palette_device)
                this.m_target = new Pointer<DeviceClass>(new MemoryContainer<DeviceClass>() { ops.cast(((palette_device)device).dipalette) });  //this->m_target = dynamic_cast<DeviceClass *>(device);
            else
                this.m_target = new Pointer<DeviceClass>(new MemoryContainer<DeviceClass>() { ops.cast(device) });  //this->m_target = dynamic_cast<DeviceClass *>(device);

            if (device != null && this.m_target == default)
            {
                osd_printf_warning("Device '{0}' found but is of incorrect type (actual type is {1})\n", this.m_tag, device.name());
            }

            return this.report_missing("device");
        }
    }


    /// \brief Optional device finder
    ///
    /// Finds device with matching type and tag.  If a device with matching
    /// tag is found but the type does not match, a message is printed at
    /// warning level.  No error is generated if a matching device is not
    /// found (the target object pointer will be null).  If you have a
    /// number of similar optional devices, consider using
    /// optional_device_array.
    /// \sa required_device optional_device_array device_finder
    //template <class DeviceClass> using optional_device = device_finder<DeviceClass, false>;
    public class optional_device<DeviceClass> : device_finder<DeviceClass, bool_constant_false>
    {
        public optional_device(device_t base_, string tag) : base(base_, tag) { }
    }


    /// \brief Required device finder
    ///
    /// Finds device with matching type and tag.  If a device with matching
    /// tag is found but the type does not match, a message is printed at
    /// warning level.  A validation error is generated if a matching device
    /// is not found.  If you have a number of similar required devices,
    /// consider using required_device_array.
    /// \sa optional_device required_device_array device_finder
    //template <class DeviceClass> using required_device = device_finder<DeviceClass, true>;
    public class required_device<DeviceClass> : device_finder<DeviceClass, bool_constant_true>
    {
        public required_device(device_t base_, string tag) : base(base_, tag) { }
    }


    //template <class DeviceClass, unsigned Count, bool Required> using device_array_finder = object_array_finder<device_finder<DeviceClass, Required>, Count>;
    public class device_array_finder<DeviceClass, unsigned_Count, bool_Required> : object_array_finder<device_finder<DeviceClass, bool_Required>, unsigned_Count>
        where unsigned_Count : uint32_constant, new()
        where bool_Required : bool_constant, new()
    {
        protected device_array_finder(device_t base_, string format, int start, Func<device_t, string, finder_base> creator_func) : base(base_, format, start, creator_func) { }
    }


    //template <class DeviceClass, unsigned Count> using optional_device_array = device_array_finder<DeviceClass, Count, false>;
    public class optional_device_array<DeviceClass, unsigned_Count> : device_array_finder<DeviceClass, unsigned_Count, bool_constant_false>
        where unsigned_Count : uint32_constant, new()
    {
        public optional_device_array(device_t base_, string format, int start, Func<device_t, string, device_finder<DeviceClass, bool_constant_false>> creator_func) : base(base_, format, start, creator_func) { }
    }


    //template <class DeviceClass, unsigned Count> using required_device_array = device_array_finder<DeviceClass, Count, true>;
    public class required_device_array<DeviceClass, unsigned_Count> : device_array_finder<DeviceClass, unsigned_Count, bool_constant_true>
        where unsigned_Count : uint32_constant, new()
    {
        public required_device_array(device_t base_, string format, int start, Func<device_t, string, device_finder<DeviceClass, bool_constant_true>> creator_func) : base(base_, format, start, creator_func) { }
    }


    /// \brief Memory region finder template
    ///
    /// Template argument is whether the memory region is required.  It is a
    /// validation error if a required memory region is not found.  This
    /// class is generally not used directly, instead the
    /// optional_memory_region and required_memory_region helpers are used.
    /// \sa optional_memory_region required_memory_region
    //template <bool Required>
    public class memory_region_finder<bool_Required> : object_finder_base<memory_region, bool_Required>
        where bool_Required : bool_constant, new()
    {
        /// \brief Memory region finder constructor
        /// \param [in] base Base device to search from.
        /// \param [in] tag Memory region tag to search for.  This is not
        ///   copied, it is the caller's responsibility to ensure this
        ///   pointer remains valid until resolution time.
        public memory_region_finder(device_t base_device, string tag) : base(base_device, tag) { }


        /// \brief Find memory region
        ///
        /// Find memory region with requested tag.  For a dry run, the
        /// target object pointer will not be set.  This method is called by
        /// the base device at resolution time.
        /// \param [in] valid Pass a pointer to the validity checker if this
        ///   is a dry run (i.e. no intention to actually start the device),
        ///   or nullptr otherwise.
        /// \return True if the memory region is optional or if a matching
        ///   memory region is found, false otherwise.
        public override bool findit(validity_checker valid)
        {
            if (valid != null)
                return this.validate_memregion(Required);

            assert(!this.m_resolved);
            this.m_resolved = true;
            this.m_target = new Pointer<memory_region>(new MemoryContainer<memory_region>() { this.m_base.get().memregion(this.m_tag) });  //this->m_target = this->m_base.get().memregion(this->m_tag);
            return report_missing("memory region");
        }
    }


    /// \brief Optional memory region finder
    ///
    /// Finds memory region with maching tag.  No error is generated if a
    /// matching memory region is not found (the target object pointer will
    /// be null).  If you have a number of similar optional memory regions,
    /// consider using optional_memory_region_array.
    /// \sa required_memory_region optional_memory_region_array
    ///   memory_region_finder
    //using optional_memory_region = memory_region_finder<false>;

    /// \brief Required memory region finder
    ///
    /// Finds memory region with maching tag.  A validation error is
    /// generated if a matching memory region is not found.  If you have a
    /// number of similar required memory regions, consider using
    /// required_memory_region_array.
    /// \sa optional_memory_region required_memory_region_array
    ///   memory_region_finder
    //using required_memory_region = memory_region_finder<true>;

    //template <unsigned Count, bool Required> using memory_region_array_finder = object_array_finder<memory_region_finder<Required>, Count>;
    //template <unsigned Count> using optional_memory_region_array = memory_region_array_finder<Count, false>;
    //template <unsigned Count> using required_memory_region_array = memory_region_array_finder<Count, true>;


    /// \brief Memory bank finder template
    ///
    /// Template argument is whether the memory bank is required.  It is a
    /// validation error if a required memory bank is not found.  This class
    /// is generally not used directly, instead the optional_memory_bank and
    /// required_memory_bank helpers are used.
    /// \sa optional_memory_bank required_memory_bank
    //template <bool Required>
    public class memory_bank_finder<bool_Required> : object_finder_base<memory_bank, bool_Required>
        where bool_Required : bool_constant, new()
    {
        /// \brief Memory bank finder constructor
        /// \param [in] base Base device to search from.
        /// \param [in] tag Memory bank tag to search for.  This is not
        ///   copied, it is the caller's responsibility to ensure this
        ///   pointer remains valid until resolution time.
        public memory_bank_finder(device_t base_, string tag) : base(base_, tag) { }


        /// \brief Find memory bank
        ///
        /// Find memory bank with requested tag.  Just returns true for a
        /// dry run.  This method is called by the base device at resolution
        /// time.
        /// \param [in] valid Pass a pointer to the validity checker if this
        ///   is a dry run (i.e. no intention to actually start the device),
        ///   or nullptr otherwise.
        /// \return True if the memory bank is optional, a matching memory
        ///   bank is found or this is a dry run, false otherwise.
        public override bool findit(validity_checker valid)
        {
            if (valid != null)
                return true;

            assert(!this.m_resolved);
            this.m_resolved = true;
            this.m_target = new Pointer<memory_bank>(new MemoryContainer<memory_bank>() { this.m_base.get().membank(this.m_tag) });  //this->m_target = this->m_base.get().membank(this->m_tag);
            return this.report_missing("memory bank");
        }
    }


    /// \brief Optional memory bank finder
    ///
    /// Finds memory bank with matching tag.  No error is generated if a
    /// matching memory bank is not found (the target object pointer will
    /// be null).  If you have a number of similar optional memory banks,
    /// consider using optional_memory_bank_array.
    /// \sa required_memory_bank optional_memory_bank_array
    ///   memory_bank_finder
    //using optional_memory_bank = memory_bank_finder<false>;

    /// \brief Required memory bank finder
    ///
    /// Finds memory bank with matching tag.  A validation error is
    /// generated if a matching memory bank is not found.  If you have a
    /// number of similar required memory banks, consider using
    /// required_memory_bank_array.
    /// \sa optional_memory_bank required_memory_bank_array
    ///   memory_bank_finder
    //using required_memory_bank = memory_bank_finder<true>;


    //template <unsigned Count, bool Required> using memory_bank_array_finder = object_array_finder<memory_bank_finder<Required>, Count>;
    public class memory_bank_array_finder<unsigned_Count, bool_Required> : object_array_finder<memory_bank_finder<bool_Required>, unsigned_Count>
        where unsigned_Count : uint32_constant, new()
        where bool_Required : bool_constant, new()
    {
        protected memory_bank_array_finder(device_t base_, string format, int start) : base(base_, format, start)
        {
            for (int i = 0; i < Count; i++)
                m_array[i] = new memory_bank_finder<bool_Required>(base_, m_tag[i]);
        }
    }


    //template <unsigned Count> using optional_memory_bank_array = memory_bank_array_finder<Count, false>;
    public class optional_memory_bank_array<unsigned_Count> : memory_bank_array_finder<unsigned_Count, bool_constant_false>
        where unsigned_Count : uint32_constant, new()
    {
        public optional_memory_bank_array(device_t base_, string format, int start) : base(base_, format, start) { }
    }

    //template <unsigned Count> using required_memory_bank_array = memory_bank_array_finder<Count, true>;
    public class required_memory_bank_array<unsigned_Count> : memory_bank_array_finder<unsigned_Count, bool_constant_true>
        where unsigned_Count : uint32_constant, new()
    {
        public required_memory_bank_array(device_t base_, string format, int start) : base(base_, format, start) { }
    }


    /// \brief Memory bank creator
    ///
    /// Creates a memory bank or finds an existing one instantiated via an
    /// address map.
    class memory_bank_creator : finder_base
    {
        /// \brief Pointer to target object
        ///
        /// Pointer to target memory bank object, or nullptr if creation has
        /// not been attempted yet.
        memory_bank m_target = null;


        /// \brief Memory bank creator constructor
        /// \param [in] base Base device to search from.
        /// \param [in] tag Memory bank tag to search for or create.  This
        ///   is not copied, it is the caller's responsibility to ensure
        ///   this pointer remains valid until resolution time.
        public memory_bank_creator(device_t base_, string tag) : base(base_, tag) { }

        /// \brief Get pointer to memory bank object
        /// \return Pointer to found or created bank object.
        //memory_bank *target() const { return m_target; }

        /// \brief Cast-to-pointer operator
        ///
        /// Allows implicit casting to a pointer to the target
        /// memory bank object.
        /// \return Pointer to found or created memory bank object.
        //operator memory_bank *() const { return m_target; }

        /// \brief Pointer member access operator
        ///
        /// Allows pointer-member-style access to members of the target
        /// memory bank object.  Asserts that the target has been found.
        /// \return Pointer to found or created bank object.
        //memory_bank *operator->() const { assert(m_target); return m_target; }


        public override bool findit(validity_checker valid)
        {
            throw new emu_unimplemented();
        }


        public override void end_configuration()
        {
            m_target = null;
        }
    }


    //template <unsigned Count> using memory_bank_array_creator = object_array_finder<memory_bank_creator, Count>;


    /// \brief Base class for optional I/O port finders
    ///
    /// Adds helpers for to test whether the I/O port was found, and to read
    /// the port value or return a default if the I/O port was not found.
    //template <>
    //class object_finder_base<ioport_port, false> : public object_finder_common_base<ioport_port, false>


    /// \brief I/O port finder template
    ///
    /// Template argument is whether the I/O port is required.  It is a
    /// validation error if a required I/O port is not found.  This class is
    /// generally not used directly, instead the optional_ioport and
    /// required_ioport helpers are used.
    /// \sa optional_ioport required_ioport
    //template <bool Required>
    public class ioport_finder<bool_Required> : object_finder_base<ioport_port, bool_Required>
        where bool_Required : bool_constant, new()
    {
        /// \brief I/O port finder constructor
        /// \param [in] base Base device to search from.
        /// \param [in] tag I/O port tag to search for.  This is not copied,
        ///   it is the caller's responsibility to ensure this pointer
        ///   remains valid until resolution time.
        public ioport_finder(device_t base_, string tag) : base(base_, tag) { }


        /// \brief Find I/O port
        ///
        /// Find I/O port with requested tag.  Just returns true for a dry
        /// run.  This method is called by the base device at resolution
        /// time.
        /// \param [in] valid Pass a pointer to the validity checker if this
        ///   is a dry run (i.e. no intention to actually start the device),
        ///   or nullptr otherwise.
        /// \return True if the I/O port is optional, a matching I/O port is
        ///   is found or this is a dry run, false otherwise.
        public override bool findit(validity_checker valid)
        {
            if (valid != null)
                return base.report_missing(!valid.ioport_missing(this.m_base.get().subtag(this.m_tag)), "I/O port", Required);

            assert(!this.m_resolved);
            this.m_resolved = true;
            this.m_target = new Pointer<ioport_port>(new MemoryContainer<ioport_port>() { this.m_base.get().ioport(this.m_tag) });  //this->m_target = this->m_base.get().ioport(this->m_tag);
            return this.report_missing("I/O port");
        }
    }


    /// \brief Optional I/O port finder
    ///
    /// Finds I/O port with matching tag.  No error is generated if a
    /// matching I/O port is not found (the target object pointer will be
    /// null).  If you have a number of similar optional I/O ports, consider
    /// using optional_ioport_array.
    /// \sa required_ioport optional_ioport_array ioport_finder
    //using optional_ioport = ioport_finder<false>;
    public class optional_ioport : ioport_finder<bool_constant_false>
    {
        public optional_ioport(device_t base_, string tag) : base(base_, tag) { }
    }

    /// \brief Required I/O port finder
    ///
    /// Finds I/O port with matching tag.  A validation error is generated
    /// if a matching I/O port is not found.  If you have a number of
    /// similar required I/O ports, consider using required_ioport_array.
    /// \sa optional_ioport required_ioport_array ioport_finder
    //using required_ioport = ioport_finder<true>;
    public class required_ioport : ioport_finder<bool_constant_true>
    {
        public required_ioport(device_t base_, string tag) : base(base_, tag) { }
    }


    //template <unsigned Count, bool Required> using ioport_array_finder = object_array_finder<ioport_finder<Required>, Count>;
    public class ioport_array_finder<unsigned_Count, bool_Required> : object_array_finder<ioport_finder<bool_Required>, unsigned_Count>
        where unsigned_Count : uint32_constant, new()
        where bool_Required : bool_constant, new()
    {
        protected ioport_array_finder(device_t base_, string format, int start) : base(base_, format, start)
        {
            for (int i = 0; i < Count; i++)
                m_array[i] = new ioport_finder<bool_Required>(base_, m_tag[i]);
        }
    }


    //template <unsigned Count> using optional_ioport_array = ioport_array_finder<Count, false>;
    public class optional_ioport_array<unsigned_Count> : ioport_array_finder<unsigned_Count, bool_constant_false>
        where unsigned_Count : uint32_constant, new()
    {
        public optional_ioport_array(device_t base_, string format, int start) : base(base_, format, start) { }
    }

    //template <unsigned Count> using required_ioport_array = ioport_array_finder<Count, true>;
    public class required_ioport_array<unsigned_Count> : ioport_array_finder<unsigned_Count, bool_constant_true>
        where unsigned_Count : uint32_constant, new()
    {
        public required_ioport_array(device_t base_, string format, int start) : base(base_, format, start) { }
    }


    /// \brief Address space finder template
    ///
    /// Template argument is whether the address space is required.  It is a
    /// validation error if a required address space is not found.  This
    /// class is generally not used directly, instead the
    /// optional_address_space and required_address_space helpers are used.
    /// \sa optional_address_space required_address_space
    //template <bool Required>
    class address_space_finder<bool_Required> : object_finder_base<address_space, bool_Required>
        where bool_Required : bool_constant, new()
    {
        int m_spacenum;
        u8 m_data_width;


        /// \brief Address space finder constructor
        /// \param [in] base Base device to search from.
        /// \param [in] tag Address space device tag to search for.
        ///   This is not copied, it is the caller's responsibility to
        ///   ensure this pointer remains valid until resolution time.
        /// \param [in] spacenum Address space number.
        /// \param [in] width Required data width in bits, or zero if
        ///   any data width is acceptable.
        public address_space_finder(device_t base_, string tag, int spacenum, u8 width = 0)
            : base(base_, tag)
        {
            m_spacenum = spacenum;
            m_data_width = width;
        }


        /// \brief Set search tag and space number
        ///
        /// Allows search tag to be changed after construction.  Note that
        /// this must be done before resolution time to take effect.  Also
        /// note that the tag is not copied.
        /// \param [in] base Updated search base.  The tag must be specified
        ///   relative to this device.
        /// \param [in] tag Updated search tag.  This is not copied, it is
        ///   the caller's responsibility to ensure this pointer remains
        ///   valid until resolution time.
        /// \param [in] spacenum Address space number.
        //void set_tag(device_t &base, char const *tag, int spacenum) { finder_base::set_tag(base, tag); m_spacenum = spacenum; }

        /// \brief Set search tag and space number
        ///
        /// Allows search tag and address space number to be changed after
        /// construction.  Note that this must be done before resolution
        /// time to take effect.  Also note that the tag is not copied.
        /// \param [in] tag Updated search tag relative to the current
        ///   device being configured.  This is not copied, it is the
        ///   caller's responsibility to ensure this pointer remains valid
        ///   until resolution time.
        /// \param [in] spacenum Address space number.
        //void set_tag(char const *tag, int spacenum) { finder_base::set_tag(tag); m_spacenum = spacenum; }

        /// \brief Set search tag and space number
        ///
        /// Allows search tag and address space number to be changed after
        /// construction.  Note that this must be done before resolution
        /// time to take effect.  Also note that the tag is not copied.
        /// \param [in] finder Object finder to take the search base and tag
        ///   from.
        /// \param [in] spacenum Address space number.
        public void set_tag(finder_base finder, int spacenum) { base.set_tag(finder); this.m_spacenum = spacenum; }

        /// \brief Set search tag and space number
        ///
        /// Allows search tag and address space number to be changed after
        /// construction.  Note that this must be done before resolution
        /// time to take effect.  Also note that the tag is not copied.
        /// \param [in] finder Address space finder to take the search base,
        ///   tag and address space number from.
        //template <bool R> void set_tag(address_space_finder<R> const &finder) { set_tag(finder, finder.spacenum()); }

        /// \brief Set data width of space
        ///
        /// Allows data width to be specified after construction.  Note
        /// that this must be done before resolution time to take effect.
        /// \param [in] width Required data width in bits, or zero if any
        ///   data width is acceptable.
        //void set_data_width(u8 width) { assert(!this->m_resolved); this->m_data_width = width; }

        /// \brief Get space number
        ///
        /// Returns the configured address space number.
        /// \return The space number to be found.
        //int spacenum() const { return m_spacenum; }

        /// \brief Find address space
        ///
        /// Find address space with requested tag.  For a dry run, the
        /// target object pointer will not be set.  This method is called by
        /// the base device at resolution time.
        /// \param [in] valid Pass a pointer to the validity checker if this
        ///   is a dry run (i.e. no intention to actually start the device),
        ///   or nullptr otherwise.
        /// \return True if the address space is optional, a matching
        ///   address space is found or this is a dry run, false otherwise.
        public override bool findit(validity_checker valid)
        {
            if (valid != null)
                return this.validate_addrspace(this.m_spacenum, this.m_data_width, Required);

            assert(!this.m_resolved);
            this.m_resolved = true;
            this.m_target = new Pointer<address_space>(new MemoryContainer<address_space>() { this.find_addrspace(this.m_spacenum, this.m_data_width, Required) });
            return this.report_missing("address space");
        }
    }


    /// \brief Optional address space finder
    ///
    /// Finds address space with matching tag and number.  No error is
    /// generated if a matching address space is not found (the target
    /// object pointer will be null).
    /// \sa required_address_space address_space_finder
    //using optional_address_space = address_space_finder<false>;

    /// \brief Required address space finder
    ///
    /// Finds address space with matching tag and number.  A validation
    /// error is generated if a matching address space is not found.
    /// \sa optional_address_space address_space_finder
    //using required_address_space = address_space_finder<true>;


    /// \brief Memory region base pointer finder
    ///
    /// Template arguments are the element type of the memory region and
    /// whether the memory region is required.  It is a validation error if
    /// a required memory region is not found.  This class is generally not
    /// used directly, instead the optional_region_ptr and
    /// required_region_ptr helpers are used.
    /// \sa optional_region_ptr required_region_ptr
    //template<typename PointerType, bool Required>
    class region_ptr_finder<PointerType, bool_Required> : object_finder_base<PointerType, bool_Required>  //class region_ptr_finder : public object_finder_base<PointerType, Required>
        where bool_Required : bool_constant, new()
    {
        /// \brief Region length
        ///
        /// Actual length of the region that was found in units of
        /// elements, or zero if no matching region has been found.
        size_t m_length;


        // construction/destruction
        protected region_ptr_finder(device_t base_, string tag)
            : base(base_, tag)
        {
            m_length = 0;
        }


        /// \brief Array access operator
        ///
        /// Returns a non-const reference to the element of the memory
        /// region at the supplied zero-based index.  Behaviour is undefined
        /// for negative element indices.
        /// \param [in] index Non-negative element index.
        /// \return Non-const reference to element at requested index.
        //PointerType &operator[](int index) const { assert(index < m_length); return this->m_target[index]; }
        public Pointer<PointerType> this[int index] { get { assert(index < m_length); return new Pointer<PointerType>(this.m_target, index); } set { throw new emu_unimplemented(); } }
        public Pointer<PointerType> this[UInt32 index] { get { assert(index < m_length); return new Pointer<PointerType>(this.m_target, (int)index); } set { throw new emu_unimplemented(); } }


        /// \brief Get length in units of elements
        ///
        /// \return Length in units of elements or zero if no matching
        ///   memory region has been found.
        //size_t length() const { return m_length; }

        /// \brief Get length in units of bytes
        ///
        /// \return Length in units of bytes or zero if no matching memory
        ///   region has been found.
        //size_t bytes() const { return m_length * sizeof(PointerType); }


        /// \brief Find memory region base pointer
        ///
        /// Find base pointer of memory region with with requested tag,
        /// width and length.  Width of memory region is checked against
        /// sizeof(PointerType).  For a dry run, only the tag and length are
        /// checked - the width is not checked and the target pointer is not
        /// set.  This method is called by the base device at resolution
        /// time.
        /// \param [in] valid Pass a pointer to the validity checker if this
        ///   is a dry run (i.e. no intention to actually start the device),
        ///   or nullptr otherwise.
        /// \return True if the memory region is optional or a matching
        ///   memory region is found, or false otherwise.
        public override bool findit(validity_checker valid)
        {
            if (valid != null)
                return this.validate_memregion(Required);

            assert(!this.m_resolved);
            this.m_resolved = true;
            this.m_target = ops.cast(new PointerU8(this.find_memregion((u8)sizeof_(typeof(PointerType)), out m_length, Required)));  //this->m_target = reinterpret_cast<PointerType *>(this->find_memregion(sizeof(PointerType), m_length, Required));
            return this.report_missing("memory region");
        }
    }


    /// \brief Optional memory region base pointer finder
    ///
    /// Finds base pointer of memory region with matching tag, width and
    /// length.  No error is generated if a matching memory region is not
    /// found (the target pointer will be null).  If you have a number of
    /// similar optional memory regions, consider using
    /// optional_region_ptr_array.
    /// \sa required_region_ptr optional_region_ptr_array region_ptr_finder
    //template <typename PointerType> using optional_region_ptr = region_ptr_finder<PointerType, false>;
    class optional_region_ptr<PointerType> : region_ptr_finder<PointerType, bool_constant_false>
    {
        public optional_region_ptr(device_t base_, string tag) : base(base_, tag) { }
    }

    /// \brief Required memory region base pointer finder
    ///
    /// Finds base pointer of memory region with matching tag, width and
    /// length.  A validation error is generated if a matching memory region
    /// is not found.  If you have a number of similar required memory
    /// regions, consider using required_region_ptr_array.
    /// \sa optional_region_ptr required_region_ptr_array region_ptr_finder
    //template <typename PointerType> using required_region_ptr = region_ptr_finder<PointerType, true>;
    class required_region_ptr<PointerType> : region_ptr_finder<PointerType, bool_constant_true>
    {
        public required_region_ptr(device_t base_, string tag) : base(base_, tag) { }
    }


    //template <typename PointerType, unsigned Count, bool Required> using region_ptr_array_finder = object_array_finder<region_ptr_finder<PointerType, Required>, Count>;
    //template <typename PointerType, unsigned Count> using optional_region_ptr_array = region_ptr_array_finder<PointerType, Count, false>;
    //template <typename PointerType, unsigned Count> using required_region_ptr_array = region_ptr_array_finder<PointerType, Count, true>;


    /// \brief Memory share base pointer finder
    ///
    /// Template arguments are the element type of the memory share and
    /// whether the memory share is required.  It is a validation error if a
    /// required memory share is not found.  This class is generally not
    /// used directly, instead the optional_shared_ptr and
    /// required_shared_ptr helpers are used.
    /// \sa optional_shared_ptr required_shared_ptr
    //template<typename PointerType, bool Required>
    public class shared_ptr_finder<PointerType, bool_Required> : object_finder_base<PointerType, bool_Required>
        where bool_Required : bool_constant, new()
    {
        /// \brief Memory share length
        ///
        /// Actual length of the memory share that was found in bytes, or
        /// zero if no matching region has been found.
        size_t m_bytes;


        /// \brief Memory share finder constructor
        ///
        /// Desired width is implied by sizeof(PointerType).
        /// \param [in] base Base device to search from.
        /// \param [in] tag Memory share tag to search for.  This is not
        ///   copied, it is the caller's responsibility to ensure this
        ///   pointer remains valid until resolution time.
        public shared_ptr_finder(device_t base_, string tag)
            : base(base_, tag)
        {
            m_bytes = 0;
        }


        /// \brief Array access operator
        ///
        /// Returns a non-const reference to the element of the memory
        /// share at the supplied zero-based index.  Behaviour is undefined
        /// for negative element indices.
        /// \param [in] index Non-negative element index.
        /// \return Non-const reference to element at requested index.
        //PointerType &operator[](int index) const { return this->m_target[index]; }
        public Pointer<PointerType> this[int index] { get { return new Pointer<PointerType>(this.m_target, index); } set { throw new emu_unimplemented(); } }
        public Pointer<PointerType> this[UInt32 index] { get { return new Pointer<PointerType>(this.m_target, (int)index); } set { throw new emu_unimplemented(); } }


        /// \brief Get length in units of elements
        ///
        /// \return Length in units of elements or zero if no matching
        ///   memory region has been found.
        //size_t length() const { return m_bytes / sizeof(PointerType); }

        /// \brief Get length in bytes
        ///
        /// \return Length in bytes or zero if no matching memory share has
        ///   been found.
        public size_t bytes() { return m_bytes; }


        /// \brief Find memory share base pointer
        ///
        /// Find base pointer of memory share with with requested tag.
        /// Width of memory share is checked against sizeof(PointerType).
        /// This method is called by the base device at resolution time.
        /// \param [in] valid Pass a pointer to the validity checker if this
        ///   is a dry run (i.e. no intention to actually start the device),
        ///   or nullptr otherwise.
        /// \return True if the memory share is optional or a matching
        ///   memory share is found, or false otherwise.
        public override bool findit(validity_checker valid)
        {
            if (valid != null)
                return true;

            assert(!this.m_resolved);
            this.m_resolved = true;
            this.m_target = ops.cast(this.find_memshare((u8)(sizeof_(typeof(PointerType)) * 8), ref m_bytes, Required));  //this->m_target = reinterpret_cast<PointerType *>(this->find_memshare(sizeof(PointerType)*8, m_bytes, Required));
            return this.report_missing("shared pointer");
        }
    }


    /// \brief Optional memory share base pointer finder
    ///
    /// Finds base pointer of memory share with matching tag and width.  No
    /// error is generated if a matching memory share is not found (the
    /// target pointer will be null).  If you have a number of similar
    /// optional memory regions, consider using optional_shared_ptr_array.
    /// \sa required_shared_ptr optional_shared_ptr_array shared_ptr_finder
    //template <typename PointerType> using optional_shared_ptr = shared_ptr_finder<PointerType, false>;
    public class optional_shared_ptr<PointerType> : shared_ptr_finder<PointerType, bool_constant_false>
    {
        public optional_shared_ptr(device_t base_, string tag)
            : base(base_, tag) { }
    }


    /// \brief Required memory share base pointer finder
    ///
    /// Finds base pointer of memory share with matching tag and width.  A
    /// validation error is generated if a matching memory share is not
    /// found.  If you have a number of similar required memory shares,
    /// consider using required_shared_ptr_array.
    /// \sa optional_shared_ptr required_shared_ptr_array shared_ptr_finder
    //template <typename PointerType> using required_shared_ptr = shared_ptr_finder<PointerType, true>;
    class required_shared_ptr<PointerType> : shared_ptr_finder<PointerType, bool_constant_true>
    {
        public required_shared_ptr(device_t base_, string tag)
            : base(base_, tag) { }
    }


    //template <typename PointerType, unsigned Count, bool Required> using shared_ptr_array_finder = object_array_finder<shared_ptr_finder<PointerType, Required>, Count>;
    //template <typename PointerType, unsigned Count> using optional_shared_ptr_array = shared_ptr_array_finder<PointerType, Count, false>;
    //template <typename PointerType, unsigned Count> using required_shared_ptr_array = shared_ptr_array_finder<PointerType, Count, true>;


    /// \brief Memory share creator template
    ///
    /// Creates a memory share or finds an existing one instantiated via an
    /// address map.  Template argument is the element type of the memory
    /// share.  If an existing memory share is found, it is an error if it
    /// doesn't match the requested width, length and endianness.
    //template <typename PointerType>
    //class memory_share_creator : finder_base

    //template <typename PointerType, unsigned Count> using memory_share_array_creator = object_array_finder<memory_share_creator<PointerType>, Count>;
}
