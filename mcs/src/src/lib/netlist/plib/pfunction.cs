// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using size_t = System.UInt32;
using size_t_constant_20 = mame.uint32_constant_20;
using uint16_t = System.UInt16;


namespace mame.plib
{
    //============================================================
    //  function evaluation
    //============================================================

    enum rpn_cmd
    {
        ADD,
        MULT,
        SUB,
        DIV,
        EQ,
        NE,
        LT,
        GT,
        LE,
        GE,
        IF,
        NEG,    // unary minus
        POW,
        LOG,
        SIN,
        COS,
        MIN,
        MAX,
        RAND, /// random number between 0 and 1
        TRUNC,

        PUSH_CONST,
        PUSH_INPUT,

        LP, // Left parenthesis - for infix parsing
        RP  // right parenthesis - for infix parsing
    }


    /// \brief Class providing support for evaluating expressions
    ///
    ///  \tparam NT Number type, should be float or double
    ///
    //template <typename NT>
    class pfunction<NT, NT_OPS> 
        where NT_OPS : constants_operators<NT>, new()
    {
        static constants_operators<NT> ops = new NT_OPS();


        //using value_type = NT;

        //using inputs_container = std::vector<pstring>;
        //using values_container = std::vector<value_type>;


        class rpn_inst
        {
            public rpn_cmd m_cmd;
            //union
            //{
            //    NT          val;
            //    std::size_t index;
            //} m_param;
            public struct param_union
            {
                public NT val;
                public size_t index;
            }
            public param_union m_param;

            public rpn_inst() { m_cmd = rpn_cmd.ADD;  m_param = new param_union() { val = plib.constants<NT, NT_OPS>.zero(), index = 0 }; }
        }


        const size_t MAX_STACK = 32;


        class pcmd_t
        {
            public rpn_cmd cmd;
            public int adj;
            public int prio;
        }

        static readonly std.map<string, pcmd_t> lpcmds = new std.map<string, pcmd_t>()
        {
            { "^",     new pcmd_t { cmd = rpn_cmd.POW,  adj =  1, prio = 30 } },
            { "neg",   new pcmd_t { cmd = rpn_cmd.NEG,  adj =  0, prio = 25 } },
            { "+",     new pcmd_t { cmd = rpn_cmd.ADD,  adj =  1, prio = 10 } },
            { "-",     new pcmd_t { cmd = rpn_cmd.SUB,  adj =  1, prio = 10 } },
            { "*",     new pcmd_t { cmd = rpn_cmd.MULT, adj =  1, prio = 20 } },
            { "/",     new pcmd_t { cmd = rpn_cmd.DIV,  adj =  1, prio = 20 } },
            { "<",     new pcmd_t { cmd = rpn_cmd.LT,   adj =  1, prio =  9 } },
            { ">",     new pcmd_t { cmd = rpn_cmd.GT,   adj =  1, prio =  9 } },
            { "<=",    new pcmd_t { cmd = rpn_cmd.LE,   adj =  1, prio =  9 } },
            { ">=",    new pcmd_t { cmd = rpn_cmd.GE,   adj =  1, prio =  9 } },
            { "==",    new pcmd_t { cmd = rpn_cmd.EQ,   adj =  1, prio =  8 } },
            { "!=",    new pcmd_t { cmd = rpn_cmd.NE,   adj =  1, prio =  8 } },
            { "if",    new pcmd_t { cmd = rpn_cmd.IF,   adj =  2, prio =  0 } },
            { "pow",   new pcmd_t { cmd = rpn_cmd.POW,  adj =  1, prio =  0 } },
            { "log",   new pcmd_t { cmd = rpn_cmd.LOG,  adj =  0, prio =  0 } },
            { "sin",   new pcmd_t { cmd = rpn_cmd.SIN,  adj =  0, prio =  0 } },
            { "cos",   new pcmd_t { cmd = rpn_cmd.COS,  adj =  0, prio =  0 } },
            { "max",   new pcmd_t { cmd = rpn_cmd.MAX,  adj =  1, prio =  0 } },
            { "min",   new pcmd_t { cmd = rpn_cmd.MIN,  adj =  1, prio =  0 } },
            { "trunc", new pcmd_t { cmd = rpn_cmd.TRUNC,adj =  0, prio =  0 } },
            { "rand",  new pcmd_t { cmd = rpn_cmd.RAND, adj = -1, prio =  0 } },

            { "(",     new pcmd_t { cmd = rpn_cmd.LP,   adj =  0, prio =  1 } },
            { ")",     new pcmd_t { cmd = rpn_cmd.RP,   adj =  0, prio =  1 } },
        };

        static std.map<string, pcmd_t> pcmds() 
        {
            return lpcmds;
        }


        // FIXME: Exa parsing conflicts with e,E parsing
        static std.map<string, NT> units_si_stat = new std.map<string, NT>()
        {
            //{ "Y", narrow_cast<F>(1e24) }, // NOLINT: Yotta
            //{ "Z", narrow_cast<F>(1e21) }, // NOLINT: Zetta
            //{ "E", narrow_cast<F>(1e18) }, // NOLINT: Exa
            { "P", ops.cast(1e15) }, // NOLINT: Peta
            { "T", ops.cast(1e12) }, // NOLINT: Tera
            { "G", ops.cast( 1e9) }, // NOLINT: Giga
            { "M", ops.cast( 1e6) }, // NOLINT: Mega
            { "k", ops.cast( 1e3) }, // NOLINT: Kilo
            { "h", ops.cast( 1e2) }, // NOLINT: Hekto
            //{ "da", narrow_cast<F>(1e1) }, // NOLINT: Deka
            { "d", ops.cast(1e-1) }, // NOLINT: Dezi
            { "c", ops.cast(1e-2) }, // NOLINT: Zenti
            { "m", ops.cast(1e-3) }, // NOLINT: Milli
            { "µ", ops.cast(1e-6) }, // NOLINT: Mikro
            { "n", ops.cast(1e-9) }, // NOLINT: Nano
            { "p", ops.cast(1e-12) }, // NOLINT: Piko
            { "f", ops.cast(1e-15) }, // NOLINT: Femto
            { "a", ops.cast(1e-18) }, // NOLINT: Atto
            { "z", ops.cast(1e-21) }, // NOLINT: Zepto
            { "y", ops.cast(1e-24) }, // NOLINT: Yokto
        };

        static std.map<string, NT> units_si()
        {
            return units_si_stat;
        }


        std.vector<rpn_inst> m_precompiled = new std.vector<rpn_inst>(); //!< precompiled expression

        uint16_t m_lfsr; //!< lfsr used for generating random numbers


        /// \brief Constructor with state saving support
        ///
        /// \param name Name of this object
        /// \param owner Owner of this object
        /// \param state_manager State manager to handle saving object state
        ///
        ///
        //pfunction(const pstring &name, const void *owner, state_manager_t &state_manager)
        //: m_lfsr(0xace1U)
        //{
        //    state_manager.save_item(owner, m_lfsr, name + ".lfsr");
        //}


        /// \brief Constructor without state saving support
        ///
        public pfunction()
        {
            m_lfsr = 0xace1;
        }


        /// \brief Constructor with compile
        ///
        //pfunction(const pstring &expr, const inputs_container &inputs = inputs_container())
        //: m_lfsr(0xace1U) // NOLINT
        //{
        //    compile(expr, inputs);
        //}

        /// \brief Evaluate the expression
        ///
        /// \param values for input variables, e.g. {1.1, 2.2}
        /// \return value of expression
        ///
        //value_type operator()(const values_container &values = values_container()) noexcept
        //{
        //    return evaluate(values);
        //}


        /// \brief Compile an expression
        ///
        /// \param expr infix or postfix expression. default is infix, postrix
        ///          to be prefixed with rpn, e.g. "rpn:A B + 1.3 /"
        /// \param inputs Vector of input variables, e.g. {"A","B"}
        ///
        //void compile(const pstring &expr, const inputs_container &inputs = inputs_container()) noexcept(false);

        /// \brief Compile a rpn expression
        ///
        /// \param expr Reverse polish notation expression, e.g. "A B + 1.3 /"
        /// \param inputs Vector of input variables, e.g. {"A","B"}
        ///
        //void compile_postfix(const pstring &expr, const inputs_container &inputs = inputs_container()) noexcept(false);


        static bool is_number(string n)
        {
            if (n.empty())
                return false;
            var ls = n.substr(0,1);
            var l = ls[0];
            return l >= '0' && l <= '9';
        }


        static bool is_id(string n)
        {
            if (n.empty())
                return false;
            var ls = n.substr(0,1);
            var l = ls[0];
            return (l >= 'a' && l <= 'z') || (l >= 'A' && l <= 'Z');
        }


        static int get_prio(string v)
        {
            var p = pcmds().find(v);
            if (p != default)
                return p.prio;
            if (plib.pglobal.left(v, 1).CompareTo("a") >= 0 && plib.pglobal.left(v, 1).CompareTo("z") <= 0)
                return 0;

            return -1;
        }


        static string pop_check(std.stack<string> stk, string expr)
        {
            if (stk.empty())
                throw new pexception(new plib.pfmt("pfunction: stack underflow during infix parsing of: <{0}>").op(expr));
            string res = stk.top();
            stk.pop();
            return res;
        }


        /// \brief Compile an infix expression
        ///
        /// \param expr Infix expression, e.g. "(A+B)/1.3"
        /// \param inputs Vector of input variables, e.g. {"A","B"}
        ///
        //template <typename NT>
        public void compile_infix(string expr) { compile_infix(expr, new std.vector<string>()); }
        public void compile_infix(string expr, std.vector<string> inputs)  //void compile_infix(const pstring &expr, const inputs_container &inputs = inputs_container()) noexcept(false);
        {
            // Shunting-yard infix parsing
            std.vector<string> sep = new std.vector<string> {"(", ")", ",", "*", "/", "+", "-", "^", "<=", ">=", "==", "!=", "<", ">"};
            std.vector<string> sexpr2 = plib.pglobal.psplit(plib.pglobal.replace_all(expr, " ", ""), sep);
            std.stack<string> opstk = new std.stack<string>();
            std.vector<string> postfix = new std.vector<string>();
            std.vector<string> sexpr1 = new std.vector<string>();
            std.vector<string> sexpr = new std.vector<string>();

            // FIXME: We really need to switch to ptokenizer and fix negative number
            //        handling in ptokenizer.

            // copy/paste to the int version below

            // Fix numbers exponential numbers
            for (size_t i = 0; i < sexpr2.size(); )
            {
                if (i + 2 < sexpr2.size() && sexpr2[i].length() > 1)
                {
                    var r = plib.pglobal.right(sexpr2[i], 1);
                    var ne = sexpr2[i+1];
                    if (is_number(sexpr2[i])
                        && (r == "e" || r == "E")
                        && (ne == "-" || ne == "+"))
                    {
                        sexpr1.push_back(sexpr2[i] + ne + sexpr2[i+2]);
                        i+=3;
                    }
                    else
                    {
                        sexpr1.push_back(sexpr2[i++]);
                    }
                }
                else
                {
                    sexpr1.push_back(sexpr2[i++]);
                }
            }

            // Fix numbers with unary minus/plus
            for (size_t i = 0; i < sexpr1.size(); )
            {
                if (sexpr1[i]=="-" && (i+1 < sexpr1.size()) && is_number(sexpr1[i+1]))
                {
                    if (i==0 || !(is_number(sexpr1[i-1]) || sexpr1[i-1] == ")" || is_id(sexpr1[i-1])))
                    {
                        sexpr.push_back("-" + sexpr1[i+1]);
                        i+=2;
                    }
                    else
                    {
                        sexpr.push_back(sexpr1[i++]);
                    }
                }
                else if (sexpr1[i]=="-" && (i+1 < sexpr1.size()) && (is_id(sexpr1[i+1]) || sexpr1[i+1] == "("))
                {
                    if (i==0 || !(is_number(sexpr1[i-1]) || sexpr1[i-1] == ")" || is_id(sexpr1[i-1])))
                    {
                        sexpr.emplace_back("neg");
                        sexpr.push_back(sexpr1[i+1]);
                        i+=2;
                    }
                    else
                    {
                        sexpr.push_back(sexpr1[i++]);
                    }
                }
                else
                {
                    sexpr.push_back(sexpr1[i++]);
                }
            }

            for (size_t i = 0; i < sexpr.size(); i++)
            {
                string s = sexpr[i];
                if (s == "(")
                {
                    opstk.push(s);
                }
                else if (s == ")")
                {
                    string x = pop_check(opstk, expr);
                    while (x != "(")
                    {
                        postfix.push_back(x);
                        x = pop_check(opstk, expr);
                    }

                    if (!opstk.empty() && get_prio(opstk.top()) == 0)
                        postfix.push_back(pop_check(opstk, expr));
                }
                else if (s == ",")
                {
                    string x = pop_check(opstk, expr);
                    while (x != "(")
                    {
                        postfix.push_back(x);
                        x = pop_check(opstk, expr);
                    }

                    opstk.push(x);
                }
                else
                {
                    int prio = get_prio(s);
                    if (prio > 0)
                    {
                        if (opstk.empty())
                        {
                            opstk.push(s);
                        }
                        else
                        {
                            if (get_prio(opstk.top()) >= prio)
                                postfix.push_back(pop_check(opstk, expr));

                            opstk.push(s);
                        }
                    }
                    else if (prio == 0) // Function or variable
                    {
                        if ((i + 1 < sexpr.size()) && sexpr[i + 1] == "(")
                            opstk.push(s);
                        else
                            postfix.push_back(s);
                    }
                    else
                    {
                        postfix.push_back(s);
                    }
                }
            }

            while (!opstk.empty())
            {
                postfix.push_back(opstk.top());
                opstk.pop();
            }

            //for (auto &e : postfix)
            //  printf("\t%s\n", e.c_str());
            compile_postfix(inputs, postfix, expr);
        }


        //template <typename NT>
        //static inline typename std::enable_if<std::is_floating_point<NT>::value, NT>::type
        NT lfsr_random_nl_fptype(ref uint16_t lfsr)  //lfsr_random(std::uint16_t &lfsr) noexcept
        {
            uint16_t lsb = (uint16_t)(lfsr & 1);
            lfsr >>= 1;
            if (lsb != 0)
                lfsr ^= 0xB400; // taps 15, 13, 12, 10
            return ops.divide(ops.cast(lfsr), ops.cast(0xffff));  //return static_cast<NT>(lfsr) / static_cast<NT>(0xffffU);
        }


        //template <typename NT>
        //static inline typename std::enable_if<std::is_integral<NT>::value, NT>::type
        //lfsr_random(std::uint16_t &lfsr) noexcept


        static NT ST1(std.array<NT, size_t_constant_20> stack, UInt32 ptr) { return stack[ptr]; }  //#define ST1 stack[ptr]
        static NT ST2(std.array<NT, size_t_constant_20> stack, UInt32 ptr) { return stack[ptr - 1]; }  //#define ST2 stack[ptr-1]

        //#define OP(OP, ADJ, EXPR) \
        //case OP: \
        //    ptr-= (ADJ); \
        //    stack[ptr-1] = (EXPR); \
        //    break;
        static void OP(ref UInt32 ptr, std.array<NT, size_t_constant_20> stack, rpn_cmd OP, UInt32 ADJ, NT EXPR)
        {
            ptr -= ADJ;
            stack[ptr - 1] = EXPR;
        }


        /// \brief Evaluate the expression
        ///
        /// \param values for input variables, e.g. {1.1, 2.2}
        /// \return value of expression
        ///
        //template <typename NT>
        public NT evaluate(std.vector<NT> values = null)  //value_type evaluate(const values_container &values = values_container()) noexcept;
        {
            if (values == null)
                values = new std.vector<NT>();

            std.array<NT, size_t_constant_20> stack = new std.array<NT, size_t_constant_20>();  //std::array<NT, 20> stack = { plib::constants<NT>::zero() };
            UInt32 ptr = 0;
            stack[0] = plib.constants<NT, NT_OPS>.zero();
            foreach (var rc in m_precompiled)
            {
                switch (rc.m_cmd)
                {
                    case rpn_cmd.ADD: OP(ref ptr, stack, rpn_cmd.ADD, 1, ops.add(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(ADD,  1, ST2 + ST1):
                    case rpn_cmd.MULT: OP(ref ptr, stack, rpn_cmd.MULT, 1, ops.multiply(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(MULT, 1, ST2 * ST1):
                    case rpn_cmd.SUB: OP(ref ptr, stack, rpn_cmd.SUB,  1, ops.subtract(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(SUB,  1, ST2 - ST1):
                    case rpn_cmd.DIV: OP(ref ptr, stack, rpn_cmd.DIV,  1, ops.divide(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(DIV,  1, ST2 / ST1):
                    case rpn_cmd.POW: OP(ref ptr, stack, rpn_cmd.POW,  1, plib.pglobal.pow<NT, NT_OPS>(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(POW,  1, plib.pow(ST2, ST1)):
                    case rpn_cmd.SIN: OP(ref ptr, stack, rpn_cmd.SIN,  0, plib.pglobal.sin<NT, NT_OPS>(ST2(stack, ptr)));  break;  //case OP(SIN,  0, plib.sin(ST2)):
                    case rpn_cmd.COS: OP(ref ptr, stack, rpn_cmd.COS,  0, plib.pglobal.cos<NT, NT_OPS>(ST2(stack, ptr)));  break;  //case OP(COS,  0, plib.cos(ST2)):
                    case rpn_cmd.MAX: OP(ref ptr, stack, rpn_cmd.MAX,  1, ops.max(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(MAX,  1, std.max(ST2, ST1)):
                    case rpn_cmd.MIN: OP(ref ptr, stack, rpn_cmd.MIN,  1, ops.min(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(MIN,  1, std.min(ST2, ST1)):
                    case rpn_cmd.TRUNC: OP(ref ptr, stack, rpn_cmd.TRUNC,  0, plib.pglobal.trunc<NT, NT_OPS>(ST2(stack, ptr)));  break;  //case OP(TRUNC,  0, plib.trunc(ST2)):
                    case rpn_cmd.RAND:
                        stack[ptr++] = lfsr_random_nl_fptype(ref m_lfsr);
                        break;
                    case rpn_cmd.PUSH_INPUT:
                        stack[ptr++] = values[rc.m_param.index];  //stack[ptr++] = values[rc.m_param.index];
                        break;
                    case rpn_cmd.PUSH_CONST:
                        stack[ptr++] = rc.m_param.val;
                        break;
                }
            }

            return stack[ptr - 1];
        }


        //template <typename ST>
        //void save_state(ST &st)
        //{
        //    st.save_item(m_lfsr, "m_lfsr");
        //}


        //template <typename NT>
        void compile_postfix(std.vector<string> inputs, std.vector<string> cmds, string expr)  //void pfunction<NT>::compile_postfix(const inputs_container &inputs, const std::vector<pstring> &cmds, const pstring &expr) noexcept(false)
        {
            m_precompiled.clear();
            int stk = 0;

            foreach (string cmd in cmds)
            {
                rpn_inst rc = new rpn_inst();
                var p = pcmds().find(cmd);
                if (p != default)
                {
                    rc.m_cmd = p.cmd;
                    stk -= p.adj;
                }
                else
                {
                    for (size_t i = 0; i < inputs.size(); i++)
                    {
                        if (inputs[i] == cmd)
                        {
                            rc.m_cmd = rpn_cmd.PUSH_INPUT;
                            rc.m_param.val = ops.cast(i);
                            rc.m_param.index = i;
                            stk += 1;
                            break;
                        }
                    }

                    if (rc.m_cmd != rpn_cmd.PUSH_INPUT)
                    {
                        //using fl_t = decltype(rc.m_param.val);

                        rc.m_cmd = rpn_cmd.PUSH_CONST;
                        bool err = false;
                        var rs = plib.pglobal.right(cmd, 1);
                        var r = units_si().find(rs);  //auto r=units_si<fl_t>().find(rs);
                        if (ops.equals(r, ops.default_))  //if (r == units_si<fl_t>().end())
                        {
                            rc.m_param.val = ops.pstonum_ne(false, cmd, out err);  //rc.m_param.val = plib::pstonum_ne<fl_t>(cmd, err);
                            rc.m_param.index = (size_t)ops.cast_int(rc.m_param.val);
                        }
                        else
                        {
                            rc.m_param.val = ops.multiply(ops.pstonum_ne(false, plib.pglobal.left(cmd, cmd.length() - 1), out err), r);  //rc.m_param.val = plib::pstonum_ne<fl_t>(plib::left(cmd, cmd.size()-1), err) * r->second;
                            rc.m_param.index = (size_t)ops.cast_int(rc.m_param.val);
                        }

                        if (err)
                            throw new pexception(new plib.pfmt("pfunction: unknown/misformatted token <{0}> in <{1}>").op(cmd, expr));

                        stk += 1;
                    }
                }

                if (stk < 1)
                    throw new pexception(new plib.pfmt("pfunction: stack underflow on token <{0}> in <{1}>").op(cmd, expr));

                if (stk >= MAX_STACK)
                    throw new pexception(new plib.pfmt("pfunction: stack overflow on token <{0}> in <{1}>").op(cmd, expr));

                m_precompiled.push_back(rc);
            }

            if (stk != 1)
                throw new pexception(new plib.pfmt("pfunction: stack count {0} different to one on <{1}>").op(stk, expr));
        }
    }


    //extern template class pfunction<float>;
    //extern template class pfunction<double>;
    //extern template class pfunction<long double>;
#if (PUSE_FLOAT128)
    //extern template class pfunction<__float128>;
#endif
}
