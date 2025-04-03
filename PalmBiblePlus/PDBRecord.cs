namespace PalmBiblePlus
{

	internal class PDBRecord
	{

		private byte[] data;

		public PDBRecord(byte[] data)
		{
			this.data = data;
		}

		public virtual byte[] Data
		{
			get
			{
				return data;
			}
		}
	}

}