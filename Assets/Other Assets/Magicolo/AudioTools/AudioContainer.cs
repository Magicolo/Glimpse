using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioContainer : INamable {
		
		public enum Types {
			MixContainer,
			RandomContainer,
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
		
		public Types type;
		
		public List<AudioSubContainer> subContainers = new List<AudioSubContainer>();
		public int idCounter;
		public List<int> childrenIds = new List<int>();
		
		Dictionary<int, AudioSubContainer> idDict;
		Dictionary<int, AudioSubContainer> IdDict {
			get {
				BuildIDDict();
				return idDict;
			}
		}
		
		public AudioContainer(string name) {
			this.Name = name;
		}
		
		public void BuildIDDict() {
			idDict = new Dictionary<int, AudioSubContainer>();
			foreach (AudioSubContainer subContainer in subContainers) {
				idDict[subContainer.id] = subContainer;
			}
		}
		
		public int GetUniqueID() {
			idCounter += 1;
			return idCounter;
		}
		
		public AudioSubContainer GetSubContainerWithID(int id) {
			AudioSubContainer subContainer = null;
			if (IdDict.ContainsKey(id)) {
				subContainer = IdDict[id];
			}
			return subContainer;
		}
	}
}