using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MangkokRacik : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Bahan yang Sudah Masuk")]
    public List<FoodType> bahanMasuk = new List<FoodType>();
    public AudioSource dropAudioSource; // Drag ke sini audio source-nya
    public AudioClip dropSuccessClip;
    public AudioClip trashau;
    [Header("UI")]
    public Image mangkokImage;
    public Color emptyColor = Color.white;
    public Color filledColor = Color.white;
    public Color cookingColor = Color.white;
    public Color readyColor = Color.white;
    [Header("Peringatan")]
    public TextMeshProUGUI warningText;   // drag Text peringatan di Inspector
    public float warningDuration = 2f;

    [Header("Tombol Racik")]
    public GameObject btnRacik;

    [Header("Animasi Racik")]
    public Sprite[] racikSprites;
    public float racikDuration = 3f;
    public float frameInterval = 0.2f;

    [Header("State")]
    public bool isCooking = false;
    public bool isReady = false;
    private Sprite defaultMangkokSprite;

    private Canvas parentCanvas;
    private Vector2 originalPosition;
    private CanvasGroup canvasGroup;
    private bool isDragging = false;
    private Vector2 offset;
    private RectTransform rectTransform;

    void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        if (mangkokImage != null)
        {
            defaultMangkokSprite = mangkokImage.sprite;
        }
    }

    void Start()
    {
        if (btnRacik != null)
            btnRacik.SetActive(false);
    }

    // ========================
    // DROP BAHAN KE MANGKOK
    // ========================
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("DROP");

        Debug.Log(eventData.pointerDrag);

        if (eventData.pointerDrag != null)
            Debug.Log(eventData.pointerDrag.name);
        

        DraggableIngredient ingredient = eventData.pointerDrag?.GetComponent<DraggableIngredient>();
       

        FoodType tipe = ingredient.ingredientData.IngredientType;

        if (bahanMasuk.Contains(tipe))
        {
            Debug.Log("⚠️ Bahan sudah ada di mangkok!");
            return;
        }

        bahanMasuk.Add(tipe);
        Debug.Log("✅ Bahan berhasil masuk: " + ingredient.ingredientData.IngredientName + " (" + bahanMasuk.Count + " bahan)");

        if (dropAudioSource != null && dropSuccessClip != null)
        {
            dropAudioSource.PlayOneShot(dropSuccessClip);
        }
        UpdateVisual();
        CekTampilTombolRacik();
    }

    void CekTampilTombolRacik()
    {
        if (btnRacik == null) return;

        RecipeData cocok = RacikManager.Instance.CekResep(bahanMasuk);

        if (cocok != null)
        {
            // bahan cocok resep → tampilkan tombol racik
            btnRacik.SetActive(true);
            return;
        }

        // cek apakah masih mungkin cocok (bahan belum penuh)
        bool masihMungkin = RacikManager.Instance.CekMasihMungkin(bahanMasuk);
        if (!masihMungkin)
        {
            // tidak ada resep yang bisa cocok → peringatan & reset
            StartCoroutine(WarningAndReset());
        }
    }

    IEnumerator WarningAndReset()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            warningText.text = "Wrong Recipe!";
        }

        yield return new WaitForSeconds(warningDuration);

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        ResetMangkok();
    }
    // ========================
    // TOMBOL RACIK DITEKAN
    // ========================
    public void MulaiRacik()
    {
        if (isCooking || isReady) return;

        RecipeData cocok = RacikManager.Instance.CekResep(bahanMasuk);
        if (cocok == null)
        {
            Debug.Log("Bahan tidak cocok resep apapun!");
            ResetMangkok();
            return;
        }

        btnRacik.SetActive(false);
        isCooking = true;
        StartCoroutine(AnimasiRacik(cocok));
    }

    IEnumerator AnimasiRacik(RecipeData resep)
    {
        mangkokImage.color = Color.white;
        float elapsed = 0f;

        while (elapsed < racikDuration)
        {
            if (racikSprites != null && racikSprites.Length > 0)
            {
                int frameIndex = Mathf.FloorToInt((elapsed / frameInterval) % racikSprites.Length);
                mangkokImage.sprite = racikSprites[frameIndex];
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        isCooking = false;
        isReady = true;
        RacikManager.Instance.currentResult = resep;
        if (resep.mangkokIsiSprite != null)
        {
            mangkokImage.sprite = resep.mangkokIsiSprite; // Gambar berubah jadi mangkok isi gurita/daging/udang
        }
        Debug.Log("Racik selesai! Drag mangkok ke panci.");
    }

    // ========================
    // DRAG MANGKOK KE PANCI
    // ========================
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isReady) return;

        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // delta dibagi scaleFactor supaya sesuai dengan Canvas Scaler
        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        canvasGroup.blocksRaycasts = true;



        GameObject target = eventData.pointerCurrentRaycast.gameObject;

        // cek drop ke panci
        CookingPan panci = target?.GetComponentInParent<CookingPan>();
        if (panci != null && isReady)
        {
            bool berhasil = panci.StartCooking(RacikManager.Instance.currentResult);
            if (berhasil)
            {
                Debug.Log("Mangkok berhasil masuk panci!");
                ResetMangkok();
                return;
            }
            else
            {
                Debug.Log("Panci masih dipakai!");
            }
        }

        // cek drop ke trash
        Trash trash = target?.GetComponentInParent<Trash>();
        if (trash != null)
        {
            dropAudioSource.PlayOneShot(trashau);
            Debug.Log("Mangkok dibuang ke trash.");
            ResetMangkok();
            return;
        }

        // gagal → balik posisi semula
        rectTransform.anchoredPosition = originalPosition;
    }

    // ========================
    // HELPER
    // ========================
    // Di dalam class MangkokRacik.cs

    void UpdateVisual()
    {
        if (mangkokImage == null) return;

        // 1. Cek resep apa yang sedang dibuat
        RecipeData resepCocok = RacikManager.Instance.CekResep(bahanMasuk);

        if (resepCocok != null)
        {
            // Langsung ambil sprite dari ScriptableObject!
            mangkokImage.sprite = resepCocok.mangkokBahanMentahSprite;
            mangkokImage.color = Color.white;
        }
        else
        {
            // Gunakan tampilan standar jika belum jadi resep
            mangkokImage.sprite = defaultMangkokSprite;
            mangkokImage.color = bahanMasuk.Count > 0 ? filledColor : emptyColor;
        }
    }

    public void ResetMangkok()
    {
        bahanMasuk.Clear();
        isCooking = false;
        isReady = false;
        if (mangkokImage != null)
        {
            mangkokImage.color = emptyColor;
            mangkokImage.sprite = defaultMangkokSprite;
        }
        GetComponent<RectTransform>().anchoredPosition = originalPosition;
        if (btnRacik != null)
            btnRacik.SetActive(false);
    }
}