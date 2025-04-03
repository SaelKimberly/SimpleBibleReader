namespace PalmBibleExport
{

	public class Chapter : Parser
	{
		protected internal const string TAG_NAME = "CHAPTER";
		protected internal const string VERSE_START = "VERSSTART";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Chapter(Tokenizer tokenizer, WordRecorder wordRecorder) throws Exception
		public Chapter(Tokenizer tokenizer, WordRecorder wordRecorder)
		{
			checkTag(tokenizer);

			int start = getIntAttrib(VERSE_START);
			for (int i = 1; i < start; i++)
			{
				addChild(new Verse());
			}

			parse(tokenizer, wordRecorder);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Chapter() throws Exception
		public Chapter()
		{
			addChild(new Verse());
		}

		public virtual int TotalVerse
		{
			get
			{
				return TotalChild;
			}
		}

		public virtual Verse getVerse(int index)
		{
			return (Verse) getChild(index);
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
			return new Verse(tokenizer, wordRecorder);
		}
	}

}