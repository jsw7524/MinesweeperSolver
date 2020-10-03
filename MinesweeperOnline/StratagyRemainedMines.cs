using System.Data;
using System.Linq;

namespace MinesweeperOnline
{
    public class StratagyRemainedMines : IStratagy
    {

        public bool Evaluate(Board b)
        {
            int remainedMines = b.defaultMines - b.map.SelectMany(a => a).Where(k => CellType.Mine == k.cellType).Count();
            int nullProbability = b.map.SelectMany(a => a).Where(k => CellType.Unknown == k.cellType && null == k.mineProbability).Count();
            foreach (var row in b.map)
            {
                foreach (var c in row)
                {
                    if (CellType.Empty == c.cellType && null == c.mineProbability)
                    {
                        c.mineProbability = (decimal)remainedMines / nullProbability;
                        //c.mineProbability -= b.GetCellsSurroundYX(c).Where(h => CellType.Unknown == h.cellType).Count() * 0.001m;
                    }
                }
            }
            return false;
        }
    }
}
