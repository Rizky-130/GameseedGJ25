using System.Collections.Generic;
using UnityEngine;

public class RacikManager : MonoBehaviour
{
    public static RacikManager Instance;

    [Header("Data")]
    public RecipeData[] allRecipes;
    [Header("Slot bahan yang dipilih")]

    public List<FoodType> selectedIngredients  = new List<FoodType>();
    public int maxIngredients = 4;
    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    // dipanggil saat pemain tap bahan di Rak
    public void SelectIngredient(FoodType ingredient)
    {
        if (selectedIngredients.Contains(ingredient))
        {
            Debug.Log("Bahan sudah dipilih!");
            return;
        }

        if (selectedIngredients.Count >= maxIngredients)
        {
            Debug.Log("Slot bahan penuh!");
            return;
        }

        selectedIngredients.Add(ingredient);
        Debug.Log("Pilih: " + ingredient);

        UIManager.Instance.UpdateRacikUI(selectedIngredients);
    }

    // dipanggil saat tombol "Racik" ditekan
    public RecipeData TryRacik()
    {
        foreach (var recipe in allRecipes)
        {
            if (IsMatch(recipe))
            {
                Debug.Log("Berhasil racik: " + recipe.resultFood);
                selectedIngredients.Clear();
                UIManager.Instance.UpdateRacikUI(selectedIngredients);
                return recipe;
            }
        }

        Debug.Log("Bahan tidak cocok dengan resep apapun!");
        selectedIngredients.Clear();
        UIManager.Instance.UpdateRacikUI(selectedIngredients);
        return null;
    }

    // cek apakah bahan yang dipilih cocok dengan resep
    bool IsMatch(RecipeData recipe)
    {
        if (recipe.requiredIngredients.Length != selectedIngredients.Count)
            return false;

        foreach (var req in recipe.requiredIngredients)
        {
            if (!selectedIngredients.Contains(req))
                return false;
        }

        return true;
    }

    public void ClearSelection()
    {
        selectedIngredients.Clear();
        UIManager.Instance.UpdateRacikUI(selectedIngredients);
    }
}
