using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    [HideInInspector] public int slotIndex;
    [HideInInspector] public CustomerSpawner spawner;

    [Header("Pesanan")]
    public FoodType orderFood;

    [Header("Timer")]
    public float waitTime = 30f;
    private float currentTime;
    private bool isWaiting = true;

    [Header("UI Referensi")]
    public TextMeshProUGUI orderText;
    public Image timerBar;

    void Start()
    {
        currentTime = waitTime;
        orderText.text = orderFood.ToString();

        // hubungkan tombol tap pelanggan
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnCustomerTapped);
        }
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

    // dipanggil saat pelanggan di-tap pemain
    void OnCustomerTapped()
    {
        RecipeData food = UIManager.Instance.readyFood;

        if (food == null)
        {
            Debug.Log("Tidak ada makanan di tangan!");
            return;
        }

        if (food.resultFood == orderFood)
        {
            CustomerMangan(food.resultFood);
            UIManager.Instance.ClearReadyFood();   // makanan sudah dipakai
        }
        else
        {
            CustomerPergi();
            Debug.Log("Makanan tidak cocok dengan pesanan pelanggan ini!");
        }
    }

    public void CustomerMangan(FoodType food)
    {
        if (food == orderFood)
        {
            isWaiting = false;
            spawner.ClearSlot(slotIndex);
            GameManager.Instance.OnCustomerServed();
            Destroy(gameObject);
        }
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
        isWaiting = false;
        spawner.ClearSlot(slotIndex);
        GameManager.Instance.OnCustomerLeft();
        Destroy(gameObject);
    }
}