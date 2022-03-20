
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Reflection;

using RestSharp;
using System.Threading;


namespace OnAir
{


    // Class to to control the device via serial port using simple handshaking
    // The device is controlled by sending a simple single byte - 48=full, 49=pulse or 50=off
    // The response to a succesful send will be 2 x bytes "OK"

    internal static class SerialController
    {
                
        static SerialPort serialPort;
        static string endOfString = "\r\n";
        static readonly object _commandLock = new object();
        static int readWriteTimeout = 1000; // [ms]
        static int waitForTransmissionTimeout = 2; // [s]
        static string rxData = "", endOfString1 = "" + char.MinValue;
        static bool WaitingForSerialData = false; // Set in SerialWrite(), cleared in SerialRead()
        public static String _portName { get; set; }
        public static int _baudRate { get; set; }
        public static int _writeTimout = 1000;
        public static int _readTimout = 2000;
        private static String buffer = string.Empty;
        
        public static String deviceHostName { get; set; }

        // These three things control the state of the device and also what the state of the UI requires.
        public static DeviceState deviceState = DeviceState.Off;
        public static DeviceMode deviceMode = DeviceMode.NotConnected;   // Start with no mode
        public static DeviceState displayMode = DeviceState.Off;
        public static DetectMode detectMode = DetectMode.Manual;        


        public static async Task<RestSharp.RestResponse> OnAirDeviceRESTAsync(DeviceState mode)
        {

            string path = Assembly.GetExecutingAssembly().Location;
            string name = Path.GetFileName(path);
            RestRequest request=null;

            string restdevice = ConfigurationManager.AppSettings["restdevice"];

            var client = new RestClient("http://" + restdevice);
            if (mode == DeviceState.Off)                
            {
                request = new RestRequest("setOff", Method.Get);
            } else if (mode == DeviceState.On)
            {
                request = new RestRequest("setOn", Method.Get);
            }
            else if (mode== DeviceState.Pulse)
            {        
                request = new RestRequest("setPulse", Method.Get);
            }
        
            var response = await client.ExecuteGetAsync(request);

            Console.WriteLine(response.ResponseStatus.ToString());
            Console.WriteLine(response.ToString());

            var result = response;

            return result;

        }

    public static async Task<bool> OnAirDeviceAsync(DeviceState mode)
        {
            
            // If the device is connected via serial - we do that 
            string comport = ConfigurationManager.AppSettings["serialdevice"];
            string baudrate = ConfigurationManager.AppSettings["serialbaud"];
            SerialController._baudRate = Int32.Parse(baudrate);
            SerialController._portName = comport;

            String commandByte="9";
            
            if (mode==DeviceState.Off)
            {
                
                if (deviceState == DeviceState.Off)
                    return false;
                deviceState = mode;
                commandByte = "0";
            }
            else if (mode== DeviceState.On )
            {
                if (deviceState == DeviceState.On) 
                    return false;
                commandByte= "1";
                deviceState = mode;

            }
            else if (mode == DeviceState.Pulse)
            {
                if (deviceState == DeviceState.Pulse)
                    return false;

                deviceState = mode;
                commandByte = "2";
            }
            else if (mode == DeviceState.Reset)
            {
                deviceState = DeviceState.Off;
                mode = DeviceState.Off;
                commandByte = "0";
            }
                     

            try
            {
                if (deviceMode == DeviceMode.REST)
                {
                    var response = await OnAirDeviceRESTAsync(mode);
                    Console.WriteLine(response.Content.ToString());
                }
                else
                {

                    SerialController.OpenPort();
                    String result = SerialController.SendCommand(commandByte);
                    Console.WriteLine(result);
                }
            }
            catch (Exception tex)

            {
                Console.WriteLine("Failed");
                Console.WriteLine(tex);
                throw new Exception(Properties.Resources.serialtimeout);
                


            }
            finally
            {
                if (deviceMode == DeviceMode.Serial)
                {
                    SerialController.ClosePort();
                    
                }
                
            }

            return true;

        }

        public static void OpenPort()
        {
            serialPort = new SerialPort(_portName, _baudRate);
            try
            {
                
                serialPort.Open();
                //serialPort.DtrEnable = true;
                //serialPort.RtsEnable = true;
                serialPort.ReadTimeout= 2000;
                serialPort.WriteTimeout = 1000;

                //serialPort.DataReceived += dataReceived;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERRROR: {e.Message}");
            }

            return;
        }
        public static void ClosePort()
        {
            try
            {
                serialPort.Close();
                serialPort.Dispose();
                serialPort = null;
            }
            catch (Exception closePortError)
            {
                Console.WriteLine(closePortError);
            }
            
            
            return;
        }

        private static void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Bytes to read: ", serialPort.BytesToRead);

            try
            {

                buffer += serialPort.ReadExisting();


                //test for termination character in buffer
                if (buffer.Contains("\r\n"))
                {
                    Console.WriteLine("Recieve Buffer: " + buffer);
                    //run code on data received from serial port
                }

                serialPort.Close();
            }
            catch (TimeoutException)
            {
                throw new Exception(Properties.Resources.serialtimeout);
            }

        }

        public static string SendCommand(string text)
        {
            lock (_commandLock)
            {
                Console.WriteLine("Text to send: "+text);
                SerialWrite(text);
                //return "SENT";
                return SerialRead();
            }
        }

        private static string SerialRead()
        {
            try
            {

                String buff = serialPort.ReadTo(endOfString);
                Console.WriteLine("Buffer: " + buff);
                return buff;
            }
            catch (TimeoutException)
            {
                throw new Exception(Properties.Resources.serialtimeout);
            }
        }

        private static void SerialWrite(string text)
        {
            try
            {
                serialPort.Write(text);
            }
            catch (TimeoutException)
            {
                throw new Exception(Properties.Resources.serialtimeout);
            }
        }


    }
}
