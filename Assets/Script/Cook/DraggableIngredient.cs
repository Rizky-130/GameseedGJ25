using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableIngredient : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Data")]
    public IngridientData ingredientData;

    [Header("UI")]
    public Image backgroundImage;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private GameObject dragVisual;
    private RectTransform dragVisualRect;
    private Canvas parentCanvas;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // buat visual icon yang ngikutin drag
        dragVisual = new GameObject("DragVisual");
        dragVisual.transform.SetParent(parentCanvas.transform, false);
        dragVisual.transform.SetAsLastSibling();

        Image img = dragVisual.AddComponent<Image>();
        if (ingredientData != null && ingredientData.icon != null)
            img.sprite = ingredientData.icon;
        else
            img.color = Color.cyan; // placeholder

        RectTransform rt = dragVisual.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);

        dragVisualRect = rt;

        CanvasGroup cg = dragVisual.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false; // supaya drop target bisa kedeteksi
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragVisualRect == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(),
            eventData.position,
            parentCanvas.worldCamera,
            out Vector2 localPoint
        );
        dragVisualRect.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragVisual != null)
            Destroy(dragVisual);
    }
}