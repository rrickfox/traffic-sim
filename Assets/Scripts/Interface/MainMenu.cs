using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interface
{
    public class MainMenu : MonoBehaviour
    {
        public TMPro.TMP_Dropdown resolutionDropdown;

        private Resolution[] _resolutions;

        public void Start()
        {
            // get available Resolutions
            var allResolutions = Screen.resolutions;
            var resolutions = new List<Resolution>();
            foreach(var r in allResolutions)
            {
                if (resolutions.Count == 0)
                    resolutions.Add(r);
                else if (r.width != resolutions[resolutions.Count - 1].width && r.height != resolutions[resolutions.Count - 1].height)
                    resolutions.Add(r);
            }
            _resolutions = resolutions.ToArray();
            resolutionDropdown.ClearOptions();
            var options = new List<string>();

            // update ResolutionDropdown
            var currentResolutionIndex = 0;
            for (var i = 0; i < _resolutions.Length; i++)
            {
                var option = _resolutions[i].width + " x " + _resolutions[i].height;
                options.Add(option);

                if (_resolutions[i].Equals(Screen.currentResolution))
                    currentResolutionIndex = i;
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        // load scene
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }

        // set graphics Quality
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        // toggle Fullscreen mode
        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }


        // set Resolution
        public void SetResolution(int resolutionIndex)
        {
            var resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
	
        // quit Application
        public void Quit()
        {
            Application.Quit();
        }
    }
}
