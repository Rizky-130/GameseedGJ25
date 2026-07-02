using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public string gameSceneName = "Sample Scene";

    public RectTransform menuPanel;
    public RectTransform creditPanel;
    public RectTransform settingsPanel;

    public Vector2 mainMenuCenter;
    public Vector2 mainMenuLeft;
    public Vector2 creditHiddenRight;
    public Vector2 creditCenter;
    public Vector2 creditHiddenLeft;

    [Header("UI Locking")]
    [Tooltip("CanvasGroup covering all menu buttons. Gets disabled the moment Play is pressed.")]
    public CanvasGroup menuCanvasGroup;

    [Header("Play Transition")]
    public AudioSource audioSource;
    public AudioClip playSound;
    public CanvasGroup fadeOverlay;
    public float fadeDuration = 0.8f;
    public float soundDelayBeforeFade = 0f;

    [System.Serializable]
    public class ParallaxLayer
    {
        public RectTransform layer;
        public float speed = 1f;
        private Vector2 restingPosition;
        private bool initialized = false;

        public void CacheRestingPosition()
        {
            if (layer == null || initialized)
                return;

            restingPosition = layer.anchoredPosition;
            initialized = true;
        }

        public Vector2 RestingPosition => restingPosition;
    }

    public ParallaxLayer[] parallaxLayers;
    public float parallaxDistance = 300f;
    public float parallaxExtraDuration = 0.4f;

    private bool transitioning = false;

    void Start()
    {
        creditPanel.anchoredPosition = creditHiddenRight;
        settingsPanel.anchoredPosition = creditHiddenLeft;

        foreach (var p in parallaxLayers)
            p.CacheRestingPosition();

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 1f;
            fadeOverlay.gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            fadeOverlay.alpha = Mathf.SmoothStep(1f, 0f, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        fadeOverlay.alpha = 0f;
        fadeOverlay.gameObject.SetActive(false);
    }

    public void PlayGame()
    {
        if (transitioning) return;

        // Lock out all menu buttons immediately
        if (menuCanvasGroup != null)
        {
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
        }

        StopAllCoroutines();
        StartCoroutine(PlayGameTransition());
    }

    IEnumerator PlayGameTransition()
    {
        transitioning = true;

        // Fire the sound but don't wait for it — it plays underneath the fade/load
        if (audioSource != null && playSound != null)
            audioSource.PlayOneShot(playSound);

        if (soundDelayBeforeFade > 0f)
            yield return new WaitForSeconds(soundDelayBeforeFade);

        // Fade screen to black
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            float t = 0f;
            while (t < fadeDuration)
            {
                fadeOverlay.alpha = Mathf.SmoothStep(0f, 1f, t / fadeDuration);
                t += Time.deltaTime;
                yield return null;
            }
            fadeOverlay.alpha = 1f;
        }

        DontDestroyOnLoad(fadeOverlay.gameObject);
        DontDestroyOnLoad(gameObject);

        AsyncOperation load = SceneManager.LoadSceneAsync(gameSceneName);
        yield return load;

        if (fadeOverlay != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                fadeOverlay.alpha = Mathf.SmoothStep(1f, 0f, t / fadeDuration);
                t += Time.deltaTime;
                yield return null;
            }
            fadeOverlay.alpha = 0f;
            fadeOverlay.gameObject.SetActive(false);
        }

        Destroy(gameObject);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenCredits()
    {
        StopAllCoroutines();
        StartCoroutine(OpenCreditsAnimation());
    }

    public void ReturnFromCredits()
    {
        StopAllCoroutines();
        StartCoroutine(CloseCreditsAnimation());
    }

    public void OpenSettings()
    {
        StopAllCoroutines();
        StartCoroutine(OpenSettingsAnimation());
    }

    public void ReturnFromSettings()
    {
        StopAllCoroutines();
        StartCoroutine(CloseSettingsAnimation());
    }

    IEnumerator Slide(RectTransform panel, Vector2 from, Vector2 to, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            float t = Mathf.SmoothStep(0, 1, time / duration);
            panel.anchoredPosition = Vector2.Lerp(from, to, t);
            time += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = to;
    }

    IEnumerator PanParallax(float directionSign, float panelDuration)
    {
        float duration = panelDuration + parallaxExtraDuration;
        float time = 0f;

        while (time < duration)
        {
            float t = Mathf.SmoothStep(0, 1, time / duration);

            foreach (var p in parallaxLayers)
            {
                if (p.layer == null)
                    continue;

                Vector2 pos = p.layer.anchoredPosition;
                pos.x = p.RestingPosition.x + directionSign * parallaxDistance * p.speed * t;
                p.layer.anchoredPosition = pos;
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ReturnParallax(float panelDuration)
    {
        float duration = panelDuration + parallaxExtraDuration;
        float time = 0f;

        Vector2[] startPositions = new Vector2[parallaxLayers.Length];
        for (int i = 0; i < parallaxLayers.Length; i++)
            if (parallaxLayers[i].layer != null)
                startPositions[i] = parallaxLayers[i].layer.anchoredPosition;

        while (time < duration)
        {
            float t = Mathf.SmoothStep(0, 1, time / duration);

            for (int i = 0; i < parallaxLayers.Length; i++)
            {
                var p = parallaxLayers[i];
                if (p.layer == null)
                    continue;

                p.layer.anchoredPosition = Vector2.Lerp(startPositions[i], p.RestingPosition, t);
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (var p in parallaxLayers)
            if (p.layer != null)
                p.layer.anchoredPosition = p.RestingPosition;
    }

    IEnumerator OpenCreditsAnimation()
    {
        transitioning = true;

        float duration = 1.0f;

        StartCoroutine(PanParallax(1f, duration));

        Vector2 menuStart = menuPanel.anchoredPosition;
        Vector2 menuTarget = new Vector2(-2340, 0);

        yield return Slide(menuPanel, menuStart, menuTarget, duration);

        yield return new WaitForSeconds(0.1f);

        Vector2 creditStart = creditPanel.anchoredPosition;
        Vector2 creditTarget = new Vector2(-2340, 0);

        yield return Slide(creditPanel, creditStart, creditTarget, duration);

        transitioning = false;
    }

    IEnumerator CloseCreditsAnimation()
    {
        transitioning = true;

        float duration = 1.0f;

        StartCoroutine(ReturnParallax(duration));

        Vector2 creditStart = creditPanel.anchoredPosition;
        Vector2 creditTarget = new Vector2(0, 0);

        yield return Slide(creditPanel, creditStart, creditTarget, duration);

        yield return new WaitForSeconds(0.1f);

        Vector2 menuStart = menuPanel.anchoredPosition;
        Vector2 menuTarget = Vector2.zero;

        yield return Slide(menuPanel, menuStart, menuTarget, duration);

        transitioning = false;
    }

    IEnumerator OpenSettingsAnimation()
    {
        transitioning = true;

        float duration = 1.0f;

        StartCoroutine(PanParallax(-1f, duration));

        Vector2 menuStart = menuPanel.anchoredPosition;
        Vector2 menuTarget = new Vector2(2528, 0);

        yield return Slide(menuPanel, menuStart, menuTarget, duration);

        yield return new WaitForSeconds(0.1f);

        Vector2 settingsStart = settingsPanel.anchoredPosition;
        Vector2 settingsTarget = new Vector2(2528, -100);

        yield return Slide(settingsPanel, settingsStart, settingsTarget, duration);

        transitioning = false;
    }

    IEnumerator CloseSettingsAnimation()
    {
        transitioning = true;

        float duration = 1.0f;

        StartCoroutine(ReturnParallax(duration));

        Vector2 settingsStart = settingsPanel.anchoredPosition;
        Vector2 settingsTarget = new Vector2(0, -100);

        yield return Slide(settingsPanel, settingsStart, settingsTarget, duration);

        yield return new WaitForSeconds(0.1f);

        Vector2 menuStart = menuPanel.anchoredPosition;
        Vector2 menuTarget = Vector2.zero;

        yield return Slide(menuPanel, menuStart, menuTarget, duration);

        transitioning = false;
    }
}