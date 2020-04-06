// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    // ======================> binding_type_exception
    // exception that is thrown when a bind fails the dynamic_cast
    class binding_type_exception : Exception
    {
        //const std::type_info &m_target_type;
        //const std::type_info &m_actual_type;


        binding_type_exception()  //binding_type_exception(const std::type_info &target_type, const std::type_info &actual_type)
        {
            //: m_target_type(target_type), m_actual_type(actual_type)
        }
    }
}
