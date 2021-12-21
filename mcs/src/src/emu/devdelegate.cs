// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame.emu
{
    // ======================> named_delegate
    //template <typename Signature>
    //class named_delegate : public delegate<Signature>

    // ======================> device_delegate
    // device_delegate is a delegate that wraps with a device tag and can be easily
    // late bound without replicating logic everywhere
    //template<typename Signature>
    //class device_delegate : public named_delegate<Signature>, public device_delegate_helper

    //template <typename ReturnType, typename... Params>
    //class device_delegate<ReturnType (Params...)> : protected named_delegate<ReturnType (Params...)>, public detail::device_delegate_helper
}
