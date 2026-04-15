using UnityEngine;

public class RotateObstacles : MonoBehaviour
{
    public Ball ball;

    [SerializeField] private float rotationSpeed = 100f;
    public float chaosInterval = 2.5f;
    public float chaosChance = 0.2f;
    public float transitionSpeed = 6f;

    private float timer;
    private float currentDirection = 1f;
    private float targetDirection = 1f;

    void Update()
    {
        float multiplier = ball.GetDifficultyMultiplier();

        timer += Time.deltaTime;

        if (timer >= chaosInterval)
        {
            timer = 0f;

            float chance = chaosChance * Difficulty.GetMultiplier(ball.GetScore());

            if (Random.value < chance)
            {
                StartFlip();
            }
        }

        currentDirection = Mathf.Lerp(
            currentDirection,
            targetDirection,
            Time.deltaTime * transitionSpeed
        );

        transform.Rotate(0f, 0f,
            rotationSpeed * multiplier * currentDirection * Time.deltaTime);
    }

    void StartFlip()
    {
        targetDirection *= -1f;
    }
}