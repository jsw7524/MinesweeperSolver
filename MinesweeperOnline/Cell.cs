using System;

namespace MinesweeperOnline
{
    public enum CellType
    {
        Empty,
        Unknown,
        Flag,
        Mine,
        Number
    }
    public class Cell
    {
        public CellType cellType;
        public Decimal? mineProbability;
        public int posY;
        public int posX;
        public int indicator;
        public Cell(CellType t = CellType.Unknown, int i = 0, int y = -1, int x = -1)
        {
            cellType = t;
            posY = y;
            posX = x;
            indicator = i;
            mineProbability = null;
        }
    }
}
