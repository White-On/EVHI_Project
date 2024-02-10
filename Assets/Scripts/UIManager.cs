using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gm; 
    

    public Canvas canvas;

    private List<GameObject> texts = new List<GameObject>();


    public void UpdateUI()
    {
        gm = GameManager.Instance;
        ClearUi();
        
        for (int i = 0; i < gm.steps.Count; i ++)
        {
            GameObject ngo = new GameObject("uistep" + i.ToString());
            texts.Add(ngo);

            ngo.transform.SetParent(canvas.transform);
            ngo.transform.localRotation = Quaternion.EulerAngles(0, 0, 0);
            ngo.transform.localPosition = new Vector3(0, -50, 0) * i;
            ngo.transform.localScale = Vector3.one;



            RectTransform t = ngo.AddComponent<RectTransform>();
            
            t.anchorMax = new Vector2(0,1);
            t.anchorMin = new Vector2(0, 1);
            t.pivot = new Vector2(0, 1);
            t.sizeDelta = new Vector2(800, 50);
            

            TextMeshProUGUI steptext = ngo.AddComponent<TextMeshProUGUI>();
            steptext.fontSize = 36;
         
            steptext.text = i.ToString() + "- " + gm.steps[i].Item1 + " -> " + gm.steps[i].Item2 + " -> " + gm.steps[i].Item3 + " = " + gm.steps[i].Item4;

        }

        texts[0].GetComponent<TextMeshProUGUI>().color = Color.green;
    }

    public void UpdateUIStyle()
    {
        for (int i = 0; i < texts.Count; i++)
        {
            TextMeshProUGUI t = texts[i].GetComponent<TextMeshProUGUI>();
            if (gm.currentStep == i)
            {
                t.color = Color.green; 
            } else if (gm.currentStep > i)
            {
                t.fontStyle = FontStyles.Strikethrough;
                t.color = Color.white;
            }
        }
    }

    public void ClearUi()
    {
        foreach (GameObject go in texts)
        {
            Destroy(go);
        }
        texts = new List<GameObject>();
    }
}
