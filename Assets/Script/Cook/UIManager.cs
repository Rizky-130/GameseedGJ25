using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Racik UI")]
    public TextMeshProUGUI[] racikSlotTexts; // 4 slot teks bahan dipilih

    void Awake()
    {
        Instance = this;
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
}