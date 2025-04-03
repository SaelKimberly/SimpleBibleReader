namespace PalmBibleExport
{


	public class BookRecord : PDBRecord
	{
		public BookRecord()
		{
		}

		public virtual void addShort(int value)
		{
			content.addShort(value);
		}
	}

}