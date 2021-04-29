// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using devcb_write_line = mame.devcb_write<int, uint, mame.devcb_operators_s32_u32, mame.devcb_operators_u32_s32, mame.devcb_constant_1<uint, uint, mame.devcb_operators_u32_u32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using u32 = System.UInt32;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;


namespace mame
{
    public class input_merger_device : device_t
    {
        devcb_write_line m_output_handler;

        u32 m_initval;
        u32 m_xorval;
        int m_active;
        u32 m_state;


        protected input_merger_device(
                machine_config mconfig,
                device_type type,
                string tag,
                device_t owner,
                uint32_t clock,
                u32 initval,
                u32 xorval,
                int active)
            : base(mconfig, type, tag, owner, clock)
        {
            m_output_handler = new devcb_write_line(this);
            m_initval = initval;
            m_xorval = xorval;
            m_active = active;
            m_state = initval;
        }

        //virtual ~input_merger_device() override;


        // device-level overrides
        protected override void device_start()
        {
            m_output_handler.resolve_safe();
            save_item(NAME(new { m_state }));
            m_state = m_initval;
        }


        // callback
        public devcb_write_line.binder output_handler() { return m_output_handler.bind(); }

        // input lines
        //template <unsigned Bit>
        public void in_w<unsigned_Bit>(int state) where unsigned_Bit : uint32_constant, new() { unsigned Bit = new unsigned_Bit().value;  static_assert(Bit < 32, "invalid bit"); machine().scheduler().synchronize(update_state, ((int)Bit << 1) | (state != 0 ? 1 : 0)); }  //DECLARE_WRITE_LINE_MEMBER(in_w) { static_assert(Bit < 32, "invalid bit"); machine().scheduler().synchronize(timer_expired_delegate(FUNC(input_merger_device::update_state), this), (Bit << 1) | (state ? 1U : 0U)); }
        //template <unsigned Bit> void in_set(u8 data) { in_w<Bit>(1); }
        //template <unsigned Bit> void in_clear(u8 data) { in_w<Bit>(0); }


        //-------------------------------------------------
        //  update_state - verify current input line state
        //-------------------------------------------------
        //TIMER_CALLBACK_MEMBER(input_merger_device::update_state)
        void update_state(object ptr, int param)
        {
            if (g.BIT(m_state, param >> 1) != g.BIT(param, 0))
            {
                LOG("state[{0}] = {1}\n", param >> 1, g.BIT(param, 0));
                m_state ^= (u32)(1 << (param >> 1));
                m_output_handler.op((m_state ^ m_xorval) != 0 ? m_active : m_active == 0 ? 1 : 0);
            }
        }
    }


    class input_merger_any_high_device : input_merger_device
    {
        //DEFINE_DEVICE_TYPE(INPUT_MERGER_ANY_HIGH, input_merger_any_high_device, "ipt_merge_any_hi", "Input Merger (any high)")
        static device_t device_creator_input_merger_any_high_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new input_merger_any_high_device(mconfig, tag, owner, clock); }
        public static readonly device_type INPUT_MERGER_ANY_HIGH = DEFINE_DEVICE_TYPE(device_creator_input_merger_any_high_device, "ipt_merge_any_hi", "Input Merger (any high)");

        input_merger_any_high_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, INPUT_MERGER_ANY_HIGH, tag, owner, clock, 0, 0, 1)
        { }
    }


    class input_merger_all_high_device : input_merger_device
    {
        //DEFINE_DEVICE_TYPE(INPUT_MERGER_ALL_HIGH, input_merger_all_high_device, "ipt_merge_all_hi", "Input Merger (all high)")
        static device_t device_creator_input_merger_all_high_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new input_merger_all_high_device(mconfig, tag, owner, clock); }
        public static readonly device_type INPUT_MERGER_ALL_HIGH = DEFINE_DEVICE_TYPE(device_creator_input_merger_all_high_device, "ipt_merge_all_hi", "Input Merger (all high)");

        input_merger_all_high_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, INPUT_MERGER_ALL_HIGH, tag, owner, clock, ~0U, ~0U, 0)
        { }
    }
}
