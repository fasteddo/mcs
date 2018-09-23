// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    // ======================> device_delegate_helper
    // device_delegate_helper does non-template work
    public class device_delegate_helper
    {
        // internal state
        string m_device_name;


        // constructor
        public device_delegate_helper(string devname)
        {
            m_device_name = devname;
        }

        // internal helpers
        //delegate_late_bind &bound_object(device_t &search_root);

        //-------------------------------------------------
        //  safe_tag - return a tag string or (unknown) if
        //  the object is not valid
        //-------------------------------------------------
        static string safe_tag(device_t obj)
        {
            return (obj != null) ? obj.tag() : "(unknown)";
        }
    };


    // ======================> device_delegate
    // device_delegate is a delegate that wraps with a device tag and can be easily
    // late bound without replicating logic everywhere
    //template<typename Signature>
    //class device_delegate : public named_delegate<Signature>, public device_delegate_helper
    public class device_delegate : device_delegate_helper
    {
        //using thistype = device_delegate<Signature>;
        //using basetype = named_delegate<Signature>;
        //template <class FunctionClass> using member_func_type = typename basetype::template member_func_type<FunctionClass>;
        //template <class FunctionClass> using const_member_func_type = typename basetype::template const_member_func_type<FunctionClass>;
        //template <class FunctionClass> using static_func_type = typename basetype::template static_func_type<FunctionClass>;
        //template <class FunctionClass> using static_ref_func_type = typename basetype::template static_ref_func_type<FunctionClass>;

        // provide the same constructors as the base class
        public device_delegate() : base(null) { }
        //device_delegate(const basetype &src) : basetype(src), device_delegate_helper(src.m_device_name) { }
        //device_delegate(const basetype &src, delegate_late_bind &object) : basetype(src, object), device_delegate_helper(src.m_device_name) { }
        //template <class FunctionClass> device_delegate(member_func_type<FunctionClass> funcptr, const char *name, FunctionClass *object) : basetype(funcptr, name, object), device_delegate_helper(safe_tag(dynamic_cast<device_t *>(object))) { }
        //template <class FunctionClass> device_delegate(const_member_func_type<FunctionClass> funcptr, const char *name, FunctionClass *object) : basetype(funcptr, name, object), device_delegate_helper(safe_tag(dynamic_cast<device_t *>(object))) { }
        //template <class FunctionClass> device_delegate(static_func_type<FunctionClass> funcptr, const char *name, FunctionClass *object) : basetype(funcptr, name, object), device_delegate_helper(safe_tag(dynamic_cast<device_t *>(object))) { }
        //template <class FunctionClass> device_delegate(static_ref_func_type<FunctionClass> funcptr, const char *name, FunctionClass *object) : basetype(funcptr, name, object), device_delegate_helper(safe_tag(dynamic_cast<device_t *>(object))) { }
        //device_delegate(std::function<Signature> funcptr, const char *name) : basetype(funcptr, name), device_delegate_helper(nullptr) { }
        //device_delegate &operator=(const thistype &src) { basetype::operator=(src); m_device_name = src.m_device_name; return *this; }

        // provide additional constructors that take a device name string
        //template <class FunctionClass> device_delegate(member_func_type<FunctionClass> funcptr, const char *name, const char *devname) : basetype(funcptr, name, static_cast<FunctionClass *>(nullptr)), device_delegate_helper(devname) { }
        //template <class FunctionClass> device_delegate(member_func_type<FunctionClass> funcptr, const char *name, const char *devname, FunctionClass *) : basetype(funcptr, name, static_cast<FunctionClass *>(nullptr)), device_delegate_helper(devname) { }
        //template <class FunctionClass> device_delegate(const_member_func_type<FunctionClass> funcptr, const char *name, const char *devname) : basetype(funcptr, name, static_cast<FunctionClass *>(nullptr)), device_delegate_helper(devname) { }
        //template <class FunctionClass> device_delegate(const_member_func_type<FunctionClass> funcptr, const char *name, const char *devname, FunctionClass *) : basetype(funcptr, name, static_cast<FunctionClass *>(nullptr)), device_delegate_helper(devname) { }
        //template <class FunctionClass> device_delegate(static_func_type<FunctionClass> funcptr, const char *name, const char *devname, FunctionClass *) : basetype(funcptr, name, static_cast<FunctionClass *>(nullptr)), device_delegate_helper(devname) { }
        //template <class FunctionClass> device_delegate(static_ref_func_type<FunctionClass> funcptr, const char *name, const char *devname, FunctionClass *) : basetype(funcptr, name, static_cast<FunctionClass *>(nullptr)), device_delegate_helper(devname) { }
        //device_delegate(static_func_type<device_t> funcptr, const char *name) : basetype(funcptr, name, static_cast<device_t *>(nullptr)), device_delegate_helper(nullptr) { }
        //device_delegate(static_ref_func_type<device_t> funcptr, const char *name) : basetype(funcptr, name, static_cast<device_t *>(nullptr)), device_delegate_helper(nullptr) { }

        // and constructors that provide a search root
        //device_delegate(const thistype &src, device_t &search_root) : basetype(src), device_delegate_helper(src.m_device_name) { bind_relative_to(search_root); }

        // perform the binding
        //void bind_relative_to(device_t &search_root) { if (!basetype::isnull()) basetype::late_bind(bound_object(search_root)); }

        // getter (for validation purposes)
        //const char *device_name() const { return m_device_name; }
    }
}
