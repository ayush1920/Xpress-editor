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



            ///////////////////////////////////////////////////////////////////////////
            ///       IMPORTANT: THIS MESSAGE CODE IS FOR MUTEX USED IN PROGRAM.CS  ///
            ///       DELETING THIS WILL CHANGE HOW MULTIPROCESS IS HANDLED         ///
            ///////////////////////////////////////////////////////////////////////////
            


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
                else
                {
                    MessageBox.Show(String.Format("Unrecognized data type = {0}.",
                    dataType), "SendMessageDemo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            else if (m.Msg == Program.WM_ACTIVATEAPP)
            {
                // Maximizes and bring it to front. - Only Main Window
                if (Program.guid== "034c3adc-0056-4167-97e0-772f92d572fa")
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
