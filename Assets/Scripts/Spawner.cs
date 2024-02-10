using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform objectToSpawn;

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Ingredient")
        {
            Instantiate(objectToSpawn, transform.position, transform.rotation);
        }
    }
}
