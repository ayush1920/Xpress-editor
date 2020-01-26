using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastColoredTextBoxNS;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WindowsFormsApp5
{
    public partial class  frame_home : Form
    {

        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        static private bool m_aeroEnabled;

        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]

        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
            );

        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();
                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW; return cp;
            }
        }
        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0; DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }


        protected override void WndProc(ref Message m)
        {
            if (m_aeroEnabled)
            {
                var v = 2;
                DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                MARGINS margins = new MARGINS()
                {
                    bottomHeight = 1,
                    leftWidth = 0,
                    rightWidth = 0,
                    topHeight = 0
                }; DwmExtendFrameIntoClientArea(this.Handle, ref margins);


            }

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT) m.Result = (IntPtr)HTCAPTION;



            ////////////////////////////////////////////////////////////////////////////////
            ///       IMPORTANT: THIS MESSAGE CODE IS FOR MUTEX USED IN PROGRAM.CS       ///
            ///           DELETING THIS WILL CHANGE HOW MULTIPROCESS IS HANDLED          ///
            ////////////////////////////////////////////////////////////////////////////////
            


            else if (m.Msg == NativeMethods.WM_COPYDATA)
            {   // Extract the file name
                NativeMethods.COPYDATASTRUCT copyData =
                (NativeMethods.COPYDATASTRUCT)Marshal.PtrToStructure
                (m.LParam, typeof(NativeMethods.COPYDATASTRUCT));
                int dataType = (int)copyData.dwData;
                if (dataType == 2)
                {
                    string fileName = Marshal.PtrToStringAnsi(copyData.lpData);
                    if (fileName != null)
                    {
                        open(fileName);
                    }
                    BringWindowToFront();

                }



                else if (dataType == 3)
                {
                    string tabdata = Marshal.PtrToStringAnsi(copyData.lpData);
                    if (tabdata != null)
                    {

                        String[] spearator = { "$" };
                        Int32 count = 10;
                        String[] strlist = tabdata.Split(spearator, count,
                      StringSplitOptions.RemoveEmptyEntries);

                        if (strlist[1] == Program.guid.ToString())
                        {
                            if (strlist[2] == NativeMethods.DRAG_RECEIVED)
                            {
                                // Write file
                                TabClosedStarted = true;
                                TabCloseTrigger = true;
                                string tempPath = Path.GetTempPath();


                                tabbutton btn = frame_home.buttonList[int.Parse(strlist[3])];
                                int ind = frame_home.access_home.getFctbBox(btn.Target);
                                tempPath = Path.Combine(tempPath, "Xpress_editorGarbage.txt");
                                File.WriteAllText(tempPath, frame_home.fctbList[ind].Text);
                                TabClosedStarted = false;

                                // Send data to open file to pid
                                string location = btn.fileLocation;
                                if (location == null)
                                    location = "null";
                                string com_data = Program.guid + "$" + strlist[0] + "$" + NativeMethods.DRAG_FINISH + "$" +
                                Path.Combine(Path.GetTempPath(), "Xpress_editorGarbage.txt") + "$" + btn.Text + "$" + location;
                                Process[] processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
                                foreach (Process p in processes)
                                {
                                    if (p.Id.ToString() == strlist[0])
                                    {
                                        IntPtr windowHandle = p.MainWindowHandle;
                                        NativeMethods.communicate(com_data, windowHandle, NativeMethods.WM_COPYDATA);
                                    }
                                }

                            }
                            else if (strlist[2] == NativeMethods.DRAG_FINISH)
                            {
                                string guid = strlist[0];
                                string grbg_path = strlist[3];
                                string name = strlist[4];
                                string location = strlist[5];
                                if (location == "null")
                                    location = null;
                                int old = buttonList.Count;
                                new Thread(() => threadFun(name,location)).Start();
                                Thread.Sleep(100);
                                newTab(grbg_path, -1);
                            }
                        }



                    }
                }
                else
                {
                    MessageBox.Show(String.Format("Unrecognized data type = {0}.",
                    dataType), "SendMessageDemo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (m.Msg == Program.WM_ACTIVATEAPP)
            {
                // Maximizes and bring it to front.
                BringWindowToFront();
              
            }
            // END
            base.WndProc(ref m);
        }

        // Maximizes and bring it to front
        private void BringWindowToFront()
        {
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }
         
        static public void InitialiseShadow()
        {
            m_aeroEnabled = false;
        }

        static private void EnableResize(frame_home frame)
        {
            frame.SetStyle(ControlStyles.ResizeRedraw, true);
            
        }
    }
}
