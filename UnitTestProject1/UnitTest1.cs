﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinesweeperOnline;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        Board CreateTestBoard()
        {
            Board board = new Board(3, 3,2);
            board.AddCell(new Cell(CellType.Unknown, -1, 0, 0));
            board.AddCell(new Cell(CellType.Unknown, -1, 0, 1));
            board.AddCell(new Cell(CellType.Unknown, -1, 0, 2));
            board.AddCell(new Cell(CellType.Unknown, -1, 1, 0));
            board.AddCell(new Cell(CellType.Unknown, -1, 1, 1));
            board.AddCell(new Cell(CellType.Unknown, -1, 1, 2));//mine
            board.AddCell(new Cell(CellType.Unknown, -1, 2, 0));
            board.AddCell(new Cell(CellType.Unknown, -1, 2, 1));//mine
            board.AddCell(new Cell(CellType.Number, 2, 2, 2));
            return board;
        }

        Board LoadTestBoard1()
        {
            WebOperator webOperator = new WebOperator(new ChromeDriver(), @"file:///C:/Users/netsp/source/repos/MinesweeperOnline/UnitTestProject1/bin/Debug/Game435842338.html");
            var data = webOperator.driver.FindElement(By.Id("A43")).FindElements(By.CssSelector(".cell"));
            Board board = new Board(9, 9,10);
            Board result = webOperator.MakeBoard(data, board);
            webOperator.driver.Quit();
            return result;
        }

        [TestMethod]
        public void TestMethod1()
        {
            Board b = CreateTestBoard();
            Cell c = b.GetCell(2, 2);
            Assert.AreEqual(2, c.indicator);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Board b = CreateTestBoard();
            Cell c = b.GetCell(1, 1);
            Assert.AreEqual(CellType.Unknown, c.cellType);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Board b = CreateTestBoard();
            IEnumerable<Cell> cs = b.GetCellsSurroundYX(2, 2);
            Assert.AreEqual(3, cs.Count());
        }
        [TestMethod]
        public void TestMethod4()
        {
            Board b = CreateTestBoard();
            IEnumerable<Cell> cs = b.GetCellsSurroundYX(1, 1);
            Assert.AreEqual(8, cs.Count());
        }

        [TestMethod]
        public void TestMethod5()
        {
            Board b = CreateTestBoard();
            StratagyBase stratagyBase = new StratagyBase();
            stratagyBase.Evaluate(b);
            Assert.AreEqual((decimal)2 / 3, b.map[1][1].mineProbability);
        }

        [TestMethod]
        public void TestMethod6()
        {
            Board b = LoadTestBoard1();
            StratagyBase stratagyBase = new StratagyBase();
            stratagyBase.Evaluate(b);
        }

    }
}
