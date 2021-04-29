// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    // error types
    public enum chd_error
    {
        CHDERR_NONE,
        CHDERR_NO_INTERFACE,
        CHDERR_OUT_OF_MEMORY,
        CHDERR_NOT_OPEN,
        CHDERR_ALREADY_OPEN,
        CHDERR_INVALID_FILE,
        CHDERR_INVALID_PARAMETER,
        CHDERR_INVALID_DATA,
        CHDERR_FILE_NOT_FOUND,
        CHDERR_REQUIRES_PARENT,
        CHDERR_FILE_NOT_WRITEABLE,
        CHDERR_READ_ERROR,
        CHDERR_WRITE_ERROR,
        CHDERR_CODEC_ERROR,
        CHDERR_INVALID_PARENT,
        CHDERR_HUNK_OUT_OF_RANGE,
        CHDERR_DECOMPRESSION_ERROR,
        CHDERR_COMPRESSION_ERROR,
        CHDERR_CANT_CREATE_FILE,
        CHDERR_CANT_VERIFY,
        CHDERR_NOT_SUPPORTED,
        CHDERR_METADATA_NOT_FOUND,
        CHDERR_INVALID_METADATA_SIZE,
        CHDERR_UNSUPPORTED_VERSION,
        CHDERR_VERIFY_INCOMPLETE,
        CHDERR_INVALID_METADATA,
        CHDERR_INVALID_STATE,
        CHDERR_OPERATION_PENDING,
        CHDERR_UNSUPPORTED_FORMAT,
        CHDERR_UNKNOWN_COMPRESSION,
        CHDERR_WALKING_PARENT,
        CHDERR_COMPRESSING
    }


    // ======================> chd_file
    // core file class
    public class chd_file
    {
        // file characteristics
        util_.core_file m_file;             // handle to the open core file


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
        public bool opened() { return (m_file != null); }
        //UINT32 version() const { return m_version; }
        public UInt64 logical_bytes() { throw new emu_unimplemented(); }
        //UINT32 hunk_bytes() const { return m_hunkbytes; }
        //UINT32 hunk_count() const { return m_hunkcount; }
        //UINT32 unit_bytes() const { return m_unitbytes; }
        //UINT64 unit_count() const { return m_unitcount; }
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
        //chd_error hunk_info(UINT32 hunknum, chd_codec_type &compressor, UINT32 &compbytes);


        // setters
        //void set_raw_sha1(sha1_t rawdata);
        //void set_parent_sha1(sha1_t parent);


        // file create
        //chd_error create(const char *filename, UINT64 logicalbytes, UINT32 hunkbytes, UINT32 unitbytes, chd_codec_type compression[4]);
        //chd_error create(core_file &file, UINT64 logicalbytes, UINT32 hunkbytes, UINT32 unitbytes, chd_codec_type compression[4]);
        //chd_error create(const char *filename, UINT64 logicalbytes, UINT32 hunkbytes, chd_codec_type compression[4], chd_file &parent);
        //chd_error create(core_file &file, UINT64 logicalbytes, UINT32 hunkbytes, chd_codec_type compression[4], chd_file &parent);


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
        public chd_error open(string filename, bool writeable = false, chd_file parent = null)
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
        public chd_error open(util_.core_file file, bool writeable = false, chd_file parent = null)
        {
            throw new emu_unimplemented();
        }


        // file close
        //void close();


        // read/write
        //chd_error read_hunk(UINT32 hunknum, void *buffer);
        //chd_error write_hunk(UINT32 hunknum, const void *buffer);
        //chd_error read_units(UINT64 unitnum, void *buffer, UINT32 count = 1);
        //chd_error write_units(UINT64 unitnum, const void *buffer, UINT32 count = 1);
        //chd_error read_bytes(UINT64 offset, void *buffer, UINT32 bytes);
        //chd_error write_bytes(UINT64 offset, const void *buffer, UINT32 bytes);


        // metadata management
        //chd_error read_metadata(chd_metadata_tag searchtag, UINT32 searchindex, astring &output);
        //chd_error read_metadata(chd_metadata_tag searchtag, UINT32 searchindex, dynamic_buffer &output);
        //chd_error read_metadata(chd_metadata_tag searchtag, UINT32 searchindex, void *output, UINT32 outputlen, UINT32 &resultlen);
        //chd_error read_metadata(chd_metadata_tag searchtag, UINT32 searchindex, dynamic_buffer &output, chd_metadata_tag &resulttag, UINT8 &resultflags);
        //chd_error write_metadata(chd_metadata_tag metatag, UINT32 metaindex, const void *inputbuf, UINT32 inputlen, UINT8 flags = CHD_MDFLAGS_CHECKSUM);
        //chd_error write_metadata(chd_metadata_tag metatag, UINT32 metaindex, const astring &input, UINT8 flags = CHD_MDFLAGS_CHECKSUM) { return write_metadata(metatag, metaindex, input.cstr(), input.len() + 1, flags); }
        //chd_error write_metadata(chd_metadata_tag metatag, UINT32 metaindex, const dynamic_buffer &input, UINT8 flags = CHD_MDFLAGS_CHECKSUM) { return write_metadata(metatag, metaindex, input, input.count(), flags); }
        //chd_error delete_metadata(chd_metadata_tag metatag, UINT32 metaindex);
        //chd_error clone_all_metadata(chd_file &source);


        // hashing helper
        //sha1_t compute_overall_sha1(sha1_t rawsha1);


        // codec interfaces
        //chd_error codec_configure(chd_codec_type codec, int param, void *config);


        // static helpers

        /**
         * @fn  const char *chd_file::error_string(chd_error err)
         *
         * @brief   -------------------------------------------------
         *            error_string - return an error string for the given CHD error
         *          -------------------------------------------------.
         *
         * @param   err The error.
         *
         * @return  null if it fails, else a char*.
         */
        public static string error_string(chd_error err)
        {
            switch (err)
            {
                case chd_error.CHDERR_NONE:                       return "no error";
                case chd_error.CHDERR_NO_INTERFACE:               return "no drive interface";
                case chd_error.CHDERR_OUT_OF_MEMORY:              return "out of memory";
                case chd_error.CHDERR_NOT_OPEN:                   return "file not open";
                case chd_error.CHDERR_ALREADY_OPEN:               return "file already open";
                case chd_error.CHDERR_INVALID_FILE:               return "invalid file";
                case chd_error.CHDERR_INVALID_PARAMETER:          return "invalid parameter";
                case chd_error.CHDERR_INVALID_DATA:               return "invalid data";
                case chd_error.CHDERR_FILE_NOT_FOUND:             return "file not found";
                case chd_error.CHDERR_REQUIRES_PARENT:            return "requires parent";
                case chd_error.CHDERR_FILE_NOT_WRITEABLE:         return "file not writeable";
                case chd_error.CHDERR_READ_ERROR:                 return "read error";
                case chd_error.CHDERR_WRITE_ERROR:                return "write error";
                case chd_error.CHDERR_CODEC_ERROR:                return "codec error";
                case chd_error.CHDERR_INVALID_PARENT:             return "invalid parent";
                case chd_error.CHDERR_HUNK_OUT_OF_RANGE:          return "hunk out of range";
                case chd_error.CHDERR_DECOMPRESSION_ERROR:        return "decompression error";
                case chd_error.CHDERR_COMPRESSION_ERROR:          return "compression error";
                case chd_error.CHDERR_CANT_CREATE_FILE:           return "can't create file";
                case chd_error.CHDERR_CANT_VERIFY:                return "can't verify file";
                case chd_error.CHDERR_NOT_SUPPORTED:              return "operation not supported";
                case chd_error.CHDERR_METADATA_NOT_FOUND:         return "can't find metadata";
                case chd_error.CHDERR_INVALID_METADATA_SIZE:      return "invalid metadata size";
                case chd_error.CHDERR_UNSUPPORTED_VERSION:        return "mismatched DIFF and CHD or unsupported CHD version";
                case chd_error.CHDERR_VERIFY_INCOMPLETE:          return "incomplete verify";
                case chd_error.CHDERR_INVALID_METADATA:           return "invalid metadata";
                case chd_error.CHDERR_INVALID_STATE:              return "invalid state";
                case chd_error.CHDERR_OPERATION_PENDING:          return "operation pending";
                case chd_error.CHDERR_UNSUPPORTED_FORMAT:         return "unsupported format";
                case chd_error.CHDERR_UNKNOWN_COMPRESSION:        return "unknown compression type";
                case chd_error.CHDERR_WALKING_PARENT:             return "currently examining parent";
                case chd_error.CHDERR_COMPRESSING:                return "currently compressing";
                default:                                return "undocumented error";
            }
        }
    }
}
