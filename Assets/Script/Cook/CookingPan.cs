using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingPan : MonoBehaviour
{
    [Header("Data Masakan")]
    public RecipeData currentRecipe;     
    public CookState state = CookState.Empty;

    [Header("Timer")]
    private float cookTimer;
    public float overcookTime = 5f;      

    [Header("UI")]
    public Image progressBar;            
    public TextMeshProUGUI stateText;    
    public Image panVisual;              

    [Header("Animasi Panci")]
    public Sprite emptyPanSprite;        // Panci diam (dipakai saat Kosong, Matang, dan Gosong)
    public Sprite[] cookingSprites;      // Array frame animasi hanya saat memasak
    public float frameInterval = 0.2f;   
    
    private float animTimer;             
    private int currentFrame;            

    [Header("Makanan Jadi")]
    public DraggableFood draggableFood;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateVisual();
    }

    void Update()
    {
        if (state == CookState.Cooking)
        {
            // 1. Logika Timer Masak
            cookTimer -= Time.deltaTime;
            progressBar.fillAmount = 1 - (cookTimer / currentRecipe.cookTime);

            // 2. Logika Animasi Memasak (Frame-by-frame bergerak)
            animTimer += Time.deltaTime;
            if (animTimer >= frameInterval)
            {
                animTimer = 0f;
                if (cookingSprites != null && cookingSprites.Length > 0)
                {
                    currentFrame = (currentFrame + 1) % cookingSprites.Length;
                    panVisual.sprite = cookingSprites[currentFrame];
                }
            }

            // 3. Cek Jika Matang
            if (cookTimer <= 0)
            {
                state = CookState.Ready;
                progressBar.fillAmount = 1;
                cookTimer = overcookTime;   
                
                // Panggil UpdateVisual agar animasi langsung berhenti & kembali ke panci kosong
                UpdateVisual(); 
            }
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
        
        // Reset frame animasi ke awal saat mulai masak
        currentFrame = 0;
        animTimer = 0f;
        
        UpdateVisual();
        return true;
    }

    public RecipeData TakeFood()
    {
        if (state == CookState.Ready || state == CookState.Overcooked)
        {
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
        panVisual.color = Color.white; 

        switch (state)
        {
            case CookState.Empty:
                panVisual.sprite = emptyPanSprite;
                stateText.text = "Kosong";
                break;
            case CookState.Cooking:
                // Sprite diatur oleh Update() saat animasi berjalan
                stateText.text = "Memasak...";
                break;
            case CookState.Ready:
                // panVisual.sprite = emptyPanSprite; // Kembali ke panci diam (animasi berhenti)
                stateText.text = "Siap! Tap untuk angkat";
                break;
            case CookState.Overcooked:
                panVisual.sprite = emptyPanSprite; // Tetap panci diam
                stateText.text = "Gosong!";
                break;
        }
    }

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