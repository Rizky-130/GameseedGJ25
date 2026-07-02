using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Core")]
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;
    public string mainMenuScene = "Title_Screen";

    [Header("Stat Display")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI customerServedText;
    public TextMeshProUGUI highScoreText;

    [Header("Hit Sequence")]
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip gameOverRevealSound;
    public Transform cameraTransform;
    public float shakeDuration = 0.4f;
    public float shakeStrength = 20f;
    public float delayBeforeReveal = 0.6f;

    [Header("Stat Reveal")]
    public float statCountUpDuration = 0.6f;
    public float statStaggerDelay = 0.3f;

    private const string HighScoreKey = "HighScore";

    private bool gameOverShown = false;
    private float elapsedTime = 0f;
    private Vector3 camOriginalPos;

    // Store stats before scene reload
    private static int savedScore = 0;
    private static float savedTime = 0f;
    private static int savedCustomerServed = 0;
    private static bool pendingGameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (cameraTransform != null)
            camOriginalPos = cameraTransform.localPosition;

        // If we reloaded for game over, show it immediately
        if (pendingGameOver)
        {
            pendingGameOver = false;
            gameOverShown = true;
            Time.timeScale = 0f;
            StartCoroutine(GameOverSequenceAfterReload());
        }
    }

    void Update()
    {
        if (!gameOverShown)
            elapsedTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G) && !gameOverShown)
        {
            ShowGameOver();
        }
    }

    public void ShowGameOver()
    {
        if (gameOverShown)
            return;

        gameOverShown = true;

        // Save stats before reload
        if (GameManager.Instance != null)
        {
            savedScore = GameManager.Instance.score;
            savedCustomerServed = GameManager.Instance.customerServed;
        }
        savedTime = elapsedTime;
        pendingGameOver = true;

        StartCoroutine(HitThenReload());
    }

    IEnumerator HitThenReload()
    {
        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        if (cameraTransform != null)
            yield return Shake();

        yield return new WaitForSecondsRealtime(delayBeforeReveal);

        // Reload the scene — Start() will detect pendingGameOver and show the canvas
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator GameOverSequenceAfterReload()
    {
        if (audioSource != null && gameOverRevealSound != null)
            audioSource.PlayOneShot(gameOverRevealSound);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        yield return FadeIn();

        yield return RevealStats();
    }

    IEnumerator Shake()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / shakeDuration);
            float damper = 1f - t;
            float offsetX = Random.Range(-1f, 1f) * shakeStrength * damper;
            float offsetY = Random.Range(-1f, 1f) * shakeStrength * damper;

            cameraTransform.localPosition = camOriginalPos + new Vector3(offsetX, offsetY, 0f);

            yield return null;
        }

        cameraTransform.localPosition = camOriginalPos;
    }

    IEnumerator RevealStats()
    {
        int finalScore = savedScore;
        int finalServed = savedCustomerServed;
        float finalTime = savedTime;

        int savedHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        int newHighScore = Mathf.Max(savedHighScore, finalScore);

        if (newHighScore != savedHighScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, newHighScore);
            PlayerPrefs.Save();
        }

        if (scoreText != null)
        {
            yield return CountUpInt(scoreText, finalScore);
            yield return new WaitForSecondsRealtime(statStaggerDelay);
        }

        if (timeText != null)
        {
            yield return CountUpTime(timeText, finalTime);
            yield return new WaitForSecondsRealtime(statStaggerDelay);
        }

        if (customerServedText != null)
        {
            yield return CountUpInt(customerServedText, finalServed);
            yield return new WaitForSecondsRealtime(statStaggerDelay);
        }

        if (highScoreText != null)
        {
            yield return CountUpInt(highScoreText, newHighScore);
        }
    }

    IEnumerator CountUpInt(TextMeshProUGUI label, int targetValue)
    {
        float timer = 0f;

        while (timer < statCountUpDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / statCountUpDuration);
            int current = Mathf.RoundToInt(Mathf.Lerp(0, targetValue, t));
            label.text = current.ToString();
            yield return null;
        }

        label.text = targetValue.ToString();
    }

    IEnumerator CountUpTime(TextMeshProUGUI label, float targetSeconds)
    {
        float timer = 0f;

        while (timer < statCountUpDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / statCountUpDuration);
            float current = Mathf.Lerp(0f, targetSeconds, t);
            label.text = FormatTime(current);
            yield return null;
        }

        label.text = FormatTime(targetSeconds);
    }

    string FormatTime(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, secs);
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}