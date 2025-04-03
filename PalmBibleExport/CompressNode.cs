using System.Collections;
using System.Text;

namespace PalmBibleExport
{



	public class CompressNode
	{
		protected internal int count;
		protected internal string value;
		protected internal CompressNode leftNode;
		protected internal CompressNode rightNode;

		public CompressNode(string verse, int start, int length, WordRecorder wordRecorder)
		{
			StringBuilder buffer = new StringBuilder();

			count = 1;

			for (int i = 0; i < length; i++)
			{
				int index = verse[start + i];

				if (index < 0xFFF0)
				{
					buffer.Append((char) wordRecorder.getNewWordIndex(index));
				}
				else
				{
					buffer.Append((char) index);
				}
			}

			value = buffer.ToString();
		}

		public virtual bool putNode(CompressNode node)
		{
			int result = node.value.CompareTo(value);

			if (result == 0)
			{
				count++;
				return true;
			}
			else if (result < 0)
			{
				if (leftNode == null)
				{
					leftNode = node;
					return false;
				}
				else
				{
					return leftNode.putNode(node);
				}
			}
			else
			{
				if (rightNode == null)
				{
					rightNode = node;
					return false;
				}
				else
				{
					return rightNode.putNode(node);
				}
			}
		}

		public virtual void fillVector(ArrayList vector, IDictionary countMap, int minCount)
		{
			if (leftNode != null)
			{
				leftNode.fillVector(vector, countMap, minCount);
			}

			if (count > minCount)
			{
				vector.Add(value);
				countMap[value] = new int?(count);
			}

			if (rightNode != null)
			{
				rightNode.fillVector(vector, countMap, minCount);
			}
		}
	}

}