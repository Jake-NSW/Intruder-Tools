//-------------------------------------------------------------//
// Intruder Tools - Created by Superboss Games.
// Copyright Superboss Games 2021.
//-------------------------------------------------------------//

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Steamworks;
using Steamworks.Data;
using Steamworks.Ugc;
using Editor = Steamworks.Ugc.Editor;

namespace Intruder.Tools
{
	public static class Steam
	{
		//-------------------------------------------------------------//
		// Entry Point
		//-------------------------------------------------------------//
		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			SteamClient.Init( Global.AppId );

			if ( SteamClient.IsValid )
			{
				PreCache();
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

		public static async void RefreshClientItems()
		{
			var workshopItems = await GetWorkshopItems();
			Workshop.ClientItems = workshopItems?.Entries.ToList();
		}

		private static async Task<Image?> GetAvatar()
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

		private static async Task<int?> GetFollowers()
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

		private static async Task<ResultPage?> GetWorkshopItems()
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


		//-------------------------------------------------------------//
		// Workshop Uploader Tool
		//-------------------------------------------------------------//
		[CustomTool( "Steam Uploader", Title = "Workshop Uploader", Description = "Use this tool to update your workshop items and update item thumbnails", Tooltip = "Upload your Content!", Priority = -100 )]
		private class WorkshopUploader : Tool
		{
			public Item? activeItem;

			public string cachedThumbnailPath;
			public Texture2D cachedThumbnail;
			public string cachedName;
			public string cachedChangelog;
			public DefaultAsset cachedAsset;

			private static bool uploadGroupFoldout = true;
			private static bool statsGroupFoldout;

			public WorkshopUploader()
			{
				GetDefaultThumbnail();
			}

			public override void InspectorGUI()
			{
				// If steam is not connected.. Don't draw GUI
				if ( !Steamworks.SteamClient.IsValid )
				{
					GUILayout.Label( "Steam not connected!", Styles.SubTitle );
					return;
				}

				// Upload Item
				using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
				{
					// Foldout group
					uploadGroupFoldout = EditorGUILayout.BeginFoldoutHeaderGroup( uploadGroupFoldout, " Upload Item", Styles.FoldoutSubTitle );
					EditorGUILayout.EndFoldoutHeaderGroup();

					if ( uploadGroupFoldout )
						UploadItemGUI();
				}

				// Workshop Statistics
				using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
				{
					// Foldout group
					statsGroupFoldout = EditorGUILayout.BeginFoldoutHeaderGroup( statsGroupFoldout, " Workshop Statistics", Styles.FoldoutSubTitle );
					EditorGUILayout.EndFoldoutHeaderGroup();

					if ( statsGroupFoldout )
						StatisticsGUI();
				}
			}

			//-------------------------------------------------------------//
			// Workshop Upload / Update GUI
			//-------------------------------------------------------------//
			private void UploadItemGUI()
			{
				// Item Meta
				using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
				{
					ItemSelectorBarGUI();
					GUILayout.Space( 4 );
					using ( new GUILayout.HorizontalScope( GUILayout.ExpandHeight( true ) ) )
					{
						ThumbnailPreviewGUI();
						using ( new GUILayout.VerticalScope( GUILayout.ExpandHeight( true ) ) )
						{
							ToolGUI.GhostedTextField( ref cachedName, true, "Item Name", 12, GUILayout.Height( 26 ) );
							ToolGUI.GhostedTextArea( ref cachedChangelog, true, "Item Changelog", 12, GUILayout.Height( 127 ) );
						}
					}
				}

				// Item Uploader
				using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
				{
					cachedAsset = EditorGUILayout.ObjectField( cachedAsset, typeof( DefaultAsset ), false ) as DefaultAsset;

					if ( cachedAsset == null )
						EditorGUILayout.HelpBox( "Need to have an object in the field above, in order to publish an item", MessageType.Error );

					using ( new GUILayout.HorizontalScope( GUILayout.ExpandHeight( true ) ) )
					{
						EditorGUI.BeginDisabledGroup( cachedAsset == null );
						{
							if ( GUILayout.Button( "Upload" ) )
							{
								var path = Path.GetFullPath( AssetDatabase.GetAssetPath( cachedAsset ) );
								Debug.Log( path );
								Debug.Log( Path.GetFullPath( cachedThumbnailPath ) );

								// UPLOAD ITEM LOGIC
								if ( activeItem == null )
									Workshop.PublishNewItem( path, UgcType.Items_ReadyToUse, cachedName, cachedChangelog, Path.GetFullPath( cachedThumbnailPath ) );
								else
									Workshop.UpdateItem( path, activeItem.Value, cachedChangelog );
							}
						}
						EditorGUI.EndDisabledGroup();

						// Update thumbnail
						if ( activeItem != null )
						{
							if ( GUILayout.Button( "Update Thumbnail!" ) )
							{
								Workshop.UpdateThumbnail( activeItem.Value, cachedThumbnailPath );
							}
						}
					}
				}
			}

			private void ItemSelectorBarGUI()
			{
				using ( new GUILayout.HorizontalScope() )
				{
					var content = (activeItem == null) ? new GUIContent( "Upload new Item?" ) : new GUIContent( activeItem?.Title );
					var style = new GUIStyle( EditorStyles.popup ) { fixedHeight = 0 };

					var rect = GUILayoutUtility.GetRect( content, style, GUILayout.Height( 20 ) );
					if ( GUI.Button( rect, content, style ) )
					{
						var dropdown = new WorkshopItemsDropdown( new AdvancedDropdownState(), this );
						dropdown.Show( rect );
					}

					if ( activeItem != null )
					{
						if ( GUILayout.Button( "View", GUILayout.Width( 96 ), GUILayout.Height( 20 ) ) )
						{
							Application.OpenURL( "steam://openurl/" + activeItem?.Url );
						}
					}
				}
			}

			private void ThumbnailPreviewGUI()
			{
				using ( new GUILayout.VerticalScope( GUILayout.MaxHeight( 144 ), GUILayout.Width( 128 ) ) )
				{
					GUILayout.Label( new GUIContent( cachedThumbnail, (cachedThumbnailPath != null) ? Path.GetFullPath( cachedThumbnailPath ) : null ), GUI.skin.box, GUILayout.Height( 128 ), GUILayout.Width( 128 ) );
					if ( GUILayout.Button( "Change Thumbnail", GUILayout.ExpandHeight( true ), GUILayout.ExpandWidth( true ) ) )
					{
						CacheThumbnail();
					}
				}
			}

			private void CacheThumbnail()
			{
				var path = EditorUtility.OpenFilePanel( "New Thumbnail Image", Application.dataPath, "png" );
				if ( string.IsNullOrEmpty( path ) )
					return;

				cachedThumbnailPath = path;
				cachedThumbnail = new Texture2D( 4, 4 );
				cachedThumbnail.LoadImage( File.ReadAllBytes( path ) );
			}

			//-------------------------------------------------------------//
			// Cool IMGUI Dropdown for Workshop Items
			//-------------------------------------------------------------//
			private class WorkshopItemsDropdown : AdvancedDropdown
			{
				private WorkshopUploader activeUploader;

				public WorkshopItemsDropdown( AdvancedDropdownState state, WorkshopUploader tool ) : base( state )
				{
					this.activeUploader = tool;
					this.minimumSize = new Vector2( 0, 200 );
				}

				protected override void ItemSelected( AdvancedDropdownItem item )
				{
					if ( !item.enabled )
						return;

					if ( item.id != -1 )
					{
						var workshopItem = Workshop.ClientItems[item.id];

						// Return if selecting the same item
						if ( activeUploader.activeItem?.Id == workshopItem.Id )
							return;

						activeUploader.activeItem = workshopItem;
						activeUploader.cachedName = workshopItem.Title;
						activeUploader.cachedThumbnailPath = null;
						activeUploader.GetItemThumbnail( workshopItem );
					}
					else
					{
						activeUploader.cachedName = string.Empty;
						activeUploader.activeItem = null;
						activeUploader.GetDefaultThumbnail();
					}
				}

				protected override AdvancedDropdownItem BuildRoot()
				{
					var root = new AdvancedDropdownItem( $"{Steamworks.SteamClient.Name} Workshop Items" );
					root.AddChild( new AdvancedDropdownItem( "New Workshop Item" ) { id = -1, icon = (Texture2D)EditorGUIUtility.IconContent( "d_CreateAddNew" ).image } );
					root.AddSeparator();

					foreach ( var item in Workshop.ClientItems )
					{
						root.AddChild( new AdvancedDropdownItem( string.IsNullOrEmpty( item.Title ) ? "Untitled" : item.Title ) { id = Workshop.ClientItems.IndexOf( item ) } );
					}

					return root;
				}
			}

			//-------------------------------------------------------------//
			// Workshop Statistics GUI
			//-------------------------------------------------------------//
			private void StatisticsGUI()
			{
				GUILayout.Space( 4 );
				if ( GUILayout.Button( "Refresh", GUILayout.Height( 24 ) ) )
				{
					Statstics.RefreshStatstics();
				}

				using ( new GUILayout.VerticalScope( Styles.Panel ) )
				{
					GUILayout.Label( "Combined Stats", Styles.SubTitle );

					GUI.enabled = false;
					EditorGUILayout.LongField( "Views", (long)Statstics.CombinedViews );
					EditorGUILayout.LongField( "Subscribers", (long)Statstics.CombinedSubscribers );
					EditorGUILayout.LongField( "Unique Subscribers", (long)Statstics.CombinedUniqueSubscribers );
					EditorGUILayout.LongField( "Favorites", (long)Statstics.CombinedFavorites );
					GUI.enabled = true;
				}

				if ( activeItem != null )
				{
					using ( new GUILayout.VerticalScope( Styles.Panel ) )
					{
						GUILayout.Label( $"{activeItem?.Title} Stats", Styles.SubTitle );

						GUI.enabled = false;
						EditorGUILayout.LongField( "Views", (long)activeItem?.NumUniqueWebsiteViews );
						EditorGUILayout.LongField( "Subscribers", (long)activeItem?.NumSubscriptions );
						EditorGUILayout.LongField( "Unique Subscribers", (long)activeItem?.NumUniqueSubscriptions );
						EditorGUILayout.LongField( "Favorites", (long)activeItem?.NumFavorites );
						GUI.enabled = true;
					}
				}
			}

			private static class Statstics
			{
				public static ulong CombinedViews { get; set; }
				public static ulong CombinedSubscribers { get; set; }
				public static ulong CombinedUniqueSubscribers { get; set; }
				public static ulong CombinedFavorites { get; set; }

				public static void RefreshStatstics()
				{
					CombinedViews = 0;
					CombinedSubscribers = 0;
					CombinedFavorites = 0;
					CombinedUniqueSubscribers = 0;

					Workshop.ClientItems.ForEach( item =>
					{
						CombinedViews += item.NumUniqueWebsiteViews;
						CombinedSubscribers += item.NumSubscriptions;
						CombinedUniqueSubscribers += item.NumUniqueSubscriptions;
						CombinedFavorites += item.NumFavorites;
					} );
				}
			}

			//-------------------------------------------------------------//
			// Class Specific Utility
			//-------------------------------------------------------------//
			public void GetItemThumbnail( Item item )
			{
				if ( string.IsNullOrEmpty( item.PreviewImageUrl ) )
					return;


				var www = UnityWebRequestTexture.GetTexture( item.PreviewImageUrl );
				var result = www.SendWebRequest();

				result.completed += ( e ) =>
				{
					var www = e as UnityWebRequestAsyncOperation;
					cachedThumbnail = ((DownloadHandlerTexture)www.webRequest.downloadHandler).texture;
					cachedThumbnailPath = null;
				};
			}

			public void GetDefaultThumbnail()
			{
				cachedThumbnail = Window.Icons.DefaultThumbnail;
				cachedThumbnailPath = AssetDatabase.GetAssetPath( Window.Icons.DefaultThumbnail );
			}
		}

		//-------------------------------------------------------------//
		// Workshop
		//-------------------------------------------------------------//
		public static class Workshop
		{
			public static List<Item> ClientItems = new List<Item>();

			//-------------------------------------------------------------//
			// Workshop Uploader Progress
			//-------------------------------------------------------------//
			public class PublishProgress : IProgress<float>
			{
				public static float progress = 0;

				public void Report( float value )
				{
					if ( progress >= value ) return;
					Debug.Log( progress );
					progress = value;
				}
			}

			public static void PublishNewItem( string content, UgcType type, string name, string changeLog, string thumbnailPath )
			{
				switch ( type )
				{
					// Normal Workshop item
					case UgcType.Items:
					case UgcType.Items_ReadyToUse:
						CreateReadyToUseItem( content, name, changeLog, thumbnailPath );
						break;

					// Curated Item
					case (UgcType.Items_Mtx):
						CreateMicrotransactionItem( content, name, changeLog, thumbnailPath );
						break;

					// Fall through						
					default:
						throw new Exception( $"Can't use UgcType {type.ToString()}" );
				}
			}

			private static async void CreateReadyToUseItem( string content, string name, string changeLog, string thumbnailPath )
			{
				var file = await Editor.NewCommunityFile.WithTitle( name ).WithContent( content ).WithPreviewFile( thumbnailPath ).WithChangeLog( changeLog ).SubmitAsync( new PublishProgress() );
				if ( file.Success )
					file.OnFinishUpload();
				else
					Debug.LogError( file.Result.ToString() );
			}

			private static async void CreateMicrotransactionItem( string content, string name, string changeLog, string thumbnailPath )
			{
				var file = await Editor.NewMicrotransactionFile.WithTitle( name ).WithContent( content ).WithPreviewFile( thumbnailPath ).WithChangeLog( changeLog ).SubmitAsync( new PublishProgress() );
				if ( file.Success )
					file.OnFinishUpload();
				else
					Debug.LogError( file.Result.ToString() );
			}

			public static async void UpdateItem( string content, Item item, string changeLog, IProgress<float> progress = null )
			{
				try
				{
					var file = await new Editor( item.Id ).WithContent( content ).WithChangeLog( changeLog ).SubmitAsync( new PublishProgress() );
					if ( file.Success )
						file.OnFinishUpload();
					else
						Debug.LogError( file.Result.ToString() );
				}
				catch ( Exception e )
				{
					Debug.Log( e );
				}
			}

			public static async void UpdateThumbnail( Item item, string thumbnailPath, IProgress<float> progress = null )
			{
				try
				{
					var file = await new Editor( item.Id ).WithPreviewFile( thumbnailPath ).SubmitAsync( progress );
					if ( file.Success )
						file.OnFinishUpload();
					else
						Debug.LogError( file.Result.ToString() );
				}
				catch ( Exception e )
				{
					Debug.Log( e );
				}
			}
		}
	}
}
