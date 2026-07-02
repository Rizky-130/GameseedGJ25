using UnityEngine;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Prefab & Spawn")]
    public GameObject customerPrefab;
    public AudioClip custCome;
    public AudioSource audioSource;
    public Transform[] customerSlots;
    [Header("Menu Restoran")]
    public RecipeData[] availableRecipes;
    [Header("Musik Background")]
    public AudioSource musicSource;
    public AudioClip musicPhase1;
    public AudioClip musicPhase2;
    public AudioClip musicPhase3;

    private int currentPhase = 0;
    [Header("Phase 1 - Awal (detik 0 - phase1Duration)")]
    public float phase1Duration = 60f;
    public int phase1MaxCustomers = 1;
    public float phase1SpawnInterval = 10f;
    public float phase1WaitTime = 35f;

    [Header("Phase 2 - Tengah (phase1Duration - phase2Duration)")]
    public float phase2Duration = 150f;
    public int phase2MaxCustomers = 2;
    public float phase2SpawnInterval = 7f;
    public float phase2WaitTime = 25f;

    [Header("Phase 3 - Susah (setelah phase2Duration)")]
    public int phase3MaxCustomers = 3;
    public float phase3SpawnIntervalMin = 4f;
    public float phase3SpawnIntervalStart = 6f;
    public float phase3SpawnScaling = 0.01f;
    public float phase3WaitTimeMin = 12f;
    public float phase3WaitTimeStart = 25f;
    public float phase3WaitScaling = 0.05f;

    [Header("Debug (jangan diubah)")]
    public float gameTime = 0f;
    public float spawnTime = 0f; // hanya jalan kalau ada slot kosong yang bisa diisi

    private Customer[] activeCustomers;

    void Start()
    {
        activeCustomers = new Customer[customerSlots.Length];
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        CheckMusicTransition();
        UpdateSpawnTimer();
    }

    // spawnTime cuma bertambah kalau ada slot kosong & belum capai max.
    // Begitu penuh/max, spawnTime dipaksa 0 dan berhenti hitung sampai ada slot kosong lagi.
    void UpdateSpawnTimer()
    {
        int emptySlot = GetEmptySlot();
        int currentMax = GetCurrentMaxCustomers();
        bool bisaSpawn = emptySlot >= 0 && CountActiveCustomers() < currentMax;

        if (!bisaSpawn)
        {
            spawnTime = 0f; // gak ada slot yang bisa diisi, timer diam di 0
            return;
        }

        spawnTime += Time.deltaTime;

        if (spawnTime >= GetCurrentSpawnInterval())
        {
            SpawnCustomer(emptySlot); // spawn 1 pelanggan di slot kosong yang ditemukan
            spawnTime = 0f;           // reset, siklus ulang dari 0
        }
    }

    public void ClearSlot(int slotIndex)
    {
        activeCustomers[slotIndex] = null;
        // tidak perlu apa-apa lagi di sini,
        // UpdateSpawnTimer() otomatis mendeteksi slot kosong ini di frame berikutnya
        // dan mulai menghitung spawnTime dari 0
    }

    int GetCurrentMaxCustomers()
    {
        if (gameTime < phase1Duration) return phase1MaxCustomers;
        if (gameTime < phase2Duration) return phase2MaxCustomers;
        return phase3MaxCustomers;
    }

    float GetCurrentSpawnInterval()
    {
        if (gameTime < phase1Duration) return phase1SpawnInterval;
        if (gameTime < phase2Duration) return phase2SpawnInterval;

        float t = gameTime - phase2Duration;
        return Mathf.Max(phase3SpawnIntervalMin, phase3SpawnIntervalStart - t * phase3SpawnScaling);
    }

    float GetCurrentWaitTime()
    {
        if (gameTime < phase1Duration) return phase1WaitTime;
        if (gameTime < phase2Duration) return phase2WaitTime;

        float t = gameTime - phase2Duration;
        return Mathf.Max(phase3WaitTimeMin, phase3WaitTimeStart - t * phase3WaitScaling);
    }

    void SpawnCustomer(int slotIndex)
    {
        GameObject obj = Instantiate(customerPrefab, customerSlots[slotIndex]);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Customer c = obj.GetComponent<Customer>();
        if (c == null)
        {
            Debug.LogError("Prefab tidak punya script Customer!");
            return;
        }

        if (availableRecipes != null && availableRecipes.Length > 0)
        {
            RecipeData pesanan = availableRecipes[Random.Range(0, availableRecipes.Length)];
            c.orderFood = pesanan.resultFood;

            if (c.orderIconImage != null)
            {
                c.orderIconImage.sprite = pesanan.foodIcon;
                c.orderIconImage.color = Color.white;
            }
        }
        else
        {
            Debug.LogWarning("Daftar resep di Spawner masih kosong!");
        }

        c.waitTime = GetCurrentWaitTime();
        c.slotIndex = slotIndex;
        c.spawner = this;
        audioSource.PlayOneShot(custCome);

        activeCustomers[slotIndex] = c;
    }

    int GetEmptySlot()
    {
        for (int i = 0; i < activeCustomers.Length; i++)
            if (activeCustomers[i] == null) return i;
        return -1;
    }

    int CountActiveCustomers()
    {
        int count = 0;
        foreach (var c in activeCustomers)
            if (c != null) count++;
        return count;
    }

    void CheckMusicTransition()
    {
        int nextPhase = 0;
        AudioClip nextClip = musicPhase1;

        if (gameTime < phase1Duration)
        {
            nextPhase = 1;
            nextClip = musicPhase1;
        }
        else if (gameTime < phase2Duration)
        {
            nextPhase = 2;
            nextClip = musicPhase2;
        }
        else
        {
            nextPhase = 3;
            nextClip = musicPhase3;
        }

        if (currentPhase != nextPhase)
        {
            currentPhase = nextPhase;
            musicSource.clip = nextClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
}