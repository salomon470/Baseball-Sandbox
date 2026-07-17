using UnityEngine;

public class SpinAxisViewer : MonoBehaviour
{
    public LayerMask rotLayer;
    bool isRot;
    public int dir;
    public Transform axis;
    public Transform axisRoot;
    public Camera axisCam;

    public float sensitivity = 500f;
    private Vector3 lastMousePosition;
    private Vector3 axisDirection;
    private Vector3 centerPoint;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MouseLook();
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = axisCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, rotLayer))
            {
                isRot = true;
                lastMousePosition = Input.mousePosition;

                // Store the axis direction we'll rotate around
                if (hitInfo.collider.transform.name.Contains("X"))
                {
                    dir = 0;
                    axisDirection = axis.right;
                }
                else if (hitInfo.collider.transform.name.Contains("Y"))
                {
                    dir = 1;
                    axisDirection = axis.up;
                }

                // Get the center point of the ring in world space
                centerPoint = hitInfo.collider.bounds.center;
            }
            else
            {
                isRot = false;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isRot = false;
        }
    }

    void MouseLook()
    {
        if (!isRot)
        {
            // Free rotation when not interacting with an axis
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            transform.Rotate(Vector3.up, mouseX, Space.World);
            transform.localEulerAngles = new Vector3(
                transform.localEulerAngles.x - mouseY,
                transform.localEulerAngles.y + mouseX,
                0f
            );
        }
        else
        {
            // Get current mouse position in screen space
            Vector3 currentMousePosition = Input.mousePosition;

            // Convert mouse positions to directions from center
            Vector3 lastDir = (lastMousePosition - axisCam.WorldToScreenPoint(centerPoint)).normalized;
            Vector3 currentDir = (currentMousePosition - axisCam.WorldToScreenPoint(centerPoint)).normalized;

            // Calculate angle between the two directions
            float angle = Vector3.SignedAngle(lastDir, currentDir, axisCam.transform.forward);

            // Apply rotation around our selected axis
            axis.Rotate(axisDirection, angle * sensitivity * -Time.deltaTime, Space.World);

            lastMousePosition = currentMousePosition;
        }
    }
}