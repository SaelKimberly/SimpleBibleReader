using System.Collections;
using System.Text;

namespace PalmBibleExport
{


	public class Tag
	{
		public const string OPEN_TAG = "<";
		public const string SLASH_TAG = "/";
		public const string CLOSE_TAG = ">";
		public const string EQUALS_TAG = "=";
		public static readonly string QUOTES_TAG = "\"";

		protected internal bool isOpenTag;
		protected internal string name;
		protected internal IDictionary attrib = new Hashtable();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Tag(Tokenizer tokenizer) throws Exception
		public Tag(Tokenizer tokenizer)
		{
			bool spaceToken = tokenizer.SpaceToken;
			tokenizer.SpaceToken = false;

			Token token = tokenizer.Token;
			name = token.ToString();

			if (name.Equals(SLASH_TAG))
			{
				token = tokenizer.Token;
				name = token.ToString();
				isOpenTag = false;
			}
			else
			{
				isOpenTag = true;
			}

			string attribName = null;
			string attribValue = null;

			token = tokenizer.Token;
			attribName = token.ToString();

			while (!attribName.Equals(CLOSE_TAG))
			{
				token = tokenizer.Token;
				attribValue = token.ToString();

				if (attribValue.Equals(EQUALS_TAG))
				{
					token = tokenizer.Token;
					attribValue = token.ToString();

					if (attribValue.Equals(QUOTES_TAG))
					{
						StringBuilder buffer = new StringBuilder();

						tokenizer.SpaceToken = true;

						token = tokenizer.Token;
						attribValue = token.ToString();

						while (!attribValue.Equals(QUOTES_TAG))
						{
							buffer.Append(attribValue);

							token = tokenizer.Token;
							attribValue = token.ToString();
						}
						attribValue = buffer.ToString();

						tokenizer.SpaceToken = false;
					}
				}
				attrib[attribName.ToUpper()] = attribValue;

				token = tokenizer.Token;
				attribName = token.ToString();
			}

			tokenizer.SpaceToken = spaceToken;
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual bool OpenTag
		{
			get
			{
				return isOpenTag;
			}
		}

		public virtual bool CloseTag
		{
			get
			{
				return !isOpenTag;
			}
		}

		public virtual string getAttrib(string name)
		{
			return (string) attrib[name.ToUpper()];
		}

		public virtual string ToString()
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append(OPEN_TAG);
			if (CloseTag)
			{
				buffer.Append(SLASH_TAG);
			}
			buffer.Append(name);

			//IDictionary.KeyCollection keySet = attrib.Keys;
            IEnumerator keys = attrib.Keys.GetEnumerator();

            while (keys.MoveNext())
			{
				buffer.Append(' ');
				string key = (string) keys.Current;
				buffer.Append(key);
				buffer.Append("=\"");
				string value = (string) attrib[key];
				buffer.Append(value);
				buffer.Append('\"');
			}

			buffer.Append(CLOSE_TAG);

			return buffer.ToString();
		}
	}

}