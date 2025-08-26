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

    public void SetValue(float value)
    {
        _tmpValue.SetText($"{Mathf.RoundToInt(value) * 100}%");
        UpdateForeground(value);
    }

    public void SetValue(float value, string additionalInfo)
    {
        _tmpValue.SetText($"{Mathf.RoundToInt(value) * 100} ({additionalInfo})%");
        UpdateForeground(value);
    }

    private void UpdateForeground(float value) { _foregroundT.anchorMax = new Vector2(Mathf.Clamp01(value), 1); }
}
}