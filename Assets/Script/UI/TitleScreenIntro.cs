using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TitleScreenIntro : MonoBehaviour
{
    [Header("Splash Logo")]
    public TextMeshProUGUI logoText;
    public Image blackOverlay;
    public float revealDuration = 0.6f;
    public float logoHoldDuration = 1.5f;
    public float logoFadeOutDuration = 0.6f;

    [Header("Menu Slide-In")]
    public RectTransform menuPanel;
    public float menuSlideDuration = 0.5f;
    public float menuOffscreenOffsetX = -1200f;
    public AnimationCurve menuSlideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("MC Slide + Color Reveal")]
    public Image mcImage;
    public RectTransform mcRectTransform;
    public float mcSlideDuration = 0.5f;
    public float mcOffscreenOffsetY = -400f;
    public float mcColorRevealDuration = 0.6f;

    [Header("Buttons Stagger Fade-In")]
    public CanvasGroup btnPlayGroup;
    public CanvasGroup btnCreditGroup;
    public CanvasGroup btnQuitGroup;
    public float buttonFadeDuration = 0.3f;
    public float buttonStaggerDelay = 0.2f;

    [Header("Settings Slide-In")]
    public RectTransform settingsButton;
    public float settingsSlideDuration = 0.5f;
    public float settingsOffscreenOffsetX = 1200f;

    [Header("Ghost Pop-In")]
    public RectTransform ghost1;
    public RectTransform ghost2;
    public float ghostPopDuration = 0.4f;
    public float ghostStaggerDelay = 0.25f;
    public AnimationCurve ghostPopCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public Vector2 ghost1StartOffset = new Vector2(40f, -150f);
    public Vector2 ghost1ControlOffset = new Vector2(-80f, -80f);

    public Vector2 ghost2StartOffset = new Vector2(-40f, -150f);
    public Vector2 ghost2ControlOffset = new Vector2(80f, -80f);

    [Header("Ghost Idle Sway")]
    public float swayAmount = 15f;
    public float swaySpeed = 1.5f;

    private float menuRestingX;
    private float settingsRestingX;
    private float mcRestingY;

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
            Color c = Color.black;
            c.a = 1f;
            mcImage.color = c;
        }

        if (mcRectTransform != null)
        {
            mcRestingY = mcRectTransform.anchoredPosition.y;

            Vector2 pos = mcRectTransform.anchoredPosition;
            pos.y = mcRestingY + mcOffscreenOffsetY;
            mcRectTransform.anchoredPosition = pos;
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

        if (ghost1 != null)
            ghost1.localScale = Vector3.zero;

        if (ghost2 != null)
            ghost2.localScale = Vector3.zero;

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
            yield return FadeGraphicAlpha(blackOverlay, 1f, 0f, revealDuration);
        }

        yield return new WaitForSeconds(logoHoldDuration);

        if (logoText != null)
        {
            yield return FadeGraphicAlpha(logoText, 1f, 0f, logoFadeOutDuration);
        }

        yield return SlideMenuIn();

        yield return SlideMCUp();

        yield return RevealMCColor();

        yield return PopInGhosts();

        StartCoroutine(SwayGhost(ghost1));
        StartCoroutine(SwayGhost(ghost2));

        yield return FadeInButtonsStaggered();

        yield return SlideSettingsIn();
    }

    IEnumerator SlideMCUp()
    {
        if (mcRectTransform == null)
            yield break;

        float startY = mcRectTransform.anchoredPosition.y;
        float timer = 0f;

        while (timer < mcSlideDuration)
        {
            timer += Time.deltaTime;
            float t = menuSlideCurve.Evaluate(Mathf.Clamp01(timer / mcSlideDuration));

            Vector2 pos = mcRectTransform.anchoredPosition;
            pos.y = Mathf.Lerp(startY, mcRestingY, t);
            mcRectTransform.anchoredPosition = pos;

            yield return null;
        }

        Vector2 finalPos = mcRectTransform.anchoredPosition;
        finalPos.y = mcRestingY;
        mcRectTransform.anchoredPosition = finalPos;
    }

    IEnumerator RevealMCColor()
    {
        if (mcImage == null)
            yield break;

        float timer = 0f;

        while (timer < mcColorRevealDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / mcColorRevealDuration);

            Color c = Color.Lerp(Color.black, Color.white, t);
            c.a = 1f;
            mcImage.color = c;

            yield return null;
        }

        mcImage.color = Color.white;
    }

    IEnumerator PopInGhosts()
    {
        yield return PopInGhost(ghost1, ghost1StartOffset, ghost1ControlOffset);
        yield return new WaitForSeconds(ghostStaggerDelay);
        yield return PopInGhost(ghost2, ghost2StartOffset, ghost2ControlOffset);
    }

    IEnumerator PopInGhost(RectTransform ghost, Vector2 startOffset, Vector2 controlOffset)
    {
        if (ghost == null)
            yield break;

        Vector2 restingPos = ghost.anchoredPosition;
        Vector2 startPos = restingPos + startOffset;
        Vector2 controlPos = restingPos + controlOffset;

        ghost.anchoredPosition = startPos;
        ghost.localScale = Vector3.zero;

        float timer = 0f;

        while (timer < ghostPopDuration)
        {
            timer += Time.deltaTime;
            float t = ghostPopCurve.Evaluate(Mathf.Clamp01(timer / ghostPopDuration));

            ghost.anchoredPosition = QuadraticBezier(startPos, controlPos, restingPos, t);
            ghost.localScale = Vector3.one * t;

            yield return null;
        }

        ghost.anchoredPosition = restingPos;
        ghost.localScale = Vector3.one;
    }

    Vector2 QuadraticBezier(Vector2 a, Vector2 control, Vector2 b, float t)
    {
        Vector2 ac = Vector2.Lerp(a, control, t);
        Vector2 cb = Vector2.Lerp(control, b, t);
        return Vector2.Lerp(ac, cb, t);
    }

    IEnumerator SwayGhost(RectTransform ghost)
    {
        if (ghost == null)
            yield break;

        float baseX = ghost.anchoredPosition.x;
        float seed = Random.Range(0f, Mathf.PI * 2f);

        while (true)
        {
            float offsetX = Mathf.Sin(Time.time * swaySpeed + seed) * swayAmount;

            Vector2 pos = ghost.anchoredPosition;
            pos.x = baseX + offsetX;
            ghost.anchoredPosition = pos;

            yield return null;
        }
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