using UnityEngine;

namespace Cameras
{
    public class BirdCamera : MonoBehaviour
    {
        public float height;

        Camera _camera;

        void Start()
        {
            _camera = GetComponent<Camera>();
        }

        void Update()
        {
            if (!_camera.enabled)
                return;
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }
    }
}