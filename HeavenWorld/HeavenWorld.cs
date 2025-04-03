using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HBB
{
    class HeavenWorld
    {

        string filename;
        string sAbbreviatedTitle;
        string sFullTitle;
        public int iVerseSystem = 0; // full, ot or nt
        int max_records;
        LightningHeader lightningHeader;
        VerseContentRaw v_cache=null;
       static string[] saKJVBNOrder = new string[]
	    {
		    "bn1",
		    "bn2",
		    "bn3",
		    "bn4",
		    "bn5",
		    "bn6",
		    "bn7",
		    "bn8",
		    "bn9",
		    "bn10",
		    "bn11",
		    "bn12",
		    "bn13",
		    "bn14",
		    "bn15",
		    "bn16",
		    "bn17",
		    "bn18",
		    "bn19",
		    "bn20",
		    "bn21",
		    "bn22",
		    "bn23",
		    "bn24",
		    "bn25",
		    "bn26",
		    "bn27",
		    "bn28",
		    "bn29",
		    "bn30",
		    "bn31",
		    "bn32",
		    "bn33",
		    "bn34",
		    "bn35",
		    "bn36",
		    "bn37",
		    "bn38",
		    "bn39",
		    "bn40",
		    "bn41",
		    "bn42",
		    "bn43",
		    "bn44",
		    "bn45",
		    "bn46",
		    "bn47",
		    "bn48",
		    "bn49",
		    "bn50",
		    "bn51",
		    "bn52",
		    "bn53",
		    "bn54",
		    "bn55",
		    "bn56",
		    "bn57",
		    "bn58",
		    "bn59",
		    "bn60",
		    "bn61",
		    "bn62",
		    "bn63",
		    "bn64",
		    "bn65",
		    "bn66"		    
	    };
     
        static int[][] aiaKJV = new int[66][]{
	        new int[50]{31,25,24,26,32,22,24,22,29,32,32,20,18,24,21,16,27,33,38,18,34,24,20,67,34,35,46,22,35,43,55,32,20,31,29,43,36,30,23,23,57,38,34,34,28,34,31,22,33,26},
	        new int[40]{22,25,22,31,23,30,25,32,35,29,10,51,22,31,27,36,16,27,25,26,36,31,33,18,40,37,21,43,46,38,18,35,23,35,35,38,29,31,43,38},
	        new int[27]{17,16,17,35,19,30,38,36,24,20,47,8,59,57,33,34,16,30,37,27,24,33,44,23,55,46,34},
	        new int[36]{54,34,51,49,31,27,89,26,23,36,35,16,33,45,41,50,13,32,22,29,35,41,30,25,18,65,23,31,40,16,54,42,56,29,34,13},
	        new int[34]{46,37,29,49,33,25,26,20,29,22,32,32,18,29,23,22,20,22,21,20,23,30,25,22,19,19,26,68,29,20,30,52,29,12},
	        new int[24]{18,24,17,24,15,27,26,35,27,43,23,24,33,15,63,10,18,28,51,9,45,34,16,33},
	        new int[21]{36,23,31,24,31,40,25,35,57,18,40,15,25,20,20,31,13,31,30,48,25},
	        new int[4]{22,23,18,22},
	        new int[31]{28,36,21,22,12,21,17,22,27,27,15,25,23,52,35,23,58,30,24,42,15,23,29,22,44,25,12,25,11,31,13},
	        new int[24]{27,32,39,12,25,23,29,18,13,19,27,31,39,33,37,23,29,33,43,26,22,51,39,25},
	        new int[22]{53,46,28,34,18,38,51,66,28,29,43,33,34,31,34,34,24,46,21,43,29,53},
	        new int[25]{18,25,27,44,27,33,20,29,37,36,21,21,25,29,38,20,41,37,37,21,26,20,37,20,30},
	        new int[29]{54,55,24,43,26,81,40,40,44,14,47,40,14,17,29,43,27,17,19,8,30,19,32,31,31,32,34,21,30},
	        new int[36]{17,18,17,22,14,42,22,18,31,19,23,16,22,15,19,14,19,34,11,37,20,12,21,27,28,23,9,27,36,27,21,33,25,33,27,23},
	        new int[10]{11,70,13,24,17,22,28,36,15,44},
	        new int[13]{11,20,32,23,19,19,73,18,38,39,36,47,31},
	        new int[10]{22,23,15,17,14,14,10,17,32,3},
	        new int[42]{22,13,26,21,27,30,21,22,35,22,20,25,28,22,35,22,16,21,29,29,34,30,17,25,6,14,23,28,25,31,40,22,33,37,16,33,24,41,30,24,34,17},
	        new int[150]{6,12,8,8,12,10,17,9,20,18,7,8,6,7,5,11,15,50,14,9,13,31,6,10,22,12,14,9,11,12,24,11,22,22,28,12,40,22,13,17,13,11,5,26,17,11,9,14,20,23,19,9,6,7,23,13,11,11,17,12,8,12,11,10,13,20,7,35,36,5,24,20,28,23,10,12,20,72,13,19,16,8,18,12,13,17,7,18,52,17,16,15,5,23,11,13,12,9,9,5,8,28,22,35,45,48,43,13,31,7,10,10,9,8,18,19,2,29,176,7,8,9,4,8,5,6,5,6,8,8,3,18,3,3,21,26,9,8,24,13,10,7,12,15,21,10,20,14,9,6},
	        new int[31]{33,22,35,27,23,35,27,36,18,32,31,28,25,35,33,33,28,24,29,30,31,29,35,34,28,28,27,28,27,33,31},
	        new int[12]{18,26,22,16,20,12,29,17,18,20,10,14},
	        new int[8]{17,17,11,16,16,13,13,14},
	        new int[66]{31,22,26,6,30,13,25,22,21,34,16,6,22,32,9,14,14,7,25,6,17,25,18,23,12,21,13,29,24,33,9,20,24,17,10,22,38,22,8,31,29,25,28,28,25,13,15,22,26,11,23,15,12,17,13,12,21,14,21,22,11,12,19,12,25,24},
	        new int[52]{19,37,25,31,31,30,34,22,26,25,23,17,27,22,21,21,27,23,15,18,14,30,40,10,38,24,22,17,32,24,40,44,26,22,19,32,21,28,18,16,18,22,13,30,5,28,7,47,39,46,64,34},
	        new int[5]{22,22,66,22,22},
	        new int[48]{28,10,27,17,17,14,27,18,11,22,25,28,23,23,8,63,24,32,14,49,32,31,49,27,17,21,36,26,21,26,18,32,33,31,15,38,28,23,29,49,26,20,27,31,25,24,23,35},
	        new int[12]{21,49,30,37,31,28,28,27,27,21,45,13},
	        new int[14]{11,23,5,19,15,11,16,14,17,15,12,14,16,9},
	        new int[3]{20,32,21},
	        new int[9]{15,16,15,13,27,14,17,14,15},
	        new int[1]{21},
	        new int[4]{17,10,10,11},
	        new int[7]{16,13,12,13,15,16,20},
	        new int[3]{15,13,19},
	        new int[3]{17,20,19},
	        new int[3]{18,15,20},
	        new int[2]{15,23},
	        new int[14]{21,13,10,14,11,15,14,23,17,12,17,14,9,21},
	        new int[4]{14,17,18,6},
	        new int[28]{25,23,17,25,48,34,29,34,38,42,30,50,58,36,39,28,27,35,30,34,46,46,39,51,46,75,66,20},
	        new int[16]{45,28,35,41,43,56,37,38,50,52,33,44,37,72,47,20},
	        new int[24]{80,52,38,44,39,49,50,56,62,42,54,59,35,35,32,31,37,43,48,47,38,71,56,53},
	        new int[21]{51,25,36,54,47,71,53,59,41,42,57,50,38,31,27,33,26,40,42,31,25},
	        new int[28]{26,47,26,37,42,15,60,40,43,48,30,25,52,28,41,40,34,28,41,38,40,30,35,27,27,32,44,31},
	        new int[16]{32,29,31,25,21,23,25,39,33,21,36,21,14,23,33,27},
	        new int[16]{31,16,23,21,13,20,40,13,27,33,34,31,13,40,58,24},
	        new int[13]{24,17,18,18,21,18,16,24,15,18,33,21,14},
	        new int[6]{24,21,29,31,26,18},
	        new int[6]{23,22,21,32,33,24},
	        new int[4]{30,30,21,23},
	        new int[4]{29,23,25,18},
	        new int[5]{10,20,13,18,28},
	        new int[3]{12,17,18},
	        new int[6]{20,15,16,16,25,21},
	        new int[4]{18,26,17,22},
	        new int[3]{16,15,15},
	        new int[1]{25},
	        new int[13]{14,18,19,16,14,20,28,13,28,39,40,29,25},
	        new int[5]{27,26,18,17,20},
	        new int[5]{25,25,22,19,14},
	        new int[3]{21,22,18},
	        new int[5]{10,29,24,21,21},
	        new int[1]{13},
	        new int[1]{14},
	        new int[1]{25},
	        new int[22]{20,29,22,11,14,17,17,13,21,11,19,17,18,20,8,21,18,24,21,15,27,21},
        };


        static string[] saKJVBNOrder_NT = new string[]
	    {
		    "bn40",
		    "bn41",
		    "bn42",
		    "bn43",
		    "bn44",
		    "bn45",
		    "bn46",
		    "bn47",
		    "bn48",
		    "bn49",
		    "bn50",
		    "bn51",
		    "bn52",
		    "bn53",
		    "bn54",
		    "bn55",
		    "bn56",
		    "bn57",
		    "bn58",
		    "bn59",
		    "bn60",
		    "bn61",
		    "bn62",
		    "bn63",
		    "bn64",
		    "bn65",
		    "bn66"		    
	    };

        static int[][] aiaKJV_NT = new int[27][]{
	        new int[28]{25,23,17,25,48,34,29,34,38,42,30,50,58,36,39,28,27,35,30,34,46,46,39,51,46,75,66,20},
	        new int[16]{45,28,35,41,43,56,37,38,50,52,33,44,37,72,47,20},
	        new int[24]{80,52,38,44,39,49,50,56,62,42,54,59,35,35,32,31,37,43,48,47,38,71,56,53},
	        new int[21]{51,25,36,54,47,71,53,59,41,42,57,50,38,31,27,33,26,40,42,31,25},
	        new int[28]{26,47,26,37,42,15,60,40,43,48,30,25,52,28,41,40,34,28,41,38,40,30,35,27,27,32,44,31},
	        new int[16]{32,29,31,25,21,23,25,39,33,21,36,21,14,23,33,27},
	        new int[16]{31,16,23,21,13,20,40,13,27,33,34,31,13,40,58,24},
	        new int[13]{24,17,18,18,21,18,16,24,15,18,33,21,14},
	        new int[6]{24,21,29,31,26,18},
	        new int[6]{23,22,21,32,33,24},
	        new int[4]{30,30,21,23},
	        new int[4]{29,23,25,18},
	        new int[5]{10,20,13,18,28},
	        new int[3]{12,17,18},
	        new int[6]{20,15,16,16,25,21},
	        new int[4]{18,26,17,22},
	        new int[3]{16,15,15},
	        new int[1]{25},
	        new int[13]{14,18,19,16,14,20,28,13,28,39,40,29,25},
	        new int[5]{27,26,18,17,20},
	        new int[5]{25,25,22,19,14},
	        new int[3]{21,22,18},
	        new int[5]{10,29,24,21,21},
	        new int[1]{13},
	        new int[1]{14},
	        new int[1]{25},
	        new int[22]{20,29,22,11,14,17,17,13,21,11,19,17,18,20,8,21,18,24,21,15,27,21},
        };

        static string[] saKJVBNOrder_OT = new string[]
	    {
		    "bn1",
		    "bn2",
		    "bn3",
		    "bn4",
		    "bn5",
		    "bn6",
		    "bn7",
		    "bn8",
		    "bn9",
		    "bn10",
		    "bn11",
		    "bn12",
		    "bn13",
		    "bn14",
		    "bn15",
		    "bn16",
		    "bn17",
		    "bn18",
		    "bn19",
		    "bn20",
		    "bn21",
		    "bn22",
		    "bn23",
		    "bn24",
		    "bn25",
		    "bn26",
		    "bn27",
		    "bn28",
		    "bn29",
		    "bn30",
		    "bn31",
		    "bn32",
		    "bn33",
		    "bn34",
		    "bn35",
		    "bn36",
		    "bn37",
		    "bn38",
		    "bn39"   
	    };

        static int[][] aiaKJV_OT = new int[39][]{
	        new int[50]{31,25,24,26,32,22,24,22,29,32,32,20,18,24,21,16,27,33,38,18,34,24,20,67,34,35,46,22,35,43,55,32,20,31,29,43,36,30,23,23,57,38,34,34,28,34,31,22,33,26},
	        new int[40]{22,25,22,31,23,30,25,32,35,29,10,51,22,31,27,36,16,27,25,26,36,31,33,18,40,37,21,43,46,38,18,35,23,35,35,38,29,31,43,38},
	        new int[27]{17,16,17,35,19,30,38,36,24,20,47,8,59,57,33,34,16,30,37,27,24,33,44,23,55,46,34},
	        new int[36]{54,34,51,49,31,27,89,26,23,36,35,16,33,45,41,50,13,32,22,29,35,41,30,25,18,65,23,31,40,16,54,42,56,29,34,13},
	        new int[34]{46,37,29,49,33,25,26,20,29,22,32,32,18,29,23,22,20,22,21,20,23,30,25,22,19,19,26,68,29,20,30,52,29,12},
	        new int[24]{18,24,17,24,15,27,26,35,27,43,23,24,33,15,63,10,18,28,51,9,45,34,16,33},
	        new int[21]{36,23,31,24,31,40,25,35,57,18,40,15,25,20,20,31,13,31,30,48,25},
	        new int[4]{22,23,18,22},
	        new int[31]{28,36,21,22,12,21,17,22,27,27,15,25,23,52,35,23,58,30,24,42,15,23,29,22,44,25,12,25,11,31,13},
	        new int[24]{27,32,39,12,25,23,29,18,13,19,27,31,39,33,37,23,29,33,43,26,22,51,39,25},
	        new int[22]{53,46,28,34,18,38,51,66,28,29,43,33,34,31,34,34,24,46,21,43,29,53},
	        new int[25]{18,25,27,44,27,33,20,29,37,36,21,21,25,29,38,20,41,37,37,21,26,20,37,20,30},
	        new int[29]{54,55,24,43,26,81,40,40,44,14,47,40,14,17,29,43,27,17,19,8,30,19,32,31,31,32,34,21,30},
	        new int[36]{17,18,17,22,14,42,22,18,31,19,23,16,22,15,19,14,19,34,11,37,20,12,21,27,28,23,9,27,36,27,21,33,25,33,27,23},
	        new int[10]{11,70,13,24,17,22,28,36,15,44},
	        new int[13]{11,20,32,23,19,19,73,18,38,39,36,47,31},
	        new int[10]{22,23,15,17,14,14,10,17,32,3},
	        new int[42]{22,13,26,21,27,30,21,22,35,22,20,25,28,22,35,22,16,21,29,29,34,30,17,25,6,14,23,28,25,31,40,22,33,37,16,33,24,41,30,24,34,17},
	        new int[150]{6,12,8,8,12,10,17,9,20,18,7,8,6,7,5,11,15,50,14,9,13,31,6,10,22,12,14,9,11,12,24,11,22,22,28,12,40,22,13,17,13,11,5,26,17,11,9,14,20,23,19,9,6,7,23,13,11,11,17,12,8,12,11,10,13,20,7,35,36,5,24,20,28,23,10,12,20,72,13,19,16,8,18,12,13,17,7,18,52,17,16,15,5,23,11,13,12,9,9,5,8,28,22,35,45,48,43,13,31,7,10,10,9,8,18,19,2,29,176,7,8,9,4,8,5,6,5,6,8,8,3,18,3,3,21,26,9,8,24,13,10,7,12,15,21,10,20,14,9,6},
	        new int[31]{33,22,35,27,23,35,27,36,18,32,31,28,25,35,33,33,28,24,29,30,31,29,35,34,28,28,27,28,27,33,31},
	        new int[12]{18,26,22,16,20,12,29,17,18,20,10,14},
	        new int[8]{17,17,11,16,16,13,13,14},
	        new int[66]{31,22,26,6,30,13,25,22,21,34,16,6,22,32,9,14,14,7,25,6,17,25,18,23,12,21,13,29,24,33,9,20,24,17,10,22,38,22,8,31,29,25,28,28,25,13,15,22,26,11,23,15,12,17,13,12,21,14,21,22,11,12,19,12,25,24},
	        new int[52]{19,37,25,31,31,30,34,22,26,25,23,17,27,22,21,21,27,23,15,18,14,30,40,10,38,24,22,17,32,24,40,44,26,22,19,32,21,28,18,16,18,22,13,30,5,28,7,47,39,46,64,34},
	        new int[5]{22,22,66,22,22},
	        new int[48]{28,10,27,17,17,14,27,18,11,22,25,28,23,23,8,63,24,32,14,49,32,31,49,27,17,21,36,26,21,26,18,32,33,31,15,38,28,23,29,49,26,20,27,31,25,24,23,35},
	        new int[12]{21,49,30,37,31,28,28,27,27,21,45,13},
	        new int[14]{11,23,5,19,15,11,16,14,17,15,12,14,16,9},
	        new int[3]{20,32,21},
	        new int[9]{15,16,15,13,27,14,17,14,15},
	        new int[1]{21},
	        new int[4]{17,10,10,11},
	        new int[7]{16,13,12,13,15,16,20},
	        new int[3]{15,13,19},
	        new int[3]{17,20,19},
	        new int[3]{18,15,20},
	        new int[2]{15,23},
	        new int[14]{21,13,10,14,11,15,14,23,17,12,17,14,9,21},
	        new int[4]{14,17,18,6}
        };


        public HeavenWorld(string mFileName)
        {
            filename = mFileName;
            LoadHeaderInfo();
        }

        private void LoadHeaderInfo()
        {
            lightningHeader = new LightningHeader();
            string text = "";


            byte[] array = new byte[18];
            byte[] array2 = null;
            byte[] array3 = new byte[80];
            try
            {
                FileStream stream = new FileStream(filename, FileMode.Open);
                stream.Read(array, 0, 18);
                int i;
                for (i = 0; i < 8; i++)
                {
                    lightningHeader.baBitHeader[i] = array[i];
                }
                lightningHeader.usMaxRecords = BitConverter.ToUInt16(array, i);
                max_records = lightningHeader.usMaxRecords;
                if (max_records >= 31102)
                    iVerseSystem = 0;
                else if (max_records >= 23145)
                    iVerseSystem = 1;
                else if (max_records >= 7957)
                    iVerseSystem = 2;
                i += 2;
                lightningHeader.uiISBN = BitConverter.ToUInt32(array, i);
                i += 4;
                lightningHeader.uiSubSectionPointerOffset = BitConverter.ToUInt32(array, i);
                int num = (int)(lightningHeader.uiSubSectionPointerOffset - 18u);
                array2 = new byte[num];
                byte[] array4 = new byte[num];
                stream.Read(array2, 0, num);
                int num2 = i = 0;
                while (i < 4)
                {
                    int num3 = 0;
                    while (num2 < array2.Length && array2[num2] > 0)
                    {
                        array4[num3] = array2[num2];
                        num2++;
                        num3++;
                    }
                    if (num2 < array2.Length)
                    {
                        array4[num3] = array2[num2];
                        num2++;
                    }
                    if (num3 > 0)
                    {
                        lightningHeader.saBasicInfo[i] = Encoding.UTF8.GetString(array4, 0, num3);
                    }
                    else
                    {
                        lightningHeader.saBasicInfo[i] = "";
                    }
                    i++;
                }
                stream.Seek((long)((ulong)lightningHeader.uiSubSectionPointerOffset), SeekOrigin.Begin);
                stream.Read(array3, 0, 80);
                for (i = 0; i < 20; i++)
                {
                    lightningHeader.auiSubSectionPointer[i] = BitConverter.ToUInt32(array3, i * 4);
                }
                stream.Dispose();
            }
            catch (Exception ex)
            {
                return;
            }
            if (text.Length > 0)
            {
                return;
            }
            else
            {
               //this.sISBN = this.lightningHeader.uiISBN.ToString();
                this.sAbbreviatedTitle = lightningHeader.saBasicInfo[0];
                this.sFullTitle = lightningHeader.saBasicInfo[1];
                //this.lightningReader.dictContentByPath.Add(this.uriPath, this);
                //this.lightningReader.dictContentByISBN.Add(this.lightningHeader.uiISBN, this);
                //result = true;
                return;
            }
        }

        public string getVerse(int book, int chapter, int verse)
        {            
            int rec_no = GetRecordNumber("bn" + book, chapter, verse);
            VerseContentRaw v = LoadVerseRawContent(rec_no, 10);
            byte[] bytes=v.baText;
            for (int i = 0; i < bytes.Length; i++)
                if(bytes[i]==1)
                bytes[i] = 0x20;
            return Encoding.UTF8.GetString(bytes);
        }

        public string getVerseCache(int book, int chapter, int verse)
        {
            int rec_no = GetRecordNumber("bn" + book, chapter, verse);
            if(v_cache==null)
                v_cache = LoadVerseRawContent(rec_no, max_records);

            byte[] idx_bytes_start = new byte[] { v_cache.baIndex[4 * (rec_no - 1) ], v_cache.baIndex[4 * (rec_no - 1) + 1], v_cache.baIndex[4 * (rec_no - 1) + 2], v_cache.baIndex[4 * (rec_no - 1) + 3] };
            byte[] idx_bytes_end = new byte[] { v_cache.baIndex[4 * rec_no], v_cache.baIndex[4 * rec_no + 1], v_cache.baIndex[4 * rec_no + 2], v_cache.baIndex[4 * rec_no + 3] };
            int idx_start = BitConverter.ToInt32(idx_bytes_start, 0);
            int idx_end = BitConverter.ToInt32(idx_bytes_end, 0);
            byte[] bytes = new byte[idx_end - idx_start];
            for (int i = idx_start; i < idx_end; i++)
                if(v_cache.baText[i]==1)
                    bytes[i - idx_start] = 0x20;
                else
                    bytes[i - idx_start] = v_cache.baText[i];
            return Encoding.UTF8.GetString(bytes);
        }

        public int GetRecordNumber(string sBN, int iCh, int iVs)
        {
            int num = -1;
            int num2 = -1;
            int num3 = iVerseSystem;
            int[][] array = null;
            string[] array2 = null;
            if (!IsValidBCV(sBN, iCh, iVs, ref num2, iVerseSystem))
            {
                return num;
            }
            if (iVerseSystem == 2 || iVerseSystem == 9)
            {
                iVerseSystem = 0;
            }
            SetVerseSystemArrays(iVerseSystem, ref array, ref array2);
            int bookOffset = GetBookOffset(sBN, ref array2);
            for (int i = num = 0; i < bookOffset; i++)
            {
                int[] array3 = array[i];
                for (int j = 0; j < array3.Length; j++)
                {
                    int num4 = array3[j];
                    num += num4;
                }
            }
            for (int i = 1; i < iCh; i++)
            {
                num += array[bookOffset][i - 1];
            }
            num += iVs;
            if (num3 == 2 || num3 == 9)
            {
                num -= GetRecordNumber("bn40", 1, 1) - 1;
            }
            return num;
        }

        private static void SetVerseSystemArrays(int iVerseSystem, ref int[][] aiaIQ, ref string[] saIQ)
        {
            switch (iVerseSystem)
            {
                case 0: // full bible
                    aiaIQ = aiaKJV;
                    saIQ = saKJVBNOrder;
                    return;
                case 1: //OT only
                    aiaIQ = aiaKJV_OT;
                    saIQ = saKJVBNOrder_OT;
                    return;
                case 2: // NT Only
                    aiaIQ = aiaKJV_NT;
                    saIQ = saKJVBNOrder_NT;
                    return;               
                default:
                    aiaIQ = aiaKJV;
                    saIQ = saKJVBNOrder;
                    return;
            }
        }

        public static bool IsValidBCV(string sBN, int iCh, int iVs, ref int iErrorType, int iVerseSystem)
        {
            int[][] array = null;
            string[] array2 = null;
            SetVerseSystemArrays(iVerseSystem, ref array, ref array2);
            int bookOffset;
            if (-1 == (bookOffset = GetBookOffset(sBN, ref array2)))
            {
                iErrorType = 0;
                return false;
            }
            if (iCh < 1 || iCh > array[bookOffset].Length)
            {
                iErrorType = 1;
                return false;
            }
            if (iVs < 1 || iVs > array[bookOffset][iCh - 1])
            {
                iErrorType = 2;
                return false;
            }
            return true;
        }


        private static int GetBookOffset(string sBN, ref string[] saIQ)
        {
            int result = -1;
            for (int i = 0; i < saIQ.Length; i++)
            {
                if (sBN == saIQ[i])
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        public VerseContentRaw LoadVerseRawContent(int iRecord, int iNumRecords)
        {
            byte[] baBibleVerseIndex;
            byte[] baBibleVerseText = null;            
            VerseContentRaw verseContentRaw = new VerseContentRaw();
            string text = "";
            uint[] array = new uint[2];
            VerseContentRaw result;

            baBibleVerseIndex = new byte[124412];

            uint num = lightningHeader.auiSubSectionPointer[0];
            try
            {
                Stream stream = new FileStream(filename, FileMode.Open);
                stream.Seek((long)((ulong)num), SeekOrigin.Begin);
                stream.Read(baBibleVerseIndex, 0, 124412);
                array[0] = BitConverter.ToUInt32(baBibleVerseIndex, 0) + lightningHeader.auiSubSectionPointer[1];
                array[1] = BitConverter.ToUInt32(baBibleVerseIndex, 124408) + lightningHeader.auiSubSectionPointer[1];
                int num2 = (int)(array[1] - array[0]);
                if (num2 > 0)
                {
                    baBibleVerseText = new byte[num2];
                    stream.Seek((long)((ulong)array[0]), SeekOrigin.Begin);
                    stream.Read(baBibleVerseText, 0, num2);
                }
                stream.Dispose();
            }
            catch (Exception ex)
            {
                text = ex.Message;
            }
            if (text.Length > 0)
            {
                result = null;
                return result;
            }

            int count = 4 * (iRecord - 1);
            int count2 = 4 * (iNumRecords + 1);
            verseContentRaw.baIndex = baBibleVerseIndex.Skip(count).Take(count2).ToArray<byte>();
            array[0] = BitConverter.ToUInt32(verseContentRaw.baIndex, 0);
            array[1] = BitConverter.ToUInt32(verseContentRaw.baIndex, 4 * iNumRecords);
            verseContentRaw.baText = baBibleVerseText.Skip((int)array[0]).Take((int)(array[1] - array[0])).ToArray<byte>();
            result = verseContentRaw;
            return result;
        }
    }
}
