using System.IO;

namespace Intruder.Tools
{
	public enum FileSize
	{
		Bytes, Kilobytes, Megabytes
	}

	public static class FileUtility
	{
		public static void DirectoryCheck( string path )
		{
			if ( !Directory.Exists( path ) )
				Directory.CreateDirectory( path );
		}

		public static float GetFileSize( string path, FileSize fileSize )
		{
			float bytes = new FileInfo( Path.GetFullPath( path ) ).Length;
			var kilobytes = bytes / 1024;
			var megabytes = kilobytes / 1024;

			switch ( fileSize )
			{
				case FileSize.Bytes:
					return bytes;
				case FileSize.Kilobytes:
					return kilobytes;
				case FileSize.Megabytes:
					return megabytes;
				default:
					return 0;
			}
		}

		public static bool IsOverSize( string path, FileSize size, float value )
		{
			return GetFileSize( path, size ) > value;
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
