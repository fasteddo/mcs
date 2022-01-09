// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    // ======================> http_manager
    public class http_manager
    {
        //DISABLE_COPYING(http_manager);

        //std::shared_ptr<asio::io_context>   m_io_context;
        //std::unique_ptr<webpp::http_server> m_server;
        //std::unique_ptr<webpp::ws_server>   m_wsserver;
        //std::thread                         m_server_thread;
        //std::unordered_map<const char *, std::function<std::tuple<std::string, int, std::string>(std::string)>> m_handlers;


        public http_manager(bool active, short port, string root)
        {
            //throw new emu_unimplemented();
        }

        ~http_manager()
        {
            //throw new emu_unimplemented();
        }


        public void clear()
        {
            //throw new emu_unimplemented();
        }

        //void add(const char *url, std::function<std::tuple<std::string,int,std::string>(std::string)> func) { m_handlers.emplace(url, func); }

        public void update()
        {
            //throw new emu_unimplemented();
        }
    }
}
