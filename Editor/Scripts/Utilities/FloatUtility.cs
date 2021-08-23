using System;
using UnityEngine;

namespace Intruder.Tools
{
	public static class FloatUtility
	{
		public static float LerpTo( this float lerp, float lerpTo, float delta )
		{
			return Mathf.Lerp( lerp, lerpTo, delta );
		}

		public static float Remap( this float input, float inputMin, float inputMax, float min, float max )
		{
			return min + (input - inputMin) * (max - min) / (inputMax - inputMin);
		}
	}
}
