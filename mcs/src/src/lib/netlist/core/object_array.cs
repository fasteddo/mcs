// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using size_t = System.UInt64;
using uint32_t = System.UInt32;


namespace mame.netlist
{
    //template<class C, std::size_t N>
    class object_array_base_t<C, size_t_N> : plib.static_vector<C, size_t_N>  //class object_array_base_t : public plib::static_vector<C, N>
        where size_t_N : u64_const, new()
    {
        //template<class D, typename... Args>
        ////object_array_base_t(D &dev, const std::initializer_list<const char *> &names, Args&&... args)
        //object_array_base_t(D &dev, std::array<const char *, N> &&names, Args&&... args)
        //{
        //    for (std::size_t i = 0; i<N; i++)
        //        this->emplace_back(dev, pstring(names[i]), std::forward<Args>(args)...);
        //}
        public object_array_base_t(object dev, params C [] args)
        {
            foreach (C arg in args)
            {
                this.emplace_back(arg);
            }
        }


        //template<class D>
        //object_array_base_t(D &dev, const pstring &fmt)
        //{
        //    for (std::size_t i = 0; i<N; i++)
        //        this->emplace_back(dev, formatted(fmt, i));
        //}

        //template<class D, typename... Args>
        //object_array_base_t(D &dev, std::size_t offset, const pstring &fmt, Args&&... args)
        //{
        //    for (std::size_t i = 0; i<N; i++)
        //        this->emplace_back(dev, formatted(fmt, i+offset), std::forward<Args>(args)...);
        //}

        //template<class D>
        //object_array_base_t(D &dev, std::size_t offset, const pstring &fmt, nldelegate delegate)
        //{
        //    for (std::size_t i = 0; i<N; i++)
        //        this->emplace_back(dev, formatted(fmt, i+offset), delegate);
        //}

        //template<class D>
        //object_array_base_t(D &dev, std::size_t offset, std::size_t output_mask, const pstring &fmt)
        //{
        //    for (std::size_t i = 0; i<N; i++)
        //    {
        //        pstring name(formatted(fmt, i+offset));
        //        if ((output_mask >> i) & 1)
        //            name += "Q";
        //        this->emplace(i, dev, name);
        //    }
        //}


        //object_array_base_t() = default;


        //static pstring formatted(const pstring &fmt, std::size_t n)
        //{
        //    if (N != 1)
        //        return plib::pfmt(fmt)(n);
        //    return plib::pfmt(fmt)("");
        //}
    }


    //template<class C, std::size_t N>
    class object_array_t<C, size_t_N> : object_array_base_t<C, size_t_N>  //class object_array_t : public object_array_base_t<C, N>
        where size_t_N : u64_const, new()
    {
        //using base_type = object_array_base_t<C, N>;
        //using base_type::base_type;

        object_array_t(object dev, params C [] args) : base(dev, args) { }
    }


    //template<std::size_t N>
    class object_array_t_logic_input_t<size_t_N> : object_array_base_t<logic_input_t, size_t_N>  //class object_array_t<logic_input_t,N> : public object_array_base_t<logic_input_t, N>
        where size_t_N : u64_const, new()
    {
        //using base_type = object_array_base_t<logic_input_t, N>;
        //using base_type::base_type;


        public object_array_t_logic_input_t(object dev, params logic_input_t [] args) : base(dev, args) { }


        //template<class D, std::size_t ND>
        //object_array_t(D &dev, std::size_t offset, std::size_t output_mask,
        //    const pstring &fmt, std::array<nl_delegate, ND> &&delegates)
        //{
        //    static_assert(N <= ND, "initializer_list size mismatch");
        //    std::size_t i = 0;
        //    for (auto &e : delegates)
        //    {
        //        if (i < N)
        //        {
        //            pstring name(this->formatted(fmt, i+offset));
        //            if ((output_mask >> i) & 1)
        //                name += "Q";
        //            this->emplace_back(dev, name, e);
        //        }
        //        i++;
        //    }
        //}

        //using value_type = typename plib::fast_type_for_bits<N>::type;
        //using value_type = std::uint32_t;
        public uint32_t op()  //value_type operator ()()
        {
            if (N == 1) return e(0) ;
            if (N == 2) return e(0) | (e(1) << 1);
            if (N == 3) return e(0) | (e(1) << 1) | (e(2) << 2);
            if (N == 4) return e(0) | (e(1) << 1) | (e(2) << 2) | (e(3) << 3);
            if (N == 5) return e(0) | (e(1) << 1) | (e(2) << 2) | (e(3) << 3)
                | (e(4) << 4);
            if (N == 6) return e(0) | (e(1) << 1) | (e(2) << 2) | (e(3) << 3)
                | (e(4) << 4) | (e(5) << 5);
            if (N == 7) return e(0) | (e(1) << 1) | (e(2) << 2) | (e(3) << 3)
                | (e(4) << 4) | (e(5) << 5) | (e(6) << 6);
            if (N == 8) return e(0) | (e(1) << 1) | (e(2) << 2) | (e(3) << 3)
                | (e(4) << 4) | (e(5) << 5) | (e(6) << 6) | (e(7) << 7);

            uint32_t r = 0;  //value_type r(0);
            for (size_t i = 0; i < N; i++)
                r = (uint32_t)(this.op(i).op() << ((int)N - 1)) | (r >> 1);  //r = static_cast<value_type>((*this)[i]() << (N-1)) | (r >> 1);

            return r;
        }

        //template <std::size_t P>
        uint32_t e(size_t P) { return this.op(P).op(); }  //constexpr const value_type &e() const { return (*this)[P](); }
    }


    //template<std::size_t N>
    class object_array_t_logic_output_t<size_t_N> : object_array_base_t<logic_output_t, size_t_N>  //class object_array_t<logic_output_t,N> : public object_array_base_t<logic_output_t, N>
        where size_t_N : u64_const, new()
    {
        //using base_type = object_array_base_t<logic_output_t, N>;
        //using base_type::base_type;


        public object_array_t_logic_output_t(object dev, params logic_output_t [] args) : base(dev, args) { }


        //template <typename T>
        public void push(UInt64 v, netlist_time t)  //void push(const T &v, const netlist_time &t)
        {
            if (N >= 1) this.op(0).push((netlist_sig_t)((v >> 0) & 1U), t);
            if (N >= 2) this.op(1).push((netlist_sig_t)((v >> 1) & 1U), t);
            if (N >= 3) this.op(2).push((netlist_sig_t)((v >> 2) & 1U), t);
            if (N >= 4) this.op(3).push((netlist_sig_t)((v >> 3) & 1U), t);
            if (N >= 5) this.op(4).push((netlist_sig_t)((v >> 4) & 1U), t);
            if (N >= 6) this.op(5).push((netlist_sig_t)((v >> 5) & 1U), t);
            if (N >= 7) this.op(6).push((netlist_sig_t)((v >> 6) & 1U), t);
            if (N >= 8) this.op(7).push((netlist_sig_t)((v >> 7) & 1U), t);
            for (size_t i = 8; i < N; i++)
                this.op(i).push((netlist_sig_t)((v >> (int)i) & 1U), t);
        }


        //template<typename T>
        public void push(UInt64 v, Pointer<netlist_time> t)  //void push(const T &v, const netlist_time * t)
        {
            if (N >= 1) this.op(0).push((netlist_sig_t)((v >> 0) & 1U), t[0]);
            if (N >= 2) this.op(1).push((netlist_sig_t)((v >> 1) & 1U), t[1]);
            if (N >= 3) this.op(2).push((netlist_sig_t)((v >> 2) & 1U), t[2]);
            if (N >= 4) this.op(3).push((netlist_sig_t)((v >> 3) & 1U), t[3]);
            if (N >= 5) this.op(4).push((netlist_sig_t)((v >> 4) & 1U), t[4]);
            if (N >= 6) this.op(5).push((netlist_sig_t)((v >> 5) & 1U), t[5]);
            if (N >= 7) this.op(6).push((netlist_sig_t)((v >> 6) & 1U), t[6]);
            if (N >= 8) this.op(7).push((netlist_sig_t)((v >> 7) & 1U), t[7]);
            for (size_t i = 8; i < N; i++)
                this.op(i).push((netlist_sig_t)((v >> (int)i) & 1U), t[i]);
        }


        //template<typename T, std::size_t NT>
        public void push<size_t_NT>(UInt64 v, std.array<netlist_time, size_t_NT> t)  //void push(const T &v, const std::array<netlist_time, NT> &t)
            where size_t_NT : u64_const, new()
        {
            //throw new emu_unimplemented();
#if false
            static_assert(NT >= N, "Not enough timing entries provided");
#endif

            push(v, t.data());
        }


        //void set_tristate(netlist_sig_t v,
        //    netlist_time ts_off_on, netlist_time ts_on_off) noexcept
        //{
        //    for (std::size_t i = 0; i < N; i++)
        //        (*this)[i].set_tristate(v, ts_off_on, ts_on_off);
        //}
    }


    //template<std::size_t N>
    //class object_array_t<tristate_output_t, N> : public object_array_base_t<tristate_output_t, N>
}
