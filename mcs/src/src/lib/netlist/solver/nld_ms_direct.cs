// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using analog_net_t_list_t = mame.std_vector<mame.netlist.analog_net_t>;


/* Disabling dynamic allocation gives a ~10% boost in performance
 * This flag has been added to support continuous storage for arrays
 * going forward in case we implement cuda solvers in the future.
 */
//#define NL_USE_DYNAMIC_ALLOCATION (1)

namespace mame.netlist
{
    namespace devices
    {
        //#define nl_ext_double _float128 // slow, very slow
        //#define nl_ext_double long double // slightly slower
        //#define nl_ext_double nl_double


        //template <std::size_t m_N, std::size_t storage_N>
        class matrix_solver_direct_t: matrix_solver_t
        {
            //friend class matrix_solver_t;


#if NL_USE_DYNAMIC_ALLOCATION
            template <typename T1, typename T2>
            nl_ext_double &A(const T1 &r, const T2 &c) { return m_A[r * m_pitch + c]; }
            template <typename T1>
            nl_ext_double &RHS(const T1 &r) { return m_A[r * m_pitch + N()]; }
#else
            //template <typename T1, typename T2>
            //nl_ext_double &A(const T1 &r, const T2 &c) { return m_A[r][c]; }
            //template <typename T1>
            //nl_ext_double &RHS(const T1 &r) { return m_A[r][N()]; }
#endif
            //nl_double m_last_RHS[storage_N]; // right hand side - contains currents


            //static const std::size_t m_pitch = (((storage_N + 1) + 0) / 1) * 1;
            //static constexpr std::size_t m_pitch = (((storage_N + 1) + 7) / 8) * 8;
            //static const std::size_t m_pitch = (((storage_N + 1) + 15) / 16) * 16;
            //static const std::size_t m_pitch = (((storage_N + 1) + 31) / 32) * 32;
#if (NL_USE_DYNAMIC_ALLOCATION)
            //nl_ext_double * RESTRICT m_A;
            std::vector<nl_ext_double> m_A;
#else
            //nl_ext_double m_A[storage_N][m_pitch];
#endif
            //nl_ext_double m_RHSx[storage_N];

            //const std::size_t m_dim;


            protected matrix_solver_direct_t(UInt32 m_N, UInt32 storage_N, netlist_t anetlist, string name, solver_parameters_t params_, UInt32 size)
                : base(null, null, eSortType.NOSORT, params_)
            {
                throw new emu_unimplemented();
            }

            protected matrix_solver_direct_t(UInt32 m_N, UInt32 storage_N, netlist_t anetlist, string name, eSortType sort, solver_parameters_t params_, UInt32 size)
                : base(null, null, eSortType.NOSORT, params_)
            {
                throw new emu_unimplemented();
            }

            ~matrix_solver_direct_t()
            {
                throw new emu_unimplemented();
            }


            protected override void vsetup(analog_net_t_list_t nets)
            {
                throw new emu_unimplemented();
            }


            protected override void reset()
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

            //constexpr std::size_t N() const { return (m_N == 0) ? m_dim : m_N; }

            //void LE_solve();

            //template <typename T>
            //void LE_back_subst(T * RESTRICT x);
        }
    } //namespace devices
} // namespace netlist
