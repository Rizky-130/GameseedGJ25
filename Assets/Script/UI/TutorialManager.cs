using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public GameObject overlay;
    public Image overlayImage;
    public GameObject howToPlayPanel;
    public CanvasGroup howToPlayGroup;

    public GameObject skipConfirmPanel;
    public GameObject suppliesSpotlight;
    public GameObject mixAndGuideSpotlight;
    public GameObject cookingSpotlight;
    public GameObject trashSpotlight;

    public GameObject endConfirmPanel;

    public string nextSceneName;
    public float sceneTransitionFadeDuration = 0.9f;
    public float sceneFadeInDuration = 0.9f;

    void Start()
    {
        HideAll();

        if (overlay != null)
            overlay.SetActive(true);

        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = 1f;
            overlayImage.color = c;
        }

        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);

        SetHowToPlayInteractable(true);

        StartCoroutine(FadeInFromBlack());
    }

    IEnumerator FadeInFromBlack()
    {
        if (overlayImage == null)
            yield break;

        float timer = 0f;
        Color c = overlayImage.color;

        while (timer < sceneFadeInDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, Mathf.Clamp01(timer / sceneFadeInDuration));
            overlayImage.color = c;
            yield return null;
        }

        c.a = 0f;
        overlayImage.color = c;
    }

    void HideAll()
    {
        SetActiveSafe(howToPlayPanel, false);
        SetActiveSafe(skipConfirmPanel, false);
        SetActiveSafe(suppliesSpotlight, false);
        SetActiveSafe(mixAndGuideSpotlight, false);
        SetActiveSafe(cookingSpotlight, false);
        SetActiveSafe(trashSpotlight, false);
        SetActiveSafe(endConfirmPanel, false);
    }

    void SetActiveSafe(GameObject go, bool state)
    {
        if (go != null)
            go.SetActive(state);
    }

    void SetHowToPlayInteractable(bool state)
    {
        if (howToPlayGroup == null)
            return;

        howToPlayGroup.interactable = state;
        howToPlayGroup.blocksRaycasts = state;
    }

    public void OnSkipPressed()
    {
        SetHowToPlayInteractable(false);
        SetActiveSafe(howToPlayPanel, false);
        SetActiveSafe(skipConfirmPanel, true);
    }

    public void OnSkipCancel()
    {
        SetActiveSafe(skipConfirmPanel, false);
        SetActiveSafe(howToPlayPanel, true);
        SetHowToPlayInteractable(true);
    }

    public void OnSkipConfirm()
    {
        EndTutorial();
    }

    public void OnContinuePressed()
    {
        SetActiveSafe(howToPlayPanel, false);
        SetActiveSafe(suppliesSpotlight, true);
    }

    public void OnSuppliesNext()
    {
        SetActiveSafe(suppliesSpotlight, false);
        SetActiveSafe(mixAndGuideSpotlight, true);
    }

    public void OnMixAndGuideNext()
    {
        SetActiveSafe(mixAndGuideSpotlight, false);
        SetActiveSafe(cookingSpotlight, true);
    }

    public void OnCookingNext()
    {
        SetActiveSafe(cookingSpotlight, false);
        SetActiveSafe(trashSpotlight, true);
    }

    public void OnTrashNext()
    {
        SetActiveSafe(trashSpotlight, false);
        SetActiveSafe(endConfirmPanel, true);
    }

    public void OnEndConfirmYes()
    {
        SetActiveSafe(endConfirmPanel, false);
        EndTutorial();
    }

    public void OnEndConfirmNo()
    {
        SetActiveSafe(endConfirmPanel, false);
        SetActiveSafe(suppliesSpotlight, true);
    }

    void EndTutorial()
    {
        SetActiveSafe(howToPlayPanel, false);
        SetActiveSafe(skipConfirmPanel, false);
        SetActiveSafe(suppliesSpotlight, false);
        SetActiveSafe(mixAndGuideSpotlight, false);
        SetActiveSafe(cookingSpotlight, false);
        SetActiveSafe(trashSpotlight, false);
        SetActiveSafe(endConfirmPanel, false);

        StartCoroutine(FadeToBlackAndLoadScene());
    }

    IEnumerator FadeToBlackAndLoadScene()
    {
        if (overlay != null)
            overlay.SetActive(true);

        if (overlayImage != null)
        {
            float timer = 0f;
            Color c = overlayImage.color;
            float startAlpha = c.a;

            while (timer < sceneTransitionFadeDuration)
            {
                timer += Time.deltaTime;
                c.a = Mathf.Lerp(startAlpha, 1f, Mathf.Clamp01(timer / sceneTransitionFadeDuration));
                overlayImage.color = c;
                yield return null;
            }

            c.a = 1f;
            overlayImage.color = c;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}