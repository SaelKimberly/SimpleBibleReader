using System.Collections;
using System.Collections.Generic;

namespace PalmBibleExport
{



	public class WordRecorder
	{
		public const int BYTE_SHIFTED_LIMIT = 0x36FF;
		public const int BYTE_SHIFTED_MAX = 0x3FE0;
		public const int BYTE_UNSHIFTED_MAX = 0xFFE0;

		protected internal string enc;
		protected internal int wordCount;
		//protected internal IDictionary words = new SortedDictionary();
        protected internal Hashtable words = new Hashtable();

		protected internal ArrayList indexs = new ArrayList();

		protected internal IDictionary sortIndex = new Hashtable();
		protected internal ArrayList sorted = new ArrayList();

		protected internal TreeNode treeRoot;

		public WordRecorder(string enc)
		{
			this.enc = enc;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getWordIndex(String word) throws Exception
		public virtual int getWordIndex(string word)
		{
            int? indexObj = null;
            
            if(words.ContainsKey(word))
                indexObj=(int?)words[word];

			if (indexObj == null)
			{
				wordCount++;
				indexObj = new int?(wordCount);
				words[word] = (int)indexObj;
				indexs.Add(word);

				TreeNode treeNode = new TreeNode(word, wordCount, enc);
				if (treeRoot == null)
				{
					treeRoot = treeNode;
				}
				else
				{
					treeNode.putNode(treeRoot);
				}

				if (wordCount > BYTE_UNSHIFTED_MAX)
				{
					throwWordCountLimitExceeded();
				}

				return wordCount;
			}
			else
			{
				return (int)indexObj;
			}
		}

		public virtual string getWordString(int index)
		{
			return (string) indexs[index - 1];
		}

		public virtual int WordCount
		{
			get
			{
				return wordCount;
			}
		}

		public virtual int getNewWordIndex(int index)
		{
			if (index <= 0xFFF0)
			{
				int? indexObj = (int?) sortIndex[new int?(index)];
				return (int)indexObj;
			}
			else
			{
				return index;
			}
		}

		public virtual string getNewWordString(int index)
		{
			if (index < 0xFFF0)
			{
				return (string) sorted[index - 1];
			}
			else
			{
				return "";
			}
		}

		public virtual void sort()
		{
			treeRoot.fillSorted(sorted, sortIndex);
			// KW: word sorting completed so release references for garbage collection
			treeRoot = null;
			words = null;
			indexs = null;
		}

		public virtual bool ByteShifted
		{
			get
			{
				if (wordCount < WordRecorder.BYTE_SHIFTED_LIMIT)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void throwWordCountLimitExceeded() throws ParserException
		protected internal virtual void throwWordCountLimitExceeded()
		{
			throw new ParserException("Word Count Limit Exceeded. You may try breaking the Bible version into parts.");
		}
	}

}