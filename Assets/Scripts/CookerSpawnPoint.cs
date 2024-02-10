using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookerSpawnPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ingredient")
        {
            transform.parent.GetComponent<Cooker>().handleIngredientEnter(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ingredient")
        {
            transform.parent.GetComponent<Cooker>().handleIngredientExit(other.transform);
        }
    }
}
