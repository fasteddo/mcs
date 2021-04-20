// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    class pexception_global
    {
        /// \brief Terminate the program.
        ///
        /// \note could be enhanced by setting a termination handler
        ///
        //[[noreturn]] void terminate(const char *msg) noexcept;
        //============================================================
        // terminate
        //============================================================
        public static void terminate(string msg)
        {
            try
            {
                std.cerr(msg + "\n");
            }
            catch (Exception)
            {
                /* ignore */
            }

            std.terminate();
        }
    }


    //[[noreturn]] void passert_fail(const char *assertion,
    //    const char *file, int lineno, const char *msg) noexcept;


    //============================================================
    //  exception base
    //============================================================
    class pexception : Exception
    {
        string m_text;


        public pexception(string text) { m_text = text; }


        //const pstring &text() const noexcept { return m_text; }
        //const char* what() const noexcept override { return m_text.c_str(); }
    }


    //class file_e : public plib::pexception

    //class file_open_e : public file_e

    //class file_read_e : public file_e

    //class file_write_e : public file_e

    //class null_argument_e : public plib::pexception

    //class out_of_mem_e : public plib::pexception

    //class fpexception_e : public pexception

    //static constexpr unsigned FP_INEXACT = 0x0001;
    //static constexpr unsigned FP_DIVBYZERO = 0x0002;
    //static constexpr unsigned FP_UNDERFLOW = 0x0004;
    //static constexpr unsigned FP_OVERFLOW = 0x0008;
    //static constexpr unsigned FP_INVALID = 0x00010;
    //static constexpr unsigned FP_ALL = 0x0001f;

    //class fpsignalenabler
}
