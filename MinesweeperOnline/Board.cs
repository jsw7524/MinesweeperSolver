using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MinesweeperOnline
{
    public class Board
    {
        public List<List<Cell>> map;
        public int width;
        public int height;
        public int size;

        public int defaultMines;
        public Board(int w, int h, int m)
        {
            width = w;
            height = h;
            defaultMines = m;
            map = new List<List<Cell>>();
        }

        public void AddCell(Cell c)
        {
            if (size % width == 0)
            {
                map.Add(new List<Cell>());
            }
            map.Last().Add(c);
            size++;
        }
        public Cell GetCell(int y, int x)
        {
            if (y >= 0 && x >= 0 && y < height && x < width)
            {
                return map[y][x];
            }
            return null;
        }

        public IEnumerable<Cell> GetCellsSurroundYX(Cell c)
        {
            return GetCellsSurroundYX(c.posY, c.posX);
        }

        public IEnumerable<Cell> GetCellsSurroundYX(int y, int x)
        {
            List<Cell> result = new List<Cell>();
            result.Add(GetCell(y - 1, x - 1));
            result.Add(GetCell(y - 1, x));
            result.Add(GetCell(y - 1, x + 1));
            result.Add(GetCell(y, x - 1));
            result.Add(GetCell(y, x + 1));
            result.Add(GetCell(y + 1, x - 1));
            result.Add(GetCell(y + 1, x));
            result.Add(GetCell(y + 1, x + 1));
            return result.Where(r => null != r);
        }
    }
}
