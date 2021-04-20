// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_timer_id = System.UInt32;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    /***************************************************************************
        atarimo.h
        Common motion object management functions for Atari raster games.
    ***************************************************************************/

    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************
    // description of the motion objects
    public class atari_motion_objects_config
    {
        public class entry
        {
            public uint16_t [] data;
            public entry(uint16_t param0) : this(param0, 0, 0, 0) { }
            public entry(uint16_t param0, uint16_t param1, uint16_t param2, uint16_t param3) { data = new uint16_t[4] { param0, param1, param2, param3 }; }
        }

        public class dual_entry
        {
            public uint16_t [] data_lower;
            public uint16_t [] data_upper;
            public dual_entry(uint16_t paraml0, uint16_t paraml1, uint16_t paraml2, uint16_t paraml3) : this(paraml0, paraml1, paraml2, paraml3, 0, 0, 0, 0) { }
            public dual_entry(uint16_t paraml0, uint16_t paraml1, uint16_t paraml2, uint16_t paraml3, uint16_t paramu0, uint16_t paramu1, uint16_t paramu2, uint16_t paramu3)
            { data_lower = new uint16_t[4] { paraml0, paraml1, paraml2, paraml3 };
              data_upper = new uint16_t[4] { paramu0, paramu1, paramu2, paramu3 }; }
        }

        public uint8_t m_gfxindex;           // index to which gfx system
        public uint8_t m_bankcount;          // number of motion object banks
        public bool m_linked;             // are the entries linked?
        public bool m_split;              // are the entries split?
        public bool m_reverse;            // render in reverse order?
        public bool m_swapxy;             // render in swapped X/Y order?
        public bool m_nextneighbor;       // does the neighbor bit affect the next object?
        public uint16_t m_slipheight;         // pixels per SLIP entry (0 for no-slip)
        public uint8_t m_slipoffset;         // pixel offset for SLIPs
        public uint16_t m_maxperline;         // maximum number of links to visit/scanline (0=all)

        public uint16_t m_palettebase;        // base palette entry
        uint16_t m_maxcolors;          // maximum number of colors (remove me)
        public uint8_t m_transpen;           // transparent pen index

        public entry m_link_entry;           // mask for the link
        public dual_entry m_code_entry;           // mask for the code index
        public dual_entry m_color_entry;          // mask for the color/priority
        public entry m_xpos_entry;           // mask for the X position
        public entry m_ypos_entry;           // mask for the Y position
        public entry m_width_entry;          // mask for the width, in tiles*/
        public entry m_height_entry;         // mask for the height, in tiles
        public entry m_hflip_entry;          // mask for the horizontal flip
        public entry m_vflip_entry;          // mask for the vertical flip
        public entry m_priority_entry;       // mask for the priority
        public entry m_neighbor_entry;       // mask for the neighbor
        public entry m_absolute_entry;       // mask for absolute coordinates
        entry m_special_entry;        // mask for the special value
        public uint16_t m_specialvalue;         // resulting value to indicate "special"


        public atari_motion_objects_config
        (
            uint8_t gfxindex,
            uint8_t bankcount,
            bool linked,
            bool split,
            bool reverse,
            bool swapxy,
            bool nextneighbor,
            uint16_t slipheight,
            uint8_t slipoffset,
            uint16_t maxperline,

            uint16_t palettebase,
            uint16_t maxcolors,
            uint8_t transpen,

            entry link_entry,
            dual_entry code_entry,
            dual_entry color_entry,
            entry xpos_entry,
            entry ypos_entry,
            entry width_entry,
            entry height_entry,
            entry hflip_entry,
            entry vflip_entry,
            entry priority_entry,
            entry neighbor_entry,
            entry absolute_entry,
            entry special_entry,
            uint16_t specialvalue
        )
        {
            this.m_gfxindex = gfxindex;
            this.m_bankcount = bankcount;
            this.m_linked = linked;
            this.m_split = split;
            this.m_reverse = reverse;
            this.m_swapxy = swapxy;
            this.m_nextneighbor = nextneighbor;
            this.m_slipheight = slipheight;
            this.m_slipoffset = slipoffset;
            this.m_maxperline = maxperline;

            this.m_palettebase = palettebase;
            this.m_maxcolors = maxcolors;
            this.m_transpen = transpen;

            this.m_link_entry = link_entry;
            this.m_code_entry = code_entry;
            this.m_color_entry = color_entry;
            this.m_xpos_entry = xpos_entry;
            this.m_ypos_entry = ypos_entry;
            this.m_width_entry = width_entry;
            this.m_height_entry = height_entry;
            this.m_hflip_entry = hflip_entry;
            this.m_vflip_entry = vflip_entry;
            this.m_priority_entry = priority_entry;
            this.m_neighbor_entry = neighbor_entry;
            this.m_absolute_entry = absolute_entry;
            this.m_special_entry = special_entry;
            this.m_specialvalue = specialvalue;
        }
    }


    // ======================> atari_motion_objects_device
    public class atari_motion_objects_device : sprite16_device_ind16
                                               //device_video_interface,
                                               //atari_motion_objects_config
    {
        // device type definition
        //DEFINE_DEVICE_TYPE(ATARI_MOTION_OBJECTS, atari_motion_objects_device, "atarimo", "Atari Motion Objects")
        static device_t device_creator_atari_motion_objects_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new atari_motion_objects_device(mconfig, tag, owner, clock); }
        public static readonly device_type ATARI_MOTION_OBJECTS = DEFINE_DEVICE_TYPE(device_creator_atari_motion_objects_device, "atarimo", "Atari Motion Objects");


        // timer IDs
        //enum
        //{
        const device_timer_id TID_FORCE_UPDATE = 0;
        //}


        // a sprite parameter, which is a word index + shift + mask
        class sprite_parameter
        {
            uint16_t              m_word;             // word index
            uint16_t              m_shift;            // shift amount
            uint16_t              m_mask;             // final mask


            public sprite_parameter()
            {
                m_word = 0;
                m_shift = 0;
                m_mask = 0;
            }


            public bool set(atari_motion_objects_config.entry input) { return set(input.data); }

            //-------------------------------------------------
            //  set: Sets the mask via an input 4-word mask.
            //-------------------------------------------------
            public bool set(uint16_t [] input)
            {
                // determine the word and make sure it's only 1
                m_word = 0xffff;
                for (uint16_t i = 0; i < 4; i++)
                {
                    if (input[i] != 0)
                    {
                        if (m_word == 0xffff)
                            m_word = i;
                        else
                            return false;
                    }
                }

                // if all-zero, it's valid
                if (m_word == 0xffff)
                {
                    m_word = m_shift = m_mask = 0;
                    return true;
                }

                // determine the shift and final mask
                m_shift = 0;
                uint16_t temp = input[m_word];
                while ((temp & 1) == 0)
                {
                    m_shift++;
                    temp >>= 1;
                }

                m_mask = temp;
                return true;
            }


            public uint16_t extract(Pointer<uint16_t> data) { return (uint16_t)(((uint32_t)data[m_word] >> m_shift) & m_mask); }  //uint16_t extract(const uint16_t *data) const { return (data[m_word] >> m_shift) & m_mask; }
            public uint16_t shift() { return m_shift; }
            public uint16_t mask() { return m_mask; }
        }


        // a sprite parameter, which is a word index + shift + mask
        class dual_sprite_parameter
        {
            sprite_parameter    m_lower = new sprite_parameter();            // lower parameter
            sprite_parameter    m_upper = new sprite_parameter();            // upper parameter
            uint16_t            m_uppershift;       // upper shift


            public dual_sprite_parameter()
            {
                m_uppershift = 0;
            }


            //-------------------------------------------------
            //  set: Sets the mask via an input 4-word mask.
            //-------------------------------------------------
            public bool set(atari_motion_objects_config.dual_entry input)
            {
                // convert the lower and upper parts
                if (!m_lower.set(input.data_lower))
                    return false;
                if (!m_upper.set(input.data_upper))
                    return false;

                // determine the upper shift amount
                uint16_t temp = m_lower.mask();
                m_uppershift = 0;
                while (temp != 0)
                {
                    m_uppershift++;
                    temp >>= 1;
                }

                return true;
            }


            public uint32_t extract(Pointer<uint16_t> data) { return (uint32_t)m_lower.extract(data) | (uint32_t)(m_upper.extract(data) << m_uppershift); }  //uint32_t extract(const uint16_t *data) const { return m_lower.extract(data) | (m_upper.extract(data) << m_uppershift); }
            public uint32_t mask() { return (uint32_t)m_lower.mask() | ((uint32_t)m_upper.mask() << m_uppershift); }
        }


        const int MAX_PER_BANK = 1024;

        // constants
        public const int PRIORITY_SHIFT = 12;
        const uint16_t PRIORITY_MASK = (0xffff << PRIORITY_SHIFT) & 0xffff;
        public const uint16_t DATA_MASK = PRIORITY_MASK ^ 0xffff;


        device_video_interface m_divideo;
        atari_motion_objects_config m_atari_motion_objects_config;


        // parameter masks
        sprite_parameter        m_linkmask = new sprite_parameter();         // mask for the link
        //sprite_parameter        m_gfxmask;          // mask for the graphics bank
        dual_sprite_parameter   m_codemask = new dual_sprite_parameter();         // mask for the code index
        dual_sprite_parameter   m_colormask = new dual_sprite_parameter();        // mask for the color
        sprite_parameter        m_xposmask = new sprite_parameter();         // mask for the X position
        sprite_parameter        m_yposmask = new sprite_parameter();         // mask for the Y position
        sprite_parameter        m_widthmask = new sprite_parameter();        // mask for the width, in tiles*/
        sprite_parameter        m_heightmask = new sprite_parameter();       // mask for the height, in tiles
        sprite_parameter        m_hflipmask = new sprite_parameter();        // mask for the horizontal flip
        sprite_parameter        m_vflipmask = new sprite_parameter();        // mask for the vertical flip
        sprite_parameter        m_prioritymask = new sprite_parameter();     // mask for the priority
        sprite_parameter        m_neighbormask = new sprite_parameter();     // mask for the neighbor
        sprite_parameter        m_absolutemask = new sprite_parameter();     // mask for absolute coordinates
        sprite_parameter        m_specialmask = new sprite_parameter();      // mask for the special value

        // derived tile information
        int m_tilewidth;          // width of non-rotated tile
        int m_tileheight;         // height of non-rotated tile
        int m_tilexshift;         // bits to shift X coordinate when drawing
        int m_tileyshift;         // bits to shift Y coordinate when drawing

        // derived bitmap information
        int m_bitmapwidth;        // width of the full playfield bitmap
        int m_bitmapheight;       // height of the full playfield bitmap
        int m_bitmapxmask;        // x coordinate mask for the playfield bitmap
        int m_bitmapymask;        // y coordinate mask for the playfield bitmap

        // derived sprite information
        int m_entrycount;         // number of entries per bank
        int m_entrybits;          // number of bits needed to represent entrycount
        int m_spriterammask;      // combined mask when accessing sprite RAM with raw addresses
        int m_spriteramsize;      // total size of sprite RAM, in entries
        int m_slipshift;          // log2(pixels_per_SLIP)
        int m_sliprammask;        // combined mask when accessing SLIP RAM with raw addresses
        int m_slipramsize;        // total size of SLIP RAM, in entries

        // live state
        emu_timer m_force_update_timer;   // timer for forced updating
        uint32_t m_bank;               // current bank number
        uint32_t m_xscroll;            // xscroll offset
        uint32_t m_yscroll;            // yscroll offset

        // arrays
        optional_shared_ptr_uint16_t m_slipram;    // pointer to the SLIP RAM
        std.vector<uint32_t> m_codelookup = new std.vector<uint32_t>();       // lookup table for codes
        std.vector<uint8_t> m_colorlookup = new std.vector<uint8_t>();       // lookup table for colors
        std.vector<uint8_t> m_gfxlookup = new std.vector<uint8_t>();         // lookup table for graphics

        MemoryContainer<uint16_t> m_activelist = new MemoryContainer<uint16_t>(MAX_PER_BANK * 4, true);  //uint16_t                  m_activelist[MAX_PER_BANK*4]; // active list
        Pointer<uint16_t> m_activelast;  //uint16_t *                m_activelast;           // last entry in the active list

        uint32_t m_last_xpos;          // (during processing) the previous X position
        uint32_t m_next_xpos;          // (during processing) the next X position
        required_device<gfxdecode_device> m_gfxdecode;


        // construction/destruction
        //template <typename T>
        atari_motion_objects_device(machine_config mconfig, string tag, device_t owner, uint32_t clock, object screen_tag, atari_motion_objects_config config)
            : this(mconfig, tag, owner, clock)
        {
            if (screen_tag is string)
                m_divideo.set_screen((string)screen_tag);
            else if (screen_tag is device_finder<screen_device>)
                m_divideo.set_screen((device_finder<screen_device>)screen_tag);
            else
                throw new emu_unimplemented();

            set_config(config);
        }


        atari_motion_objects_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, ATARI_MOTION_OBJECTS, tag, owner)
        { 
            m_divideo = new device_video_interface(mconfig, this);  //, device_video_interface(mconfig, *this)


            m_tilewidth = 0;
            m_tileheight = 0;
            m_tilexshift = 0;
            m_tileyshift = 0;
            m_bitmapwidth = 0;
            m_bitmapheight = 0;
            m_bitmapxmask = 0;
            m_bitmapymask = 0;
            m_entrycount = 0;
            m_entrybits = 0;
            m_spriterammask = 0;
            m_spriteramsize = 0;
            m_slipshift = 0;
            m_sliprammask = 0;
            m_slipramsize = 0;
            m_bank = 0;
            m_xscroll = 0;
            m_yscroll = 0;
            m_slipram = new optional_shared_ptr_uint16_t(this, "slip");
            m_activelast = null;
            m_last_xpos = 0;
            m_next_xpos = 0;
            m_gfxdecode = new required_device<gfxdecode_device>(this, finder_base.DUMMY_TAG);
        }


        public void atari_motion_objects_device_after_ctor(object screen_tag, atari_motion_objects_config config)
        {
            if (screen_tag is string)
                m_divideo.set_screen((string)screen_tag);
            else if (screen_tag is device_finder<screen_device>)
                m_divideo.set_screen((device_finder<screen_device>)screen_tag);
            else
                throw new emu_unimplemented();

            set_config(config);
        }


        // configuration
        //template <typename T> void set_gfxdecode(T &&tag) { m_gfxdecode.set_tag(std::forward<T>(tag)); }
        public void set_gfxdecode(string tag) { m_gfxdecode.set_tag(tag); }
        public void set_gfxdecode(device_finder<gfxdecode_device> tag) { m_gfxdecode.set_tag(tag); }

        void set_config(atari_motion_objects_config config) { m_atari_motion_objects_config = config; }  //void set_config(const atari_motion_objects_config &config) { static_cast<atari_motion_objects_config &>(*this) = config; }


        // getters
        //int bank() const { return m_bank; }
        //int xscroll() const { return m_xscroll; }
        //int yscroll() const { return m_yscroll; }
        //std::vector<uint32_t> &code_lookup() { return m_codelookup; }
        //std::vector<uint8_t> &color_lookup() { return m_colorlookup; }
        //std::vector<uint8_t> &gfx_lookup() { return m_gfxlookup; }

        // setters
        //void set_bank(int bank) { m_bank = bank; }
        //void set_xscroll(int xscroll) { m_xscroll = xscroll & m_bitmapxmask; }
        //void set_yscroll(int yscroll) { m_yscroll = yscroll & m_bitmapymask; }
        //void set_scroll(int xscroll, int yscroll) { set_xscroll(xscroll); set_yscroll(yscroll); }
        //void set_slipram(uint16_t *ram) { m_slipram.set_target(ram, 2); }


        // rendering
        //-------------------------------------------------
        //  draw: Render the motion objects to the
        //  destination bitmap.
        //-------------------------------------------------
        protected override void draw(bitmap_ind16 bitmap, rectangle cliprect)
        {
            // compute start/stop bands
            int startband = ((cliprect.top() + (int)m_yscroll - m_atari_motion_objects_config.m_slipoffset) & m_bitmapymask) >> m_slipshift;
            int stopband = ((cliprect.bottom() + (int)m_yscroll - m_atari_motion_objects_config.m_slipoffset) & m_bitmapymask) >> m_slipshift;
            if (startband > stopband)
                startband -= m_bitmapheight >> m_slipshift;
            if (m_slipshift == 0)
                stopband = startband;

            // loop over SLIP bands
            for (int band = startband; band <= stopband; band++)
            {
                // compute the starting link and clip for the current band
                rectangle bandclip = cliprect;
                int link = 0;
                if (m_slipshift != 0)
                {
                    // extract the link from the SLIP RAM
                    link = (m_slipram.target[band & m_sliprammask] >> m_linkmask.shift()) & m_linkmask.mask();

                    // compute minimum Y and wrap around if necessary
                    bandclip.min_y = ((band << m_slipshift) - (int)m_yscroll + m_atari_motion_objects_config.m_slipoffset) & m_bitmapymask;
                    if (bandclip.min_y >= bitmap.height())
                        bandclip.min_y -= m_bitmapheight;

                    // maximum Y is based on the minimum
                    bandclip.set_height(1 << m_slipshift);

                    // keep within the cliprect
                    bandclip &= cliprect;
                }

                // if this matches the last link, we don't need to re-process the list
                build_active_list(link);

                // initialize the parameters
                m_next_xpos = 123456;

                // safety check
                if (m_activelist == m_activelast.Buffer && m_activelast.Offset == 0)  //if (m_activelist == m_activelast)
                    continue;

                // set the start and end points
                Pointer<uint16_t> first;  //uint16_t *first, *last;
                Pointer<uint16_t> last;
                int step;
                if (m_atari_motion_objects_config.m_reverse)
                {
                    first = m_activelast - 4;
                    last = new Pointer<uint16_t>(m_activelist);
                    step = -4;
                }
                else
                {
                    first = new Pointer<uint16_t>(m_activelist);
                    last = m_activelast - 4;
                    step = 4;
                }

                // render the mos
                for (Pointer<uint16_t> current = new Pointer<uint16_t>(first); ; current += step)  //for (uint16_t *current = first; ; current += step)
                {
                    render_object(bitmap, bandclip, current);
                    if (current.Buffer == last.Buffer && current.Offset == last.Offset)  //if (current == last)
                        break;
                }
            }
        }


        //void apply_stain(bitmap_ind16 &bitmap, uint16_t *pf, uint16_t *mo, int x, int y);

        // memory access
        //uint16_t &slipram(int offset) { return m_slipram[offset]; }


        // device-level overrides

        //-------------------------------------------------
        //  device_start: Start up the device
        //-------------------------------------------------
        protected override void device_start()
        {
            // call parent
            base.device_start();

            // verify configuration
            gfx_element gfx = m_gfxdecode.target.digfx.gfx(m_atari_motion_objects_config.m_gfxindex);
            if (gfx == null)
                throw new emu_fatalerror("No gfxelement #{0}!", m_atari_motion_objects_config.m_gfxindex);

            // determine the masks
            m_linkmask.set(m_atari_motion_objects_config.m_link_entry);
            m_codemask.set(m_atari_motion_objects_config.m_code_entry);
            m_colormask.set(m_atari_motion_objects_config.m_color_entry);
            m_xposmask.set(m_atari_motion_objects_config.m_xpos_entry);
            m_yposmask.set(m_atari_motion_objects_config.m_ypos_entry);
            m_widthmask.set(m_atari_motion_objects_config.m_width_entry);
            m_heightmask.set(m_atari_motion_objects_config.m_height_entry);
            m_hflipmask.set(m_atari_motion_objects_config.m_hflip_entry);
            m_vflipmask.set(m_atari_motion_objects_config.m_vflip_entry);
            m_prioritymask.set(m_atari_motion_objects_config.m_priority_entry);
            m_neighbormask.set(m_atari_motion_objects_config.m_neighbor_entry);
            m_absolutemask.set(m_atari_motion_objects_config.m_absolute_entry);

            // derive tile information
            m_tilewidth = gfx.width();
            m_tileheight = gfx.height();
            m_tilexshift = compute_log(m_tilewidth);
            m_tileyshift = compute_log(m_tileheight);

            // derive bitmap information
            m_bitmapwidth = round_to_powerof2(m_xposmask.mask());
            m_bitmapheight = round_to_powerof2(m_yposmask.mask());
            m_bitmapxmask = m_bitmapwidth - 1;
            m_bitmapymask = m_bitmapheight - 1;

            // derive sprite information
            m_entrycount = round_to_powerof2(m_linkmask.mask());
            m_entrybits = compute_log(m_entrycount);
            m_spriteramsize = m_atari_motion_objects_config.m_bankcount * m_entrycount;
            m_spriterammask = m_spriteramsize - 1;
            m_slipshift = (m_atari_motion_objects_config.m_slipheight != 0) ? compute_log(m_atari_motion_objects_config.m_slipheight) : 0;
            m_slipramsize = m_bitmapheight >> m_slipshift;
            m_sliprammask = m_slipramsize - 1;
            if (m_atari_motion_objects_config.m_maxperline == 0)
                m_atari_motion_objects_config.m_maxperline = MAX_PER_BANK;

            // allocate and initialize the code lookup
            int codesize = round_to_powerof2((int)m_codemask.mask());
            m_codelookup.resize(codesize);
            for (int i = 0; i < codesize; i++)
                m_codelookup[i] = (uint32_t)i;

            // allocate and initialize the color lookup
            int colorsize = round_to_powerof2((int)m_colormask.mask());
            m_colorlookup.resize(colorsize);
            for (int i = 0; i < colorsize; i++)
                m_colorlookup[i] = (uint8_t)i;

            // allocate and the gfx lookup
            int gfxsize = codesize / 256;
            m_gfxlookup.resize(gfxsize);
            for (int i = 0; i < gfxsize; i++)
                m_gfxlookup[i] = m_atari_motion_objects_config.m_gfxindex;

            // allocate a timer to periodically force update
            m_force_update_timer = timer_alloc(TID_FORCE_UPDATE);
            m_force_update_timer.adjust(m_divideo.screen().time_until_pos(0));

            // register for save states
            save_item(NAME(new { m_bank }));
            save_item(NAME(new { m_xscroll }));
            save_item(NAME(new { m_yscroll }));
        }


        //-------------------------------------------------
        //  device_reset: Handle a device reset by
        //  clearing the interrupt lines and states
        //-------------------------------------------------
        protected override void device_reset()
        {
            // call parent
            base.device_reset();

            // reset the live state
            m_bank = 0;
        }


        //-------------------------------------------------
        //  device_timer: Handle device-specific timer
        //  calbacks
        //-------------------------------------------------
        protected override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr)
        {
            switch (id)
            {
                case TID_FORCE_UPDATE:
                    if (param > 0)
                        m_divideo.screen().update_partial(param - 1);
                    param += 64;
                    if (param >= m_divideo.screen().visible_area().bottom())
                        param = 0;
                    timer.adjust(m_divideo.screen().time_until_pos(param), param);
                    break;
            }
        }


        // internal helpers

        //-------------------------------------------------
        //  compute_log: Computes the number of bits
        //  necessary to hold a given value. The input must
        //  be an even power of two.
        //-------------------------------------------------
        int compute_log(int value)
        {
            int log = 0;

            if (value == 0)
                return -1;

            while ((value & 1) == 0)
            {
                log++;
                value >>= 1;
            }

            if (value != 1)
                return -1;

            return log;
        }


        //-------------------------------------------------
        //  round_to_powerof2: Rounds a number up to the
        //  nearest power of 2. Even powers of 2 are
        //  rounded up to the next greatest power
        //  (e.g., 4 returns 8).
        //-------------------------------------------------
        int round_to_powerof2(int value)
        {
            int log = 0;

            if (value == 0)
                return 1;

            while ((value >>= 1) != 0)
                log++;

            return 1 << (log + 1);
        }


        //-------------------------------------------------
        //  build_active_list: Build a list of active
        //  objects.
        //-------------------------------------------------
        void build_active_list(int link)
        {
            PointerU16 bankbase = new PointerU16(spriteram(), (int)m_bank << (m_entrybits + 2));  //uint16_t *bankbase = &spriteram()[m_bank << (m_entrybits + 2)];
            Pointer<uint16_t> current = new Pointer<uint16_t>(m_activelist, 0);  //uint16_t *current = &m_activelist[0];

            // visit all the motion objects and copy their data into the display list
            uint8_t [] visited = new uint8_t [MAX_PER_BANK];
            for (int i = 0; i < m_atari_motion_objects_config.m_maxperline && (visited[link] == 0); i++)
            {
                // copy the current entry into the list
                Pointer<uint16_t> modata = new Pointer<uint16_t>(current);  //uint16_t *modata = current;
                if (!m_atari_motion_objects_config.m_split)
                {
                    PointerU16 srcdata = new PointerU16(bankbase, link * 4);  //uint16_t *srcdata = &bankbase[link * 4];
                    current[0] = srcdata[0]; current++;  //*current++ = srcdata[0];
                    current[0] = srcdata[1]; current++;  //*current++ = srcdata[1];
                    current[0] = srcdata[2]; current++;  //*current++ = srcdata[2];
                    current[0] = srcdata[3]; current++;  //*current++ = srcdata[3];
                }
                else
                {
                    PointerU16 srcdata = new PointerU16(bankbase, link);  //uint16_t *srcdata = &bankbase[link];
                    current[0] = srcdata[(uint32_t)(0 << m_entrybits)]; current++;  //*current++ = srcdata[uint32_t(0 << m_entrybits)];
                    current[0] = srcdata[(uint32_t)(1 << m_entrybits)]; current++;  //*current++ = srcdata[uint32_t(1 << m_entrybits)];
                    current[0] = srcdata[(uint32_t)(2 << m_entrybits)]; current++;  //*current++ = srcdata[uint32_t(2 << m_entrybits)];
                    current[0] = srcdata[(uint32_t)(3 << m_entrybits)]; current++;  //*current++ = srcdata[uint32_t(3 << m_entrybits)];
                }

                // link to the next object
                visited[link] = 1;
                if (m_atari_motion_objects_config.m_linked)
                    link = m_linkmask.extract(modata);
                else
                    link = (link + 1) & m_linkmask.mask();
            }

            // note the last entry
            m_activelast = current;
        }


        //-------------------------------------------------
        //  render_object: Internal processing callback
        //  that renders to the backing bitmap and then
        //  copies the result  to the destination.
        //-------------------------------------------------
        void render_object(bitmap_ind16 bitmap, rectangle cliprect, Pointer<uint16_t> entry)  //void atari_motion_objects_device::render_object(bitmap_ind16 &bitmap, const rectangle &cliprect, const uint16_t *entry)
        {
            // select the gfx element and save off key information
            int rawcode = (int)m_codemask.extract(entry);
            gfx_element gfx = m_gfxdecode.target.digfx.gfx(m_gfxlookup[rawcode >> 8]);
            int save_granularity = gfx.granularity();
            int save_colorbase = (int)gfx.colorbase();
            int save_colors = (int)gfx.colors();
            gfx.set_granularity(1);
            gfx.set_colorbase(0);
            gfx.set_colors(65536);

            // extract data from the various words
            int code = (int)m_codelookup[rawcode];
            int color = m_colorlookup[m_colormask.extract(entry)];
            int xpos = m_xposmask.extract(entry);
            int ypos = -m_yposmask.extract(entry);
            int hflip = m_hflipmask.extract(entry);
            int vflip = m_vflipmask.extract(entry);
            int width = m_widthmask.extract(entry) + 1;
            int height = m_heightmask.extract(entry) + 1;
            int priority = m_prioritymask.extract(entry);

            // compute the effective color, merging in priority
            color = (color * save_granularity) | (priority << PRIORITY_SHIFT);
            color += m_atari_motion_objects_config.m_palettebase;

            // add in the scroll positions if we're not in absolute coordinates
            if (m_absolutemask.extract(entry) == 0)
            {
                xpos -= (int)m_xscroll;
                ypos -= (int)m_yscroll;
            }

            // adjust for height
            ypos -= height << m_tileyshift;

            // handle previous hold bits
            if (m_next_xpos != 123456)
                xpos = (int)m_next_xpos;
            m_next_xpos = 123456;

            // check for the hold bit
            if (m_neighbormask.extract(entry) != 0)
            {
                if (!m_atari_motion_objects_config.m_nextneighbor)
                    xpos = (int)m_last_xpos + m_tilewidth;
                else
                    m_next_xpos = (uint32_t)(xpos + m_tilewidth);
            }
            m_last_xpos = (uint32_t)xpos;

            // adjust the final coordinates
            xpos &= m_bitmapxmask;
            ypos &= m_bitmapymask;
            if (xpos >= bitmap.width())
                xpos -= m_bitmapwidth;
            if (ypos >= bitmap.height())
                ypos -= m_bitmapheight;

            // is this one special?
            if (m_specialmask.mask() == 0 || m_specialmask.extract(entry) != m_atari_motion_objects_config.m_specialvalue)
            {
                // adjust for h flip
                int xadv = m_tilewidth;
                if (hflip != 0)
                {
                    xpos += (width - 1) << m_tilexshift;
                    xadv = -xadv;
                }

                // adjust for v flip
                int yadv = m_tileheight;
                if (vflip != 0)
                {
                    ypos += (height - 1) << m_tileyshift;
                    yadv = -yadv;
                }

                // standard order is: loop over Y first, then X
                if (!m_atari_motion_objects_config.m_swapxy)
                {
                    // loop over the height
                    for (int y = 0, sy = ypos; y < height; y++, sy += yadv)
                    {
                        // clip the Y coordinate
                        if (sy <= cliprect.top() - m_tileheight)
                        {
                            code += width;
                            continue;
                        }
                        else if (sy > cliprect.bottom())
                        {
                            break;
                        }

                        // loop over the width
                        for (int x = 0, sx = xpos; x < width; x++, sx += xadv, code++)
                        {
                            // clip the X coordinate
                            if (sx <= -cliprect.left() - m_tilewidth || sx > cliprect.right())
                                continue;

                            // draw the sprite
                            gfx.transpen_raw(bitmap, cliprect, (u32)code, (u32)color, hflip, vflip, sx, sy, m_atari_motion_objects_config.m_transpen);
                            mark_dirty(sx, sx + m_tilewidth - 1, sy, sy + m_tileheight - 1);
                        }
                    }
                }

                // alternative order is swapped
                else
                {
                    // loop over the width
                    for (int x = 0, sx = xpos; x < width; x++, sx += xadv)
                    {
                        // clip the X coordinate
                        if (sx <= cliprect.left() - m_tilewidth)
                        {
                            code += height;
                            continue;
                        }
                        else if (sx > cliprect.right())
                        {
                            break;
                        }

                        // loop over the height
                        for (int y = 0, sy = ypos; y < height; y++, sy += yadv, code++)
                        {
                            // clip the X coordinate
                            if (sy <= -cliprect.top() - m_tileheight || sy > cliprect.bottom())
                                continue;

                            // draw the sprite
                            gfx.transpen_raw(bitmap, cliprect, (u32)code, (u32)color, hflip, vflip, sx, sy, m_atari_motion_objects_config.m_transpen);
                            mark_dirty(sx, sx + m_tilewidth - 1, sy, sy + m_tileheight - 1);
                        }
                    }
                }
            }

            // restore original gfx information
            gfx.set_granularity((u16)save_granularity);
            gfx.set_colorbase((u16)save_colorbase);
            gfx.set_colors((u32)save_colors);
        }
    }
}
