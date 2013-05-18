using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.ComponentModel;

namespace PcTool.Logic
{
#region Enums

    public enum ManualCommand
    {
        StraightOneBlock = 0,
        Rot90Right = 1,
        Rot90Left = 2,
        Rot45Right = 3,
        Rot45Left = 4,
        VirtualTurn = 5,
        Testdrive = 6
    }

    public enum ControlParam
    {
        L1_x = 0,
        L2_theta = 1,
        L3_omega = 2,
        L1_theta = 3,
        L2_omega = 4,
        PowerRightPair = 5,
        PowerLeftPair = 6,
    }

#endregion


    public static class RobotConnector
    {

        public delegate void DebugDataUpdateHandler(Dictionary<string, int> dict);
        public delegate void MapUpdateHandler(int x, int y, bool isFree);
        public delegate void ConnectionChangedHandler();

        public static event ConnectionChangedHandler ConnectionChanged;
        public static event DebugDataUpdateHandler DebugDataUpdate;
        public static event MapUpdateHandler MapUpdate;

        public delegate void TempHandler(byte b);
        public static event TempHandler newByte;

        private static SerialPort port;
        private static BackgroundWorker worker;

        // Handshake
        //private static byte[] HANDSHAKE = new byte[1] {0xFF};
        private static int currentHandshakeState = 0;
        //public static bool isHandshaked = false;

        // Meddelandespecifika variabler
        private static List<byte> messageBuffer = new List<byte>();
        private static int currentRemainingBytes;

        #region Public Properties

        /// <summary>
        /// Returns whether the software is connected to the robot or not
        /// </summary>
        public static bool IsConnected {
            get { return (port !=null)?port.IsOpen:false; }
        }

        /// <summary>
        /// Returnerar om en handskakning är utförd. En handskakning måste utföras innan meddelanden kan böra skickas/mottas
        /// </summary>
        /*public static bool IsHandshaked
        {
            get { return isHandshaked; }
        }*/

        #endregion

        public static void Connect()
        {
            if (port != null && port.IsOpen)
                return;

            port = new SerialPort(Properties.Settings.Default.COMport, Properties.Settings.Default.BaudRate, Parity.None, 8, StopBits.One);
            port.ReadTimeout = 200000;
            port.WriteTimeout = 200000;

            try
            {
                worker = new BackgroundWorker();
                worker.DoWork += ReadBluetooth;
                worker.WorkerSupportsCancellation = true;

                port.Open();

                worker.RunWorkerAsync();
                
                if (ConnectionChanged != null)
                    ConnectionChanged();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Fel: " + e.ToString());
            }
            
        }

        public static void Disconnect()
        {
            if(port.IsOpen)
                port.Close();

            worker.CancelAsync();

            currentHandshakeState = 0;
            currentRemainingBytes = 0;

            if (ConnectionChanged != null)
                ConnectionChanged();
        }


        public static void SendCommand(ManualCommand command)
        {
            Message m = new Message(Message.SendType.MANUAL_COMMAND, new Byte[1] { (byte)command });
            SendMessage(m);
        }

        public static void UpdateControlParam(ControlParam controlParam, byte value)
        {
            Message m = new Message(Message.SendType.CHANGE_PARAM, new Byte[2] { (byte)controlParam, value });
            SendMessage(m);
        }

        public static void SendEmergencyStop()
        {
            Message m = new Message(Message.SendType.EMERGENCY_STOP);
            SendMessage(m);
        }

#region Privat funktionalitet

        /// <summary>
        /// Parsar ett helt mottaget meddelande och reser rätt event
        /// </summary>
        private static void ParseMessage()
        {
            //for (int i = 0; i < messageBuffer.Length; i++)
            //    if (newByte != null)
                    //newByte(messageBuffer[i]);

            // Läs ut typen
            Message.RecieveType messageType = (Message.RecieveType)((messageBuffer[0] & 224)>>5);
            uint len = (uint)(messageBuffer[0] & 31);
            switch (messageType)
            {
                case Message.RecieveType.MAP_DATA:
                    if (len == 3)
                    {
                        MapMessage mapmessage = new MapMessage(messageBuffer.ToArray());
                        if (MapUpdate != null && mapmessage.x < 16 && mapmessage.y < 16 && 0 < mapmessage.x && 0 < mapmessage.y)
                        {
                            MapUpdate(mapmessage.x, mapmessage.y, mapmessage.isFree);
                        }
                    }
                    break;
                case Message.RecieveType.DEBUG_DATA:
                    DebugDataMessage ddmessage = new DebugDataMessage(messageBuffer.ToArray());
                    if (DebugDataUpdate != null)
                        DebugDataUpdate(ddmessage.Data);
                    break;
                // TODO: case mottagen styrparameter
                default:
                    // Unsupported, do nothing
                    break;
            }
        }

        /// <summary>
        /// Skickar iväg ett meddelande via COM-porten
        /// </summary>
        /// <param name="message"></param>
        private static void SendMessage(Message message)
        {
            try
            {
                byte[] bytes = message.GetBytes();
                port.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("Fel vid skickning av meddelande: \n\n" + e.Message);
            }
        }

#endregion

#region Eventhantering

        /// <summary>
        /// När en byte är mottagen från roboten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ReadBluetooth(object sender, DoWorkEventArgs e)
        {

            while (IsConnected && !worker.CancellationPending)
            {
                if (port.BytesToRead > 0)
                {
                    byte b = ReadByte();

                    if (currentRemainingBytes == 0)
                    {
                        byte firstByte = b;
                        int length = (int)(firstByte & 31);
                        messageBuffer = new List<byte>(length+1);

                        currentRemainingBytes = length;
                    }else
                        currentRemainingBytes--;

                    messageBuffer.Add(b);

                    // Om vi mottagit 5 0xFF i rad så vet vi att nästa byte som kommer är början på ett nytt meddelande
                    if (currentHandshakeState == 5)
                    {
                        currentRemainingBytes = 0;
                        currentHandshakeState = 0;
                    }
                    else if (currentRemainingBytes == 0) // Om hela meddelandet nu är hämtat
                    {
                        if(App.Current != null) // Den sätts till null om vi håller på och stänga programmet
                            App.Current.Dispatcher.Invoke(new Action(() => ParseMessage()), null);
                    }
                }else{
                    Thread.Sleep(1);
                }
            }
        }

        private static byte ReadByte()
        {
            byte b = (byte)port.ReadByte();
            currentHandshakeState = (b == 255) ? currentHandshakeState + 1 : 0;
            return b;
        }

#endregion

    }
}
