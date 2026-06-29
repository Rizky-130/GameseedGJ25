using UnityEngine;
using UnityEngine.UI;

public class CookingPan : MonoBehaviour
{
    [Header("Data Masakan")]
    public RecipeData currentRecipe;     
    public CookState state = CookState.Empty;

    [Header("Audio")]
    public AudioSource masakAudio;
    
    [Header("Timer")]
    private float cookTimer;
    public float overcookTime = 5f;      

    [Header("UI")]
    public Image progressBar;            
    public Image panVisual;              

    [Header("Animasi Panci")]
    public Sprite emptyPanSprite;        // Panci diam (dipakai saat Kosong dan Gosong)
    public Sprite[] cookingSprites;      // Array frame animasi
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
        // 1. Logika Animasi (Berjalan terus selama Memasak ATAU Matang)
        if (state == CookState.Cooking || state == CookState.Ready)
        {
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
        }

        // 2. Logika Timer berdasarkan State
        if (state == CookState.Cooking)
        {
            cookTimer -= Time.deltaTime;
            progressBar.fillAmount = 1 - (cookTimer / currentRecipe.cookTime);

            masakAudio.Play();

            // Cek Jika Matang
            if (cookTimer <= 0)
            {
                state = CookState.Ready;
                progressBar.fillAmount = 1;
                cookTimer = overcookTime;   
                UpdateVisual(); 
            }
        }
        else if (state == CookState.Ready)
        {
            cookTimer -= Time.deltaTime;
            
            // Cek Jika Gosong
            if (cookTimer <= 0)
            {   
                masakAudio.Stop();
                state = CookState.Overcooked;
                UpdateVisual(); // Ini akan menghentikan animasi & ganti ke panci kosong
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
                break;
            case CookState.Cooking:
                // Animasi di-handle oleh Update()
                break;
            case CookState.Ready:
                // Animasi masih terus dilanjutkan oleh Update()
                break;
            case CookState.Overcooked:
                panVisual.sprite = emptyPanSprite; // Kembali ke panci diam
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