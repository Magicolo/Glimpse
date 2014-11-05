using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;
using Magicolo.GeneralTools;

[System.Serializable]
public abstract class AudioItem : INamable {

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
	int id;
	public int Id {
		get {
			return id;
		}
		set {
			id = value;
		}
	}
	
	[SerializeField]
	AudioStates state;
	public AudioStates State {
		get {
			return state;
		}
		set {
			state = value;
		}
	}

	[SerializeField]
	float volume;
	protected float Volume {
		get {
			return volume;
		}
		set {
			volume = value;
		}
	}
	
	[SerializeField]
	float pitch;
	protected float Pitch {
		get {
			return pitch;
		}
		set {
			pitch = value;
		}
	}
	
	protected List<AudioAction> actions = new List<AudioAction>();
	
	protected AudioItemManager itemManager;
	protected Magicolo.AudioTools.Player player;
	
	protected AudioItem(string name, int id, float volume, float pitch, AudioStates state, AudioItemManager itemManager, Magicolo.AudioTools.Player player) {
		this.Name = name;
		this.Id = id;
		this.Volume = volume;
		this.Pitch = pitch;
		this.State = state;
		this.itemManager = itemManager;
		this.player = player;
	}
	
	protected AudioItem(string name, int id, AudioItemManager itemManager, Magicolo.AudioTools.Player player) {
		this.Name = name;
		this.Id = id;
		this.Volume = 1;
		this.Pitch = 1;
		this.State = AudioStates.Waiting;
		this.itemManager = itemManager;
		this.player = player;
	}
	
	public virtual void Update() {
		foreach (AudioAction action in actions.ToArray()) {
			if (action == null) {
				continue;
			}
			
			action.Update();
			if (action.isApplied) {
				actions.Remove(action);
			}
		}
	}
		
	protected abstract void UpdateVolume();
		
	protected abstract void UpdatePitch();

	/// <summary>
	/// Resumes the AudioItem if it has been paused.
	/// </summary>
	public virtual void Play(params AudioOption[] audioOptions) {
		State = AudioStates.Playing;
	}

	/// <summary>
	/// Pauses the AudioItem.
	/// </summary>
	public virtual void Pause(params AudioOption[] audioOptions) {
		State = AudioStates.Paused;
	}

	/// <summary>
	/// Stops the AudioItem with fade out. After being stopped, an AudioItem is obsolete.
	/// </summary>
	public virtual void Stop(params AudioOption[] audioOptions) {
		State = AudioStates.Stopped;
	}

	/// <summary>
	/// Stops the AudioItem immediatly without fade out. After being stopped, an AudioItem is obsolete.
	/// </summary>
	public virtual void StopImmediate() {
		State = AudioStates.Stopped;
	}

	/// <summary>
	/// Gets the volume of the AudioItem.
	/// </summary>
	/// <returns>The volume.</returns>
	public virtual float GetVolume() {
		return Volume;
	}

	/// <summary>
	/// Ramps the volume of the AudioItem.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be ramped.</param>
	/// <param name="time">The time it will take for the volume to reach the <paramref name="targetVolume"/>.</param>
	public abstract void SetVolume(float targetVolume, float time);
	
	/// <summary>
	/// Sets the volume of the AudioItem.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be set.</param>
	public abstract void SetVolume(float targetVolume);

	/// <summary>
	/// Gets the pitch of the AudioItem.
	/// </summary>
	/// <returns>The pitch.</returns>
	public virtual float GetPitch() {
		return Pitch;
	}
	
	/// <summary>
	/// Ramps the pitch of the AudioItem proportionally.
	/// </summary>
	/// <param name="targetPitch">The target to which the pitch will be ramped.</param>
	/// <param name="time">The time it will take for the pitch to reach the <paramref name="targetPitch"/>.</param>
	/// <param name="quantizeStep">The size of each pitch grid step in semi-tones.</param>
	/// <remarks>Note that using negative pitches will not work as expected.</remarks>
	public abstract void SetPitch(float targetPitch, float time, float quantizeStep);
	
	/// <summary>
	/// Ramps the pitch of the AudioItem proportionally.
	/// </summary>
	/// <param name="targetPitch">The target to which the pitch will be ramped.</param>
	/// <param name="time">The time it will take for the pitch to reach the <paramref name="targetPitch"/>.</param>
	/// <remarks>Note that using negative pitches will not work as expected.</remarks>
	public abstract void SetPitch(float targetPitch, float time);
	
	/// <summary>
	/// Sets the pitch of the AudioItem.
	/// </summary>
	/// <param name="targetPitch">The target to which the pitch will be set.</param>
	public abstract void SetPitch(float targetPitch);

	#region IEnumerators
	protected virtual IEnumerator RampVolume(float startVolume, float targetVolume, float time) {
		float counter = 0;
			
		while (counter < time) {
			Volume = ((counter / time) * (targetVolume - startVolume) + startVolume);
			UpdateVolume();
			counter += Time.deltaTime;
			yield return new WaitForSeconds(0);
		}
			
		Volume = targetVolume;
		UpdateVolume();
	}
		
	protected virtual IEnumerator RampPitch(float startPitch, float targetPitch, float time, float quantizeStep) {
		float counter = 0;
		float currentStep = 0;
		float currentRatio = 1;
		float direction = ((targetPitch - startPitch) / Mathf.Abs(targetPitch - startPitch)).Round();
		
		while (counter < time) {
			float rampPitch = startPitch * Mathf.Pow(targetPitch / startPitch, counter / time);
			
			if (quantizeStep <= 0) {
				Pitch = rampPitch;
			}
			else {
				float roundedPitchTarget = startPitch * currentRatio;
				if ((direction < 0 && rampPitch <= roundedPitchTarget) || (direction > 0 && rampPitch >= roundedPitchTarget)) {
					Pitch = roundedPitchTarget;
					currentStep += quantizeStep * direction;
					currentRatio = Mathf.Pow(2, currentStep / 12);
				}
			}
				
			UpdatePitch();
			counter += Time.deltaTime;
			yield return new WaitForSeconds(0);
		}
			
		Pitch = targetPitch;
		UpdatePitch();
	}
	#endregion
	
	public override string ToString() {
		return string.Format("{0}({1}, {2}, {3})", GetType().Name, Name, Id, State);
	}

}
