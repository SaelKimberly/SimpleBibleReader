namespace PalmBibleExport
{


	public class BookIndexRecord : PDBRecord
	{
		public BookIndexRecord(Book book)
		{
			int totalChapter = book.TotalChapter;
			content.addShort(totalChapter);

			int totalVerseAcc = 0;
			int totalChapterCharAcc = 0;

			ByteBuffer verseCharBuffer = new ByteBuffer();
			ByteBuffer chapterCharBuffer = new ByteBuffer();

			for (int i = 0; i < totalChapter; i++)
			{
				int totalChapterChar = 0;

				chapterCharBuffer.addInt(totalChapterCharAcc);

				Chapter chapter = book.getChapter(i);
				int totalVerse = chapter.TotalVerse;
				totalVerseAcc += totalVerse;

				content.addShort(totalVerseAcc);

				int totalVerseCharAcc = 0;

				for (int j = 0; j < totalVerse; j++)
				{
					Verse verse = chapter.getVerse(j);
					int totalVerseChar = verse.TotalCompressedChar;
					totalVerseCharAcc += totalVerseChar;
					verseCharBuffer.addShort(totalVerseCharAcc);
				}

				totalChapterCharAcc += totalVerseCharAcc;
			}

			content.addByteBuffer(chapterCharBuffer);
			content.addByteBuffer(verseCharBuffer);
		}
	}

}