using System.Collections;

namespace PalmBibleExport
{


	public class ByteBuffer
	{
		protected internal static string enc;

		protected internal ArrayList elements = new ArrayList();
		protected internal int elementIndex;

		protected internal byte[] currentBytes;
		protected internal int byteIndex;
		protected internal int size_Renamed = 0;

		public ByteBuffer()
		{
		}

		public static string Encoding
		{
			set
			{
				ByteBuffer.enc = value;
			}
		}

		public virtual void addByte(int value)
		{
			byte[] element = new byte[1];
			element[0] = (byte)(value & 0x000000FF);
			elements.Add(element);
			size_Renamed += 1;
		}

		public virtual void addByte(string @string)
		{
			addByte(@string, @string.Length);
		}

		public virtual void addByte(string @string, int length)
		{
			int strLen = @string.Length;

			byte[] element = new byte[length];
			for (int i = 0; i < length; i++)
			{
				if (i < strLen)
				{
					element[i] = (byte) @string[i];
				}
				else
				{
					element[i] = 0;
				}
			}

			elements.Add(element);
			size_Renamed += length;
		}

		public virtual void addShort(int value)
		{
			byte[] shortValue = new byte[2];

			shortValue[0] = (byte)((value & 0x0000FF00) >> 8);
			shortValue[1] = (byte)(value & 0x000000FF);

			elements.Add(shortValue);
			size_Renamed += 2;
		}

		public virtual void addInt(int value)
		{
			byte[] intValue = new byte[4];

			intValue[0] = (byte)((value & 0xFF000000) >> 24);
			intValue[1] = (byte)((value & 0x00FF0000) >> 16);
			intValue[2] = (byte)((value & 0x0000FF00) >> 8);
			intValue[3] = (byte)(value & 0x000000FF);

			elements.Add(intValue);
			size_Renamed += 4;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addString(String string, int length) throws Exception
		public virtual void addString(string @string, int length)
		{
			byte[] element = new byte[length];

			byte[] encByte =  System.Text.Encoding.GetEncoding(enc).GetBytes( @string);
			int strLen = encByte.Length;

			for (int i = 0; i < length - 1; i++)
			{
				if (i < strLen)
				{
					element[i] = encByte[i];
				}
				else
				{
					element[i] = 0;
				}
			}
			element[length - 1] = 0;

			elements.Add(element);
			size_Renamed += length;
		}

		public virtual void addByteBuffer(ByteBuffer byteBuffer)
		{
			ArrayList vector = byteBuffer.elements;
			int totalElements = vector.Count;

			for (int i = 0; i < totalElements; i++)
			{
				elements.Add(vector[i]);
			}

			size_Renamed += byteBuffer.size_Renamed;
		}

		public virtual void reset()
		{
			currentBytes = (byte[]) elements[0];
			byteIndex = 0;
			elementIndex = 0;
		}

		public virtual byte read()
		{
			byte result = 0;

			if (byteIndex < currentBytes.Length)
			{
				result = currentBytes[byteIndex];
				byteIndex++;
			}
			else
			{
				if (elementIndex < elements.Count - 1)
				{
					elementIndex++;
					currentBytes = (byte[]) elements[elementIndex];
					result = currentBytes[0];
					byteIndex = 1;
				}
			}

			return result;
		}

		public virtual int size()
		{
			return size_Renamed;
		}
	}

}