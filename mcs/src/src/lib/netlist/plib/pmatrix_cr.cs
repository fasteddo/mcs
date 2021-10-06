// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using size_t = System.UInt64;
using uint16_t = System.UInt16;
using unsigned = System.UInt32;


namespace mame.plib
{
    interface pmatrix_cr_operators<T>
    {
        T cast(UInt64 a);
        int cast_int32(T a);
        UInt32 cast_uint32(T a);
        T add(T a, UInt64 b);
    }

    class pmatrix_cr_operators_uint16 : pmatrix_cr_operators<uint16_t>
    {
        public uint16_t cast(UInt64 a) { return (uint16_t)a; }
        public int cast_int32(uint16_t a) { return a; }
        public UInt32 cast_uint32(uint16_t a) { return a; }
        public uint16_t add(uint16_t a, UInt64 b) { return (uint16_t)(a + b); }
    }


    //template<typename T, int N, typename C = uint16_t>
    class pmatrix_cr<T, int_N, C, C_OPS>  //struct pmatrix_cr
        where int_N : int_const, new()
        where C_OPS : pmatrix_cr_operators<C>, new()
    {
        static readonly int N = new int_N().value;
        static readonly C_OPS ops = new C_OPS();


        //using index_type = C;
        //using value_type = T;


        //static constexpr const int NSQ = (N < 0 ? -N * N : N * N);
        //static constexpr const int Np1 = (N == 0) ? 0 : (N < 0 ? N - 1 : N + 1);


        //PCOPYASSIGNMOVE(pmatrix_cr_t, default)


        public enum constants_e
        {
            FILL_INFINITY = 9999999
        }


        public C [] diag; //parray<index_type, N> diag;      // diagonal index pointer n
        public C [] row_idx;  //parray<index_type, Np1> row_idx;      // row index pointer n + 1
        public C [] col_idx;  //parray<index_type, NSQ> col_idx;       // column index array nz_num, initially (n * n)
        public MemoryContainer<T> A;  //parray<value_type, NSQ> A;    // Matrix elements nz_num, initially (n * n)

        public size_t nz_num;

        ////parray<std::vector<index_type>, N > m_nzbd;    // Support for gaussian elimination
        pmatrix2d_vrl<C> m_nzbd;    // Support for gaussian elimination  //pmatrix2d_vrl<index_type> m_nzbd;    // Support for gaussian elimination
        size_t m_size;


        public pmatrix_cr(size_t n)
        {
            diag = new C [n];  //diag(n)
            row_idx = new C [n + 1];  //row_idx(n+1)
            col_idx = new C [n * n];  //col_idx(n*n)
            A = new MemoryContainer<T>((int)(n * n), true);  //A(n*n)
            nz_num = 0;
            ////, nzbd(n * (n+1) / 2)
            m_nzbd = new pmatrix2d_vrl<C>(n, n);
            m_size = n;


            for (size_t i = 0; i < n + 1; i++)
            {
                row_idx[i] = ops.cast(0);
            }
        }

        //~pmatrix_cr_t() = default;


        protected size_t size() { return (N > 0) ? (size_t)N : m_size; }  //constexpr std::size_t size() const noexcept { return (N>0) ? static_cast<std::size_t>(N) : m_size; }

        //void clear()

        public void set_scalar(T scalar) { throw new emu_unimplemented(); }  //void set_scalar(T scalar) noexcept

        //void set_row_scalar(C r, T val) noexcept

        //void set(C r, C c, T val)


        //template <typename M>
        public void build_from_fill_mat(std.vector<std.vector<unsigned>> f, size_t max_fill = (size_t)constants_e.FILL_INFINITY - 1, size_t band_width = (size_t)constants_e.FILL_INFINITY)  //void build_from_fill_mat(const M &f, std::size_t max_fill = FILL_INFINITY - 1, std::size_t band_width = FILL_INFINITY)
        {
            C nz = ops.cast(0);  //C nz = 0;
            if (nz_num != 0)
                throw new pexception("build_from_mat only allowed on empty CR matrix");

            for (size_t k = 0; k < size(); k++)
            {
                row_idx[k] = nz;

                for (size_t j = 0; j < size(); j++)
                {
                    if (f[k][j] <= max_fill && plib.pg.abs((int)k - (int)j) <= (int)band_width)
                    {
                        col_idx[ops.cast_int32(nz)] = ops.cast(j);  //col_idx[nz] = static_cast<C>(j);
                        if (j == k)
                            diag[k] = nz;
                        nz = ops.add(nz, 1);  //nz++;
                    }
                }
            }

            row_idx[size()] = nz;
            nz_num = ops.cast_uint32(nz);

            // build nzbd

            for (size_t k = 0; k < size(); k++)
            {
                for (size_t j = k + 1; j < size(); j++)
                {
                    if (f[j][k] < (size_t)constants_e.FILL_INFINITY)
                        m_nzbd.set(k, m_nzbd.colcount(k), ops.cast(j));  //m_nzbd.set(k, m_nzbd.colcount(k), narrow_cast<C>(j));
                }

                m_nzbd.set(k, m_nzbd.colcount(k), ops.cast(0)); // end of sequence
            }
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


        public Pointer<C> nzbd(size_t row) { return m_nzbd.op(row); }
        public size_t nzbd_count(size_t row) { return m_nzbd.colcount(row) - 1; }
    }


    class pmatrix_cr<T, int_N> : pmatrix_cr<T, int_N, uint16_t, pmatrix_cr_operators_uint16>
        where int_N : int_const, new()
    {
        public pmatrix_cr(size_t n) : base(n) { }
    }


    //template<typename B>
    class pGEmatrix_cr<B, int_N> : pmatrix_cr<B, int_N>  //struct pGEmatrix_cr : public B
        where int_N : int_const, new()
    {
        //using base_type = B;
        //using index_type = typename base_type::index_type;


        public std.vector<std.vector<size_t>> m_ge_par = new std.vector<std.vector<size_t>>();  // parallel execution support for Gauss


        public pGEmatrix_cr(size_t n) : base(n) { }


        //template <typename M>
        public std.pair<size_t, size_t> gaussian_extend_fill_mat(std.vector<std.vector<unsigned>> fill)  //std::pair<std::size_t, std::size_t> gaussian_extend_fill_mat(M &fill)
        {
            size_t ops = 0;
            size_t fill_max = 0;

            for (size_t k = 0; k < fill.size(); k++)
            {
                ops++; // 1/A(k,k)
                for (size_t row = k + 1; row < fill.size(); row++)
                {
                    if (fill[row][k] < (size_t)pmatrix_cr<B, int_N>.constants_e.FILL_INFINITY)
                    {
                        ops++;
                        for (size_t col = k + 1; col < fill[row].size(); col++)
                            //if (fill[k][col] < FILL_INFINITY)
                            {
                                var f = std.min(fill[row][col], 1 + fill[row][k] + fill[k][col]);
                                if (f < (size_t)pmatrix_cr<B, int_N>.constants_e.FILL_INFINITY)
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

            build_parallel_gaussian_execution_scheme(fill);

            return new std.pair<size_t, size_t>(fill_max, ops);
        }


        //template <typename V>
        public void gaussian_elimination(B [] RHS) { throw new emu_unimplemented(); }  //void gaussian_elimination(V & RHS)


        //int get_parallel_level(std::size_t k) const
        //template <typename V>
        //void gaussian_elimination_parallel(V & RHS)


        //template <typename V1, typename V2>
        public void gaussian_back_substitution(Pointer<nl_fptype> V, nl_fptype [] RHS) { throw new emu_unimplemented(); }  //void gaussian_back_substitution(V1 &V, const V2 &RHS)


        //template <typename V1>
        //void gaussian_back_substitution(V1 &V)


        //template <typename M>
        void build_parallel_gaussian_execution_scheme(std.vector<std.vector<unsigned>> fill)  //void build_parallel_gaussian_execution_scheme(const M &fill)
        {
            // calculate parallel scheme for gaussian elimination
            std.vector<std.vector<size_t>> rt = new std.vector<std.vector<size_t>>(base.size());
            for (size_t k = 0; k < base.size(); k++)
            {
                rt[k] = new std.vector<size_t>();
                for (size_t j = k + 1; j < base.size(); j++)
                {
                    if (fill[j][k] < (size_t)pmatrix_cr<B, int_N>.constants_e.FILL_INFINITY)
                    {
                        rt[k].push_back(j);
                    }
                }
            }

            std.vector<size_t> levGE = new std.vector<size_t>(base.size(), 0);
            size_t cl = 0;

            for (size_t k = 0; k < base.size(); k++ )
            {
                if (levGE[k] >= cl)
                {
                    std.vector<size_t> t = rt[k];
                    for (size_t j = k + 1; j < base.size(); j++ )
                    {
                        bool overlap = false;
                        // is there overlap
                        if (plib.container.contains(t, j))
                            overlap = true;

                        foreach (var x in rt[j])
                        {
                            if (plib.container.contains(t, x))
                            {
                                overlap = true;
                                break;
                            }
                        }

                        if (overlap)
                        {
                            levGE[j] = cl + 1;
                        }
                        else
                        {
                            t.push_back(j);
                            foreach (var x in rt[j])
                                t.push_back(x);
                        }
                    }
                    cl++;
                }
            }

            m_ge_par.clear();
            m_ge_par.resize(cl + 1);
            m_ge_par.Fill(() => { return new std.vector<size_t>(); });
            for (size_t k = 0; k < base.size(); k++)
            {
                m_ge_par[levGE[k]].push_back(k);
            }

            //for (std::size_t k = 0; k < m_ge_par.size(); k++)
            //  printf("%d %d\n", (int) k, (int) m_ge_par[k].size());
        }
    }


    //template<typename B>
    //struct pLUmatrix_cr : public B

} // namespace plib
