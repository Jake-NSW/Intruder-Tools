using System.Threading.Tasks;
using Steamworks.Data;
using Steamworks.Ugc;

namespace Intruder.Tools
{
	public static class PublishResultUtility
	{
		public static async void OnFinishUpload( this PublishResult publishResult )
		{
			var item = await Item.GetAsync( publishResult.FileId );
			await Task.WhenAll( item.Value.Subscribe(), item.Value.Vote( true ), item.Value.AddFavorite() );
		}
	}
}
