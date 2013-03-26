using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InTheHand.Net.Sockets;
using System.Net.Sockets;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;

namespace PcTool.Logic
{
    public static class RobotConnector
    {

        public delegate void DebugDataUpdateHandler(Dictionary<string, int> dict);
        public delegate void MapUpdateHandler(int x, int y, bool isFree);

        public static event DebugDataUpdateHandler DebugDataUpdate;
        public static event MapUpdateHandler MapUpdate;

        private static BluetoothClient client;
        private static NetworkStream stream;

        private static byte[] buffer1;
        private static byte[] buffer2;

        public static void Connect()
        {
            BluetoothAddress addr = BluetoothAddress.Parse("001122334455");
            Guid serviceClass;
            serviceClass = BluetoothService.SerialPort;

            var ep = new BluetoothEndPoint(addr, serviceClass);
            client = new BluetoothClient();
            client.Connect(ep);
            stream = client.GetStream();

            ReadMessage();
        }

        public static void SendCommand(int command)
        {

        }

        public static void UpdateControlParam(int controlParam, int value)
        {

        }

        private static void ReadMessage()
        {
            stream.BeginRead(buffer1, 0, 1, new AsyncCallback(ReadCallback), null);
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            stream.EndRead(ar);

            int messageType = buffer1[0] & 248;
            int messageLength = buffer1[0] & 7;

            // Read param
            stream.Read(buffer2, 0, messageLength);

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
            }

            // Read next message
            stream.BeginRead(buffer1, 0, 1, new AsyncCallback(ReadCallback), null);
        }

    }
}
