using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternPoint : MonoBehaviour
{

    private Pattern pattern;

    private void Start()
    {
        pattern = transform.parent.GetComponent<Pattern>();
    }

    private void OnTriggerEnter(Collider other)
    {
        pattern.PatternPointTriggered(int.Parse(gameObject.name));
    }
}
