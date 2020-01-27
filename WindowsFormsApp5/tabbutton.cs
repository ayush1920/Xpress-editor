using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Bunifu.Framework.UI;
using WindowsFormsApp5;

namespace WindowsFormsApp5
{


    public partial class tabbutton : BunifuFlatButton
    {

        public static int movedtab = 0;
        static int initialtab = 0;
        static bool move = false;
        public string Target=null; // target of fctb attached to the button
        public string fileLocation=null; // location of saved file
        public bool openedTab = false;

        public tabbutton()
        {
            string target = Target;
            string filelocation = fileLocation;
            bool openedtab = openedTab;
            this.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(54)))), ((int)(((byte)(63)))));
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BorderRadius = 0;
            this.ButtonText = "";
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DisabledColor = System.Drawing.Color.Gray;
            this.Iconcolor = System.Drawing.Color.Transparent;
            this.Iconimage = null;
            this.Location = new System.Drawing.Point(0, 0);
            this.Iconimage_right = null;
            this.Iconimage_right_Selected = null;
            this.Iconimage_Selected = null;
            this.IconMarginLeft = 0;
            this.IconMarginRight = 0;
            this.IconRightVisible = true;
            this.IconRightZoom = 0D;
            this.IconVisible = true;
            this.IconZoom = 90D;
            this.IsTab = false;
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.Name = "";
            this.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            this.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(43)))));
            this.OnHoverTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(39)))), ((int)(((byte)(255)))));
            this.selected = false;
            this.Size = new System.Drawing.Size(320, 35);
            this.TabIndex = 4;
            this.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Textcolor = System.Drawing.Color.White;
            this.TextFont = new System.Drawing.Font("Georgia", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Click += new System.EventHandler(this.bunifuFlatButton_Click);
            this.MouseDown += new System.EventHandler(this.bunifuFlatButton_MouseDown);
            this.MouseUp+= new System.EventHandler(this.bunifuFlatButton_MouseUp);
            // 

        }

        public void bunifuFlatButton_Click(object sender, EventArgs e)
        {
            int count = -1;
            // Reset Color of tab
            foreach (tabbutton item in WindowsFormsApp5.frame_home.buttonList)
            {
                count = count + 1;
                if (item.Name == Name) {
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(216)))), ((int)(((byte)(107))))); ;
                    Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(216)))), ((int)(((byte)(107)))));
                    frame_home.focusedtab = count;
                    continue;
                }
                item.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52))))); ;
                item.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
            }
        
        }

        private void bunifuFlatButton_MouseDown(object sender, EventArgs e)
        {

            move = true;
            // get initial tab
            int count = -1;
            foreach (tabbutton item in frame_home.buttonList)
            {
                count = count + 1;
                if (item.Name == Name)
                {initialtab = count;
                    break;
                }
            }
            movedtab = initialtab;

            new Thread(new ThreadStart(test)).Start();

            DataObject data = new DataObject();
            string fileLocation = frame_home.buttonList[initialtab].fileLocation;
            if (fileLocation == null)
                fileLocation = "null";
            string tabname = frame_home.buttonList[initialtab].Text;
            string garbagefile = Path.Combine(Path.GetTempPath(), "Xpress_editorGarbage.txt");
            data.SetData("tabdata", Program.guid + "$" + frame_home.tabCount+"$"+initialtab);
            DoDragDrop(data, DragDropEffects.Move);
            move = false;
            frame_home.focusedtab = movedtab;
        }

        private void bunifuFlatButton_MouseUp(object sender, EventArgs e)
        {
            move = false;
        }

        private void test()
        {
            while (move)
            { this.Invoke((MethodInvoker)delegate
                 {

                     int cnt = -1;
                     foreach (tabbutton item in frame_home.buttonList)
                     {
                         cnt++;
                        
                         // best way to access non static method
                         // make a static variable of class1;
                         // in main class type  variable = this
                         // access the static variable from other class
                         Point pt = frame_home.access_home.PointToClient(Cursor.Position);
                         // Individual window work only for top and left
                         if (pt.X>0 && pt.Y >0 && pt.X< frame_home.access_home.getWindowSize().X && pt.Y < frame_home.access_home.getWindowSize().Y)
                         {
                             if (Decimal.ToInt16((MousePosition.X - frame_home.access_home.getWindowLocation().X) / 320) == cnt)
                             {movedtab = cnt;
                                 if (initialtab != cnt)
                                 {
                                     item.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(173)))), ((int)(((byte)(107))))); ;
                                     item.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(173)))), ((int)(((byte)(107)))));
                                 }
                                 continue;
                             }
                             
                         }
                         else
                           movedtab = -1;
                            item.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52))))); ;
                            item.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
                     }
                 });

                Thread.Sleep(100);
            }

            // open cnt tab;

            if (movedtab == -1 && frame_home.buttonList.Count>1)
            {
                string tempPath = Path.GetTempPath();
                int ind = frame_home.access_home.getFctbBox(frame_home.buttonList[initialtab].Target);
                tempPath = Path.Combine(tempPath, "Xpress_editorGarbage.txt");
                File.WriteAllText(tempPath,frame_home.fctbList[ind].Text);
                
                // If Within .2 sec no message comes Proceed to open it as new window.
                Thread.Sleep(200);
                if (frame_home.TabCloseTrigger == true)
                {while (frame_home.TabClosedStarted)
                        Thread.Sleep(100);
                    // force close without saving
                    Invoke((MethodInvoker)delegate
                    {
                        frame_home.access_home.closeTab(true, initialtab);
                    });

                    frame_home.TabCloseTrigger = false;

                    return;
                }


                // Start tab as a new Window
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Path.Combine(System.Reflection.Assembly.GetEntryAssembly().Location);
                // Data Location, Name, saved location
                string location = frame_home.buttonList[initialtab].fileLocation;
                if (location == null)
                    location = "null";
                startInfo.Arguments = "\""+tempPath+"\" \""+ frame_home.buttonList[initialtab].Text+"\" \"" + location+"\" \""+ 
                    Convert.ToInt32(frame_home.fctbList[frame_home.access_home.getFctbBox(frame_home.buttonList[initialtab].Target)].IsChanged)+"\"";
                Process.Start(startInfo);
                Invoke((MethodInvoker)delegate
                {
                    frame_home.access_home.closeTab(true,initialtab);
                });
                
                return;
            }
            if (movedtab == -1)
                movedtab = initialtab;

            // Adjust tab order
            frame_home.access_home.swapButtonLocation(initialtab, movedtab);
            // make a copy of initialtab at movedtab
            tabbutton tmp = frame_home.buttonList[initialtab];
            
            if (initialtab - movedtab < 0)
            frame_home.buttonList.Insert(movedtab + 1, tmp);

            else
            frame_home.buttonList.Insert(movedtab, tmp);

            // remove copied tab
            if (initialtab - movedtab < 0)
            frame_home.buttonList.RemoveAt(initialtab);

            else
            frame_home.buttonList.RemoveAt(initialtab + 1);
            

            // reset spaces
            frame_home.access_home.resetButtonLocation();
            int cnt1 = -1;
            // reset color
            foreach (tabbutton item in WindowsFormsApp5.frame_home.buttonList)
            { cnt1++;
                item.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52))))); ;
                item.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(52)))));
             }

            // highlight moved tab color
            frame_home.buttonList[movedtab].BackColor = Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(216)))), ((int)(((byte)(107))))); ;
            frame_home.buttonList[movedtab].Normalcolor = Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(216)))), ((int)(((byte)(107)))));
            Invoke((MethodInvoker)delegate
            {
                frame_home.fctbList[frame_home.access_home.getFctbBox(frame_home.buttonList[movedtab].Target)].BringToFront();
            });
            
        }

      private void dragEnter(object sender, EventArgs e)
        {
            Debug.WriteLine("mouse Drag");
        }
    }
}