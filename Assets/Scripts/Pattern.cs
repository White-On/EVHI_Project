using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern : MonoBehaviour
{

    public enum PatternTypes {Single, Repeated}
    public PatternTypes patternType;

    [Range(0.0f,10f)] public float hitboxSizeFactor;



    private List<bool> patternPointsState;
    private int patternPointsTriggered;

    private int lastPatternPointTriggered; 

    void Start()
    {
        patternPointsTriggered = 0;
        lastPatternPointTriggered = -1;
        patternPointsState = new List<bool>();

        for (int i = 1; i < GetComponentsInChildren<Transform>().Length; i++) patternPointsState.Add(false);

        //TODO Apply the hitboxSizeFactor
    }

    public void PatternPointTriggered(int patternPoint)
    {
        if (patternPoint > lastPatternPointTriggered)
        {
            patternPointsState[patternPoint] = true;
            patternPointsTriggered++;
            lastPatternPointTriggered = patternPoint;

            if (patternPoint == (patternPointsState.Count - 1)) PatternDone(); //TODO : Add a constraint about the time
        }
        

    }

    public void PatternDone()
    {
        Debug.Log("Pattern Done with a score = " + patternPointsTriggered + "/" + patternPointsState.Count);
        //gameObject.SetActive(false);
    }
}
