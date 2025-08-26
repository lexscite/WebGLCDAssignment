using System;
using VContainer;
using VContainer.Unity;

namespace WebGLCD
{
public class Debug : IStartable
{
    private static Debug _instance;

    [Inject]
    private DevOverlayController _devOverlay;

    public void Start() { _instance = this; }

    public static void Log(object o)
    {
        #if UNITY_EDITOR
        UnityEngine.Debug.Log(o);
        #endif
        _instance._devOverlay.AddConsoleEntry(o.ToString(), ConsoleEntryType.Default);
    }

    public static void LogWarning(object o)
    {
        #if UNITY_EDITOR
        UnityEngine.Debug.LogWarning(o);
        #endif
        _instance._devOverlay.AddConsoleEntry(o.ToString(), ConsoleEntryType.Warning);
    }

    public static void LogError(object o)
    {
        #if UNITY_EDITOR
        UnityEngine.Debug.LogError(o);
        #endif
        _instance._devOverlay.AddConsoleEntry(o.ToString(), ConsoleEntryType.Error);
    }

    public static void LogException(Exception e)
    {
        #if UNITY_EDITOR
        UnityEngine.Debug.LogException(e);
        #endif
        _instance._devOverlay.AddConsoleEntry(e.ToString(), ConsoleEntryType.Error);
    }
}
}