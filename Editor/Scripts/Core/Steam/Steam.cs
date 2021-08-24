using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Steamworks;
using Steamworks.Data;
using Steamworks.Ugc;

namespace Intruder.Tools.Steamworks
{
	public static class Steam
	{
		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			SteamClient.Init( Global.AppId );

			if ( SteamClient.IsValid )
			{
				PreCache();

				// I have to do this? Or else it'll just disappear? why why why???
				// You'd think i would have to do this on every other GUI image...
				// but no only on the avatar cause unity doesnt like steam avatars
				EditorSceneManager.sceneOpened += ( e, d ) => GetAndCacheAvatar();
			}
			else
				Debug.LogError( "Steam Client not valid" );
		}

		//-------------------------------------------------------------//
		// Cache
		//-------------------------------------------------------------//
		private static async void PreCache()
		{
			// Get tasks
			var avatar = GetAvatar();
			var workshopItems = GetWorkshopItems();
			var followers = GetFollowers();

			await Task.WhenAll( avatar, workshopItems, followers );

			// Cache Items
			Local.Avatar = avatar.Result?.Covert();
			Local.Followers = followers.Result.Value;
			Workshop.ClientItems = workshopItems.Result?.Entries.ToList();
		}

		public static async void GetAndCacheAvatar()
		{
			var avatar = await GetAvatar();
			Local.Avatar = avatar?.Covert();
		}

		public static async Task<Image?> GetAvatar()
		{
			try
			{
				return await SteamFriends.GetLargeAvatarAsync( SteamClient.SteamId );
			}
			catch ( Exception e )
			{
				Debug.Log( e );
				return null;
			}
		}

		public static async Task<int?> GetFollowers()
		{
			try
			{
				return await SteamFriends.GetFollowerCount( Local.SteamId );
			}
			catch ( Exception e )
			{
				Debug.Log( e );
				return null;
			}
		}

		public static async Task<ResultPage?> GetWorkshopItems()
		{
			try
			{
				var query = Query.Items
								.WhereUserPublished( Local.SteamId )
								.SortByTitleAsc();


				return await query.GetPageAsync( 1 );
			}
			catch ( Exception e )
			{
				Debug.Log( e );
				return null;
			}
		}
	}
}
