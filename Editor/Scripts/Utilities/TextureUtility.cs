using System;
using UnityEngine;
using Steamworks.Data;

namespace Intruder.Tools
{
	public static class TextureUtility
	{
		public static Texture2D Covert( this Image image )
		{
			var avatar = new Texture2D( (int)image.Width, (int)image.Height, TextureFormat.ARGB32, false );
			avatar.filterMode = FilterMode.Trilinear;

			// Flip image
			for ( int x = 0; x < image.Width; x++ )
			{
				for ( int y = 0; y < image.Height; y++ )
				{
					var p = image.GetPixel( x, y );
					avatar.SetPixel( x, (int)image.Height - y, new UnityEngine.Color( p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f ) );
				}
			}

			avatar.Apply();
			return avatar;
		}
	}
}
