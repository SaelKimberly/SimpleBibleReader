using System.Text;
using System.IO;
using System;
using PalmBiblePlusCSharp.bibleplus;

namespace PalmBiblePlus
{


	internal class PDBHeader
	{
		/* 78 bytes total */
		//char	 name[ dmDBNameLength ]; /*0*/
		private byte[] _name;
		private int attributes; //32
		private int version; //34
		private int create_time; //36
		private int modify_time; //40
		private int backup_time; //44
		private int modification_number; //48
		private int app_info_id; //52
		private int sort_info_id; //56
		private string type_str; //4 char (offset 60)
		private byte[] type;
		private string creator_str; //4 char (offset 64)
		private byte[] creator;
		private int id_seed; // 68
		private int next_record_list; //72
		private int num_records; //76-78

		private byte[] headerdata;


		public virtual string Creator
		{
			get
			{
				return creator_str;
			}
		}

		public virtual string Type
		{
			get
			{
				return type_str;
			}
		}

		public virtual int RecordCount
		{
			get
			{
				return num_records;
			}
		}

		public PDBHeader(byte[] headerdata)
		{
			this.headerdata = headerdata;
		}

		public virtual string ToString()
		{
			StringBuilder sb = new StringBuilder();
            sb.Append("Name: ").Append(getName("UTF-8")).Append("\n");
            sb.Append("type_str: ").Append(type_str).Append("\n");
            sb.Append("creator_str: ").Append(creator_str).Append("\n");
            sb.Append("num records: ").Append(num_records).Append("\n");
			return sb.ToString();
		}

		internal virtual string getName(string encoding)
		{
			return Util.readStringTrimZero(_name, 0, 32, encoding);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load() throws java.io.IOException
		public virtual void load()
		{
            //Console.WriteLine(headerdata.Length);
            MemoryStream bis = new MemoryStream(headerdata);
            MyBinaryReader dis = new MyBinaryReader(bis);
			_name = new byte[32];
			dis.Read(_name,0,32);
            attributes = dis.ReadInt16();
            version = dis.ReadInt16();
            create_time = dis.ReadInt32();
            modify_time = dis.ReadInt32();
            backup_time = dis.ReadInt32();
            modification_number = dis.ReadInt32();
            app_info_id = dis.ReadInt32();
            sort_info_id = dis.ReadInt32();
			type = new byte[4];
			dis.Read(type, 0, 4);
			type_str = Encoding.UTF8.GetString(type);
			creator = new byte[4];
			dis.Read(creator, 0, 4);
            creator_str = Encoding.UTF8.GetString(creator);
            id_seed = dis.ReadInt32();
            next_record_list = dis.ReadInt32();
            num_records = dis.ReadInt16();
		}

	}

}