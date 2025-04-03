using System;
using System.Collections;
using System.Text;

namespace PalmBibleExport
{


	public abstract class Parser
	{

		protected internal string tagName = "UNKNOWN";
		protected internal ArrayList childs = new ArrayList();
		protected internal Tag openTag = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Parser() throws Exception
		public Parser() : base()
		{
		}

		protected internal virtual int TotalChild
		{
			get
			{
				return childs.Count;
			}
		}

		protected internal virtual object getChild(int index)
		{
			return childs[index];
		}

		protected internal virtual void addChild(object @object)
		{
			childs.Add(@object);
		}

		public virtual int getIntAttrib(string attrib)
		{
			int value = 0;

			string valueString = openTag.getAttrib(attrib);

			try
			{
				value = Convert.ToInt32(valueString);
	// RWS: don't stackTrace for a simple conversion exception
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			return value;
		}

		public virtual string getStringAttrib(string attrib)
		{
			return (string) openTag.getAttrib(attrib);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void checkTag(Tokenizer tokenizer) throws Exception
		protected internal virtual void checkTag(Tokenizer tokenizer)
		{
			tagName = TagName;
			Token token = tokenizer.Token;
			if (!isOpenTag(token))
			{
				throwUnexpectedToken(token);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void parse(Tokenizer tokenizer, WordRecorder wordRecorder) throws Exception
		protected internal virtual void parse(Tokenizer tokenizer, WordRecorder wordRecorder)
		{
			// RWS 2004.12.14 changed both tokenizer.getToken() calls to tokenizer.myGetToken()
			// the getToken() wasn't handling the beginning of "<VERSE>XY...</VERSE>" correctly
			// where X was a " and Y was a normal character or X was a normal character and Y
			// was a tokenized character.  getToken() always separates X and Y into two tokens,
			// whereas myGetToken does not tokenize ", nor does it tokenize numbers and other
			// specific punctuation characters when -kp is specified on the command-line.
			Token token = tokenizer.myGetToken();
			while (!isCloseTag(token))
			{
				tokenizer.ungetToken();
				childs.Add(parseChild(tokenizer, wordRecorder));
				token = tokenizer.myGetToken();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract Object parseChild(Tokenizer tokenizer, WordRecorder wordRecorder) throws Exception;
		protected internal abstract object parseChild(Tokenizer tokenizer, WordRecorder wordRecorder);
		protected internal abstract string TagName {get;}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean isOpenTag(Token token) throws Exception
		protected internal virtual bool isOpenTag(Token token)
		{
			if (token == null)
			{
				throwUnexpectedEnd(token);
			}

			if (token.isTag)
			{
				Tag tag = token.Tag;

				if (tag.OpenTag && tag.Name.Equals(tagName))
				{
					openTag = tag;
					return true;
				}
			}

			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean isOpenTag(Token token, String tagName) throws Exception
		protected internal virtual bool isOpenTag(Token token, string tagName)
		{
			if (token == null)
			{
				throwUnexpectedEnd(token);
			}

			if (token.isTag)
			{
				Tag tag = token.Tag;

				if (tag.OpenTag && tag.Name.Equals(tagName))
				{
					openTag = tag;
					return true;
				}
			}

			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected boolean isCloseTag(Token token) throws Exception
		protected internal virtual bool isCloseTag(Token token)
		{
			if (token == null)
			{
				throwUnexpectedEnd(token);
			}

			if (token.isTag)
			{
				Tag tag = token.Tag;

				if (tag.CloseTag && tag.Name.Equals(tagName))
				{
					return true;
				}
			}
			return false;
		}

		public virtual string ToString()
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append(openTag.ToString() + "-begin\n\r");

			for (int i = 0; i < childs.Count;i++)
			{
				buffer.Append(childs[i].ToString());
			}

			buffer.Append("\n\r" + openTag.ToString() + "-end\n\r");

			return buffer.ToString();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void printMessage(String string) throws Exception
		protected internal static void printMessage(string @string)
		{
			Console.WriteLine(@string);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void printError(String string) throws Exception
		protected internal static void printError(string @string)
		{
            Console.WriteLine(@string);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void throwUnexpectedEnd(Token token) throws ParserException
		protected internal virtual void throwUnexpectedEnd(Token token)
		{
			throw new ParserException(token.getErrorMessage("Unexpected End of file"));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void throwUnexpectedToken(Token token) throws ParserException
		protected internal virtual void throwUnexpectedToken(Token token)
		{
			throw new ParserException(token.getErrorMessage("Unexpected token"));
		}
	}

}