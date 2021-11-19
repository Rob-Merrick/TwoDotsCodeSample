using System;
using UnityEngine;

public static class MusicalScale
{
	/// <summary>
	/// Returns the pitch shift needed from an arbitrary root note (pitch shift 1) based on
	/// the given <paramref name="steps"/> away from the root note to produce a diatonic scale.
	/// For example, 0 steps will return a pitch shift of 1. A steps value of 1 will return 1.125,
	/// which corresponds to the next note up in the scale. A steps value of -1 will return 0.9167,
	/// which corresponds to the note just below the root note. A steps value of 7 will return 2.0,
	/// respresenting the note that is one octave higher than the root note. A diatonic scale can be
	/// thought of as the white notes on a piano if the key signature has no sharps and flats.
	/// </summary>
	public static float GetDiatonicPitchShift(int steps)
	{
		int stepInOctave = Mathf.FloorToInt(Mathf.Repeat(steps, 7));
		float octaveLeap = Mathf.Pow(2, Mathf.Floor(steps / 7.0F));

		switch(Math.Abs(stepInOctave))
		{
			case 0: return 1.0F / 1.0F * octaveLeap;
			case 1: return 9.0F / 8.0F * octaveLeap;
			case 2: return 5.0F / 4.0F * octaveLeap;
			case 3: return 4.0F / 3.0F * octaveLeap;
			case 4: return 3.0F / 2.0F * octaveLeap;
			case 5: return 5.0F / 3.0F * octaveLeap;
			case 6: return 15.0F / 8.0F * octaveLeap;
			default: throw new Exception("There is no note in this octave");
		}
	}
}
