using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class FloorToggles : MonoBehaviour
{
    public Button EGbutton;
    public Button OG1button;
    public Button OG2button;

    public GameObject EG;
    public GameObject OG1;
    public GameObject OG2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleEG (Button EG)
    {
        if (EGbutton.GetComponent<Toggle>().isOn == true)
        {
            EG.SetActive(true);
        }


        if (button.GetComponent<Image>().color == button.colors.normalColor)
        {
            comments.AddRange(GameObject.FindGameObjectsWithTag("Marker"));
            foreach (GameObject go in comments)
            {
                if (go.GetComponentInChildren<LineRenderer>() is null)
                {
                    go.SetActive(false);
                }
            }
            button.GetComponent<Image>().color = button.colors.disabledColor;
        }
        else
        {
            foreach (GameObject go in comments)
            {
                go.SetActive(true);
            }
            button.GetComponent<Image>().color = button.colors.normalColor;
        }
    }
}
