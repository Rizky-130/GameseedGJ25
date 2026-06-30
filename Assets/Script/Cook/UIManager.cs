using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Racik Slots")]
    public TextMeshProUGUI[] racikSlotTexts;
    [Header("Makanan Siap Serah")]
    public RecipeData readyFood;

    [Header("Tombol Racik & Clear")]
    public GameObject btnRacik;
    public GameObject btnClear;

    void Awake()
    {
        Instance = this;

        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAndDestroy();
    }

    public void UpdateRacikUI(List<FoodType> selected)
    {
        for (int i = 0; i < racikSlotTexts.Length; i++)
        {
            if (i < selected.Count)
                racikSlotTexts[i].text = selected[i].ToString();
            else
                racikSlotTexts[i].text = "---";
        }
    }

    public void ShowRacikResult(RecipeData result)
    {
        if (result != null)
            Debug.Log("✅ Racik sukses: " + result.recipeName);
        else
            Debug.Log("❌ Racik gagal, bahan tidak cocok");
    }

    public void SetReadyFood(RecipeData food)
    {
        readyFood = food;
        Debug.Log("Makanan siap diserahkan: " + (food != null ? food.resultFood.ToString() : "tidak ada"));
    }

    public void ClearReadyFood()    // ← tambahkan ini
    {
        readyFood = null;
    }
}