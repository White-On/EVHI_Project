using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooker : MonoBehaviour
{
    private GameManager gm;
    public Transform[] inRangeFXs; 

    public bool playerInRange = false;
    public bool ingredientInRange = false;

    public Transform ingredientInside; 

    private void Start()
    {
        DisableInRangeFx();
        gm = GameManager.Instance;
    }

    public void handleIngredientEnter(Transform ingredient)
    {
        if (ingredient.name.Contains(gm.steps[gm.currentStep].Item1)) //Correct Ingredient
        {
            //disable object grabs
            ingredientInRange = true;
            ingredientInside = ingredient; 
            goodFeedBack();
            gm.OnCookerEvent(); // USE THIS IF WANT TO CALL WHEN THE USER PUTS THE INGREDIENT IN TO COOKING AREA or GO READ FUNCTION handleCooker
        }
        else // Wrong Ingredient
        {
            Rigidbody rb = ingredient.GetComponent<Rigidbody>();
            rb.AddForce(rb.velocity * -150);
            badFeedback(); 
        }
    }

    public void handleIngredientExit(Transform ingredient)
    {
        if (ingredient.name.Contains(gm.steps[gm.currentStep].Item1)) //Correct Ingredient
        {
            //disable object grabs
            ingredientInRange = false;
            ingredientInside = null; 
        }
    }

    public void handleCooker()
    {
        if (gm.steps[gm.currentStep].Item2 == name)
        {
            //Then correct pattern
            goodFeedBack();
            //gm.OnCookerEvent(); // USE THIS IF WANT TO CALL RIGHT WHEN THE USER ENTERS THE COOKER AREA or GO READ FUNCTION handleIngredientEnter
        }
        else
        {
            //False pattern
            badFeedback();
        }
    }

    public void OnTriggerEnter(Collider other) // Handles cooker
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = true;
            EnableInRangeFX();
            handleCooker();
        }
    }

    private void EnableInRangeFX()
    {
        foreach (Transform t in inRangeFXs)
        {
            //t.gameObject.SetActive(true);
            t.GetComponent<ParticleSystem>().Play();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
            DisableInRangeFx();
        }
    }

    private void DisableInRangeFx()
    {
        foreach (Transform t in inRangeFXs)
        {
            //t.gameObject.SetActive(false);
            t.GetComponent<ParticleSystem>().Stop();
        }
    }

    private void goodFeedBack()
    {
        //Good VFX and SFX
        foreach (Transform t in inRangeFXs)
        {
            ParticleSystem.MainModule ps = t.GetComponent<ParticleSystem>().main;
            StartCoroutine(revert(ps, ps.startColor.color));
            ps.startColor = Color.green;
        }
        //Debug.Log("good feedback");
    }

    private IEnumerator revert(ParticleSystem.MainModule ps, Color color)
    {
        yield return new WaitForSeconds(2);
        ps.startColor = color; 
    }

    private void badFeedback()
    {
        //Bad VFX and SFX
        foreach (Transform t in inRangeFXs)
        {
            ParticleSystem.MainModule ps = t.GetComponent<ParticleSystem>().main;
            StartCoroutine(revert(ps, ps.startColor.color));
            ps.startColor = Color.red;
        }
        //Debug.Log("bad feedback");
    }
}
