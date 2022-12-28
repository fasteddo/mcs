// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;

using static mame.osdfile_global;


namespace mame
{
    public static partial class util
    {
        /// \defgroup pathutils Filesystem path utilities
        /// \{

        /// \brief Is a character a directory separator?
        ///
        /// Determine whether a character is used to separate components within
        /// a filesystem path.
        /// \param [in] c A character to test.
        /// \return True if the character is used to separate components in
        ///   filesystem paths.
        public static bool is_directory_separator(char c)
        {
//#if defined(_WIN32)
            return ('\\' == c) || ('/' == c) || (':' == c);
//#else
//            return '/' == c;
//#endif
        }


        /// \brief Append components to a filesystem path
        ///
        /// Appends directory components to a filesystem path.
        /// \param [in,out] path The path to append to.
        /// \param [in] next The first directory component to append to the
        ///   path.
        /// \param [in] more Additional directory components to append to the
        ///   path.
        /// \return A reference to the modified path.
        //template <typename T, typename... U>
        public static string path_append(ref string path, string next)  //inline std::string &path_append(std::string &path, T &&next, U &&... more)
        {
            if (!path.empty() && !is_directory_separator(path.back()))
                path += PATH_SEPARATOR;  //path.append(PATH_SEPARATOR);
            path += next;  //path.append(std::forward<T>(next));
            //if constexpr (sizeof...(U) > 0U)
            //    return path_append(path, std::forward<U>(more)...);
            //else
                return path;
        }


        /// \brief Concatenate filsystem paths
        ///
        /// Concatenates multiple filesystem paths.
        /// \param [in] first Initial filesystem path.
        /// \param [in] more Additional directory components to append to the
        ///   intial path.
        /// \return The concatenated filesystem path.
        //template <typename T, typename... U>
        public static string path_concat(string first, string more)  //inline std::string path_concat(T &&first, U &&... more)
        {
            string result = first;  //std::string result(std::forward<T>(first));
            //if constexpr (sizeof...(U) > 0U)
                path_append(ref result, more);  //path_append(result, std::forward<U>(more)...);
            return result;
        }


        /***************************************************************************
            FUNCTION PROTOTYPES
        ***************************************************************************/

        /* ----- filename utilities ----- */

        // extract the base part of a filename (remove extensions and paths)
        // -------------------------------------------------
        // core_filename_extract_base - extract the base
        // name from a filename; note that this makes
        // assumptions about path separators
        // -------------------------------------------------
        public static string core_filename_extract_base(string name, bool strip_extension = false)
        {
            //// find the start of the basename
            //auto const start = std::find_if(name.rbegin(), name.rend(), &util::is_directory_separator);
            //if (start == name.rbegin())
            //    return std::string_view();
            //
            //// find the end of the basename
            //auto const chop_position = strip_extension
            //    ? std::find(name.rbegin(), start, '.')
            //    : start;
            //auto const end = ((chop_position != start) && (std::next(chop_position) != start))
            //    ? std::next(chop_position)
            //    : name.rbegin();
            //
            //return std::string_view(&*start.base(), end.base() - start.base());

            if (strip_extension)
                return Path.GetFileNameWithoutExtension(name);
            else
                return Path.GetFileName(name);
        }


        // extracts the file extension from a filename
        //std::string_view core_filename_extract_extension(std::string_view filename, bool strip_period = false) noexcept;


        // true if the given filename ends with a particular extension
        // -------------------------------------------------
        // core_filename_ends_with - does the given
        // filename end with the specified extension?
        // -------------------------------------------------
        public static bool core_filename_ends_with(string filename, string extension)
        {
            //auto namelen = filename.length();
            //auto extlen = extension.length();
            //
            //// first if the extension is bigger than the name, we definitely don't match
            //bool matches = namelen >= extlen;
            //
            //// work backwards checking for a match
            //while (matches && extlen > 0 && namelen > 0)
            //{
            //    if (std::tolower(uint8_t(filename[--namelen])) != std::tolower(uint8_t(extension[--extlen])))
            //        matches = false;
            //}
            //
            //return matches;

            return filename.EndsWith(extension);
        }

        /// \}
    }
}
