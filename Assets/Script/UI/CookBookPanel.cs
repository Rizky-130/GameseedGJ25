using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CookBookPanel : MonoBehaviour
{
    public RectTransform panel;
    public Button openButton;
    public Button exitButton;
    public GameObject[] foodImages;
    public Button[] foodButtons;

    public float animDuration = 0.4f;
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip toggleSound;

    private float _panelOnscreenX;
    private float _panelOffscreenX;
    private bool _isOpen;
    private Coroutine _activeCoroutine;

    void Awake()
    {
        _panelOnscreenX  = panel.anchoredPosition.x;
        _panelOffscreenX = _panelOnscreenX + panel.rect.width;

        SetPanelX(_panelOffscreenX);
        panel.gameObject.SetActive(false);

        _isOpen = false;

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

    void PlayToggleSound()
    {
        if (audioSource != null && toggleSound != null)
            audioSource.PlayOneShot(toggleSound);
    }

    public void Open()
    {
        if (_isOpen) return;
        _isOpen = true;

        PlayToggleSound();

        panel.gameObject.SetActive(true);

        ClearSelection();

        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(Animate(true));
    }

    public void Close()
    {
        if (!_isOpen) return;
        _isOpen = false;

        PlayToggleSound();

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
        float startX  = panel.anchoredPosition.x;
        float endX    = open ? _panelOnscreenX : _panelOffscreenX;
        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t      = Mathf.Clamp01(elapsed / animDuration);
            float curved = slideCurve.Evaluate(t);

            SetPanelX(Mathf.Lerp(startX, endX, curved));
            yield return null;
        }

        SetPanelX(endX);

        if (!open)
        {
            panel.gameObject.SetActive(false);
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
}