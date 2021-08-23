using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Intruder.Tools
{
	public static class StringUtility
	{
		// Copied from this (https://stackoverflow.com/a/2132004)
		public static string[] SplitArguments( this string commandLine )
		{
			var parmChars = commandLine.ToCharArray();
			var inSingleQuote = false;
			var inDoubleQuote = false;
			for ( var index = 0; index < parmChars.Length; index++ )
			{
				if ( parmChars[index] == '"' && !inSingleQuote )
				{
					inDoubleQuote = !inDoubleQuote;
					parmChars[index] = '\n';
				}
				if ( parmChars[index] == '\'' && !inDoubleQuote )
				{
					inSingleQuote = !inSingleQuote;
					parmChars[index] = '\n';
				}
				if ( !inSingleQuote && !inDoubleQuote && parmChars[index] == ' ' )
					parmChars[index] = '\n';
			}
			return (new string( parmChars )).Split( new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries );
		}

		public static string RemoveSpaces( this string input )
		{
			return string.Join( "", input.Split( default( string[] ), System.StringSplitOptions.RemoveEmptyEntries ) );
		}

		public static bool ToBool( this string input )
		{
			switch ( input )
			{
				case "true":
				case "on":
				case "1":
					return true;

				case "false":
				case "off":
				case "0":
					return false;

				default:
					return false;
			}
		}

		public static int ToInt( this string input )
		{
			return System.Convert.ToInt32( input );
		}

		public static float ToFloat( this string input )
		{
			return System.Convert.ToSingle( input );
		}

		public static bool Contains( this string source, string toCheck, StringComparison comp )
		{
			return source?.IndexOf( toCheck, comp ) >= 0;
		}

		public static bool Matches( this string source, string toCheck )
		{
			string[] ar = source.Split( ' ' );

			foreach ( string str in ar )
			{
				if ( str.ToLower() == toCheck.ToLower() )
					return true;
			}
			return false;
		}

		public static string[] GetStringsFromString( this string source )
		{
			return source.Split( '"' );
		}
	}
}

