using UnityEngine;

public class ColorSwitch : MonoBehaviour
{
    private bool used;

    private void OnEnable()
    {
        used = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;

        Ball ball = other.GetComponent<Ball>();

        if (ball != null)
        {
            used = true;
            ball.HandleColorSwitch(gameObject);
        }
    }
}