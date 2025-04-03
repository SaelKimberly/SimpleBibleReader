using System;
using System.Text;

namespace PalmBiblePlus
{


	public class Util
	{

		public static int binarySearch(int[] array, int el)
		{
			//return Arrays.binarySearch(array, el);
			return -1;
		}

		public static string escape(string s)
		{
			return Util.escape(s, '%', ":\r\n");
		}

		public static string unescape(string s)
		{
			return Util.unescape(s, '%');
		}

		public static StringBuilder addField(StringBuilder sb, int i)
		{
			return sb.Append(i).Append(":");
		}

		public static StringBuilder addField(StringBuilder sb, string s)
		{
            return sb.Append(Util.escape(s)).Append(":");
		}

		public static StringBuilder addLastField(StringBuilder sb, string s)
		{
			return sb.Append(Util.escape(s));
		}

		public static StringBuilder addLastField(StringBuilder sb, int i)
		{
			return sb.Append(i);
		}


		public static string unescape(string s, char prefix)
		{
			if (s == null)
			{
				return null;
			}
			StringBuilder res = new StringBuilder();
			int i = 0;
			while (i < s.Length)
			{
				char c = s[i];
				if (c == prefix)
				{
					if (i + 4 >= s.Length)
					{
						res.Append(c);
					}
					else
					{
						string vs = s.Substring(i + 1, i + 5 - (i + 1));
						try
						{
							int v = Convert.ToInt32(vs, 16);
							char cn = (char)v;
							res.Append(cn);
							i += 5;
							continue;
						}
						catch (Exception e)
						{
							res.Append(c);
						}
					}
				}
				else
				{
					res.Append(c);
				}
				i++;
			}
			return res.ToString();
		}

		public static string escape(string s, char prefix, string charsToReplace)
		{
			if (s == null || charsToReplace == null)
			{
				return null;
			}
			StringBuilder res = new StringBuilder();
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (c == prefix || charsToReplace.IndexOf(c) >= 0)
				{
					res.Append(prefix);
					int v = (int)c;
					string vs = v.ToString("X");
					if (vs.Length < 4)
					{
						res.Append("0000".Substring(vs.Length));
					}
					res.Append(vs);

				}
				else
				{
					res.Append(c);
				}
			}
			return res.ToString();
		}

	/*
		public static void dumpBytes(byte[] data) {
			for (int i = 0; i < data.length; i++) {
				if (i>0 && (i%16==0)) {
	//				System.out.println();
				}
				String s=  Integer.toHexString(data[i] & 0xff);
				if (s.length()==1) 
					s = "0" + s;
	//			System.out.print(s + " ");
			}		
			//System.out.println();
			}*/

		public static string readStringTrimZero(byte[] data, int offs, int length, string encoding)
		{
			try
			{
				int i = offs + length - 1;
				while (i >= offs)
				{
					if (data[i] != 0)
					{
						break;
					}
					i--;
				}
				int len = i - offs + 1;
				if (len < 0)
				{
					return "";
				}
                    return Encoding.GetEncoding(encoding).GetString(data, offs, len);
			}
			catch (Exception e)
			{
				return "";
			}
		}

		internal static int[] hebrewtab;
		internal static int[] greektab;

		public static void setTables(int[] _hebrewtab, int[] _greektab)
		{
			hebrewtab = _hebrewtab;
			greektab = _greektab;
		}


		public static string readStringHebrew(byte[] data, int offs, int length)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = offs; i < offs + length; i++)
			{
				int c = data[i] & 0xff;
				sb.Append((char)(hebrewtab[c]));
			}
			return sb.ToString();
		}

		public static string readStringGreek(byte[] data, int offs, int length)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = offs; i < offs + length; i++)
			{
				int c = data[i] & 0xff;
				sb.Append((char)(greektab[c]));
			}
			return sb.ToString();
		}


		public static string readStringISO8859_1(byte[] data, int offs, int length)
		{
			try
			{
				//return new string(data, offs, length, "ISO-8859-1");
                return Encoding.GetEncoding("ISO-8859-1").GetString(data, offs, length);
			}
			catch (Exception e)
			{

				return "";
			}
		}

		public static string readString(byte[] data, int offs, int length, string encoding)
		{
			try
			{
				//return new string(data, offs, length, encoding);
                return Encoding.GetEncoding(encoding).GetString(data, offs, length);
			}
			catch (Exception e)
			{
				return readStringISO8859_1(data, offs, length);
			}
		}


		public static int readShort(byte[] data, int offs)
		{
            return (ushort) (((data[offs] & 0xff) << 8) | (data[offs + 1] & 0xff));
		}

		public static int readInt(byte[] data, int offs)
		{
			return (((data[offs] & 0xff) << 24) | ((data[offs + 1] & 0xff) << 16) | ((data[offs + 2] & 0xff) << 8) | (data[offs + 3] & 0xff));
		}

	}

}