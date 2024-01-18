using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : MonoBehaviour
{
    public List<Ingredient> ingredients;
    public Cooker cooker;

    private void OnRecipeComplete()
    {
        Debug.Log("Recipe finished");
    }

}
