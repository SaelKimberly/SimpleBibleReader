namespace PalmBiblePlus
{


	public abstract class PDBDataStream
	{

		public abstract int CurrentPosition {get;}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void read(byte[] data) throws java.io.IOException;
		public abstract void read(byte[] data);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void skip(int nbytes) throws java.io.IOException;
		public abstract void skip(int nbytes);
		/// <summary>
		/// if canSeek is false, it should be able to seek forward using skip
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void seek(int position) throws java.io.IOException
		public virtual void seek(int position)
		{
			int cpos = CurrentPosition;
			skip(position - cpos);
		}
		/// <summary>
		/// can seek backward/forward
		/// </summary>
		public abstract bool canSeek();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void close() throws java.io.IOException;
		public abstract void close();
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract long getSize() throws java.io.IOException;
		public abstract long Size {get;}

		public abstract string PathName {get;}
	}

}