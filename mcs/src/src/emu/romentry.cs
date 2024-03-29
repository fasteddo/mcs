// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u8 = System.Byte;
using u32 = System.UInt32;

using static mame.romentry_global;
using static mame.util;


namespace mame
{
    public static class romentry_global
    {
        /* ----- type constants ----- */
        public const u32 ROMENTRY_TYPEMASK           = 0x0000000f;          /* type of entry */
        //enum
        //{
        public const u32 ROMENTRYTYPE_ROM          =  0;     /* this entry is an actual ROM definition */
        public const u32 ROMENTRYTYPE_REGION       =  1;     /* this entry marks the start of a region */
        public const u32 ROMENTRYTYPE_END          =  2;     /* this entry marks the end of a region */
        public const u32 ROMENTRYTYPE_RELOAD       =  3;     /* this entry reloads the previous ROM */
        public const u32 ROMENTRYTYPE_CONTINUE     =  4;     /* this entry continues loading the previous ROM */
        public const u32 ROMENTRYTYPE_FILL         =  5;     /* this entry fills an area with a constant value */
        public const u32 ROMENTRYTYPE_COPY         =  6;     /* this entry copies data from another region/offset */
        const u32 ROMENTRYTYPE_CARTRIDGE    =  7;     /* this entry specifies a cartridge (MESS) */
        public const u32 ROMENTRYTYPE_IGNORE       =  8;     /* this entry continues loading the previous ROM but throws the data away */
        public const u32 ROMENTRYTYPE_SYSTEM_BIOS  =  9;     /* this entry specifies a bios */
        public const u32 ROMENTRYTYPE_DEFAULT_BIOS = 10;     /* this entry specifies a default bios */
        public const u32 ROMENTRYTYPE_PARAMETER    = 11;     /* this entry specifies a per-game parameter */
        const u32 ROMENTRYTYPE_COUNT        = 12;
        //}


        /* ----- per-region constants ----- */
        public const u32 ROMREGION_WIDTHMASK         = 0x00000300;          /* native width of region, as power of 2 */
        //#define     ROMREGION_8BIT          0x00000000          /*    (non-CPU regions only) */
        //#define     ROMREGION_16BIT         0x00000100
        public const u32 ROMREGION_32BIT             = 0x00000200;
        public const u32 ROMREGION_64BIT             = 0x00000300;

        public const u32 ROMREGION_ENDIANMASK        = 0x00000400;          /* endianness of the region */
        public const u32 ROMREGION_LE                = 0x00000000;          /*    (non-CPU regions only) */
        public const u32 ROMREGION_BE                = 0x00000400;

        public const u32 ROMREGION_INVERTMASK        = 0x00000800;          /* invert the bits of the region */
        //#define     ROMREGION_NOINVERT      0x00000000
        public const u32 ROMREGION_INVERT            = 0x00000800;

        public const u32 ROMREGION_ERASEMASK         = 0x00002000;          /* erase the region before loading */
        //#define     ROMREGION_NOERASE       0x00000000
        public const u32 ROMREGION_ERASE             = 0x00002000;

        public const u32 ROMREGION_DATATYPEMASK      = 0x00004000;          /* type of region (ROM versus disk) */
        public const u32 ROMREGION_DATATYPEROM       = 0x00000000;
        public const u32 ROMREGION_DATATYPEDISK      = 0x00004000;

        public const u32 ROMREGION_ERASEVALMASK      = 0x00ff0000;          /* value to erase the region to */
        public static u32 ROMREGION_ERASEVAL(u32 x) { return ((x & 0xff) << 16) | ROMREGION_ERASE; }
        public static readonly u32 ROMREGION_ERASE00 = ROMREGION_ERASEVAL(0);
        public static readonly u32 ROMREGION_ERASEFF = ROMREGION_ERASEVAL(0xff);


        /* ----- per-ROM constants ----- */
        public const u32 DISK_READONLYMASK           = 0x00000010;          /* is the disk read-only? */
        //#define     DISK_READWRITE          0x00000000
        public const u32 DISK_READONLY               = 0x00000010;

        public const u32 ROM_OPTIONALMASK            = 0x00000020;          /* optional - won't hurt if it's not there */
        //#define     ROM_REQUIRED            0x00000000
        public const u32 ROM_OPTIONAL                = 0x00000020;

        public const u32 ROM_REVERSEMASK             = 0x00000040;          /* reverse the byte order within a group */
        //#define     ROM_NOREVERSE           0x00000000
        public const u32 ROM_REVERSE                 = 0x00000040;

        public const u32 ROM_INHERITFLAGSMASK        = 0x00000080;          /* inherit all flags from previous definition */
        public const u32 ROM_INHERITFLAGS            = 0x00000080;

        public const u32 ROM_GROUPMASK               = 0x00000f00;          /* load data in groups of this size + 1 */
        //#define     ROM_GROUPSIZE(n)        ((((n) - 1) & 15) << 8)
        //#define     ROM_GROUPBYTE           ROM_GROUPSIZE(1)
        //#define     ROM_GROUPWORD           ROM_GROUPSIZE(2)
        //#define     ROM_GROUPDWORD          ROM_GROUPSIZE(4)

        public const u32 ROM_SKIPMASK                = 0x0000f000;          /* skip this many bytes after each group */
        public static u32 ROM_SKIP(u32 n) { return (n & 15) << 12; }
        //#define     ROM_NOSKIP              ROM_SKIP(0)

        public const u32 ROM_BITWIDTHMASK            = 0x000f0000;          /* width of data in bits */
        static u32 ROM_BITWIDTH(u32 n) { return (n & 15) << 16; }
        public static readonly u32 ROM_NIBBLE        = ROM_BITWIDTH(4);
        //#define     ROM_FULLBYTE            ROM_BITWIDTH(8)

        public const u32 ROM_BITSHIFTMASK            = 0x00f00000;          /* left-shift count for the bits */
        static u32 ROM_BITSHIFT(u32 n) { return (n & 15) << 20; }
        //#define     ROM_NOSHIFT             ROM_BITSHIFT(0)
        public static readonly u32 ROM_SHIFT_NIBBLE_LO = ROM_BITSHIFT(0);
        public static readonly u32 ROM_SHIFT_NIBBLE_HI = ROM_BITSHIFT(4);

        public const u32 ROM_BIOSFLAGSMASK           = 0xff000000;          /* only loaded if value matches device bios value */
        //#define     ROM_BIOS(n)             ((((n) + 1) & 255) << 24)

        public const u32 ROM_INHERITEDFLAGS          = (ROM_GROUPMASK | ROM_SKIPMASK | ROM_REVERSEMASK | ROM_BITWIDTHMASK | ROM_BITSHIFTMASK | ROM_BIOSFLAGSMASK);


        /* ----- start/stop macros ----- */
        //#define ROM_NAME(name)                              rom_##name
        //#define ROM_START(name)                             static const tiny_rom_entry ROM_NAME(name)[] = {
        public static tiny_rom_entry ROM_END { get { return new tiny_rom_entry(null, null, 0, 0, ROMENTRYTYPE_END); } }


        /* ----- ROM region macros ----- */
        public static tiny_rom_entry ROM_REGION(u32 length, string tag, u32 flags) { return new tiny_rom_entry(tag, null, 0, length, ROMENTRYTYPE_REGION | flags); }
        //#define ROM_REGION16_LE(length,tag,flags)           ROM_REGION(length, tag, (flags) | ROMREGION_16BIT | ROMREGION_LE)
        //#define ROM_REGION16_BE(length,tag,flags)           ROM_REGION(length, tag, (flags) | ROMREGION_16BIT | ROMREGION_BE)
        //#define ROM_REGION32_LE(length,tag,flags)           ROM_REGION(length, tag, (flags) | ROMREGION_32BIT | ROMREGION_LE)
        public static tiny_rom_entry ROM_REGION32_BE(u32 length, string tag, u32 flags) { return ROM_REGION(length, tag, (flags) | ROMREGION_32BIT | ROMREGION_BE); }
        public static tiny_rom_entry ROM_REGION64_LE(u32 length, string tag, u32 flags) { return ROM_REGION(length, tag, (flags) | ROMREGION_64BIT | ROMREGION_LE); }
        //#define ROM_REGION64_BE(length,tag,flags)           ROM_REGION(length, tag, (flags) | ROMREGION_64BIT | ROMREGION_BE)


        /* ----- core ROM loading macros ----- */
        public static tiny_rom_entry ROMX_LOAD(string name, u32 offset, u32 length, string hash, u32 flags) { return new tiny_rom_entry(name, hash, offset, length, ROMENTRYTYPE_ROM | flags); }
        public static tiny_rom_entry ROM_LOAD(string name, u32 offset, u32 length, string hash) { return ROMX_LOAD(name, offset, length, hash, 0); }
        //#define ROM_LOAD_OPTIONAL(name,offset,length,hash)  ROMX_LOAD(name, offset, length, hash, ROM_OPTIONAL)


        /* ----- specialized loading macros ----- */
        public static tiny_rom_entry ROM_LOAD_NIB_HIGH(string name, u32 offset, u32 length, string hash) { return ROMX_LOAD(name, offset, length, hash, ROM_NIBBLE | ROM_SHIFT_NIBBLE_HI); }
        public static tiny_rom_entry ROM_LOAD_NIB_LOW(string name, u32 offset, u32 length, string hash) { return ROMX_LOAD(name, offset, length, hash, ROM_NIBBLE | ROM_SHIFT_NIBBLE_LO); }
        public static tiny_rom_entry ROM_LOAD16_BYTE(string name, u32 offset, u32 length, string hash) { return ROMX_LOAD(name, offset, length, hash, ROM_SKIP(1)); }
        //#define ROM_LOAD16_WORD(name,offset,length,hash)        ROM_LOAD(name, offset, length, hash)
        //#define ROM_LOAD16_WORD_SWAP(name,offset,length,hash)   ROMX_LOAD(name, offset, length, hash, ROM_GROUPWORD | ROM_REVERSE)
        //#define ROM_LOAD32_BYTE(name,offset,length,hash)        ROMX_LOAD(name, offset, length, hash, ROM_SKIP(3))
        //#define ROM_LOAD32_WORD(name,offset,length,hash)        ROMX_LOAD(name, offset, length, hash, ROM_GROUPWORD | ROM_SKIP(2))
        //#define ROM_LOAD32_WORD_SWAP(name,offset,length,hash)   ROMX_LOAD(name, offset, length, hash, ROM_GROUPWORD | ROM_REVERSE | ROM_SKIP(2))
        //#define ROM_LOAD32_DWORD(name,offset,length,hash)       ROMX_LOAD(name, offset, length, hash, ROM_GROUPDWORD)
        //#define ROM_LOAD64_BYTE(name,offset,length,hash)        ROMX_LOAD(name, offset, length, hash, ROM_SKIP(7))
        //#define ROM_LOAD64_WORD(name,offset,length,hash)        ROMX_LOAD(name, offset, length, hash, ROM_GROUPWORD | ROM_SKIP(6))
        //#define ROM_LOAD64_WORD_SWAP(name,offset,length,hash)   ROMX_LOAD(name, offset, length, hash, ROM_GROUPWORD | ROM_REVERSE | ROM_SKIP(6))
        //#define ROM_LOAD64_DWORD_SWAP(name,offset,length,hash)  ROMX_LOAD(name, offset, length, hash, ROM_GROUPDWORD | ROM_REVERSE | ROM_SKIP(4))


        /* ----- ROM_RELOAD related macros ----- */
        public static tiny_rom_entry ROM_RELOAD(u32 offset, u32 length) { return new tiny_rom_entry(null, null, offset, length, ROMENTRYTYPE_RELOAD | ROM_INHERITFLAGS); }
        //#define ROM_RELOAD_PLAIN(offset,length)             { nullptr, nullptr, offset, length, ROMENTRYTYPE_RELOAD },

        /* ----- additional ROM-related macros ----- */
        public static tiny_rom_entry ROM_CONTINUE(u32 offset, u32 length) { return new tiny_rom_entry(null, null, offset, length, ROMENTRYTYPE_CONTINUE | ROM_INHERITFLAGS); }
        //#define ROM_IGNORE(length)                          { nullptr,  nullptr,                 0,        (length), ROMENTRYTYPE_IGNORE | ROM_INHERITFLAGS },
        public static tiny_rom_entry ROM_FILL(u32 offset, u32 length, u8 value) { return new tiny_rom_entry(null, value.ToString(), offset, length, ROMENTRYTYPE_FILL); }
        //#define ROMX_FILL(offset,length,value,flags)        { nullptr,  (const char *)(value),   (offset), (length), ROMENTRYTYPE_FILL | flags },
        //#define ROM_COPY(srctag,srcoffs,offset,length)      { (srctag), (const char *)(srcoffs), (offset), (length), ROMENTRYTYPE_COPY },


        /* ----- system BIOS macros ----- */
        //#define ROM_SYSTEM_BIOS(value,name,description)     { name, description, 0, 0, ROMENTRYTYPE_SYSTEM_BIOS | ROM_BIOS(value) },
        //#define ROM_DEFAULT_BIOS(name)                      { name, nullptr,     0, 0, ROMENTRYTYPE_DEFAULT_BIOS },


        /* ----- game parameter macro ----- */
        //#define ROM_PARAMETER(tag, value)                   { tag, value, 0, 0, ROMENTRYTYPE_PARAMETER },

        /* ----- disk loading macros ----- */
        //#define DISK_REGION(tag)                            ROM_REGION(1, tag, ROMREGION_DATATYPEDISK)
        //#define DISK_IMAGE(name,idx,hash)                   ROMX_LOAD(name, idx, 0, hash, DISK_READWRITE)
        //#define DISK_IMAGE_READONLY(name,idx,hash)          ROMX_LOAD(name, idx, 0, hash, DISK_READONLY)
        //#define DISK_IMAGE_READONLY_OPTIONAL(name,idx,hash) ROMX_LOAD(name, idx, 0, hash, DISK_READONLY | ROM_OPTIONAL)
    }


    public interface rom_entry_interface
    {
        string name();
        string hashdata();
        u32 get_offset();
        u32 get_length();
        u32 get_flags();
    }


    // ======================> tiny_rom_entry
    public class tiny_rom_entry : rom_entry_interface
    {
        public string name_;
        public string hashdata_;
        public u32 offset;
        public u32 length;
        public u32 flags;

        public tiny_rom_entry() { }
        public tiny_rom_entry(string name, string hashdata, u32 offset, u32 length, u32 flags) { this.name_ = name; this.hashdata_ = hashdata; this.offset = offset; this.length = length; this.flags = flags; }


        public string name() { return name_; }
        public string hashdata() { return hashdata_; }
        public u32 get_offset() { return offset; }
        public u32 get_length() { return length; }
        public u32 get_flags() { return flags; }
    }


    // ======================> rom_entry
    public class rom_entry : rom_entry_interface
    {
        string m_name;
        string m_hashdata;
        u32 m_offset;
        u32 m_length;
        u32 m_flags;


        //-------------------------------------------------
        //  ctor (with move constructors)
        //-------------------------------------------------
        public rom_entry(string name, string hashdata, u32 offset, u32 length, u32 flags)
        {
            m_name = name;
            m_hashdata = hashdata;
            m_offset = offset;
            m_length = length;
            m_flags = flags;
        }


        //-------------------------------------------------
        //  ctor (with tiny_rom_entry)
        //-------------------------------------------------
        public rom_entry(tiny_rom_entry ent)
        {
            m_name = ent.name_ != null ? ent.name_ : "";
            m_hashdata = hashdata_from_tiny_rom_entry(ent);
            m_offset = ent.offset;
            m_length = ent.length;
            m_flags = ent.flags;
        }

        //rom_entry(rom_entry const &) = default;
        //rom_entry(rom_entry &&) = default;
        public rom_entry(rom_entry ent)
        {
            this.m_name = ent.m_name;
            this.m_hashdata = ent.m_hashdata;
            this.m_offset = ent.m_offset;
            this.m_length = ent.m_length;
            this.m_flags = ent.m_flags;
        }

        //rom_entry &operator=(rom_entry const &) = default;
        //rom_entry &operator=(rom_entry &&) = default;


        // accessors
        public string name() { return m_name; }
        public string hashdata() { return m_hashdata; }
        public u32 get_offset() { return m_offset; }
        public u32 get_length() { return m_length; }
        public u32 get_flags() { return m_flags; }
        public void set_flags(u32 flags) { m_flags = flags; }


        //-------------------------------------------------
        //  hashdata_from_tiny_rom_entry - calculates the
        //  proper hashdata string from the value in the
        //  tiny_rom_entry
        //-------------------------------------------------
        static string hashdata_from_tiny_rom_entry(tiny_rom_entry ent)
        {
            string result = "";
            switch (ent.flags & ROMENTRY_TYPEMASK)
            {
                case ROMENTRYTYPE_FILL:
                case ROMENTRYTYPE_COPY:
                    // for these types, tiny_rom_entry::hashdata is an integer typecasted to a pointer
                    result = string_format("0x{0}", ent.hashdata_);  //result = string_format("0x%x", (unsigned)(uintptr_t)ent.hashdata);
                    break;

                default:
                    if (ent.hashdata_ != null)
                        result = ent.hashdata_;
                    break;
            }

            return result;
        }
    }
}
