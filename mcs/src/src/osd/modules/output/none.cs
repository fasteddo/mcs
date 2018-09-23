// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    class output_none : output_module
    {
        //MODULE_DEFINITION(OUTPUT_NONE, output_none)
        static osd_module module_creator_output_none() { return new output_none(); }
        public static readonly module_type OUTPUT_NONE = MODULE_DEFINITION(module_creator_output_none);


        output_none() : base(output_module.OSD_OUTPUT_PROVIDER, "none") { } //, output_module()

        public override int init(osd_options options) { return 0; }
        public override void exit() { }

        // output_module
        public override void notify(string outname, int value) { }
    }
}
