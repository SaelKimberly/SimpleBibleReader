using System;
using System.Text;

namespace PalmBiblePlus
{


	/// <summary>
	/// Fixed size bit vector
	/// </summary>

	public class BitVector
	{

		private int[] storage;
		private int size;

		private int[] start_pos = new int[256];

		internal int[] wordnums_array;
		internal int[] matchbits_array;

		internal int wcount;

		internal virtual void setStartPos()
		{
			for (int i = 0; i < start_pos.Length; i++)
			{
				start_pos[i] = 0;
			}

			//System.out.println("wcount = " + wcount);

			//int min = wordnums_array[0];

			for (int i = wcount - 1; i >= 0; i--)
			{
				int v = wordnums_array[i];
				int pos = (v & 0xff);
				//System.out.println(v + " pos " + pos + " in " + i);
				start_pos[pos] = i;
			}

			//for (int i = 0; i < start_pos.length; i++) {
			//	System.out.println("start pos " + start_pos[i]);
			//}
		}

		public BitVector(int _size)
		{
			size = _size;
			int sz = (size+31) / 32;
			storage = new int[sz];
			allocated = 20;
			wordnums_array = new int[allocated];
			matchbits_array = new int[allocated];
			wcount = 0;
		}

		internal virtual void reset()
		{
			wcount = 0;
			int sz = (size+31) / 32;
			for (int i = 0 ; i < sz; i++)
			{
				storage[i] = 0;
			}
		}


		internal static int[] bitsettable;

		private int allocated;


		internal virtual int getMatchBitsForWord(int n)
		{

			int start = start_pos[n & 0xff];
			//int start = 0;

			for (int i = start; i < wcount; i++)
			{
				if (wordnums_array[i] == n)
				{
					return matchbits_array[i];
				}
				// if (i > start ) {
				// 	System.out.println("miss  n = " + n +  " start = " + start + " wordnums_array[start] " + wordnums_array[start]);
				// 	System.exit(0);
				// }
			}
			return 0; //0 = nothing match
		}


		internal virtual int storeMatchInfo(int wordnum, int matches)
		{
			if (wcount >= allocated)
			{
				int[] wordnums_array_tmp = new int[2 * allocated];
				Array.Copy(wordnums_array, 0, wordnums_array_tmp, 0, wordnums_array.Length);
				wordnums_array = wordnums_array_tmp;
				int[] matchbits_array_tmp = new int[2 * allocated];
				Array.Copy(matchbits_array, 0, matchbits_array_tmp, 0, matchbits_array.Length);
				matchbits_array = matchbits_array_tmp;
				allocated = 2 * allocated;
			}

			wordnums_array[wcount] = wordnum;
			matchbits_array[wcount] = matches;
			wcount++;
			return wcount - 1;
		}


		private void fillBitSet()
		{
			bitsettable = new int[256];
			bitsettable[0] = 0;
			for (int i = 0; i < 256; i++)
			{
				bitsettable[i] = (i & 1) + bitsettable[i / 2];
			}

		}

		public virtual int countBits(long l)
		{
			if (bitsettable == null)
			{
				fillBitSet();
			}
			int n1 = (int)(l & 0xffffffff);
			int n2 = (int)((l >> 32) & 0xffffffff);
			int c1 = bitsettable[n1 & 0xff] + bitsettable[(n1 >> 8) & 0xff] + bitsettable[(n1 >> 16) & 0xff] + bitsettable[(n1 >> 24) & 0xff];
			int c2 = bitsettable[n2 & 0xff] + bitsettable[(n2 >> 8) & 0xff] + bitsettable[(n2 >> 16) & 0xff] + bitsettable[(n2 >> 24) & 0xff];
			return c1 + c2;
		}

		public virtual int countSet()
		{
			if (bitsettable == null)
			{
				fillBitSet();
			}
			int total = 0;
			for (int i = 0; i < storage.Length; i++)
			{
				int v = storage[i];
				int c = bitsettable[v & 0xff] + bitsettable[(v >> 8) & 0xff] + bitsettable[(v >> 16) & 0xff] + bitsettable[(v >> 24) & 0xff];
				total += c;
			}
			return total;
		}

		public virtual int get(int pos)
		{
			int int_pos = pos / 32;
			if (int_pos >= storage.Length)
			{
				return 0;
			}
			long l = storage[int_pos] & 0xffffffff;
			if (l == 0)
			{
				return 0;
			}

			int bit_pos = pos % 32;

			l &= (1 << bit_pos);
			return l != 0?1:0;
		}

		public virtual void set(int pos)
		{
			int int_pos = pos / 32;
			int bit_pos = pos % 32;
			long l = storage[int_pos] & 0xffffffff;
			l |= (1 << bit_pos);
			storage[int_pos] = (int)l;
		}

		// public void clear(int pos) {
		// 	int int_pos = pos/32;
		// 	int bit_pos = pos % 32;
		// 	long l = storage[int_pos] & 0xffffffff;
		// 	l &= ~(1<<bit_pos);
		// 	storage[int_pos] = (int)l;
		// }

		public virtual string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < size; i++)
			{
				sb.Append(get(i) == 1?"1":"0");
			}
			sb.Append("Count:").Append(countSet());
			sb.AppendLine();
			return sb.ToString();
		}

		public virtual int[] listSet()
		{
			int[] res = new int[countSet()];
			int m = 0;
			for (int i = 0; i < size; i++)
			{
				if (get(i) == 1)
				{
					res[m++] = i;
				}
			}
			return res;
		}


	}

}