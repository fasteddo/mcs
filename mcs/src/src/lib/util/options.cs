// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using size_t = System.UInt64;

using static mame.corestr_global;
using static mame.cpp_global;
using static mame.options_global;
using static mame.options_internal;


namespace mame
{
    public static partial class options_global
    {
        //**************************************************************************
        //  CONSTANTS
        //**************************************************************************

        // option priorities
        public const int OPTION_PRIORITY_DEFAULT   = 0;            // defaults are at 0 priority
        public const int OPTION_PRIORITY_LOW       = 50;           // low priority
        public const int OPTION_PRIORITY_NORMAL    = 100;          // normal priority
        public const int OPTION_PRIORITY_HIGH      = 150;          // high priority
        public const int OPTION_PRIORITY_MAXIMUM   = 255;          // maximum priority
    }


    // exception thrown by core_options when an illegal request is made
    class options_exception : emu_exception
    {
        string m_message;

        protected options_exception(string message) { m_message = message; }

        public string message() { return m_message; }
        //virtual const char *what() const noexcept override { return message().c_str(); }
    }

    class options_warning_exception : options_exception
    {
        //template <typename... Params>
        public options_warning_exception(string format, params object [] args) : base(string.Format(format, args)) { }
        public options_warning_exception(string message) : base(message) { }
        //options_warning_exception(const options_warning_exception &) = default;
        //options_warning_exception(options_warning_exception &&) = default;
        //options_warning_exception& operator=(const options_warning_exception &) = default;
        //options_warning_exception& operator=(options_warning_exception &&) = default;
    }

    class options_error_exception : options_exception
    {
        //template <typename... Params>
        public options_error_exception(string format, params object [] args) : base(string.Format(format, args)) { }
        public options_error_exception(string message) : base(message) { }
        //options_error_exception(const options_error_exception &) = default;
        //options_error_exception(options_error_exception &&) = default;
        //options_error_exception& operator=(const options_error_exception &) = default;
        //options_error_exception& operator=(options_error_exception &&) = default;
    }


    public class core_options : IEnumerable<core_options.entry>
    {
        public enum option_type
        {
            INVALID,         // invalid
            HEADER,          // a header item
            COMMAND,         // a command
            BOOLEAN,         // boolean option
            INTEGER,         // integer option
            FLOAT,           // floating-point option
            STRING           // string option
        }


        // used internally in core_options
        enum condition_type
        {
            NONE,
            WARN,
            ERR
        }


        public enum override_get_value_result
        {
            NONE,
            OVERRIDE,
            SKIP
        }


        // information about a single entry in the options
        public abstract class entry
        {
            //typedef std::shared_ptr<entry> shared_ptr;
            //typedef std::weak_ptr<entry> weak_ptr;


            std.vector<string> m_names;
            int m_priority;
            core_options.option_type m_type;
            string m_description;
            Action<string> m_value_changed_handler;  //std::function<void(const char *)>           m_value_changed_handler;


            // construction/destruction
            protected entry(std.vector<string> names, option_type type = option_type.STRING, string description = null)
            {
                m_names = names;
                m_priority = OPTION_PRIORITY_DEFAULT;
                m_type = type;
                m_description = description;


                assert(m_names.empty() == (m_type == option_type.HEADER));
            }

            protected entry(string name, option_type type = option_type.STRING, string description = null)
                : this(new std.vector<string>() { name }, type, description)
            {
            }

            //entry(const entry &) = delete;
            //entry(entry &&) = delete;
            //entry& operator=(const entry &) = delete;
            //entry& operator=(entry &&) = delete;


            // accessors
            public MemoryContainer<string> names() { return m_names; }
            public string name() { return m_names[0]; }

            //-------------------------------------------------
            //  entry::value
            //-------------------------------------------------
            public virtual string value()
            {
                // returning 'nullptr' from here signifies a value entry that is essentially "write only"
                // and cannot be meaningfully persisted (e.g. - a command or the software name)
                return null;
            }

            public int priority() { return m_priority;  }
            public void set_priority(int priority) { m_priority = priority; }
            public option_type type() { return m_type; }
            public string description() { return m_description; }
            public virtual string default_value()
            {
                // I don't really want this generally available, but MewUI seems to need it.  Please
                // do not use
                throw new emu_exception();  // abort();
            }
            protected virtual string minimum() { return null; }
            protected virtual string maximum() { return null; }
            bool has_range() { return minimum() != null && maximum() != null; }


            // mutators

            //-------------------------------------------------
            //  entry::set_value
            //-------------------------------------------------
            public void set_value(string newvalue, int priority_value, bool always_override = false)
            {
                // it is invalid to set the value on a header
                assert(type() != option_type.HEADER);

                validate(newvalue);

                // only set the value if we have priority
                if (always_override || priority_value >= priority())
                {
                    internal_set_value(newvalue);
                    m_priority = priority_value;

                    // invoke the value changed handler, if appropriate
                    if (m_value_changed_handler != null)
                        m_value_changed_handler(value());
                }
            }


            //-------------------------------------------------
            //  entry::set_default_value
            //-------------------------------------------------
            public virtual void set_default_value(string newvalue)
            {
                // set_default_value() is not necessarily supported for all entry types
                throw new emu_exception();  //false;
            }


            //-------------------------------------------------
            //  set_description - set the description of
            //  an option
            //-------------------------------------------------
            public void set_description(string description)
            {
                m_description = description;
            }


            public void set_value_changed_handler(Action<string> handler) { m_value_changed_handler = handler; }  //std::function<void(const char *)> &&handler) { m_value_changed_handler = std::move(handler); }


            public virtual void revert(int priority_hi, int priority_lo) { }


            protected abstract void internal_set_value(string newvalue);


            void validate(string value)
            {
                //throw new emu_unimplemented();
            }
        }


        class simple_entry : entry
        {
            // internal state
            string m_data;             // data for this item
            string m_defdata;          // default data for this item
            string m_minimum;          // minimum value
            string m_maximum;          // maximum value


            // construction/destruction
            public simple_entry(std.vector<string> names, string description, option_type type, string defdata, string minimum, string maximum)
                : base(names, type, description)
            {
                m_defdata = defdata;
                m_minimum = minimum;
                m_maximum = maximum;


                m_data = m_defdata;
            }

            //simple_entry(const simple_entry &) = delete;
            //simple_entry(simple_entry &&) = delete;
            //simple_entry& operator=(const simple_entry &) = delete;
            //simple_entry& operator=(simple_entry &&) = delete;


            // getters
            //-------------------------------------------------
            //  simple_entry::value
            //-------------------------------------------------
            public override string value()
            {
                string result;
                switch (type())
                {
                case option_type.BOOLEAN:
                case option_type.INTEGER:
                case option_type.FLOAT:
                case option_type.STRING:
                    return m_data;

                default:
                    // this is an option type for which returning a value is
                    // a meaningless operation (e.g. - core_options::option_type::COMMAND)
                    return null;
                }
            }

            //-------------------------------------------------
            //  minimum
            //-------------------------------------------------
            protected override string minimum()
            {
                return m_minimum;
            }

            //-------------------------------------------------
            //  maximum
            //-------------------------------------------------
            protected override string maximum()
            {
                return m_maximum;
            }

            //-------------------------------------------------
            //  simple_entry::default_value
            //-------------------------------------------------
            public override string default_value()
            {
                // only MewUI seems to need this; please don't use
                return m_defdata;
            }

            //-------------------------------------------------
            //  revert - revert back to our default if we are
            //  within the given priority range
            //-------------------------------------------------
            public override void revert(int priority_hi, int priority_lo)
            {
                // if our priority is within the range, revert to the default
                if (priority() <= priority_hi && priority() >= priority_lo)
                {
                    set_value(default_value(), priority(), true);
                    set_priority(OPTION_PRIORITY_DEFAULT);
                }
            }

            //-------------------------------------------------
            //  set_default_value
            //-------------------------------------------------
            public override void set_default_value(string newvalue)
            {
                m_data = m_defdata = newvalue;
            }

            //-------------------------------------------------
            //  internal_set_value
            //-------------------------------------------------
            protected override void internal_set_value(string newvalue)
            {
                m_data = newvalue;
            }
        }


        static readonly string [] s_option_unadorned = new string []
        {
            "<UNADORNED0>",
            "<UNADORNED1>",
            "<UNADORNED2>",
            "<UNADORNED3>",
            "<UNADORNED4>",
            "<UNADORNED5>",
            "<UNADORNED6>",
            "<UNADORNED7>",
            "<UNADORNED8>",
            "<UNADORNED9>",
            "<UNADORNED10>",
            "<UNADORNED11>",
            "<UNADORNED12>",
            "<UNADORNED13>",
            "<UNADORNED14>",
            "<UNADORNED15>"
        };

        const int MAX_UNADORNED_OPTIONS = 16;


        // internal state
        std.vector<entry> m_entries = new std.vector<entry>();  //std::vector<entry::shared_ptr>                      m_entries;              // canonical list of entries
        std.unordered_map<string, entry> m_entrymap = new std.unordered_map<string, entry>();  //std::unordered_map<std::string, entry::weak_ptr>    m_entrymap;             // map for fast lookup
        string m_command;              // command found
        std.vector<string> m_command_arguments = new std.vector<string>();   // command arguments


        //core_options(const core_options &) = delete;
        //core_options(core_options &&) = delete;
        //core_options& operator=(const core_options &) = delete;
        //core_options& operator=(core_options &&) = delete;


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<entry> GetEnumerator()
        {
            return m_entries.GetEnumerator();
        }


        // getters
        public string command() { return m_command; }
        public std.vector<string> command_arguments() { assert(!m_command.empty()); return m_command_arguments; }

        public entry get_entry(string name)
        {
            var curentry = m_entrymap.find(name);
            return (curentry != null) ? curentry : null;
        }

        //entry::shared_ptr get_entry(const std::string &name);
        public MemoryContainer<entry> entries() { return m_entries; }
        public bool exists(string name) { return get_entry(name) != null; }
        //bool header_exists(const char *description) const;


        // configuration

        //-------------------------------------------------
        //  add_entry - adds an entry
        //-------------------------------------------------
        protected void add_entry(entry entry, string after_header = null)
        {
            // update the entry map
            foreach (string name in entry.names())
            {
                // append the entry
                add_to_entry_map(name, entry);
            }

            // and add the entry to the vector
            m_entries.emplace_back(entry);
        }


        //-------------------------------------------------
        //  add_entry - adds an entry based on an
        //  options_entry
        //-------------------------------------------------
        void add_entry(options_entry opt, bool override_existing = false)
        {
            std.vector<string> names = new std.vector<string>();
            string minimum = "";
            string maximum = "";

            // copy in the name(s) as appropriate
            if (opt.name != null)
            {
                // first extract any range
                string namestr = opt.name;
                size_t lparen = namestr.find_first_of('(', 0);
                if (lparen != npos)
                {
                    size_t dash = namestr.find_first_of('-', lparen + 1);
                    if (dash != npos)
                    {
                        size_t rparen = namestr.find_first_of(')', dash + 1);
                        if (rparen != npos)
                        {
                            minimum = namestr.Substring((int)lparen + 1, (int)(dash - (lparen + 1))).Trim();  //minimum.assign(strtrimspace(std::string_view(&namestr[lparen + 1], dash - (lparen + 1))));
                            maximum = namestr.Substring((int)dash + 1, (int)(rparen - (dash + 1))).Trim();  //maximum.assign(strtrimspace(std::string_view(&namestr[dash + 1], rparen - (dash + 1))));
                            namestr = namestr.Remove((int)lparen, (int)(rparen + 1 - lparen));  //namestr.erase(lparen, rparen + 1 - lparen);
                        }
                    }
                }

                // then chop up any semicolon-separated names
                size_t semi;
                while ((semi = namestr.find_first_of(';')) != npos)
                {
                    names.push_back(namestr.substr(0, semi));

                    // for booleans, add the "-noXYZ" option as well
                    if (opt.type == option_type.BOOLEAN)
                        names.push_back("no" + names.back());

                    namestr = namestr.Remove(0, (int)semi + 1);  //namestr.erase(0, semi + 1);
                }

                // finally add the last item
                names.push_back(namestr);
                if (opt.type == option_type.BOOLEAN)
                    names.push_back("no" + names.back());
            }

            // we might be called with an existing entry
            entry existing_entry = null;
            do
            {
                foreach (string name in names)
                {
                    existing_entry = get_entry(name);
                    if (existing_entry != null)
                        break;
                }

                if (existing_entry != null)
                {
                    if (override_existing)
                        remove_entry(existing_entry);
                    else
                        return;
                }
            } while (existing_entry != null);

            // set the default value
            string defdata = opt.defvalue != null ? opt.defvalue : "";

            // create and add the entry
            add_entry(
                    names,
                    opt.description,
                    opt.type,
                    defdata,
                    minimum,
                    maximum);
        }


        //-------------------------------------------------
        //  add_entry
        //-------------------------------------------------
        void add_entry(std.vector<string> names, string description, option_type type, string default_value = "", string minimum = "", string maximum = "")
        {
            // create the entry
            entry new_entry = new simple_entry(
                    names,
                    description,
                    type,
                    default_value,
                    minimum,
                    maximum);

            // and add it
            add_entry(new_entry);
        }


        //-------------------------------------------------
        //  add_header
        //-------------------------------------------------
        void add_header(string description)
        {
            add_entry(new std.vector<string>(), description, option_type.HEADER);
        }


        //-------------------------------------------------
        //  add_entries - add entries to the current
        //  options sets
        //-------------------------------------------------
        public void add_entries(IEnumerable<options_entry> entrylist, bool override_existing = false)
        {
            foreach (var entry in entrylist)  // for (int i = 0; entrylist[i].name || entrylist[i].type == option_type.HEADER; i++)
            {
                if (!(entry.name != null || entry.type == option_type.HEADER))
                    break;

                add_entry(entry, override_existing);  //entrylist[i], override_existing);
            }
        }


        //-------------------------------------------------
        //  set_default_value - change the default value
        //  of an option
        //-------------------------------------------------
        public void set_default_value(string name, string defvalue)
        {
            // update the data and default data
            var entry = get_entry(name);
            assert(entry != null);
            entry.set_default_value(defvalue);
        }


        //-------------------------------------------------
        //  set_description - change the description
        //  of an option
        //-------------------------------------------------
        public void set_description(string name, string description)
        {
            // update the data and default data
            var entry = get_entry(name);
            assert(entry != null);
            entry.set_description(description);
        }


        //-------------------------------------------------
        //  remove_entry - remove an entry from our list
        //  and map
        //-------------------------------------------------
        void remove_entry(entry delentry)
        {
            throw new emu_unimplemented();
        }


        protected void set_value_changed_handler(string name, Action<string> handler)  //void core_options::set_value_changed_handler(std::string_view name, std::function<void(const char *)> &&handler)
        {
            var entry = get_entry(name);
            assert(entry != null);
            entry.set_value_changed_handler(handler);
        }


        //-------------------------------------------------
        //  revert - revert options at or below a certain
        //  priority back to their defaults
        //-------------------------------------------------
        public void revert(int priority_hi = OPTION_PRIORITY_MAXIMUM, int priority_lo = OPTION_PRIORITY_DEFAULT)
        {
            foreach (entry curentry in m_entries)
            {
                if (curentry.type() != option_type.HEADER && curentry.type() != option_type.COMMAND)
                    curentry.revert(priority_hi, priority_lo);
            }
        }


        // parsing/input
        //-------------------------------------------------
        //  parse_command_line - parse a series of
        //  command line arguments
        //-------------------------------------------------
        public void parse_command_line(std.vector<string> args, int priority, bool ignore_unknown_options = false)
        {
            string error_stream = "";  //std::ostringstream error_stream;
            condition_type condition = condition_type.NONE;

            // reset the errors and the command
            m_command = "";

            // we want to identify commands first
            for (int arg = 1; arg < args.Count; arg++)
            {
                if (!args[(int)arg].empty() && args[(int)arg][0] == '-')
                {
                    var curentry = get_entry(args[arg][1..]);
                    if (curentry != null && curentry.type() == core_options.option_type.COMMAND)
                    {
                        // can only have one command
                        if (!m_command.empty())
                            throw new options_error_exception("Error: multiple commands specified -{0} and {1}\n", m_command, args[arg]);

                        m_command = curentry.name();
                    }
                }
            }

            // iterate through arguments
            int unadorned_index = 0;
            for (int arg = 1; arg < args.Count; arg++)
            {
                // determine the entry name to search for
                string curarg = args[(int)arg];
                bool is_unadorned = curarg[0] != '-';
                string optionname = is_unadorned ? core_options.unadorned(unadorned_index++) : curarg.Remove(0, 1);  // curarg[1];

                // special case - collect unadorned arguments after commands into a special place
                if (is_unadorned && !m_command.empty())
                {
                    m_command_arguments.push_back(args[(int)arg]);
                    command_argument_processed();
                    continue;
                }

                // find our entry; if not found, continue
                var curentry = get_entry(optionname);
                if (curentry == null)
                {
                    if (!ignore_unknown_options)
                        throw new options_error_exception("Error: unknown option: -{0}\n", optionname);
                    continue;
                }

                // at this point, we've already processed commands
                if (curentry.type() == core_options.option_type.COMMAND)
                    continue;

                // get the data for this argument, special casing booleans
                string newdata;
                if (curentry.type() == option_type.BOOLEAN)
                {
                    newdata = string.Compare(curarg.Remove(0, 1), 0, "no", 0, 2) == 0 ? "0" : "1";  //(strncmp(&curarg[1], "no", 2) == 0) ? "0" : "1";
                }
                else if (is_unadorned)
                {
                    newdata = curarg;
                }
                else if (arg + 1 < args.Count)
                {
                    newdata = args[++arg];
                }
                else
                {
                    throw new options_error_exception("Error: option {0} expected a parameter\n", curarg);
                }

                // set the new data
                do_set_value(curentry, newdata, priority, ref error_stream, condition);
            }

            // did we have any errors that may need to be aggregated?
            throw_options_exception_if_appropriate(condition, error_stream);
        }


        //-------------------------------------------------
        //  parse_ini_file - parse a series of entries in
        //  an INI file
        //-------------------------------------------------
        public void parse_ini_file(util.core_file inifile, int priority, bool ignore_unknown_options, bool always_override)
        {
            string error_stream = "";  //std::ostringstream error_stream;
            condition_type condition = condition_type.NONE;

            // loop over lines in the file
            string buffer;  //char buffer[4096];
            while (inifile.gets(out buffer, 4096) != null)  //while (inifile.gets(buffer, std::size(buffer)) != nullptr)
            {
                // find the extent of the name
                int optionnameIdx = 0;  //char *optionname;
                for (optionnameIdx = 0; optionnameIdx < buffer.Length; optionnameIdx++)  //for (optionname = buffer; *optionname != 0; optionname++)
                {
                    if (isspace(buffer[optionnameIdx]) == 0)  //if (!isspace((uint8_t)*optionname))
                        break;
                }

                // skip comments
                if (optionnameIdx >= buffer.Length || buffer[optionnameIdx] == 0 || buffer[optionnameIdx] == '#')  //if (*optionname == 0 || *optionname == '#')
                    continue;

                // scan forward to find the first space
                int tempIdx;  //char *temp;
                for (tempIdx = optionnameIdx; tempIdx < buffer.Length; tempIdx++)  //for (temp = optionname; *temp != 0; temp++)
                {
                    if (isspace(buffer[tempIdx]) != 0)  //if (isspace((uint8_t)*temp))
                        break;
                }

                // if we hit the end early, print a warning and continue
                if (tempIdx >= buffer.Length || buffer[tempIdx] == 0)  //if (*temp == 0)
                {
                    condition = (condition_type)std.max((UInt32)condition, (UInt32)condition_type.WARN);
                    util.stream_format(ref error_stream, "Warning: invalid line in INI: {0}", buffer);
                    continue;
                }

                // NULL-terminate
                tempIdx++;  //*temp++ = 0;
                int optiondataIdx = tempIdx;  //char *optiondata = temp;

                // scan the data, stopping when we hit a comment
                bool inquotes = false;
                for (tempIdx = optiondataIdx; tempIdx < buffer.Length; tempIdx++)  //for (temp = optiondata; *temp != 0; temp++)
                {
                    if (buffer[tempIdx] == '"')
                        inquotes = !inquotes;

                    if (buffer[tempIdx] == '#' && !inquotes)
                        break;
                }

                //*temp = 0;

                string optionname = buffer.Substring(optionnameIdx, optiondataIdx - optionnameIdx - 1);
                string optiondata = buffer[optiondataIdx..];

                // find our entry
                entry curentry = get_entry(optionname);  //entry::shared_ptr curentry = get_entry(optionname);
                if (curentry == null)
                {
                    if (!ignore_unknown_options)
                    {
                        condition = (condition_type)std.max((UInt32)condition, (UInt32)condition_type.WARN);
                        util.stream_format(ref error_stream, "Warning: unknown option in INI: {0}\n", optionname);
                    }
                    continue;
                }

                // set the new data
                do_set_value(curentry, trim_spaces_and_quotes(optiondata), priority, ref error_stream, condition);
            }

            // did we have any errors that may need to be aggregated?
            throw_options_exception_if_appropriate(condition, error_stream);
        }


        //void copy_from(const core_options &that);


        // output

        //-------------------------------------------------
        //  output_ini - output the options in INI format,
        //  only outputting entries that different from
        //  the optional diff
        //-------------------------------------------------
        public string output_ini(core_options diff = null)
        {
            // INI files are complete, so always start with a blank buffer
            string buffer = "";
            //buffer.imbue(std::locale::classic());

            int num_valid_headers = 0;
            int unadorned_index = 0;
            string last_header = null;
            string overridden_value;

            // loop over all items
            foreach (var curentry in m_entries)
            {
                if (curentry.type() == option_type.HEADER)
                {
                    // header: record description
                    last_header = curentry.description();
                }
                else
                {
                    string name = curentry.name();
                    string value = curentry.value();

                    // check if it's unadorned
                    bool is_unadorned = false;
                    if (name == core_options.unadorned(unadorned_index))
                    {
                        unadorned_index++;
                        is_unadorned = true;
                    }

                    // output entries for all non-command items (items with value)
                    if (value != null)
                    {
                        // look up counterpart in diff, if diff is specified
                        if (diff == null || std.strcmp(value, diff.value(name)) != 0)
                        {
                            // output header, if we have one
                            if (last_header != null)
                            {
                                if (num_valid_headers++ != 0)
                                    buffer += "\n";
                                util.stream_format(ref buffer, "#\n# {0}\n#\n", last_header);
                                last_header = null;
                            }

                            // and finally output the data, skip if unadorned
                            if (!is_unadorned)
                            {
                                if (value.IndexOf(' ') != -1)
                                    util.stream_format(ref buffer, "{0,-25} \"{1}\"\n", name, value);  // %-25s \"%s\"\n
                                else
                                    util.stream_format(ref buffer, "{0,-25} {1}\n", name, value);  // %-25s %s\n
                            }
                        }
                    }
                }
            }

            return buffer;
        }


        //-------------------------------------------------
        //  output_help - output option help to a string
        //-------------------------------------------------
        public string output_help()
        {
            // start empty
            string buffer = "";

            // loop over all items
            foreach (var curentry in m_entries)
            {
                // header: just print
                if (curentry.type() == option_type.HEADER)
                    util.stream_format(ref buffer, "\n#\n# {0}\n#\n", curentry.description());

                // otherwise, output entries for all non-deprecated items
                else if (!string.IsNullOrEmpty(curentry.description()))
                    util.stream_format(ref buffer, "-{0,-20}{1}\n", curentry.name(), curentry.description());  // -%-20s%s
            }

            return buffer;
        }


        // reading

        //-------------------------------------------------
        //  value - return the raw option value
        //-------------------------------------------------
        public string value(string option)
        {
            var entry = get_entry(option);
            return entry != null ? entry.value() : null;
        }

        //-------------------------------------------------
        //  description - return description of option
        //-------------------------------------------------
        public string description(string option)
        {
            var entry = get_entry(option);
            return entry != null ? entry.description() : null;
        }

        public bool bool_value(string option) { return int_value(option) != 0; }

        public int int_value(string option)
        {
            string data = value(option);
            if (data == null)
                return 0;
            string str = data;  //std::istringstream str(data);
            //str.imbue(std::locale::classic());
            int ival;
            if (int.TryParse(str, out ival))  //if (str >> ival)
                return ival;
            else
                return 0;
        }

        public float float_value(string option)
        {
            string data = value(option);
            if (data == null)
                return 0.0f;
            string str = data;  //std::istringstream str(data);
            //str.imbue(std::locale::classic());
            float fval;
            if (float.TryParse(str, out fval))  //if (str >> fval)
                return fval;
            else
                return 0.0f;
        }


        // setting

        //**************************************************************************
        //  LEGACY
        //**************************************************************************
        //-------------------------------------------------
        //  set_value - set the raw option value
        //-------------------------------------------------
        public void set_value(string name, string value, int priority)
        {
            var entry = get_entry(name);
            assert(entry != null);
            entry.set_value(value, priority);
        }

        //void set_value(const std::string &name, std::string &&value, int priority);

        public void set_value(string name, int value, int priority)
        {
            set_value(name, value.ToString(), priority);
        }

        public void set_value(string name, float value, int priority)
        {
            set_value(name, value.ToString(), priority);
        }


        // misc
        public static string unadorned(int x = 0) { return s_option_unadorned[std.min(x, MAX_UNADORNED_OPTIONS - 1)]; }


        protected virtual void command_argument_processed() { }


        // internal helpers

        //-------------------------------------------------
        //  add_to_entry_map - adds an entry to the entry
        //  map
        //-------------------------------------------------
        void add_to_entry_map(string name, entry entry)
        {
            // it is illegal to call this method for something that already exists
            assert(m_entrymap.find(name) == null);

            // append the entry
            m_entrymap.emplace(name, entry);
        }


        //-------------------------------------------------
        //  do_set_value
        //-------------------------------------------------
        void do_set_value(entry curentry, string data, int priority, ref string error_stream, condition_type condition)
        {
            // this is called when parsing a command line or an INI - we want to catch the option_exception and write
            // any exception messages to the error stream
            try
            {
                curentry.set_value(data, priority);
            }
            catch (options_warning_exception ex)
            {
                // we want to aggregate option exceptions
                error_stream += ex.message();
                condition = (condition_type)std.max((int)condition, (int)condition_type.WARN);
            }
            catch (options_error_exception ex)
            {
                // we want to aggregate option exceptions
                error_stream += ex.message();
                condition = (condition_type)std.max((int)condition, (int)condition_type.ERR);
            }
        }


        //-------------------------------------------------
        //  throw_options_exception_if_appropriate
        //-------------------------------------------------
        void throw_options_exception_if_appropriate(condition_type condition, string error_stream)
        {
            switch (condition)
            {
            case condition_type.NONE:
                // do nothing
                break;

            case condition_type.WARN:
                throw new options_warning_exception(error_stream);

            case condition_type.ERR:
                throw new options_error_exception(error_stream);

            default:
                // should not get here
                throw new emu_exception();  // false;
            }
        }
    }


    // static structure describing a single option with its description and default value
    public struct options_entry
    {
        public string name;               // name on the command line
        public string defvalue;           // default value of this argument
        public core_options.option_type type;              // flags to describe the option
        public string description;        // description for -showusage

        public options_entry(string name) : this(name, null, 0, null) { }
        public options_entry(string name, string defvalue, core_options.option_type type, string description)
        { this.name = name; this.defvalue = defvalue; this.type = type; this.description = description; }
    }


    public static class options_internal
    {
        public static string trim_spaces_and_quotes(string data)
        {
            // trim any whitespace
            data = strtrimspace(data);

            // trim quotes
            if (data.length() >= 2 && data[0] == '"' && data[data.Length - 1] == '"')  //if (data.length() >= 2 && data.front() == '"' && data.back() == '"')
            {
                data = data[1..];  //data.remove_prefix(1);
                data = data.Remove(data.Length - 1);  //data.remove_suffix(1);
            }

            return data;
        }
    }
}
