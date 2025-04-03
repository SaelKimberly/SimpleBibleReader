using System.IO;
namespace PalmBiblePlus
{



	internal class PDBFileStream : PDBDataStream
	{

		internal FileStream fis;
		internal int pos;

		public override int CurrentPosition
		{
			get
			{
				return pos;
			}
		}

		internal string filename;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: PDBFileStream(String _filename) throws java.io.IOException
		internal PDBFileStream(string _filename)
		{
			filename = _filename;
            fis = new FileStream(filename, FileMode.Open);
			pos = 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void read(byte[] data) throws java.io.IOException
		public override void read(byte[] data)
		{
			fis.Read(data,0,data.Length);
			pos += data.Length;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void seek(int position) throws java.io.IOException
		public override void seek(int position)
		{
			fis.Seek(position,SeekOrigin.Begin);
			pos = position;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void skip(int nbytes) throws java.io.IOException
		public override void skip(int nbytes)
		{
			seek(pos + nbytes);
		}

		public override bool canSeek()
		{
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
		public override void close()
		{
            fis.Close();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long getSize() throws java.io.IOException
		public override long Size
		{
			get
			{
                return fis.Length;
			}
		}

		public override string PathName
		{
			get
			{
				return filename;
			}
		}
	}

}