using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "CookingGame/Ingredient")]
public class IngridientData : ScriptableObject
{
   public string IngredientName;
   public FoodType IngredientType;
   public Sprite icon;
}
