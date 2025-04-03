using System;

namespace PalmBibleExport
{

	public class BibleChecker
	{
		protected internal bool[] bookMarked;
		protected internal int[][] bookData;
		protected internal bool catholic = false;

		public BibleChecker()
		{
			bookMarked = new bool[80];
			bookData = new int[80][];

			prepareBookData();
		}

		public virtual void checkBook(Book book)
		{
			int bookNumber = book.BookNumber;
			int totalChapter = book.TotalChapter;

			if (bookNumber > 790)
			{
				return;
			}

			if (bookNumber == 760 || bookNumber == 770 || bookNumber == 780 || bookNumber == 790)
			{
				printApocryphaWarning(book, false);
			}
			else
			{
				if (bookNumber / 10 * 10 == bookNumber && bookNumber != 0)
				{

					if (bookNumber == 740 || bookNumber == 750)
					{
						printApocryphaWarning(book, true);
					}

					bookNumber /= 10;

					if (bookNumber == 19)
					{
						if (totalChapter == book190c.Length)
						{
							catholic = true;
							checkBook(book, book190c);
						}
						else if (totalChapter == bookData[bookNumber].Length)
						{
							checkBook(book, bookData[bookNumber]);
						}
						else
						{
							printChapterWarning(book, totalChapter, book190c.Length, bookData[bookNumber].Length);
						}
					}
					else if (bookNumber == 34)
					{
						if (totalChapter == book340c.Length)
						{
							catholic = true;
							checkBook(book, book340c);
						}
						else if (totalChapter == bookData[bookNumber].Length)
						{
							checkBook(book, bookData[bookNumber]);
						}
						else
						{
							printChapterWarning(book, totalChapter, book340c.Length, bookData[bookNumber].Length);
						}
					}
					else if (totalChapter == bookData[bookNumber].Length)
					{
						checkBook(book, bookData[bookNumber]);
					}
					else
					{
						printChapterWarning(book, totalChapter, bookData[bookNumber].Length);
					}

					if (!bookMarked[bookNumber])
					{
						bookMarked[bookNumber] = true;
					}
					else
					{
						printDuplicateWarning(book);
					}
				}
				else if (bookNumber == 175)
				{
					printApocryphaWarning(book, true);

					if (totalChapter == book175.Length)
					{
						catholic = true;
						checkBook(book, book175);
					}
					else
					{
						printChapterWarning(book, totalChapter, book175.Length);
					}
				}
				else
				{
					printBookWarning(book);
				}
			}
		}

		public virtual void checkBook(Book book, int[] bookList)
		{
			int totalChapter = book.TotalChapter;

			for (int i = 0; i < totalChapter; i++)
			{
				Chapter chapter = book.getChapter(i);
				int totalVerse = chapter.TotalVerse;
				if (totalVerse != bookList[i])
				{
					printVerseWarning(book, (i + 1), totalVerse, bookList[i]);
				}
			}
		}

		public virtual void printDuplicateWarning(Book book)
		{
			printWarning(book, "Duplicate Book Number : " + book.BookNumber);
		}

		public virtual void printApocryphaWarning(Book book, bool @checked)
		{
			if (@checked)
			{
				printWarning(book, "Book is Apocrypha");
			}
			else
			{
				printWarning(book, "Book is Apocrypha and Not Checked");
			}
		}

		public virtual void printBookWarning(Book book)
		{
			printWarning(book, "Unknown Book : " + book.BookNumber);
		}

		public virtual void printChapterWarning(Book book, int totalChapter, int shouldBe)
		{
			printChapterWarning(book, totalChapter, shouldBe, 0);
		}

		public virtual void printChapterWarning(Book book, int totalChapter, int shouldBe, int orShouldBe)
		{

			if (orShouldBe == 0)
			{
				printWarning(book, "Total chapters is not correct : Total chapters = " + totalChapter + " and Should be = " + shouldBe + " (Verse check skipped)");
			}
			else
			{
				printWarning(book, "Total chapters is not correct : Total chapters = " + totalChapter + " and Should be = " + shouldBe + " or " + orShouldBe + " (Verse check skipped)");
			}
		}

		public virtual void printVerseWarning(Book book, int chapter, int totalVerse, int shouldBe)
		{
			printWarning(book, "Total verses is not correct : For Chapter " + chapter + ", Total verses = " + totalVerse + " and Should be = " + shouldBe);
		}

		public virtual void printWarning(Book book, string message)
		{
			printWarning(message);
		}

		public virtual void printWarning(string message)
		{
			printMessage("Warning : " + message);
		}

		public virtual void printMessage(string message)
		{
			Console.WriteLine(message);
		}

		// Machine Generated for Prepare Book Data

		protected internal virtual void prepareBookData()
		{
			bookData[0] = null;
			bookData[1] = book10;
			bookData[2] = book20;
			bookData[3] = book30;
			bookData[4] = book40;
			bookData[5] = book50;
			bookData[6] = book60;
			bookData[7] = book70;
			bookData[8] = book80;
			bookData[9] = book90;
			bookData[10] = book100;
			bookData[11] = book110;
			bookData[12] = book120;
			bookData[13] = book130;
			bookData[14] = book140;
			bookData[15] = book150;
			bookData[16] = book160;
			bookData[17] = book170;
			bookData[18] = book180;
			bookData[19] = book190;
			bookData[20] = book200;
			bookData[21] = book210;
			bookData[22] = book220;
			bookData[23] = book230;
			bookData[24] = book240;
			bookData[25] = book250;
			bookData[26] = book260;
			bookData[27] = book270;
			bookData[28] = book280;
			bookData[29] = book290;
			bookData[30] = book300;
			bookData[31] = book310;
			bookData[32] = book320;
			bookData[33] = book330;
			bookData[34] = book340;
			bookData[35] = book350;
			bookData[36] = book360;
			bookData[37] = book370;
			bookData[38] = book380;
			bookData[39] = book390;
			bookData[40] = book400;
			bookData[41] = book410;
			bookData[42] = book420;
			bookData[43] = book430;
			bookData[44] = book440;
			bookData[45] = book450;
			bookData[46] = book460;
			bookData[47] = book470;
			bookData[48] = book480;
			bookData[49] = book490;
			bookData[50] = book500;
			bookData[51] = book510;
			bookData[52] = book520;
			bookData[53] = book530;
			bookData[54] = book540;
			bookData[55] = book550;
			bookData[56] = book560;
			bookData[57] = book570;
			bookData[58] = book580;
			bookData[59] = book590;
			bookData[60] = book600;
			bookData[61] = book610;
			bookData[62] = book620;
			bookData[63] = book630;
			bookData[64] = book640;
			bookData[65] = book650;
			bookData[66] = book660;
			bookData[67] = book670;
			bookData[68] = book680;
			bookData[69] = book690;
			bookData[70] = book700;
			bookData[71] = book710;
			bookData[72] = book720;
			bookData[73] = book730;
			bookData[74] = book740;
			bookData[75] = book750;
		}

		// Books Where Different from Catholic

		// Esther (Catholic)
		public int[] book190c = {22, 23, 15, 17, 14, 14, 10, 17, 32, 13, 12, 6, 18, 19, 19, 24};

		// Daniel (Catholic)
		public int[] book340c = {21, 49, 100, 34, 31, 28, 28, 27, 27, 21, 45, 13, 65, 42};

		// Esther
		public int[] book190 = {22, 23, 15, 17, 14, 14, 10, 17, 32, 3};

		// Daniel
		public int[] book340 = {21, 49, 30, 37, 31, 28, 28, 27, 27, 21, 45, 13};

		// Genesis
		public int[] book10 = {31, 25, 24, 26, 32, 22, 24, 22, 29, 32, 32, 20, 18, 24, 21, 16, 27, 33, 38, 18, 34, 24, 20, 67, 34, 35, 46, 22, 35, 43, 55, 32, 20, 31, 29, 43, 36, 30, 23, 23, 57, 38, 34, 34, 28, 34, 31, 22, 33, 26};

		// Exodus
		public int[] book20 = {22, 25, 22, 31, 23, 30, 25, 32, 35, 29, 10, 51, 22, 31, 27, 36, 16, 27, 25, 26, 36, 31, 33, 18, 40, 37, 21, 43, 46, 38, 18, 35, 23, 35, 35, 38, 29, 31, 43, 38};

		// Leviticus
		public int[] book30 = {17, 16, 17, 35, 19, 30, 38, 36, 24, 20, 47, 8, 59, 57, 33, 34, 16, 30, 37, 27, 24, 33, 44, 23, 55, 46, 34};

		// Numbers
		public int[] book40 = {54, 34, 51, 49, 31, 27, 89, 26, 23, 36, 35, 16, 33, 45, 41, 50, 13, 32, 22, 29, 35, 41, 30, 25, 18, 65, 23, 31, 40, 16, 54, 42, 56, 29, 34, 13};

		// Deuteronomy
		public int[] book50 = {46, 37, 29, 49, 33, 25, 26, 20, 29, 22, 32, 32, 18, 29, 23, 22, 20, 22, 21, 20, 23, 30, 25, 22, 19, 19, 26, 68, 29, 20, 30, 52, 29, 12};

		// Joshua
		public int[] book60 = {18, 24, 17, 24, 15, 27, 26, 35, 27, 43, 23, 24, 33, 15, 63, 10, 18, 28, 51, 9, 45, 34, 16, 33};

		// Judges
		public int[] book70 = {36, 23, 31, 24, 31, 40, 25, 35, 57, 18, 40, 15, 25, 20, 20, 31, 13, 31, 30, 48, 25};

		// Ruth
		public int[] book80 = {22, 23, 18, 22};

		// 1 Samuel
		public int[] book90 = {28, 36, 21, 22, 12, 21, 17, 22, 27, 27, 15, 25, 23, 52, 35, 23, 58, 30, 24, 42, 15, 23, 29, 22, 44, 25, 12, 25, 11, 31, 13};

		// 2 Samuel
		public int[] book100 = {27, 32, 39, 12, 25, 23, 29, 18, 13, 19, 27, 31, 39, 33, 37, 23, 29, 33, 43, 26, 22, 51, 39, 25};

		// 1 Kings
		public int[] book110 = {53, 46, 28, 34, 18, 38, 51, 66, 28, 29, 43, 33, 34, 31, 34, 34, 24, 46, 21, 43, 29, 53};

		// 2 Kings
		public int[] book120 = {18, 25, 27, 44, 27, 33, 20, 29, 37, 36, 21, 21, 25, 29, 38, 20, 41, 37, 37, 21, 26, 20, 37, 20, 30};

		// 1 Chronicles
		public int[] book130 = {54, 55, 24, 43, 26, 81, 40, 40, 44, 14, 47, 40, 14, 17, 29, 43, 27, 17, 19, 8, 30, 19, 32, 31, 31, 32, 34, 21, 30};

		// 2 Chronicles
		public int[] book140 = {17, 18, 17, 22, 14, 42, 22, 18, 31, 19, 23, 16, 22, 15, 19, 14, 19, 34, 11, 37, 20, 12, 21, 27, 28, 23, 9, 27, 36, 27, 21, 33, 25, 33, 27, 23};

		// Ezra
		public int[] book150 = {11, 70, 13, 24, 17, 22, 28, 36, 15, 44};

		// Nehemiah
		public int[] book160 = {11, 20, 32, 23, 19, 19, 73, 18, 38, 39, 36, 47, 31};

		// Job
		public int[] book220 = {22, 13, 26, 21, 27, 30, 21, 22, 35, 22, 20, 25, 28, 22, 35, 22, 16, 21, 29, 29, 34, 30, 17, 25, 6, 14, 23, 28, 25, 31, 40, 22, 33, 37, 16, 33, 24, 41, 30, 24, 34, 17};

		// Psalms
		public int[] book230 = {6, 12, 8, 8, 12, 10, 17, 9, 20, 18, 7, 8, 6, 7, 5, 11, 15, 50, 14, 9, 13, 31, 6, 10, 22, 12, 14, 9, 11, 12, 24, 11, 22, 22, 28, 12, 40, 22, 13, 17, 13, 11, 5, 26, 17, 11, 9, 14, 20, 23, 19, 9, 6, 7, 23, 13, 11, 11, 17, 12, 8, 12, 11, 10, 13, 20, 7, 35, 36, 5, 24, 20, 28, 23, 10, 12, 20, 72, 13, 19, 16, 8, 18, 12, 13, 17, 7, 18, 52, 17, 16, 15, 5, 23, 11, 13, 12, 9, 9, 5, 8, 28, 22, 35, 45, 48, 43, 13, 31, 7, 10, 10, 9, 8, 18, 19, 2, 29, 176, 7, 8, 9, 4, 8, 5, 6, 5, 6, 8, 8, 3, 18, 3, 3, 21, 26, 9, 8, 24, 13, 10, 7, 12, 15, 21, 10, 20, 14, 9, 6};

		// Proverbs
		public int[] book240 = {33, 22, 35, 27, 23, 35, 27, 36, 18, 32, 31, 28, 25, 35, 33, 33, 28, 24, 29, 30, 31, 29, 35, 34, 28, 28, 27, 28, 27, 33, 31};

		// Ecclesiastes
		public int[] book250 = {18, 26, 22, 16, 20, 12, 29, 17, 18, 20, 10, 14};

		// Song of Solomon
		public int[] book260 = {17, 17, 11, 16, 16, 13, 13, 14};

		// Isaiah
		public int[] book290 = {31, 22, 26, 6, 30, 13, 25, 22, 21, 34, 16, 6, 22, 32, 9, 14, 14, 7, 25, 6, 17, 25, 18, 23, 12, 21, 13, 29, 24, 33, 9, 20, 24, 17, 10, 22, 38, 22, 8, 31, 29, 25, 28, 28, 25, 13, 15, 22, 26, 11, 23, 15, 12, 17, 13, 12, 21, 14, 21, 22, 11, 12, 19, 12, 25, 24};

		// Jeremiah
		public int[] book300 = {19, 37, 25, 31, 31, 30, 34, 22, 26, 25, 23, 17, 27, 22, 21, 21, 27, 23, 15, 18, 14, 30, 40, 10, 38, 24, 22, 17, 32, 24, 40, 44, 26, 22, 19, 32, 21, 28, 18, 16, 18, 22, 13, 30, 5, 28, 7, 47, 39, 46, 64, 34};

		// Lamentations
		public int[] book310 = {22, 22, 66, 22, 22};

		// Ezekiel
		public int[] book330 = {28, 10, 27, 17, 17, 14, 27, 18, 11, 22, 25, 28, 23, 23, 8, 63, 24, 32, 14, 49, 32, 31, 49, 27, 17, 21, 36, 26, 21, 26, 18, 32, 33, 31, 15, 38, 28, 23, 29, 49, 26, 20, 27, 31, 25, 24, 23, 35};

		// Hosea
		public int[] book350 = {11, 23, 5, 19, 15, 11, 16, 14, 17, 15, 12, 14, 16, 9};

		// Joel
		public int[] book360 = {20, 32, 21};

		// Amos
		public int[] book370 = {15, 16, 15, 13, 27, 14, 17, 14, 15};

		// Obadiah
		public int[] book380 = {21};

		// Jonah
		public int[] book390 = {17, 10, 10, 11};

		// Micah
		public int[] book400 = {16, 13, 12, 13, 15, 16, 20};

		// Nahum
		public int[] book410 = {15, 13, 19};

		// Habakkuk
		public int[] book420 = {17, 20, 19};

		// Zephaniah
		public int[] book430 = {18, 15, 20};

		// Haggai
		public int[] book440 = {15, 23};

		// Zechariah
		public int[] book450 = {21, 13, 10, 14, 11, 15, 14, 23, 17, 12, 17, 14, 9, 21};

		// Malachi
		public int[] book460 = {14, 17, 18, 6};

		// Matthew
		public int[] book470 = {25, 23, 17, 25, 48, 34, 29, 34, 38, 42, 30, 50, 58, 36, 39, 28, 27, 35, 30, 34, 46, 46, 39, 51, 46, 75, 66, 20};

		// Mark
		public int[] book480 = {45, 28, 35, 41, 43, 56, 37, 38, 50, 52, 33, 44, 37, 72, 47, 20};

		// Luke
		public int[] book490 = {80, 52, 38, 44, 39, 49, 50, 56, 62, 42, 54, 59, 35, 35, 32, 31, 37, 43, 48, 47, 38, 71, 56, 53};

		// John
		public int[] book500 = {51, 25, 36, 54, 47, 71, 53, 59, 41, 42, 57, 50, 38, 31, 27, 33, 26, 40, 42, 31, 25};

		// Acts
		public int[] book510 = {26, 47, 26, 37, 42, 15, 60, 40, 43, 48, 30, 25, 52, 28, 41, 40, 34, 28, 41, 38, 40, 30, 35, 27, 27, 32, 44, 31};

		// Romans
		public int[] book520 = {32, 29, 31, 25, 21, 23, 25, 39, 33, 21, 36, 21, 14, 23, 33, 27};

		// 1 Corint[]hians
		public int[] book530 = {31, 16, 23, 21, 13, 20, 40, 13, 27, 33, 34, 31, 13, 40, 58, 24};

		// 2 Corint[]hians
		public int[] book540 = {24, 17, 18, 18, 21, 18, 16, 24, 15, 18, 33, 21, 14};

		// Galatians
		public int[] book550 = {24, 21, 29, 31, 26, 18};

		// Ephesians
		public int[] book560 = {23, 22, 21, 32, 33, 24};

		// Philippians
		public int[] book570 = {30, 30, 21, 23};

		// Colossians
		public int[] book580 = {29, 23, 25, 18};

		// 1 Thessalonians
		public int[] book590 = {10, 20, 13, 18, 28};

		// 2 Thessalonians
		public int[] book600 = {12, 17, 18};

		// 1 Timothy
		public int[] book610 = {20, 15, 16, 16, 25, 21};

		// 2 Timothy
		public int[] book620 = {18, 26, 17, 22};

		// Titus
		public int[] book630 = {16, 15, 15};

		// Philemon
		public int[] book640 = {25};

		// Hebrews
		public int[] book650 = {14, 18, 19, 16, 14, 20, 28, 13, 28, 39, 40, 29, 25};

		// James
		public int[] book660 = {27, 26, 18, 17, 20};

		// 1 Peter
		public int[] book670 = {25, 25, 22, 19, 14};

		// 2 Peter
		public int[] book680 = {21, 22, 18};

		// 1 John
		public int[] book690 = {10, 29, 24, 21, 21};

		// 2 John
		public int[] book700 = {13};

		// 3 John
		public int[] book710 = {14};

		// Jude
		public int[] book720 = {25};

		// Revelation
		public int[] book730 = {20, 29, 22, 11, 14, 17, 17, 13, 21, 11, 19, 17, 18, 20, 8, 21, 18, 24, 21, 15, 27, 21};

		// Books where Catholic Only

		// Tobit
		public int[] book170 = {22, 14, 17, 21, 22, 18, 17, 21, 6, 14, 18, 22, 18, 15};

		// Tobias
		public int[] book175 = {25, 23, 25, 23, 28, 22, 20, 24, 12, 13, 21, 22, 23, 17};

		// Judith
		public int[] book180 = {12, 18, 15, 16, 29, 21, 25, 34, 19, 20, 21, 20, 31, 18, 15, 31};

		// 1 Maccabees
		public int[] book200 = {67, 70, 60, 61, 68, 63, 50, 32, 73, 89, 74, 54, 54, 49, 41, 24};

		// 2 Maccabees
		public int[] book210 = {36, 33, 40, 50, 27, 31, 42, 36, 29, 38, 38, 46, 26, 46, 40};

		// Wisdom
		public int[] book270 = {16, 25, 19, 20, 24, 27, 30, 21, 19, 21, 27, 27, 19, 31, 19, 29, 20, 25, 20};

		// Sirach
		public int[] book280 = {40, 23, 34, 36, 18, 37, 40, 22, 25, 34, 36, 19, 32, 27, 22, 31, 31, 33, 28, 33, 31, 33, 38, 47, 36, 28, 33, 30, 34, 27, 42, 28, 33, 31, 26, 28, 34, 39, 41, 32, 28, 26, 37, 27, 31, 23, 31, 28, 19, 31, 38};

		// Baruch
		public int[] book320 = {22, 35, 38, 37, 9, 72};

		// Apocrypha

		// 1 Esdras
		public int[] book740 = {58, 30, 24, 63, 73, 34, 15, 96, 55};

		// 2 Esdras
		public int[] book750 = {40, 48, 36, 52, 56, 59, 70, 63, 47, 59, 46, 51, 58, 48, 63, 78};
	}

}