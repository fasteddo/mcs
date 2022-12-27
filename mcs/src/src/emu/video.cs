// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using s8 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.attotime_global;
using static mame.cpp_global;
using static mame.eminline_global;
using static mame.machine_global;
using static mame.osdcore_global;
using static mame.osdfile_global;
using static mame.profiler_global;
using static mame.render_global;
using static mame.util;


namespace mame
{
    // ======================> video_manager
    public class video_manager
    {
        const bool LOG_THROTTLE = false;

        // number of levels of frameskipping supported
        const int FRAMESKIP_LEVELS = 12;
        public const int MAX_FRAMESKIP = FRAMESKIP_LEVELS - 2;

        // frameskipping tables
        static readonly bool [,] s_skiptable =
        {
            { false, false, false, false, false, false, false, false, false, false, false, false },
            { false, false, false, false, false, false, false, false, false, false, false, true  },
            { false, false, false, false, false, true , false, false, false, false, false, true  },
            { false, false, false, true , false, false, false, true , false, false, false, true  },
            { false, false, true , false, false, true , false, false, true , false, false, true  },
            { false, true , false, false, true , false, true , false, false, true , false, true  },
            { false, true , false, true , false, true , false, true , false, true , false, true  },
            { false, true , false, true , true , false, true , false, true , true , false, true  },
            { false, true , true , false, true , true , false, true , true , false, true , true  },
            { false, true , true , true , false, true , true , true , false, true , true , true  },
            { false, true , true , true , true , true , false, true , true , true , true , true  },
            { false, true , true , true , true , true , true , true , true , true , true , true  }
        };

        const attoseconds_t ATTOSECONDS_PER_SPEED_UPDATE = ATTOSECONDS_PER_SECOND / 4;
        const int PAUSED_REFRESH_RATE = 30;


        // internal state
        running_machine m_machine;                  // reference to our machine

        // screenless systems
        emu_timer m_screenless_frame_timer;   // timer to signal VBLANK start
        bool m_output_changed;           // did an output element change?

        // throttling calculations
        osd_ticks_t m_throttle_last_ticks;      // osd_ticks the last call to throttle
        attotime m_throttle_realtime;        // real time the last call to throttle
        attotime m_throttle_emutime;         // emulated time the last call to throttle
        u32 m_throttle_history;         // history of frames where we were fast enough

        // dynamic speed computation
        osd_ticks_t m_speed_last_realtime;      // real time at the last speed calculation
        attotime m_speed_last_emutime;       // emulated time at the last speed calculation
        double m_speed_percent;            // most recent speed percentage

        // overall speed computation
        public u32 m_overall_real_seconds;     // accumulated real seconds at normal speed
        public osd_ticks_t m_overall_real_ticks;       // accumulated real ticks at normal speed
        public attotime m_overall_emutime;          // accumulated emulated time at normal speed
        u32 m_overall_valid_counter;    // number of consecutive valid time periods

        u32 m_frame_update_counter;     // how many times frame_update() has been called

        // configuration
        bool m_throttled;                // flag: TRUE if we're currently throttled
        float m_throttle_rate;            // target rate for throttling
        bool m_fastforward;              // flag: TRUE if we're currently fast-forwarding
        u32 m_seconds_to_run;           // number of seconds to run before quitting
        bool m_auto_frameskip;           // flag: TRUE if we're automatically frameskipping
        u32 m_speed;                    // overall speed (*1000)
        bool m_low_latency;              // flag: true if we are throttling after blitting

        // frameskipping
        u8 m_empty_skip_count;         // number of empty frames we have skipped
        u8 m_frameskip_max;            // maximum frameskip level
        u8 m_frameskip_level;          // current frameskip level
        u8 m_frameskip_counter;        // counter that counts through the frameskip steps
        s8 m_frameskip_adjust;
        bool m_skipping_this_frame;      // flag: TRUE if we are skipping the current frame
        osd_ticks_t m_average_oversleep;        // average number of ticks the OSD oversleeps

        // snapshot stuff
        render_target m_snap_target;              // screen shapshot target
        bitmap_rgb32 m_snap_bitmap = new bitmap_rgb32();              // screen snapshot bitmap
        bool m_snap_native;              // are we using native per-screen layouts?
        s32 m_snap_width;               // width of snapshots (0 == auto)
        s32 m_snap_height;              // height of snapshots (0 == auto)

        // movie recordings
        std.vector<movie_recording> m_movie_recordings = new std.vector<movie_recording>();


        // construction/destruction

        //-------------------------------------------------
        //  video_manager - constructor
        //-------------------------------------------------
        public video_manager(running_machine machine)
        {
            m_machine = machine;
            m_screenless_frame_timer = null;
            m_output_changed = false;
            m_throttle_realtime = attotime.zero;
            m_throttle_emutime = attotime.zero;
            m_throttle_history = 0;
            m_speed_last_realtime = 0;
            m_speed_last_emutime = attotime.zero;
            m_speed_percent = 1.0;
            m_overall_real_seconds = 0;
            m_overall_real_ticks = 0;
            m_overall_emutime = attotime.zero;
            m_overall_valid_counter = 0;
            m_frame_update_counter = 0;
            m_throttled = true;
            m_throttle_rate = 1.0f;
            m_fastforward = false;
            m_seconds_to_run = (u32)machine.options().seconds_to_run();
            m_auto_frameskip = machine.options().auto_frameskip();
            m_speed = (u32)original_speed_setting();
            m_low_latency = machine.options().low_latency();
            m_empty_skip_count = 0;
            m_frameskip_max = m_auto_frameskip ? (u8)machine.options().frameskip() : (u8)0;
            m_frameskip_level = m_auto_frameskip ? (u8)0 : (u8)machine.options().frameskip();
            m_frameskip_counter = 0;
            m_frameskip_adjust = 0;
            m_skipping_this_frame = false;
            m_average_oversleep = 0;
            m_snap_target = null;
            m_snap_native = true;
            m_snap_width = 0;
            m_snap_height = 0;


            // request a callback upon exiting
            machine.add_notifier(machine_notification.MACHINE_NOTIFY_EXIT, exit);
            machine.save().register_postload(postload);

            // extract initial execution state from global configuration settings
            update_refresh_speed();

            unsigned screen_count = (unsigned)(new screen_device_enumerator(machine.root_device()).count());
            bool no_screens = screen_count == 0;

            // create a render target for snapshots
            string viewname = machine.options().snap_view();
            m_snap_native = !no_screens && std.strcmp(viewname, "native") == 0;

            // the native target is hard-coded to our internal layout and has all options disabled
            if (m_snap_native)
            {
                throw new emu_unimplemented();
            }
            else
            {
                // other targets select the specified view and turn off effects
                m_snap_target = machine.render().target_alloc(null, RENDER_CREATE_HIDDEN);
                m_snap_target.set_view(m_snap_target.configured_view(viewname, 0, 1));
                m_snap_target.set_screen_overlay_enabled(false);
            }

            // extract snap resolution if present
            //if (sscanf(machine.options().snap_size(), "%dx%d", &m_snap_width, &m_snap_height) != 2)
            var parts = machine.options().snap_size().Split('x');
            if (parts.Length == 2)
            {
                m_snap_width  = Convert.ToInt32(parts[0]);
                m_snap_height = Convert.ToInt32(parts[1]);
            }
            else
            {
                m_snap_width = m_snap_height = 0;
            }

            // if no screens, create a periodic timer to drive updates
            if (no_screens)
            {
                m_screenless_frame_timer = machine.scheduler().timer_alloc(screenless_update_callback, this);
                m_screenless_frame_timer.adjust(screen_device.DEFAULT_FRAME_PERIOD, 0, screen_device.DEFAULT_FRAME_PERIOD);
                machine.output().set_global_notifier(video_notifier_callback, this);
            }
        }


        // getters
        running_machine machine() { return m_machine; }
        public bool skip_this_frame() { return m_skipping_this_frame; }
        public int speed_factor() { return (int)m_speed; }
        public int frameskip() { return m_auto_frameskip ? -1 : m_frameskip_level; }
        public bool throttled() { return m_throttled; }
        float throttle_rate() { return m_throttle_rate; }
        bool fastforward() { return m_fastforward; }


        // setters

        //-------------------------------------------------
        //  set_frameskip - set the current actual
        //  frameskip (-1 means autoframeskip)
        //-------------------------------------------------
        public void set_frameskip(int frameskip)
        {
            if (0 > frameskip)
            {
                // -1 means autoframeskip
                if (!m_auto_frameskip)
                    m_frameskip_level = 0;
                m_auto_frameskip = true;
            }
            else
            {
                // any other level is a direct control
                m_auto_frameskip = false;
                m_frameskip_level = (u8)std.min(frameskip, MAX_FRAMESKIP);
            }
        }

        public void set_throttled(bool throttled) { m_throttled = throttled; }
        void set_throttle_rate(float throttle_rate) { m_throttle_rate = throttle_rate; }
        public void set_fastforward(bool ffwd) { m_fastforward = ffwd; }
        void set_output_changed() { m_output_changed = true; }


        // misc

        //-------------------------------------------------
        //  toggle_record_movie
        //-------------------------------------------------
        public void toggle_record_movie(movie_recording.format format)
        {
            throw new emu_unimplemented();
        }


        std.error_condition open_next(emu_file file, string extension, uint32_t index = 0) { throw new emu_unimplemented(); }


        //void compute_snapshot_size(s32 &width, s32 &height);
        //void pixels(u32 *buffer);


        // render a frame

        //-------------------------------------------------
        //  frame_update - handle frameskipping and UI,
        //  plus updating the screen during normal
        //  operations
        //-------------------------------------------------
        public void frame_update(bool from_debugger = false)
        {
            m_frame_update_counter++;

            // only render sound and video if we're in the running phase
            machine_phase phase = machine().phase();
            bool skipped_it = m_skipping_this_frame;
            if (phase == machine_phase.RUNNING && (!machine().paused() || machine().options().update_in_pause()))
            {
                bool anything_changed = finish_screen_updates();

                // if none of the screens changed and we haven't skipped too many frames in a row,
                // mark this frame as skipped to prevent throttling; this helps for games that
                // don't update their screen at the monitor refresh rate
                if (!anything_changed && !m_auto_frameskip && m_frameskip_level == 0 && m_empty_skip_count++ < 3)
                    skipped_it = true;
                else
                    m_empty_skip_count = 0;
            }

            // draw the user interface
            emulator_info.draw_user_interface(machine());

            // if we're throttling, synchronize before rendering
            attotime current_time = machine().time();
            if (!from_debugger && !skipped_it && phase > machine_phase.INIT && !m_low_latency && effective_throttle())
                update_throttle(current_time);

            // ask the OSD to update

            g_profiler.start(profile_type.PROFILER_BLIT);

            machine().osd().update(!from_debugger && skipped_it);

            g_profiler.stop();

            // we synchronize after rendering instead of before, if low latency mode is enabled
            if (!from_debugger && !skipped_it && phase > machine_phase.INIT && m_low_latency && effective_throttle())
                update_throttle(current_time);

            // get most recent input now
            machine().osd().input_update();

            emulator_info.periodic_check();

            if (!from_debugger)
            {
                // perform tasks for this frame
                machine().call_notifiers(machine_notification.MACHINE_NOTIFY_FRAME);

                // update frameskipping
                if (phase > machine_phase.INIT)
                    update_frameskip();

                // update speed computations
                if (!skipped_it && phase > machine_phase.INIT)
                    recompute_speed(current_time);
            }


            // call the end-of-frame callback
            if (phase == machine_phase.RUNNING)
            {
                // reset partial updates if we're paused or if the debugger is active
                screen_device screen = new screen_device_enumerator(machine().root_device()).first();
                bool debugger_enabled = (machine().debug_flags & DEBUG_FLAG_ENABLED) != 0;
                bool within_instruction_hook = debugger_enabled && machine().debugger().within_instruction_hook();
                if (screen != null && ((machine().paused() && machine().options().update_in_pause()) || from_debugger || within_instruction_hook))
                    screen.reset_partial_updates();
            }
        }


        // current speed helpers

        //-------------------------------------------------
        //  speed_text - print the text to be displayed
        //  into a string buffer
        //-------------------------------------------------
        public string speed_text()
        {
            string str = "";

            // if we're paused, just display Paused
            bool paused = machine().paused();
            if (paused)
                str += "paused";

            // if we're fast forwarding, just display Fast-forward
            else if (m_fastforward)
                str += "fast ";

            // if we're auto frameskipping, display that plus the level
            else if (effective_autoframeskip())
                util.stream_format(ref str, "auto{0}/{1}", effective_frameskip(), m_frameskip_max != 0 ? m_frameskip_max : MAX_FRAMESKIP);

            // otherwise, just display the frameskip plus the level
            else
                util.stream_format(ref str, "skip {0}/{1}", effective_frameskip(), MAX_FRAMESKIP);

            // append the speed for all cases except paused
            if (!paused)
                util.stream_format(ref str, "  {0:f2}%", (int)(100 * m_speed_percent + 0.5));  //%4d%%

            // display the number of partial updates as well
            int partials = 0;
            foreach (screen_device screen in new screen_device_enumerator(machine().root_device()))
                partials += screen.partial_updates();

            if (partials > 1)
                util.stream_format(ref str, "  {0} partial updates", partials);

            return str;
        }

        public double speed_percent() { return m_speed_percent; }
        public u32 frame_update_count() { return m_frame_update_counter; }


        // snapshots

        //bool snap_native() const { return m_snap_native; }
        //render_target &snapshot_target() { return *m_snap_target; }


        //-------------------------------------------------
        //  save_snapshot - save a snapshot to the given
        //  file handle
        //-------------------------------------------------
        void save_snapshot(screen_device screen, emu_file file)
        {
            throw new emu_unimplemented();
        }

        //-------------------------------------------------
        //  save_active_screen_snapshots - save a
        //  snapshot of all active screens
        //-------------------------------------------------
        public void save_active_screen_snapshots()
        {
            throw new emu_unimplemented();
        }


        // movies

        //-------------------------------------------------
        //  begin_recording - begin recording of a movie
        //-------------------------------------------------
        public void begin_recording(string name, movie_recording.format format)
        {
            throw new emu_unimplemented();
        }


        void end_recording()
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  add_sound_to_recording - add sound to a movie
        //  recording
        //-------------------------------------------------
        public void add_sound_to_recording(Pointer<s16> sound, int numsamples)  // const s16 *sound, int numsamples)
        {
            foreach (var recording in m_movie_recordings)
                recording.add_sound_to_recording(sound, numsamples);
        }


        public bool is_recording() { return !m_movie_recordings.empty(); }


        // internal helpers

        //-------------------------------------------------
        //  video_exit - close down the video system
        //-------------------------------------------------
        void exit(running_machine machine_)
        {
            // stop recording any movie
            m_movie_recordings.clear();

            // free the snapshot target
            machine().render().target_free(m_snap_target);
            m_snap_bitmap.reset();

            // print a final result if we have at least 2 seconds' worth of data
            if (!emulator_info.standalone() && m_overall_emutime.seconds() >= 1)
            {
                osd_ticks_t tps = m_osdcore.osd_ticks_per_second();
                double final_real_time = (double)m_overall_real_seconds + (double)m_overall_real_ticks / (double)tps;
                double final_emu_time = m_overall_emutime.as_double();
                osd_printf_info("Average speed: {0}%% ({1} seconds)\n", 100 * final_emu_time / final_real_time, (m_overall_emutime + new attotime(0, ATTOSECONDS_PER_SECOND / 2)).seconds());  // %.2f%% (%d seconds)\n
            }
        }


        //-------------------------------------------------
        //  screenless_update_callback - update generator
        //  when there are no screens to drive it
        //-------------------------------------------------
        void screenless_update_callback(object o, int param)  //void *ptr, int param)
        {
            // force an update
            frame_update(false);
        }

        //-------------------------------------------------
        //  postload - callback for resetting things after
        //  state has been loaded
        //-------------------------------------------------
        void postload()
        {
            attotime emutime = machine().time();
            foreach (var x in m_movie_recordings)
                x.set_next_frame_time(emutime);

            // reset speed counters
            m_speed_last_realtime = m_osdcore.osd_ticks();
            m_speed_last_emutime = emutime;
        }


        // effective value helpers

        //-------------------------------------------------
        //  effective_autoframeskip - return the effective
        //  autoframeskip value, accounting for fast
        //  forward
        //-------------------------------------------------
        bool effective_autoframeskip()
        {
            // if we're fast forwarding or paused, autoframeskip is disabled
            if (m_fastforward || machine().paused())
                return false;

            // otherwise, it's up to the user
            return m_auto_frameskip;
        }


        //-------------------------------------------------
        //  effective_frameskip - return the effective
        //  frameskip value, accounting for fast
        //  forward
        //-------------------------------------------------
        int effective_frameskip()
        {
            // if we're fast forwarding, use the maximum frameskip
            if (m_fastforward)
                return FRAMESKIP_LEVELS - 1;

            // otherwise, it's up to the user
            return m_frameskip_level;
        }


        //-------------------------------------------------
        //  effective_throttle - return the effective
        //  throttle value, accounting for fast
        //  forward and user interface
        //-------------------------------------------------
        bool effective_throttle()
        {
            // if we're paused, or if the UI is active, we always throttle
            if (machine().paused() && !machine().options().update_in_pause()) //|| machine().ui().is_menu_active())
                return true;

            // if we're fast forwarding, we don't throttle
            if (m_fastforward)
                return false;

            // otherwise, it's up to the user
            return throttled();
        }


        // speed and throttling helpers
        //-------------------------------------------------
        //  original_speed_setting - return the original
        //  speed setting
        //-------------------------------------------------
        int original_speed_setting() { return (int)(machine().options().speed() * 1000.0f + 0.5f); }


        //-------------------------------------------------
        //  finish_screen_updates - finish updating all
        //  the screens
        //-------------------------------------------------
        bool finish_screen_updates()
        {
            // finish updating the screens
            screen_device_enumerator iter = new screen_device_enumerator(machine().root_device());

            bool has_live_screen = false;
            foreach (screen_device screen in iter)
            {
                if (screen.partial_scan_hpos() >= 0) // previous update ended mid-scanline
                    screen.update_now();

                screen.update_partial(screen.visible_area().max_y);

                if (machine().render().is_live(screen))
                    has_live_screen = true;
            }

            // now add the quads for all the screens
            bool anything_changed = !has_live_screen || m_output_changed;
            m_output_changed = false;

            // now add the quads for all the screens
            foreach (screen_device screen in iter)
                if (screen.update_quads())
                    anything_changed = true;

            // draw HUD from LUA callback (if any)
            anything_changed |= emulator_info.frame_hook();

            // update our movie recording and burn-in state
            if (!machine().paused())
            {
                record_frame();

                // iterate over screens and update the burnin for the ones that care
                foreach (screen_device screen in iter)
                    screen.update_burnin();
            }

            // draw any crosshairs
            foreach (screen_device screen in iter)
                machine().crosshair().render(screen);

            return anything_changed;
        }


        //-------------------------------------------------
        //  update_throttle - throttle to the game's
        //  natural speed
        //-------------------------------------------------
        void update_throttle(attotime emutime)
        {
            /*

               Throttling theory:

               This routine is called periodically with an up-to-date emulated time.
               The idea is to synchronize real time with emulated time. We do this
               by "throttling", or waiting for real time to catch up with emulated
               time.

               In an ideal world, it will take less real time to emulate and render
               each frame than the emulated time, so we need to slow things down to
               get both times in sync.

               There are many complications to this model:

                   * some games run too slow, so each frame we get further and
                       further behind real time; our only choice here is to not
                       throttle

                   * some games have very uneven frame rates; one frame will take
                       a long time to emulate, and the next frame may be very fast

                   * we run on top of multitasking OSes; sometimes execution time
                       is taken away from us, and this means we may not get enough
                       time to emulate one frame

                   * we may be paused, and emulated time may not be marching
                       forward

                   * emulated time could jump due to resetting the machine or
                       restoring from a saved state

            */

            // outer scope so we can break out in case of a resync
            while (true)
            {
                // apply speed factor to emu time
                if (m_speed != 0 && m_speed != 1000)
                {
                    // multiply emutime by 1000, then divide by the global speed factor
                    emutime = (emutime * 1000) / m_speed;
                }

                // compute conversion factors up front
                osd_ticks_t ticks_per_second = m_osdcore.osd_ticks_per_second();
                attoseconds_t attoseconds_per_tick = (attoseconds_t)(ATTOSECONDS_PER_SECOND / ticks_per_second * m_throttle_rate);

                // if we're paused, emutime will not advance; instead, we subtract a fixed
                // amount of time (1/60th of a second) from the emulated time that was passed in,
                // and explicitly reset our tracked real and emulated timers to that value ...
                // this means we pretend that the last update was exactly 1/60th of a second
                // ago, and was in sync in both real and emulated time
                if (machine().paused())
                {
                    m_throttle_emutime = emutime - new attotime(0, ATTOSECONDS_PER_SECOND / PAUSED_REFRESH_RATE);
                    m_throttle_realtime = m_throttle_emutime;
                }

                // attempt to detect anomalies in the emulated time by subtracting the previously
                // reported value from our current value; this should be a small value somewhere
                // between 0 and 1/10th of a second ... anything outside of this range is obviously
                // wrong and requires a resync
                attoseconds_t emu_delta_attoseconds = (emutime - m_throttle_emutime).as_attoseconds();
                if (emu_delta_attoseconds < 0 || emu_delta_attoseconds > ATTOSECONDS_PER_SECOND / 10)
                {
                    if (LOG_THROTTLE)
                        machine().logerror("Resync due to weird emutime delta: {0}\n", new attotime(0, emu_delta_attoseconds).as_string(18));
                    break;
                }

                // now determine the current real time in OSD-specified ticks; we have to be careful
                // here because counters can wrap, so we only use the difference between the last
                // read value and the current value in our computations
                osd_ticks_t diff_ticks = m_osdcore.osd_ticks() - m_throttle_last_ticks;
                m_throttle_last_ticks += diff_ticks;

                // if it has been more than a full second of real time since the last call to this
                // function, we just need to resynchronize
                if (diff_ticks >= ticks_per_second)
                {
                    if (LOG_THROTTLE)
                        machine().logerror("Resync due to real time advancing by more than 1 second\n");
                    break;
                }

                // convert this value into attoseconds for easier comparison
                attoseconds_t real_delta_attoseconds = (attoseconds_t)diff_ticks * attoseconds_per_tick;

                // now update our real and emulated timers with the current values
                m_throttle_emutime = emutime;
                m_throttle_realtime += new attotime(0, real_delta_attoseconds);

                // keep a history of whether or not emulated time beat real time over the last few
                // updates; this can be used for future heuristics
                m_throttle_history = (u32)((m_throttle_history << 1) | ((emu_delta_attoseconds > real_delta_attoseconds) ? 1U : 0));

                // determine how far ahead real time is versus emulated time; note that we use the
                // accumulated times for this instead of the deltas for the current update because
                // we want to track time over a longer duration than a single update
                attoseconds_t real_is_ahead_attoseconds = (m_throttle_emutime - m_throttle_realtime).as_attoseconds();

                // if we're more than 1/10th of a second out, or if we are behind at all and emulation
                // is taking longer than the real frame, we just need to resync
                if (real_is_ahead_attoseconds < -ATTOSECONDS_PER_SECOND / 10 ||
                    (real_is_ahead_attoseconds < 0 && population_count_32(m_throttle_history & 0xff) < 6))
                {
                    if (LOG_THROTTLE)
                        machine().logerror("Resync due to being behind: {0} (history={1})\n", new attotime(0, -real_is_ahead_attoseconds).as_string(18), m_throttle_history);
                    break;
                }

                // if we're behind, it's time to just get out
                if (real_is_ahead_attoseconds < 0)
                    return;

                // compute the target real time, in ticks, where we want to be
                osd_ticks_t target_ticks = m_throttle_last_ticks + (osd_ticks_t)(real_is_ahead_attoseconds / attoseconds_per_tick);

                // throttle until we read the target, and update real time to match the final time
                diff_ticks = throttle_until_ticks(target_ticks) - m_throttle_last_ticks;
                m_throttle_last_ticks += diff_ticks;
                m_throttle_realtime += new attotime(0, (attoseconds_t)diff_ticks * attoseconds_per_tick);
                return;
            }

            // reset realtime and emutime to the same value
            m_throttle_realtime = m_throttle_emutime = emutime;
        }


        //-------------------------------------------------
        //  throttle_until_ticks - spin until the
        //  specified target time, calling the OSD code
        //  to sleep if possible
        //-------------------------------------------------
        osd_ticks_t throttle_until_ticks(osd_ticks_t target_ticks)
        {
            // we're allowed to sleep via the OSD code only if we're configured to do so
            // and we're not frameskipping due to autoframeskip, or if we're paused
            bool allowed_to_sleep = (machine().options().sleep() && (!effective_autoframeskip() || effective_frameskip() == 0)) || machine().paused();

            // loop until we reach our target
            g_profiler.start(profile_type.PROFILER_IDLE);

            osd_ticks_t current_ticks = m_osdcore.osd_ticks();
            while (current_ticks < target_ticks)
            {
                // compute how much time to sleep for, taking into account the average oversleep
                osd_ticks_t delta = target_ticks - current_ticks;
                if (delta > m_average_oversleep / 1000)
                    delta -= m_average_oversleep / 1000;
                else
                    delta = 0;

                // see if we can sleep
                bool slept = allowed_to_sleep && delta != 0;
                if (slept)
                    m_osdcore.osd_sleep(delta);

                // read the new value
                osd_ticks_t new_ticks = m_osdcore.osd_ticks();

                // keep some metrics on the sleeping patterns of the OSD layer
                if (slept)
                {
                    // if we overslept, keep an average of the amount
                    osd_ticks_t actual_ticks = new_ticks - current_ticks;
                    if (actual_ticks > delta)
                    {
                        // take 99% of the previous average plus 1% of the new value
                        osd_ticks_t oversleep_milliticks = 1000 * (actual_ticks - delta);
                        m_average_oversleep = (m_average_oversleep * 99 + oversleep_milliticks) / 100;

                        if (LOG_THROTTLE)
                            machine().logerror("Slept for {0} ticks, got {1} ticks, avgover = {2}\n", delta, actual_ticks, m_average_oversleep);
                    }
                }
                current_ticks = new_ticks;
            }

            g_profiler.stop();

            return current_ticks;
        }

        //-------------------------------------------------
        //  update_frameskip - update frameskipping
        //  counters and periodically update autoframeskip
        //-------------------------------------------------
        void update_frameskip()
        {
            // if we're throttling and autoframeskip is on, adjust
            if (effective_throttle() && effective_autoframeskip() && m_frameskip_counter == 0)
            {
                // calibrate the "adjusted speed" based on the target
                double adjusted_speed_percent = m_speed_percent / (double)m_throttle_rate;

                double speed = m_speed * 0.001;
                if (adjusted_speed_percent >= 0.995 * speed)
                {
                    // if we're too fast, attempt to decrease the frameskip
                    // but only after 3 consecutive frames where we are too fast
                    if (++m_frameskip_adjust >= 3)
                    {
                        m_frameskip_adjust = 0;
                        if (m_frameskip_level > 0)
                            m_frameskip_level--;
                    }
                }
                else
                {
                    // if we're too slow, attempt to increase the frameskip
                    if (adjusted_speed_percent < 0.80 *  speed) // if below 80% speed, be more aggressive
                        m_frameskip_adjust -= (s8)((0.90 * speed - m_speed_percent) / 0.05);
                    else if (m_frameskip_level < 8) // if we're close, only force it up to frameskip 8
                        m_frameskip_adjust--;

                    // perform the adjustment
                    while (m_frameskip_adjust <= -2)
                    {
                        m_frameskip_adjust += 2;
                        if (m_frameskip_level < (m_frameskip_max != 0 ? m_frameskip_max : MAX_FRAMESKIP))
                            m_frameskip_level++;
                    }
                }
            }

            // increment the frameskip counter and determine if we will skip the next frame
            m_frameskip_counter = (u8)((m_frameskip_counter + 1) % FRAMESKIP_LEVELS);
            m_skipping_this_frame = s_skiptable[effective_frameskip(), m_frameskip_counter];
        }

        //-------------------------------------------------
        //  update_refresh_speed - update the m_speed
        //  based on the maximum refresh rate supported
        //-------------------------------------------------
        public void update_refresh_speed()
        {
            // only do this if the refreshspeed option is used
            if (machine().options().refresh_speed())
            {
                double minrefresh = machine().render().max_update_rate();
                if (minrefresh != 0)
                {
                    // find the screen with the shortest frame period (max refresh rate)
                    // note that we first check the token since this can get called before all screens are created
                    attoseconds_t min_frame_period = ATTOSECONDS_PER_SECOND;
                    foreach (screen_device screen in new screen_device_enumerator(machine().root_device()))
                    {
                        attoseconds_t period = screen.frame_period().attoseconds();
                        if (period != 0)
                            min_frame_period = std.min(min_frame_period, period);
                    }

                    // compute a target speed as an integral percentage
                    // note that we lop 0.25Hz off of the minrefresh when doing the computation to allow for
                    // the fact that most refresh rates are not accurate to 10 digits...
                    u32 target_speed = (u32)std.floor((minrefresh - 0.25) * 1000.0 / ATTOSECONDS_TO_HZ(min_frame_period));
                    u32 original_speed = (u32)original_speed_setting();
                    target_speed = std.min(target_speed, original_speed);

                    // if we changed, log that verbosely
                    if (target_speed != m_speed)
                    {
                        osd_printf_verbose("Adjusting target speed to {0}%% (hw={1}Hz, game={2}Hz, adjusted={3}Hz)\n", target_speed / 10.0, minrefresh, ATTOSECONDS_TO_HZ(min_frame_period), ATTOSECONDS_TO_HZ((attoseconds_t)(min_frame_period * 1000.0 / target_speed)));
                        m_speed = target_speed;
                    }
                }
            }
        }

        //-------------------------------------------------
        //  recompute_speed - recompute the current
        //  overall speed; we assume this is called only
        //  if we did not skip a frame
        //-------------------------------------------------
        void recompute_speed(attotime emutime)
        {
            // if we don't have a starting time yet, or if we're paused, reset our starting point
            if (m_speed_last_realtime == 0 || machine().paused())
            {
                m_speed_last_realtime = m_osdcore.osd_ticks();
                m_speed_last_emutime = emutime;
            }

            // if it has been more than the update interval, update the time
            attotime delta_emutime = emutime - m_speed_last_emutime;
            if (delta_emutime > new attotime(0, ATTOSECONDS_PER_SPEED_UPDATE))
            {
                // convert from ticks to attoseconds
                osd_ticks_t realtime = m_osdcore.osd_ticks();
                osd_ticks_t delta_realtime = realtime - m_speed_last_realtime;
                osd_ticks_t tps = m_osdcore.osd_ticks_per_second();
                m_speed_percent = delta_emutime.as_double() * (double)tps / (double)delta_realtime;

                // remember the last times
                m_speed_last_realtime = realtime;
                m_speed_last_emutime = emutime;

                // if we're throttled, this time period counts for overall speed; otherwise, we reset the counter
                if (!m_fastforward)
                    m_overall_valid_counter++;
                else
                    m_overall_valid_counter = 0;

                // if we've had at least 4 consecutive valid periods, accumulate stats
                if (m_overall_valid_counter >= 4)
                {
                    m_overall_real_ticks += delta_realtime;
                    while (m_overall_real_ticks >= tps)
                    {
                        m_overall_real_ticks -= tps;
                        m_overall_real_seconds++;
                    }
                    m_overall_emutime += delta_emutime;
                }
            }

            // if we're past the "time-to-execute" requested, signal an exit
            if (m_seconds_to_run != 0 && emutime.seconds() >= m_seconds_to_run)
            {
                // create a final screenshot
                emu_file file = new emu_file(machine().options().snapshot_directory(), OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
                std.error_condition filerr = open_next(file, "png");
                if (!filerr)
                    save_snapshot(null, file);

                file.close();

                //printf("Scheduled exit at %f\n", emutime.as_double());

                // schedule our demise
                machine().schedule_exit();
            }
        }


        // snapshot/movie helpers

        //-------------------------------------------------
        //  create_snapshot_bitmap - creates a
        //  bitmap containing the screenshot for the
        //  given screen
        //-------------------------------------------------
        //typedef software_renderer<UINT32, 0,0,0, 16,8,0, false, true> snap_renderer_bilinear;
        //typedef software_renderer<UINT32, 0,0,0, 16,8,0, false, false> snap_renderer;

        void create_snapshot_bitmap(screen_device screen)
        {
            // select the appropriate view in our dummy target
            if (m_snap_native && screen != null)
            {
                screen_device_enumerator iter = new screen_device_enumerator(machine().root_device());
                int view_index = iter.indexof(screen);
                assert(view_index != -1);
                m_snap_target.set_view((unsigned)view_index);
            }

            // get the minimum width/height and set it on the target and bitmap
            s32 width;
            s32 height;
            compute_snapshot_size(out width, out height);
            m_snap_target.set_bounds(width, height);
            if (width != m_snap_bitmap.width() || height != m_snap_bitmap.height())
                m_snap_bitmap.resize(width, height);

            // render the screen there
            render_primitive_list primlist = m_snap_target.get_primitives();
            primlist.acquire_lock();
            if (machine().options().snap_bilinear())
            {
                //typedef software_renderer<u32, 0,0,0, 16,8,0, false, true> snap_renderer_bilinear;
                //snap_renderer_bilinear::draw_primitives(primlist, &m_snap_bitmap.pix(0), width, height, m_snap_bitmap.rowpixels());
                software_renderer<u32,  int_const_0, int_const_0, int_const_0,  int_const_16, int_const_8, int_const_0, bool_const_false, bool_const_true>.draw_primitives(primlist, m_snap_bitmap.pix(0), (u32)width, (u32)height, (u32)m_snap_bitmap.rowpixels());
            }
            else
            {
                //typedef software_renderer<u32, 0,0,0, 16,8,0, false, false> snap_renderer;
                //snap_renderer::draw_primitives(primlist, &m_snap_bitmap.pix(0), width, height, m_snap_bitmap.rowpixels());
                software_renderer<u32,  int_const_0, int_const_0, int_const_0,  int_const_16, int_const_8, int_const_0, bool_const_false, bool_const_false>.draw_primitives(primlist, m_snap_bitmap.pix(0), (u32)width, (u32)height, (u32)m_snap_bitmap.rowpixels());
            }
            primlist.release_lock();
        }


        //-------------------------------------------------
        //  compute_snapshot_size - computes width and
        //  height of the current snapshot target
        //  accounting for OPTION_SNAPSIZE
        //-------------------------------------------------
        void compute_snapshot_size(out s32 width, out s32 height)
        {
            width = m_snap_width;
            height = m_snap_height;
            if (width == 0 || height == 0)
                m_snap_target.compute_minimum_size(out width, out height);
        }


        //-------------------------------------------------
        //  record_frame - record a frame of a movie
        //-------------------------------------------------
        void record_frame()
        {
            // ignore if nothing to do
            if (!is_recording())
                return;

            // start the profiler and get the current time
            g_profiler.start(profile_type.PROFILER_MOVIE_REC);
            attotime curtime = machine().time();

            throw new emu_unimplemented();
        }


        // movies
        //void begin_recording_screen(const std::string &filename, uint32_t index, screen_device *screen, movie_recording::format format);


        static void video_notifier_callback(string outname, int value, object param)
        {
            video_manager vm = (video_manager)param;

            vm.set_output_changed();
        }
    }
}
