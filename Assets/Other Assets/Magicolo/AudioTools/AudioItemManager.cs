using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioItemManager {
		
		public static List<SingleAudioItem> activeSingleAudioItems = new List<SingleAudioItem>();
		public static List<MultipleAudioItem> activeMultipleAudioItems = new List<MultipleAudioItem>();
		public static List<SingleAudioItem> inactiveSingleAudioItems = new List<SingleAudioItem>();
		public static List<MultipleAudioItem> inactiveMultipleAudioItems = new List<MultipleAudioItem>();
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
		}
		
		public virtual void Update() {
			foreach (SingleAudioItem audioItem in activeSingleAudioItems.ToArray()) {
				audioItem.Update();
			}
			foreach (MultipleAudioItem audioItem in activeMultipleAudioItems.ToArray()) {
				audioItem.Update();
			}
			foreach (SingleAudioItem audioItem in inactiveSingleAudioItems.ToArray()) {
				audioItem.Update();
			}
			foreach (MultipleAudioItem audioItem in inactiveMultipleAudioItems.ToArray()) {
				audioItem.Update();
			}
		}
		
		public virtual void Activate(SingleAudioItem audioItem) {
			inactiveSingleAudioItems.Remove(audioItem);
			activeSingleAudioItems.Add(audioItem);
			activeAudioObjects.Remove(audioItem.gameObject);
			LimitVoices();
		}
		
		public virtual void Activate(MultipleAudioItem audioItem) {
			inactiveMultipleAudioItems.Remove(audioItem);
			activeMultipleAudioItems.Add(audioItem);
		}

		public bool IsActive(SingleAudioItem audioItem) {
			return activeSingleAudioItems.Contains(audioItem);
		}
		
		public bool IsActive(MultipleAudioItem audioItem) {
			return activeMultipleAudioItems.Contains(audioItem);
		}
		
		public bool IsActive(GameObject gameObject) {
			return activeAudioObjects.Contains(gameObject);
		}
		
		public virtual void Deactivate(SingleAudioItem audioItem) {
			activeSingleAudioItems.Remove(audioItem);
			activeAudioObjects.Remove(audioItem.gameObject);
			inactiveSingleAudioItems.Remove(audioItem);
			inactiveAudioObjects.Add(audioItem.gameObject);
			audioItem.gameObject.transform.parent = player.transform;
			audioItem.gameObject.SetActive(false);
			audioItem.audioSource.clip = null;
		}
		
		public virtual void Deactivate(MultipleAudioItem audioItem) {
			activeMultipleAudioItems.Remove(audioItem);
			inactiveMultipleAudioItems.Remove(audioItem);
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
				
		public virtual SingleAudioItem GetSingleAudioItem(string soundName, GameObject source) {
			AudioInfo audioInfo = infoManager.GetAudioInfo(soundName);
			AudioSource audioSource = GetAudioSource(audioInfo, source);
			CoroutineHolder coroutineHolder = audioSource.GetOrAddComponent<CoroutineHolder>();
			GainManager gainManager = audioSource.GetOrAddComponent<GainManager>();
			
			idCounter += 1;
			SingleAudioItem audioItem = new SingleAudioItem(audioInfo.Name, idCounter, audioSource, audioInfo, coroutineHolder, gainManager, this, player);
			
			gainManager.Initialize(source, audioItem, player);
			audioItem.Update();
			inactiveSingleAudioItems.Add(audioItem);
			return audioItem;
		}
			
		public virtual AudioSource GetAudioSource(AudioInfo audioInfo, GameObject source) {
			GameObject gameObject = GetGameObject(source);
			return SetAudioSource(gameObject.GetOrAddComponent<AudioSource>(), audioInfo);
		}
		
		public virtual AudioSource SetAudioSource(AudioSource audioSource, AudioInfo audioInfo) {
			audioSource.Copy(audioInfo.Source);
			audioSource.clip = audioInfo.Clip;
			audioSource.volume += Random.Range(-audioInfo.randomVolume, audioInfo.randomVolume);
			audioSource.pitch += Random.Range(-audioInfo.randomPitch, audioInfo.randomPitch);
			return audioSource;
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
