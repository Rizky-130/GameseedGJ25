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

    private bool transitioning = false;

    void Start()
    {
        creditPanel.anchoredPosition = creditHiddenRight;

        settingsPanel.anchoredPosition = creditHiddenLeft;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
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

    IEnumerator OpenCreditsAnimation()
    {
        transitioning = true;

        float duration = 1.0f;
        float time = 0f;

        Vector2 menuStart = menuPanel.anchoredPosition;
        Vector2 menuTarget = new Vector2(-2340, 0);

        while (time < duration)
        {
            float t = Mathf.SmoothStep(0, 1, time / duration);

            menuPanel.anchoredPosition =
                Vector2.Lerp(menuStart, menuTarget, t);

            time += Time.deltaTime;
            yield return null;
        }

        menuPanel.anchoredPosition = menuTarget;

        yield return new WaitForSeconds(0.1f);

        time = 0f;

        Vector2 creditStart = creditPanel.anchoredPosition;
        Vector2 creditTarget = new Vector2(-2340, 0);

        while (time < duration)
        {
            float t = Mathf.SmoothStep(0, 1, time / duration);

            creditPanel.anchoredPosition =
                Vector2.Lerp(creditStart, creditTarget, t);

            time += Time.deltaTime;
            yield return null;
        }

        creditPanel.anchoredPosition = creditTarget;

        transitioning = false;
    }

    IEnumerator CloseCreditsAnimation()
    {
        transitioning = true;

        float duration = 1.0f;
        float time = 0f;

        Vector2 creditStart = creditPanel.anchoredPosition;
        Vector2 creditTarget = new Vector2(1067, 0);

        while (time < duration)
        {
            float t = Mathf.SmoothStep(0, 1, time / duration);

            creditPanel.anchoredPosition =
                Vector2.Lerp(creditStart, creditTarget, t);

            time += Time.deltaTime;
            yield return null;
        }

        creditPanel.anchoredPosition = creditTarget;

        yield return new WaitForSeconds(0.1f);
        time = 0f;

        Vector2 menuStart = menuPanel.anchoredPosition;
        Vector2 menuTarget = Vector2.zero;

        while (time < duration)
        {
            float t = Mathf.SmoothStep(0, 1, time / duration);

            menuPanel.anchoredPosition =
                Vector2.Lerp(menuStart, menuTarget, t);

            time += Time.deltaTime;
            yield return null;
        }

        menuPanel.anchoredPosition = menuTarget;

        transitioning = false;
    }

    IEnumerator OpenSettingsAnimation()
{
    transitioning = true;

    float duration = 1.0f;
    float time = 0f;

    Vector2 menuStart = menuPanel.anchoredPosition;
    Vector2 menuTarget = new Vector2(2528, 0);

    while (time < duration)
    {
        float t = Mathf.SmoothStep(0, 1, time / duration);

        menuPanel.anchoredPosition =
            Vector2.Lerp(menuStart, menuTarget, t);

        time += Time.deltaTime;
        yield return null;
    }

    menuPanel.anchoredPosition = menuTarget;

    yield return new WaitForSeconds(0.1f);

    time = 0f;

    Vector2 settingsStart = settingsPanel.anchoredPosition;
    Vector2 settingsTarget = new Vector2(2528, -100);

    while (time < duration)
    {
        float t = Mathf.SmoothStep(0, 1, time / duration);

        settingsPanel.anchoredPosition =
            Vector2.Lerp(settingsStart, settingsTarget, t);

        time += Time.deltaTime;
        yield return null;
    }

    settingsPanel.anchoredPosition = settingsTarget;

    transitioning = false;
}

IEnumerator CloseSettingsAnimation()
{
    transitioning = true;

    float duration = 1.0f;
    float time = 0f;

    Vector2 settingsStart = settingsPanel.anchoredPosition;
    Vector2 settingsTarget = new Vector2(0, 0);

    while (time < duration)
    {
        float t = Mathf.SmoothStep(0, 1, time / duration);

        settingsPanel.anchoredPosition =
            Vector2.Lerp(settingsStart, settingsTarget, t);

        time += Time.deltaTime;
        yield return null;
    }

    settingsPanel.anchoredPosition = settingsTarget;

    yield return new WaitForSeconds(0.1f);

    time = 0f;

    Vector2 menuStart = menuPanel.anchoredPosition;
    Vector2 menuTarget = Vector2.zero;

    while (time < duration)
    {
        float t = Mathf.SmoothStep(0, 1, time / duration);

        menuPanel.anchoredPosition =
            Vector2.Lerp(menuStart, menuTarget, t);

        time += Time.deltaTime;
        yield return null;
    }

    menuPanel.anchoredPosition = menuTarget;

    transitioning = false;
}
}