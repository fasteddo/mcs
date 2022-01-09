// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint64_t = System.UInt64;


namespace mame
{
    // ======================> chd_file
    // core file class
    public class chd_file
    {
        // error types
        enum error
        {
            NO_INTERFACE = 1,
            NOT_OPEN,
            ALREADY_OPEN,
            INVALID_FILE,
            INVALID_DATA,
            REQUIRES_PARENT,
            FILE_NOT_WRITEABLE,
            CODEC_ERROR,
            INVALID_PARENT,
            HUNK_OUT_OF_RANGE,
            DECOMPRESSION_ERROR,
            COMPRESSION_ERROR,
            CANT_VERIFY,
            METADATA_NOT_FOUND,
            INVALID_METADATA_SIZE,
            UNSUPPORTED_VERSION,
            VERIFY_INCOMPLETE,
            INVALID_METADATA,
            INVALID_STATE,
            OPERATION_PENDING,
            UNSUPPORTED_FORMAT,
            UNKNOWN_COMPRESSION,
            WALKING_PARENT,
            COMPRESSING
        }


        // file characteristics
        util.random_read_write m_file;  //util::random_read_write::ptr m_file;        // handle to the open core file


        // construction/destruction
        /**
         * @fn  chd_file::chd_file()
         *
         * @brief   -------------------------------------------------
         *            chd_file - constructor
         *          -------------------------------------------------.
         */
        public chd_file()
        {
            throw new emu_unimplemented();
        }

        /**
         * @fn  chd_file::~chd_file()
         *
         * @brief   -------------------------------------------------
         *            ~chd_file - destructor
         *          -------------------------------------------------.
         */
        ~chd_file()
        {
            throw new emu_unimplemented();
        }


        // getters
        //util::random_read &file();
        public bool opened() { return m_file != null; }
        //uint32_t version() const { return m_version; }
        public uint64_t logical_bytes() { throw new emu_unimplemented(); }
        //uint32_t hunk_bytes() const { return m_hunkbytes; }
        //uint32_t hunk_count() const { return m_hunkcount; }
        //uint32_t unit_bytes() const { return m_unitbytes; }
        //uint64_t unit_count() const { return m_unitcount; }
        public bool compressed() { throw new emu_unimplemented(); }
        //chd_codec_type compression(int index) const { return m_compression[index]; }
        //chd_file *parent() const { return m_parent; }

        /**
         * @fn  sha1_t chd_file::sha1()
         *
         * @brief   -------------------------------------------------
         *            sha1 - return our SHA1 value
         *          -------------------------------------------------.
         *
         * @return  A sha1_t.
         */
        public util.sha1_t sha1() { throw new emu_unimplemented(); }

        //sha1_t raw_sha1();
        //sha1_t parent_sha1();
        //std::error_condition hunk_info(uint32_t hunknum, chd_codec_type &compressor, uint32_t &compbytes);


        // setters
        //void set_raw_sha1(sha1_t rawdata);
        //void set_parent_sha1(sha1_t parent);


        // file create
        //std::error_condition create(std::string_view filename, uint64_t logicalbytes, uint32_t hunkbytes, uint32_t unitbytes, chd_codec_type compression[4]);
        //std::error_condition create(util::random_read_write::ptr &&file, uint64_t logicalbytes, uint32_t hunkbytes, uint32_t unitbytes, chd_codec_type compression[4]);
        //std::error_condition create(std::string_view filename, uint64_t logicalbytes, uint32_t hunkbytes, chd_codec_type compression[4], chd_file &parent);
        //std::error_condition create(util::random_read_write::ptr &&file, uint64_t logicalbytes, uint32_t hunkbytes, chd_codec_type compression[4], chd_file &parent);


        // file open

        /**
         * @fn  chd_error chd_file::open(const char *filename, bool writeable, chd_file *parent)
         *
         * @brief   -------------------------------------------------
         *            open - open an existing file for read or read/write
         *          -------------------------------------------------.
         *
         * @param   filename        Filename of the file.
         * @param   writeable       true if writeable.
         * @param [in,out]  parent  If non-null, the parent.
         *
         * @return  A chd_error.
         */
        public std.error_condition open(string filename, bool writeable = false, chd_file parent = null)
        {
            throw new emu_unimplemented();
        }


        /**
         * @fn  chd_error chd_file::open(core_file &file, bool writeable, chd_file *parent)
         *
         * @brief   -------------------------------------------------
         *            open - open an existing file for read or read/write
         *          -------------------------------------------------.
         *
         * @param [in,out]  file    The file.
         * @param   writeable       true if writeable.
         * @param [in,out]  parent  If non-null, the parent.
         *
         * @return  A chd_error.
         */
        public std.error_condition open(util.core_file file, bool writeable = false, chd_file parent = null)
        {
            throw new emu_unimplemented();
        }


        // file close
        //void close();


        // read/write
        //std::error_condition read_hunk(uint32_t hunknum, void *buffer);
        //std::error_condition write_hunk(uint32_t hunknum, const void *buffer);
        //std::error_condition read_units(uint64_t unitnum, void *buffer, uint32_t count = 1);
        //std::error_condition write_units(uint64_t unitnum, const void *buffer, uint32_t count = 1);
        //std::error_condition read_bytes(uint64_t offset, void *buffer, uint32_t bytes);
        //std::error_condition write_bytes(uint64_t offset, const void *buffer, uint32_t bytes);


        // metadata management
        //std::error_condition read_metadata(chd_metadata_tag searchtag, uint32_t searchindex, std::string &output);
        //std::error_condition read_metadata(chd_metadata_tag searchtag, uint32_t searchindex, std::vector<uint8_t> &output);
        //std::error_condition read_metadata(chd_metadata_tag searchtag, uint32_t searchindex, void *output, uint32_t outputlen, uint32_t &resultlen);
        //std::error_condition read_metadata(chd_metadata_tag searchtag, uint32_t searchindex, std::vector<uint8_t> &output, chd_metadata_tag &resulttag, uint8_t &resultflags);
        //std::error_condition write_metadata(chd_metadata_tag metatag, uint32_t metaindex, const void *inputbuf, uint32_t inputlen, uint8_t flags = CHD_MDFLAGS_CHECKSUM);
        //std::error_condition write_metadata(chd_metadata_tag metatag, uint32_t metaindex, const std::string &input, uint8_t flags = CHD_MDFLAGS_CHECKSUM) { return write_metadata(metatag, metaindex, input.c_str(), input.length() + 1, flags); }
        //std::error_condition write_metadata(chd_metadata_tag metatag, uint32_t metaindex, const std::vector<uint8_t> &input, uint8_t flags = CHD_MDFLAGS_CHECKSUM) { return write_metadata(metatag, metaindex, &input[0], input.size(), flags); }
        //std::error_condition delete_metadata(chd_metadata_tag metatag, uint32_t metaindex);
        //std::error_condition clone_all_metadata(chd_file &source);


        // hashing helper
        //sha1_t compute_overall_sha1(sha1_t rawsha1);


        // codec interfaces
        //std::error_condition codec_configure(chd_codec_type codec, int param, void *config);
    }
}
