// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using osd_ticks_t = System.UInt64;
using u64 = System.UInt64;


namespace mame
{
    public enum profile_type
    {
        PROFILER_DEVICE_FIRST = 0,
        PROFILER_DEVICE_MAX = PROFILER_DEVICE_FIRST + 256,
        PROFILER_DRC_COMPILE,
        PROFILER_MEM_REMAP,
        PROFILER_MEMREAD,
        PROFILER_MEMWRITE,
        PROFILER_VIDEO,
        PROFILER_DRAWGFX,
        PROFILER_COPYBITMAP,
        PROFILER_TILEMAP_DRAW,
        PROFILER_TILEMAP_DRAW_ROZ,
        PROFILER_TILEMAP_UPDATE,
        PROFILER_BLIT,
        PROFILER_SOUND,
        PROFILER_TIMER_CALLBACK,
        PROFILER_INPUT,             // input.cpp and inptport.cpp
        PROFILER_MOVIE_REC,         // movie recording
        PROFILER_LOGERROR,          // logerror
        PROFILER_LUA,               // LUA
        PROFILER_EXTRA,             // everything else

        // the USER types are available to driver writers to profile
        // custom sections of the code
        PROFILER_USER1,
        PROFILER_USER2,
        PROFILER_USER3,
        PROFILER_USER4,
        PROFILER_USER5,
        PROFILER_USER6,
        PROFILER_USER7,
        PROFILER_USER8,

        PROFILER_PROFILER,
        PROFILER_IDLE,
        PROFILER_TOTAL
    }
    //DECLARE_ENUM_INCDEC_OPERATORS(profile_type)


    public static class profiler_global
    {
#if MAME_PROFILER
        public static real_profiler_state g_profiler = new real_profiler_state();
#else
        public static dummy_profiler_state g_profiler = new dummy_profiler_state();
#endif
    }


    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> real_profiler_state
    public class real_profiler_state : global_object
    {
        // an entry in the FILO
        struct filo_entry
        {
            public int type;                       // type of entry
            public osd_ticks_t start;                      // start time
        };

        struct profile_string
        {
            public int type;
            public string str;
        };


        const double TEXT_UPDATE_TIME = 0.5;

        static readonly profile_string [] names = new profile_string []
        {
            new profile_string() { type = (int)profile_type.PROFILER_DRC_COMPILE,      str = "DRC Compilation" },
            new profile_string() { type = (int)profile_type.PROFILER_MEM_REMAP,        str = "Memory Remapping" },
            new profile_string() { type = (int)profile_type.PROFILER_MEMREAD,          str = "Memory Read" },
            new profile_string() { type = (int)profile_type.PROFILER_MEMWRITE,         str = "Memory Write" },
            new profile_string() { type = (int)profile_type.PROFILER_VIDEO,            str = "Video Update" },
            new profile_string() { type = (int)profile_type.PROFILER_DRAWGFX,          str = "drawgfx" },
            new profile_string() { type = (int)profile_type.PROFILER_COPYBITMAP,       str = "copybitmap" },
            new profile_string() { type = (int)profile_type.PROFILER_TILEMAP_DRAW,     str = "Tilemap Draw" },
            new profile_string() { type = (int)profile_type.PROFILER_TILEMAP_DRAW_ROZ, str = "Tilemap ROZ Draw" },
            new profile_string() { type = (int)profile_type.PROFILER_TILEMAP_UPDATE,   str = "Tilemap Update" },
            new profile_string() { type = (int)profile_type.PROFILER_BLIT,             str = "OSD Blitting" },
            new profile_string() { type = (int)profile_type.PROFILER_SOUND,            str = "Sound Generation" },
            new profile_string() { type = (int)profile_type.PROFILER_TIMER_CALLBACK,   str = "Timer Callbacks" },
            new profile_string() { type = (int)profile_type.PROFILER_INPUT,            str = "Input Processing" },
            new profile_string() { type = (int)profile_type.PROFILER_MOVIE_REC,        str = "Movie Recording" },
            new profile_string() { type = (int)profile_type.PROFILER_LOGERROR,         str = "Error Logging" },
            new profile_string() { type = (int)profile_type.PROFILER_LUA,              str = "LUA" },
            new profile_string() { type = (int)profile_type.PROFILER_EXTRA,            str = "Unaccounted/Overhead" },
            new profile_string() { type = (int)profile_type.PROFILER_USER1,            str = "User 1" },
            new profile_string() { type = (int)profile_type.PROFILER_USER2,            str = "User 2" },
            new profile_string() { type = (int)profile_type.PROFILER_USER3,            str = "User 3" },
            new profile_string() { type = (int)profile_type.PROFILER_USER4,            str = "User 4" },
            new profile_string() { type = (int)profile_type.PROFILER_USER5,            str = "User 5" },
            new profile_string() { type = (int)profile_type.PROFILER_USER6,            str = "User 6" },
            new profile_string() { type = (int)profile_type.PROFILER_USER7,            str = "User 7" },
            new profile_string() { type = (int)profile_type.PROFILER_USER8,            str = "User 8" },
            new profile_string() { type = (int)profile_type.PROFILER_PROFILER,         str = "Profiler" },
            new profile_string() { type = (int)profile_type.PROFILER_IDLE,             str = "Idle" }
        };



        // internal state
        int m_filoptrIdx = -1; //filo_entry m_filoptr;                  // current FILO index
        string m_text = "";                     // profiler text
        attotime m_text_time;                // profiler text last update
        filo_entry [] m_filo = new filo_entry[32];                 // array of FILO entries
        osd_ticks_t [] m_data = new osd_ticks_t[(int)profile_type.PROFILER_TOTAL + 1]; // array of data


        // construction/destruction
        //-------------------------------------------------
        //  real_profiler_state - constructor
        //-------------------------------------------------
        public real_profiler_state()
        {
            //memset(m_filo, 0, sizeof(m_filo));
            //memset(m_data, 0, sizeof(m_data));
            reset(false);
        }


        // getters
        bool enabled()
        {
            return m_filoptrIdx != -1;  // return m_filoptr != null;
        }

        //-------------------------------------------------
        //  text - return the current text in an astring
        //-------------------------------------------------
        public string text(running_machine machine)
        {
            start(profile_type.PROFILER_PROFILER);

            // get the current time
            attotime current_time = machine.scheduler().time();

            // we only want to update the text periodically
            if ((m_text_time == attotime.never) || ((current_time - m_text_time).as_double() >= TEXT_UPDATE_TIME))
            {
                update_text(machine);
                m_text_time = current_time;
            }

            stop();
            return m_text;
        }

        // enable/disable
        public void enable(bool state = true)
        {
            if (state != enabled())
            {
                reset(state);
            }
        }

        // start/stop
        public void start(profile_type type) { if (enabled()) real_start(type); }
        public void stop() { if (enabled()) real_stop(); }


        //-------------------------------------------------
        //  reset - initializes state
        //-------------------------------------------------
        void reset(bool enabled)
        {
            m_text_time = attotime.never;

            if (enabled)
            {
                // we're enabled now
                m_filoptrIdx = 0;  // m_filoptr = m_filo;

                // set up dummy entry
                m_filo[m_filoptrIdx].start = 0;
                m_filo[m_filoptrIdx].type = (int)profile_type.PROFILER_TOTAL;
            }
            else
            {
                // magic value to indicate disabled
                m_filoptrIdx = -1;
            }
        }

        //-------------------------------------------------
        //  update_text - update the current astring
        //-------------------------------------------------
        void update_text(running_machine machine)
        {
            // compute the total time for all bits, not including profiler or idle
            u64 computed = 0;
            profile_type curtype;
            for (curtype = profile_type.PROFILER_DEVICE_FIRST; curtype < profile_type.PROFILER_PROFILER; curtype++)
                computed += m_data[(int)curtype];

            // save that result in normalize, and continue adding the rest
            u64 normalize = computed;
            for ( ; curtype < profile_type.PROFILER_TOTAL; curtype++)
                computed += m_data[(int)curtype];

            // this becomes the total; if we end up with 0 for anything, we were just started, so return empty
            u64 total = computed;
            if (total == 0 || normalize == 0)
            {
                m_text = "";
                return;
            }

            // loop over all types and generate the string
            device_enumerator iter = new device_enumerator(machine.root_device());
            string stream = "";
            for (curtype = profile_type.PROFILER_DEVICE_FIRST; curtype < profile_type.PROFILER_TOTAL; curtype++)
            {
                // determine the accumulated time for this type
                computed = m_data[(int)curtype];

                // if we have non-zero data and we're ready to display, do it
                if (computed != 0)
                {
                    // start with the un-normalized percentage
                    stream += string.Format("{0}%% ", (int)((computed * 100 + total/2) / total));

                    // followed by the normalized percentage for everything but profiler and idle
                    if (curtype < profile_type.PROFILER_PROFILER)
                        stream += string.Format("{0}%% ", (int)((computed * 100 + normalize/2) / normalize));

                    // and then the text
                    if (curtype >= profile_type.PROFILER_DEVICE_FIRST && curtype <= profile_type.PROFILER_DEVICE_MAX)
                    {
                        stream += string.Format("'{0}'", iter.byindex(curtype - profile_type.PROFILER_DEVICE_FIRST).tag());
                    }
                    else
                    {
                        for (int nameindex = 0; nameindex < names.Length; nameindex++)
                        {
                            if (names[nameindex].type == (int)curtype)
                            {
                                stream += names[nameindex].str;
                                break;
                            }
                        }
                    }

                    // followed by a carriage return
                    stream += "\n";
                }
            }

            // reset data set to 0
            memset<osd_ticks_t>(m_data, 0, (UInt32)m_data.Length);
            m_text = stream.str();
        }

        //-------------------------------------------------
        //  real_start - mark the beginning of a
        //  profiler entry
        //-------------------------------------------------
        void real_start(profile_type type)
        {
            // fail if we overflow
            if (m_filoptrIdx >= m_filo.Length - 1)  //if (m_filoptr >= &m_filo[ARRAY_LENGTH(m_filo) - 1])
                throw new emu_fatalerror("Profiler FILO overflow (type = {0})\n", type);

            // get current tick count
            osd_ticks_t curticks = (UInt64)get_profile_ticks();

            // update previous entry
            m_data[m_filo[m_filoptrIdx].type] += curticks - m_filo[m_filoptrIdx].start;

            // move to next entry
            m_filoptrIdx++;  // m_filoptr++;

            // fill in this entry
            m_filo[m_filoptrIdx].type = (int)type;
            m_filo[m_filoptrIdx].start = curticks;
        }

        //-------------------------------------------------
        //  real_stop - mark the end of a profiler entry
        //-------------------------------------------------
        void real_stop()
        {
            // degenerate scenario
            if (m_filoptrIdx <= -1)  // if (UNEXPECTED(m_filoptr <= m_filo))
                return;

            // get current tick count
            osd_ticks_t curticks = (UInt64)get_profile_ticks();

            // account for the time taken
            m_data[m_filo[m_filoptrIdx].type] += curticks - m_filo[m_filoptrIdx].start;

            // move back an entry
            m_filoptrIdx--;  // m_filoptr--;

            // reset previous entry start time
            m_filo[m_filoptrIdx].start = curticks;
        }
    }


    // ======================> dummy_profiler_state
    public class dummy_profiler_state
    {
        // construction/destruction
        //-------------------------------------------------
        //  dummy_profiler_state - constructor
        //-------------------------------------------------
        public dummy_profiler_state() {}

        // getters
        bool enabled() { return false; }
        public string text(running_machine machine) { return ""; }

        // enable/disable
        public void enable(bool state = true) { }

        // start/stop
        public void start(profile_type type) { }
        public void stop() { }
    }
}
