using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interface
{
    public class SimulationUI : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetAxis("Cancel") != 0)
                LoadMenu();
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
