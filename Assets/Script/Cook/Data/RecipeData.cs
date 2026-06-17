using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "CookingGame/Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public FoodType resultFood;          // makanan yang dihasilkan
    public FoodType[] requiredIngredients; // bahan     yang dibutuhkan
    public float cookTime = 5f;          // berapa lama memasaknya
    public Sprite foodIcon;
}