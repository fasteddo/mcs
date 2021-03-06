﻿// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
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
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            System.Diagnostics.Trace.WriteLine(string.Format("Environment.OSVersion.Platform: {0}", Environment.OSVersion.Platform));
            System.Diagnostics.Trace.WriteLine(string.Format("Environment.OSVersion.VersionString: {0}", Environment.OSVersion.VersionString));
            System.Diagnostics.Trace.WriteLine(string.Format("RuntimeInformation.FrameworkDescription: {0}", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription));


            Form1 form = new Form1();


            osdcore_WinForms osdcore = new osdcore_WinForms();
            osd_file_WinForms osdfile = new osd_file_WinForms();
            osd_directory_static_WinForms osddirectory = new osd_directory_static_WinForms();
            osd_options_WinForms options = new osd_options_WinForms();
            osd_interface_WinForms osd = new osd_interface_WinForms(options);
            osd.register_options();
            cli_frontend frontend = new cli_frontend(options, osd);

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
            //options.set_value(emu_options.OPTION_SYSTEMNAME, gamename, global_object.OPTION_PRIORITY_MAXIMUM);

            osdcore_global.set_osdcore(osdcore);
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
