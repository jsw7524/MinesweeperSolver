using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace MinesweeperOnline
{
    public class WebOperator
    {
        public IWebDriver driver;

        public WebOperator(IWebDriver d, string targetURL = @"https://minesweeper.online/")
        {
            driver = d ?? new ChromeDriver();
            driver.Navigate().GoToUrl(targetURL);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            Thread.Sleep(500);
        }

        public int GetBoardWidth()
        {
            var col = driver.FindElement(By.Id("A43")).FindElements(By.CssSelector(".cell")).Count() / GetBoardHeight();
            return col;
        }

        public int GetBoardHeight()
        {
            var rows = driver.FindElement(By.Id("A43")).FindElements(By.CssSelector(".clear")).Count();
            return rows;
        }

        public int GetMinesNumber()
        {
            Regex regexDigit = new Regex("top-area-num(?<num>\\d)");
            var num100 = driver.FindElement(By.Id("top_area_mines_100")).GetAttribute("class");
            var num10 = driver.FindElement(By.Id("top_area_mines_10")).GetAttribute("class");
            var num1 = driver.FindElement(By.Id("top_area_mines_1")).GetAttribute("class");
            Match match100 = regexDigit.Match(string.Join("", num100));
            Match match10 = regexDigit.Match(string.Join("", num10));
            Match match1 = regexDigit.Match(string.Join("", num1));
            return int.Parse(match100.Groups["num"].Value + match10.Groups["num"].Value + match1.Groups["num"].Value);
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
            Thread.Sleep(100);
        }
    }
}
