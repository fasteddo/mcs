// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib.omp
{
    public static class pomp_global
    {
        //template <class T>
        //void for_static(const int start, const int end, const T &what)
        //{
        //#if HAS_OPENMP && USE_OPENMP
        //    #pragma omp parallel
        //#endif
        //    {
        //#if HAS_OPENMP && USE_OPENMP
        //        #pragma omp for schedule(static)
        //#endif
        //        for (int i = start; i <  end; i++)
        //            what(i);
        //    }
        //}

        //inline void set_num_threads(const int threads)
        //{
        //#if HAS_OPENMP && USE_OPENMP
        //    omp_set_num_threads(threads);
        //#endif
        //}

        public static int get_max_threads()
        {
//#if HAS_OPENMP && USE_OPENMP
            //return omp_get_max_threads();
//#else
            return 1;
//#endif
        }
    }
}
