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

        // Meddelandespecifika variabler
        private static byte[] messageBuffer;
        private static int currentRemainingBytes;

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

#region Privat funktionalitet

        private static void ParseMessage()
        {
            // Läs ut typen
            Message.RecieveType messageType = (Message.RecieveType)(messageBuffer[0] & 224);

            switch (messageType)
            {
                case Message.RecieveType.MAP_DATA:
                    MapMessage mapmessage = new MapMessage(messageBuffer);
                    if (MapUpdate != null)
                        MapUpdate(mapmessage.x, mapmessage.y, mapmessage.isFree);
                    break;
                case Message.RecieveType.DEBUG_DATA:
                    DebugDataMessage ddmessage = new DebugDataMessage(messageBuffer);
                    if (DebugDataUpdate != null)
                        DebugDataUpdate(ddmessage.Data);
                    break;
                // TODO: case mottagen styrparameter
                default:
                    // Unsupported, do nothing
                    break;
            }
        }

#endregion

#region Eventhantering

        /// <summary>
        /// När en byte är mottagen från roboten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            // Om det är början på ett nytt meddelande
            if (currentRemainingBytes == 0)
            {
                byte firstByte = (byte)port.ReadByte();
                int length = (int)(firstByte & 31);
                messageBuffer = new byte[length + 1];
                messageBuffer[0] = firstByte;
                currentRemainingBytes = length;// - port.BytesToRead;
                //port.Read(messageBuffer, 1, (port.BytesToRead < length) ? port.BytesToRead : length);
            }
            else
            {
                currentRemainingBytes -= 1; //port.BytesToRead;
                port.Read(messageBuffer, messageBuffer.Length - currentRemainingBytes, 1);

                // Om hela meddelandet nu är hämtat
                if (currentRemainingBytes == 0)
                    ParseMessage();
            }
        }

#endregion

    }
}
