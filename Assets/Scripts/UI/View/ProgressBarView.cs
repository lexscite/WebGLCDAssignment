using System.Globalization;
using TMPro;
using UnityEngine;

namespace WebGLCD
{
public class ProgressBarView : MonoBehaviour
{
    [SerializeField]
    private RectTransform _foregroundT;

    [SerializeField]
    private TMP_Text _tmpValue;

    public void SetValue(float value, string additionalInfo = null)
    {
        var text = $"{(value * 100).ToString("F1", CultureInfo.InvariantCulture)}%";
        if (additionalInfo != null)
        {
            text += $"({additionalInfo})";
        }

        _tmpValue.SetText(text);
        UpdateForeground(value);
    }

    private void UpdateForeground(float value) { _foregroundT.anchorMax = new Vector2(Mathf.Clamp01(value), 1); }
}
}