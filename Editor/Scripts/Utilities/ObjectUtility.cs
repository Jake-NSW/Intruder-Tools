using System;
using UnityEngine;

namespace Intruder.Tools
{
	public static class ObjectUtility
	{
		public static string Dump( this object text )
		{
			Debug.Log( text.ToString() );
			return text.ToString();
		}
	}
}
