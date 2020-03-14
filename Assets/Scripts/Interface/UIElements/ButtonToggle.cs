using UnityEngine;

namespace Interface {
    // class to switch between two images for pause and play
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
