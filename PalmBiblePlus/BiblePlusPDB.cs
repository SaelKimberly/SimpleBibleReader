using System;
using System.Collections;

namespace PalmBiblePlus
{


	/// 
	/// <summary>
	/// Usage:
	/// - loadVersionInfo(); //only load version info, for example to get info about bible version and book names
	/// - loadWordIndex(); //for decompression
	/// 
	/// </summary>

	public class BiblePlusPDB
	{

		private string versionName;
		private string versionInfo;
		private string sepChar;
		private int versionAttr;
		private int wordIndex;
		private int totalWordRec;
		private int totalBooks;
		private PDBHeader header;


		private string encoding = "UTF-8";

		private bool wordIndexLoaded; // initialized with false;


		private const int infVerInfoRecord = 0;
		private const int infCopyProtected = 0x01;
		private const int infByteNotShifted = 0x02;
		private const int infRightAligned = 0x04;

		private BitVector bv;

		private static readonly int[] book_numbers = {10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 175, 180, 190, 200, 210, 220, 230, 240, 250, 260, 270, 280, 290, 300, 310, 320, 330, 340, 350, 360, 370, 380, 390, 400, 410, 420, 430, 440, 450, 460, 470, 480, 490, 500, 510, 520, 530, 540, 550, 560, 570, 580, 590, 600, 610, 620, 630, 640, 650, 660, 670, 680, 690, 700, 710, 720, 730, 740, 750};

		// public static int[] getBookNumbers() {
		// 	return book_numbers;
		// }

		internal const string EMPTY_BOOKNAME = "";

		// public String getEnglishBookName(int bookindex) {
		// 	if (bookindex < 0 || bookindex>=booksinfo.length) 
		// 		return EMPTY_BOOKNAME;
		// 	BookInfo bi = booksinfo[bookindex];
		// 	int booknum = bi.getBookNumber();
		// 	int pos = Util.binarySearch(book_numbers, booknum);
		// 	if (pos<0) 
		// 		return EMPTY_BOOKNAME;
		// 	return book_names[pos];
		// }

		public static int findGlobalBookNumberIndex(int booknumber)
		{
			return Util.binarySearch(book_numbers, booknumber);
		}

		// public static String getEnglishBookNameFromBookNumber(int booknumber) {
		// 	int pos = findGlobalBookNumberIndex(booknumber);
		// 	if (pos<0) 
		// 		return null;
		// 	return book_names[pos];
		// }


		/// <summary>
		/// return book index
		/// </summary>
		public virtual int findBookNumber(int booknum)
		{
			for (int i = 0; i < booksinfo.Length; i++)
			{
				BookInfo bi = booksinfo[i];
				if (bi.BookNumber == booknum)
				{
					return i;
				}
			}
			return -1;
		}

		private bool validInteger(string s)
		{
			try
			{
				Convert.ToInt32(s);
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}


		internal virtual string[] parse(string s)
		{
			string[] res = new string[3];
			if (s == null || s.Length == 0)
			{
				return null;
			}
			MyStringTokenizer st = new MyStringTokenizer(s, " :");
			int count = st.countTokens();
			if (count == 0)
			{
				return null;
			}
			string[] temp = new string[count];
			string chapter = "1";
			string verse = "1";

			for (int i = 0 ; i < count; i++)
			{
				temp[i] = st.nextToken();
			}
			if (count == 1)
			{
				res[0] = temp[0];
				res[1] = chapter;
				res[2] = verse;
				return res;
			}

			int end = count;

			if (validInteger(temp[count - 1]))
			{
				if (count > 2)
				{
					if (validInteger(temp[count - 2]))
					{
						chapter = temp[count - 2];
						verse = temp[count - 1];
						end = count - 2;
					}
					else
					{
						chapter = temp[count - 1];
						end = count - 1;
					}
				}
				else
				{
					chapter = temp[count - 1];
					end = count - 1;
				}
			}
			string book = "";
			for (int i = 0; i < end; i++)
			{
				if (i > 0)
				{
					book += " ";
				}
				book += temp[i];
			}

			res[0] = book;
			res[1] = chapter;
			res[2] = verse;

			return res;
		}


		/// <summary>
		/// Returns (bookIndex, chapterNumber, verseNumber)
		/// or null if query is invalid
		/// </summary>
		public virtual int[] parseQuery(string query)
		{
			string[] res = parse(query);
			if (res == null)
			{
				//System.out.println("failed parsing to parts");
				return null;
			}

			// System.out.println("part 0: "+res[0]);
			// System.out.println("part 1: "+res[1]);
			// System.out.println("part 2: "+res[2]);

			//try matching first part to the full book name

			int bookidx = -1;

			for (int i = 0; i < booksinfo.Length; i++)
			{
				BookInfo bi = getBook(i);
				//System.out.println("'"+bi.getFullName()+"'");
				//todo: may be we should remove spaces here
				if (res[0].ToUpper() == bi.FullName.ToUpper())
				{
					bookidx = i;
					break;
				}
			}
			//try the short name
			if (bookidx == -1)
			{
				for (int i = 0; i < booksinfo.Length; i++)
				{
					BookInfo bi = getBook(i);
					//todo: may be we should remove spaces here
					if (res[0].ToUpper() == bi.ShortName.ToUpper())
					{
						bookidx = i;
						break;
					}
				}
			}
			//try substring of long name
			if (bookidx == -1)
			{
				string sname = res[0].ToLower();
				for (int i = 0; i < booksinfo.Length; i++)
				{
					BookInfo bi = getBook(i);
					//todo: may be we should remove spaces here
					string fname = bi.FullName.ToLower();
					if (fname.IndexOf(sname) >= 0)
					{
						bookidx = i;
						break;
					}
				}
			}
			if (bookidx == -1)
			{
				return null;
			}

			int[] r = new int[3];
			r[0] = bookidx;

			try
			{
				r[1] = Convert.ToInt32(res[1]);
				r[2] = Convert.ToInt32(res[2]);
			}
			catch (Exception e)
			{
				return null;
			}

			return r;
		}


		public virtual bool ByteShifted
		{
			get
			{
				return (versionAttr & infByteNotShifted) == 0;
			}
		}

		private PDBAccess pdbaccess;
		private BookInfo[] booksinfo;

		public virtual BookInfo[] BooksInfo
		{
			get
			{
				return booksinfo;
			}
		}


		internal virtual PDBHeader Header
		{
			get
			{
				return header;
			}
		}

		internal virtual string SepChar
		{
			get
			{
				return sepChar;
			}
		}

		public virtual int BookCount
		{
			get
			{
				return booksinfo.Length;
			}
		}

		public virtual BookInfo getBook(int index)
		{
			return booksinfo[index];
		}

		internal virtual PDBAccess PDBAccess
		{
			get
			{
				return pdbaccess;
			}
		}

		private bool canSeek;

		internal virtual bool supportRandomAccess()
		{
			return canSeek;
		}

		private string pathName;
		private string pathSeparator = "/";
		private string filenamepart;

		// public void setPathSeparator(String sep) {
		// 	pathSeparator = sep;
		// }

		private bool is_greek = false;
		private bool is_hebrew = false;

		public virtual bool Greek
		{
			get
			{
				return is_greek;
			}
		}

		public virtual bool Hebrew
		{
			get
			{
				return is_hebrew;
			}
		}

		private void getFileNamePart()
		{
			int pos = pathName.LastIndexOf(pathSeparator[0]);
			if (pos < 0)
			{
				filenamepart = pathName;
				return;
			}
			filenamepart = pathName.Substring(pos + 1);
			is_greek = filenamepart.StartsWith("z");
			is_hebrew = filenamepart.StartsWith("q");
		}

		public BiblePlusPDB(PDBDataStream @is, int[] _hebrewtab, int[] _greektab)
		{
			Util.setTables(_hebrewtab, _greektab);
			canSeek = @is.canSeek();
			pdbaccess = new PDBAccess(@is);
			pathName = @is.PathName;
			filenamepart=pathName;
		}

		public BiblePlusPDB(PDBDataStream @is, string _encoding)
		{
			canSeek = @is.canSeek();
			pdbaccess = new PDBAccess(@is);
			encoding = _encoding;
			pathName = @is.PathName;
            filenamepart = pathName;
		}

		// public void setEncoding(String _encoding) {
		// 	encoding = _encoding;
		// }

		public virtual string Encoding
		{
			get
			{
				return encoding;
			}
		}


		private byte[] word_data;
		private int[] wordLength;
		private int[] totalWord;
		private bool[] compressed;
		private bool[] nothing;

		private int[] byteacc;
		private int totalWords; // initialized with 0;


		private void AddResult(ArrayList result, int b, int c, int v)
		{
			result.Add(new int?(b));
			result.Add(new int?(c));
			result.Add(new int?(v));
		}

		/*is substr in str (fullmatch)*/
		private bool fullMatch(string s, string str)
		{
			if (str.IndexOf(sepChar + s + sepChar) >= 0)
			{
				return true;
			}
			if (str.StartsWith(s + sepChar))
			{
				return true;
			}
			if (str.EndsWith(sepChar + s))
			{
				return true;
			}
			return false;
		}

		private bool matchAtLeastOneQueryToVerse(string[] lower_query, int book, int chapter, int verse, bool partial)
		{
			BookInfo bi = getBook(book);
			if (bi == null)
			{
				return false;
			}
			try
			{
				bi.openBook();
			}
			catch (Exception e)
			{
				return false;
			}
			string s = bi.getVerse(chapter, verse).ToLower();
			for (int i = 0; i < lower_query.Length; i++)
			{
				if (partial)
				{
					if (s.IndexOf(lower_query[i]) >= 0)
					{
						return false;
					}
				}
				else
				{
					if (fullMatch(lower_query[i], s))
					{
						return true;
					}
				}
			}
			return true;
		}


		private bool matchAllQueryToVerse(string[] lower_query, int book, int chapter, int verse, bool partial)
		{
			BookInfo bi = getBook(book);
			if (bi == null)
			{
				return false;
			}
			try
			{
				bi.openBook();
			}
			catch (Exception e)
			{
				return false;
			}
			string s = bi.getVerse(chapter, verse).ToLower();
			for (int i = 0; i < lower_query.Length; i++)
			{

				if (s.IndexOf(lower_query[i]) < 0)
				{
						return false;
				}
				if (!partial)
				{
					if (!fullMatch(lower_query[i], s))
					{
						return false;
					}
				}
			}
			// System.out.println("match: " + s);
			return true;
		}

		/// <summary>
		/// partial = partial match, "he" will match "whether"
		/// allwords = all words must be present in that verse
		/// </summary>

		public virtual BitVector getWordNumbers(string[] query, bool partial, bool allwords, int maxsteps)
		{

			if (query == null || query.Length == 0)
			{
				return null;
			}

			int minlength = query[0].Length;
			int maxlength = minlength;

			//System.out.println(query[0]);


			for (int i = 1; i < query.Length; i++)
			{

				//System.out.println(query[i]);

				 int l = query[i].Length;
				 if (l < minlength)
				 {
					 minlength = l;
				 }
				 if (l > maxlength)
				 {
					 maxlength = l;
				 }
			}

			// System.out.println("minl " + minlength + " maxl " + maxlength);

			if (bv == null)
			{
				bv = new BitVector(totalWords);
			}
			else
			{
				bv.reset();
			}

			int wn = 0;
			for (int i = 0; i < totalWord.Length; i++)
			{
				int len = wordLength[i];
				//System.out.println("words with length " +  len);
				if (compressed[i] || (len < minlength))
				{
					wn += totalWord[i];
					continue;
				}

				if (!partial && len > maxlength)
				{
					wn += totalWord[i];
					continue;
				}

				for (int j = 0; j < totalWord[i]; j++)
				{
					wn++;
					int index = byteacc[i] + j * len;
					string word;

					word = readString(index, len);


					string wordlc = word.ToLower();
					if (partial)
					{
						//System.out.println(wn + "word " + word);

						int bits = 0;
						for (int k = 0; k < query.Length; k++)
						{
							if (wordlc.IndexOf(query[k]) >= 0)
							{
								//System.out.println("partial match " + wordlc);
								bv.set(wn);
								if (!allwords)
								{
									break;
								}
								bits |= (1 << k);
							}
						}
						if (allwords && bits != 0)
						{
							//System.out.print(word + " " );
							bv.storeMatchInfo(wn, bits);
						}
					}
					else
					{
						//must be full match
						int bits = 0;
						for (int k = 0; k < query.Length; k++)
						{
							if (query[k].Equals(wordlc))
							{
								bv.set(wn);
								if (!allwords)
								{
									break;
								}
								bits |= (1 << k);
							}
						}
						if (allwords && bits != 0)
						{
							bv.storeMatchInfo(wn, bits);
						}
					}
					//System.out.println(word);
				}
			}

			wn = 0;

			//handle compressed case

			if (allwords && query.Length > 1)
			{
				bv.setStartPos();
			}

			for (int i = 0; i < totalWord.Length; i++)
			{
				int len = wordLength[i];
				if (!compressed[i])
				{
					wn += totalWord[i];
					continue;
				}
				int repeat = len / 2;
				for (int j = 0; j < totalWord[i]; j++)
				{
					wn++;
					int index = byteacc[i] + j * len;
					int bits = 0;
					for (int r = 0; r < repeat; r++)
					{
						int n = Util.readShort(word_data, index + r * 2);
						if (bv.get(n) == 1)
						{
							bv.set(wn);
							if (!allwords)
							{
								break;
							}
							int _b = bv.getMatchBitsForWord(n);
							bits |= _b;
						}
					}
					if (allwords)
					{
						bv.storeMatchInfo(wn, bits);
					}
				}
			}

			if (allwords)
			{
				int filter = 0;
				for (int k = 0; k < query.Length; k++)
				{
					filter |= (1 << k);
				}

				/// <summary>
				/// If word in query never appear, then the search result is empty
				/// </summary>
				int res = 0;

				for (int i = 0; i < bv.wcount; i++)
				{
					long l = bv.matchbits_array[i];
					res |= (int)l;
				}

				if ((filter & res) != filter)
				{
					return null;
				}
				bv.setStartPos();
			}

			if (bv.countSet() == 0)
			{
				return null;
			}

			return bv;
		}

		/// <summary>
		/// load word index (for decompression)
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void loadWordIndex() throws java.io.IOException
		public virtual void loadWordIndex()
		{

			if (wordIndexLoaded)
			{
				return;
			}

			PDBRecord r = pdbaccess.readRecord(wordIndex);
			int idx = 0;
			byte[] index_data = r.Data;
			int totalIndexes = Util.readShort(index_data, idx);
			// System.out.println("total indexes: " + totalIndexes);
			idx += 2;
			wordLength = new int[totalIndexes];
			totalWord = new int[totalIndexes];
			compressed = new bool[totalIndexes];
			nothing = new bool[totalIndexes];

			for (int i = 0; i < totalIndexes; i++)
			{
				wordLength[i] = Util.readShort(index_data, idx);
				idx += 2;
				totalWord[i] = Util.readShort(index_data, idx);
				totalWords += totalWord[i];
				idx += 2;
				compressed[i] = index_data[idx++] != 0;
				// System.out.println("len " + wordLength[i] + " totalword[" +i + "]=" + totalWord[i] + " compressed = "+compressed[i]);
				nothing[i] = index_data[idx++] != 0;
			}
			// System.out.println("all total " + totalWords);

			int totalByteAcc = 0;
			byteacc = new int[totalIndexes + 1];
			byteacc[0] = 0;
			for (int i = 1; i <= totalIndexes; i++)
			{
				int _totalWord = totalWord[i - 1];
				int _wordLen = wordLength[i - 1];
				totalByteAcc += _totalWord * _wordLen;
				byteacc[i] = totalByteAcc;
			}

			PDBRecord[] records = new PDBRecord[totalWordRec];
			int total_len = 0;
			for (int i = 0; i < totalWordRec; i++)
			{
				records[i] = pdbaccess.readRecord(wordIndex + i + 1);
				byte[] d = records[i].Data;
				total_len += d.Length;
			}

			word_data = new byte[total_len];
			int l = 0;
			for (int i = 0; i < totalWordRec; i++)
			{
				byte[] d = records[i].Data;
				Array.Copy(d, 0, word_data, l, d.Length);
				l += d.Length;
				pdbaccess.removeFromCache(wordIndex + i + 1);
			}

			wordIndexLoaded = true;
		}

		public virtual string VersionName
		{
			get
			{
				return versionName;
			}
		}

		private const int BOOK_REC_SIZE = 46;


		private int fail_reason;

		public const int SUCCESS = 0;
		public const int ERR_NOT_PDB_FILE = 1;
		public const int ERR_NOT_BIBLE_PLUS_FILE = 2;
		public const int ERR_FILE_CORRUPTED = 3;


		public virtual int FailReason
		{
			get
			{
				return fail_reason;
			}
		}

		/*
		 * if this fails, then the bible file is not valid, or not enough memory available
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean loadVersionInfo() throws java.io.IOException
		public virtual bool loadVersionInfo()
		{

			fail_reason = SUCCESS;

			header = pdbaccess.Header;

			if (header == null)
			{
				fail_reason = ERR_NOT_PDB_FILE;
				return false;
			}

			if (pdbaccess.Corrupted)
			{
				fail_reason = ERR_FILE_CORRUPTED;
				return false;
			}

			if (!header.Type.Equals("bibl"))
			{
				fail_reason = ERR_NOT_BIBLE_PLUS_FILE;
				return false;
			}


			PDBRecord version = pdbaccess.readRecord(0);

			byte[] data = version.Data;

			int idx = 0;
            versionName = Util.readStringTrimZero(data, idx, 16, encoding);
            //System.Text.Encoding.GetEncoding(encoding).GetString(data, idx, 16);//  //
			idx += 16;
			versionInfo = Util.readStringTrimZero(data, idx, 128, encoding);
			// System.out.println("Info:" + versionInfo);


			idx += 128;
			sepChar = Util.readString(data, idx, 1, encoding);
			idx++;
			versionAttr = data[idx] & 0xff;
			// System.out.println("version attr " + versionAttr);

			int v = (versionAttr & infCopyProtected);
			// System.out.println("Copy protected:" + v);
			v = (versionAttr & infByteNotShifted);
			// System.out.println("Byte not shifted:" + v);
			v = (versionAttr & infRightAligned);
			// System.out.println("right aligned:" + v);

			idx++;
			wordIndex = Util.readShort(data, idx);
			// System.out.println("Word index: " + wordIndex);

			idx += 2;
			totalWordRec = Util.readShort(data, idx);
			// System.out.println("totalWordRecord: " + totalWordRec);

			if (wordIndex + totalWordRec >= header.RecordCount)
			{
				fail_reason = ERR_FILE_CORRUPTED;
				return false;
			}

			idx += 2;
			totalBooks = Util.readShort(data, idx);
			idx += 2;
			// System.out.println("totalBooks: " + totalBooks);

			if (totalBooks < 0)
			{
				fail_reason = ERR_FILE_CORRUPTED;
				return false;
			}

			booksinfo = new BookInfo[totalBooks];

			for (int i = 0; i < totalBooks; i++)
			{
				if (idx + BOOK_REC_SIZE > data.Length)
				{
					fail_reason = ERR_FILE_CORRUPTED;
					return false;
				}

				booksinfo[i] = BookInfo.createFromData(this, data, idx, i);
				if (booksinfo[i] == null)
				{
					fail_reason = ERR_FILE_CORRUPTED;
					return false;
				}

				//System.out.println(booksinfo[i]);
				idx += BOOK_REC_SIZE;
			}

			pdbaccess.removeFromCache(0);

			return true;
		}

		internal virtual int[] getRepeat(int pos, int wordNum)
		{


			int repeat = 1;
			int[] result;
			//System.out.println("word repeat----");
			if (wordNum < 0xFFF0)
			{
				bool compressed = pos == 0?true:isCompressed(pos);
				int len = getWordLength(pos);
				int wordIndex = getWordIndex(pos, wordNum);
				if (compressed)
				{
					//System.out.println("compressed\n");
					repeat = len / 2;
					if (repeat == 0)
					{
						return null;
					}

					result = new int[repeat];
					int st = wordIndex;
					//System.out.println("repeat " + repeat + " wi " + wordIndex);
					for (int i = 0; i < repeat; i++)
					{
						result[i] = Util.readShort(word_data, st);
						//System.out.println("numval = " + result[i]);
						st += 2;
					}
					return result;
				}
				else
				{
					//System.out.println("Not compressed");
					result = new int[1];
					result[0] = wordNum;
					return result;
				}
			}
			result = new int[1];
			result[0] = wordNum;
			return result;
		}



		private bool isCompressed(int pos)
		{
			return compressed[pos];
		}

		private int getWordLength(int pos)
		{
			return wordLength[pos];
		}

		internal virtual int getWordIndex(int pos, int wordNum)
		{
			int relNum = wordNum - 1;
			int decWordIndex = 0;
			for (int i = 0; i <= pos; i++)
			{
				int _totalWord = totalWord[i];
				if (relNum < _totalWord)
				{
					int decWordLen = wordLength[i];
					//System.out.println("wl;"+decWordLen);
					decWordIndex = byteacc[i] + relNum * decWordLen;
					break;
				}
				else
				{
					relNum = (relNum - _totalWord);
				}
			}
			return decWordIndex;
		}

		internal virtual int getWordPos(int wordNum)
		{
			int relNum = wordNum - 1;
			for (int i = 0; i < totalWord.Length; i++)
			{
				int _totalWord = totalWord[i];
				if (relNum < _totalWord)
				{
					return i;
				}
				else
				{
					relNum = (relNum - _totalWord);
				}
			}
			return 0;
		}

		private string readString(int index, int len)
		{
			if (is_greek)
			{
				return Util.readStringGreek(word_data, index, len);
			}
			else if (is_hebrew)
			{
				return Util.readStringHebrew(word_data, index, len);
			}
			else
			{
				return Util.readString(word_data, index, len, encoding);
			}
		}

		internal virtual string getWord(int wordNum)
		{
			int pos = getWordPos(wordNum);
			int index = getWordIndex(pos, wordNum);
			int len = getWordLength(pos);
			if (index == -1)
			{
				return "";
			}
			return readString(index, len);
		}

		public virtual void close()
		{

			word_data = null;
			wordLength = null;
			totalWord = null;
			compressed = null;
			nothing = null;
			byteacc = null;

			if (booksinfo != null)
			{
				for (int i = 0; i < booksinfo.Length; i++)
				{
					if (booksinfo[i] != null)
					{
						booksinfo[i].close();
						booksinfo[i] = null;
					}
				}
			}
			if (pdbaccess != null)
			{
				pdbaccess.close();
				pdbaccess = null;
			}
			//System.gc();
		}

	}

}