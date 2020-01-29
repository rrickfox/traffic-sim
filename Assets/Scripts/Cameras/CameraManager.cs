using System.Collections.Generic;
using UnityEngine;

namespace Cameras
{
    public class CameraManager : MonoBehaviour
    {
        public List<Camera> cams;

        int _activeCamIndex;

        void Start()
        {
            ChangeCam(0);
        }

        // Switching Cameras with Q for previous and E for next
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (_activeCamIndex == 0)
                    ChangeCam(cams.Count - 1);
                else
                    ChangeCam(_activeCamIndex - 1);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_activeCamIndex == cams.Count - 1)
                    ChangeCam(0);
                else
                    ChangeCam(_activeCamIndex + 1);
            }
        }

        // Set active Camera, disable all others
        void ChangeCam(int newCamIndex)
        {
            cams[_activeCamIndex].enabled = false;
            cams[newCamIndex].enabled = true;
            _activeCamIndex = newCamIndex;
        }
    }
}