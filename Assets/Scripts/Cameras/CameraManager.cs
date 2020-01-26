using System.Collections.Generic;
using UnityEngine;

namespace Cameras
{
    public class CameraManager : MonoBehaviour
    {
        public List<Camera> cams;

        int _activeCamIndex;
        int _newCamIndex;

        void Start()
        {
            _activeCamIndex = 0;
            _newCamIndex = _activeCamIndex;
            ChangeCam();
        }

        // Switching Cameras with Q for previous and E for next
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (_activeCamIndex == 0)
                    _newCamIndex = cams.Count - 1;
                else
                    _newCamIndex--;
                ChangeCam();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_activeCamIndex == cams.Count - 1)
                    _newCamIndex = 0;
                else
                    _newCamIndex++;
                ChangeCam();
            }
        }

        // Set active Camera, disable all others
        void ChangeCam()
        {
            cams[_activeCamIndex].enabled = false;
            cams[_newCamIndex].enabled = true;
            _activeCamIndex = _newCamIndex;
        }
    }
}