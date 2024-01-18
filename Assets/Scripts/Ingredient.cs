using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public List<Pattern> parts;

    public void addPart(Pattern part)
    {
        //TODO : Traitement pour bien position la nouvelle partie
        parts.Add(part);
    }
}
