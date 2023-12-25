using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern : MonoBehaviour
{

    public enum PatternTypes {Single, Repeated}
    public PatternTypes patternType;

    private List<bool> patternPointsState;
    private int patternPointsTriggered;

    void Start()
    {
        patternPointsTriggered = 0;
        patternPointsState = new List<bool>();

        for (int i = 1; i < GetComponentsInChildren<Transform>().Length; i++) patternPointsState.Add(false);
        Debug.Log(patternPointsState.Count);
    }

    public void PatternPointTriggered(int patternPoint)
    {
        
        patternPointsState[patternPoint] = true;
        patternPointsTriggered++;

        if (patternPoint == (patternPointsState.Count - 1)) PatternDone(); //TODO : Add a constraint about the time

    }

    public void PatternDone()
    {
        Debug.Log("Pattern Done with a score = " + patternPointsTriggered + "/" + patternPointsState.Count);
        gameObject.active = false;
    }
}
