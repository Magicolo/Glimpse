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
			for (int i = 0; i < activeSingleAudioItems.Count; i++) {
				activeSingleAudioItems[i].Update();
			}
			for (int i = 0; i < activeMultipleAudioItems.Count; i++) {
				activeMultipleAudioItems[i].Update();
			}
			for (int i = 0; i < inactiveSingleAudioItems.Count; i++) {
				inactiveSingleAudioItems[i].Update();
			}
			for (int i = 0; i < inactiveMultipleAudioItems.Count; i++) {
				inactiveMultipleAudioItems[i].Update();
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
//			Object.Destroy(audioItem.gameObject);
			audioItem.gameObject.SetActive(false);
		}
		
		public virtual void Deactivate(MultipleAudioItem audioItem) {
			activeMultipleAudioItems.Remove(audioItem);
			inactiveMultipleAudioItems.Remove(audioItem);
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
				
		public virtual SingleAudioItem GetSingleAudioItem(string soundName, object source) {
			AudioInfo audioInfo = infoManager.GetAudioInfo(soundName);
			AudioSource audioSource = GetAudioSource(audioInfo, source);
			CoroutineHolder coroutineHolder = audioSource.GetOrAddComponent<CoroutineHolder>();
			GainManager gainManager = audioSource.GetOrAddComponent<GainManager>();
			
			idCounter += 1;
			SingleAudioItem audioItem = new SingleAudioItem(audioInfo.Name, idCounter, audioSource, audioInfo, coroutineHolder, gainManager, this, player);
			
			gainManager.Initialize(source, audioItem, player);
			inactiveSingleAudioItems.Add(audioItem);
			return audioItem;
		}

		public virtual MultipleAudioItem GetMultipleAudioItem(AudioContainer container, object source) {
			MultipleAudioItem multipleAudioItem;
			
			switch (container.type) {
				case AudioContainer.Types.RandomContainer:
					multipleAudioItem = GetRandomAudioItem(container, container.childrenIds, source);
					break;
				case AudioContainer.Types.SwitchContainer:
					multipleAudioItem = GetSwitchAudioItem(container, container.childrenIds, source);
					break;
				default:
					multipleAudioItem = GetMixAudioItem(container, container.childrenIds, source);
					break;
			}
			return multipleAudioItem;
		}
		
		public virtual AudioItem GetSubContainerAudioItem(AudioContainer container, AudioSubContainer subContainer, object source) {
			AudioItem audioItem = null;
			
			if (subContainer.IsSource) {
				audioItem = GetSourceAudioItem(container, subContainer, source);
			}
			else {
				audioItem = GetContainerAudioItem(container, subContainer, source);
			}
			return audioItem;
		}

		public virtual SingleAudioItem GetSourceAudioItem(AudioContainer container, AudioSubContainer subContainer, object source) {
			SingleAudioItem sourceAudioItem = null;
			
			switch (subContainer.type) {
				case AudioSubContainer.Types.Sampler:
					sourceAudioItem = player.generalSettings.Sampler.itemManager.GetSingleAudioItem(player.generalSettings.Sampler.itemManager.GetInstrument(subContainer.instrumentName), subContainer.note, subContainer.velocity, source);
					if (sourceAudioItem != null) {
						sourceAudioItem.startAudioOptions = subContainer.audioOptions;
					}
					break;
				default:
					sourceAudioItem = GetSingleAudioItem(subContainer.audioInfo.Name, source);
					if (sourceAudioItem != null) {
						sourceAudioItem.startAudioOptions = subContainer.audioOptions;
					}
					break;
			}
			return sourceAudioItem;
		}
		
		public virtual MultipleAudioItem GetContainerAudioItem(AudioContainer container, AudioSubContainer subContainer, object source) {
			MultipleAudioItem multipleAudioItem = null;
			
			switch (subContainer.type) {
				case AudioSubContainer.Types.RandomContainer:
					multipleAudioItem = GetRandomAudioItem(container, subContainer.childrenIds, source);
					break;
				case AudioSubContainer.Types.SwitchContainer:
					multipleAudioItem = GetSwitchAudioItem(container, subContainer.childrenIds, source);
					break;
				default:
					multipleAudioItem = GetMixAudioItem(container, subContainer.childrenIds, source);
					break;
			}
			return multipleAudioItem;
		}
		
		public virtual MultipleAudioItem GetMixAudioItem(AudioContainer container, List<int> childrenIds, object source) {
			idCounter += 1;
			MultipleAudioItem mixAudioItem = new MultipleAudioItem(container.Name, idCounter, this, player);
			
			foreach (int childrenId in childrenIds) {
				AudioItem childAudioItem = GetSubContainerAudioItem(container, container.GetSubContainerWithID(childrenId), source);
				if (childAudioItem != null) {
					mixAudioItem.AddAudioItem(childAudioItem);
				}
			}
			
			mixAudioItem.Update();
			inactiveMultipleAudioItems.Add(mixAudioItem);
			return mixAudioItem;
		}
		
		public virtual MultipleAudioItem GetRandomAudioItem(AudioContainer container, List<int> childrenIds, object source) {
			idCounter += 1;
			MultipleAudioItem randomAudioItem = new MultipleAudioItem(container.Name, idCounter, this, player);
			List<AudioSubContainer> childcontainers = new List<AudioSubContainer>();
			List<float> weights = new List<float>();
			
			for (int i = 0; i < childrenIds.Count; i++) {
				AudioSubContainer childContainer = container.GetSubContainerWithID(childrenIds[i]);
				if (childContainer != null) {
					childcontainers.Add(childContainer);
					weights.Add(childContainer.weight);
				}
			}
			
			AudioSubContainer randomChildContainer = HelperFunctions.WeightedRandom(childcontainers, weights);
			if (randomAudioItem != null) {
				AudioItem childAudioItem = GetSubContainerAudioItem(container, randomChildContainer, source);
				if (childAudioItem != null) {
					randomAudioItem.AddAudioItem(childAudioItem);
				}
			}
			
			randomAudioItem.Update();
			inactiveMultipleAudioItems.Add(randomAudioItem);
			return randomAudioItem;
		}
		
		public virtual MultipleAudioItem GetSwitchAudioItem(AudioContainer container, List<int> childrenIds, object source) {
			idCounter += 1;
			MultipleAudioItem switchAudioItem = new MultipleAudioItem(container.Name, idCounter, this, player);
			
			int stateIndex = int.MinValue;
			AudioSubContainer[] childrenSubContainers = container.IdsToSubContainers(childrenIds);
			
			if (childrenSubContainers[0].parentId == 0 && container.stateHolder != null && !string.IsNullOrEmpty(container.statePath)) {
				object stateValue = container.stateHolder.GetValueFromMember(container.statePath);
				stateIndex = stateValue == null ? int.MinValue : stateValue.GetHashCode();
			}
			else {
				AudioSubContainer parentSubContainer = container.GetSubContainerWithID(childrenSubContainers[0].parentId);
				
				if (parentSubContainer.stateHolder != null && !string.IsNullOrEmpty(parentSubContainer.statePath)) {
					object stateValue = parentSubContainer.stateHolder.GetValueFromMember(parentSubContainer.statePath);
					stateIndex = stateValue == null ? int.MinValue : stateValue.GetHashCode();
				}
			}
			
			if (stateIndex != int.MinValue) {
				foreach (AudioSubContainer childSubContainer in childrenSubContainers) {
					if (childSubContainer.stateIndex == stateIndex) {
						AudioItem childAudioItem = GetSubContainerAudioItem(container, childSubContainer, source);
						if (childAudioItem != null) {
							switchAudioItem.AddAudioItem(childAudioItem);
						}
					}
				}
			}
			
			switchAudioItem.Update();
			inactiveMultipleAudioItems.Add(switchAudioItem);
			return switchAudioItem;
		}

		public virtual AudioSource GetAudioSource(AudioInfo audioInfo, object source) {
			GameObject gameObject = GetGameObject(source);
			return SetAudioSource(gameObject.GetOrAddComponent<AudioSource>(), audioInfo);
		}
		
		public virtual AudioSource GetAudioSource(AudioInfo audioInfo, Vector3 source) {
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
		
		public virtual GameObject GetGameObject(object source) {
			GameObject gameObject;
			
			gameObject = inactiveAudioObjects.Count == 0 ? new GameObject() : inactiveAudioObjects.PopLast();
			gameObject.transform.parent = player.transform;
			gameObject.transform.Reset();
			
			if (source is GameObject) {
				gameObject.transform.position = ((GameObject)source).transform.position;
			}
			else if (source is Vector3) {
				gameObject.transform.position = ((Vector3)source);
			}
			
			gameObject.SetActive(true);
			Object.DontDestroyOnLoad(gameObject);
			
			return gameObject;
		}
	}
}
