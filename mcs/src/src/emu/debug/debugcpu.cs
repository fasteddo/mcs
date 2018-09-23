// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;


namespace mame
{
    public static class debugcpu_global
    {
        /* ----- debugging status & information ----- */

        /* return the visible CPU device (the one that commands should apply to) */
        //device_t *debug_cpu_get_visible_cpu(running_machine &machine);

        /* TRUE if the debugger is currently stopped within an instruction hook callback */
        /*-------------------------------------------------
            debug_cpu_within_instruction_hook - true if
            the debugger is currently live
        -------------------------------------------------*/
        public static int debug_cpu_within_instruction_hook(running_machine machine)
        {
            //throw new emu_unimplemented();
            return 0;
        }

        /* return TRUE if the current execution state is stopped */
        //int debug_cpu_is_stopped(running_machine &machine);


        /* ----- debugger comment helpers ----- */

        // save all comments for a given machine
        //-------------------------------------------------
        //  debug_comment_save - save all comments for
        //  the given machine
        //-------------------------------------------------
        public static bool debug_comment_save(running_machine machine)
        {
            //throw new emu_unimplemented();
            return false;
        }


        // load all comments for a given machine
        //bool debug_comment_load(running_machine &machine);


        /* ----- debugger memory accessors ----- */

        /* return the physical address corresponding to the given logical address */
        //int debug_cpu_translate(address_space &space, int intention, offs_t *address);

        /* return a byte from the specified memory space */
        //UINT8 debug_read_byte(address_space &space, offs_t address, int apply_translation);

        /* return a word from the specified memory space */
        //UINT16 debug_read_word(address_space &space, offs_t address, int apply_translation);

        /* return a dword from the specified memory space */
        //UINT32 debug_read_dword(address_space &space, offs_t address, int apply_translation);

        /* return a qword from the specified memory space */
        //UINT64 debug_read_qword(address_space &space, offs_t address, int apply_translation);

        /* return 1,2,4 or 8 bytes from the specified memory space */
        //UINT64 debug_read_memory(address_space &space, offs_t address, int size, int apply_translation);

        /* write a byte to the specified memory space */
        //void debug_write_byte(address_space &space, offs_t address, UINT8 data, int apply_translation);

        /* write a word to the specified memory space */
        //void debug_write_word(address_space &space, offs_t address, UINT16 data, int apply_translation);

        /* write a dword to the specified memory space */
        //void debug_write_dword(address_space &space, offs_t address, UINT32 data, int apply_translation);

        /* write a qword to the specified memory space */
        //void debug_write_qword(address_space &space, offs_t address, UINT64 data, int apply_translation);

        /* write 1,2,4 or 8 bytes to the specified memory space */
        //void debug_write_memory(address_space &space, offs_t address, UINT64 data, int size, int apply_translation);

        /* read 1,2,4 or 8 bytes at the given offset from opcode space */
        //UINT64 debug_read_opcode(address_space &space, offs_t offset, int size);
    }


    // ======================> device_debug
    public class device_debug
    {
        // construction/destruction
        //-------------------------------------------------
        //  device_debug - constructor
        //-------------------------------------------------
        public device_debug(device_t device)
        {
            throw new emu_unimplemented();
        }

        //-------------------------------------------------
        //  ~device_debug - constructor
        //-------------------------------------------------
        ~device_debug()
        {
            throw new emu_unimplemented();
        }


        // getters
        //symbol_table &symtable() { return m_symtable; }


        // commonly-used pass-throughs
        //int logaddrchars(address_spacenum spacenum = AS_0) const { return (m_memory != NULL && m_memory->has_space(spacenum)) ? m_memory->space(spacenum).logaddrchars() : 8; }
        //bool is_octal() const { return (m_memory != nullptr && m_memory->has_space(AS_PROGRAM)) ? m_memory->space(AS_PROGRAM).is_octal() : false; }
        //device_t& device() const { return m_device; }


        // hooks used by the rest of the system

        public void start_hook(attotime endtime)
        {
            throw new emu_unimplemented();
        }

        public void stop_hook()
        {
            throw new emu_unimplemented();
        }

        public void interrupt_hook(int irqline)
        {
            throw new emu_unimplemented();
        }

        //void exception_hook(int exception);

        //-------------------------------------------------
        //  instruction_hook - called by the CPU cores
        //  before executing each instruction
        //-------------------------------------------------
        public void instruction_hook(offs_t curpc)
        {
            throw new emu_unimplemented();
        }


        // hooks into our operations
        //void set_instruction_hook(debug_instruction_hook_func hook);
        //void set_dasm_override(dasm_override_func dasm_override) { m_dasm_override = dasm_override; }


        // disassembly
        //offs_t disassemble(char *buffer, offs_t pc, const UINT8 *oprom, const UINT8 *opram) const;


        // debugger focus
        //void ignore(bool ignore = true);
        //bool observing() const { return ((m_flags & DEBUG_FLAG_OBSERVING) != 0); }


        // single stepping
        //void single_step(int numsteps = 1);
        //void single_step_over(int numsteps = 1);
        //void single_step_out();


        // execution
        //void go(offs_t targetpc = ~0);
        //void go_vblank();
        //void go_interrupt(int irqline = -1);
        //void go_exception(int exception);
        //void go_milliseconds(UINT64 milliseconds);
        //void go_next_device();
        //void halt_on_next_instruction(const char *fmt, ...) ATTR_PRINTF(2,3);


        // breakpoints
        //breakpoint *breakpoint_first() const { return m_bplist; }
        //int breakpoint_set(offs_t address, const char *condition = NULL, const char *action = NULL);
        //bool breakpoint_clear(int index);
        //void breakpoint_clear_all();
        //bool breakpoint_enable(int index, bool enable = true);
        //void breakpoint_enable_all(bool enable = true);


        // watchpoints
        //const std::vector<std::unique_ptr<watchpoint>> &watchpoint_vector(int spacenum) const { return m_wplist[spacenum]; }
        //int watchpoint_set(address_space &space, read_or_write type, offs_t address, offs_t length, const char *condition, const char *action);
        //bool watchpoint_clear(int wpnum);
        //void watchpoint_clear_all();
        //bool watchpoint_enable(int index, bool enable = true);
        //void watchpoint_enable_all(bool enable = true);


        // registerpoints
        //registerpoint *registerpoint_first() const { return m_rplist; }
        //int registerpoint_set(const char *condition, const char *action = NULL);
        //bool registerpoint_clear(int index);
        //void registerpoint_clear_all();
        //bool registerpoint_enable(int index, bool enable = true);
        //void registerpoint_enable_all(bool enable = true );


        // hotspots
        //bool hotspot_tracking_enabled() const { return (m_hotspots != NULL); }
        //void hotspot_track(int numspots, int threshhold);


        // comments
        //void comment_add(offs_t address, const char *comment, rgb_t color);
        //bool comment_remove(offs_t addr);
        //const char *comment_text(offs_t addr) const;
        //UINT32 comment_count() const { return m_comment_set.size(); }
        //UINT32 comment_change_count() const { return m_comment_change; }
        //bool comment_export(xml_data_node &node);
        //bool comment_import(xml_data_node &node);
        //UINT32 compute_opcode_crc32(offs_t pc) const;


        // history
        //offs_t history_pc(int index) const;


        // pc tracking
        //void set_track_pc(bool value) { m_track_pc = value; }
        //bool track_pc_visited(const offs_t& pc) const;
        //void set_track_pc_visited(const offs_t& pc);
        //void track_pc_data_clear() { m_track_pc_set.clear(); }


        // memory tracking
        //void set_track_mem(bool value) { m_track_mem = value; }
        //offs_t track_mem_pc_from_space_address_data(const address_spacenum& space,
        //                                            const offs_t& address,
        //                                            const UINT64& data) const;
        //void track_mem_data_clear() { m_track_mem_set.clear(); }


        // tracing
        //void trace(FILE *file, bool trace_over, bool logerror, const char *action);
        //void trace_printf(const char *fmt, ...) ATTR_PRINTF(2,3);
        //void trace_flush() { if (m_trace != NULL) m_trace->flush(); }


        //void reset_transient_flag() { m_flags &= ~DEBUG_FLAG_TRANSIENT; }


        //void halt_on_next_instruction_impl(util::format_argument_pack<std::ostream> &&args);


        // internal helpers
        //void compute_debug_flags();
        //void prepare_for_step_overout(offs_t pc);
        //void errorlog_write_line(const char *line);


        // breakpoint and watchpoint helpers
        //void breakpoint_update_flags();
        //void breakpoint_check(offs_t pc);
        //void hotspot_check(address_space &space, offs_t address);
        //void reinstall_all(read_or_write mode);
        //void reinstall(address_space &space, read_or_write mode);
        //void write_tracking(address_space &space, offs_t address, u64 data);


        // symbol get/set callbacks
        //static u64 get_current_pc(symbol_table &table);
        //static u64 get_cycles(symbol_table &table);
        //static u64 get_totalcycles(symbol_table &table);
        //static u64 get_lastinstructioncycles(symbol_table &table);
        //static u64 get_state(symbol_table &table, int index);
        //static void set_state(symbol_table &table, int index, u64 value);
    }


    public class debugcpu_private
    {
#if false
        device_t *livecpu;
        device_t *visiblecpu;
        device_t *breakcpu;

        symbol_table *  symtable;                   /* global symbol table */

        bool            within_instruction_hook;
        bool            vblank_occurred;
        bool            memory_modified;

        int             execution_state;
        device_t *      m_stop_when_not_device;     // stop execution when the device ceases to be this

        UINT32          bpindex;
        UINT32          wpindex;
        UINT32          rpindex;

        UINT64          wpdata;
        UINT64          wpaddr;
        UINT64          tempvar[NUM_TEMP_VARIABLES];

        osd_ticks_t     last_periodic_update_time;

        bool            comments_loaded;
#endif
    }
}
