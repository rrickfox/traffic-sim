using UnityEngine;

namespace Cameras
{
    public class BirdCamera : MonoBehaviour
    {
        public float height;

        void Start()
        {

        }

        void Update()
        {
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }
    }
}