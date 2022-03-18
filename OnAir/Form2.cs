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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Local the current setting - if it exists
            string webcamregkey = ConfigurationManager.AppSettings["webcamregkey"];
            string devicemode = ConfigurationManager.AppSettings["devicemode"];
            string serialdevice = ConfigurationManager.AppSettings["serialdevice"];
            string restdevice = ConfigurationManager.AppSettings["restdevice"];


            comboBox1.Text = devicemode;
            comboBox2.Text = webcamregkey;

            textBox1.Text = serialdevice;
            textBox2.Text = restdevice;

            if (webcamregkey != null)
            {

            }

            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged"))
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                        {

                            
                            string theKey = subKey.ToString();

                            
                            int idx = theKey.LastIndexOf('\\');
                            comboBox2.Items.Add(theKey.Substring(idx+1));
                            Console.WriteLine(theKey.Substring(idx + 1));


                            var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                            if (endTime <= 0)
                            {

                                Console.WriteLine("Webcam app in use is: " + subKey);

                            }
                        }
                    }
                }


            }
            comboBox2.Text = webcamregkey;

            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string devicemode = ConfigurationManager.AppSettings["devicemode"];

            if (comboBox1.SelectedItem == null) return;

            var selectMode = (String)comboBox1.SelectedItem;

            if (selectMode != null)
            {
                // Update the app setting 
                Console.WriteLine(selectMode);
                HelperFunctions.AddOrUpdateAppSettings("devicemode", selectMode);

            }
            // Set the mode of the controller
            HelperFunctions.setDeviceMode();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Console.WriteLine(sender.ToString());
            var port = (TextBox) sender;
            HelperFunctions.AddOrUpdateAppSettings("serialdevice",port.Text );
        }
        

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            Console.WriteLine(sender.ToString());
            var host = (TextBox)sender;
            HelperFunctions.AddOrUpdateAppSettings("restdevice", host.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Try to connect device based on mode
            
            try
            {
                var res = SerialController.OnAirDeviceAsync(DeviceState.Off);
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show("Device Connected OK", "OnAir Device", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception tex)

            {
                Console.WriteLine("Failed");
                Console.WriteLine(tex);
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(tex.ToString(), "OnAir Device", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                

            }
            
            

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string v = ConfigurationManager.AppSettings["webcamregkey"];

            if (comboBox2.SelectedItem == null) return;

            var webcamregkey = (String)comboBox2.SelectedItem;

            if (webcamregkey != null)
            {
                // Update the app setting 
                Console.WriteLine(webcamregkey);
                HelperFunctions.AddOrUpdateAppSettings("webcamregkey", webcamregkey);

            }
        }
    }
}
