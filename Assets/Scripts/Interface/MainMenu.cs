using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void StartSimulation()
	{
		// load simulation
		SceneManager.LoadSceneAsync(1);
	}
	
	public void Quit()
	{
		Application.Quit();
	}
}
