using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static bool pause;

    public void SetSimulationTime(bool newTime)
    {
        pause = newTime;
    }
}
