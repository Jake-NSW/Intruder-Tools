using System.IO;

namespace Intruder.Tools
{
	public static class FileUtility
	{
		public static void DirectoryCheck( string path )
		{
			if ( !Directory.Exists( path ) )
				Directory.CreateDirectory( path );
		}

		public static void DeleteAllFilesWithExtensionAtPath( string path, string extension )
		{
			foreach ( var item in Directory.GetFiles( Path.GetFullPath( path ), $"*.{extension}" ) )
			{
				File.Delete( item );
			}
		}
	}
}
