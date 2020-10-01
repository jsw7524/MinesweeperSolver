using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Deployment.Internal;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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

    public class Board
    {
        public List<List<Cell>> map;
        public int width;
        public int height;
        public int size;
        public Board(int w, int h)
        {
            width = w;
            height = h;
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
    }

    public interface IStratagy
    {
        void Evaluate(Board b);
    }

    public class StratagyBase : IStratagy
    {
        Board board;
        public StratagyBase(Board b)
        {
            board = b;

        }
        public void Evaluate(Board b)
        {

        }
    }



    public class MinesweeperSolver
    {
        public List<IStratagy> stratagies = new List<IStratagy>();

        public Cell BestMove(Board board)
        {
            foreach (IStratagy sgt in stratagies)
            {
                sgt.Evaluate(board);
            }
            return board.map.SelectMany(a => a).Where(b => null != b.mineProbability).OrderBy(c => c).FirstOrDefault();
        }
    }


    public class WebOperator
    {
        public Board MakeBoard(ReadOnlyCollection<IWebElement> data, Board b)
        {
            Board board = b;
            Regex regexDigit = new Regex("hd_type(?<num>\\d)");
            foreach (var d in data)
            {
                var css = d.GetAttribute("class").Split(' ');
                Cell c;
                int x = int.Parse(d.GetAttribute("data-x"));
                int y = int.Parse(d.GetAttribute("data-y"));
                if (css.Contains("hd_flag"))
                {
                    c = new Cell(CellType.Flag, 0, y, x);
                }
                else if (css.Any(s => s.Contains("hd_type")))
                {
                    Match m = regexDigit.Match(string.Join("", css));
                    c = new Cell(CellType.Number, int.Parse(m.Groups["num"].Value), y, x);
                }
                else
                {
                    c = new Cell(CellType.Unknown, -1, y, x);
                }
                board.AddCell(c);
            }
            return board;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(@"https://minesweeper.online/");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            Thread.Sleep(1);

            //var faceIcon = driver.FindElement(By.Id("top_area_face"));
            //faceIcon.Click();
            //Thread.Sleep(1);

            var cell11 = driver.FindElement(By.Id("cell_1_1"));

            Thread.Sleep(1000);

            cell11.Click();


            Thread.Sleep(1000);
            var cell22 = driver.FindElement(By.Id("cell_2_2"));

            Thread.Sleep(1000);

            cell22.Click();
            Thread.Sleep(1000);

            var data = driver.FindElement(By.Id("A43")).FindElements(By.CssSelector(".cell"));

            /////////////////////
            Board board = new Board(9, 9);
            WebOperator webOperator = new WebOperator();

            webOperator.MakeBoard(data, board);


            //////////////////
            ////driver.Quit();
        }
    }
}
