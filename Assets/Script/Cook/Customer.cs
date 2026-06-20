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
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWaiting) return;

        currentTime -= Time.deltaTime;

        //update timer visual, 1 = penuh, 0 = habis
        if (timerBar != null)
            timerBar.fillAmount = currentTime / waitTime;
        if (currentTime <= 0)
        {
            CustomerPergi();
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
    public void CustomerPergi()
    {
        isWaiting = false;
        spawner.ClearSlot(slotIndex); 
        GameManager.Instance.OnCustomerLeft();
        Destroy(gameObject);
    }
}
