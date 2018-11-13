// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using u8 = System.Byte;


namespace mame
{
    /// \brief Helper class to find arrays of devices, etc.
    ///
    /// Useful when a machine/device has a number of similar subdevices, I/O
    /// ports, memory regions, etc.  Template arguments are the element type
    /// and number of elements in the array.  It's assumed that the element
    /// can be constructed with a device_t reference and a C string tag.
    //template <typename T, unsigned Count>
    public class object_array_finder<T>
    {
        int Count;

        /// \brief Generated tag names
        ///
        /// Finder objects do not copy the search tag supplied at
        /// construction.  Tags that are programmatically generated at
        /// construction are stored here so they persist until resolution
        /// time (and beyond).
        protected string [] m_tag;  //std::string const m_tag[Count];

        /// \brief The object discovery elements
        ///
        /// These are the actual object discovery helpers.  Note that this
        /// member must be initialised after m_tag, as it may depend on
        /// programmatically generated tags.
        protected T [] m_array;  //T m_array[Count];

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


        //template <typename F, typename... Param, unsigned... V>
        //object_array_finder(device_t &base, F const &fmt, unsigned start, std::integer_sequence<unsigned, V...>, Param const &... arg)
        //    : m_tag{ util::string_format(fmt, start + V)... }
        //    , m_array{ { base, m_tag[V].c_str(), arg... }... }
        //{
        //}

        protected object_array_finder(int Count, device_t basedevice, string fmt, int start)
        {
            this.Count = Count;


            //m_tag{ util::string_format(fmt, start + V)... }
            m_tag = new string[Count];
            for (int i = 0; i < Count; i++)
                m_tag[i] = string.Format(fmt, start + i);

            //m_array{ { base, m_tag[V].c_str(), arg... }... }
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
        /// \arg [in] Optional additional constructor argument(s) passed to
        ///   all elements.
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
        /// \arg [in] Optional additional constructor argument(s) passed to
        ///   all elements.
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
        /// Returns aconstant iterator one past the last element in the
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
        /// Returns whether the arary has no elements (compile-time
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
        public virtual T op(int index) { return at(index); }

        /// \brief Checked element accesor
        ///
        /// Returns a reference to the element at the supplied index if less
        /// than the size of the array, or throws std::out_of_range
        /// otherwise.
        /// \param [in] index Index of desired element (zero-based).
        /// \return Reference to element at specified index.
        /// \throw std::out_of_range
        //T const &at(unsigned index) const { if (Count > index) return m_array[index]; else throw std::out_of_range("Index out of range"); }
        //T &at(unsigned index) { if (Count > index) return m_array[index]; else throw std::out_of_range("Index out of range"); }
        public virtual T at(int index) { return m_array[index]; }
    }


    // ======================> finder_base
    /// \brief Base class for object discovery helpers
    ///
    /// Abstract non-template base class for object auto-discovery helpers.
    /// Provides the interface that the device_t uses to manage discovery at
    /// resolution time.
    public abstract class finder_base
    {
        /// \brief Dummy tag always treated as not found
        public const string DUMMY_TAG = "finder_dummy_tag";


        // internal state

        /// \brief Pointer to next registered discovery helper
        ///
        /// This is a polymorphic class, so it can't be held in a standardlist
        /// container that requires elements of the same type.  Hence it
        /// implements basic single-linked list behaviour.
        finder_base m_next;

        /// \brief Base device to search from
        device_t m_base;

        /// \brief Object tag to search for
        string m_tag;

        /// \brief Set when object resolution completes
        bool m_resolved;


        // construction/destruction
        //-------------------------------------------------
        //  finder_base - constructor
        //-------------------------------------------------
        protected finder_base(device_t base_device, string tag)
        {
            m_next = base_device.register_auto_finder(this);
            m_base = base_device;
            m_tag = tag;
        }


        protected bool resolved { get { return m_resolved; } set { m_resolved = value; } }


        // getters
        public finder_base next() { return m_next; }
        public abstract bool findit(bool isvalidation = false);

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
        public KeyValuePair<device_t, string> finder_target() { return global.make_pair(m_base, m_tag); }  // std::pair<device_t &, char const *>

        public virtual device_t base_() { return m_base; }
        public virtual string tag() { return m_tag; }


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
        public void set_tag(device_t base_, string tag) { global.assert(!m_resolved); m_base = base_; m_tag = tag; }

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
        public void set_tag(finder_base finder) { global.assert(!m_resolved); var kvp = finder.finder_target(); m_base = kvp.Key; m_tag = kvp.Value; }  //std::tie(m_base, m_tag) = finder.finder_target(); }


        // helpers

        //void *find_memregion(UINT8 width, size_t &length, bool required);

        protected bool validate_memregion(UInt32 bytes, bool required)
        {
            // make sure we can resolve the full path to the region
            UInt32 bytes_found = 0;
            string region_fulltag = m_base.get().subtag(m_tag);

            // look for the region
            foreach (device_t dev in new device_iterator(m_base.get().mconfig().root_device()))
            {
                foreach (romload.region region in new romload.entries(dev.rom_region()).get_regions())
                {
                    if (dev.subtag(region.get_tag()) == region_fulltag)
                    {
                        bytes_found = region.get_length();
                        break;
                    }
                }

                if (bytes_found != 0)
                    break;
            }

            // check the length and warn if other than specified
            if ((bytes_found != 0) && (bytes != 0) && (bytes != bytes_found))
            {
                global.osd_printf_warning("Region '{0}' found but has {1} bytes, not {2} as requested\n", m_tag, bytes_found, bytes);
                bytes_found = 0;
            }

            return report_missing(bytes_found != 0, "memory region", required);
        }

        //void *find_memshare(UINT8 width, size_t &bytes, bool required);  // moved to object_finder_base below

        //-------------------------------------------------
        //  report_missing - report missing objects and
        //  return true if it's ok
        //-------------------------------------------------
        public virtual bool report_missing(bool found, string objname, bool required)
        {
            if (required && m_tag == DUMMY_TAG)
            {
                global.osd_printf_error("Tag not defined for required {0}\n", objname);
                return false;
            }

            // just pass through in the found case
            if (found)
                return true;

            // otherwise, report
            string region_fulltag = m_base.get().subtag(m_tag);
            if (required)
                global.osd_printf_error("Required {0} '{1}' not found\n", objname, region_fulltag);
            else
                global.osd_printf_verbose("Optional {0} '{1}' not found\n", objname, region_fulltag);

            return !required;
        }


        public virtual void printf_warning(string format, params object [] args)
        {
            global.osd_printf_warning(format, args);
        }
    }


    /// \brief Base class for object discovery helpers
    ///
    /// Abstract template base for auto-discovery of objects of a particular
    /// type.  Provides implicit cast-to-pointer and pointer member access
    /// operators.  Template arguments are the type of object to discover,
    /// and whether failure to find the object is considered an error.
    /// Assumes that non-null pointer is found, and null pointer is not
    /// found.
    public abstract class object_finder_base<ObjectClass /*, bool Required>*/> : finder_base where ObjectClass : class
    {
        protected bool Required;

        // internal state
        ObjectClass m_target = null;


        // construction/destruction
        protected object_finder_base(bool required, device_t base_, string tag) : base(base_, tag) { Required = required; }

        // operators to make use transparent
        //operator _ObjectClass *() const { return m_target; }
        //_ObjectClass *operator->() const { assert(m_target != NULL); return m_target; }

        // getters for explicit fetching
        public virtual bool found() { return target != null; }

        /// \brief Clear temporary binding from configuration
        ///
        /// Object finders may allow temporary binding to the anticipated
        /// target during configuration.  This needs to be cleared to ensure
        /// the correct target is found if a device further up the hierarchy
        /// subsequently removes or replaces devices.
        public override void end_configuration() { global.assert(!resolved); m_target = null; }

        // setter for setting the object
        public virtual ObjectClass target { get { return m_target; } set { m_target = value; } }

        protected bool report_missing(string objname) { return report_missing(found(), objname, Required); }
    }


    /// \brief Device finder template
    ///
    /// Template arguments are the device class to find, and whether the
    /// device is required.  It is a validation error if a required device
    /// is not found.  If a device with matching tag is found but the class
    /// does not match, a message is printed at warning level.  This class
    /// is generally not used directly, instead the optional_device and
    /// required_device helpers are used.
    /// \sa optional_device required_device
    //template<class _DeviceClass, bool Required>
    public class device_finder<DeviceClass> : object_finder_base<DeviceClass> where DeviceClass : class
    {
        //using object_finder_base<DeviceClass, Required>::set_tag;


        object m_targetObject;  // some uses of optional_device<> and required_device<> don't actually use devices (eg, dac_byte_interface)


        // construction/destruction
        public device_finder(bool required, device_t basedevice, string tag) : base(required, basedevice, tag) { }

        // make reference use transparent as well
        //operator _DeviceClass &() { assert(object_finder_base<_DeviceClass>::m_target != NULL); return *object_finder_base<_DeviceClass>::m_target; }

        public override DeviceClass target { get { return (m_targetObject is DeviceClass) ? (DeviceClass)m_targetObject : default(DeviceClass); } set { m_targetObject = value; } }


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
        //{
        //    assert(!this->m_resolved);
        //    assert(is_expected_tag(device));
        //    this->m_target = &device;
        //    return device;
        //}

        /// \brief Check that device implementation has expected tag
        /// \param [in] device Reference to device.
        /// \return True if supplied device matches the configured target
        ///   tag, or false otherwise.
        //template <typename T>
        //std::enable_if_t<emu::detail::is_device_implementation<T>::value, bool> is_expected_tag(T const &device) const
        //{
        //    return this->m_base.get().subtag(this->m_tag) == device.tag();
        //}

        /// \brief Check that device mixin has expected tag
        /// \param [in] device Reference to interface/mixin.
        /// \return True if supplied mixin matches the configured target
        ///   tag, or false otherwise.
        //template <typename T>
        //std::enable_if_t<emu::detail::is_device_interface<T>::value, bool> is_expected_tag(T const &interface) const
        //{
        //    return this->m_base.get().subtag(this->m_tag) == interface.device().tag();
        //}

        // finder
        public override bool findit(bool isvalidation = false)
        {
            if (!isvalidation)
            {
                global.assert(!this.resolved);
                this.resolved = true;
            }

            device_t device = base_().get().subdevice(tag());
            m_targetObject = device;
            if (device != null && target == null)
            {
                printf_warning("Device '{0}' found but is of incorrect type (actual type is {1})\n", tag(), device.name());
            }
            return report_missing("device");
        }
    }


    /// \brief Optional device finder
    ///
    /// Finds device with maching type and tag.  If a device with matching
    /// tag is found but the type does not match, a message is printed at
    /// warning level.  No error is generated if a matching device is not
    /// found (the target object pointer will be null).  If you have a
    /// number of similar optional devices, consider using
    /// optional_device_array.
    /// \sa required_device optional_device_array device_finder
    //template <class DeviceClass> using optional_device = device_finder<DeviceClass, false>;
    public class optional_device<DeviceClass> : device_finder<DeviceClass> where DeviceClass : class
    {
        public optional_device(device_t basedevice, string tag) : base(false, basedevice, tag) { }
    }

    /// \brief Required device finder
    ///
    /// Finds device with maching type and tag.  If a device with matching
    /// tag is found but the type does not match, a message is printed at
    /// warning level.  A validation error is generated if a matching device
    /// is not found.  If you have a number of similar required devices,
    /// consider using required_device_array.
    /// \sa optional_device required_device_array device_finder
    //template <class DeviceClass> using required_device = device_finder<DeviceClass, true>;
    public class required_device<DeviceClass> : device_finder<DeviceClass> where DeviceClass : class
    {
        public required_device(device_t basedevice, string tag) : base(true, basedevice, tag) { }
    }


    //template <class DeviceClass, unsigned Count, bool Required> using device_array_finder = object_array_finder<device_finder<DeviceClass, Required>, Count>;
    public class device_array_finder<DeviceClass> : object_array_finder<device_finder<DeviceClass>> where DeviceClass : class
    {
        bool Required;

        protected device_array_finder(int Count, bool Required, device_t basedevice, string format, int start) : base(Count, basedevice, format, start) { this.Required = Required; }
    }

    //template <class DeviceClass, unsigned Count> using optional_device_array = device_array_finder<DeviceClass, Count, false>;
    public class optional_device_array<DeviceClass> : device_array_finder<DeviceClass> where DeviceClass : class
    {
        public optional_device_array(int Count, device_t basedevice, string format, int start) : base(Count, false, basedevice, format, start) { }
    }

    public class optional_device_array_ay8910_device : optional_device_array<ay8910_device>
    {
        public optional_device_array_ay8910_device(int Count, device_t basedevice, string format, int start) : base(Count, basedevice, format, start)
        {
            m_array = new device_finder<ay8910_device>[Count];
            for (int i = 0; i < Count; i++)
                m_array[i] = new device_finder<ay8910_device>(false, basedevice, m_tag[i]);
        }
    }

    public class optional_device_array_i8255_device : optional_device_array<i8255_device>
    {
        public optional_device_array_i8255_device(int Count, device_t basedevice, string format, int start) : base(Count, basedevice, format, start)
        {
            m_array = new device_finder<i8255_device>[Count];
            for (int i = 0; i < Count; i++)
                m_array[i] = new device_finder<i8255_device>(false, basedevice, m_tag[i]);
        }
    }

    //template <class DeviceClass, unsigned Count> using required_device_array = device_array_finder<DeviceClass, Count, true>;
    public class required_device_array<DeviceClass> : device_array_finder<DeviceClass> where DeviceClass : class
    {
        public required_device_array(int Count, device_t basedevice, string format, int start) : base(Count, false, basedevice, format, start) { }
    }


    /// \brief Memory region finder template
    ///
    /// Template argument is whether the memory region is required.  It is a
    /// validation error if a required memory region is not found.  This
    /// class is generally not used directly, instead the
    /// optional_memory_region and required_memory_region helpers are used.
    /// \sa optional_memory_region required_memory_region
    //template <bool Required>
    public class memory_region_finder : object_finder_base<memory_region>  //, Required>
    {
        /// \brief Memory region finder constructor
        /// \param [in] base Base device to search from.
        /// \param [in] tag Memory region tag to search for.  This is not
        ///   copied, it is the caller's responsibility to ensure this
        ///   pointer remains valid until resolution time.
        protected memory_region_finder(bool required, device_t base_device, string tag) : base(required, base_device, tag) { }


        /// \brief Find memory region
        ///
        /// Find memory region with requested tag.  For a dry run, the
        /// target object pointer will not be set.  This method is called by
        /// the base device at resolution time.
        /// \param [in] isvalidation True if this is a dry run (not
        ///   intending to run the machine, just checking for errors).
        /// \return True if the memory region is optional or if a matching
        ///   memory region is found, false otherwise.
        public override bool findit(bool isvalidation)
        {
            if (isvalidation)
                return validate_memregion(0, Required);

            global.assert(!this.resolved);
            this.resolved = true;
            target = base_().get().memregion(tag());
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
    public class optional_memory_region : memory_region_finder
    {
        public optional_memory_region(device_t basedevice, string tag) : base(false, basedevice, tag) { }
    }

    /// \brief Required memory region finder
    ///
    /// Finds memory region with maching tag.  A validation error is
    /// generated if a matching memory region is not found.  If you have a
    /// number of similar required memory regions, consider using
    /// required_memory_region_array.
    /// \sa optional_memory_region required_memory_region_array
    ///   memory_region_finder
    //using required_memory_region = memory_region_finder<true>;
    public class required_memory_region : memory_region_finder
    {
        public required_memory_region(device_t basedevice, string tag) : base(true, basedevice, tag) { }
    }


    /// \brief I/O port finder template
    ///
    /// Template argument is whether the I/O port is required.  It is a
    /// validation error if a required I/O port is not found.  This class is
    /// generally not used directly, instead the optional_ioport and
    /// required_ioport helpers are used.
    /// \sa optional_ioport required_ioport
    //template <bool Required>
    public class ioport_finder : object_finder_base<ioport_port>
    {
        /// \brief I/O port finder constructor
        /// \param [in] base Base device to search from.
        /// \param [in] tag I/O port tag to search for.  This is not copied,
        ///   it is the caller's responsibility to ensure this pointer
        ///   remains valid until resolution time.
        public ioport_finder(bool Required, device_t basedevice, string tag) : base(Required, basedevice, tag) { }


        /// \brief Read I/O port if found or return default value
        ///
        /// If the I/O port was found, this reads a value from the I/O port
        /// and returns it.  If the I/O port was not found, the default
        /// value (supplied as a parameter) is returned.
        /// \param [in] defval Value to return if I/O port was not found.
        /// \return Value read from I/O port if found, or supplied default
        ///   value otherwise.
        //ioport_value read_safe(ioport_value defval) { return this->m_target ? this->m_target->read() : defval; }


        /// \brief Find I/O port
        ///
        /// Find I/O port with requested tag.  Just returns true for a dry
        /// run.  This method is called by the base device at resolution
        /// time.
        /// \param [in] isvalidation True if this is a dry run (not
        ///   intending to run the machine, just checking for errors).
        /// \return True if the I/O port is optional, a matching I/O port is
        ///   is found or this is a dry run, false otherwise.
        public override bool findit(bool isvalidation)
        {
            if (isvalidation)
                return true;

            global.assert(!this.resolved);
            this.resolved = true;
            target = base_().get().ioport(tag());
            return report_missing("I/O port");
        }
    }


    /// \brief Optional I/O port finder
    ///
    /// Finds I/O port with maching tag.  No error is generated if a
    /// matching I/O port is not found (the target object pointer will be
    /// null).  If you have a number of similar optional I/O ports, consider
    /// using optional_ioport_array.
    /// \sa required_ioport optional_ioport_array ioport_finder
    //using optional_ioport = ioport_finder<false>;
    public class optional_ioport : ioport_finder
    {
        public optional_ioport(device_t basedevice, string tag) : base(false, basedevice, tag) { }
    }

    /// \brief Required I/O port finder
    ///
    /// Finds I/O port with maching tag.  A validation error is generated if
    /// a matching I/O port is not found.  If you have a number of similar
    /// required I/O ports, consider using required_ioport_array.
    /// \sa optional_ioport required_ioport_array ioport_finder
    //using required_ioport = ioport_finder<true>;
    public class required_ioport : ioport_finder
    {
        public required_ioport(device_t basedevice, string tag) : base(true, basedevice, tag) { }
    }


    //template <unsigned Count, bool Required> using ioport_array_finder = object_array_finder<ioport_finder<Required>, Count>;
    public class ioport_array_finder : object_array_finder<ioport_finder>
    {
        bool Required;

        protected ioport_array_finder(int Count, bool Required, device_t basedevice, string format, int start) : base(Count, basedevice, format, start)
        {
            this.Required = Required;

            m_array = new ioport_finder[Count];
            for (int i = 0; i < Count; i++)
                m_array[i] = new ioport_finder(Required, basedevice, m_tag[i]);
        }
    }

    //template <unsigned Count> using optional_ioport_array = ioport_array_finder<Count, false>;
    public class optional_ioport_array : ioport_array_finder
    {
        public optional_ioport_array(int Count, device_t basedevice, string format, int start) : base(Count, false, basedevice, format, start) { }
    }

    //template <unsigned Count> using required_ioport_array = ioport_array_finder<Count, true>;
    public class required_ioport_array : ioport_array_finder
    {
        public required_ioport_array(int Count, device_t basedevice, string format, int start) : base(Count, true, basedevice, format, start) { }
    }


    /// \brief Memory region base pointer finder
    ///
    /// Template arguments are the element type of the memory region and
    /// whether the memory region is required.  It is a validation error if
    /// a required memory region is not found.  This class is generally not
    /// used directly, instead the optional_region_ptr and
    /// required_region_ptr helpers are used.
    /// \sa optional_region_ptr required_region_ptr
    //template<typename PointerType, bool Required>
    class region_ptr_finder<PointerType/*, bool Required*/> : object_finder_base<ListPointer<PointerType>>
    {
        bool Required;


        // internal state

        /// \brief Desired region length
        ///
        /// Desired region length in units of elements.
        UInt32 m_desired_length;  //size_t const m_desired_length;

        /// \brief Matched region length
        ///
        /// Actual length of the region that was found in units of
        /// elements, or zero if no matching region has been found.
        UInt32 m_length;  //size_t m_length;


        // construction/destruction
        public region_ptr_finder(bool required, device_t basedevice, string tag, UInt32 length = 0)
            : base(required, basedevice, tag)
        {
            Required = required;

            m_length = length;
        }


        // operators to make use transparent
        //PointerType operator[](int index) const { assert(index < m_length); return this->m_target[index]; }
        //PointerType &operator[](int index) { assert(index < m_length); return this->m_target[index]; }


        // setter for setting the object and its length
        //void set_target(PointerType *target, size_t length) { this->m_target = target; m_length = length; }


        // getter for explicit fetching
        //UINT32 length() const { return m_length; }
        //UINT32 bytes() const { return m_length * sizeof(PointerType); }
        //UINT32 mask() const { return m_length - 1; } // only valid if length is known to be a power of 2


        // finder
        public override bool findit(bool isvalidation = false)
        {
            if (isvalidation)
                return validate_memregion((UInt32)global.sizeof_(typeof(PointerType)) * m_length, Required);

            global.assert(!this.resolved);
            this.resolved = true;
            m_length = m_desired_length;
            target = find_memregion((byte)global.sizeof_(typeof(PointerType)), ref m_length, Required);  //reinterpret_cast<PointerType *>(find_memregion(sizeof(PointerType), m_length, m_required));
            return report_missing(target != null, "memory region", Required);
        }


        public virtual ListPointer<PointerType> find_memregion(byte width, ref UInt32 length, bool required) { return null; }
    }


    // optional region pointer finder
    //template<class PointerType>
    class optional_region_ptr<PointerType> : region_ptr_finder<PointerType/*, false*/>
    {
        public optional_region_ptr(device_t basedevice, string tag, UInt32 length = 0) : base(false, basedevice, tag, length) { }
    }


    class optional_region_ptr_byte : optional_region_ptr<byte/*, false*/>
    {
        public optional_region_ptr_byte(device_t basedevice, string tag, UInt32 length = 0) : base(basedevice, tag, length) { }


        //-------------------------------------------------
        //  find_memregion - find memory region
        //-------------------------------------------------
        public override ListBytesPointer find_memregion(byte width, ref UInt32 length, bool required)
        {
            // look up the region and return nullptr if not found
            memory_region region = base_().memregion(tag());
            if (region == null)
            {
                length = 0;
                return null;
            }

            // check the width and warn if not correct
            if (region.bytewidth() != width)
            {
                if (required)
                    global.osd_printf_warning("Region '{0}' found but is width {1}, not {2} as requested\n", tag(), region.bitwidth(), width*8);
                length = 0;
                return null;
            }

            // check the length and warn if other than specified
            UInt32 length_found = region.bytes() / width;
            if (length != 0 && length != length_found)
            {
                if (required)
                    global.osd_printf_warning("Region '{0}' found but has {1} bytes, not {2} as requested\n", tag(), region.bytes(), (int)length*width);
                length = 0;
                return null;
            }

            // return results
            length = length_found;
            return new ListBytesPointer(region.base_());
        }
    }


    // required region pointer finder
    //template<class PointerType>
    class required_region_ptr<PointerType> : region_ptr_finder<PointerType/*, true*/>
    {
        public required_region_ptr(device_t basedevice, string tag, UInt32 length = 0) : base(true, basedevice, tag, length) { }
    }


    // ======================> shared_ptr_finder
    // shared pointer finder template
    //template<typename PointerType, bool Required>
    public class shared_ptr_finder<PointerType/*, bool Required*/> : object_finder_base<ListPointer<PointerType>>
    {
        bool Required;

        // internal state
        u8 m_width;
        UInt32 m_bytes;
        //std::vector<PointerType> m_allocated;

        ListPointer<PointerType> m_targetPtr;


        // construction/destruction
        public shared_ptr_finder(bool required, device_t basedevice, string tag, u8 width = 255) // = sizeof(PointerType) * 8)
            : base(required, basedevice, tag)
        {
            Required = required;
            if (width == 255)
                width = (u8)(global.sizeof_(typeof(PointerType)) * 8);

            m_width = width;
            m_bytes = 0;
        }

        // operators to make use transparent
        //PointerType operator[](int index) const { return this->m_target[index]; }
        //PointerType &operator[](int index) { return this->m_target[index]; }

        // getter for explicit fetching
        //UINT32 bytes() const { return m_bytes; }
        //public UInt32 bytes() { return (UInt32)target().m_buffer.count(); }
        public UInt32 bytes() { return m_bytes; }
        //UINT32 mask() const { return m_bytes - 1; } // FIXME: wrong when sizeof(PointerType) != 1

        // setter for setting the object
        //void set_target(PointerType *target, size_t bytes) { this->m_target = target; m_bytes = bytes; }


        public override ListPointer<PointerType> target { get { return m_targetPtr; } set { m_targetPtr = value; } }


        // dynamic allocation of a shared pointer
        //void allocate(UINT32 entries)


        public virtual ListPointer<PointerType> find_memshare(byte width, out UInt32 bytes, bool required) { bytes = 0; return null; }

        // finder
        public override bool findit(bool isvalidation = false)
        {
            if (isvalidation)
                return true;

            global.assert(!this.resolved);
            this.resolved = true;
            target = find_memshare(0, out m_bytes, Required);  // reinterpret_cast<_PointerType *>(this.find_memshare(m_width, m_bytes, _Required));
            return report_missing(this.target != null, "shared pointer", Required);
        }
    }

    // optional shared pointer finder
    //template<class PointerType>
    public class optional_shared_ptr<PointerType> : shared_ptr_finder<PointerType/*, false*/>
    {
        public optional_shared_ptr(device_t basedevice, string tag, u8 width = 255)//, UINT8 width = sizeof(PointerType) * 8)
            : base(false, basedevice, tag, width) { }
    }

    public class optional_shared_ptr_byte : optional_shared_ptr<byte/*, false*/>
    {
        public optional_shared_ptr_byte(device_t basedevice, string tag, u8 width = 255)//, UINT8 width = sizeof(PointerType) * 8)
            : base(basedevice, tag, width) { }

        //-------------------------------------------------
        //  find_memshare - find memory share
        //-------------------------------------------------
        public override ListBytesPointer find_memshare(u8 width, out UInt32 bytes, bool required)
        {
            bytes = 0;

            // look up the share and return NULL if not found
            memory_share share = base_().memshare(tag());
            if (share == null)
                return null;

            // check the width and warn if not correct
            if (width != 0 && share.bitwidth() != width)
            {
                if (required)
                    global.osd_printf_warning("Shared ptr '{0}' found but is width {1}, not {2} as requested\n", tag(), share.bitwidth(), width);
                return null;
            }

            // return results
            bytes = (UInt32)share.bytes();
            return share.ptr();
        }

        public byte this[int i] { get { return target[i]; } set { target[i] = value; } }
        public byte this[UInt32 i] { get { return target[i]; } set { target[i] = value; } }
    }

    // required shared pointer finder
    //template<class PointerType>
    class required_shared_ptr<PointerType> : shared_ptr_finder<PointerType/*, true*/>
    {
        public required_shared_ptr(device_t basedevice, string tag, u8 width = 255)  //, UINT8 width = sizeof(PointerType) * 8)
            : base(true, basedevice, tag, width) { }
    }

    class required_shared_ptr_byte : required_shared_ptr<u8/*, true*/>
    {
        public required_shared_ptr_byte(device_t basedevice, string tag, u8 width = 255)  //, UINT8 width = sizeof(PointerType) * 8)
            : base(basedevice, tag, width) { }

        //-------------------------------------------------
        //  find_memshare - find memory share
        //-------------------------------------------------
        public override ListBytesPointer find_memshare(u8 width, out UInt32 bytes, bool required)
        {
            bytes = 0;

            // look up the share and return NULL if not found
            memory_share share = base_().memshare(tag());
            if (share == null)
                return null;

            // check the width and warn if not correct
            if (width != 0 && share.bitwidth() != width)
            {
                if (required)
                    global.osd_printf_warning("Shared ptr '{0}' found but is width {1}, not {2} as requested\n", tag(), share.bitwidth(), width);
                return null;
            }

            // return results
            bytes = (UInt32)share.bytes();
            return share.ptr();
        }

        public byte this[int i] { get { return target[i]; } set { target[i] = value; } }
        public byte this[UInt32 i] { get { return target[i]; } set { target[i] = value; } }
    }
}
