using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioPlayerItemManager : AudioItemManager {
		
		public AudioPlayer audioPlayer;
		
		public AudioPlayerItemManager(AudioPlayer audioPlayer)
			: base(audioPlayer.listener, audioPlayer.infoManager, audioPlayer) {
			
			this.audioPlayer = audioPlayer;
		}

		public virtual AudioItem Play(string soundName, GameObject source, params AudioOption[] audioOptions) {
			AudioItem audioItem = GetSingleAudioItem(soundName, source);
			audioItem.Play(audioOptions);
			return audioItem;
		}

		public virtual AudioItem PlayContainer(string containerName, GameObject source, params AudioOption[] audioOptions) {
			AudioItem audioItem = GetAudioItem(audioPlayer.containerManager.GetContainer(containerName), source);
			audioItem.Play(audioOptions);
			return audioItem;
		}

		public virtual AudioItem GetAudioItem(AudioContainer container, GameObject source) {
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
		
		public virtual AudioItem GetAudioItem(AudioContainer container, AudioSubContainer subContainer, GameObject source) {
			AudioItem audioItem = null;
			
			if (subContainer.IsSource) {
				audioItem = GetSourceAudioItem(container, subContainer, source);
			}
			else {
				audioItem = GetContainerAudioItem(container, subContainer, source);
			}
			return audioItem;
		}
		
		public virtual SingleAudioItem GetSourceAudioItem(AudioContainer container, AudioSubContainer subContainer, GameObject source) {
			SingleAudioItem sourceAudioItem = null;
			
			switch (subContainer.type) {
				case AudioSubContainer.Types.Sampler:
					sourceAudioItem = audioPlayer.generalSettings.sampler.itemManager.GetSingleAudioItem(audioPlayer.generalSettings.sampler.itemManager.GetInstrument(subContainer.instrumentName), subContainer.note, subContainer.velocity, source);
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
		
		public virtual MultipleAudioItem GetContainerAudioItem(AudioContainer container, AudioSubContainer subContainer, GameObject source) {
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
		
		public virtual MultipleAudioItem GetMixAudioItem(AudioContainer container, List<int> childrenIds, GameObject source) {
			idCounter += 1;
			MultipleAudioItem mixAudioItem = new MultipleAudioItem(container.Name, idCounter, this, audioPlayer);
			
			foreach (int childrenId in childrenIds) {
				AudioItem childAudioItem = GetAudioItem(container, container.GetSubContainerWithID(childrenId), source);
				if (childAudioItem != null) {
					mixAudioItem.AddAudioItem(childAudioItem);
				}
			}
			
			mixAudioItem.Update();
			inactiveMultipleAudioItems.Add(mixAudioItem);
			return mixAudioItem;
		}
		
		public virtual MultipleAudioItem GetRandomAudioItem(AudioContainer container, List<int> childrenIds, GameObject source) {
			idCounter += 1;
			MultipleAudioItem randomAudioItem = new MultipleAudioItem(container.Name, idCounter, this, audioPlayer);
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
				AudioItem childAudioItem = GetAudioItem(container, randomChildContainer, source);
				if (childAudioItem != null) {
					randomAudioItem.AddAudioItem(childAudioItem);
				}
			}
			
			randomAudioItem.Update();
			inactiveMultipleAudioItems.Add(randomAudioItem);
			return randomAudioItem;
		}
		
		public virtual MultipleAudioItem GetSwitchAudioItem(AudioContainer container, List<int> childrenIds, GameObject source) {
			idCounter += 1;
			MultipleAudioItem switchAudioItem = new MultipleAudioItem(container.Name, idCounter, this, audioPlayer);
			
			string stateName = "";
			AudioSubContainer[] childrenSubContainers = container.IdsToSubContainers(childrenIds);
			
			if (childrenSubContainers[0].parentId == 0 && container.stateHolder != null && !string.IsNullOrEmpty(container.statePath)) {
				stateName = "" + container.stateHolder.GetValueFromMember(container.statePath);
			}
			else {
				AudioSubContainer parentSubContainer = container.GetSubContainerWithID(childrenSubContainers[0].parentId);
				
				if (parentSubContainer.stateHolder != null && !string.IsNullOrEmpty(parentSubContainer.statePath)) {
					stateName = "" + parentSubContainer.stateHolder.GetValueFromMember(parentSubContainer.statePath);
				}
			}
			
			if (!string.IsNullOrEmpty(stateName)) {
				foreach (AudioSubContainer childSubContainer in childrenSubContainers) {
					if (childSubContainer.stateName == stateName) {
						AudioItem childAudioItem = GetAudioItem(container, childSubContainer, source);
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
	}
}
