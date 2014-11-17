using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	public class Metronome : MonoBehaviour {

		[SerializeField, PropertyField(typeof(RangeAttribute), 0.1F, 1000F)]
		float beatsPerMinute = 120;
		public float BeatsPerMinute {
			get {
				return beatsPerMinute;
			}
			set {
				beatsPerMinute = value;
				beatDuration = 60F / beatsPerMinute;
				measureDuration = (60F / beatsPerMinute) * beatsPerMeasure;
			}
		}
		
		[SerializeField, PropertyField(typeof(MinAttribute), 1F)]
		int beatsPerMeasure = 4;
		public int BeatsPerMeasure {
			get {
				return beatsPerMeasure;
			}
			set {
				beatsPerMeasure = value;
				beatDuration = 60D / beatsPerMinute;
				measureDuration = (60D / beatsPerMinute) * beatsPerMeasure;
			}
		}

		public bool playOnAwake;

		int currentBeat;
		public int CurrentBeat { 
			get {
				return currentBeat;
			}
		}
		
		int currentMeasure;
		public int CurrentMeasure { 
			get {
				return currentMeasure;
			}
		}
	
		bool isPlaying;
		public bool IsPlaying {
			get {
				return isPlaying;
			}
		}
		
		double nextBeatTime;
		double nextMeasureTime;
		double beatDuration;
		double measureDuration;
		List<ISyncable> tickables = new List<ISyncable>();
		IEnumerator ticker;
	
		void Awake() {
			if (playOnAwake) {
				Play();
			}
		}
		
		void Update() {
			Tick();
		}
	
		void LateUpdate() {
			Tick();
		}
	
		void FixedUpdate() {
			Tick();
		}
	
		void OnGUI() {
			Tick();
		}
	
		void OnRenderObject() {
			Tick();
		}
		
		public void Play() {
			isPlaying = true;
			
			currentBeat = 0;
			currentMeasure = 0;
			nextBeatTime = AudioSettings.dspTime;
			nextMeasureTime = AudioSettings.dspTime;
			beatDuration = 60F / beatsPerMinute;
			measureDuration = (60F / beatsPerMinute) * beatsPerMeasure;
			ticker = Ticker();
		}
		
		public void Stop() {
			isPlaying = false;
		}
		
		public void Subscribe(ISyncable tickable) {
			tickables.Add(tickable);
		}
		
		public void Unsubscribe(ISyncable tickable) {
			tickables.Remove(tickable);
		}
		
		public void GetTempo(out float beatsPerMinute, out int beatsPerMeasure) {
			beatsPerMinute = BeatsPerMinute;
			beatsPerMeasure = BeatsPerMeasure;
		}
		
		public void SetTempo(float beatsPerMinute, int beatsPerMeasure) {
			BeatsPerMinute = beatsPerMeasure;
			BeatsPerMeasure = beatsPerMeasure;
		}

		public float GetAdjustedDelay(float delay, SyncMode syncMode) {
			float adjustedDelay = 0;
			
			switch (syncMode) {
				case SyncMode.None:
					adjustedDelay = delay;
					break;
				case SyncMode.Beat:
					adjustedDelay = (float)(nextBeatTime - AudioSettings.dspTime + delay * beatDuration);
					break;
				case SyncMode.Measure:
					adjustedDelay = (float)(nextMeasureTime - AudioSettings.dspTime + delay * measureDuration);
					break;
			}
			return adjustedDelay;
		}

		public int ConvertToBeats(float delay, SyncMode syncMode) {
			int beats = 0;
			
			switch (syncMode) {
				case SyncMode.None:
					beats = (int)delay.Round();
					break;
				case SyncMode.Beat:
					beats = (int)delay.Round();
					break;
				case SyncMode.Measure:
					beats = BeatsPerMeasure - (currentBeat + 1) + (int)(delay * BeatsPerMeasure).Round();
					break;
			}
			return beats;
		}
		
		void Tick() {
			if (isPlaying) {
				TickEvent();
				ticker.MoveNext();
			}
		}
		
		void TickEvent() {
			for (int i = 0; i < tickables.Count; i++) {
				if (tickables[i] != null) {
					tickables[i].TickEvent();
				}
			}
		}
		
		void BeatEvent() {
			for (int i = 0; i < tickables.Count; i++) {
				if (tickables[i] != null) {
					tickables[i].BeatEvent(CurrentBeat);
				}
			}
		}

		void MeasureEvent() {
			for (int i = 0; i < tickables.Count; i++) {
				if (tickables[i] != null) {
					tickables[i].MeasureEvent(CurrentMeasure);
				}
			}
		}
	
		IEnumerator Ticker() {
			while (true) {
				double currentTime = AudioSettings.dspTime;
				if (currentTime >= nextBeatTime) {
					if (CurrentBeat == 0) {
						currentMeasure += 1;
						nextMeasureTime += measureDuration;
						MeasureEvent();
					}
				
					currentBeat = (CurrentBeat + 1) % beatsPerMeasure;
					nextBeatTime += beatDuration;
					BeatEvent();
				}
				yield return null;
			}
		}
	}
}
