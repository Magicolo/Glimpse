﻿using UnityEngine;
using Magicolo.AudioTools;
using System.Collections;

// TODO Fuse PDEditorModule and PDModule.
// TODO Add sends in the inspector.
// TODO Check if the persistent option works with Pure Data.

[RequireComponent(typeof(AudioPlayer))]
[ExecuteInEditMode]
public class PDPlayer : Magicolo.AudioTools.Player {

	public string patchesPath = "Patches";
	
	static PDPlayer instance;
	static PDPlayer Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<PDPlayer>();
			}
			return instance;
		}
	}
	
	#region Components
	public PDEditorHelper editorHelper;
	public PDBridge bridge;
	public PDCommunicator communicator;
	public PDPatchManager patchManager;
	public PDItemManager itemManager;
	public PDAudioFilterRead filterRead;
	#endregion

	protected override void Awake() {
		base.Awake();
		
		if (Application.isPlaying) {
			bridge = new PDBridge(Instance);
			communicator = new PDCommunicator(Instance);
			patchManager = new PDPatchManager(Instance);
			itemManager = new PDItemManager(Instance);
			
			bridge.Start();
			communicator.Start();
			patchManager.Start();
		}
		
		editorHelper = editorHelper ?? new PDEditorHelper();
		editorHelper.Initialize(Instance);
	}
	
	protected override void SetAudioListener() {
		base.SetAudioListener();
		
		listener.enabled = false;
		filterRead = listener.GetOrAddComponent<PDAudioFilterRead>();
		filterRead.pdPlayer = Instance;
		listener.enabled = true;
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
	
	protected virtual void OnApplicationQuit() {
		if (Application.isPlaying) {
			patchManager.Stop();
			bridge.Stop();
			communicator.Stop();
		}
	}
	
	#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	static void OnReloadScripts() {
		if (Instance != null) {
			Instance.Awake();
		}
	}
	#endif
	
	void OnDrawGizmos() {
		editorHelper.DrawGizmos();
	}

	/// <summary>
	/// Plays a module with an audio source spatialized around the <paramref name="source"/>. In Pure Data, you can receive the play command (float 1) with a <c>[receive <paramref name="moduleName"/>_Play]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be played. If the module doesn't exist, one will be created with the default settings.</param>
	/// <param name="soundName">The name of sound to be played. In Pure Data, use <c>[ureceive~ <paramref name="moduleName"/>_<paramref name="soundName"/>]</c> to receive the audio. Do not send this audio signal through a <c>[uspatialize~]</c> because it is already spatialized.</param>
	/// <param name="source">The source around which the module and audio source will be spatialized. If a source is already provided in the inspector, it will be overriden. In Pure Data, send the audio that you want spatialized through a <c>[uspatialize~ <paramref name="moduleName"/>]</c>.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="soundName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the audio source.</returns>
	public static AudioItem Play(string moduleName, string soundName, GameObject source, params AudioOption[] audioOptions) {
		return instance.itemManager.Play(moduleName, soundName, source, audioOptions);
	}
	
	/// <summary>
	/// Plays a module with an audio source spatialized around the listener. In Pure Data, you can receive the play command (float 1) with a <c>[receive <paramref name="moduleName"/>_Play]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be played. If the module doesn't exist, one will be created with the default settings.</param>
	/// <param name="soundName">The name of sound to be played. In Pure Data, use <c>[ureceive~ <paramref name="moduleName"/>_<paramref name="soundName"/>]</c> to receive the audio. Do not send this audio signal through a <c>[uspatialize~]</c> because it is already spatialized.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="soundName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the audio source.</returns>
	public static AudioItem Play(string moduleName, string soundName, params AudioOption[] audioOptions) {
		return instance.itemManager.Play(moduleName, soundName, null, audioOptions);
	}
	
	/// <summary>
	/// Plays a module spatialized around the <paramref name="source"/>. In Pure Data, you can receive the play command (float 1) with a <c>[receive <paramref name="moduleName"/>_Play]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be played. If the module doesn't exist, one will be created with the default settings.</param>
	/// <param name="source">The source around which the module will be spatialized. If a source is already provided in the inspector, it will be overriden. In Pure Data, send the audio that you want spatialized through a <c>[uspatialize~ <paramref name="moduleName"/>]</c>.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the inspector.</param>
	/// <returns>The AudioItem that will let you control the module.</returns>
	public static AudioItem Play(string moduleName, GameObject source, params AudioOption[] audioOptions) {
		return instance.itemManager.Play(moduleName, source, audioOptions);
	}
	
	/// <summary>
	/// Plays a module with an audio source spatialized around the listener. In Pure Data, you can receive the play command (float 1) with a <c>[receive <paramref name="moduleName"/>_Play]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be played. If the module doesn't exist, one will be created with the default settings.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the inspector.</param>
	/// <returns>The AudioItem that will let you control the module.</returns>
	public static AudioItem Play(string moduleName, params AudioOption[] audioOptions) {
		return instance.itemManager.Play(moduleName, null, audioOptions);
	}

	/// <summary>
	/// Plays a container spatialized around the <paramref name="source"/>. In Pure Data, you can receive the play command (float 1) with a <c>[receive <paramref name="moduleName"/>_Play]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be played. If the module doesn't exist, one will be created with the default settings.</param>
	/// <param name="containerName">The name of the container to be played.</param>
	/// <param name="source">The source around which the module will be spatialized. If a source is already provided in the inspector, it will be overriden. In Pure Data, send the audio that you want spatialized through a <c>[uspatialize~ <paramref name="moduleName"/>]</c>.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="containerName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the container.</returns>
	public static AudioItem PlayContainer(string moduleName, string containerName, GameObject source, params AudioOption[] audioOptions) {
		return Instance.itemManager.PlayContainer(moduleName, containerName, source, audioOptions);
	}
	
	/// <summary>
	/// Plays a container spatialized around the listener. In Pure Data, you can receive the play command (float 1) with a <c>[receive <paramref name="moduleName"/>_Play]</c>.
	/// </summary>
	/// <param name="moduleName">The name of the module to be played. If the module doesn't exist, one will be created with the default settings.</param>
	/// <param name="containerName">The name of the container to be played.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="containerName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the container.</returns>
	public static AudioItem PlayContainer(string moduleName, string containerName, params AudioOption[] audioOptions) {
		return Instance.itemManager.PlayContainer(moduleName, containerName, null, audioOptions);
	}
	
	/// <summary>
	/// Gets the module of name <paramref name="moduleName"/>.
	/// </summary>
	/// <param name="moduleName">The name of the module to get.</param>
	/// <returns>The module.</returns>
	public static AudioItem GetModule(string moduleName) {
		return Instance.itemManager.GetModule(moduleName);
	}
	
	/// <summary>
	/// Gets the master volume of the AudioPlayer.
	/// </summary>
	/// <returns>The master volume.</returns>
	public static float GetMasterVolume() {
		return instance.generalSettings.MasterVolume;
	}
	
	/// <summary>
	/// Ramps the master volume of the AudioPlayer. In Pure Data, you can receive the volume command (float <paramref name="targetVolume"/>) with a <c>[receive <paramref name="moduleName"/>_Volume]</c>.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be ramped.</param>
	/// <param name="time">The time it will take for the volume to reach the <paramref name="targetVolume"/>.</param>
	public static void SetMasterVolume(float targetVolume, float time) {
		instance.itemManager.SetMasterVolume(targetVolume, time);
	}
	
	/// <summary>
	/// Sets the master volume of the AudioPlayer. In Pure Data, you can receive the volume command (float <paramref name="targetVolume"/>) with a <c>[receive <paramref name="moduleName"/>_Volume]</c>.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be set.</param>
	public static void SetMasterVolume(float targetVolume) {
		instance.itemManager.SetMasterVolume(targetVolume);
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
	
	#region Send
	/// <summary>
	/// Converts and sends a value to Pure Data. In Pure Data, you can receive the value with a <c>[receive <paramref name="receiverName"/>]</c>.
	/// </summary>
	/// <param name="receiverName">The name of to be used in Pure Data to receive the value.</param>
	/// <param name="toSend">The value to be sent. Valid types include int, int[] float, float[], double, double[], bool, bool[], char, char[], string, string[], Enum, Enum[], Vector2, Vector3, Vector4, Quaternion, Rect, Bounds and Color.</param>
	/// <returns>True if the value has been successfully sent and received.</returns>
	public static bool SendValue(string receiverName, object toSend) {
		return Instance.communicator.SendValue(receiverName, toSend);
	}
	
	/// <summary>
	/// Sends a bang to Pure Data. In Pure Data, you can receive the bang with a <c>[receive <paramref name="receiverName"/>]</c>.
	/// </summary>
	/// <param name="receiverName">The name of to be used in Pure Data to receive the value.</param>
	/// <returns>True if the bang has been successfully sent and received.</returns>
	public static bool SendBang(string receiverName) {
		return Instance.communicator.SendBang(receiverName);
	}
	
	/// <summary>
	/// Sends a message to Pure Data. In Pure Data, you can receive the message with a <c>[receive <paramref name="receiverName"/>]</c>.
	/// </summary>
	/// <param name="receiverName">The name of to be used in Pure Data to receive the value.</param>
	/// <param name="message">The message to be sent.</param>
	/// <param name="arguments">Additional arguments can be added to the message. Valid types include int, float, string.</param>
	/// <returns>True if the message has been successfully sent and received.</returns>
	public static bool SendMessage<T>(string receiverName, string message, params T[] arguments) {
		return Instance.communicator.SendMessage(receiverName, message, arguments);
	}
	
	/// <summary>
	/// Sends a aftertouch event to Pure Data. In Pure Data, you can receive the aftertouch event with a <c>[touchin]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the aftertouch event has been successfully sent and received.</returns>
	public static bool SendAftertouch(int channel, int value) {
		return Instance.communicator.SendAftertouch(channel, value);
	}
	
	/// <summary>
	/// Sends a control change event to Pure Data. In Pure Data, you can receive the control change event with a <c>[ctlin]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="controller">The controller to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the control change event has been successfully sent and received.</returns>
	public static bool SendControlChange(int channel, int controller, int value) {
		return Instance.communicator.SendControlChange(channel, controller, value);
	}
	
	/// <summary>
	/// Sends a raw midi byte to Pure Data. In Pure Data, you can receive the raw midi byte with a <c>[midiin]</c>.
	/// </summary>
	/// <param name="port">The port to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the raw midi byte has been successfully sent and received.</returns>
	public static bool SendMidiByte(int port, int value) {
		return Instance.communicator.SendMidiByte(port, value);
	}
	
	/// <summary>
	/// Sends a note on event to Pure Data. In Pure Data, you can receive the note on event with a <c>[notein]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="pitch">The pitch to be sent.</param>
	/// <param name="velocity">The velocity to be sent.</param>
	/// <returns>True if the note on event has been successfully sent and received.</returns>
	public static bool SendNoteOn(int channel, int pitch, int velocity) {
		return Instance.communicator.SendNoteOn(channel, pitch, velocity);
	}
	
	/// <summary>
	/// Sends a pitch bend event to Pure Data. In Pure Data, you can receive the pitch bend event with a <c>[bendin]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the pitch bend event has been successfully sent and received.</returns>
	public static bool SendPitchbend(int channel, int value) {
		return Instance.communicator.SendPitchbend(channel, value);
	}
	
	/// <summary>
	/// Sends a polyphonic aftertouch event to Pure Data. In Pure Data, you can receive the polyphonic aftertouch event with a <c>[polytouchin]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="pitch">The pitch to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the polyphonic aftertouch event has been successfully sent and received.</returns>
	public static bool SendPolyAftertouch(int channel, int pitch, int value) {
		return Instance.communicator.SendPolyAftertouch(channel, pitch, value);
	}
	
	/// <summary>
	/// Sends a program change event to Pure Data. In Pure Data, you can receive the program change event with a <c>[pgmin]</c>.
	/// </summary>
	/// <param name="channel">The channel to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the program change event has been successfully sent and received.</returns>
	public static bool SendProgramChange(int channel, int value) {
		return Instance.communicator.SendProgramChange(channel, value);
	}
	
	/// <summary>
	/// Sends a byte of a sysex message to Pure Data. In Pure Data, you can receive the byte of the sysex message with a <c>[sysexin]</c>.
	/// </summary>
	/// <param name="port">The port to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the byte of the sysex message has been successfully sent and received.</returns>
	public static bool SendSysex(int port, int value) {
		return Instance.communicator.SendSysex(port, value);
	}
	
	/// <summary>
	/// Sends a byte to Pure Data. In Pure Data, you can receive the byte with a <c>[realtimein]</c>.
	/// </summary>
	/// <param name="port">The port to be sent.</param>
	/// <param name="value">The value to be sent.</param>
	/// <returns>True if the byte has been successfully sent and received.</returns>
	public static bool SendSysRealtime(int port, int value) {
		return Instance.communicator.SendSysRealtime(port, value);
	}
	
	/// <summary>
	/// Writes an audio clip to a Pure Data array (the array will be resized if needed). In Pure Data, you can receive the data with a <c>[table <paramref name="arrayName"/>]</c>.
	/// </summary>
	/// <param name="arrayName">The name of the array that will receive the data.</param>
	/// <param name="soundName">The name of the sound to be written to the array.</param>
	/// <returns>True if the data has been successfully sent and received.</returns>
	public static bool WriteArray(string arrayName, string soundName) {
		return Instance.communicator.WriteArray(arrayName, soundName);
	}
	
	/// <summary>
	/// Writes an audio clip to a Pure Data array (the array will be resized if needed). In Pure Data, you can receive the data with a <c>[table <paramref name="arrayName"/>]</c>.
	/// </summary>
	/// <param name="arrayName">The name of the array that will receive the data.</param>
	/// <param name="data">The data to be written to the array.</param>
	/// <returns>True if the data has been successfully sent and received.</returns>
	public static bool WriteArray(string arrayName, float[] data) {
		return Instance.communicator.WriteArray(arrayName, data);
	}
	
	/// <summary>
	/// Resizes a Pure Data array to a new size. Can be used to free memory.
	/// </summary>
	/// <param name="arrayName">The name of the array to be resized.</param>
	/// <param name="size">The target size of the array.</param>
	/// <returns>True if the array has been successfully resized.</returns>
	public static bool ResizeArray(string arrayName, int size) {
		return Instance.communicator.ResizeArray(arrayName, size);
	}
	#endregion
	
	/// <summary>
	/// Opens a patch and starts the DSP.
	/// </summary>
	/// <param name="patchName">The name of the patch (without the extension) to be opened relative to <c>Assets/StreamingAssets/<paramref name="patchesPath"/>/</c></param>.
	public static void OpenPatch(string patchName) {
		Instance.patchManager.Open(patchName);
	}
	
	/// <summary>
	/// Opens patches and starts the DSP.
	/// </summary>
	/// <param name="patchesName">The name of the patches (without the extension) to be opened relative to <c>Assets/StreamingAssets/<paramref name="patchesPath"/>/</c></param>
	public static void OpenPatches(params string[] patchesName) {
		Instance.patchManager.Open(patchesName);
	}
	
	/// <summary>
	/// Closes a patch.
	/// </summary>
	/// <param name="patchName">The name of the patch (without the extension and directory) to be closed.</param>
	public static void ClosePatch(string patchName) {
		Instance.patchManager.Close(patchName);
	}
	
	/// <summary>
	/// Closes patches.
	/// </summary>
	/// <param name="patchesName">The name of the patches (without the extension and directory) to be closed.</param>
	public static void ClosePatches(params string[] patchesName) {
		Instance.patchManager.Close(patchesName);
	}
	
	/// <summary>
	/// Closes all opened patches.
	/// </summary>
	public static void CloseAllPatches() {
		Instance.patchManager.CloseAll();
	}
}
