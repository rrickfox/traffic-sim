using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMPro.TMP_Dropdown resolutionDropdown;

    Resolution[] _resolutions;

    public void Start()
    {
        // get available Resolutions
        Resolution[] allResolutions = Screen.resolutions;
        List<Resolution> resolutions = new List<Resolution>();
        foreach(Resolution r in allResolutions)
        {
            if (resolutions.Count == 0)
                resolutions.Add(r);
            else if (r.width != resolutions[resolutions.Count - 1].width && r.height != resolutions[resolutions.Count - 1].height)
                resolutions.Add(r);
        }
        _resolutions = resolutions.ToArray();
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        // update ResolutionDropdown
        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);

            if (_resolutions[i].Equals(Screen.currentResolution)){
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // load scene
    public void LoadScene(int sceneIndex)
    {
		SceneManager.LoadScene(sceneIndex);
	}

    // set graphics Quality
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // toggle Fullscreenmode
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }


    // set Resolution
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
	
    // quit Application
	public void Quit()
	{
		Application.Quit();
	}
}
