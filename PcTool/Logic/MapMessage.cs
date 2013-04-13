using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcTool.Logic
{
    class MapMessage : Message
    {
        /// <summary>
        /// Konstruerar ett meddelande från en mottagen byte-array
        /// </summary>
        /// <param name="msg">< param>
        public MapMessage(byte[] msg) : base(msg)
        {
            if (Type != (byte)Message.RecieveType.MAP_DATA)
                throw new InvalidOperationException("Kan inte skapa ett MapMessage från ett meddelande som inte är av denna typ");

            x = Param[0];
            y = Param[1];
            isFree = Param[2] == 1;
        }

        public int x { get; set; }
        public int y { get; set; }
        public bool isFree { get; set; }
    }
}
