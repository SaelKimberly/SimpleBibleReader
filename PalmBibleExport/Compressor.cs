using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace PalmBibleExport
{



	public class Compressor
	{
		public const int COMPRESS_DEPTH = 2;
		public const int COMPRESS_MIN = 2;

		protected internal int[] lastValue = new int[COMPRESS_DEPTH];
		protected internal int wordRecorderCount = 0;
		protected internal int shiftValue = 0;
		protected internal int filledCount = 0;
		protected internal ArrayList sorted = new ArrayList();
		//protected internal IDictionary sortedIndex = new SortedDictionary();
        protected internal Hashtable sortedIndex = new Hashtable();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Compressor(BibleDoc bibleDoc, WordRecorder wordRecorder) throws Exception
		public Compressor(BibleDoc bibleDoc, WordRecorder wordRecorder)
		{
			wordRecorderCount = wordRecorder.WordCount;

			CompressNode rootNode = null;

			int totalBook = bibleDoc.TotalBook;
			for (int i = 0; i < totalBook; i++)
			{
				Book book = bibleDoc.getBook(i);

				int totalChapter = book.TotalChapter;

				for (int j = 0; j < totalChapter; j++)
				{
					Chapter chapter = book.getChapter(j);
					int totalVerse = chapter.TotalVerse;

					for (int k = 0; k < totalVerse; k++)
					{
						Verse verse = chapter.getVerse(k);
						string verseString = verse.String;

						int totalChar = verse.TotalChar;
						for (int m = 0; m < totalChar - 1; m++)
						{
							bool hasWord = false;
							for (int n = COMPRESS_DEPTH; n >= COMPRESS_MIN && !hasWord; n--)
							{
								if (m + n < totalChar)
								{
									CompressNode node = new CompressNode(verseString, m, n, wordRecorder);

									if (rootNode == null)
									{
										rootNode = node;
									}
									else
									{
										hasWord = rootNode.putNode(node);

										if (hasWord)
										{
											m += n;
										}
									}
								}
							}
						}
					}
				}
			}

			ArrayList elementList = new ArrayList();
			//IDictionary countMap = new SortedDictionary();
            Hashtable countMap = new Hashtable();

			rootNode.fillVector(elementList, countMap, 5);

			CompressFilteredNode rootFilteredNode = null;

			int totalElement = elementList.Count;
			for (int i = 0; i < totalElement; i++)
			{
				string element = (string) elementList[i];
				int? count = (int?) countMap[element];

				CompressFilteredNode node = new CompressFilteredNode(element, (int)count);

				if (rootFilteredNode == null)
				{
					rootFilteredNode = node;
				}
				else
				{
					rootFilteredNode.putNode(node);
				}
			}

			ArrayList filteredElementList = new ArrayList();
			// KW: rootFilteredNode can be null if no repeat patterns with count > 5
			if (rootFilteredNode != null)
			{
				rootFilteredNode.fillVector(sorted, sortedIndex, getMaxValue(wordRecorder));
			}

			// KW: sorting completed, release the references for garbage collection
			rootNode = null;
			rootFilteredNode = null;
		}

		public virtual int getMaxValue(WordRecorder wordRecorder)
		{
			int maxValue;

			if (wordRecorder.ByteShifted)
			{
				maxValue = WordRecorder.BYTE_SHIFTED_MAX - wordRecorderCount;
			}
			else
			{
				maxValue = WordRecorder.BYTE_UNSHIFTED_MAX - wordRecorderCount;
			}

			return maxValue;
		}

		public virtual int TotalCompressedWord
		{
			get
			{
				return sorted.Count;
			}
		}

		public virtual string getCompressedWord(int index)
		{
			return (string) sorted[index - 1];
		}

		internal int temp = 0;

		public virtual int getCompressedIndex(int value)
		{
			StringBuilder buffer = new StringBuilder();

			LastValue = value;

			if (filledCount == COMPRESS_DEPTH)
			{
				for (int i = 0; i < COMPRESS_DEPTH; i++)
				{
					buffer.Append((char) getLastValue(i));
                    int? index = null;
                    if(sortedIndex.ContainsKey(buffer.ToString()))
                        index=(int?)sortedIndex[buffer.ToString()];

					if (index != null)
					{

						flushLastValue(i + 1);
						return (int)index + wordRecorderCount;
					}
				}

				int resultValue = getLastValue(0);

				flushLastValue(1);
				return resultValue;
			}
			else
			{
				return -1;
			}
		}

		public virtual int CompressedIndex
		{
			get
			{
				return getCompressedIndex(0);
			}
		}

		protected internal virtual int getLastValue(int i)
		{
			if (shiftValue >= COMPRESS_DEPTH)
			{
				shiftValue -= COMPRESS_DEPTH;
			}

			int index = i + shiftValue;

			while (index >= COMPRESS_DEPTH)
			{
				index -= COMPRESS_DEPTH;
			}

			return lastValue[index];
		}

		protected internal virtual int LastValue
		{
			set
			{
				filledCount++;
    
				if (shiftValue >= COMPRESS_DEPTH)
				{
					shiftValue -= COMPRESS_DEPTH;
				}
    
				int index = shiftValue + filledCount - 1;
    
				while (index >= COMPRESS_DEPTH)
				{
					index -= COMPRESS_DEPTH;
				}
    
				lastValue[index] = value;
			}
		}

		protected internal virtual void flushLastValue(int count)
		{
			shiftValue += count;
			filledCount -= count;
		}
	}

}