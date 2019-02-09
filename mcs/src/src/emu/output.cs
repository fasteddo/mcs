// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using notify_vector = mame.std.vector<mame.output_manager.output_notify>;
using s32 = System.Int32;
using u32 = System.UInt32;


namespace mame
{
    //typedef void (*output_notifier_func)(const char *outname, INT32 value, void *param);
    public delegate void output_notifier_func(string outname, int value, object param);


    // ======================> output_manager
    public class output_manager : global_object
    {
        //template <typename Input, std::make_unsigned_t<Input> DefaultMask> friend class devcb_write;


        public class output_notify
        {
            output_notifier_func m_notifier;       // callback to call
            object m_param;  //void *                  m_param;          // parameter to pass the callback

            public output_notify(output_notifier_func callback, object param)  // void *param)
            {
                m_notifier = callback;
                m_param = param;
            }

            //void operator()(char const *outname, s32 value) const { m_notifier(outname, value, m_param); }
            public void notify(string outname, int value) { m_notifier(outname, value, m_param); }
        }

        //using notify_vector = std::vector<output_notify>;

        public class output_item
        {
            output_manager m_manager;     // parent output manager
            string m_name;           // string name of the item
            u32 m_id;             // unique ID for this item
            s32 m_value;          // current value
            notify_vector m_notifylist = new notify_vector();     // list of notifier callbacks


            //output_item(output_item &&) = delete;
            //output_item(output_item const &) = delete;
            //output_item &operator=(output_item &&) = delete;
            //output_item &operator=(output_item const &) = delete;

            public output_item(
                    output_manager manager,
                    string name,
                    u32 id,
                    s32 value)
            {
                m_manager = manager;
                m_name = name;
                m_id = id;
                m_value = value;
                m_notifylist = new notify_vector();
            }

            //std::string const &name() const { return m_name; }
            //u32 id() const { return m_id; }
            public s32 get() { return m_value; }
            public void set(s32 value) { if (m_value != value) { notify(value); } }

            public void notify(s32 value)
            {
                if (OUTPUT_VERBOSE)
                    m_manager.machine().logerror("Output {0} = {1} (was {2})\n", m_name, value, m_value);
                m_value = value;

                // call the local notifiers first
                foreach (var notify in m_notifylist)
                    notify.notify(m_name.c_str(), value);

                // call the global notifiers next
                foreach (var notify in m_manager.m_global_notifylist)
                    notify.notify(m_name.c_str(), value);
            }

            public void set_notifier(output_notifier_func callback, object param) { m_notifylist.emplace_back(new output_notify(callback, param)); }
        }


        public class item_proxy : global_object
        {
            output_item m_item = null;

            //item_proxy() = default;

            //**************************************************************************
            //  OUTPUT ITEM PROXY
            //**************************************************************************
            public void resolve(device_t device, string name)
            {
                assert(m_item == null);
                m_item = device.machine().output().find_or_create_item(name.c_str(), 0);
            }

            //operator s32() const { return m_item->get(); }
            //s32 operator=(s32 value) { m_item->set(value); return m_item->get(); }
            public s32 op() { return m_item.get(); }
            public void op(s32 value) { m_item.set(value); }
        }

        //template <unsigned M, unsigned... N> struct item_proxy_array { typedef typename item_proxy_array<N...>::type type[M]; };
        //template <unsigned N> struct item_proxy_array<N> { typedef item_proxy type[N]; };
        //template <unsigned... N> using item_proxy_array_t = typename item_proxy_array<N...>::type;

        //template <typename X, unsigned... N>
        public class output_finder
        {
            int N;

            device_t m_device;
            string m_format;
            u32 m_start;
            u32 [] m_start_args;  //unsigned const              m_start_args[sizeof...(N)];
            item_proxy [] m_proxies;  //item_proxy_array_t<N...>    m_proxies;


            //template <typename... T>
            public output_finder(int N, device_t device, string format, u32 start)//, T &&... start_args)
            {
                this.N = N;

                m_device = device;
                m_format = format;

                m_start = start;
                //m_start_args;//{ std::forward<T>(start_args)... };
                m_start_args = new UInt32[N];
                m_proxies = new item_proxy[N];
                for (int i = 0; i < m_proxies.Length; i++)
                    m_proxies[i] = new item_proxy();
            }

            public s32 this[int n] { get { return m_proxies[n].op(); } set { m_proxies[n].op(value); } }  //auto &operator[](unsigned n) { return m_proxies[n]; }
            public s32 this[UInt32 n] { get { return m_proxies[n].op(); } set { m_proxies[n].op(value); } }  //auto &operator[](unsigned n) { return m_proxies[n]; }
            //auto &operator[](unsigned n) const { return m_proxies[n]; }

            //auto begin() { return std::begin(m_proxies); }
            //auto end() { return std::end(m_proxies); }
            //auto begin() const { return std::begin(m_proxies); }
            //auto end() const { return std::end(m_proxies); }
            //auto cbegin() const { return std::begin(m_proxies); }
            //auto cend() const { return std::end(m_proxies); }

            //void resolve() { resolve<0U>(m_proxies); }


            //template <unsigned A, unsigned C, typename... T>
            //void resolve(item_proxy (&proxies)[C], T &&... i)
            //{
            //    for (unsigned j = 0U; C > j; ++j)
            //        proxies[j].resolve(m_device, util::string_format(m_format, std::forward<T>(i)..., j + m_start_args[A]));
            //}
            public void resolve()
            {
                for (int i = 0; i < N; i++)
                {
                    m_proxies[i].resolve(m_device, string.Format(m_format, m_start + i));
                }
            }

            //template <unsigned A, unsigned C, unsigned D, typename T, typename... U>
            //void resolve(T (&proxies)[C][D], U &&... i)
            //{
            //    for (unsigned j = 0U; C > j; ++j)
            //        resolve<A + 1>(proxies[j], std::forward<U>(i)..., j + m_start_args[A]);
            //}
        }

        //template <typename X>
        class output_finder<X>
        {
            device_t m_device;
            string m_format;
            //item_proxy          m_proxy;

            output_finder(device_t device, string format)
            {
                m_device = device;
                m_format = format;
            }

            //operator s32() const { return m_proxy; }
            //s32 operator=(s32 value) { return m_proxy = value; }

            //void resolve() { m_proxy.resolve(m_device, m_format); }
        }


        public const bool OUTPUT_VERBOSE = false;


        // internal state
        running_machine m_machine;                  // reference to our machine
        std.unordered_map<string, output_item> m_itemtable = new std.unordered_map<string, output_item>();
        notify_vector m_global_notifylist = new notify_vector();
        u32 m_uniqueid;


        // construction/destruction
        //-------------------------------------------------
        //  output_manager - constructor
        //-------------------------------------------------
        public output_manager(running_machine machine)
        {
            m_machine = machine;
            m_uniqueid = 12345;


            /* add pause callback */
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_PAUSE, pause);
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_RESUME, resume);
        }


        // getters
        running_machine machine() { return m_machine; }


        // set the value for a given output
        /*-------------------------------------------------
            output_set_value - set the value of an output
        -------------------------------------------------*/
        void set_value(string outname, int value)
        {
            output_item item = find_item(outname);

            // if no item of that name, create a new one and force notification
            if (item == null)
                create_new_item(outname, value).notify(value);
            else
                item.set(value); // set the new value (notifies on change)
        }


        // return the current value for a given output
        /*-------------------------------------------------
            output_get_value - return the value of an
            output
        -------------------------------------------------*/
        public int get_value(string outname)
        {
            output_item item = find_item(outname);

            // if no item, value is 0
            return item != null ? item.get() : 0;
        }


        // set a notifier on a particular output, or globally if nullptr
        /*-------------------------------------------------
            output_set_notifier - sets a notifier callback
            for a particular output, or for all outputs
            if nullptr is specified
        -------------------------------------------------*/
        public void set_notifier(string outname, output_notifier_func callback, object param)  //void *param)
        {
            // if an item is specified, find/create it
            if (outname != null)
            {
                output_item item = find_item(outname);
                if (item != null) item.set_notifier(callback, param); else create_new_item(outname, 0).set_notifier(callback, param);  // (item ? *item : create_new_item(outname, 0)).set_notifier(callback, param);
            }
            else
            {
                m_global_notifylist.emplace_back(new output_notify(callback, param));
            }
        }


        // set a notifier on a particular output, or globally if nullptr
        //void notify_all(output_module *module);


        // map a name to a unique ID
        //UINT32 name_to_id(const char *outname);


        // map a unique ID back to a name
        //const char *id_to_name(UINT32 id);


        /*-------------------------------------------------
            output_pause - send pause message
        -------------------------------------------------*/
        void pause(running_machine machine)
        {
            set_value("pause", 1);
        }

        void resume(running_machine machine)
        {
            set_value("pause", 0);
        }


        // set an indexed value for an output (concatenates basename + index)
        /*-------------------------------------------------
            output_set_indexed_value - set the value of an
            indexed output
        -------------------------------------------------*/
        void set_indexed_value(string basename, int index, int value)
        {
            //char [] buffer = new char[100];
            //char *dest = buffer;

            /* copy the string */
            //while (*basename != 0)
            //    *dest++ = *basename++;

            string buffer = basename;

            /* append the index */
            if (index >= 1000) buffer += '0' + ((index / 1000) % 10);  // *dest++ = '0' + ((index / 1000) % 10);
            if (index >= 100) buffer += '0' + ((index / 100) % 10);  // *dest++ = '0' + ((index / 100) % 10);
            if (index >= 10) buffer += '0' + ((index / 10) % 10);  // *dest++ = '0' + ((index / 10) % 10);
            buffer += '0' + (index % 10);  // *dest++ = '0' + (index % 10);
            //*dest++ = 0;

            /* set the value */
            set_value(buffer, value);
        }


        /*-------------------------------------------------
            find_item - find an item based on a string
        -------------------------------------------------*/
        output_item find_item(string str)
        {
            var item = m_itemtable.find(str);
            if (item != null)
                return item;

            return null;
        }


        /*-------------------------------------------------
            create_new_item - create a new item
        -------------------------------------------------*/
        output_item create_new_item(string outname, int value)
        {
            var output_item = new output_item(this, outname, m_uniqueid++, value);
            var ins = m_itemtable.emplace(
                    //std::piecewise_construct,
                    outname,  //std::forward_as_tuple(outname),
                    output_item);  // std::forward_as_tuple(this, outname, m_uniqueid++, value));
            assert(ins);  //ins.second);
            return output_item;  //ins.first.second;
        }


        public output_item find_or_create_item(string outname, s32 value)
        {
            output_item item = find_item(outname);
            return item != null ? item : create_new_item(outname, value);
        }
    }


    //template <unsigned... N> using output_finder = output_manager::output_finder<void, N...>;
}
