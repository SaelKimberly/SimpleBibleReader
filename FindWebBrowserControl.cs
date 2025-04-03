using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;

namespace Simple_Bible_Reader
{
    public partial class FindWebBrowserControl : Form
    {
        WebBrowser SearchResultsArea = null;
        RendererWebBrowserControl sbr = null;
        public string searchTerm = null;

        public static int SEARCH_TYPE_AND = 0;
        public static int SEARCH_TYPE_OR = 1;
        public static int SEARCH_TYPE_EXACT = 2;
        public static int MAX_SEARCH_LIMIT = 1000;
        int searchType = 0;
        public bool showExtraVerses = true;
        string searchResultsHtml = "";
        public System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
        public string rtf_header = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fprq2\\fcharset0 *DEFAULT*;}{\\f1\\fnil\\fprq2\\fcharset161 *GREEK*;}{\\f2\\fbidi\\fprq2\\fcharset177 *HEBREW*;}{\\f3\\fnil\\fprq2\\fcharset0 *LATIN*;}{\\f4\\fnil\\fprq2\\fcharset136 *TRADITIONAL*;}{\\f5\\fnil\\fprq2\\fcharset204 *CYRILLIC*;}{\\f6\\fnil\\fprq2\\fcharset238 *EUROPEAN*;}{\\f7\\fnil\\fprq2\\fcharset134 *SIMPLIFIED*;}{\\f8\\fnil\\fprq2\\fcharset222 *THAI*;}{\\f9\\fnil\\fprq2\\fcharset129 *KOREAN*;}{\\f10\\fbidi\\fprq2\\fcharset178 *ARABIC*;}{\\f11\\fnil\\fprq2\\fcharset162 *TURKISH*;}{\\f12\\fnil\\fprq2\\fcharset163 *VIETNAMESE*;}{\\f13\\fnil\\fprq2\\fcharset128 *JAPANESE*;}}";


        public static string CSS
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<style type=\'text/css\'>");
                sb.Append("body { ");
                sb.Append("  font-size: " + GlobalMemory.getInstance().LegacyFontSizeBIB  + ";");
                sb.Append("  font-family: " + GlobalMemory.getInstance().FontFamilyBIB + ";");   
                sb.Append("}");

                sb.Append("li a { ");
                sb.Append("   cursor: default;");
                sb.Append("   color: black;");
                sb.Append("   text-decoration: none;");
                sb.Append("  font-size: " + GlobalMemory.getInstance().LegacyFontSizeBIB  + ";");
                sb.Append("  font-family: " + GlobalMemory.getInstance().FontFamilyBIB + ";");   
                sb.Append("}");
                sb.Append("li a:hover { ");
                sb.Append("   background: #CFECEC;");
                sb.Append("   cursor: default;");
                sb.Append("   color: black;");
                sb.Append("   text-decoration: none;");
                sb.Append("  font-size: " + GlobalMemory.getInstance().LegacyFontSizeBIB  + ";");
                sb.Append("  font-family: " + GlobalMemory.getInstance().FontFamilyBIB + ";");   
                sb.Append("}");

                sb.Append(".TreeView ");
                sb.Append("{");
                //sb.Append("    font: Verdana;");
                sb.Append("  font-size: " + GlobalMemory.getInstance().LegacyFontSizeBIB  + ";");
                sb.Append("  font-family: " + GlobalMemory.getInstance().FontFamilyBIB + ";");   
                sb.Append("    line-height: 20px;");
                sb.Append("	cursor: default; ");
                sb.Append("	font-style: normal;");
                sb.Append("}");
                sb.Append("");
                sb.Append(".TreeView LI");
                sb.Append("{");
                sb.Append("    /* The padding is for the tree view nodes */");
                sb.Append("  font-size: " + GlobalMemory.getInstance().LegacyFontSizeBIB  + ";");
                sb.Append("  font-family: " + GlobalMemory.getInstance().FontFamilyBIB + ";");   
                sb.Append("    padding: 0 0 0 18px;");
                sb.Append("    float: left;");
                sb.Append("    width: 100%;");
                sb.Append("    list-style: none;");
                sb.Append("}");
                sb.Append("TD");
                sb.Append("{");
                sb.Append("  font-size: " + GlobalMemory.getInstance().LegacyFontSizeBIB  + ";");
                sb.Append("  font-family: " + GlobalMemory.getInstance().FontFamilyBIB + ";");   
                sb.Append("}");
                sb.Append(".TreeView, .TreeView ul");
                sb.Append("{");
                sb.Append("    margin: 0;");
                sb.Append("    padding: 0;");
                sb.Append("}");
                sb.Append("");
                sb.Append("LI.Expanded ");
                sb.Append("{");
                sb.Append("    background: url(data:image/gif;base64,R0lGODlhEAAQAKEBAICAgP///wAAAP///yH5BAEKAAMALAAAAAAQABAAAAIjnI+py+0PE5iULhCyDuBu3SnYx3nCeZYiqUps6JIwUtXR3RQAOw==) no-repeat left top;");
                sb.Append("}");
                sb.Append("");
                sb.Append("LI.Expanded ul");
                sb.Append("{");
                sb.Append("    display: block;");
                sb.Append("}");
                sb.Append("");
                sb.Append("LI.Collapsed ");
                sb.Append("{");
                sb.Append("    background: url(data:image/gif;base64,R0lGODlhEAAQAKEBAICAgP///wAAAP///yH5BAEKAAMALAAAAAAQABAAAAImnI+py+0PE5iULhCyDuBqoXUKloGZKAXCunJeGb5qPG60VFXR3hQAOw==) no-repeat left top;");
                sb.Append("}");
                sb.Append("");
                sb.Append("LI.Collapsed ul");
                sb.Append("{");
                sb.Append("    display: none;");
                sb.Append("}");
                sb.Append("");
                sb.Append(".Highlighted");
                sb.Append("{");
                sb.Append("    color: blue;");
                sb.Append("}");
                sb.Append("");
                sb.Append(".AlternateHighlight");
                sb.Append("{");
                sb.Append("    color: blue;");
                sb.Append("}");
                sb.Append("</style>");
                return sb.ToString();

            }
        }

        public static string JS
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<script type='text/javascript' language='javascript'>");
                sb.Append(Simple_Bible_Reader.Properties.Resources.SearchResultTreeJS);
                sb.Append("</script>");
                return sb.ToString();
            }
        }

        public FindWebBrowserControl(WebBrowser search_area,RendererWebBrowserControl _sbr)
        {
            InitializeComponent();            
            this.SearchResultsArea = search_area;
            this.sbr = _sbr;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CheckKeys);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Please enter a text to search!", "Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            searchTerm = textBox1.Text;
            showExtraVerses = this.sbr.moreVersesBtn.Checked;
            

            if (!this.sbr.searchToolStripMenuItem.Checked)
                this.sbr.searchToolStripMenuItem.PerformClick();

            if (radioAndBtn.Checked)
                searchType = SEARCH_TYPE_AND;
            else if (radioOrBtn.Checked)
                searchType = SEARCH_TYPE_OR;
            else if (radioExactBtn.Checked)
                searchType = SEARCH_TYPE_EXACT;


            this.Visible = false;
            if (!searchBackgroundJob.IsBusy)
                searchBackgroundJob.RunWorkerAsync();
            else
                MessageBox.Show("Another search is already in action!","Search");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private static string ReplaceEx(string original,
                    string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) *
                      (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern,
                                              position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            return new string(chars, 0, count);
        }

        private void CheckKeys(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                button1.PerformClick();
            }
        }

        private void searchBackgroundJob_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker b = (BackgroundWorker)sender;
            string search_term = searchTerm;
            StringBuilder sb = new StringBuilder();
            sb.Append("<html dir=\'ltr\'><head>");
            sb.Append(CSS);
            sb.Append(JS);
            sb.Append("</head><body><b>");
            sb.Append(Localization.Find_SearchResultsFor+" '" + search_term + "' (@count@)");
            sb.Append("</b><hr>");
            StringBuilder result_verse = new StringBuilder();
            XmlNodeList verses = BibleFormat.BibleXmlDocument.SelectNodes("//VERS");
            string tmp = "";
            string tmp2 = "";
            string t1 = "";
            string t2 = "";
            string vref = "";
            string c_book = "";
            Dictionary<string, Dictionary<string, string>> results = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> tmp_verses = null;
            bool found = false;
            string[] andArray = null;
            //
            int search_count = 0;
            int found_count = 0;
            int total_verses = verses.Count;            

            //
            int prev_percent=0;
            string stmp = "";
            bool match_found = false;
            foreach (XmlNode verse in verses)
            {
                ///////////////////////////////////////// for every verse ///////////////////////////////////
                search_count++;
                match_found = false;
                if(prev_percent != (search_count * 100 / total_verses))
                    b.ReportProgress((search_count * 100 / total_verses));
                prev_percent = (search_count * 100 / total_verses);

                tmp = verse.InnerText;

                if (verse.PreviousSibling != null)
                {
                    if(verse.PreviousSibling.Attributes["vnumber"]!=null)
                        t1 = "<sup><span style='text-color:black;'>" + verse.PreviousSibling.Attributes["vnumber"].Value + "</span></sup>" + verse.PreviousSibling.InnerText;
                    t1 = "";
                }
                else
                    t1 = "";
                if (verse.NextSibling != null)
                    t2 = "<sup><span style='text-color:black;'>" + verse.NextSibling.Attributes["vnumber"].Value + "</span></sup>" + verse.NextSibling.InnerText;
                else
                    t2 = "";

                //====================================== different search replaces ===================================
                ///############ Regular Expression support
                if (chkBoxMatchCase.Checked && chkBoxRegex.Checked)
                {
                    //Regex.IsMatch
                    tmp = Regex.Replace(tmp, searchTerm, "<span style='background-color:yellow'>$0</span>");
                    if (tmp == verse.InnerText)
                        match_found = false;
                    else
                        match_found = true;
                }
                else if (!chkBoxMatchCase.Checked && chkBoxRegex.Checked)
                {            
                    tmp = Regex.Replace(tmp, searchTerm, "<span style='background-color:yellow'>$0</span>", RegexOptions.IgnoreCase);
                    if (tmp == verse.InnerText)
                        match_found = false;
                    else
                        match_found = true;
                }
                //############ Case sensitive Exact/And/Or
                else if (chkBoxMatchCase.Checked && searchType == SEARCH_TYPE_EXACT)
                {
                    if (tmp.IndexOf(search_term) != -1)
                    {
                        tmp = tmp.Replace(searchTerm, "<span style='background-color:yellow'>" + searchTerm + "</span>");
                        match_found = true;
                    }
                    else
                        match_found = false;
                }
                else if (chkBoxMatchCase.Checked && searchType == SEARCH_TYPE_AND)
                {
                    search_term = search_term.Replace("  ", " ");
                    search_term = search_term.Replace("  ", " ");
                    andArray = search_term.Split(' ');
                    found = true;
                    for (int i = 0; i < andArray.Length; i++)
                    {
                        if (tmp.IndexOf(andArray[i]) == -1)
                        {
                            found = false;
                            break;
                        }
                        tmp = tmp.Replace(andArray[i], "<span style='background-color:yellow'>" + andArray[i] + "</span>");
                    }
                    if (!found)
                        match_found = false;
                    else
                        match_found = true;
                }
                else if (chkBoxMatchCase.Checked && searchType == SEARCH_TYPE_OR)
                {                    
                    search_term = search_term.Replace("  ", " ");
                    search_term = search_term.Replace("  ", " ");
                    andArray = search_term.Split(' ');
                    for (int i = 0; i < andArray.Length; i++)
                        tmp = tmp.Replace(andArray[i], "<span style='background-color:yellow'>" + andArray[i] + "</span>");
                    if (tmp == verse.InnerText)
                        match_found = false;
                    else
                        match_found = true;
                }
                //######### Case Insensitive Exact/And/Or
                else if (!chkBoxMatchCase.Checked && searchType == SEARCH_TYPE_EXACT)
                {
                    if (tmp.ToLower().IndexOf(search_term.ToLower()) != -1)
                    {
                        tmp2 = verse.InnerText.Substring(verse.InnerText.ToLower().IndexOf(search_term.ToLower()), search_term.Length);
                        tmp = ReplaceEx(tmp, searchTerm, "<span style='background-color:yellow'>" + tmp2 + "</span>");
                        match_found = true;
                    }
                    else
                        match_found = false;
                }
                else if (!chkBoxMatchCase.Checked && searchType == SEARCH_TYPE_AND)
                {
                    search_term = search_term.Replace("  ", " ");
                    search_term = search_term.Replace("  ", " ");
                    andArray = search_term.Split(' ');
                    found = true;
                    for (int i = 0; i < andArray.Length; i++)
                    {
                        stmp = andArray[i];
                        if (tmp.ToLower().IndexOf(stmp.ToLower()) != -1)
                        {
                            tmp2 = verse.InnerText.Substring(verse.InnerText.ToLower().IndexOf(stmp.ToLower()), stmp.Length);
                            tmp = ReplaceEx(tmp, stmp, "<span style='background-color:yellow'>" + tmp2 + "</span>");
                        }
                        else
                        {
                            found = false;
                            break;
                        }
                        
                    }
                    if (!found)
                        match_found = false;
                    else
                        match_found = true;
                }
                else if (!chkBoxMatchCase.Checked && searchType == SEARCH_TYPE_OR)
                {
                    search_term = searchTerm;
                    search_term = search_term.Replace("  ", " ");
                    search_term = search_term.Replace("  ", " ");
                    andArray = search_term.Split(' ');
                    for (int i = 0; i < andArray.Length; i++)
                    {
                        stmp = andArray[i];
                        if (tmp.ToLower().IndexOf(stmp.ToLower()) != -1)
                        {
                            tmp2 = verse.InnerText.Substring(verse.InnerText.ToLower().IndexOf(stmp.ToLower()), stmp.Length);
                            tmp = ReplaceEx(tmp, stmp, "<span style='background-color:yellow'>" + tmp2 + "</span>");
                        }
                    }
                    if (tmp == verse.InnerText)
                        match_found = false;
                    else
                        match_found = true;
                }                
                

                //====================================================================================================

                if (!match_found) // not found;
                {
                    if (!chkBoxNot.Checked)
                        continue;
                }
                else
                {
                    if (chkBoxNot.Checked)
                        continue;
                }

                found_count++;

                tmp = "<sup><span style='text-color:black;'>" + verse.Attributes["vnumber"].Value + "</span></sup>" + tmp;
                result_verse.Length = 0;
                if (showExtraVerses)
                {
                    result_verse.Append("<font color='gray'>");
                    result_verse.Append(t1);
                    result_verse.Append("</font> ");
                }
                if (verse.ParentNode.ParentNode.Attributes.GetNamedItem("bname") != null)
                    c_book = verse.ParentNode.ParentNode.Attributes.GetNamedItem("bname").InnerText;
                else
                    c_book = Localization.getBookNames()[FormatUtil.zefaniaBookNoToIndex(int.Parse(verse.ParentNode.ParentNode.Attributes.GetNamedItem("bnumber").InnerText))];
                vref = c_book + " " + verse.ParentNode.Attributes.GetNamedItem("cnumber").InnerText + ":" + verse.Attributes.GetNamedItem("vnumber").InnerText;

                result_verse.Append("<a href='$" + vref + "'>" + tmp + "</a>");

                if (showExtraVerses)
                {
                    result_verse.Append(" <font color='gray'>");
                    result_verse.Append(t2);
                    result_verse.Append("</font>");
                }
                if (results.ContainsKey(c_book))
                {
                    tmp_verses = results[c_book];
                    results.Remove(c_book);
                }
                else
                    tmp_verses = new Dictionary<string, string>();
                tmp_verses.Add(vref, result_verse.ToString());
                results.Add(c_book, tmp_verses);
                ///////////////////////////////////////// for every verse ///////////////////////////////////
            }

            if (found_count > MAX_SEARCH_LIMIT)
            {
                sb.Append("<font color='red'><i>More than " + MAX_SEARCH_LIMIT.ToString() + " search results found. Only the first " + MAX_SEARCH_LIMIT.ToString() + " matches are displayed. Please refine your search with additional criteria</i></font>");
            }

            sb.Append("<ul class='TreeView' id='TreeView'>");
            int count = 0;
            foreach (string book in results.Keys)
            {
                sb.Append("<li class='Collapsed'>");
                sb.Append("<table border='0' width='80%'><tr><td align='left'>" + book + "</td><td align='right'><i>(" + results[book].Count.ToString() + ")</i></td></tr></table>");
                tmp_verses = results[book];
                foreach (string v_ref in tmp_verses.Keys)
                {
                    count++;
                    sb.Append("<ul><li class='Highlighted'><b>" + v_ref + "</b></li>");
                    sb.Append("<ul><li>" + tmp_verses[v_ref] + "</li></ul>");
                    sb.Append("</ul>");
                    if (count > MAX_SEARCH_LIMIT)
                        break;
                }
                sb.Append("</li>");
                if (count > MAX_SEARCH_LIMIT)
                    break;
            }
            sb.Append("</ul>");

            sb.Append("<script type='text/javascript' language='javascript'>SetupTreeView('TreeView');</script>");


            sb.Append("</body></html>");
            searchResultsHtml = sb.ToString().Replace("@count@", found_count.ToString());
        }

        private void searchBackgroundJob_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchResultsArea.DocumentText = searchResultsHtml;
            sbr.closeProgressStatus();
        }

        private void searchBackgroundJob_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            sbr.setProgressStatus(e.ProgressPercentage, e.ProgressPercentage.ToString() + " %");
        }

        private void chkBoxRegex_CheckedChanged(object sender, EventArgs e)
        {
             groupBox1.Enabled = !chkBoxRegex.Checked;
        }

    }
}
