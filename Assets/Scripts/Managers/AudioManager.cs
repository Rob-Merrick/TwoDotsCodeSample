using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Manager<AudioManager>
{
	[SerializeField] private AudioInfo[] _audioInfo;

	private readonly IDictionary<string, AudioSource> _audioInfoLookup = new Dictionary<string, AudioSource>();

	protected override void OnAwake()
	{
		CreateAudioInfoLookup();
	}

	public void PlaySound(string soundName, bool restartIfAlreadyPlaying = false, float pitch = 1.0F)
	{
		pitch = Mathf.Clamp(pitch, 0.0F, 3.0F);
		AudioSource audioSource = _audioInfoLookup[soundName];

		if(restartIfAlreadyPlaying || !audioSource.isPlaying)
		{
			audioSource.pitch = pitch;
			audioSource.Play();
		}
	}

	public void PlaySoundOneShot(string soundName, float pitch = 1.0F)
	{
		pitch = Mathf.Clamp(pitch, 0.0F, 3.0F);
		AudioSource audioSource = _audioInfoLookup[soundName];
		audioSource.pitch = pitch;
		audioSource.PlayOneShot(audioSource.clip);
	}

	public void Stop(AudioSource sound)
	{
		StartCoroutine(QuickFadeOut(sound));
	}

	private void CreateAudioInfoLookup()
	{
		_audioInfoLookup.Clear();

		foreach(AudioInfo audioInfo in _audioInfo)
		{
			_audioInfoLookup.Add(audioInfo.Name, audioInfo.AudioSource);
		}
	}

	private IEnumerator QuickFadeOut(AudioSource sound)
	{
		float totalTimeTaken = 0.0F;
		float timeToFade = 0.2F;
		float originalVolume = sound.volume;

		while(totalTimeTaken < timeToFade)
		{
			sound.volume = Mathf.Lerp(originalVolume, 0.0F, totalTimeTaken / timeToFade);
			yield return null;
			totalTimeTaken += Time.deltaTime;
		}

		sound.volume = originalVolume;
		sound.Stop();
	}
}
