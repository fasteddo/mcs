// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;
using param_int_t = mame.netlist.param_num_t<int, mame.netlist.param_num_t_operators_int>;  //using param_int_t = param_num_t<int>;
using param_logic_t = mame.netlist.param_num_t<bool, mame.netlist.param_num_t_operators_bool>;  //using param_logic_t = param_num_t<bool>;
using param_model_t_value_str_t = mame.netlist.param_model_t.value_base_t<string, mame.netlist.param_model_t.value_base_t_operators_string>;  //using value_str_t = value_base_t<pstring>;
using param_model_t_value_t = mame.netlist.param_model_t.value_base_t<System.Double, mame.netlist.param_model_t.value_base_t_operators_double>;  //using value_t = value_base_t<nl_fptype>;

using static mame.netlist.nl_errstr_global;


namespace mame.netlist
{
    /// @brief Base class for all device parameters
    ///
    /// All device parameters classes derive from this object.
    public abstract class param_t : detail.device_object_t
    {
        enum param_type_t
        {
            STRING,
            DOUBLE,
            INTEGER,
            LOGIC,
            POINTER // Special-case which is always initialized at MAME startup time
        }


        //device-less, it's the responsibility of the owner to register!
        protected param_t(string name)
            : base(null, name)
        {
        }


        protected param_t(core_device_t device, string name)
            : base(device, device.name() + "." + name)
        {
            device.state().setup().register_param_t(this);
        }


        //PCOPYASSIGNMOVE(param_t, delete)
        //virtual ~param_t() noexcept;


        //param_type_t param_type() const noexcept(false);


        public abstract string valstr();


        protected string get_initial(core_device_t dev, out bool found)
        {
            string res = dev.state().setup().get_initial_param_val(this.name(), "");
            found = !res.empty();
            return res;
        }


        //template<typename C>
        protected void set_and_update_param(object p, object v)
        {
            if (p != v)
            {
                p = v;
                device().update_param();
            }
        }
    }


    // -----------------------------------------------------------------------------
    // numeric parameter template
    // -----------------------------------------------------------------------------

    public interface param_num_t_operators<T>
    {
        bool is_integral();
        T cast(double a);
    }


    public class param_num_t_operators_bool : param_num_t_operators<bool>
    {
        public bool is_integral() { return true; }
        public bool cast(double a) { return a != 0; }
    }

    public class param_num_t_operators_int : param_num_t_operators<int>
    {
        public bool is_integral() { return true; }
        public int cast(double a) { return (int)a; }
    }

    public class param_num_t_operators_uint32 : param_num_t_operators<UInt32>
    {
        public bool is_integral() { return true; }
        public UInt32 cast(double a) { return (UInt32)a; }
    }

    public class param_num_t_operators_uint64 : param_num_t_operators<UInt64>
    {
        public bool is_integral() { return true; }
        public UInt64 cast(double a) { return (UInt64)a; }
    }

    public class param_num_t_operators_double : param_num_t_operators<double>
    {
        public bool is_integral() { return false; }
        public double cast(double a) { return a; }
    }


    //template <typename T>
    public class param_num_t<T, T_OPS> : param_t
        where T_OPS : param_num_t_operators<T>, new()
    {
        static readonly param_num_t_operators<T> ops = new T_OPS();


        //using value_type = T;


        T m_param;


        //template <typename T>
        public param_num_t(core_device_t device, string name, T val)
            : base(device, name)
        {
            m_param = val;


            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                plib.pfunction<nl_fptype, nl_fptype_ops> func = new plib.pfunction<nl_fptype, nl_fptype_ops>();
                func.compile_infix(p, new std.vector<string>());
                var valx = func.evaluate();
                if (ops.is_integral())  //if (plib::is_integral<T>::value)
                    if (plib.pg.abs(valx - plib.pg.trunc(valx)) > nlconst.magic(1e-6))
                        throw new nl_exception(MF_INVALID_NUMBER_CONVERSION_1_2(device.name() + "." + name, p));
                m_param = ops.cast(valx);  //m_param = plib::narrow_cast<T>(valx);
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }


        public T op() { return m_param; }  //const T operator()() const NL_NOEXCEPT { return m_param; }


        public void set(T param) { set_and_update_param(m_param, param); }


        public override string valstr()
        {
            return new plib.pfmt("{0}").op(m_param);
        }
    }


    public interface param_enum_t_operators<T>
    {
        bool set_from_string(string s, out T e);
        int cast_int(T a);
    }


    public class param_enum_t_operators_matrix_type_e : param_enum_t_operators<solver.matrix_type_e>
    {
        public bool set_from_string(string s, out solver.matrix_type_e e) { return plib.penum_base.set_from_string(s, out e); }
        public int cast_int(solver.matrix_type_e a) { return (int)a; }
    }

    public class param_enum_t_operators_matrix_fp_type_e : param_enum_t_operators<solver.matrix_fp_type_e>
    {
        public bool set_from_string(string s, out solver.matrix_fp_type_e e) { return plib.penum_base.set_from_string(s, out e); }
        public int cast_int(solver.matrix_fp_type_e a) { return (int)a; }
    }

    public class param_enum_t_operators_matrix_sort_type_e : param_enum_t_operators<solver.matrix_sort_type_e>
    {
        public bool set_from_string(string s, out solver.matrix_sort_type_e e) { return plib.penum_base.set_from_string(s, out e); }
        public int cast_int(solver.matrix_sort_type_e a) { return (int)a; }
    }


    //template <typename T>
    public class param_enum_t<T, T_OPS> : param_t
        where T_OPS : param_enum_t_operators<T>, new()
    {
        static readonly param_enum_t_operators<T> ops = new T_OPS();


        //using value_type = T;


        T m_param;


        public param_enum_t(core_device_t device, string name, T val)
            : base(device, name)
        {
            m_param = val;


            bool found = false;
            string p = this.get_initial(device, out found);
            if (found)
            {
                T temp = val;
                bool ok = ops.set_from_string(p, out temp);  //bool ok = temp.set_from_string(p);
                if (!ok)
                {
                    device.state().log().fatal.op(MF_INVALID_ENUM_CONVERSION_1_2(name, p));
                    throw new nl_exception(MF_INVALID_ENUM_CONVERSION_1_2(name, p));
                }

                m_param = temp;
            }

            device.state().save(this, m_param, this.name(), "m_param");
        }


        public T op() { return m_param; }  //T operator()() const noexcept { return m_param; }


        //void set(const T &param) noexcept { set_and_update_param(m_param, param); }


        public override string valstr()
        {
            // returns the numerical value
            return new plib.pfmt("{0}").op(ops.cast_int(m_param));  //return plib::pfmt("{}")(static_cast<int>(m_param));
        }
    }


    // FIXME: these should go as well
    //using param_logic_t = param_num_t<bool>;
    //using param_int_t = param_num_t<int>;
    //using param_fp_t = param_num_t<nl_fptype>;

    // -----------------------------------------------------------------------------
    // pointer parameter
    // -----------------------------------------------------------------------------
    // FIXME: not a core component -> legacy
    //class param_ptr_t final: public param_t


    // -----------------------------------------------------------------------------
    // string parameter
    // -----------------------------------------------------------------------------
    public class param_str_t : param_t
    {
        string m_param;  //host_arena::unique_ptr<pstring> m_param;


        public param_str_t(core_device_t device, string name, string val)
            : base(device, name)
        {
            m_param = val;  //m_param = plib::make_unique<pstring, host_arena>(val);
            m_param = device.state().setup().get_initial_param_val(this.name(), val);  //*m_param = device.state().setup().get_initial_param_val(this->name(),val);
        }


        public param_str_t(netlist_state_t state, string name, string val)
            : base(name)
        {
            // deviceless parameter, no registration, owner is responsible
            m_param = val;  //m_param = plib::make_unique<pstring, host_arena>(val);
            m_param = state.setup().get_initial_param_val(this.name(), val);  //*m_param = state.setup().get_initial_param_val(this->name(),val);
        }


        //pstring operator()() const noexcept { return str(); }
        public string op() { return str(); }


        //void set(const pstring &param)


        public override string valstr() { return m_param; }


        protected virtual void changed() { }


        protected string str() { return m_param; }
    }


    public interface param_value_interface
    {
        string value_str(string entity);
        nl_fptype value(string entity);
    }


    // -----------------------------------------------------------------------------
    // model parameter
    // -----------------------------------------------------------------------------
    public class param_model_t : param_str_t, param_value_interface
    {
        public interface value_base_t_operators<T>
        {
            bool is_arithmetic();
            T cast(double a);
            T cast(string a);
        }


        public class value_base_t_operators_double : value_base_t_operators<double>
        {
            public bool is_arithmetic() { return true; }
            public double cast(double a) { return a; }
            public double cast(string a) { return Convert.ToDouble(a); }
        }


        public class value_base_t_operators_string : value_base_t_operators<string>
        {
            public bool is_arithmetic() { return false; }
            public string cast(double a) { return a.ToString(); }
            public string cast(string a) { return a; }
        }


        //template <typename T>
        public class value_base_t<T, T_OPS>
            where T_OPS : value_base_t_operators<T>, new()
        {
            static readonly value_base_t_operators<T> ops = new T_OPS();


            T m_value;


            //template <typename P, typename Y=T, typename DUMMY = std::enable_if_t<plib::is_arithmetic<Y>::value>>
            public value_base_t(param_value_interface param, string name)
            {
                if (ops.is_arithmetic())
                    m_value = ops.cast(param.value(name));  //: m_value(gsl::narrow<T>(param.value(name)))
                else
                    m_value = ops.cast(param.value_str(name));  //: m_value(static_cast<T>(param.value_str(name)))
            }

            //template <typename P, typename Y=T, std::enable_if_t<!plib::is_arithmetic<Y>::value, int> = 0>
            //value_base_t(P &param, const pstring &name)
            //: m_value(static_cast<T>(param.value_str(name)))
            //{
            //}


            //T operator()() const noexcept { return m_value; }
            //operator T() const noexcept { return m_value; }
            public T op() { return m_value; }
        }


        //using value_t = value_base_t<nl_fptype>;
        //using value_str_t = value_base_t<pstring>;


        public param_model_t(core_device_t device, string name, string val)
            : base(device, name, val)
        {
        }


        public string value_str(string entity)
        {
            return state().setup().models().get_model(str()).value_str(entity);
        }


        public nl_fptype value(string entity)
        {
            return state().setup().models().get_model(str()).value(entity);
        }


        public string type()
        {
            var mod = state().setup().models().get_model(str());
            return mod.type();
        }


        // hide this
        //void set(const pstring &param) = delete;


        protected override void changed() { }
    }


    // -----------------------------------------------------------------------------
    // data parameter
    // -----------------------------------------------------------------------------
    //class param_data_t : public param_str_t

    // -----------------------------------------------------------------------------
    // rom parameter
    // -----------------------------------------------------------------------------
    //template <typename ST, std::size_t AW, std::size_t DW>
    //class param_rom_t final: public param_data_t

} // namespace netlist
