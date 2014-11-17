using System.IO;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	public abstract class Player : MonoBehaviour {
		
		public AudioPlayer audioPlayer;
		
		public AudioGeneralSettings generalSettings;
		public AudioHierarchyManager hierarchyManager;
		public AudioInfoManager infoManager;
		public AudioContainerManager containerManager;
		
		public Metronome metronome;
		public CoroutineHolder coroutineHolder;
		public AudioListener listener;
		
		public bool initialized;
		
		protected virtual void Awake() {
			if (Application.isPlaying) {
				audioPlayer = gameObject.GetOrAddComponent<AudioPlayer>();
				
				generalSettings = audioPlayer.generalSettings;
				hierarchyManager = audioPlayer.hierarchyManager;
				infoManager = audioPlayer.infoManager;
				containerManager = audioPlayer.containerManager;
				
				metronome = gameObject.GetOrAddComponent<Metronome>();
				coroutineHolder = gameObject.GetOrAddComponent<CoroutineHolder>();
				
				SetAudioListener();
				if (generalSettings.persistent) {
					Object.DontDestroyOnLoad(gameObject);
				}
			}
		}

		protected virtual void OnLevelWasLoaded(int level) {
			SetAudioListener();
		}

		protected virtual void SingletonCheck(Player instance) {
			if (instance != null && instance != this) {
				if (!Application.isPlaying) {
					Debug.LogError(string.Format("There can only be one {0}.", GetType().Name));
				}
				gameObject.Remove();
			}
		}
		
		protected virtual void SetAudioListener() {
			listener = FindObjectOfType<AudioListener>();
			if (listener == null) {
				GameObject newListener = new GameObject("Listener");
				listener = newListener.AddComponent<AudioListener>();
				listener.transform.Reset();
				Debug.LogWarning("No listener was found in the scene. One was automatically created.");
			}
		}
	}
}
