using UnityEngine;

namespace Music
{
	[RequireComponent(typeof(AudioSource))]
	public class MusicController : MonoBehaviour
	{
		private AudioSource _audioData;

		private void Start()
		{
			_audioData = GetComponent<AudioSource>();
			_audioData.volume = 0;
			_audioData.Play(0);
		}
	
		public void SetVolume(float volume)
		{
			_audioData.volume = volume;
		}
	}
}