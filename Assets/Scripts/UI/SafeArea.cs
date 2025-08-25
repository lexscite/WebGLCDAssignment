using System;
using JetBrains.Annotations;
using UnityEngine;

namespace PaperStag
{
public class SafeArea : MonoBehaviour
{
    [Flags]
    protected enum ControlAxes
    {
        Horizontal = 1,
        Vertical = 2,
    }

    [SerializeField]
    protected ControlAxes _controlAxes;

    [SerializeField]
    protected bool _bottom = true;

    private RectTransform _rectTransform;

    private RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }

            return _rectTransform;
        }
    }

    private static float AspectRatio
        => Screen.width > Screen.height
            ? (float)Screen.width / Screen.height
            : (float)Screen.height / Screen.width;

    private static bool HaveNotch
        => Application.platform != RuntimePlatform.IPhonePlayer
            && AspectRatio is > 18.0f / 9.0f and < 19.5f / 9.0f;

    private static Rect Area => Screen.safeArea;

    private Rect _lastSafeArea;

    private void Start() { Refresh(); }

    private void Update() { Refresh(); }

    private void Refresh()
    {
        if (Area != _lastSafeArea)
        {
            ApplySafeArea(Area);
        }
    }

    [ContextMenu("Force Refresh")]
    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    private void ForceRefresh() { ApplySafeArea(Area); }

    private void ApplySafeArea(Rect rect)
    {
        _lastSafeArea = rect;

        var anchorMin = rect.position;
        var anchorMax = rect.position + rect.size;

        #if (UNITY_IOS || UNITY_ANDROID)
        if (HaveNotch)
        {
            anchorMin.x = 0;
            anchorMax.x = 1;

            if (_controlAxes.HasFlag(ControlAxes.Vertical))
            {
                if (_bottom)
                {
                    anchorMin.y = 0.03794643f;
                }
                else
                {
                    anchorMin.y = 0;
                }

                anchorMax.y = 0.9508929f;
            }
            else
            {
                anchorMin.y = 0;
                anchorMax.y = 1;
            }

            RectTransform.anchorMin = anchorMin;
            RectTransform.anchorMax = anchorMax;

            return;
        }
        #endif

        if (_controlAxes.HasFlag(ControlAxes.Horizontal))
        {
            anchorMin.x /= Screen.width;
            anchorMax.x /= Screen.width;
        }
        else
        {
            anchorMin.x = 0;
            anchorMax.x = 1;
        }

        if (_controlAxes.HasFlag(ControlAxes.Vertical))
        {
            if (_bottom)
            {
                anchorMin.y /= Screen.height;
            }
            else
            {
                anchorMin.y = 0;
            }

            anchorMax.y /= Screen.height;
        }
        else
        {
            anchorMin.y = 0;
            anchorMax.y = 1;
        }

        RectTransform.anchorMin = anchorMin;
        RectTransform.anchorMax = anchorMax;

        RectTransform.offsetMax = Vector2.zero;
        RectTransform.offsetMin = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if (Application.isPlaying) return;
        UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
        UnityEditor.SceneView.RepaintAll();
        #endif
    }
}
}