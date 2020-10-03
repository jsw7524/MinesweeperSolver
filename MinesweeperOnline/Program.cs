using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Deployment.Internal;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperOnline
{

    class Program
    {
        static void Main(string[] args)
        {
            WebOperator webOperator = new WebOperator(new ChromeDriver());
            int mineNumber = webOperator.GetMinesNumber();
            int BoardHeight = webOperator.GetBoardHeight();
            int BoardWidth = webOperator.GetBoardWidth();
            webOperator.ClickOnBoard(BoardHeight / 2 + 2, BoardWidth / 2 + 2);
            webOperator.ClickOnBoard(BoardHeight / 2 - 2, BoardWidth / 2 - 2);

            while (true)
            {
                Board board = new Board(BoardWidth, BoardHeight, mineNumber);
                var data = webOperator.driver.FindElement(By.Id("A43")).FindElements(By.CssSelector(".cell"));
                webOperator.MakeBoard(data, board);
                MinesweeperSolver solver = new MinesweeperSolver();
                var nextMove = solver.BestMove(board);
                if (null == nextMove)
                {
                    break;
                }
                Debug.WriteLine($"{nextMove.posY} {nextMove.posX}");
                webOperator.ClickOnBoard(nextMove.posY, nextMove.posX);
            }
        }
    }
}
