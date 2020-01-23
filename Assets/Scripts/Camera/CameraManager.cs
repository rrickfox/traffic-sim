using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public List<Camera> cams;

    int activeCam;

    void Start()
    {
        activeCam = 0;
        ChangeCam();
    }

    // Switching Cameras with Q for previous and E for next
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (activeCam == 0) activeCam = cams.Count-1;
            else activeCam--;
            ChangeCam();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (activeCam == cams.Count-1) activeCam = 0;
            else activeCam++;
            ChangeCam();
        }
    }

    // Set active Camera, disable all others
    void ChangeCam()
    {
        for (int i = 0; i < cams.Count; i++)
        {
            if (i == activeCam)
                cams[i].enabled = true;
            else
                cams[i].enabled = false;
        }
    }
}
