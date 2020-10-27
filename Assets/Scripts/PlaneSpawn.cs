using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;

public class PlaneSpawn : MonoBehaviour
{
    RaycastHit hitInfo;
    public Transform model;

    Vector3 scale;
    int downloadsRemaining;

    public GameObject marker;

    public Material transparentMat;

    // Start is called before the first frame update
    void Start()
    {
        scale = model.localScale;
        string downloadPrefix = "https://cloud.awesome-technologies.de/index.php/s/RaZJJykyLedrK28/download?path=%2FDieburg&files=";

        string[] files = new string[] { "Dieburg_houses.obj", "Dieburg_plane.obj", "Dieburg_windows.obj", "_tmp_plan.jpg" };
        downloadsRemaining = 0;
        foreach(string file in files)
        {
            ++downloadsRemaining;
            StartCoroutine(DownloadFile(downloadPrefix + file, file));
        }

        
        
    }

    GameObject SpawnMesh(string path, bool transparent=false, string texture=null, Color ?color=null)
    {
        GameObject spawned = new GameObject(path);
        MeshFilter mf = spawned.AddComponent<MeshFilter>();
        MeshRenderer mr = spawned.AddComponent<MeshRenderer>();
        mf.mesh = FastObjImporter.Instance.ImportFile(path);
        if (transparent)
            mr.material = new Material(this.transparentMat);
        else
            mr.material = new Material(Shader.Find("Standard"));
        if(texture != null)
            mr.material.mainTexture = LoadPNG(texture);
        if(color.HasValue)
            mr.material.color = (Color) color;

        spawned.AddComponent<MeshCollider>();
        return spawned;
    }

    public static Color getColor(float[] array)
    {
        if (array.Length == 4)
            return new Color(array[0], array[1], array[2], array[3]);
        return new Color(array[0], array[1], array[2]);
    }

    IEnumerator DownloadFile(string url, string filename)
    {
        var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        
        string path = Path.Combine(Application.persistentDataPath, filename);
        uwr.downloadHandler = new DownloadHandlerFile(path);
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError || uwr.isHttpError)
            Debug.LogError(uwr.error);
        else
        {
            Debug.Log("File successfully downloaded and saved to " + path);
            downloadsRemaining--;

        }
            
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    // Update is called once per frame
    void Update()
    {
        
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                //Debug.Log("Touched the screen");
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                int layerMask = 1 << 10;
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
                {
                    //Debug.Log("We hit something");
                    ARPlane plane = hitInfo.collider.GetComponent<ARPlane>();
                    if (plane)
                    {
                        model.position = plane.center + Vector3.up * 0.3f;
                        model.localScale = scale * Mathf.Min(plane.size.x, plane.size.y);
                    }
                }
            }
        }

        if(downloadsRemaining == 0)
        {
            Debug.Log("no more Downloads remaining");
            Transform parent = SpawnMesh(Path.Combine(Application.persistentDataPath, "Dieburg_houses.obj")).transform;
            SpawnMesh(Path.Combine(Application.persistentDataPath, "Dieburg_plane.obj"), texture: Path.Combine(Application.persistentDataPath, "_tmp_plan.jpg")).transform.parent = parent;
            SpawnMesh(Path.Combine(Application.persistentDataPath, "Dieburg_windows.obj"), transparent: true, color: new Color(0.3921569f, 0.5843137f, 0.9294118f, 0.5019608f)).transform.parent = parent;
            parent.localScale = new Vector3(0.13f, 0.13f, -0.13f);
            if (marker)
                marker.transform.parent = parent;
            model.gameObject.SetActive(false);
            model = parent;
            
            downloadsRemaining = -1;
            
        }

    }
}
