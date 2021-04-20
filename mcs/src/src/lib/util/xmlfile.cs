// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using std_string = System.String;
using uint32_t = System.UInt32;


namespace mame.util.xml
{
    public static class xmlfile_global
    {
        //***************************************************************************
        //    MISCELLANEOUS INTERFACES
        //***************************************************************************

        //-------------------------------------------------
        //    normalize_string - normalize a string
        //    to ensure it doesn't contain embedded tags
        //-------------------------------------------------
        public static string normalize_string(string string_)
        {
            //throw new emu_unimplemented();

            return string_;
        }
    }


    // extended error information from parsing
    class parse_error
    {
        public string error_message;
        public int error_line;
        public int error_column;
    }


    // parsing options
    class parse_options
    {
        public parse_error error;
        //void                (*init_parser)(XML_ParserStruct *parser);
        uint32_t flags;
    }


    // a node representing a data item and its relationships
    public class data_node : global_object
    {
        // a node representing an attribute
        class attribute_node
        {
            public string name;
            public string value;

            //template <typename T, typename U>
            public attribute_node(string name, string value)
            {
                this.name = name;  //(std::forward<T>(name)),
                this.value = value;  //(std::forward<U>(value))
            }
        }


        //enum class int_format
        //{
        //    DECIMAL,
        //    DECIMAL_HASH,
        //    HEX_DOLLAR,
        //    HEX_C
        //};


        data_node m_next;
        data_node m_first_child;
        string m_name;
        string m_value;
        data_node m_parent;
        std.list<attribute_node> m_attributes;


        int line;           // line number for this node's start


        protected data_node()
        {
            line = 0;
            m_next = null;
            m_first_child = null;
            m_name = "";
            m_value = "";
            m_parent = null;
            m_attributes = new std.list<attribute_node>();
        }

        protected data_node(data_node parent, string name, string value)
        {
            line = 0;
            m_next = null;
            m_first_child = null;
            m_name = name;
            m_value = value != null ? value : "";
            m_parent = parent;
            m_attributes = new std.list<attribute_node>();


            m_name = m_name.ToLower();  //std::transform(m_name.begin(), m_name.end(), m_name.begin(), [] (char ch) { return std::tolower(uint8_t(ch)); });
        }

        //data_node(data_node const &) = delete;
        //data_node(data_node &&) = delete;
        //data_node &operator=(data_node &&) = delete;
        //data_node &operator=(data_node const &) = delete;

        //~data_node()
        //{
        //    free_children();
        //}


        // ----- XML node management -----

        public string get_name() { return m_name.empty() ? null : m_name.c_str(); }
        public string get_value() { return m_value.empty() ? null : m_value.c_str(); }
        //void set_value(char const *value);
        //void append_value(char const *value, int length);
        //void trim_whitespace();

        //data_node *get_parent() { return m_parent; }
        //data_node const *get_parent() const { return m_parent; }

        // count the number of children
        //std::size_t count_children() const;
        //std::size_t count_attributes() const;

        // get the first child
        public data_node get_first_child() { return m_first_child; }

        // find the first child with the given tag
        public data_node get_child(string name) { return m_first_child != null ? m_first_child.get_sibling(name) : null; }

        // find the first child with the given tag and/or attribute/value pair
        //data_node *find_first_matching_child(const char *name, const char *attribute, const char *matchval);
        //data_node const *find_first_matching_child(const char *name, const char *attribute, const char *matchval) const;


        // get the next sibling
        public data_node get_next_sibling() { return m_next; }

        // find the next sibling with the given tag
        public data_node get_next_sibling(string name) { return m_next != null ? m_next.get_sibling(name) : null; }

        // find the next sibling with the given tag and/or attribute/value pair
        //data_node *find_next_matching_sibling(const char *name, const char *attribute, const char *matchval);
        //data_node const *find_next_matching_sibling(const char *name, const char *attribute, const char *matchval) const;

        // add a new child node
        public data_node add_child(string name, string value)
        {
            if (string.IsNullOrEmpty(name))  //if (!name || !*name)
                return null;

            // new element: create a new node
            data_node node;
            node = new data_node(this, name, value);

            if (node.get_name() == null || (node.get_value() == null && value != null))
            {
                //delete node;
                return null;
            }

            // add us to the end of the list of siblings
            data_node pnode;
            if (m_first_child == null)
            {
                m_first_child = node;
            }
            else
            {
                for (pnode = m_first_child; pnode.m_next != null; pnode = pnode.m_next) { }

                pnode.m_next = node;
            }

            return node;
        }

        // either return an existing child node or create one if it doesn't exist
        //data_node *get_or_add_child(const char *name, const char *value);

        // recursively copy as child of another node
        //data_node *copy_into(data_node &parent) const;

        // delete a node and its children
        //void delete_node();


        // ----- XML attribute management -----

        // return whether a node has the specified attribute
        public bool has_attribute(string attribute)
        {
            return get_attribute(attribute) != null;
        }


        // return a pointer to the string value of an attribute, or nullptr if not present
        //-------------------------------------------------
        //  get_attribute_string_ptr - get a pointer to
        //  the string value of the specified attribute;
        //  if not found, return = nullptr
        //-------------------------------------------------
        public std_string get_attribute_string_ptr(string attribute)
        {
            attribute_node attr = get_attribute(attribute);
            return attr != null ? attr.value : null;
        }


        // return the string value of an attribute, or the specified default if not present
        public string get_attribute_string(string attribute, string defvalue)
        {
            attribute_node attr = get_attribute(attribute);
            return attr != null ? attr.value.c_str() : defvalue;
        }


        // return the integer value of an attribute, or the specified default if not present
        public Int64 get_attribute_int(string attribute, Int64 defvalue)  //long long get_attribute_int(const char *attribute, long long defvalue) const;
        {
            attribute_node attr = get_attribute(attribute);
            if (attr == null)
                return defvalue;
            std_string string_ = attr.value;

            //std::istringstream stream;
            //stream.imbue(f_portable_locale);
            Int64 result = 0;  //long long result;
            bool success = true;
            if (string_[0] == '$')
            {
                //stream.str(&string[1]);
                //unsigned long long uvalue;
                //stream >> std::hex >> uvalue;
                //result = static_cast<long long>(uvalue);
                string stream = string_.Substring(1);
                try { result = Convert.ToInt64(stream, 16); }
                catch (Exception) { success = false; }
            }
            else if ((string_[0] == '0') && ((string_[1] == 'x') || (string_[1] == 'X')))
            {
                //stream.str(&string[2]);
                //unsigned long long uvalue;
                //stream >> std::hex >> uvalue;
                //result = static_cast<long long>(uvalue);
                string stream = string_.Substring(2);
                try { result = Convert.ToInt64(stream, 16); }
                catch (Exception) { success = false; }
            }
            else if (string_[0] == '#')
            {
                //stream.str(&string[1]);
                //stream >> result;
                string stream = string_.Substring(1);
                success = Int64.TryParse(stream, out result);
            }
            else
            {
                //stream.str(&string[0]);
                //stream >> result;
                string stream = string_;
                success = Int64.TryParse(stream, out result);
            }

            return success ? result : defvalue;  //return stream ? result : defvalue;
        }


        // return the format of the given integer attribute
        //int_format get_attribute_int_format(const char *attribute) const;


        // return the float value of an attribute, or the specified default if not present
        //-------------------------------------------------
        //    get_attribute_float - get the float
        //    value of the specified attribute; if not
        //    found, return = the provided default
        //-------------------------------------------------
        public float get_attribute_float(string attribute, float defvalue)
        {
            attribute_node attr = get_attribute(attribute);
            if (attr == null)
                return defvalue;

            string stream = attr.value;  //std::istringstream stream(attr->value);
            //stream.imbue(f_portable_locale);
            //float result;
            //return (stream >> result) ? result : defvalue;
            return Convert.ToSingle(stream);
        }


        // set the string value of an attribute
        public void set_attribute(string name, string value)
        {
            attribute_node anode;

            // first find an existing one to replace
            anode = get_attribute(name);

            if (anode != null)
            {
                // if we found it, free the old value and replace it
                anode.value = value;
            }
            else
            {
                // otherwise, create a new node
                add_attribute(name, value);
            }
        }

        // set the integer value of an attribute
        public void set_attribute_int(string name, Int64 value) { set_attribute(name, string_format("{0}", value).c_str()); }  //void set_attribute_int(const char *name, long long value);

        // set the float value of an attribute
        public void set_attribute_float(string name, float value) { set_attribute(name, string_format("{0}", value).c_str()); }


        // add an attribute even if an attribute with the same name already exists
        void add_attribute(string name, string value)
        {
            //attribute_node &anode = *m_attributes.emplace(m_attributes.end(), name, value);
            attribute_node anode = new attribute_node(name, value);
            m_attributes.emplace_back(anode);
            anode.name = anode.name.ToLower();  //std::transform(anode.name.begin(), anode.name.end(), anode.name.begin(), [] (char ch) { return std::tolower(uint8_t(ch)); });
        }


        //void write_recursive(int indent, util::core_file &file) const;


        data_node get_sibling(string name)
        {
            // loop over siblings and find a matching name
            for (data_node node = this; node != null; node = node.get_next_sibling())
            {
                if (strcmp(node.get_name(), name) == 0)
                    return node;
            }

            return null;
        }


        //data_node *find_matching_sibling(const char *name, const char *attribute, const char *matchval);
        //data_node const *find_matching_sibling(const char *name, const char *attribute, const char *matchval) const;


        attribute_node get_attribute(string attribute)
        {
            // loop over attributes and find a match
            foreach (attribute_node anode in m_attributes)
            {
                if (strcmp(anode.name.c_str(), attribute) == 0)
                    return anode;
            }

            return null;
        }


        //void free_children();
    }


    // a node representing the root of a document
    class file : data_node
    {
        //using ptr = std::unique_ptr<file>;

        //file();
        //~file();


        // create a new, empty XML file
        public static file create()  //static ptr create();
        {
            //try { return ptr(new file); }
            //catch (...) { return ptr(); }
            return new file();
        }


        // parse an XML file into its nodes
        public static file read(util.core_file file, parse_options opts)  //static ptr read(util::core_file &file, parse_options const *opts);
        {
            throw new emu_unimplemented();
        }


        // parse an XML string into its nodes
        //static ptr string_read(const char *string, parse_options const *opts);

        // write an XML tree to a file
        //void write(util::core_file &file) const;
    }
}
