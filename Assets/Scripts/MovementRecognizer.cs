using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;

public class MovementRecognizer : MonoBehaviour
{
    private GameManager gm; 

    public XRNode inputSource;
    public InputHelpers.Button inputButton; 
    public float inputThreshold = 0.1f;
    public Transform movementSource;

    public TrailRenderer vfx;

    public float newPositionThresholdDistance = 0.05f;
    public GameObject debugCubePrefab;
    public bool creationMode = true;
    public string newGestureName;

    

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> {  }
    public UnityStringEvent OnRecognized;

    private List<Gesture> trainingSet = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> positionsList = new List<Vector3>();

    void Start()
    {
        gm = GameManager.Instance; 

        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (var file in gestureFiles)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(file)); // C:\Users\Nassim\AppData\LocalLow\DefaultCompany\vr_test
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);

        if (!isMoving && isPressed)
        {
            StartMovement();
        } else if (isMoving && !isPressed)
        {
            EndMovement();
        } else if (isMoving && isPressed)
        {
            UpdateMovement();
        }
    }

    void StartMovement()
    {
        isMoving = true;
        positionsList.Clear();
        positionsList.Add(movementSource.position);

        //TODO : ACTIVATE TRAIL EFFECT HERE

        vfx.emitting = true;

        if (debugCubePrefab)
            Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 3);
    }

    void EndMovement()
    {
        isMoving = false;
        vfx.emitting = false;


        Point[] pointArray = new Point[positionsList.Count];

        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);

        if (creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        } 
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            //Debug.Log(result.GestureClass + result.Score);
            if (result.Score > gm.patternRecognitionThreshold)
            {
                
                if (result.GestureClass == gm.steps[gm.currentStep].Item3)
                {
                    OnRecognized.Invoke(result.GestureClass);
                    goodFeedBack();
                }
                else
                {
                    badFeedback();
                }
            }
            else
            {
                badFeedback();
            }
        }

    }

    void UpdateMovement()
    {
        
        Vector3 lastPosition = positionsList[positionsList.Count - 1];
        if (Vector3.Distance(lastPosition, movementSource.position) > newPositionThresholdDistance)
        {
            positionsList.Add(movementSource.position);
            if (debugCubePrefab)
                Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 3);
        }
        
    }

    private IEnumerator revert()
    {
        yield return new WaitForSeconds(2);
        vfx.startColor = Color.white;
        vfx.endColor = Color.white;
    }


    private void goodFeedBack()
    {
        StartCoroutine(revert());
        vfx.startColor = Color.green;
        vfx.endColor = Color.green;
        //

    }

    private void badFeedback()
    {
        StartCoroutine(revert());
        vfx.startColor = Color.red;
        vfx.endColor = Color.red;
    }
}
