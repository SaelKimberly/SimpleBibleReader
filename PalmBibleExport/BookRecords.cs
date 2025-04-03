using System.Collections;

namespace PalmBibleExport
{



	public class BookRecords
	{
		protected internal const int recordSize = 2048;
		protected internal ArrayList content = new ArrayList();

		public BookRecords(Book book, WordRecorder wordRecorder, Compressor compressor)
		{
			BookRecord bookRecord = new BookRecord();
			content.Add(bookRecord);

			int charIndex = 0;
			int totalChapter = book.TotalChapter;

			ShortShifter shortShifter = new ShortShifter(wordRecorder.ByteShifted);
			for (int i = 0; i < totalChapter; i++)
			{
				Chapter chapter = book.getChapter(i);
				int totalVerse = chapter.TotalVerse;

				for (int j = 0; j < totalVerse; j++)
				{
					int compressedCharIndex = 0;
					Verse verse = chapter.getVerse(j);

					int totalChar = verse.TotalChar;
					string verseString = verse.String;

					for (int k = 0; k < totalChar; k++)
					{
						if (charIndex >= recordSize)
						{
							bookRecord = new BookRecord();
							content.Add(bookRecord);
							charIndex = 0;
						}

						int newIndex = wordRecorder.getNewWordIndex(verseString[k]);

						int compressedIndex = compressor.getCompressedIndex(newIndex);

						// KW: zero compressed index should be ignored too
						if (compressedIndex > 0)
						{
							compressedCharIndex++;
							int short14 = shortShifter.getShort(compressedIndex);
							if (short14 != -1)
							{
								bookRecord.addShort(short14);
								charIndex++;
							}
						}
					}

					for (int k = 0; k < Compressor.COMPRESS_DEPTH; k++)
					{
						int compressedIndex = compressor.CompressedIndex;
						// KW: zero compressed index should be ignored too
						if (compressedIndex > 0)
						{
							compressedCharIndex++;

							if (charIndex >= recordSize)
							{
								bookRecord = new BookRecord();
								content.Add(bookRecord);
								charIndex = 0;
							}

							int short14 = shortShifter.getShort(compressedIndex);
							if (short14 != -1)
							{
								bookRecord.addShort(short14);
								charIndex++;
							}
						}
					}

					verse.TotalCompressedChar = compressedCharIndex;
				}
			}

			int short14_1 = shortShifter.flush();
            if (short14_1 != 0)
			{
				if (charIndex >= recordSize)
				{
					bookRecord = new BookRecord();
					content.Add(bookRecord);
					charIndex = 0;
				}

                bookRecord.addShort(short14_1);
				charIndex++;
			}
		}

		public virtual int TotalRecord
		{
			get
			{
				return content.Count;
			}
		}

		public virtual BookRecord getRecord(int index)
		{
			return (BookRecord) content[index];
		}
	}

}