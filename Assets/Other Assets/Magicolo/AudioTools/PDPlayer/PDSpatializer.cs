using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDSpatializer {

		public enum RolloffMode {
			Logarithmic,
			Linear
		}
		
		string moduleName;
		public string ModuleName {
			get {
				return moduleName;
			}
			set {
				moduleName = value;
				Spatialize();
			}
		}

		object source;
		public object Source {
			get {
				return source;
			}
			set {
				source = value;
				Spatialize();
			}
		}

		RolloffMode volumeRolloff;
		public RolloffMode VolumeRolloff {
			get {
				return volumeRolloff;
			}
			set {
				volumeRolloff = value;
				Spatialize();
			}
		}

		float minDistance = 1;
		public float MinDistance {
			get {
				return minDistance;
			}
			set {
				minDistance = value;
				Spatialize();
			}
		}

		float maxDistance = 500;
		public float MaxDistance {
			get {
				return maxDistance;
			}
			set {
				maxDistance = value;
				Spatialize();
			}
		}

		float panLevel = 0.75F;
		public float PanLevel {
			get {
				return panLevel;
			}
			set {
				panLevel = value;
				Spatialize();
			}
		}
		
		public PDPlayer pdPlayer;

		string hrfLeftName;
		string hrfRightName;
		string panLeftName;
		string panRightName;
		string attenuationName;
		
		public PDSpatializer(string moduleName, PDEditorModule editorModule, PDPlayer pdPlayer) {
			this.moduleName = moduleName;
			this.source = editorModule.Source;
			this.volumeRolloff = editorModule.VolumeRolloff;
			this.minDistance = editorModule.MinDistance;
			this.maxDistance = editorModule.MaxDistance;
			this.panLevel = editorModule.PanLevel;
			this.pdPlayer = pdPlayer;
			SetNames();
		}
		
		public PDSpatializer(PDEditorModule editorModule, PDPlayer pdPlayer) {
			this.moduleName = editorModule.Name;
			this.source = editorModule.Source;
			this.volumeRolloff = editorModule.VolumeRolloff;
			this.minDistance = editorModule.MinDistance;
			this.maxDistance = editorModule.MaxDistance;
			this.panLevel = editorModule.PanLevel;
			this.pdPlayer = pdPlayer;
			SetNames();
		}
		
		public void Initialize() {
			pdPlayer.communicator.SendValue(hrfLeftName, 20000);
			pdPlayer.communicator.SendValue(hrfRightName, 20000);
			pdPlayer.communicator.SendValue(panLeftName, 1);
			pdPlayer.communicator.SendValue(panRightName, 1);
			pdPlayer.communicator.SendValue(attenuationName, 1);
		}
		
		public void Update() {
			if (CheckForChanges()) {
				Spatialize();
			}
		}
		
		public void Spatialize() {
			if (Source != null) {
				const float fullFrequencyRange = 20000;
				const float hrfFactor = 1500;
				const float curveDepth = 3.5F;
			
				Vector3 sourcePosition = GetSourcePosition();
				
				Vector3 listenerToSource = sourcePosition - pdPlayer.listener.transform.position;
				float angle = Vector3.Angle(pdPlayer.listener.transform.right, listenerToSource);
				float panLeft = (1 - PanLevel) + PanLevel * Mathf.Sin(Mathf.Max(180 - angle, 90) * Mathf.Deg2Rad);
				float panRight = (1 - PanLevel) + PanLevel * Mathf.Sin(Mathf.Max(angle, 90) * Mathf.Deg2Rad);
				
				float behindFactor = 1 + 4 * (Mathf.Clamp(Vector3.Angle(listenerToSource, pdPlayer.listener.transform.forward), 90, 135) - 90) / (135 - 90);
				float hrfLeft = Mathf.Pow(panLeft, 2) * (fullFrequencyRange - hrfFactor) / behindFactor + hrfFactor;
				float hrfRight = Mathf.Pow(panRight, 2) * (fullFrequencyRange - hrfFactor) / behindFactor + hrfFactor;
				float distance = Vector3.Distance(sourcePosition, pdPlayer.listener.transform.position);
				float adjustedDistance = Mathf.Clamp01(Mathf.Max(distance - MinDistance, 0) / Mathf.Max(MaxDistance - MinDistance, 0.001F));
				
				float attenuation;
				if (VolumeRolloff == RolloffMode.Linear) {
					attenuation = 1F - adjustedDistance;
				}
				else {
					attenuation = Mathf.Pow((1F - Mathf.Pow(adjustedDistance, 1F / curveDepth)), curveDepth);
				}
			
				pdPlayer.communicator.SendValue(hrfLeftName, hrfLeft);
				pdPlayer.communicator.SendValue(hrfRightName, hrfRight);
				pdPlayer.communicator.SendValue(panLeftName, panLeft);
				pdPlayer.communicator.SendValue(panRightName, panRight);
				pdPlayer.communicator.SendValue(attenuationName, attenuation);
			}
		}
	
		public Vector3 GetSourcePosition() {
			Vector3 sourcePosition = pdPlayer.listener.transform.position;
			
			if (Source as GameObject != null) {
				sourcePosition = ((GameObject)Source).transform.position;
			}
			else if (Source is Vector3) {
				sourcePosition = ((Vector3)Source);
			}
			return sourcePosition;
		}
		
		public bool CheckForChanges() {
			bool changed = false;
			
			if (Source as GameObject != null) {
				if (((GameObject)Source).transform.hasChanged || pdPlayer.listener.transform.hasChanged) {
					changed = true;
					pdPlayer.SetTransformHasChanged(((GameObject)Source).transform, false);
					pdPlayer.SetTransformHasChanged(pdPlayer.listener.transform, false);
				}
			}
			return changed;
		}

		public void SetNames() {
			hrfLeftName = ModuleName + "_HRFLeft";
			hrfRightName = ModuleName + "_HRFRight";
			panLeftName = ModuleName + "_PanLeft";
			panRightName = ModuleName + "_PanRight";
			attenuationName = ModuleName + "_Attenuation";
		}
	}
}
