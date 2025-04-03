namespace PalmBiblePlus
{

	public class MyStringTokenizer
	{

		private string[] tokens;
		private int current;

		public virtual int countTokens()
		{
			return tokens.Length;
		}

		public virtual bool hasMoreTokens()
		{
			return current < tokens.Length;
		}

		public virtual string nextToken()
		{
			return tokens[current++];
		}

		public virtual string[] Tokens
		{
			get
			{
				return tokens;
			}
		}

		public MyStringTokenizer(string str, string delim)
		{
			if (str == null || str.Length == 0)
			{
				return;
			}

			//compute necessary space
			int n = 0;
			int i = 0;
			while (i < str.Length)
			{
				//skip delim
				while (i < str.Length)
				{
					if (delim.IndexOf(str[i]) < 0)
					{
						break;
					}
					i++;
				}
				if (i >= str.Length)
				{
					break;
				}
				//read token until delim
				while (i < str.Length)
				{
					if (delim.IndexOf(str[i]) >= 0)
					{
						break;
					}
					i++;
				}
				n++;
			}
			tokens = new string[n];
			i = 0;
			n = 0;
			while (i < str.Length)
			{
				//skip delim
				while (i < str.Length)
				{
					if (delim.IndexOf(str[i]) < 0)
					{
						break;
					}
					i++;
				}
				if (i >= str.Length)
				{
					break;
				}
				//read token until delim
				int start = i;
				while (i < str.Length)
				{
					if (delim.IndexOf(str[i]) >= 0)
					{
						break;
					}
					i++;
				}
				tokens[n++] = str.Substring(start, i - start);
			}

		}

	}
}