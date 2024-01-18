using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int levelTime = 10; // In minutes

    public int recepiesDone = 0;

    public List<Pattern> parts;
    public List<Ingredient> ingredients;
    public List<Cooker> cookers;

    //ToDo : Here we should add the model of the user.          
    //ToDo : Here we should add the functions to trigger events.

    //Steering Law parameters
    public float a = 10; // Adaptation time
    public float b = 100; // Sensibility
    public float sphere_collider_radius = 0.025f; //Scale of the object * the radius
    public Recipe GenerateRecipe()
    {
        Recipe newRecipe = new Recipe();

        int nbrIngredients = Random.Range(0, 3);
        newRecipe.ingredients = new List<Ingredient>();
        for (int i = 0; i < nbrIngredients; i++)
        {
            newRecipe.ingredients.Add(GenerateIngredient());
        }

        int cooker_index = Random.Range(0, cookers.Count);
        newRecipe.cooker = cookers[cooker_index];

        return newRecipe;
    }

    public Ingredient GenerateIngredient()
    {
        Ingredient newIngredient = new Ingredient();

        newIngredient.parts = new List<Pattern>();
        int nbrParts = Random.Range(0, 3);
        for (int i = 0; i < nbrParts; i++)
        {
            int pattern_index = Random.Range(0, parts.Count);
            Pattern newPattern = parts[pattern_index];
            newIngredient.addPart(newPattern);
        }

        return newIngredient;
    }

    public int estimate_recipe_time(Recipe recipe)
    {
        int time = 0;
        return time;
    }

    public float estimate_cut_time(Ingredient ingredient)
    {
        float time = a; // Should we add A at each point ? :eyes:


        Transform[] patternPoints = ingredient.parts[0].GetComponentsInChildren<Transform>();
        
        for (int i = 0; i<patternPoints.Length - 1; i++)
        {
            float distance = Vector3.Distance(patternPoints[i].position, patternPoints[i + 1].position);

            time += b * distance / sphere_collider_radius; //Maybe add here a small portion of a;
        }

        return time;
    }
}
