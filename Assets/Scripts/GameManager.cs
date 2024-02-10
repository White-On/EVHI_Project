using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // A static reference to the GameManager instance
    public UIManager uim; 

    public int levelTime = 5 * 60 * 60; // In seconds
    public int recipesNbr = 5; // How many recepies we want to acheive

    public List<GameObject> ingredients;
    private Dictionary<string, int> ingredientsId = new Dictionary<string, int>();

    public List<Cooker> cookers;
    private Dictionary<string, int> cookersId = new Dictionary<string, int>();

    public List<string> patterns;
    private Dictionary<string, int> patternsId = new Dictionary<string, int>();

    public float patternRecognitionThreshold = 0.9f;

    //Prediction 

    private float recipePredictionTimeThreshold = 10; // Fenetre pour considerer la prediction du temps d une rectte correcte en seconde
    private float ingredientPredictionTimeThreshold = 5;
    private float cookerPredictionTimeThreshold = 3;
    private float patternPredictionTimeThreshold = 2;

    public float estimatedTime;
    public float estimatedIngredientTime;
    public float estimatedCookerTime;
    public float estimatedPatternTime; 

    private float decayFactorIngredient = -0.5f;
    private float decayFactorCooker = -0.5f;
    private float decayFactorPattern = -0.5f;

    private float stepTransitionFactor = 10;

    private List<List<float>> patternTimes = new List<List<float>>();
    private List<List<float>> cookerTimes = new List<List<float>>();
    private List<List<float>> ingredientTimes = new List<List<float>>();

    private int[] patternCounts;
    private float[] patternAvg;

    private int[] cookerCounts;
    private float[] cookerAvg;

    private int[] ingredientCounts;
    private float[] ingredientAvg;



    //Game loop
    private float timeRemaining;

    public List<Tuple<string, string, string, string>> steps = new List<Tuple<string, string, string, string>>(); //Ingredient, cooker, pattern
    public List<GameObject> outputs = new List<GameObject>();
    public int currentStep = 0;

    
    public float recipeStartTime;
    public float ingredientFoundTime;
    public float cookerFoundTime;
    public float patternFoundTime; 

    public float predictedRecipeDuration;

    public int recipesDone = 0;

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }

        timeRemaining = levelTime;

        recipeStartTime = Time.time;
        predictedRecipeDuration = 120;
        Tuple<string, string, string, string> t = new Tuple<string, string, string, string>("crystal", "PurpleCircle", "o", "steak");
        //Tuple<string, string, string, string> tt = new Tuple<string, string, string, string>("blackwidow", "BlueCircle", "inf", "steak");
        steps.Add(t);
        //steps.Add(tt);
        currentStep = 0;

        // Dictionaries & counts setup 
        
        for (int i = 0; i < ingredients.Count; i++)
        {
            ingredientsId.Add(ingredients[i].name, i);
            ingredientTimes.Add(new List<float>());
        }
        ingredientCounts = new int[ingredients.Count];
        ingredientAvg = new float[ingredients.Count];

        for (int i = 0; i < patterns.Count; i++)
        {
            patternsId.Add(patterns[i], i);
            patternTimes.Add(new List<float>());
        }
        patternCounts = new int[patterns.Count];
        patternAvg = new float[patterns.Count];
        
        for (int i = 0; i < cookers.Count; i++)
        {
            cookersId.Add(cookers[i].name, i);
            cookerTimes.Add(new List<float>());
        }
        cookerCounts = new int[cookers.Count];
        cookerAvg = new float[cookers.Count];
    }

    private void Start()
    {
        uim.UpdateUI();
    }

    // Main Game Loop Functions 
    public void OnRecipeFinishEvent()
    {
        float duration = Time.time - recipeStartTime;
        Debug.Log("FINISHED. This is the tame u took duration : " + duration.ToString());
        recipesDone++;

        UpdateParams(); 

        if (recipesDone == recipesNbr)
        {
            Debug.Log("game finished u ve done all the recipes"); 
        }
        //else if (timeRemaining < 0)
        //{
        //Debug.Log("TIMES OUT");
        //}
        else
        {
            steps.Clear();
            steps = GenerateRecipe();
            currentStep = 0;
            uim.UpdateUI(); 
        }
    }
    
    public void OnIngredientEvent(SelectEnterEventArgs interactionEvent)
    {
        if (interactionEvent.interactableObject.transform.name == steps[currentStep].Item1)
        {
            if (ingredientFoundTime == 0)
            {
                ingredientFoundTime = Time.time;
                updateIngredientTime(steps[currentStep].Item1, recipeStartTime);
            }
        }
        else
        {
            //bad feedback if we want
        }
    }

    public void OnCookerEvent()
    {
        // This is good cooker for sure the verification here is made in the cooker class (it seemed easier to do so)
        if (cookerFoundTime == 0)
        {
            cookerFoundTime = Time.time;
            updateCookerTime(steps[currentStep].Item2, ingredientFoundTime);
        }
    }

    public void OnPatternEvent(string pattern)
    {
        
        if (cookers[cookersId[steps[currentStep].Item2]].playerInRange & cookers[cookersId[steps[currentStep].Item2]].ingredientInRange)
        {
            if (pattern == steps[currentStep].Item3)
            {
                patternFoundTime = Time.time;
                updatePatternTime(pattern, cookerFoundTime);
                Vector3 positionSpawn = cookers[cookersId[steps[currentStep].Item2]].ingredientInside.position;
                Destroy(cookers[cookersId[steps[currentStep].Item2]].ingredientInside.gameObject);

                cookers[cookersId[steps[currentStep].Item2]].ingredientInRange = false;
                cookers[cookersId[steps[currentStep].Item2]].ingredientInside = null;


                Instantiate(ingredients[ingredientsId[steps[currentStep].Item4]], position: positionSpawn, Quaternion.identity);
                
                

                return;
            }
            else
            {
                //JAAAAAAAAAAAAAAAAj
            }
            
        }
        
        // This part of the code is executed if the player is not in range of any cooker OR ingredient wrong in the cooker OR Wrong pattern
    }

    public void AddedToCauldron()
    {
        currentStep++;
        uim.UpdateUIStyle(); 
        if (currentStep == steps.Count)
        {
            OnRecipeFinishEvent(); 
        }
    }

    // Generation de recettes

    public void UpdateParams()
    {
        float realDuration = Time.time - recipeStartTime;
        if (estimatedTime > realDuration + recipePredictionTimeThreshold) stepTransitionFactor += 0.5f;
        else if (estimatedTime < realDuration - recipePredictionTimeThreshold) stepTransitionFactor -= 0.5f;

        float ingredientDuration = ingredientFoundTime - recipeStartTime;
        if (estimatedIngredientTime > ingredientDuration + ingredientPredictionTimeThreshold) decayFactorIngredient += 0.03f;
        else if (estimatedIngredientTime < ingredientDuration - ingredientPredictionTimeThreshold) decayFactorIngredient -= 0.03f;

        float cookerDuration = cookerFoundTime - ingredientFoundTime;
        if (estimatedCookerTime > cookerDuration + cookerPredictionTimeThreshold) decayFactorCooker += 0.04f;
        else if (estimatedCookerTime < cookerDuration - cookerPredictionTimeThreshold) decayFactorCooker -= 0.02f;

        float patternDuration = patternFoundTime - cookerFoundTime;
        if (estimatedPatternTime > patternDuration + patternPredictionTimeThreshold) decayFactorCooker += 0.04f;
        else if (estimatedPatternTime < patternDuration - patternPredictionTimeThreshold) decayFactorCooker -= 0.02f;
    }

    public List<Tuple<string, string, string, string>> GenerateRecipe()
    {
        float recipeTime = 120;
        recipeStartTime = Time.time;

        estimatedTime = 0;
        estimatedIngredientTime = 0;
        estimatedCookerTime = 0;
        estimatedPatternTime = 0;

        List<Tuple<string, string, string, string>> newSteps = new List<Tuple<string, string, string, string>>();

        while (recipeTime - estimatedTime > 0)
        {
            string newIngredient = ingredients[Random.Range(0, 5)].name;
            string newCooker = cookers[Random.Range(0, 3)].name;
            string newPattern = patterns[Random.Range(0, 5)];
            string newOut = ingredients[Random.Range(5, 10)].name;

            Tuple<string, string, string, string> newStep = new Tuple<string, string, string, string>(newIngredient, newCooker, newPattern, newOut);
            newSteps.Add(newStep);
            estimatedTime += estimateStepTime(newStep) + stepTransitionFactor;
            
            //Debug.Log("ESTIMATION OF NEW RECIPE : " + estimatedTime.ToString());
            //return newSteps; 
        }

        Debug.Log("ESTIMATION OF NEW RECIPE : " + estimatedTime.ToString());

        return newSteps; 
    }

    /*
     Modeles d estimation : 
        Estimation recette = somme des estimation des etapes + facteur de transition * nombre d'etape. 
        Estimation etape = Estimation ingredient + estimation cooker + estimation pattern 
        Estimation ingredient = estimation recherche ingredient + distance ingredient * temps pour tp
        Estimation cooker = estimation recherche cooker + estimation distance cooker + estimation depot d ingredient 
        estimation pattern = steering law (ou temps moyen de trajet) + law of decay  

       la recherche d'ingredient : law of decay . 
     */

    public float estimateRecipeTime(List<Tuple<string, string, string, string>> recipe)
    {
        float time = 0;

        foreach (Tuple<string, string, string, string> step in recipe)
        {
            time += estimateStepTime(step);
        }

        time += steps.Count * stepTransitionFactor; 

        return time;
    }

    public float estimateStepTime (Tuple<string, string, string, string> step)
    {
        float time = 0;
        float ti = estimateIngredientTime(step.Item1);
        float tc = estimateCookerTime(step.Item2);
        float tp = estimatePatternTime(step.Item3);

        //if (ti < 0) Debug.Log("CRINGE");
        //if (tc < 0) Debug.Log("CRINGE");
        //if (tp < 0) Debug.Log("CRINGE");

        estimatedIngredientTime += ti;
        estimatedCookerTime += tc;  
        estimatedPatternTime += tp;

        time = ti + tc + tp;

        return time; 
    }

    public float estimateIngredientTime(string ingredient)
    {
        int idx = ingredientsId[ingredient];
        List<float> times = ingredientTimes[idx];

        if (times.Count == 0)
        {
            return 15;
        }

        float currentTime = Time.time; 

        float bi = 0;
        
        foreach (float ti in times)
        {
            bi += Mathf.Pow(currentTime - ti, decayFactorIngredient);
        }

        bi = Mathf.Log(bi);

        return ingredientAvg[idx] / bi;
    }

    public float estimateCookerTime(string cooker)
    {
        int idx = cookersId[cooker];
        List<float> times = cookerTimes[idx];

        if (times.Count == 0)
        {
            return 15;
        }

        float currentTime = Time.time;

        float bi = 0;

        foreach (float ti in times)
        {
            bi += Mathf.Pow(currentTime - ti, decayFactorCooker);
        }

        bi = Mathf.Log(bi);

        return cookerAvg[idx] / bi;
    }

    public float estimatePatternTime(string pattern)
    {
        int idx = patternsId[pattern];
        List<float> times = patternTimes[idx];

        if (times.Count == 0)
        {
            return 15;
        }

        float currentTime = Time.time;

        float bi = 0;

        foreach (float ti in times)
        {
            bi += Mathf.Pow(currentTime - ti, decayFactorPattern);
        }

        bi = Mathf.Log(bi);

        return patternAvg[idx] / bi;
    }

    public float estimateChaudronTime()
    {
        float time = 0;
        return time; 
    }

    // Updating The model parameters 

    public void updatePatternTime(string patternLabel, float timeStart)
    {
        float timeDone = Time.time;
        patternTimes[patternsId[patternLabel]].Add(timeDone);
        patternAvg[patternsId[patternLabel]] = (patternAvg[patternsId[patternLabel]] * patternCounts[patternsId[patternLabel]] + timeDone - timeStart) / (patternCounts[patternsId[patternLabel]] + 1);
        patternCounts[patternsId[patternLabel]]++;
    } 

    public void updateCookerTime(string cookerLabel, float timeStart)
    {
        float timeDone = Time.time;
        cookerTimes[cookersId[cookerLabel]].Add(timeDone);
        cookerAvg[cookersId[cookerLabel]] = (cookerAvg[cookersId[cookerLabel]] * cookerCounts[cookersId[cookerLabel]] + timeDone - timeStart) / (cookerCounts[cookersId[cookerLabel]] + 1);
        cookerCounts[cookersId[cookerLabel]]++;
    }

    public void updateIngredientTime(string ingredientLabel, float timeStart)
    {
        float timeDone = Time.time; 
        ingredientTimes[ingredientsId[ingredientLabel]].Add(timeDone);
        ingredientAvg[ingredientsId[ingredientLabel]] = (ingredientAvg[ingredientsId[ingredientLabel]] * ingredientCounts[ingredientsId[ingredientLabel]] + timeDone - timeStart) / (ingredientCounts[ingredientsId[ingredientLabel]] + 1);
        ingredientCounts[ingredientsId[ingredientLabel]]++;
    }
    
    public void updateChaudronTime(float timeStart)
    {

    } 
}