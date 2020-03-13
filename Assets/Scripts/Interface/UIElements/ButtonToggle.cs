using UnityEngine;

namespace Interface {
    public class ButtonToggle : MonoBehaviour
    {
        public GameObject playImage;
        public GameObject pauseImage;

        private void Start()
        {
            playImage.SetActive(false);
            pauseImage.SetActive(true);
        }

        public void ToggleState()
        {
            SimulationManager.pause = !SimulationManager.pause;
            playImage.SetActive(SimulationManager.pause);
            pauseImage.SetActive(!SimulationManager.pause);
        }
    }
}
