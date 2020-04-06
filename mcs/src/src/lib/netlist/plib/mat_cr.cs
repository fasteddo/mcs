// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using uint16_t = System.UInt16;


namespace mame.plib
{
    //template<typename T, int N, typename C = uint16_t>
    class pmatrix_cr_t_double_uint16_t
    {
        //using index_type = C;
        //using value_type = T;

        //COPYASSIGNMOVE(matrix_compressed_rows_t, default)


        public enum constants_e
        {
            FILL_INFINITY = 9999999
        }


        int N;


        public uint16_t [] diag; //parray<index_type, N> diag;      // diagonal index pointer n
        public uint16_t [] row_idx;  //parray<index_type, Np1> row_idx;      // row index pointer n + 1
        public uint16_t [] col_idx;  //parray<index_type, NSQ> col_idx;       // column index array nz_num, initially (n * n)
        public ListBase<double> A;  //parray<value_type, NSQ> A;    // Matrix elements nz_num, initially (n * n)

        public UInt32 nz_num;  //std::size_t nz_num;

        //parray<C, N < 0 ? -N * (N-1) / 2 : N * (N+1) / 2 > nzbd;    // Support for gaussian elimination
        std.vector<uint16_t> [] nzbd;  //parray<std::vector<index_type>, N > nzbd;    // Support for gaussian elimination
        // contains elimination rows below the diagonal
        // FIXME: convert to pvector
        public std.vector<std.vector<uint16_t>> m_ge_par;  //std::vector<std::vector<index_type>> m_ge_par;

        UInt32 m_size;  //std::size_t m_size;


        public pmatrix_cr_t_double_uint16_t(int N, UInt32 n) //std::size_t n)
        {
            this.N = N;

            diag = new uint16_t [n];  //diag(n)
            row_idx = new uint16_t [n + 1];  //row_idx(n+1)
            col_idx = new uint16_t [n * n];  //col_idx(n*n)
            A = new ListBase<double>((int)(n * n));  A.resize((int)(n * n));  //A(n*n)
            nz_num = 0;
            //, nzbd(n * (n+1) / 2)
            nzbd = new std.vector<uint16_t> [n];
            m_size = n;


            for (UInt32 i = 0; i < n + 1; i++)  //for (std::size_t i=0; i<n+1; i++)
            {
                row_idx[i] = 0;
            }
        }

        //~pmatrix_cr_t() = default;


        //constexpr std::size_t size() const noexcept { return (N>0) ? static_cast<std::size_t>(N) : m_size; }

        //void clear()

        //void set_scalar(const T scalar)

        //void set(C r, C c, T val)


        //template <typename M>
        public void build_from_fill_mat(std.vector<std.vector<UInt32>> f, UInt32 max_fill = (UInt32)constants_e.FILL_INFINITY - 1, UInt32 band_width = (UInt32)constants_e.FILL_INFINITY)  //void build_from_fill_mat(const M &f, std::size_t max_fill = FILL_INFINITY - 1, std::size_t band_width = FILL_INFINITY)
        {
            throw new emu_unimplemented();
        }


        //template <typename VTV, typename VTR>
        //void mult_vec(VTR & res, const VTV & x)

        // throws error if P(source)>P(destination)
        //template <typename LUMAT>
        //void slim_copy_from(LUMAT & src)

        // only copies common elements
        //template <typename LUMAT>
        //void reduction_copy_from(LUMAT & src)

        // no checks at all - may crash
        //template <typename LUMAT>
        //void raw_copy_from(LUMAT & src)
    }


    //template<typename B>
    class pGEmatrix_cr_t_pmatrix_cr_t_double_uint16 : pmatrix_cr_t_double_uint16_t  //struct pGEmatrix_cr_t : public B
    {
        public pGEmatrix_cr_t_pmatrix_cr_t_double_uint16(int N, UInt32 n) : base(N, n) { }


        //template <typename M>
        public KeyValuePair<UInt32, UInt32> gaussian_extend_fill_mat(std.vector<std.vector<UInt32>> fill)  //std::pair<std::size_t, std::size_t> gaussian_extend_fill_mat(M &fill)
        {
            throw new emu_unimplemented();
        }
    }
} // namespace plib
