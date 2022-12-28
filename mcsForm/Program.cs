// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using mame;


namespace mcsForm
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // see https://aka.ms/applicationconfiguration
            ApplicationConfiguration.Initialize();


            Trace.WriteLine(string.Format("Environment.OSVersion.Platform: {0}", Environment.OSVersion.Platform));
            Trace.WriteLine(string.Format("Environment.OSVersion.VersionString: {0}", Environment.OSVersion.VersionString));
            Trace.WriteLine(string.Format("RuntimeInformation.FrameworkDescription: {0}", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription));


            Form1 form = new();

            drivlist_global_generated.init();
            object_finder_operations_global.init();
            samples_device_enumerator_helper_samples.init();
            cassette_device_enumerator_helper_cassette.init();
            nld_sound_in_helper_global.init();

            osdcore_WinForms osdcore = new();
            osdlib_WinForms osdlib = new();
            osd_file_WinForms osdfile = new();
            osd_directory_static_WinForms osddirectory = new();
            osd_options_WinForms options = new();
            osd_interface_WinForms osd = new(options);
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
            osdlib_global.set_osdlib(osdlib);
            osdfile_global.set_osdfile(osdfile);
            osdfile_global.set_osddirectory(osddirectory);
            mame_machine_manager.instance(options, osd);

            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true; 
                Thread.CurrentThread.Name = "machine_manager.execute()";

                int ret = frontend.execute(new std.vector<string>(Environment.GetCommandLineArgs()));

                // tell form that it should close
                form.Invoke((MethodInvoker)delegate { form.Close(); });
            }).Start();


            Application.Run(form);

            mame_machine_manager.close_instance();
            osd.Dispose();
        }
    }
}
