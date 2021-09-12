using System.IO;
using Steamworks;
using Steamworks.Ugc;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using Intruder.Tools.IMGUI;
using Intruder.Tools.IMGUI.Controls;

namespace Intruder.Tools.Steamworks
{
	[CustomTool( "Steam Uploader", Title = "Workshop Uploader", Description = "Use this tool to update your workshop items and update item thumbnails", Tooltip = "Upload your Content!", HasOptions = true, Priority = -100 )]
	public sealed class WorkshopUploader : Tool, IPackageDirectory
	{
		public static WorkshopUploader current;
		public static Item? activeItem;
		public string cachedDirectory;

		private string cachedThumbnailPath;
		private Texture2D cachedThumbnail;
		private string cachedName;
		private string cachedChangelog;

		private static bool uploadGroupFoldout = true;
		private static bool statsGroupFoldout;

		public WorkshopUploader()
		{
			current = this;
			GetDefaultThumbnail();
		}

		public override void InspectorGUI()
		{
			// If steam is not connected.. Don't draw GUI
			if ( !SteamClient.IsValid )
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

		public override void OptionsGUI()
		{
			UtilityGUI.Panel( () =>
			{
				if ( GUILayout.Button( "Refresh Workshop Items" ) )
				{
					Workshop.RefreshClientItems();
				}
			} );
		}

		//-------------------------------------------------------------//
		// Workshop Upload / Update GUI
		//-------------------------------------------------------------//
		public void SelectItem( Item item )
		{
			// Return if selecting the same item
			if ( activeItem?.Id == item.Id )
				return;

			activeItem = item;
			cachedName = item.Title;
			cachedThumbnailPath = null;
			GetItemThumbnail( item );
			Window.current.Repaint();
		}

		public void RemoveActiveItem()
		{
			activeItem = null;
			cachedName = string.Empty;
			GetDefaultThumbnail();
			Window.current.Repaint();
		}

		private void UploadItemGUI()
		{
			// Item Meta
			EditorGUI.BeginDisabledGroup( PublishProgress.progress > 0 );
			{
				using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
				{
					ItemSelectorBarGUI();
					GUILayout.Space( 4 );
					using ( new GUILayout.HorizontalScope( GUILayout.ExpandHeight( true ) ) )
					{
						ThumbnailPreviewGUI();
						using ( new GUILayout.VerticalScope( GUILayout.ExpandHeight( true ) ) )
						{
							UtilityGUI.GhostedTextField( ref cachedName, true, "Item Name", 12, GUILayout.Height( 26 ) );
							UtilityGUI.GhostedTextArea( ref cachedChangelog, true, "Item Changelog", 12, GUILayout.Height( 127 ) );
						}
					}
				}
			}
			EditorGUI.EndDisabledGroup();

			// Item Uploader
			using ( new GUILayout.VerticalScope( Styles.Panel, GUILayout.ExpandHeight( true ) ) )
			{
				// Publish progress
				if ( PublishProgress.progress > 0 )
				{
					var prog = PublishProgress.progress;
					EditorGUI.ProgressBar( EditorGUILayout.GetControlRect( GUILayout.Height( 32 ) ), prog, $"Upload Progress - {prog * 100}%" );
					return;
				}

				var content = (string.IsNullOrEmpty( cachedDirectory )) ? new GUIContent( "No Package Selected" ) : new GUIContent( Path.GetFileName( cachedDirectory ) );
				var style = new GUIStyle( EditorStyles.popup ) { fixedHeight = 0 };

				var rect = GUILayoutUtility.GetRect( content, style, GUILayout.Height( 20 ) );
				if ( GUI.Button( rect, content, style ) )
				{
					var dropdown = new PackagesDropdown( new AdvancedDropdownState(), this );
					dropdown.Show( rect );
				}

				if ( string.IsNullOrEmpty( cachedDirectory ) )
					EditorGUILayout.HelpBox( "Need to have a selected package in the field above, in order to publish an item", MessageType.Error );

				using ( new GUILayout.HorizontalScope( GUILayout.ExpandHeight( true ) ) )
				{
					EditorGUI.BeginDisabledGroup( string.IsNullOrEmpty( cachedDirectory ) );
					{
						if ( GUILayout.Button( (activeItem == null) ? "Upload" : "Update Item", GUILayout.Height( 24 ) ) )
						{
							var path = Path.GetFullPath( cachedDirectory );

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
						EditorGUI.BeginDisabledGroup( string.IsNullOrEmpty( cachedThumbnailPath ) );
						{
							if ( GUILayout.Button( "Update Thumbnail!", GUILayout.Height( 24 ) ) )
							{
								Workshop.UpdateThumbnail( activeItem.Value, cachedThumbnailPath );
							}
						}
						EditorGUI.EndDisabledGroup();
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
				GUILayout.Label( new GUIContent( cachedThumbnail, (string.IsNullOrEmpty( cachedThumbnailPath )) ? Path.GetFullPath( cachedThumbnailPath ) : null ), GUI.skin.box, GUILayout.Height( 128 ), GUILayout.Width( 128 ) );
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

			// If thumbnail is over 3mb then return
			if ( FileUtility.IsOverSize( path, FileSize.Megabytes, 3 ) )
			{
				EditorUtility.DisplayDialog( "Warning", "Steam does not allow for thumbnails to be over 3mb in file size", "Okay" );
				return;
			}

			cachedThumbnailPath = path;
			cachedThumbnail = new Texture2D( 4, 4 );
			cachedThumbnail.LoadImage( File.ReadAllBytes( path ) );
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

		void IPackageDirectory.OnNothingSelected()
		{
			cachedDirectory = null;
		}

		void IPackageDirectory.OnSkinSelected( string path )
		{
			cachedDirectory = path;
		}

		void IPackageDirectory.OnMapSelected( string path )
		{
			cachedDirectory = path;
		}
	}
}
