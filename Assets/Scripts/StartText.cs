using UnityEngine;
using TMPro;

public class StartText : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI textMesh;

    [Header("Scale Animation")]
    public float minScale = 1f;
    public float maxScale = 1.2f;
    public float scaleSpeed = 2f;

    [Header("Color Animation")]
    public Color colorA = Color.white;
    public Color colorB = Color.cyan;
    public float colorSpeed = 2f;

    void Update()
    {
        AnimateScale();
        AnimateColor();
    }

    void AnimateScale()
    {
        float t = (Mathf.Sin(Time.time * scaleSpeed) + 1f) * 0.5f;
        float scale = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = Vector3.one * scale;
    }

    void AnimateColor()
    {
        float t = (Mathf.Sin(Time.time * colorSpeed) + 1f) * 0.5f;
        textMesh.color = Color.Lerp(colorA, colorB, t);
    }
}