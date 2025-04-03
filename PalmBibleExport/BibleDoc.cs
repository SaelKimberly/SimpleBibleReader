using System.IO;
namespace PalmBibleExport
{


	public class BibleDoc : Parser
	{
		protected internal const string TAG_NAME = "BIBLE";
		protected internal WordRecorder wordRecorder;
		protected internal bool breakUnicode = false;
		protected internal string encode = "iso-8859-1";
		protected internal string decode = "iso-8859-1";

		protected internal bool quiet = false;
		protected internal bool keepPunctuation = false; // RWS 2004.12.14

		protected internal BibleChecker bibleCheck = new BibleChecker();

		// RWS 2004.12.14 added original signature constructor to accomodate old calls
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BibleDoc(java.io.File file, String dec, String enc, boolean quiet) throws Exception
		public BibleDoc(string file, string dec, string enc, bool quiet) : this(file, dec, enc, quiet, false)
		{
		}

		// RWS 2004.12.14 added keepPunctuation to constructor
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BibleDoc(java.io.File file, String dec, String enc, boolean quiet, boolean keepPunctuation) throws Exception
		public BibleDoc(string file, string dec, string enc, bool quiet, bool keepPunctuation)
		{
			this.quiet = quiet;
			this.keepPunctuation = keepPunctuation;

            byte[] file_bytes = File.ReadAllBytes(file);
            Tokenizer tokenizer = new Tokenizer(new MemoryStream(file_bytes), encode);
			ParserInfo parserInfo = new ParserInfo(tokenizer);
			tokenizer.close();

			if (dec == null)
			{
				decode = parserInfo.Decoding;
			}
			else
			{
				decode = dec;
			}

			if (enc == null)
			{
				encode = parserInfo.Encoding;
			}
			else
			{
				encode = enc;
			}

			// RWS 2004.12.14 added keepPunctuation parm
            tokenizer = new Tokenizer(new MemoryStream(file_bytes), decode, keepPunctuation);

			wordRecorder = new WordRecorder(encode);
			parserInfo = new ParserInfo(tokenizer);

			breakUnicode = parserInfo.BreakUnicode;
			tokenizer.BreakUnicode = breakUnicode;

			checkTag(tokenizer);
			parse(tokenizer, wordRecorder);
			tokenizer.close();

			sortBook();
		}

		// RWS 2004.12.14
		public virtual bool Quiet
		{
			get
			{
				return quiet;
			}
		}

		public virtual string Encoding
		{
			get
			{
				return encode;
			}
		}

		public virtual WordRecorder WordRecorder
		{
			get
			{
				return wordRecorder;
			}
		}

		public virtual bool BreakUnicode
		{
			get
			{
				return breakUnicode;
			}
		}

		public virtual bool RightAligned
		{
			get
			{
				string align = getStringAttrib("ALIGN");
    
				if (align != null && align.ToUpper().Equals("RIGHT"))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public virtual bool ByteShifted
		{
			get
			{
				return wordRecorder.ByteShifted;
			}
		}

		public virtual bool CopyProtected
		{
			get
			{
				return false;
			}
		}

		public virtual string BibleDocName
		{
			get
			{
				return getStringAttrib("NAME");
			}
		}

		public virtual string BibleDocInfo
		{
			get
			{
				string info = getStringAttrib("INFO");
    
				if (info == null)
				{
					return "";
				}
				else
				{
					return info;
				}
			}
		}

		public virtual int TotalBook
		{
			get
			{
				return TotalChild;
			}
		}

		public virtual Book getBook(int index)
		{
			return (Book) getChild(index);
		}

        protected internal override string TagName
		{
			get
			{
				return TAG_NAME;
			}
		}

		protected internal virtual void sortBook()
		{
			int totalBook = TotalBook;
			Book[] books = new Book[totalBook];

			for (int i = 0; i < totalBook; i++)
			{
				books[i] = getBook(i);
			}

			for (int i = 0; i < totalBook; i++)
			{
				for (int j = 0; j < totalBook - 1; j++)
				{
					if (books[j].BookNumber > books[j + 1].BookNumber)
					{
						Book book = books[j];
						books[j] = books[j + 1];
						books[j + 1] = book;
					}
				}
			}

			childs.Clear();

			for (int i = 0; i < totalBook; i++)
			{
				childs.Add(books[i]);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object parseChild(Tokenizer tokenizer, WordRecorder wordRecorder) throws Exception
		protected internal override object parseChild(Tokenizer tokenizer, WordRecorder wordRecorder)
		{
			Book book = new Book(tokenizer, wordRecorder);

			if (!quiet)
			{
				bibleCheck.checkBook(book);
			}
			return book;
		}
	}

}