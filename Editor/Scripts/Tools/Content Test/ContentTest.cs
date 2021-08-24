using System;
using System.Diagnostics;
using UnityEngine;
using Steamworks;

namespace Intruder.Tools.Testing
{
	public class ContentTest
	{
		public static Process LaunchIntruder( string launchArgs = "" )
		{
			// Kills the current intruder process
			foreach ( var item in Process.GetProcessesByName( "Intruder" ) )
				item.Kill();

			// Start Intruder
			return Process.Start( SteamApps.AppInstallDir() + "/IntruderLauncher.exe", launchArgs );
		}

		public static void LaunchIntruderThroughSteam()
		{
			// Kills the current intruder process
			foreach ( var item in Process.GetProcessesByName( "Intruder" ) )
				item.Kill();

			// Start Intruder
			Application.OpenURL( $"steam://rungameid/{Global.AppId}" );
		}

		public static string LoadLevelArgs( string path )
		{
			switch ( Application.platform )
			{
				case (RuntimePlatform.WindowsPlayer):
				case (RuntimePlatform.WindowsEditor):
					return $"loadlevel \"{path}/map.ilfw\"";

				case (RuntimePlatform.OSXPlayer):
				case (RuntimePlatform.OSXEditor):
					return $"loadlevel \"{path}/map.ilfm\"";

				default:
					return "";
			}
		}
	}
}
