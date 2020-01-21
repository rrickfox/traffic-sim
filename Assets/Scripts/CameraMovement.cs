using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float flySpeed = 0.1f;
    public float turnSpeed = 1f;
    public float maxTurnAngle = 80f;
    public float minTurnAngle = 20f;
    public float minZoom = 10f;
    public float maxZoom = 20f;
    public float zoomSpeed = 10f;

    Transform cam;
    float camDistance = 5f;

    // Setting camera right, focus the center
    void Start()
    {
        cam = Camera.main.transform;
        cam.LookAt(transform.position);
    }

    void FixedUpdate()
    {
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
            if (cam.rotation.eulerAngles.x <= maxTurnAngle && cam.rotation.eulerAngles.x >= minTurnAngle)
                cam.RotateAround(transform.position, cam.right, -1 * Input.GetAxis("Mouse Y") * turnSpeed);
            if (cam.rotation.eulerAngles.x < minTurnAngle)
                cam.RotateAround(transform.position, cam.right, minTurnAngle - cam.rotation.eulerAngles.x);
            if (cam.rotation.eulerAngles.x > maxTurnAngle)
                cam.RotateAround(transform.position, cam.right, maxTurnAngle - cam.rotation.eulerAngles.x);
        }
    }

    void Move()
    {
        // Moving with W,A,S,D or arrow keys
        transform.position += transform.forward * Input.GetAxis("Vertical") * flySpeed;
        transform.position += transform.right * Input.GetAxis("Horizontal") * flySpeed;

        // Moving if mouse is near to the edge of the game window
        if (Input.GetMouseButton(1)) return;
        if (Input.mousePosition.x < 5)
            transform.position -= transform.right * flySpeed * (5f - Input.mousePosition.x) / 5f;
        if (Input.mousePosition.x > Screen.width - 5)
            transform.position += transform.right * flySpeed * (5f + Input.mousePosition.x - Screen.width) / 5f;
        if (Input.mousePosition.y < 5)
            transform.position -= transform.forward * flySpeed * (5f - Input.mousePosition.y) / 5f;
        if (Input.mousePosition.y > Screen.height - 5)
            transform.position += transform.forward * flySpeed * (5f + Input.mousePosition.y - Screen.height) / 5f;
    }

    // Zooming with the mousewheel betwenn maxZoom and minZoom
    void Zoom()
    {
        camDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        camDistance = Mathf.Clamp(camDistance, minZoom, maxZoom);
        cam.localPosition = transform.InverseTransformPoint(cam.position).normalized * camDistance;
    }
}
