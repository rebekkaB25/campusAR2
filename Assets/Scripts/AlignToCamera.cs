using UnityEngine;
//using Vuforia;

namespace Scripts
{
    public class AlignToCamera : MonoBehaviour
    {
        protected virtual void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
            Vector3 v = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(v.x, v.y, 0);
        }
    }
}
