// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using int32_t = System.Int32;
using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.osdcore_global;


namespace mame
{
    public abstract class osdcore_interface
    {
        /// \brief Get environment variable value
        ///
        /// \param [in] name Name of the environment variable as a
        ///   NUL-terminated string.
        /// \return Pointer to environment variable value as a NUL-terminated
        ///   string if found, or nullptr if not found.
        public virtual string osd_getenv(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }


        /// \brief Get current process ID
        ///
        /// \return The process ID of the current process.
        //int osd_getpid();


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


        /***************************************************************************
            TIMING INTERFACES
        ***************************************************************************/

        /* a osd_ticks_t is a 64-bit unsigned integer that is used as a core type in timing interfaces */
        //typedef uint64_t osd_ticks_t;


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

        /* this is the maximum number of supported threads for a single work queue */
        /* threadid values are expected to range from 0..WORK_MAX_THREADS-1 */
        public const int WORK_MAX_THREADS            = 16;

        /* these flags can be set when creating a queue to give hints to the code about
           how to configure the queue */
        public const uint32_t WORK_QUEUE_FLAG_IO        = 0x0001;
        public const uint32_t WORK_QUEUE_FLAG_MULTI     = 0x0002;
        public const uint32_t WORK_QUEUE_FLAG_HIGH_FREQ = 0x0004;

        /* these flags can be set when queueing a work item to indicate how to handle
           its deconstruction */
        public const uint32_t WORK_ITEM_FLAG_AUTO_RELEASE = 0x0001;


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

        /// \brief Break into host debugger if attached
        ///
        /// This function is called when a fatal error occurs.  If a debugger is
        /// attached, it should break and display the specified message.
        /// \param [in] message Message to output to the debugger as a
        ///   NUL-terminated string.
        public abstract void osd_break_into_debugger(string message);

        public void osd_break_into_debugger(string format, params object [] args) { osd_break_into_debugger(string.Format(format, args)); }


        /*-----------------------------------------------------------------------------
          MESS specific code below
        -----------------------------------------------------------------------------*/

        /// \brief Get clipboard text
        ///
        /// Gets current clipboard content as UTF-8 text.  Returns an empty
        /// string if the clipboard contents cannot be converted to plain text.
        /// \return Clipboard contents or an empty string.
        //std::string osd_get_clipboard_text(void);


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
            osd_subst_env: substitute environment variables with values

            Parameters:

                src - source string

        -----------------------------------------------------------------------------*/
        public abstract string osd_subst_env(string src);


        //class osd_gpu


        // returns command line arguments as an std::vector<std::string> in UTF-8
        //std::vector<std::string> osd_get_command_line(int argc, char *argv[]);

        // specifies "aggressive focus" - should MAME capture input for any windows co-habiting a MAME window?
        //void osd_set_aggressive_input_focus(bool aggressive_focus);


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
    }


    /* osd_work_queue is an opaque type which represents a queue of work items */
    // defined in osdsync.cs
    //struct osd_work_queue;

    /* osd_work_item is an opaque type which represents a single work item */
    // defined in osdsync.cs
    //struct osd_work_item;

    /* osd_work_callback is a callback function that does work */
    public delegate Object osd_work_callback(Object param, int threadid);  //typedef void *(*osd_work_callback)(void *param, int threadid);


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


    public abstract class osd_output
    {
        const int MAXSTACK = 10;


        static osd_output [] m_stack = new osd_output[MAXSTACK];
        static int m_ptr = -1;


        osd_output m_chain;


        public osd_output() { }


        public static int ptr() { return m_ptr; }
        public static osd_output stack(int index) { return m_stack[index]; }


        public abstract void output_callback(osd_output_channel channel, string format, params object [] args);  //virtual void output_callback(osd_output_channel channel, util::format_argument_pack<std::ostream> const &args) = 0;


        public static void push(osd_output callback)
        {
            if (m_ptr < MAXSTACK - 1)
            {
                callback.m_chain = (m_ptr >= 0 ? m_stack[m_ptr] : null);
                m_ptr++;
                m_stack[m_ptr] = callback;
            }
        }

        public static void pop(osd_output callback)
        {
            int f = -1;
            for (int i = 0; i <= m_ptr; i++)
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


        protected void chain_output(osd_output_channel channel, string format, params object [] args)  //void chain_output(osd_output_channel channel, util::format_argument_pack<std::ostream> const &args) const
        {
            if (m_chain != null)
                m_chain.output_callback(channel, format, args);
        }
    }


    public static class osdcore_global
    {
        /*-------------------------------------------------
            osd_vprintf_error - output an error to the
            appropriate callback
        -------------------------------------------------*/
        static void osd_vprintf_error(string format, params object [] args)  //void osd_vprintf_error(util::format_argument_pack<std::ostream> const &args)
        {
#if false //defined(SDLMAME_ANDROID)
            SDL_LogMessage(SDL_LOG_CATEGORY_APPLICATION, SDL_LOG_PRIORITY_ERROR, "%s", util::string_format(args).c_str());
#else
            if (osd_output.ptr() >= 0) osd_output.stack(osd_output.ptr()).output_callback(osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR, format, args);  //if (m_ptr >= 0) m_stack[m_ptr]->output_callback(OSD_OUTPUT_CHANNEL_ERROR, args);
#endif
        }

        /*-------------------------------------------------
            osd_vprintf_warning - output a warning to the
            appropriate callback
        -------------------------------------------------*/
        static void osd_vprintf_warning(string format, params object [] args)  //void osd_vprintf_warning(util::format_argument_pack<std::ostream> const &args)
        {
#if false //defined(SDLMAME_ANDROID)
            SDL_LogMessage(SDL_LOG_CATEGORY_APPLICATION, SDL_LOG_PRIORITY_ERROR, "%s", util::string_format(args).c_str());
#else
            if (osd_output.ptr() >= 0) osd_output.stack(osd_output.ptr()).output_callback(osd_output_channel.OSD_OUTPUT_CHANNEL_WARNING, format, args);  //if (m_ptr >= 0) m_stack[m_ptr]->output_callback(OSD_OUTPUT_CHANNEL_WARNING, args);
#endif
        }

        /*-------------------------------------------------
            osd_vprintf_info - output info text to the
            appropriate callback
        -------------------------------------------------*/
        static void osd_vprintf_info(string format, params object [] args)  //void osd_vprintf_info(util::format_argument_pack<std::ostream> const &args)
        {
#if false //defined(SDLMAME_ANDROID)
            SDL_LogMessage(SDL_LOG_CATEGORY_APPLICATION, SDL_LOG_PRIORITY_INFO, "%s", util::string_format(args).c_str());
#else
            if (osd_output.ptr() >= 0) osd_output.stack(osd_output.ptr()).output_callback(osd_output_channel.OSD_OUTPUT_CHANNEL_INFO, format, args);  //if (m_ptr >= 0) m_stack[m_ptr]->output_callback(OSD_OUTPUT_CHANNEL_INFO, args);
#endif
        }

        /*-------------------------------------------------
            osd_vprintf_verbose - output verbose text to
            the appropriate callback
        -------------------------------------------------*/
        static void osd_vprintf_verbose(string format, params object [] args)  //void osd_vprintf_verbose(util::format_argument_pack<std::ostream> const &args)
        {
#if false //defined(SDLMAME_ANDROID)
            SDL_LogMessage(SDL_LOG_CATEGORY_APPLICATION, SDL_LOG_PRIORITY_VERBOSE, "%s", util::string_format(args).c_str());
#else
            if (osd_output.ptr() >= 0) osd_output.stack(osd_output.ptr()).output_callback(osd_output_channel.OSD_OUTPUT_CHANNEL_VERBOSE, format, args);  //if (m_ptr >= 0) m_stack[m_ptr]->output_callback(OSD_OUTPUT_CHANNEL_VERBOSE, args);
#endif
        }

        /*-------------------------------------------------
            osd_vprintf_debug - output debug text to the
            appropriate callback
        -------------------------------------------------*/
        static void osd_vprintf_debug(string format, params object [] args)  //void osd_vprintf_debug(util::format_argument_pack<std::ostream> const &args)
        {
#if false //defined(SDLMAME_ANDROID)
            SDL_LogMessage(SDL_LOG_CATEGORY_APPLICATION, SDL_LOG_PRIORITY_DEBUG, "%s", util::string_format(args).c_str());
#else
            if (osd_output.ptr() >= 0) osd_output.stack(osd_output.ptr()).output_callback(osd_output_channel.OSD_OUTPUT_CHANNEL_DEBUG, format, args);  //if (m_ptr >= 0) m_stack[m_ptr]->output_callback(OSD_OUTPUT_CHANNEL_DEBUG, args);
#endif
        }


        /// \brief Print error message
        ///
        /// By default, error messages are sent to standard error.  The relaxed
        /// format rules used by util::string_format apply.
        /// \param [in] fmt Message format string.
        /// \param [in] args Optional message format arguments.
        /// \sa util::string_format
        public static void osd_printf_error(string fmt, params object [] args)  //template <typename Format, typename... Params> void osd_printf_error(Format &&fmt, Params &&...args)
        {
            osd_vprintf_error(fmt, args);  //return osd_vprintf_error(util::make_format_argument_pack(std::forward<Format>(fmt), std::forward<Params>(args)...));
        }

        /// \brief Print warning message
        ///
        /// By default, warning messages are sent to standard error.  The
        /// relaxed format rules used by util::string_format apply.
        /// \param [in] fmt Message format string.
        /// \param [in] args Optional message format arguments.
        /// \sa util::string_format
        public static void osd_printf_warning(string fmt, params object [] args)  //template <typename Format, typename... Params> void osd_printf_warning(Format &&fmt, Params &&...args)
        {
            osd_vprintf_warning(fmt, args);  //return osd_vprintf_warning(util::make_format_argument_pack(std::forward<Format>(fmt), std::forward<Params>(args)...));
        }

        /// \brief Print informational message
        ///
        /// By default, informational messages are sent to standard output.
        /// The relaxed format rules used by util::string_format apply.
        /// \param [in] fmt Message format string.
        /// \param [in] args Optional message format arguments.
        /// \sa util::string_format
        public static void osd_printf_info(string fmt, params object [] args)  //template <typename Format, typename... Params> void osd_printf_info(Format &&fmt, Params &&...args)
        {
            osd_vprintf_info(fmt, args);  //return osd_vprintf_info(util::make_format_argument_pack(std::forward<Format>(fmt), std::forward<Params>(args)...));
        }

        /// \brief Print verbose diagnostic message
        ///
        /// Verbose diagnostic messages are disabled by default.  If enabled,
        /// they are sent to standard output by default.  The relaxed format
        /// rules used by util::string_format apply.  Note that the format
        /// string and arguments will always be evaluated, even if verbose
        /// diagnostic messages are disabled.
        /// \param [in] fmt Message format string.
        /// \param [in] args Optional message format arguments.
        /// \sa util::string_format
        public static void osd_printf_verbose(string fmt, params object [] args)  //template <typename Format, typename... Params> void osd_printf_verbose(Format &&fmt, Params &&...args)
        {
            osd_vprintf_verbose(fmt, args);  //return osd_vprintf_verbose(util::make_format_argument_pack(std::forward<Format>(fmt), std::forward<Params>(args)...));
        }

        /// \brief Print debug message
        ///
        /// By default, debug messages are sent to standard output for debug
        /// builds only.  The relaxed format rules used by util::string_format
        /// apply.  Note that the format string and arguments will always be
        /// evaluated, even if debug messages are disabled.
        /// \param [in] fmt Message format string.
        /// \param [in] args Optional message format arguments.
        /// \sa util::string_format
        public static void osd_printf_debug(string fmt, params object [] args)  //template <typename Format, typename... Params> void osd_printf_debug(Format &&fmt, Params &&...args)
        {
            osd_vprintf_debug(fmt, args);  //return osd_vprintf_debug(util::make_format_argument_pack(std::forward<Format>(fmt), std::forward<Params>(args)...));
        }


        public static osdcore_interface m_osdcore;

        public static void set_osdcore(osdcore_interface osdcore) { m_osdcore = osdcore; }
    }
}
