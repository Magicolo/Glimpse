using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;

// TODO Add the ability to subscribe/unsubscribe to metronome events.
// FIXME Some AudioItems can be updated multiple times per frame (ex: Sampler plays a Pure Data sound)

[ExecuteInEditMode]
public class AudioPlayer : Magicolo.AudioTools.Player {

	static AudioPlayer instance;
	static AudioPlayer Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<AudioPlayer>();
			}
			if (instance == null && Application.isPlaying) {
				Debug.LogError("No AudioPlayer was found in the scene.");
			}
			return instance;
		}
	}
	
	AudioPlayer(){
	}
	
	#region Components
	public AudioPlayerEditorHelper editorHelper;
	public AudioHierarchyEditorHelper hierarchyEditorHelper;
	public AudioPlayerItemManager itemManager;
	#endregion
	
	protected override void Awake() {
		base.Awake();
		
		if (Application.isPlaying) {
			infoManager = new AudioInfoManager(Instance);
			containerManager = new AudioContainerManager(Instance);
			itemManager = new AudioPlayerItemManager(Instance);
		}
		
		generalSettings = generalSettings ?? new AudioGeneralSettings();
		generalSettings.Initialize(Instance);
		hierarchyManager = hierarchyManager ?? new AudioHierarchyManager();
		hierarchyManager.Initialize(Instance);
		editorHelper = editorHelper ?? new AudioPlayerEditorHelper();
		editorHelper.Initialize(Instance);
		hierarchyEditorHelper = hierarchyEditorHelper ?? new AudioHierarchyEditorHelper();
		hierarchyEditorHelper.Initialize(Instance);
	}
	
	protected virtual void Start() {
		SingletonCheck(Instance);
		if (Application.isPlaying) {
			metronome.Play();
		}
	}
	
	protected virtual void Update() {
		if (Application.isPlaying) {
			itemManager.Update();
		}
	}
	
	protected override void OnLevelWasLoaded(int level) {
		base.OnLevelWasLoaded(level);
		
		SingletonCheck(Instance);
		Resources.UnloadUnusedAssets();
	}

	#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	static void OnReloadScripts() {
		if (Instance != null) {
			Instance.Awake();
		}
	}
	#endif
	
	/// <summary>
	/// Plays an audio source spatialized around the listener.
	/// </summary>
	/// <param name="soundName">The name of the sound to be played.</param>
	/// <returns>The AudioItem that will let you control the audio source.</returns>
	/// <remarks>This is the non static version of AudioPlayer.Play(soundName) mainly intended for easy integration in UI elements.</remarks>
	public void play(string soundName) {
		Play(soundName);
	}
	
	/// <summary>
	/// Plays a container spatialized around the listener.
	/// </summary>
	/// <param name="containerName">The name of the container to be played.</param>
	/// <returns>The AudioItem that will let you control the container.</returns>
	/// <remarks>This is the non static version of AudioPlayer.PlayContainer(containerName) mainly intended for easy integration in UI elements.</remarks>
	public void playContainer(string containerName) {
		PlayContainer(containerName);
	}
	
	/// <summary>
	/// Plays an audio source spatialized around the <paramref name="source"/>.
	/// </summary>
	/// <param name="soundName">The name of the sound to be played.</param>
	/// <param name="source">The source around which the audio source will be spatialized.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="soundName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the audio source.</returns>
	public static AudioItem Play(string soundName, GameObject source, params AudioOption[] audioOptions) {
		return Instance.itemManager.Play(soundName, source, audioOptions);
	}
		
	/// <summary>
	/// Plays an audio source spatialized around the <paramref name="source"/>.
	/// </summary>
	/// <param name="soundName">The name of the sound to be played.</param>
	/// <param name="source">The source around which the audio source will be spatialized.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="soundName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the audio source.</returns>
	public static AudioItem Play(string soundName, Transform source, params AudioOption[] audioOptions) {
		return Instance.itemManager.Play(soundName, source.gameObject, audioOptions);
	}
	
	/// <summary>
	/// Plays an audio source spatialized around the <paramref name="source"/>.
	/// </summary>
	/// <param name="soundName">The name of the sound to be played.</param>
	/// <param name="source">The source around which the audio source will be spatialized.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="soundName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the audio source.</returns>
	public static AudioItem Play(string soundName, Vector3 source, params AudioOption[] audioOptions) {
		return Instance.itemManager.Play(soundName, source, audioOptions);
	}

	/// <summary>
	/// Plays an audio source spatialized around the listener.
	/// </summary>
	/// <param name="soundName">The name of the sound to be played.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="soundName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the audio source.</returns>
	public static AudioItem Play(string soundName, params AudioOption[] audioOptions) {
		return Instance.itemManager.Play(soundName, Instance.listener.gameObject, audioOptions);
	}

	/// <summary>
	/// Plays a container spatialized around the <paramref name="source"/>.
	/// </summary>
	/// <param name="containerName">The name of the container to be played.</param>
	/// <param name="source">The source around which the container will be spatialized.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="containerName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the container.</returns>
	public static AudioItem PlayContainer(string containerName, GameObject source, params AudioOption[] audioOptions) {
		return Instance.itemManager.PlayContainer(containerName, source, audioOptions);
	}
	
	/// <summary>
	/// Plays a container spatialized around the <paramref name="source"/>.
	/// </summary>
	/// <param name="containerName">The name of the container to be played.</param>
	/// <param name="source">The source around which the container will be spatialized.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="containerName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the container.</returns>
	public static AudioItem PlayContainer(string containerName, Transform source, params AudioOption[] audioOptions) {
		return Instance.itemManager.PlayContainer(containerName, source.gameObject, audioOptions);
	}
	
	/// <summary>
	/// Plays a container spatialized around the <paramref name="source"/>.
	/// </summary>
	/// <param name="containerName">The name of the container to be played.</param>
	/// <param name="source">The source around which the container will be spatialized.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="containerName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the container.</returns>
	public static AudioItem PlayContainer(string containerName, Vector3 source, params AudioOption[] audioOptions) {
		return Instance.itemManager.PlayContainer(containerName, source, audioOptions);
	}
	
	/// <summary>
	/// Plays a container spatialized around the listener.
	/// </summary>
	/// <param name="containerName">The name of the container to be played.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="containerName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the container.</returns>
	public static AudioItem PlayContainer(string containerName, params AudioOption[] audioOptions) {
		return Instance.itemManager.PlayContainer(containerName, Instance.listener.gameObject, audioOptions);
	}
	
	/// <summary>
	/// Gets the master volume of the PDPlayer.
	/// </summary>
	/// <returns>The master volume.</returns>
	public static float GetMasterVolume() {
		return Instance.generalSettings.MasterVolume;
	}
	
	/// <summary>
	/// Ramps the master volume of the AudioPlayer.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be ramped.</param>
	/// <param name="time">The time it will take for the volume to reach the <paramref name="targetVolume"/>.</param>
	public static void SetMasterVolume(float targetVolume, float time) {
		Instance.generalSettings.SetMasterVolume(targetVolume, time);
	}
	
	/// <summary>
	/// Sets the master volume of the AudioPlayer.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be set.</param>
	public static void SetMasterVolume(float targetVolume) {
		Instance.generalSettings.SetMasterVolume(targetVolume);
	}

	/// <summary>
	/// Gets the tempo settigns.
	/// </summary>
	/// <param name="beatsPerMinute">The number of beat events per minute.</param>
	/// <param name="beatsPerMeasure">The number of beats required before a measure event is triggered.</param>
	public static void GetTempo(out float beatsPerMinute, out int beatsPerMeasure) {
		Instance.metronome.GetTempo(out beatsPerMinute, out beatsPerMeasure);
	}
	
	/// <summary>
	/// Sets the tempo settings.
	/// </summary>
	/// <param name="beatsPerMinute">The number of beat events per minute.</param>
	/// <param name="beatsPerMeasure">The number of beats required before a measure event is triggered.</param>
	public static void SetTempo(float beatsPerMinute, int beatsPerMeasure) {
		Instance.metronome.SetTempo(beatsPerMinute, beatsPerMeasure);
	}
}
