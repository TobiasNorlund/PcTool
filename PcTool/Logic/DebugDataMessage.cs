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
            { 13, "y"},
            {14, "theta"},
            {16, "Y est. forw."},
            {17, "thetaReg"},
            {18, "Long front"},
            {19, "xRelReg"},
            {20, "relYnew"},
            {21, "vinkelhst. hjul"},
            {22, "syncSpike"},
            {23, "x pos"},
            {24, "y pos"},
            {25, "y sum"},
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
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                        case 20:
                        case 25:
                            Int16 gyro = BitConverter.ToInt16(new byte[2] {msg[i + 2], msg[i + 1]},0);// little endian på pc:n big endian på avr, dvs reverse bitt order
                            Data.Add(DebugDataNamesLookup[msg[i]], gyro);
                            i++;
                            break;
                        case 12:
                        case 13:
                        case 21:
                            Data.Add(DebugDataNamesLookup[msg[i]], (sbyte)msg[i + 1] );
                            break;
                        case 14:
                            Data.Add(DebugDataNamesLookup[msg[i]], (sbyte)msg[i + 1]);
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
                                // Vi har inget namn på datat, skriv IDt bara
                                if (!Data.Keys.Contains(msg[i].ToString()))
                                {
                                    Data.Add(msg[i].ToString(), msg[i + 1]);
                                }
                                else
                                {
                                    Data[msg[i].ToString()] = msg[i + 1];
                                }
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
