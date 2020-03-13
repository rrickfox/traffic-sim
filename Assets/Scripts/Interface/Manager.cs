﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using Saves;

public class Manager : MonoBehaviour
{
    public static bool pause;
    public static bool menu;

    public SaveLoader saveLoader;
    public GameObject simulationCanvas;
    public GameObject menuCanvas;

    private void Start()
    {
        menuCanvas.SetActive(true);
        simulationCanvas.SetActive(false);
        menu = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && SaveLoader.paths != null)
        {
            ToggleMenu();
        }
    }

    public void SetSimulationTime(bool newTime)
    {
        pause = newTime;
    }

    public void SwitchToMenu()
    {
        simulationCanvas.SetActive(false);
        menuCanvas.SetActive(true);
        menu = true;
    }

    public void SwitchToSimulation()
    {
        simulationCanvas.SetActive(true);
        menuCanvas.SetActive(false);
        menu = false;
    }

    public void ToggleMenu()
    {
        if (menu)
            SwitchToSimulation();
        else
            SwitchToMenu();
    }
}
