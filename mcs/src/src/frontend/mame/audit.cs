// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using record_list = mame.std_list<mame.audit_record>;


namespace mame
{
    // ======================> audit_record
    // holds the result of auditing a single item
    class audit_record : simple_list_item<audit_record>
    {
        //friend class simple_list<audit_record>;


        // media types
        public enum media_type
        {
            MEDIA_ROM = 0,
            MEDIA_DISK,
            MEDIA_SAMPLE
        }


        // status values
        public enum audit_status
        {
            STATUS_GOOD = 0,
            STATUS_FOUND_INVALID,
            STATUS_NOT_FOUND,
            STATUS_ERROR
        }


        // substatus values
        public enum audit_substatus
        {
            SUBSTATUS_GOOD = 0,
            SUBSTATUS_GOOD_NEEDS_REDUMP,
            SUBSTATUS_FOUND_NODUMP,
            SUBSTATUS_FOUND_BAD_CHECKSUM,
            SUBSTATUS_FOUND_WRONG_LENGTH,
            SUBSTATUS_NOT_FOUND,
            SUBSTATUS_NOT_FOUND_NODUMP,
            SUBSTATUS_NOT_FOUND_OPTIONAL,
            SUBSTATUS_ERROR = 100
        }


        audit_record m_next;
        media_type m_type;                 /* type of item that was audited */
        audit_status m_status;               /* status of audit on this item */
        audit_substatus m_substatus;            /* finer-detail status */
        string m_name;                 /* name of item */
        UInt64 m_explength;            /* expected length of item */
        UInt64 m_length;               /* actual length of item */
        util.hash_collection m_exphashes = new util.hash_collection();            /* expected hash data */
        util.hash_collection m_hashes;               /* actual hash information */
        device_t m_shared_device;        /* device that shares the rom */


        // construction/destruction
        //-------------------------------------------------
        //  audit_record - constructor
        //-------------------------------------------------
        public audit_record(device_t device, int romOffset, rom_entry media, media_type type)
        {
            m_next = null;
            m_type = type;
            m_status = audit_status.STATUS_ERROR;
            m_substatus = audit_substatus.SUBSTATUS_ERROR;
            m_name = romload_global.ROM_GETNAME(media);
            m_explength = romload_global.rom_file_size(device, romOffset);
            m_length = 0;
            m_shared_device = null;


            m_exphashes.from_internal_string(romload_global.ROM_GETHASHDATA(media));
        }

        public audit_record(string name, media_type type)
        {
            m_next = null;
            m_type = type;
            m_status = audit_status.STATUS_ERROR;
            m_substatus = audit_substatus.SUBSTATUS_ERROR;
            m_name = name;
            m_explength = 0;
            m_length = 0;
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
        public UInt64 expected_length() { return m_explength; }
        public UInt64 actual_length() { return m_length; }
        public util.hash_collection expected_hashes() { return m_exphashes; }
        public util.hash_collection actual_hashes() { return m_hashes; }
        public device_t shared_device() { return m_shared_device; }


        // setters

        public void set_status(audit_status status, audit_substatus substatus)
        {
            m_status = status;
            m_substatus = substatus;
        }


        public void set_actual(util.hash_collection hashes, UInt64 length = 0)
        {
            m_hashes = hashes;
            m_length = length;
        }


        public void set_shared_device(device_t shared_device)
        {
            m_shared_device = shared_device;
        }
    }


    // ======================> media_auditor
    // class which manages auditing of items
    class media_auditor
    {
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
        record_list m_record_list = new record_list();
        driver_enumerator m_enumerator;
        string m_validation;
        string m_searchpath;


        // construction/destruction
        //-------------------------------------------------
        //  media_auditor - constructor
        //-------------------------------------------------
        public media_auditor(driver_enumerator enumerator)
        {
            m_enumerator = enumerator;
            m_validation = AUDIT_VALIDATE_FULL;
            m_searchpath = null;
        }


        // getters
        //const simple_list<audit_record> &records() const { return m_record_list; }


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

// temporary hack until romload is update: get the driver path and support it for
// all searches
string driverpath = m_enumerator.config().root_device().searchpath();

            int found = 0;
            int required = 0;
            int shared_found = 0;
            int shared_required = 0;

            // iterate over devices and regions
            foreach (device_t device in new device_iterator(m_enumerator.config().root_device()))
            {
                // determine the search path for this source and iterate through the regions
                m_searchpath = device.searchpath();

                // now iterate over regions and ROMs within
                int regionOffset = 0;
                for (rom_entry region = romload_global.rom_first_region(device, ref regionOffset); region != null; region = romload_global.rom_next_region(device, ref regionOffset))
                {
// temporary hack: add the driver path & region name
string combinedpath = device.searchpath() + ";" + driverpath;
if (!string.IsNullOrEmpty(device.shortname()))
    combinedpath += ";" + device.shortname();
m_searchpath = combinedpath;

                    int romOffset = regionOffset;
                    for (rom_entry rom = romload_global.rom_first_file(device, ref romOffset); rom != null; rom = romload_global.rom_next_file(device, ref romOffset))
                    {
                        string name = romload_global.ROM_GETNAME(rom);
                        util.hash_collection hashes = new util.hash_collection(romload_global.ROM_GETHASHDATA(rom));
                        device_t shared_device = find_shared_device(device, name, hashes, romload_global.ROM_GETLENGTH(rom));

                        // count the number of files with hashes
                        if (!hashes.flag(util.hash_collection.FLAG_NO_DUMP) && !romload_global.ROM_ISOPTIONAL(rom))
                        {
                            required++;
                            if (shared_device != null)
                                shared_required++;
                        }

                        // audit a file
                        audit_record record = null;
                        if (romload_global.ROMREGION_ISROMDATA(region))
                            record = audit_one_rom(device, romOffset, rom);

                        // audit a disk
                        else if (romload_global.ROMREGION_ISDISKDATA(region))
                            record = audit_one_disk(device, romOffset, rom);

                        if (record != null)
                        {
                            // count the number of files that are found.
                            if (record.status() == audit_record.audit_status.STATUS_GOOD || (record.status() == audit_record.audit_status.STATUS_FOUND_INVALID && find_shared_device(device, name, record.actual_hashes(), record.actual_length()) == null))
                            {
                                found++;
                                if (shared_device != null)
                                    shared_found++;
                            }

                            record.set_shared_device(shared_device);
                        }
                    }
                }
            }

            // if we only find files that are in the parent & either the set has no unique files or the parent is not found, then assume we don't have the set at all
            if (found == shared_found && required > 0 && (required != shared_required || shared_found == 0))
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
            m_searchpath = device.shortname();

            int found = 0;
            int required = 0;

            // now iterate over regions and ROMs within
            int regionOffset = 0;
            for (rom_entry region = romload_global.rom_first_region(device, ref regionOffset); region != null; region = romload_global.rom_next_region(device, ref regionOffset))
            {
                int romOffset = regionOffset;
                for (rom_entry rom = romload_global.rom_first_file(device, ref romOffset); rom != null; rom = romload_global.rom_next_file(device, ref romOffset))
                {
                    util.hash_collection hashes = new util.hash_collection(romload_global.ROM_GETHASHDATA(rom));

                    // count the number of files with hashes
                    if (!hashes.flag(util.hash_collection.FLAG_NO_DUMP) && !romload_global.ROM_ISOPTIONAL(rom))
                    {
                        required++;
                    }

                    // audit a file
                    audit_record record = null;
                    if (romload_global.ROMREGION_ISROMDATA(region))
                        record = audit_one_rom(device, romOffset, rom);
                    // audit a disk
                    else if (romload_global.ROMREGION_ISDISKDATA(region))
                        record = audit_one_disk(device, romOffset, rom);

                    // count the number of files that are found.
                    if (record != null && (record.status() == audit_record.audit_status.STATUS_GOOD || record.status() == audit_record.audit_status.STATUS_FOUND_INVALID))
                    {
                        found++;
                    }
                }
            }

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
        public summary audit_software(string list_name, software_info swinfo, string validation = AUDIT_VALIDATE_FULL)
        {
            // start fresh
            m_record_list.clear();

            // store validation for later
            m_validation = validation;

            string combinedpath = swinfo.shortname() + ";" + list_name + osdcore_global.PATH_SEPARATOR + swinfo.shortname();
            string locationtag = list_name + "%" + swinfo.shortname() + "%";
            if (swinfo.parentname() != null)
            {
                locationtag += swinfo.parentname();
                combinedpath += ";" + swinfo.parentname() + ";" + list_name + osdcore_global.PATH_SEPARATOR + swinfo.parentname();
            }
            m_searchpath = combinedpath;

            int found = 0;
            int required = 0;

            throw new emu_unimplemented();
#if false
            // now iterate over software parts
            foreach (software_part part in swinfo.parts())
                audit_regions(part.romdata().data(), locationtag.c_str(), found, required);
#endif


            if (found == 0 && required > 0)
            {
                m_record_list.clear();
                return summary.NOTFOUND;
            }

            // return a summary
            string unused = "";
            return summarize(list_name, ref unused);
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
            foreach (samples_device device in new samples_device_iterator(m_enumerator.config().root_device()))
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
                    audit_record record = m_record_list.emplace_back(new audit_record(samplename, audit_record.media_type.MEDIA_SAMPLE)).Value;  //audit_record &record = *m_record_list.emplace(m_record_list.end(), samplename, media_type::SAMPLE);

                    // look for the files
                    emu_file file = new emu_file(m_enumerator.options().sample_path(), osdcore_global.OPEN_FLAG_READ | osdcore_global.OPEN_FLAG_NO_PRELOAD);
                    path_iterator path = new path_iterator(searchpath);
                    string curpath;
                    while (path.next(out curpath, samplename))
                    {
                        // attempt to access the file (.flac) or (.wav)
                        osd_file.error filerr = file.open(curpath, ".flac");
                        if (filerr != osd_file.error.NONE)
                            filerr = file.open(curpath, ".wav");

                        if (filerr == osd_file.error.NONE)
                        {
                            record.set_status(audit_record.audit_status.STATUS_GOOD, audit_record.audit_substatus.SUBSTATUS_GOOD);
                            found++;
                        }
                        else
                        {
                            record.set_status(audit_record.audit_status.STATUS_NOT_FOUND, audit_record.audit_substatus.SUBSTATUS_NOT_FOUND);
                        }
                    }
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
                if (record.substatus() == audit_record.audit_substatus.SUBSTATUS_GOOD)
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
                    case audit_record.audit_substatus.SUBSTATUS_GOOD_NEEDS_REDUMP:
                        if (output != null) output += "NEEDS REDUMP\n";
                        best_new_status = summary.BEST_AVAILABLE;
                        break;

                    case audit_record.audit_substatus.SUBSTATUS_FOUND_NODUMP:
                        if (output != null) output += "NO GOOD DUMP KNOWN\n";
                        best_new_status = summary.BEST_AVAILABLE;
                        break;

                    case audit_record.audit_substatus.SUBSTATUS_FOUND_BAD_CHECKSUM:
                        if (output != null)
                        {
                            output += "INCORRECT CHECKSUM:\n";
                            output += string.Format("EXPECTED: {0}\n", record.expected_hashes().macro_string());
                            output += string.Format("   FOUND: {0}\n", record.actual_hashes().macro_string());
                        }
                        break;

                    case audit_record.audit_substatus.SUBSTATUS_FOUND_WRONG_LENGTH:
                        if (output != null) output += string.Format("INCORRECT LENGTH: {0} bytes\n", record.actual_length());  // %" I64FMT "d bytes\n
                        break;

                    case audit_record.audit_substatus.SUBSTATUS_NOT_FOUND:
                        if (output != null)
                        {
                            device_t shared_device = record.shared_device();
                            if (shared_device == null)
                                output += "NOT FOUND\n";
                            else
                                output += string.Format("NOT FOUND ({0})\n", shared_device.shortname());
                        }
                        break;

                    case audit_record.audit_substatus.SUBSTATUS_NOT_FOUND_NODUMP:
                        if (output != null) output += "NOT FOUND - NO GOOD DUMP KNOWN\n";
                        best_new_status = summary.BEST_AVAILABLE;
                        break;

                    case audit_record.audit_substatus.SUBSTATUS_NOT_FOUND_OPTIONAL:
                        if (output != null) output += "NOT FOUND BUT OPTIONAL\n";
                        best_new_status = summary.BEST_AVAILABLE;
                        break;

                    default:
                        global.assert(false);
                        break;
                }

                // downgrade the overall status if necessary
                overall_status = (summary)Math.Max((int)overall_status, (int)best_new_status);
            }

            return overall_status;
        }


        // internal helpers

        //-------------------------------------------------
        //  audit_one_rom - validate a single ROM entry
        //-------------------------------------------------
        audit_record audit_one_rom(device_t device, int romOffset, rom_entry rom)
        {
            // allocate and append a new record
            audit_record record = m_record_list.emplace_back(new audit_record(device, romOffset, rom, audit_record.media_type.MEDIA_ROM)).Value;  //audit_record &record = *m_record_list.emplace(m_record_list.end(), *rom, media_type::ROM);

            // see if we have a CRC and extract it if so
            UInt32 crc;
            bool has_crc = record.expected_hashes().crc(out crc);

            // find the file and checksum it, getting the file length along the way
            emu_file file = new emu_file(m_enumerator.options().media_path(), osdcore_global.OPEN_FLAG_READ | osdcore_global.OPEN_FLAG_NO_PRELOAD);
            file.set_restrict_to_mediapath(true);
            path_iterator path = new path_iterator(m_searchpath);
            string curpath;
            while (path.next(out curpath, record.name()))
            {
                // open the file if we can
                osd_file.error filerr;
                if (has_crc)
                    filerr = file.open(curpath, crc);
                else
                    filerr = file.open(curpath);

                // if it worked, get the actual length and hashes, then stop
                if (filerr == osd_file.error.NONE)
                {
                    record.set_actual(file.hashes(m_validation), file.size());
                    break;
                }
            }

            // compute the final status
            compute_status(record, rom, record.actual_length() != 0);
            return record;
        }


        //-------------------------------------------------
        //  audit_one_disk - validate a single disk entry
        //-------------------------------------------------
        audit_record audit_one_disk(device_t device, int romOffset, rom_entry rom, string locationtag = null)
        {
            // allocate and append a new record
            audit_record record = m_record_list.emplace_back(new audit_record(device, romOffset, rom, audit_record.media_type.MEDIA_DISK)).Value;  //audit_record &record = *m_record_list.emplace(m_record_list.end(), *rom, media_type::DISK);

            // open the disk
            chd_file source = new chd_file();
            chd_error err = (chd_error)romload_global.open_disk_image(m_enumerator.options(), m_enumerator.driver(), rom, source, locationtag);

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
            compute_status(record, rom, err == chd_error.CHDERR_NONE);
            return record;
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
                    record.set_status(audit_record.audit_status.STATUS_NOT_FOUND, audit_record.audit_substatus.SUBSTATUS_NOT_FOUND_NODUMP);

                // optional ROM
                else if (romload_global.ROM_ISOPTIONAL(rom))
                    record.set_status(audit_record.audit_status.STATUS_NOT_FOUND, audit_record.audit_substatus.SUBSTATUS_NOT_FOUND_OPTIONAL);

                // just plain old not found
                else
                    record.set_status(audit_record.audit_status.STATUS_NOT_FOUND, audit_record.audit_substatus.SUBSTATUS_NOT_FOUND);
            }

            // if found, provide more details
            else
            {
                // length mismatch
                if (record.expected_length() != record.actual_length())
                    record.set_status(audit_record.audit_status.STATUS_FOUND_INVALID, audit_record.audit_substatus.SUBSTATUS_FOUND_WRONG_LENGTH);

                // found but needs a dump
                else if (record.expected_hashes().flag(util.hash_collection.FLAG_NO_DUMP))
                    record.set_status(audit_record.audit_status.STATUS_GOOD, audit_record.audit_substatus.SUBSTATUS_FOUND_NODUMP);

                // incorrect hash
                else if (record.expected_hashes() != record.actual_hashes())
                    record.set_status(audit_record.audit_status.STATUS_FOUND_INVALID, audit_record.audit_substatus.SUBSTATUS_FOUND_BAD_CHECKSUM);

                // correct hash but needs a redump
                else if (record.expected_hashes().flag(util.hash_collection.FLAG_BAD_DUMP))
                    record.set_status(audit_record.audit_status.STATUS_GOOD, audit_record.audit_substatus.SUBSTATUS_GOOD_NEEDS_REDUMP);

                // just plain old good
                else
                    record.set_status(audit_record.audit_status.STATUS_GOOD, audit_record.audit_substatus.SUBSTATUS_GOOD);
            }
        }


        //-------------------------------------------------
        //  find_shared_device - return the source that
        //  shares a media entry with the same hashes
        //-------------------------------------------------
        device_t find_shared_device(device_t device, string name, util.hash_collection romhashes, UInt64 romlength)
        {
            bool dumped = !romhashes.flag(util.hash_collection.FLAG_NO_DUMP);

            // special case for non-root devices
            device_t highest_device = null;
            if (device.owner() != null)
            {
                int regionOffset = 0;
                for (rom_entry region = romload_global.rom_first_region(device, ref regionOffset); region != null; region = romload_global.rom_next_region(device, ref regionOffset))
                {
                    int romOffset = regionOffset;
                    for (rom_entry rom = romload_global.rom_first_file(device, ref romOffset); rom != null; rom = romload_global.rom_next_file(device, ref romOffset))
                    {
                        if (romload_global.ROM_GETLENGTH(rom) == romlength)
                        {
                            util.hash_collection hashes = new util.hash_collection(romload_global.ROM_GETHASHDATA(rom));
                            if ((dumped && hashes == romhashes) || (!dumped && romload_global.ROM_GETNAME(rom) == name))
                                highest_device = device;
                        }
                    }
                }
            }
            else
            {
                // iterate up the parent chain
                for (int drvindex = driver_enumerator.find(m_enumerator.driver().parent); drvindex != -1; drvindex = driver_enumerator.find(driver_enumerator.driver((UInt32)drvindex).parent))
                {
                    foreach (device_t scandevice in new device_iterator(m_enumerator.config((UInt32)drvindex).root_device()))
                    {
                        int regionOffset = 0;
                        for (rom_entry region = romload_global.rom_first_region(scandevice, ref regionOffset); region != null; region = romload_global.rom_next_region(scandevice, ref regionOffset))
                        {
                            int romOffset = regionOffset;
                            for (rom_entry rom = romload_global.rom_first_file(scandevice, ref romOffset); rom != null; rom = romload_global.rom_next_file(scandevice, ref romOffset))
                            {
                                if (romload_global.ROM_GETLENGTH(rom) == romlength)
                                {
                                    util.hash_collection hashes = new util.hash_collection(romload_global.ROM_GETHASHDATA(rom));
                                    if ((dumped && hashes == romhashes) || (!dumped && romload_global.ROM_GETNAME(rom) == name))
                                        highest_device = scandevice;
                                }
                            }
                        }
                    }
                }
            }

            return highest_device;
        }
    }
}
