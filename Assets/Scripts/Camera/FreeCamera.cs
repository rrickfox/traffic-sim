using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    [Header("Fly Settings")]
    public float flySpeed = 0.1f;
    [Header("Rotation Settings")]
    public float turnSpeed = 1f;
    public float maxTurnAngle = 90f;
    public float minTurnAngle = 20f;
    [Header("Zoom Settings")]
    public float minZoom = 10f;
    public float maxZoom = 20f;
    public float zoomSpeed = 10f;
    [Header("Borders")]
    public Vector3 border1;
    public Vector3 border2;

    Vector3 newPosition;
    Camera cam;
    float camDistance = 5f;

    // Setting camera right, focus the center
    void Start()
    {
        cam = Camera.main;
        cam.transform.LookAt(transform.position);
    }

    void FixedUpdate()
    {
        if (!cam.enabled) return;
        Rotate();
        Move();
        Zoom();
    }

    // Rotating Camera around center while right mousebutton is pressed, up-down-rotation (X-axis) can be limited
    void Rotate()
    {
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(0f, Input.GetAxis("Mouse X") * turnSpeed, 0f);
            if (cam.transform.rotation.eulerAngles.x <= maxTurnAngle && cam.transform.rotation.eulerAngles.x >= minTurnAngle)
                cam.transform.RotateAround(transform.position, cam.transform.right, -1 * Input.GetAxis("Mouse Y") * turnSpeed);
            if (cam.transform.rotation.eulerAngles.x < minTurnAngle)
            {
                cam.transform.RotateAround(transform.position, cam.transform.right, minTurnAngle - cam.transform.rotation.eulerAngles.x);
                Debug.Log("min Angle reached");
            }
            if (cam.transform.rotation.eulerAngles.x > maxTurnAngle)
            {
                cam.transform.RotateAround(transform.position, cam.transform.right, maxTurnAngle - cam.transform.rotation.eulerAngles.x);
                Debug.Log("max Angle reached");
            }
        }
    }

    void Move()
    {
        newPosition = transform.position;

        // Moving with W,A,S,D or arrow keys
        newPosition += transform.forward * Input.GetAxis("Vertical") * flySpeed;
        newPosition += transform.right * Input.GetAxis("Horizontal") * flySpeed;

        // Moving if mouse is near to the edge of the game window
        if (!Input.GetMouseButton(1))
        {
            if (Input.mousePosition.x < 5)
                newPosition -= transform.right * flySpeed * Mathf.Clamp((5f - Input.mousePosition.x) / 5f, 0f, 1f);
            if (Input.mousePosition.x > Screen.width - 5)
                newPosition += transform.right * flySpeed * Mathf.Clamp((5f + Input.mousePosition.x - Screen.width) / 5f, 0f, 1f);
            if (Input.mousePosition.y < 5)
                newPosition -= transform.forward * flySpeed * Mathf.Clamp((5f - Input.mousePosition.y) / 5f, 0f, 1f);
            if (Input.mousePosition.y > Screen.height - 5)
                newPosition += transform.forward * flySpeed * Mathf.Clamp((5f + Input.mousePosition.y - Screen.height) / 5f, 0f, 1f);
        }

        // Keeping Position inside the Borders
        newPosition.x = Mathf.Clamp(newPosition.x, Mathf.Min(border1.x, border2.x), Mathf.Max(border1.x, border2.x));
        newPosition.y = Mathf.Clamp(newPosition.y, Mathf.Min(border1.y, border2.y), Mathf.Max(border1.y, border2.y));
        newPosition.z = Mathf.Clamp(newPosition.z, Mathf.Min(border1.z, border2.z), Mathf.Max(border1.z, border2.z));
        transform.position = newPosition;
    }

    // Zooming with the mousewheel betwenn maxZoom and minZoom
    void Zoom()
    {
        camDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        camDistance = Mathf.Clamp(camDistance, minZoom, maxZoom);
        cam.transform.localPosition = transform.InverseTransformPoint(cam.transform.position).normalized * camDistance;
    }

    // Visualtion of border
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(border1, 2);
        Gizmos.DrawWireSphere(border2, 2);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(border1, new Vector3(border1.x, border1.y, border2.z));
        Gizmos.DrawLine(border1, new Vector3(border1.x, border2.y, border1.z));
        Gizmos.DrawLine(border1, new Vector3(border2.x, border1.y, border1.z));
        Gizmos.DrawLine(new Vector3(border1.x, border2.y, border1.z), new Vector3(border1.x, border2.y, border2.z));
        Gizmos.DrawLine(new Vector3(border1.x, border2.y, border1.z), new Vector3(border2.x, border2.y, border1.z));
        Gizmos.DrawLine(new Vector3(border2.x, border1.y, border1.z), new Vector3(border2.x, border2.y, border1.z));
        Gizmos.DrawLine(border2, new Vector3(border2.x, border2.y, border1.z));
        Gizmos.DrawLine(border2, new Vector3(border2.x, border1.y, border2.z));
        Gizmos.DrawLine(border2, new Vector3(border1.x, border2.y, border2.z));
        Gizmos.DrawLine(new Vector3(border2.x, border1.y, border2.z), new Vector3(border2.x, border1.y, border1.z));
        Gizmos.DrawLine(new Vector3(border2.x, border1.y, border2.z), new Vector3(border1.x, border1.y, border2.z));
        Gizmos.DrawLine(new Vector3(border1.x, border2.y, border2.z), new Vector3(border1.x, border1.y, border2.z));
    }
}
