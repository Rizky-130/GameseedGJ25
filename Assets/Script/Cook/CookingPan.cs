using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingPan : MonoBehaviour
{
    [Header("Data Masakan")]
    public RecipeData currentRecipe;     // resep yang sedang dimasak
    public CookState state = CookState.Empty;

    [Header("Timer")]
    private float cookTimer;
    public float overcookTime = 5f;      // berapa lama bisa gosong setelah matang

    [Header("UI")]
    public Image progressBar;            // bar visual proses masak
    public TextMeshProUGUI stateText;    // teks status (Kosong/Memasak/Siap/Gosong)
    public Image panVisual;              // warna wajan berubah sesuai state

    [Header("Warna State")]
    public Color emptyColor = Color.gray;
    public Color cookingColor = Color.yellow;
    public Color readyColor = Color.green;
    public Color overcookColor = Color.red;
    [Header("Makanan Jadi")]
    public DraggableFood draggableFood;

    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    void Update()
    {
        if (state == CookState.Cooking)
        {
            cookTimer -= Time.deltaTime;
            progressBar.fillAmount = 1 - (cookTimer / currentRecipe.cookTime);

            if (cookTimer <= 0)
            {
                state = CookState.Ready;
                progressBar.fillAmount = 1;
                cookTimer = overcookTime;   // mulai hitung mundur overcook
            }
            UpdateVisual();
        }
        else if (state == CookState.Ready)
        {
            cookTimer -= Time.deltaTime;
            if (cookTimer <= 0)
            {
                state = CookState.Overcooked;
                UpdateVisual();
            }
        }
    }

    // dipanggil saat pemain taruh hasil racik ke wajan
    public bool StartCooking(RecipeData recipe)
    {
        if (state != CookState.Empty)
        {
            Debug.Log("Wajan masih dipakai!");
            return false;
        }

        currentRecipe = recipe;
        cookTimer = recipe.cookTime;
        state = CookState.Cooking;
        UpdateVisual();
        return true;
    }

    // dipanggil saat pemain tap wajan untuk ambil makanan
    public RecipeData TakeFood()
    {
        if (state == CookState.Ready)
        {
            RecipeData result = currentRecipe;
            ResetPan();
            return result;
        }
        else if (state == CookState.Overcooked)
        {
            Debug.Log("Makanan gosong, tidak bisa diserahkan!");
            RecipeData result = currentRecipe;
            ResetPan();
            return result;
        }
        else
        {
            Debug.Log("Makanan belum siap!");
            return null;
        }
    }

    void ResetPan()
    {
        state = CookState.Empty;
        currentRecipe = null;
        progressBar.fillAmount = 0;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        switch (state)
        {
            case CookState.Empty:
                panVisual.color = emptyColor;
                stateText.text = "Kosong";
                break;
            case CookState.Cooking:
                panVisual.color = cookingColor;
                stateText.text = "Memasak...";
                break;
            case CookState.Ready:
                panVisual.color = readyColor;
                stateText.text = "Siap! Tap untuk angkat";
                break;
            case CookState.Overcooked:
                panVisual.color = overcookColor;
                stateText.text = "Gosong!";
                break;
        }
    }

    // dipanggil saat wajan di-tap (lewat Button component)
    public void OnPanTapped()
    {
        if (state == CookState.Ready)
        {
            RecipeData food = TakeFood();
            if (food != null)
            {
                Debug.Log("Makanan diangkat: " + food.resultFood);
                draggableFood.SetFood(food, rectTransform);
            }
        }
        else if (state == CookState.Overcooked)
        {
            RecipeData food = TakeFood();
            if (food != null)
            {
                Debug.Log("Makanan gosong diangkat.");
                draggableFood.SetFoodBurnt(food, rectTransform);
            }
        }
    }
}