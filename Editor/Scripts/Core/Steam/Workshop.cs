using System;
using System.Linq;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Ugc;
using UnityEngine;

namespace Intruder.Tools.Steamworks
{
	public static class Workshop
	{
		public static List<Item> ClientItems = new List<Item>();

		public static async void RefreshClientItems( PublishResult? selectItem = null )
		{
			var workshopItems = await Steam.GetWorkshopItems();
			Workshop.ClientItems = workshopItems?.Entries.ToList();

			if ( selectItem == null )
			{
				WorkshopUploader.current.RemoveActiveItem();
				return;
			}

			WorkshopUploader.current.cachedDirectory = null;
			WorkshopUploader.current.SelectItem( ClientItems.Where( e => e.Id == selectItem.Value.FileId ).FirstOrDefault() );
		}

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
				file.OnFinishUpload( $"Finished uploading new ready to use item: {name}" );
			else
				Debug.LogError( file.Result.ToString() );
		}

		private static async void CreateMicrotransactionItem( string content, string name, string changeLog, string thumbnailPath )
		{
			var file = await Editor.NewMicrotransactionFile.WithTitle( name ).WithContent( content ).WithPreviewFile( thumbnailPath ).WithChangeLog( changeLog ).SubmitAsync( new PublishProgress() );
			if ( file.Success )
				file.OnFinishUpload( $"Finished uploading new microtransaction item: {name}" );
			else
				Debug.LogError( file.Result.ToString() );
		}

		public static async void UpdateItem( string content, Item item, string changeLog )
		{
			try
			{
				var file = await new Editor( item.Id ).WithContent( content ).WithChangeLog( changeLog ).SubmitAsync( new PublishProgress() );
				if ( file.Success )
					file.OnFinishUpload( $"Successfully updated item: {item.Title}" );
				else
					Debug.LogError( file.Result.ToString() );
			}
			catch ( Exception e )
			{
				Debug.Log( e );
			}
		}

		public static async void UpdateThumbnail( Item item, string thumbnailPath )
		{
			try
			{
				var file = await new Editor( item.Id ).WithPreviewFile( thumbnailPath ).SubmitAsync( new PublishProgress() );
				if ( file.Success )
					file.OnFinishUpload( $"Thumbnail updated on item: {item.Title}" );
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
