// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    namespace solver
    {
        //template <typename FT, int SIZE>
        abstract class matrix_solver_direct_t : matrix_solver_ext_t
        {
            //friend class matrix_solver_t;


            //typedef FT float_type;


            //const std::size_t m_pitch;


            //static constexpr const std::size_t SIZEABS = plib::parray<FT, SIZE>::SIZEABS();
            //static constexpr const std::size_t m_pitch_ABS = (((SIZEABS + 0) + 7) / 8) * 8;


            //PALIGNAS_VECTOROPT()
            //plib::parray2D<FT, SIZE, m_pitch_ABS> m_A;


            protected matrix_solver_direct_t(int SIZE, netlist_state_t anetlist, string name, analog_net_t.list_t nets, solver_parameters_t params_, UInt32 size)
                : base(SIZE, anetlist, name, nets, params_, size)
            {
                throw new emu_unimplemented();
            }


            public override void reset()
            {
                throw new emu_unimplemented();
#if false
                matrix_solver_t::reset();
#endif
            }


            protected override UInt32 vsolve_non_dynamic(bool newton_raphson)
            {
                throw new emu_unimplemented();
            }


            //unsigned solve_non_dynamic(const bool newton_raphson);

            //void LE_solve();

            //template <typename T>
            //void LE_back_subst(T * RESTRICT x);
        }
    }
}
