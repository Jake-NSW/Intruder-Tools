using System.Threading.Tasks;
using Steamworks.Data;
using Steamworks.Ugc;

namespace Intruder.Tools.Steamworks
{
	public static class PublishResultUtility
	{
		public static async void OnFinishUpload( this PublishResult publishResult, string finishMessage )
		{
			var item = await Item.GetAsync( publishResult.FileId );
			await Task.WhenAll( item.Value.Subscribe(), item.Value.Vote( true ), item.Value.AddFavorite() );
			Workshop.RefreshClientItems( publishResult );
			PublishProgress.progress = 0;

			if ( !UnityEditor.EditorUtility.DisplayDialog( "Upload Complete!", finishMessage, "Okay", "View on Workshop" ) )
			{
				UnityEngine.Application.OpenURL( $"steam://openurl/{item.Value.Url}" );
			}
		}
	}
}
