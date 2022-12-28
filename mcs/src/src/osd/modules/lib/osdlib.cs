// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.osdlib_global;


namespace mame
{
    public abstract class osdlib_interface
    {
        /*-----------------------------------------------------------------------------
            osd_process_kill: kill the current process

            Parameters:

                None.

            Return value:

                None.
        -----------------------------------------------------------------------------*/
        //void osd_process_kill();


        /*-----------------------------------------------------------------------------
            osd_setenv: set environment variable

            Parameters:

                name  - name of environment variable
                value - value to write
                overwrite - overwrite if it exists

            Return value:

                0 on success
        -----------------------------------------------------------------------------*/
        //int osd_setenv(const char *name, const char *value, int overwrite);


        /// \brief Get clipboard text
        ///
        /// Gets current clipboard content as UTF-8 text.  Returns an empty
        /// string if the clipboard contents cannot be converted to plain text.
        /// \return Clipboard contents or an empty string.
        public abstract string osd_get_clipboard_text();


        /// \brief Set clipboard text
        ///
        /// Sets the desktop environment's clipboard contents to the supplied
        /// UTF-8 text.  The contents of the clipboard may be changed on error.
        /// \param [in] text The text to copy to the clipboard.
        /// \return An error condition if the operation failed or is
        ///   unsupported.
        //std::error_condition osd_set_clipboard_text(std::string_view text) noexcept;
    }


    //namespace osd {

    //bool invalidate_instruction_cache(void const *start, std::size_t size);


    //class virtual_memory_allocation


    //class dynamic_module

    //} // namespace osd


    //#define OSD_DYNAMIC_API(apiname, ...) osd::dynamic_module::ptr m_##apiname##module = osd::dynamic_module::open( { __VA_ARGS__ } )
    //#define OSD_DYNAMIC_API_FN(apiname, ret, conv, fname, ...) ret(conv *m_##fname##_pfn)( __VA_ARGS__ ) = m_##apiname##module->bind<ret(conv *)( __VA_ARGS__ )>(#fname)
    //#define OSD_DYNAMIC_CALL(fname, ...) (*m_##fname##_pfn) ( __VA_ARGS__ )
    //#define OSD_DYNAMIC_API_TEST(fname) (m_##fname##_pfn != nullptr)


    public static class osdlib_global
    {
        public static osdlib_interface m_osdlib;

        public static void set_osdlib(osdlib_interface osdlib) { m_osdlib = osdlib; }
    }
}
