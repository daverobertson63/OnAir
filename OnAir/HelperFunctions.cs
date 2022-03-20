using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnAir
{
    internal class HelperFunctions
    {
        public static void setDeviceMode()
        {
            String devicemode = ConfigurationManager.AppSettings["devicemode"];

            if (devicemode.Equals("Serial Connection"))
            {
                SerialController.deviceMode = DeviceMode.Serial;
            }
            else if (devicemode.Equals("REST with WIFI"))
            {
                SerialController.deviceMode = DeviceMode.REST;
            }
            else
            {
                SerialController.deviceMode = DeviceMode.NotConnected;
            }
        }
        public static void setDetectMode()
        {
            String detectMode = ConfigurationManager.AppSettings["detectmode"];

            if (detectMode.Equals("Manual"))
            {
                SerialController.detectMode = DetectMode.Manual;
            }
            else if (detectMode.Equals("Camera"))
            {
                SerialController.detectMode = DetectMode.Camera;
            }
            else if (detectMode.Equals("Microphone"))
            {
                SerialController.detectMode = DetectMode.Microphone;
            }
            else if (detectMode.Equals("Camera and Microphone"))
            {
                SerialController.detectMode = DetectMode.CameraMicrophone;
            }
                        
        }
        public static void setDisplayMode(String theState)
        {
            

            if (theState.Equals("On"))
            {
                SerialController.displayMode = DeviceState.On;
            }
            else if (theState.Equals("Off"))
            {
                SerialController.displayMode = DeviceState.Off;
            }
            else if (theState.Equals("Pulse"))
            {
                SerialController.displayMode = DeviceState.Pulse;
            }
            
        }

        public static void AddOrUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
    // https://stackoverflow.com/questions/630803/associating-enums-with-strings-in-c-sharp
    public class LogCategory
    {
        private LogCategory(string value) { Value = value; }

        public string Value { get; private set; }

        public static LogCategory Trace { get { return new LogCategory("Trace"); } }
        public static LogCategory Debug { get { return new LogCategory("Debug"); } }
        public static LogCategory Info { get { return new LogCategory("Info"); } }
        public static LogCategory Warning { get { return new LogCategory("Warning"); } }
        public static LogCategory Error { get { return new LogCategory("Error"); } }
    }

    // The device state is what it can do - off, normal or pulsiing.
    // I also added a reset whihc allows the device to be rest to a known state of off
    // This can be used to drive the controller and also to save the state you want to work at
    public class DeviceStateClass

    {
        private DeviceStateClass(string value) { Value = value; }

        public string Value { get; private set; }

        public static DeviceStateClass Off { get { return new DeviceStateClass("Off"); } }
        public static DeviceStateClass On { get { return new DeviceStateClass("On"); } }
        public static DeviceStateClass Pulse { get { return new DeviceStateClass("Pulse"); } }
        public static DeviceStateClass Reset { get { return new DeviceStateClass("Reset"); } }

    }

    public class DeviceModeClass

    {
        private DeviceModeClass(string value) { Value = value; }

        public string Value { get; private set; }

        public static DeviceModeClass Serial { get { return new DeviceModeClass("Serial Connection"); } }
        public static DeviceModeClass REST { get { return new DeviceModeClass("REST Connection"); } }
        public static DeviceModeClass NotConnected { get { return new DeviceModeClass("Not Connected"); } }



    }

    


    public enum DeviceMode : int { Serial, REST, NotConnected, Other }
    public enum DeviceState : int { Off, On, Pulse, Reset }
    public enum DetectMode : int { Manual, Camera, Microphone, CameraMicrophone }



}
