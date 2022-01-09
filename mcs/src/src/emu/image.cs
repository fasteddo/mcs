// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using image_interface_enumerator = mame.device_interface_enumerator<mame.device_image_interface>;  //typedef device_interface_enumerator<device_image_interface> image_interface_enumerator;

using static mame.cpp_global;
using static mame.main_global;
using static mame.osdfile_global;


namespace mame
{
    // ======================> image_manager
    public class image_manager
    {
        // internal state
        running_machine m_machine;                  // reference to our machine


        // construction/destruction
        public image_manager(running_machine machine)
        {
            m_machine = machine;


            // make sure that any required devices have been allocated
            foreach (device_image_interface image in new image_interface_enumerator(machine.root_device()))
            {
                // ignore things not user loadable
                if (!image.user_loadable())
                    continue;

                throw new emu_unimplemented();
            }

            machine.configuration().config_register("image_directories", config_load, config_save);
        }


        //-------------------------------------------------
        //  unload_all - unload all images and
        //  extract options
        //-------------------------------------------------
        void unload_all(running_machine machine_)
        {
            // extract the options
            options_extract();

            foreach (device_image_interface image in new image_interface_enumerator(machine().root_device()))
            {
                // unload this image
                image.unload();
            }
        }


        /*-------------------------------------------------
            postdevice_init - initialize devices for a specific
            running_machine
        -------------------------------------------------*/
        public void postdevice_init()
        {
            /* make sure that any required devices have been allocated */
            foreach (device_image_interface image in new image_interface_enumerator(machine().root_device()))
            {
                image_init_result result = image.finish_load();

                /* did the image load fail? */
                if (result != image_init_result.PASS)
                {
                    /* retrieve image error message */
                    string image_err = image.error();

                    /* unload all images */
                    unload_all(machine());

                    throw new emu_fatalerror(EMU_ERR_DEVICE, "Device {0} load failed: {1}",
                        image.device().name(),
                        image_err);
                }
            }

            /* add a callback for when we shut down */
            machine().add_notifier(machine_notification.MACHINE_NOTIFY_EXIT, unload_all);
        }


        // getters
        running_machine machine() { return m_machine; }


        //std::string setup_working_directory();


        void config_load(config_type cfg_type, config_level cfg_level, util.xml.data_node parentnode)
        {
            if ((cfg_type == config_type.SYSTEM) && (parentnode != null))
            {
                throw new emu_unimplemented();
            }
        }


        /*-------------------------------------------------
            config_save - saves out image device
            directories to the configuration file
        -------------------------------------------------*/
        void config_save(config_type cfg_type, util.xml.data_node parentnode)
        {
            // only save system-specific data
            if (cfg_type == config_type.SYSTEM)
            {
                foreach (device_image_interface image in new image_interface_enumerator(machine().root_device()))
                {
                    string dev_instance = image.instance_name();

                    throw new emu_unimplemented();
                }
            }
        }


        /*-------------------------------------------------
            options_extract - extract device options
            out of core into the options
        -------------------------------------------------*/
        void options_extract()
        {
            foreach (device_image_interface image in new image_interface_enumerator(machine().root_device()))
            {
                // There are two scenarios where we want to extract the option:
                //
                //  1.  When for the device, is_reset_on_load() is false (mounting devices for which is reset_and_load()
                //      is true forces a reset, hence the name)
                //
                //  2.  When is_reset_on_load(), and this results in a device being unmounted (unmounting is_reset_and_load()
                //      doesn't force an unmount).
                //
                //      Note that as a part of #2, we cannot extract the option when the image in question is a part of an
                //      active reset_on_load; hence the check for is_reset_and_loading() (see issue #2414)
                if (!image.is_reset_on_load()
                    || (!image.exists() && !image.is_reset_and_loading()
                        && machine().options().has_image_option(image.instance_name()) && !machine().options().image_option(image.instance_name()).value().empty()))
                {
                    // we have to assemble the image option differently for software lists and for normal images
                    string image_opt = "";
                    if (image.exists())
                    {
                        if (!image.loaded_through_softlist())
                            image_opt = image.filename();
                        else if (image.part_entry() != null && !image.part_entry().name().empty())
                            image_opt = util.string_format("{0}:{1}:{2}", image.software_list_name(), image.full_software_name(), image.part_entry().name());
                        else
                            image_opt = util.string_format("{0}:{1}", image.software_list_name(), image.full_software_name());
                    }

                    // and set the option (provided that it hasn't been removed out from under us)
                    if (machine().options().exists(image.instance_name()) && machine().options().has_image_option(image.instance_name()))
                        machine().options().image_option(image.instance_name()).specify(image_opt);
                }
            }

            // write the config, if appropriate
            if (machine().options().write_config())
                write_config(machine().options(), null, machine().system());
        }


        /*-------------------------------------------------
            write_config - emit current option statuses as
            INI files
        -------------------------------------------------*/
        int write_config(emu_options options, string filename, game_driver gamedrv)
        {
            string buffer;
            int retval = 1;

            if (gamedrv != null)
            {
                sprintf(out buffer, "{0}.ini", gamedrv.name);
                filename = buffer;
            }

            emu_file file = new emu_file(options.ini_path(), OPEN_FLAG_WRITE | OPEN_FLAG_CREATE);
            std.error_condition filerr = file.open(filename);
            if (!filerr)
            {
                string inistring = options.output_ini();
                file.puts(inistring);
                retval = 0;
            }
            file.close();

            return retval;
        }


        //bool try_change_working_directory(std::string &working_directory, const std::string &subdir);;
    }
}
