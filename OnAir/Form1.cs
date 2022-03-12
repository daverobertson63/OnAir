﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemTray
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //if the form is minimized
            //hide it from the task bar
            //and show the system tray icon (represented by the NotifyIcon control)
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(1000);
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool result = IsWebCamInUse();

            if (result)
            {
                pictureBox1.Visible = true;

            }
            else
            {
                pictureBox1.Visible = false;
            }

            Console.WriteLine(result);
        }
        private static bool IsWebCamInUse()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged"))
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                        {
                            //Console.WriteLine(subKey);
                            var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                            if (endTime <= 0)
                            {

                                Console.WriteLine("Webcam app in use is: " + subKey);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("Timer - check status of WebCam");

            bool result = IsWebCamInUse();

            if (result)
            {
                pictureBox1.Visible = true;
                this.TopMost = true;

            }
            else
            {
                pictureBox1.Visible = false;
                this.TopMost = false;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

}
