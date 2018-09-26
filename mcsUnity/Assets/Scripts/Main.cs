// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

using osd_ticks_t = System.UInt64;


public class Main : MonoBehaviour
{
    public Texture2D m_whiteTexture;


    enum ControllerMenus             { NOMENU,       STATS,   CONFIG,   LENGTH };
    string [] m_controllerMenuText = { "debug menu", "stats", "config", };
    ControllerMenus m_controllerMenuSelected = ControllerMenus.NOMENU;

    bool m_showController = false;
    float m_timeSlider = 1.0f;
    GUIStyle m_guiLabel;

    FpsCounter m_fpsCounter;

    Texture2D m_win_video_window_unity;
    Texture2D m_palette;

    Thread m_thread;
    bool m_exitScheduled = false;
    bool m_keyManualOverride = false;

    [NonSerialized] public static DeferredSynchronizeInvoke m_synchronizeInvoke;

    List<Color> m_colorCacheList = new List<Color>(0xffffff + 1);
    List<bool> m_colorCacheListUsed = new List<bool>(0xffffff + 1);

    int m_audioUpdateCount = 0;
    //double m_sampleRate = 0;


    void Awake()
    {
        //m_sampleRate = AudioSettings.outputSampleRate;
    }


    void Start()
    {
        Debug.Log("Unity Version: " + Application.unityVersion);
        Debug.Log("Platform: " + Application.platform);
        Debug.Log(string.Format("Application.streamingAssetsPath - '{0}'", Application.streamingAssetsPath));
        Debug.Log(string.Format("Directory.GetCurrentDirectory() - '{0}'", Directory.GetCurrentDirectory()));
        Debug.Log(string.Format("Application.dataPath - '{0}'", Application.dataPath));
        Debug.Log(string.Format("Application.persistantDataPath - '{0}'", Application.persistentDataPath));
        Debug.Log(string.Format("Path.GetFullPath('.') - '{0}'", Path.GetFullPath(".")));


        Application.targetFrameRate = 60;


        m_fpsCounter = FindObjectOfType<FpsCounter>();


        if (IsAndroid())
        {
            m_win_video_window_unity = new Texture2D(400, 400, TextureFormat.RGB24, false);
        }
        else
        {
            m_win_video_window_unity = new Texture2D(400, 400, TextureFormat.ARGB32, false);
        }


        {
            var pixels = m_win_video_window_unity.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.magenta;

            m_win_video_window_unity.SetPixels(pixels);
            m_win_video_window_unity.Apply();
        }

        m_palette = new Texture2D(200, 200, TextureFormat.RGB24, false);

        {
            var pixels = m_palette.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.red;

            m_palette.SetPixels(pixels);
            m_palette.Apply();
        }


        m_showController = true;


        m_synchronizeInvoke = new DeferredSynchronizeInvoke();

        for (int i = 0; i < 0xffffff + 1; i++)
        {
            m_colorCacheList.Add(new Color());
            m_colorCacheListUsed.Add(false);
        }
    }


    void Update()
    {
        m_synchronizeInvoke.ProcessQueue();


        if (m_exitScheduled)
        {
            ApplicationQuit();
        }


        if (Input.GetKeyDown(KeyCode.Escape)) { }
        if (Input.GetKeyDown(KeyCode.Alpha1)) { Debug.Log("Mame Update()"); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { }
        if (Input.GetKeyDown(KeyCode.C)) { m_showController = !m_showController; }
        if (Input.GetKeyDown(KeyCode.Z)) { }
        if (Input.GetMouseButtonDown(0)) { }

        //foreach (KeyCode c in Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>())
        foreach (KeyCode c in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(c)) { }
            if (Input.GetKeyUp(c)) { }
        }


        {
            var instance = mame.mame_machine_manager.instance();
            var osd = instance != null ? (mcsUnity.osd_interface_Unity)instance.osd() : null;

            if (osd != null && osd.keyboard_state != null)
            {
                if (!m_keyManualOverride)
                {
                    foreach (var key in mcsUnity.osd_interface_Unity.keymap)
                    {
                        bool keyStatus = Input.GetKey(key.Key);
                        osd.keyboard_state[(int)key.Value].i = keyStatus ? 1 : 0;
                    }
                }
            }


            if (osd != null && osd.mouse_axis_state != null)
            {
                // Input.mousePosition
                // The bottom-left of the screen or window is at (0, 0). The top-right of the screen or window is at (Screen.width, Screen.height).
                // Note: Input.mousePosition reports the position of the mouse even when it is not inside the Game View

                float mouseX = Input.mousePosition.x;
                float mouseY = Input.mousePosition.y;

                mouseY = Screen.height - mouseY;

                if (mouseX >= 0 && mouseX <= Screen.width &&
                    mouseY >= 0 && mouseY <= Screen.height)
                {
                    // using ScaleAndCrop here, because I think there's a bug in the function.  It should be ScaleToFit, just like the DrawTexture() call.
                    Vector2 result = ScaleFromTo(new Vector2(Screen.width, Screen.height), new Vector2(m_win_video_window_unity.width, m_win_video_window_unity.height), ScaleMode.ScaleAndCrop);

                    float mouseXscaled = mouseX * result.x;
                    float mouseYscaled = mouseY * result.y;
                    float widthScaled = Screen.width * result.x;
                    float heightScaled = Screen.height * result.y;

                    float emptyRegionWidth = (widthScaled - m_win_video_window_unity.width) / 2;
                    float emptyRegionHeight = (heightScaled - m_win_video_window_unity.height) / 2;

                    //Debug.LogFormat("{0}x{1} - {2}", mouseX, mouseY, result);
                    //Debug.LogFormat("Mouse: {0}x{1} - {2} - {3} - {4} - {5} - {6}", result.x, result.y, mouseXscaled, Screen.width, m_win_video_window_unity.width, widthScaled, emptyRegionWidth);

                    bool outside = false;
                    if (mouseXscaled < emptyRegionWidth || mouseXscaled > (m_win_video_window_unity.width + emptyRegionWidth))
                    {
                        outside = true;
                        //Debug.LogFormat("Mouse outside region: {0}", mouseXscaled);
                    }

                    if (mouseYscaled < emptyRegionHeight || mouseYscaled > (m_win_video_window_unity.height + emptyRegionHeight))
                    {
                        outside = true;
                        //Debug.LogFormat("Mouse outside region: {0}", mouseYscaled);
                    }

                    if (!outside)
                    {
                        int mouseXint = (int)(mouseXscaled - emptyRegionWidth);
                        int mouseYint = (int)(mouseYscaled - emptyRegionHeight);

                        //Console.WriteLine("Form1_MouseMove() - '{0}' - '{1}' - '{2}' - '{3}'", mouseX, mouseY, dpiX, scaleX);

                        osd.mouse_axis_state[0].i = mouseXint;
                        osd.mouse_axis_state[1].i = mouseYint;

                        osd.ui_input_push_mouse_move_event(mouseXint, mouseYint);

                        if (osd.mouse_button_state != null)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                bool mouseButton = Input.GetMouseButton(i);
                                osd.mouse_button_state[i].i = mouseButton ? 1 : 0;

                                if (i == 0)
                                {
                                    if (mouseButton) osd.ui_input_push_mouse_down_event(mouseXint, mouseYint);
                                    else             osd.ui_input_push_mouse_up_event(mouseXint, mouseYint);
                                }
                            }
                        }
                    }
                }
            }
        }


        {
            var instance = mame.mame_machine_manager.instance();
            var osd = instance != null ? (mcsUnity.osd_interface_Unity)instance.osd() : null;
            var screenbuffer = osd != null ? osd.screenbufferptr : null;
            if (screenbuffer != null)
            {
                lock (osd.osdlock)
                {
                    if (osd.osdlock_screen_buffer_updated)
                    {
                        // https://docs.unity3d.com/ScriptReference/Texture2D.GetPixels.html

                        // TODO - use GetPixels32()
                        // TODO - in ToColor(), return a ref, to avoid copies
                        var pixels = m_win_video_window_unity.GetPixels();
                        int texw = m_win_video_window_unity.width;
                        int texh = m_win_video_window_unity.height;

                        int stride = 400;


                        System.Random r = new System.Random();
                        for (int i = 0; i < 10; i++)
                            screenbuffer.set_uint32(i + 200 + 100 * stride, (UInt32)(r.Next() % 16000000));

                        for (int i = 0; i < 10; i++)
                            screenbuffer.set_uint32(i + 200 + 110 * stride, mame.rgb_t.white());


                        for (int y = 0; y < 400; y++)
                        {
                            for (int x = 0; x < 400; x++)
                            {
                                Color color = ToColor((int)screenbuffer.get_uint32((y * stride) + x));
                                pixels[x + ((texh - (y + 1)) * texw)] = color;
                            }
                        }


                        m_win_video_window_unity.SetPixels(pixels);

                        m_win_video_window_unity.Apply();

                        osd.osdlock_screen_buffer_updated = false;
                    }
                }
            }
        }
    }


    void OnGUI()
    {
        if (m_guiLabel == null)
        {
            m_guiLabel = new GUIStyle(GUI.skin.label);
            m_guiLabel.padding = new RectOffset(0, 0, 0, 0);
        }


        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        GUI.DrawTexture(rect, m_win_video_window_unity, ScaleMode.ScaleToFit);


        if (m_showController)
        {
            float areaX = 0;
            float areaY = 0;
            float areaW = 230;
            float buttonH = 20;
            float spaceHeight = 10;

            if (IsIOS() || IsAndroid())
                buttonH = 110;

            if (m_controllerMenuSelected == ControllerMenus.STATS)
                areaW = 500;

            GUILayout.BeginArea(new Rect(areaX, areaY, areaW, Screen.height));

            GUILayout.BeginVertical(GUI.skin.box);


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<", GUILayout.Height(buttonH))) { m_controllerMenuSelected = (ControllerMenus)DecrementWithRollover((int)m_controllerMenuSelected, m_controllerMenuText.Length); }
            if (GUILayout.Button(m_controllerMenuText[(int)m_controllerMenuSelected], GUILayout.Height(buttonH))) { m_controllerMenuSelected = (ControllerMenus)IncrementWithRollover((int)m_controllerMenuSelected, m_controllerMenuText.Length); }
            if (GUILayout.Button(">", GUILayout.Height(buttonH))) { m_controllerMenuSelected = (ControllerMenus)IncrementWithRollover((int)m_controllerMenuSelected, m_controllerMenuText.Length); }
            GUILayout.EndHorizontal();


            GUILayout.Space(spaceHeight);

            if (m_controllerMenuSelected == ControllerMenus.NOMENU)
            {
                var instance = mame.mame_machine_manager.instance();
                var machine = instance != null ? instance.machine() : null;
                var video = machine != null ? machine.video() : null;
                if (video != null)
                {
                    osd_ticks_t tps = mame.osdcore_global.m_osdcore.osd_ticks_per_second();
                    double final_real_time = tps == 0 ? 0 : (double)video.overall_real_seconds + (double)video.overall_real_ticks / (double)tps;
                    double final_emu_time = video.overall_emutime.as_double();
                    double average_speed_percentage = final_real_time == 0 ? 0 : 100 * final_emu_time / final_real_time;
                    string total_time = (video.overall_emutime + new mame.attotime(0, mame.attotime.ATTOSECONDS_PER_SECOND / 2)).as_string(2);
                    GUILayout.Label(string.Format("Avg Spd: {0:f2}% ({1} secs) - speed_text: {2}", average_speed_percentage, total_time, video.speed_text()), m_guiLabel);
                }

                float fps = 0;
                float averageFps = 0;
                if (m_fpsCounter)
                {
                    fps = m_fpsCounter.Fps;
                    averageFps = m_fpsCounter.AverageFps;
                }

                float lowFpsThreshold = 30;
                float fpsColor = Mathf.Min(1.0f, averageFps / lowFpsThreshold);
                GUI.color = new Color(1, fpsColor, fpsColor);
                GUILayout.Label(string.Format("T: {0:f2} F: {1} AVG: {2:f0} FPS: {3:f2}", Time.time, Time.frameCount, averageFps, fps), m_guiLabel);
                GUI.color = Color.white;


                if (GUILayout.Button("Start Emulator", GUILayout.Height(buttonH)))
                {
                    if (m_thread == null)
                    {
                        mcsUnity.osdcore_Unity osdcore = new mcsUnity.osdcore_Unity();
                        mcsUnity.osd_file_Unity osdfile = new mcsUnity.osd_file_Unity();
                        mcsUnity.osd_directory_static_Unity osddirectory = new mcsUnity.osd_directory_static_Unity();
                        mcsUnity.osd_options_Unity options = new mcsUnity.osd_options_Unity();
                        mcsUnity.osd_interface_Unity osd = new mcsUnity.osd_interface_Unity(options);
                        osd.register_options();
                        mame.cli_frontend frontend = new mame.cli_frontend(options, osd);


                        mame.osdcore_global.set_osdcore(osdcore);
                        mame.osdcore_global.set_osdfile(osdfile);
                        mame.osdcore_global.set_osddirectory(osddirectory);
                        mame.mame_machine_manager.instance(options, osd);

                        Debug.Log("Starting Emulator...");
                        m_thread = new Thread(() => SafeExecute(() => StartEmulatorThread(frontend), Handler));
                        m_thread.IsBackground = true;
                        m_thread.Name = "mame.cli_frontend.execute()";
                        m_thread.Start();
                        //new Thread(() => 
                        //{
                        //    Debug.Log("Starting Background thread...");
                        //
                        //    Thread.CurrentThread.IsBackground = true; 
                        //    Thread.CurrentThread.Name = "machine_manager.execute()";
                        //
                        //    int ret = frontend.execute(new List<string>(Environment.GetCommandLineArgs()));
                        //
                        //    // tell form that it should close
                        //    //form.Invoke((MethodInvoker)delegate { form.Close(); });
                        //    VHUtils.ApplicationQuit();
                        //}).Start();
                    }
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Q1", GUILayout.Height(buttonH))) { StartCoroutine(SendKey(KeyCode.Alpha5)); }
                if (GUILayout.Button("S1", GUILayout.Height(buttonH))) { StartCoroutine(SendKey(KeyCode.Alpha1)); }
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Up", GUILayout.Height(buttonH))) { StartCoroutine(SendKey(KeyCode.UpArrow)); }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Left", GUILayout.Height(buttonH))) { StartCoroutine(SendKey(KeyCode.LeftArrow)); }
                if (GUILayout.Button("Right", GUILayout.Height(buttonH))) { StartCoroutine(SendKey(KeyCode.RightArrow)); }
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Down", GUILayout.Height(buttonH))) { StartCoroutine(SendKey(KeyCode.DownArrow)); }
                if (GUILayout.Button("Fire", GUILayout.Height(buttonH))) { StartCoroutine(SendKey(KeyCode.LeftControl)); }
                if (GUILayout.Button("Enter", GUILayout.Height(buttonH))) { StartCoroutine(SendKey(KeyCode.Return)); }
            }
            else if (m_controllerMenuSelected == ControllerMenus.STATS)
            {
                var instance = mame.mame_machine_manager.instance();
                var machine = instance != null ? instance.machine() : null;
                var video = machine != null ? machine.video() : null;
                if (video != null)
                {
                    osd_ticks_t tps = mame.osdcore_global.m_osdcore.osd_ticks_per_second();
                    double final_real_time = tps == 0 ? 0 : (double)video.overall_real_seconds + (double)video.overall_real_ticks / (double)tps;
                    double final_emu_time = video.overall_emutime.as_double();
                    double average_speed_percentage = final_real_time == 0 ? 0 : 100 * final_emu_time / final_real_time;
                    string total_time = (video.overall_emutime + new mame.attotime(0, mame.attotime.ATTOSECONDS_PER_SECOND / 2)).as_string(2);
                    GUILayout.Label(string.Format("Avg Spd: {0:f2}% ({1} secs) - speed_text: {2}", average_speed_percentage, total_time, video.speed_text()), m_guiLabel);
                }

                float fps = 0;
                float averageFps = 0;
                if (m_fpsCounter)
                {
                    fps = m_fpsCounter.Fps;
                    averageFps = m_fpsCounter.AverageFps;
                }

                float lowFpsThreshold = 30;
                float fpsColor = Mathf.Min(1.0f, averageFps / lowFpsThreshold);
                GUI.color = new Color(1, fpsColor, fpsColor);
                GUILayout.Label(string.Format("T: {0:f2} F: {1} AVG: {2:f0} FPS: {3:f2}", Time.time, Time.frameCount, averageFps, fps), m_guiLabel);
                GUI.color = Color.white;

                GUILayout.Label(string.Format("{0}x{1}x{2} ({3}) {4:f0}dpi", Screen.width, Screen.height, Screen.currentResolution.refreshRate, GetCommonAspectText((float)Screen.width / Screen.height), Screen.dpi), m_guiLabel);
                GUILayout.Label(string.Format("Unity Version: {0}", Application.unityVersion), m_guiLabel);
                GUILayout.Label(string.Format("Platform: {0}", Application.platform), m_guiLabel);
                GUILayout.Label(string.Format("{0}", SystemInfo.operatingSystem), m_guiLabel);
                GUILayout.Label(string.Format("{0} x {1}", SystemInfo.processorCount, SystemInfo.processorType), m_guiLabel);
                GUILayout.Label(string.Format("Mem: {0:f1}gb", SystemInfo.systemMemorySize / 1000.0f), m_guiLabel);
                GUILayout.Label(string.Format("{0} - deviceID: {1}", SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceID), m_guiLabel);
                GUILayout.Label(string.Format("{0} - vendorID: {1}", SystemInfo.graphicsDeviceVendor, SystemInfo.graphicsDeviceVendorID), m_guiLabel);
                GUILayout.Label(string.Format("{0}", SystemInfo.graphicsDeviceVersion), m_guiLabel);
                GUILayout.Label(string.Format("VMem: {0}mb", SystemInfo.graphicsMemorySize), m_guiLabel);
                GUILayout.Label(string.Format("Shader Level: {0:f1}", SystemInfo.graphicsShaderLevel / 10.0f), m_guiLabel);
                GUILayout.Label(string.Format("Shadows:{0} FX:{1} MT:{2}", SystemInfo.supportsShadows ? "y" : "n", SystemInfo.supportsImageEffects ? "y" : "n", SystemInfo.graphicsMultiThreaded ? "y" : "n"), m_guiLabel);
                GUILayout.Label(string.Format("deviceUniqueIdentifier: {0}", SystemInfo.deviceUniqueIdentifier), m_guiLabel);
                GUILayout.Label(string.Format("deviceName: {0}", SystemInfo.deviceName), m_guiLabel);
                GUILayout.Label(string.Format("deviceModel: {0}", SystemInfo.deviceModel), m_guiLabel);
                GUILayout.Label(string.Format("deviceType: {0}", SystemInfo.deviceType), m_guiLabel);
                GUILayout.Label(string.Format("UserName: {0}", System.Environment.UserName), m_guiLabel);

                GUILayout.Label(string.Format("MonoHeap: {0:N0}", UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong()), m_guiLabel);
                GUILayout.Label(string.Format("TempAllocator: {0:N0}", UnityEngine.Profiling.Profiler.GetTempAllocatorSize()), m_guiLabel);
                GUILayout.Label(string.Format("AllocatedMemory: {0:N0}", UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong()), m_guiLabel);
                GUILayout.Label(string.Format("ReservedMemory: {0:N0}", UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong()), m_guiLabel);
                GUILayout.Label(string.Format("UnusedReservedMemory: {0:N0}", UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong()), m_guiLabel);

                GUILayout.Label(string.Format("Scene: {0}", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name), m_guiLabel);
                GUILayout.Label(string.Format("Qual: {0} - {1}", QualitySettings.names[QualitySettings.GetQualityLevel()], QualitySettings.activeColorSpace), m_guiLabel);
            }
            else if (m_controllerMenuSelected == ControllerMenus.CONFIG)
            {
                GUILayout.Label("Quality:", m_guiLabel);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("<", GUILayout.Width(areaW * 0.16f))) { QualitySettings.SetQualityLevel(Extensions.Clamp(QualitySettings.GetQualityLevel() - 1, 0, QualitySettings.names.Length - 1)); }
                GUILayout.Button(string.Format("{0}", QualitySettings.names[QualitySettings.GetQualityLevel()]), GUILayout.Width(areaW * 0.6f));
                if (GUILayout.Button(">", GUILayout.Width(areaW * 0.16f))) { QualitySettings.SetQualityLevel(Extensions.Clamp(QualitySettings.GetQualityLevel() + 1, 0, QualitySettings.names.Length - 1)); }
                GUILayout.EndHorizontal();

                GUILayout.Space(spaceHeight);
                m_timeSlider = GUILayout.HorizontalSlider(m_timeSlider, 0.01f, 3);
                GUILayout.Label(string.Format("Time: {0}", m_timeSlider), m_guiLabel);
            }


            GUILayout.Space(spaceHeight);

            GUILayout.EndVertical();

            GUILayout.EndArea();

            Time.timeScale = m_timeSlider;
        }
    }


    void OnApplicationQuit()
    {
        m_synchronizeInvoke = null;

        var logCallbackHandler = GameObject.FindObjectOfType<LogCallbackHandler>();
        if (logCallbackHandler) { logCallbackHandler.RemoveAllCallbacks(); logCallbackHandler.enabled = false; }

        Debug.Log("Killing Threads");

        // tell child thread to quit
        if (mame.mame_machine_manager.instance() != null)
        {
            mcsUnity.osd_interface_Unity osd = (mcsUnity.osd_interface_Unity)mame.mame_machine_manager.instance().osd();

            if (osd != null)
            {
                lock (osd.osdlock)
                {
                    osd.ScheduleExit();
                }
            }
        }

        if (m_thread != null)
            m_thread.Join(2000);

        //System.Threading.Thread.Sleep(1000);

        Debug.Log("Exiting Main Thread.");
    }


    void OnAudioFilterRead(float[] data, int channels)
    {
        int actualCount = 0;


        //double bpm = 140.0F;
        //int signatureLo = 4;
        //double sampleRate = AudioSettings.outputSampleRate;

        //double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        //double sample = AudioSettings.dspTime * sampleRate;
        //int dataLen = data.Length / channels;


        var instance = mame.mame_machine_manager.instance();
        var osd = instance != null ? (mcsUnity.osd_interface_Unity)instance.osd() : null;
        if (osd == null)
        {
            //while (actualCount < sampleCount)
            //{
            //    buffer[actualCount + offset] = 0;  // 20000;
            //    actualCount++;
            //}
            data[0] = 0;
            actualCount++;

            //return actualCount;
            return;
        }

        if (m_audioUpdateCount++ % 100 == 0)
            Debug.LogFormat("OnAudioFilterRead() - {0} - data: {1} channels: {2} osd.m_audiobuffer: {3}", m_audioUpdateCount, data.Length, channels, osd.m_audiobuffer.Count);

        lock (osd.osdlock_audio)
        {
            //for (int i = 0; i < sampleCount; i++)
            for (int i = 0; i < data.Length; i++)
            {
                if (osd.m_audiobuffer.Count == 0)
                    break;

                Int16 sample = osd.m_audiobuffer.Dequeue();
                float samplef = sample < 0 ? (float)sample / -Int16.MinValue : (float)sample / Int16.MaxValue;
                data[i] = samplef;
                actualCount++;
            }

            osd.m_audiobuffer.Clear();

            //while (actualCount < sampleCount)
            //{
            //    buffer[actualCount + offset] = 0;  // 20000;
            //    actualCount++;
            //}

            if (actualCount == 0)
            {
                data[0] = 0;
                actualCount++;
            }
        }

        //return actualCount;
    }


    public Color ToColor(int hexVal)
    {
        int index = hexVal & 0xFFFFFF;
        if (m_colorCacheListUsed[index])
            return m_colorCacheList[index];

        byte R = (byte)((hexVal >> 16) & 0xFF);
        byte G = (byte)((hexVal >> 8) & 0xFF);
        byte B = (byte)((hexVal) & 0xFF);

        Color32 color = new Color32(R, G, B, 255);

        m_colorCacheList[index] = (Color)color;
        m_colorCacheListUsed[index] = true;

        return color;
    }


    static void Handler(Exception exception)
    {
        Debug.Log(exception);
    }


    static void SafeExecute(Action test, Action<Exception> handler)
    {
        try
        {
            test.Invoke();
        }
        catch (Exception ex)
        {
            handler(ex);
        }
    }


    void StartEmulatorThread(mame.cli_frontend frontend)
    {
        Debug.Log("Starting Background thread...");

        int ret = frontend.execute(new mame.std_vector<string>() { "mcsUnity" });  //Environment.GetCommandLineArgs()));

        Debug.LogFormat("frontend.execute() returned: {0}", ret);

        // tell main thread to quit
        ScheduleExit();
    }


    void ScheduleExit()
    {
        m_exitScheduled = true;
    }


    IEnumerator SendKey(KeyCode key)
    {
        mcsUnity.osd_interface_Unity osd = (mcsUnity.osd_interface_Unity)mame.mame_machine_manager.instance().osd();

        if (osd.keyboard_state != null)
        {
            m_keyManualOverride = true;

            var unitykey = mcsUnity.osd_interface_Unity.keymap[key];

            osd.keyboard_state[(int)unitykey].i = 1;

            yield return new WaitForSeconds(0.4f);

            osd.keyboard_state[(int)unitykey].i = 0;

            m_keyManualOverride = false;
        }
    }


    //https://github.com/JaySoyer/Unity-Utils/blob/master/Scripts/Vectors.cs
    public static Vector2 ScaleFromTo(Vector2 from, Vector2 to, ScaleMode scaleMode = ScaleMode.StretchToFill)
    {
        Vector2 scale = Vector2.one;
        scale.x = to.x / from.x;
        scale.y = to.y / from.y;

        switch (scaleMode)
        {
        case ScaleMode.ScaleAndCrop:
            scale.x = scale.y = Mathf.Max(scale.x, scale.y);
            break;

        case ScaleMode.ScaleToFit:
            scale.x = scale.y = Mathf.Min(scale.x, scale.y);
            break;

        case ScaleMode.StretchToFill:
            //Do nothing
            break;

        default:
            Debug.LogException(new System.NotImplementedException("The Received ScaleMode." + scaleMode.ToString() + " is not implemented."));
            break;
        }

        return scale;
    }


    public static bool IsEditor()
    {
        return Application.isEditor;
    }


    public static bool IsIOS()
    {
        if (IsEditor())
        {
#if UNITY_EDITOR
            return UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS;
#endif
        }

        return Application.platform == RuntimePlatform.IPhonePlayer;
    }


    public static bool IsAndroid()
    {
        if (IsEditor())
        {
#if UNITY_EDITOR
            return UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android;
#endif
        }

        return Application.platform == RuntimePlatform.Android;
    }


    public static void ApplicationQuit()
    {
        if (IsEditor())
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
#endif
        }
        else
        {
            Application.Quit();
        }
    }

    public static Rect ScaleToRes(ref Rect r)
    {
        r.x *= Screen.width;
        r.y *= Screen.height;
        r.width *= Screen.width;
        r.height *= Screen.height;
        return r;
    }

    public static int IncrementWithRollover(int value, int max)
    {
        if (max == 0)
            return 0;

        return (value + 1) % max;
    }

    public static int DecrementWithRollover(int value, int max)
    {
        if (max == 0)
            return 0;

        return (value == 0) ? max - 1 : value - 1;
    }

    public static string GetCommonAspectText(float aspectRatio)
    {
       // http://en.wikipedia.org/wiki/List_of_common_resolutions
       const float check = 0.04f;
       if      (Math.Abs(aspectRatio - 1.00f) < check) return "1:1";
       else if (Math.Abs(aspectRatio - 1.25f) < check) return "5:4";
       else if (Math.Abs(aspectRatio - 1.33f) < check) return "4:3";
       else if (Math.Abs(aspectRatio - 1.50f) < check) return "3:2";
       else if (Math.Abs(aspectRatio - 1.60f) < check) return "16:10";
       else if (Math.Abs(aspectRatio - 1.66f) < check) return "5:3";
       else if (Math.Abs(aspectRatio - 1.77f) < check) return "16:9";

       // reverse
       else if (Math.Abs(aspectRatio - 0.80f) < check) return "4:5";
       else if (Math.Abs(aspectRatio - 0.75f) < check) return "3:4";
       else if (Math.Abs(aspectRatio - 0.66f) < check) return "2:3";
       else if (Math.Abs(aspectRatio - 0.62f) < check) return "10:16";
       else if (Math.Abs(aspectRatio - 0.60f) < check) return "3:5";
       else if (Math.Abs(aspectRatio - 0.56f) < check) return "9:16";

       else return "";
    }
}


public static class Extensions
{
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0) return min;
        else if(val.CompareTo(max) > 0) return max;
        else return val;
    }
}
