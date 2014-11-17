using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDItemManager : AudioItemManager {
		
		public string currentModuleName;
		public PDPlayer pdPlayer;
		
		Dictionary<string, PDModule> moduleDict;
		
		public PDItemManager(PDPlayer pdPlayer)
			: base(pdPlayer.listener, pdPlayer.infoManager, pdPlayer) {
			this.pdPlayer = pdPlayer;
			BuildModulesDict();
		}

		public PDSingleAudioItem Play(string moduleName, string soundName, object source, params AudioOption[] audioOptions) {
			PDModule module = GetModule(moduleName, source);
			PDSingleAudioItem audioItem = GetSingleAudioItem(soundName, module.spatializer.Source) as PDSingleAudioItem;
			
			if (module.State != AudioStates.Playing) {
				module.Initialize();
				module.Play();
			}
			
			audioItem.Play(audioOptions);
			module.AddAudioItem(audioItem);
			LimitVoices();
			
			return audioItem;
		}

		public PDModule Play(string moduleName, object source, params AudioOption[] audioOptions) {
			PDModule module = GetModule(moduleName, source);
			
			if (module.State != AudioStates.Playing) {
				module.Initialize();
				module.Play(audioOptions);
			}
			
			return module;
		}
		
		public MultipleAudioItem PlayContainer(string moduleName, string containerName, object source, params AudioOption[] audioOptions) {
			PDModule module = GetModule(moduleName, source);
			MultipleAudioItem audioItem = GetMultipleAudioItem(player.containerManager.GetContainer(containerName), module.spatializer.Source);
			
			if (module.State != AudioStates.Playing) {
				module.Initialize();
				module.Play();
			}
			
			audioItem.Play(audioOptions);
			module.AddAudioItem(audioItem);
			
			return audioItem;
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
		
		public virtual PDModule GetModule(string moduleName) {
			return moduleDict[moduleName];
		}

		public virtual PDModule GetModule(string moduleName, object source) {
			currentModuleName = moduleName;
			PDModule module;
			
			if (moduleDict.ContainsKey(moduleName)) {
				module = moduleDict[moduleName];
				module.spatializer.Source = source == player.listener.gameObject && module.spatializer.Source != null ? module.spatializer.Source : source;
			}
			else {
				idCounter += 1;
				module = new PDModule(moduleName, idCounter, pdPlayer.editorHelper.defaultModule, this, pdPlayer);
				module.spatializer.Source = source == player.listener.gameObject && module.spatializer.Source != null ? module.spatializer.Source : source;
				moduleDict[moduleName] = module;
				pdPlayer.editorHelper.modules.Add(new PDEditorModule(module, pdPlayer));
				
				module.Update();
				inactiveMultipleAudioItems.Add(module);
			}
			return module;
		}

		public override SingleAudioItem GetSingleAudioItem(string soundName, object source) {
			AudioInfo audioInfo = infoManager.GetAudioInfo(soundName);
			AudioSource audioSource = GetAudioSource(audioInfo, source);
			CoroutineHolder coroutineHolder = audioSource.GetOrAddComponent<CoroutineHolder>();
			PDGainManager gainManager = audioSource.GetOrAddComponent<PDGainManager>();
			
			idCounter += 1;
			PDSingleAudioItem audioItem = new PDSingleAudioItem(currentModuleName, idCounter, audioSource, audioInfo, coroutineHolder, gainManager, this, pdPlayer);
			
			gainManager.Initialize(source, audioItem, pdPlayer);
			inactiveSingleAudioItems.Add(audioItem);
			return audioItem;
		}
	}
}
