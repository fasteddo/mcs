// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using machine_config_cache = mame.util.lru_cache_map<System.UInt64, mame.machine_config>;  //typedef util::lru_cache_map<std::size_t, std::shared_ptr<machine_config> > machine_config_cache;
using size_t = System.UInt64;
using software_list_device_enumerator = mame.device_type_enumerator<mame.software_list_device>;  //typedef device_type_enumerator<software_list_device> software_list_device_enumerator;
using u64 = System.UInt64;


namespace mame
{
    // ======================> driver_list
    // driver_list is a purely static class that wraps the global driver list
    public class driver_list
    {
        // use variables in drivlist_global
        //static std::size_t const            s_driver_count;
        //static game_driver const * const    s_drivers_sorted[];


        // getters
        public static size_t total() { return drivlist_global.s_driver_count; }


        // any item by index
        public static game_driver driver(size_t index) { g.assert(index < total());  return drivlist_global.s_drivers_sorted[(int)index]; }
        public static int clone(size_t index) { return find(driver(index).parent); }
        public static int non_bios_clone(size_t index) { int result = find(driver(index).parent); return (result >= 0 && ((u64)driver((size_t)result).flags & g.MACHINE_IS_BIOS_ROOT) == 0) ? result : -1; }
        //static int compatible_with(std::size_t index) { return find(driver(index).compatible_with); }


        // any item by driver
        public static int clone(game_driver driver) { int index = find(driver); g.assert(index >= 0); return clone((size_t)index); }
        public static int non_bios_clone(game_driver driver) { int index = find(driver); g.assert(index >= 0); return non_bios_clone((size_t)index); }
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
            var driver = Array.Find(drivlist_global.s_drivers_sorted, d => { index++; return d.name == name; });
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
            return wildstring == null || g.core_strwildcmp(wildstring, str) == 0;
        }
    }


    // ======================> driver_enumerator
    // driver_enumerator enables efficient iteration through the driver list
    class driver_enumerator : driver_list
    {
        const int CONFIG_CACHE_COUNT = 100;


        //typedef util::lru_cache_map<std::size_t, std::shared_ptr<machine_config> > machine_config_cache;


        // internal state
        int m_current;
        size_t m_filtered_count;
        emu_options m_options;
        std.vector<bool> m_included;
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
            m_included = new std.vector<bool>();
            m_included.resize(drivlist_global.s_driver_count);
            m_config = new machine_config_cache(CONFIG_CACHE_COUNT);


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
        public size_t count() { return m_filtered_count; }
        public int current() { return m_current; }
        public emu_options options() { return m_options; }


        // current item
        public game_driver driver() { return driver((size_t)m_current); }
        public machine_config config() { return config((size_t)m_current, m_options); }
        public int clone() { return clone((size_t)m_current); }
        public int non_bios_clone() { return non_bios_clone((size_t)m_current); }
        //int compatible_with() { return driver_list::compatible_with(m_current); }
        public void include() { include((size_t)m_current); }
        void exclude() { exclude((size_t)m_current); }


        // any item by index

        public bool included(size_t index)
        {
            g.assert(index < m_included.size());
            return m_included[index];
        }


        //bool excluded(UInt32 index) const { assert(index >= 0 && index < s_driver_count); return !m_included[index]; }


        public machine_config config(size_t index) { return config(index, m_options); }


        //-------------------------------------------------
        //  config - return a machine_config for the given
        //  driver, allocating on demand if needed
        //-------------------------------------------------
        machine_config config(size_t index, emu_options options)
        {
            g.assert(index < drivlist_global.s_driver_count);

            // if we don't have it cached, add it
            machine_config config = m_config.find(index);  //m_config[index];
            if (config == null)
                config = new machine_config(drivlist_global.s_drivers_sorted[index], options);

            return config;
        }


        public void include(size_t index)
        {
            g.assert(index < m_included.size());
            if (!m_included[index])
            {
                m_included[index] = true;
                m_filtered_count++;
            }
        }


        void exclude(size_t index)
        {
            g.assert(index < m_included.size());
            if (m_included[index])
            {
                m_included[index] = false;
                m_filtered_count--;
            }
        }


        // filtering/iterating

        //-------------------------------------------------
        //  filter - filter the driver list against the
        //  given string
        //-------------------------------------------------
        size_t filter(string filterstring = null)
        {
            // reset the count
            exclude_all();

            // match name against each driver in the list
            for (size_t index = 0; index < drivlist_global.s_driver_count; index++)
            {
                if (matches(filterstring, drivlist_global.s_drivers_sorted[index].name))
                    include(index);
            }

            return m_filtered_count;
        }


        //-------------------------------------------------
        //  filter - filter the driver list against the
        //  given driver
        //-------------------------------------------------
        size_t filter(game_driver driver)
        {
            // reset the count
            exclude_all();

            // match name against each driver in the list
            for (size_t index = 0; index < drivlist_global.s_driver_count; index++)
            {
                if (drivlist_global.s_drivers_sorted[index] == driver)
                    include(index);
            }

            return m_filtered_count;
        }


        //-------------------------------------------------
        //  include_all - include all non-internal drivers
        //-------------------------------------------------
        void include_all()
        {
            std.fill(m_included, true);
            m_filtered_count = m_included.size();

            // always exclude the empty driver
            exclude((size_t)find("___empty"));
        }


        public void exclude_all()
        {
            std.fill(m_included, false);
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
            for (m_current++; (m_current < (int)drivlist_global.s_driver_count) && !m_included[m_current]; m_current++) { }

            // return true if we end up in range
            return (m_current >= 0) && (m_current < (int)drivlist_global.s_driver_count);
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
            for (m_current++; (m_current < (int)drivlist_global.s_driver_count) && m_included[m_current]; m_current++) { }

            // return true if we end up in range
            return (m_current >= 0) && (m_current < (int)drivlist_global.s_driver_count);
        }


        // general helpers

        //void set_current(UInt32 index) { assert(index >= -1 && index <= s_driver_count); m_current = index; }

        //-------------------------------------------------
        //  driver_sort_callback - compare two items in
        //  an array of game_driver pointers
        //-------------------------------------------------
        public void find_approximate_matches(string string_, int count, out int [] results)
        {
            //#undef rand

            results = new int [count];

            // if no name, pick random entries
            if (string_.empty())
            {
                // seed the RNG first
                //srand(osd_ticks());
                Random r = new Random((int)osdcore_global.m_osdcore.osd_ticks());

                // allocate a temporary list
                std.vector<int> templist = new std.vector<int>(m_filtered_count);
                int arrayindex = 0;
                for (int index = 0; index < (int)drivlist_global.s_driver_count; index++)
                {
                    if (m_included[index])
                        templist[arrayindex++] = index;
                }

                g.assert(arrayindex == (int)m_filtered_count);

                // shuffle
                for (int shufnum = 0; shufnum < (int)(4 * drivlist_global.s_driver_count); shufnum++)
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
                std.vector<std.pair<double, int>> penalty = new std.vector<std.pair<double, int>>();
                penalty.reserve((size_t)count);
                string search = unicode_global.ustr_from_utf8(unicode_global.normalize_unicode(string_, unicode_global.unicode_normalization_form.D, true));
                string composed;
                string candidate;

                // scan the entire drivers array
                for (int index = 0; index < (int)drivlist_global.s_driver_count; index++)
                {
                    if (m_included[index])
                    {
                        // cheat on the shortname as it's always lowercase ASCII
                        game_driver drv = drivlist_global.s_drivers_sorted[index];
                        size_t namelen = std.strlen(drv.name);
                        //candidate.resize(namelen);
                        candidate = drv.name;  //std.copy_n(drv.name, namelen, candidate.begin());
                        double curpenalty = util.edit_distance(search, candidate);

                        // if it's not a perfect match, try the description
                        if (curpenalty != 0)
                        {
                            candidate = unicode_global.ustr_from_utf8(unicode_global.normalize_unicode(drv.type.fullname(), unicode_global.unicode_normalization_form.D, true));
                            double p = util.edit_distance(search, candidate);
                            if (p < curpenalty)
                                curpenalty = p;
                        }

                        // also check "<manufacturer> <description>"
                        if (curpenalty != 0)
                        {
                            composed = drv.manufacturer;
                            composed += ' ';
                            composed += drv.type.fullname();
                            candidate = unicode_global.ustr_from_utf8(unicode_global.normalize_unicode(composed, unicode_global.unicode_normalization_form.D, true));
                            double p = util.edit_distance(search, candidate);
                            if (p < curpenalty)
                                curpenalty = p;
                        }

                        // insert into the sorted table of matches
                        //var it = std.upper_bound(penalty.begin(), penalty.end(), std.make_pair(curpenalty, index));
                        int it;
                        for (it = 0; it < penalty.Count; it++)
                        {
                            if (penalty[it].first > curpenalty)
                                break;
                        }

                        if (penalty.Count != it)
                        {
                            if ((int)penalty.size() >= count)
                                penalty.resize((size_t)count - 1);

                            penalty.emplace(it, new std.pair<double, int>(curpenalty, index));
                        }
                        else if ((int)penalty.size() < count)
                        {
                            penalty.emplace(it, new std.pair<double, int>(curpenalty, index));
                        }
                    }
                }

                // copy to output and pad with -1
                //std::fill(
                //        std::transform(
                //            penalty.begin(),
                //            penalty.end(),
                //            results,
                //            [] (std::pair<double, int> const &x) { return x.second; }),
                //        results + count,
                //        -1);
                results = new int [penalty.Count];
                for (int i = 0; i < results.Length; i++)
                    results[i] = penalty[i].second;
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
            if ((m_current >= 0) && (m_current < (int)drivlist_global.s_driver_count))
            {
                // skip if we haven't cached a config
                var cached = m_config.find((size_t)m_current);
                if (cached != null)
                {
                    // iterate over software lists in this entry and reset
                    foreach (software_list_device swlistdev in new software_list_device_enumerator(cached.root_device()))
                        swlistdev.release();
                }
            }
        }
    }
}
