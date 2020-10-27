using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class IMG2Sprite : MonoBehaviour
{
    public GameObject finalScreenShot;
    private static IMG2Sprite instance;
    

    private void Awake()
    {
        instance = this;

    }

    public static void GetPhoto_static()
    {
        instance.GetPhoto();
    }

    public void GetPhoto()
    {
        string currentTime = System.DateTime.Now.ToString("HHmmss");

        //string url = "Assets/Screenshots/CameraScreenshot.png";  //works on pc not on ios
        string url = Application.persistentDataPath + "/screenshot_" + currentTime + ".png";

        //string url = Application.persistentDataPath + "/CameraScreenshot.png";


        var bytes = File.ReadAllBytes(url);
        Texture2D texture = new Texture2D(2, 2);
        bool imageLoadSuccess = texture.LoadImage(bytes);
        while (!imageLoadSuccess)
        {
            print("image load failed");
            bytes = File.ReadAllBytes(url);
            imageLoadSuccess = texture.LoadImage(bytes);
        }
        print("Image load success: " + imageLoadSuccess);
        finalScreenShot.GetComponent<Image>().overrideSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 0f), 100f);
    }

}