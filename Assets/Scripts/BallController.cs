using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class BallController : MonoBehaviour
{
    public ObstacleController spawner;

    public GameObject ballPrefab;
    public bool isBallActive = false;

    public TextMeshProUGUI targetText;

    public GameObject startMenuPanel;
    public GameObject gamePlayPanel;

    void Start()
    {
        if (ballPrefab != null)
            ballPrefab.SetActive(false);

        if (startMenuPanel != null)
            startMenuPanel.SetActive(true);

        if (gamePlayPanel != null)
            gamePlayPanel.SetActive(false);
    }

    void Update()
    {
        if (isBallActive || ballPrefab == null) return;

        if (IsScreenPressed() && IsClickOnTarget())
        {
            ActivateGame();
        }
    }

    void ActivateGame()
    {
        SpriteRenderer ballSr = ballPrefab.GetComponent<SpriteRenderer>();
        if (ballSr != null)
        {
            ballSr.enabled = true;
            Debug.Log("✅ Top görünür yapıldı");
        }

        ballPrefab.SetActive(true);
        isBallActive = true;

        Rigidbody2D rb = ballPrefab.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.linearVelocity = Vector2.zero;
            rb.simulated = true;
        }

        Ball ballScript = ballPrefab.GetComponent<Ball>();
        if (ballScript != null)
        {
            ballScript.SetColor(ballScript.currentColor);
        }

        if (spawner != null)
            spawner.StartGame();

        if (gamePlayPanel != null)
            gamePlayPanel.SetActive(true);

        if (startMenuPanel != null)
            startMenuPanel.SetActive(false);
    }

    private bool IsClickOnTarget()
    {
        if (EventSystem.current == null || targetText == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = GetPointerPosition();

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            if (r.gameObject == targetText.gameObject)
                return true;
        }

        return false;
    }

    private Vector2 GetPointerPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();

        return Mouse.current.position.ReadValue();
    }

    private bool IsScreenPressed()
    {
        if (Touchscreen.current != null)
            return Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

        if (Mouse.current != null)
            return Mouse.current.leftButton.wasPressedThisFrame;

        return false;
    }

    public void StartRestart()
    {
        StartCoroutine(RestartSceneAfterDelay(2f));
    }

    private IEnumerator RestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}