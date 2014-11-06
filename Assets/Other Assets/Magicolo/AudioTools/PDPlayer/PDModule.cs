
namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDModule : MultipleAudioItem {

		public PDSpatializer spatializer;
		public PDPlayer pdPlayer;
		
		public PDModule(string name, int id, PDSpatializer spatializer, PDItemManager itemManager, PDPlayer pdPlayer)
			: base(name, id, itemManager, pdPlayer) {
			this.spatializer = spatializer;
			this.pdPlayer = pdPlayer;
			
			Initialize();
		}

		public PDModule(string name, int id, PDEditorModule editorModule, PDItemManager itemManager, PDPlayer pdPlayer)
			: base(name, id, itemManager, pdPlayer) {
			this.Volume = editorModule.Volume;
			this.spatializer = new PDSpatializer(name, editorModule, pdPlayer);
			this.pdPlayer = pdPlayer;
			
			Initialize();
		}
		
		public PDModule(int id, PDEditorModule editorModule, PDItemManager itemManager, PDPlayer pdPlayer)
			: base(editorModule.Name, id, itemManager, pdPlayer) {
			this.Volume = editorModule.Volume;
			this.spatializer = new PDSpatializer(editorModule, pdPlayer);
			this.pdPlayer = pdPlayer;
			
			Initialize();
		}
		
		public void Initialize() {
			spatializer.Initialize(Volume);
		}
		
		public override void Update() {
			UpdateActions();
			RemoveStoppedAudioItems();
			
			if (State == AudioStates.Playing) {
				spatializer.Update();
			}
		}

		protected override void UpdateVolume() {
			base.UpdateVolume();
			
			pdPlayer.communicator.SendValue(Name + "_Volume", Volume);
		}
		
		protected override void UpdatePitch() {
			base.UpdatePitch();
			
			pdPlayer.communicator.SendValue(Name + "_Pitch", Pitch);
		}
		
		public override void Play(params AudioOption[] audioOptions) {
			if (audioOptions.Length == 0 || !TryAddDelayedAction(AudioAction.ActionTypes.Play, audioOptions)) {
				base.Play(audioOptions);
			
				pdPlayer.communicator.SendValue(Name + "_Play", 1);
			}
		}
		
		public override void Pause(params AudioOption[] audioOptions) {
			if (audioOptions.Length == 0 || !TryAddDelayedAction(AudioAction.ActionTypes.Pause, audioOptions)) {
				base.Pause(audioOptions);
			
				pdPlayer.communicator.SendValue(Name + "_Pause", 0);
			}
		}
	
		public override void Stop(params AudioOption[] audioOptions) {
			if (audioOptions.Length == 0 || !TryAddDelayedAction(AudioAction.ActionTypes.Stop, audioOptions)) {
				base.Stop(audioOptions);
			
				pdPlayer.communicator.SendValue(Name + "_Stop", 0);
			}
		}
		
		public override void StopImmediate() {
			base.StopImmediate();
			
			pdPlayer.communicator.SendValue(Name + "_Stop", 0);
		}
	}
}
