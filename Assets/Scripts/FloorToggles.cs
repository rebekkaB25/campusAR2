using UnityEngine;
using UnityEngine.UI;


public class FloorToggles : MonoBehaviour
{
    public Toggle EGtoggle;
    public Toggle OG1toggle;
    public Toggle OG2toggle;


    public GameObject EG;
    public GameObject OG1;
    public GameObject OG2;

    public GameObject Dieburg;

    public void Start()
    {
        EG.transform.SetParent(null);
        OG1.transform.SetParent(null);
        OG2.transform.SetParent(null);
        EG.transform.position = new Vector3(1.55f, 0.118f, 1.32f);
        OG1.transform.position = new Vector3(1.55f, 0.118f, 1.32f);
        OG2.transform.position = new Vector3(1.55f, 0.118f, 1.32f);
    }

    public void Update()
    {
        if (EGtoggle.GetComponent<Toggle>().isOn == false && OG1toggle.GetComponent<Toggle>().isOn == false && OG2toggle.GetComponent<Toggle>().isOn == false)
        {
            Dieburg.SetActive(true);
        }
    }
    public void ToggleEG (bool newValue1)
    {
        EG.SetActive(newValue1);
        Dieburg.SetActive(false);
      
    }

    public void ToggleOG1(bool newValue2)
    {
        OG1.SetActive(newValue2);
        Dieburg.SetActive(false);
    }

    public void ToggleOG2(bool newValue3)
    {
        OG2.SetActive(newValue3);
        Dieburg.SetActive(false);
    }
}
