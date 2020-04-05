// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.IO;
using int32_t = System.Int32;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using osd_ticks_t = System.UInt64;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame
{
    // output channel callback
    //typedef delegate<void (const char *, va_list)> output_delegate;
    public delegate void output_delegate(string format, params object [] args);


    // output channels
    public enum osd_output_channel
    {
        OSD_OUTPUT_CHANNEL_ERROR,
        OSD_OUTPUT_CHANNEL_WARNING,
        OSD_OUTPUT_CHANNEL_INFO,
        OSD_OUTPUT_CHANNEL_DEBUG,
        OSD_OUTPUT_CHANNEL_VERBOSE,
        OSD_OUTPUT_CHANNEL_LOG,
        OSD_OUTPUT_CHANNEL_COUNT
    }


    public static class osdcore_global
    {
        /* Make sure we have a path separator (default to /) */
        public const string PATH_SEPARATOR         = "/";

        /* flags controlling file access */
        public const UInt32 OPEN_FLAG_READ         = 0x0001;      /* open for read */
        public const UInt32 OPEN_FLAG_WRITE        = 0x0002;      /* open for write */
        public const UInt32 OPEN_FLAG_CREATE       = 0x0004;      /* create & truncate file */
        public const UInt32 OPEN_FLAG_CREATE_PATHS = 0x0008;      /* create paths as necessary */
        public const UInt32 OPEN_FLAG_NO_PRELOAD   = 0x0010;      /* do not decompress on open */


        public static osdcore_interface m_osdcore;
        public static osd_file m_osdfile;
        public static osd.directory_static m_osddirectory;


        public static void set_osdcore(osdcore_interface osdcore) { m_osdcore = osdcore; }
        public static void set_osdfile(osd_file osdfile) { m_osdfile = osdfile; }
        public static void set_osddirectory(osd.directory_static osddirectory) { m_osddirectory = osddirectory; }
    }


    /* osd_work_queue is an opaque type which represents a queue of work items */
    // defined in osdsync.cs
    //public abstract class osd_work_queue { }

    /* osd_work_item is an opaque type which represents a single work item */
    // defined in osdsync.cs
    //public abstract class osd_work_item { }

    /* osd_work_callback is a callback function that does work */
    //typedef void *(*osd_work_callback)(void *param, int threadid);
    public delegate Object osd_work_callback(Object param, int threadid);


    // osd_file is an interface which represents an open file/PTY/socket
    public abstract class osd_file
    {
        // error codes returned by routines below
        public enum error
        {
            NONE,
            FAILURE,
            OUT_OF_MEMORY,
            NOT_FOUND,
            ACCESS_DENIED,
            ALREADY_OPEN,
            TOO_MANY_FILES,
            INVALID_DATA,
            INVALID_ACCESS
        }

        //typedef std::unique_ptr<osd_file> ptr;


        /*-----------------------------------------------------------------------------
            osd_file::open: open a new file.

            Parameters:

                path - path to the file to open

                openflags - some combination of:

                    OPEN_FLAG_READ - open the file for read access
                    OPEN_FLAG_WRITE - open the file for write access
                    OPEN_FLAG_CREATE - create/truncate the file when opening
                    OPEN_FLAG_CREATE_PATHS - specifies that non-existant paths
                            should be created if necessary

                file - reference to an osd_file::ptr to receive the newly-opened file
                    handle; this is only valid if the function returns FILERR_NONE

                filesize - reference to a UINT64 to receive the size of the opened
                    file; this is only valid if the function returns FILERR_NONE

            Return value:

                a file_error describing any error that occurred while opening
                the file, or FILERR_NONE if no error occurred

            Notes:

                This function is called by core_fopen and several other places in
                the core to access files. These functions will construct paths by
                concatenating various search paths held in the options.c options
                database with partial paths specified by the core. The core assumes
                that the path separator is the first character of the string
                PATH_SEPARATOR, but does not interpret any path separators in the
                search paths, so if you use a different path separator in a search
                path, you may get a mixture of PATH_SEPARATORs (from the core) and
                alternate path separators (specified by users and placed into the
                options database).
        -----------------------------------------------------------------------------*/
        public abstract error open(string path, uint32_t openflags, out osd_file file, out uint64_t filesize);


        /*-----------------------------------------------------------------------------
            osd_file::openpty: create a new PTY pair

            Parameters:

                file - reference to an osd_file::ptr to receive the handle of the master
                    side of the newly-created PTY; this is only valid if the function
                    returns FILERR_NONE

                name - reference to string where slave filename will be stored

            Return value:

                a file_error describing any error that occurred while creating the
                PTY, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        protected abstract error openpty(out osd_file file, out string name);


        /*-----------------------------------------------------------------------------
            osd_file::~osd_file: close an open file
        -----------------------------------------------------------------------------*/
        //~osd_file() { }


        /*-----------------------------------------------------------------------------
            osd_file::read: read from an open file

            Parameters:

                buffer - pointer to memory that will receive the data read

                offset - offset within the file to read from

                length - number of bytes to read from the file

                actual - reference to a UINT32 to receive the number of bytes actually
                    read during the operation; valid only if the function returns
                    FILERR_NONE

            Return value:

                a file_error describing any error that occurred while reading
                from the file, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        public abstract error read(ListBytesPointer buffer, uint64_t offset, uint32_t length, out uint32_t actual);  //void *buffer

        /*-----------------------------------------------------------------------------
            osd_file::write: write to an open file

            Parameters:

                buffer - pointer to memory that contains the data to write

                offset - offset within the file to write to

                length - number of bytes to write to the file

                actual - reference to a UINT32 to receive the number of bytes actually
                    written during the operation; valid only if the function returns
                    FILERR_NONE

            Return value:

                a file_error describing any error that occurred while writing to
                the file, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        public abstract error write(ListBytesPointer buffer, uint64_t offset, uint32_t length, out uint32_t actual);  //void const *buffer


        /*-----------------------------------------------------------------------------
            osd_file::truncate: change the size of an open file

            Parameters:

    .           offset - future size of the file

            Return value:

                a file_error describing any error that occurred while writing to
                the file, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        //abstract error truncate(UInt64 offset);


        /*-----------------------------------------------------------------------------
            osd_file::flush: flush file buffers

            Parameters:

                file - handle to a file previously opened via osd_open

            Return value:

                a file_error describing any error that occurred while flushing file
                buffers, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        //abstract error flush();


        /*-----------------------------------------------------------------------------
            osd_file::remove: deletes a file

            Parameters:

                filename - path to file to delete

            Return value:

                a file_error describing any error that occurred while deleting
                the file, or FILERR_NONE if no error occurred
        -----------------------------------------------------------------------------*/
        public abstract error remove(string filename);


        public abstract Stream stream { get; }
    }


    namespace osd
    {
        // directory is an opaque type which represents an open directory
        public abstract class directory
        {
            //typedef std::unique_ptr<directory> ptr;

            // osd::directory::entry contains basic information about a file when iterating through
            // a directory
            public class entry
            {
                public enum entry_type
                {
                    NONE,
                    FILE,
                    DIR,
                    OTHER
                }

                public string name;           // name of the entry
                public entry_type type;           // type of the entry
                uint64_t size;           // size of the entry
                //std::chrono::system_clock::time_point   last_modified;  // last modified time
            }


            // -----------------------------------------------------------------------------
            // osd::directory::~directory: close an open directory
            // -----------------------------------------------------------------------------
            //virtual ~directory() { }

            // -----------------------------------------------------------------------------
            // osd::directory::read: return information about the next entry in the directory
            //
            // Return value:
            //
            // a constant pointer to an entry representing the current item
            // in the directory, or nullptr, indicating that no more entries are
            // present
            // -----------------------------------------------------------------------------
            public abstract entry read();
        }


        public abstract class directory_static
        {
            // -----------------------------------------------------------------------------
            // osd::directory::open: open a directory for iteration
            //
            // Parameters:
            //
            // dirname - path to the directory in question
            //
            // Return value:
            //
            // upon success, this function should return an directory pointer
            // which contains opaque data necessary to traverse the directory; on
            // failure, this function should return nullptr
            // -----------------------------------------------------------------------------
            public abstract directory open(string dirname);  //static ptr open(std::string const &dirname);
        }
    }


    public abstract class osdcore_interface
    {
        /* a osd_ticks_t is a 64-bit unsigned integer that is used as a core type in timing interfaces */
        //typedef UINT64 osd_ticks_t;


        /***************************************************************************
            WORK ITEM INTERFACES
        ***************************************************************************/

        /* this is the maximum number of supported threads for a single work queue */
        /* threadid values are expected to range from 0..WORK_MAX_THREADS-1 */
        public const int WORK_MAX_THREADS            = 16;

        /* these flags can be set when creating a queue to give hints to the code about
           how to configure the queue */
        public const UInt32 WORK_QUEUE_FLAG_IO        = 0x0001;
        public const UInt32 WORK_QUEUE_FLAG_MULTI     = 0x0002;
        public const UInt32 WORK_QUEUE_FLAG_HIGH_FREQ = 0x0004;

        /* these flags can be set when queueing a work item to indicate how to handle
           its deconstruction */
        public const UInt32 WORK_ITEM_FLAG_AUTO_RELEASE = 0x0001;


        /****************************************************************************

            The prototypes in this file describe the interfaces that the MAME core
            and various tools rely upon to interact with the outside world. They are
            broken out into several categories.

        ***************************************************************************/


        /*-----------------------------------------------------------------------------
            osd_getenv: return pointer to environment variable

            Parameters:

                name  - name of environment variable

            Return value:

                pointer to value
        -----------------------------------------------------------------------------*/
        public virtual string osd_getenv(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }


        /*-----------------------------------------------------------------------------
            osd_getpid: gets process id

            Return value:

                process id
        -----------------------------------------------------------------------------*/
        //int osd_getpid();


        /*-----------------------------------------------------------------------------
            osd_get_physical_drive_geometry: if the given path points to a physical
                drive, return the geometry of that drive

            Parameters:

                filename - pointer to a path which might describe a physical drive

                cylinders - pointer to a UINT32 to receive the number of cylinders
                    of the physical drive

                heads - pointer to a UINT32 to receive the number of heads per
                    cylinder of the physical drive

                sectors - pointer to a UINT32 to receive the number of sectors per
                    cylinder of the physical drive

                bps - pointer to a UINT32 to receive the number of bytes per sector
                    of the physical drive

            Return value:

                TRUE if the filename points to a physical drive and if the values
                pointed to by cylinders, heads, sectors, and bps are valid; FALSE in
                any other case
        -----------------------------------------------------------------------------*/
        //bool osd_get_physical_drive_geometry(const char *filename, uint32_t *cylinders, uint32_t *heads, uint32_t *sectors, uint32_t *bps);


        /*-----------------------------------------------------------------------------
            osd_uchar_from_osdchar: convert the given character or sequence of
                characters from the OS-default encoding to a Unicode character

            Parameters:

                uchar - pointer to a UINT32 to receive the resulting unicode
                    character

                osdchar - pointer to one or more chars that are in the OS-default
                    encoding

                count - number of characters provided in the OS-default encoding

            Return value:

                The number of characters required to form a Unicode character.
        -----------------------------------------------------------------------------*/
        //int osd_uchar_from_osdchar(char32_t *uchar, const char *osdchar, size_t count);


        /*-----------------------------------------------------------------------------
            osd_is_valid_filename_char: is the given character legal for filenames?

            Parameters:

                uchar - the character to check

            Return value:

                Whether this character is legal in a filename
        -----------------------------------------------------------------------------*/
        //bool osd_is_valid_filename_char(char32_t uchar);


        /*-----------------------------------------------------------------------------
            osd_is_valid_filepath_char: is the given character legal for paths?

            Parameters:

                uchar - the character to check

            Return value:

                Whether this character is legal in a file path
        -----------------------------------------------------------------------------*/
        //bool osd_is_valid_filepath_char(char32_t uchar);



        /***************************************************************************
            DIRECTORY INTERFACES
        ***************************************************************************/

        /*-----------------------------------------------------------------------------
            osd_is_absolute_path: returns whether the specified path is absolute

            Parameters:

                path - the path in question

            Return value:

                non-zero if the path is absolute, zero otherwise
        -----------------------------------------------------------------------------*/
        //bool osd_is_absolute_path(const char *path);



        /***************************************************************************
            TIMING INTERFACES
        ***************************************************************************/

        /*-----------------------------------------------------------------------------
            osd_ticks: return the current running tick counter

            Parameters:

                None

            Return value:

                an osd_ticks_t value which represents the current tick counter

            Notes:

                The resolution of this counter should be 1ms or better for accurate
                performance. It is also important that the source of this timer be
                accurate. It is ok if this call is not ultra-fast, since it is
                primarily used for once/frame synchronization.
        -----------------------------------------------------------------------------*/
        public virtual osd_ticks_t osd_ticks() { return (osd_ticks_t)DateTime.UtcNow.Ticks; }


        /*-----------------------------------------------------------------------------
            osd_ticks_per_second: return the number of ticks per second

            Parameters:

                None

            Return value:

                an osd_ticks_t value which represents the number of ticks per
                second
        -----------------------------------------------------------------------------*/
        public virtual osd_ticks_t osd_ticks_per_second() { return TimeSpan.TicksPerSecond; }


        /*-----------------------------------------------------------------------------
            osd_sleep: sleep for the specified time interval

            Parameters:

                duration - an osd_ticks_t value that specifies how long we should
                    sleep

            Return value:

                None

            Notes:

                The OSD layer should try to sleep for as close to the specified
                duration as possible, or less. This is used as a mechanism to
                "give back" unneeded time to other programs running in the system.
                On a simple, non multitasking system, this routine can be a no-op.
                If there is significant volatility in the amount of time that the
                sleep occurs for, the OSD layer should strive to sleep for less time
                than specified rather than sleeping too long.
        -----------------------------------------------------------------------------*/
        public abstract void osd_sleep(osd_ticks_t duration);



        /***************************************************************************
            WORK ITEM INTERFACES
        ***************************************************************************/

        /*-----------------------------------------------------------------------------
            osd_work_queue_alloc: create a new work queue

            Parameters:

                flags - one or more of the WORK_QUEUE_FLAG_* values ORed together:

                    WORK_QUEUE_FLAG_IO - indicates that the work queue will do some
                        I/O; this may be a useful hint so that threads are created
                        even on single-processor systems since I/O can often be
                        overlapped with other work

                    WORK_QUEUE_FLAG_MULTI - indicates that the work queue should
                        take advantage of as many processors as it can; items queued
                        here are assumed to be fully independent or shared

                    WORK_QUEUE_FLAG_HIGH_FREQ - indicates that items are expected
                        to be queued at high frequency and acted upon quickly; in
                        general, this implies doing some spin-waiting internally
                        before falling back to OS-specific synchronization

            Return value:

                A pointer to an allocated osd_work_queue object.

            Notes:

                A work queue abstracts the notion of how potentially threaded work
                can be performed. If no threading support is available, it is a
                simple matter to execute the work items as they are queued.
        -----------------------------------------------------------------------------*/
        public abstract osd_work_queue osd_work_queue_alloc(int flags);


        /*-----------------------------------------------------------------------------
            osd_work_queue_items: return the number of pending items in the queue

            Parameters:

                queue - pointer to an osd_work_queue that was previously created via
                    osd_work_queue_alloc

            Return value:

                The number of incomplete items remaining in the queue.
        -----------------------------------------------------------------------------*/
        //int osd_work_queue_items(osd_work_queue *queue);


        /*-----------------------------------------------------------------------------
            osd_work_queue_wait: wait for the queue to be empty

            Parameters:

                queue - pointer to an osd_work_queue that was previously created via
                    osd_work_queue_alloc

                timeout - a timeout value in osd_ticks_per_second()

            Return value:

                true if the queue is empty; false if the wait timed out before the
                queue was emptied.
        -----------------------------------------------------------------------------*/
        public abstract bool osd_work_queue_wait(osd_work_queue queue, osd_ticks_t timeout);


        /*-----------------------------------------------------------------------------
            osd_work_queue_free: free a work queue, waiting for all items to complete

            Parameters:

                queue - pointer to an osd_work_queue that was previously created via
                    osd_work_queue_alloc

            Return value:

                None.
        -----------------------------------------------------------------------------*/
        public abstract void osd_work_queue_free(osd_work_queue queue);


        /*-----------------------------------------------------------------------------
            osd_work_item_queue_multiple: queue a set of work items

            Parameters:

                queue - pointer to an osd_work_queue that was previously created via
                    osd_work_queue_alloc

                callback - pointer to a function that will do the work

                numitems - number of work items to queue

                param - a void * parameter that can be used to pass data to the
                    function

                paramstep - the number of bytes to increment param by for each item
                    queued; for example, if you have an array of work_unit objects,
                    you can point param to the base of the array and set paramstep to
                    sizeof(work_unit)

                flags - one or more of the WORK_ITEM_FLAG_* values ORed together:

                    WORK_ITEM_FLAG_AUTO_RELEASE - indicates that the work item
                        should be automatically freed when it is complete

            Return value:

                A pointer to the final allocated osd_work_item in the list.

            Notes:

                On single-threaded systems, this function may actually execute the
                work item immediately before returning.
        -----------------------------------------------------------------------------*/
        public abstract osd_work_item osd_work_item_queue_multiple(osd_work_queue queue, osd_work_callback callback, int32_t numitems, List<Object> parambase, /*int paramstep,*/ uint32_t flags);


        /* inline helper to queue a single work item using the same interface */
        public osd_work_item osd_work_item_queue(osd_work_queue queue, osd_work_callback callback, Object param, uint32_t flags)
        {
            List<Object> parambase = new List<object>();
            parambase.Add(param);
            return osd_work_item_queue_multiple(queue, callback, 1, parambase, /*0,*/ flags);
        }


        /*-----------------------------------------------------------------------------
            osd_work_item_wait: wait for a work item to complete

            Parameters:

                item - pointer to an osd_work_item that was previously returned from
                    osd_work_item_queue

                timeout - a timeout value in osd_ticks_per_second()

            Return value:

                true if the item completed; false if the wait timed out before the
                item completed.
        -----------------------------------------------------------------------------*/
        //bool osd_work_item_wait(osd_work_item *item, osd_ticks_t timeout);


        /*-----------------------------------------------------------------------------
            osd_work_item_result: get the result of a work item

            Parameters:

                item - pointer to an osd_work_item that was previously returned from
                    osd_work_item_queue

            Return value:

                A void * that represents the work item's result.
        -----------------------------------------------------------------------------*/
        //void *osd_work_item_result(osd_work_item *item);


        /*-----------------------------------------------------------------------------
            osd_work_item_release: release the memory allocated to a work item

            Parameters:

                item - pointer to an osd_work_item that was previously returned from
                    osd_work_item_queue

            Return value:

                None.

            Notes:

                The osd_work_item exists until explicitly released, even if it has
                long since completed. It is the queuer's responsibility to release
                any work items it has queued.
        -----------------------------------------------------------------------------*/
        //void osd_work_item_release(osd_work_item *item);



        /***************************************************************************
            MISCELLANEOUS INTERFACES
        ***************************************************************************/

        /*-----------------------------------------------------------------------------
            osd_alloc_executable: allocate memory that can contain executable code

            Parameters:

                size - the number of bytes to allocate

            Return value:

                a pointer to the allocated memory

            Notes:

                On many systems, this call may acceptably map to malloc(). On systems
                where pages are tagged with "no execute" privileges, it may be
                necessary to perform some kind of special allocation to ensure that
                code placed into this buffer can be executed.
        -----------------------------------------------------------------------------*/
        //void *osd_alloc_executable(size_t size);


        /*-----------------------------------------------------------------------------
            osd_free_executable: free memory allocated by osd_alloc_executable

            Parameters:

                ptr - the pointer returned from osd_alloc_executable

                size - the number of bytes originally requested

            Return value:

                None
        -----------------------------------------------------------------------------*/
        //void osd_free_executable(void *ptr, size_t size);


        /*-----------------------------------------------------------------------------
            osd_break_into_debugger: break into the hosting system's debugger if one
                is attached

            Parameters:

                message - pointer to string to output to the debugger

            Return value:

                None.

            Notes:

                This function is called when an assertion or other important error
                occurs. If a debugger is attached to the current process, it should
                break into the debugger and display the given message.
        -----------------------------------------------------------------------------*/
        public abstract void osd_break_into_debugger(string message);

        public void osd_break_into_debugger(string format, params object [] args) { osd_break_into_debugger(string.Format(format, args)); }


        /*-----------------------------------------------------------------------------
          MESS specific code below
        -----------------------------------------------------------------------------*/

        /*-----------------------------------------------------------------------------
            osd_get_clipboard_text: retrieves text from the clipboard

            Return value:

                the returned string needs to be osd_free()-ed!

        -----------------------------------------------------------------------------*/
        //char *osd_get_clipboard_text(void);



        /***************************************************************************
            DIRECTORY INTERFACES
        ***************************************************************************/

        /*-----------------------------------------------------------------------------
            osd_stat: return a directory entry for a path

            Parameters:

                path - path in question

            Return value:

                an allocated pointer to an osd::directory::entry representing
                info on the path; even if the file does not exist.
                free with osd_free()

        -----------------------------------------------------------------------------*/
        //std::unique_ptr<osd::directory::entry> osd_stat(std::string const &path);



        /***************************************************************************
            PATH INTERFACES
        ***************************************************************************/

        /*-----------------------------------------------------------------------------
            osd_get_full_path: retrieves the full path

            Parameters:

                path - the path in question
                dst - pointer to receive new path; the returned string needs to be osd_free()-ed!

            Return value:

                file error

        -----------------------------------------------------------------------------*/
        //file_error osd_get_full_path(char **dst, const char *path);



        /***************************************************************************
            MIDI I/O INTERFACES
        ***************************************************************************/

        public abstract class osd_midi_device
        {
            // free result with osd_close_midi_channel()
            //virtual bool open_input(const char *devname) = 0;
            // free result with osd_close_midi_channel()
            //virtual bool open_output(const char *devname) = 0;
            //virtual void close() = 0;
            //virtual bool poll() = 0;
            //virtual int read(UINT8 *pOut) = 0;
            //virtual void write(UINT8 data) = 0;
        }


        //FIXME: really needed here?
        public abstract void osd_list_network_adapters();



        /***************************************************************************
            UNCATEGORIZED INTERFACES
        ***************************************************************************/

        /*-----------------------------------------------------------------------------
            osd_get_volume_name: retrieves the volume name

            Parameters:

                idx - order number of volume

            Return value:

                pointer to volume name

        -----------------------------------------------------------------------------*/
        //const char *osd_get_volume_name(int idx);


        /*-----------------------------------------------------------------------------
            osd_subst_env: substitute environment variables with values

            Parameters:

                dst - result pointer
                src - source string

        -----------------------------------------------------------------------------*/
        public abstract void osd_subst_env(out string dst, string src);


        /* calls to be used by the code */
        public static void osd_printf_error(string format) { if (osd_output.ptr() >= 0) osd_output.stack(osd_output.ptr()).output_callback(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, format); }
        public static void osd_printf_warning(string format) { if (osd_output.ptr() >= 0) osd_output.stack(osd_output.ptr()).output_callback(osd_output_channel.OSD_OUTPUT_CHANNEL_WARNING, format); }
        public static void osd_printf_info(string format) { if (osd_output.ptr() >= 0) osd_output.stack(osd_output.ptr()).output_callback(osd_output_channel.OSD_OUTPUT_CHANNEL_INFO, format); }
        public static void osd_printf_verbose(string format) { if (osd_output.ptr() >= 0) osd_output.stack(osd_output.ptr()).output_callback(osd_output_channel.OSD_OUTPUT_CHANNEL_VERBOSE, format); }
        public static void osd_printf_debug(string format) { if (osd_output.ptr() >= 0) osd_output.stack(osd_output.ptr()).output_callback(osd_output_channel.OSD_OUTPUT_CHANNEL_DEBUG, format); }

        public static void osd_printf_error(string format, params object [] args) { osd_printf_error(string.Format(format, args)); }
        public static void osd_printf_warning(string format, params object [] args) { osd_printf_warning(string.Format(format, args)); }
        public static void osd_printf_info(string format, params object [] args) { osd_printf_info(string.Format(format, args)); }
        public static void osd_printf_verbose(string format, params object [] args) { osd_printf_verbose(string.Format(format, args)); }
        public static void osd_printf_debug(string format, params object [] args) { osd_printf_debug(string.Format(format, args)); }


        // returns command line arguments as an std::vector<std::string> in UTF-8
        //std::vector<std::string> osd_get_command_line(int argc, char *argv[]);

        // specifies "aggressive focus" - should MAME capture input for any windows co-habiting a MAME window?
        //void osd_set_aggressive_input_focus(bool aggressive_focus);
    }


    /* ----- output management ----- */
    public abstract class osd_output : global_object
    {
        const int MAXSTACK = 10;


        static osd_output [] m_stack = new osd_output[MAXSTACK];
        static int m_ptr = -1;


        osd_output m_chain;


        public osd_output() { m_chain = null; }


        public static int ptr() { return m_ptr; }
        public static osd_output stack(int index) { return m_stack[index]; }


        public abstract void output_callback(osd_output_channel channel, string msg);


        public static void push(osd_output callback)
        {
            if (m_ptr < MAXSTACK)
            {
                callback.m_chain = (m_ptr >= 0 ? m_stack[m_ptr] : null);
                m_ptr++;
                m_stack[m_ptr] = callback;
            }
        }

        public static void pop(osd_output callback)
        {
            int f = -1;
            for (int i=0; i<=m_ptr; i++)
            {
                if (m_stack[i] == callback)
                {
                    f = i;
                    break;
                }
            }

            if (f >= 0)
            {
                if (f < m_ptr)
                {
                    m_stack[f+1].m_chain = m_stack[f].m_chain;
                }
                m_ptr--;
                for (int i = f; i <= m_ptr; i++)
                    m_stack[i] = m_stack[i+1];
            }
        }


        protected void chain_output(osd_output_channel channel, string msg)
        {
            if (m_chain != null)
                m_chain.output_callback(channel, msg);
        }
    }
}
