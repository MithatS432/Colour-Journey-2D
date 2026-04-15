using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    public Ball ball;

    public float speed = 2f;
    public float despawnY = -10f;
    public string poolTag = "Obstacle";

    void Update()
    {
        float multiplier = ball.GetDifficultyMultiplier();

        transform.position += Vector3.down * (speed * multiplier) * Time.deltaTime;

        if (transform.position.y < despawnY)
            ReturnToPool();
    }

    void ReturnToPool()
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}