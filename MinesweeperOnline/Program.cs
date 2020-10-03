using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Deployment.Internal;
using System.Diagnostics;
using System.IO;
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

    public interface IStratagy
    {
        bool Evaluate(Board b);
    }

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

    public class StratagyRemainedMines : IStratagy
    {
        public int defaultMines;

        public StratagyRemainedMines(int m)
        {
            defaultMines = m;
        }

        public bool Evaluate(Board b)
        {
            int remainedMines = defaultMines - b.map.SelectMany(a => a).Where(k => CellType.Mine == k.cellType).Count();
            int nullProbability = b.map.SelectMany(a => a).Where(k => CellType.Unknown == k.cellType && null == k.mineProbability).Count();
            foreach (var row in b.map)
            {
                foreach (var c in row)
                {
                    if (CellType.Empty == c.cellType && null == c.mineProbability)
                    {
                        c.mineProbability = (decimal)remainedMines / nullProbability;
                    }
                }
            }
            return false;
        }
    }

    public class MinesweeperSolver
    {
        public List<IStratagy> stratagies = new List<IStratagy>();

        public MinesweeperSolver()
        {
            stratagies.Add(new StratagyBase());
            stratagies.Add(new StratagyRemainedMines(10));
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

    public class WebOperator
    {
        public IWebDriver driver;
        public WebOperator(IWebDriver d, string targetURL = @"https://minesweeper.online/")
        {
            driver = d ?? new ChromeDriver();
            driver.Navigate().GoToUrl(targetURL);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            Thread.Sleep(1000);
        }
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

        public void ClickOnBoard(Cell c)
        {
            ClickOnBoard(c.posY, c.posX);
        }
        public void ClickOnBoard(int y, int x)
        {
            var target = driver.FindElement(By.Id($"cell_{x}_{y}"));
            target.Click();
            Thread.Sleep(500);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            WebOperator webOperator = new WebOperator(new ChromeDriver());
            webOperator.ClickOnBoard(6, 6);
            webOperator.ClickOnBoard(2, 2);

            while (true)
            {
                Board board = new Board(9, 9);
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
