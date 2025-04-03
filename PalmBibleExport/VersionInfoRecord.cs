using System.Collections;

namespace PalmBibleExport
{

	public class VersionInfoRecord : PDBRecord
	{
		protected internal const int COPY_PROTECTED = 1;
		protected internal const int BYTE_UNSHIFTED = 2;
		protected internal const int RIGHT_ALIGNED = 4;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public VersionInfoRecord(BibleDoc bibleDoc, WordRecords wordRecords, java.util.Vector booksRecords) throws Exception
		public VersionInfoRecord(BibleDoc bibleDoc, WordRecords wordRecords, ArrayList booksRecords)
		{
			content.addString(bibleDoc.BibleDocName, 16);
			content.addString(bibleDoc.BibleDocInfo, 128);

			if (bibleDoc.BreakUnicode)
			{
				content.addByte(0);
			}
			else
			{
				content.addByte(' ');
			}

			int verAttrib = 0;
			if (bibleDoc.CopyProtected)
			{
				verAttrib |= COPY_PROTECTED;
			}

			if (!bibleDoc.ByteShifted)
			{
				verAttrib |= BYTE_UNSHIFTED;
			}

			if (bibleDoc.RightAligned)
			{
				verAttrib |= RIGHT_ALIGNED;
			}
			content.addByte(verAttrib);

			int totalWordRecord = wordRecords.TotalRecord;
			content.addShort(1);
			content.addShort(totalWordRecord);

			int totalBooks = bibleDoc.TotalBook;
			content.addShort(totalBooks);

			int bookRecordNo = totalWordRecord + 2;
			for (int i = 0; i < totalBooks; i++)
			{
				Book book = bibleDoc.getBook(i);

				BookRecords bookRecords = (BookRecords) booksRecords[i];
				int totalBookRecord = bookRecords.TotalRecord;

				content.addShort(book.BookNumber);
				content.addShort(bookRecordNo);
				content.addShort(totalBookRecord);
				content.addString(book.BookShortcut, 8);
				content.addString(book.BookName, 32);

				bookRecordNo += totalBookRecord + 1;
			}
		}
	}

}