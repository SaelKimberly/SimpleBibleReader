namespace Sword
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    using ComponentAce.Compression.Libs.zlib;

    public class BibleZtextWriter 
    {
        #region Fields
        public const int BOOKS_IN_BIBLE = 66;
        public const int BOOKS_IN_NT = 27;
        public const int BOOKS_IN_OT = 39;
        public const int CHAPTERS_IN_BIBLE = 1189;
        public const int CHAPTERS_IN_NT = 260;
        public const int CHAPTERS_IN_OT = 929;
        public const string DIR_CONF = "mods.d";
        public const string DIR_DATA = "modules";
        public const string EXTENSION_CONF = ".conf";
        public const string EXTENSION_DATA = ".dat";
        public const string EXTENSION_INDEX = ".idx";
        public const string EXTENSION_VSS = ".vss";
        public const string FILE_NT = "nt";
        public const string FILE_OT = "ot";
        public static readonly short[] CHAPTERS_IN_BOOK = { 50, 40, 27, 36, 34, 24, 21, 4, 31, 24, 22, 25, 29, 36, 10, 13, 10, 42, 150, 31, 12, 8, 66, 52, 5, 48, 12, 14, 3, 9, 1, 4, 7, 3, 3, 3, 2, 14, 4, 28, 16, 24, 21, 28, 16, 16, 13, 6, 6, 4, 4, 5, 3, 6, 4, 3, 1, 13, 5, 5, 3, 5, 1, 1, 1, 22 };
        public static readonly Dictionary<string, int> osisBibeNamesToAbsoluteChapterNum = new Dictionary<string, int>
        {
            {"gen",0 },
            {"exod",50 },
            {"lev",90 },
            {"num",117 },
            {"deut",153 },
            {"josh",187 },
            {"judg",211 },
            {"ruth",232 },
            {"1sam",236 },
            {"2sam",267 },
            {"1kgs",291 },
            {"2kgs",313 },
            {"1chr",338 },
            {"2chr",367 },
            {"ezra",403 },
            {"neh",413 },
            {"esth",426 },
            {"job",436 },
            {"ps",478 },
            {"prov",628 },
            {"eccl",659 },
            {"song",671 },
            {"isa",679 },
            {"jer",745 },
            {"lam",797 },
            {"ezek",802 },
            {"dan",850 },
            {"hos",862 },
            {"joel",876 },
            {"amos",879 },
            {"obad",888 },
            {"jonah",889 },
            {"mic",893 },
            {"nah",900 },
            {"hab",903 },
            {"zeph",906 },
            {"hag",909 },
            {"zech",911 },
            {"mal",925 },
            {"matt",929 },
            {"mark",957 },
            {"luke",973 },
            {"john",997 },
            {"acts",1018 },
            {"rom",1046 },
            {"1cor",1062 },
            {"2cor",1078 },
            {"gal",1091 },
            {"eph",1097 },
            {"phil",1103 },
            {"col",1107 },
            {"1thess",1111 },
            {"2thess",1116 },
            {"1tim",1119 },
            {"2tim",1125 },
            {"titus",1129 },
            {"phlm",1132 },
            {"heb",1133 },
            {"jas",1146 },
            {"1pet",1151 },
            {"2pet",1156 },
            {"1john",1159 },
            {"2john",1164 },
            {"3john",1165 },
            {"jude",1166 },
            {"rev",1167 },
        };
       
        public List<BookPos> bookPositions = new List<BookPos>();
        public List<ChapterPos> chapters = new List<ChapterPos>();
        public BibleZtextWriterSerialData serial = new BibleZtextWriterSerialData(false, "", "", 0, 0);
        internal const short VERSES_IN_BIBLE = 31102;
        internal static readonly short[] FIRST_CHAPTERNUM_IN_BOOK = { 0, 50, 90, 117, 153, 187, 211, 232, 236, 267, 291, 313, 338, 367, 403, 413, 426, 436, 478, 628, 659, 671, 679, 745, 797, 802, 850, 862, 876, 879, 888, 889, 893, 900, 903, 906, 909, 911, 925, 929, 957, 973, 997, 1018, 1046, 1062, 1078, 1091, 1097, 1103, 1107, 1111, 1116, 1119, 1125, 1129, 1132, 1133, 1146, 1151, 1156, 1159, 1164, 1165, 1166, 1167 };
        internal static readonly short[] VERSES_IN_BOOK = { 1533, 1213, 859, 1288, 959, 658, 618, 85, 810, 695, 816, 719, 942, 822, 280, 406, 167, 1070, 2461, 915, 222, 117, 1292, 1364, 154, 1273, 357, 197, 73, 146, 21, 48, 105, 47, 56, 53, 38, 211, 55, 1071, 678, 1151, 879, 1007, 433, 437, 257, 149, 155, 104, 95, 89, 47, 113, 83, 46, 25, 303, 108, 105, 61, 105, 13, 14, 25, 404 };
        internal static readonly short[][] VERSES_IN_CHAPTER = { new short[] { 31, 25, 24, 26, 32, 22, 24, 22, 29, 32, 32, 20, 18, 24, 21, 16, 27, 33, 38, 18, 34, 24, 20, 67, 34, 35, 46, 22, 35, 43, 55, 32, 20, 31, 29, 43, 36, 30, 23, 23, 57, 38, 34, 34, 28, 34, 31, 22, 33, 26 }, new short[] { 22, 25, 22, 31, 23, 30, 25, 32, 35, 29, 10, 51, 22, 31, 27, 36, 16, 27, 25, 26, 36, 31, 33, 18, 40, 37, 21, 43, 46, 38, 18, 35, 23, 35, 35, 38, 29, 31, 43, 38 }, new short[] { 17, 16, 17, 35, 19, 30, 38, 36, 24, 20, 47, 8, 59, 57, 33, 34, 16, 30, 37, 27, 24, 33, 44, 23, 55, 46, 34 }, new short[] { 54, 34, 51, 49, 31, 27, 89, 26, 23, 36, 35, 16, 33, 45, 41, 50, 13, 32, 22, 29, 35, 41, 30, 25, 18, 65, 23, 31, 40, 16, 54, 42, 56, 29, 34, 13 }, new short[] { 46, 37, 29, 49, 33, 25, 26, 20, 29, 22, 32, 32, 18, 29, 23, 22, 20, 22, 21, 20, 23, 30, 25, 22, 19, 19, 26, 68, 29, 20, 30, 52, 29, 12 }, new short[] { 18, 24, 17, 24, 15, 27, 26, 35, 27, 43, 23, 24, 33, 15, 63, 10, 18, 28, 51, 9, 45, 34, 16, 33 }, new short[] { 36, 23, 31, 24, 31, 40, 25, 35, 57, 18, 40, 15, 25, 20, 20, 31, 13, 31, 30, 48, 25 }, new short[] { 22, 23, 18, 22 }, new short[] { 28, 36, 21, 22, 12, 21, 17, 22, 27, 27, 15, 25, 23, 52, 35, 23, 58, 30, 24, 42, 15, 23, 29, 22, 44, 25, 12, 25, 11, 31, 13 }, new short[] { 27, 32, 39, 12, 25, 23, 29, 18, 13, 19, 27, 31, 39, 33, 37, 23, 29, 33, 43, 26, 22, 51, 39, 25 }, new short[] { 53, 46, 28, 34, 18, 38, 51, 66, 28, 29, 43, 33, 34, 31, 34, 34, 24, 46, 21, 43, 29, 53 }, new short[] { 18, 25, 27, 44, 27, 33, 20, 29, 37, 36, 21, 21, 25, 29, 38, 20, 41, 37, 37, 21, 26, 20, 37, 20, 30 }, new short[] { 54, 55, 24, 43, 26, 81, 40, 40, 44, 14, 47, 40, 14, 17, 29, 43, 27, 17, 19, 8, 30, 19, 32, 31, 31, 32, 34, 21, 30 }, new short[] { 17, 18, 17, 22, 14, 42, 22, 18, 31, 19, 23, 16, 22, 15, 19, 14, 19, 34, 11, 37, 20, 12, 21, 27, 28, 23, 9, 27, 36, 27, 21, 33, 25, 33, 27, 23 }, new short[] { 11, 70, 13, 24, 17, 22, 28, 36, 15, 44 }, new short[] { 11, 20, 32, 23, 19, 19, 73, 18, 38, 39, 36, 47, 31 }, new short[] { 22, 23, 15, 17, 14, 14, 10, 17, 32, 3 }, new short[] { 22, 13, 26, 21, 27, 30, 21, 22, 35, 22, 20, 25, 28, 22, 35, 22, 16, 21, 29, 29, 34, 30, 17, 25, 6, 14, 23, 28, 25, 31, 40, 22, 33, 37, 16, 33, 24, 41, 30, 24, 34, 17 }, new short[] { 6, 12, 8, 8, 12, 10, 17, 9, 20, 18, 7, 8, 6, 7, 5, 11, 15, 50, 14, 9, 13, 31, 6, 10, 22, 12, 14, 9, 11, 12, 24, 11, 22, 22, 28, 12, 40, 22, 13, 17, 13, 11, 5, 26, 17, 11, 9, 14, 20, 23, 19, 9, 6, 7, 23, 13, 11, 11, 17, 12, 8, 12, 11, 10, 13, 20, 7, 35, 36, 5, 24, 20, 28, 23, 10, 12, 20, 72, 13, 19, 16, 8, 18, 12, 13, 17, 7, 18, 52, 17, 16, 15, 5, 23, 11, 13, 12, 9, 9, 5, 8, 28, 22, 35, 45, 48, 43, 13, 31, 7, 10, 10, 9, 8, 18, 19, 2, 29, 176, 7, 8, 9, 4, 8, 5, 6, 5, 6, 8, 8, 3, 18, 3, 3, 21, 26, 9, 8, 24, 13, 10, 7, 12, 15, 21, 10, 20, 14, 9, 6 }, new short[] { 33, 22, 35, 27, 23, 35, 27, 36, 18, 32, 31, 28, 25, 35, 33, 33, 28, 24, 29, 30, 31, 29, 35, 34, 28, 28, 27, 28, 27, 33, 31 }, new short[] { 18, 26, 22, 16, 20, 12, 29, 17, 18, 20, 10, 14 }, new short[] { 17, 17, 11, 16, 16, 13, 13, 14 }, new short[] { 31, 22, 26, 6, 30, 13, 25, 22, 21, 34, 16, 6, 22, 32, 9, 14, 14, 7, 25, 6, 17, 25, 18, 23, 12, 21, 13, 29, 24, 33, 9, 20, 24, 17, 10, 22, 38, 22, 8, 31, 29, 25, 28, 28, 25, 13, 15, 22, 26, 11, 23, 15, 12, 17, 13, 12, 21, 14, 21, 22, 11, 12, 19, 12, 25, 24 }, new short[] { 19, 37, 25, 31, 31, 30, 34, 22, 26, 25, 23, 17, 27, 22, 21, 21, 27, 23, 15, 18, 14, 30, 40, 10, 38, 24, 22, 17, 32, 24, 40, 44, 26, 22, 19, 32, 21, 28, 18, 16, 18, 22, 13, 30, 5, 28, 7, 47, 39, 46, 64, 34 }, new short[] { 22, 22, 66, 22, 22 }, new short[] { 28, 10, 27, 17, 17, 14, 27, 18, 11, 22, 25, 28, 23, 23, 8, 63, 24, 32, 14, 49, 32, 31, 49, 27, 17, 21, 36, 26, 21, 26, 18, 32, 33, 31, 15, 38, 28, 23, 29, 49, 26, 20, 27, 31, 25, 24, 23, 35 }, new short[] { 21, 49, 30, 37, 31, 28, 28, 27, 27, 21, 45, 13 }, new short[] { 11, 23, 5, 19, 15, 11, 16, 14, 17, 15, 12, 14, 16, 9 }, new short[] { 20, 32, 21 }, new short[] { 15, 16, 15, 13, 27, 14, 17, 14, 15 }, new short[] { 21 }, new short[] { 17, 10, 10, 11 }, new short[] { 16, 13, 12, 13, 15, 16, 20 }, new short[] { 15, 13, 19 }, new short[] { 17, 20, 19 }, new short[] { 18, 15, 20 }, new short[] { 15, 23 }, new short[] { 21, 13, 10, 14, 11, 15, 14, 23, 17, 12, 17, 14, 9, 21 }, new short[] { 14, 17, 18, 6 }, new short[] { 25, 23, 17, 25, 48, 34, 29, 34, 38, 42, 30, 50, 58, 36, 39, 28, 27, 35, 30, 34, 46, 46, 39, 51, 46, 75, 66, 20 }, new short[] { 45, 28, 35, 41, 43, 56, 37, 38, 50, 52, 33, 44, 37, 72, 47, 20 }, new short[] { 80, 52, 38, 44, 39, 49, 50, 56, 62, 42, 54, 59, 35, 35, 32, 31, 37, 43, 48, 47, 38, 71, 56, 53 }, new short[] { 51, 25, 36, 54, 47, 71, 53, 59, 41, 42, 57, 50, 38, 31, 27, 33, 26, 40, 42, 31, 25 }, new short[] { 26, 47, 26, 37, 42, 15, 60, 40, 43, 48, 30, 25, 52, 28, 41, 40, 34, 28, 41, 38, 40, 30, 35, 27, 27, 32, 44, 31 }, new short[] { 32, 29, 31, 25, 21, 23, 25, 39, 33, 21, 36, 21, 14, 23, 33, 27 }, new short[] { 31, 16, 23, 21, 13, 20, 40, 13, 27, 33, 34, 31, 13, 40, 58, 24 }, new short[] { 24, 17, 18, 18, 21, 18, 16, 24, 15, 18, 33, 21, 14 }, new short[] { 24, 21, 29, 31, 26, 18 }, new short[] { 23, 22, 21, 32, 33, 24 }, new short[] { 30, 30, 21, 23 }, new short[] { 29, 23, 25, 18 }, new short[] { 10, 20, 13, 18, 28 }, new short[] { 12, 17, 18 }, new short[] { 20, 15, 16, 16, 25, 21 }, new short[] { 18, 26, 17, 22 }, new short[] { 16, 15, 15 }, new short[] { 25 }, new short[] { 14, 18, 19, 16, 14, 20, 28, 13, 28, 39, 40, 29, 25 }, new short[] { 27, 26, 18, 17, 20 }, new short[] { 25, 25, 22, 19, 14 }, new short[] { 21, 22, 18 }, new short[] { 10, 29, 24, 21, 21 }, new short[] { 13 }, new short[] { 14 }, new short[] { 25 }, new short[] { 20, 29, 22, 11, 14, 17, 17, 13, 21, 11, 19, 17, 18, 20, 8, 21, 18, 24, 21, 15, 27, 21 } };

        protected static byte[] prefix = System.Text.UTF8Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<versee>");
        protected static byte[] prefixIso = System.Text.UTF8Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\n<versee>");
        protected static byte[] suffix = System.Text.UTF8Encoding.UTF8.GetBytes("\n</versee>");

        protected BibleNames bookNames = null;

        #endregion Fields

        public BibleZtextWriter(string path, string iso2DigitLangCode,bool isIsoEncoding)
        {
            this.serial.iso2DigitLangCode = iso2DigitLangCode;
            this.serial.path = path;
            this.serial.isIsoEncoding = isIsoEncoding;
            ReloadSettingsFile();
        }

        #region Methods        

        protected byte[] getChapterBytes(int chapterNumber)
        {         
            ChapterPos versesForChapterPositions = chapters[chapterNumber];
            BookPos bookPos = bookPositions[versesForChapterPositions.booknum];

            FileStream fs = null;
            if (chapterNumber < CHAPTERS_IN_OT)
            {
                try
                {                    
                    fs = new FileStream(serial.path + "ot.bzz", FileMode.Open ,FileAccess.Read );
                }
                catch (Exception e)
                {
                    // does not exist
                    return System.Text.UTF8Encoding.UTF8.GetBytes("Does not exist - "+e.Message );
                }
            }
            else
            {
                try
                {
                    //fs = fileStorage.OpenFile(serial.path + "nt.bzz", FileMode.Open, FileAccess.Read);
                    fs = new FileStream(serial.path + "nt.bzz", FileMode.Open, FileAccess.Read);
                }
                catch (Exception e)
                {
                    // does not exist
                    return System.Text.UTF8Encoding.UTF8.GetBytes("Does not exist - " + e.Message);
                }
            }

            // adjust the start postion of the stream to where this book begins.
            // we must read the entire book up to the chapter we want even though we just want one chapter.
            fs.Position = bookPos.startPos;
            ZInputStream zipStream = new ZInputStream(fs);
            byte[] chapterBuffer = new byte[versesForChapterPositions.length];
            int totalBytesRead = 0;
            int totalBytesCopied = 0;
            int len = 0;
            try
            {
                byte[] buffer = new byte[10000];

                while (true)
                {
                    len = zipStream.read(buffer, 0, 10000);
                    if (len <= 0)
                    {
                        // we should never come to this point.  Just here as a safety procaution
                        break;
                    }
                    else
                    {
                        totalBytesRead += len;
                        if (totalBytesRead >= versesForChapterPositions.startPos)
                        {
                            // we are now inside of where the chapter we want is so we need to start saving it.
                            int startOffset = 0;
                            if (totalBytesCopied == 0)
                            {
                                // but our actual chapter might begin in the middle of the buffer.  Find the offset from the
                                // beginning of the buffer.
                                startOffset = len - (totalBytesRead - (int)versesForChapterPositions.startPos);                                
                            }
                            int i;
                            for (i = totalBytesCopied; i < versesForChapterPositions.length && (i - totalBytesCopied) < (len - startOffset); i++)
                            {
                                chapterBuffer[i] = buffer[i - totalBytesCopied + startOffset];
                            }
                            totalBytesCopied += (len - startOffset);
                            if (totalBytesCopied >= versesForChapterPositions.length)
                            {
                                // we are done. no more reason to read anymore of this book stream, just get out.
                                break;
                            }                           
                        }

                    }
                }
            }
            catch (Exception)
            {             
            }
            fs.Close();
            zipStream.Close();
            return chapterBuffer;
        }        

        protected long getInt48FromStream(FileStream fs)
        {
            byte[] buf = new byte[7];
            fs.Read(buf, 0, 7);
            return buf[2] * 0x100000000000000 + buf[1] * 0x100000000000 + buf[0] * 0x100000000 + buf[6] * 0x1000000 + buf[5] * 0x10000 + buf[4] * 0x100 + buf[3];
        }

        protected long getintFromStream(FileStream fs)
        {
            byte[] buf = new byte[4];
            fs.Read(buf, 0, 4);
            return buf[3] * 0x100000 + buf[2] * 0x10000 + buf[1] * 0x100 + buf[0];
        }

        protected int getShortIntFromStream(FileStream fs)
        {
            byte[] buf = new byte[2];
            fs.Read(buf, 0, 2);
            return buf[1] * 0x100 + buf[0];
        }        

        protected int getByteFromStream(FileStream fs)
        {
            byte[] buf = new byte[1];
            fs.Read(buf, 0, 1);
            return  buf[0];
        }
        
        private void ReloadSettingsFile()
        {
            //IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            bookPositions = new List<BookPos>();
            chapters = new List<ChapterPos>();
            FileStream fs = null;
            // read the Sword index to the ot bzz file which is the bzs file
            try
            {
                fs = new FileStream(serial.path + "ot.bzs", FileMode.Open, FileAccess.Read);
                for (int i = 0; i < BOOKS_IN_OT; i++)
                {
                    bookPositions.Add(new BookPos(getintFromStream(fs), getintFromStream(fs), getintFromStream(fs)));
                }
                fs.Close();

                // read the verse holder for versification bzv file old testememt
                fs = new FileStream(serial.path + "ot.bzv", FileMode.Open, FileAccess.Read);
                // dump the first 4 posts
                for (int i = 0; i < 4; i++)
                {
                    int booknum = getByteFromStream(fs);
                    long startPos = getInt48FromStream(fs);
                    int length = getShortIntFromStream(fs);
                }

                // now start getting old testament
                for (int i = 0; i < BOOKS_IN_OT; i++)
                {
                    for (int j = 0; j < CHAPTERS_IN_BOOK[i]; j++)
                    {
                        long chapterStartPos = 0;
                        ChapterPos chapt = null;
                        int booknum = 0;
                        long startPos = 0;
                        long lastNonZeroStartPos = 0;
                        int length = 0;
                        for (int k = 0; k < VERSES_IN_CHAPTER[i][j]; k++)
                        {
                            booknum = getByteFromStream(fs);
                            startPos = getInt48FromStream(fs);
                            if (startPos != 0)
                            {
                                lastNonZeroStartPos = startPos;
                            }
                            length = getShortIntFromStream(fs);
                            if (k == 0)
                            {
                                chapterStartPos = startPos;
                                chapt = new ChapterPos(chapterStartPos, i, j);
                                bookPositions[i].chapters.Add(chapt);
                            }
                            if (booknum == 0 && startPos == 0 && length == 0)
                            {
                                chapt.verses.Add(new VersePos(0, 0, i));
                            }
                            else
                            {
                                chapt.verses.Add(new VersePos(startPos - chapterStartPos, length, i));
                            }
                        }
                        // update the chapter length now that we know it
                        chapt.length = (int)(lastNonZeroStartPos - chapterStartPos) + length;

                        chapters.Add(chapt);

                        // dump a post for the chapter break
                        getByteFromStream(fs);
                        long x = getInt48FromStream(fs);
                        getShortIntFromStream(fs);
                    }
                    // dump a post for the book break
                    getByteFromStream(fs);
                    getInt48FromStream(fs);
                    getShortIntFromStream(fs);
                }
                fs.Close();
            }
            catch (Exception)
            {
                // failed to load old testement.  Maybe it does not exist.
                // fill with fake data
                for (int i = 0; i < BOOKS_IN_OT; i++)
                {
                    bookPositions.Add(new BookPos(0, 0, 0));
                }
                for (int i = 0; i < BOOKS_IN_OT; i++)
                {
                    for (int j = 0; j < CHAPTERS_IN_BOOK[i]; j++)
                    {
                        ChapterPos chapt = new ChapterPos(0, i, j);
                        for (int k = 0; k < VERSES_IN_CHAPTER[i][j]; k++)
                        {
                            chapt.verses.Add(new VersePos(0, 0, i));
                        }
                        // update the chapter length now that we know it
                        chapt.length =0;
                        chapters.Add(chapt);
                        bookPositions[i].chapters.Add(chapt);
                    }
                }
            }
            try
            {
                // do it for the new testement
                fs = new FileStream(serial.path + "nt.bzs", FileMode.Open, FileAccess.Read);
                for (int i = 0; i < BOOKS_IN_NT; i++)
                {
                    bookPositions.Add(new BookPos(getintFromStream(fs), getintFromStream(fs), getintFromStream(fs)));
                }
                fs.Close();

                // read the verse holder for versification bzv file new testememt
                fs = new FileStream(serial.path + "nt.bzv", FileMode.Open, FileAccess.Read);
                // dump the first 4 posts
                for (int i = 0; i < 4; i++)
                {
                    int booknum = getByteFromStream(fs);
                    long startPos = getInt48FromStream(fs);
                    int length = getShortIntFromStream(fs);
                }

                // now start getting new testament
                for (int i = BOOKS_IN_OT; i < (BOOKS_IN_OT + BOOKS_IN_NT); i++)
                {
                    for (int j = 0; j < CHAPTERS_IN_BOOK[i]; j++)
                    {
                        long chapterStartPos = 0;
                        ChapterPos chapt = null;
                        int booknum = 0;
                        long startPos = 0;
                        long lastNonZeroStartPos = 0;
                        int length = 0;
                        for (int k = 0; k < VERSES_IN_CHAPTER[i][j]; k++)
                        {
                            booknum = getByteFromStream(fs);
                            startPos = getInt48FromStream(fs);
                            if (startPos != 0)
                            {
                                lastNonZeroStartPos = startPos;
                            }
                            length = getShortIntFromStream(fs);
                            if (k == 0)
                            {
                                chapterStartPos = startPos;
                                chapt = new ChapterPos(chapterStartPos, i, j);
                                bookPositions[i].chapters.Add(chapt);
                            }
                            if (booknum == 0 && startPos == 0 && length == 0)
                            {
                                chapt.verses.Add(new VersePos(0, 0, i));
                            }
                            else
                            {
                                chapt.verses.Add(new VersePos(startPos - chapterStartPos, length, i));
                            }
                        }
                        // update the chapter length now that we know it
                        chapt.length = (int)(lastNonZeroStartPos - chapterStartPos) + length;

                        chapters.Add(chapt);

                        // dump a post for the chapter break
                        getByteFromStream(fs);
                        long x = getInt48FromStream(fs);
                        getShortIntFromStream(fs);
                    }
                    // dump a post for the book break
                    getByteFromStream(fs);
                    getInt48FromStream(fs);
                    getShortIntFromStream(fs);
                }
                fs.Close();
            }
            catch (Exception)
            {
                // failed to load new testement.  Maybe it does not exist.
                // fill with fake data
                for (int i = 0; i < BOOKS_IN_NT; i++)
                {
                    bookPositions.Add(new BookPos(0, 0, 0));
                }
                for (int i = BOOKS_IN_OT; i < (BOOKS_IN_OT + BOOKS_IN_NT); i++)
                {
                    for (int j = 0; j < CHAPTERS_IN_BOOK[i]; j++)
                    {
                        ChapterPos chapt = new ChapterPos(0, i, j);
                        for (int k = 0; k < VERSES_IN_CHAPTER[i][j]; k++)
                        {
                            chapt.verses.Add(new VersePos(0, 0, i));
                        }
                        // update the chapter length now that we know it
                        chapt.length = 0;
                        chapters.Add(chapt);
                        bookPositions[i].chapters.Add(chapt);
                    }
                }
            }
        }

        #endregion Methods

        #region Nested Types

        
        public struct VersePos
        {
            #region Fields

            
            public int booknum;
            
            public int length;
            
            public long startPos;

            #endregion Fields

            #region Constructors

            public VersePos(long startPos, int length, int booknum)
            {
                this.startPos = startPos;
                this.length = length;
                this.booknum = booknum;
            }

            #endregion Constructors
        }

        public class BookPos
        {
            #region Fields

            
            public List<ChapterPos> chapters = new List<ChapterPos>();
            
            public long length;
            
            public long startPos;
            
            public long unused;

            #endregion Fields

            #region Constructors

            public BookPos(long startPos, long length, long unused)
            {
                this.startPos = startPos;
                this.length = length;
                this.unused = unused;
            }

            #endregion Constructors
        }

        public class ChapterPos
        {
            #region Fields

            
            public int booknum;
            
            public int bookRelativeChapterNum;
            
            public int length = 0;
            
            public long startPos;
            
            public List<VersePos> verses = new List<VersePos>();

            #endregion Fields

            #region Constructors

            public ChapterPos(long startPos, int booknum, int bookRelativeChapterNum)
            {
                this.startPos = startPos;
                this.booknum = booknum;
                this.bookRelativeChapterNum = bookRelativeChapterNum;
            }

            #endregion Constructors
        }

        #endregion Nested Types
    }

    
    public class BibleZtextWriterSerialData
    {
        #region Fields

        
        public bool isIsoEncoding = false;
        
        public string iso2DigitLangCode = string.Empty;
        
        public string path = string.Empty;
        
        public int posChaptNum = 0;
        
        public int posVerseNum = 0;

        #endregion Fields

        #region Constructors

        public BibleZtextWriterSerialData(BibleZtextWriterSerialData from)
        {
            cloneFrom(from);
        }

        public BibleZtextWriterSerialData(bool isIsoEncoding, string iso2DigitLangCode, string path, int posChaptNum, int posVerseNum)
        {
            this.isIsoEncoding = isIsoEncoding;
            this.iso2DigitLangCode=iso2DigitLangCode;
            this.path=path;
            this.posChaptNum=posChaptNum;
            this.posVerseNum=posVerseNum;
        }

        #endregion Constructors

        #region Methods

        public void cloneFrom(BibleZtextWriterSerialData from)
        {
            this.isIsoEncoding=from.isIsoEncoding;
            this.iso2DigitLangCode=from.iso2DigitLangCode;
            this.path=from.path;
            this.posChaptNum=from.posChaptNum;
            this.posVerseNum=from.posVerseNum;
        }

        #endregion Methods
    }
}