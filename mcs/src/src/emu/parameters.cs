// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    // ======================> parameters_manager
    public class parameters_manager
    {
        running_machine m_machine;              // reference to owning machine
        std.unordered_map<string, string> m_parameters = new std.unordered_map<string, string>();  //std::unordered_map<std::string,std::string>       m_parameters;


        // construction/destruction
        public parameters_manager(running_machine machine)
        {
            m_machine = machine;
        }


        // getters
        //running_machine &machine() const { return m_machine; }
        //std::string lookup(std::string tag) const;


        // setters
        public void add(string tag, string value)
        {
            m_parameters.insert(std.make_pair(tag, value));
        } 
    }
}
