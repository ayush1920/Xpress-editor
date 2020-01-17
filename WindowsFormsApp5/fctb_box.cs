using System.Diagnostics;
using WindowsFormsApp5;

namespace WindowsFormsApp5
{


    public partial class fctb_box : FastColoredTextBoxNS.FastColoredTextBox
    {
        public static frame_home access_home;


        public fctb_box()
        {
            this.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*" +
    "(?<range>:)\\s*(?<range>[^;]+);";
            this.AutoIndentExistingLines = false;
            this.AutoScrollMinSize = new System.Drawing.Size(38, 19);
            this.BackBrush = null;
           // this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CharHeight = 19;
            this.CharWidth = 9;
            this.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.DelayedEventsInterval = 500;// changed
            this.DelayedTextChangedInterval = 1000;// chagned
            this.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FoldingIndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(54)))), ((int)(((byte)(63)))));
            this.Font = new System.Drawing.Font("Consolas", 9.75F);
            //this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.IsReplaceMode = false;
            this.Location = new System.Drawing.Point(0, 28);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "";
            this.Paddings = new System.Windows.Forms.Padding(0);
            this.ReservedCountOfLineNumberChars = 2;
            this.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.ServiceColors = null;
            this.Size = new System.Drawing.Size(1074, 631);
            this.TabIndex = 3;
            this.Zoom = 100;
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();



        }

        protected override void newTab()
        {
            base.newTab();
            frame_home.access_home.newTab(null,-1);
        }
        protected override void closeTab()
        {
            base.closeTab();
            frame_home.access_home.closeTab();
        }


    }
}