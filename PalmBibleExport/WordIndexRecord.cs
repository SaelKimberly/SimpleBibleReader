namespace PalmBibleExport
{


	public class WordIndexRecord : PDBRecord
	{
		protected internal const int WORD_INDEX_LIMIT = 4095;

		public WordIndexRecord(WordRecorder wordRecorder, Compressor compressor)
		{
			int i, totalWordInEntry, totalIndex;

			ByteBuffer byteBuffer = new ByteBuffer();
			totalIndex = 0;

			int totalWord = wordRecorder.WordCount;
			int length = 0;
			totalWordInEntry = 0;
			for (i = 0; i < totalWord; i++)
			{
				string word = wordRecorder.getNewWordString(i + 1);

				if (length == 0)
				{
					length = word.Length;
				}

				if (word.Length != length || totalWordInEntry >= WORD_INDEX_LIMIT)
				{
					byteBuffer.addShort(length);
					byteBuffer.addShort(totalWordInEntry);
					byteBuffer.addShort(0);
					length = word.Length;
					totalIndex++;
					totalWordInEntry = 0;
				}

				totalWordInEntry++;
			}

			byteBuffer.addShort(length);
			byteBuffer.addShort(totalWordInEntry);
			byteBuffer.addShort(0);
			totalIndex++;

			int totalCompressedWord = compressor.TotalCompressedWord;
			length = 0;

			totalWordInEntry = 0;
			for (i = 0; i < totalCompressedWord; i++)
			{
				string word = compressor.getCompressedWord(i + 1);

				if (length == 0)
				{
					length = word.Length;
				}

				if (word.Length != length || totalWordInEntry >= WORD_INDEX_LIMIT)
				{
					byteBuffer.addShort(length * 2);
					byteBuffer.addShort(totalWordInEntry);
					byteBuffer.addByte(1);
					byteBuffer.addByte(1);
					length = word.Length;
					totalIndex++;
					totalWordInEntry = 0;
				}

				totalWordInEntry++;
			}

			byteBuffer.addShort(length * 2);
			byteBuffer.addShort(totalWordInEntry);
			byteBuffer.addByte(1);
			byteBuffer.addByte(1);
			totalIndex++;
			content.addShort(totalIndex);
			content.addByteBuffer(byteBuffer);
		}
	}

}