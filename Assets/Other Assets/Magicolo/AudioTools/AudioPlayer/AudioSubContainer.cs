using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioSubContainer : INamable, IShowable {

		public enum Types {
			AudioSource,
			Sampler,
			MixContainer,
			RandomContainer,
			SwitchContainer
		}
		
		[SerializeField]
		string name;
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		[SerializeField]
		bool showing;
		public bool Showing {
			get {
				return showing;
			}
			set {
				showing = value;
			}
		}
		
		public bool IsSource {
			get {
				return type == Types.AudioSource || type == Types.Sampler;
			}
		}
		
		public bool IsContainer {
			get {
				return type == Types.MixContainer || type == Types.RandomContainer || type == Types.SwitchContainer;
			}
		}
		
		public Types type;
		
		// Audio Source
		[SerializeField]
		AudioSetup audioSetup;
		public AudioSetup AudioSetup {
			get {
				if (audioSetup == null && audioInfo != null) {
					audioSetup = audioInfo.AudioSetup;
				}
				return audioSetup;
			}
			set {
				audioSetup = value;
				if (audioSetup != null) {
					audioInfo = audioSetup.audioInfo;
				}
			}
		}
		public AudioInfo audioInfo;
		
		// Sampler
		public string instrumentName;
		public int note = 60;
		public float velocity = 100;
		
		// Random Container
		[Min(0)] public float weight = 1;
		
		// Switch Container
		public Object stateHolder;
		public string statePath;
		public string stateName;
		
		public AudioOption[] audioOptions;
		
		public int id;
		public int parentId;
		public List<int> childrenIds = new List<int>();
		
		public AudioSubContainer(AudioContainer container, int parentId, AudioSubContainer subContainer) {
			this.Copy(subContainer, "id", "parentId", "childrenIds");
			
			this.name = container.Name;
			this.id = container.GetUniqueID();
			this.parentId = parentId;
			
			if (parentId == 0) {
				container.childrenIds.Add(id);
			}
			else {
				container.GetSubContainerWithID(parentId).childrenIds.Add(id);
			}
		}
		
		public AudioSubContainer(AudioContainer container, int parentId, AudioSetup audioSetup) {
			this.name = container.Name;
			this.id = container.GetUniqueID();
			this.parentId = parentId;
			AudioSetup = audioSetup;
			
			if (parentId == 0) {
				container.childrenIds.Add(id);
			}
			else {
				container.GetSubContainerWithID(parentId).childrenIds.Add(id);
			}
		}
		
		public AudioSubContainer(AudioContainer container, int parentId) {
			this.name = container.Name;
			this.id = container.GetUniqueID();
			this.parentId = parentId;
			
			if (parentId == 0) {
				container.childrenIds.Add(id);
			}
			else {
				container.GetSubContainerWithID(parentId).childrenIds.Add(id);
			}
		}
		
		public void Remove(AudioContainer container) {
			if (parentId == 0) {
				container.childrenIds.Remove(id);
			}
			else {
				AudioSubContainer parent = container.GetSubContainerWithID(parentId);
				if (parent != null) {
					parent.childrenIds.Remove(id);
				}
			}
			
			if (container.subContainers.Contains(this)) {
				container.subContainers.Remove(this);
			}
			
			foreach (int childrenId in childrenIds.ToArray()) {
				container.GetSubContainerWithID(childrenId).Remove(container);
			}
		}

		public override string ToString() {
			return string.Format("{0}({1}, {2})", GetType().Name, Name, id);
		}
	}
}
