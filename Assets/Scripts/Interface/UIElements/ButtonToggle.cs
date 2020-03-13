using UnityEngine;

namespace Interface {
    public class ButtonToggle : MonoBehaviour
    {
        public GameObject playImage;
        public GameObject pauseImage;

        public void ToggleState()
        {
            SimulationManager.pause = !SimulationManager.pause;
            playImage.SetActive(SimulationManager.pause);
            pauseImage.SetActive(!SimulationManager.pause);
        }
    }
}
