using System.Collections;
using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{

    private static Screenshot instance;
    private Camera myCamera;
    private bool takeScreenshotOnNextFrame;
    private Renderer texRenderer;
    private Sprite MySprite;

    private void Awake()
    {
        instance = this;
        myCamera = gameObject.GetComponent<Camera>();

    }

    
    public void makeScreenshot()
    {
            TakeScreenshot_Static(Screen.width, Screen.height);
        
    }
   

    public void OnPostRender()
    {
        string currentTime = System.DateTime.Now.ToString("HHmmss");

        if (takeScreenshotOnNextFrame)
        {


            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = myCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            //Skalieren der Textur-crash
            //Texture2D newTex = Instantiate (renderResult);
            //texRenderer.material.mainTexture = newTex;
            //TextureScale.Bilinear(newTex, renderResult.width / 4, renderResult.height / 4);

            byte[] byteArray = renderResult.EncodeToPNG();
            //byte[] byteArray = newTex.EncodeToPNG();
            //System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/CameraScreenshot.png", byteArray);   //saves Screenshots to Screenshots Folder

            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/screenshot_" + currentTime + ".png", byteArray);      //auf iOS evtl woanders
            //System.IO.File.WriteAllBytes(Application.persistentDataPath + "/CameraScreenshot.png", byteArray);

            Array.Clear(byteArray, 0, byteArray.Length);
            

            Debug.Log("SavedCameraScreenshot");

            RenderTexture.ReleaseTemporary(renderTexture);
        
            myCamera.targetTexture = null;

        }

    }

    public void TakeScreenshot(int width, int height)
    {
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotOnNextFrame = true;

    }

    public static void TakeScreenshot_Static(int width, int height)
    {
        instance.TakeScreenshot(width, height);
    }
    
}