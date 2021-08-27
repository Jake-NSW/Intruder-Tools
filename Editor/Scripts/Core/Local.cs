using UnityEngine;
using Steamworks;

namespace Intruder.Tools
{
	public static class Local
	{
		//-------------------------------------------------------------//
		// Client Specific
		//-------------------------------------------------------------//
		public static string Username => SteamClient.Name;
		public static SteamId SteamId => SteamClient.SteamId;
		
		public static Texture2D Avatar { get; set; }
		public static int Followers { get; set; }
	}
}
