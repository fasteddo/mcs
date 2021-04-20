// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    public static class romload_global
    {
        public const UInt32 TEMPBUFFER_MAX_SIZE     = 1024 * 1024 * 1024;


        //template <typename T> inline std::enable_if_t<!std::is_pointer<T>::value, T const &> ROMENTRY_UNWRAP(T const &r) { return r; }
        //template <typename T> inline T const &ROMENTRY_UNWRAP(T const *r) { return *r; }


        /* ----- per-entry macros ----- */
        public static UInt32 ROMENTRY_GETTYPE(rom_entry_interface r) { return r.get_flags() & romentry_global.ROMENTRY_TYPEMASK; }
        //#define ROMENTRY_ISSPECIAL(r)       (ROMENTRY_GETTYPE(r) != ROMENTRYTYPE_ROM)
        public static bool ROMENTRY_ISFILE(rom_entry_interface r) { return ROMENTRY_GETTYPE(r) == romentry_global.ROMENTRYTYPE_ROM; }
        public static bool ROMENTRY_ISREGION(rom_entry_interface r) { return ROMENTRY_GETTYPE(r) == romentry_global.ROMENTRYTYPE_REGION; }
        public static bool ROMENTRY_ISEND(rom_entry_interface r) { return ROMENTRY_GETTYPE(r) == romentry_global.ROMENTRYTYPE_END; }
        public static bool ROMENTRY_ISRELOAD(rom_entry_interface r) { return ROMENTRY_GETTYPE(r) == romentry_global.ROMENTRYTYPE_RELOAD; }
        public static bool ROMENTRY_ISCONTINUE(rom_entry_interface r) { return ROMENTRY_GETTYPE(r) == romentry_global.ROMENTRYTYPE_CONTINUE; }
        public static bool ROMENTRY_ISFILL(rom_entry_interface r) { return ROMENTRY_GETTYPE(r) == romentry_global.ROMENTRYTYPE_FILL; }
        public static bool ROMENTRY_ISCOPY(rom_entry_interface r) { return ROMENTRY_GETTYPE(r) == romentry_global.ROMENTRYTYPE_COPY; }
        public static bool ROMENTRY_ISIGNORE(rom_entry_interface r) { return ROMENTRY_GETTYPE(r) == romentry_global.ROMENTRYTYPE_IGNORE; }
        public static bool ROMENTRY_ISSYSTEM_BIOS(rom_entry_interface r) { return ROMENTRY_GETTYPE(r) == romentry_global.ROMENTRYTYPE_SYSTEM_BIOS; }
        public static bool ROMENTRY_ISDEFAULT_BIOS(rom_entry_interface r) { return ROMENTRY_GETTYPE(r) == romentry_global.ROMENTRYTYPE_DEFAULT_BIOS; }
        public static bool ROMENTRY_ISPARAMETER(rom_entry_interface r) {return ROMENTRY_GETTYPE(r) == romentry_global.ROMENTRYTYPE_PARAMETER; }
        public static bool ROMENTRY_ISREGIONEND(rom_entry_interface r) { return ROMENTRY_ISREGION(r) || ROMENTRY_ISPARAMETER(r) || ROMENTRY_ISEND(r); }

        /* ----- per-region macros ----- */
        //#define ROMREGION_GETTAG(r)         ((r)->_name)
        public static UInt32 ROMREGION_GETLENGTH(rom_entry_interface r) { return r.get_length(); }
        public static UInt32 ROMREGION_GETFLAGS(rom_entry_interface r) { return r.get_flags(); }
        public static UInt32 ROMREGION_GETWIDTH(rom_entry_interface r) { return 8u << (int)((ROMREGION_GETFLAGS(r) & romentry_global.ROMREGION_WIDTHMASK) >> 8); }
        //#define ROMREGION_ISLITTLEENDIAN(r) ((ROMREGION_GETFLAGS(r) & ROMREGION_ENDIANMASK) == ROMREGION_LE)
        public static bool ROMREGION_ISBIGENDIAN(rom_entry_interface r) { return (ROMREGION_GETFLAGS(r) & romentry_global.ROMREGION_ENDIANMASK) == romentry_global.ROMREGION_BE; }
        public static bool ROMREGION_ISINVERTED(rom_entry_interface r) { return (ROMREGION_GETFLAGS(r) & romentry_global.ROMREGION_INVERTMASK) == romentry_global.ROMREGION_INVERT; }
        public static bool ROMREGION_ISERASE(rom_entry_interface r) { return (ROMREGION_GETFLAGS(r) & romentry_global.ROMREGION_ERASEMASK) == romentry_global.ROMREGION_ERASE; }
        public static UInt32 ROMREGION_GETERASEVAL(rom_entry_interface r) { return ((ROMREGION_GETFLAGS(r) & romentry_global.ROMREGION_ERASEVALMASK) >> 16); }
        public static UInt32 ROMREGION_GETDATATYPE(rom_entry_interface r) { return ROMREGION_GETFLAGS(r) & romentry_global.ROMREGION_DATATYPEMASK; }
        public static bool ROMREGION_ISROMDATA(rom_entry_interface r) { return ROMREGION_GETDATATYPE(r) == romentry_global.ROMREGION_DATATYPEROM; }
        public static bool ROMREGION_ISDISKDATA(rom_entry_interface r) { return ROMREGION_GETDATATYPE(r) == romentry_global.ROMREGION_DATATYPEDISK; }

        /* ----- per-ROM macros ----- */
        public static string ROM_GETNAME(rom_entry_interface r) { return r.name(); }
        //#define ROM_SAFEGETNAME(r)          (ROMENTRY_ISFILL(r) ? "fill" : ROMENTRY_ISCOPY(r) ? "copy" : ROM_GETNAME(r))
        public static UInt32 ROM_GETOFFSET(rom_entry_interface r) { return r.get_offset(); }
        public static UInt32 ROM_GETLENGTH(rom_entry_interface r) { return r.get_length(); }
        public static UInt32 ROM_GETFLAGS(rom_entry_interface r) { return r.get_flags(); }
        public static string ROM_GETHASHDATA(rom_entry_interface r) { return r.hashdata(); }
        public static bool ROM_ISOPTIONAL(rom_entry_interface r) { return (ROM_GETFLAGS(r) & romentry_global.ROM_OPTIONALMASK) == romentry_global.ROM_OPTIONAL; }
        public static UInt32 ROM_GETGROUPSIZE(rom_entry_interface r) { return ((ROM_GETFLAGS(r) & romentry_global.ROM_GROUPMASK) >> 8) + 1; }
        public static UInt32 ROM_GETSKIPCOUNT(rom_entry_interface r) {return (ROM_GETFLAGS(r) & romentry_global.ROM_SKIPMASK) >> 12; }
        public static bool ROM_ISREVERSED(rom_entry_interface r) { return (ROM_GETFLAGS(r) & romentry_global.ROM_REVERSEMASK) == romentry_global.ROM_REVERSE; }
        public static UInt32 ROM_GETBITWIDTH(rom_entry_interface r) { return (((ROM_GETFLAGS(r) & romentry_global.ROM_BITWIDTHMASK) >> 16) + 8U * (((ROM_GETFLAGS(r) & romentry_global.ROM_BITWIDTHMASK) == 0) ? 1U : 0U)); }
        public static UInt32 ROM_GETBITSHIFT(rom_entry_interface r) { return (ROM_GETFLAGS(r) & romentry_global.ROM_BITSHIFTMASK) >> 20; }
        public static bool ROM_INHERITSFLAGS(rom_entry_interface r) { return (ROM_GETFLAGS(r) & romentry_global.ROM_INHERITFLAGSMASK) == romentry_global.ROM_INHERITFLAGS; }
        public static UInt32 ROM_GETBIOSFLAGS(rom_entry_interface r) { return (ROM_GETFLAGS(r) & romentry_global.ROM_BIOSFLAGSMASK) >> 24; }

        /* ----- per-disk macros ----- */
        public static UInt32 DISK_GETINDEX(rom_entry_interface r) { return r.get_offset(); }
        public static bool DISK_ISREADONLY(rom_entry_interface r) { return (ROM_GETFLAGS(r) & romentry_global.DISK_READONLYMASK) == romentry_global.DISK_READONLY; }


        /* load the ROMs and open the disk images associated with the given machine */


        /* ----- Helpers ----- */

        /***************************************************************************
            HELPERS
         ***************************************************************************/

        public static Func<rom_entry> next_parent_system(game_driver system)  //auto next_parent_system(game_driver const &system)
        {
            return
                    () =>  //[sys = &system, roms = std::vector<rom_entry>()] () mutable -> rom_entry const *
                    {
                        var sys = system;
                        var roms = new std.vector<rom_entry>();

                        if (sys == null)
                            return null;
                        int parent = driver_list.find(sys.parent);
                        if (0 > parent)
                        {
                            sys = null;
                            return null;
                        }
                        else
                        {
                            sys = driver_list.driver(parent);
                            roms = rom_build_entries(sys.rom);
                            return roms[0];
                        }
                    };
        }


        //auto next_parent_software(std::vector<software_info const *> const &parents)
        //std::vector<std::string> make_software_searchpath(software_list_device &swlist, software_info const &swinfo, std::vector<software_info const *> &parents)


        public static chd_error do_open_disk(emu_options options, std.vector<string> [] searchpath, Pointer<rom_entry> romp, chd_file chd, Func<rom_entry> next_parent)  //chd_error do_open_disk(const emu_options &options, std::initializer_list<std::reference_wrapper<const std::vector<std::string> > > searchpath, const rom_entry *romp, chd_file &chd, std::function<const rom_entry * ()> next_parent)
        {
            throw new emu_unimplemented();
        }


        /* ----- ROM iteration ----- */

        /***************************************************************************
            ROM LOADING
        ***************************************************************************/

        /* return pointer to the first ROM region within a source */
        /*-------------------------------------------------
            rom_first_region - return pointer to first ROM
            region
        -------------------------------------------------*/
        public static Pointer<rom_entry> rom_first_region(device_t device)  //const rom_entry *rom_first_region(const device_t &device);
        {
            return rom_first_region(new Pointer<rom_entry>(device.rom_region_vector()));
        }


        public static Pointer<rom_entry> rom_first_region(Pointer<rom_entry> romp)  //const rom_entry *rom_first_region(const rom_entry *romp);
        {
            while (ROMENTRY_ISPARAMETER(romp[0]) || ROMENTRY_ISSYSTEM_BIOS(romp[0]) || ROMENTRY_ISDEFAULT_BIOS(romp[0]))
                romp++;
            return !ROMENTRY_ISEND(romp[0]) ? romp : null;
        }


        /* return pointer to the next ROM region within a source */
        /*-------------------------------------------------
            rom_next_region - return pointer to next ROM
            region
        -------------------------------------------------*/
        public static Pointer<rom_entry> rom_next_region(Pointer<rom_entry> rompIn)  //const rom_entry *rom_next_region(const rom_entry *romp);
        {
            Pointer<rom_entry> romp = new Pointer<rom_entry>(rompIn);

            romp++;
            while (!ROMENTRY_ISREGIONEND(romp[0]))
                romp++;
            while (ROMENTRY_ISPARAMETER(romp[0]))
                romp++;
            return ROMENTRY_ISEND(romp[0]) ? null : romp;
        }

        /* return pointer to the first ROM file within a region */
        /*-------------------------------------------------
            rom_first_file - return pointer to first ROM
            file
        -------------------------------------------------*/
        public static Pointer<rom_entry> rom_first_file(Pointer<rom_entry> rompIn)  //const rom_entry *rom_first_file(const rom_entry *romp);
        {
            Pointer<rom_entry> romp = new Pointer<rom_entry>(rompIn);

            romp++;
            while (!ROMENTRY_ISFILE(romp[0]) && !ROMENTRY_ISREGIONEND(romp[0]))
                romp++;
            return ROMENTRY_ISREGIONEND(romp[0]) ? null : romp;
        }

        /* return pointer to the next ROM file within a region */
        /*-------------------------------------------------
            rom_next_file - return pointer to next ROM
            file
        -------------------------------------------------*/
        public static Pointer<rom_entry> rom_next_file(Pointer<rom_entry> rompIn)  //const rom_entry *rom_next_file(const rom_entry *romp);
        {
            Pointer<rom_entry> romp = new Pointer<rom_entry>(rompIn);

            romp++;
            while (!ROMENTRY_ISFILE(romp[0]) && !ROMENTRY_ISREGIONEND(romp[0]))
                romp++;
            return ROMENTRY_ISREGIONEND(romp[0]) ? null : romp;
        }

        /* return the expected size of a file given the ROM description */
        /*-------------------------------------------------
            rom_file_size - return the expected size of a
            file given the ROM description
        -------------------------------------------------*/
        public static u32 rom_file_size(Pointer<rom_entry> rompIn)  //u32 rom_file_size(const rom_entry *romp);
        {
            Pointer<rom_entry> romp = new Pointer<rom_entry>(rompIn);

            u32 maxlength = 0;

            /* loop until we run out of reloads */
            do
            {
                /* loop until we run out of continues/ignores */
                u32 curlength = ROM_GETLENGTH(romp[0]);
                romp++;
                while (ROMENTRY_ISCONTINUE(romp[0]) || ROMENTRY_ISIGNORE(romp[0]))
                {
                    curlength += ROM_GETLENGTH(romp[0]);
                    romp++;
                }

                /* track the maximum length */
                maxlength = Math.Max(maxlength, curlength);
            }
            while (ROMENTRY_ISRELOAD(romp[0]));

            return maxlength;
        }


        /* return pointer to the first per-game parameter */
        /*-------------------------------------------------
            rom_first_parameter - return pointer to the first
            per-game parameter
        -------------------------------------------------*/
        public static rom_entry rom_first_parameter(device_t device, ref int curIndex)
        {
            MemoryContainer<rom_entry> romp = device.rom_region_vector();
            while (romp != null && romp.Count > 0 && romp[curIndex] != null && !ROMENTRY_ISEND(romp[curIndex]) && !ROMENTRY_ISPARAMETER(romp[curIndex]))
                curIndex++;

            return (romp != null && romp.Count > 0 && romp[curIndex] != null && !ROMENTRY_ISEND(romp[curIndex])) ? romp[curIndex] : null;
        } 


        /* return pointer to the next per-game parameter */
        /*-------------------------------------------------
            rom_next_parameter - return pointer to the next
            per-game parameter
        -------------------------------------------------*/
        public static rom_entry rom_next_parameter(device_t device, ref int curIndex)
        {
            //romp++;
            curIndex++;

            MemoryContainer<rom_entry> romp = device.rom_region_vector();

            while (!ROMENTRY_ISREGIONEND(romp[curIndex]) && !ROMENTRY_ISPARAMETER(romp[curIndex]))
                curIndex++;

            return ROMENTRY_ISEND(romp[curIndex]) ? null : romp[curIndex];
        }


        // builds a rom_entry vector from a tiny_rom_entry array
        // -------------------------------------------------
        // rom_build_entries - builds a rom_entry vector
        // from a tiny_rom_entry array
        // -------------------------------------------------
        public static std.vector<rom_entry> rom_build_entries(Pointer<tiny_rom_entry> tinyentries)
        {
            std.vector<rom_entry> result = new std.vector<rom_entry>();

            if (tinyentries != null)
            {
                int i = 0;
                do
                {
                    result.emplace_back(new rom_entry(tinyentries[i]));
                }
                while (!ROMENTRY_ISEND(tinyentries[i++]));
            }
            else
            {
                tiny_rom_entry end_entry = new tiny_rom_entry(null, null, 0, 0, romentry_global.ROMENTRYTYPE_END);
                result.emplace_back(new rom_entry(end_entry));
            }

            return result;
        }
    }


    namespace romload
    {
        //template <typename T>
        class const_entry_iterator<T> where T : tiny_rom_entry
        {
            //typedef T value_type;
            //typedef value_type const *pointer;
            //typedef value_type const &reference;
            //typedef std::ptrdiff_t difference_type;
            //typedef std::forward_iterator_tag iterator_category;


            protected Pointer<tiny_rom_entry> m_data;
            protected int m_dataOffset;

            protected const_entry_iterator() { m_data = null; }
            protected const_entry_iterator(Pointer<tiny_rom_entry> data, int dataOffset) { m_data = data; m_dataOffset = dataOffset; }
            //constexpr const_entry_iterator(const_entry_iterator const &) noexcept = default;
            //const_entry_iterator(const_entry_iterator &&) noexcept = default;
            //const_entry_iterator &operator=(const_entry_iterator const &) noexcept = default;
            //const_entry_iterator &operator=(const_entry_iterator &&) noexcept = default;


            //reference operator*() const noexcept { return reinterpret_cast<reference>(*m_data); }
            //pointer operator->() const noexcept { return reinterpret_cast<pointer>(m_data); }
            public T get() { return m_data == null ? null : (T)m_data[m_dataOffset]; }
        }


        class file : tiny_rom_entry
        {
            //file() = default;
            //file(file const &) = delete;
            //file &operator=(file const &) = delete;


            // ROM
            //constexpr char const *get_name()       const { return name; }
            //constexpr u32         get_offset()     const { return offset; }
            //constexpr u32         get_length()     const { return length; }
            //constexpr u32         get_flags()      const { return flags; }
            //constexpr char const *get_hashdata()   const { return hashdata; }
            //constexpr bool        is_optional()    const { return (flags & ROM_OPTIONALMASK) == ROM_OPTIONAL; }
            //constexpr u32         get_groupsize()  const { return ((flags & ROM_GROUPMASK) >> 8) + 1; }
            //constexpr u32         get_skipcount()  const { return (flags & ROM_SKIPMASK) >> 12; }
            //constexpr bool        is_reversed()    const { return (flags & ROM_REVERSEMASK) == ROM_REVERSE; }
            //constexpr u32         get_bitwidth()   const { return (flags & ROM_BITWIDTHMASK) ? ((flags & ROM_BITWIDTHMASK) >> 16) : 8; }
            //constexpr u32         get_bitshift()   const { return (flags & ROM_BITSHIFTMASK) >> 20; }
            //constexpr bool        inherits_flags() const { return (flags & ROM_INHERITFLAGSMASK) == ROM_INHERITFLAGS; }
            //constexpr u32         get_bios_flags() const { return (flags & ROM_BIOSFLAGSMASK) >> 24; }

            // disk
            //constexpr u32         get_index()      const { return offset; }
            //constexpr bool        is_readonly()    const { return (flags & DISK_READONLYMASK) == DISK_READONLY; }
        }


        class files
        {
            class const_iterator //: const_entry_iterator<file>
            {
                //friend class files;

                //constexpr const_iterator(tiny_rom_entry const *data) noexcept : const_entry_iterator<file>(data) { }


                //constexpr const_iterator() noexcept = default;
                //constexpr const_iterator(const_iterator const &) noexcept = default;
                //const_iterator(const_iterator &&) noexcept = default;
                //const_iterator &operator=(const_iterator const &) noexcept = default;
                //const_iterator &operator=(const_iterator &&) noexcept = default;

                //const_iterator &operator++() noexcept
                //{
                //    while (m_data)
                //    {
                //        ++m_data;
                //        if (ROMENTRY_ISFILE(m_data))
                //            break;
                //        else if (ROMENTRY_ISREGIONEND(m_data))
                //            m_data = nullptr;
                //    }
                //    return *this;
                //}

                //const_iterator operator++(int) noexcept { const_iterator result(*this); operator++(); return result; }

                //constexpr bool operator==(const_iterator const &rhs) const noexcept { return m_data == rhs.m_data; }
                //constexpr bool operator!=(const_iterator const &rhs) const noexcept { return m_data != rhs.m_data; }
            }


            //tiny_rom_entry const *m_data;


            //files(tiny_rom_entry const *data) : m_data(data)
            //{
            //    while (m_data && !ROMENTRY_ISFILE(m_data))
            //    {
            //        if (ROMENTRY_ISREGIONEND(m_data))
            //            m_data = nullptr;
            //        else
            //            ++m_data;
            //    }
            //}

            //const_iterator begin() const { return const_iterator(m_data); }
            //const_iterator cbegin() const { return const_iterator(m_data); }
            //const_iterator end() const { return const_iterator(nullptr); }
            //const_iterator cend() const { return const_iterator(nullptr); }
        }


        static class region  //class region final : tiny_rom_entry
        {
            //region() = default;
            //region(region const &) = delete;
            //region &operator=(region const &) = delete;


            public static string get_tag(tiny_rom_entry rom) { return rom.name_; }
            //constexpr u32         get_length()      const { return length; }
            //constexpr u32         get_width()       const { return 8 << ((flags & ROMREGION_WIDTHMASK) >> 8); }
            //constexpr bool        is_littleendian() const { return (flags & ROMREGION_ENDIANMASK) == ROMREGION_LE; }
            //constexpr bool        is_bigendian()    const { return (flags & ROMREGION_ENDIANMASK) == ROMREGION_BE; }
            //constexpr bool        is_inverted()     const { return (flags & ROMREGION_INVERTMASK) == ROMREGION_INVERT; }
            //constexpr bool        is_erase()        const { return (flags & ROMREGION_ERASEMASK) == ROMREGION_ERASE; }
            //constexpr u32         get_eraseval()    const { return (flags & ROMREGION_ERASEVALMASK) >> 16; }
            static UInt32 get_datatype(tiny_rom_entry rom) { return rom.flags & romentry_global.ROMREGION_DATATYPEMASK; }
            //constexpr bool        is_romdata()      const { return get_datatype() == ROMREGION_DATATYPEROM; }
            public static bool is_diskdata(tiny_rom_entry rom) { return get_datatype(rom) == romentry_global.ROMREGION_DATATYPEDISK; }

            //files get_files() const { return files(static_cast<tiny_rom_entry const *>(this) + 1); }
        }


        class regions : IEnumerable<tiny_rom_entry>
        {
            class const_iterator : const_entry_iterator<tiny_rom_entry>
            {
                //friend class regions;

                public const_iterator(Pointer<tiny_rom_entry> data, int dataOffset) : base(data, dataOffset) { }


                //constexpr const_iterator() noexcept = default;
                //constexpr const_iterator(const_iterator const &) noexcept = default;
                //const_iterator(const_iterator &&) noexcept = default;
                //const_iterator &operator=(const_iterator const &) noexcept = default;
                //const_iterator &operator=(const_iterator &&) noexcept = default;

                //const_iterator &operator++() noexcept
                //{
                //    while (m_data)
                //    {
                //        ++m_data;
                //        if (ROMENTRY_ISREGION(m_data))
                //            break;
                //        else if (ROMENTRY_ISEND(m_data))
                //            m_data = nullptr;
                //    }
                //    return *this;
                //}

                //const_iterator operator++(int) noexcept { const_iterator result(*this); operator++(); return result; }
                public void next()
                {
                    while (m_data != null && m_data[m_dataOffset] != null)
                    {
                        ++m_dataOffset;
                        if (romload_global.ROMENTRY_ISREGION(m_data[m_dataOffset]))
                            break;
                        else if (romload_global.ROMENTRY_ISEND(m_data[m_dataOffset]))
                            m_data = null;
                    }
                    //return *this;
                }


                //constexpr bool operator==(const_iterator const &rhs) const noexcept { return m_data == rhs.m_data; }
                //constexpr bool operator!=(const_iterator const &rhs) const noexcept { return m_data != rhs.m_data; }
            }


            Pointer<tiny_rom_entry> m_data;
            int m_dataOffset = 0;


            public regions(Pointer<tiny_rom_entry> data)
            {
                m_data = data;


                m_dataOffset = 0;
                while (m_data[m_dataOffset] != null && !romload_global.ROMENTRY_ISREGION(m_data[m_dataOffset]))
                {
                    if (romload_global.ROMENTRY_ISEND(m_data[m_dataOffset]))
                        m_data = null;
                    else
                        ++m_dataOffset;
                }
            }


            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
            public IEnumerator<tiny_rom_entry> GetEnumerator()
            {
                var iter = new const_iterator(m_data, m_dataOffset);

                while (iter.get() != null)
                {
                    yield return iter.get();
                    iter.next();
                }
            }

            //const_iterator begin() const { return const_iterator(m_data); }
            //const_iterator cbegin() const { return const_iterator(m_data); }
            //const_iterator end() const { return const_iterator(nullptr); }
            //const_iterator cend() const { return const_iterator(nullptr); }
        }


        class system_bios : tiny_rom_entry
        {
            //system_bios() = default;
            //system_bios(system_bios const &) = delete;
            //system_bios &operator=(system_bios const &) = delete;


            //constexpr u32         get_value()       const { return (flags & ROM_BIOSFLAGSMASK) >> 24; }
            //constexpr char const *get_name()        const { return name; }
            //constexpr char const *get_description() const { return hashdata; }
        }


        class system_bioses
        {
            class const_iterator //: const_entry_iterator<system_bios>
            {
                //friend class system_bioses;

                //constexpr const_iterator(tiny_rom_entry const *data) noexcept : const_entry_iterator<system_bios>(data) { }


                //constexpr const_iterator() noexcept = default;
                //constexpr const_iterator(const_iterator const &) noexcept = default;
                //const_iterator(const_iterator &&) noexcept = default;
                //const_iterator &operator=(const_iterator const &) noexcept = default;
                //const_iterator &operator=(const_iterator &&) noexcept = default;

                //const_iterator &operator++() noexcept
                //{
                //    while (m_data)
                //    {
                //        ++m_data;
                //        if (ROMENTRY_ISSYSTEM_BIOS(m_data))
                //            break;
                //        else if (ROMENTRY_ISEND(m_data))
                //            m_data = nullptr;
                //    }
                //    return *this;
                //}

                //const_iterator operator++(int) noexcept { const_iterator result(*this); operator++(); return result; }

                //constexpr bool operator==(const_iterator const &rhs) const noexcept { return m_data == rhs.m_data; }
                //constexpr bool operator!=(const_iterator const &rhs) const noexcept { return m_data != rhs.m_data; }
            }


            //tiny_rom_entry const *m_data;


            //system_bioses(tiny_rom_entry const *data) : m_data(data)
            //{
            //    while (m_data && !ROMENTRY_ISSYSTEM_BIOS(m_data))
            //    {
            //        if (ROMENTRY_ISEND(m_data))
            //            m_data = nullptr;
            //        else
            //            ++m_data;
            //    }
            //}

            //const_iterator begin() const { return const_iterator(m_data); }
            //const_iterator cbegin() const { return const_iterator(m_data); }
            //const_iterator end() const { return const_iterator(nullptr); }
            //const_iterator cend() const { return const_iterator(nullptr); }
        }


        class default_bios : tiny_rom_entry
        {
            //default_bios() = default;
            //default_bios(default_bios const &) = delete;
            //default_bios &operator=(default_bios const &) = delete;

            //constexpr char const *get_name() const { return name; }
        }


        class entries
        {
            Pointer<tiny_rom_entry> m_data;

            public entries(Pointer<tiny_rom_entry> data) { m_data = new Pointer<tiny_rom_entry>(data); }

            public regions get_regions() { return new regions(m_data); }
            //system_bioses get_system_bioses() const { return system_bioses(m_data); }
        }
    }


    // ======================> rom_load_manager
    public class rom_load_manager : global_object
    {
        const bool LOG_LOAD = false;
        static void LOG(string format, params object [] args) { if (LOG_LOAD) debugload(format, args); }


        class open_chd
        {
            string m_region;               /* disk region we came from */
            chd_file m_origchd;              /* handle to the original CHD */
            chd_file m_diffchd;              /* handle to the diff CHD */


            public open_chd(string region) { m_region = region; }

            //const char *region() const { return m_region.c_str(); }
            //chd_file &chd() { return m_diffchd.opened() ? m_diffchd : m_origchd; }
            public chd_file orig_chd() { return m_origchd; }
            public chd_file diff_chd() { return m_diffchd; }
        }


        // internal state
        running_machine m_machine;                  // reference to our machine

        int m_warnings;              // warning count during processing
        int m_knownbad;              // BAD_DUMP/NO_DUMP count during processing
        int m_errors;                // error count during processing

        int m_romsloaded;            // current ROMs loaded count
        int m_romstotal;             // total number of ROMs to read
        u64 m_romsloadedsize;     // total size of ROMs loaded so far
        u64 m_romstotalsize;      // total size of ROMs to read

        std.vector<open_chd> m_chd_list;  //std::vector<std::unique_ptr<open_chd>> m_chd_list;     /* disks */

        memory_region m_region;           // info about current region

        string m_errorstring;        // error string
        string m_softwarningstring;       // software warning string


        // construction/destruction
        /*-------------------------------------------------
            rom_init - load the ROMs and open the disk
            images associated with the given machine
        -------------------------------------------------*/
        public rom_load_manager(running_machine machine)
        {
            m_machine = machine;
            m_warnings = 0;
            m_knownbad = 0;
            m_errors = 0;
            m_romsloaded = 0;
            m_romstotal = 0;
            m_romsloadedsize = 0;
            m_romstotalsize = 0;
            m_chd_list = new std.vector<open_chd>();
            m_region = null;
            m_errorstring = null;
            m_softwarningstring = null;


            // figure out which BIOS we are using
            std.map<string, string> card_bios = new std.map<string, string>();
            foreach (device_t device in new device_enumerator(machine.config().root_device()))
            {
                device_slot_interface slot = device.GetClassInterface<device_slot_interface>();  //device_slot_interface const *const slot(dynamic_cast<device_slot_interface *>(&device));
                if (slot != null)
                {
                    device_t card = slot.get_card_device();
                    slot_option slot_opt = machine.options().slot_option(slot.slot_name());
                    if (card != null && !slot_opt.bios().empty())
                        card_bios.emplace(card.tag(), slot_opt.bios());
                }

                if (device.rom_region() != null)
                {
                    string specbios = "";
                    if (device.owner() == null)
                    {
                        specbios = machine.options().bios();
                    }
                    else
                    {
                        var found = card_bios.find(device.tag());
                        if (null != found)
                        {
                            specbios = found;
                            card_bios.erase(found);
                        }
                    }

                    determine_bios_rom(device, specbios);
                }
            }

            // count the total number of ROMs
            count_roms();

            // reset the disk list
            m_chd_list.Clear();

            // process the ROM entries we were passed
            process_region_list();

            // display the results and exit
            display_rom_load_results(false);
        }


        // getters
        running_machine machine() { return m_machine; }


        /* return the number of warnings we generated */
        public int warnings() { return m_warnings; }

        public string software_load_warnings_message() { return m_softwarningstring; }

        /* return the number of BAD_DUMP/NO_DUMP warnings we generated */
        public int knownbad() { return m_knownbad; }


        /* ----- disk handling ----- */

        /* return a pointer to the CHD file associated with the given region */
        //chd_file *get_disk_handle(const char *region);


        /* set a pointer to the CHD file associated with the given region */
        //int set_disk_handle(const char *region, const char *fullpath);


        //void load_software_part_region(device_t &device, software_list_device &swlist, const char *swname, const rom_entry *start_region);


        /* get search path for a software item */
        //static std::vector<std::string> get_software_searchpath(software_list_device &swlist, const software_info &swinfo);

        /* open a disk image, searching up the parent and loading by checksum */
        //static chd_error open_disk_image(const emu_options &options, const device_t &device, const rom_entry *romp, chd_file &image_chd);
        //static chd_error open_disk_image(const emu_options &options, software_list_device &swlist, const software_info &swinfo, const rom_entry *romp, chd_file &image_chd);


        /*-------------------------------------------------
            determine_bios_rom - determine system_bios
            from SystemBios structure and OPTION_BIOS
        -------------------------------------------------*/
        void determine_bios_rom(device_t device, string specbios)
        {
            // default is applied by the device at config complete time
            if (!string.IsNullOrEmpty(specbios) && core_stricmp(specbios, "default") != 0)  // (specbios && *specbios && core_stricmp(specbios, "default"))
            {
                bool found = false;
                foreach (rom_entry rom in device.rom_region_vector())
                {
                    if (romload_global.ROMENTRY_ISSYSTEM_BIOS(rom))
                    {
                        string biosname = romload_global.ROM_GETNAME(rom);
                        int bios_flags = (int)romload_global.ROM_GETBIOSFLAGS(rom);
                        string bios_number;

                        // Allow '-bios n' to still be used
                        //sprintf(bios_number, "%d", bios_flags - 1);
                        bios_number = string.Format("{0}", bios_flags - 1);
                        if (core_stricmp(bios_number, specbios) == 0 || core_stricmp(biosname, specbios) == 0)
                        {
                            found = true;
                            device.set_system_bios((u8)bios_flags);
                            break;
                        }
                    }
                }

                // if we got neither an empty string nor 'default' then warn the user
                if (!found)
                {
                    m_errorstring += string_format("{0}: invalid BIOS \"{1}\", reverting to default\n", device.tag(), specbios);
                    m_warnings++;
                }
            }

            // log final result
            LOG("For \"{0}\" using System BIOS: {1}\n", device.tag(), device.system_bios());
        }


        /*-------------------------------------------------
            count_roms - counts the total number of ROMs
            that will need to be loaded
        -------------------------------------------------*/
        void count_roms()
        {
            Pointer<rom_entry> region;  //const rom_entry *region, *rom;
            Pointer<rom_entry> rom;

            /* start with 0 */
            m_romstotal = 0;
            m_romstotalsize = 0;

            /* loop over regions, then over files */
            foreach (device_t device in new device_enumerator(machine().config().root_device()))
            {
                for (region = romload_global.rom_first_region(device); region != null; region = romload_global.rom_next_region(region))
                {
                    for (rom = romload_global.rom_first_file(region); rom != null; rom = romload_global.rom_next_file(rom))
                    {
                        if (romload_global.ROM_GETBIOSFLAGS(rom[0]) == 0 || romload_global.ROM_GETBIOSFLAGS(rom[0]) == device.system_bios())
                        {
                            m_romstotal++;
                            m_romstotalsize += romload_global.rom_file_size(rom);
                        }
                    }
                }
            }
        }


        /*-------------------------------------------------
            fill_random - fills an area of memory with
            random data
        -------------------------------------------------*/
        void fill_random(Pointer<u8> base_, u32 length)  //u8 *base
        {
            while (length-- > 0)
            {
                base_[0] = (byte)machine().rand();
                base_++;
            }
        }


        /*-------------------------------------------------
            handle_missing_file - handles error generation
            for missing files
        -------------------------------------------------*/
        void handle_missing_file(rom_entry romp, std.vector<string> tried_file_names, chd_error chderr)
        {
            string tried = "";
            if (!tried_file_names.empty())
            {
                tried = " (tried in";
                foreach (string path in tried_file_names)
                {
                    tried += ' ';
                    tried += path;
                }
                tried += ')';
            }

            string name = romload_global.ROM_GETNAME(romp);

            bool is_chd = chderr != chd_error.CHDERR_NONE;
            if (is_chd)
                name += ".chd";

            bool is_chd_error = is_chd && chderr != chd_error.CHDERR_FILE_NOT_FOUND;
            if (is_chd_error)
                m_errorstring += string.Format("{0} CHD ERROR: {1}\n", name, chd_file.error_string(chderr));

            if (romload_global.ROM_ISOPTIONAL(romp))
            {
                // optional files are okay
                if (!is_chd_error)
                    m_errorstring += string.Format("OPTIONAL {0} NOT FOUND{1}\n", name, tried);
                m_warnings++;
            }
            else if (new util.hash_collection(romload_global.ROM_GETHASHDATA(romp)).flag(util.hash_collection.FLAG_NO_DUMP))
            {
                // no good dumps are okay
                if (!is_chd_error)
                    m_errorstring += string.Format("{0} NOT FOUND (NO GOOD DUMP KNOWN){1}\n", name, tried);
                m_knownbad++;
            }
            else
            {
                // anything else is bad
                if (!is_chd_error)
                    m_errorstring += string.Format("{0} NOT FOUND{1}\n", name, tried);
                m_errors++;
            }
        }


        /*-------------------------------------------------
            dump_wrong_and_correct_checksums - dump an
            error message containing the wrong and the
            correct checksums for a given ROM
        -------------------------------------------------*/
        void dump_wrong_and_correct_checksums(util.hash_collection hashes, util.hash_collection acthashes)
        {
            m_errorstring += string.Format("    EXPECTED: {0}\n", hashes.macro_string());
            m_errorstring += string.Format("       FOUND: {0}\n", acthashes.macro_string());
        }


        /*-------------------------------------------------
            verify_length_and_hash - verify the length
            and hash signatures of a file
        -------------------------------------------------*/
        void verify_length_and_hash(emu_file file, string name, u32 explength, util.hash_collection hashes)
        {
            // we've already complained if there is no file
            if (file == null)
                return;

            // verify length
            u64 actlength = file.size();
            if (explength != actlength)
            {
                m_errorstring += string.Format("{0} WRONG LENGTH (expected: {1} found: {2})\n", name, explength, actlength);  // %08x found: %08x
                m_warnings++;
            }

            if (hashes.flag(util.hash_collection.FLAG_NO_DUMP))
            {
                // If there is no good dump known, write it
                m_errorstring += string.Format("{0} NO GOOD DUMP KNOWN\n", name);
                m_knownbad++;
            }
            else
            {
                // verify checksums
                util.hash_collection acthashes = file.hashes(hashes.hash_types().c_str());
                if (hashes != acthashes)
                {
                    // otherwise, it's just bad
                    util.hash_collection all_acthashes = (acthashes.hash_types() == util.hash_collection.HASH_TYPES_ALL)
                            ? acthashes
                            : file.hashes(util.hash_collection.HASH_TYPES_ALL);
                    m_errorstring += string_format("{0} WRONG CHECKSUMS:\n", name);
                    dump_wrong_and_correct_checksums(hashes, all_acthashes);
                    m_warnings++;
                }
                else if (hashes.flag(util.hash_collection.FLAG_BAD_DUMP))
                {
                    // If it matches, but it is actually a bad dump, write it
                    m_errorstring += string_format("{0} ROM NEEDS REDUMP\n", name);
                    m_knownbad++;
                }
            }
        }


        /*-------------------------------------------------
            display_loading_rom_message - display
            messages about ROM loading to the user
        -------------------------------------------------*/
        void display_loading_rom_message(string name, bool from_list)
        {
            string buffer;

            if (!string.IsNullOrEmpty(name))
                buffer = string_format("{0} ({1}%)", from_list ? "Loading Software" : "Loading Machine", (u32)(100 * m_romsloadedsize / m_romstotalsize));
            else
                buffer = "Loading Complete";

            if (!machine().ui().is_menu_active())
                machine().ui().set_startup_text(buffer.c_str(), false);
        }


        /*-------------------------------------------------
            display_rom_load_results - display the final
            results of ROM loading
        -------------------------------------------------*/
        void display_rom_load_results(bool from_list)
        {
            /* final status display */
            display_loading_rom_message(null, from_list);

            /* if we had errors, they are fatal */
            if (m_errors != 0)
            {
                /* create the error message and exit fatally */
                osd_printf_error("{0}", m_errorstring);
                throw new emu_fatalerror(EMU_ERR_MISSING_FILES, "Required files are missing, the machine cannot be run.");
            }

            /* if we had warnings, output them, but continue */
            if (m_warnings != 0 || m_knownbad != 0)
            {
                m_errorstring += "WARNING: the machine might not run correctly.";
                osd_printf_warning("{0}\n", m_errorstring);
            }
        }


        /*-------------------------------------------------
            region_post_process - post-process a region,
            byte swapping and inverting data as necessary
        -------------------------------------------------*/
        void region_post_process(memory_region region, bool invert)
        {
            // do nothing if no region
            if (region == null)
                return;

            LOG("+ datawidth={0}bit endian={1}\n", region.bitwidth(),
                    region.endianness() == endianness_t.ENDIANNESS_LITTLE ? "little" : "big");

            /* if the region is inverted, do that now */
            if (invert)
            {
                LOG(("+ Inverting region\n"));
                var base_ = new Pointer<u8>(region.base_());  //u8 *base_ = region->base_();
                for (int i = 0; i < region.bytes(); i++)
                {
                    base_[0] ^= 0xff;  base_++;  //*base_++ ^= 0xff;
                }
            }

            /* swap the endianness if we need to */
            if (region.bytewidth() > 1 && region.endianness() != ENDIANNESS_NATIVE)
            {
                LOG("+ Byte swapping region\n");
                int datawidth = region.bytewidth();
                var base_ = new Pointer<u8>(region.base_());  //u8 *base = region->base();
                for (int i = 0; i < region.bytes(); i += datawidth)
                {
                    MemoryU8 temp = new MemoryU8(8, true);  //u8 temp[8];
                    memcpy(new Pointer<u8>(temp), base_, (UInt32)datawidth);  //memcpy(temp, base, datawidth);
                    for (int j = datawidth - 1; j >= 0; j--)
                    {
                        base_[0] = temp[j];  base_++;  //*base++ = temp[j];
                    }
                }
            }
        }


        /*-------------------------------------------------
            open_rom_file - open a ROM file, searching
            up the parent and loading by checksum
        -------------------------------------------------*/
        emu_file open_rom_file(std.vector<string> [] searchpath, Pointer<rom_entry> romp, std.vector<string> tried_file_names, bool from_list)  //std::unique_ptr<emu_file> rom_load_manager::open_rom_file(std::initializer_list<std::reference_wrapper<const std::vector<std::string> > > searchpath, const rom_entry *romp, std::vector<std::string> &tried_file_names, bool from_list)
        {
            osd_file.error filerr = osd_file.error.NOT_FOUND;
            u32 romsize = romload_global.rom_file_size(romp);
            tried_file_names.clear();

            // update status display
            display_loading_rom_message(romload_global.ROM_GETNAME(romp[0]), from_list);

            // extract CRC to use for searching
            u32 crc = 0;
            bool has_crc = new util.hash_collection(romload_global.ROM_GETHASHDATA(romp[0])).crc(out crc);

            // attempt reading up the chain through the parents
            // it also automatically attempts any kind of load by checksum supported by the archives.
            emu_file result = null;
            foreach (std.vector<string> paths in searchpath)
            {
                result = open_rom_file(paths, tried_file_names, has_crc, crc, ROM_GETNAME(romp[0]), out filerr);
                if (result != null)
                    break;
            }

            // update counters
            m_romsloaded++;
            m_romsloadedsize += romsize;

            // return the result
            if (osd_file.error.NONE != filerr)
                return null;
            else
                return result;
        }


        emu_file open_rom_file(std.vector<string> paths, std.vector<string> tried, bool has_crc, u32 crc, string name, out osd_file.error filerr)
        {
            // record the set names we search
            tried.AddRange(paths);  //tried.insert(tried.end(), paths.begin(), paths.end());

            // attempt to open the file
            emu_file result = new emu_file(machine().options().media_path(), paths, OPEN_FLAG_READ);
            result.set_restrict_to_mediapath(1);
            if (has_crc)
                filerr = result.open(name, crc);
            else
                filerr = result.open(name);

            // don't return anything if unsuccessful
            if (osd_file.error.NONE != filerr)
                return null;
            else
                return result;
        }


        /*-------------------------------------------------
            rom_fread - cheesy fread that fills with
            random data for a nullptr file
        -------------------------------------------------*/
        int rom_fread(emu_file file, Pointer<u8> buffer, int length, rom_entry parent_region)  //int rom_fread(emu_file *file, u8 *buffer, int length, const rom_entry *parent_region);
        {
            if (file != null) // files just pass through
                return (int)file.read(buffer, (UInt32)length);

            if (!romload_global.ROMREGION_ISERASE(parent_region)) // otherwise, fill with randomness unless it was already specifically erased
                fill_random(buffer, (UInt32)length);

            return length;
        }


        /*-------------------------------------------------
            read_rom_data - read ROM data for a single
            entry
        -------------------------------------------------*/
        int read_rom_data(emu_file file, rom_entry parent_region, rom_entry romp)
        {
            int datashift = (int)romload_global.ROM_GETBITSHIFT(romp);
            int datamask = ((1 << (int)romload_global.ROM_GETBITWIDTH(romp)) - 1) << datashift;
            int numbytes = (int)romload_global.ROM_GETLENGTH(romp);
            int groupsize = (int)romload_global.ROM_GETGROUPSIZE(romp);
            int skip = (int)romload_global.ROM_GETSKIPCOUNT(romp);
            int reversed = romload_global.ROM_ISREVERSED(romp) ? 1 : 0;
            int numgroups = (numbytes + groupsize - 1) / groupsize;
            Pointer<u8> base_ = new Pointer<u8>(m_region.base_(), (int)romload_global.ROM_GETOFFSET(romp));  //u8 *base = m_region->base() + ROM_GETOFFSET(romp);

            u32 tempbufsize;
            int i;

            LOG("Loading ROM data: offs={0} len={1} mask={2} group={3} skip={4} reverse={5}\n", romload_global.ROM_GETOFFSET(romp), numbytes, datamask, groupsize, skip, reversed);  // %X len=%X mask=%02X group=%d skip=%d reverse=%d\n

            /* make sure the length was an even multiple of the group size */
            if (numbytes % groupsize != 0)
                osd_printf_warning("Warning in RomModule definition: {0} length not an even multiple of group size\n", romload_global.ROM_GETNAME(romp));

            /* make sure we only fill within the region space */
            if (romload_global.ROM_GETOFFSET(romp) + numgroups * groupsize + (numgroups - 1) * skip > m_region.bytes())
                throw new emu_fatalerror("Error in RomModule definition: {0} out of memory region space\n", romload_global.ROM_GETNAME(romp));

            /* make sure the length was valid */
            if (numbytes == 0)
                throw new emu_fatalerror("Error in RomModule definition: {0} has an invalid length\n", romload_global.ROM_GETNAME(romp));

            /* special case for simple loads */
            if (datamask == 0xff && (groupsize == 1 || reversed == 0) && skip == 0)
                return rom_fread(file, base_, numbytes, parent_region);

            /* use a temporary buffer for complex loads */
            tempbufsize = Math.Min(romload_global.TEMPBUFFER_MAX_SIZE, (UInt32)numbytes);
            MemoryU8 tempbuf = new MemoryU8((int)tempbufsize, true);

            /* chunky reads for complex loads */
            skip += groupsize;
            while (numbytes > 0)
            {
                int evengroupcount = ((int)tempbufsize / groupsize) * groupsize;
                int bytesleft = (numbytes > evengroupcount) ? evengroupcount : numbytes;
                Pointer<u8> bufptr = new Pointer<u8>(tempbuf);  //u8 *bufptr = &tempbuf[0];

                /* read as much as we can */
                LOG("  Reading {0} bytes into buffer\n", bytesleft);  // %X
                if (rom_fread(file, bufptr, bytesleft, parent_region) != bytesleft)
                    return 0;
                numbytes -= bytesleft;

                LOG("  Copying to {0}\n", base_);  // %p

                /* unmasked cases */
                if (datamask == 0xff)
                {
                    /* non-grouped data */
                    if (groupsize == 1)
                    {
                        for (i = 0; i < bytesleft; i++, base_ += skip)
                        {
                            base_[0] = bufptr[0];  // *base = *bufptr++;
                            bufptr++;
                        }
                    }

                    /* grouped data -- non-reversed case */
                    else if (reversed == 0)
                    {
                        while (bytesleft != 0)
                        {
                            for (i = 0; i < groupsize && bytesleft != 0; i++, bytesleft--)
                            {
                                base_[i] = bufptr[0];  // base[i] = *bufptr++;
                                bufptr++;
                            }

                            base_ += skip;
                        }
                    }

                    /* grouped data -- reversed case */
                    else
                    {
                        while (bytesleft != 0)
                        {
                            for (i = groupsize - 1; i >= 0 && bytesleft != 0; i--, bytesleft--)
                            {
                                base_[i] = bufptr[0];
                                bufptr++;
                            }

                            base_ += skip;
                        }
                    }
                }

                /* masked cases */
                else
                {
                    /* non-grouped data */
                    if (groupsize == 1)
                    {
                        for (i = 0; i < bytesleft; i++, base_ += skip)
                        {
                            base_[0] = (byte)((base_[0] & ~datamask) | ((bufptr[0] << (int)datashift) & datamask));
                            bufptr++;
                        }
                    }

                    /* grouped data -- non-reversed case */
                    else if (reversed == 0)
                    {
                        while (bytesleft != 0)
                        {
                            for (i = 0; i < groupsize && bytesleft != 0; i++, bytesleft--)
                            {
                                base_[i] = (byte)((base_[i] & ~datamask) | ((bufptr[0] << (int)datashift) & datamask));
                                bufptr++;
                            }

                            base_ += skip;
                        }
                    }

                    /* grouped data -- reversed case */
                    else
                    {
                        while (bytesleft != 0)
                        {
                            for (i = groupsize - 1; i >= 0 && bytesleft != 0; i--, bytesleft--)
                            {
                                base_[i] = (byte)((base_[i] & ~datamask) | ((bufptr[0] << (int)datashift) & datamask));
                                bufptr++;
                            }

                            base_ += skip;
                        }
                    }
                }
            }

            LOG("  All done\n");
            return (int)romload_global.ROM_GETLENGTH(romp);
        }


        /*-------------------------------------------------
            fill_rom_data - fill a region of ROM space
        -------------------------------------------------*/
        void fill_rom_data(rom_entry romp)
        {
            u32 numbytes = romload_global.ROM_GETLENGTH(romp);
            Pointer<u8> base_ = new Pointer<u8>(m_region.base_(), (int)romload_global.ROM_GETOFFSET(romp));  //u8 *base = m_region->base() + ROM_GETOFFSET(romp);

            /* make sure we fill within the region space */
            if (romload_global.ROM_GETOFFSET(romp) + numbytes > m_region.bytes())
                throw new emu_fatalerror("Error in RomModule definition: FILL out of memory region space\n");

            /* make sure the length was valid */
            if (numbytes == 0)
                throw new emu_fatalerror("Error in RomModule definition: FILL has an invalid length\n");

            /* fill the data (filling value is stored in place of the hashdata) */
            memset(base_, (byte)(romload_global.ROM_GETHASHDATA(romp)[0] & 0xff), numbytes);  // memset(base_, (FPTR)ROM_GETHASHDATA(romp) & 0xff, numbytes);
        }


        /*-------------------------------------------------
            copy_rom_data - copy a region of ROM space
        -------------------------------------------------*/
        void copy_rom_data(rom_entry romp)
        {
            Pointer<u8> base_ = new Pointer<u8>(m_region.base_(), (int)romload_global.ROM_GETOFFSET(romp));  //u8 *base = m_region->base() + ROM_GETOFFSET(romp);
            string srcrgntag = romload_global.ROM_GETNAME(romp);
            u32 numbytes = romload_global.ROM_GETLENGTH(romp);
            u32 srcoffs = romload_global.ROM_GETHASHDATA(romp)[0];  /* srcoffset in place of hashdata */

            /* make sure we copy within the region space */
            if (romload_global.ROM_GETOFFSET(romp) + numbytes > m_region.bytes())
                throw new emu_fatalerror("Error in RomModule definition: COPY out of target memory region space\n");

            /* make sure the length was valid */
            if (numbytes == 0)
                throw new emu_fatalerror("Error in RomModule definition: COPY has an invalid length\n");

            /* make sure the source was valid */
            memory_region region = machine().root_device().memregion(srcrgntag);
            if (region == null)
                throw new emu_fatalerror("Error in RomModule definition: COPY from an invalid region\n");

            /* make sure we find within the region space */
            if (srcoffs + numbytes > region.bytes())
                throw new emu_fatalerror("Error in RomModule definition: COPY out of source memory region space\n");

            /* fill the data */
            memcpy(base_, new Pointer<u8>(region.base_(), (int)srcoffs), numbytes);  // memcpy(base_, region->base_() + srcoffs, numbytes);
        }


        /*-------------------------------------------------
            process_rom_entries - process all ROM entries
            for a region
        -------------------------------------------------*/
        void process_rom_entries(std.vector<string> [] searchpath, u8 bios, rom_entry parent_region, Pointer<rom_entry> romp, bool from_list)  //void process_rom_entries(std::initializer_list<std::reference_wrapper<const std::vector<std::string> > > searchpath, u8 bios, const rom_entry *parent_region, const rom_entry *romp, bool from_list);
        {
            u32 lastflags = 0;
            std.vector<string> tried_file_names = new std.vector<string>();

            // loop until we hit the end of this region
            while (!romload_global.ROMENTRY_ISREGIONEND(romp[0]))
            {
                tried_file_names.clear();

                if (romload_global.ROMENTRY_ISCONTINUE(romp[0]))
                    throw new emu_fatalerror("Error in RomModule definition: ROM_CONTINUE not preceded by ROM_LOAD\n");

                if (romload_global.ROMENTRY_ISIGNORE(romp[0]))
                    throw new emu_fatalerror("Error in RomModule definition: ROM_IGNORE not preceded by ROM_LOAD\n");

                if (romload_global.ROMENTRY_ISRELOAD(romp[0]))
                    throw new emu_fatalerror("Error in RomModule definition: ROM_RELOAD not preceded by ROM_LOAD\n");

                if (romload_global.ROMENTRY_ISFILL(romp[0]))
                {
                    if (ROM_GETBIOSFLAGS(romp[0]) == 0 || ROM_GETBIOSFLAGS(romp[0]) == bios)
                        fill_rom_data(romp[0]);

                    romp++;
                }
                else if (romload_global.ROMENTRY_ISCOPY(romp[0]))
                {
                    copy_rom_data(romp[0]);
                    romp++;
                }
                else if (romload_global.ROMENTRY_ISFILE(romp[0]))
                {
                    // handle files
                    bool irrelevantbios = (romload_global.ROM_GETBIOSFLAGS(romp[0]) != 0) && (romload_global.ROM_GETBIOSFLAGS(romp[0]) != bios);
                    rom_entry baserom = romp[0];
                    int explength = 0;

                    // open the file if it is a non-BIOS or matches the current BIOS
                    LOG("Opening ROM file: {0}\n", romload_global.ROM_GETNAME(romp[0]));
                    emu_file file = null;
                    if (!irrelevantbios)
                    {
                        file = open_rom_file(searchpath, romp, tried_file_names, from_list);
                        if (file == null)
                            handle_missing_file(romp[0], tried_file_names, chd_error.CHDERR_NONE);
                    }

                    // loop until we run out of reloads
                    do
                    {
                        // loop until we run out of continues/ignores
                        do
                        {
                            rom_entry modified_romp = romp[0]; // *romp++;
                            romp++;
                            //int readresult;

                            // handle flag inheritance
                            if (!romload_global.ROM_INHERITSFLAGS(modified_romp))
                                lastflags = modified_romp.get_flags();
                            else
                                modified_romp.set_flags((modified_romp.get_flags() & ~romentry_global.ROM_INHERITEDFLAGS) | lastflags);

                            explength += (int)romload_global.ROM_GETLENGTH(modified_romp);

                            // attempt to read using the modified entry
                            if (!romload_global.ROMENTRY_ISIGNORE(modified_romp) && !irrelevantbios)
                                /*readresult = */read_rom_data(file, parent_region, modified_romp);
                        }
                        while (romload_global.ROMENTRY_ISCONTINUE(romp[0]) || romload_global.ROMENTRY_ISIGNORE(romp[0]));

                        // if this was the first use of this file, verify the length and CRC
                        if (baserom != null)
                        {
                            LOG("Verifying length ({0}) and checksums\n", explength);  // %X
                            verify_length_and_hash(file, romload_global.ROM_GETNAME(baserom), (UInt32)explength, new util.hash_collection(romload_global.ROM_GETHASHDATA(baserom)));
                            LOG("Verify finished\n");
                        }

                        // re-seek to the start and clear the baserom so we don't reverify
                        if (file != null)
                            file.seek(0, emu_file.SEEK_SET);

                        baserom = null;
                        explength = 0;
                    }
                    while (romload_global.ROMENTRY_ISRELOAD(romp[0]));

                    // close the file
                    if (file != null)
                    {
                        LOG(("Closing ROM file\n"));
                        file.close();
                        file = null;
                    }
                }
                else
                {
                    romp++; // something else - skip
                }
            }
        }


        /*-------------------------------------------------
            open_disk_diff - open a DISK diff file
        -------------------------------------------------*/
        chd_error open_disk_diff(emu_options options, rom_entry romp, chd_file source, chd_file diff_chd)
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            process_disk_entries - process all disk entries
            for a region
        -------------------------------------------------*/
        void process_disk_entries(std.vector<string> [] searchpath, string regiontag, Pointer<rom_entry> romp, Func<rom_entry> next_parent)  //void process_disk_entries(std::initializer_list<std::reference_wrapper<const std::vector<std::string> > > searchpath, const char *regiontag, const rom_entry *romp, std::function<const rom_entry * ()> next_parent);
        {
            //throw new emu_unimplemented();
#if false
            /* remove existing disk entries for this region */
            m_chd_list.erase(std::remove_if(m_chd_list.begin(), m_chd_list.end(), [regiontag] (std::unique_ptr<open_chd> &chd) { return !strcmp(chd->region(), regiontag); }), m_chd_list.end());
#endif

            /* loop until we hit the end of this region */
            for ( ; !romload_global.ROMENTRY_ISREGIONEND(romp[0]); romp++)
            {
                /* handle files */
                if (romload_global.ROMENTRY_ISFILE(romp[0]))
                {
                    var chd = new open_chd(regiontag);
                    chd_error err;

                    /* make the filename of the source */
                    string filename = romload_global.ROM_GETNAME(romp[0]) + ".chd";

                    /* first open the source drive */
                    // FIXME: we've lost the ability to search parents here
                    LOG("Opening disk image: {0}\n", filename);
                    err = romload_global.do_open_disk(machine().options(), searchpath, romp, chd.orig_chd(), next_parent);
                    if (err != chd_error.CHDERR_NONE)
                    {
                        handle_missing_file(romp[0], new std.vector<string>(), err);
                        chd = null;
                        continue;
                    }

                    /* get the header and extract the SHA1 */
                    util.hash_collection acthashes = new util.hash_collection();
                    acthashes.add_sha1(chd.orig_chd().sha1());

                    /* verify the hash */
                    util.hash_collection hashes = new util.hash_collection(ROM_GETHASHDATA(romp[0]));
                    if (hashes != acthashes)
                    {
                        m_errorstring += string.Format("{0} WRONG CHECKSUMS:\n", filename);
                        dump_wrong_and_correct_checksums(hashes, acthashes);
                        m_warnings++;
                    }
                    else if (hashes.flag(util.hash_collection.FLAG_BAD_DUMP))
                    {
                        m_errorstring += string.Format("{0} CHD NEEDS REDUMP\n", filename);
                        m_knownbad++;
                    }

                    /* if not read-only, make the diff file */
                    if (!romload_global.DISK_ISREADONLY(romp[0]))
                    {
                        /* try to open or create the diff */
                        err = open_disk_diff(machine().options(), romp[0], chd.orig_chd(), chd.diff_chd());
                        if (err != chd_error.CHDERR_NONE)
                        {
                            m_errorstring += string.Format("{0} DIFF CHD ERROR: {1}\n", filename, chd_file.error_string(err));
                            m_errors++;
                            chd = null;
                            continue;
                        }
                    }

                    /* we're okay, add to the list of disks */
                    LOG("Assigning to handle {0}\n", romload_global.DISK_GETINDEX(romp[0]));
                    m_chd_list.Add(chd);
                }
            }
        }


        /*-------------------------------------------------
            normalize_flags_for_device - modify the region
            flags for the given device
        -------------------------------------------------*/
        void normalize_flags_for_device(string rgntag, ref u8 width, ref endianness_t endian)
        {
            device_t device = machine().root_device().subdevice(rgntag);
            device_memory_interface memory;
            if (device != null && device.interface_(out memory))
            {
                address_space_config spaceconfig = memory.space_config();
                if (spaceconfig != null)
                {
                    int buswidth;

                    /* set the endianness */
                    if (spaceconfig.endianness() == endianness_t.ENDIANNESS_LITTLE)
                        endian = endianness_t.ENDIANNESS_LITTLE;
                    else
                        endian = endianness_t.ENDIANNESS_BIG;

                    /* set the width */
                    buswidth = spaceconfig.data_width();
                    if (buswidth <= 8)
                        width = 1;
                    else if (buswidth <= 16)
                        width = 2;
                    else if (buswidth <= 32)
                        width = 4;
                    else
                        width = 8;
                }
            }
        }


        /*-------------------------------------------------
            process_region_list - process a region list
        -------------------------------------------------*/
        void process_region_list()
        {
            // loop until we hit the end
            device_enumerator deviter = new device_enumerator(machine().root_device());
            std.vector<string> searchpath = new std.vector<string>();
            foreach (device_t device in deviter)
            {
                searchpath.clear();
                Func<rom_entry> next_parent = null;  //std::function<const rom_entry * ()> next_parent;

                for (Pointer<rom_entry> region = romload_global.rom_first_region(device); region != null; region = romload_global.rom_next_region(region))
                {
                    u32 regionlength = romload_global.ROMREGION_GETLENGTH(region[0]);

                    string regiontag = device.subtag(ROM_GETNAME(region[0]));
                    LOG("Processing region \"{0}\" (length={1})\n", regiontag, regionlength);

                    // the first entry must be a region
                    assert(romload_global.ROMENTRY_ISREGION(region[0]));

                    if (romload_global.ROMREGION_ISROMDATA(region[0]))
                    {
                        // if this is a device region, override with the device width and endianness
                        u8 width = (byte)(romload_global.ROMREGION_GETWIDTH(region[0]) / 8);
                        endianness_t endianness = romload_global.ROMREGION_ISBIGENDIAN(region[0]) ? endianness_t.ENDIANNESS_BIG : endianness_t.ENDIANNESS_LITTLE;
                        normalize_flags_for_device(regiontag, ref width, ref endianness);

                        // remember the base and length
                        m_region = machine().memory().region_alloc(regiontag, regionlength, width, endianness);
                        LOG("Allocated {0} bytes @ {1}\n", m_region.bytes(), m_region.name());  // %X bytes @ %p\n

                        if (romload_global.ROMREGION_ISERASE(region[0])) // clear the region if it's requested
                            memset(new Pointer<u8>(m_region.base_()), (u8)romload_global.ROMREGION_GETERASEVAL(region[0]), m_region.bytes());
                        else if (m_region.bytes() <= 0x400000) // or if it's sufficiently small (<= 4MB)
                            memset(new Pointer<u8>(m_region.base_()), (u8)0, m_region.bytes());

#if MAME_DEBUG
                        else // if we're debugging, fill region with random data to catch errors
                            fill_random(m_region->base_(), m_region->bytes());
#endif

                        // now process the entries in the region
                        if (searchpath.empty())
                            searchpath = device.searchpath();
                        assert(!searchpath.empty());
                        process_rom_entries(new std.vector<string> [] { searchpath }, device.system_bios(), region[0], region + 1, false);  //process_rom_entries({ searchpath }, device.system_bios(), region, region + 1, false);
                    }
                    else if (romload_global.ROMREGION_ISDISKDATA(region[0]))
                    {
                        if (searchpath.empty())
                            searchpath = device.searchpath();
                        assert(!searchpath.empty());
                        if (next_parent == null)
                        {
                            driver_device driver = (driver_device)device;  //driver_device const *const driver(dynamic_cast<driver_device const *>(&device));
                            if (driver != null)
                                next_parent = romload_global.next_parent_system(driver.system());
                            else
                                next_parent = () => { return null; };
                        }
                        process_disk_entries(new std.vector<string> [] { searchpath }, regiontag.c_str(), region + 1, next_parent);  //process_disk_entries({ searchpath }, regiontag.c_str(), region + 1, next_parent);
                    }
                }
            }

            // now go back and post-process all the regions
            foreach (device_t device in deviter)
            {
                for (Pointer<rom_entry> region = romload_global.rom_first_region(device); region != null; region = romload_global.rom_next_region(region))
                {
                    region_post_process(device.memregion(ROM_GETNAME(region[0])), romload_global.ROMREGION_ISINVERTED(region[0]));
                }
            }

            // and finally register all per-game parameters
            foreach (device_t device in deviter)
            {
                int curIndex = 0;
                for (rom_entry param = romload_global.rom_first_parameter(device, ref curIndex); param != null; param = romload_global.rom_next_parameter(device, ref curIndex))
                {
                    string regiontag = device.subtag(param.name().c_str());
                    machine().parameters().add(regiontag, param.hashdata());
                }
            }
        }


        /*-------------------------------------------------
            debugload - log data to a file
        -------------------------------------------------*/
        static void debugload(string format, params object [] args)
        {
            //static int opened;
            //va_list arg;
            //FILE *f;

            //f = fopen("romload.log", opened++ ? "a" : "w");
            //if (f)
            //{
            //    va_start(arg, string);
            //    vfprintf(f, string, arg);
            //    va_end(arg);
            //    fclose(f);
            //}

            osd_printf_info(format, args);
        }
    }
}
