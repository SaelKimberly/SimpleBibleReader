namespace PalmBibleExport
{


	public abstract class PDBRecord
	{
		public static readonly byte SECRET_RECORD_BIT = (byte)16;
		public static readonly byte BUSY_BIT = (byte)32;
		public static readonly byte DIRTY_BIT = (byte)64;
		public static readonly byte DEL_ON_HSYNC_BIT = (byte)128;

		public ByteBuffer content = new ByteBuffer();
		public byte attr = 0;

		public PDBRecord() : base()
		{
		}

		public virtual int size()
		{
			return content.size();
		}

		public virtual byte Attributes
		{
			set
			{
				this.attr = value;
			}
			get
			{
				return attr;
			}
		}


		public virtual ByteBuffer Bytes
		{
			get
			{
				return content;
			}
			set
			{
				this.content = value;
			}
		}

	}

}