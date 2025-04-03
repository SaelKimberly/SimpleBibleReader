using System;
using System.Collections;
using System.IO;

namespace PalmBibleExport
{



	public class PDBDoc
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PDBDoc(BibleDoc bibleDoc, String fileName) throws Exception
		public PDBDoc(BibleDoc bibleDoc, string fileName)
		{
			WordRecorder wordRecorder = bibleDoc.WordRecorder;
			wordRecorder.sort();

			int wordCount = wordRecorder.WordCount;
			//Console.WriteLine("Word Recorder Count ... " + wordCount);
			//Console.WriteLine();

			ByteBuffer.Encoding = bibleDoc.Encoding;

			//Console.WriteLine("Compressing ... ");

			Compressor compressor = new Compressor(bibleDoc, wordRecorder);

			int compressWordCount = compressor.TotalCompressedWord;
			//Console.WriteLine();
			//Console.WriteLine("Compressor Word Count ... " + compressWordCount);
			//Console.WriteLine();

			Console.WriteLine("Processing Word Index and Words Record ...");
			WordIndexRecord wordIndexRecord = new WordIndexRecord(wordRecorder, compressor);
			WordRecords wordRecords = new WordRecords(wordRecorder, compressor);

			//Console.WriteLine("Processing Book Index and Book Records ... ");

			ArrayList booksRecords = new ArrayList();
			ArrayList booksIndexRecord = new ArrayList();

			int totalBook = bibleDoc.TotalBook;
			for (int i = 0; i < totalBook; i++)
			{
				Book book = bibleDoc.getBook(i);

				booksRecords.Add(new BookRecords(book, wordRecorder, compressor));
				booksIndexRecord.Add(new BookIndexRecord(book));
			}

			//Console.WriteLine("Processing Version Info Record ... ");
			VersionInfoRecord versionInfoRecord = new VersionInfoRecord(bibleDoc, wordRecords, booksRecords);

			//Console.WriteLine();
			//Console.WriteLine("Making Palm PDB File ...");
			//Console.WriteLine();

			//File pdbfile = new File();
			if (File.Exists(fileName))
			{
                File.Delete(fileName);
			}

			// RWS 2004.12.14 added quiet parm to new PDBFile
			PDBFile pdbFile = new PDBFile(fileName, Path.GetFileNameWithoutExtension(fileName).ToUpper(), "PPBL", "bibl", bibleDoc.Quiet);

			//Console.WriteLine("Adding Version Info Record ...");
			pdbFile.addRecord(versionInfoRecord);

			//Console.WriteLine("Adding Word Index and Word Records ...");
			pdbFile.addRecord(wordIndexRecord);

			int totalWordRecord = wordRecords.TotalRecord;
			for (int i = 0; i < totalWordRecord; i++)
			{
				pdbFile.addRecord(wordRecords.getRecord(i));
			}

			//Console.WriteLine("Adding Book Index and Book Records ...");
			for (int i = 0; i < totalBook; i++)
			{
				pdbFile.addRecord((BookIndexRecord) booksIndexRecord[i]);

				BookRecords bookRecords = (BookRecords) booksRecords[i];
				int totalBookRecord = bookRecords.TotalRecord;

				for (int j = 0; j < totalBookRecord; j++)
				{
					pdbFile.addRecord(bookRecords.getRecord(j));
				}
			}

			pdbFile.close();

			//Console.WriteLine();
			//Console.WriteLine("Palm PDB File Generated ...");
			//Console.WriteLine();
		}
	}

}