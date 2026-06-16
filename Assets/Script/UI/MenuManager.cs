using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public string gameSceneName = "Sample Scene";

    public GameObject TitleText;
    public GameObject playButton;
    public GameObject continueButton;
    public GameObject settingsButton;
    public GameObject creditButton;
    public GameObject quitButton;

    public GameObject returnButton;

    [Header("Credits")]
    public RectTransform creditPanel;
    public Vector2 hiddenPosition;
    public Vector2 shownPosition;

    private bool creditsOpen = false;

    void Start()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        if (returnButton != null)
        {
            returnButton.SetActive(false);
        }

        if (creditPanel != null)
        {
            creditPanel.anchoredPosition = hiddenPosition;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        Debug.Log("Settings clicked.");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenCredits()
    {
        if (creditsOpen)
            return;

        creditsOpen = true;

        TitleText.SetActive(false);
        playButton.SetActive(false);
        settingsButton.SetActive(false);
        creditButton.SetActive(false);
        quitButton.SetActive(false);

        returnButton.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(SlidePanel(shownPosition));
    }

    public void ReturnFromCredits()
    {
        if (!creditsOpen)
            return;

        StopAllCoroutines();
        StartCoroutine(CloseCredits());
    }

    IEnumerator CloseCredits()
    {
        yield return StartCoroutine(SlidePanel(hiddenPosition));

        TitleText.SetActive(true);
        playButton.SetActive(true);
        settingsButton.SetActive(true);
        creditButton.SetActive(true);
        quitButton.SetActive(true);

        returnButton.SetActive(false);

        creditsOpen = false;
    }

    IEnumerator SlidePanel(Vector2 targetPos)
    {
        float duration = 0.5f;
        float time = 0f;

        Vector2 startPos = creditPanel.anchoredPosition;

        while (time < duration)
        {
            creditPanel.anchoredPosition =
                Vector2.Lerp(startPos, targetPos, time / duration);

            time += Time.deltaTime;

            yield return null;
        }

        creditPanel.anchoredPosition = targetPos;
    }
}