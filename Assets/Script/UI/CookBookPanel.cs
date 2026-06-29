using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CookBookPanel : MonoBehaviour
{
    public RectTransform panel;
    public Image overlayImage;
    [Range(0f, 1f)]
    public float overlayTargetAlpha = 0.6f;
    public Button openButton;
    public Button exitButton;
    public GameObject[] foodImages;
    public Button[] foodButtons;

    public float animDuration = 0.4f;
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float _panelOnscreenX;
    private float _panelOffscreenX;
    private bool _isOpen;
    private Coroutine _activeCoroutine;

    void Awake()
    {
        _panelOnscreenX  = panel.anchoredPosition.x;
        _panelOffscreenX = _panelOnscreenX + panel.rect.width;

        SetPanelX(_panelOffscreenX);
        SetOverlayAlpha(0f);
        overlayImage.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);

        if (openButton) openButton.onClick.AddListener(Open);
        if (exitButton) exitButton.onClick.AddListener(Close);

        for (int i = 0; i < foodButtons.Length; i++)
        {
            int index = i;
            foodButtons[i].onClick.AddListener(() => ShowFood(index));
        }

        ShowFood(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && Time.timeScale != 0)
            Toggle();
    }

    public void Toggle()
    {
        if (_isOpen) Close();
        else Open();
    }

    public void Open()
    {
        _isOpen = true;

        panel.gameObject.SetActive(true);
        overlayImage.gameObject.SetActive(true);

        ClearSelection();

        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(Animate(true));
    }

    public void Close()
    {
        _isOpen = false;

        ClearSelection();

        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(Animate(false));
    }

    private void ClearSelection()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator Animate(bool open)
    {
        float startX     = panel.anchoredPosition.x;
        float endX       = open ? _panelOnscreenX : _panelOffscreenX;
        float startAlpha = overlayImage.color.a;
        float endAlpha   = open ? overlayTargetAlpha : 0f;
        float elapsed    = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t      = Mathf.Clamp01(elapsed / animDuration);
            float curved = slideCurve.Evaluate(t);

            SetPanelX(Mathf.Lerp(startX, endX, curved));
            SetOverlayAlpha(Mathf.Lerp(startAlpha, endAlpha, curved));
            yield return null;
        }

        SetPanelX(endX);
        SetOverlayAlpha(endAlpha);

        if (!open)
        {
            panel.gameObject.SetActive(false);
            overlayImage.gameObject.SetActive(false);
        }
    }

    public void ShowFood(int index)
    {
        for (int i = 0; i < foodImages.Length; i++)
            foodImages[i].SetActive(i == index);
    }

    private void SetPanelX(float x)
    {
        Vector2 pos = panel.anchoredPosition;
        pos.x = x;
        panel.anchoredPosition = pos;
    }

    private void SetOverlayAlpha(float alpha)
    {
        Color c = overlayImage.color;
        c.a = alpha;
        overlayImage.color = c;
    }
}