// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using image_interface_enumerator = mame.device_interface_enumerator<mame.device_image_interface>;  //typedef device_interface_enumerator<device_image_interface> image_interface_enumerator;


namespace mame
{
    enum image_error_t : int
    {
        INTERNAL = 1,
        UNSUPPORTED,
        INVALIDIMAGE,
        ALREADYOPEN,
        UNSPECIFIED
    }


    //const std::error_category &image_category() noexcept;
    //inline std::error_condition make_error_condition(image_error e) noexcept { return std::error_condition(int(e), image_category()); }
    //namespace std { template <> struct is_error_condition_enum<image_error> : public std::true_type { }; }


    //class image_device_format


    public enum image_init_result { PASS, FAIL }
    public enum image_verify_result { PASS, FAIL }


    //**************************************************************************
    //  MACROS
    //**************************************************************************

    //#define DEVICE_IMAGE_LOAD_MEMBER(_name)             image_init_result _name(device_image_interface &image)
    //#define DECLARE_DEVICE_IMAGE_LOAD_MEMBER(_name)     DEVICE_IMAGE_LOAD_MEMBER(_name)

    //#define DEVICE_IMAGE_UNLOAD_MEMBER(_name)           void _name(device_image_interface &image)
    //#define DECLARE_DEVICE_IMAGE_UNLOAD_MEMBER(_name)   DEVICE_IMAGE_UNLOAD_MEMBER(_name)


    // ======================> device_image_interface
    // class representing interface-specific live image
    public abstract class device_image_interface : device_interface
    {
        //typedef device_delegate<image_init_result (device_image_interface &)> load_delegate;
        //typedef device_delegate<void (device_image_interface &)> unload_delegate;

        //typedef std::vector<std::unique_ptr<image_device_format>> formatlist_type;


        // error related info
        std.error_condition m_err;
        string m_err_message;

        // variables that are only non-zero when an image is mounted
        //core_file *m_file;
        //emu_file *m_mame_file;
        string m_image_name;
        //astring m_basename;
        string m_basename_noext;
        //astring m_filetype;

        // Software information
        string m_full_software_name;
        software_part m_software_part_ptr;
        string m_software_list_name;

        // creation info
        //formatlist_type m_formatlist;

        // working directory; persists across mounts
        //std::string m_working_directory;

        // flags
        //bool m_readonly;
        //bool m_created;

        // special - used when creating
        //int m_create_format;
        //option_resolution *m_create_args;

        //hash_collection m_hash;

        string m_instance_name;                // e.g. - "cartridge", "floppydisk2"
        string m_brief_instance_name;          // e.g. - "cart", "flop2"
        string m_canonical_instance_name;      // e.g. - "cartridge1", "floppydisk2" - only used internally in emuopts.cpp

        // in the case of arcade cabinet with fixed carts inserted,
        // we want to disable command line cart loading...
        bool m_user_loadable;

        bool m_must_be_loaded;

        bool m_is_loading;

        bool m_is_reset_and_loading;


        // construction/destruction
        //-------------------------------------------------
        //  device_image_interface - constructor
        //-------------------------------------------------
        public device_image_interface(machine_config mconfig, device_t device)
            : base(device, "image")
        {
            throw new emu_unimplemented();
        }


        //virtual image_init_result call_load() { return FALSE; }
        //virtual image_init_result call_create(int format_type, option_resolution *format_options) { return FALSE; }
        //virtual void call_unload() { }
        public virtual string call_display() { return ""; }
        //virtual u32 unhashed_header_length() const { return 0; }
        //virtual bool core_opens_image_file() const { return TRUE; }
        protected virtual bool image_is_chd_type() { return false; }
        protected abstract bool is_readable();
        protected abstract bool is_writeable();
        protected abstract bool is_creatable();
        public abstract bool is_reset_on_load();
        protected virtual bool support_command_line_image_creation() { return false; }

        public abstract string image_interface();
        public abstract string file_extensions();
        //protected abstract option_guide create_option_guide();
        public abstract string image_type_name();
        protected abstract string image_brief_type_name();


        //const image_device_format *device_get_indexed_creatable_format(int index) const noexcept { return (index < m_formatlist.size()) ? m_formatlist.at(index).get() : nullptr;  }
        //const image_device_format *device_get_named_creatable_format(std::string_view format_name) const noexcept;
        //const option_guide *device_get_creation_option_guide() { return create_option_guide(); }


        /*-------------------------------------------------
            error - returns the error text for an image
            error
        -------------------------------------------------*/
        public string error()
        {
            if (m_err && m_err_message.empty())
                m_err_message = m_err.message();
            return m_err_message;
        }


        //void seterror(std::error_condition err, const char *message = nullptr);
        //void message(const char *format, ...) ATTR_PRINTF(2,3);


        public bool exists() { return !m_image_name.empty(); }

        // get image file path/name
        public string filename() { return m_image_name.empty() ? null : m_image_name; }
        //string basename() { return m_basename.empty() ? null : m_basename.c_str(); }
        public string basename_noext() { return m_basename_noext.empty() ? null : m_basename_noext; }
        //string filetype() { return m_filetype; }
        //bool is_filetype(const std::string &candidate_filetype);

        //bool is_open() const noexcept { return bool(m_file); }
        //util::core_file &image_core_file() const noexcept { assert(is_open()); return *m_file; }
        //u64 length() { check_for_file(); return m_file->size(); }
        //bool is_readonly() const noexcept { return m_readonly; }

        // image file I/O wrappers
        // TODO: move away from using these and let implementations use the I/O interface directly
        // FIXME: don't swallow errors
        //u64 length()
        //{
        //    check_for_file();
        //    u64 result = 0;
        //    m_file->length(result);
        //    return result;
        //}
        //u32 fread(void *buffer, u32 length)
        //{
        //    check_for_file();
        //    size_t actual;
        //    m_file->read(buffer, length, actual);
        //    return actual;
        //}
        //u32 fwrite(const void *buffer, u32 length)
        //{
        //    check_for_file();
        //    size_t actual;
        //    m_file->write(buffer, length, actual);
        //    return actual;
        //}
        //std::error_condition fseek(s64 offset, int whence)
        //{
        //    check_for_file();
        //    return m_file->seek(offset, whence);
        //}
        //u64 ftell()
        //{
        //    check_for_file();
        //    u64 result = 0;
        //    m_file->tell(result);
        //    return result;
        //}
        //bool image_feof()
        //{
        //    check_for_file();
        //    return m_file->eof();
        //}

        // allocate and read into buffers
        //u32 fread(std::unique_ptr<u8 []> &ptr, u32 length) { ptr = std::make_unique<u8 []>(length); return fread(ptr.get(), length); }
        //u32 fread(std::unique_ptr<u8 []> &ptr, u32 length, offs_t offset) { ptr = std::make_unique<u8 []>(length); return fread(ptr.get() + offset, length - offset); }

        // access to software list item information
        public software_info software_entry() { return (m_software_part_ptr == null) ? null : m_software_part_ptr.info(); }
        public software_part part_entry() { return m_software_part_ptr; }
        public string software_list_name() { return m_software_list_name; }
        public bool loaded_through_softlist() { return m_software_part_ptr != null; }


        // working directory
        //void set_working_directory(std::string_view working_directory) { m_working_directory = working_directory; }
        //void set_working_directory(std::string &&working_directory) { m_working_directory = std::move(working_directory); }
        //const std::string &working_directory() const { return m_working_directory; }

        // access to software list properties and ROM data areas
        //u8 *get_software_region(std::string_view tag);
        //u32 get_software_region_length(std::string_view tag);
        //const char *get_feature(std::string_view feature_name) const;
        //bool load_software_region(std::string_view tag, std::unique_ptr<u8[]> &ptr);

        //u32 crc();
        //hash_collection& hash() { return m_hash; }
        //util::hash_collection calculate_hash_on_file(util::random_read &file) const;


        //void battery_load(void *buffer, int length, int fill);
        //void battery_load(void *buffer, int length, void *def_buffer);
        //void battery_save(const void *buffer, int length);


        public string instance_name() { return m_instance_name; }
        public string brief_instance_name() { return m_brief_instance_name; }
        //const std::string &canonical_instance_name() const { return m_canonical_instance_name; }
        //formatlist_type formatlist() const { return m_formatlist.first(); }


        // loads an image file
        //image_init_result load(const std::string &path);

        // loads a softlist item by name
        //image_init_result load_software(const std::string &software_identifier);


        /*-------------------------------------------------
            image_finish_load - special call - only use
            from core
        -------------------------------------------------*/
        public image_init_result finish_load()
        {
            throw new emu_unimplemented();
        }


        /*-------------------------------------------------
            unload - main call to unload an image
        -------------------------------------------------*/
        public void unload()
        {
            throw new emu_unimplemented();
        }


        //image_init_result create(const char *path, const image_device_format *create_format, option_resolution *create_args);
        //image_init_result create(const std::string &path);
        //bool load_software(software_list_device &swlist, const char *swname, const rom_entry *entry);
        //std::error_condition reopen_for_write(std::string_view path);

        //void set_user_loadable(bool user_loadable) noexcept { m_user_loadable = user_loadable; }
        //void set_must_be_loaded(bool must_be_loaded) noexcept { m_must_be_loaded = must_be_loaded; }


        public bool user_loadable() { return m_user_loadable; }
        public bool must_be_loaded() { return m_must_be_loaded; }
        public bool is_reset_and_loading() { return m_is_reset_and_loading; }
        public string full_software_name() { return m_full_software_name; }


        // interface-level overrides

        //-------------------------------------------------
        //  interface_config_complete - perform any
        //  operations now that the configuration is
        //  complete
        //-------------------------------------------------
        public override void interface_config_complete()
        {
            // set brief and instance name
            update_names();
        }


        //virtual const software_list_loader &get_software_list_loader() const;
        //virtual const bool use_software_list_file_extension_for_filetype() const { return false; }
        //image_init_result load_internal(const std::string &path, bool is_create, int create_format, util::option_resolution *create_args);
        //void determine_open_plan(int is_create, UINT32 *open_plan);
        //std::error_condition load_image_by_path(u32 open_flags, std::string_view path);
        //void clear();
        //bool is_loaded();


        //void set_image_filename(const std::string &filename);


        //void clear_error();


        //void check_for_file() const { if (!m_file) throw emu_fatalerror("%s(%s): Illegal operation on unmounted image", device().shortname(), device().tag()); }


        //void make_readonly() { m_readonly = true; }


        //bool image_checkhash();


        //software_part *find_software_item(const char *path, bool restrict_to_interface);
        //string software_get_default_slot(const char *default_card_slot);


        //void add_format(std::unique_ptr<image_device_format> &&format);
        //void add_format(std::string &&name, std::string &&description, std::string &&extensions, std::string &&optspec);


        //std::vector<u32> determine_open_plan(bool is_create);

        //-------------------------------------------------
        //  update_names - update brief and instance names
        //-------------------------------------------------
        void update_names()
        {
            throw new emu_unimplemented();
        }


        //bool load_software_part(const std::string &identifier);

        //bool init_phase() const;
        //static std::error_condition run_hash(util::random_read &file, u32 skip_bytes, util::hash_collection &hashes, const char *types);

        // loads an image or software items and resets - called internally when we
        // load an is_reset_on_load() item
        //void reset_and_load(const std::string &path);
    }


    // iterator
    //typedef device_interface_enumerator<device_image_interface> image_interface_enumerator;
}
