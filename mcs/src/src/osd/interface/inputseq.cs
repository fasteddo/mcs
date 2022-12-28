// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.cpp_global;


namespace mame.osd
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // a combination of input_codes, supporting AND/OR and inversion

    public class input_seq
    {
        // constant codes used in sequences
        public static readonly input_code end_code = new input_code(input_device_class.DEVICE_CLASS_INTERNAL, 0, input_item_class.ITEM_CLASS_INVALID, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SEQ_END);
        public static readonly input_code default_code = new input_code(input_device_class.DEVICE_CLASS_INTERNAL, 0, input_item_class.ITEM_CLASS_INVALID, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SEQ_DEFAULT);
        public static readonly input_code not_code = new input_code(input_device_class.DEVICE_CLASS_INTERNAL, 0, input_item_class.ITEM_CLASS_INVALID, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SEQ_NOT);
        public static readonly input_code or_code = new input_code(input_device_class.DEVICE_CLASS_INTERNAL, 0, input_item_class.ITEM_CLASS_INVALID, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SEQ_OR);

        // constant sequences
        public static readonly input_seq empty_seq = new input_seq();


        // internal state
        std.array<input_code, u64_const_16> m_code = new std.array<input_code, u64_const_16>();


        // construction/destruction
        //input_seq() : this(std::make_index_sequence<std::tuple_size<decltype(m_code)>::value>()) { }
        //template <typename... T>
        //input_seq(input_code code_0, T... code_n) : this(std::make_index_sequence<std::tuple_size<decltype(m_code)>::value - sizeof...(T) - 1>(), code_0, code_n...) { }
        //constexpr input_seq(const input_seq &rhs) noexcept = default;
        public input_seq(params input_code [] codes)
        {
            set(codes);
        }


        // operators

        //bool operator==(const input_seq &rhs) const noexcept { return m_code == rhs.m_code; }
        //bool operator!=(const input_seq &rhs) const noexcept { return m_code != rhs.m_code; }

        public input_code this[int index] { get { return (index >= 0 && index < (int)m_code.size()) ? m_code[index] : end_code; } }  //constexpr input_code operator[](int index) const noexcept { return (index >= 0 && index < m_code.size()) ? m_code[index] : end_code; }


        //input_seq &operator+=(input_code code) noexcept;
        //-------------------------------------------------
        //  operator+= - append a code to the end of an
        //  input sequence
        //-------------------------------------------------
        public input_seq append_code_to_sequence_plus(input_code code)
        {
            // if not enough room, return false
            int curlength = length();
            if (curlength < (int)m_code.size())
            {
                m_code[curlength] = code;
                if ((curlength + 1) < (int)m_code.size())
                    m_code[curlength + 1] = end_code;
            }

            return this;
        }


        //input_seq &operator|=(input_code code) noexcept;
        //-------------------------------------------------
        //  operator|= - append a code to a sequence; if
        //  the sequence is non-empty, insert an OR
        //  before the new code
        //-------------------------------------------------
        public input_seq append_code_to_sequence_or(input_code code)
        {
            // overwrite end/default with the new code
            if (m_code[0] == default_code)
            {
                m_code[0] = code;
                m_code[1] = end_code;
            }
            else
            {
                // otherwise, append an OR token and then the new code
                int curlength = length();
                if ((curlength + 1) < (int)m_code.size())
                {
                    m_code[curlength] = or_code;
                    m_code[curlength + 1] = code;
                    if ((curlength + 2) < (int)m_code.size())
                        m_code[curlength + 2] = end_code;
                }
            }

            return this;
        }


        // getters
        public bool empty() { return m_code[0] == end_code; }
        //constexpr int max_size() const noexcept { return std::tuple_size<decltype(m_code)>::value; }
        int length() { throw new emu_unimplemented(); }
        //bool is_valid() const noexcept;
        public bool is_default() { return m_code[0] == default_code; }

        // setters
        void set(params input_code [] codes)  //template <typename... T> void set(input_code code_0, T... code_n) noexcept
        {
            assert(codes.Length <= (int)m_code.size(), "too many codes for input_seq");  //static_assert(sizeof...(T) < std::tuple_size<decltype(m_code)>::value, "too many codes for input_seq");

            //set<0>(code_0, code_n...);
            for (int i = 0; i < (int)m_code.size(); i++)
                m_code[i] = i < codes.Length ? codes[i] : end_code;
        }


        //void reset() noexcept { set(end_code); }

        public void set_default() { set(default_code); }

        public void backspace() { throw new emu_unimplemented(); }

        //void replace(input_code oldcode, input_code newcode) noexcept;


        //static constexpr input_code get_end_code(size_t) noexcept { return end_code; }

        //template <size_t... N, typename... T>
        //input_seq(std::integer_sequence<size_t, N...>, T... code) noexcept : m_code({ code..., get_end_code(N)... })
        //{
        //}
        //template <size_t... N>
        //input_seq(std::integer_sequence<size_t, N...>) noexcept : m_code({ get_end_code(N)... })
        //{
        //}

        //template <unsigned N> void set() noexcept
        //{
        //    std::fill(std::next(m_code.begin(), N), m_code.end(), end_code);
        //}
        //template <unsigned N, typename... T> void set(input_code code_0, T... code_n) noexcept
        //{
        //    m_code[N] = code_0;
        //    set<N + 1>(code_n...);
        //}
    }
}
