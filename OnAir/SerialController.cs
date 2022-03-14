using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public  static int _writeTimout=1000;
        public static int _readTimout = 2000;
        private static String buffer = string.Empty;
        public static int deviceState { get; set; }


    public static Boolean OnAirDevice(int mode)
        {

            // If the device is connected via serial - we do that 
            string comport = ConfigurationManager.AppSettings["serialdevice"];
            string baudrate = ConfigurationManager.AppSettings["serialbaud"];
            SerialController._baudRate = Int32.Parse(baudrate);
            SerialController._portName = comport;

            String commandByte="9";
            

            if (mode==0)
            {
                commandByte = "0";
                deviceState = mode;
            }
            else if (mode==1 )
            {
                if (deviceState == 1) 
                    return false;
                commandByte= "1";
            }
            else if (mode == 2)
            {
                if (deviceState == 1)
                    return false;
                commandByte = "2";
            }

            deviceState = mode;

            try
            {
                SerialController.OpenPort();
                String result = SerialController.SendCommand(commandByte);
                Console.WriteLine(result);

            }
            catch (Exception tex)

            {
                Console.WriteLine("Failed");
                Console.WriteLine(tex);
                throw new Exception(Properties.Resources.serialtimeout);
                


            }
            finally
            {
                SerialController.ClosePort();
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
