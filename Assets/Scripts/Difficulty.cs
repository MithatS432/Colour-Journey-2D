using UnityEngine;

public static class Difficulty
{
    public static float GetT(int score)
    {
        return Mathf.Clamp01(score / 100f);
    }

    public static float GetMultiplier(int score)
    {
        float t = GetT(score);
        return Mathf.Lerp(1f, 2.5f, t * t);
    }

    public static float GetSpawnInterval(int score)
    {
        float t = GetT(score);
        return Mathf.Lerp(3f, 1.2f, t * t);
    }
}