// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    // helper class to putput
    class info_xml_creator
    {
        //static const char s_dtd_string[];


        // internal state
        //FILE *                  m_output;
        //driver_enumerator &     m_drivlist;
        //emu_options             m_lookup_options;


        // construction/destruction
        //-------------------------------------------------
        //  info_xml_creator - constructor
        //-------------------------------------------------
        public info_xml_creator(emu_options options, bool dtd = true)
        {
            throw new emu_unimplemented();
        }


        // output
        //-------------------------------------------------
        //  output_mame_xml - print the XML information
        //  for all known games
        //-------------------------------------------------
        public void output(object output, std.vector<string> patterns)  //FILE *out)
        {
            throw new emu_unimplemented();
        }


        // internal helper
        //void output_one();
        //void output_sampleof();
        //void output_bios();
        //void output_rom(device_t &device);
        //void output_device_roms();
        //void output_sample(device_t &device);
        //void output_chips(device_t &device, const char *root_tag);
        //void output_display(device_t &device, const char *root_tag);
        //void output_sound(device_t &device);
        //void output_ioport_condition(const ioport_condition &condition, unsigned indent);
        //void output_input(const ioport_list &portlist);
        //void output_switches(const ioport_list &portlist, const char *root_tag, int type, const char *outertag, const char *innertag);
        //void output_ports(const ioport_list &portlist);
        //void output_adjusters(const ioport_list &portlist);
        //void output_driver();
        //void output_images(device_t &device, const char *root_tag);
        //void output_slots(device_t &device, const char *root_tag);
        //void output_software_list();
        //void output_ramoptions();


        //void output_one_device(device_t &device, const char *devtag);
        //void output_devices();


        //const char *get_merge_name(const hash_collection &romhashes);
    }
}
