#region Header

// <copyright file="BibleNoteReader.cs" company="Thomas Dilts">
//
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace Sword.reader
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///   Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    public class BibleNoteReader : BibleZtextReader
    {
        #region Fields

        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(false, "", "", 0, 0);
        public string TitleBrowserWindow = string.Empty;

        #endregion Fields

        #region Constructors

        public BibleNoteReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string titleBrowserWindow)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            Serial2.CloneFrom(Serial);
            TitleBrowserWindow = titleBrowserWindow;
        }

        #endregion Constructors

        #region Properties

        public override bool IsHearable
        {
            get { return false; }
        }

        public override bool IsLocalChangeDuringLink
        {
            get { return false; }
        }

        public override bool IsSearchable
        {
            get { return false; }
        }

        public override bool IsTranslateable
        {
            get { return true; }
        }

        #endregion Properties

        #region Methods

        public override void GetInfo(out int bookNum, out int absoluteChaptNum, out int relChaptNum, out int verseNum,
            out string fullName, out string title)
        {
            verseNum = Serial.PosVerseNum;
            absoluteChaptNum = Serial.PosChaptNum;
            GetInfo(Serial.PosChaptNum, Serial.PosVerseNum, out bookNum, out relChaptNum, out fullName, out title);
            title = TitleBrowserWindow + " " + fullName + ":" + (relChaptNum + 1);
        }

        public override string GetVerseTextOnly(DisplaySettings displaySettings, int chapterNumber, int verseNumber)
        {
            //give them the notes if you can.
            var chapterBuffer = GetChapterBytes(chapterNumber);
            var verse = Chapters[chapterNumber].Verses[verseNumber];
            int noteMarker = 'a';
            return ParseOsisText(displaySettings,
                                 "",
                                 "",
                                 chapterBuffer,
                                 (int) verse.StartPos,
                                 verse.Length,
                                 Serial.IsIsoEncoding,
                                 true,
                                 true,
                                 ref noteMarker);
        }

        public override void Resume()
        {
            Serial.CloneFrom(Serial2);
            base.Resume();
        }

        public override void SerialSave()
        {
            Serial2.CloneFrom(Serial);
        }

        protected override string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor,
            string htmlForegroundColor, string htmlPhoneAccentColor,
            double htmlFontSize,string fontFamily, bool isNotesOnly, bool addStartFinishHtml, bool forceReload)
        {
            return GetChapterHtml(displaySettings, Serial.PosChaptNum, htmlBackgroundColor, htmlForegroundColor,
                                  htmlPhoneAccentColor, htmlFontSize,fontFamily, true, addStartFinishHtml, forceReload);
        }

        #endregion Methods
    }
    public class DisplaySettings
    {
        #region Fields

        public string CustomBibleDownloadLinks = 
            @"/bibles/raw,/bibles/biblelist";
        public bool EachVerseNewLine;
        public string GreekDictionaryLink = 
            @"";
        public string HebrewDictionaryLink = 
            @"";
        public bool HighlightMarkings;
        public int NumberOfScreens = 3;
        public bool ShowAddedNotesByChapter;
        public bool ShowBookName;
        public bool ShowChapterNumber;
        public bool ShowHeadings = true;
        public bool ShowMorphology;
        public bool ShowNotePositions;
        public bool ShowStrongsNumbers;
        public bool ShowVerseNumber = true;
        public bool SmallVerseNumbers = true;
        public string SoundLink = 
            @"";
        public bool UseInternetGreekHebrewDict;
        public string UserUniqueGuuid = "";
        public bool WordsOfChristRed;

        #endregion Fields

        #region Methods

        public void CheckForNullAndFix()
        {
            var fixer = new DisplaySettings();
            if (SoundLink == null || SoundLink.Equals(@"http://www.chaniel.se/crossconnect/bibles/talking/getabsolutechapter.php?chapternum={0}&language={1}"))
            {
                SoundLink = fixer.SoundLink;
            }
            if (GreekDictionaryLink == null)
            {
                GreekDictionaryLink = fixer.GreekDictionaryLink;
            }
            if (HebrewDictionaryLink == null)
            {
                HebrewDictionaryLink = fixer.HebrewDictionaryLink;
            }
            if (CustomBibleDownloadLinks == null || CustomBibleDownloadLinks.Equals(@"www.chaniel.se,/crossconnect/bibles/raw,/crossconnect/bibles/biblelist"))
            {
                CustomBibleDownloadLinks = fixer.CustomBibleDownloadLinks;
            }
            if (string.IsNullOrEmpty(UserUniqueGuuid))
            {
                UserUniqueGuuid = Guid.NewGuid().ToString();
            }
            if(NumberOfScreens==0)
            {
                NumberOfScreens = 3;
            }
        }

        public DisplaySettings Clone()
        {
            var cloned = new DisplaySettings
                             {
                                 CustomBibleDownloadLinks = CustomBibleDownloadLinks,
                                 EachVerseNewLine = EachVerseNewLine,
                                 GreekDictionaryLink = GreekDictionaryLink,
                                 HebrewDictionaryLink = HebrewDictionaryLink,
                                 HighlightMarkings = HighlightMarkings,
                                 ShowAddedNotesByChapter = ShowAddedNotesByChapter,
                                 ShowBookName = ShowBookName,
                                 ShowChapterNumber = ShowChapterNumber,
                                 ShowHeadings = ShowHeadings,
                                 ShowMorphology = ShowMorphology,
                                 ShowNotePositions = ShowNotePositions,
                                 ShowStrongsNumbers = ShowStrongsNumbers,
                                 ShowVerseNumber = ShowVerseNumber,
                                 SmallVerseNumbers = SmallVerseNumbers,
                                 SoundLink = SoundLink,
                                 WordsOfChristRed = WordsOfChristRed,
                                 UserUniqueGuuid = UserUniqueGuuid,
                                 UseInternetGreekHebrewDict = UseInternetGreekHebrewDict
                             };
            return cloned;
        }

        #endregion Methods
    }
}