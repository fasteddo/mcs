// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using size_t = System.UInt64;
using system_list_system_reference_vector = mame.std.vector<mame.ui_system_info>;  //using system_reference_vector = std::vector<system_reference>;
using system_list_system_vector = mame.std.vector<mame.ui_system_info>;  //using system_vector = std::vector<ui_system_info>;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.osdfile_global;
using static mame.unicode_global;


namespace mame.ui
{
    class system_list
    {
        public enum available : unsigned
        {
            AVAIL_NONE                  = 0U,
            AVAIL_SYSTEM_NAMES          = 1U << 0,
            AVAIL_SORTED_LIST           = 1U << 1,
            AVAIL_BIOS_COUNT            = 1U << 2,
            AVAIL_UCS_SHORTNAME         = 1U << 3,
            AVAIL_UCS_DESCRIPTION       = 1U << 4,
            AVAIL_UCS_MANUF_DESC        = 1U << 5,
            AVAIL_UCS_DFLT_DESC         = 1U << 6,
            AVAIL_UCS_MANUF_DFLT_DESC   = 1U << 7,
            AVAIL_FILTER_DATA           = 1U << 8,
        }


        //using system_vector = std::vector<ui_system_info>;
        //using system_reference = std::reference_wrapper<ui_system_info>;
        //using system_reference_vector = std::vector<system_reference>;


        // synchronisation
        object m_mutex = new object();  //std::mutex                      m_mutex;
        //std::condition_variable         m_condition;
        //std::unique_ptr<std::thread>    m_thread;
        bool m_started;  //std::atomic<bool>               m_started;
        unsigned m_available;  //std::atomic<unsigned>           m_available;

        // data
        system_list_system_vector m_systems = new system_list_system_reference_vector();
        system_list_system_reference_vector m_sorted_list = new system_list_system_reference_vector();
        machine_filter_data m_filter_data = new machine_filter_data();
        int m_bios_count;


        static system_list data = new system_list();

        public static system_list instance()
        {
            //static system_list data;
            return data;
        }


        system_list()
        {
            m_started = false;
            m_available = (unsigned)available.AVAIL_NONE;
            m_bios_count = 0;
        }

        //~system_list();
        //{
        //    if (m_thread)
        //        m_thread->join();
        //}


        public void cache_data(ui_options options)
        {
            lock (m_mutex)  //std::unique_lock<std::mutex> lock(m_mutex);
            {
                if (!m_started)
                {
                    m_started = true;
                    //m_thread = std::make_unique<std::thread>(
                    //        [this, datpath = std::string(options.history_path()), titles = std::string(options.system_names())]
                    //        {
                    //            do_cache_data(datpath, titles);
                    //        });
                    var datpath = options.history_path();
                    var titles = options.system_names();
                    do_cache_data(datpath, titles);
                }
            }
        }


        public void reset_cache()
        {
            //std::unique_lock<std::mutex> lock(m_mutex);
            //if (m_thread)
            //    m_thread->join();
            //m_thread.reset();
            m_started = false;
            m_available = (unsigned)available.AVAIL_NONE;
            m_systems.clear();
            m_sorted_list.clear();
            m_filter_data = new machine_filter_data();
            m_bios_count = 0;
        }


        public bool is_available(available desired)
        {
            return (m_available & (unsigned)desired) == (unsigned)desired;  //return (m_available.load(std::memory_order_acquire) & desired) == desired;
        }


        void wait_available(available desired)
        {
            if (!is_available(desired))
            {
                assert(m_started);
                lock (m_mutex)  //std::unique_lock<std::mutex> lock(m_mutex);
                {
                    //if (!is_available(desired))
                    //    m_condition.wait(lock, [this, desired] () { return is_available(desired); });
                }
            }
        }


        public system_list_system_vector systems()
        {
            wait_available(available.AVAIL_SYSTEM_NAMES);
            return m_systems;
        }


        public system_list_system_reference_vector sorted_list()
        {
            wait_available(available.AVAIL_SORTED_LIST);
            return m_sorted_list;
        }


        public int bios_count()
        {
            wait_available(available.AVAIL_BIOS_COUNT);
            return m_bios_count;
        }


        //bool unavailable_systems()
        //{
        //    wait_available(AVAIL_SORTED_LIST);
        //    return std::find_if(m_sorted_list.begin(), m_sorted_list.end(), [] (ui_system_info const &info) { return !info.available; }) != m_sorted_list.end();
        //}


        public machine_filter_data filter_data()
        {
            wait_available(available.AVAIL_FILTER_DATA);
            return m_filter_data;
        }


        void notify_available(available value)
        {
            //std::unique_lock<std::mutex> lock(m_mutex);
            //m_available.fetch_or(value, std::memory_order_release);
            //m_condition.notify_all();
        }


        void do_cache_data(string datpath, string titles)
        {
            // try to open the titles file for optimisation reasons
            emu_file titles_file = new emu_file(datpath, OPEN_FLAG_READ);
            bool try_titles = !titles.empty() && !titles_file.open(titles);

            // generate full list - initially ordered by shortname
            populate_list(!try_titles);

            // notify that BIOS count is valie
            notify_available(available.AVAIL_BIOS_COUNT);

            // try to load localised descriptions
            if (try_titles)
            {
                load_titles(titles_file.core_file_get());

                // populate parent descriptions while still ordered by shortname
                // already done on the first pass if built-in titles are used
                populate_parents();
            }

            // system names are finalised now
            notify_available(available.AVAIL_SYSTEM_NAMES);

            // get rid of the "empty" driver - we don't need positions to line up any more
            m_sorted_list.reserve(m_systems.size() - 1);
            var empty = driver_list.find(___empty.driver____empty);
            foreach (ui_system_info info in m_systems)
            {
                if (info.index != empty)
                    m_sorted_list.emplace_back(info);
            }

            // sort drivers and notify
            //std::collate<wchar_t> const &coll = std::use_facet<std::collate<wchar_t> >(std::locale());
            //auto const compare_names =
            //        [&coll] (std::wstring const &wx, std::wstring const &wy) -> bool
            //        {
            //            return 0 > coll.compare(wx.data(), wx.data() + wx.size(), wy.data(), wy.data() + wy.size());
            //        };
            Func<string, string, int> compare_names = (string wx, string wy) => { return wx.CompareTo(wy); };
            //std::stable_sort(
            //        m_sorted_list.begin(),
            //        m_sorted_list.end(),
            //        [&compare_names] (ui_system_info const &lhs, ui_system_info const &rhs)
            m_sorted_list.Sort((ui_system_info lhs, ui_system_info rhs) =>
            {
                game_driver x = lhs.driver;
                game_driver y = rhs.driver;

                if (!lhs.is_clone && !rhs.is_clone)
                {
                    return compare_names(
                            lhs.reading_description.empty() ? wstring_from_utf8(lhs.description) : lhs.reading_description,
                            rhs.reading_description.empty() ? wstring_from_utf8(rhs.description) : rhs.reading_description);
                }
                else if (lhs.is_clone && rhs.is_clone)
                {
                    if (std.strcmp(x.parent, y.parent) == 0)
                    {
                        return compare_names(
                                lhs.reading_description.empty() ? wstring_from_utf8(lhs.description) : lhs.reading_description,
                                rhs.reading_description.empty() ? wstring_from_utf8(rhs.description) : rhs.reading_description);
                    }
                    else
                    {
                        return compare_names(
                                lhs.reading_parent.empty() ? wstring_from_utf8(lhs.parent) : lhs.reading_parent,
                                rhs.reading_parent.empty() ? wstring_from_utf8(rhs.parent) : rhs.reading_parent);
                    }
                }
                else if (!lhs.is_clone && rhs.is_clone)
                {
                    if (std.strcmp(x.name, y.parent) == 0)
                    {
                        return 0;  //return true;
                    }
                    else
                    {
                        return compare_names(
                                lhs.reading_description.empty() ? wstring_from_utf8(lhs.description) : lhs.reading_description,
                                rhs.reading_parent.empty() ? wstring_from_utf8(rhs.parent) : rhs.reading_parent);
                    }
                }
                else
                {
                    if (std.strcmp(x.parent, y.name) == 0)
                    {
                        return -1;  //return false;
                    }
                    else
                    {
                        return compare_names(
                                lhs.reading_parent.empty() ? wstring_from_utf8(lhs.parent) : lhs.reading_parent,
                                rhs.reading_description.empty() ? wstring_from_utf8(rhs.description) : rhs.reading_description);
                    }
                }
            });

            notify_available(available.AVAIL_SORTED_LIST);

            // sort manufacturers and years
            m_filter_data.finalise();
            notify_available(available.AVAIL_FILTER_DATA);

            // convert shortnames to UCS-4
            foreach (ui_system_info info in m_sorted_list)
                info.ucs_shortname = ustr_from_utf8(normalize_unicode(info.driver.name, unicode_normalization_form.D, true));

            notify_available(available.AVAIL_UCS_SHORTNAME);

            // convert descriptions to UCS-4
            foreach (ui_system_info info in m_sorted_list)
                info.ucs_description = ustr_from_utf8(normalize_unicode(info.description, unicode_normalization_form.D, true));

            notify_available(available.AVAIL_UCS_DESCRIPTION);

            // convert "<manufacturer> <description>" to UCS-4
            string buf;
            foreach (ui_system_info info in m_sorted_list)
            {
                buf = info.driver.manufacturer;
                buf = buf.append_(1, ' ');
                buf = buf.append_(info.description);
                info.ucs_manufacturer_description = ustr_from_utf8(normalize_unicode(buf, unicode_normalization_form.D, true));
            }

            notify_available(available.AVAIL_UCS_MANUF_DESC);

            // convert default descriptions to UCS-4
            if (try_titles)
            {
                foreach (ui_system_info info in m_sorted_list)
                {
                    string fullname = info.driver.type.fullname();
                    if (info.description != fullname)
                        info.ucs_default_description = ustr_from_utf8(normalize_unicode(fullname, unicode_normalization_form.D, true));
                }
            }

            notify_available(available.AVAIL_UCS_DFLT_DESC);

            // convert "<manufacturer> <default description>" to UCS-4
            if (try_titles)
            {
                foreach (ui_system_info info in m_sorted_list)
                {
                    string fullname = info.driver.type.fullname();
                    if (info.description != fullname)
                    {
                        buf = info.driver.manufacturer;
                        buf = buf.append_(1, ' ');
                        buf = buf.append_(fullname);
                        info.ucs_manufacturer_default_description = ustr_from_utf8(normalize_unicode(buf, unicode_normalization_form.D, true));
                    }
                }
            }

            notify_available(available.AVAIL_UCS_MANUF_DFLT_DESC);
        }


        void populate_list(bool copydesc)
        {
            m_systems.reserve(driver_list.total());
            std.unordered_set<string> manufacturers;
            std.unordered_set<string> years;
            for (int x = 0; x < (int)driver_list.total(); ++x)
            {
                game_driver driver = driver_list.driver((size_t)x);
                ui_system_info ins = new ui_system_info(driver, x, false);
                m_systems.emplace_back(ins);
                if (driver != ___empty.driver____empty)
                {
                    if ((driver.flags & machine_flags.type.IS_BIOS_ROOT) != 0)
                        ++m_bios_count;

                    if ((driver.parent.Length >= 1 && driver.parent[0] != '0') || (driver.parent.Length >= 2 && driver.parent[1] != 0))  //if ((driver.parent[0] != '0') || driver.parent[1])
                    {
                        var parentindex = driver_list.find(driver.parent);
                        if (copydesc)
                        {
                            if (0 <= parentindex)
                            {
                                game_driver parentdriver = driver_list.driver((size_t)parentindex);
                                ins.is_clone = (parentdriver.flags & machine_flags.type.IS_BIOS_ROOT) == 0;
                                ins.parent = parentdriver.type.fullname();
                            }
                            else
                            {
                                ins.is_clone = false;
                                ins.parent = driver.parent;
                            }
                        }
                        else
                        {
                            ins.is_clone = (0 <= parentindex) && (driver_list.driver((size_t)parentindex).flags & machine_flags.type.IS_BIOS_ROOT) == 0;
                        }
                    }

                    if (copydesc)
                        ins.description = driver.type.fullname();

                    m_filter_data.add_manufacturer(driver.manufacturer);
                    m_filter_data.add_year(driver.year);
                }
            }
        }


        void load_titles(util.core_file file)
        {
            throw new emu_unimplemented();
        }


        void populate_parents()
        {
            throw new emu_unimplemented();
        }
    }
}
