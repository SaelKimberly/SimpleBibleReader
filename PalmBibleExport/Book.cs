using System;

namespace PalmBibleExport
{

	public class Book : Parser
	{
		protected internal const string TAG_NAME = "BOOK";
		protected internal const string CHAPTER_START = "CHAPSTART";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Book(Tokenizer tokenizer, WordRecorder wordRecorder) throws Exception
		public Book(Tokenizer tokenizer, WordRecorder wordRecorder)
		{
			checkTag(tokenizer);

			int start = getIntAttrib(CHAPTER_START);
			for (int i = 1; i < start; i++)
			{
				addChild(new Chapter());
			}

			parse(tokenizer, wordRecorder);
			printMessage(BookName + " ....");
		}

		public virtual string BookName
		{
			get
			{
				return getStringAttrib("NAME");
			}
		}

		public virtual string BookShortcut
		{
			get
			{
				return getStringAttrib("SHORTCUT");
			}
		}

		public virtual int BookNumber
		{
			get
			{
				return getIntAttrib("NUMBER");
			}
		}

		public virtual int TotalChapter
		{
			get
			{
				return TotalChild;
			}
		}

		public virtual Chapter getChapter(int index)
		{
			return (Chapter) getChild(index);
		}

		protected internal override string TagName
		{
			get
			{
				return TAG_NAME;
			}
		}

		protected internal static string formatInteger(int value)
		{
			string valueStr = Convert.ToString(value);

			if (value < 10)
			{
				return valueStr + "  ";
			}
			else if (value < 100)
			{
				return valueStr + " ";
			}
			else
			{
				return valueStr;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object parseChild(Tokenizer tokenizer, WordRecorder wordRecorder) throws Exception
		protected internal override object parseChild(Tokenizer tokenizer, WordRecorder wordRecorder)
		{
			return new Chapter(tokenizer, wordRecorder);
		}
	}

}