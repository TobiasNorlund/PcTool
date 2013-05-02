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
            { 5, "Vänster fram"},
            { 6, "Höger bak"},
            { 7, "Vänster bak"},
            { 8, "Gyro"},
            { 9, "Rot. höger"},
            { 10, "Rot. vänster"},
            { 12, "x"},
            { 13, "max"},
            {14, "theta"}
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
            try
            {
                // Skapa Dictionary från Param
                Data = new Dictionary<string, int>();
                for (int i = 1; i < msg.Length; i+=2)
                {
                    switch (msg[i])
                    {
                        case 4: // Korta avståndssensorer (dela med 2)
                        case 5:
                        case 6:
                        case 7:
                            Data.Add(DebugDataNamesLookup[msg[i]], (int)msg[i + 1] / 2);
                            break;
                        case 8:
                            Int16 gyro = BitConverter.ToInt16(new byte[2] {msg[i + 2], msg[i + 1]},0);// little endian på pc:n big endian på avr, dvs reverse bitt order
                            Data.Add("Gyro", gyro);
                            i++;
                            break;
                        case 12:
                            Data.Add(DebugDataNamesLookup[msg[i]], (char)msg[i + 1] / 2);
                            break;
                        default:
                            if (DebugDataNamesLookup.Keys.Contains(msg[i]))
                            {
                                if (!Data.Keys.Contains(DebugDataNamesLookup[msg[i]]))
                                {
                                    Data.Add(DebugDataNamesLookup[msg[i]], msg[i + 1]);
                                }
                                else
                                {
                                    Data[DebugDataNamesLookup[msg[i]]] = msg[i + 1];
                                }
                            }
                            else
                            {
                                /*// Vi har inget namn på datat, skriv IDt bara
                                if (!Data.Keys.Contains(msg[i].ToString()))
                                {
                                    Data.Add(msg[i].ToString(), msg[i + 1]);
                                }
                                else
                                {
                                    Data[msg[i].ToString()] = msg[i + 1];
                                }*/
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public Dictionary<string, int> Data { get; private set; }
    }
}
