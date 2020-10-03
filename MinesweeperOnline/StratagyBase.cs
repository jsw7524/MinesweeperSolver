using System.Data;
using System.Linq;

namespace MinesweeperOnline
{
    public class StratagyBase : IStratagy
    {
        public bool Evaluate(Board b)
        {
            bool isChanged = false;
            foreach (var row in b.map)
            {
                foreach (var c in row)
                {
                    switch (c.cellType)
                    {
                        case CellType.Number:
                            var sur = b.GetCellsSurroundYX(c.posY, c.posX);
                            decimal tatalUnknown = sur.Where(k => CellType.Unknown == k.cellType).Count();
                            decimal tatalMines = sur.Where(k => CellType.Mine == k.cellType).Count();
                            if (0 == tatalUnknown)
                            {
                                continue;
                            }
                            foreach (var s in sur.Where(k => CellType.Unknown == k.cellType))
                            {
                                if (0 == s.mineProbability || 0 == (c.indicator - tatalMines))
                                {
                                    s.mineProbability = 0;
                                    continue;
                                }

                                if (null == s.mineProbability || s.mineProbability < ((c.indicator - tatalMines) / tatalUnknown))
                                {
                                    s.mineProbability = (c.indicator - tatalMines) / tatalUnknown;
                                    if (s.mineProbability >= 1)
                                    {
                                        s.cellType = CellType.Mine;
                                        isChanged = true;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return isChanged;
        }
    }
}
