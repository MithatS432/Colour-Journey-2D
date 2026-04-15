using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class Ball : MonoBehaviour
{
    [Header("References")]
    public BallController controller;
    public BallColor currentColor;
    public Button pauseButton;
    public Button continueButton;

    [Header("Death Settings")]
    public GameObject deathEffect;
    public AudioClip deathSound;
    public AudioClip jumpSound;
    private bool isDead = false;

    [Header("Configuration")]
    public float jumpForce = 5f;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool colorReRollActive;
    private float colorReRollTimer;
    private float colorReRollChance = 0.35f;

    private int streakCount = 0;
    private int bonusThreshold = 3;
    private int streakMultiplier = 1;

    [Header("Power Up")]
    public AudioClip powerUpSound;
    public GameObject powerUpEffect;
    private bool isInvincible = false;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;
    private int score = 0;
    private int highscore = 0;


    [Header("Audio Clips")]
    public AudioClip scoreSound;
    public AudioClip obstacleScoreSound;
    public AudioClip colourSwitchSound;

    [Header("Effects")]
    public GameObject scoreEffect;
    public GameObject obstacleScoreEffect;
    public GameObject colourSwitchEffect;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (rb != null)
            rb.gravityScale = 1f;

        SetColor(currentColor);
        ResetStreak();
        highscore = PlayerPrefs.GetInt("HighScore", 0);
        highscoreText.text = $"Best Score: {highscore}";
    }

    #region PC & Mobile Input
    void Update()
    {
        if (isDead) return;

        if (IsPressed())
            Jump();

        HandleColorReRoll();

        if (transform.position.y >= 6f)
        {
            transform.position = new Vector3(transform.position.x, 6f, transform.position.z);
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Jump()
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        PlaySFX(jumpSound);
    }

    bool IsPressed()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }
    #endregion

    #region Collision Handling
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // DEAD
        if (other.CompareTag("BallDeadZone"))
        {
            Die();
            return;
        }

        // COLLECTIBLE
        if (other.CompareTag("Collectible"))
        {
            AddScore(1 * streakMultiplier);
            UpdateStreak();
            CheckHighScore();
            CheckPowerUp();

            PlaySFX(scoreSound);

            if (scoreEffect != null)
            {
                GameObject scoreEffectInstance = Instantiate(scoreEffect, other.transform.position, Quaternion.identity);
                Destroy(scoreEffectInstance, 1f);
            }

            ObjectPool.Instance.ReturnToPool("Collectible", other.gameObject);
            return;
        }

        // COLOR SWITCH
        if (other.CompareTag("ColourSwitch"))
        {
            HandleColorSwitch(other.gameObject);
            return;
        }

        // OBSTACLE
        ObstaclePart part = other.GetComponent<ObstaclePart>();

        if (part != null)
        {
            if (part.color != currentColor && !isInvincible)
            {
                Die();
                return;
            }

            MoveLeft move = other.GetComponentInParent<MoveLeft>();

            if (move == null)
            {
                Debug.LogError("MoveLeft bulunamadı!");
                return;
            }

            GameObject root = move.gameObject;

            ObjectPool.Instance.ReturnToPool(move.poolTag, root);

            AddScore(1 * streakMultiplier);
            UpdateStreak();
            CheckHighScore();
            CheckPowerUp();

            PlaySFX(obstacleScoreSound);

            if (obstacleScoreEffect != null)
            {
                GameObject obstacleEffect = Instantiate(obstacleScoreEffect, root.transform.position, Quaternion.identity);
                Destroy(obstacleEffect, 1f);
            }
        }
    }

    public void HandleColorSwitch(GameObject switchObj)
    {
        SetColor((BallColor)Random.Range(0, 4));
        AddScore(1 * streakMultiplier);
        ResetStreak();
        CheckHighScore();
        CheckPowerUp();
        PlaySFX(colourSwitchSound);

        if (colourSwitchEffect != null)
        {
            GameObject colourEffect = Instantiate(colourSwitchEffect, switchObj.transform.position, Quaternion.identity);
            Destroy(colourEffect, 1f);
        }
        ObjectPool.Instance.ReturnToPool("ColourSwitch", switchObj);

        StartColorReRoll();
    }
    void StartColorReRoll()
    {
        colorReRollActive = true;
        colorReRollTimer = 0f;
    }
    void HandleColorReRoll()
    {
        if (!colorReRollActive) return;

        colorReRollTimer += Time.deltaTime;

        if (colorReRollTimer >= 5f)
        {
            colorReRollTimer = 0f;

            float t = Difficulty.GetMultiplier(score);
            float chance = colorReRollChance * t;

            if (Random.value < chance)
            {
                SetColor((BallColor)Random.Range(0, 4));
                PlaySFX(colourSwitchSound);
            }

            colorReRollActive = false;
        }
    }
    public void SetColor(BallColor color)
    {
        currentColor = color;

        if (sr == null) return;

        sr.color = color switch
        {
            BallColor.Green => new Color32(132, 177, 121, 255),
            BallColor.Blue => new Color32(122, 170, 206, 255),
            BallColor.Orange => new Color32(255, 170, 0, 255),
            BallColor.Red => new Color32(235, 76, 76, 255),
            _ => Color.white
        };
    }
    #endregion
    void PlaySFX(AudioClip clip)
    {
        if (!SFXManager.IsSFXOn) return;
        if (audioSource == null || clip == null) return;

        audioSource.PlayOneShot(clip);
    }
    #region Score & Power-Up Management
    void CheckHighScore()
    {
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("HighScore", highscore);
            PlayerPrefs.Save();

            highscoreText.text = $"Best Score: {highscore}";
        }
    }
    void AddScore(int amount)
    {
        score += amount * streakMultiplier;
        scoreText.text = score.ToString();
    }
    public int GetScore()
    {
        return score;
    }
    public float GetDifficultyMultiplier()
    {
        return Difficulty.GetMultiplier(score);
    }
    void CheckPowerUp()
    {
        if (score % 20 == 0)
        {
            ActivatePowerUp();
        }
    }
    void ActivatePowerUp()
    {
        if (isInvincible) return;

        StartCoroutine(PowerUpRoutine());
    }
    IEnumerator PowerUpRoutine()
    {
        isInvincible = true;

        PlaySFX(powerUpSound);

        GameObject fx = null;

        if (powerUpEffect != null)
        {
            fx = Instantiate(powerUpEffect, transform.position, Quaternion.identity);
            fx.transform.SetParent(transform);
        }

        float duration = powerUpSound != null ? powerUpSound.length : 2f;

        yield return new WaitForSeconds(duration);

        isInvincible = false;

        if (fx != null)
            Destroy(fx);
    }
    void UpdateStreak()
    {
        streakCount++;

        if (streakCount >= bonusThreshold)
        {
            streakMultiplier = Mathf.Min(streakMultiplier + 1, 5);
            streakCount = 0;
        }
    }
    void ResetStreak()
    {
        streakCount = 0;
        streakMultiplier = 1;
    }
    #endregion
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        streakCount = 0;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        sr.enabled = false;

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        PlaySFX(deathSound);

        if (controller != null)
            controller.StartRestart();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        sr.enabled = false;
        if (pauseButton != null) pauseButton.gameObject.SetActive(false);
        if (continueButton != null) continueButton.gameObject.SetActive(true);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        sr.enabled = true;
        if (pauseButton != null) pauseButton.gameObject.SetActive(true);
        if (continueButton != null) continueButton.gameObject.SetActive(false);
    }
}