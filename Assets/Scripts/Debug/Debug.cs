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
        UnityEngine.Debug.Log(o);
        _instance._devOverlay.AddConsoleEntry(o.ToString(), ConsoleEntryType.Default);
    }

    public static void LogWarning(object o)
    {
        UnityEngine.Debug.LogWarning(o);
        _instance._devOverlay.AddConsoleEntry(o.ToString(), ConsoleEntryType.Warning);
    }

    public static void LogError(object o)
    {
        UnityEngine.Debug.LogError(o);
        _instance._devOverlay.AddConsoleEntry(o.ToString(), ConsoleEntryType.Error);
    }

    public static void LogException(Exception e)
    {
        UnityEngine.Debug.LogException(e);
        _instance._devOverlay.AddConsoleEntry(e.ToString(), ConsoleEntryType.Error);
    }
}
}