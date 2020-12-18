using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class CommentDraw : MonoBehaviour
{

    public GameObject marker;
    RaycastHit hitInfo;

    public GameObject markerPanel;
    public InputField field;

    public GameObject ScreenshotPreview;

    public GameObject chatbox;

    GameObject commentMarker;
    
    public GameObject menuPanel;

    static CommentDraw inst;
    GameObject tracked;
    float yaw, pitch, startYaw, startPitch;
    public float rotateSpeed = 1;
    bool touchStartedOnUI = false;

    bool is3D;

    Vector2 fp, lp;

    public ARTrackedImageManager manager;
    Vector3 startPos;

    List<GameObject> infos, comments, drawings, models;

    string Comment3DString = "Legen Sie die Position für den Kommentar fest, " + Environment.NewLine + "indem Sie auf die entsprechende Stelle tippen.";
    string CommentARString = "Legen Sie die Position für den Kommentar fest," + Environment.NewLine +
                    "indem Sie das Tablet bewegen und den Sucher positionieren.";
    const string DrawString = "Zeichnen Sie mit dem Finger auf dem Modell";

    string DrawPlaceString = "Legen Sie den Ausschnitt für die Zeichnung fest," + Environment.NewLine +
                    "indem Sie das Tablet bewegen.";

    string DrawPlaceString3D = "Legen Sie die Position für die Zeichnung fest, " + Environment.NewLine + "indem Sie auf die entsprechende Stelle tippen.";

    string ModelPlaceString = "Legen Sie die Position für das Objekt fest, " + Environment.NewLine + "indem Sie das Tablet bewegen und den Sucher positionieren.";

    string ModelPlaceString3D = "Legen Sie die Position für das Objekt fest, " + Environment.NewLine + "indem Sie auf die entsprechende Stelle tippen.";

    //chat
    [SerializeField] private TMP_Text chatText = null; 
    [SerializeField] private InputField inputField = null; 
    [SerializeField] private GameObject canvas = null;

    private string message;
    public Button commentChatButton;

    LineRenderer lineRenderer;
    List<Vector3> positions;

    public GameObject Line;

    bool startWith3D = false;

    public Image background;

    bool freezeBackground;

    Vector3 drawOrigin;
    Quaternion drawRot;
    bool drawTouchReleased;

    public GameObject pickModelPanel;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
        is3D = false;
        infos = new List<GameObject>();
        comments = new List<GameObject>();
        drawings = new List<GameObject>();
        models = new List<GameObject>();

        positions = new List<Vector3>();
        drawTouchReleased = false;
        pitch = -45;
        yaw = 45;

        commentChatButton.onClick.AddListener(TaskOnClick);

        freezeBackground = true;

        GameObject button = pickModelPanel.transform.Find("CubeButton").gameObject;
        foreach (PrimitiveType primType in (PrimitiveType[])Enum.GetValues(typeof(PrimitiveType)))
        {
            GameObject spawned = Instantiate(button);
            spawned.transform.SetParent(pickModelPanel.transform);
            spawned.transform.localScale = Vector3.one;
            spawned.name = primType.ToString() + "Button";
            spawned.GetComponentInChildren<Text>().text = primType.ToString();
            spawned.GetComponent<Button>().onClick.AddListener(() => AddModel(primType.ToString()));
        }

        foreach (GameObject go in Resources.LoadAll<GameObject>(""))
        {
            GameObject spawned = Instantiate(button);
            spawned.transform.SetParent(pickModelPanel.transform);
            spawned.transform.localScale = Vector3.one;
            spawned.name = go.name + "Button";
            spawned.GetComponentInChildren<Text>().text = go.name;
            spawned.GetComponent<Button>().onClick.AddListener(() => AddModel(go, true));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tracked)
        {
            if (startWith3D) //is true after switching to 3D mode
            {
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Tracked")) 
                {
                    if (go != tracked)
                    {
                        while (tracked.transform.childCount > 0)
                        {
                            Transform child = tracked.transform.GetChild(0);
                            Vector3 localPos = child.localPosition;
                            Quaternion localRot = child.localRotation;
                            child.SetParent(go.transform.Find("CommentParent"));
                            child.localPosition = localPos;
                            child.localRotation = localRot;
                        }
                        Destroy(tracked);
                        tracked = go;

                        startWith3D = false;
                        break;
                    }
                }
            }
            marker.transform.parent = tracked.transform; 
        }
        else 
           tracked = GameObject.FindGameObjectWithTag("Tracked");



        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                fp = touch.position;
                lp = touch.position;
                startYaw = yaw;
                startPitch = pitch;
                startPos = tracked.transform.localPosition;
                if (IsPointerOverGameObject())
                    touchStartedOnUI = true;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                lp = touch.position;
                if (!drawTouchReleased)
                {
                    positions.Clear();
                }
                drawTouchReleased = true;
            }

            if (is3D)
            {
                string markerPanelText = markerPanel.GetComponentInChildren<Text>().text;
                if (markerPanel.activeSelf && (markerPanelText == Comment3DString || markerPanelText == DrawPlaceString3D || markerPanelText == ModelPlaceString3D))
                {
                    int layerMask = ~(1 << 11);
                    Ray ray = Camera.main.ScreenPointToRay(new Vector3(lp.x, lp.y, 1));
                    RaycastHit tempHit;
                    if (Physics.Raycast(ray, out tempHit, Mathf.Infinity, layerMask))
                    {
                        hitInfo = tempHit;

                        marker.transform.position = hitInfo.point;
                        Vector3 tangent = Vector3.forward;
                        Vector3 normal = hitInfo.normal;
                        Vector3.OrthoNormalize(ref normal, ref tangent);
                        marker.transform.rotation = Quaternion.LookRotation(tangent, normal);

                        if (hitInfo.collider.GetComponent<Comment>())
                        {
                            marker.transform.position = hitInfo.transform.position;
                        }
                    }

                }
                else if ((!markerPanel.activeSelf || markerPanelText != DrawString) && !IsPointerOverGameObject() && !touchStartedOnUI && touch.phase != TouchPhase.Ended) //evtl müssen hier neue ScreenshotStrings hinzugefügt werden
                {
                    yaw = startYaw + rotateSpeed * (-lp.x + fp.x);
                    pitch = startPitch + rotateSpeed * (lp.y - fp.y);
                    pitch = Mathf.Clamp(pitch, -90, 0);
                    tracked.transform.localEulerAngles = new Vector3(0, yaw, 0);
                    tracked.transform.Rotate(Camera.main.transform.right, pitch, Space.World);
                }
            }

            if (markerPanel.activeSelf)
            {
                string markerPanelText = markerPanel.GetComponentInChildren<Text>().text;

                if (markerPanelText == DrawPlaceString || markerPanelText == DrawPlaceString3D)
                {
                    marker.transform.Find("drawingModel").GetComponent<MeshRenderer>().enabled = true;
                    marker.transform.Find("commentModel").GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    marker.transform.Find("drawingModel").GetComponent<MeshRenderer>().enabled = false;
                    marker.transform.Find("commentModel").GetComponent<MeshRenderer>().enabled = true;
                }


                if (markerPanelText == DrawString)  //drawing function
                {
                    int layerMask = ~(1 << 11);
                    
                    Ray ray = Camera.main.ScreenPointToRay(new Vector3(lp.x, lp.y, 1));
                    RaycastHit tempHit;
                    if (Physics.Raycast(ray, out tempHit, Mathf.Infinity, layerMask))
                    {
                        lineRenderer.SetWidth(0.01f, 0.01f);
                        lineRenderer.SetColors(Color.black, Color.black); 
                        hitInfo = tempHit;
                        positions.Add(lineRenderer.transform.InverseTransformPoint(hitInfo.point));
                        lineRenderer.positionCount = positions.Count;
                        lineRenderer.SetPositions(positions.ToArray());
                    }
                }
            }
           
            if (touch.phase == TouchPhase.Ended)
            {
                touchStartedOnUI = false;
            }

        }

        if (!is3D)
        {
            int layerMask = ~(1 << 11);
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, Mathf.Infinity, layerMask))
            {
                //Debug.Log("We hit something");
                marker.transform.position = hitInfo.point;
                Vector3 tangent = Vector3.forward;
                Vector3 normal = hitInfo.normal;
                Vector3.OrthoNormalize(ref normal, ref tangent);
                marker.transform.rotation = Quaternion.LookRotation(tangent, normal);
                //Debug.Log("Set Position to " + hitInfo.point);

                if (hitInfo.collider.GetComponent<Comment>())
                {
                    marker.transform.position = hitInfo.transform.position;
                }

            }
     
        }

        //activates campus if floors are disabled

        if ((GameObject.FindGameObjectWithTag("eg").GetComponent<MeshRenderer>().enabled == false) &&
            (GameObject.FindGameObjectWithTag("og1").GetComponent<MeshRenderer>().enabled == false) &&
            (GameObject.FindGameObjectWithTag("og2").GetComponent<MeshRenderer>().enabled == false) )
        {
            GameObject.FindGameObjectWithTag("campus").GetComponent<MeshRenderer>().enabled = true;
        }

    }


    public static bool IsPointerOverGameObject()
    {
        // Check mouse
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        // Check touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return true;
            }
        }

        return false;
    }


    public void Place()
    {
        if (markerPanel.GetComponentInChildren<Text>().text == DrawString) 
        {
            //freezeBackground = false;

            markerPanel.SetActive(false); 
            field.transform.parent.gameObject.SetActive(true);
            field.Select();
        }
        else
        {
            if (hitInfo.collider)
            {
                if (hitInfo.collider.GetComponent<Comment>())
                {
                    commentMarker = hitInfo.collider.gameObject;
                    field.text = commentMarker.GetComponent<Comment>().text;
                }
                else
                {
                    if (marker.transform.parent)
                        commentMarker = Instantiate(marker, marker.transform.parent);
                    else
                    {
                        commentMarker = Instantiate(marker);
                        commentMarker.transform.SetParent(tracked.transform.Find("CommentParent")); //auch parent für floors?
                    }
                    commentMarker.GetComponent<SphereCollider>().enabled = true;

                    if (marker.transform.Find("ModelComment"))
                    {
                        Destroy(marker.transform.Find("ModelComment").gameObject);
                        marker.transform.Find("marker").gameObject.SetActive(false);
                    }
                }

                if (markerPanel.GetComponentInChildren<Text>().text == DrawPlaceString || markerPanel.GetComponentInChildren<Text>().text == DrawPlaceString3D)
                {
                    drawTouchReleased = false;
                    manager.enabled = false;

                    tracked.transform.SetParent(transform);
                    drawOrigin = tracked.transform.localPosition;
                    drawRot = tracked.transform.localRotation;

                    //freezeBackground = true;

                    markerPanel.GetComponentInChildren<Text>().text = DrawString;
                    GameObject spawned = Instantiate(Line, Vector3.zero, Quaternion.identity, commentMarker.transform);
                    lineRenderer = spawned.GetComponent<LineRenderer>();
                    spawned.transform.localPosition = Vector3.zero;
                    spawned.transform.localRotation = Quaternion.identity;

                    marker.SetActive(false);
                }
                else
                {
                    freezeBackground = false;
                    field.transform.parent.gameObject.SetActive(true);
                    field.Select();
                    markerPanel.SetActive(false);
                    marker.SetActive(false);
                }
            }
        }
    }



    public void OnPostRender() //screenshot
    {
        //Screenshot.TakeScreenshot_Static(Screen.width, Screen.height);
        //IMG2Sprite.GetPhoto_static();

        //freezeBackground = false;
        
    }


    
    
    public void Comment()
    {
        chatbox.SetActive(false);

        if (field.text == "")
        {
            Destroy(commentMarker);
        }
        else
        {
            Comment comment = commentMarker.GetComponent<Comment>();
            
            comment.chat = chatText.text; 
            comment.text = field.text;
            comment.enabled = true;
            commentMarker = null;
        }

        if (lineRenderer)
        {
            lineRenderer.enabled = false;
            if (!is3D) 
            {
                tracked.transform.parent = null;
                manager.enabled = true;
            }
        }

        field.transform.parent.gameObject.SetActive(false);
        field.text = "";
        chatText.text = "";
    }


    //try with static

    public static void _Comment()
    {
        inst.Comment();
    }

    public void CancelComment()
    {
        if (commentMarker != null)
        {
            var line = commentMarker.GetComponentInChildren<LineRenderer>();
            if (line)
                Destroy(line);
            if (commentMarker != null)
                Destroy(commentMarker);
            field.text = "";
            field.transform.parent.gameObject.SetActive(false);

            if (!is3D)
            {
                tracked.transform.parent = null;
                manager.enabled = true;
            }
        }

        if (marker.transform.Find("ModelComment"))
        {
            Destroy(marker.transform.Find("ModelComment").gameObject);
        }
    }

    public void StartComment()
    {
        chatbox.SetActive(false);

        if (is3D)
            markerPanel.GetComponentInChildren<Text>().text = Comment3DString;
        else
            markerPanel.GetComponentInChildren<Text>().text = CommentARString;
        marker.SetActive(true);
        marker.transform.Find("marker").gameObject.SetActive(true);
        marker.transform.Find("commentModel").gameObject.SetActive(true);
    }

    public static void EditComment(Comment comment)
    {
        inst._EditComment(comment);
    }

    void _EditComment(Comment comment)
    {
        if (field.transform.parent.gameObject.activeSelf || markerPanel.activeSelf)
            return;

        chatbox.SetActive(true);
        menuPanel.SetActive(false); 
     
        chatText.text = comment.chat;

        field.text = comment.text;
        commentMarker = comment.gameObject;
        field.transform.parent.gameObject.SetActive(true);
        LineRenderer line = commentMarker.GetComponentInChildren<LineRenderer>();
        if (line)
            line.enabled = true;
    }
   
    public void TaskOnClick() //chat
    {
        if (inputField.text != "")
        {
            string currentTime = DateTime.Now.ToString("ddd, dd.MM.yy HH:mm");
            message = inputField.text;
            chatText.text += "[" + currentTime + "]: "+ Environment.NewLine + message + Environment.NewLine + Environment.NewLine;

            message = null;
            inputField.text = string.Empty;
        }
    }


    public void SwitchAR3D()
    {
        if (tracked)
        {
            if (is3D) //goes in AR
            {
                tracked.transform.Find("Image 2").GetComponent<MeshRenderer>().enabled = false;

                if (markerPanel.GetComponentInChildren<Text>().text != DrawString || !markerPanel.activeSelf)
                {
                    tracked.SetActive(false);
                    tracked.transform.parent = null;
                    manager.enabled = true;
                }
                else
                {
                    tracked.transform.localPosition = drawOrigin;
                    tracked.transform.localRotation = drawRot;
                }

                GetComponent<ARCameraBackground>().enabled = true;
                if (markerPanel.GetComponentInChildren<Text>().text == Comment3DString)
                    markerPanel.GetComponentInChildren<Text>().text = CommentARString;

                if (markerPanel.GetComponentInChildren<Text>().text == DrawPlaceString3D)
                    markerPanel.GetComponentInChildren<Text>().text = DrawPlaceString;

                if (markerPanel.GetComponentInChildren<Text>().text == ModelPlaceString3D)
                    markerPanel.GetComponentInChildren<Text>().text = ModelPlaceString;

                is3D = false;
                
            }
            else //goes in 3D
            {
                tracked.transform.Find("Image 2").GetComponent<MeshRenderer>().enabled = true;

                manager.enabled = false;
                tracked.transform.SetParent(Camera.main.transform);
                tracked.transform.localPosition = new Vector3(0, 0, 0.5f);
                tracked.SetActive(true);
                pitch = Mathf.Clamp(pitch, -90, 0);
                tracked.transform.localEulerAngles = new Vector3(0, yaw, 0);
                tracked.transform.Rotate(Camera.main.transform.right, pitch, Space.World);
                if (markerPanel.GetComponentInChildren<Text>().text == CommentARString)
                    markerPanel.GetComponentInChildren<Text>().text = Comment3DString;

                if (markerPanel.GetComponentInChildren<Text>().text == DrawPlaceString)
                    markerPanel.GetComponentInChildren<Text>().text = DrawPlaceString3D;

                if (markerPanel.GetComponentInChildren<Text>().text == ModelPlaceString)
                    markerPanel.GetComponentInChildren<Text>().text = ModelPlaceString3D;

                GetComponent<ARCameraBackground>().enabled = false;

                is3D = true;
            }

        }
        else // !tracked, beginning state
        {
            if (is3D) 
            {
                
            }
            else //Ar switches to 3D for the first time
            {
                manager.enabled = false;
                tracked = Instantiate(manager.trackedImagePrefab);

                tracked.transform.Find("Image 2").GetComponent<MeshRenderer>().enabled = true;

                tracked.transform.SetParent(Camera.main.transform);
                tracked.transform.localPosition = new Vector3(0, 0, 0.5f);
                tracked.SetActive(true);
                pitch = Mathf.Clamp(pitch, -90, 0);
                tracked.transform.localEulerAngles = new Vector3(0, yaw, 0);
                tracked.transform.Rotate(Camera.main.transform.right, pitch, Space.World);


                if (markerPanel.GetComponentInChildren<Text>().text == CommentARString)
                    markerPanel.GetComponentInChildren<Text>().text = Comment3DString;

                if (markerPanel.GetComponentInChildren<Text>().text == DrawPlaceString)
                    markerPanel.GetComponentInChildren<Text>().text = DrawPlaceString3D;

                if (markerPanel.GetComponentInChildren<Text>().text == ModelPlaceString)
                    markerPanel.GetComponentInChildren<Text>().text = ModelPlaceString3D;

                GetComponent<ARCameraBackground>().enabled = false;

                startWith3D = true;

                is3D = true;
            }
        }
    }

    //toggles for floors

    public void ToggleEG(Toggle toggleEG)
    {
        if (toggleEG.isOn)
        {
            GameObject.FindGameObjectWithTag("eg").GetComponent<MeshRenderer>().enabled = true;
            GameObject.FindGameObjectWithTag("campus").GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            GameObject.FindGameObjectWithTag("eg").GetComponent<MeshRenderer>().enabled = false;
        }
        
    }

    public void ToggleOG1(Toggle toggleOG1)
    {
        if (toggleOG1.isOn)
        {
            GameObject.FindGameObjectWithTag("og1").GetComponent<MeshRenderer>().enabled = true;
            GameObject.FindGameObjectWithTag("campus").GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            GameObject.FindGameObjectWithTag("og1").GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void ToggleOG2(Toggle toggleOG2)
    {
        if (toggleOG2.isOn)
        {
            GameObject.FindGameObjectWithTag("og2").GetComponent<MeshRenderer>().enabled = true;
            GameObject.FindGameObjectWithTag("campus").GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            GameObject.FindGameObjectWithTag("og2").GetComponent<MeshRenderer>().enabled = false;
        }
    }


    //toggles for infos, drawings, comments

    public void ToggleInfos(Button button)
    {
        if (button.GetComponent<Image>().color == button.colors.normalColor)
        {
            infos.AddRange(GameObject.FindGameObjectsWithTag("billboard"));
            foreach (GameObject go in infos)
            {
                go.SetActive(false);

            }
            button.GetComponent<Image>().color = button.colors.disabledColor;
        }
        else
        {
            foreach (GameObject go in infos)
            {
                go.SetActive(true);
            }
            button.GetComponent<Image>().color = button.colors.normalColor;
        }
    }

    public void ToggleComments(Button button)
    {
        if (button.GetComponent<Image>().color == button.colors.normalColor)
        {
            comments.AddRange(GameObject.FindGameObjectsWithTag("Marker"));
            foreach (GameObject go in comments)
            {
                if (go.GetComponentInChildren<LineRenderer>() is null && go.GetComponentInChildren<ModelMarker>() is null)
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
                if (go.GetComponentInChildren<LineRenderer>() is null && go.GetComponentInChildren<ModelMarker>() is null)
                {
                    go.SetActive(true);
                }
            }
            button.GetComponent<Image>().color = button.colors.normalColor;
        }
    }

    public void ToggleDrawings(Button button)
    {
        if (button.GetComponent<Image>().color == button.colors.normalColor)
        {

            drawings.AddRange(GameObject.FindGameObjectsWithTag("Marker"));
            foreach (GameObject go in drawings)
            {
                if (go.GetComponentInChildren<LineRenderer>() && (go.GetComponentInChildren<ModelMarker>() is null))
                {
                    go.SetActive(false);
                }
            }
            button.GetComponent<Image>().color = button.colors.disabledColor;
        }
        else
        {
            foreach (GameObject go in drawings)
            {
                if (go.GetComponentInChildren<LineRenderer>() && (go.GetComponentInChildren<ModelMarker>() is null))
                {
                    go.SetActive(true);
                }
            }
            button.GetComponent<Image>().color = button.colors.normalColor;
        }
    }

    public void ToggleModels(Button button)
    {
        if (button.GetComponent<Image>().color == button.colors.normalColor)
        {

            models.AddRange(GameObject.FindGameObjectsWithTag("Marker"));
            foreach (GameObject go in models)
            {
                if (go.GetComponentInChildren<ModelMarker>() && (go.GetComponentInChildren<LineRenderer>() is null) )
                {
                    go.SetActive(false);
                }
            }
            button.GetComponent<Image>().color = button.colors.disabledColor;
        }
        else
        {
            foreach (GameObject go in models) 
            {
                if (go.GetComponentInChildren<ModelMarker>() && (go.GetComponentInChildren<LineRenderer>() is null))
                {
                    go.SetActive(true);
                }
            }
            button.GetComponent<Image>().color = button.colors.normalColor;
        }
    }


    public void Draw()
    {
        if (is3D)
            markerPanel.GetComponentInChildren<Text>().text = DrawPlaceString3D;
        else
            markerPanel.GetComponentInChildren<Text>().text = DrawPlaceString;
        marker.SetActive(true);
        marker.transform.Find("marker").gameObject.SetActive(true);
      
    }

    public void AddModel(string name)
    {
        pickModelPanel.SetActive(false);
        GameObject spawned = null;
        PrimitiveType primitiveType = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), name);

        spawned = GameObject.CreatePrimitive(primitiveType);
        AddModel(spawned, false);
    }

    public void AddModel(GameObject go, bool isPrefab)
    {
        pickModelPanel.SetActive(false);
        GameObject spawned = null;
        if (isPrefab)
            spawned = Instantiate(go);
        else
            spawned = go;
        spawned.name = "ModelComment";
        spawned.layer = 11;
        
        if (spawned.GetComponent<MeshRenderer>() && !isPrefab)
            spawned.GetComponent<MeshRenderer>().material.color = Color.black;
        spawned.transform.SetParent(marker.transform);
        if (spawned.GetComponentInChildren<MeshFilter>())
        {
            float min = float.MaxValue;
            foreach (MeshFilter mf in spawned.GetComponentsInChildren<MeshFilter>())
            {
                if (mf.mesh.bounds.min.y < min)
                    min = mf.mesh.bounds.min.y;
            }
            spawned.transform.localPosition = -Vector3.up * min;
        }
        else
            spawned.transform.localPosition = Vector3.zero;
        spawned.transform.localRotation = Quaternion.identity;
        spawned.transform.localScale = Vector3.one;

        var type = Type.GetType("ModelMarker");
        marker.AddComponent(type);

        marker.transform.Find("marker").gameObject.SetActive(false);
        marker.transform.Find("commentModel").gameObject.SetActive(false); //deactivates comment marker when objects is spawned
        marker.SetActive(true);
        if (is3D)
            markerPanel.GetComponentInChildren<Text>().text = ModelPlaceString3D;
        else
            markerPanel.GetComponentInChildren<Text>().text = ModelPlaceString;
        markerPanel.SetActive(true);

    }
}