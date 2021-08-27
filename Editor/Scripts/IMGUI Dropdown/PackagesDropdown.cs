using System.IO;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Intruder.Tools.Steamworks
{
	public class PackagesDropdown : AdvancedDropdown
	{
		private WorkshopUploader activeUploader;

		public PackagesDropdown( AdvancedDropdownState state, WorkshopUploader tool ) : base( state )
		{
			this.activeUploader = tool;
			this.minimumSize = new Vector2( 0, 200 );
		}

		protected override void ItemSelected( AdvancedDropdownItem item )
		{
			var projectPath = Path.GetFullPath( Application.dataPath + "/../" );

			switch ( item.id )
			{
				case 0:
					activeUploader.cachedDirectory = null;
					break;
				// Item is map
				case 1:
					activeUploader.cachedDirectory = Path.GetFullPath( projectPath + "/Exports/Maps/" + item.name );
					break;

				// Item is Skin
				case 2:
					activeUploader.cachedDirectory = Path.GetFullPath( projectPath + "/Exports/Skins/" + item.name );
					break;
			}
		}

		protected override AdvancedDropdownItem BuildRoot()
		{
			var root = new AdvancedDropdownItem( $"Select a Package" );
			var projectPath = Path.GetFullPath( Application.dataPath + "/../" );

			root.AddChild( new AdvancedDropdownItem( "None" ) { id = 0 } );
			root.AddSeparator();

			if ( Directory.Exists( projectPath + "/Exports/Maps" ) )
			{
				var maps = new AdvancedDropdownItem( "Map Packages" );

				foreach ( var item in Directory.GetDirectories( projectPath + $"/Exports/Maps" ) )
					maps.AddChild( new AdvancedDropdownItem( Path.GetFileName( item ) ) { id = 1 } );

				root.AddChild( maps );
			}

			if ( Directory.Exists( projectPath + "/Exports/Skins" ) )
			{
				var skins = new AdvancedDropdownItem( "Skin Packages" );

				foreach ( var item in Directory.GetDirectories( projectPath + $"/Exports/Skins" ) )
					skins.AddChild( new AdvancedDropdownItem( Path.GetFileName( item ) ) { id = 2 } );

				root.AddChild( skins );
			}

			return root;
		}
	}
}
