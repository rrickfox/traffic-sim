using Interface;
using UnityEngine;
using Saves;
using UnityEngine.EventSystems;

namespace Cameras
{
    public class FreeCamera : MonoBehaviour
    {
        [Header("Borders")]
        public Vector3 border1;
        public Vector3 border2;

        public bool _following;
        public Transform _targetCar;
        Vector3 _newPosition;
        Camera _cam;
        float _camDistance = 50f;

        // Setting camera right, focus the center
        void Start()
        {
            _cam = GetComponentInChildren<Camera>(true);
            _cam.transform.LookAt(transform.position);
        }

        private void Update()
        {
            if (SimulationManager.menu)
                return;


            if (!_cam.enabled)
            {
                // reset camera
                if (_following)
                {
                    _following = false;
                    _targetCar = null;
                }
                return;
            }

            SelectCar();
            Follow();
            Rotate();
            Move();
            Zoom();
        }

        // Rotating Camera around center while right mouse button is pressed, up-down-rotation (X-axis) can be limited
        void Rotate()
        {
            if (Input.GetMouseButton(1))
            {
                transform.Rotate(0f, Input.GetAxis("Mouse X") * UserSettings.turnSpeed * Time.deltaTime, 0f);
                var targetRotation = _cam.transform.rotation.eulerAngles.x + -1 * Input.GetAxis("Mouse Y") * UserSettings.turnSpeed * Time.deltaTime;
                targetRotation = Mathf.Clamp(targetRotation, UserSettings.minTurnAngle, UserSettings.maxTurnAngle);
                _cam.transform.RotateAround(transform.position, _cam.transform.right, targetRotation - _cam.transform.rotation.eulerAngles.x);
            }
        }

        void Move()
        {
            _newPosition = transform.position;

            // Moving with W,A,S,D or arrow keys
            _newPosition += transform.forward * Input.GetAxis("Vertical") * UserSettings.flySpeed * Time.deltaTime;
            _newPosition += transform.right * Input.GetAxis("Horizontal") * UserSettings.flySpeed * Time.deltaTime;

            // Moving if mouse is near to the edge of the game window
            if (!Input.GetMouseButton(1) && UserSettings.moveOnEdges)
            {
                if (Input.mousePosition.x < 5)
                    _newPosition -= transform.right * UserSettings.flySpeed * Mathf.Clamp((5f - Input.mousePosition.x) / 5f, 0f, 1f);
                if (Input.mousePosition.x > Screen.width - 5)
                    _newPosition += transform.right * UserSettings.flySpeed * Mathf.Clamp((5f + Input.mousePosition.x - Screen.width) / 5f, 0f, 1f);
                if (Input.mousePosition.y < 5)
                    _newPosition -= transform.forward * UserSettings.flySpeed * Mathf.Clamp((5f - Input.mousePosition.y) / 5f, 0f, 1f);
                if (Input.mousePosition.y > Screen.height - 5)
                    _newPosition += transform.forward * UserSettings.flySpeed * Mathf.Clamp((5f + Input.mousePosition.y - Screen.height) / 5f, 0f, 1f);
            }

            // Keeping Position inside the Borders
            _newPosition.x = Mathf.Clamp(_newPosition.x, Mathf.Min(border1.x, border2.x), Mathf.Max(border1.x, border2.x));
            _newPosition.y = Mathf.Clamp(_newPosition.y, Mathf.Min(border1.y, border2.y), Mathf.Max(border1.y, border2.y));
            _newPosition.z = Mathf.Clamp(_newPosition.z, Mathf.Min(border1.z, border2.z), Mathf.Max(border1.z, border2.z));

            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                _following = false;
            }
            transform.position = _newPosition;
        }

        // Zooming with the mousewheel between UserSettings.maxZoom and UserSettings.minZoom
        void Zoom()
        {
            _camDistance -= Input.GetAxis("Mouse ScrollWheel") * UserSettings.zoomSpeed;
            _camDistance = Mathf.Clamp(_camDistance, UserSettings.minZoom, UserSettings.maxZoom);
            _cam.transform.localPosition = transform.InverseTransformPoint(_cam.transform.position).normalized * _camDistance;
        }

        // follows a target Car
        void Follow()
        {
            if (_targetCar != null && _following)
            {
                transform.position = _targetCar.position;
            }
            else if (_following)
                _following = false;
        }

        // selects a Car with left mouse button to follow
        void SelectCar()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // checks if UI is clicked
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                _following = false;
                // shoots a ray to get a car located at the mousePosition
                if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out var hit, 500f, LayerMask.GetMask("Cars")))
                {
                    _following = true;
                    _targetCar = hit.transform;
                }
            }
        }

        // Visualisation of border
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