using System.Collections;
using System.Text;

namespace PalmBibleExport
{


	public class TreeNode
	{
		protected internal string enc;

		public string @string;
		public int index;
		public TreeNode leftNode = null;
		public TreeNode rightNode = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TreeNode(String string, int index) throws Exception
		public TreeNode(string @string, int index)
		{
			this.@string = @string;
			this.index = index;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TreeNode(String string, int index, String enc) throws Exception
		public TreeNode(string @string, int index, string enc)
		{
            this.@string = @string;// new string(Encoding.GetEncoding(enc).GetBytes(@string), "iso-8859-1");
			this.index = index;
		}

		public virtual void putNode(TreeNode rootNode)
		{
			if (compare(rootNode, this) > 0)
			{
				if (rootNode.leftNode != null)
				{
					putNode(rootNode.leftNode);
				}
				else
				{
					rootNode.leftNode = this;
				}
			}
			else
			{
				if (rootNode.rightNode != null)
				{
					putNode(rootNode.rightNode);
				}
				else
				{
					rootNode.rightNode = this;
				}
			}
		}

		public virtual void fillSorted(ArrayList words, IDictionary indexMap)
		{
			if (leftNode != null)
			{
				leftNode.fillSorted(words, indexMap);
			}

			words.Add(@string);
			indexMap[new int?(index)] = new int?(words.Count);

			if (rightNode != null)
			{
				rightNode.fillSorted(words, indexMap);
			}
		}

		protected internal virtual int compare(TreeNode node1, TreeNode node2)
		{
			int length1 = node1.@string.Length;
			int length2 = node2.@string.Length;

			if (length1 < length2)
			{
				return -1;
			}
			else if (length1 > length2)
			{
				return 1;
			}
			else
			{
				return node1.@string.CompareTo(node2.@string);
			}
		}
	}

}