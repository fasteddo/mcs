using System;
using System.Diagnostics;
using mame;


namespace mcsConsole
{
    static class Program
    {
        // for redirecting to console
        class execute_commands_output : osd_output, IDisposable
        {
            public execute_commands_output()
            {
                osd_output.push(this);
            }

            public void Dispose()
            {
                osd_output.pop(this);
            }

            public override void output_callback(osd_output_channel channel, string format, params object [] args)  //virtual void output_callback(osd_output_channel channel, const util::format_argument_pack<std::ostream> &args) override;
            {
                switch (channel)
                {
                    case osd_output_channel.OSD_OUTPUT_CHANNEL_ERROR:
                    case osd_output_channel.OSD_OUTPUT_CHANNEL_WARNING:
                    case osd_output_channel.OSD_OUTPUT_CHANNEL_INFO:
                    case osd_output_channel.OSD_OUTPUT_CHANNEL_DEBUG:
                    case osd_output_channel.OSD_OUTPUT_CHANNEL_VERBOSE:
                    case osd_output_channel.OSD_OUTPUT_CHANNEL_LOG:
                        Debug.Write(string.Format(format, args));
                        Console.Write(format, args);
                        break;

                    default:
                        chain_output(channel, format, args);
                        break;
                }
            }
        }


        [STAThread]
        static int Main(string[] args)
        {
            Console.WriteLine("\n");
            Console.WriteLine("Environment.OSVersion.Platform: {0}", Environment.OSVersion.Platform);
            Console.WriteLine("Environment.OSVersion.VersionString: {0}", Environment.OSVersion.VersionString);
            Console.WriteLine("RuntimeInformation.FrameworkDescription: {0}", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
            Console.WriteLine("\n");

            drivlist_global_generated.init();
            object_finder_operations_global.init();
            samples_device_enumerator_helper_samples.init();
            cassette_device_enumerator_helper_cassette.init();
            nld_sound_in_helper_global.init();

            osdcore_WinForms osdcore = new();
            osd_file_WinForms osdfile = new();
            osd_directory_static_WinForms osddirectory = new();
            osd_options_WinForms options = new();
            osd_interface_WinForms osd = new(options);
            using var output = new execute_commands_output();
            osd.register_options();
            cli_frontend frontend = new(options, osd);

            // to jump to a specific game on startup
            //string gamename = "___empty";
            //string gamename = "puckman";
            //string gamename = "pacman";
            //string gamename = "mspacman";
            //string gamename = "pacplus";
            //string gamename = "galaga";
            //string gamename = "xevious";
            //string gamename = "digdug";
            //string gamename = "1942";
            //string gamename = "paperboy";
            //options.set_value(emu_options.OPTION_SYSTEMNAME, gamename, g.OPTION_PRIORITY_MAXIMUM);

            osdcore_global.set_osdcore(osdcore);
            osdfile_global.set_osdfile(osdfile);
            osdfile_global.set_osddirectory(osddirectory);
            mame_machine_manager.instance(options, osd);

            string [] command_line_args = Environment.GetCommandLineArgs();
            if (command_line_args.Length == 1)
                command_line_args = new string [] { command_line_args[0], "-help" };

            int ret = frontend.execute(new std.vector<string>(command_line_args));

            mame_machine_manager.close_instance();
            osd.Dispose();

            return ret;
        }
    }
}
