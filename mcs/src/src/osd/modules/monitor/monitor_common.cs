// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    abstract class monitor_module_base : monitor_module
    {
        //std::map<std::uint64_t, std::shared_ptr<osd_monitor_info>> m_monitor_index;

        bool m_initialized;


        protected monitor_module_base(string type, string name)
            : base(type, name)
        {
            m_initialized = false;
        }


        //std::shared_ptr<osd_monitor_info> pick_monitor(osd_options& options, int index) override;
        //std::shared_ptr<osd_monitor_info> monitor_from_handle(std::uint64_t handle) override;


        public override int init(osd_options options)
        {
            if (!m_initialized)
            {
                int result = init_internal(options);

                if (result == 0)
                    m_initialized = true;

                return result;
            }

            return 0;
        }


        //void exit() override;


        protected abstract int init_internal(osd_options options);

        //void add_monitor(std::shared_ptr<osd_monitor_info> monitor);


        //std::shared_ptr<osd_monitor_info> pick_monitor_internal(osd_options& options, int index);
        //static float get_aspect(const char *defdata, const char *data, int report_error);
    }
}
