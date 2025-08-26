using System;
using UnityEngine;

namespace WebGLCD
{
public class DevOverlayController : MonoBehaviour
{
    [SerializeField]
    private DevOverlayView _view;

    public void AddConsoleEntry(string msg, ConsoleEntryType type) { _view.AddConsoleEntry(msg, type); }
}
}