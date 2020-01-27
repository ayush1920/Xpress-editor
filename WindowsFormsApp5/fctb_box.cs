using WindowsFormsApp5;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

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
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.fctb_box_DragOver);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.fctb_box_DragDrop);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();



        }
        // override for shortcut ctrl+ T
        protected override void newTab()
        {
            base.newTab();
            frame_home.access_home.newTab(null, -1);
        }
        // override for shortcut ctrl+ W
        protected override void closeTab()
        {
            base.closeTab();
            frame_home.access_home.closeTab();
        }
        // override for shortcut Ctrl+O
        protected override void openFile()
        {
            base.openFile();
            frame_home.access_home.openFile();
        }
        // override for shortcut Ctrl+S
        protected override void saveFile()
        {
            base.saveFile();
            frame_home.access_home.saveFile();
        }
        // override for shortcut Ctrl+Q
        protected override void exit()
        {
            base.exit();
            frame_home.access_home.Exit();
        }
        // override for shortcut Ctrl+Alt+ Left
        protected override void jumpTabLeft()
        {
            base.jumpTabLeft();
            frame_home.access_home.jumptabLeft();

        }
        // overide for shortcut Ctrl+Alt+ Right
        protected override void jumpTabRight()
        {
            base.jumpTabRight();
            frame_home.access_home.jumptabRight();
        }


        private void fctb_box_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void fctb_box_DragDrop(object sender, DragEventArgs e)
        {  if (e.Data.GetDataPresent("tabdata"))
            {
                e.Effect = DragDropEffects.Move;
                string data = (string)e.Data.GetData("tabdata");
                String[] spearator = { "$" };
                Int32 count = 3;
                // using the method 
                String[] strlist = data.Split(spearator, count,
                       StringSplitOptions.RemoveEmptyEntries);
                if (strlist[0] == Program.guid || int.Parse(strlist[1]) == 1)
                    return;
                else
                {/*
                    data communication format - 
                    own ID :
                    send ID :
                    transaction type :
                    data if available :
                    */
                    string com_data = Program.guid + "$" + strlist[0] + "$" + NativeMethods.DRAG_RECEIVED + "$" + strlist[2];
                    Debug.WriteLine("send data-" + com_data);
                    Process[] processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

                    foreach (Process p in processes)
                    {
                        if (p.Id.ToString() == strlist[0])
                        {
                            IntPtr windowHandle = p.MainWindowHandle;

                            NativeMethods.communicate(com_data,windowHandle, NativeMethods.WM_COPYDATA);
                            break;
                        }
                    }
                }
            }
            else
                e.Effect = DragDropEffects.None;

        }

    }
}