// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using nvram_interface_enumerator = mame.device_interface_enumerator<mame.device_nvram_interface>;  //typedef device_interface_enumerator<device_nvram_interface> nvram_interface_enumerator;
using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using s64 = System.Int64;
using time_t = System.Int64;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;

using static mame.cpp_global;
using static mame.emumem_global;
using static mame.gamedrv_global;
using static mame.machine_global;
using static mame.main_global;
using static mame.osdcore_global;
using static mame.osdfile_global;
using static mame.profiler_global;


namespace mame
{
    // machine phases
    public enum machine_phase
    {
        PREINIT,
        INIT,
        RESET,
        RUNNING,
        EXIT
    }


    // notification callback types
    public enum machine_notification
    {
        MACHINE_NOTIFY_FRAME,
        MACHINE_NOTIFY_RESET,
        MACHINE_NOTIFY_PAUSE,
        MACHINE_NOTIFY_RESUME,
        MACHINE_NOTIFY_EXIT,
        MACHINE_NOTIFY_COUNT
    }


    public static class machine_global
    {
        // debug flags
        public const int DEBUG_FLAG_ENABLED        = 0x00000001;       // debugging is enabled
        public const int DEBUG_FLAG_CALL_HOOK      = 0x00000002;       // CPU cores must call instruction hook
        public const int DEBUG_FLAG_OSD_ENABLED    = 0x00001000;       // The OSD debugger is enabled
    }


    // ======================> system_time
    // system time description, both local and UTC
    public class system_time
    {
        struct full_time
        {
            //UINT8       second;     // seconds (0-59)
            //UINT8       minute;     // minutes (0-59)
            //UINT8       hour;       // hours (0-23)
            //UINT8       mday;       // day of month (1-31)
            //UINT8       month;      // month (0-11)
            //INT32       year;       // year (1=1 AD)
            //UINT8       weekday;    // day of week (0-6)
            //UINT16      day;        // day of year (0-365)
            //UINT8       is_dst;     // is this daylight savings?

            //void set(struct tm &t);
        }


        public s64 time;       // number of seconds elapsed since midnight, January 1 1970 UTC
        full_time local_time; // local time
        full_time utc_time;   // UTC coordinated time


        //-------------------------------------------------
        //  system_time - constructor
        //-------------------------------------------------
        public system_time()
        {
            set(0);
        }


        public system_time(time_t t)
        {
            set(t);
        }


        //-------------------------------------------------
        //  set - fills out a system_time structure
        //-------------------------------------------------
        public void set(time_t t)
        {
            // FIXME: this crashes if localtime or gmtime returns nullptr
            time = t;

            //throw new emu_unimplemented();
#if false
            local_time.set(*localtime(&t));
            utc_time.set(*gmtime(&t));
#endif
        }
    }


    // ======================> running_machine

    public delegate void machine_notify_delegate(running_machine machine);  //typedef delegate<void ()> machine_notify_delegate;

    // description of the currently-running machine
    public class running_machine : IDisposable
    {
        //DISABLE_COPYING(running_machine);

        //class side_effects_disabler;

        //friend class sound_manager;
        //friend class memory_manager;


        public delegate void logerror_callback(string format, params object [] args);  //typedef std::function<void (const char*)> logerror_callback;


        class side_effects_disabler
        {
            //running_machine *m_machine;
            //bool m_disable_se;

            //side_effect_disabler(running_machine *m, bool disable_se) : m_machine(m), m_disable_se(disable_se)
            //{
            //    if(m_disable_se)
            //        m_machine->disable_side_effect_count();
            //}

            //~side_effect_disabler()
            //{
            //    if(m_disable_se)
            //        m_machine->enable_side_effect_count();
            //}

            //side_effect_disabler(const side_effect_disabler &) = delete;
            //side_effect_disabler(side_effect_disabler &&) = default;
        }


        // side effect disable counter
        u32 m_side_effects_disabled;

        // debugger-related information
        public u32 debug_flags;        // the current debug flags

        // internal state
        machine_config m_config;               // reference to the constructed machine_config
        game_driver m_system;               // reference to the definition of the game machine
        machine_manager m_manager;              // reference to machine manager system

        // managers
        render_manager m_render;          // internal data from render.cpp
        input_manager m_input;            // internal data from input.cpp
        sound_manager m_sound;            // internal data from sound.cpp
        video_manager m_video;            // internal data from video.cpp
        ui_manager m_ui;                  // internal data from ui.cpp
        ui_input_manager m_ui_input;      // internal data from uiinput.cpp
        tilemap_manager m_tilemap;        // internal data from tilemap.cpp
        debug_view_manager m_debug_view;  // internal data from debugvw.cpp
        network_manager m_network;        // internal data from network.cpp
        bookkeeping_manager m_bookkeeping;// internal data from bookkeeping.cpp
        configuration_manager m_configuration; // internal data from config.cpp
        output_manager m_output;          // internal data from output.cpp
        crosshair_manager m_crosshair;    // internal data from crsshair.cpp
        image_manager m_image;            // internal data from image.cpp
        rom_load_manager m_rom_load;      // internal data from romload.cpp
        debugger_manager m_debugger;      // internal data from debugger.cpp
        natural_keyboard m_natkeyboard;   // internal data from natkeyboard.cpp

        // system state
        machine_phase m_current_phase;        // current execution phase
        bool m_paused;               // paused?
        bool m_hard_reset_pending;   // is a hard reset pending?
        bool m_exit_pending;         // is an exit pending?
        emu_timer m_soft_reset_timer;     // timer used to schedule a soft reset

        // misc state
        u32 m_rand_seed;            // current random number seed
        bool m_ui_active;            // ui active or not (useful for games / systems with keyboard inputs)
        time_t m_base_time;            // real time at initial emulation time
        string m_basename;             // basename used for game-related paths
        public int m_sample_rate;          // the digital audio sample rate
        emu_file m_logfile;              // pointer to the active error.log file
        emu_file m_debuglogfile;      // pointer to the active debug.log file

        // load/save management
        enum saveload_schedule
        {
            NONE,
            SAVE,
            LOAD
        }
        saveload_schedule m_saveload_schedule;
        attotime m_saveload_schedule_time;
        string m_saveload_pending_file;
        string m_saveload_searchpath;

        // notifier callbacks
        class notifier_callback_item
        {
            public machine_notify_delegate m_func;

            public notifier_callback_item(machine_notify_delegate func) { m_func = func; }
        }

        std.list<notifier_callback_item> [] m_notifier_list = new std.list<notifier_callback_item>[(int)machine_notification.MACHINE_NOTIFY_COUNT];  //std::list<std::unique_ptr<notifier_callback_item>> m_notifier_list[MACHINE_NOTIFY_COUNT];


        // logerror callbacks
        class logerror_callback_item
        {
            public logerror_callback m_func;

            public logerror_callback_item(logerror_callback func) { m_func = func; }
        }

        std.list<logerror_callback_item> m_logerror_list = new std.list<logerror_callback_item>();  //std::list<std::unique_ptr<logerror_callback_item>> m_logerror_list;


        // embedded managers and objects
        save_manager m_save;                 // save manager
        memory_manager m_memory;               // memory manager
        ioport_manager m_ioport;               // I/O port manager
        parameters_manager m_parameters;           // parameters manager
        device_scheduler m_scheduler;            // scheduler object

        // string formatting buffer
        string m_string_buffer;  //mutable util::ovectorstream m_string_buffer;


        // construction/destruction

        //-------------------------------------------------
        //  running_machine - constructor
        //-------------------------------------------------
        public running_machine(machine_config _config, machine_manager manager)
        {
            m_side_effects_disabled = 0;
            debug_flags = 0;
            m_config = _config;
            m_system = _config.gamedrv();
            m_manager = manager;
            m_current_phase = machine_phase.PREINIT;
            m_paused = false;
            m_hard_reset_pending = false;
            m_exit_pending = false;
            m_soft_reset_timer = null;
            m_rand_seed = 0x9d14abd7;
            m_ui_active = _config.options().ui_active();
            m_basename = _config.gamedrv().name;
            m_sample_rate = _config.options().sample_rate();
            m_saveload_schedule = saveload_schedule.NONE;
            m_saveload_schedule_time = attotime.zero;
            m_saveload_searchpath = null;

            m_save = new save_manager(this);
            m_memory = new memory_manager(this);
            m_ioport = new ioport_manager(this);
            m_scheduler = new device_scheduler(this);
            m_scheduler.device_scheduler_after_ctor(this);


            for (int i = 0; i < m_notifier_list.Length; i++)
                m_notifier_list[i] = new std.list<notifier_callback_item>();

            m_base_time = 0;

            // set the machine on all devices
            device_enumerator iter = new device_enumerator(root_device());
            foreach (device_t device in iter)
                device.set_machine(this);

            // fetch core options
            if (options().debug())
                debug_flags = (DEBUG_FLAG_ENABLED | DEBUG_FLAG_CALL_HOOK) | (DEBUG_FLAG_OSD_ENABLED);
        }

        ~running_machine()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            if (m_scheduler != null)
                m_scheduler.Dispose();
            m_scheduler = null;

            if (m_debugger != null)
                m_debugger.Dispose();
            m_debugger = null;

            if (m_tilemap != null)
                m_tilemap.Dispose();
            m_tilemap = null;

            if (m_render != null)
                m_render.Dispose();
            m_render = null;

            m_isDisposed = true;
        }


        // getters

        public machine_config config() { return m_config; }
        public device_t root_device() { return m_config.root_device(); }
        public game_driver system() { return m_system; }
        public osd_interface osd() { return m_manager.osd(); }
        machine_manager manager() { return m_manager; }
        public device_scheduler scheduler() { return m_scheduler; }
        public save_manager save() { return m_save; }
        public memory_manager memory() { return m_memory; }
        public ioport_manager ioport() { return m_ioport; }
        public parameters_manager parameters() { return m_parameters; }
        public render_manager render() { assert(m_render != null); return m_render; }
        public input_manager input() { assert(m_input != null); return m_input; }
        public sound_manager sound() { return m_sound; }
        public video_manager video() { assert(m_video != null); return m_video; }
        network_manager network() { assert(m_network != null); return m_network; }
        public bookkeeping_manager bookkeeping() { assert(m_bookkeeping != null); return m_bookkeeping; }
        public configuration_manager configuration() { assert(m_configuration != null); return m_configuration; }
        public output_manager output() { assert(m_output != null); return m_output; }
        public ui_manager ui() { assert(m_ui != null); return m_ui; }
        public ui_input_manager ui_input() { assert(m_ui_input != null); return m_ui_input; }
        public crosshair_manager crosshair() { assert(m_crosshair != null); return m_crosshair; }
        public image_manager image() { assert(m_image != null); return m_image; }
        public rom_load_manager rom_load() { assert(m_rom_load != null); return m_rom_load; }
        public tilemap_manager tilemap() { assert(m_tilemap != null); return m_tilemap; }
        //debug_view_manager &debug_view() const { assert(m_debug_view != NULL); return *m_debug_view; }
        public debugger_manager debugger() { assert(m_debugger != null); return m_debugger; }
        public natural_keyboard natkeyboard() { assert(m_natkeyboard != null); return m_natkeyboard; }
        public DriverClass driver_data<DriverClass>() where DriverClass : driver_device { return (DriverClass)root_device(); }
        public machine_phase phase() { return m_current_phase; }
        public bool paused() { return m_paused || (m_current_phase != machine_phase.RUNNING); }
        public bool exit_pending() { return m_exit_pending; }
        bool hard_reset_pending() { return m_hard_reset_pending; }
        public bool ui_active() { return m_ui_active; }
        public string basename() { return m_basename; }
        public int sample_rate() { return m_sample_rate; }
        //bool save_or_load_pending() const { return m_saveload_pending_file; }


        // RAII-based side effect disable
        // NOP-ed when passed false, to make it more easily conditional
        //side_effects_disabler disable_side_effects(bool disable_se = true) { return side_effects_disabler(this, disable_se); }
        public bool side_effects_disabled() { return m_side_effects_disabled != 0; }


        // additional helpers
        public emu_options options() { return m_config.options(); }
        public attotime time() { return m_scheduler.time(); }
        public bool scheduled_event_pending() { return m_exit_pending || m_hard_reset_pending; }
        public bool allow_logging() { return !m_logerror_list.empty(); }


        public void sample_rate_set(int value) { m_sample_rate = value; }


        // fetch items by name
        //template <class DeviceClass> [[deprecated("absolute tag lookup; use subdevice or finder instead")]] inline DeviceClass *device(const char *tag) { return downcast<DeviceClass *>(root_device().subdevice(tag)); }


        // immediate operations

        //-------------------------------------------------
        //  run - execute the machine
        //-------------------------------------------------
        public int run(bool quiet)
        {
            int error = EMU_ERR_NONE;

            // use try/catch for deep error recovery
            //throw new emu_unimplemented();
#if false
            try
#endif
            {
                m_manager.http().clear();

                // move to the init phase
                m_current_phase = machine_phase.INIT;

                // if we have a logfile, set up the callback
                if (options().log() && !quiet)
                {
                    m_logfile = new emu_file(OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
                    std.error_condition filerr = m_logfile.open("error.log");
                    if (filerr)
                        throw new emu_fatalerror("running_machine::run: unable to open error.log file");

                    //using namespace std::placeholders;
                    add_logerror_callback(logfile_callback);
                }

                if (options().debug() && options().debuglog())
                {
                    m_debuglogfile = new emu_file(OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
                    std.error_condition filerr = m_debuglogfile.open("debug.log");
                    if (filerr)
                        throw new emu_fatalerror("running_machine::run: unable to open debug.log file");
                }

                // then finish setting up our local machine
                start();

                // load the configuration settings
                manager().before_load_settings(this);
                m_configuration.load_settings();

                // disallow save state registrations starting here.
                // Don't do it earlier, config load can create network
                // devices with timers.
                m_save.allow_registration(false);

                // load the NVRAM
                nvram_load();

                // set the time on RTCs (this may overwrite parts of NVRAM)
                set_rtc_datetime(new system_time(m_base_time));

                sound().ui_mute(false);
                if (!quiet)
                    sound().start_recording();

                m_hard_reset_pending = false;

                // initialize ui lists
                // display the startup screens
                manager().ui_initialize(this);

                // perform a soft reset -- this takes us to the running phase
                soft_reset();

                // handle initial load
                if (m_saveload_schedule != saveload_schedule.NONE)
                    handle_saveload();

                export_http_api();

#if EMSCRIPTEN
                // break out to our async javascript loop and halt
                emscripten_set_running_machine(this);
#endif

                // run the CPUs until a reset or exit
                while ((!m_hard_reset_pending && !m_exit_pending) || m_saveload_schedule != saveload_schedule.NONE)
                {
                    g_profiler.start(profile_type.PROFILER_EXTRA);

                    // execute CPUs if not paused
                    if (!m_paused)
                        m_scheduler.timeslice();
                    // otherwise, just pump video updates through
                    else
                        m_video.frame_update();

                    // handle save/load
                    if (m_saveload_schedule != saveload_schedule.NONE)
                        handle_saveload();

                    g_profiler.stop();
                }

                m_manager.http().clear();

                // and out via the exit phase
                m_current_phase = machine_phase.EXIT;

                // save the NVRAM and configuration
                sound().ui_mute(true);
                if (options().nvram_save())
                    nvram_save();
                m_configuration.save_settings();
            }
#if false
            catch (emu_fatalerror fatal)
            {
                osd_printf_error("Fatal error: %s\n", fatal.what());
                error = EMU_ERR_FATALERROR;
                if (fatal.exitcode() != 0)
                    error = fatal.exitcode();
            }
            catch (emu_exception)
            {
                osd_printf_error("Caught unhandled emulator exception\n");
                error = machine_manager.MAMERR.MAMERR_FATALERROR;
            }
            catch (binding_type_exception)// btex)
            {
                osd_printf_error("Error performing a late bind of function expecting type %s to instance of type %s\n", btex.target_type().name(), btex.actual_type().name());
                error = machine_manager.MAMERR.MAMERR_FATALERROR;
            }
            catch (tag_add_exception &aex)
            {
                osd_printf_error("Tag '%s' already exists in tagged map\n", aex.tag());
                error = EMU_ERR_FATALERROR;
            }
            catch (std::exception &ex)
            {
                osd_printf_error("Caught unhandled %s exception: %s\n", typeid(ex).name(), ex.what());
                error = EMU_ERR_FATALERROR;
            }
            catch (...)
            {
                osd_printf_error("Caught unhandled exception\n");
                error = EMU_ERR_FATALERROR;
            }
#endif
            //throw new emu_unimplemented();
#if false
            catch (Exception e)
            {
                OsdCore.osd_printf_error("Caught unhandled exception\n" + e);
                error = machine_manager.MAMERR.MAMERR_FATALERROR;
            }
            finally
            {
            }
#endif

            // make sure our phase is set properly before cleaning up,
            // in case we got here via exception
            m_current_phase = machine_phase.EXIT;

            // call all exit callbacks registered
            call_notifiers(machine_notification.MACHINE_NOTIFY_EXIT);
            util.archive_file.cache_clear();

            // close the logfile
            if (m_logfile != null)
                m_logfile.close();
            m_logfile = null;

            return error;
        }

        //-------------------------------------------------
        //  pause - pause the system
        //-------------------------------------------------
        public void pause()
        {
            // ignore if nothing has changed
            if (m_paused)
                return;

            m_paused = true;

            // call the callbacks
            call_notifiers(machine_notification.MACHINE_NOTIFY_PAUSE);
        }

        //-------------------------------------------------
        //  resume - resume the system
        //-------------------------------------------------
        public void resume()
        {
            // ignore if nothing has changed
            if (!m_paused)
                return;

            m_paused = false;

            // call the callbacks
            call_notifiers(machine_notification.MACHINE_NOTIFY_RESUME);
        }

        //-------------------------------------------------
        //  toggle_pause - toggles the pause state
        //-------------------------------------------------
        public void toggle_pause()
        {
            if (paused())
            {
                rewind_invalidate();
                resume();
            }
            else
            {
                pause();
            }
        }

        //-------------------------------------------------
        //  add_notifier - add a notifier of the
        //  given type
        //-------------------------------------------------
        public void add_notifier(machine_notification notification_event, machine_notify_delegate callback, bool first = false)
        {
            if (m_current_phase != machine_phase.INIT)
                throw new emu_fatalerror("Can only call running_machine::add_notifier at init time!");

            if (first)
                m_notifier_list[(int)notification_event].push_front(new notifier_callback_item(callback));

            // exit notifiers are added to the head, and executed in reverse order
            else if (notification_event == machine_notification.MACHINE_NOTIFY_EXIT)
                m_notifier_list[(int)notification_event].push_front(new notifier_callback_item(callback));

            // all other notifiers are added to the tail, and executed in the order registered
            else
                m_notifier_list[(int)notification_event].push_back(new notifier_callback_item(callback));
        }

        //-------------------------------------------------
        //  call_notifiers - call notifiers of the given
        //  type
        //-------------------------------------------------
        public void call_notifiers(machine_notification which)
        {
            foreach (var cb in m_notifier_list[(int)which])
                cb.m_func(this);
        }

        //-------------------------------------------------
        //  add_logerror_callback - adds a callback to be
        //  called on logerror()
        //-------------------------------------------------
        public void add_logerror_callback(logerror_callback callback)
        {
            if (m_current_phase != machine_phase.INIT)
                throw new emu_fatalerror("Can only call running_machine::add_logerror_callback at init time!");

            //m_string_buffer.reserve(1024);
            m_logerror_list.push_back(new logerror_callback_item(callback));
        }


        public void set_ui_active(bool active) { m_ui_active = active; }


        //void debug_break();


        void export_http_api()
        {
            //throw new emu_unimplemented();
        }


        // TODO: Do saves and loads still require scheduling?
        //void immediate_save(std::string_view filename);
        //void immediate_load(std::string_view filename);


        // rewind operations

        //-------------------------------------------------
        //  rewind_capture - capture and append a new
        //  state to the rewind list
        //-------------------------------------------------
        public bool rewind_capture()
        {
            throw new emu_unimplemented();
#if false
            return m_save.rewind().capture();
#endif
        }

        //-------------------------------------------------
        //  rewind_step - a single step back through
        //  rewind states
        //-------------------------------------------------
        public bool rewind_step()
        {
            throw new emu_unimplemented();
#if false
            return m_save.rewind().step();
#endif
        }

        //-------------------------------------------------
        //  rewind_invalidate - mark all the future rewind
        //  states as invalid
        //-------------------------------------------------
        void rewind_invalidate()
        {
            throw new emu_unimplemented();
#if false
            m_save.rewind().invalidate();
#endif
        }


        // scheduled operations

        //-------------------------------------------------
        //  schedule_exit - schedule a clean exit
        //-------------------------------------------------
        public void schedule_exit()
        {
            m_exit_pending = true;

            // if we're executing, abort out immediately
            m_scheduler.eat_all_cycles();

            // if we're autosaving on exit, schedule a save as well
            if (options().autosave() && ((u64)m_system.flags & MACHINE_SUPPORTS_SAVE) != 0 && this.time() > attotime.zero)
                schedule_save("auto");
        }

        //-------------------------------------------------
        //  schedule_hard_reset - schedule a hard-reset of
        //  the machine
        //-------------------------------------------------
        public void schedule_hard_reset()
        {
            m_hard_reset_pending = true;

            // if we're executing, abort out immediately
            m_scheduler.eat_all_cycles();
        }

        //-------------------------------------------------
        //  schedule_soft_reset - schedule a soft-reset of
        //  the system
        //-------------------------------------------------
        public void schedule_soft_reset()
        {
            m_soft_reset_timer.adjust(attotime.zero);

            // we can't be paused since the timer needs to fire
            resume();

            // if we're executing, abort out immediately
            m_scheduler.eat_all_cycles();
        }


        //-------------------------------------------------
        //  schedule_save - schedule a save to occur as
        //  soon as possible
        //-------------------------------------------------
        void schedule_save(string filename)
        {
            throw new emu_unimplemented();
#if false
            // specify the filename to save or load
            set_saveload_filename(filename);
#endif
            // note the start time and set a timer for the next timeslice to actually schedule it
            m_saveload_schedule = saveload_schedule.SAVE;
            m_saveload_schedule_time = time();

            // we can't be paused since we need to clear out anonymous timers
            resume();
        }


        //-------------------------------------------------
        //  schedule_load - schedule a load to occur as
        //  soon as possible
        //-------------------------------------------------
        void schedule_load(string filename)
        {
            throw new emu_unimplemented();
#if false
            // specify the filename to save or load
            set_saveload_filename(filename);
#endif

            // note the start time and set a timer for the next timeslice to actually schedule it
            m_saveload_schedule = saveload_schedule.LOAD;
            m_saveload_schedule_time = time();

            // we can't be paused since we need to clear out anonymous timers
            resume();
        }


        // date & time

        //-------------------------------------------------
        //  base_datetime - retrieve the time of the host
        //  system; useful for RTC implementations
        //-------------------------------------------------
        public void base_datetime(out system_time systime)
        {
            systime = new system_time();
            systime.set(m_base_time);
        }


        //void current_datetime(system_time &systime);


        //-------------------------------------------------
        //  set_rtc_datetime - set the current time on
        //  battery-backed RTCs
        //-------------------------------------------------
        void set_rtc_datetime(system_time systime)
        {
            //throw new emu_unimplemented();
#if false
            foreach (device_rtc_interface rtc in new rtc_interface_iterator(root_device()))
            {
                if (rtc.has_battery())
                    rtc.set_current_time(systime);
            }
#endif
        }


        /*-------------------------------------------------
            popmessage - pop up a user-visible message
        -------------------------------------------------*/
        public void popmessage() { popmessage(""); }

        public void popmessage(string format, params object [] args)
        {
            // if the format is NULL, it is a signal to clear the popmessage
            if (format == null)
            {
                ui().popup_time(0, " ");
            }

            // otherwise, generate the buffer and call the UI to display the message
            else
            {
                string temp;
                //va_list arg;

                // dump to the buffer
                //va_start(arg, format);
                //strvprintf(temp,format, arg);
                //va_end(arg);

                temp = string.Format(format, args);

                // pop it in the UI
                ui().popup_time(temp.Length / 40 + 2, temp);
            }
        }


        /*-------------------------------------------------
            logerror - log to the debugger and any other
            OSD-defined output streams
        -------------------------------------------------*/
        public void logerror(string format, params object [] args)
        {
            // process only if there is a target
            if (allow_logging())
            {
                g_profiler.start(profile_type.PROFILER_LOGERROR);

                // dump to the buffer
                //m_string_buffer.clear();
                //m_string_buffer.seekp(0);
                //util::stream_format(m_string_buffer, std::forward<Format>(fmt), std::forward<Params>(args)...);
                //m_string_buffer.put('\0');
                m_string_buffer = string.Format(format, args);

                strlog(m_string_buffer);

                g_profiler.stop();
            }
        }


        //-------------------------------------------------
        //  strlog - send an error logging string to the
        //  debugger and any OSD-defined output streams
        //-------------------------------------------------
        public void strlog(string str)
        {
            // log to all callbacks
            foreach (var cb in m_logerror_list)
                cb.m_func(str);
        }


        //-------------------------------------------------
        //  rand - standardized random numbers
        //-------------------------------------------------
        // TODO: using this function in the core is strongly discouraged (can affect inp playback),
        //       maybe we should consider moving this function to somewhere else instead.
        public u32 rand()
        {
            m_rand_seed = 1664525 * m_rand_seed + 1013904223;

            // return rotated by 16 bits; the low bits have a short period
            // and are frequently used
            return (m_rand_seed >> 16) | (m_rand_seed << 16);
        }

        //-------------------------------------------------
        //  describe_context - return a string describing
        //  which device is currently executing and its
        //  PC
        //-------------------------------------------------
        public string describe_context()
        {
            device_execute_interface executing = m_scheduler.currently_executing();
            if (executing != null)
            {
                cpu_device cpu = executing.device() is cpu_device cpu_device ? cpu_device : null;
                if (cpu != null)
                {
                    address_space prg = cpu.memory().space(AS_PROGRAM);
                    return util.string_format(prg.is_octal() ? "'{0}' ({1})" :  "'{0}' ({1})", cpu.tag(), prg.logaddrchars(), cpu.GetClassInterface<device_state_interface>().pc());  // "'%s' (%0*o)" :  "'%s' (%0*X)"
                }
            }

            return "(no context)";
        }


        //std::string compose_saveload_filename(const char *base_filename, const char **searchpath = nullptr);
        //std::string get_statename(const char *statename_opt) const;


        //bool debug_enabled() { return (debug_flags & DEBUG_FLAG_ENABLED) != 0; }

        // used by debug_console to take ownership of the debug.log file
        //std::unique_ptr<emu_file> steal_debuglogfile();


        //void disable_side_effects_count() { m_side_effects_disabled++; }
        //void enable_side_effects_count()  { m_side_effects_disabled--; }


        // internal helpers

        //template <typename T> struct is_null { template <typename U> static bool value(U &&x) { return false; } };
        //template <typename T> struct is_null<T *> { template <typename U> static bool value(U &&x) { return !x; } };


        //-------------------------------------------------
        //  start - initialize the emulated machine
        //-------------------------------------------------
        void start()
        {
            // initialize basic can't-fail systems here
            m_configuration = new configuration_manager(this);
            m_input = new input_manager(this);
            m_output = new output_manager(this);
            m_render = new render_manager(this);
            m_bookkeeping = new bookkeeping_manager(this);

            // allocate a soft_reset timer
            m_soft_reset_timer = m_scheduler.timer_alloc(soft_reset);

            // intialize UI input
            m_ui_input = new ui_input_manager(this);

            // init the osd layer
            m_manager.osd().init(this);

            // create the video manager
            m_video = new video_manager(this);
            m_ui = m_manager.create_ui(this);

            //throw new emu_unimplemented();
#if false
            // initialize the base time (needed for doing record/playback)
            ::time(&m_base_time);
#endif

            // initialize the input system and input ports for the game
            // this must be done before memory_init in order to allow specifying
            // callbacks based on input port tags
            time_t newbase = m_ioport.initialize();
            if (newbase != 0)
                m_base_time = newbase;

            // initialize natural keyboard support after ports have been initialized
            m_natkeyboard = new natural_keyboard(this);

            // initialize the streams engine before the sound devices start
            m_sound = new sound_manager(this);

            // resolve objects that can be used by memory maps
            foreach (device_t device in new device_enumerator(root_device()))
                device.resolve_pre_map();

            // configure the address spaces, load ROMs (which needs
            // width/endianess of the spaces), then populate memory (which
            // needs rom bases), and finally initialize CPUs (which needs
            // complete address spaces).  These operations must proceed in this
            // order
            m_rom_load = new rom_load_manager(this);
            m_memory.initialize();

            // save the random seed or save states might be broken in drivers that use the rand() method
            save().save_item(m_rand_seed, "m_rand_seed");

            // initialize image devices
            m_image = new image_manager(this);
            m_tilemap = new tilemap_manager(this);
            m_crosshair = new crosshair_manager(this);
            m_network = new network_manager(this);

            // initialize the debugger
            if ((debug_flags & DEBUG_FLAG_ENABLED) != 0)
            {
                m_debug_view = new debug_view_manager(this);
                m_debugger = new debugger_manager(this);
            }

            manager().create_custom(this);

            // resolve objects that are created by memory maps
            foreach (device_t device in new device_enumerator(root_device()))
                device.resolve_post_map();

            // register callbacks for the devices, then start them
            add_notifier(machine_notification.MACHINE_NOTIFY_RESET, reset_all_devices);
            add_notifier(machine_notification.MACHINE_NOTIFY_EXIT, stop_all_devices);
            save().register_presave(presave_all_devices);
            start_all_devices();
            save().register_postload(postload_all_devices);

            // save outputs created before start time
            output().register_save();

            m_render.resolve_tags();

            // load cheat files
            manager().load_cheatfiles(this);

            // start recording movie if specified
            string filename = options().mng_write();
            if (!string.IsNullOrEmpty(filename))
                m_video.begin_recording(filename, movie_recording.format.MNG);

            filename = options().avi_write();
            if (!string.IsNullOrEmpty(filename) && !m_video.is_recording())
                m_video.begin_recording(filename, movie_recording.format.AVI);

            // if we're coming in with a savegame request, process it now
            string savegame = options().state();
            if (!string.IsNullOrEmpty(savegame))
                schedule_load(savegame);

            // if we're in autosave mode, schedule a load
            else if (options().autosave() && ((u64)m_system.flags & MACHINE_SUPPORTS_SAVE) != 0)
                schedule_load("auto");

            manager().update_machine();
        }


        //void set_saveload_filename(const char *filename);


        void handle_saveload()
        {
            //throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  soft_reset - actually perform a soft-reset
        //  of the system
        //-------------------------------------------------
        void soft_reset(s32 param = 0)
        {
            logerror("Soft reset\n");

            // temporarily in the reset phase
            m_current_phase = machine_phase.RESET;

            // call all registered reset callbacks
            call_notifiers(machine_notification.MACHINE_NOTIFY_RESET);

            // now we're running
            m_current_phase = machine_phase.RUNNING;
        }


        /*-------------------------------------------------
            nvram_filename - returns filename of system's
            NVRAM depending of selected BIOS
        -------------------------------------------------*/
        string nvram_filename(device_t device)
        {
            // start with either basename or basename_biosnum
            string result;  //std::ostringstream result;
            result = basename();
            if (root_device().system_bios() != 0 && root_device().default_bios() != root_device().system_bios())
                result += string.Format("_{0}", root_device().system_bios() - 1);

            // device-based NVRAM gets its own name in a subdirectory
            if (device.owner() != null)
            {
                // add per software nvrams into one folder
                string software = null;
                for (device_t dev = device; dev.owner() != null; dev = dev.owner())
                {
                    device_image_interface intf;
                    if (dev.interface_(out intf))
                    {
                        software = intf.basename_noext();
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(software))
                    result += PATH_SEPARATOR + software;

                string tag = device.tag();
                tag = tag.Remove(0, 1);
                tag = tag.Replace(':', '_');
                result += PATH_SEPARATOR + tag;
            }

            return result;
        }


        /*-------------------------------------------------
            nvram_load - load a system's NVRAM
        -------------------------------------------------*/
        void nvram_load()
        {
            foreach (device_nvram_interface nvram in new nvram_interface_enumerator(root_device()))
            {
                emu_file file = new emu_file(options().nvram_directory(), OPEN_FLAG_READ);
                if (nvram.nvram_backup_enabled() && !file.open(nvram_filename(nvram.device())))
                {
                    if (!nvram.nvram_load(file.core_file_))
                        osd_printf_error("Error reading NVRAM file {0}\n", file.filename());

                    file.close();
                }
                else
                {
                    nvram.nvram_reset();
                }
            }
        }

        /*-------------------------------------------------
            nvram_save - save a system's NVRAM
        -------------------------------------------------*/
        void nvram_save()
        {
            foreach (device_nvram_interface nvram in new nvram_interface_enumerator(root_device()))
            {
                if (nvram.nvram_can_save())
                {
                    emu_file file = new emu_file(options().nvram_directory(), OPEN_FLAG_WRITE | OPEN_FLAG_CREATE | OPEN_FLAG_CREATE_PATHS);
                    if (!file.open(nvram_filename(nvram.device())))
                    {
                        if (!nvram.nvram_save(file.core_file_))
                            osd_printf_error("Error writing NVRAM file {0}\n", file.filename());

                        file.close();
                    }
                }
            }
        }


        //void popup_clear() const;
        //void popup_message(util::format_argument_pack<std::ostream> const &args) const;


        // internal callbacks
        //-------------------------------------------------
        //  logfile_callback - callback for logging to
        //  logfile
        //-------------------------------------------------
        void logfile_callback(string format, params object [] args)
        {
            if (m_logfile != null)
            {
                m_logfile.puts(string.Format(format, args));
                m_logfile.flush();
            }
        }


        // internal device helpers

        //-------------------------------------------------
        //  start_all_devices - start any unstarted devices
        //-------------------------------------------------
        void start_all_devices()
        {
            // iterate through the devices
            int last_failed_starts = -1;
            do
            {
                // iterate over all devices
                int failed_starts = 0;
                foreach (device_t device in new device_enumerator(root_device()))
                {
                    if (!device.started())
                    {
                        // attempt to start the device, catching any expected exceptions
                        try
                        {
                            // if the device doesn't have a machine yet, set it first
                            if (device.machine() == null)
                                device.set_machine(this);

                            // now start the device
                            osd_printf_verbose("Starting {0} '{1}'\n", device.name(), device.tag());
                            device.start();
                        }
                        catch (device_missing_dependencies)
                        {
                            // handle missing dependencies by moving the device to the end
                            osd_printf_verbose("  (missing dependencies; rescheduling)\n");
                            failed_starts++;
                        }
                    }
                }

                // each iteration should reduce the number of failed starts; error if this doesn't happen
                if (failed_starts == last_failed_starts)
                    throw new emu_fatalerror("Circular dependency in device startup!");

                last_failed_starts = failed_starts;
            }
            while (last_failed_starts != 0);
        }

        //-------------------------------------------------
        //  reset_all_devices - reset all devices in the
        //  hierarchy
        //-------------------------------------------------
        void reset_all_devices(running_machine machine_)
        {
            // reset the root and it will reset children
            root_device().reset();
        }

        //-------------------------------------------------
        //  stop_all_devices - stop all the devices in the
        //  hierarchy
        //-------------------------------------------------
        void stop_all_devices(running_machine machine_)
        {
            //throw new emu_unimplemented();
#if false
            // first let the debugger save comments
            if ((debug_flags & DEBUG_FLAG_ENABLED) != 0)
                debugger().cpu().comment_save();
#endif

            // iterate over devices and stop them
            foreach (device_t device in new device_enumerator(root_device()))
                device.stop();
        }

        //-------------------------------------------------
        //  presave_all_devices - tell all the devices we
        //  are about to save
        //-------------------------------------------------
        void presave_all_devices()
        {
            foreach (device_t device in new device_enumerator(root_device()))
                device.pre_save();
        }

        //-------------------------------------------------
        //  postload_all_devices - tell all the devices we
        //  just completed a load
        //-------------------------------------------------
        void postload_all_devices()
        {
            foreach (device_t device in new device_enumerator(root_device()))
                device.post_load();
        }
    }
}
