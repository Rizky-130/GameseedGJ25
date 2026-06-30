using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "CookingGame/Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public FoodType resultFood;
    public FoodType[] requiredIngredients;
    public float cookTime = 5f;
    public Sprite foodIcon;

    [Header("Visual Mangkok")]
    public Sprite mangkokIsiSprite;           // Sprite HASIL JADI (setelah diracik/dimasak)
    public Sprite mangkokBahanMentahSprite;   // Sprite SAAT BAHAN SUDAH MASUK (sebelum diracik)
}