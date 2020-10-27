
using UnityEngine;
using System.Runtime.InteropServices;

namespace Scripts
{
    public class ClickARButton : ARButton
    {
        [SerializeField] private GameObject go;
        [SerializeField] private GameObject button;

        [SerializeField] private bool setVisible;

        protected override void HandleTouch()
        {

            Debug.Log("Touching the button!!!!");

            if (IsOnAndroid)
            {
                go.SetActive(setVisible);
                button.SetActive(setVisible);
            }
            else if (IsOnIOS)
            {
                Debug.Log("Touching the button ON IOSSSS!!!!");
                go.SetActive(setVisible);
                button.SetActive(setVisible);
            }
            else
            {
                Debug.Log("Hit via Raycast");
            }
            

        }
    }
}
