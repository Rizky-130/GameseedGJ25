using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DraggableFood : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Data")]
    public RecipeData currentFood;
    [Header("audio")]
    public AudioClip trashau;
    public AudioSource audioSource;
    [Header("State")]
    public bool isBurnt = false;

    [Header("UI")]
    public Image foodImage;

    private Canvas parentCanvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private bool isDragging = false;

    void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // dipanggil dari CookingPan saat makanan diangkat
    public void SetFood(RecipeData food, RectTransform panRect)
    {
        gameObject.SetActive(true);

        currentFood = food;
        isBurnt = false;

        

        if (food.foodIcon != null && foodImage != null)
            foodImage.sprite = food.foodIcon;
            foodImage.color = Color.white;

        // pakai posisi pan langsung
        rectTransform.position = panRect.position;
        originalPosition = rectTransform.anchoredPosition;
    }
    public void SetFoodBurnt(RecipeData food, RectTransform panRect)
    {
        SetFood(food, panRect);
        isBurnt = true;
        if (foodImage != null)
            foodImage.color = Color.black;  // visual gosong
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        GameObject target = eventData.pointerCurrentRaycast.gameObject;

        // cek drop ke pelanggan
        Customer customer = target?.GetComponentInParent<Customer>();
        if (customer != null)
        {
            if (isBurnt)
            {
                // makanan gosong diserahkan ke pelanggan → nyawa berkurang
                customer.CustomerPergi();

                Debug.Log("Makanan gosong diserahkan! Nyawa berkurang.");
                gameObject.SetActive(false);
                return;
            }

            bool berhasil = customer.TryServe(currentFood);
            if (berhasil)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                customer.CustomerPergi();
                Debug.Log("Makanan salah! Nyawa berkurang.");
                gameObject.SetActive(false);
                return;
            }
        }

        Trash trash = target?.GetComponentInParent<Trash>();
        if (trash != null)
        {
            audioSource.PlayOneShot(trashau);
            if (isBurnt)
            {
                Debug.Log("Makanan gosong dibuang!");
            }
            else
            {
                Debug.Log("Makanan dibuang ke trash.");
            }
            gameObject.SetActive(false);
            return;
        }
    }
}