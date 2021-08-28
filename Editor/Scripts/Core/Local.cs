using UnityEngine;
using Steamworks;

namespace Intruder.Tools
{
	public static class Local
	{
		//-------------------------------------------------------------//
		// Client Specific
		//-------------------------------------------------------------//
		public static string Username => SteamClient.IsValid ? SteamClient.Name : "No Name";
		public static SteamId SteamId => SteamClient.IsValid ? SteamClient.SteamId : 0;

		public static Texture2D Avatar { get; set; }
		public static int Followers { get; set; }
	}
}
