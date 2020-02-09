﻿using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    private AudioSource _audioData;

    void Start()
    {
		Object.DontDestroyOnLoad(gameObject);
        _audioData = GetComponent<AudioSource>();
		_audioData.volume = 0;
        _audioData.Play(0);
    }
	
	public void SetVolume(float volume)
	{
		_audioData.volume = volume;
	}
}