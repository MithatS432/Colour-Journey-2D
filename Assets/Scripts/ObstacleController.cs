using UnityEngine;
using System.Collections;

public class ObstacleController : MonoBehaviour
{
    public enum SpawnType
    {
        Obstacle,
        ColourSwitch,
        Collectible
    }

    public Ball ball;

    public bool isGameStarted = false;
    public float spawnX = 0.3f;
    public float spawnY = 8f;

    private Coroutine spawnRoutine;

    public void StartGame()
    {
        if (spawnRoutine != null) return;

        isGameStarted = true;
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (isGameStarted)
        {
            SpawnObject();

            yield return new WaitForSeconds(
                Difficulty.GetSpawnInterval(ball.GetScore())
            );
        }
    }

    void SpawnObject()
    {
        if (!isGameStarted) return;

        ObjectPool pool = ObjectPool.Instance;
        if (pool == null) return;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        SpawnType type = GetSpawnType();

        string tagToSpawn = GetTagFromType(type);

        GameObject spawned = pool.SpawnFromPool(tagToSpawn, spawnPos, Quaternion.identity);

        if (spawned == null)
        {
            Debug.LogWarning($"Spawn failed: {tagToSpawn}");
        }
    }

    SpawnType GetSpawnType()
    {
        float t = Difficulty.GetT(ball.GetScore());

        float obstacleChance = Mathf.Lerp(0.45f, 0.70f, t);
        float switchChance = Mathf.Lerp(0.30f, 0.20f, t);
        float collectChance = Mathf.Lerp(0.25f, 0.10f, t);

        float sum = obstacleChance + switchChance + collectChance;

        obstacleChance /= sum;
        switchChance /= sum;
        collectChance /= sum;

        float r = Random.value;

        if (r < obstacleChance)
            return SpawnType.Obstacle;

        if (r < obstacleChance + switchChance)
            return SpawnType.ColourSwitch;

        return SpawnType.Collectible;
    }

    string GetTagFromType(SpawnType type)
    {
        return type switch
        {
            SpawnType.Obstacle => GetRandomObstacleTag(),
            SpawnType.ColourSwitch => "ColourSwitch",
            SpawnType.Collectible => "Collectible",
            _ => "Obstacle1"
        };
    }
    string GetRandomObstacleTag()
    {
        int r = Random.Range(0, 3);

        return r switch
        {
            0 => "Obstacle1",
            1 => "Obstacle2",
            _ => "Obstacle3"
        };
    }
}