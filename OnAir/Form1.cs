using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnAir
{
    
    public partial class Form1 : Form
    {
        DeviceMode deviceMode=DeviceMode.NotConnected;  // State of the device as required by UI and settings
        DeviceState deviceState = DeviceState.Off;  // State of the device as required by UI and settings
        Boolean topMode = false;   
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
            bool result = IsWebCamInUse(SerialController.detectMode);

            if (result)
            {
                pictureBox1.Visible = true;

            }
            else
            {
                pictureBox1.Visible = false;
            }

            Console.WriteLine(result);

            Form2 f2 = new Form2();
            f2.ShowDialog();
        }
        
        private static bool IsWebCamInUse(DetectMode mode)
        {

            //Computer\HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone\NonPackaged

                // We check the setting 
            if (SerialController.detectMode == DetectMode.Manual)
            {
                if (SerialController.displayMode== DeviceState.On ||  SerialController.displayMode == DeviceState.Pulse )
                    return true;
            }
           
            Boolean webcam = false;
            Boolean microphone = false; 
            
            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged"))
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                        {
                            //Console.WriteLine(subKey);
                            Console.WriteLine(subKeyName + " Value: " + subKey.GetValue("LastUsedTimeStop"));
                            var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                            if (endTime <= 0)
                            {

                                Console.WriteLine("Webcam app in use is: " + subKey);
                                webcam = true;
                                
                            }
                        }
                    }
                }
            }

            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone\NonPackaged"))
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                        {
                            //Console.WriteLine(subKey);
                            Console.WriteLine(subKeyName + " Value: " + subKey.GetValue("LastUsedTimeStop"));
                            var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                            if (endTime <= 0)
                            {

                                Console.WriteLine("Webcam app in use is: " + subKey);
                                microphone = true;
                            }
                        }
                    }
                }
            }

            if (SerialController.detectMode == DetectMode.Microphone)
            {
                if (microphone)
                    return true;
            }
            else if (SerialController.detectMode == DetectMode.Camera)
            { 
                if (webcam)
                {
                    return true;
                } 
            }
            else if (SerialController.detectMode == DetectMode.CameraMicrophone)
            {
                if (webcam && microphone)
                {
                    return true;
                }
            }

            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            pictureBox2.Visible = true;
            string displaymode = ConfigurationManager.AppSettings["displaymode"];

            comboBox1.Text = displaymode;

            if (displaymode.Equals("On"))
                deviceState = DeviceState.On;
            if (displaymode.Equals("Off"))
                deviceState = DeviceState.Off;
            if (displaymode.Equals("Pulse"))
                deviceState = DeviceState.Pulse;

                    
            
            HelperFunctions.setDeviceMode();

            string topmode = ConfigurationManager.AppSettings["topmode"];

            // Set the checkbox
            checkBox1.Checked = Convert.ToBoolean(topmode);
            topMode = Convert.ToBoolean(topmode);

            string detectmode = ConfigurationManager.AppSettings["detectmode"];
            comboBox2.Text = detectmode;
            HelperFunctions.setDetectMode();

            //< add key = "topmode" value = "True" />
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("Timer - check status of WebCam");

            string topmode = ConfigurationManager.AppSettings["topmode"];
            topMode = Convert.ToBoolean(topmode);

            timer1.Enabled = false;
            


            try
            {
                bool result = IsWebCamInUse(SerialController.detectMode);


                if (result)
                {

                    pictureBox1.Visible = true;
                    pictureBox2.Visible = false;
                    var res = SerialController.OnAirDeviceAsync(SerialController.displayMode);

                    //this.TopMost = topMode;

                }
                else
                {
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = true;
                    var res = SerialController.OnAirDeviceAsync(DeviceState.Off);
                    
                    //this.TopMost = topMode;
                }
                pictureBox3.Image = OnAir.Properties.Resources.connected;
                timer1.Enabled = true;
            }
            catch (Exception onAirError)
            {
                pictureBox3.Image = OnAir.Properties.Resources.disconnected;
                Console.WriteLine(onAirError);
                timer1.Enabled = true;

            }
            finally
            {
                timer1.Enabled = true;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            string topmode = ConfigurationManager.AppSettings["topmode"];

           var result = Convert.ToBoolean(topmode);

            CheckBox cb = (CheckBox)sender;
                                    

            if (cb != null)
            {
                // Update the app setting 
                Console.WriteLine(cb);
                
                HelperFunctions.AddOrUpdateAppSettings("topmode", cb.Checked.ToString()   );
                                
            }
            this.TopMost = cb.Checked;


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string displaymode = ConfigurationManager.AppSettings["displaymode"];

            if (comboBox1.SelectedItem == null) return;

            var selectMode = (String)comboBox1.SelectedItem;

            if (selectMode != null)
            {
                // Update the app setting 
                Console.WriteLine(selectMode);
                HelperFunctions.AddOrUpdateAppSettings("displaymode", selectMode);
                

            }
            HelperFunctions.setDisplayMode(selectMode);


        }

        private void button2_Click(object sender, EventArgs e)
        {
            var result= SerialController.OnAirDeviceAsync(DeviceState.Reset);    // Reset the device = it will come back on of the web cam is still active
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string detectmode = ConfigurationManager.AppSettings["detectmode"];

            if (comboBox2.SelectedItem == null) return;

            var selectMode = (String)comboBox2.SelectedItem;

            if (selectMode != null)
            {
                // Update the app setting 
                Console.WriteLine(selectMode);
                HelperFunctions.AddOrUpdateAppSettings("detectmode", selectMode);


            }
            HelperFunctions.setDetectMode();
        }
    }

}
