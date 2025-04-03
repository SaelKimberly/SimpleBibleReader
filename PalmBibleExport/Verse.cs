using System.Text;

namespace PalmBibleExport
{

	public class Verse : Parser
	{
		protected internal const int VERSE_LIMIT = 8192;

		protected internal const string TAG_NAME = "VERSE";
		protected internal const string BOOKTEXT = "BOOKTEXT";
		protected internal const string CHAPTEXT = "CHAPTEXT";
		protected internal const string DESCTEXT = "DESCTEXT";
		protected internal const string VERSTEXT = "VERSTEXT";
		protected internal int totalCompressedChar;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Verse(Tokenizer tokenizer, WordRecorder wordRecorder) throws Exception
		public Verse(Tokenizer tokenizer, WordRecorder wordRecorder)
		{
			checkTag(tokenizer);
			parse(tokenizer, wordRecorder);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Verse() throws Exception
		public Verse()
		{
			addChild("");
		}

		public virtual int TotalChar
		{
			get
			{
				if (childs.Count > 0)
				{
					return childs[0].ToString().Length;
				}
				else
				{
					return 0;
				}
			}
		}

		public virtual int TotalCompressedChar
		{
			get
			{
				return totalCompressedChar;
			}
			set
			{
				totalCompressedChar = value;
			}
		}


		public virtual string String
		{
			get
			{
				if (childs.Count > 0)
				{
					return childs[0].ToString();
				}
				else
				{
					return "";
				}
			}
		}

        protected internal override string TagName
		{
			get
			{
				return TAG_NAME;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object parseChild(Tokenizer tokenizer, WordRecorder wordRecorder) throws Exception
		protected internal override object parseChild(Tokenizer tokenizer, WordRecorder wordRecorder)
		{
			StringBuilder buffer = new StringBuilder();
			Token token = tokenizer.myGetToken();

			while (!isCloseTag(token))
			{
				 if (buffer.Length > VERSE_LIMIT)
				 {
					 throwUnexpectedLongVerse(token);
				 }

				if (isOpenTag(token, BOOKTEXT))
				{
					buffer.Append((char) 0xFFFF);
				}
				else if (isOpenTag(token, CHAPTEXT))
				{
					buffer.Append((char) 0xFFFE);
				}
				else if (isOpenTag(token, DESCTEXT))
				{
					buffer.Append((char) 0xFFFD);
				}
				else if (isOpenTag(token, VERSTEXT))
				{
					buffer.Append((char) 0xFFFC);
				}
				else if (token.isTag)
				{
					throwUnexpectedToken(token);
				}
				else
				{
					int wordIndex = wordRecorder.getWordIndex(token.ToString());
					buffer.Append((char) wordIndex);
				}
				token = tokenizer.myGetToken();
			}
			tokenizer.ungetToken();

			return buffer.ToString();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void throwUnexpectedLongVerse(Token token) throws ParserException
		protected internal virtual void throwUnexpectedLongVerse(Token token)
		{
			throw new ParserException(token.getErrorMessage("Unexpected Long of Verse"));
		}
	}

}