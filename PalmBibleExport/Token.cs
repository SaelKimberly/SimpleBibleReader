namespace PalmBibleExport
{

	public class Token
	{
		protected internal Tag tag;
		protected internal string @string;

		protected internal string prevString1;
		protected internal string prevString2;
		protected internal int line;

		public Token(string @string, string prevString1, string prevString2, int line) : this(prevString1, prevString2, line)
		{
			this.@string = @string;
			this.tag = null;
		}

		public Token(Tag tag, string prevString1, string prevString2, int line) : this(prevString1, prevString2, line)
		{
			this.tag = tag;
			this.@string = null;
		}

		protected internal Token(string prevString1, string prevString2, int line)
		{
			this.prevString1 = prevString1;
			this.prevString2 = prevString2;
			this.line = line;
		}

		public virtual bool isTag
		{
			get
			{
				return tag != null;
			}
		}

		public virtual Tag Tag
		{
			get
			{
				return tag;
			}
		}

		public virtual int Line
		{
			get
			{
				return line;
			}
		}

		public virtual string ToString()
		{
			if (tag == null)
			{
				return @string;
			}
			else
			{
				return tag.ToString();
			}
		}

		public virtual string getErrorMessage(string message)
		{
			if (isTag)
			{
				return message + " : " + tag.ToString() + " : ... at line " + line;
			}
			else
			{
				return message + " : " + @string + " : ... " + prevString2 + " " + prevString1 + @string + " ... at line " + line;
			}
		}
	}

}