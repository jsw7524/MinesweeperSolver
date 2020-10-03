using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MinesweeperOnline
{
    public class MinesweeperSolver
    {
        public List<IStratagy> stratagies = new List<IStratagy>();

        public MinesweeperSolver()
        {
            stratagies.Add(new StratagyBase());
            stratagies.Add(new StratagyRemainedMines());
        }

        public Cell BestMove(Board board)
        {
            foreach (IStratagy sgt in stratagies)
            {
                bool chk = true;
                while (chk)
                {
                    chk = sgt.Evaluate(board);
                }
            }
            return board.map.SelectMany(a => a).Where(b => null != b.mineProbability && CellType.Unknown == b.cellType).OrderBy(c => c.mineProbability).FirstOrDefault();
        }
    }
}
