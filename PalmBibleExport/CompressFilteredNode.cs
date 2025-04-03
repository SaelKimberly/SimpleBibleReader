using System.Collections;

namespace PalmBibleExport
{


	public class CompressFilteredNode
	{
		protected internal string value;
		protected internal int count;
		protected internal int size;
		protected internal CompressFilteredNode leftNode;
		protected internal CompressFilteredNode rightNode;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CompressFilteredNode(String value, int count) throws Exception
		public CompressFilteredNode(string value, int count)
		{
			this.value = value;
			this.count = count;
			this.size = value.Length * (count - 1);
		}

		public virtual void putNode(CompressFilteredNode node)
		{
			if (size == node.size)
			{
				if (value.Length < node.value.Length)
				{
					if (leftNode == null)
					{
						leftNode = node;
					}
					else
					{
						leftNode.putNode(node);
					}
				}
				else
				{
					if (rightNode == null)
					{
						rightNode = node;
					}
					else
					{
						rightNode.putNode(node);
					}
				}
			}
			else if (size < node.size)
			{
				if (leftNode == null)
				{
					leftNode = node;
				}
				else
				{
					leftNode.putNode(node);
				}
			}
			else
			{
				if (rightNode == null)
				{
					rightNode = node;
				}
				else
				{
					rightNode.putNode(node);
				}
			}
		}

		public virtual void fillVector(ArrayList vector, IDictionary indexMap, int max)
		{
			if (leftNode != null)
			{
				leftNode.fillVector(vector, indexMap, max);
			}

			if (vector.Count < max && count > 2)
			{

				vector.Add(value);
				indexMap[value] = new int?(vector.Count);

				if (rightNode != null)
				{
					rightNode.fillVector(vector, indexMap, max);
				}
			}
		}
	}

}