using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenIntro : MonoBehaviour
{
    public TextMeshProUGUI logoText;
    public Image blackOverlay;
    public float revealDuration = 0.6f;
    public float logoHoldDuration = 1.5f;
    public float logoFadeOutDuration = 0.6f;

    public RectTransform menuPanel;
    public float menuSlideDuration = 0.5f;
    public float menuOffscreenOffsetX = -1200f;
    public AnimationCurve menuSlideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public Image mcImage;
    public int mcFlickerCount = 5;
    public float mcFlickerInterval = 0.08f;

    public CanvasGroup btnPlayGroup;
    public CanvasGroup btnCreditGroup;
    public CanvasGroup btnQuitGroup;
    public float buttonFadeDuration = 0.3f;
    public float buttonStaggerDelay = 0.2f;

    public RectTransform settingsButton;
    public float settingsSlideDuration = 0.5f;
    public float settingsOffscreenOffsetX = 1200f;

    private float menuRestingX;
    private float settingsRestingX;

    void Start()
    {
        if (logoText != null)
        {
            Color c = logoText.color;
            c.a = 1f;
            logoText.color = c;
        }

        if (blackOverlay != null)
        {
            Color c = blackOverlay.color;
            c.a = 1f;
            blackOverlay.color = c;
        }

        if (menuPanel != null)
        {
            menuRestingX = menuPanel.anchoredPosition.x;

            Vector2 pos = menuPanel.anchoredPosition;
            pos.x = menuRestingX + menuOffscreenOffsetX;
            menuPanel.anchoredPosition = pos;
        }

        if (mcImage != null)
        {
            Color c = mcImage.color;
            c.a = 0f;
            mcImage.color = c;
        }

        SetButtonHidden(btnPlayGroup);
        SetButtonHidden(btnCreditGroup);
        SetButtonHidden(btnQuitGroup);

        if (settingsButton != null)
        {
            settingsRestingX = settingsButton.anchoredPosition.x;

            Vector2 pos = settingsButton.anchoredPosition;
            pos.x = settingsRestingX + settingsOffscreenOffsetX;
            settingsButton.anchoredPosition = pos;
        }

        StartCoroutine(PlayIntroSequence());
    }

    void SetButtonHidden(CanvasGroup group)
    {
        if (group == null)
            return;

        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    IEnumerator PlayIntroSequence()
    {
        if (blackOverlay != null)
        {
            yield return FadeGraphicAlpha(blackOverlay, 2f, 0f, revealDuration);
        }

        yield return new WaitForSeconds(logoHoldDuration);

        if (logoText != null)
        {
            yield return FadeGraphicAlpha(logoText, 1f, 0f, logoFadeOutDuration);
        }

        yield return SlideMenuIn();

        yield return FlickerMC();

        yield return FadeInButtonsStaggered();

        yield return SlideSettingsIn();
    }

    IEnumerator FlickerMC()
    {
        if (mcImage == null)
            yield break;

        for (int i = 0; i < mcFlickerCount; i++)
        {
            SetGraphicAlpha(mcImage, 1f);
            yield return new WaitForSeconds(mcFlickerInterval);

            SetGraphicAlpha(mcImage, 0f);
            yield return new WaitForSeconds(mcFlickerInterval);
        }

        SetGraphicAlpha(mcImage, 1f);
    }

    void SetGraphicAlpha(Graphic graphic, float a)
    {
        Color c = graphic.color;
        c.a = a;
        graphic.color = c;
    }

    IEnumerator FadeInButtonsStaggered()
    {
        yield return FadeInButton(btnPlayGroup);
        yield return new WaitForSeconds(buttonStaggerDelay);

        yield return FadeInButton(btnCreditGroup);
        yield return new WaitForSeconds(buttonStaggerDelay);

        yield return FadeInButton(btnQuitGroup);
    }

    IEnumerator FadeInButton(CanvasGroup group)
    {
        if (group == null)
            yield break;

        float timer = 0f;

        while (timer < buttonFadeDuration)
        {
            timer += Time.deltaTime;
            group.alpha = Mathf.Clamp01(timer / buttonFadeDuration);
            yield return null;
        }

        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    IEnumerator SlideSettingsIn()
    {
        if (settingsButton == null)
            yield break;

        float startX = settingsButton.anchoredPosition.x;
        float timer = 0f;

        while (timer < settingsSlideDuration)
        {
            timer += Time.deltaTime;
            float t = menuSlideCurve.Evaluate(Mathf.Clamp01(timer / settingsSlideDuration));

            Vector2 pos = settingsButton.anchoredPosition;
            pos.x = Mathf.Lerp(startX, settingsRestingX, t);
            settingsButton.anchoredPosition = pos;

            yield return null;
        }

        Vector2 finalPos = settingsButton.anchoredPosition;
        finalPos.x = settingsRestingX;
        settingsButton.anchoredPosition = finalPos;
    }

    IEnumerator FadeGraphicAlpha(Graphic graphic, float from, float to, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(from, to, Mathf.Clamp01(timer / duration));

            Color c = graphic.color;
            c.a = a;
            graphic.color = c;

            yield return null;
        }

        Color final = graphic.color;
        final.a = to;
        graphic.color = final;
    }

    IEnumerator SlideMenuIn()
    {
        if (menuPanel == null)
            yield break;

        float startX = menuPanel.anchoredPosition.x;
        float timer = 0f;

        while (timer < menuSlideDuration)
        {
            timer += Time.deltaTime;
            float t = menuSlideCurve.Evaluate(Mathf.Clamp01(timer / menuSlideDuration));

            Vector2 pos = menuPanel.anchoredPosition;
            pos.x = Mathf.Lerp(startX, menuRestingX, t);
            menuPanel.anchoredPosition = pos;

            yield return null;
        }

        Vector2 finalPos = menuPanel.anchoredPosition;
        finalPos.x = menuRestingX;
        menuPanel.anchoredPosition = finalPos;
    }
}