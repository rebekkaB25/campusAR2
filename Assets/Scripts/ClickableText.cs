using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ClickableText : MonoBehaviour, IPointerClickHandler //Ui knows we need ponter action
{
    public GameObject Popuppanel;
    public void OnPointerClick(PointerEventData eventData) 
    {
        var text = GetComponent<TextMeshProUGUI>(); //get tmpro component
        if (eventData.button == PointerEventData.InputButton.Left) //maybe change to touch
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null); //null is camera, maybe change to main, gives us index on which index we clicked on
            if (linkIndex > -1) //did we click on a link
            {
                var linkInfo = text.textInfo.linkInfo[linkIndex];
                //var linkID = linkInfo.GetLinkID(); // gets the key <> in the tmpro, gets id

                Popuppanel.SetActive(true);
            }
        }
    }
}

