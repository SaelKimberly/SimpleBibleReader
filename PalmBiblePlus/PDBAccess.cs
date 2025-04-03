using System;
namespace PalmBiblePlus
{


	internal class PDBAccess
	{

		private bool read_all;

		private string filename;
		private int filesize;
		private PDBHeader header;
		private PDBRecord[] records;
		private byte[] header_data;
		private PDBDataStream @is;

		private int[] record_offsets;
		private int[] record_attrs;

		private bool is_corrupted;

		public PDBAccess(PDBDataStream @is)
		{
			this.@is = @is;
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void loadRecord(int recno) throws java.io.IOException
		private void loadRecord(int recno)
		{
			int length = 0;
			if (recno < records.Length - 1)
			{
				length = record_offsets[recno + 1] - record_offsets[recno];
			}
			else
			{
				length = (int)@is.Size - record_offsets[recno];
			}
			@is.seek(record_offsets[recno]);
			byte[] data = new byte[length];
			@is.read(data);
			PDBRecord pr = new PDBRecord(data);
			records[recno] = pr;
		}


		public virtual void removeFromCache(int recno)
		{
			records[recno] = null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PDBRecord readRecord(int recno) throws java.io.IOException
		public virtual PDBRecord readRecord(int recno)
		{
            PDBRecord pr = records[recno];
		    if (pr==null) {
			    if (@is.canSeek()) {
				    loadRecord(recno);
			    }
		    }
		    return records[recno];
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readOffsets() throws java.io.IOException
		private void readOffsets()
		{

			int n = header.RecordCount;

			byte[] temp_read = new byte[8 * n];
			@is.read(temp_read);
			int offs = 0;
			record_offsets = new int[n];
			record_attrs = new int[n];
			for (int i = 0; i < n; i++)
			{

				int val = (((temp_read[offs + 0] & 0xff) << 24) | ((temp_read[offs + 1] & 0xff) << 16) | ((temp_read[offs + 2] & 0xff) << 8) | (temp_read[offs + 3] & 0xff));

				record_offsets[i] = val;
				//System.out.println("offsets: " + record_offsets[i]);
				record_attrs[i] = temp_read[offs + 4];
				offs += 8;
			}
		}

		public virtual bool Corrupted
		{
			get
			{
				return is_corrupted;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PDBHeader getHeader() throws java.io.IOException
		public virtual PDBHeader Header
		{
			get
			{
    
    
    
				if (header == null)
				{
					header_data = new byte[78];
					@is.read(header_data);
					header = new PDBHeader(header_data);
					header.load();
					//Console.Write(header);
					records = new PDBRecord[header.RecordCount];
					readOffsets();
    
					if (record_offsets[record_offsets.Length - 1] > @is.Size)
					{
						is_corrupted = true;
						return null;
					}
    
					if (!@is.canSeek())
					{
						readAll();
					}
				}
				return header;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void readAll() throws java.io.IOException
		internal virtual void readAll()
		{
			//System.out.println("reading all");
			for (int i = 0; i < header.RecordCount; i++)
			{
				loadRecord(i);
			}
		}

		internal virtual void close()
		{
			try
			{
				@is.close();
				//make sure it is garbage collected
				header = null;
				for (int i = 0; i < records.Length; i++)
				{
					records[i] = null;
				}
				records = null;
				header_data = null;
				@is = null;
				record_offsets = null;
				record_attrs = null;

			}
			catch (Exception e)
			{

			}

		}

	}

}