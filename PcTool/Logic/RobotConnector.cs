using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.IO.Ports;

namespace PcTool.Logic
{
    public static class RobotConnector
    {

        public delegate void DebugDataUpdateHandler(Dictionary<string, int> dict);
        public delegate void MapUpdateHandler(int x, int y, bool isFree);
        public delegate void ConnectionChangedHandler();

        public static event ConnectionChangedHandler ConnectionChanged;
        public static event DebugDataUpdateHandler DebugDataUpdate;
        public static event MapUpdateHandler MapUpdate;

        private static SerialPort port;

        private static byte[] buffer1;
        private static byte[] buffer2;

        #region Public Properties

        /// <summary>
        /// Returns whether the software is connected to the robot or not
        /// </summary>
        public static bool IsConnected {
            get { return (port !=null)?port.IsOpen:false; }
        }

        #endregion

        public static void Connect()
        {
            port = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            port.Open();

            if (ConnectionChanged != null)
                ConnectionChanged();
        }

        // TEMP
        public static void SendByte(byte b)
        {
            byte[] bytes = new byte[1];
            bytes[0] = b;
            port.Write(bytes, 0, 1);
        }

        public static void SendCommand(int command)
        {

        }

        public static void UpdateControlParam(int controlParam, int value)
        {

        }


        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

/*
            int messageType = buffer1[0] & 248;
            int messageLength = buffer1[0] & 7;

            buffer2 = new byte[messageLength];

            switch (messageType)
            {
                case 1:
                    if (MapUpdate != null)
                        MapUpdate(buffer2[0], buffer2[1], buffer2[2] == 0);
                    break;
                case 2:
                    Dictionary<string, int> dict = new Dictionary<string, int>();
                    // ...
                    if (DebugDataUpdate != null)
                        DebugDataUpdate(dict);
                    break;
                default:
                    // Unsupported, do nothing
                    break;
            }*/
        }

    }
}
