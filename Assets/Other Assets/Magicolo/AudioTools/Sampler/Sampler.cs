using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;

// TODO Add the logic to send Sampler notes to Pure Data.

[RequireComponent(typeof(AudioPlayer))]
[ExecuteInEditMode]
public class Sampler : Magicolo.AudioTools.Player {
	
	static Sampler instance;
	static Sampler Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<Sampler>();
			}
			if (instance == null && Application.isPlaying){
				Debug.LogError("No Sampler was found in the scene.");
			}
			return instance;
		}
	}
	
	#region Components
	public SamplerEditorHelper editorHelper;
	public SamplerItemManager itemManager;
	#endregion
	
	protected override void Awake() {
		base.Awake();
		
		if (Application.isPlaying) {
			itemManager = new SamplerItemManager(Instance);
		}
		
		editorHelper = editorHelper ?? new SamplerEditorHelper();
		editorHelper.Initialize(Instance);
	}
	
	protected virtual void Start() {
		SingletonCheck(Instance);
	}

	protected override void OnLevelWasLoaded(int level) {
		base.OnLevelWasLoaded(level);
		
		SingletonCheck(Instance);
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
	/// Plays a note from a sampler instrument spatialized around the <paramref name="source"/>.
	/// </summary>
	/// <param name="instrumentName">The name of the sampler instrument to be played.</param>
	/// <param name = "note">The note to be played.</param>
	/// <param name = "velocity">The velocity at which the note will be played.</param>
	/// <param name="source">The source around which the note will be spatialized.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="instrumentName"/> inspector.</param>
	/// <returns>If the velocity > 0, the AudioItem that will let you control the note; Otherwise, the AudioItem representing the instrument.</returns>
	public static AudioItem Play(string instrumentName, int note, float velocity, GameObject source, params AudioOption[] audioOptions) {
		return Instance.itemManager.Play(instrumentName, note, velocity, source, audioOptions);
	}

	/// <summary>
	/// Plays a note from a sampler instrument spatialized around the listener.
	/// </summary>
	/// <param name="instrumentName">The name of the sampler instrument to be played.</param>
	/// <param name = "note">The note to be played.</param>
	/// <param name = "velocity">The velocity at which the note will be played.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="instrumentName"/> inspector.</param>
	/// <returns>If the velocity > 0, the AudioItem that will let you control the note; Otherwise, the AudioItem representing the instrument.</returns>
	public static AudioItem Play(string instrumentName, int note, float velocity, params AudioOption[] audioOptions) {
		return Instance.itemManager.Play(instrumentName, note, velocity, null, audioOptions);
	}

	/// <summary>
	/// Gets the instrument of name <paramref name="instrumentName"/>.
	/// </summary>
	/// <param name="instrumentName">The name of the instrument to get.</param>
	/// <returns>The instrument.</returns>
	public static AudioItem GetInstrument(string instrumentName){
		return Instance.itemManager.GetInstrument(instrumentName);
	}
}
