namespace PalmBibleExport
{


	public class WordRecord : PDBRecord
	{
		public WordRecord()
		{
		}

		public virtual void addByte(string @string)
		{
			content.addByte(@string);
		}

		public virtual void addByte(int value)
		{
			content.addByte(value);
		}
	}

}