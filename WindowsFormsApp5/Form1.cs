﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace WindowsFormsApp5
{

    public partial class frame_home : Form
    {
        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        
        public static int tabCount = 0;
        public static int incrementcount = 0;
        public static int focusedtab = 0;
        public static List<tabbutton> buttonList = new List<tabbutton>();
        public static List<fctb_box> fctbList = new List<fctb_box>();
        public static Bunifu.Framework.UI.BunifuFlatButton counter;
        public static frame_home access_home;

        public frame_home(string filename)

        {
            InitializeComponent();
            InitialiseShadow();
            access_home = this;
            if (filename != null)
                newTab(filename, -1);
            else
                newTab(null, -1);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newTab(null,-1);
        }

        public void newTab(string filename, int tabIndex) {


            String button_text = "untitled";
            String fileLocation = null;

            if (filename != null)
            {
                fileLocation = filename;
                filename = Path.GetFileName(filename);
                if (filename.Length>3 && filename.Substring(filename.Length - 3, 3) == ".py") {
                    filename = filename.Substring(0, filename.Length - 3);
                }
                button_text = filename;

                if (tabIndex != -1)
                {
                    tabbutton temp_button = buttonList[tabIndex];
                    temp_button.openedTab = true;
                    temp_button.Text = button_text;
                    int ind = getFctbBox(temp_button.Target);
                    fctbList[ind].OpenFile(fileLocation);
                    fctbList[ind].BringToFront();
                    temp_button.fileLocation = fileLocation;
                    buttonList[tabIndex] = temp_button;
                    resetColor(buttonList[tabIndex]);
                    return;
                }

            }
            incrementcount++;
            tabCount++;
            button_text = "untitled" + incrementcount;
            String button_name = "tab" + incrementcount;
            String fctb_name = "fctb" + incrementcount;

            fctb_box fctb_box = new fctb_box();
            tabbutton button = new tabbutton();

            if (filename != null)
            {
                fctb_box.OpenFile(fileLocation);
                button_text = filename;
                button.openedTab = true;
            }
            fctb_box.Name = fctb_name;
            fctbList.Add(fctb_box);
            panel_editor.Controls.Add(fctbList[fctbList.Count - 1]);
            fctbList[fctbList.Count - 1].BringToFront();
            
            if (!panel_editor.Controls.Contains(tabpanel))
                panel_editor.Controls.Add(tabpanel);


            button.Text = button_text;
            button.Name = button_name;
            button.Target = fctb_name;
            button.fileLocation = fileLocation;
            button.Location = new Point((tabCount - 1) * 320, 0);
            buttonList.Add(button);
            tabpanel.Controls.Add(buttonList[buttonList.Count - 1]);
            resetColor(buttonList[buttonList.Count - 1]);
            fctbList[fctbList.Count - 1].Select();
        }


        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeTab();
        }

        public void closeTab(bool forced=false,int forcedtab=0) {

            if (forced == true)
                focusedtab = forcedtab;
            Debug.WriteLine("forced tab"+ focusedtab);
            if (tabCount < 1)
                return;
            int ind = getFctbBox(buttonList[focusedtab].Target);

            if (fctbList[ind].IsChanged && (!forced))
                {
                    switch (MessageBox.Show("Do you want save " + buttonList[focusedtab].Text + " ?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
                    {
                        case DialogResult.Yes:
                            if (!Save(buttonList[focusedtab] as tabbutton, fctbList[ind]))
                                return;
                            break;

                        case DialogResult.Cancel:
                            return;
                    }
                }
            buttonList[focusedtab].Dispose();
            tabCount--;
            fctbList[ind].Dispose();
            fctbList.Remove(fctbList[ind]);
            buttonList.RemoveAt(focusedtab);

            if (tabCount > 0)
            {
                if (focusedtab > 0)
                {
                    focusedtab--;
                    ind = getFctbBox(buttonList[focusedtab].Target);
                    resetColor(buttonList[focusedtab]);
                    fctbList[ind].BringToFront();
                    fctbList[ind].Select();
                }
                else if (focusedtab == 0)
                {
                    focusedtab++;
                    
                    ind = getFctbBox(buttonList[0].Target);
                    resetColor(buttonList[0]);
                    fctbList[ind].BringToFront();
                    fctbList[ind].Select();
                }
                resetButtonLocation();
            }
        }

        private bool Save(tabbutton item, fctb_box fctb)
        {
            
            if (item.fileLocation == null)
            {
                sfdMain.FileName = item.Text;
                if (sfdMain.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return false;
                item.fileLocation = sfdMain.FileName;
                string filename = Path.GetFileName(item.fileLocation);
                if (filename.Length > 3 && filename.Substring(filename.Length - 3, 3) == ".py")
                {
                    filename = filename.Substring(0, filename.Length - 3);
                }
                item.Text = filename;
            }
                
            try
            {
                // Replace 4 spaces with Tab while saving file
                // File.WriteAllText(item.fileLocation as string, fctb.Text.Replace("    ","\t"));
                File.WriteAllText(item.fileLocation as string,fctb.Text);
                fctb.IsChanged = false;
                item.openedTab = true;
            }
            catch (Exception ex)
            {
                item.fileLocation = null;
                item.Text = "Untitled";
                if (MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    return Save(item, fctb);
                else
                    return false;
            }
            return true;
        }

        public void open(string filename)
        {
            if (filename == null)
            {
                Debug.WriteLine("Oops! This should not happen. Error At open function");
                return; }

            int tabIndex = 0;
            //  check each tab for empty fctb
            foreach (tabbutton button in buttonList)
            {
                if (button.openedTab == true)
                {
                    tabIndex++;
                    continue;
                }
                int ind = getFctbBox(button.Target);
                if (fctbList[ind].Text == "")
                {
                    break;
                }
                tabIndex++;
            }
            // open file on empty and non- opened fctb
            if (tabIndex == buttonList.Count)
                tabIndex = -1;
            // Debug.WriteLine(tabIndex);
            newTab(filename, tabIndex);
        }

        private void resetColor(tabbutton newbutton)
        {
            int count = -1;
            // Reset Color of tab
            foreach (tabbutton item in WindowsFormsApp5.frame_home.buttonList)
            {
                count = count + 1;
                if (item.Name == newbutton.Name)
                {
                    newbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(216)))), ((int)(((byte)(107))))); ;
                    newbutton.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(216)))), ((int)(((byte)(107)))));
                    frame_home.focusedtab = count;
                    continue;
                }
                item.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52))))); ;
                item.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            }
        }

        public void resetButtonLocation()
        {
            int diff = 0;
            foreach (tabbutton item in buttonList)
            {
                int x =item.Location.X;
                if (x !=diff)
               Invoke((MethodInvoker)delegate{item.Location = new Point(x - 320, 0); });
                diff = diff + 320;
            }
        }
        public Point getWindowLocation()
        {
            return Location;

        }
        public Point getWindowSize()
        {
            return new Point(Width, Height);
        }

        public void swapButtonLocation(int original, int changed)
        {
            if (original == changed)
                return;
         

            if (original - changed > 0)
            {
                int count = -1;
                foreach (tabbutton item in buttonList)
                {
                    count++;
                    if (count >= changed)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            item.Location = new Point(item.Location.X + 320, 0);
                        });
                    }
                }
                Invoke((MethodInvoker)delegate
                {
                    buttonList[original].Location = new Point(changed * 320, 0);
                });
            }
            else {
                int count = -1;
                foreach (tabbutton item in buttonList)
                {
                    count++;
                    if (count >= original && count<=changed)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            item.Location = new Point(item.Location.X - 320, 0);
                        });
                        continue;
                    }
                    Invoke((MethodInvoker)delegate
                    { item.Location = new Point(item.Location.X + 320, 0); });
                }
                Invoke((MethodInvoker)delegate
                {
                    buttonList[original].Location = new Point(changed * 320, 0);
                });

            }
        
        }


        public int getFctbBox(string name)
        {
            int cnt = -1;
            foreach (fctb_box item in fctbList)
            {cnt++;

            if (item.Name == name)
               return cnt;
            }
            Debug.WriteLine("This Error should not come. Check fctb string Name");
            return 0;

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(buttonList[focusedtab],fctbList[getFctbBox(buttonList[focusedtab].Target)]);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdMain.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                open(ofdMain.FileName);               
            EmptyWorkingSet(Process.GetCurrentProcess().Handle);
        }


        public void newWindowChange(string buttonname,string location)
        {
            new Thread(()=>threadFun(buttonname, location)).Start();
        }

        private void threadFun(string buttonname,string location)
        {
            while (true)
            {
                if (buttonList.Count > focusedtab &&IsHandleCreated)
                {
                    BeginInvoke((MethodInvoker)delegate
                    {
                        buttonList[focusedtab].Text = buttonname;
                        buttonList[focusedtab].fileLocation = location;
                        
                    });
                    break;
                }
                Thread.Sleep(200);
            }

        }



        ///////////////////////////////////////////////////////////////////////////
        ///       IMPORTANT: DEFINES MULTIPROCESS PROGRAM BEHAVIOR              ///
        ///////////////////////////////////////////////////////////////////////////
       

        private void frame_home_Load(object sender, EventArgs e)
        {
            NativeMethods.CHANGEFILTERSTRUCT changeFilter = new NativeMethods.CHANGEFILTERSTRUCT();
            changeFilter.size = (uint)Marshal.SizeOf(changeFilter);
            changeFilter.info = 0;
            if (!NativeMethods.ChangeWindowMessageFilterEx
            (this.Handle, NativeMethods.WM_COPYDATA,
            NativeMethods.ChangeWindowMessageFilterExAction.Allow, ref changeFilter))
            {
                int error = Marshal.GetLastWin32Error();
                MessageBox.Show(String.Format("The error {0} occurred.", error));
            }
        }

        private void tabpanel_DragEnter(object sender, DragEventArgs e)
        {
            
            Debug.WriteLine(e.Data.GetType());
            if (e.Data.GetData(typeof(tabbutton)).GetType().Name== "tabbutton")
            {
                Debug.WriteLine("drag-2");
                e.Effect = DragDropEffects.All; }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

    }

    }
