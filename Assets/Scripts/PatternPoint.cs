using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternPoint : MonoBehaviour
{

    private Pattern parent;

    private void Start()
    {
        parent = transform.parent.GetComponent<Pattern>();
    }

    private void OnTriggerEnter(Collider other)
    {
        parent.PatternPointTriggered(int.Parse(gameObject.name));
    }
}
