// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using nl_fptype = System.Double;
using uint16_t = System.UInt16;


namespace mame.plib
{
    //============================================================
    //  function evaluation
    //============================================================

    /// \brief Class providing support for evaluating expressions
    ///
    ///  \tparam NT Number type, should be float or double
    ///
    //template <typename NT>
    class pfunction_nl_fptype
    {
        enum rpn_cmd
        {
            ADD,
            MULT,
            SUB,
            DIV,
            POW,
            SIN,
            COS,
            MIN,
            MAX,
            RAND, /// random number between 0 and 1
            TRUNC,
            PUSH_CONST,
            PUSH_INPUT
        }


        class rpn_inst
        {
            public rpn_cmd m_cmd;
            public nl_fptype m_param;

            public rpn_inst() { m_cmd = rpn_cmd.ADD;  m_param = plib.constants_nl_fptype.zero(); }
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
        public pfunction_nl_fptype()
        {
            m_lfsr = 0xace1;
        }


        /// \brief Compile an expression
        ///
        /// \param expr infix or postfix expression. default is infix, postrix
        ///          to be prefixed with rpn, e.g. "rpn:A B + 1.3 /"
        /// \param inputs Vector of input variables, e.g. {"A","B"}
        ///
        //void compile(const pstring &expr, const std::vector<pstring> &inputs) noexcept(false);

        /// \brief Compile a rpn expression
        ///
        /// \param expr Reverse polish notation expression, e.g. "A B + 1.3 /"
        /// \param inputs Vector of input variables, e.g. {"A","B"}
        ///
        //void compile_postfix(const pstring &expr, const std::vector<pstring> &inputs) noexcept(false);


        static int get_prio(string v)
        {
            if (v == "(" || v == ")")
                return 1;
            if (plib.pglobal.left(v, 1).CompareTo("a") >= 0 && plib.pglobal.left(v, 1).CompareTo("z") <= 0)
                return 0;
            if (v == "*" || v == "/")
                return 20;
            if (v == "+" || v == "-")
                return 10;
            if (v == "^")
                return 30;

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
        public void compile_infix(string expr, std.vector<string> inputs)
        {
            // Shunting-yard infix parsing
            std.vector<string> sep = new std.vector<string>() { "(", ")", ",", "*", "/", "+", "-", "^" };
            std.vector<string> sexpr1 = new std.vector<string>(plib.pglobal.psplit(plib.pglobal.replace_all(expr, " ", ""), sep));
            std.stack<string> opstk = new std.stack<string>();
            std.vector<string> postfix = new std.vector<string>();
            std.vector<string> sexpr = new std.vector<string>();

            // FIXME: We really need to switch to ptokenizer and fix negative number
            //        handling in ptokenizer.

            // Fix numbers
            for (UInt32 i = 0; i < sexpr1.size(); )
            {
                if ((i == 0) && (sexpr1.size() > 1) && (sexpr1[0] == "-")
                    && (plib.pglobal.left(sexpr1[1], 1).CompareTo("0") >= 0) && (plib.pglobal.left(sexpr1[1], 1).CompareTo("9") <= 0))  //&& (plib::left(sexpr1[1],1) >= "0") && (plib::left(sexpr1[1],1) <= "9"))
                {
                    if (sexpr1.size() < 4)
                    {
                        sexpr.push_back(sexpr1[0] + sexpr1[1]);
                        i += 2;
                    }
                    else
                    {
                        var r = plib.pglobal.right(sexpr1[1], 1);
                        var ne = sexpr1[2];
                        if ((r == "e" || r == "E") && (ne == "-" || ne == "+"))
                        {
                            sexpr.push_back(sexpr1[0] + sexpr1[1] + ne + sexpr1[3]);
                            i += 4;
                        }
                        else
                        {
                            sexpr.push_back(sexpr1[0] + sexpr1[1]);
                            i += 2;
                        }
                    }
                }
                else if (i + 2 < sexpr1.size() && sexpr1[i].length() > 1)
                {
                    var l = plib.pglobal.left(sexpr1[i], 1);
                    var r = plib.pglobal.right(sexpr1[i], 1);
                    var ne = sexpr1[i + 1];
                    if ((l.CompareTo("0") >= 0) && (l.CompareTo("9") <= 0) && (r == "e" || r == "E") && (ne == "-" || ne == "+"))  //if ((l >= "0") && (l <= "9") && (r == "e" || r == "E") && (ne == "-" || ne == "+"))
                    {
                        sexpr.push_back(sexpr1[i] + ne + sexpr1[i + 2]);
                        i += 3;
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

            for (UInt32 i = 0; i < sexpr.size(); i++)
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
                    int p = get_prio(s);
                    if (p > 0)
                    {
                        if (opstk.empty())
                        {
                            opstk.push(s);
                        }
                        else
                        {
                            if (get_prio(opstk.top()) >= get_prio(s))
                                postfix.push_back(pop_check(opstk, expr));
                            opstk.push(s);
                        }
                    }
                    else if (p == 0) // Function or variable
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

            //printf("e : %s\n", expr.c_str());
            //for (auto &s : postfix)
            //  printf("x : %s\n", s.c_str());
            compile_postfix(inputs, postfix, expr);
        }


        //template <typename NT>
        //static inline typename std::enable_if<std::is_floating_point<NT>::value, NT>::type
        nl_fptype lfsr_random_nl_fptype(ref uint16_t lfsr)  //lfsr_random(std::uint16_t &lfsr) noexcept
        {
            uint16_t lsb = (uint16_t)(lfsr & 1);
            lfsr >>= 1;
            if (lsb != 0)
                lfsr ^= 0xB400; // taps 15, 13, 12, 10
            return (nl_fptype)lfsr / (nl_fptype)0xffff;  //return static_cast<NT>(lfsr) / static_cast<NT>(0xffffU);
        }


        //template <typename NT>
        //static inline typename std::enable_if<std::is_integral<NT>::value, NT>::type
        //lfsr_random(std::uint16_t &lfsr) noexcept


        static nl_fptype ST1(std.array<nl_fptype> stack, UInt32 ptr) { return stack[ptr]; }  //#define ST1 stack[ptr]
        static nl_fptype ST2(std.array<nl_fptype> stack, UInt32 ptr) { return stack[ptr - 1]; }  //#define ST2 stack[ptr-1]

        //#define OP(OP, ADJ, EXPR) \
        //case OP: \
        //    ptr-= (ADJ); \
        //    stack[ptr-1] = (EXPR); \
        //    break;
        static void OP(ref UInt32 ptr, std.array<nl_fptype> stack, rpn_cmd OP, UInt32 ADJ, nl_fptype EXPR)
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
        public nl_fptype evaluate(std.vector<nl_fptype> values = null)  //NT evaluate(const std::vector<NT> &values = std::vector<NT>()) noexcept;
        {
            if (values == null)
                values = new std.vector<nl_fptype>();

            std.array<nl_fptype> stack = new std.array<nl_fptype>(20);  //std::array<NT, 20> stack = { plib::constants<NT>::zero() };
            UInt32 ptr = 0;
            stack[0] = plib.constants_nl_fptype.zero();
            foreach (var rc in m_precompiled)
            {
                switch (rc.m_cmd)
                {
                    case rpn_cmd.ADD: OP(ref ptr, stack, rpn_cmd.ADD, 1, ST2(stack, ptr) + ST1(stack, ptr));  break;  //case OP(ADD,  1, ST2 + ST1):
                    case rpn_cmd.MULT: OP(ref ptr, stack, rpn_cmd.MULT, 1, ST2(stack, ptr) * ST1(stack, ptr));  break;  //case OP(MULT, 1, ST2 * ST1):
                    case rpn_cmd.SUB: OP(ref ptr, stack, rpn_cmd.SUB,  1, ST2(stack, ptr) - ST1(stack, ptr));  break;  //case OP(SUB,  1, ST2 - ST1):
                    case rpn_cmd.DIV: OP(ref ptr, stack, rpn_cmd.DIV,  1, ST2(stack, ptr) / ST1(stack, ptr));  break;  //case OP(DIV,  1, ST2 / ST1):
                    case rpn_cmd.POW: OP(ref ptr, stack, rpn_cmd.POW,  1, plib.pglobal.pow(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(POW,  1, plib.pow(ST2, ST1)):
                    case rpn_cmd.SIN: OP(ref ptr, stack, rpn_cmd.SIN,  0, plib.pglobal.sin(ST2(stack, ptr)));  break;  //case OP(SIN,  0, plib.sin(ST2)):
                    case rpn_cmd.COS: OP(ref ptr, stack, rpn_cmd.COS,  0, plib.pglobal.cos(ST2(stack, ptr)));  break;  //case OP(COS,  0, plib.cos(ST2)):
                    case rpn_cmd.MAX: OP(ref ptr, stack, rpn_cmd.MAX,  1, std.max(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(MAX,  1, std.max(ST2, ST1)):
                    case rpn_cmd.MIN: OP(ref ptr, stack, rpn_cmd.MIN,  1, std.min(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(MIN,  1, std.min(ST2, ST1)):
                    case rpn_cmd.TRUNC: OP(ref ptr, stack, rpn_cmd.TRUNC,  0, plib.pglobal.trunc(ST2(stack, ptr)));  break;  //case OP(TRUNC,  0, plib.trunc(ST2)):
                    case rpn_cmd.RAND:
                        stack[ptr++] = lfsr_random_nl_fptype(ref m_lfsr);
                        break;
                    case rpn_cmd.PUSH_INPUT:
                        stack[ptr++] = values[(UInt32)rc.m_param];
                        break;
                    case rpn_cmd.PUSH_CONST:
                        stack[ptr++] = rc.m_param;
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


        //void compile_postfix(const std::vector<pstring> &inputs,
        //        const std::vector<pstring> &cmds, const pstring &expr);
        //template <typename NT>
        void compile_postfix(std.vector<string> inputs,
                std.vector<string> cmds, string expr)
        {
            m_precompiled.clear();
            int stk = 0;

            foreach (string cmd in cmds)
            {
                rpn_inst rc = new rpn_inst();
                if (cmd == "+")
                    { rc.m_cmd = rpn_cmd.ADD; stk -= 1; }
                else if (cmd == "-")
                    { rc.m_cmd = rpn_cmd.SUB; stk -= 1; }
                else if (cmd == "*")
                    { rc.m_cmd = rpn_cmd.MULT; stk -= 1; }
                else if (cmd == "/")
                    { rc.m_cmd = rpn_cmd.DIV; stk -= 1; }
                else if (cmd == "pow")
                    { rc.m_cmd = rpn_cmd.POW; stk -= 1; }
                else if (cmd == "sin")
                    { rc.m_cmd = rpn_cmd.SIN; stk -= 0; }
                else if (cmd == "cos")
                    { rc.m_cmd = rpn_cmd.COS; stk -= 0; }
                else if (cmd == "max")
                    { rc.m_cmd = rpn_cmd.MAX; stk -= 1; }
                else if (cmd == "min")
                    { rc.m_cmd = rpn_cmd.MIN; stk -= 1; }
                else if (cmd == "trunc")
                    { rc.m_cmd = rpn_cmd.TRUNC; stk -= 0; }
                else if (cmd == "rand")
                    { rc.m_cmd = rpn_cmd.RAND; stk += 1; }
                else
                {
                    for (UInt32 i = 0; i < inputs.size(); i++)
                    {
                        if (inputs[i] == cmd)
                        {
                            rc.m_cmd = rpn_cmd.PUSH_INPUT;
                            rc.m_param = (nl_fptype)i;
                            stk += 1;
                            break;
                        }
                    }

                    if (rc.m_cmd != rpn_cmd.PUSH_INPUT)
                    {
                        rc.m_cmd = rpn_cmd.PUSH_CONST;
                        bool err = false;
                        rc.m_param = plib.pglobal.pstonum_ne_nl_fptype(false, cmd, out err);
                        if (err)
                            throw new pexception(new plib.pfmt("pfunction: unknown/misformatted token <{0}> in <{1}>").op(cmd, expr));
                        stk += 1;
                    }
                }

                if (stk < 1)
                    throw new pexception(new plib.pfmt("pfunction: stack underflow on token <{0}> in <{1}>").op(cmd, expr));

                m_precompiled.push_back(rc);
            }

            if (stk != 1)
                throw new pexception(new plib.pfmt("pfunction: stack count different to one on <{0}>").op(expr));
        }
    }

    class pfunction_int
    {
        enum rpn_cmd
        {
            ADD,
            MULT,
            SUB,
            DIV,
            POW,
            SIN,
            COS,
            MIN,
            MAX,
            RAND, /// random number between 0 and 1
            TRUNC,
            PUSH_CONST,
            PUSH_INPUT
        }


        class rpn_inst
        {
            public rpn_cmd m_cmd;
            public int m_param;

            public rpn_inst() { m_cmd = rpn_cmd.ADD;  m_param = (int)plib.constants_nl_fptype.zero(); }
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
        public pfunction_int()
        {
            m_lfsr = 0xace1;
        }


        /// \brief Compile an expression
        ///
        /// \param expr infix or postfix expression. default is infix, postrix
        ///          to be prefixed with rpn, e.g. "rpn:A B + 1.3 /"
        /// \param inputs Vector of input variables, e.g. {"A","B"}
        ///
        //void compile(const pstring &expr, const std::vector<pstring> &inputs) noexcept(false);

        /// \brief Compile a rpn expression
        ///
        /// \param expr Reverse polish notation expression, e.g. "A B + 1.3 /"
        /// \param inputs Vector of input variables, e.g. {"A","B"}
        ///
        //void compile_postfix(const pstring &expr, const std::vector<pstring> &inputs) noexcept(false);


        static int get_prio(string v)
        {
            if (v == "(" || v == ")")
                return 1;
            if (plib.pglobal.left(v, 1).CompareTo("a") >= 0 && plib.pglobal.left(v, 1).CompareTo("z") <= 0)
                return 0;
            if (v == "*" || v == "/")
                return 20;
            if (v == "+" || v == "-")
                return 10;
            if (v == "^")
                return 30;

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
        public void compile_infix(string expr, std.vector<string> inputs)
        {
            // Shunting-yard infix parsing
            std.vector<string> sep = new std.vector<string>() { "(", ")", ",", "*", "/", "+", "-", "^" };
            std.vector<string> sexpr1 = new std.vector<string>(plib.pglobal.psplit(plib.pglobal.replace_all(expr, " ", ""), sep));
            std.stack<string> opstk = new std.stack<string>();
            std.vector<string> postfix = new std.vector<string>();
            std.vector<string> sexpr = new std.vector<string>();

            // FIXME: We really need to switch to ptokenizer and fix negative number
            //        handling in ptokenizer.

            // Fix numbers
            for (UInt32 i = 0; i < sexpr1.size(); )
            {
                if ((i == 0) && (sexpr1.size() > 1) && (sexpr1[0] == "-")
                    && (plib.pglobal.left(sexpr1[1], 1).CompareTo("0") >= 0) && (plib.pglobal.left(sexpr1[1], 1).CompareTo("9") <= 0))  //&& (plib::left(sexpr1[1],1) >= "0") && (plib::left(sexpr1[1],1) <= "9"))
                {
                    if (sexpr1.size() < 4)
                    {
                        sexpr.push_back(sexpr1[0] + sexpr1[1]);
                        i += 2;
                    }
                    else
                    {
                        var r = plib.pglobal.right(sexpr1[1], 1);
                        var ne = sexpr1[2];
                        if ((r == "e" || r == "E") && (ne == "-" || ne == "+"))
                        {
                            sexpr.push_back(sexpr1[0] + sexpr1[1] + ne + sexpr1[3]);
                            i += 4;
                        }
                        else
                        {
                            sexpr.push_back(sexpr1[0] + sexpr1[1]);
                            i += 2;
                        }
                    }
                }
                else if (i + 2 < sexpr1.size() && sexpr1[i].length() > 1)
                {
                    var l = plib.pglobal.left(sexpr1[i], 1);
                    var r = plib.pglobal.right(sexpr1[i], 1);
                    var ne = sexpr1[i + 1];
                    if ((l.CompareTo("0") >= 0) && (l.CompareTo("9") <= 0) && (r == "e" || r == "E") && (ne == "-" || ne == "+"))  //if ((l >= "0") && (l <= "9") && (r == "e" || r == "E") && (ne == "-" || ne == "+"))
                    {
                        sexpr.push_back(sexpr1[i] + ne + sexpr1[i + 2]);
                        i += 3;
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

            for (UInt32 i = 0; i < sexpr.size(); i++)
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
                    int p = get_prio(s);
                    if (p > 0)
                    {
                        if (opstk.empty())
                        {
                            opstk.push(s);
                        }
                        else
                        {
                            if (get_prio(opstk.top()) >= get_prio(s))
                                postfix.push_back(pop_check(opstk, expr));
                            opstk.push(s);
                        }
                    }
                    else if (p == 0) // Function or variable
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

            //printf("e : %s\n", expr.c_str());
            //for (auto &s : postfix)
            //  printf("x : %s\n", s.c_str());
            compile_postfix(inputs, postfix, expr);
        }


        //template <typename NT>
        //static inline typename std::enable_if<std::is_floating_point<NT>::value, NT>::type
        //lfsr_random(std::uint16_t &lfsr) noexcept


        //template <typename NT>
        //static inline typename std::enable_if<std::is_integral<NT>::value, NT>::type
        int lfsr_random_int(ref uint16_t lfsr)  //lfsr_random(std::uint16_t &lfsr) noexcept
        {
            uint16_t lsb = (uint16_t)(lfsr & 1);
            lfsr >>= 1;
            if (lsb != 0)
                lfsr ^= 0xB400; // taps 15, 13, 12, 10
            return (int)lfsr;
        }



        static int ST1(std.array<int> stack, UInt32 ptr) { return stack[ptr]; }  //#define ST1 stack[ptr]
        static int ST2(std.array<int> stack, UInt32 ptr) { return stack[ptr - 1]; }  //#define ST2 stack[ptr-1]

        //#define OP(OP, ADJ, EXPR) \
        //case OP: \
        //    ptr-= (ADJ); \
        //    stack[ptr-1] = (EXPR); \
        //    break;
        static void OP(ref UInt32 ptr, std.array<int> stack, rpn_cmd OP, UInt32 ADJ, int EXPR)
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
        public int evaluate(std.vector<int> values = null)  //NT evaluate(const std::vector<NT> &values = std::vector<NT>()) noexcept;
        {
            if (values == null)
                values = new std.vector<int>();

            std.array<int> stack = new std.array<int>(20);  //std::array<NT, 20> stack = { plib::constants<NT>::zero() };
            UInt32 ptr = 0;
            stack[0] = (int)plib.constants_nl_fptype.zero();
            foreach (var rc in m_precompiled)
            {
                switch (rc.m_cmd)
                {
                    case rpn_cmd.ADD: OP(ref ptr, stack, rpn_cmd.ADD, 1, ST2(stack, ptr) + ST1(stack, ptr));  break;  //case OP(ADD,  1, ST2 + ST1):
                    case rpn_cmd.MULT: OP(ref ptr, stack, rpn_cmd.MULT, 1, ST2(stack, ptr) * ST1(stack, ptr));  break;  //case OP(MULT, 1, ST2 * ST1):
                    case rpn_cmd.SUB: OP(ref ptr, stack, rpn_cmd.SUB,  1, ST2(stack, ptr) - ST1(stack, ptr));  break;  //case OP(SUB,  1, ST2 - ST1):
                    case rpn_cmd.DIV: OP(ref ptr, stack, rpn_cmd.DIV,  1, ST2(stack, ptr) / ST1(stack, ptr));  break;  //case OP(DIV,  1, ST2 / ST1):
                    case rpn_cmd.POW: OP(ref ptr, stack, rpn_cmd.POW,  1, (int)plib.pglobal.pow(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(POW,  1, plib.pow(ST2, ST1)):
                    case rpn_cmd.SIN: OP(ref ptr, stack, rpn_cmd.SIN,  0, (int)plib.pglobal.sin(ST2(stack, ptr)));  break;  //case OP(SIN,  0, plib.sin(ST2)):
                    case rpn_cmd.COS: OP(ref ptr, stack, rpn_cmd.COS,  0, (int)plib.pglobal.cos(ST2(stack, ptr)));  break;  //case OP(COS,  0, plib.cos(ST2)):
                    case rpn_cmd.MAX: OP(ref ptr, stack, rpn_cmd.MAX,  1, std.max(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(MAX,  1, std.max(ST2, ST1)):
                    case rpn_cmd.MIN: OP(ref ptr, stack, rpn_cmd.MIN,  1, std.min(ST2(stack, ptr), ST1(stack, ptr)));  break;  //case OP(MIN,  1, std.min(ST2, ST1)):
                    case rpn_cmd.TRUNC: OP(ref ptr, stack, rpn_cmd.TRUNC,  0, (int)plib.pglobal.trunc(ST2(stack, ptr)));  break;  //case OP(TRUNC,  0, plib.trunc(ST2)):
                    case rpn_cmd.RAND:
                        stack[ptr++] = lfsr_random_int(ref m_lfsr);
                        break;
                    case rpn_cmd.PUSH_INPUT:
                        stack[ptr++] = values[(UInt32)rc.m_param];
                        break;
                    case rpn_cmd.PUSH_CONST:
                        stack[ptr++] = rc.m_param;
                        break;
                }
            }

            return stack[ptr - 1];
        }


        //void compile_postfix(const std::vector<pstring> &inputs,
        //        const std::vector<pstring> &cmds, const pstring &expr);
        //template <typename NT>
        void compile_postfix(std.vector<string> inputs,
                std.vector<string> cmds, string expr)
        {
            m_precompiled.clear();
            int stk = 0;

            foreach (string cmd in cmds)
            {
                rpn_inst rc = new rpn_inst();
                if (cmd == "+")
                    { rc.m_cmd = rpn_cmd.ADD; stk -= 1; }
                else if (cmd == "-")
                    { rc.m_cmd = rpn_cmd.SUB; stk -= 1; }
                else if (cmd == "*")
                    { rc.m_cmd = rpn_cmd.MULT; stk -= 1; }
                else if (cmd == "/")
                    { rc.m_cmd = rpn_cmd.DIV; stk -= 1; }
                else if (cmd == "pow")
                    { rc.m_cmd = rpn_cmd.POW; stk -= 1; }
                else if (cmd == "sin")
                    { rc.m_cmd = rpn_cmd.SIN; stk -= 0; }
                else if (cmd == "cos")
                    { rc.m_cmd = rpn_cmd.COS; stk -= 0; }
                else if (cmd == "max")
                    { rc.m_cmd = rpn_cmd.MAX; stk -= 1; }
                else if (cmd == "min")
                    { rc.m_cmd = rpn_cmd.MIN; stk -= 1; }
                else if (cmd == "trunc")
                    { rc.m_cmd = rpn_cmd.TRUNC; stk -= 0; }
                else if (cmd == "rand")
                    { rc.m_cmd = rpn_cmd.RAND; stk += 1; }
                else
                {
                    for (UInt32 i = 0; i < inputs.size(); i++)
                    {
                        if (inputs[i] == cmd)
                        {
                            rc.m_cmd = rpn_cmd.PUSH_INPUT;
                            rc.m_param = (int)i;
                            stk += 1;
                            break;
                        }
                    }

                    if (rc.m_cmd != rpn_cmd.PUSH_INPUT)
                    {
                        rc.m_cmd = rpn_cmd.PUSH_CONST;
                        bool err = false;
                        rc.m_param = plib.pglobal.pstonum_ne_int(false, cmd, out err);
                        if (err)
                            throw new pexception(new plib.pfmt("pfunction: unknown/misformatted token <{0}> in <{1}>").op(cmd, expr));
                        stk += 1;
                    }
                }

                if (stk < 1)
                    throw new pexception(new plib.pfmt("pfunction: stack underflow on token <{0}> in <{1}>").op(cmd, expr));

                m_precompiled.push_back(rc);
            }

            if (stk != 1)
                throw new pexception(new plib.pfmt("pfunction: stack count different to one on <{0}>").op(expr));
        }
    }


    //extern template class pfunction<float>;
    //extern template class pfunction<double>;
    //extern template class pfunction<long double>;
#if (PUSE_FLOAT128)
    //extern template class pfunction<__float128>;
#endif
}
