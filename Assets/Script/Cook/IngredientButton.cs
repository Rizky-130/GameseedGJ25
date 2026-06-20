using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientButton : MonoBehaviour
{
    [Header("Data")]
    public IngridientData ingredientData;

    [Header("UI")]
    public TextMeshProUGUI nameText;
    public Image iconImage;
    public Image backgroundImage;     // ← tambah ini untuk warna highlight

    [Header("Warna")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    void Start()
    {
        if (ingredientData != null)
        {
            nameText.text = ingredientData.IngredientName;
            if (ingredientData.icon != null)
                iconImage.sprite = ingredientData.icon;
        }

        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();   // ← tambah ini, bersihkan listener lama
        btn.onClick.AddListener(() =>
        {
            RacikManager.Instance.SelectIngredient(ingredientData.IngredientType);
            UpdateVisual();
        });
    }

    void UpdateVisual()
    {
        bool isSelected = RacikManager.Instance.selectedIngredients.Contains(ingredientData.IngredientType);
        backgroundImage.color = isSelected ? selectedColor : normalColor;
    }
}