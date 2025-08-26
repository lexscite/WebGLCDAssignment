using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WebGLCD
{
public class DevOverlayView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _tmpConsole;

    [SerializeField]
    private ScrollRect _scrollView;

    private readonly StringBuilder _sb;

    private DevOverlayView() { _sb = new StringBuilder(); }

    public void AddConsoleEntry(string text, ConsoleEntryType type)
    {
        text = type switch
        {
            ConsoleEntryType.Error => $"<color=red>{text}</color>",
            ConsoleEntryType.Warning => $"<color=yellow>{text}</color>",
            _ => text
        };
        
        var needScroll = Mathf.Approximately(_scrollView.normalizedPosition.y, 1f);
        _sb.AppendLine(text);
        _tmpConsole.text = _sb.ToString();

        if (needScroll)
        {
            _scrollView.normalizedPosition = new Vector2(0f, 1f);
        }
    }
}
}