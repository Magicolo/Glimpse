using UnityEngine;
using System.Collections;

namespace Magicolo.GeneralTools {
	public interface ITickable {
		
		void TickEvent();
		
		void BeatEvent(int currentBeat);
		
		void MeasureEvent(int currentMeasure);
	}
}