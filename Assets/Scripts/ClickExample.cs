using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClickExample : MonoBehaviour
{
    [SerializeField] private GameObject pflegeheim;
    [SerializeField] private GameObject kapelle;
    [SerializeField] private GameObject tages;
    [SerializeField] private GameObject wohnen;
    [SerializeField] private GameObject aerzte;
    public Button yourButton;
    

	void Start()
	{
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

    void TaskOnClick()
    {
    pflegeheim.SetActive(false);
    kapelle.SetActive(false);
    tages.SetActive(false);
    wohnen.SetActive(false);
    aerzte.SetActive(false);

    gameObject.SetActive(false);
    }
}