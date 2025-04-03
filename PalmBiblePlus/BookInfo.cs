using System;
using System.Text;

namespace PalmBiblePlus
{


	public class BookInfo
	{

		/*book number*/
		private int bookNum;
		/*index record*/
		private int bookIndex;
		private int totalBookRec;
		private int bookPosition;
		private string simpleName;
		private string complexName;
		private BiblePlusPDB bible;

		public virtual int BookNumber
		{
			get
			{
				return bookNum;
			}
		}

		public virtual string FullName
		{
			get
			{
				return complexName;
			}
		}

		public virtual string ShortName
		{
			get
			{
				return simpleName;
			}
		}

		internal int totalChapters;

		internal bool bookOpened; //default initialized to false;


		public readonly int bookTextType = 0xFFFF;
		public readonly int chapTextType = 0xFFFE;
		public readonly int descTextType = 0xFFFD;
		public readonly int versTextType = 0xFFFC;


		public int RECORD_SIZE = 4096;

		private byte[] index_data;
		private byte[] data;
		private int[] totalVersesAcc;
		private int[] totalChapterCharsAcc;
		private int[] totalVerseCharsAcc;

		internal virtual byte[] Data
		{
			get
			{
				return data;
			}
		}

		/// <summary>
		/// Get number of chapters in this book </summary>
		/// <returns> chapter count </returns>
		public virtual int ChapterCount
		{
			get
			{
				return totalChapters;
			}
		}

		private StringBuilder[] sb;

		/*
		int getBookIndex() {
			return bookIndex;
		}
		*/
		internal BookInfo(BiblePlusPDB bible)
		{
			this.bible = bible;
			sb = new StringBuilder[4];

			for (int i = 0; i < 4; i++)
			{
				sb[i] = new StringBuilder();
			}

		}

		public virtual string ToString()
		{
			return complexName;
		}

		// public String toStringInfo() {
		// 	StringBuffer sb = new StringBuffer();
		// 	sb.append("Book Num: ").append(bookNum).append("\n");
		// 	sb.append("Book Index: ").append(bookIndex).append("\n");
		// 	sb.append("Book rec: ").append(totalBookRec).append("\n");
		// 	sb.append("Name: ").append(simpleName).append("\n");
		// 	sb.append("Full Name: ").append(complexName).append("\n");
		// 	return sb.toString();
		// }

		internal virtual int getVerseStart(int chapter, int verse)
		{
			//System.out.println("vstart = " + (chapter==1?0:totalVersesAcc[chapter - 2]));
			int verseAcc = (chapter == 1?0:totalVersesAcc[chapter - 2]) + verse;
			//System.out.println("verseacc = " + verseAcc);
			int verseStart = chapter == 0?0:totalChapterCharsAcc[chapter - 1];
			//System.out.println("versestart1: " + verseStart);

			if (verse > 1)
			{
				verseStart += verseAcc == 1?0:totalVerseCharsAcc[verseAcc - 2];
			}
			//System.out.println("versestart2: " + verseStart);

			return verseStart;
		}

		public virtual int getVerseCount(int chapter)
		{
			int v1 = totalVersesAcc[chapter - 1];
			int v2 = chapter == 1?0:totalVersesAcc[chapter - 2];
			return v1 - v2;
		}


		private int vlen(int index)
		{
			//System.out.println("vlen = " + index);
			int v1 = totalVerseCharsAcc[index];
			int v2 = index == 0?0:totalVerseCharsAcc[index - 1];
			int diff = v1 - v2;
			if (diff < 0)
			{
				return 0;
			}
			return diff;
		}


		internal virtual int getVerseLength(int chapter, int verse)
		{
			int verseAcc = (chapter == 1?0:totalVersesAcc[chapter - 2]) + verse;
			int verseLength;
			if (verse > 1)
			{
				verseLength = vlen(verseAcc - 1);
			}
			else
			{
				verseLength = verseAcc == 0?0:totalVerseCharsAcc[verseAcc - 1];
			}
			return verseLength;
		}

		private int[] shiftLookup = {0, 3, 2, 1};
		private int[] verseShiftLookup = {10, 4, 6, 8};

		internal virtual void addSepChar(StringBuilder sb, string word)
		{
			if (sb.Length > 0)
			{
				if (word.Length == 1)
				{
					if (".,?!;:-".IndexOf(word[0]) < 0)
					{
						sb.Append(bible.SepChar);
					}
				}
				else
				{
					sb.Append(bible.SepChar);
				}
			}
		}

		internal virtual StringBuilder[] getVerseByteShifted(int chapter, int verse)
		{
			int verseStart = getVerseStart(chapter, verse);
			int verseLength = getVerseLength(chapter, verse);

			for (int i = 0; i < 4; i++)
			{
				sb[i].Length = 0;
			}

			//System.out.println("Start " + verseStart + " length " + verseLength);

			int decShift = 0;
			int[] decValueBuffer = new int[3];

			int compStart;
			compStart = verseStart * 7 / 4;
			decShift = shiftLookup[verseStart * 7 % 4];

			int idx = compStart;

			switch (decShift)
			{
			case 1:
				decValueBuffer[1] = data[idx++];
				break;
			case 2:
			case 3:
				decValueBuffer[2] = data[idx++];
				break;
			default:
				;
			break;
			}

			int sbpos = 0;

			for (int i = 0; i < verseLength; i++)
			{
                try
                {
                    switch (decShift)
                    {
                        case 0:
                            decValueBuffer[0] = data[idx++] & 0xff;
                            decValueBuffer[1] = data[idx++] & 0xff;
                            decValueBuffer[2] = 0;
                            break;
                        case 1:
                            decValueBuffer[0] = decValueBuffer[1];
                            decValueBuffer[1] = data[idx++] & 0xff;
                            decValueBuffer[2] = data[idx++] & 0xff;
                            break;
                        case 2:
                            decValueBuffer[0] = decValueBuffer[2];
                            decValueBuffer[1] = data[idx++] & 0xff;
                            decValueBuffer[2] = data[idx++] & 0xff;
                            break;
                        case 3:
                            decValueBuffer[0] = decValueBuffer[2];
                            decValueBuffer[1] = data[idx++] & 0xff;
                            decValueBuffer[2] = 0;
                            break;
                        default:
                            ;
                            break;
                    }
                }
                catch (Exception)
                {
                    
                    //throw;
                }
				

				int value = decValueBuffer[0] << 16 | decValueBuffer[1] << 8 | decValueBuffer[2];
				value = value >> verseShiftLookup[decShift];
				value = value & 0x3FFF;
				decShift++;

				if (decShift == 4)
				{
					decShift = 0;
				}

				if (value > 0x3FF0)
				{
					value |= 0xC000;
				}

				int decWordNum = value;
				int pos = bible.getWordPos(decWordNum);
				int wordIndex = bible.getWordIndex(pos, decWordNum);
				int[] r = bible.getRepeat(pos, decWordNum);
				if (r != null)
				{
					for (int j = 0; j < r.Length; j++)
					{
						if (r[j] > 0x3FF0)
						{
							r[j] |= 0xC000;
						}

						if (r[j] == bookTextType || r[j] == chapTextType || r[j] == descTextType || r[j] == versTextType)
						{
							sbpos = r[j] - versTextType;
							continue;
						}

						string word = bible.getWord(r[j]);

						addSepChar(sb[sbpos], word);

						sb[sbpos].Append(word);
					}
				}
				else
				{
					string word = bible.getWord(decWordNum);
					addSepChar(sb[sbpos], word);
					sb[sbpos].Append(word);
					//System.out.println("r is null" + word);
				}
			}
			return sb;
		}

		public virtual StringBuilder[] getCompleteVerse(int chapter, int verse)
		{

			if (!bookOpened)
			{
				return null;
			}

			if (chapter < 0 || chapter>ChapterCount)
			{
				return null;
			}

			if (verse < 0 || verse>getVerseCount(chapter))
			{
				return null;
			}

			if (bible.ByteShifted)
			{
				return getVerseByteShifted(chapter, verse);
			}

			for (int i = 0; i < 4; i++)
			{
				sb[i].Length = 0;
			}

			int sbpos = 0; //verse


			int verseStart = getVerseStart(chapter, verse);
			//System.out.println("versestart " + verseStart);

			int verseLength = getVerseLength(chapter, verse);

			//System.out.println("Start " + verseStart + " length " + verseLength);

			int compStart = verseStart * 2;

			int idx = compStart;

			for (int i = 0; i < verseLength; i++)
			{
				int decWordNum = (data[idx] & 0xff) * 256 + (data[idx + 1] & 0xff);
				idx += 2;
				//System.out.println("decWordNum " + decWordNum);
				int pos = bible.getWordPos(decWordNum);
				//System.out.println("wordpos " + pos);
				//System.out.println("wordlength " + bible.getWordLength(pos));
				int wordIndex = bible.getWordIndex(pos, decWordNum);
				//System.out.println("wordindex " + wordIndex);
				int[] r = bible.getRepeat(pos, decWordNum);
				if (r != null)
				{
					for (int j = 0; j < r.Length; j++)
					{
						//if (r[j]<0) break;
						//System.out.print("rj " + r[j]+" ");

						if (r[j] == bookTextType || r[j] == chapTextType || r[j] == descTextType || r[j] == versTextType)
						{
							sbpos = r[j] - versTextType;
							//System.out.print("switch sbpos " + sbpos+" ");
							continue;
						}


						string word = bible.getWord(r[j]);

						//System.out.println("Word " + word);
						addSepChar(sb[sbpos], word);
						sb[sbpos].Append(word);
					}
				}
				else
				{
					string word = bible.getWord(decWordNum);
					addSepChar(sb[sbpos], word);
					sb[sbpos].Append(word);
					//System.out.println("r is null" + word);
				}
			}

			return sb;

		}


		public virtual string getVerse(int chapter, int verse)
		{
			StringBuilder[] sb = getCompleteVerse(chapter, verse);
			if (sb == null)
			{
				return null;
			}
			return sb[0].ToString();
		}


		public virtual bool tryOpenBook()
		{
			try
			{
				openBook();
			}
			catch (Exception e)
			{
				return false;
			}
			return true;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void openBook() throws java.io.IOException
		public virtual void openBook()
		{

			if (bookOpened)
			{
				return;
			}

			if (!bible.supportRandomAccess())
			{
				//if we don't have random access
				//must make sure all previous books have been opened
				//the correct way to do it is to load all previous books in main thread, and make progress bar
				//this is to prevent error
				for (int i = 0; i < bookPosition; i++)
				{
					BookInfo bi = bible.getBook(i);
					bi.openBook();
				}
			}

			PDBAccess access = bible.PDBAccess;

			PDBRecord r = access.readRecord(bookIndex);
			index_data = r.Data;

			//Util.dumpBytes(index_data);

			PDBRecord[] records = new PDBRecord[totalBookRec];
			int total_len = 0;
			for (int i = 0; i < totalBookRec; i++)
			{
				records[i] = access.readRecord(bookIndex + i + 1);
				byte[] d = records[i].Data;
				//System.out.println("d = "+ d.length);
				total_len += d.Length;
			}
			data = new byte[total_len];
			for (int i = 0; i < totalBookRec; i++)
			{
				byte[] d = records[i].Data;
				Array.Copy(d, 0, data, i * RECORD_SIZE, d.Length);
				access.removeFromCache(bookIndex + i + 1);
			}

			totalChapters = Util.readShort(index_data, 0);

			//System.out.println("total chapters: " + totalChapters);

			totalVersesAcc = new int[totalChapters];

			int offs = 2;

			for (int i = 0; i < totalChapters; i++)
			{
				totalVersesAcc[i] = Util.readShort(index_data, offs);
				//System.out.println("totalVersesAcc " + totalVersesAcc[i]);
				offs += 2;
			}

			totalChapterCharsAcc = new int[totalChapters];

			for (int i = 0; i < totalChapters; i++)
			{
				totalChapterCharsAcc[i] = Util.readInt(index_data, offs);
				offs += 4;
			}


			totalVerseCharsAcc = new int[(index_data.Length - offs) / 2];


			for (int i = 0; offs < index_data.Length; i++)
			{
				totalVerseCharsAcc[i] = Util.readShort(index_data, offs);
				offs += 2;
			}

			//System.out.println("total length " + total_len  + " total chapter " + totalChapters);

			bookOpened = true;

		}

		public virtual int BookPosition
		{
			get
			{
				return bookPosition;
			}
		}

		public static BookInfo createFromData(BiblePlusPDB bible, byte[] data, int offset, int book_pos)
		{
			BookInfo bi = new BookInfo(bible);
			int idx = 0;
			bi.bookPosition = book_pos;
			bi.bookNum = Util.readShort(data, offset + idx);
			idx += 2;
			bi.bookIndex = Util.readShort(data, offset + idx);
			idx += 2;
			bi.totalBookRec = Util.readShort(data, offset + idx);
			if (bi.bookIndex + bi.totalBookRec > bible.Header.RecordCount)
			{
				return null;
			}

			idx += 2;
			bi.simpleName = Util.readStringTrimZero(data, offset + idx, 8, bible.Encoding);
			idx += 8;
			bi.complexName = Util.readStringTrimZero(data, offset + idx, 32, bible.Encoding);
			return bi;
		}

		internal virtual void close()
		{
			index_data = null;
			data = null;
			totalVersesAcc = null;
			totalChapterCharsAcc = null;
			totalVerseCharsAcc = null;
		}

	}

}