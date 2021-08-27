using Steamworks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Intruder.Tools.Steamworks
{
	public class WorkshopItemsDropdown : AdvancedDropdown
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
				activeUploader.SelectItem( workshopItem );
			}
			else
			{
				activeUploader.RemoveActiveItem();
			}
		}

		protected override AdvancedDropdownItem BuildRoot()
		{
			var root = new AdvancedDropdownItem( $"{SteamClient.Name} Workshop Items" );
			root.AddChild( new AdvancedDropdownItem( "New Workshop Item" ) { id = -1, icon = (Texture2D)EditorGUIUtility.IconContent( "d_CreateAddNew" ).image } );
			root.AddSeparator();

			foreach ( var item in Workshop.ClientItems )
				root.AddChild( new AdvancedDropdownItem( string.IsNullOrEmpty( item.Title ) ? "Untitled" : item.Title ) { id = Workshop.ClientItems.IndexOf( item ) } );

			return root;
		}
	}
}
