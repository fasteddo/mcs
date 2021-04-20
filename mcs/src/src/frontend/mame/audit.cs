// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Linq;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using media_auditor_record_list = mame.std.list<mame.media_auditor.audit_record>;  //using record_list = std::list<audit_record>;
using samples_device_enumerator = mame.device_type_enumerator<mame.samples_device>;  //typedef device_type_enumerator<samples_device> samples_device_enumerator;
using size_t = System.UInt32;
using std_string = System.String;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame
{
    // ======================> media_auditor
    // class which manages auditing of items
    class media_auditor : global_object
    {
        //using record_list = std::list<audit_record>;


        // media types
        public enum media_type
        {
            ROM = 0,
            DISK,
            SAMPLE
        }


        // status values
        public enum audit_status
        {
            GOOD = 0,
            FOUND_INVALID,
            NOT_FOUND,
            UNVERIFIED = 100
        }


        // substatus values
        public enum audit_substatus
        {
            GOOD = 0,
            GOOD_NEEDS_REDUMP,
            FOUND_NODUMP,
            FOUND_BAD_CHECKSUM,
            FOUND_WRONG_LENGTH,
            NOT_FOUND,
            NOT_FOUND_NODUMP,
            NOT_FOUND_OPTIONAL,
            UNVERIFIED = 100
        }


        //enum summary


        // ======================> audit_record
        // holds the result of auditing a single item
        public class audit_record : simple_list_item<audit_record>
        {
            //friend class simple_list<audit_record>;


            audit_record m_next;
            media_type m_type;                 /* type of item that was audited */
            audit_status m_status;               /* status of audit on this item */
            audit_substatus m_substatus;            /* finer-detail status */
            string m_name;                 /* name of item */
            uint64_t m_explength;            /* expected length of item */
            uint64_t m_length;               /* actual length of item */
            util.hash_collection m_exphashes = new util.hash_collection();            /* expected hash data */
            util.hash_collection m_hashes;               /* actual hash information */
            device_type m_shared_device;        /* device that shares the rom */  //std::add_pointer_t<device_type> m_shared_device;    // device that shares the ROM


            // construction/destruction
            //-------------------------------------------------
            //  audit_record - constructor
            //-------------------------------------------------
            public audit_record(Pointer<rom_entry> media, media_type type)
            {
                m_next = null;
                m_type = type;
                m_status = audit_status.UNVERIFIED;
                m_substatus = audit_substatus.UNVERIFIED;
                m_name = media[0].name();
                m_explength = romload_global.rom_file_size(media);
                m_length = 0;
                m_exphashes = new util.hash_collection(media[0].hashdata());
                m_hashes = new util.hash_collection();
                m_shared_device = null;
            }

            public audit_record(string name, media_type type)
            {
                m_next = null;
                m_type = type;
                m_status = audit_status.UNVERIFIED;
                m_substatus = audit_substatus.UNVERIFIED;
                m_name = name;
                m_explength = 0;
                m_length = 0;
                m_exphashes = new util.hash_collection();
                m_hashes = new util.hash_collection();
                m_shared_device = null;
            }


            // getters
            public audit_record next() { return m_next; }
            public audit_record m_next_get() { return m_next; }
            public void m_next_set(audit_record value) { m_next = value; }

            //media_type type() const { return m_type; }
            public audit_status status() { return m_status; }
            public audit_substatus substatus() { return m_substatus; }
            public string name() { return m_name; }
            public uint64_t expected_length() { return m_explength; }
            public uint64_t actual_length() { return m_length; }
            public util.hash_collection expected_hashes() { return m_exphashes; }
            public util.hash_collection actual_hashes() { return m_hashes; }
            public device_type shared_device() { return m_shared_device; }


            // setters

            public void set_status(audit_status status, audit_substatus substatus)
            {
                m_status = status;
                m_substatus = substatus;
            }


            public void set_actual(util.hash_collection hashes, uint64_t length = 0)
            {
                m_hashes = hashes;
                m_length = length;
            }


            public void set_shared_device(device_type shared_device)
            {
                m_shared_device = shared_device;
            }
        }


        //using record_list = std::list<audit_record>;


        // hashes to use for validation
        public const string AUDIT_VALIDATE_FAST = "R";     /* CRC only */
        public const string AUDIT_VALIDATE_FULL = "RS";    /* CRC + SHA1 */


        // summary values
        public enum summary
        {
            CORRECT = 0,
            NONE_NEEDED,
            BEST_AVAILABLE,
            INCORRECT,
            NOTFOUND
        }


        // internal state
        media_auditor_record_list m_record_list = new media_auditor_record_list();
        driver_enumerator m_enumerator;
        string m_validation;


        // construction/destruction
        //-------------------------------------------------
        //  media_auditor - constructor
        //-------------------------------------------------
        public media_auditor(driver_enumerator enumerator)
        {
            m_enumerator = enumerator;
            m_validation = AUDIT_VALIDATE_FULL;
        }


        // getters
        public media_auditor_record_list records() { return m_record_list; }


        // audit operations

        //-------------------------------------------------
        //  audit_media - audit the media described by the
        //  currently-enumerated driver
        //-------------------------------------------------
        public summary audit_media(string validation = AUDIT_VALIDATE_FULL)
        {
            // start fresh
            m_record_list.clear();

            // store validation for later
            m_validation = validation;

            // first walk the parent chain for required ROMs
            parent_rom_vector parentroms = new parent_rom_vector();
            for (var drvindex = driver_list.find(m_enumerator.driver().parent); 0 <= drvindex; drvindex = driver_list.find(driver_list.driver(drvindex).parent))  //for (auto drvindex = m_enumerator.find(m_enumerator.driver().parent); 0 <= drvindex; drvindex = m_enumerator.find(m_enumerator.driver(drvindex).parent))
            {
                game_driver parent = driver_list.driver(drvindex);
                LOG(null, "Checking parent {0} for ROM files\n", parent.type.shortname());
                std.vector<rom_entry> roms = rom_build_entries(parent.rom);
                for (Pointer<rom_entry> region = romload_global.rom_first_region(new Pointer<rom_entry>(roms)); region != null; region = romload_global.rom_next_region(region))  //for (rom_entry const *region = rom_first_region(&roms.front()); region; region = rom_next_region(region))
                {
                    for (Pointer<rom_entry> rom = romload_global.rom_first_file(region); rom != null; rom = romload_global.rom_next_file(rom))  //for (rom_entry const *rom = rom_first_file(region); rom; rom = rom_next_file(rom))
                    {
                        LOG(null, "Adding parent ROM {0}\n", rom[0].name());
                        parentroms.emplace_back(new parent_rom(parent.type, rom));
                    }
                }
            }

            parentroms.remove_redundant_parents();

            // count ROMs required/found
            size_t found = 0;
            size_t required = 0;
            size_t shared_found = 0;
            size_t shared_required = 0;
            size_t parent_found = 0;

            // iterate over devices and regions
            std.vector<string> searchpath = new std.vector<string>();
            foreach (device_t device in new device_enumerator(m_enumerator.config().root_device()))
            {
                searchpath.clear();

                // now iterate over regions and ROMs within
                for (Pointer<rom_entry> region = romload_global.rom_first_region(device); region != null; region = romload_global.rom_next_region(region))
                {
                    for (Pointer<rom_entry> rom = romload_global.rom_first_file(region); rom != null; rom = romload_global.rom_next_file(rom))
                    {
                        if (searchpath.empty())
                        {
                            LOG(null, "Audit media for device {0}({1})\n", device.shortname(), device.tag());
                            searchpath = device.searchpath();
                        }

                        // look for a matching parent or device ROM
                        std_string name = rom[0].name();
                        util.hash_collection hashes = new util.hash_collection(rom[0].hashdata());
                        bool dumped = !hashes.flag(util.hash_collection.FLAG_NO_DUMP);
                        device_type shared_device = parentroms.find_shared_device(device, name, hashes, romload_global.rom_file_size(rom));
                        if (shared_device != null)
                            LOG(null, "File '{0}' {1}{2}dumped shared with {3}\n", name, romload_global.ROM_ISOPTIONAL(rom[0]) ? "optional " : "", dumped ? "" : "un", shared_device.shortname());
                        else
                            LOG(null, "File '{0}' {1}{2}dumped\n", name, romload_global.ROM_ISOPTIONAL(rom[0]) ? "optional " : "", dumped ? "" : "un");

                        // count the number of files with hashes
                        if (dumped && !romload_global.ROM_ISOPTIONAL(rom[0]))
                        {
                            required++;
                            if (shared_device != null)
                                shared_required++;
                        }

                        // audit a file
                        audit_record record = null;
                        if (romload_global.ROMREGION_ISROMDATA(region[0]))
                            record = audit_one_rom(searchpath, rom);

                        // audit a disk
                        else if (romload_global.ROMREGION_ISDISKDATA(region[0]))
                            record = audit_one_disk(rom, device);

                        if (record != null)
                        {
                            // see if the actual content found belongs to a parent
                            var matchesshared = parentroms.actual_matches_shared(device, record);
                            if (matchesshared.first != null)
                                LOG(null, "Actual ROM file shared with {0}parent {1}\n", matchesshared.second ? "immediate " : "", matchesshared.first.shortname());

                            // count the number of files that are found.
                            if ((record.status() == audit_status.GOOD) || ((record.status() == audit_status.FOUND_INVALID) && (matchesshared.first == null)))
                            {
                                found++;
                                if (shared_device != null)
                                    shared_found++;
                                if (matchesshared.second)
                                    parent_found++;
                            }

                            record.set_shared_device(shared_device);
                        }
                    }
                }
            }

            if (!searchpath.empty())
                LOG(null, "Total required={0} (shared={1}) found={2} (shared={3} parent={4})\n", required, shared_required, found, shared_found, parent_found);

            // if we only find files that are in the parent & either the set has no unique files or the parent is not found, then assume we don't have the set at all
            if ((found == shared_found) && required != 0 && ((required != shared_required) || parent_found == 0))
            {
                m_record_list.clear();
                return summary.NOTFOUND;
            }

            // return a summary
            string unused = "";
            return summarize(m_enumerator.driver().name, ref unused);
        }


        //-------------------------------------------------
        //  audit_device - audit the device
        //-------------------------------------------------
        public summary audit_device(device_t device, string validation = AUDIT_VALIDATE_FULL)
        {
            // start fresh
            m_record_list.clear();

            // store validation for later
            m_validation = validation;

            size_t found = 0;
            size_t required = 0;

            std.vector<string> searchpath = new std.vector<string>();
            audit_regions(
                    (rom_entry region, Pointer<rom_entry> rom) =>  //[this, &device, &searchpath] (rom_entry const *region, rom_entry const *rom) -> audit_record const *
                    {
                        if (ROMREGION_ISROMDATA(region))
                        {
                            if (searchpath.empty())
                                searchpath = device.searchpath();

                            return audit_one_rom(searchpath, rom);
                        }
                        else if (ROMREGION_ISDISKDATA(region))
                        {
                            return audit_one_disk(rom, device);
                        }
                        else
                        {
                            return null;
                        }
                    },
                    romload_global.rom_first_region(device)[0],
                    out found,
                    out required);

            if (found == 0 && required > 0)
            {
                m_record_list.clear();
                return summary.NOTFOUND;
            }

            // return a summary
            string unused = "";
            return summarize(device.shortname(), ref unused);
        }


        //-------------------------------------------------
        //  audit_software
        //-------------------------------------------------
        public summary audit_software(software_list_device swlist, software_info swinfo, string validation = AUDIT_VALIDATE_FULL)
        {
            // start fresh
            m_record_list.clear();

            // store validation for later
            m_validation = validation;

            int found = 0;
            int required = 0;

            throw new emu_unimplemented();
#if false
            // now iterate over software parts
            std::vector<std::string> searchpath;
            auto const do_audit =
                    [this, &swlist, &swinfo, &searchpath] (rom_entry const *region, rom_entry const *rom) -> audit_record const *
                    {
                        if (ROMREGION_ISROMDATA(region))
                        {
                            if (searchpath.empty())
                                searchpath = rom_load_manager::get_software_searchpath(swlist, swinfo);
                            return &audit_one_rom(searchpath, rom);
                        }
                        else if (ROMREGION_ISDISKDATA(region))
                        {
                            return &audit_one_disk(rom, swlist, swinfo);
                        }
                        else
                        {
                            return nullptr;
                        }
                    };
            for (const software_part &part : swinfo.parts())
                audit_regions(do_audit, part.romdata().data(), found, required);

            if (found == 0 && required > 0)
            {
                m_record_list.clear();
                return summary.NOTFOUND;
            }

            // return a summary
            return summarize(swlist.list_name().c_str());
#endif
        }


        //-------------------------------------------------
        //  audit_samples - validate the samples for the
        //  currently-enumerated driver
        //-------------------------------------------------
        public summary audit_samples()
        {
            // start fresh
            m_record_list.clear();

            int required = 0;
            int found = 0;

            // iterate over sample entries
            foreach (samples_device device in new samples_device_enumerator(m_enumerator.config().root_device()))
            {
                // by default we just search using the driver name
                string searchpath = m_enumerator.driver().name;

                // add the alternate path if present
                samples_iterator samplesiter = new samples_iterator(device);
                if (samplesiter.altbasename() != null)
                    searchpath += ";" + samplesiter.altbasename();

                // iterate over samples in this entry
                for (string samplename = samplesiter.first(); samplename != null; samplename = samplesiter.next())
                {
                    required++;

                    // create a new record
                    audit_record record = m_record_list.emplace_back(new audit_record(samplename, media_type.SAMPLE)).Value;  //audit_record &record = *m_record_list.emplace(m_record_list.end(), samplename, media_type::SAMPLE);

                    // look for the files
                    emu_file file = new emu_file(m_enumerator.options().sample_path(), osdfile_global.OPEN_FLAG_READ | osdfile_global.OPEN_FLAG_NO_PRELOAD);
                    path_iterator path = new path_iterator(searchpath);
                    string curpath;
                    while (path.next(out curpath, samplename))
                    {
                        // attempt to access the file (.flac) or (.wav)
                        osd_file.error filerr = file.open(curpath + ".flac");
                        if (filerr != osd_file.error.NONE)
                            filerr = file.open(curpath + ".wav");

                        if (filerr == osd_file.error.NONE)
                        {
                            record.set_status(audit_status.GOOD, audit_substatus.GOOD);
                            found++;
                        }
                        else
                        {
                            record.set_status(audit_status.NOT_FOUND, audit_substatus.NOT_FOUND);
                        }
                    }

                    file.close();
                }
            }

            if (found == 0 && required > 0)
            {
                m_record_list.clear();
                return summary.NOTFOUND;
            }

            // return a summary
            string unused = "";
            return summarize(m_enumerator.driver().name, ref unused);
        }


        //-------------------------------------------------
        //  summary - generate a summary, with an optional
        //  string format
        //-------------------------------------------------
        public summary summarize(string name, ref string output)
        {
            if (m_record_list.empty())
            {
                return summary.NONE_NEEDED;
            }

            // loop over records
            summary overall_status = summary.CORRECT;
            foreach (audit_record record in m_record_list)
            {
                summary best_new_status = summary.INCORRECT;

                // skip anything that's fine
                if (record.substatus() == audit_substatus.GOOD)
                    continue;

                // output the game name, file name, and length (if applicable)
                if (output != null)
                {
                    if (name != null)
                        output += string.Format("{0}: {1}", name, record.name());  // %-12s: %s
                    else
                        output += string.Format("{0}", record.name());
                    if (record.expected_length() > 0)
                        output += string.Format(" ({0} bytes)", record.expected_length());  //  (%" I64FMT "d bytes)
                    output += " - ";
                }

                // use the substatus for finer details
                switch (record.substatus())
                {
                    case audit_substatus.GOOD_NEEDS_REDUMP:
                        if (output != null) output += "NEEDS REDUMP\n";
                        best_new_status = summary.BEST_AVAILABLE;
                        break;

                    case audit_substatus.FOUND_NODUMP:
                        if (output != null) output += "NO GOOD DUMP KNOWN\n";
                        best_new_status = summary.BEST_AVAILABLE;
                        break;

                    case audit_substatus.FOUND_BAD_CHECKSUM:
                        if (output != null)
                        {
                            output += "INCORRECT CHECKSUM:\n";
                            output += string.Format("EXPECTED: {0}\n", record.expected_hashes().macro_string());
                            output += string.Format("   FOUND: {0}\n", record.actual_hashes().macro_string());
                        }
                        break;

                    case audit_substatus.FOUND_WRONG_LENGTH:
                        if (output != null) output += string.Format("INCORRECT LENGTH: {0} bytes\n", record.actual_length());  // %" I64FMT "d bytes\n
                        break;

                    case audit_substatus.NOT_FOUND:
                        if (output != null)
                        {
                            device_type shared_device = record.shared_device();
                            if (shared_device == null)
                                output += "NOT FOUND\n";
                            else
                                output += string.Format("NOT FOUND ({0})\n", shared_device.shortname());
                        }
                        break;

                    case audit_substatus.NOT_FOUND_NODUMP:
                        if (output != null) output += "NOT FOUND - NO GOOD DUMP KNOWN\n";
                        best_new_status = summary.BEST_AVAILABLE;
                        break;

                    case audit_substatus.NOT_FOUND_OPTIONAL:
                        if (output != null) output += "NOT FOUND BUT OPTIONAL\n";
                        best_new_status = summary.BEST_AVAILABLE;
                        break;

                    default:
                        assert(false);
                        break;
                }

                // downgrade the overall status if necessary
                overall_status = (summary)Math.Max((int)overall_status, (int)best_new_status);
            }

            return overall_status;
        }


        // internal helpers

        void audit_regions(Func<rom_entry, Pointer<rom_entry>, audit_record> do_audit, rom_entry region, out size_t found, out size_t required)  //template <typename T> void audit_regions(T do_audit, const rom_entry *region, std::size_t &found, std::size_t &required);
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  audit_one_rom - validate a single ROM entry
        //-------------------------------------------------
        audit_record audit_one_rom(std.vector<string> searchpath, Pointer<rom_entry> rom)  //audit_record &audit_one_rom(const std::vector<std::string> &searchpath, const rom_entry *rom);
        {
            // allocate and append a new record
            audit_record record = m_record_list.emplace_back(new audit_record(rom, media_type.ROM)).Value;  //audit_record &record = *m_record_list.emplace(m_record_list.end(), *rom, media_type::ROM);

            // see if we have a CRC and extract it if so
            uint32_t crc;
            bool has_crc = record.expected_hashes().crc(out crc);

            // find the file and checksum it, getting the file length along the way
            emu_file file = new emu_file(m_enumerator.options().media_path(), searchpath, OPEN_FLAG_READ | OPEN_FLAG_NO_PRELOAD);
            file.set_restrict_to_mediapath(1);

            // open the file if we can
            osd_file.error filerr;
            if (has_crc)
                filerr = file.open(record.name(), crc);
            else
                filerr = file.open(record.name());

            // if it worked, get the actual length and hashes, then stop
            if (filerr == osd_file.error.NONE)
                record.set_actual(file.hashes(m_validation), file.size());

            file.close();

            // compute the final status
            compute_status(record, rom[0], record.actual_length() != 0);
            return record;
        }


        //-------------------------------------------------
        //  audit_one_disk - validate a single disk entry
        //-------------------------------------------------
        audit_record audit_one_disk(Pointer<rom_entry> rom, object args)  //template <typename... T> audit_record &audit_one_disk(const rom_entry *rom, T &&... args);
        {
            // allocate and append a new record
            audit_record record = m_record_list.emplace_back(new audit_record(rom, media_type.DISK)).Value;  //audit_record &record = *m_record_list.emplace(m_record_list.end(), *rom, media_type::DISK);

            // open the disk
            chd_file source = new chd_file();

            throw new emu_unimplemented();
#if false
            chd_error err = rom_load_manager.open_disk_image(m_enumerator.options(), args, rom[0], source);

            // if we succeeded, get the hashes
            if (err == chd_error.CHDERR_NONE)
            {
                util.hash_collection hashes = new util.hash_collection();

                // if there's a SHA1 hash, add them to the output hash
                if (source.sha1() != null)
                    hashes.add_sha1(source.sha1());

                // update the actual values
                record.set_actual(hashes);
            }

            // compute the final status
            compute_status(record, rom[0], err == chd_error.CHDERR_NONE);
            return record;
#endif
        }


        //-------------------------------------------------
        //  compute_status - compute a detailed status
        //  based on the information we have
        //-------------------------------------------------
        void compute_status(audit_record record, rom_entry rom, bool found)
        {
            // if not found, provide more details
            if (!found)
            {
                // no good dump
                if (record.expected_hashes().flag(util.hash_collection.FLAG_NO_DUMP))
                    record.set_status(audit_status.NOT_FOUND, audit_substatus.NOT_FOUND_NODUMP);

                // optional ROM
                else if (romload_global.ROM_ISOPTIONAL(rom))
                    record.set_status(audit_status.NOT_FOUND, audit_substatus.NOT_FOUND_OPTIONAL);

                // just plain old not found
                else
                    record.set_status(audit_status.NOT_FOUND, audit_substatus.NOT_FOUND);
            }

            // if found, provide more details
            else
            {
                // length mismatch
                if (record.expected_length() != record.actual_length())
                    record.set_status(audit_status.FOUND_INVALID, audit_substatus.FOUND_WRONG_LENGTH);

                // found but needs a dump
                else if (record.expected_hashes().flag(util.hash_collection.FLAG_NO_DUMP))
                    record.set_status(audit_status.GOOD, audit_substatus.FOUND_NODUMP);

                // incorrect hash
                else if (record.expected_hashes() != record.actual_hashes())
                    record.set_status(audit_status.FOUND_INVALID, audit_substatus.FOUND_BAD_CHECKSUM);

                // correct hash but needs a redump
                else if (record.expected_hashes().flag(util.hash_collection.FLAG_BAD_DUMP))
                    record.set_status(audit_status.GOOD, audit_substatus.GOOD_NEEDS_REDUMP);

                // just plain old good
                else
                    record.set_status(audit_status.GOOD, audit_substatus.GOOD);
            }
        }
    }


    class parent_rom
    {
        public device_type type;  //std::reference_wrapper<std::remove_reference_t<device_type> >   type;
        public string name;
        public util.hash_collection hashes;
        public uint64_t length;


        public parent_rom(device_type t, Pointer<rom_entry> r)
        {
            throw new emu_unimplemented();
#if false
            type(t), name(r->name()), hashes(r->hashdata()), length(rom_file_size(r)) { }
#endif
        }
    }


    class parent_rom_vector : std.vector<parent_rom>
    {
        //using std::vector<parent_rom>::vector;


        public void remove_redundant_parents()
        {
            while (!empty())
            {
                // find where the next parent starts
                //auto const last(
                //        std::find_if(
                //            std::next(cbegin()),
                //            cend(),
                //            [this] (parent_rom const &r) { return &front().type.get() != &r.type.get(); }));
                var last = this.FindIndex(1, (parent_rom r) => { return front().type != r.type; });

                // examine dumped ROMs in this generation
                for (var iIdx = 0; last != iIdx; ++iIdx)  //for (auto i = cbegin(); last != i; ++i)
                {
                    var i = this[iIdx];

                    if (!i.hashes.flag(util.hash_collection.FLAG_NO_DUMP))
                    {
                        //auto const match(
                        //        std::find_if(
                        //            last,
                        //            cend(),
                        //            [&i] (parent_rom const &r) { return (i->length == r.length) && (i->hashes == r.hashes); }));
                        var match = this.FindIndex(last, (parent_rom r) => { return (i.length == r.length) && (i.hashes == r.hashes); });
                        if (-1 == match)  //if (cend() == match)
                            return;
                    }
                }

                RemoveRange(0, last);  //erase(cbegin(), last);
            }
        }


        public device_type find_shared_device(device_t current, string name, util.hash_collection hashes, uint64_t length)  //std::add_pointer_t<device_type> find_shared_device(device_t &current, char const *name, util::hash_collection const &hashes, uint64_t length) const
        {
            // if we're examining a child device, it will always have a perfect match
            if (current.owner() != null)
                return current.type();

            // scan backwards through parents for a matching definition
            bool dumped = !hashes.flag(util.hash_collection.FLAG_NO_DUMP);
            device_type best = null;
            foreach (var it in this.Reverse())  //for (const_reverse_iterator it = crbegin(); crend() != it; ++it)
            {
                if (it.length == length)
                {
                    if (dumped)
                    {
                        if (it.hashes == hashes)
                            return it.type;
                    }
                    else if (it.name == name)
                    {
                        if (it.hashes.flag(util.hash_collection.FLAG_NO_DUMP))
                            return it.type;
                        else if (best == null)
                            best = it.type;
                    }
                }
            }

            return best;
        }


        public std.pair<device_type, bool> actual_matches_shared(device_t current, media_auditor.audit_record record)  //std::pair<std::add_pointer_t<device_type>, bool> actual_matches_shared(device_t &current, media_auditor::audit_record const &record)
        {
            // no result if no matching file was found
            if ((record.status() != media_auditor.audit_status.GOOD) && (record.status() != media_auditor.audit_status.FOUND_INVALID))
                return std.make_pair((device_type)null, false);

            // if we're examining a child device, scan it first
            bool matches_device_undumped = false;
            if (current.owner() != null)
            {
                for (Pointer<rom_entry> region = romload_global.rom_first_region(current); region != null; region = romload_global.rom_next_region(region))
                {
                    for (Pointer<rom_entry> rom = romload_global.rom_first_file(region); rom != null; rom = romload_global.rom_next_file(rom))
                    {
                        if (romload_global.rom_file_size(rom) == record.actual_length())
                        {
                            util.hash_collection hashes = new util.hash_collection(rom[0].hashdata());
                            if (hashes == record.actual_hashes())
                                return std.make_pair(current.type(), empty());
                            else if (hashes.flag(util.hash_collection.FLAG_NO_DUMP) && (rom[0].name() == record.name()))
                                matches_device_undumped = true;
                        }
                    }
                }
            }

            // look for a matching parent ROM
            device_type closest_bad = null;
            foreach (var it in this.Reverse())  //for (const_reverse_iterator it = crbegin(); crend() != it; ++it)
            {
                if (it.length == record.actual_length())
                {
                    if (it.hashes == record.actual_hashes())
                        return std.make_pair(it.type, it.type == this[0].type);
                    else if (it.hashes.flag(util.hash_collection.FLAG_NO_DUMP) && (it.name == record.name()))
                        closest_bad = it.type;
                }
            }

            // fall back to the nearest bad dump
            if (closest_bad != null)
                return std.make_pair(closest_bad, this[0].type == closest_bad);
            else if (matches_device_undumped)
                return std.make_pair(current.type(), empty());
            else
                return std.make_pair((device_type)null, false);
        }
    }
}
