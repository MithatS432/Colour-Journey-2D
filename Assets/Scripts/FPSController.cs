using UnityEngine;
using TMPro;

public class FPSController : MonoBehaviour
{
    [Header("FPS Settings")]
    public int targetFPS = 60;

    [Header("UI (TextMeshPro)")]
    public TMP_Text fpsText;

    [Header("Smoothing")]
    [Range(0.01f, 1f)]
    public float smoothing = 0.9f;

    private float fps;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;

        Time.maximumDeltaTime = 0.033f;
    }

    void Update()
    {
        float currentFPS = 1f / Time.unscaledDeltaTime;

        fps = Mathf.Lerp(fps, currentFPS, 1f - smoothing);

        if (fpsText != null)
        {
            fpsText.text = $"FPS: {fps:0}";
        }
    }
}