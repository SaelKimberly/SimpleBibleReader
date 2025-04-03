using System;
using System.Collections;
using System.IO;
using System.Text;

namespace PalmBibleExport
{



	public class PDBFile
	{
		MyBinaryWriter dataStream;         
		// RWS 2004.12.14	
		protected internal bool quiet = true;

		protected internal const int HEADER_SIZE = 78;
        FileStream file;
		protected internal string pdbName;
		protected internal byte[] filler = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
		protected internal short fileAttributes = 24;
		protected internal short version = 1;
		protected internal int creationDate;
		protected internal int modificationDate;
		protected internal int lastBackupDate;
		protected internal int modificationNumber = 0;
		protected internal int appInfoArea = 0;
		protected internal int sortInfoArea = 0;
		protected internal byte[] databaseType = {0,0,0,0};
		protected internal byte[] creatorID = {0,0,0,0};
		protected internal int uniqueIDSeed = 0;
		protected internal int nextRecord = 0;
		protected internal short numRecs = 0;

		protected internal ArrayList records;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PDBFile(String fileName) throws java.io.IOException
		public PDBFile(string fileName) : this(fileName, "UNDEFINED")
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PDBFile(String fileName, String pdbName) throws java.io.IOException
		public PDBFile(string fileName, string pdbName) : this(fileName, pdbName, "USER")
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PDBFile(String fileName, String pdbName, String creatorID) throws java.io.IOException
		public PDBFile(string fileName, string pdbName, string creatorID) : this(fileName, pdbName, creatorID, "DATA")
		{
		}

		// RWS 2004.12.14 keep original constructor functionality defaulting quiet to false
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PDBFile(String fileName, String pdbName, String creatorID, String databaseType) throws java.io.IOException
		public PDBFile(string fileName, string pdbName, string creatorID, string databaseType) : this(fileName, pdbName, creatorID, databaseType, false)
		{
		}

		// RWS 2004.12.14 add constructor with quiet parm
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PDBFile(String fileName, String pdbName, String creatorID, String databaseType, boolean quiet) throws java.io.IOException
		public PDBFile(string fileName, string pdbName, string creatorID, string databaseType, bool quiet)
		{
			file = new FileStream(fileName,FileMode.CreateNew);
            dataStream = new MyBinaryWriter(file);

			PDBName = pdbName;
			CreatorID = creatorID;
			DatabaseType = databaseType;
			this.quiet = quiet;

			creationDate = convertPalmDate(DateTime.Now);
			modificationDate = creationDate;
			lastBackupDate = creationDate;

			records = new ArrayList();
		}

		public virtual string PDBName
		{
			get
			{
				return pdbName;
			}
			set
			{
				this.pdbName = value;
			}
		}


		public virtual short FileAttributes
		{
			get
			{
				return fileAttributes;
			}
			set
			{
				this.fileAttributes = value;
			}
		}


		public virtual short Version
		{
			get
			{
				return version;
			}
			set
			{
				this.version = value;
			}
		}


		public virtual DateTime CreationDate
		{
			get
			{
				return convertPCDate(creationDate);
			}
			set
			{
				this.creationDate = convertPalmDate(value);
			}
		}


		public virtual DateTime ModificationDate
		{
			get
			{
				return convertPCDate(modificationDate);
			}
			set
			{
				this.modificationDate = convertPalmDate(value);
			}
		}


		public virtual DateTime LastBackupDate
		{
			get
			{
				return convertPCDate(lastBackupDate);
			}
			set
			{
				this.lastBackupDate = convertPalmDate(value);
			}
		}


		public virtual string DatabaseType
		{
			get
			{
				return Encoding.UTF8.GetString(databaseType);
			}
			set
			{
				this.databaseType[0] = (byte) value[0];
				this.databaseType[1] = (byte) value[1];
				this.databaseType[2] = (byte) value[2];
				this.databaseType[3] = (byte) value[3];
			}
		}


		public virtual string CreatorID
		{
			get
			{
				return Encoding.UTF8.GetString(creatorID);
			}
			set
			{
				this.creatorID[0] = (byte) value[0];
				this.creatorID[1] = (byte) value[1];
				this.creatorID[2] = (byte) value[2];
				this.creatorID[3] = (byte) value[3];
			}
		}


		public virtual short NumRecs
		{
			get
			{
				return numRecs;
			}
		}

		public virtual void addRecord(PDBRecord pdbRecord)
		{
			records.Add(pdbRecord);
		}

		public virtual void insertRecord(PDBRecord pdbRecord, int i)
		{
			records.Insert(i, pdbRecord);
		}

		public virtual PDBRecord getRecord(int i)
		{
			return (PDBRecord) records[i];
		}

		public virtual void removeRecord(int i)
		{
			records.RemoveAt(i);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public virtual void close()
		{
			writeHeader();
			writeRecordList();
			writeFiller();
			writeRecords();

			dataStream.Close();
            file.Close();
		}

		protected internal virtual void writeHeader()
		{
			try
			{
                byte[] bytes = Encoding.UTF8.GetBytes(pdbName);
				dataStream.Write (bytes,0,bytes.Length);
				dataStream.Write(filler, 0, 32 - bytes.Length);
				dataStream.Write(fileAttributes);
                dataStream.Write(version);
                dataStream.Write(creationDate);
                dataStream.Write(modificationDate);
                dataStream.Write(lastBackupDate);
                dataStream.Write(modificationNumber);
                dataStream.Write(appInfoArea);
                dataStream.Write(sortInfoArea);
                dataStream.Write(databaseType);
                dataStream.Write(creatorID);
                dataStream.Write(uniqueIDSeed);
                dataStream.Write(nextRecord);
                dataStream.Write((short)records.Count);
			}
			catch (IOException e)
			{
				Console.Error.WriteLine("ERROR Writing PDB Header:");
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writeRecordList() throws java.io.IOException
		protected internal virtual void writeRecordList()
		{
			int runningTotal = HEADER_SIZE;
			runningTotal += 8 * records.Count + 2;

			for (int i = 0; i < records.Count; i++)
			{
				PDBRecord pdbRecord = (PDBRecord) records[i];

                dataStream.Write(runningTotal);
	// RWS: debug help for very large records
	if (!quiet)
	{
		Console.WriteLine("Record " + (i + 1) + " offset: " + runningTotal + "  size: " + pdbRecord.size());
	}
        dataStream.Write(pdbRecord.Attributes);
        dataStream.Write(filler, 0, 3);

				runningTotal += pdbRecord.size();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writeFiller() throws java.io.IOException
		protected internal virtual void writeFiller()
		{
			dataStream.Write(filler, 0, 2);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writeRecords() throws java.io.IOException
		protected internal virtual void writeRecords()
		{
			for (int i = 0; i < records.Count; i++)
			{
				PDBRecord pdbRecord = (PDBRecord) records[i];
				ByteBuffer byteBuffer = pdbRecord.Bytes;
				byteBuffer.reset();

				for (int j = 0; j < pdbRecord.size(); j++)
				{
					dataStream.Write(byteBuffer.read());
				}
			}
		}

		private int convertPalmDate(DateTime date)
		{
			DateTime calenderNow = new DateTime();
			calenderNow = new DateTime(1904, 1, 1);
			DateTime dateSince = new DateTime(date.Ticks + calenderNow.Ticks);
            DateTime jan1970 = new DateTime(1970, 1, 1);
            long date_millis = (long)((dateSince - jan1970).TotalMilliseconds);
            return (int)(date_millis / 1000);
		}

		private DateTime convertPCDate(int date)
		{
			DateTime calenderNow = new DateTime();
			calenderNow = new DateTime(1904, 1, 1);
            DateTime jan1970 = new DateTime(1970, 1, 1);
            long cn_millis = (long)((calenderNow - jan1970).TotalMilliseconds);
            return new DateTime(date * 1000 - cn_millis);
		}
	}


}