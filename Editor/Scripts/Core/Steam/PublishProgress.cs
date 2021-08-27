using System;
using UnityEngine;

namespace Intruder.Tools.Steamworks
{
	public class PublishProgress : IProgress<float>
	{
		public static float progress = 0;

		public void Report( float value )
		{
			if ( progress == value )
				return;

			Window.current.Repaint();
			progress = value;
		}
	}
}
