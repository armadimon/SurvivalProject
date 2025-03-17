using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecipeType
{
    Equip,
    Food,
    Process
}

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = ("Crafting/Recipe"))]
public class CraftingRecipe : ScriptableObject
{
    public ItemData[] requireResourcesItem;    //필요한 재료 리스트
    public ItemData resultItem;     // 완성될 아이템
    public int resultAmount = 1;        // 제작 시 생성되는 갯수
    public RecipeType recipeType;
}
