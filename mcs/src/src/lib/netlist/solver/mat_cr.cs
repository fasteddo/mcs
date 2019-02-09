// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using uint16_t = System.UInt16;


namespace mame.plib
{
    //template<typename T, int N, typename C = uint16_t>
    struct mat_cr_t
    {
        //typedef C index_type;
        //typedef T value_type;


        //public enum constants_e
        //{
        public const UInt32 FILL_INFINITY = 9999999;
        //}


        // template parameter
        int N;


        public uint16_t [] diag;  //parray<C, N> diag;      // diagonal index pointer n
        public uint16_t [] row_idx;  //parray<C, (N == 0) ? 0 : (N < 0 ? N - 1 : N + 1)> row_idx;      // row index pointer n + 1
        public uint16_t [] col_idx;  //parray<C, N < 0 ? -N * N : N *N> col_idx;       // column index array nz_num, initially (n * n)
        public uint16_t [] A;  //parray<T, N < 0 ? -N * N : N *N> A;    // Matrix elements nz_num, initially (n * n)
        //parray<C, N < 0 ? -N * N / 2 : N * N / 2> nzbd;    // Support for gaussian elimination
        uint16_t [] nzbd;  //parray<C, N < 0 ? -N * (N-1) / 2 : N * (N+1) / 2 > nzbd;    // Support for gaussian elimination
        // contains elimination rows below the diagonal

        UInt32 m_size;
        public UInt32 nz_num;


        public mat_cr_t(int N, UInt32 n)
        {
            this.N = N;


            diag = new uint16_t [n];
            row_idx = new uint16_t [n + 1];
            col_idx = new uint16_t [n * n];
            A = new uint16_t [n * n];
            nzbd = new uint16_t [n * (n + 1) / 2];
            m_size = n;
            nz_num = 0;


            for (int i = 0; i < n + 1; i++)
                A[i] = 0;
        }

        //~mat_cr_t() { }


        UInt32 size() { return m_size; }

        //void set_scalar(const T scalar)
        //void set(C r, C c, T val)


        //template <typename M>
        public KeyValuePair<UInt32, UInt32> gaussian_extend_fill_mat(std.vector<std.vector<UInt32>> fill)  //std::pair<std::size_t, std::size_t> gaussian_extend_fill_mat(M &fill)
        {
            UInt32 ops = 0;
            UInt32 fill_max = 0;

            for (UInt32 k = 0; k < fill.size(); k++)
            {
                ops++; // 1/A(k,k)
                for (UInt32 row = k + 1; row < fill.size(); row++)
                {
                    if (fill[row][k] < FILL_INFINITY)
                    {
                        ops++;
                        for (UInt32 col = k + 1; col < fill[row].size(); col++)
                        {
                            //if (fill[k][col] < FILL_INFINITY)
                            {
                                var f = std.min(fill[row][col], 1 + fill[row][k] + fill[k][col]);
                                if (f < FILL_INFINITY)
                                {
                                    if (f > fill_max)
                                        fill_max = f;
                                    ops += 2;
                                }

                                fill[row][col] = f;
                            }
                        }
                    }
                }
            }

            return new KeyValuePair<uint, uint>(fill_max, ops);
        }


        //template <typename M>
        public void build_from_fill_mat(std.vector<std.vector<UInt32>> f, UInt32 max_fill = FILL_INFINITY - 1, UInt32 band_width = FILL_INFINITY)  //void build_from_fill_mat(const M &f, std::size_t max_fill = FILL_INFINITY - 1, unsigned band_width = FILL_INFINITY)
        {
            uint16_t nz = 0;  //C nz = 0;
            if (nz_num != 0)
                throw new Exception("build_from_mat only allowed on empty CR matrix");

            for (UInt32 k = 0; k < size(); k++)
            {
                row_idx[k] = nz;

                for (UInt32 j = 0; j < size(); j++)
                {
                    if (f[k][j] <= max_fill && std.abs((int)k - (int)j) <= (int)band_width)
                    {
                        col_idx[nz] = (uint16_t)j;  //col_idx[nz] = (C)j;
                        if (j == k)
                            diag[k] = nz;
                        nz++;
                    }
                }
            }

            row_idx[size()] = nz;
            nz_num = nz;
            /* build nzbd */

            UInt32 p = 0;
            for (UInt32 k = 0; k < size(); k++)
            {
                for (UInt32 j = k + 1; j < size(); j++)
                {
                    if (f[j][k] < FILL_INFINITY)
                        nzbd[p++] = (uint16_t)j;  //nzbd[p++] = (C)j;
                }

                nzbd[p++] = 0; // end of sequence
            }
        }


        //void mult_vec(const T * RESTRICT x, T * RESTRICT res)
        //void incomplete_LU_factorization(T * RESTRICT LU)
        //void solveLUx (const T * RESTRICT LU, T * RESTRICT r)
    }
}
