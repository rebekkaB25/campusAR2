
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

/// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
/// and overlays some information as well as the source Texture2D on top of the
/// detected image.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageInfoManager : MonoBehaviour
{

    //[SerializeField] private GameObject no_Marker_Canvas;

    private bool oneMarkerVisible = false;

    private bool isInstantiated;
    //create the “trackable” manager to detect 2D images
    ARTrackedImageManager m_TrackedImageManager;

    //call Awake method to initialize tracked image manager 
    void Awake()
    {
        //initialized tracked image manager  
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        isInstantiated = false;
    }


    //when the tracked image manager is enabled add binding to the tracked 
    //image changed event handler by calling a method to iterate through 
    //image reference’s changes 
    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }


    //when the tracked image manager is disabled remove binding to the 
    //tracked image changed event handler by calling a method to iterate 
    //through image reference’s changes
    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }


    //method to iterate tracked image changed event handler arguments 
    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // for each tracked image that has been added
        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            //trackedImage.transform.localScale = new Vector3(1f, 1f, 1f);

        }

        // for each tracked image that has been updated
        foreach (var trackedImage in eventArgs.updated)
        {
            //throw tracked image to check tracking state
            UpdateGameObject(trackedImage);
        }


        // for each tracked image that has been removed  
        foreach (var trackedImage in eventArgs.removed)
        {

            // destroy the AR object associated with the tracked image
            Destroy(trackedImage.gameObject);
        }

    }


    //https://vrgamedevelopment.pro/master-image-tracking-with-arkit-3-part-2/

    // method to update image tracked game object visibility
    void UpdateGameObject(ARTrackedImage trackedImage)
    {
        Debug.Log("Tracking State: " + "Tracking Image");
        //if tracked image tracking state is comparable to tracking
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            //This Code worked for IOS, but not for Android
            //Debug.Log("Tracking State: Ich will die Canvas AUSSCHALTEN");

        Debug.Log("Tracking State: Bild gefunden: " + trackedImage.referenceImage.name);

            if (trackedImage.referenceImage.name == "caritas")
            {
                trackedImage.gameObject.SetActive(true);

                //Debug.Log("Prefab game Object: " + trackedImage.gameObject.name);

                //trackedImage.gameObject.transform.Find("P7_MotorTarget").gameObject.SetActive(true);
                //trackedImage.gameObject.transform.Find("MotorTarget").gameObject.SetActive(true);
                //trackedImage.gameObject.transform.Find("P5_WaterTarget").gameObject.SetActive(true);

            }
            
            
            
        }
        else if (trackedImage.trackingState == TrackingState.Limited)//if tracked image tracking state is limited or none 
        {
            Debug.Log("Tracking State: " + "Limited");

            
                trackedImage.gameObject.SetActive(false);
            


        }
        else if (trackedImage.trackingState == TrackingState.None)//if tracked image tracking state is limited or none 
        {

            Debug.Log("Tracking State: " + "None");

        }
    }
}
