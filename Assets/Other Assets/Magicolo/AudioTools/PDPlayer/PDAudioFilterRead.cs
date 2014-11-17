using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using LibPDBinding;

namespace Magicolo.AudioTools {
	public class PDAudioFilterRead : MonoBehaviour {

		[HideInInspector] public bool readerSwitch;
		[HideInInspector] public bool initialized;
		[HideInInspector] public bool applicationFocused;
		[HideInInspector] public bool editorPaused;
		[HideInInspector] public PDPlayer pdPlayer;
	
		GCHandle dataHandle;
		IntPtr dataPtr;
		
		public static float[] dataSum = new float[0];
	
		void Start() {
			dataSum = new float[pdPlayer.generalSettings.maxVoices * pdPlayer.bridge.bufferSize * 2];
			initialized = true;
		}
		
		void OnDestroy() {
			dataHandle.Free();
			dataPtr = IntPtr.Zero;
		}
		
		void OnApplicationFocus(bool focus) {
			applicationFocused = focus || Application.isEditor;
		}
		
		void OnAudioFilterRead(float[] data, int channels) {
			if (pdPlayer.initialized) {
				if (dataPtr == IntPtr.Zero) {
					dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
					dataPtr = dataHandle.AddrOfPinnedObject();
				}
			
				if (PDGainManager.soundNameVoice.Count > 0 && !readerSwitch) {
					readerSwitch = true;
					pdPlayer.communicator.SendValue("UReaderSwitch", 1);
				}
				else if (PDGainManager.soundNameVoice.Count == 0 && readerSwitch) {
					readerSwitch = false;
					pdPlayer.communicator.SendValue("UReaderSwitch", 0);
				}
			
				if (pdPlayer.bridge.initialized && applicationFocused && !editorPaused) {
					pdPlayer.communicator.WriteArray("UMasterReceive", dataSum);
					LibPD.Process(pdPlayer.bridge.ticks, dataPtr, dataPtr);
				}
			}
		}
	}
}
