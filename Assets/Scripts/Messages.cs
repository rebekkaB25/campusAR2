using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messages : MonoBehaviour
{

    static Messages instance;
    int like;
    int dislike;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EditComment2()
    {
        CommentDraw.EditComment(this);

    }
}
