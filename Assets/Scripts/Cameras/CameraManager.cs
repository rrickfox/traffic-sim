using System.Collections.Generic;
using UnityEngine;

namespace Cameras
{
    public class CameraManager : MonoBehaviour
    {
        public List<Camera> cams;

        int _activeCam;

        void Start()
        {
            _activeCam = 0;
            ChangeCam();
        }

        // Switching Cameras with Q for previous and E for next
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (_activeCam == 0) _activeCam = cams.Count - 1;

                else _activeCam--;
                ChangeCam();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_activeCam == cams.Count - 1) _activeCam = 0;
                else _activeCam++;
                ChangeCam();
            }
        }

        // Set active Camera, disable all others
        void ChangeCam()
        {
            int i = 0;
            foreach (Camera cam in cams)
            {
                if (i == _activeCam)
                    cam.enabled = true;
                else
                    cam.enabled = false;
            }
        }
    }
}