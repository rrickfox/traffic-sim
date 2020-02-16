using UnityEngine;

namespace Cameras
{
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
        [Header("Parent")]
        public Transform parent;

        Vector3 _newPosition;
        Camera _cam;
        float _camDistance = 50f;
        float _scroll;

        // Setting camera right, focus the center
        void Start()
        {
            _cam = Camera.main;
            _cam.transform.LookAt(transform.position);
        }

        private void Update()
        {
            SelectCar();
            _scroll = Input.GetAxis("Mouse ScrollWheel");
        }

        void FixedUpdate()
        {
            if (!_cam.enabled)
                return;
            Rotate();
            Move();
            Zoom();
        }

        // Rotating Camera around center while right mouse button is pressed, up-down-rotation (X-axis) can be limited
        void Rotate()
        {
            if (Input.GetMouseButton(1))
            {
                transform.Rotate(0f, Input.GetAxis("Mouse X") * turnSpeed, 0f);
                if (_cam.transform.rotation.eulerAngles.x <= maxTurnAngle && _cam.transform.rotation.eulerAngles.x >= minTurnAngle)
                    _cam.transform.RotateAround(transform.position, _cam.transform.right, -1 * Input.GetAxis("Mouse Y") * turnSpeed);
                if (_cam.transform.rotation.eulerAngles.x < minTurnAngle)
                    _cam.transform.RotateAround(transform.position, _cam.transform.right, minTurnAngle - _cam.transform.rotation.eulerAngles.x);
                if (_cam.transform.rotation.eulerAngles.x > maxTurnAngle)
                    _cam.transform.RotateAround(transform.position, _cam.transform.right, maxTurnAngle - _cam.transform.rotation.eulerAngles.x);
            }
        }

        void Move()
        {
            _newPosition = transform.position;

            // Moving with W,A,S,D or arrow keys
            _newPosition += transform.forward * Input.GetAxis("Vertical") * flySpeed;
            _newPosition += transform.right * Input.GetAxis("Horizontal") * flySpeed;

            if (_newPosition != transform.position)
                transform.SetParent(parent);

            // Moving if mouse is near to the edge of the game window
            if (!Input.GetMouseButton(1))
            {
                if (Input.mousePosition.x < 5)
                    _newPosition -= transform.right * flySpeed * Mathf.Clamp((5f - Input.mousePosition.x) / 5f, 0f, 1f);
                if (Input.mousePosition.x > Screen.width - 5)
                    _newPosition += transform.right * flySpeed * Mathf.Clamp((5f + Input.mousePosition.x - Screen.width) / 5f, 0f, 1f);
                if (Input.mousePosition.y < 5)
                    _newPosition -= transform.forward * flySpeed * Mathf.Clamp((5f - Input.mousePosition.y) / 5f, 0f, 1f);
                if (Input.mousePosition.y > Screen.height - 5)
                    _newPosition += transform.forward * flySpeed * Mathf.Clamp((5f + Input.mousePosition.y - Screen.height) / 5f, 0f, 1f);
            }

            // Keeping Position inside the Borders
            _newPosition.x = Mathf.Clamp(_newPosition.x, Mathf.Min(border1.x, border2.x), Mathf.Max(border1.x, border2.x));
            _newPosition.y = Mathf.Clamp(_newPosition.y, Mathf.Min(border1.y, border2.y), Mathf.Max(border1.y, border2.y));
            _newPosition.z = Mathf.Clamp(_newPosition.z, Mathf.Min(border1.z, border2.z), Mathf.Max(border1.z, border2.z));
            transform.position = _newPosition;
        }

        // Zooming with the mousewheel between maxZoom and minZoom
        void Zoom()
        {
            _camDistance -= _scroll * zoomSpeed;
            _camDistance = Mathf.Clamp(_camDistance, minZoom, maxZoom);
            _cam.transform.localPosition = transform.InverseTransformPoint(_cam.transform.position).normalized * _camDistance;
        }

        // selects a Car with left mouse button to follow
        void SelectCar()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // shoots a ray to get a car located at the mousePosition
                transform.SetParent(
                    Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition),
                                    out var hit,
                                    200f,
                                    LayerMask.GetMask("Cars"))
                        ? hit.transform : parent
                );
            }
        }

        // Visualization of border
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
}