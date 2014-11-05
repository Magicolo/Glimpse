using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDEditorModule : Magicolo.GeneralTools.INamable {
		
		[SerializeField, PropertyField]
		string name;
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		AudioStates state = AudioStates.Waiting;
		public AudioStates State {
			get {
				return Application.isPlaying ? pdPlayer.itemManager.GetModule(Name).State : state;
			}
		}
		
		[SerializeField, PropertyField(typeof(RangeAttribute), 0F, 5F)]
		float volume = 1;
		public float Volume {
			get {
				return volume;
			}
			set {
				volume = value;
				if (Application.isPlaying && !string.IsNullOrEmpty(Name)) {
					pdPlayer.itemManager.SetVolume(Name, volume);
				}
			}
		}

		[SerializeField, PropertyField]
		GameObject source;
		public GameObject Source {
			get {
				return source;
			}
			set {
				source = value;
				if (Application.isPlaying && !string.IsNullOrEmpty(Name)) {
					pdPlayer.itemManager.GetModule(Name).spatializer.Source = source;
				}
			}
		}

		[SerializeField, PropertyField]
		PDSpatializer.RolloffMode volumeRolloff;
		public PDSpatializer.RolloffMode VolumeRolloff {
			get {
				return volumeRolloff;
			}
			set {
				volumeRolloff = value;
				if (Application.isPlaying && !string.IsNullOrEmpty(Name)) {
					pdPlayer.itemManager.GetModule(Name).spatializer.VolumeRolloff = volumeRolloff;
				}
			}
		}

		[SerializeField]
		float minDistance = 1;
		public float MinDistance {
			get {
				return minDistance;
			}
			set {
				minDistance = value;
				if (Application.isPlaying && !string.IsNullOrEmpty(Name)) {
					pdPlayer.itemManager.GetModule(Name).spatializer.MinDistance = minDistance;
				}
			}
		}

		[SerializeField]
		float maxDistance = 500;
		public float MaxDistance {
			get {
				return maxDistance;
			}
			set {
				maxDistance = value;
				if (Application.isPlaying && !string.IsNullOrEmpty(Name)) {
					pdPlayer.itemManager.GetModule(Name).spatializer.MaxDistance = maxDistance;
				}
			}
		}

		[SerializeField, PropertyField(typeof(RangeAttribute), 0F, 1F)]
		float panLevel = 1;
		public float PanLevel {
			get {
				return panLevel;
			}
			set {
				panLevel = value;
				if (Application.isPlaying && !string.IsNullOrEmpty(Name)) {
					pdPlayer.itemManager.GetModule(Name).spatializer.PanLevel = panLevel;
				}
			}
		}

		public PDPlayer pdPlayer;
		public bool spatializerShowing = true;
		
		public PDEditorModule(string name, PDPlayer pdPlayer) {
			this.name = name;
			this.pdPlayer = pdPlayer;
			
			volume = 1;
			minDistance = 1;
			maxDistance = 500;
			panLevel = 1;
		}
		
		public PDEditorModule(PDModule module, PDPlayer pdPlayer) {
			this.name = module.Name;
			this.volume = module.GetVolume();
			this.source = module.spatializer.Source;
			this.volumeRolloff = module.spatializer.VolumeRolloff;
			this.minDistance = module.spatializer.MinDistance;
			this.maxDistance = module.spatializer.MaxDistance;
			this.panLevel = module.spatializer.PanLevel;
			this.pdPlayer = pdPlayer;
		}
		
		public PDEditorModule(string name, PDEditorModule editorModule, PDPlayer pdPlayer) {
			this.name = name;
			this.volume = editorModule.Volume;
			this.source = editorModule.Source;
			this.volumeRolloff = editorModule.VolumeRolloff;
			this.minDistance = editorModule.MinDistance;
			this.maxDistance = editorModule.MaxDistance;
			this.panLevel = editorModule.PanLevel;
			this.pdPlayer = pdPlayer;
		}
	
		public PDEditorModule() {
		}
	}
}
