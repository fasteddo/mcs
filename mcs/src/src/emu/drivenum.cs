// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using machine_config_cache = mame.util.lru_cache_map<System.UInt32, mame.machine_config>;


namespace mame
{
    // ======================> driver_list
    // driver_list is a purely static class that wraps the global driver list
    public class driver_list
    {
        // use variables in drivlist_global
        //static int s_driver_count;
        //static List<game_driver> s_drivers_sorted = new List<game_driver>();


        // getters
        public static UInt32 total() { return drivlist_global.s_driver_count; }


        // any item by index
        public static game_driver driver(UInt32 index) { global.assert(index < total());  return drivlist_global.s_drivers_sorted[(int)index]; }
        public static int clone(UInt32 index) { return find(driver(index).parent); }
        public static int non_bios_clone(UInt32 index) { int result = find(driver(index).parent); return (result >= 0 && ((UInt64)driver((UInt32)result).flags & gamedrv_global.MACHINE_IS_BIOS_ROOT) == 0) ? result : -1; }
        //static int compatible_with(UInt32 index) { return find(driver(index).compatible_with); }


        // any item by driver
        public static int clone(game_driver driver) { int index = find(driver); global.assert(index >= 0); return clone((UInt32)index); }
        public static int non_bios_clone(game_driver driver) { int index = find(driver); global.assert(index >= 0); return non_bios_clone((UInt32)index); }
        //static int compatible_with(const game_driver &driver) { int index = find(driver); assert(index != -1); return compatible_with(index); }


        // general helpers

        //-------------------------------------------------
        //  find - find a driver by name
        //-------------------------------------------------
        public static int find(string name)
        {
            // if no name, bail
            if (string.IsNullOrEmpty(name))
                return -1;

            // binary search to find it
            //game_driver begin = s_drivers_sorted;
            //game_driver end = begin + s_driver_count;
            //var cmp = [] (game_driver const *driver, char const *name) { return core_stricmp(driver->name, name) < 0; };
            //game_driver result = std::lower_bound(begin, end, name, cmp);
            //return ((result == end) || core_stricmp(result.name, name)) ? -1 : std::distance(begin, result);
            int index = 0;
            var driver = drivlist_global.s_drivers_sorted.Find(d => { index++; return d.name == name; });
            return driver == null ? -1 : index - 1;
        }

        public static int find(game_driver driver) { return find(driver.name); }


        // static helpers

        //-------------------------------------------------
        //  matches - true if we match, taking into
        //  account wildcards in the wildstring
        //-------------------------------------------------
        public static bool matches(string wildstring, string str)
        {
            // can only match internal drivers if the wildstring starts with an underscore
            if (str[0] == '_' && (wildstring == null || wildstring[0] != '_'))
                return false;

            // match everything else normally
            return wildstring == null || global.core_strwildcmp(wildstring, str) == 0;
        }


        //-------------------------------------------------
        //  penalty_compare - compare two strings for
        //  closeness and assign a score.
        //-------------------------------------------------
        public static int penalty_compare(string source, string target)
        {
            int gaps = 1;
            bool last = true;

            int sourceIdx = 0;
            int targetIdx = 0;
            // scan the strings
            for ( ; sourceIdx < source.Length && targetIdx < target.Length; targetIdx++)  //for ( ; *source && *target; target++)
            {
                // do a case insensitive match
                bool match = char.ToLower(source[sourceIdx]) == char.ToLower(target[targetIdx]);  //bool match = (tolower((UINT8)*source) == tolower((UINT8)*target));

                // if we matched, advance the source
                if (match)
                    sourceIdx++;

                // if the match state changed, count gaps
                if (match != last)
                {
                    last = match;
                    if (!match)
                        gaps++;
                }
            }

            // penalty if short string does not completely fit in
            for ( ; sourceIdx < source.Length; sourceIdx++)  //for ( ; *source; source++)
                gaps++;

            // if we matched perfectly, gaps == 0
            if (gaps == 1 && sourceIdx == source.Length && targetIdx == target.Length)  // *source == 0 && *target == 0)
                gaps = 0;

            return gaps;
        }
    }


    // ======================> driver_enumerator
    // driver_enumerator enables efficient iteration through the driver list
    class driver_enumerator : driver_list
    {
        const UInt32 CONFIG_CACHE_COUNT = 100;


        //typedef util::lru_cache_map<std::size_t, std::shared_ptr<machine_config> > machine_config_cache;


        // internal state
        int m_current;
        UInt32 m_filtered_count;
        emu_options m_options;
        std_vector<bool> m_included;
        machine_config_cache m_config;  //mutable machine_config_cache m_config;


        // construction/destruction
        //-------------------------------------------------
        //  driver_enumerator - constructor
        //-------------------------------------------------
        public driver_enumerator(emu_options options)
            : base()
        {
            m_current = -1;
            m_filtered_count = 0;
            m_options = options;
            m_included = new std_vector<bool>();
            m_included.resize((int)drivlist_global.s_driver_count);
            m_config = new util.lru_cache_map<uint, machine_config>(CONFIG_CACHE_COUNT);


            include_all();
        }

        public driver_enumerator(emu_options options, string str)
            : this(options)
        {
            filter(str);
        }

        public driver_enumerator(emu_options options, game_driver driver)
            : this(options)
        {
            filter(driver);
        }


        // getters
        public UInt32 count() { return m_filtered_count; }
        public int current() { return m_current; }
        public emu_options options() { return m_options; }


        // current item
        public game_driver driver() { return driver((UInt32)m_current); }
        public machine_config config() { return config((UInt32)m_current, m_options); }
        public int clone() { return clone((UInt32)m_current); }
        public int non_bios_clone() { return non_bios_clone((UInt32)m_current); }
        //int compatible_with() { return driver_list::compatible_with(m_current); }
        public void include() { include((UInt32)m_current); }
        void exclude() { exclude((UInt32)m_current); }


        // any item by index

        public bool included(UInt32 index)
        {
            global.assert(index < m_included.size());
            return m_included[(int)index];
        }


        //bool excluded(UInt32 index) const { assert(index >= 0 && index < s_driver_count); return !m_included[index]; }


        public machine_config config(UInt32 index) { return config(index, m_options); }


        //-------------------------------------------------
        //  config - return a machine_config for the given
        //  driver, allocating on demand if needed
        //-------------------------------------------------
        machine_config config(UInt32 index, emu_options options)
        {
            global.assert(index < drivlist_global.s_driver_count);

            // if we don't have it cached, add it
            machine_config config = m_config.find(index);  //m_config[index];
            if (config == null)
                config = new machine_config(drivlist_global.s_drivers_sorted[(int)index], options);

            return config;
        }


        public void include(UInt32 index)
        {
            global.assert(index < m_included.size());
            if (!m_included[(int)index])
            {
                m_included[(int)index] = true;
                m_filtered_count++;
            }
        }


        void exclude(UInt32 index)
        {
            global.assert(index < m_included.size());
            if (m_included[(int)index])
            {
                m_included[(int)index] = false;
                m_filtered_count--;
            }
        }


        // filtering/iterating

        //-------------------------------------------------
        //  filter - filter the driver list against the
        //  given string
        //-------------------------------------------------
        UInt32 filter(string filterstring = null)
        {
            // reset the count
            exclude_all();

            // match name against each driver in the list
            for (UInt32 index = 0; index < drivlist_global.s_driver_count; index++)
            {
                if (matches(filterstring, drivlist_global.s_drivers_sorted[(int)index].name))
                    include(index);
            }

            return m_filtered_count;
        }


        //-------------------------------------------------
        //  filter - filter the driver list against the
        //  given driver
        //-------------------------------------------------
        UInt32 filter(game_driver driver)
        {
            // reset the count
            exclude_all();

            // match name against each driver in the list
            for (UInt32 index = 0; index < drivlist_global.s_driver_count; index++)
            {
                if (drivlist_global.s_drivers_sorted[(int)index] == driver)
                    include(index);
            }

            return m_filtered_count;
        }


        //-------------------------------------------------
        //  include_all - include all non-internal drivers
        //-------------------------------------------------
        void include_all()
        {
            global.fill(m_included, true);  // std::fill(m_included.begin(), m_included.end(), true);
            m_filtered_count = (UInt32)m_included.size();

            // always exclude the empty driver
            exclude((UInt32)find("___empty"));
        }


        public void exclude_all()
        {
            global.fill(m_included, false);  //std::fill(m_included.begin(), m_included.end(), false);
            m_filtered_count = 0;
        }


        public void reset() { m_current = -1; }


        //-------------------------------------------------
        //  next - get the next driver matching the given
        //  filter
        //-------------------------------------------------
        public bool next()
        {
            release_current();

            // always advance one
            // if we have a filter, scan forward to the next match
            for (m_current++; (m_current < drivlist_global.s_driver_count) && !m_included[m_current]; m_current++) { }

            // return true if we end up in range
            return (m_current >= 0) && (m_current < drivlist_global.s_driver_count);
        }


        //-------------------------------------------------
        //  next_excluded - get the next driver that is
        //  not currently included in the list
        //-------------------------------------------------
        public bool next_excluded()
        {
            release_current();

            // always advance one
            // if we have a filter, scan forward to the next match
            for (m_current++; (m_current < drivlist_global.s_driver_count) && m_included[m_current]; m_current++) { }

            // return true if we end up in range
            return (m_current >= 0) && (m_current < drivlist_global.s_driver_count);
        }


        // general helpers

        //void set_current(UInt32 index) { assert(index >= -1 && index <= s_driver_count); m_current = index; }

        //-------------------------------------------------
        //  driver_sort_callback - compare two items in
        //  an array of game_driver pointers
        //-------------------------------------------------
        public void find_approximate_matches(string str, UInt32 count, out int [] results)
        {
            //#undef rand

            results = new int [count];

            // if no name, pick random entries
            if (string.IsNullOrEmpty(str))
            {
                // seed the RNG first
                //srand(osd_ticks());
                Random r = new Random((int)osdcore_global.m_osdcore.osd_ticks());

                // allocate a temporary list
                std_vector<int> templist = new std_vector<int>(m_filtered_count);
                int arrayindex = 0;
                for (int index = 0; index < drivlist_global.s_driver_count; index++)
                {
                    if (m_included[index])
                        templist[arrayindex++] = index;
                }

                global.assert(arrayindex == m_filtered_count);

                // shuffle
                for (int shufnum = 0; shufnum < (4 * drivlist_global.s_driver_count); shufnum++)
                {
                    int item1 = r.Next() % (int)m_filtered_count;
                    int item2 = r.Next() % (int)m_filtered_count;
                    int temp = templist[item1];
                    templist[item1] = templist[item2];
                    templist[item2] = temp;
                }

                // copy out the first few entries
                for (int matchnum = 0; matchnum < count; matchnum++)
                    results[matchnum] = templist[matchnum % (int)m_filtered_count];
            }
            else
            {
                // allocate memory to track the penalty value
                std_vector<int> penalty = new std_vector<int>(count);

                // initialize everyone's states
                for (int matchnum = 0; matchnum < count; matchnum++)
                {
                    penalty[matchnum] = 9999;
                    results[matchnum] = -1;
                }

                // scan the entire drivers array
                for (int index = 0; index < drivlist_global.s_driver_count; index++)
                {
                    if (m_included[index])
                    {
                        // pick the best match between driver name and description
                        int curpenalty = penalty_compare(str, drivlist_global.s_drivers_sorted[index].type.fullname());
                        int tmp = penalty_compare(str, drivlist_global.s_drivers_sorted[index].name);
                        curpenalty = Math.Min(curpenalty, tmp);

                        // insert into the sorted table of matches
                        for (int matchnum = (int)count - 1; matchnum >= 0; matchnum--)
                        {
                            // stop if we're worse than the current entry
                            if (curpenalty >= penalty[matchnum])
                                break;

                            // as long as this isn't the last entry, bump this one down
                            if (matchnum < count - 1)
                            {
                                penalty[matchnum + 1] = penalty[matchnum];
                                results[matchnum + 1] = results[matchnum];
                            }

                            results[matchnum] = index;
                            penalty[matchnum] = curpenalty;
                        }
                    }
                }
            }
        }


        // internal helpers
        //-------------------------------------------------
        //  release_current - release bulky memory
        //  structures from the current entry because
        //  we're done with it
        //-------------------------------------------------
        void release_current()
        {
            // skip if no current entry
            if ((m_current >= 0) && (m_current < drivlist_global.s_driver_count))
            {
                // skip if we haven't cached a config
                var cached = m_config.find((UInt32)m_current);
                if (cached != null)
                {
                    // iterate over software lists in this entry and reset
                    foreach (software_list_device swlistdev in new software_list_device_iterator(cached.root_device()))
                        swlistdev.release();
                }
            }
        }
    }
}
