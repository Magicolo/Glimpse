using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioSubContainer : INamable, IShowable {

		public enum Types {
			AudioSource,
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
				return type == Types.AudioSource;
			}
		}
		
		public bool IsContainer {
			get {
				return type == Types.MixContainer || type == Types.RandomContainer;
			}
		}
		
		public Types type;
		[Min(0)] public float weight = 1;
		public AudioOptions audioOptions;
		public AudioOption[] options;
		
		public int id;
		public int parentId;
		public List<int> childrenIds = new List<int>();

		public AudioSubContainer(AudioContainer container, int parentId, AudioSubContainer subContainer) {
			this.name = container.Name;
			this.id = container.GetUniqueID();
			this.parentId = parentId;
			
			type = subContainer.type;
			weight = subContainer.weight;
			
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
			return string.Format("Name: {0}, Id: {1}, ParentId: {2}, ChildrenIds: {3}", name, id, parentId, Logger.ObjectToString(childrenIds));
		}
	}
}
