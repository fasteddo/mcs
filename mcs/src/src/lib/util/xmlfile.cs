// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;


namespace mame.util.xml
{
    public static class xmlfile_global
    {
        /***************************************************************************
            MISCELLANEOUS INTERFACES
        ***************************************************************************/

        /*-------------------------------------------------
            normalize_string - normalize a string
            to ensure it doesn't contain embedded tags
        -------------------------------------------------*/
        public static string normalize_string(string string_)
        {
            //throw new emu_unimplemented();

            return string_;
        }
    }


    /* a node representing a data item and its relationships */
    public class data_node
    {
        //data_node *                 m_next;
        //data_node *                 m_first_child;
        //std::string                 m_name;
        //std::string                 m_value;
        //data_node *                 m_parent;
        //std::list<attribute_node>   m_attributes;

        //enum class int_format
        //{
        //    DECIMAL,
        //    DECIMAL_HASH,
        //    HEX_DOLLAR,
        //    HEX_C
        //};


        /* ----- XML node management ----- */

        //char const *get_name() const { return m_name.empty() ? nullptr : m_name.c_str(); }
        //
        //char const *get_value() const { return m_value.empty() ? nullptr : m_value.c_str(); }
        //void set_value(char const *value);
        //void append_value(char const *value, int length);
        //void trim_whitespace();

        //data_node *get_parent() { return m_parent; }
        //data_node const *get_parent() const { return m_parent; }

        // count the number of child nodes
        //int count_children() const;

        // get the first child
        //data_node *get_first_child() { return m_first_child; }
        //data_node const *get_first_child() const { return m_first_child; }

        // find the first child with the given tag
        //data_node *get_child(const char *name);
        //data_node const *get_child(const char *name) const;

        // find the first child with the given tag and/or attribute/value pair
        //data_node *find_first_matching_child(const char *name, const char *attribute, const char *matchval);
        //data_node const *find_first_matching_child(const char *name, const char *attribute, const char *matchval) const;

        // get the next sibling
        //data_node *get_next_sibling() { return m_next; }
        //data_node const *get_next_sibling() const { return m_next; }

        // find the next sibling with the given tag
        //data_node *get_next_sibling(const char *name);
        //data_node const *get_next_sibling(const char *name) const;

        // find the next sibling with the given tag and/or attribute/value pair
        //data_node *find_next_matching_sibling(const char *name, const char *attribute, const char *matchval);
        //data_node const *find_next_matching_sibling(const char *name, const char *attribute, const char *matchval) const;

        // add a new child node
        public data_node add_child(string name, string value)
        {
            //throw new emu_unimplemented();

            return new data_node();
        }

        // either return an existing child node or create one if it doesn't exist
        //data_node *get_or_add_child(const char *name, const char *value);

        // recursively copy as child of another node
        //data_node *copy_into(data_node &parent) const;

        // delete a node and its children
        //void delete_node();


        /* ----- XML attribute management ----- */

        // return whether a node has the specified attribute
        //bool has_attribute(const char *attribute) const;

        // return the string value of an attribute, or the specified default if not present
        //const char *get_attribute_string(const char *attribute, const char *defvalue) const;

        // return the integer value of an attribute, or the specified default if not present
        //int get_attribute_int(const char *attribute, int defvalue) const;

        // return the format of the given integer attribute
        //int_format get_attribute_int_format(const char *attribute) const;

        // return the float value of an attribute, or the specified default if not present
        //float get_attribute_float(const char *attribute, float defvalue) const;

        // set the string value of an attribute
        public void set_attribute(string name, string value)
        {
            //throw new emu_unimplemented();
        }

        // set the integer value of an attribute
        public void set_attribute_int(string name, int value)
        {
            //throw new emu_unimplemented();
        }

        // set the float value of an attribute
        public void set_attribute_float(string name, float value)
        {
            //throw new emu_unimplemented();
        }

        // add an attribute even if an attribute with the same name already exists
        //void add_attribute(const char *name, const char *value);


        //int                     line;           /* line number for this node's start */


        //data_node();
        //~data_node();

        //void write_recursive(int indent, util::core_file &file) const;


        // a node representing an attribute
        //struct attribute_node
        //{
        //    template <typename T, typename U> attribute_node(T &&name, U &&value) : name(std::forward<T>(name)), value(std::forward<U>(value)) { }
        //
        //    std::string name;
        //    std::string value;
        //};


        //data_node(data_node *parent, const char *name, const char *value);

        //data_node(data_node const &) = delete;
        //data_node(data_node &&) = delete;
        //data_node &operator=(data_node &&) = delete;
        //data_node &operator=(data_node const &) = delete;

        //data_node *get_sibling(const char *name);
        //data_node const *get_sibling(const char *name) const;
        //data_node *find_matching_sibling(const char *name, const char *attribute, const char *matchval);
        //data_node const *find_matching_sibling(const char *name, const char *attribute, const char *matchval) const;

        //attribute_node *get_attribute(const char *attribute);
        //attribute_node const *get_attribute(const char *attribute) const;

        //void free_children();
    }


    // a node representing the root of a document
    class file : data_node
    {
        //using ptr = std::unique_ptr<file>;


        //file();

        //~file();

        // create a new, empty XML file
        //static ptr create();
        public static file create()
        {
            //throw new emu_unimplemented();

            return new file();
        }

        // parse an XML file into its nodes
        //static ptr read(util::core_file &file, parse_options const *opts);

        // parse an XML string into its nodes
        //static ptr string_read(const char *string, parse_options const *opts);

        // write an XML tree to a file
        //void write(util::core_file &file) const;
    }
}
