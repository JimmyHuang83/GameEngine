using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameEngine
{
	public class AudioManager : MonoBehaviour {

		public AudioClip bgmusic;
		// Use this for initialization
		AudioSource bgAudioSource;
		void Start () 
		{
			bgAudioSource = PlayAudio (bgmusic);
			if (bgAudioSource) {
				bgAudioSource.loop = true;
				bgAudioSource.volume = soundEnabled ? 0.5f : 0f;
			}
		}
		
		// Update is called once per frame
		void Update () {
			
		}
		bool _soundEnabled = true;
		public bool soundEnabled
		{
			set 
			{
				_soundEnabled = value;
				if (bgAudioSource != null)
					bgAudioSource.volume = _soundEnabled ? 0.5f : 0f;
				PlayerPrefs.SetInt("SoundONOFF", _soundEnabled ? 0 : 1);
			}
			get 
			{
				_soundEnabled = PlayerPrefs.GetInt("SoundONOFF",0)==0;
				return _soundEnabled;
			}
		}
		
		private List<AudioSource> audioSourceList = new List<AudioSource>();
		static AudioManager __Instance;
		
		void Awake()
		{
			for(int i = 0; i < 20; i++)
			{
				AudioSource audioSource = gameObject.AddComponent<AudioSource> ();
				audioSourceList.Add(audioSource);
			}
			DontDestroyOnLoad (gameObject);
		}
		
		public AudioSource PlayAudio(AudioClip audios)
		{
			if (audios == null)
				return null;
			if (soundEnabled == false)
				return null;
			foreach(AudioSource ass in audioSourceList)
			{
				if (ass.clip != null && ass.isPlaying)
					continue;
				ass.clip = audios;
				ass.Play ();
				return ass;
			}
			return null;
		}

		public void StopAudio(AudioClip audios)
		{
			if (audios == null)
				return;
			foreach(AudioSource ass in audioSourceList)
			{
				if (ass.clip != null && ass.clip == audios)
				{
					ass.Stop();
				}
			}
		}
		
		public static AudioManager Instance
		{
			get
			{
				if (__Instance == null)
				{
					GameObject go = new GameObject("AudioManager");
					GameObject.DontDestroyOnLoad(go);
					__Instance = go.AddComponent<AudioManager>(); 
				}
				return __Instance;
			}
		}
	}
}