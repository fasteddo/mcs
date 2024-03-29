// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Linq;

using default_layout_map = mame.std.map<string, mame.internal_layout>;  //std::map<char const *, internal_layout const *, bool (*)(char const *, char const *)> default_layout_map;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using maximum_quantum_map = mame.std.map<string, mame.attotime>;  //std::map<char const *, attotime, bool (*)(char const *, char const *)> maximum_quantum_map;
using size_t = System.UInt64;
using slot_interface_enumerator = mame.device_interface_enumerator<mame.device_slot_interface>;  //typedef device_interface_enumerator<device_slot_interface> slot_interface_enumerator;
using u8 = System.Byte;
using u32 = System.UInt32;

using static mame.cpp_global;
using static mame.osdcore_global;


namespace mame
{
    namespace emu.detail
    {
        public class machine_config_replace
        {
            //friend class ::machine_config;


            public machine_config config;


            //machine_config_replace(machine_config_replace const &) = default;

            public machine_config_replace(machine_config c) { config = c; }
        }
    } // namespace emu::detail


    /// \brief Internal layout description
    ///
    /// Holds the compressed and decompressed data size, compression method,
    /// and a reference to the compressed layout data.  Note that copying
    /// the structure will not copy the referenced data.
    public class internal_layout
    {
        public enum compression { NONE, ZLIB }

        public int decompressed_size;
        public int compressed_size;
        public compression compression_type;
        public u8 [] data;

        public internal_layout(int decompressed_size, int compressed_size, compression compression_type, u8 [] data) { this.decompressed_size = decompressed_size; this.compressed_size = compressed_size; this.compression_type = compression_type; this.data = data; }
    }


    public class machine_config
    {
        //class current_device_stack;
        //typedef std::map<char const *, internal_layout const *, bool (*)(char const *, char const *)> default_layout_map;
        //typedef std::map<char const *, attotime, bool (*)(char const *, char const *)> maximum_quantum_map;


        public class token : IDisposable
        {
            machine_config m_host;
            device_t m_device;


            public token(machine_config host, device_t device)
            {
                m_host = host;
                m_device = device;


                assert(m_device == m_host.m_current_device);
            }

            token(token that)
            {
                m_host = that.m_host;
                m_device = that.m_device;


                that.m_device = null;
                assert(m_device == null || (m_device == m_host.m_current_device));
            }

            //token(token const &) = delete;
            //token &operator=(token &&) = delete;
            //token &operator=(token const &) = delete;

            ~token()
            {
                assert(m_isDisposed);  // can remove
            }

            bool m_isDisposed = false;
            public void Dispose()
            {
                if (m_device != null)
                {
                    assert(m_device == m_host.m_current_device);
                    m_host.m_current_device = null;
                    m_device = null;
                }

                m_isDisposed = true;
            }
        }


        class current_device_stack : IDisposable
        {
            machine_config m_host;
            device_t m_device;

            //current_device_stack(current_device_stack const &) = delete;
            public current_device_stack(machine_config host)
            {
                m_host = host;
                m_device = host.m_current_device;


                m_host.m_current_device = null;
            }
            //~current_device_stack() { global.assert(!m_host.m_current_device); m_host.m_current_device = m_device; }

            public void Dispose()
            {
                assert(m_host.m_current_device == null);

                m_host.m_current_device = m_device;
            }
        }


        // public state
        attotime m_minimum_quantum;          // minimum scheduling quantum
        string m_perfect_cpu_quantum;      // tag of CPU to use for "perfect" scheduling

        // internal state
        game_driver m_gamedrv;
        emu_options m_options;
        device_t m_root_device;
        default_layout_map m_default_layouts;
        device_t m_current_device;
        maximum_quantum_map m_maximum_quantums;
        std.pair<device_t, string> m_perfect_quantum_device;


        //-------------------------------------------------
        //  machine_config - constructor
        //-------------------------------------------------
        public machine_config(game_driver gamedrv, emu_options options)
        {
            m_gamedrv = gamedrv;
            m_options = options;
            m_root_device = null;
            m_default_layouts = new default_layout_map();  //([] (char const *a, char const *b) { return 0 > std::strcmp(a, b); })
            m_current_device = null;
            m_maximum_quantums = new maximum_quantum_map();  //m_maximum_quantums([] (char const *a, char const *b) { return 0 > std::strcmp(a, b); });
            m_perfect_quantum_device = new std.pair<device_t, string>(null, "");


            // add the root device
            device_add("root", gamedrv.type, 0);

            // intialize slot devices - make sure that any required devices have been allocated
            foreach (device_slot_interface slot in new slot_interface_enumerator(root_device()))
            {
                device_t owner = slot.device();
                string slot_option_name = owner.tag()[1..];  // + 1;

                // figure out which device goes into this slot
                bool has_option = options.has_slot_option(slot_option_name);
                string selval;
                bool is_default;
                if (!has_option)
                {
                    // The only time we should be getting here is when emuopts.cpp is invoking
                    // us to evaluate slot/image options, and the internal state of emuopts.cpp has
                    // not caught up yet
                    selval = slot.default_option();
                    is_default = true;
                }
                else
                {
                    slot_option opt = options.slot_option(slot_option_name);
                    selval = opt.value();
                    is_default = !opt.specified();
                }

                if (!string.IsNullOrEmpty(selval))
                {
                    // TODO: make this thing more self-contained so it can apply itself - shouldn't need to know all this here
                    device_slot_interface.slot_option option = slot.option(selval);

                    if ((option != null) && (is_default || option.selectable()))
                    {
                        // create the device
                        using token tok = begin_configuration(owner);

                        device_t new_dev = device_add(option.name(), option.devtype(), option.clock());
                        slot.set_card_device(new_dev);

                        string default_bios = option.default_bios();
                        if (!string.IsNullOrEmpty(default_bios))
                            new_dev.set_default_bios_tag(default_bios);

                        var additions = option.machine_config();
                        if (additions != null)
                            additions(this, new_dev);

                        input_device_default [] input_device_defaults = option.input_device_defaults();
                        if (input_device_defaults != null)
                            new_dev.set_input_default(input_device_defaults);
                    }
                    else
                    {
                        throw new emu_fatalerror("Unknown slot option '{0}' in slot '{1}'", selval, owner.tag()[1..]);
                    }
                }
            }

            // then notify all devices that their configuration is complete
            foreach (device_t device in new device_enumerator(root_device()))
            {
                if (!device.configured())
                    device.config_complete();
            }
        }


        // getters
        public string perfect_cpu_quantum() { return m_perfect_cpu_quantum; }
        public game_driver gamedrv() { return m_gamedrv; }
        public device_t root_device() { assert(m_root_device != null); return m_root_device; }
        public device_t current_device() { assert(m_current_device != null); return m_current_device; }
        public emu_options options() { return m_options; }
        device_t device(string tag) { return root_device().subdevice(tag); }
        //template<class DeviceClass> inline DeviceClass *device(const char *tag) const { return downcast<DeviceClass *>(device(tag)); }


        public attotime maximum_quantum(attotime default_quantum)
        {
            //return std::accumulate(
            //        m_maximum_quantums.begin(),
            //        m_maximum_quantums.end(),
            //        default_quantum,
            //        [] (attotime const &lhs, maximum_quantum_map::value_type const &rhs) { return (std::min)(lhs, rhs.second); });
            return Enumerable.Aggregate(m_maximum_quantums, default_quantum, (current, next) => { return attotime.Min(current, next.second()); });
        }


        public device_execute_interface perfect_quantum_device()
        {
            if (m_perfect_quantum_device.first == null)
                return null;

            device_t found = m_perfect_quantum_device.first.subdevice(m_perfect_quantum_device.second);
            if (found == null)
            {
                throw new emu_fatalerror(
                        "Device {0} relative to {1} specified for perfect interleave is not present!\n",
                        m_perfect_quantum_device.second,
                        m_perfect_quantum_device.first.tag());
            }

            device_execute_interface result;
            if (!found.interface_(out result))
            {
                throw new emu_fatalerror("Device {0} ({1}) specified for perfect interleave does not implement device_execute_interface!\n",
                        found.tag(),
                        found.shortname());
            }

            return result;
        }


        public delegate void apply_default_layouts_func(device_t device, internal_layout layout);

        /// \brief Apply visitor to internal layouts
        ///
        /// Calls the supplied visitor for each device with an internal
        /// layout.  The order of devices is implementation-dependent.
        /// \param [in] op The visitor.  It must provide a function call
        //    operator that can be invoked with two arguments: a reference
        //    to a #device_t and a const reference to an #internal_layout.
        //template <typename T>
        public void apply_default_layouts(apply_default_layouts_func op)  //T &&op) const
        {
            foreach (var lay in m_default_layouts)  //for (std::pair<char const *, internal_layout const *> const &lay : m_default_layouts)
                op(device(lay.first()), lay.second());
        }


        /// \brief Get a device replacement helper
        ///
        /// Pass the result in place of the machine configuration itself to
        /// replace an existing device.
        /// \return A device replacement helper to pass to a device type
        ///   when replacing an existing device.
        public emu.detail.machine_config_replace replace() { return new emu.detail.machine_config_replace(this); }


        /// \brief Set internal layout for current device
        ///
        /// Set internal layout for current device.  Each device in the
        /// system can have its own internal layout.  Tags in the layout
        /// will be resolved relative to the device.  Replaces previously
        /// set layout if any.
        /// \param [in] layout Reference to the internal layout description
        ///   structure.  Neither the description structure nor the
        ///   compressed data is copied.  It is the caller's responsibility
        ///   to ensure both remain valid until layouts and views are
        ///   instantiated.
        //void set_default_layout(internal_layout const &layout);


        /// \brief Set maximum scheduling quantum
        ///
        /// Set the maximum scheduling quantum required for the current
        /// device.  The smallest maximum quantum requested by a device in
        /// the system will be used.
        /// \param [in] quantum Maximum scheduling quantum in attoseconds.
        public void set_maximum_quantum(attotime quantum)
        {
            bool ins = m_maximum_quantums.emplace(current_device().tag(), quantum);  //std::pair<maximum_quantum_map::iterator, bool> const ins(m_maximum_quantums.emplace(current_device().tag(), quantum));
            if (!ins)  //if (!ins.second)
                m_maximum_quantums[current_device().tag()] = quantum;  //ins.first->second = quantum;
        }


        //template <typename T>
        //void set_perfect_quantum(T &&tag)
        //{
        //    set_perfect_quantum(current_device(), std::forward<T>(tag));
        //}
        //template <class DeviceClass, bool Required>
        //void set_perfect_quantum(device_finder<DeviceClass, Required> const &finder)
        //{
        //    std::pair<device_t &, char const *> const target(finder.finder_target());
        //    set_perfect_quantum(target.first, target.second);
        //}
        //template <class DeviceClass, bool Required>
        //void set_perfect_quantum(device_finder<DeviceClass, Required> &finder)
        //{
        //    set_perfect_quantum(const_cast<device_finder<DeviceClass, Required> const &>(finder));
        //}


        // helpers during configuration; not for general use

        public token begin_configuration(device_t device)
        {
            assert(m_current_device == null);
            m_current_device = device;
            return new token(this, device);
        }


        //-------------------------------------------------
        //  device_add - configuration helper to add a
        //  new device
        //-------------------------------------------------
        public device_t device_add(string tag, device_type type, u32 clock)
        {
            std.pair<string, device_t> owner = resolve_owner(tag);
            return add_device(type.create(this, owner.first, owner.second, clock), owner.second);
        }

        //template <typename Creator>
        //device_t *device_add(const char *tag, Creator &&type, u32 clock)
        //{
        //    return device_add(tag, device_type(type), clock);
        //}
        //template <typename Creator, typename... Params>

        //auto device_add(const char *tag, Creator &&type, Params &&... args)
        //{
        //    std::pair<const char *, device_t *> const owner(resolve_owner(tag));
        //    auto device(type.create(*this, owner.first, owner.second, std::forward<Params>(args)...));
        //    auto &result(*device);
        //    assert(type.type() == typeid(result));
        //    add_device(std::move(device), owner.second);
        //    return &result;
        //}

        //template <typename Creator, typename... Params>
        //auto device_add(const char *tag, Creator &&type, XTAL clock, Params &&... args)
        //{
        //    clock.validate(std::string("Instantiating device ") + tag);
        //    return device_add(tag, std::forward<Creator>(type), clock.value(), std::forward<Params>(args)...);
        //}


        //device_t *device_replace(const char *tag, device_type type, u32 clock);
        public device_t device_replace(string tag, device_type type, u32 clock)
        {
            var existing = prepare_replace(tag);  //std::tuple<const char *, device_t *, device_t *> const existing(prepare_replace(tag));
            var device = type.create(this, existing.Item1, existing.Item2, clock);  //std::unique_ptr<device_t> device(type.create(*this, std::get<0>(existing), std::get<1>(existing), clock));
            return replace_device(device, existing.Item2, existing.Item3);  //return &replace_device(std::move(device), *std::get<1>(existing), std::get<2>(existing));
        }

        //template <typename Creator>
        //device_t *device_replace(const char *tag, Creator &&type, u32 clock)
        //{
        //    return device_replace(tag, device_type(type), clock);
        //}
        //template <typename Creator, typename... Params>
        //auto device_replace(const char *tag, Creator &&type, Params &&... args)
        //{
        //    std::tuple<const char *, device_t *, device_t *> const existing(prepare_replace(tag));
        //    auto device(type.create(*this, std::get<0>(existing), std::get<1>(existing), std::forward<Params>(args)...));
        //    auto &result(*device);
        //    assert(type.type() == typeid(result));
        //    replace_device(std::move(device), *std::get<1>(existing), std::get<2>(existing));
        //    return &result;
        //}
        //template <typename Creator, typename... Params>
        //auto device_replace(const char *tag, Creator &&type, XTAL clock, Params &&... args)
        //{
        //    clock.validate(std::string("Replacing device ") + tag);
        //    return device_replace(tag, std::forward<Creator>(type), clock.value(), std::forward<Params>(args)...);
        //}


        //-------------------------------------------------
        //  device_remove - configuration helper to
        //  remove a device
        //-------------------------------------------------
        public device_t device_remove(string tag)
        {
            // find the original device by relative tag (must exist)
            assert(m_current_device != null);
            device_t device = m_current_device.subdevice(tag);
            if (device == null)
            {
                osd_printf_warning("Warning: attempting to remove non-existent device '{0}'\n", tag);
            }
            else
            {
                // make sure we have the old device's actual owner
                device_t owner = device.owner();
                assert(owner != null);

                // remove references to the old device
                remove_references(device);

                // let the device's owner do the work
                owner.subdevices().remove(device);
            }

            return null;
        }


        // internal helpers

        //-------------------------------------------------
        //  resolve_owner - get the actual owner and base
        //  tag given tag relative to current context
        //-------------------------------------------------
        std.pair<string, device_t> resolve_owner(string tag)
        {
            //throw new emu_unimplemented();
#if false
            global.assert(m_current_device == m_root_device);
#endif

            string orig_tag = tag;

            device_t owner = m_current_device;

            // if the device path is absolute, start from the root
            if (string.IsNullOrEmpty(tag) || (':' == tag[0]) || ('^' == tag[0]))  //if (!*tag || (':' == *tag) || ('^' == *tag))
                throw new emu_fatalerror("Attempting to add device with tag containing parent references '{0}'\n", orig_tag);

            // go down the path until we're done with it
            int nextIdx;  //char const *next;
            while ((nextIdx = tag.IndexOf(':')) != -1)  //while ((next = strchr(tag, ':')) != nullptr)
            {
                assert(nextIdx != 0);  //assert(next != tag);
                string part = tag[..nextIdx];  //std::string_view part(tag, next - tag);
                owner = owner.subdevices().find(part);
                if (owner == null)
                    throw new emu_fatalerror("Could not find '{0}' when looking up path for device '{1}'\n", part, orig_tag);
                tag = tag[(nextIdx + 1)..];  //tag = next + 1;
                if ('^' == tag[0])  //if ('^' == *tag)
                    throw new emu_fatalerror("Attempting to add device with tag containing parent references '{0}'\n", orig_tag);
            }
            assert(!string.IsNullOrEmpty(tag) && tag[0] != '\0');  //assert(*tag != '\0');

            return std.make_pair(tag, owner);
        }


        (string, device_t, device_t) prepare_replace(string tag)  //std::tuple<const char *, device_t *, device_t *> prepare_replace(const char *tag);
        {
            // make sure we have the old device's actual owner
            std.pair<string, device_t> owner = resolve_owner(tag);  //std::pair<const char *, device_t *> const owner(resolve_owner(tag));
            assert(owner.second != null);

            // remove references to the old device
            device_t old_device = owner.second.subdevice(owner.first);
            if (old_device != null)
                remove_references(old_device);
            else
                throw new emu_fatalerror("Attempting to replace non-existent device '{0}'\n", tag);

            return (owner.first, owner.second, old_device);
        }


        //-------------------------------------------------
        //  add_device - add a new device at the correct
        //  point in the hierarchy
        //-------------------------------------------------
        device_t add_device(device_t device, device_t owner)
        {
            using current_device_stack context = new current_device_stack(this);  //current_device_stack context = this;

            if (owner != null)
            {
                // allocate the new device and append it to the owner's list
                device_t result = owner.subdevices().append(device);
                result.add_machine_configuration(this);
                return result;
            }
            else
            {
                // allocate the root device directly
                assert(m_root_device == null);
                m_root_device = device;
                m_root_device.add_machine_configuration(this);
                return m_root_device;
            }
        }


        //-------------------------------------------------
        //  replace_device - substitute the new device for
        //  the old one in the owner's list
        //-------------------------------------------------
        device_t replace_device(device_t device, device_t owner, device_t existing)
        {
            using current_device_stack context = new current_device_stack(this);  //current_device_stack const context(*this);

            device_t result = existing != null
                    ? owner.subdevices().replace_and_remove(device, existing)
                    : owner.subdevices().append(device);
            result.add_machine_configuration(this);
            return result;
        }


        //-------------------------------------------------
        //  remove_references - globally remove references
        //  to a device about to be removed from the tree
        //-------------------------------------------------
        void remove_references(device_t device)
        {
            // sanity check
            if (m_perfect_quantum_device.first == device)
            {
                throw new emu_fatalerror(
                        "Removing {0} device {1} would make the perfect quantum device target invalid\n",
                        device.shortname(),
                        device.tag());
            }

            // remove default layouts and maximum quantum settings for subdevices
            string tag = device.tag();
            size_t taglen = std.strlen(tag);
            //for (auto it = m_default_layouts.lower_bound(tag); (m_default_layouts.end() != it) && !std::strncmp(tag, it->first, taglen); )
            //{
            //    if (!it->first[taglen] || (':' == it->first[taglen]))
            //        it = m_default_layouts.erase(it);
            //    else
            //        ++it;
            //}
            List<string> default_layouts_to_erase = new List<string>();
            foreach (var it in m_default_layouts)  //for (auto it = m_default_layouts.lower_bound(tag); (m_default_layouts.end() != it) && !std::strncmp(tag, it->first, taglen); )
            {
                if (std.strncmp(tag, it.Key, taglen) != 0)  //std::strncmp(tag, it->first, taglen)
                    if (it.Key.Length < (int)taglen || (':' == it.Key[(int)taglen]))  //if (!it->first[taglen] || (':' == it->first[taglen]))
                        default_layouts_to_erase.Add(it.Key);  //it = m_default_layouts.erase(it);
            }
            foreach (var key in default_layouts_to_erase)
                m_default_layouts.erase(key);  //it = m_default_layouts.erase(it);

            //for (auto it = m_maximum_quantums.lower_bound(tag); (m_maximum_quantums.end() != it) && !std::strncmp(tag, it->first, taglen); )
            //{
            //    if (!it->first[taglen] || (':' == it->first[taglen]))
            //        it = m_maximum_quantums.erase(it);
            //    else
            //        ++it;
            //}
            List<string> maximum_quantums_to_erase = new List<string>();
            foreach (var it in m_maximum_quantums)  //for (auto it = m_maximum_quantums.lower_bound(tag); (m_maximum_quantums.end() != it) && !std::strncmp(tag, it->first, taglen); )
            {
                if (std.strncmp(tag, it.Key, taglen) != 0)  //std::strncmp(tag, it->first, taglen)
                    if (it.Key.Length < (int)taglen || (':' == it.Key[(int)taglen]))  //if (!it->first[taglen] || (':' == it->first[taglen]))
                        default_layouts_to_erase.Add(it.Key);  //it = m_maximum_quantums.erase(it);
            }
            foreach (var key in maximum_quantums_to_erase)
                m_maximum_quantums.erase(key);  //it = m_maximum_quantums.erase(it);
        }


        //void set_perfect_quantum(device_t &device, std::string tag);
    }
}
