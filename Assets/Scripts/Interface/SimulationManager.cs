using Saves;
using UnityEngine;

namespace Interface
{
    public class SimulationManager : MonoBehaviour
    {
        public static bool pause;
        public static bool menu;

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

        // pause or unpause the simulation
        public void SetSimulationTime(bool newTime)
        {
            pause = newTime;
        }

        // show menu, deactivate ingame UI
        public void SwitchToMenu()
        {
            simulationCanvas.SetActive(false);
            menuCanvas.SetActive(true);
            menu = true;
        }

        // show ingame UI, deactivate menu
        public void SwitchToSimulation()
        {
            simulationCanvas.SetActive(true);
            menuCanvas.SetActive(false);
            menu = false;
        }

        // toggle from the actual shown UI/menu
        public void ToggleMenu()
        {
            if (menu)
                SwitchToSimulation();
            else
                SwitchToMenu();
        }
    }
}
