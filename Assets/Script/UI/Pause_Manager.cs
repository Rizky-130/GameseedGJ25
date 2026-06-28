using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuManager : MonoBehaviour
{
    public RectTransform shutter;
    public RectTransform blackTintRect;
    public CanvasGroup pausePanelGroup;

    public RectTransform settingsShutter;
    public float settingsClosedY = 0f;
    public float settingsOpenY = 1000f;
    public bool settingsIsOpen = false;

    public GameObject confirmation1;
    public GameObject confirmation2;
    public string titleScreenScene = "Title_Screen";

    public float closedY = 0f;
    public float openY = 1000f;
    public float shutterDuration = 0.35f;
    public AnimationCurve shutterCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public Transform cameraTransform;
    public float shakeDuration = 0.15f;
    public float shakeStrength = 5f;

    private bool isPaused = false;
    private bool isAnimating = false;
    private Vector3 camOriginalPos;
    private float tintOffsetY;

    void Start()
    {
        if (shutter != null && blackTintRect != null)
            tintOffsetY = blackTintRect.anchoredPosition.y - shutter.anchoredPosition.y;

        SetShutterPosition(openY);

        if (pausePanelGroup != null)
        {
            pausePanelGroup.alpha = 0f;
            pausePanelGroup.interactable = false;
            pausePanelGroup.blocksRaycasts = false;
        }

        if (cameraTransform != null)
            camOriginalPos = cameraTransform.localPosition;

        if (confirmation1 != null)
            confirmation1.SetActive(false);

        if (confirmation2 != null)
            confirmation2.SetActive(false);

        if (settingsShutter != null)
            SetSettingsShutterPosition(settingsOpenY);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isAnimating)
        {
            if (settingsIsOpen)
            {
                OnSettingsBackPressed();
            }
            else
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (isPaused || isAnimating)
            return;

        isPaused = true;
        Time.timeScale = 0f;

        StartCoroutine(CloseShutter());
    }

    public void ResumeGame()
    {
        if (!isPaused || isAnimating)
            return;

        StartCoroutine(OpenShutter());
    }

    IEnumerator CloseShutter()
    {
        isAnimating = true;

        if (pausePanelGroup != null)
        {
            pausePanelGroup.alpha = 1f;
            pausePanelGroup.interactable = false;
            pausePanelGroup.blocksRaycasts = false;
        }

        yield return AnimateShutter(openY, closedY);

        if (cameraTransform != null)
            yield return Shake();

        if (pausePanelGroup != null)
        {
            pausePanelGroup.interactable = true;
            pausePanelGroup.blocksRaycasts = true;
        }

        isAnimating = false;
    }

    IEnumerator OpenShutter()
    {
        isAnimating = true;

        if (pausePanelGroup != null)
        {
            pausePanelGroup.interactable = false;
            pausePanelGroup.blocksRaycasts = false;
        }

        yield return AnimateShutter(closedY, openY);

        isPaused = false;
        Time.timeScale = 1f;
        isAnimating = false;
    }

    IEnumerator AnimateShutter(float fromY, float toY)
    {
        yield return AnimateValue(fromY, toY, SetShutterPosition);
    }

    IEnumerator AnimateValue(float fromY, float toY, System.Action<float> applyY)
    {
        float timer = 0f;

        while (timer < shutterDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = shutterCurve.Evaluate(Mathf.Clamp01(timer / shutterDuration));
            applyY(Mathf.Lerp(fromY, toY, t));
            yield return null;
        }

        applyY(toY);
    }

    void SetShutterPosition(float y)
    {
        if (shutter != null)
        {
            Vector2 pos = shutter.anchoredPosition;
            pos.y = y;
            shutter.anchoredPosition = pos;
        }

        if (blackTintRect != null)
        {
            Vector2 tintPos = blackTintRect.anchoredPosition;
            tintPos.y = y + tintOffsetY;
            blackTintRect.anchoredPosition = tintPos;
        }
    }

    void SetSettingsShutterPosition(float y)
    {
        if (settingsShutter == null)
            return;

        Vector2 pos = settingsShutter.anchoredPosition;
        pos.y = y;
        settingsShutter.anchoredPosition = pos;
    }

    IEnumerator Shake()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / shakeDuration);
            float offsetY = -shakeStrength * Mathf.Sin(t * Mathf.PI) * (1f - t);

            cameraTransform.localPosition = camOriginalPos + new Vector3(0f, offsetY, 0f);

            yield return null;
        }

        cameraTransform.localPosition = camOriginalPos;
    }

    public void OnQuitPressed()
    {
        if (confirmation1 != null)
            confirmation1.SetActive(true);
    }

    public void OnConfirmation1Confirm()
    {
        if (confirmation1 != null)
            confirmation1.SetActive(false);

        if (confirmation2 != null)
            confirmation2.SetActive(true);
    }

    public void OnConfirmation1Cancel()
    {
        if (confirmation1 != null)
            confirmation1.SetActive(false);
    }

    public void OnConfirmation2Confirm()
    {
        if (confirmation2 != null)
            confirmation2.SetActive(false);

        Time.timeScale = 1f;
        SceneManager.LoadScene(titleScreenScene);
    }

    public void OnConfirmation2Cancel()
    {
        if (confirmation2 != null)
            confirmation2.SetActive(false);
    }

    public void OnSettingsPressed()
    {
        if (settingsShutter == null || settingsIsOpen)
            return;

        settingsIsOpen = true;
        StartCoroutine(CloseSettingsShutter());
    }

    IEnumerator CloseSettingsShutter()
    {
        yield return AnimateValue(settingsOpenY, settingsClosedY, SetSettingsShutterPosition);

        if (cameraTransform != null)
            yield return Shake();
    }

    public void OnSettingsBackPressed()
    {
        if (settingsShutter == null || !settingsIsOpen)
            return;

        settingsIsOpen = false;
        StartCoroutine(AnimateValue(settingsClosedY, settingsOpenY, SetSettingsShutterPosition));
    }
}