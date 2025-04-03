using System.Text;
using System.IO;
using System;

namespace PalmBibleExport
{


	public class Tokenizer
	{
		protected internal bool breakUnicode = true;
		protected internal bool breakNumber = false;
		protected internal bool spaceToken = false;
		// RWS 2004.12.14 added command-line option -kp
		protected internal bool keepPunctuation = false;

		protected internal int line = 1;

		 BinaryReader inputReader;

		protected internal string lastString1;
		protected internal string lastString2;

		protected internal Token ungetToken_Renamed;
		protected internal bool unget = false;

		// RWS 2004.12.14 keep default non-keepPunctuation functionality
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Tokenizer(java.io.InputStream inputStream, String enc) throws Exception
		public Tokenizer(MemoryStream inputStream, string enc) : this(inputStream, enc, false)
		{
		}

		// RWS 2004.12.14 added keepPunctuation parm
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Tokenizer(java.io.InputStream inputStream, String enc, boolean keepPunctuation) throws Exception
        public Tokenizer(MemoryStream inputStream, string enc, bool keepPunctuation)
		{
			this.keepPunctuation = keepPunctuation;
			try
			{
				inputReader = new BinaryReader(inputStream, Encoding.GetEncoding(enc));
			}
			catch (Exception e)
			{
				throwUnknownEncoding(enc);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws Exception
		public virtual void close()
		{
            inputReader.Close();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Token myGetToken() throws Exception
		public virtual Token myGetToken()
		{
			// RWS: Used by Verse class to tokenize versetext - altered below to not tokenize punctuation.
			// Tends to give a small hit in final pdb size because the word list now includes punctuated variations,
			// but the end result is a much more readable text without extraneous spaces around punctuation.

			bool process = true;
			bool lastNumber = false;

			if (unget)
			{
				unget = false;
				return ungetToken_Renamed;
			}
			else
			{
				int ch = 0;
				StringBuilder buffer = new StringBuilder();

				while (process && (ch = inputReader.Read()) != -1)
				{
					switch (ch)
					{
						case '<':
							if (buffer.Length != 0)
							{
								generateToken(buffer, ch);
								process = false;
							}
							else
							{
								Tag tag = new Tag(this);
								generateToken(tag);
								process = false;
							}
							break;
						case '0':
					case '1':
				case '2':
			case '3':
		case '4':
						case '5':
					case '6':
				case '7':
			case '8':
		case '9':
							// RWS 2004.12.14 added keepPunctuation condition
							if (!breakNumber && (keepPunctuation || lastNumber || buffer.Length == 0))
							{
									buffer.Append((char) ch);
								lastNumber = true;
							}
							else
							{
								generateToken(buffer, ch);
								process = false;
							}
							break;

						// RWS 2004.12.14 don't tokenize punctuation if command-line -kp specified
						case '~':
					case '@':
				case '#':
			case '$':
		case '%':
						case '^':
					case '*':
				case '_':
			case '+':
		case '/':
							if (keepPunctuation)
							{
								lastNumber = false; // RWS 2004.12.14 set lastNumber false for anything other than 0-9
								buffer.Append((char) ch);
							}
							else
							{
								generateToken(buffer, ch);
								process = false;
							}
							break;

						// RWS 2004.12.14 Bible+ handles spacing around these as a special case
						case '.':
					case ',':
				case '?':
			case '!':
						case ';':
					case ':':
				case '>':
						case '(':
					case ')':
						case '[':
					case ']':
						case '{':
					case '}':
								generateToken(buffer, ch);
							process = false;
							break;
						case '\n':
							line++;
						goto case '\r';
						case '\r':
						case ' ':
							if (spaceToken)
							{
								generateToken(buffer, ch);
								process = false;
							}
							else if (buffer.Length != 0)
							{
								generateToken(buffer, -1);
								process = false;
							}
							break;
						case '\\':
                            ch = inputReader.Read();
							if (ch == 'n')
							{
								ch = '\n';
							}
							else if (ch == 'C')
							{
								ch = (char)14;
							}
							goto default;
						default:
							// RWS 2004.12.14 added keepPunctuation condition (prevents splitting of #'s from letters also)
							if (lastNumber && !keepPunctuation)
							{
									generateToken(buffer, ch);
								process = false;
							}
							else if (ch >= 1 || ch == '\n')
							{
								if (!breakUnicode || ch < 256)
								{
									lastNumber = false; // RWS 2004.12.14 set lastNumber false for anything other than 0-9
										buffer.Append((char) ch);
								}
								else
								{
									generateToken(buffer, ch);
								process = false;
								}
							}
						break;
					}
				}
			}

			if (process)
			{
				return null;
			}
			else
			{
				unget = false;
				return ungetToken_Renamed;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Token getToken() throws Exception
		public virtual Token Token
		{
			get
			{
				// RWS: This is used to tokenize everything but verse text (i.e. - tags)
				// "verse" text includes the contents of VERSE, BOOKTEXT, CHAPTEXT, DESCTEXT, and VERSTEXT tags
    
				bool process = true;
				bool lastNumber = false;
    
				if (unget)
				{
					unget = false;
					return ungetToken_Renamed;
				}
				else
				{
					int ch = 0;
					StringBuilder buffer = new StringBuilder();
    
					while (process && (ch = inputReader.Read()) != -1)
					{
						switch (ch)
						{
							case '<':
								if (buffer.Length != 0)
								{
									generateToken(buffer, ch);
									process = false;
								}
								else
								{
									Tag tag = new Tag(this);
									generateToken(tag);
									process = false;
								}
								break;
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								if (!breakNumber && (lastNumber || buffer.Length == 0))
								{
									buffer.Append((char) ch);
									lastNumber = true;
								}
								else
								{
									generateToken(buffer, ch);
									process = false;
								}
								break;
							case '?':
						case '!':
					case ':':
				case ';':
							case '(':
						case ')':
					case '{':
				case '}':
							case '[':
						case ']':
					case '.':
				case ',':
							case '+': // case '-':
						case '*':
					case '=':
							case '%':
						case '$':
					case '@':
				case '#':
							case '^':
						case '_':
					case '>':
				case '"':
							case '/': // case '`':
						case '~':
								generateToken(buffer, ch);
								process = false;
								break;
							case '\n':
								line++;
							goto case '\r';
							case '\r':
							case ' ':
								if (spaceToken)
								{
									generateToken(buffer, ch);
									process = false;
								}
								else if (buffer.Length != 0)
								{
									generateToken(buffer, -1);
									process = false;
								}
								break;
							case '\\':
                                ch = inputReader.Read();
								if (ch == 'n')
								{
									ch = '\n';
								}
								else if (ch == 'C')
								{
									ch = (char)14;
								}
								goto default;
							default:
								if (lastNumber)
								{
									generateToken(buffer, ch);
									process = false;
								}
								else if (ch >= 1 || ch == '\n')
								{
									if (!breakUnicode || ch < 256)
									{
										buffer.Append((char) ch);
									}
									else
									{
										generateToken(buffer, ch);
										process = false;
									}
								}
							break;
						}
					}
				}
    
				if (process)
				{
					return null;
				}
				else
				{
					unget = false;
					return ungetToken_Renamed;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ungetToken() throws Exception
		public virtual void ungetToken()
		{
			if (unget)
			{
				throwUngetLimitExceeded();
			}
			else
			{
				unget = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBreakUnicode(boolean breakUnicode) throws Exception
		public virtual bool BreakUnicode
		{
			set
			{
				this.breakUnicode = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setBreakNumber(boolean breakNumber) throws Exception
		public virtual bool BreakNumber
		{
			set
			{
				this.breakNumber = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSpaceToken(boolean spaceToken) throws Exception
		public virtual bool SpaceToken
		{
			set
			{
				this.spaceToken = value;
			}
			get
			{
				return spaceToken;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getSpaceToken() throws Exception

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void generateToken(StringBuffer buffer, int ch) throws Exception
		protected internal virtual void generateToken(StringBuilder buffer, int ch)
		{
			if (buffer.Length == 0)
			{
				buffer.Append((char) ch);
			}
			else if (ch != -1)
			{
                inputReader.BaseStream.Seek(-1, SeekOrigin.Current);
			}

			lastString2 = lastString1;
			if (ungetToken_Renamed != null)
			{
				lastString1 = ungetToken_Renamed.ToString();
			}

			ungetToken_Renamed = new Token(buffer.ToString(), lastString1, lastString2, line);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void generateToken(Tag tag) throws Exception
		protected internal virtual void generateToken(Tag tag)
		{
			ungetToken_Renamed = new Token(tag, lastString1, lastString2, line);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void throwUngetLimitExceeded() throws ParserException
		protected internal virtual void throwUngetLimitExceeded()
		{
			throw new ParserException(ungetToken_Renamed.getErrorMessage("Ungetlimited Exceeded"));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void throwUnknownEncoding(String enc) throws ParserException
		protected internal virtual void throwUnknownEncoding(string enc)
		{
			throw new ParserException("Unknown Encoding : " + enc);
		}
	}

}