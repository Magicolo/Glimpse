using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;

[RequireComponent(typeof(AudioPlayer))]
[ExecuteInEditMode]
public class Sampler : Magicolo.AudioTools.Player {
	
	static Sampler instance;
	static Sampler Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<Sampler>();
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
	
	protected virtual void Update() {
		if (Application.isPlaying) {
			itemManager.Update();
		}
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
	
	public static AudioItem Play(string instrumentName, int note, float velocity, GameObject source, params AudioOption[] audioOptions) {
		return Instance.itemManager.Play(instrumentName, note, velocity, source, audioOptions);
	}
	
	public static AudioItem GetInstrument(string instrumentName){
		return Instance.itemManager.GetInstrument(instrumentName);
	}
}
