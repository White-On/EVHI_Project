using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaudron : MonoBehaviour
{
    private GameManager gm;

    public Transform[] vfxs; 

    private void Start()
    {
        gm = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.name.Contains(gm.steps[gm.currentStep].Item4))
        {  
            goodFeedBack();
            gm.AddedToCauldron(); 
        }
        else
        {
            if (other.transform.tag == "Ingredient")
            {
                Rigidbody rb = other.transform.GetComponent<Rigidbody>();
                
                rb.AddForce(rb.velocity * -150);
            }
            badFeedback(); 
        }
    }
    private IEnumerator revert(ParticleSystem.MainModule ps, Color color)
    {
        yield return new WaitForSeconds(2);
        ps.startColor = color;
    }


    private void goodFeedBack()
    {
        foreach (Transform t in vfxs)
        {
            ParticleSystem.MainModule ps = t.GetComponent<ParticleSystem>().main;
            StartCoroutine(revert(ps, ps.startColor.color));
            ps.startColor = Color.green;
        }
    }

    private void badFeedback()
    {
        foreach (Transform t in vfxs)
        {
            ParticleSystem.MainModule ps = t.GetComponent<ParticleSystem>().main;
            StartCoroutine(revert(ps, ps.startColor.color));
            ps.startColor = Color.red;
        }
    }
}
