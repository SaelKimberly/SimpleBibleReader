using System.Collections;

namespace PalmBibleExport
{



	public class WordRecords
	{
		protected internal const int recordSize = 4096;
		protected internal ArrayList content = new ArrayList();
		protected internal ArrayList wordIndex = new ArrayList();

		public WordRecords(WordRecorder wordRecorder, Compressor compressor)
		{
			WordRecord wordRecord = new WordRecord();
			content.Add(wordRecord);

			int totalWord = wordRecorder.WordCount;
			int charIndex = 0;

			for (int i = 0; i < totalWord; i++)
			{
				string word = wordRecorder.getNewWordString(i + 1);
				int totalChar = word.Length;

				if (charIndex + totalChar <= recordSize)
				{
					wordRecord.addByte(word);
					charIndex += totalChar;
				}
				else
				{
					int trails = recordSize - charIndex;

					string part1 = word.Substring(0, trails);
					string part2 = word.Substring(trails);

					charIndex = totalChar - trails;

					wordRecord.addByte(part1);

					wordRecord = new WordRecord();
					content.Add(wordRecord);

					wordRecord.addByte(part2);
				}
			}

			int totalCompressedWord = compressor.TotalCompressedWord;

			for (int i = 0; i < totalCompressedWord; i++)
			{
				string word = compressor.getCompressedWord(i + 1);
				int totalChar = word.Length;

				for (int j = 0; j < totalChar; j++)
				{
					if (charIndex >= recordSize)
					{
						wordRecord = new WordRecord();
						content.Add(wordRecord);
						charIndex = 0;
					}

					int compressedChar = word[j];
					int upperByte = compressedChar >> 8;
					wordRecord.addByte(upperByte);
					charIndex++;

					if (charIndex >= recordSize)
					{
						wordRecord = new WordRecord();
						content.Add(wordRecord);
						charIndex = 0;
					}

					int lowerByte = compressedChar;
					wordRecord.addByte(lowerByte);
					charIndex++;
				}
			}
		}

		public virtual int TotalRecord
		{
			get
			{
				return content.Count;
			}
		}

		public virtual WordRecord getRecord(int index)
		{
			return (WordRecord) content[index];
		}
	}

}