using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcTool.Logic
{
    public class Message
    {
        /// <summary>
        /// Meddelandetyper, som PC-mjukvaran kan motta
        /// </summary>
        public enum RecieveType
        {
            PARAM_CHANGED_CONFIRM = 1,
            DEBUG_DATA = 2,
            MAP_DATA = 3
        }
         
        /// <summary>
        /// Meddelandetyper, som PC-mjukvaran kan skicka
        /// </summary>
        public enum SendType
        {
            MANUAL_COMMAND = 1,
            CHANGE_PARAM = 2,
            EMERGENCY_STOP = 4
        }

        /// <summary>
        /// Konstruerar ett meddelande som ska skickas till roboten
        /// </summary>
        public Message(SendType type, byte[] param = null)
        {
            Type = (byte)type;
            Param = (param==null)?new byte[0]:param;
        }

        /// <summary>
        /// Konstruerar ett meddelande från en bottagen byte-array
        /// </summary>
        /// <param name="msg">< param>
        public Message(byte[] msg)
        {
            Type = (byte)((msg[0] & 224)>>5);
            Param = new byte[msg.Length - 1];
            Array.Copy(msg, 1, Param, 0, msg.Length - 1);
        }

        /// <summary>
        /// Den typ som detta meddelande är av
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Parametern i detta meddelande
        /// </summary>
        public byte[] Param { get; set; }

        /// <summary>
        /// Returnerar meddelandet som en byte-array
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[Param.Length + 1];
            bytes[0] = (byte)((Type << 5) | Param.Length);
            for (int i = 1; i <= Param.Length; i++)
                bytes[i] = Param[i - 1];
            return bytes;
        }

    }
}
