using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    static class Program
    {
        // Multiple Instance Control
        // https://stackoverflow.com/questions/1777668/send-message-to-a-windows-process-not-its-main-window
        // Credits - Matt Davis
        // https://www.codeproject.com/Tips/1017834/How-to-Send-Data-from-One-Process-to-Another-in-Cs
        // Credits - George Jonsson
        // https://stackoverflow.com/questions/1777668/send-message-to-a-windows-process-not-its-main-window
        // Credits - chitza


        #region Dll Imports
        public const int HWND_BROADCAST = 0xFFFF;

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
        #endregion Dll Imports

        public static readonly int WM_ACTIVATEAPP = RegisterWindowMessage("WM_ACTIVATEAPP");

        public static int WM_COPYDATA { get; private set; }
        public static string guid="";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]



        /// args  -  Filepath of garbage file, name of tab, if tab is saved - location of saved file
        static void Main(string[] args)
        { 
            guid = "034c3adc-0056-4167-97e0-772f92d572fa";
            bool createdNew = true;
            if (args.Length == 3)
                guid = Guid.NewGuid().ToString();
            
            //by creating a mutex, the next application instance will detect it
            //and the code will flow through the "else" branch 
            using (Mutex mutex = new Mutex(true, guid, out createdNew))//make sure it's an unique identifier (a GUID would be better)
            {
               
                if (createdNew)
                {
                    string filename;

                    if (args != null && args.Length > 0)
                        filename = args[0];
                    else
                        filename = null;

                   
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    frame_home frame = new frame_home(filename);

                    // check for modifications
                    if (args.Length == 3)
                    {
                        string location = args[2];
                        if (location == "null")
                            location = null;
                        frame.newWindowChange(args[1], location);
                    }
                    Application.Run(frame);
                }
                else
                {
                    //we tried to create a mutex, but there's already one (createdNew = false - another app created it before)
                    //so there's another instance of this application running
                    Process currentProcess = Process.GetCurrentProcess();
                  
                    //get the process that has the same name as the current one but a different ID
                    foreach (Process process in Process.GetProcessesByName(currentProcess.ProcessName))
                    {
                        if (process.Id != currentProcess.Id)
                        {
                            IntPtr handle = process.MainWindowHandle;

                            //if the handle is non-zero then the main window is visible (but maybe somewhere in the background, that's the reason the user started a new instance)
                            //so just bring the window to front
                            if (handle != IntPtr.Zero)
                            {
                                IntPtr ptrCopyData = IntPtr.Zero;
                                try
                                {
                                
                                    // Create the data structure and fill with data
                                    NativeMethods.COPYDATASTRUCT copyData = new NativeMethods.COPYDATASTRUCT();
                                    copyData.dwData = new IntPtr(2);
                                    if (args != null && args.Length > 0)
                                    {
                                        string fileName = args[0];
                                        // Just a number to identify the data type
                                        copyData.cbData = fileName.Length + 1;  // One extra byte for the \0 character
                                        copyData.lpData = Marshal.StringToHGlobalAnsi(fileName);

                                        // Allocate memory for the data and copy
                                        ptrCopyData = Marshal.AllocCoTaskMem(Marshal.SizeOf(copyData));
                                        Marshal.StructureToPtr(copyData, ptrCopyData, false);

                                        // Send the message
                                        NativeMethods.SendMessage(handle, NativeMethods.WM_COPYDATA, IntPtr.Zero, ptrCopyData);
                                    }
                                    else
                                    {
                                        PostMessage((IntPtr)HWND_BROADCAST, WM_ACTIVATEAPP, IntPtr.Zero, IntPtr.Zero);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString(), "SendMessage Demo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                finally
                                {
                                    // Free the allocated memory after the control has been returned
                                    if (ptrCopyData != IntPtr.Zero)
                                        Marshal.FreeCoTaskMem(ptrCopyData);
                                }
                            }
                            else
                                //tough luck, can't activate the window, it's not visible and we can't get its handle
                                //so instead notify the process that it has to show it's window
                                PostMessage((IntPtr)HWND_BROADCAST, WM_ACTIVATEAPP, IntPtr.Zero, IntPtr.Zero);//this message will be sent to MainForm

                            break;
                        }
                    }
                }
            }
        }
    }
}