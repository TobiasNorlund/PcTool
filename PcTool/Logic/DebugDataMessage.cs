using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcTool.Logic
{
    class DebugDataMessage : Message
    {
        #region Statiska variabler

        /// <summary>
        /// Lookup-tabell för att matcha debugdata/sensordata till namn
        /// </summary>
        private static Dictionary<byte, string> DebugDataNamesLookup = new Dictionary<byte, string>() {
            { 0, "Fram"},
            { 1, "Höger"},
            { 2, "Bak"},
            { 3, "Vänster"},
            { 4, "Höger fram"},
            { 5, "Höger bak"},
            { 6, "Vänster fram"},
            { 7, "Vänster bak"},
            { 8, "Gyro"},
            { 9, "Rot. höger"},
            { 10, "Rot. vänster"}
        };

        #endregion
        
        /// <summary>
        /// Konstruerar ett meddelande från en mottagen byte-array
        /// </summary>
        /// <param name="msg">< param>
        public DebugDataMessage(byte[] msg) : base(msg)
        {
            if (Type != (byte)Message.RecieveType.DEBUG_DATA)
                throw new InvalidOperationException("Kan inte skapa ett DebugDataMessage från ett meddelande som inte är av denna typ");

            // Skapa Dictionary från Param
            Data = new Dictionary<string, int>();
            for (int i = 1; i < msg.Length; i+=2)
            {
                if (msg[i] == 8)
                {
                    Data.Add("Gyro 1", msg[i + 1]);
                    Data.Add("Gyro 2", msg[i + 2]);
                    i++;
                }
                else
                {
                    Data.Add(DebugDataNamesLookup[msg[i]], msg[i + 1]);
                }
            }

        }

        public Dictionary<string, int> Data { get; private set; }
    }
}
