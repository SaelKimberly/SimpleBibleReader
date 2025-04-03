namespace PalmBibleExport
{

	public class ShortShifter
	{
		protected internal bool byteShifted = false;
		protected internal int shift = 0;
		protected internal int lastShort = 0;

		public ShortShifter(bool byteShifted)
		{
			this.byteShifted = byteShifted;
		}

		public virtual int getShort(int input)
		{
			if (byteShifted)
			{
				int result = 0;

				input = input & 0x3FFF;
				switch (shift)
				{
					case 0:
						result = -1;
						break;
					case 1:
						result = lastShort << 2 | input >> 12;
						break;
					case 2:
						result = lastShort << 4 | input >> 10;
						break;
					case 3:
						result = lastShort << 6 | input >> 8;
						break;
					case 4:
						result = lastShort << 8 | input >> 6;
						break;
					case 5:
						result = lastShort << 10 | input >> 4;
						break;
					case 6:
						result = lastShort << 12 | input >> 2;
						break;
					case 7:
						result = lastShort << 14 | input;
						break;
				}

				shift++;
				if (shift > 7)
				{
					shift = 0;
				}

				lastShort = input;

				if (result != -1)
				{
					result = result & 0xFFFF;
				}

				return result;
			}
			else
			{
				return input;
			}
		}

		public virtual int flush()
		{
			if (byteShifted)
			{
				return getShort(0);
			}
			else
			{
				return -1;
			}
		}
	}

}