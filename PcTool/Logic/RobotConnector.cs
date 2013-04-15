﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.IO.Ports;

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
        VirtualTurnaround = 5,
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

        // Handshake
        private static byte[] HANDSHAKE = new byte[2] {0xFF, 0xFF};
        private static int currentHandshakePos = 0;
        private static bool isHandshaked = false;

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

        /// <summary>
        /// Returnerar om en handskakning är utförd. En handskakning måste utföras innan meddelanden kan böra skickas/mottas
        /// </summary>
        public static bool IsHandshaked
        {
            get { return isHandshaked; }
        }

        #endregion

        public static void Connect()
        {
            port = new SerialPort("COM4", 115200, Parity.None, 8, StopBits.One);
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            port.Open();
            
            if (ConnectionChanged != null)
                ConnectionChanged();
        }

        public static void Disconnect()
        {
            port.Close();

            isHandshaked = false;
            currentHandshakePos = 0;

            if (ConnectionChanged != null)
                ConnectionChanged();
        }

        // TEMP
        //public static void SendByte(byte b)
        //{
        //    byte[] bytes = new byte[1];
        //    bytes[0] = b;
        //    port.Write(bytes, 0, 1);
        //}

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

#region Privat funktionalitet

        private static void ParseMessage()
        {
            for (int i = 0; i < messageBuffer.Length; i++)
                if (newByte != null)
                    newByte(messageBuffer[i]);

            // Läs ut typen
            Message.RecieveType messageType = (Message.RecieveType)((messageBuffer[0] & 224)>>5);

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

        private static void SendMessage(Message message)
        {
            byte[] bytes = message.GetBytes();
            port.Write(bytes, 0, bytes.Length);
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
            // Test för att kolla vilka bytes som kommer
            //while (port.BytesToRead > 0)
            //    if (newByte != null)
            //        newByte((byte)port.ReadByte());
            
            // Läs av handshake om det inte redan gjorts
            if (!isHandshaked)
            {
                while (!isHandshaked && port.BytesToRead > 0){
                    if (port.ReadByte() == HANDSHAKE[currentHandshakePos])
                    {
                        currentHandshakePos++;
                        if (currentHandshakePos >= HANDSHAKE.Length)
                            isHandshaked = true;
                    }
                    else
                    {
                        // En felaktig byte var mottagen, börja om
                        currentHandshakePos = 0;
                    }
                }
                if (!isHandshaked)
                    return;
            }

            // Om det är början på ett nytt meddelande
            if (currentRemainingBytes == 0)
            {
                byte firstByte = (byte)port.ReadByte();
                int length = (int)(firstByte & 31);
                messageBuffer = new byte[length + 1];
                messageBuffer[0] = firstByte;
                currentRemainingBytes = (port.BytesToRead < length)?length - port.BytesToRead:0;
                port.Read(messageBuffer, 1, (port.BytesToRead < length) ? port.BytesToRead : length);

                // Om vi har ett helt meddelande
                if(currentRemainingBytes==0)
                    ParseMessage();
                
                // Om det finns bytes kvar i buffern, kör funktionen igen
                if (port.BytesToRead > 0)
                    DataReceivedHandler(sender, e);
                
            }
            else
            {
                // Om buffern innehåller mindre eller lika många bytes som saknas för meddelandet
                if (port.BytesToRead <= currentRemainingBytes)
                {
                    int bytesToRead = port.BytesToRead;
                    port.Read(messageBuffer, messageBuffer.Length - currentRemainingBytes, port.BytesToRead);
                    currentRemainingBytes -= bytesToRead;

                    // Om hela meddelandet nu är hämtat
                    if (currentRemainingBytes == 0)
                        ParseMessage();
                }
                else
                {
                    // Om buffern innehåller FLER bytes av vad som behövs till meddelandet
                    int bytesToRead = currentRemainingBytes;
                    port.Read(messageBuffer, messageBuffer.Length - currentRemainingBytes, bytesToRead);
                    currentRemainingBytes = 0;

                    // Hela meddelandet är nu hämtat
                    ParseMessage();

                    // Börja på nytt meddelande...
                    DataReceivedHandler(sender, e);
                }
            }
        }

#endregion

    }
}
