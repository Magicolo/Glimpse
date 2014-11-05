using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

// TODO Add tempo sync events

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioItemManager : ITickable {
		
		public static List<SingleAudioItem> activeSingleAudioItems = new List<SingleAudioItem>();
		public static List<MultipleAudioItem> activeMultipleAudioItems = new List<MultipleAudioItem>();
		public static List<AudioItem> inactiveAudioItems = new List<AudioItem>();
		public static List<GameObject> activeAudioObjects = new List<GameObject>();
		public static List<GameObject> inactiveAudioObjects = new List<GameObject>();
		public static int idCounter;
		
		public AudioListener listener;
		public AudioInfoManager infoManager;
		public Magicolo.AudioTools.Player player;

		public AudioItemManager(AudioListener listener, AudioInfoManager infoManager, Magicolo.AudioTools.Player player) {
			this.listener = listener;
			this.infoManager = infoManager;
			this.player = player;
			
			player.metronome.Subscribe(this);
		}
		
		public virtual void Update() {
			foreach (AudioItem audioItem in activeSingleAudioItems.ToArray()) {
				audioItem.Update();
			}
			foreach (AudioItem audioItem in activeMultipleAudioItems.ToArray()) {
				audioItem.Update();
			}
			foreach (AudioItem audioItem in inactiveAudioItems.ToArray()) {
				audioItem.Update();
			}
		}
		
		public virtual void Activate(SingleAudioItem audioItem) {
			inactiveAudioItems.Remove(audioItem);
			activeSingleAudioItems.Add(audioItem);
			activeAudioObjects.Remove(audioItem.gameObject);
			LimitVoices();
		}
		
		public virtual void Activate(MultipleAudioItem audioItem) {
			inactiveAudioItems.Remove(audioItem);
			activeMultipleAudioItems.Add(audioItem);
		}
		
		public virtual void Deactivate(SingleAudioItem audioItem) {
			activeSingleAudioItems.Remove(audioItem);
			activeAudioObjects.Remove(audioItem.gameObject);
			inactiveAudioObjects.Add(audioItem.gameObject);
			audioItem.gameObject.transform.parent = player.transform;
			audioItem.gameObject.SetActive(false);
			Resources.UnloadAsset(audioItem.audioSource.clip);
		}
		
		public virtual void Deactivate(MultipleAudioItem audioItem) {
			activeMultipleAudioItems.Remove(audioItem);
		}
		
		public virtual void SetMasterVolume(float targetVolume, float time) {
			player.coroutineHolder.RemoveCoroutines("FadeMasterVolume");
			player.coroutineHolder.AddCoroutine("FadeMasterVolume", FadeMasterVolume(player.generalSettings.MasterVolume, targetVolume, time));
		}
		
		public virtual void SetMasterVolume(float targetVolume) {
			player.coroutineHolder.RemoveCoroutines("FadeMasterVolume");
			player.generalSettings.MasterVolume = targetVolume;
		}
		
		public virtual void LimitVoices() {
			if (activeSingleAudioItems.Count > player.generalSettings.maxVoices) {
				foreach (SingleAudioItem audioItem in activeSingleAudioItems.ToArray()) {
					if (!audioItem.audioInfo.doNotKill) {
						audioItem.StopImmediate();
						
						if (activeSingleAudioItems.Count <= player.generalSettings.maxVoices) {
							break;
						}
					}
				}
			}
		}
		
		public virtual AudioSource GetAudioSource(AudioInfo audioInfo, GameObject source) {
			GameObject gameObject = GetGameObject(source);
			return SetAudioSource(gameObject.GetOrAddComponent<AudioSource>(), audioInfo);
		}
		
		public virtual GameObject GetGameObject(GameObject source) {
			GameObject gameObject;
			
			gameObject = inactiveAudioObjects.Count == 0 ? new GameObject() : inactiveAudioObjects.Pop();
			gameObject.transform.parent = player.transform;
			gameObject.transform.position = source == null ? player.transform.position : source.transform.position;
			gameObject.SetActive(true);
			Object.DontDestroyOnLoad(gameObject);
			
			return gameObject;
		}
	
		public virtual AudioSource SetAudioSource(AudioSource audioSource, AudioInfo audioInfo) {
			audioSource.Copy(audioInfo.Source);
			audioSource.clip = audioInfo.Clip;
			audioSource.volume += Random.Range(-audioInfo.randomVolume, audioInfo.randomVolume);
			audioSource.pitch += Random.Range(-audioInfo.randomPitch, audioInfo.randomPitch);
			return audioSource;
		}
	
		public virtual void BeatEvent(int currentBeat) {
		}

		public virtual void MeasureEvent(int currentMeasure) {
		}
		
		public virtual IEnumerator FadeMasterVolume(float startVolume, float targetVolume, float time) {
			float counter = 0;
			
			while (counter < time) {
				player.generalSettings.MasterVolume = (counter / time) * (targetVolume - startVolume) + startVolume;
				counter += Time.deltaTime;
				yield return new WaitForSeconds(0);
			}
			
			player.generalSettings.MasterVolume = targetVolume;
		}
	}
}
