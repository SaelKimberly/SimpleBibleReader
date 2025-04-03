using System;

namespace PalmBibleExport
{

	public class ParserException : Exception
	{
		public ParserException(string @string) : base(@string)
		{
		}
	}

}