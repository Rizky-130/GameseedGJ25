using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Stats")]
    public int score = 0;
    public int customerServed = 0;
    public int nyawa = 5;
    [Header("Efek")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 10f;

    [Header("UI Visual")]
    public List<Image> healthHearts;    // Drag 5 Image hati ke sini di Inspector
    public Sprite fullHeart;            // Sprite hati penuh
    public Sprite emptyHeart;           // Sprite hati kosong

    [Header("UI Text")]
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textServed;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void OnCustomerServed()
    {
        score += 100;
        customerServed++;
        UpdateUI();
    }

    public void OnCustomerLeft()
    {
        if (nyawa <= 0) return; // Mencegah error jika nyawa sudah habis

        int targetIndex = nyawa - 1; // Hati yang akan pecah
        nyawa--;

        // Jalankan Coroutine
        StartCoroutine(ShakeAndBreak(healthHearts[targetIndex]));
        
        // Update score dsb, tapi jangan panggil UpdateUI dulu sampai getar selesai
        UpdateUI(); 

        if (nyawa <= 0)
        {
            GameOver();
        }
    }

    IEnumerator ShakeAndBreak(Image heartImage)
    {
        RectTransform rt = heartImage.GetComponent<RectTransform>();
        Vector2 originPos = rt.anchoredPosition;
        float elapsed = 0;

        // ANIMASI GETAR
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            
            // TAMBAHKAN originPos + new Vector2(x, y) agar posisinya bergeser
            rt.anchoredPosition = originPos + new Vector2(x, y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Kembalikan ke posisi awal setelah getar selesai
        rt.anchoredPosition = originPos;
    }
    void UpdateUI()
    {
        textScore.text = "Score : " + score;
        textServed.text = customerServed.ToString();

        // Update visual hati
        for (int i = 0; i < healthHearts.Count; i++)
        {
            if (i < nyawa)
            {
                healthHearts[i].sprite = fullHeart; // Hati aktif
            }
            else
            {
                healthHearts[i].sprite = emptyHeart; // Hati yang sudah hilang
            }
        }
    }

    void GameOver()
    {
        if (GameOverManager.Instance != null)
            GameOverManager.Instance.ShowGameOver();
    }
}