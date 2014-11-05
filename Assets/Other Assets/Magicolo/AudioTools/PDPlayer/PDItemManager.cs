using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDItemManager : AudioItemManager {
		
		public PDPlayer pdPlayer;
		
		Dictionary<string, PDModule> moduleDict;
		
		public PDItemManager(PDPlayer pdPlayer)
			: base(pdPlayer.listener, pdPlayer.infoManager, pdPlayer) {
			this.pdPlayer = pdPlayer;
			BuildModulesDict();
		}
		
		public void Initialize() {
			foreach (PDModule module in moduleDict.Values) {
				module.Initialize();
			}
		}
		
		public PDSingleAudioItem Play(string moduleName, string soundName, GameObject source, params AudioOption[] audioOptions) {
			PDModule module = GetModule(moduleName, source);
			PDSingleAudioItem audioItem = GetSingleAudioItem(moduleName, soundName, module.spatializer.Source);
			module.AddAudioItem(audioItem);
			LimitVoices();
			module.Play(audioOptions);
			return audioItem;
		}

		public PDModule Play(string moduleName, GameObject source, params AudioOption[] audioOptions) {
			PDModule module = GetModule(moduleName, source);
			module.Play(audioOptions);
			return module;
		}
		
		public PDModule PlayContainer(string moduleName, string containerName, GameObject source, params AudioOption[] audioOptions) {
			PDModule module = GetModule(moduleName, source);
			module.AddAudioItem(GetAudioItem(moduleName, pdPlayer.containerManager.GetContainer(containerName), module.spatializer.Source));
			module.Play(audioOptions);
			return module;
		}
		
		public void Pause(string moduleName) {
			GetModule(moduleName).Pause();
		}
		
		public void Stop(string moduleName) {
			GetModule(moduleName).Stop();
		}
		
		public float GetVolume(string moduleName) {
			return GetModule(moduleName).GetVolume();
		}
		
		public void SetVolume(string moduleName, float targetVolume, float time) {
			GetModule(moduleName).SetVolume(targetVolume, time);
		}
		
		public void SetVolume(string moduleName, float targetVolume) {
			GetModule(moduleName).SetVolume(targetVolume);
		}
		
		public void BuildModulesDict() {
			moduleDict = new Dictionary<string, PDModule>();
			
			foreach (PDEditorModule editorModule in pdPlayer.editorHelper.modules) {
				idCounter += 1;
				PDModule module = new PDModule(idCounter, editorModule, this, pdPlayer);
				moduleDict[module.Name] = module;
			}
		}
		
		public PDModule GetModule(string moduleName) {
			return moduleDict[moduleName];
		}

		public PDModule GetModule(string moduleName, GameObject source) {
			PDModule module;
			if (moduleDict.ContainsKey(moduleName)) {
				module = moduleDict[moduleName];
				module.spatializer.Source = source ?? module.spatializer.Source;
			}
			else {
				idCounter += 1;
				module = new PDModule(moduleName, idCounter, pdPlayer.editorHelper.defaultModule, this, pdPlayer);
				module.spatializer.Source = source ?? module.spatializer.Source;
				moduleDict[moduleName] = module;
				pdPlayer.editorHelper.modules.Add(new PDEditorModule(module, pdPlayer));
				
				module.Update();
				inactiveAudioItems.Add(module);
			}
			return module;
		}

		public PDSingleAudioItem GetSingleAudioItem(string moduleName, string soundName, GameObject source) {
			AudioInfo audioInfo = infoManager.GetAudioInfo(soundName);
			AudioSource audioSource = GetAudioSource(audioInfo, source);
			CoroutineHolder coroutineHolder = audioSource.GetOrAddComponent<CoroutineHolder>();
			PDGainManager gainManager = audioSource.GetOrAddComponent<PDGainManager>();
			
			idCounter += 1;
			PDSingleAudioItem audioItem = new PDSingleAudioItem(moduleName, idCounter, audioSource, audioInfo, coroutineHolder, gainManager, this, pdPlayer);
			
			gainManager.Initialize(source, audioItem, pdPlayer);
			audioItem.Update();
			inactiveAudioItems.Add(audioItem);
			return audioItem;
		}
		
		public virtual AudioItem GetAudioItem(string moduleName, AudioContainer container, GameObject source) {
			MultipleAudioItem multipleAudioItem;
			
			idCounter += 1;
			switch (container.type) {
				case AudioContainer.Types.RandomContainer:
					multipleAudioItem = GetRandomAudioItem(moduleName, container, container.childrenIds, source);
					break;
				case AudioContainer.Types.SwitchContainer:
					multipleAudioItem = GetSwitchAudioItem(moduleName, container, container.childrenIds, source);
					break;
				default:
					multipleAudioItem = GetMixAudioItem(moduleName, container, container.childrenIds, source);
					break;
			}
			return multipleAudioItem;
		}
		
		public virtual AudioItem GetAudioItem(string moduleName, AudioContainer container, AudioSubContainer subContainer, GameObject source) {
			AudioItem audioItem = null;
			
			if (subContainer.IsSource) {
				audioItem = GetSourceAudioItem(moduleName, container, subContainer, source);
			}
			else {
				audioItem = GetContainerAudioItem(moduleName, container, subContainer, source);
			}
			return audioItem;
		}
		
		public virtual PDSingleAudioItem GetSourceAudioItem(string moduleName, AudioContainer container, AudioSubContainer subContainer, GameObject source) {
			PDSingleAudioItem sourceAudioItem = null;
			
			switch (subContainer.type) {
				default:
					if (subContainer.audioInfo != null) {
						sourceAudioItem = GetSingleAudioItem(moduleName, subContainer.audioInfo.Name, source);
						sourceAudioItem.startAudioOptions = subContainer.audioOptions;
					}
					break;
			}
			return sourceAudioItem;
		}
		
		public virtual MultipleAudioItem GetContainerAudioItem(string moduleName, AudioContainer container, AudioSubContainer subContainer, GameObject source) {
			MultipleAudioItem multipleAudioItem = null;
				
			switch (subContainer.type) {
				case AudioSubContainer.Types.RandomContainer:
					multipleAudioItem = GetRandomAudioItem(moduleName, container, subContainer.childrenIds, source);
					break;
				case AudioSubContainer.Types.SwitchContainer:
					multipleAudioItem = GetSwitchAudioItem(moduleName, container, subContainer.childrenIds, source);
					break;
				default:
					multipleAudioItem = GetMixAudioItem(moduleName, container, subContainer.childrenIds, source);
					break;
			}
			return multipleAudioItem;
		}
		
		public virtual MultipleAudioItem GetMixAudioItem(string moduleName, AudioContainer container, List<int> childrenIds, GameObject source) {
			idCounter += 1;
			MultipleAudioItem mixAudioItem = new MultipleAudioItem(container.Name, idCounter, this, pdPlayer);
			
			foreach (int childrenId in childrenIds) {
				AudioItem childAudioItem = GetAudioItem(moduleName, container, container.GetSubContainerWithID(childrenId), source);
				if (childAudioItem != null) {
					mixAudioItem.AddAudioItem(childAudioItem);
				}
			}
			
			mixAudioItem.Update();
			inactiveAudioItems.Add(mixAudioItem);
			return mixAudioItem;
		}
		
		public virtual MultipleAudioItem GetRandomAudioItem(string moduleName, AudioContainer container, List<int> childrenIds, GameObject source) {
			idCounter += 1;
			MultipleAudioItem randomAudioItem = new MultipleAudioItem(container.Name, idCounter, this, pdPlayer);
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
				AudioItem childAudioItem = GetAudioItem(moduleName, container, randomChildContainer, source);
				if (childAudioItem != null) {
					randomAudioItem.AddAudioItem(childAudioItem);
				}
			}
			
			randomAudioItem.Update();
			inactiveAudioItems.Add(randomAudioItem);
			return randomAudioItem;
		}
		
		public virtual MultipleAudioItem GetSwitchAudioItem(string moduleName, AudioContainer container, List<int> childrenIds, GameObject source) {
			idCounter += 1;
			MultipleAudioItem switchAudioItem = new MultipleAudioItem(container.Name, idCounter, this, pdPlayer);
			
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
						AudioItem childAudioItem = GetAudioItem(moduleName, container, childSubContainer, source);
						if (childAudioItem != null) {
							switchAudioItem.AddAudioItem(childAudioItem);
						}
					}
				}
			}
			
			switchAudioItem.Update();
			inactiveAudioItems.Add(switchAudioItem);
			return switchAudioItem;
		}
	}
}
