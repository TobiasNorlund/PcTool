using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcTool.Logic
{
    /// <summary>
    /// Hanterar och representerar kartan
    /// </summary>
    class MapHandler
    {

        public MapHandler()
        {
            map = new int[16,16];
            walls = new bool[16, 16, 2];
        }

        public delegate void PositionUpdatedDelegate(int x, int y, bool isFree);
        public delegate void WallDetectedDelegate(int x, int y, bool isX);
        public event PositionUpdatedDelegate PositionUpdated;
        public event WallDetectedDelegate WallDetected;
        public event Action Cleared;

        private int[,] map { get; set; } // 16x16: 0 ej känd, 1 tillgänglig, 2 otillgänglig 
        private bool[,,] walls { get; set; } // 16x16: 3:e dim ([x,y,?]): Första pos är vägg längst y-axeln, andra är vägg längs x-axeln

        public void UpdatePosition(int x, int y, bool isFree)
        {
            map[x, y] = (isFree) ? 1 : 2;
            if (PositionUpdated != null)
                PositionUpdated(x-8, y-8, isFree);

            // Kolla om en ny vägg är upptäckt
            if (((isFree && map[x + 1, y] == 2) || (!isFree && map[x + 1, y] == 1)) && !walls[x + 1, y, 0])
            {
                walls[x + 1, y, 0] = true;
                if (WallDetected != null)
                    WallDetected(x + 1 -8, y -8, false);
            }
            if (((isFree && map[x - 1, y] == 2) || (!isFree && map[x - 1, y] == 1)) && !walls[x, y, 0])
            {
                walls[x, y, 0] = true;
                if (WallDetected != null)
                    WallDetected(x - 8, y - 8, false);
            }
            if (((isFree && map[x, y + 1] == 2) || (!isFree && map[x, y + 1] == 1)) && !walls[x, y + 1, 1])
            {
                walls[x, y + 1, 1] = true;
                if (WallDetected != null)
                    WallDetected(x - 8, y + 1 - 8, true);
            }
            if (((isFree && map[x, y - 1] == 2) || (!isFree && map[x, y - 1] == 1)) && !walls[x, y, 1])
            {
                walls[x, y, 1] = true;
                if (WallDetected != null)
                    WallDetected(x - 8, y - 8, true);
            }
        }

        public void Clear()
        {
            map = new int[16, 16];
            walls = new bool[16, 16, 2];

            if (Cleared != null)
                Cleared();
        }

    }
}
