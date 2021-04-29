// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using size_t = System.UInt32;


namespace mame.netlist
{
    //template<class C, std::size_t N>
    class object_array_base_t<C, size_t_N> : plib.static_vector<C, size_t_N>  //class object_array_base_t : public plib::static_vector<C, N>
        where size_t_N : uint32_constant, new()
    {
        //template<class D, typename... Args>
        ////object_array_base_t(D &dev, const std::initializer_list<const char *> &names, Args&&... args)
        //object_array_base_t(D &dev, std::array<const char *, N> &&names, Args&&... args)
        //{
        //    for (std::size_t i = 0; i<N; i++)
        //        this->emplace_back(dev, pstring(names[i]), std::forward<Args>(args)...);
        //}

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
        //object_array_base_t(D &dev, std::size_t offset, std::size_t qmask, const pstring &fmt)
        //{
        //    for (std::size_t i = 0; i<N; i++)
        //    {
        //        pstring name(formatted(fmt, i+offset));
        //        if ((qmask >> i) & 1)
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
        where size_t_N : uint32_constant, new()
    {
        //using base_type = object_array_base_t<C, N>;
        //using base_type::base_type;
    }


    //template<std::size_t N>
    //class object_array_t<logic_input_t,N> : public object_array_base_t<logic_input_t, N>

    //template<std::size_t N>
    //class object_array_t<logic_output_t,N> : public object_array_base_t<logic_output_t, N>

    //template<std::size_t N>
    //class object_array_t<tristate_output_t, N> : public object_array_base_t<tristate_output_t, N>

} // namespace netlist
