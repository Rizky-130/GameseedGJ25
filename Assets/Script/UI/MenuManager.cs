using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public string gameSceneName = "Sample Scene";

    public RectTransform menuPanel;
    public RectTransform creditPanel;
    public RectTransform settingsPanel;

    public RectTransform background;

    public Vector2 bgPositionMenu = new Vector2(0, 0);
    public Vector2 bgPositionCredits = new Vector2(200, 0);   // tweak in Inspector
    public Vector2 bgPositionSettings = new Vector2(-200, 0); // tweak in Inspector

    [Range(0f, 1f)]
    public float bgParallaxSpeed = 0.3f;

    public float tiltDegrees = 35f;

    private bool transitioning = false;

    void Start()
    {
        creditPanel.anchoredPosition = new Vector2(2340, 0);
        settingsPanel.anchoredPosition = new Vector2(-2528, 0);

        if (background != null)
            background.anchoredPosition = bgPositionMenu;
    }

    public void PlayGame() => SceneManager.LoadScene(gameSceneName);

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

    IEnumerator SlideAndSpin(RectTransform panel, Vector2 from, Vector2 to, float duration, float tiltSign)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = Mathf.SmoothStep(0, 1, time / duration);
            panel.anchoredPosition = Vector2.Lerp(from, to, t);
            float tiltT = Mathf.Sin(t * Mathf.PI);
            panel.localRotation = Quaternion.Euler(0, tiltSign * tiltDegrees * tiltT, 0);
            time += Time.deltaTime;
            yield return null;
        }
        panel.anchoredPosition = to;
        panel.localRotation = Quaternion.identity;
    }

    IEnumerator SlideWithBG(RectTransform panel, Vector2 from, Vector2 to, float duration, float tiltSign, Vector2 bgTarget)
    {
        float time = 0f;
        Vector2 bgFrom = background != null ? background.anchoredPosition : Vector2.zero;

        while (time < duration)
        {
            float t = Mathf.SmoothStep(0, 1, time / duration);

            panel.anchoredPosition = Vector2.Lerp(from, to, t);

            float tiltT = Mathf.Sin(t * Mathf.PI);
            panel.localRotation = Quaternion.Euler(0, tiltSign * tiltDegrees * tiltT, 0);

            if (background != null)
                background.anchoredPosition = Vector2.Lerp(bgFrom, bgTarget, t);

            time += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = to;
        panel.localRotation = Quaternion.identity;
        if (background != null)
            background.anchoredPosition = bgTarget;
    }

    IEnumerator OpenCreditsAnimation()
    {
        transitioning = true;
        float duration = 1.0f;

        yield return SlideWithBG(menuPanel, menuPanel.anchoredPosition, 
        new Vector2(-2340, 0), duration, -0.6f, bgPositionCredits);
        yield return SlideAndSpin(creditPanel, creditPanel.anchoredPosition, 
        new Vector2(-2340, 0), duration, -0.6f);

        transitioning = false;
    }

    IEnumerator CloseCreditsAnimation()
    {
        transitioning = true;
        float duration = 1.0f;

        yield return SlideAndSpin(creditPanel, creditPanel.anchoredPosition, 
        new Vector2(2340, 0), duration, 1f);
        yield return SlideWithBG(menuPanel, menuPanel.anchoredPosition,
        Vector2.zero, duration, 1f, bgPositionMenu);

        transitioning = false;
    }

    IEnumerator OpenSettingsAnimation()
    {
        transitioning = true;
        float duration = 1.0f;

        yield return SlideWithBG(menuPanel, menuPanel.anchoredPosition, -
        new Vector2(-2528, -100), duration, -0.6f, bgPositionSettings);
        yield return SlideAndSpin(settingsPanel, settingsPanel.anchoredPosition, 
        new Vector2(2528, -100), duration, -0.6f);

        transitioning = false;
    }

    IEnumerator CloseSettingsAnimation()
    {
        transitioning = true;
        float duration = 1.0f;

        yield return SlideAndSpin(settingsPanel, settingsPanel.anchoredPosition, 
        new Vector2(-2528, 0), duration, 1f);
        yield return SlideWithBG(menuPanel, menuPanel.anchoredPosition, 
        Vector2.zero, duration, 1f, bgPositionMenu);

        transitioning = false;
    }
}