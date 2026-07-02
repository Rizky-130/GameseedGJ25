using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Customer : MonoBehaviour
{
    [HideInInspector] public int slotIndex;
    [HideInInspector] public CustomerSpawner spawner;

    [Header("Audio")]
    public AudioClip custServed;
    public AudioClip heartBreak;
    public AudioSource audioSource;

    [Header("Pesanan")]
    public FoodType orderFood;

    [Header("Timer")]
    public float waitTime = 30f;
    private float currentTime;
    private bool isWaiting = true;
    private bool sudahSelesai = false; // biar gak diproses dobel

    [Header("UI Referensi")]
    public Image orderIconImage;
    public Image timerBar;

    [Header("Visual Pelanggan")]
    public Image customerImage;
    public Sprite[] customerSprites;

    [Header("Animasi Datang & Pergi")]
    public float durasiAnimasi = 0.5f;
    public float jarakGeserSamping = 300f; // seberapa jauh dari samping saat datang
    public float jarakTurunSaatPergi = 150f; // seberapa jauh turun saat pergi

    private RectTransform rt;
    private CanvasGroup canvasGroup;
    private Vector2 posisiAsli; // posisi target saat sudah "berhenti"

    void Awake()
    {
        rt = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Start()
    {
        if (customerImage != null && customerSprites != null && customerSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, customerSprites.Length);
            customerImage.sprite = customerSprites[randomIndex];
            customerImage.color = Color.white;
        }

        currentTime = waitTime;

        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnCustomerTapped);
        }

        // simpan posisi asli (posisi target dari slot) sebelum digeser buat animasi datang
        posisiAsli = rt.anchoredPosition;

        StartCoroutine(AnimasiDatang());
    }

    void Update()
    {
        if (!isWaiting) return;

        currentTime -= Time.deltaTime;
        if (timerBar != null)
            timerBar.fillAmount = currentTime / waitTime;

        if (currentTime <= 0)
        {
            CustomerPergi();
        }
    }

    void OnCustomerTapped()
    {
        if (sudahSelesai) return;

        RecipeData food = UIManager.Instance.readyFood;
        if (food == null)
        {
            Debug.Log("Tidak ada makanan di tangan!");
            return;
        }

        if (food.resultFood == orderFood)
        {
            CustomerMangan(food.resultFood);
            UIManager.Instance.ClearReadyFood();
        }
        else
        {
            CustomerPergi();
            Debug.Log("Makanan tidak cocok dengan pesanan pelanggan ini!");
        }
    }

    public void CustomerMangan(FoodType food)
    {
        if (sudahSelesai) return;
        if (food != orderFood) return;

        sudahSelesai = true;
        isWaiting = false;

        spawner.ClearSlot(slotIndex); // slot langsung kosong biar spawner bisa isi lagi
        GameManager.Instance.OnCustomerServed();

        StartCoroutine(ProsesKeluar(custServed));
    }

    public bool TryServe(RecipeData food)
    {
        if (food == null) return false;

        if (food.resultFood == orderFood)
        {
            CustomerMangan(food.resultFood);
            return true;
        }
        else
        {
            Debug.Log("Makanan tidak cocok dengan pesanan pelanggan ini!");
            return false;
        }
    }

    public void CustomerPergi()
    {
        if (sudahSelesai) return;

        sudahSelesai = true;
        isWaiting = false;

        spawner.ClearSlot(slotIndex);
        GameManager.Instance.OnCustomerLeft();

        StartCoroutine(ProsesKeluar(heartBreak));
    }

    // geser dari samping (kiri/kanan acak) menuju posisi asli, dengan easing
    private IEnumerator AnimasiDatang()
    {
        float arah = Random.value < 0.5f ? -1f : 1f; // -1 = dari kiri, 1 = dari kanan
        Vector2 posisiAwal = posisiAsli + new Vector2(jarakGeserSamping * arah, 0f);

        rt.anchoredPosition = posisiAwal;
        canvasGroup.alpha = 0f;

        float waktuBerjalan = 0f;
        while (waktuBerjalan < durasiAnimasi)
        {
            waktuBerjalan += Time.deltaTime;
            float t = Mathf.Clamp01(waktuBerjalan / durasiAnimasi);
            float eased = EaseOutCubic(t); // biar berhenti dengan halus, gak tiba-tiba stop

            rt.anchoredPosition = Vector2.Lerp(posisiAwal, posisiAsli, eased);
            canvasGroup.alpha = eased;

            yield return null;
        }

        rt.anchoredPosition = posisiAsli;
        canvasGroup.alpha = 1f;
    }

    // mainkan audio + animasi keluar, baru destroy setelah keduanya selesai
    private IEnumerator ProsesKeluar(AudioClip clip)
    {
        Button btn = GetComponent<Button>();
        if (btn != null) btn.interactable = false;

        float audioLength = 0f;
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
            audioLength = clip.length;
        }

        StartCoroutine(AnimasiPergi());

        float delay = Mathf.Max(audioLength, durasiAnimasi);
        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

    // bergerak turun sambil fade out
    // Geser ke samping (kiri atau kanan) sambil menghilang
    private IEnumerator AnimasiPergi()
    {
        Vector2 posisiAwal = rt.anchoredPosition;
        // Menggunakan arah yang sama dengan AnimasiDatang (menggeser ke luar)
        // Kita bisa ambil nilai arah dari AnimasiDatang atau set random lagi
        float arah = Random.value < 0.5f ? -1f : 1f; 
        Vector2 posisiAkhir = posisiAwal + new Vector2(jarakGeserSamping * arah, 0f);
        
        float alphaAwal = canvasGroup.alpha;

        float waktuBerjalan = 0f;
        while (waktuBerjalan < durasiAnimasi)
        {
            waktuBerjalan += Time.deltaTime;
            float t = Mathf.Clamp01(waktuBerjalan / durasiAnimasi);
            float eased = EaseInCubic(t); 

            rt.anchoredPosition = Vector2.Lerp(posisiAwal, posisiAkhir, eased);
            canvasGroup.alpha = Mathf.Lerp(alphaAwal, 0f, t);

            yield return null;
        }

        rt.anchoredPosition = posisiAkhir;
        canvasGroup.alpha = 0f;
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    private float EaseInCubic(float t)
    {
        return t * t * t;
    }
}