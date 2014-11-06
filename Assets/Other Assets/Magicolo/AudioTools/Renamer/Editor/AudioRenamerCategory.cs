using System.Collections.Generic;
using Magicolo.GeneralTools;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioRenamerCategory : INamable {

		public string Name { get; set; }
		public List<string> tags = new List<string>();
		public string selectedTag = "";
		public int selectedTagIndex;
		
		public string tagNameToAdd;
	
		public AudioRenamerCategory() {
			Name = "default";
		}
	
		public AudioRenamerCategory(string name) {
			this.Name = name;
		}
		
		public AudioRenamerCategory(string name, params string[] tags) {
			this.Name = name;
			foreach (string tag in tags) {
				if (!string.IsNullOrEmpty(tag)) {
					this.tags.Add(tag);
				}
			}
			this.tags.Sort();
		}
		
		public void AddTag(string tag) {
			tags.Add(tag);
			tags.Sort();
		}
	}
}
