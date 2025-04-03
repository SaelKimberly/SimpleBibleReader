using DarkUI.Config;
using DarkUI.Forms;
using Microsoft.Web.WebView2.WinForms;
using RtfPipe.Tokens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simple_Bible_Reader
{
    public class Themes
    {
        static Dictionary<string, Dictionary<string, string>> mThemeDict = null;

        public static DarkRenderer darkRenderer = null;
        //
        public enum ThemeCSS
        {
            UI,
            Main,
            Commentary,
            Dictionary,
            Search
        }


        public static void initThemes()
        {
            if (mThemeDict == null)
            {
                List<string> mThemes = new List<string>();
                List<string> mFiles = new List<string>();
                mThemes = Directory.EnumerateDirectories("Themes").ToList();
                mThemeDict = new Dictionary<string, Dictionary<string, string>>();
                foreach (string theme in mThemes)
                {
                    mFiles = Directory.EnumerateFiles(theme).ToList();
                    Dictionary<string, string> mFilesDict = new Dictionary<string, string>();
                    foreach (string file in mFiles)
                    {
                        mFilesDict.Add(Path.GetFileName(file), File.ReadAllText(file));
                    }
                    mThemeDict.Add(Path.GetFileName(theme), mFilesDict);
                }
            }
        }

        public static string[] getThemes()
        {
            if (mThemeDict == null)
                initThemes();
            return mThemeDict.Keys.ToArray();
        }

        public static string getCSS(string theme, ThemeCSS cssInput)
        {
            string cssString = "";

            if (mThemeDict == null)
                initThemes();

            if (mThemeDict.ContainsKey(theme))
            {

                switch (cssInput)
                {
                    case ThemeCSS.UI:
                        if (mThemeDict[theme].ContainsKey("UI.css"))
                            cssString = mThemeDict[theme]["UI.css"];
                        break;
                    case ThemeCSS.Main:
                        if (mThemeDict[theme].ContainsKey("Main.css"))
                            cssString = mThemeDict[theme]["Main.css"];
                        break;
                    case ThemeCSS.Commentary:
                        if (mThemeDict[theme].ContainsKey("Commentary.css"))
                            cssString = mThemeDict[theme]["Commentary.css"];
                        break;
                    case ThemeCSS.Dictionary:
                        if (mThemeDict[theme].ContainsKey("Dictionary.css"))
                            cssString = mThemeDict[theme]["Dictionary.css"];
                        break;
                    case ThemeCSS.Search:
                        if (mThemeDict[theme].ContainsKey("Search.css"))
                            cssString = mThemeDict[theme]["Search.css"];
                        break;
                    default:
                        cssString = "";
                        break;
                }

                //
                cssString = cssString.Replace("@FontFamilyBIB@", GlobalMemory.getInstance().FontFamilyBIB);
                cssString = cssString.Replace("@FontFamilyCMT@", GlobalMemory.getInstance().FontFamilyCMT);
                cssString = cssString.Replace("@FontFamilyDICT@", GlobalMemory.getInstance().FontFamilyCMT);
            }
            return "<style>" + cssString + "</style>";
        }

        public static string CSS_MAIN
        {
            get
            {
                return getCSS(GlobalMemory.getInstance().SelectedTheme, ThemeCSS.Main);
            }
        }
        public static string CSS_COMMENTARY
        {
            get
            {
                return getCSS(GlobalMemory.getInstance().SelectedTheme, ThemeCSS.Commentary);
            }
        }

        public static string CSS_DICTIONARY
        {
            get
            {
                return getCSS(GlobalMemory.getInstance().SelectedTheme, ThemeCSS.Dictionary);
            }
        }

        public static string CSS_SEARCH
        {
            get
            {
                return getCSS(GlobalMemory.getInstance().SelectedTheme, ThemeCSS.Search);
            }
        }


        public static string CSS_UI
        {
            get
            {
                return getCSS(GlobalMemory.getInstance().SelectedTheme, ThemeCSS.UI);
            }
        }

        public static void ChangeDarkMode(Control.ControlCollection container)
        {
            foreach (Control component in container)
            {
                component.BackColor = Colors.GreyBackground;
                component.ForeColor = Colors.LightText;

                if (component is Button) // for specific
                    ((Button)component).FlatStyle = FlatStyle.Flat;
                if (component is ComboBox)           
                    ((ComboBox)component).FlatStyle = FlatStyle.Flat;
                if (component is CheckBox)
                    ((CheckBox)component).FlatStyle = FlatStyle.Flat;
                if (component is RadioButton)
                    ((RadioButton)component).FlatStyle = FlatStyle.Flat;
                
                if (component is DataGridView)
                {
                    ((DataGridView)component).BackgroundColor= Colors.GreyBackground;
                    ((DataGridView)component).ForeColor = Colors.LightText;

                    DataGridViewCellStyle dgvdark_style = new DataGridViewCellStyle()
                    {
                        BackColor = Colors.GreyBackground,
                        ForeColor = Colors.LightText,
                    };

                    ((DataGridView)component).DefaultCellStyle = dgvdark_style;
                    ((DataGridView)component).EnableHeadersVisualStyles = false;
                    ((DataGridView)component).ColumnHeadersBorderStyle=DataGridViewHeaderBorderStyle.Single;
                    ((DataGridView)component).RowHeadersDefaultCellStyle = dgvdark_style;
                    ((DataGridView)component).ColumnHeadersDefaultCellStyle = dgvdark_style;

                    foreach (DataGridViewColumn col in ((DataGridView)component).Columns)
                    {                        
                        if(col is DataGridViewCheckBoxColumn)
                            ((DataGridViewCheckBoxColumn)col).FlatStyle= FlatStyle.Flat;
                        if(col is DataGridViewButtonColumn)
                            ((DataGridViewButtonColumn)col).FlatStyle = FlatStyle.Flat;
                    }
                }
                if (component.Controls.Count > 0)
                    ChangeDarkMode(component.Controls);                
            }
        }

        public static void ChangeMenuDarkMode(ToolStripItemCollection menuItems)
        {
            foreach (ToolStripItem component in menuItems)
            {
                if (component is ToolStripMenuItem)
                {
                    component.BackColor = Colors.GreyBackground;
                    component.ForeColor = Colors.LightText;
                    if (((ToolStripMenuItem)component).DropDownItems.Count > 0)
                        ChangeMenuDarkMode(((ToolStripMenuItem)component).DropDownItems);
                }
                else if (component is ToolStripSeparator)
                {
                    component.BackColor = Colors.GreyBackground;
                    component.ForeColor = Colors.LightText;
                }
            }
        }

        public static void ChangeMenuNormalMode(ToolStripItemCollection menuItems)
        {
            foreach (ToolStripItem component in menuItems)
            {
                if (component is ToolStripMenuItem)
                {                    
                    component.BackColor = Control.DefaultBackColor;
                    component.ForeColor = Control.DefaultForeColor;
                    if (((ToolStripMenuItem)component).DropDownItems.Count > 0)
                        ChangeMenuNormalMode(((ToolStripMenuItem)component).DropDownItems);
                }
                else if (component is ToolStripSeparator)
                {
                    component.BackColor = Control.DefaultBackColor;
                    component.ForeColor = Control.DefaultForeColor;
                }
            }
        }

        public static void ChangeNormalMode(Control.ControlCollection container)
        {
            foreach (Control component in container)
            {
                if (component is Button) // for specific
                {
                    component.BackColor = Control.DefaultBackColor;
                    component.ForeColor = Control.DefaultForeColor;
                }
                else
                {
                    component.BackColor = Control.DefaultBackColor;
                    component.ForeColor = Control.DefaultForeColor;
                    if (component.Controls.Count > 0)
                        ChangeNormalMode(component.Controls);
                }
            }
        }

        public static DarkRenderer getDarkRender()
        {
            if (darkRenderer == null)
                darkRenderer = new DarkRenderer();
            return darkRenderer;
        }


        public class DarkRenderer : ToolStripProfessionalRenderer
        {
            public DarkRenderer() : base() { }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                Rectangle r = e.Item.ContentRectangle;
                if (e.Item.Selected)
                {
                    e.Item.ForeColor= Color.Black;
                    using (Brush b = new SolidBrush(Color.LightGray))
                    {
                        e.Graphics.FillRectangle(b, r);
                    }                    
                }
                else
                {
                    e.Item.ForeColor = Color.LightGray;
                    using (Brush b = new SolidBrush(Color.Black))
                    {
                        e.Graphics.FillRectangle(b, r);
                    }
                    e.Item.BackColor = Color.Black;
                }
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                Rectangle r = e.Item.ContentRectangle;
                using (Brush b = new SolidBrush(Color.LightGray))
                {
                    e.Graphics.FillRectangle(b, r);
                }
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                e.ToolStrip.BackColor = Color.Black;
            }
        }

        public static DialogResult MessageBox(string message)
        {
            DialogResult result = DialogResult.Cancel;
            if (GlobalMemory.getInstance().SelectedTheme.ToLower().Contains("dark"))
                result = DarkMessageBox.ShowInformation(message, "Information", DarkDialogButton.Ok);
            else
                result = System.Windows.Forms.MessageBox.Show(message);
            return result;
        }

        public static DialogResult MessageBox(string message, string title)
        {
            DialogResult result = DialogResult.Cancel;
            if (GlobalMemory.getInstance().SelectedTheme.ToLower().Contains("dark"))
                result = DarkMessageBox.ShowInformation(message, title, DarkDialogButton.Ok);
            else
                result = System.Windows.Forms.MessageBox.Show(message, title);
            return result;
        }

        public static DialogResult MessageBox(string message, string title, MessageBoxButtons buttons)
        {
            DialogResult result = DialogResult.Cancel;
            if (GlobalMemory.getInstance().SelectedTheme.ToLower().Contains("dark"))
            {
                DarkDialogButton button = DarkDialogButton.Ok;
                switch (buttons)
                {
                    case MessageBoxButtons.OK:
                        button = DarkDialogButton.Ok;
                        break;
                    case MessageBoxButtons.OKCancel:
                        button = DarkDialogButton.OkCancel;
                        break;
                    case MessageBoxButtons.AbortRetryIgnore:
                        button = DarkDialogButton.AbortRetryIgnore;
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        button = DarkDialogButton.YesNoCancel;
                        break;
                    case MessageBoxButtons.YesNo:
                        button = DarkDialogButton.YesNo;
                        break;
                    case MessageBoxButtons.RetryCancel:
                        button = DarkDialogButton.RetryCancel;
                        break;
                    default:
                        button = DarkDialogButton.Ok;
                        break;
                }
                result= DarkMessageBox.ShowInformation(message, title, button);
            }
            else
                result = System.Windows.Forms.MessageBox.Show(message, title, buttons);
            return result;
        }

        public static DialogResult MessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            DialogResult result = DialogResult.Cancel;
            if (GlobalMemory.getInstance().SelectedTheme.ToLower().Contains("dark"))
            {
                DarkDialogButton button = DarkDialogButton.Ok;
                switch (buttons)
                {
                    case MessageBoxButtons.OK:
                        button = DarkDialogButton.Ok;
                        break;
                    case MessageBoxButtons.OKCancel:
                        button = DarkDialogButton.OkCancel;
                        break;
                    case MessageBoxButtons.AbortRetryIgnore:
                        button = DarkDialogButton.AbortRetryIgnore;
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        button = DarkDialogButton.YesNoCancel;
                        break;
                    case MessageBoxButtons.YesNo:
                        button = DarkDialogButton.YesNo;
                        break;
                    case MessageBoxButtons.RetryCancel:
                        button = DarkDialogButton.RetryCancel;
                        break;
                    default:
                        button = DarkDialogButton.Ok;
                        break;
                }
                //                
                switch (icon)
                {
                    case MessageBoxIcon.None:
                        result = DarkMessageBox.ShowInformation(message, title, button);
                        break;
                    case MessageBoxIcon.Error:
                        result = DarkMessageBox.ShowError(message, title, button);
                        break;
                    case MessageBoxIcon.Warning:
                        result = DarkMessageBox.ShowWarning(message, title, button);
                        break;
                    case MessageBoxIcon.Information:
                        result = DarkMessageBox.ShowInformation(message, title, button);
                        break;
                    case MessageBoxIcon.Question:
                        result = DarkMessageBox.ShowInformation(message, title, button);
                        break;
                    default:
                        result = DarkMessageBox.ShowInformation(message, title, button);
                        break;
                }
            }
            else
                result = System.Windows.Forms.MessageBox.Show(message, title, buttons, icon);
            return result;
        }
    }
}
