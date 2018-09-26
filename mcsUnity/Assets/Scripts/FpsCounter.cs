// license:BSD-3-Clause
// copyright-holders:Edward Fast

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class FpsCounter : MonoBehaviour
{
    public int m_numFpsEntries = 50;       // number of entries in the average array.  larger numbers will generate a smoother average over time.
    public int m_initialAverageFps = 20;   // what value to populate the history with so that initial average values are extreme in any one direction

    List<float> m_averageFpsArray;
    float m_averageSum = 0;

    float m_fps;
    float m_smoothFps;
    float m_averageFps;


    public float Fps { get { return m_fps; } }
    public float SmoothFps { get { return m_smoothFps; } }    // this uses Unity's smoothDeltaTime to compute.  A little more sporatic than our computed average
    public float AverageFps { get { return m_averageFps; } }  // this computes our own average based on the past m_numFpsEntries frames.  Ends up being a little more smoother than SmoothFps


    public void Start()
    {
        m_averageFpsArray = new List<float>(m_numFpsEntries);
        float average = 1.0f / m_initialAverageFps;
        m_averageFpsArray.AddRange(Enumerable.Repeat(average, m_numFpsEntries));
        m_averageSum = average * m_numFpsEntries;
    }


    public void Update()
    {
        float adjustedDeltaTime;
        float adjustedSmoothDeltaTime;

        if (Time.deltaTime == 0 || Time.timeScale == 0)
        {
            // alternate computation, not really accurate, but just here to give some number.
            adjustedDeltaTime = Time.realtimeSinceStartup / Time.frameCount;
            adjustedSmoothDeltaTime = Time.realtimeSinceStartup / Time.frameCount;
        }
        else
        {
            adjustedDeltaTime = Time.deltaTime / Time.timeScale;
            adjustedSmoothDeltaTime = Time.smoothDeltaTime / Time.timeScale;
        }

        m_fps = 1 / adjustedDeltaTime;
        m_smoothFps = 1 / adjustedSmoothDeltaTime;

        // compute average
        int i = Time.frameCount % m_numFpsEntries;
        m_averageSum -= m_averageFpsArray[ i ];
        m_averageFpsArray[ i ] = adjustedDeltaTime;
        m_averageSum += adjustedDeltaTime;
        m_averageFps = m_numFpsEntries / m_averageSum;
    }
}
