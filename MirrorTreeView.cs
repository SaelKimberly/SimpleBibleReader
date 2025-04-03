using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;

using System;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;


namespace Simple_Bible_Reader
{
	public class MirrorTreeView : System.Windows.Forms.TreeView
	{
		
		private bool _mirrored = false;
		[Description("Change to the right-to-left layout."), DefaultValue(false), Localizable(true), Category("Appearance"), Browsable(true)]public bool Mirrored
		{
			get
			{
				return _mirrored;
			}
			set
			{
				if (_mirrored != value)
				{
					_mirrored = value;
					base.OnRightToLeftChanged(EventArgs.Empty);
				}
			}
		}
		
		protected override System.Windows.Forms.CreateParams CreateParams
		{
			get
			{
				System.Windows.Forms.CreateParams CP = base.CreateParams;
				if (Mirrored)
				{
					CP.ExStyle = CP.ExStyle | 0x400000;
				}
				return CP;
			}
		}
		
		// Rest of control code here
		
	}
}
