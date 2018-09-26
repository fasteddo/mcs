// license:BSD-3-Clause
// copyright-holders:Edward Fast

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class LogCallbackHandler : MonoBehaviour
{
    protected List<Application.LogCallback> m_callbacks = new List<Application.LogCallback>();


    void Awake()
    {
        Application.logMessageReceived += LogCallback;
    }


    void OnDestroy()
    {
        Application.logMessageReceived -= LogCallback;
    }

    void Start()
    {
    }


    void LogCallback(string logString, string stackTrace, LogType type)
    {
        foreach (var callback in m_callbacks)
        {
            callback(logString, stackTrace, type);
        }
    }


    public void AddCallback(Application.LogCallback callback)
    {
        m_callbacks.Add(callback);
    }

    public void RemoveCallback(Application.LogCallback callback)
    {
        m_callbacks.Remove(callback);
    }

    public void RemoveAllCallbacks()
    {
        m_callbacks.Clear();
    }
}
