using System.Collections.Generic;
using UnityEngine;

public class RacikManager : MonoBehaviour
{
    public static RacikManager Instance;

    [Header("Data")]
    public RecipeData[] allRecipes;
    [Header("Wajan")]
    public CookingPan cookingPan;

    [Header("Slot bahan yang dipilih")]
    public List<FoodType> selectedIngredients = new List<FoodType>();
    public int maxIngredients = 4;

    [Header("Hasil Racikan Sekarang")]
    public RecipeData currentResult;

    void Awake()
    {
        Instance = this;
        selectedIngredients.Clear();
    }

    public void SelectIngredient(FoodType ingredient)
    {
        if (selectedIngredients.Contains(ingredient))
        {
            selectedIngredients.Remove(ingredient);
            Debug.Log("Batal pilih: " + ingredient);
        }
        else
        {
            if (selectedIngredients.Count >= maxIngredients)
            {
                Debug.Log("Slot bahan penuh!");
                return;
            }
            selectedIngredients.Add(ingredient);
            Debug.Log("Pilih: " + ingredient);
        }
        UIManager.Instance.UpdateRacikUI(selectedIngredients);
    }

    // ganti dari "public RecipeData TryRacik()" jadi void
    public void TryRacik()
    {
        foreach (var recipe in allRecipes)
        {
            if (IsMatch(recipe))
            {
                Debug.Log("Berhasil racik: " + recipe.resultFood);
                currentResult = recipe;
                selectedIngredients.Clear();
                UIManager.Instance.UpdateRacikUI(selectedIngredients);

                // langsung taruh ke wajan
                bool success = cookingPan.StartCooking(recipe);
                if (!success)
                {
                    Debug.Log("Tidak bisa masak, wajan masih penuh!");
                }

                return;
            }
        }

        Debug.Log("Bahan tidak cocok dengan resep apapun!");
        currentResult = null;
        selectedIngredients.Clear();
        UIManager.Instance.UpdateRacikUI(selectedIngredients);
        UIManager.Instance.ShowRacikResult(null);
    }

    public RecipeData CekResep(List<FoodType> bahan)
    {
        foreach (var recipe in allRecipes)
        {
            if (recipe.requiredIngredients.Length != bahan.Count) continue;

            bool cocok = true;
            foreach (var req in recipe.requiredIngredients)
            {
                if (!bahan.Contains(req))
                {
                    cocok = false;
                    break;
                }
            }

            if (cocok) return recipe;
        }
        return null;
    }
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
    public bool CekMasihMungkin(List<FoodType> bahanSekarang)
    {
        foreach (var recipe in allRecipes)
        {
            bool semuaAdaDiResep = true;

            foreach (var bahan in bahanSekarang)
            {
                bool adaDiResepIni = false;
                foreach (var req in recipe.requiredIngredients)
                {
                    if (req == bahan)
                    {
                        adaDiResepIni = true;
                        break;
                    }
                }

                if (!adaDiResepIni)
                {
                    semuaAdaDiResep = false;
                    break;
                }
            }

            if (semuaAdaDiResep) return true;
        }

        return false; // tidak ada resep yang mungkin cocok
    }

    public void ClearSelection()
    {
        selectedIngredients.Clear();
        UIManager.Instance.UpdateRacikUI(selectedIngredients);
    }
}