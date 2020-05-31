using Constants;
using UnityEngine;

namespace GUI
{
    public class CameraController : MonoBehaviour
    {
        public Camera Camera;
        public float Speed = 1.5f;

        public void Start()
        {
            Camera = GetComponent<Camera>();
        }
        
        public void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (GuiConstants.IsInModelPreview)
                {
                    var activeModel = GameObject.Find("activeModel");
                    if (activeModel)
                    {
                        var inputX = Input.GetAxis("Mouse X") * Speed;
                        var inputY = Input.GetAxis("Mouse Y") * Speed;
                        
                        activeModel.transform.Rotate(activeModel.transform.forward, -inputY, Space.World);
                        activeModel.transform.Rotate(activeModel.transform.up, -inputX, Space.World);

                        var quaternion = activeModel.transform.rotation;
                        quaternion.eulerAngles = new Vector3(0, quaternion.eulerAngles.y, quaternion.eulerAngles.z);
                        activeModel.transform.rotation = quaternion;
                    }
                }
            }
        }
    }
}