using System;

namespace PalmBibleExport
{

	public class ParserInfo
	{
		protected internal const string ENCODE_DEFAULT = "iso-8859-1";

		protected internal const string CONT_TYPE = "CONT";
		protected internal const string SPCSEP_TYPE = "SPCSEP";
		protected internal const string PARSERINFO_TAG = "PARSERINFO";
		protected internal Tag tag;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParserInfo(Tokenizer tokenizer) throws Exception
		public ParserInfo(Tokenizer tokenizer)
		{
			Token token = tokenizer.Token;
			while (!token.isTag)
			{
				token = tokenizer.Token;
			}
			if (token.isTag)
			{
				tag = token.Tag;
				Console.WriteLine(tag + "=" + PARSERINFO_TAG);
				if (tag.Name.Equals(PARSERINFO_TAG))
				{
					return;
				}
			}
			throwNoParserInfo();
		}

		public virtual string Encoding
		{
			get
			{
				string encode = tag.getAttrib("ENCODE");
    
				if (encode == null)
				{
					encode = ENCODE_DEFAULT;
				}
    
				return encode;
			}
		}

		public virtual string Decoding
		{
			get
			{
				string decode = tag.getAttrib("DECODE");
    
				if (decode == null)
				{
					decode = Encoding;
				}
    
				return decode;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getBreakUnicode() throws ParserException
		public virtual bool BreakUnicode
		{
			get
			{
				string wordType = tag.getAttrib("WORDTYPE");
    
				if (wordType.Equals(CONT_TYPE))
				{
					return true;
				}
				else if (wordType.Equals(SPCSEP_TYPE))
				{
					return false;
				}
				else
				{
					throwUnknownWordType(wordType);
				}
    
				return true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void throwUnknownWordType(String wordType) throws ParserException
		protected internal virtual void throwUnknownWordType(string wordType)
		{
			throw new ParserException("Unknow Word Type : " + wordType);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void throwNoParserInfo() throws ParserException
		protected internal virtual void throwNoParserInfo()
		{
			throw new ParserException("Unable to find parser info.");
		}
	}

}